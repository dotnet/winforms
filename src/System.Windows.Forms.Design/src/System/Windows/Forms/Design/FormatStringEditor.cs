// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class FormatStringEditor : UITypeEditor
{
    private FormatStringDialog? _formatStringDialog;

    /// Edits the specified value using the specified provider within the specified context.
    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (!provider.TryGetService(out IWindowsFormsEditorService? editorService))
        {
            return value;
        }

        object component = context?.Instance!;
        DataGridViewCellStyle? cellStyle = component as DataGridViewCellStyle;
        ListControl? listControl = component as ListControl;

        Debug.Assert(
            listControl is not null || cellStyle is not null,
            "this editor is used for the DataGridViewCellStyle::Format and the ListControl::FormatString properties");

        Application.SetHighDpiMode(HighDpiMode.SystemAware);

        _formatStringDialog ??= new FormatStringDialog(context);

        if (listControl is not null)
        {
            _formatStringDialog.ListControl = listControl;
        }
        else
        {
            _formatStringDialog.DataGridViewCellStyle = cellStyle!;
        }

        if (provider.TryGetService(out IComponentChangeService? changeService))
        {
            if (cellStyle is not null)
            {
                changeService.OnComponentChanging(cellStyle, TypeDescriptor.GetProperties(cellStyle)["Format"]);
                changeService.OnComponentChanging(cellStyle, TypeDescriptor.GetProperties(cellStyle)["NullValue"]);
                changeService.OnComponentChanging(cellStyle, TypeDescriptor.GetProperties(cellStyle)["FormatProvider"]);
            }
            else
            {
                changeService.OnComponentChanging(component, TypeDescriptor.GetProperties(component)["FormatString"]);
                changeService.OnComponentChanging(component, TypeDescriptor.GetProperties(component)["FormatInfo"]);
            }
        }

        editorService.ShowDialog(_formatStringDialog);
        FormatStringDialog.End();

        if (!_formatStringDialog.Dirty)
        {
            return value;
        }

        // Since the bindings may have changed, the properties listed in the properties window need to be refreshed.
        TypeDescriptor.Refresh(component);

        if (changeService is not null)
        {
            if (cellStyle is not null)
            {
                changeService.OnComponentChanged(cellStyle, TypeDescriptor.GetProperties(cellStyle)["Format"]);
                changeService.OnComponentChanged(cellStyle, TypeDescriptor.GetProperties(cellStyle)["NullValue"]);
                changeService.OnComponentChanged(cellStyle, TypeDescriptor.GetProperties(cellStyle)["FormatProvider"]);
            }
            else
            {
                changeService.OnComponentChanged(component, TypeDescriptor.GetProperties(component)["FormatString"]);
                changeService.OnComponentChanged(component, TypeDescriptor.GetProperties(component)["FormatInfo"]);
            }
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
        => UITypeEditorEditStyle.Modal;
}
