// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.Metafiles;
using Windows.Win32;
using Windows.Win32.Foundation;
using static Interop;

namespace System.Windows.Forms;

public static class ControlExtensions
{
    /// <summary>
    ///  Creates a metafile for the specified <see cref="Control"/> by calling <see cref="User32.WM.PRINT"/>.
    /// </summary>
    internal static void PrintToMetafile(
        this Control control,
        EmfScope emf,
        int prf = PInvoke.PRF_CHILDREN | PInvoke.PRF_CLIENT)
    {
        PInvoke.SendMessage(control, User32.WM.PRINT, (WPARAM)emf.HDC, (LPARAM)(uint)prf);
    }
}
