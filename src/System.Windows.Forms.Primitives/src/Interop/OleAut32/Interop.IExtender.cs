// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal partial class Interop
{
    internal static partial class Oleaut32
    {
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

            object Parent { [return: MarshalAs(UnmanagedType.Interface)] get; }

            IntPtr Hwnd { get; }

            object Container { [return: MarshalAs(UnmanagedType.Interface)] get; }

            void Move(
                [MarshalAs(UnmanagedType.Interface)] object left,
                [MarshalAs(UnmanagedType.Interface)] object top,
                [MarshalAs(UnmanagedType.Interface)] object width,
                [MarshalAs(UnmanagedType.Interface)] object height);
        }
    }
}
