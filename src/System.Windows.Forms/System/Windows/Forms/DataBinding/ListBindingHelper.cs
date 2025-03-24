// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Reflection;

namespace System.Windows.Forms;

public static class ListBindingHelper
{
    private static Attribute[]? s_browsableAttribute;

    private static Attribute[] BrowsableAttributeList => s_browsableAttribute ??= [new BrowsableAttribute(true)];

    public static object? GetList(object? list)
    {
        if (list is IListSource listSource)
        {
            return listSource.GetList();
        }
        else
        {
            return list;
        }
    }

    public static object? GetList(object? dataSource, string? dataMember)
    {
        //
        // The purpose of this method is to find a list, given a 'data source' object and a
        // description of some 'data member' property of that object which returns the list.
        //
        // - If the data source is not a list, we get the list by just querying for the
        //   current value of that property on the data source itself.
        //
        // - If the data source is a list, we have to first pick some item from that list,
        //   then query for the value of that property on the individual list item.
        //

        dataSource = GetList(dataSource);
        if (dataSource is null || dataSource is Type || string.IsNullOrEmpty(dataMember))
        {
            return dataSource;
        }

        PropertyDescriptorCollection dsProps = GetListItemProperties(dataSource);
        PropertyDescriptor? dmProp = dsProps.Find(dataMember, true)
            ?? throw new ArgumentException(string.Format(SR.DataSourceDataMemberPropNotFound, dataMember));

        object? currentItem;

        if (dataSource is ICurrencyManagerProvider currencyManagerProvider)
        {
            // Data source is another BindingSource so ask for its current item
            CurrencyManager? cm = currencyManagerProvider.CurrencyManager;
            bool currentKnown = (cm is not null && cm.Position >= 0 && cm.Position <= cm.Count - 1);
            currentItem = currentKnown ? cm!.Current : null;
        }
        else if (dataSource is IEnumerable enumerable)
        {
            // Data source is an enumerable list, so walk to the first item
            currentItem = GetFirstItemByEnumerable(enumerable);
        }
        else
        {
            // Data source is not a list, so just use the data source itself
            currentItem = dataSource;
        }

        // Query the data member property on the chosen object to get back the list
        return (currentItem is null) ? null : dmProp.GetValue(currentItem);
    }

    public static string GetListName(object? list, PropertyDescriptor[]? listAccessors)
    {
        if (list is null)
        {
            return string.Empty;
        }

        string name;

        if (list is ITypedList typedList)
        {
            // Use typed list
            name = typedList.GetListName(listAccessors);
        }
        else
        {
            Type type;
            // We always resolve via type in this case (not an instance)
            if (listAccessors is null || listAccessors.Length == 0 || listAccessors[0] is null)
            {
                if (list is Type listAsType)
                {
                    type = listAsType;
                }
                else
                {
                    type = list.GetType();
                }
            }
            else
            {
                // We don't walk down - always use type name
                type = listAccessors[0].PropertyType;
            }

            name = GetListNameFromType(type);
        }

        return name;
    }

    public static PropertyDescriptorCollection GetListItemProperties(object? list)
    {
        PropertyDescriptorCollection pdc;

        if (list is null)
        {
            return new PropertyDescriptorCollection(null);
        }
        else if (list is Type type)
        {
            pdc = GetListItemPropertiesByType(type);
        }
        else
        {
            object? target = GetList(list);

            if (target is ITypedList typedList)
            {
                pdc = typedList.GetItemProperties(null);
            }
            else if (target is IEnumerable enumerable)
            {
                pdc = GetListItemPropertiesByEnumerable(enumerable);
            }
            else
            {
                pdc = TypeDescriptor.GetProperties(target!);
            }
        }

        return pdc;
    }

