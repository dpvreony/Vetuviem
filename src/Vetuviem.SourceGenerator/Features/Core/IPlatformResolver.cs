namespace Vetuviem.SourceGenerator.Features.Core
{
    public interface IPlatformResolver
    {
        string[] GetAssemblyNames();

        string GetBaseUiElement();

        string GetCommandInterface();
    }
}
