// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class ListBoxes
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
        this.label1 = new System.Windows.Forms.Label();
        this.listBox1 = new System.Windows.Forms.ListBox();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.addButton1 = new System.Windows.Forms.Button();
        this.deleteButton1 = new System.Windows.Forms.Button();
        this.label2 = new System.Windows.Forms.Label();
        this.listBox2 = new System.Windows.Forms.CheckedListBox();
        this.textBox2 = new System.Windows.Forms.TextBox();
        this.addButton2 = new System.Windows.Forms.Button();
        this.deleteButton2 = new System.Windows.Forms.Button();
        this.label3 = new System.Windows.Forms.Label();
        this.listBox3 = new System.Windows.Forms.ListBox();
        this.textBox3 = new System.Windows.Forms.TextBox();
        this.addButton3 = new System.Windows.Forms.Button();
        this.deleteButton3 = new System.Windows.Forms.Button();
        this.label4 = new System.Windows.Forms.Label();
        this.listBox4 = new System.Windows.Forms.ListBox();
        this.textBox4 = new System.Windows.Forms.TextBox();
        this.addButton4 = new System.Windows.Forms.Button();
        this.deleteButton4 = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // label1
        //
        this.label1.Location = new System.Drawing.Point(20, 10);
        this.label1.Size = new System.Drawing.Size(200, 20);
        this.label1.Text = "ListBox with DrawMode.Normal";
        // 
        // listBox1
        // 
        this.listBox1.Location = new System.Drawing.Point(20, 40);
        this.listBox1.Name = "listBox1";
        this.listBox1.Size = new System.Drawing.Size(200, 150);
        this.listBox1.DrawMode = System.Windows.Forms.DrawMode.Normal;
        this.listBox1.SelectionMode = SelectionMode.MultiSimple;
        // 
        // textBox1
        //
        this.textBox1.Location = new System.Drawing.Point(240, 40);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(100, 30);
        // 
        // addButton1
        // 
        this.addButton1.AutoSize = true;
        this.addButton1.Location = new System.Drawing.Point(240, 80);
        this.addButton1.Text = "Add";
        this.addButton1.Tag = new object[]{ listBox1, textBox1 };
        this.addButton1.Click += this.addButton_Click;
        // 
        // deleteButton1
        // 
        this.deleteButton1.AutoSize = true;
        this.deleteButton1.Location = new System.Drawing.Point(240, 110);
        this.deleteButton1.Text = "Delete";
        this.deleteButton1.Tag = listBox1;
        this.deleteButton1.Click += this.deleteButton_Click;
        // 
        // label2
        //
        this.label2.Location = new System.Drawing.Point(20, 200);
        this.label2.Text = "CheckedListBox";
        // 
        // listBox2
        // 
        this.listBox2.Location = new System.Drawing.Point(20, 240);
        this.listBox2.Name = "listBox2";
        this.listBox2.Size = new System.Drawing.Size(200, 150);
        // 
        // textBox2
        //
        this.textBox2.Location = new System.Drawing.Point(240, 240);
        this.textBox2.Name = "textBox2";
        this.textBox2.Size = new System.Drawing.Size(100, 30);
        // 
        // addButton2
        // 
        this.addButton2.AutoSize = true;
        this.addButton2.Location = new System.Drawing.Point(240, 300);
        this.addButton2.Text = "Add";
        this.addButton2.Tag = new object[] { listBox2, textBox2 };
        this.addButton2.Click += this.addButton_Click;
        // 
        // deleteButton2
        // 
        this.deleteButton2.AutoSize = true;
        this.deleteButton2.Location = new System.Drawing.Point(240, 330);
        this.deleteButton2.Text = "Delete";
        this.deleteButton2.Tag = listBox2;
        this.deleteButton2.Click += this.deleteButton_Click;
        // 
        // label3
        //
        this.label3.Location = new System.Drawing.Point(360, 10);
        this.label3.Size = new System.Drawing.Size(250, 20);
        this.label3.Text = "ListBox DrawMode.OwnerDrawFixed";
        // 
        // listBox3
        // 
        this.listBox3.Location = new System.Drawing.Point(360, 40);
        this.listBox3.Name = "listBox3";
        this.listBox3.Size = new System.Drawing.Size(200, 150);
        this.listBox3.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
        this.listBox3.DrawItem += ListBox_DrawItem;
        // 
        // textBox3
        //
        this.textBox3.Location = new System.Drawing.Point(600, 40);
        this.textBox3.Name = "textBox3";
        this.textBox3.Size = new System.Drawing.Size(100, 30);
        // 
        // addButton3
        // 
        this.addButton3.AutoSize = true;
        this.addButton3.Location = new System.Drawing.Point(600, 80);
        this.addButton3.Text = "Add";
        this.addButton3.Tag = new object[] { listBox3, textBox3 };
        this.addButton3.Click += this.addButton_Click;
        // 
        // deleteButton3
        // 
        this.deleteButton3.AutoSize = true;
        this.deleteButton3.Location = new System.Drawing.Point(600, 110);
        this.deleteButton3.Text = "Delete";
        this.deleteButton3.Tag = listBox3;
        this.deleteButton3.Click += this.deleteButton_Click;
        // 
        // label4
        //
        this.label4.Location = new System.Drawing.Point(360, 200);
        this.label4.Size = new System.Drawing.Size(250, 20);
        this.label4.Text = "ListBox DrawMode.OwnerDrawVariable";
        // 
        // listBox4
        // 
        this.listBox4.Location = new System.Drawing.Point(360, 240);
        this.listBox4.Name = "listBox4";
        this.listBox4.Size = new System.Drawing.Size(200, 150);
        this.listBox4.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawVariable;
        this.listBox4.DrawItem += ListBox_DrawItem;
        this.listBox4.MeasureItem += ListBox_MeasureItem;
        // 
        // textBox4
        //
        this.textBox4.Location = new System.Drawing.Point(600, 240);
        this.textBox4.Name = "textBox4";
        this.textBox4.Size = new System.Drawing.Size(100, 30);
        // 
        // addButton4
        // 
        this.addButton4.AutoSize = true;
        this.addButton4.Location = new System.Drawing.Point(600, 300);
        this.addButton4.Text = "Add";
        this.addButton4.Tag = new object[] { listBox4, textBox4 };
        this.addButton4.Click += this.addButton_Click;
        // 
        // deleteButton4
        // 
        this.deleteButton4.AutoSize = true;
        this.deleteButton4.Location = new System.Drawing.Point(600, 330);
        this.deleteButton4.Text = "Delete";
        this.deleteButton4.Tag = listBox4;
        this.deleteButton4.Click += this.deleteButton_Click;
        // 
        // Form
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 500);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.listBox1);
        this.Controls.Add(this.textBox1);
        this.Controls.Add(this.addButton1);
        this.Controls.Add(this.deleteButton1);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.listBox2);
        this.Controls.Add(this.textBox2);
        this.Controls.Add(this.addButton2);
        this.Controls.Add(this.deleteButton2);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.listBox3);
        this.Controls.Add(this.textBox3);
        this.Controls.Add(this.addButton3);
        this.Controls.Add(this.deleteButton3);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.listBox4);
        this.Controls.Add(this.textBox4);
        this.Controls.Add(this.addButton4);
        this.Controls.Add(this.deleteButton4);
        this.Name = "ListBoxes";
        this.Text = "LIstBoxes";
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ListBox listBox1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button addButton1;
    private System.Windows.Forms.Button deleteButton1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.CheckedListBox listBox2;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Button addButton2;
    private System.Windows.Forms.Button deleteButton2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ListBox listBox3;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Button addButton3;
    private System.Windows.Forms.Button deleteButton3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.ListBox listBox4;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.Button addButton4;
    private System.Windows.Forms.Button deleteButton4;
}

