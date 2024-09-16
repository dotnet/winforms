// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

partial class Password
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
        textBox1 = new TextBox();
        textBox2 = new TextBox();
        label1 = new Label();
        label2 = new Label();
        maskedTextBox1 = new MaskedTextBox();
        maskedTextBox2 = new MaskedTextBox();
        maskedTextBox3 = new MaskedTextBox();
        textBox3 = new TextBox();
        button1 = new Button();
        textBox4 = new TextBox();
        textBox5 = new TextBox();
        label3 = new Label();
        label4 = new Label();
        label5 = new Label();
        label6 = new Label();
        label7 = new Label();
        label8 = new Label();
        label9 = new Label();
        label10 = new Label();
        label11 = new Label();
        SuspendLayout();
        // 
        // textBox1
        // 
        textBox1.Location = new Point(26, 113);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(125, 27);
        textBox1.TabIndex = 2;
        textBox1.UseSystemPasswordChar = true;
        // 
        // textBox2
        // 
        textBox2.Location = new Point(26, 191);
        textBox2.Name = "textBox2";
        textBox2.PasswordChar = '*';
        textBox2.Size = new Size(125, 27);
        textBox2.TabIndex = 4;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(26, 84);
        label1.Name = "label1";
        label1.Size = new Size(215, 20);
        label1.TabIndex = 1;
        label1.Text = "UseSystemPasswordChar = true";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(25, 163);
        label2.Name = "label2";
        label2.Size = new Size(130, 20);
        label2.TabIndex = 3;
        label2.Text = "PasswordChar = '*'";
        // 
        // maskedTextBox1
        // 
        maskedTextBox1.Location = new Point(316, 191);
        maskedTextBox1.Name = "maskedTextBox1";
        maskedTextBox1.PasswordChar = '#';
        maskedTextBox1.Size = new Size(125, 27);
        maskedTextBox1.TabIndex = 11;
        // 
        // maskedTextBox2
        // 
        maskedTextBox2.Location = new Point(316, 113);
        maskedTextBox2.Name = "maskedTextBox2";
        maskedTextBox2.Size = new Size(125, 27);
        maskedTextBox2.TabIndex = 9;
        maskedTextBox2.UseSystemPasswordChar = true;
        // 
        // maskedTextBox3
        // 
        maskedTextBox3.Location = new Point(316, 279);
        maskedTextBox3.Mask = "0000000";
        maskedTextBox3.Name = "maskedTextBox3";
        maskedTextBox3.PasswordChar = '*';
        maskedTextBox3.Size = new Size(125, 27);
        maskedTextBox3.TabIndex = 13;
        maskedTextBox3.ValidatingType = typeof(int);
        // 
        // textBox3
        // 
        textBox3.Location = new Point(26, 279);
        textBox3.Name = "textBox3";
        textBox3.Size = new Size(125, 27);
        textBox3.TabIndex = 6;
        textBox3.UseSystemPasswordChar = true;
        // 
        // button1
        // 
        button1.Location = new Point(26, 338);
        button1.Name = "button1";
        button1.Size = new Size(94, 29);
        button1.TabIndex = 19;
        button1.Text = "Show/hide";
        button1.UseVisualStyleBackColor = true;
        button1.Click += button1_Click;
        // 
        // textBox4
        // 
        textBox4.Location = new Point(623, 251);
        textBox4.Multiline = true;
        textBox4.Name = "textBox4";
        textBox4.PasswordChar = '*';
        textBox4.Size = new Size(125, 76);
        textBox4.TabIndex = 18;
        // 
        // textBox5
        // 
        textBox5.Location = new Point(623, 113);
        textBox5.Multiline = true;
        textBox5.Name = "textBox5";
        textBox5.Size = new Size(125, 76);
        textBox5.TabIndex = 16;
        textBox5.UseSystemPasswordChar = true;
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(26, 27);
        label3.Name = "label3";
        label3.Size = new Size(75, 20);
        label3.TabIndex = 0;
        label3.Text = "TextBoxes";
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(310, 27);
        label4.Name = "label4";
        label4.Size = new Size(126, 20);
        label4.TabIndex = 7;
        label4.Text = "MaskedTextBoxes";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(611, 27);
        label5.Name = "label5";
        label5.Size = new Size(137, 20);
        label5.TabIndex = 14;
        label5.Text = "Multiline TextBoxes";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(611, 213);
        label6.Name = "label6";
        label6.Size = new Size(130, 20);
        label6.TabIndex = 17;
        label6.Text = "PasswordChar = '*'";
        // 
        // label7
        // 
        label7.AutoSize = true;
        label7.Location = new Point(611, 84);
        label7.Name = "label7";
        label7.Size = new Size(215, 20);
        label7.TabIndex = 15;
        label7.Text = "UseSystemPasswordChar = true";
        // 
        // label8
        // 
        label8.AutoSize = true;
        label8.Location = new Point(310, 163);
        label8.Name = "label8";
        label8.Size = new Size(130, 20);
        label8.TabIndex = 10;
        label8.Text = "PasswordChar = '#'";
        // 
        // label9
        // 
        label9.AutoSize = true;
        label9.Location = new Point(310, 84);
        label9.Name = "label9";
        label9.Size = new Size(215, 20);
        label9.TabIndex = 8;
        label9.Text = "UseSystemPasswordChar = true";
        // 
        // label10
        // 
        label10.AutoSize = true;
        label10.Location = new Point(26, 251);
        label10.Name = "label10";
        label10.Size = new Size(148, 20);
        label10.TabIndex = 5;
        label10.Text = "Hide/Show Password";
        // 
        // label11
        // 
        label11.AutoSize = true;
        label11.Location = new Point(310, 251);
        label11.Name = "label11";
        label11.Size = new Size(234, 20);
        label11.TabIndex = 12;
        label11.Text = "PasswordChar ='*' with input Mask";
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(8F, 20F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(898, 451);
        Controls.Add(label11);
        Controls.Add(label10);
        Controls.Add(label9);
        Controls.Add(label8);
        Controls.Add(label7);
        Controls.Add(label6);
        Controls.Add(label5);
        Controls.Add(label4);
        Controls.Add(label3);
        Controls.Add(textBox5);
        Controls.Add(textBox4);
        Controls.Add(button1);
        Controls.Add(textBox3);
        Controls.Add(maskedTextBox3);
        Controls.Add(maskedTextBox2);
        Controls.Add(maskedTextBox1);
        Controls.Add(label2);
        Controls.Add(label1);
        Controls.Add(textBox2);
        Controls.Add(textBox1);
        Name = "Form1";
        Text = "Form1";
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private TextBox textBox1;
    private TextBox textBox2;
    private Label label1;
    private Label label2;
    private MaskedTextBox maskedTextBox1;
    private MaskedTextBox maskedTextBox2;
    private MaskedTextBox maskedTextBox3;
    private TextBox textBox3;
    private Button button1;
    private TextBox textBox4;
    private TextBox textBox5;
    private Label label3;
    private Label label4;
    private Label label5;
    private Label label6;
    private Label label7;
    private Label label8;
    private Label label9;
    private Label label10;
    private Label label11;
}
