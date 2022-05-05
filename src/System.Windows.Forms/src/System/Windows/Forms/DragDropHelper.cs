// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using static Interop;
using static Interop.Shell32;
using static Interop.User32;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Helper class for drop targets to display the drag image while the cursor is over the target window and allows
    ///  the application to specify the drag image bitmap that will be displayed during a drag-and-drop operation.
    /// </summary>
    internal static class DragDropHelper
    {
        private const int DSH_ALLOWDROPDESCRIPTIONTEXT = 0x0001;
        private const string CF_DROPDESCRIPTION = "DropDescription";
        private const string CF_INSHELLDRAGLOOP = "InShellDragLoop";
        private const string CF_ISSHOWINGTEXT = "IsShowingText";
        private const string CF_USINGDEFAULTDRAGIMAGE = "UsingDefaultDragImage";

        /// <summary>
        ///  Creates an in-process server drag-image manager object and returns an interface pointer to
        ///  IDragSourceHelper2. Exposes methods that allow the application to specify the image that will be displayed
        ///  during a drag-and-drop operation.
        /// </summary>
        private static bool TryGetDragSourceHelper([NotNullWhen(true)] out IDragSourceHelper2? dragSourceHelper)
        {
            try
            {
                Ole32.CoCreateInstance(
                    ref CLSID.DragDropHelper,
                    IntPtr.Zero,
                    Ole32.CLSCTX.INPROC_SERVER,
                    ref NativeMethods.ActiveX.IID_IUnknown,
                    out object obj).ThrowIfFailed();
                dragSourceHelper = (IDragSourceHelper2)obj;
                return true;
            }
            catch
            {
                dragSourceHelper = null;
                return false;
            }
        }

        /// <summary>
        ///  Creates an in-process server drag-image manager object and returns an interface pointer to
        ///  IDropTargetHelper. Exposes methods that allow drop targets to display a drag image while the image is over
        ///  the target window.
        /// </summary>
        private static bool TryGetDropTargetHelper([NotNullWhen(true)] out IDropTargetHelper? dropTargetHelper)
        {
            try
            {
                Ole32.CoCreateInstance(
                    ref CLSID.DragDropHelper,
                    IntPtr.Zero,
                    Ole32.CLSCTX.INPROC_SERVER,
                    ref NativeMethods.ActiveX.IID_IUnknown,
                    out object obj).ThrowIfFailed();
                dropTargetHelper = (IDropTargetHelper)obj;
                return true;
            }
            catch
            {
                dropTargetHelper = null;
                return false;
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the information
        ///  needed to display the drag image.
        /// </summary>
        public static void DragEnter(IntPtr hwndTarget, IComDataObject dataObject, ref Point ppt, uint dwEffect)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragEnter(hwndTarget, dataObject, ref ppt, dwEffect).ThrowIfFailed();
            }
            catch
            {
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the cursor position has changed and provides it with the information
        ///  needed to display the drag image.
        /// </summary>
        public static void DragOver(ref Point ppt, uint dwEffect)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragOver(ref ppt, dwEffect).ThrowIfFailed();
            }
            catch
            {
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the cursor has left the drop target.
        /// </summary>
        public static void DragLeave()
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragLeave().ThrowIfFailed();
            }
            catch
            {
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the object has been dropped, and provides it with the information
        ///  needed to display the drag image.
        /// </summary>
        public static void Drop(IComDataObject dataObject, ref Point ppt, uint dwEffect)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.Drop(dataObject, ref ppt, dwEffect).ThrowIfFailed();
            }
            catch
            {
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Releases formats used by the drag-and-drop helper.
        /// </summary>
        private static void ReleaseDragDropFormats(IComDataObject comDataObject)
        {
            if (comDataObject is IDataObject iDataObject)
            {
                foreach (string format in iDataObject.GetFormats())
                {
                    if (iDataObject.GetData(format) is DragDropFormat dragDropFormat)
                    {
                        dragDropFormat.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Copies a given STGMEDIUM structure.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="mediumSrc"/> was copied into <paramref name="mediumDest"/>
        /// successfully; otherwise <see langword="false"/>.
        /// </returns>
        public static bool CopyMedium(short cfFormat, ref STGMEDIUM mediumSrc, out STGMEDIUM mediumDest)
        {
            mediumDest = new();

            try
            {
                // Copy the handle.
                switch (mediumSrc.tymed)
                {
                    case TYMED.TYMED_HGLOBAL:
                    case TYMED.TYMED_FILE:
                    case TYMED.TYMED_ENHMF:
                    case TYMED.TYMED_GDI:
                    case TYMED.TYMED_MFPICT:

                        mediumDest.unionmember = Ole32.OleDuplicateData(
                            mediumSrc.unionmember,
                            cfFormat,
                            Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT);
                        if (mediumDest.unionmember == IntPtr.Zero)
                        {
                            return false;
                        }

                        break;

                    case TYMED.TYMED_ISTORAGE:
                    case TYMED.TYMED_ISTREAM:

                        mediumDest.unionmember = mediumSrc.unionmember;

                        // Increment the reference count.
                        Marshal.AddRef(mediumSrc.unionmember);
                        break;

                    default:
                    case TYMED.TYMED_NULL:

                        mediumDest.unionmember = IntPtr.Zero;
                        break;
                }

                // Copy the storage medium type and release pointer.
                mediumDest.tymed = mediumSrc.tymed;
                mediumDest.pUnkForRelease = mediumSrc.pUnkForRelease;

                if (mediumSrc.pUnkForRelease is not null)
                {
                    // Increment the reference count.
                    Marshal.GetIUnknownForObject(mediumSrc.pUnkForRelease);
                }

                return true;
            }
            catch
            {
                Ole32.ReleaseStgMedium(ref mediumDest);
                return false;
            }
        }

        /// <summary>
        ///  Sets the drop description into a data object.
        /// </summary>
        public static unsafe void SetDropDescription(IComDataObject dataObject, DropImageType dropIcon, string message, string insert)
        {
            if (dataObject is null
                || dropIcon < DropImageType.Invalid
                || dropIcon > DropImageType.NoImage
                || (message is not null && message.Length >= Kernel32.MAX_PATH)
                || (insert is not null && insert.Length >= Kernel32.MAX_PATH))
            {
                return;
            }

            STGMEDIUM medium = default;

            try
            {
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(CF_DROPDESCRIPTION),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                medium = new()
                {
                    pUnkForRelease = null,
                    tymed = TYMED.TYMED_HGLOBAL,
                    unionmember = Kernel32.GlobalAlloc(
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                        (uint)sizeof(DROPDESCRIPTION))
                };
                if (medium.unionmember == IntPtr.Zero)
                {
                    return;
                }

                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    Kernel32.GlobalFree(medium.unionmember);
                    medium.unionmember = IntPtr.Zero;
                    return;
                }

                DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
                pDropDescription->Type = (DROPIMAGETYPE)dropIcon;
                pDropDescription->Message = message;
                pDropDescription->Insert = insert;
                Kernel32.GlobalUnlock(medium.unionmember);
                dataObject.SetData(ref formatEtc, ref medium, true);
            }
            catch
            {
                Ole32.ReleaseStgMedium(ref medium);
                return;
            }

            SetIsShowingText(dataObject, true);
        }

        /// <summary>
        /// Sets the drag image into a data object.
        /// </summary>
        public static void SetDragImage(IComDataObject? dataObject, Bitmap dragImage, Point cursorOffset, bool usingDefaultDragImage)
        {
            if (dataObject is null
                || dragImage is null
                || !TryGetDragSourceHelper(out IDragSourceHelper2? dragSourceHelper))
            {
                return;
            }

            // Set the InShellDragLoop flag to true to facilitate loading and retrieving arbitrary private formats.
            // The drag-and-drop helper object calls IDataObject::SetData to load private formats--used for cross-process
            // support--into the data object. It later retrieves these formats by calling IDataObject::GetData.
            // The data object's SetData and GetData implementations inspect for the InShellDragLoop flag to know when the
            // data object is in a drag-and-drop loop and needs to load and retrieve the arbitrary private formats.
            if (!GetInDragLoop(dataObject))
            {
                SetInDragLoop(dataObject, true);
            }

            Gdi32.HBITMAP hbmpDragImage = (Gdi32.HBITMAP)IntPtr.Zero;

            try
            {
                // The Windows drag image manager will own this bitmap object and free the memory when its finished.
                // Only call DeleteObject if an exception occurs while initializing.
                hbmpDragImage = dragImage.GetHBITMAP();
                SHDRAGIMAGE shDragImage = new()
                {
                    hbmpDragImage = hbmpDragImage,
                    sizeDragImage = dragImage.Size,
                    ptOffset = cursorOffset,
                    crColorKey = GetSysColor(COLOR.WINDOW)
                };

                dragSourceHelper.SetFlags(DSH_ALLOWDROPDESCRIPTIONTEXT).ThrowIfFailed();
                dragSourceHelper.InitializeFromBitmap(shDragImage, dataObject).ThrowIfFailed();
            }
            catch
            {
                Gdi32.DeleteObject(hbmpDragImage);
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dragSourceHelper);
            }

            SetIsShowingText(dataObject, true);
            SetUsingDefaultDragImage(dataObject, usingDefaultDragImage);
        }

        /// <summary>
        ///  Determines whether the specified FORMATETC is InDragLoop.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="formatEtc"/> is the InDragLoop format; otherwise <see langword="false"/>.
        /// </returns>
        public static bool GetInDragLoopFormat(FORMATETC formatEtc)
        {
            return DataFormats.GetFormat(formatEtc.cfFormat).Name.Equals(CF_INSHELLDRAGLOOP);
        }

        /// <summary>
        ///  Determines whether the data object is in a drag-and-drop loop.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
        /// </returns>
        public static unsafe bool GetInDragLoop(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(CF_INSHELLDRAGLOOP)
                && dataObject.GetData(CF_INSHELLDRAGLOOP) is DragDropFormat dragDropFormat
                && dragDropFormat.Medium.unionmember != IntPtr.Zero)
            {
                try
                {
                    IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                    if (basePtr == IntPtr.Zero)
                    {
                        return false;
                    }

                    BOOL* pValue = (BOOL*)basePtr;
                    return *pValue == BOOL.TRUE;
                }
                finally
                {
                    Kernel32.GlobalUnlock(dragDropFormat.Medium.unionmember);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Gets the InShellDragLoop format from a data object.
        /// </summary>
        public static bool GetInDragLoop(IComDataObject dataObject)
        {
            return GetBooleanFormat(dataObject, CF_INSHELLDRAGLOOP);
        }

        /// <summary>
        ///  Gets boolean formats from a data object.
        /// </summary>
        private static unsafe bool GetBooleanFormat(IComDataObject dataObject, string format)
        {
            if (dataObject is null || string.IsNullOrWhiteSpace(format))
            {
                return false;
            }

            FORMATETC formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW(format),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = TYMED.TYMED_HGLOBAL
            };

            if (dataObject.QueryGetData(ref formatEtc) != (int)HRESULT.S_OK)
            {
                return false;
            }

            STGMEDIUM medium = new();

            try
            {
                dataObject.GetData(ref formatEtc, out medium);
                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    return false;
                }

                BOOL* pValue = (BOOL*)basePtr;
                return *pValue == BOOL.TRUE;
            }
            finally
            {
                Kernel32.GlobalUnlock(medium.unionmember);
            }
        }

        /// <summary>
        ///  Sets the InShellDragLoop format into a data object.
        /// </summary>
        public static void SetInDragLoop(IComDataObject? dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_INSHELLDRAGLOOP, value);

            if (!value && dataObject is not null)
            {
                // The drag loop is over, release the drag and drop formats.
                ReleaseDragDropFormats(dataObject);
            }
        }

        /// <summary>
        ///  Sets the IsShowingText format into a data object.
        /// </summary>
        private static void SetIsShowingText(IComDataObject? dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_ISSHOWINGTEXT, value);
        }

        /// <summary>
        ///  Sets the UsingDefaultDragImage format into a data object.
        /// </summary>
        private static void SetUsingDefaultDragImage(IComDataObject? dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_USINGDEFAULTDRAGIMAGE, value);
        }

        /// <summary>
        ///  Sets boolean formats into a data object.
        /// </summary>
        private static unsafe void SetBooleanFormat(IComDataObject? dataObject, string format, bool value)
        {
            if (dataObject is null)
            {
                return;
            }

            STGMEDIUM medium = default;

            try
            {
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(format),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                medium = new()
                {
                    pUnkForRelease = null,
                    tymed = TYMED.TYMED_HGLOBAL,
                    unionmember = Kernel32.GlobalAlloc(
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                        sizeof(BOOL))
                };
                if (medium.unionmember == IntPtr.Zero)
                {
                    return;
                }

                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    Kernel32.GlobalFree(medium.unionmember);
                    medium.unionmember = IntPtr.Zero;
                    return;
                }

                BOOL* pValue = (BOOL*)basePtr;
                *pValue = value.ToBOOL();
                Kernel32.GlobalUnlock(medium.unionmember);
                dataObject.SetData(ref formatEtc, ref medium, true);
            }
            catch
            {
                Ole32.ReleaseStgMedium(ref medium);
            }
        }
    }
}
