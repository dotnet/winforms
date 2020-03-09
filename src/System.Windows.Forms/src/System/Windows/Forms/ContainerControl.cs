﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Defines a base class for controls that can parent other controls.
    /// </summary>
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class ContainerControl : ScrollableControl, IContainerControl
    {
        private Control _activeControl;

        /// <remarks>
        ///  The current focused control. Do not directly edit this value.
        /// </remarks>
        private Control _focusedControl;

        /// <remarks>
        ///  The last control that requires validation. Do not directly edit this value.
        /// </remarks>
        private Control _unvalidatedControl;

        /// <summary>
        ///  Indicates whether automatic validation is turned on.
        /// </summary>
        private AutoValidate _autoValidate = AutoValidate.Inherit;

        private EventHandler _autoValidateChanged;

        private SizeF _autoScaleDimensions = SizeF.Empty;

        private SizeF _currentAutoScaleDimensions = SizeF.Empty;

        private AutoScaleMode _autoScaleMode = AutoScaleMode.Inherit;

        private BitVector32 _state = new BitVector32();

        /// <summary>
        ///  True if we need to perform scaling when layout resumes
        /// </summary>
        private static readonly int s_stateScalingNeededOnLayout = BitVector32.CreateMask();

        /// <summary>
        ///  Indicates whether we're currently state[stateValidating].
        /// </summary>
        private static readonly int s_stateValidating = BitVector32.CreateMask(s_stateScalingNeededOnLayout);

        /// <summary>
        ///  Indicates whether we or one of our children is currently processing a mnemonic.
        /// </summary>
        private static readonly int s_stateProcessingMnemonic = BitVector32.CreateMask(s_stateValidating);

        /// <summary>
        ///  True while we are scaling a child control
        /// </summary>
        private static readonly int s_stateScalingChild = BitVector32.CreateMask(s_stateProcessingMnemonic);

        /// <summary>
        ///  Flagged when a parent changes so we can adpat our scaling logic to match.
        /// </summary>
        private static readonly int s_stateParentChanged = BitVector32.CreateMask(s_stateScalingChild);

        private static readonly int s_propAxContainer = PropertyStore.CreateKey();

        private const string FontMeasureString = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        ///  Initializes a new instance of the <see cref="ContainerControl"/> class.
        /// </summary>
        public ContainerControl() : base()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, false);

            // This class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);
        }

        /// <summary>
        ///  AutoScaleDimensions represents the DPI or Font setting that the control has been scaled
        ///  to or designed at. Specifically, at design time this property will be set by the
        ///  designer to the value that the developer is designing at. Then, at runtime, when the
        ///  form loads if the CurrentAutoScaleDimensions are different from the AutoScaleDimensions,
        ///  PerformAutoScale will be called and AutoScaleDimensions will be set to the new value to
        ///  match the CurrentAutoScaleDimensions by PerformAutoScale.
        /// </summary>
        [Localizable(true)]
        [Browsable(false)]
        [SRCategory(nameof(SR.CatLayout))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SizeF AutoScaleDimensions
        {
            get => _autoScaleDimensions;
            set
            {
                if (value.Width < 0 || value.Height < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, SR.ContainerControlInvalidAutoScaleDimensions);
                }

                _autoScaleDimensions = value;
                if (!_autoScaleDimensions.IsEmpty)
                {
                    LayoutScalingNeeded();
                }
            }
        }

        /// <summary>
        ///  AutoScaleFactor represents the scaling factor difference between
        ///  CurrentAutoScaleDimensions and AutoScaleDimensions. This value is
        ///  calculated on the fly. Eg: If CurrentAutoScaleDimensions is 192, 192
        ///  and AutoScaleDimensions is 96, 96 then the AutoScaleFactor is 2.0, 2.0
        /// </summary>
        protected SizeF AutoScaleFactor
        {
            get
            {
                SizeF current = CurrentAutoScaleDimensions;
                SizeF saved = AutoScaleDimensions;

                // If no one has configured auto scale dimensions yet, the scaling factor
                // is the unit scale.
                if (saved.IsEmpty)
                {
                    return new SizeF(1.0F, 1.0F);
                }

                return new SizeF(current.Width / saved.Width, current.Height / saved.Height);
            }
        }

        /// <summary>
        ///  Determines the scaling mode of this control. The default is no scaling.
        ///  Scaling by Font is useful if you wish to have a control
        ///  or form stretch or shrink according to the size of the fonts in the system, and should
        ///  be used when the control or form's size itself does not matter.
        ///  Scaling by DPI is useful when you wish to keep a control or form a specific size
        ///  independent of font. for example, a control displaying a chart or other graphic
        ///  may want to use DPI scaling to increase in size to account for higher DPI monitors.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.ContainerControlAutoScaleModeDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public AutoScaleMode AutoScaleMode
        {
            get => _autoScaleMode;
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoScaleMode.None, (int)AutoScaleMode.Inherit))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoScaleMode));
                }

                bool scalingNeeded = false;
                if (value != _autoScaleMode)
                {
                    // Invalidate any current scaling factors. If we are changing AutoScaleMode to
                    // anything other than its default, we should clear out autoScaleDimensions as
                    // it is nonsensical.
                    if (_autoScaleMode != AutoScaleMode.Inherit)
                    {
                        _autoScaleDimensions = SizeF.Empty;
                    }

                    _currentAutoScaleDimensions = SizeF.Empty;
                    _autoScaleMode = value;
                    scalingNeeded = true;
                }

                OnAutoScaleModeChanged();
                if (scalingNeeded)
                {
                    LayoutScalingNeeded();
                }
            }
        }

        /// <summary>
        ///  Indicates whether controls in this container will be automatically validated when the focus changes.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [AmbientValue(AutoValidate.Inherit)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ContainerControlAutoValidate))]
        public virtual AutoValidate AutoValidate
        {
            get
            {
                if (_autoValidate != AutoValidate.Inherit)
                {
                    return _autoValidate;
                }

                return GetAutoValidateForControl(this);
            }
            set
            {
                if (value < AutoValidate.Inherit || value > AutoValidate.EnableAllowFocusChange)
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoValidate));
                }

                if (_autoValidate == value)
                {
                    return;
                }

                _autoValidate = value;
                OnAutoValidateChanged(EventArgs.Empty);
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ContainerControlOnAutoValidateChangedDescr))]
        public event EventHandler AutoValidateChanged
        {
            add => _autoValidateChanged += value;
            remove => _autoValidateChanged -= value;
        }

        /// <summary>
        ///  The binding manager for the container control.
        /// </summary>
        [Browsable(false)]
        [SRDescription(nameof(SR.ContainerControlBindingContextDescr))]
        public override BindingContext BindingContext
        {
            get
            {
                BindingContext bm = base.BindingContext;
                if (bm == null)
                {
                    bm = new BindingContext();
                    BindingContext = bm;
                }

                return bm;
            }
            set => base.BindingContext = value;
        }

        /// <summary>
        ///  Container controls support ImeMode only to allow child controls to inherit it from their parents.
        /// </summary>
        protected override bool CanEnableIme
        {
            get
            {
                // Note: If overriding this property make sure to copy the Debug code and call this method.

                Debug.Indent();
                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CanEnableIme(), value = false" + ", this = " + this);
                Debug.Unindent();

                return false;
            }
        }

        /// <summary>
        ///  Indicates the current active control on the container control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ContainerControlActiveControlDescr))]
        public Control ActiveControl
        {
            get => _activeControl;
            set => SetActiveControl(value);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= (int)User32.WS_EX.CONTROLPARENT;
                return cp;
            }
        }

        /// <summary>
        ///  Represent the actual DPI or Font settings of the display at runtime. If the AutoScaleMode
        ///  is set to 'None' then the CurrentAutoScaleDimensions is equal to the ActualScaleDimensions.
        /// </summary>
        [Browsable(false)]
        [SRCategory(nameof(SR.CatLayout))]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public SizeF CurrentAutoScaleDimensions
        {
            get
            {
                if (_currentAutoScaleDimensions.IsEmpty)
                {
                    switch (AutoScaleMode)
                    {
                        case AutoScaleMode.Font:
                            _currentAutoScaleDimensions = GetFontAutoScaleDimensions();
                            break;

                        case AutoScaleMode.Dpi:
                            // Screen Dpi
                            if (DpiHelper.IsPerMonitorV2Awareness)
                            {
                                _currentAutoScaleDimensions = new SizeF((float)_deviceDpi, (float)_deviceDpi);
                            }
                            else
                            {
                                // this DPI value comes from the primary monitor.
                                _currentAutoScaleDimensions = WindowsGraphicsCacheManager.MeasurementGraphics.DeviceContext.Dpi;
                            }
                            break;

                        default:
                            _currentAutoScaleDimensions = AutoScaleDimensions;
                            break;
                    }
                }

                return _currentAutoScaleDimensions;
            }
        }

        /// <summary>
        ///  Indicates the form that the scrollable control is assigned to. This property is read-only.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ContainerControlParentFormDescr))]
        public Form ParentForm
        {
            get
            {
                if (ParentInternal != null)
                {
                    return ParentInternal.FindForm();
                }
                if (this is Form)
                {
                    return null;
                }

                return FindForm();
            }
        }

        /// <summary>
        ///  Activates the specified control.
        /// </summary>
        bool IContainerControl.ActivateControl(Control control)
        {
            return ActivateControl(control, originator: true);
        }

        internal bool ActivateControl(Control control)
        {
            return ActivateControl(control, originator: true);
        }

        internal bool ActivateControl(Control control, bool originator)
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::ActivateControl(" + (control == null ? "null" : control.Name) + "," + originator.ToString() + ") - " + Name);

            // Recursive function that makes sure that the chain of active controls is coherent.
            bool ret = true;
            bool updateContainerActiveControl = false;
            ContainerControl cc = null;
            Control parent = ParentInternal;
            if (parent != null)
            {
                cc = (parent.GetContainerControl()) as ContainerControl;
                if (cc != null)
                {
                    updateContainerActiveControl = (cc.ActiveControl != this);
                }
            }
            if (control != _activeControl || updateContainerActiveControl)
            {
                if (updateContainerActiveControl)
                {
                    if (!cc.ActivateControl(this, false))
                    {
                        return false;
                    }
                }

                ret = AssignActiveControlInternal((control == this) ? null : control);
            }

            if (originator)
            {
                ScrollActiveControlIntoView();
            }

            return ret;
        }

        /// <summary>
        ///  Used for UserControls - checks if the control has a focusable control inside or not
        /// </summary>
        private bool HasFocusableChild()
        {
            Control ctl = null;
            do
            {
                ctl = GetNextControl(ctl, true);
                if (ctl != null && ctl.CanSelect && ctl.TabStop)
                {
                    break;
                }
            } while (ctl != null);

            return ctl != null;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void AdjustFormScrollbars(bool displayScrollbars)
        {
            base.AdjustFormScrollbars(displayScrollbars);

            if (!GetScrollState(ScrollStateUserHasScrolled))
            {
                ScrollActiveControlIntoView();
            }
        }

        /// <summary>
        ///  Cleans up form state after a control has been removed.
        /// </summary>
        internal virtual void AfterControlRemoved(Control control, Control oldParent)
        {
            ContainerControl cc;
            Debug.Assert(control != null);
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::AfterControlRemoved(" + control.Name + ") - " + Name);
            if (control == _activeControl || control.Contains(_activeControl))
            {
                bool selected = SelectNextControl(control, true, true, true, true);
                if (selected && _activeControl != control)
                {
                    // Add the check. If it is set to true, do not call into FocusActiveControlInternal().
                    // The TOP MDI window could be gone and CreateHandle method will fail
                    // because it try to create a parking window Parent for the MDI children
                    if (!_activeControl.Parent.IsTopMdiWindowClosing)
                    {
                        FocusActiveControlInternal();
                    }
                }
                else
                {
                    SetActiveControl(null);
                }
            }
            else if (_activeControl == null && ParentInternal != null)
            {
                // The last control of an active container was removed. Focus needs to be given to the next
                // control in the Form.
                cc = ParentInternal.GetContainerControl() as ContainerControl;
                if (cc != null && cc.ActiveControl == this)
                {
                    Form f = FindForm();
                    if (f != null)
                    {
                        f.SelectNextControl(this, true, true, true, true);
                    }
                }
            }

            // Two controls in UserControls that don't take focus via UI can have bad behavior if ...
            // When a control is removed from a container, not only do we need to clear the unvalidatedControl of that
            // container potentially, but the unvalidatedControl of all its container parents, up the chain, needs to
            // now point to the old parent of the disappearing control.
            cc = this;
            while (cc != null)
            {
                Control parent = cc.ParentInternal;
                if (parent == null)
                {
                    break;
                }
                else
                {
                    cc = parent.GetContainerControl() as ContainerControl;
                }
                if (cc != null &&
                    cc._unvalidatedControl != null &&
                    (cc._unvalidatedControl == control || control.Contains(cc._unvalidatedControl)))
                {
                    cc._unvalidatedControl = oldParent;
                }
            }

            if (control == _unvalidatedControl || control.Contains(_unvalidatedControl))
            {
                _unvalidatedControl = null;
            }
        }

        private bool AssignActiveControlInternal(Control value)
        {
#if DEBUG
            if (value == null || (value != null && value.ParentInternal != null && !value.ParentInternal.IsContainerControl))
            {
                Debug.Assert(value == null || (value.ParentInternal != null && this == value.ParentInternal.GetContainerControl()));
            }
#endif

            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::AssignActiveControlInternal(" + (value == null ? "null" : value.Name) + ") - " + Name);
            if (_activeControl != value)
            {
                try
                {
                    if (value != null)
                    {
                        value.BecomingActiveControl = true;
                    }

                    _activeControl = value;
                    UpdateFocusedControl();
                }
                finally
                {
                    if (value != null)
                    {
                        value.BecomingActiveControl = false;
                    }
                }

                if (_activeControl == value)
                {
                    Form form = FindForm();
                    if (form != null)
                    {
                        form.UpdateDefaultButton();
                    }
                }
            }
            else
            {
                _focusedControl = _activeControl;
            }

            return _activeControl == value;
        }

        /// <summary>
        ///  Used to notify the AxContainer that the form has been created. This should only be called
        ///  if there is an AX container.
        /// </summary>
        private void AxContainerFormCreated()
        {
            ((AxHost.AxContainer)Properties.GetObject(s_propAxContainer)).FormCreated();
        }

        /// <summary>
        ///  Specifies whether this control can process the mnemonic or not.
        /// </summary>
        internal override bool CanProcessMnemonic()
        {
#if DEBUG
            TraceCanProcessMnemonic();
#endif
            if (_state[s_stateProcessingMnemonic])
            {
                return true;
            }

            return base.CanProcessMnemonic();
        }

        internal AxHost.AxContainer CreateAxContainer()
        {
            object aXContainer = Properties.GetObject(s_propAxContainer);
            if (aXContainer == null)
            {
                aXContainer = new AxHost.AxContainer(this);
                Properties.SetObject(s_propAxContainer, aXContainer);
            }

            return (AxHost.AxContainer)aXContainer;
        }

        /// <summary>
        ///  Disposes of the resources (other than memory) used by the <see cref="ContainerControl"/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _activeControl = null;
            }

            base.Dispose(disposing);

            _focusedControl = null;
            _unvalidatedControl = null;
        }

        /// <summary>
        ///  Recursively enables required scaling from the given control
        /// </summary>
        private void EnableRequiredScaling(Control start, bool enable)
        {
            start.RequiredScalingEnabled = enable;
            foreach (Control c in start.Controls)
            {
                EnableRequiredScaling(c, enable);
            }
        }

        /// <summary>
        ///  Assigns focus to the activeControl. If there is no activeControl then focus is given to
        ///  the form. package scope for Form
        /// </summary>
        internal void FocusActiveControlInternal()
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::FocusActiveControlInternal() - " + Name);
#if DEBUG
            // Things really get ugly if you try to pop up an assert dialog here
            if (_activeControl != null && !Contains(_activeControl))
            {
                Debug.WriteLine("ActiveControl is not a child of this ContainerControl");
            }
