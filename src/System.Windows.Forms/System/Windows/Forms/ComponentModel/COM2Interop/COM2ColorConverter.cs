// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This class maps an OLE_COLOR to a managed Color editor.
/// </summary>
internal sealed class Com2ColorConverter : Com2DataTypeToManagedDataTypeConverter
{
    public override Type ManagedType => typeof(Color);

    public override object ConvertNativeToManaged(VARIANT nativeValue, Com2PropertyDescriptor property)
        => ColorTranslator.FromOle(nativeValue.Type switch
        {
            VARENUM.VT_UI4 or VARENUM.VT_UINT => (int)(uint)nativeValue,
            VARENUM.VT_I4 or VARENUM.VT_INT => (int)nativeValue,
            _ => 0
        });

    public override VARIANT ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor property, ref bool cancelSet)
    {
        // Don't cancel the set.
        cancelSet = false;

        // We default to black.
        managedValue ??= Color.Black;

        if (managedValue is Color managedValueAsColor)
        {
            return (VARIANT)ColorTranslator.ToOle(managedValueAsColor);
        }

        Debug.Fail($"Don't know how to set type: {managedValue.GetType().Name}");
        return (VARIANT)0;
    }
}
