using System;
using System.Collections.Generic;
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
    public abstract class AbstractBaseGenerator<TGeneratorProcessor> : ISourceGenerator
        where TGeneratorProcessor : AbstractGeneratorProcessor, new()
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

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

        /// <summary>
        /// Create the syntax tree representing the expansion of some member to which this attribute is applied.
        /// </summary>
        /// <param name="context">The transformation context being generated for.</param>
        /// <param name="progress">A way to report diagnostic messages.</param>
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
            var desiredCommandInterface = platformResolver.GetCommandInterface();

            var generatorProcessor = new TGeneratorProcessor();

            var result = generatorProcessor.GenerateObjects(
                namespaceDeclaration,
                referencesOfInterest,
                compilation,
                context.ReportDiagnostic,
                desiredBaseType,
                desiredCommandInterface);

            return result;
        }

        protected abstract IPlatformResolver GetPlatformResolver();

        private static IEnumerable<MetadataReference> GetReferencesOfInterest(
            IEnumerable<MetadataReference> compilationReferences,
            string[] assembliesOfInterest)
        {
            foreach (var compilationReference in compilationReferences)
            {
                if (assembliesOfInterest.Any(
                    assemblyOfInterest => compilationReference.Display != null
                                          && compilationReference.Display.EndsWith(
                                              assemblyOfInterest,
                                              StringComparison.Ordinal)))
                {
                    yield return compilationReference;
                }
            }
        }

        private static string[] GetPlatformAssemblyPaths(GeneratorExecutionContext context)
        {
            if (AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES") is string trustedPlatformAssemblies)
            {
                return trustedPlatformAssemblies.Split(Path.PathSeparator);
            }

            return null;
        }

        protected abstract string GetNamespace();
    }
}
