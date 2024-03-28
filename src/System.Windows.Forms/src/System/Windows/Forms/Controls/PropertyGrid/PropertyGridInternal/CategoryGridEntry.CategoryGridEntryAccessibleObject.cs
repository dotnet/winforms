// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms.PropertyGridInternal;

internal partial class CategoryGridEntry
{
    /// <summary>
    ///  Defines the Category Grid Entry accessible object that is derived from Grid Entry accessible object.
    /// </summary>
    internal class CategoryGridEntryAccessibleObject : GridEntryAccessibleObject
    {
        /// <summary>
        ///  Initializes new instance of CategoryGridEntryAccessibleObject.
        /// </summary>
        /// <param name="owningCategoryGridEntry">The owning Category Grid Entry object.</param>
        public CategoryGridEntryAccessibleObject(CategoryGridEntry owningCategoryGridEntry) : base(owningCategoryGridEntry)
        {
        }

        public override AccessibleRole Role => AccessibleRole.ButtonDropDownGrid;

        // Category is in the first column.
        internal override int Column => 0;

        internal override int Row
        {
            get
            {
                if (!this.TryGetOwnerAs(out CategoryGridEntry? owner)
                    || Parent is not PropertyGridView.PropertyGridViewAccessibleObject parent)
                {
                    return -1;
                }

                if (!parent.TryGetOwnerAs(out PropertyGridView? gridView)
                    || gridView.OwnerGrid is null
                    || !gridView.OwnerGrid.SortedByCategories)
                {
                    return -1;
                }

                GridEntryCollection? topLevelGridEntries = gridView.TopLevelGridEntries;
                if (topLevelGridEntries is null)
                {
                    return -1;
                }

                int categoryIndex = 0;
                foreach (var topLevelGridEntry in topLevelGridEntries)
                {
                    if (owner == topLevelGridEntry)
                    {
                        return categoryIndex;
                    }

                    if (topLevelGridEntry is CategoryGridEntry)
                    {
                        categoryIndex++;
                    }
                }

                return -1;
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (Parent is not PropertyGridView.PropertyGridViewAccessibleObject parent
                || !this.TryGetOwnerAs(out CategoryGridEntry? owner))
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => Parent,
                NavigateDirection.NavigateDirection_NextSibling => parent.GetNextCategory(owner),
                NavigateDirection.NavigateDirection_PreviousSibling => parent.GetPreviousCategory(owner),
                NavigateDirection.NavigateDirection_FirstChild => parent.GetFirstChildProperty(owner),
                NavigateDirection.NavigateDirection_LastChild => parent.GetLastChildProperty(owner),
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
        {
            // To announce expanded collapsed state control type should be appropriate:
            // https://docs.microsoft.com/en-us/windows/win32/winauto/uiauto-controlpatternmapping
            UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_TreeItemControlTypeId,
            UIA_PROPERTY_ID.UIA_LocalizedControlTypePropertyId => (VARIANT)SR.CategoryPropertyGridLocalizedControlType,
            _ => base.GetPropertyValue(propertyID),
        };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
        {
            if (patternId is UIA_PATTERN_ID.UIA_GridItemPatternId or UIA_PATTERN_ID.UIA_TableItemPatternId)
            {
                return true;
            }

            return base.IsPatternSupported(patternId);
        }
    }
}
