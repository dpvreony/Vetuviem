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
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.Extensions.Logging;
using NetTestRegimentation.XUnit.Logging;
using Vetuviem.SourceGenerator;
using Vetuviem.SourceGenerator.Features.Core;
using Xunit;

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
        public abstract class BaseExecuteMethod<TGenerator, TGeneratorProcessor> : TestWithLoggingBase
            where TGenerator : AbstractBaseSourceGenerator<TGeneratorProcessor>
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

                var source = GetProjectSourceCode();

                var comp = CreateCompilation(source, references);

                var analyzerConfigOptionsProvider = GetAnalyzerConfigOptionsProvider();

                var newComp = RunGenerators(
                    comp,
                    analyzerConfigOptionsProvider,
                    out var generatorDiags,
                    instance.AsSourceGenerator());

                Logger.LogInformation($"Generator Diagnostic count : {generatorDiags.Length}");

                var hasErrors = false;
                foreach (var generatorDiag in generatorDiags)
                {
                    Logger.LogInformation(generatorDiag.ToString());

                    hasErrors |= generatorDiag.Severity == DiagnosticSeverity.Error;
                }

                foreach (var newCompSyntaxTree in newComp.SyntaxTrees)
                {
                    Logger.LogInformation("Syntax Tree:");
                    Logger.LogInformation(newCompSyntaxTree.GetText().ToString());
                }

                Assert.False(hasErrors);
            }

            protected abstract string GetProjectSourceCode();

            /// <summary>
            /// Gets the analyzer config options provider to test with.
            /// </summary>
            /// <returns>Analyzer Config Options.</returns>
            protected abstract AnalyzerConfigOptionsProvider? GetAnalyzerConfigOptionsProvider();

            /// <summary>
            /// Allows addition of platform specific metadata references. Unit Tests start in an agnostic fashion
            /// with no specific references loaded. Source generators typically take these via MSBuild loading
            /// from the csproj file, but you need to do it yourself in a test.
            /// </summary>
            /// <param name="metadataReferences">List of metadata references.</param>
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

            private static GeneratorDriver CreateDriver(
                Compilation compilation,
                AnalyzerConfigOptionsProvider? analyzerConfigOptionsProvider,
                params ISourceGenerator[] generators) => CSharpGeneratorDriver.Create(
                generators: ImmutableArray.Create(generators),
                additionalTexts: ImmutableArray<AdditionalText>.Empty,
                parseOptions: (CSharpParseOptions)compilation.SyntaxTrees.First().Options,
                optionsProvider: analyzerConfigOptionsProvider);

            private static Compilation RunGenerators(
                Compilation compilation,
                AnalyzerConfigOptionsProvider? analyzerConfigOptionsProvider,
                out ImmutableArray<Diagnostic> diagnostics,
                params ISourceGenerator[] generators)
            {
                var driver = CreateDriver(
                    compilation,
                    analyzerConfigOptionsProvider,
                    generators);

                driver.RunGeneratorsAndUpdateCompilation(
                    compilation,
                    out var updatedCompilation,
                    out diagnostics);

                return updatedCompilation;
            }
        }
    }
}
