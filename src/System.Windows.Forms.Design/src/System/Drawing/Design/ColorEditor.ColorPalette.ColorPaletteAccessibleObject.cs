// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    private partial class ColorPalette
    {
        public partial class ColorPaletteAccessibleObject : ControlAccessibleObject
        {
            private readonly ColorCellAccessibleObject[] _cells;

            public ColorPaletteAccessibleObject(ColorPalette owner) : base(owner)
            {
                _cells = new ColorCellAccessibleObject[CellsAcross * CellsDown];
            }

            internal ColorPalette? ColorPalette => (ColorPalette?)Owner;

            public override int GetChildCount() => CellsAcross * CellsDown;

            public override AccessibleObject? GetChild(int id)
            {
                if (ColorPalette is not { } palette || id < 0 || id >= CellsAcross * CellsDown)
                {
                    return null;
                }

                return _cells[id] ??= new ColorCellAccessibleObject(this, palette.GetColorFromCell(id), id);
            }

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (ColorPalette is not { } palette || !palette.IsHandleCreated)
                {
                    return base.HitTest(x, y);
                }

                // Convert from screen to client coordinates
                Point point = palette.PointToClient(new(x, y));

                int cell = GetCellFromLocationMouse(point.X, point.Y);
                if (cell != -1)
                {
                    return GetChild(cell);
                }

                return base.HitTest(x, y);
            }
        }
    }
}
