// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;
using Com = Windows.Win32.System.Com;
using ComTypes = System.Runtime.InteropServices.ComTypes;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms;

/// <summary>
///  Helper class for drop targets to display the drag image while the cursor is over the target window and allows the
///  application to specify the drag image bitmap that will be displayed during a drag-and-drop operation.
/// </summary>
internal static unsafe class DragDropHelper
{
    /// <summary>
    ///  A format used internally by the drag image manager.
    /// </summary>
    internal const string DRAGCONTEXT = "DragContext";

    /// <summary>
    ///  A format that contains the drag image bottom-up device-independent bitmap bits.
    /// </summary>
    internal const string DRAGIMAGEBITS = "DragImageBits";

    /// <summary>
    ///  A format that contains the value passed to <see cref="IDragSourceHelper2.Interface.SetFlags(uint)"/>
    ///  and controls whether to allow text specified in <see cref="DROPDESCRIPTION"/> to be displayed on the drag image.
    /// </summary>
    internal const string DRAGSOURCEHELPERFLAGS = "DragSourceHelperFlags";

    /// <summary>
    ///  A format used to identify an object's drag image window so that it's visual information can be updated dynamically.
    /// </summary>
    internal const string DRAGWINDOW = "DragWindow";

    /// <summary>
    ///  A format that is non-zero if the drop target supports drag images.
    /// </summary>
    internal const string ISSHOWINGLAYERED = "IsShowingLayered";

    /// <summary>
    ///  A format that is non-zero if the drop target supports drop description text.
    /// </summary>
    internal const string ISSHOWINGTEXT = "IsShowingText";

    /// <summary>
    ///  A format that is non-zero if the drag image is a layered window with a size of 96x96.
    /// </summary>
    internal const string USINGDEFAULTDRAGIMAGE = "UsingDefaultDragImage";

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
    public static void DragEnter(HWND targetWindowHandle, DragEventArgs e)
    {
        if (e.Data is not IComDataObject dataObject)
        {
            return;
        }

        Point point = new(e.X, e.Y);
        DragEnter(targetWindowHandle, dataObject, ref point, (DROPEFFECT)(uint)e.Effect);
    }

