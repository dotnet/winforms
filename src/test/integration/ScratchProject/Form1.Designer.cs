// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ScratchProject;

partial class Form1
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
        textBox3 = new TextBox();
        label1 = new Label();
        label2 = new Label();
        label3 = new Label();
        textBox4 = new TextBox();
        textBox5 = new TextBox();
        textBox6 = new TextBox();
        label4 = new Label();
        label5 = new Label();
        label6 = new Label();
        panel1 = new Panel();
        checkBox1 = new CheckBox();
        radioButton1 = new RadioButton();
        textBox7 = new TextBox();
        panel2 = new Panel();
        checkBox2 = new CheckBox();
        radioButton2 = new RadioButton();
        textBox8 = new TextBox();
        button1 = new Button();
        button2 = new Button();
        comboBox1 = new ComboBox();
        comboBox2 = new ComboBox();
        domainUpDown1 = new DomainUpDown();
        domainUpDown2 = new DomainUpDown();
        numericUpDown1 = new NumericUpDown();
        numericUpDown2 = new NumericUpDown();
        panel1.SuspendLayout();
        panel2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDown2).BeginInit();
        SuspendLayout();
        // 
        // textBox1
        // 
        textBox1.Location = new Point(173, 37);
        textBox1.Name = "textBox1";
        textBox1.Size = new Size(175, 35);
        textBox1.TabIndex = 0;
        // 
        // textBox2
        // 
        textBox2.BorderStyle = BorderStyle.FixedSingle;
        textBox2.Location = new Point(173, 186);
        textBox2.Name = "textBox2";
        textBox2.Size = new Size(175, 35);
        textBox2.TabIndex = 0;
        // 
        // textBox3
        // 
        textBox3.BorderStyle = BorderStyle.None;
        textBox3.Location = new Point(173, 250);
        textBox3.Name = "textBox3";
        textBox3.Size = new Size(175, 28);
        textBox3.TabIndex = 0;
        // 
        // label1
        // 
        label1.AutoSize = true;
        label1.Location = new Point(45, 37);
        label1.Name = "label1";
        label1.Size = new Size(92, 30);
        label1.TabIndex = 1;
        label1.Text = "Fixed3D:";
        // 
        // label2
        // 
        label2.AutoSize = true;
        label2.Location = new Point(45, 191);
        label2.Name = "label2";
        label2.Size = new Size(122, 30);
        label2.TabIndex = 1;
        label2.Text = "FixedSingle:";
        // 
        // label3
        // 
        label3.AutoSize = true;
        label3.Location = new Point(45, 250);
        label3.Name = "label3";
        label3.Size = new Size(69, 30);
        label3.TabIndex = 1;
        label3.Text = "None:";
        // 
        // textBox4
        // 
        textBox4.Location = new Point(180, 37);
        textBox4.Name = "textBox4";
        textBox4.Size = new Size(175, 48);
        textBox4.TabIndex = 0;
        // 
        // textBox5
        // 
        textBox5.BorderStyle = BorderStyle.FixedSingle;
        textBox5.Location = new Point(180, 181);
        textBox5.Name = "textBox5";
        textBox5.Size = new Size(175, 44);
        textBox5.TabIndex = 0;
        // 
        // textBox6
        // 
        textBox6.BorderStyle = BorderStyle.None;
        textBox6.Location = new Point(180, 248);
        textBox6.Name = "textBox6";
        textBox6.Size = new Size(175, 42);
        textBox6.TabIndex = 0;
        // 
        // label4
        // 
        label4.AutoSize = true;
        label4.Location = new Point(52, 37);
        label4.Name = "label4";
        label4.Size = new Size(92, 30);
        label4.TabIndex = 1;
        label4.Text = "Fixed3D:";
        // 
        // label5
        // 
        label5.AutoSize = true;
        label5.Location = new Point(52, 186);
        label5.Name = "label5";
        label5.Size = new Size(122, 30);
        label5.TabIndex = 1;
        label5.Text = "FixedSingle:";
        // 
        // label6
        // 
        label6.AutoSize = true;
        label6.Location = new Point(52, 248);
        label6.Name = "label6";
        label6.Size = new Size(69, 30);
        label6.TabIndex = 1;
        label6.Text = "None:";
        // 
        // panel1
        // 
        panel1.Controls.Add(numericUpDown1);
        panel1.Controls.Add(domainUpDown1);
        panel1.Controls.Add(comboBox1);
        panel1.Controls.Add(button1);
        panel1.Controls.Add(checkBox1);
        panel1.Controls.Add(radioButton1);
        panel1.Controls.Add(label1);
        panel1.Controls.Add(textBox7);
        panel1.Controls.Add(textBox1);
        panel1.Controls.Add(label3);
        panel1.Controls.Add(textBox2);
        panel1.Controls.Add(textBox3);
        panel1.Controls.Add(label2);
        panel1.Location = new Point(22, 54);
        panel1.Name = "panel1";
        panel1.Size = new Size(519, 728);
        panel1.TabIndex = 2;
        // 
        // checkBox1
        // 
        checkBox1.AutoSize = true;
        checkBox1.Location = new Point(45, 387);
        checkBox1.Name = "checkBox1";
        checkBox1.Size = new Size(137, 34);
        checkBox1.TabIndex = 3;
        checkBox1.Text = "checkBox1";
        checkBox1.UseVisualStyleBackColor = true;
        // 
        // radioButton1
        // 
        radioButton1.AutoSize = true;
        radioButton1.BackColor = SystemColors.ActiveCaption;
        radioButton1.Location = new Point(45, 325);
        radioButton1.Name = "radioButton1";
        radioButton1.Size = new Size(158, 34);
        radioButton1.TabIndex = 2;
        radioButton1.TabStop = true;
        radioButton1.Text = "radioButton1";
        radioButton1.UseVisualStyleBackColor = false;
        // 
        // textBox7
        // 
        textBox7.Location = new Point(173, 105);
        textBox7.Multiline = true;
        textBox7.Name = "textBox7";
        textBox7.Size = new Size(175, 60);
        textBox7.TabIndex = 0;
        textBox7.Text = "test";
        // 
        // panel2
        // 
        panel2.Controls.Add(numericUpDown2);
        panel2.Controls.Add(domainUpDown2);
        panel2.Controls.Add(comboBox2);
        panel2.Controls.Add(button2);
        panel2.Controls.Add(checkBox2);
        panel2.Controls.Add(radioButton2);
        panel2.Controls.Add(label4);
        panel2.Controls.Add(textBox8);
        panel2.Controls.Add(textBox4);
        panel2.Controls.Add(label6);
        panel2.Controls.Add(textBox5);
        panel2.Controls.Add(label5);
        panel2.Controls.Add(textBox6);
        panel2.Location = new Point(573, 54);
        panel2.Name = "panel2";
        panel2.Size = new Size(536, 728);
        panel2.TabIndex = 3;
        panel2.VisualStylesMode = VisualStylesMode.Net11;
        // 
        // checkBox2
        // 
        checkBox2.Appearance = Appearance.ToggleSwitch;
        checkBox2.AutoSize = true;
        checkBox2.Location = new Point(52, 387);
        checkBox2.Name = "checkBox2";
        checkBox2.Size = new Size(169, 30);
        checkBox2.TabIndex = 5;
        checkBox2.Text = "checkBox2";
        checkBox2.UseVisualStyleBackColor = true;
        // 
        // radioButton2
        // 
        radioButton2.Appearance = Appearance.ToggleSwitch;
        radioButton2.AutoSize = true;
        radioButton2.BackColor = SystemColors.ActiveCaption;
        radioButton2.Location = new Point(52, 325);
        radioButton2.Name = "radioButton2";
        radioButton2.Size = new Size(191, 30);
        radioButton2.TabIndex = 4;
        radioButton2.TabStop = true;
        radioButton2.Text = "radioButton2";
        radioButton2.UseVisualStyleBackColor = false;
        // 
        // textBox8
        // 
        textBox8.Location = new Point(180, 105);
        textBox8.Multiline = true;
        textBox8.Name = "textBox8";
        textBox8.Size = new Size(175, 70);
        textBox8.TabIndex = 0;
        textBox8.Text = "text";
        // 
        // button1
        // 
        button1.Location = new Point(45, 449);
        button1.Name = "button1";
        button1.Size = new Size(131, 40);
        button1.TabIndex = 4;
        button1.Text = "button1";
        button1.UseVisualStyleBackColor = true;
        // 
        // button2
        // 
        button2.Location = new Point(52, 449);
        button2.Name = "button2";
        button2.Size = new Size(147, 49);
        button2.TabIndex = 4;
        button2.Text = "button1";
        button2.UseVisualStyleBackColor = true;
        // 
        // comboBox1
        // 
        comboBox1.FormattingEnabled = true;
        comboBox1.Items.AddRange(new object[] { "111", "222" });
        comboBox1.Location = new Point(35, 531);
        comboBox1.Name = "comboBox1";
        comboBox1.Size = new Size(212, 38);
        comboBox1.TabIndex = 5;
        comboBox1.Text = "000";
        // 
        // comboBox2
        // 
        comboBox2.FormattingEnabled = true;
        comboBox2.Items.AddRange(new object[] { "111", "222" });
        comboBox2.Location = new Point(52, 521);
        comboBox2.Name = "comboBox2";
        comboBox2.Size = new Size(212, 48);
        comboBox2.TabIndex = 5;
        comboBox2.Text = "000";
        // 
        // domainUpDown1
        // 
        domainUpDown1.Location = new Point(35, 601);
        domainUpDown1.Name = "domainUpDown1";
        domainUpDown1.Size = new Size(210, 35);
        domainUpDown1.TabIndex = 6;
        domainUpDown1.Text = "domainUpDown1";
        // 
        // domainUpDown2
        // 
        domainUpDown2.Location = new Point(52, 601);
        domainUpDown2.Name = "domainUpDown2";
        domainUpDown2.Size = new Size(258, 48);
        domainUpDown2.TabIndex = 6;
        domainUpDown2.Text = "domainUpDown1";
        // 
        // numericUpDown1
        // 
        numericUpDown1.Location = new Point(35, 668);
        numericUpDown1.Name = "numericUpDown1";
        numericUpDown1.Size = new Size(210, 35);
        numericUpDown1.TabIndex = 7;
        // 
        // numericUpDown2
        // 
        numericUpDown2.Location = new Point(52, 668);
        numericUpDown2.Name = "numericUpDown2";
        numericUpDown2.Size = new Size(258, 48);
        numericUpDown2.TabIndex = 7;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(12F, 30F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1121, 890);
        Controls.Add(panel2);
        Controls.Add(panel1);
        Name = "Form1";
        Text = "Form1";
        panel1.ResumeLayout(false);
        panel1.PerformLayout();
        panel2.ResumeLayout(false);
        panel2.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numericUpDown1).EndInit();
        ((System.ComponentModel.ISupportInitialize)numericUpDown2).EndInit();
        ResumeLayout(false);
    }

    #endregion

    private TextBox textBox1;
    private TextBox textBox2;
    private TextBox textBox3;
    private Label label1;
    private Label label2;
    private Label label3;
    private TextBox textBox4;
    private TextBox textBox5;
    private TextBox textBox6;
    private Label label4;
    private Label label5;
    private Label label6;
    private Panel panel1;
    private Panel panel2;
    private CheckBox checkBox1;
    private RadioButton radioButton1;
    private CheckBox checkBox2;
    private RadioButton radioButton2;
    private TextBox textBox7;
    private TextBox textBox8;
    private Button button1;
    private Button button2;
    private DomainUpDown domainUpDown1;
    private ComboBox comboBox1;
    private DomainUpDown domainUpDown2;
    private ComboBox comboBox2;
    private NumericUpDown numericUpDown1;
    private NumericUpDown numericUpDown2;
}