    public static PropertyDescriptorCollection GetListItemProperties(object? list, PropertyDescriptor[]? listAccessors)
    {
        if (listAccessors is null || listAccessors.Length == 0)
        {
            return GetListItemProperties(list);
        }
        else if (list is Type)
        {
            return GetListItemPropertiesByType(listAccessors, 0);
        }

        object? target = GetList(list);

        if (target is ITypedList typedList)
        {
            return typedList.GetItemProperties(listAccessors);
        }
        else if (target is IEnumerable enumerable)
        {
            return GetListItemPropertiesByEnumerable(enumerable, listAccessors, 0);
        }

        return GetListItemPropertiesByInstance(target, listAccessors, 0);
    }

    public static PropertyDescriptorCollection GetListItemProperties(object? dataSource, string? dataMember, PropertyDescriptor[]? listAccessors)
    {
        dataSource = GetList(dataSource);

        if (!string.IsNullOrEmpty(dataMember))
        {
            // Find the property on the data source specified by the data member
            PropertyDescriptorCollection dsProps = GetListItemProperties(dataSource);
            PropertyDescriptor? dmProp = dsProps.Find(dataMember, true);

            // Add the data member property to the list accessors
            int len = (listAccessors is null) ? 1 : (listAccessors.Length + 1);
            PropertyDescriptor[] listAccessors2 = new PropertyDescriptor[len];
            listAccessors2[0] = dmProp ?? throw new ArgumentException(string.Format(SR.DataSourceDataMemberPropNotFound, dataMember));
            for (int i = 1; i < len; ++i)
            {
                listAccessors2[i] = listAccessors![i - 1];
            }

            // Replace old accessors with new accessors
            listAccessors = listAccessors2;
        }

        return GetListItemProperties(dataSource, listAccessors);
    }

    [return: NotNullIfNotNull(nameof(list))]
    public static Type? GetListItemType(object? list)
    {
        if (list is null)
        {
            return null;
        }

        // special case for IListSource
        if (list is Type listAsType && typeof(IListSource).IsAssignableFrom(listAsType))
        {
            list = CreateInstanceOfType(listAsType);
        }

        list = GetList(list);
        if (list is null)
        {
            return null;
        }

        Type listType = (list is Type type) ? type : list.GetType();
        object? listInstance = (list is Type) ? null : list;

        if (typeof(Array).IsAssignableFrom(listType))
        {
            return listType.GetElementType()!;
        }

        PropertyInfo? indexer = GetTypedIndexer(listType);
        if (indexer is not null)
        {
            return indexer.PropertyType;
        }
        else if (listInstance is IEnumerable enumerable)
        {
            return GetListItemTypeByEnumerable(enumerable);
        }

        return listType;
    }

    // Create an object of the given type. Throw an exception if this fails.
    [ExcludeFromCodeCoverage]
    private static object? CreateInstanceOfType(Type type)
    {
        object? instancedObject = null;
        Exception? instanceException = null;

        try
        {
            instancedObject = Activator.CreateInstance(type);
        }
        catch (TargetInvocationException ex)
        {
            instanceException = ex; // Default ctor threw an exception
        }
        catch (MethodAccessException ex)
        {
            instanceException = ex; // Default ctor was not public
        }
        catch (MissingMethodException ex)
        {
            instanceException = ex; // No default ctor defined
        }

        if (instanceException is not null)
        {
            throw new NotSupportedException(SR.BindingSourceInstanceError, instanceException);
        }

        return instancedObject;
    }

    public static Type GetListItemType(object? dataSource, string? dataMember)
    {
        // No data source
        if (dataSource is null)
        {
            return typeof(object);
        }

        // No data member - Determine item type directly from data source
        if (string.IsNullOrEmpty(dataMember))
        {
            return GetListItemType(dataSource);
        }

        // Get list item properties for this data source
        PropertyDescriptorCollection dsProps = GetListItemProperties(dataSource);
        if (dsProps is null)
        {
            return typeof(object);
        }

        // Find the property specified by the data member
        PropertyDescriptor? dmProp = dsProps.Find(dataMember, true);
        if (dmProp is null || dmProp.PropertyType is ICustomTypeDescriptor)
        {
            return typeof(object);
        }

        // Determine item type from data member property
        return GetListItemType(dmProp.PropertyType);
    }

