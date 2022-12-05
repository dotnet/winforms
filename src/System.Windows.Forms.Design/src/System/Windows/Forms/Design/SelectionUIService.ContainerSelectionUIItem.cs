// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms.Design
{
    internal sealed partial class SelectionUIService
    {
        private class ContainerSelectionUIItem : SelectionUIItem
        {
            public const int CONTAINER_WIDTH = 13;
            public const int CONTAINER_HEIGHT = 13;

            public ContainerSelectionUIItem(SelectionUIService selUIsvc, object component) : base(selUIsvc, component)
            {
            }

            public override Cursor GetCursorAtPoint(Point pt)
            {
                if ((GetHitTest(pt) & CONTAINER_SELECTOR) != 0 && (GetRules() & SelectionRules.Moveable) != SelectionRules.None)
                {
                    return Cursors.SizeAll;
                }
                else
                {
                    return null;
                }
            }

            public override int GetHitTest(Point pt)
            {
                int ht = NOHIT;
                if ((GetRules() & SelectionRules.Visible) != SelectionRules.None && !_outerRect.IsEmpty)
                {
                    Rectangle r = new Rectangle(_outerRect.X, _outerRect.Y, CONTAINER_WIDTH, CONTAINER_HEIGHT);

                    if (r.Contains(pt))
                    {
                        ht = CONTAINER_SELECTOR;
                        if ((GetRules() & SelectionRules.Moveable) != SelectionRules.None)
                        {
                            ht |= MOVE_X | MOVE_Y;
                        }
                    }
                }

                return ht;
            }

            public override void DoPaint(Graphics gr)
            {
                // If we're not visible, then there's nothing to do...
                if ((GetRules() & SelectionRules.Visible) == SelectionRules.None)
                {
                    return;
                }

                Rectangle glyphBounds = new Rectangle(_outerRect.X, _outerRect.Y, CONTAINER_WIDTH, CONTAINER_HEIGHT);
                ControlPaint.DrawContainerGrabHandle(gr, glyphBounds);
            }

            public override Region GetRegion()
            {
                if (_region is null)
                {
                    if ((GetRules() & SelectionRules.Visible) != SelectionRules.None && !_outerRect.IsEmpty)
                    {
                        Rectangle r = new Rectangle(_outerRect.X, _outerRect.Y, CONTAINER_WIDTH, CONTAINER_HEIGHT);
                        _region = new Region(r);
                    }
                    else
                    {
                        _region = new Region(new Rectangle(0, 0, 0, 0));
                    }
                }

                return _region;
            }
        }
    }
}
