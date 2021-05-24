using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public sealed class ViewBindingModelGeneratorProcessor : AbstractGeneratorProcessor
    {
        public override NamespaceDeclarationSyntax GenerateObjects(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction)
        {
            foreach (var metadataReference in assembliesOfInterest)
            {
                GenerateClassesForAssembly(
                    namespaceDeclaration,
                    metadataReference,
                    compilation,
                    reportDiagnosticAction);
            }

            return namespaceDeclaration;
        }

        private void GenerateClassesForAssembly(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference metadataReference,
            Compilation compilation,
            Action<Diagnostic> reportDiagnosticAction)
        {
            var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(metadataReference) as IAssemblySymbol;

            if (assemblySymbol == null)
            {
                return;
            }

            var globalNamespace = assemblySymbol.GlobalNamespace;
            CheckNamespaceForUiTypes(globalNamespace, reportDiagnosticAction);
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

            }

            return null;
        }

        private void CheckNamespaceForUiTypes(
            INamespaceSymbol namespaceSymbol,
            Action<Diagnostic> reportDiagnosticAction)
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
                    reportDiagnosticAction(ReportDiagnostics.MatchedBaseUiElement(desiredBaseType));
                    return;
                }
            }

            var namespaceSymbols = namespaceSymbol.GetNamespaceMembers();

            foreach (var nestedNamespaceSymbol in namespaceSymbols)
            {
                CheckNamespaceForUiTypes(
                    nestedNamespaceSymbol,
                    reportDiagnosticAction);
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
    }
}
