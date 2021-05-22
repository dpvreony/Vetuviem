using System;
using System.Collections.Generic;
using System.IO;
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
            context.ReportDiagnostic(InfoDiagnostic(typeof(TGeneratorProcessor).ToString()));

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

            var hintName = $"{feature}.{guid}.g.cs";

            context.AddSource(
                hintName,
                sourceText);
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

            // we won't want to run this in the source generator
            // we want to build a support tool that can use this to double check what we build into the generator.
            // i.e. this work gets done at compile time.
            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            foreach (string trustedAssembliesPath in trustedAssembliesPaths)
            {
                if (!trustedAssembliesPath.EndsWith("PresentationCore.dll", StringComparison.Ordinal))
                {
                    continue;
                }

                var metadataReference = MetadataReference.CreateFromFile(trustedAssembliesPath);
                compilation = compilation.AddReferences(metadataReference);
            }

            var typeOfInterest = GetUiTypes(context, compilation);

            var generatorProcessor = new TGeneratorProcessor();

            var result = generatorProcessor.GenerateObjects(namespaceDeclaration);

            return result;
        }

        private string GetUiTypes(GeneratorExecutionContext context, Compilation compilation)
        {
            var metadataReferences = compilation.References;
            foreach (var mr in metadataReferences)
            {
                if (!mr.Display.EndsWith("PresentationCore.dll", StringComparison.Ordinal))
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

                if (fullName.Equals("global::System.Windows.UIElement", StringComparison.Ordinal))
                {
                    context.ReportDiagnostic(InfoDiagnostic("Matched UiElement"));
                    return;
                }

                // check if we inherit from our desired element.
            }

            var namespaceSymbols = namespaceSymbol.GetNamespaceMembers();

            foreach (var nestedNamespaceSymbol in namespaceSymbols)
            {
                CheckNamespaceForUiTypes(
                    context,
                    nestedNamespaceSymbol);
            }
        }

        protected abstract string GetNamespace();

        private static Diagnostic InfoDiagnostic(string message)
        {
            return Diagnostic.Create(
                "VET-I0001",
                "Vetuviem Generation",
                message,
                DiagnosticSeverity.Info,
                DiagnosticSeverity.Info,
                true,
                1,
                "Model Generation");
        }
    }
}
