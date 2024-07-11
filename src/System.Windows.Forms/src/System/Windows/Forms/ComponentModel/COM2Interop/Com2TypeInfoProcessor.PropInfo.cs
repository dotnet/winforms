// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.ComponentModel.Com2Interop;

internal static partial class Com2TypeInfoProcessor
{
    private class PropertyInfo
    {
        public const int ReadOnlyUnknown = 0;
        public const int ReadOnlyTrue = 1;
        public const int ReadOnlyFalse = 2;

#pragma warning disable IDE0036 // required must come first
#pragma warning disable SA1206 // Declaration keywords should follow order
        required public string Name { get; init; }
#pragma warning restore SA1206
#pragma warning restore IDE0036

        public int DispId { get; set; } = PInvokeCore.DISPID_UNKNOWN;

        public Type? ValueType { get; set; }

        public List<Attribute> Attributes { get; } = [];

        public int ReadOnly { get; set; } = ReadOnlyUnknown;

        public bool IsDefault { get; set; }

        public object? TypeData { get; set; }

        public bool NonBrowsable { get; set; }

        public int Index { get; set; }

        public override int GetHashCode() => Name?.GetHashCode() ?? base.GetHashCode();
    }
}
