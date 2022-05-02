// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
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
using Vetuviem.SourceGenerator.Features.Core;

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

                var memberDeclarationSyntax = GenerateAsync(context, context.CancellationToken);

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
                context.ReportDiagnostic(ReportDiagnostics.UnhandledException(e));
            }
        }

        /// <summary>
        /// Gets the name of the platform used in the namespaces for the generated code.
        /// </summary>
        /// <returns>Name identifier for the platform.</returns>
        protected abstract string GetPlatformName();

        /// <summary>
        /// Create the syntax tree representing the expansion of some member to which this attribute is applied.
        /// </summary>
        /// <param name="context">The transformation context being generated for.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The generated member syntax to be added to the project.</returns>
        private MemberDeclarationSyntax? GenerateAsync(
            GeneratorExecutionContext context,
            CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            var namespaceName = GetNamespace();

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

            var assembliesOfInterest = platformResolver.GetAssemblyNames();

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
                context.ReportDiagnostic(ReportDiagnostics.ReferencesOfInterestCountMismatch(assembliesOfInterest.Length, referencesOfInterest.Length));
                return namespaceDeclaration;
            }

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

        protected abstract MetadataReference? CheckIfShouldAddMissingAssemblyReference(string assemblyOfInterest);

        /// <summary>
        /// Gets the root namespace to place the generated code inside.
        /// </summary>
        /// <returns>Fully qualified root namespace.</returns>
        protected abstract string GetNamespace();
    }
}
