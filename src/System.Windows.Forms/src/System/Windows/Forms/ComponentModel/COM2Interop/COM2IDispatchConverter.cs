// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {

    using System.Diagnostics;
    using System;
    
    using System.ComponentModel;
    using System.Collections;
    using Microsoft.Win32;
    using System.Globalization;

    internal class Com2IDispatchConverter : Com2ExtendedTypeConverter{
        Com2PropertyDescriptor propDesc;

        /// <include file='doc\COM2IDispatchConverter.uex' path='docs/doc[@for="Com2IDispatchConverter.none"]/*' />
        /// <devdoc>
        ///     What we return textually for null.
        /// </devdoc>
        protected static readonly string none = SR.toStringNone;

        private bool allowExpand;


        public Com2IDispatchConverter(Com2PropertyDescriptor propDesc, bool allowExpand, TypeConverter baseConverter) : base(baseConverter){
             this.propDesc = propDesc;
             this.allowExpand = allowExpand;
        }


        public Com2IDispatchConverter(Com2PropertyDescriptor propDesc, bool allowExpand) : base(propDesc.PropertyType){
             this.propDesc = propDesc;
             this.allowExpand = allowExpand;
        }
        
        /// <include file='doc\COM2IDispatchConverter.uex' path='docs/doc[@for="Com2IDispatchConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            return false;
        }
        
        /// <include file='doc\COM2IDispatchConverter.uex' path='docs/doc[@for="Com2IDispatchConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object to the given destination
        ///      type.
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            return destinationType == typeof(string);
        }
        
        
        /// <include file='doc\COM2IDispatchConverter.uex' path='docs/doc[@for="Com2IDispatchConverter.ConvertTo"]/*' />
        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == typeof(string)){
               if (value == null){
                  return none;
               }
               
               string text = ComNativeDescriptor.Instance.GetName(value);
   
               if (text == null || text.Length == 0){
                     text = ComNativeDescriptor.Instance.GetClassName(value);
               }
   
               if (text == null){
                  return "(Object)";
               }
               return text;
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {      
            return TypeDescriptor.GetProperties(value, attributes);
        }
        
        /// <include file='doc\COM2IDispatchConverter.uex' path='docs/doc[@for="Com2IDispatchConverter.GetPropertiesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports properties.  By default, this
        ///      is false.
        /// </devdoc>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            return this.allowExpand;
        }
        
        // no dropdown, please!
        //
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return false;
        }
    }
}
