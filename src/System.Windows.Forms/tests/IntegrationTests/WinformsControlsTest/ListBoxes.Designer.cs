// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest
{
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
            this.listBox = new System.Windows.Forms.ListBox();
            this.textBox = new System.Windows.Forms.TextBox();
            this.addButton = new System.Windows.Forms.Button();
            this.deleteButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBox
            // 
            this.listBox.Location = new System.Drawing.Point(20, 20);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(200, 150);
            // 
            // textBox
            //

            this.textBox.Location = new System.Drawing.Point(240, 20);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(100, 30);
            // 
            // addButton
            // 
            this.addButton.AutoSize = true;
            this.addButton.Location = new System.Drawing.Point(240, 60);
            this.addButton.Text = "Add";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // addButton
            // 
            this.deleteButton.AutoSize = true;
            this.deleteButton.Location = new System.Drawing.Point(240, 90);
            this.deleteButton.Text = "Delete";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 250);
            this.Controls.Add(this.listBox);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.deleteButton);
            this.Name = "ListBoxes";
            this.Text = "LIstBoxes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.TextBox textBox;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button deleteButton;
    }
}

