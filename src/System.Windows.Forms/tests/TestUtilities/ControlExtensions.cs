// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Metafiles;
using Windows.Win32;
using Windows.Win32.Foundation;

namespace System.Windows.Forms;

public static class ControlExtensions
{
    /// <summary>
    ///  Creates a metafile for the specified <see cref="Control"/> by calling <see cref="PInvokeCore.WM_PRINT"/>.
    /// </summary>
    internal static void PrintToMetafile(
        this Control control,
        EmfScope emf,
        int prf = PInvoke.PRF_CHILDREN | PInvoke.PRF_CLIENT)
    {
        PInvokeCore.SendMessage(control, PInvokeCore.WM_PRINT, (WPARAM)emf.HDC, (LPARAM)(uint)prf);
    }
}
