using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public sealed class ViewBindingHelperGeneratorProcessor : AbstractGeneratorProcessor
    {
        public override NamespaceDeclarationSyntax GenerateObjects(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest,
            Compilation generatorExecutionContext,
            Action<Diagnostic> reportDiagnosticAction)
        {
            return namespaceDeclaration;
        }
    }
}
