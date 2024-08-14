// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

/// <summary>
///  Provides an editor for visually picking a color.
/// </summary>
[CLSCompliant(false)]
public partial class ColorEditor : UITypeEditor
{
    /// <summary>
    ///  Edits the given object value using the editor style provided by ColorEditor.GetEditStyle.
    /// </summary>
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        using ColorUI? _colorUI = new(this);

        _colorUI.Start(editorService, value);
        editorService.DropDownControl(_colorUI);

        if (_colorUI.Value is Color colorValue && colorValue != Color.Empty)
        {
            value = colorValue;
        }

        _colorUI.End();
        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;

    /// <inheritdoc />
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => true;

    /// <inheritdoc />
    public override void PaintValue(PaintValueEventArgs e)
    {
        if (e.Value is Color color)
        {
            using var brush = color.GetCachedSolidBrushScope();
            e.Graphics.FillRectangle(brush, e.Bounds);
        }
    }
}
