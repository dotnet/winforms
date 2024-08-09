// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class MessageBoxes
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
        this.resources = new System.ComponentModel.ComponentResourceManager(typeof(Dialogs));
        this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        this.SuspendLayout();
        // 
        // propertyGrid1
        // 
        this.propertyGrid1.CommandsBorderColor = System.Drawing.SystemColors.Control;
        this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid1.Location = new System.Drawing.Point(171, 0);
        this.propertyGrid1.Margin = new System.Windows.Forms.Padding(2);
        this.propertyGrid1.Name = "propertyGrid1";
        this.propertyGrid1.Size = new System.Drawing.Size(332, 430);
        this.propertyGrid1.TabIndex = 0;
        this.propertyGrid1.SelectedObjectsChanged += this.propertyGrid1_SelectedObjectsChanged;
        // 
        // Dialogs
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(503, 430);
        this.Controls.Add(this.propertyGrid1);
        this.Name = "Dialogs";
        this.Text = "Dialogs Test";
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private System.ComponentModel.ComponentResourceManager resources;
}
