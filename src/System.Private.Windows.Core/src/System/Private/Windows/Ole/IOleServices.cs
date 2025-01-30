// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

internal unsafe interface IOleServices
{
    /// <summary>
    ///  This method is called to validate that OLE is initialized and
    ///  that the current thread is a single-threaded apartment (STA).
    /// </summary>
    /// <exception cref="ThreadStateException">Current thread is not in the right state for OLE.</exception>
    static abstract void EnsureThreadState();

    /// <summary>
    ///  Called after unsuccessfully performing clipboard <see cref="TYMED.TYMED_HGLOBAL"/> serialization.
    /// </summary>
    static abstract HRESULT GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium);
}
