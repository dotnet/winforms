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
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    /// <include file='doc\OpacityConverter.uex' path='docs/doc[@for="OpacityConverter"]/*' />
    /// <devdoc>
    ///      OpacityConverter is a class that can be used to convert
    ///      opacity values from one data type to another.  Access this
    ///      class through the TypeDescriptor.
    /// </devdoc>
    public class OpacityConverter : TypeConverter {
    
        /// <include file='doc\OpacityConverter.uex' path='docs/doc[@for="OpacityConverter.CanConvertFrom"]/*' />
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

        /// <include file='doc\OpacityConverter.uex' path='docs/doc[@for="OpacityConverter.ConvertFrom"]/*' />
        /// <devdoc>
        ///      Converts the given object to the converter's native type.
        /// </devdoc>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
            if (value is string) {

                string text = ((string)value).Replace('%', ' ').Trim();
                double val = Double.Parse(text, CultureInfo.CurrentCulture);
                int indexOfPercent = ((string)value).IndexOf("%");
                if (indexOfPercent > 0 && (val >= 0.0 && val <= 1.0)) {
                    val /= 100.0;
                    text = val.ToString(CultureInfo.CurrentCulture);
                }
                double percent = 1.0;

                try {
                    percent = (double)TypeDescriptor.GetConverter(typeof(double)).ConvertFrom(context, culture, text);
                    
                    // assume they meant a percentage if it is > 1.0, else
                    // they actually typed the correct double...
                    //
                    if (percent > 1.0) {
                        percent /= 100.0;
                    }
                }
                catch (FormatException e) {
                    throw new FormatException(string.Format(SR.InvalidBoundArgument,
                                                                    "Opacity",
                                                                    text,
                                                                    "0%",
                                                                    "100%"), e);
                }
                
                // Now check to see if it is within our bounds.
                //
                if (percent < 0.0 || percent > 1.0) {
                    throw new FormatException(string.Format(SR.InvalidBoundArgument,
                                                                    "Opacity",
                                                                    text,
                                                                    "0%",
                                                                    "100%"));
                }

                return percent;
            }
            
            return base.ConvertFrom(context, culture, value);
        }
        
        /// <include file='doc\OpacityConverter.uex' path='docs/doc[@for="OpacityConverter.ConvertTo"]/*' />
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

            if (destinationType == typeof(string)) {
                double val = (double)value;
                int perc = (int)(val * 100.0);
                return perc.ToString(CultureInfo.CurrentCulture) + "%";
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

