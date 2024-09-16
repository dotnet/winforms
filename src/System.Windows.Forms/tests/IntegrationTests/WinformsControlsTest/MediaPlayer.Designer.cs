// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class MediaPlayer
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

        axWindowsMediaPlayer1.Dispose();
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new(typeof(MediaPlayer));
        this.axWindowsMediaPlayer1 = new AxWMPLib.AxWindowsMediaPlayer();
        ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).BeginInit();
        this.SuspendLayout();
        // 
        // axWindowsMediaPlayer1
        // 
        this.axWindowsMediaPlayer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.axWindowsMediaPlayer1.Enabled = true;
        this.axWindowsMediaPlayer1.Location = new System.Drawing.Point(0, 0);
        this.axWindowsMediaPlayer1.Name = "axWindowsMediaPlayer1";
        this.axWindowsMediaPlayer1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWindowsMediaPlayer1.OcxState")));
        this.axWindowsMediaPlayer1.Size = new System.Drawing.Size(800, 450);
        this.axWindowsMediaPlayer1.TabIndex = 0;
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(800, 450);
        this.Controls.Add(this.axWindowsMediaPlayer1);
        this.Name = "Form1";
        this.Text = "Form1";
        ((System.ComponentModel.ISupportInitialize)(this.axWindowsMediaPlayer1)).EndInit();
        this.ResumeLayout(false);

    }

    #endregion
    private AxWMPLib.AxWindowsMediaPlayer axWindowsMediaPlayer1;
}
