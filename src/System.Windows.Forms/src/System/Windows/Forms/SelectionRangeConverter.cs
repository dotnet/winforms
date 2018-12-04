// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;

    using Microsoft.Win32;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Drawing;
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter"]/*' />
    /// <devdoc>
    ///      SelectionRangeConverter is a class that can be used to convert
    ///      SelectionRange objects from one data type to another.  Access this
    ///      class through the TypeDescriptor.
    /// </devdoc>
    public class SelectionRangeConverter : TypeConverter {
    
        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string) || sourceType == typeof(DateTime)) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(DateTime)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {
                string text = ((string)value).Trim();
                if (text.Length == 0) {
                    return new SelectionRange(DateTime.Now.Date, DateTime.Now.Date);
                }
                
                // Separate the string into the two dates, and parse each one
                //
                if (culture == null) {
                    culture = CultureInfo.CurrentCulture;
                }                    
                char separator = culture.TextInfo.ListSeparator[0];
                string[] tokens = text.Split(new char[] {separator});
                
                if (tokens.Length == 2) {
                    TypeConverter dateTimeConverter = TypeDescriptor.GetConverter(typeof(DateTime));
                    DateTime start = (DateTime)dateTimeConverter.ConvertFromString(context, culture, tokens[0]);
                    DateTime end   = (DateTime)dateTimeConverter.ConvertFromString(context, culture, tokens[1]);
                    return new SelectionRange(start, end);
                }
                else {
                    throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                              text,
                                                              "Start" + separator + " End"));
                }
            }
            else if (value is DateTime) {
                DateTime dt = (DateTime)value;
                return new SelectionRange(dt, dt);
            }
            
            return base.ConvertFrom(context, culture, value);
        }

        
        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.ConvertTo"]/*' />
        /// <devdoc>
        ///      Converts the given object to another type.  The most common types to convert
        ///      are to and from a string object.  The default implementation will make a call
        ///      to ToString on the object if the object is valid and if the destination
        ///      type is string.  If this cannot convert to the desitnation type, this will
        ///      throw a NotSupportedException.
        /// </devdoc>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            SelectionRange range = value as SelectionRange;
            if (range != null) {
                if (destinationType == typeof(string)) {
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture; 
                    }
                    string sep = culture.TextInfo.ListSeparator + " ";
                    PropertyDescriptorCollection props = GetProperties(value);
                    string[] args = new string[props.Count];
                    
                    for (int i = 0; i < props.Count; i++) {
                        object propValue = props[i].GetValue(value);
                        args[i] = TypeDescriptor.GetConverter(propValue).ConvertToString(context, culture, propValue);
                    }
                    
                    return string.Join(sep, args);
                }
                if (destinationType == typeof(DateTime)) {
                    return range.Start;
                }
                if (destinationType == typeof(InstanceDescriptor)) {
                    ConstructorInfo ctor = typeof(SelectionRange).GetConstructor(new Type[] {
                        typeof(DateTime), typeof(DateTime)});
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[] {range.Start, range.End});
                    }
                }
            }            
            return base.ConvertTo(context, culture, value, destinationType);
        }
        
        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.CreateInstance"]/*' />
        /// <devdoc>
        ///      Creates an instance of this type given a set of property values
        ///      for the object.  This is useful for objects that are immutable, but still
        ///      want to provide changable properties.
        /// </devdoc>
        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
            try
            {
                return new SelectionRange((DateTime)propertyValues["Start"],
                                          (DateTime)propertyValues["End"]);
            }
            catch (InvalidCastException invalidCast)
            {
                throw new ArgumentException(SR.PropertyValueInvalidEntry, invalidCast);
            }
            catch (NullReferenceException nullRef)
            {
                throw new ArgumentException(SR.PropertyValueInvalidEntry, nullRef);
            }
        }

        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.GetCreateInstanceSupported"]/*' />
        /// <devdoc>
        ///      Determines if changing a value on this object should require a call to
        ///      CreateInstance to create a new value.
        /// </devdoc>
        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
            return true;
        }

        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.GetProperties"]/*' />
        /// <devdoc>
        ///      Retrieves the set of properties for this type.  By default, a type has
        ///      does not return any properties.  An easy implementation of this method
        ///      can just call TypeDescriptor.GetProperties for the correct data type.
        /// </devdoc>
        public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(SelectionRange), attributes);
            return props.Sort(new string[] {"Start", "End"});
        }
       
        /// <include file='doc\SelectionRangeConverter.uex' path='docs/doc[@for="SelectionRangeConverter.GetPropertiesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports properties.  By default, this
        ///      is false.
        /// </devdoc>
        public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }
}

