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
            button2 = new Button();
            button3 = new Button();
            internalButton = new Button();
            _label = new Label();
            tabControl1.SuspendLayout();
            SuspendLayout();
            //
            // Set tooltip
            //
            //toolTip.SetToolTip(tabPage1, "Ultra super tabpage");

            //
            // Label
            //
            _label.Location = new Point(15, 180);
            _label.MaximumSize = new Size(150, 18);
            _label.AutoSize = true;
            _label.Text = "Some label as.dkgfhas.dfkgjs/dakfgj/sdfkgj/alsdkfjg asdlfmas/dlkmf/asdkmf/askldf/sakdfaskgn";
            _label.AutoEllipsis = true;

            //
            // button1
            //
            button1.Location = new System.Drawing.Point(15, 140);
            button1.Text = "Button1";
            button1.Click += ButtonClick;
            toolTip.SetToolTip(button1, "Button1");
            // button2
            //
            button2.Location = new System.Drawing.Point(100, 140);
            button2.Text = "Button2";
            button2.Click += Button2Click;
            toolTip.SetToolTip(button2, "Button2");
            // button3
            //
            button3.Location = new System.Drawing.Point(185, 140);
            button3.Text = "Button3";
            toolTip.SetToolTip(button3, "Button3");
            button3.Click += Button3Click;
            //
            // internalButton
            //
            tabPage1.Controls.Add(internalButton);
            internalButton.Location = new System.Drawing.Point(15, 15);
            internalButton.Size = new Size(100, 20);
            internalButton.BackColor = Color.White;
            internalButton.Text = "Internal button";
            toolTip.SetToolTip(internalButton, "Internal button");
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
            // tabPage1
            // 
            tabPage1.Location = new System.Drawing.Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(20, 20);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "tabPage1";
            //tabPage1.ToolTipText = "1_item";
            toolTip.SetToolTip(tabPage1, "1_item");

            tabPage1.BackColor = Color.Red;
            // 
            // tabPage2
            // 
            tabPage2.Location = new System.Drawing.Point(4, 22);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new System.Windows.Forms.Padding(3);
            tabPage2.Size = new System.Drawing.Size(20, 20);
            tabPage2.TabIndex = 0;
            tabPage2.Text = "tabPage2";
            //tabPage2.ToolTipText = "2_item";
            toolTip.SetToolTip(tabPage2, "2_item");

            tabPage2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(343, 200);
            Controls.Add(tabControl1);
            Controls.Add(button1);
            Controls.Add(button2);
            Controls.Add(button3);
            Controls.Add(_label);
            Name = "Form1";
            Text = "Form1";
            tabControl1.ResumeLayout(false);
            ResumeLayout(false);
        }

        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button button1;
        private Button button2;
        private Button button3;
        private Button internalButton;
        private ToolTip toolTip;
        private Label _label;
    }
}
