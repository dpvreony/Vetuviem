﻿// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Configuration;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator
{
    /// <summary>
    /// Base logic for a source generator.
    /// </summary>
    /// <typeparam name="TGeneratorProcessor">The type for the generator processor.</typeparam>
    public abstract class AbstractBaseSourceGenerator<TGeneratorProcessor> : ISourceGenerator
        where TGeneratorProcessor : AbstractGeneratorProcessor, new()
    {
        /// <inheritdoc />
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc />
        public void Execute(GeneratorExecutionContext context)
        {
            var configurationModel = GetConfiguration(context);
            GenerateFromAssemblies(context, configurationModel);
            GenerateFromProjectSourceCode(context, configurationModel);
        }

        private static ConfigurationModel GetConfiguration(GeneratorExecutionContext context) =>
            ConfigurationFactory.Create(context);

        private void GenerateFromProjectSourceCode(
            GeneratorExecutionContext context,
            ConfigurationModel configurationModel)
        {
            var compilation = context.Compilation;
            var syntaxTrees = compilation.SyntaxTrees;
            foreach (var syntaxTree in syntaxTrees)
            {
                var semanticModel = compilation.GetSemanticModel(syntaxTree);
            }
        }

        private void GenerateFromAssemblies(
            GeneratorExecutionContext context,
            ConfigurationModel configurationModel)
        {
            try
            {
                context.ReportDiagnostic(ReportDiagnosticFactory.StartingSourceGenerator());

                var memberDeclarationSyntax = Generate(context, configurationModel,  context.CancellationToken);

                var nullableDirectiveTrivia = SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true);
                var trivia = SyntaxFactory.Trivia(nullableDirectiveTrivia);
                var leadingSyntaxTriviaList = SyntaxFactory.TriviaList(trivia);

                if (memberDeclarationSyntax == null)
                {
                    return;
                }

                memberDeclarationSyntax = memberDeclarationSyntax.WithLeadingTrivia(leadingSyntaxTriviaList);

                var parseOptions = context.ParseOptions;

                var cu = SyntaxFactory.CompilationUnit()
                    .AddMembers(memberDeclarationSyntax)
                    .NormalizeWhitespace();

                var feature = typeof(TGeneratorProcessor).ToString();

                var sourceText = SyntaxFactory.SyntaxTree(
                        cu,
                        parseOptions,
                        encoding: Encoding.UTF8)
                    .GetText();

                var hintName = $"{feature}.g.cs";

                context.AddSource(
                    hintName,
                    sourceText);
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                context.ReportDiagnostic(ReportDiagnosticFactory.UnhandledException(e));
            }
        }

        /// <summary>
        /// Gets the name of the platform used in the namespaces for the generated code.
        /// </summary>
        /// <returns>Name identifier for the platform.</returns>
        protected abstract string GetPlatformName();

        /// <summary>
        /// Gets the platform resolver used for searching for UI types for the platform.
        /// </summary>
        /// <returns>Platform specific resolver.</returns>
        protected abstract IPlatformResolver GetPlatformResolver();

        /// <summary>
        /// Works out if a assembly reference should be included if it's found to be missing.
        /// </summary>
        /// <param name="assemblyOfInterest">Name of the assembly.</param>
        /// <returns>A metadata reference for an assembly, if required.</returns>
        protected abstract MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest);

        /// <summary>
        /// Gets the root namespace to place the generated code inside.
        /// </summary>
        /// <param name="rootNamespace"></param>
        /// <returns>Fully qualified root namespace.</returns>
        protected abstract string GetNamespace(string? rootNamespace);

        /// <summary>
        /// Create the syntax tree representing the expansion of some member to which this attribute is applied.
        /// </summary>
        /// <param name="context">The transformation context being generated for.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The generated member syntax to be added to the project.</returns>
        private MemberDeclarationSyntax? Generate(
            GeneratorExecutionContext context,
            ConfigurationModel configurationModel,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var namespaceName = GetNamespace(configurationModel.RootNamespace);

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName));

            var compilation = context.Compilation;

            var platformResolver = GetPlatformResolver();

#if PLATFORMASSEMBLIES
            //// we work on assumption we have the references already in the build chain
            //var trustedAssembliesPaths = GetPlatformAssemblyPaths(context);
            //if (trustedAssembliesPaths == null || trustedAssembliesPaths.Length == 0)
            //{
            //    // we don't have the assemblies
            //    // we can fall back to searching for the reference assemblies path
            //    // or trigger nuget
            //    // for now we drop out
            //    return namespaceDeclaration;
            //}
