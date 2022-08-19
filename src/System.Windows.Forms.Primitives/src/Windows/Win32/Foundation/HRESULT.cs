// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Windows.Win32.Foundation
{
    internal readonly partial struct HRESULT
    {
        internal static HRESULT S_OK => new(0);
        internal static HRESULT S_FALSE => new(1);
        internal static HRESULT DRAGDROP_S_DROP => new(0x00040100);
        internal static HRESULT DRAGDROP_S_CANCEL => new(0x00040101);
        internal static HRESULT DRAGDROP_S_USEDEFAULTCURSORS => new(0x00040102);

        internal static HRESULT E_NOTIMPL => new(unchecked((int)0x80004001));
        internal static HRESULT E_NOINTERFACE => new(unchecked((int)0x80004002));
        internal static HRESULT E_POINTER => new(unchecked((int)0x80004003));
        internal static HRESULT E_ABORT => new(unchecked((int)0x80004004));
        internal static HRESULT E_FAIL => new(unchecked((int)0x80004005));

        // These are CLR HRESULTs
        internal static HRESULT InvalidArgFailure => new(unchecked((int)0x80008081));
        internal static HRESULT CoreHostLibLoadFailure => new(unchecked((int)0x80008082));
        internal static HRESULT CoreHostLibMissingFailure => new(unchecked((int)0x80008083));
        internal static HRESULT CoreHostEntryPointFailure => new(unchecked((int)0x80008084));
        internal static HRESULT CoreHostCurHostFindFailure => new(unchecked((int)0x80008085));
        internal static HRESULT CoreClrResolveFailure => new(unchecked((int)0x80008087));
        internal static HRESULT CoreClrBindFailure => new(unchecked((int)0x80008088));
        internal static HRESULT CoreClrInitFailure => new(unchecked((int)0x80008089));
        internal static HRESULT CoreClrExeFailure => new(unchecked((int)0x8000808a));
        internal static HRESULT LibHostExecModeFailure => new(unchecked((int)0x80008090));
        internal static HRESULT LibHostSdkFindFailure => new(unchecked((int)0x80008091));
        internal static HRESULT LibHostInvalidArgs => new(unchecked((int)0x80008092));
        internal static HRESULT InvalidConfigFile => new(unchecked((int)0x80008093));
        internal static HRESULT AppArgNotRunnable => new(unchecked((int)0x80008094));
        internal static HRESULT AppHostExeNotBoundFailure => new(unchecked((int)0x80008095));
        internal static HRESULT FrameworkMissingFailure => new(unchecked((int)0x80008096));
        internal static HRESULT HostApiFailed => new(unchecked((int)0x80008097));
        internal static HRESULT HostApiBufferTooSmall => new(unchecked((int)0x80008098));
        internal static HRESULT LibHostUnknownCommand => new(unchecked((int)0x80008099));
        internal static HRESULT LibHostAppRootFindFailure => new(unchecked((int)0x8000809a));
        internal static HRESULT SdkResolverResolveFailure => new(unchecked((int)0x8000809b));
        internal static HRESULT FrameworkCompatFailure => new(unchecked((int)0x8000809c));
        internal static HRESULT FrameworkCompatRetry => new(unchecked((int)0x8000809d));

        internal static HRESULT RPC_E_CHANGED_MODE => new(unchecked((int)0x80010106));
        internal static HRESULT DISP_E_MEMBERNOTFOUND => new(unchecked((int)0x80020003));
        internal static HRESULT DISP_E_PARAMNOTFOUND => new(unchecked((int)0x80020004));
        internal static HRESULT DISP_E_UNKNOWNNAME => new(unchecked((int)0x80020006));
        internal static HRESULT DISP_E_EXCEPTION => new(unchecked((int)0x80020009));
        internal static HRESULT DISP_E_UNKNOWNLCID => new(unchecked((int)0x8002000C));
        internal static HRESULT DISP_E_DIVBYZERO => new(unchecked((int)0x80020012));
        internal static HRESULT TYPE_E_BADMODULEKIND => new(unchecked((int)0x800288BD));
        internal static HRESULT STG_E_INVALIDFUNCTION => new(unchecked((int)0x80030001));
        internal static HRESULT STG_E_FILENOTFOUND => new(unchecked((int)0x80030002));
        internal static HRESULT STG_E_ACCESSDENIED => new(unchecked((int)0x80030005));
        internal static HRESULT STG_E_INVALIDPOINTER => new(unchecked((int)0x80030009));
        internal static HRESULT STG_E_INVALIDPARAMETER => new(unchecked((int)0x80030057));
        internal static HRESULT STG_E_INVALIDFLAG => new(unchecked((int)0x800300FF));
        internal static HRESULT OLE_E_ADVISENOTSUPPORTED => new(unchecked((int)0x80040003));
        internal static HRESULT OLE_E_NOCONNECTION => new(unchecked((int)0x80040004));
        internal static HRESULT OLE_E_PROMPTSAVECANCELLED => new(unchecked((int)0x8004000C));
        internal static HRESULT OLE_E_INVALIDRECT => new(unchecked((int)0x8004000D));
        internal static HRESULT DV_E_FORMATETC => new(unchecked((int)0x80040064));
        internal static HRESULT DV_E_TYMED => new(unchecked((int)0x80040069));
        internal static HRESULT DV_E_DVASPECT => new(unchecked((int)0x8004006B));
        internal static HRESULT DRAGDROP_E_NOTREGISTERED => new(unchecked((int)0x80040100));
        internal static HRESULT DRAGDROP_E_ALREADYREGISTERED => new(unchecked((int)0x80040101));
        internal static HRESULT VIEW_E_DRAW => new(unchecked((int)0x80040140));
        internal static HRESULT INPLACE_E_NOTOOLSPACE => new(unchecked((int)0x800401A1));
        internal static HRESULT CO_E_OBJNOTREG => new(unchecked((int)0x800401FB));
        internal static HRESULT CO_E_OBJISREG => new(unchecked((int)0x800401FC));
        internal static HRESULT E_ACCESSDENIED => new(unchecked((int)0x80070005));
        internal static HRESULT E_OUTOFMEMORY => new(unchecked((int)0x8007000E));
        internal static HRESULT E_INVALIDARG => new(unchecked((int)0x80070057));
        internal static HRESULT ERROR_CANCELLED => new(unchecked((int)0x800704C7));
        /*internal enum Values : int
        {
            S_OK = 0,
            S_FALSE = 1,
            DRAGDROP_S_DROP = 0x00040100,
            DRAGDROP_S_CANCEL = 0x00040101,
            DRAGDROP_S_USEDEFAULTCURSORS = 0x00040102,

            E_NOTIMPL = unchecked((int)0x80004001),
            E_NOINTERFACE = unchecked((int)0x80004002),
            E_POINTER = unchecked((int)0x80004003),
            E_ABORT = unchecked((int)0x80004004),
            E_FAIL = unchecked((int)0x80004005),

            // These are CLR HRESULTs
            InvalidArgFailure = unchecked((int)0x80008081),
            CoreHostLibLoadFailure = unchecked((int)0x80008082),
            CoreHostLibMissingFailure = unchecked((int)0x80008083),
            CoreHostEntryPointFailure = unchecked((int)0x80008084),
            CoreHostCurHostFindFailure = unchecked((int)0x80008085),
            CoreClrResolveFailure = unchecked((int)0x80008087),
            CoreClrBindFailure = unchecked((int)0x80008088),
            CoreClrInitFailure = unchecked((int)0x80008089),
            CoreClrExeFailure = unchecked((int)0x8000808a),
            LibHostExecModeFailure = unchecked((int)0x80008090),
            LibHostSdkFindFailure = unchecked((int)0x80008091),
            LibHostInvalidArgs = unchecked((int)0x80008092),
            InvalidConfigFile = unchecked((int)0x80008093),
            AppArgNotRunnable = unchecked((int)0x80008094),
            AppHostExeNotBoundFailure = unchecked((int)0x80008095),
            FrameworkMissingFailure = unchecked((int)0x80008096),
            HostApiFailed = unchecked((int)0x80008097),
            HostApiBufferTooSmall = unchecked((int)0x80008098),
            LibHostUnknownCommand = unchecked((int)0x80008099),
            LibHostAppRootFindFailure = unchecked((int)0x8000809a),
            SdkResolverResolveFailure = unchecked((int)0x8000809b),
            FrameworkCompatFailure = unchecked((int)0x8000809c),
            FrameworkCompatRetry = unchecked((int)0x8000809d),

            RPC_E_CHANGED_MODE = unchecked((int)0x80010106),
            DISP_E_MEMBERNOTFOUND = unchecked((int)0x80020003),
            DISP_E_PARAMNOTFOUND = unchecked((int)0x80020004),
            DISP_E_UNKNOWNNAME = unchecked((int)0x80020006),
            DISP_E_EXCEPTION = unchecked((int)0x80020009),
            DISP_E_UNKNOWNLCID = unchecked((int)0x8002000C),
            DISP_E_DIVBYZERO = unchecked((int)0x80020012),
            TYPE_E_BADMODULEKIND = unchecked((int)0x800288BD),
            STG_E_INVALIDFUNCTION = unchecked((int)0x80030001),
            STG_E_FILENOTFOUND = unchecked((int)0x80030002),
            STG_E_ACCESSDENIED = unchecked((int)0x80030005),
            STG_E_INVALIDPOINTER = unchecked((int)0x80030009),
            STG_E_INVALIDPARAMETER = unchecked((int)0x80030057),
            STG_E_INVALIDFLAG = unchecked((int)0x800300FF),
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
            CO_E_OBJNOTREG = unchecked((int)0x800401FB),
            CO_E_OBJISREG = unchecked((int)0x800401FC),
            E_ACCESSDENIED = unchecked((int)0x80070005),
            E_OUTOFMEMORY = unchecked((int)0x8007000E),
            E_INVALIDARG = unchecked((int)0x80070057),
            ERROR_CANCELLED = unchecked((int)0x800704C7),
        }

        public static implicit operator HRESULT(Values value) => new((int)value);*/
    }
}
