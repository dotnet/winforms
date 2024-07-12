// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.System.Threading;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.Graphics.Dwm;

namespace System.Windows.Forms;

/// <summary>
///  Represents a window or dialog box that makes up an application's user interface.
/// </summary>
[ToolboxItemFilter("System.Windows.Forms.Control.TopLevel")]
[ToolboxItem(false)]
[DesignTimeVisible(false)]
[Designer($"System.Windows.Forms.Design.FormDocumentDesigner, {AssemblyRef.SystemDesign}", typeof(IRootDesigner))]
[DefaultEvent(nameof(Load))]
[InitializationEvent(nameof(Load))]
[DesignerCategory("Form")]
public partial class Form : ContainerControl
{
    private static readonly object s_activatedEvent = new();
    private static readonly object s_closingEvent = new();
    private static readonly object s_closedEvent = new();
    private static readonly object s_formClosingEvent = new();
    private static readonly object s_formClosedEvent = new();
    private static readonly object s_deactivateEvent = new();
    private static readonly object s_loadEvent = new();
    private static readonly object s_mdiChildActivateEvent = new();
    private static readonly object s_inputLanguageChangeEvent = new();
    private static readonly object s_inputLanguageChangeRequestEvent = new();
    private static readonly object s_menuStartEvent = new();
    private static readonly object s_menuCompleteEvent = new();
    private static readonly object s_maximumSizeChangedEvent = new();
    private static readonly object s_minimumSizeChangedEvent = new();
    private static readonly object s_helpButtonChangedEvent = new();
    private static readonly object s_shownEvent = new();
    private static readonly object s_resizeBeginEvent = new();
    private static readonly object s_resizeEndEvent = new();
    private static readonly object s_rightToLeftLayoutChangedEvent = new();
    private static readonly object s_dpiChangedEvent = new();

    //
    // The following flags should be used with formState[..] not formStateEx[..]
    // Don't add any more sections to this vector, it is already full.
    //
    private static readonly BitVector32.Section s_formStateAllowTransparency = BitVector32.CreateSection(1);
    private static readonly BitVector32.Section s_formStateBorderStyle = BitVector32.CreateSection(6, s_formStateAllowTransparency);
    private static readonly BitVector32.Section s_formStateTaskBar = BitVector32.CreateSection(1, s_formStateBorderStyle);
    private static readonly BitVector32.Section s_formStateControlBox = BitVector32.CreateSection(1, s_formStateTaskBar);
    private static readonly BitVector32.Section s_formStateKeyPreview = BitVector32.CreateSection(1, s_formStateControlBox);
    private static readonly BitVector32.Section s_formStateLayered = BitVector32.CreateSection(1, s_formStateKeyPreview);
    private static readonly BitVector32.Section s_formStateMaximizeBox = BitVector32.CreateSection(1, s_formStateLayered);
    private static readonly BitVector32.Section s_formStateMinimizeBox = BitVector32.CreateSection(1, s_formStateMaximizeBox);
    private static readonly BitVector32.Section s_formStateHelpButton = BitVector32.CreateSection(1, s_formStateMinimizeBox);
    private static readonly BitVector32.Section s_formStateStartPos = BitVector32.CreateSection(4, s_formStateHelpButton);
    private static readonly BitVector32.Section s_formStateWindowState = BitVector32.CreateSection(2, s_formStateStartPos);
    private static readonly BitVector32.Section s_formStateShowWindowOnCreate = BitVector32.CreateSection(1, s_formStateWindowState);
    private static readonly BitVector32.Section s_formStateAutoScaling = BitVector32.CreateSection(1, s_formStateShowWindowOnCreate);
    private static readonly BitVector32.Section s_formStateSetClientSize = BitVector32.CreateSection(1, s_formStateAutoScaling);
    private static readonly BitVector32.Section s_formStateTopMost = BitVector32.CreateSection(1, s_formStateSetClientSize);
    private static readonly BitVector32.Section s_formStateSWCalled = BitVector32.CreateSection(1, s_formStateTopMost);
    private static readonly BitVector32.Section s_formStateMdiChildMax = BitVector32.CreateSection(1, s_formStateSWCalled);
    private static readonly BitVector32.Section s_formStateRenderSizeGrip = BitVector32.CreateSection(1, s_formStateMdiChildMax);
    private static readonly BitVector32.Section s_formStateSizeGripStyle = BitVector32.CreateSection(2, s_formStateRenderSizeGrip);
    private static readonly BitVector32.Section s_formStateIsWindowActivated = BitVector32.CreateSection(1, s_formStateSizeGripStyle);
    private static readonly BitVector32.Section s_formStateIsTextEmpty = BitVector32.CreateSection(1, s_formStateIsWindowActivated);
    private static readonly BitVector32.Section s_formStateIsActive = BitVector32.CreateSection(1, s_formStateIsTextEmpty);
    private static readonly BitVector32.Section s_formStateIconSet = BitVector32.CreateSection(1, s_formStateIsActive);

    // The following flags should be used with formStateEx[...] not formState[..]
    private static readonly BitVector32.Section s_formStateExCalledClosing = BitVector32.CreateSection(1);
    private static readonly BitVector32.Section s_formStateExUpdateMenuHandlesSuspendCount = BitVector32.CreateSection(8, s_formStateExCalledClosing);
    private static readonly BitVector32.Section s_formStateExUpdateMenuHandlesDeferred = BitVector32.CreateSection(1, s_formStateExUpdateMenuHandlesSuspendCount);
    private static readonly BitVector32.Section s_formStateExUseMdiChildProc = BitVector32.CreateSection(1, s_formStateExUpdateMenuHandlesDeferred);
    private static readonly BitVector32.Section s_formStateExCalledOnLoad = BitVector32.CreateSection(1, s_formStateExUseMdiChildProc);
    private static readonly BitVector32.Section s_formStateExCalledMakeVisible = BitVector32.CreateSection(1, s_formStateExCalledOnLoad);
    private static readonly BitVector32.Section s_formStateExCalledCreateControl = BitVector32.CreateSection(1, s_formStateExCalledMakeVisible);
    private static readonly BitVector32.Section s_formStateExAutoSize = BitVector32.CreateSection(1, s_formStateExCalledCreateControl);
    private static readonly BitVector32.Section s_formStateExInUpdateMdiControlStrip = BitVector32.CreateSection(1, s_formStateExAutoSize);
    private static readonly BitVector32.Section s_formStateExShowIcon = BitVector32.CreateSection(1, s_formStateExInUpdateMdiControlStrip);
    private static readonly BitVector32.Section s_formStateExMnemonicProcessed = BitVector32.CreateSection(1, s_formStateExShowIcon);
    private static readonly BitVector32.Section s_formStateExInScale = BitVector32.CreateSection(1, s_formStateExMnemonicProcessed);
    private static readonly BitVector32.Section s_formStateExInModalSizingLoop = BitVector32.CreateSection(1, s_formStateExInScale);
    private static readonly BitVector32.Section s_formStateExSettingAutoScale = BitVector32.CreateSection(1, s_formStateExInModalSizingLoop);
    private static readonly BitVector32.Section s_formStateExWindowBoundsWidthIsClientSize = BitVector32.CreateSection(1, s_formStateExSettingAutoScale);
    private static readonly BitVector32.Section s_formStateExWindowBoundsHeightIsClientSize = BitVector32.CreateSection(1, s_formStateExWindowBoundsWidthIsClientSize);
    private static readonly BitVector32.Section s_formStateExWindowClosing = BitVector32.CreateSection(1, s_formStateExWindowBoundsHeightIsClientSize);

    private const int SizeGripSize = 16;

    private static Icon? s_defaultIcon;
    private static readonly Lock s_internalSyncObject = new();

    // Property store keys for properties.  The property store allocates most efficiently
    // in groups of four, so we try to lump properties in groups of four based on how
    // likely they are going to be used in a group.

    private static readonly int s_propAcceptButton = PropertyStore.CreateKey();
    private static readonly int s_propCancelButton = PropertyStore.CreateKey();
    private static readonly int s_propDefaultButton = PropertyStore.CreateKey();
    private static readonly int s_propDialogOwner = PropertyStore.CreateKey();

    private static readonly int s_propOwner = PropertyStore.CreateKey();
    private static readonly int s_propOwnedForms = PropertyStore.CreateKey();
    private static readonly int s_propMaximizedBounds = PropertyStore.CreateKey();
    private static readonly int s_propOwnedFormsCount = PropertyStore.CreateKey();

    private static readonly int s_propMinTrackSizeWidth = PropertyStore.CreateKey();
    private static readonly int s_propMinTrackSizeHeight = PropertyStore.CreateKey();
    private static readonly int s_propMaxTrackSizeWidth = PropertyStore.CreateKey();
    private static readonly int s_propMaxTrackSizeHeight = PropertyStore.CreateKey();

    private static readonly int s_propFormMdiParent = PropertyStore.CreateKey();
    private static readonly int s_propActiveMdiChild = PropertyStore.CreateKey();
    private static readonly int s_propFormerlyActiveMdiChild = PropertyStore.CreateKey();
    private static readonly int s_propMdiChildFocusable = PropertyStore.CreateKey();

    private static readonly int s_propDummyMdiMenu = PropertyStore.CreateKey();
    private static readonly int s_propMainMenuStrip = PropertyStore.CreateKey();
    private static readonly int s_propMdiWindowListStrip = PropertyStore.CreateKey();
    private static readonly int s_propMdiControlStrip = PropertyStore.CreateKey();

    private static readonly int s_propOpacity = PropertyStore.CreateKey();
    private static readonly int s_propTransparencyKey = PropertyStore.CreateKey();

    // Form per instance members
    // Note: Do not add anything to this list unless absolutely necessary.

    private BitVector32 _formState = new(0x21338);   // magic value... all the defaults... see the ctor for details...
    private BitVector32 _formStateEx;

    private Icon? _icon;
    private Icon? _smallIcon;
    private Size _autoScaleBaseSize = Size.Empty;
    private Size _minAutoSize = Size.Empty;
    private Rectangle _restoredWindowBounds = new(-1, -1, -1, -1);
    private BoundsSpecified _restoredWindowBoundsSpecified;
    private DialogResult _dialogResult;
    private MdiClient? _ctlClient;
    private NativeWindow? _ownerWindow;
    private bool _rightToLeftLayout;

    private Rectangle _restoreBounds = new(-1, -1, -1, -1);
    private CloseReason _closeReason = CloseReason.None;

    private VisualStyleRenderer? _sizeGripRenderer;

    // Cache Form's size for the DPI. When Form is moved between the monitors with different DPI settings, we use
    // cached values to set the size matching the DPI on the Form instead of recalculating the size again. This help
    // preventing rounding error in size calculations with float DPI factor and rounding it to nearest integer.
    private Dictionary<int, Size>? _dpiFormSizes;
    private bool _processingDpiChanged;
    private bool _inRecreateHandle;

    public enum WindowCornerPreference
    {
        Default = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT,
        DoNotRound = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND,
        Round = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND,
        RoundSmall = DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL
    }

    /// <summary>
    ///  Initializes a new instance of the <see cref="Form"/> class.
    /// </summary>
    public Form() : base()
    {
        // Assert section.
        Debug.Assert(_formState[s_formStateAllowTransparency] == 0, "Failed to set formState[FormStateAllowTransparency]");
        Debug.Assert(_formState[s_formStateBorderStyle] == (int)FormBorderStyle.Sizable, "Failed to set formState[FormStateBorderStyle]");
        Debug.Assert(_formState[s_formStateTaskBar] == 1, "Failed to set formState[FormStateTaskBar]");
        Debug.Assert(_formState[s_formStateControlBox] == 1, "Failed to set formState[FormStateControlBox]");
        Debug.Assert(_formState[s_formStateKeyPreview] == 0, "Failed to set formState[FormStateKeyPreview]");
        Debug.Assert(_formState[s_formStateLayered] == 0, "Failed to set formState[FormStateLayered]");
        Debug.Assert(_formState[s_formStateMaximizeBox] == 1, "Failed to set formState[FormStateMaximizeBox]");
        Debug.Assert(_formState[s_formStateMinimizeBox] == 1, "Failed to set formState[FormStateMinimizeBox]");
        Debug.Assert(_formState[s_formStateHelpButton] == 0, "Failed to set formState[FormStateHelpButton]");
        Debug.Assert(_formState[s_formStateStartPos] == (int)FormStartPosition.WindowsDefaultLocation, "Failed to set formState[FormStateStartPos]");
        Debug.Assert(_formState[s_formStateWindowState] == (int)FormWindowState.Normal, "Failed to set formState[FormStateWindowState]");
        Debug.Assert(_formState[s_formStateShowWindowOnCreate] == 0, "Failed to set formState[FormStateShowWindowOnCreate]");
        Debug.Assert(_formState[s_formStateAutoScaling] == 1, "Failed to set formState[FormStateAutoScaling]");
        Debug.Assert(_formState[s_formStateSetClientSize] == 0, "Failed to set formState[FormStateSetClientSize]");
        Debug.Assert(_formState[s_formStateTopMost] == 0, "Failed to set formState[FormStateTopMost]");
        Debug.Assert(_formState[s_formStateSWCalled] == 0, "Failed to set formState[FormStateSWCalled]");
        Debug.Assert(_formState[s_formStateMdiChildMax] == 0, "Failed to set formState[FormStateMdiChildMax]");
        Debug.Assert(_formState[s_formStateRenderSizeGrip] == 0, "Failed to set formState[FormStateRenderSizeGrip]");
        Debug.Assert(_formState[s_formStateSizeGripStyle] == 0, "Failed to set formState[FormStateSizeGripStyle]");
        Debug.Assert(_formState[s_formStateIsWindowActivated] == 0, "Failed to set formState[FormStateIsWindowActivated]");
        Debug.Assert(_formState[s_formStateIsTextEmpty] == 0, "Failed to set formState[FormStateIsTextEmpty]");
        Debug.Assert(_formState[s_formStateIsActive] == 0, "Failed to set formState[FormStateIsActive]");
        Debug.Assert(_formState[s_formStateIconSet] == 0, "Failed to set formState[FormStateIconSet]");

        _formStateEx[s_formStateExShowIcon] = 1;

        SetState(States.Visible, false);
        SetState(States.TopLevel, true);
    }

