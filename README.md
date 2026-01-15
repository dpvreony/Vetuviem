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

## Getting started

### Install the desired package

TODO

### Add workaround for Windows based builds

On CI runners you can occasionally find that the Source generators fail to load with soemthing along the lines of `ErrorCode: ReferencesNewerCompiler, ReferencedCompilerVersion: 5.0.0.0`.
NOTE: different .NET SDK or MSBuild versions are inconsistent with the error message, you may sometimes see it prefixed with ADR0001 or CS9057, other times it has no prefix.

This is currently (as of 2026-01) an issue with Github Actions where Visual Studio 2022 is on the Agent. This forces MSBuild to use the desired version rather than the one built into Visual Studio and MSBuild.

Place the following in a Dir.Build.Props file or the relevant C# project file(s).

```xml
  <!--
  This roslyn override is here because on the Windows agent the Visual Studio MSBuild gets invoked
  so Vetuviem has been failing to generate because it needs a newer Roslyn compiler than the one installed in VS.
  -->
  <PropertyGroup>
    <!-- Force use of .NET SDK Roslyn compiler on Windows when VS MSBuild is invoked -->
    <UseSharedCompilation>false</UseSharedCompilation>
  </PropertyGroup>
  
  <ItemGroup>
    <!-- Explicitly reference Roslyn 5.0+ compiler toolset -->
    <PackageReference Include="Microsoft.Net.Compilers.Toolset" Version="5.0.0" Condition="'$(MSBuildRuntimeType)' != 'Core'">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
  </ItemGroup>
```

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

## Configuration

Vetuviem source generators can be configured through MSBuild properties in your project file (`.csproj`). Add these properties to a `<PropertyGroup>` section to customize code generation behavior.

### Available Properties

| Property | Description | Default Value |
|----------|-------------|---------------|
| `Vetuviem_Root_Namespace` | Override the root namespace for generated code | (Project's root namespace) |
| `Vetuviem_Make_Classes_Public` | Make generated classes public instead of internal | `false` |
| `Vetuviem_Assemblies` | Comma-separated list of assemblies to scan for controls | (Platform-specific defaults) |
| `Vetuviem_Assembly_Mode` | How to use custom assemblies: `Replace` or `Extend` | `Replace` |
| `Vetuviem_Base_Namespace` | Base namespace when using custom assemblies | (none) |
| `Vetuviem_Include_Obsolete_Items` | Include properties marked with `ObsoleteAttribute` | `false` |
| `Vetuviem_Allow_Experimental_Properties` | Include properties marked with `ExperimentalAttribute` | `false` |
| `Vetuviem_Logging_Implementation_Mode` | Logging implementation: `None` or `SplatViaServiceLocator` | `SplatViaServiceLocator` |

### Property Details

#### Vetuviem_Root_Namespace

Override the default root namespace for generated code. This is useful when you want generated binding models to reside in a specific namespace.

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Root_Namespace>MyApp.Generated</Vetuviem_Root_Namespace>
</PropertyGroup>
```

#### Vetuviem_Make_Classes_Public

Controls the visibility of generated binding model classes. By default, classes are generated as `internal`. Set this to `true` to make them `public`.

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Make_Classes_Public>true</Vetuviem_Make_Classes_Public>
</PropertyGroup>
```

#### Vetuviem_Assemblies

Specify a comma-separated list of assemblies to scan for control types. Use in conjunction with `Vetuviem_Assembly_Mode` to either replace or extend the default platform assemblies.

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Assemblies>CustomControls.dll,ThirdPartyControls.dll</Vetuviem_Assemblies>
</PropertyGroup>
```

#### Vetuviem_Assembly_Mode

Determines how the assemblies specified in `Vetuviem_Assemblies` are used:
- **`Replace`** (default): Replace platform defaults with your custom assemblies
- **`Extend`**: Add custom assemblies in addition to platform defaults

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Assembly_Mode>Extend</Vetuviem_Assembly_Mode>
</PropertyGroup>
```

#### Vetuviem_Base_Namespace

Used with custom assemblies to specify a base namespace. This allows third parties to use the generator and produce custom namespaces that inherit from the root or use a custom namespace.

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Base_Namespace>MyCompany.Controls</Vetuviem_Base_Namespace>
</PropertyGroup>
```

#### Vetuviem_Include_Obsolete_Items

Controls whether properties marked with `ObsoleteAttribute` are included in code generation. By default, obsolete items are excluded.

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Include_Obsolete_Items>true</Vetuviem_Include_Obsolete_Items>
</PropertyGroup>
```

#### Vetuviem_Allow_Experimental_Properties

Starting with .NET 10, properties can be marked with `ExperimentalAttribute` to indicate they are experimental APIs. This property controls how these are handled:

**When `false` or not set (default):**
- Properties marked with `ExperimentalAttribute` are **excluded** from code generation
- No warnings are generated
- Experimental properties are silently skipped
- Ensures backward compatibility

**When `true`:**
- Properties marked with `ExperimentalAttribute` are **included** in code generation
- A `SuppressMessage` attribute is automatically added to suppress experimental warnings
- The diagnostic ID from the `ExperimentalAttribute` is extracted and used in the suppression

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Allow_Experimental_Properties>true</Vetuviem_Allow_Experimental_Properties>
</PropertyGroup>
```

**Example Control Property:**
```csharp
[Experimental("WFDEV001")]
public ThemeMode ThemeMode { get; set; }
```

**Generated Binding Property (when enabled):**
```csharp
[SuppressMessage("Usage", "WFDEV001")]
public IOneOrTwoWayBind<TViewModel, ThemeMode>? ThemeMode { get; init; }
```

**Notes:**
- The diagnostic ID is read from the first constructor argument of `ExperimentalAttribute`
- Only properties with `public` accessibility are considered for generation (standard behavior)

#### Vetuviem_Logging_Implementation_Mode

Controls the logging implementation generated in binding code:
- **`None`**: No logging implementation
- **`SplatViaServiceLocator`** (default): Uses Splat logging via service locator

**Example:**
```xml
<PropertyGroup>
  <Vetuviem_Logging_Implementation_Mode>None</Vetuviem_Logging_Implementation_Mode>
</PropertyGroup>
```

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

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

