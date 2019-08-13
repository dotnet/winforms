// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        /// <summary>
        ///  The accessible object class for a PropertyGridView. The child accessible objects
        ///  are accessible objects corresponding to the property grid entries.
        /// </summary>
        [ComVisible(true)]
        internal class PropertyGridViewAccessibleObject : ControlAccessibleObject
        {
            private readonly PropertyGridView _owningPropertyGridView;
            private readonly PropertyGrid _parentPropertyGrid;

            /// <summary>
            ///  Construct a PropertyGridViewAccessibleObject
            /// </summary>
            public PropertyGridViewAccessibleObject(PropertyGridView owner, PropertyGrid parentPropertyGrid) : base(owner)
            {
                _owningPropertyGridView = owner;
                _parentPropertyGrid = parentPropertyGrid;
            }

            /// <summary>
            ///  Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </summary>
            /// <param name="x">x coordinate of point to check</param>
            /// <param name="y">y coordinate of point to check</param>
            /// <returns>Return the child element at the specified point, if one exists,
            ///  otherwise return this element if the point is on this element,
            ///  otherwise return null.
            /// </returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                return HitTest((int)x, (int)y);
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                if (_parentPropertyGrid.AccessibilityObject is PropertyGridAccessibleObject propertyGridAccessibleObject)
                {
                    UnsafeNativeMethods.IRawElementProviderFragment navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                    if (navigationTarget != null)
                    {
                        return navigationTarget;
                    }
                }

                if (_owningPropertyGridView.OwnerGrid.SortedByCategories)
                {
                    switch (direction)
                    {
                        case UnsafeNativeMethods.NavigateDirection.FirstChild:
                            return GetFirstCategory();
                        case UnsafeNativeMethods.NavigateDirection.LastChild:
                            return GetLastCategory();
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case UnsafeNativeMethods.NavigateDirection.FirstChild:
                            return GetChild(0);
                        case UnsafeNativeMethods.NavigateDirection.LastChild:
                            int childCount = GetChildCount();
                            if (childCount > 0)
                            {
                                return GetChild(childCount - 1);
                            }

                            return null;
                    }
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _owningPropertyGridView.OwnerGrid.AccessibilityObject;
                }
            }

            /// <summary>
            ///  Gets the accessible object for the currently focused grid entry.
            /// </summary>
            /// <returns>The accessible object for the currently focused grid entry.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment GetFocus()
            {
                return GetFocused();
            }

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyId">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_TableControlTypeId;
                }
                else if (propertyID == NativeMethods.UIA_NamePropertyId)
                {
                    return Name;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }
                    else
                    {
                        return SR.PropertyGridDefaultAccessibleName;
                    }
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    else
                    {
                        return AccessibleRole.Table;
                    }
                }
            }

            public AccessibleObject Next(GridEntry current)
            {
                int row = ((PropertyGridView)Owner).GetRowFromGridEntry(current);
                GridEntry nextEntry = ((PropertyGridView)Owner).GetGridEntryFromRow(++row);
                if (nextEntry != null)
                {
                    return nextEntry.AccessibilityObject;
                }
                return null;
            }

            internal AccessibleObject GetCategory(int categoryIndex)
            {
                GridEntry[] targetEntries = new GridEntry[1];
                GridEntryCollection topLevelGridEntries = _owningPropertyGridView.TopLevelGridEntries;
                var topLevelGridEntriesCount = topLevelGridEntries.Count;
                if (topLevelGridEntriesCount > 0)
                {
                    GridItem targetEntry = topLevelGridEntries[categoryIndex];
                    if (targetEntry is CategoryGridEntry categoryGridEntry)
                    {
                        return categoryGridEntry.AccessibilityObject;
                    }
                }

                return null;
            }

            internal AccessibleObject GetFirstCategory()
            {
                return GetCategory(0);
            }

            internal AccessibleObject GetLastCategory()
            {
                GridEntryCollection topLevelGridEntries = _owningPropertyGridView.TopLevelGridEntries;
                var topLevelGridEntriesCount = topLevelGridEntries.Count;
                return GetCategory(topLevelGridEntries.Count - 1);
            }

            /// <summary>
            ///  Gets the previous grid entry accessibility object.
            /// </summary>
            /// <param name="currentGridEntry">The current grid entry.</param>
            /// <param name="gridEntryCollection">The grid entry collection.</param>
            /// <param name="currentGridEntryFound">Indicates whether the current grid entry is found.</param>
            /// <returns>The previous grid entry.</returns>
            internal AccessibleObject GetPreviousGridEntry(GridEntry currentGridEntry, GridEntryCollection gridEntryCollection, out bool currentGridEntryFound)
            {
                GridEntry previousGridEntry = null;
                currentGridEntryFound = false;

                foreach (GridEntry gridEntry in gridEntryCollection)
                {
                    if (currentGridEntry == gridEntry)
                    {
                        // Set to true to return the previous iterable element.
                        currentGridEntryFound = true;
                        if (previousGridEntry != null)
                        {
                            // In the current iteration return previous entry if the current entry == iterated grid entry.
                            return previousGridEntry.AccessibilityObject;
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        previousGridEntry = gridEntry;
                        if (gridEntry.ChildCount > 0)
                        {
                            AccessibleObject foundChild = GetPreviousGridEntry(currentGridEntry, gridEntry.Children, out currentGridEntryFound);
                            if (foundChild != null)
                            {
                                // Return some down-level child if found.
                                return foundChild;
                            }
                            else if (currentGridEntryFound)
                            {
                                // If the passed current is found but there is no next near this current.
                                return null;
                            }
                        }
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the next grid entry.
            /// </summary>
            /// <param name="currentGridEntry">The current grid entry.</param>
            /// <param name="gridEntryCollection">The grid entry collection.</param>
            /// <param name="currentGridEntryFound">Indicates whether the current grid entry is found.</param>
            /// <returns>The next grid entry.</returns>
            internal AccessibleObject GetNextGridEntry(GridEntry currentGridEntry, GridEntryCollection gridEntryCollection, out bool currentGridEntryFound)
            {
                currentGridEntryFound = false;

                foreach (GridEntry gridEntry in gridEntryCollection)
                {
                    if (currentGridEntryFound)
                    {
                        // Return the next entry via IEnumerable.Next() if previous entry == passed current.
                        return gridEntry.AccessibilityObject;
                    }

                    if (currentGridEntry == gridEntry)
                    {
                        // Set to true to return the next iterable element. (see above)
                        currentGridEntryFound = true;
                    }
                    else if (gridEntry.ChildCount > 0)
                    {
                        AccessibleObject foundChild = GetNextGridEntry(currentGridEntry, gridEntry.Children, out currentGridEntryFound);
                        if (foundChild != null)
                        {
                            // Return some down-level child if found.
                            return foundChild;
                        }
                        else if (currentGridEntryFound)
                        {
                            // If the passed current is found but there is no next near this current.
                            return null;
                        }
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the first child property.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The first child property.</returns>
            internal AccessibleObject GetFirstChildProperty(CategoryGridEntry current)
            {
                if (current.ChildCount > 0)
                {
                    GridEntryCollection subGridEntry = current.Children;
                    if (subGridEntry != null && subGridEntry.Count > 0)
                    {
                        GridEntry[] targetEntries = new GridEntry[1];
                        try
                        {
                            _owningPropertyGridView.GetGridEntriesFromOutline(subGridEntry, 0, 0, targetEntries);
                        }
                        catch (Exception ex)
                        {
                            Debug.Fail(ex.ToString());
                        }

                        return targetEntries[0].AccessibilityObject;
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the last child property.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The last child property.</returns>
            internal AccessibleObject GetLastChildProperty(CategoryGridEntry current)
            {
                if (current.ChildCount > 0)
                {
                    GridEntryCollection subGridEntry = current.Children;
                    if (subGridEntry != null && subGridEntry.Count > 0)
                    {
                        GridEntry[] targetEntries = new GridEntry[1];
                        try
                        {
                            _owningPropertyGridView.GetGridEntriesFromOutline(subGridEntry, 0, subGridEntry.Count - 1, targetEntries);
                        }
                        catch (Exception ex)
                        {
                            Debug.Fail(ex.ToString());
                        }

                        return targetEntries[0].AccessibilityObject;
                    }
                }

                return null;
            }

            /// <summary>
            ///  Gets the next category.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The next category.</returns>
            internal AccessibleObject GetNextCategory(CategoryGridEntry current)
            {
                int row = _owningPropertyGridView.GetRowFromGridEntry(current);

                GridEntry nextEntry;

                do
                {
                    nextEntry = _owningPropertyGridView.GetGridEntryFromRow(++row);
                    if (nextEntry is CategoryGridEntry)
                    {
                        return nextEntry.AccessibilityObject;
                    }
                }
                while (nextEntry != null);

                return null;
            }

            public AccessibleObject Previous(GridEntry current)
            {
                int row = ((PropertyGridView)Owner).GetRowFromGridEntry(current);
                GridEntry prevEntry = ((PropertyGridView)Owner).GetGridEntryFromRow(--row);
                if (prevEntry != null)
                {
                    return prevEntry.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  Gets the previous category.
            /// </summary>
            /// <param name="current">The current grid entry.</param>
            /// <returns>The previous category.</returns>
            internal AccessibleObject GetPreviousCategory(CategoryGridEntry current)
            {
                int row = _owningPropertyGridView.GetRowFromGridEntry(current);

                GridEntry previousEntry;

                do
                {
                    previousEntry = _owningPropertyGridView.GetGridEntryFromRow(--row);
                    if (previousEntry is CategoryGridEntry)
                    {
                        return previousEntry.AccessibilityObject;
                    }
                }
                while (previousEntry != null);

                return null;
            }

            /// <summary>
            ///  Get the accessible child at the given index.
            ///  The accessible children of a PropertyGridView are accessible objects
            ///  corresponding to the property grid entries.
            /// </summary>
            public override AccessibleObject GetChild(int index)
            {

                GridEntryCollection properties = ((PropertyGridView)Owner).AccessibilityGetGridEntries();
                if (properties != null && index >= 0 && index < properties.Count)
                {
                    return properties.GetEntry(index).AccessibilityObject;
                }
                else
                {
                    return null;
                }
            }

            /// <summary>
            ///  Get the number of accessible children.
            ///  The accessible children of a PropertyGridView are accessible objects
            ///  corresponding to the property grid entries.
            /// </summary>
            public override int GetChildCount()
            {
                GridEntryCollection properties = ((PropertyGridView)Owner).AccessibilityGetGridEntries();

                if (properties != null)
                {
                    return properties.Count;
                }
                else
                {
                    return 0;
                }
            }

            /// <summary>
            ///  Get the accessible object for the currently focused grid entry.
            /// </summary>
            public override AccessibleObject GetFocused()
            {

                GridEntry gridEntry = ((PropertyGridView)Owner).SelectedGridEntry;
                if (gridEntry != null && gridEntry.Focus)
                {
                    return gridEntry.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  Get the accessible object for the currently selected grid entry.
            /// </summary>
            public override AccessibleObject GetSelected()
            {
                GridEntry gridEntry = ((PropertyGridView)Owner).SelectedGridEntry;
                if (gridEntry != null)
                {
                    return gridEntry.AccessibilityObject;
                }
                return null;
            }

            /// <summary>
            ///  Get the accessible child at the given screen location.
            ///  The accessible children of a PropertyGridView are accessible objects
            ///  corresponding to the property grid entries.
            /// </summary>
            public override AccessibleObject HitTest(int x, int y)
            {
                // Convert to client coordinates
                var pt = new Point(x, y);
                UnsafeNativeMethods.ScreenToClient(new HandleRef(Owner, Owner.Handle), ref pt);

                // Find the grid entry at the given client coordinates
                //
                Point pos = ((PropertyGridView)Owner).FindPosition(pt.X, pt.Y);
                if (pos != PropertyGridView.InvalidPosition)
                {
                    GridEntry gridEntry = ((PropertyGridView)Owner).GetGridEntryFromRow(pos.Y);
                    if (gridEntry != null)
                    {

                        // Return the accessible object for this grid entry
                        //
                        return gridEntry.AccessibilityObject;
                    }
                }

                // No grid entry at this point
                //
                return null;
            }

            /// <summary>
            ///  Navigate to another object.
            /// </summary>
            public override AccessibleObject Navigate(AccessibleNavigation navdir)
            {

                if (GetChildCount() > 0)
                {
                    // We're only handling FirstChild and LastChild here
                    switch (navdir)
                    {
                        case AccessibleNavigation.FirstChild:
                            return GetChild(0);
                        case AccessibleNavigation.LastChild:
                            return GetChild(GetChildCount() - 1);
                    }
                }
                return null;    // Perform default behavior
            }
        }
    }
}
