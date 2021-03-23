// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class MergePropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor[] _descriptors;

        private enum TriState
        {
            Unknown,
            Yes,
            No
        }

        private TriState _localizable = TriState.Unknown;
        private TriState _readOnly = TriState.Unknown;
        private TriState _canReset = TriState.Unknown;

        private MultiMergeCollection _collection;

        public MergePropertyDescriptor(PropertyDescriptor[] descriptors) : base(descriptors[0].Name, null)
        {
            _descriptors = descriptors;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the type of the
        ///  component this property
        ///  is bound to.
        /// </summary>
        public override Type ComponentType
        {
            get
            {
                return _descriptors[0].ComponentType;
            }
        }

        /// <summary>
        ///  Gets the type converter for this property.
        /// </summary>
        public override TypeConverter Converter
        {
            get
            {
                return _descriptors[0].Converter;
            }
        }

        public override string DisplayName
        {
            get
            {
                return _descriptors[0].DisplayName;
            }
        }

        /// <summary>
        ///  Gets a value
        ///  indicating whether this property should be localized, as
        ///  specified in the <see cref='LocalizableAttribute'/>.
        /// </summary>
        public override bool IsLocalizable
        {
            get
            {
                if (_localizable == TriState.Unknown)
                {
                    _localizable = TriState.Yes;
                    foreach (PropertyDescriptor pd in _descriptors)
                    {
                        if (!pd.IsLocalizable)
                        {
                            _localizable = TriState.No;
                            break;
                        }
                    }
                }

                return (_localizable == TriState.Yes);
            }
        }

        /// <summary>
        ///  When overridden in
        ///  a derived class, gets a value
        ///  indicating whether this property is read-only.
        /// </summary>
        public override bool IsReadOnly
        {
            get
            {
                if (_readOnly == TriState.Unknown)
                {
                    _readOnly = TriState.No;
                    foreach (PropertyDescriptor pd in _descriptors)
                    {
                        if (pd.IsReadOnly)
                        {
                            _readOnly = TriState.Yes;
                            break;
                        }
                    }
                }

                return (_readOnly == TriState.Yes);
            }
        }

        /// <summary>
        ///  When overridden in a derived class,
        ///  gets the type of the property.
        /// </summary>
        public override Type PropertyType
        {
            get
            {
                return _descriptors[0].PropertyType;
            }
        }

        public PropertyDescriptor this[int index]
        {
            get
            {
                return _descriptors[index];
            }
        }

        /// <summary>
        ///  When overridden in a derived class, indicates whether
        ///  resetting the <paramref name="component"/> will change the value of the
        ///  <paramref name="component"/>.
        /// </summary>
        public override bool CanResetValue(object component)
        {
            Debug.Assert(component is Array, "MergePropertyDescriptor::CanResetValue called with non-array value");
            if (_canReset == TriState.Unknown)
            {
                _canReset = TriState.Yes;
                Array a = (Array)component;
                for (int i = 0; i < _descriptors.Length; i++)
                {
                    if (!_descriptors[i].CanResetValue(GetPropertyOwnerForComponent(a, i)))
                    {
                        _canReset = TriState.No;
                        break;
                    }
                }
            }

            return (_canReset == TriState.Yes);
        }

        /// <summary>
        ///  This method attempts to copy the given value so unique values are
        ///  always passed to each object.  If the object cannot be copied it
        ///  will be returned.
        /// </summary>
        private object CopyValue(object value)
        {
            // null is always OK
            if (value is null)
            {
                return value;
            }

            Type type = value.GetType();

            // value types are always copies
            if (type.IsValueType)
            {
                return value;
            }

            object clonedValue = null;

            // ICloneable is the next easiest thing
            if (value is ICloneable clone)
            {
                clonedValue = clone.Clone();
            }

            // Next, access the type converter
            if (clonedValue is null)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(value);
                if (converter.CanConvertTo(typeof(InstanceDescriptor)))
                {
                    // Instance descriptors provide full fidelity unless
                    // they are marked as incomplete.
                    InstanceDescriptor desc = (InstanceDescriptor)converter.ConvertTo(null, CultureInfo.InvariantCulture, value, typeof(InstanceDescriptor));
                    if (desc is not null && desc.IsComplete)
                    {
                        clonedValue = desc.Invoke();
                    }
                }

                // If that didn't work, try conversion to/from string
                if (clonedValue is null && converter.CanConvertTo(typeof(string)) && converter.CanConvertFrom(typeof(string)))
                {
                    object stringRep = converter.ConvertToInvariantString(value);
                    clonedValue = converter.ConvertFromInvariantString((string)stringRep);
                }
            }

            // How about serialization?
            if (clonedValue is null && type.IsSerializable)
            {
                BinaryFormatter f = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                f.Serialize(ms, value);
                ms.Position = 0;
                clonedValue = f.Deserialize(ms);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }

            if (clonedValue is not null)
            {
                return clonedValue;
            }

            // we failed.  This object's reference will be set on each property.
            return value;
        }

        /// <summary>
        ///  Creates a collection of attributes using the
        ///  array of attributes that you passed to the constructor.
        /// </summary>
        protected override AttributeCollection CreateAttributeCollection()
        {
            return new MergedAttributeCollection(this);
        }

        private object GetPropertyOwnerForComponent(Array a, int i)
        {
            object propertyOwner = a.GetValue(i);
            if (propertyOwner is ICustomTypeDescriptor)
            {
                propertyOwner = ((ICustomTypeDescriptor)propertyOwner).GetPropertyOwner(_descriptors[i]);
            }

            return propertyOwner;
        }

        /// <summary>
        ///  Gets an editor of the specified type.
        /// </summary>
        public override object GetEditor(Type editorBaseType)
        {
            return _descriptors[0].GetEditor(editorBaseType);
        }

        /// <summary>
        ///  When overridden in a derived class, gets the current
        ///  value
        ///  of the
        ///  property on a component.
        /// </summary>
        public override object GetValue(object component)
        {
            Debug.Assert(component is Array, "MergePropertyDescriptor::GetValue called with non-array value");
            return GetValue((Array)component, out bool temp);
        }

        public object GetValue(Array components, out bool allEqual)
        {
            allEqual = true;
            object obj = _descriptors[0].GetValue(GetPropertyOwnerForComponent(components, 0));

            if (obj is ICollection)
            {
                if (_collection is null)
                {
                    _collection = new MultiMergeCollection((ICollection)obj);
                }
                else if (_collection.Locked)
                {
                    return _collection;
                }
                else
                {
                    _collection.SetItems((ICollection)obj);
                }
            }

            for (int i = 1; i < _descriptors.Length; i++)
            {
                object objCur = _descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));

                if (_collection is not null)
                {
                    if (!_collection.MergeCollection((ICollection)objCur))
                    {
                        allEqual = false;
                        return null;
                    }
                }
                else if ((obj is null && objCur is null) ||
                         (obj is not null && obj.Equals(objCur)))
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

            return (_collection ?? obj);
        }

        internal object[] GetValues(Array components)
        {
            object[] values = new object[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                values[i] = _descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));
            }

            return values;
        }

        /// <summary>
        ///  When overridden in a derived class, resets the
        ///  value
        ///  for this property
        ///  of the component.
        /// </summary>
        public override void ResetValue(object component)
        {
            Debug.Assert(component is Array, "MergePropertyDescriptor::ResetValue called with non-array value");
            Array a = (Array)component;
            for (int i = 0; i < _descriptors.Length; i++)
            {
                _descriptors[i].ResetValue(GetPropertyOwnerForComponent(a, i));
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

                // now we have to copy the value into each property.
                object[] values = new object[listValue.Count];

                listValue.CopyTo(values, 0);

                for (int i = 0; i < _descriptors.Length; i++)
                {
                    if (!(_descriptors[i].GetValue(GetPropertyOwnerForComponent(a, i)) is IList propList))
                    {
                        continue;
                    }

                    propList.Clear();
                    foreach (object val in values)
                    {
                        propList.Add(val);
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

        /// <summary>
        ///  When overridden in a derived class, sets the value of
        ///  the component to a different value.
        /// </summary>
        public override void SetValue(object component, object value)
        {
            Debug.Assert(component is Array, "MergePropertyDescriptor::SetValue called with non-array value");
            Array a = (Array)component;
            if (value is IList && typeof(IList).IsAssignableFrom(PropertyType))
            {
                SetCollectionValues(a, (IList)value);
            }
            else
            {
                for (int i = 0; i < _descriptors.Length; i++)
                {
                    object clonedValue = CopyValue(value);
                    _descriptors[i].SetValue(GetPropertyOwnerForComponent(a, i), clonedValue);
                }
            }
        }

        /// <summary>
        ///  When overridden in a derived class, indicates whether the
        ///  value of
        ///  this property needs to be persisted.
        /// </summary>
        public override bool ShouldSerializeValue(object component)
        {
            Debug.Assert(component is Array, "MergePropertyDescriptor::ShouldSerializeValue called with non-array value");
            Array a = (Array)component;
            for (int i = 0; i < _descriptors.Length; i++)
            {
                if (!_descriptors[i].ShouldSerializeValue(GetPropertyOwnerForComponent(a, i)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