    private static string GetListNameFromType(Type type)
    {
        string name;

        if (typeof(Array).IsAssignableFrom(type))
        {
            // If the type is Customers[], this will return "Customers"
            Type? elementType = type.GetElementType();
            if (elementType is not null)
            {
                name = elementType.Name;
            }
            else
            {
                // Fallback to type name
                name = type.Name;
            }
        }
        else if (typeof(IList).IsAssignableFrom(type))
        {
            // If the type is BindingList<T>, TCollection, TList (or equiv), this will return "T"
            PropertyInfo? indexer = GetTypedIndexer(type);
            if (indexer is not null)
            {
                name = indexer.PropertyType.Name;
            }
            else
            {
                name = type.Name;
            }
        }
        else
        {
            // Fallback to type name
            name = type.Name;
        }

        return name;
    }

    private static PropertyDescriptorCollection GetListItemPropertiesByType(PropertyDescriptor[] listAccessors, int startIndex)
    {
        PropertyDescriptorCollection? pdc;
        if (listAccessors[startIndex] is null)
        {
            return new PropertyDescriptorCollection(null);
        }

        Type subType = listAccessors[startIndex].PropertyType;

        // subType is the property type - which is not to be confused with the item type.
        // For example, if a class Customer has a property of type Orders[], then Given:
        //        GetListItemProperties(typeof(Customer), PDForOrders)
        //      PDForOrders.PropertyType will be Array (not Orders)
        //
        //        If there are no more ListAccessors, then we want:
        //            GetListItemProperties(PDForOrders.PropertyType)  // this returns the shape of Orders not Array
        //        If there are more listAccessors, then we'll call
        //            GetListItemProperties(PDForOrders.PropertyType, listAccessors, startIndex++)
        startIndex += 1;

        if (startIndex >= listAccessors.Length)
        {
            // Last item, return shape of item
            pdc = GetListItemProperties(subType);
        }
        else
        {
            // Walk down the tree
            pdc = GetListItemPropertiesByType(listAccessors, startIndex);
        }

        // Return descriptors
        return pdc;
    }

