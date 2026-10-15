// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Drawing;
using System.Drawing.Drawing2D;

namespace WinFormsControlsTest;

/// <summary>
///  Exploratory-testing harness for conservative and modern (.NET 11 VisualStyles) control renderers.
///  Toggle the "Modern visual styles" check box to flip every sample control between
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
    private readonly List<RadioButton> _sampleRadioToggles = [];
    private readonly List<ButtonBase> _sampleAppearanceButtons = [];
    private readonly List<Control> _sampleControls = [];
    private readonly PatternedGradientPanel _sampleParent;
    private readonly CheckBox _modernToggle;
    private readonly CheckBox _parentModernToggle;
    private readonly CheckBox _rightToLeftToggle;
    private readonly Label _environmentLabel;

    public VisualStylesButtons()
    {
        Text = "VisualStyles Buttons (exploratory)";
        AutoScaleMode = AutoScaleMode.Dpi;
        Size = new Size(1100, 1050);
        Padding = new Padding(8);

        _sampleParent = new PatternedGradientPanel
        {
            Dock = DockStyle.Top,
            Height = 600,
            Padding = new Padding(8),
            VisualStylesMode = VisualStylesMode.Classic
        };

        _modernToggle = new CheckBox
        {
            Text = "Modern child styles (.NET 11; off = inherit)",
            AutoSize = true,
            Dock = DockStyle.Top,
            Padding = new Padding(4)
        };
        _modernToggle.CheckedChanged += OnModernToggleChanged;

        _parentModernToggle = new CheckBox
        {
            Text = "Modern parent mode",
            AutoSize = true,
            Checked = false,
            Margin = new Padding(8, 4, 8, 4)
        };
        _parentModernToggle.CheckedChanged += OnParentModernToggleChanged;

        _rightToLeftToggle = new CheckBox
        {
            Text = "Right-to-left",
            AutoSize = true,
            Margin = new Padding(8, 4, 8, 4)
        };
        _rightToLeftToggle.CheckedChanged += OnRightToLeftToggleChanged;

        _environmentLabel = new Label
        {
            AutoSize = true,
            Margin = new Padding(8, 8, 8, 4)
        };
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
            "AutoSize + AutoEllipsis",
            "Focused",
            "Pressed (hold mouse)",
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

        Button? focusedButton = _sampleButtons.FirstOrDefault(
            button => button.Text == "Focus me" && button.FlatStyle == FlatStyle.Standard);

        BuildTextAndUpDownSamples();
        _sampleParent.Controls.Add(BuildGroupAndComboSamples());
        Controls.Add(table);
        Controls.Add(_sampleParent);
        Controls.Add(BuildCheckablePopupPanel());
        Controls.Add(BuildToggleSwitchPanel());
        Controls.Add(_modernToggle);

        _environmentLabel.Text = $"DPI: {DeviceDpi}    High contrast: {SystemInformation.HighContrast}";
        ApplyChildVisualStylesMode();
        BeginInvoke(() => focusedButton?.Focus());
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

        foreach (float fontSize in new[] { 8f, 9f, 11f })
        {
            CheckBox toggle = new()
            {
                Appearance = Appearance.ToggleSwitch,
                AutoSize = true,
                Text = $"CheckBox {fontSize:0} pt",
                Checked = fontSize != 9f,
                Font = new Font(Font.FontFamily, fontSize),
                Margin = new Padding(8, 4, 8, 4)
            };

            _sampleToggles.Add(toggle);
            panel.Controls.Add(toggle);

            RadioButton radioToggle = new()
            {
                Appearance = Appearance.ToggleSwitch,
                AutoSize = true,
                Text = $"RadioButton {fontSize:0} pt",
                Checked = fontSize == 9f,
                Font = new Font(Font.FontFamily, fontSize),
                Margin = new Padding(8, 4, 8, 4)
            };

            _sampleRadioToggles.Add(radioToggle);
            panel.Controls.Add(radioToggle);
        }

        panel.Controls.Add(_parentModernToggle);
        panel.Controls.Add(_rightToLeftToggle);
        panel.Controls.Add(_environmentLabel);

        return panel;
    }

    private void BuildTextAndUpDownSamples()
    {
        TableLayoutPanel table = new()
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.Transparent,
            ColumnCount = 3,
            RowCount = 11,
            Padding = new Padding(4)
        };

        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        table.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
        table.Controls.Add(new Label { Text = "Control / font", AutoSize = true, Font = new Font(Font, FontStyle.Bold) }, 0, 0);
        table.Controls.Add(new Label { Text = "AutoSize", AutoSize = true, Font = new Font(Font, FontStyle.Bold) }, 1, 0);
        table.Controls.Add(new Label { Text = "Fixed small", AutoSize = true, Font = new Font(Font, FontStyle.Bold) }, 2, 0);

        string[] controlKinds = ["TextBox", "MaskedTextBox", "RichTextBox", "NumericUpDown", "DomainUpDown"];
        int row = 1;
        foreach (float fontSize in new[] { 9f, 11f })
        {
            foreach (string controlKind in controlKinds)
            {
                table.Controls.Add(
                    new Label
                    {
                        Text = $"{controlKind} ({fontSize:0} pt)",
                        AutoSize = true,
                        Anchor = AnchorStyles.Left
                    },
                    0,
                    row);

                Control autoSized = CreateSampleControl(controlKind, fontSize, autoSize: true);
                Control fixedSmall = CreateSampleControl(controlKind, fontSize, autoSize: false);
                _sampleControls.Add(autoSized);
                _sampleControls.Add(fixedSmall);
                table.Controls.Add(autoSized, 1, row);
                table.Controls.Add(fixedSmall, 2, row);
                row++;
            }
        }

        _sampleParent.Controls.Add(table);
    }

    private TableLayoutPanel BuildGroupAndComboSamples()
    {
        TableLayoutPanel table = new()
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            BackColor = Color.Transparent,
            ColumnCount = s_styles.Length + 1,
            Dock = DockStyle.Bottom,
            RowCount = 5,
            Padding = new Padding(4)
        };
        table.Controls.Add(
            new Label
            {
                AutoSize = true,
                Font = new Font(Font, FontStyle.Bold),
                Text = "GroupBox / ComboBox"
            },
            0,
            0);

        for (int column = 0; column < s_styles.Length; column++)
        {
            FlatStyle flatStyle = s_styles[column];
            table.Controls.Add(
                new Label
                {
                    AutoSize = true,
                    Font = new Font(Font, FontStyle.Bold),
                    Text = flatStyle.ToString()
                },
                column + 1,
                0);

            GroupBox groupBox = new()
            {
                FlatStyle = flatStyle,
                Size = new Size(160, 76),
                Text = "Group caption",
                VisualStylesMode = VisualStylesMode.Inherit
            };
            groupBox.Controls.Add(
                new Label
                {
                    AutoSize = true,
                    Dock = DockStyle.Fill,
                    Text = "Docked content"
                });
            _sampleControls.Add(groupBox);
            table.Controls.Add(groupBox, column + 1, 1);

            AddComboBox(
                ComboBoxStyle.DropDown,
                flatStyle,
                column + 1,
                row: 2);
            AddComboBox(
                ComboBoxStyle.DropDownList,
                flatStyle,
                column + 1,
                row: 3);
            AddComboBox(
                ComboBoxStyle.Simple,
                flatStyle,
                column + 1,
                row: 4);
        }

        table.Controls.Add(
            new Label { AutoSize = true, Text = "GroupBox" },
            0,
            1);
        table.Controls.Add(
            new Label { AutoSize = true, Text = "Combo DropDown" },
            0,
            2);
        table.Controls.Add(
            new Label { AutoSize = true, Text = "Combo DropDownList" },
            0,
            3);
        table.Controls.Add(
            new Label { AutoSize = true, Text = "Combo Simple" },
            0,
            4);

        return table;

        void AddComboBox(
            ComboBoxStyle dropDownStyle,
            FlatStyle flatStyle,
            int column,
            int row)
        {
            ComboBox comboBox = new()
            {
                DropDownStyle = dropDownStyle,
                FlatStyle = flatStyle,
                Padding = new Padding(2, 1, 4, 3),
                Width = 160,
                VisualStylesMode = VisualStylesMode.Inherit
            };
            comboBox.Items.AddRange(["First", "Second", "Third"]);
            comboBox.SelectedIndex = 0;
            if (dropDownStyle == ComboBoxStyle.Simple)
            {
                comboBox.Height = 72;
            }

            _sampleControls.Add(comboBox);
            table.Controls.Add(comboBox, column, row);
        }
    }

    private static Control CreateSampleControl(string controlKind, float fontSize, bool autoSize)
    {
        Control control = controlKind switch
        {
            "TextBox" => new TextBox { Text = "Text sample" },
            "MaskedTextBox" => new MaskedTextBox { Mask = "00000", Text = "12345" },
            "RichTextBox" => new RichTextBox { Text = "Rich text", Multiline = true, ScrollBars = RichTextBoxScrollBars.Vertical },
            "NumericUpDown" => new NumericUpDown { Minimum = 0, Maximum = 100, Value = 42 },
            "DomainUpDown" => new DomainUpDown { Items = { "First", "Second", "Third" }, SelectedIndex = 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(controlKind))
        };

        control.Font = new Font(Control.DefaultFont.FontFamily, fontSize);
        control.VisualStylesMode = VisualStylesMode.Inherit;
        control.RightToLeft = RightToLeft.Inherit;
        control.Margin = new Padding(4);
        control.AutoSize = autoSize;

        if (!autoSize)
        {
            control.Size = new Size(120, 14);
        }
        else
        {
            control.Width = 150;
        }

        return control;
    }

    private FlowLayoutPanel BuildCheckablePopupPanel()
    {
        FlowLayoutPanel panel = new()
        {
            Dock = DockStyle.Bottom,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(4)
        };

        panel.Controls.Add(new Label
        {
            Text = "Popup focus/default:",
            AutoSize = true,
            Padding = new Padding(0, 9, 8, 0)
        });

        Button defaultButton = new()
        {
            FlatStyle = FlatStyle.Popup,
            Text = "Default",
            Size = new Size(100, 36),
            TabStop = true
        };
        defaultButton.NotifyDefault(true);
        defaultButton.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 90, 180);
        panel.Controls.Add(defaultButton);
        _sampleButtons.Add(defaultButton);

        CheckBox checkBox = new()
        {
            Appearance = Appearance.Button,
            FlatStyle = FlatStyle.Popup,
            Text = "CheckBox",
            AutoSize = true,
            Checked = true
        };
        panel.Controls.Add(checkBox);
        _sampleAppearanceButtons.Add(checkBox);

        RadioButton radioButton = new()
        {
            Appearance = Appearance.Button,
            FlatStyle = FlatStyle.Popup,
            Text = "RadioButton",
            AutoSize = true,
            Checked = true
        };
        panel.Controls.Add(radioButton);
        _sampleAppearanceButtons.Add(radioButton);

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
            case "Focused":
                button.Text = "Focus me";
                button.TabStop = true;
                break;
            case "Pressed (hold mouse)":
                button.Text = "Hold to press";
                button.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 90, 180);
                button.MouseDown += (_, _) => button.Text = "Pressed";
                button.MouseUp += (_, _) => button.Text = "Hold to press";
                break;
        }

        if (scenario.StartsWith("Default", StringComparison.Ordinal))
        {
            button.NotifyDefault(true);
        }

        return button;
    }

    private void OnModernToggleChanged(object? sender, EventArgs e)
        => ApplyChildVisualStylesMode();

    private void ApplyChildVisualStylesMode()
    {
        VisualStylesMode mode = _modernToggle.Checked ? VisualStylesMode.Net11 : VisualStylesMode.Inherit;
        foreach (Button button in _sampleButtons)
        {
            button.VisualStylesMode = mode;
        }

        foreach (CheckBox toggle in _sampleToggles)
        {
            toggle.VisualStylesMode = mode;
        }

        foreach (RadioButton toggle in _sampleRadioToggles)
        {
            toggle.VisualStylesMode = mode;
        }

        foreach (ButtonBase button in _sampleAppearanceButtons)
        {
            button.VisualStylesMode = mode;
        }

        foreach (Control control in _sampleControls)
        {
            control.VisualStylesMode = mode;
        }
    }

    private void OnParentModernToggleChanged(object? sender, EventArgs e)
    {
        _sampleParent.VisualStylesMode = _parentModernToggle.Checked
            ? VisualStylesMode.Net11
            : VisualStylesMode.Classic;
    }

    private void OnRightToLeftToggleChanged(object? sender, EventArgs e)
    {
        RightToLeft = _rightToLeftToggle.Checked ? RightToLeft.Yes : RightToLeft.No;
        RightToLeftLayout = _rightToLeftToggle.Checked;
        _sampleParent.RightToLeft = RightToLeft;
    }

    private sealed class PatternedGradientPanel : Panel
    {
        public PatternedGradientPanel()
        {
            SetStyle(ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientSize.Width == 0 || ClientSize.Height == 0)
            {
                return;
            }

            using LinearGradientBrush gradient = new(
                ClientRectangle,
                Color.FromArgb(38, 72, 126),
                Color.FromArgb(126, 76, 128),
                LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(gradient, ClientRectangle);

            using HatchBrush pattern = new(
                HatchStyle.DiagonalCross,
                Color.FromArgb(32, Color.White),
                Color.Transparent);
            e.Graphics.FillRectangle(pattern, ClientRectangle);
        }
    }
}
