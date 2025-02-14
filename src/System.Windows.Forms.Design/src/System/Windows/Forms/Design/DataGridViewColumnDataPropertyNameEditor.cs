// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;
internal class DataGridViewColumnDataPropertyNameEditor : UITypeEditor
{
    public DataGridViewColumnDataPropertyNameEditor() : base() { }

    private DesignBindingPicker? _designBindingPicker;

    public override bool IsDropDownResizable => true;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is null || context is null || context.Instance is null)
        {
            return value;
        }

        DataGridView? dataGridView;
        if (context.Instance is DataGridViewColumnCollectionDialog.ListBoxItem item)
        {
            dataGridView = item.DataGridViewColumn.DataGridView;
        }
        else
        {
            dataGridView = (context.Instance as DataGridViewColumn)?.DataGridView;
        }

        if (dataGridView is null)
        {
            return value;
        }

        object? dataSource = dataGridView.DataSource;
        string? dataMember = dataGridView.DataMember;

        ArgumentNullException.ThrowIfNull(value);
        string valueString = (string)value;
        string? selectedMember = $"{dataMember}.{valueString}";
        if (dataSource is null)
        {
            dataMember = string.Empty;
            selectedMember = valueString;
        }

        _designBindingPicker ??= new();

        DesignBinding oldSelection = new(dataSource, selectedMember);
        DesignBinding? newSelection = _designBindingPicker.Pick(context,
            provider,
            showDataSources: false,
            showDataMembers: true,
            selectListMembers: false,
            dataSource,
            dataMember,
            oldSelection);
        if (dataSource is not null && newSelection is not null)
        {
            value = newSelection.DataField;
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
