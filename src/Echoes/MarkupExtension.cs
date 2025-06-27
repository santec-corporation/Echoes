using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Markup.Xaml;

namespace Echoes;

public sealed class Translate(TranslationUnit unit, params object?[] args) : MarkupExtension
{
    private readonly IBinding[] bindings = [];
    private readonly IBinding formatBinding = unit.Value.ToBinding();

    public Translate(TranslationUnit unit) : this(unit, [])
    {
    }

    public Translate(TranslationUnit unit, BindingBase binding1) : this(unit, [])
    {
        bindings = [formatBinding, binding1];
    }

    public Translate(TranslationUnit unit, BindingBase binding1, BindingBase binding2) : this(unit, [])
    {
        bindings = [formatBinding, binding1, binding2];
    }

    public Translate(TranslationUnit unit, BindingBase binding1, BindingBase binding2, BindingBase binding3) :
        this(unit, [])
    {
        bindings = [formatBinding, binding1, binding2, binding3];
    }

    public Translate(TranslationUnit unit, BindingBase binding1, BindingBase binding2, BindingBase binding3,
        BindingBase binding4) : this(unit, [])
    {
        bindings = [formatBinding, binding1, binding2, binding3, binding4];
    }

    public Translate(TranslationUnit unit, BindingBase binding1, BindingBase binding2, BindingBase binding3,
        BindingBase binding4, BindingBase binding5) : this(unit, [])
    {
        bindings = [formatBinding, binding1, binding2, binding3, binding4, binding5];
    }

    public Translate(TranslationUnit unit, BindingBase binding1, BindingBase binding2, BindingBase binding3,
        BindingBase binding4, BindingBase binding5, BindingBase binding6) : this(unit, [])
    {
        bindings = [formatBinding, binding1, binding2, binding3, binding4, binding5, binding6];
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        //var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        //var targetObject = target.TargetObject as AvaloniaObject;
        //var targetProperty = target.TargetProperty as AvaloniaProperty;
        //var targetType = targetProperty?.PropertyType;

        return (bindings.Length, args.Length) switch
        {
            (0, 0) => formatBinding,
            (_, > 0) => unit.Value.Select(str => string.IsNullOrEmpty(str) ? str : string.Format(str, args))
                .ToBinding(),
            (> 0, _) => new MultiBinding
            {
                Bindings = bindings,
                Converter = BindingConverter.Instance
            },
            _ => formatBinding
        };
    }

    private class BindingConverter : IMultiValueConverter
    {
        private BindingConverter()
        {
        }

        public static BindingConverter Instance { get; } = new();

        public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        {
            if (values.Count == 0) return string.Empty;
            var format = values[0] as string;
            if (string.IsNullOrEmpty(format)) return string.Empty;
            return string.Format(culture, format, values.Skip(1).ToArray());
        }
    }
}