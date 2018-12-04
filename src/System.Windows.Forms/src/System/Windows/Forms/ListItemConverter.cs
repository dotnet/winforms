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

    /// <include file='doc\ListItemConverter.uex' path='docs/doc[@for="ListViewItemConverter"]/*' />
    /// <devdoc>
    ///      ListViewItemConverter is a class that can be used to convert
    ///      ListViewItem objects from one data type to another.  Access this
    ///      class through the TypeDescriptor.
    /// </devdoc>
    public class ListViewItemConverter : ExpandableObjectConverter {
    
        /// <include file='doc\ListItemConverter.uex' path='docs/doc[@for="ListViewItemConverter.CanConvertTo"]/*' />
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
        
        /// <include file='doc\ListItemConverter.uex' path='docs/doc[@for="ListViewItemConverter.ConvertTo"]/*' />
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

            if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem) {
                ListViewItem item = (ListViewItem)value;
                ConstructorInfo ctor;
                
                // Should we use the subitem constructor?
                //
                for(int i=1; i < item.SubItems.Count; ++i) {
                    if (item.SubItems[i].CustomStyle) {
                        if (!String.IsNullOrEmpty(item.ImageKey)) {
                            ctor = typeof(ListViewItem).GetConstructor(new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(string)});
                            if (ctor != null) {
                                ListViewItem.ListViewSubItem[] subItemArray = new ListViewItem.ListViewSubItem[item.SubItems.Count];
                                ((ICollection)item.SubItems).CopyTo(subItemArray, 0);
                                return new InstanceDescriptor(ctor, new object[] {subItemArray, item.ImageKey}, false);
                            }       
                            else {
                                break;
                            }
                        } else {
                            ctor = typeof(ListViewItem).GetConstructor(new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(int)});
                            if (ctor != null) {
                                ListViewItem.ListViewSubItem[] subItemArray = new ListViewItem.ListViewSubItem[item.SubItems.Count];
                                ((ICollection)item.SubItems).CopyTo(subItemArray, 0);
                                return new InstanceDescriptor(ctor, new object[] {subItemArray, item.ImageIndex}, false);
                            }       
                            else {
                                break;
                            }
                        }
                    }
                }                
                
                // Convert SubItem array to string array
                //
                string[] subItems = new string[item.SubItems.Count];
                for(int i=0; i < subItems.Length; ++i) {
                    subItems[i] = item.SubItems[i].Text;
                }
                
                // ForeColor, BackColor or ItemFont set
                //
                if (item.SubItems[0].CustomStyle) {
                    if (!String.IsNullOrEmpty(item.ImageKey)) {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[] {
                            typeof(string[]),
                            typeof(string),
                            typeof(Color),
                            typeof(Color),
                            typeof(Font)});
                        if (ctor != null) {
                            return new InstanceDescriptor(ctor, new object[] {
                                subItems,
                                item.ImageKey,
                                item.SubItems[0].CustomForeColor ? item.ForeColor : Color.Empty,
                                item.SubItems[0].CustomBackColor ? item.BackColor : Color.Empty,
                                item.SubItems[0].CustomFont ? item.Font : null
                                }, false);
                        }
                    } else {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[] {
                            typeof(string[]),
                            typeof(int),
                            typeof(Color),
                            typeof(Color),
                            typeof(Font)});
                        if (ctor != null) {
                            return new InstanceDescriptor(ctor, new object[] {
                                subItems,
                                item.ImageIndex,
                                item.SubItems[0].CustomForeColor ? item.ForeColor : Color.Empty,
                                item.SubItems[0].CustomBackColor ? item.BackColor : Color.Empty,
                                item.SubItems[0].CustomFont ? item.Font : null
                                }, false);
                        }
                    }
                }

                // Text
                //
                if (item.ImageIndex == -1 && String.IsNullOrEmpty(item.ImageKey) && item.SubItems.Count <= 1) {
                    ctor = typeof(ListViewItem).GetConstructor(new Type[] {typeof(string)});
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[] {item.Text}, false);
                    }
                }
                
                // Text and Image
                //
                if (item.SubItems.Count <= 1) {
                    if (!String.IsNullOrEmpty(item.ImageKey)) {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[] {
                            typeof(string),
                            typeof(string)});
                        if (ctor != null) {
                            return new InstanceDescriptor(ctor, new object[] {item.Text, item.ImageKey}, false);
                        }
                    } else {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[] {
                            typeof(string),
                            typeof(int)});
                        if (ctor != null) {
                            return new InstanceDescriptor(ctor, new object[] {item.Text, item.ImageIndex}, false);
                        }
                    }
                }

                // Text, Image and SubItems
                //
                if (!String.IsNullOrEmpty(item.ImageKey)) {
                    ctor = typeof(ListViewItem).GetConstructor(new Type[] {
                        typeof(string[]),
                        typeof(string)});
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[] {subItems, item.ImageKey}, false);
                    }
                } else {
                    ctor = typeof(ListViewItem).GetConstructor(new Type[] {
                        typeof(string[]),
                        typeof(int)});
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[] {subItems, item.ImageIndex}, false);
                    }
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }    

    internal class ListViewSubItemConverter : ExpandableObjectConverter {
    
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
            if (destinationType == typeof(InstanceDescriptor)) {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }
        
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
            if (destinationType == null) {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem.ListViewSubItem) {
                ListViewItem.ListViewSubItem item = (ListViewItem.ListViewSubItem)value;
                ConstructorInfo ctor;
                
                // Subitem has custom style
                //
                if (item.CustomStyle) {
                    ctor = typeof(ListViewItem.ListViewSubItem).GetConstructor(new Type[] {
                        typeof(ListViewItem),
                        typeof(string),
                        typeof(Color),
                        typeof(Color),
                        typeof(Font)});
                    if (ctor != null) {
                        return new InstanceDescriptor(ctor, new object[] {
                            null,
                            item.Text,
                            item.ForeColor,
                            item.BackColor,
                            item.Font}, true);
                    }
                }

                // Otherwise, just use the text constructor
                //
                ctor = typeof(ListViewItem.ListViewSubItem).GetConstructor(new Type[] {typeof(ListViewItem), typeof(string)});
                if (ctor != null) {
                    return new InstanceDescriptor(ctor, new object[] {null, item.Text}, true);
                }
            }
            
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}

