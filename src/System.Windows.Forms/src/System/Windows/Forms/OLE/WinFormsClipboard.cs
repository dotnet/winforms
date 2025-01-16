// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.OLE;
using Com = Windows.Win32.System.Com;
using CoreSR = System.Private.Windows.Core.Resources.SR;

namespace System.Windows.Forms;
internal class WinFormsClipboard : DesktopClipboard
{
    public override void EnsureStaThread()
    {
        if (Application.OleRequired() != ApartmentState.STA)
        {
            throw new ThreadStateException(SR.ThreadMustBeSTA);
        }
    }

    internal override DesktopDataObject CreateDataObject(string format, object data) => new WinFormsDataObject(format, data);
    internal override DesktopDataObject CreateDataObject() => new WinFormsDataObject();
    internal override DesktopDataObject CreateDataObject(string format, bool autoConvert, object data) => new WinFormsDataObject(format, autoConvert, data);

    internal override bool GetTypedDataObject<T>(string format, [MaybeNullWhen(false), NotNullWhen(true)] out ITypedDataObjectDesktop typed)
    {
        typed = default;
        if (!WinFormsDataObject.IsValidFormatAndTypeCore<T>(format)
            || GetDataObject() is not { } dataObject)
        {
            // Invalid format or no object on the clipboard at all.
            return false;
        }

        if (dataObject is not ITypedDataObjectDesktop typedDataObject)
        {
            throw new NotSupportedException(string.Format(CoreSR.ITypeDataObject_Not_Implemented, dataObject.GetType().FullName));
        }

        typed = typedDataObject;
        return true;
    }

    internal override unsafe DesktopDataObject WrapForGetDataObject(Com.IDataObject* dataObject) =>
        new WinFormsDataObject(dataObject);

    internal override DesktopDataObject WrapForSetDataObject(object data) =>
        data as DesktopDataObject ?? new WinFormsDataObject(data is IDataObject ido ? new DataObjectAdapter(ido) : data) { IsOriginalNotIDataObject = data is not IDataObject };
}
