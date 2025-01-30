// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Private.Windows.Graphics;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.UI.Shell;
using static Windows.Win32.System.Memory.GLOBAL_ALLOC_FLAGS;

namespace System.Private.Windows.Ole;

/// <summary>
///  Helper class for drop targets to display the drag image while the cursor is over the target window and allows the
///  application to specify the drag image bitmap that will be displayed during a drag-and-drop operation.
/// </summary>
/// <devdoc>
///  This class effectively requires the data object on events to be an instance of DataObject
///  (via <see cref="IComVisibleDataObject"/>) currently.
/// </devdoc>
internal static unsafe class DragDropHelper<TOleServices, TDataFormat>
    where TOleServices : IOleServices
    where TDataFormat : IDataFormat<TDataFormat>
{
    /// <summary>
    ///  Sets the drop object image and accompanying text back to the default.
    /// </summary>
    public static void ClearDropDescription(IComVisibleDataObject? dataObject)
    {
        if (dataObject is null)
        {
            return;
        }

        SetDropDescription(dataObject, DROPIMAGETYPE.DROPIMAGE_INVALID, string.Empty, string.Empty);
    }

    /// <summary>
    ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the information
    ///  needed to display the drag image.
    /// </summary>
    public static void DragEnter(HWND targetWindowHandle, IDragEvent e)
    {
        if (e.DataObject is not IComVisibleDataObject dataObject)
        {
            return;
        }

        Point point = new(e.X, e.Y);
        DragEnter(targetWindowHandle, dataObject, ref point, e.Effect);
    }

    /// <summary>
    ///  Notifies the drag-image manager that the drop target has been entered, and provides it with the information
    ///  needed to display the drag image.
    /// </summary>
    public static void DragEnter(HWND targetWindowHandle, IComVisibleDataObject dataObject, ref Point point, DROPEFFECT effect)
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper))
        {
            return;
        }

        using var dataObjectScope = ComHelpers.GetComScope<IDataObject>(dataObject);
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
    public static void DragOver(IDragEvent e)
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper))
        {
            return;
        }

        Point point = new(e.X, e.Y);
        dropTargetHelper.Value->DragOver(&point, (DROPEFFECT)(uint)e.Effect);
    }

    /// <summary>
    ///  Notifies the drag-image manager that the object has been dropped, and provides it with the information needed
    ///  to display the drag image.
    /// </summary>
    public static void Drop(IDragEvent e)
    {
        using ComScope<IDropTargetHelper> dropTargetHelper = new(null);
        if (!TryGetDragDropHelper<IDropTargetHelper>(dropTargetHelper) || e.DataObject is not IDataObject.Interface dataObject)
        {
            return;
        }

        Point point = new(e.X, e.Y);
        using var dataObjectScope = ComHelpers.GetComScope<IDataObject>(dataObject);
        dropTargetHelper.Value->Drop(dataObjectScope, in point, (DROPEFFECT)(uint)e.Effect);
    }

    /// <summary>
    ///  Determines whether the data object is in a drag loop.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
    /// </returns>
    public static unsafe bool IsInDragLoop(IDataObjectInternal dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

        // https://learn.microsoft.com/windows/win32/shell/clipboard#cfstr_indragloop
        if (dataObject.GetDataPresent(PInvokeCore.CFSTR_INDRAGLOOP)
            && dataObject.GetData(PInvokeCore.CFSTR_INDRAGLOOP) is DragDropFormat dragDropFormat)
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
    ///  Determines whether the specified format is a drag loop format.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="format"/> is a drag loop format; otherwise <see langword="false"/>.
    /// </returns>
    public static bool IsInDragLoopFormat(FORMATETC format)
    {
        string formatName = DataFormatsCore<TDataFormat>.GetOrAddFormat(format.cfFormat).Name;
        return formatName.Equals(DataFormatNames.DragContext)
            || formatName.Equals(DataFormatNames.DragImageBits)
            || formatName.Equals(DataFormatNames.DragSourceHelperFlags)
            || formatName.Equals(DataFormatNames.DragWindow)
            || formatName.Equals(PInvokeCore.CFSTR_DROPDESCRIPTION)
            || formatName.Equals(PInvokeCore.CFSTR_INDRAGLOOP)
            || formatName.Equals(DataFormatNames.IsShowingLayered)
            || formatName.Equals(DataFormatNames.IsShowingText)
            || formatName.Equals(DataFormatNames.UsingDefaultDragImage);
    }

    /// <summary>
    ///  Releases formats used by the drag-and-drop helper.
    /// </summary>
    public static void ReleaseDragDropFormats(IComVisibleDataObject dataObject)
    {
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
    private static unsafe void SetBooleanFormat(IComVisibleDataObject dataObject, string format, bool value)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        ArgumentException.ThrowIfNullOrEmpty(format);

        FORMATETC formatEtc = new()
        {
            cfFormat = (ushort)(short)PInvokeCore.RegisterClipboardFormat(format),
            dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = null,
            tymed = (uint)TYMED.TYMED_HGLOBAL
        };

        STGMEDIUM medium = new()
        {
            pUnkForRelease = null,
            tymed = TYMED.TYMED_HGLOBAL,
            hGlobal = PInvokeCore.GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, (nuint)sizeof(BOOL))
        };

        if (medium.hGlobal.IsNull)
        {
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        void* basePtr = PInvokeCore.GlobalLock(medium.hGlobal);
        if (basePtr is null)
        {
            PInvokeCore.GlobalFree(medium.hGlobal);
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        *(BOOL*)basePtr = value;
        PInvokeCore.GlobalUnlock(medium.hGlobal);
        dataObject.SetData(&formatEtc, &medium, true);
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
    public static void SetDragImage(IComVisibleDataObject dataObject, IGiveFeedbackEvent e)
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
    public static void SetDragImage(IComVisibleDataObject dataObject, IBitmap? dragImage, Point cursorOffset, bool usingDefaultDragImage)
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

        using HBITMAP hbmpDragImage = dragImage is not null ? dragImage.GetHbitmap() : HBITMAP.Null;

        // The Windows drag image manager will own this bitmap object and free the memory when its finished. Only
        // call DeleteObject if an exception occurs while initializing.
        SHDRAGIMAGE shDragImage = new()
        {
            hbmpDragImage = hbmpDragImage,
            sizeDragImage = dragImage is not null ? dragImage.Size : default,
            ptOffset = cursorOffset,
            crColorKey = (COLORREF)PInvokeCore.GetSysColor(SYS_COLOR_INDEX.COLOR_WINDOW)
        };

        // Allow text specified in DROPDESCRIPTION to be displayed on the drag image. If you pass a drag image into an IDragSourceHelper
        // object, then by default, the extra text description of the drag-and-drop operation is not displayed.
        if (dragSourceHelper.Value->SetFlags((uint)DSH_FLAGS.DSH_ALLOWDROPDESCRIPTIONTEXT).Failed)
        {
            PInvokeCore.DeleteObject(hbmpDragImage);
            return;
        }

        using var dataObjectScope = ComHelpers.GetComScope<IDataObject>(dataObject);
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
    public static void SetDropDescription(IDragEvent e)
    {
        if (e.DataObject is not IComVisibleDataObject dataObject)
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
        IComVisibleDataObject dataObject,
        DROPIMAGETYPE dropImageType,
        string message,
        string messageReplacementToken)
    {
        ArgumentNullException.ThrowIfNull(dataObject);
        if (dropImageType is < DROPIMAGETYPE.DROPIMAGE_INVALID or > DROPIMAGETYPE.DROPIMAGE_NOIMAGE)
        {
            throw new InvalidEnumArgumentException(nameof(dropImageType), (int)dropImageType, typeof(DROPIMAGETYPE));
        }

        if (message.Length >= (int)PInvokeCore.MAX_PATH)
        {
            throw new ArgumentOutOfRangeException(nameof(message));
        }

        if (messageReplacementToken.Length >= (int)PInvokeCore.MAX_PATH)
        {
            throw new ArgumentOutOfRangeException(nameof(messageReplacementToken));
        }

        FORMATETC formatEtc = new()
        {
            cfFormat = (ushort)(short)PInvokeCore.RegisterClipboardFormat(PInvokeCore.CFSTR_DROPDESCRIPTION),
            dwAspect = (uint)DVASPECT.DVASPECT_CONTENT,
            lindex = -1,
            ptd = null,
            tymed = (uint)TYMED.TYMED_HGLOBAL
        };

        STGMEDIUM medium = new()
        {
            pUnkForRelease = null,
            tymed = TYMED.TYMED_HGLOBAL,
            hGlobal = PInvokeCore.GlobalAlloc(GMEM_MOVEABLE | GMEM_ZEROINIT, (nuint)sizeof(DROPDESCRIPTION))
        };

        if (medium.hGlobal.IsNull)
        {
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        void* basePtr = PInvokeCore.GlobalLock(medium.hGlobal);
        if (basePtr is null)
        {
            PInvokeCore.GlobalFree(medium.hGlobal);
            throw new Win32Exception(Marshal.GetLastSystemError(), SR.ExternalException);
        }

        DROPDESCRIPTION* pDropDescription = (DROPDESCRIPTION*)basePtr;
        pDropDescription->type = dropImageType;
        pDropDescription->szMessage = message;
        pDropDescription->szInsert = messageReplacementToken;
        PInvokeCore.GlobalUnlock(medium.hGlobal);

        // Set the InShellDragLoop flag to true to facilitate loading and retrieving arbitrary private formats. The
        // drag-and-drop helper object calls IDataObject::SetData to load private formats--used for cross-process support--into
        // the data object. It later retrieves these formats by calling IDataObject::GetData. The data object's SetData
        // and GetData implementations inspect for the InShellDragLoop flag to know when the data object is in a drag-and-drop
        // loop and needs to load and retrieve the arbitrary private formats.
        if (!IsInDragLoop(dataObject))
        {
            SetInDragLoop(dataObject, inDragLoop: true);
        }

        dataObject.SetData(&formatEtc, &medium, fRelease: true);
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
    public static void SetInDragLoop(IComVisibleDataObject dataObject, bool inDragLoop)
    {
        SetBooleanFormat(dataObject, PInvokeCore.CFSTR_INDRAGLOOP, inDragLoop);

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
    private static void SetIsShowingText(IComVisibleDataObject dataObject, bool isShowingText)
        => SetBooleanFormat(dataObject, DataFormatNames.IsShowingText, isShowingText);

    /// <summary>
    ///  Sets the UsingDefaultDragImage format into a data object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Specify <see langword="true"/> for <paramref name="usingDefaultDragImage"/> to use a layered window
    ///   drag image with a size of 96x96.
    ///  </para>
    /// </remarks>
    private static void SetUsingDefaultDragImage(IComVisibleDataObject dataObject, bool usingDefaultDragImage)
        => SetBooleanFormat(dataObject, DataFormatNames.UsingDefaultDragImage, usingDefaultDragImage);

    /// <summary>
    ///  Creates an in-process server drag-image manager object and returns the specified interface pointer.
    /// </summary>
    private static bool TryGetDragDropHelper<TDragHelper>(TDragHelper** dragDropHelper)
        where TDragHelper : unmanaged, IComIID
    {
        TOleServices.EnsureThreadState();

        HRESULT hr = PInvokeCore.CoCreateInstance(
            CLSID.DragDropHelper,
            pUnkOuter: null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            out *dragDropHelper);

        return hr.Succeeded;
    }
}
