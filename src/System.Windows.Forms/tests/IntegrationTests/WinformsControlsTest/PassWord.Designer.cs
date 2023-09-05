// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinformsControlsTest;

partial class PassWord
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
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.textBox2 = new System.Windows.Forms.TextBox();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
        this.maskedTextBox2 = new System.Windows.Forms.MaskedTextBox();
        this.maskedTextBox3 = new System.Windows.Forms.MaskedTextBox();
        this.textBox3 = new System.Windows.Forms.TextBox();
        this.button1 = new System.Windows.Forms.Button();
        this.textBox4 = new System.Windows.Forms.TextBox();
        this.textBox5 = new System.Windows.Forms.TextBox();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label5 = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.label8 = new System.Windows.Forms.Label();
        this.label9 = new System.Windows.Forms.Label();
        this.SuspendLayout();
        // 
        // textBox1
        // 
        this.textBox1.AccessibleName = "Test_Box1";
        this.textBox1.Location = new System.Drawing.Point(85, 97);
        this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(110, 23);
        this.textBox1.TabIndex = 0;
        this.textBox1.UseSystemPasswordChar = true;
        // 
        // textBox2
        // 
        this.textBox2.AccessibleName = "Text_Box2";
        this.textBox2.Location = new System.Drawing.Point(85, 176);
        this.textBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.textBox2.Name = "textBox2";
        this.textBox2.PasswordChar = '*';
        this.textBox2.Size = new System.Drawing.Size(110, 23);
        this.textBox2.TabIndex = 1;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(85, 70);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(174, 15);
        this.label1.TabIndex = 2;
        this.label1.Text = "UseSystemPasswordChar = true";
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(85, 151);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(107, 15);
        this.label2.TabIndex = 3;
        this.label2.Text = "PasswordChar = \'*\'";
        // 
        // maskedTextBox1
        // 
        this.maskedTextBox1.AccessibleName = "Mask_Text_Box2";
        this.maskedTextBox1.Location = new System.Drawing.Point(333, 176);
        this.maskedTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.maskedTextBox1.Name = "maskedTextBox1";
        this.maskedTextBox1.PasswordChar = '#';
        this.maskedTextBox1.Size = new System.Drawing.Size(110, 23);
        this.maskedTextBox1.TabIndex = 4;
        // 
        // maskedTextBox2
        // 
        this.maskedTextBox2.AccessibleName = "Mask_Text_Box1";
        this.maskedTextBox2.Location = new System.Drawing.Point(332, 97);
        this.maskedTextBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.maskedTextBox2.Name = "maskedTextBox2";
        this.maskedTextBox2.Size = new System.Drawing.Size(110, 23);
        this.maskedTextBox2.TabIndex = 5;
        this.maskedTextBox2.UseSystemPasswordChar = true;
        // 
        // maskedTextBox3
        // 
        this.maskedTextBox3.AccessibleName = "Mask_Text_Box3";
        this.maskedTextBox3.Location = new System.Drawing.Point(332, 225);
        this.maskedTextBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.maskedTextBox3.Mask = "0000000";
        this.maskedTextBox3.Name = "maskedTextBox3";
        this.maskedTextBox3.PasswordChar = '*';
        this.maskedTextBox3.Size = new System.Drawing.Size(110, 23);
        this.maskedTextBox3.TabIndex = 6;
        this.maskedTextBox3.ValidatingType = typeof(int);
        // 
        // textBox3
        // 
        this.textBox3.AccessibleName = "Text_Box3";
        this.textBox3.Location = new System.Drawing.Point(85, 225);
        this.textBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.textBox3.Name = "textBox3";
        this.textBox3.Size = new System.Drawing.Size(110, 23);
        this.textBox3.TabIndex = 7;
        this.textBox3.UseSystemPasswordChar = true;
        // 
        // button1
        // 
        this.button1.Location = new System.Drawing.Point(85, 250);
        this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(82, 22);
        this.button1.TabIndex = 8;
        this.button1.Text = "Show/hide";
        this.button1.UseVisualStyleBackColor = true;
        this.button1.Click += new System.EventHandler(this.button1_Click);
        // 
        // textBox4
        // 
        this.textBox4.AccessibleName = "Test_Box5";
        this.textBox4.Location = new System.Drawing.Point(560, 188);
        this.textBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.textBox4.Multiline = true;
        this.textBox4.Name = "textBox4";
        this.textBox4.PasswordChar = '*';
        this.textBox4.Size = new System.Drawing.Size(110, 58);
        this.textBox4.TabIndex = 9;
        // 
        // textBox5
        // 
        this.textBox5.AccessibleName = "Test_Box4";
        this.textBox5.Location = new System.Drawing.Point(560, 97);
        this.textBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.textBox5.Multiline = true;
        this.textBox5.Name = "textBox5";
        this.textBox5.Size = new System.Drawing.Size(110, 58);
        this.textBox5.TabIndex = 10;
        this.textBox5.UseSystemPasswordChar = true;
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(85, 24);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(59, 15);
        this.label3.TabIndex = 11;
        this.label3.Text = "TextBoxes";
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(332, 31);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(100, 15);
        this.label4.TabIndex = 12;
        this.label4.Text = "MaskedTextBoxes";
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(560, 31);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(109, 15);
        this.label5.TabIndex = 13;
        this.label5.Text = "Multiline TextBoxes";
        // 
        // label6
        // 
        this.label6.AutoSize = true;
        this.label6.Location = new System.Drawing.Point(560, 164);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(107, 15);
        this.label6.TabIndex = 14;
        this.label6.Text = "PasswordChar = \'*\'";
        // 
        // label7
        // 
        this.label7.AutoSize = true;
        this.label7.Location = new System.Drawing.Point(560, 70);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(174, 15);
        this.label7.TabIndex = 15;
        this.label7.Text = "UseSystemPasswordChar = true";
        // 
        // label8
        // 
        this.label8.AutoSize = true;
        this.label8.Location = new System.Drawing.Point(329, 148);
        this.label8.Name = "label8";
        this.label8.Size = new System.Drawing.Size(107, 15);
        this.label8.TabIndex = 16;
        this.label8.Text = "PasswordChar = \'*\'";
        // 
        // label9
        // 
        this.label9.AutoSize = true;
        this.label9.Location = new System.Drawing.Point(332, 70);
        this.label9.Name = "label9";
        this.label9.Size = new System.Drawing.Size(174, 15);
        this.label9.TabIndex = 17;
        this.label9.Text = "UseSystemPasswordChar = true";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(786, 338);
        this.Controls.Add(this.label9);
        this.Controls.Add(this.label8);
        this.Controls.Add(this.label7);
        this.Controls.Add(this.label6);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.textBox5);
        this.Controls.Add(this.textBox4);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.textBox3);
        this.Controls.Add(this.maskedTextBox3);
        this.Controls.Add(this.maskedTextBox2);
        this.Controls.Add(this.maskedTextBox1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.textBox2);
        this.Controls.Add(this.textBox1);
        this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.Name = "Form1";
        this.Text = "Form1";
        this.ResumeLayout(false);
        this.PerformLayout();
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

}

