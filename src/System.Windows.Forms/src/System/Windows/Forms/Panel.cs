// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;

    using System.Diagnostics;

    using System;
    using System.Security.Permissions;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Windows.Forms.Layout;

    /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Represents a <see cref='System.Windows.Forms.Panel'/>
    ///       control.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(BorderStyle)),
    DefaultEvent(nameof(Paint)),
    Docking(DockingBehavior.Ask),
    Designer("System.Windows.Forms.Design.PanelDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionPanel))
    ]
    public class Panel : ScrollableControl {

        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.Panel"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Panel'/> class.</para>
        /// </devdoc>
        public Panel()
        : base() {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);  
            TabStop = false;
            SetStyle(ControlStyles.Selectable |
                     ControlStyles.AllPaintingInWmPaint, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.AutoSize"]/*' />
        /// <devdoc>
        ///    <para> Override to re-expose AutoSize.</para>
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.AutoSizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
        {
            add
            {
                base.AutoSizeChanged += value;
            }
            remove
            {
                base.AutoSizeChanged -= value;
            }
        }

        /// <devdoc>
        ///     Allows the control to optionally shrink when AutoSize is true.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ControlAutoSizeModeDescr)),
        SRCategory(nameof(SR.CatLayout)),        
        Browsable(true),
        DefaultValue(AutoSizeMode.GrowOnly),
        Localizable(true)
        ]
        public virtual AutoSizeMode AutoSizeMode {
            get {
                return GetAutoSizeMode();
            }
            set {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }
                if (GetAutoSizeMode() != value) {
                    SetAutoSizeMode(value);
                    if(ParentInternal != null) {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if(ParentInternal.LayoutEngine == DefaultLayout.Instance) {
                            ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.BorderStyle"]/*' />
        /// <devdoc>
        ///    <para> Indicates the
        ///       border style for the control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.None),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.PanelBorderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                if (borderStyle != value) {
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Returns the parameters needed to create the handle.  Inheriting classes
        ///    can override this to provide extra functionality.  They should not,
        ///    however, forget to call base.getCreateParams() first to get the struct
        ///    filled up with the basic info.
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= NativeMethods.WS_EX_CONTROLPARENT;

                cp.ExStyle &= (~NativeMethods.WS_EX_CLIENTEDGE);
                cp.Style &= (~NativeMethods.WS_BORDER);

                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }
                return cp;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(200, 100);
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedSize) {
            // Translating 0,0 from ClientSize to actual Size tells us how much space
            // is required for the borders.
            Size borderSize = SizeFromClientSize(Size.Empty);
            Size totalPadding = borderSize + Padding.Size;

            return LayoutEngine.GetPreferredSize(this, proposedSize - totalPadding) + totalPadding;
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.KeyUp"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyUp {
            add {
                base.KeyUp += value;
            }
            remove {
                base.KeyUp -= value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.KeyDown"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown {
            add {
                base.KeyDown += value;
            }
            remove {
                base.KeyDown -= value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.KeyPress"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler KeyPress {
            add {
                base.KeyPress += value;
            }
            remove {
                base.KeyPress -= value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.TabStop"]/*' />
        /// <devdoc>
        /// </devdoc>
        [DefaultValue(false)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.Text"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.OnResize"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Fires the event indicating that the panel has been resized.
        ///       Inheriting controls should use this in favour of actually listening to
        ///       the event, but should not forget to call base.onResize() to
        ///       ensure that the event is still fired for external listeners.</para>
        /// </devdoc>
        protected override void OnResize(EventArgs eventargs) {
            if (DesignMode && borderStyle == BorderStyle.None) {
                Invalidate();
            }
            base.OnResize(eventargs);
        }
    

        internal override void PrintToMetaFileRecursive(HandleRef hDC, IntPtr lParam, Rectangle bounds) {
            base.PrintToMetaFileRecursive(hDC, lParam, bounds);

            using (WindowsFormsUtils.DCMapping mapping = new WindowsFormsUtils.DCMapping(hDC, bounds)) {
                using(Graphics g = Graphics.FromHdcInternal(hDC.Handle)) {
                    ControlPaint.PrintBorder(g, new Rectangle(Point.Empty, Size), BorderStyle, Border3DStyle.Sunken);
                }
            }
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.StringFromBorderStyle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private static string StringFromBorderStyle(BorderStyle value) {      
            Type borderStyleType = typeof(BorderStyle);
            return (ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D)) ? (borderStyleType.ToString() + "." + value.ToString()) : "[Invalid BorderStyle]";
        }

        /// <include file='doc\Panel.uex' path='docs/doc[@for="Panel.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", BorderStyle: " + StringFromBorderStyle(borderStyle);
        }

    }
}
