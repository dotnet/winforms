// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.IntegrationTests.Common;
using Microsoft.Win32;
using WindowsFormsApp1;
using WinFormsControlsTest.UserControls;

namespace WinFormsControlsTest;

[DesignerCategory("code")]
public partial class MainForm : Form
{
    public MainForm()
    {
        this.BringToForeground();
        InitializeComponent();

        // Init buttons
        IReadOnlyDictionary<MainFormControlsTabOrder, InitInfo> buttonsInitInfo = GetButtonsInitInfo();
        Array mainFormControlsTabOrderItems = Enum.GetValues(typeof(MainFormControlsTabOrder));

        foreach (MainFormControlsTabOrder item in mainFormControlsTabOrderItems)
        {
            InitInfo info = buttonsInitInfo[item];
            Button button = new Button
            {
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                Name = info.Name,
                TabIndex = (int)item,
                Text = info.Name,
                UseVisualStyleBackColor = true
            };
            button.Click += info.Click;
            overarchingFlowLayoutPanel.Controls.Add(button);
        }

        Text = RuntimeInformation.FrameworkDescription;
        _colorComboBox.SelectedIndexChanged += (s, e) =>
        {
            if (_colorComboBox.SelectedItem is string selectedItem)
            {
                switch (selectedItem)
                {
                    case "System":
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
                        Application.SetColorMode(SystemColorMode.System);

                        break;
                    case "Classic":
                        Application.SetColorMode(SystemColorMode.Classic);
                        break;
                    case "Dark":
                        Application.SetColorMode(SystemColorMode.Dark);
                        break;
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

                }
            }
        };
        _colorPanel.Controls.Add(_colorComboBox);
        _colorPanel.Controls.Add(_colorLabel);
        overarchingFlowLayoutPanel.Controls.Add(_colorPanel);
        SystemEvents.UserPreferenceChanged += (s, e) =>
        {
            // The default font gets reset for UserPreferenceCategory.Color
            // though perhaps it should've been done for UserPreferenceCategory.Window
            if (e.Category == UserPreferenceCategory.Color)
            {
                UpdateLayout();
            }
        };
    }

