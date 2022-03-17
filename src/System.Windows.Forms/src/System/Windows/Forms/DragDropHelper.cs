// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
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
    ///  Helper class to allow drop targets to display a drag image while the cursor is over the target window
    ///  and allow an application to specify the image that will be displayed during a Shell drag-and-drop operation.
    /// </summary>
    internal static class DragDropHelper
    {
        private const int DSH_ALLOWDROPDESCRIPTIONTEXT = 0x0001;
        private const string CF_DISABLEDRAGTEXT = "DisableDragText";
        private const string CF_DRAGCONTEXT = "DragContext";
        private const string CF_DRAGIMAGE = "DragImage";
        private const string CF_DRAGIMAGEBITS = "DragImageBits";
        private const string CF_DRAGSOURCEHELPERFLAGS = "DragSourceHelperFlags";
        private const string CF_DRAGWINDOW = "DragWindow";
        private const string CF_DROPDESCRIPTION = "DropDescription";
        private const string CF_ISCOMPUTINGIMAGE = "IsComputingImage";
        private const string CF_ISSHOWINGLAYERED = "IsShowingLayered";
        private const string CF_ISSHOWINGTEXT = "IsShowingText";
        private const string CF_SHELL_IDLIST_ARRAY = "Shell IDList Array";
        private const string CF_UNTRUSTEDDRAGDROP = "UntrustedDragDrop";
        private const string CF_USINGDEFAULTDRAGIMAGE = "UsingDefaultDragImage";

        public static readonly string[] s_formats = new string[]
        {
            CF_DISABLEDRAGTEXT,
            CF_DRAGCONTEXT,
            CF_DRAGIMAGE,
            CF_DRAGIMAGEBITS,
            CF_DRAGSOURCEHELPERFLAGS,
            CF_DRAGWINDOW,
            CF_DROPDESCRIPTION,
            CF_ISCOMPUTINGIMAGE,
            CF_ISSHOWINGLAYERED,
            CF_ISSHOWINGTEXT,
            CF_SHELL_IDLIST_ARRAY,
            CF_UNTRUSTEDDRAGDROP,
            CF_USINGDEFAULTDRAGIMAGE
        };

        /// <summary>
        ///  Creates an in-process server drag-image manager object and returns an interface pointer
        ///  to IDragSourceHelper2. Exposes methods that allow the application to specify the imaged
        ///  that will be displayed during a Shell drag-and-drop operation.
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

                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "TryGetDragSourceHelper IDragSourceHelper2 created");
                dragSourceHelper = (IDragSourceHelper2)obj;
                return true;
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"TryGetDragSourceHelper COMException {comEx}");
                dragSourceHelper = null;
                return false;
            }
        }

        /// <summary>
        ///  Creates an in-process server drag-image manager object and returns an interface pointer
        ///  to IDropTargetHelper. Exposes methods that allow drop targets to display a drag image
        ///  while the image is over the target window.
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

                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "TryGetDropTargetHelper IDropTargetHelper created");
                dropTargetHelper = (IDropTargetHelper)obj;
                return true;
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"TryGetDropTargetHelper COMException {comEx}");
                dropTargetHelper = null;
                return false;
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the
        ///  information needed to display the drag image.
        /// </summary>
        public static HRESULT DragEnter(IntPtr hwndTarget, IComDataObject dataObject, ref Point ppt, uint dwEffect)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return HRESULT.S_FALSE;
            }

            try
            {
                dropTargetHelper.DragEnter(hwndTarget, dataObject, ref ppt, dwEffect).ThrowIfFailed();
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper::DragEnter COM error {comEx}");
                return (HRESULT)comEx.HResult;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dropTargetHelper);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Notifies the drag-image manager that the cursor position has changed and provides it with the
        ///  information needed to display the drag image.
        /// </summary>
        public static HRESULT DragOver(ref Point ppt, uint dwEffect)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return HRESULT.S_FALSE;
            }

            try
            {
                dropTargetHelper.DragOver(ref ppt, dwEffect).ThrowIfFailed();
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper::DragOver COM error {comEx}");
                return HRESULT.S_FALSE;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dropTargetHelper);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Notifies the drag-image manager that the cursor has left the drop target.
        /// </summary>
        public static HRESULT DragLeave()
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return HRESULT.S_FALSE;
            }

            try
            {
                dropTargetHelper.DragLeave().ThrowIfFailed();
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper::DragLeave COM error {comEx}");
                return HRESULT.S_FALSE;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dropTargetHelper);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Notifies the drag-image manager that the object has been dropped, and provides it with the
        ///  information needed to display the drag image.
        /// </summary>
        public static HRESULT Drop(IComDataObject dataObject, ref Point ppt, uint dwEffect)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return HRESULT.S_FALSE;
            }

            try
            {
                dropTargetHelper.Drop(dataObject, ref ppt, dwEffect).ThrowIfFailed();
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper::Drop COM error {comEx}");
                return HRESULT.S_FALSE;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dropTargetHelper);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        ///  Determines whether a drop description format is present in a data object.
        /// </summary>
        /// <returns><see langword="true"/> if a drop description format is present in <paramref name="dataObject"/>; otherwise <see langword="false"/>.</returns>
        private static bool GetDropDescriptionPresent(IComDataObject dataObject)
        {
            if (dataObject is null)
            {
                return false;
            }

            try
            {
                // Create the drop description clipboard format.
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(CF_DROPDESCRIPTION),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                // Check if the data object contains a drop description.
                return dataObject.QueryGetData(ref formatEtc) == (int)HRESULT.S_OK;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        ///  Gets the drop description from a data object.
        /// </summary>
        /// <returns><see langword="true"/> if the drop description was successfully obtained from <paramref name="dataObject"/>; otherwise <see langword="false"/>.</returns>
        public static unsafe bool GetDropDescription(IComDataObject dataObject, out DropIconType dropIcon, out string message, out string insert)
        {
            dropIcon = DropIconType.Default;
            message = string.Empty;
            insert = string.Empty;

            // Check if the data object contains a drop description format.
            if (dataObject is null || !GetDropDescriptionPresent(dataObject))
            {
                return false;
            }

            STGMEDIUM medium = default;

            try
            {
                // Create the drop description clipboard format.
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(CF_DROPDESCRIPTION),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                // Create the storage medium used for data transfer.
                medium = new();

                // Get the drop description from the data object.
                dataObject.GetData(ref formatEtc, out medium);

                // Lock the global memory object.
                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    return false;
                }

                // Read the drop description from the global memory handle.
                DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;

                // Check if we have a valid drop description.
                if (pDropDescription->Message.IsEmpty
                    || pDropDescription->Type < DROPIMAGETYPE.DROPIMAGE_INVALID
                    || pDropDescription->Type > DROPIMAGETYPE.DROPIMAGE_NOIMAGE)
                {
                    return false;
                }

                // Get the drop description.
                dropIcon = (DropIconType)pDropDescription->Type;
                message = pDropDescription->Message.ToString();
                insert = pDropDescription->Insert.ToString();

                return true;
            }
            finally
            {
                // Unlock the global memory object.
                Kernel32.GlobalUnlock(medium.unionmember);

                if (medium.unionmember != IntPtr.Zero)
                    Kernel32.GlobalFree(medium.unionmember);

                Ole32.ReleaseStgMedium(ref medium);
            }
        }

        /// <summary>
        ///  Sets the drop description into a data object.
        /// </summary>
        public static unsafe void SetDropDescription(IComDataObject dataObject, DropIconType dropIcon, string message, string insert)
        {
            if (dataObject is null
                || dropIcon < DropIconType.Default
                || dropIcon > DropIconType.NoDropIcon
                || (message is not null && message.Length >= Kernel32.MAX_PATH)
                || (insert is not null && insert.Length >= Kernel32.MAX_PATH))
            {
                return;
            }

            STGMEDIUM medium = default;

            try
            {
                // Create the drop description clipboard format.
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(CF_DROPDESCRIPTION),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                // Create the storage medium used for data transfer.
                medium = new()
                {
                    pUnkForRelease = null,
                    tymed = TYMED.TYMED_HGLOBAL,

                    // Allocate a suitably sized block of memory.
                    unionmember = Kernel32.GlobalAlloc(
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                        (uint)sizeof(DROPDESCRIPTION))
                };
                if (medium.unionmember == IntPtr.Zero)
                {
                    return;
                }

                // Lock the global memory object.
                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    Kernel32.GlobalFree(medium.unionmember);
                    medium.unionmember = IntPtr.Zero;
                    return;
                }

                // Write out the drop description to the global memory handle.
                DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
                pDropDescription->Type = (DROPIMAGETYPE)dropIcon;
                pDropDescription->Message = message;
                pDropDescription->Insert = insert;

                // Unlock the global memory object.
                Kernel32.GlobalUnlock(medium.unionmember);

                // Load the drop description into the data object.
                dataObject.SetData(ref formatEtc, ref medium, true);

                // Set IsShowingText to true.
                SetIsShowingText(dataObject, true);
            }
            catch
            {
                Kernel32.GlobalFree(medium.unionmember);
                medium.unionmember = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Determines whether a drag image format is present in a data object.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns><see langword="true"/> if a drag image format is present in <paramref name="dataObject"/>; otherwise <see langword="false"/>.</returns>
        public static bool GetDragImagePresent(IComDataObject dataObject)
        {
            if (dataObject is null)
            {
                return false;
            }

            try
            {
                // Create the drag image clipboard format.
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(CF_DRAGIMAGEBITS),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                // Check if the data object contains a drag image.
                return dataObject.QueryGetData(ref formatEtc) == (int)HRESULT.S_OK;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets the drag image into a data object.
        /// </summary>
        public static HRESULT SetDragImage(IComDataObject dataObject, Bitmap dragImage, Point cursorOffset, bool usingDefaultDragImage)
        {
            if (dataObject is null
                || dragImage is null
                || !TryGetDragSourceHelper(out IDragSourceHelper2? dragSourceHelper))
            {
                return HRESULT.S_FALSE;
            }

            try
            {
                SHDRAGIMAGE shDragImage = new()
                {
                    hbmpDragImage = dragImage.GetHBITMAP(),
                    sizeDragImage = dragImage.Size,
                    ptOffset = cursorOffset,
                    crColorKey = GetSysColor(COLOR.WINDOW)
                };

                if (usingDefaultDragImage)
                {
                    SetUsingDefaultDragImage(dataObject, usingDefaultDragImage);
                }

                dragSourceHelper.SetFlags(DSH_ALLOWDROPDESCRIPTIONTEXT).ThrowIfFailed();
                dragSourceHelper.InitializeFromBitmap(shDragImage, dataObject).ThrowIfFailed();
            }
            catch (COMException comEx)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropHelper SetDragImage COM error {comEx}");
                return HRESULT.S_FALSE;
            }
            finally
            {
                Marshal.FinalReleaseComObject(dragSourceHelper);
            }

            return HRESULT.S_OK;
        }

        /// <summary>
        /// This function copies a given drag-and-drop STGMEDIUM structure.
        /// </summary>
        public static bool CopyDragDropStgMedium(ref STGMEDIUM mediumSrc, short cfFormat, out STGMEDIUM mediumDest)
        {
            Debug.WriteLineIf(CompModSwitches.DataObject.TraceVerbose, $"CopyDragDropStgMedium: {mediumSrc.tymed}");

            mediumDest = new()
            {
                pUnkForRelease = mediumSrc.pUnkForRelease,
                tymed = mediumSrc.tymed
            };

            if (mediumSrc.tymed.Equals(TYMED.TYMED_HGLOBAL))
            {
                mediumDest.unionmember = Ole32.OleDuplicateData(
                    mediumSrc.unionmember,
                    cfFormat,
                    Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT);
                if (mediumDest.unionmember == IntPtr.Zero)
                {
                    return false;
                }

                return true;
            }
            else if (mediumSrc.tymed.Equals(TYMED.TYMED_ISTREAM) && mediumSrc.unionmember != IntPtr.Zero)
            {
                mediumDest.unionmember = mediumSrc.unionmember;

                // Increment the reference count.
                Marshal.AddRef(mediumSrc.unionmember);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Sets the IsShowingLayered format for a data object.
        /// </summary>
        private static void SetIsShowingLayered(IComDataObject dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_ISSHOWINGLAYERED, value);
        }

        /// <summary>
        ///  Sets the IsShowingText format for a data object.
        /// </summary>
        private static void SetIsShowingText(IComDataObject dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_ISSHOWINGTEXT, value);
        }

        /// <summary>
        ///  Sets the DisableDragText format for a data object.
        /// </summary>
        private static void SetDisableDragText(IComDataObject dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_DISABLEDRAGTEXT, value);
        }

        /// <summary>
        ///  Sets the UsingDefaultDragImage format for a data object.
        /// </summary>
        private static void SetUsingDefaultDragImage(IComDataObject dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_USINGDEFAULTDRAGIMAGE, value);
        }

        /// <summary>
        ///  Sets boolean formats for a data object.
        /// </summary>
        private static unsafe void SetBooleanFormat(IComDataObject dataObject, string format, bool value)
        {
            if (dataObject is null)
            {
                return;
            }

            STGMEDIUM medium = default;

            try
            {
                // Create the clipboard format.
                FORMATETC formatEtc = new()
                {
                    cfFormat = (short)RegisterClipboardFormatW(format),
                    dwAspect = DVASPECT.DVASPECT_CONTENT,
                    lindex = -1,
                    ptd = IntPtr.Zero,
                    tymed = TYMED.TYMED_HGLOBAL
                };

                // Create a global memory object storage medium.
                medium = new()
                {
                    pUnkForRelease = null,
                    tymed = TYMED.TYMED_HGLOBAL,

                    // Allocate a suitably sized block of memory.
                    unionmember = Kernel32.GlobalAlloc(
                        Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                        sizeof(uint))
                };
                if (medium.unionmember == IntPtr.Zero)
                {
                    return;
                }

                // Lock the global memory object.
                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    Kernel32.GlobalFree(medium.unionmember);
                    medium.unionmember = IntPtr.Zero;
                    return;
                }

                // Write the boolean out to the global memory handle.
                BOOL* pValue = (BOOL*)basePtr;
                *pValue = value.ToBOOL();

                // Unlock the global memory object
                Kernel32.GlobalUnlock(medium.unionmember);

                // Load the boolean format into the data object.
                dataObject.SetData(ref formatEtc, ref medium, true);
            }
            catch
            {
                Kernel32.GlobalFree(medium.unionmember);
                medium.unionmember = IntPtr.Zero;
            }
        }
    }
}
