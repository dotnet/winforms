// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

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
        this.label1_2 = new System.Windows.Forms.Label();
        this.label1 = new System.Windows.Forms.Label();
        this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);

        this.textBox2 = new System.Windows.Forms.TextBox();
        this.label2_1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.errorProvider2 = new System.Windows.Forms.ErrorProvider(this.components);
        this.errorProvider2.Icon = SystemIcons.Shield;

        this.submitButton = new System.Windows.Forms.Button();

        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).BeginInit();

        this.SuspendLayout();
        //
        // label1
        //
        this.label1.AutoSize = true;
        this.label1.Font = new System.Drawing.Font("Calibri", 12F, FontStyle.Bold);
        this.label1.Location = new System.Drawing.Point(19, 26);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(155, 15);
        this.label1.TabIndex = 2;
        this.label1.Text = "ErrorProvider with standard icon";
        // 
        // label1_2
        // 
        this.label1_2.AutoSize = true;
        this.label1_2.Location = new System.Drawing.Point(19, 41);
        this.label1_2.Name = "label1_2";
        this.label1_2.Size = new System.Drawing.Size(155, 15);
        this.label1_2.TabIndex = 2;
        this.label1_2.Text = "Type from 5 to 10 characters";
        // 
        // textBox1
        // 
        this.textBox1.Location = new System.Drawing.Point(19, 60);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(120, 23);
        this.textBox1.TabIndex = 0;
        // 
        // errorProvider1
        // 
        this.errorProvider1.ContainerControl = this;
        //
        // label2
        //
        this.label2.AutoSize = true;
        this.label2.Font = new System.Drawing.Font("Calibri", 12F, FontStyle.Bold);
        this.label2.Location = new System.Drawing.Point(19, 100);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(155, 15);
        this.label2.TabIndex = 2;
        this.label2.Text = "ErrorProvider with multi-resolution icon";
        // 
        // label2_1
        // 
        this.label2_1.AutoSize = true;
        this.label2_1.Location = new System.Drawing.Point(19, 115);
        this.label2_1.Name = "label2_1";
        this.label2_1.Size = new System.Drawing.Size(155, 23);
        this.label2_1.TabIndex = 2;
        this.label2_1.Text = "Type from 5 to 20 characters";
        // 
        // textBox2
        // 
        this.textBox2.Location = new System.Drawing.Point(19, 135);
        this.textBox2.Name = "textBox2";
        this.textBox2.Size = new System.Drawing.Size(120, 23);
        this.textBox2.TabIndex = 0;
        // 
        // errorProvider2
        // 
        this.errorProvider2.ContainerControl = this;
        // 
        // submitButton
        // 
        this.submitButton.Location = new System.Drawing.Point(19, 180);
        this.submitButton.Name = "submitButton";
        this.submitButton.Size = new System.Drawing.Size(120, 27);
        this.submitButton.TabIndex = 1;
        this.submitButton.Text = "Validate";
        this.submitButton.UseVisualStyleBackColor = true;
        this.submitButton.Click += this.submitButton_Click;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(290, 230);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.label1_2);
        this.Controls.Add(this.textBox1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label2_1);
        this.Controls.Add(this.textBox2);
        this.Controls.Add(this.submitButton);
        this.Name = "Form1";
        this.Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.errorProvider2)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.ErrorProvider errorProvider1;
    private System.Windows.Forms.ErrorProvider errorProvider2;
    private System.Windows.Forms.Button submitButton;
    private System.Windows.Forms.Label label1_2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2_1;
    private System.Windows.Forms.Label label2;
}
