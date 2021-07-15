// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms.Design;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class MultiSelectRootGridEntry
    {
        internal static class PropertyMerger
        {
            public static MultiPropertyDescriptorGridEntry[] GetMergedProperties(object[] rgobjs, GridEntry parentEntry, PropertySort sort, PropertyTab tab)
            {
                MultiPropertyDescriptorGridEntry[] result = null;
                try
                {
                    int cLength = rgobjs.Length;
                    object[] rgobjArgs = new object[1];

                    if ((sort & PropertySort.Alphabetical) != 0)
                    {
                        ArrayList props = GetCommonProperties(rgobjs, true, tab, parentEntry);

                        var entries = new MultiPropertyDescriptorGridEntry[props.Count];
                        for (int i = 0; i < entries.Length; i++)
                        {
                            entries[i] = new MultiPropertyDescriptorGridEntry(parentEntry.OwnerGrid, parentEntry, rgobjs, (PropertyDescriptor[])props[i], false);
                        }

                        result = SortParenEntries(entries);
                    }
                    else
                    {
                        object[] sortObjs = new object[cLength - 1];
                        Array.Copy(rgobjs, 1, sortObjs, 0, cLength - 1);

                        ArrayList props = GetCommonProperties(sortObjs, true, tab, parentEntry);

                        // this'll work for just one as well
                        ArrayList firstProps = GetCommonProperties(new object[] { rgobjs[0] }, false, tab, parentEntry);

                        var firstPds = new PropertyDescriptor[firstProps.Count];
                        for (int i = 0; i < firstProps.Count; i++)
                        {
                            firstPds[i] = ((PropertyDescriptor[])firstProps[i])[0];
                        }

                        props = UnsortedMerge(firstPds, props);

                        var entries = new MultiPropertyDescriptorGridEntry[props.Count];

                        for (int i = 0; i < entries.Length; i++)
                        {
                            entries[i] = new MultiPropertyDescriptorGridEntry(parentEntry.OwnerGrid, parentEntry, rgobjs, (PropertyDescriptor[])props[i], false);
                        }

                        result = SortParenEntries(entries);
                    }
                }
                catch
                {
                }

                return result;
            }

            /// <summary>
            ///  Returns an array list of the PropertyDescriptor arrays, one for each component.
            /// </summary>
            private static ArrayList GetCommonProperties(object[] objs, bool presort, PropertyTab tab, GridEntry parentEntry)
            {
                var propCollections = new PropertyDescriptorCollection[objs.Length];
                var attrs = new Attribute[parentEntry.BrowsableAttributes.Count];

                parentEntry.BrowsableAttributes.CopyTo(attrs, 0);

                for (int i = 0; i < objs.Length; i++)
                {
                    PropertyDescriptorCollection pdc = tab.GetProperties(parentEntry, objs[i], attrs);
                    if (presort)
                    {
                        pdc = pdc.Sort(s_propertyComparer);
                    }

                    propCollections[i] = pdc;
                }

                ArrayList mergedList = new();
                var matchArray = new PropertyDescriptor[objs.Length];

                //
                // Merge the property descriptors
                //

                int[] posArray = new int[propCollections.Length];
                for (int i = 0; i < propCollections[0].Count; i++)
                {
                    PropertyDescriptor pivotDesc = propCollections[0][i];

                    bool match = pivotDesc.Attributes[typeof(MergablePropertyAttribute)].IsDefaultAttribute();

                    for (int j = 1; match && j < propCollections.Length; j++)
                    {
                        if (posArray[j] >= propCollections[j].Count)
                        {
                            match = false;
                            break;
                        }

                        // Check to see if we're on a match.
                        PropertyDescriptor jProp = propCollections[j][posArray[j]];
                        if (pivotDesc.Equals(jProp))
                        {
                            posArray[j] += 1;

                            if (!jProp.Attributes[typeof(MergablePropertyAttribute)].IsDefaultAttribute())
                            {
                                match = false;
                                break;
                            }

                            matchArray[j] = jProp;
                            continue;
                        }

                        int jPos = posArray[j];
                        jProp = propCollections[j][jPos];

                        match = false;

                        // If we aren't on a match, check all the items until we're past where the matching item would be.
                        while (s_propertyComparer.Compare(jProp, pivotDesc) <= 0)
                        {
                            // Got a match!
                            if (pivotDesc.Equals(jProp))
                            {
                                if (!jProp.Attributes[typeof(MergablePropertyAttribute)].IsDefaultAttribute())
                                {
                                    match = false;
                                    jPos++;
                                }
                                else
                                {
                                    match = true;
                                    matchArray[j] = jProp;
                                    posArray[j] = jPos + 1;
                                }

                                break;
                            }

                            // Try again.
                            jPos++;
                            if (jPos < propCollections[j].Count)
                            {
                                jProp = propCollections[j][jPos];
                            }
                            else
                            {
                                break;
                            }
                        }

                        // If we got here, there is no match, quit for this one.
                        if (!match)
                        {
                            posArray[j] = jPos;
                            break;
                        }
                    }

                    // Do we have a match?
                    if (match)
                    {
                        matchArray[0] = pivotDesc;
                        mergedList.Add(matchArray.Clone());
                    }
                }

                return mergedList;
            }

            private static MultiPropertyDescriptorGridEntry[] SortParenEntries(MultiPropertyDescriptorGridEntry[] entries)
            {
                MultiPropertyDescriptorGridEntry[] newEntries = null;
                int newPos = 0;

                // First scan the list and move any parenthesized properties to the front.
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i].ParensAroundName)
                    {
                        if (newEntries is null)
                        {
                            newEntries = new MultiPropertyDescriptorGridEntry[entries.Length];
                        }

                        newEntries[newPos++] = entries[i];
                        entries[i] = null;
                    }
                }

                // Second pass, copy any that didn't have the parens.
                if (newPos > 0)
                {
                    for (int i = 0; i < entries.Length; i++)
                    {
                        if (entries[i] is not null)
                        {
                            newEntries[newPos++] = entries[i];
                        }
                    }

                    entries = newEntries;
                }

                return entries;
            }

            /// <summary>
            ///  Merges an unsorted array of grid entries with a sorted array of grid entries that
            ///  have already been merged.  The resulting array is the intersection of entries between the two,
            ///  but in the order of baseEntries.
            /// </summary>
            private static ArrayList UnsortedMerge(PropertyDescriptor[] baseEntries, ArrayList sortedMergedEntries)
            {
                ArrayList mergedEntries = new();
                var mergeArray = new PropertyDescriptor[((PropertyDescriptor[])sortedMergedEntries[0]).Length + 1];

                for (int i = 0; i < baseEntries.Length; i++)
                {
                    PropertyDescriptor basePd = baseEntries[i];

                    // First, do a binary search for a matching item.
                    PropertyDescriptor[] mergedEntryList = null;
                    string entryName = $"{basePd.Name} {basePd.PropertyType.FullName}";

                    int len = sortedMergedEntries.Count;

                    // Perform a binary search.
                    int offset = len / 2;
                    int start = 0;

                    while (len > 0)
                    {
                        var pdList = (PropertyDescriptor[])sortedMergedEntries[start + offset];
                        PropertyDescriptor pd = pdList[0];
                        string sortString = pd.Name + " " + pd.PropertyType.FullName;
                        int result = string.Compare(entryName, sortString, false, CultureInfo.InvariantCulture);
                        if (result == 0)
                        {
                            mergedEntryList = pdList;
                            break;
                        }
                        else if (result < 0)
                        {
                            len = offset;
                        }
                        else
                        {
                            int delta = offset + 1;
                            start += delta;
                            len -= delta;
                        }

                        offset = len / 2;
                    }

                    if (mergedEntryList is not null)
                    {
                        mergeArray[0] = basePd;
                        Array.Copy(mergedEntryList, 0, mergeArray, 1, mergedEntryList.Length);
                        mergedEntries.Add(mergeArray.Clone());
                    }
                }

                return mergedEntries;
            }
        }
    }
}
