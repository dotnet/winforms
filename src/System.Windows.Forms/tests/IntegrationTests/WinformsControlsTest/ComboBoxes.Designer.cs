// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WinformsControlsTest
{
    partial class ComboBoxes
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
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.comboBox2 = new System.Windows.Forms.ComboBox();
            this.comboBox3 = new System.Windows.Forms.ComboBox();
            this.comboBox4 = new System.Windows.Forms.ComboBox();
            this.comboBox5 = new System.Windows.Forms.ComboBox();
            this.comboBox6 = new System.Windows.Forms.ComboBox();
            this.comboBox7 = new System.Windows.Forms.ComboBox();
            this.comboBox8 = new System.Windows.Forms.ComboBox();
            this.comboBox9 = new System.Windows.Forms.ComboBox();
            this.comboBox10 = new System.Windows.Forms.ComboBox();
            this.comboBox11 = new System.Windows.Forms.ComboBox();
            this.comboBox12 = new System.Windows.Forms.ComboBox();
            this.dataBoundComboBox = new System.Windows.Forms.ComboBox();
            this.currentDPILabel1 = new WinformsControlsTest.CurrentDPILabel();
            this.label1 = new System.Windows.Forms.Label();
            this.dataBoundLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            //
            // comboBox1
            //
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "dropdown",
            "system"});
            this.comboBox1.Location = new System.Drawing.Point(21, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 21);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.ComboBox1_SelectedIndexChanged);
            // 
            // comboBox2
            //
            this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox2.FormattingEnabled = true;
            this.comboBox2.Items.AddRange(new object[] {
            "dropdownlist",
            "aa",
            "bb",
            "cc",
            "d"});
            this.comboBox2.Location = new System.Drawing.Point(148, 22);
            this.comboBox2.Name = "comboBox2";
            this.comboBox2.Size = new System.Drawing.Size(121, 21);
            this.comboBox2.TabIndex = 4;
            //
            // comboBox3
            //
            this.comboBox3.FormattingEnabled = true;
            this.comboBox3.Items.AddRange(new object[] {
            "dropdown",
            "standard",
            "aa",
            "bb",
            "cc"});
            this.comboBox3.Location = new System.Drawing.Point(21, 59);
            this.comboBox3.Name = "comboBox3";
            this.comboBox3.Size = new System.Drawing.Size(121, 21);
            this.comboBox3.TabIndex = 5;
            //
            // comboBox4
            //
            this.comboBox4.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox4.FormattingEnabled = true;
            this.comboBox4.Items.AddRange(new object[] {
            "DropDownList",
            "aaa",
            "aaa",
            "aaa"});
            this.comboBox4.Location = new System.Drawing.Point(148, 59);
            this.comboBox4.Name = "comboBox4";
            this.comboBox4.Size = new System.Drawing.Size(121, 21);
            this.comboBox4.TabIndex = 6;
            //
            // comboBox5
            //
            this.comboBox5.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox5.FormattingEnabled = true;
            this.comboBox5.Items.AddRange(new object[] {
            "popup"});
            this.comboBox5.Location = new System.Drawing.Point(21, 96);
            this.comboBox5.Name = "comboBox5";
            this.comboBox5.Size = new System.Drawing.Size(121, 21);
            this.comboBox5.TabIndex = 7;
            //
            // comboBox6
            //
            this.comboBox6.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox6.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox6.FormattingEnabled = true;
            this.comboBox6.Items.AddRange(new object[] {
            "popup",
            "dropdownlist"});
            this.comboBox6.Location = new System.Drawing.Point(148, 96);
            this.comboBox6.Name = "comboBox6";
            this.comboBox6.Size = new System.Drawing.Size(121, 21);
            this.comboBox6.TabIndex = 8;
            //
            // comboBox7
            //
            this.comboBox7.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox7.FormattingEnabled = true;
            this.comboBox7.Items.AddRange(new object[] {
            "flat",
            "dropdown"});
            this.comboBox7.Location = new System.Drawing.Point(21, 136);
            this.comboBox7.Name = "comboBox7";
            this.comboBox7.Size = new System.Drawing.Size(121, 21);
            this.comboBox7.TabIndex = 9;
            //
            // comboBox8
            //
            this.comboBox8.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox8.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox8.FormattingEnabled = true;
            this.comboBox8.Items.AddRange(new object[] {
            "flat ",
            "drop down list"});
            this.comboBox8.Location = new System.Drawing.Point(148, 136);
            this.comboBox8.Name = "comboBox8";
            this.comboBox8.Size = new System.Drawing.Size(121, 21);
            this.comboBox8.TabIndex = 10;
            //
            // comboBox9
            //
            this.comboBox9.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBox9.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBox9.FormattingEnabled = true;
            this.comboBox9.Items.AddRange(new object[] {
            "flat ",
            "simple"});
            this.comboBox9.Location = new System.Drawing.Point(696, 22);
            this.comboBox9.Name = "comboBox9";
            this.comboBox9.Size = new System.Drawing.Size(121, 150);
            this.comboBox9.TabIndex = 14;
            //
            // comboBox10
            //
            this.comboBox10.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBox10.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox10.FormattingEnabled = true;
            this.comboBox10.Items.AddRange(new object[] {
            "popup",
            "simple"});
            this.comboBox10.Location = new System.Drawing.Point(560, 22);
            this.comboBox10.Name = "comboBox10";
            this.comboBox10.Size = new System.Drawing.Size(121, 150);
            this.comboBox10.TabIndex = 13;
            //
            // comboBox11
            //
            this.comboBox11.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBox11.FormattingEnabled = true;
            this.comboBox11.Items.AddRange(new object[] {
            "simple",
            "aaa",
            "aaa",
            "aaa"});
            this.comboBox11.Location = new System.Drawing.Point(419, 22);
            this.comboBox11.Name = "comboBox11";
            this.comboBox11.Size = new System.Drawing.Size(121, 150);
            this.comboBox11.TabIndex = 12;
            //
            // comboBox12
            //
            this.comboBox12.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBox12.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.comboBox12.FormattingEnabled = true;
            this.comboBox12.Items.AddRange(new object[] {
            "system",
            "simple",
            "aa",
            "bb",
            "cc",
            "d"});
            this.comboBox12.Location = new System.Drawing.Point(275, 22);
            this.comboBox12.Name = "comboBox12";
            this.comboBox12.Size = new System.Drawing.Size(121, 150);
            this.comboBox12.TabIndex = 11;
            //
            // dataBoundComboBox
            //
            this.dataBoundComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.dataBoundComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.dataBoundComboBox.Location = new System.Drawing.Point(836, 22);
            this.dataBoundComboBox.Name = "DataBoundComboBox";
            this.dataBoundComboBox.Size = new System.Drawing.Size(121, 150);
            this.dataBoundComboBox.TabIndex = 13;
            //
            // currentDPILabel1
            //
            this.currentDPILabel1.AutoSize = true;
            this.currentDPILabel1.Location = new System.Drawing.Point(18, 173);
            this.currentDPILabel1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.currentDPILabel1.Name = "currentDPILabel1";
            this.currentDPILabel1.Size = new System.Drawing.Size(90, 13);
            this.currentDPILabel1.TabIndex = 15;
            this.currentDPILabel1.Text = "Current scaling is 100%";
            //
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "label1";
            //
            // dataBoundLabel
            // 
            this.dataBoundLabel.AutoSize = true;
            this.dataBoundLabel.Location = new System.Drawing.Point(836, 6);
            this.dataBoundLabel.Name = "dataBoundLabel";
            this.dataBoundLabel.Size = new System.Drawing.Size(65, 13);
            this.dataBoundLabel.TabIndex = 16; 
            this.dataBoundLabel.Text = "data-bound";
            // 
            // ComboBoxes
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(975, 198);
            this.Controls.Add(this.currentDPILabel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataBoundLabel);
            this.Controls.Add(this.dataBoundComboBox);
            this.Controls.Add(this.comboBox9);
            this.Controls.Add(this.comboBox10);
            this.Controls.Add(this.comboBox11);
            this.Controls.Add(this.comboBox12);
            this.Controls.Add(this.comboBox8);
            this.Controls.Add(this.comboBox7);
            this.Controls.Add(this.comboBox6);
            this.Controls.Add(this.comboBox5);
            this.Controls.Add(this.comboBox4);
            this.Controls.Add(this.comboBox3);
            this.Controls.Add(this.comboBox2);
            this.Controls.Add(this.comboBox1);
            this.Name = "ComboBoxes";
            this.Text = "ComboBoxes";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ComboBox comboBox2;
        private System.Windows.Forms.ComboBox comboBox3;
        private System.Windows.Forms.ComboBox comboBox4;
        private System.Windows.Forms.ComboBox comboBox5;
        private System.Windows.Forms.ComboBox comboBox6;
        private System.Windows.Forms.ComboBox comboBox7;
        private System.Windows.Forms.ComboBox comboBox8;
        private System.Windows.Forms.ComboBox comboBox9;
        private System.Windows.Forms.ComboBox comboBox10;
        private System.Windows.Forms.ComboBox comboBox11;
        private System.Windows.Forms.ComboBox comboBox12;
        private System.Windows.Forms.ComboBox dataBoundComboBox;
        private CurrentDPILabel currentDPILabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label dataBoundLabel;
    }
}
