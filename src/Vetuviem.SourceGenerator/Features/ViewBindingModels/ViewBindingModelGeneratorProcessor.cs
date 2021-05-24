using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public sealed class ViewBindingModelGeneratorProcessor : AbstractGeneratorProcessor
    {
        public override NamespaceDeclarationSyntax GenerateObjects(
            NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest)
        {
            return namespaceDeclaration;
        }
    }
}
