// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Ole;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms.Ole;

/// <summary>
///  Provides Windows Forms specific OLE services.
/// </summary>
internal sealed class WinFormsOleServices : IOleServices
{
    // Prevent instantiation
    private WinFormsOleServices() { }

    public static void EnsureThreadState()
    {
        // There were some cases historically where we would try not to throw to avoid user code crashing from
        // attempting to call OLE code in a finalizer. There isn't a bullet proof way to know whether or not
        // we're on the finalizer thread, this should be left to the finalizer implementers to handle (even if
        // that might be us on behalf of the user).
        //
        // In one other case we were checking for Control.CheckForIllegalCrossThreadCalls, but that is also not
        // a great idea, as it overloaded the meaning of the property. Try to keep this as simple and as consistent
        // as possible.
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }
    }

    static unsafe HRESULT IOleServices.GetDataHere(string format, object data, FORMATETC* pformatetc, STGMEDIUM* pmedium)
    {
        if (format == DataFormatNames.Dib && data is Image)
        {
            // GDI+ does not properly handle saving to DIB images. Since the clipboard will take
            // an HBITMAP and publish a Dib, we don't need to support this.
            return HRESULT.DV_E_TYMED;
        }

        if (((TYMED)pformatetc->tymed).HasFlag(TYMED.TYMED_GDI))
        {
            if (format.Equals(DataFormatNames.Bitmap) && data is Bitmap bitmap)
            {
                // Save bitmap
                pmedium->u.hBitmap = GetCompatibleBitmap(bitmap);
            }

            return HRESULT.S_OK;
        }

        return HRESULT.DV_E_TYMED;

        static HBITMAP GetCompatibleBitmap(Bitmap bitmap)
        {
            using var screenDC = GetDcScope.ScreenDC;

            // GDI+ returns a DIBSECTION based HBITMAP. The clipboard only deals well with bitmaps created using
            // CreateCompatibleBitmap(). So, we convert the DIBSECTION into a compatible bitmap.
            HBITMAP hbitmap = bitmap.GetHBITMAP();

            // Create a compatible DC to render the source bitmap.
            using CreateDcScope sourceDC = new(screenDC);
            using SelectObjectScope sourceBitmapSelection = new(sourceDC, hbitmap);

            // Create a compatible DC and a new compatible bitmap.
            using CreateDcScope destinationDC = new(screenDC);
            HBITMAP compatibleBitmap = PInvokeCore.CreateCompatibleBitmap(screenDC, bitmap.Size.Width, bitmap.Size.Height);

            // Select the new bitmap into a compatible DC and render the blt the original bitmap.
            using SelectObjectScope destinationBitmapSelection = new(destinationDC, compatibleBitmap);
            PInvokeCore.BitBlt(
                destinationDC,
                0,
                0,
                bitmap.Size.Width,
                bitmap.Size.Height,
                sourceDC,
                0,
                0,
                ROP_CODE.SRCCOPY);

            return compatibleBitmap;
        }
    }

    static unsafe bool IOleServices.TryGetBitmapFromDataObject<T>(Com.IDataObject* dataObject, [NotNullWhen(true)] out T data)
    {
        if ((typeof(Bitmap) == typeof(T) || typeof(Image) == typeof(T))
            && TryGetBitmapData(dataObject, out Bitmap? bitmap))
        {
            data = (T)(object)bitmap!;
            return true;
        }

        data = default!;
        return false;

        static unsafe bool TryGetBitmapData(Com.IDataObject* dataObject, [NotNullWhen(true)] out Bitmap? data)
        {
            data = default;

            FORMATETC formatEtc = new()
            {
                cfFormat = (ushort)CLIPBOARD_FORMAT.CF_BITMAP,
                dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                tymed = (uint)TYMED.TYMED_GDI
            };

            STGMEDIUM medium = default;

            if (dataObject->QueryGetData(formatEtc).Succeeded)
            {
                HRESULT hr = dataObject->GetData(formatEtc, out medium);

                // One of the ways this can happen is when we attempt to put binary formatted data onto the
                // clipboard, which will succeed as Windows ignores all errors when putting data on the clipboard.
                // The data state, however, is not good, and this error will be returned by Windows when asking to
                // get the data out.
                Debug.WriteLineIf(hr == HRESULT.CLIPBRD_E_BAD_DATA, "CLIPBRD_E_BAD_DATA returned when trying to get clipboard data.");
            }

            try
            {
                // GDI+ doesn't own this HBITMAP, but we can't delete it while the object is still around. So we
                // have to do the really expensive thing of cloning the image so we can release the HBITMAP.
                if ((uint)medium.tymed == (uint)TYMED.TYMED_GDI
                    && !medium.hGlobal.IsNull
                    && Image.FromHbitmap(medium.hGlobal) is Bitmap clipboardBitmap)
                {
                    data = (Bitmap)clipboardBitmap.Clone();
                    clipboardBitmap.Dispose();
                    return true;
                }
            }
            finally
            {
                PInvokeCore.ReleaseStgMedium(ref medium);
            }

            return false;
        }
    }

    static bool IOleServices.AllowTypeWithoutResolver<T>() =>
        // Image is a special case because we are reading Bitmaps directly from the SerializationRecord.
        typeof(T) == typeof(Image);

    static void IOleServices.ValidateDataStoreData(ref string format, bool autoConvert, object? data)
    {
        // We do not have proper support for Dibs, so if the user explicitly asked
        // for Dib and provided a Bitmap object we can't convert. Instead, publish as an HBITMAP
        // and let the system provide the conversion for us.
        if (data is Bitmap && format.Equals(DataFormatNames.Dib))
        {
            format = autoConvert ? DataFormatNames.Bitmap : throw new NotSupportedException(SR.DataObjectDibNotSupported);
        }
    }

    static bool IOleServices.IsValidTypeForFormat(Type type, string format) => format switch
    {
        DataFormatNames.Bitmap or DataFormatNames.BinaryFormatBitmap => type == typeof(Bitmap) || type == typeof(Image),
        // All else should fall through as valid.
        _ => true
    };

    static IComVisibleDataObject IOleServices.CreateDataObject() =>
        new DataObject();

    static unsafe HRESULT IOleServices.OleGetClipboard(Com.IDataObject** dataObject) =>
        PInvokeCore.OleGetClipboard(dataObject);

    static unsafe HRESULT IOleServices.OleSetClipboard(Com.IDataObject* dataObject) =>
        PInvokeCore.OleSetClipboard(dataObject);

    static HRESULT IOleServices.OleFlushClipboard() =>
        PInvokeCore.OleFlushClipboard();
}
