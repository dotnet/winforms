// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Private.Windows.GdiPlus.Resources;

namespace Windows.Win32.Graphics.GdiPlus;

internal static class StatusExtensions
{
    internal static void ThrowIfFailed(this Status status)
    {
        if (status != Status.Ok)
        {
            throw status.GetException();
        }
    }

#pragma warning disable CA2201 // Do not raise reserved exception types - Expected in this case as these are effectively "runtime" exceptions
    internal static Exception GetException(this Status status)
    {
        Debug.Assert(status != Status.Ok, "Throwing an exception for an 'Ok' return code");

        switch (status)
        {
            case Status.GenericError:
                return new ExternalException(SR.GdiplusGenericError, (int)HRESULT.E_FAIL);
            case Status.InvalidParameter:
                return new ArgumentException(SR.GdiplusInvalidParameter);
            case Status.OutOfMemory:
                return new OutOfMemoryException(SR.GdiplusOutOfMemory);
            case Status.ObjectBusy:
                return new InvalidOperationException(SR.GdiplusObjectBusy);
            case Status.InsufficientBuffer:
                return new OutOfMemoryException(SR.GdiplusInsufficientBuffer);
            case Status.NotImplemented:
                return new NotImplementedException(SR.GdiplusNotImplemented);
            case Status.Win32Error:
                return new ExternalException(SR.GdiplusGenericError, (int)HRESULT.E_FAIL);
            case Status.WrongState:
                return new InvalidOperationException(SR.GdiplusWrongState);
            case Status.Aborted:
                return new ExternalException(SR.GdiplusAborted, (int)HRESULT.E_ABORT);
            case Status.FileNotFound:
                return new FileNotFoundException(SR.GdiplusFileNotFound);
            case Status.ValueOverflow:
                return new OverflowException(SR.GdiplusOverflow);
            case Status.AccessDenied:
                return new ExternalException(SR.GdiplusAccessDenied, (int)HRESULT.E_ACCESSDENIED);
            case Status.UnknownImageFormat:
                return new ArgumentException(SR.GdiplusUnknownImageFormat);
            case Status.PropertyNotFound:
                return new ArgumentException(SR.GdiplusPropertyNotFoundError);
            case Status.PropertyNotSupported:
                return new ArgumentException(SR.GdiplusPropertyNotSupportedError);

            case Status.FontFamilyNotFound:
                Debug.Fail("We should be special casing FontFamilyNotFound so we can provide the font name");
                return new ArgumentException(string.Format(SR.GdiplusFontFamilyNotFound, "?"));

            case Status.FontStyleNotFound:
                Debug.Fail("We should be special casing FontStyleNotFound so we can provide the font name");
                return new ArgumentException(string.Format(SR.GdiplusFontStyleNotFound, "?", "?"));

            case Status.NotTrueTypeFont:
                Debug.Fail("We should be special casing NotTrueTypeFont so we can provide the font name");
                return new ArgumentException(SR.GdiplusNotTrueTypeFont_NoName);

            case Status.UnsupportedGdiplusVersion:
                return new ExternalException(SR.GdiplusUnsupportedGdiplusVersion, (int)HRESULT.E_FAIL);

            case Status.GdiplusNotInitialized:
                return new ExternalException(SR.GdiplusNotInitialized, (int)HRESULT.E_FAIL);
        }

        return new ExternalException($"{SR.GdiplusUnknown} [{status}]", (int)HRESULT.E_UNEXPECTED);
    }
#pragma warning restore CA2201
}
