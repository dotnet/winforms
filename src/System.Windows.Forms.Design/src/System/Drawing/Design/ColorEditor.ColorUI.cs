// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    /// <summary>
    ///  Editor UI for the color editor.
    /// </summary>
    private sealed partial class ColorUI : Control
    {
        private readonly ColorEditor _editor;
        private IWindowsFormsEditorService? _edSvc;
        private object? _value;
        private ColorEditorTabControl _tabControl;
        private TabPage _systemTabPage;
        private TabPage _commonTabPage;
        private TabPage _paletteTabPage;
        private ListBox _lbSystem;
        private ListBox _lbCommon;
        private ColorPalette _pal;
        private Color[]? _systemColorConstants;
        private Color[]? _colorConstants;
        private Color[]? _customColors;
        private bool _commonHeightSet;
        private bool _systemHeightSet;

        public ColorUI(ColorEditor editor)
        {
            _editor = editor;
            InitializeComponent();
            AdjustListBoxItemHeight();
        }

        /// <summary>
        ///  Array of standard colors.
        /// </summary>
        private Color[] ColorValues => _colorConstants ??= GetConstants(typeof(Color));

        /// <summary>
        ///  Retrieves the array of custom colors for our use.
        /// </summary>
        private Color[] CustomColors
        {
            get
            {
                if (_customColors is null)
                {
                    _customColors = new Color[ColorPalette.CellsCustom];
                    for (int i = 0; i < ColorPalette.CellsCustom; i++)
                    {
                        _customColors[i] = Color.White;
                    }
                }

                return _customColors;
            }
        }

        /// <summary>
        ///  Allows someone else to close our dropdown.
        /// </summary>
        public IWindowsFormsEditorService? EditorService => _edSvc;

        /// <summary>
        ///  Array of system colors.
        /// </summary>
        private Color[] SystemColorValues => _systemColorConstants ??= GetConstants(typeof(SystemColors));

        public object? Value => _value;

        public void End()
        {
            _edSvc = null;
            _value = null;
        }

        private void AdjustColorUIHeight()
        {
            // Compute the default size for the color UI
            Size size = _pal.Size;
            Rectangle rectItemSize = _tabControl.GetTabRect(0);
            int CMARGIN = 0;
            Size = new Size(size.Width + 2 * CMARGIN, size.Height + 2 * CMARGIN + rectItemSize.Height);
            _tabControl.Size = Size;
        }

        private void AdjustListBoxItemHeight()
        {
            _lbSystem.ItemHeight = Font.Height + 2;
            _lbCommon.ItemHeight = Font.Height + 2;
        }

        /// <summary>
        ///  Takes the given color and looks for an instance in the ColorValues table.
        /// </summary>
        private Color GetBestColor(Color color)
        {
            Color[] colors = ColorValues;
            int rgb = color.ToArgb();
            for (int i = 0; i < colors.Length; i++)
            {
                if (colors[i].ToArgb() == rgb)
                {
                    return colors[i];
                }
            }

            return color;
        }

        /// <summary>
        ///  Retrieves an array of color constants for the given object.
        /// </summary>
        private static Color[] GetConstants(Type enumType)
        {
            MethodAttributes attrs = MethodAttributes.Public | MethodAttributes.Static;
            PropertyInfo[] props = enumType.GetProperties();

            List<Color> colorList = [];

            for (int i = 0; i < props.Length; i++)
            {
                PropertyInfo prop = props[i];
                if (prop.PropertyType != typeof(Color))
                {
                    continue;
                }

                MethodInfo? method = prop.GetGetMethod();
                if (method is null || (method.Attributes & attrs) != attrs)
                {
                    continue;
                }

                if (prop.GetValue(obj: null, index: null) is not Color outColor)
                {
                    continue;
                }

                colorList.Add(outColor);
            }

            return [.. colorList];
        }

        [MemberNotNull(
            nameof(_tabControl),
            nameof(_pal),
            nameof(_paletteTabPage),
            nameof(_commonTabPage),
            nameof(_systemTabPage),
            nameof(_lbSystem),
            nameof(_lbCommon))]
        private void InitializeComponent()
        {
            _paletteTabPage = new TabPage(SR.ColorEditorPaletteTab);
            _commonTabPage = new TabPage(SR.ColorEditorStandardTab);
            _systemTabPage = new TabPage(SR.ColorEditorSystemTab);

            AccessibleName = SR.ColorEditorAccName;

            _tabControl = new ColorEditorTabControl();
            _tabControl.TabPages.Add(_paletteTabPage);
            _tabControl.TabPages.Add(_commonTabPage);
            _tabControl.TabPages.Add(_systemTabPage);
            _tabControl.TabStop = false;
            _tabControl.SelectedTab = _systemTabPage;
            _tabControl.SelectedIndexChanged += OnTabControlSelChange;
            _tabControl.Dock = DockStyle.Fill;
            _tabControl.Resize += OnTabControlResize;

            _lbSystem = new ColorEditorListBox
            {
                DrawMode = DrawMode.OwnerDrawFixed,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false,
                Sorted = false
            };

            _lbSystem.Click += OnListClick;
            _lbSystem.DrawItem += OnListDrawItem;
            _lbSystem.KeyDown += OnListKeyDown;
            _lbSystem.Dock = DockStyle.Fill;
            _lbSystem.FontChanged += OnFontChanged;

            _lbCommon = new ColorEditorListBox
            {
                DrawMode = DrawMode.OwnerDrawFixed,
                BorderStyle = BorderStyle.FixedSingle,
                IntegralHeight = false,
                Sorted = false
            };

            _lbCommon.Click += OnListClick;
            _lbCommon.DrawItem += OnListDrawItem;
            _lbCommon.KeyDown += OnListKeyDown;
            _lbCommon.Dock = DockStyle.Fill;

            Array.Sort(ColorValues, StandardColorComparer.Instance);
            Array.Sort(SystemColorValues, comparer: new SystemColorComparer());

            _lbCommon.Items.Clear();
            foreach (Color color in ColorValues)
            {
                _lbCommon.Items.Add(color);
            }

            _lbSystem.Items.Clear();
            foreach (Color color in SystemColorValues)
            {
                _lbSystem.Items.Add(color);
            }

            _pal = new ColorPalette(this, CustomColors);
            _pal.Picked += OnPalettePick;

            _paletteTabPage.Controls.Add(_pal);
            _systemTabPage.Controls.Add(_lbSystem);
            _commonTabPage.Controls.Add(_lbCommon);

            Controls.Add(_tabControl);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            OnTabControlSelChange(this, EventArgs.Empty);
        }

        private void OnFontChanged(object? sender, EventArgs e)
        {
            _commonHeightSet = _systemHeightSet = false;
        }

        private void OnListClick(object? sender, EventArgs e)
        {
            if (sender is ListBox lb && lb.SelectedItem is Color selectedColor)
            {
                _value = selectedColor;
            }

            _edSvc?.CloseDropDown();
        }

        private void OnListDrawItem(object? sender, DrawItemEventArgs die)
        {
            if (sender is not ListBox lb)
            {
                return;
            }

            Color value = (Color)lb.Items[die.Index];
            Font font = Font;

            if (lb == _lbCommon && !_commonHeightSet)
            {
                lb.ItemHeight = lb.Font.Height;
                _commonHeightSet = true;
            }
            else if (lb == _lbSystem && !_systemHeightSet)
            {
                lb.ItemHeight = lb.Font.Height;
                _systemHeightSet = true;
            }

            Graphics graphics = die.Graphics;
            die.DrawBackground();

            _editor.PaintValue(value, graphics, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 22, die.Bounds.Height - 4));
            graphics.DrawRectangle(SystemPens.WindowText, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 22 - 1, die.Bounds.Height - 4 - 1));
            Brush foreBrush = new SolidBrush(die.ForeColor);
            graphics.DrawString(value.Name, font, foreBrush, die.Bounds.X + 26, die.Bounds.Y);
            foreBrush.Dispose();
        }

        private void OnListKeyDown(object? sender, KeyEventArgs ke)
        {
            if (ke.KeyCode == Keys.Return)
            {
                OnListClick(sender, EventArgs.Empty);
            }
        }

        private void OnPalettePick(object? sender, EventArgs e)
        {
            if (sender is ColorPalette palette)
            {
                _value = GetBestColor(palette.SelectedColor);
            }

            _edSvc?.CloseDropDown();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            AdjustListBoxItemHeight();
            AdjustColorUIHeight();
        }

        private void OnTabControlResize(object? sender, EventArgs e)
        {
            Rectangle rectTabControl = _tabControl.TabPages[0].ClientRectangle;
            Rectangle rectItemSize = _tabControl.GetTabRect(1);
            rectTabControl.Y = 0;
            rectTabControl.Height -= rectTabControl.Y;
            int CMARGIN = 2;
            _lbSystem.SetBounds(CMARGIN, rectTabControl.Y + 2 * CMARGIN,
                               rectTabControl.Width - CMARGIN,
                               _pal.Size.Height - rectItemSize.Height + 2 * CMARGIN);
            _lbCommon.Bounds = _lbSystem.Bounds;
            _pal.Location = new Point(0, rectTabControl.Y);
        }

        private void OnTabControlSelChange(object? sender, EventArgs e)
        {
            TabPage? selectedPage = _tabControl.SelectedTab;

            if (selectedPage is not null && selectedPage.Controls.Count > 0)
            {
                selectedPage.Controls[0].Focus();
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // We treat tab characters as switching tab pages. In most other contexts,
            // ctrl-tab switches tab pages, but I couldn't get that to work, and besides,
            // then there would be nothing for tab to do in this editor.
            if ((keyData & Keys.Alt) == 0
                && (keyData & Keys.Control) == 0
                && (keyData & Keys.KeyCode) == Keys.Tab)
            {
                // Logic taken straight out of TabBase
                bool forward = (keyData & Keys.Shift) == 0;
                int sel = _tabControl.SelectedIndex;
                if (sel != -1)
                {
                    int count = _tabControl.TabPages.Count;
                    sel = forward ? (sel + 1) % count : (sel + count - 1) % count;
                    _tabControl.SelectedTab = _tabControl.TabPages[sel];
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        public void Start(IWindowsFormsEditorService edSvc, object? value)
        {
            _edSvc = edSvc;
            _value = value;

            AdjustColorUIHeight();

            // Now look for the current color so we can select the proper tab.
            if (value is not null)
            {
                Color[] values = ColorValues;
                TabPage selectedTab = _paletteTabPage;

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].Equals(value))
                    {
                        _lbCommon.SelectedItem = value;
                        selectedTab = _commonTabPage;
                        break;
                    }
                }

                if (selectedTab == _paletteTabPage)
                {
                    values = SystemColorValues;
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (values[i].Equals(value))
                        {
                            _lbSystem.SelectedItem = value;
                            selectedTab = _systemTabPage;
                            break;
                        }
                    }
                }

                _tabControl.SelectedTab = selectedTab;
            }
        }
    }
}
