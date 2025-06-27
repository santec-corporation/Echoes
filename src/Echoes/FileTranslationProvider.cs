using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Tommy;

namespace Echoes;

public class FileTranslationProvider(Assembly assembly, string embeddedResourceKey)
{
    private readonly ImmutableDictionary<string, string> _invariantTranslations = ReadResource(assembly, embeddedResourceKey)
                                                                                  ?? throw new Exception("Embedded resource could not be found. ");
    private (CultureInfo Culture, ImmutableDictionary<string, string> Lookup)? _translations = null;

    public string? ReadTranslation(string key, CultureInfo culture)
    {
        ArgumentNullException.ThrowIfNull(culture);

        var lookup = _translations?.Lookup;
        var lookupCulture = _translations?.Culture;

        if (lookup == null || (!lookupCulture?.Equals(culture) ?? false))
        {
            var fileName = Path.GetFileNameWithoutExtension(embeddedResourceKey);
            var fullName = fileName + "_" + culture.TwoLetterISOLanguageName + ".toml";

            var fullMatch = ReadResource(assembly, fullName) ?? ImmutableDictionary<string, string>.Empty;
            _translations = (culture, fullMatch);

            lookup = fullMatch;
        }

        return lookup.TryGetValue(key, out var result) ? result : CollectionExtensions.GetValueOrDefault(_invariantTranslations, key);
    }

    private static ImmutableDictionary<string, string>? ReadResource(Assembly assembly, string file)
    {
        var resourceNames = assembly.GetManifestResourceNames();

        var resourcePath =
            resourceNames
                .FirstOrDefault(str =>
                    str.EndsWith(file.Replace("/", ".").Replace(@"\", "."), StringComparison.OrdinalIgnoreCase));

        if (resourcePath == null)
            return null;

        using var stream = assembly.GetManifestResourceStream(resourcePath);

        if (stream == null)
            return null;

        using var reader = new StreamReader(stream);

        var root = TOML.Parse(reader);

        if (root.RawTable.TryGetValue("translations", out var translations))
        {
            var immutableDict = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var pair in translations.AsTable.RawTable)
                if (pair.Value.IsString)
                    immutableDict.Add(pair.Key, pair.Value.AsString);

            return immutableDict.ToImmutable();
        }
        else
        {
            var immutableDict = ImmutableDictionary.CreateBuilder<string, string>();

            foreach (var pair in root.RawTable)
                if (pair.Value.IsString)
                    immutableDict.Add(pair.Key, pair.Value.AsString);

            return immutableDict.ToImmutable();
        }
    }
}