// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

partial class ObsoleteControls
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
#pragma warning disable WFDEV005, WFDEV006, WFDEV007, WFDEV008, WFDEV009, WFDEV010 // Type or member is obsolete
        components = new System.ComponentModel.Container();
        button1 = new System.Windows.Forms.Button();
        button2 = new System.Windows.Forms.Button();
        labTitle = new System.Windows.Forms.Label();
        dataGrid1 = new System.Windows.Forms.DataGrid();
        contextMenu1 = new System.Windows.Forms.ContextMenu();
        menuItem1 = new System.Windows.Forms.MenuItem();
        toolBar1 = new System.Windows.Forms.ToolBar();
        toolBarButton1 = new System.Windows.Forms.ToolBarButton();
        toolBarButton2 = new System.Windows.Forms.ToolBarButton();
        statusBar1 = new System.Windows.Forms.StatusBar();
        panel1 = new System.Windows.Forms.StatusBarPanel();
        panel2 = new System.Windows.Forms.StatusBarPanel();
        SuspendLayout();
        // 
        // button1
        // 
        button1.Location = new Point(24, 16);
        button1.Size = new System.Drawing.Size(120, 24);
        button1.Text = "Change Appearance";
        button1.Click += new System.EventHandler(button1_Click);
        // 
        // button2
        // 
        button2.Location = new Point(150, 16);
        button2.Size = new System.Drawing.Size(120, 24);
        button2.Text = "Get Binding Manager";
        button2.Click += new System.EventHandler(button2_Click);
        // 
        // labTitle
        // 
        labTitle.AutoSize = true;
        labTitle.Location = new System.Drawing.Point(469, 33);
        labTitle.Name = "label1";
        labTitle.Size = new System.Drawing.Size(51, 20);
        labTitle.TabIndex = 3;
        labTitle.Text = "label1";
        // 
        // dataGrid1
        // 
        dataGrid1.Location = new Point(24, 50);
        dataGrid1.Size = new Size(300, 200);
        dataGrid1.CaptionText = "Microsoft DataGrid Control";
        dataGrid1.MouseUp += new MouseEventHandler(Grid_MouseUp);
        // dataGrid1.ContextMenu = contextMenu1;
        // 
        // contextMenu1
        // 
        contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            menuItem1});
        // 
        // menuItem1
        // 
        menuItem1.Index = 0;
        menuItem1.Text = "New";
        menuItem1.Click += new System.EventHandler(menuItem1_Click);
        //
        // toolBar1
        //
        toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] { toolBarButton1, toolBarButton2 });
        toolBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
        toolBarButton1.Text = "Save";
        toolBarButton2.Text = "Open";
        //
        // statusBar1
        //
        statusBar1.Location = new System.Drawing.Point(4, 300);
        statusBar1.Size = new System.Drawing.Size(50, 50);
        statusBar1.ShowPanels = true;
        panel1.Text = "Ready";
        panel2.Text = "Loading...";
        statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] { panel1, panel2 });
        // 
        // Form1
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(800, 450);
        Controls.Add(button1);
        Controls.Add(button2);
        Controls.Add(labTitle);
        Controls.Add(dataGrid1);
        Name = "Obsolete-DataGrid";
        Text = "Obsolete-DataGrid";
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.DataGrid dataGrid1;
    private System.Data.DataSet myDataSet;
    private System.Windows.Forms.ContextMenu contextMenu1;
    private System.Windows.Forms.MenuItem menuItem1;
    private System.Windows.Forms.ToolBar toolBar1;
    private System.Windows.Forms.ToolBarButton toolBarButton1;
    private System.Windows.Forms.ToolBarButton toolBarButton2;
    private System.Windows.Forms.StatusBar statusBar1;
    private System.Windows.Forms.StatusBarPanel panel1;
    private System.Windows.Forms.StatusBarPanel panel2;
    private System.Windows.Forms.Label labTitle;
#pragma warning restore WFDEV005, WFDEV006, WFDEV007, WFDEV008, WFDEV009, WFDEV010
}
