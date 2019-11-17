// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for a ListView subitems collection.
    /// </summary>
    internal class ListViewSubItemCollectionEditor : CollectionEditor
    {
        private static int _count = 0;
        private ListViewItem.ListViewSubItem _firstSubItem = null;

        /// <summary>
        ///  Initializes a new instance of the <see cref='System.Windows.Forms.Design.ListViewSubItemCollectionEditor'/> class.
        /// </summary>
        public ListViewSubItemCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        ///  Creates an instance of the specified type in the collection.
        /// </summary>
        protected override object CreateInstance(Type type)
        {
            object instance = base.CreateInstance(type);

            // slap in a default site-like name
            if (instance is ListViewItem.ListViewSubItem)
            {
                ((ListViewItem.ListViewSubItem)instance).Name = SR.ListViewSubItemBaseName + _count++;
            }

            return instance;
        }

        /// <summary>
        ///  Retrieves the display text for the given list sub item.
        /// </summary>
        protected override string GetDisplayText(object value)
        {
            if (value == null)
            {
                return string.Empty;
            }

            string text;

            PropertyDescriptor prop = TypeDescriptor.GetDefaultProperty(CollectionType);

            if (prop != null && prop.PropertyType == typeof(string))
            {
                text = (string)prop.GetValue(value);

                if (text != null && text.Length > 0)
                {
                    return text;
                }
            }

            text = TypeDescriptor.GetConverter(value).ConvertToString(value);

            if (text == null || text.Length == 0)
            {
                text = value.GetType().Name;
            }

            return text;
        }

        protected override object[] GetItems(object editValue)
        {
            // take the fist sub item out of the collection
            ListViewItem.ListViewSubItemCollection subItemsColl = (ListViewItem.ListViewSubItemCollection)editValue;

            // add all the other sub items
            object[] values = new object[subItemsColl.Count];
            ((ICollection)subItemsColl).CopyTo(values, 0);

            if (values.Length > 0)
            {
                // save the first sub item
                _firstSubItem = subItemsColl[0];

                // now return the rest.
                object[] subValues = new object[values.Length - 1];
                Array.Copy(values, 1, subValues, 0, subValues.Length);
                values = subValues;
            }

            return values;
        }

        protected override object SetItems(object editValue, object[] value)
        {
            IList list = editValue as IList;
            list.Clear();

            list.Add(_firstSubItem);

            for (int i = 0; i < value.Length; i++)
            {
                list.Add(value[i]);
            }

            return editValue;
        }
    }
}
