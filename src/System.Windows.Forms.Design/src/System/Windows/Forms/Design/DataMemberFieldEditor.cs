// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class DataMemberFieldEditor : UITypeEditor
{
    private DesignBindingPicker? _designBindingPicker;

    public override bool IsDropDownResizable => true;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is null || context is null || context?.Instance is not { } instance)
        {
            return value;
        }

        if (TypeDescriptor.GetProperties(instance)[nameof(ComboBox.DataSource)] is not PropertyDescriptor property)
        {
            return value;
        }

        object? dataSource = property.GetValue(instance);

        if (dataSource is null )
        {
            return value;
        }

        _designBindingPicker ??= new();

        DesignBinding oldSelection = new DesignBinding(dataSource, (string?)value);
        DesignBinding? newSelection = _designBindingPicker.Pick(
            context,
            provider,
            showDataSources: false,
            showDataMembers: true,
            selectListMembers: false,
            rootDataSource: dataSource,
            rootDataMember: string.Empty,
            initialSelectedItem: oldSelection
        );

        if (newSelection is null)
        {
            return value;
        }

        return newSelection.DataMember;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
