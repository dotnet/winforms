// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  ToolStripOverflowButton
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.None)]
    public class ToolStripOverflowButton : ToolStripDropDownButton
    {
        // we need to cache this away as the Parent property gets reset a lot.
        private readonly ToolStrip parentToolStrip;

        private static bool isScalingInitialized;
        private const int MAX_WIDTH = 16;
        private const int MAX_HEIGHT = 16;
        private static int maxWidth = MAX_WIDTH;
        private static int maxHeight = MAX_HEIGHT;

        internal ToolStripOverflowButton(ToolStrip parentToolStrip)
        {
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

        protected override void Dispose(bool disposing)
        {
            if (disposing && HasDropDownItems)
            {
                DropDown.Dispose();
            }

            base.Dispose(disposing);
        }

        protected internal override Padding DefaultMargin
        {
            get
            {
                return Padding.Empty;
            }
        }

        public override bool HasDropDownItems
        {
            get
            {
                return ParentInternal.OverflowItems.Count > 0;
            }
        }

        internal override bool OppositeDropDownAlign
        {
            get { return true; }
        }

        internal ToolStrip ParentToolStrip
        {
            get { return parentToolStrip; }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool RightToLeftAutoMirrorImage
        {
            get => base.RightToLeftAutoMirrorImage;
            set => base.RightToLeftAutoMirrorImage = value;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripOverflowButtonAccessibleObject(this);
        }

        protected override ToolStripDropDown CreateDefaultDropDown()
        {
            // AutoGenerate a ToolStrip DropDown - set the property so we hook events
            return new ToolStripOverflow(this);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = constrainingSize;
            if (ParentInternal != null)
            {
                if (ParentInternal.Orientation == Orientation.Horizontal)
                {
                    preferredSize.Width = Math.Min(constrainingSize.Width, maxWidth);
                }
                else
                {
                    preferredSize.Height = Math.Min(constrainingSize.Height, maxHeight);
                }
            }
            return preferredSize + Padding.Size;
        }

        // make sure the Overflow button extends from edge-edge. (Ignore Padding/Margin).
        internal protected override void SetBounds(Rectangle bounds)
        {
            if (ParentInternal != null && ParentInternal.LayoutEngine is ToolStripSplitStackLayout)
            {
                if (ParentInternal.Orientation == Orientation.Horizontal)
                {
                    bounds.Height = ParentInternal.Height;
                    bounds.Y = 0;
                }
                else
                {
                    bounds.Width = ParentInternal.Width;
                    bounds.X = 0;
                }
            }
            base.SetBounds(bounds);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (ParentInternal != null)
            {
                ToolStripRenderer renderer = ParentInternal.Renderer;
                renderer.DrawOverflowButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));
            }
        }

        internal class ToolStripOverflowButtonAccessibleObject : ToolStripDropDownItemAccessibleObject
        {
            private string stockName;

            public ToolStripOverflowButtonAccessibleObject(ToolStripOverflowButton owner) : base(owner)
            {
            }

            public override string Name
            {
                get
                {
                    string name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }
                    if (string.IsNullOrEmpty(stockName))
                    {
                        stockName = SR.ToolStripOptions;
                    }
                    return stockName;
                }
                set => base.Name = value;
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.ControlTypePropertyId)
                {
                    return UiaCore.UIA.MenuItemControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
