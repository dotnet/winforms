// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using System.Security;
    using System.Security.Permissions;


    [DefaultProperty(nameof(Value))]
    public class ToolStripProgressBar : ToolStripControlHost {

        internal static readonly object EventRightToLeftLayoutChanged = new object();

        private static readonly Padding defaultMargin = new Padding(1, 2, 1, 1);
        private static readonly Padding defaultStatusStripMargin = new Padding(1, 3, 1, 3);
        private Padding scaledDefaultMargin = defaultMargin;
        private Padding scaledDefaultStatusStripMargin = defaultStatusStripMargin;



        public ToolStripProgressBar()
            : base(CreateControlInstance()) {
            ToolStripProgressBarControl toolStripProgressBarControl = Control as ToolStripProgressBarControl;
            if (toolStripProgressBarControl != null) {
                toolStripProgressBarControl.Owner = this;
            }

            if (DpiHelper.IsScalingRequirementMet) {
                scaledDefaultMargin = DpiHelper.LogicalToDeviceUnits(defaultMargin);
                scaledDefaultStatusStripMargin = DpiHelper.LogicalToDeviceUnits(defaultStatusStripMargin);
            }
        }

        public ToolStripProgressBar(string name) : this() {
            this.Name = name;
        }


        /// <devdoc>
        /// Create a strongly typed accessor for the class
        /// </devdoc>
        /// <value></value>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ProgressBar ProgressBar {
            get {
                return this.Control as ProgressBar;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }


        /// <devdoc>
        /// Specify what size you want the item to start out at
        /// </devdoc>
        /// <value></value>
        protected override System.Drawing.Size DefaultSize {
            get {              
                return new Size(100,15);
            }
        }


        /// <devdoc>
        /// Specify how far from the edges you want to be
        /// </devdoc>
        /// <value></value>
        protected internal override Padding DefaultMargin {
            get {
                if (this.Owner != null && this.Owner is StatusStrip) {
                    return scaledDefaultStatusStripMargin;
                }
                else {
                    return scaledDefaultMargin;
                }
            }
        }

        [
        DefaultValue(100),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ProgressBarMarqueeAnimationSpeed))
        ]
        public int MarqueeAnimationSpeed {
            get { return ProgressBar.MarqueeAnimationSpeed; }
            set { ProgressBar.MarqueeAnimationSpeed = value; }
        }


        [
       DefaultValue(100),
       SRCategory(nameof(SR.CatBehavior)),
       RefreshProperties(RefreshProperties.Repaint),
       SRDescription(nameof(SR.ProgressBarMaximumDescr))
       ]
        public int Maximum {
            get {
                return ProgressBar.Maximum;
            }
            set {
                ProgressBar.Maximum = value;
            }
        }
        [
        DefaultValue(0),
        SRCategory(nameof(SR.CatBehavior)),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.ProgressBarMinimumDescr))
        ]
        public int Minimum {
            get {
                return ProgressBar.Minimum;
            }
            set {
                ProgressBar.Minimum = value;
            }
        }


        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))
        ]
        public virtual bool RightToLeftLayout {
            get {

                return ProgressBar.RightToLeftLayout;
            }

            set {
                ProgressBar.RightToLeftLayout = value;
            }
        }


        /// <devdoc>
        /// Wrap some commonly used properties
        /// </devdoc>
        /// <value></value>
        [
        DefaultValue(10),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ProgressBarStepDescr))
        ]
        public int Step {
            get {
                return ProgressBar.Step;
            }
            set {
                ProgressBar.Step = value;
            }
        }


        /// <devdoc>
        /// Wrap some commonly used properties
        /// </devdoc>
        /// <value></value>
        [
        DefaultValue(ProgressBarStyle.Blocks),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ProgressBarStyleDescr))
        ]
        public ProgressBarStyle Style {
            get {
                return ProgressBar.Style;
            }
            set {
                ProgressBar.Style = value;
            }
        }


        /// <devdoc>
        /// Hide the property.
        /// </devdoc>
        [
        Browsable(false), 
        EditorBrowsable(EditorBrowsableState.Never), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
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



        /// <devdoc>
        /// Wrap some commonly used properties
        /// </devdoc>
        /// <value></value>
        [
        DefaultValue(0),
        SRCategory(nameof(SR.CatBehavior)),
        Bindable(true),
        SRDescription(nameof(SR.ProgressBarValueDescr))
        ]
        public int Value {
            get {
                return ProgressBar.Value;
            }
            set {
                ProgressBar.Value = value;
            }
        }

        /// <summary>
        ///     Constructs the new instance of the accessibility object for this ToolStripProgressBar ToolStrip item.
        /// </summary>
        /// <returns>
        ///     The new instance of the accessibility object for this ToolStripProgressBar ToolStrip item
        /// </returns>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance() {
            if (AccessibilityImprovements.Level3) {
                return new ToolStripProgressBarAccessibleObject(this);
            }

            return base.CreateAccessibilityInstance();
        }

        private static Control CreateControlInstance() {
            ProgressBar progressBar = AccessibilityImprovements.Level3 ? new ToolStripProgressBarControl() : new ProgressBar();
            progressBar.Size = new Size(100,15);
            return progressBar;
        }

        private void HandleRightToLeftLayoutChanged(object sender, EventArgs e) {
            OnRightToLeftLayoutChanged(e);
        }


        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            RaiseEvent(EventRightToLeftLayoutChanged, e);
        }


        protected override void OnSubscribeControlEvents(Control control) {
            ProgressBar bar = control as ProgressBar;
            if (bar != null) {
                // Please keep this alphabetized and in sync with Unsubscribe
                // 
                bar.RightToLeftLayoutChanged += new EventHandler(HandleRightToLeftLayoutChanged);
            }

            base.OnSubscribeControlEvents(control);
        }


        protected override void OnUnsubscribeControlEvents(Control control) {

            ProgressBar bar = control as ProgressBar;
            if (bar != null) {
                // Please keep this alphabetized and in sync with Subscribe
                // 
                bar.RightToLeftLayoutChanged -= new EventHandler(HandleRightToLeftLayoutChanged);
            }
            base.OnUnsubscribeControlEvents(control);

        }


        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event KeyEventHandler KeyDown
        {
            add
            {
                base.KeyDown += value;
            }
            remove
            {
                base.KeyDown -= value;
            }
        }


        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event KeyPressEventHandler KeyPress
        {
            add
            {
                base.KeyPress += value;
            }
            remove
            {
                base.KeyPress -= value;
            }
        }


        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event KeyEventHandler KeyUp
        {
            add
            {
                base.KeyUp += value;
            }
            remove
            {
                base.KeyUp -= value;
            }
        }

        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler LocationChanged
        {
            add
            {
                base.LocationChanged += value;
            }
            remove
            {
                base.LocationChanged -= value;
            }
        }


        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler OwnerChanged
        {
            add
            {
                base.OwnerChanged += value;
            }
            remove
            {
                base.OwnerChanged -= value;
            }
        }


        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add {
                Events.AddHandler(EventRightToLeftLayoutChanged, value);
            }
            remove {
                Events.RemoveHandler(EventRightToLeftLayoutChanged, value);
            }
        }


        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler TextChanged
        {
            add
            {
                base.TextChanged += value;
            }
            remove
            {
                base.TextChanged -= value;
            }
        }



        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event EventHandler Validated
        {
            add
            {
                base.Validated += value;
            }
            remove
            {
                base.Validated -= value;
            }
        }


        /// <devdoc>
        /// <para>Hide the event.</para>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        new public event CancelEventHandler Validating
        {
            add
            {
                base.Validating += value;
            }
            remove
            {
                base.Validating -= value;
            }
        }
        public void Increment(int value) {
            ProgressBar.Increment(value);
        }

        public void PerformStep() {
            ProgressBar.PerformStep();
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        internal class ToolStripProgressBarAccessibleObject : ToolStripItemAccessibleObject {
            private ToolStripProgressBar ownerItem = null;

            public ToolStripProgressBarAccessibleObject(ToolStripProgressBar ownerItem) : base(ownerItem) {
              this.ownerItem = ownerItem;
            }
         
            public override AccessibleRole Role {
               get {
                   AccessibleRole role = Owner.AccessibleRole;
                   if (role != AccessibleRole.Default) {
                       return role;
                   }

                   return AccessibleRole.ProgressBar;
               }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                if (direction == UnsafeNativeMethods.NavigateDirection.FirstChild ||
                    direction == UnsafeNativeMethods.NavigateDirection.LastChild) {
                    return this.ownerItem.ProgressBar.AccessibilityObject;
                }

                // Handle Parent and other directions in base ToolStripItem.FragmentNavigate() method.
                return base.FragmentNavigate(direction);
            }
        }

        internal class ToolStripProgressBarControl : ProgressBar {

            private ToolStripProgressBar ownerItem;

            public ToolStripProgressBar Owner {
                get { return ownerItem; }
                set { ownerItem = value; }
            }

            internal override bool SupportsUiaProviders {
                get {
                    return AccessibilityImprovements.Level3;
                }
            }

            protected override AccessibleObject CreateAccessibilityInstance() {
                if (AccessibilityImprovements.Level3) {
                    return new ToolStripProgressBarControlAccessibleObject(this);
                }

                return base.CreateAccessibilityInstance();
            }
        }

        internal class ToolStripProgressBarControlAccessibleObject : ProgressBar.ProgressBarAccessibleObject {
            public ToolStripProgressBarControlAccessibleObject(ToolStripProgressBarControl toolStripProgressBarControl) : base(toolStripProgressBarControl) {
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot {
                get {
                    var toolStripProgressBarControl = this.Owner as ToolStripProgressBarControl;
                    if (toolStripProgressBarControl != null) {
                        return toolStripProgressBarControl.Owner.Owner.AccessibilityObject;
                    }

                    return base.FragmentRoot;
                }
            }

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                switch (direction) {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        var toolStripProgressBarControl = Owner as ToolStripProgressBarControl;
                        if (toolStripProgressBarControl != null) {
                            return toolStripProgressBarControl.Owner.AccessibilityObject.FragmentNavigate(direction);
                        }
                        break;
                }

                return base.FragmentNavigate(direction);
            }

            internal override object GetPropertyValue(int propertyID) {
                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
