// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class DataGridViewTest
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
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new();
        System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new();
        this.dataGridView1 = new System.Windows.Forms.DataGridView();
        this.column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.column3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        this.column4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
        this.column5 = new System.Windows.Forms.DataGridViewComboBoxColumn();
        this.column6 = new System.Windows.Forms.DataGridViewImageColumn();
        this.column7 = new System.Windows.Forms.DataGridViewButtonColumn();
        this.column8 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        this.currentDPILabel1 = new WinFormsControlsTest.CurrentDPILabel();
        this.changeFontButton = new System.Windows.Forms.Button();
        this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
        this.label1 = new System.Windows.Forms.Label();
        this.resetFontButton = new System.Windows.Forms.Button();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
        this.SuspendLayout();
        // 
        // dataGridView1
        // 
        dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
        dataGridViewCellStyle1.Font = new System.Drawing.Font("Cascadia Code", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
        dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
        this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
        this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        this.column1,
        this.column2,
        this.column5,
        this.column3,
        this.column4,
        this.column6,
        this.column7,
        this.column8, });
        dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
        dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Info;
        dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Maroon;
        dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
        dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
        dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
        this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
        this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.dataGridView1.Location = new System.Drawing.Point(0, 0);
        this.dataGridView1.Name = "dataGridView1";
        this.dataGridView1.Size = new System.Drawing.Size(750, 150);
        this.dataGridView1.TabIndex = 0;
        // 
        // column1
        // 
        this.column1.HeaderText = "Column1";
        this.column1.Name = "column1";
        // 
        // column2
        // 
        this.column2.HeaderText = "Column2";
        this.column2.Name = "column2";
        // 
        // column3
        // 
        this.column3.HeaderText = "Column3";
        this.column3.Name = "column3";
        // 
        // column4
        // 
        this.column4.HeaderText = "Column4";
        this.column4.Name = "column4";
        this.column4.Items.AddRange(new[] {"First", "Second"});
        this.column4.AutoComplete = true;
        // 
        // column5
        // 
        this.column5.HeaderText = "Hidden Column";
        this.column5.Name = "column5";
        this.column5.Visible = false;
        // 
        // column6
        // 
        this.column6.HeaderText = "Column6";
        this.column6.Name = "column6";
        // 
        // column7
        // 
        this.column7.HeaderText = "Column7";
        this.column7.Name = "column7";
        this.column7.Text = "Button";
        this.column7.UseColumnTextForButtonValue = true;
        // 
        // column8
        // 
        this.column8.HeaderText = "Column8";
        this.column8.Name = "column8";
        this.column8.ThreeState = true;
        // 
        // currentDPILabel1
        // 
        this.currentDPILabel1.AutoSize = true;
        this.currentDPILabel1.Location = new System.Drawing.Point(12, 174);
        this.currentDPILabel1.Name = "currentDPILabel1";
        this.currentDPILabel1.Size = new System.Drawing.Size(116, 13);
        this.currentDPILabel1.TabIndex = 1;
        this.currentDPILabel1.Text = "Current scaling is 100%";
        // 
        // changeFontButton
        // 
        this.changeFontButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        this.changeFontButton.AutoSize = true;
        this.changeFontButton.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.changeFontButton.Location = new System.Drawing.Point(12, 234);
        this.changeFontButton.Name = "button1";
        this.changeFontButton.Size = new System.Drawing.Size(154, 29);
        this.changeFontButton.TabIndex = 1;
        this.changeFontButton.Text = "Change DGV font";
        this.changeFontButton.UseVisualStyleBackColor = true;
        this.changeFontButton.Click += this.changeFontButton_Click;
        // 
        // numericUpDown1
        // 
        this.numericUpDown1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        this.numericUpDown1.Location = new System.Drawing.Point(89, 208);
        this.numericUpDown1.Maximum = new decimal(new int[] {
        14,
        0,
        0,
        0});
        this.numericUpDown1.Minimum = new decimal(new int[] {
        8,
        0,
        0,
        0});
        this.numericUpDown1.Name = "numericUpDown1";
        this.numericUpDown1.Size = new System.Drawing.Size(64, 20);
        this.numericUpDown1.TabIndex = 2;
        this.numericUpDown1.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        this.numericUpDown1.Value = new decimal(new int[] {
        8,
        0,
        0,
        0});
        this.numericUpDown1.ValueChanged += this.numericUpDown1_ValueChanged;
        // 
        // label1
        // 
        this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(12, 210);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(51, 13);
        this.label1.TabIndex = 3;
        this.label1.Text = "Form font";
        // 
        // resetFontButton
        // 
        this.resetFontButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)));
        this.resetFontButton.AutoSize = true;
        this.resetFontButton.Font = new System.Drawing.Font("Consolas", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.resetFontButton.Location = new System.Drawing.Point(192, 234);
        this.resetFontButton.Name = "button2";
        this.resetFontButton.Size = new System.Drawing.Size(154, 29);
        this.resetFontButton.TabIndex = 4;
        this.resetFontButton.Text = "Reset DGV font";
        this.resetFontButton.UseVisualStyleBackColor = true;
        this.resetFontButton.Click += this.resetFontButton_Click;
        // 
        // DataGridViewTest
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.ClientSize = new System.Drawing.Size(750, 272);
        this.Controls.Add(this.resetFontButton);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.numericUpDown1);
        this.Controls.Add(this.currentDPILabel1);
        this.Controls.Add(this.changeFontButton);
        this.Controls.Add(this.dataGridView1);
        this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.Name = "DataGridViewTest";
        this.Text = "DataGridView";
        ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn column1;
    private System.Windows.Forms.DataGridViewTextBoxColumn column2;
    private System.Windows.Forms.DataGridViewCheckBoxColumn column3;
    private System.Windows.Forms.DataGridViewComboBoxColumn column4;
    private System.Windows.Forms.DataGridViewComboBoxColumn column5;
    private System.Windows.Forms.DataGridViewImageColumn column6;
    private System.Windows.Forms.DataGridViewButtonColumn column7;
    private System.Windows.Forms.DataGridViewCheckBoxColumn column8;
    private WinFormsControlsTest.CurrentDPILabel currentDPILabel1;
    private System.Windows.Forms.Button changeFontButton;
    private System.Windows.Forms.NumericUpDown numericUpDown1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button resetFontButton;
}
