// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            (new Buttons()).Show();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            (new DesignTimeAligned()).Show();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            (new TreeViewTest()).Show();
        }

        private void calendar_Click(object sender, EventArgs e)
        {
            (new Calendar()).Show();
        }

        private void multipleControls_Click(object sender, EventArgs e)
        {
            (new MultipleControls()).Show();
        }

        private void menuesButton_Click(object sender, EventArgs e)
        {
            (new MenuStripAndCheckedListBox()).Show();
        }

        private void dataGridViewButton_Click(object sender, EventArgs e)
        {
            (new DataGridViewHeaders()).Show();
        }

        private void ComboBoxesButton_Click(object sender, EventArgs e)
        {
            (new ComboBoxes()).Show();
        }

        private void splitterButton_Click(object sender, EventArgs e)
        {
            (new Splitter()).Show();
        }

        private void panelsButton_Click(object sender, EventArgs e)
        {
            (new Panels()).Show();
        }

        private void mdiParent_Click(object sender, EventArgs e)
        {
            (new MDIParent()).Show();
        }

        private void propertyGrid_Click(object sender, EventArgs e)
        {
            (new PropertyGrid()).Show();
        }

        private void MenuForm_Load(object sender, EventArgs e)
        {

        }

        private void checkedListViewButton_Click(object sender, EventArgs e)
        {
            // (new CheckedListView()).Show();
        }

        private void listViewButton_Click(object sender, EventArgs e)
        {
            (new ListViewTest()).Show();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            (new DateTimePicker()).Show();
        }

        private void folderBrowserDialogButton_Click(object sender, EventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.ShowDialog();
        }
    }
}
