// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.ComponentModel.Com2Interop {

    using System.Diagnostics;

    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Globalization;
    
    using System.Collections;

    /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter"]/*' />
    /// <devdoc>
    /// Base class for value editors that extend basic functionality.
    /// calls will be delegated to the "base value editor".
    /// </devdoc>
    internal class Com2ExtendedTypeConverter : TypeConverter {
         private TypeConverter innerConverter;
         
         public Com2ExtendedTypeConverter(TypeConverter innerConverter) {
            this.innerConverter = innerConverter;
         }
         
         public Com2ExtendedTypeConverter(Type baseType) {
            this.innerConverter = TypeDescriptor.GetConverter(baseType);
         }
         
         public TypeConverter InnerConverter {
            get {
               return innerConverter;
            }
         }
         
         public TypeConverter GetWrappedConverter(Type t) {
            
            TypeConverter converter = innerConverter;
            
            while (converter != null) {
                if (t.IsInstanceOfType(converter)) {
                    return converter;
                }
                
                if (converter is Com2ExtendedTypeConverter) {
                    converter = ((Com2ExtendedTypeConverter)converter).InnerConverter;
                }
                else {
                    break;
                }
            }
            return null;
         }
         
         
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (innerConverter != null) {
               return innerConverter.CanConvertFrom(context, sourceType);
            }
            return base.CanConvertFrom(context, sourceType);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object to the given destination
        ///      type.
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (innerConverter != null) {
               return innerConverter.CanConvertTo(context, destinationType);
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (innerConverter != null) {
               return innerConverter.ConvertFrom(context, culture, value);
            }
            return base.ConvertFrom(context, culture, value);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.ConvertTo"]/*' />
        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (innerConverter != null) {
               return innerConverter.ConvertTo(context, culture, value, destinationType);
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.CreateInstance"]/*' />
        /// <devdoc>
        ///      Creates an instance of this type given a set of property values
        ///      for the object.  This is useful for objects that are immutable, but still
        ///      want to provide changable properties.
        /// </devdoc>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
            if (innerConverter != null) {
               return innerConverter.CreateInstance(context, propertyValues);
            }
            return base.CreateInstance(context, propertyValues);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.GetCreateInstanceSupported"]/*' />
        /// <devdoc>
        ///      Determines if changing a value on this object should require a call to
        ///      CreateInstance to create a new value.
        /// </devdoc>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
            if (innerConverter != null) {
               return innerConverter.GetCreateInstanceSupported(context);
            }
            return base.GetCreateInstanceSupported(context);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.GetProperties"]/*' />
        /// <devdoc>
        ///      Retrieves the set of properties for this type.  By default, a type has
        ///      does not return any properties.  An easy implementation of this method
        ///      can just call TypeDescriptor.GetProperties for the correct data type.
        /// </devdoc>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {      
            if (innerConverter != null) {
               return innerConverter.GetProperties(context, value, attributes);
            }
            return base.GetProperties(context, value, attributes);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.GetPropertiesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports properties.  By default, this
        ///      is false.
        /// </devdoc>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            if (innerConverter != null) {
               return innerConverter.GetPropertiesSupported(context);
            }
            return base.GetPropertiesSupported(context);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.GetStandardValues"]/*' />
        /// <devdoc>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
        /// </devdoc>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (innerConverter != null) {
               return innerConverter.GetStandardValues(context);
            }
            return base.GetStandardValues(context);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.GetStandardValuesExclusive"]/*' />
        /// <devdoc>
        ///      Determines if the list of standard values returned from
        ///      GetStandardValues is an exclusive list.  If the list
        ///      is exclusive, then no other values are valid, such as
        ///      in an enum data type.  If the list is not exclusive,
        ///      then there are other valid values besides the list of
        ///      standard values GetStandardValues provides.
        /// </devdoc>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            if (innerConverter != null) {
               return innerConverter.GetStandardValuesExclusive(context);
            }
            return base.GetStandardValuesExclusive(context);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.GetStandardValuesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports a standard set of values
        ///      that can be picked from a list.
        /// </devdoc>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            if (innerConverter != null) {
               return innerConverter.GetStandardValuesSupported(context);
            }
            return base.GetStandardValuesSupported(context);
        }
        
        /// <include file='doc\COM2ExtendedTypeConverter.uex' path='docs/doc[@for="Com2ExtendedTypeConverter.IsValid"]/*' />
        /// <devdoc>
        ///      Determines if the given object value is valid for this type.
        /// </devdoc>
        public override bool IsValid(ITypeDescriptorContext context, object value) {
            if (innerConverter != null) {
               return innerConverter.IsValid(context, value);
            }
            return base.IsValid(context, value);
        }
    }
}

