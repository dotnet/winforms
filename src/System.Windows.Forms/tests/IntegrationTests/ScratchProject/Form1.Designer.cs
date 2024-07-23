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
        this.components = new System.ComponentModel.Container();
        listView1 = new System.Windows.Forms.ListView();
        // 
        // listView1
        // 
        listView1.Location = new Point(190, 269);
        listView1.Name = "listView1";
        listView1.Size = new Size(121, 97);
        listView1.TabIndex = 1;
        listView1.View = View.List;
        listView1.VirtualMode =true;
        listView1.VirtualListSize = 2;
        listView1.UseCompatibleStateImageBehavior = false;
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Text = "Form1";
        this.Controls.Add(listView1);
    }
    private System.Windows.Forms.ListView listView1;

    #endregion
}
