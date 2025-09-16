<p align="center">
  <img src="/img/icon.webp" width="100"/>
  <h1 align="center">Echoes</h1>
  <p align="center">
    Simple type safe translations for .NET
  </p>
</p>

<p align="center">
  <img src="/img/editor-demo.png" width="80%"/>
</p>

### Features
- Change language at runtime without reloading views or flicker (obviously - but hard with ResX)
- Translation keys are generated at compile time. Missing keys (from the invariant) will show up as compiler errors.
- [Markup extension](https://docs.avaloniaui.net/docs/concepts/markupextensions) for simple usage
- Simple translation file format based on [TOML](https://toml.io/en/)
- Multiple translation files, so you can split translations by feature or module
- Inside each file, translations can be grouped and nested using *dotted key syntax* or *group/table syntax*, providing a clean, hierarchical structure within a single file
- Supports [ISO 639-1 (en, de)](https://en.wikipedia.org/wiki/ISO_639-1) and [RRC 5646 (en-US, en-GB, de-DE)](https://www.rfc-editor.org/rfc/rfc5646.html) translation identifiers
- Built-in automatic fallback: if a key is missing in a specific locale (e.g., `de-AT`), it will automatically fall back to a language-only locale (e.g., `de`), and then to the invariant file if still not found
- Autocomplete of translation keys
  <img width="952" height="151" alt="Screenshot 2025-08-05 at 10 03 21" src="https://github.com/user-attachments/assets/98d8aa66-50bc-4778-928d-b93d1da579ae" />


### Getting Started

It's best to take a look at the [Sample Project](https://github.com/Voyonic-Systems/Echoes/tree/main/src/Echoes.SampleApp)

Add references to the following packages:
```xml
<PackageReference Include="Echoes" Version=".."/>
<PackageReference Include="Echoes.Generator" Version=".."/>

<!-- For Avalonia Helpers -->
<PackageReference Include="Echoes.Avalonia" Version=".."/>
```

Specify translations files (Embedded Resources, Source Generator)
```xml
<ItemGroup>
    <!-- Include all .toml files as embedded resources (so we can dynamically load them at runtime) -->
    <EmbeddedResource Include="**\*.toml" />

    <!-- Specify invariant files that are fed into the generator (Echoes.Generator) -->
    <AdditionalFiles Include="Translations\Strings.toml" />
</ItemGroup>
```

> [!CAUTION] 
> You currently have to place your translation (.toml) files and the generated code in a **separate project**. This is because Avalonia also generates
> code using their XAML compiler. In order for the xaml compiler to see your translations you need to put them in a different project. Otherwise you'll get a
> compiler error.


### Translation Files
Translations are loaded from `.toml` files. The invariant file is **special** as it defines structure and configuration.
Additional language files are identified by `_{lang}.toml` postfix. 

```
Strings.toml
Strings_de.toml
Strings_de-AT.toml
Strings_es.toml
```

You can split translations in multiple toml files. 

```
FeatureA.toml
FeatureA_de.toml
FeatureA_es.toml

FeatureB.toml
FeatureB_de.toml
FeatureB_es.toml
```

### File Format
#### Example: Strings.toml
```toml
[echoes_config]
generated_class_name = "Strings"
generated_namespace = "Echoes.SampleApp.Translations"

[translations]
hello_world = 'Hello World'
greeting = 'Hello {0}, how are you?'

# Nested via dotted keys
dialog.ok     = "OK"
dialog.cancel = "Cancel"

# Nested via tables
[settings.display]
brightness = "Brightness"
contrast   = "Contrast"
```

#### Example: Strings_de.toml
```toml
hello_world = 'Hallo Welt'
greeting = 'Hallo {0}, wie geht es dir?'

dialog.ok     = "OK"
dialog.cancel = "Abbrechen"

[settings.display]
brightness = "Helligkeit"
contrast   = "Kontrast"
```

### XAML Usage

#### Namespaces
Add namespaces for the generated translations and the helper markup extension:

```xml
<Window
    xmlns="https://github.com/avaloniaui"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:translations="clr-namespace:Your.Namespace;assembly=Your.Assembly"
    xmlns:echoes="clr-namespace:Echoes.Avalonia;assembly=Echoes.Avalonia">
```

#### Referencing keys (using the **Echoes Avalonia** markup extension)

**Top-level entry (offers IntelliSense):**
```xml
<TextBlock Text="{echoes:Translate {x:Static translations:Strings.hello_world}}" />
```

**Nested entry (after `+` IntelliSense is currently limited by XAML tooling):**
```xml
<TextBlock Text="{echoes:Translate {x:Static translations:Strings+settings+display.brightness}}" />
<Button Content="{echoes:Translate {x:Static translations:Strings+dialog.ok}}" />
```

When the culture is changed at runtime, all bound `Translate` values automatically update without needing to reload views.

### Is this library stable?
No, it's currently in preview. See the version number.

### Why is it named "Echoes"?
The library is named after the Pink Floyd song [Echoes](https://en.wikipedia.org/wiki/Echoes_(Pink_Floyd_song)).
