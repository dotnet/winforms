// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a window or dialog box that makes up an application's user interface.
    /// </summary>
    [ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
    [ToolboxItem(false)]
    [DesignTimeVisible(false)]
    [Designer("System.Windows.Forms.Design.FormDocumentDesigner, " + AssemblyRef.SystemDesign, typeof(IRootDesigner))]
    [DefaultEvent(nameof(Load))]
    [InitializationEvent(nameof(Load))]
    [DesignerCategory("Form")]
    public partial class Form : ContainerControl
    {
        private static readonly object EVENT_ACTIVATED = new object();
        private static readonly object EVENT_CLOSING = new object();
        private static readonly object EVENT_CLOSED = new object();
        private static readonly object EVENT_FORMCLOSING = new object();
        private static readonly object EVENT_FORMCLOSED = new object();
        private static readonly object EVENT_DEACTIVATE = new object();
        private static readonly object EVENT_LOAD = new object();
        private static readonly object EVENT_MDI_CHILD_ACTIVATE = new object();
        private static readonly object EVENT_INPUTLANGCHANGE = new object();
        private static readonly object EVENT_INPUTLANGCHANGEREQUEST = new object();
        private static readonly object EVENT_MENUSTART = new object();
        private static readonly object EVENT_MENUCOMPLETE = new object();
        private static readonly object EVENT_MAXIMUMSIZECHANGED = new object();
        private static readonly object EVENT_MINIMUMSIZECHANGED = new object();
        private static readonly object EVENT_HELPBUTTONCLICKED = new object();
        private static readonly object EVENT_SHOWN = new object();
        private static readonly object EVENT_RESIZEBEGIN = new object();
        private static readonly object EVENT_RESIZEEND = new object();
        private static readonly object EVENT_RIGHTTOLEFTLAYOUTCHANGED = new object();
        private static readonly object EVENT_DPI_CHANGED = new object();

        //
        // The following flags should be used with formState[..] not formStateEx[..]
        // Don't add any more sections to this vector, it is already full.
        //
        private static readonly BitVector32.Section FormStateAllowTransparency = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section FormStateBorderStyle = BitVector32.CreateSection(6, FormStateAllowTransparency);
        private static readonly BitVector32.Section FormStateTaskBar = BitVector32.CreateSection(1, FormStateBorderStyle);
        private static readonly BitVector32.Section FormStateControlBox = BitVector32.CreateSection(1, FormStateTaskBar);
        private static readonly BitVector32.Section FormStateKeyPreview = BitVector32.CreateSection(1, FormStateControlBox);
        private static readonly BitVector32.Section FormStateLayered = BitVector32.CreateSection(1, FormStateKeyPreview);
        private static readonly BitVector32.Section FormStateMaximizeBox = BitVector32.CreateSection(1, FormStateLayered);
        private static readonly BitVector32.Section FormStateMinimizeBox = BitVector32.CreateSection(1, FormStateMaximizeBox);
        private static readonly BitVector32.Section FormStateHelpButton = BitVector32.CreateSection(1, FormStateMinimizeBox);
        private static readonly BitVector32.Section FormStateStartPos = BitVector32.CreateSection(4, FormStateHelpButton);
        private static readonly BitVector32.Section FormStateWindowState = BitVector32.CreateSection(2, FormStateStartPos);
        private static readonly BitVector32.Section FormStateShowWindowOnCreate = BitVector32.CreateSection(1, FormStateWindowState);
        private static readonly BitVector32.Section FormStateAutoScaling = BitVector32.CreateSection(1, FormStateShowWindowOnCreate);
        private static readonly BitVector32.Section FormStateSetClientSize = BitVector32.CreateSection(1, FormStateAutoScaling);
        private static readonly BitVector32.Section FormStateTopMost = BitVector32.CreateSection(1, FormStateSetClientSize);
        private static readonly BitVector32.Section FormStateSWCalled = BitVector32.CreateSection(1, FormStateTopMost);
        private static readonly BitVector32.Section FormStateMdiChildMax = BitVector32.CreateSection(1, FormStateSWCalled);
        private static readonly BitVector32.Section FormStateRenderSizeGrip = BitVector32.CreateSection(1, FormStateMdiChildMax);
        private static readonly BitVector32.Section FormStateSizeGripStyle = BitVector32.CreateSection(2, FormStateRenderSizeGrip);
        private static readonly BitVector32.Section FormStateIsWindowActivated = BitVector32.CreateSection(1, FormStateSizeGripStyle);
        private static readonly BitVector32.Section FormStateIsTextEmpty = BitVector32.CreateSection(1, FormStateIsWindowActivated);
        private static readonly BitVector32.Section FormStateIsActive = BitVector32.CreateSection(1, FormStateIsTextEmpty);
        private static readonly BitVector32.Section FormStateIconSet = BitVector32.CreateSection(1, FormStateIsActive);

        // The following flags should be used with formStateEx[...] not formState[..]
        private static readonly BitVector32.Section FormStateExCalledClosing = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section FormStateExUpdateMenuHandlesSuspendCount = BitVector32.CreateSection(8, FormStateExCalledClosing);
        private static readonly BitVector32.Section FormStateExUpdateMenuHandlesDeferred = BitVector32.CreateSection(1, FormStateExUpdateMenuHandlesSuspendCount);
        private static readonly BitVector32.Section FormStateExUseMdiChildProc = BitVector32.CreateSection(1, FormStateExUpdateMenuHandlesDeferred);
        private static readonly BitVector32.Section FormStateExCalledOnLoad = BitVector32.CreateSection(1, FormStateExUseMdiChildProc);
        private static readonly BitVector32.Section FormStateExCalledMakeVisible = BitVector32.CreateSection(1, FormStateExCalledOnLoad);
        private static readonly BitVector32.Section FormStateExCalledCreateControl = BitVector32.CreateSection(1, FormStateExCalledMakeVisible);
        private static readonly BitVector32.Section FormStateExAutoSize = BitVector32.CreateSection(1, FormStateExCalledCreateControl);
        private static readonly BitVector32.Section FormStateExInUpdateMdiControlStrip = BitVector32.CreateSection(1, FormStateExAutoSize);
        private static readonly BitVector32.Section FormStateExShowIcon = BitVector32.CreateSection(1, FormStateExInUpdateMdiControlStrip);
        private static readonly BitVector32.Section FormStateExMnemonicProcessed = BitVector32.CreateSection(1, FormStateExShowIcon);
        private static readonly BitVector32.Section FormStateExInScale = BitVector32.CreateSection(1, FormStateExMnemonicProcessed);
        private static readonly BitVector32.Section FormStateExInModalSizingLoop = BitVector32.CreateSection(1, FormStateExInScale);
        private static readonly BitVector32.Section FormStateExSettingAutoScale = BitVector32.CreateSection(1, FormStateExInModalSizingLoop);
        private static readonly BitVector32.Section FormStateExWindowBoundsWidthIsClientSize = BitVector32.CreateSection(1, FormStateExSettingAutoScale);
        private static readonly BitVector32.Section FormStateExWindowBoundsHeightIsClientSize = BitVector32.CreateSection(1, FormStateExWindowBoundsWidthIsClientSize);
        private static readonly BitVector32.Section FormStateExWindowClosing = BitVector32.CreateSection(1, FormStateExWindowBoundsHeightIsClientSize);

        private const int SizeGripSize = 16;

        private static Icon defaultIcon;
        private static readonly object internalSyncObject = new object();

        // Property store keys for properties.  The property store allocates most efficiently
        // in groups of four, so we try to lump properties in groups of four based on how
        // likely they are going to be used in a group.
        //
        private static readonly int PropAcceptButton = PropertyStore.CreateKey();
        private static readonly int PropCancelButton = PropertyStore.CreateKey();
        private static readonly int PropDefaultButton = PropertyStore.CreateKey();
        private static readonly int PropDialogOwner = PropertyStore.CreateKey();

        private static readonly int PropOwner = PropertyStore.CreateKey();
        private static readonly int PropOwnedForms = PropertyStore.CreateKey();
        private static readonly int PropMaximizedBounds = PropertyStore.CreateKey();
        private static readonly int PropOwnedFormsCount = PropertyStore.CreateKey();

        private static readonly int PropMinTrackSizeWidth = PropertyStore.CreateKey();
        private static readonly int PropMinTrackSizeHeight = PropertyStore.CreateKey();
        private static readonly int PropMaxTrackSizeWidth = PropertyStore.CreateKey();
        private static readonly int PropMaxTrackSizeHeight = PropertyStore.CreateKey();

        private static readonly int PropFormMdiParent = PropertyStore.CreateKey();
        private static readonly int PropActiveMdiChild = PropertyStore.CreateKey();
        private static readonly int PropFormerlyActiveMdiChild = PropertyStore.CreateKey();
        private static readonly int PropMdiChildFocusable = PropertyStore.CreateKey();

        private static readonly int PropMainMenuStrip = PropertyStore.CreateKey();
        private static readonly int PropMdiWindowListStrip = PropertyStore.CreateKey();
        private static readonly int PropMdiControlStrip = PropertyStore.CreateKey();

        private static readonly int PropOpacity = PropertyStore.CreateKey();
        private static readonly int PropTransparencyKey = PropertyStore.CreateKey();

        // Form per instance members
        // Note: Do not add anything to this list unless absolutely necessary.

        private BitVector32 formState = new BitVector32(0x21338);   // magic value... all the defaults... see the ctor for details...
        private BitVector32 formStateEx;

        private Icon icon;
        private Icon smallIcon;
        private Size autoScaleBaseSize = System.Drawing.Size.Empty;
        private Size minAutoSize = Size.Empty;
        private Rectangle restoredWindowBounds = new Rectangle(-1, -1, -1, -1);
        private BoundsSpecified restoredWindowBoundsSpecified;
        private DialogResult dialogResult;
        private MdiClient ctlClient;
        private NativeWindow ownerWindow;
        private bool rightToLeftLayout;

        private Rectangle restoreBounds = new Rectangle(-1, -1, -1, -1);
        private CloseReason closeReason = CloseReason.None;

        private VisualStyleRenderer sizeGripRenderer;

        /// <summary>
        ///  Initializes a new instance of the <see cref='Form'/> class.
        /// </summary>
        public Form() : base()
        {
            // Assert section.
            Debug.Assert(formState[FormStateAllowTransparency] == 0, "Failed to set formState[FormStateAllowTransparency]");
            Debug.Assert(formState[FormStateBorderStyle] == (int)FormBorderStyle.Sizable, "Failed to set formState[FormStateBorderStyle]");
            Debug.Assert(formState[FormStateTaskBar] == 1, "Failed to set formState[FormStateTaskBar]");
            Debug.Assert(formState[FormStateControlBox] == 1, "Failed to set formState[FormStateControlBox]");
            Debug.Assert(formState[FormStateKeyPreview] == 0, "Failed to set formState[FormStateKeyPreview]");
            Debug.Assert(formState[FormStateLayered] == 0, "Failed to set formState[FormStateLayered]");
            Debug.Assert(formState[FormStateMaximizeBox] == 1, "Failed to set formState[FormStateMaximizeBox]");
            Debug.Assert(formState[FormStateMinimizeBox] == 1, "Failed to set formState[FormStateMinimizeBox]");
            Debug.Assert(formState[FormStateHelpButton] == 0, "Failed to set formState[FormStateHelpButton]");
            Debug.Assert(formState[FormStateStartPos] == (int)FormStartPosition.WindowsDefaultLocation, "Failed to set formState[FormStateStartPos]");
            Debug.Assert(formState[FormStateWindowState] == (int)FormWindowState.Normal, "Failed to set formState[FormStateWindowState]");
            Debug.Assert(formState[FormStateShowWindowOnCreate] == 0, "Failed to set formState[FormStateShowWindowOnCreate]");
            Debug.Assert(formState[FormStateAutoScaling] == 1, "Failed to set formState[FormStateAutoScaling]");
            Debug.Assert(formState[FormStateSetClientSize] == 0, "Failed to set formState[FormStateSetClientSize]");
            Debug.Assert(formState[FormStateTopMost] == 0, "Failed to set formState[FormStateTopMost]");
            Debug.Assert(formState[FormStateSWCalled] == 0, "Failed to set formState[FormStateSWCalled]");
            Debug.Assert(formState[FormStateMdiChildMax] == 0, "Failed to set formState[FormStateMdiChildMax]");
            Debug.Assert(formState[FormStateRenderSizeGrip] == 0, "Failed to set formState[FormStateRenderSizeGrip]");
            Debug.Assert(formState[FormStateSizeGripStyle] == 0, "Failed to set formState[FormStateSizeGripStyle]");
            Debug.Assert(formState[FormStateIsWindowActivated] == 0, "Failed to set formState[FormStateIsWindowActivated]");
            Debug.Assert(formState[FormStateIsTextEmpty] == 0, "Failed to set formState[FormStateIsTextEmpty]");
            Debug.Assert(formState[FormStateIsActive] == 0, "Failed to set formState[FormStateIsActive]");
            Debug.Assert(formState[FormStateIconSet] == 0, "Failed to set formState[FormStateIconSet]");

            formStateEx[FormStateExShowIcon] = 1;

            SetState(States.Visible, false);
            SetState(States.TopLevel, true);
        }

        /// <summary>
        ///  Indicates the <see cref='Button'/> control on the form that is clicked when
        ///  the user presses the ENTER key.
        /// </summary>
        [DefaultValue(null)]
        [SRDescription(nameof(SR.FormAcceptButtonDescr))]
        public IButtonControl AcceptButton
        {
            get
            {
                return (IButtonControl)Properties.GetObject(PropAcceptButton);
            }
            set
            {
                if (AcceptButton != value)
                {
                    Properties.SetObject(PropAcceptButton, value);
                    UpdateDefaultButton();
                }
            }
        }

        /// <summary>
        ///  Retrieves true if this form is currently active.
        /// </summary>
        internal bool Active
        {
            get
            {
                Form parentForm = ParentForm;
                if (parentForm is null)
                {
                    return formState[FormStateIsActive] != 0;
                }

                return parentForm.ActiveControl == this && parentForm.Active;
            }
            set
            {
                Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Form::set_Active - " + Name);
                if ((formState[FormStateIsActive] != 0) != value)
                {
                    if (value)
                    {
                        if (!CanRecreateHandle())
                        {
                            return;
                        }
                    }

                    formState[FormStateIsActive] = value ? 1 : 0;

                    if (value)
                    {
                        formState[FormStateIsWindowActivated] = 1;

                        // Check if validation has been cancelled to avoid raising Validation event multiple times.
                        if (!ValidationCancelled)
                        {
                            if (ActiveControl is null)
                            {
                                // If no control is selected focus will go to form
                                SelectNextControl(null, true, true, true, false);
                            }

                            InnerMostActiveContainerControl.FocusActiveControlInternal();
                        }

                        OnActivated(EventArgs.Empty);
                    }
                    else
                    {
                        formState[FormStateIsWindowActivated] = 0;
                        OnDeactivate(EventArgs.Empty);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets the currently active form for this application.
        /// </summary>
        public static Form ActiveForm
        {
            get
            {
                IntPtr hwnd = User32.GetForegroundWindow();
                Control c = Control.FromHandle(hwnd);
                if (c != null && c is Form)
                {
                    return (Form)c;
                }
                return null;
            }
        }

        /// <summary>
        ///
        ///  Gets the currently active multiple document interface (MDI) child window.
        ///  Note: Don't use this property internally, use ActiveMdiChildInternal instead (see comments below).
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormActiveMDIChildDescr))]
        public Form ActiveMdiChild
        {
            get
            {
                Form mdiChild = ActiveMdiChildInternal;

                // We keep the active mdi child in the cached in the property store; when changing its value
                // (due to a change to one of the following properties/methods: Visible, Enabled, Active, Show/Hide,
                // Focus() or as the  result of WM_SETFOCUS/WM_ACTIVATE/WM_MDIACTIVATE) we temporarily set it to null
                // (to properly handle menu merging among other things) rendering the cache out-of-date; the problem
                // arises when the user has an event handler that is raised during this process; in that case we ask
                // Windows for it (see ActiveMdiChildFromWindows).

                if (mdiChild is null)
                {
                    // If this.MdiClient != null it means this.IsMdiContainer == true.
                    if (ctlClient != null && ctlClient.IsHandleCreated)
                    {
                        IntPtr hwnd = User32.SendMessageW(ctlClient, User32.WM.MDIGETACTIVE);
                        mdiChild = Control.FromHandle(hwnd) as Form;
                    }
                }
                if (mdiChild != null && mdiChild.Visible && mdiChild.Enabled)
                {
                    return mdiChild;
                }
                return null;
            }
        }

        /// <summary>
        ///  Property to be used internally.  See comments a on ActiveMdiChild property.
        /// </summary>
        internal Form ActiveMdiChildInternal
        {
            get
            {
                return (Form)Properties.GetObject(PropActiveMdiChild);
            }

            set
            {
                Properties.SetObject(PropActiveMdiChild, value);
            }
        }

        //we don't repaint the mdi child that used to be active any more.  We used to do this in Activated, but no
        //longer do because of added event Deactivate.
        private Form FormerlyActiveMdiChild
        {
            get
            {
                return (Form)Properties.GetObject(PropFormerlyActiveMdiChild);
            }

            set
            {
                Properties.SetObject(PropFormerlyActiveMdiChild, value);
            }
        }

        /// <summary>
        ///  Gets or sets
        ///  a value indicating whether the opacity of the form can be
        ///  adjusted.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ControlAllowTransparencyDescr))]
        public bool AllowTransparency
        {
            get
            {
                return formState[FormStateAllowTransparency] != 0;
            }
            set
            {
                if (value != (formState[FormStateAllowTransparency] != 0))
                {
                    formState[FormStateAllowTransparency] = (value ? 1 : 0);

                    formState[FormStateLayered] = formState[FormStateAllowTransparency];

                    UpdateStyles();

                    if (!value)
                    {
                        if (Properties.ContainsObject(PropOpacity))
                        {
                            Properties.SetObject(PropOpacity, (object)1.0f);
                        }
                        if (Properties.ContainsObject(PropTransparencyKey))
                        {
                            Properties.SetObject(PropTransparencyKey, Color.Empty);
                        }
                        UpdateLayered();
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the form will adjust its size
        ///  to fit the height of the font used on the form and scale
        ///  its controls.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.FormAutoScaleDescr)),
        Obsolete("This property has been deprecated. Use the AutoScaleMode property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoScale
        {
            get
            {
                return formState[FormStateAutoScaling] != 0;
            }

            set
            {
                formStateEx[FormStateExSettingAutoScale] = 1;
                try
                {
                    if (value)
                    {
                        formState[FormStateAutoScaling] = 1;

                        // if someone insists on auto scaling,
                        // force the new property back to none so they
                        // don't compete.
                        AutoScaleMode = AutoScaleMode.None;
                    }
                    else
                    {
                        formState[FormStateAutoScaling] = 0;
                    }
                }
                finally
                {
                    formStateEx[FormStateExSettingAutoScale] = 0;
                }
            }
        }

        // Our STRONG recommendation to customers is to upgrade to AutoScaleDimensions
        // however, since this is generated by default in Everett, and there's not a 1:1 mapping of
        // the old to the new, we are un-obsoleting the setter for AutoScaleBaseSize only.
        /// <summary>
        ///  The base size used for autoscaling. The AutoScaleBaseSize is used
        ///  internally to determine how much to scale the form when AutoScaling is
        ///  used.
        /// </summary>
        //
        // Virtual so subclasses like PrintPreviewDialog can prevent changes.
        [Localizable(true)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual Size AutoScaleBaseSize
        {
            get
            {
                if (autoScaleBaseSize.IsEmpty)
                {
                    SizeF real = GetAutoScaleSize(Font);
                    return new Size((int)Math.Round(real.Width), (int)Math.Round(real.Height));
                }
                return autoScaleBaseSize;
            }

            set
            {
                // Only allow the set when not in designmode, this prevents us from
                // preserving an old value.  The form design should prevent this for
                // us by shadowing this property, so we just assert that the designer
                // is doing its job.
                //
                Debug.Assert(!DesignMode, "Form designer should not allow base size set in design mode.");
                autoScaleBaseSize = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the form implements
        ///  autoscrolling.
        /// </summary>
        [Localizable(true)]
        public override bool AutoScroll
        {
            get => base.AutoScroll;

            set
            {
                if (value)
                {
                    IsMdiContainer = false;
                }
                base.AutoScroll = value;
            }
        }

        // Forms implement their own AutoSize in OnLayout so we shadow this property
        // just in case someone parents a Form to a container control.
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize
        {
            get { return formStateEx[FormStateExAutoSize] != 0; }
            set
            {
                if (value != AutoSize)
                {
                    formStateEx[FormStateExAutoSize] = value ? 1 : 0;
                    if (!AutoSize)
                    {
                        minAutoSize = Size.Empty;
                        // If we just disabled AutoSize, restore the original size.
                        Size = CommonProperties.GetSpecifiedBounds(this).Size;
                    }
                    LayoutTransaction.DoLayout(this, this, PropertyNames.AutoSize);
                    OnAutoSizeChanged(EventArgs.Empty);
                }
                Debug.Assert(AutoSize == value, "Error detected setting Form.AutoSize.");
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        new public event EventHandler AutoSizeChanged
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
                    Control toLayout = DesignMode || ParentInternal is null ? this : ParentInternal;

                    if (toLayout != null)
                    {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if (toLayout.LayoutEngine == DefaultLayout.Instance)
                        {
                            toLayout.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(toLayout, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates whether controls in this container will be automatically validated when the focus changes.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override AutoValidate AutoValidate
        {
            get => base.AutoValidate;
            set => base.AutoValidate = value;
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public new event EventHandler AutoValidateChanged
        {
            add => base.AutoValidateChanged += value;
            remove => base.AutoValidateChanged -= value;
        }

        /// <summary>
        ///  The background color of this control. This is an ambient property and
        ///  will always return a non-null value.
        /// </summary>
        public override Color BackColor
        {
            get
            {
                // Forms should not inherit BackColor from their parent,
                // particularly if the parent is an MDIClient.
                Color c = RawBackColor; // inheritedProperties.BackColor
                if (!c.IsEmpty)
                {
                    return c;
                }

                return DefaultBackColor;
            }
            set => base.BackColor = value;
        }

        private bool CalledClosing
        {
            get
            {
                return formStateEx[FormStateExCalledClosing] != 0;
            }
            set
            {
                formStateEx[FormStateExCalledClosing] = (value ? 1 : 0);
            }
        }

        private bool CalledCreateControl
        {
            get
            {
                return formStateEx[FormStateExCalledCreateControl] != 0;
            }
            set
            {
                formStateEx[FormStateExCalledCreateControl] = (value ? 1 : 0);
            }
        }

        private bool CalledMakeVisible
        {
            get
            {
                return formStateEx[FormStateExCalledMakeVisible] != 0;
            }
            set
            {
                formStateEx[FormStateExCalledMakeVisible] = (value ? 1 : 0);
            }
        }

        private bool CalledOnLoad
        {
            get
            {
                return formStateEx[FormStateExCalledOnLoad] != 0;
            }
            set
            {
                formStateEx[FormStateExCalledOnLoad] = (value ? 1 : 0);
            }
        }

        /// <summary>
        ///  Gets or sets the border style of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(FormBorderStyle.Sizable)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.FormBorderStyleDescr))]
        public FormBorderStyle FormBorderStyle
        {
            get => (FormBorderStyle)formState[FormStateBorderStyle];
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FormBorderStyle.None, (int)FormBorderStyle.SizableToolWindow))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormBorderStyle));
                }

                formState[FormStateBorderStyle] = (int)value;
                if (formState[FormStateSetClientSize] == 1 && !IsHandleCreated)
                {
                    ClientSize = ClientSize;
                }

                // Since setting the border style induces a call to SetBoundsCore, which,
                // when the WindowState is not Normal, will cause the values stored in the field restoredWindowBounds
                // to be replaced. The side-effect of this, of course, is that when the form window is restored,
                // the Form's size is restored to the size it was when in a non-Normal state.
                // So, we need to cache these values now before the call to UpdateFormStyles() to prevent
                // these existing values from being lost. Then, if the WindowState is something other than
                // FormWindowState.Normal after the call to UpdateFormStyles(), restore these cached values to
                // the restoredWindowBounds field.
                Rectangle preClientUpdateRestoredWindowBounds = restoredWindowBounds;
                BoundsSpecified preClientUpdateRestoredWindowBoundsSpecified = restoredWindowBoundsSpecified;
                int preWindowBoundsWidthIsClientSize = formStateEx[FormStateExWindowBoundsWidthIsClientSize];
                int preWindowBoundsHeightIsClientSize = formStateEx[FormStateExWindowBoundsHeightIsClientSize];

                UpdateFormStyles();

                // In Windows Theme, the FixedDialog tend to have a small Icon.
                // So to make this behave uniformly with other styles, we need to make
                // the call to UpdateIcon after the the form styles have been updated.
                if (formState[FormStateIconSet] == 0)
                {
                    UpdateWindowIcon(false);
                }

                // Now restore the values cached above.
                if (WindowState != FormWindowState.Normal)
                {
                    restoredWindowBounds = preClientUpdateRestoredWindowBounds;
                    restoredWindowBoundsSpecified = preClientUpdateRestoredWindowBoundsSpecified;
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] = preWindowBoundsWidthIsClientSize;
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] = preWindowBoundsHeightIsClientSize;
                }
            }
        }

        /// <summary>
        ///  Gets
        ///  or
        ///  sets the button control that will be clicked when the
        ///  user presses the ESC key.
        /// </summary>
        [DefaultValue(null)]
        [SRDescription(nameof(SR.FormCancelButtonDescr))]
        public IButtonControl CancelButton
        {
            get
            {
                return (IButtonControl)Properties.GetObject(PropCancelButton);
            }
            set
            {
                Properties.SetObject(PropCancelButton, value);

                if (value != null && value.DialogResult == DialogResult.None)
                {
                    value.DialogResult = DialogResult.Cancel;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the size of the client area of the form.
        /// </summary>
        [Localizable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        new public Size ClientSize
        {
            get => base.ClientSize;
            set => base.ClientSize = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether a control box is displayed in the
        ///  caption bar of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FormControlBoxDescr))]
        public bool ControlBox
        {
            get => formState[FormStateControlBox] != 0;
            set
            {
                formState[FormStateControlBox] = value ? 1 : 0;
                UpdateFormStyles();
            }
        }

        /// <summary>
        ///  Retrieves the CreateParams used to create the window.
        ///  If a subclass overrides this function, it must call the base implementation.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (IsHandleCreated && (WindowStyle & (int)User32.WS.DISABLED) != 0)
                {
                    // Forms that are parent of a modal dialog must keep their WS_DISABLED style
                    cp.Style |= (int)User32.WS.DISABLED;
                }
                else if (TopLevel)
                {
                    // It doesn't seem to make sense to allow a top-level form to be disabled
                    //
                    cp.Style &= ~(int)User32.WS.DISABLED;
                }

                if (TopLevel && (formState[FormStateLayered] != 0))
                {
                    cp.ExStyle |= (int)User32.WS_EX.LAYERED;
                }

                IWin32Window dialogOwner = (IWin32Window)Properties.GetObject(PropDialogOwner);
                if (dialogOwner != null)
                {
                    cp.Parent = Control.GetSafeHandle(dialogOwner);
                }

                FillInCreateParamsBorderStyles(cp);
                FillInCreateParamsWindowState(cp);
                FillInCreateParamsBorderIcons(cp);

                if (formState[FormStateTaskBar] != 0)
                {
                    cp.ExStyle |= (int)User32.WS_EX.APPWINDOW;
                }

                FormBorderStyle borderStyle = FormBorderStyle;
                if (!ShowIcon &&
                    (borderStyle == FormBorderStyle.Sizable ||
                     borderStyle == FormBorderStyle.Fixed3D ||
                     borderStyle == FormBorderStyle.FixedSingle))
                {
                    cp.ExStyle |= (int)User32.WS_EX.DLGMODALFRAME;
                }

                if (IsMdiChild)
                {
                    if (Visible
                            && (WindowState == FormWindowState.Maximized
                                || WindowState == FormWindowState.Normal))
                    {
                        Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                        Form form = formMdiParent.ActiveMdiChildInternal;

                        if (form != null
                            && form.WindowState == FormWindowState.Maximized)
                        {
                            cp.Style |= (int)User32.WS.MAXIMIZE;
                            formState[FormStateWindowState] = (int)FormWindowState.Maximized;
                            SetState(States.SizeLockedByOS, true);
                        }
                    }

                    if (formState[FormStateMdiChildMax] != 0)
                    {
                        cp.Style |= (int)User32.WS.MAXIMIZE;
                    }
                    cp.ExStyle |= (int)User32.WS_EX.MDICHILD;
                }

                if (TopLevel || IsMdiChild)
                {
                    FillInCreateParamsStartPosition(cp);
                    // Delay setting to visible until after the handle gets created
                    // to allow applyClientSize to adjust the size before displaying
                    // the form.
                    //
                    if ((cp.Style & (int)User32.WS.VISIBLE) != 0)
                    {
                        formState[FormStateShowWindowOnCreate] = 1;
                        cp.Style &= ~(int)User32.WS.VISIBLE;
                    }
                    else
                    {
                        formState[FormStateShowWindowOnCreate] = 0;
                    }
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
                {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= (int)(User32.WS_EX.LAYOUTRTL | User32.WS_EX.NOINHERITLAYOUT);
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(int)(User32.WS_EX.RTLREADING | User32.WS_EX.RIGHT | User32.WS_EX.LEFTSCROLLBAR);
                }
                return cp;
            }
        }

        internal CloseReason CloseReason
        {
            get { return closeReason; }
            set { closeReason = value; }
        }
        /// <summary>
        ///  The default icon used by the Form. This is the standard "windows forms" icon.
        /// </summary>
        internal static Icon DefaultIcon
        {
            get
            {
                // Avoid locking if the value is filled in...
                //
                if (defaultIcon is null)
                {
                    lock (internalSyncObject)
                    {
                        // Once we grab the lock, we re-check the value to avoid a
                        // race condition.
                        //
                        if (defaultIcon is null)
                        {
                            defaultIcon = new Icon(typeof(Form), "wfc");
                        }
                    }
                }
                return defaultIcon;
            }
        }

        protected override ImeMode DefaultImeMode
        {
            get
            {
                return ImeMode.NoControl;
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
                return new Size(300, 300);
            }
        }

        /// <summary>
        ///  Gets or sets the size and location of the form on the Windows desktop.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormDesktopBoundsDescr))]
        public Rectangle DesktopBounds
        {
            get
            {
                Rectangle screen = SystemInformation.WorkingArea;
                Rectangle bounds = Bounds;
                bounds.X -= screen.X;
                bounds.Y -= screen.Y;
                return bounds;
            }

            set
            {
                SetDesktopBounds(value.X, value.Y, value.Width, value.Height);
            }
        }

        /// <summary>
        ///  Gets or sets the location of the form on the Windows desktop.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormDesktopLocationDescr))]
        public Point DesktopLocation
        {
            get
            {
                Rectangle screen = SystemInformation.WorkingArea;
                Point loc = Location;
                loc.X -= screen.X;
                loc.Y -= screen.Y;
                return loc;
            }

            set
            {
                SetDesktopLocation(value.X, value.Y);
            }
        }

        /// <summary>
        ///  Gets or sets the dialog result for the form.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormDialogResultDescr))]
        public DialogResult DialogResult
        {
            get
            {
                return dialogResult;
            }

            set
            {
                //valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int)DialogResult.No))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DialogResult));
                }

                dialogResult = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether a
        ///  help button should be displayed in the caption box of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FormHelpButtonDescr))]
        public bool HelpButton
        {
            get
            {
                return formState[FormStateHelpButton] != 0;
            }

            set
            {
                if (value)
                {
                    formState[FormStateHelpButton] = 1;
                }
                else
                {
                    formState[FormStateHelpButton] = 0;
                }
                UpdateFormStyles();
            }
        }

        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormHelpButtonClickedDescr))]
        public event CancelEventHandler HelpButtonClicked
        {
            add => Events.AddHandler(EVENT_HELPBUTTONCLICKED, value);
            remove => Events.RemoveHandler(EVENT_HELPBUTTONCLICKED, value);
        }

        /// <summary>
        ///  Gets or sets the icon for the form.
        /// </summary>
        [AmbientValue(null)]
        [Localizable(true)]
        [SRCategory(nameof(SR.CatWindowStyle))]
        [SRDescription(nameof(SR.FormIconDescr))]
        public Icon Icon
        {
            get
            {
                if (formState[FormStateIconSet] == 0)
                {
                    return DefaultIcon;
                }

                return icon;
            }
            set
            {
                if (icon != value)
                {
                    // If the user is setting the default back in, treat this
                    // as a reset.
                    if (value == defaultIcon)
                    {
                        value = null;
                    }

                    // If null is passed, reset the icon.
                    formState[FormStateIconSet] = value is null ? 0 : 1;
                    icon = value;

                    if (smallIcon != null)
                    {
                        smallIcon.Dispose();
                        smallIcon = null;
                    }

                    UpdateWindowIcon(true);
                }
            }
        }

        /// <summary>
        ///  Determines whether the window is closing.
        /// </summary>
        private bool IsClosing
        {
            get
            {
                return formStateEx[FormStateExWindowClosing] == 1;
            }
            set
            {
                formStateEx[FormStateExWindowClosing] = value ? 1 : 0;
            }
        }

        // Returns a more accurate statement of whether or not the form is maximized.
        // during handle creation, an MDIChild is created as not maximized.
        private bool IsMaximized
        {
            get
            {
                return (WindowState == FormWindowState.Maximized || (IsMdiChild && (formState[FormStateMdiChildMax] == 1)));
            }
        }

        /// <summary>
        ///  Gets a value indicating whether the form is a multiple document
        ///  interface (MDI) child form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormIsMDIChildDescr))]
        public bool IsMdiChild
        {
            get
            {
                return (Properties.GetObject(PropFormMdiParent) != null);
            }
        }

        // Deactivates active MDI child and temporarily marks it as unfocusable,
        // so that WM_SETFOCUS sent to MDIClient does not activate that child. (See MdiClient.WndProc).
        internal bool IsMdiChildFocusable
        {
            get
            {
                if (Properties.ContainsObject(PropMdiChildFocusable))
                {
                    return (bool)Properties.GetObject(PropMdiChildFocusable);
                }
                return false;
            }
            set
            {
                if (value != IsMdiChildFocusable)
                {
                    Properties.SetObject(PropMdiChildFocusable, value);
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the form is a container for multiple document interface
        ///  (MDI) child forms.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FormIsMDIContainerDescr))]
        public bool IsMdiContainer
        {
            get
            {
                return ctlClient != null;
            }

            set
            {
                if (value == IsMdiContainer)
                {
                    return;
                }

                if (value)
                {
                    Debug.Assert(ctlClient is null, "why isn't ctlClient null");
                    AllowTransparency = false;
                    Controls.Add(new MdiClient());
                }
                else
                {
                    Debug.Assert(ctlClient != null, "why is ctlClient null");
                    ActiveMdiChildInternal = null;
                    ctlClient.Dispose();
                }
                //since we paint the background when mdi is true, we need
                //to invalidate here
                //
                Invalidate();
            }
        }

        /// <summary>
        ///  Determines if this form should display a warning banner when the form is
        ///  displayed in an unsecure mode.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsRestrictedWindow => false;

        /// <summary>
        ///  Gets or sets a value
        ///  indicating whether the form will receive key events
        ///  before the event is passed to the control that has focus.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.FormKeyPreviewDescr))]
        public bool KeyPreview
        {
            get
            {
                return formState[FormStateKeyPreview] != 0;
            }
            set
            {
                if (value)
                {
                    formState[FormStateKeyPreview] = 1;
                }
                else
                {
                    formState[FormStateKeyPreview] = 0;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the location of the form.
        /// </summary>
        [SettingsBindable(true)]
        public new Point Location
        {
            get => base.Location;
            set => base.Location = value;
        }

        /// <summary>
        ///  Gets the size of the form when it is
        ///  maximized.
        /// </summary>
        protected Rectangle MaximizedBounds
        {
            get
            {
                return Properties.GetRectangle(PropMaximizedBounds, out _);
            }
            set
            {
                if (!value.Equals(MaximizedBounds))
                {
                    Properties.SetRectangle(PropMaximizedBounds, value);
                    OnMaximizedBoundsChanged(EventArgs.Empty);
                }
            }
        }

        private static readonly object EVENT_MAXIMIZEDBOUNDSCHANGED = new object();

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.FormOnMaximizedBoundsChangedDescr))]
        public event EventHandler MaximizedBoundsChanged
        {
            add => Events.AddHandler(EVENT_MAXIMIZEDBOUNDSCHANGED, value);

            remove => Events.RemoveHandler(EVENT_MAXIMIZEDBOUNDSCHANGED, value);
        }

        /// <summary>
        ///  Gets the maximum size the form can be resized to.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [SRDescription(nameof(SR.FormMaximumSizeDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(typeof(Size), "0, 0")]
        public override Size MaximumSize
        {
            get
            {
                if (Properties.ContainsInteger(PropMaxTrackSizeWidth))
                {
                    return new Size(Properties.GetInteger(PropMaxTrackSizeWidth), Properties.GetInteger(PropMaxTrackSizeHeight));
                }
                return Size.Empty;
            }
            set
            {
                if (!value.Equals(MaximumSize))
                {
                    if (value.Width < 0 || value.Height < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(MaximumSize));
                    }

                    Properties.SetInteger(PropMaxTrackSizeWidth, value.Width);
                    Properties.SetInteger(PropMaxTrackSizeHeight, value.Height);

                    // Bump minimum size if necessary
                    //
                    if (!MinimumSize.IsEmpty && !value.IsEmpty)
                    {
                        if (Properties.GetInteger(PropMinTrackSizeWidth) > value.Width)
                        {
                            Properties.SetInteger(PropMinTrackSizeWidth, value.Width);
                        }

                        if (Properties.GetInteger(PropMinTrackSizeHeight) > value.Height)
                        {
                            Properties.SetInteger(PropMinTrackSizeHeight, value.Height);
                        }
                    }

                    // Keep form size within new limits
                    //
                    Size size = Size;
                    if (!value.IsEmpty && (size.Width > value.Width || size.Height > value.Height))
                    {
                        Size = new Size(Math.Min(size.Width, value.Width), Math.Min(size.Height, value.Height));
                    }

                    OnMaximumSizeChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.FormOnMaximumSizeChangedDescr))]
        public event EventHandler MaximumSizeChanged
        {
            add => Events.AddHandler(EVENT_MAXIMUMSIZECHANGED, value);

            remove => Events.RemoveHandler(EVENT_MAXIMUMSIZECHANGED, value);
        }

        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.FormMenuStripDescr))]
        [TypeConverter(typeof(ReferenceConverter))]
        public MenuStrip MainMenuStrip
        {
            get
            {
                return (MenuStrip)Properties.GetObject(PropMainMenuStrip);
            }
            set
            {
                Properties.SetObject(PropMainMenuStrip, value);
                if (IsHandleCreated)
                {
                    UpdateMenuHandles();
                }
            }
        }

        /// <summary>
        ///  Hide Margin/MarginChanged
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new Padding Margin
        {
            get => base.Margin;
            set => base.Margin = value;
        }

        /// <summary>
        ///  Hide Margin/MarginChanged
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MarginChanged
        {
            add => base.MarginChanged += value;
            remove => base.MarginChanged -= value;
        }

        /// <summary>
        ///  Gets the minimum size the form can be resized to.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [SRDescription(nameof(SR.FormMinimumSizeDescr))]
        [RefreshProperties(RefreshProperties.Repaint)]
        public override Size MinimumSize
        {
            get
            {
                if (Properties.ContainsInteger(PropMinTrackSizeWidth))
                {
                    return new Size(Properties.GetInteger(PropMinTrackSizeWidth), Properties.GetInteger(PropMinTrackSizeHeight));
                }

                return DefaultMinimumSize;
            }
            set
            {
                if (!value.Equals(MinimumSize))
                {
                    if (value.Width < 0 || value.Height < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(MinimumSize));
                    }

                    Rectangle bounds = Bounds;
                    bounds.Size = value;
                    value = WindowsFormsUtils.ConstrainToScreenWorkingAreaBounds(bounds).Size;

                    Properties.SetInteger(PropMinTrackSizeWidth, value.Width);
                    Properties.SetInteger(PropMinTrackSizeHeight, value.Height);

                    // Bump maximum size if necessary
                    if (!MaximumSize.IsEmpty && !value.IsEmpty)
                    {
                        if (Properties.GetInteger(PropMaxTrackSizeWidth) < value.Width)
                        {
                            Properties.SetInteger(PropMaxTrackSizeWidth, value.Width);
                        }

                        if (Properties.GetInteger(PropMaxTrackSizeHeight) < value.Height)
                        {
                            Properties.SetInteger(PropMaxTrackSizeHeight, value.Height);
                        }
                    }

                    // Keep form size within new limits
                    Size size = Size;
                    if (size.Width < value.Width || size.Height < value.Height)
                    {
                        Size = new Size(Math.Max(size.Width, value.Width), Math.Max(size.Height, value.Height));
                    }

                    if (IsHandleCreated)
                    {
                        // "Move" the form to the same size and position to prevent windows from moving it
                        // when the user tries to grab a resizing border.
                        User32.SetWindowPos(
                            new HandleRef(this, Handle),
                            User32.HWND_TOP,
                            Location.X,
                            Location.Y,
                            Size.Width,
                            Size.Height,
                            User32.SWP.NOZORDER);
                    }

                    OnMinimumSizeChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.FormOnMinimumSizeChangedDescr))]
        public event EventHandler MinimumSizeChanged
        {
            add => Events.AddHandler(EVENT_MINIMUMSIZECHANGED, value);

            remove => Events.RemoveHandler(EVENT_MINIMUMSIZECHANGED, value);
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the maximize button is
        ///  displayed in the caption bar of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FormMaximizeBoxDescr))]
        public bool MaximizeBox
        {
            get
            {
                return formState[FormStateMaximizeBox] != 0;
            }
            set
            {
                if (value)
                {
                    formState[FormStateMaximizeBox] = 1;
                }
                else
                {
                    formState[FormStateMaximizeBox] = 0;
                }
                UpdateFormStyles();
            }
        }

        /// <summary>
        ///  Gets an array of forms that represent the
        ///  multiple document interface (MDI) child forms that are parented to this
        ///  form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormMDIChildrenDescr))]
        public Form[] MdiChildren
        {
            get
            {
                if (ctlClient != null)
                {
                    return ctlClient.MdiChildren;
                }
                else
                {
                    return Array.Empty<Form>();
                }
            }
        }

        /// <summary>
        ///  Gets the MDIClient that the MDI container form is using to contain Multiple Document Interface (MDI) child forms,
        ///  if this is an MDI container form.
        ///  Represents the client area of a Multiple Document Interface (MDI) Form window, also known as the MDI child window.
        /// </summary>
        internal MdiClient MdiClient
        {
            get
            {
                return ctlClient;
            }
        }

        /// <summary>
        ///  Indicates the current multiple document
        ///  interface (MDI) parent form of this form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormMDIParentDescr))]
        public Form MdiParent
        {
            get
            {
                return MdiParentInternal;
            }
            set
            {
                MdiParentInternal = value;
            }
        }

        private Form MdiParentInternal
        {
            get => (Form)Properties.GetObject(PropFormMdiParent);
            set
            {
                Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                if (value == formMdiParent && (value != null || ParentInternal is null))
                {
                    return;
                }

                if (value != null && CreateThreadId != value.CreateThreadId)
                {
                    throw new ArgumentException(SR.AddDifferentThreads, nameof(value));
                }

                bool oldVisibleBit = GetState(States.Visible);
                Visible = false;

                try
                {
                    if (value is null)
                    {
                        ParentInternal = null;
                        SetTopLevel(true);
                    }
                    else
                    {
                        if (IsMdiContainer)
                        {
                            throw new ArgumentException(SR.FormMDIParentAndChild, nameof(value));
                        }
                        if (!value.IsMdiContainer)
                        {
                            throw new ArgumentException(SR.MDIParentNotContainer, nameof(value));
                        }

                        // Setting TopLevel forces a handle recreate before Parent is set,
                        // which causes problems because we try to assign an MDI child to the
                        // parking window, which can't take MDI children. We explicitly destroy
                        // and create the handle here.

                        Dock = DockStyle.None;
                        Properties.SetObject(PropFormMdiParent, value);

                        SetState(States.TopLevel, false);
                        ParentInternal = value.MdiClient;

                        // If it is an MDIChild, and it is not yet visible, we'll
                        // hold off on recreating the window handle. We'll do that
                        // when MdiChild's visibility is set to true (see

                        // But if the handle has already been created, we need to destroy it
                        // so the form gets MDI-parented properly.
                        if (ParentInternal.IsHandleCreated && IsMdiChild && IsHandleCreated)
                        {
                            DestroyHandle();
                        }
                    }

                    UpdateMenuHandles();
                }
                finally
                {
                    UpdateStyles();
                    Visible = oldVisibleBit;
                }
            }
        }

        private MdiWindowListStrip MdiWindowListStrip
        {
            get { return Properties.GetObject(PropMdiWindowListStrip) as MdiWindowListStrip; }
            set { Properties.SetObject(PropMdiWindowListStrip, value); }
        }

        private MdiControlStrip MdiControlStrip
        {
            get { return Properties.GetObject(PropMdiControlStrip) as MdiControlStrip; }
            set { Properties.SetObject(PropMdiControlStrip, value); }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the minimize button is displayed in the caption bar of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(true)]
        [SRDescription(nameof(SR.FormMinimizeBoxDescr))]
        public bool MinimizeBox
        {
            get
            {
                return formState[FormStateMinimizeBox] != 0;
            }
            set
            {
                if (value)
                {
                    formState[FormStateMinimizeBox] = 1;
                }
                else
                {
                    formState[FormStateMinimizeBox] = 0;
                }
                UpdateFormStyles();
            }
        }

        /// <summary>
        ///  Gets a value indicating whether this form is
        ///  displayed modally.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormModalDescr))]
        public bool Modal
        {
            get
            {
                return GetState(States.Modal);
            }
        }

        /// <summary>
        ///  Determines the opacity of the form. This can only be set on top level controls.
        ///  Opacity requires Windows 2000 or later, and is ignored on earlier operating systems.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [TypeConverter(typeof(OpacityConverter))]
        [SRDescription(nameof(SR.FormOpacityDescr))]
        [DefaultValue(1.0)]
        public double Opacity
        {
            get
            {
                object opacity = Properties.GetObject(PropOpacity);
                if (opacity != null)
                {
                    return Convert.ToDouble(opacity, CultureInfo.InvariantCulture);
                }

                return 1.0f;
            }
            set
            {
                if (value > 1.0)
                {
                    value = 1.0f;
                }
                else if (value < 0.0)
                {
                    value = 0.0f;
                }

                Properties.SetObject(PropOpacity, value);

                bool oldLayered = (formState[FormStateLayered] != 0);

                if (OpacityAsByte < 255)
                {
                    AllowTransparency = true;
                    if (formState[FormStateLayered] != 1)
                    {
                        formState[FormStateLayered] = 1;
                        if (!oldLayered)
                        {
                            UpdateStyles();
                        }
                    }
                }
                else
                {
                    formState[FormStateLayered] = (TransparencyKey != Color.Empty) ? 1 : 0;
                    if (oldLayered != (formState[FormStateLayered] != 0))
                    {
                        int exStyle = unchecked((int)(long)User32.GetWindowLong(this, User32.GWL.EXSTYLE));
                        CreateParams cp = CreateParams;
                        if (exStyle != cp.ExStyle)
                        {
                            User32.SetWindowLong(this, User32.GWL.EXSTYLE, (IntPtr)cp.ExStyle);
                        }
                    }
                }

                UpdateLayered();
            }
        }

        private byte OpacityAsByte
        {
            get
            {
                return (byte)(Opacity * 255.0f);
            }
        }

        /// <summary>
        ///  Gets an array of <see cref='Form'/> objects that represent all forms that are owned by this form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormOwnedFormsDescr))]
        public Form[] OwnedForms
        {
            get
            {
                Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

                Form[] result = new Form[ownedFormsCount];
                if (ownedFormsCount > 0)
                {
                    Array.Copy(ownedForms, 0, result, 0, ownedFormsCount);
                }

                return result;
            }
        }

        /// <summary>
        ///  Gets or sets the form that owns this form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.FormOwnerDescr))]
        public Form Owner
        {
            get
            {
                return OwnerInternal;
            }
            set
            {
                Form ownerOld = OwnerInternal;
                if (ownerOld == value)
                {
                    return;
                }

                if (value != null && !TopLevel)
                {
                    throw new ArgumentException(SR.NonTopLevelCantHaveOwner, nameof(value));
                }

                CheckParentingCycle(this, value);
                CheckParentingCycle(value, this);

                Properties.SetObject(PropOwner, null);

                if (ownerOld != null)
                {
                    ownerOld.RemoveOwnedForm(this);
                }

                Properties.SetObject(PropOwner, value);

                if (value != null)
                {
                    value.AddOwnedForm(this);
                }

                UpdateHandleWithOwner();
            }
        }

        internal Form OwnerInternal
        {
            get
            {
                return (Form)Properties.GetObject(PropOwner);
            }
        }

        /// <summary>
        ///  Gets or sets the restored bounds of the Form.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Rectangle RestoreBounds
        {
            get
            {
                if (restoreBounds.Width == -1
                    && restoreBounds.Height == -1
                    && restoreBounds.X == -1
                    && restoreBounds.Y == -1)
                {
                    // Form scaling depends on this property being
                    // set correctly.  In some cases (where the size has not yet been set or
                    // has only been set to the default, restoreBounds will remain uninitialized until the
                    // handle has been created.  In this case, return the current Bounds.
                    return Bounds;
                }
                return restoreBounds;
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
                return rightToLeftLayout;
            }

            set
            {
                if (value != rightToLeftLayout)
                {
                    rightToLeftLayout = value;
                    using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                    {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        internal override Control ParentInternal
        {
            get => base.ParentInternal;
            set
            {
                if (value != null)
                {
                    Owner = null;
                }
                base.ParentInternal = value;
            }
        }

        /// <summary>
        ///  If ShowInTaskbar is true then the form will be displayed in the Windows Taskbar.
        /// </summary>
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatWindowStyle))]
        [SRDescription(nameof(SR.FormShowInTaskbarDescr))]
        public bool ShowInTaskbar
        {
            get => formState[FormStateTaskBar] != 0;
            set
            {
                if (ShowInTaskbar != value)
                {
                    formState[FormStateTaskBar] = value ? 1 : 0;
                    if (IsHandleCreated)
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether an icon is displayed in the
        ///  caption bar of the form.
        ///  If ControlBox == false, then the icon won't be shown no matter what
        ///  the value of ShowIcon is
        /// </summary>
        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatWindowStyle))]
        [SRDescription(nameof(SR.FormShowIconDescr))]
        public bool ShowIcon
        {
            get => formStateEx[FormStateExShowIcon] != 0;
            set
            {
                formStateEx[FormStateExShowIcon] = value ? 1 : 0;
                if (!value)
                {
                    UpdateStyles();
                }

                UpdateWindowIcon(true);
            }
        }

        internal override User32.SW ShowParams
        {
            get
            {
                // From MSDN:
                //      The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter.

                //      As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to ShowWindow if the program that launched the application specifies startup information in the STARTUPINFO structure. In this case, ShowWindow uses the information specified in the STARTUPINFO structure to show the window. On subsequent calls, the application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the application. This behavior is designed for the following situations:
                //
                //      Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.
                //      Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared, and later call ShowWindow with the SW_SHOW flag set to make it visible.
                //

                switch (WindowState)
                {
                    case FormWindowState.Maximized:
                        return User32.SW.SHOWMAXIMIZED;
                    case FormWindowState.Minimized:
                        return User32.SW.SHOWMINIMIZED;
                }
                if (ShowWithoutActivation)
                {
                    return User32.SW.SHOWNOACTIVATE;
                }
                return User32.SW.SHOW;
            }
        }

        /// <summary>
        ///  When this property returns true, the internal ShowParams property will return User32.ShowWindowCommand.SHOWNOACTIVATE.
        /// </summary>
        [Browsable(false)]
        protected virtual bool ShowWithoutActivation
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Gets or sets the size of the form.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Localizable(false)]
        new public Size Size
        {
            get => base.Size;
            set => base.Size = value;
        }

        /// <summary>
        ///  Gets or sets the style of size grip to display in the lower-left corner of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [DefaultValue(SizeGripStyle.Auto)]
        [SRDescription(nameof(SR.FormSizeGripStyleDescr))]
        public SizeGripStyle SizeGripStyle
        {
            get
            {
                return (SizeGripStyle)formState[FormStateSizeGripStyle];
            }
            set
            {
                if (SizeGripStyle != value)
                {
                    //do some bounds checking here
                    //
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)SizeGripStyle.Auto, (int)SizeGripStyle.Hide))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(SizeGripStyle));
                    }

                    formState[FormStateSizeGripStyle] = (int)value;
                    UpdateRenderSizeGrip();
                }
            }
        }

        /// <summary>
        ///  Gets or sets the
        ///  starting position of the form at run time.
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(FormStartPosition.WindowsDefaultLocation)]
        [SRDescription(nameof(SR.FormStartPositionDescr))]
        public FormStartPosition StartPosition
        {
            get
            {
                return (FormStartPosition)formState[FormStateStartPos];
            }
            set
            {
                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FormStartPosition.Manual, (int)FormStartPosition.CenterParent))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormStartPosition));
                }
                formState[FormStateStartPos] = (int)value;
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public int TabIndex
        {
            get => base.TabIndex;
            set => base.TabIndex = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabIndexChanged
        {
            add => base.TabIndexChanged += value;
            remove => base.TabIndexChanged -= value;
        }

        /// <summary>
        ///  This property has no effect on Form, we need to hide it from browsers.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(true)]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DispId((int)Ole32.DispatchID.TABSTOP)]
        [SRDescription(nameof(SR.ControlTabStopDescr))]
        public new bool TabStop
        {
            get => base.TabStop;
            set => base.TabStop = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        /// <summary>
        ///  For forms that are show in task bar false, this returns a HWND
        ///  they must be parented to in order for it to work.
        /// </summary>
        private HandleRef TaskbarOwner
        {
            get
            {
                if (ownerWindow is null)
                {
                    ownerWindow = new NativeWindow();
                }

                if (ownerWindow.Handle == IntPtr.Zero)
                {
                    CreateParams cp = new CreateParams
                    {
                        ExStyle = (int)User32.WS_EX.TOOLWINDOW
                    };
                    ownerWindow.CreateHandle(cp);
                }

                return new HandleRef(ownerWindow, ownerWindow.Handle);
            }
        }

        [SettingsBindable(true)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        /// <summary>
        ///  Gets or sets a value indicating whether to display the form as a top-level
        ///  window.
        /// </summary>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TopLevel
        {
            get
            {
                return GetTopLevel();
            }
            set
            {
                if (!value && ((Form)this).IsMdiContainer && !DesignMode)
                {
                    throw new ArgumentException(SR.MDIContainerMustBeTopLevel, nameof(value));
                }
                SetTopLevel(value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the form should be displayed as the
        ///  top-most form of the application.
        /// </summary>
        [DefaultValue(false)]
        [SRCategory(nameof(SR.CatWindowStyle))]
        [SRDescription(nameof(SR.FormTopMostDescr))]
        public bool TopMost
        {
            get => formState[FormStateTopMost] != 0;
            set
            {
                if (IsHandleCreated && TopLevel)
                {
                    User32.SetWindowPos(
                        new HandleRef(this, Handle),
                        value ? User32.HWND_TOPMOST : User32.HWND_NOTOPMOST,
                        flags: User32.SWP.NOMOVE | User32.SWP.NOSIZE);
                }

                formState[FormStateTopMost] = value ? 1 : 0;
            }
        }

        /// <summary>
        ///  Gets or sets the color that will represent transparent areas of the form.
        /// </summary>
        [SRCategory(nameof(SR.CatWindowStyle))]
        [SRDescription(nameof(SR.FormTransparencyKeyDescr))]
        public Color TransparencyKey
        {
            get
            {
                object key = Properties.GetObject(PropTransparencyKey);
                if (key != null)
                {
                    return (Color)key;
                }
                return Color.Empty;
            }
            set
            {
                Properties.SetObject(PropTransparencyKey, value);
                if (!IsMdiContainer)
                {
                    bool oldLayered = (formState[FormStateLayered] == 1);
                    if (value != Color.Empty)
                    {
                        AllowTransparency = true;
                        formState[FormStateLayered] = 1;
                    }
                    else
                    {
                        formState[FormStateLayered] = (OpacityAsByte < 255) ? 1 : 0;
                    }
                    if (oldLayered != (formState[FormStateLayered] != 0))
                    {
                        UpdateStyles();
                    }
                    UpdateLayered();
                }
            }
        }

        //
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetVisibleCore(bool value)
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Form::SetVisibleCore(" + value.ToString() + ") - " + Name);

            // If DialogResult.OK and the value == Visible then this code has been called either through
            // ShowDialog( ) or explicit Hide( ) by the user. So dont go through this function again.
            // This will avoid flashing during closing the dialog;
            if (value == Visible && dialogResult == DialogResult.OK)
            {
                return;
            }

            // (!value || calledMakeVisible) is to make sure that we fall
            // through and execute the code below atleast once.
            if (value == Visible && (!value || CalledMakeVisible))
            {
                base.SetVisibleCore(value);
                return;
            }

            if (value)
            {
                CalledMakeVisible = true;
                if (CalledCreateControl)
                {
                    if (CalledOnLoad)
                    {
                        // Make sure the form is in the Application.OpenForms collection
                        if (!Application.OpenForms.Contains(this))
                        {
                            Application.OpenForms.Add(this);
                        }
                    }
                    else
                    {
                        CalledOnLoad = true;
                        OnLoad(EventArgs.Empty);
                        if (dialogResult != DialogResult.None)
                        {
                            // Don't show the dialog if the dialog result was set
                            // in the OnLoad event.
                            //
                            value = false;
                        }
                    }
                }
            }

            if (!IsMdiChild)
            {
                base.SetVisibleCore(value);

                // We need to force this call if we were created
                // with a STARTUPINFO structure (e.g. launched from explorer), since
                // it won't send a WM_SHOWWINDOW the first time it's called.
                // when WM_SHOWWINDOW gets called, we'll flip this bit to true
                //
                if (0 == formState[FormStateSWCalled])
                {
                    User32.SendMessageW(this, User32.WM.SHOWWINDOW, PARAM.FromBool(value));
                }
            }
            else
            {
                // Throw away any existing handle.
                if (IsHandleCreated)
                {
                    // Everett/RTM used to wrap this in an assert for AWP.
                    DestroyHandle();
                }

                if (!value)
                {
                    SetState(States.Visible, false);
                }
                else
                {
                    // The ordering is important here... Force handle creation
                    // (getHandle) then show the window (ShowWindow) then finish
                    // creating children using createControl...
                    //
                    SetState(States.Visible, true);

                    // Ask the mdiClient to re-layout the controls so that any docking or
                    // anchor settings for this mdi child window will be honored.
                    MdiParentInternal.MdiClient.PerformLayout();

                    if (ParentInternal != null && ParentInternal.Visible)
                    {
                        SuspendLayout();
                        try
                        {
                            User32.ShowWindow(this, User32.SW.SHOW);
                            CreateControl();

                            // If this form is mdichild and maximized, we need to redraw the MdiParent non-client area to
                            // update the menu bar because we always create the window as if it were not maximized.
                            // See comment on CreateHandle about this.
                            if (WindowState == FormWindowState.Maximized)
                            {
                                MdiParentInternal.UpdateWindowIcon(true);
                            }
                        }
                        finally
                        {
                            ResumeLayout();
                        }
                    }
                }
                OnVisibleChanged(EventArgs.Empty);
            }

            //(

            if (value && !IsMdiChild && (WindowState == FormWindowState.Maximized || TopMost))
            {
                if (ActiveControl is null)
                {
                    SelectNextControl(null, true, true, true, false);
                }
                FocusActiveControlInternal();
            }
        }

        /// <summary>
        ///  Gets or sets the form's window state.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(FormWindowState.Normal)]
        [SRDescription(nameof(SR.FormWindowStateDescr))]
        public FormWindowState WindowState
        {
            get => (FormWindowState)formState[FormStateWindowState];
            set
            {
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FormWindowState.Normal, (int)FormWindowState.Maximized))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormWindowState));
                }

                switch (value)
                {
                    case FormWindowState.Normal:
                        SetState(States.SizeLockedByOS, false);
                        break;
                    case FormWindowState.Maximized:
                    case FormWindowState.Minimized:
                        SetState(States.SizeLockedByOS, true);
                        break;
                }

                if (IsHandleCreated && Visible)
                {
                    IntPtr hWnd = Handle;
                    switch (value)
                    {
                        case FormWindowState.Normal:
                            User32.ShowWindow(this, User32.SW.NORMAL);
                            break;
                        case FormWindowState.Maximized:
                            User32.ShowWindow(this, User32.SW.MAXIMIZE);
                            break;
                        case FormWindowState.Minimized:
                            User32.ShowWindow(this, User32.SW.MINIMIZE);
                            break;
                    }
                }

                // Now set the local property to the passed in value so that
                // when UpdateWindowState is by the ShowWindow call above, the window state in effect when
                // this call was made will still be effective while processing that method.
                formState[FormStateWindowState] = (int)value;
            }
        }

        /// <summary>
        ///  Gets or sets the text to display in the caption bar of the form.
        /// </summary>
        internal override string WindowText
        {
            get => base.WindowText;
            set
            {
                string oldText = WindowText;
                base.WindowText = value;

                // For non-default FormBorderStyles, we do not set the WS_CAPTION style if
                // the Text property is null or "".
                // When we reload the form from code view, the text property is not set till
                // the very end, and so we do not end up updating the CreateParams with
                // WS_CAPTION. Fixed this by making sure we call UpdateStyles() when
                // we transition from a non-null value to a null value or vice versa in
                // Form.WindowText.
                if (string.IsNullOrEmpty(oldText) || string.IsNullOrEmpty(value))
                {
                    UpdateFormStyles();
                }
            }
        }

        /// <summary>
        ///  Occurs when the form is activated in code or by the user.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.FormOnActivateDescr))]
        public event EventHandler Activated
        {
            add => Events.AddHandler(EVENT_ACTIVATED, value);
            remove => Events.RemoveHandler(EVENT_ACTIVATED, value);
        }

        /// <summary>
        ///  Occurs when the form is closing.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnClosingDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event CancelEventHandler Closing
        {
            add => Events.AddHandler(EVENT_CLOSING, value);
            remove => Events.RemoveHandler(EVENT_CLOSING, value);
        }

        /// <summary>
        ///  Occurs when the form is closed.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnClosedDescr))]
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler Closed
        {
            add => Events.AddHandler(EVENT_CLOSED, value);
            remove => Events.RemoveHandler(EVENT_CLOSED, value);
        }

        /// <summary>
        ///  Occurs when the form loses focus and is not the active form.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus))]
        [SRDescription(nameof(SR.FormOnDeactivateDescr))]
        public event EventHandler Deactivate
        {
            add => Events.AddHandler(EVENT_DEACTIVATE, value);
            remove => Events.RemoveHandler(EVENT_DEACTIVATE, value);
        }

        /// <summary>
        ///  Occurs when the form is closing.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnFormClosingDescr))]
        public event FormClosingEventHandler FormClosing
        {
            add => Events.AddHandler(EVENT_FORMCLOSING, value);
            remove => Events.RemoveHandler(EVENT_FORMCLOSING, value);
        }

        /// <summary>
        ///  Occurs when the form is closed.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnFormClosedDescr))]
        public event FormClosedEventHandler FormClosed
        {
            add => Events.AddHandler(EVENT_FORMCLOSED, value);
            remove => Events.RemoveHandler(EVENT_FORMCLOSED, value);
        }

        /// <summary>
        ///  Occurs before the form becomes visible.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnLoadDescr))]
        public event EventHandler Load
        {
            add => Events.AddHandler(EVENT_LOAD, value);
            remove => Events.RemoveHandler(EVENT_LOAD, value);
        }

        /// <summary>
        ///  Occurs when a Multiple Document Interface (MDI) child form is activated or closed
        ///  within an MDI application.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.FormOnMDIChildActivateDescr))]
        public event EventHandler MdiChildActivate
        {
            add => Events.AddHandler(EVENT_MDI_CHILD_ACTIVATE, value);
            remove => Events.RemoveHandler(EVENT_MDI_CHILD_ACTIVATE, value);
        }

        /// <summary>
        ///  Occurs when the menu of a form loses focus.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnMenuCompleteDescr))]
        [Browsable(false)]
        public event EventHandler MenuComplete
        {
            add => Events.AddHandler(EVENT_MENUCOMPLETE, value);
            remove => Events.RemoveHandler(EVENT_MENUCOMPLETE, value);
        }

        /// <summary>
        ///  Occurs when the menu of a form receives focus.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnMenuStartDescr))]
        [Browsable(false)]
        public event EventHandler MenuStart
        {
            add => Events.AddHandler(EVENT_MENUSTART, value);
            remove => Events.RemoveHandler(EVENT_MENUSTART, value);
        }

        /// <summary>
        ///  Occurs after the input language of the form has changed.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnInputLangChangeDescr))]
        public event InputLanguageChangedEventHandler InputLanguageChanged
        {
            add => Events.AddHandler(EVENT_INPUTLANGCHANGE, value);
            remove => Events.RemoveHandler(EVENT_INPUTLANGCHANGE, value);
        }

        /// <summary>
        ///  Occurs when the the user attempts to change the input language for the
        ///  form.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnInputLangChangeRequestDescr))]
        public event InputLanguageChangingEventHandler InputLanguageChanging
        {
            add => Events.AddHandler(EVENT_INPUTLANGCHANGEREQUEST, value);
            remove => Events.RemoveHandler(EVENT_INPUTLANGCHANGEREQUEST, value);
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => Events.AddHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            remove => Events.RemoveHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
        }

        /// <summary>
        ///  Occurs whenever the form is first shown.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.FormOnShownDescr))]
        public event EventHandler Shown
        {
            add => Events.AddHandler(EVENT_SHOWN, value);
            remove => Events.RemoveHandler(EVENT_SHOWN, value);
        }

        /// <summary>
        ///  Activates the form and gives it focus.
        /// </summary>
        public void Activate()
        {
            if (Visible && IsHandleCreated)
            {
                if (IsMdiChild)
                {
                    User32.SendMessageW(MdiParentInternal.MdiClient, User32.WM.MDIACTIVATE, Handle, IntPtr.Zero);
                }
                else
                {
                    User32.SetForegroundWindow(this);
                }
            }
        }

        protected void ActivateMdiChild(Form form)
        {
            if (FormerlyActiveMdiChild != null && !FormerlyActiveMdiChild.IsClosing)
            {
                FormerlyActiveMdiChild.UpdateWindowIcon(true);
                FormerlyActiveMdiChild = null;
            }

            Form activeMdiChild = ActiveMdiChildInternal;
            if (activeMdiChild == form)
            {
                return;
            }

            // Don't believe we ever hit this with non-null, but leaving it intact in
            // case removing it would cause a problem.
            if (activeMdiChild != null)
            {
                activeMdiChild.Active = false;
            }

            activeMdiChild = form;
            ActiveMdiChildInternal = form;

            if (activeMdiChild != null)
            {
                activeMdiChild.IsMdiChildFocusable = true;
                activeMdiChild.Active = true;
            }
            else if (Active)
            {
                ActivateControl(this);
            }

            // Note: It is possible that we are raising this event when the activeMdiChild is null,
            // this is the case when the only visible mdi child is being closed.  See DeactivateMdiChild
            // for more info.
            OnMdiChildActivate(EventArgs.Empty);
        }

        /// <summary>
        ///  Adds
        ///  an owned form to this form.
        /// </summary>
        public void AddOwnedForm(Form ownedForm)
        {
            if (ownedForm is null)
            {
                return;
            }

            if (ownedForm.OwnerInternal != this)
            {
                ownedForm.Owner = this; // NOTE: this calls AddOwnedForm again with correct owner set.
                return;
            }

            Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
            int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

            // Make sure this isn't already in the list:
            for (int i = 0; i < ownedFormsCount; i++)
            {
                if (ownedForms[i] == ownedForm)
                {
                    return;
                }
            }

            if (ownedForms is null)
            {
                ownedForms = new Form[4];
                Properties.SetObject(PropOwnedForms, ownedForms);
            }
            else if (ownedForms.Length == ownedFormsCount)
            {
                Form[] newOwnedForms = new Form[ownedFormsCount * 2];
                Array.Copy(ownedForms, 0, newOwnedForms, 0, ownedFormsCount);
                ownedForms = newOwnedForms;
                Properties.SetObject(PropOwnedForms, ownedForms);
            }

            ownedForms[ownedFormsCount] = ownedForm;
            Properties.SetInteger(PropOwnedFormsCount, ownedFormsCount + 1);
        }

        // When shrinking the form (i.e. going from Large Fonts to Small
        // Fonts) we end up making everything too small due to roundoff,
        // etc... solution - just don't shrink as much.
        //
        private float AdjustScale(float scale)
        {
            // NOTE : This function is cloned in FormDocumentDesigner... remember to keep
            //      : them in sync
            //

            // Map 0.0 - .92... increment by 0.08
            //
            if (scale < .92f)
            {
                return scale + 0.08f;
            }

            // Map .92 - .99 to 1.0
            //
            else if (scale < 1.0f)
            {
                return 1.0f;
            }

            // Map 1.02... increment by 0.08
            //
            else if (scale > 1.01f)
            {
                return scale + 0.08f;
            }

            // Map 1.0 - 1.01... no change
            //
            else
            {
                return scale;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void AdjustFormScrollbars(bool displayScrollbars)
        {
            if (WindowState != FormWindowState.Minimized)
            {
                base.AdjustFormScrollbars(displayScrollbars);
            }
        }

        private void AdjustSystemMenu(IntPtr hmenu)
        {
            UpdateWindowState();
            FormWindowState winState = WindowState;
            FormBorderStyle borderStyle = FormBorderStyle;
            bool sizableBorder = (borderStyle == FormBorderStyle.SizableToolWindow
                                  || borderStyle == FormBorderStyle.Sizable);

            bool showMin = MinimizeBox && winState != FormWindowState.Minimized;
            bool showMax = MaximizeBox && winState != FormWindowState.Maximized;
            bool showClose = ControlBox;
            bool showRestore = winState != FormWindowState.Normal;
            bool showSize = sizableBorder && winState != FormWindowState.Minimized
                            && winState != FormWindowState.Maximized;

            if (!showMin)
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.MINIMIZE, User32.MF.BYCOMMAND | User32.MF.GRAYED);
            }
            else
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.MINIMIZE, User32.MF.BYCOMMAND | User32.MF.ENABLED);
            }
            if (!showMax)
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.MAXIMIZE, User32.MF.BYCOMMAND | User32.MF.GRAYED);
            }
            else
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.MAXIMIZE, User32.MF.BYCOMMAND | User32.MF.ENABLED);
            }
            if (!showClose)
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.CLOSE, User32.MF.BYCOMMAND | User32.MF.GRAYED);
            }
            else
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.CLOSE, User32.MF.BYCOMMAND | User32.MF.ENABLED);
            }
            if (!showRestore)
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.RESTORE, User32.MF.BYCOMMAND | User32.MF.GRAYED);
            }
            else
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.RESTORE, User32.MF.BYCOMMAND | User32.MF.ENABLED);
            }
            if (!showSize)
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.SIZE, User32.MF.BYCOMMAND | User32.MF.GRAYED);
            }
            else
            {
                User32.EnableMenuItem(new HandleRef(this, hmenu), User32.SC.SIZE, User32.MF.BYCOMMAND | User32.MF.ENABLED);
            }
        }

        /// <summary>
        ///  This forces the SystemMenu to look like we want.
        /// </summary>
        private void AdjustSystemMenu()
        {
            if (IsHandleCreated)
            {
                IntPtr hmenu = User32.GetSystemMenu(new HandleRef(this, Handle), bRevert: BOOL.FALSE);
                AdjustSystemMenu(hmenu);
            }
        }

        /// <summary>
        ///  This auto scales the form based on the AutoScaleBaseSize.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method has been deprecated. Use the ApplyAutoScaling method instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected void ApplyAutoScaling()
        {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "ApplyAutoScaling... ");
            Debug.Indent();
            // NOTE : This function is cloned in FormDocumentDesigner... remember to keep
            //      : them in sync
            //

            // We also don't do this if the property is empty.  Otherwise we will perform
            // two GetAutoScaleBaseSize calls only to find that they returned the same
            // value.
            //
            if (!autoScaleBaseSize.IsEmpty)
            {
                Size baseVar = AutoScaleBaseSize;
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "base  =" + baseVar);
                SizeF newVarF = GetAutoScaleSize(Font);
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "new(f)=" + newVarF);
                Size newVar = new Size((int)Math.Round(newVarF.Width), (int)Math.Round(newVarF.Height));
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "new(i)=" + newVar);

                // We save a significant amount of time by bailing early if there's no work to be done
                if (baseVar.Equals(newVar))
                {
                    return;
                }

                float percY = AdjustScale(((float)newVar.Height) / ((float)baseVar.Height));
                float percX = AdjustScale(((float)newVar.Width) / ((float)baseVar.Width));
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "scale=" + percX + ", " + percY);
                Scale(percX, percY);
                // This would ensure that we use the new
                // font information to calculate the AutoScaleBaseSize. According to Triage
                // this was decided to Fix in this version.
                //
                AutoScaleBaseSize = newVar;
            }
            Debug.Unindent();
        }

        /// <summary>
        ///  This adjusts the size of the windowRect so that the client rect is the
        ///  correct size.
        /// </summary>
        private void ApplyClientSize()
        {
            if ((FormWindowState)formState[FormStateWindowState] != FormWindowState.Normal
                || !IsHandleCreated)
            {
                return;
            }

            // Cache the clientSize, since calling setBounds will end up causing
            // clientSize to get reset to the actual clientRect size...
            //
            Size correctClientSize = ClientSize;
            bool hscr = HScroll;
            bool vscr = VScroll;

            // This logic assumes that the caller of setClientSize() knows if the scrollbars
            // are showing or not. Since the 90% case is that setClientSize() is the persisted
            // ClientSize, this is correct.
            // Without this logic persisted forms that were saved with the scrollbars showing,
            // don't get set to the correct size.
            //
            bool adjustScroll = false;
            if (formState[FormStateSetClientSize] != 0)
            {
                adjustScroll = true;
                formState[FormStateSetClientSize] = 0;
            }
            if (adjustScroll)
            {
                if (hscr)
                {
                    correctClientSize.Height += SystemInformation.HorizontalScrollBarHeight;
                }
                if (vscr)
                {
                    correctClientSize.Width += SystemInformation.VerticalScrollBarWidth;
                }
            }

            IntPtr h = Handle;
            RECT rc = new RECT();
            User32.GetClientRect(new HandleRef(this, h), ref rc);
            Rectangle currentClient = Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);

            Rectangle bounds = Bounds;

            // If the width is incorrect, compute the correct size with
            // computeWindowSize. We only do this logic if the width needs to
            // be adjusted to avoid double adjusting the window.
            //
            if (correctClientSize.Width != currentClient.Width)
            {
                Size correct = ComputeWindowSize(correctClientSize);

                // Since computeWindowSize ignores scrollbars, we must tack these on to
                // assure the correct size.
                //
                if (vscr)
                {
                    correct.Width += SystemInformation.VerticalScrollBarWidth;
                }
                if (hscr)
                {
                    correct.Height += SystemInformation.HorizontalScrollBarHeight;
                }
                bounds.Width = correct.Width;
                bounds.Height = correct.Height;
                Bounds = bounds;
                User32.GetClientRect(new HandleRef(this, h), ref rc);
                currentClient = Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }

            // If it still isn't correct, then we assume that the problem is
            // menu wrapping (since computeWindowSize doesn't take that into
            // account), so we just need to adjust the height by the correct
            // amount.
            //
            if (correctClientSize.Height != currentClient.Height)
            {
                int delta = correctClientSize.Height - currentClient.Height;
                bounds.Height += delta;
                Bounds = bounds;
            }

            UpdateBounds();
        }

        /// <summary>
        ///  Assigns a new parent control. Sends out the appropriate property change
        ///  notifications for properties that are affected by the change of parent.
        /// </summary>
        internal override void AssignParent(Control value)
        {
            // If we are being unparented from the MDI client control, remove
            // formMDIParent as well.
            //
            Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
            if (formMdiParent != null && formMdiParent.MdiClient != value)
            {
                Properties.SetObject(PropFormMdiParent, null);
            }

            base.AssignParent(value);
        }

        /// <summary>
        ///  Checks whether a modal dialog is ready to close. If the dialogResult
        ///  property is not DialogResult.None, the OnClosing and OnClosed events
        ///  are fired. A return value of true indicates that both events were
        ///  successfully dispatched and that event handlers did not cancel closing
        ///  of the dialog. User code should never have a reason to call this method.
        ///  It is internal only so that the Application class can call it from a
        ///  modal message loop.
        ///
        ///  closingOnly is set when this is called from WmClose so that we can do just the beginning portion
        ///  of this and then let the message loop finish it off with the actual close code.
        ///
        ///  Note we set a flag to determine if we've already called close or not.
        /// </summary>
        internal bool CheckCloseDialog(bool closingOnly)
        {
            if (dialogResult == DialogResult.None && Visible)
            {
                return false;
            }
            try
            {
                FormClosingEventArgs e = new FormClosingEventArgs(closeReason, false);

                if (!CalledClosing)
                {
                    OnClosing(e);
                    OnFormClosing(e);
                    if (e.Cancel)
                    {
                        dialogResult = DialogResult.None;
                    }
                    else
                    {
                        // we have called closing here, and it wasn't cancelled, so we're expecting a close
                        // call again soon.
                        CalledClosing = true;
                    }
                }

                if (!closingOnly && dialogResult != DialogResult.None)
                {
                    FormClosedEventArgs fc = new FormClosedEventArgs(closeReason);
                    OnClosed(fc);
                    OnFormClosed(fc);

                    // reset called closing.
                    //
                    CalledClosing = false;
                }
            }
            catch (Exception e)
            {
                dialogResult = DialogResult.None;
                if (NativeWindow.WndProcShouldBeDebuggable)
                {
                    throw;
                }
                else
                {
                    Application.OnThreadException(e);
                }
            }
            return dialogResult != DialogResult.None || !Visible;
        }

        /// <summary>
        ///  Closes the form.
        /// </summary>
        public void Close()
        {
            if (GetState(States.CreatingHandle))
            {
                throw new InvalidOperationException(string.Format(SR.ClosingWhileCreatingHandle, "Close"));
            }

            if (IsHandleCreated)
            {
                closeReason = CloseReason.UserClosing;
                User32.SendMessageW(this, User32.WM.CLOSE);
            }
            else
            {
                // MSDN: When a form is closed, all resources created within the object are closed and the form is disposed.
                // For MDI child: MdiChildren collection gets updated
                Dispose();
            }
        }

        /// <summary>
        ///  Computes the window size from the clientSize based on the styles
        ///  returned from CreateParams.
        /// </summary>
        private Size ComputeWindowSize(Size clientSize)
        {
            CreateParams cp = CreateParams;
            return ComputeWindowSize(clientSize, cp.Style, cp.ExStyle);
        }

        /// <summary>
        ///  Computes the window size from the clientSize base on the specified
        ///  window styles. This will not return the correct size if menus wrap.
        /// </summary>
        private Size ComputeWindowSize(Size clientSize, int style, int exStyle)
        {
            RECT result = new RECT(0, 0, clientSize.Width, clientSize.Height);
            AdjustWindowRectEx(ref result, style, false, exStyle);
            return new Size(result.right - result.left, result.bottom - result.top);
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new FormAccessibleObject(this);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        /// <summary>
        ///  Cleans up form state after a control has been removed.
        ///  Package scope for Control
        /// </summary>
        internal override void AfterControlRemoved(Control control, Control oldParent)
        {
            base.AfterControlRemoved(control, oldParent);

            if (control == AcceptButton)
            {
                AcceptButton = null;
            }
            if (control == CancelButton)
            {
                CancelButton = null;
            }
            if (control == ctlClient)
            {
                ctlClient = null;
                UpdateMenuHandles();
            }
        }

        /// <summary>
        ///  Creates the handle for the Form. If a
        ///  subclass overrides this function,
        ///  it must call the base implementation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void CreateHandle()
        {
            // In the windows MDI code we have to suspend menu
            // updates on the parent while creating the handle. Otherwise if the
            // child is created maximized, the menu ends up with two sets of
            // MDI child ornaments.
            Form form = (Form)Properties.GetObject(PropFormMdiParent);
            if (form != null)
            {
                form.SuspendUpdateMenuHandles();
            }

            try
            {
                // If the child is created before the MDI parent we can
                // get Win32 exceptions as the MDI child is parked to the parking window.
                if (IsMdiChild && MdiParentInternal.IsHandleCreated)
                {
                    MdiClient mdiClient = MdiParentInternal.MdiClient;
                    if (mdiClient != null && !mdiClient.IsHandleCreated)
                    {
                        mdiClient.CreateControl();
                    }
                }

                // If we think that we are maximized, then a duplicate set of mdi gadgets are created.
                // If we create the window maximized, BUT our internal form state thinks it
                // isn't... then everything is OK...
                //
                // We really should find out what causes this... but I can't find it...
                //
                if (IsMdiChild
                    && (FormWindowState)formState[FormStateWindowState] == FormWindowState.Maximized)
                {
                    // This is the reason why we see the blue borders
                    // when creating a maximized mdi child, unfortunately we cannot fix this now...
                    formState[FormStateWindowState] = (int)FormWindowState.Normal;
                    formState[FormStateMdiChildMax] = 1;
                    base.CreateHandle();
                    formState[FormStateWindowState] = (int)FormWindowState.Maximized;
                    formState[FormStateMdiChildMax] = 0;
                }
                else
                {
                    base.CreateHandle();
                }

                UpdateHandleWithOwner();
                UpdateWindowIcon(false);

                AdjustSystemMenu();

                if ((FormStartPosition)formState[FormStateStartPos] != FormStartPosition.WindowsDefaultBounds)
                {
                    ApplyClientSize();
                }
                if (formState[FormStateShowWindowOnCreate] == 1)
                {
                    Visible = true;
                }

                // avoid extra SetMenu calls for perf
                if (!TopLevel || IsMdiContainer)
                {
                    UpdateMenuHandles();
                }

                // In order for a window not to have a taskbar entry, it must
                // be owned.
                //
                if (!ShowInTaskbar && OwnerInternal is null && TopLevel)
                {
                    User32.SetWindowLong(this, User32.GWL.HWNDPARENT, TaskbarOwner);

                    // Make sure the large icon is set so the ALT+TAB icon
                    // reflects the real icon of the application
                    Icon icon = Icon;
                    if (icon != null && TaskbarOwner.Handle != IntPtr.Zero)
                    {
                        User32.SendMessageW(TaskbarOwner, User32.WM.SETICON, (IntPtr)User32.ICON.BIG, icon.Handle);
                    }
                }

                if (formState[FormStateTopMost] != 0)
                {
                    TopMost = true;
                }
            }
            finally
            {
                if (form != null)
                {
                    form.ResumeUpdateMenuHandles();
                }

                // We need to reset the styles in case Windows tries to set us up
                // with "correct" styles
                //
                UpdateStyles();
            }
        }

        // Deactivates active MDI child and temporarily marks it as unfocusable,
        // so that WM_SETFOCUS sent to MDIClient does not activate that child.
        private void DeactivateMdiChild()
        {
            Form activeMdiChild = ActiveMdiChildInternal;
            if (null != activeMdiChild)
            {
                Form mdiParent = activeMdiChild.MdiParentInternal;

                activeMdiChild.Active = false;
                activeMdiChild.IsMdiChildFocusable = false;
                if (!activeMdiChild.IsClosing)
                {
                    FormerlyActiveMdiChild = activeMdiChild;
                }
                // Enter/Leave events on child controls are raised from the ActivateMdiChild method, usually when another
                // Mdi child is getting activated after deactivating this one; but if this is the only visible MDI child
                // we need to fake the activation call so MdiChildActivate and Leave events are raised properly. (We say
                // in the MSDN doc that the MdiChildActivate event is raised when an mdi child is activated or closed -
                // we actually meant the last mdi child is closed).
                bool fakeActivation = true;
                foreach (Form mdiChild in mdiParent.MdiChildren)
                {
                    if (mdiChild != this && mdiChild.Visible)
                    {
                        fakeActivation = false; // more than one mdi child visible.
                        break;
                    }
                }

                if (fakeActivation)
                {
                    mdiParent.ActivateMdiChild(null);
                }

                ActiveMdiChildInternal = null;

                // Note: WM_MDIACTIVATE message is sent to the form being activated and to the form being deactivated, ideally
                // we would raise the MdiChildActivate event here accordingly but it would constitute a breaking change.

                // undo merge
                UpdateMenuHandles();
                UpdateToolStrip();
            }
        }

        /// <summary>
        ///  Calls the default window proc for the form. If
        ///  a subclass overrides this function,
        ///  it must call the base implementation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void DefWndProc(ref Message m)
        {
            if (ctlClient != null && ctlClient.IsHandleCreated && ctlClient.ParentInternal == this)
            {
                m.Result = User32.DefFrameProcW(m.HWnd, ctlClient.Handle, (User32.WM)m.Msg, m.WParam, m.LParam);
            }
            else if (0 != formStateEx[FormStateExUseMdiChildProc])
            {
                m.Result = User32.DefMDIChildProcW(m.HWnd, (User32.WM)m.Msg, m.WParam, m.LParam);
            }
            else
            {
                base.DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Releases all the system resources associated with the Form. If a subclass
        ///  overrides this function, it must call the base implementation.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CalledOnLoad = false;
                CalledMakeVisible = false;
                CalledCreateControl = false;

                if (Properties.ContainsObject(PropAcceptButton))
                {
                    Properties.SetObject(PropAcceptButton, null);
                }

                if (Properties.ContainsObject(PropCancelButton))
                {
                    Properties.SetObject(PropCancelButton, null);
                }

                if (Properties.ContainsObject(PropDefaultButton))
                {
                    Properties.SetObject(PropDefaultButton, null);
                }

                if (Properties.ContainsObject(PropActiveMdiChild))
                {
                    Properties.SetObject(PropActiveMdiChild, null);
                }

                if (MdiWindowListStrip != null)
                {
                    MdiWindowListStrip.Dispose();
                    MdiWindowListStrip = null;
                }

                if (MdiControlStrip != null)
                {
                    MdiControlStrip.Dispose();
                    MdiControlStrip = null;
                }

                if (MainMenuStrip != null)
                {
                    // should NOT call dispose on MainMenuStrip - it's likely NOT to be in the form's control collection.
                    MainMenuStrip = null;
                }

                Form owner = (Form)Properties.GetObject(PropOwner);
                if (owner != null)
                {
                    owner.RemoveOwnedForm(this);
                    Properties.SetObject(PropOwner, null);
                }

                Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

                for (int i = ownedFormsCount - 1; i >= 0; i--)
                {
                    if (ownedForms[i] != null)
                    {
                        // it calls remove and removes itself.
                        ownedForms[i].Dispose();
                    }
                }

                if (smallIcon != null)
                {
                    smallIcon.Dispose();
                    smallIcon = null;
                }

                base.Dispose(disposing);
                ctlClient = null;
            }
            else
            {
                base.Dispose(disposing);
            }
        }

        /// <summary>
        ///  Adjusts the window style of the CreateParams to reflect the bordericons.
        /// </summary>
        private void FillInCreateParamsBorderIcons(CreateParams cp)
        {
            if (FormBorderStyle != FormBorderStyle.None)
            {
                if (!string.IsNullOrEmpty(Text))
                {
                    cp.Style |= (int)User32.WS.CAPTION;
                }

                if (ControlBox)
                {
                    cp.Style |= (int)(User32.WS.SYSMENU | User32.WS.CAPTION);
                }
                else
                {
                    cp.Style &= ~(int)User32.WS.SYSMENU;
                }

                if (MaximizeBox)
                {
                    cp.Style |= (int)User32.WS.MAXIMIZEBOX;
                }
                else
                {
                    cp.Style &= ~(int)User32.WS.MAXIMIZEBOX;
                }

                if (MinimizeBox)
                {
                    cp.Style |= (int)User32.WS.MINIMIZEBOX;
                }
                else
                {
                    cp.Style &= ~(int)User32.WS.MINIMIZEBOX;
                }

                if (HelpButton && !MaximizeBox && !MinimizeBox && ControlBox)
                {
                    // Windows should ignore WS_EX_CONTEXTHELP unless all those conditions hold.
                    // But someone must have failed the check, because Windows 2000
                    // will show a help button if either the maximize or
                    // minimize button is disabled.
                    cp.ExStyle |= (int)User32.WS_EX.CONTEXTHELP;
                }
                else
                {
                    cp.ExStyle &= ~(int)User32.WS_EX.CONTEXTHELP;
                }
            }
        }

        /// <summary>
        ///  Adjusts the window style of the CreateParams to reflect the borderstyle.
        /// </summary>
        private void FillInCreateParamsBorderStyles(CreateParams cp)
        {
            switch ((FormBorderStyle)formState[FormStateBorderStyle])
            {
                case FormBorderStyle.None:
                    break;
                case FormBorderStyle.FixedSingle:
                    cp.Style |= (int)User32.WS.BORDER;
                    break;
                case FormBorderStyle.Sizable:
                    cp.Style |= (int)(User32.WS.BORDER | User32.WS.THICKFRAME);
                    break;
                case FormBorderStyle.Fixed3D:
                    cp.Style |= (int)User32.WS.BORDER;
                    cp.ExStyle |= (int)User32.WS_EX.CLIENTEDGE;
                    break;
                case FormBorderStyle.FixedDialog:
                    cp.Style |= (int)User32.WS.BORDER;
                    cp.ExStyle |= (int)User32.WS_EX.DLGMODALFRAME;
                    break;
                case FormBorderStyle.FixedToolWindow:
                    cp.Style |= (int)User32.WS.BORDER;
                    cp.ExStyle |= (int)User32.WS_EX.TOOLWINDOW;
                    break;
                case FormBorderStyle.SizableToolWindow:
                    cp.Style |= (int)(User32.WS.BORDER | User32.WS.THICKFRAME);
                    cp.ExStyle |= (int)User32.WS_EX.TOOLWINDOW;
                    break;
            }
        }

        /// <summary>
        ///  Adjusts the CreateParams to reflect the window bounds and start position.
        /// </summary>
        private void FillInCreateParamsStartPosition(CreateParams cp)
        {
            if (formState[FormStateSetClientSize] != 0)
            {
                // When computing the client window size, don't tell them that
                // we are going to be maximized!
                int maskedStyle = cp.Style & ~(int)(User32.WS.MAXIMIZE | User32.WS.MINIMIZE);
                Size correct = ComputeWindowSize(ClientSize, maskedStyle, cp.ExStyle);
                cp.Width = correct.Width;
                cp.Height = correct.Height;
            }

            switch ((FormStartPosition)formState[FormStateStartPos])
            {
                case FormStartPosition.WindowsDefaultBounds:
                    cp.Width = NativeMethods.CW_USEDEFAULT;
                    cp.Height = NativeMethods.CW_USEDEFAULT;
                    // no break, fall through to set the location to default...
                    goto case FormStartPosition.WindowsDefaultLocation;
                case FormStartPosition.WindowsDefaultLocation:
                case FormStartPosition.CenterParent:
                    // Since FillInCreateParamsStartPosition() is called
                    // several times when a window is shown, we'll need to force the location
                    // each time for MdiChild windows that are docked so that the window will
                    // be created in the correct location and scroll bars will not be displayed.
                    if (IsMdiChild && DockStyle.None != Dock)
                    {
                        break;
                    }

                    cp.X = NativeMethods.CW_USEDEFAULT;
                    cp.Y = NativeMethods.CW_USEDEFAULT;
                    break;
                case FormStartPosition.CenterScreen:
                    if (IsMdiChild)
                    {
                        Control mdiclient = MdiParentInternal.MdiClient;
                        Rectangle clientRect = mdiclient.ClientRectangle;

                        cp.X = Math.Max(clientRect.X, clientRect.X + (clientRect.Width - cp.Width) / 2);
                        cp.Y = Math.Max(clientRect.Y, clientRect.Y + (clientRect.Height - cp.Height) / 2);
                    }
                    else
                    {
                        Screen desktop = null;
                        IWin32Window dialogOwner = (IWin32Window)Properties.GetObject(PropDialogOwner);
                        if ((OwnerInternal != null) || (dialogOwner != null))
                        {
                            IntPtr ownerHandle = (dialogOwner != null) ? Control.GetSafeHandle(dialogOwner) : OwnerInternal.Handle;
                            desktop = Screen.FromHandle(ownerHandle);
                        }
                        else
                        {
                            desktop = Screen.FromPoint(Control.MousePosition);
                        }

                        Rectangle screenRect = desktop.WorkingArea;
                        // if, we're maximized, then don't set the x & y coordinates (they're @ (0,0) )
                        if (WindowState != FormWindowState.Maximized)
                        {
                            cp.X = Math.Max(screenRect.X, screenRect.X + (screenRect.Width - cp.Width) / 2);
                            cp.Y = Math.Max(screenRect.Y, screenRect.Y + (screenRect.Height - cp.Height) / 2);
                        }
                    }

                    break;
            }
        }

        /// <summary>
        ///  Adjusts the Createparams to reflect the window state.
        /// </summary>
        private void FillInCreateParamsWindowState(CreateParams cp)
        {
            switch ((FormWindowState)formState[FormStateWindowState])
            {
                case FormWindowState.Maximized:
                    cp.Style |= (int)User32.WS.MAXIMIZE;
                    break;
                case FormWindowState.Minimized:
                    cp.Style |= (int)User32.WS.MINIMIZE;
                    break;
            }
        }

        /// <summary>
        ///  Attempts to set focus to this Form.
        /// </summary>
        private protected override bool FocusInternal()
        {
            Debug.Assert(IsHandleCreated, "Attempt to set focus to a form that has not yet created its handle.");

            // If this form is a MdiChild, then we need to set the focus differently.
            if (IsMdiChild)
            {
                User32.SendMessageW(MdiParentInternal.MdiClient, User32.WM.MDIACTIVATE, Handle, IntPtr.Zero);
                return Focused;
            }

            return base.FocusInternal();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method has been deprecated. Use the AutoScaleDimensions property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public static SizeF GetAutoScaleSize(Font font)
        {
            float height = font.Height;
            float width = 9.0f;

            try
            {
                using (Graphics graphics = Graphics.FromHwndInternal(IntPtr.Zero /*screen*/))
                {
                    string magicString = "The quick brown fox jumped over the lazy dog.";
                    double magicNumber = 44.549996948242189; // chosen for compatibility with older versions of windows forms, but approximately magicString.Length
                    float stringWidth = graphics.MeasureString(magicString, font).Width;
                    width = (float)(stringWidth / magicNumber);
                }
            }
            catch
            { // We may get an bogus OutOfMemoryException
              // (which is a critical exception - according to ClientUtils.IsCriticalException())
              // from GDI+. So we can't use ClientUtils.IsCriticalException here and rethrow.
            }

            return new SizeF(width, height);
        }

        private void CallShownEvent()
        {
            OnShown(EventArgs.Empty);
        }

        /// <summary>
        ///  Override since CanProcessMnemonic is overriden too (base.CanSelectCore calls CanProcessMnemonic).
        /// </summary>
        internal override bool CanSelectCore()
        {
            if (!GetStyle(ControlStyles.Selectable) || !Enabled || !Visible)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  When an MDI form is hidden it means its handle has not yet been created or has been destroyed (see
        ///  SetVisibleCore).  If the handle is recreated, the form will be made visible which should be avoided.
        /// </summary>
        internal bool CanRecreateHandle()
        {
            if (IsMdiChild)
            {
                // During changing visibility, it is possible that the style returns true for visible but the handle has
                // not yet been created, add a check for both.
                return GetState(States.Visible) && IsHandleCreated;
            }
            return true;
        }

        /// <summary>
        ///  Overriden to handle MDI mnemonic processing properly.
        /// </summary>
        internal override bool CanProcessMnemonic()
        {
#if DEBUG
            TraceCanProcessMnemonic();
#endif
            // If this is a Mdi child form, child controls should process mnemonics only if this is the active mdi child.
            if (IsMdiChild && (formStateEx[FormStateExMnemonicProcessed] == 1 || this != MdiParentInternal.ActiveMdiChildInternal || WindowState == FormWindowState.Minimized))
            {
                return false;
            }

            return base.CanProcessMnemonic();
        }

        /// <summary>
        ///  Overriden to handle MDI mnemonic processing properly.
        /// </summary>
        protected internal override bool ProcessMnemonic(char charCode)
        {
            // MDI container form has at least one control, the MDI client which contains the MDI children. We need
            // to allow the MDI children process the mnemonic before any other control in the MDI container (like
            // menu items or any other control).
            if (base.ProcessMnemonic(charCode))
            {
                return true;
            }

            if (IsMdiContainer)
            {
                // ContainerControl have already processed the active MDI child for us (if any) in the call above,
                // now process remaining controls (non-mdi children).
                if (Controls.Count > 1)
                { // Ignore the MdiClient control
                    for (int index = 0; index < Controls.Count; index++)
                    {
                        Control ctl = Controls[index];
                        if (ctl is MdiClient)
                        {
                            continue;
                        }

                        if (ctl.ProcessMnemonic(charCode))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        ///  Centers the dialog to its parent.
        /// </summary>
        protected void CenterToParent()
        {
            if (TopLevel)
            {
                Point p = new Point();
                Size s = Size;
                IntPtr ownerHandle = IntPtr.Zero;

                ownerHandle = User32.GetWindowLong(this, User32.GWL.HWNDPARENT);
                if (ownerHandle != IntPtr.Zero)
                {
                    Screen desktop = Screen.FromHandle(ownerHandle);
                    Rectangle screenRect = desktop.WorkingArea;
                    var ownerRect = new RECT();
                    User32.GetWindowRect(ownerHandle, ref ownerRect);

                    p.X = (ownerRect.left + ownerRect.right - s.Width) / 2;
                    if (p.X < screenRect.X)
                    {
                        p.X = screenRect.X;
                    }
                    else if (p.X + s.Width > screenRect.X + screenRect.Width)
                    {
                        p.X = screenRect.X + screenRect.Width - s.Width;
                    }

                    p.Y = (ownerRect.top + ownerRect.bottom - s.Height) / 2;
                    if (p.Y < screenRect.Y)
                    {
                        p.Y = screenRect.Y;
                    }
                    else if (p.Y + s.Height > screenRect.Y + screenRect.Height)
                    {
                        p.Y = screenRect.Y + screenRect.Height - s.Height;
                    }

                    Location = p;
                }
                else
                {
                    CenterToScreen();
                }
            }
        }

        /// <summary>
        ///  Centers the dialog to the screen. This will first attempt to use
        ///  the owner property to determine the correct screen, then
        ///  it will try the HWND owner of the form, and finally this will
        ///  center the form on the same monitor as the mouse cursor.
        /// </summary>
        protected void CenterToScreen()
        {
            Point p = new Point();
            Screen desktop = null;
            if (OwnerInternal != null)
            {
                desktop = Screen.FromControl(OwnerInternal);
            }
            else
            {
                IntPtr hWndOwner = IntPtr.Zero;
                if (TopLevel)
                {
                    hWndOwner = User32.GetWindowLong(this, User32.GWL.HWNDPARENT);
                }
                if (hWndOwner != IntPtr.Zero)
                {
                    desktop = Screen.FromHandle(hWndOwner);
                }
                else
                {
                    desktop = Screen.FromPoint(Control.MousePosition);
                }
            }
            Rectangle screenRect = desktop.WorkingArea;
            p.X = Math.Max(screenRect.X, screenRect.X + (screenRect.Width - Width) / 2);
            p.Y = Math.Max(screenRect.Y, screenRect.Y + (screenRect.Height - Height) / 2);
            Location = p;
        }

        /// <summary>
        ///  Arranges the Multiple Document Interface
        ///  (MDI) child forms according to value.
        /// </summary>
        public void LayoutMdi(MdiLayout value)
        {
            ctlClient?.LayoutMdi(value);
        }

        /// <summary>
        ///  The activate event is fired when the form is activated.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnActivated(EventArgs e)
        {
            ((EventHandler)Events[EVENT_ACTIVATED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Override of AutoScaleModeChange method from ContainerControl. We use this to keep our
        ///  own AutoScale property in sync.
        /// </summary>
        private protected override void OnAutoScaleModeChanged()
        {
            base.OnAutoScaleModeChanged();
            if (formStateEx[FormStateExSettingAutoScale] != 1)
            {
                // Obsolete code required here for backwards compat
                AutoScale = false;
            }
        }

        protected override void OnBackgroundImageChanged(EventArgs e)
        {
            base.OnBackgroundImageChanged(e);
            if (IsMdiContainer)
            {
                MdiClient.BackgroundImage = BackgroundImage;
                // requires explicit invalidate to update the image.
                MdiClient.Invalidate();
            }
        }

        protected override void OnBackgroundImageLayoutChanged(EventArgs e)
        {
            base.OnBackgroundImageLayoutChanged(e);
            if (IsMdiContainer)
            {
                MdiClient.BackgroundImageLayout = BackgroundImageLayout;
                // requires explicit invalidate to update the imageLayout.
                MdiClient.Invalidate();
            }
        }

        /// <summary>
        ///  The Closing event is fired when the form is closed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClosing(CancelEventArgs e)
        {
            ((CancelEventHandler)Events[EVENT_CLOSING])?.Invoke(this, e);
        }

        /// <summary>
        ///  The Closed event is fired when the form is closed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClosed(EventArgs e)
        {
            ((EventHandler)Events[EVENT_CLOSED])?.Invoke(this, e);
        }

        /// <summary>
        ///  The Closing event is fired before the form is closed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFormClosing(FormClosingEventArgs e)
        {
            ((FormClosingEventHandler)Events[EVENT_FORMCLOSING])?.Invoke(this, e);
        }

        /// <summary>
        ///  The Closed event is fired when the form is closed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFormClosed(FormClosedEventArgs e)
        {
            //Remove the form from Application.OpenForms (nothing happens if isn't present)
            Application.OpenForms.Remove(this);

            ((FormClosedEventHandler)Events[EVENT_FORMCLOSED])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the CreateControl event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnCreateControl()
        {
            CalledCreateControl = true;
            base.OnCreateControl();

            if (CalledMakeVisible && !CalledOnLoad)
            {
                CalledOnLoad = true;
                OnLoad(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Raises the Deactivate event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDeactivate(EventArgs e)
        {
            ((EventHandler)Events[EVENT_DEACTIVATE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the EnabledChanged event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (!DesignMode && Enabled && Active)
            {
                // Make sure we activate the active control.
                Control activeControl = ActiveControl;

                if (activeControl is null)
                {
                    SelectNextControl(this, true, true, true, true);
                }
                else
                {
                    FocusActiveControlInternal();
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);

            // Enter events are not raised on mdi child form controls on form Enabled.
            if (IsMdiChild)
            {
                UpdateFocusedControl();
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnFontChanged(EventArgs e)
        {
            if (DesignMode)
            {
                UpdateAutoScaleBaseSize();
            }
            base.OnFontChanged(e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to find out when the
        ///  handle has been created.
        ///  Call base.OnHandleCreated first.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleCreated(EventArgs e)
        {
            formStateEx[FormStateExUseMdiChildProc] = (IsMdiChild && Visible) ? 1 : 0;
            base.OnHandleCreated(e);
            UpdateLayered();
        }

        /// <summary>
        ///  Inheriting classes should override this method to find out when the
        ///  handle is about to be destroyed.
        ///  Call base.OnHandleDestroyed last.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleDestroyed(EventArgs e)
        {
            base.OnHandleDestroyed(e);
            formStateEx[FormStateExUseMdiChildProc] = 0;

            // just make sure we're no longer in the forms collection list
            Application.OpenForms.Remove(this);
        }

        /// <summary>
        ///  Handles the event that a helpButton is clicked
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHelpButtonClicked(CancelEventArgs e)
        {
            ((CancelEventHandler)Events[EVENT_HELPBUTTONCLICKED])?.Invoke(this, e);
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            // Typically forms are either top level or parented to an MDIClient area.
            // In either case, forms a responsible for managing their own size.
            if (AutoSize)
            {
                // If AutoSized, set the Form to the maximum of its preferredSize or the user
                // specified size.
                Size prefSize = PreferredSize;
                minAutoSize = prefSize;

                // This used to use "GetSpecifiedBounds" - but it was not updated when we're in the middle of
                // a modal resizing loop (WM_WINDOWPOSCHANGED).
                Size adjustedSize = AutoSizeMode == AutoSizeMode.GrowAndShrink ? prefSize : LayoutUtils.UnionSizes(prefSize, Size);

                if (this is IArrangedElement form)
                {
                    form.SetBounds(new Rectangle(Left, Top, adjustedSize.Width, adjustedSize.Height), BoundsSpecified.None);
                }
            }
            base.OnLayout(levent);
        }

        /// <summary>
        ///  The Load event is fired before the form becomes visible for the first time.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLoad(EventArgs e)
        {
            //First - add the form to Application.OpenForms
            Application.OpenForms.Add(this);
            if (Application.UseWaitCursor)
            {
                UseWaitCursor = true;
            }

            // subhag: This will apply AutoScaling to the form just
            // before the form becomes visible.
            //
            if (formState[FormStateAutoScaling] == 1 && !DesignMode)
            {
                // Turn off autoscaling so we don't do this on every handle
                // creation.
                //
                formState[FormStateAutoScaling] = 0;
                // Obsolete code required here for backwards compat
                ApplyAutoScaling();
            }

            // Also, at this time we can now locate the form the the correct
            // area of the screen.  We must do this after applying any
            // autoscaling.
            //
            if (GetState(States.Modal))
            {
                FormStartPosition startPos = (FormStartPosition)formState[FormStateStartPos];
                if (startPos == FormStartPosition.CenterParent)
                {
                    CenterToParent();
                }
                else if (startPos == FormStartPosition.CenterScreen)
                {
                    CenterToScreen();
                }
            }

            // There is no good way to explain this event except to say
            // that it's just another name for OnControlCreated.
            EventHandler handler = (EventHandler)Events[EVENT_LOAD];
            if (handler != null)
            {
                string text = Text;

                handler(this, e);

                // It seems that if you set a window style during the onload
                // event, we have a problem initially painting the window.
                // So in the event that the user has set the on load event
                // in their application, we should go ahead and invalidate
                // the controls in their collection so that we paint properly.
                // This seems to manifiest itself in changes to the window caption,
                // and changes to the control box and help.

                foreach (Control c in Controls)
                {
                    c.Invalidate();
                }
            }

            //finally fire the newOnShown(unless the form has already been closed)
            if (IsHandleCreated)
            {
                BeginInvoke(new MethodInvoker(CallShownEvent));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMaximizedBoundsChanged(EventArgs e)
        {
            if (Events[EVENT_MAXIMIZEDBOUNDSCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMaximumSizeChanged(EventArgs e)
        {
            if (Events[EVENT_MAXIMUMSIZECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMinimumSizeChanged(EventArgs e)
        {
            if (Events[EVENT_MINIMUMSIZECHANGED] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='InputLanguageChanged'/>
        ///  event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnInputLanguageChanged(InputLanguageChangedEventArgs e)
        {
            ((InputLanguageChangedEventHandler)Events[EVENT_INPUTLANGCHANGE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='InputLanguageChanging'/>
        ///  event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnInputLanguageChanging(InputLanguageChangingEventArgs e)
        {
            ((InputLanguageChangingEventHandler)Events[EVENT_INPUTLANGCHANGEREQUEST])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnVisibleChanged(EventArgs e)
        {
            UpdateRenderSizeGrip();
            Form mdiParent = MdiParentInternal;
            if (mdiParent != null)
            {
                mdiParent.UpdateMdiWindowListStrip();
            }
            base.OnVisibleChanged(e);

            // Windows forms have to behave like dialog boxes sometimes. If the
            // user has specified that the mouse should snap to the
            // Accept button using the Mouse applet in the control panel,
            // we have to respect that setting each time our form is made visible.
            bool data = false;
            if (IsHandleCreated
                    && Visible
                    && (AcceptButton != null)
                    && User32.SystemParametersInfoW(User32.SPI.GETSNAPTODEFBUTTON, ref data)
                    && data)
            {
                Control button = AcceptButton as Control;
                var ptToSnap = new Point(button.Left + button.Width / 2, button.Top + button.Height / 2);
                User32.ClientToScreen(new HandleRef(this, Handle), ref ptToSnap);
                if (!button.IsWindowObscured)
                {
                    Cursor.Position = ptToSnap;
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='MdiChildActivate'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMdiChildActivate(EventArgs e)
        {
            UpdateMenuHandles();
            UpdateToolStrip();
            ((EventHandler)Events[EVENT_MDI_CHILD_ACTIVATE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MenuStart'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMenuStart(EventArgs e)
        {
            EventHandler handler = (EventHandler)Events[EVENT_MENUSTART];
            handler?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the MenuComplete event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMenuComplete(EventArgs e)
        {
            ((EventHandler)Events[EVENT_MENUCOMPLETE])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the Paint event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (formState[FormStateRenderSizeGrip] != 0)
            {
                Size size = ClientSize;
                if (Application.RenderWithVisualStyles)
                {
                    if (sizeGripRenderer is null)
                    {
                        sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                    }

                    using var hdc = new DeviceContextHdcScope(e);
                    sizeGripRenderer.DrawBackground(
                        hdc,
                        new Rectangle(size.Width - SizeGripSize, size.Height - SizeGripSize, SizeGripSize, SizeGripSize));
                }
                else
                {
                    ControlPaint.DrawSizeGrip(
                        e,
                        BackColor,
                        size.Width - SizeGripSize,
                        size.Height - SizeGripSize,
                        SizeGripSize,
                        SizeGripSize);
                }
            }

            if (IsMdiContainer)
            {
                e.GraphicsInternal.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);
            }
        }

        /// <summary>
        ///  Raises the Resize event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (formState[FormStateRenderSizeGrip] != 0)
            {
                Invalidate();
            }
        }

        /// <summary>
        ///  Raises the DpiChanged event.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        protected virtual void OnDpiChanged(DpiChangedEventArgs e)
        {
            if (e.DeviceDpiNew != e.DeviceDpiOld)
            {
                CommonProperties.xClearAllPreferredSizeCaches(this);

                // call any additional handlers
                ((DpiChangedEventHandler)Events[EVENT_DPI_CHANGED])?.Invoke(this, e);

                if (!e.Cancel)
                {
                    float factor = (float)e.DeviceDpiNew / (float)e.DeviceDpiOld;
                    SuspendAllLayout(this);
                    try
                    {
                        User32.SetWindowPos(
                            new HandleRef(this, HandleInternal),
                            User32.HWND_TOP,
                            e.SuggestedRectangle.X,
                            e.SuggestedRectangle.Y,
                            e.SuggestedRectangle.Width,
                            e.SuggestedRectangle.Height,
                            User32.SWP.NOZORDER | User32.SWP.NOACTIVATE);
                        if (AutoScaleMode != AutoScaleMode.Font)
                        {
                            Font = new Font(Font.FontFamily, Font.Size * factor, Font.Style);
                            FormDpiChanged(factor);
                        }
                        else
                        {
                            ScaleFont(factor);
                            FormDpiChanged(factor);
                        }
                    }
                    finally
                    {
                        // We want to perform layout for dpi-changed HDpi improvements - setting the second parameter to 'true'
                        ResumeAllLayout(this, true);
                    }
                }
            }
        }

        /// <summary>
        ///  Occurs when the DPI resolution of the screen this top level window is displayed on changes,
        ///  either when the top level window is moved between monitors or when the OS settings are changed.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.FormOnDpiChangedDescr))]
        public event DpiChangedEventHandler DpiChanged
        {
            add => Events.AddHandler(EVENT_DPI_CHANGED, value);
            remove => Events.RemoveHandler(EVENT_DPI_CHANGED, value);
        }

        /// <summary>
        ///  Handles the WM_DPICHANGED message
        /// </summary>
        private void WmDpiChanged(ref Message m)
        {
            DefWndProc(ref m);

            DpiChangedEventArgs e = new DpiChangedEventArgs(_deviceDpi, m);
            _deviceDpi = e.DeviceDpiNew;

            OnDpiChanged(e);
        }

        /// <summary>
        ///  Allows derived form to handle WM_GETDPISCALEDSIZE message.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool OnGetDpiScaledSize(int deviceDpiOld, int deviceDpiNew, ref Size desiredSize)
        {
            return false; // scale linearly
        }

        /// <summary>
        ///  Handles the WM_GETDPISCALEDSIZE message, this is a chance for the application to
        ///  scale window size non-lineary. If this message is not processed, the size is scaled linearly by Windows.
        ///  This message is sent to top level windows before WM_DPICHANGED.
        ///  If the application responds to this message, the resulting size will be the candidate rectangle
        ///  sent to WM_DPICHANGED. The WPARAM contains a DPI value. The size needs to be computed if
        ///  the window were to switch to this DPI. LPARAM is unused and will be zero.
        ///  The return value is a size, where the LOWORD is the desired width of the window and the HIWORD
        ///  is the desired height of the window. A return value of zero indicates that the app does not
        ///  want any special behavior and the candidate rectangle will be computed linearly.
        /// </summary>
        private void WmGetDpiScaledSize(ref Message m)
        {
            DefWndProc(ref m);

            Size desiredSize = new Size();
            if (OnGetDpiScaledSize(_deviceDpi, PARAM.SignedLOWORD(m.WParam), ref desiredSize))
            {
                m.Result = PARAM.FromLowHigh(Size.Width, Size.Height);
            }
            else
            {
                m.Result = IntPtr.Zero;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            if (RightToLeft == RightToLeft.Yes)
            {
                RecreateHandle();
            }

            if (Events[EVENT_RIGHTTOLEFTLAYOUTCHANGED] is EventHandler eh)
            {
                eh(this, e);
            }

            // Want to do this after we fire the event.
            if (RightToLeft == RightToLeft.Yes)
            {
                foreach (Control c in Controls)
                {
                    c.RecreateHandleCore();
                }
            }
        }

        /// <summary>
        ///  Thi event fires whenever the form is first shown.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnShown(EventArgs e)
        {
            ((EventHandler)Events[EVENT_SHOWN])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);

            // If there is no control box, there should only be a title bar if text != "".
            int newTextEmpty = Text.Length == 0 ? 1 : 0;
            if (!ControlBox && formState[FormStateIsTextEmpty] != newTextEmpty)
            {
                RecreateHandle();
            }

            formState[FormStateIsTextEmpty] = newTextEmpty;
        }

        /// <summary>
        ///  Simulates a InputLanguageChanged event. Used by Control to forward events
        ///  to the parent form.
        /// </summary>
        internal void PerformOnInputLanguageChanged(InputLanguageChangedEventArgs iplevent)
        {
            OnInputLanguageChanged(iplevent);
        }

        /// <summary>
        ///  Simulates a InputLanguageChanging event. Used by Control to forward
        ///  events to the parent form.
        /// </summary>
        internal void PerformOnInputLanguageChanging(InputLanguageChangingEventArgs iplcevent)
        {
            OnInputLanguageChanging(iplcevent);
        }

        /// <summary>
        ///  Processes a command key. Overrides Control.processCmdKey() to provide
        ///  additional handling of main menu command keys and Mdi accelerators.
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (base.ProcessCmdKey(ref msg, keyData))
            {
                return true;
            }

            // Process MDI accelerator keys.
            bool retValue = false;
            User32.MSG win32Message = msg;
            if (ctlClient != null && ctlClient.Handle != IntPtr.Zero &&
                User32.TranslateMDISysAccel(ctlClient.Handle, ref win32Message).IsTrue())
            {
                retValue = true;
            }

            msg.Msg = (int)win32Message.message;
            msg.WParam = win32Message.wParam;
            msg.LParam = win32Message.lParam;
            msg.HWnd = win32Message.hwnd;

            return retValue;
        }

        /// <summary>
        ///  Processes a dialog key. Overrides Control.processDialogKey(). This
        ///  method implements handling of the RETURN, and ESCAPE keys in dialogs.
        ///  The method performs no processing on keys that include the ALT or
        ///  CONTROL modifiers.
        /// </summary>
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
            {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                IButtonControl button;

                switch (keyCode)
                {
                    case Keys.Return:
                        button = (IButtonControl)Properties.GetObject(PropDefaultButton);
                        if (button != null)
                        {
                            //PerformClick now checks for validationcancelled...
                            if (button is Control)
                            {
                                button.PerformClick();
                            }
                            return true;
                        }
                        break;
                    case Keys.Escape:
                        button = (IButtonControl)Properties.GetObject(PropCancelButton);
                        if (button != null)
                        {
                            // In order to keep the behavior in sync with native
                            // and MFC dialogs, we want to not give the cancel button
                            // the focus on Escape. If we do, we end up with giving it
                            // the focus when we reshow the dialog.
                            //
                            //if (button is Control) {
                            //    ((Control)button).Focus();
                            //}
                            button.PerformClick();
                            return true;
                        }
                        break;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  Processes a dialog character For a MdiChild.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override bool ProcessDialogChar(char charCode)
        {
#if DEBUG
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Form.ProcessDialogChar [" + charCode.ToString() + "]");
#endif
            // If we're the top-level form or control, we need to do the mnemonic handling
            //
            if (IsMdiChild && charCode != ' ')
            {
                if (ProcessMnemonic(charCode))
                {
                    return true;
                }

                // ContainerControl calls ProcessMnemonic starting from the active MdiChild form (this)
                // so let's flag it as processed.
                formStateEx[FormStateExMnemonicProcessed] = 1;
                try
                {
                    return base.ProcessDialogChar(charCode);
                }
                finally
                {
                    formStateEx[FormStateExMnemonicProcessed] = 0;
                }
            }

            // Non-MdiChild form, just pass the call to ContainerControl.
            return base.ProcessDialogChar(charCode);
        }

        protected override bool ProcessKeyPreview(ref Message m)
        {
            if (formState[FormStateKeyPreview] != 0 && ProcessKeyEventArgs(ref m))
            {
                return true;
            }

            return base.ProcessKeyPreview(ref m);
        }

        protected override bool ProcessTabKey(bool forward)
        {
            if (SelectNextControl(ActiveControl, forward, true, true, true))
            {
                return true;
            }

            // I've added a special case for UserControls because they shouldn't cycle back to the
            // beginning if they don't have a parent form, such as when they're on an ActiveXBridge.
            if (IsMdiChild || ParentForm is null)
            {
                bool selected = SelectNextControl(null, forward, true, true, false);

                if (selected)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  Raises the FormClosed event for this form when Application.Exit is called.
        /// </summary>
        internal void RaiseFormClosedOnAppExit()
        {
            if (!Modal)
            {
                // Fire FormClosed event on all the forms that this form owns and are not in the Application.OpenForms collection
                // This is to be consistent with what WmClose does.
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                if (ownedFormsCount > 0)
                {
                    Form[] ownedForms = OwnedForms;
                    FormClosedEventArgs fce = new FormClosedEventArgs(CloseReason.FormOwnerClosing);
                    for (int i = ownedFormsCount - 1; i >= 0; i--)
                    {
                        if (ownedForms[i] != null && !Application.OpenForms.Contains(ownedForms[i]))
                        {
                            ownedForms[i].OnFormClosed(fce);
                        }
                    }
                }
            }
            OnFormClosed(new FormClosedEventArgs(CloseReason.ApplicationExitCall));
        }

        /// <summary>
        ///  Raises the FormClosing event for this form when Application.Exit is called.
        ///  Returns e.Cancel returned by the event handler.
        /// </summary>
        internal bool RaiseFormClosingOnAppExit()
        {
            FormClosingEventArgs e = new FormClosingEventArgs(CloseReason.ApplicationExitCall, false);
            // e.Cancel = !Validate(true);    This would cause a breaking change between v2.0 and v1.0/v1.1 in case validation fails.
            if (!Modal)
            {
                // Fire FormClosing event on all the forms that this form owns and are not in the Application.OpenForms collection
                // This is to be consistent with what WmClose does.
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                if (ownedFormsCount > 0)
                {
                    Form[] ownedForms = OwnedForms;
                    FormClosingEventArgs fce = new FormClosingEventArgs(CloseReason.FormOwnerClosing, false);
                    for (int i = ownedFormsCount - 1; i >= 0; i--)
                    {
                        if (ownedForms[i] != null && !Application.OpenForms.Contains(ownedForms[i]))
                        {
                            ownedForms[i].OnFormClosing(fce);
                            if (fce.Cancel)
                            {
                                e.Cancel = true;
                                break;
                            }
                        }
                    }
                }
            }
            OnFormClosing(e);
            return e.Cancel;
        }

        internal override void RecreateHandleCore()
        {
            var wp = new User32.WINDOWPLACEMENT();
            FormStartPosition oldStartPosition = FormStartPosition.Manual;

            if (!IsMdiChild && (WindowState == FormWindowState.Minimized || WindowState == FormWindowState.Maximized))
            {
                User32.GetWindowPlacement(Handle, out wp);
            }

            if (StartPosition != FormStartPosition.Manual)
            {
                oldStartPosition = StartPosition;
                // Set the startup postion to manual, to stop the form from
                // changing position each time RecreateHandle() is called.
                StartPosition = FormStartPosition.Manual;
            }

            EnumThreadWindowsCallback etwcb = null;
            if (IsHandleCreated)
            {
                // First put all the owned windows into a list
                etwcb = new EnumThreadWindowsCallback(Handle);
                User32.EnumThreadWindows(
                    Kernel32.GetCurrentThreadId(),
                    etwcb.Callback);
                GC.KeepAlive(this);
                // Reset the owner of the windows in the list
                etwcb.ResetOwners();
            }

            base.RecreateHandleCore();

            if (etwcb != null)
            {
                // Set the owner of the windows in the list back to the new Form's handle
                etwcb.SetOwners(new HandleRef(this, Handle));
            }

            if (oldStartPosition != FormStartPosition.Manual)
            {
                StartPosition = oldStartPosition;
            }

            if (wp.length > 0)
            {
                User32.SetWindowPlacement(this, ref wp);
            }
        }

        /// <summary>
        ///  Removes a form from the list of owned forms. Also sets the owner of the
        ///  removed form to null.
        /// </summary>
        public void RemoveOwnedForm(Form ownedForm)
        {
            if (ownedForm is null)
            {
                return;
            }

            if (ownedForm.OwnerInternal != null)
            {
                ownedForm.Owner = null; // NOTE: this will call RemoveOwnedForm again, bypassing if.
                return;
            }

            Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
            int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

            if (ownedForms != null)
            {
                for (int i = 0; i < ownedFormsCount; i++)
                {
                    if (ownedForm.Equals(ownedForms[i]))
                    {
                        // clear out the reference.
                        //
                        ownedForms[i] = null;

                        // compact the array.
                        //
                        if (i + 1 < ownedFormsCount)
                        {
                            Array.Copy(ownedForms, i + 1, ownedForms, i, ownedFormsCount - i - 1);
                            ownedForms[ownedFormsCount - 1] = null;
                        }
                        ownedFormsCount--;
                    }
                }

                Properties.SetInteger(PropOwnedFormsCount, ownedFormsCount);
            }
        }

        /// <summary>
        ///  Resets the form's icon the the default value.
        /// </summary>
        private void ResetIcon()
        {
            icon = null;
            if (smallIcon != null)
            {
                smallIcon.Dispose();
                smallIcon = null;
            }
            formState[FormStateIconSet] = 0;
            UpdateWindowIcon(true);
        }

        /// <summary>
        ///  Resets the TransparencyKey to Color.Empty.
        /// </summary>
        private void ResetTransparencyKey()
        {
            TransparencyKey = Color.Empty;
        }

        /// <summary>
        ///  Occurs when the form enters the sizing modal loop
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.FormOnResizeBeginDescr))]
        public event EventHandler ResizeBegin
        {
            add => Events.AddHandler(EVENT_RESIZEBEGIN, value);
            remove => Events.RemoveHandler(EVENT_RESIZEBEGIN, value);
        }

        /// <summary>
        ///  Occurs when the control exits the sizing modal loop.
        /// </summary>
        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.FormOnResizeEndDescr))]
        public event EventHandler ResizeEnd
        {
            add => Events.AddHandler(EVENT_RESIZEEND, value);
            remove => Events.RemoveHandler(EVENT_RESIZEEND, value);
        }

        /// <summary>
        ///  This is called when we have just been restored after being
        ///  minimized.  At this point we resume our layout.
        /// </summary>
        private void ResumeLayoutFromMinimize()
        {
            // If we're currently minimized, resume our layout because we are
            // about to snap out of it.
            if (formState[FormStateWindowState] == (int)FormWindowState.Minimized)
            {
                ResumeLayout();
            }
        }

        // If someone set Location or Size while the form was maximized or minimized,
        // we had to cache the new value away until after the form was restored to normal size.
        // This function is called after WindowState changes, and handles the above logic.
        // In the normal case where no one sets Location or Size programmatically,
        // Windows does the restoring for us.
        //
        private void RestoreWindowBoundsIfNecessary()
        {
            if (WindowState == FormWindowState.Normal)
            {
                Size restoredSize = restoredWindowBounds.Size;
                if ((restoredWindowBoundsSpecified & BoundsSpecified.Size) != 0)
                {
                    restoredSize = SizeFromClientSize(restoredSize.Width, restoredSize.Height);
                }
                SetBounds(restoredWindowBounds.X, restoredWindowBounds.Y,
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] == 1 ? restoredSize.Width : restoredWindowBounds.Width,
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] == 1 ? restoredSize.Height : restoredWindowBounds.Height,
                          restoredWindowBoundsSpecified);
                restoredWindowBoundsSpecified = 0;
                restoredWindowBounds = new Rectangle(-1, -1, -1, -1);
                formStateEx[FormStateExWindowBoundsHeightIsClientSize] = 0;
                formStateEx[FormStateExWindowBoundsWidthIsClientSize] = 0;
            }
        }

        /// <summary>
        ///  Decrements updateMenuHandleSuspendCount. If updateMenuHandleSuspendCount
        ///  becomes zero and updateMenuHandlesDeferred is true, updateMenuHandles
        ///  is called.
        /// </summary>
        private void ResumeUpdateMenuHandles()
        {
            int suspendCount = formStateEx[FormStateExUpdateMenuHandlesSuspendCount];
            if (suspendCount <= 0)
            {
                throw new InvalidOperationException(SR.TooManyResumeUpdateMenuHandles);
            }

            formStateEx[FormStateExUpdateMenuHandlesSuspendCount] = --suspendCount;
            if (suspendCount == 0 && formStateEx[FormStateExUpdateMenuHandlesDeferred] != 0)
            {
                UpdateMenuHandles();
            }
        }

        /// <summary>
        ///  Selects this form, and optionally selects the next/previous control.
        /// </summary>
        protected override void Select(bool directed, bool forward)
        {
            if (directed)
            {
                SelectNextControl(null, forward, true, true, false);
            }

            if (TopLevel)
            {
                User32.SetActiveWindow(new HandleRef(this, Handle));
            }
            else if (IsMdiChild)
            {
                User32.SetActiveWindow(new HandleRef(MdiParentInternal, MdiParentInternal.Handle));
                User32.SendMessageW(MdiParentInternal.MdiClient, User32.WM.MDIACTIVATE, Handle, IntPtr.Zero);
            }
            else
            {
                Form form = ParentForm;
                if (form != null)
                {
                    form.ActiveControl = this;
                }
            }
        }

        /// <summary>
        ///  Base function that performs scaling of the form.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float x, float y)
        {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, GetType().Name + "::ScaleCore(" + x + ", " + y + ")");
            SuspendLayout();
            try
            {
                if (WindowState == FormWindowState.Normal)
                {
                    //Get size values in advance to prevent one change from affecting another.
                    Size clientSize = ClientSize;
                    Size minSize = MinimumSize;
                    Size maxSize = MaximumSize;
                    if (!MinimumSize.IsEmpty)
                    {
                        MinimumSize = ScaleSize(minSize, x, y);
                    }
                    if (!MaximumSize.IsEmpty)
                    {
                        MaximumSize = ScaleSize(maxSize, x, y);
                    }
                    ClientSize = ScaleSize(clientSize, x, y);
                }

                ScaleDockPadding(x, y);

                foreach (Control control in Controls)
                {
                    if (control != null)
                    {
                        control.Scale(x, y);
                    }
                }
            }
            finally
            {
                ResumeLayout();
            }
        }

        /// <summary>
        ///  For overrides this to calculate scaled bounds based on the restored rect
        ///  if it is maximized or minimized.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            // If we're maximized or minimized, scale using the restored bounds, not
            // the real bounds.
            if (WindowState != FormWindowState.Normal)
            {
                bounds = RestoreBounds;
            }
            return base.GetScaledBounds(bounds, factor, specified);
        }

        /// <summary>
        ///  Scale this form.  Form overrides this to enforce a maximum / minimum size.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            formStateEx[FormStateExInScale] = 1;
            try
            {
                // don't scale the location of MDI child forms
                if (MdiParentInternal != null)
                {
                    specified &= ~BoundsSpecified.Location;
                }

                base.ScaleControl(factor, specified);
            }
            finally
            {
                formStateEx[FormStateExInScale] = 0;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if (WindowState != FormWindowState.Normal)
            {
                // See RestoreWindowBoundsIfNecessary for an explanation of this
                // Only restore position when x,y is not -1,-1
                if (x != -1 || y != -1)
                {
                    restoredWindowBoundsSpecified |= (specified & (BoundsSpecified.X | BoundsSpecified.Y));
                }

                restoredWindowBoundsSpecified |= (specified & (BoundsSpecified.Width | BoundsSpecified.Height));

                if ((specified & BoundsSpecified.X) != 0)
                {
                    restoredWindowBounds.X = x;
                }
                if ((specified & BoundsSpecified.Y) != 0)
                {
                    restoredWindowBounds.Y = y;
                }
                if ((specified & BoundsSpecified.Width) != 0)
                {
                    restoredWindowBounds.Width = width;
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] = 0;
                }
                if ((specified & BoundsSpecified.Height) != 0)
                {
                    restoredWindowBounds.Height = height;
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] = 0;
                }
            }

            // Update RestoreBounds
            if ((specified & BoundsSpecified.X) != 0)
            {
                restoreBounds.X = x;
            }
            if ((specified & BoundsSpecified.Y) != 0)
            {
                restoreBounds.Y = y;
            }
            if ((specified & BoundsSpecified.Width) != 0 || restoreBounds.Width == -1)
            {
                restoreBounds.Width = width;
            }
            if ((specified & BoundsSpecified.Height) != 0 || restoreBounds.Height == -1)
            {
                restoreBounds.Height = height;
            }

            // Enforce maximum size...
            if (WindowState == FormWindowState.Normal && (Height != height || Width != width))
            {
                Size max = SystemInformation.MaxWindowTrackSize;
                if (height > max.Height)
                {
                    height = max.Height;
                }
                if (width > max.Width)
                {
                    width = max.Width;
                }
            }

            // Only enforce the minimum size if the form has a border and is a top
            // level form.
            FormBorderStyle borderStyle = FormBorderStyle;
            if (borderStyle != FormBorderStyle.None
                && borderStyle != FormBorderStyle.FixedToolWindow
                && borderStyle != FormBorderStyle.SizableToolWindow
                && ParentInternal is null)
            {
                Size min = SystemInformation.MinWindowTrackSize;
                if (height < min.Height)
                {
                    height = min.Height;
                }
                if (width < min.Width)
                {
                    width = min.Width;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <summary>
        ///  Sets the defaultButton for the form. The defaultButton is "clicked" when
        ///  the user presses Enter.
        /// </summary>
        private void SetDefaultButton(IButtonControl button)
        {
            IButtonControl defaultButton = (IButtonControl)Properties.GetObject(PropDefaultButton);

            if (defaultButton != button)
            {
                if (defaultButton != null)
                {
                    defaultButton.NotifyDefault(false);
                }

                Properties.SetObject(PropDefaultButton, button);
                if (button != null)
                {
                    button.NotifyDefault(true);
                }
            }
        }

        /// <summary>
        ///  Sets the clientSize of the form. This will adjust the bounds of the form
        ///  to make the clientSize the requested size.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetClientSizeCore(int x, int y)
        {
            bool hadHScroll = HScroll, hadVScroll = VScroll;
            base.SetClientSizeCore(x, y);

            if (IsHandleCreated)
            {
                // Adjust for the scrollbars, if they were introduced by
                // the call to base.SetClientSizeCore
                if (VScroll != hadVScroll)
                {
                    if (VScroll)
                    {
                        x += SystemInformation.VerticalScrollBarWidth;
                    }
                }
                if (HScroll != hadHScroll)
                {
                    if (HScroll)
                    {
                        y += SystemInformation.HorizontalScrollBarHeight;
                    }
                }
                if (x != ClientSize.Width || y != ClientSize.Height)
                {
                    base.SetClientSizeCore(x, y);
                }
            }
            formState[FormStateSetClientSize] = 1;
        }

        /// <summary>
        ///  Sets the bounds of the form in desktop coordinates.
        /// </summary>
        public void SetDesktopBounds(int x, int y, int width, int height)
        {
            Rectangle workingArea = SystemInformation.WorkingArea;
            SetBounds(x + workingArea.X, y + workingArea.Y, width, height, BoundsSpecified.All);
        }

        /// <summary>
        ///  Sets the location of the form in desktop coordinates.
        /// </summary>
        public void SetDesktopLocation(int x, int y)
        {
            Rectangle workingArea = SystemInformation.WorkingArea;
            Location = new Point(workingArea.X + x, workingArea.Y + y);
        }

        /// <summary>
        ///  Makes the control display by setting the visible property to true
        /// </summary>
        public void Show(IWin32Window owner)
        {
            if (owner == this)
            {
                throw new InvalidOperationException(string.Format(SR.OwnsSelfOrOwner, "Show"));
            }

            if (Visible)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnVisible, "Show"));
            }

            if (!Enabled)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnDisabled, "Show"));
            }

            if (!TopLevel)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnNonTopLevel, "Show"));
            }

            if (!SystemInformation.UserInteractive)
            {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }

            if ((owner != null) && ((int)User32.GetWindowLong(new HandleRef(owner, Control.GetSafeHandle(owner)), User32.GWL.EXSTYLE)
                     & (int)User32.WS_EX.TOPMOST) == 0)
            {   // It's not the top-most window
                if (owner is Control ownerControl)
                {
                    owner = ownerControl.TopLevelControlInternal;
                }
            }

            IntPtr hWndActive = User32.GetActiveWindow();
            IntPtr hWndOwner = owner is null ? hWndActive : Control.GetSafeHandle(owner);
            IntPtr hWndOldOwner = IntPtr.Zero;
            Properties.SetObject(PropDialogOwner, owner);
            Form oldOwner = OwnerInternal;
            if (owner is Form ownerForm && owner != oldOwner)
            {
                Owner = ownerForm;
            }
            if (hWndOwner != IntPtr.Zero && hWndOwner != Handle)
            {
                // Catch the case of a window trying to own its owner
                if (User32.GetWindowLong(new HandleRef(owner, hWndOwner), User32.GWL.HWNDPARENT) == Handle)
                {
                    throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner, "show"), nameof(owner));
                }

                // Set the new owner.
                hWndOldOwner = User32.GetWindowLong(this, User32.GWL.HWNDPARENT);
                User32.SetWindowLong(this, User32.GWL.HWNDPARENT, new HandleRef(owner, hWndOwner));
            }
            Visible = true;
        }

        /// <summary>
        ///  Displays this form as a modal dialog box with no owner window.
        /// </summary>
        public DialogResult ShowDialog()
        {
            return ShowDialog(null);
        }

        /// <summary>
        ///  Shows this form as a modal dialog with the specified owner.
        /// </summary>
        public DialogResult ShowDialog(IWin32Window owner)
        {
            if (owner == this)
            {
                throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner, "showDialog"), nameof(owner));
            }

            if (Visible)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnVisible, "showDialog"));
            }

            if (!Enabled)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnDisabled, "showDialog"));
            }

            if (!TopLevel)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnNonTopLevel, "showDialog"));
            }

            if (Modal)
            {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnModal, "showDialog"));
            }

            if (!SystemInformation.UserInteractive)
            {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }

            if ((owner != null) && ((int)User32.GetWindowLong(new HandleRef(owner, GetSafeHandle(owner)), User32.GWL.EXSTYLE)
                     & (int)User32.WS_EX.TOPMOST) == 0)
            {   // It's not the top-most window
                if (owner is Control ownerControl)
                {
                    owner = ownerControl.TopLevelControlInternal;
                }
            }

            CalledOnLoad = false;
            CalledMakeVisible = false;

            // for modal dialogs make sure we reset close reason.
            CloseReason = CloseReason.None;

            IntPtr hWndCapture = User32.GetCapture();
            if (hWndCapture != IntPtr.Zero)
            {
                User32.SendMessageW(hWndCapture, User32.WM.CANCELMODE);
                User32.ReleaseCapture();
            }
            IntPtr hWndActive = User32.GetActiveWindow();
            IntPtr hWndOwner = owner is null ? hWndActive : Control.GetSafeHandle(owner);

            Form oldOwner = OwnerInternal;

            try
            {
                SetState(States.Modal, true);

                // It's possible that while in the process of creating the control,
                // (i.e. inside the CreateControl() call) the dialog can be closed.
                // e.g. A user might call Close() inside the OnLoad() event.
                // Calling Close() will set the DialogResult to some value, so that
                // we'll know to terminate the RunDialog loop immediately.
                // Thus we must initialize the DialogResult *before* the call
                // to CreateControl().
                //
                dialogResult = DialogResult.None;

                // If "this" is an MDI parent then the window gets activated,
                // causing GetActiveWindow to return "this.handle"... to prevent setting
                // the owner of this to this, we must create the control AFTER calling
                // GetActiveWindow.
                //
                CreateControl();

                if (hWndOwner != IntPtr.Zero && hWndOwner != Handle)
                {
                    // Catch the case of a window trying to own its owner
                    if (User32.GetWindowLong(new HandleRef(owner, hWndOwner), User32.GWL.HWNDPARENT) == Handle)
                    {
                        throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner, "showDialog"), nameof(owner));
                    }

                    // In a multi DPI environment and applications in PMV2 mode, DPI changed events triggered
                    // only when there is a DPI change happened for the Handle directly or via its parent.
                    // So, it is necessary to not set the owner before creating the handle. Otherwise,
                    // the window may never receive DPI changed event even if its parent has different DPI.
                    // Users at runtime, has to move the window between the screens to get the DPI changed events triggered.

                    Properties.SetObject(PropDialogOwner, owner);
                    if (owner is Form form && owner != oldOwner)
                    {
                        Owner = form;
                    }
                    else
                    {
                        // Set the new parent.
                        User32.SetWindowLong(this, User32.GWL.HWNDPARENT, new HandleRef(owner, hWndOwner));
                    }
                }

                try
                {
                    // If the DialogResult was already set, then there's
                    // no need to actually display the dialog.
                    //
                    if (dialogResult == DialogResult.None)
                    {
                        // Application.RunDialog sets this dialog to be visible.
                        Application.RunDialog(this);
                    }
                }
                finally
                {
                    // Call SetActiveWindow before setting Visible = false.
                    //

                    if (User32.IsWindow(hWndActive).IsFalse())
                    {
                        hWndActive = hWndOwner;
                    }

                    if (User32.IsWindow(hWndActive).IsTrue() && User32.IsWindowVisible(hWndActive).IsTrue())
                    {
                        User32.SetActiveWindow(hWndActive);
                    }
                    else if (User32.IsWindow(hWndOwner).IsTrue() && User32.IsWindowVisible(hWndOwner).IsTrue())
                    {
                        User32.SetActiveWindow(hWndOwner);
                    }

                    SetVisibleCore(false);
                    if (IsHandleCreated)
                    {
                        // If this is a dialog opened from an MDI Container, then invalidate
                        // so that child windows will be properly updated.
                        if (OwnerInternal != null &&
                            OwnerInternal.IsMdiContainer)
                        {
                            OwnerInternal.Invalidate(true);
                            OwnerInternal.Update();
                        }

                        // Everett/RTM used to wrap this in an assert for AWP.
                        DestroyHandle();
                    }
                    SetState(States.Modal, false);
                }
            }
            finally
            {
                Owner = oldOwner;
                Properties.SetObject(PropDialogOwner, null);
            }
            return DialogResult;
        }

        /// <summary>
        ///  Indicates whether the <see cref='AutoScaleBaseSize'/> property should be
        ///  persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeAutoScaleBaseSize()
        {
            return formState[FormStateAutoScaling] != 0;
        }

        private bool ShouldSerializeClientSize()
        {
            return true;
        }

        /// <summary>
        ///  Indicates whether the <see cref='Icon'/> property should be persisted.
        /// </summary>
        private bool ShouldSerializeIcon()
        {
            return formState[FormStateIconSet] == 1;
        }

        /// <summary>
        ///  Determines if the Location property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeLocation()
        {
            return Left != 0 || Top != 0;
        }

        /// <summary>
        ///  Indicates whether the <see cref='Size'/> property should be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override bool ShouldSerializeSize()
        {
            return false;
        }

        /// <summary>
        ///  Indicates whether the <see cref='TransparencyKey'/> property should be
        ///  persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeTransparencyKey()
        {
            return !TransparencyKey.Equals(Color.Empty);
        }

        internal override bool SupportsUiaProviders => true;

        /// <summary>
        ///  This is called when we are about to become minimized.  Laying out
        ///  while minimized can be a problem because the physical dimensions
        ///  of the window are very small.  So, we simply suspend.
        /// </summary>
        private void SuspendLayoutForMinimize()
        {
            // If we're not currently minimized, suspend our layout because we are
            // about to become minimized
            if (formState[FormStateWindowState] != (int)FormWindowState.Minimized)
            {
                SuspendLayout();
            }
        }

        /// <summary>
        ///  Increments updateMenuHandleSuspendCount.
        /// </summary>
        private void SuspendUpdateMenuHandles()
        {
            int suspendCount = formStateEx[FormStateExUpdateMenuHandlesSuspendCount];
            formStateEx[FormStateExUpdateMenuHandlesSuspendCount] = ++suspendCount;
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return s + ", Text: " + Text;
        }

        /// <summary>
        ///  Updates the autoscalebasesize based on the current font.
        /// </summary>
        private void UpdateAutoScaleBaseSize()
        {
            autoScaleBaseSize = Size.Empty;
        }

        private void UpdateRenderSizeGrip()
        {
            int current = formState[FormStateRenderSizeGrip];
            switch (FormBorderStyle)
            {
                case FormBorderStyle.None:
                case FormBorderStyle.FixedSingle:
                case FormBorderStyle.Fixed3D:
                case FormBorderStyle.FixedDialog:
                case FormBorderStyle.FixedToolWindow:
                    formState[FormStateRenderSizeGrip] = 0;
                    break;
                case FormBorderStyle.Sizable:
                case FormBorderStyle.SizableToolWindow:
                    switch (SizeGripStyle)
                    {
                        case SizeGripStyle.Show:
                            formState[FormStateRenderSizeGrip] = 1;
                            break;
                        case SizeGripStyle.Hide:
                            formState[FormStateRenderSizeGrip] = 0;
                            break;
                        case SizeGripStyle.Auto:
                            if (GetState(States.Modal))
                            {
                                formState[FormStateRenderSizeGrip] = 1;
                            }
                            else
                            {
                                formState[FormStateRenderSizeGrip] = 0;
                            }
                            break;
                    }
                    break;
            }

            if (formState[FormStateRenderSizeGrip] != current)
            {
                Invalidate();
            }
        }

        /// <summary>
        ///  Updates the default button based on current selection, and the
        ///  acceptButton property.
        /// </summary>
        protected override void UpdateDefaultButton()
        {
            ContainerControl cc = this;

            while (cc.ActiveControl is ContainerControl)
            {
                cc = cc.ActiveControl as ContainerControl;
                Debug.Assert(cc != null);

                if (cc is Form)
                {
                    // Don't allow a parent form to get its default button from a child form,
                    // otherwise the two forms will 'compete' for the Enter key and produce unpredictable results.
                    // This is aimed primarily at fixing the behavior of MDI container forms.
                    cc = this;
                    break;
                }
            }

            if (cc.ActiveControl is IButtonControl)
            {
                SetDefaultButton((IButtonControl)cc.ActiveControl);
            }
            else
            {
                SetDefaultButton(AcceptButton);
            }
        }

        /// <summary>
        ///  Updates the underlying hWnd with the correct parent/owner of the form.
        /// </summary>
        private void UpdateHandleWithOwner()
        {
            if (IsHandleCreated && TopLevel)
            {
                HandleRef ownerHwnd = NativeMethods.NullHandleRef;

                Form owner = (Form)Properties.GetObject(PropOwner);

                if (owner != null)
                {
                    ownerHwnd = new HandleRef(owner, owner.Handle);
                }
                else
                {
                    if (!ShowInTaskbar)
                    {
                        ownerHwnd = TaskbarOwner;
                    }
                }

                User32.SetWindowLong(this, User32.GWL.HWNDPARENT, ownerHwnd);
            }
        }

        /// <summary>
        ///  Updates the layered window attributes if the control
        ///  is in layered mode.
        /// </summary>
        private void UpdateLayered()
        {
            if ((formState[FormStateLayered] != 0) && IsHandleCreated && TopLevel)
            {
                BOOL result;

                Color transparencyKey = TransparencyKey;

                if (transparencyKey.IsEmpty)
                {
                    result = User32.SetLayeredWindowAttributes(this, 0, OpacityAsByte, User32.LWA.ALPHA);
                }
                else if (OpacityAsByte == 255)
                {
                    // Windows doesn't do so well setting colorkey and alpha, so avoid it if we can
                    result = User32.SetLayeredWindowAttributes(this, ColorTranslator.ToWin32(transparencyKey), 0, User32.LWA.COLORKEY);
                }
                else
                {
                    result = User32.SetLayeredWindowAttributes(this, ColorTranslator.ToWin32(transparencyKey), OpacityAsByte, User32.LWA.ALPHA | User32.LWA.COLORKEY);
                }

                if (result.IsFalse())
                {
                    throw new Win32Exception();
                }
            }
        }

        private void UpdateMenuHandles()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            if (ctlClient is null || !ctlClient.IsHandleCreated)
            {
                User32.SetMenu(this, NativeMethods.NullHandleRef);
            }
            else
            {
                Debug.Assert(IsMdiContainer, "Not an MDI container!");
                // when both MainMenuStrip and Menu are set, we honor the win32 menu over
                // the MainMenuStrip as the place to store the system menu controls for the maximized MDI child.

                MenuStrip mainMenuStrip = MainMenuStrip;
                // (New fix: Only destroy Win32 Menu if using a MenuStrip)
                if (mainMenuStrip != null)
                {
                    // If MainMenuStrip, we need to remove any Win32 Menu to make room for it.
                    IntPtr hMenu = User32.GetMenu(this);
                    if (hMenu != IntPtr.Zero)
                    {
                        // We had a MainMenu and now we're switching over to MainMenuStrip

                        // Remove the current menu.
                        User32.SetMenu(this, NativeMethods.NullHandleRef);

                        // because we have messed with the child's system menu by shoving in our own dummy menu,
                        // once we clear the main menu we're in trouble - this eats the close, minimize, maximize gadgets
                        // of the child form. (See WM_MDISETMENU in MSDN)
                        Form activeMdiChild = ActiveMdiChildInternal;
                        if (activeMdiChild != null && activeMdiChild.WindowState == FormWindowState.Maximized)
                        {
                            activeMdiChild.RecreateHandle();
                        }

                        // Since we're removing a menu but we possibly had a menu previously,
                        // we need to clear the cached size so that new size calculations will be performed correctly.
                        CommonProperties.xClearPreferredSizeCache(this);
                    }
                }
            }

            User32.DrawMenuBar(this);

            formStateEx[FormStateExUpdateMenuHandlesDeferred] = 0;
        }

        // Call this function instead of UpdateStyles() when the form's client-size must
        // be preserved e.g. when changing the border style.
        //
        internal void UpdateFormStyles()
        {
            Size previousClientSize = ClientSize;
            base.UpdateStyles();
            if (!ClientSize.Equals(previousClientSize))
            {
                ClientSize = previousClientSize;
            }
        }

        private static Type FindClosestStockType(Type type)
        {
            Type[] stockTypes = new Type[] { typeof(MenuStrip) }; // as opposed to what we had before...
                                                                  // simply add other types here from most specific to most generic if we want to merge other types of toolstrips...
            foreach (Type t in stockTypes)
            {
                if (t.IsAssignableFrom(type))
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary> ToolStrip MDI Merging support </summary>
        private void UpdateToolStrip()
        {
            // try to merge each one of the MDI Child toolstrip with the first toolstrip
            // in the parent form that has the same type NOTE: THESE LISTS ARE ORDERED (See ToolstripManager)
            ToolStrip thisToolstrip = MainMenuStrip;
            ArrayList childrenToolStrips = ToolStripManager.FindMergeableToolStrips(ActiveMdiChildInternal);

            // revert any previous merge
            if (thisToolstrip != null)
            {
                ToolStripManager.RevertMerge(thisToolstrip);
            }

            // if someone has a MdiWindowListItem specified we should merge in the
            // names of all the MDI child forms.
            UpdateMdiWindowListStrip();

            if (ActiveMdiChildInternal != null)
            {
                // do the new merging
                foreach (ToolStrip sourceToolStrip in childrenToolStrips)
                {
                    Type closestMatchingSourceType = FindClosestStockType(sourceToolStrip.GetType());
                    if (thisToolstrip != null)
                    {
                        Type closestMatchingTargetType = FindClosestStockType(thisToolstrip.GetType());
                        if (closestMatchingTargetType != null && closestMatchingSourceType != null &&
                            closestMatchingSourceType == closestMatchingTargetType &&
                            thisToolstrip.GetType().IsAssignableFrom(sourceToolStrip.GetType()))
                        {
                            ToolStripManager.Merge(sourceToolStrip, thisToolstrip);
                            break;
                        }
                    }
                }
            }

            // add in the control gadgets for the mdi child form to the first menu strip
            Form activeMdiForm = ActiveMdiChildInternal;
            UpdateMdiControlStrip(activeMdiForm != null && activeMdiForm.IsMaximized);
        }

        private void UpdateMdiControlStrip(bool maximized)
        {
            if (formStateEx[FormStateExInUpdateMdiControlStrip] != 0)
            {
                return;
            }

            // we dont want to be redundantly called as we could merge in two control menus.
            formStateEx[FormStateExInUpdateMdiControlStrip] = 1;

            try
            {
                MdiControlStrip mdiControlStrip = MdiControlStrip;

                if (MdiControlStrip != null)
                {
                    if (mdiControlStrip.MergedMenu != null)
                    {
#if DEBUG
                        int numWindowListItems = 0;
                        if (MdiWindowListStrip != null && MdiWindowListStrip.MergedMenu != null && MdiWindowListStrip.MergedMenu.MdiWindowListItem != null)
                        {
                            numWindowListItems = MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count;
                        }
#endif

                        ToolStripManager.RevertMergeInternal(mdiControlStrip.MergedMenu, mdiControlStrip,/*revertMDIStuff*/true);

#if DEBUG
                        // double check that RevertMerge doesnt accidentally revert more than it should.
                        if (MdiWindowListStrip != null && MdiWindowListStrip.MergedMenu != null && MdiWindowListStrip.MergedMenu.MdiWindowListItem != null)
                        {
                            Debug.Assert(numWindowListItems == MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count, "Calling RevertMerge modified the mdiwindowlistitem");
                        }
#endif
                    }
                    mdiControlStrip.MergedMenu = null;
                    mdiControlStrip.Dispose();
                    MdiControlStrip = null;
                }

                if (ActiveMdiChildInternal != null && maximized)
                {
                    if (ActiveMdiChildInternal.ControlBox)
                    {
                        Debug.WriteLineIf(ToolStrip.s_mdiMergeDebug.TraceVerbose, "UpdateMdiControlStrip: Detected ControlBox on ActiveMDI child, adding in MDIControlStrip.");

                        // determine if we need to add control gadgets into the MenuStrip
                        // double check GetMenu incase someone is using interop
                        IntPtr hMenu = User32.GetMenu(this);
                        if (hMenu == IntPtr.Zero)
                        {
                            MenuStrip sourceMenuStrip = ToolStripManager.GetMainMenuStrip(this);
                            if (sourceMenuStrip != null)
                            {
                                MdiControlStrip = new MdiControlStrip(ActiveMdiChildInternal);
                                Debug.WriteLineIf(ToolStrip.s_mdiMergeDebug.TraceVerbose, "UpdateMdiControlStrip: built up an MDI control strip for " + ActiveMdiChildInternal.Text + " with " + MdiControlStrip.Items.Count.ToString(CultureInfo.InvariantCulture) + " items.");
                                Debug.WriteLineIf(ToolStrip.s_mdiMergeDebug.TraceVerbose, "UpdateMdiControlStrip: merging MDI control strip into source menustrip - items before: " + sourceMenuStrip.Items.Count.ToString(CultureInfo.InvariantCulture));
                                ToolStripManager.Merge(MdiControlStrip, sourceMenuStrip);
                                Debug.WriteLineIf(ToolStrip.s_mdiMergeDebug.TraceVerbose, "UpdateMdiControlStrip: merging MDI control strip into source menustrip - items after: " + sourceMenuStrip.Items.Count.ToString(CultureInfo.InvariantCulture));
                                MdiControlStrip.MergedMenu = sourceMenuStrip;
                            }
                        }
                    }
                }
            }
            finally
            {
                formStateEx[FormStateExInUpdateMdiControlStrip] = 0;
            }
        }

        internal void UpdateMdiWindowListStrip()
        {
            if (IsMdiContainer)
            {
                if (MdiWindowListStrip != null && MdiWindowListStrip.MergedMenu != null)
                {
                    ToolStripManager.RevertMergeInternal(MdiWindowListStrip.MergedMenu, MdiWindowListStrip,/*revertMdiStuff*/true);
                }

                MenuStrip sourceMenuStrip = ToolStripManager.GetMainMenuStrip(this);
                if (sourceMenuStrip != null && sourceMenuStrip.MdiWindowListItem != null)
                {
                    if (MdiWindowListStrip is null)
                    {
                        MdiWindowListStrip = new MdiWindowListStrip();
                    }
                    int nSubItems = sourceMenuStrip.MdiWindowListItem.DropDownItems.Count;
                    bool shouldIncludeSeparator = (nSubItems > 0 &&
                        !(sourceMenuStrip.MdiWindowListItem.DropDownItems[nSubItems - 1] is ToolStripSeparator));
                    MdiWindowListStrip.PopulateItems(this, sourceMenuStrip.MdiWindowListItem, shouldIncludeSeparator);
                    ToolStripManager.Merge(MdiWindowListStrip, sourceMenuStrip);
                    MdiWindowListStrip.MergedMenu = sourceMenuStrip;
                }
            }
        }

        /// <summary>
        ///  Raises the <see cref='ResizeBegin'/>
        ///  event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResizeBegin(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                ((EventHandler)Events[EVENT_RESIZEBEGIN])?.Invoke(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='ResizeEnd'/>
        ///  event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResizeEnd(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                ((EventHandler)Events[EVENT_RESIZEEND])?.Invoke(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnStyleChanged(EventArgs e)
        {
            base.OnStyleChanged(e);
            AdjustSystemMenu();
        }

        /// <summary>
        ///  Updates the window icon.
        /// </summary>
        private unsafe void UpdateWindowIcon(bool redrawFrame)
        {
            if (IsHandleCreated)
            {
                Icon icon;

                // Preserve Win32 behavior by keeping the icon we set NULL if
                // the user hasn't specified an icon and we are a dialog frame.
                if ((FormBorderStyle == FormBorderStyle.FixedDialog && formState[FormStateIconSet] == 0) || !ShowIcon)
                {
                    icon = null;
                }
                else
                {
                    icon = Icon;
                }

                if (icon != null)
                {
                    if (smallIcon is null)
                    {
                        try
                        {
                            smallIcon = new Icon(icon, SystemInformation.SmallIconSize);
                        }
                        catch
                        {
                        }
                    }

                    if (smallIcon != null)
                    {
                        User32.SendMessageW(this, User32.WM.SETICON, (IntPtr)User32.ICON.SMALL, smallIcon.Handle);
                    }

                    User32.SendMessageW(this, User32.WM.SETICON, (IntPtr)User32.ICON.BIG, icon.Handle);
                }
                else
                {
                    User32.SendMessageW(this, User32.WM.SETICON, (IntPtr)User32.ICON.SMALL, IntPtr.Zero);
                    User32.SendMessageW(this, User32.WM.SETICON, (IntPtr)User32.ICON.BIG, IntPtr.Zero);
                }

                if (redrawFrame)
                {
                    User32.RedrawWindow(new HandleRef(this, Handle), null, IntPtr.Zero, User32.RDW.INVALIDATE | User32.RDW.FRAME);
                }
            }
        }

        /// <summary>
        ///  Updated the window state from the handle, if created.
        /// </summary>
        //
        // This function is called from all over the place, including my personal favorite,
        // WM_ERASEBKGRND.  Seems that's one of the first messages we get when a user clicks the min/max
        // button, even before WM_WINDOWPOSCHANGED.
        private void UpdateWindowState()
        {
            if (IsHandleCreated)
            {
                FormWindowState oldState = WindowState;
                User32.GetWindowPlacement(this, out User32.WINDOWPLACEMENT wp);

                switch (wp.showCmd)
                {
                    case User32.SW.NORMAL:
                    case User32.SW.RESTORE:
                    case User32.SW.SHOW:
                    case User32.SW.SHOWNA:
                    case User32.SW.SHOWNOACTIVATE:
                        if (formState[FormStateWindowState] != (int)FormWindowState.Normal)
                        {
                            formState[FormStateWindowState] = (int)FormWindowState.Normal;
                        }
                        break;
                    case User32.SW.SHOWMAXIMIZED:
                        if (formState[FormStateMdiChildMax] == 0)
                        {
                            formState[FormStateWindowState] = (int)FormWindowState.Maximized;
                        }
                        break;
                    case User32.SW.SHOWMINIMIZED:
                    case User32.SW.MINIMIZE:
                    case User32.SW.SHOWMINNOACTIVE:
                        if (formState[FormStateMdiChildMax] == 0)
                        {
                            formState[FormStateWindowState] = (int)FormWindowState.Minimized;
                        }
                        break;
                    case User32.SW.HIDE:
                    default:
                        break;
                }

                // If we used to be normal and we just became minimized or maximized,
                // stash off our current bounds so we can properly restore.
                if (oldState == FormWindowState.Normal && WindowState != FormWindowState.Normal)
                {
                    if (WindowState == FormWindowState.Minimized)
                    {
                        SuspendLayoutForMinimize();
                    }

                    restoredWindowBounds.Size = ClientSize;
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] = 1;
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] = 1;
                    restoredWindowBoundsSpecified = BoundsSpecified.Size;
                    restoredWindowBounds.Location = Location;
                    restoredWindowBoundsSpecified |= BoundsSpecified.Location;

                    // stash off restoreBounds As well...
                    restoreBounds.Size = Size;
                    restoreBounds.Location = Location;
                }

                // If we just became normal or maximized resume
                if (oldState == FormWindowState.Minimized && WindowState != FormWindowState.Minimized)
                {
                    ResumeLayoutFromMinimize();
                }

                switch (WindowState)
                {
                    case FormWindowState.Normal:
                        SetState(States.SizeLockedByOS, false);
                        break;
                    case FormWindowState.Maximized:
                    case FormWindowState.Minimized:
                        SetState(States.SizeLockedByOS, true);
                        break;
                }

                if (oldState != WindowState)
                {
                    AdjustSystemMenu();
                }
            }
        }

        /// <summary>
        ///  Validates all selectable child controls in the container, including descendants. This is
        ///  equivalent to calling ValidateChildren(ValidationConstraints.Selectable). See <see cref='ValidationConstraints.Selectable'/>
        ///  for details of exactly which child controls will be validated.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren()
        {
            return base.ValidateChildren();
        }

        /// <summary>
        ///  Validates all the child controls in the container. Exactly which controls are
        ///  validated and which controls are skipped is determined by <paramref name="validationConstraints"/>.
        /// </summary>
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren(ValidationConstraints validationConstraints)
        {
            return base.ValidateChildren(validationConstraints);
        }

        /// <summary>
        ///  WM_ACTIVATE handler
        /// </summary>
        private void WmActivate(ref Message m)
        {
            Application.FormActivated(Modal, true); // inform MsoComponentManager we're active
            Active = PARAM.LOWORD(m.WParam) != (int)User32.WA.INACTIVE;
            Application.FormActivated(Modal, Active); // inform MsoComponentManager we're active
        }

        /// <summary>
        ///  WM_ENTERSIZEMOVE handler, so that user can hook up OnResizeBegin event.
        /// </summary>
        private void WmEnterSizeMove(ref Message m)
        {
            formStateEx[FormStateExInModalSizingLoop] = 1;
            OnResizeBegin(EventArgs.Empty);
        }

        /// <summary>
        ///  WM_EXITSIZEMOVE handler, so that user can hook up OnResizeEnd event.
        /// </summary>
        private void WmExitSizeMove(ref Message m)
        {
            formStateEx[FormStateExInModalSizingLoop] = 0;
            OnResizeEnd(EventArgs.Empty);
        }

        /// <summary>
        ///  WM_CREATE handler
        /// </summary>
        private void WmCreate(ref Message m)
        {
            base.WndProc(ref m);
            var si = new Kernel32.STARTUPINFOW();
            Kernel32.GetStartupInfoW(ref si);

            // If we've been created from explorer, it may
            // force us to show up normal.  Force our current window state to
            // the specified state, unless it's _specified_ max or min
            if (TopLevel && (si.dwFlags & Kernel32.STARTF.USESHOWWINDOW) != 0)
            {
                switch ((User32.SW)si.wShowWindow)
                {
                    case User32.SW.MAXIMIZE:
                        WindowState = FormWindowState.Maximized;
                        break;
                    case User32.SW.MINIMIZE:
                        WindowState = FormWindowState.Minimized;
                        break;
                }
            }
        }

        /// <summary>
        ///  WM_CLOSE, WM_QUERYENDSESSION, and WM_ENDSESSION handler
        /// </summary>
        private void WmClose(ref Message m)
        {
            FormClosingEventArgs e = new FormClosingEventArgs(CloseReason, false);

            // Pass 1 (WM_CLOSE & WM_QUERYENDSESSION)... Closing
            if (m.Msg != (int)User32.WM.ENDSESSION)
            {
                if (Modal)
                {
                    if (dialogResult == DialogResult.None)
                    {
                        dialogResult = DialogResult.Cancel;
                    }
                    CalledClosing = false;

                    // if this comes back false, someone canceled the close.  we want
                    // to call this here so that we can get the cancel event properly,
                    // and if this is a WM_QUERYENDSESSION, appriopriately set the result
                    // based on this call.
                    //
                    // NOTE: We should also check !Validate(true) below too in the modal case,
                    // but we cannot, because we didn't to this in Everett, and doing so
                    // now would introduce a breaking change. User can always validate in the
                    // FormClosing event if they really need to.

                    e.Cancel = !CheckCloseDialog(true);
                }
                else
                {
                    e.Cancel = !Validate(true);

                    // Call OnClosing/OnFormClosing on all MDI children
                    if (IsMdiContainer)
                    {
                        FormClosingEventArgs fe = new FormClosingEventArgs(CloseReason.MdiFormClosing, e.Cancel);
                        foreach (Form mdiChild in MdiChildren)
                        {
                            if (mdiChild.IsHandleCreated)
                            {
                                mdiChild.OnClosing(fe);
                                mdiChild.OnFormClosing(fe);
                                if (fe.Cancel)
                                {
                                    // Set the Cancel property for the MDI Container's
                                    // FormClosingEventArgs, as well, so that closing the MDI container
                                    // will be cancelled.
                                    e.Cancel = true;
                                    break;
                                }
                            }
                        }
                    }

                    //Always fire OnClosing irrespectively of the validation result
                    //Pass the validation result into the EventArgs...

                    // Call OnClosing/OnFormClosing on all the forms that current form owns.
                    Form[] ownedForms = OwnedForms;
                    int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                    for (int i = ownedFormsCount - 1; i >= 0; i--)
                    {
                        FormClosingEventArgs cfe = new FormClosingEventArgs(CloseReason.FormOwnerClosing, e.Cancel);
                        if (ownedForms[i] != null)
                        {
                            //Call OnFormClosing on the child forms.
                            ownedForms[i].OnFormClosing(cfe);
                            if (cfe.Cancel)
                            {
                                // Set the cancel flag for the Owner form
                                e.Cancel = true;
                                break;
                            }
                        }
                    }

                    OnClosing(e);
                    OnFormClosing(e);
                }

                if (m.Msg == (int)User32.WM.QUERYENDSESSION)
                {
                    m.Result = (IntPtr)(e.Cancel ? 0 : 1);
                }
                else if (e.Cancel && (MdiParent != null))
                {
                    // This is the case of an MDI child close event being canceled by the user.
                    CloseReason = CloseReason.None;
                }

                if (Modal)
                {
                    return;
                }
            }
            else
            {
                e.Cancel = m.WParam == IntPtr.Zero;
            }

            // Pass 2 (WM_CLOSE & WM_ENDSESSION)... Fire closed
            // event on all mdi children and ourselves
            if (m.Msg != (int)User32.WM.QUERYENDSESSION)
            {
                FormClosedEventArgs fc;
                if (!e.Cancel)
                {
                    IsClosing = true;

                    if (IsMdiContainer)
                    {
                        fc = new FormClosedEventArgs(CloseReason.MdiFormClosing);
                        foreach (Form mdiChild in MdiChildren)
                        {
                            if (mdiChild.IsHandleCreated)
                            {
                                mdiChild.IsTopMdiWindowClosing = IsClosing;
                                mdiChild.OnClosed(fc);
                                mdiChild.OnFormClosed(fc);
                            }
                        }
                    }

                    // Call OnClosed/OnFormClosed on all the forms that current form owns.
                    Form[] ownedForms = OwnedForms;
                    int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                    for (int i = ownedFormsCount - 1; i >= 0; i--)
                    {
                        fc = new FormClosedEventArgs(CloseReason.FormOwnerClosing);
                        if (ownedForms[i] != null)
                        {
                            //Call OnClosed and OnFormClosed on the child forms.
                            ownedForms[i].OnClosed(fc);
                            ownedForms[i].OnFormClosed(fc);
                        }
                    }

                    fc = new FormClosedEventArgs(CloseReason);
                    OnClosed(fc);
                    OnFormClosed(fc);

                    Dispose();
                }
            }
        }

        /// <summary>
        ///  WM_ENTERMENULOOP handler
        /// </summary>
        private void WmEnterMenuLoop(ref Message m)
        {
            OnMenuStart(EventArgs.Empty);
            base.WndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_ERASEBKGND message
        /// </summary>
        private void WmEraseBkgnd(ref Message m)
        {
            UpdateWindowState();
            base.WndProc(ref m);
        }

        /// <summary>
        ///  WM_EXITMENULOOP handler
        /// </summary>
        private void WmExitMenuLoop(ref Message m)
        {
            OnMenuComplete(EventArgs.Empty);
            base.WndProc(ref m);
        }

        /// <summary>
        ///  WM_GETMINMAXINFO handler
        /// </summary>
        private void WmGetMinMaxInfo(ref Message m)
        {
            // Form should gracefully stop at the minimum preferred size.
            // When we're set to AutoSize true, we should take a look at minAutoSize - which is snapped in onlayout.
            // as the form contracts, we should not let it size past here as we're just going to readjust the size
            // back to it later.
            Size minTrack = (AutoSize && formStateEx[FormStateExInModalSizingLoop] == 1) ? LayoutUtils.UnionSizes(minAutoSize, MinimumSize) : MinimumSize;

            Size maxTrack = MaximumSize;
            Rectangle maximizedBounds = MaximizedBounds;

            if (!minTrack.IsEmpty || !maxTrack.IsEmpty || !maximizedBounds.IsEmpty)
            {
                WmGetMinMaxInfoHelper(ref m, minTrack, maxTrack, maximizedBounds);
            }

            if (IsMdiChild)
            {
                base.WndProc(ref m);
            }
        }

        private unsafe void WmGetMinMaxInfoHelper(ref Message m, Size minTrack, Size maxTrack, Rectangle maximizedBounds)
        {
            User32.MINMAXINFO* mmi = (User32.MINMAXINFO*)m.LParam;
            if (!minTrack.IsEmpty)
            {
                mmi->ptMinTrackSize.X = minTrack.Width;
                mmi->ptMinTrackSize.Y = minTrack.Height;

                // When the MinTrackSize is set to a value larger than the screen
                // size but the MaxTrackSize is not set to a value equal to or greater than the
                // MinTrackSize and the user attempts to "grab" a resizing handle, Windows makes
                // the window move a distance equal to either the width when attempting to resize
                // horizontally or the height of the window when attempting to resize vertically.
                // So, the workaround to prevent this problem is to set the MaxTrackSize to something
                // whenever the MinTrackSize is set to a value larger than the respective dimension
                // of the virtual screen.
                if (maxTrack.IsEmpty)
                {
                    // Only set the max track size dimensions if the min track size dimensions
                    // are larger than the VirtualScreen dimensions.
                    Size virtualScreen = SystemInformation.VirtualScreen.Size;
                    if (minTrack.Height > virtualScreen.Height)
                    {
                        mmi->ptMaxTrackSize.Y = int.MaxValue;
                    }
                    if (minTrack.Width > virtualScreen.Width)
                    {
                        mmi->ptMaxTrackSize.X = int.MaxValue;
                    }
                }
            }

            if (!maxTrack.IsEmpty)
            {
                // Is the specified MaxTrackSize smaller than the smallest allowable Window size?
                Size minTrackWindowSize = SystemInformation.MinWindowTrackSize;
                mmi->ptMaxTrackSize.X = Math.Max(maxTrack.Width, minTrackWindowSize.Width);
                mmi->ptMaxTrackSize.Y = Math.Max(maxTrack.Height, minTrackWindowSize.Height);
            }

            if (!maximizedBounds.IsEmpty)
            {
                mmi->ptMaxPosition.X = maximizedBounds.X;
                mmi->ptMaxPosition.Y = maximizedBounds.Y;
                mmi->ptMaxSize.X = maximizedBounds.Width;
                mmi->ptMaxSize.Y = maximizedBounds.Height;
            }

            m.Result = IntPtr.Zero;
        }

        /// <summary>
        ///  WM_MDIACTIVATE handler
        /// </summary>
        private void WmMdiActivate(ref Message m)
        {
            base.WndProc(ref m);
            Debug.Assert(Properties.GetObject(PropFormMdiParent) != null, "how is formMdiParent null?");
            Debug.Assert(IsHandleCreated, "how is handle 0?");

            Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);

            if (formMdiParent != null)
            {
                // This message is propagated twice by the MDIClient window. Once to the
                // window being deactivated and once to the window being activated.
                if (Handle == m.WParam)
                {
                    formMdiParent.DeactivateMdiChild();
                }
                else if (Handle == m.LParam)
                {
                    formMdiParent.ActivateMdiChild(this);
                }
            }
        }

        private void WmNcButtonDown(ref Message m)
        {
            if (IsMdiChild)
            {
                Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                if (formMdiParent.ActiveMdiChildInternal == this)
                {
                    if (ActiveControl != null && !ActiveControl.ContainsFocus)
                    {
                        InnerMostActiveContainerControl.FocusActiveControlInternal();
                    }
                }
            }
            base.WndProc(ref m);
        }

        /// <summary>
        ///  WM_NCDESTROY handler
        /// </summary>
        private void WmNCDestroy(ref Message m)
        {
            base.WndProc(ref m);

            // Destroy the owner window, if we created one.  We
            // cannot do this in OnHandleDestroyed, because at
            // that point our handle is not actually destroyed so
            // destroying our parent actually causes a recursive
            // WM_DESTROY.
            if (ownerWindow != null)
            {
                ownerWindow.DestroyHandle();
                ownerWindow = null;
            }

            if (Modal && dialogResult == DialogResult.None)
            {
                DialogResult = DialogResult.Cancel;
            }
        }

        /// <summary>
        ///  WM_NCHITTEST handler
        /// </summary>
        private void WmNCHitTest(ref Message m)
        {
            if (formState[FormStateRenderSizeGrip] != 0)
            {
                int x = PARAM.SignedLOWORD(m.LParam);
                int y = PARAM.SignedHIWORD(m.LParam);

                // Convert to client coordinates
                var pt = new Point(x, y);
                User32.ScreenToClient(new HandleRef(this, Handle), ref pt);

                Size clientSize = ClientSize;

                // If the grip is not fully visible the grip area could overlap with the system control box; we need to disable
                // the grip area in this case not to get in the way of the control box.  We only need to check for the client's
                // height since the window width will be at least the size of the control box which is always bigger than the
                // grip width.
                if (pt.X >= (clientSize.Width - SizeGripSize) &&
                    pt.Y >= (clientSize.Height - SizeGripSize) &&
                    clientSize.Height >= SizeGripSize)
                {
                    m.Result = (IntPtr)(IsMirrored ? User32.HT.BOTTOMLEFT : User32.HT.BOTTOMRIGHT);
                    return;
                }
            }

            base.WndProc(ref m);

            // If we got any of the "edge" hits (bottom, top, topleft, etc),
            // and we're AutoSizeMode.GrowAndShrink, return non-resizable border
            // The edge values are the 8 values from HTLEFT (10) to HTBOTTOMRIGHT (17).
            if (AutoSizeMode == AutoSizeMode.GrowAndShrink)
            {
                int result = unchecked((int)(long)m.Result);
                if (result >= (int)User32.HT.LEFT &&
                    result <= (int)User32.HT.BOTTOMRIGHT)
                {
                    m.Result = (IntPtr)User32.HT.BORDER;
                }
            }
        }

        /// <summary>
        ///  WM_SHOWWINDOW handler
        /// </summary>
        private void WmShowWindow(ref Message m)
        {
            formState[FormStateSWCalled] = 1;
            base.WndProc(ref m);
        }

        /// <summary>
        ///  WM_SYSCOMMAND handler
        /// </summary>
        private void WmSysCommand(ref Message m)
        {
            bool callDefault = true;

            User32.SC sc = (User32.SC)(PARAM.LOWORD(m.WParam) & 0xFFF0);
            switch (sc)
            {
                case User32.SC.CLOSE:
                    CloseReason = CloseReason.UserClosing;
                    if (IsMdiChild && !ControlBox)
                    {
                        callDefault = false;
                    }
                    break;
                case User32.SC.KEYMENU:
                    if (IsMdiChild && !ControlBox)
                    {
                        callDefault = false;
                    }
                    break;
                case User32.SC.SIZE:
                case User32.SC.MOVE:
                    // Set this before WM_ENTERSIZELOOP because WM_GETMINMAXINFO can be called before WM_ENTERSIZELOOP.
                    formStateEx[FormStateExInModalSizingLoop] = 1;
                    break;
                case User32.SC.CONTEXTHELP:
                    CancelEventArgs e = new CancelEventArgs(false);
                    OnHelpButtonClicked(e);
                    if (e.Cancel == true)
                    {
                        callDefault = false;
                    }
                    break;
            }

            if (Command.DispatchID(PARAM.LOWORD(m.WParam)))
            {
                callDefault = false;
            }

            if (callDefault)
            {
                base.WndProc(ref m);
            }
        }

        /// <summary>
        ///  WM_SIZE handler
        /// </summary>
        private void WmSize(ref Message m)
        {
            // If this is an MDI parent, don't pass WM_SIZE to the default
            // window proc. We handle resizing the MDIClient window ourselves
            // (using ControlDock.FILL).
            //
            if (ctlClient is null)
            {
                base.WndProc(ref m);
                if (MdiControlStrip is null && MdiParentInternal != null && MdiParentInternal.ActiveMdiChildInternal == this)
                {
                    int wParam = m.WParam.ToInt32();
                    MdiParentInternal.UpdateMdiControlStrip(wParam == (int)User32.WINDOW_SIZE.MAXIMIZED);
                }
            }
        }

        /// <summary>
        ///  WM_WINDOWPOSCHANGED handler
        /// </summary>
        private void WmWindowPosChanged(ref Message m)
        {
            //           We must update the windowState, because resize is fired
            //           from here... (in Control)
            UpdateWindowState();
            base.WndProc(ref m);

            RestoreWindowBoundsIfNecessary();
        }

        /// <summary>
        ///  Base wndProc encapsulation.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void WndProc(ref Message m)
        {
            switch ((User32.WM)m.Msg)
            {
                case User32.WM.NCACTIVATE:
                    base.WndProc(ref m);
                    break;
                case User32.WM.NCLBUTTONDOWN:
                case User32.WM.NCRBUTTONDOWN:
                case User32.WM.NCMBUTTONDOWN:
                case User32.WM.NCXBUTTONDOWN:
                    WmNcButtonDown(ref m);
                    break;
                case User32.WM.ACTIVATE:
                    WmActivate(ref m);
                    break;
                case User32.WM.MDIACTIVATE:
                    WmMdiActivate(ref m);
                    break;
                case User32.WM.CLOSE:
                    if (CloseReason == CloseReason.None)
                    {
                        CloseReason = CloseReason.TaskManagerClosing;
                    }
                    WmClose(ref m);
                    break;

                case User32.WM.QUERYENDSESSION:
                case User32.WM.ENDSESSION:
                    CloseReason = CloseReason.WindowsShutDown;
                    WmClose(ref m);
                    break;
                case User32.WM.ENTERSIZEMOVE:
                    WmEnterSizeMove(ref m);
                    DefWndProc(ref m);
                    break;
                case User32.WM.EXITSIZEMOVE:
                    WmExitSizeMove(ref m);
                    DefWndProc(ref m);
                    break;
                case User32.WM.CREATE:
                    WmCreate(ref m);
                    break;
                case User32.WM.ERASEBKGND:
                    WmEraseBkgnd(ref m);
                    break;

                case User32.WM.NCDESTROY:
                    WmNCDestroy(ref m);
                    break;
                case User32.WM.NCHITTEST:
                    WmNCHitTest(ref m);
                    break;
                case User32.WM.SHOWWINDOW:
                    WmShowWindow(ref m);
                    break;
                case User32.WM.SIZE:
                    WmSize(ref m);
                    break;
                case User32.WM.SYSCOMMAND:
                    WmSysCommand(ref m);
                    break;
                case User32.WM.GETMINMAXINFO:
                    WmGetMinMaxInfo(ref m);
                    break;
                case User32.WM.WINDOWPOSCHANGED:
                    WmWindowPosChanged(ref m);
                    break;
                //case User32.WM.WINDOWPOSCHANGING:
                //    WmWindowPosChanging(ref m);
                //    break;
                case User32.WM.ENTERMENULOOP:
                    WmEnterMenuLoop(ref m);
                    break;
                case User32.WM.EXITMENULOOP:
                    WmExitMenuLoop(ref m);
                    break;
                case User32.WM.CAPTURECHANGED:
                    base.WndProc(ref m);

                    // This is a work-around for the Win32 scroll bar; it doesn't release
                    // it's capture in response to a CAPTURECHANGED message, so we force
                    // capture away if no button is down.

                    if (Capture && MouseButtons == MouseButtons.None)
                    {
                        Capture = false;
                    }
                    break;
                case User32.WM.GETDPISCALEDSIZE:
                    Debug.Assert(PARAM.SignedLOWORD(m.WParam) == PARAM.SignedHIWORD(m.WParam), "Non-square pixels!");
                    WmGetDpiScaledSize(ref m);
                    break;
                case User32.WM.DPICHANGED:
                    WmDpiChanged(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Represents a collection of controls on the form.
        /// </summary>
        public new class ControlCollection : Control.ControlCollection
        {
            private readonly Form owner;

            /*C#r:protected*/

            /// <summary>
            ///  Initializes a new instance of the ControlCollection class.
            /// </summary>
            public ControlCollection(Form owner)
            : base(owner)
            {
                this.owner = owner;
            }

            /// <summary>
            ///  Adds a control
            ///  to the form.
            /// </summary>
            public override void Add(Control value)
            {
                if (value is MdiClient && owner.ctlClient is null)
                {
                    if (!owner.TopLevel && !owner.DesignMode)
                    {
                        throw new ArgumentException(SR.MDIContainerMustBeTopLevel, nameof(value));
                    }
                    owner.AutoScroll = false;
                    if (owner.IsMdiChild)
                    {
                        throw new ArgumentException(SR.FormMDIParentAndChild, nameof(value));
                    }
                    owner.ctlClient = (MdiClient)value;
                }

                // make sure we don't add a form that has a valid mdi parent
                //
                if (value is Form && ((Form)value).MdiParentInternal != null)
                {
                    throw new ArgumentException(SR.FormMDIParentCannotAdd, nameof(value));
                }

                base.Add(value);

                if (owner.ctlClient != null)
                {
                    owner.ctlClient.SendToBack();
                }
            }

            /// <summary>
            ///  Removes a control from the form.
            /// </summary>
            public override void Remove(Control value)
            {
                if (value == owner.ctlClient)
                {
                    owner.ctlClient = null;
                }
                base.Remove(value);
            }
        }

        // Class used to temporarily reset the owners of windows owned by this Form
        // before its handle recreation, then setting them back to the new handle
        // after handle recreation
        private class EnumThreadWindowsCallback
        {
            private List<HandleRef> ownedWindows;

            private readonly IntPtr _formHandle;

            internal EnumThreadWindowsCallback(IntPtr formHandle)
            {
                this._formHandle = formHandle;
            }

            internal BOOL Callback(IntPtr hWnd)
            {
                HandleRef hRef = new HandleRef(null, hWnd);
                IntPtr parent = User32.GetWindowLong(hRef, User32.GWL.HWNDPARENT);
                if (parent == _formHandle)
                {
                    // Enumerated window is owned by this Form.
                    // Store it in a list for further treatment.
                    if (ownedWindows is null)
                    {
                        ownedWindows = new List<HandleRef>();
                    }
                    ownedWindows.Add(hRef);
                }
                return BOOL.TRUE;
            }

            // Resets the owner of all the windows owned by this Form before handle recreation.
            internal void ResetOwners()
            {
                if (ownedWindows != null)
                {
                    foreach (HandleRef hRef in ownedWindows)
                    {
                        User32.SetWindowLong(hRef, User32.GWL.HWNDPARENT, NativeMethods.NullHandleRef);
                    }
                }
            }

            // Sets the owner of the windows back to this Form after its handle recreation.
            internal void SetOwners(HandleRef hRefOwner)
            {
                if (ownedWindows != null)
                {
                    foreach (HandleRef hRef in ownedWindows)
                    {
                        User32.SetWindowLong(hRef, User32.GWL.HWNDPARENT, hRefOwner);
                    }
                }
            }
        }
    }
}
