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

    /// <include file='doc\ListItemConverter.uex' path='docs/doc[@for="ListViewGroupConverter"]/*' />
    /// <devdoc>
    ///      ListViewGroupConverter is a class that can be used to convert
    ///      ListViewGroup objects from one data type to another.  Access this
    ///      class through the TypeDescriptor.
    /// </devdoc>
    internal class ListViewGroupConverter : TypeConverter {
    
        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="CursorConverter.CanConvertFrom"]/*' />
        /// <devdoc>
        ///      Determines if this converter can convert an object in the given source
        ///      type to the native type of the converter.
        /// </devdoc>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
            if (sourceType == typeof(string) && context != null && context.Instance is ListViewItem) {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="ListViewGroupConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor)) {
                return true;
            }
            if (destinationType == typeof(string) && context != null && context.Instance is ListViewItem) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="ListViewGroupConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
        
            if (value is string) {
                string text = ((string)value).Trim();

                if (context != null && context.Instance != null) {
                    ListViewItem item = context.Instance as ListViewItem;
                    if (item != null && item.ListView != null) {
                        
                        foreach(ListViewGroup group in item.ListView.Groups) {
                            if (group.Header == text) {
                                return group;
                            }
                        }
                    }
                }
            }                        

            if (value == null || value.Equals(SR.toStringNone)) {
                return null;
            }

            return base.ConvertFrom(context, culture, value);
        }
        
        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="ListViewGroupConverter.ConvertTo"]/*' />
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

            if (destinationType == typeof(InstanceDescriptor) && value is ListViewGroup) {
                ListViewGroup group = (ListViewGroup)value;
                ConstructorInfo ctor;
                
                // Header
                //
                ctor = typeof(ListViewGroup).GetConstructor(new Type[] {typeof(string), typeof(HorizontalAlignment)});
                if (ctor != null) {
                    return new InstanceDescriptor(ctor, new object[] { group.Header, group.HeaderAlignment }, false);
                }
            }

            if (destinationType == typeof(string) && value == null) {
                return SR.toStringNone;
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }

        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="ListViewGroupConverter.GetStandardValues"]/*' />
        /// <devdoc>
        ///      Retrieves a collection containing a set of standard values
        ///      for the data type this validator is designed for.  This
        ///      will return null if the data type does not support a
        ///      standard set of values.
        /// </devdoc>
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
            if (context != null && context.Instance != null) {
                ListViewItem item = context.Instance as ListViewItem;
                if (item != null && item.ListView != null) {
                    ArrayList list = new ArrayList();
                    foreach (ListViewGroup group in item.ListView.Groups) {
                        list.Add(group);
                    }
                    list.Add(null);
                    return new StandardValuesCollection(list);
                }
            }
            return null;
        }

        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="ListViewGroupConverter.GetStandardValuesExclusive"]/*' />
        /// <devdoc>
        ///      Determines if the list of standard values returned from
        ///      GetStandardValues is an exclusive list.  If the list
        ///      is exclusive, then no other values are valid, such as
        ///      in an enum data type.  If the list is not exclusive,
        ///      then there are other valid values besides the list of
        ///      standard values GetStandardValues provides.
        /// </devdoc>
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
            return true;
        }

        /// <include file='doc\ListViewGroupConverter.uex' path='docs/doc[@for="ListViewGroupConverter.GetStandardValuesSupported"]/*' />
        /// <devdoc>
        ///      Determines if this object supports a standard set of values
        ///      that can be picked from a list.
        /// </devdoc>
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
            return true;
        }
    }    
}

