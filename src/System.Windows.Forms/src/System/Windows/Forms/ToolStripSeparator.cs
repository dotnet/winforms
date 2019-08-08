// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms.Design;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Called when the background of the ToolStrip is being rendered
    /// </summary>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class ToolStripSeparator : ToolStripItem
    {
        private const int ToolStrip_SEPARATORTHICKNESS = 6;
        private const int ToolStrip_SEPARATORHEIGHT = 23;

        public ToolStripSeparator()
        {
            ForeColor = SystemColors.ControlDark;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool AutoToolTip
        {
            get
            {
                return base.AutoToolTip;
            }
            set
            {
                base.AutoToolTip = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;

            }
            set
            {
                base.BackgroundImage = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }

        public override bool CanSelect
        {
            get
            {
                return DesignMode;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(ToolStrip_SEPARATORTHICKNESS, ToolStrip_SEPARATORTHICKNESS);
            }
        }

        protected internal override Padding DefaultMargin
        {
            get
            {
                return Padding.Empty;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool DoubleClickEnabled
        {
            get
            {
                return base.DoubleClickEnabled;
            }
            set
            {
                base.DoubleClickEnabled = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }

        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged
        {
            add => base.EnabledChanged += value;
            remove => base.EnabledChanged -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ToolStripItemDisplayStyle DisplayStyle
        {
            get
            {
                return base.DisplayStyle;
            }
            set
            {
                base.DisplayStyle = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DisplayStyleChanged
        {
            add => base.DisplayStyleChanged += value;
            remove => base.DisplayStyleChanged -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ContentAlignment ImageAlign
        {
            get
            {
                return base.ImageAlign;
            }
            set
            {
                base.ImageAlign = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override Image Image
        {
            get
            {
                return base.Image;
            }
            set
            {
                base.Image = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        RefreshProperties(RefreshProperties.Repaint),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new int ImageIndex
        {
            get
            {
                return base.ImageIndex;
            }
            set
            {
                base.ImageIndex = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new string ImageKey
        {
            get
            {
                return base.ImageKey;
            }
            set
            {
                base.ImageKey = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Color ImageTransparentColor
        {
            get
            {
                return base.ImageTransparentColor;
            }
            set
            {
                base.ImageTransparentColor = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ToolStripItemImageScaling ImageScaling
        {
            get
            {
                return base.ImageScaling;
            }
            set
            {
                base.ImageScaling = value;
            }
        }

        private bool IsVertical
        {
            get
            {
                ToolStrip parent = ParentInternal;

                if (parent == null)
                {
                    parent = Owner;
                }
                if (parent is ToolStripDropDownMenu dropDownMenu)
                {
                    return false;
                }
                switch (parent.LayoutStyle)
                {
                    case ToolStripLayoutStyle.VerticalStackWithOverflow:
                        return false;
                    case ToolStripLayoutStyle.HorizontalStackWithOverflow:
                    case ToolStripLayoutStyle.Flow:
                    case ToolStripLayoutStyle.Table:
                    default:
                        return true;
                }
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new ContentAlignment TextAlign
        {
            get
            {
                return base.TextAlign;
            }
            set
            {
                base.TextAlign = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(ToolStripTextDirection.Horizontal)]
        public override ToolStripTextDirection TextDirection
        {
            get
            {
                return base.TextDirection;
            }
            set
            {
                base.TextDirection = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new TextImageRelation TextImageRelation
        {
            get
            {
                return base.TextImageRelation;
            }
            set
            {
                base.TextImageRelation = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new string ToolTipText
        {
            get
            {
                return base.ToolTipText;
            }
            set
            {
                base.ToolTipText = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new bool RightToLeftAutoMirrorImage
        {
            get
            {
                return base.RightToLeftAutoMirrorImage;
            }
            set
            {
                base.RightToLeftAutoMirrorImage = value;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripSeparatorAccessibleObject(this);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            ToolStrip parent = ParentInternal;

            if (parent == null)
            {
                parent = Owner;
            }
            if (parent == null)
            {
                return new Size(ToolStrip_SEPARATORTHICKNESS, ToolStrip_SEPARATORTHICKNESS);
            }

            if (parent is ToolStripDropDownMenu dropDownMenu)
            {
                return new Size(parent.Width - (parent.Padding.Horizontal - dropDownMenu.ImageMargin.Width), ToolStrip_SEPARATORTHICKNESS);
            }
            else
            {
                if (parent.LayoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow || parent.LayoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow)
                {
                    // we dont actually know what size to make it, so just keep it a stock size.
                    constrainingSize.Width = ToolStrip_SEPARATORHEIGHT;
                    constrainingSize.Height = ToolStrip_SEPARATORHEIGHT;
                }
                if (IsVertical)
                {
                    return new Size(ToolStrip_SEPARATORTHICKNESS, constrainingSize.Height);
                }
                else
                {
                    return new Size(constrainingSize.Width, ToolStrip_SEPARATORTHICKNESS);
                }
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (Owner != null && ParentInternal != null)
            {
                Renderer.DrawSeparator(new ToolStripSeparatorRenderEventArgs(e.Graphics, this, IsVertical));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void OnFontChanged(EventArgs e)
        {
            // PERF: dont call base, we dont care if the font changes
            RaiseEvent(EventFontChanged, e);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override bool ShouldSerializeForeColor()
        {
            return (ForeColor != SystemColors.ControlDark);
        }

        internal protected override void SetBounds(Rectangle rect)
        {
            if (Owner is ToolStripDropDownMenu dropDownMenu)
            {

                // Scooch over by the padding amount.  The padding is added to
                // the ToolStripDropDownMenu to keep the non-menu item riffraff
                // aligned to the text rectangle. When flow layout comes through to set our position
                // via IArrangedElement DEFY IT!
                if (dropDownMenu != null)
                {
                    rect.X = 2;
                    rect.Width = dropDownMenu.Width - 4;
                }
            }
            base.SetBounds(rect);
        }

        /// <summary>
        ///  An implementation of AccessibleChild for use with ToolStripItems
        /// </summary>
        [Runtime.InteropServices.ComVisible(true)]
        internal class ToolStripSeparatorAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripSeparator ownerItem = null;

            public ToolStripSeparatorAccessibleObject(ToolStripSeparator ownerItem) : base(ownerItem)
            {
                this.ownerItem = ownerItem;
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = ownerItem.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    else
                    {
                        return AccessibleRole.Separator;
                    }

                }
            }

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_SeparatorControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
