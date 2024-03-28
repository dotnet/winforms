// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Collections;

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a base designer for data grid view columns.
/// </summary>
internal class DataGridViewComboBoxColumnDesigner : DataGridViewColumnDesigner
{
    private static BindingContext? s_bindingContext;

    private string ValueMember
    {
        get
        {
            DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)Component;
            return dataGridViewComboBoxColumn.ValueMember;
        }
        set
        {
            DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)Component;
            if (dataGridViewComboBoxColumn.DataSource is null)
            {
                return;
            }

            if (ValidDataMember(dataGridViewComboBoxColumn.DataSource, dataMember: value))
            {
                dataGridViewComboBoxColumn.ValueMember = value;
            }
        }
    }

    private string DisplayMember
    {
        get
        {
            DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)Component;
            return dataGridViewComboBoxColumn.DisplayMember;
        }
        set
        {
            DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)Component;
            if (dataGridViewComboBoxColumn.DataSource is null)
            {
                return;
            }

            if (ValidDataMember(dataGridViewComboBoxColumn.DataSource, value))
            {
                dataGridViewComboBoxColumn.DisplayMember = value;
            }
        }
    }

    private bool ShouldSerializeDisplayMember()
    {
        DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)Component;
        return !string.IsNullOrEmpty(dataGridViewComboBoxColumn.DisplayMember);
    }

    private bool ShouldSerializeValueMember()
    {
        DataGridViewComboBoxColumn dataGridViewComboBoxColumn = (DataGridViewComboBoxColumn)Component;
        return !string.IsNullOrEmpty(dataGridViewComboBoxColumn.ValueMember);
    }

    private static bool ValidDataMember(object dataSource, string dataMember)
    {
        if (string.IsNullOrEmpty(dataMember))
        {
            // A null string is a valid value
            return true;
        }

        s_bindingContext ??= new BindingContext();

        BindingMemberInfo bindingMemberInfo = new(dataMember);
        BindingManagerBase bindingManagerBase;

        try
        {
            bindingManagerBase = s_bindingContext[dataSource, bindingMemberInfo.BindingPath];
        }
        catch (ArgumentException)
        {
            return false;
        }

        return bindingManagerBase is not null
            && (bindingManagerBase.GetItemProperties()?[bindingMemberInfo.BindingField]) is not null;
    }

    protected override void PreFilterProperties(IDictionary properties)
    {
        base.PreFilterProperties(properties);

        PropertyDescriptor? property = (PropertyDescriptor?)properties["ValueMember"];
        if (property is not null)
        {
            properties["ValueMember"] = TypeDescriptor.CreateProperty(typeof(DataGridViewComboBoxColumnDesigner), property, []);
        }

        property = (PropertyDescriptor?)properties["DisplayMember"];
        if (property is not null)
        {
            properties["DisplayMember"] = TypeDescriptor.CreateProperty(typeof(DataGridViewComboBoxColumnDesigner), property, []);
        }
    }
}
