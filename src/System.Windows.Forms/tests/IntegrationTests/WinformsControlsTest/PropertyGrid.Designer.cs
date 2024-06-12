// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class PropertyGrid
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
        this.propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        this.SuspendLayout();
        //
        // propertyGrid1
        //
        this.propertyGrid1.CommandsBorderColor = System.Drawing.SystemColors.Control;
        this.propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.propertyGrid1.Location = new System.Drawing.Point(0, 0);
        this.propertyGrid1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
        this.propertyGrid1.Name = "propertyGrid1";
        this.propertyGrid1.Size = new System.Drawing.Size(465, 413);
        this.propertyGrid1.TabIndex = 0;
        //
        // PropertyGrid
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(465, 413);
        this.Controls.Add(this.propertyGrid1);
        this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
        this.Name = "PropertyGrid";
        this.Text = "Property Grid Test";
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.PropertyGrid propertyGrid1;
}
