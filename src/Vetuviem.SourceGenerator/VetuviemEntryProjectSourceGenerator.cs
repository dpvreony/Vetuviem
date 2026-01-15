using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vetuviem.SourceGenerator.Features.Configuration;

namespace Vetuviem.SourceGenerator
{
    [Generator]
    public sealed class VetuviemEntryProjectSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    static (s, _) => s is ClassDeclarationSyntax,
                    static (ctx, _) => ctx);

            // TODO: the combine syntax is horrid, pull the bunch of helpers from nucleotide that do this cleaner.
            var trigger = classDeclarations.Combine(context.AnalyzerConfigOptionsProvider)
                .Combine(context.ParseOptionsProvider)
                .Select(
                    (tuple1, _) => (
                        SyntaxProvider: tuple1.Left.Left,
                        AnalyzerConfigOptions: tuple1.Left.Right,
                        ParseOptions: tuple1.Right));

            context.RegisterSourceOutput(
                trigger,
                static (productionContext, tuple) => DoSourceGeneration(
                    productionContext,
                    tuple.SyntaxProvider,
                    tuple.AnalyzerConfigOptions,
                    tuple.ParseOptions));
        }

        private static void DoSourceGeneration(
            SourceProductionContext productionContext,
            GeneratorSyntaxContext syntaxProvider,
            AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
            ParseOptions parseOptions)
        {
            var configurationModel = ConfigurationFactory.Create(analyzerConfigOptionsProvider);

            var generator = GetGenerator(configurationModel.UiFramework);
            generator.DoSourceGenerationForClass(
                productionContext,
                syntaxProvider,
                configurationModel,
                parseOptions);
        }

        private static AbstractProjectControlBindingModelSourceGenerator GetGenerator(UiFramework uiFramework)
        {
            return uiFramework switch
            {
                UiFramework.Avalonia => new Features.Avalonia.AvaloniaProjectControlBindingModelSourceGenerator(),
                UiFramework.Blazor => new Features.Blazor.BlazorProjectControlBindingModelSourceGenerator(),
                UiFramework.Maui => new Features.Maui.MauiProjectControlBindingModelSourceGenerator(),
                UiFramework.Winforms => new Features.Wpf.WpfProjectControlBindingModelSourceGenerator(),
                UiFramework.WinUi => new Features.Wpf.WpfProjectControlBindingModelSourceGenerator(),
                UiFramework.Wpf => new Features.Wpf.WpfProjectControlBindingModelSourceGenerator(),
                _ => throw new NotSupportedException($"The UI framework '{uiFramework}' is not supported."),
            };
        }
    }
}