    /// <summary>
    ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the information
    ///  needed to display the drag image.
    /// </summary>
    public static void DragEnter(HWND targetWindowHandle, IComDataObject dataObject, ref Point point, DROPEFFECT effect)
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper))
        {
            return;
        }

        using var dataObjectScope = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        dropTargetHelper.Value->DragEnter(targetWindowHandle, dataObjectScope, in point, effect);
    }

    /// <summary>
    ///  Notifies the drag-image manager that the cursor has left the drop target.
    /// </summary>
    public static void DragLeave()
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper))
        {
            return;
        }

        dropTargetHelper.Value->DragLeave();
    }

    /// <summary>
    ///  Notifies the drag-image manager that the cursor position has changed
    ///  and provides it with the information needed to display the drag image.
    /// </summary>
    public static void DragOver(DragEventArgs e)
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper))
        {
            return;
        }

        Point point = new(e.X, e.Y);
        dropTargetHelper.Value->DragOver(in point, (DROPEFFECT)(uint)e.Effect);
    }

    /// <summary>
    ///  Notifies the drag-image manager that the object has been dropped, and provides it with the information needed
    ///  to display the drag image.
    /// </summary>
    public static void Drop(DragEventArgs e)
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper) || e.Data is not IComDataObject dataObject)
        {
            return;
        }

        Point point = new(e.X, e.Y);
        using var dataObjectScope = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        dropTargetHelper.Value->Drop(dataObjectScope, in point, (DROPEFFECT)(uint)e.Effect);
    }

    /// <summary>
    ///  Gets boolean formats from a data object used to set and display drag images and drop descriptions.
    /// </summary>
    private static unsafe bool GetBooleanFormat(IComDataObject dataObject, string format)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        ArgumentException.ThrowIfNullOrEmpty(format);

        ComTypes.STGMEDIUM medium = default;

        try
        {
            ComTypes.FORMATETC formatEtc = new()
            {
                cfFormat = (short)PInvokeCore.RegisterClipboardFormat(format),
                dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
                lindex = -1,
                ptd = IntPtr.Zero,
                tymed = ComTypes.TYMED.TYMED_HGLOBAL
            };

            if (dataObject.QueryGetData(ref formatEtc) != (int)HRESULT.S_OK)
            {
                return false;
            }

            medium = default;
            dataObject.GetData(ref formatEtc, out medium);
            void* basePtr = PInvokeCore.GlobalLock((HGLOBAL)medium.unionmember);
            return (basePtr is not null) && (*(BOOL*)basePtr == true);
        }
        finally
        {
            PInvokeCore.GlobalUnlock((HGLOBAL)medium.unionmember);
            var comMedium = (STGMEDIUM)medium;
            PInvoke.ReleaseStgMedium(ref comMedium);
        }
    }

    /// <summary>
    ///  Determines whether the data object is in a drag loop.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
    /// </returns>
    public static unsafe bool IsInDragLoop(IDataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        if (dataObject.GetDataPresent(PInvoke.CFSTR_INDRAGLOOP)
            && dataObject.GetData(PInvoke.CFSTR_INDRAGLOOP) is DragDropFormat dragDropFormat)
        {
            try
            {
                void* basePtr = PInvokeCore.GlobalLock(dragDropFormat.Medium.hGlobal);
                return (basePtr is not null) && (*(BOOL*)basePtr == true);
            }
            finally
            {
                PInvokeCore.GlobalUnlock(dragDropFormat.Medium.hGlobal);
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
    ///  <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsInDragLoop(IComDataObject dataObject) => GetBooleanFormat(dataObject, PInvoke.CFSTR_INDRAGLOOP);

    /// <summary>
    ///  Determines whether the specified format is a drag loop format.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="format"/> is a drag loop format; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsInDragLoopFormat(FORMATETC format)
    {
        string formatName = DataFormats.GetFormat(format.cfFormat).Name;
        return formatName.Equals(DRAGCONTEXT) || formatName.Equals(DRAGIMAGEBITS) || formatName.Equals(DRAGSOURCEHELPERFLAGS)
            || formatName.Equals(DRAGWINDOW) || formatName.Equals(PInvoke.CFSTR_DROPDESCRIPTION) || formatName.Equals(PInvoke.CFSTR_INDRAGLOOP)
            || formatName.Equals(ISSHOWINGLAYERED) || formatName.Equals(ISSHOWINGTEXT) || formatName.Equals(USINGDEFAULTDRAGIMAGE);
    }

    /// <summary>
    ///  Releases formats used by the drag-and-drop helper.
    /// </summary>
    public static void ReleaseDragDropFormats(IComDataObject comDataObject)
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
        ArgumentNullException.ThrowIfNull(dataObject);
        ArgumentException.ThrowIfNullOrEmpty(format);

        ComTypes.FORMATETC formatEtc = new()
        {
            cfFormat = (short)PInvokeCore.RegisterClipboardFormat(format),
            dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = IntPtr.Zero,
            tymed = ComTypes.TYMED.TYMED_HGLOBAL
        };

        ComTypes.STGMEDIUM medium = new()
        {
            pUnkForRelease = null,
            tymed = ComTypes.TYMED.TYMED_HGLOBAL,
            unionmember = PInvokeCore.GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, (nuint)sizeof(BOOL))
        };

        if (medium.unionmember == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        void* basePtr = PInvokeCore.GlobalLock((HGLOBAL)medium.unionmember);
        if (basePtr is null)
        {
            PInvokeCore.GlobalFree((HGLOBAL)medium.unionmember);
            medium.unionmember = IntPtr.Zero;
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        *(BOOL*)basePtr = value;
        PInvokeCore.GlobalUnlock((HGLOBAL)medium.unionmember);
        dataObject.SetData(ref formatEtc, ref medium, release: true);
    }

    /// <summary>
    ///  Initializes the drag-image manager and sets the drag image bitmap into a data object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Because InitializeFromBitmap always performs the RGB multiplication step in calculating the alpha value, you should
    ///   always pass a bitmap without pre-multiplied alpha blending. Note that no error will result from passing a bitmap
    ///   with pre-multiplied alpha blending, but this method will multiply it again, doubling the resulting alpha value.
    ///  </para>
    /// </remarks>
    public static void SetDragImage(IComDataObject dataObject, GiveFeedbackEventArgs e)
    {
        ArgumentNullException.ThrowIfNull(e);
        SetDragImage(dataObject, e.DragImage, e.CursorOffset, e.UseDefaultDragImage);
    }

    /// <summary>
    ///  Initializes the drag-image manager and sets the drag image bitmap into a data object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Because InitializeFromBitmap always performs the RGB multiplication step in calculating the alpha value, you should
    ///   always pass a bitmap without pre-multiplied alpha blending. Note that no error will result from passing a bitmap
    ///   with pre-multiplied alpha blending, but this method will multiply it again, doubling the resulting alpha value.
    ///  </para>
    /// </remarks>
    public static void SetDragImage(IComDataObject dataObject, Bitmap? dragImage, Point cursorOffset, bool usingDefaultDragImage)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        using ComScope<IDragSourceHelper2> dragSourceHelper = new(null);
        if (!TryGetDragDropHelper<IDragSourceHelper2>(dragSourceHelper))
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
            SetInDragLoop(dataObject, inDragLoop: true);
        }

        using HBITMAP hbmpDragImage = dragImage is not null ? dragImage.GetHBITMAP() : HBITMAP.Null;

        // The Windows drag image manager will own this bitmap object and free the memory when its finished. Only
        // call DeleteObject if an exception occurs while initializing.
        SHDRAGIMAGE shDragImage = new()
        {
            hbmpDragImage = hbmpDragImage,
            sizeDragImage = dragImage is not null ? dragImage.Size : default,
            ptOffset = cursorOffset,
            crColorKey = (COLORREF)PInvoke.GetSysColor(SYS_COLOR_INDEX.COLOR_WINDOW)
        };

        // Allow text specified in DROPDESCRIPTION to be displayed on the drag image. If you pass a drag image into an IDragSourceHelper
        // object, then by default, the extra text description of the drag-and-drop operation is not displayed.
        if (dragSourceHelper.Value->SetFlags((uint)DSH_FLAGS.DSH_ALLOWDROPDESCRIPTIONTEXT).Failed)
        {
            PInvokeCore.DeleteObject(hbmpDragImage);
            return;
        }

        using var dataObjectScope = ComHelpers.GetComScope<Com.IDataObject>(dataObject);
        if (dragSourceHelper.Value->InitializeFromBitmap(shDragImage, dataObjectScope).Failed)
        {
            return;
        }

        // To effectively display the drop description after changing the drag image bitmap, set IsShowingText to true;
        // otherwise the drop description text will not be displayed.
        SetIsShowingText(dataObject, isShowingText: true);
        SetUsingDefaultDragImage(dataObject, usingDefaultDragImage);
    }

    /// <summary>
    ///  Sets the drop description into a data object. Describes the image and accompanying text for a drop object.
    /// </summary>
    public static void SetDropDescription(DragEventArgs e)
    {
        if (e.Data is not IComDataObject dataObject)
        {
            return;
        }

        e.Message ??= string.Empty;
        e.MessageReplacementToken ??= string.Empty;
        SetDropDescription(dataObject, e.DropImageType, e.Message, e.MessageReplacementToken);
    }

    /// <summary>
    ///  Sets the drop description into a data object. Describes the image and accompanying text for a drop object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Some UI coloring is applied to the text in <paramref name="message"/> if used by specifying %1 in
    ///   <paramref name="messageReplacementToken"/>. The characters %% and %1 are the subset of FormatMessage
    ///   markers that are processed here.
    ///  </para>
    /// </remarks>
    public static unsafe void SetDropDescription(
        IComDataObject dataObject,
        DropImageType dropImageType,
        string message,
        string messageReplacementToken)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        SourceGenerated.EnumValidator.Validate(dropImageType, nameof(dropImageType));

        if (message.Length >= (int)PInvokeCore.MAX_PATH)
        {
            throw new ArgumentOutOfRangeException(nameof(message));
        }

        if (messageReplacementToken.Length >= (int)PInvokeCore.MAX_PATH)
        {
            throw new ArgumentOutOfRangeException(nameof(messageReplacementToken));
        }

        ComTypes.FORMATETC formatEtc = new()
        {
            cfFormat = (short)PInvokeCore.RegisterClipboardFormat(PInvoke.CFSTR_DROPDESCRIPTION),
            dwAspect = ComTypes.DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = IntPtr.Zero,
            tymed = ComTypes.TYMED.TYMED_HGLOBAL
        };

        ComTypes.STGMEDIUM medium = new()
        {
            pUnkForRelease = null,
            tymed = ComTypes.TYMED.TYMED_HGLOBAL,
            unionmember = PInvokeCore.GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, (uint)sizeof(DROPDESCRIPTION))
        };

        if (medium.unionmember == IntPtr.Zero)
        {
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        void* basePtr = PInvokeCore.GlobalLock((HGLOBAL)medium.unionmember);
        if (basePtr is null)
        {
            PInvokeCore.GlobalFree((HGLOBAL)medium.unionmember);
            medium.unionmember = IntPtr.Zero;
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
        pDropDescription->type = (DROPIMAGETYPE)dropImageType;
        pDropDescription->szMessage = message;
        pDropDescription->szInsert = messageReplacementToken;
        PInvokeCore.GlobalUnlock((HGLOBAL)medium.unionmember);

        // Set the InShellDragLoop flag to true to facilitate loading and retrieving arbitrary private formats. The
        // drag-and-drop helper object calls IDataObject::SetData to load private formats--used for cross-process support--into
        // the data object. It later retrieves these formats by calling IDataObject::GetData. The data object's SetData
        // and GetData implementations inspect for the InShellDragLoop flag to know when the data object is in a drag-and-drop
        // loop and needs to load and retrieve the arbitrary private formats.
        if (!IsInDragLoop(dataObject))
        {
            SetInDragLoop(dataObject, inDragLoop: true);
        }

        dataObject.SetData(ref formatEtc, ref medium, release: true);
        SetIsShowingText(dataObject, isShowingText: true);
    }

    /// <summary>
    ///  Sets the InDragLoop format into a data object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Used to determine whether the data object is in a drag loop.
    ///  </para>
    /// </remarks>
    public static void SetInDragLoop(IComDataObject dataObject, bool inDragLoop)
    {
        SetBooleanFormat(dataObject, PInvoke.CFSTR_INDRAGLOOP, inDragLoop);

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
    ///   To effectively display the drop description after changing the drag image bitmap, set <paramref name="isShowingText"/>
    ///   to true; otherwise the drop description text will not be displayed.
    ///  </para>
    /// </remarks>
    private static void SetIsShowingText(IComDataObject dataObject, bool isShowingText)
        => SetBooleanFormat(dataObject, ISSHOWINGTEXT, isShowingText);

    /// <summary>
    ///  Sets the UsingDefaultDragImage format into a data object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Specify <see langword="true"/> for <paramref name="usingDefaultDragImage"/> to use a layered window
    ///   drag image with a size of 96x96.
    ///  </para>
    /// </remarks>
    private static void SetUsingDefaultDragImage(IComDataObject dataObject, bool usingDefaultDragImage)
        => SetBooleanFormat(dataObject, USINGDEFAULTDRAGIMAGE, usingDefaultDragImage);

    /// <summary>
    ///  Creates an in-process server drag-image manager object and returns the specified interface pointer.
    /// </summary>
    private static bool TryGetDragDropHelper<TDragHelper>(TDragHelper** dragDropHelper)
        where TDragHelper : unmanaged, IComIID
    {
        if (Control.CheckForIllegalCrossThreadCalls && Application.OleRequired() != ApartmentState.STA)
        {
            throw new InvalidOperationException(SR.ThreadMustBeSTA);
        }

        HRESULT hr = PInvokeCore.CoCreateInstance(
            CLSID.DragDropHelper,
            pUnkOuter: null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            out *dragDropHelper);

        return hr.Succeeded;
    }
}
