// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
#if DEBUG
using System.Diagnostics;
#endif
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
                s_categoryStates ??= new();

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

            SetFlag(Flags.LabelBold, true);
        }

        /// <summary>
        ///  Returns true if this GridEntry has a value field in the right hand column.
        /// </summary>
        internal override bool HasValue => false;

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _backBrush?.Dispose();
                _backBrush = null;
                ChildCollection = null;
            }

            base.Dispose(disposing);
        }

        // Categories should never dispose
        public override void DisposeChildren() { }

        // Don't want this participating in property depth.
        public override int PropertyDepth => base.PropertyDepth - 1;

        /// <summary>
        ///  Gets the accessibility object for the current category grid entry.
        /// </summary>
        protected override GridEntryAccessibleObject GetAccessibilityObject() => new CategoryGridEntryAccessibleObject(this);

        protected override Color GetBackgroundColor() => GridEntryHost.GetLineColor();

        protected override Color LabelTextColor => OwnerGrid.CategoryForeColor;

        public override bool Expandable => !GetFlagSet(Flags.ExpandableFailed);

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

        public override GridItemType GridItemType => GridItemType.Category;

        public override string HelpKeyword => null;

        public override string PropertyLabel => _name;

        internal override int PropertyLabelIndent
        {
            get
            {
                PropertyGridView gridHost = GridEntryHost;

                // Give an extra pixel for breathing room.
                // Calling base.PropertyDepth to avoid the -1 in the override.
                return 1 + gridHost.GetOutlineIconSize() + OutlineIconPadding + (base.PropertyDepth * gridHost.GetDefaultOutlineIndent());
            }
        }

        public override string GetPropertyTextValue(object o) => string.Empty;

        public override Type PropertyType => typeof(void);

        /// <inheritdoc />
        public override object GetChildValueOwner(GridEntry childEntry) => ParentGridEntry.GetChildValueOwner(childEntry);

        protected override bool CreateChildren(bool diffOldChildren) => true;

        public override string GetTestingInfo() => $"object = ({FullLabel}), Category = ({PropertyLabel})";

        public override void PaintLabel(Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel)
        {
            base.PaintLabel(g, rect, clipRect, false, true);

            // Draw the focus rect.
            if (selected && HasFocus)
            {
                bool bold = (EntryFlags & Flags.LabelBold) != 0;
                Font font = GetFont(bold);
                int labelWidth = GetLabelTextWidth(PropertyLabel, g, font);

                int indent = PropertyLabelIndent - 2;
                Rectangle focusRect = new(indent, rect.Y, labelWidth + 3, rect.Height - 1);
                if (SystemInformation.HighContrast && !OwnerGrid._developerOverride)
                {
                    // Line color is SystemColors.ControlDarkDark in high contrast mode.
                    ControlPaint.DrawFocusRectangle(g, focusRect, SystemColors.ControlText, OwnerGrid.LineColor);
                }
                else
                {
                    ControlPaint.DrawFocusRectangle(g, focusRect);
                }
            }

            // Draw the line along the top.
            if (ParentGridEntry.GetChildIndex(this) > 0)
            {
                using var topLinePen = OwnerGrid.CategorySplitterColor.GetCachedPenScope();
                g.DrawLine(topLinePen, rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Y - 1);
            }
        }

        public override void PaintValue(object value, Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags)
        {
            base.PaintValue(value, g, rect, clipRect, paintFlags & ~PaintValueFlags.DrawSelected);

            // Draw the line along the top.
            if (ParentGridEntry.GetChildIndex(this) > 0)
            {
                using var topLinePen = OwnerGrid.CategorySplitterColor.GetCachedPenScope();
                g.DrawLine(topLinePen, rect.X - 2, rect.Y - 1, rect.Width + 1, rect.Y - 1);
            }
        }

        protected internal override bool NotifyChildValue(GridEntry entry, Notify type)
            => ParentGridEntry.NotifyChildValue(entry, type);
    }
}
