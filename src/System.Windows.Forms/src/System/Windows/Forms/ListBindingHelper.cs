// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Diagnostics;    
    using System.ComponentModel;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper"]/*' />
    /// <devdoc>
    /// </devdoc>
    public static class ListBindingHelper {

        private static Attribute[] browsableAttribute;

        private static Attribute[] BrowsableAttributeList {
            get {
                if (browsableAttribute == null) {
                    browsableAttribute = new Attribute[] {new BrowsableAttribute(true)};
                }

                return browsableAttribute;
            }
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetList"]/*' />
        public static object GetList(object list) {
            if (list is IListSource) {
                return (list as IListSource).GetList();
            }
            else {
                return list;
            }
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetList2"]/*' />
        public static object GetList(object dataSource, string dataMember) {
            //
            // The purpose of this method is to find a list, given a 'data source' object and a
            // decription of some 'data member' property of that object which returns the list.
            //
            // - If the data source is not a list, we get the list by just querying for the
            //   current value of that property on the data source itself.
            //
            // - If the data source is a list, we have to first pick some item from that list,
            //   then query for the value of that property on the individual list item.
            //

            dataSource = GetList(dataSource);
            if (dataSource == null || dataSource is Type || String.IsNullOrEmpty(dataMember)) {
                return dataSource;
            }

            PropertyDescriptorCollection dsProps = ListBindingHelper.GetListItemProperties(dataSource);
            PropertyDescriptor dmProp = dsProps.Find(dataMember, true);
            if (dmProp == null) {
                throw new System.ArgumentException(string.Format(SR.DataSourceDataMemberPropNotFound, dataMember));
            }

            object currentItem;

            if (dataSource is ICurrencyManagerProvider) {
                // Data source is another BindingSource so ask for its current item
                CurrencyManager cm = (dataSource as ICurrencyManagerProvider).CurrencyManager;
                bool currentKnown = (cm != null && cm.Position >= 0 && cm.Position <= cm.Count - 1);
                currentItem = currentKnown ? cm.Current : null;
            }
            else if (dataSource is IEnumerable) {
                // Data source is an enumerable list, so walk to the first item
                currentItem = GetFirstItemByEnumerable(dataSource as IEnumerable);
            }
            else {
                // Data source is not a list, so just use the data source itself
                currentItem = dataSource;
            }

            // Query the data member property on the chosen object to get back the list
            return (currentItem == null) ? null : dmProp.GetValue(currentItem);
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetListName"]/*' />
        public static string GetListName(object list, PropertyDescriptor[] listAccessors) {
            string name;

            if (list == null) {
                return string.Empty;
            }
            ITypedList typedList = list as ITypedList;
            if (typedList != null) {
                // Use typed list
                name = typedList.GetListName(listAccessors);
            }
            else {
                Type type;
                // We always resolve via type in this case (not an instance)
                if ((null == listAccessors) || (listAccessors.Length == 0)) {
                    Type listAsType = list as Type;
                    if (listAsType != null) {
                        type = listAsType;
                    }
                    else {
                        type = list.GetType();
                    }
                }
                else {
                    // We don't walk down - always use type name
                    PropertyDescriptor pd = listAccessors[0];
                    type = pd.PropertyType;
                }

                name = GetListNameFromType(type);
            }

            return name;
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetListItemProperties"]/*' />
        public static PropertyDescriptorCollection GetListItemProperties(object list) {
            PropertyDescriptorCollection pdc;

            if (list == null) {
                return new PropertyDescriptorCollection(null);
            } else if (list is Type) {
                pdc = GetListItemPropertiesByType(list as Type);
            }
            else {
                object target = GetList(list);

                if (target is ITypedList) {
                    pdc = (target as ITypedList).GetItemProperties(null);
                }
                else if (target is IEnumerable) {
                    pdc = GetListItemPropertiesByEnumerable(target as IEnumerable);
                }
                else {
                    pdc = TypeDescriptor.GetProperties(target);
                }
            }

            return pdc;
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetListItemProperties1"]/*' />
        public static PropertyDescriptorCollection GetListItemProperties(object list, PropertyDescriptor[] listAccessors) {
            PropertyDescriptorCollection pdc;

            if ((null == listAccessors) || (listAccessors.Length == 0)) {
                pdc = GetListItemProperties(list);
            }
            else {
                if (list is Type) {
                    pdc = GetListItemPropertiesByType(list as Type, listAccessors);
                }
                else {
                    object target = GetList(list);

                    if (target is ITypedList) {
                        pdc = (target as ITypedList).GetItemProperties(listAccessors);
                    }
                    else if (target is IEnumerable) {
                        pdc = GetListItemPropertiesByEnumerable(target as IEnumerable, listAccessors);
                    }
                    else {
                        pdc = GetListItemPropertiesByInstance(target, listAccessors, 0);
                    }
                }
            }

            return pdc;
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetListItemProperties2"]/*' />
        public static PropertyDescriptorCollection GetListItemProperties(object dataSource, string dataMember, PropertyDescriptor[] listAccessors) {
            dataSource = GetList(dataSource);

            if (!String.IsNullOrEmpty(dataMember)) {
                // Find the property on the data source specified by the data member
                PropertyDescriptorCollection dsProps = ListBindingHelper.GetListItemProperties(dataSource);
                PropertyDescriptor dmProp = dsProps.Find(dataMember, true);

                // Error: Property not found - data member is invalid
                if (dmProp == null) {
                    throw new System.ArgumentException(string.Format(SR.DataSourceDataMemberPropNotFound, dataMember));
                }

                // Add the data member property to the list accessors
                int len = (listAccessors == null) ? 1 : (listAccessors.Length + 1);
                PropertyDescriptor[] listAccessors2 = new PropertyDescriptor[len];
                listAccessors2[0] = dmProp;
                for (int i = 1; i < len; ++i) {
                    listAccessors2[i] = listAccessors[i - 1];
                }

                // Replace old accessors with new accessors
                listAccessors = listAccessors2;
            }

            return GetListItemProperties(dataSource, listAccessors);
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetListItemType"]/*' />
        public static Type GetListItemType(object list) {
            if (list == null) {
                return null;
            }

            Type itemType = null;

            // special case for IListSource
            if ((list is Type) && (typeof(IListSource).IsAssignableFrom(list as Type))) {
                list = CreateInstanceOfType(list as Type);
            }

            list = GetList(list);
            Type listType = (list is Type) ? (list as Type) : list.GetType();
            object listInstance = (list is Type) ? null : list;

            if (typeof(Array).IsAssignableFrom(listType)) {
                itemType = listType.GetElementType();
            }
            else {
                PropertyInfo indexer = GetTypedIndexer(listType);

                if (indexer != null) {
                    itemType = indexer.PropertyType;
                }
                else if (listInstance is IEnumerable) {
                    itemType = GetListItemTypeByEnumerable(listInstance as IEnumerable);
                }
                else {
                    itemType = listType;
                }
            }

            return itemType;
        }

        // Create an object of the given type. Throw an exception if this fails.
        private static object CreateInstanceOfType(Type type) {
            object instancedObject = null;
            Exception instanceException = null;

            try {
                instancedObject = SecurityUtils.SecureCreateInstance(type);
            }
            catch (TargetInvocationException ex) {
                instanceException = ex; // Default ctor threw an exception
            }
            catch (MethodAccessException ex) {
                instanceException = ex; // Default ctor was not public
            }
            catch (MissingMethodException ex) {
                instanceException = ex; // No default ctor defined
            }

            if (instanceException != null) {
                throw new NotSupportedException(SR.BindingSourceInstanceError, instanceException);
            }

            return instancedObject;
        }

        /// <include file='doc\ListBindingHelper.uex' path='docs/doc[@for="ListBindingHelper.GetListItemType2"]/*' />
        public static Type GetListItemType(object dataSource, string dataMember) {
            // No data source
            if (dataSource == null) {
                return typeof(Object);
            }

            // No data member - Determine item type directly from data source
            if (String.IsNullOrEmpty(dataMember)) {
                return GetListItemType(dataSource);
            }

            // Get list item properties for this data source
            PropertyDescriptorCollection dsProps = GetListItemProperties(dataSource);
            if (dsProps == null) {
                return typeof(Object);
            }

            // Find the property specified by the data member
            PropertyDescriptor dmProp = dsProps.Find(dataMember, true);
            if (dmProp == null || dmProp.PropertyType is ICustomTypeDescriptor) {
                return typeof(Object);
            }

            // Determine item type from data member property
            return GetListItemType(dmProp.PropertyType);
        }

        private static string GetListNameFromType(Type type) {
            string name;

            if (typeof(Array).IsAssignableFrom(type)) {
                // If the type is Customers[], this will return "Customers"
                name = type.GetElementType().Name;
            }
            else if (typeof(IList).IsAssignableFrom(type)) {
                // If the type is BindingList<T>, TCollection, TList (or equiv), this will return "T"
                PropertyInfo indexer = GetTypedIndexer(type);
                if (indexer != null)
                {
                    name = indexer.PropertyType.Name;
                }
                else
                {
                    name = type.Name;
                }
            }
            else {
                // Fallback to type name
                name = type.Name;
            }

            return name;
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByType(Type type, PropertyDescriptor[] listAccessors) {
            PropertyDescriptorCollection pdc = null;

            if ((null == listAccessors) || (listAccessors.Length == 0)) {
                pdc = GetListItemPropertiesByType(type);
            }
            else {
                pdc = GetListItemPropertiesByType(type, listAccessors, 0);
            }

            return pdc;
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByType(Type type, PropertyDescriptor[] listAccessors, int startIndex) {
            PropertyDescriptorCollection pdc = null;
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
            startIndex = startIndex + 1;

            if (startIndex >= listAccessors.Length) {
                // Last item, return shape of item
                pdc = GetListItemProperties(subType);
            }
            else {
                // Walk down the tree
                pdc = GetListItemPropertiesByType(subType, listAccessors, startIndex);
            }
            // Return descriptors
            return pdc;
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable iEnumerable, PropertyDescriptor[] listAccessors, int startIndex) {
            PropertyDescriptorCollection pdc = null;
            object subList = null;
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
            object instance = GetFirstItemByEnumerable(iEnumerable);

            if (instance != null) {
                // This calls GetValue(Customers[0], "Orders") - or Customers[0].Orders
                // If this list is non-null, it is an instance of Orders (Order[]) for the first customer
                subList = GetList(listAccessors[startIndex].GetValue(instance));
            }

            if (null == subList) {
                // Can't get shape by Instance, try by Type
                pdc = GetListItemPropertiesByType(listAccessors[startIndex].PropertyType, listAccessors, startIndex);
            }
            else {
                // We have the Instance (e.g. Orders)
                ++startIndex;

                IEnumerable ienumerableSubList = subList as IEnumerable;
                if (ienumerableSubList != null) {
                    if (startIndex == listAccessors.Length) {
                        // Last one, so get the shape
                        pdc = GetListItemPropertiesByEnumerable(ienumerableSubList);
                    }
                    else {
                        // Looks like they want more (e.g. Customers.Orders.OrderDetails)
                        pdc = GetListItemPropertiesByEnumerable(ienumerableSubList, listAccessors, startIndex);
                    }
                }
                else {
                    // Not a list, so switch to a non-list based method of retrieving properties
                    pdc = GetListItemPropertiesByInstance(subList, listAccessors, startIndex);
                }
            }

            return pdc;
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable enumerable, PropertyDescriptor[] listAccessors) {
            PropertyDescriptorCollection pdc = null;

            if ((null == listAccessors) || (listAccessors.Length == 0)) {
                pdc = GetListItemPropertiesByEnumerable(enumerable);
            }
            else  {
                ITypedList typedList = enumerable as ITypedList;
                if (typedList != null) {
                    pdc = typedList.GetItemProperties(listAccessors);
                }
                else {
                    // Walk the tree
                    pdc = GetListItemPropertiesByEnumerable(enumerable, listAccessors, 0);
                }
            }
            return pdc;
        }

        private static Type GetListItemTypeByEnumerable(IEnumerable iEnumerable) {
            object instance = GetFirstItemByEnumerable(iEnumerable);
            return (instance != null) ? instance.GetType() : typeof(object);
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByInstance(object target, PropertyDescriptor[] listAccessors, int startIndex) {
            // At this point, things can be simplified because:
            //   We know target is _not_ a list
            //   We have an instance

            PropertyDescriptorCollection pdc;

            // Get the value of the first listAccessor
            if (listAccessors != null && listAccessors.Length > startIndex) {
                // Get the value (e.g. given Foo with property Bar, this gets Foo.Bar)
                object value = listAccessors[startIndex].GetValue(target);

                if (value == null) {
                    // It's null - we can't walk down by Instance so use Type
                    pdc = GetListItemPropertiesByType(listAccessors[startIndex].PropertyType, listAccessors, startIndex);
                }
                else {
                    PropertyDescriptor[] accessors = null;

                    if (listAccessors.Length > startIndex + 1) {
                        int accessorsCount = listAccessors.Length - (startIndex + 1);
                        accessors = new PropertyDescriptor[accessorsCount];
                        for (int i = 0; i < accessorsCount; ++i) {
                            accessors[i] = listAccessors[startIndex + 1 + i];
                        }
                    }

                    // We've got the instance of Bar - now get it's shape
                    pdc = GetListItemProperties(value, accessors);
                }
            }
            else {
                // Fallback to TypeDescriptor
                pdc = TypeDescriptor.GetProperties(target, BrowsableAttributeList);
            }

            return pdc;
        }

        // returns true if 'type' can be treated as a list
        private static bool IsListBasedType(Type type)
        {
            // check for IList, ITypedList, IListSource
            if (typeof(IList).IsAssignableFrom(type) ||
                typeof(ITypedList).IsAssignableFrom(type) ||
                typeof(IListSource).IsAssignableFrom(type)) {
                return true;
            }

            // check for IList<>:
            if (type.IsGenericType && !type.IsGenericTypeDefinition) {
                if (typeof(IList<>).IsAssignableFrom(type.GetGenericTypeDefinition())) {
                    return true;
                }
            }

            // check for SomeObject<T> : IList<T> / SomeObject : IList<(SpecificListObjectType)>
            foreach (Type curInterface in type.GetInterfaces()) {
                if (curInterface.IsGenericType) {
                    if (typeof(IList<>).IsAssignableFrom(curInterface.GetGenericTypeDefinition())) {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <devdoc>
        /// 
        /// Returns info about the 'indexer' property on the specified type. The presence of an indexer is used to
        /// determine that the type represents a collection or list. The return type of that indexer is used to
        /// determine the underlying item type.
        /// 
        /// PROCESS: We look for the first public instance property on the type that is an 'indexer'. This property
        /// is usually - but not always - called "Item". So we look at 'indexer parameters' to identify true indexers,
        /// rather than looking at the property name. And we also ignore any indexers that return an item type of just
        /// Object, since we are trying to use indexers here to determine the actual underlying item type!
        ///
        /// NOTE: A special rule is also enforced here - we only want to consider using the typed indexer on list
        /// based types, ie. types we already know are supposed to be treated as lists (rather than list items).
        ///
        /// </devdoc>
        private static PropertyInfo GetTypedIndexer(Type type)
        {
            PropertyInfo indexer = null;

            if (!IsListBasedType(type)) {
                return null;
            }

            System.Reflection.PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int idx = 0; idx < props.Length; idx++) {
                if (props[idx].GetIndexParameters().Length > 0 && props[idx].PropertyType != typeof(object)) {
                    indexer = props[idx];
                    //Prefer the standard indexer, if there is one
                    if (indexer.Name == "Item") {
                        break;
                    }
                }
            }

            return indexer;
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByType(Type type) {
            return TypeDescriptor.GetProperties(GetListItemType(type), BrowsableAttributeList);
        }

        private static PropertyDescriptorCollection GetListItemPropertiesByEnumerable(IEnumerable enumerable) {
            PropertyDescriptorCollection pdc = null;
            Type targetType = enumerable.GetType();

            if (typeof(Array).IsAssignableFrom(targetType)) {
                pdc = TypeDescriptor.GetProperties(targetType.GetElementType(), BrowsableAttributeList);
            }
            else {
                ITypedList typedListEnumerable = enumerable as ITypedList;
                if (typedListEnumerable != null) {
                    pdc = typedListEnumerable.GetItemProperties(null);
                }
                else {
                    PropertyInfo indexer = GetTypedIndexer(targetType);

                    if (indexer != null && !typeof(ICustomTypeDescriptor).IsAssignableFrom(indexer.PropertyType)) {
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
                        //         if (props != null) {
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
            if (null == pdc) {
                object instance = GetFirstItemByEnumerable(enumerable);
                if (enumerable is string) {
                    pdc = TypeDescriptor.GetProperties(enumerable, BrowsableAttributeList);
                }
                else if (instance == null) {
                    pdc = new PropertyDescriptorCollection(null);
                } 
                else {
                    pdc = TypeDescriptor.GetProperties(instance, BrowsableAttributeList);

                    if (!(enumerable is IList) && (pdc == null || pdc.Count == 0)) {
                        pdc = TypeDescriptor.GetProperties(enumerable, BrowsableAttributeList);
                    }
                }
                
            }

            // Return results
            return pdc;
        }

        private static object GetFirstItemByEnumerable(IEnumerable enumerable) {
            object instance = null;

            if (enumerable is IList) {
                // If the list supports IList (which is a superset of IEnumerable), then try to use its IList indexer
                // to get the first item, since some ILists don't support use of their plain IEnumerable interface.
                IList list = enumerable as IList;
                instance = (list.Count > 0) ? list[0] : null;
            }
            else {
                // Otherwise use the enumerator to get the first item...
                try {
                    IEnumerator listEnumerator = enumerable.GetEnumerator();

                    listEnumerator.Reset();

                    if (listEnumerator.MoveNext())
                        instance = listEnumerator.Current;

                    // after we are done w/ the enumerator, reset it
                    listEnumerator.Reset();
                }
                catch (NotSupportedException) {
                    // Some data sources do not offer a full implementation of IEnumerable. For example, SqlDataReader
                    // only supports reading forwards through items, so it does not support calls to IEnumerable.Reset().
                    instance = null;
                }
            }

            return instance;
        }

    }

}
