// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
        // It appears that WFC originally defined this interface under this GUID. Interestingly it appears to
        // have been defined as IDispatch there. There is no reference to this guid in Windows sources outside of
        // code that was copied from Windows Forms in the first place. It appears that this was inspired by
        // IExtender in VB for their Extender Control, which used the same IExtender interface name with similar
        // properties, but under a completely different GUID and DISPIDs.
        //
        // There don't seem to be any references to this interface on the web. No references were found for this
        // interfac in OLE specfications from the mid 90s. Looks to be purely our own projection and it might be
        // unused in the wild.
        [ComImport]
        [Guid("39088D7E-B71E-11D1-8F39-00C04FD946D0")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IExtender
        {
            int Align { get; set; }

            BOOL Enabled { get; set; }

            int Height { get; set; }

            int Left { get; set; }

            BOOL TabStop { get; set; }

            int Top { get; set; }

            BOOL Visible { get; set; }

            int Width { get; set; }

            string Name { get; }

            object? Parent { [return: MarshalAs(UnmanagedType.Interface)] get; }

            IntPtr Hwnd { get; }

            object? Container { [return: MarshalAs(UnmanagedType.Interface)] get; }

            void Move(
                [MarshalAs(UnmanagedType.Interface)] object left,
                [MarshalAs(UnmanagedType.Interface)] object top,
                [MarshalAs(UnmanagedType.Interface)] object width,
                [MarshalAs(UnmanagedType.Interface)] object height);
        }
    }
}
