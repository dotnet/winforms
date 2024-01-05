// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Printing;

public abstract partial class PrintController
{
    /// <summary>
    ///  Represents a SafeHandle for a Printer's Device Mode struct handle (DEVMODE)
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   DEVMODEs are pretty expensive, so we cache one here and share it
    ///   with the Standard and Preview print controllers.
    ///  </para>
    /// </remarks>
    internal sealed class SafeDeviceModeHandle : SafeHandle
    {
        public SafeDeviceModeHandle() : base(IntPtr.Zero, ownsHandle: true)
        {
        }

        internal SafeDeviceModeHandle(IntPtr handle) : base(IntPtr.Zero, ownsHandle: true)
        {
            SetHandle(handle);
        }

        public override bool IsInvalid => handle == 0;

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                PInvokeCore.GlobalFree((HGLOBAL)handle);
            }

            handle = 0;
            return true;
        }

        public static implicit operator IntPtr(SafeDeviceModeHandle handle)
        {
            return (handle is null) ? 0 : handle.handle;
        }

        public static explicit operator SafeDeviceModeHandle(IntPtr handle)
        {
            return new SafeDeviceModeHandle(handle);
        }

        public static implicit operator HGLOBAL(SafeDeviceModeHandle handle)
            => (handle is null) ? HGLOBAL.Null : (HGLOBAL)handle.handle;
    }
}
