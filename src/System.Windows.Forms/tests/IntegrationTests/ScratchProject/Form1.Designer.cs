// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject;

partial class Form1
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        ListViewGroup listViewGroup1 = new ListViewGroup("Group1", HorizontalAlignment.Left);
        ListViewGroup listViewGroup2 = new ListViewGroup("Group2", HorizontalAlignment.Center);
        ListViewItem listViewItem1 = new ListViewItem(new string[] { "Item1", "1", "2", "3" }, 0);
        ListViewItem listViewItem2 = new ListViewItem(new string[] { "Item2", "4", "5", "6" }, 1);
        ListViewItem listViewItem3 = new ListViewItem(new string[] { "Item3", "7", "8", "9" }, 0);
        listView1 = new ListView();
        columnHeader1 = new ColumnHeader();
        columnHeader2 = new ColumnHeader();
        columnHeader3 = new ColumnHeader();
        columnHeader4 = new ColumnHeader();
        SuspendLayout();
        // 
        // listView1
        // 
        listView1.AllowColumnReorder = true;
        listView1.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader2, columnHeader3, columnHeader4 });
        listView1.Dock = DockStyle.Fill;
        listView1.FullRowSelect = true;
        listView1.GridLines = true;
        listViewGroup1.Header = "Group1";
        listViewGroup1.Name = "listViewGroup1";
        listViewGroup2.Header = "Group2";
        listViewGroup2.HeaderAlignment = HorizontalAlignment.Center;
        listViewGroup2.Name = "listViewGroup2";
        listView1.Groups.AddRange(new ListViewGroup[] { listViewGroup1, listViewGroup2 });
        listViewItem1.StateImageIndex = 0;
        listViewItem2.StateImageIndex = 0;
        listViewItem3.StateImageIndex = 0;
        listView1.Items.AddRange(new ListViewItem[] { listViewItem1, listViewItem2, listViewItem3 });
        listView1.LabelEdit = true;
        listView1.Location = new Point(0, 0);
        listView1.Name = "listView1";
        listView1.Size = new Size(420, 247);
        listView1.Sorting = SortOrder.Ascending;
        listView1.TabIndex = 0;
        listView1.UseCompatibleStateImageBehavior = false;
        listView1.View = View.Tile;
        // 
        // columnHeader1
        // 
        columnHeader1.Text = "ItemColumn";
        // 
        // columnHeader2
        // 
        columnHeader2.Text = "Column2";
        // 
        // columnHeader3
        // 
        columnHeader3.Text = "Column3";
        // 
        // columnHeader4
        // 
        columnHeader4.Text = "Column4";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(420, 247);
        Controls.Add(listView1);
        Name = "Form1";
        Text = "Form1";
        Load += Form1_Load;
        ResumeLayout(false);

    }

    #endregion

    private ListView listView1;
    private ColumnHeader columnHeader1;
    private ColumnHeader columnHeader2;
    private ColumnHeader columnHeader3;
    private ColumnHeader columnHeader4;

}
