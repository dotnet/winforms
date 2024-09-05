// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.ComponentModel.Design;

public partial class ComponentDesigner
{
    /// <summary>
    ///  Collection that holds shadow properties.
    /// </summary>
    protected sealed class ShadowPropertyCollection
    {
        private readonly ComponentDesigner _designer;
        private Dictionary<string, object?>? _properties;
        private Dictionary<string, PropertyDescriptor>? _descriptors;

        internal ShadowPropertyCollection(ComponentDesigner designer) => _designer = designer;

        /// <summary>
        ///  Accesses the given property name. This will throw an exception if the property does not exist on the
        ///  base component.
        /// </summary>
        public object? this[string propertyName]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(propertyName);

                // First, check to see if the name is in the given properties table
                if (_properties is not null && _properties.TryGetValue(propertyName, out object? existing))
                {
                    return existing;
                }

                // Next, check to see if the name is in the descriptors table. If it isn't, we will search the
                // underlying component and add it.
                PropertyDescriptor property = GetShadowedPropertyDescriptor(propertyName) ?? throw new ArgumentException("The requested property does not exist", nameof(propertyName));

                return property.GetValue(_designer.Component);
            }
            set
            {
                _properties ??= [];
                _properties[propertyName] = value;
            }
        }

        /// <summary>
        ///  Returns true if this shadow properties object contains the given property name.
        /// </summary>
        public bool Contains(string propertyName) => _properties is not null && _properties.ContainsKey(propertyName);

        /// <summary>
        ///  Returns the underlying property descriptor for this property on the component
        /// </summary>
        private PropertyDescriptor? GetShadowedPropertyDescriptor(string propertyName)
        {
            _descriptors ??= [];

            if (!_descriptors.TryGetValue(propertyName, out PropertyDescriptor? descriptor))
            {
                descriptor = TypeDescriptor.GetProperties(_designer.Component.GetType())[propertyName];
                if (descriptor is not null)
                {
                    _descriptors.Add(propertyName, descriptor);
                }
            }

            return descriptor;
        }

        /// <summary>
        ///  Returns true if the given property name should be serialized, or false if not.
        ///  This is useful in implementing your own ShouldSerialize* methods on shadowed properties.
        /// </summary>
        internal bool ShouldSerializeValue(string propertyName, object? defaultValue)
        {
            ArgumentNullException.ThrowIfNull(propertyName);

            if (Contains(propertyName))
            {
                return !Equals(this[propertyName], defaultValue);
            }

            PropertyDescriptor shadowedPropertyDescriptor = GetShadowedPropertyDescriptor(propertyName) ?? throw new InvalidOperationException("Failed to retrieve the shadowed PropertyDescriptor");

            return shadowedPropertyDescriptor.ShouldSerializeValue(_designer.Component);
        }
    }
}
