// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinformsControlsTest;

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
#pragma warning disable CS0618 // Type or member is obsolete
        this.components = new System.ComponentModel.Container();
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.dataGrid1 = new System.Windows.Forms.DataGrid();
        this.contextMenu1 = new System.Windows.Forms.ContextMenu();
        this.menuItem1 = new System.Windows.Forms.MenuItem();
        this.toolBar1 = new System.Windows.Forms.ToolBar();
        this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
        this.toolBarButton2 = new System.Windows.Forms.ToolBarButton();
        this.statusBar1 = new System.Windows.Forms.StatusBar();
        this.panel1 = new System.Windows.Forms.StatusBarPanel();
        this.panel2 = new System.Windows.Forms.StatusBarPanel();
        this.SuspendLayout();
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
        // dataGrid1
        // 
        dataGrid1.Location = new Point(24, 50);
        dataGrid1.Size = new Size(300, 200);
        dataGrid1.CaptionText = "Microsoft DataGrid Control";
        dataGrid1.MouseUp += new MouseEventHandler(Grid_MouseUp);
        // dataGrid1.ContextMenu = this.contextMenu1;
        // 
        // contextMenu1
        // 
        this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1});
        // 
        // menuItem1
        // 
        this.menuItem1.Index = 0;
        this.menuItem1.Text = "New";
        this.menuItem1.Click += new System.EventHandler(this.menuItem1_Click);
        //
        // toolBar1
        //
        this.toolBar1.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[] { this.toolBarButton1, this.toolBarButton2 });
        this.toolBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.toolBarButton1.Text = "Save";
        this.toolBarButton2.Text = "Open";
        //
        // statusBar1
        //
        this.statusBar1.Location = new System.Drawing.Point(4, 300);
        this.statusBar1.Size = new System.Drawing.Size(50, 50);
        this.statusBar1.ShowPanels = true;
        this.panel1.Text = "Ready";
        this.panel2.Text = "Loading...";
        this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] { this.panel1, this.panel2 });
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.dataGrid1);
        this.Name = "Obsolete-DataGrid";
        this.Text = "Obsolete-DataGrid";
        this.ResumeLayout(false);
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
#pragma warning restore CS0618 // Type or member is obsolete
}
