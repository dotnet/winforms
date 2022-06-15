// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;

namespace System.Drawing.Design
{
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

                internal ColorPalette ColorPalette => (ColorPalette)Owner;

                public override int GetChildCount() => CellsAcross * CellsDown;

                public override AccessibleObject GetChild(int id)
                {
                    if (id < 0 || id >= CellsAcross * CellsDown)
                    {
                        return null;
                    }

                    return _cells[id] ??= new ColorCellAccessibleObject(this, ColorPalette.GetColorFromCell(id), id);
                }

                public override AccessibleObject HitTest(int x, int y)
                {
                    if (!ColorPalette.IsHandleCreated)
                    {
                        return base.HitTest(x, y);
                    }

                    // Convert from screen to client coordinates
                    Point point = ColorPalette.PointToClient(new(x, y));

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
}
