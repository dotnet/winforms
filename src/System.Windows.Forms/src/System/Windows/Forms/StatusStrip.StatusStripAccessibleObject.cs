// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class StatusStrip
    {
        internal class StatusStripAccessibleObject : ToolStripAccessibleObject
        {
            public StatusStripAccessibleObject(StatusStrip owner) : base(owner)
            {
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return AccessibleRole.StatusBar;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (Owner is not StatusStrip statusStrip || statusStrip.Items.Count == 0)
                {
                    return base.FragmentNavigate(direction);
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        AccessibleObject? firstChild;
                        for (int i = 0; i < GetChildCount(); i++)
                        {
                            firstChild = GetChild(i);
                            if (firstChild is not null && firstChild is not ControlAccessibleObject)
                            {
                                return firstChild;
                            }
                        }

                        return null;

                    case UiaCore.NavigateDirection.LastChild:
                        AccessibleObject? lastChild;
                        for (int i = GetChildCount() - 1; i >= 0; i--)
                        {
                            lastChild = GetChild(i);
                            if (lastChild is not null && lastChild is not ControlAccessibleObject)
                            {
                                return lastChild;
                            }
                        }

                        return null;
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                => Owner.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            internal override UiaCore.IRawElementProviderFragment? GetFocus() => Owner.IsHandleCreated ? GetFocused() : null;
        }
    }
}
