// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class PropertyGridView
{
    /// <summary>
    ///  The accessible object class for a PropertyGridView. The child accessible objects
    ///  are accessible objects corresponding to the property grid entries.
    /// </summary>
    internal class PropertyGridViewAccessibleObject : ControlAccessibleObject
    {
        private readonly WeakReference<PropertyGrid> _parentPropertyGrid;

        public PropertyGridViewAccessibleObject(PropertyGridView owner, PropertyGrid parentPropertyGrid) : base(owner)
        {
            _parentPropertyGrid = new(parentPropertyGrid);
        }

        internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            => this.IsOwnerHandleCreated(out Control? _) ? HitTest((int)x, (int)y) : null;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!_parentPropertyGrid.TryGetTarget(out PropertyGrid? target))
            {
                return null;
            }

            if (target.IsHandleCreated
                // Created is set to false in WM_DESTROY, but the window Handle is released on NCDESTROY, which comes after DESTROY.
                // But between these calls, AccessibleObject can be recreated and might cause memory leaks.
                && target.Created
                && target.AccessibilityObject is PropertyGrid.PropertyGridAccessibleObject propertyGridAccessibleObject)
            {
                IRawElementProviderFragment.Interface? navigationTarget = propertyGridAccessibleObject.ChildFragmentNavigate(this, direction);
                if (navigationTarget is not null)
                {
                    return navigationTarget;
                }
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_FirstChild => IsSortedByCategories ? GetCategory(0) : GetChild(0),
                NavigateDirection.NavigateDirection_LastChild => IsSortedByCategories ? GetLastCategory() : GetLastChild(),
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
            this.TryGetOwnerAs(out PropertyGridView? owner)
                ? owner.OwnerGrid?.AccessibilityObject
                : null;

        internal override IRawElementProviderFragment.Interface? GetFocus() => GetFocused();

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TableControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_TablePatternId => true,
                UIA_PATTERN_ID.UIA_GridPatternId => true,
                _ => base.IsPatternSupported(patternId)
            };

        private bool IsSortedByCategories
            => this.TryGetOwnerAs(out PropertyGridView? owner) && owner.OwnerGrid is { } ownerGrid && ownerGrid.SortedByCategories;

        public override string Name
        {
            get
            {
                if (!this.TryGetOwnerAs(out PropertyGridView? owner))
                {
                    return string.Empty;
                }

                return owner.AccessibleName is { } name
                    ? name
                    : string.Format(
                        SR.PropertyGridDefaultAccessibleNameTemplate,
                        owner.OwnerGrid.AccessibilityObject.Name);
            }
        }

        internal override bool CanGetNameInternal => false;

        public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.Table);

        public AccessibleObject? Next(GridEntry current)
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            int row = owner.GetRowFromGridEntry(current);
            GridEntry? nextEntry = owner.GetGridEntryFromRow(++row);
            return nextEntry?.AccessibilityObject;
        }

        internal AccessibleObject? GetCategory(int categoryIndex)
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            GridEntryCollection? topLevelGridEntries = owner.TopLevelGridEntries;
            if (topLevelGridEntries is not null && topLevelGridEntries.Count > 0)
            {
                GridItem targetEntry = topLevelGridEntries[categoryIndex];
                if (targetEntry is CategoryGridEntry categoryGridEntry)
                {
                    return categoryGridEntry.AccessibilityObject;
                }
            }

            return null;
        }

        internal AccessibleObject? GetLastCategory()
            => !this.TryGetOwnerAs(out PropertyGridView? owner) ? null : GetCategory(owner.TopLevelGridEntries!.Count - 1);

        internal AccessibleObject? GetLastChild()
        {
            int childCount = GetChildCount();
            return childCount > 0 ? GetChild(childCount - 1) : null;
        }

        /// <summary>
        ///  Gets the previous grid entry accessibility object.
        /// </summary>
        /// <param name="currentGridEntry">The current grid entry.</param>
        /// <param name="gridEntryCollection">The grid entry collection.</param>
        /// <param name="currentGridEntryFound">Indicates whether the current grid entry is found.</param>
        /// <returns>The previous grid entry.</returns>
        internal static AccessibleObject? GetPreviousGridEntry(GridEntry currentGridEntry, GridEntryCollection? gridEntryCollection, out bool currentGridEntryFound)
        {
            currentGridEntryFound = false;

            if (gridEntryCollection is null)
            {
                return null;
            }

            GridEntry? previousGridEntry = null;

            foreach (GridEntry gridEntry in gridEntryCollection)
            {
                if (currentGridEntry == gridEntry)
                {
                    // Set to true to return the previous iterable element.
                    currentGridEntryFound = true;
                    if (previousGridEntry is not null)
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
                        AccessibleObject? foundChild = GetPreviousGridEntry(currentGridEntry, gridEntry.Children, out currentGridEntryFound);
                        if (foundChild is not null)
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
        internal static AccessibleObject? GetNextGridEntry(GridEntry currentGridEntry, GridEntryCollection? gridEntryCollection, out bool currentGridEntryFound)
        {
            currentGridEntryFound = false;

            if (gridEntryCollection is null)
            {
                return null;
            }

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
                    AccessibleObject? foundChild = GetNextGridEntry(currentGridEntry, gridEntry.Children, out currentGridEntryFound);
                    if (foundChild is not null)
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
        internal AccessibleObject? GetFirstChildProperty(CategoryGridEntry current)
        {
            if (current.ChildCount <= 0 || !this.TryGetOwnerAs(out PropertyGridView? _))
            {
                return null;
            }

            GridEntryCollection subGridEntry = current.Children;
            if (subGridEntry.Count > 0)
            {
                var targetEntries = new GridEntry[1];
                try
                {
                    GetGridEntriesFromOutline(subGridEntry, 0, 0, targetEntries);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }

                return targetEntries[0].AccessibilityObject;
            }

            return null;
        }

        /// <summary>
        ///  Gets the last child property.
        /// </summary>
        /// <param name="current">The current grid entry.</param>
        /// <returns>The last child property.</returns>
        internal AccessibleObject? GetLastChildProperty(CategoryGridEntry current)
        {
            if (current.ChildCount <= 0 || !this.TryGetOwnerAs(out PropertyGridView? _))
            {
                return null;
            }

            GridEntryCollection subGridEntry = current.Children;
            if (subGridEntry.Count > 0)
            {
                var targetEntries = new GridEntry[1];
                try
                {
                    GetGridEntriesFromOutline(subGridEntry, 0, subGridEntry.Count - 1, targetEntries);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }

                return targetEntries[0].AccessibilityObject;
            }

            return null;
        }

        /// <summary>
        ///  Gets the next category.
        /// </summary>
        /// <param name="current">The current grid entry.</param>
        /// <returns>The next category.</returns>
        internal AccessibleObject? GetNextCategory(CategoryGridEntry current)
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            int row = owner.GetRowFromGridEntry(current);

            GridEntry? nextEntry;

            do
            {
                nextEntry = owner.GetGridEntryFromRow(++row);
                if (nextEntry is CategoryGridEntry)
                {
                    return nextEntry.AccessibilityObject;
                }
            }
            while (nextEntry is not null);

            return null;
        }

        public AccessibleObject? Previous(GridEntry current)
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            int row = owner.GetRowFromGridEntry(current);
            GridEntry? previousEntry = owner.GetGridEntryFromRow(--row);
            return previousEntry?.AccessibilityObject;
        }

        /// <summary>
        ///  Gets the previous category.
        /// </summary>
        /// <param name="current">The current grid entry.</param>
        /// <returns>The previous category.</returns>
        internal AccessibleObject? GetPreviousCategory(CategoryGridEntry current)
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            int row = owner.GetRowFromGridEntry(current);

            GridEntry? previousEntry;

            do
            {
                previousEntry = owner.GetGridEntryFromRow(--row);
                if (previousEntry is CategoryGridEntry)
                {
                    return previousEntry.AccessibilityObject;
                }
            }
            while (previousEntry is not null);

            return null;
        }

        public override AccessibleObject? GetChild(int index)
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            GridEntryCollection? properties = owner.AccessibilityGetGridEntries();
            if (properties is not null && index >= 0 && index < properties.Count)
            {
                return properties[index].AccessibilityObject;
            }
            else
            {
                return null;
            }
        }

        private protected override bool IsInternal => true;

        public override int GetChildCount() =>
            this.TryGetOwnerAs(out PropertyGridView? owner) && owner.AccessibilityGetGridEntries() is { } entries
                ? entries.Count
                : 0;

        public override AccessibleObject? GetFocused()
        {
            if (!this.TryGetOwnerAs(out PropertyGridView? owner))
            {
                return null;
            }

            GridEntry? gridEntry = owner.SelectedGridEntry;
            if (gridEntry is not null && gridEntry.HasFocus)
            {
                return gridEntry.AccessibilityObject;
            }

            return null;
        }

        public override AccessibleObject? GetSelected()
            => !this.TryGetOwnerAs(out PropertyGridView? owner) ? null : owner.SelectedGridEntry?.AccessibilityObject;

        public override AccessibleObject? HitTest(int x, int y)
        {
            if (!this.IsOwnerHandleCreated(out PropertyGridView? owner))
            {
                return null;
            }

            // Convert to client coordinates
            Point point = new(x, y);
            PInvoke.ScreenToClient(owner, ref point);

            // Find the grid entry at the given client coordinates
            Point position = owner.FindPosition(point.X, point.Y);
            if (position != InvalidPosition)
            {
                GridEntry? gridEntry = owner.GetGridEntryFromRow(position.Y);
                if (gridEntry is not null)
                {
                    // Return the accessible object for this grid entry
                    return gridEntry.AccessibilityObject;
                }
            }

            // No grid entry at this point
            return null;
        }

        public override AccessibleObject? Navigate(AccessibleNavigation navdir)
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

            // Perform default behavior
            return null;
        }

        internal override IRawElementProviderSimple.Interface? GetItem(int row, int column) => GetChild(row);

        internal override int RowCount
        {
            get
            {
                if (!this.TryGetOwnerAs(out PropertyGridView? owner)
                    || owner.TopLevelGridEntries is not { } topLevelGridEntries)
                {
                    return 0;
                }

                if (!IsSortedByCategories)
                {
                    return topLevelGridEntries.Count;
                }

                int categoriesCount = 0;
                foreach (var topLevelGridEntry in topLevelGridEntries)
                {
                    if (topLevelGridEntry is CategoryGridEntry)
                    {
                        categoriesCount++;
                    }
                }

                return categoriesCount;
            }
        }

        internal override int ColumnCount => 1; // There is one column: grid item represents both label and input.
    }
}
