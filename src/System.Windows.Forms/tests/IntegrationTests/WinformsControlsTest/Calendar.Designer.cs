// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class Calendar
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
        this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
        this.minDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
        this.setMinDateButton = new System.Windows.Forms.Button();
        this.label1 = new System.Windows.Forms.Label();
        this.label2 = new System.Windows.Forms.Label();
        this.setMaxDateButton = new System.Windows.Forms.Button();
        this.maxDateDateTimePicker = new System.Windows.Forms.DateTimePicker();
        this.label3 = new System.Windows.Forms.Label();
        this.daysOfWeekComboBox = new System.Windows.Forms.ComboBox();
        this.label4 = new System.Windows.Forms.Label();
        this.resetMinDateButton = new System.Windows.Forms.Button();
        this.resetMaxDateButton = new System.Windows.Forms.Button();
        this.showWeekNumbersCheckBox = new System.Windows.Forms.CheckBox();
        this.showTodayCheckBox = new System.Windows.Forms.CheckBox();
        this.label5 = new System.Windows.Forms.Label();
        this.currentDPILabel1 = new CurrentDPILabel();
        this.SuspendLayout();
        // 
        // monthCalendar1
        // 
        this.monthCalendar1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
        | System.Windows.Forms.AnchorStyles.Left)
        | System.Windows.Forms.AnchorStyles.Right)));
        this.monthCalendar1.AnnuallyBoldedDates = new System.DateTime[] {
    new System.DateTime(2021, 8, 1, 0, 0, 0, 0),
    new System.DateTime(2021, 8, 2, 0, 0, 0, 0)};
        this.monthCalendar1.BoldedDates = new System.DateTime[] {
    new System.DateTime(2021, 7, 3, 0, 0, 0, 0),
    new System.DateTime(2021, 7, 4, 0, 0, 0, 0)};
        this.monthCalendar1.Location = new System.Drawing.Point(18, 80);
        this.monthCalendar1.MonthlyBoldedDates = new System.DateTime[] {
    new System.DateTime(2021, 9, 5, 0, 0, 0, 0),
    new System.DateTime(2021, 9, 6, 0, 0, 0, 0)};
        this.monthCalendar1.Name = "monthCalendar1";
        this.monthCalendar1.TabIndex = 0;
        // 
        // minDateDateTimePicker
        // 
        this.minDateDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.minDateDateTimePicker.Location = new System.Drawing.Point(18, 334);
        this.minDateDateTimePicker.Name = "minDateDateTimePicker";
        this.minDateDateTimePicker.Size = new System.Drawing.Size(251, 27);
        this.minDateDateTimePicker.TabIndex = 1;
        // 
        // setMinDateButton
        // 
        this.setMinDateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.setMinDateButton.Location = new System.Drawing.Point(18, 367);
        this.setMinDateButton.Name = "setMinDateButton";
        this.setMinDateButton.Size = new System.Drawing.Size(123, 29);
        this.setMinDateButton.TabIndex = 2;
        this.setMinDateButton.Text = "Set MinDate";
        this.setMinDateButton.UseVisualStyleBackColor = true;
        this.setMinDateButton.Click += this.setMinDateButton_Click;
        // 
        // label1
        // 
        this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(18, 308);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(94, 20);
        this.label1.TabIndex = 3;
        this.label1.Text = "Set MinDate:";
        // 
        // label2
        // 
        this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(19, 422);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(97, 20);
        this.label2.TabIndex = 6;
        this.label2.Text = "Set MaxDate:";
        // 
        // setMaxDateButton
        // 
        this.setMaxDateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.setMaxDateButton.Location = new System.Drawing.Point(19, 481);
        this.setMaxDateButton.Name = "setMaxDateButton";
        this.setMaxDateButton.Size = new System.Drawing.Size(123, 29);
        this.setMaxDateButton.TabIndex = 5;
        this.setMaxDateButton.Text = "Set MaxDate";
        this.setMaxDateButton.UseVisualStyleBackColor = true;
        this.setMaxDateButton.Click += this.setMaxDateButton_Click;
        // 
        // maxDateDateTimePicker
        // 
        this.maxDateDateTimePicker.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.maxDateDateTimePicker.Location = new System.Drawing.Point(19, 448);
        this.maxDateDateTimePicker.Name = "maxDateDateTimePicker";
        this.maxDateDateTimePicker.Size = new System.Drawing.Size(250, 27);
        this.maxDateDateTimePicker.TabIndex = 4;
        // 
        // label3
        // 
        this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(301, 336);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(142, 20);
        this.label3.TabIndex = 7;
        this.label3.Text = "Set FirstDayOfWeek:";
        // 
        // daysOfWeekComboBox
        // 
        this.daysOfWeekComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.daysOfWeekComboBox.FormattingEnabled = true;
        this.daysOfWeekComboBox.Items.AddRange(new object[] {
        "Monday",
        "Tuesday",
        "Wednesday",
        "Thursday",
        "Friday",
        "Saturday",
        "Sunday",
        "Default"});
        this.daysOfWeekComboBox.Location = new System.Drawing.Point(301, 363);
        this.daysOfWeekComboBox.Name = "daysOfWeekComboBox";
        this.daysOfWeekComboBox.Size = new System.Drawing.Size(151, 28);
        this.daysOfWeekComboBox.TabIndex = 8;
        this.daysOfWeekComboBox.SelectedIndexChanged += this.daysOfWeekComboBox_SelectedIndexChanged;
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(18, 50);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(334, 20);
        this.label4.TabIndex = 9;
        this.label4.Text = "Change the form size to change the calendar size";
        // 
        // resetMinDateButton
        // 
        this.resetMinDateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.resetMinDateButton.Location = new System.Drawing.Point(146, 367);
        this.resetMinDateButton.Name = "resetMinDateButton";
        this.resetMinDateButton.Size = new System.Drawing.Size(123, 29);
        this.resetMinDateButton.TabIndex = 10;
        this.resetMinDateButton.Text = "Reset";
        this.resetMinDateButton.UseVisualStyleBackColor = true;
        this.resetMinDateButton.Click += this.resetMinDateButton_Click;
        // 
        // resetMaxDateButton
        // 
        this.resetMaxDateButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.resetMaxDateButton.Location = new System.Drawing.Point(146, 481);
        this.resetMaxDateButton.Name = "resetMaxDateButton";
        this.resetMaxDateButton.Size = new System.Drawing.Size(123, 29);
        this.resetMaxDateButton.TabIndex = 11;
        this.resetMaxDateButton.Text = "Reset";
        this.resetMaxDateButton.UseVisualStyleBackColor = true;
        this.resetMaxDateButton.Click += this.resetMaxDateButton_Click;
        // 
        // showWeekNumbersCheckBox
        //
        this.showWeekNumbersCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.showWeekNumbersCheckBox.AutoSize = true;
        this.showWeekNumbersCheckBox.Location = new System.Drawing.Point(301, 452);
        this.showWeekNumbersCheckBox.Name = "showWeekNumbersCheckBox";
        this.showWeekNumbersCheckBox.Size = new System.Drawing.Size(161, 24);
        this.showWeekNumbersCheckBox.TabIndex = 12;
        this.showWeekNumbersCheckBox.Text = "showWeekNumbers";
        this.showWeekNumbersCheckBox.UseVisualStyleBackColor = true;
        this.showWeekNumbersCheckBox.CheckedChanged += this.showWeekNumbersCheckBox_CheckedChanged;
        // 
        // showTodayCheckBox
        //
        this.showTodayCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.showTodayCheckBox.AutoSize = true;
        this.showTodayCheckBox.Location = new System.Drawing.Point(301, 484);
        this.showTodayCheckBox.Name = "showTodayCheckBox";
        this.showTodayCheckBox.Size = new System.Drawing.Size(105, 24);
        this.showTodayCheckBox.TabIndex = 13;
        this.showTodayCheckBox.Text = "showToday";
        this.showTodayCheckBox.UseVisualStyleBackColor = true;
        this.showTodayCheckBox.CheckedChanged += this.showTodayCheckBox_CheckedChanged;
        // 
        // label5
        // 
        this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(301, 422);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(137, 20);
        this.label5.TabIndex = 14;
        this.label5.Text = "Additional settings:";
        //
        // currentDPILabel1
        //
        this.currentDPILabel1.Location = new System.Drawing.Point(18, 18);
        this.currentDPILabel1.Name = "currentDPILabel1";
        this.currentDPILabel1.Size = new System.Drawing.Size(227, 23);
        this.currentDPILabel1.TabIndex = 1;
        this.currentDPILabel1.Text = "currentDPILabel1";
        // 
        // Form1
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(509, 528);
        this.Controls.Add(this.currentDPILabel1);
        this.Controls.Add(this.label5);
        this.Controls.Add(this.showTodayCheckBox);
        this.Controls.Add(this.showWeekNumbersCheckBox);
        this.Controls.Add(this.resetMaxDateButton);
        this.Controls.Add(this.resetMinDateButton);
        this.Controls.Add(this.label4);
        this.Controls.Add(this.daysOfWeekComboBox);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.setMaxDateButton);
        this.Controls.Add(this.maxDateDateTimePicker);
        this.Controls.Add(this.label1);
        this.Controls.Add(this.setMinDateButton);
        this.Controls.Add(this.minDateDateTimePicker);
        this.Controls.Add(this.monthCalendar1);
        this.MinimumSize = new System.Drawing.Size(527, 575);
        this.Name = "Form1";
        this.Text = "Form1";
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private System.Windows.Forms.MonthCalendar monthCalendar1;
    private System.Windows.Forms.DateTimePicker minDateDateTimePicker;
    private System.Windows.Forms.Button setMinDateButton;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Button setMaxDateButton;
    private System.Windows.Forms.DateTimePicker maxDateDateTimePicker;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.ComboBox daysOfWeekComboBox;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button resetMinDateButton;
    private System.Windows.Forms.Button resetMaxDateButton;
    private System.Windows.Forms.CheckBox showWeekNumbersCheckBox;
    private System.Windows.Forms.CheckBox showTodayCheckBox;
    private System.Windows.Forms.Label label5;
    private CurrentDPILabel currentDPILabel1;
}
