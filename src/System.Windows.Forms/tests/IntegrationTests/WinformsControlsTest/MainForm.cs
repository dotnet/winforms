// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.IntegrationTests.Common;
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
                    Name = info.Name,
                    Size = new Size(259, 23),
                    TabIndex = (int)item,
                    Text = info.Name,
                    UseVisualStyleBackColor = true
                };
                button.Click += info.Click;

                flowLayoutPanelUITypeEditors.Controls.Add(button);
            }

            // Calculate the form size.
            ClientSize = new Size(545, 18 + (mainFormControlsTabOrderItems.Length + 1) / 2 * 29);
            MinimumSize = Size;

            // Force the panel to show all buttons
            flowLayoutPanelUITypeEditors.PerformLayout();
            flowLayoutPanelUITypeEditors.Controls[(int)MainFormControlsTabOrder.ButtonsButton].Focus();
        }

        private IReadOnlyDictionary<MainFormControlsTabOrder, InitInfo> GetButtonsInitInfo() => new Dictionary<MainFormControlsTabOrder, InitInfo>
        {
            {
                MainFormControlsTabOrder.ButtonsButton,
                new InitInfo("Buttons", (obj, e) => new Buttons().Show())
            },
            {
                MainFormControlsTabOrder.CalendarButton,
                new InitInfo("Calendar", (obj, e) => new Calendar().Show())
            },
            {
                MainFormControlsTabOrder.MultipleControlsButton,
                new InitInfo("MultipleControls", (obj, e) => new MultipleControls().Show())
            },
            {
                MainFormControlsTabOrder.ComboBoxesButton,
                new InitInfo("ComboBoxes", (obj, e) => new ComboBoxes().Show())
            },
            {
                MainFormControlsTabOrder.DateTimePickerButton,
                new InitInfo("DateTimePicker", (obj, e) => new DateTimePicker().Show())
            },
            {
                MainFormControlsTabOrder.FolderBrowserDialogButton,
                new InitInfo("FolderBrowserDialog", (obj, e) => new FolderBrowserDialog().ShowDialog())
            },
            {
                MainFormControlsTabOrder.ThreadExceptionDialogButton,
                new InitInfo("ThreadExceptionDialog", (obj, e) => new ThreadExceptionDialog(new Exception("Really long exception description string, because we want to see if it properly wraps around or is truncated.")).ShowDialog())
            },
            {
                MainFormControlsTabOrder.PrintDialogButton,
                new InitInfo("PrintDialog", (obj, e) => new PrintDialog().ShowDialog())
            },
            {
                MainFormControlsTabOrder.DataGridViewButton,
                new InitInfo("DataGridView", (obj, e) => new DataGridViewHeaders().Show())
            },
            {
                MainFormControlsTabOrder.TreeViewButton,
                new InitInfo("TreeView, ImageList", (obj, e) => new TreeViewTest().Show())
            },
            {
                MainFormControlsTabOrder.ContentAlignmentButton,
                new InitInfo("ContentAlignment", (obj, e) => new DesignTimeAligned().Show())
            },
            {
                MainFormControlsTabOrder.MenusButton,
                new InitInfo("Menus", (obj, e) => new MenuStripAndCheckedListBox().Show())
            },
            {
                MainFormControlsTabOrder.PanelsButton,
                new InitInfo("Panels", (obj, e) => new Panels().Show())
            },
            {
                MainFormControlsTabOrder.SplitterButton,
                new InitInfo("Splitter", (obj, e) => new Splitter().Show())
            },
            {
                MainFormControlsTabOrder.MdiParentButton,
                new InitInfo("MDI Parent", (obj, e) => new MDIParent().Show())
            },
            {
                MainFormControlsTabOrder.PropertyGridButton,
                new InitInfo("PropertyGrid", (obj, e) => new PropertyGrid(new UserControlWithObjectCollectionEditor()).Show())
            },
            {
                MainFormControlsTabOrder.ListViewButton,
                new InitInfo("ListVew", (obj, e) => new ListViewTest().Show())
            },
            {
                MainFormControlsTabOrder.FontNameEditorButton,
                new InitInfo("FontNameEditor", (obj, e) => new PropertyGrid(new UserControlWithFontNameEditor()).Show())
            },
            {
                MainFormControlsTabOrder.CollectionEditorsButton,
                new InitInfo("CollectionEditors", (obj, e) => new CollectionEditors().Show())
            },
            {
                MainFormControlsTabOrder.RichTextBoxesButton,
                new InitInfo("RichTextBoxes", (obj, e) => new RichTextBoxes().Show())
            },
            {
                MainFormControlsTabOrder.PictureBoxesButton,
                new InitInfo("PictureBoxes", (obj, e) => new PictureBoxes().Show())
            },
            {
                MainFormControlsTabOrder.FormBorderStylesButton,
                new InitInfo("FormBorderStyles", (obj, e) => new FormBorderStyles().Show())
            },
            {
                MainFormControlsTabOrder.ToggleIconButton,
                new InitInfo("ToggleFormIcon", (obj, e) => ShowIcon = !ShowIcon)
            },
            {
                MainFormControlsTabOrder.FileDialogButton,
                new InitInfo("FileDialog", (obj, e) => new FileDialog().Show())
            }
        };

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
