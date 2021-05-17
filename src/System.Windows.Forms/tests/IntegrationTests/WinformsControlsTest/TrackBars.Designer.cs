// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest
{
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
            this.lblTrackBarValue = new System.Windows.Forms.Label();
            this.numericMinimum = new System.Windows.Forms.NumericUpDown();
            this.numericMaximum = new System.Windows.Forms.NumericUpDown();
            this.lblMinimum = new System.Windows.Forms.Label();
            this.lblMaximum = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.gbOrientation.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinimum)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaximum)).BeginInit();
            this.SuspendLayout();
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(413, 67);
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(350, 350);
            this.trackBar1.TabIndex = 0;
            this.trackBar1.Value = 5;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // rbHorizontal
            // 
            this.rbHorizontal.AutoSize = true;
            this.rbHorizontal.Checked = true;
            this.rbHorizontal.Location = new System.Drawing.Point(20, 35);
            this.rbHorizontal.Name = "rbHorizontal";
            this.rbHorizontal.Size = new System.Drawing.Size(100, 24);
            this.rbHorizontal.TabIndex = 1;
            this.rbHorizontal.TabStop = true;
            this.rbHorizontal.Text = "Horizontal";
            this.rbHorizontal.UseVisualStyleBackColor = true;
            this.rbHorizontal.CheckedChanged += new System.EventHandler(this.rbHorizontal_CheckedChanged);
            // 
            // rbVertical
            // 
            this.rbVertical.AutoSize = true;
            this.rbVertical.Location = new System.Drawing.Point(20, 74);
            this.rbVertical.Name = "rbVertical";
            this.rbVertical.Size = new System.Drawing.Size(79, 24);
            this.rbVertical.TabIndex = 2;
            this.rbVertical.Text = "Vertical";
            this.rbVertical.UseVisualStyleBackColor = true;
            this.rbVertical.CheckedChanged += new System.EventHandler(this.rbVertical_CheckedChanged);
            // 
            // gbOrientation
            // 
            this.gbOrientation.Controls.Add(this.rbHorizontal);
            this.gbOrientation.Controls.Add(this.rbVertical);
            this.gbOrientation.Location = new System.Drawing.Point(56, 67);
            this.gbOrientation.Name = "gbOrientation";
            this.gbOrientation.Size = new System.Drawing.Size(250, 125);
            this.gbOrientation.TabIndex = 3;
            this.gbOrientation.TabStop = false;
            this.gbOrientation.Text = "Orientation";
            // 
            // chbRightToLeft
            // 
            this.chbRightToLeft.AutoSize = true;
            this.chbRightToLeft.Location = new System.Drawing.Point(56, 223);
            this.chbRightToLeft.Name = "chbRightToLeft";
            this.chbRightToLeft.Size = new System.Drawing.Size(107, 24);
            this.chbRightToLeft.TabIndex = 4;
            this.chbRightToLeft.Text = "RightToLeft";
            this.chbRightToLeft.UseVisualStyleBackColor = true;
            this.chbRightToLeft.CheckedChanged += new System.EventHandler(this.chbRightToLeft_CheckedChanged);
            // 
            // chbRightToLeftLayout
            // 
            this.chbRightToLeftLayout.AutoSize = true;
            this.chbRightToLeftLayout.Location = new System.Drawing.Point(56, 279);
            this.chbRightToLeftLayout.Name = "chbRightToLeftLayout";
            this.chbRightToLeftLayout.Size = new System.Drawing.Size(151, 24);
            this.chbRightToLeftLayout.TabIndex = 5;
            this.chbRightToLeftLayout.Text = "RightToLeftLayout";
            this.chbRightToLeftLayout.UseVisualStyleBackColor = true;
            this.chbRightToLeftLayout.CheckedChanged += new System.EventHandler(this.chbRightToLeftLayout_CheckedChanged);
            // 
            // lblTrackBarValue
            // 
            this.lblTrackBarValue.AutoSize = true;
            this.lblTrackBarValue.Location = new System.Drawing.Point(422, 26);
            this.lblTrackBarValue.Name = "lblTrackBarValue";
            this.lblTrackBarValue.Size = new System.Drawing.Size(50, 20);
            this.lblTrackBarValue.TabIndex = 6;
            this.lblTrackBarValue.Text = "label1";
            // 
            // numericMinimum
            // 
            this.numericMinimum.Location = new System.Drawing.Point(56, 359);
            this.numericMinimum.Name = "numericMinimum";
            this.numericMinimum.Size = new System.Drawing.Size(150, 27);
            this.numericMinimum.TabIndex = 7;
            this.numericMinimum.ValueChanged += new System.EventHandler(this.numericMinimum_ValueChanged);
            // 
            // numericMaximum
            // 
            this.numericMaximum.Location = new System.Drawing.Point(56, 418);
            this.numericMaximum.Name = "numericMaximum";
            this.numericMaximum.Size = new System.Drawing.Size(150, 27);
            this.numericMaximum.TabIndex = 8;
            this.numericMaximum.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numericMaximum.ValueChanged += new System.EventHandler(this.numericMaximum_ValueChanged);
            // 
            // lblMinimum
            // 
            this.lblMinimum.AutoSize = true;
            this.lblMinimum.Location = new System.Drawing.Point(56, 336);
            this.lblMinimum.Name = "lblMinimum";
            this.lblMinimum.Size = new System.Drawing.Size(72, 20);
            this.lblMinimum.TabIndex = 9;
            this.lblMinimum.Text = "Minimum";
            // 
            // lblMaximum
            // 
            this.lblMaximum.AutoSize = true;
            this.lblMaximum.Location = new System.Drawing.Point(56, 395);
            this.lblMaximum.Name = "lblMaximum";
            this.lblMaximum.Size = new System.Drawing.Size(75, 20);
            this.lblMaximum.TabIndex = 10;
            this.lblMaximum.Text = "Maximum";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(826, 468);
            this.Controls.Add(this.lblMaximum);
            this.Controls.Add(this.lblMinimum);
            this.Controls.Add(this.numericMaximum);
            this.Controls.Add(this.numericMinimum);
            this.Controls.Add(this.lblTrackBarValue);
            this.Controls.Add(this.chbRightToLeftLayout);
            this.Controls.Add(this.chbRightToLeft);
            this.Controls.Add(this.gbOrientation);
            this.Controls.Add(this.trackBar1);
            this.Name = "TrackBars";
            this.Text = "TrackBars";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.gbOrientation.ResumeLayout(false);
            this.gbOrientation.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericMinimum)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericMaximum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.RadioButton rbHorizontal;
        private System.Windows.Forms.RadioButton rbVertical;
        private System.Windows.Forms.GroupBox gbOrientation;
        private System.Windows.Forms.CheckBox chbRightToLeft;
        private System.Windows.Forms.CheckBox chbRightToLeftLayout;
        private System.Windows.Forms.Label lblTrackBarValue;
        private System.Windows.Forms.NumericUpDown numericMinimum;
        private System.Windows.Forms.NumericUpDown numericMaximum;
        private System.Windows.Forms.Label lblMinimum;
        private System.Windows.Forms.Label lblMaximum;
    }
}

