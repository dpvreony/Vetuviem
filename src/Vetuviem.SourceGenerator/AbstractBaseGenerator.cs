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
                var guid = Guid.NewGuid();

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
                var hintName = $"{feature}.{guid}.g.cs";

                // context.AddSource(logFileHintName, logFileSourceText);

                context.AddSource(
                    hintName,
                    sourceText);
            }
            catch (Exception e)
            {
                context.ReportDiagnostic(ReportDiagnostics.UnhandledException(e));
                //throw;
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

            // we work on assumption we have the references already in the build chain
            var trustedAssembliesPaths = GetPlatformAssemblyPaths(context);
            if (trustedAssembliesPaths == null || trustedAssembliesPaths.Length == 0)
            {
                // we don't have the assemblies
                // we can fall back to searching for the reference assemblies path
                // or trigger nuget
                // for now we drop out
                return namespaceDeclaration;
            }

            var assembliesOfInterest = new[]
            {
                "PresentationCore.dll",
                "PresentationFramework.dll",
                "PresentationFramework.Aero.dll",
                "PresentationFramework.Aero2.dll",
                "PresentationFramework.AeroLite.dll",
                "PresentationFramework.Classic.dll",
                "PresentationFramework.Luna.dll",
                "PresentationFramework.Royale.dll",
                "PresentationFramework-SystemCore.dll",
                "PresentationFramework-SystemData.dll",
                "PresentationFramework-SystemDrawing.dll",
                "PresentationFramework-SystemXml.dll",
                "PresentationFramework-SystemXmlLinq.dll",
                "PresentationFrameworkUI.dll",
            };

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

            var generatorProcessor = new TGeneratorProcessor();

            var result = generatorProcessor.GenerateObjects(
                namespaceDeclaration,
                referencesOfInterest);

            return result;
        }

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

        private string[] GetUiTypes(
            GeneratorExecutionContext context,
            Compilation compilation,
            string[] assembliesOfInterest)
        {
            var metadataReferences = compilation.References;
            foreach (var mr in metadataReferences)
            {
                if (assembliesOfInterest.All(assemblyOfInterest => !mr.Display.EndsWith(assemblyOfInterest, StringComparison.Ordinal)))
                {
                    continue;
                }

                var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(mr) as IAssemblySymbol;

                if (assemblySymbol == null)
                {
                    continue;
                }

                var globalNamespace = assemblySymbol.GlobalNamespace;
                CheckNamespaceForUiTypes(context, globalNamespace);
            }

            return null;
        }

        private void CheckNamespaceForUiTypes(
            GeneratorExecutionContext context,
            INamespaceSymbol namespaceSymbol)
        {
            var namedTypeSymbols = namespaceSymbol.GetTypeMembers();

            foreach (var namedTypeSymbol in namedTypeSymbols)
            {
                var fullName = namedTypeSymbol.GetFullName();

                var desiredBaseType = "global::System.Windows.UIElement";

                // check if we inherit from our desired element.
                if (HasDesiredBaseType(desiredBaseType, namedTypeSymbol))
                {
                    return;
                }

                if (fullName.Equals(desiredBaseType, StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(ReportDiagnostics.MatchedBaseUiElement(desiredBaseType));
                    return;
                }
            }

            var namespaceSymbols = namespaceSymbol.GetNamespaceMembers();

            foreach (var nestedNamespaceSymbol in namespaceSymbols)
            {
                CheckNamespaceForUiTypes(
                    context,
                    nestedNamespaceSymbol);
            }
        }

        private bool HasDesiredBaseType(string desiredBaseType, INamedTypeSymbol namedTypeSymbol)
        {
            var baseType = namedTypeSymbol.BaseType;

            while (baseType != null)
            {
                var baseTypeFullName = baseType.GetFullName();
                if (baseTypeFullName.Equals(desiredBaseType, StringComparison.Ordinal))
                {
                    return true;
                }

                if (baseTypeFullName.Equals("global::System.Object", StringComparison.Ordinal))
                {
                    return false;
                }

                baseType = baseType.BaseType;
            }

            return false;
        }

        protected abstract string GetNamespace();
    }
}
