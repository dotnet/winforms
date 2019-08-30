// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal enum HRESULT : int
    {
        S_OK = 0,
        S_FALSE = 1,
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        E_NOTIMPL = unchecked((int)0x80004001),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_ABORT = unchecked((int)0x80004004),
        E_FAIL = unchecked((int)0x80004005),
        OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C),
        DRAGDROP_E_NOTREGISTERED = unchecked((int)0x80040100),
        DRAGDROP_E_ALREADYREGISTERED = unchecked((int)0x80040101),
        STG_E_INVALIDFUNCTION = unchecked((int)0x80030001L),
        STG_E_FILENOTFOUND = unchecked((int)0x80030002),
        STG_E_ACCESSDENIED = unchecked((int)0x80030005),
        STG_E_INVALIDPARAMETER = unchecked((int)0x80030057),
        STG_E_INVALIDFLAG = unchecked((int)0x800300FF),
        E_ACCESSDENIED = unchecked((int)0x80070005L),
        E_INVALIDARG = unchecked((int)0x80070057),
    }

    public static bool Succeeded(HRESULT hr) => hr >= 0;
}
