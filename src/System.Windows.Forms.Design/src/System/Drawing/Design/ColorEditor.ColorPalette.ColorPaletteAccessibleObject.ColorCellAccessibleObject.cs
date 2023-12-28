// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    private partial class ColorPalette
    {
        public partial class ColorPaletteAccessibleObject
        {
            public class ColorCellAccessibleObject : AccessibleObject
            {
                private readonly Color _color;
                private readonly ColorPaletteAccessibleObject _parent;
                private readonly int _cell;

                public ColorCellAccessibleObject(ColorPaletteAccessibleObject parent, Color color, int cell)
                {
                    _color = color;
                    _parent = parent;
                    _cell = cell;
                }

                public override Rectangle Bounds
                {
                    get
                    {
                        Point cellPt = Get2DFrom1D(_cell);
                        Rectangle rect = default;
                        FillRectWithCellBounds(cellPt.X, cellPt.Y, ref rect);

                        // Translate rect to screen coordinates
                        Point pt = new(rect.X, rect.Y);

                        if (_parent.ColorPalette is { } palette && palette.IsHandleCreated)
                        {
                            PInvoke.ClientToScreen(palette, ref pt);
                        }

                        return new Rectangle(pt.X, pt.Y, rect.Width, rect.Height);
                    }
                }

                public override string Name => _color.ToString();

                public override AccessibleObject Parent => _parent;

                public override AccessibleRole Role => AccessibleRole.Cell;

                public override AccessibleStates State
                {
                    get
                    {
                        AccessibleStates state = base.State;
                        if (_parent.ColorPalette is { } palette && _cell == palette.FocusedCell)
                        {
                            state |= AccessibleStates.Focused;
                        }

                        return state;
                    }
                }

                public override string Value => _color.ToString();
            }
        }
    }
}