#endif

            if (_activeControl != null && _activeControl.Visible)
            {
                // Avoid focus loops, especially with ComboBoxes.
                IntPtr focusHandle = User32.GetFocus();
                if (focusHandle == IntPtr.Zero || Control.FromChildHandle(focusHandle) != _activeControl)
                {
                    User32.SetFocus(new HandleRef(_activeControl, _activeControl.Handle));
                }
            }
            else
            {
                // Determine and focus closest visible parent
                ContainerControl cc = this;
                while (cc != null && !cc.Visible)
                {
                    Control parent = cc.ParentInternal;
                    if (parent != null)
                    {
                        cc = parent.GetContainerControl() as ContainerControl;
                    }
                    else
                    {
                        break;
                    }
                }

                if (cc != null && cc.Visible)
                {
                    User32.SetFocus(new HandleRef(cc, cc.Handle));
                }
            }
        }

        internal override Size GetPreferredSizeCore(Size proposedSize)
        {
            // Translating 0,0 from ClientSize to actual Size tells us how much space
            // is required for the borders.
            Size borderSize = SizeFromClientSize(Size.Empty);
            Size totalPadding = borderSize + Padding.Size;
            return LayoutEngine.GetPreferredSize(this, proposedSize - totalPadding) + totalPadding;
        }

        internal override Rectangle GetToolNativeScreenRectangle()
        {
            if (GetTopLevel())
            {
                // Get window's client rectangle (i.e. without chrome) expressed in screen coordinates
                var clientRectangle = new RECT();
                User32.GetClientRect(new HandleRef(this, Handle), ref clientRectangle);
                var topLeftPoint = new Point();
                User32.ClientToScreen(new HandleRef(this, Handle), ref topLeftPoint);
                return new Rectangle(topLeftPoint.X, topLeftPoint.Y, clientRectangle.right, clientRectangle.bottom);
            }

            return base.GetToolNativeScreenRectangle();
        }

        /// <summary>
        ///  This method calcuates the auto scale dimensions based on the control's current font.
        /// </summary>
        private SizeF GetFontAutoScaleDimensions()
        {
            SizeF retval = SizeF.Empty;

            // Windows uses CreateCompatibleDC(NULL) to get a memory DC for
            // the monitor the application is currently on.
            IntPtr dc = Gdi32.CreateCompatibleDC(IntPtr.Zero);
            if (dc == IntPtr.Zero)
            {
                throw new Win32Exception();
            }

            HandleRef hdc = new HandleRef(this, dc);

            try
            {
                // We clone the Windows scaling function here as closely as
                // possible. They use textmetric for height, and textmetric
                // for width of fixed width fonts. For variable width fonts
                // they use GetTextExtentPoint32 and pass in a long a-Z string.
                // We must do the same here if our dialogs are to scale in a
                // similar fashion.
                IntPtr hfontOld = Gdi32.SelectObject(hdc, new HandleRef(this, FontHandle));

                try
                {
                    var tm = new Gdi32.TEXTMETRICW();
                    Gdi32.GetTextMetricsW(hdc, ref tm);

                    retval.Height = tm.tmHeight;

                    if ((tm.tmPitchAndFamily & Gdi32.TMPF.FIXED_PITCH) != 0)
                    {
                        var size = new Size();
                        Gdi32.GetTextExtentPoint32W(hdc, FontMeasureString, FontMeasureString.Length, ref size);
                        // Note: intentional integer round off here for Win32 compat
                        retval.Width = (int)Math.Round(size.Width / ((float)FontMeasureString.Length));
                    }
                    else
                    {
                        retval.Width = tm.tmAveCharWidth;
                    }
                }
                finally
                {
                    Gdi32.SelectObject(dc, hfontOld);
                }
            }
            finally
            {
                Gdi32.DeleteDC(dc);
            }

            return retval;
        }

        /// <summary>
        ///  This method is called when one of the auto scale properties changes, indicating that we
        ///  should scale controls on the next layout.
        /// </summary>
        private void LayoutScalingNeeded()
        {
            EnableRequiredScaling(this, true);
            _state[s_stateScalingNeededOnLayout] = true;

            // If layout is not currently suspended, then perform a layout now,
            // as otherwise we don't know when one will happen.
            if (!IsLayoutSuspended)
            {
                LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
            }
        }

        /// <summary>
        ///  To maintain backwards compat with AutoScale on form, we need to keep the
        ///  two models from conflicting. This method is only here for Form to override
        ///  it and update its AutoScale property appropriately.
        /// </summary>
        private protected virtual void OnAutoScaleModeChanged()
        {
        }

        /// <summary>
        ///  Raises the AutoValidateChanged event.
        /// </summary>
        protected virtual void OnAutoValidateChanged(EventArgs e) => _autoValidateChanged?.Invoke(this, e);

        private protected override void OnFrameWindowActivate(bool fActivate)
        {
            if (fActivate)
            {
                if (ActiveControl == null)
                {
                    SelectNextControl(null, true, true, true, false);
                }
                InnerMostActiveContainerControl.FocusActiveControlInternal();
            }
        }

        /// <summary>
        ///  Called when a child is about to resume its layout. The default implementation
        ///  calls OnChildLayoutResuming on the parent.
        /// </summary>
        internal override void OnChildLayoutResuming(Control child, bool performLayout)
        {
            base.OnChildLayoutResuming(child, performLayout);

            // do not scale children if AutoScaleMode is set to Dpi
            if (AutoScaleMode == AutoScaleMode.Dpi)
            {
                return;
            }

            // We need to scale children before their layout engines get to them.
            // We don't have a lot of opportunity for that because the code
            // generator always generates a PerformLayout() right after a
            // ResumeLayout(false), so this seems to be the most opportune place
            // for this.
            if (!_state[s_stateScalingChild] && !performLayout && AutoScaleMode != AutoScaleMode.None && AutoScaleMode != AutoScaleMode.Inherit && _state[s_stateScalingNeededOnLayout])
            {
                _state[s_stateScalingChild] = true;
                try
                {
                    child.Scale(AutoScaleFactor, SizeF.Empty, this);
                }
                finally
                {
                    _state[s_stateScalingChild] = false;
                }
            }
        }

        /// <summary>
        ///  Raises the CreateControl event.
        /// </summary>
        protected override void OnCreateControl()
        {
            base.OnCreateControl();

            if (Properties.GetObject(s_propAxContainer) != null)
            {
                AxContainerFormCreated();
            }
            OnBindingContextChanged(EventArgs.Empty);
        }

        /// <summary>
        ///  We override this to clear the current autoscale cache.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnFontChanged(EventArgs e)
        {
            if (AutoScaleMode == AutoScaleMode.Font)
            {
                _currentAutoScaleDimensions = SizeF.Empty;

                // If the font changes and we are going to autoscale
                // as a result, do it now, and wrap the entire
                // transaction in a suspend layout to prevent
                // the layout engines from conflicting with our
                // work.
                SuspendAllLayout(this);

                try
                {
                    PerformAutoScale(!RequiredScalingEnabled, true);
                }
                finally
                {
                    ResumeAllLayout(this, false);
                }
            }

            base.OnFontChanged(e);
        }

        /// <summary>
        ///  This is called by the top level form to clear the current autoscale cache.
        /// </summary>
        private protected void FormDpiChanged(float factor)
        {
            Debug.Assert(this is Form);

            _currentAutoScaleDimensions = SizeF.Empty;

            SuspendAllLayout(this);
            SizeF factorSize = new SizeF(factor, factor);
            try
            {
                ScaleChildControls(factorSize, factorSize, this, true);
            }
            finally
            {
                ResumeAllLayout(this, false);
            }
        }

        /// <summary>
        ///  Container controls scale during layout.
        /// </summary>
        protected override void OnLayout(LayoutEventArgs e)
        {
            PerformNeededAutoScaleOnLayout();
            base.OnLayout(e);
        }

        /// <summary>
        ///  Called when the last resume layout call is made. If performLayout is true a layout will
        ///  occur as soon as this call returns. Layout is still suspended when this call is made.
        ///  The default implementation calls OnChildLayoutResuming on the parent, if it exists.
        /// </summary>
        internal override void OnLayoutResuming(bool performLayout)
        {
            PerformNeededAutoScaleOnLayout();
            base.OnLayoutResuming(performLayout);
        }

        /// <summary>
        ///  Called when the parent changes. Container controls prefer to have their parents scale
        ///  themselves, but when a parent is first changed, and as a result the font changes as
        ///  well, a container control should scale itself. We save off this state so a later
        ///  font change can trigger a scale of us. We only set this state if required scaling is
        ///  disabled:  if it is enabled we are still initializing and parent changes are normal.
        /// </summary>
        protected override void OnParentChanged(EventArgs e)
        {
            _state[s_stateParentChanged] = !RequiredScalingEnabled;
            base.OnParentChanged(e);
        }

        /// <summary>
        ///  Performs scaling of this control. Scaling works by scaling all children of this control.
        ///  Those children that are ContainerControls will have their PerformAutoScale method called
        ///  so they can scale their children.
        /// </summary>
        public void PerformAutoScale() => PerformAutoScale(includedBounds: true, excludedBounds: true);

        /// <summary>
        ///  Performs scaling of this control. Scaling works by scaling all children of this control.
        ///
        ///  If includedBounds is true those controls whose bounds have changed since
        ///  they were last scaled will be auto scaled. If excludedBounds is true those
        ///  controls whose bounds have not changed since they were last scaled will be
        ///  auto scaled.
        ///
        ///  PerformAutoScale is automatically called during OnLayout. The parameters to
        ///  PerformAutoScale are passed as follows:
        ///  1. If AutoScaleDimensions are set, includedBounds is set to true.
        ///  2. If a font change occurred, excludedBounds is set to true.
        /// </summary>
        private void PerformAutoScale(bool includedBounds, bool excludedBounds)
        {
            bool suspended = false;

            try
            {
                if (AutoScaleMode != AutoScaleMode.None && AutoScaleMode != AutoScaleMode.Inherit)
                {
                    SuspendAllLayout(this);
                    suspended = true;

                    // Walk each control recursively and scale. We search the control
                    // for its own set of scaling data; if we don't find it, we use the current
                    // container control's scaling data. Once we scale a control, we set
                    // its scaling factors to unity. As we walk out of a container control,
                    // we set its scaling factor to unity too.
                    SizeF included = includedBounds ? AutoScaleFactor : SizeF.Empty;
                    SizeF excluded = excludedBounds ? AutoScaleFactor : SizeF.Empty;
                    Scale(included, excluded, this);
                    _autoScaleDimensions = CurrentAutoScaleDimensions;
                }
            }
            finally
            {
                if (includedBounds)
                {
                    _state[s_stateScalingNeededOnLayout] = false;
                    EnableRequiredScaling(this, enable: false);
                }
                _state[s_stateParentChanged] = false;

                if (suspended)
                {
                    ResumeAllLayout(this, performLayout: false);
                }
            }
        }

        /// <summary>
        ///  Checks to see if we need to perform an autoscale in response to a layout.
        /// </summary>
        private void PerformNeededAutoScaleOnLayout()
        {
            if (_state[s_stateScalingNeededOnLayout])
            {
                PerformAutoScale(_state[s_stateScalingNeededOnLayout], false);
            }
        }

        /// <summary>
        ///  Recursively resumes all layout.
        /// </summary>
        internal void ResumeAllLayout(Control start, bool performLayout)
        {
            ControlCollection controlsCollection = start.Controls;
            // This may have changed the sizes of our children.
            // PERFNOTE: This is more efficient than using Foreach. Foreach
            // forces the creation of an array subset enum each time we
            // enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                ResumeAllLayout(controlsCollection[i], performLayout);
            }

            start.ResumeLayout(performLayout);
        }

        /// <summary>
        ///  Recursively suspends all layout.
        /// </summary>
        internal void SuspendAllLayout(Control start)
        {
            start.SuspendLayout();
            CommonProperties.xClearPreferredSizeCache(start);

            ControlCollection controlsCollection = start.Controls;
            // This may have changed the sizes of our children. For performance, this is more
            // efficient than using Foreach. Foreach forces the creation of an array subset enum
            // each time we enumerate
            for (int i = 0; i < controlsCollection.Count; i++)
            {
                SuspendAllLayout(controlsCollection[i]);
            }
        }

        /// <summary>
        ///  Overrides the default scaling mechanism to account for autoscaling. This override
        ///  behaves as follows: any unchanged controls are always scaled according to the container
        ///  control's AutoScaleFactor. Any changed controls are scaled according to the provided
        ///  scaling factor.
        /// </summary>
        internal override void Scale(SizeF includedFactor, SizeF excludedFactor, Control requestingControl)
        {
            // If we're inhieriting our scaling from our parent, Scale is really easy:  just do the
            // base class implementation.
            if (AutoScaleMode == AutoScaleMode.Inherit)
            {
                base.Scale(includedFactor, excludedFactor, requestingControl);
            }
            else
            {
                // We scale our controls based on our own auto scaling
                // factor, not the one provided to us. We only do this for
                // controls that are not required to be scaled (excluded controls).
                SizeF ourExcludedFactor = excludedFactor;
                SizeF childIncludedFactor = includedFactor;

                if (!ourExcludedFactor.IsEmpty)
                {
                    ourExcludedFactor = AutoScaleFactor;
                }

                // If we're not supposed to be scaling, don't scale the internal ones either.
                if (AutoScaleMode == AutoScaleMode.None)
                {
                    childIncludedFactor = AutoScaleFactor;
                }

                // When we scale, we are establishing new baselines for the
                // positions of all controls. Therefore, we should resume(false).
                using (new LayoutTransaction(this, this, PropertyNames.Bounds, false))
                {
                    // Our own container control poses a problem. We want
                    // an outer control to be responsible for scaling it,
                    // because the outer control knows the container's dimensions.
                    // We detect this by checking who is requesting that the
                    // scaling occur.
                    SizeF ourExternalContainerFactor = ourExcludedFactor;

                    if (!excludedFactor.IsEmpty && ParentInternal != null)
                    {
                        ourExternalContainerFactor = SizeF.Empty;

                        bool scaleUs = (requestingControl != this || _state[s_stateParentChanged]);

                        // For design time support:  we may be parented within another form
                        // that is not part of the designer.
                        if (!scaleUs)
                        {
                            bool dt = Site?.DesignMode ?? false;
                            bool parentDt = ParentInternal.Site?.DesignMode ?? false;
                            if (dt && !parentDt)
                            {
                                scaleUs = true;
                            }
                        }

                        if (scaleUs)
                        {
                            ourExternalContainerFactor = excludedFactor;
                        }
                    }

                    ScaleControl(includedFactor, ourExternalContainerFactor, requestingControl);
                    ScaleChildControls(childIncludedFactor, ourExcludedFactor, requestingControl);
                }
            }
        }

        /// <summary>
        ///  Process an arrowKey press by selecting the next control in the group that the activeControl
        ///  belongs to.
        /// </summary>
        private bool ProcessArrowKey(bool forward)
        {
            Control group = this;
            if (_activeControl != null)
            {
                group = _activeControl.ParentInternal;
            }

            return group.SelectNextControl(_activeControl, forward, false, false, true);
        }

        /// <summary>
        ///  Processes a dialog character. Overrides Control.processDialogChar(). This method calls
        ///  the ProcessMnemonic() method to check if the character is a mnemonic for one of the
        ///  controls on the form. If processMnemonic() does not consume the character, then
        ///  base.ProcessDialogChar() is called.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override bool ProcessDialogChar(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "ContainerControl.ProcessDialogChar [" + charCode.ToString() + "]");

            // If we're the top-level form or control, we need to do the mnemonic handling
            if (GetContainerControl() is ContainerControl parent && charCode != ' ' && ProcessMnemonic(charCode))
            {
                return true;
            }

            return base.ProcessDialogChar(charCode);
        }

        /// <summary>
        ///  Processes a dialog key. Overrides Control.processDialogKey(). This method implements
        ///  handling of the TAB, LEFT, RIGHT, UP, and DOWN keys in dialogs.
        ///  The method performs no processing on keys that include the ALT or CONTROL modifiers.
        ///  For the TAB key, the method selects the next control on the form. For the arrow keys, !!!
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "ContainerControl.ProcessDialogKey [" + keyData.ToString() + "]");

            LastKeyData = keyData;
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
            {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                switch (keyCode)
                {
                    case Keys.Tab:
                        if (ProcessTabKey((keyData & Keys.Shift) == Keys.None))
                        {
                            return true;
                        }

                        break;
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        if (ProcessArrowKey(keyCode == Keys.Right ||
                                            keyCode == Keys.Down))
                        {
                            return true;
                        }

                        break;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "ContainerControl.ProcessCmdKey " + msg.ToString());

            if (base.ProcessCmdKey(ref msg, keyData))
            {
                return true;
            }
            if (ParentInternal == null)
            {
                // Unfortunately, we have to stick this here for the case where we're hosted without
                // a form in the chain. This would be something like a context menu strip with shortcuts
                // hosted within Office, VS or IE.
                //
                // this is an optimized search O(number of ToolStrips in thread)
                // that happens only if the key routing makes it to the top.
                return ToolStripManager.ProcessCmdKey(ref msg, keyData);
            }

            return false;
        }

        protected internal override bool ProcessMnemonic(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "ContainerControl.ProcessMnemonic [" + charCode.ToString() + "]");
            Debug.Indent();
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "this == " + ToString());

            if (!CanProcessMnemonic())
            {
                return false;
            }

            if (Controls.Count == 0)
            {
                Debug.Unindent();
                return false;
            }

            // Start with the active control.
            //
            Control start = ActiveControl;

