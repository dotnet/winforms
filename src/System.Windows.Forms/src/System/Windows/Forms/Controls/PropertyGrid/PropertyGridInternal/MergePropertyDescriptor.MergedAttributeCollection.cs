// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class MergePropertyDescriptor
{
    private class MergedAttributeCollection : AttributeCollection
    {
        private readonly MergePropertyDescriptor _owner;

        private AttributeCollection[]? _attributeCollections;
        private Dictionary<Type, Attribute>? _foundAttributes;

        public MergedAttributeCollection(MergePropertyDescriptor owner) : base(attributes: null)
        {
            _owner = owner;
        }

        public override Attribute? this[[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type attributeType] => GetCommonAttribute(attributeType);

        private Attribute? GetCommonAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor | DynamicallyAccessedMemberTypes.PublicFields)] Type attributeType)
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

            if (_foundAttributes is not null
                && _foundAttributes.TryGetValue(attributeType, out Attribute? value))
            {
                return value;
            }

            value = _attributeCollections[0][attributeType];

            if (value is null)
            {
                return null;
            }

            for (int i = 1; i < _attributeCollections.Length; i++)
            {
                Attribute? newValue = _attributeCollections[i][attributeType];
                if (!value.Equals(newValue))
                {
                    value = GetDefaultAttribute(attributeType);
                    break;
                }
            }

            _foundAttributes ??= [];
            if (value is not null)
            {
                _foundAttributes[attributeType] = value;
            }

            return value;
        }
    }
}
