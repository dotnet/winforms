// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class TrackBars
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
        this.trackBar1 = new System.Windows.Forms.TrackBar();
        this.rbHorizontal = new System.Windows.Forms.RadioButton();
        this.rbVertical = new System.Windows.Forms.RadioButton();
        this.gbOrientation = new System.Windows.Forms.GroupBox();
        this.chbRightToLeft = new System.Windows.Forms.CheckBox();
        this.chbRightToLeftLayout = new System.Windows.Forms.CheckBox();
        this.tickstyleNone = new System.Windows.Forms.CheckBox();
        this.numericMinimum = new System.Windows.Forms.NumericUpDown();
        this.numericMaximum = new System.Windows.Forms.NumericUpDown();
        this.numericFrequency = new System.Windows.Forms.NumericUpDown();
        this.lblMinimum = new System.Windows.Forms.Label();
        this.lblMaximum = new System.Windows.Forms.Label();
        this.lblTickFrequency = new System.Windows.Forms.Label();
        this.lblTrackBarSize = new System.Windows.Forms.Label();
        this.lblTrackBarValue = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
        this.gbOrientation.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numericMinimum)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericMaximum)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericFrequency)).BeginInit();
        this.SuspendLayout();
        // 
        // trackBar1
        // 
        this.trackBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.trackBar1.Location = new System.Drawing.Point(10, 30);
        this.trackBar1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.trackBar1.Name = "trackBar1";
        this.trackBar1.Size = new System.Drawing.Size(457, 45);
        this.trackBar1.TabIndex = 0;
        this.trackBar1.Scroll += this.trackBar1_Scroll;
        this.trackBar1.SizeChanged += this.trackBar1_SizeChanged;
        // 
        // rbHorizontal
        // 
        this.rbHorizontal.AutoSize = true;
        this.rbHorizontal.Checked = true;
        this.rbHorizontal.Location = new System.Drawing.Point(18, 26);
        this.rbHorizontal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.rbHorizontal.Name = "rbHorizontal";
        this.rbHorizontal.Size = new System.Drawing.Size(80, 19);
        this.rbHorizontal.TabIndex = 1;
        this.rbHorizontal.TabStop = true;
        this.rbHorizontal.Text = "Horizontal";
        this.rbHorizontal.UseVisualStyleBackColor = true;
        this.rbHorizontal.CheckedChanged += this.rbHorizontal_CheckedChanged;
        // 
        // rbVertical
        // 
        this.rbVertical.AutoSize = true;
        this.rbVertical.Location = new System.Drawing.Point(18, 56);
        this.rbVertical.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.rbVertical.Name = "rbVertical";
        this.rbVertical.Size = new System.Drawing.Size(63, 19);
        this.rbVertical.TabIndex = 2;
        this.rbVertical.Text = "Vertical";
        this.rbVertical.UseVisualStyleBackColor = true;
        this.rbVertical.CheckedChanged += this.rbVertical_CheckedChanged;
        // 
        // gbOrientation
        // 
        this.gbOrientation.Controls.Add(this.rbHorizontal);
        this.gbOrientation.Controls.Add(this.rbVertical);
        this.gbOrientation.Location = new System.Drawing.Point(65, 78);
        this.gbOrientation.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.gbOrientation.Name = "gbOrientation";
        this.gbOrientation.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.gbOrientation.Size = new System.Drawing.Size(168, 84);
        this.gbOrientation.TabIndex = 3;
        this.gbOrientation.TabStop = false;
        this.gbOrientation.Text = "Orientation";
        // 
        // chbRightToLeft
        // 
        this.chbRightToLeft.AutoSize = true;
        this.chbRightToLeft.Location = new System.Drawing.Point(83, 176);
        this.chbRightToLeft.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.chbRightToLeft.Name = "chbRightToLeft";
        this.chbRightToLeft.Size = new System.Drawing.Size(86, 19);
        this.chbRightToLeft.TabIndex = 4;
        this.chbRightToLeft.Text = "RightToLeft";
        this.chbRightToLeft.UseVisualStyleBackColor = true;
        this.chbRightToLeft.CheckedChanged += this.chbRightToLeft_CheckedChanged;
        // 
        // chbRightToLeftLayout
        // 
        this.chbRightToLeftLayout.AutoSize = true;
        this.chbRightToLeftLayout.Location = new System.Drawing.Point(83, 199);
        this.chbRightToLeftLayout.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.chbRightToLeftLayout.Name = "chbRightToLeftLayout";
        this.chbRightToLeftLayout.Size = new System.Drawing.Size(122, 19);
        this.chbRightToLeftLayout.TabIndex = 5;
        this.chbRightToLeftLayout.Text = "RightToLeftLayout";
        this.chbRightToLeftLayout.UseVisualStyleBackColor = true;
        this.chbRightToLeftLayout.CheckedChanged += this.chbRightToLeftLayout_CheckedChanged;
        // 
        // tickstyleNone
        // 
        this.tickstyleNone.AutoSize = true;
        this.tickstyleNone.Location = new System.Drawing.Point(83, 222);
        this.tickstyleNone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.tickstyleNone.Name = "tickstyleNone";
        this.tickstyleNone.Size = new System.Drawing.Size(100, 19);
        this.tickstyleNone.TabIndex = 11;
        this.tickstyleNone.Text = "TickstyleNone";
        this.tickstyleNone.UseVisualStyleBackColor = true;
        this.tickstyleNone.CheckedChanged += this.tickstyleNone_CheckedChanged;
        // 
        // numericMinimum
        // 
        this.numericMinimum.Location = new System.Drawing.Point(260, 104);
        this.numericMinimum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.numericMinimum.Minimum = new decimal(new int[] {
        -1,
        -1,
        -1,
        -2147483648});
        this.numericMinimum.Name = "numericMinimum";
        this.numericMinimum.Size = new System.Drawing.Size(131, 23);
        this.numericMinimum.TabIndex = 7;
        this.numericMinimum.Value = new decimal(new int[] {
        100,
        0,
        0,
        -2147483648});
        this.numericMinimum.ValueChanged += this.numericMinimum_ValueChanged;
        // 
        // numericMaximum
        // 
        this.numericMaximum.Location = new System.Drawing.Point(260, 149);
        this.numericMaximum.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.numericMaximum.Maximum = new decimal(new int[] {
        -1,
        -1,
        -1,
        0});
        this.numericMaximum.Name = "numericMaximum";
        this.numericMaximum.Size = new System.Drawing.Size(131, 23);
        this.numericMaximum.TabIndex = 8;
        this.numericMaximum.Value = new decimal(new int[] {
        100,
        0,
        0,
        0});
        this.numericMaximum.ValueChanged += this.numericMaximum_ValueChanged;
        // 
        // numericFrequency
        // 
        this.numericFrequency.Location = new System.Drawing.Point(260, 193);
        this.numericFrequency.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.numericFrequency.Minimum = new decimal(new int[] {
        1,
        0,
        0,
        0});
        this.numericFrequency.Name = "numericFrequency";
        this.numericFrequency.Size = new System.Drawing.Size(131, 23);
        this.numericFrequency.TabIndex = 11;
        this.numericFrequency.Value = new decimal(new int[] {
        1,
        0,
        0,
        0});
        this.numericFrequency.ValueChanged += this.numericFrequency_ValueChanged;
        // 
        // lblMinimum
        // 
        this.lblMinimum.AutoSize = true;
        this.lblMinimum.Location = new System.Drawing.Point(260, 87);
        this.lblMinimum.Name = "lblMinimum";
        this.lblMinimum.Size = new System.Drawing.Size(60, 15);
        this.lblMinimum.TabIndex = 9;
        this.lblMinimum.Text = "Minimum";
        // 
        // lblMaximum
        // 
        this.lblMaximum.AutoSize = true;
        this.lblMaximum.Location = new System.Drawing.Point(260, 131);
        this.lblMaximum.Name = "lblMaximum";
        this.lblMaximum.Size = new System.Drawing.Size(62, 15);
        this.lblMaximum.TabIndex = 10;
        this.lblMaximum.Text = "Maximum";
        // 
        // lblTickFrequency
        // 
        this.lblTickFrequency.AutoSize = true;
        this.lblTickFrequency.Location = new System.Drawing.Point(260, 176);
        this.lblTickFrequency.Name = "lblTickFrequency";
        this.lblTickFrequency.Size = new System.Drawing.Size(86, 15);
        this.lblTickFrequency.TabIndex = 12;
        this.lblTickFrequency.Text = "Tick Frequency";
        // 
        // lblTrackBarSize
        // 
        this.lblTrackBarSize.AutoSize = true;
        this.lblTrackBarSize.Location = new System.Drawing.Point(166, 9);
        this.lblTrackBarSize.Name = "lblTrackBarSize";
        this.lblTrackBarSize.Size = new System.Drawing.Size(39, 15);
        this.lblTrackBarSize.TabIndex = 13;
        this.lblTrackBarSize.Text = "Size: 0";
        // 
        // lblTrackBarValue
        // 
        this.lblTrackBarValue.AutoSize = true;
        this.lblTrackBarValue.Location = new System.Drawing.Point(12, 9);
        this.lblTrackBarValue.Name = "lblTrackBarValue";
        this.lblTrackBarValue.Size = new System.Drawing.Size(47, 15);
        this.lblTrackBarValue.TabIndex = 14;
        this.lblTrackBarValue.Text = "Value: 0";
        // 
        // TrackBars
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(479, 308);
        this.Controls.Add(this.lblTrackBarValue);
        this.Controls.Add(this.lblTrackBarSize);
        this.Controls.Add(this.lblTickFrequency);
        this.Controls.Add(this.lblMaximum);
        this.Controls.Add(this.lblMinimum);
        this.Controls.Add(this.numericFrequency);
        this.Controls.Add(this.numericMaximum);
        this.Controls.Add(this.numericMinimum);
        this.Controls.Add(this.chbRightToLeftLayout);
        this.Controls.Add(this.chbRightToLeft);
        this.Controls.Add(this.gbOrientation);
        this.Controls.Add(this.trackBar1);
        this.Controls.Add(this.tickstyleNone);
        this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
        this.Name = "TrackBars";
        this.Text = "TrackBars";
        ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
        this.gbOrientation.ResumeLayout(false);
        this.gbOrientation.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.numericMinimum)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericMaximum)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.numericFrequency)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TrackBar trackBar1;
    private System.Windows.Forms.RadioButton rbHorizontal;
    private System.Windows.Forms.RadioButton rbVertical;
    private System.Windows.Forms.GroupBox gbOrientation;
    private System.Windows.Forms.CheckBox chbRightToLeft;
    private System.Windows.Forms.CheckBox tickstyleNone;
    private System.Windows.Forms.CheckBox chbRightToLeftLayout;
    private System.Windows.Forms.NumericUpDown numericMinimum;
    private System.Windows.Forms.NumericUpDown numericMaximum;
    private System.Windows.Forms.NumericUpDown numericFrequency;
    private System.Windows.Forms.Label lblMinimum;
    private System.Windows.Forms.Label lblMaximum;
    private System.Windows.Forms.Label lblTickFrequency;
    private System.Windows.Forms.Label lblTrackBarSize;
    private System.Windows.Forms.Label lblTrackBarValue;
}

