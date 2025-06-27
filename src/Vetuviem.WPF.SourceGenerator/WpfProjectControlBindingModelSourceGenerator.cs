using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Vetuviem.WPF.SourceGenerator
{
    [Generator]
    public sealed class WpfProjectControlBindingModelSourceGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            var classDeclarations = context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (s, _) => s is ClassDeclarationSyntax,
                    transform: (ctx, _) => (ClassDeclarationSyntax)ctx.Node)
                .Where(cls => cls != null);

            context.RegisterSourceOutput(classDeclarations, (spc, cls) =>
            {
                spc.AddSource(cls.Identifier.ToFullString(), SourceText.From("// Classes collected", Encoding.UTF8));
            });
        }
    }
}
