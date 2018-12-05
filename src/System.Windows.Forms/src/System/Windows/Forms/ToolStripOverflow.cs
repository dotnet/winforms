// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms.Layout;
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.InteropServices;
    

    /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow"]/*' />
    /// <devdoc>
    /// ToolStripOverflow
    /// </devdoc>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class ToolStripOverflow : ToolStripDropDown, IArrangedElement {

       

#if DEBUG        
        internal static readonly TraceSwitch PopupLayoutDebug = new TraceSwitch("PopupLayoutDebug", "Debug ToolStripPopup Layout code");
#else
        internal static readonly TraceSwitch PopupLayoutDebug;
#endif

        private ToolStripOverflowButton ownerItem;

        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.ToolStripOverflow"]/*' />
        public ToolStripOverflow (ToolStripItem parentItem) : base(parentItem) {
            if (parentItem == null) {
                throw new ArgumentNullException(nameof(parentItem));
            }
            ownerItem = parentItem as ToolStripOverflowButton;
        }


        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.DisplayedItems"]/*' />
        protected internal override ToolStripItemCollection DisplayedItems {
            get {
                if (ParentToolStrip != null) {
                    ToolStripItemCollection items =  ParentToolStrip.OverflowItems;
                    return items;
                }
                return new ToolStripItemCollection(null, false);
            } 
        }

        public override ToolStripItemCollection Items {
           get {
                return new ToolStripItemCollection(null, /*ownedCollection=*/false, /*readonly=*/true);
           }
       }

        private ToolStrip ParentToolStrip {
            get { 
                if (ownerItem != null) {
                    return ownerItem.ParentToolStrip;                     
                }
                return null;
            }
        }

        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.IArrangedElement.Children"]/*' />
        /// <internalonly/>
        ArrangedElementCollection IArrangedElement.Children {
            get { return DisplayedItems; }
        }
        
        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.IArrangedElement.Container"]/*' />
        /// <internalonly/>
        IArrangedElement IArrangedElement.Container {
            get { return ParentInternal; }
        }
        
        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.IArrangedElement.ParticipatesInLayout"]/*' />
        /// <internalonly/>
        bool IArrangedElement.ParticipatesInLayout {
            get { return GetState(STATE_VISIBLE); }
        }
                

        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.IArrangedElement.Properties"]/*' />
        /// <internalonly/>
        PropertyStore IArrangedElement.Properties {
            get { return Properties; }
        }
        
        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.IArrangedElement.SetBounds"]/*' />
        /// <internalonly/>
        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified) {
            SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);
        }

        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.CreateLayoutEngine"]/*' />
        /// <devdoc>
        /// Summary of CreateLayoutEngine.
        /// </devdoc>
        /// <param name=item></param>
        public override LayoutEngine LayoutEngine {
            get {
                return FlowLayout.Instance; 
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance() {
            return new ToolStripOverflowAccessibleObject(this);
        }

        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
        public override Size GetPreferredSize(Size constrainingSize) {
            constrainingSize.Width = 200;
            return base.GetPreferredSize(constrainingSize);
        }
        protected override void OnLayout(LayoutEventArgs e) {

            if (ParentToolStrip != null && ParentToolStrip.IsInDesignMode) {
                if (FlowLayout.GetFlowDirection(this) != FlowDirection.TopDown) {
                    FlowLayout.SetFlowDirection(this, FlowDirection.TopDown);
                }
                if (FlowLayout.GetWrapContents(this)) {
                    FlowLayout.SetWrapContents(this, false);
                }
            }
            else {
                if (FlowLayout.GetFlowDirection(this) != FlowDirection.LeftToRight) {
                    FlowLayout.SetFlowDirection(this, FlowDirection.LeftToRight);
                }
                if (!FlowLayout.GetWrapContents(this)) {
                   FlowLayout.SetWrapContents(this, true);
                }
            }
            base.OnLayout(e);

        }

        /// <include file='doc\ToolStripOverflow.uex' path='docs/doc[@for="ToolStripOverflow.SetDisplayedItems"]/*' />
        protected override void SetDisplayedItems() {
            // do nothing here.... this is really for the setting the overflow/displayed items on the 
            // main winbar.   Our working item collection is our displayed item collection... calling
            // base would clear it out.
            Size biggestItemSize = Size.Empty;
            for (int j = 0; j < DisplayedItems.Count; j++) {
                ToolStripItem item = DisplayedItems[j];
                if (((IArrangedElement)item).ParticipatesInLayout) {
                    HasVisibleItems = true;
                    biggestItemSize = LayoutUtils.UnionSizes(biggestItemSize, item.Bounds.Size);
                }
            }
            SetLargestItemSize(biggestItemSize);
        }

        private class ToolStripOverflowAccessibleObject : ToolStripAccessibleObject {
            public ToolStripOverflowAccessibleObject(ToolStripOverflow owner)
                : base(owner) {
            }

            public override AccessibleObject GetChild(int index) {
                return ((ToolStripOverflow)Owner).DisplayedItems[index].AccessibilityObject;
            }

            public override int GetChildCount() {
                return ((ToolStripOverflow)Owner).DisplayedItems.Count;
            }
        }

    }

}
