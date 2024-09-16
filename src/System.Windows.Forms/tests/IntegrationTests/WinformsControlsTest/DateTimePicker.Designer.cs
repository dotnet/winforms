// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

partial class DateTimePicker
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
        this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
        this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
        this.dateTimePicker3 = new System.Windows.Forms.DateTimePicker();
        this.dateTimePicker4 = new System.Windows.Forms.DateTimePicker();
        this.dateTimePicker5 = new System.Windows.Forms.DateTimePicker();
        this.SuspendLayout();
        // 
        // dateTimePicker1
        // 
        this.dateTimePicker1.Location = new Point(31, 32);
        this.dateTimePicker1.Margin = new Padding(4, 3, 4, 3);
        this.dateTimePicker1.Name = "dateTimePicker1";
        this.dateTimePicker1.Size = new Size(233, 23);
        this.dateTimePicker1.TabIndex = 1;
        this.dateTimePicker1.ShowCheckBox = true;
        // 
        // dateTimePicker2
        // 
        this.dateTimePicker2.Format = DateTimePickerFormat.Long;
        this.dateTimePicker2.Location = new Point(31, 78);
        this.dateTimePicker2.Margin = new Padding(4, 3, 4, 3);
        this.dateTimePicker2.Name = "dateTimePicker2";
        this.dateTimePicker2.Size = new Size(233, 23);
        this.dateTimePicker2.TabIndex = 2;
        this.dateTimePicker2.ShowCheckBox = true;
        this.dateTimePicker2.ShowUpDown = true;
        // 
        // dateTimePicker3
        // 
        this.dateTimePicker3.Format = DateTimePickerFormat.Short;
        this.dateTimePicker3.Location = new Point(31, 128);
        this.dateTimePicker3.Margin = new Padding(4, 3, 4, 3);
        this.dateTimePicker3.Name = "dateTimePicker3";
        this.dateTimePicker3.Size = new Size(233, 23);
        this.dateTimePicker3.TabIndex = 3;
        this.dateTimePicker3.ShowCheckBox = true;
        // 
        // dateTimePicker4
        // 
        this.dateTimePicker4.Format = DateTimePickerFormat.Time;
        this.dateTimePicker4.Location = new Point(31, 175);
        this.dateTimePicker4.Margin = new Padding(4, 3, 4, 3);
        this.dateTimePicker4.Name = "dateTimePicker4";
        this.dateTimePicker4.Size = new Size(233, 23);
        this.dateTimePicker4.TabIndex = 4;
        this.dateTimePicker4.ShowCheckBox = true;
        // 
        // dateTimePicker5
        // 
        this.dateTimePicker5.Format = DateTimePickerFormat.Custom;
        this.dateTimePicker5.Location = new Point(31, 221);
        this.dateTimePicker5.Margin = new Padding(4, 3, 4, 3);
        this.dateTimePicker5.Name = "dateTimePicker5";
        this.dateTimePicker5.Size = new Size(233, 23);
        this.dateTimePicker5.TabIndex = 5;
        this.dateTimePicker5.ShowCheckBox = true;
        // 
        // DateTimePicker
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(331, 301);
        this.Controls.Add(dateTimePicker5);
        this.Controls.Add(dateTimePicker4);
        this.Controls.Add(dateTimePicker3);
        this.Controls.Add(dateTimePicker2);
        this.Controls.Add(dateTimePicker1);
        this.Margin = new Padding(4, 3, 4, 3);
        this.Name = "DateTimePicker";
        this.Text = "DateTimePicker";
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.DateTimePicker dateTimePicker1;
    private System.Windows.Forms.DateTimePicker dateTimePicker2;
    private System.Windows.Forms.DateTimePicker dateTimePicker3;
    private System.Windows.Forms.DateTimePicker dateTimePicker4;
    private System.Windows.Forms.DateTimePicker dateTimePicker5;
}
