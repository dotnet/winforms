// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;

namespace WinFormsControlsTest;

/// <summary>
///  Exploratory-testing harness for the conservative and modern (.NET 11 VisualStyles) button renderers.
///  Toggle the "Modern visual styles" check box to flip every sample button between
///  <see cref="VisualStylesMode.Classic"/> and <see cref="VisualStylesMode.Net11"/> at runtime.
/// </summary>
/// <remarks>
///  <para>
///   The application-wide color mode (Classic/Dark) is a start-up, set-once setting, so to evaluate the dark
///   palette the host application must be started in dark mode. The modern vs. conservative look, however, is
///   driven by the per-control <see cref="VisualStylesMode"/> ambient property and can be toggled live here.
///  </para>
/// </remarks>
[DesignerCategory("Default")]
public sealed class VisualStylesButtons : Form
{
    private static readonly FlatStyle[] s_styles =
    [
        FlatStyle.Standard,
        FlatStyle.Flat,
        FlatStyle.Popup,
        FlatStyle.System
    ];

    private readonly List<Button> _sampleButtons = [];
    private readonly List<CheckBox> _sampleToggles = [];
    private readonly CheckBox _modernToggle;

    public VisualStylesButtons()
    {
        Text = "VisualStyles Buttons (exploratory)";
        Size = new Size(900, 640);
        Padding = new Padding(8);

        _modernToggle = new CheckBox
        {
            Text = "Modern visual styles (.NET 11)",
            AutoSize = true,
            Dock = DockStyle.Top,
            Padding = new Padding(4)
        };
        _modernToggle.CheckedChanged += OnModernToggleChanged;
    }

    protected override void OnLoad(EventArgs e)
    {
        TableLayoutPanel table = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = s_styles.Length + 1,
            AutoScroll = true
        };

        string[] scenarios =
        [
            "Normal",
            "Default (Accept)",
            "Disabled",
            "With Image",
            "With BackgroundImage",
            "FlatAppearance",
            "AutoSize + AutoEllipsis"
        ];

        table.RowCount = scenarios.Length + 1;

        // Header row.
        table.Controls.Add(new Label { Text = "Scenario \\ FlatStyle", AutoSize = true, Font = new Font(Font, FontStyle.Bold) }, 0, 0);
        for (int c = 0; c < s_styles.Length; c++)
        {
            table.Controls.Add(
                new Label { Text = s_styles[c].ToString(), AutoSize = true, Font = new Font(Font, FontStyle.Bold) },
                c + 1,
                0);
        }

        Bitmap image = SystemIcons.GetStockIcon(StockIconId.DesktopPC).ToBitmap();

        for (int r = 0; r < scenarios.Length; r++)
        {
            table.Controls.Add(new Label { Text = scenarios[r], AutoSize = true }, 0, r + 1);

            for (int c = 0; c < s_styles.Length; c++)
            {
                Button button = CreateSampleButton(scenarios[r], s_styles[c], image);
                _sampleButtons.Add(button);
                table.Controls.Add(button, c + 1, r + 1);

                // Use the first "Default (Accept)" Standard button as the form's accept button.
                if (scenarios[r].StartsWith("Default", StringComparison.Ordinal) && s_styles[c] == FlatStyle.Standard)
                {
                    AcceptButton = button;
                }
            }
        }

        Controls.Add(table);
        Controls.Add(BuildToggleSwitchPanel());
        Controls.Add(_modernToggle);

        base.OnLoad(e);
    }

    private FlowLayoutPanel BuildToggleSwitchPanel()
    {
        FlowLayoutPanel panel = new()
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(4)
        };

        panel.Controls.Add(new Label { Text = "Toggle switches:", AutoSize = true, Padding = new Padding(0, 6, 8, 0) });

        foreach (string caption in new[] { "Wi\u2011Fi", "Bluetooth (off)", "Airplane mode" })
        {
            CheckBox toggle = new()
            {
                Appearance = Appearance.ToggleSwitch,
                AutoSize = true,
                Text = caption,
                Checked = !caption.Contains("off", StringComparison.OrdinalIgnoreCase),
                Margin = new Padding(8, 4, 8, 4)
            };

            _sampleToggles.Add(toggle);
            panel.Controls.Add(toggle);
        }

        return panel;
    }

    private Button CreateSampleButton(string scenario, FlatStyle style, Bitmap image)
    {
        Button button = new()
        {
            FlatStyle = style,
            Text = scenario == "AutoSize + AutoEllipsis" ? "A rather long button caption" : "Go to",
            Margin = new Padding(6)
        };

        switch (scenario)
        {
            case "Disabled":
                button.Enabled = false;
                break;
            case "With Image":
                button.Image = image;
                button.TextImageRelation = TextImageRelation.ImageBeforeText;
                break;
            case "With BackgroundImage":
                button.BackgroundImage = image;
                button.BackgroundImageLayout = ImageLayout.Center;
                break;
            case "FlatAppearance":
                button.FlatAppearance.BorderColor = Color.MediumPurple;
                button.FlatAppearance.BorderSize = 2;
                button.FlatAppearance.MouseOverBackColor = Color.MediumPurple;
                button.FlatAppearance.MouseDownBackColor = Color.Indigo;
                break;
            case "AutoSize + AutoEllipsis":
                button.AutoSize = false;
                button.AutoEllipsis = true;
                button.Size = new Size(90, 28);
                break;
        }

        return button;
    }

    private void OnModernToggleChanged(object? sender, EventArgs e)
    {
        VisualStylesMode mode = _modernToggle.Checked ? VisualStylesMode.Net11 : VisualStylesMode.Classic;
        foreach (Button button in _sampleButtons)
        {
            button.VisualStylesMode = mode;
        }

        foreach (CheckBox toggle in _sampleToggles)
        {
            toggle.VisualStylesMode = mode;
        }
    }
}
