// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class AnchorLayoutTests
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
        this.button1 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.button3 = new System.Windows.Forms.Button();
        this.button4 = new System.Windows.Forms.Button();
        this.button5 = new System.Windows.Forms.Button();
        this.button6 = new System.Windows.Forms.Button();
        this.button7 = new System.Windows.Forms.Button();
        this.button8 = new System.Windows.Forms.Button();
        this.button9 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(12, 12);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(112, 34);
        this.button1.TabIndex = 0;
        this.button1.Text = "Top-Left";
        this.button1.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        this.button2.Anchor = System.Windows.Forms.AnchorStyles.Top;
        this.button2.Location = new System.Drawing.Point(338, 12);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(112, 34);
        this.button2.TabIndex = 1;
        this.button2.Text = "Top";
        this.button2.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        this.button3.Location = new System.Drawing.Point(676, 12);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(112, 34);
        this.button3.TabIndex = 2;
        this.button3.Text = "Top-Right";
        this.button3.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        this.button4.Anchor = System.Windows.Forms.AnchorStyles.Left;
        this.button4.Location = new System.Drawing.Point(12, 198);
        this.button4.Name = "button4";
        this.button4.Size = new System.Drawing.Size(112, 34);
        this.button4.TabIndex = 3;
        this.button4.Text = "Left";
        this.button4.UseVisualStyleBackColor = true;
        // 
        // button5
        // 
        this.button5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.button5.Location = new System.Drawing.Point(12, 404);
        this.button5.Name = "button5";
        this.button5.Size = new System.Drawing.Size(112, 34);
        this.button5.TabIndex = 4;
        this.button5.Text = "Bottom-Left";
        this.button5.UseVisualStyleBackColor = true;
        // 
        // button6
        // 
        this.button6.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
        | System.Windows.Forms.AnchorStyles.Left) 
        | System.Windows.Forms.AnchorStyles.Right)));
        this.button6.Location = new System.Drawing.Point(338, 198);
        this.button6.Name = "button6";
        this.button6.Size = new System.Drawing.Size(112, 34);
        this.button6.TabIndex = 5;
        this.button6.Text = "All";
        this.button6.UseVisualStyleBackColor = true;
        // 
        // button7
        // 
        this.button7.Anchor = System.Windows.Forms.AnchorStyles.Right;
        this.button7.Location = new System.Drawing.Point(676, 198);
        this.button7.Name = "button7";
        this.button7.Size = new System.Drawing.Size(112, 34);
        this.button7.TabIndex = 6;
        this.button7.Text = "Right";
        this.button7.UseVisualStyleBackColor = true;
        // 
        // button8
        // 
        this.button8.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
        this.button8.Location = new System.Drawing.Point(338, 404);
        this.button8.Name = "button8";
        this.button8.Size = new System.Drawing.Size(112, 34);
        this.button8.TabIndex = 7;
        this.button8.Text = "Bottom";
        this.button8.UseVisualStyleBackColor = true;
        // 
        // button9
        // 
        this.button9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
        this.button9.Location = new System.Drawing.Point(676, 404);
        this.button9.Name = "button9";
        this.button9.Size = new System.Drawing.Size(112, 34);
        this.button9.TabIndex = 8;
        this.button9.Text = "Bottom-Right";
        this.button9.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.button9);
        this.Controls.Add(this.button8);
        this.Controls.Add(this.button7);
        this.Controls.Add(this.button6);
        this.Controls.Add(this.button5);
        this.Controls.Add(this.button4);
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button1);
        this.Name = "Form1";
        this.Text = "Form1";
        this.ResumeLayout(false);

    }

    #endregion

    private Button button1;
    private Button button2;
    private Button button3;
    private Button button4;
    private Button button5;
    private Button button6;
    private Button button7;
    private Button button8;
    private Button button9;
}
