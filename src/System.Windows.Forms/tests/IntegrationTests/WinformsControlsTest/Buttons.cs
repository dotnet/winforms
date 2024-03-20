// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace WinFormsControlsTest;

[DesignerCategory("Default")]
public partial class Buttons : Form
{
    private readonly FlatStyle[] _styles =
    [
        FlatStyle.Flat,
        FlatStyle.Popup,
        FlatStyle.Standard,
        FlatStyle.System
    ];

    public Buttons()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        TableLayoutPanel table = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 70.0f));
        table.RowStyles.Add(new RowStyle(SizeType.Percent, 30.0f));
        Controls.Add(table);

        FlowLayoutPanel panel = new()
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown
        };
        table.Controls.Add(panel, column: 0, row: 0);

        RadioButton radioButton;
        foreach (FlatStyle style in _styles)
        {
            radioButton = new RadioButton
            {
                AutoSize = true,
                FlatStyle = style,
                Text = style.ToString(),
                Checked = true
            };

            panel.Controls.Add(radioButton);
        }

        panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown
        };
        table.Controls.Add(panel, column: 1, row: 0);

        CheckBox checkBox;
        foreach (FlatStyle style in _styles)
        {
            checkBox = new CheckBox
            {
                AutoSize = true,
                FlatStyle = style,
                Text = style.ToString(),
                Checked = true
            };

            panel.Controls.Add(checkBox);
        }

        panel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown
        };
        table.Controls.Add(panel, column: 2, row: 0);

        Button button;
        foreach (FlatStyle style in _styles)
        {
            button = new Button
            {
                AutoSize = true,
                FlatStyle = style,
                Text = style.ToString()
            };

            toolTip1.SetToolTip(button, $"{style}.");

            panel.Controls.Add(button);
        }

        table.Controls.Add(
            new Button
            {
                AutoSize = true,
                Image = SystemIcons.GetStockIcon(StockIconId.DesktopPC).ToBitmap()
            },
            column: 0,
            row: 1);

        table.Controls.Add(
            new Button
            {
                AutoSize = true,
                Image = Icon.ExtractIcon("regedit.exe", 0, 256).ToBitmap()
            },
            column: 1,
            row: 1);

        base.OnLoad(e);
    }
}
