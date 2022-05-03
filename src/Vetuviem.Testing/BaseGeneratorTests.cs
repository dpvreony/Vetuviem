// Copyright (c) 2022 DPVreony and Contributors. All rights reserved.
// DPVreony and Contributors licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

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
using Vetuviem.SourceGenerator.Features.Core;
using Xunit;
using Xunit.Abstractions;

namespace Vetuviem.Testing
{
    /// <summary>
    /// Unit Tests for a Source Generator.
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class BaseGeneratorTests
    {
        /// <summary>
        /// Unit Tests for the Execute Method.
        /// </summary>
        /// <typeparam name="TGenerator">The type for the source generator.</typeparam>
        /// <typeparam name="TGeneratorProcessor">The type for the source generator processor.</typeparam>
        public abstract class BaseExecuteMethod<TGenerator, TGeneratorProcessor> : Foundatio.Xunit.TestWithLoggingBase
            where TGenerator : AbstractBaseGenerator<TGeneratorProcessor>
            where TGeneratorProcessor : AbstractGeneratorProcessor, new()
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="BaseExecuteMethod{TGenerator, TGeneratorProcessor}"/> class.
            /// </summary>
            /// <param name="output">Test Output Helper.</param>
            protected BaseExecuteMethod(ITestOutputHelper output)
                : base(output)
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

            /// <summary>
            /// Allows addition of platform specific metadata references. Unit Tests start in an agnostic fashion
            /// with no specific references loaded. Source generators typically take these via MSBuild loading
            /// from the csproj file, but you need to do it yourself in a test.
            /// </summary>
            /// <param name="metadataReferences"></param>
            protected abstract void AddReferenceAssemblies(IList<MetadataReference> metadataReferences);

            /// <summary>
            /// Gets the factory method for creating a source generator.
            /// </summary>
            /// <returns>Function for creating a source generator.</returns>
            protected abstract Func<TGenerator> GetFactory();

            private static Compilation CreateCompilation(string source, IEnumerable<MetadataReference> reference) => CSharpCompilation.Create(
                assemblyName: "compilation",
                syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source, new CSharpParseOptions(LanguageVersion.Preview)) },
                references: reference,
                options: new CSharpCompilationOptions(OutputKind.ConsoleApplication));

            private static GeneratorDriver CreateDriver(Compilation compilation, params ISourceGenerator[] generators) => CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generators),
                additionalTexts: ImmutableArray<AdditionalText>.Empty,
                parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options,
                optionsProvider: null);

            private static Compilation RunGenerators(Compilation compilation, out ImmutableArray<Diagnostic> diagnostics, params ISourceGenerator[] generators)
            {
                CreateDriver(compilation, generators).RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out diagnostics);
                return updatedCompilation;
            }
        }
    }
}
