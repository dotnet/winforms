// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32;
using Windows.Win32.System.Com;
using DataFormats = System.Private.Windows.Core.OLE.DesktopDataFormats;
using IDataObject = System.Private.Windows.Core.OLE.IDataObjectDesktop;
using Windows.Win32.UI.Shell;

namespace System.Private.Windows.Core.OLE;

internal static class DragDropHelper
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
    ///  Determines whether the data object is in a drag loop.
    /// </summary>
    /// <returns>
    ///  <see langword="true"/> if <paramref name="dataObject"/> is in a drag-and-drop loop; otherwise <see langword="false"/>.
    /// </returns>
    public static unsafe bool IsInDragLoop(IDataObject dataObject)
    {
        ArgumentNullException.ThrowIfNull(dataObject);

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
        string formatName = DataFormats.GetFormat(format.cfFormat).Name;
        return formatName.Equals(DRAGCONTEXT) || formatName.Equals(DRAGIMAGEBITS) || formatName.Equals(DRAGSOURCEHELPERFLAGS)
            || formatName.Equals(DRAGWINDOW) || formatName.Equals(PInvokeCore.CFSTR_DROPDESCRIPTION) || formatName.Equals(PInvokeCore.CFSTR_INDRAGLOOP)
            || formatName.Equals(ISSHOWINGLAYERED) || formatName.Equals(ISSHOWINGTEXT) || formatName.Equals(USINGDEFAULTDRAGIMAGE);
    }
}
