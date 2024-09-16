// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Design;

/// <summary>
///  Describes and represents inherited properties in an inherited class.
/// </summary>
internal sealed class InheritedPropertyDescriptor : PropertyDescriptor
{
    private PropertyDescriptor _propertyDescriptor;
    private object? _defaultValue;
    private static readonly object s_noDefault = new();

    /// <summary>
    ///  Initializes a new instance of the <see cref="InheritedPropertyDescriptor"/> class.
    /// </summary>
    public InheritedPropertyDescriptor(PropertyDescriptor propertyDescriptor, object component) : base(propertyDescriptor, [])
    {
        Debug.Assert(propertyDescriptor is not InheritedPropertyDescriptor, $"Recursive inheritance propertyDescriptor {propertyDescriptor}");
        _propertyDescriptor = propertyDescriptor;

        InitInheritedDefaultValue(component);

        // Check to see if this property points to a collection of objects that are not IComponents.
        // We cannot serialize the delta between two collections if they do not contain components,
        // so if we detect this case we will make the property invisible to serialization.
        // We only do this if there are already items in the collection. Otherwise, it is safe.
        bool readOnlyCollection = false;

        if (typeof(ICollection).IsAssignableFrom(propertyDescriptor.PropertyType) &&
            propertyDescriptor.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
        {
            if (propertyDescriptor.GetValue(component) is ICollection { Count: > 0 } collection)
            {
                // Trawl Add and AddRange methods looking for the first compatible serializable method.
                // All we need is the data type.
                bool addComponentExists = false;
                bool addNonComponentExists = false;
                foreach (MethodInfo method in TypeDescriptor.GetReflectionType(collection).GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    ParameterInfo[] parameters = method.GetParameters();
                    if (parameters is [{ } parameter])
                    {
                        string name = method.Name;
                        Type? collectionType = null;

                        if (name.Equals("AddRange") && parameter.ParameterType.IsArray)
                        {
                            collectionType = parameter.ParameterType.GetElementType();
                        }
                        else if (name.Equals("Add"))
                        {
                            collectionType = parameter.ParameterType;
                        }

                        if (collectionType is not null)
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
                    List<Attribute> attributes = new(AttributeArray!)
                    {
                        DesignerSerializationVisibilityAttribute.Hidden,
                        ReadOnlyAttribute.Yes,
                        new EditorAttribute(typeof(UITypeEditor), typeof(UITypeEditor)),
                        new TypeConverterAttribute(typeof(ReadOnlyCollectionConverter))
                    };

                    AttributeArray = [.. attributes];
                    readOnlyCollection = true;
                }
            }
        }

        if (!readOnlyCollection)
        {
            if (_defaultValue != s_noDefault)
            {
                List<Attribute> attributes = new(AttributeArray!)
                {
                    new DefaultValueAttribute(_defaultValue)
                };

                AttributeArray = [.. attributes];
            }
        }
    }

    /// <summary>
    ///  Gets the type of the component this property descriptor is bound to.
    /// </summary>
    public override Type ComponentType => _propertyDescriptor.ComponentType;

    /// <summary>
    ///  Gets a value indicating whether this property is read only.
    /// </summary>
    public override bool IsReadOnly => _propertyDescriptor.IsReadOnly || Equals(Attributes[typeof(ReadOnlyAttribute)], ReadOnlyAttribute.Yes);

    internal object? OriginalValue { get; private set; }

    internal PropertyDescriptor PropertyDescriptor
    {
        get => _propertyDescriptor;
        set
        {
            Debug.Assert(value is not InheritedPropertyDescriptor, $"Recursive inheritance propertyDescriptor {_propertyDescriptor}");
            _propertyDescriptor = value;
        }
    }

    /// <summary>
    ///  Gets the type of the property.
    /// </summary>
    public override Type PropertyType => _propertyDescriptor.PropertyType;

    /// <summary>
    ///  Indicates whether reset will change the value of the component.
    /// </summary>
    public override bool CanResetValue(object component)
    {
        // We always have a default value, because we got it from the component when we were constructed.
        if (_defaultValue == s_noDefault)
        {
            return _propertyDescriptor.CanResetValue(component);
        }
        else
        {
            return !Equals(GetValue(component), _defaultValue);
        }
    }

    [return: NotNullIfNotNull(nameof(value))]
    private object? ClonedDefaultValue(object? value)
    {
        // if we have a persist contents guy, we'll need to try to clone the value because otherwise
        // we won't be able to tell when it's been modified.
        if (value is null ||
            !_propertyDescriptor.TryGetAttribute(out DesignerSerializationVisibilityAttribute? dsva) ||
            dsva.Visibility != DesignerSerializationVisibility.Content)
        {
            return value;
        }

        if (value is ICloneable cloneable)
        {
            // if it's cloneable, clone it...
            return cloneable.Clone();
        }

        // otherwise, we'll just have to always spit it.
        return s_noDefault;
    }

    /// <summary>
    ///  We need to merge in attributes from the wrapped property descriptor here.
    /// </summary>
    protected override void FillAttributes(IList attributeList)
    {
        base.FillAttributes(attributeList);
        foreach (Attribute attr in _propertyDescriptor.Attributes)
        {
            attributeList.Add(attr);
        }
    }

    /// <summary>
    ///  Gets the current value of the property on the component, invoking the getXXX method.
    /// </summary>
    public override object? GetValue(object? component)
    {
        return _propertyDescriptor.GetValue(component);
    }

    private void InitInheritedDefaultValue(object component)
    {
        try
        {
            object? currentValue;
            // Don't just get the default value. Check to see if the propertyDescriptor has indicated ShouldSerialize,
            // and if it hasn't try to use the default value.
            // We need to do this for properties that inherit from their parent.
            // If we are processing properties on the root component,
            // we always favor the presence of a default value attribute.
            // The root component is always inherited but some values should always be written into code.
            if (!_propertyDescriptor.ShouldSerializeValue(component))
            {
                if (_propertyDescriptor.TryGetAttribute(out DefaultValueAttribute? defaultAttribute))
                {
                    _defaultValue = defaultAttribute.Value;
                    currentValue = _defaultValue;
                }
                else
                {
                    _defaultValue = s_noDefault;
                    currentValue = _propertyDescriptor.GetValue(component);
                }
            }
            else
            {
                _defaultValue = _propertyDescriptor.GetValue(component);
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

        ShouldSerializeValue(component);
    }

    /// <summary>
    ///  Resets the default value for this property on the component.
    /// </summary>
    public override void ResetValue(object component)
    {
        if (_defaultValue == s_noDefault)
        {
            _propertyDescriptor.ResetValue(component);
        }
        else
        {
            SetValue(component, _defaultValue);
        }
    }

    private void SaveOriginalValue(object? value)
    {
        if (value is ICollection collection)
        {
            OriginalValue = new object[collection.Count];
            collection.CopyTo((Array)OriginalValue, 0);
        }
        else
        {
            OriginalValue = value;
        }
    }

    /// <summary>
    ///  Sets the value to be the new value of this property on the component by invoking the setXXX method on the component.
    /// </summary>
    public override void SetValue(object? component, object? value)
    {
        _propertyDescriptor.SetValue(component, value);
    }

    /// <summary>
    ///  Indicates whether the value of this property needs to be persisted.
    /// </summary>
    public override bool ShouldSerializeValue(object component)
    {
        if (IsReadOnly)
        {
            return _propertyDescriptor.ShouldSerializeValue(component) && Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);
        }

        if (_defaultValue == s_noDefault)
        {
            return _propertyDescriptor.ShouldSerializeValue(component);
        }
        else
        {
            return !Equals(GetValue(component), _defaultValue);
        }
    }

    private class ReadOnlyCollectionConverter : TypeConverter
    {
        public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return SR.GetResourceString(SR.InheritanceServiceReadOnlyCollection);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
