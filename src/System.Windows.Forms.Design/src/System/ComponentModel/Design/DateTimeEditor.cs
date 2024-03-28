// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design;

/// <summary>
///  This date/time editor is a <see cref="UITypeEditor"/> suitable for visually editing
///  <see cref="DateTime"/> objects.
/// </summary>
public partial class DateTimeEditor : UITypeEditor
{
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        using (DateTimeUI dateTimeUI = new(editorService, value))
        {
            editorService.DropDownControl(dateTimeUI);
            value = dateTimeUI.Value;
        }

        return value;
    }

    /// <inheritdoc />
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