#endif

            var assembliesOfInterest = GetAssembliesOfInterest(
                platformResolver,
                configurationModel.AssembliesArray,
                configurationModel.AssemblyMode);
            if (assembliesOfInterest.Length == 0)
            {
                return null;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var referencesOfInterest = GetReferencesOfInterest(
                compilation.References,
                assembliesOfInterest).ToArray();
            if (referencesOfInterest.Length != assembliesOfInterest.Length)
            {
                // not got the expected count back, drop out.
                context.ReportDiagnostic(ReportDiagnosticFactory.ReferencesOfInterestCountMismatch(
                    assembliesOfInterest.Length,
                    referencesOfInterest.Length));
                return namespaceDeclaration;
            }

            var desiredBaseType = platformResolver.GetBaseUiElement();
            var desiredNameWithoutGlobal = desiredBaseType.Replace(
                "global::",
                string.Empty);
            var desiredBaseTypeSymbolMatch = compilation.GetTypeByMetadataName(desiredNameWithoutGlobal);

            if (desiredBaseTypeSymbolMatch == null)
            {
                context.ReportDiagnostic(ReportDiagnosticFactory.FailedToFindDesiredBaseTypeSymbol(desiredBaseType));
                return namespaceDeclaration;
            }

            // blazor uses an interface, so we check once to drive different inheritance check.
            var desiredBaseTypeIsInterface = false;
            switch (desiredBaseTypeSymbolMatch.TypeKind)
            {
                case TypeKind.Interface:
                    desiredBaseTypeIsInterface = true;
                    break;
                case TypeKind.Class:
                    break;
                default:
                    context.ReportDiagnostic(ReportDiagnosticFactory.DesiredBaseTypeSymbolNotInterfaceOrClass(desiredBaseType));
                    return namespaceDeclaration;
            }

            var desiredCommandInterface = platformResolver.GetCommandSourceInterface();

            var generatorProcessor = new TGeneratorProcessor();

            var platformName = GetPlatformName();

            var result = generatorProcessor.GenerateNamespaceDeclaration(
                namespaceDeclaration,
                referencesOfInterest,
                compilation,
                context.ReportDiagnostic,
                desiredBaseType,
                desiredBaseTypeIsInterface,
                desiredCommandInterface,
                platformName,
                namespaceName,
                configurationModel.MakeClassesPublic,
                configurationModel.IncludeObsoleteItems,
                platformResolver.GetCommandInterface());

            return result;
        }

        private static string[] GetAssembliesOfInterest(
            IPlatformResolver platformResolver,
            IReadOnlyCollection<string>? assembliesArray,
            AssemblyMode assemblyMode)
        {
            var assembliesOfInterest = platformResolver.GetAssemblyNames();
            if (assembliesArray?.Count > 0)
            {
                switch (assemblyMode)
                {
                    case AssemblyMode.Replace:
                        assembliesOfInterest = assembliesArray.ToArray();
                        break;
                    case AssemblyMode.Extend:
                        assembliesOfInterest = assembliesOfInterest.Concat(assembliesArray).ToArray();
                        break;
                    default:
                        throw new InvalidOperationException("Invalid assembly mode.");
                }
            }

            return assembliesOfInterest;
        }

        private void ValidateRootNamespace(string? rootNamespace)
        {
            if (string.IsNullOrWhiteSpace(rootNamespace))
            {
                return;
            }

            // this is crude right now, look to see if roslyn can validate it, or produce a more expansive check.
            // todo: pass the csproj key and value back in the error.
            if (rootNamespace.Any(c => char.IsLetter(c) && c != '.'))
            {
                throw new InvalidOperationException("Root namespace in project config must be a valid namespace.");
            }
        }

        private IEnumerable<MetadataReference> GetReferencesOfInterest(
            IEnumerable<MetadataReference> compilationReferences,
            string[] assembliesOfInterest)
        {
            var compilationReferenceArray = compilationReferences.ToImmutableArray();

            foreach (var assemblyOfInterest in assembliesOfInterest)
            {
                var match = compilationReferenceArray.FirstOrDefault(
                    metaDataRef => metaDataRef.Display != null
                                          && metaDataRef.Display.EndsWith(
                                              assemblyOfInterest,
                                              StringComparison.Ordinal));
                if (match != null)
                {
                    yield return match;
                }
                else
                {
                    var itemToAdd = CheckIfShouldAddMissingAssemblyReference(assemblyOfInterest);
                    if (itemToAdd != null)
                    {
                        yield return itemToAdd;
                    }
                }
            }
        }
    }
}
