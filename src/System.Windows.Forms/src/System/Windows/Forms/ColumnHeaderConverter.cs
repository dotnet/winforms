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

    /// <include file='doc\ColumnHeaderConverter.uex' path='docs/doc[@for="ColumnHeaderConverter"]/*' />
    /// <devdoc>
    /// </devdoc>
    public class ColumnHeaderConverter : ExpandableObjectConverter {
    
        /// <include file='doc\ColumnHeaderConverter.uex' path='docs/doc[@for="ColumnHeaderConverter.CanConvertTo"]/*' />
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
        
        /// <include file='doc\ColumnHeaderConverter.uex' path='docs/doc[@for="ColumnHeaderConverter.ConvertTo"]/*' />
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

            if (destinationType == typeof(InstanceDescriptor) && value is ColumnHeader) {
                ColumnHeader col = (ColumnHeader) value;
                ConstructorInfo ctor;

                Type t = TypeDescriptor.GetReflectionType(value);
                InstanceDescriptor id = null;
                
                if (col.ImageIndex != -1) {
                    ctor = t.GetConstructor(new Type[]{typeof(int)});
                    if (ctor != null) {
                        id = new InstanceDescriptor(ctor, new object[]{col.ImageIndex}, false);
                    }
                    
                } 

                if (id == null && !String.IsNullOrEmpty(col.ImageKey)) {
                    ctor = t.GetConstructor(new Type[]{typeof(string)});
                    if (ctor != null) {
                        id = new InstanceDescriptor(ctor, new object[]{col.ImageKey}, false);
                    }
                } 

                if (id == null) {
                    ctor = t.GetConstructor(new Type[0]);
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[0], false);
                    }
                    else {
                        throw new ArgumentException(string.Format(SR.NoDefaultConstructor, t.FullName));
                    }
                }
                return id;                
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }    
}

