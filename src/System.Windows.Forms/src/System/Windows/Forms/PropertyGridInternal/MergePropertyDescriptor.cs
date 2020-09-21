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
    internal class MergePropertyDescriptor : PropertyDescriptor
    {
        private readonly PropertyDescriptor[] descriptors;

        private enum TriState
        {
            Unknown,
            Yes,
            No
        }

        private TriState localizable = TriState.Unknown;
        private TriState readOnly = TriState.Unknown;
        private TriState canReset = TriState.Unknown;

        private MultiMergeCollection collection;

        public MergePropertyDescriptor(PropertyDescriptor[] descriptors) : base(descriptors[0].Name, null)
        {
            this.descriptors = descriptors;
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
                return descriptors[0].ComponentType;
            }
        }

        /// <summary>
        ///  Gets the type converter for this property.
        /// </summary>
        public override TypeConverter Converter
        {
            get
            {
                return descriptors[0].Converter;
            }
        }

        public override string DisplayName
        {
            get
            {
                return descriptors[0].DisplayName;
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
                if (localizable == TriState.Unknown)
                {
                    localizable = TriState.Yes;
                    foreach (PropertyDescriptor pd in descriptors)
                    {
                        if (!pd.IsLocalizable)
                        {
                            localizable = TriState.No;
                            break;
                        }
                    }
                }
                return (localizable == TriState.Yes);
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
                if (readOnly == TriState.Unknown)
                {
                    readOnly = TriState.No;
                    foreach (PropertyDescriptor pd in descriptors)
                    {
                        if (pd.IsReadOnly)
                        {
                            readOnly = TriState.Yes;
                            break;
                        }
                    }
                }
                return (readOnly == TriState.Yes);
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
                return descriptors[0].PropertyType;
            }
        }

        public PropertyDescriptor this[int index]
        {
            get
            {
                return descriptors[index];
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
            if (canReset == TriState.Unknown)
            {
                canReset = TriState.Yes;
                Array a = (Array)component;
                for (int i = 0; i < descriptors.Length; i++)
                {
                    if (!descriptors[i].CanResetValue(GetPropertyOwnerForComponent(a, i)))
                    {
                        canReset = TriState.No;
                        break;
                    }
                }
            }
            return (canReset == TriState.Yes);
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
                    if (desc != null && desc.IsComplete)
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
#pragma warning disable SYSLIB0011
                f.Serialize(ms, value);
                ms.Position = 0;
                clonedValue = f.Deserialize(ms);
#pragma warning restore SYSLIB0011
            }

            if (clonedValue != null)
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
                propertyOwner = ((ICustomTypeDescriptor)propertyOwner).GetPropertyOwner(descriptors[i]);
            }
            return propertyOwner;
        }

        /// <summary>
        ///  Gets an editor of the specified type.
        /// </summary>
        public override object GetEditor(Type editorBaseType)
        {
            return descriptors[0].GetEditor(editorBaseType);
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
            object obj = descriptors[0].GetValue(GetPropertyOwnerForComponent(components, 0));

            if (obj is ICollection)
            {
                if (collection is null)
                {
                    collection = new MultiMergeCollection((ICollection)obj);
                }
                else if (collection.Locked)
                {
                    return collection;
                }
                else
                {
                    collection.SetItems((ICollection)obj);
                }
            }

            for (int i = 1; i < descriptors.Length; i++)
            {
                object objCur = descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));

                if (collection != null)
                {
                    if (!collection.MergeCollection((ICollection)objCur))
                    {
                        allEqual = false;
                        return null;
                    }
                }
                else if ((obj is null && objCur is null) ||
                         (obj != null && obj.Equals(objCur)))
                {
                    continue;
                }
                else
                {
                    allEqual = false;
                    return null;
                }
            }

            if (allEqual && collection != null && collection.Count == 0)
            {
                return null;
            }

            return (collection ?? obj);
        }

        internal object[] GetValues(Array components)
        {
            object[] values = new object[components.Length];

            for (int i = 0; i < components.Length; i++)
            {
                values[i] = descriptors[i].GetValue(GetPropertyOwnerForComponent(components, i));
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
            for (int i = 0; i < descriptors.Length; i++)
            {
                descriptors[i].ResetValue(GetPropertyOwnerForComponent(a, i));
            }
        }

        private void SetCollectionValues(Array a, IList listValue)
        {
            try
            {
                if (collection != null)
                {
                    collection.Locked = true;
                }

                // now we have to copy the value into each property.
                object[] values = new object[listValue.Count];

                listValue.CopyTo(values, 0);

                for (int i = 0; i < descriptors.Length; i++)
                {
                    if (!(descriptors[i].GetValue(GetPropertyOwnerForComponent(a, i)) is IList propList))
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
                if (collection != null)
                {
                    collection.Locked = false;
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
                for (int i = 0; i < descriptors.Length; i++)
                {
                    object clonedValue = CopyValue(value);
                    descriptors[i].SetValue(GetPropertyOwnerForComponent(a, i), clonedValue);
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
            for (int i = 0; i < descriptors.Length; i++)
            {
                if (!descriptors[i].ShouldSerializeValue(GetPropertyOwnerForComponent(a, i)))
                {
                    return false;
                }
            }
            return true;
        }

        private class MultiMergeCollection : ICollection
        {
            private object[] items;
            private bool locked;

            public MultiMergeCollection(ICollection original)
            {
                SetItems(original);
            }

            /// <summary>
            ///  Retrieves the number of items.
            /// </summary>
            public int Count
            {
                get
                {
                    if (items != null)
                    {
                        return items.Length;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }

            /// <summary>
            ///  Prevents the contents of the collection from being re-initialized;
            /// </summary>
            public bool Locked
            {
                get
                {
                    return locked;
                }
                set
                {
                    locked = value;
                }
            }

            object ICollection.SyncRoot
            {
                get
                {
                    return this;
                }
            }

            bool ICollection.IsSynchronized
            {
                get
                {
                    return false;
                }
            }

            public void CopyTo(Array array, int index)
            {
                if (items is null)
                {
                    return;
                }

                Array.Copy(items, 0, array, index, items.Length);
            }

            public IEnumerator GetEnumerator()
            {
                if (items != null)
                {
                    return items.GetEnumerator();
                }
                else
                {
                    return Array.Empty<object>().GetEnumerator();
                }
            }

            /// <summary>
            ///  Ensures that the new collection equals the exisitng one.
            ///  Otherwise, it wipes out the contents of the new collection.
            /// </summary>
            public bool MergeCollection(ICollection newCollection)
            {
                if (locked)
                {
                    return true;
                }

                if (items.Length != newCollection.Count)
                {
                    items = Array.Empty<object>();
                    return false;
                }

                object[] newItems = new object[newCollection.Count];
                newCollection.CopyTo(newItems, 0);
                for (int i = 0; i < newItems.Length; i++)
                {
                    if (((newItems[i] is null) != (items[i] is null)) ||
                        (items[i] != null && !items[i].Equals(newItems[i])))
                    {
                        items = Array.Empty<object>();
                        return false;
                    }
                }
                return true;
            }

            public void SetItems(ICollection collection)
            {
                if (locked)
                {
                    return;
                }
                items = new object[collection.Count];
                collection.CopyTo(items, 0);
            }
        }

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
                if (foundAttributes != null)
                {
                    value = foundAttributes[attributeType] as Attribute;
                    if (value != null)
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
