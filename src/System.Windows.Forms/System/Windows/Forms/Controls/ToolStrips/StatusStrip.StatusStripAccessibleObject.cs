// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class StatusStrip
{
    internal class StatusStripAccessibleObject : ToolStripAccessibleObject
    {
        public StatusStripAccessibleObject(StatusStrip owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.StatusBar);

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            => this.IsOwnerHandleCreated(out StatusStrip? _) ? HitTest((int)x, (int)y) : null;

        internal override IRawElementProviderFragment.Interface? GetFocus()
            => this.IsOwnerHandleCreated(out StatusStrip? _) ? GetFocused() : null;
    }
}
