// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Drawing.Design;

/// <summary>
///  Provides an editor that provides a way to visually edit the values of a COM2 type.
/// </summary>
internal class Com2ExtendedUITypeEditor : UITypeEditor
{
    private readonly UITypeEditor? _innerEditor;

    public Com2ExtendedUITypeEditor(UITypeEditor? baseTypeEditor)
    {
        _innerEditor = baseTypeEditor;
    }

    public Com2ExtendedUITypeEditor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type baseType)
    {
        _innerEditor = (UITypeEditor?)TypeDescriptor.GetEditor(baseType, typeof(UITypeEditor));
    }

    public UITypeEditor? InnerEditor => _innerEditor;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
        => _innerEditor is not null
            ? _innerEditor.EditValue(context, provider, value)
            : base.EditValue(context, provider, value);

    public override bool GetPaintValueSupported(ITypeDescriptorContext? context)
        => _innerEditor is not null
            ? _innerEditor.GetPaintValueSupported(context)
            : base.GetPaintValueSupported(context);

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        => _innerEditor is not null
            ? _innerEditor.GetEditStyle(context)
            : base.GetEditStyle(context);

    public override void PaintValue(PaintValueEventArgs e)
    {
        _innerEditor?.PaintValue(e);
        base.PaintValue(e);
    }
}
