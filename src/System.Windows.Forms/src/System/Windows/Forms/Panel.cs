// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a <see cref='Panel'/> control.
    /// </summary>
    [DefaultProperty(nameof(BorderStyle))]
    [DefaultEvent(nameof(Paint))]
    [Docking(DockingBehavior.Ask)]
    [Designer("System.Windows.Forms.Design.PanelDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionPanel))]
    public class Panel : ScrollableControl
    {
        private BorderStyle _borderStyle = BorderStyle.None;

        /// <summary>
        ///  Initializes a new instance of the <see cref='Panel'/> class.
        /// </summary>
        public Panel() : base()
        {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);
            TabStop = false;
            SetStyle(ControlStyles.Selectable | ControlStyles.AllPaintingInWmPaint, false);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
        }

        /// <summary>
        ///  Override to re-expose AutoSize.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get => base.AutoSize;
            set => base.AutoSize = value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        [SRDescription(nameof(SR.ControlAutoSizeModeDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(true)]
        [DefaultValue(AutoSizeMode.GrowOnly)]
        [Localizable(true)]
        public virtual AutoSizeMode AutoSizeMode
        {
            get => GetAutoSizeMode();
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }

                if (GetAutoSizeMode() != value)
                {
                    SetAutoSizeMode(value);
                    if (ParentInternal != null)
                    {
                        // DefaultLayout does not keep anchor information until it needs to. When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if (ParentInternal.LayoutEngine == DefaultLayout.Instance)
                        {
                            ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates the border style for the control.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.None)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.PanelBorderStyleDescr))]
        public BorderStyle BorderStyle
        {
            get => _borderStyle;
            set
            {
                if (_borderStyle != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    _borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <summary>
        ///  Returns the parameters needed to create the handle. Inheriting classes can override
        ///  this to provide extra functionality. They should not, however, forget to call
        ///  base.getCreateParams() first to get the struct filled up with the basic info.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.Style &= ~(int)User32.WS.BORDER;
                cp.ExStyle |= (int)User32.WS_EX.CONTROLPARENT;
                cp.ExStyle &= ~(int)User32.WS_EX.CLIENTEDGE;

                switch (_borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= (int)User32.WS_EX.CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= (int)User32.WS.BORDER;
                        break;
                }
                return cp;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize => new Size(200, 100);

        internal override Size GetPreferredSizeCore(Size proposedSize)
        {
            // Translating 0,0 from ClientSize to actual Size tells us how much space
            // is required for the borders.
            Size borderSize = SizeFromClientSize(Size.Empty);
            Size totalPadding = borderSize + Padding.Size;

            return LayoutEngine.GetPreferredSize(this, proposedSize - totalPadding) + totalPadding;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyUp
        {
            add => base.KeyUp += value;
            remove => base.KeyUp -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown
        {
            add => base.KeyDown += value;
            remove => base.KeyDown -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler KeyPress
        {
            add => base.KeyPress += value;
            remove => base.KeyPress -= value;
        }

        [DefaultValue(false)]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  Fires the event indicating that the panel has been resized.
        ///  Inheriting controls should use this in favour of actually listening to
        ///  the event, but should not forget to call base.onResize() to
        ///  ensure that the event is still fired for external listeners.
        /// </summary>
        protected override void OnResize(EventArgs eventargs)
        {
            if (DesignMode && _borderStyle == BorderStyle.None)
            {
                Invalidate();
            }

            base.OnResize(eventargs);
        }

        private protected override void PrintToMetaFileRecursive(Gdi32.HDC hDC, IntPtr lParam, Rectangle bounds)
        {
            base.PrintToMetaFileRecursive(hDC, lParam, bounds);

            using var mapping = new DCMapping(hDC, bounds);
            using Graphics g = hDC.CreateGraphics();
            ControlPaint.PrintBorder(g, new Rectangle(Point.Empty, Size), BorderStyle, Border3DStyle.Sunken);
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", BorderStyle: " + typeof(BorderStyle).ToString() + "." + _borderStyle.ToString();
        }
    }
}
