// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using static Interop;

namespace Windows.Win32
{
    internal static partial class PInvoke
    {
        public static unsafe IShellItem* SHCreateItemFromParsingName(string path)
        {
            Guid shellItemIID = IID.IShellItem;
            HRESULT hr = SHCreateItemFromParsingName(path, pbc: null, in shellItemIID, out void* ppv);
            if (hr.Failed)
            {
                throw new Win32Exception((int)hr);
            }

            return (IShellItem*)ppv;
        }
    }
}
