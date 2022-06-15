// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design
{
    public partial class ColorEditor
    {
        /// <summary>
        ///  Editor UI for the color editor.
        /// </summary>
        private class ColorUI : Control
        {
            private readonly ColorEditor editor;
            private IWindowsFormsEditorService edSvc;
            private object value;
            private ColorEditorTabControl tabControl;
            private TabPage systemTabPage;
            private TabPage commonTabPage;
            private TabPage paletteTabPage;
            private ListBox lbSystem;
            private ListBox lbCommon;
            private ColorPalette pal;
            private object[] systemColorConstants;
            private object[] colorConstants;
            private Color[] customColors;
            private bool commonHeightSet;
            private bool systemHeightSet;

            public ColorUI(ColorEditor editor)
            {
                this.editor = editor;
                InitializeComponent();
                AdjustListBoxItemHeight();
            }

            /// <summary>
            ///  Array of standard colors.
            /// </summary>
            private object[] ColorValues => colorConstants ?? (colorConstants = GetConstants(typeof(Color)));

            /// <summary>
            ///  Retrieves the array of custom colors for our use.
            /// </summary>
            private Color[] CustomColors
            {
                get
                {
                    if (customColors is null)
                    {
                        customColors = new Color[ColorPalette.CellsCustom];
                        for (int i = 0; i < ColorPalette.CellsCustom; i++)
                        {
                            customColors[i] = Color.White;
                        }
                    }

                    return customColors;
                }

                set
                {
                    customColors = value;
                    pal = null;
                }
            }

            /// <summary>
            ///  Allows someone else to close our dropdown.
            /// </summary>
            public IWindowsFormsEditorService EditorService => edSvc;

            /// <summary>
            ///  Array of system colors.
            /// </summary>
            private object[] SystemColorValues => systemColorConstants ?? (systemColorConstants = GetConstants(typeof(SystemColors)));

            public object Value => value;

            public void End()
            {
                edSvc = null;
                value = null;
            }

            private void AdjustColorUIHeight()
            {
                // Compute the default size for the color UI
                //
                Size size = pal.Size;
                Rectangle rectItemSize = tabControl.GetTabRect(0);
                int CMARGIN = 0;
                Size = new Size(size.Width + 2 * CMARGIN, size.Height + 2 * CMARGIN + rectItemSize.Height);
                tabControl.Size = Size;
            }

            private void AdjustListBoxItemHeight()
            {
                lbSystem.ItemHeight = Font.Height + 2;
                lbCommon.ItemHeight = Font.Height + 2;
            }

            /// <summary>
            ///  Takes the given color and looks for an instance in the ColorValues table.
            /// </summary>
            private Color GetBestColor(Color color)
            {
                object[] colors = ColorValues;
                int rgb = color.ToArgb();
                for (int i = 0; i < colors.Length; i++)
                {
                    if (((Color)colors[i]).ToArgb() == rgb)
                    {
                        return (Color)colors[i];
                    }
                }

                return color;
            }

            /// <summary>
            ///  Retrieves an array of color constants for the given object.
            /// </summary>
            private static object[] GetConstants(Type enumType)
            {
                MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Static;
                PropertyInfo[] props = enumType.GetProperties();

                ArrayList colorList = new ArrayList();

                for (int i = 0; i < props.Length; i++)
                {
                    PropertyInfo prop = props[i];
                    if (prop.PropertyType == typeof(Color))
                    {
                        MethodInfo method = prop.GetGetMethod();
                        if (method != null && (method.Attributes & attrs) == attrs)
                        {
                            object[] tempIndex = null;
                            colorList.Add(prop.GetValue(null, tempIndex));
                        }
                    }
                }

                return colorList.ToArray();
            }

            private void InitializeComponent()
            {
                paletteTabPage = new TabPage(SR.ColorEditorPaletteTab);
                commonTabPage = new TabPage(SR.ColorEditorStandardTab);
                systemTabPage = new TabPage(SR.ColorEditorSystemTab);

                AccessibleName = SR.ColorEditorAccName;

                tabControl = new ColorEditorTabControl();
                tabControl.TabPages.Add(paletteTabPage);
                tabControl.TabPages.Add(commonTabPage);
                tabControl.TabPages.Add(systemTabPage);
                tabControl.TabStop = false;
                tabControl.SelectedTab = systemTabPage;
                tabControl.SelectedIndexChanged += new EventHandler(OnTabControlSelChange);
                tabControl.Dock = DockStyle.Fill;
                tabControl.Resize += new EventHandler(OnTabControlResize);

                lbSystem = new ColorEditorListBox
                {
                    DrawMode = DrawMode.OwnerDrawFixed,
                    BorderStyle = BorderStyle.FixedSingle,
                    IntegralHeight = false,
                    Sorted = false
                };
                lbSystem.Click += new EventHandler(OnListClick);
                lbSystem.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                lbSystem.KeyDown += new KeyEventHandler(OnListKeyDown);
                lbSystem.Dock = DockStyle.Fill;
                lbSystem.FontChanged += new EventHandler(OnFontChanged);

                lbCommon = new ColorEditorListBox
                {
                    DrawMode = DrawMode.OwnerDrawFixed,
                    BorderStyle = BorderStyle.FixedSingle,
                    IntegralHeight = false,
                    Sorted = false
                };
                lbCommon.Click += new EventHandler(OnListClick);
                lbCommon.DrawItem += new DrawItemEventHandler(OnListDrawItem);
                lbCommon.KeyDown += new KeyEventHandler(OnListKeyDown);
                lbCommon.Dock = DockStyle.Fill;

                Array.Sort(ColorValues, new StandardColorComparer());
                Array.Sort(SystemColorValues, new SystemColorComparer());

                lbCommon.Items.Clear();
                foreach (object color in ColorValues)
                {
                    lbCommon.Items.Add(color);
                }

                lbSystem.Items.Clear();
                foreach (object color in SystemColorValues)
                {
                    lbSystem.Items.Add(color);
                }

                pal = new ColorPalette(this, CustomColors);
                pal.Picked += new EventHandler(OnPalettePick);

                paletteTabPage.Controls.Add(pal);
                systemTabPage.Controls.Add(lbSystem);
                commonTabPage.Controls.Add(lbCommon);

                Controls.Add(tabControl);
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                OnTabControlSelChange(this, EventArgs.Empty);
            }

            private void OnFontChanged(object sender, EventArgs e)
            {
                commonHeightSet = systemHeightSet = false;
            }

            private void OnListClick(object sender, EventArgs e)
            {
                ListBox lb = (ListBox)sender;
                if (lb.SelectedItem != null)
                {
                    value = (Color)lb.SelectedItem;
                }

                edSvc.CloseDropDown();
            }

            private void OnListDrawItem(object sender, DrawItemEventArgs die)
            {
                ListBox lb = (ListBox)sender;
                object value = lb.Items[die.Index];
                Font font = Font;

                if (lb == lbCommon && !commonHeightSet)
                {
                    lb.ItemHeight = lb.Font.Height;
                    commonHeightSet = true;
                }
                else if (lb == lbSystem && !systemHeightSet)
                {
                    lb.ItemHeight = lb.Font.Height;
                    systemHeightSet = true;
                }

                Graphics graphics = die.Graphics;
                die.DrawBackground();

                editor.PaintValue(value, graphics, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 22, die.Bounds.Height - 4));
                graphics.DrawRectangle(SystemPens.WindowText, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 22 - 1, die.Bounds.Height - 4 - 1));
                Brush foreBrush = new SolidBrush(die.ForeColor);
                graphics.DrawString(((Color)value).Name, font, foreBrush, die.Bounds.X + 26, die.Bounds.Y);
                foreBrush.Dispose();
            }

            private void OnListKeyDown(object sender, KeyEventArgs ke)
            {
                if (ke.KeyCode == Keys.Return)
                {
                    OnListClick(sender, EventArgs.Empty);
                }
            }

            private void OnPalettePick(object sender, EventArgs e)
            {
                ColorPalette p = (ColorPalette)sender;
                value = GetBestColor(p.SelectedColor);
                edSvc.CloseDropDown();
            }

            protected override void OnFontChanged(EventArgs e)
            {
                base.OnFontChanged(e);
                AdjustListBoxItemHeight();
                AdjustColorUIHeight();
            }

            private void OnTabControlResize(object sender, EventArgs e)
            {
                Rectangle rectTabControl = tabControl.TabPages[0].ClientRectangle;
                Rectangle rectItemSize = tabControl.GetTabRect(1);
                rectTabControl.Y = 0;
                rectTabControl.Height -= rectTabControl.Y;
                int CMARGIN = 2;
                lbSystem.SetBounds(CMARGIN, rectTabControl.Y + 2 * CMARGIN,
                                   rectTabControl.Width - CMARGIN,
                                   pal.Size.Height - rectItemSize.Height + 2 * CMARGIN);
                lbCommon.Bounds = lbSystem.Bounds;
                pal.Location = new Point(0, rectTabControl.Y);
            }

            private void OnTabControlSelChange(object sender, EventArgs e)
            {
                TabPage selectedPage = tabControl.SelectedTab;

                if (selectedPage != null && selectedPage.Controls.Count > 0)
                {
                    selectedPage.Controls[0].Focus();
                }
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                // We treat tab characters as switching tab pages.  In most other contexts,
                // ctrl-tab switches tab pages, but I couldn't get that to work, and besides,
                // then there would be nothing for tab to do in this editor.
                if ((keyData & Keys.Alt) == 0
                    && (keyData & Keys.Control) == 0
                    && (keyData & Keys.KeyCode) == Keys.Tab)
                {
                    // Logic taken straight out of TabBase
                    bool forward = (keyData & Keys.Shift) == 0;
                    int sel = tabControl.SelectedIndex;
                    if (sel != -1)
                    {
                        int count = tabControl.TabPages.Count;
                        if (forward)
                        {
                            sel = (sel + 1) % count;
                        }
                        else
                        {
                            sel = (sel + count - 1) % count;
                        }

                        tabControl.SelectedTab = tabControl.TabPages[sel];

                        return true;
                    }
                }

                return base.ProcessDialogKey(keyData);
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                this.edSvc = edSvc;
                this.value = value;

                AdjustColorUIHeight();

                // Now look for the current color so we can select the proper tab.
                //
                if (value != null)
                {
                    object[] values = ColorValues;
                    TabPage selectedTab = paletteTabPage;

                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].Equals(value))
                        {
                            lbCommon.SelectedItem = value;
                            selectedTab = commonTabPage;
                            break;
                        }
                    }

                    if (selectedTab == paletteTabPage)
                    {
                        values = SystemColorValues;
                        for (int i = 0; i < values.Length; i++)
                        {
                            if (values[i].Equals(value))
                            {
                                lbSystem.SelectedItem = value;
                                selectedTab = systemTabPage;
                                break;
                            }
                        }
                    }

                    tabControl.SelectedTab = selectedTab;
                }
            }

            private class ColorEditorListBox : ListBox
            {
                protected override bool IsInputKey(Keys keyData)
                {
                    switch (keyData)
                    {
                        case Keys.Return:
                            return true;
                    }

                    return base.IsInputKey(keyData);
                }
            }

            private class ColorEditorTabControl : TabControl
            {
                public ColorEditorTabControl() : base()
                {
                }

                protected override void OnGotFocus(EventArgs e)
                {
                    TabPage selectedTab = SelectedTab;
                    if (selectedTab != null && selectedTab.Controls.Count > 0)
                    {
                        selectedTab.Controls[0].Focus();
                    }
                }
            }
        }
    }
}
