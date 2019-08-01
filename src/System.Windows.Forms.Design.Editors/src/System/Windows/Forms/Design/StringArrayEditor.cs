// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <include file='doc\StringArrayEditor.uex' path='docs/doc[@for="StringArrayEditor"]/*' />
    /// <devdoc>
    ///      The StringArrayEditor is a collection editor that is specifically
    ///      designed to edit arrays containing strings.
    /// </devdoc>
    internal class StringArrayEditor : StringCollectionEditor
    {
        public StringArrayEditor(Type type)
            : base(type)
        {
        }

        /// <include file='doc\StringArrayEditor.uex' path='docs/doc[@for="StringArrayEditor.CreateCollectionItemType"]/*' />
        /// <devdoc>
        ///      Retrieves the data type this collection contains.  The default 
        ///      implementation looks inside of the collection for the Item property
        ///      and returns the returning datatype of the item.  Do not call this
        ///      method directly.  Instead, use the CollectionItemType property.  Use this
        ///      method to override the default implementation.
        /// </devdoc>
        protected override Type CreateCollectionItemType()
        {
            return CollectionType.GetElementType();
        }

        /// <include file='doc\StringArrayEditor.uex' path='docs/doc[@for="StringArrayEditor.GetItems"]/*' />
        /// <devdoc>
        ///      We implement the getting and setting of items on this collection.
        /// </devdoc>
        protected override object[] GetItems(object editValue)
        {
            Array valueArray = editValue as Array;
            if (valueArray == null)
            {
                return new object[0];
            }
            else
            {
                object[] items = new object[valueArray.GetLength(0)];
                Array.Copy(valueArray, items, items.Length);
                return items;
            }
        }

        /// <include file='doc\StringArrayEditor.uex' path='docs/doc[@for="StringArrayEditor.SetItems"]/*' />
        /// <devdoc>
        ///      We implement the getting and setting of items on this collection.
        ///      It should return an instance to replace editValue with, or editValue
        ///      if there is no need to replace the instance.
        /// </devdoc>
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

