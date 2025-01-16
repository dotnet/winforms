// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.OLE;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject
{
    internal unsafe partial class Composition
    {
        private unsafe class NativeToDesktopAdapter : DesktopDataObject.Composition.NativeToDesktopAdapter
        {
            public NativeToDesktopAdapter(Com.IDataObject* dataObject) : base(dataObject) { }

            public override DesktopDataObject.Composition.BinaryFormatUtilities GetBinaryFormatUtilities() => BinaryFormatUtilitiesInstance;

            public override bool TryGetBitmap<T>(Com.IDataObject* dataObject, string format, [NotNullWhen(true)] out object? bitmap)
            {
                bitmap = default;
                if ((typeof(Bitmap) == typeof(T) || typeof(Image) == typeof(T)))
                {
                    if (format != DesktopDataFormats.BitmapConstant)
                    {
                        return false;
                    }

                    Com.FORMATETC formatEtc = new()
                    {
                        cfFormat = (ushort)DesktopDataFormats.GetFormat(format).Id,
                        dwAspect = (uint)Com.DVASPECT.DVASPECT_CONTENT,
                        lindex = -1,
                        tymed = (uint)Com.TYMED.TYMED_GDI
                    };

                    Com.STGMEDIUM medium = default;

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
                        if ((uint)medium.tymed == (uint)Com.TYMED.TYMED_GDI
                            && !medium.hGlobal.IsNull
                            && Image.FromHbitmap(medium.hGlobal) is Bitmap clipboardBitmap)
                        {
                            bitmap = (Bitmap)clipboardBitmap.Clone();
                            clipboardBitmap.Dispose();
                            return true;
                        }
                    }
                    finally
                    {
                        PInvokeCore.ReleaseStgMedium(ref medium);
                    }
                }

                return false;
            }

            protected override void ThrowIfFormatAndTypeRequireResolver<T>(string format)
            {
                // Restricted format is either read directly from the HGLOBAL or serialization record is read manually.
                if (!IsRestrictedFormat(format)
                    // This check is a convenience for simple usages if TryGetData APIs that don't take the resolver.
                    && IsUnboundedType())
                {
                    throw new NotSupportedException(string.Format(
                        SR.ClipboardOrDragDrop_InvalidType,
                        typeof(T).FullName));
                }

                static bool IsUnboundedType()
                {
                    if (typeof(T) == typeof(object))
                    {
                        return true;
                    }

                    Type type = typeof(T);
                    // Image is a special case because we are reading Bitmaps directly from the SerializationRecord.
                    return type.IsInterface || (typeof(T) != typeof(Image) && type.IsAbstract);
                }
            }
        }
    }
}
