// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

//#define PAINT_CATEGORY_TRIANGLE

using System.Collections;
using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal class CategoryGridEntry : GridEntry
    {
        internal string name;
        private Brush backBrush;
        private static Hashtable categoryStates;

        public CategoryGridEntry(PropertyGrid ownerGrid, GridEntry peParent, string name, GridEntry[] childGridEntries)
        : base(ownerGrid, peParent)
        {
            this.name = name;

#if DEBUG
            for (int n = 0; n < childGridEntries.Length; n++)
            {
                Debug.Assert(childGridEntries[n] != null, "Null item in category subproperty list");
            }
#endif
            if (categoryStates is null)
            {
                categoryStates = new Hashtable();
            }

            lock (categoryStates)
            {
                if (!categoryStates.ContainsKey(name))
                {
                    categoryStates.Add(name, true);
                }
            }

            IsExpandable = true;

            for (int i = 0; i < childGridEntries.Length; i++)
            {
                childGridEntries[i].ParentGridEntry = this;
            }

            ChildCollection = new GridEntryCollection(this, childGridEntries);

            lock (categoryStates)
            {
                InternalExpanded = (bool)categoryStates[name];
            }

            SetFlag(GridEntry.FLAG_LABEL_BOLD, true);
        }

        /// <summary>
        ///  Returns true if this GridEntry has a value field in the right hand column.
        /// </summary>
        internal override bool HasValue
        {
            get
            {
                return false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (backBrush != null)
                {
                    backBrush.Dispose();
                    backBrush = null;
                }

                if (ChildCollection != null)
                {
                    ChildCollection = null;
                }
            }
            base.Dispose(disposing);
        }

        public override void DisposeChildren()
        {
            // Categories should never dispose
            return;
        }

        // Don't want this participating in property depth.
        public override int PropertyDepth => base.PropertyDepth - 1;

        /// <summary>
        ///  Gets the accessibility object for the current category grid entry.
        /// </summary>
        protected override GridEntryAccessibleObject GetAccessibilityObject()
        {
            return new CategoryGridEntryAccessibleObject(this);
        }

        protected override Color GetBackgroundColor() => GridEntryHost.GetLineColor();

        protected override Color LabelTextColor => ownerGrid.CategoryForeColor;

        public override bool Expandable
        {
            get
            {
                return !GetFlagSet(FL_EXPANDABLE_FAILED);
            }
        }

        internal override bool InternalExpanded
        {
            set
            {
                base.InternalExpanded = value;
                lock (categoryStates)
                {
                    categoryStates[name] = value;
                }
            }
        }

        public override GridItemType GridItemType
        {
            get
            {
                return GridItemType.Category;
            }
        }
        public override string HelpKeyword
        {
            get
            {
                return null;
            }
        }

        public override string PropertyLabel
        {
            get
            {
                return name;
            }
        }

        internal override int PropertyLabelIndent
        {
            get
            {
                // we give an extra pixel for breathing room
                // we want to make sure that we return 0 for property depth here instead of
                PropertyGridView gridHost = GridEntryHost;

                // we call base.PropertyDepth here because we don't want the subratction to happen.
                return 1 + gridHost.GetOutlineIconSize() + OUTLINE_ICON_PADDING + (base.PropertyDepth * gridHost.GetDefaultOutlineIndent());
            }
        }

        public override string GetPropertyTextValue(object o)
        {
            return "";
        }

        public override Type PropertyType
        {
            get
            {
                return typeof(void);
            }
        }

        /// <summary>
        ///  Gets the owner of the current value.  This is usually the value of the
        ///  root entry, which is the object being browsed
        /// </summary>
        public override object GetChildValueOwner(GridEntry childEntry)
        {
            return ParentGridEntry.GetChildValueOwner(childEntry);
        }

        protected override bool CreateChildren(bool diffOldChildren)
        {
            return true;
        }

        public override string GetTestingInfo()
        {
            string str = "object = (";
            str += FullLabel;
            str += "), Category = (" + PropertyLabel + ")";
            return str;
        }

        public override void PaintLabel(Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel)
        {
            base.PaintLabel(g, rect, clipRect, false, true);

            // now draw the focus rect
            if (selected && hasFocus)
            {
                bool bold = ((Flags & GridEntry.FLAG_LABEL_BOLD) != 0);
                Font font = GetFont(bold);
                int labelWidth = GetLabelTextWidth(PropertyLabel, g, font);

                int indent = PropertyLabelIndent - 2;
                Rectangle focusRect = new Rectangle(indent, rect.Y, labelWidth + 3, rect.Height - 1);
                if (SystemInformation.HighContrast && !OwnerGrid._developerOverride)
                {
                    // we changed line color to SystemColors.ControlDarkDark in high contrast mode
                    ControlPaint.DrawFocusRectangle(g, focusRect, SystemColors.ControlText, OwnerGrid.LineColor);
                }
                else
                {
                    ControlPaint.DrawFocusRectangle(g, focusRect);
                }
            }

            // draw the line along the top
            if (parentPE.GetChildIndex(this) > 0)
            {
                using var topLinePen = ownerGrid.CategorySplitterColor.GetCachedPenScope();
                g.DrawLine(topLinePen, rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Y - 1);
            }
        }

        public override void PaintValue(object val, Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags)
        {
            base.PaintValue(val, g, rect, clipRect, paintFlags & ~PaintValueFlags.DrawSelected);

            // draw the line along the top
            if (parentPE.GetChildIndex(this) > 0)
            {
                using var topLinePen = ownerGrid.CategorySplitterColor.GetCachedPenScope();
                g.DrawLine(topLinePen, rect.X - 2, rect.Y - 1, rect.Width + 1, rect.Y - 1);
            }
        }

        internal override bool NotifyChildValue(GridEntry pe, int type)
        {
            return parentPE.NotifyChildValue(pe, type);
        }

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

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                PropertyGridView.PropertyGridViewAccessibleObject parent = (PropertyGridView.PropertyGridViewAccessibleObject)Parent;

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                        return parent.GetNextCategory(_owningCategoryGridEntry);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return parent.GetPreviousCategory(_owningCategoryGridEntry);
                    case UiaCore.NavigateDirection.FirstChild:
                        return parent.GetFirstChildProperty(_owningCategoryGridEntry);
                    case UiaCore.NavigateDirection.LastChild:
                        return parent.GetLastChildProperty(_owningCategoryGridEntry);
                }

                return base.FragmentNavigate(direction);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.GridItemPatternId ||
                    patternId == UiaCore.UIA.TableItemPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.ControlTypePropertyId:
                        // To announce expanded collapsed state control type should be appropriate:
                        // https://docs.microsoft.com/en-us/windows/win32/winauto/uiauto-controlpatternmapping
                        return UiaCore.UIA.TreeItemControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override AccessibleRole Role
            {
                get
                {
                    return AccessibleRole.ButtonDropDownGrid;
                }
            }

            internal override int Row
            {
                get
                {
                    var parent = Parent as PropertyGridView.PropertyGridViewAccessibleObject;
                    if (parent is null)
                    {
                        return -1;
                    }

                    var gridView = parent.Owner as PropertyGridView;
                    if (gridView is null || gridView.OwnerGrid is null || !gridView.OwnerGrid.SortedByCategories)
                    {
                        return -1;
                    }

                    var topLevelGridEntries = gridView.TopLevelGridEntries;
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

            internal override int Column => 0; // Category is in the first column.
        }
    }
}
