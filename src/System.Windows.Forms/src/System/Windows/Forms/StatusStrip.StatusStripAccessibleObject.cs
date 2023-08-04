﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using static Interop;

namespace System.Windows.Forms;

public partial class StatusStrip
{
    internal class StatusStripAccessibleObject : ToolStripAccessibleObject
    {
        public StatusStripAccessibleObject(StatusStrip owner) : base(owner)
        {
        }

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.StatusBar);

        internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            => this.IsOwnerHandleCreated(out StatusStrip? _) ? HitTest((int)x, (int)y) : null;

        internal override UiaCore.IRawElementProviderFragment? GetFocus()
            => this.IsOwnerHandleCreated(out StatusStrip? _) ? GetFocused() : null;
    }
}
