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
            private readonly MergePropertyDescriptor owner;

            private AttributeCollection[] attributeCollections;
            private IDictionary foundAttributes;

            public MergedAttributeCollection(MergePropertyDescriptor owner) : base((Attribute[])null)
            {
                this.owner = owner;
            }

            public override Attribute this[Type attributeType]
            {
                get
                {
                    return GetCommonAttribute(attributeType);
                }
            }

            private Attribute GetCommonAttribute(Type attributeType)
            {
                if (attributeCollections is null)
                {
                    attributeCollections = new AttributeCollection[owner.descriptors.Length];
                    for (int i = 0; i < owner.descriptors.Length; i++)
                    {
                        attributeCollections[i] = owner.descriptors[i].Attributes;
                    }
                }

                if (attributeCollections.Length == 0)
                {
                    return GetDefaultAttribute(attributeType);
                }

                Attribute value;
                if (foundAttributes is not null)
                {
                    value = foundAttributes[attributeType] as Attribute;
                    if (value is not null)
                    {
                        return value;
                    }
                }

                value = attributeCollections[0][attributeType];

                if (value is null)
                {
                    return null;
                }

                for (int i = 1; i < attributeCollections.Length; i++)
                {
                    Attribute newValue = attributeCollections[i][attributeType];
                    if (!value.Equals(newValue))
                    {
                        value = GetDefaultAttribute(attributeType);
                        break;
                    }
                }

                if (foundAttributes is null)
                {
                    foundAttributes = new Hashtable();
                }

                foundAttributes[attributeType] = value;
                return value;
            }
        }
    }
}
