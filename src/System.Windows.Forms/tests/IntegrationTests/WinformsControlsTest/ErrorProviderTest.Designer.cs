// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinformsControlsTest;

partial class ErrorProviderTest
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
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
        this.submitButton = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        this.SuspendLayout();
        // 
        // textBox1
        // 
        this.textBox1.Location = new System.Drawing.Point(35, 60);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(120, 23);
        this.textBox1.TabIndex = 0;
        // 
        // errorProvider1
        // 
        this.errorProvider1.ContainerControl = this;
        // 
        // submitButton
        // 
        this.submitButton.Location = new System.Drawing.Point(35, 109);
        this.submitButton.Name = "submitButton";
        this.submitButton.Size = new System.Drawing.Size(120, 27);
        this.submitButton.TabIndex = 1;
        this.submitButton.Text = "Submit";
        this.submitButton.UseVisualStyleBackColor = true;
        this.submitButton.Click += new System.EventHandler(this.submitButton_Click);
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(19, 26);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(155, 15);
        this.label1.TabIndex = 2;
        this.label1.Text = "Type from 5 to 10 characters";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(194, 165);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.submitButton);
        this.Controls.Add(this.textBox1);
        this.Name = "Form1";
        this.Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.Button submitButton;
    private System.Windows.Forms.Label label1;
}
