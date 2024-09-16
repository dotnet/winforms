// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.ComponentModel;

internal static class TypeConverterHelper
{
    // Feature switch, when set to true, used for trimming to access ComponentModel in a trim safe manner
    [FeatureSwitchDefinition("System.Windows.Forms.Primitives.TypeConverterHelper.UseComponentModelRegisteredTypes")]
#pragma warning disable IDE0075 // Simplify conditional expression - the simpler expression is hard to read
    private static bool UseComponentModelRegisteredTypes { get; } =
        AppContext.TryGetSwitch("System.Windows.Forms.Primitives.TypeConverterHelper.UseComponentModelRegisteredTypes", out bool isEnabled)
            ? isEnabled
            : false;
#pragma warning restore IDE0075

    /// <summary>
    /// Converts the given text to list of objects, using the specified context and culture information.
    /// </summary>
    /// <param name="context">An ITypeDescriptorContext that provides a format context.</param>
    /// <param name="culture">A CultureInfo. If null is passed, the current culture is assumed.</param>
    /// <param name="text">The chars to convert.</param>
    /// <param name="output">The converted text chunks</param>
    /// <typeparam name="T"></typeparam>
    /// <returns>true if text was converted successfully; otherwise, false.</returns>
    public static bool TryParseAsSpan<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ITypeDescriptorContext? context, CultureInfo? culture, ReadOnlySpan<char> text, Span<T> output)
    {
        culture ??= CultureInfo.CurrentCulture;

        using BufferScope<Range> tokens = new(stackalloc Range[16]);
        int tokensCount = text.Split(tokens, culture.TextInfo.ListSeparator[0]);

        if (tokensCount != output.Length)
        {
            return false;
        }

        TypeConverter converter;
        if (!UseComponentModelRegisteredTypes)
        {
            converter = TypeDescriptor.GetConverter(typeof(T));
        }
        else
        {
            // Call the trim safe API
            TypeDescriptor.RegisterType<T>();
            converter = TypeDescriptor.GetConverterFromRegisteredType(typeof(T));
        }

        for (int i = 0; i < output.Length; i++)
        {
            // Note: ConvertFromString will raise exception if value cannot be converted.
            output[i] = (T)converter.ConvertFromString(context, culture, text[tokens[i]].ToString())!;
        }

        return true;
    }
}
