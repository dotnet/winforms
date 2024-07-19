// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

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
        tableLayoutPanel1 = new TableLayoutPanel();
        textBox = new TextBox();
        richTextBox = new RichTextBox();
        pictureBox1 = new PictureBox();
        pictureBox2 = new PictureBox();
        pictureBox3 = new PictureBox();
        pictureBox4 = new PictureBox();
        pictureBox5 = new PictureBox();
        tableLayoutPanel2 = new TableLayoutPanel();
        buttonOpenCats = new Button();
        buttonClear = new Button();
        tableLayoutPanel1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
        tableLayoutPanel2.SuspendLayout();
        SuspendLayout();
        // 
        // tableLayoutPanel1
        // 
        tableLayoutPanel1.ColumnCount = 3;
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 115F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55.55556F));
        tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 44.44445F));
        tableLayoutPanel1.Controls.Add(textBox, 2, 0);
        tableLayoutPanel1.Controls.Add(richTextBox, 1, 0);
        tableLayoutPanel1.Controls.Add(pictureBox1, 0, 0);
        tableLayoutPanel1.Controls.Add(pictureBox2, 0, 1);
        tableLayoutPanel1.Controls.Add(pictureBox3, 0, 2);
        tableLayoutPanel1.Controls.Add(pictureBox4, 0, 3);
        tableLayoutPanel1.Controls.Add(pictureBox5, 0, 4);
        tableLayoutPanel1.Controls.Add(tableLayoutPanel2, 2, 5);
        tableLayoutPanel1.Dock = DockStyle.Fill;
        tableLayoutPanel1.Location = new Point(0, 0);
        tableLayoutPanel1.Name = "tableLayoutPanel1";
        tableLayoutPanel1.RowCount = 6;
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 112F));
        tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tableLayoutPanel1.Size = new Size(1226, 625);
        tableLayoutPanel1.TabIndex = 0;
        // 
        // textBox
        // 
        textBox.Dock = DockStyle.Fill;
        textBox.Font = new Font("Segoe UI", 14.25F);
        textBox.Location = new Point(735, 3);
        textBox.Multiline = true;
        textBox.Name = "textBox";
        tableLayoutPanel1.SetRowSpan(textBox, 5);
        textBox.Size = new Size(488, 554);
        textBox.TabIndex = 18;
        // 
        // richTextBox
        // 
        richTextBox.Dock = DockStyle.Fill;
        richTextBox.Font = new Font("Segoe UI", 14.25F);
        richTextBox.Location = new Point(118, 3);
        richTextBox.Name = "richTextBox";
        tableLayoutPanel1.SetRowSpan(richTextBox, 5);
        richTextBox.Size = new Size(611, 554);
        richTextBox.TabIndex = 22;
        richTextBox.Text = "";
        // 
        // pictureBox1
        // 
        pictureBox1.BorderStyle = BorderStyle.FixedSingle;
        pictureBox1.Dock = DockStyle.Fill;
        pictureBox1.InitialImage = null;
        pictureBox1.Location = new Point(3, 3);
        pictureBox1.Name = "pictureBox1";
        pictureBox1.Size = new Size(109, 106);
        pictureBox1.TabIndex = 13;
        pictureBox1.TabStop = false;
        // 
        // pictureBox2
        // 
        pictureBox2.BorderStyle = BorderStyle.FixedSingle;
        pictureBox2.Dock = DockStyle.Fill;
        pictureBox2.InitialImage = null;
        pictureBox2.Location = new Point(3, 115);
        pictureBox2.Name = "pictureBox2";
        pictureBox2.Size = new Size(109, 106);
        pictureBox2.TabIndex = 13;
        pictureBox2.TabStop = false;
        // 
        // pictureBox3
        // 
        pictureBox3.BorderStyle = BorderStyle.FixedSingle;
        pictureBox3.Dock = DockStyle.Fill;
        pictureBox3.InitialImage = null;
        pictureBox3.Location = new Point(3, 227);
        pictureBox3.Name = "pictureBox3";
        pictureBox3.Size = new Size(109, 106);
        pictureBox3.TabIndex = 13;
        pictureBox3.TabStop = false;
        // 
        // pictureBox4
        // 
        pictureBox4.BorderStyle = BorderStyle.FixedSingle;
        pictureBox4.Dock = DockStyle.Fill;
        pictureBox4.InitialImage = null;
        pictureBox4.Location = new Point(3, 339);
        pictureBox4.Name = "pictureBox4";
        pictureBox4.Size = new Size(109, 106);
        pictureBox4.TabIndex = 13;
        pictureBox4.TabStop = false;
        // 
        // pictureBox5
        // 
        pictureBox5.BorderStyle = BorderStyle.FixedSingle;
        pictureBox5.Dock = DockStyle.Fill;
        pictureBox5.InitialImage = null;
        pictureBox5.Location = new Point(3, 451);
        pictureBox5.Name = "pictureBox5";
        pictureBox5.Size = new Size(109, 106);
        pictureBox5.TabIndex = 13;
        pictureBox5.TabStop = false;
        // 
        // tableLayoutPanel2
        // 
        tableLayoutPanel2.ColumnCount = 2;
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.Controls.Add(buttonOpenCats, 0, 0);
        tableLayoutPanel2.Controls.Add(buttonClear, 1, 0);
        tableLayoutPanel2.Dock = DockStyle.Right;
        tableLayoutPanel2.Location = new Point(1046, 563);
        tableLayoutPanel2.Name = "tableLayoutPanel2";
        tableLayoutPanel2.RowCount = 1;
        tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tableLayoutPanel2.Size = new Size(177, 59);
        tableLayoutPanel2.TabIndex = 23;
        // 
        // buttonOpenCats
        // 
        buttonOpenCats.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonOpenCats.Location = new Point(3, 3);
        buttonOpenCats.Name = "buttonOpenCats";
        buttonOpenCats.Size = new Size(82, 53);
        buttonOpenCats.TabIndex = 21;
        buttonOpenCats.Text = "Open Cats";
        buttonOpenCats.UseVisualStyleBackColor = true;
        // 
        // buttonClear
        // 
        buttonClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        buttonClear.Location = new Point(92, 3);
        buttonClear.Name = "buttonClear";
        buttonClear.Size = new Size(82, 53);
        buttonClear.TabIndex = 20;
        buttonClear.Text = "Clear";
        buttonClear.UseVisualStyleBackColor = true;
        // 
        // Form1
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1226, 625);
        Controls.Add(tableLayoutPanel1);
        Name = "Form1";
        Text = "Form1";
        tableLayoutPanel1.ResumeLayout(false);
        tableLayoutPanel1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
        ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
        tableLayoutPanel2.ResumeLayout(false);
        ResumeLayout(false);
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
