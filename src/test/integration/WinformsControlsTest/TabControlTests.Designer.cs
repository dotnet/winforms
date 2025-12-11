// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

partial class TabControlTests
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
        if (disposing && (components is not null))
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
        this.tabControl1 = new TabControl();
        this.tabPage1 = new TabPage();
        this.tabPage2 = new TabPage();
        this.tabPage3 = new TabPage();
        this.tabControl2 = new TabControl();
        this.tabPage4 = new TabPage();
        this.tabPage5 = new TabPage();
        this.tabPage6 = new TabPage();
        this.tabControl3 = new TabControl();
        this.tabPage7 = new TabPage();
        this.tabPage8 = new TabPage();
        this.tabPage9 = new TabPage();
        this.tabControl4 = new TabControl();
        this.tabPage10 = new TabPage();
        this.tabPage11 = new TabPage();
        this.tabPage12 = new TabPage();
        this.label1 = new Label();
        this.label2 = new Label();
        this.label3 = new Label();
        this.label4 = new Label();
        
        this.tabControl1.SuspendLayout();
        this.tabControl2.SuspendLayout();
        this.tabControl3.SuspendLayout();
        this.tabControl4.SuspendLayout();
        this.SuspendLayout();
        
        // 
        // tabControl1 (Top alignment)
        // 
        this.tabControl1.Controls.Add(this.tabPage1);
        this.tabControl1.Controls.Add(this.tabPage2);
        this.tabControl1.Controls.Add(this.tabPage3);
        this.tabControl1.Location = new Point(12, 35);
        this.tabControl1.Name = "tabControl1";
        this.tabControl1.SelectedIndex = 0;
        this.tabControl1.Size = new Size(300, 250);
        this.tabControl1.TabIndex = 0;
        this.tabControl1.Alignment = TabAlignment.Top;
        
        // 
        // tabPage1
        // 
        this.tabPage1.Location = new Point(4, 24);
        this.tabPage1.Name = "tabPage1";
        this.tabPage1.Padding = new Padding(3);
        this.tabPage1.Size = new Size(292, 222);
        this.tabPage1.TabIndex = 0;
        this.tabPage1.Text = "tabPage1";
        this.tabPage1.UseVisualStyleBackColor = true;
        
        // 
        // tabPage2
        // 
        this.tabPage2.Location = new Point(4, 24);
        this.tabPage2.Name = "tabPage2";
        this.tabPage2.Padding = new Padding(3);
        this.tabPage2.Size = new Size(292, 222);
        this.tabPage2.TabIndex = 1;
        this.tabPage2.Text = "tabPage2";
        this.tabPage2.UseVisualStyleBackColor = true;
        
        // 
        // tabPage3
        // 
        this.tabPage3.Location = new Point(4, 24);
        this.tabPage3.Name = "tabPage3";
        this.tabPage3.Size = new Size(292, 222);
        this.tabPage3.TabIndex = 2;
        this.tabPage3.Text = "tabPage3";
        this.tabPage3.UseVisualStyleBackColor = true;
        
        // 
        // tabControl2 (Bottom alignment)
        // 
        this.tabControl2.Controls.Add(this.tabPage4);
        this.tabControl2.Controls.Add(this.tabPage5);
        this.tabControl2.Controls.Add(this.tabPage6);
        this.tabControl2.Location = new Point(330, 35);
        this.tabControl2.Name = "tabControl2";
        this.tabControl2.SelectedIndex = 0;
        this.tabControl2.Size = new Size(300, 250);
        this.tabControl2.TabIndex = 1;
        this.tabControl2.Alignment = TabAlignment.Bottom;
        
        // 
        // tabPage4
        // 
        this.tabPage4.Location = new Point(4, 4);
        this.tabPage4.Name = "tabPage4";
        this.tabPage4.Padding = new Padding(3);
        this.tabPage4.Size = new Size(292, 222);
        this.tabPage4.TabIndex = 0;
        this.tabPage4.Text = "tabPage4";
        this.tabPage4.UseVisualStyleBackColor = true;
        
        // 
        // tabPage5
        // 
        this.tabPage5.Location = new Point(4, 4);
        this.tabPage5.Name = "tabPage5";
        this.tabPage5.Padding = new Padding(3);
        this.tabPage5.Size = new Size(292, 222);
        this.tabPage5.TabIndex = 1;
        this.tabPage5.Text = "tabPage5";
        this.tabPage5.UseVisualStyleBackColor = true;
        
        // 
        // tabPage6
        // 
        this.tabPage6.Location = new Point(4, 4);
        this.tabPage6.Name = "tabPage6";
        this.tabPage6.Size = new Size(292, 222);
        this.tabPage6.TabIndex = 2;
        this.tabPage6.Text = "tabPage6";
        this.tabPage6.UseVisualStyleBackColor = true;
        
        // 
        // tabControl3 (Left alignment)
        // 
        this.tabControl3.Controls.Add(this.tabPage7);
        this.tabControl3.Controls.Add(this.tabPage8);
        this.tabControl3.Controls.Add(this.tabPage9);
        this.tabControl3.Location = new Point(12, 310);
        this.tabControl3.Name = "tabControl3";
        this.tabControl3.SelectedIndex = 0;
        this.tabControl3.Size = new Size(300, 250);
        this.tabControl3.TabIndex = 2;
        this.tabControl3.Alignment = TabAlignment.Left;
        
        // 
        // tabPage7
        // 
        this.tabPage7.Location = new Point(23, 4);
        this.tabPage7.Name = "tabPage7";
        this.tabPage7.Padding = new Padding(3);
        this.tabPage7.Size = new Size(273, 242);
        this.tabPage7.TabIndex = 0;
        this.tabPage7.Text = "tabPage7";
        this.tabPage7.UseVisualStyleBackColor = true;
        
        // 
        // tabPage8
        // 
        this.tabPage8.Location = new Point(23, 4);
        this.tabPage8.Name = "tabPage8";
        this.tabPage8.Padding = new Padding(3);
        this.tabPage8.Size = new Size(273, 242);
        this.tabPage8.TabIndex = 1;
        this.tabPage8.Text = "tabPage8";
        this.tabPage8.UseVisualStyleBackColor = true;
        
        // 
        // tabPage9
        // 
        this.tabPage9.Location = new Point(23, 4);
        this.tabPage9.Name = "tabPage9";
        this.tabPage9.Size = new Size(273, 242);
        this.tabPage9.TabIndex = 2;
        this.tabPage9.Text = "tabPage9";
        this.tabPage9.UseVisualStyleBackColor = true;
        
        // 
        // tabControl4 (Right alignment)
        // 
        this.tabControl4.Controls.Add(this.tabPage10);
        this.tabControl4.Controls.Add(this.tabPage11);
        this.tabControl4.Controls.Add(this.tabPage12);
        this.tabControl4.Location = new Point(330, 310);
        this.tabControl4.Name = "tabControl4";
        this.tabControl4.SelectedIndex = 0;
        this.tabControl4.Size = new Size(300, 250);
        this.tabControl4.TabIndex = 3;
        this.tabControl4.Alignment = TabAlignment.Right;
        
        // 
        // tabPage10
        // 
        this.tabPage10.Location = new Point(4, 4);
        this.tabPage10.Name = "tabPage10";
        this.tabPage10.Padding = new Padding(3);
        this.tabPage10.Size = new Size(273, 242);
        this.tabPage10.TabIndex = 0;
        this.tabPage10.Text = "tabPage10";
        this.tabPage10.UseVisualStyleBackColor = true;
        
        // 
        // tabPage11
        // 
        this.tabPage11.Location = new Point(4, 4);
        this.tabPage11.Name = "tabPage11";
        this.tabPage11.Padding = new Padding(3);
        this.tabPage11.Size = new Size(273, 242);
        this.tabPage11.TabIndex = 1;
        this.tabPage11.Text = "tabPage11";
        this.tabPage11.UseVisualStyleBackColor = true;
        
        // 
        // tabPage12
        // 
        this.tabPage12.Location = new Point(4, 4);
        this.tabPage12.Name = "tabPage12";
        this.tabPage12.Size = new Size(273, 242);
        this.tabPage12.TabIndex = 2;
        this.tabPage12.Text = "tabPage12";
        this.tabPage12.UseVisualStyleBackColor = true;
        
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new Point(12, 12);
        this.label1.Name = "label1";
        this.label1.Size = new Size(150, 15);
        this.label1.TabIndex = 4;
        this.label1.Text = "Alignment = Top";
        
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new Point(330, 12);
        this.label2.Name = "label2";
        this.label2.Size = new Size(150, 15);
        this.label2.TabIndex = 5;
        this.label2.Text = "Alignment = Bottom";
        
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new Point(12, 292);
        this.label3.Name = "label3";
        this.label3.Size = new Size(150, 15);
        this.label3.TabIndex = 6;
        this.label3.Text = "Alignment = Left";
        
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new Point(330, 292);
        this.label4.Name = "label4";
        this.label4.Size = new Size(150, 15);
        this.label4.TabIndex = 7;
        this.label4.Text = "Alignment = Right";
        
        // 
        // TabControlTests
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(642, 572);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.tabControl4);
        this.Controls.Add(this.tabControl3);
        this.Controls.Add(this.tabControl2);
        this.Controls.Add(this.tabControl1);
        this.Name = "TabControlTests";
        this.Text = "TabControl Dark Mode Tests";
        
        this.tabControl1.ResumeLayout(false);
        this.tabControl2.ResumeLayout(false);
        this.tabControl3.ResumeLayout(false);
        this.tabControl4.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private TabControl tabControl1;
    private TabPage tabPage1;
    private TabPage tabPage2;
    private TabPage tabPage3;
    private TabControl tabControl2;
    private TabPage tabPage4;
    private TabPage tabPage5;
    private TabPage tabPage6;
    private TabControl tabControl3;
    private TabPage tabPage7;
    private TabPage tabPage8;
    private TabPage tabPage9;
    private TabControl tabControl4;
    private TabPage tabPage10;
    private TabPage tabPage11;
    private TabPage tabPage12;
    private Label label1;
    private Label label2;
    private Label label3;
    private Label label4;
}
