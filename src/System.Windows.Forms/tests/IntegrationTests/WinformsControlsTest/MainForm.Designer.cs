// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

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
        this.overarchingFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
        this.overarchingFlowLayoutPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // overarchingFlowLayoutPanel
        // 
        this.overarchingFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
        this.overarchingFlowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.overarchingFlowLayoutPanel.Location = new System.Drawing.Point(8, 8);
        this.overarchingFlowLayoutPanel.Name = "flowLayoutPanelUITypeEditors";
        this.overarchingFlowLayoutPanel.TabIndex = 0;
        // 
        // MainForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(570, 30);
        this.Controls.Add(this.overarchingFlowLayoutPanel);
        this.Name = "MainForm";
        this.Padding = new System.Windows.Forms.Padding(8);
        this.Text = "MenuForm";
        this.overarchingFlowLayoutPanel.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.FlowLayoutPanel overarchingFlowLayoutPanel;
}

