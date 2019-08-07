// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.ButtonInternal;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a
    ///  Windows button.
    /// </summary>
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     SRDescription(nameof(SR.DescriptionButton)),
     Designer("System.Windows.Forms.Design.ButtonBaseDesigner, " + AssemblyRef.SystemDesign)
    ]
    public class Button : ButtonBase, IButtonControl
    {
        /// <summary>
        ///  The dialog result that will be sent to the parent dialog form when
        ///  we are clicked.
        /// </summary>
        private DialogResult dialogResult;

        private const int InvalidDimensionValue = int.MinValue;

        /// <summary>
        ///  For buttons whose FaltStyle = FlatStyle.Flat, this property specifies the size, in pixels
        ///  of the border around the button.
        /// </summary>
        private Size systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);

        /// <summary>
        ///  Initializes a new instance of the <see cref='Button'/>
        ///  class.
        /// </summary>
        public Button() : base()
        {
            // Buttons shouldn't respond to right clicks, so we need to do all our own click logic
            SetStyle(ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick,
                     false);
        }

        /// <summary>
        ///  Allows the control to optionally shrink when AutoSize is true.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(true),
        DefaultValue(AutoSizeMode.GrowOnly),
        Localizable(true),
        SRDescription(nameof(SR.ControlAutoSizeModeDescr))
        ]
        public AutoSizeMode AutoSizeMode
        {
            get
            {
                return GetAutoSizeMode();
            }
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
                        // DefaultLayout does not keep anchor information until it needs to.  When
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

        internal override ButtonBaseAdapter CreateFlatAdapter()
        {
            return new ButtonFlatAdapter(this);
        }

        internal override ButtonBaseAdapter CreatePopupAdapter()
        {
            return new ButtonPopupAdapter(this);
        }

        internal override ButtonBaseAdapter CreateStandardAdapter()
        {
            return new ButtonStandardAdapter(this);
        }

        internal override Size GetPreferredSizeCore(Size proposedConstraints)
        {
            if (FlatStyle != FlatStyle.System)
            {
                Size prefSize = base.GetPreferredSizeCore(proposedConstraints);
                return AutoSizeMode == AutoSizeMode.GrowAndShrink ? prefSize : LayoutUtils.UnionSizes(prefSize, Size);
            }

            if (systemSize.Width == InvalidDimensionValue)
            {
                Size requiredSize;
                // Note: The result from the BCM_GETIDEALSIZE message isn't accurate if the font has been
                // changed, because this method is called before the font is set into the device context.

                requiredSize = TextRenderer.MeasureText(Text, Font);
                requiredSize = SizeFromClientSize(requiredSize);

                // This padding makes FlatStyle.System about the same size as FlatStyle.Standard
                // with an 8px font.
                requiredSize.Width += 14;
                requiredSize.Height += 9;
                systemSize = requiredSize;
            }
            Size paddedSize = systemSize + Padding.Size;
            return AutoSizeMode == AutoSizeMode.GrowAndShrink ? paddedSize : LayoutUtils.UnionSizes(paddedSize, Size);
        }

        /// <summary>
        ///  This is called when creating a window. Inheriting classes can overide
        ///  this to add extra functionality, but should not forget to first call
        ///  base.CreateParams() to make sure the control continues to work
        ///  correctly.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "BUTTON";
                if (GetStyle(ControlStyles.UserPaint))
                {
                    cp.Style |= NativeMethods.BS_OWNERDRAW;
                }
                else
                {
                    cp.Style |= NativeMethods.BS_PUSHBUTTON;
                    if (IsDefault)
                    {
                        cp.Style |= NativeMethods.BS_DEFPUSHBUTTON;
                    }
                }
                return cp;
            }
        }

        /// <summary>
        ///  Gets or sets a value that is returned to the
        ///  parent form when the button
        ///  is clicked.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DialogResult.None),
        SRDescription(nameof(SR.ButtonDialogResultDescr))
        ]
        public virtual DialogResult DialogResult
        {
            get
            {
                return dialogResult;
            }

            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int)DialogResult.No))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DialogResult));
                }
                dialogResult = value;
            }
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseEnter'/> event.
        /// </summary>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
        }

        /// <summary>
        ///  Raises the <see cref='Control.OnMouseLeave'/> event.
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        /// <hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new event MouseEventHandler MouseDoubleClick
        {
            add => base.MouseDoubleClick += value;
            remove => base.MouseDoubleClick -= value;
        }

        /// <summary>
        ///  Notifies the <see cref='Button'/>
        ///  whether it is the default button so that it can adjust its appearance
        ///  accordingly.
        /// </summary>
        public virtual void NotifyDefault(bool value)
        {
            if (IsDefault != value)
            {
                IsDefault = value;
            }
        }

        /// <summary>
        ///  This method actually raises the Click event. Inheriting classes should
        ///  override this if they wish to be notified of a Click event. (This is far
        ///  preferable to actually adding an event handler.) They should not,
        ///  however, forget to call base.onClick(e); before exiting, to ensure that
        ///  other recipients do actually get the event.
        /// </summary>
        protected override void OnClick(EventArgs e)
        {
            Form form = FindForm();
            if (form != null)
            {
                form.DialogResult = dialogResult;
            }

            // accessibility stuff
            //
            AccessibilityNotifyClients(AccessibleEvents.StateChange, -1);
            AccessibilityNotifyClients(AccessibleEvents.NameChange, -1);

            base.OnClick(e);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
            base.OnFontChanged(e);
        }

        /// <summary>
        ///  Raises the <see cref='ButtonBase.OnMouseUp'/> event.
        /// </summary>
        protected override void OnMouseUp(MouseEventArgs mevent)
        {
            if (mevent.Button == MouseButtons.Left && MouseIsPressed)
            {
                bool isMouseDown = base.MouseIsDown;

                if (GetStyle(ControlStyles.UserPaint))
                {
                    //Paint in raised state...
                    ResetFlagsandPaint();
                }
                if (isMouseDown)
                {
                    Point pt = PointToScreen(new Point(mevent.X, mevent.Y));
                    if (UnsafeNativeMethods.WindowFromPoint(pt) == Handle && !ValidationCancelled)
                    {
                        if (GetStyle(ControlStyles.UserPaint))
                        {
                            OnClick(mevent);
                        }
                        OnMouseClick(mevent);
                    }
                }
            }
            base.OnMouseUp(mevent);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
            base.OnTextChanged(e);
        }

        /// <summary>
        ///  When overridden in a derived class, handles rescaling of any magic numbers used in control painting.
        ///  Must call the base class method to get the current DPI values. This method is invoked only when
        ///  Application opts-in into the Per-monitor V2 support, targets .NETFX 4.7 and has
        ///  EnableDpiChangedMessageHandling and EnableDpiChangedHighDpiImprovements config switches turned on.
        /// </summary>
        /// <param name="deviceDpiOld">Old DPI value</param>
        /// <param name="deviceDpiNew">New DPI value</param>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            if (DpiHelper.IsScalingRequirementMet)
            {
                // reset cached boundary size - it needs to be recalculated for new DPI
                systemSize = new Size(InvalidDimensionValue, InvalidDimensionValue);
            }
        }

        /// <summary>
        ///  Generates a <see cref='Control.Click'/> event for a
        ///  button.
        /// </summary>
        public void PerformClick()
        {
            if (CanSelect)
            {
                bool validate = ValidateActiveControl(out bool validatedControlAllowsFocusChange);
                if (!ValidationCancelled && (validate || validatedControlAllowsFocusChange))
                {
                    //Paint in raised state...
                    //
                    ResetFlagsandPaint();
                    OnClick(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Lets a control process mnmemonic characters. Inheriting classes can
        ///  override this to add extra functionality, but should not forget to call
        ///  base.ProcessMnemonic(charCode); to ensure basic functionality
        ///  remains unchanged.
        /// </summary>
        protected internal override bool ProcessMnemonic(char charCode)
        {
            if (UseMnemonic && CanProcessMnemonic() && IsMnemonic(charCode, Text))
            {
                PerformClick();
                return true;
            }
            return base.ProcessMnemonic(charCode);
        }

        /// <summary>
        ///  Provides some interesting information for the Button control in
        ///  String form.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", Text: " + Text;
        }

        /// <summary>
        ///  The button's window procedure.  Inheriting classes can override this
        ///  to add extra functionality, but should not forget to call
        ///  base.wndProc(m); to ensure the button continues to function properly.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case WindowMessages.WM_REFLECT + WindowMessages.WM_COMMAND:
                    if (NativeMethods.Util.HIWORD(m.WParam) == NativeMethods.BN_CLICKED)
                    {
                        Debug.Assert(!GetStyle(ControlStyles.UserPaint), "Shouldn't get BN_CLICKED when UserPaint");
                        if (!ValidationCancelled)
                        {
                            OnClick(EventArgs.Empty);
                        }
                    }
                    break;
                case WindowMessages.WM_ERASEBKGND:
                    DefWndProc(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}


