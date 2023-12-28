// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class DataControls : Form
{
    public DataControls()
    {
        InitializeComponent();
    }

    private void DataControls_Load(object sender, EventArgs e)
    {
        DataTable dataTable = new();
        dataTable.Columns.Add("Name");
        dataTable.Columns.Add("Id");
        dataTable.Columns.Add("Desc");
        for (int i = 0; i < 20; i++)
        {
            DataRow dataRow = dataTable.NewRow();
            dataRow[0] = $"Jack{i}";
            dataRow[1] = i * 10;
            dataRow[2] = $"I like{i}";
            dataTable.Rows.Add(dataRow);
        }

        bindingSource1.DataSource = dataTable;
        dataGridView2.DataSource = bindingSource1;
        bindingNavigator1.BindingSource = bindingSource1;

        dataGridView1.Rows[0].Cells[0].Value = "Rose";
        dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
        dataGridView1.BeginEdit(false);
        DataGridViewComboBoxEditingControl cbox = dataGridView1.EditingControl as DataGridViewComboBoxEditingControl;
        if (cbox is not null)
            cbox.DroppedDown = true;
    }

    private void bindingNavigatorAddNewItem_Click(object sender, EventArgs e)
    {
        dataGridView2.Focus();
        bindingNavigator1.AccessibilityObject.RaiseAutomationNotification(
            System.Windows.Forms.Automation.AutomationNotificationKind.Other,
          System.Windows.Forms.Automation.AutomationNotificationProcessing.CurrentThenMostRecent,
          "Please enter first name now");
    }
}