#if DEBUG
            int count = 0;
#endif

            // Set the processing mnemonic flag so child controls don't check for it when checking if they
            // can process the mnemonic.
            _state[s_stateProcessingMnemonic] = true;

            bool processed = false;

            try
            {
                // Safety flag to avoid infinite loop when testing controls in a container.
                bool wrapped = false;

                Control ctl = start;
                Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Check starting at '" + ((start != null) ? start.ToString() : "<null>") + "'");

                do
                {
                    // Loop through the controls starting at the control next to the current Active control in the Tab order
                    // till we find someone willing to process this mnemonic.
                    // We don't start the search on the Active control to allow controls in the same container with the same
                    // mnemonic (bad UI design but supported) to be processed sequentially
#if DEBUG
                    count++;
                    Debug.Assert(count <= 999, "Infinite loop trying to find controls which can ProcessMnemonic()!!!");
#endif
                    ctl = GetNextControl(ctl, true);

                    if (ctl != null)
                    {
#if DEBUG
                        Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "  ...checking for mnemonics on " + ctl.ToString());
                        // Control.TraceMnemonicProcessing.Enabled disables CanProcessMnemonic consistency check.
                        bool canProcess = s_traceMnemonicProcessing.Enabled ? true : ctl.CanProcessMnemonic(); // Processing the mnemonic can change the value of CanProcessMnemonic.
#endif
                        // Processing the mnemonic can change the value of CanProcessMnemonic.
                        if (ctl.ProcessMnemonic(charCode))
                        {
#if DEBUG
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "  ...mnemonics found");
                            Debug.Assert((s_traceMnemonicProcessing.Enabled || canProcess), "ProcessMnemonic returned true, even though CanProcessMnemonic() is false. Someone probably overrode ProcessMnemonic and forgot to test CanSelect or CanProcessMnemonic().");
                            Debug.Unindent();
#endif
                            processed = true;
                            break;
                        }
                    }
                    else
                    {
                        if (wrapped)
                        {
                            // This avoids infinite loops
                            break;
                        }

                        wrapped = true;
                    }
                } while (ctl != start);
            }
            finally
            {
                _state[s_stateProcessingMnemonic] = false;
            }

            Debug.Unindent();
            return processed;
        }

        /// <summary>
        ///  Selects the next available control and makes it the active control.
        /// </summary>
        protected virtual bool ProcessTabKey(bool forward)
        {
            return SelectNextControl(_activeControl, forward, tabStopOnly: true, nested: true, wrap: false);
        }

        private ScrollableControl FindScrollableParent(Control ctl)
        {
            Control current = ctl.ParentInternal;
            while (current != null && !(current is ScrollableControl))
            {
                current = current.ParentInternal;
            }

            return (ScrollableControl)current;
        }

        private void ScrollActiveControlIntoView()
        {
            Control last = _activeControl;
            if (last != null)
            {
                ScrollableControl scrollParent = FindScrollableParent(last);

                while (scrollParent != null)
                {
                    scrollParent.ScrollControlIntoView(_activeControl);
                    last = scrollParent;
                    scrollParent = FindScrollableParent(scrollParent);
                }
            }
        }

        protected override void Select(bool directed, bool forward)
        {
            bool correctParentActiveControl = true;
            if (ParentInternal != null)
            {
                IContainerControl c = ParentInternal.GetContainerControl();
                if (c != null)
                {
                    c.ActiveControl = this;
                    correctParentActiveControl = (c.ActiveControl == this);
                }
            }
            if (directed && correctParentActiveControl)
            {
                SelectNextControl(null, forward, tabStopOnly: true, nested: true, wrap: false);
            }
        }

        /// <summary>
        ///  Implements ActiveControl property setter.
        /// </summary>
        internal void SetActiveControl(Control value)
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::SetActiveControl(" + (value == null ? "null" : value.Name) + ") - " + Name);

            if (_activeControl != value || (value != null && !value.Focused))
            {
                if (value != null && !Contains(value))
                {
                    throw new ArgumentException(SR.CannotActivateControl, nameof(value));
                }

                bool ret;
                ContainerControl cc = this;

                if (value != null)
                {
                    cc = (value.ParentInternal.GetContainerControl()) as ContainerControl;
                }
                if (cc != null)
                {
                    // Call to the recursive function that corrects the chain of active controls
                    ret = cc.ActivateControl(value, false);
                }
                else
                {
                    ret = AssignActiveControlInternal(value);
                }

                if (cc != null && ret)
                {
                    ContainerControl ccAncestor = this;
                    while (ccAncestor.ParentInternal != null &&
                           ccAncestor.ParentInternal.GetContainerControl() is ContainerControl)
                    {
                        ccAncestor = ccAncestor.ParentInternal.GetContainerControl() as ContainerControl;
                        Debug.Assert(ccAncestor != null);
                    }

                    if (ccAncestor.ContainsFocus &&
                        (value == null ||
                         !(value is UserControl) ||
                         (value is UserControl && !((UserControl)value).HasFocusableChild())))
                    {
                        cc.FocusActiveControlInternal();
                    }
                }
            }
        }

        private protected ContainerControl InnerMostActiveContainerControl
        {
            get
            {
                ContainerControl ret = this;
                while (ret.ActiveControl is ContainerControl)
                {
                    ret = (ContainerControl)ret.ActiveControl;
                }

                return ret;
            }
        }

        private ContainerControl InnerMostFocusedContainerControl
        {
            get
            {
                ContainerControl ret = this;
                while (ret._focusedControl is ContainerControl)
                {
                    ret = (ContainerControl)ret._focusedControl;
                }

                return ret;
            }
        }

        /// <summary>
        ///  Updates the default button based on current selection, and the acceptButton property.
        /// </summary>
        protected virtual void UpdateDefaultButton()
        {
        }

        /// <summary>
        ///  Updates the focusedControl variable by walking towards the activeControl variable, firing
        ///  enter and leave events and validation as necessary.
        /// </summary>
        internal void UpdateFocusedControl()
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::UpdateFocusedControl() - " + Name);

            // Capture the current focusedControl as the unvalidatedControl if we don't have one/are not validating.
            EnsureUnvalidatedControl(_focusedControl);
            Control pathControl = _focusedControl;

            while (_activeControl != pathControl)
            {
                if (pathControl == null || pathControl.IsDescendant(_activeControl))
                {
                    // Heading down. Find next control on path.
                    Control nextControlDown = _activeControl;
                    while (true)
                    {
                        Control parent = nextControlDown.ParentInternal;
                        if (parent == this || parent == pathControl)
                        {
                            break;
                        }

                        nextControlDown = nextControlDown.ParentInternal;
                    }

                    Control priorFocusedControl = _focusedControl = pathControl;
                    EnterValidation(nextControlDown);
                    // If validation changed position, then jump back to the loop.
                    if (_focusedControl != priorFocusedControl)
                    {
                        pathControl = _focusedControl;
                        continue;
                    }

                    pathControl = nextControlDown;
                    if (NativeWindow.WndProcShouldBeDebuggable)
                    {
                        pathControl.NotifyEnter();
                    }
                    else
                    {
                        try
                        {
                            pathControl.NotifyEnter();
                        }
                        catch (Exception e)
                        {
                            Application.OnThreadException(e);
                        }
                    }
                }
                else
                {
                    // Heading up.
                    ContainerControl innerMostFCC = InnerMostFocusedContainerControl;
                    Control stopControl = null;

                    if (innerMostFCC._focusedControl != null)
                    {
                        pathControl = innerMostFCC._focusedControl;
                        stopControl = innerMostFCC;

                        if (innerMostFCC != this)
                        {
                            innerMostFCC._focusedControl = null;
                            if (!(innerMostFCC.ParentInternal != null && innerMostFCC.ParentInternal is MdiClient))
                            {
                                // Don't reset the active control of a MDIChild that loses the focus
                                innerMostFCC._activeControl = null;
                            }
                        }
                    }
                    else
                    {
                        pathControl = innerMostFCC;
                        // innerMostFCC.ParentInternal can be null when the ActiveControl is deleted.
                        if (innerMostFCC.ParentInternal != null)
                        {
                            ContainerControl cc = (innerMostFCC.ParentInternal.GetContainerControl()) as ContainerControl;
                            stopControl = cc;
                            if (cc != null && cc != this)
                            {
                                cc._focusedControl = null;
                                cc._activeControl = null;
                            }
                        }
                    }

                    do
                    {
                        Control leaveControl = pathControl;

                        if (pathControl != null)
                        {
                            pathControl = pathControl.ParentInternal;
                        }

                        if (pathControl == this)
                        {
                            pathControl = null;
                        }

                        if (leaveControl != null)
                        {
                            if (NativeWindow.WndProcShouldBeDebuggable)
                            {
                                leaveControl.NotifyLeave();
                            }
                            else
                            {
                                try
                                {
                                    leaveControl.NotifyLeave();
                                }
                                catch (Exception e)
                                {
                                    Application.OnThreadException(e);
                                }
                            }
                        }
                    }
                    while (pathControl != null &&
                           pathControl != stopControl &&
                           !pathControl.IsDescendant(_activeControl));
                }
            }

