// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;

namespace System.ComponentModel;

internal static class TypeConverterHelper
{
    public static bool TryParseAsSpan<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(ITypeDescriptorContext? context, CultureInfo? culture, ReadOnlySpan<char> text, Span<T> output)
    {
        culture ??= CultureInfo.CurrentCulture;

        Span<Range> tokens = stackalloc Range[output.Length + 1];
        int tokensCount = text.Split(tokens, culture.TextInfo.ListSeparator[0]);

        if (tokensCount != output.Length)
        {
            return false;
        }

        TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
        for (int i = 0; i < output.Length; i++)
        {
            // Note: ConvertFromString will raise exception if value cannot be converted.
            output[i] = (T)converter.ConvertFromString(context, culture, text[tokens[i]].ToString())!;
        }

        return true;
    }
}
