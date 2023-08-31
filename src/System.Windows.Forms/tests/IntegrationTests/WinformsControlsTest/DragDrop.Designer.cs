// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinformsControlsTest;

partial class DragDrop
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
        this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
        this.textBox = new System.Windows.Forms.TextBox();
        this.richTextBox = new System.Windows.Forms.RichTextBox();
        this.pictureBox1 = new System.Windows.Forms.PictureBox();
        this.pictureBox2 = new System.Windows.Forms.PictureBox();
        this.pictureBox3 = new System.Windows.Forms.PictureBox();
        this.pictureBox4 = new System.Windows.Forms.PictureBox();
        this.pictureBox5 = new System.Windows.Forms.PictureBox();
        this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
        this.buttonOpenCats = new System.Windows.Forms.Button();
        this.buttonClear = new System.Windows.Forms.Button();
        this.tableLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
        this.tableLayoutPanel2.SuspendLayout();
        this.SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.ColumnCount = 3;
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 115F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 55.55556F));
        this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 44.44445F));
        this.tableLayoutPanel1.Controls.Add(this.textBox, 2, 0);
        this.tableLayoutPanel1.Controls.Add(this.richTextBox, 1, 0);
        this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 0, 0);
        this.tableLayoutPanel1.Controls.Add(this.pictureBox2, 0, 1);
        this.tableLayoutPanel1.Controls.Add(this.pictureBox3, 0, 2);
        this.tableLayoutPanel1.Controls.Add(this.pictureBox4, 0, 3);
        this.tableLayoutPanel1.Controls.Add(this.pictureBox5, 0, 4);
        this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel2, 2, 5);
        this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 6;
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 112F));
        this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.tableLayoutPanel1.Size = new System.Drawing.Size(1226, 674);
        this.tableLayoutPanel1.TabIndex = 0;
        // 
        // textBox
        // 
        this.textBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.textBox.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.textBox.Location = new System.Drawing.Point(742, 3);
        this.textBox.Multiline = true;
        this.textBox.Name = "textBox";
        this.tableLayoutPanel1.SetRowSpan(this.textBox, 5);
        this.textBox.Size = new System.Drawing.Size(481, 619);
        this.textBox.TabIndex = 18;
        // 
        // richTextBox
        // 
        this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.richTextBox.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        this.richTextBox.Location = new System.Drawing.Point(134, 3);
        this.richTextBox.Name = "richTextBox";
        this.tableLayoutPanel1.SetRowSpan(this.richTextBox, 5);
        this.richTextBox.Size = new System.Drawing.Size(602, 619);
        this.richTextBox.TabIndex = 22;
        this.richTextBox.Text = "";
        // 
        // pictureBox1
        // 
        this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox1.InitialImage = null;
        this.pictureBox1.Location = new System.Drawing.Point(3, 3);
        this.pictureBox1.Name = "pictureBox1";
        this.pictureBox1.Size = new System.Drawing.Size(125, 119);
        this.pictureBox1.TabIndex = 13;
        this.pictureBox1.TabStop = false;
        // 
        // pictureBox2
        // 
        this.pictureBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pictureBox2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox2.InitialImage = null;
        this.pictureBox2.Location = new System.Drawing.Point(3, 128);
        this.pictureBox2.Name = "pictureBox2";
        this.pictureBox2.Size = new System.Drawing.Size(125, 119);
        this.pictureBox2.TabIndex = 13;
        this.pictureBox2.TabStop = false;
        // 
        // pictureBox3
        // 
        this.pictureBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pictureBox3.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox3.InitialImage = null;
        this.pictureBox3.Location = new System.Drawing.Point(3, 253);
        this.pictureBox3.Name = "pictureBox3";
        this.pictureBox3.Size = new System.Drawing.Size(125, 119);
        this.pictureBox3.TabIndex = 13;
        this.pictureBox3.TabStop = false;
        // 
        // pictureBox4
        // 
        this.pictureBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pictureBox4.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox4.InitialImage = null;
        this.pictureBox4.Location = new System.Drawing.Point(3, 378);
        this.pictureBox4.Name = "pictureBox4";
        this.pictureBox4.Size = new System.Drawing.Size(125, 119);
        this.pictureBox4.TabIndex = 13;
        this.pictureBox4.TabStop = false;
        // 
        // pictureBox5
        // 
        this.pictureBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
        this.pictureBox5.Dock = System.Windows.Forms.DockStyle.Fill;
        this.pictureBox5.InitialImage = null;
        this.pictureBox5.Location = new System.Drawing.Point(3, 503);
        this.pictureBox5.Name = "pictureBox5";
        this.pictureBox5.Size = new System.Drawing.Size(125, 119);
        this.pictureBox5.TabIndex = 13;
        this.pictureBox5.TabStop = false;
        // 
        // tableLayoutPanel2
        // 
        this.tableLayoutPanel2.ColumnCount = 2;
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel2.Controls.Add(this.buttonOpenCats, 0, 0);
        this.tableLayoutPanel2.Controls.Add(this.buttonClear, 1, 0);
        this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Right;
        this.tableLayoutPanel2.Location = new System.Drawing.Point(1046, 628);
        this.tableLayoutPanel2.Name = "tableLayoutPanel2";
        this.tableLayoutPanel2.RowCount = 1;
        this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.tableLayoutPanel2.Size = new System.Drawing.Size(177, 43);
        this.tableLayoutPanel2.TabIndex = 23;
        // 
        // buttonOpenCats
        // 
        this.buttonOpenCats.Dock = System.Windows.Forms.DockStyle.Right;
        this.buttonOpenCats.Location = new System.Drawing.Point(3, 3);
        this.buttonOpenCats.Name = "buttonOpenCats";
        this.buttonOpenCats.Size = new System.Drawing.Size(82, 37);
        this.buttonOpenCats.TabIndex = 21;
        this.buttonOpenCats.Text = "Open Cats";
        this.buttonOpenCats.UseVisualStyleBackColor = true;
        // 
        // buttonClear
        // 
        this.buttonClear.Dock = System.Windows.Forms.DockStyle.Right;
        this.buttonClear.Location = new System.Drawing.Point(92, 3);
        this.buttonClear.Name = "buttonClear";
        this.buttonClear.Size = new System.Drawing.Size(82, 37);
        this.buttonClear.TabIndex = 20;
        this.buttonClear.Text = "Clear";
        this.buttonClear.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1226, 625);
        this.Controls.Add(this.tableLayoutPanel1);
        this.Name = "Form1";
        this.Text = "Form1";
        this.tableLayoutPanel1.ResumeLayout(false);
        this.tableLayoutPanel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
        this.tableLayoutPanel2.ResumeLayout(false);
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.PictureBox pictureBox1;
    private System.Windows.Forms.PictureBox pictureBox2;
    private System.Windows.Forms.PictureBox pictureBox3;
    private System.Windows.Forms.PictureBox pictureBox4;
    private System.Windows.Forms.PictureBox pictureBox5;
    private System.Windows.Forms.RichTextBox richTextBox;
    private System.Windows.Forms.TextBox textBox;
    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
    private System.Windows.Forms.Button buttonClear;
    private System.Windows.Forms.Button buttonOpenCats;
}
