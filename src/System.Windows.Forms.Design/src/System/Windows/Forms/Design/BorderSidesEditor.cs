// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for setting the ToolStripStatusLabel BorderSides property..
/// </summary>
[CLSCompliant(false)]
public partial class BorderSidesEditor : UITypeEditor
{
    private BorderSidesEditorUI? _borderSidesEditorUI;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _borderSidesEditorUI ??= new BorderSidesEditorUI();

        _borderSidesEditorUI.Start(editorService, value);
        editorService.DropDownControl(_borderSidesEditorUI);

        if (_borderSidesEditorUI.Value is not null)
        {
            value = _borderSidesEditorUI.Value;
        }

        _borderSidesEditorUI.End();
        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
