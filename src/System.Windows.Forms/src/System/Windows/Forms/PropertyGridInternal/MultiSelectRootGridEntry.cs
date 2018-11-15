// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Collections;
    using System.Reflection;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Globalization;

    internal class MultiSelectRootGridEntry : SingleSelectRootGridEntry {

        private static PDComparer PropertyComparer = new PDComparer();

 
        internal MultiSelectRootGridEntry(PropertyGridView view, object obj, IServiceProvider baseProvider, IDesignerHost host, PropertyTab tab, PropertySort sortType)
        : base(view,obj, baseProvider, host, tab, sortType) {
        }

        internal override bool ForceReadOnly {
            get {
                if (!forceReadOnlyChecked) {
                    bool anyRO = false;
                    foreach (object obj in (Array)objValue) {
                        ReadOnlyAttribute readOnlyAttr = (ReadOnlyAttribute)TypeDescriptor.GetAttributes(obj)[typeof(ReadOnlyAttribute)];
                        if ((readOnlyAttr != null && !readOnlyAttr.IsDefaultAttribute()) || TypeDescriptor.GetAttributes(obj).Contains(InheritanceAttribute.InheritedReadOnly)) {
                            anyRO = true;
                            break;
                        }
                    }
                    if (anyRO) {
                        flags |= FLAG_FORCE_READONLY;
                    }
                    forceReadOnlyChecked = true;
                }
                return base.ForceReadOnly;
            }
        }

        protected override bool CreateChildren() {
             return CreateChildren(false);
        }

        protected override bool CreateChildren(bool diffOldChildren) {
            try {
                object[] rgobjs = (object[])objValue;
                

                ChildCollection.Clear();

                MultiPropertyDescriptorGridEntry[] mergedProps = PropertyMerger.GetMergedProperties(rgobjs, this, this.PropertySort, CurrentTab);
                
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose && mergedProps == null, "PropertyGridView: MergedProps returned null!");

                if (mergedProps != null) {
                    ChildCollection.AddRange(mergedProps);
                }
                bool fExpandable = this.Children.Count > 0;
                if (!fExpandable) {
                    //Debug.WriteLine("Object " + rgobjs[0].GetType().FullName + " is not expandable because it has no children");
                    SetFlag(GridEntry.FL_EXPANDABLE_FAILED,true);
                }
                CategorizePropEntries();
                return fExpandable;
            }
            catch {
                return false;
            }
        }

       
        internal static class PropertyMerger {

             public static MultiPropertyDescriptorGridEntry[] GetMergedProperties(object[] rgobjs, GridEntry parentEntry, PropertySort sort, PropertyTab tab) {

                MultiPropertyDescriptorGridEntry[] result = null;
                try {
                    int cLength = rgobjs.Length;
                    object[] rgobjArgs = new object[1];
                    
                    if((sort & PropertySort.Alphabetical) != 0) {
                        ArrayList props = GetCommonProperties(rgobjs, true, tab, parentEntry);
                        
                        MultiPropertyDescriptorGridEntry[] entries = new MultiPropertyDescriptorGridEntry[props.Count];
                        for (int i=0; i < entries.Length; i++) {
                            entries[i] = new MultiPropertyDescriptorGridEntry(parentEntry.OwnerGrid, parentEntry, rgobjs, (PropertyDescriptor[])props[i], false);
                        }
    
                        result = SortParenEntries(entries);
                    }
                    else {
                        object[] sortObjs = new object[cLength - 1];
                        Array.Copy(rgobjs, 1, sortObjs, 0, cLength - 1);
    
                        ArrayList props = GetCommonProperties(sortObjs, true, tab, parentEntry);
    
                        // this'll work for just one as well
                        ArrayList firstProps = GetCommonProperties(new object[]{rgobjs[0]}, false, tab, parentEntry);
    
                        PropertyDescriptor[] firstPds = new PropertyDescriptor[firstProps.Count];
                        for (int i = 0; i < firstProps.Count; i++) {
                            firstPds[i] = ((PropertyDescriptor[])firstProps[i])[0];
                        }
                        
                        props = UnsortedMerge(firstPds, props);
                        
                        MultiPropertyDescriptorGridEntry[] entries = new MultiPropertyDescriptorGridEntry[props.Count];
                        
                        for (int i=0; i < entries.Length; i++) {
                            entries[i] = new MultiPropertyDescriptorGridEntry(parentEntry.OwnerGrid, parentEntry, rgobjs, (PropertyDescriptor[])props[i], false);
                        }
    
                        result = SortParenEntries(entries);
                    }
                }
                catch {
                }
    
                return result;
    
            }

            // this returns an array list of the propertydescriptor arrays, one for each
            // component
            //
            private static ArrayList GetCommonProperties(object[] objs, bool presort, PropertyTab tab, GridEntry parentEntry) {
                PropertyDescriptorCollection[] propCollections = new PropertyDescriptorCollection[objs.Length];
    
                Attribute[] attrs = new Attribute[parentEntry.BrowsableAttributes.Count];
                parentEntry.BrowsableAttributes.CopyTo(attrs, 0);
                
                for (int i = 0; i < objs.Length; i++) {
                    PropertyDescriptorCollection pdc = tab.GetProperties(parentEntry, objs[i], attrs);
                    if (presort) {
                        pdc = pdc.Sort(PropertyComparer);
                    }
                    propCollections[i] = pdc;
                }
                
                ArrayList mergedList = new ArrayList();
                PropertyDescriptor[] matchArray = new PropertyDescriptor[objs.Length];
    
               // 
               // Merge the property descriptors
               //
               int[] posArray = new int[propCollections.Length];
               for (int i = 0; i < propCollections[0].Count; i++) {
                   PropertyDescriptor pivotDesc = propCollections[0][i];
                   
                   bool match = pivotDesc.Attributes[typeof(MergablePropertyAttribute)].IsDefaultAttribute();
                        
                   for (int j = 1; match && j < propCollections.Length; j++) {
        
                       if (posArray[j] >= propCollections[j].Count) {
                           match = false;
                           break;
                       }
        
                       // check to see if we're on a match
                       //
                       PropertyDescriptor jProp = propCollections[j][posArray[j]];
                       if (pivotDesc.Equals(jProp)) {
                           posArray[j] += 1;
                           
                           if (!jProp.Attributes[typeof(MergablePropertyAttribute)].IsDefaultAttribute()) {
                                match = false;
                                break;
                           }
                           matchArray[j] = jProp;
                           continue;
                       }
                       
                       int jPos = posArray[j];
                       jProp = propCollections[j][jPos];
    
                       match = false;
        
                       // if we aren't on a match, check all the items until we're past
                       // where the matching item would be
                       while (PropertyComparer.Compare(jProp, pivotDesc) <= 0) {
        
                           // got a match!
                           if (pivotDesc.Equals(jProp)) {
    
                               if (!jProp.Attributes[typeof(MergablePropertyAttribute)].IsDefaultAttribute()) {
                                  match = false;
                                  jPos++;
                               }
                               else {
                                  match = true;
                                  matchArray[j] = jProp;
                                  posArray[j] = jPos + 1;
                               }
                               break;
                           }
        
                           // try again
                           jPos++;
                           if (jPos < propCollections[j].Count) {
                               jProp = propCollections[j][jPos];
                           }
                           else {
                               break;
                           }
                       }
        
                       // if we got here, there is no match, quit for this guy
                       if (!match) {
                            posArray[j] = jPos;
                            break;
                       }
                   }
        
                   // do we have a match?
                   if (match) {
                       matchArray[0] = pivotDesc;
                       mergedList.Add(matchArray.Clone());
                   }
               }
    
               return mergedList;
            }



            private static MultiPropertyDescriptorGridEntry[] SortParenEntries(MultiPropertyDescriptorGridEntry[] entries) {

                MultiPropertyDescriptorGridEntry[] newEntries = null;
                int newPos = 0;
                
                // first scan the list and move any parentesized properties to the front.
                for (int i = 0; i < entries.Length; i++) {
                    if (entries[i].ParensAroundName) {
                        if (newEntries == null) {
                            newEntries = new MultiPropertyDescriptorGridEntry[entries.Length];
                        }
                        newEntries[newPos++] = entries[i];
                        entries[i] = null;
                    }
                }
    
                // second pass, copy any that didn't have the parens.
                if (newPos > 0) {
                    for (int i = 0; i < entries.Length; i++) {
                        if (entries[i] != null) {
                            newEntries[newPos++] = entries[i];
                        }
                    } 
                    entries = newEntries;
                }
                return entries;
            }

            /// <include file='doc\MultiSelectRootGridEntry.uex' path='docs/doc[@for="MultiSelectRootGridEntry.UnsortedMerge"]/*' />
            /// <devdoc>
            /// merges an unsorted array of grid entries with a sorted array of grid entries that
            /// have already been merged.  The resulting array is the intersection of entries between the two,
            /// but in the order of baseEntries.
            /// </devdoc>
            private static ArrayList UnsortedMerge(PropertyDescriptor[] baseEntries, ArrayList sortedMergedEntries) {
                
                ArrayList mergedEntries = new ArrayList();
                PropertyDescriptor[] mergeArray = new PropertyDescriptor[((PropertyDescriptor[])sortedMergedEntries[0]).Length + 1];
                
                for (int i = 0; i < baseEntries.Length; i++) {
    
                    PropertyDescriptor basePd = baseEntries[i];
    
                    // first, do a binary search for a matching item
                    PropertyDescriptor[] mergedEntryList = null;
                    string entryName = basePd.Name + " " + basePd.PropertyType.FullName;
    
                    int len = sortedMergedEntries.Count;
                    // perform a binary search
                    int offset = len / 2;
                    int start = 0;
                    
                    while (len > 0) {
                        PropertyDescriptor[] pdList = (PropertyDescriptor[])sortedMergedEntries[start + offset];
                        PropertyDescriptor pd = pdList[0];
                        string sortString = pd.Name + " " + pd.PropertyType.FullName;
                        int result = String.Compare(entryName, sortString, false, CultureInfo.InvariantCulture);
                        if (result == 0) {
                            mergedEntryList = pdList;
                            break;
                        }
                        else if (result < 0) {
                            len = offset;
                        }
                        else {
                            int delta = offset + 1;
                            start += delta;
                            len -= delta;
                        }
                        offset = len / 2;
                    }
                    
                    if (mergedEntryList != null) {
                        mergeArray[0] = basePd;
                        Array.Copy(mergedEntryList, 0, mergeArray, 1,  mergedEntryList.Length);
                        mergedEntries.Add(mergeArray.Clone());
                    }
                }
                return mergedEntries;
            }

        }
        
        private class PDComparer : IComparer {
            public int Compare(object obj1, object obj2) {
                PropertyDescriptor a1 = obj1 as PropertyDescriptor;
                PropertyDescriptor a2 = obj2 as PropertyDescriptor;
            
                if (a1 == null && a2 == null) {
                    return 0;
                }
                else if (a1 == null) {
                    return -1;
                }
                else if (a2 == null) {
                    return 1;
                }

                int result = String.Compare(a1.Name, a2.Name, false, CultureInfo.InvariantCulture);

                if (result == 0) {
                    // 
                    result = String.Compare(a1.PropertyType.FullName, a2.PropertyType.FullName, true, System.Globalization.CultureInfo.CurrentCulture);
                }
                return result;
            }
        }
     }
}
