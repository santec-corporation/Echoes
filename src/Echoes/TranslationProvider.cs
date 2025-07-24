using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;

namespace Echoes;

public static class TranslationProvider
{
    private static readonly ConcurrentDictionary<string, FileTranslationProvider> Providers;

    static TranslationProvider()
    {
        Culture = CultureInfo.CurrentUICulture;
        Providers = new ConcurrentDictionary<string, FileTranslationProvider>();
    }

    public static CultureInfo Culture { get; private set; }

    public static event EventHandler<CultureInfo>? OnCultureChanged;

    public static void SetCulture(CultureInfo culture)
    {
        Culture = culture;
        OnCultureChanged?.Invoke(null, Culture);
    }

    public static string ReadTranslation(Assembly assembly, string file, string key, CultureInfo culture)
    {
        if (Providers.TryGetValue(file, out var provider)) return provider.ReadTranslation(key, culture);

        provider = new FileTranslationProvider(assembly, file);

        Providers[file] = provider;

        return provider.ReadTranslation(key, culture);
    }
}