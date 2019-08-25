// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class ToolStripOverflow : ToolStripDropDown, IArrangedElement
    {
#if DEBUG
        internal static readonly TraceSwitch PopupLayoutDebug = new TraceSwitch("PopupLayoutDebug", "Debug ToolStripPopup Layout code");
#else
        internal static readonly TraceSwitch PopupLayoutDebug;
#endif

        private readonly ToolStripOverflowButton ownerItem;

        public ToolStripOverflow(ToolStripItem parentItem) : base(parentItem)
        {
            if (parentItem == null)
            {
                throw new ArgumentNullException(nameof(parentItem));
            }
            ownerItem = parentItem as ToolStripOverflowButton;
        }

        protected internal override ToolStripItemCollection DisplayedItems
        {
            get
            {
                if (ParentToolStrip != null)
                {
                    ToolStripItemCollection items = ParentToolStrip.OverflowItems;
                    return items;
                }
                return new ToolStripItemCollection(null, false);
            }
        }

        public override ToolStripItemCollection Items
        {
            get
            {
                return new ToolStripItemCollection(null, /*ownedCollection=*/false, /*readonly=*/true);
            }
        }

        private ToolStrip ParentToolStrip
        {
            get
            {
                if (ownerItem != null)
                {
                    return ownerItem.ParentToolStrip;
                }
                return null;
            }
        }

        ArrangedElementCollection IArrangedElement.Children
        {
            get { return DisplayedItems; }
        }

        IArrangedElement IArrangedElement.Container
        {
            get { return ParentInternal; }
        }

        bool IArrangedElement.ParticipatesInLayout
        {
            get { return GetState(STATE_VISIBLE); }
        }

        PropertyStore IArrangedElement.Properties
        {
            get { return Properties; }
        }

        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);
        }

        /// <summary>
        ///  Summary of CreateLayoutEngine.
        /// </summary>
        /// <param name=item></param>
        public override LayoutEngine LayoutEngine
        {
            get
            {
                return FlowLayout.Instance;
            }
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripOverflowAccessibleObject(this);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            constrainingSize.Width = 200;
            return base.GetPreferredSize(constrainingSize);
        }
        protected override void OnLayout(LayoutEventArgs e)
        {
            if (ParentToolStrip != null && ParentToolStrip.IsInDesignMode)
            {
                if (FlowLayout.GetFlowDirection(this) != FlowDirection.TopDown)
                {
                    FlowLayout.SetFlowDirection(this, FlowDirection.TopDown);
                }
                if (FlowLayout.GetWrapContents(this))
                {
                    FlowLayout.SetWrapContents(this, false);
                }
            }
            else
            {
                if (FlowLayout.GetFlowDirection(this) != FlowDirection.LeftToRight)
                {
                    FlowLayout.SetFlowDirection(this, FlowDirection.LeftToRight);
                }
                if (!FlowLayout.GetWrapContents(this))
                {
                    FlowLayout.SetWrapContents(this, true);
                }
            }
            base.OnLayout(e);

        }

        protected override void SetDisplayedItems()
        {
            // do nothing here.... this is really for the setting the overflow/displayed items on the
            // main ToolStrip.   Our working item collection is our displayed item collection... calling
            // base would clear it out.
            Size biggestItemSize = Size.Empty;
            for (int j = 0; j < DisplayedItems.Count; j++)
            {
                ToolStripItem item = DisplayedItems[j];
                if (((IArrangedElement)item).ParticipatesInLayout)
                {
                    HasVisibleItems = true;
                    biggestItemSize = LayoutUtils.UnionSizes(biggestItemSize, item.Bounds.Size);
                }
            }
            SetLargestItemSize(biggestItemSize);
        }

        internal class ToolStripOverflowAccessibleObject : ToolStripAccessibleObject
        {
            public ToolStripOverflowAccessibleObject(ToolStripOverflow owner)
                : base(owner)
            {
            }

            public override AccessibleObject GetChild(int index)
            {
                return ((ToolStripOverflow)Owner).DisplayedItems[index].AccessibilityObject;
            }

            public override int GetChildCount()
            {
                return ((ToolStripOverflow)Owner).DisplayedItems.Count;
            }
        }

    }

}
