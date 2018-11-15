// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;    
    using System.Drawing;    
    using System.Collections;
    using Microsoft.Win32;
    using System.Runtime.Versioning;

    /// <include file='doc\COM2PictureConverter.uex' path='docs/doc[@for="Com2PictureConverter"]/*' />
    /// <devdoc>
    /// This class maps an IPicture to a System.Drawing.Image.
    /// </devdoc>
    internal class Com2PictureConverter : Com2DataTypeToManagedDataTypeConverter {

        object lastManaged;
        IntPtr lastNativeHandle;
        WeakReference pictureRef;
        IntPtr lastPalette = IntPtr.Zero;

        Type pictureType = typeof(Bitmap);

        public Com2PictureConverter(Com2PropertyDescriptor pd) {
            if (pd.DISPID == NativeMethods.ActiveX.DISPID_MOUSEICON || pd.Name.IndexOf("Icon") != -1) {
                pictureType = typeof(Icon);
            }
        }

        /// <include file='doc\COM2PictureConverter.uex' path='docs/doc[@for="Com2PictureConverter.ManagedType"]/*' />
        /// <devdoc>
        ///     Returns the managed type that this editor maps the property type to.
        /// </devdoc>
        public override Type ManagedType {
            get {
                return pictureType;
            }
        }

        /// <include file='doc\COM2PictureConverter.uex' path='docs/doc[@for="Com2PictureConverter.ConvertNativeToManaged"]/*' />
        /// <devdoc>
        ///     Converts the native value into a managed value
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd) {

            if (nativeValue == null) {
                return null;
            }

            Debug.Assert(nativeValue is UnsafeNativeMethods.IPicture, "nativevalue is not IPicture");

            UnsafeNativeMethods.IPicture nativePicture = (UnsafeNativeMethods.IPicture)nativeValue;
            IntPtr handle = nativePicture.GetHandle();

            if (lastManaged != null && handle == lastNativeHandle) {
                return lastManaged;
            }

            lastNativeHandle = handle;
            //lastPalette = nativePicture.GetHPal();
            if (handle != IntPtr.Zero) {
                switch (nativePicture.GetPictureType()) {
                    case  NativeMethods.Ole.PICTYPE_ICON:
                        pictureType = typeof(Icon);
                        lastManaged = Icon.FromHandle(handle);
                        break;
                    case   NativeMethods.Ole.PICTYPE_BITMAP:
                        pictureType = typeof(Bitmap);
                        lastManaged = Image.FromHbitmap(handle);
                        break;
                    default:
                        Debug.Fail("Unknown picture type");
			break;
                }
                pictureRef = new WeakReference(nativePicture);
            }
            else {
                lastManaged = null;
                pictureRef = null;
            }
            return lastManaged;
        }

        /// <include file='doc\COM2PictureConverter.uex' path='docs/doc[@for="Com2PictureConverter.ConvertManagedToNative"]/*' />
        /// <devdoc>
        ///     Converts the managed value into a native value
        /// </devdoc>
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet) {
            // don't cancel the set
            cancelSet = false;

            if (lastManaged != null && lastManaged.Equals(managedValue) && pictureRef != null && pictureRef.IsAlive) {
                return pictureRef.Target;
            }

            // we have to build an IPicture
            lastManaged = managedValue;

            if (managedValue != null) {
                Guid g = typeof(UnsafeNativeMethods.IPicture).GUID;
                NativeMethods.PICTDESC pictdesc = null;
                bool own = false;

                if (lastManaged is Icon) {
                    pictdesc = NativeMethods.PICTDESC.CreateIconPICTDESC(((Icon)lastManaged).Handle);
                }
                else if (lastManaged is Bitmap) {
                    pictdesc = NativeMethods.PICTDESC.CreateBitmapPICTDESC(((Bitmap)lastManaged).GetHbitmap(), lastPalette);
                    own = true;
                }
                else {
                    Debug.Fail("Unknown Image type: " + managedValue.GetType().Name);
                }

                UnsafeNativeMethods.IPicture pict = UnsafeNativeMethods.OleCreatePictureIndirect(pictdesc, ref g, own);
                lastNativeHandle = pict.GetHandle();
                pictureRef = new WeakReference(pict);
                return pict;
            }
            else {
                lastManaged = null;
                lastNativeHandle = lastPalette = IntPtr.Zero;
                pictureRef = null;
                return null;
            }
        }
    }
}

