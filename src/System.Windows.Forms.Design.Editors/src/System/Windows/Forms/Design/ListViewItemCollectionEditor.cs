// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides an editor for a ListView items collection.
    /// </summary>
    internal class ListViewItemCollectionEditor : CollectionEditor
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref='System.Windows.Forms.Design.ListViewItemCollectionEditor'/> class.
        /// </summary>
        public ListViewItemCollectionEditor(Type type) : base(type)
        { }

        /// <summary>
        ///  Retrieves the display text for the given list item.
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
    }
}
