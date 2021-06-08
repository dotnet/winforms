// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

//#define PAINT_CATEGORY_TRIANGLE

using System.Collections;
using System.Diagnostics;
using System.Drawing;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class CategoryGridEntry : GridEntry
    {
        private readonly string _name;
        private Brush _backBrush;
        private static Hashtable s_categoryStates;
        private readonly static object s_lock = new();

        public CategoryGridEntry(PropertyGrid ownerGrid, GridEntry peParent, string name, GridEntry[] childGridEntries)
            : base(ownerGrid, peParent)
        {
            _name = name;

#if DEBUG
            for (int n = 0; n < childGridEntries.Length; n++)
            {
                Debug.Assert(childGridEntries[n] is not null, "Null item in category subproperty list");
            }
#endif

            lock (s_lock)
            {
                s_categoryStates ??= new Hashtable();

                if (!s_categoryStates.ContainsKey(name))
                {
                    s_categoryStates.Add(name, true);
                }
            }

            IsExpandable = true;

            for (int i = 0; i < childGridEntries.Length; i++)
            {
                childGridEntries[i].ParentGridEntry = this;
            }

            ChildCollection = new GridEntryCollection(this, childGridEntries);

            lock (s_lock)
            {
                InternalExpanded = (bool)s_categoryStates[name];
            }

            SetFlag(FLAG_LABEL_BOLD, true);
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
                if (_backBrush is not null)
                {
                    _backBrush.Dispose();
                    _backBrush = null;
                }

                if (ChildCollection is not null)
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

        protected override Color LabelTextColor => OwnerGrid.CategoryForeColor;

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
                lock (s_lock)
                {
                    s_categoryStates[_name] = value;
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
                return _name;
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
                return 1 + gridHost.GetOutlineIconSize() + OutlineIconPadding + (base.PropertyDepth * gridHost.GetDefaultOutlineIndent());
            }
        }

        public override string GetPropertyTextValue(object o) => string.Empty;

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
            if (selected && HasFocus)
            {
                bool bold = ((Flags & FLAG_LABEL_BOLD) != 0);
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

            // Draw the line along the top
            if (ParentGridEntry.GetChildIndex(this) > 0)
            {
                using var topLinePen = OwnerGrid.CategorySplitterColor.GetCachedPenScope();
                g.DrawLine(topLinePen, rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Y - 1);
            }
        }

        public override void PaintValue(object val, Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags)
        {
            base.PaintValue(val, g, rect, clipRect, paintFlags & ~PaintValueFlags.DrawSelected);

            // Draw the line along the top
            if (ParentGridEntry.GetChildIndex(this) > 0)
            {
                using var topLinePen = OwnerGrid.CategorySplitterColor.GetCachedPenScope();
                g.DrawLine(topLinePen, rect.X - 2, rect.Y - 1, rect.Width + 1, rect.Y - 1);
            }
        }

        internal override bool NotifyChildValue(GridEntry pe, int type)
        {
            return ParentGridEntry.NotifyChildValue(pe, type);
        }
    }
}