#if DEBUG
            if (_activeControl == null || (_activeControl != null && _activeControl.ParentInternal != null && !_activeControl.ParentInternal.IsContainerControl))
            {
                Debug.Assert(_activeControl == null || _activeControl.ParentInternal.GetContainerControl() == this);
            }
#endif
            _focusedControl = _activeControl;
            if (_activeControl != null)
            {
                EnterValidation(_activeControl);
            }
        }

        /// <summary>
        ///  Make sure we have a valid choice of last unvalidated control if at all possible.
        /// </summary>
        private void EnsureUnvalidatedControl(Control candidate)
        {
            // Don't change the unvalidated control while in the middle of validation (re-entrancy)
            if (_state[s_stateValidating])
            {
                return;
            }

            // Don't change the existing unvalidated control
            if (_unvalidatedControl != null)
            {
                return;
            }

            // No new choice of unvalidated control was specified - leave unvalidated control blank
            if (candidate == null)
            {
                return;
            }

            // Specified control has auto-validation disabled - leave unvalidated control blank
            if (!candidate.ShouldAutoValidate)
            {
                return;
            }

            // Go ahead and make specified control the current unvalidated control for this container
            _unvalidatedControl = candidate;

            // In the case of nested container controls, try to pick the deepest possible unvalidated
            // control. For a container with no unvalidated control, use the active control instead.
            // Stop as soon as we encounter any control that has auto-validation turned off.
            while (_unvalidatedControl is ContainerControl)
            {
                ContainerControl container = _unvalidatedControl as ContainerControl;

                if (container._unvalidatedControl != null && container._unvalidatedControl.ShouldAutoValidate)
                {
                    _unvalidatedControl = container._unvalidatedControl;
                }
                else if (container._activeControl != null && container._activeControl.ShouldAutoValidate)
                {
                    _unvalidatedControl = container._activeControl;
                }
                else
                {
                    break;
                }
            }
        }

        /// <summary>
        ///  Validates the last unvalidated control and its ancestors (up through the ancestor in common
        ///  with enterControl) if enterControl causes validation.
        /// </summary>
        private void EnterValidation(Control enterControl)
        {
            // No unvalidated control to validate - stop now
            if (_unvalidatedControl == null)
            {
                return;
            }

            // Entered control does not trigger validation - stop now
            if (!enterControl.CausesValidation)
            {
                return;
            }

            // Get the effective AutoValidate mode for this control (based on its container control)
            AutoValidate autoValidateMode = Control.GetAutoValidateForControl(_unvalidatedControl);

            // Auto-validate has been turned off in container of unvalidated control - stop now
            if (autoValidateMode == AutoValidate.Disable)
            {
                return;
            }

            // Find common ancestor of entered control and unvalidated control
            Control commonAncestor = enterControl;
            while (commonAncestor != null && !commonAncestor.IsDescendant(_unvalidatedControl))
            {
                commonAncestor = commonAncestor.ParentInternal;
            }

            // Should we force focus to stay on same control if there is a validation error?
            bool preventFocusChangeOnError = (autoValidateMode == AutoValidate.EnablePreventFocusChange);

            // Validate control and its ancestors, up to (but not including) the common ancestor
            ValidateThroughAncestor(commonAncestor, preventFocusChangeOnError);
        }

        /// <summary>
        ///  Validates the last unvalidated control and its ancestors up through, but not including the current control.
        ///
        ///  This version always performs validation, regardless of the AutoValidate setting of the control's parent.
        /// </summary>
        /// <remarks>
        ///  This version is intended for user code that wants to force validation, even
        ///  while auto-validation is turned off. When adding any explicit Validate() calls to our code, consider using
        ///  Validate(true) rather than Validate(), so that you will be sensitive to the current auto-validation setting.
        /// </remarks>
        public bool Validate() => Validate(checkAutoValidate: false);

        /// <summary>
        ///  Validates the last unvalidated control and its ancestors up through, but not including the current control.
        ///  This version will skip validation if checkAutoValidate is true and the effective AutoValidate setting, as
        ///  determined by the control's parent, is AutoValidate.Disable.
        /// </summary>
        public bool Validate(bool checkAutoValidate)
        {
            return ValidateInternal(checkAutoValidate, out bool validatedControlAllowsFocusChange);
        }

        internal bool ValidateInternal(bool checkAutoValidate, out bool validatedControlAllowsFocusChange)
        {
            validatedControlAllowsFocusChange = false;

            if (AutoValidate == AutoValidate.EnablePreventFocusChange ||
                (_activeControl != null && _activeControl.CausesValidation))
            {
                if (_unvalidatedControl == null)
                {
                    if (_focusedControl is ContainerControl && _focusedControl.CausesValidation)
                    {
                        ContainerControl c = (ContainerControl)_focusedControl;
                        if (!c.ValidateInternal(checkAutoValidate, out validatedControlAllowsFocusChange))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        _unvalidatedControl = _focusedControl;
                    }
                }

                // Should we force focus to stay on same control if there is a validation error?
                bool preventFocusChangeOnError = true;

                Control controlToValidate = _unvalidatedControl ?? _focusedControl;

                if (controlToValidate != null)
                {
                    // Get the effective AutoValidate mode for unvalidated control (based on its container control)
                    AutoValidate autoValidateMode = Control.GetAutoValidateForControl(controlToValidate);

                    // Auto-validate has been turned off in container of unvalidated control - stop now
                    if (checkAutoValidate && autoValidateMode == AutoValidate.Disable)
                    {
                        return true;
                    }

                    preventFocusChangeOnError = (autoValidateMode == AutoValidate.EnablePreventFocusChange);
                    validatedControlAllowsFocusChange = (autoValidateMode == AutoValidate.EnableAllowFocusChange);
                }

                return ValidateThroughAncestor(null, preventFocusChangeOnError);
            }

            return true;
        }

        /// <summary>
        ///  Validates all selectable child controls in the container, including descendants. This is
        ///  equivalent to calling ValidateChildren(ValidationConstraints.Selectable). See <see cref='ValidationConstraints.Selectable'/>
        ///  for details of exactly which child controls will be validated.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool ValidateChildren() => ValidateChildren(ValidationConstraints.Selectable);

        /// <summary>
        ///  Validates all the child controls in the container. Exactly which controls are
        ///  validated and which controls are skipped is determined by <paramref name="validationConstraints"/>.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual bool ValidateChildren(ValidationConstraints validationConstraints)
        {
            if ((int)validationConstraints < 0x00 || (int)validationConstraints > 0x1F)
            {
                throw new InvalidEnumArgumentException(nameof(validationConstraints), (int)validationConstraints, typeof(ValidationConstraints));
            }

            return !PerformContainerValidation(validationConstraints);
        }

        private bool ValidateThroughAncestor(Control ancestorControl, bool preventFocusChangeOnError)
        {
            if (ancestorControl == null)
            {
                ancestorControl = this;
            }

            if (_state[s_stateValidating])
            {
                return false;
            }

            if (_unvalidatedControl == null)
            {
                _unvalidatedControl = _focusedControl;
            }

            // Return true for a Container Control with no controls to validate.
            if (_unvalidatedControl == null)
            {
                return true;
            }

            if (!ancestorControl.IsDescendant(_unvalidatedControl))
            {
                return false;
            }

            _state[s_stateValidating] = true;
            bool cancel = false;

            Control currentActiveControl = _activeControl;
            Control currentValidatingControl = _unvalidatedControl;
            if (currentActiveControl != null)
            {
                currentActiveControl.ValidationCancelled = false;
                if (currentActiveControl is ContainerControl currentActiveContainerControl)
                {
                    currentActiveContainerControl.ResetValidationFlag();
                }
            }
            try
            {
                while (currentValidatingControl != null && currentValidatingControl != ancestorControl)
                {
                    try
                    {
                        cancel = currentValidatingControl.PerformControlValidation(false);
                    }
                    catch
                    {
                        cancel = true;
                        throw;
                    }

                    if (cancel)
                    {
                        break;
                    }

                    currentValidatingControl = currentValidatingControl.ParentInternal;
                }

                if (cancel && preventFocusChangeOnError)
                {
                    if (_unvalidatedControl == null && currentValidatingControl != null &&
                        ancestorControl.IsDescendant(currentValidatingControl))
                    {
                        _unvalidatedControl = currentValidatingControl;
                    }
                    // This bit 'marks' the control that was going to get the focus, so that it will ignore any pending
                    // mouse or key events. Otherwise it would still perform its default 'click' action or whatever.
                    if (currentActiveControl == _activeControl)
                    {
                        if (currentActiveControl != null)
                        {
                            CancelEventArgs ev = new CancelEventArgs
                            {
                                Cancel = true
                            };
                            currentActiveControl.NotifyValidationResult(currentValidatingControl, ev);
                            if (currentActiveControl is ContainerControl)
                            {
                                ContainerControl currentActiveContainerControl = currentActiveControl as ContainerControl;
                                if (currentActiveContainerControl._focusedControl != null)
                                {
                                    currentActiveContainerControl._focusedControl.ValidationCancelled = true;
                                }
                                currentActiveContainerControl.ResetActiveAndFocusedControlsRecursive();
                            }
                        }
                    }

                    // This bit forces the focus to move back to the invalid control
                    SetActiveControl(_unvalidatedControl);
                }
            }
            finally
            {
                _unvalidatedControl = null;
                _state[s_stateValidating] = false;
            }

            return !cancel;
        }

        private void ResetValidationFlag()
        {
            // Performance: This is more efficient than using Foreach. Foreach forces the creation of
            // an array subset enum each time we enumerate
            ControlCollection children = Controls;
            int count = children.Count;
            for (int i = 0; i < count; i++)
            {
                children[i].ValidationCancelled = false;
            }
        }

        internal void ResetActiveAndFocusedControlsRecursive()
        {
            if (_activeControl is ContainerControl activeContainerControl)
            {
                activeContainerControl.ResetActiveAndFocusedControlsRecursive();
            }

            _activeControl = null;
            _focusedControl = null;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeAutoValidate() => _autoValidate != AutoValidate.Inherit;

        /// <summary>
        ///  WM_SETFOCUS handler
        /// </summary>
        private void WmSetFocus(ref Message m)
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "ContainerControl::WmSetFocus() - " + Name);
            if (!HostedInWin32DialogManager)
            {
                if (ActiveControl != null)
                {
                    WmImeSetFocus();
                    // Do not raise GotFocus event since the focus is given to the visible ActiveControl
                    if (!ActiveControl.Visible)
                    {
                        InvokeGotFocus(this, EventArgs.Empty);
                    }

                    FocusActiveControlInternal();
                }
                else
                {
                    if (ParentInternal != null)
                    {
                        IContainerControl c = ParentInternal.GetContainerControl();
                        if (c != null)
                        {
                            bool succeeded = false;

                            if (c is ContainerControl knowncontainer)
                            {
                                succeeded = knowncontainer.ActivateControl(this);
                            }
                            else
                            {
                                succeeded = c.ActivateControl(this);
                            }
                            if (!succeeded)
                            {
                                return;
                            }
                        }
                    }

                    base.WndProc(ref m);
                }
            }
            else
            {
                base.WndProc(ref m);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.SETFOCUS:
                    WmSetFocus(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
