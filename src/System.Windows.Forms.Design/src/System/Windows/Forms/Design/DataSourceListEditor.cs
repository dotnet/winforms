// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing.Design;

namespace System.Windows.Forms.Design;

internal class DataSourceListEditor : UITypeEditor
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
            _designBindingPicker ??= new DesignBindingPicker();

            DesignBinding oldSelection = new(value, "");
            DesignBinding? newSelection = _designBindingPicker.Pick(context, provider,
                                                                  true,  /* showDataSources   */
                                                                  false, /* showDataMembers   */
                                                                  false, /* selectListMembers */
                                                                  null, string.Empty,
                                                                  oldSelection);
            if (newSelection is not null)
            {
                value = newSelection.DataSource;
            }
        }

        return value;
    }

    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext? context)
    {
        return UITypeEditorEditStyle.DropDown;
    }
}
