# Experimental Properties Support

## Overview

Starting with .NET 10, properties can be marked with the `ExperimentalAttribute` to indicate they are experimental APIs. Vetuviem now provides built-in support for handling these properties in source generation.

## Configuration

Add the following property to your project file (`.csproj`) to control how experimental properties are handled:

```xml
<PropertyGroup>
  <Vetuviem_Allow_Experimental_Properties>true</Vetuviem_Allow_Experimental_Properties>
</PropertyGroup>
```

## Behavior

### When `Vetuviem_Allow_Experimental_Properties` is `false` or not set (default)

- Properties marked with `ExperimentalAttribute` are **excluded** from code generation
- No warnings are generated
- Experimental properties are silently skipped

### When `Vetuviem_Allow_Experimental_Properties` is `true`

- Properties marked with `ExperimentalAttribute` are **included** in code generation
- A `SuppressMessage` attribute is automatically added to suppress experimental warnings
- The diagnostic ID from the `ExperimentalAttribute` is extracted and used in the suppression

## Example

Given a property in a control:

```csharp
[Experimental("WFDEV001")]
public ThemeMode ThemeMode { get; set; }
```

When `Vetuviem_Allow_Experimental_Properties` is `true`, the generated binding property will include:

```csharp
[SuppressMessage("Usage", "WFDEV001")]
public IOneOrTwoWayBind<TViewModel, ThemeMode>? ThemeMode { get; init; }
```

This allows you to opt-in to using experimental APIs while automatically handling the warning suppressions in generated code.

## Notes

- The default behavior (excluding experimental properties) ensures backward compatibility
- The diagnostic ID is read from the first constructor argument of `ExperimentalAttribute`
- Only properties with `public` accessibility are considered for generation (standard behavior)
