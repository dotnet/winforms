// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class DataGridViewInVirtualModeTest
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
        this.dataGridView1 = new System.Windows.Forms.DataGridView();
        this.personNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.ageColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.hasAJobColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        this.genderColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
        ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
        this.SuspendLayout();
        // 
        // dataGridView1
        // 
        this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
        this.personNameColumn,
        this.ageColumn,
        this.hasAJobColumn,
        this.genderColumn});
        this.dataGridView1.Location = new System.Drawing.Point(0, 15);
        this.dataGridView1.Name = "dataGridView1";
        this.dataGridView1.Size = new System.Drawing.Size(492, 150);
        this.dataGridView1.TabIndex = 5;
        this.dataGridView1.VirtualMode = true;
        // 
        // personNameColumn
        // 
        this.personNameColumn.HeaderText = "Name";
        this.personNameColumn.Name = "personNameColumn";
        // 
        // ageColumn
        // 
        this.ageColumn.HeaderText = "Age";
        this.ageColumn.Name = "ageColumn";
        // 
        // hasAJobColumn
        // 
        this.hasAJobColumn.HeaderText = "Has a job";
        this.hasAJobColumn.Name = "hasAJobColumn";
        // 
        // genderColumn
        // 
        this.genderColumn.HeaderText = "Gender";
        this.genderColumn.Name = "genderColumn";
        this.genderColumn.Items.Add("Male");
        this.genderColumn.Items.Add("Female");
        // 
        // DataGridViewInVirtualModeTest
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
        this.ClientSize = new System.Drawing.Size(492, 180);
        this.Controls.Add(this.dataGridView1);
        this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        this.Name = "DataGridViewInVirtualModeTest";
        this.Text = "DataGridView in Virtual mode";
        ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.DataGridViewTextBoxColumn personNameColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ageColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn hasAJobColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn genderColumn;
}
