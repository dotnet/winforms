// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;

using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This class maps an IPicture to a System.Drawing.Image.
    /// </summary>
    internal class Com2PictureConverter : Com2DataTypeToManagedDataTypeConverter
    {
        object lastManaged;
        IntPtr lastNativeHandle;
        WeakReference pictureRef;
        IntPtr lastPalette = IntPtr.Zero;

        Type pictureType = typeof(Bitmap);

        public Com2PictureConverter(Com2PropertyDescriptor pd)
        {
            if (pd.DISPID == NativeMethods.ActiveX.DISPID_MOUSEICON || pd.Name.IndexOf("Icon") != -1)
            {
                pictureType = typeof(Icon);
            }
        }

        /// <summary>
        ///  Returns the managed type that this editor maps the property type to.
        /// </summary>
        public override Type ManagedType
        {
            get
            {
                return pictureType;
            }
        }

        /// <summary>
        ///  Converts the native value into a managed value
        /// </summary>
        public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd)
        {
            if (nativeValue == null)
            {
                return null;
            }

            Debug.Assert(nativeValue is Ole32.IPicture, "nativevalue is not IPicture");

            Ole32.IPicture nativePicture = (Ole32.IPicture)nativeValue;
            IntPtr handle = nativePicture.Handle;

            if (lastManaged != null && handle == lastNativeHandle)
            {
                return lastManaged;
            }

            lastNativeHandle = handle;

            if (handle != IntPtr.Zero)
            {
                switch ((Ole32.PICTYPE)nativePicture.Type)
                {
                    case Ole32.PICTYPE.ICON:
                        pictureType = typeof(Icon);
                        lastManaged = Icon.FromHandle(handle);
                        break;
                    case Ole32.PICTYPE.BITMAP:
                        pictureType = typeof(Bitmap);
                        lastManaged = Image.FromHbitmap(handle);
                        break;
                    default:
                        Debug.Fail("Unknown picture type");
                        break;
                }
                pictureRef = new WeakReference(nativePicture);
            }
            else
            {
                lastManaged = null;
                pictureRef = null;
            }
            return lastManaged;
        }

        /// <summary>
        ///  Converts the managed value into a native value
        /// </summary>
        public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet)
        {
            // don't cancel the set
            cancelSet = false;

            if (lastManaged != null && lastManaged.Equals(managedValue) && pictureRef != null && pictureRef.IsAlive)
            {
                return pictureRef.Target;
            }

            // We have to build an IPicture

            lastManaged = managedValue;

            if (managedValue != null)
            {
                Ole32.PICTDESC pictdesc = default;
                BOOL own = BOOL.FALSE;

                if (lastManaged is Icon icon)
                {
                    pictdesc = Ole32.PICTDESC.FromIcon(icon, copy: false);
                }
                else if (lastManaged is Bitmap bitmap)
                {
                    pictdesc = Ole32.PICTDESC.FromBitmap(bitmap, lastPalette);
                    own = BOOL.TRUE;
                }
                else
                {
                    Debug.Fail($"Unknown Image type: {managedValue.GetType().Name}");
                }

                Guid iid = typeof(Ole32.IPicture).GUID;
                Ole32.IPicture pict = (Ole32.IPicture)Ole32.OleCreatePictureIndirect(ref pictdesc, ref iid, own);
                lastNativeHandle = pict.Handle;
                pictureRef = new WeakReference(pict);
                return pict;
            }
            else
            {
                lastManaged = null;
                lastNativeHandle = lastPalette = IntPtr.Zero;
                pictureRef = null;
                return null;
            }
        }
    }
}
