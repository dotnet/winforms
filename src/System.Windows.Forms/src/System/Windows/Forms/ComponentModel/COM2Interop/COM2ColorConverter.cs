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

    /// <include file='doc\COM2ColorConverter.uex' path='docs/doc[@for="Com2ColorConverter"]/*' />
    /// <devdoc>
    /// This class maps an OLE_COLOR to a managed Color editor.
    /// </devdoc>
    internal class Com2ColorConverter : Com2DataTypeToManagedDataTypeConverter{

         /// <include file='doc\COM2ColorConverter.uex' path='docs/doc[@for="Com2ColorConverter.ManagedType"]/*' />
         /// <devdoc>
         ///     Returns the managed type that this editor maps the property type to.
         /// </devdoc>
         public override Type ManagedType{
            get{
               return typeof(Color);
            }
         }

         /// <include file='doc\COM2ColorConverter.uex' path='docs/doc[@for="Com2ColorConverter.ConvertNativeToManaged"]/*' />
         /// <devdoc>
         ///     Converts the native value into a managed value
         /// </devdoc>
         public override object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd){
               object baseValue = nativeValue;
               int intVal = 0;

               // get the integer value out of the native...
               //
               if (nativeValue is UInt32){
                  intVal = (int)(UInt32)nativeValue;
               }
               else if (nativeValue is Int32){
                  intVal = (int)nativeValue;
               }

               return ColorTranslator.FromOle(intVal);
         }

         /// <include file='doc\COM2ColorConverter.uex' path='docs/doc[@for="Com2ColorConverter.ConvertManagedToNative"]/*' />
         /// <devdoc>
         ///     Converts the managed value into a native value
         /// </devdoc>
         public override object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet){
               // don't cancel the set
               cancelSet = false;

               // we default to black.
               //
               if (managedValue == null){
                  managedValue = Color.Black;
               }

               if (managedValue is Color){
                  return ColorTranslator.ToOle(((Color)managedValue));

               }
               Debug.Fail("Don't know how to set type:" + managedValue.GetType().Name);
               return 0;
         }

    }
}

