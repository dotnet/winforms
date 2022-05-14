// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
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
    ///  Helper class for drop targets to display the drag image while the cursor is over the target window and allows the
    ///  application to specify the drag image bitmap that will be displayed during a drag-and-drop operation.
    /// </summary>
    internal static class DragDropHelper
    {
        private const int DSH_ALLOWDROPDESCRIPTIONTEXT = 0x0001;
        private const string CF_DROPDESCRIPTION = "DropDescription";
        private const string CF_INSHELLDRAGLOOP = "InShellDragLoop";
        private const string CF_ISSHOWINGTEXT = "IsShowingText";
        private const string CF_USINGDEFAULTDRAGIMAGE = "UsingDefaultDragImage";

        /// <summary>
        ///  Sets the drop object image and accompanying text back to the default.
        /// </summary>
        public static void ClearDropDescription(IDataObject? dataObject)
        {
            if (dataObject is not IComDataObject comDataObject)
            {
                return;
            }

            SetDropDescription(comDataObject, DropImageType.Invalid, string.Empty, string.Empty);
        }

        /// <summary>
        ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the information
        ///  needed to display the drag image.
        /// </summary>
        public static void DragEnter(IntPtr hwndTarget, IComDataObject dataObject, ref Point ppt, uint dwEffect)
        {
            if (!TryGetDragDropHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragEnter(hwndTarget, dataObject, ref ppt, dwEffect);
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the information
        ///  needed to display the drag image.
        /// </summary>
        public static void DragEnter(IntPtr hwndTarget, DragEventArgs drgevent)
        {
            if (!TryGetDragDropHelper(out IDropTargetHelper? dropTargetHelper) || drgevent.Data is not IComDataObject dataObject)
            {
                return;
            }

            try
            {
                Point point = new(drgevent.X, drgevent.Y);
                dropTargetHelper.DragEnter(hwndTarget, dataObject, ref point, (uint)drgevent.Effect);
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
            if (!TryGetDragDropHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                dropTargetHelper.DragLeave();
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the cursor position has changed and provides it with the information needed
        ///  to display the drag image.
        /// </summary>
        public static void DragOver(DragEventArgs drgevent)
        {
            if (!TryGetDragDropHelper(out IDropTargetHelper? dropTargetHelper))
            {
                return;
            }

            try
            {
                Point point = new(drgevent.X, drgevent.Y);
                dropTargetHelper.DragOver(ref point, (uint)drgevent.Effect);
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Notifies the drag-image manager that the object has been dropped, and provides it with the information needed
        ///  to display the drag image.
        /// </summary>
        public static void Drop(DragEventArgs drgevent)
        {
            if (!TryGetDragDropHelper(out IDropTargetHelper? dropTargetHelper) || drgevent.Data is not IComDataObject dataObject)
            {
                return;
            }

            try
            {
                Point point = new(drgevent.X, drgevent.Y);
                dropTargetHelper.Drop(dataObject, ref point, (uint)drgevent.Effect);
            }
            finally
            {
                Marshal.ReleaseComObject(dropTargetHelper);
            }
        }

        /// <summary>
        ///  Gets boolean formats from a data object used to set and display drag images and drop descriptions.
        /// </summary>
        private static unsafe bool GetBooleanFormat(IComDataObject dataObject, string format)
        {
            ArgumentNullException.ThrowIfNull(nameof(dataObject));
            ArgumentException.ThrowIfNullOrEmpty(nameof(format));

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

                if (dataObject.QueryGetData(ref formatEtc) != (int)HRESULT.S_OK)
                {
                    return false;
                }

                medium = new();
                dataObject.GetData(ref formatEtc, out medium);
                IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
                return (basePtr != IntPtr.Zero) && (*(BOOL*)basePtr == BOOL.TRUE);
            }
            finally
            {
                Kernel32.GlobalUnlock(medium.unionmember);
                Ole32.ReleaseStgMedium(ref medium);
            }
        }

        /// <summary>
        ///  Determines whether the data object is in a drag loop.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
        /// </returns>
        public static unsafe bool IsInDragLoop(IDataObject dataObject)
        {
            if (dataObject.GetDataPresent(CF_INSHELLDRAGLOOP)
                && dataObject.GetData(CF_INSHELLDRAGLOOP) is DragDropFormat dragDropFormat
                && dragDropFormat.Medium.unionmember != IntPtr.Zero)
            {
                try
                {
                    IntPtr basePtr = Kernel32.GlobalLock(dragDropFormat.Medium.unionmember);
                    return (basePtr != IntPtr.Zero) && (*(BOOL*)basePtr == BOOL.TRUE);
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
        ///  Determines whether the data object is in a drag loop.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsInDragLoop(IComDataObject dataObject)
        {
            return GetBooleanFormat(dataObject, CF_INSHELLDRAGLOOP);
        }

        /// <summary>
        ///  Determines whether the specified format is InDragLoop.
        /// </summary>
        /// <returns>
        /// <see langword="true"/> if <paramref name="formatEtc"/> is the InDragLoop format; otherwise <see langword="false"/>.
        /// </returns>
        public static bool IsInDragLoopFormat(FORMATETC formatEtc)
        {
            return DataFormats.GetFormat(formatEtc.cfFormat).Name.Equals(CF_INSHELLDRAGLOOP);
        }

        /// <summary>
        ///  Releases formats used by the drag-and-drop helper.
        /// </summary>
        private static void ReleaseDragDropFormats(IComDataObject comDataObject)
        {
            if (comDataObject is not IDataObject dataObject)
            {
                return;
            }

            foreach (string format in dataObject.GetFormats())
            {
                if (dataObject.GetData(format) is DragDropFormat dragDropFormat)
                {
                    dragDropFormat.Dispose();
                }
            }
        }

        /// <summary>
        ///  Sets boolean formats into a data object used to set and display drag images and drop descriptions.
        /// </summary>
        private static unsafe void SetBooleanFormat(IComDataObject dataObject, string format, bool value)
        {
            ArgumentNullException.ThrowIfNull(nameof(dataObject));
            ArgumentException.ThrowIfNullOrEmpty(nameof(format));

            FORMATETC formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW(format),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = TYMED.TYMED_HGLOBAL
            };

            STGMEDIUM medium = new()
            {
                pUnkForRelease = null,
                tymed = TYMED.TYMED_HGLOBAL,
                unionmember = Kernel32.GlobalAlloc(
                    Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                    sizeof(BOOL))
            };
            if (medium.unionmember == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastPInvokeError(), SR.ExternalException);
            }

            IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
            if (basePtr == IntPtr.Zero)
            {
                Kernel32.GlobalFree(medium.unionmember);
                medium.unionmember = IntPtr.Zero;
                throw new Win32Exception(Marshal.GetLastPInvokeError(), SR.ExternalException);
            }

            *(BOOL*)basePtr = value.ToBOOL();
            Kernel32.GlobalUnlock(medium.unionmember);
            dataObject.SetData(ref formatEtc, ref medium, true);
        }

        /// <summary>
        /// Initializes the drag-image manager and sets the drag image bitmap into a data object.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  Because InitializeFromBitmap always performs the RGB multiplication step in calculating the alpha value, you should
        ///  always pass a bitmap without premultiplied alpha blending. Note that no error will result from passing a bitmap
        ///  with premultiplied alpha blending, but this method will multiply it again, doubling the resulting alpha value.
        /// </para>
        /// </remarks>
        public static void SetDragImage(IComDataObject dataObject, Bitmap dragImage, Point cursorOffset, bool usingDefaultDragImage)
        {
            ArgumentNullException.ThrowIfNull(nameof(dataObject));
            ArgumentNullException.ThrowIfNull(nameof(dragImage));

            if (!TryGetDragDropHelper(out IDragSourceHelper2? dragSourceHelper))
            {
                return;
            }

            // Set the InShellDragLoop flag to true to facilitate loading and retrieving arbitrary private formats. The
            // drag-and-drop helper object calls IDataObject::SetData to load private formats--used for cross-process support--into
            // the data object. It later retrieves these formats by calling IDataObject::GetData. The data object's SetData
            // and GetData implementations inspect for the InShellDragLoop flag to know when the data object is in a drag-and-drop
            // loop and needs to load and retrieve the arbitrary private formats.
            if (!IsInDragLoop(dataObject))
            {
                SetInDragLoop(dataObject, true);
            }

            Gdi32.HBITMAP hbmpDragImage = (Gdi32.HBITMAP)IntPtr.Zero;

            try
            {
                // The Windows drag image manager will own this bitmap object and free the memory when its finished. Only
                // call DeleteObject if an exception occurs while initializing.
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

            // To effectively display the drop description after changing the drag image bitmap, set IsShowingText to true;
            // otherwise the drop description text will not be displayed.
            SetIsShowingText(dataObject, true);
            SetUsingDefaultDragImage(dataObject, usingDefaultDragImage);
        }

        /// <summary>
        ///  Sets the drop description into a data object. Describes the image and accompanying text for a drop object.
        /// </summary>
        public static void SetDropDescription(DragEventArgs drgevent)
        {
            if (drgevent.Data is not IComDataObject dataObject)
            {
                return;
            }

            drgevent.Message ??= string.Empty;
            drgevent.MessageReplacementToken ??= string.Empty;
            SetDropDescription(dataObject, drgevent.DropImageType, drgevent.Message, drgevent.MessageReplacementToken);
        }

        /// <summary>
        ///  Sets the drop description into a data object. Describes the image and accompanying text for a drop object.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  Some UI coloring is applied to the text in <paramref name="message"/> if used by specifying %1 in <paramref name="messageReplacementToken"/>.
        ///  The characters %% and %1 are the subset of FormatMessage markers that are processed here.
        /// </para>
        /// </remarks>
        public static unsafe void SetDropDescription(IComDataObject dataObject, DropImageType dropImageType, string message, string messageReplacementToken)
        {
            ArgumentNullException.ThrowIfNull(nameof(dataObject));
            SourceGenerated.EnumValidator.Validate(dropImageType, nameof(dropImageType));

            if (message.Length >= Kernel32.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(nameof(message));
            }

            if (messageReplacementToken.Length >= Kernel32.MAX_PATH)
            {
                throw new ArgumentOutOfRangeException(nameof(messageReplacementToken));
            }

            FORMATETC formatEtc = new()
            {
                cfFormat = (short)RegisterClipboardFormatW(CF_DROPDESCRIPTION),
                dwAspect = DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = TYMED.TYMED_HGLOBAL
            };

            STGMEDIUM medium = new()
            {
                pUnkForRelease = null,
                tymed = TYMED.TYMED_HGLOBAL,
                unionmember = Kernel32.GlobalAlloc(
                    Kernel32.GMEM.MOVEABLE | Kernel32.GMEM.DDESHARE | Kernel32.GMEM.ZEROINIT,
                    (uint)sizeof(DROPDESCRIPTION))
            };
            if (medium.unionmember == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastPInvokeError(), SR.ExternalException);
            }

            IntPtr basePtr = Kernel32.GlobalLock(medium.unionmember);
            if (basePtr == IntPtr.Zero)
            {
                Kernel32.GlobalFree(medium.unionmember);
                medium.unionmember = IntPtr.Zero;
                throw new Win32Exception(Marshal.GetLastPInvokeError(), SR.ExternalException);
            }

            DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
            pDropDescription->Type = (DROPIMAGETYPE)dropImageType;
            pDropDescription->Message = message;
            pDropDescription->Insert = messageReplacementToken;
            Kernel32.GlobalUnlock(medium.unionmember);
            dataObject.SetData(ref formatEtc, ref medium, true);
            SetIsShowingText(dataObject, true);
        }

        /// <summary>
        ///  Sets the InDragLoop format into a data object.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  Used to determine whether the data object is in a drag loop.
        /// </para>
        /// </remarks>
        public static void SetInDragLoop(IComDataObject dataObject, bool inDragLoop)
        {
            SetBooleanFormat(dataObject, CF_INSHELLDRAGLOOP, inDragLoop);

            // If drag loop is over, release the drag and drop formats.
            if (!inDragLoop)
            {
                ReleaseDragDropFormats(dataObject);
            }
        }

        /// <summary>
        ///  Sets the IsShowingText format into a data object.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  To effectively display the drop description after changing the drag image bitmap, set <paramref name="isShowingText"/>
        ///  to true; otherwise the drop description text will not be displayed.
        /// </para>
        /// </remarks>
        private static void SetIsShowingText(IComDataObject dataObject, bool isShowingText)
        {
            SetBooleanFormat(dataObject, CF_ISSHOWINGTEXT, isShowingText);
        }

        /// <summary>
        ///  Sets the UsingDefaultDragImage format into a data object.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///  Specify <see langword="true"/> for <paramref name="usingDefaultDragImage"/> to use a layered window drag image with a size of 96x96.
        /// </para>
        /// </remarks>
        private static void SetUsingDefaultDragImage(IComDataObject dataObject, bool usingDefaultDragImage)
        {
            SetBooleanFormat(dataObject, CF_USINGDEFAULTDRAGIMAGE, usingDefaultDragImage);
        }

        /// <summary>
        ///  Creates an in-process server drag-image manager object and returns the specified interface pointer.
        /// </summary>
        private static bool TryGetDragDropHelper<TDragDropHelper>([NotNullWhen(true)] out TDragDropHelper? dragDropHelper)
        {
            try
            {
                HRESULT hr = Ole32.CoCreateInstance(
                    ref CLSID.DragDropHelper,
                    IntPtr.Zero,
                    Ole32.CLSCTX.INPROC_SERVER,
                    ref NativeMethods.ActiveX.IID_IUnknown,
                    out object obj);
                if (hr.Succeeded())
                {
                    dragDropHelper = (TDragDropHelper)obj;
                    return true;
                }
            }
            catch { }

            dragDropHelper = default;
            return false;
        }
    }
}
