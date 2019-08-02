// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  The <c>StringArrayEditor</c> is a collection editor that is specifically
    ///  designed to edit arrays containing strings.
    /// </summary>
    internal class StringArrayEditor : StringCollectionEditor
    {
        public StringArrayEditor(Type type)
            : base(type)
        {
        }

        /// <summary>
        ///  Retrieves the data type this collection contains.
        ///  The default implementation looks inside of the collection for the Item property
        ///  and returns the returning datatype of the item.
        ///  Do not call this method directly. Instead, use the <see cref="CollectionItemType"/> property.
        ///  Use this method to override the default implementation.
        /// </summary>
        protected override Type CreateCollectionItemType() => CollectionType.GetElementType();

        /// <summary>
        ///  We implement the getting and setting of items on this collection.
        /// </summary>
        protected override object[] GetItems(object editValue)
        {
            Array valueArray = editValue as Array;
            if (valueArray == null)
            {
                return Array.Empty<object>();
            }

            object[] items = new object[valueArray.GetLength(0)];
            Array.Copy(valueArray, items, items.Length);
            return items;
        }

        /// <summary>
        ///  We implement the getting and setting of items on this collection.
        ///  It should return an instance to replace <paramref name="editValue"/> with, or
        ///  <paramref name="editValue"/> if there is no need to replace the instance.
        /// </summary>
        protected override object SetItems(object editValue, object[] value)
        {
            if (editValue is Array || editValue == null)
            {
                Array newArray = Array.CreateInstance(CollectionItemType, value.Length);
                Array.Copy(value, newArray, value.Length);
                return newArray;
            }

            return editValue;
        }
    }
}

