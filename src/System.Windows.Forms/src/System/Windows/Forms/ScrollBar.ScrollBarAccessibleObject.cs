// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ScrollBar
    {
        internal class ScrollBarAccessibleObject : ControlAccessibleObject
        {
            private readonly ScrollBar _owningScrollBar;
            private ScrollBarFirstLineButtonAccessibleObject? _firstLineButtonAccessibleObject;
            private ScrollBarFirstPageButtonAccessibleObject? _firstPageButtonAccessibleObject;
            private ScrollBarLastLineButtonAccessibleObject? _lastLineButtonAccessibleObject;
            private ScrollBarLastPageButtonAccessibleObject? _lastPageButtonAccessibleObject;
            private ScrollBarThumbAccessibleObject? _thumbAccessibleObject;

            internal ScrollBarAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
            {
                _owningScrollBar = owningScrollBar;
            }

            internal ScrollBarFirstLineButtonAccessibleObject FirstLineButtonAccessibleObject
                => _firstLineButtonAccessibleObject ??= new(_owningScrollBar);

            internal ScrollBarFirstPageButtonAccessibleObject FirstPageButtonAccessibleObject
                => _firstPageButtonAccessibleObject ??= new(_owningScrollBar);

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            internal ScrollBarLastLineButtonAccessibleObject LastLineButtonAccessibleObject
                => _lastLineButtonAccessibleObject ??= new(_owningScrollBar);

            internal ScrollBarLastPageButtonAccessibleObject LastPageButtonAccessibleObject
                => _lastPageButtonAccessibleObject ??= new(_owningScrollBar);

            internal ScrollBarThumbAccessibleObject ThumbAccessibleObject
                => _thumbAccessibleObject ??= new(_owningScrollBar);

            // The maximum value can only be reached programmatically. The value of a scroll bar cannot reach its maximum
            // value through user interaction at run time. The maximum value that can be reached through user interaction
            // is equal to 1 plus the Maximum property value minus the LargeChange property value.
            internal int UIMaximum => _owningScrollBar.Maximum - _owningScrollBar.LargeChange + 1;

            private bool ArePageButtonsDisplayed
                => FirstPageButtonAccessibleObject.IsDisplayed && LastPageButtonAccessibleObject.IsDisplayed;

            private bool ArePageButtonsHidden
                => !FirstPageButtonAccessibleObject.IsDisplayed && !LastPageButtonAccessibleObject.IsDisplayed;

            public override AccessibleObject? GetChild(int index)
            {
                if (!_owningScrollBar.IsHandleCreated)
                {
                    return null;
                }

                return index switch
                {
                    0 => FirstLineButtonAccessibleObject,
                    1 => FirstPageButtonAccessibleObject.IsDisplayed ? FirstPageButtonAccessibleObject : ThumbAccessibleObject,
                    2 => FirstPageButtonAccessibleObject.IsDisplayed
                        ? ThumbAccessibleObject
                        : ArePageButtonsHidden ? LastLineButtonAccessibleObject : LastPageButtonAccessibleObject,
                    3 => ArePageButtonsDisplayed
                        ? LastPageButtonAccessibleObject
                        : ArePageButtonsHidden ? null : LastLineButtonAccessibleObject,
                    4 => ArePageButtonsDisplayed ? LastLineButtonAccessibleObject : null,
                    _ => null
                };
            }

            public override int GetChildCount()
                => _owningScrollBar.IsHandleCreated
                    ? ArePageButtonsDisplayed
                        ? 5
                        : ArePageButtonsHidden ? 3 : 4
                    : -1;

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningScrollBar.IsHandleCreated)
                {
                    return null;
                }

                Point point = new(x, y);
                if (ThumbAccessibleObject.Bounds.Contains(point))
                {
                    return ThumbAccessibleObject;
                }

                if (FirstLineButtonAccessibleObject.Bounds.Contains(point))
                {
                    return FirstLineButtonAccessibleObject;
                }

                if (FirstPageButtonAccessibleObject.Bounds.Contains(point))
                {
                    return FirstPageButtonAccessibleObject;
                }

                if (LastPageButtonAccessibleObject.Bounds.Contains(point))
                {
                    return LastPageButtonAccessibleObject;
                }

                if (LastLineButtonAccessibleObject.Bounds.Contains(point))
                {
                    return LastLineButtonAccessibleObject;
                }

                return null;
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject? element = HitTest((int)x, (int)y);

                return element ?? base.ElementProviderFromPoint(x, y);
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => FirstLineButtonAccessibleObject,
                    UiaCore.NavigateDirection.LastChild => LastLineButtonAccessibleObject,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UiaCore.UIA.ControlTypePropertyId when
                        _owningScrollBar.AccessibleRole == AccessibleRole.Default
                        => UiaCore.UIA.ScrollBarControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owningScrollBar.Focused,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }
        }
    }
}
