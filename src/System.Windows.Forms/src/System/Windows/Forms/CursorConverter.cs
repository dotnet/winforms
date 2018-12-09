// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using System.Diagnostics;
    using Microsoft.Win32;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Reflection;
    using System.IO;

    /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter"]/*' />
    /// <devdoc>
    ///      CursorConverter is a class that can be used to convert
    ///      colors from one data type to another.  Access this
    ///      class through the TypeDescriptor.
    /// </devdoc>
    public class CursorConverter : TypeConverter {
    
        private StandardValuesCollection values;

        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string) || sourceType == typeof(byte[])) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(byte[])) {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }
        
        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        
            if (value is string) {
                string text = ((string)value).Trim();
                
                PropertyInfo[] props = GetProperties();
                for (int i = 0; i < props.Length; i++) {
                    PropertyInfo prop = props[i];
                    if (string.Equals(prop.Name, text, StringComparison.OrdinalIgnoreCase) ){
                        object[] tempIndex = null;
                        return prop.GetValue(null, tempIndex);
                    }
                }
            }
            
            if (value is byte[]) {
                MemoryStream ms = new MemoryStream((byte[])value);
                return new Cursor(ms);
            }

            return base.ConvertFrom(context, culture, value);
        }

        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.ConvertTo"]/*' />
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

            if (destinationType == typeof(string) && value != null) {
                PropertyInfo[] props = GetProperties();
                int bestMatch = -1;

                for (int i = 0; i < props.Length; i++) {
                    PropertyInfo prop = props[i];
                    object[] tempIndex = null;
                    Cursor c = (Cursor)prop.GetValue(null, tempIndex);
                    if (c == (Cursor)value) {
                        if (Object.ReferenceEquals(c, value)) {
                            return prop.Name;
                        }
                        else {
                            bestMatch = i;
                        }
                    }
                }

                if (bestMatch != -1) {
                    return props[bestMatch].Name;
                }
                
                // We throw here because we cannot meaningfully convert a custom
                // cursor into a string. In fact, the ResXResourceWriter will use
                // this exception to indicate to itself that this object should
                // be serialized through ISeriazable instead of a string.
                //
                throw new FormatException(SR.CursorCannotCovertToString);
            }

            if (destinationType == typeof(InstanceDescriptor) && value is Cursor) {
                PropertyInfo[] props = GetProperties();
                foreach(PropertyInfo prop in props) {
                    if (prop.GetValue(null, null) == value) {
                        return new InstanceDescriptor(prop, null);
                    }
                }
            }
            
            if (destinationType == typeof(byte[])) {
                if (value != null) {
                    MemoryStream ms = new MemoryStream();
                    Cursor cursor = (Cursor)value;
                    cursor.SavePicture(ms);
                    ms.Close();
                    return ms.ToArray();
                }
                else 
                    return new byte[0];
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
        
        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.GetProperties"]/*' />
        /// <devdoc>
        ///      Retrieves the properties for the available cursors.
        /// </devdoc>
        private PropertyInfo[] GetProperties() {
            return typeof(Cursors).GetProperties(BindingFlags.Static | BindingFlags.Public);
        }

        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.GetStandardValues"]/*' />
        /// <devdoc>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
        /// </devdoc>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (values == null) {
                ArrayList list = new ArrayList();
                PropertyInfo[] props = GetProperties();
                for (int i = 0; i < props.Length; i++) {
                    PropertyInfo prop = props[i];
                    object[] tempIndex = null;
                    Debug.Assert(prop.GetValue(null, tempIndex) != null, "Property " + prop.Name + " returned NULL");
                    list.Add(prop.GetValue(null, tempIndex));
                }
                
                values = new StandardValuesCollection(list.ToArray());
            }
            
            return values;
        }

        /// <include file='doc\CursorConverter.uex' path='docs/doc[@for="CursorConverter.GetStandardValuesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports a standard set of values
        ///      that can be picked from a list.
        /// </devdoc>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }
}

