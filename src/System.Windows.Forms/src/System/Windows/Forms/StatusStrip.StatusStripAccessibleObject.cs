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

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                => Owner.IsHandleCreated ? HitTest((int)x, (int)y) : null;

            internal override UiaCore.IRawElementProviderFragment? GetFocus() => Owner.IsHandleCreated ? GetFocused() : null;
        }
    }
}
