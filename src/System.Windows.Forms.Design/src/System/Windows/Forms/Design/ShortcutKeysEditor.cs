// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides an editor for picking shortcut keys.
/// </summary>
[CLSCompliant(false)]
public partial class ShortcutKeysEditor : UITypeEditor
{
    private ShortcutKeysUI? _shortcutKeysUI;

    /// <summary>
    ///  Edits the given object value using the editor style provided by ShortcutKeysEditor.GetEditStyle.
    /// </summary>
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _shortcutKeysUI ??= new ShortcutKeysUI()
        {
            BackColor = SystemColors.Control
        };

        _shortcutKeysUI.Start(value);
        editorService.DropDownControl(_shortcutKeysUI);

        if (_shortcutKeysUI.Value is not null)
        {
            value = _shortcutKeysUI.Value;
        }

        _shortcutKeysUI.End();
        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
