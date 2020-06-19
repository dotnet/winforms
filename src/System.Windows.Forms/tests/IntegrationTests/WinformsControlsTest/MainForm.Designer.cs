// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest
{
    partial class MainForm
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
            this.flowLayoutPanelUITypeEditors = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanelUITypeEditors.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanelUITypeEditors
            // 
            this.flowLayoutPanelUITypeEditors.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanelUITypeEditors.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanelUITypeEditors.Location = new System.Drawing.Point(8, 8);
            this.flowLayoutPanelUITypeEditors.Name = "flowLayoutPanelUITypeEditors";
            this.flowLayoutPanelUITypeEditors.TabIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(570, 30);
            this.Controls.Add(this.flowLayoutPanelUITypeEditors);
            this.Name = "MainForm";
            this.Padding = new System.Windows.Forms.Padding(8);
            this.Text = "MenuForm";
            this.flowLayoutPanelUITypeEditors.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelUITypeEditors;
    }
}

