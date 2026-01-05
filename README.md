# Vetuviem
Vetuviem is a toolkit to support View to View Model binding (MVVM -> V2VM -> Ve-Tu-Viem) aimed at offering a structure to get more re-usability out of ReactiveUI.

## Mission Statement
* To give a mechanism to reduce the amount of boiler plate code being produced, by allowing some of the ReactiveUI specific logic to be hidden away
* Allow the developer to think along the lines of standard behaviours for controls by offering a way to produce re-usable behaviours through a class and\or function design pattern
* Allow the developer to focus on what matters on the ViewModel
* Reduce the cognitive load by
  * Removing the risk of misusing 1 way or 2 way binding
  * Remove the need for the user to think about having to cater for Bind vs BindCommand
* Offer a structure that allows for more work to be done potentially with Source Generators to reduce reflection and improve the build time developer experience.

## Current Status

This is currently a proof of concept alpha. For understanding of the design reasoning please see https://www.dpvreony.com/articles/designing-vetuviem/

## Nuget Packages

[VetuviemCoreNuget]: https://www.nuget.org/packages/Vetuviem.Core/
[VetuviemCoreBadge]: https://img.shields.io/nuget/v/Vetuviem.Core.svg
[VetuviemSourceGeneratorNuGet]: https://www.nuget.org/packages/Vetuviem.SourceGenerator/
[VetuviemSourceGeneratorBadge]: https://img.shields.io/nuget/v/Vetuviem.SourceGenerator.svg

| Purpose | Package | NuGet |
|---------|-------|------|
| Command Line Generation | Coming soon | Coming Soon
| Visual Studio Integration | [Vetuviem.SourceGenerator][VetuviemSourceGeneratorNuGet] | [![VetuviemSourceGeneratorBadge]][VetuviemSourceGeneratorNuGet] |
| Core Functionality | [Vetuviem.Core][VetuviemCoreNuget] | [![VetuviemCoreBadge]][VetuviemCoreNuget] |

## An example

Currently to write binding logic in the codebehind you have to write something similar to this for a single control

```csharp
// Traditional ReactiveUI binding approach
this.WhenActivated(disposables =>
{
    this.Bind(ViewModel, vm => vm.Forename, v => v.Forename.Text)
        .DisposeWith(disposables);
    this.OneWayBind(ViewModel, vm => vm.ForenameLengthRemaining, v => v.ForenameLengthRemaining.Content)
        .DisposeWith(disposables);
    this.OneWayBind(ViewModel, vm => vm.ForenameLengthRemaining, v => v.ForenameLengthRemaining.Foreground, 
        lengthRemaining => GetBrushForLengthRemaining(lengthRemaining))
        .DisposeWith(disposables);
});
```

But what if you have a way to simplify logic and offer a way to even make it reusable without all the boilerplate leg work?

```csharp
// Vetuviem approach using ViewBindingModels
public sealed class QuestionnaireViewBindingModels : AbstractEnableViewToViewModelBindings<QuestionnaireView, QuestionnaireViewModel>
{
    protected override IEnumerable<IControlBindingModel<QuestionnaireView, QuestionnaireViewModel>> GetBindings()
    {
        yield return new TextBoxControlBindingModel<QuestionnaireView, QuestionnaireViewModel>(vw => vw.Forename)
        {
            Text = new TwoWayBinding<QuestionnaireViewModel, string>(vm => vm.Forename),
        };
        
        yield return new LabelControlBindingModel<QuestionnaireView, QuestionnaireViewModel>(vw => vw.ForenameLengthRemaining)
        {
            Content = new OneWayBindingOnOneOrTwoWayBind<QuestionnaireViewModel, object>(vm => vm.ForenameLengthRemaining, o => o?.ToString() ?? string.Empty),
            Foreground = new OneWayBindingWithConversionOnOneOrTwoWayBind<QuestionnaireViewModel, Brush, int>(vm => vm.ForenameLengthRemaining, lengthRemaining => GetBrushForLengthRemaining(lengthRemaining))
        };
    }
}
```

## Sponsorship

If you find Vetuviem useful, please consider sponsoring the project to help with ongoing development and maintenance. You can sponsor through [GitHub Sponsors](https://github.com/sponsors/dpvreony).

## Support

For support, please:
* Check the [design article](https://www.dpvreony.com/articles/designing-vetuviem/) for understanding the design reasoning
* Report bugs or request features via [GitHub Issues](https://github.com/dpvreony/Vetuviem/issues)
* Ask questions in [GitHub Discussions](https://github.com/dpvreony/Vetuviem/discussions)
* Review the sample applications in the `src` directory for practical examples

## Contribute

Contributions are welcome! Please:
* Read the [Code of Conduct](CODE_OF_CONDUCT.md)
* Fork the repository and create a feature branch
* Write tests for your changes
* Ensure all tests pass and code follows the existing style
* Submit a Pull Request with a clear description of your changes

For more information on experimental properties support, see [EXPERIMENTAL_PROPERTIES.md](EXPERIMENTAL_PROPERTIES.md).

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

