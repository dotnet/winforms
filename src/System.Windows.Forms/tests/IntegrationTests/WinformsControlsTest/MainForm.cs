// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
using Microsoft.Win32;
using WindowsFormsApp1;
using WinformsControlsTest.UserControls;

namespace WinformsControlsTest
{
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

                flowLayoutPanelUITypeEditors.Controls.Add(button);
            }

            Text = RuntimeInformation.FrameworkDescription;

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
                MainFormControlsTabOrder.ToggleIconButton,
                new InitInfo("ToggleFormIcon", (obj, e) => ShowIcon = !ShowIcon)
            },
            {
                MainFormControlsTabOrder.FileDialogButton,
                new InitInfo("FileDialog", (obj, e) => new FileDialog().Show(this))
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
            }
        };

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            UpdateLayout();
            flowLayoutPanelUITypeEditors.Controls[(int)MainFormControlsTabOrder.ButtonsButton].Focus();
        }

        private void UpdateLayout()
        {
            MinimumSize = default;
            Debug.WriteLine($"MessageBoxFont: {SystemFonts.MessageBoxFont}", nameof(MainForm));
            Debug.WriteLine($"Default font: {Control.DefaultFont}", nameof(MainForm));

            // 1. Auto-size all buttons
            flowLayoutPanelUITypeEditors.SuspendLayout();
            foreach (Control c in flowLayoutPanelUITypeEditors.Controls)
            {
                if (c is Button button)
                {
                    button.AutoSize = true;
                }
            }

            flowLayoutPanelUITypeEditors.ResumeLayout(true);

            // 2. Find the biggest button
            Size biggestButton = default;
            foreach (Control c in flowLayoutPanelUITypeEditors.Controls)
            {
                if (c is Button button)
                {
                    if (button.Width > biggestButton.Width)
                    {
                        biggestButton = button.Size;
                    }
                }
            }

            Debug.WriteLine($"Biggest button size: {biggestButton}", nameof(MainForm));

            // 3. Size all buttons to the biggest button
            flowLayoutPanelUITypeEditors.SuspendLayout();
            foreach (Control c in flowLayoutPanelUITypeEditors.Controls)
            {
                if (c is Button button)
                {
                    button.AutoSize = false;
                    button.Size = biggestButton;
                }
            }

            flowLayoutPanelUITypeEditors.ResumeLayout(true);

            // 4. Calculate the new form size showing all buttons in two vertical columns
            int padding = flowLayoutPanelUITypeEditors.Controls[0].Margin.All;
            ClientSize = new Size(
                (biggestButton.Width + padding * 2) * 2 + padding * 2,
                (int)(flowLayoutPanelUITypeEditors.Controls.Count / 2 * (biggestButton.Height + padding * 2) + padding * 2)
                );
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
}
