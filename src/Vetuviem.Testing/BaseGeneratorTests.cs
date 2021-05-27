using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.Logging;
using Vetuviem.SourceGenerator;
using Xunit;
using Xunit.Abstractions;
using Vetuviem.SourceGenerator.GeneratorProcessors;

namespace Dhgms.Nucleotide.UnitTests.Generators
{

    [ExcludeFromCodeCoverage]
    public class BaseGeneratorTests
    {
        public abstract class BaseExecuteMethod<TGenerator, TGeneratorProcessor> : Foundatio.Xunit.TestWithLoggingBase
            where TGenerator : AbstractBaseGenerator<TGeneratorProcessor>
            where TGeneratorProcessor : AbstractGeneratorProcessor, new()
        {
            protected BaseExecuteMethod(ITestOutputHelper output) : base(output)
            {
            }

            /// <summary>
            /// Test to ensure code is generated with no errors in the diagnostic handler.
            /// </summary>
            [Fact]
            public void GeneratesCode()
            {
                var factory = GetFactory();

                var instance = factory();

                var references = new List<MetadataReference>
                {
                    MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location)
                };

                AddReferenceAssemblies(references);

                var comp = CreateCompilation(string.Empty, references);

                var newComp = RunGenerators(
                    comp,
                    out var generatorDiags,
                    instance);

                _logger.LogInformation($"Generator Diagnostic count : {generatorDiags.Length}");

                var hasErrors = false;
                foreach (var generatorDiag in generatorDiags)
                {
                    _logger.LogInformation(generatorDiag.ToString());

                    hasErrors |= generatorDiag.Severity == DiagnosticSeverity.Error;
                }

                Assert.False(hasErrors);
            }

            protected abstract void AddReferenceAssemblies(List<MetadataReference> metadataReferences);


            protected abstract Func<TGenerator> GetFactory();

            private static Compilation CreateCompilation(string source, IEnumerable<MetadataReference> reference) => CSharpCompilation.Create(
                assemblyName: "compilation",
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)) },
                references: reference,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication)
            );

            private static GeneratorDriver CreateDriver(Compilation compilation, params ISourceGenerator[] generators) => CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generators),
                additionalTexts: ImmutableArray<AdditionalText>.Empty,
                parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options,
                optionsProvider: null
            );

            private static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
            {
                CreateDriver(compilation, generators).RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);
                return updatedCompilation;
            }
        }
    }
}
