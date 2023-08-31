// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  An editor for editing strings that supports multiple lines of text and is resizable.
/// </summary>
public sealed partial class MultilineStringEditor : UITypeEditor
{
    private MultilineStringEditorUI? _editorUI;

    /// <inheritdoc />
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _editorUI ??= new MultilineStringEditorUI();

        _editorUI.BeginEdit(editorService, value);
        editorService.DropDownControl(_editorUI);
        object newValue = _editorUI.Value;

        return _editorUI.EndEdit() ? newValue : value;
    }

    /// <summary>
    ///  The MultilineStringEditor is a drop down editor, so this returns UITypeEditorEditStyle.DropDown.
    /// </summary>
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;

    /// <summary>
    ///  Returns false; no extra painting is performed.
    /// </summary>
    public override bool GetPaintValueSupported(ITypeDescriptorContext? context) => false;
}