    /// <summary>
    ///  Indicates the <see cref="Button"/> control on the form that is clicked when
    ///  the user presses the ENTER key.
    /// </summary>
    [DefaultValue(null)]
    [SRDescription(nameof(SR.FormAcceptButtonDescr))]
    public IButtonControl? AcceptButton
    {
        get
        {
            return (IButtonControl?)Properties.GetObject(s_propAcceptButton);
        }
        set
        {
            if (AcceptButton != value)
            {
                Properties.SetObject(s_propAcceptButton, value);
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
            Form? parentForm = ParentForm;
            if (parentForm is null)
            {
                return _formState[s_formStateIsActive] != 0;
            }

            return parentForm.ActiveControl == this && parentForm.Active;
        }
        set
        {
            if ((_formState[s_formStateIsActive] != 0) != value)
            {
                if (value)
                {
                    if (!CanRecreateHandle())
                    {
                        return;
                    }
                }

                _formState[s_formStateIsActive] = value ? 1 : 0;

                if (value)
                {
                    _formState[s_formStateIsWindowActivated] = 1;

                    // Check if validation has been canceled to avoid raising Validation event multiple times.
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
                    _formState[s_formStateIsWindowActivated] = 0;
                    OnDeactivate(EventArgs.Empty);
                }
            }
        }
    }

    /// <summary>
    ///  Gets the currently active form for this application.
    /// </summary>
    public static Form? ActiveForm => FromHandle(PInvokeCore.GetForegroundWindow()) as Form;

    /// <summary>
    ///
    ///  Gets the currently active multiple document interface (MDI) child window.
    ///  Note: Don't use this property internally, use ActiveMdiChildInternal instead (see comments below).
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.FormActiveMDIChildDescr))]
    public Form? ActiveMdiChild
    {
        get
        {
            Form? mdiChild = ActiveMdiChildInternal;

            // We keep the active mdi child in the cached in the property store; when changing its value
            // (due to a change to one of the following properties/methods: Visible, Enabled, Active, Show/Hide,
            // Focus() or as the  result of WM_SETFOCUS/WM_ACTIVATE/WM_MDIACTIVATE) we temporarily set it to null
            // (to properly handle menu merging among other things) rendering the cache out-of-date; the problem
            // arises when the user has an event handler that is raised during this process; in that case we ask
            // Windows for it (see ActiveMdiChildFromWindows).

            if (mdiChild is null)
            {
                // If this.MdiClient is not null it means this.IsMdiContainer == true.
                if (_ctlClient is not null && _ctlClient.IsHandleCreated)
                {
                    IntPtr hwnd = PInvoke.SendMessage(_ctlClient, PInvoke.WM_MDIGETACTIVE);
                    mdiChild = FromHandle(hwnd) as Form;
                }
            }

            if (mdiChild is not null && mdiChild.Visible && mdiChild.Enabled)
            {
                return mdiChild;
            }

            return null;
        }
    }

    /// <summary>
    ///  Property to be used internally.  See comments a on ActiveMdiChild property.
    /// </summary>
    internal Form? ActiveMdiChildInternal
    {
        get
        {
            return (Form?)Properties.GetObject(s_propActiveMdiChild);
        }

        set
        {
            Properties.SetObject(s_propActiveMdiChild, value);
        }
    }

    // we don't repaint the mdi child that used to be active any more.  We used to do this in Activated, but no
    // longer do because of added event Deactivate.
    private Form? FormerlyActiveMdiChild
    {
        get
        {
            return (Form?)Properties.GetObject(s_propFormerlyActiveMdiChild);
        }

        set
        {
            Properties.SetObject(s_propFormerlyActiveMdiChild, value);
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
            return _formState[s_formStateAllowTransparency] != 0;
        }
        set
        {
            if (value != (_formState[s_formStateAllowTransparency] != 0))
            {
                _formState[s_formStateAllowTransparency] = (value ? 1 : 0);

                _formState[s_formStateLayered] = _formState[s_formStateAllowTransparency];

                UpdateStyles();

                if (!value)
                {
                    if (Properties.ContainsObject(s_propOpacity))
                    {
                        Properties.SetObject(s_propOpacity, 1.0f);
                    }

                    if (Properties.ContainsObject(s_propTransparencyKey))
                    {
                        Properties.SetObject(s_propTransparencyKey, Color.Empty);
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
    Obsolete("This property has been deprecated. Use the AutoScaleMode property instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool AutoScale
    {
        get
        {
            return _formState[s_formStateAutoScaling] != 0;
        }

        set
        {
            _formStateEx[s_formStateExSettingAutoScale] = 1;
            try
            {
                if (value)
                {
                    _formState[s_formStateAutoScaling] = 1;

                    // if someone insists on auto scaling,
                    // force the new property back to none so they
                    // don't compete.
                    AutoScaleMode = AutoScaleMode.None;
                }
                else
                {
                    _formState[s_formStateAutoScaling] = 0;
                }
            }
            finally
            {
                _formStateEx[s_formStateExSettingAutoScale] = 0;
            }
        }
    }

    // Our STRONG recommendation to customers is to upgrade to AutoScaleDimensions
    // however, since this is generated by default in Everett, and there's not a 1:1 mapping of
    // the old to the new, we are un-obsoleting the setter for AutoScaleBaseSize only.

    /// <summary>
    ///  The base size used for autoscaling. The AutoScaleBaseSize is used internally to determine how much to scale
    ///  the form when AutoScaling is used.
    /// </summary>
    [Localizable(true)]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public virtual Size AutoScaleBaseSize
    {
        get
        {
            if (_autoScaleBaseSize.IsEmpty)
            {
#pragma warning disable CS0618 // Type or member is obsolete - compat
                SizeF real = GetAutoScaleSize(Font);
#pragma warning restore CS0618
                return new Size((int)Math.Round(real.Width), (int)Math.Round(real.Height));
            }

            return _autoScaleBaseSize;
        }

        set
        {
            // Only allow the set when not in designmode, this prevents us from
            // preserving an old value.  The form design should prevent this for
            // us by shadowing this property, so we just assert that the designer
            // is doing its job.
            //
            Debug.Assert(!DesignMode, "Form designer should not allow base size set in design mode.");
            _autoScaleBaseSize = value;
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
        get { return _formStateEx[s_formStateExAutoSize] != 0; }
        set
        {
            if (value != AutoSize)
            {
                _formStateEx[s_formStateExAutoSize] = value ? 1 : 0;
                if (!AutoSize)
                {
                    _minAutoSize = Size.Empty;
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
    public new event EventHandler? AutoSizeChanged
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
            SourceGenerated.EnumValidator.Validate(value);

            if (GetAutoSizeMode() != value)
            {
                SetAutoSizeMode(value);
                Control toLayout = DesignMode || ParentInternal is null ? this : ParentInternal;

                if (toLayout is not null)
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
    public new event EventHandler? AutoValidateChanged
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
            return _formStateEx[s_formStateExCalledClosing] != 0;
        }
        set
        {
            _formStateEx[s_formStateExCalledClosing] = (value ? 1 : 0);
        }
    }

    private bool CalledCreateControl
    {
        get
        {
            return _formStateEx[s_formStateExCalledCreateControl] != 0;
        }
        set
        {
            _formStateEx[s_formStateExCalledCreateControl] = (value ? 1 : 0);
        }
    }

    private bool CalledMakeVisible
    {
        get
        {
            return _formStateEx[s_formStateExCalledMakeVisible] != 0;
        }
        set
        {
            _formStateEx[s_formStateExCalledMakeVisible] = (value ? 1 : 0);
        }
    }

    private bool CalledOnLoad
    {
        get
        {
            return _formStateEx[s_formStateExCalledOnLoad] != 0;
        }
        set
        {
            _formStateEx[s_formStateExCalledOnLoad] = (value ? 1 : 0);
        }
    }

    /// <summary>
    ///  Gets or sets the border style of the form.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FormBorderStyle.Sizable)]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.FormBorderStyleDescr))]
    public FormBorderStyle FormBorderStyle
    {
        get => (FormBorderStyle)_formState[s_formStateBorderStyle];
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            _formState[s_formStateBorderStyle] = (int)value;
            if (_formState[s_formStateSetClientSize] == 1 && !IsHandleCreated)
            {
                Size size = ClientSize;
                ClientSize = size;
            }

            // Since setting the border style induces a call to SetBoundsCore, which,
            // when the WindowState is not Normal, will cause the values stored in the field restoredWindowBounds
            // to be replaced. The side-effect of this, of course, is that when the form window is restored,
            // the Form's size is restored to the size it was when in a non-Normal state.
            // So, we need to cache these values now before the call to UpdateFormStyles() to prevent
            // these existing values from being lost. Then, if the WindowState is something other than
            // FormWindowState.Normal after the call to UpdateFormStyles(), restore these cached values to
            // the restoredWindowBounds field.
            Rectangle preClientUpdateRestoredWindowBounds = _restoredWindowBounds;
            BoundsSpecified preClientUpdateRestoredWindowBoundsSpecified = _restoredWindowBoundsSpecified;
            int preWindowBoundsWidthIsClientSize = _formStateEx[s_formStateExWindowBoundsWidthIsClientSize];
            int preWindowBoundsHeightIsClientSize = _formStateEx[s_formStateExWindowBoundsHeightIsClientSize];

            UpdateFormStyles();

            // In Windows Theme, the FixedDialog tend to have a small Icon.
            // So to make this behave uniformly with other styles, we need to make
            // the call to UpdateIcon after the form styles have been updated.
            if (_formState[s_formStateIconSet] == 0)
            {
                UpdateWindowIcon(false);
            }

            // Now restore the values cached above.
            if (WindowState != FormWindowState.Normal)
            {
                _restoredWindowBounds = preClientUpdateRestoredWindowBounds;
                _restoredWindowBoundsSpecified = preClientUpdateRestoredWindowBoundsSpecified;
                _formStateEx[s_formStateExWindowBoundsWidthIsClientSize] = preWindowBoundsWidthIsClientSize;
                _formStateEx[s_formStateExWindowBoundsHeightIsClientSize] = preWindowBoundsHeightIsClientSize;
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
    public IButtonControl? CancelButton
    {
        get
        {
            return (IButtonControl?)Properties.GetObject(s_propCancelButton);
        }
        set
        {
            Properties.SetObject(s_propCancelButton, value);

            if (value is not null && value.DialogResult == DialogResult.None)
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
    public new Size ClientSize
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
        get => _formState[s_formStateControlBox] != 0;
        set
        {
            _formState[s_formStateControlBox] = value ? 1 : 0;
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

            if (IsHandleCreated && WindowStyle.HasFlag(WINDOW_STYLE.WS_DISABLED))
            {
                // Forms that are parent of a modal dialog must keep their WS_DISABLED style
                cp.Style |= (int)WINDOW_STYLE.WS_DISABLED;
            }
            else if (TopLevel)
            {
                // It doesn't seem to make sense to allow a top-level form to be disabled
                //
                cp.Style &= ~(int)WINDOW_STYLE.WS_DISABLED;
            }

            if (TopLevel && (_formState[s_formStateLayered] != 0))
            {
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYERED;
            }

            IWin32Window? dialogOwner = (IWin32Window?)Properties.GetObject(s_propDialogOwner);
            if (dialogOwner is not null)
            {
                cp.Parent = GetSafeHandle(dialogOwner).Handle;
            }

            FillInCreateParamsBorderStyles(cp);
            FillInCreateParamsWindowState(cp);
            FillInCreateParamsBorderIcons(cp);

            if (_formState[s_formStateTaskBar] != 0)
            {
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_APPWINDOW;
            }

            FormBorderStyle borderStyle = FormBorderStyle;
            if (!ShowIcon &&
                (borderStyle == FormBorderStyle.Sizable ||
                 borderStyle == FormBorderStyle.Fixed3D ||
                 borderStyle == FormBorderStyle.FixedSingle))
            {
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME;
            }

            if (IsMdiChild)
            {
                if (Visible
                        && (WindowState == FormWindowState.Maximized
                            || WindowState == FormWindowState.Normal))
                {
                    Form? formMdiParent = (Form?)Properties.GetObject(s_propFormMdiParent);
                    Form? form = formMdiParent?.ActiveMdiChildInternal;

                    if (form is not null
                        && form.WindowState == FormWindowState.Maximized)
                    {
                        cp.Style |= (int)WINDOW_STYLE.WS_MAXIMIZE;
                        _formState[s_formStateWindowState] = (int)FormWindowState.Maximized;
                        SetState(States.SizeLockedByOS, true);
                    }
                }

                if (_formState[s_formStateMdiChildMax] != 0)
                {
                    cp.Style |= (int)WINDOW_STYLE.WS_MAXIMIZE;
                }

                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_MDICHILD;
            }

            if (TopLevel || IsMdiChild)
            {
                FillInCreateParamsStartPosition(cp);
                // Delay setting to visible until after the handle gets created
                // to allow applyClientSize to adjust the size before displaying
                // the form.
                //
                if ((cp.Style & (int)WINDOW_STYLE.WS_VISIBLE) != 0)
                {
                    _formState[s_formStateShowWindowOnCreate] = 1;
                    cp.Style &= ~(int)WINDOW_STYLE.WS_VISIBLE;
                }
                else
                {
                    _formState[s_formStateShowWindowOnCreate] = 0;
                }
            }

            if (RightToLeft == RightToLeft.Yes && RightToLeftLayout)
            {
                // We want to turn on mirroring for Form explicitly.
                cp.ExStyle |= (int)(WINDOW_EX_STYLE.WS_EX_LAYOUTRTL | WINDOW_EX_STYLE.WS_EX_NOINHERITLAYOUT);
                // Don't need these styles when mirroring is turned on.
                cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
            }

            return cp;
        }
    }

    internal CloseReason CloseReason
    {
        get { return _closeReason; }
        set { _closeReason = value; }
    }

    /// <summary>
    ///  The default icon used by the Form. This is the standard "windows forms" icon.
    /// </summary>
    internal static Icon DefaultIcon
    {
        get
        {
            // Avoid locking if the value is filled in...
            if (s_defaultIcon is null)
            {
                lock (s_internalSyncObject)
                {
                    // Once we grab the lock, we re-check the value to avoid a
                    // race condition.
                    s_defaultIcon ??= new Icon(typeof(Form), "wfc");
                }
            }

            return s_defaultIcon;
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
            return _dialogResult;
        }

        set
        {
            // valid values are 0x0 to 0x7
            SourceGenerated.EnumValidator.Validate(value);

            _dialogResult = value;
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
            return _formState[s_formStateHelpButton] != 0;
        }

        set
        {
            if (value)
            {
                _formState[s_formStateHelpButton] = 1;
            }
            else
            {
                _formState[s_formStateHelpButton] = 0;
            }

            UpdateFormStyles();
        }
    }

    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormHelpButtonClickedDescr))]
    public event CancelEventHandler? HelpButtonClicked
    {
        add => Events.AddHandler(s_helpButtonChangedEvent, value);
        remove => Events.RemoveHandler(s_helpButtonChangedEvent, value);
    }

    /// <summary>
    ///  Gets or sets the icon for the form.
    /// </summary>
    [AmbientValue(null)]
    [Localizable(true)]
    [SRCategory(nameof(SR.CatWindowStyle))]
    [SRDescription(nameof(SR.FormIconDescr))]
    public Icon? Icon
    {
        get
        {
            if (_formState[s_formStateIconSet] == 0)
            {
                return DefaultIcon;
            }

            return _icon;
        }
        set
        {
            if (_icon != value)
            {
                // If the user is setting the default back in, treat this
                // as a reset.
                if (value == s_defaultIcon)
                {
                    value = null;
                }

                // If null is passed, reset the icon.
                _formState[s_formStateIconSet] = value is null ? 0 : 1;
                _icon = value;

                if (_smallIcon is not null)
                {
                    _smallIcon.Dispose();
                    _smallIcon = null;
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
            return _formStateEx[s_formStateExWindowClosing] == 1;
        }
        set
        {
            _formStateEx[s_formStateExWindowClosing] = value ? 1 : 0;
        }
    }

    // Returns a more accurate statement of whether or not the form is maximized.
    // during handle creation, an MDIChild is created as not maximized.
    private bool IsMaximized
    {
        get
        {
            return (WindowState == FormWindowState.Maximized || (IsMdiChild && (_formState[s_formStateMdiChildMax] == 1)));
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
    [MemberNotNullWhen(true, nameof(MdiParentInternal))]
    public bool IsMdiChild
    {
        get => Properties.ContainsObjectThatIsNotNull(s_propFormMdiParent);
    }

    // Deactivates active MDI child and temporarily marks it as unfocusable,
    // so that WM_SETFOCUS sent to MDIClient does not activate that child. (See MdiClient.WndProc).
    internal bool IsMdiChildFocusable
    {
        get => Properties.TryGetObject(s_propMdiChildFocusable, out bool value) && value;
        set
        {
            if (value != IsMdiChildFocusable)
            {
                Properties.SetObject(s_propMdiChildFocusable, value);
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
    [MemberNotNullWhen(true, nameof(MdiClient))]
    public bool IsMdiContainer
    {
        get
        {
            return _ctlClient is not null;
        }

        set
        {
            if (value == IsMdiContainer)
            {
                return;
            }

            if (value)
            {
                Debug.Assert(_ctlClient is null, "why isn't ctlClient null");
                AllowTransparency = false;
                Controls.Add(new MdiClient());
            }
            else
            {
                Debug.Assert(_ctlClient is not null, "why is ctlClient null");
                ActiveMdiChildInternal = null;
                _ctlClient.Dispose();
            }

            // since we paint the background when mdi is true, we need
            // to invalidate here
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
        get => _formState[s_formStateKeyPreview] != 0;
        set => _formState[s_formStateKeyPreview] = value ? 1 : 0;
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
        get => Properties.GetRectangle(s_propMaximizedBounds, out _);
        set
        {
            if (!value.Equals(MaximizedBounds))
            {
                Properties.SetRectangle(s_propMaximizedBounds, value);
                OnMaximizedBoundsChanged(EventArgs.Empty);
            }
        }
    }

    private static readonly object s_maximizeBoundsChangedEvent = new();

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.FormOnMaximizedBoundsChangedDescr))]
    public event EventHandler? MaximizedBoundsChanged
    {
        add => Events.AddHandler(s_maximizeBoundsChangedEvent, value);
        remove => Events.RemoveHandler(s_maximizeBoundsChangedEvent, value);
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
            if (Properties.ContainsInteger(s_propMaxTrackSizeWidth))
            {
                return new Size(Properties.GetInteger(s_propMaxTrackSizeWidth), Properties.GetInteger(s_propMaxTrackSizeHeight));
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

                UpdateMaximumSize(value);
            }
        }
    }

    private void UpdateMaximumSize(Size value, bool updateFormSize = true)
    {
        Properties.SetInteger(s_propMaxTrackSizeWidth, value.Width);
        Properties.SetInteger(s_propMaxTrackSizeHeight, value.Height);

        // Bump minimum size if necessary
        if (!MinimumSize.IsEmpty && !value.IsEmpty)
        {
            if (Properties.GetInteger(s_propMinTrackSizeWidth) > value.Width)
            {
                Properties.SetInteger(s_propMinTrackSizeWidth, value.Width);
            }

            if (Properties.GetInteger(s_propMinTrackSizeHeight) > value.Height)
            {
                Properties.SetInteger(s_propMinTrackSizeHeight, value.Height);
            }
        }

        // UpdateFormSize=false when Minimum/Maximum sizes get updated as a result of DPI_CHANGED message.
        // DPI_CHANGED message updates the Form size with the SuggestedRectangle provided by Windows.
        if (updateFormSize)
        {
            // Keep form size within new limits
            Size size = Size;
            if (!value.IsEmpty && (size.Width > value.Width || size.Height > value.Height))
            {
                Size = new Size(Math.Min(size.Width, value.Width), Math.Min(size.Height, value.Height));
            }
        }

        OnMaximumSizeChanged(EventArgs.Empty);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.FormOnMaximumSizeChangedDescr))]
    public event EventHandler? MaximumSizeChanged
    {
        add => Events.AddHandler(s_maximumSizeChangedEvent, value);

        remove => Events.RemoveHandler(s_maximumSizeChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatWindowStyle))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.FormMenuStripDescr))]
    [TypeConverter(typeof(ReferenceConverter))]
    public MenuStrip? MainMenuStrip
    {
        get
        {
            return (MenuStrip?)Properties.GetObject(s_propMainMenuStrip);
        }
        set
        {
            Properties.SetObject(s_propMainMenuStrip, value);
            if (IsHandleCreated)
            {
                UpdateMenuHandles(recreateMenu: true);
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
    public new event EventHandler? MarginChanged
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
            if (Properties.ContainsInteger(s_propMinTrackSizeWidth))
            {
                return new Size(Properties.GetInteger(s_propMinTrackSizeWidth), Properties.GetInteger(s_propMinTrackSizeHeight));
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

                UpdateMinimumSize(value);
            }
        }
    }

    private void UpdateMinimumSize(Size value, bool updateFormSize = true)
    {
        Properties.SetInteger(s_propMinTrackSizeWidth, value.Width);
        Properties.SetInteger(s_propMinTrackSizeHeight, value.Height);

        // Bump maximum size if necessary
        if (!MaximumSize.IsEmpty && !value.IsEmpty)
        {
            if (Properties.GetInteger(s_propMaxTrackSizeWidth) < value.Width)
            {
                Properties.SetInteger(s_propMaxTrackSizeWidth, value.Width);
            }

            if (Properties.GetInteger(s_propMaxTrackSizeHeight) < value.Height)
            {
                Properties.SetInteger(s_propMaxTrackSizeHeight, value.Height);
            }
        }

        // UpdateFormSize=false when Minimum/Maximum sizes get updated as a result of DPI_CHANGED message.
        // DPI_CHANGED message updates the Form size with the SuggestedRectangle provided by Windows.
        if (updateFormSize)
        {
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
                PInvoke.SetWindowPos(
                    this,
                    HWND.HWND_TOP,
                    Location.X,
                    Location.Y,
                    Size.Width,
                    Size.Height,
                    SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
            }
        }

        OnMinimumSizeChanged(EventArgs.Empty);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.FormOnMinimumSizeChangedDescr))]
    public event EventHandler? MinimumSizeChanged
    {
        add => Events.AddHandler(s_minimumSizeChangedEvent, value);

        remove => Events.RemoveHandler(s_minimumSizeChangedEvent, value);
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
            return _formState[s_formStateMaximizeBox] != 0;
        }
        set
        {
            if (value)
            {
                _formState[s_formStateMaximizeBox] = 1;
            }
            else
            {
                _formState[s_formStateMaximizeBox] = 0;
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
            if (_ctlClient is not null)
            {
                return _ctlClient.MdiChildren;
            }
            else
            {
                return [];
            }
        }
    }

    /// <summary>
    /// Gets or sets the anchoring for minimized MDI children.
    /// </summary>
    /// <value><see langword="true" /> to anchor minimized MDI children to the bottom left of the parent form; <see langword="false" /> to anchor to the top left of the parent form.</value>
    /// <remarks>
    ///  <para>
    ///   By default Windows Forms anchors MDI children to the bottom left of the parent form, whilst the Windows default is top left.
    ///  </para>
    /// </remarks>
    [SRCategory(nameof(SR.CatWindowStyle))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.FormMdiChildrenMinimizedAnchorBottomDescr))]
    public bool MdiChildrenMinimizedAnchorBottom { get; set; } = true;

    /// <summary>
    ///  Gets the MDIClient that the MDI container form is using to contain Multiple Document Interface (MDI) child forms,
    ///  if this is an MDI container form.
    ///  Represents the client area of a Multiple Document Interface (MDI) Form window, also known as the MDI child window.
    /// </summary>
    internal MdiClient? MdiClient
    {
        get
        {
            return _ctlClient;
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
    public Form? MdiParent
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

    private Form? MdiParentInternal
    {
        get => (Form?)Properties.GetObject(s_propFormMdiParent);
        set
        {
            Form? formMdiParent = (Form?)Properties.GetObject(s_propFormMdiParent);
            if (value == formMdiParent && (value is not null || ParentInternal is null))
            {
                return;
            }

            if (value is not null && CreateThreadId != value.CreateThreadId)
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
                    Properties.SetObject(s_propFormMdiParent, value);

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

                InvalidateMergedMenu();
                UpdateMenuHandles();
            }
            finally
            {
                UpdateStyles();
                Visible = oldVisibleBit;
            }
        }
    }

    private MdiWindowListStrip? MdiWindowListStrip
    {
        get { return Properties.GetObject(s_propMdiWindowListStrip) as MdiWindowListStrip; }
        set { Properties.SetObject(s_propMdiWindowListStrip, value); }
    }

    private MdiControlStrip? MdiControlStrip
    {
        get { return Properties.GetObject(s_propMdiControlStrip) as MdiControlStrip; }
        set { Properties.SetObject(s_propMdiControlStrip, value); }
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
            return _formState[s_formStateMinimizeBox] != 0;
        }
        set
        {
            if (value)
            {
                _formState[s_formStateMinimizeBox] = 1;
            }
            else
            {
                _formState[s_formStateMinimizeBox] = 0;
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
            object? opacity = Properties.GetObject(s_propOpacity);
            if (opacity is not null)
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

            Properties.SetObject(s_propOpacity, value);

            bool oldLayered = (_formState[s_formStateLayered] != 0);

            if (OpacityAsByte < 255)
            {
                AllowTransparency = true;
                if (_formState[s_formStateLayered] != 1)
                {
                    _formState[s_formStateLayered] = 1;
                    if (!oldLayered)
                    {
                        UpdateStyles();
                    }
                }
            }
            else
            {
                _formState[s_formStateLayered] = (TransparencyKey != Color.Empty) ? 1 : 0;
                if (oldLayered != (_formState[s_formStateLayered] != 0))
                {
                    CreateParams cp = CreateParams;
                    if ((int)ExtendedWindowStyle != cp.ExStyle)
                    {
                        PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_EXSTYLE, cp.ExStyle);
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
    ///  Gets an array of <see cref="Form"/> objects that represent all forms that are owned by this form.
    /// </summary>
    [SRCategory(nameof(SR.CatWindowStyle))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.FormOwnedFormsDescr))]
    public Form[] OwnedForms
    {
        get
        {
            Form?[]? ownedForms = (Form?[]?)Properties.GetObject(s_propOwnedForms);
            int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);

            Form[] result = new Form[ownedFormsCount];
            if (ownedFormsCount > 0)
            {
                Array.Copy(ownedForms!, 0, result, 0, ownedFormsCount);
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
    public Form? Owner
    {
        get
        {
            return OwnerInternal;
        }
        set
        {
            Form? ownerOld = OwnerInternal;
            if (ownerOld == value)
            {
                return;
            }

            if (value is not null && !TopLevel)
            {
                throw new ArgumentException(SR.NonTopLevelCantHaveOwner, nameof(value));
            }

            CheckParentingCycle(this, value);
            CheckParentingCycle(value, this);

            Properties.SetObject(s_propOwner, null);

            ownerOld?.RemoveOwnedForm(this);

            Properties.SetObject(s_propOwner, value);

            value?.AddOwnedForm(this);

            UpdateHandleWithOwner();
        }
    }

    internal Form? OwnerInternal
    {
        get
        {
            return (Form?)Properties.GetObject(s_propOwner);
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
            if (_restoreBounds.Width == -1
                && _restoreBounds.Height == -1
                && _restoreBounds.X == -1
                && _restoreBounds.Y == -1)
            {
                // Form scaling depends on this property being
                // set correctly.  In some cases (where the size has not yet been set or
                // has only been set to the default, restoreBounds will remain uninitialized until the
                // handle has been created.  In this case, return the current Bounds.
                return Bounds;
            }

            return _restoreBounds;
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
            return _rightToLeftLayout;
        }

        set
        {
            if (value != _rightToLeftLayout)
            {
                _rightToLeftLayout = value;
                using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                {
                    OnRightToLeftLayoutChanged(EventArgs.Empty);
                }
            }
        }
    }

    internal override Control? ParentInternal
    {
        get => base.ParentInternal;
        set
        {
            if (value is not null)
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
        get => _formState[s_formStateTaskBar] != 0;
        set
        {
            if (ShowInTaskbar != value)
            {
                _formState[s_formStateTaskBar] = value ? 1 : 0;
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
        get => _formStateEx[s_formStateExShowIcon] != 0;
        set
        {
            _formStateEx[s_formStateExShowIcon] = value ? 1 : 0;
            if (!value)
            {
                UpdateStyles();
            }

            UpdateWindowIcon(true);
        }
    }

    internal override SHOW_WINDOW_CMD ShowParams
    {
        get
        {
            // From MSDN:
            //
            //  The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter
            //  as its nCmdShow parameter. Subsequent calls to ShowWindow must use one of the values in the given list,
            //  instead of the one specified by the WinMain function's nCmdShow parameter.

            // As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call
            //  to ShowWindow if the program that launched the application specifies startup information in the
            //  STARTUPINFO structure. In this case, ShowWindow uses the information specified in the STARTUPINFO
            //  structure to show the window. On subsequent calls, the application must call ShowWindow with nCmdShow
            //  set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the
            //  application. This behavior is designed for the following situations:
            //
            //    Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.
            //
            //    Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared, and
            //    later call ShowWindow with the SW_SHOW flag set to make it visible.

            switch (WindowState)
            {
                case FormWindowState.Maximized:
                    return SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED;
                case FormWindowState.Minimized:
                    return SHOW_WINDOW_CMD.SW_SHOWMINIMIZED;
            }

            if (ShowWithoutActivation)
            {
                return SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE;
            }

            return SHOW_WINDOW_CMD.SW_SHOW;
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
    public new Size Size
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
            return (SizeGripStyle)_formState[s_formStateSizeGripStyle];
        }
        set
        {
            if (SizeGripStyle != value)
            {
                // do some bounds checking here
                //
                // valid values are 0x0 to 0x2
                SourceGenerated.EnumValidator.Validate(value);

                _formState[s_formStateSizeGripStyle] = (int)value;
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
            return (FormStartPosition)_formState[s_formStateStartPos];
        }
        set
        {
            // valid values are 0x0 to 0x4
            SourceGenerated.EnumValidator.Validate(value);
            _formState[s_formStateStartPos] = (int)value;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new int TabIndex
    {
        get => base.TabIndex;
        set => base.TabIndex = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabIndexChanged
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
    [DispId(PInvokeCore.DISPID_TABSTOP)]
    [SRDescription(nameof(SR.ControlTabStopDescr))]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    /// <summary>
    ///  For forms that are show in task bar false, this returns a HWND they must be parented to in order for it to work.
    /// </summary>
    private IHandle<HWND> TaskbarOwner
    {
        get
        {
            _ownerWindow ??= new NativeWindow();

            if (_ownerWindow.Handle == IntPtr.Zero)
            {
                CreateParams cp = new CreateParams
                {
                    ExStyle = (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW
                };

                _ownerWindow.CreateHandle(cp);
            }

            return _ownerWindow;
        }
    }

    [SettingsBindable(true)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether to display the form as a top-level window.
    /// </summary>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool TopLevel
    {
        get => GetTopLevel();
        set
        {
            if (!value && IsMdiContainer && !DesignMode)
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
        get => _formState[s_formStateTopMost] != 0;
        set
        {
            if (IsHandleCreated && TopLevel)
            {
                PInvoke.SetWindowPos(
                    this,
                    value ? HWND.HWND_TOPMOST : HWND.HWND_NOTOPMOST,
                    0, 0, 0, 0,
                    SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE);
            }

            _formState[s_formStateTopMost] = value ? 1 : 0;
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
            object? key = Properties.GetObject(s_propTransparencyKey);
            if (key is not null)
            {
                return (Color)key;
            }

            return Color.Empty;
        }
        set
        {
            Properties.SetObject(s_propTransparencyKey, value);
            if (!IsMdiContainer)
            {
                bool oldLayered = (_formState[s_formStateLayered] == 1);
                if (value != Color.Empty)
                {
                    AllowTransparency = true;
                    _formState[s_formStateLayered] = 1;
                }
                else
                {
                    _formState[s_formStateLayered] = (OpacityAsByte < 255) ? 1 : 0;
                }

                if (oldLayered != (_formState[s_formStateLayered] != 0))
                {
                    UpdateStyles();
                }

                UpdateLayered();
            }
        }
    }

    /// <summary>
    /// Adjusts form location based on <see cref="FormStartPosition"/>
    /// </summary>
    internal void AdjustFormPosition()
    {
        FormStartPosition startPos = (FormStartPosition)_formState[s_formStateStartPos];
        if (startPos == FormStartPosition.CenterParent)
        {
            CenterToParent();
        }
        else if (startPos == FormStartPosition.CenterScreen)
        {
            CenterToScreen();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void SetVisibleCore(bool value)
    {
        // If DialogResult.OK and the value == Visible then this code has been called either through
        // ShowDialog( ) or explicit Hide( ) by the user. So don't go through this function again.
        // This will avoid flashing during closing the dialog;
        if (value == Visible && _dialogResult == DialogResult.OK)
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
                    if (_dialogResult != DialogResult.None)
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
            if (_formState[s_formStateSWCalled] == 0)
            {
                PInvoke.SendMessage(this, PInvoke.WM_SHOWWINDOW, (WPARAM)(BOOL)value);
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
                InvalidateMergedMenu();
                SetState(States.Visible, false);
            }
            else
            {
                // The ordering is important here... Force handle creation
                // (getHandle) then show the window (ShowWindow) then finish
                // creating children using createControl...
                SetState(States.Visible, true);

                // Ask the mdiClient to re-layout the controls so that any docking or
                // anchor settings for this mdi child window will be honored.
                MdiParentInternal.MdiClient?.PerformLayout();

                if (ParentInternal is not null && ParentInternal.Visible)
                {
                    using SuspendLayoutScope scope = new(this);
                    PInvoke.ShowWindow(this, SHOW_WINDOW_CMD.SW_SHOW);
                    CreateControl();

                    // If this form is mdichild and maximized, we need to redraw the MdiParent non-client area to
                    // update the menu bar because we always create the window as if it were not maximized.
                    // See comment on CreateHandle about this.
                    if (WindowState == FormWindowState.Maximized)
                    {
                        MdiParentInternal.UpdateWindowIcon(true);
                    }
                }
            }

            OnVisibleChanged(EventArgs.Empty);
        }

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
    ///  Sets the rounding style of the corners of this Form.
    /// </summary>
    /// <param name="cornerPreference">
    ///  A value of the <see cref="WindowCornerPreference"/> enum
    ///  which determines the corner rounding style.
    /// </param>
    public void SetWindowCornerPreference(WindowCornerPreference cornerPreference)
    {
        SetWindowCornerPreferenceInternal(cornerPreference);
    }

    private unsafe void SetWindowCornerPreferenceInternal(WindowCornerPreference cornerPreference)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        DWM_WINDOW_CORNER_PREFERENCE dwmCornerPreference = cornerPreference switch
        {
            WindowCornerPreference.Default => DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DEFAULT,
            WindowCornerPreference.DoNotRound => DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_DONOTROUND,
            WindowCornerPreference.Round => DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND,
            WindowCornerPreference.RoundSmall => DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUNDSMALL,
            _ => throw new ArgumentOutOfRangeException(nameof(cornerPreference))
        };

        PInvoke.DwmSetWindowAttribute(
            HWND,
            DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE,
            &dwmCornerPreference,
            sizeof(DWM_WINDOW_CORNER_PREFERENCE));
    }

    /// <summary>
    ///  Sets the color of the border of this Form.
    /// </summary>
    /// <param name="color">The border color.</param>
    public void SetWindowBorderColor(Color color)
    {
        SetWindowBorderColorInternal(color);
    }

    private unsafe void SetWindowBorderColorInternal(Color color)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        COLORREF colorRef = color;

        PInvoke.DwmSetWindowAttribute(
            HWND,
            DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR,
            &colorRef,
            (uint)sizeof(COLORREF));
    }

    /// <summary>
    ///  Sets the color of the title bar of this Form containing the caption and control buttons.
    /// </summary>
    /// <param name="color">The title bar color.</param>
    public void SetWindowCaptionColor(Color color)
    {
        SetWindowCaptionColorInternal(color);
    }

    private unsafe void SetWindowCaptionColorInternal(Color color)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        COLORREF colorRef = color;

        PInvoke.DwmSetWindowAttribute(
            HWND,
            DWMWINDOWATTRIBUTE.DWMWA_CAPTION_COLOR,
            &colorRef,
            (uint)sizeof(COLORREF));
    }

    /// <summary>
    ///  Sets the color of the text in the title bar of this Form.
    /// </summary>
    /// <param name="color">The text color of the title bar.</param>
    public void SetWindowCaptionTextColor(Color color)
    {
        SetWindowCaptionTextColorInternal(color);
    }

    private unsafe void SetWindowCaptionTextColorInternal(Color color)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        COLORREF colorRef = color;

        PInvoke.DwmSetWindowAttribute(
            HWND,
            DWMWINDOWATTRIBUTE.DWMWA_TEXT_COLOR,
            &colorRef,
            (uint)sizeof(COLORREF));
    }

    /// <summary>
    ///  Gets or sets the form's window state.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [DefaultValue(FormWindowState.Normal)]
    [SRDescription(nameof(SR.FormWindowStateDescr))]
    public FormWindowState WindowState
    {
        get => (FormWindowState)_formState[s_formStateWindowState];
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

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
                switch (value)
                {
                    case FormWindowState.Normal:
                        PInvoke.ShowWindow(this, SHOW_WINDOW_CMD.SW_NORMAL);
                        break;
                    case FormWindowState.Maximized:
                        PInvoke.ShowWindow(this, SHOW_WINDOW_CMD.SW_MAXIMIZE);
                        break;
                    case FormWindowState.Minimized:
                        PInvoke.ShowWindow(this, SHOW_WINDOW_CMD.SW_MINIMIZE);
                        break;
                }
            }

            // Now set the local property to the passed in value so that
            // when UpdateWindowState is by the ShowWindow call above, the window state in effect when
            // this call was made will still be effective while processing that method.
            _formState[s_formStateWindowState] = (int)value;
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
    public event EventHandler? Activated
    {
        add => Events.AddHandler(s_activatedEvent, value);
        remove => Events.RemoveHandler(s_activatedEvent, value);
    }

    /// <summary>
    ///  Occurs when the form is closing.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnClosingDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event CancelEventHandler? Closing
    {
        add => Events.AddHandler(s_closingEvent, value);
        remove => Events.RemoveHandler(s_closingEvent, value);
    }

    /// <summary>
    ///  Occurs when the form is closed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnClosedDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public event EventHandler? Closed
    {
        add => Events.AddHandler(s_closedEvent, value);
        remove => Events.RemoveHandler(s_closedEvent, value);
    }

    /// <summary>
    ///  Occurs when the form loses focus and is not the active form.
    /// </summary>
    [SRCategory(nameof(SR.CatFocus))]
    [SRDescription(nameof(SR.FormOnDeactivateDescr))]
    public event EventHandler? Deactivate
    {
        add => Events.AddHandler(s_deactivateEvent, value);
        remove => Events.RemoveHandler(s_deactivateEvent, value);
    }

    /// <summary>
    ///  Occurs when the form is closing.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnFormClosingDescr))]
    public event FormClosingEventHandler? FormClosing
    {
        add => Events.AddHandler(s_formClosingEvent, value);
        remove => Events.RemoveHandler(s_formClosingEvent, value);
    }

    /// <summary>
    ///  Occurs when the form is closed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnFormClosedDescr))]
    public event FormClosedEventHandler? FormClosed
    {
        add => Events.AddHandler(s_formClosedEvent, value);
        remove => Events.RemoveHandler(s_formClosedEvent, value);
    }

    /// <summary>
    ///  Occurs before the form becomes visible.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnLoadDescr))]
    public event EventHandler? Load
    {
        add => Events.AddHandler(s_loadEvent, value);
        remove => Events.RemoveHandler(s_loadEvent, value);
    }

    /// <summary>
    ///  Occurs when a Multiple Document Interface (MDI) child form is activated or closed
    ///  within an MDI application.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.FormOnMDIChildActivateDescr))]
    public event EventHandler? MdiChildActivate
    {
        add => Events.AddHandler(s_mdiChildActivateEvent, value);
        remove => Events.RemoveHandler(s_mdiChildActivateEvent, value);
    }

    /// <summary>
    ///  Occurs when the menu of a form loses focus.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnMenuCompleteDescr))]
    [Browsable(false)]
    public event EventHandler? MenuComplete
    {
        add => Events.AddHandler(s_menuCompleteEvent, value);
        remove => Events.RemoveHandler(s_menuCompleteEvent, value);
    }

    /// <summary>
    ///  Occurs when the menu of a form receives focus.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnMenuStartDescr))]
    [Browsable(false)]
    public event EventHandler? MenuStart
    {
        add => Events.AddHandler(s_menuStartEvent, value);
        remove => Events.RemoveHandler(s_menuStartEvent, value);
    }

    /// <summary>
    ///  Occurs after the input language of the form has changed.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnInputLangChangeDescr))]
    public event InputLanguageChangedEventHandler? InputLanguageChanged
    {
        add => Events.AddHandler(s_inputLanguageChangeEvent, value);
        remove => Events.RemoveHandler(s_inputLanguageChangeEvent, value);
    }

    /// <summary>
    ///  Occurs when the user attempts to change the input language for the
    ///  form.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnInputLangChangeRequestDescr))]
    public event InputLanguageChangingEventHandler? InputLanguageChanging
    {
        add => Events.AddHandler(s_inputLanguageChangeRequestEvent, value);
        remove => Events.RemoveHandler(s_inputLanguageChangeRequestEvent, value);
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => Events.AddHandler(s_rightToLeftLayoutChangedEvent, value);
        remove => Events.RemoveHandler(s_rightToLeftLayoutChangedEvent, value);
    }

    /// <summary>
    ///  Occurs whenever the form is first shown.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.FormOnShownDescr))]
    public event EventHandler? Shown
    {
        add => Events.AddHandler(s_shownEvent, value);
        remove => Events.RemoveHandler(s_shownEvent, value);
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
                if (MdiParentInternal.MdiClient is not null)
                {
                    PInvoke.SendMessage(MdiParentInternal.MdiClient, PInvoke.WM_MDIACTIVATE, (WPARAM)HWND);
                }
            }
            else
            {
                PInvoke.SetForegroundWindow(this);
            }
        }
    }

    protected void ActivateMdiChild(Form? form)
    {
        if (FormerlyActiveMdiChild is not null && !FormerlyActiveMdiChild.IsClosing)
        {
            FormerlyActiveMdiChild.UpdateWindowIcon(true);
            FormerlyActiveMdiChild = null;
        }

        Form? activeMdiChild = ActiveMdiChildInternal;
        if (activeMdiChild == form)
        {
            return;
        }

        // Don't believe we ever hit this with non-null, but leaving it intact in
        // case removing it would cause a problem.
        if (activeMdiChild is not null)
        {
            activeMdiChild.Active = false;
        }

        activeMdiChild = form;
        ActiveMdiChildInternal = form;

        if (activeMdiChild is not null)
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
    public void AddOwnedForm(Form? ownedForm)
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

        Form?[]? ownedForms = (Form?[]?)Properties.GetObject(s_propOwnedForms);
        int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);

        // Make sure this isn't already in the list:
        for (int i = 0; i < ownedFormsCount; i++)
        {
            if (ownedForms![i] == ownedForm)
            {
                return;
            }
        }

        if (ownedForms is null)
        {
            ownedForms = new Form[4];
            Properties.SetObject(s_propOwnedForms, ownedForms);
        }
        else if (ownedForms.Length == ownedFormsCount)
        {
            Form[] newOwnedForms = new Form[ownedFormsCount * 2];
            Array.Copy(ownedForms, 0, newOwnedForms, 0, ownedFormsCount);
            ownedForms = newOwnedForms;
            Properties.SetObject(s_propOwnedForms, ownedForms);
        }

        ownedForms[ownedFormsCount] = ownedForm;
        Properties.SetInteger(s_propOwnedFormsCount, ownedFormsCount + 1);
    }

    // When shrinking the form (i.e. going from Large Fonts to Small
    // Fonts) we end up making everything too small due to roundoff,
    // etc... solution - just don't shrink as much.
    //
    private static float AdjustScale(float scale)
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

    private void AdjustSystemMenu(HMENU hmenu)
    {
        UpdateWindowState();
        FormWindowState winState = WindowState;
        FormBorderStyle borderStyle = FormBorderStyle;
        bool sizableBorder = (borderStyle is FormBorderStyle.SizableToolWindow
                              or FormBorderStyle.Sizable);

        bool showMin = MinimizeBox && winState != FormWindowState.Minimized;
        bool showMax = MaximizeBox && winState != FormWindowState.Maximized;
        bool showClose = ControlBox;
        bool showRestore = winState != FormWindowState.Normal;
        bool showSize = sizableBorder && winState != FormWindowState.Minimized
                        && winState != FormWindowState.Maximized;

        if (!showMin)
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_MINIMIZE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_GRAYED);
        }
        else
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_MINIMIZE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_ENABLED);
        }

        if (!showMax)
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_MAXIMIZE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_GRAYED);
        }
        else
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_MAXIMIZE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_ENABLED);
        }

        if (!showClose)
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_CLOSE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_GRAYED);
        }
        else
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_CLOSE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_ENABLED);
        }

        if (!showRestore)
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_RESTORE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_GRAYED);
        }
        else
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_RESTORE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_ENABLED);
        }

        if (!showSize)
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_SIZE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_GRAYED);
        }
        else
        {
            PInvoke.EnableMenuItem(hmenu, PInvoke.SC_SIZE, MENU_ITEM_FLAGS.MF_BYCOMMAND | MENU_ITEM_FLAGS.MF_ENABLED);
        }

        // Prevent the finalizer running to avoid closing the handle.
        GC.KeepAlive(this);
    }

    /// <summary>
    ///  This forces the SystemMenu to look like we want.
    /// </summary>
    private void AdjustSystemMenu()
    {
        if (IsHandleCreated)
        {
            HMENU hmenu = PInvoke.GetSystemMenu(this, bRevert: false);
            AdjustSystemMenu(hmenu);
        }
    }

    /// <summary>
    ///  This auto scales the form based on the AutoScaleBaseSize.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method has been deprecated. Use the ApplyAutoScaling method instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    protected void ApplyAutoScaling()
    {
        // NOTE : This function is cloned in FormDocumentDesigner, remember to keep them in sync.

        // We also don't do this if the property is empty. Otherwise we will perform two GetAutoScaleBaseSize
        // calls only to find that they returned the same value.
        if (!_autoScaleBaseSize.IsEmpty)
        {
            Size baseVar = AutoScaleBaseSize;
            SizeF newVarF = GetAutoScaleSize(Font);
            Size newVar = new((int)Math.Round(newVarF.Width), (int)Math.Round(newVarF.Height));

            // We save a significant amount of time by bailing early if there's no work to be done
            if (baseVar.Equals(newVar))
            {
                return;
            }

            float percY = AdjustScale(newVar.Height / ((float)baseVar.Height));
            float percX = AdjustScale(newVar.Width / ((float)baseVar.Width));
            Scale(percX, percY);

            // This would ensure that we use the new font information to calculate the AutoScaleBaseSize.
            AutoScaleBaseSize = newVar;
        }
    }

    /// <summary>
    ///  This adjusts the size of the windowRect so that the client rect is the
    ///  correct size.
    /// </summary>
    private void ApplyClientSize()
    {
        if ((FormWindowState)_formState[s_formStateWindowState] != FormWindowState.Normal
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
        if (_formState[s_formStateSetClientSize] != 0)
        {
            adjustScroll = true;
            _formState[s_formStateSetClientSize] = 0;
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

        PInvokeCore.GetClientRect(this, out RECT currentClient);
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
            PInvokeCore.GetClientRect(this, out currentClient);
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
    internal override void AssignParent(Control? value)
    {
        // If we are being unparented from the MDI client control, remove
        // formMDIParent as well.
        Form? formMdiParent = (Form?)Properties.GetObject(s_propFormMdiParent);
        if (formMdiParent is not null && formMdiParent.MdiClient != value)
        {
            Properties.SetObject(s_propFormMdiParent, null);
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
        if (_dialogResult == DialogResult.None && Visible)
        {
            return false;
        }

        try
        {
            FormClosingEventArgs e = new(_closeReason, false);

            if (!CalledClosing)
            {
                OnClosing(e);
                OnFormClosing(e);
                if (e.Cancel)
                {
                    _dialogResult = DialogResult.None;
                }
                else
                {
                    // we have called closing here, and it wasn't cancelled, so we're expecting a close
                    // call again soon.
                    CalledClosing = true;
                }
            }

            if (!closingOnly && _dialogResult != DialogResult.None)
            {
                FormClosedEventArgs fc = new(_closeReason);
                OnClosed(fc);
                OnFormClosed(fc);

                // reset called closing.
                //
                CalledClosing = false;
            }
        }
        catch (Exception e)
        {
            _dialogResult = DialogResult.None;
            if (NativeWindow.WndProcShouldBeDebuggable)
            {
                throw;
            }
            else
            {
                Application.OnThreadException(e);
            }
        }

        return _dialogResult != DialogResult.None || !Visible;
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
            _closeReason = CloseReason.UserClosing;
            PInvoke.SendMessage(this, PInvoke.WM_CLOSE);
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
        return ComputeWindowSize(clientSize, (WINDOW_STYLE)cp.Style, (WINDOW_EX_STYLE)cp.ExStyle);
    }

    /// <summary>
    ///  Computes the window size from the clientSize base on the specified
    ///  window styles. This will not return the correct size if menus wrap.
    /// </summary>
    private Size ComputeWindowSize(Size clientSize, WINDOW_STYLE style, WINDOW_EX_STYLE exStyle)
    {
        RECT result = new(clientSize);
        AdjustWindowRectExForControlDpi(ref result, style, false, exStyle);
        return result.Size;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        FormAccessibleObject accessibleObject = new(this);

        // Try to raise UIA focus event for the form, if it's focused.
        // Try it after the accessible object creation, because a screen reader
        // gets the accessible object after "OnGotFocus", "OnLoad", "OnShown" handlers,
        // and the object is not created yet, when these methods work, so we can't raise
        // the event while the object is not created. It's the Form control's feature only,
        // for the rest controls the accessibility tree will be built, when they get focus.
        // This case works for an empty form or a form with disabled or invisible controls
        // to have consistent behavior with .NET Framework.
        // If the form has any control (ActiveControl is true), a screen reader will focus on it instead.
        if (Focused && ActiveControl is null)
        {
            accessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }

        return accessibleObject;
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

        if (control == _ctlClient)
        {
            _ctlClient = null;
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
        Form? form = (Form?)Properties.GetObject(s_propFormMdiParent);
        form?.SuspendUpdateMenuHandles();

        try
        {
            // If the child is created before the MDI parent we can
            // get Win32 exceptions as the MDI child is parked to the parking window.
            if (IsMdiChild && MdiParentInternal.IsHandleCreated)
            {
                MdiClient? mdiClient = MdiParentInternal.MdiClient;
                if (mdiClient is not null && !mdiClient.IsHandleCreated)
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
                && (FormWindowState)_formState[s_formStateWindowState] == FormWindowState.Maximized)
            {
                // This is the reason why we see the blue borders
                // when creating a maximized mdi child, unfortunately we cannot fix this now...
                _formState[s_formStateWindowState] = (int)FormWindowState.Normal;
                _formState[s_formStateMdiChildMax] = 1;
                base.CreateHandle();
                _formState[s_formStateWindowState] = (int)FormWindowState.Maximized;
                _formState[s_formStateMdiChildMax] = 0;
            }
            else
            {
                base.CreateHandle();
            }

            UpdateHandleWithOwner();
            UpdateWindowIcon(false);

            AdjustSystemMenu();

            if ((FormStartPosition)_formState[s_formStateStartPos] != FormStartPosition.WindowsDefaultBounds)
            {
                ApplyClientSize();
            }

            if (_formState[s_formStateShowWindowOnCreate] == 1)
            {
                Visible = true;
            }

            // avoid extra SetMenu calls for perf
            if (!TopLevel || IsMdiContainer)
            {
                UpdateMenuHandles();
            }

            // In order for a window not to have a taskbar entry, it must be owned.
            if (!ShowInTaskbar && OwnerInternal is null && TopLevel)
            {
                PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, TaskbarOwner);

                // Make sure the large icon is set so the ALT+TAB icon
                // reflects the real icon of the application
                Icon? icon = Icon;
                if (icon is not null && !TaskbarOwner.Handle.IsNull)
                {
                    PInvoke.SendMessage(TaskbarOwner, PInvoke.WM_SETICON, (WPARAM)PInvoke.ICON_BIG, (LPARAM)icon.Handle);
                }
            }

            if (_formState[s_formStateTopMost] != 0)
            {
                TopMost = true;
            }
        }
        finally
        {
            form?.ResumeUpdateMenuHandles();

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
        Form? activeMdiChild = ActiveMdiChildInternal;
        if (activeMdiChild is not null)
        {
            Form? mdiParent = activeMdiChild.MdiParentInternal;

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
            if (mdiParent is not null)
            {
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
            }

            ActiveMdiChildInternal = null;

            // Note: WM_MDIACTIVATE message is sent to the form being activated and to the form being deactivated, ideally
            // we would raise the MdiChildActivate event here accordingly but it would constitute a breaking change.

            // undo merge
            UpdateMenuHandles();
            UpdateToolStrip();
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void DefWndProc(ref Message m)
    {
        if (_ctlClient is not null && _ctlClient.IsHandleCreated && _ctlClient.ParentInternal == this)
        {
            m.ResultInternal = PInvoke.DefFrameProc(
                m.HWND,
                _ctlClient.HWND,
                (uint)m.Msg,
                m.WParamInternal,
                m.LParamInternal);

            GC.KeepAlive(_ctlClient);
        }
        else if (_formStateEx[s_formStateExUseMdiChildProc] != 0)
        {
            m.ResultInternal = PInvoke.DefMDIChildProc(m.HWND, (uint)m.Msg, m.WParamInternal, m.LParamInternal);
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

            if (Properties.ContainsObject(s_propAcceptButton))
            {
                Properties.SetObject(s_propAcceptButton, null);
            }

            if (Properties.ContainsObject(s_propCancelButton))
            {
                Properties.SetObject(s_propCancelButton, null);
            }

            if (Properties.ContainsObject(s_propDefaultButton))
            {
                Properties.SetObject(s_propDefaultButton, null);
            }

            if (Properties.ContainsObject(s_propActiveMdiChild))
            {
                Properties.SetObject(s_propActiveMdiChild, null);
            }

            if (MdiWindowListStrip is not null)
            {
                MdiWindowListStrip.Dispose();
                MdiWindowListStrip = null;
            }

            if (MdiControlStrip is not null)
            {
                MdiControlStrip.Dispose();
                MdiControlStrip = null;
            }

            if (MainMenuStrip is not null)
            {
                // should NOT call dispose on MainMenuStrip - it's likely NOT to be in the form's control collection.
                MainMenuStrip = null;
            }

            Form? owner = (Form?)Properties.GetObject(s_propOwner);
            if (owner is not null)
            {
                owner.RemoveOwnedForm(this);
                Properties.SetObject(s_propOwner, null);
            }

            Properties.SetObject(s_propDialogOwner, null);

            Form?[]? ownedForms = (Form?[]?)Properties.GetObject(s_propOwnedForms);
            int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);

            for (int i = ownedFormsCount - 1; i >= 0; i--)
            {
                // it calls remove and removes itself.
                ownedForms![i]?.Dispose();
            }

            if (_smallIcon is not null)
            {
                _smallIcon.Dispose();
                _smallIcon = null;
            }

            base.Dispose(disposing);
            _ctlClient = null;

            if (Properties.TryGetObject(s_propDummyMdiMenu, out HMENU dummyMenu) && !dummyMenu.IsNull)
            {
                Properties.RemoveObject(s_propDummyMdiMenu);
                PInvoke.DestroyMenu(dummyMenu);
            }
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
                cp.Style |= (int)WINDOW_STYLE.WS_CAPTION;
            }

            if (ControlBox)
            {
                cp.Style |= (int)(WINDOW_STYLE.WS_SYSMENU | WINDOW_STYLE.WS_CAPTION);
            }
            else
            {
                cp.Style &= ~(int)WINDOW_STYLE.WS_SYSMENU;
            }

            if (MaximizeBox)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_MAXIMIZEBOX;
            }
            else
            {
                cp.Style &= ~(int)WINDOW_STYLE.WS_MAXIMIZEBOX;
            }

            if (MinimizeBox)
            {
                cp.Style |= (int)WINDOW_STYLE.WS_MINIMIZEBOX;
            }
            else
            {
                cp.Style &= ~(int)WINDOW_STYLE.WS_MINIMIZEBOX;
            }

            if (HelpButton && !MaximizeBox && !MinimizeBox && ControlBox)
            {
                // Windows should ignore WS_EX_CONTEXTHELP unless all those conditions hold.
                // But someone must have failed the check, because Windows 2000
                // will show a help button if either the maximize or
                // minimize button is disabled.
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CONTEXTHELP;
            }
            else
            {
                cp.ExStyle &= ~(int)WINDOW_EX_STYLE.WS_EX_CONTEXTHELP;
            }
        }
    }

    /// <summary>
    ///  Adjusts the window style of the CreateParams to reflect the borderstyle.
    /// </summary>
    private void FillInCreateParamsBorderStyles(CreateParams cp)
    {
        switch ((FormBorderStyle)_formState[s_formStateBorderStyle])
        {
            case FormBorderStyle.None:
                break;
            case FormBorderStyle.FixedSingle:
                cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                break;
            case FormBorderStyle.Sizable:
                cp.Style |= (int)(WINDOW_STYLE.WS_BORDER | WINDOW_STYLE.WS_THICKFRAME);
                break;
            case FormBorderStyle.Fixed3D:
                cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                break;
            case FormBorderStyle.FixedDialog:
                cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_DLGMODALFRAME;
                break;
            case FormBorderStyle.FixedToolWindow:
                cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
                break;
            case FormBorderStyle.SizableToolWindow:
                cp.Style |= (int)(WINDOW_STYLE.WS_BORDER | WINDOW_STYLE.WS_THICKFRAME);
                cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_TOOLWINDOW;
                break;
        }
    }

    /// <summary>
    ///  Adjusts the CreateParams to reflect the window bounds and start position.
    /// </summary>
    private void FillInCreateParamsStartPosition(CreateParams cp)
    {
        if (_formState[s_formStateSetClientSize] != 0)
        {
            // When computing the client window size, don't tell them that we are going to be maximized.
            int maskedStyle = cp.Style & ~(int)(WINDOW_STYLE.WS_MAXIMIZE | WINDOW_STYLE.WS_MINIMIZE);
            Size correct = ComputeWindowSize(ClientSize, (WINDOW_STYLE)maskedStyle, (WINDOW_EX_STYLE)cp.ExStyle);
            cp.Width = correct.Width;
            cp.Height = correct.Height;
        }

        switch ((FormStartPosition)_formState[s_formStateStartPos])
        {
            case FormStartPosition.WindowsDefaultBounds:
                cp.Width = PInvoke.CW_USEDEFAULT;
                cp.Height = PInvoke.CW_USEDEFAULT;
                // no break, fall through to set the location to default...
                goto case FormStartPosition.WindowsDefaultLocation;
            case FormStartPosition.WindowsDefaultLocation:
            case FormStartPosition.CenterParent:
                // Since FillInCreateParamsStartPosition() is called
                // several times when a window is shown, we'll need to force the location
                // each time for MdiChild windows that are docked so that the window will
                // be created in the correct location and scroll bars will not be displayed.
                if (IsMdiChild && Dock != DockStyle.None)
                {
                    break;
                }

                cp.X = PInvoke.CW_USEDEFAULT;
                cp.Y = PInvoke.CW_USEDEFAULT;
                break;
            case FormStartPosition.CenterScreen:
                if (IsMdiChild)
                {
                    Control? mdiclient = MdiParentInternal.MdiClient;
                    Rectangle clientRect = mdiclient is null ? Rectangle.Empty : mdiclient.ClientRectangle;

                    cp.X = Math.Max(clientRect.X, clientRect.X + (clientRect.Width - cp.Width) / 2);
                    cp.Y = Math.Max(clientRect.Y, clientRect.Y + (clientRect.Height - cp.Height) / 2);
                }
                else
                {
                    Screen desktop;
                    IWin32Window? dialogOwner = (IWin32Window?)Properties.GetObject(s_propDialogOwner);
                    if ((OwnerInternal is not null) || (dialogOwner is not null))
                    {
                        HandleRef<HWND> ownerHandle = dialogOwner is not null
                            ? GetSafeHandle(dialogOwner)
                            : new(OwnerInternal!);
                        desktop = Screen.FromHandle(ownerHandle.Handle);
                        GC.KeepAlive(ownerHandle.Wrapper);
                    }
                    else
                    {
                        desktop = Screen.FromPoint(MousePosition);
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
    ///  Adjusts the CreateParams to reflect the window state.
    /// </summary>
    private void FillInCreateParamsWindowState(CreateParams cp)
    {
        switch ((FormWindowState)_formState[s_formStateWindowState])
        {
            case FormWindowState.Maximized:
                cp.Style |= (int)WINDOW_STYLE.WS_MAXIMIZE;
                break;
            case FormWindowState.Minimized:
                cp.Style |= (int)WINDOW_STYLE.WS_MINIMIZE;
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
            if (MdiParentInternal.MdiClient is not null)
            {
                PInvoke.SendMessage(MdiParentInternal.MdiClient, PInvoke.WM_MDIACTIVATE, this);
            }

            return Focused;
        }

        return base.FocusInternal();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete("This method has been deprecated. Use the AutoScaleDimensions property instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    public static SizeF GetAutoScaleSize(Font font)
    {
        float height = font.Height;
        float width = 9.0f;

        try
        {
            // Get the screen HDC
            using Graphics graphics = Graphics.FromHwndInternal(0);
            string magicString = "The quick brown fox jumped over the lazy dog.";
            double magicNumber = 44.549996948242189; // chosen for compatibility with older versions of windows forms, but approximately magicString.Length
            float stringWidth = graphics.MeasureString(magicString, font).Width;
            width = (float)(stringWidth / magicNumber);
        }
        catch
        {
            // We may get an bogus OutOfMemoryException
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
        // If this is a Mdi child form, child controls should process mnemonics only if this is the active mdi child.
        if (IsMdiChild &&
            (_formStateEx[s_formStateExMnemonicProcessed] == 1
                || this != MdiParentInternal.ActiveMdiChildInternal
                || WindowState == FormWindowState.Minimized))
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
        if (!TopLevel)
        {
            return;
        }

        Point p = default;
        Size s = Size;
        HWND ownerHandle = (HWND)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);

        if (!ownerHandle.IsNull)
        {
            Screen desktop = Screen.FromHandle(ownerHandle);
            Rectangle screenRect = desktop.WorkingArea;
            PInvoke.GetWindowRect(ownerHandle, out var ownerRect);

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

    /// <summary>
    ///  Centers the dialog to the screen. This will first attempt to use
    ///  the owner property to determine the correct screen, then
    ///  it will try the HWND owner of the form, and finally this will
    ///  center the form on the same monitor as the mouse cursor.
    /// </summary>
    protected void CenterToScreen()
    {
        Point p = default;
        Screen desktop;
        if (OwnerInternal is not null)
        {
            desktop = Screen.FromControl(OwnerInternal);
        }
        else
        {
            HWND hWndOwner = default;
            if (TopLevel)
            {
                hWndOwner = (HWND)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT);
            }

            desktop = !hWndOwner.IsNull ? Screen.FromHandle(hWndOwner) : Screen.FromPoint(MousePosition);
        }

        Rectangle screenRect = desktop.WorkingArea;
        p.X = Math.Max(screenRect.X, screenRect.X + (screenRect.Width - Width) / 2);
        p.Y = Math.Max(screenRect.Y, screenRect.Y + (screenRect.Height - Height) / 2);
        Location = p;
    }

    /// <summary>
    ///  Invalidates the merged menu, forcing the menu to be recreated if
    ///  needed again.
    /// </summary>
    private void InvalidateMergedMenu() => ParentForm?.UpdateMenuHandles();

    /// <summary>
    ///  Arranges the Multiple Document Interface
    ///  (MDI) child forms according to value.
    /// </summary>
    public void LayoutMdi(MdiLayout value)
    {
        _ctlClient?.LayoutMdi(value);
    }

    /// <summary>
    ///  The activate event is fired when the form is activated.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnActivated(EventArgs e)
    {
        ((EventHandler?)Events[s_activatedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Override of AutoScaleModeChange method from ContainerControl. We use this to keep our
    ///  own AutoScale property in sync.
    /// </summary>
    private protected override void OnAutoScaleModeChanged()
    {
        base.OnAutoScaleModeChanged();
        if (_formStateEx[s_formStateExSettingAutoScale] != 1)
        {
            // Obsolete code required here for backwards compat
#pragma warning disable CS0618 // Type or member is obsolete
            AutoScale = false;
#pragma warning restore CS0618
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
        ((CancelEventHandler?)Events[s_closingEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  The Closed event is fired when the form is closed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnClosed(EventArgs e)
    {
        ((EventHandler?)Events[s_closedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  The Closing event is fired before the form is closed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnFormClosing(FormClosingEventArgs e)
    {
        ((FormClosingEventHandler?)Events[s_formClosingEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  The Closed event is fired when the form is closed.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnFormClosed(FormClosedEventArgs e)
    {
        // Remove the form from Application.OpenForms (nothing happens if isn't present)
        Application.OpenForms.Remove(this);

        ((FormClosedEventHandler?)Events[s_formClosedEvent])?.Invoke(this, e);
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
        ((EventHandler?)Events[s_deactivateEvent])?.Invoke(this, e);
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
            Control? activeControl = ActiveControl;

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
    protected internal override void OnEnter(EventArgs e)
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

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);

        // Raise the UIA focus event for an empty form (form with no ActiveControl),
        // when it gets focus to a screen reader can focus on it and announce its title text.
        // If the form has any control, a screen reader will focus on it instead.
        if (Focused && IsAccessibilityObjectCreated && ActiveControl is null)
        {
            AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    /// <summary>
    ///  Inheriting classes should override this method to find out when the
    ///  handle has been created.
    ///  Call base.OnHandleCreated first.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnHandleCreated(EventArgs e)
    {
        _formStateEx[s_formStateExUseMdiChildProc] = (IsMdiChild && Visible) ? 1 : 0;
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
        _formStateEx[s_formStateExUseMdiChildProc] = 0;

        // Remove the form from OpenForms collection only if we're not recreating the handle of this form
        // (e.g., when ShowInTaskbar or RightToLeft properties get changed).
        if (!_inRecreateHandle)
        {
            Application.OpenForms.Remove(this);
        }
    }

    /// <summary>
    ///  Handles the event that a helpButton is clicked
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnHelpButtonClicked(CancelEventArgs e)
    {
        ((CancelEventHandler?)Events[s_helpButtonChangedEvent])?.Invoke(this, e);
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
            _minAutoSize = prefSize;

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
        // First - add the form to Application.OpenForms
        Application.OpenForms.Add(this);
        if (Application.UseWaitCursor)
        {
            UseWaitCursor = true;
        }

        // This will apply AutoScaling to the form just before the form becomes visible.
        if (_formState[s_formStateAutoScaling] == 1 && !DesignMode)
        {
            // Turn off autoscaling so we don't do this on every handle creation.
            _formState[s_formStateAutoScaling] = 0;

            // Obsolete code required here for backwards compat.
#pragma warning disable CS0618 // Type or member is obsolete
            ApplyAutoScaling();
#pragma warning restore CS0618
        }

        // Also, at this time we can now locate the form on the correct area of the screen.
        // We must do this after applying any autoscaling.
        if (GetState(States.Modal))
        {
            AdjustFormPosition();
        }

        // There is no good way to explain this event except to say
        // that it's just another name for OnControlCreated.
        EventHandler? handler = (EventHandler?)Events[s_loadEvent];
        if (handler is not null)
        {
            string text = Text;

            handler(this, e);

            // It seems that if you set a window style during the onload
            // event, we have a problem initially painting the window.
            // So in the event that the user has set the on load event
            // in their application, we should go ahead and invalidate
            // the controls in their collection so that we paint properly.
            // This seems to manifest itself in changes to the window caption,
            // and changes to the control box and help.

            foreach (Control c in Controls)
            {
                c.Invalidate();
            }
        }

        // Finally fire the new OnShown(unless the form has already been closed).
        if (IsHandleCreated)
        {
#pragma warning disable WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
            if (IsDarkModeEnabled)
            {
                PInvoke.SetWindowTheme(HWND, "DarkMode_Explorer", null);
            }
#pragma warning restore WFO9001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.

            BeginInvoke(new MethodInvoker(CallShownEvent));
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMaximizedBoundsChanged(EventArgs e)
    {
        if (Events[s_maximizeBoundsChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMaximumSizeChanged(EventArgs e)
    {
        if (Events[s_maximumSizeChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMinimumSizeChanged(EventArgs e)
    {
        if (Events[s_minimumSizeChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="InputLanguageChanged"/>
    ///  event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnInputLanguageChanged(InputLanguageChangedEventArgs e)
    {
        ((InputLanguageChangedEventHandler?)Events[s_inputLanguageChangeEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="InputLanguageChanging"/>
    ///  event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnInputLanguageChanging(InputLanguageChangingEventArgs e)
    {
        ((InputLanguageChangingEventHandler?)Events[s_inputLanguageChangeRequestEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnVisibleChanged(EventArgs e)
    {
        UpdateRenderSizeGrip();
        MdiParentInternal?.UpdateMdiWindowListStrip();

        base.OnVisibleChanged(e);

        // Windows forms have to behave like dialog boxes sometimes. If the
        // user has specified that the mouse should snap to the
        // Accept button using the Mouse applet in the control panel,
        // we have to respect that setting each time our form is made visible.
        bool data = false;
        if (IsHandleCreated
            && Visible
            && (AcceptButton is not null)
            && PInvokeCore.SystemParametersInfo(SYSTEM_PARAMETERS_INFO_ACTION.SPI_GETSNAPTODEFBUTTON, ref data)
            && data)
        {
            Control button = (Control)AcceptButton;
            Point pointToSnap = new(button.Left + button.Width / 2, button.Top + button.Height / 2);
            PInvoke.ClientToScreen(this, ref pointToSnap);
            if (!button.IsWindowObscured)
            {
                Cursor.Position = pointToSnap;
            }
        }
    }

    /// <summary>
    ///  Raises the <see cref="MdiChildActivate"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMdiChildActivate(EventArgs e)
    {
        UpdateMenuHandles();
        UpdateToolStrip();
        ((EventHandler?)Events[s_mdiChildActivateEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the <see cref="MenuStart"/> event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMenuStart(EventArgs e)
    {
        EventHandler? handler = (EventHandler?)Events[s_menuStartEvent];
        handler?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the MenuComplete event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnMenuComplete(EventArgs e)
    {
        ((EventHandler?)Events[s_menuCompleteEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Raises the Paint event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        if (_formState[s_formStateRenderSizeGrip] != 0)
        {
            Size size = ClientSize;
            if (Application.RenderWithVisualStyles)
            {
                _sizeGripRenderer ??= new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);

                using DeviceContextHdcScope hdc = new(e);
                _sizeGripRenderer.DrawBackground(
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
        if (!_processingDpiChanged)
        {
            _dpiFormSizes?.Clear();
        }

        base.OnResize(e);
        if (_formState[s_formStateRenderSizeGrip] != 0)
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
            _oldDeviceDpi = e.DeviceDpiOld;

            // call any additional handlers
            ((DpiChangedEventHandler?)Events[s_dpiChangedEvent])?.Invoke(this, e);

            if (e.Cancel)
            {
                return;
            }

            try
            {
                // Cache Form's size for the current and new DPI if not already done. We do this only
                // for AutoScaleMode is Font. In other modes, Windows OS will compute Form's size.
                if (AutoScaleMode == AutoScaleMode.Font)
                {
                    _dpiFormSizes ??= [];

                    if (!_dpiFormSizes.ContainsKey(e.DeviceDpiNew))
                    {
                        _dpiFormSizes.Add(e.DeviceDpiNew, new Size(e.SuggestedRectangle.Width, e.SuggestedRectangle.Height));
                    }

                    // Store size of the Form for current DPI.
                    if (!_dpiFormSizes.ContainsKey(e.DeviceDpiOld))
                    {
                        _dpiFormSizes.Add(e.DeviceDpiOld, Size);
                    }

                    // Prevent clearing Form's size cache while applying bounds from DPI change.
                    // Any other events that cause Form's size change should clear the cache.
                    _processingDpiChanged = true;
                }

                ScaleContainerForDpi(e.DeviceDpiNew, e.DeviceDpiOld, e.SuggestedRectangle);
            }
            finally
            {
                _processingDpiChanged = false;
            }
        }
    }

    /// <summary>
    ///  Occurs when the Dpi resolution of the screen this top level window is displayed on changes,
    ///  either when the top level window is moved between monitors or when the OS settings are changed.
    /// </summary>
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.FormOnDpiChangedDescr))]
    public event DpiChangedEventHandler? DpiChanged
    {
        add => Events.AddHandler(s_dpiChangedEvent, value);
        remove => Events.RemoveHandler(s_dpiChangedEvent, value);
    }

    /// <summary>
    ///  Handles the WM_DPICHANGED message
    /// </summary>
    private void WmDpiChanged(ref Message m)
    {
        DefWndProc(ref m);

        DpiChangedEventArgs e = new(_deviceDpi, m);
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
        // Compute font for the current DPI and cache it. DPI specific fonts cache is available only in PermonitorV2 mode applications.
        Font fontForDpi = GetScaledFont(Font, deviceDpiNew, deviceDpiOld);

        // If AutoScaleMode=AutoScaleMode.Dpi then we continue with the linear size we get from Windows for the top-level window.
        if (AutoScaleMode == AutoScaleMode.Dpi)
        {
            return false;
        }

        // Calculate AutoscaleFactor for AutoScaleMode.Font. We will be using this factor to scale child controls
        // and use same factor to compute desired size for top-level windows for the current DPI.
        // This desired size is then used to notify Windows that we need non-linear size for top-level window.
        FontHandleWrapper fontwrapper = new(fontForDpi);
        SizeF currentAutoScaleDimensions = GetCurrentAutoScaleDimensions(fontwrapper.Handle);
        SizeF autoScaleFactor = GetCurrentAutoScaleFactor(currentAutoScaleDimensions, AutoScaleDimensions);

        desiredSize.Width = (int)(Size.Width * autoScaleFactor.Width);
        desiredSize.Height = (int)(Size.Height * autoScaleFactor.Height);
        Debug.WriteLine($"AutoScaleFactor computed for new DPI = {autoScaleFactor.Width} - {autoScaleFactor.Height}");

        // Notify Windows that the top-level window size should be based on AutoScaleMode value.
        return true;
    }

    /// <summary>
    ///  Handles the WM_GETDPISCALEDSIZE message, this is a chance for the application to
    ///  scale window size non-lineary. If this message is not processed, the size is scaled linearly by Windows.
    ///  This message is sent to top level windows before WM_DPICHANGED.
    ///  If the application responds to this message, the resulting size will be the candidate rectangle
    ///  sent to WM_DPICHANGED. The WPARAM contains a Dpi value. The size needs to be computed if
    ///  the window were to switch to this Dpi. LPARAM is used to store the Size desired for top-level window.
    ///  A return value of zero indicates that the app does not want any special behavior and the candidate rectangle will be computed linearly.
    /// </summary>
    private unsafe void WmGetDpiScaledSize(ref Message m)
    {
        DefWndProc(ref m);

        Size desiredSize = default;
        if ((_dpiFormSizes is not null && _dpiFormSizes.TryGetValue(m.WParamInternal.LOWORD, out desiredSize))
            || OnGetDpiScaledSize(_deviceDpi, m.WParamInternal.LOWORD, ref desiredSize))
        {
            SIZE* size = (SIZE*)m.LParamInternal;
            size->cx = desiredSize.Width;
            size->cy = desiredSize.Height;
            m.ResultInternal = (LRESULT)1;
            return;
        }

        m.ResultInternal = (LRESULT)0;
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

        if (Events[s_rightToLeftLayoutChangedEvent] is EventHandler eh)
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
    ///  This event fires whenever the form is first shown.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnShown(EventArgs e)
    {
        ((EventHandler?)Events[s_shownEvent])?.Invoke(this, e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnTextChanged(EventArgs e)
    {
        base.OnTextChanged(e);

        // If there is no control box, there should only be a title bar if text != "".
        int newTextEmpty = Text.Length == 0 ? 1 : 0;
        if (!ControlBox && _formState[s_formStateIsTextEmpty] != newTextEmpty)
        {
            RecreateHandle();
        }

        _formState[s_formStateIsTextEmpty] = newTextEmpty;
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
        MSG win32Message = msg;
        if (_ctlClient is not null && _ctlClient.Handle != IntPtr.Zero &&
            PInvoke.TranslateMDISysAccel(_ctlClient.HWND, win32Message))
        {
            retValue = true;
        }

        msg.MsgInternal = (MessageId)win32Message.message;
        msg.WParamInternal = win32Message.wParam;
        msg.LParamInternal = win32Message.lParam;
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
            Keys keyCode = keyData & Keys.KeyCode;
            IButtonControl? button;

            switch (keyCode)
            {
                case Keys.Return:
                    button = (IButtonControl?)Properties.GetObject(s_propDefaultButton);
                    if (button is not null)
                    {
                        // PerformClick now checks for validationcancelled...
                        if (button is Control)
                        {
                            button.PerformClick();
                        }

                        return true;
                    }

                    break;
                case Keys.Escape:
                    button = (IButtonControl?)Properties.GetObject(s_propCancelButton);
                    if (button is not null)
                    {
                        // In order to keep the behavior in sync with native
                        // and MFC dialogs, we want to not give the cancel button
                        // the focus on Escape. If we do, we end up with giving it
                        // the focus when we reshow the dialog.
                        //
                        // if (button is Control) {
                        //    ((Control)button).Focus();
                        // }
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
        // If we're the top-level form or control, we need to do the mnemonic handling.
        if (IsMdiChild && charCode != ' ')
        {
            if (ProcessMnemonic(charCode))
            {
                return true;
            }

            // ContainerControl calls ProcessMnemonic starting from the active MdiChild form (this)
            // so let's flag it as processed.
            _formStateEx[s_formStateExMnemonicProcessed] = 1;
            try
            {
                return base.ProcessDialogChar(charCode);
            }
            finally
            {
                _formStateEx[s_formStateExMnemonicProcessed] = 0;
            }
        }

        // Non-MdiChild form, just pass the call to ContainerControl.
        return base.ProcessDialogChar(charCode);
    }

    protected override bool ProcessKeyPreview(ref Message m)
    {
        if (_formState[s_formStateKeyPreview] != 0 && ProcessKeyEventArgs(ref m))
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
            int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);
            if (ownedFormsCount > 0)
            {
                Form[] ownedForms = OwnedForms;
                FormClosedEventArgs fce = new(CloseReason.FormOwnerClosing);
                for (int i = ownedFormsCount - 1; i >= 0; i--)
                {
                    if (ownedForms[i] is not null && !Application.OpenForms.Contains(ownedForms[i]))
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
        FormClosingEventArgs e = new(CloseReason.ApplicationExitCall, false);
        // e.Cancel = !Validate(true);    This would cause a breaking change between v2.0 and v1.0/v1.1 in case validation fails.
        if (!Modal)
        {
            // Fire FormClosing event on all the forms that this form owns and are not in the Application.OpenForms collection
            // This is to be consistent with what WmClose does.
            int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);
            if (ownedFormsCount > 0)
            {
                Form[] ownedForms = OwnedForms;
                FormClosingEventArgs fce = new(CloseReason.FormOwnerClosing, false);
                for (int i = ownedFormsCount - 1; i >= 0; i--)
                {
                    if (ownedForms[i] is not null && !Application.OpenForms.Contains(ownedForms[i]))
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

    internal override unsafe void RecreateHandleCore()
    {
        WINDOWPLACEMENT wp = default;

        FormStartPosition oldStartPosition = FormStartPosition.Manual;

        if (!IsMdiChild && (WindowState == FormWindowState.Minimized || WindowState == FormWindowState.Maximized))
        {
            wp.length = (uint)sizeof(WINDOWPLACEMENT);
            bool result = PInvoke.GetWindowPlacement(HWND, &wp);
            Debug.Assert(result);
        }

        if (StartPosition != FormStartPosition.Manual)
        {
            oldStartPosition = StartPosition;

            // Set the startup position to manual, to stop the form from
            // changing position each time RecreateHandle() is called.
            StartPosition = FormStartPosition.Manual;
        }

        EnumThreadWindowsCallback? callback = null;
        if (IsHandleCreated)
        {
            // First put all the owned windows into a list
            callback = new EnumThreadWindowsCallback(HWND);
            PInvoke.EnumCurrentThreadWindows(callback.Callback);

            // Reset the owner of the windows in the list
            callback.ResetOwners();
        }

        _inRecreateHandle = true;
        try
        {
            base.RecreateHandleCore();
        }
        finally
        {
            _inRecreateHandle = false;
        }

        // Set the owner of the windows in the list back to the new Form's handle
        callback?.SetOwners(Handle);

        if (oldStartPosition != FormStartPosition.Manual)
        {
            StartPosition = oldStartPosition;
        }

        if (wp.length > 0)
        {
            bool result = PInvoke.SetWindowPlacement(HWND, &wp);
            Debug.Assert(result);
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Removes a form from the list of owned forms. Also sets the owner of the
    ///  removed form to null.
    /// </summary>
    public void RemoveOwnedForm(Form? ownedForm)
    {
        if (ownedForm is null)
        {
            return;
        }

        if (ownedForm.OwnerInternal is not null)
        {
            ownedForm.Owner = null; // NOTE: this will call RemoveOwnedForm again, bypassing if.
            return;
        }

        Form?[]? ownedForms = (Form?[]?)Properties.GetObject(s_propOwnedForms);
        int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);

        if (ownedForms is not null)
        {
            for (int i = 0; i < ownedFormsCount; i++)
            {
                if (ownedForm.Equals(ownedForms[i]))
                {
                    // clear out the reference.
                    ownedForms[i] = null;

                    // compact the array.
                    if (i + 1 < ownedFormsCount)
                    {
                        Array.Copy(ownedForms, i + 1, ownedForms, i, ownedFormsCount - i - 1);
                        ownedForms[ownedFormsCount - 1] = null;
                    }

                    ownedFormsCount--;
                }
            }

            Properties.SetInteger(s_propOwnedFormsCount, ownedFormsCount);
        }
    }

    /// <summary>
    ///  Resets the form's icon the default value.
    /// </summary>
    private void ResetIcon()
    {
        _icon = null;
        if (_smallIcon is not null)
        {
            _smallIcon.Dispose();
            _smallIcon = null;
        }

        _formState[s_formStateIconSet] = 0;
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
    public event EventHandler? ResizeBegin
    {
        add => Events.AddHandler(s_resizeBeginEvent, value);
        remove => Events.RemoveHandler(s_resizeBeginEvent, value);
    }

    /// <summary>
    ///  Occurs when the control exits the sizing modal loop.
    /// </summary>
    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.FormOnResizeEndDescr))]
    public event EventHandler? ResizeEnd
    {
        add => Events.AddHandler(s_resizeEndEvent, value);
        remove => Events.RemoveHandler(s_resizeEndEvent, value);
    }

    /// <summary>
    ///  This is called when we have just been restored after being
    ///  minimized.  At this point we resume our layout.
    /// </summary>
    private void ResumeLayoutFromMinimize()
    {
        // If we're currently minimized, resume our layout because we are
        // about to snap out of it.
        if (_formState[s_formStateWindowState] == (int)FormWindowState.Minimized)
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
            Size restoredSize = _restoredWindowBounds.Size;
            if ((_restoredWindowBoundsSpecified & BoundsSpecified.Size) != 0)
            {
                restoredSize = SizeFromClientSizeInternal(restoredSize);
            }

            SetBounds(_restoredWindowBounds.X, _restoredWindowBounds.Y,
                _formStateEx[s_formStateExWindowBoundsWidthIsClientSize] == 1 ? restoredSize.Width : _restoredWindowBounds.Width,
                _formStateEx[s_formStateExWindowBoundsHeightIsClientSize] == 1 ? restoredSize.Height : _restoredWindowBounds.Height,
                      _restoredWindowBoundsSpecified);
            _restoredWindowBoundsSpecified = 0;
            _restoredWindowBounds = new Rectangle(-1, -1, -1, -1);
            _formStateEx[s_formStateExWindowBoundsHeightIsClientSize] = 0;
            _formStateEx[s_formStateExWindowBoundsWidthIsClientSize] = 0;
        }
    }

    /// <summary>
    ///  Decrements updateMenuHandleSuspendCount. If updateMenuHandleSuspendCount
    ///  becomes zero and updateMenuHandlesDeferred is true, updateMenuHandles
    ///  is called.
    /// </summary>
    private void ResumeUpdateMenuHandles()
    {
        int suspendCount = _formStateEx[s_formStateExUpdateMenuHandlesSuspendCount];
        if (suspendCount <= 0)
        {
            throw new InvalidOperationException(SR.TooManyResumeUpdateMenuHandles);
        }

        _formStateEx[s_formStateExUpdateMenuHandlesSuspendCount] = --suspendCount;
        if (suspendCount == 0 && _formStateEx[s_formStateExUpdateMenuHandlesDeferred] != 0)
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
            PInvoke.SetActiveWindow(this);
        }
        else if (IsMdiChild)
        {
            PInvoke.SetActiveWindow(MdiParentInternal);
            if (MdiParentInternal.MdiClient is not null)
            {
                PInvoke.SendMessage(MdiParentInternal.MdiClient, PInvoke.WM_MDIACTIVATE, (WPARAM)HWND);
            }
        }
        else
        {
            Form? form = ParentForm;
            if (form is not null)
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
        using SuspendLayoutScope scope = new(this);

        // Get size values in advance to prevent one change from affecting another.
        Size clientSize = ClientSize;
        ScaleMinMaxSize(x, y);
        ScaleDockPadding(x, y);
        if (WindowState == FormWindowState.Normal)
        {
            ClientSize = ScaleSize(clientSize, x, y);
        }

        foreach (Control control in Controls)
        {
#pragma warning disable CS0618 // Type or member is obsolete - compat
            control?.Scale(x, y);
#pragma warning restore CS0618
        }
    }

    /// <summary>
    /// Scales Form's properties Min and Max size with the scale factor provided.
    /// </summary>
    /// <param name="xScaleFactor">The scale factor to be applied on width of the property being scaled.</param>
    /// <param name="yScaleFactor">The scale factor to be applied on height of the property being scaled.</param>
    /// <param name="updateContainerSize"><see langword="true"/> to resize of the Form along with properties being scaled; otherwise, <see langword="false"/>.</param>
    protected override void ScaleMinMaxSize(float xScaleFactor, float yScaleFactor, bool updateContainerSize = true)
    {
        base.ScaleMinMaxSize(xScaleFactor, yScaleFactor, updateContainerSize);
        if (WindowState == FormWindowState.Normal)
        {
            Size minSize = MinimumSize;
            Size maxSize = MaximumSize;
            if (!minSize.IsEmpty)
            {
                UpdateMinimumSize(ScaleSize(minSize, xScaleFactor, yScaleFactor), updateContainerSize);
            }

            if (!maxSize.IsEmpty)
            {
                UpdateMaximumSize(ScaleSize(maxSize, xScaleFactor, yScaleFactor), updateContainerSize);
            }
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
        _formStateEx[s_formStateExInScale] = 1;
        try
        {
            // don't scale the location of MDI child forms
            if (MdiParentInternal is not null)
            {
                specified &= ~BoundsSpecified.Location;
            }

            base.ScaleControl(factor, specified);
        }
        finally
        {
            _formStateEx[s_formStateExInScale] = 0;
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
                _restoredWindowBoundsSpecified |= (specified & (BoundsSpecified.X | BoundsSpecified.Y));
            }

            _restoredWindowBoundsSpecified |= (specified & (BoundsSpecified.Width | BoundsSpecified.Height));

            if ((specified & BoundsSpecified.X) != 0)
            {
                _restoredWindowBounds.X = x;
            }

            if ((specified & BoundsSpecified.Y) != 0)
            {
                _restoredWindowBounds.Y = y;
            }

            if ((specified & BoundsSpecified.Width) != 0)
            {
                _restoredWindowBounds.Width = width;
                _formStateEx[s_formStateExWindowBoundsWidthIsClientSize] = 0;
            }

            if ((specified & BoundsSpecified.Height) != 0)
            {
                _restoredWindowBounds.Height = height;
                _formStateEx[s_formStateExWindowBoundsHeightIsClientSize] = 0;
            }
        }

        // Update RestoreBounds
        if ((specified & BoundsSpecified.X) != 0)
        {
            _restoreBounds.X = x;
        }

        if ((specified & BoundsSpecified.Y) != 0)
        {
            _restoreBounds.Y = y;
        }

        if ((specified & BoundsSpecified.Width) != 0 || _restoreBounds.Width == -1)
        {
            _restoreBounds.Width = width;
        }

        if ((specified & BoundsSpecified.Height) != 0 || _restoreBounds.Height == -1)
        {
            _restoreBounds.Height = height;
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
    private void SetDefaultButton(IButtonControl? button)
    {
        IButtonControl? defaultButton = (IButtonControl?)Properties.GetObject(s_propDefaultButton);

        if (defaultButton != button)
        {
            defaultButton?.NotifyDefault(false);

            Properties.SetObject(s_propDefaultButton, button);
            button?.NotifyDefault(true);
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

        _formState[s_formStateSetClientSize] = 1;
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
    public void Show(IWin32Window? owner)
    {
        if (owner == this)
        {
            throw new InvalidOperationException(string.Format(SR.OwnsSelfOrOwner, nameof(Show)));
        }

        if (Visible)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnVisible, nameof(Show)));
        }

        if (!Enabled)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnDisabled, nameof(Show)));
        }

        if (!TopLevel)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnNonTopLevel, nameof(Show)));
        }

        if (!SystemInformation.UserInteractive)
        {
            throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
        }

        if ((owner is not null) && !owner.GetExtendedStyle().HasFlag(WINDOW_EX_STYLE.WS_EX_TOPMOST))
        {
            // It's not the top-most window
            if (owner is Control ownerControl)
            {
                owner = ownerControl.TopLevelControlInternal;
            }
        }

        HWND activeHwnd = PInvoke.GetActiveWindow();
        HandleRef<HWND> ownerHwnd = owner is null ? GetHandleRef(activeHwnd) : GetSafeHandle(owner);
        Properties.SetObject(s_propDialogOwner, owner);
        Form? oldOwner = OwnerInternal;
        if (owner is Form ownerForm && owner != oldOwner)
        {
            Owner = ownerForm;
        }

        if (!ownerHwnd.IsNull && ownerHwnd.Handle != HWND)
        {
            // Catch the case of a window trying to own its owner
            if (PInvoke.GetWindowLong(ownerHwnd, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT) == HWND)
            {
                throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner, nameof(Show)), nameof(owner));
            }

            // Set the new owner.
            PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, ownerHwnd);
        }

        Visible = true;
    }

    /// <summary>
    ///  Displays this form as a modal dialog box with no owner window.
    /// </summary>
    public DialogResult ShowDialog() => ShowDialog(null);

    /// <summary>
    ///  Shows this form as a modal dialog with the specified owner.
    /// </summary>
    public DialogResult ShowDialog(IWin32Window? owner)
    {
        if (owner == this)
        {
            throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner, nameof(ShowDialog)), nameof(owner));
        }

        if (Visible)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnVisible, nameof(ShowDialog)));
        }

        if (!Enabled)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnDisabled, nameof(ShowDialog)));
        }

        if (!TopLevel)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnNonTopLevel, nameof(ShowDialog)));
        }

        if (Modal)
        {
            throw new InvalidOperationException(string.Format(SR.ShowDialogOnModal, nameof(ShowDialog)));
        }

        if (!SystemInformation.UserInteractive)
        {
            throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
        }

        if ((owner is not null) && !owner.GetExtendedStyle().HasFlag(WINDOW_EX_STYLE.WS_EX_TOPMOST))
        {
            // It's not the top-most window
            if (owner is Control ownerControl)
            {
                owner = ownerControl.TopLevelControlInternal;
            }
        }

        CalledOnLoad = false;
        CalledMakeVisible = false;

        // for modal dialogs make sure we reset close reason.
        CloseReason = CloseReason.None;

        HWND captureHwnd = PInvoke.GetCapture();
        if (!captureHwnd.IsNull)
        {
            PInvoke.SendMessage(captureHwnd, PInvoke.WM_CANCELMODE);
            PInvoke.ReleaseCapture();
        }

        HWND activeHwnd = PInvoke.GetActiveWindow();
        HandleRef<HWND> ownerHwnd = owner is null ? GetHandleRef(activeHwnd) : GetSafeHandle(owner);

        Form? oldOwner = OwnerInternal;

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
            _dialogResult = DialogResult.None;

            // If "this" is an MDI parent then the window gets activated,
            // causing GetActiveWindow to return "this.handle"... to prevent setting
            // the owner of this to this, we must create the control AFTER calling
            // GetActiveWindow.
            CreateControl();

            if (!ownerHwnd.IsNull && ownerHwnd.Handle != HWND)
            {
                // Catch the case of a window trying to own its owner
                if (PInvoke.GetWindowLong(ownerHwnd.Handle, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT) == Handle)
                {
                    throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner, nameof(ShowDialog)), nameof(owner));
                }

                // In a multi Dpi environment and applications in PMV2 mode, Dpi changed events triggered
                // only when there is a Dpi change happened for the Handle directly or via its parent.
                // So, it is necessary to not set the owner before creating the handle. Otherwise,
                // the window may never receive Dpi changed event even if its parent has different Dpi.
                // Users at runtime, has to move the window between the screens to get the Dpi changed events triggered.

                Properties.SetObject(s_propDialogOwner, owner);
                if (owner is Form form && owner != oldOwner)
                {
                    Owner = form;
                }
                else
                {
                    // Set the new parent.
                    PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, ownerHwnd);
                }
            }

            try
            {
                // If the DialogResult was already set, then there's no need to actually display the dialog.
                if (_dialogResult == DialogResult.None)
                {
                    // Application.RunDialog sets this dialog to be visible.
                    Application.RunDialog(this);
                }
            }
            finally
            {
                // Call SetActiveWindow before setting Visible = false.

                if (!PInvoke.IsWindow(activeHwnd))
                {
                    activeHwnd = ownerHwnd.Handle;
                }

                if (PInvoke.IsWindow(activeHwnd) && PInvoke.IsWindowVisible(activeHwnd))
                {
                    PInvoke.SetActiveWindow(activeHwnd);
                }
                else if (PInvoke.IsWindow(ownerHwnd) && PInvoke.IsWindowVisible(ownerHwnd))
                {
                    PInvoke.SetActiveWindow(ownerHwnd);
                }

                SetVisibleCore(false);
                if (IsHandleCreated)
                {
                    // If this is a dialog opened from an MDI Container, then invalidate
                    // so that child windows will be properly updated.
                    if (OwnerInternal is not null &&
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
            Properties.SetObject(s_propDialogOwner, null);
            GC.KeepAlive(ownerHwnd.Wrapper);
        }

        return DialogResult;
    }

    /// <summary>
    ///  Indicates whether the <see cref="AutoScaleBaseSize"/> property should be
    ///  persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal virtual bool ShouldSerializeAutoScaleBaseSize()
    {
        return _formState[s_formStateAutoScaling] != 0;
    }

    private static bool ShouldSerializeClientSize()
    {
        return true;
    }

    /// <summary>
    ///  Indicates whether the <see cref="Icon"/> property should be persisted.
    /// </summary>
    private bool ShouldSerializeIcon()
    {
        return _formState[s_formStateIconSet] == 1;
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
    ///  Indicates whether the <see cref="Size"/> property should be persisted.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal override bool ShouldSerializeSize()
    {
        return false;
    }

    /// <summary>
    ///  Indicates whether the <see cref="TransparencyKey"/> property should be
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
        if (_formState[s_formStateWindowState] != (int)FormWindowState.Minimized)
        {
            SuspendLayout();
        }
    }

    /// <summary>
    ///  Increments updateMenuHandleSuspendCount.
    /// </summary>
    private void SuspendUpdateMenuHandles()
    {
        int suspendCount = _formStateEx[s_formStateExUpdateMenuHandlesSuspendCount];
        _formStateEx[s_formStateExUpdateMenuHandlesSuspendCount] = ++suspendCount;
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString() => $"{base.ToString()}, Text: {Text}";

    /// <summary>
    ///  Updates the autoscalebasesize based on the current font.
    /// </summary>
    private void UpdateAutoScaleBaseSize()
    {
        _autoScaleBaseSize = Size.Empty;
    }

    private void UpdateRenderSizeGrip()
    {
        int current = _formState[s_formStateRenderSizeGrip];
        switch (FormBorderStyle)
        {
            case FormBorderStyle.None:
            case FormBorderStyle.FixedSingle:
            case FormBorderStyle.Fixed3D:
            case FormBorderStyle.FixedDialog:
            case FormBorderStyle.FixedToolWindow:
                _formState[s_formStateRenderSizeGrip] = 0;
                break;
            case FormBorderStyle.Sizable:
            case FormBorderStyle.SizableToolWindow:
                switch (SizeGripStyle)
                {
                    case SizeGripStyle.Show:
                        _formState[s_formStateRenderSizeGrip] = 1;
                        break;
                    case SizeGripStyle.Hide:
                        _formState[s_formStateRenderSizeGrip] = 0;
                        break;
                    case SizeGripStyle.Auto:
                        if (GetState(States.Modal))
                        {
                            _formState[s_formStateRenderSizeGrip] = 1;
                        }
                        else
                        {
                            _formState[s_formStateRenderSizeGrip] = 0;
                        }

                        break;
                }

                break;
        }

        if (_formState[s_formStateRenderSizeGrip] != current)
        {
            Invalidate();
        }
    }

    protected override void UpdateDefaultButton()
    {
        ContainerControl? containerControl = this;

        while (containerControl.ActiveControl is ContainerControl)
        {
            containerControl = containerControl.ActiveControl as ContainerControl;
            Debug.Assert(containerControl is not null);

            if (containerControl is Form)
            {
                // Don't allow a parent form to get its default button from a child form,
                // otherwise the two forms will 'compete' for the Enter key and produce unpredictable results.
                // This is aimed primarily at fixing the behavior of MDI container forms.
                containerControl = this;
                break;
            }
        }

        if (containerControl.ActiveControl is IButtonControl control)
        {
            SetDefaultButton(control);
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
            IHandle<HWND> ownerHwnd = NullHandle<HWND>.Instance;

            Form? owner = (Form?)Properties.GetObject(s_propOwner);

            if (owner is not null)
            {
                ownerHwnd = owner;
            }
            else
            {
                if (!ShowInTaskbar)
                {
                    ownerHwnd = TaskbarOwner;
                }
            }

            PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_HWNDPARENT, ownerHwnd);
            GC.KeepAlive(ownerHwnd);
        }
    }

    /// <summary>
    ///  Updates the layered window attributes if the control
    ///  is in layered mode.
    /// </summary>
    private void UpdateLayered()
    {
        if ((_formState[s_formStateLayered] != 0) && IsHandleCreated && TopLevel)
        {
            BOOL result;

            Color transparencyKey = TransparencyKey;

            if (transparencyKey.IsEmpty)
            {
                result = PInvoke.SetLayeredWindowAttributes(this, (COLORREF)0, OpacityAsByte, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA);
            }
            else if (OpacityAsByte == 255)
            {
                // Windows doesn't do so well setting colorkey and alpha, so avoid it if we can
                result = PInvoke.SetLayeredWindowAttributes(this, (COLORREF)transparencyKey, 0, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY);
            }
            else
            {
                result = PInvoke.SetLayeredWindowAttributes(this, (COLORREF)transparencyKey, OpacityAsByte, LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_ALPHA | LAYERED_WINDOW_ATTRIBUTES_FLAGS.LWA_COLORKEY);
            }

            if (!result)
            {
                throw new Win32Exception();
            }
        }
    }

    private void UpdateMenuHandles(bool recreateMenu = false)
    {
        if (!IsHandleCreated)
        {
            return;
        }

        if (_ctlClient is null || !_ctlClient.IsHandleCreated)
        {
            PInvoke.SetMenu(this, HMENU.Null);
        }
        else
        {
            Debug.Assert(IsMdiContainer, "Not an MDI container!");
            // when both MainMenuStrip and Menu are set, we honor the win32 menu over
            // the MainMenuStrip as the place to store the system menu controls for the maximized MDI child.

            MenuStrip? mainMenuStrip = MainMenuStrip;
            if (mainMenuStrip is null)
            {
                // We are dealing with a Win32 Menu; MenuStrip doesn't have control buttons.

                // We need to set the "dummy" menu even when a menu is being removed
                // (set to null) so that duplicate control buttons are not placed on the menu bar when
                // an ole menu is being removed.
                // Make MDI forget the mdi item position.
                if (!Properties.TryGetObject(s_propDummyMdiMenu, out HMENU dummyMenu) || dummyMenu.IsNull || recreateMenu)
                {
                    dummyMenu = PInvoke.CreateMenu();
                    Properties.SetObject(s_propDummyMdiMenu, dummyMenu);
                }

                PInvoke.SendMessage(_ctlClient, PInvoke.WM_MDISETMENU, (WPARAM)dummyMenu.Value);
            }

            // (New fix: Only destroy Win32 Menu if using a MenuStrip)
            if (mainMenuStrip is not null)
            {
                // If MainMenuStrip, we need to remove any Win32 Menu to make room for it.
                HMENU hMenu = PInvoke.GetMenu(this);
                if (hMenu != HMENU.Null)
                {
                    // Remove the current menu.
                    PInvoke.SetMenu(this, HMENU.Null);

                    // because we have messed with the child's system menu by shoving in our own dummy menu,
                    // once we clear the main menu we're in trouble - this eats the close, minimize, maximize gadgets
                    // of the child form. (See WM_MDISETMENU in MSDN)
                    Form? activeMdiChild = ActiveMdiChildInternal;
                    if (activeMdiChild is not null && activeMdiChild.WindowState == FormWindowState.Maximized)
                    {
                        activeMdiChild.RecreateHandle();
                    }

                    // Since we're removing a menu but we possibly had a menu previously,
                    // we need to clear the cached size so that new size calculations will be performed correctly.
                    CommonProperties.xClearPreferredSizeCache(this);
                }
            }
        }

        PInvoke.DrawMenuBar(this);
        _formStateEx[s_formStateExUpdateMenuHandlesDeferred] = 0;
    }

    // Call this function instead of UpdateStyles() when the form's client-size must
    // be preserved e.g. when changing the border style.
    //
    internal void UpdateFormStyles()
    {
        Size previousClientSize = ClientSize;
        UpdateStyles();
        if (!ClientSize.Equals(previousClientSize))
        {
            ClientSize = previousClientSize;
        }
    }

    private static Type? FindClosestStockType(Type type)
    {
        Type[] stockTypes = [typeof(MenuStrip)];

        // Add other types here from most specific to most generic if we want to merge other types of toolstrips.

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
        ToolStrip? thisToolstrip = MainMenuStrip;
        List<ToolStrip> childrenToolStrips = ToolStripManager.FindMergeableToolStrips(ActiveMdiChildInternal);

        // revert any previous merge
        if (thisToolstrip is not null)
        {
            ToolStripManager.RevertMerge(thisToolstrip);
        }

        // if someone has a MdiWindowListItem specified we should merge in the
        // names of all the MDI child forms.
        UpdateMdiWindowListStrip();

        if (ActiveMdiChildInternal is not null)
        {
            // do the new merging
            foreach (ToolStrip sourceToolStrip in childrenToolStrips)
            {
                Type? closestMatchingSourceType = FindClosestStockType(sourceToolStrip.GetType());
                if (thisToolstrip is not null)
                {
                    Type? closestMatchingTargetType = FindClosestStockType(thisToolstrip.GetType());
                    if (closestMatchingTargetType is not null && closestMatchingSourceType is not null &&
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
        Form? activeMdiForm = ActiveMdiChildInternal;
        UpdateMdiControlStrip(activeMdiForm is not null && activeMdiForm.IsMaximized);
    }

    private void UpdateMdiControlStrip(bool maximized)
    {
        if (_formStateEx[s_formStateExInUpdateMdiControlStrip] != 0)
        {
            return;
        }

        // we don't want to be redundantly called as we could merge in two control menus.
        _formStateEx[s_formStateExInUpdateMdiControlStrip] = 1;

        try
        {
            MdiControlStrip? mdiControlStrip = MdiControlStrip;

            if (mdiControlStrip is not null)
            {
                if (mdiControlStrip.MergedMenu is not null)
                {
#if DEBUG
                    int numWindowListItems = 0;
                    if (MdiWindowListStrip is not null && MdiWindowListStrip.MergedMenu is not null && MdiWindowListStrip.MergedMenu.MdiWindowListItem is not null)
                    {
                        numWindowListItems = MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count;
                    }
#endif

                    ToolStripManager.RevertMergeInternal(mdiControlStrip.MergedMenu, mdiControlStrip, revertMDIControls: true);

#if DEBUG
                    // double check that RevertMerge doesnt accidentally revert more than it should.
                    if (MdiWindowListStrip is not null && MdiWindowListStrip.MergedMenu is not null && MdiWindowListStrip.MergedMenu.MdiWindowListItem is not null)
                    {
                        Debug.Assert(numWindowListItems == MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count, "Calling RevertMerge modified the mdiwindowlistitem");
                    }
#endif
                }

                mdiControlStrip.MergedMenu = null;
                mdiControlStrip.Dispose();
                MdiControlStrip = null;
            }

            if (ActiveMdiChildInternal is not null && maximized && ActiveMdiChildInternal.ControlBox)
            {
                // Determine if we need to add control gadgets into the MenuStrip.
                // Double check GetMenu incase someone is using interop.
                HMENU hMenu = PInvoke.GetMenu(this);
                if (hMenu == HMENU.Null)
                {
                    MenuStrip? sourceMenuStrip = ToolStripManager.GetMainMenuStrip(this);
                    if (sourceMenuStrip is not null)
                    {
                        MdiControlStrip = new MdiControlStrip(ActiveMdiChildInternal);
                        ToolStripManager.Merge(MdiControlStrip, sourceMenuStrip);
                        MdiControlStrip.MergedMenu = sourceMenuStrip;
                    }
                }
            }
        }
        finally
        {
            _formStateEx[s_formStateExInUpdateMdiControlStrip] = 0;
        }
    }

    internal void UpdateMdiWindowListStrip()
    {
        if (IsMdiContainer)
        {
            if (MdiWindowListStrip is not null && MdiWindowListStrip.MergedMenu is not null)
            {
                ToolStripManager.RevertMergeInternal(MdiWindowListStrip.MergedMenu, MdiWindowListStrip, revertMDIControls: true);
            }

            MenuStrip? sourceMenuStrip = ToolStripManager.GetMainMenuStrip(this);
            if (sourceMenuStrip is not null && sourceMenuStrip.MdiWindowListItem is not null)
            {
                MdiWindowListStrip ??= new MdiWindowListStrip();

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
    ///  Raises the <see cref="ResizeBegin"/>
    ///  event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnResizeBegin(EventArgs e)
    {
        if (CanRaiseEvents)
        {
            ((EventHandler?)Events[s_resizeBeginEvent])?.Invoke(this, e);
        }
    }

    /// <summary>
    ///  Raises the <see cref="ResizeEnd"/>
    ///  event.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnResizeEnd(EventArgs e)
    {
        if (CanRaiseEvents)
        {
            ((EventHandler?)Events[s_resizeEndEvent])?.Invoke(this, e);
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
            Icon? icon;

            // Preserve Win32 behavior by keeping the icon we set NULL if
            // the user hasn't specified an icon and we are a dialog frame.
            if ((FormBorderStyle == FormBorderStyle.FixedDialog && _formState[s_formStateIconSet] == 0) || !ShowIcon)
            {
                icon = null;
            }
            else
            {
                icon = Icon;
            }

            if (icon is not null)
            {
                if (_smallIcon is null)
                {
                    try
                    {
                        _smallIcon = new Icon(icon, SystemInformation.SmallIconSize);
                    }
                    catch
                    {
                    }
                }

                if (_smallIcon is not null)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETICON, (WPARAM)PInvoke.ICON_SMALL, (LPARAM)_smallIcon.Handle);
                }

                PInvoke.SendMessage(this, PInvoke.WM_SETICON, (WPARAM)PInvoke.ICON_BIG, (LPARAM)icon.Handle);
            }
            else
            {
                PInvoke.SendMessage(this, PInvoke.WM_SETICON, (WPARAM)PInvoke.ICON_SMALL);
                PInvoke.SendMessage(this, PInvoke.WM_SETICON, (WPARAM)PInvoke.ICON_BIG);
            }

            if (WindowState == FormWindowState.Maximized && MdiParent?.MdiControlStrip is not null)
            {
                MdiParent.MdiControlStrip.updateIcon();
            }

            if (redrawFrame)
            {
                PInvoke.RedrawWindow(this, lprcUpdate: null, HRGN.Null, REDRAW_WINDOW_FLAGS.RDW_INVALIDATE | REDRAW_WINDOW_FLAGS.RDW_FRAME);
            }
        }
    }

    /// <summary>
    ///  Update the window state from the handle, if created.
    /// </summary>
    private unsafe void UpdateWindowState()
    {
        // This function is called from all over the place, including my personal favorite,
        // WM_ERASEBKGRND.  Seems that's one of the first messages we get when a user clicks the min/max
        // button, even before WM_WINDOWPOSCHANGED.

        if (!IsHandleCreated)
        {
            return;
        }

        FormWindowState oldState = WindowState;
        WINDOWPLACEMENT wp = new()
        {
            length = (uint)sizeof(WINDOWPLACEMENT)
        };
        PInvoke.GetWindowPlacement(HWND, &wp);

        switch (wp.showCmd)
        {
            case SHOW_WINDOW_CMD.SW_NORMAL:
            case SHOW_WINDOW_CMD.SW_RESTORE:
            case SHOW_WINDOW_CMD.SW_SHOW:
            case SHOW_WINDOW_CMD.SW_SHOWNA:
            case SHOW_WINDOW_CMD.SW_SHOWNOACTIVATE:
                if (_formState[s_formStateWindowState] != (int)FormWindowState.Normal)
                {
                    _formState[s_formStateWindowState] = (int)FormWindowState.Normal;
                }

                break;
            case SHOW_WINDOW_CMD.SW_SHOWMAXIMIZED:
                if (_formState[s_formStateMdiChildMax] == 0)
                {
                    _formState[s_formStateWindowState] = (int)FormWindowState.Maximized;
                }

                break;
            case SHOW_WINDOW_CMD.SW_SHOWMINIMIZED:
            case SHOW_WINDOW_CMD.SW_MINIMIZE:
            case SHOW_WINDOW_CMD.SW_SHOWMINNOACTIVE:
                if (_formState[s_formStateMdiChildMax] == 0)
                {
                    _formState[s_formStateWindowState] = (int)FormWindowState.Minimized;
                }

                break;
            case SHOW_WINDOW_CMD.SW_HIDE:
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

            if (!OsVersion.IsWindows11_OrGreater())
            {
                _restoredWindowBounds.Size = ClientSize;
                _formStateEx[s_formStateExWindowBoundsWidthIsClientSize] = 1;
                _formStateEx[s_formStateExWindowBoundsHeightIsClientSize] = 1;
                _restoredWindowBoundsSpecified = BoundsSpecified.Size;
                _restoredWindowBounds.Location = Location;
                _restoredWindowBoundsSpecified |= BoundsSpecified.Location;
            }

            // stash off restoreBounds As well...
            _restoreBounds.Size = Size;
            _restoreBounds.Location = Location;
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

    /// <summary>
    ///  Validates all selectable child controls in the container, including descendants. This is
    ///  equivalent to calling ValidateChildren(ValidationConstraints.Selectable). See <see cref="ValidationConstraints.Selectable"/>
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
        Application.FormActivated(Modal, true);
        Active = m.WParamInternal.LOWORD != PInvoke.WA_INACTIVE;
        Application.FormActivated(Modal, Active);
    }

    /// <summary>
    ///  WM_ENTERSIZEMOVE handler, so that user can hook up OnResizeBegin event.
    /// </summary>
    private void WmEnterSizeMove()
    {
        _formStateEx[s_formStateExInModalSizingLoop] = 1;
        OnResizeBegin(EventArgs.Empty);
    }

    /// <summary>
    ///  WM_EXITSIZEMOVE handler, so that user can hook up OnResizeEnd event.
    /// </summary>
    private void WmExitSizeMove()
    {
        _formStateEx[s_formStateExInModalSizingLoop] = 0;
        OnResizeEnd(EventArgs.Empty);
    }

    /// <summary>
    ///  WM_CREATE handler
    /// </summary>
    private void WmCreate(ref Message m)
    {
        base.WndProc(ref m);
        PInvoke.GetStartupInfo(out STARTUPINFOW si);

        // If we've been created from explorer, it may
        // force us to show up normal.  Force our current window state to
        // the specified state, unless it's _specified_ max or min
        if (TopLevel && (si.dwFlags & STARTUPINFOW_FLAGS.STARTF_USESHOWWINDOW) != 0)
        {
            switch ((SHOW_WINDOW_CMD)si.wShowWindow)
            {
                case SHOW_WINDOW_CMD.SW_MAXIMIZE:
                    WindowState = FormWindowState.Maximized;
                    break;
                case SHOW_WINDOW_CMD.SW_MINIMIZE:
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
        FormClosingEventArgs e = new(CloseReason, false);

        // Pass 1 (WM_CLOSE & WM_QUERYENDSESSION)... Closing
        if (m.Msg != (int)PInvoke.WM_ENDSESSION)
        {
            if (Modal)
            {
                if (_dialogResult == DialogResult.None)
                {
                    _dialogResult = DialogResult.Cancel;
                }

                CalledClosing = false;

                // if this comes back false, someone canceled the close.  we want
                // to call this here so that we can get the cancel event properly,
                // and if this is a WM_QUERYENDSESSION, appropriately set the result
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
                    FormClosingEventArgs fe = new(CloseReason.MdiFormClosing, e.Cancel);
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

                // Always fire OnClosing irrespectively of the validation result
                // Pass the validation result into the EventArgs...

                // Call OnClosing/OnFormClosing on all the forms that current form owns.
                Form[] ownedForms = OwnedForms;
                int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);
                for (int i = ownedFormsCount - 1; i >= 0; i--)
                {
                    FormClosingEventArgs cfe = new(CloseReason.FormOwnerClosing, e.Cancel);
                    if (ownedForms[i] is not null)
                    {
                        // Call OnFormClosing on the child forms.
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

            if (m.MsgInternal == PInvoke.WM_QUERYENDSESSION)
            {
                m.ResultInternal = (LRESULT)(BOOL)!e.Cancel;
            }
            else if (e.Cancel && (MdiParent is not null))
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
            e.Cancel = m.WParamInternal == 0;
        }

        // Pass 2 (WM_CLOSE & WM_ENDSESSION)... Fire closed
        // event on all mdi children and ourselves
        if (m.Msg != (int)PInvoke.WM_QUERYENDSESSION)
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
                int ownedFormsCount = Properties.GetInteger(s_propOwnedFormsCount);
                for (int i = ownedFormsCount - 1; i >= 0; i--)
                {
                    fc = new FormClosedEventArgs(CloseReason.FormOwnerClosing);
                    if (ownedForms[i] is not null)
                    {
                        // Call OnClosed and OnFormClosed on the child forms.
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
        Size minTrack = (AutoSize && _formStateEx[s_formStateExInModalSizingLoop] == 1) ? LayoutUtils.UnionSizes(_minAutoSize, MinimumSize) : MinimumSize;

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
        MINMAXINFO* mmi = (MINMAXINFO*)(nint)m.LParamInternal;
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

        m.ResultInternal = (LRESULT)0;
    }

    /// <summary>
    ///  WM_MDIACTIVATE handler
    /// </summary>
    private void WmMdiActivate(ref Message m)
    {
        base.WndProc(ref m);
        Debug.Assert(Properties.ContainsObjectThatIsNotNull(s_propFormMdiParent), "how is formMdiParent null?");
        Debug.Assert(IsHandleCreated, "how is handle 0?");

        Form? formMdiParent = (Form?)Properties.GetObject(s_propFormMdiParent);

        if (formMdiParent is not null)
        {
            // This message is propagated twice by the MDIClient window. Once to the
            // window being deactivated and once to the window being activated.
            if (HWND == (HWND)m.WParamInternal)
            {
                formMdiParent.DeactivateMdiChild();
            }
            else if (HWND == m.LParamInternal)
            {
                formMdiParent.ActivateMdiChild(this);
            }
        }
    }

    private void WmNcButtonDown(ref Message m)
    {
        if (IsMdiChild)
        {
            if (MdiParentInternal.ActiveMdiChildInternal == this)
            {
                if (ActiveControl is not null && !ActiveControl.ContainsFocus)
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
        if (_ownerWindow is not null)
        {
            _ownerWindow.DestroyHandle();
            _ownerWindow = null;
        }

        if (Modal && _dialogResult == DialogResult.None)
        {
            if (!GetState(States.Recreate))
            {
                DialogResult = DialogResult.Cancel;
            }
        }
    }

    /// <summary>
    ///  WM_NCHITTEST handler.
    /// </summary>
    private void WmNCHitTest(ref Message m)
    {
        if (_formState[s_formStateRenderSizeGrip] != 0)
        {
            // Convert to client coordinates
            Point point = PointToClient(PARAM.ToPoint(m.LParamInternal));

            Size clientSize = ClientSize;

            // If the grip is not fully visible the grip area could overlap with the system control box; we need to disable
            // the grip area in this case not to get in the way of the control box.  We only need to check for the client's
            // height since the window width will be at least the size of the control box which is always bigger than the
            // grip width.
            if (point.X >= (clientSize.Width - SizeGripSize) &&
                point.Y >= (clientSize.Height - SizeGripSize) &&
                clientSize.Height >= SizeGripSize)
            {
                m.ResultInternal = (LRESULT)(nint)(IsMirrored ? PInvoke.HTBOTTOMLEFT : PInvoke.HTBOTTOMRIGHT);
                return;
            }
        }

        base.WndProc(ref m);

        // If we got any of the "edge" hits (bottom, top, topleft, etc),
        // and we're AutoSizeMode.GrowAndShrink, return non-resizable border
        // The edge values are the 8 values from HTLEFT (10) to HTBOTTOMRIGHT (17).
        if (AutoSizeMode == AutoSizeMode.GrowAndShrink)
        {
            int result = (int)m.ResultInternal;
            if (result is >= ((int)PInvoke.HTLEFT) and <= ((int)PInvoke.HTBOTTOMRIGHT))
            {
                m.ResultInternal = (LRESULT)(nint)PInvoke.HTBORDER;
            }
        }
    }

    /// <summary>
    ///  WM_SHOWWINDOW handler
    /// </summary>
    private void WmShowWindow(ref Message m)
    {
        _formState[s_formStateSWCalled] = 1;
        base.WndProc(ref m);
    }

    /// <summary>
    ///  WM_SYSCOMMAND handler
    /// </summary>
    private void WmSysCommand(ref Message m)
    {
        bool callDefault = true;

        uint sc = (uint)(m.WParamInternal.LOWORD & 0xFFF0);
        switch (sc)
        {
            case PInvoke.SC_CLOSE:
                CloseReason = CloseReason.UserClosing;
                if (IsMdiChild && !ControlBox)
                {
                    callDefault = false;
                }

                break;
            case PInvoke.SC_KEYMENU:
                if (IsMdiChild && !ControlBox)
                {
                    callDefault = false;
                }

                break;
            case PInvoke.SC_SIZE:
            case PInvoke.SC_MOVE:
                // Set this before WM_ENTERSIZELOOP because WM_GETMINMAXINFO can be called before WM_ENTERSIZELOOP.
                _formStateEx[s_formStateExInModalSizingLoop] = 1;
                break;
            case PInvoke.SC_CONTEXTHELP:
                CancelEventArgs e = new(false);
                OnHelpButtonClicked(e);
                if (e.Cancel)
                {
                    callDefault = false;
                }

                break;
        }

        if (Command.DispatchID(m.WParamInternal.LOWORD))
        {
            callDefault = false;
        }

        if (callDefault)
        {
            base.WndProc(ref m);
        }
    }

    /// <summary>
    ///  WM_SIZE handler.
    /// </summary>
    private void WmSize(ref Message m)
    {
        // If this is an MDI parent, don't pass WM_SIZE to the default window proc. We handle resizing the
        // MDIClient window ourselves (using ControlDock.FILL).
        if (_ctlClient is null)
        {
            base.WndProc(ref m);
            if (MdiControlStrip is null && MdiParentInternal is not null && MdiParentInternal.ActiveMdiChildInternal == this)
            {
                MdiParentInternal.UpdateMdiControlStrip((nint)m.WParamInternal == PInvoke.SIZE_MAXIMIZED);
            }
        }
    }

    /// <summary>
    ///  WM_WINDOWPOSCHANGED handler
    /// </summary>
    private void WmWindowPosChanged(ref Message m)
    {
        // We must update the windowState, because resize is fired from here (in Control).
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
        switch (m.MsgInternal)
        {
            case PInvoke.WM_NCACTIVATE:
                base.WndProc(ref m);
                break;
            case PInvoke.WM_NCLBUTTONDOWN:
            case PInvoke.WM_NCRBUTTONDOWN:
            case PInvoke.WM_NCMBUTTONDOWN:
            case PInvoke.WM_NCXBUTTONDOWN:
                WmNcButtonDown(ref m);
                break;
            case PInvoke.WM_ACTIVATE:
                WmActivate(ref m);
                break;
            case PInvoke.WM_MDIACTIVATE:
                WmMdiActivate(ref m);
                break;
            case PInvoke.WM_CLOSE:
                if (CloseReason == CloseReason.None)
                {
                    CloseReason = CloseReason.TaskManagerClosing;
                }

                WmClose(ref m);
                break;

            case PInvoke.WM_QUERYENDSESSION:
            case PInvoke.WM_ENDSESSION:
                CloseReason = CloseReason.WindowsShutDown;
                WmClose(ref m);
                break;
            case PInvoke.WM_ENTERSIZEMOVE:
                WmEnterSizeMove();
                DefWndProc(ref m);
                break;
            case PInvoke.WM_EXITSIZEMOVE:
                WmExitSizeMove();
                DefWndProc(ref m);
                break;
            case PInvoke.WM_CREATE:
                WmCreate(ref m);
                break;
            case PInvoke.WM_ERASEBKGND:
                WmEraseBkgnd(ref m);
                break;

            case PInvoke.WM_NCDESTROY:
                WmNCDestroy(ref m);
                break;
            case PInvoke.WM_NCHITTEST:
                WmNCHitTest(ref m);
                break;
            case PInvoke.WM_SHOWWINDOW:
                WmShowWindow(ref m);
                break;
            case PInvoke.WM_SIZE:
                WmSize(ref m);
                break;
            case PInvoke.WM_SYSCOMMAND:
                WmSysCommand(ref m);
                break;
            case PInvoke.WM_GETMINMAXINFO:
                WmGetMinMaxInfo(ref m);
                break;
            case PInvoke.WM_WINDOWPOSCHANGED:
                WmWindowPosChanged(ref m);
                break;
            // case PInvoke.WM_WINDOWPOSCHANGING:
            //    WmWindowPosChanging(ref m);
            //    break;
            case PInvoke.WM_ENTERMENULOOP:
                WmEnterMenuLoop(ref m);
                break;
            case PInvoke.WM_EXITMENULOOP:
                WmExitMenuLoop(ref m);
                break;
            case PInvoke.WM_CAPTURECHANGED:
                base.WndProc(ref m);

                // This is a work-around for the Win32 scroll bar; it doesn't release
                // it's capture in response to a CAPTURECHANGED message, so we force
                // capture away if no button is down.

                if (Capture && MouseButtons == MouseButtons.None)
                {
                    Capture = false;
                }

                break;
            case PInvoke.WM_GETDPISCALEDSIZE:
                WmGetDpiScaledSize(ref m);
                break;
            case PInvoke.WM_DPICHANGED:
                WmDpiChanged(ref m);
                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
