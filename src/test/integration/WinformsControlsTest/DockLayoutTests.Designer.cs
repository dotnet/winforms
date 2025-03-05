// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace WinFormsControlsTest;

partial class DockLayoutTests
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.button1 = new System.Windows.Forms.Button();
        this.button3 = new System.Windows.Forms.Button();
        this.button2 = new System.Windows.Forms.Button();
        this.button4 = new System.Windows.Forms.Button();
        this.button5 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // button1
        // 
        this.button1.Dock = System.Windows.Forms.DockStyle.Left;
        this.button1.Location = new System.Drawing.Point(0, 0);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(112, 450);
        this.button1.TabIndex = 0;
        this.button1.Text = "Left";
        this.button1.UseVisualStyleBackColor = true;
        // 
        // button3
        // 
        this.button3.Dock = System.Windows.Forms.DockStyle.Right;
        this.button3.Location = new System.Drawing.Point(688, 0);
        this.button3.Name = "button3";
        this.button3.Size = new System.Drawing.Size(112, 450);
        this.button3.TabIndex = 2;
        this.button3.Text = "Right";
        this.button3.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        this.button2.Dock = System.Windows.Forms.DockStyle.Top;
        this.button2.Location = new System.Drawing.Point(112, 0);
        this.button2.Name = "button2";
        this.button2.Size = new System.Drawing.Size(576, 89);
        this.button2.TabIndex = 3;
        this.button2.Text = "Top";
        this.button2.UseVisualStyleBackColor = true;
        // 
        // button4
        // 
        this.button4.Dock = System.Windows.Forms.DockStyle.Bottom;
        this.button4.Location = new System.Drawing.Point(112, 384);
        this.button4.Name = "button4";
        this.button4.Size = new System.Drawing.Size(576, 66);
        this.button4.TabIndex = 4;
        this.button4.Text = "Bottom";
        this.button4.UseVisualStyleBackColor = true;
        // 
        // button5
        // 
        this.button5.Dock = System.Windows.Forms.DockStyle.Fill;
        this.button5.Location = new System.Drawing.Point(112, 89);
        this.button5.Name = "button5";
        this.button5.Size = new System.Drawing.Size(576, 295);
        this.button5.TabIndex = 5;
        this.button5.Text = "Fill";
        this.button5.UseVisualStyleBackColor = true;
        // 
        // DockLayoutTests
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.button5);
        this.Controls.Add(this.button4);
        this.Controls.Add(this.button2);
        this.Controls.Add(this.button3);
        this.Controls.Add(this.button1);
        this.Name = "DockLayoutTests";
        this.Text = "DockLayoutTests";
        this.ResumeLayout(false);

    }

    #endregion

    private Button button1;
    private Button button3;
    private Button button2;
    private Button button4;
    private Button button5;
}
