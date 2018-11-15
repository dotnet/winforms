// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;    
    using System.Drawing;
    using System.Collections;
    using Microsoft.Win32;
    using System.Runtime.Versioning;

    /// <include file='doc\COM2FontConverter.uex' path='docs/doc[@for="Com2FontConverter"]/*' />
    /// <devdoc>
    /// This class maps an OLE_COLOR to a managed Color editor.
    /// </devdoc>
    internal class Com2FontConverter : Com2DataTypeToManagedDataTypeConverter {

        private IntPtr lastHandle = IntPtr.Zero;
        private Font lastFont  = null;

        public override bool AllowExpand {
             get {
                 return true;
             }
         }

        /// <include file='doc\COM2FontConverter.uex' path='docs/doc[@for="Com2FontConverter.ManagedType"]/*' />
        /// <devdoc>
        ///     Returns the managed type that this editor maps the property type to.
        /// </devdoc>
        public override Type ManagedType {
            get {
                return typeof(Font);
            }
        }


        /// <include file='doc\COM2FontConverter.uex' path='docs/doc[@for="Com2FontConverter.ConvertNativeToManaged"]/*' />
        /// <devdoc>
        ///     Converts the native value into a managed value
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd) {
            // we're getting an IFont thing here
            UnsafeNativeMethods.IFont nativeFont = nativeValue as UnsafeNativeMethods.IFont;

            if (nativeFont == null) {
                lastHandle = IntPtr.Zero;
                lastFont = Control.DefaultFont;
                return lastFont;
            }

            IntPtr fontHandle = nativeFont.GetHFont();

            // see if we have this guy cached
            if (fontHandle == lastHandle && lastFont != null) {
                return lastFont;
            }

            lastHandle = fontHandle;
            
            try {
                // this wasn't working because it was converting everything to 
                // world units.
                //
                Font font = Font.FromHfont(lastHandle);
                try {
                    lastFont = ControlPaint.FontInPoints(font);
                } 
                finally {
                    font.Dispose();
                }
            }
            catch(ArgumentException) {
                // we will fail on non-truetype fonts, so 
                // just use the default font.
                lastFont = Control.DefaultFont;
            }
            
            return lastFont;
        }

        /// <include file='doc\COM2FontConverter.uex' path='docs/doc[@for="Com2FontConverter.ConvertManagedToNative"]/*' />
        /// <devdoc>
        ///     Converts the managed value into a native value
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet) {

            // we default to black.
            //
            if (managedValue == null) {
                managedValue = Control.DefaultFont;
            }

            cancelSet = true;

            if (lastFont != null && lastFont.Equals(managedValue)) {
                // don't do anything here.
                return null;
            }


            lastFont = (Font)managedValue;
            UnsafeNativeMethods.IFont nativeFont = (UnsafeNativeMethods.IFont)pd.GetNativeValue(pd.TargetObject);

            // now, push all the values into the native side
            if (nativeFont != null) {
                bool changed = ControlPaint.FontToIFont(lastFont, nativeFont);

                if (changed) {
                    // here, we want to pick up a new font from the handle
                    lastFont = null;
                    ConvertNativeToManaged(nativeFont, pd);

                }
            }
            return null;
        }

    }
}

