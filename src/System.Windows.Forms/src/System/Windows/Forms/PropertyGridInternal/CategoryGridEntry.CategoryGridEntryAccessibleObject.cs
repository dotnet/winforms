// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class CategoryGridEntry
    {
        /// <summary>
        ///  Defines the Category Grid Entry accessible object that is derived from Grid Entry accessible object.
        /// </summary>
        internal class CategoryGridEntryAccessibleObject : GridEntryAccessibleObject
        {
            private readonly CategoryGridEntry _owningCategoryGridEntry;

            /// <summary>
            ///  Initializes new instance of CategoryGridEntryAccessibleObject.
            /// </summary>
            /// <param name="owningCategoryGridEntry">The owning Category Grid Entry object.</param>
            public CategoryGridEntryAccessibleObject(CategoryGridEntry owningCategoryGridEntry) : base(owningCategoryGridEntry)
            {
                _owningCategoryGridEntry = owningCategoryGridEntry;
            }

            public override AccessibleRole Role => AccessibleRole.ButtonDropDownGrid;

            // Category is in the first column.
            internal override int Column => 0;

            internal override int Row
            {
                get
                {
                    if (Parent is not PropertyGridView.PropertyGridViewAccessibleObject parent)
                    {
                        return -1;
                    }

                    if (parent.Owner is not PropertyGridView gridView || gridView.OwnerGrid is null || !gridView.OwnerGrid.SortedByCategories)
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
                        if (_owningCategoryGridEntry == topLevelGridEntry)
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

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (Parent is not PropertyGridView.PropertyGridViewAccessibleObject parent)
                {
                    return null;
                }

                return direction switch
                {
                    UiaCore.NavigateDirection.Parent => Parent,
                    UiaCore.NavigateDirection.NextSibling => parent.GetNextCategory(_owningCategoryGridEntry),
                    UiaCore.NavigateDirection.PreviousSibling => parent.GetPreviousCategory(_owningCategoryGridEntry),
                    UiaCore.NavigateDirection.FirstChild => parent.GetFirstChildProperty(_owningCategoryGridEntry),
                    UiaCore.NavigateDirection.LastChild => parent.GetLastChildProperty(_owningCategoryGridEntry),
                    _ => base.FragmentNavigate(direction),
                };
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
            {
                // To announce expanded collapsed state control type should be appropriate:
                // https://docs.microsoft.com/en-us/windows/win32/winauto/uiauto-controlpatternmapping
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.TreeItemControlTypeId,
                UiaCore.UIA.LocalizedControlTypePropertyId => SR.CategoryPropertyGridLocalizedControlType,
                _ => base.GetPropertyValue(propertyID),
            };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.GridItemPatternId ||
                    patternId == UiaCore.UIA.TableItemPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }
        }
    }
}
