// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace WinFormsControlsTest;

partial class ChartControl
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
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
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        // chart1
        System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new();
        System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series1 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series2 = new();
        System.Windows.Forms.DataVisualization.Charting.Title title1 = new();

        // chart2
        System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series3 = new();
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint1 = new(36890D, 32D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint2 = new(36891D, 56D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint3 = new(36892D, 35D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint4 = new(36893D, 12D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint5 = new(36894D, 35D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint6 = new(36895D, 6D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint7 = new(36896D, 23D);

        System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series4 = new();
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint8 = new(36890D, 67D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint9 = new(36891D, 24D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint10 = new(36892D, 12D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint11 = new(36893D, 8D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint12 = new(36894D, 46D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint13 = new(36895D, 14D);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint14 = new(36896D, 76D);
        System.Windows.Forms.DataVisualization.Charting.Title title2 = new();

        // chart3
        System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series5 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series6 = new();
        System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new();
        System.Windows.Forms.DataVisualization.Charting.Title title3 = new();

        // chart4
        System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new();
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint15 = new(0, 39);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint16 = new(0, 18);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint17 = new(0, 15);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint18 = new(0, 12);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint19 = new(0, 8);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint20 = new(0, 4.5);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint21 = new(0, 3.2000000476837158);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint22 = new(0, 2);
        System.Windows.Forms.DataVisualization.Charting.DataPoint dataPoint23 = new(0, 1);
        System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new();
        System.Windows.Forms.DataVisualization.Charting.Series series7 = new();
        System.Windows.Forms.DataVisualization.Charting.Title title4 = new();

        this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
        this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
        this.chart3 = new System.Windows.Forms.DataVisualization.Charting.Chart();
        this.chart4 = new System.Windows.Forms.DataVisualization.Charting.Chart();
        ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.chart3)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.chart4)).BeginInit();
        this.SuspendLayout();
        // 
        // chart1
        // 
        this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
        this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        this.chart1.BackSecondaryColor = System.Drawing.Color.White;
        this.chart1.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
        this.chart1.BorderlineWidth = 2;
        this.chart1.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
        chartArea1.Area3DStyle.Inclination = 15;
        chartArea1.Area3DStyle.IsClustered = true;
        chartArea1.Area3DStyle.IsRightAngleAxes = false;
        chartArea1.Area3DStyle.Perspective = 10;
        chartArea1.Area3DStyle.Rotation = 10;
        chartArea1.Area3DStyle.WallWidth = 0;
        chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea1.AxisY.IsLabelAutoFit = false;
        chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea1.AxisY.MajorTickMark.Size = 0.6F;
        chartArea1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(165)))), ((int)(((byte)(191)))), ((int)(((byte)(228)))));
        chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        chartArea1.BackSecondaryColor = System.Drawing.Color.White;
        chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea1.Name = "Default";
        chartArea1.Position.Auto = false;
        chartArea1.Position.Height = 78F;
        chartArea1.Position.Width = 88F;
        chartArea1.Position.X = 5F;
        chartArea1.Position.Y = 15F;
        chartArea1.ShadowColor = System.Drawing.Color.Transparent;
        this.chart1.ChartAreas.Add(chartArea1);
        legend1.Alignment = System.Drawing.StringAlignment.Far;
        legend1.BackColor = System.Drawing.Color.Transparent;
        legend1.Enabled = false;
        legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        legend1.IsTextAutoFit = false;
        legend1.Name = "Default";
        legend1.Position.Auto = false;
        legend1.Position.Height = 14.23021F;
        legend1.Position.Width = 19.34047F;
        legend1.Position.X = 74.73474F;
        legend1.Position.Y = 74.08253F;
        this.chart1.Legends.Add(legend1);
        this.chart1.Location = new System.Drawing.Point(16, 64);
        this.chart1.Name = "chart1";
        series1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        series1.BorderWidth = 3;
        series1.ChartArea = "Default";
        series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Polar;
        series1.Legend = "Default";
        series1.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        series1.MarkerColor = System.Drawing.Color.LightSkyBlue;
        series1.MarkerSize = 7;
        series1.Name = "Series1";
        series1.ShadowOffset = 1;
        series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        series1.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        series2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        series2.BorderWidth = 3;
        series2.ChartArea = "Default";
        series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Polar;
        series2.Legend = "Default";
        series2.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        series2.MarkerColor = System.Drawing.Color.Gold;
        series2.MarkerSize = 7;
        series2.Name = "Series2";
        series2.ShadowOffset = 1;
        series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        series2.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        this.chart1.Series.Add(series1);
        this.chart1.Series.Add(series2);
        this.chart1.Size = new System.Drawing.Size(650, 400);
        this.chart1.TabIndex = 0;
        title1.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
        title1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        title1.Name = "Title1";
        title1.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        title1.ShadowOffset = 3;
        title1.Text = "Polar Chart";
        this.chart1.Titles.Add(title1);
        // 
        // chart2
        // 
        this.chart2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
        this.chart2.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        this.chart2.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
        this.chart2.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
        this.chart2.BorderlineWidth = 2;
        this.chart2.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
        chartArea2.Area3DStyle.Inclination = 15;
        chartArea2.Area3DStyle.IsClustered = true;
        chartArea2.Area3DStyle.IsRightAngleAxes = false;
        chartArea2.Area3DStyle.Perspective = 10;
        chartArea2.Area3DStyle.Rotation = 10;
        chartArea2.Area3DStyle.WallWidth = 0;
        chartArea2.AxisX.LabelAutoFitMaxFontSize = 8;
        chartArea2.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea2.AxisX.LabelStyle.Format = "MM-dd";
        chartArea2.AxisX.LabelStyle.IsEndLabelVisible = false;
        chartArea2.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea2.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea2.AxisY.LabelAutoFitMaxFontSize = 8;
        chartArea2.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea2.AxisY.LabelStyle.Format = "C0";
        chartArea2.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea2.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea2.BackColor = System.Drawing.Color.OldLace;
        chartArea2.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        chartArea2.BackSecondaryColor = System.Drawing.Color.White;
        chartArea2.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea2.Name = "Default";
        chartArea2.ShadowColor = System.Drawing.Color.Transparent;
        this.chart2.ChartAreas.Add(chartArea2);
        legend2.Name = "Default";
        legend2.Enabled = false;
        this.chart2.Legends.Add(legend2);
        this.chart2.Location = new System.Drawing.Point(752, 64);
        this.chart2.Name = "chart2";
        series3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        series3.ChartArea = "Default";
        series3.Legend = "Default";
        series3.Name = "Series3";
        series3.Points.Add(dataPoint1);
        series3.Points.Add(dataPoint2);
        series3.Points.Add(dataPoint3);
        series3.Points.Add(dataPoint4);
        series3.Points.Add(dataPoint5);
        series3.Points.Add(dataPoint6);
        series3.Points.Add(dataPoint7);
        series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
        series4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        series4.ChartArea = "Default";
        series4.Legend = "Default";

        series4.Name = "Series4";
        series4.Points.Add(dataPoint8);
        series4.Points.Add(dataPoint9);
        series4.Points.Add(dataPoint10);
        series4.Points.Add(dataPoint11);
        series4.Points.Add(dataPoint12);
        series4.Points.Add(dataPoint13);
        series4.Points.Add(dataPoint14);
        series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
        this.chart2.Series.Add(series3);
        this.chart2.Series.Add(series4);
        this.chart2.Size = new System.Drawing.Size(654, 400);
        this.chart2.TabIndex = 0;
        title2.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
        title2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        title2.Name = "Title2";
        title2.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        title2.ShadowOffset = 3;
        title2.Text = "Column Chart";
        this.chart2.Titles.Add(title2);
        // 
        // chart3
        // 
        this.chart3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
        this.chart3.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        this.chart3.BackSecondaryColor = System.Drawing.Color.White;
        this.chart3.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        this.chart3.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
        this.chart3.BorderlineWidth = 2;
        this.chart3.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
        chartArea3.Area3DStyle.Inclination = 15;
        chartArea3.Area3DStyle.IsClustered = true;
        chartArea3.Area3DStyle.IsRightAngleAxes = false;
        chartArea3.Area3DStyle.Perspective = 10;
        chartArea3.Area3DStyle.Rotation = 10;
        chartArea3.Area3DStyle.WallWidth = 0;
        chartArea3.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea3.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea3.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(165)))), ((int)(((byte)(191)))), ((int)(((byte)(228)))));
        chartArea3.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        chartArea3.BackSecondaryColor = System.Drawing.Color.White;
        chartArea3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea3.Name = "Default";
        chartArea3.ShadowColor = System.Drawing.Color.Transparent;
        this.chart3.ChartAreas.Add(chartArea3);
        legend3.BackColor = System.Drawing.Color.Transparent;
        legend3.Enabled = false;
        legend3.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        legend3.IsTextAutoFit = false;
        legend3.Name = "Default";
        this.chart3.Legends.Add(legend3);
        this.chart3.Location = new System.Drawing.Point(16, 522);
        this.chart3.Name = "chart3";
        series5.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        series5.ChartArea = "Default";
        series5.Legend = "Default";
        series5.Name = "DataSeries";
        series6.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        series6.ChartArea = "Default";
        series6.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.ErrorBar;
        series6.Color = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(180)))), ((int)(((byte)(65)))));
        series6.Legend = "Default";
        series6.MarkerBorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        series6.MarkerSize = 6;
        series6.Name = "ErrorBar";
        series6.ShadowOffset = 1;
        series6.YValuesPerPoint = 3;
        this.chart3.Series.Add(series5);
        this.chart3.Series.Add(series6);
        this.chart3.Size = new System.Drawing.Size(650, 438);
        this.chart3.TabIndex = 1;
        title3.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
        title3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        title3.Name = "Title2";
        title3.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        title3.ShadowOffset = 3;
        title3.Text = "ErrorBar Chart";
        this.chart3.Titles.Add(title3);
        // 
        // chart4
        // 
        this.chart4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
        this.chart4.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        this.chart4.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        this.chart4.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
        this.chart4.BorderlineWidth = 2;
        this.chart4.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
        chartArea4.Area3DStyle.IsClustered = true;
        chartArea4.Area3DStyle.IsRightAngleAxes = false;
        chartArea4.Area3DStyle.PointGapDepth = 900;
        chartArea4.Area3DStyle.Rotation = 162;
        chartArea4.Area3DStyle.WallWidth = 25;
        chartArea4.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea4.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea4.AxisX.MajorGrid.Enabled = false;
        chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea4.AxisX.MajorTickMark.Enabled = false;
        chartArea4.AxisX2.MajorGrid.Enabled = false;
        chartArea4.AxisX2.MajorTickMark.Enabled = false;
        chartArea4.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        chartArea4.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea4.AxisY.MajorGrid.Enabled = false;
        chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea4.AxisY.MajorTickMark.Enabled = false;
        chartArea4.AxisY2.MajorGrid.Enabled = false;
        chartArea4.AxisY2.MajorTickMark.Enabled = false;
        chartArea4.BackColor = System.Drawing.Color.Transparent;
        chartArea4.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
        chartArea4.BackSecondaryColor = System.Drawing.Color.Transparent;
        chartArea4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        chartArea4.Name = "ChartArea4";
        chartArea4.ShadowColor = System.Drawing.Color.Transparent;
        this.chart4.ChartAreas.Add(chartArea4);
        this.chart4.IsSoftShadows = false;
        legend4.BackColor = System.Drawing.Color.Transparent;
        legend4.Font = new System.Drawing.Font("Trebuchet MS", 8F, System.Drawing.FontStyle.Bold);
        legend4.IsEquallySpacedItems = true;
        legend4.IsTextAutoFit = false;
        legend4.Name = "Default";
        this.chart4.Legends.Add(legend4);
        this.chart4.Location = new System.Drawing.Point(752, 522);
        this.chart4.Name = "chart4";
        series7.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
        series7.ChartArea = "ChartArea4";
        series7.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Pie;
        series7.Color = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(65)))), ((int)(((byte)(140)))), ((int)(((byte)(240)))));
        series7.CustomProperties = "DoughnutRadius=25, PieDrawingStyle=Concave, CollectedLabel=Other, MinimumRelative" +
            "PieSize=20";
        series7.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
        series7.Label = "#PERCENT{P1}";
        series7.Legend = "Default";
        series7.MarkerStyle = System.Windows.Forms.DataVisualization.Charting.MarkerStyle.Circle;
        series7.Name = "Series1";
        dataPoint15.CustomProperties = "OriginalPointIndex=0";
        dataPoint15.IsValueShownAsLabel = false;
        dataPoint15.LegendText = "RUS";
        dataPoint16.CustomProperties = "OriginalPointIndex=1";
        dataPoint16.IsValueShownAsLabel = false;
        dataPoint16.LegendText = "CAN";
        dataPoint17.CustomProperties = "OriginalPointIndex=2";
        dataPoint17.IsValueShownAsLabel = false;
        dataPoint17.LegendText = "USA";
        dataPoint18.CustomProperties = "OriginalPointIndex=3";
        dataPoint18.LegendText = "PRC";
        dataPoint19.CustomProperties = "OriginalPointIndex=5";
        dataPoint19.LegendText = "DEN";
        dataPoint20.LegendText = "AUS";
        dataPoint21.CustomProperties = "OriginalPointIndex=4";
        dataPoint21.LegendText = "IND";
        dataPoint22.LegendText = "ARG";
        dataPoint23.LegendText = "FRA";
        series7.Points.Add(dataPoint15);
        series7.Points.Add(dataPoint16);
        series7.Points.Add(dataPoint17);
        series7.Points.Add(dataPoint18);
        series7.Points.Add(dataPoint19);
        series7.Points.Add(dataPoint20);
        series7.Points.Add(dataPoint21);
        series7.Points.Add(dataPoint22);
        series7.Points.Add(dataPoint23);
        series7.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        series7.YValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.Double;
        this.chart4.Series.Add(series7);
        this.chart4.Size = new System.Drawing.Size(654, 438);
        this.chart4.TabIndex = 1;
        title4.Font = new System.Drawing.Font("Trebuchet MS", 14.25F, System.Drawing.FontStyle.Bold);
        title4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
        title4.Name = "Title2";
        title4.ShadowColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
        title4.ShadowOffset = 3;
        title4.Text = "Pie Chart";
        this.chart4.Titles.Add(title4);

        // 
        // ChartControl
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1535, 1017);
        this.Controls.Add(this.chart4);
        this.Controls.Add(this.chart3);
        this.Controls.Add(this.chart2);
        this.Controls.Add(this.chart1);
        this.Name = "ChartControl";
        this.Text = "Form1";
        this.Load += this.ChartControl_Load;
        ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.chart3)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.chart4)).EndInit();
        this.ResumeLayout(false);

    }
    #endregion

    private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
    private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
    private System.Windows.Forms.DataVisualization.Charting.Chart chart3;
    private System.Windows.Forms.DataVisualization.Charting.Chart chart4;
}

