// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using Windows.Win32.System.Ole;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT DoDragDrop(
            IDataObject pDataObj,
            IDropSource.Interface pDropSource,
            DROPEFFECT dwOKEffects,
            out DROPEFFECT pdwEffect);
    }
}
