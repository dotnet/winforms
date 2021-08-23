// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class MergePropertyDescriptor
    {
        private class MergedAttributeCollection : AttributeCollection
        {
            private readonly MergePropertyDescriptor _owner;

            private AttributeCollection[] _attributeCollections;
            private IDictionary _foundAttributes;

            public MergedAttributeCollection(MergePropertyDescriptor owner) : base((Attribute[])null)
            {
                _owner = owner;
            }

            public override Attribute this[Type attributeType] => GetCommonAttribute(attributeType);

            private Attribute GetCommonAttribute(Type attributeType)
            {
                if (_attributeCollections is null)
                {
                    _attributeCollections = new AttributeCollection[_owner._descriptors.Length];
                    for (int i = 0; i < _owner._descriptors.Length; i++)
                    {
                        _attributeCollections[i] = _owner._descriptors[i].Attributes;
                    }
                }

                if (_attributeCollections.Length == 0)
                {
                    return GetDefaultAttribute(attributeType);
                }

                Attribute value;
                if (_foundAttributes is not null)
                {
                    value = _foundAttributes[attributeType] as Attribute;
                    if (value is not null)
                    {
                        return value;
                    }
                }

                value = _attributeCollections[0][attributeType];

                if (value is null)
                {
                    return null;
                }

                for (int i = 1; i < _attributeCollections.Length; i++)
                {
                    Attribute newValue = _attributeCollections[i][attributeType];
                    if (!value.Equals(newValue))
                    {
                        value = GetDefaultAttribute(attributeType);
                        break;
                    }
                }

                _foundAttributes ??= new Hashtable();
                _foundAttributes[attributeType] = value;
                return value;
            }
        }
    }
}