    private IReadOnlyDictionary<MainFormControlsTabOrder, InitInfo> GetButtonsInitInfo() => new Dictionary<MainFormControlsTabOrder, InitInfo>
    {
        {
            MainFormControlsTabOrder.ButtonsButton,
            new InitInfo("Buttons", (obj, e) => new Buttons().Show(this))
        },
        {
            MainFormControlsTabOrder.CalendarButton,
            new InitInfo("Calendar", (obj, e) => new Calendar().Show(this))
        },
        {
            MainFormControlsTabOrder.MultipleControlsButton,
            new InitInfo("MultipleControls", (obj, e) => new MultipleControls().Show(this))
        },
        {
            MainFormControlsTabOrder.ComboBoxesButton,
            new InitInfo("ComboBoxes", (obj, e) => new ComboBoxes().Show(this))
        },
        {
            MainFormControlsTabOrder.ComboBoxesWithScrollBarsButton,
            new InitInfo("ComboBoxes with ScrollBars", (obj, e) => new ComboBoxesWithScrollBars().Show(this))
        },
        {
            MainFormControlsTabOrder.DateTimePickerButton,
            new InitInfo("DateTimePicker", (obj, e) => new DateTimePicker().Show(this))
        },
        {
            MainFormControlsTabOrder.DialogsButton,
            new InitInfo("Dialogs", (obj, e) => new Dialogs().ShowDialog(this))
        },
        {
            MainFormControlsTabOrder.DataGridViewButton,
            new InitInfo("DataGridView", (obj, e) => new DataGridViewTest().Show(this))
        },
        {
            MainFormControlsTabOrder.DataGridViewInVirtualModeButton,
            new InitInfo("DataGridView in Virtual mode", (obj, e) => new DataGridViewInVirtualModeTest().Show(this))
        },
        {
            MainFormControlsTabOrder.TreeViewButton,
            new InitInfo("TreeView, ImageList", (obj, e) => new TreeViewTest().Show(this))
        },
        {
            MainFormControlsTabOrder.ContentAlignmentButton,
            new InitInfo("ContentAlignment", (obj, e) => new DesignTimeAligned().Show(this))
        },
        {
            MainFormControlsTabOrder.MenusButton,
            new InitInfo("Menus", (obj, e) => new MenuStripAndCheckedListBox().Show(this))
        },
        {
            MainFormControlsTabOrder.PanelsButton,
            new InitInfo("Panels", (obj, e) => new Panels().Show(this))
        },
        {
            MainFormControlsTabOrder.SplitterButton,
            new InitInfo("Splitter", (obj, e) => new Splitter().Show(this))
        },
        {
            MainFormControlsTabOrder.MdiParentButton,
            new InitInfo("MDI Parent", (obj, e) => new MdiParent().Show(this))
        },
        {
            MainFormControlsTabOrder.PropertyGridButton,
            new InitInfo("PropertyGrid", (obj, e) => new PropertyGrid(new UserControlWithObjectCollectionEditor()).Show(this))
        },
        {
            MainFormControlsTabOrder.ListViewButton,
            new InitInfo("ListView", (obj, e) => new ListViewTest().Show(this))
        },
        {
            MainFormControlsTabOrder.FontNameEditorButton,
            new InitInfo("FontNameEditor", (obj, e) => new PropertyGrid(new UserControlWithFontNameEditor()).Show(this))
        },
        {
            MainFormControlsTabOrder.CollectionEditorsButton,
            new InitInfo("CollectionEditors", (obj, e) => new CollectionEditors().Show(this))
        },
        {
            MainFormControlsTabOrder.RichTextBoxesButton,
            new InitInfo("RichTextBoxes", (obj, e) => new RichTextBoxes().Show(this))
        },
        {
            MainFormControlsTabOrder.PictureBoxesButton,
            new InitInfo("PictureBoxes", (obj, e) => new PictureBoxes().Show(this))
        },
        {
            MainFormControlsTabOrder.FormBorderStylesButton,
            new InitInfo("FormBorderStyles", (obj, e) => new FormBorderStyles().Show(this))
        },
        {
            MainFormControlsTabOrder.FormShowInTaskbarButton,
            new InitInfo("FormShowInTaskbar", (obj, e) => new FormShowInTaskbar().Show(this))
        },
        {
            MainFormControlsTabOrder.ToggleIconButton,
            new InitInfo("ToggleFormIcon", (obj, e) => ShowIcon = !ShowIcon)
        },
        {
            MainFormControlsTabOrder.ErrorProviderButton,
            new InitInfo("ErrorProvider", (obj, e) => new ErrorProviderTest().Show(this))
        },
        {
            MainFormControlsTabOrder.TaskDialogButton,
            new InitInfo("Task Dialog", (obj, e) => new TaskDialogSamples().Show(this))
        },
        {
            MainFormControlsTabOrder.MessageBoxButton,
            new InitInfo("MessageBox", (obj, e) => new MessageBoxes().Show(this))
        },
        {
            MainFormControlsTabOrder.ToolStripsButton,
            new InitInfo("ToolStrips", (obj, e) => new ToolStripTests().Show(this))
        },
        {
            MainFormControlsTabOrder.TrackBarsButton,
            new InitInfo("TrackBars", (obj, e) => new TrackBars().Show(this))
        },
        {
            MainFormControlsTabOrder.ScrollBarsButton,
            new InitInfo("ScrollBars", (obj, e) => new ScrollBars().Show(this))
        },
        {
            MainFormControlsTabOrder.ToolTipsButton,
            new InitInfo("ToolTips", (obj, e) => new ToolTipTests().Show(this))
        },
        {
            MainFormControlsTabOrder.AnchorLayoutButton,
            new InitInfo("AnchorLayout", (obj, e) => new AnchorLayoutTests().Show(this))
        },
        {
            MainFormControlsTabOrder.DockLayoutButton,
            new InitInfo("DockLayout", (obj, e) => new DockLayoutTests().Show(this))
        },
        {
            MainFormControlsTabOrder.DragAndDrop,
            new InitInfo("Drag and Drop", (obj, e) => new DragDrop().Show(this))
        },
        {
            MainFormControlsTabOrder.TextBoxesButton,
            new InitInfo("TextBoxes", (obj, e) => new TextBoxes().Show(this))
        },
        {
            MainFormControlsTabOrder.MediaPlayerButton,
            new InitInfo("MediaPlayer", (obj, e) => new MediaPlayer().Show(this))
        },
        {
            MainFormControlsTabOrder.FormOwnerTestButton,
            new InitInfo("FormOwnerTest", (obj, e) => new FormOwnerTestForm().Show(this))
        },
        {
            MainFormControlsTabOrder.ListBoxTestButton,
            new InitInfo("ListBoxes", (obj, e) => new ListBoxes().Show(this))
        },
        {
            MainFormControlsTabOrder.PasswordButton,
            new InitInfo("Password", (obj, e) => new Password().Show(this))
        },
        {
            MainFormControlsTabOrder.ChartControlButton,
            new InitInfo("ChartControl", (obj, e) => new ChartControl().Show(this))
        },
        {
            // Test GetPreferredSize output https://github.com/dotnet/winforms/issues/2576
            MainFormControlsTabOrder.ToolStripSeparatorPreferredSize,
            new InitInfo("ToolStripSeparatorPreferredSize", (obj, e) => new ToolStripSeparatorPreferredSize().Show(this))
        },
        {
            // Test possible approach to https://github.com/dotnet/winforms/issues/6514
            MainFormControlsTabOrder.CustomComCtl32Button,
            new InitInfo("ComCtl32 Button Custom Border", (obj, e) => new CustomComCtl32Button().Show(this))
        },
        {
            MainFormControlsTabOrder.ScrollableControlsButton,
            new InitInfo("ScrollableControlsButton", (obj, e) => new ScrollableControls().Show(this))
        }
    };

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        overarchingFlowLayoutPanel.Controls[(int)MainFormControlsTabOrder.ButtonsButton].Focus();
        UpdateLayout();
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        _colorComboBox.SelectedIndex = (int)Application.ColorMode;
        Point point = _colorLabel.Location;
        point.Offset(_colorLabel.Width - 30, 0);
        Size size = _colorComboBox.Size;
        Size buttonSize = overarchingFlowLayoutPanel.Controls[(int)MainFormControlsTabOrder.ButtonsButton].Size;
        int width = buttonSize.Width - _colorLabel.Width - 2;
        size.Width = width;
        _colorComboBox.Size = size;
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    }

    private readonly ComboBox _colorComboBox = new ComboBox
    {
        Name = "ColorComboBox",
        TabIndex = 52,
        Text = "ColorMode",
        AutoSize = true,
#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        DataSource = Enum.GetNames<SystemColorMode>(),
#pragma warning restore WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        DropDownStyle = ComboBoxStyle.DropDownList,
    };
    private readonly Label _colorLabel = new Label
    {
        Name = "ColorLabel",
        Text = "ColorMode: ",
        AutoSize = true,
        TabIndex = 51,
        Margin = new Padding(0, 6, 0, 0)
    };
    private readonly FlowLayoutPanel _colorPanel = new FlowLayoutPanel
    {
        Name = "ColorPanel",
        AutoSize = true,
        AutoSizeMode = AutoSizeMode.GrowAndShrink,
        TabIndex = 50,
        FlowDirection = FlowDirection.RightToLeft,
    };

    private void UpdateLayout()
    {
        MinimumSize = default;
        Debug.WriteLine($"MessageBoxFont: {SystemFonts.MessageBoxFont}", nameof(MainForm));
        Debug.WriteLine($"Default font: {DefaultFont}", nameof(MainForm));

        List<Button> buttons = [];
        foreach (Control control in overarchingFlowLayoutPanel.Controls)
        {
            if (control is Button button )
            {
                buttons.Add(button);
            }
            else
            {
              if (control is not (FlowLayoutPanel or Label or ComboBox))
                {
                    Debug.WriteLine($"Why did we get a {control.GetType().Name} instead a {nameof(Button)} on {nameof(MainForm)}?");
                }
            }
        }

        // 1. Auto-size all buttons
        overarchingFlowLayoutPanel.SuspendLayout();
        foreach (Button button in buttons)
        {
            button.AutoSize = true;
        }

        overarchingFlowLayoutPanel.ResumeLayout(true);

        // 2. Find the biggest button
        Size biggestButton = default;
        foreach (Button button in buttons)
        {
            if (button.Width > biggestButton.Width)
            {
                biggestButton = button.Size;
            }
        }

        Debug.WriteLine($"Biggest button size: {biggestButton}", nameof(MainForm));

        // 3. Size all buttons to the biggest button
        overarchingFlowLayoutPanel.SuspendLayout();
        foreach (Button button in buttons)
        {
            button.AutoSize = false;
            button.Size = biggestButton;
        }

        overarchingFlowLayoutPanel.ResumeLayout(true);

        // 4. Calculate the new form size showing all buttons in three vertical columns
        int padding = overarchingFlowLayoutPanel.Controls[0].Margin.All;

        ClientSize = new Size(
            ((biggestButton.Width + (padding * 2)) * 3) + (padding * 2) + (overarchingFlowLayoutPanel.Location.X * 2),
            ((int)Math.Ceiling((overarchingFlowLayoutPanel.Controls.Count + 1) / 3.0) * (biggestButton.Height + (padding * 2)))
                + (padding * 2) + (overarchingFlowLayoutPanel.Location.Y * 2));
        MinimumSize = Size;
        Debug.WriteLine($"Minimum form size: {MinimumSize}", nameof(MainForm));
    }

    private struct InitInfo
    {
        public InitInfo(string name, EventHandler handler)
        {
            Name = name;
            Click = handler;
        }

        public string Name { get; }

        public EventHandler Click { get; }
    }
}
