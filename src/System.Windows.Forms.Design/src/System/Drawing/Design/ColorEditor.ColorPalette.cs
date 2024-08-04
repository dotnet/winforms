// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    /// <summary>
    ///  A control to display the color palette.
    /// </summary>
    private partial class ColorPalette : Control
    {
        public const int CellsAcross = 8;
        public const int CellsDown = 8;
        public const int CellsCustom = 16; // last cells
        public const int TotalCells = CellsAcross * CellsDown;
        public const int CellSize = 16;
        public const int MarginWidth = 8;

        private static bool s_isScalingInitialized;
        private static int s_cellSizeX = CellSize;
        private static int s_cellSizeY = CellSize;
        private static int s_marginX = MarginWidth;
        private static int s_marginY = MarginWidth;

        private static readonly int[] s_staticCells =
        [
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
        ];

        private readonly Color[] _staticColors;
        private Color _selectedColor;
        private Point _focus;
        protected EventHandler? _onPicked;
        private readonly ColorUI _colorUI;

        public ColorPalette(ColorUI colorUI, Color[] customColors)
        {
            if (!s_isScalingInitialized)
            {
                s_cellSizeX = ScaleHelper.ScaleToInitialSystemDpi(CellSize);
                s_cellSizeY = ScaleHelper.ScaleToInitialSystemDpi(CellSize);
                s_marginX = ScaleHelper.ScaleToInitialSystemDpi(MarginWidth);
                s_marginY = ScaleHelper.ScaleToInitialSystemDpi(MarginWidth);

                s_isScalingInitialized = true;
            }

            _colorUI = colorUI;
            SetStyle(ControlStyles.Opaque, true);

            BackColor = SystemColors.Control;

            Size = new Size(
                CellsAcross * (s_cellSizeX + s_marginX) + s_marginX + 2,
                CellsDown * (s_cellSizeY + s_marginY) + s_marginY + 2);

            _staticColors = new Color[TotalCells - CellsCustom];

            for (int i = 0; i < s_staticCells.Length; i++)
            {
                _staticColors[i] = ColorTranslator.FromOle(s_staticCells[i]);
            }

            CustomColors = customColors;
        }

        public Color[] CustomColors { get; }

        internal int FocusedCell => Get1DFrom2D(_focus);

        public Color SelectedColor
        {
            get => _selectedColor;
            set
            {
                if (!value.Equals(_selectedColor))
                {
                    InvalidateSelection();
                    _selectedColor = value;

                    SetFocus(GetCellFromColor(value));
                    InvalidateSelection();
                }
            }
        }

        public event EventHandler Picked
        {
            add => _onPicked += value;
            remove => _onPicked -= value;
        }

        protected override AccessibleObject CreateAccessibilityInstance() => new ColorPaletteAccessibleObject(this);

        protected EventHandler? Get_onPicked()
        {
            return _onPicked;
        }

        protected void OnPicked(EventArgs e, EventHandler? onPicked) => onPicked?.Invoke(this, e);

        private static void FillRectWithCellBounds(int across, int down, ref Rectangle rect)
        {
            rect.X = s_marginX + across * (s_cellSizeX + s_marginX);
            rect.Y = s_marginY + down * (s_cellSizeY + s_marginY);
            rect.Width = s_cellSizeX;
            rect.Height = s_cellSizeY;
        }

        private Point GetCellFromColor(Color c)
        {
            for (int y = 0; y < CellsDown; y++)
            {
                for (int x = 0; x < CellsAcross; x++)
                {
                    if (GetColorFromCell(x, y).Equals(c))
                    {
                        return new Point(x, y);
                    }
                }
            }

            return Point.Empty;
        }

        private Color GetColorFromCell(int across, int down) => GetColorFromCell(Get1DFrom2D(across, down));

        private Color GetColorFromCell(int index)
        {
            if (index < TotalCells - CellsCustom)
            {
                return _staticColors[index];
            }

            return CustomColors[index - TotalCells + CellsCustom];
        }

        private static Point GetCell2DFromLocationMouse(int x, int y)
        {
            int across = x / (s_cellSizeX + s_marginX);
            int down = y / (s_cellSizeY + s_marginY);

            // Check if we're outside the cells
            if (across < 0 || down < 0 || across >= CellsAcross || down >= CellsDown)
            {
                return new Point(-1, -1);
            }

            // Check if we're in the margin
            if ((x - (s_cellSizeX + s_marginX) * across) < s_marginX
                || (y - (s_cellSizeY + s_marginY) * down) < s_marginY)
            {
                return new Point(-1, -1);
            }

            return new Point(across, down);
        }

        private static int GetCellFromLocationMouse(int x, int y) => Get1DFrom2D(GetCell2DFromLocationMouse(x, y));

        private static int Get1DFrom2D(Point pt) => Get1DFrom2D(pt.X, pt.Y);

        private static int Get1DFrom2D(int x, int y)
        {
            if (x == -1 || y == -1)
            {
                return -1;
            }

            return x + CellsAcross * y;
        }

        private static Point Get2DFrom1D(int cell)
        {
            int x = cell % CellsAcross;
            int y = cell / CellsAcross;
            return new Point(x, y);
        }

        private void InvalidateSelection()
        {
            for (int y = 0; y < CellsDown; y++)
            {
                for (int x = 0; x < CellsAcross; x++)
                {
                    if (SelectedColor.Equals(GetColorFromCell(x, y)))
                    {
                        Rectangle r = default;
                        FillRectWithCellBounds(x, y, ref r);
                        Invalidate(Rectangle.Inflate(r, 5, 5));
                        break;
                    }
                }
            }
        }

        private void InvalidateFocus()
        {
            Rectangle r = default;
            FillRectWithCellBounds(_focus.X, _focus.Y, ref r);
            Invalidate(Rectangle.Inflate(r, 5, 5));
            PInvoke.NotifyWinEvent(
                (uint)AccessibleEvents.Focus,
                this,
                (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                1 + Get1DFrom2D(_focus.X, _focus.Y));
        }

        protected override bool IsInputKey(Keys keyData) => keyData switch
        {
            Keys.Left or Keys.Right or Keys.Up or Keys.Down or Keys.Enter => true,
            // If we don't do this in ProcessDialogKey, VS will take it from us
            Keys.F2 => false,
            _ => base.IsInputKey(keyData),
        };

        protected virtual void LaunchDialog(int customIndex)
        {
            Invalidate();
            _colorUI.EditorService!.CloseDropDown(); // It will be closed anyway as soon as it sees the WM_ACTIVATE
            CustomColorDialog dialog = new();

            HWND hwndFocus = PInvoke.GetFocus();
            try
            {
                DialogResult result = dialog.ShowDialog();
                if (result != DialogResult.Cancel)
                {
                    CustomColors[customIndex] = dialog.Color;
                    SelectedColor = CustomColors[customIndex];
                    OnPicked(EventArgs.Empty, Get_onPicked());
                }

                dialog.Dispose();
            }
            finally
            {
                if (!hwndFocus.IsNull)
                {
                    PInvoke.SetFocus(hwndFocus);
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
                    SelectedColor = GetColorFromCell(_focus.X, _focus.Y);
                    InvalidateFocus();
                    OnPicked(EventArgs.Empty, Get_onPicked());
                    break;
                case Keys.Space:
                    SelectedColor = GetColorFromCell(_focus.X, _focus.Y);
                    InvalidateFocus();
                    break;
                case Keys.Left:
                    SetFocus(_focus with { X = _focus.X - 1 });
                    break;
                case Keys.Right:
                    SetFocus(_focus with { X = _focus.X + 1 });
                    break;
                case Keys.Up:
                    SetFocus(_focus with { Y = _focus.Y - 1 });
                    break;
                case Keys.Down:
                    SetFocus(_focus with { Y = _focus.Y + 1 });
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

                if (cell2D.X != -1 && cell2D.Y != -1 && cell2D != _focus)
                {
                    SetFocus(cell2D);
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs me)
        {
            base.OnMouseMove(me);
            if (me.Button == MouseButtons.Left && Bounds.Contains(me.X, me.Y))
            {
                Point cell2D = GetCell2DFromLocationMouse(me.X, me.Y);

                if (cell2D.X != -1 && cell2D.Y != -1 && cell2D != _focus)
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
                    SelectedColor = GetColorFromCell(_focus.X, _focus.Y);
                    OnPicked(EventArgs.Empty, Get_onPicked());
                }
            }
            else if (me.Button == MouseButtons.Right)
            {
                int cell = GetCellFromLocationMouse(me.X, me.Y);
                if (cell is >= TotalCells - CellsCustom and < TotalCells)
                {
                    LaunchDialog(cell - TotalCells + CellsCustom);
                }
            }
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            Graphics graphics = pe.Graphics;
            using var brush = BackColor.GetCachedSolidBrushScope();
            graphics.FillRectangle(brush, ClientRectangle);

            Rectangle rect = new()
            {
                Width = s_cellSizeX,
                Height = s_cellSizeY,
                X = s_marginX,
                Y = s_marginY
            };

            bool drawSelected = false;

            for (int y = 0; y < CellsDown; y++)
            {
                for (int x = 0; x < CellsAcross; x++)
                {
                    Color color = GetColorFromCell(Get1DFrom2D(x, y));

                    FillRectWithCellBounds(x, y, ref rect);

                    if (color.Equals(SelectedColor) && !drawSelected)
                    {
                        ControlPaint.DrawBorder(
                            graphics,
                            Rectangle.Inflate(rect, 3, 3),
                            SystemColors.ControlText,
                            ButtonBorderStyle.Solid);

                        drawSelected = true;
                    }

                    if (_focus.X == x && _focus.Y == y && Focused)
                    {
                        ControlPaint.DrawFocusRectangle(
                            graphics,
                            Rectangle.Inflate(rect, 5, 5),
                            SystemColors.ControlText,
                            SystemColors.Control);
                    }

                    ControlPaint.DrawBorder(
                        graphics,
                        Rectangle.Inflate(rect, 2, 2),
                        SystemColors.Control, 2, ButtonBorderStyle.Inset,
                        SystemColors.Control, 2, ButtonBorderStyle.Inset,
                        SystemColors.Control, 2, ButtonBorderStyle.Inset,
                        SystemColors.Control, 2, ButtonBorderStyle.Inset);

                    PaintValue(color, graphics, rect);
                }
            }
        }

        private static void PaintValue(Color color, Graphics g, Rectangle rect)
        {
            using var brush = color.GetCachedSolidBrushScope();
            g.FillRectangle(brush, rect);
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == Keys.F2)
            {
                // No ctrl, alt, shift.
                int cell = Get1DFrom2D(_focus.X, _focus.Y);
                if (cell is >= TotalCells - CellsCustom and < TotalCells)
                {
                    LaunchDialog(cell - TotalCells + CellsCustom);
                    return true;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void SetFocus(Point newFocus)
        {
            // Make sure newFocus is within correct range of cells
            if (newFocus.X < 0)
            {
                newFocus.X = 0;
            }

            if (newFocus.Y < 0)
            {
                newFocus.Y = 0;
            }

            if (newFocus.X >= CellsAcross)
            {
                newFocus.X = CellsAcross - 1;
            }

            if (newFocus.Y >= CellsDown)
            {
                newFocus.Y = CellsDown - 1;
            }

            if (_focus != newFocus)
            {
                InvalidateFocus();
                _focus = newFocus;
                InvalidateFocus();
            }
        }
    }
}
