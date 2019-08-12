// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms.Design;

    /// <devdoc/>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolStripButton : ToolStripItem
    {
        private CheckState checkState = CheckState.Unchecked;
        private bool checkOnClick = false;
        private const int STANDARD_BUTTON_WIDTH = 23;
        private int standardButtonWidth = STANDARD_BUTTON_WIDTH;

        private static readonly object EventCheckedChanged = new object();
        private static readonly object EventCheckStateChanged = new object();

        /// <summary>
        ///  Summary of ToolStripButton.
        /// </summary>
        public ToolStripButton()
        {
            Initialize();
        }
        public ToolStripButton(string text) : base(text, null, null)
        {
            Initialize();
        }
        public ToolStripButton(Image image) : base(null, image, null)
        {
            Initialize();
        }
        public ToolStripButton(string text, Image image) : base(text, image, null)
        {
            Initialize();
        }
        public ToolStripButton(string text, Image image, EventHandler onClick) : base(text, image, onClick)
        {
            Initialize();
        }
        public ToolStripButton(string text, Image image, EventHandler onClick, string name) : base(text, image, onClick, name)
        {
            Initialize();
        }

        [DefaultValue(true)]
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

        /// <summary>
        ///  Summary of CanSelect.
        /// </summary>
        public override bool CanSelect
        {
            get
            {
                return true;
            }
        }

        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ToolStripButtonCheckOnClickDescr))
        ]
        public bool CheckOnClick
        {
            get
            {
                return checkOnClick;
            }
            set
            {
                checkOnClick = value;
            }
        }

        /// <summary>
            ///  Gets or sets a value indicating whether the item is checked.
            /// </summary>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripButtonCheckedDescr))
        ]
        public bool Checked
        {
            get
            {
                return checkState != CheckState.Unchecked;
            }

            set
            {
                if (value != Checked)
                {
                    CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                    InvokePaint();

                }
            }
        }

        /// <summary>
        ///  Gets
        ///  or sets a value indicating whether the check box is checked.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(CheckState.Unchecked),
        SRDescription(nameof(SR.CheckBoxCheckStateDescr))
        ]
        public CheckState CheckState
        {
            get
            {
                return checkState;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)CheckState.Unchecked, (int)CheckState.Indeterminate))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CheckState));
                }

                if (value != checkState)
                {
                    checkState = value;
                    Invalidate();
                    OnCheckedChanged(EventArgs.Empty);
                    OnCheckStateChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Occurs when the
        ///  value of the <see cref='CheckBox.Checked'/>
        ///  property changes.
        /// </summary>
        [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
        public event EventHandler CheckedChanged
        {
            add => Events.AddHandler(EventCheckedChanged, value);
            remove => Events.RemoveHandler(EventCheckedChanged, value);
        }
        /// <summary>
        ///  Occurs when the
        ///  value of the <see cref='CheckBox.CheckState'/>
        ///  property changes.
        /// </summary>
        [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
        public event EventHandler CheckStateChanged
        {
            add => Events.AddHandler(EventCheckStateChanged, value);
            remove => Events.RemoveHandler(EventCheckStateChanged, value);
        }

        protected override bool DefaultAutoToolTip
        {
            get
            {
                return true;
            }
        }

        internal override int DeviceDpi
        {
            get => base.DeviceDpi;

            // This gets called via ToolStripItem.RescaleConstantsForDpi.
            // It's practically calling Initialize on DpiChanging with the new Dpi value.
            // ToolStripItem.RescaleConstantsForDpi is already behind quirks.
            set
            {
                if (base.DeviceDpi != value)
                {
                    base.DeviceDpi = value;
                    standardButtonWidth = DpiHelper.LogicalToDeviceUnits(STANDARD_BUTTON_WIDTH, DeviceDpi);
                }
            }
        }

        /// <include file='doc\ToolStripButton.uex' path='docs/doc[@for="ToolStripButton.CreateAccessibilityInstance"]/*' />
        /// <devdoc>
        ///  constructs the new instance of the accessibility object for this ToolStripItem. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new ToolStripButtonAccessibleObject(this);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size prefSize = base.GetPreferredSize(constrainingSize);
            prefSize.Width = Math.Max(prefSize.Width, standardButtonWidth);
            return prefSize;
        }

        /// <summary>
        ///  Called by all constructors of ToolStripButton.
        /// </summary>
        private void Initialize()
        {
            SupportsSpaceKey = true;
            if (DpiHelper.IsScalingRequirementMet)
            {
                standardButtonWidth = DpiHelper.LogicalToDeviceUnitsX(STANDARD_BUTTON_WIDTH);
            }
        }

        /// <summary>
        ///  Raises the <see cref='ToolStripMenuItem.CheckedChanged'/>
        ///  event.
        /// </summary>
        protected virtual void OnCheckedChanged(EventArgs e)
        {
            ((EventHandler)Events[EventCheckedChanged])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='ToolStripMenuItem.CheckStateChanged'/> event.
        /// </summary>
        protected virtual void OnCheckStateChanged(EventArgs e)
        {
            AccessibilityNotifyClients(AccessibleEvents.StateChange);
            ((EventHandler)Events[EventCheckStateChanged])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            if (Owner != null)
            {
                ToolStripRenderer renderer = Renderer;

                renderer.DrawButtonBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

                if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                {
                    ToolStripItemImageRenderEventArgs rea = new ToolStripItemImageRenderEventArgs(e.Graphics, this, InternalLayout.ImageRectangle)
                    {
                        ShiftOnPress = true
                    };
                    renderer.DrawItemImage(rea);
                }

                if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                {
                    renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(e.Graphics, this, Text, InternalLayout.TextRectangle, ForeColor, Font, InternalLayout.TextFormat));
                }
            }
        }

        protected override void OnClick(EventArgs e)
        {
            if (checkOnClick)
            {
                Checked = !Checked;
            }
            base.OnClick(e);
        }

        /// <summary>
        ///  An implementation of AccessibleChild for use with ToolStripItems
        /// </summary>
        [Runtime.InteropServices.ComVisible(true)]
        internal class ToolStripButtonAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripButton ownerItem = null;

            public ToolStripButtonAccessibleObject(ToolStripButton ownerItem) : base(ownerItem)
            {
                this.ownerItem = ownerItem;
            }

            internal override object GetPropertyValue(int propertyID)
            {
                switch (propertyID)
                {
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_ButtonControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override AccessibleRole Role
            {
                get
                {
                    if (ownerItem.CheckOnClick)
                    {
                        return AccessibleRole.CheckButton;
                    }
                    else
                    {
                        return base.Role;
                    }
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (ownerItem.Enabled && ownerItem.Checked)
                    {
                        return base.State | AccessibleStates.Checked;
                    }

                    // Disabled ToolStripButton, that is selected, must have focus state so that Narrator can announce it
                    if (!ownerItem.Enabled && ownerItem.Selected)
                    {
                        return base.State | AccessibleStates.Focused;
                    }

                    return base.State;
                }
            }

        }
    }
}

