// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Reflection;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Globalization;
    using System.Collections;

    /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea"]/*' />
    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [
        TypeConverterAttribute(typeof(LinkArea.LinkAreaConverter)),
        Serializable
    ]
    public struct LinkArea {
        int start;
        int length;

        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkArea"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public LinkArea(int start, int length) {
            this.start = start;
            this.length = length;
        }

        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.Start"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Start {
            get {
                return start;
            }
            set {
                start = value;
            }
        }

        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.Length"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public int Length {
            get {
                return length;
            }
            set {
                length = value;
            }
        }
        
        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.IsEmpty"]/*' />
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsEmpty {
            get {
                return length == start && start == 0;
            }
        }
        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.Equals"]/*' />
        public override bool Equals(object o) {
            if (!(o is LinkArea)) {
                return false;
            }
            
            LinkArea a = (LinkArea)o;
            return this == a;
        }

        public override string ToString() {
            return "{Start=" + Start.ToString(CultureInfo.CurrentCulture) + ", Length=" + Length.ToString(CultureInfo.CurrentCulture) + "}";
        }

        public static bool operator == (LinkArea linkArea1, LinkArea linkArea2){
            return (linkArea1.start == linkArea2.start) && (linkArea1.length == linkArea2.length);
        }

        public static bool operator != (LinkArea linkArea1, LinkArea linkArea2) {
            return !(linkArea1 == linkArea2);
        }

        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.GetHashCode"]/*' />
        public override int GetHashCode() {
            return start << 4 | length;
        }

        /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter"]/*' />
        /// <devdoc>
        ///      LinkAreaConverter is a class that can be used to convert
        ///      LinkArea from one data type to another.  Access this
        ///      class through the TypeDescriptor.
        /// </devdoc>
        public class LinkAreaConverter : TypeConverter {

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.CanConvertFrom"]/*' />
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

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.CanConvertTo"]/*' />
            /// <devdoc>
            ///    <para>Gets a value indicating whether this converter can
            ///       convert an object to the given destination type using the context.</para>
            /// </devdoc>
            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
                if (destinationType == typeof(InstanceDescriptor)) {
                    return true;
                }
                return base.CanConvertTo(context, destinationType);
            }

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.ConvertFrom"]/*' />
            /// <devdoc>
            ///      Converts the given object to the converter's native type.
            /// </devdoc>
            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {

                if (value is string) {

                    string text = ((string)value).Trim();

                    if (text.Length == 0) {
                        return null;
                    }
                    else {

                        // Parse 2 integer values.
                        //
                        if (culture == null) {
                            culture = CultureInfo.CurrentCulture;
                        }                    
                        char sep = culture.TextInfo.ListSeparator[0];
                        string[] tokens = text.Split(new char[] {sep});
                        int[] values = new int[tokens.Length];
                        TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                        for (int i = 0; i < values.Length; i++) {
                            values[i] = (int)intConverter.ConvertFromString(context, culture, tokens[i]);
                        }

                        if (values.Length == 2) {
                            return new LinkArea(values[0], values[1]);
                        }
                        else {
                            throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                                      text,
                                                                      "start, length"));
                        }
                    }
                }

                return base.ConvertFrom(context, culture, value);
            }

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.ConvertTo"]/*' />
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

                if (destinationType == typeof(string) && value is LinkArea) {
                    LinkArea pt = (LinkArea)value;

                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string sep = culture.TextInfo.ListSeparator + " ";
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    string[] args = new string[2];
                    int nArg = 0;

                    args[nArg++] = intConverter.ConvertToString(context, culture, pt.Start);
                    args[nArg++] = intConverter.ConvertToString(context, culture, pt.Length);

                    return string.Join(sep, args);
                }
                if (destinationType == typeof(InstanceDescriptor) && value is LinkArea) {
                    LinkArea pt = (LinkArea)value;

                    ConstructorInfo ctor = typeof(LinkArea).GetConstructor(new Type[] {typeof(int), typeof(int)});
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[] {pt.Start, pt.Length});
                    }
                }

                return base.ConvertTo(context, culture, value, destinationType);
            }

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.CreateInstance"]/*' />
            /// <devdoc>
            ///      Creates an instance of this type given a set of property values
            ///      for the object.  This is useful for objects that are immutable, but still
            ///      want to provide changable properties.
            /// </devdoc>
            public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues) {
                return new LinkArea((int)propertyValues["Start"],
                                 (int)propertyValues["Length"]);
            }

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.GetCreateInstanceSupported"]/*' />
            /// <devdoc>
            ///      Determines if changing a value on this object should require a call to
            ///      CreateInstance to create a new value.
            /// </devdoc>
            public override bool GetCreateInstanceSupported(ITypeDescriptorContext context) {
                return true;
            }

            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.GetProperties"]/*' />
            /// <devdoc>
            ///      Retrieves the set of properties for this type.  By default, a type has
            ///      does not return any properties.  An easy implementation of this method
            ///      can just call TypeDescriptor.GetProperties for the correct data type.
            /// </devdoc>
            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(LinkArea), attributes);
                return props.Sort(new string[] {"Start", "Length"});
            }


            /// <include file='doc\LinkArea.uex' path='docs/doc[@for="LinkArea.LinkAreaConverter.GetPropertiesSupported"]/*' />
            /// <devdoc>
            ///      Determines if this object supports properties.  By default, this
            ///      is false.
            /// </devdoc>
            public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
                return true;
            }

        }
    }
}
