// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Private.Windows.Core.OLE;
using System.Reflection.Metadata;
using Com = Windows.Win32.System.Com;

namespace System.Windows.Forms;

internal partial class WinFormsDataObject : DesktopDataObject
{
    private readonly DataObject? _dataObject;

    private bool _haveCheckedOverride;

    internal unsafe WinFormsDataObject(Com.IDataObject* data, DataObject? dataObject = null) : base(data, new Composition()) { _dataObject = dataObject; }

    public WinFormsDataObject(DataObject? dataObject = null) : base(new Composition()) { _dataObject = dataObject; }

    public WinFormsDataObject(object data, DataObject? dataObject = null) : base(data, new Composition()) { _dataObject = dataObject; }

    public WinFormsDataObject(string format, object data, DataObject? dataObject = null) : base(format, data, new Composition()) { _dataObject = dataObject; }

    internal WinFormsDataObject(string format, bool autoConvert, object data, DataObject? dataObject = null) : base(format, autoConvert, data, new Composition()) { _dataObject = dataObject; }

    public override IDataObjectDesktop CreateIDataObject() => new WinFormsDataStore();

    public static bool IsValidFormatAndTypeCore<T>(string format)
    {
        if (string.IsNullOrWhiteSpace(format))
        {
            return false;
        }

        if (IsValidPredefinedFormatTypeCombination(format))
        {
            return true;
        }

        throw new NotSupportedException(string.Format(
            SR.ClipboardOrDragDrop_InvalidFormatTypeCombination,
            typeof(T).FullName, format));

        static bool IsValidPredefinedFormatTypeCombination(string format) => format switch
        {
            DesktopDataFormats.TextConstant
                or DesktopDataFormats.UnicodeTextConstant
                or DesktopDataFormats.StringConstant
                or DesktopDataFormats.RtfConstant
                or DesktopDataFormats.HtmlConstant
                or DesktopDataFormats.OemTextConstant => typeof(string) == typeof(T),

            DesktopDataFormats.FileDropConstant
                or CF_DEPRECATED_FILENAME
                or CF_DEPRECATED_FILENAMEW => typeof(string[]) == typeof(T),

            DesktopDataFormats.BitmapConstant or BitmapFullName => typeof(Bitmap) == typeof(T) || typeof(Image) == typeof(T),
            _ => true
        };
    }

    internal override bool IsValidFormatAndType<T>(string format) => IsValidFormatAndTypeCore<T>(format);

    public override bool IsKnownType<T>() => DesktopDataObject.Composition.Binder.IsKnownTypeCore<T>() || typeof(T) == typeof(ImageListStreamer) || typeof(T) == typeof(Bitmap);

    public override void CheckDataObjectForJsonSet<T>()
    {
        if (typeof(T) == typeof(DesktopDataObject) || typeof(T) == typeof(DataObject))
        {
            throw new InvalidOperationException(string.Format(SR.ClipboardOrDragDrop_CannotJsonSerializeDataObject, nameof(SetData)));
        }
    }

    internal override bool TryGetDataCore<[DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes)(-1))] T>(string format, Func<TypeName, Type>? resolver, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data)
    {
        if (!_haveCheckedOverride && _dataObject is not null && _dataObject.GetType() != typeof(DataObject))
        {
            // TryGetDataCore could be overridden. Call the potential overridden version and flag that it's been called so that
            // we don't end up in an infinite loop if it hasn't been overridden.
            _haveCheckedOverride = true;
             bool result = _dataObject.TryGetDataCoreInternal(format, resolver, autoConvert, out data);
            _haveCheckedOverride = false;
            return result;
        }

        return base.TryGetDataCore(format, resolver, autoConvert, out data);
    }
}
