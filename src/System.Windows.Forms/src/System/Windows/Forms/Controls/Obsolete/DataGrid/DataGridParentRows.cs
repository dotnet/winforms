// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms;

#nullable disable
[Obsolete(
    Obsoletions.DataGridParentRowsMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridParentRowsDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
internal class DataGridParentRows
{
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public AccessibleObject AccessibleObject
        => throw new PlatformNotSupportedException();

    [ComVisible(true)]
    [Obsolete(
    Obsoletions.DataGridParentRowsAccessibleObjectMessage,
    error: false,
    DiagnosticId = Obsoletions.DataGridParentRowsAccessibleObjectDiagnosticId,
    UrlFormat = Obsoletions.SharedUrlFormat)]
    protected internal class DataGridParentRowsAccessibleObject : AccessibleObject
    {
        public DataGridParentRowsAccessibleObject(DataGridParentRows owner) : base()
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Rectangle Bounds
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string DefaultAction
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Name
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleObject Parent
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleRole Role
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override AccessibleStates State
            => throw new PlatformNotSupportedException();

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Value
            => throw new PlatformNotSupportedException();

        public override void DoDefaultAction()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject GetChild(int index)
            => throw new PlatformNotSupportedException();

        public override int GetChildCount()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject GetFocused()
            => throw new PlatformNotSupportedException();

        public override AccessibleObject Navigate(AccessibleNavigation navdir)
            => throw new PlatformNotSupportedException();

        public override void Select(AccessibleSelection flags)
            => throw new PlatformNotSupportedException();
    }
}
