// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;

namespace System.ComponentModel.Design
{
    public partial class ComponentDesigner
    {
        /// <summary>
        ///  Collection that holds shadow properties.
        /// </summary>
        protected sealed class ShadowPropertyCollection
        {
            private readonly ComponentDesigner _designer;
            private Hashtable _properties;
            private Hashtable _descriptors;

            internal ShadowPropertyCollection(ComponentDesigner designer) => _designer = designer;

            /// <summary>
            ///  Accesses the given property name.  This will throw an exception if the property does not exsit on the
            ///  base component.
            /// </summary>
            public object this[string propertyName]
            {
                get
                {
                    if (propertyName is null)
                    {
                        throw new ArgumentNullException(nameof(propertyName));
                    }

                    // First, check to see if the name is in the given properties table
                    if (_properties != null && _properties.ContainsKey(propertyName))
                    {
                        return _properties[propertyName];
                    }

                    // Next, check to see if the name is in the descriptors table.  If it isn't, we will search the
                    //underlying component and add it.
                    PropertyDescriptor property = GetShadowedPropertyDescriptor(propertyName);

                    return property.GetValue(_designer.Component);
                }
                set
                {
                    if (_properties is null)
                    {
                        _properties = new Hashtable();
                    }
                    _properties[propertyName] = value;
                }
            }

            /// <summary>
            ///  Returns true if this shadow properties object contains the given property name.
            /// </summary>
            public bool Contains(string propertyName) => _properties != null && _properties.ContainsKey(propertyName);

            /// <summary>
            ///  Returns the underlying property descriptor for this property on the component
            /// </summary>
            private PropertyDescriptor GetShadowedPropertyDescriptor(string propertyName)
            {
                if (_descriptors is null)
                {
                    _descriptors = new Hashtable();
                }

                PropertyDescriptor property = (PropertyDescriptor)_descriptors[propertyName];
                if (property is null)
                {
                    property = TypeDescriptor.GetProperties(_designer.Component.GetType())[propertyName];
                    //_descriptors[propertyName] = property ?? throw new ArgumentException(SR.GetResourceString(SR.DesignerPropNotFound, propertyName));
                }
                return property;
            }

            /// <summary>
            ///  Returns true if the given property name should be serialized, or false  if not.
            ///  This is useful in implementing your own ShouldSerialize* methods on shadowed properties.
            /// </summary>
            internal bool ShouldSerializeValue(string propertyName, object defaultValue)
            {
                if (propertyName is null)
                {
                    throw new ArgumentNullException(nameof(propertyName));
                }

                if (Contains(propertyName))
                {
                    return !object.Equals(this[propertyName], defaultValue);
                }
                else
                {
                    return GetShadowedPropertyDescriptor(propertyName).ShouldSerializeValue(_designer.Component);
                }
            }
        }
    }
}
