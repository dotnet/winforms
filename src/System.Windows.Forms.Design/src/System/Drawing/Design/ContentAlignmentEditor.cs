// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

/// <summary>
///  Provides a <see cref="UITypeEditor"/> for visually editing content alignment.
/// </summary>
public partial class ContentAlignmentEditor : UITypeEditor
{
    private ContentUI? _contentUI;

    /// <summary>
    ///  Edits the given object value using the editor style provided by GetEditStyle.
    /// </summary>
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        _contentUI ??= new ContentUI();

        _contentUI.Start(editorService, value);
        editorService.DropDownControl(_contentUI);
        value = _contentUI.Value;
        _contentUI.End();

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        => UITypeEditorEditStyle.DropDown;
}
