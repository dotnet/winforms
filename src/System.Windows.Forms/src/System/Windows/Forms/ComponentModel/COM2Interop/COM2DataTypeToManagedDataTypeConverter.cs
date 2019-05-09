// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {
    using System.Runtime.Serialization.Formatters;
    using System.ComponentModel;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.ComponentModel.Design;
    using System.Collections;
    using Microsoft.Win32;

    /// <devdoc>
    /// This base class maps an ole defined data type (OLE_COLOR, IFont, etc.),
    ///
    /// </devdoc>
    internal abstract class Com2DataTypeToManagedDataTypeConverter{


         public virtual bool AllowExpand {
             get {
                 return false;
             }
         }

         /// <devdoc>
         ///     Returns the managed type that this editor maps the property type to.
         /// </devdoc>
         public abstract Type ManagedType{
            get;
         }

         /// <devdoc>
         ///     Converts the native value into a managed value
         /// </devdoc>
         public abstract object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd);

         /// <devdoc>
         ///     Converts the managed value into a native value
         /// </devdoc>
         public abstract object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet);
    }
}

