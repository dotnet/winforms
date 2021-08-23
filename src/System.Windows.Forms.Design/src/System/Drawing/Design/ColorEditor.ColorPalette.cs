// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Interop;
using static Interop.User32;

namespace System.Drawing.Design
{
    public partial class ColorEditor
    {
        /// <summary>
        ///  A control to display the color palette.
        /// </summary>
        private class ColorPalette : Control
        {
            public const int CELLS_ACROSS = 8;
            public const int CELLS_DOWN = 8;
            public const int CELLS_CUSTOM = 16; // last cells
            public const int CELLS = CELLS_ACROSS * CELLS_DOWN;
            public const int CELL_SIZE = 16;
            public const int MARGIN = 8;

            private static bool isScalingInitialized;
            private static int cellSizeX = CELL_SIZE;
            private static int cellSizeY = CELL_SIZE;
            private static int marginX = MARGIN;
            private static int marginY = MARGIN;

            private static readonly int[] staticCells = new int[]
            {
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
                {
                    staticColors[i] = ColorTranslator.FromOle(staticCells[i]);
                }

                CustomColors = customColors;
            }

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
                {
                    return staticColors[index];
                }

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
                NotifyWinEvent((uint)AccessibleEvents.Focus, new HandleRef(this, Handle), OBJID.CLIENT, 1 + Get1DFrom2D(focus.X, focus.Y));
            }

            protected override bool IsInputKey(Keys keyData)
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

                IntPtr hwndFocus = GetFocus();
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
                        User32.SetFocus(hwndFocus);
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

                Rectangle rect = new Rectangle
                {
                    Width = cellSizeX,
                    Height = cellSizeY,
                    X = marginX,
                    Y = marginY
                };
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

                    if (cells[id] is null)
                    {
                        cells[id] = new ColorCellAccessibleObject(this, ColorPalette.GetColorFromCell(id), id);
                    }

                    return cells[id];
                }

                public override AccessibleObject HitTest(int x, int y)
                {
                    if (!ColorPalette.IsHandleCreated)
                    {
                        return base.HitTest(x, y);
                    }

                    // Convert from screen to client coordinates
                    var pt = new Point(x, y);

                    ScreenToClient(new HandleRef(ColorPalette, ColorPalette.Handle), ref pt);

                    int cell = ColorPalette.GetCellFromLocationMouse(pt.X, pt.Y);
                    if (cell != -1)
                    {
                        return GetChild(cell);
                    }

                    return base.HitTest(x, y);
                }

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
                            var pt = new Point(rect.X, rect.Y);
                            var palette = parent.ColorPalette;

                            if (palette.IsHandleCreated)
                            {
                                ClientToScreen(new HandleRef(palette, palette.Handle), ref pt);
                            }

                            return new Rectangle(pt.X, pt.Y, rect.Width, rect.Height);
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
    }
}
