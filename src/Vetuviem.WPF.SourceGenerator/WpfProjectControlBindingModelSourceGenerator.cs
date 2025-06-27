using System;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Vetuviem.SourceGenerator.Features.Configuration;
using Vetuviem.SourceGenerator.Features.ControlBindingModels;
using Vetuviem.SourceGenerator.Features.Core;

namespace Vetuviem.WPF.SourceGenerator
{
    [Generator]
    public sealed class WpfProjectControlBindingModelSourceGenerator : IIncrementalGenerator
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
                static (productionContext, tuple) => DoSourceGenerationForClass(
                    productionContext,
                    tuple.SyntaxProvider,
                    tuple.AnalyzerConfigOptions,
                    tuple.ParseOptions));
        }

        private static void DoSourceGenerationForClass(
            SourceProductionContext productionContext,
            GeneratorSyntaxContext syntaxContext,
            AnalyzerConfigOptionsProvider analyzerConfigOptionsProvider,
            ParseOptions parseOptions)
        {
            var configurationModel = ConfigurationFactory.Create(analyzerConfigOptionsProvider);

            // it would be nice to cache some of this such as the platform resolver, but need to get it working first.
            if (syntaxContext.SemanticModel.GetDeclaredSymbol(syntaxContext.Node) is not INamedTypeSymbol namedTypeSymbol)
            {
                return;
            }

            // Only process the first declaration of a partial class
            var firstDeclaration = namedTypeSymbol.DeclaringSyntaxReferences
                .Select(r => r.GetSyntax())
                .OfType<ClassDeclarationSyntax>()
                .OrderBy(s => s.SpanStart)
                .FirstOrDefault();

            if (syntaxContext.Node != firstDeclaration)
            {
                return;
            }

            var platformResolver = new WpfPlatformResolver();
            if (!IsDesiredUiType(namedTypeSymbol, syntaxContext, productionContext, platformResolver))
            {
                return;
            }

            // TODO: Generate the binding model source code here.
            var memberDeclarationSyntaxes = new SyntaxList<MemberDeclarationSyntax>();

            var rootNamespace = configurationModel.RootNamespace;

            var classGenerators = GetClassGenerators();
            const string platformName = "WPF";
            var desiredNamespace = GetNamespace(rootNamespace, platformName);
            foreach (var classGeneratorFactory in classGenerators)
            {
                var generator = classGeneratorFactory();
                var generatedClass = generator.GenerateClass(
                    namedTypeSymbol,
                    platformResolver.GetBaseUiElement(),
                    platformResolver.GetCommandInterface(),
                    platformName,
                    desiredNamespace,
                    configurationModel.MakeClassesPublic,
                    configurationModel.IncludeObsoleteItems,
                    platformResolver.GetCommandInterface());

                memberDeclarationSyntaxes.Add(generatedClass);
            }


            var namespaceDeclaration = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(desiredNamespace));
            namespaceDeclaration = namespaceDeclaration
                .WithMembers(memberDeclarationSyntaxes);
            var nullableDirectiveTrivia = SyntaxFactory.NullableDirectiveTrivia(SyntaxFactory.Token(SyntaxKind.EnableKeyword), true);
            var trivia = SyntaxFactory.Trivia(nullableDirectiveTrivia);
            var leadingSyntaxTriviaList = SyntaxFactory.TriviaList(trivia);

            namespaceDeclaration = namespaceDeclaration.WithLeadingTrivia(leadingSyntaxTriviaList);

            var cu = SyntaxFactory.CompilationUnit()
                .AddMembers(namespaceDeclaration)
                .NormalizeWhitespace();

            var sourceText = SyntaxFactory.SyntaxTree(
                    cu,
                    parseOptions,
                    encoding: Encoding.UTF8)
                .GetText();

            var hintName = $"{namedTypeSymbol}.g.cs";

            productionContext.AddSource(
                hintName,
                sourceText);

        }

        private static bool IsDesiredUiType(INamedTypeSymbol namedTypeSymbol,
            GeneratorSyntaxContext generatorSyntaxContext,
            SourceProductionContext productionContext,
            WpfPlatformResolver platformResolver)
        {
            var compilation = generatorSyntaxContext.SemanticModel.Compilation;

            var desiredBaseType = platformResolver.GetBaseUiElement();
            var desiredNameWithoutGlobal = desiredBaseType.Replace(
                "global::",
                string.Empty);
            var desiredBaseTypeSymbolMatch = compilation.GetTypeByMetadataName(desiredNameWithoutGlobal);

            if (desiredBaseTypeSymbolMatch == null)
            {
                productionContext.ReportDiagnostic(ReportDiagnosticFactory.FailedToFindDesiredBaseTypeSymbol(desiredBaseType));
                return false;
            }

            // blazor uses an interface, so we check once to drive different inheritance check.
            var desiredBaseTypeIsInterface = false;
            switch (desiredBaseTypeSymbolMatch.TypeKind)
            {
                case TypeKind.Interface:
                    desiredBaseTypeIsInterface = true;
                    break;
                case TypeKind.Class:
                    break;
                default:
                    productionContext.ReportDiagnostic(ReportDiagnosticFactory.DesiredBaseTypeSymbolNotInterfaceOrClass(desiredBaseType));
                    return false;
            }

            return IsDesiredUiType(
                namedTypeSymbol,
                desiredBaseType,
                desiredBaseTypeIsInterface);
        }

        private static bool IsDesiredUiType(
            INamedTypeSymbol namedTypeSymbol,
            string baseUiElement,
            bool desiredBaseTypeIsInterface)
        {
            if (namedTypeSymbol.IsStatic
                || !NamedTypeSymbolHelpers.HasDesiredBaseType(
                     baseUiElement,
                     desiredBaseTypeIsInterface,
                     namedTypeSymbol))
            {
                return false;
            }

            return true;
        }

        private static Func<IClassGenerator>[] GetClassGenerators()
        {
            return new Func<IClassGenerator>[]
            {
                () => new GenericControlBindingModelClassGenerator(),
                () => new ControlBoundControlBindingModelClassGenerator(),
            };
        }

        private static string GetNamespace(string? rootNamespace, string platformName)
        {
            if (string.IsNullOrWhiteSpace(rootNamespace))
            {
                rootNamespace = "VetuviemGenerated";
            }

            return $"{rootNamespace}.{platformName}.ViewToViewModelBindings";
        }
    }
}
