// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Globalization;
using static System.TrimmingConstants;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class MergePropertyDescriptor : PropertyDescriptor
{
    private readonly PropertyDescriptor[] _descriptors;

    private bool? _localizable;
    private bool? _readOnly;
    private bool? _canReset;

    private MultiMergeCollection? _collection;

    public MergePropertyDescriptor(PropertyDescriptor[] descriptors)
        : base(descriptors[0].Name, attrs: null)
    {
        _descriptors = descriptors;
    }

    public override Type ComponentType => _descriptors[0].ComponentType;

    public override TypeConverter Converter
    {
        [RequiresUnreferencedCode(PropertyDescriptorPropertyTypeMessage)]
        get => _descriptors[0].Converter;
    }

    public override string DisplayName => _descriptors[0].DisplayName;

    public override bool IsLocalizable
    {
        get
        {
            if (!_localizable.HasValue)
            {
                _localizable = true;
                foreach (PropertyDescriptor pd in _descriptors)
                {
                    if (!pd.IsLocalizable)
                    {
                        _localizable = false;
                        break;
                    }
                }
            }

            return _localizable.Value;
        }
    }

    public override bool IsReadOnly
    {
        get
        {
            if (!_readOnly.HasValue)
            {
                _readOnly = false;
                foreach (PropertyDescriptor pd in _descriptors)
                {
                    if (pd.IsReadOnly)
                    {
                        _readOnly = true;
                        break;
                    }
                }
            }

            return _readOnly.Value;
        }
    }

    public override Type PropertyType => _descriptors[0].PropertyType;

    public PropertyDescriptor this[int index] => _descriptors[index];

    public override bool CanResetValue(object component)
    {
        Debug.Assert(component is Array, "MergePropertyDescriptor::CanResetValue called with non-array value");
        if (!_canReset.HasValue)
        {
            _canReset = true;
            var a = (Array)component;
            for (int i = 0; i < _descriptors.Length; i++)
            {
                if (!_descriptors[i].CanResetValue(GetPropertyOwnerForComponent(a, i)!))
                {
                    _canReset = false;
                    break;
                }
            }
        }

        return _canReset.Value;
    }

    /// <summary>
    ///  This method attempts to copy the given value so unique values are always passed to each object.
    ///  If the value cannot be copied the original value will be returned.
    /// </summary>
    private static object? CopyValue(object? value)
    {
        // Null is always OK.
        if (value is null)
        {
            return value;
        }

        Type type = value.GetType();

        // Value types are always copies.
        if (type.IsValueType)
        {
            return value;
        }

        object? clonedValue = null;

        // ICloneable is the next easiest thing.
        if (value is ICloneable clone)
        {
            clonedValue = clone.Clone();

            if (clonedValue is not null)
            {
                return clonedValue;
            }
        }

        // Next, access the type converter.
        TypeConverter converter = TypeDescriptor.GetConverter(value);
        if (converter.CanConvertTo(typeof(InstanceDescriptor)))
        {
            // Instance descriptors provide full fidelity unless they are marked as incomplete.
            var instanceDescriptor = (InstanceDescriptor?)converter.ConvertTo(
                null,
                CultureInfo.InvariantCulture,
                value,
                typeof(InstanceDescriptor));

            if (instanceDescriptor is not null && instanceDescriptor.IsComplete)
            {
                clonedValue = instanceDescriptor.Invoke();
                if (clonedValue is not null)
                {
                    return clonedValue;
                }
            }
        }

        // If that didn't work, try conversion to/from string.
        if (converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
        {
            string stringRepresentation = converter.ConvertToInvariantString(value)!;
            clonedValue = converter.ConvertFromInvariantString(stringRepresentation);
            if (clonedValue is not null)
            {
                return clonedValue;
            }
        }

        if (clonedValue is not null)
        {
            return clonedValue;
        }

        // We failed. This object's reference will be set on each property.
        return value;
    }

    protected override AttributeCollection CreateAttributeCollection() => new MergedAttributeCollection(this);

    private object? GetPropertyOwnerForComponent(Array a, int i)
    {
        object? propertyOwner = a.GetValue(i);
        if (propertyOwner is ICustomTypeDescriptor descriptor)
        {
            propertyOwner = descriptor.GetPropertyOwner(_descriptors[i]);
        }

        return propertyOwner;
    }

    [RequiresUnreferencedCode($"{EditorRequiresUnreferencedCode} {PropertyDescriptorPropertyTypeMessage}")]
    public override object? GetEditor(Type editorBaseType) => _descriptors[0].GetEditor(editorBaseType);

    public override object? GetValue(object? component)
    {
        Debug.Assert(component is Array, "MergePropertyDescriptor::GetValue called with non-array value");
        return GetValue((Array)component, out _);
    }

    public object? GetValue(Array components, out bool allEqual)
    {
        allEqual = true;
        object? @object = _descriptors[0].GetValue(GetPropertyOwnerForComponent(components, 0));

        if (@object is ICollection collection)
        {
            if (_collection is null)
            {
                _collection = new MultiMergeCollection(collection);
            }
            else if (_collection.Locked)
            {
                return _collection;
            }
            else
            {
                _collection.SetItems(collection);
            }
        }

        for (int i = 1; i < _descriptors.Length; i++)
        {
            object? currentObject = _descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));

            if (_collection is not null)
            {
                if (!_collection.ReinitializeIfNotEqual((ICollection)currentObject!))
                {
                    allEqual = false;
                    return null;
                }
            }
            else if ((@object is null && currentObject is null) || (@object is not null && @object.Equals(currentObject)))
            {
                continue;
            }
            else
            {
                allEqual = false;
                return null;
            }
        }

        if (allEqual && _collection is not null && _collection.Count == 0)
        {
            return null;
        }

        return _collection ?? @object;
    }

    internal object?[] GetValues(Array components)
    {
        object?[] values = new object?[components.Length];

        for (int i = 0; i < components.Length; i++)
        {
            values[i] = _descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));
        }

        return values;
    }

    public override void ResetValue(object component)
    {
        Debug.Assert(component is Array, "MergePropertyDescriptor::ResetValue called with non-array value");
        var array = (Array)component;
        for (int i = 0; i < _descriptors.Length; i++)
        {
            _descriptors[i].ResetValue(GetPropertyOwnerForComponent(array, i)!);
        }
    }

    private void SetCollectionValues(Array a, IList listValue)
    {
        try
        {
            if (_collection is not null)
            {
                _collection.Locked = true;
            }

            // Now we have to copy the value into each property.
            object[] values = new object[listValue.Count];

            listValue.CopyTo(values, 0);

            for (int i = 0; i < _descriptors.Length; i++)
            {
                if (_descriptors[i].GetValue(GetPropertyOwnerForComponent(a, i)) is not IList properties)
                {
                    continue;
                }

                properties.Clear();
                foreach (object value in values)
                {
                    properties.Add(value);
                }
            }
        }
        finally
        {
            if (_collection is not null)
            {
                _collection.Locked = false;
            }
        }
    }

    public override void SetValue(object? component, object? value)
    {
        Debug.Assert(component is Array, "MergePropertyDescriptor::SetValue called with non-array value");
        var array = (Array)component;
        if (value is IList list && typeof(IList).IsAssignableFrom(PropertyType))
        {
            SetCollectionValues(array, list);
        }
        else
        {
            for (int i = 0; i < _descriptors.Length; i++)
            {
                object? clonedValue = CopyValue(value);
                _descriptors[i].SetValue(GetPropertyOwnerForComponent(array, i), clonedValue);
            }
        }
    }

    public override bool ShouldSerializeValue(object component)
    {
        Debug.Assert(component is Array, "MergePropertyDescriptor::ShouldSerializeValue called with non-array value");
        var array = (Array)component;
        for (int i = 0; i < _descriptors.Length; i++)
        {
            if (!_descriptors[i].ShouldSerializeValue(GetPropertyOwnerForComponent(array, i)!))
            {
                return false;
            }
        }

        return true;
    }
}
