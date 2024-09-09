// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace WinFormsControlsTest;

partial class ScrollBars
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
        this.hScrollBar = new System.Windows.Forms.HScrollBar();
        this.vScrollBar = new System.Windows.Forms.VScrollBar();
        this.numericMinimum = new System.Windows.Forms.NumericUpDown();
        this.numericMaximum = new System.Windows.Forms.NumericUpDown();
        this.chbRightToLeft = new System.Windows.Forms.CheckBox();
        this.lblMinimum = new System.Windows.Forms.Label();
        this.lblMaximum = new System.Windows.Forms.Label();
        this.lblHValue = new System.Windows.Forms.Label();
        this.lblVValue = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.numericMinimum)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericMaximum)).BeginInit();
        this.SuspendLayout();
        // 
        // hScrollBar
        // 
        this.hScrollBar.Location = new System.Drawing.Point(37, 63);
        this.hScrollBar.Name = "hScrollBar";
        this.hScrollBar.Size = new System.Drawing.Size(200, 26);
        this.hScrollBar.TabIndex = 0;
        this.hScrollBar.Scroll += this.hScrollBar_Scroll;
        // 
        // vScrollBar
        // 
        this.vScrollBar.Location = new System.Drawing.Point(275, 63);
        this.vScrollBar.Name = "vScrollBar";
        this.vScrollBar.Size = new System.Drawing.Size(26, 200);
        this.vScrollBar.TabIndex = 1;
        this.vScrollBar.Scroll += this.vScrollBar_Scroll;
        // 
        // numericMinimum
        // 
        this.numericMinimum.Location = new System.Drawing.Point(37, 132);
        this.numericMinimum.Name = "numericMinimum";
        this.numericMinimum.Size = new System.Drawing.Size(150, 27);
        this.numericMinimum.TabIndex = 2;
        this.numericMinimum.ValueChanged += this.numericMinimum_ValueChanged;
        // 
        // numericMaximum
        // 
        this.numericMaximum.Location = new System.Drawing.Point(37, 194);
        this.numericMaximum.Name = "numericMaximum";
        this.numericMaximum.Size = new System.Drawing.Size(150, 27);
        this.numericMaximum.TabIndex = 3;
        this.numericMaximum.Value = new decimal(new int[] {
        100,
        0,
        0,
        0});
        this.numericMaximum.ValueChanged += this.numericMaximum_ValueChanged;
        // 
        // chbRightToLeft
        // 
        this.chbRightToLeft.AutoSize = true;
        this.chbRightToLeft.Location = new System.Drawing.Point(37, 250);
        this.chbRightToLeft.Name = "chbRightToLeft";
        this.chbRightToLeft.Size = new System.Drawing.Size(110, 24);
        this.chbRightToLeft.TabIndex = 4;
        this.chbRightToLeft.Text = "Right to left";
        this.chbRightToLeft.UseVisualStyleBackColor = true;
        this.chbRightToLeft.CheckedChanged += this.chbRightToLeft_CheckedChanged;
        // 
        // lblMinimum
        // 
        this.lblMinimum.AutoSize = true;
        this.lblMinimum.Location = new System.Drawing.Point(37, 109);
        this.lblMinimum.Name = "lblMinimum";
        this.lblMinimum.Size = new System.Drawing.Size(72, 20);
        this.lblMinimum.TabIndex = 5;
        this.lblMinimum.Text = "Minimum";
        // 
        // lblMaximum
        // 
        this.lblMaximum.AutoSize = true;
        this.lblMaximum.Location = new System.Drawing.Point(37, 171);
        this.lblMaximum.Name = "lblMaximum";
        this.lblMaximum.Size = new System.Drawing.Size(75, 20);
        this.lblMaximum.TabIndex = 6;
        this.lblMaximum.Text = "Maximum";
        // 
        // lblHValue
        // 
        this.lblHValue.AutoSize = true;
        this.lblHValue.Location = new System.Drawing.Point(37, 33);
        this.lblHValue.Name = "lblHValue";
        this.lblHValue.Size = new System.Drawing.Size(50, 20);
        this.lblHValue.TabIndex = 7;
        this.lblHValue.Text = "label3";
        // 
        // lblVValue
        // 
        this.lblVValue.AutoSize = true;
        this.lblVValue.Location = new System.Drawing.Point(275, 33);
        this.lblVValue.Name = "lblVValue";
        this.lblVValue.Size = new System.Drawing.Size(50, 20);
        this.lblVValue.TabIndex = 8;
        this.lblVValue.Text = "label4";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.lblVValue);
        this.Controls.Add(this.lblHValue);
        this.Controls.Add(this.lblMaximum);
        this.Controls.Add(this.lblMinimum);
        this.Controls.Add(this.chbRightToLeft);
        this.Controls.Add(this.numericMaximum);
        this.Controls.Add(this.numericMinimum);
        this.Controls.Add(this.vScrollBar);
        this.Controls.Add(this.hScrollBar);
        this.Name = "Form1";
        this.Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)(this.numericMinimum)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericMaximum)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.HScrollBar hScrollBar;
    private System.Windows.Forms.VScrollBar vScrollBar;
    private System.Windows.Forms.NumericUpDown numericMinimum;
    private System.Windows.Forms.NumericUpDown numericMaximum;
    private System.Windows.Forms.CheckBox chbRightToLeft;
    private System.Windows.Forms.Label lblMinimum;
    private System.Windows.Forms.Label lblMaximum;
    private System.Windows.Forms.Label lblHValue;
    private System.Windows.Forms.Label lblVValue;
}
