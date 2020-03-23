// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    [DefaultProperty(nameof(Value))]
    public class ToolStripProgressBar : ToolStripControlHost
    {
        internal static readonly object EventRightToLeftLayoutChanged = new object();

        private static readonly Padding defaultMargin = new Padding(1, 2, 1, 1);
        private static readonly Padding defaultStatusStripMargin = new Padding(1, 3, 1, 3);
        private Padding scaledDefaultMargin = defaultMargin;
        private Padding scaledDefaultStatusStripMargin = defaultStatusStripMargin;

        public ToolStripProgressBar()
            : base(CreateControlInstance())
        {
            if (Control is ToolStripProgressBarControl toolStripProgressBarControl)
            {
                toolStripProgressBarControl.Owner = this;
            }

            if (DpiHelper.IsScalingRequirementMet)
            {
                scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(defaultMargin);
                scaledDefaultStatusStripMargin = DpiHelper.LogicalToDeviceUnits(defaultStatusStripMargin);
            }
        }

        public ToolStripProgressBar(string name) : this()
        {
            Name = name;
        }

        /// <summary>
        ///  Create a strongly typed accessor for the class
        /// </summary>
        /// <value></value>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ProgressBar ProgressBar
        {
            get
            {
                return Control as ProgressBar;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override ImageLayout BackgroundImageLayout
        {
            get => base.BackgroundImageLayout;
            set => base.BackgroundImageLayout = value;
        }

        /// <summary>
        ///  Specify what size you want the item to start out at
        /// </summary>
        /// <value></value>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(100, 15);
            }
        }

        /// <summary>
        ///  Specify how far from the edges you want to be
        /// </summary>
        /// <value></value>
        protected internal override Padding DefaultMargin
        {
            get
            {
                if (Owner != null && Owner is StatusStrip)
                {
                    return scaledDefaultStatusStripMargin;
                }
                else
                {
                    return scaledDefaultMargin;
                }
            }
        }

        [DefaultValue(100)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ProgressBarMarqueeAnimationSpeed))]
        public int MarqueeAnimationSpeed
        {
            get { return ProgressBar.MarqueeAnimationSpeed; }
            set { ProgressBar.MarqueeAnimationSpeed = value; }
        }

        [DefaultValue(100)]
        [SRCategory(nameof(SR.CatBehavior))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.ProgressBarMaximumDescr))]
        public int Maximum
        {
            get
            {
                return ProgressBar.Maximum;
            }
            set
            {
                ProgressBar.Maximum = value;
            }
        }

        [DefaultValue(0)]
        [SRCategory(nameof(SR.CatBehavior))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.ProgressBarMinimumDescr))]
        public int Minimum
        {
            get
            {
                return ProgressBar.Minimum;
            }
            set
            {
                ProgressBar.Minimum = value;
            }
        }

        /// <summary>
        ///  This is used for international applications where the language is written from RightToLeft.
        ///  When this property is true, and the RightToLeft is true, mirroring will be turned on on
        ///  the form, and control placement and text will be from right to left.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Localizable(true)]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))]
        public virtual bool RightToLeftLayout
        {
            get
            {
                return ProgressBar.RightToLeftLayout;
            }

            set
            {
                ProgressBar.RightToLeftLayout = value;
            }
        }

        /// <summary>
        ///  Wrap some commonly used properties
        /// </summary>
        /// <value></value>
        [DefaultValue(10)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ProgressBarStepDescr))]
        public int Step
        {
            get
            {
                return ProgressBar.Step;
            }
            set
            {
                ProgressBar.Step = value;
            }
        }

        /// <summary>
        ///  Wrap some commonly used properties
        /// </summary>
        /// <value></value>
        [DefaultValue(ProgressBarStyle.Blocks)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ProgressBarStyleDescr))]
        public ProgressBarStyle Style
        {
            get
            {
                return ProgressBar.Style;
            }
            set
            {
                ProgressBar.Style = value;
            }
        }

        /// <summary>
        ///  Hide the property.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override string Text
        {
            get
            {
                return Control.Text;
            }
            set
            {
                Control.Text = value;
            }
        }

        /// <summary>
        ///  Wrap some commonly used properties
        /// </summary>
        /// <value></value>
        [DefaultValue(0)]
        [SRCategory(nameof(SR.CatBehavior))]
        [Bindable(true)]
        [SRDescription(nameof(SR.ProgressBarValueDescr))]
        public int Value
        {
            get
            {
                return ProgressBar.Value;
            }
            set
            {
                ProgressBar.Value = value;
            }
        }

        private static Control CreateControlInstance()
        {
            ProgressBar progressBar = new ToolStripProgressBarControl
            {
                Size = new Size(100, 15)
            };
            return progressBar;
        }

        private void HandleRightToLeftLayoutChanged(object sender, EventArgs e)
        {
            OnRightToLeftLayoutChanged(e);
        }

        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            RaiseEvent(EventRightToLeftLayoutChanged, e);
        }

        protected override void OnSubscribeControlEvents(Control control)
        {
            if (control is ProgressBar bar)
            {
                // Please keep this alphabetized and in sync with Unsubscribe
                //
                bar.RightToLeftLayoutChanged += new EventHandler(HandleRightToLeftLayoutChanged);
            }

            base.OnSubscribeControlEvents(control);
        }

        protected override void OnUnsubscribeControlEvents(Control control)
        {
            if (control is ProgressBar bar)
            {
                // Please keep this alphabetized and in sync with Subscribe
                //
                bar.RightToLeftLayoutChanged -= new EventHandler(HandleRightToLeftLayoutChanged);
            }
            base.OnUnsubscribeControlEvents(control);
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyPressEventHandler KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event KeyEventHandler KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }
        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler LocationChanged
        {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler OwnerChanged
        {
            add => base.OwnerChanged += value;
            remove => base.OwnerChanged -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => Events.AddHandler(EventRightToLeftLayoutChanged, value);
            remove => Events.RemoveHandler(EventRightToLeftLayoutChanged, value);
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler Validated
        {
            add => base.Validated += value;
            remove => base.Validated -= value;
        }

        /// <summary>
        ///  Hide the event.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event CancelEventHandler Validating
        {
            add => base.Validating += value;
            remove => base.Validating -= value;
        }
        public void Increment(int value)
        {
            ProgressBar.Increment(value);
        }

        public void PerformStep()
        {
            ProgressBar.PerformStep();
        }

        internal class ToolStripProgressBarControl : ProgressBar
        {
            private ToolStripProgressBar ownerItem;

            public ToolStripProgressBar Owner
            {
                get { return ownerItem; }
                set { ownerItem = value; }
            }

            internal override bool SupportsUiaProviders => true;

            protected override AccessibleObject CreateAccessibilityInstance()
            {
                return new ToolStripProgressBarControlAccessibleObject(this);
            }
        }

        internal class ToolStripProgressBarControlAccessibleObject : ProgressBar.ProgressBarAccessibleObject
        {
            public ToolStripProgressBarControlAccessibleObject(ToolStripProgressBarControl toolStripProgressBarControl) : base(toolStripProgressBarControl)
            {
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    if (Owner is ToolStripProgressBarControl toolStripProgressBarControl)
                    {
                        return toolStripProgressBarControl.Owner.Owner.AccessibilityObject;
                    }

                    return base.FragmentRoot;
                }
            }

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                    case UiaCore.NavigateDirection.PreviousSibling:
                    case UiaCore.NavigateDirection.NextSibling:
                        if (Owner is ToolStripProgressBarControl toolStripProgressBarControl)
                        {
                            return toolStripProgressBarControl.Owner.AccessibilityObject.FragmentNavigate(direction);
                        }
                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
