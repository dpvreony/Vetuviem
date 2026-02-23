using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Vetuviem.SourceGenerator.Features.Configuration;

namespace Vetuviem.SourceGenerator
{
    [Generator]
    public sealed class VetuviemEntryAssemblySourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var trigger = context.AnalyzerConfigOptionsProvider
                .Combine(context.ParseOptionsProvider)
                .Combine(context.MetadataReferencesProvider.Collect())
                .Combine(context.CompilationProvider)
                .Select((tuple, _) =>
                {
                    (
                        (
                            (
                                AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
                                ParseOptions parseOptionsProvider),
                            ImmutableArray<MetadataReference> metadataReferencesProvider),
                        Compilation compilationProvider) = tuple;

                    return (analyzerConfigOptionsProvider, parseOptionsProvider, metadataReferencesProvider, compilationProvider);
                });

            context.RegisterImplementationSourceOutput(
                trigger,
                static (productionContext, tuple) => Execute(
                    productionContext,
                    tuple.analyzerConfigOptionsProvider,
                    tuple.parseOptionsProvider,
                    tuple.metadataReferencesProvider,
                    tuple.compilationProvider));
        }

        private static void Execute(
            SourceProductionContext context,
            AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
            ParseOptions parseOptions,
            ImmutableArray<MetadataReference> metadataReferencesProvider,
            Compilation compilation)
        {
            var configurationModel = ConfigurationFactory.Create(analyzerConfigOptionsProvider);

            var generator = GetGenerator(configurationModel.UiFramework);
            generator.GenerateFromAssemblies(
                context,
                configurationModel,
                parseOptions,
                metadataReferencesProvider,
                compilation);
        }
        private static AbstractControlBindingModelSourceGenerator GetGenerator(UiFramework uiFramework)
        {
            return uiFramework switch
            {
                UiFramework.Avalonia => new Features.Avalonia.AvaloniaControlBindingModelSourceGenerator(),
                UiFramework.Blazor => new Features.Blazor.BlazorControlBindingModelSourceGenerator(),
                UiFramework.Maui => new Features.Maui.MauiControlBindingModelSourceGenerator(),
                UiFramework.Winforms => new Features.Winforms.WinformsControlBindingModelSourceGenerator(),
                UiFramework.WinUi => new Features.WinUi.WinUi3ControlBindingModelSourceGenerator(),
                UiFramework.Wpf => new Features.Wpf.WpfControlBindingModelSourceGenerator(),
                _ => throw new NotSupportedException($"The UI framework '{uiFramework}' is not supported."),
            };
        }
    }
}
