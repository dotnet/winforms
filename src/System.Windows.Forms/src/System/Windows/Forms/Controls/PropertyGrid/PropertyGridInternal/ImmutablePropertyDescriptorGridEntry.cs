// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms.PropertyGridInternal;

/// <summary>
///  This grid entry is used for immutable objects.
/// </summary>
/// <remarks>
///  <para>
///   An immutable object is identified through it's <see cref="TypeConverter"/> when it returns true for
///   <see cref="TypeConverter.GetCreateInstanceSupported()"/>. In this case, we never go through the
///   <see cref="PropertyDescriptor"/> to change the value, but recreate the property object each time.
///  </para>
/// </remarks>
internal sealed class ImmutablePropertyDescriptorGridEntry : PropertyDescriptorGridEntry
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

    public override object? PropertyValue
    {
        get => base.PropertyValue;
        set
        {
            // Create a new instance of the value and set it into the parent grid entry.
            object? owner = GetValueOwner();
            object? newObject = null;
            GridEntry? parentEntry = InstanceParentGridEntry;
            if (parentEntry is null)
            {
                return;
            }

            TypeConverter parentConverter = parentEntry.TypeConverter;

            if (owner is null)
            {
                return;
            }

            PropertyDescriptorCollection? properties = parentConverter.GetProperties(parentEntry, owner);
            if (properties is not null)
            {
                Hashtable values = new Hashtable(properties.Count);
                for (int i = 0; i < properties.Count; i++)
                {
                    if (PropertyDescriptor.Name is not null && PropertyDescriptor.Name.Equals(properties[i].Name))
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
                            string.Format(SR.ExceptionCreatingObject, InstanceParentGridEntry?.PropertyType?.FullName, e),
                            e);
                    }
                    else
                    {
                        throw; // rethrow the same exception
                    }
                }
            }

            if (newObject is not null)
            {
                parentEntry.PropertyValue = newObject;
            }
        }
    }

    protected override bool SendNotification(object? owner, Notify notification)
        => ParentGridEntry?.SendNotificationToParent(notification) ?? false;

    public override bool ShouldRenderReadOnly => InstanceParentGridEntry?.ShouldRenderReadOnly ?? false;

    private GridEntry? InstanceParentGridEntry
    {
        get
        {
            GridEntry? parent = ParentGridEntry;

            if (parent is CategoryGridEntry)
            {
                parent = parent.ParentGridEntry;
            }

            return parent;
        }
    }
}