    private static PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable iEnumerable, PropertyDescriptor[] listAccessors, int startIndex)
    {
        PropertyDescriptorCollection? pdc;
        object? subList = null;

        // Walk down the tree - first try and get the value
        // This is tricky, because we can't do a standard GetValue - we need an instance of one of the
        // items in the list.
        //
        // For example:
        //
        //        Customer has a property Orders which is of type Order[]
        //
        //        Customers is a Customer[] (this is the IList)
        //        Customers does not have the property "Orders" - Customer has that property
        //        So we need to get the value of Customers[0]
        //
        object? instance = GetFirstItemByEnumerable(iEnumerable);

        if (instance is not null)
        {
            // This calls GetValue(Customers[0], "Orders") - or Customers[0].Orders
            // If this list is non-null, it is an instance of Orders (Order[]) for the first customer
            subList = GetList(listAccessors[startIndex].GetValue(instance));
        }

        if (subList is null)
        {
            // Can't get shape by Instance, try by Type
            pdc = GetListItemPropertiesByType(listAccessors, startIndex);
        }
        else
        {
            // We have the Instance (e.g. Orders)
            ++startIndex;

            if (subList is IEnumerable ienumerableSubList)
            {
                if (startIndex == listAccessors.Length)
                {
                    // Last one, so get the shape
                    pdc = GetListItemPropertiesByEnumerable(ienumerableSubList);
                }
                else
                {
                    // Looks like they want more (e.g. Customers.Orders.OrderDetails)
                    pdc = GetListItemPropertiesByEnumerable(ienumerableSubList, listAccessors, startIndex);
                }
            }
            else
            {
                // Not a list, so switch to a non-list based method of retrieving properties
                pdc = GetListItemPropertiesByInstance(subList, listAccessors, startIndex);
            }
        }

        return pdc;
    }

    private static Type GetListItemTypeByEnumerable(IEnumerable iEnumerable)
    {
        object? instance = GetFirstItemByEnumerable(iEnumerable);
        return (instance is not null) ? instance.GetType() : typeof(object);
    }

    private static PropertyDescriptorCollection GetListItemPropertiesByInstance(object? target, PropertyDescriptor[] listAccessors, int startIndex)
    {
        Debug.Assert(listAccessors is not null);

        // At this point, things can be simplified because:
        //   We know target is _not_ a list
        //   We have an instance
        if (listAccessors.Length > startIndex)
        {
            if (listAccessors[startIndex] is null)
            {
                return new PropertyDescriptorCollection(null);
            }

            // Get the value (e.g. given Foo with property Bar, this gets Foo.Bar)
            object? value = listAccessors[startIndex].GetValue(target);

            if (value is null)
            {
                // It's null - we can't walk down by Instance so use Type
                return GetListItemPropertiesByType(listAccessors, startIndex);
            }
            else
            {
                PropertyDescriptor[]? accessors = null;

                if (listAccessors.Length > startIndex + 1)
                {
                    int accessorsCount = listAccessors.Length - (startIndex + 1);
                    accessors = new PropertyDescriptor[accessorsCount];
                    for (int i = 0; i < accessorsCount; ++i)
                    {
                        accessors[i] = listAccessors[startIndex + 1 + i];
                    }
                }

                // We've got the instance of Bar - now get it's shape
                return GetListItemProperties(value, accessors);
            }
        }

        return TypeDescriptor.GetProperties(target!, BrowsableAttributeList);
    }

    // returns true if 'type' can be treated as a list
    private static bool IsListBasedType(Type type)
    {
        // check for IList, ITypedList, IListSource
        if (typeof(IList).IsAssignableFrom(type) ||
            typeof(ITypedList).IsAssignableFrom(type) ||
            typeof(IListSource).IsAssignableFrom(type))
        {
            return true;
        }

        // check for IList<>:
        if (type.IsGenericType && !type.IsGenericTypeDefinition)
        {
            if (typeof(IList<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                return true;
            }
        }

        // check for SomeObject<T> : IList<T> / SomeObject : IList<(SpecificListObjectType)>
        foreach (Type curInterface in type.GetInterfaces())
        {
            if (curInterface.IsGenericType)
            {
                if (typeof(IList<>).IsAssignableFrom(curInterface.GetGenericTypeDefinition()))
                {
                    return true;
                }
            }
        }

        return false;
    }

    /// <summary>
    ///
    ///  Returns info about the 'indexer' property on the specified type. The presence of an indexer is used to
    ///  determine that the type represents a collection or list. The return type of that indexer is used to
    ///  determine the underlying item type.
    ///
    ///  PROCESS: We look for the first public instance property on the type that is an 'indexer'. This property
    ///  is usually - but not always - called "Item". So we look at 'indexer parameters' to identify true indexers,
    ///  rather than looking at the property name. And we also ignore any indexers that return an item type of just
    ///  Object, since we are trying to use indexers here to determine the actual underlying item type!
    ///
    ///  NOTE: A special rule is also enforced here - we only want to consider using the typed indexer on list
    ///  based types, ie. types we already know are supposed to be treated as lists (rather than list items).
    /// </summary>
    private static PropertyInfo? GetTypedIndexer(Type type)
    {
        PropertyInfo? indexer = null;

        if (!IsListBasedType(type))
        {
            return null;
        }

        PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        for (int idx = 0; idx < props.Length; idx++)
        {
            if (props[idx].GetIndexParameters().Length > 0 && props[idx].PropertyType != typeof(object))
            {
                indexer = props[idx];
                // Prefer the standard indexer, if there is one
                if (indexer.Name == "Item")
                {
                    break;
                }
            }
        }

        return indexer;
    }

    private static PropertyDescriptorCollection GetListItemPropertiesByType(Type type)
    {
        return TypeDescriptor.GetProperties(GetListItemType(type), BrowsableAttributeList);
    }

    private static PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable enumerable)
    {
        PropertyDescriptorCollection? pdc = null;
        Type targetType = enumerable.GetType();

        if (typeof(Array).IsAssignableFrom(targetType))
        {
            pdc = TypeDescriptor.GetProperties(targetType.GetElementType()!, BrowsableAttributeList);
        }
        else
        {
            if (enumerable is ITypedList typedListEnumerable)
            {
                pdc = typedListEnumerable.GetItemProperties(null);
            }
            else
            {
                PropertyInfo? indexer = GetTypedIndexer(targetType);

                if (indexer is not null && !typeof(ICustomTypeDescriptor).IsAssignableFrom(indexer.PropertyType))
                {
                    Type type = indexer.PropertyType;
                    pdc = TypeDescriptor.GetProperties(type, BrowsableAttributeList);

                    // Reflection, and consequently TypeDescriptor would not return properties defined on the "base" interface,
                    // for example
                    // public interface IPerson {String FirstName { get; set; }}
                    // public interface ITeacher : IPerson {int ClassRoom { get; set; }}
                    // typeof (ITeacher).GetProperties() would return only the "ClassRoom" property

                    // if (type.IsInterface) {
                    //     Type[] interfaces = type.GetInterfaces();
                    //    // initialize the list to an arbitrary length greater than pdc.Count
                    //     List<PropertyDescriptor> merged = new List<PropertyDescriptor>(pdc.Count * 2 + 1);
                    //     foreach (Type baseInterface in interfaces) {
                    //         PropertyDescriptorCollection props = TypeDescriptor.GetProperties(baseInterface, BrowsableAttributeList);
                    //         if (props is not null) {
                    //             foreach (PropertyDescriptor p in props) {
                    //                 merged.Add(p);
                    //             }
                    //         }
                    //     }
                    //     if (merged.Count != 0) {
                    //         PropertyDescriptor[] props = new PropertyDescriptor[pdc.Count];
                    //         pdc.CopyTo(props, 0);
                    //         merged.AddRange(props);
                    //         pdc = new PropertyDescriptorCollection(merged.ToArray());
                    //     }
                    // }
                }
            }
        }

        // See if we were successful - if not, return the shape of the first
        // item in the list
        if (pdc is null)
        {
            object? instance = GetFirstItemByEnumerable(enumerable);
            if (enumerable is string)
            {
                pdc = TypeDescriptor.GetProperties(enumerable, BrowsableAttributeList);
            }
            else if (instance is null)
            {
                pdc = new PropertyDescriptorCollection(null);
            }
            else
            {
                pdc = TypeDescriptor.GetProperties(instance, BrowsableAttributeList);

                if (!(enumerable is IList) && pdc.Count == 0)
                {
                    pdc = TypeDescriptor.GetProperties(enumerable, BrowsableAttributeList);
                }
            }
        }

        // Return results
        return pdc;
    }

    private static object? GetFirstItemByEnumerable(IEnumerable enumerable)
    {
        object? instance = null;

        if (enumerable is IList list)
        {
            // If the list supports IList (which is a superset of IEnumerable), then try to use its IList indexer
            // to get the first item, since some ILists don't support use of their plain IEnumerable interface.
            instance = (list.Count > 0) ? list[0] : null;
        }
        else
        {
            // Otherwise use the enumerator to get the first item...
            try
            {
                IEnumerator listEnumerator = enumerable.GetEnumerator();
                if (listEnumerator is null)
                {
                    return null;
                }

                listEnumerator.Reset();

                if (listEnumerator.MoveNext())
                {
                    instance = listEnumerator.Current;
                }

                // after we are done w/ the enumerator, reset it
                listEnumerator.Reset();
            }
            catch (NotSupportedException)
            {
                // Some data sources do not offer a full implementation of IEnumerable. For example, SqlDataReader
                // only supports reading forwards through items, so it does not support calls to IEnumerable.Reset().
                instance = null;
            }
        }

        return instance;
    }
}
