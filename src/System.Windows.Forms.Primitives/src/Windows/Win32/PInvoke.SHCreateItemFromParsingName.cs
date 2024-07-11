// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace Windows.Win32;

internal static partial class PInvoke
{
    /// <inheritdoc cref="SHCreateItemFromParsingName(string, System.Com.IBindCtx*, in Guid, out void*)"/>
    public static unsafe IShellItem* SHCreateItemFromParsingName(string path)
    {
        HRESULT hr = SHCreateItemFromParsingName(path, pbc: null, in IID.GetRef<IShellItem>(), out void* ppv);
        if (hr.Failed)
        {
            throw new Win32Exception((int)hr);
        }

        return (IShellItem*)ppv;
    }
}
