// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  ToolStripCustomTypeDescriptor class.
    /// </summary>
    internal class ToolStripCustomTypeDescriptor : CustomTypeDescriptor
    {
        ToolStrip _instance = null;
        PropertyDescriptor _propItems = null;
        PropertyDescriptorCollection _collection = null;

        public ToolStripCustomTypeDescriptor(ToolStrip instance) : base()
        {
            _instance = instance;
        }

        /// <summary>
        /// The GetPropertyOwner method returns an instance of an object that
        /// owns the given property for the object this type descriptor is representing.
        /// An optional attribute array may be provided to filter the collection that is
        /// returned.  Returning null from this method causes the TypeDescriptor object
        /// to use its default type description services.
        /// </summary>
        /// <param name="propertyDescriptord"></param>
        /// <returns>The TiilStrip control.</returns>
        public override object GetPropertyOwner(PropertyDescriptor propertyDescriptord)
        {
            return _instance;
        }

        /// <summary>
        /// The GetProperties method returns a collection of property descriptors 
        /// for the object this type descriptor is representing.  An optional 
        /// attribute array may be provided to filter the collection that is returned.  
        /// If no parent is provided,this will return an empty
        /// property collection.
        /// </summary>
        /// <returns>The properties.</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            if (_instance != null && _collection == null)
            {
                PropertyDescriptorCollection retColl = TypeDescriptor.GetProperties(_instance);
                PropertyDescriptor[] propArray = new PropertyDescriptor[retColl.Count];

                retColl.CopyTo(propArray, 0);

                _collection = new PropertyDescriptorCollection(propArray, false);
            }

            if (_collection.Count > 0)
            {
                _propItems = _collection["Items"];
                if (_propItems != null)
                {
                    _collection.Remove(_propItems);
                }
            }

            return _collection;
        }

        /// <summary>
        /// The GetProperties method returns a collection of property descriptors
        /// for the object this type descriptor is representing.  An optional
        /// attribute array may be provided to filter the collection that is returned.
        /// If no parent is provided,this will return an empty
        /// property collection.
        /// Here we will pass the "collection without the "items" property.
        /// </summary>
        /// <param name="attributes">The attributes collection.</param>
        /// <returns>The properties.</returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (_instance != null && _collection == null)
            {
                PropertyDescriptorCollection retColl = TypeDescriptor.GetProperties(_instance);
                PropertyDescriptor[] propArray = new PropertyDescriptor[retColl.Count];

                retColl.CopyTo(propArray, 0);

                _collection = new PropertyDescriptorCollection(propArray, false);
            }

            if (_collection.Count > 0)
            {
                _propItems = _collection["Items"];
                if (_propItems != null)
                {
                    _collection.Remove(_propItems);
                }
            }

            return _collection;
        }
    }
}
