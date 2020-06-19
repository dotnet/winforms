// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using static Interop;
using static Interop.Ole32;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an IPicture to a System.Drawing.Image.
    /// </summary>
    internal class Com2PictureConverter : Com2DataTypeToManagedDataTypeConverter
    {
        private object _lastManaged;
        private IntPtr _lastNativeHandle;
        private WeakReference _pictureRef;

        private Type _pictureType = typeof(Bitmap);

        public Com2PictureConverter(Com2PropertyDescriptor pd)
        {
            if (pd.DISPID == DispatchID.MOUSEICON || pd.Name.IndexOf("Icon") != -1)
            {
                _pictureType = typeof(Icon);
            }
        }

        /// <summary>
        ///  Returns the managed type that this editor maps the property type to.
        /// </summary>
        public override Type ManagedType
        {
            get
            {
                return _pictureType;
            }
        }

        /// <summary>
        ///  Converts the native value into a managed value
        /// </summary>
        public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd)
        {
            if (nativeValue is null)
            {
                return null;
            }

            Debug.Assert(nativeValue is IPicture, "nativevalue is not IPicture");

            IPicture nativePicture = (IPicture)nativeValue;
            IntPtr handle = (IntPtr)nativePicture.Handle;

            if (_lastManaged != null && handle == _lastNativeHandle)
            {
                return _lastManaged;
            }

            if (handle != IntPtr.Zero)
            {
                switch ((PICTYPE)nativePicture.Type)
                {
                    case PICTYPE.ICON:
                        _pictureType = typeof(Icon);
                        _lastManaged = Icon.FromHandle(handle);
                        break;
                    case PICTYPE.BITMAP:
                        _pictureType = typeof(Bitmap);
                        _lastManaged = Image.FromHbitmap(handle);
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

        /// <summary>
        ///  Converts the managed value into a native value
        /// </summary>
        public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // Don't cancel the set
            cancelSet = false;

            if (_lastManaged?.Equals(managedValue) == true)
            {
                object target = _pictureRef?.Target;
                if (target != null)
                {
                    return target;
                }
            }

            // We have to build an IPicture
            if (managedValue != null)
            {
                BOOL own = BOOL.FALSE;

                PICTDESC pictdesc;
                if (managedValue is Icon icon)
                {
                    pictdesc = PICTDESC.FromIcon(icon, copy: false);
                }
                else if (managedValue is Bitmap bitmap)
                {
                    pictdesc = PICTDESC.FromBitmap(bitmap);
                    own = BOOL.TRUE;
                }
                else
                {
                    Debug.Fail($"Unknown Image type: {managedValue.GetType().Name}");
                    return null;
                }

                Guid iid = typeof(IPicture).GUID;
                IPicture pict = (IPicture)OleCreatePictureIndirect(ref pictdesc, ref iid, own);
                _lastManaged = managedValue;
                _lastNativeHandle = (IntPtr)pict.Handle;
                _pictureRef = new WeakReference(pict);
                return pict;
            }
            else
            {
                _lastManaged = null;
                _lastNativeHandle = IntPtr.Zero;
                _pictureRef = null;
                return null;
            }
        }
    }
}
