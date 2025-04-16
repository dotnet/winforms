// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Text.RegularExpressions;

namespace System.Windows.Forms.Tests;

internal static partial class ResourceStrings
{
    [GeneratedRegex(@"{[0-9]}")]
    private static partial Regex PlaceholdersPattern();

    internal static string InvalidTypeFormatCombinationMessage =>
        PlaceholdersPattern().Replace(SR.ClipboardOrDragDrop_InvalidFormatTypeCombination, "*");
    internal static string TypeRequiresResolver => PlaceholdersPattern().Replace(SR.ClipboardOrDragDrop_InvalidType, "*");
    internal static string UseTryGetDataWithResolver => PlaceholdersPattern().Replace(SR.ClipboardOrDragDrop_UseTypedAPI, "*");
    internal static string TypedInterfaceNotImplemented => PlaceholdersPattern().Replace(SR.ITypeDataObject_Not_Implemented, "*");
}
