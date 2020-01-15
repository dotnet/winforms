// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    partial class TabControlTest
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

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            tabPage2 = new TabPage();
            toolTip = new ToolTip();
            button1 = new Button();
            tabControl1.SuspendLayout();
            SuspendLayout();
            //
            // setToolTipButton
            //
            button1.Location = new System.Drawing.Point(15, 140);
            button1.Text = "Button";
            button1.Click += ButtonClick;
            // 
            // tabControl1
            // 
            tabControl1.TabPages.Add(tabPage1);
            tabControl1.TabPages.Add(tabPage2);
            tabControl1.Location = new Point(15, 15);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(200, 100);
            tabControl1.TabIndex = 0;
            tabControl1.ShowToolTips = true;
            //
            //tooltip
            //
            toolTip.AutoPopDelay = 1000;
            toolTip.InitialDelay = 374;
            // 
            // tabPage1
            // 
            tabPage1.Location = new System.Drawing.Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(20, 20);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            tabPage1.ToolTipText = "1_item";
            tabPage1.BackColor = Color.Red;
            //tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabPage2
            // 
            tabPage2.Location = new System.Drawing.Point(4, 22);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(20, 20);
            tabPage2.TabIndex = 0;
            tabPage2.Text = "tabPage2";
            tabPage2.ToolTipText = "2_item";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(343, 183);
            Controls.Add(this.tabControl1);
            Controls.Add(this.button1);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button button1;
        private ToolTip toolTip;
    }
}
