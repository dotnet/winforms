// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class LinkLabel
    {
        internal class LinkLabelAccessibleObject : LabelAccessibleObject
        {
            private readonly LinkLabel _owningLinkLabel;

            public LinkLabelAccessibleObject(LinkLabel owner) : base(owner)
            {
                _owningLinkLabel = owner;
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            {
                if (!_owningLinkLabel.IsHandleCreated)
                {
                    return base.ElementProviderFromPoint(x, y);
                }

                return HitTest((int)x, (int)y) ?? base.ElementProviderFromPoint(x, y);
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.FirstChild
                        => _owningLinkLabel.Links.Count != 0
                            ? _owningLinkLabel.Links[0].AccessibleObject
                            : null,
                    UiaCore.NavigateDirection.LastChild
                        => _owningLinkLabel.Links.Count != 0
                            ? _owningLinkLabel.Links[^1].AccessibleObject
                            : null,
                    _ => base.FragmentNavigate(direction),
                };

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            public override AccessibleObject? GetChild(int index)
            {
                if (index >= 0 && index < GetChildCount())
                {
                    return _owningLinkLabel.Links[index].AccessibleObject;
                }

                return null;
            }

            public override int GetChildCount() => _owningLinkLabel.Links.Count;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override AccessibleObject? HitTest(int x, int y)
            {
                if (!_owningLinkLabel.IsHandleCreated)
                {
                    return null;
                }

                Point p = Owner.PointToClient(new Point(x, y));
                Link? hit = _owningLinkLabel.PointInLink(p.X, p.Y);

                if (hit is not null)
                {
                    return hit.AccessibleObject;
                }

                if (Bounds.Contains(x, y))
                {
                    return this;
                }

                return null;
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override int[] RuntimeId
                => new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(_owningLinkLabel.InternalHandle),
                    _owningLinkLabel.GetHashCode()
                };
        }
    }
}
