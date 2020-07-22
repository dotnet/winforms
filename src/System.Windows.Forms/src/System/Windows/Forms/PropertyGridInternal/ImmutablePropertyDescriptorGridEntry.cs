// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal
{
    // This grid entry is used for immutable objects.  An immutable object is identified
    // through it's TypeConverter, which returns TRUE to ShouldCreateInstance.  For this case,
    // we never go through the property descriptor to change the value, but recreate each
    // time.
    internal class ImmutablePropertyDescriptorGridEntry : PropertyDescriptorGridEntry
    {
        internal ImmutablePropertyDescriptorGridEntry(PropertyGrid ownerGrid, GridEntry peParent, PropertyDescriptor propInfo, bool hide)
        : base(ownerGrid, peParent, propInfo, hide)
        {
        }

        internal override bool IsPropertyReadOnly
        {
            get
            {
                return ShouldRenderReadOnly;
            }
        }

        public override object PropertyValue
        {
            get => base.PropertyValue;
            set
            {
                // Create a new instance of the value and set it into the parent grid entry.
                //
                object owner = GetValueOwner();
                GridEntry parentEntry = InstanceParentGridEntry;
                TypeConverter parentConverter = parentEntry.TypeConverter;

                PropertyDescriptorCollection props = parentConverter.GetProperties(parentEntry, owner);
                IDictionary values = new Hashtable(props.Count);
                object newObject = null;

                for (int i = 0; i < props.Count; i++)
                {
                    if (_propertyInfo.Name != null && _propertyInfo.Name.Equals(props[i].Name))
                    {
                        values[props[i].Name] = value;
                    }
                    else
                    {
                        values[props[i].Name] = props[i].GetValue(owner);
                    }
                }

                try
                {
                    newObject = parentConverter.CreateInstance(parentEntry, values);
                }
                catch (Exception e)
                {
                    if (string.IsNullOrEmpty(e.Message))
                    {
                        throw new TargetInvocationException(string.Format(SR.ExceptionCreatingObject,
                                                            InstanceParentGridEntry.PropertyType.FullName,
                                                            e.ToString()), e);
                    }
                    else
                    {
                        throw; // rethrow the same exception
                    }
                }

                if (newObject != null)
                {
                    parentEntry.PropertyValue = newObject;
                }
            }
        }

        internal override bool NotifyValueGivenParent(object obj, int type)
        {
            return ParentGridEntry.NotifyValue(type);
        }

        public override bool ShouldRenderReadOnly
        {
            get
            {
                return InstanceParentGridEntry.ShouldRenderReadOnly;
            }
        }

        private GridEntry InstanceParentGridEntry
        {
            get
            {
                GridEntry parent = ParentGridEntry;

                if (parent is CategoryGridEntry)
                {
                    parent = parent.ParentGridEntry;
                }

                return parent;
            }
        }
    }
}
