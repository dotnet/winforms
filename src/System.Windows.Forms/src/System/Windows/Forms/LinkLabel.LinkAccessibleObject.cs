// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class LinkLabel
    {
        internal class LinkAccessibleObject : AccessibleObject
        {
            private readonly Link _ownerLink;

            public LinkAccessibleObject(Link link) : base()
            {
                _ownerLink = link;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!_ownerLink.Owner.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Region region = _ownerLink.VisualRegion;
                    Graphics g = Graphics.FromHwnd(_ownerLink.Owner.Handle);

                    // Make sure we have a region for this link
                    //
                    if (region is null)
                    {
                        _ownerLink.Owner.EnsureRun(g);
                        region = _ownerLink.VisualRegion;
                        if (region is null)
                        {
                            g.Dispose();
                            return Rectangle.Empty;
                        }
                    }

                    Rectangle rect;
                    try
                    {
                        rect = Rectangle.Ceiling(region.GetBounds(g));
                    }
                    finally
                    {
                        g.Dispose();
                    }

                    // Translate rect to screen coordinates
                    //
                    return _ownerLink.Owner.RectangleToScreen(rect);
                }
            }

            public override string DefaultAction
            {
                get
                {
                    return SR.AccessibleActionClick;
                }
            }

            public override string Description
            {
                get
                {
                    return _ownerLink.Description;
                }
            }

            public override string? Name
            {
                get
                {
                    string text = _ownerLink.Owner.Text;
                    string name;

                    // return the full name of the link label
                    // as sometimes the link name in isolation
                    // is unusable when using a screen reader
                    name = text;
                    if (_ownerLink.Owner.UseMnemonic)
                    {
                        name = WindowsFormsUtils.TextWithoutMnemonics(name);
                    }

                    return name;
                }
                set => base.Name = value;
            }

            public override AccessibleObject Parent
            {
                get
                {
                    return _ownerLink.Owner.AccessibilityObject;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.Link;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates state = AccessibleStates.Focusable;

                    // Selected state
                    //
                    if (_ownerLink.Owner.FocusLink == _ownerLink)
                    {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }

            public override string Value
            {
                get
                {
                    // Narrator announces Link's text twice, once as a Name property and once as a Value, thus removing value.
                    // Value is optional for this role (Link).
                    return string.Empty;
                }
            }

            public override void DoDefaultAction()
            {
                _ownerLink.Owner.OnLinkClicked(new LinkLabelLinkClickedEventArgs(_ownerLink));
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.IsEnabledPropertyId)
                {
                    if (!_ownerLink.Owner.Enabled)
                    {
                        return false;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
