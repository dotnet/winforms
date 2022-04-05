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
    ///  Helper class to allow drop targets to display a drag image while the cursor is over the target window and allow
    ///  an application to specify the image that will be displayed during a Shell drag and drop operation.
    /// </summary>
    internal static class DragDropHelper
    {
        private const int DSH_ALLOWDROPDESCRIPTIONTEXT = 0x0001;
        private const string CF_DROPDESCRIPTION = "DropDescription";
        private const string CF_INSHELLDRAGLOOP = "InShellDragLoop";
        private const string CF_ISNEWDRAGIMAGE = "IsNewDragImage";
        private const string CF_ISSHOWINGTEXT = "IsShowingText";
        private const string CF_USINGDEFAULTDRAGIMAGE = "UsingDefaultDragImage";

        /// <summary>
        ///  Creates an in-process server drag-image manager object and returns an interface pointer to
        ///  IDragSourceHelper2. Exposes methods that allow the application to specify the image that will be displayed
        ///  during a Shell drag and drop operation.
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
            catch (COMException ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"TryGetDragSourceHelper COMException {ex}");
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

                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "TryGetDropTargetHelper IDropTargetHelper created");
                dropTargetHelper = (IDropTargetHelper)obj;
                return true;
            }
            catch (COMException ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"TryGetDropTargetHelper COMException {ex}");
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
            catch (COMException ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper DragEnter {ex}");
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }

            SetIsNewDragImage(dataObject, false);
        }

        /// <summary>
        ///  Notifies the drag-image manager that the cursor position has changed and provides it with the information
        ///  needed to display the drag image.
        /// </summary>
        public static void DragOver(IntPtr hwndTarget, IComDataObject dataObject, ref Point ppt, uint dwEffect)
        {
            // If the application has set a new drag image, e.g. in DropSource.GiveFeedback, we must call DragEnter
            // before calling DragOver in order for the Windows drag image manager to effectively display the drag image.
            if (GetIsNewDragImage(dataObject))
            {
                DragEnter(hwndTarget, dataObject, ref ppt, dwEffect);
            }

            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragOver(ref ppt, dwEffect).ThrowIfFailed();
            }
            catch (COMException ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper DragOver {ex}");
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
        public static void DragLeave(IComDataObject dataObject)
        {
            if (!TryGetDropTargetHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragLeave().ThrowIfFailed();
            }
            catch (COMException ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper DragLeave {ex}");
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
            catch (COMException ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"IDropTargetHelper Drop {ex}");
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        /// This function copies a given STGMEDIUM structure.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="mediumSrc"/> was copied into <paramref name="mediumDest"/>
        /// successfully; otherwise <see langword="false"/>.
        /// </returns>
        public static bool CopyStgMedium(ref STGMEDIUM mediumSrc, FORMATETC formatEtc, out STGMEDIUM mediumDest)
        {
            mediumDest = new();

            try
            {
                string formatName = DataFormats.GetFormat(formatEtc.cfFormat).Name;
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"CopyStgMedium {mediumSrc.tymed} {formatName}");

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
                            formatEtc.cfFormat,
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
                mediumDest.pUnkForRelease = null;
                mediumDest.tymed = TYMED.TYMED_NULL;
                Kernel32.GlobalFree(mediumDest.unionmember);
                mediumDest.unionmember = IntPtr.Zero;
                return false;
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
            }
            catch
            {
                Kernel32.GlobalFree(medium.unionmember);
                medium.unionmember = IntPtr.Zero;
                return;
            }

            // Set IsShowingText to true.
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

            // Set InDragLoop to true to indicate the data object is in a drag-and-drop loop.
            SetInDragLoop(dataObject, true);

            Gdi32.HBITMAP hbmpDragImage = (Gdi32.HBITMAP)IntPtr.Zero;

            try
            {
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
            catch (Exception ex)
            {
                Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, $"DragDropHelper SetDragImage {ex}");
                Gdi32.DeleteObject(hbmpDragImage);
                return;
            }
            finally
            {
                Marshal.ReleaseComObject(dragSourceHelper);
            }

            SetIsNewDragImage(dataObject, true);
            SetIsShowingText(dataObject, true);
            SetUsingDefaultDragImage(dataObject, usingDefaultDragImage);
        }

        /// <summary>
        ///  Returns whether the specified FORMATETC is the CFSTR_INDRAGLOOP format.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="formatEtc"/> is the CFSTR_INDRAGLOOP format; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsInDragLoopFormat(FORMATETC formatEtc)
        {
            return DataFormats.GetFormat(formatEtc.cfFormat).Name.Equals(CF_INSHELLDRAGLOOP);
        }

        /// <summary>
        ///  Returns whether the data object is in a drag-and-drop loop.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
        /// </returns>
        public static unsafe bool InDragLoop(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(CF_INSHELLDRAGLOOP)
                && dataObject.GetData(CF_INSHELLDRAGLOOP) is DragDropFormat dragDropFormat)
            {
                try
                {
                    if (dragDropFormat.Medium.unionmember == IntPtr.Zero)
                    {
                        return false;
                    }

                    // Lock the global memory object.
                    IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                    if (basePtr == IntPtr.Zero)
                    {
                        return false;
                    }

                    // Read the BOOL from the global memory handle.
                    BOOL* pValue = (BOOL*)basePtr;
                    return *pValue == BOOL.TRUE;
                }
                finally
                {
                    // Unlock the global memory object
                    Kernel32.GlobalUnlock(dragDropFormat.Medium.unionmember);
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Gets the IsNewDragImage format from a data object.
        /// </summary>
        private static bool GetIsNewDragImage(IComDataObject dataObject)
        {
            return GetBooleanFormat(dataObject, CF_ISNEWDRAGIMAGE);
        }

        /// <summary>
        ///  Gets boolean formats from a data object.
        /// </summary>
        private static unsafe bool GetBooleanFormat(IComDataObject dataObject, string format)
        {
            if (dataObject is null
                || string.IsNullOrWhiteSpace(format))
            {
                return false;
            }

            // Create the clipboard format.
            FORMATETC formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW(format),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = TYMED.TYMED_HGLOBAL
            };

            // Check if the data object contains a boolean.
            if (dataObject.QueryGetData(ref formatEtc) != (int)HRESULT.S_OK)
            {
                return false;
            }

            // Create the storage medium used for data transfer.
            STGMEDIUM medium = new();
            try
            {
                // Get the boolean from the data object.
                dataObject.GetData(ref formatEtc, out medium);

                // Lock the global memory object.
                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                if (basePtr == IntPtr.Zero)
                {
                    return false;
                }

                // Read the BOOL from the global memory handle.
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
        }

        /// <summary>
        ///  Sets the IsNewDragImage format into a data object.
        /// </summary>
        private static void SetIsNewDragImage(IComDataObject? dataObject, bool value)
        {
            SetBooleanFormat(dataObject, CF_ISNEWDRAGIMAGE, value);
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
                        sizeof(BOOL))
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
