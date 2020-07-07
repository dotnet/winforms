// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

internal static partial class Interop
{
    internal enum HRESULT : int
    {
        S_OK = 0,
        S_FALSE = 1,
        DRAGDROP_S_DROP = 0x00040100,
        DRAGDROP_S_CANCEL = 0x00040101,
        DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102,
        DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
        DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
        DISP_E_UNKNOWNNAME = unchecked((int)0x80020006),
        DISP_E_EXCEPTION = unchecked((int)0x80020009),
        DISP_E_UNKNOWNLCID = unchecked((int)0x8002000C),
        DISP_E_DIVBYZERO = unchecked((int)0x80020012),
        TYPE_E_BADMODULEKIND = unchecked((int)0x800288BD),
        E_NOTIMPL = unchecked((int)0x80004001),
        E_NOINTERFACE = unchecked((int)0x80004002),
        E_POINTER = unchecked((int)0x80004003),
        E_ABORT = unchecked((int)0x80004004),
        E_FAIL = unchecked((int)0x80004005),
        OLE_E_ADVISENOTSUPPORTED = unchecked((int)0x80040003),
        OLE_E_NOCONNECTION = unchecked((int)0x80040004),
        OLE_E_PROMPTSAVECANCELLED = unchecked((int)0x8004000C),
        OLE_E_INVALIDRECT = unchecked((int)0x8004000D),
        DV_E_FORMATETC = unchecked((int)0x80040064),
        DV_E_TYMED = unchecked((int)0x80040069),
        DV_E_DVASPECT = unchecked((int)0x8004006B),
        DRAGDROP_E_NOTREGISTERED = unchecked((int)0x80040100),
        DRAGDROP_E_ALREADYREGISTERED = unchecked((int)0x80040101),
        VIEW_E_DRAW = unchecked((int)0x80040140),
        INPLACE_E_NOTOOLSPACE = unchecked((int)0x800401A1),
        STG_E_INVALIDFUNCTION = unchecked((int)0x80030001L),
        STG_E_FILENOTFOUND = unchecked((int)0x80030002),
        STG_E_ACCESSDENIED = unchecked((int)0x80030005),
        STG_E_INVALIDPARAMETER = unchecked((int)0x80030057),
        STG_E_INVALIDFLAG = unchecked((int)0x800300FF),
        E_OUTOFMEMORY = unchecked((int)0x8007000E),
        E_ACCESSDENIED = unchecked((int)0x80070005L),
        E_INVALIDARG = unchecked((int)0x80070057),
        ERROR_CANCELLED = unchecked((int)0x800704C7),
        RPC_E_CHANGED_MODE = unchecked((int)0x80010106),
    }
}

internal static class HResultExtensions
{
    public static bool Succeeded(this HRESULT hr) => hr >= 0;

    public static bool Failed(this HRESULT hr) => hr < 0;
}
