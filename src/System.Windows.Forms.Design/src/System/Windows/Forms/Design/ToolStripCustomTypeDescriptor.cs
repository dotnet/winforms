// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.Design;

internal class ToolStripCustomTypeDescriptor : CustomTypeDescriptor
{
    private readonly ToolStrip _instance;
    private PropertyDescriptor? _propItems;
    private PropertyDescriptorCollection? _collection;

    public ToolStripCustomTypeDescriptor(ToolStrip instance) : base()
    {
        _instance = instance;
    }

    /// <summary>
    ///  The GetPropertyOwner method returns an instance of an object that
    ///  owns the given property for the object this type descriptor is representing.
    ///  An optional attribute array may be provided to filter the collection that is
    ///  returned.  Returning null from this method causes the TypeDescriptor object
    ///  to use its default type description services.
    /// </summary>
    /// <param name="pd">PropertyDescriptor</param>
    /// <returns></returns>
    public override object GetPropertyOwner(PropertyDescriptor? pd) => _instance;

    /// <summary>
    ///  The GetProperties method returns a collection of property descriptors
    ///  for the object this type descriptor is representing.  An optional
    ///  attribute array may be provided to filter the collection that is returned
    ///  If no parent is provided,this will return an empty
    ///  property collection.
    /// </summary>
    /// <returns></returns>
    public override PropertyDescriptorCollection GetProperties()
    {
        if (_instance is not null && _collection is null)
        {
            PropertyDescriptorCollection retColl = TypeDescriptor.GetProperties(_instance);
            PropertyDescriptor[] propArray = new PropertyDescriptor[retColl.Count];

            retColl.CopyTo(propArray, 0);

            _collection = new PropertyDescriptorCollection(propArray, false);
        }

        if (_collection?.Count > 0)
        {
            _propItems = _collection["Items"];
            if (_propItems is not null)
            {
                _collection.Remove(_propItems);
            }
        }

        return _collection!;
    }

    /// <summary>
    ///  The GetProperties method returns a collection of property descriptors
    ///  for the object this type descriptor is representing.  An optional
    ///  attribute array may be provided to filter the collection that is returned.
    ///  If no parent is provided,this will return an empty
    ///  property collection.
    ///  Here we will pass the "collection without the "items" property.
    /// </summary>
    /// <param name="attributes"></param>
    /// <returns></returns>
    public override PropertyDescriptorCollection GetProperties(Attribute[]? attributes)
    {
        if (_instance is not null && _collection is null)
        {
            PropertyDescriptorCollection retColl = TypeDescriptor.GetProperties(_instance);
            PropertyDescriptor[] propArray = new PropertyDescriptor[retColl.Count];

            retColl.CopyTo(propArray, 0);

            _collection = new PropertyDescriptorCollection(propArray, false);
        }

        if (_collection?.Count > 0)
        {
            _propItems = _collection["Items"];
            if (_propItems is not null)
            {
                _collection.Remove(_propItems);
            }
        }

        return _collection!;
    }
}
