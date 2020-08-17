// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Windows.Forms;

namespace AccessibilityTests
{
    public partial class DataControls : Form
    {
        public DataControls()
        {
            InitializeComponent();
        }

        private void DataControls_Load(object sender, EventArgs e)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Name");
            dt.Columns.Add("Id");
            dt.Columns.Add("Desc");
            for (int i = 0; i < 20; i++)
            {
                DataRow dr = dt.NewRow();
                dr[0] = "Jack" + i.ToString();
                dr[1] = i * 10;
                dr[2] = "I like" + i.ToString();
                dt.Rows.Add(dr);
            }
            //this.dataGridView2.DataSource = dt;

            bindingSource1.DataSource = dt;
            dataGridView2.DataSource = bindingSource1;
            bindingNavigator1.BindingSource = bindingSource1;

            dataGridView1.Rows[0].Cells[0].Value = "Rose";
            dataGridView1.CurrentCell = dataGridView1.Rows[0].Cells[0];
            dataGridView1.BeginEdit(false);
            DataGridViewComboBoxEditingControl cbox = dataGridView1.EditingControl as DataGridViewComboBoxEditingControl;
            if (cbox != null)
                cbox.DroppedDown = true;
        }
    }
}
