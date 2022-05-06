// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        private static extern HRESULT OleSetClipboard(IntPtr pDataObj);

        public static HRESULT OleSetClipboard(IDataObject? pDataObj)
        {
            if (pDataObj is null)
            {
                return OleSetClipboard(IntPtr.Zero);
            }

            return OleSetClipboard(WinFormsComWrappers.Instance.GetComPointer(pDataObj, IID.IDataObject));
        }
    }
}
