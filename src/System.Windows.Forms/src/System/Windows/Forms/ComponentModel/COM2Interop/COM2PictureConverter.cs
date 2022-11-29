// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an IPicture to a System.Drawing.Image.
    /// </summary>
    internal unsafe class Com2PictureConverter : Com2DataTypeToManagedDataTypeConverter
    {
        private object? _lastManaged;

        private OLE_HANDLE _lastNativeHandle;
        private WeakReference? _pictureRef;

        private Type _pictureType = typeof(Bitmap);

        public Com2PictureConverter(Com2PropertyDescriptor pd)
        {
            if (pd.DISPID == DispatchID.MOUSEICON || pd.Name.Contains("Icon"))
            {
                _pictureType = typeof(Icon);
            }
        }

        public override Type ManagedType => _pictureType;

        public override object? ConvertNativeToManaged(object? nativeValue, Com2PropertyDescriptor pd)
        {
            if (nativeValue is null)
            {
                return null;
            }

            Debug.Assert(nativeValue is IPicture.Interface, "nativevalue is not IPicture");

            IPicture.Interface nativePicture = (IPicture.Interface)nativeValue;
            OLE_HANDLE handle = nativePicture.Handle;

            if (_lastManaged is not null && handle == _lastNativeHandle)
            {
                return _lastManaged;
            }

            if (handle != 0)
            {
                // GDI handles are sign extended 32 bit values.
                // We need to first cast to int so sign extension happens correctly.
                nint extendedHandle = (int)handle.Value;
                short type = nativePicture.Type;
                switch ((PICTYPE)type)
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
                _pictureRef = new WeakReference(nativePicture);
            }
            else
            {
                _lastManaged = null;
                _pictureRef = null;
            }

            return _lastManaged;
        }

        public override object? ConvertManagedToNative(object? managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // Don't cancel the set.
            cancelSet = false;

            if (_lastManaged?.Equals(managedValue) == true)
            {
                object? target = _pictureRef?.Target;
                if (target is not null)
                {
                    return target;
                }
            }

            // We have to build an IPicture.
            if (managedValue is not null)
            {
                BOOL own = false;

                PICTDESC pictdesc;
                if (managedValue is Icon icon)
                {
                    pictdesc = PICTDESC.FromIcon(icon, copy: false);
                }
                else if (managedValue is Bitmap bitmap)
                {
                    pictdesc = PICTDESC.FromBitmap(bitmap);
                    own = true;
                }
                else
                {
                    Debug.Fail($"Unknown Image type: {managedValue.GetType().Name}");
                    return null;
                }

                using ComScope<IPicture> picture = new(null);
                PInvoke.OleCreatePictureIndirect(&pictdesc, IID.Get<IPicture>(), own, picture).ThrowOnFailure();
                _lastManaged = managedValue;
                _lastNativeHandle = picture.Value->Handle;
                var pictureObject = Marshal.GetObjectForIUnknown(picture);
                _pictureRef = new WeakReference(pictureObject);
                return pictureObject;
            }
            else
            {
                _lastManaged = null;
                _lastNativeHandle = default;
                _pictureRef = null;
                return null;
            }
        }
    }
}
