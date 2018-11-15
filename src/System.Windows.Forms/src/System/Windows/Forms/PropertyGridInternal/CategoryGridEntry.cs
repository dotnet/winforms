// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//#define PAINT_CATEGORY_TRIANGLE

namespace System.Windows.Forms.PropertyGridInternal {

    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

     using System;
     using System.Collections;
     using System.Reflection;
     
     using System.ComponentModel;
     using System.ComponentModel.Design;
     using System.Windows.Forms;
     using System.Drawing;
     using Microsoft.Win32;

     internal class CategoryGridEntry : GridEntry {

        internal string name;
        private Brush backBrush = null;
        private static Hashtable categoryStates = null;

        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // GridEntry classes are internal so we have complete
                                                                                                    // control over who does what in the constructor.
        ]
        public CategoryGridEntry(PropertyGrid ownerGrid, GridEntry peParent,string name, GridEntry[] childGridEntries)
        : base(ownerGrid, peParent) {
            this.name = name;

#if DEBUG
            for (int n = 0;n < childGridEntries.Length; n++) {
                Debug.Assert(childGridEntries[n] != null, "Null item in category subproperty list");
            }
#endif
            if (categoryStates == null) {
                categoryStates = new Hashtable();
            }

            lock (categoryStates) {
                if (!categoryStates.ContainsKey(name)) {
                    categoryStates.Add(name, true);
                }
            }

            this.IsExpandable = true;
            
            for (int i = 0; i < childGridEntries.Length; i++) {
                childGridEntries[i].ParentGridEntry = this;
            }
            
            this.ChildCollection = new GridEntryCollection(this, childGridEntries);

            lock (categoryStates) {
                this.InternalExpanded = (bool)categoryStates[name];
            }

            this.SetFlag(GridEntry.FLAG_LABEL_BOLD,true);
        }
        
          
        /// <include file='doc\CategoryGridEntry.uex' path='docs/doc[@for="CategoryGridEntry.HasValue"]/*' />
        /// <devdoc>
        /// Returns true if this GridEntry has a value field in the right hand column.
        /// </devdoc>
        internal override bool HasValue {
            get {
               return false;
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (backBrush != null) {
                    backBrush.Dispose();
                    backBrush = null;
                }

                if (ChildCollection != null) {
                    ChildCollection = null;
                }
            }
            base.Dispose(disposing);
        }

        public override void DisposeChildren() {

            // categories should never dispose
            //
            return;
        }
        
        
        // we don't want this guy participating in property depth.
        public override int PropertyDepth {
            get {
                return base.PropertyDepth - 1;
            }
        }

        protected override Brush GetBackgroundBrush(Graphics g) {
            return this.GridEntryHost.GetLineBrush(g);
        }

        protected override Color LabelTextColor {
            get {
                return ownerGrid.CategoryForeColor;
            }
        }

        public override bool Expandable {
            get {
                return !GetFlagSet(FL_EXPANDABLE_FAILED);
            }
        }
        
        internal override bool InternalExpanded {
            set {
                base.InternalExpanded = value;
                lock (categoryStates) {
                    categoryStates[this.name] = value;
                }
            }
        }
        
        public override GridItemType GridItemType {
            get {
                return GridItemType.Category;
            }
        }
        public override string HelpKeyword {
            get {
               return null;
            }
        }

        public override string PropertyLabel {
            get {
                return name;
            }
        }
        
        internal override int PropertyLabelIndent {
            get {
                // we give an extra pixel for breathing room
                // we want to make sure that we return 0 for property depth here instead of
                PropertyGridView gridHost = this.GridEntryHost;
                
                // we call base.PropertyDepth here because we don't want the subratction to happen.
                return 1+gridHost.GetOutlineIconSize()+OUTLINE_ICON_PADDING + (base.PropertyDepth * gridHost.GetDefaultOutlineIndent());
            }
        }

        public override string GetPropertyTextValue(object o) {
            return "";
        }

        public override Type PropertyType {
            get {
                return typeof(void);
            }
        }

        /// <include file='doc\CategoryGridEntry.uex' path='docs/doc[@for="CategoryGridEntry.GetChildValueOwner"]/*' />
        /// <devdoc>
        /// Gets the owner of the current value.  This is usually the value of the
        /// root entry, which is the object being browsed
        /// </devdoc>
        public override object GetChildValueOwner(GridEntry childEntry) {
            return ParentGridEntry.GetChildValueOwner(childEntry);
        }

        protected override bool CreateChildren(bool diffOldChildren) {
            return true;
        }

        public override string GetTestingInfo() {
            string str = "object = (";
            str += FullLabel;
            str += "), Category = (" + this.PropertyLabel + ")";
            return str;
        }

        public override void PaintLabel(System.Drawing.Graphics g, Rectangle rect, Rectangle clipRect, bool selected, bool paintFullLabel) {

            base.PaintLabel(g, rect, clipRect, false, true);

            // now draw the focus rect
            if (selected && hasFocus) {
                bool bold = ((this.Flags & GridEntry.FLAG_LABEL_BOLD) != 0);
                Font font = GetFont(bold);
                int labelWidth = GetLabelTextWidth(this.PropertyLabel, g, font);

                int indent = PropertyLabelIndent-2;
                Rectangle focusRect = new Rectangle(indent, rect.Y, labelWidth+3, rect.Height-1);
                if (SystemInformation.HighContrast && !OwnerGrid.developerOverride && AccessibilityImprovements.Level1) {
                    // we changed line color to SystemColors.ControlDarkDark in high contrast mode
                    ControlPaint.DrawFocusRectangle(g, focusRect, SystemColors.ControlText, OwnerGrid.LineColor);
                }
                else {
                    ControlPaint.DrawFocusRectangle(g, focusRect);
                }
            }

            // draw the line along the top
            if (parentPE.GetChildIndex(this) > 0) {
                using (Pen topLinePen = new System.Drawing.Pen(ownerGrid.CategorySplitterColor, 1)) {
                    g.DrawLine(topLinePen, rect.X - 1, rect.Y - 1, rect.Width + 2, rect.Y - 1);
                }
            }
        }

        public override void PaintValue(object val, System.Drawing.Graphics g, Rectangle rect, Rectangle clipRect, PaintValueFlags paintFlags) {
            base.PaintValue(val, g, rect, clipRect, paintFlags & ~PaintValueFlags.DrawSelected);

            // draw the line along the top
            if (parentPE.GetChildIndex(this) > 0) {
                using (Pen topLinePen = new System.Drawing.Pen(ownerGrid.CategorySplitterColor, 1)) {
                    g.DrawLine(topLinePen, rect.X - 2, rect.Y - 1, rect.Width + 1, rect.Y - 1);
                }
            }
        }

        internal override bool NotifyChildValue(GridEntry pe, int type) {
            return parentPE.NotifyChildValue(pe, type);
        }
    }

}
