// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Drawing.Design;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Describes and represents inherited properties in an inherited class.
    /// </summary>
    internal sealed class InheritedPropertyDescriptor : PropertyDescriptor
    {
        private PropertyDescriptor propertyDescriptor;
        private object _defaultValue;
        private static readonly object s_noDefault = new Object();
        private bool _initShouldSerialize;
        private object _originalValue;

        /// <summary>
        ///  Initializes a new instance of the <see cref='System.ComponentModel.Design.InheritedPropertyDescriptor'/> class.
        /// </summary>
        public InheritedPropertyDescriptor( PropertyDescriptor propertyDescriptor, object component) : base(propertyDescriptor, new Attribute[] { })
        {
            Debug.Assert(!(propertyDescriptor is InheritedPropertyDescriptor), "Recursive inheritance propertyDescriptor " + propertyDescriptor.ToString());
            this.propertyDescriptor = propertyDescriptor;

            InitInheritedDefaultValue(component);

            // Check to see if this property points to a collection of objects that are not IComponents.  We cannot serialize the delta between two collections if they do not contain components, so if we detect this case we will make the property invisible to serialization.
            // We only do this if there are already items in the collection.  Otherwise, it is safe.
            bool readOnlyCollection = false;

            if (typeof(ICollection).IsAssignableFrom(propertyDescriptor.PropertyType) &&
                propertyDescriptor.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
            {
                if (propertyDescriptor.GetValue(component) is ICollection collection && collection.Count > 0)
                {
                    // Trawl Add and AddRange methods looking for the first compatible serializable method.  All we need is the data type.
                    bool addComponentExists = false;
                    bool addNonComponentExists = false;
                    foreach (MethodInfo method in TypeDescriptor.GetReflectionType(collection).GetMethods(BindingFlags.Public | BindingFlags.Instance))
                    {
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters.Length == 1)
                        {
                            string name = method.Name;
                            Type collectionType = null;

                            if (name.Equals("AddRange") && parameters[0].ParameterType.IsArray)
                            {
                                collectionType = parameters[0].ParameterType.GetElementType();
                            }
                            else if (name.Equals("Add"))
                            {
                                collectionType = parameters[0].ParameterType;
                            }

                            if (collectionType != null)
                            {
                                if (!typeof(IComponent).IsAssignableFrom(collectionType))
                                {
                                    addNonComponentExists = true;
                                }
                                else
                                {
                                    // this collection has at least one Add(IComponent) method.
                                    addComponentExists = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (addNonComponentExists && !addComponentExists)
                    {
                        // Must mark this object as read-only if we can add only non-components to it
                        ArrayList attributes = new ArrayList(AttributeArray)
                        {
                            DesignerSerializationVisibilityAttribute.Hidden,
                            ReadOnlyAttribute.Yes,
                            new EditorAttribute(typeof(UITypeEditor), typeof(UITypeEditor)),
                            new TypeConverterAttribute(typeof(ReadOnlyCollectionConverter))
                        };
                        Attribute[] attributeArray = (Attribute[])attributes.ToArray(typeof(Attribute));
                        AttributeArray = attributeArray;
                        readOnlyCollection = true;
                    }
                }
            }

            if (!readOnlyCollection)
            {
                if (_defaultValue != s_noDefault)
                {
                    ArrayList attributes = new ArrayList(AttributeArray)
                    {
                        new DefaultValueAttribute(_defaultValue)
                    };

                    Attribute[] attributeArray = new Attribute[attributes.Count];

                    attributes.CopyTo(attributeArray, 0);
                    AttributeArray = attributeArray;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the type of the component this property descriptor is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return propertyDescriptor.ComponentType;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether this property is read only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                return propertyDescriptor.IsReadOnly || Attributes[typeof(ReadOnlyAttribute)].Equals(ReadOnlyAttribute.Yes);
            }
        }

        internal object OriginalValue
        {
            get
            {
                return _originalValue;
            }
        }

        internal PropertyDescriptor PropertyDescriptor
        {
            get
            {
                return propertyDescriptor;
            }
            set
            {
                Debug.Assert(!(value is InheritedPropertyDescriptor), "Recursive inheritance propertyDescriptor " + propertyDescriptor.ToString());
                propertyDescriptor = value;
            }
        }

        /// <summary>
        ///  Gets or sets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get => propertyDescriptor.PropertyType;
        }

        /// <summary>
        ///  Indicates whether reset will change the value of the component.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            // We always have a default value, because we got it from the component when we were constructed.
            if (_defaultValue == s_noDefault)
            {
                return propertyDescriptor.CanResetValue(component);
            }
            else
            {
                return !object.Equals(GetValue(component), _defaultValue);
            }
        }

        private object ClonedDefaultValue(object value)
        {
            DesignerSerializationVisibilityAttribute dsva = (DesignerSerializationVisibilityAttribute)propertyDescriptor.Attributes[typeof(DesignerSerializationVisibilityAttribute)];
            DesignerSerializationVisibility serializationVisibility;

            // if we have a persist contents guy, we'll need to try to clone the value because otherwise we won't be able to tell when it's been modified.
            if (dsva == null)
            {
                serializationVisibility = DesignerSerializationVisibility.Visible;
            }
            else
            {
                serializationVisibility = dsva.Visibility;
            }

            if (value != null && serializationVisibility == DesignerSerializationVisibility.Content)
            {
                if (value is ICloneable)
                {
                    // if it's clonable, clone it...
                    value = ((ICloneable)value).Clone();
                }
                else
                {
                    // otherwise, we'll just have to always spit it.
                    value = s_noDefault;
                }
            }
            return value;
        }

        /// <summary>
        ///  We need to merge in attributes from the wrapped property descriptor here.
        /// </summary>
        protected override void FillAttributes(IList attributeList)
        {
            base.FillAttributes(attributeList);
            foreach (Attribute attr in propertyDescriptor.Attributes)
            {
                attributeList.Add(attr);
            }
        }

        /// <summary>
        ///  Gets the current value of the property on the component, invoking the getXXX method.
        /// </summary>
        public override object GetValue(object component)
        {
            return propertyDescriptor.GetValue(component);
        }

        private void InitInheritedDefaultValue(object component)
        {
            try
            {
                object currentValue;
                // Don't just get the default value.  Check to see if the propertyDescriptor has indicated ShouldSerialize, and if it hasn't try to use the default value.
                // We need to do this for properties that inherit from their parent.  If we are processing properties on the root component, we always favor the presence of a default value attribute.
                // The root component is always inherited but some values should always be written into code.
                if (!propertyDescriptor.ShouldSerializeValue(component))
                {
                    DefaultValueAttribute defaultAttribute = (DefaultValueAttribute)propertyDescriptor.Attributes[typeof(DefaultValueAttribute)];
                    if (defaultAttribute != null)
                    {
                        _defaultValue = defaultAttribute.Value;
                        currentValue = _defaultValue;
                    }
                    else
                    {
                        _defaultValue = s_noDefault;
                        currentValue = propertyDescriptor.GetValue(component);
                    }
                }
                else
                {
                    _defaultValue = propertyDescriptor.GetValue(component);
                    currentValue = _defaultValue;
                    _defaultValue = ClonedDefaultValue(_defaultValue);
                }
                SaveOriginalValue(currentValue);
            }
            catch
            {
                // If the property get blows chunks, then the default value is NoDefault and we resort to the base property descriptor.
                _defaultValue = s_noDefault;
            }
            _initShouldSerialize = ShouldSerializeValue(component);
        }

        /// <summary>
        ///  Resets the default value for this property on the component.
        /// </summary>
        public override void ResetValue(object component)
        {
            if (_defaultValue == s_noDefault)
            {
                propertyDescriptor.ResetValue(component);
            }
            else
            {
                SetValue(component, _defaultValue);
            }
        }

        private void SaveOriginalValue(object value)
        {
            if (value is ICollection)
            {
                _originalValue = new object[((ICollection)value).Count];
                ((ICollection)value).CopyTo((Array)_originalValue, 0);
            }
            else
            {
                _originalValue = value;
            }
        }

        /// <summary>
        ///  Sets the value to be the new value of this property on the component by invoking the setXXX method on the component.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            propertyDescriptor.SetValue(component, value);
        }

        /// <summary>
        ///  Indicates whether the value of this property needs to be persisted.
        /// </summary>
        public override bool ShouldSerializeValue(object component)
        {
            if (IsReadOnly)
            {
                return propertyDescriptor.ShouldSerializeValue(component) && Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
            }

            if (_defaultValue == s_noDefault)
            {
                return propertyDescriptor.ShouldSerializeValue(component);
            }
            else
            {
                return !object.Equals(GetValue(component), _defaultValue);
            }
        }

        private class ReadOnlyCollectionConverter : TypeConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string))
                {
                    return SR.GetResourceString(SR.InheritanceServiceReadOnlyCollection);
                }
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
