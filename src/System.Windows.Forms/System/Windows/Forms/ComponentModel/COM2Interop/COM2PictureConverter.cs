// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.Graphics.GdiPlus;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;

namespace System.Windows.Forms.ComponentModel.Com2Interop;

/// <summary>
///  This class maps an IPicture to a System.Drawing.Image.
/// </summary>
internal sealed unsafe class Com2PictureConverter : Com2DataTypeToManagedDataTypeConverter
{
    private object? _lastManaged;

    private OLE_HANDLE _lastNativeHandle;

    private Type _pictureType = typeof(Bitmap);

    public Com2PictureConverter(Com2PropertyDescriptor property)
    {
        if (property.DISPID == PInvokeCore.DISPID_MOUSEICON || property.Name.Contains("Icon"))
        {
            _pictureType = typeof(Icon);
        }
    }

    public override Type ManagedType => _pictureType;

    public override object? ConvertNativeToManaged(VARIANT nativeValue, Com2PropertyDescriptor property)
    {
        if (nativeValue.Type != VARENUM.VT_UNKNOWN)
        {
            Debug.Assert(nativeValue.IsEmpty, $"Expected IUnknown, got {nativeValue.Type}");
            return null;
        }

        using var picture = ComScope<IPicture>.TryQueryFrom((IUnknown*)nativeValue, out HRESULT hr);

        if (hr.Failed)
        {
            Debug.Fail($"Failed to get IPicture: {hr}");
            return null;
        }

        picture.Value->get_Handle(out OLE_HANDLE handle).ThrowOnFailure();

        if (_lastManaged is not null && handle == _lastNativeHandle)
        {
            return _lastManaged;
        }

        if (handle != 0)
        {
            // GDI handles are sign extended 32 bit values.
            // We need to first cast to int so sign extension happens correctly.
            nint extendedHandle = (int)handle.Value;
            picture.Value->get_Type(out PICTYPE type).ThrowOnFailure();

            switch (type)
            {
                case PICTYPE.PICTYPE_ICON:
                    _pictureType = typeof(Icon);
                    _lastManaged = Icon.FromHandle(extendedHandle);
                    break;
                case PICTYPE.PICTYPE_BITMAP:
                    _pictureType = typeof(Bitmap);
                    _lastManaged = Image.FromHbitmap(extendedHandle);
                    break;
                default:
                    Debug.Fail("Unknown picture type");
                    return null;
            }

            _lastNativeHandle = handle;
        }
        else
        {
            _lastManaged = null;
        }

        return _lastManaged;
    }

    public override VARIANT ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor property, ref bool cancelSet)
    {
        if (managedValue == _lastManaged)
        {
            // There should be no point in setting the same object back for this property.
            cancelSet = true;
            return VARIANT.Empty;
        }

        cancelSet = false;

        // Build the IPicture.
        if (managedValue is not null)
        {
            BOOL own = false;

            PICTDESC pictdesc;
            if (managedValue is Icon icon)
            {
                pictdesc = icon.CreatePICTDESC(copy: false);
            }
            else if (managedValue is Bitmap bitmap)
            {
                pictdesc = bitmap.CreatePICTDESC();
                own = true;
            }
            else
            {
                Debug.Fail($"Unknown Image type: {managedValue.GetType().Name}");
                return VARIANT.Empty;
            }

            using ComScope<IPicture> picture = new(null);
            PInvokeCore.OleCreatePictureIndirect(&pictdesc, IID.Get<IPicture>(), own, picture).ThrowOnFailure();
            _lastManaged = managedValue;
            picture.Value->get_Handle(out _lastNativeHandle).ThrowOnFailure();
            IUnknown* unknown;
            picture.Value->QueryInterface(IID.Get<IUnknown>(), (void**)&unknown).ThrowOnFailure();
            return (VARIANT)unknown;
        }
        else
        {
            _lastManaged = null;
            _lastNativeHandle = default;
            return VARIANT.Empty;
        }
    }
}
