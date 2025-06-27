using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

            context.RegisterSourceOutput(
                classDeclarations,
                static (productionContext, syntaxContext) => DoSourceGenerationForClass(productionContext, syntaxContext));
        }

        private static void DoSourceGenerationForClass(
            SourceProductionContext productionContext,
            GeneratorSyntaxContext syntaxContext)
        {
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

            if (!IsDesiredUiType(namedTypeSymbol, syntaxContext, productionContext))
            {
                return;
            }

            // TODO: Generate the binding model source code here.
            var hintName = $"{namedTypeSymbol}.g.cs";
            productionContext.AddSource(hintName, "// TODO: code generation");
        }

        private static bool IsDesiredUiType(
            INamedTypeSymbol namedTypeSymbol,
            GeneratorSyntaxContext generatorSyntaxContext,
            SourceProductionContext productionContext)
        {
            var compilation = generatorSyntaxContext.SemanticModel.Compilation;

            var platformResolver = new WpfPlatformResolver();
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
    }
}
