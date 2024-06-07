// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static System.Windows.Forms.DateTimePicker;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class Calendar : Form
{
    public Calendar()
    {
        InitializeComponent();
        daysOfWeekComboBox.SelectedIndex = (int)monthCalendar1.FirstDayOfWeek;
        showWeekNumbersCheckBox.Checked = monthCalendar1.ShowWeekNumbers;
        showTodayCheckBox.Checked = monthCalendar1.ShowToday;
        monthCalendar1.DateSelected += monthCalendar1_DateSelected;
    }

    private unsafe void monthCalendar1_DateSelected(object sender, DateRangeEventArgs e)
    {
        if (monthCalendar1.BoldedDates.Contains(e.Start))
        {
            monthCalendar1.RemoveBoldedDate(e.Start);
        }
        else
        {
            monthCalendar1.AddBoldedDate(e.Start);
        }

        monthCalendar1.UpdateBoldedDates();
    }

    private void setMinDateButton_Click(object sender, EventArgs e)
    {
        if (minDateDateTimePicker.Value > monthCalendar1.MaxDate)
        {
            MessageBox.Show("MinDate should be less then MaxDate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        monthCalendar1.MinDate = minDateDateTimePicker.Value;
    }

    private void setMaxDateButton_Click(object sender, EventArgs e)
    {
        if (maxDateDateTimePicker.Value < monthCalendar1.MinDate)
        {
            MessageBox.Show("MaxDate should be grater then MinDate", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        monthCalendar1.MaxDate = maxDateDateTimePicker.Value;
    }

    private void daysOfWeekComboBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        monthCalendar1.FirstDayOfWeek = (Day)daysOfWeekComboBox.SelectedIndex;
    }

    private void resetMinDateButton_Click(object sender, EventArgs e)
    {
        monthCalendar1.MinDate = MinimumDateTime;
    }

    private void resetMaxDateButton_Click(object sender, EventArgs e)
    {
        monthCalendar1.MaxDate = MaximumDateTime;
    }

    private void showWeekNumbersCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        monthCalendar1.ShowWeekNumbers = showWeekNumbersCheckBox.Checked;
    }

    private void showTodayCheckBox_CheckedChanged(object sender, EventArgs e)
    {
        monthCalendar1.ShowToday = showTodayCheckBox.Checked;
    }
}
