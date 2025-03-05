// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace WinFormsControlsTest;

[DesignerCategory("default")]
public partial class ChartControl : Form
{
    public ChartControl()
    {
        InitializeComponent();
    }

    private void ChartControl_Load(object sender, EventArgs e)
    {
        // Fill series data for chart1
        for (double angle = 0.0; angle <= 360.0; angle += 10.0)
        {
            double yValue = (1.0 + Math.Sin(angle / 180.0 * Math.PI)) * 10.0;
            chart1.Series["Series1"].Points.AddXY(angle + 90.0, yValue);
            chart1.Series["Series2"].Points.AddXY(angle + 270.0, yValue);
        }

        // Fill series data for chart2
        chart2.ChartAreas[0].AxisX.LabelStyle.IntervalOffset = 1;
        chart2.ChartAreas[0].AxisX.LabelStyle.IntervalOffsetType = DateTimeIntervalType.Days;
        chart2.ChartAreas[0].AxisX.LabelStyle.Interval = 2;
        chart2.ChartAreas[0].AxisX.LabelStyle.IntervalType = DateTimeIntervalType.Days;

        // Fill series data for chart3
        double[] yValues = [32.4, 56.9, 89.7, 80.5, 59.3, 33.8, 78.8, 44.6, 76.4, 68.9];
        chart3.Series["DataSeries"].Points.DataBindY(yValues);
        // Link error bar series with data series
        chart3.Series["ErrorBar"]["ErrorBarSeries"] = "DataSeries";

        // Fill series data for chart4
        chart4.Series[0].Font = new Font("Trebuchet MS", 8, FontStyle.Bold);
        chart4.Series[0]["CollectedToolTip"] = "Other";
    }
}
