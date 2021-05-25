namespace ReactiveUI.WinUI3.VetuviemGenerator
{
    public sealed class AssemblyResolver
    {
        public string[] GetAssemblyNames()
        {
            return new[]
            {
                "Microsoft.WinUI.dll",
            };
        }

        public string[] GetBaseElements()
        {
            return new []
            {
                "Microsoft.WinUI.Xaml.UIElement",
            };
        }
    }
}
