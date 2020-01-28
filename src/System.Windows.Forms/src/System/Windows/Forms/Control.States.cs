// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class Control
    {
        [Flags]
        private protected enum States
        {
            Created = 0x00000001,
            Visible = 0x00000002,
            Enabled = 0x00000004,
            TabStop = 0x00000008,
            Recreate = 0x00000010,
            Modal = 0x00000020,
            AllowDrop = 0x00000040,
            DropTarget = 0x00000080,
            NoZOrder = 0x00000100,
            LayoutDeferred = 0x00000200,
            UseWaitCursor = 0x00000400,
            Disposed = 0x00000800,
            Disposing = 0x00001000,
            MouseEnterPending = 0x00002000,
            TrackingMouseEvent = 0x00004000,
            ThreadMarshalPending = 0x00008000,
            SizeLockedByOS = 0x00010000,
            CausesValidation = 0x00020000,
            CreatingHandle = 0x00040000,
            TopLevel = 0x00080000,
            IsAccessible = 0x00100000,
            OwnCtlBrush = 0x00200000,
            ExceptionWhilePainting = 0x00400000,
            LayoutIsDirty = 0x00800000,
            CheckedHost = 0x01000000,
            HostedInDialog = 0x02000000,
            DoubleClickFired = 0x04000000,
            MousePressed = 0x08000000,
            ValidationCancelled = 0x10000000,
            ParentRecreating = 0x20000000,
            Mirrored = 0x40000000,
        }
    }
}
