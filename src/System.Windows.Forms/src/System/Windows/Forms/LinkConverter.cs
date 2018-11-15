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

    /// <include file='doc\LinkConverter.uex' path='docs/doc[@for="LinkConverter"]/*' />
    /// <devdoc>
    ///      <para>A TypeConverter for LinkLabel.Link objects. Access this
    ///      class through the TypeDescriptor. </para>
    /// </devdoc>
    public class LinkConverter : TypeConverter {

        /// <include file='doc\LinkConverter.uex' path='docs/doc[@for="LinkConverter.CanConvertFrom"]/*' />
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
    
        /// <include file='doc\LinkConverter.uex' path='docs/doc[@for="LinkConverter.CanConvertTo"]/*' />
        /// <devdoc>
        ///    <para>Gets a value indicating whether this converter can
        ///       convert an object to the given destination type using the context.</para>
        /// </devdoc>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor) || destinationType == typeof(string)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        /// <include file='doc\LinkConverter.uex' path='docs/doc[@for="LinkConverter.ConvertFrom"]/*' />
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
                
                    // Parse 2 integer values - Start & Length of the Link.
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
                        return new LinkLabel.Link(values[0], values[1]);
                    }
                    else {
                        throw new ArgumentException(string.Format(SR.TextParseFailedFormat,
                                                                  text,
                                                                  "Start, Length"));
                    }
                }
            }
            
            return base.ConvertFrom(context, culture, value);
        }
        
        /// <include file='doc\LinkConverter.uex' path='docs/doc[@for="LinkConverter.ConvertTo"]/*' />
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

            if (value is LinkLabel.Link) {
                if (destinationType == typeof(string)) {
                    LinkLabel.Link link = (LinkLabel.Link)value;
                    
                    if (culture == null) {
                        culture = CultureInfo.CurrentCulture;
                    }
                    string sep = culture.TextInfo.ListSeparator + " ";
                    TypeConverter intConverter = TypeDescriptor.GetConverter(typeof(int));
                    string[] args = new string[2];
                    int nArg = 0;
                    
                    args[nArg++] = intConverter.ConvertToString(context, culture, link.Start);
                    args[nArg++] = intConverter.ConvertToString(context, culture, link.Length);
                    
                    return string.Join(sep, args);
                }
    
                if (destinationType == typeof(InstanceDescriptor)) {
                    LinkLabel.Link link = (LinkLabel.Link)value;
                    MemberInfo info;
                    if (link.LinkData == null) {
                        info = typeof(LinkLabel.Link).GetConstructor(new Type[] {typeof(int), typeof(int)});
                        if (info != null) {
                            return new InstanceDescriptor(info, new object[] {link.Start, link.Length}, true);
                        }
                    }
                    else {
                        info = typeof(LinkLabel.Link).GetConstructor(new Type[] {typeof(int), typeof(int), typeof(object)});
                        if (info != null) {
                            return new InstanceDescriptor(info, new object[] {link.Start, link.Length, link.LinkData}, true);
                        }
                    }
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

