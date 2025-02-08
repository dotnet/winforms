// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;
internal class DataGridViewColumnDataPropertyNameEditor : UITypeEditor
{
    // FxCop made me add this constructor.
    public DataGridViewColumnDataPropertyNameEditor() : base() { }

    private DesignBindingPicker? _designBindingPicker;

    public override bool IsDropDownResizable
    {
        get
        {
            return true;
        }
    }

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is not null && context is not null && context.Instance is not null)
        {
            DataGridViewColumnCollectionDialog.ListBoxItem? item = context.Instance as DataGridViewColumnCollectionDialog.ListBoxItem;
            DataGridView? dataGridView;
            if (item is not null)
            {
                dataGridView = item.DataGridViewColumn.DataGridView;
            }
            else
            {
                DataGridViewColumn? col = context.Instance as DataGridViewColumn;
                dataGridView = col?.DataGridView;
            }

            if (dataGridView is null)
            {
                return value;
            }

            object? dataSource = dataGridView.DataSource;
            string? dataMember = dataGridView.DataMember;
            string? valueString = value as string;
            string? selectedMember = dataMember + "." + valueString;
            if (dataSource is null)
            {
                dataMember = string.Empty;
                selectedMember = valueString;
            }

            _designBindingPicker ??= new DesignBindingPicker();

            DesignBinding oldSelection = new DesignBinding(dataSource, selectedMember);
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
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
    {
        return UITypeEditorEditStyle.DropDown;
    }
}
