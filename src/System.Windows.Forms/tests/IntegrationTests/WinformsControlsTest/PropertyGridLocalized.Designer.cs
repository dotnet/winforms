// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinformsControlsTest;

partial class PropertyGridLocalized
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        propertyGrid1 = new System.Windows.Forms.PropertyGrid();
        cultureComboBox = new ComboBox();
        tableLayoutPanel1 = new TableLayoutPanel();
        tableLayoutPanel1.SuspendLayout();
        SuspendLayout();
        //
        // propertyGrid1
        //
        propertyGrid1.CommandsBorderColor = System.Drawing.SystemColors.Control;
        propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
        propertyGrid1.Location = new System.Drawing.Point(3, 32);
        propertyGrid1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
        propertyGrid1.Name = "propertyGrid1";
        propertyGrid1.Size = new System.Drawing.Size(460, 400);
        propertyGrid1.TabIndex = 0;
        // 
        // comboBox1
        // 
        cultureComboBox.Dock = DockStyle.Top;
        cultureComboBox.FormattingEnabled = true;
        cultureComboBox.Location = new Point(3, 3);
        cultureComboBox.Name = "cultureComboBox";
        cultureComboBox.Size = new Size(460, 23);
        cultureComboBox.TabIndex = 1;
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 1;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle());
        tableLayoutPanel1.Controls.Add(cultureComboBox, 0, 0);
        tableLayoutPanel1.Controls.Add(propertyGrid1, 0, 1);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 2;
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.RowStyles.Add(new RowStyle());
        tableLayoutPanel1.Size = new Size(460, 413);
        tableLayoutPanel1.TabIndex = 2;
        //
        // PropertyGrid
        //
        AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(465, 413);
        Controls.Add(tableLayoutPanel1);
        Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
        Name = "PropertyGridLocalized";
        Text = "Localized Property Grid Test";
        tableLayoutPanel1.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.PropertyGrid propertyGrid1;
    private System.Windows.Forms.ComboBox cultureComboBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
}
