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

public sealed class Translate(TranslationUnit unit) : MarkupExtension
{
    private readonly object[] args = [];
    private readonly IBinding[] bindings = [];
    private readonly IBinding formatBinding = unit.Value.ToBinding();

    public Translate(TranslationUnit unit, object obj1) : this(unit)
    {
        args = [obj1];
        bindings = [formatBinding, .. CheckTypes(args)];
    }

    public Translate(TranslationUnit unit, object obj1, object obj2) : this(unit)
    {
        args = [obj1, obj2];
        bindings = [formatBinding, .. CheckTypes(args)];
    }

    public Translate(TranslationUnit unit, object obj1, object obj2, object obj3) :
        this(unit)
    {
        args = [obj1, obj2, obj3];
        bindings = [formatBinding, .. CheckTypes(args)];
    }

    public Translate(TranslationUnit unit, object obj1, object obj2, object obj3,
        object obj4) : this(unit)
    {
        args = [obj1, obj2, obj3, obj4];
        bindings = [formatBinding, .. CheckTypes(args)];
    }

    public Translate(TranslationUnit unit, object obj1, object obj2, object obj3,
        object obj4, object obj5) : this(unit)
    {
        args = [obj1, obj2, obj3, obj4, obj5];
        bindings = [formatBinding, .. CheckTypes(args)];
    }

    public Translate(TranslationUnit unit, object obj1, object obj2, object obj3,
        object obj4, object obj5, object obj6) : this(unit)
    {
        args = [obj1, obj2, obj3, obj4, obj5, obj6];
        bindings = [formatBinding, .. CheckTypes(args)];
    }

    private static IBinding[] CheckTypes(object[] args)
    {
        if (!args.Any(arg => arg is BindingBase)) return [];

        if (!args.All(arg => arg is BindingBase)) throw new Exception("Cannot mix bindings with variables");
        return [.. args.OfType<BindingBase>()];
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        //var target = serviceProvider.GetService(typeof(IProvideValueTarget)) as IProvideValueTarget;
        //var targetObject = target.TargetObject as AvaloniaObject;
        //var targetProperty = target.TargetProperty as AvaloniaProperty;
        //var targetType = targetProperty?.PropertyType;

        if (bindings.Length > 1)
            return new MultiBinding
            {
                Bindings = bindings,
                Converter = BindingConverter.Instance
            };

        return unit.Value
            .Select(str => string.IsNullOrEmpty(str) ? str : string.Format(str, args))
            .ToBinding();
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