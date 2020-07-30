// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ListViewItemConverter is a class that can be used to convert
    ///  ListViewItem objects from one data type to another.  Access this
    ///  class through the TypeDescriptor.
    /// </summary>
    public class ListViewItemConverter : ExpandableObjectConverter
    {
        /// <summary>
        ///  Gets a value indicating whether this converter can convert an object to the given
        ///  destination type using the context.
        /// </summary>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        /// <summary>
        ///  Converts the given object to another type. The most common types to convert
        ///  are to and from a string object. The default implementation will make a call
        ///  to ToString on the object if the object is valid and if the destination
        ///  type is string. If this cannot convert to the desitnation type, this will
        ///  throw a NotSupportedException.
        /// </summary>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem item)
            {
                ConstructorInfo ctor;
                // Should we use the subitem constructor?
                for (int i = 1; i < item.SubItems.Count; ++i)
                {
                    if (item.SubItems[i].CustomStyle)
                    {
                        if (!string.IsNullOrEmpty(item.ImageKey))
                        {
                            ctor = typeof(ListViewItem).GetConstructor(new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(string) });
                            Debug.Assert(ctor != null, "Expected the constructor to exist.");
                            ListViewItem.ListViewSubItem[] subItemArray = new ListViewItem.ListViewSubItem[item.SubItems.Count];
                            ((ICollection)item.SubItems).CopyTo(subItemArray, 0);
                            return new InstanceDescriptor(ctor, new object[] { subItemArray, item.ImageKey }, false);
                        }
                        else
                        {
                            ctor = typeof(ListViewItem).GetConstructor(new Type[] { typeof(ListViewItem.ListViewSubItem[]), typeof(int) });
                            Debug.Assert(ctor != null, "Expected the constructor to exist.");
                            ListViewItem.ListViewSubItem[] subItemArray = new ListViewItem.ListViewSubItem[item.SubItems.Count];
                            ((ICollection)item.SubItems).CopyTo(subItemArray, 0);
                            return new InstanceDescriptor(ctor, new object[] { subItemArray, item.ImageIndex }, false);
                        }
                    }
                }

                // Convert SubItem array to string array
                string[] subItems = new string[item.SubItems.Count];
                for (int i = 0; i < subItems.Length; ++i)
                {
                    subItems[i] = item.SubItems[i].Text;
                }

                // ForeColor, BackColor or ItemFont set
                if (item.SubItems[0].CustomStyle)
                {
                    if (!string.IsNullOrEmpty(item.ImageKey))
                    {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[]
                        {
                            typeof(string[]),
                            typeof(string),
                            typeof(Color),
                            typeof(Color),
                            typeof(Font)
                        });
                        Debug.Assert(ctor != null, "Expected the constructor to exist.");
                        return new InstanceDescriptor(ctor, new object[]
                        {
                            subItems,
                            item.ImageKey,
                            item.SubItems[0].CustomForeColor ? item.ForeColor : Color.Empty,
                            item.SubItems[0].CustomBackColor ? item.BackColor : Color.Empty,
                            item.SubItems[0].CustomFont ? item.Font : null
                        }, false);
                    }
                    else
                    {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[]
                        {
                            typeof(string[]),
                            typeof(int),
                            typeof(Color),
                            typeof(Color),
                            typeof(Font)
                        });
                        Debug.Assert(ctor != null, "Expected the constructor to exist.");
                        return new InstanceDescriptor(ctor, new object[]
                        {
                            subItems,
                            item.ImageIndex,
                            item.SubItems[0].CustomForeColor ? item.ForeColor : Color.Empty,
                            item.SubItems[0].CustomBackColor ? item.BackColor : Color.Empty,
                            item.SubItems[0].CustomFont ? item.Font : null
                        }, false);
                    }
                }

                // Text
                if (item.ImageIndex == -1 && string.IsNullOrEmpty(item.ImageKey) && item.SubItems.Count <= 1)
                {
                    ctor = typeof(ListViewItem).GetConstructor(new Type[] { typeof(string) });
                    Debug.Assert(ctor != null, "Expected the constructor to exist.");
                    return new InstanceDescriptor(ctor, new object[] { item.Text }, false);
                }

                // Text and Image
                if (item.SubItems.Count <= 1)
                {
                    if (!string.IsNullOrEmpty(item.ImageKey))
                    {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[]
                        {
                            typeof(string),
                            typeof(string)
                        });
                        Debug.Assert(ctor != null, "Expected the constructor to exist.");
                        return new InstanceDescriptor(ctor, new object[] { item.Text, item.ImageKey }, false);
                    }
                    else
                    {
                        ctor = typeof(ListViewItem).GetConstructor(new Type[]
                        {
                            typeof(string),
                            typeof(int)
                        });
                        Debug.Assert(ctor != null, "Expected the constructor to exist.");
                        return new InstanceDescriptor(ctor, new object[] { item.Text, item.ImageIndex }, false);
                    }
                }

                // Text, Image and SubItems
                if (!string.IsNullOrEmpty(item.ImageKey))
                {
                    ctor = typeof(ListViewItem).GetConstructor(new Type[]
                    {
                        typeof(string[]),
                        typeof(string)
                    });
                    Debug.Assert(ctor != null, "Expected the constructor to exist.");
                    return new InstanceDescriptor(ctor, new object[] { subItems, item.ImageKey }, false);
                }
                else
                {
                    ctor = typeof(ListViewItem).GetConstructor(new Type[]
                    {
                        typeof(string[]),
                        typeof(int)
                    });
                    return new InstanceDescriptor(ctor, new object[] { subItems, item.ImageIndex }, false);
                }
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    internal class ListViewSubItemConverter : ExpandableObjectConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(InstanceDescriptor))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType is null)
            {
                throw new ArgumentNullException(nameof(destinationType));
            }

            if (destinationType == typeof(InstanceDescriptor) && value is ListViewItem.ListViewSubItem item)
            {
                ConstructorInfo ctor;

                // Subitem has custom style
                if (item.CustomStyle)
                {
                    ctor = typeof(ListViewItem.ListViewSubItem).GetConstructor(new Type[]
                    {
                        typeof(ListViewItem),
                        typeof(string),
                        typeof(Color),
                        typeof(Color),
                        typeof(Font)
                    });
                    Debug.Assert(ctor != null, "Expected the constructor to exist.");
                    return new InstanceDescriptor(ctor, new object[] {
                        null,
                        item.Text,
                        item.ForeColor,
                        item.BackColor,
                        item.Font
                    }, true);
                }

                // Otherwise, just use the text constructor
                ctor = typeof(ListViewItem.ListViewSubItem).GetConstructor(new Type[] { typeof(ListViewItem), typeof(string) });
                Debug.Assert(ctor != null, "Expected the constructor to exist.");
                return new InstanceDescriptor(ctor, new object[] { null, item.Text }, true);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
