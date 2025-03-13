// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class MultipleControls
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
        this.progressBar1 = new System.Windows.Forms.ProgressBar();
        this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
        this.button1 = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.maskedTextBox1 = new System.Windows.Forms.MaskedTextBox();
        this.richTextBox1 = new System.Windows.Forms.RichTextBox();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabPage1 = new System.Windows.Forms.TabPage();
        this.comboBox1 = new System.Windows.Forms.ComboBox();
        this.tabPage2 = new System.Windows.Forms.TabPage();
        this.checkBox1 = new System.Windows.Forms.CheckBox();
        this.checkBox2 = new System.Windows.Forms.CheckBox();
        this.radioButton2 = new System.Windows.Forms.RadioButton();
        this.radioButton1 = new System.Windows.Forms.RadioButton();
        this.groupBox1 = new System.Windows.Forms.GroupBox();
        this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
        this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        this.domainUpDown1 = new System.Windows.Forms.DomainUpDown();
        this.linkLabel1 = new System.Windows.Forms.LinkLabel();
        this.linkLabel2 = new System.Windows.Forms.LinkLabel();
        this.checkedListBox2 = new System.Windows.Forms.CheckedListBox();
        this.tabControl1.SuspendLayout();
        this.tabPage1.SuspendLayout();
        this.tabPage2.SuspendLayout();
        this.groupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        this.SuspendLayout();
        // 
        // progressBar1
        // 
        this.progressBar1.Location = new System.Drawing.Point(0, 0);
        this.progressBar1.Name = "progressBar1";
        this.progressBar1.Size = new System.Drawing.Size(331, 27);
        this.progressBar1.TabIndex = 0;
        // 
        // backgroundWorker1
        // 
        this.backgroundWorker1.WorkerReportsProgress = true;
        this.backgroundWorker1.DoWork += this.backgroundWorker1_DoWork;
        this.backgroundWorker1.ProgressChanged += this.backgroundWorker1_ProgressChanged;
        // 
        // button1
        // 
        this.button1.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.button1.Location = new System.Drawing.Point(15, 50);
        this.button1.Name = "button1";
        this.button1.Size = new System.Drawing.Size(88, 27);
        this.button1.TabIndex = 1;
        this.button1.Text = "button1";
        this.button1.UseVisualStyleBackColor = true;
        // 
        // label1
        // 
        this.label1.AccessibleDescription = "Test Label AccessibleDescription";
        this.label1.AccessibleName = "Test Label AccessibleName";
        this.label1.AccessibleRole = System.Windows.Forms.AccessibleRole.Indicator;
        this.label1.AutoSize = true;
        this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
        this.label1.Location = new System.Drawing.Point(15, 84);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(38, 15);
        this.label1.TabIndex = 2;
        this.label1.Text = "label1";
        // 
        // maskedTextBox1
        // 
        this.maskedTextBox1.Location = new System.Drawing.Point(15, 104);
        this.maskedTextBox1.Name = "maskedTextBox1";
        this.maskedTextBox1.Size = new System.Drawing.Size(116, 23);
        this.maskedTextBox1.TabIndex = 5;
        this.maskedTextBox1.Text = "Masked";
        // 
        // richTextBox1
        // 
        this.richTextBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.richTextBox1.Location = new System.Drawing.Point(15, 135);
        this.richTextBox1.Name = "richTextBox1";
        this.richTextBox1.Size = new System.Drawing.Size(116, 110);
        this.richTextBox1.TabIndex = 6;
        this.richTextBox1.Text = "LLLL";
        // 
        // textBox1
        // 
        this.textBox1.Location = new System.Drawing.Point(15, 264);
        this.textBox1.Name = "textBox1";
        this.textBox1.PlaceholderText = "Type some text here...";
        this.textBox1.Size = new System.Drawing.Size(116, 23);
        this.textBox1.TabIndex = 7;
        this.textBox1.Text = "LLLLL";
        // 
        // tabControl1
        // 
        this.tabControl1.Controls.Add(this.tabPage1);
        this.tabControl1.Controls.Add(this.tabPage2);
        this.tabControl1.Location = new System.Drawing.Point(141, 50);
        this.tabControl1.Name = "tabControl1";
        this.tabControl1.SelectedIndex = 0;
        this.tabControl1.ShowToolTips = true;
        this.tabControl1.Size = new System.Drawing.Size(233, 115);
        this.tabControl1.TabIndex = 8;
        // 
        // tabPage1
        // 
        this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.tabPage1.Controls.Add(this.comboBox1);
        this.tabPage1.Location = new System.Drawing.Point(4, 24);
        this.tabPage1.Name = "tabPage1";
        this.tabPage1.Size = new System.Drawing.Size(225, 87);
        this.tabPage1.TabIndex = 0;
        this.tabPage1.Text = "tabPage1";
        this.tabPage1.ToolTipText = "I am tabPage1!";
        this.tabPage1.UseVisualStyleBackColor = true;
        // 
        // comboBox1
        // 
        this.comboBox1.FormattingEnabled = true;
        this.comboBox1.Location = new System.Drawing.Point(22, 30);
        this.comboBox1.Name = "comboBox1";
        this.comboBox1.Size = new System.Drawing.Size(140, 23);
        this.comboBox1.TabIndex = 0;
        // 
        // tabPage2
        // 
        this.tabPage2.Controls.Add(this.checkBox1);
        this.tabPage2.Controls.Add(this.checkBox2);
        this.tabPage2.Location = new System.Drawing.Point(4, 24);
        this.tabPage2.Name = "tabPage2";
        this.tabPage2.Size = new System.Drawing.Size(225, 87);
        this.tabPage2.TabIndex = 1;
        this.tabPage2.Text = "tabPage2";
        this.tabPage2.ToolTipText = "I am tabPage2!\r\nI am multiline tooltip!";
        this.tabPage2.UseVisualStyleBackColor = true;
        // 
        // checkBox1
        // 
        this.checkBox1.AutoSize = true;
        this.checkBox1.Location = new System.Drawing.Point(8, 22);
        this.checkBox1.Name = "checkBox1";
        this.checkBox1.Size = new System.Drawing.Size(83, 19);
        this.checkBox1.TabIndex = 0;
        this.checkBox1.Text = "checkBox1";
        this.checkBox1.UseVisualStyleBackColor = true;
        // 
        // checkBox2
        // 
        this.checkBox2.AutoSize = true;
        this.checkBox2.Location = new System.Drawing.Point(8, 50);
        this.checkBox2.Name = "checkBox2";
        this.checkBox2.Size = new System.Drawing.Size(153, 19);
        this.checkBox2.TabIndex = 0;
        this.checkBox2.Text = "Three state CheckBox";
        this.checkBox2.ThreeState = true;
        this.checkBox2.UseVisualStyleBackColor = true;
        // 
        // radioButton2
        // 
        this.radioButton2.Location = new System.Drawing.Point(19, 55);
        this.radioButton2.Name = "radioButton2";
        this.radioButton2.Size = new System.Drawing.Size(99, 20);
        this.radioButton2.TabIndex = 1;
        this.radioButton2.TabStop = true;
        this.radioButton2.Text = "radioButton2";
        this.radioButton2.UseVisualStyleBackColor = true;
        // 
        // radioButton1
        // 
        this.radioButton1.Location = new System.Drawing.Point(19, 29);
        this.radioButton1.Name = "radioButton1";
        this.radioButton1.Size = new System.Drawing.Size(99, 20);
        this.radioButton1.TabIndex = 0;
        this.radioButton1.TabStop = true;
        this.radioButton1.Text = "radioButton1";
        this.radioButton1.UseVisualStyleBackColor = true;
        // 
        // groupBox1
        // 
        this.groupBox1.AccessibleDescription = "Test GroupBox AccessibleDescription";
        this.groupBox1.AccessibleName = "Test GroupBox AccessibleName";
        this.groupBox1.AccessibleRole = System.Windows.Forms.AccessibleRole.Table;
        this.groupBox1.Controls.Add(this.radioButton2);
        this.groupBox1.Controls.Add(this.radioButton1);
        this.groupBox1.Location = new System.Drawing.Point(146, 197);
        this.groupBox1.Name = "groupBox1";
        this.groupBox1.Size = new System.Drawing.Size(206, 93);
        this.groupBox1.TabIndex = 9;
        this.groupBox1.TabStop = false;
        this.groupBox1.Text = "groupBox1";
        // 
        // checkedListBox1
        // 
        this.checkedListBox1.BackColor = System.Drawing.SystemColors.Window;
        this.checkedListBox1.FormattingEnabled = true;
        this.checkedListBox1.Items.AddRange(new object[] {
        "Washington",
        "California",
        "Florida",
        "New York"});
        this.checkedListBox1.Location = new System.Drawing.Point(378, 49);
        this.checkedListBox1.Name = "checkedListBox1";
        this.checkedListBox1.Size = new System.Drawing.Size(140, 112);
        this.checkedListBox1.TabIndex = 10;
        this.checkedListBox1.SetItemCheckState(0, CheckState.Checked);
        this.checkedListBox1.SetItemCheckState(1, CheckState.Indeterminate);
        // 
        // numericUpDown1
        // 
        this.numericUpDown1.Location = new System.Drawing.Point(378, 197);
        this.numericUpDown1.Name = "numericUpDown1";
        this.numericUpDown1.Size = new System.Drawing.Size(140, 23);
        this.numericUpDown1.TabIndex = 11;
        // 
        // domainUpDown1
        // 
        this.domainUpDown1.Items.Add("First");
        this.domainUpDown1.Items.Add("Second");
        this.domainUpDown1.Items.Add("Third");
        this.domainUpDown1.Items.Add("Fourth");
        this.domainUpDown1.Location = new System.Drawing.Point(378, 227);
        this.domainUpDown1.Name = "domainUpDown1";
        this.domainUpDown1.Size = new System.Drawing.Size(140, 23);
        this.domainUpDown1.TabIndex = 12;
        this.domainUpDown1.Text = "domainUpDown1";
        // 
        // linkLabel1
        // 
        this.linkLabel1.AutoSize = true;
        this.linkLabel1.Location = new System.Drawing.Point(62, 84);
        this.linkLabel1.Name = "linkLabel1";
        this.linkLabel1.Size = new System.Drawing.Size(60, 15);
        this.linkLabel1.TabIndex = 3;
        this.linkLabel1.TabStop = true;
        this.linkLabel1.Text = "linkLabel1";
        // 
        // linkLabel2
        // 
        this.linkLabel2.AutoSize = true;
        this.linkLabel2.Location = new System.Drawing.Point(378, 255);
        this.linkLabel2.Name = "linkLabel2";
        this.linkLabel2.Size = new System.Drawing.Size(108, 21);
        this.linkLabel2.TabIndex = 4;
        this.linkLabel2.TabStop = true;
        this.linkLabel2.Text = "Home MSN Github";
        this.linkLabel2.UseCompatibleTextRendering = true;
        // 
        // checkedListBox2
        // 
        this.checkedListBox2.BackColor = System.Drawing.SystemColors.Window;
        this.checkedListBox2.FormattingEnabled = true;
        this.checkedListBox2.Items.AddRange(new object[] {
        "Beijing",
        "Moscow",
        "Ivanovo",
        "ShangHai",
        "Vichuga",
        "Tokyo"});
        this.checkedListBox2.Location = new System.Drawing.Point(525, 49);
        this.checkedListBox2.Name = "checkedListBox2";
        this.checkedListBox2.Size = new System.Drawing.Size(140, 58);
        this.checkedListBox2.TabIndex = 13;
        this.checkedListBox2.SetItemCheckState(0, CheckState.Checked);
        this.checkedListBox2.SetItemCheckState(1, CheckState.Indeterminate);
        // 
        // MultipleControls
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(746, 458);
        this.Controls.Add(this.checkedListBox2);
        this.Controls.Add(this.domainUpDown1);
        this.Controls.Add(this.numericUpDown1);
        this.Controls.Add(this.checkedListBox1);
        this.Controls.Add(this.groupBox1);
        this.Controls.Add(this.tabControl1);
        this.Controls.Add(this.textBox1);
        this.Controls.Add(this.richTextBox1);
        this.Controls.Add(this.maskedTextBox1);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.button1);
        this.Controls.Add(this.progressBar1);
        this.Controls.Add(this.linkLabel1);
        this.Controls.Add(this.linkLabel2);
        this.Name = "MultipleControls";
        this.Text = "These look ok";
        this.Load += this.Test3_Load;
        this.tabControl1.ResumeLayout(false);
        this.tabPage1.ResumeLayout(false);
        this.tabPage2.ResumeLayout(false);
        this.tabPage2.PerformLayout();
        this.groupBox1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ProgressBar progressBar1;
    private System.ComponentModel.BackgroundWorker backgroundWorker1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.MaskedTextBox maskedTextBox1;
    private System.Windows.Forms.RichTextBox richTextBox1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.TabPage tabPage2;
    private System.Windows.Forms.CheckBox checkBox1;
    private System.Windows.Forms.CheckBox checkBox2;
    private System.Windows.Forms.RadioButton radioButton2;
    private System.Windows.Forms.RadioButton radioButton1;
    private System.Windows.Forms.GroupBox groupBox1;
    private System.Windows.Forms.CheckedListBox checkedListBox1;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.DomainUpDown domainUpDown1;
    private System.Windows.Forms.LinkLabel linkLabel1;
    private System.Windows.Forms.LinkLabel linkLabel2;
    private System.Windows.Forms.CheckedListBox checkedListBox2;
}
