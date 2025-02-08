// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class DataMemberListEditor : UITypeEditor
{
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
            PropertyDescriptor? dataSourceProperty = TypeDescriptor.GetProperties(context.Instance)["DataSource"];
            if (dataSourceProperty is not null)
            {
                object? dataSource = dataSourceProperty.GetValue(context.Instance);
                _designBindingPicker ??= new DesignBindingPicker();

                DesignBinding oldSelection = new DesignBinding(dataSource, value as string);
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
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
    {
        return UITypeEditorEditStyle.DropDown;
    }
}
