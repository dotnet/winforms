// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class MultiSelectRootGridEntry
{
    internal static class PropertyMerger
    {
        public static MultiPropertyDescriptorGridEntry[]? GetMergedProperties(
            object?[] objects,
            GridEntry parentEntry,
            PropertySort sort,
            PropertyTab? tab)
        {
            if (tab is null)
            {
                return null;
            }

            MultiPropertyDescriptorGridEntry[]? result = null;
            try
            {
                if ((sort & PropertySort.Alphabetical) != 0)
                {
                    List<PropertyDescriptor[]>? commonProperties = GetCommonProperties(objects, presort: true, tab, parentEntry);
                    if (commonProperties is not null)
                    {
                        var entries = new MultiPropertyDescriptorGridEntry[commonProperties.Count];
                        for (int i = 0; i < entries.Length; i++)
                        {
                            entries[i] = new MultiPropertyDescriptorGridEntry(
                                parentEntry.OwnerGrid,
                                parentEntry,
                                objects!, // all elements are not null if commonProperties is not null
                                commonProperties[i],
                                hide: false);
                        }

                        result = SortParenEntries(entries);
                    }
                }
                else
                {
                    List<PropertyDescriptor[]>? properties = GetCommonProperties(objects.AsSpan(1), presort: true, tab, parentEntry);

                    // This will work for just one as well.
                    List<PropertyDescriptor[]>? firstProperties = GetCommonProperties(objects.AsSpan(0, 1), presort: false, tab, parentEntry);

                    if (properties is not null && firstProperties is not null)
                    {
                        var firstPropertyDescriptors = new PropertyDescriptor[firstProperties.Count];
                        for (int i = 0; i < firstProperties.Count; i++)
                        {
                            firstPropertyDescriptors[i] = firstProperties[i][0];
                        }

                        properties = UnsortedMerge(firstPropertyDescriptors, properties);

                        var entries = new MultiPropertyDescriptorGridEntry[properties.Count];

                        for (int i = 0; i < entries.Length; i++)
                        {
                            entries[i] = new MultiPropertyDescriptorGridEntry(
                                parentEntry.OwnerGrid,
                                parentEntry,
                                objects!, // all elements are not null if properties and firstProperties are not null
                                properties[i],
                                hide: false);
                        }

                        result = SortParenEntries(entries);
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
            }

            return result;
        }

        /// <summary>
        ///  Returns a list of <see cref="PropertyDescriptor"/> arrays, one for each component.
        /// </summary>
        private static List<PropertyDescriptor[]>? GetCommonProperties(
            ReadOnlySpan<object?> objects,
            bool presort,
            PropertyTab tab,
            GridEntry parentEntry)
        {
            var objectProperties = new PropertyDescriptorCollection[objects.Length];
            Attribute[]? attributes = null;
            if (parentEntry.BrowsableAttributes is not null)
            {
                attributes = new Attribute[parentEntry.BrowsableAttributes.Count];
                parentEntry.BrowsableAttributes.CopyTo(attributes, 0);
            }

            for (int i = 0; i < objects.Length; i++)
            {
                object? obj = objects[i];
                if (obj is null)
                {
                    return null;
                }

                PropertyDescriptorCollection? properties = tab.GetProperties(parentEntry, obj, attributes);
                if (properties is null)
                {
                    return null;
                }

                if (presort)
                {
                    properties = properties.Sort(s_propertyComparer);
                }

                objectProperties[i] = properties;
            }

            List<PropertyDescriptor[]> mergedList = [];
            var matchArray = new PropertyDescriptor[objects.Length];

            //
            // Merge the property descriptors
            //

            int[] positions = new int[objectProperties.Length];

            // Iterate through the first object's properties to see if it has matches in the other objects.
            for (int i = 0; i < objectProperties[0].Count; i++)
            {
                PropertyDescriptor pivotProperty = objectProperties[0][i];

                bool match = pivotProperty.GetAttribute<MergablePropertyAttribute>()?.IsDefaultAttribute() ?? false;

                for (int j = 1; match && j < objectProperties.Length; j++)
                {
                    if (positions[j] >= objectProperties[j].Count)
                    {
                        match = false;
                        break;
                    }

                    // Check to see if we're on a match.
                    PropertyDescriptor property = objectProperties[j][positions[j]];
                    if (pivotProperty.Equals(property))
                    {
                        positions[j] += 1;

                        if (property.GetAttribute<MergablePropertyAttribute>()?.IsDefaultAttribute() ?? false)
                        {
                            matchArray[j] = property;
                            continue;
                        }

                        match = false;
                        break;
                    }

                    int position = positions[j];
                    property = objectProperties[j][position];

                    match = false;

                    // If we aren't on a match, check all the items until we're past where the matching item would be.
                    while (s_propertyComparer.Compare(property, pivotProperty) <= 0)
                    {
                        // Got a match!
                        if (pivotProperty.Equals(property))
                        {
                            match = property.GetAttribute<MergablePropertyAttribute>()?.IsDefaultAttribute() ?? false;
                            if (!match)
                            {
                                position++;
                            }
                            else
                            {
                                matchArray[j] = property;
                                positions[j] = position + 1;
                            }

                            break;
                        }

                        // Try again.
                        position++;
                        if (position < objectProperties[j].Count)
                        {
                            property = objectProperties[j][position];
                        }
                        else
                        {
                            break;
                        }
                    }

                    // If we got here, there is no match, quit for this one.
                    if (!match)
                    {
                        positions[j] = position;
                        break;
                    }
                }

                // Do we have a match?
                if (match)
                {
                    matchArray[0] = pivotProperty;
                    mergedList.Add((PropertyDescriptor[])matchArray.Clone());
                }
            }

            return mergedList;
        }

        private static MultiPropertyDescriptorGridEntry[] SortParenEntries(MultiPropertyDescriptorGridEntry[] entries)
        {
            MultiPropertyDescriptorGridEntry[]? newEntries = null;
            int newPosition = 0;

            // First scan the list and move any parenthesized properties to the front.
            for (int i = 0; i < entries.Length; i++)
            {
                if (entries[i].ParensAroundName)
                {
                    newEntries ??= new MultiPropertyDescriptorGridEntry[entries.Length];

                    newEntries[newPosition++] = entries[i];
                    entries[i] = null!; // we're using this for the second pass below
                }
            }

            // Second pass, copy any that didn't have the parens.
            if (newEntries is not null)
            {
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i] is not null)
                    {
                        newEntries[newPosition++] = entries[i];
                    }
                }

                return newEntries;
            }

            return entries;
        }

        /// <summary>
        ///  Merges an unsorted array of grid entries with a sorted array of grid entries that
        ///  have already been merged. The resulting array is the intersection of entries between the two,
        ///  but in the order of <paramref name="baseEntries"/>.
        /// </summary>
        private static List<PropertyDescriptor[]> UnsortedMerge(
            PropertyDescriptor[] baseEntries,
            List<PropertyDescriptor[]> sortedMergedEntries)
        {
            List<PropertyDescriptor[]> mergedEntries = [];

            foreach (PropertyDescriptor basePropertyDescriptor in baseEntries)
            {
                // First do a binary search for a matching item.
                string entryName = $"{basePropertyDescriptor.Name} {basePropertyDescriptor.PropertyType.FullName}";

                int length = sortedMergedEntries.Count;

                // Perform a binary search.
                int offset = length / 2;
                int start = 0;

                while (length > 0)
                {
                    var propertyDescriptors = sortedMergedEntries[start + offset];
                    PropertyDescriptor propertyDescriptor = propertyDescriptors[0];
                    string sortString = $"{propertyDescriptor.Name} {propertyDescriptor.PropertyType.FullName}";
                    int result = string.Compare(entryName, sortString, ignoreCase: false, CultureInfo.InvariantCulture);
                    if (result == 0)
                    {
                        mergedEntries.Add([basePropertyDescriptor, .. propertyDescriptors]);
                        break;
                    }
                    else if (result < 0)
                    {
                        length = offset;
                    }
                    else
                    {
                        int delta = offset + 1;
                        start += delta;
                        length -= delta;
                    }

                    offset = length / 2;
                }
            }

            return mergedEntries;
        }
    }
}
