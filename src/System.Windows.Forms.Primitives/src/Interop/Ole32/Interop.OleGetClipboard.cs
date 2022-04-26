// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Ole32, ExactSpelling = true)]
        public static extern HRESULT OleGetClipboard(out IntPtr data);

        public static bool OleGetClipboard([NotNullWhen(true)] out IDataObject? data, out HRESULT result)
        {
            result = OleGetClipboard(out IntPtr ptr);
            if (result == HRESULT.S_OK)
            {
                data = (IDataObject)WinFormsComWrappers.Instance.GetOrCreateObjectForComInstance(ptr, CreateObjectFlags.Unwrap);
                return true;
            }

            data = null;
            return false;
        }
    }
}
