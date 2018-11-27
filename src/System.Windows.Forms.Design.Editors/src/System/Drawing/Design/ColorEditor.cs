// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.Design.Editors.Resources;

namespace System.Drawing.Design
{
    /// <summary>
    ///     Provides an editor for visually picking a color.
    /// </summary>
    [CLSCompliant(false)]
    public class ColorEditor : UITypeEditor
    {
        private ColorUI colorUI;

        /// <summary>
        ///     Edits the given object value using the editor style
        ///     provided by ColorEditor.GetEditStyle.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            object returnValue = value;

            if (provider != null)
            {
                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null)
                {
                    if (colorUI == null)
                    {
                        colorUI = new ColorUI(this);
                    }
                    colorUI.Start(edSvc, value);
                    edSvc.DropDownControl(colorUI);

                    if (colorUI.Value != null && ((Color)colorUI.Value) != Color.Empty)
                    {
                        value = colorUI.Value;
                    }

                    colorUI.End();
                }
            }

            return value;
        }

        /// <summary>
        ///     Gets the editing style of the Edit method. If the method
        ///     is not supported, this will return UITypeEditorEditStyle.None.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///     Gets a value indicating if this editor supports the painting of a representation
        ///     of an object's value.
        /// </summary>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        /// <summary>
        ///     Paints a representative value of the given object to the provided
        ///     canvas. Painting should be done within the boundaries of the
        ///     provided rectangle.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        [SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")] //Benign code
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override void PaintValue(PaintValueEventArgs e)
        {
            if (e.Value is Color)
            {
                Color color = (Color)e.Value;
                SolidBrush b = new SolidBrush(color);
                e.Graphics.FillRectangle(b, e.Bounds);
                b.Dispose();
            }
        }

        /// <summary>
        ///     A control to display the color palette.
        /// </summary>
        private class ColorPalette : Control
        {
            public const int CELLS_ACROSS = 8;
            public const int CELLS_DOWN = 8;
            public const int CELLS_CUSTOM = 16; // last cells
            public const int CELLS = CELLS_ACROSS * CELLS_DOWN;
            public const int CELL_SIZE = 16;
            public const int MARGIN = 8;

            private static bool isScalingInitialized = false;
            private static int cellSizeX = CELL_SIZE;
            private static int cellSizeY = CELL_SIZE;
            private static int marginX = MARGIN;
            private static int marginY = MARGIN;

            private static readonly int[] staticCells = new int[] {
                0x00ffffff, 0x00c0c0ff, 0x00c0e0ff, 0x00c0ffff,
                0x00c0ffc0, 0x00ffffc0, 0x00ffc0c0, 0x00ffc0ff,

                0x00e0e0e0, 0x008080ff, 0x0080c0ff, 0x0080ffff,
                0x0080ff80, 0x00ffff80, 0x00ff8080, 0x00ff80ff,

                0x00c0c0c0, 0x000000ff, 0x000080ff, 0x0000ffff,
                0x0000ff00, 0x00ffff00, 0x00ff0000, 0x00ff00ff,

                0x00808080, 0x000000c0, 0x000040c0, 0x0000c0c0,
                0x0000c000, 0x00c0c000, 0x00c00000, 0x00c000c0,

                0x00404040, 0x00000080, 0x00004080, 0x00008080,
                0x00008000, 0x00808000, 0x00800000, 0x00800080,

                0x00000000, 0x00000040, 0x00404080, 0x00004040,
                0x00004000, 0x00404000, 0x00400000, 0x00400040
            };

            private readonly Color[] staticColors;
            private Color selectedColor;
            private Point focus = new Point(0, 0);
            private EventHandler onPicked;
            private readonly ColorUI colorUI;

            public ColorPalette(ColorUI colorUI, Color[] customColors)
            {
                
                if (!isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        cellSizeX = DpiHelper.LogicalToDeviceUnitsX(CELL_SIZE);
                        cellSizeY = DpiHelper.LogicalToDeviceUnitsY(CELL_SIZE);
                        marginX = DpiHelper.LogicalToDeviceUnitsX(MARGIN);
                        marginY = DpiHelper.LogicalToDeviceUnitsY(MARGIN);
                    }
                    isScalingInitialized = true;
                }

                this.colorUI = colorUI;
                SetStyle(ControlStyles.Opaque, true);

                BackColor = SystemColors.Control;

                Size = new Size(CELLS_ACROSS * (cellSizeX + marginX) + marginX + 2,
                                CELLS_DOWN * (cellSizeY + marginY) + marginY + 2);

                staticColors = new Color[CELLS - CELLS_CUSTOM];

                for (int i = 0; i < staticCells.Length; i++)
                    staticColors[i] = ColorTranslator.FromOle(staticCells[i]);

                this.CustomColors = customColors;
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public Color[] CustomColors { get; }

            internal int FocusedCell => Get1DFrom2D(focus);

            public Color SelectedColor
            {
                get
                {
                    return selectedColor;
                }
                set
                {
                    if (!value.Equals(selectedColor))
                    {
                        InvalidateSelection();
                        selectedColor = value;

                        SetFocus(GetCellFromColor(value));
                        InvalidateSelection();
                    }
                }
            }

            public event EventHandler Picked
            {
                add => onPicked += value;
                remove => onPicked -= value;
            }

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new ColorPaletteAccessibleObject(this);
            }

            protected void OnPicked(EventArgs e)
            {
                onPicked?.Invoke(this, e);
            }

            private static void FillRectWithCellBounds(int across, int down, ref Rectangle rect)
            {
                rect.X = marginX + across * (cellSizeX + marginX);
                rect.Y = marginY + down * (cellSizeY + marginY);
                rect.Width = cellSizeX;
                rect.Height = cellSizeY;
            }

            private Point GetCellFromColor(Color c)
            {
                for (int y = 0; y < CELLS_DOWN; y++)
                {
                    for (int x = 0; x < CELLS_ACROSS; x++)
                    {
                        if (GetColorFromCell(x, y).Equals(c))
                        {
                            return new Point(x, y);
                        }
                    }
                }
                return Point.Empty;
            }

            private Color GetColorFromCell(int across, int down)
            {
                return GetColorFromCell(Get1DFrom2D(across, down));
            }

            private Color GetColorFromCell(int index)
            {
                if (index < CELLS - CELLS_CUSTOM)
                    return staticColors[index];
                return CustomColors[index - CELLS + CELLS_CUSTOM];
            }

            private static Point GetCell2DFromLocationMouse(int x, int y)
            {
                int across = x / (cellSizeX + marginX);
                int down = y / (cellSizeY + marginY);

                // Check if we're outside the cells
                //
                if (across < 0 || down < 0 || across >= CELLS_ACROSS || down >= CELLS_DOWN)
                {
                    return new Point(-1, -1);
                }

                // Check if we're in the margin
                //
                if ((x - (cellSizeX + marginX) * across) < marginX ||
                    (y - (cellSizeY + marginY) * down) < marginY)
                {
                    return new Point(-1, -1);
                }

                return new Point(across, down);
            }

            private static int GetCellFromLocationMouse(int x, int y)
            {
                return Get1DFrom2D(GetCell2DFromLocationMouse(x, y));
            }

            private static int Get1DFrom2D(Point pt)
            {
                return Get1DFrom2D(pt.X, pt.Y);
            }

            private static int Get1DFrom2D(int x, int y)
            {
                if (x == -1 || y == -1)
                {
                    return -1;
                }
                return x + CELLS_ACROSS * y;
            }

            private static Point Get2DFrom1D(int cell)
            {
                int x = cell % CELLS_ACROSS;
                int y = cell / CELLS_ACROSS;
                return new Point(x, y);
            }

            private void InvalidateSelection()
            {
                for (int y = 0; y < CELLS_DOWN; y++)
                {
                    for (int x = 0; x < CELLS_ACROSS; x++)
                    {
                        if (SelectedColor.Equals(GetColorFromCell(x, y)))
                        {
                            Rectangle r = new Rectangle();
                            FillRectWithCellBounds(x, y, ref r);
                            Invalidate(Rectangle.Inflate(r, 5, 5));
                            break;
                        }
                    }
                }
            }

            private void InvalidateFocus()
            {
                Rectangle r = new Rectangle();
                FillRectWithCellBounds(focus.X, focus.Y, ref r);
                Invalidate(Rectangle.Inflate(r, 5, 5));
                UnsafeNativeMethods.NotifyWinEvent((int)AccessibleEvents.Focus, new HandleRef(this, this.Handle), UnsafeNativeMethods.OBJID_CLIENT, 1 + Get1DFrom2D(focus.X, focus.Y));
            }

            protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
            {
                switch (keyData)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Enter:
                        return true;

                    // If we don't do this in ProcessDialogKey, VS will take it from us (ASURT 37231)
                    case Keys.F2:
                        return false;
                }
                return base.IsInputKey(keyData);
            }

            protected virtual void LaunchDialog(int customIndex)
            {
                Invalidate();
                colorUI.EditorService.CloseDropDown(); // It will be closed anyway as soon as it sees the WM_ACTIVATE
                CustomColorDialog dialog = new CustomColorDialog();

                IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
                try
                {
                    DialogResult result = dialog.ShowDialog();
                    if (result != DialogResult.Cancel)
                    {
                        Color color = dialog.Color;
                        CustomColors[customIndex] = dialog.Color;
                        SelectedColor = CustomColors[customIndex];
                        OnPicked(EventArgs.Empty);
                    }

                    dialog.Dispose();
                }
                finally
                {
                    if (hwndFocus != IntPtr.Zero)
                    {
                        UnsafeNativeMethods.SetFocus(new HandleRef(null, hwndFocus));
                    }
                }
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                InvalidateFocus();
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                base.OnKeyDown(e);
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        SelectedColor = GetColorFromCell(focus.X, focus.Y);
                        InvalidateFocus();
                        OnPicked(EventArgs.Empty);
                        break;
                    case Keys.Space:
                        SelectedColor = GetColorFromCell(focus.X, focus.Y);
                        InvalidateFocus();
                        break;
                    case Keys.Left:
                        SetFocus(new Point(focus.X - 1, focus.Y));
                        break;
                    case Keys.Right:
                        SetFocus(new Point(focus.X + 1, focus.Y));
                        break;
                    case Keys.Up:
                        SetFocus(new Point(focus.X, focus.Y - 1));
                        break;
                    case Keys.Down:
                        SetFocus(new Point(focus.X, focus.Y + 1));
                        break;
                }
            }

            protected override void OnLostFocus(EventArgs e)
            {
                base.OnLostFocus(e);
                InvalidateFocus();
            }

            protected override void OnMouseDown(MouseEventArgs me)
            {
                base.OnMouseDown(me);
                if (me.Button == MouseButtons.Left)
                {
                    Point cell2D = GetCell2DFromLocationMouse(me.X, me.Y);

                    if (cell2D.X != -1 && cell2D.Y != -1 && cell2D != focus)
                    {
                        SetFocus(cell2D);
                    }
                }
            }

            protected override void OnMouseMove(MouseEventArgs me)
            {
                base.OnMouseMove(me);
                if (me.Button == MouseButtons.Left &&
                    Bounds.Contains(me.X, me.Y))
                {
                    Point cell2D = GetCell2DFromLocationMouse(me.X, me.Y);

                    if (cell2D.X != -1 && cell2D.Y != -1 && cell2D != focus)
                    {
                        SetFocus(cell2D);
                    }
                }
            }

            protected override void OnMouseUp(MouseEventArgs me)
            {
                base.OnMouseUp(me);

                if (me.Button == MouseButtons.Left)
                {
                    Point cell2D = GetCell2DFromLocationMouse(me.X, me.Y);
                    if (cell2D.X != -1 && cell2D.Y != -1)
                    {
                        SetFocus(cell2D);
                        SelectedColor = GetColorFromCell(focus.X, focus.Y);
                        OnPicked(EventArgs.Empty);
                    }
                }
                else if (me.Button == MouseButtons.Right)
                {
                    int cell = GetCellFromLocationMouse(me.X, me.Y);
                    if (cell != -1 && cell >= (CELLS - CELLS_CUSTOM) && cell < CELLS)
                    {
                        LaunchDialog(cell - CELLS + CELLS_CUSTOM);
                    }
                }
            }

            protected override void OnPaint(PaintEventArgs pe)
            {
                Graphics g = pe.Graphics;
                using (SolidBrush brush = new SolidBrush(BackColor))
                {
                    g.FillRectangle(brush, ClientRectangle);
                }
                Rectangle rect = new Rectangle();

                rect.Width = cellSizeX;
                rect.Height = cellSizeY;
                rect.X = marginX;
                rect.Y = marginY;
                bool drawSelected = false;

                for (int y = 0; y < CELLS_DOWN; y++)
                {
                    for (int x = 0; x < CELLS_ACROSS; x++)
                    {
                        Color cur = GetColorFromCell(Get1DFrom2D(x, y));

                        FillRectWithCellBounds(x, y, ref rect);

                        if (cur.Equals(SelectedColor) && !drawSelected)
                        {
                            ControlPaint.DrawBorder(g, Rectangle.Inflate(rect, 3, 3), SystemColors.ControlText, ButtonBorderStyle.Solid);
                            drawSelected = true;
                        }

                        if (focus.X == x && focus.Y == y && Focused)
                        {
                            ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(rect, 5, 5), SystemColors.ControlText, SystemColors.Control);
                        }

                        ControlPaint.DrawBorder(g, Rectangle.Inflate(rect, 2, 2),
                                                SystemColors.Control, 2, ButtonBorderStyle.Inset,
                                                SystemColors.Control, 2, ButtonBorderStyle.Inset,
                                                SystemColors.Control, 2, ButtonBorderStyle.Inset,
                                                SystemColors.Control, 2, ButtonBorderStyle.Inset);
                        PaintValue(cur, g, rect);
                    }
                }
            }

            private static void PaintValue(Color color, Graphics g, Rectangle rect)
            {
                using (SolidBrush brush = new SolidBrush(color))
                {
                    g.FillRectangle(brush, rect);
                }
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if (keyData == Keys.F2)
                { // no ctrl, alt, shift...
                    int cell = Get1DFrom2D(focus.X, focus.Y);
                    if (cell >= (CELLS - CELLS_CUSTOM) && cell < CELLS)
                    {
                        LaunchDialog(cell - CELLS + CELLS_CUSTOM);
                        return true;
                    }
                }

                return base.ProcessDialogKey(keyData);
            }

            private void SetFocus(Point newFocus)
            {
                // Make sure newFocus is within correct range of cells
                //
                if (newFocus.X < 0)
                {
                    newFocus.X = 0;
                }
                if (newFocus.Y < 0)
                {
                    newFocus.Y = 0;
                }
                if (newFocus.X >= CELLS_ACROSS)
                {
                    newFocus.X = CELLS_ACROSS - 1;
                }
                if (newFocus.Y >= CELLS_DOWN)
                {
                    newFocus.Y = CELLS_DOWN - 1;
                }

                if (focus != newFocus)
                {
                    InvalidateFocus();
                    focus = newFocus;
                    InvalidateFocus();
                }
            }

            [ComVisible(true)]
            public class ColorPaletteAccessibleObject : ControlAccessibleObject
            {
                private readonly ColorCellAccessibleObject[] cells;

                public ColorPaletteAccessibleObject(ColorPalette owner) : base(owner)
                {
                    cells = new ColorCellAccessibleObject[CELLS_ACROSS * CELLS_DOWN];
                }

                internal ColorPalette ColorPalette => (ColorPalette)Owner;

                public override int GetChildCount()
                {
                    return CELLS_ACROSS * CELLS_DOWN;
                }

                public override AccessibleObject GetChild(int id)
                {
                    if (id < 0 || id >= CELLS_ACROSS * CELLS_DOWN)
                    {
                        return null;
                    }

                    if (cells[id] == null)
                    {
                        cells[id] = new ColorCellAccessibleObject(this, ColorPalette.GetColorFromCell(id), id);
                    }
                    return cells[id];
                }

                public override AccessibleObject HitTest(int x, int y)
                {
                    // Convert from screen to client coordinates
                    //
                    NativeMethods.POINT pt = new NativeMethods.POINT(x, y);
                    UnsafeNativeMethods.ScreenToClient(new HandleRef(ColorPalette, ColorPalette.Handle), pt);

                    int cell = ColorPalette.GetCellFromLocationMouse(pt.x, pt.y);
                    if (cell != -1)
                    {
                        return GetChild(cell);
                    }

                    return base.HitTest(x, y);
                }

                [ComVisible(true)]
                public class ColorCellAccessibleObject : AccessibleObject
                {
                    private readonly Color color;
                    private readonly ColorPaletteAccessibleObject parent;
                    private readonly int cell;

                    public ColorCellAccessibleObject(ColorPaletteAccessibleObject parent, Color color, int cell)
                    {
                        this.color = color;
                        this.parent = parent;
                        this.cell = cell;
                    }

                    public override Rectangle Bounds
                    {
                        get
                        {
                            Point cellPt = ColorPalette.Get2DFrom1D(cell);
                            Rectangle rect = new Rectangle();
                            ColorPalette.FillRectWithCellBounds(cellPt.X, cellPt.Y, ref rect);

                            // Translate rect to screen coordinates
                            //
                            NativeMethods.POINT pt = new NativeMethods.POINT(rect.X, rect.Y);
                            UnsafeNativeMethods.ClientToScreen(new HandleRef(parent.ColorPalette, parent.ColorPalette.Handle), pt);

                            return new Rectangle(pt.x, pt.y, rect.Width, rect.Height);
                        }
                    }

                    public override string Name => color.ToString();

                    public override AccessibleObject Parent => parent;

                    public override AccessibleRole Role => AccessibleRole.Cell;

                    public override AccessibleStates State
                    {
                        get
                        {
                            AccessibleStates state = base.State;
                            if (cell == parent.ColorPalette.FocusedCell)
                            {
                                state |= AccessibleStates.Focused;
                            }
                            return state;
                        }
                    }
                    public override string Value => color.ToString();
                }
            }
        }

        /// <summary>
        ///      Editor UI for the color editor.
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
            /// Array of standard colors.
            /// </summary>
            private object[] ColorValues => colorConstants ?? (colorConstants = GetConstants(typeof(Color)));

            /// <summary>
            /// Retrieves the array of custom colors for our use.
            /// </summary>
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            private Color[] CustomColors
            {
                get
                {
                    if (customColors == null)
                    {
                        customColors = new Color[ColorPalette.CELLS_CUSTOM];
                        for (int i = 0; i < ColorPalette.CELLS_CUSTOM; i++)
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
            /// Allows someone else to close our dropdown.
            /// </summary>
            public IWindowsFormsEditorService EditorService => edSvc;

            /// <summary> 
            /// Array of system colors.
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
                this.Size = new Size(size.Width + 2 * CMARGIN, size.Height + 2 * CMARGIN + rectItemSize.Height);
                tabControl.Size = this.Size;
            }

            private void AdjustListBoxItemHeight()
            {
                lbSystem.ItemHeight = Font.Height + 2;
                lbCommon.ItemHeight = Font.Height + 2;
            }

            /// <summary>
            /// Takes the given color and looks for an instance in the ColorValues table.
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
            /// Retrieves an array of color constants for the given object.
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
                tabControl.SelectedIndexChanged += new EventHandler(this.OnTabControlSelChange);
                tabControl.Dock = DockStyle.Fill;
                tabControl.Resize += new EventHandler(this.OnTabControlResize);

                lbSystem = new ColorEditorListBox();
                lbSystem.DrawMode = DrawMode.OwnerDrawFixed;
                lbSystem.BorderStyle = BorderStyle.FixedSingle;
                lbSystem.IntegralHeight = false;
                lbSystem.Sorted = false;
                lbSystem.Click += new EventHandler(this.OnListClick);
                lbSystem.DrawItem += new DrawItemEventHandler(this.OnListDrawItem);
                lbSystem.KeyDown += new KeyEventHandler(this.OnListKeyDown);
                lbSystem.Dock = DockStyle.Fill;
                lbSystem.FontChanged += new EventHandler(this.OnFontChanged);

                lbCommon = new ColorEditorListBox();
                lbCommon.DrawMode = DrawMode.OwnerDrawFixed;
                lbCommon.BorderStyle = BorderStyle.FixedSingle;
                lbCommon.IntegralHeight = false;
                lbCommon.Sorted = false;
                lbCommon.Click += new EventHandler(this.OnListClick);
                lbCommon.DrawItem += new DrawItemEventHandler(this.OnListDrawItem);
                lbCommon.KeyDown += new KeyEventHandler(this.OnListKeyDown);
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
                pal.Picked += new EventHandler(this.OnPalettePick);

                paletteTabPage.Controls.Add(pal);
                systemTabPage.Controls.Add(lbSystem);
                commonTabPage.Controls.Add(lbCommon);

                this.Controls.Add(tabControl);
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

            [SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
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
                            sel = (sel + 1) % count;
                        else
                            sel = (sel + count - 1) % count;
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
                protected override bool IsInputKey(System.Windows.Forms.Keys keyData)
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
                    TabPage selectedTab = this.SelectedTab;
                    if (selectedTab != null && selectedTab.Controls.Count > 0)
                    {
                        selectedTab.Controls[0].Focus();
                    }
                }
            }
        }

        private class CustomColorDialog : ColorDialog
        {
            private const int COLOR_HUE = 703;
            private const int COLOR_SAT = 704;
            private const int COLOR_LUM = 705;
            private const int COLOR_RED = 706;
            private const int COLOR_GREEN = 707;
            private const int COLOR_BLUE = 708;
            private const int COLOR_ADD = 712;
            private const int COLOR_MIX = 719;

            private IntPtr hInstance;

            public CustomColorDialog()
            {
                // colordlg.data was copied from VB6's dlg-4300.dlg
                Stream stream = typeof(ColorEditor).Module.Assembly.GetManifestResourceStream(typeof(ColorEditor), "colordlg.data");

                int size = (int)(stream.Length - stream.Position);
                byte[] buffer = new byte[size];
                stream.Read(buffer, 0, size);

                hInstance = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, 0, hInstance, size);
            }

            protected override IntPtr Instance
            {
                get
                {
                    Debug.Assert(hInstance != IntPtr.Zero, "Dialog has been disposed");
                    return hInstance;
                }
            }

            protected override int Options => NativeMethods.CC_FULLOPEN | NativeMethods.CC_ENABLETEMPLATEHANDLE;

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (hInstance != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(hInstance);
                        hInstance = IntPtr.Zero;
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }

            protected override IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                switch (msg)
                {
                    case NativeMethods.WM_INITDIALOG:
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_HUE, NativeMethods.EM_SETMARGINS, (IntPtr)(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN), IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_SAT, NativeMethods.EM_SETMARGINS, (IntPtr)(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN), IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_LUM, NativeMethods.EM_SETMARGINS, (IntPtr)(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN), IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_RED, NativeMethods.EM_SETMARGINS, (IntPtr)(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN), IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_GREEN, NativeMethods.EM_SETMARGINS, (IntPtr)(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN), IntPtr.Zero);
                        NativeMethods.SendDlgItemMessage(hwnd, COLOR_BLUE, NativeMethods.EM_SETMARGINS, (IntPtr)(NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN), IntPtr.Zero);
                        IntPtr hwndCtl = NativeMethods.GetDlgItem(hwnd, COLOR_MIX);
                        NativeMethods.EnableWindow(hwndCtl, false);
                        NativeMethods.SetWindowPos(hwndCtl, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_HIDEWINDOW);
                        hwndCtl = NativeMethods.GetDlgItem(hwnd, NativeMethods.IDOK);
                        NativeMethods.EnableWindow(hwndCtl, false);
                        NativeMethods.SetWindowPos(hwndCtl, IntPtr.Zero, 0, 0, 0, 0, NativeMethods.SWP_HIDEWINDOW);
                        this.Color = Color.Empty;
                        break;
                    case NativeMethods.WM_COMMAND:
                        switch (NativeMethods.Util.LOWORD(unchecked((int)(long)wParam)))
                        {
                            case COLOR_ADD:
                                byte red, green, blue;
                                bool[] err = new bool[1];
                                red = (byte)NativeMethods.GetDlgItemInt(hwnd, COLOR_RED, err, false);
                                Debug.Assert(!err[0], "Couldn't find dialog member COLOR_RED");
                                green = (byte)NativeMethods.GetDlgItemInt(hwnd, COLOR_GREEN, err, false);
                                Debug.Assert(!err[0], "Couldn't find dialog member COLOR_GREEN");
                                blue = (byte)NativeMethods.GetDlgItemInt(hwnd, COLOR_BLUE, err, false);
                                Debug.Assert(!err[0], "Couldn't find dialog member COLOR_BLUE");
                                this.Color = Color.FromArgb(red, green, blue);
                                NativeMethods.PostMessage(hwnd, NativeMethods.WM_COMMAND, (IntPtr)NativeMethods.Util.MAKELONG(NativeMethods.IDOK, 0), NativeMethods.GetDlgItem(hwnd, NativeMethods.IDOK));
                                break;
                        }
                        break;
                }
                return base.HookProc(hwnd, msg, wParam, lParam);
            }
        }

        /// <summary>
        /// Comparer for system colors.
        /// </summary>
        private class SystemColorComparer : IComparer
        {
            [SuppressMessage("Microsoft.Globalization", "CA130:UseOrdinalStringComparison")]
            public int Compare(object x, object y)
            {
                return String.Compare(((Color)x).Name, ((Color)y).Name, false, CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// Comparer for standard colors
        /// </summary>
        private class StandardColorComparer : IComparer
        {
            public int Compare(object x, object y)
            {
                Color left = (Color)x;
                Color right = (Color)y;

                if (left.A < right.A)
                {
                    return -1;
                }

                if (left.A > right.A)
                {
                    return 1;
                }

                if ((float)left.GetHue() < (float)right.GetHue())
                {
                    return -1;
                }

                if ((float)left.GetHue() > (float)right.GetHue())
                {
                    return 1;
                }

                if ((float)left.GetSaturation() < (float)right.GetSaturation())
                {
                    return -1;
                }

                if ((float)left.GetSaturation() > (float)right.GetSaturation())
                {
                    return 1;
                }

                if ((float)left.GetBrightness() < (float)right.GetBrightness())
                {
                    return -1;
                }

                if ((float)left.GetBrightness() > (float)right.GetBrightness())
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
