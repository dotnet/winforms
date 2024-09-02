// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class ComboBoxesWithScrollBars
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
        this.components = new System.ComponentModel.Container();
        this.comboBox1 = new System.Windows.Forms.ComboBox();
        this.comboBox2 = new System.Windows.Forms.ComboBox();
        this.comboBox3 = new System.Windows.Forms.ComboBox();
        this.changeHeightGroupBox1 = new System.Windows.Forms.GroupBox();
        this.label1 = new System.Windows.Forms.Label();
        this.changeDDH_UpDown1 = new System.Windows.Forms.NumericUpDown();
        this.label2 = new System.Windows.Forms.Label();
        this.changeCBHeight_UpDown2 = new System.Windows.Forms.NumericUpDown();
        this.label3 = new System.Windows.Forms.Label();
        this.changeDDH_UpDown3 = new System.Windows.Forms.NumericUpDown();
        this.changeMaxDropDownItemsGroupBox2 = new System.Windows.Forms.GroupBox();
        this.maxDropDownItemsUpDown1 = new System.Windows.Forms.NumericUpDown();
        this.maxDropDownItemsUpDown2 = new System.Windows.Forms.NumericUpDown();
        this.maxDropDownItemsUpDown3 = new System.Windows.Forms.NumericUpDown();
        this.changeIntegralHeightGroupBox3 = new System.Windows.Forms.GroupBox();
        this.integralHeightCheckBox1 = new System.Windows.Forms.CheckBox();
        this.integralHeightCheckBox2 = new System.Windows.Forms.CheckBox();
        this.integralHeightCheckBox3 = new System.Windows.Forms.CheckBox();
        this.changeHeightsStyleGroupBox4 = new System.Windows.Forms.GroupBox();
        this.useDifferentHeightsCheckBox1 = new System.Windows.Forms.CheckBox();
        this.useDifferentHeightsCheckBox2 = new System.Windows.Forms.CheckBox();
        this.useDifferentHeightsCheckBox3 = new System.Windows.Forms.CheckBox();
        this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
        this.changeHeightGroupBox1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.changeDDH_UpDown1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.changeCBHeight_UpDown2)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.changeDDH_UpDown3)).BeginInit();
        this.changeMaxDropDownItemsGroupBox2.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.maxDropDownItemsUpDown1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.maxDropDownItemsUpDown2)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.maxDropDownItemsUpDown3)).BeginInit();
        this.changeIntegralHeightGroupBox3.SuspendLayout();
        this.changeHeightsStyleGroupBox4.SuspendLayout();
        this.SuspendLayout();
        // 
        // comboBox1
        // 
        this.comboBox1.FormattingEnabled = true;
        this.comboBox1.IntegralHeight = false;
        this.comboBox1.Location = new System.Drawing.Point(37, 348);
        this.comboBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.comboBox1.Name = "comboBox1";
        this.comboBox1.Size = new System.Drawing.Size(116, 23);
        this.comboBox1.TabIndex = 0;
        this.comboBox1.DrawItem += this.comboBox_DrawItem;
        // 
        // comboBox2
        // 
        this.comboBox2.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
        this.comboBox2.FormattingEnabled = true;
        this.comboBox2.IntegralHeight = false;
        this.comboBox2.Location = new System.Drawing.Point(206, 348);
        this.comboBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.comboBox2.Name = "comboBox2";
        this.comboBox2.Size = new System.Drawing.Size(116, 155);
        this.comboBox2.TabIndex = 1;
        this.comboBox2.DrawItem += this.comboBox_DrawItem;
        // 
        // comboBox3
        // 
        this.comboBox3.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.comboBox3.FormattingEnabled = true;
        this.comboBox3.IntegralHeight = false;
        this.comboBox3.Location = new System.Drawing.Point(370, 348);
        this.comboBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.comboBox3.Name = "comboBox3";
        this.comboBox3.Size = new System.Drawing.Size(116, 23);
        this.comboBox3.TabIndex = 2;
        this.comboBox3.DrawItem += this.comboBox_DrawItem;
        // 
        // changeHeightGroupBox1
        // 
        this.changeHeightGroupBox1.Controls.Add(this.label1);
        this.changeHeightGroupBox1.Controls.Add(this.changeDDH_UpDown1);
        this.changeHeightGroupBox1.Controls.Add(this.label2);
        this.changeHeightGroupBox1.Controls.Add(this.changeCBHeight_UpDown2);
        this.changeHeightGroupBox1.Controls.Add(this.label3);
        this.changeHeightGroupBox1.Controls.Add(this.changeDDH_UpDown3);
        this.changeHeightGroupBox1.Location = new System.Drawing.Point(14, 14);
        this.changeHeightGroupBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeHeightGroupBox1.Name = "changeHeightGroupBox1";
        this.changeHeightGroupBox1.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeHeightGroupBox1.Size = new System.Drawing.Size(502, 100);
        this.changeHeightGroupBox1.TabIndex = 3;
        this.changeHeightGroupBox1.TabStop = false;
        this.changeHeightGroupBox1.Text = "Change a height";
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(23, 28);
        this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(100, 30);
        this.label1.TabIndex = 0;
        this.label1.Text = "Change \r\nDropDownHeight";
        // 
        // changeDDH_UpDown1
        // 
        this.changeDDH_UpDown1.Location = new System.Drawing.Point(23, 61);
        this.changeDDH_UpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeDDH_UpDown1.Maximum = new decimal(new int[] {
        1000,
        0,
        0,
        0});
        this.changeDDH_UpDown1.Name = "changeDDH_UpDown1";
        this.changeDDH_UpDown1.Size = new System.Drawing.Size(117, 23);
        this.changeDDH_UpDown1.TabIndex = 0;
        this.toolTip1.SetToolTip(this.changeDDH_UpDown1, "Sets IntegralHeight to false. MaxDropDownItems won\'t be available after it settin" +
    "g");
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(192, 28);
        this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(106, 30);
        this.label2.TabIndex = 1;
        this.label2.Text = "Change \r\nComboBox Height";
        // 
        // changeCBHeight_UpDown2
        // 
        this.changeCBHeight_UpDown2.Location = new System.Drawing.Point(192, 61);
        this.changeCBHeight_UpDown2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeCBHeight_UpDown2.Maximum = new decimal(new int[] {
        1000,
        0,
        0,
        0});
        this.changeCBHeight_UpDown2.Name = "changeCBHeight_UpDown2";
        this.changeCBHeight_UpDown2.Size = new System.Drawing.Size(117, 23);
        this.changeCBHeight_UpDown2.TabIndex = 1;
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(356, 28);
        this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(100, 30);
        this.label3.TabIndex = 2;
        this.label3.Text = "Change \r\nDropDownHeight";
        // 
        // changeDDH_UpDown3
        // 
        this.changeDDH_UpDown3.Location = new System.Drawing.Point(356, 61);
        this.changeDDH_UpDown3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeDDH_UpDown3.Maximum = new decimal(new int[] {
        1000,
        0,
        0,
        0});
        this.changeDDH_UpDown3.Name = "changeDDH_UpDown3";
        this.changeDDH_UpDown3.Size = new System.Drawing.Size(117, 23);
        this.changeDDH_UpDown3.TabIndex = 2;
        this.toolTip1.SetToolTip(this.changeDDH_UpDown3, "Sets IntegralHeight to false. MaxDropDownItems won\'t be available after it settin" +
    "g");
        // 
        // changeMaxDropDownItemsGroupBox2
        // 
        this.changeMaxDropDownItemsGroupBox2.Controls.Add(this.maxDropDownItemsUpDown1);
        this.changeMaxDropDownItemsGroupBox2.Controls.Add(this.maxDropDownItemsUpDown2);
        this.changeMaxDropDownItemsGroupBox2.Controls.Add(this.maxDropDownItemsUpDown3);
        this.changeMaxDropDownItemsGroupBox2.Location = new System.Drawing.Point(14, 121);
        this.changeMaxDropDownItemsGroupBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeMaxDropDownItemsGroupBox2.Name = "changeMaxDropDownItemsGroupBox2";
        this.changeMaxDropDownItemsGroupBox2.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeMaxDropDownItemsGroupBox2.Size = new System.Drawing.Size(502, 63);
        this.changeMaxDropDownItemsGroupBox2.TabIndex = 4;
        this.changeMaxDropDownItemsGroupBox2.TabStop = false;
        this.changeMaxDropDownItemsGroupBox2.Text = "Change MaxDropDownItems value";
        // 
        // maxDropDownItemsUpDown1
        // 
        this.maxDropDownItemsUpDown1.Location = new System.Drawing.Point(23, 22);
        this.maxDropDownItemsUpDown1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.maxDropDownItemsUpDown1.Maximum = new decimal(new int[] {
        40,
        0,
        0,
        0});
        this.maxDropDownItemsUpDown1.Name = "maxDropDownItemsUpDown1";
        this.maxDropDownItemsUpDown1.Size = new System.Drawing.Size(117, 23);
        this.maxDropDownItemsUpDown1.TabIndex = 0;
        // 
        // maxDropDownItemsUpDown2
        // 
        this.maxDropDownItemsUpDown2.Location = new System.Drawing.Point(192, 22);
        this.maxDropDownItemsUpDown2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.maxDropDownItemsUpDown2.Maximum = new decimal(new int[] {
        40,
        0,
        0,
        0});
        this.maxDropDownItemsUpDown2.Name = "maxDropDownItemsUpDown2";
        this.maxDropDownItemsUpDown2.Size = new System.Drawing.Size(117, 23);
        this.maxDropDownItemsUpDown2.TabIndex = 1;
        this.toolTip1.SetToolTip(this.maxDropDownItemsUpDown2, "It doesn\'t affect the combobox with the Simple dropdown style");
        // 
        // maxDropDownItemsUpDown3
        // 
        this.maxDropDownItemsUpDown3.Location = new System.Drawing.Point(356, 22);
        this.maxDropDownItemsUpDown3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.maxDropDownItemsUpDown3.Maximum = new decimal(new int[] {
        40,
        0,
        0,
        0});
        this.maxDropDownItemsUpDown3.Name = "maxDropDownItemsUpDown3";
        this.maxDropDownItemsUpDown3.Size = new System.Drawing.Size(117, 23);
        this.maxDropDownItemsUpDown3.TabIndex = 2;
        // 
        // changeIntegralHeightGroupBox3
        // 
        this.changeIntegralHeightGroupBox3.Controls.Add(this.integralHeightCheckBox1);
        this.changeIntegralHeightGroupBox3.Controls.Add(this.integralHeightCheckBox2);
        this.changeIntegralHeightGroupBox3.Controls.Add(this.integralHeightCheckBox3);
        this.changeIntegralHeightGroupBox3.Location = new System.Drawing.Point(14, 192);
        this.changeIntegralHeightGroupBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeIntegralHeightGroupBox3.Name = "changeIntegralHeightGroupBox3";
        this.changeIntegralHeightGroupBox3.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeIntegralHeightGroupBox3.Size = new System.Drawing.Size(502, 63);
        this.changeIntegralHeightGroupBox3.TabIndex = 5;
        this.changeIntegralHeightGroupBox3.TabStop = false;
        this.changeIntegralHeightGroupBox3.Text = "Change IntegralHeight value";
        // 
        // integralHeightCheckBox1
        // 
        this.integralHeightCheckBox1.AutoSize = true;
        this.integralHeightCheckBox1.Location = new System.Drawing.Point(23, 33);
        this.integralHeightCheckBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.integralHeightCheckBox1.Name = "integralHeightCheckBox1";
        this.integralHeightCheckBox1.Size = new System.Drawing.Size(102, 19);
        this.integralHeightCheckBox1.TabIndex = 0;
        this.integralHeightCheckBox1.Text = "IntegralHeight";
        this.integralHeightCheckBox1.UseVisualStyleBackColor = true;
        // 
        // integralHeightCheckBox2
        // 
        this.integralHeightCheckBox2.AutoSize = true;
        this.integralHeightCheckBox2.Location = new System.Drawing.Point(192, 33);
        this.integralHeightCheckBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.integralHeightCheckBox2.Name = "integralHeightCheckBox2";
        this.integralHeightCheckBox2.Size = new System.Drawing.Size(102, 19);
        this.integralHeightCheckBox2.TabIndex = 1;
        this.integralHeightCheckBox2.Text = "IntegralHeight";
        this.integralHeightCheckBox2.UseVisualStyleBackColor = true;
        // 
        // integralHeightCheckBox3
        // 
        this.integralHeightCheckBox3.AutoSize = true;
        this.integralHeightCheckBox3.Location = new System.Drawing.Point(356, 33);
        this.integralHeightCheckBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.integralHeightCheckBox3.Name = "integralHeightCheckBox3";
        this.integralHeightCheckBox3.Size = new System.Drawing.Size(102, 19);
        this.integralHeightCheckBox3.TabIndex = 2;
        this.integralHeightCheckBox3.Text = "IntegralHeight";
        this.integralHeightCheckBox3.UseVisualStyleBackColor = true;
        // 
        // changeHeightsStyleGroupBox4
        // 
        this.changeHeightsStyleGroupBox4.Controls.Add(this.useDifferentHeightsCheckBox1);
        this.changeHeightsStyleGroupBox4.Controls.Add(this.useDifferentHeightsCheckBox2);
        this.changeHeightsStyleGroupBox4.Controls.Add(this.useDifferentHeightsCheckBox3);
        this.changeHeightsStyleGroupBox4.Location = new System.Drawing.Point(14, 262);
        this.changeHeightsStyleGroupBox4.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeHeightsStyleGroupBox4.Name = "changeHeightsStyleGroupBox4";
        this.changeHeightsStyleGroupBox4.Padding = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.changeHeightsStyleGroupBox4.Size = new System.Drawing.Size(502, 63);
        this.changeHeightsStyleGroupBox4.TabIndex = 6;
        this.changeHeightsStyleGroupBox4.TabStop = false;
        this.changeHeightsStyleGroupBox4.Text = "Change items heights style";
        // 
        // useDifferentHeightsCheckBox1
        // 
        this.useDifferentHeightsCheckBox1.AutoSize = true;
        this.useDifferentHeightsCheckBox1.Location = new System.Drawing.Point(23, 33);
        this.useDifferentHeightsCheckBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.useDifferentHeightsCheckBox1.Name = "useDifferentHeightsCheckBox1";
        this.useDifferentHeightsCheckBox1.Size = new System.Drawing.Size(135, 19);
        this.useDifferentHeightsCheckBox1.TabIndex = 0;
        this.useDifferentHeightsCheckBox1.Text = "Use different heights";
        this.useDifferentHeightsCheckBox1.UseVisualStyleBackColor = true;
        // 
        // useDifferentHeightsCheckBox2
        // 
        this.useDifferentHeightsCheckBox2.AutoSize = true;
        this.useDifferentHeightsCheckBox2.Location = new System.Drawing.Point(192, 33);
        this.useDifferentHeightsCheckBox2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.useDifferentHeightsCheckBox2.Name = "useDifferentHeightsCheckBox2";
        this.useDifferentHeightsCheckBox2.Size = new System.Drawing.Size(135, 19);
        this.useDifferentHeightsCheckBox2.TabIndex = 1;
        this.useDifferentHeightsCheckBox2.Text = "Use different heights";
        this.useDifferentHeightsCheckBox2.UseVisualStyleBackColor = true;
        // 
        // useDifferentHeightsCheckBox3
        // 
        this.useDifferentHeightsCheckBox3.AutoSize = true;
        this.useDifferentHeightsCheckBox3.Location = new System.Drawing.Point(356, 33);
        this.useDifferentHeightsCheckBox3.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.useDifferentHeightsCheckBox3.Name = "useDifferentHeightsCheckBox3";
        this.useDifferentHeightsCheckBox3.Size = new System.Drawing.Size(135, 19);
        this.useDifferentHeightsCheckBox3.TabIndex = 2;
        this.useDifferentHeightsCheckBox3.Text = "Use different heights";
        this.toolTip1.SetToolTip(this.useDifferentHeightsCheckBox3, "Changes DropDownList dropdown style to DropDown in view.\nIt is impossible to retu" +
    "rn DropDownList style for the combobox");
        this.useDifferentHeightsCheckBox3.UseVisualStyleBackColor = true;
        // 
        // ComboBoxesWithScrollBars
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(531, 517);
        this.Controls.Add(this.changeHeightGroupBox1);
        this.Controls.Add(this.changeMaxDropDownItemsGroupBox2);
        this.Controls.Add(this.changeIntegralHeightGroupBox3);
        this.Controls.Add(this.changeHeightsStyleGroupBox4);
        this.Controls.Add(this.comboBox1);
        this.Controls.Add(this.comboBox2);
        this.Controls.Add(this.comboBox3);
        this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
        this.Name = "ComboBoxesWithScrollBars";
        this.Text = "ComboBoxes with ScrollBars";
        this.changeHeightGroupBox1.ResumeLayout(false);
        this.changeHeightGroupBox1.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)(this.changeDDH_UpDown1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.changeCBHeight_UpDown2)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.changeDDH_UpDown3)).EndInit();
        this.changeMaxDropDownItemsGroupBox2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.maxDropDownItemsUpDown1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.maxDropDownItemsUpDown2)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.maxDropDownItemsUpDown3)).EndInit();
        this.changeIntegralHeightGroupBox3.ResumeLayout(false);
        this.changeIntegralHeightGroupBox3.PerformLayout();
        this.changeHeightsStyleGroupBox4.ResumeLayout(false);
        this.changeHeightsStyleGroupBox4.PerformLayout();
        this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.ComboBox comboBox1;
    private System.Windows.Forms.ComboBox comboBox2;
    private System.Windows.Forms.ComboBox comboBox3;
    private System.Windows.Forms.GroupBox changeHeightGroupBox1;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.NumericUpDown changeDDH_UpDown1;
    private System.Windows.Forms.NumericUpDown changeCBHeight_UpDown2;
    private System.Windows.Forms.NumericUpDown changeDDH_UpDown3;
    private System.Windows.Forms.GroupBox changeMaxDropDownItemsGroupBox2;
    private System.Windows.Forms.NumericUpDown maxDropDownItemsUpDown1;
    private System.Windows.Forms.NumericUpDown maxDropDownItemsUpDown2;
    private System.Windows.Forms.NumericUpDown maxDropDownItemsUpDown3;
    private System.Windows.Forms.GroupBox changeIntegralHeightGroupBox3;
    private System.Windows.Forms.CheckBox integralHeightCheckBox2;
    private System.Windows.Forms.CheckBox integralHeightCheckBox1;
    private System.Windows.Forms.CheckBox integralHeightCheckBox3;
    private System.Windows.Forms.GroupBox changeHeightsStyleGroupBox4;
    private System.Windows.Forms.CheckBox useDifferentHeightsCheckBox3;
    private System.Windows.Forms.CheckBox useDifferentHeightsCheckBox2;
    private System.Windows.Forms.CheckBox useDifferentHeightsCheckBox1;
    private System.Windows.Forms.ToolTip toolTip1;
}

