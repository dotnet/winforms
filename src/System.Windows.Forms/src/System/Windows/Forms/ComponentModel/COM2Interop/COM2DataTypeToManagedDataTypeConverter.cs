// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    /// <summary>
    ///  This base class maps an ole defined data type (OLE_COLOR, IFont, etc.),
    /// </summary>
    internal abstract class Com2DataTypeToManagedDataTypeConverter
    {
        public virtual bool AllowExpand
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Returns the managed type that this editor maps the property type to.
        /// </summary>
        public abstract Type ManagedType
        {
            get;
        }

        /// <summary>
        ///  Converts the native value into a managed value
        /// </summary>
        public abstract object ConvertNativeToManaged(object nativeValue, Com2PropertyDescriptor pd);

        /// <summary>
        ///  Converts the managed value into a native value
        /// </summary>
        public abstract object ConvertManagedToNative(object managedValue, Com2PropertyDescriptor pd, ref bool cancelSet);
    }
}
