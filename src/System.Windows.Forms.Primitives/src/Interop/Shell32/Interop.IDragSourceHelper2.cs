// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Shell32
    {
        [ComImport]
        [Guid("DE5BF786-477A-11D2-839D-00C04FD918D0")]
        [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        public interface IDragSourceHelper2
        {
            HRESULT InitializeFromBitmap(
                SHDRAGIMAGE pshdi,
                IDataObject dataObject);

            HRESULT InitializeFromWindow(
                IntPtr hwnd,
                Point ppt,
                IDataObject dataObject);

            HRESULT SetFlags(
                int dwFlags);
        }
    }
}
