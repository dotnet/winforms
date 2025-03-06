// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class DataMemberListEditor : UITypeEditor
{
    private DesignBindingPicker? _designBindingPicker;

    public override bool IsDropDownResizable => true;

    public override object? EditValue(ITypeDescriptorContext? context, IServiceProvider provider, object? value)
    {
        if (provider is null || context is null || context.Instance is null)
        {
            return value;
        }

        if (TypeDescriptor.GetProperties(context.Instance)[nameof(ComboBox.DataSource)] is { } dataSourceProperty)
        {
            object? dataSource = dataSourceProperty.GetValue(context.Instance);
            _designBindingPicker ??= new();

            DesignBinding oldSelection = new(dataSource, value as string);
            DesignBinding? newSelection = _designBindingPicker.Pick(context,
                provider,
                showDataSources: false,
                showDataMembers: true,
                selectListMembers: true,
                dataSource,
                string.Empty,
                oldSelection);
            if (dataSource is not null && newSelection is not null)
            {
                value = newSelection.DataMember;
            }
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context) => UITypeEditorEditStyle.DropDown;
}
