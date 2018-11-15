// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Diagnostics;
    using System.Windows.Forms.Design;


    /// <include file='doc\ToolStripOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton"]/*' />
    /// <devdoc>
    /// ToolStripOverflowButton
    /// </devdoc>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
    public class ToolStripOverflowButton : ToolStripDropDownButton {

        // we need to cache this away as the Parent property gets reset a lot.
        private ToolStrip parentToolStrip;
       
        private static bool isScalingInitialized = false;
        private const int MAX_WIDTH = 16;
        private const int MAX_HEIGHT = 16;
        private static int maxWidth = MAX_WIDTH;
        private static int maxHeight = MAX_HEIGHT;
        
        /// <include file='doc\ToolStripOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton.ToolStripOverflowButton"]/*' />
	    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        internal ToolStripOverflowButton(ToolStrip parentToolStrip) {
            if (!isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    maxWidth = DpiHelper.LogicalToDeviceUnitsX(MAX_WIDTH);
                    maxHeight = DpiHelper.LogicalToDeviceUnitsY(MAX_HEIGHT);
                }

                isScalingInitialized = true;
            }

            SupportsItemClick = false;
            this.parentToolStrip = parentToolStrip;
        }
       
        protected override void Dispose(bool disposing) {
            if (disposing && this.HasDropDownItems) {
                this.DropDown.Dispose();
            }

            base.Dispose(disposing);
        }

        /// <include file='doc\WinBarOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton.DefaultMargin"]/*' />
        protected internal override Padding DefaultMargin {
            get {
                return Padding.Empty;
            }
        }

        /// <include file='doc\ToolStripOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton.HasDropDownItems"]/*' />
        public override bool HasDropDownItems {
            get {
                return this.ParentInternal.OverflowItems.Count > 0;
            }
        }


        internal override bool OppositeDropDownAlign {
            get { return true; }
        }

        internal ToolStrip ParentToolStrip {
            get { return parentToolStrip; }
        }
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool RightToLeftAutoMirrorImage {
            get {
                return base.RightToLeftAutoMirrorImage;
            }
            set {
                base.RightToLeftAutoMirrorImage = value;
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance() {
            return new ToolStripOverflowButtonAccessibleObject(this);
        }


        /// <include file='doc\ToolStripOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton.CreateDefaultDropDown"]/*' />
        protected override ToolStripDropDown CreateDefaultDropDown() {
            // AutoGenerate a Winbar DropDown - set the property so we hook events
             return new ToolStripOverflow(this);
        }
       
        /// <include file='doc\ToolStripOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton.GetPreferredSize"]/*' />
        public override Size GetPreferredSize(Size constrainingSize) {
            Size preferredSize = constrainingSize;
            if (this.ParentInternal != null)  {
              if (this.ParentInternal.Orientation == Orientation.Horizontal) {
                  preferredSize.Width = Math.Min(constrainingSize.Width, maxWidth);
              }
              else {
                  preferredSize.Height = Math.Min(constrainingSize.Height, maxHeight);
              }                
            }
            return preferredSize + this.Padding.Size;
        }

        // make sure the Overflow button extends from edge-edge. (Ignore Padding/Margin).
        internal protected override void SetBounds(Rectangle bounds) {
            if (ParentInternal != null && ParentInternal.LayoutEngine is ToolStripSplitStackLayout) {
                
                if (ParentInternal.Orientation == Orientation.Horizontal) {
                    bounds.Height = ParentInternal.Height;
                    bounds.Y = 0;
                }
                else {
                    bounds.Width = ParentInternal.Width;
                    bounds.X = 0;
                }
            }
            base.SetBounds(bounds);
        }

        /// <include file='doc\ToolStripOverflowButton.uex' path='docs/doc[@for="ToolStripOverflowButton.OnPaint"]/*' />
        protected override void OnPaint(PaintEventArgs e) {
            if (this.ParentInternal != null) {
                ToolStripRenderer renderer = this.ParentInternal.Renderer;            
                renderer.DrawOverflowButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
            }
        }

        internal class ToolStripOverflowButtonAccessibleObject : ToolStripDropDownItemAccessibleObject {
            private string stockName;

            public ToolStripOverflowButtonAccessibleObject(ToolStripOverflowButton owner) : base(owner){
            }
 
            
            public override string Name {
                get {
                    string name = Owner.AccessibleName;
                    if (name != null) {
                        return name;
                    }
                    if (string.IsNullOrEmpty(stockName)) {
                        stockName = SR.ToolStripOptions;
                    }
                    return stockName;
                }
                set {
                    base.Name  = value;
                }
            }

        }

    }
}
