// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal
{
    /// <summary>
    ///  This grid entry is used for immutable objects.  An immutable object is identified through it's
    ///  <see cref="TypeConverter"/>, which returns TRUE to ShouldCreateInstance.  For this case, we never go through
    ///  the property descriptor to change the value, but recreate each time
    /// </summary>
    internal class ImmutablePropertyDescriptorGridEntry : PropertyDescriptorGridEntry
    {
        internal ImmutablePropertyDescriptorGridEntry(
            PropertyGrid ownerGrid,
            GridEntry parent,
            PropertyDescriptor propertyInfo,
            bool hide)
            : base(ownerGrid, parent, propertyInfo, hide)
        {
        }

        internal override bool IsPropertyReadOnly => ShouldRenderReadOnly;

        public override object PropertyValue
        {
            get => base.PropertyValue;
            set
            {
                // Create a new instance of the value and set it into the parent grid entry.
                object owner = GetValueOwner();
                GridEntry parentEntry = InstanceParentGridEntry;
                TypeConverter parentConverter = parentEntry.TypeConverter;

                PropertyDescriptorCollection properties = parentConverter.GetProperties(parentEntry, owner);
                IDictionary values = new Hashtable(properties.Count);
                object newObject;

                for (int i = 0; i < properties.Count; i++)
                {
                    if (_propertyInfo.Name is not null && _propertyInfo.Name.Equals(properties[i].Name))
                    {
                        values[properties[i].Name] = value;
                    }
                    else
                    {
                        values[properties[i].Name] = properties[i].GetValue(owner);
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
                        throw new TargetInvocationException(
                            string.Format(SR.ExceptionCreatingObject, InstanceParentGridEntry.PropertyType.FullName, e.ToString()),
                            e);
                    }
                    else
                    {
                        throw; // rethrow the same exception
                    }
                }

                if (newObject is not null)
                {
                    parentEntry.PropertyValue = newObject;
                }
            }
        }

        protected internal override bool NotifyValueGivenParent(object obj, Notify type)
            => ParentGridEntry.NotifyValue(type);

        public override bool ShouldRenderReadOnly => InstanceParentGridEntry.ShouldRenderReadOnly;

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
