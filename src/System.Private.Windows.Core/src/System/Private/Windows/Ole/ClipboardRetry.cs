// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace System.Private.Windows.Ole;

/// <summary>
///  Retries native clipboard data-object operations when another process temporarily owns the clipboard.
/// </summary>
internal static unsafe class ClipboardRetry
{
    private const int RetryCount = 10;
    private const int RetryDelay = 100;

    internal static HRESULT QueryGetData(IDataObject* dataObject, FORMATETC formatEtc)
    {
        int retryCount = RetryCount;
        HRESULT result;
        while ((result = dataObject->QueryGetData(formatEtc)) == HRESULT.CLIPBRD_E_CANT_OPEN)
        {
            if (--retryCount < 0)
            {
                break;
            }

            Thread.Sleep(RetryDelay);
        }

        return result;
    }

    internal static HRESULT GetData(IDataObject* dataObject, FORMATETC formatEtc, out STGMEDIUM medium)
    {
        int retryCount = RetryCount;
        HRESULT result;
        while ((result = dataObject->GetData(formatEtc, out medium)) == HRESULT.CLIPBRD_E_CANT_OPEN)
        {
            if (--retryCount < 0)
            {
                break;
            }

            Thread.Sleep(RetryDelay);
        }

        return result;
    }
}
