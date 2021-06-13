﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;
using Vetuviem.SourceGenerator.GeneratorProcessors;

namespace Vetuviem.SourceGenerator
{
    /// <summary>
    /// Base logic for a source generator.
    /// </summary>
    /// <typeparam name="TGeneratorProcessor"></typeparam>
    public abstract class AbstractBaseGenerator<TGeneratorProcessor> : ISourceGenerator
        where TGeneratorProcessor : AbstractGeneratorProcessor, new()
    {
        /// <inheritdoc />
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        /// <inheritdoc />
        public void Execute(GeneratorExecutionContext context)
        {
            try
            {
                context.ReportDiagnostic(ReportDiagnostics.StartingSourceGenerator());

                var memberDeclarationSyntax = GenerateAsync(context, CancellationToken.None);

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

                //var logRoot = SyntaxFactory.CompilationUnit();

                //var logFileSourceText = SyntaxFactory.SyntaxTree(
                //        logRoot,
                //        parseOptions,
                //        encoding: Encoding.UTF8)
                //    .GetText();

                // var logFileHintName = $"{feature}.{guid}.g.log.txt";
                var hintName = $"{feature}.g.cs";

                context.AddSource(
                    hintName,
                    sourceText);
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(ReportDiagnostics.UnhandledException(e));
            }
        }

        protected abstract string GetPlatformName();

        /// <summary>
        /// Create the syntax tree representing the expansion of some member to which this attribute is applied.
        /// </summary>
        /// <param name="context">The transformation context being generated for.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The generated member syntax to be added to the project.</returns>
        private MemberDeclarationSyntax GenerateAsync(
            GeneratorExecutionContext context,
            CancellationToken cancellationToken)
        {
            var namespaceName = GetNamespace();

            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(namespaceName));

            var compilation = context.Compilation;

            var platformResolver = GetPlatformResolver();

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

            var assembliesOfInterest = platformResolver.GetAssemblyNames();

            var referencesOfInterest = GetReferencesOfInterest(
                compilation.References,
                assembliesOfInterest).ToArray();
            if (referencesOfInterest.Length != assembliesOfInterest.Length)
            {
                // not got the expected count back, drop out.
                context.ReportDiagnostic(ReportDiagnostics.ReferencesOfInterestCountMismatch(assembliesOfInterest.Length, referencesOfInterest.Length));
                return namespaceDeclaration;
            }

            /*
            var missingAssemblies = assembliesOfInterest.ToList();

            foreach (MetadataReference metadataReference in references)
            {
            }


            foreach (string trustedAssembliesPath in trustedAssembliesPaths)
            {
                if (assembliesOfInterest.All(assemblyOfInterest => !trustedAssembliesPath.EndsWith(assemblyOfInterest, StringComparison.Ordinal)))
                {
                    continue;
                }

                var metadataReference = MetadataReference.CreateFromFile(trustedAssembliesPath);
                compilation = compilation.AddReferences(metadataReference);
            }
            */
            var desiredBaseType = platformResolver.GetBaseUiElement();
            var desiredNameWithoutGlobal = desiredBaseType.Replace("global::", string.Empty);
            var desiredBaseTypeSymbolMatch = compilation.GetTypeByMetadataName(desiredNameWithoutGlobal);

            if (desiredBaseTypeSymbolMatch == null)
            {
                context.ReportDiagnostic(ReportDiagnostics.FailedToFindDesiredBaseTypeSymbol(desiredBaseType));
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
                    context.ReportDiagnostic(ReportDiagnostics.DesiredBaseTypeSymbolNotInterfaceOrClass(desiredBaseType));
                    return namespaceDeclaration;
            }

            var desiredCommandInterface = platformResolver.GetCommandInterface();

            var generatorProcessor = new TGeneratorProcessor();

            var platformName = GetPlatformName();

            var result = generatorProcessor.GenerateObjects(
                namespaceDeclaration,
                referencesOfInterest,
                compilation,
                context.ReportDiagnostic,
                desiredBaseType,
                desiredBaseTypeIsInterface,
                desiredCommandInterface,
                platformName);

            return result;
        }

        /// <summary>
        /// Gets the platform resolver used for searching for UI types for the platform.
        /// </summary>
        /// <returns>Platform specific resolver.</returns>
        protected abstract IPlatformResolver GetPlatformResolver();

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

        protected abstract MetadataReference CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest);

        private static string[] GetPlatformAssemblyPaths(GeneratorExecutionContext context)
        {
            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trustedPlatformAssemblies)
            {
                return trustedPlatformAssemblies.Split(Path.PathSeparator);
            }

            return null;
        }

        /// <summary>
        /// Gets the root namespace to place the generated code inside.
        /// </summary>
        /// <returns>Fully qualified root namespace.</returns>
        protected abstract string GetNamespace();
    }
}
