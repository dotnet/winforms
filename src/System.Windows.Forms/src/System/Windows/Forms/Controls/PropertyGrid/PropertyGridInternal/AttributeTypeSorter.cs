// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms.PropertyGridInternal;

internal class AttributeTypeSorter : IComparer, IComparer<Attribute>
{
    private static readonly ConditionalWeakTable<Attribute, string> s_typeIds = [];

    public int Compare(object? x, object? y) => Compare(x as Attribute, y as Attribute);

    public int Compare(Attribute? x, Attribute? y)
    {
        if (IComparerHelpers.CompareReturnIfNull(x, y, out int? returnValue))
        {
            return (int)returnValue;
        }

        return string.Compare(GetTypeIdString(x), GetTypeIdString(y), ignoreCase: false, CultureInfo.InvariantCulture);
    }

    private static string? GetTypeIdString(Attribute attribute)
    {
        if (s_typeIds.TryGetValue(attribute, out string? result))
        {
            return result;
        }

        object? typeId = attribute.TypeId;

        if (typeId is null)
        {
            Debug.Fail($"Attribute '{attribute.GetType().FullName}' does not have a typeid.");
            return string.Empty;
        }

        result = typeId.ToString();
        s_typeIds.AddOrUpdate(attribute, result ?? string.Empty);
        return result;
    }
}
