using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vetuviem.SourceGenerator.GeneratorProcessors
{
    public abstract class AbstractGeneratorProcessor
    {
        public abstract NamespaceDeclarationSyntax GenerateObjects(NamespaceDeclarationSyntax namespaceDeclaration,
            MetadataReference[] assembliesOfInterest,
            Compilation generatorExecutionContext,
            Action<Diagnostic> reportDiagnosticAction,
            string desiredBaseType,
            bool desiredBaseTypeIsInterface,
            string desiredCommandInterface,
            string platformName);
    }
}
