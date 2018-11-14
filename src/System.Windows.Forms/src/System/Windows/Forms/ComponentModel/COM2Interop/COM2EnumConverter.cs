// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.ComponentModel.Com2Interop {

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Collections;
    using Microsoft.Win32;
    using System.Globalization;

    internal class Com2EnumConverter : TypeConverter {

        internal readonly Com2Enum com2Enum;
        private  StandardValuesCollection values;

        public Com2EnumConverter(Com2Enum enumObj) {
            com2Enum = enumObj;
        }
        
        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType) {
           if (base.CanConvertTo(context, destType)) {
               return true;
           }
           return destType.IsEnum;
       }


        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                 return com2Enum.FromString((string)value);
            }
            return base.ConvertFrom(context, culture, value);
        }
    
        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.ConvertTo"]/*' />
        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException("destinationType");
            }

            if (destinationType == typeof(string)) {
                if (value != null) {
                    string str = com2Enum.ToString(value);
                    return (str == null ? "" : str);
                }
            }

            if (destinationType.IsEnum) {
                return Enum.ToObject(destinationType, value);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
     
        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.GetStandardValues"]/*' />
        /// <devdoc>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
        /// </devdoc>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (values == null) {
                object[] objValues = com2Enum.Values;
                if (objValues != null) {
                    values = new StandardValuesCollection(objValues);
                }
            }
            return values;
        }
    
        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.GetStandardValuesExclusive"]/*' />
        /// <devdoc>
        ///      Determines if the list of standard values returned from
        ///      GetStandardValues is an exclusive list.  If the list
        ///      is exclusive, then no other values are valid, such as
        ///      in an enum data type.  If the list is not exclusive,
        ///      then there are other valid values besides the list of
        ///      standard values GetStandardValues provides.
        /// </devdoc>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return com2Enum.IsStrictEnum;
        }
        
        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.GetStandardValuesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports a standard set of values
        ///      that can be picked from a list.
        /// </devdoc>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
        
        /// <include file='doc\COM2EnumConverter.uex' path='docs/doc[@for="Com2EnumConverter.IsValid"]/*' />
        /// <devdoc>
        ///      Determines if the given object value is valid for this type.
        /// </devdoc>
        public override bool IsValid(ITypeDescriptorContext context, object value) {
            string strValue = com2Enum.ToString(value);
            return strValue != null && strValue.Length > 0;
        }

        public void RefreshValues() {
            this.values = null;
        }
    }
}
