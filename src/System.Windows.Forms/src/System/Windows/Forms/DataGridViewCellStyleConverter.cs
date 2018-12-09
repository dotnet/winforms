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
    using System.Diagnostics;
    using System.Globalization;
    using System.Reflection;

    /// <include file='doc\DataGridViewCellStyleConverter.uex' path='docs/doc[@for="DataGridViewCellStyleConverter"]/*' />
    /// <devdoc>    
    /// </devdoc>
    public class DataGridViewCellStyleConverter : TypeConverter {
    
        /// <include file='doc\DataGridViewCellStyleConverter.uex' path='docs/doc[@for="DataGridViewCellStyleConverter.CanConvertTo"]/*' />
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
        
        /// <include file='doc\DataGridViewCellStyleConverter.uex' path='docs/doc[@for="DataGridViewCellStyleConverter.ConvertTo"]/*' />
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

            if (destinationType == typeof(InstanceDescriptor) && value is DataGridViewCellStyle) {
                ConstructorInfo ctor = value.GetType().GetConstructor(new Type[0]);
                return new InstanceDescriptor(ctor, new object[0], false);
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

