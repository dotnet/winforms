// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

/// <summary>
///  Provides an editor that can perform default file searching for cursor (.cur) files.
/// </summary>
[CLSCompliant(false)]
public partial class CursorEditor : UITypeEditor
{
    private CursorUI? _cursorUI;

    /// <summary>
    ///  Returns true, indicating that this drop-down control can be resized.
    /// </summary>
    public override bool IsDropDownResizable => true;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _cursorUI ??= new CursorUI();

        _cursorUI.Start(editorService, value);
        editorService.DropDownControl(_cursorUI);
        value = _cursorUI.Value;
        _cursorUI.End();

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
