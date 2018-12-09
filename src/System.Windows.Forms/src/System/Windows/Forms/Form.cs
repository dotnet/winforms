// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Globalization;
    using System.Net;
    using System.Reflection;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Security.Policy;
    using System.Threading;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Layout;
    using System.Windows.Forms.VisualStyles;

    /// <include file='doc\Form.uex' path='docs/doc[@for="Form"]/*' />
    /// <devdoc>
    ///    <para>Represents a window or dialog box that makes up an application's user interface.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    ToolboxItemFilter("System.Windows.Forms.Control.TopLevel"),
    ToolboxItem(false),
    DesignTimeVisible(false),
    Designer("System.Windows.Forms.Design.FormDocumentDesigner, " + AssemblyRef.SystemDesign, typeof(IRootDesigner)),
    DesignerCategory("Form"),
    DefaultEvent(nameof(Load)),
    InitializationEvent(nameof(Load)),
    ]
    public class Form : ContainerControl {
#if DEBUG
        static readonly BooleanSwitch AlwaysRestrictWindows = new BooleanSwitch("AlwaysRestrictWindows", "Always make Form classes behave as though they are restricted");
#endif
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
        private static readonly BitVector32.Section FormStateAllowTransparency           = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section FormStateBorderStyle                 = BitVector32.CreateSection(6, FormStateAllowTransparency);
        private static readonly BitVector32.Section FormStateTaskBar                     = BitVector32.CreateSection(1, FormStateBorderStyle);
        private static readonly BitVector32.Section FormStateControlBox                  = BitVector32.CreateSection(1, FormStateTaskBar);
        private static readonly BitVector32.Section FormStateKeyPreview                  = BitVector32.CreateSection(1, FormStateControlBox);
        private static readonly BitVector32.Section FormStateLayered                     = BitVector32.CreateSection(1, FormStateKeyPreview);
        private static readonly BitVector32.Section FormStateMaximizeBox                 = BitVector32.CreateSection(1, FormStateLayered);
        private static readonly BitVector32.Section FormStateMinimizeBox                 = BitVector32.CreateSection(1, FormStateMaximizeBox);
        private static readonly BitVector32.Section FormStateHelpButton                  = BitVector32.CreateSection(1, FormStateMinimizeBox);
        private static readonly BitVector32.Section FormStateStartPos                    = BitVector32.CreateSection(4, FormStateHelpButton);
        private static readonly BitVector32.Section FormStateWindowState                 = BitVector32.CreateSection(2, FormStateStartPos);
        private static readonly BitVector32.Section FormStateShowWindowOnCreate          = BitVector32.CreateSection(1, FormStateWindowState);
        private static readonly BitVector32.Section FormStateAutoScaling                 = BitVector32.CreateSection(1, FormStateShowWindowOnCreate);
        private static readonly BitVector32.Section FormStateSetClientSize               = BitVector32.CreateSection(1, FormStateAutoScaling);
        private static readonly BitVector32.Section FormStateTopMost                     = BitVector32.CreateSection(1, FormStateSetClientSize);
        private static readonly BitVector32.Section FormStateSWCalled                    = BitVector32.CreateSection(1, FormStateTopMost);
        private static readonly BitVector32.Section FormStateMdiChildMax                 = BitVector32.CreateSection(1, FormStateSWCalled);
        private static readonly BitVector32.Section FormStateRenderSizeGrip              = BitVector32.CreateSection(1, FormStateMdiChildMax);
        private static readonly BitVector32.Section FormStateSizeGripStyle               = BitVector32.CreateSection(2, FormStateRenderSizeGrip);
        private static readonly BitVector32.Section FormStateIsRestrictedWindow          = BitVector32.CreateSection(1, FormStateSizeGripStyle);
        private static readonly BitVector32.Section FormStateIsRestrictedWindowChecked   = BitVector32.CreateSection(1, FormStateIsRestrictedWindow);
        private static readonly BitVector32.Section FormStateIsWindowActivated           = BitVector32.CreateSection(1, FormStateIsRestrictedWindowChecked);
        private static readonly BitVector32.Section FormStateIsTextEmpty                 = BitVector32.CreateSection(1, FormStateIsWindowActivated);
        private static readonly BitVector32.Section FormStateIsActive                    = BitVector32.CreateSection(1, FormStateIsTextEmpty);
        private static readonly BitVector32.Section FormStateIconSet                     = BitVector32.CreateSection(1, FormStateIsActive);

#if SECURITY_DIALOG
        private static readonly BitVector32.Section FormStateAddedSecurityMenuItem       = BitVector32.CreateSection(1, FormStateIconSet);
#endif

        //
        // The following flags should be used with formStateEx[...] not formState[..]
        //
        private static readonly BitVector32.Section FormStateExCalledClosing                               = BitVector32.CreateSection(1);
        private static readonly BitVector32.Section FormStateExUpdateMenuHandlesSuspendCount               = BitVector32.CreateSection(8, FormStateExCalledClosing);
        private static readonly BitVector32.Section FormStateExUpdateMenuHandlesDeferred                   = BitVector32.CreateSection(1, FormStateExUpdateMenuHandlesSuspendCount);
        private static readonly BitVector32.Section FormStateExUseMdiChildProc                             = BitVector32.CreateSection(1, FormStateExUpdateMenuHandlesDeferred);
        private static readonly BitVector32.Section FormStateExCalledOnLoad                                = BitVector32.CreateSection(1, FormStateExUseMdiChildProc);
        private static readonly BitVector32.Section FormStateExCalledMakeVisible                           = BitVector32.CreateSection(1, FormStateExCalledOnLoad);
        private static readonly BitVector32.Section FormStateExCalledCreateControl                         = BitVector32.CreateSection(1, FormStateExCalledMakeVisible);
        private static readonly BitVector32.Section FormStateExAutoSize                                    = BitVector32.CreateSection(1, FormStateExCalledCreateControl);
        private static readonly BitVector32.Section FormStateExInUpdateMdiControlStrip                     = BitVector32.CreateSection(1, FormStateExAutoSize);
        private static readonly BitVector32.Section FormStateExShowIcon                                    = BitVector32.CreateSection(1, FormStateExInUpdateMdiControlStrip);
        private static readonly BitVector32.Section FormStateExMnemonicProcessed                           = BitVector32.CreateSection(1, FormStateExShowIcon);
        private static readonly BitVector32.Section FormStateExInScale                                     = BitVector32.CreateSection(1, FormStateExMnemonicProcessed);
        private static readonly BitVector32.Section FormStateExInModalSizingLoop                           = BitVector32.CreateSection(1, FormStateExInScale);
        private static readonly BitVector32.Section FormStateExSettingAutoScale                            = BitVector32.CreateSection(1, FormStateExInModalSizingLoop);
        private static readonly BitVector32.Section FormStateExWindowBoundsWidthIsClientSize               = BitVector32.CreateSection(1, FormStateExSettingAutoScale);
        private static readonly BitVector32.Section FormStateExWindowBoundsHeightIsClientSize              = BitVector32.CreateSection(1, FormStateExWindowBoundsWidthIsClientSize);
        private static readonly BitVector32.Section FormStateExWindowClosing                               = BitVector32.CreateSection(1, FormStateExWindowBoundsHeightIsClientSize);

        private const int SizeGripSize = 16;

        private static Icon defaultIcon = null;
        private static Icon defaultRestrictedIcon = null;
#if MAGIC_PADDING
        private static Padding FormPadding = new Padding(9);  // UI guideline
#endif
        private static object internalSyncObject = new object();

        // Property store keys for properties.  The property store allocates most efficiently
        // in groups of four, so we try to lump properties in groups of four based on how
        // likely they are going to be used in a group.
        //
        private static readonly int PropAcceptButton           = PropertyStore.CreateKey();
        private static readonly int PropCancelButton           = PropertyStore.CreateKey();
        private static readonly int PropDefaultButton          = PropertyStore.CreateKey();
        private static readonly int PropDialogOwner            = PropertyStore.CreateKey();

        private static readonly int PropMainMenu               = PropertyStore.CreateKey();
        private static readonly int PropDummyMenu              = PropertyStore.CreateKey();
        private static readonly int PropCurMenu                = PropertyStore.CreateKey();
        private static readonly int PropMergedMenu             = PropertyStore.CreateKey();

        private static readonly int PropOwner                  = PropertyStore.CreateKey();
        private static readonly int PropOwnedForms             = PropertyStore.CreateKey();
        private static readonly int PropMaximizedBounds        = PropertyStore.CreateKey();
        private static readonly int PropOwnedFormsCount        = PropertyStore.CreateKey();

        private static readonly int PropMinTrackSizeWidth      = PropertyStore.CreateKey();
        private static readonly int PropMinTrackSizeHeight     = PropertyStore.CreateKey();
        private static readonly int PropMaxTrackSizeWidth      = PropertyStore.CreateKey();
        private static readonly int PropMaxTrackSizeHeight     = PropertyStore.CreateKey();

        private static readonly int PropFormMdiParent          = PropertyStore.CreateKey();
        private static readonly int PropActiveMdiChild         = PropertyStore.CreateKey();
        private static readonly int PropFormerlyActiveMdiChild = PropertyStore.CreateKey();
        private static readonly int PropMdiChildFocusable      = PropertyStore.CreateKey();

        private static readonly int PropMainMenuStrip          = PropertyStore.CreateKey();
        private static readonly int PropMdiWindowListStrip     = PropertyStore.CreateKey();
        private static readonly int PropMdiControlStrip        = PropertyStore.CreateKey();
        private static readonly int PropSecurityTip = PropertyStore.CreateKey();

        private static readonly int PropOpacity = PropertyStore.CreateKey();
        private static readonly int PropTransparencyKey = PropertyStore.CreateKey();
#if SECURITY_DIALOG
        private static readonly int PropSecuritySystemMenuItem = PropertyStore.CreateKey();
#endif

        ///////////////////////////////////////////////////////////////////////
        // Form per instance members
        //
        // Note: Do not add anything to this list unless absolutely neccessary.
        //
        // Begin Members {

        // List of properties that are generally set, so we keep them directly on
        // Form.
        //

        private BitVector32      formState = new BitVector32(0x21338);   // magic value... all the defaults... see the ctor for details...
        private BitVector32      formStateEx = new BitVector32();


        private Icon             icon;
        private Icon             smallIcon;
        private Size             autoScaleBaseSize = System.Drawing.Size.Empty;
        private Size             minAutoSize = Size.Empty;
        private Rectangle        restoredWindowBounds = new Rectangle(-1, -1, -1, -1);
        private BoundsSpecified  restoredWindowBoundsSpecified;
        private DialogResult dialogResult;
        private MdiClient        ctlClient;
        private NativeWindow     ownerWindow;
        private string           userWindowText; // Used to cache user's text in semi-trust since the window text is added security info.
        private string           securityZone;
        private string           securitySite;
        private bool             rightToLeftLayout = false;


        //Whidbey RestoreBounds ...
        private Rectangle        restoreBounds = new Rectangle(-1, -1, -1, -1);
        private CloseReason      closeReason = CloseReason.None;

        private VisualStyleRenderer sizeGripRenderer;

        // } End Members
        ///////////////////////////////////////////////////////////////////////


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Form"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.Form'/> class.
        ///    </para>
        /// </devdoc>
        public Form()
        : base() {

            // we must setup the formState *before* calling Control's ctor... so we do that
            // at the member variable... that magic number is generated by switching
            // the line below to "true" and running a form.
            //
            // keep the "init" and "assert" sections always in sync!
            //
#if false
            // init section...
            //
            formState[FormStateAllowTransparency]           = 0;
            formState[FormStateBorderStyle]                 = (int)FormBorderStyle.Sizable;
            formState[FormStateTaskBar]                     = 1;
            formState[FormStateControlBox]                  = 1;
            formState[FormStateKeyPreview]                  = 0;
            formState[FormStateLayered]                     = 0;
            formState[FormStateMaximizeBox]                 = 1;
            formState[FormStateMinimizeBox]                 = 1;
            formState[FormStateHelpButton]                  = 0;
            formState[FormStateStartPos]                    = (int)FormStartPosition.WindowsDefaultLocation;
            formState[FormStateWindowState]                 = (int)FormWindowState.Normal;
            formState[FormStateShowWindowOnCreate]          = 0;
            formState[FormStateAutoScaling]                 = 1;
            formState[FormStateSetClientSize]               = 0;
            formState[FormStateTopMost]                     = 0;
            formState[FormStateSWCalled]                    = 0;
            formState[FormStateMdiChildMax]                 = 0;
            formState[FormStateRenderSizeGrip]              = 0;
            formState[FormStateSizeGripStyle]               = 0;
            formState[FormStateIsRestrictedWindow]          = 0;
            formState[FormStateIsRestrictedWindowChecked]   = 0;
            formState[FormStateIsWindowActivated]           = 0;
            formState[FormStateIsTextEmpty]                 = 0;
            formState[FormStateIsActive]                    = 0;
            formState[FormStateIconSet]                     = 0;

#if SECURITY_DIALOG
            formState[FormStateAddedSecurityMenuItem]       = 0;

#endif




            Debug.WriteLine("initial formState: 0x" + formState.Data.ToString("X"));
#endif
            // assert section...
            //
            Debug.Assert(formState[FormStateAllowTransparency]           == 0, "Failed to set formState[FormStateAllowTransparency]");
            Debug.Assert(formState[FormStateBorderStyle]                 == (int)FormBorderStyle.Sizable, "Failed to set formState[FormStateBorderStyle]");
            Debug.Assert(formState[FormStateTaskBar]                     == 1, "Failed to set formState[FormStateTaskBar]");
            Debug.Assert(formState[FormStateControlBox]                  == 1, "Failed to set formState[FormStateControlBox]");
            Debug.Assert(formState[FormStateKeyPreview]                  == 0, "Failed to set formState[FormStateKeyPreview]");
            Debug.Assert(formState[FormStateLayered]                     == 0, "Failed to set formState[FormStateLayered]");
            Debug.Assert(formState[FormStateMaximizeBox]                 == 1, "Failed to set formState[FormStateMaximizeBox]");
            Debug.Assert(formState[FormStateMinimizeBox]                 == 1, "Failed to set formState[FormStateMinimizeBox]");
            Debug.Assert(formState[FormStateHelpButton]                  == 0, "Failed to set formState[FormStateHelpButton]");
            Debug.Assert(formState[FormStateStartPos]                    == (int)FormStartPosition.WindowsDefaultLocation, "Failed to set formState[FormStateStartPos]");
            Debug.Assert(formState[FormStateWindowState]                 == (int)FormWindowState.Normal, "Failed to set formState[FormStateWindowState]");
            Debug.Assert(formState[FormStateShowWindowOnCreate]          == 0, "Failed to set formState[FormStateShowWindowOnCreate]");
            Debug.Assert(formState[FormStateAutoScaling]                 == 1, "Failed to set formState[FormStateAutoScaling]");
            Debug.Assert(formState[FormStateSetClientSize]               == 0, "Failed to set formState[FormStateSetClientSize]");
            Debug.Assert(formState[FormStateTopMost]                     == 0, "Failed to set formState[FormStateTopMost]");
            Debug.Assert(formState[FormStateSWCalled]                    == 0, "Failed to set formState[FormStateSWCalled]");
            Debug.Assert(formState[FormStateMdiChildMax]                 == 0, "Failed to set formState[FormStateMdiChildMax]");
            Debug.Assert(formState[FormStateRenderSizeGrip]              == 0, "Failed to set formState[FormStateRenderSizeGrip]");
            Debug.Assert(formState[FormStateSizeGripStyle]               == 0, "Failed to set formState[FormStateSizeGripStyle]");
            // can't check these... Control::.ctor may force the check
            // of security... you can only assert these are 0 when running
            // under full trust...
            //
            //Debug.Assert(formState[FormStateIsRestrictedWindow]          == 0, "Failed to set formState[FormStateIsRestrictedWindow]");
            //Debug.Assert(formState[FormStateIsRestrictedWindowChecked]   == 0, "Failed to set formState[FormStateIsRestrictedWindowChecked]");
            Debug.Assert(formState[FormStateIsWindowActivated]           == 0, "Failed to set formState[FormStateIsWindowActivated]");
            Debug.Assert(formState[FormStateIsTextEmpty]                 == 0, "Failed to set formState[FormStateIsTextEmpty]");
            Debug.Assert(formState[FormStateIsActive]                    == 0, "Failed to set formState[FormStateIsActive]");
            Debug.Assert(formState[FormStateIconSet]                     == 0, "Failed to set formState[FormStateIconSet]");


#if SECURITY_DIALOG
            Debug.Assert(formState[FormStateAddedSecurityMenuItem]       == 0, "Failed to set formState[FormStateAddedSecurityMenuItem]");
#endif

            // SECURITY NOTE: The IsRestrictedWindow check is done once and cached. We force it to happen here
            // since we want to ensure the check is done on the code that constructs the form.
            bool temp = IsRestrictedWindow;

            formStateEx[FormStateExShowIcon]                             = 1;

            SetState(STATE_VISIBLE, false);
            SetState(STATE_TOPLEVEL, true);

#if EVERETTROLLBACK
            // (MDI: Roll back feature to Everett + QFE source base).  Code left here for ref.
            // Enabling this code introduces a breaking change that has was approved. 
            // If this needs to be enabled, also CanTabStop and TabStop code needs to be added back in Control.cs
            // and Form.cs.
            
            // Set this value to false
            // so that the window style will not include the WS_TABSTOP bit, which is
            // identical to WS_MAXIMIZEBOX. Otherwise, our test suite won't be able to
            // determine whether or not the window utilizes the Maximize Box in the
            // window caption.
            SetState(STATE_TABSTOP, false);
#endif
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AcceptButton"]/*' />
        /// <devdoc>
        /// <para>Indicates the <see cref='System.Windows.Forms.Button'/> control on the form that is clicked when
        ///    the user presses the ENTER key.</para>
        /// </devdoc>
        [
        DefaultValue(null),
        SRDescription(nameof(SR.FormAcceptButtonDescr))
        ]
        public IButtonControl AcceptButton {
            get {
                return (IButtonControl)Properties.GetObject(PropAcceptButton);
            }
            set {
                if (AcceptButton != value) {
                    Properties.SetObject(PropAcceptButton, value);
                    UpdateDefaultButton();

                    // this was removed as it breaks any accept button that isn't
                    // an OK, like in the case of wizards 'next' button. 
                    /*
                    if (acceptButton != null && acceptButton.DialogResult == DialogResult.None) {
                        acceptButton.DialogResult = DialogResult.OK;
                    }
                    */
                }
            } 
        }

        /// <devdoc>
        ///     Retrieves true if this form is currently active.
        /// </devdoc>
        internal bool Active {
            get {
                Form parentForm = ParentFormInternal;
                if (parentForm == null) {
                    return formState[FormStateIsActive] != 0;
                }
                return(parentForm.ActiveControl == this && parentForm.Active);
            }

            set {
                Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Form::set_Active - " + this.Name);
                if ((formState[FormStateIsActive] != 0) != value) {
                    if (value) {
                        // There is a weird user32 



                        if (!CanRecreateHandle()){
                            //Debug.Fail("Setting Active window when not yet visible");
                            return;
                        }
                    }

                    formState[FormStateIsActive] = value ? 1 : 0;

                    if (value) {
                        formState[FormStateIsWindowActivated] = 1;
                        if (IsRestrictedWindow) {
                            WindowText = userWindowText;
                        }
                        // Check if validation has been cancelled to avoid raising Validation event multiple times.
                        if (!ValidationCancelled) {
                            if( ActiveControl == null ) {
                                // Security reviewed : This internal method is called from various places, all
                                // of which are OK. Since SelectNextControl (a public function)
                                // Demands ModifyFocus, we must call the internal version.
                                //
                                SelectNextControlInternal(null, true, true, true, false);
                                // If no control is selected focus will go to form
                            }

                            InnerMostActiveContainerControl.FocusActiveControlInternal();
                        }
                        OnActivated(EventArgs.Empty);
                     }
                    else {
                        formState[FormStateIsWindowActivated] = 0;
                        if (IsRestrictedWindow) {
                            Text = userWindowText;
                        }
                        OnDeactivate(EventArgs.Empty);
                    }
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ActiveForm"]/*' />
        /// <devdoc>
        ///    <para> Gets the currently active form for this application.</para>
        /// </devdoc>
        public static Form ActiveForm {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "GetParent Demanded");
                IntSecurity.GetParent.Demand();

                IntPtr hwnd = UnsafeNativeMethods.GetForegroundWindow();
                Control c = Control.FromHandleInternal(hwnd);
                if (c != null && c is Form) {
                    return(Form)c;
                }
                return null;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ActiveMdiChild"]/*' />
        /// <devdoc>
        ///    <para> 
        ///         Gets the currently active multiple document interface (MDI) child window.
        ///         Note: Don't use this property internally, use ActiveMdiChildInternal instead (see comments below).
        ///    </para>
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormActiveMDIChildDescr))
        ]
        public Form ActiveMdiChild {
            get {
                Form mdiChild = ActiveMdiChildInternal;

                // We keep the active mdi child in the cached in the property store; when changing its value 
                // (due to a change to one of the following properties/methods: Visible, Enabled, Active, Show/Hide, 
                // Focus() or as the  result of WM_SETFOCUS/WM_ACTIVATE/WM_MDIACTIVATE) we temporarily set it to null 
                // (to properly handle menu merging among other things) rendering the cache out-of-date; the problem 
                // arises when the user has an event handler that is raised during this process; in that case we ask 
                // Windows for it (see ActiveMdiChildFromWindows).
                
                if( mdiChild == null ){
                    // If this.MdiClient != null it means this.IsMdiContainer == true.
                    if( this.ctlClient != null && this.ctlClient.IsHandleCreated){
                        IntPtr hwnd = this.ctlClient.SendMessage(NativeMethods.WM_MDIGETACTIVE, 0, 0);
                        mdiChild = Control.FromHandleInternal( hwnd ) as Form;
                    }
                }
                if( mdiChild != null && mdiChild.Visible && mdiChild.Enabled ){
                    return mdiChild;
                }
                return null;
            }
        }

        /// <devdoc>
        ///    Property to be used internally.  See comments a on ActiveMdiChild property.
        /// </devdoc>
        internal Form ActiveMdiChildInternal{
            get{
                return (Form)Properties.GetObject(PropActiveMdiChild);
            }

            set{
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

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AllowTransparency"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       a value indicating whether the opacity of the form can be
        ///       adjusted.</para>
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlAllowTransparencyDescr))
        ]
        public bool AllowTransparency {
            get {
                return formState[FormStateAllowTransparency] != 0;
            }
            set {
                if (value != (formState[FormStateAllowTransparency] != 0) &&
                    OSFeature.Feature.IsPresent(OSFeature.LayeredWindows)) {
                    formState[FormStateAllowTransparency] = (value ? 1 : 0);

                    formState[FormStateLayered] = formState[FormStateAllowTransparency];

                    UpdateStyles();

                    if (!value) {
                        if (Properties.ContainsObject(PropOpacity)) {
                            Properties.SetObject(PropOpacity, (object)1.0f);
                        }
                        if (Properties.ContainsObject(PropTransparencyKey)) {
                            Properties.SetObject(PropTransparencyKey, Color.Empty);
                        }
                        UpdateLayered();
                    }
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoScale"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the form will adjust its size
        ///       to fit the height of the font used on the form and scale
        ///       its controls.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.FormAutoScaleDescr)),
        Obsolete("This property has been deprecated. Use the AutoScaleMode property instead.  http://go.microsoft.com/fwlink/?linkid=14202"),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public bool AutoScale {
            get {
                return formState[FormStateAutoScaling] != 0;
            }

            set {
                formStateEx[FormStateExSettingAutoScale] = 1;
                try {
                    if (value) {
                        formState[FormStateAutoScaling] = 1;

                        // if someone insists on auto scaling,
                        // force the new property back to none so they
                        // don't compete.
                        AutoScaleMode = AutoScaleMode.None;
                       

                    }
                    else {
                        formState[FormStateAutoScaling] = 0;
                    }
                }
                finally {
                    formStateEx[FormStateExSettingAutoScale] = 0;
                }
            }
        }

// Our STRONG recommendation to customers is to upgrade to AutoScaleDimensions
// however, since this is generated by default in Everett, and there's not a 1:1 mapping of 
// the old to the new, we are un-obsoleting the setter for AutoScaleBaseSize only.
#pragma warning disable 618
        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoScaleBaseSize"]/*' />
        /// <devdoc>
        ///     The base size used for autoscaling. The AutoScaleBaseSize is used
        ///     internally to determine how much to scale the form when AutoScaling is
        ///     used.
        /// </devdoc>
        //
        // Virtual so subclasses like PrintPreviewDialog can prevent changes.
        [
        Localizable(true),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public virtual Size AutoScaleBaseSize {            
            get {
                if (autoScaleBaseSize.IsEmpty) {
                    SizeF real = GetAutoScaleSize(Font);
                    return new Size((int)Math.Round(real.Width), (int)Math.Round(real.Height));
                }
                return autoScaleBaseSize;
            }

            set {
                // Only allow the set when not in designmode, this prevents us from
                // preserving an old value.  The form design should prevent this for
                // us by shadowing this property, so we just assert that the designer
                // is doing its job.
                //
                Debug.Assert(!DesignMode, "Form designer should not allow base size set in design mode.");
                autoScaleBaseSize = value;
            }
        }
#pragma warning restore 618


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the form implements
        ///       autoscrolling.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true)
        ]
        public override bool AutoScroll {
            get { return base.AutoScroll;}

            set {
                if (value) {
                    IsMdiContainer = false;
                }
                base.AutoScroll = value;
            }
        }

        // Forms implement their own AutoSize in OnLayout so we shadow this property
        // just in case someone parents a Form to a container control.
        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoSize"]/*' />
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public override bool AutoSize {
            get { return formStateEx[FormStateExAutoSize] != 0; }
            set {
                if (value != AutoSize) {
                    formStateEx[FormStateExAutoSize] = value ? 1 : 0;
                    if (!AutoSize) {
                        minAutoSize = Size.Empty;
                        // If we just disabled AutoSize, restore the original size.
                        this.Size = CommonProperties.GetSpecifiedBounds(this).Size;
                    }
                    LayoutTransaction.DoLayout(this, this, PropertyNames.AutoSize);
                    OnAutoSizeChanged(EventArgs.Empty);
                }
                Debug.Assert(AutoSize == value, "Error detected setting Form.AutoSize.");
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoSizeChanged"]/*' />
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
        public AutoSizeMode AutoSizeMode {
            get {
                return GetAutoSizeMode();
            }
            set {

                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoSizeMode.GrowAndShrink, (int)AutoSizeMode.GrowOnly)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoSizeMode));
                }
                
                if (GetAutoSizeMode() != value) {
                    SetAutoSizeMode(value);
                    Control toLayout = DesignMode || ParentInternal == null ? this : ParentInternal;

                    if(toLayout != null) {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if(toLayout.LayoutEngine == DefaultLayout.Instance) {
                            toLayout.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(toLayout, this, PropertyNames.AutoSize);
                    }
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoValidate"]/*' />
        /// <devdoc>
        ///     Indicates whether controls in this container will be automatically validated when the focus changes.
        /// </devdoc>
        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        ]
        public override AutoValidate AutoValidate {
            get {
                return base.AutoValidate;
            }
            set {
                base.AutoValidate = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AutoValidateChanged"]/*' />
        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        ]
        public new event EventHandler AutoValidateChanged {
            add {
                base.AutoValidateChanged += value;
            }
            remove {
                base.AutoValidateChanged -= value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.BackColor"]/*' />
        /// <devdoc>
        ///     The background color of this control. This is an ambient property and
        ///     will always return a non-null value.
        /// </devdoc>
        public override Color BackColor {
            get {
                // Forms should not inherit BackColor from their parent,
                // particularly if the parent is an MDIClient.
                Color c = RawBackColor; // inheritedProperties.BackColor
                if (!c.IsEmpty)
                    return c;

                return DefaultBackColor;
            }

            set {
                base.BackColor = value;
            }
        }

        private bool CalledClosing {
            get{
                return formStateEx[FormStateExCalledClosing] != 0;
            }
            set{
                formStateEx[FormStateExCalledClosing] = (value ? 1 : 0);
            }
        }

        private bool CalledCreateControl {
            get{
                return formStateEx[FormStateExCalledCreateControl] != 0;
            }
            set{
                formStateEx[FormStateExCalledCreateControl] = (value ? 1 : 0);
            }
        }

        private bool CalledMakeVisible {
            get{
                return formStateEx[FormStateExCalledMakeVisible] != 0;
            }
            set{
                formStateEx[FormStateExCalledMakeVisible] = (value ? 1 : 0);
            }
        }

        private bool CalledOnLoad {
            get{
                return formStateEx[FormStateExCalledOnLoad] != 0;
            }
            set{
                formStateEx[FormStateExCalledOnLoad] = (value ? 1 : 0);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.FormBorderStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the border style of the form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(FormBorderStyle.Sizable),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.FormBorderStyleDescr))
        ]
        public FormBorderStyle FormBorderStyle {
            get {
                return(FormBorderStyle)formState[FormStateBorderStyle];
            }

            set {
                //validate FormBorderStyle enum
                //
                //valid values are 0x0 to 0x6
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FormBorderStyle.None, (int)FormBorderStyle.SizableToolWindow))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormBorderStyle));
                }

                // In rectricted mode we don't allow windows w/o min/max/close functionality.
                if (IsRestrictedWindow) {
                    switch (value) {
                        case FormBorderStyle.None:
                            value = FormBorderStyle.FixedSingle;
                            break;
                        case FormBorderStyle.FixedSingle:
                        case FormBorderStyle.Fixed3D:
                        case FormBorderStyle.FixedDialog:
                        case FormBorderStyle.Sizable:
                            // nothing needed here, we can just let these stay
                            //
                            break;
                        case FormBorderStyle.FixedToolWindow:
                            value = FormBorderStyle.FixedSingle;
                            break;
                        case FormBorderStyle.SizableToolWindow:
                            value = FormBorderStyle.Sizable;
                            break;
                        default:
                            value = FormBorderStyle.Sizable;
                            break;
                    }
                }

                formState[FormStateBorderStyle] = (int)value;

                //(

                if (formState[FormStateSetClientSize] == 1 && !IsHandleCreated) {
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
                Rectangle       preClientUpdateRestoredWindowBounds = restoredWindowBounds;
                BoundsSpecified preClientUpdateRestoredWindowBoundsSpecified = restoredWindowBoundsSpecified;
                int preWindowBoundsWidthIsClientSize = formStateEx[FormStateExWindowBoundsWidthIsClientSize];
                int preWindowBoundsHeightIsClientSize = formStateEx[FormStateExWindowBoundsHeightIsClientSize];

                UpdateFormStyles();

                // In Windows XP Theme, the FixedDialog tend to have a small Icon.
                // So to make this behave uniformly with other styles, we need to make
                // the call to UpdateIcon after the the form styles have been updated.
                if (formState[FormStateIconSet] == 0 && !IsRestrictedWindow) {
                    UpdateWindowIcon(false);
                }

                // Now restore the values cached above.
                if (WindowState != FormWindowState.Normal) {
                    restoredWindowBounds = preClientUpdateRestoredWindowBounds;
                    restoredWindowBoundsSpecified = preClientUpdateRestoredWindowBoundsSpecified;
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] = preWindowBoundsWidthIsClientSize;
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] = preWindowBoundsHeightIsClientSize;
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.CancelButton"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or
        ///       sets the button control that will be clicked when the
        ///       user presses the ESC key.</para>
        /// </devdoc>
        [
        DefaultValue(null),
        SRDescription(nameof(SR.FormCancelButtonDescr))
        ]
        public IButtonControl CancelButton {
            get {
                return (IButtonControl)Properties.GetObject(PropCancelButton);
            }
            set {
                Properties.SetObject(PropCancelButton, value);

                if (value != null && value.DialogResult == DialogResult.None) {
                    value.DialogResult = DialogResult.Cancel;
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ClientSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the size of the client area of the form.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true), 
        DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)
        ]
        new public Size ClientSize {
            get {
                return base.ClientSize;
            }
            set {
                base.ClientSize = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ControlBox"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether a control box is displayed in the
        ///       caption bar of the form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(true),
        SRDescription(nameof(SR.FormControlBoxDescr))
        ]
        public bool ControlBox {
            get {
                return formState[FormStateControlBox] != 0;
            }

            set {
                // Window style in restricted mode must always have a control box.
                if (IsRestrictedWindow) {
                    return;
                }

                if (value) {
                    formState[FormStateControlBox] = 1;
                }
                else {
                    formState[FormStateControlBox] = 0;
                }
                UpdateFormStyles();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Retrieves the CreateParams used to create the window.
        ///    If a subclass overrides this function, it must call the base implementation.
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;

                if (this.IsHandleCreated && (this.WindowStyle & NativeMethods.WS_DISABLED) != 0)
                {
                    // Forms that are parent of a modal dialog must keep their WS_DISABLED style
                    cp.Style |= NativeMethods.WS_DISABLED;
                }
                else if (TopLevel)
                {
                    // It doesn't seem to make sense to allow a top-level form to be disabled
                    //
                    cp.Style &= (~NativeMethods.WS_DISABLED);
                }

                if (TopLevel && (formState[FormStateLayered] != 0)) {
                    cp.ExStyle |= NativeMethods.WS_EX_LAYERED;
                }

                IWin32Window dialogOwner = (IWin32Window)Properties.GetObject(PropDialogOwner);
                if (dialogOwner != null) {
                    cp.Parent = Control.GetSafeHandle(dialogOwner);
                }

                FillInCreateParamsBorderStyles(cp);
                FillInCreateParamsWindowState(cp);
                FillInCreateParamsBorderIcons(cp);

                if (formState[FormStateTaskBar] != 0) {
                    cp.ExStyle |= NativeMethods.WS_EX_APPWINDOW;
                }

                FormBorderStyle borderStyle = FormBorderStyle;
                if (!ShowIcon &&
                    (borderStyle == FormBorderStyle.Sizable ||
                     borderStyle == FormBorderStyle.Fixed3D ||
                     borderStyle == FormBorderStyle.FixedSingle))
                {
                    cp.ExStyle |= NativeMethods.WS_EX_DLGMODALFRAME;
                }

                if (IsMdiChild) {
                    if (Visible
                            && (WindowState == FormWindowState.Maximized
                                || WindowState == FormWindowState.Normal)) {
                        Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                        Form form = formMdiParent.ActiveMdiChildInternal;

                        if (form != null
                            && form.WindowState == FormWindowState.Maximized) {
                            cp.Style |= NativeMethods.WS_MAXIMIZE;
                            formState[FormStateWindowState] = (int)FormWindowState.Maximized;
                            SetState(STATE_SIZELOCKEDBYOS, true);
                        }
                    }

                    if (formState[FormStateMdiChildMax] != 0) {
                        cp.Style |= NativeMethods.WS_MAXIMIZE;
                    }
                    cp.ExStyle |= NativeMethods.WS_EX_MDICHILD;
                }

                if (TopLevel || IsMdiChild) {
                    FillInCreateParamsStartPosition(cp);
                    // Delay setting to visible until after the handle gets created
                    // to allow applyClientSize to adjust the size before displaying
                    // the form.
                    //
                    if ((cp.Style & NativeMethods.WS_VISIBLE) != 0) {
                        formState[FormStateShowWindowOnCreate] = 1;
                        cp.Style &= (~NativeMethods.WS_VISIBLE);
                    }
                    else {
                        formState[FormStateShowWindowOnCreate] = 0;
                    }
                }

                if (IsRestrictedWindow) {
                    cp.Caption = RestrictedWindowText(cp.Caption);
                }

                if (RightToLeft == RightToLeft.Yes && RightToLeftLayout == true) {
                    //We want to turn on mirroring for Form explicitly.
                    cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL | NativeMethods.WS_EX_NOINHERITLAYOUT;
                    //Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                }
                return cp;
            }
        }

        internal CloseReason CloseReason {
            get { return closeReason; }
            set { closeReason = value; }
        }
        /// <devdoc>
        ///     The default icon used by the Form. This is the standard "windows forms" icon.
        /// </devdoc>
        internal static Icon DefaultIcon {
            get {
                // Avoid locking if the value is filled in...
                //
                if (defaultIcon == null) {
                    lock(internalSyncObject) {
                        // Once we grab the lock, we re-check the value to avoid a
                        // race condition.
                        //
                        if (defaultIcon == null) {
                            defaultIcon = new Icon(typeof(Form), "wfc.ico");
                        }
                    }
                }
                return defaultIcon;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DefaultImeMode"]/*' />
        protected override ImeMode DefaultImeMode {
            get {
                return ImeMode.NoControl;
            }
        }

        /// <devdoc>
        ///     The default icon used by the Form. This is the standard "windows forms" icon.
        /// </devdoc>
        private static Icon DefaultRestrictedIcon {
            get {
                // Note: We do this as a static property to allow delay
                // loading of the resource. There are some issues with doing
                // an OleInitialize from a static constructor...
                //

                // Avoid locking if the value is filled in...
                //
                if (defaultRestrictedIcon == null) {
                    lock (internalSyncObject)
                    {
                        // Once we grab the lock, we re-check the value to avoid a
                        // race condition.
                        //
                        if (defaultRestrictedIcon == null) {
                            defaultRestrictedIcon = new Icon(typeof(Form), "wfsecurity.ico");
                        }
                    }
                }
                return defaultRestrictedIcon;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(300, 300);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DesktopBounds"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the size and location of the form on the Windows desktop.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormDesktopBoundsDescr))
        ]
        public Rectangle DesktopBounds {
            get {
                Rectangle screen = SystemInformation.WorkingArea;
                Rectangle bounds = Bounds;
                bounds.X -= screen.X;
                bounds.Y -= screen.Y;
                return bounds;
            }

            set {
                SetDesktopBounds(value.X, value.Y, value.Width, value.Height);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DesktopLocation"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the location of the form on the Windows desktop.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormDesktopLocationDescr))
        ]
        public Point DesktopLocation {
            get {
                Rectangle screen = SystemInformation.WorkingArea;
                Point loc = Location;
                loc.X -= screen.X;
                loc.Y -= screen.Y;
                return loc;
            }

            set {
                SetDesktopLocation(value.X, value.Y);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DialogResult"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the dialog result for the form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormDialogResultDescr))
        ]
        public DialogResult DialogResult {
            get {
                return dialogResult;
            }

            set {
                //valid values are 0x0 to 0x7
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)DialogResult.None, (int)DialogResult.No))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DialogResult));
                }

                dialogResult = value;
            }
        }

        internal override bool HasMenu {
            get {
                bool hasMenu = false;

                // Verify that the menu actually contains items so that any
                // size calculations will only include a menu height if the menu actually exists.
                // Note that Windows will not draw a menu bar for a menu that does not contain
                // any items.
                Menu menu = Menu;
                if (TopLevel && menu != null && menu.ItemCount > 0) {
                    hasMenu = true;
                }
                return hasMenu;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.HelpButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether a
        ///       help button should be displayed in the caption box of the form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(false),
        SRDescription(nameof(SR.FormHelpButtonDescr))
        ]
        public bool HelpButton {
            get {
                return formState[FormStateHelpButton] != 0;
            }

            set {
                if (value) {
                    formState[FormStateHelpButton] = 1;
                }
                else {
                    formState[FormStateHelpButton] = 0;
                }
                UpdateFormStyles();
            }
        }

        [
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Always),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.FormHelpButtonClickedDescr))
        ]
        public event CancelEventHandler HelpButtonClicked {
            add {
                Events.AddHandler(EVENT_HELPBUTTONCLICKED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_HELPBUTTONCLICKED, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Icon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the icon for the form.
        ///    </para>
        /// </devdoc>
        [
        AmbientValue(null),
        Localizable(true),
        SRCategory(nameof(SR.CatWindowStyle)),
        SRDescription(nameof(SR.FormIconDescr))
        ]
        public Icon Icon {
            get {
                if (formState[FormStateIconSet] == 0) {
                    // In restricted mode, the security icon cannot be changed.
                    if (IsRestrictedWindow) {
                        return DefaultRestrictedIcon;
                    }
                    else {
                        return DefaultIcon;
                    }
                }

                return icon;
            }

            set {
                if (icon != value && !IsRestrictedWindow) {

                    // If the user is poking the default back in,
                    // treat this as a null (reset).
                    //
                    if (value == defaultIcon) {
                        value = null;
                    }

                    // And if null is passed, reset the icon.
                    //
                    formState[FormStateIconSet] = (value == null ? 0 : 1);
                    this.icon = value;

                    if (smallIcon != null) {
                        smallIcon.Dispose();
                        smallIcon = null;
                    }

                    UpdateWindowIcon(true);
                }
            }
        }

        /// <summary>
        ///     Determines whether the window is closing.
        /// </summary>
        private bool IsClosing {
            get {
                return formStateEx[FormStateExWindowClosing] == 1;
            }
            set {
                formStateEx[FormStateExWindowClosing] = value ? 1 : 0;
            }
        }

        // Returns a more accurate statement of whether or not the form is maximized.
        // during handle creation, an MDIChild is created as not maximized.
        private bool IsMaximized {
            get { 
                return (WindowState == FormWindowState.Maximized || (IsMdiChild && (formState[FormStateMdiChildMax] ==1))); 
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.IsMdiChild"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether the form is a multiple document
        ///       interface (MDI) child form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormIsMDIChildDescr))
        ]
        public bool IsMdiChild {
            get {
                return (Properties.GetObject(PropFormMdiParent) != null);
            }
        }

        // Deactivates active MDI child and temporarily marks it as unfocusable,
        // so that WM_SETFOCUS sent to MDIClient does not activate that child. (See MdiClient.WndProc).
        internal bool IsMdiChildFocusable {
            get {
                if (this.Properties.ContainsObject(PropMdiChildFocusable)) {
                    return (bool) this.Properties.GetObject(PropMdiChildFocusable);
                }
                return false;
            }
            set {
                if (value != this.IsMdiChildFocusable) {
                    this.Properties.SetObject(PropMdiChildFocusable, value);
                }
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.IsMdiContainer"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the form is a container for multiple document interface
        ///       (MDI) child forms.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(false),
        SRDescription(nameof(SR.FormIsMDIContainerDescr))
        ]
        public bool IsMdiContainer {
            get {
                return ctlClient != null;
            }

            set {
                if (value == IsMdiContainer)
                    return;

                if (value) {
                    Debug.Assert(ctlClient == null, "why isn't ctlClient null");
                    AllowTransparency = false;
                    Controls.Add(new MdiClient());
                }
                else {
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

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.IsRestrictedWindow"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para> Determines if this form should display a warning banner
        ///       when the form is displayed in an unsecure mode.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool IsRestrictedWindow {
            get {
                /// 
                if (formState[FormStateIsRestrictedWindowChecked] == 0) {
                    formState[FormStateIsRestrictedWindow] = 0;
                    Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "Checking for restricted window...");
                    Debug.Indent();
#if DEBUG
                    if (AlwaysRestrictWindows.Enabled) {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "Always restricted switch is on...");
                        formState[FormStateIsRestrictedWindow] = 1;
                        formState[FormStateIsRestrictedWindowChecked] = 1;
                        Debug.Unindent();
                        return true;
                    }
#endif

                    try {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "WindowAdornmentModification Demanded");
                        IntSecurity.WindowAdornmentModification.Demand();
                    }
                    catch (SecurityException) {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "Caught security exception, we are restricted...");
                        formState[FormStateIsRestrictedWindow] = 1;
                    }
                    catch {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "Caught non-security exception...");
                        Debug.Unindent();
                        formState[FormStateIsRestrictedWindow] = 1; // To be on the safe side
                        formState[FormStateIsRestrictedWindowChecked] = 1;
                        throw;
                    }
                    Debug.Unindent();
                    formState[FormStateIsRestrictedWindowChecked] = 1;
                }

                return formState[FormStateIsRestrictedWindow] != 0;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.KeyPreview"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value
        ///       indicating whether the form will receive key events
        ///       before the event is passed to the control that has focus.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.FormKeyPreviewDescr))
        ]
        public bool KeyPreview {
            get {
                return formState[FormStateKeyPreview] != 0;
            }
            set {
                if (value) {
                    formState[FormStateKeyPreview] = 1;
                }
                else {
                    formState[FormStateKeyPreview] = 0;
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Location"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the location of the form.
        ///    </para>
        /// </devdoc>
        [SettingsBindable(true)]
        public new Point Location {
            get {
                return base.Location;
            }
            set {
                base.Location = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MaximizedBounds"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the size of the form when it is
        ///       maximized.
        ///    </para>
        /// </devdoc>
        protected Rectangle MaximizedBounds {
            get {
                return Properties.GetRectangle(PropMaximizedBounds);            
            }
            set {
                if (!value.Equals( MaximizedBounds )) {
                    Properties.SetRectangle(PropMaximizedBounds, value);
                    OnMaximizedBoundsChanged(EventArgs.Empty);
                }
            }
        }

        private static readonly object EVENT_MAXIMIZEDBOUNDSCHANGED = new object();

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MaximizedBoundsChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.FormOnMaximizedBoundsChangedDescr))]
        public event EventHandler MaximizedBoundsChanged {
            add {
                Events.AddHandler(EVENT_MAXIMIZEDBOUNDSCHANGED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_MAXIMIZEDBOUNDSCHANGED, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MaximumSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the maximum size the form can be resized to.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.FormMaximumSizeDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(typeof(Size), "0, 0")
        ]
        public override Size MaximumSize {
            get {
                if (Properties.ContainsInteger(PropMaxTrackSizeWidth)) {
                    return new Size(Properties.GetInteger(PropMaxTrackSizeWidth), Properties.GetInteger(PropMaxTrackSizeHeight));
                }
                return Size.Empty;
            }
            set {
                if (!value.Equals( MaximumSize )) {

                    if (value.Width < 0 || value.Height < 0 ) {
                        throw new ArgumentOutOfRangeException(nameof(MaximumSize));
                    }

                    Properties.SetInteger(PropMaxTrackSizeWidth, value.Width);
                    Properties.SetInteger(PropMaxTrackSizeHeight, value.Height);

                    // Bump minimum size if necessary
                    //
                    if (!MinimumSize.IsEmpty && !value.IsEmpty) {

                        if (Properties.GetInteger(PropMinTrackSizeWidth) > value.Width) {
                            Properties.SetInteger(PropMinTrackSizeWidth, value.Width);
                        }

                        if (Properties.GetInteger(PropMinTrackSizeHeight) > value.Height) {
                            Properties.SetInteger(PropMinTrackSizeHeight, value.Height);
                        }
                    }

                    // Keep form size within new limits
                    //
                    Size size = Size;
                    if (!value.IsEmpty && (size.Width > value.Width || size.Height > value.Height)) {
                        Size = new Size(Math.Min(size.Width, value.Width), Math.Min(size.Height, value.Height));
                    }

                    OnMaximumSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MaximumSizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.FormOnMaximumSizeChangedDescr))]
        public event EventHandler MaximumSizeChanged {
            add {
                Events.AddHandler(EVENT_MAXIMUMSIZECHANGED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_MAXIMUMSIZECHANGED, value);
            }
        }
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(null),
        SRDescription(nameof(SR.FormMenuStripDescr)),
        TypeConverter(typeof(ReferenceConverter))
        ]
        public MenuStrip MainMenuStrip {
            get {
                return (MenuStrip)Properties.GetObject(PropMainMenuStrip);
            }
            set {
                Properties.SetObject(PropMainMenuStrip, value);
                if (IsHandleCreated && Menu == null) {
                    UpdateMenuHandles();
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Margin"]/*' />
        /// <devdoc>
        ///    <para>Hide Margin/MarginChanged</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new Padding Margin {
            get { return base.Margin; }
            set {
                  base.Margin = value;
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MarginChanged"]/*' />
        /// <devdoc>
        ///    <para>Hide Margin/MarginChanged</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MarginChanged {
            add {
                base.MarginChanged += value;
            }
            remove {
                base.MarginChanged -= value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Menu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the <see cref='System.Windows.Forms.MainMenu'/>
        ///       that is displayed in the form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(null),
        SRDescription(nameof(SR.FormMenuDescr)),
        TypeConverter(typeof(ReferenceConverter)),
        Browsable(false),
        ]
        public MainMenu Menu {
            get {
                return (MainMenu)Properties.GetObject(PropMainMenu);
            }
            set {
                MainMenu mainMenu = Menu;

                if (mainMenu != value) {
                    if (mainMenu != null) {
                        mainMenu.form = null;
                    }

                    Properties.SetObject(PropMainMenu, value);

                    if (value != null) {
                        if (value.form != null) {
                            value.form.Menu = null;
                        }
                        value.form = this;
                    }

                    if (formState[FormStateSetClientSize] == 1 && !IsHandleCreated) {
                        ClientSize = ClientSize;
                    }


                    MenuChanged(Windows.Forms.Menu.CHANGE_ITEMS, value);
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MinimumSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the minimum size the form can be resized to.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.FormMinimumSizeDescr)),
        RefreshProperties(RefreshProperties.Repaint),
        ]
        public override Size MinimumSize {
            get {
                if (Properties.ContainsInteger(PropMinTrackSizeWidth)) {
                    return new Size(Properties.GetInteger(PropMinTrackSizeWidth), Properties.GetInteger(PropMinTrackSizeHeight));
                }
                return DefaultMinimumSize;
            }
            set {
                if (!value.Equals( MinimumSize )) {

                    if (value.Width < 0 || value.Height < 0 ) {
                        throw new ArgumentOutOfRangeException(nameof(MinimumSize));
                    }

                    // ensure that the size we've applied fits into the screen 
                    // when IsRestrictedWindow.
                    Rectangle bounds = this.Bounds;
                    bounds.Size = value;                    
                    value = WindowsFormsUtils.ConstrainToScreenWorkingAreaBounds(bounds).Size;

                    Properties.SetInteger(PropMinTrackSizeWidth, value.Width);
                    Properties.SetInteger(PropMinTrackSizeHeight, value.Height);

                    // Bump maximum size if necessary
                    //
                    if (!MaximumSize.IsEmpty && !value.IsEmpty) {

                        if (Properties.GetInteger(PropMaxTrackSizeWidth) < value.Width) {
                            Properties.SetInteger(PropMaxTrackSizeWidth, value.Width);
                        }

                        if (Properties.GetInteger(PropMaxTrackSizeHeight) < value.Height) {
                            Properties.SetInteger(PropMaxTrackSizeHeight, value.Height);
                        }
                    }

                    // Keep form size within new limits
                    //
                    Size size = Size;
                    if (size.Width < value.Width || size.Height < value.Height) {
                        Size = new Size(Math.Max(size.Width, value.Width), Math.Max(size.Height, value.Height));
                    }

                    if (IsHandleCreated) {
                        // "Move" the form to the same size and position to prevent windows from moving it
                        // when the user tries to grab a resizing border.
                        SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle), NativeMethods.NullHandleRef,
                                                       Location.X,
                                                       Location.Y,
                                                       Size.Width,
                                                       Size.Height,
                                                       NativeMethods.SWP_NOZORDER);
                    }

                    OnMinimumSizeChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MinimumSizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.FormOnMinimumSizeChangedDescr))]
        public event EventHandler MinimumSizeChanged {
            add {
                Events.AddHandler(EVENT_MINIMUMSIZECHANGED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_MINIMUMSIZECHANGED, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MaximizeBox"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the maximize button is
        ///       displayed in the caption bar of the form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(true),
        SRDescription(nameof(SR.FormMaximizeBoxDescr))
        ]
        public bool MaximizeBox {
            get {
                return formState[FormStateMaximizeBox] != 0;
            }
            set {
                if (value) {
                    formState[FormStateMaximizeBox] = 1;
                }
                else {
                    formState[FormStateMaximizeBox] = 0;
                }
                UpdateFormStyles();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MdiChildren"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets an array of forms that represent the
        ///       multiple document interface (MDI) child forms that are parented to this
        ///       form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormMDIChildrenDescr))
        ]
        public Form[] MdiChildren {
            get {
                if (ctlClient != null) {
                    return ctlClient.MdiChildren;
                }
                else {
                    return new Form[0];
                }
            }
        }

        /// <devdoc>
        ///     <para>
        ///         Gets the MDIClient that the MDI container form is using to contain Multiple Document Interface (MDI) child forms,
        ///         if this is an MDI container form.
        ///         Represents the client area of a Multiple Document Interface (MDI) Form window, also known as the MDI child window.
        ///     </para>
        /// </devdoc>
        internal MdiClient MdiClient {
            get {
                return this.ctlClient;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MdiParent"]/*' />
        /// <devdoc>
        ///    <para> Indicates the current multiple document
        ///       interface (MDI) parent form of this form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormMDIParentDescr))
        ]
        public Form MdiParent {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "GetParent Demanded");
                IntSecurity.GetParent.Demand();

                return MdiParentInternal;
            }
            set {
                MdiParentInternal = value;
            }
        }

        private Form MdiParentInternal {
            get {
                return (Form)Properties.GetObject(PropFormMdiParent);
            }
            set {
                Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                if (value == formMdiParent && (value != null || ParentInternal == null)) {
                    return;
                }

                if (value != null && this.CreateThreadId != value.CreateThreadId) {
                    throw new ArgumentException(SR.AddDifferentThreads, "value");
                }

                bool oldVisibleBit = GetState(STATE_VISIBLE);
                //
                Visible = false;

                try {
                    if (value == null) {
                        ParentInternal = null;
                        // Not calling SetTopLevelInternal so that IntSecurity.TopLevelWindow.Demand() isn't skipped.
                        SetTopLevel(true);
                    }
                    else {
                        if (IsMdiContainer) {
                            throw new ArgumentException(SR.FormMDIParentAndChild, "value");
                        }
                        if (!value.IsMdiContainer) {
                            throw new ArgumentException(SR.MDIParentNotContainer, "value");
                        }

                        // Setting TopLevel forces a handle recreate before Parent is set,
                        // which causes problems because we try to assign an MDI child to the parking window,
                        // which can't take MDI children.  So we explicitly destroy and create the handle here.

                        Dock = DockStyle.None;
                        Properties.SetObject(PropFormMdiParent, value);

                        SetState(STATE_TOPLEVEL, false);
                        ParentInternal = value.MdiClient;

                        // If it is an MDIChild, and it is not yet visible, we'll
                        // hold off on recreating the window handle. We'll do that
                        // when MdiChild's visibility is set to true (see 

                        // But if the handle has already been created, we need to destroy it
                        // so the form gets MDI-parented properly. See 
                        if (ParentInternal.IsHandleCreated && IsMdiChild && IsHandleCreated) {
                            DestroyHandle();
                        }
                    }
                    InvalidateMergedMenu();
                    UpdateMenuHandles();
                }
                finally {
                    UpdateStyles();
                    Visible = oldVisibleBit;
                }
            }
        }

        private MdiWindowListStrip MdiWindowListStrip {
            get { return Properties.GetObject(PropMdiWindowListStrip) as MdiWindowListStrip; }
            set { Properties.SetObject(PropMdiWindowListStrip, value); }
        }

        private MdiControlStrip MdiControlStrip {
            get { return Properties.GetObject(PropMdiControlStrip) as MdiControlStrip; }
            set { Properties.SetObject(PropMdiControlStrip, value); }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MergedMenu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the merged menu for the
        ///       form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormMergedMenuDescr)),
        ]
        public MainMenu MergedMenu {
            [UIPermission(SecurityAction.Demand, Window=UIPermissionWindow.AllWindows)]
            get {
                return this.MergedMenuPrivate;
            }
        }

        private MainMenu MergedMenuPrivate {
            get {
                Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                if (formMdiParent == null) return null;

                MainMenu mergedMenu = (MainMenu)Properties.GetObject(PropMergedMenu);
                if (mergedMenu != null) return mergedMenu;

                MainMenu parentMenu = formMdiParent.Menu;
                MainMenu mainMenu = Menu;

                if (mainMenu == null) return parentMenu;
                if (parentMenu == null) return mainMenu;

                // Create a menu that merges the two and save it for next time.
                mergedMenu = new MainMenu();
                mergedMenu.ownerForm = this;
                mergedMenu.MergeMenu(parentMenu);
                mergedMenu.MergeMenu(mainMenu);
                Properties.SetObject(PropMergedMenu, mergedMenu);
                return mergedMenu;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MinimizeBox"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the minimize button is displayed in the caption bar of the form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(true),
        SRDescription(nameof(SR.FormMinimizeBoxDescr))
        ]
        public bool MinimizeBox {
            get {
                return formState[FormStateMinimizeBox] != 0;
            }
            set {
                if (value) {
                    formState[FormStateMinimizeBox] = 1;
                }
                else {
                    formState[FormStateMinimizeBox] = 0;
                }
                UpdateFormStyles();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Modal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets a value indicating whether this form is
        ///       displayed modally.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormModalDescr))
        ]
        public bool Modal {
            get {
                return GetState(STATE_MODAL);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Opacity"]/*' />
        /// <devdoc>
        ///     Determines the opacity of the form. This can only be set on top level
        ///     controls.  Opacity requires Windows 2000 or later, and is ignored on earlier
        ///     operating systems.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        TypeConverterAttribute(typeof(OpacityConverter)),
        SRDescription(nameof(SR.FormOpacityDescr)),
        DefaultValue(1.0)
        ]
        public double Opacity {
            get {
                object opacity = Properties.GetObject(PropOpacity);
                if (opacity != null) {
                    return Convert.ToDouble(opacity, CultureInfo.InvariantCulture);
                }
                else {
                    return 1.0f;
                }
            }
            set {
                // In restricted mode a form cannot be made less visible than 50% opacity.
                if (IsRestrictedWindow) {
                    value = Math.Max(value, .50f);
                }

                if (value > 1.0) {
                    value = 1.0f;
                }
                else if (value < 0.0) {
                    value = 0.0f;
                }

                Properties.SetObject(PropOpacity, value);

                bool oldLayered = (formState[FormStateLayered] != 0);

                if (OpacityAsByte < 255 && OSFeature.Feature.IsPresent(OSFeature.LayeredWindows))
                {
                    AllowTransparency = true;
                    if (formState[FormStateLayered] != 1) {
                        formState[FormStateLayered] = 1;
                        if (!oldLayered) {
                            UpdateStyles();
                        }
                    }
                }
                else {
                    formState[FormStateLayered] = (this.TransparencyKey != Color.Empty) ? 1 : 0;
                    if (oldLayered != (formState[FormStateLayered] != 0)) {
                        int exStyle = unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_EXSTYLE));
                        CreateParams cp = CreateParams;
                        if (exStyle != cp.ExStyle) {
                            UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_EXSTYLE, new HandleRef(null, (IntPtr)cp.ExStyle));
                        }
                    }
                }

                UpdateLayered();
            }
        }

        private byte OpacityAsByte {
            get {
                return (byte)(Opacity * 255.0f);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OwnedForms"]/*' />
        /// <devdoc>
        /// <para>Gets an array of <see cref='System.Windows.Forms.Form'/> objects that represent all forms that are owned by this form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormOwnedFormsDescr))
        ]
        public Form[] OwnedForms {
            get {
                Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

                Form[] result = new Form[ownedFormsCount];
                if (ownedFormsCount > 0) {
                    Array.Copy(ownedForms, 0, result, 0, ownedFormsCount);
                }

                return result;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Owner"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the form that owns this form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.FormOwnerDescr))
        ]
        public Form Owner {
            get {
                IntSecurity.GetParent.Demand();
                return OwnerInternal;
            }
            set {
                Form ownerOld = OwnerInternal;
                if (ownerOld == value)
                    return;

                if (value != null && !TopLevel) {
                    throw new ArgumentException(SR.NonTopLevelCantHaveOwner, "value");
                }

                CheckParentingCycle(this, value);
                CheckParentingCycle(value, this);

                Properties.SetObject(PropOwner, null);

                if (ownerOld != null) {
                    ownerOld.RemoveOwnedForm(this);
                }

                Properties.SetObject(PropOwner, value);

                if (value != null) {
                    value.AddOwnedForm(this);
                }

                UpdateHandleWithOwner();
            }
        }

        internal Form OwnerInternal {
            get {
                return (Form)Properties.GetObject(PropOwner);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RestoreBounds"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the restored bounds of the Form.</para>
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)
        ]
        public Rectangle RestoreBounds {
            get {
                if (restoreBounds.Width == -1 
                    && restoreBounds.Height == -1 
                    && restoreBounds.X == -1
                    && restoreBounds.Y == -1) {
                    // Form scaling depends on this property being
                    // set correctly.  In some cases (where the size has not yet been set or
                    // has only been set to the default, restoreBounds will remain uninitialized until the
                    // handle has been created.  In this case, return the current Bounds.
                    return Bounds;
                }
                return restoreBounds;
            }
        }
        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RightToLeftLayout"]/*' />
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

                return rightToLeftLayout;
            }

            set {
                if (value != rightToLeftLayout) {
                    rightToLeftLayout = value;
                    using(new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout)) {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }

        internal override Control ParentInternal {
            get {
                return base.ParentInternal;
            }
            set {
                if (value != null) {
                    Owner = null;
                }
                base.ParentInternal = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShowInTaskbar"]/*' />
        /// <devdoc>
        ///    <para>If ShowInTaskbar is true then the form will be displayed
        ///       in the Windows Taskbar.</para>
        /// </devdoc>
        [
        DefaultValue(true),
        SRCategory(nameof(SR.CatWindowStyle)),
        SRDescription(nameof(SR.FormShowInTaskbarDescr))
        ]
        public bool ShowInTaskbar {
            get {
                return formState[FormStateTaskBar] != 0;
            }
            set {
                // Restricted windows must always show in task bar.
                if (IsRestrictedWindow) {
                    return;
                }

                if (ShowInTaskbar != value) {
                    if (value) {
                        formState[FormStateTaskBar] = 1;
                    }
                    else {
                        formState[FormStateTaskBar] = 0;
                    }
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <devdoc>
        ///    Gets or sets a value indicating whether an icon is displayed in the
        ///    caption bar of the form.
        ///    If ControlBox == false, then the icon won't be shown no matter what
        ///    the value of ShowIcon is
        /// </devdoc>
        [
        DefaultValue(true),
        SRCategory(nameof(SR.CatWindowStyle)),
        SRDescription(nameof(SR.FormShowIconDescr))
        ]
        public bool ShowIcon {
            get {
                return formStateEx[FormStateExShowIcon] != 0;
            }

            set {
                if (value) {
                    formStateEx[FormStateExShowIcon] = 1;
                }
                else {
                    // The icon must always be shown for restricted forms.
                    if (IsRestrictedWindow) {
                        return;
                    }
                    formStateEx[FormStateExShowIcon] = 0;
                    UpdateStyles();
                }
                UpdateWindowIcon(true);
            }
        }

        internal override int ShowParams {
            get {
                // From MSDN:
                //      The first time an application calls ShowWindow, it should use the WinMain function's nCmdShow parameter as its nCmdShow parameter. Subsequent calls to ShowWindow must use one of the values in the given list, instead of the one specified by the WinMain function's nCmdShow parameter.

                //      As noted in the discussion of the nCmdShow parameter, the nCmdShow value is ignored in the first call to ShowWindow if the program that launched the application specifies startup information in the STARTUPINFO structure. In this case, ShowWindow uses the information specified in the STARTUPINFO structure to show the window. On subsequent calls, the application must call ShowWindow with nCmdShow set to SW_SHOWDEFAULT to use the startup information provided by the program that launched the application. This behavior is designed for the following situations:
                //
                //      Applications create their main window by calling CreateWindow with the WS_VISIBLE flag set.
                //      Applications create their main window by calling CreateWindow with the WS_VISIBLE flag cleared, and later call ShowWindow with the SW_SHOW flag set to make it visible.
                //

                switch(WindowState) {
                    case FormWindowState.Maximized:
                        return NativeMethods.SW_SHOWMAXIMIZED;
                    case FormWindowState.Minimized:
                        return NativeMethods.SW_SHOWMINIMIZED;
                }
                if (ShowWithoutActivation)
                {
                    return NativeMethods.SW_SHOWNOACTIVATE;
                }
                return NativeMethods.SW_SHOW;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       When this property returns true, the internal ShowParams property will return NativeMethods.SW_SHOWNOACTIVATE.
        ///    </para>
        /// </devdoc>
        [Browsable(false)]
        protected virtual bool ShowWithoutActivation
        {
            get
            {
                return false;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Size"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the size of the form.
        ///    </para>
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Localizable(false)
        ]
        new public Size Size {
            get {
                return base.Size;
            }
            set {
                base.Size = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.SizeGripStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the style of size grip to display in the lower-left corner of the form.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        DefaultValue(SizeGripStyle.Auto),
        SRDescription(nameof(SR.FormSizeGripStyleDescr))
        ]
        public SizeGripStyle SizeGripStyle {
            get {
                return(SizeGripStyle)formState[FormStateSizeGripStyle];
            }
            set {
                if (SizeGripStyle != value) {
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

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.StartPosition"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the
        ///       starting position of the form at run time.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(FormStartPosition.WindowsDefaultLocation),
        SRDescription(nameof(SR.FormStartPositionDescr))
        ]
        public FormStartPosition StartPosition {
            get {
                return(FormStartPosition)formState[FormStateStartPos];
            }
            set {
                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FormStartPosition.Manual, (int)FormStartPosition.CenterParent))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormStartPosition));
                }
                formState[FormStateStartPos] = (int)value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TabIndex"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        new public int TabIndex {
            get {
                return base.TabIndex;
            }
            set {
                base.TabIndex = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TabIndexChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabIndexChanged {
            add {
                base.TabIndexChanged += value;
            }
            remove {
                base.TabIndexChanged -= value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TabStop"]/*' />
        /// <devdoc>
        ///     This property has no effect on Form, we need to hide it from browsers.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DispId(NativeMethods.ActiveX.DISPID_TABSTOP),
        SRDescription(nameof(SR.ControlTabStopDescr))
        ]
        public new bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TabStopChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

        /// <devdoc>
        ///     For forms that are show in task bar false, this returns a HWND
        ///     they must be parented to in order for it to work.
        /// </devdoc>
        private HandleRef TaskbarOwner {
            get {
                if (ownerWindow == null) {
                    ownerWindow = new NativeWindow();
                }

                if (ownerWindow.Handle == IntPtr.Zero) {
                    CreateParams cp = new CreateParams();
                    cp.ExStyle = NativeMethods.WS_EX_TOOLWINDOW;
                    ownerWindow.CreateHandle(cp);
                }

                return new HandleRef(ownerWindow, ownerWindow.Handle);
            }
        }

        [SettingsBindable(true)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TopLevel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether to display the form as a top-level
        ///       window.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool TopLevel {
            get {
                return GetTopLevel();
            }
            set {
                if (!value && ((Form)this).IsMdiContainer && !DesignMode) {
                    throw new ArgumentException(SR.MDIContainerMustBeTopLevel, "value");
                }
                SetTopLevel(value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TopMost"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the form should be displayed as the top-most
        ///       form of your application.</para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatWindowStyle)),
        SRDescription(nameof(SR.FormTopMostDescr))
        ]
        public bool TopMost {
            get {
                return formState[FormStateTopMost] != 0;
            }
            set {
                // Restricted windows cannot be top most to avoid DOS attack by obscuring other windows.
                if (IsRestrictedWindow) {
                    return;
                }

                if (IsHandleCreated && TopLevel) {
                    HandleRef key = value ? NativeMethods.HWND_TOPMOST : NativeMethods.HWND_NOTOPMOST;
                    SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle), key, 0, 0, 0, 0,
                                         NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                }

                if (value) {
                    formState[FormStateTopMost] = 1;
                }
                else {
                    formState[FormStateTopMost] = 0;
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.TransparencyKey"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the color that will represent transparent areas of the form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatWindowStyle)),
        SRDescription(nameof(SR.FormTransparencyKeyDescr))
        ]
        public Color TransparencyKey {
            get {
                object key = Properties.GetObject(PropTransparencyKey);
                if (key != null) {
                    return (Color)key;
                }
                return Color.Empty;
            }
            set {
                Properties.SetObject(PropTransparencyKey, value);
                if (!IsMdiContainer) {
                    bool oldLayered = (formState[FormStateLayered] == 1);
                    if (value != Color.Empty)
                    {
                        IntSecurity.TransparentWindows.Demand();
                        AllowTransparency = true;
                        formState[FormStateLayered] = 1;
                    }
                    else
                    {
                        formState[FormStateLayered] = (this.OpacityAsByte < 255) ? 1 : 0;
                    }
                    if (oldLayered != (formState[FormStateLayered] != 0))
                    {
                        UpdateStyles();
                    }
                    UpdateLayered();
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.SetVisibleCore"]/*' />
        //
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetVisibleCore(bool value) {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Form::SetVisibleCore(" + value.ToString() + ") - " + this.Name);

            // If DialogResult.OK and the value == GetVisibleCore() then this code has been called either through 
            // ShowDialog( ) or explicit Hide( ) by the user. So dont go through this function again. 
            // This will avoid flashing during closing the dialog;
            if (GetVisibleCore() == value && dialogResult == DialogResult.OK)
            {
                return;
            }

            // (!value || calledMakeVisible) is to make sure that we fall
            // through and execute the code below atleast once.
            if (GetVisibleCore() == value && (!value || CalledMakeVisible)) {
                base.SetVisibleCore(value);
                return;
            }

            if (value) {
                CalledMakeVisible = true;
                if (CalledCreateControl) {
                    if (CalledOnLoad) {
                        // Make sure the form is in the Application.OpenForms collection
                        if (!Application.OpenFormsInternal.Contains(this)) {
                            Application.OpenFormsInternalAdd(this);
                        }
                    }
                    else {
                        CalledOnLoad = true;
                        OnLoad(EventArgs.Empty);
                        if (dialogResult != DialogResult.None) {
                            // Don't show the dialog if the dialog result was set
                            // in the OnLoad event.
                            //
                            value = false;
                        }
                    }
                }
            }
            else {
                ResetSecurityTip(true /* modalOnly */);
            }

            if (!IsMdiChild)  {
                base.SetVisibleCore(value);

                // We need to force this call if we were created
                // with a STARTUPINFO structure (e.g. launched from explorer), since
                // it won't send a WM_SHOWWINDOW the first time it's called.
                // when WM_SHOWWINDOW gets called, we'll flip this bit to true
                //
                if (0==formState[FormStateSWCalled]) {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.WM_SHOWWINDOW, value ? 1 : 0, 0);
                }
            }
            else  {
                // Throw away any existing handle.
                if (IsHandleCreated)  {
                    // Everett/RTM used to wrap this in an assert for AWP.
                    DestroyHandle();
                }

                if (!value) {
                    InvalidateMergedMenu();
                    SetState(STATE_VISIBLE, false);
                }
                else {

                    // The ordering is important here... Force handle creation
                    // (getHandle) then show the window (ShowWindow) then finish
                    // creating children using createControl...
                    //
                    SetState(STATE_VISIBLE, true);

                    // Ask the mdiClient to re-layout the controls so that any docking or
                    // anchor settings for this mdi child window will be honored.
                    MdiParentInternal.MdiClient.PerformLayout();
                                        
                    if (ParentInternal != null && ParentInternal.Visible)  {
                        SuspendLayout();
                        try{
                            SafeNativeMethods.ShowWindow(new HandleRef(this, Handle), NativeMethods.SW_SHOW);
                            CreateControl();

                            // If this form is mdichild and maximized, we need to redraw the MdiParent non-client area to 
                            // update the menu bar because we always create the window as if it were not maximized. 
                            // See comment on CreateHandle about this.
                            if (WindowState == FormWindowState.Maximized) { 
                                MdiParentInternal.UpdateWindowIcon(true);
                            }
                        }
                        finally{
                            ResumeLayout();
                        }
                    }
                }
                OnVisibleChanged(EventArgs.Empty);
            }

            //(

            
            if (value && !IsMdiChild && (WindowState == FormWindowState.Maximized || TopMost)) {
                if (ActiveControl == null){
                    SelectNextControlInternal(null, true, true, true, false);
                }
                FocusActiveControlInternal();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.WindowState"]/*' />
        /// <devdoc>
        ///    <para> Gets or sets the form's window state.
        ///       </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        DefaultValue(FormWindowState.Normal),
        SRDescription(nameof(SR.FormWindowStateDescr))
        ]
        public FormWindowState WindowState {
            get {
                return(FormWindowState)formState[FormStateWindowState];
            }
            set {

                //verify that 'value' is a valid enum type...
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FormWindowState.Normal, (int)FormWindowState.Maximized))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FormWindowState));
                }

                if (TopLevel && IsRestrictedWindow) {
                    // We don't allow to minimize or maximze a top level window programatically if it is restricted.
                    // When maximized, the desktop is obscured by the window (DOS attack) and when minimize spoofing 
                    // identity is the thread, the minimized window could steal the user's keystrokes and obtain a 
                    // password for instance.
                    if (value != FormWindowState.Normal) {
                        return;
                    }
                }

                switch (value) {
                        case FormWindowState.Normal:
                        SetState(STATE_SIZELOCKEDBYOS, false);
                        break;
                    case FormWindowState.Maximized:
                    case FormWindowState.Minimized:
                        SetState(STATE_SIZELOCKEDBYOS, true);
                        break;
                }

                if (IsHandleCreated && Visible) {
                    IntPtr hWnd = Handle;
                    switch (value) {
                        case FormWindowState.Normal:
                            SafeNativeMethods.ShowWindow(new HandleRef(this, hWnd), NativeMethods.SW_NORMAL);
                            break;
                        case FormWindowState.Maximized:
                            SafeNativeMethods.ShowWindow(new HandleRef(this, hWnd), NativeMethods.SW_MAXIMIZE);
                            break;
                        case FormWindowState.Minimized:
                            SafeNativeMethods.ShowWindow(new HandleRef(this, hWnd), NativeMethods.SW_MINIMIZE);
                            break;
                    }
                }

                // Now set the local property to the passed in value so that
                // when UpdateWindowState is by the ShowWindow call above, the window state in effect when
                // this call was made will still be effective while processing that method.
                formState[FormStateWindowState] = (int)value;
            }
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the text to display in the caption bar of the form.
        ///    </para>
        /// </devdoc>
        internal override string WindowText {
            get {
                // In restricted mode, the windows caption (Text) is modified to show the url of the window.
                // The userWindowText is used to cache the user's text.
                if (IsRestrictedWindow && formState[FormStateIsWindowActivated] == 1) {
                    if (userWindowText == null) {
                        return "";
                    }
                    return userWindowText;
                }

                return base.WindowText;

            }

            set {
                string oldText = this.WindowText;

                userWindowText = value;

                if (IsRestrictedWindow && formState[FormStateIsWindowActivated] == 1) {
                    if (value == null) {
                        value = "";
                    }
                    base.WindowText = RestrictedWindowText(value);
                }
                else {
                    base.WindowText = value;
                }

                // For non-default FormBorderStyles, we do not set the WS_CAPTION style if the Text property is null or "".
                // When we reload the form from code view, the text property is not set till the very end, and so we do not
                // end up updating the CreateParams with WS_CAPTION. Fixed this by making sure we call UpdateStyles() when
                // we transition from a non-null value to a null value or vice versa in Form.WindowText.
                //
                if (oldText == null || (oldText.Length == 0)|| value == null || (value.Length == 0)) {
                    UpdateFormStyles();
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Activated"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form is activated in code or by the user.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.FormOnActivateDescr))]
        public event EventHandler Activated {
            add {
                Events.AddHandler(EVENT_ACTIVATED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_ACTIVATED, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Closing"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form is closing.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.FormOnClosingDescr)),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public event CancelEventHandler Closing {
            add {
                Events.AddHandler(EVENT_CLOSING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CLOSING, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Closed"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form is closed.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.FormOnClosedDescr)),
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public event EventHandler Closed {
            add {
                Events.AddHandler(EVENT_CLOSED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CLOSED, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Deactivate"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form loses focus and is not the active form.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.FormOnDeactivateDescr))]
        public event EventHandler Deactivate {
            add {
                Events.AddHandler(EVENT_DEACTIVATE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DEACTIVATE, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.FormClosing"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form is closing.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.FormOnFormClosingDescr))]
        public event FormClosingEventHandler FormClosing {
            add {
                Events.AddHandler(EVENT_FORMCLOSING, value);
            }
            remove {
                Events.RemoveHandler(EVENT_FORMCLOSING, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Closed"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form is closed.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.FormOnFormClosedDescr))]
        public event FormClosedEventHandler FormClosed {
            add {
                Events.AddHandler(EVENT_FORMCLOSED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_FORMCLOSED, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Load"]/*' />
        /// <devdoc>
        ///    <para>Occurs before the form becomes visible.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.FormOnLoadDescr))]
        public event EventHandler Load {
            add {
                Events.AddHandler(EVENT_LOAD, value);
            }
            remove {
                Events.RemoveHandler(EVENT_LOAD, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MdiChildActivate"]/*' />
        /// <devdoc>
        ///    <para>Occurs when a Multiple Document Interface (MDI) child form is activated or closed
        ///       within an MDI application.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.FormOnMDIChildActivateDescr))]
        public event EventHandler MdiChildActivate {
            add {
                Events.AddHandler(EVENT_MDI_CHILD_ACTIVATE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MDI_CHILD_ACTIVATE, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MenuComplete"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the menu of a form loses focus.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.FormOnMenuCompleteDescr)),
        Browsable(false)
        ]
        public event EventHandler MenuComplete {
            add {
                Events.AddHandler(EVENT_MENUCOMPLETE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MENUCOMPLETE, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.MenuStart"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the menu of a form receives focus.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)), 
        SRDescription(nameof(SR.FormOnMenuStartDescr)),
        Browsable(false)
        ]
        public event EventHandler MenuStart {
            add {
                Events.AddHandler(EVENT_MENUSTART, value);
            }
            remove {
                Events.RemoveHandler(EVENT_MENUSTART, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.InputLanguageChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs after the input language of the form has changed.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.FormOnInputLangChangeDescr))]
        public event InputLanguageChangedEventHandler InputLanguageChanged {
            add {
                Events.AddHandler(EVENT_INPUTLANGCHANGE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_INPUTLANGCHANGE, value);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.InputLanguageChanging"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the the user attempts to change the input language for the
        ///       form.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.FormOnInputLangChangeRequestDescr))]
        public event InputLanguageChangingEventHandler InputLanguageChanging {
            add {
                Events.AddHandler(EVENT_INPUTLANGCHANGEREQUEST, value);
            }
            remove {
                Events.RemoveHandler(EVENT_INPUTLANGCHANGEREQUEST, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RightToLeftLayoutChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add {
                Events.AddHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_RIGHTTOLEFTLAYOUTCHANGED, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Shown"]/*' />
        /// <devdoc>
        ///    <para>Occurs whenever the form is first shown.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.FormOnShownDescr))]
        public event EventHandler Shown {
            add {
                Events.AddHandler(EVENT_SHOWN, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SHOWN, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Activate"]/*' />
        /// <devdoc>
        ///    <para>Activates the form and gives it focus.</para>
        /// </devdoc>
        public void Activate() {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ModifyFocus Demanded");
            IntSecurity.ModifyFocus.Demand();

            if (Visible && IsHandleCreated) {
                if (IsMdiChild) {
                    MdiParentInternal.MdiClient.SendMessage(NativeMethods.WM_MDIACTIVATE, Handle, 0);
                }
                else {
                    UnsafeNativeMethods.SetForegroundWindow(new HandleRef(this, Handle));
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ActivateMdiChildInternal"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     This function handles the activation of a MDI child form. If a subclass
        ///     overrides this function, it must call base.ActivateMdiChild.
        ///     From MSDN: This member supports the .NET Framework infrastructure and is not intended 
        ///     to be used directly from your code.
        /// </devdoc>
        protected void ActivateMdiChild(Form form) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ModifyFocus Demanded");
            IntSecurity.ModifyFocus.Demand();
            
            ActivateMdiChildInternal(form);
        }

        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        private void ActivateMdiChildInternal(Form form) {
            if (FormerlyActiveMdiChild != null && !FormerlyActiveMdiChild.IsClosing) {
                FormerlyActiveMdiChild.UpdateWindowIcon(true);
                FormerlyActiveMdiChild = null;
            }

            Form activeMdiChild = ActiveMdiChildInternal;
            if (activeMdiChild == form) {
                return;
            }

            //Don't believe we ever hit this with non-null, but leaving it intact in case removing it would cause a problem.
            if (null != activeMdiChild) {
                activeMdiChild.Active = false;
            }

            activeMdiChild = form;
            ActiveMdiChildInternal = form;

            if (null != activeMdiChild) {
                activeMdiChild.IsMdiChildFocusable = true;
                activeMdiChild.Active = true;
            }
            else if (this.Active) {
                ActivateControlInternal(this);
            }

            // Note: It is possible that we are raising this event when the activeMdiChild is null,
            // this is the case when the only visible mdi child is being closed.  See DeactivateMdiChild
            // for more info.
            OnMdiChildActivate(EventArgs.Empty);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AddOwnedForm"]/*' />
        /// <devdoc>
        ///    <para> Adds
        ///       an owned form to this form.</para>
        /// </devdoc>
        public void AddOwnedForm(Form ownedForm) {
            if (ownedForm == null)
                return;

            if (ownedForm.OwnerInternal != this) {
                ownedForm.Owner = this; // NOTE: this calls AddOwnedForm again with correct owner set.
                return;
            }

            Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
            int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

            // Make sure this isn't already in the list:
            for (int i=0;i < ownedFormsCount;i++) {
                if (ownedForms[i]==ownedForm) {
                    return;
                }
            }

            if (ownedForms == null) {
                ownedForms = new Form[4];
                Properties.SetObject(PropOwnedForms, ownedForms);
            }
            else if (ownedForms.Length == ownedFormsCount) {
                Form[] newOwnedForms = new Form[ownedFormsCount*2];
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
        private float AdjustScale(float scale) {
            // NOTE : This function is cloned in FormDocumentDesigner... remember to keep
            //      : them in sync
            //


            // Map 0.0 - .92... increment by 0.08
            //
            if (scale < .92f) {
                return scale + 0.08f;
            }

            // Map .92 - .99 to 1.0
            //
            else if (scale < 1.0f) {
                return 1.0f;
            }

            // Map 1.02... increment by 0.08
            //
            else if (scale > 1.01f) {
                return scale + 0.08f;
            }

            // Map 1.0 - 1.01... no change
            //
            else {
                return scale;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.AdjustFormScrollbars"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void AdjustFormScrollbars(bool displayScrollbars) {
            if (WindowState != FormWindowState.Minimized) {
                base.AdjustFormScrollbars(displayScrollbars);
            }
        }

        private void AdjustSystemMenu(IntPtr hmenu) {
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

            if (!showMin) {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_MINIMIZE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_GRAYED);
            }
            else {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_MINIMIZE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_ENABLED);
            }
            if (!showMax) {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_MAXIMIZE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_GRAYED);
            }
            else {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_MAXIMIZE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_ENABLED);
            }
            if (!showClose) {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_CLOSE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_GRAYED);
            }
            else {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_CLOSE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_ENABLED);
            }
            if (!showRestore) {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_RESTORE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_GRAYED);
            }
            else {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_RESTORE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_ENABLED);
            }
            if (!showSize) {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_SIZE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_GRAYED);
            }
            else {
                UnsafeNativeMethods.EnableMenuItem(new HandleRef(this, hmenu), NativeMethods.SC_SIZE,
                                       NativeMethods.MF_BYCOMMAND | NativeMethods.MF_ENABLED);
            }

#if SECURITY_DIALOG
            AdjustSystemMenuForSecurity(hmenu);
#endif
        }

#if SECURITY_DIALOG
        private void AdjustSystemMenuForSecurity(IntPtr hmenu) {
            if (formState[FormStateAddedSecurityMenuItem] == 0) {
                formState[FormStateAddedSecurityMenuItem] = 1;

                SecurityMenuItem securitySystemMenuItem = new SecurityMenuItem(this);
                Properties.SetObject(PropSecuritySystemMenuItem, securitySystemMenuItem);

                NativeMethods.MENUITEMINFO_T info = new NativeMethods.MENUITEMINFO_T();
                info.fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_STATE |
                             NativeMethods.MIIM_SUBMENU | NativeMethods.MIIM_TYPE | NativeMethods.MIIM_DATA;
                info.fType = 0;
                info.fState = 0;
                info.wID = securitySystemMenuItem.ID;
                info.hbmpChecked = IntPtr.Zero;
                info.hbmpUnchecked = IntPtr.Zero;
                info.dwItemData = IntPtr.Zero;

                // Note:  This code is not shipping in the final product.  We do not want to measure the
                //     :  performance hit of loading the localized resource for this at startup, so I
                //     :  am hard-wiring the strings below.  If you need to localize these, move them to
                //     :  a SECONDARY resource file so we don't have to contend with our big error message
                //     :  file on startup.
                //
                if (IsRestrictedWindow) {
                    info.dwTypeData = ".NET Restricted Window...";
                }
                else {
                    info.dwTypeData = ".NET Window...";
                }
                info.cch = 0;
                UnsafeNativeMethods.InsertMenuItem(new HandleRef(this, hmenu), 0, true, info);


                NativeMethods.MENUITEMINFO_T sep = new NativeMethods.MENUITEMINFO_T();
                sep.fMask = NativeMethods.MIIM_ID | NativeMethods.MIIM_STATE |
                             NativeMethods.MIIM_SUBMENU | NativeMethods.MIIM_TYPE | NativeMethods.MIIM_DATA;
                sep.fType = NativeMethods.MFT_MENUBREAK;
                UnsafeNativeMethods.InsertMenuItem(new HandleRef(this, hmenu), 1, true, sep);

            }
        }
#endif

        /// <devdoc>
        ///     This forces the SystemMenu to look like we want.
        /// </devdoc>
        /// <internalonly/>
        private void AdjustSystemMenu() {
            if (IsHandleCreated) {
                IntPtr hmenu = UnsafeNativeMethods.GetSystemMenu(new HandleRef(this, Handle), false);
                AdjustSystemMenu(hmenu);
                hmenu = IntPtr.Zero;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ApplyAutoScaling"]/*' />
        /// <devdoc>
        ///     This auto scales the form based on the AutoScaleBaseSize.
        /// </devdoc>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method has been deprecated. Use the ApplyAutoScaling method instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected void ApplyAutoScaling() {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "ApplyAutoScaling... ");
            Debug.Indent();
            // NOTE : This function is cloned in FormDocumentDesigner... remember to keep
            //      : them in sync
            //


            // We also don't do this if the property is empty.  Otherwise we will perform
            // two GetAutoScaleBaseSize calls only to find that they returned the same
            // value.
            //
            if (!autoScaleBaseSize.IsEmpty) {
                Size baseVar = AutoScaleBaseSize;
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "base  =" + baseVar);
                SizeF newVarF = GetAutoScaleSize(Font);
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "new(f)=" + newVarF);
                Size newVar = new Size((int)Math.Round(newVarF.Width), (int)Math.Round(newVarF.Height));
                Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, "new(i)=" + newVar);

                // We save a significant amount of time by bailing early if there's no work to be done
                if (baseVar.Equals(newVar))
                    return;

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

        /// <devdoc>
        ///     This adjusts the size of the windowRect so that the client rect is the
        ///     correct size.
        /// </devdoc>
        /// <internalonly/>
        private void ApplyClientSize() {
            if ((FormWindowState)formState[FormStateWindowState] != FormWindowState.Normal
                || !IsHandleCreated) {
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
            if (formState[FormStateSetClientSize] != 0) {
                adjustScroll = true;
                formState[FormStateSetClientSize] = 0;
            }
            if (adjustScroll) {
                if (hscr) {
                    correctClientSize.Height += SystemInformation.HorizontalScrollBarHeight;
                }
                if (vscr) {
                    correctClientSize.Width += SystemInformation.VerticalScrollBarWidth;
                }
            }

            IntPtr h = Handle;
            NativeMethods.RECT rc = new NativeMethods.RECT();
            SafeNativeMethods.GetClientRect(new HandleRef(this, h), ref rc);
            Rectangle currentClient = Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);

            Rectangle bounds = Bounds;

            // If the width is incorrect, compute the correct size with
            // computeWindowSize. We only do this logic if the width needs to
            // be adjusted to avoid double adjusting the window.
            //
            if (correctClientSize.Width != currentClient.Width) {
                Size correct = ComputeWindowSize(correctClientSize);

                // Since computeWindowSize ignores scrollbars, we must tack these on to
                // assure the correct size.
                //
                if (vscr) {
                    correct.Width += SystemInformation.VerticalScrollBarWidth;
                }
                if (hscr) {
                    correct.Height += SystemInformation.HorizontalScrollBarHeight;
                }
                bounds.Width = correct.Width;
                bounds.Height = correct.Height;
                Bounds = bounds;
                SafeNativeMethods.GetClientRect(new HandleRef(this, h), ref rc);
                currentClient = Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }

            // If it still isn't correct, then we assume that the problem is
            // menu wrapping (since computeWindowSize doesn't take that into
            // account), so we just need to adjust the height by the correct
            // amount.
            //
            if (correctClientSize.Height != currentClient.Height) {

                int delta = correctClientSize.Height - currentClient.Height;
                bounds.Height += delta;
                Bounds = bounds;
            }

            UpdateBounds();
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Assigns a new parent control. Sends out the appropriate property change
        ///       notifications for properties that are affected by the change of parent.</para>
        /// </devdoc>
        internal override void AssignParent(Control value) {

            // If we are being unparented from the MDI client control, remove
            // formMDIParent as well.
            //
            Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
            if (formMdiParent != null && formMdiParent.MdiClient != value) {
                Properties.SetObject(PropFormMdiParent, null);
            }

            base.AssignParent(value);
        }

        /// <devdoc>
        ///     Checks whether a modal dialog is ready to close. If the dialogResult
        ///     property is not DialogResult.None, the OnClosing and OnClosed events
        ///     are fired. A return value of true indicates that both events were
        ///     successfully dispatched and that event handlers did not cancel closing
        ///     of the dialog. User code should never have a reason to call this method.
        ///     It is internal only so that the Application class can call it from a
        ///     modal message loop.
        ///
        ///     closingOnly is set when this is called from WmClose so that we can do just the beginning portion
        ///     of this and then let the message loop finish it off with the actual close code.
        ///
        ///     Note we set a flag to determine if we've already called close or not.
        ///
        /// </devdoc>
        internal bool CheckCloseDialog(bool closingOnly) {
            if (dialogResult == DialogResult.None && Visible) {
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

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Close"]/*' />
        /// <devdoc>
        ///    <para>Closes the form.</para>
        /// </devdoc>
        public void Close() {

            if (GetState(STATE_CREATINGHANDLE))
                throw new InvalidOperationException(string.Format(SR.ClosingWhileCreatingHandle, "Close"));

            if (IsHandleCreated) {
                closeReason = CloseReason.UserClosing;
                SendMessage(NativeMethods.WM_CLOSE, 0, 0);
            }
            else{
                // MSDN: When a form is closed, all resources created within the object are closed and the form is disposed.
                // For MDI child: MdiChildren collection gets updated
                Dispose();
            }
        }

        /// <devdoc>
        ///     Computes the window size from the clientSize based on the styles
        ///     returned from CreateParams.
        /// </devdoc>
        private Size ComputeWindowSize(Size clientSize) {
            CreateParams cp = CreateParams;
            return ComputeWindowSize(clientSize, cp.Style, cp.ExStyle);
        }

        /// <devdoc>
        ///     Computes the window size from the clientSize base on the specified
        ///     window styles. This will not return the correct size if menus wrap.
        /// </devdoc>
        private Size ComputeWindowSize(Size clientSize, int style, int exStyle) {
            NativeMethods.RECT result = new NativeMethods.RECT(0, 0, clientSize.Width, clientSize.Height);
            AdjustWindowRectEx(ref result, style, HasMenu, exStyle);
            return new Size(result.right - result.left, result.bottom - result.top);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.CreateControlsInstance"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Control.ControlCollection CreateControlsInstance() {
            return new ControlCollection(this);
        }

        /// <devdoc>
        ///     Cleans up form state after a control has been removed.
        ///     Package scope for Control
        /// </devdoc>
        /// <internalonly/>
        internal override void AfterControlRemoved(Control control, Control oldParent) {
            base.AfterControlRemoved(control, oldParent);

            if (control == AcceptButton) {
                this.AcceptButton = null;
            }
            if (control == CancelButton) {
                this.CancelButton = null;
            }
            if (control == ctlClient) {
                ctlClient = null;
                UpdateMenuHandles();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.CreateHandle"]/*' />
        /// <devdoc>
        ///    <para>Creates the handle for the Form. If a
        ///       subclass overrides this function,
        ///       it must call the base implementation.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void CreateHandle() {
            // In the windows MDI code we have to suspend menu
            // updates on the parent while creating the handle. Otherwise if the
            // child is created maximized, the menu ends up with two sets of
            // MDI child ornaments.
            Form form = (Form)Properties.GetObject(PropFormMdiParent);
            if (form != null){
                form.SuspendUpdateMenuHandles();
            }

            try {
                // If the child is created before the MDI parent we can
                // get Win32 exceptions as the MDI child is parked to the parking window.
                if (IsMdiChild && MdiParentInternal.IsHandleCreated) {
                    MdiClient mdiClient = MdiParentInternal.MdiClient;
                    if (mdiClient != null && !mdiClient.IsHandleCreated) {
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
                    && (FormWindowState)formState[FormStateWindowState] == FormWindowState.Maximized) {

                    // This is the reason why we see the blue borders
                    // when creating a maximized mdi child, unfortunately we cannot fix this now...
                    formState[FormStateWindowState] = (int)FormWindowState.Normal;
                    formState[FormStateMdiChildMax] = 1;
                    base.CreateHandle();
                    formState[FormStateWindowState] = (int)FormWindowState.Maximized;
                    formState[FormStateMdiChildMax] = 0;
                }
                else {
                    base.CreateHandle();
                }

                UpdateHandleWithOwner();
                UpdateWindowIcon(false);

                AdjustSystemMenu();

                if ((FormStartPosition)formState[FormStateStartPos] != FormStartPosition.WindowsDefaultBounds) {
                    ApplyClientSize();
                }
                if (formState[FormStateShowWindowOnCreate] == 1) {
                    Visible = true;
                }


                // avoid extra SetMenu calls for perf
                if (Menu != null || !TopLevel || IsMdiContainer)
                    UpdateMenuHandles();

                // In order for a window not to have a taskbar entry, it must
                // be owned.
                //
                if (!ShowInTaskbar && OwnerInternal == null && TopLevel) {
                    UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT, TaskbarOwner);

                    // Make sure the large icon is set so the ALT+TAB icon 
                    // reflects the real icon of the application
                    Icon icon = Icon;
                    if (icon != null && TaskbarOwner.Handle != IntPtr.Zero) {
                       UnsafeNativeMethods.SendMessage(TaskbarOwner, NativeMethods.WM_SETICON, NativeMethods.ICON_BIG, icon.Handle);
                    }
                }

                if (formState[FormStateTopMost] != 0) {
                    TopMost = true;
                }

            }
            finally {
                 if (form != null)
                    form.ResumeUpdateMenuHandles();

                // We need to reset the styles in case Windows tries to set us up
                // with "correct" styles
                //
                UpdateStyles();
            }
        }

        // Deactivates active MDI child and temporarily marks it as unfocusable,
        // so that WM_SETFOCUS sent to MDIClient does not activate that child.
        private void DeactivateMdiChild() {
            Form activeMdiChild = ActiveMdiChildInternal;
            if (null != activeMdiChild) {
                Form mdiParent = activeMdiChild.MdiParentInternal;

                activeMdiChild.Active = false;
                activeMdiChild.IsMdiChildFocusable = false;
                if (!activeMdiChild.IsClosing) {
                    FormerlyActiveMdiChild = activeMdiChild;
                }
                // Enter/Leave events on child controls are raised from the ActivateMdiChildInternal method, usually when another 
                // Mdi child is getting activated after deactivating this one; but if this is the only visible MDI child 
                // we need to fake the activation call so MdiChildActivate and Leave events are raised properly. (We say
                // in the MSDN doc that the MdiChildActivate event is raised when an mdi child is activated or closed -
                // we actually meant the last mdi child is closed).
                bool fakeActivation = true;
                foreach(Form mdiChild in mdiParent.MdiChildren ){
                    if( mdiChild != this && mdiChild.Visible ){
                        fakeActivation = false; // more than one mdi child visible.
                        break;
                    }
                }

                if( fakeActivation ){
                    mdiParent.ActivateMdiChildInternal(null);
                }

                ActiveMdiChildInternal = null;

                // Note: WM_MDIACTIVATE message is sent to the form being activated and to the form being deactivated, ideally
                // we would raise the event here accordingly but it would constitute a breaking change.
                //OnMdiChildActivate(EventArgs.Empty);

                // undo merge
                UpdateMenuHandles();
                UpdateToolStrip();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DefWndProc"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Calls the default window proc for the form. If
        ///       a
        ///       subclass overrides this function,
        ///       it must call the base implementation.
        ///       </para>
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected override void DefWndProc(ref Message m) {
            if (ctlClient != null && ctlClient.IsHandleCreated && ctlClient.ParentInternal == this){
                m.Result = UnsafeNativeMethods.DefFrameProc(m.HWnd, ctlClient.Handle, m.Msg, m.WParam, m.LParam);
            }
            else if (0 != formStateEx[FormStateExUseMdiChildProc]){
                m.Result = UnsafeNativeMethods.DefMDIChildProc(m.HWnd, m.Msg, m.WParam, m.LParam);
            }
            else {
                base.DefWndProc(ref m);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Dispose"]/*' />
        /// <devdoc>
        ///    <para>Releases all the system resources associated with the Form. If a subclass
        ///       overrides this function, it must call the base implementation.</para>
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                CalledOnLoad = false;
                CalledMakeVisible = false;
                CalledCreateControl = false;

                if (Properties.ContainsObject(PropAcceptButton)) Properties.SetObject(PropAcceptButton, null);
                if (Properties.ContainsObject(PropCancelButton)) Properties.SetObject(PropCancelButton, null);
                if (Properties.ContainsObject(PropDefaultButton)) Properties.SetObject(PropDefaultButton, null);
                if (Properties.ContainsObject(PropActiveMdiChild)) Properties.SetObject(PropActiveMdiChild, null);

                if (MdiWindowListStrip != null){
                    MdiWindowListStrip.Dispose();
                    MdiWindowListStrip = null;
                }

                if (MdiControlStrip != null){
                    MdiControlStrip.Dispose();
                    MdiControlStrip = null;
                }

                if (MainMenuStrip != null) {
                    // should NOT call dispose on MainMenuStrip - it's likely NOT to be in the form's control collection.
                    MainMenuStrip = null;
                }
             
                Form owner = (Form)Properties.GetObject(PropOwner);
                if (owner != null) {
                    owner.RemoveOwnedForm(this);
                    Properties.SetObject(PropOwner, null);
                }

                Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

                for (int i = ownedFormsCount-1 ; i >= 0; i--) {
                    if (ownedForms[i] != null) {
                        // it calls remove and removes itself.
                        ownedForms[i].Dispose();
                    }
                }

                if (smallIcon != null) {
                    smallIcon.Dispose();
                    smallIcon = null;
                }

                ResetSecurityTip(false /* modalOnly */);

                base.Dispose(disposing);
                ctlClient = null;

                MainMenu mainMenu = Menu;

                // We should only dispose this form's menus!
                if (mainMenu != null && mainMenu.ownerForm == this) {
                    mainMenu.Dispose();
                    Properties.SetObject(PropMainMenu, null);
                }

                if (Properties.GetObject(PropCurMenu) != null) {
                    Properties.SetObject(PropCurMenu, null);
                }

                MenuChanged(Windows.Forms.Menu.CHANGE_ITEMS, null);

                MainMenu dummyMenu = (MainMenu)Properties.GetObject(PropDummyMenu);

                if (dummyMenu != null) {
                    dummyMenu.Dispose();
                    Properties.SetObject(PropDummyMenu, null);
                }

                MainMenu mergedMenu = (MainMenu)Properties.GetObject(PropMergedMenu);

                if (mergedMenu != null) {
                    if (mergedMenu.ownerForm == this || mergedMenu.form == null) {
                        mergedMenu.Dispose();
                    }
                    Properties.SetObject(PropMergedMenu, null);
                }
            }
            else {
                base.Dispose(disposing);
            }
        }

        /// <devdoc>
        ///     Adjusts the window style of the CreateParams to reflect the bordericons.
        /// </devdoc>
        /// <internalonly/>
        private void FillInCreateParamsBorderIcons(CreateParams cp) {
            if (FormBorderStyle != FormBorderStyle.None) {
                if (Text != null && Text.Length != 0) {
                    cp.Style |= NativeMethods.WS_CAPTION;
                }

                // In restricted mode, the form must have a system menu, caption and max/min/close boxes.

                if (ControlBox || IsRestrictedWindow) {
                    cp.Style |= NativeMethods.WS_SYSMENU | NativeMethods.WS_CAPTION;
                }
                else {
                    cp.Style &= (~NativeMethods.WS_SYSMENU);
                }

                if (MaximizeBox || IsRestrictedWindow) {
                    cp.Style |= NativeMethods.WS_MAXIMIZEBOX;
                }
                else {
                    cp.Style &= ~NativeMethods.WS_MAXIMIZEBOX;
                }

                if (MinimizeBox || IsRestrictedWindow) {
                    cp.Style |= NativeMethods.WS_MINIMIZEBOX;
                }
                else {
                    cp.Style &= ~NativeMethods.WS_MINIMIZEBOX;
                }

                if (HelpButton && !MaximizeBox && !MinimizeBox && ControlBox) {
                    // Windows should ignore WS_EX_CONTEXTHELP unless all those conditions hold.
                    // But someone must have failed the check, because Windows 2000
                    // will show a help button if either the maximize or
                    // minimize button is disabled.
                    cp.ExStyle |= NativeMethods.WS_EX_CONTEXTHELP;
                }
                else {
                    cp.ExStyle &= ~NativeMethods.WS_EX_CONTEXTHELP;
                }
            }
        }

        /// <devdoc>
        ///     Adjusts the window style of the CreateParams to reflect the borderstyle.
        /// </devdoc>
        private void FillInCreateParamsBorderStyles(CreateParams cp) {
            switch ((FormBorderStyle)formState[FormStateBorderStyle]) {
                case FormBorderStyle.None:
                    // 




                    if (IsRestrictedWindow) {
                        goto case FormBorderStyle.FixedSingle;
                    }
                    break;
                case FormBorderStyle.FixedSingle:
                    cp.Style |= NativeMethods.WS_BORDER;
                    break;
                case FormBorderStyle.Sizable:
                    cp.Style |= NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME;
                    break;
                case FormBorderStyle.Fixed3D:
                    cp.Style |= NativeMethods.WS_BORDER;
                    cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                    break;
                case FormBorderStyle.FixedDialog:
                    cp.Style |= NativeMethods.WS_BORDER;
                    cp.ExStyle |= NativeMethods.WS_EX_DLGMODALFRAME;
                    break;
                case FormBorderStyle.FixedToolWindow:
                    cp.Style |= NativeMethods.WS_BORDER;
                    cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
                    break;
                case FormBorderStyle.SizableToolWindow:
                    cp.Style |= NativeMethods.WS_BORDER | NativeMethods.WS_THICKFRAME;
                    cp.ExStyle |= NativeMethods.WS_EX_TOOLWINDOW;
                    break;
            }
        }

        /// <devdoc>
        ///     Adjusts the CreateParams to reflect the window bounds and start position.
        /// </devdoc>
        private void FillInCreateParamsStartPosition(CreateParams cp) {

            if (formState[FormStateSetClientSize] != 0) {

                // When computing the client window size, don't tell them that
                // we are going to be maximized!
                //
                int maskedStyle = cp.Style & ~(NativeMethods.WS_MAXIMIZE | NativeMethods.WS_MINIMIZE);
                Size correct = ComputeWindowSize(ClientSize, maskedStyle, cp.ExStyle);
                
                if (IsRestrictedWindow) {
                    correct = ApplyBoundsConstraints(cp.X, cp.Y, correct.Width, correct.Height).Size;
                }               
                cp.Width = correct.Width;
                cp.Height = correct.Height;
            }

            switch ((FormStartPosition)formState[FormStateStartPos]) {
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
                    if (IsMdiChild && DockStyle.None != Dock){
                        break;
                    }

                    cp.X = NativeMethods.CW_USEDEFAULT;
                    cp.Y = NativeMethods.CW_USEDEFAULT;
                    break;
                case FormStartPosition.CenterScreen:
                    if (IsMdiChild) {
                        Control mdiclient = MdiParentInternal.MdiClient;
                        Rectangle clientRect = mdiclient.ClientRectangle;

                        cp.X = Math.Max(clientRect.X,clientRect.X + (clientRect.Width - cp.Width)/2);
                        cp.Y = Math.Max(clientRect.Y,clientRect.Y + (clientRect.Height - cp.Height)/2);
                    }
                    else {
                        Screen desktop = null;
                        IWin32Window dialogOwner = (IWin32Window)Properties.GetObject(PropDialogOwner);
                        if ((OwnerInternal != null) || (dialogOwner != null)) {
                            IntPtr ownerHandle = (dialogOwner != null) ? Control.GetSafeHandle(dialogOwner) : OwnerInternal.Handle;
                            desktop = Screen.FromHandleInternal(ownerHandle);
                        }
                        else {
                            desktop = Screen.FromPoint(Control.MousePosition);
                        }
                        Rectangle screenRect = desktop.WorkingArea;
                        //if, we're maximized, then don't set the x & y coordinates (they're @ (0,0) )
                        if (WindowState != FormWindowState.Maximized) {
                            cp.X = Math.Max(screenRect.X,screenRect.X + (screenRect.Width - cp.Width)/2);
                            cp.Y = Math.Max(screenRect.Y,screenRect.Y + (screenRect.Height - cp.Height)/2);
                        }
                    }
                    break;
            }
        }

        /// <devdoc>
        ///     Adjusts the Createparams to reflect the window state.
        /// </devdoc>
        private void FillInCreateParamsWindowState(CreateParams cp) {
            // SECUNDONE: We don't need to check for restricted window here since the only way to set the WindowState
            //            programatically is by changing the property and we have a check for it in the property setter.
            //            We don't want to check it here again because it would not allow to set the CreateParams.Style
            //            to the current WindowState so the window will be set to its current Normal size 
            //            (which in some cases is quite bogus) when Control.UpdateStylesCore is called.              
            //
            //    if( IsRestrictedWindow ){
            //        return;
            //    }

            switch ((FormWindowState)formState[FormStateWindowState]) {
                case FormWindowState.Maximized:
                    cp.Style |= NativeMethods.WS_MAXIMIZE;
                    break;
                case FormWindowState.Minimized:
                    cp.Style |= NativeMethods.WS_MINIMIZE;
                    break;
            }
        }

        /// <devdoc>
        ///    <para> Sets focus to the Form.</para>
        ///    <para>Attempts to set focus to this Form.</para>
        /// </devdoc>
        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal override bool FocusInternal() {
            Debug.Assert( IsHandleCreated, "Attempt to set focus to a form that has not yet created its handle." );
            //if this form is a MdiChild, then we need to set the focus in a different way...
            //
            if (IsMdiChild) {
                MdiParentInternal.MdiClient.SendMessage(NativeMethods.WM_MDIACTIVATE, Handle, 0);
                return Focused;
            }

            return base.FocusInternal();
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.GetAutoScaleSize"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This method has been deprecated. Use the AutoScaleDimensions property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // "The quick ..." is the archetipal string
                                                                                                        // used to measure font height.
                                                                                                        // So we don't have to localize it.
        ]
        public static SizeF GetAutoScaleSize(Font font) {

            float height = font.Height;
            float width = 9.0f;

            try {
                using( Graphics graphics = Graphics.FromHwndInternal(IntPtr.Zero /*screen*/) ) {
                    string magicString = "The quick brown fox jumped over the lazy dog.";
                    double magicNumber = 44.549996948242189; // chosen for compatibility with older versions of windows forms, but approximately magicString.Length
                    float stringWidth = graphics.MeasureString(magicString, font).Width;
                    width = (float) (stringWidth / magicNumber);
                }
            }
            catch { // We may get an bogus OutOfMemoryException
                    // (which is a critical exception - according to ClientUtils.IsCriticalException())
                    // from GDI+. So we can't use ClientUtils.IsCriticalException here and rethrow.
            }

            return new SizeF(width, height);
        }

        internal override Size GetPreferredSizeCore(Size proposedSize) {

            // 

            Size preferredSize = base.GetPreferredSizeCore(proposedSize);

#if MAGIC_PADDING
//We are removing the magic padding in runtime.  

            bool dialogFixRequired = false;

            if (Padding == Padding.Empty) {
                Padding paddingToAdd = Padding.Empty;
                foreach (Control c in this.Controls) {
                    if (c.Dock == DockStyle.None) {
                        AnchorStyles anchor = c.Anchor;
                        if (anchor == AnchorStyles.None) {
                            break;
                        }

                        // TOP
                        // if we are anchored to the top only add padding if the top edge is too far down
                        if ((paddingToAdd.Bottom == 0) && DefaultLayout.IsAnchored(anchor, AnchorStyles.Top)) {
                            if (c.Bottom > preferredSize.Height - FormPadding.Bottom) {
                               paddingToAdd.Bottom = FormPadding.Bottom;

                               dialogFixRequired = true;
                            }
                        }
                        // BOTTOM
                        // if we are anchored to the bottom
                        // dont add any padding - it's way too confusing to be dragging a button up
                        // and have the form grow to the bottom.

                        // LEFT
                        // if we are anchored to the left only add padding if the right edge is too far right
                        if ((paddingToAdd.Left == 0) && DefaultLayout.IsAnchored(anchor, AnchorStyles.Left)) {
                            if (c.Right > preferredSize.Width - FormPadding.Right) {
                               paddingToAdd.Right = FormPadding.Right;
                               dialogFixRequired = true;
                            }
                        }
                        // RIGHT
                        // if we are anchored to the bottom
                        // dont add any padding - it's way too confusing to be dragging a button left
                        // and have the form grow to the right

                    }
                }
                Debug.Assert(!dialogFixRequired, "this dialog needs update: " + this.Name);
                return preferredSize + paddingToAdd.Size;
            }
#endif
            return preferredSize;
        }

        /// <devdoc>
        ///    This private method attempts to resolve security zone and site
        ///    information given a list of urls (sites).  This list is supplied
        ///    by the runtime and will contain the paths of all loaded assemblies
        ///    on the stack.  From here, we can examine zones and sites and
        ///    attempt to identify the unique and mixed scenarios for each.  This
        ///    information will be displayed in the titlebar of the Form in a
        ///    semi-trust environment.
        /// </devdoc>
        private void ResolveZoneAndSiteNames(ArrayList sites, ref string securityZone, ref string securitySite) {

            //Start by defaulting to 'unknown zone' and 'unknown site' strings.  We will return this
            //information if anything goes wrong while trying to resolve this information.
            //
            securityZone = SR.SecurityRestrictedWindowTextUnknownZone;
            securitySite = SR.SecurityRestrictedWindowTextUnknownSite;

            try
            {
                //these conditions must be met
                //
                if (sites == null || sites.Count == 0)
                    return;

                //create a new zone array list which has no duplicates and no
                //instances of mycomputer
                ArrayList zoneList = new ArrayList();
                foreach (object arrayElement in sites)
                {
                    if (arrayElement == null)
                        return;

                    string url = arrayElement.ToString();

                    if (url.Length == 0)
                        return;

                    //into a zoneName
                    //
                    Zone currentZone = Zone.CreateFromUrl(url);

                    //skip this if the zone is mycomputer
                    //
                    if (currentZone.SecurityZone.Equals(SecurityZone.MyComputer))
                        continue;

                    //add our unique zonename to our list of zones
                    //
                    string zoneName = currentZone.SecurityZone.ToString();

                    if (!zoneList.Contains(zoneName))
                    {
                        zoneList.Add(zoneName);
                    }
                }

                //now, we resolve the zone name based on the unique information
                //left in the zoneList
                //
                if (zoneList.Count == 0)
                {
                    //here, all the original zones were 'mycomputer'
                    //so we can just return that
                    securityZone = SecurityZone.MyComputer.ToString();
                }
                else if (zoneList.Count == 1)
                {
                    //we only found 1 unique zone other than
                    //mycomputer
                    securityZone = zoneList[0].ToString();
                }
                else
                {
                    //here, we found multiple zones
                    //
                    securityZone = SR.SecurityRestrictedWindowTextMixedZone;
                }

                //generate a list of loaded assemblies that came from the gac, this
                //way we can safely ignore these from the url list
                ArrayList loadedAssembliesFromGac = new ArrayList();

                // 

                FileIOPermission fiop = new FileIOPermission( PermissionState.None );
                fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
                fiop.Assert();

                try
                {
                    foreach (Assembly asm in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        if (asm.GlobalAssemblyCache)
                        {
                            loadedAssembliesFromGac.Add(asm.CodeBase.ToUpper(CultureInfo.InvariantCulture));
                        }
                    }
                }
                finally
                {
                    CodeAccessPermission.RevertAssert();
                }

                //now, build up a sitelist which contains a friendly string
                //we've extracted via the uri class and omit any urls that reference
                //our local gac
                //
                ArrayList siteList = new ArrayList();
                foreach (object arrayElement in sites)
                {
                    //we know that each element is valid because of our
                    //first pass
                    Uri currentSite = new Uri(arrayElement.ToString());

                    //if we see a site that contains the path to our gac,
                    //we'll just skip it

                    if (loadedAssembliesFromGac.Contains(currentSite.AbsoluteUri.ToUpper(CultureInfo.InvariantCulture)))
                    {
                        continue;
                    }

                    //add the unique host name to our list
                    string hostName = currentSite.Host;
                    if (hostName.Length > 0 && !siteList.Contains(hostName))
                        siteList.Add(hostName);
                }


                //resolve the site name from our list, if siteList.count == 0
                //then we have already set our securitySite to "unknown site"
                //
                if (siteList.Count == 0) {
                    //here, we'll set the local machine name to the site string
                    //

                    // 

                    new EnvironmentPermission(PermissionState.Unrestricted).Assert();
                    try {
                        securitySite = Environment.MachineName;
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }
                else if (siteList.Count == 1)
                {
                    //We found 1 unique site other than the info in the
                    //gac
                    securitySite = siteList[0].ToString();
                }
                else
                {
                    //multiple sites, we'll have to return 'mixed sites'
                    //
                    securitySite = SR.SecurityRestrictedWindowTextMultipleSites;
                }
            }
            catch
            {
                //We'll do nothing here. The idea is that zone and security strings are set
                //to "unkown" at the top of this method - if an exception is thrown, we'll
                //stick with those values
            }
        }

        /// <devdoc>
        ///    Sets the restricted window text (titlebar text of a form) when running
        ///    in a semi-trust environment.  The format is: [zone info] - Form Text - [site info]
        /// </devdoc>
        private string RestrictedWindowText(string original) {
            EnsureSecurityInformation();
            return string.Format(CultureInfo.CurrentCulture, Application.SafeTopLevelCaptionFormat, original, securityZone, securitySite);
        }

        private void EnsureSecurityInformation() {
            if (securityZone == null || securitySite == null) {
                ArrayList zones;
                ArrayList sites;

                SecurityManager.GetZoneAndOrigin( out zones, out sites );

                ResolveZoneAndSiteNames(sites, ref securityZone, ref securitySite);
            }
        }

        private void CallShownEvent()
        {
            OnShown(EventArgs.Empty);
        }

        /// <devdoc>
        ///     Override since CanProcessMnemonic is overriden too (base.CanSelectCore calls CanProcessMnemonic).
        /// </devdoc>
        internal override bool CanSelectCore() {
            if( !GetStyle(ControlStyles.Selectable) || !this.Enabled || !this.Visible) {
                return false;
            }
            return true;
        }

        /// <devdoc>
        ///     When an MDI form is hidden it means its handle has not yet been created or has been destroyed (see
        ///     SetVisibleCore).  If the handle is recreated, the form will be made visible which should be avoided.
        /// </devdoc>
        internal bool CanRecreateHandle() {
            if( IsMdiChild ){
                // During changing visibility, it is possible that the style returns true for visible but the handle has
                // not yet been created, add a check for both.
                return GetState(STATE_VISIBLE) && IsHandleCreated;
            }
            return true;
        }

        /// <devdoc>
        ///     Overriden to handle MDI mnemonic processing properly.
        /// </devdoc>
        internal override bool CanProcessMnemonic()  {
#if DEBUG
            TraceCanProcessMnemonic();
#endif
            // If this is a Mdi child form, child controls should process mnemonics only if this is the active mdi child.
            if (this.IsMdiChild && (formStateEx[FormStateExMnemonicProcessed] == 1 || this != this.MdiParentInternal.ActiveMdiChildInternal || this.WindowState == FormWindowState.Minimized)){
                return false;
            }

            return base.CanProcessMnemonic();
        }

        /// <devdoc>
        ///     Overriden to handle MDI mnemonic processing properly.
        /// </devdoc>        
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
            // MDI container form has at least one control, the MDI client which contains the MDI children. We need
            // to allow the MDI children process the mnemonic before any other control in the MDI container (like
            // menu items or any other control).
            if( base.ProcessMnemonic( charCode ) ){
                return true;
            }

            if( this.IsMdiContainer ){
                // ContainerControl have already processed the active MDI child for us (if any) in the call above,
                // now process remaining controls (non-mdi children).
                if( this.Controls.Count > 1 ){ // Ignore the MdiClient control
                    for( int index = 0; index < this.Controls.Count; index++ ){
                        Control ctl = this.Controls[index];
                        if( ctl is MdiClient ){
                            continue;
                        }

                        if( ctl.ProcessMnemonic( charCode ) ){
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.CenterToParent"]/*' />
        /// <devdoc>
        ///     Centers the dialog to its parent.
        /// </devdoc>
        /// <internalonly/>
        protected void CenterToParent() {
            if (TopLevel) {
                Point p = new Point();
                Size s = Size;
                IntPtr ownerHandle = IntPtr.Zero;

                ownerHandle = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT);
                if (ownerHandle != IntPtr.Zero) {
                    Screen desktop = Screen.FromHandleInternal(ownerHandle);
                    Rectangle screenRect = desktop.WorkingArea;
                    NativeMethods.RECT ownerRect = new NativeMethods.RECT();

                    UnsafeNativeMethods.GetWindowRect(new HandleRef(null, ownerHandle), ref ownerRect);

                    p.X = (ownerRect.left + ownerRect.right - s.Width) / 2;
                    if (p.X < screenRect.X)
                        p.X = screenRect.X;
                    else if (p.X + s.Width > screenRect.X + screenRect.Width)
                        p.X = screenRect.X + screenRect.Width - s.Width;

                    p.Y = (ownerRect.top + ownerRect.bottom - s.Height) / 2;
                    if (p.Y < screenRect.Y)
                        p.Y = screenRect.Y;
                    else if (p.Y + s.Height > screenRect.Y + screenRect.Height)
                        p.Y = screenRect.Y + screenRect.Height - s.Height;

                    Location = p;
                }
                else {
                    CenterToScreen();
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.CenterToScreen"]/*' />
        /// <devdoc>
        ///     Centers the dialog to the screen. This will first attempt to use
        ///     the owner property to determine the correct screen, then
        ///     it will try the HWND owner of the form, and finally this will
        ///     center the form on the same monitor as the mouse cursor.
        /// </devdoc>
        /// <internalonly/>
        protected void CenterToScreen() {
            Point p = new Point();
            Screen desktop = null;
            if (OwnerInternal != null) {
                desktop = Screen.FromControl(OwnerInternal);
            }
            else {
                IntPtr hWndOwner = IntPtr.Zero;
                if (TopLevel) {
                    hWndOwner = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT);
                }
                if (hWndOwner != IntPtr.Zero) {
                    desktop = Screen.FromHandleInternal(hWndOwner);
                }
                else {
                    desktop = Screen.FromPoint(Control.MousePosition);
                }
            }
            Rectangle screenRect = desktop.WorkingArea;
            p.X = Math.Max(screenRect.X,screenRect.X + (screenRect.Width - Width)/2);
            p.Y = Math.Max(screenRect.Y,screenRect.Y + (screenRect.Height - Height)/2);
            Location = p;
        }

        /// <devdoc>
        ///     Invalidates the merged menu, forcing the menu to be recreated if
        ///     needed again.
        /// </devdoc>
        private void InvalidateMergedMenu() {
            // here, we just set the merged menu to null (indicating that the menu structure
            // needs to be rebuilt).  Then, we signal the parent to updated its menus.
            if (Properties.ContainsObject(PropMergedMenu)) {
                MainMenu menu = Properties.GetObject(PropMergedMenu) as MainMenu;
                if (menu != null && menu.ownerForm == this) {
                    menu.Dispose();
                }
                Properties.SetObject(PropMergedMenu, null);
            }

            Form parForm = ParentFormInternal;
            if (parForm != null) {
                parForm.MenuChanged(0, parForm.Menu);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.LayoutMdi"]/*' />
        /// <devdoc>
        ///    <para> Arranges the Multiple Document Interface
        ///       (MDI) child forms according to value.</para>
        /// </devdoc>
        public void LayoutMdi(MdiLayout value) {
            if (ctlClient == null){
                return;
            }
            ctlClient.LayoutMdi(value);
        }

        // Package scope for menu interop
        internal void MenuChanged(int change, Menu menu) {
            Form parForm = ParentFormInternal;
            if (parForm != null && this == parForm.ActiveMdiChildInternal) {
                parForm.MenuChanged(change, menu);
                return;
            }

            switch (change) {
                case Windows.Forms.Menu.CHANGE_ITEMS:
                case Windows.Forms.Menu.CHANGE_MERGE:
                    if (ctlClient == null || !ctlClient.IsHandleCreated) {
                        if (menu == Menu && change == Windows.Forms.Menu.CHANGE_ITEMS)
                            UpdateMenuHandles();
                        break;
                    }

                    // Tell the children to toss their mergedMenu.
                    if (IsHandleCreated) {
                        UpdateMenuHandles(null, false);
                    }

                    Control.ControlCollection children = ctlClient.Controls;
                    for (int i = children.Count; i-- > 0;) {
                        Control ctl = children[i];
                        if (ctl is Form && ctl.Properties.ContainsObject(PropMergedMenu)) {
                            MainMenu mainMenu = ctl.Properties.GetObject(PropMergedMenu) as MainMenu;
                            if (mainMenu != null && mainMenu.ownerForm == ctl) {
                                mainMenu.Dispose();
                            }
                            ctl.Properties.SetObject(PropMergedMenu, null);
                        }
                    }

                    UpdateMenuHandles();
                    break;
                case Windows.Forms.Menu.CHANGE_VISIBLE:
                    if (menu == Menu || (this.ActiveMdiChildInternal != null && menu == this.ActiveMdiChildInternal.Menu)) {
                        UpdateMenuHandles();
                    }
                    break;
                case Windows.Forms.Menu.CHANGE_MDI:
                    if (ctlClient != null && ctlClient.IsHandleCreated) {
                        UpdateMenuHandles();
                    }
                    break;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnActivated"]/*' />
        /// <devdoc>
        ///    <para>The activate event is fired when the form is activated.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnActivated(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_ACTIVATED];
            if (handler != null) handler(this,e);
        }

        /// <devdoc>
        ///     Override of AutoScaleModeChange method from ContainerControl.  We use
        ///     this to keep our own AutoScale property in sync.
        /// </devdoc>
        internal override void OnAutoScaleModeChanged() {
            base.OnAutoScaleModeChanged();
            // Obsolete code required here for backwards compat
#pragma warning disable 618
            if (formStateEx[FormStateExSettingAutoScale] != 1) {
                AutoScale = false;
            }
#pragma warning restore 618

        }

        protected override void OnBackgroundImageChanged(EventArgs e) {
            base.OnBackgroundImageChanged(e);
            if (IsMdiContainer)
            {
                MdiClient.BackgroundImage = BackgroundImage;
                // requires explicit invalidate to update the image.
                MdiClient.Invalidate();
            }
        }

        protected override void OnBackgroundImageLayoutChanged(EventArgs e) {
            base.OnBackgroundImageLayoutChanged(e);
            if (IsMdiContainer)
            {
                MdiClient.BackgroundImageLayout = BackgroundImageLayout;
                // requires explicit invalidate to update the imageLayout.
                MdiClient.Invalidate();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnClosing"]/*' />
        /// <devdoc>
        ///    <para>The Closing event is fired when the form is closed.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClosing(CancelEventArgs e) {
            CancelEventHandler handler = (CancelEventHandler)Events[EVENT_CLOSING];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnClosed"]/*' />
        /// <devdoc>
        ///    <para>The Closed event is fired when the form is closed.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClosed(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_CLOSED];
            if (handler != null) handler(this,e);
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnFormClosing"]/*' />
        /// <devdoc>
        ///    <para>The Closing event is fired before the form is closed.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFormClosing(FormClosingEventArgs e) {
            FormClosingEventHandler handler = (FormClosingEventHandler)Events[EVENT_FORMCLOSING];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnFormClosed"]/*' />
        /// <devdoc>
        ///    <para>The Closed event is fired when the form is closed.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFormClosed(FormClosedEventArgs e) {
            //Remove the form from Application.OpenForms (nothing happens if isn't present)
            Application.OpenFormsInternalRemove(this);

            FormClosedEventHandler handler = (FormClosedEventHandler)Events[EVENT_FORMCLOSED];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnCreateControl"]/*' />
        /// <devdoc>
        ///    <para> Raises the CreateControl event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnCreateControl() {
            CalledCreateControl = true;
            base.OnCreateControl();

            if (CalledMakeVisible && !CalledOnLoad) {
                CalledOnLoad = true;
                OnLoad(EventArgs.Empty);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnDeactivate"]/*' />
        /// <devdoc>
        /// <para>Raises the Deactivate event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDeactivate(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_DEACTIVATE];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnEnabledChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the EnabledChanged event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnEnabledChanged(EventArgs e) {
            base.OnEnabledChanged(e);
            if (!DesignMode && Enabled && Active) { 
                // Make sure we activate the active control.
                Control activeControl = ActiveControl;

                // Seems safe to call this here without demanding permissions, since we only
                // get here if this form is enabled and active.
                if( activeControl == null ){
                    SelectNextControlInternal(this, true, true, true, true);
                }
                else{
                    FocusActiveControlInternal();
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnEnter(EventArgs e) {
            base.OnEnter(e);

            // Enter events are not raised on mdi child form controls on form Enabled.
            if( IsMdiChild ){
                UpdateFocusedControl();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnFontChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnFontChanged(EventArgs e) {
            if (DesignMode) {
                UpdateAutoScaleBaseSize();
            }
            base.OnFontChanged(e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to find out when the
        ///     handle has been created.
        ///     Call base.OnHandleCreated first.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleCreated(EventArgs e) {
            formStateEx[FormStateExUseMdiChildProc] = (IsMdiChild && Visible) ? 1 : 0;
            base.OnHandleCreated(e);
            UpdateLayered();
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnHandleDestroyed"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Inheriting classes should override this method to find out when the
        ///    handle is about to be destroyed.
        ///    Call base.OnHandleDestroyed last.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleDestroyed(EventArgs e) {
            base.OnHandleDestroyed(e);
            formStateEx[FormStateExUseMdiChildProc] = 0;

            // just make sure we're no longer in the forms collection list
            Application.OpenFormsInternalRemove(this);

            // If the handle is being destroyed, and the security tip hasn't been dismissed
            // then we remove it from the property bag. When we come back around and get
            // an NCACTIVATE we will see that this is missing and recreate the security
            // tip in it's default state.
            //
            ResetSecurityTip(true /* modalOnly */);
        }

        /// <internalonly/>
        /// <devdoc>
        ///    Handles the event that a helpButton is clicked
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHelpButtonClicked(CancelEventArgs e) {
            CancelEventHandler handler = (CancelEventHandler)Events[EVENT_HELPBUTTONCLICKED];
            if (handler != null) {
                handler(this,e);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnLayout"]/*' />
        protected override void OnLayout(LayoutEventArgs levent) {

            // Typically forms are either top level or parented to an MDIClient area.
            // In either case, forms a responsible for managing their own size.
            if(AutoSize /*&& DesignMode*/) {
                // If AutoSized, set the Form to the maximum of its preferredSize or the user
                // specified size.
                Size prefSize = this.PreferredSize;
                minAutoSize = prefSize;

                // This used to use "GetSpecifiedBounds" - but it was not updated when we're in the middle of
                // a modal resizing loop (WM_WINDOWPOSCHANGED).
                Size adjustedSize = AutoSizeMode == AutoSizeMode.GrowAndShrink ? prefSize : LayoutUtils.UnionSizes(prefSize, Size);

                IArrangedElement form = this as IArrangedElement;
                if (form != null) {
                    form.SetBounds(new Rectangle(this.Left, this.Top, adjustedSize.Width, adjustedSize.Height), BoundsSpecified.None);
                }
            }
            base.OnLayout(levent);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnLoad"]/*' />
        /// <devdoc>
        ///    <para>The Load event is fired before the form becomes visible for the first time.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLoad(EventArgs e) {
            //First - add the form to Application.OpenForms
            Application.OpenFormsInternalAdd(this);
            if (Application.UseWaitCursor) {
                this.UseWaitCursor = true;
            }

            // subhag: This will apply AutoScaling to the form just
            // before the form becomes visible.
            //
            if (formState[FormStateAutoScaling] == 1 && !DesignMode) {
                // Turn off autoscaling so we don't do this on every handle
                // creation.
                //
                formState[FormStateAutoScaling] = 0;
                // Obsolete code required here for backwards compat
#pragma warning disable 618
                ApplyAutoScaling();
#pragma warning restore 618
                
            }

/*
            /// 












*/

            // Also, at this time we can now locate the form the the correct
            // area of the screen.  We must do this after applying any
            // autoscaling.
            //
            if (GetState(STATE_MODAL)) {
                FormStartPosition startPos = (FormStartPosition)formState[FormStateStartPos];
                if (startPos == FormStartPosition.CenterParent) {
                    CenterToParent();
                }
                else if (startPos == FormStartPosition.CenterScreen) {
                    CenterToScreen();
                }
            }

            // There is no good way to explain this event except to say
            // that it's just another name for OnControlCreated.
            EventHandler handler = (EventHandler)Events[EVENT_LOAD];
            if (handler != null) {
                string text = Text;

                handler(this,e);


                // It seems that if you set a window style during the onload
                // event, we have a problem initially painting the window.
                // So in the event that the user has set the on load event
                // in their application, we should go ahead and invalidate
                // the controls in their collection so that we paint properly.
                // This seems to manifiest itself in changes to the window caption,
                // and changes to the control box and help.

                foreach (Control c in Controls) {
                    c.Invalidate();
                }
            }

            //finally fire the newOnShown(unless the form has already been closed)
            if (IsHandleCreated)
            {
                this.BeginInvoke(new MethodInvoker(CallShownEvent));
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnMaximizedBoundsChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMaximizedBoundsChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_MAXIMIZEDBOUNDSCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnMaximumSizeChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMaximumSizeChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_MAXIMUMSIZECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnMinimumSizeChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMinimumSizeChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_MINIMUMSIZECHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnInputLanguageChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Form.InputLanguageChanged'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnInputLanguageChanged(InputLanguageChangedEventArgs e) {
            InputLanguageChangedEventHandler handler = (InputLanguageChangedEventHandler)Events[EVENT_INPUTLANGCHANGE];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnInputLanguageChanging"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Form.InputLanguageChanging'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnInputLanguageChanging(InputLanguageChangingEventArgs e) {
            InputLanguageChangingEventHandler handler = (InputLanguageChangingEventHandler)Events[EVENT_INPUTLANGCHANGEREQUEST];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnVisibleChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnVisibleChanged(EventArgs e) {
            UpdateRenderSizeGrip();
            Form mdiParent = MdiParentInternal;
            if (mdiParent != null) {
                mdiParent.UpdateMdiWindowListStrip();
            }
            base.OnVisibleChanged(e);

            // windows forms have to behave like dialog boxes sometimes. If the
            // user has specified that the mouse should snap to the
            // Accept button using the Mouse applet in the control panel,
            // we have to respect that setting each time our form is made visible.
            bool data = false;
            if (IsHandleCreated
                    && Visible
                    && (AcceptButton != null)
                    && UnsafeNativeMethods.SystemParametersInfo(NativeMethods.SPI_GETSNAPTODEFBUTTON, 0, ref data, 0)
                    && data) {

                Control button = AcceptButton as Control;
                NativeMethods.POINT ptToSnap = new NativeMethods.POINT(
                    button.Left + button.Width / 2,
                    button.Top + button.Height / 2);

                UnsafeNativeMethods.ClientToScreen(new HandleRef(this, Handle), ptToSnap);
                if (!button.IsWindowObscured) {
                    // 

                    IntSecurity.AdjustCursorPosition.Assert();
                    try {
                        Cursor.Position = new Point(ptToSnap.x, ptToSnap.y);
                    }
                    finally {
                        System.Security.CodeAccessPermission.RevertAssert();
                    }
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnMdiChildActivate"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Form.MdiChildActivate'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMdiChildActivate(EventArgs e) {
            UpdateMenuHandles();
            UpdateToolStrip();
            EventHandler handler = (EventHandler)Events[EVENT_MDI_CHILD_ACTIVATE];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnMenuStart"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Form.MenuStart'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMenuStart(EventArgs e) {
            SecurityToolTip secTip = (SecurityToolTip)Properties.GetObject(PropSecurityTip);
            if (secTip != null) {
                secTip.Pop(true /*noLongerFirst*/);
            }
            EventHandler handler = (EventHandler)Events[EVENT_MENUSTART];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnMenuComplete"]/*' />
        /// <devdoc>
        /// <para>Raises the MenuComplete event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMenuComplete(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_MENUCOMPLETE];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnPaint"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Raises the Paint event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            if (formState[FormStateRenderSizeGrip] != 0) {
                Size sz = ClientSize;
                if (Application.RenderWithVisualStyles) {
                    if (sizeGripRenderer == null) {
                        sizeGripRenderer = new VisualStyleRenderer(VisualStyleElement.Status.Gripper.Normal);
                    }

                    sizeGripRenderer.DrawBackground(e.Graphics, new Rectangle(sz.Width - SizeGripSize, sz.Height - SizeGripSize, SizeGripSize, SizeGripSize));
                }
                else {
                    ControlPaint.DrawSizeGrip(e.Graphics, BackColor, sz.Width - SizeGripSize, sz.Height - SizeGripSize, SizeGripSize, SizeGripSize);
                }
            }
            
            if (IsMdiContainer) {
                e.Graphics.FillRectangle(SystemBrushes.AppWorkspace, ClientRectangle);
            }
        }

       /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnResize"]/*' />
       /// <internalonly/>
        /// <devdoc>
        ///    <para>Raises the Resize event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (formState[FormStateRenderSizeGrip] != 0) {
                Invalidate();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnDpiChanged"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Raises the DpiChanged event.</para>
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        protected virtual void OnDpiChanged(DpiChangedEventArgs e) {
            if (e.DeviceDpiNew != e.DeviceDpiOld) {
                CommonProperties.xClearAllPreferredSizeCaches(this);

                // call any additional handlers
                ((DpiChangedEventHandler)Events[EVENT_DPI_CHANGED])?.Invoke(this, e);

                if (!e.Cancel) {
                    float factor = (float)e.DeviceDpiNew / (float)e.DeviceDpiOld;
                    SuspendAllLayout(this);
                    try {
                        SafeNativeMethods.SetWindowPos(new HandleRef(this, HandleInternal), NativeMethods.NullHandleRef, e.SuggestedRectangle.X, e.SuggestedRectangle.Y, e.SuggestedRectangle.Width, e.SuggestedRectangle.Height, NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
                        if (AutoScaleMode != AutoScaleMode.Font) {
                            Font = new Font(this.Font.FontFamily, this.Font.Size * factor, this.Font.Style);
                            FormDpiChanged(factor);
                        }
                        else {
                            ScaleFont(factor);
                            FormDpiChanged(factor);
                        }
                    }
                    finally {
                        // We want to perform layout for dpi-changed HDpi improvements - setting the second parameter to 'true'
                        ResumeAllLayout(this, true);
                    }
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.DpiChanged"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the DPI resolution of the screen this top level window is displayed on changes, 
        ///    either when the top level window is moved between monitors or when the OS settings are changed.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.FormOnDpiChangedDescr))]
        public event DpiChangedEventHandler DpiChanged {
            add {
                Events.AddHandler(EVENT_DPI_CHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DPI_CHANGED, value);
            }
        }

        /// <devdoc>
        ///     Handles the WM_DPICHANGED message
        /// </devdoc>
        private void WmDpiChanged(ref Message m) {
            DefWndProc(ref m);

            DpiChangedEventArgs e = new DpiChangedEventArgs(deviceDpi, m);
            deviceDpi = e.DeviceDpiNew;
            
            OnDpiChanged(e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnGetDpiScaledSize"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Allows derived form to handle WM_GETDPISCALEDSIZE message.</para>
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool OnGetDpiScaledSize(int deviceDpiOld, int deviceDpiNew, ref Size desiredSize) {
            return false; // scale linearly
        }

        /// <devdoc>
        ///     Handles the WM_GETDPISCALEDSIZE message, this is a chance for the application to 
        ///     scale window size non-lineary. If this message is not processed, the size is scaled linearly by Windows.
        ///     This message is sent to top level windows before WM_DPICHANGED.
        ///     If the application responds to this message, the resulting size will be the candidate rectangle 
        ///     sent to WM_DPICHANGED. The WPARAM contains a DPI value. The size needs to be computed if 
        ///     the window were to switch to this DPI. LPARAM is unused and will be zero. 
        ///     The return value is a size, where the LOWORD is the desired width of the window and the HIWORD 
        ///     is the desired height of the window. A return value of zero indicates that the app does not 
        ///     want any special behavior and the candidate rectangle will be computed linearly.
        /// </devdoc>
        private void WmGetDpiScaledSize(ref Message m) {
            DefWndProc(ref m);
            
            Size desiredSize = new Size();
            if (OnGetDpiScaledSize(deviceDpi, NativeMethods.Util.SignedLOWORD(m.WParam), ref desiredSize)) {
                m.Result = (IntPtr)(unchecked((Size.Height & 0xFFFF) << 16) | (Size.Width & 0xFFFF)); 
            } else {
                m.Result = IntPtr.Zero;
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnRightToLeftLayoutChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (RightToLeft == RightToLeft.Yes) {
                RecreateHandle();
            }

            EventHandler eh = Events[EVENT_RIGHTTOLEFTLAYOUTCHANGED] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            // Want to do this after we fire the event.
            if (RightToLeft == RightToLeft.Yes) {
                foreach(Control c in this.Controls) {
                    c.RecreateHandleCore();
                }
            }
            
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnShown"]/*' />
        /// <devdoc>
        ///    <para>Thi event fires whenever the form is first shown.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnShown(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_SHOWN];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnTextChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnTextChanged(EventArgs e) {
            base.OnTextChanged(e);

            // If there is no control box, there should only be a title bar if text != "".
            int newTextEmpty = Text.Length == 0 ? 1 : 0;
            if (!ControlBox && formState[FormStateIsTextEmpty] != newTextEmpty)
                this.RecreateHandle();

            formState[FormStateIsTextEmpty] = newTextEmpty;
        }

        /// <devdoc>
        ///     Simulates a InputLanguageChanged event. Used by Control to forward events
        ///     to the parent form.
        /// </devdoc>
        /// <internalonly/>
        internal void PerformOnInputLanguageChanged(InputLanguageChangedEventArgs iplevent) {
            OnInputLanguageChanged(iplevent);
        }

        /// <devdoc>
        ///     Simulates a InputLanguageChanging event. Used by Control to forward
        ///     events to the parent form.
        /// </devdoc>
        /// <internalonly/>
        internal void PerformOnInputLanguageChanging(InputLanguageChangingEventArgs iplcevent) {
            OnInputLanguageChanging(iplcevent);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ProcessCmdKey"]/*' />
        /// <devdoc>
        ///     Processes a command key. Overrides Control.processCmdKey() to provide
        ///     additional handling of main menu command keys and Mdi accelerators.
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData) {
            if (base.ProcessCmdKey(ref msg, keyData)) return true;

            MainMenu curMenu = (MainMenu)Properties.GetObject(PropCurMenu);
            if (curMenu != null && curMenu.ProcessCmdKey(ref msg, keyData)) return true;

            // Process MDI accelerator keys.

            bool retValue = false;

            NativeMethods.MSG win32Message = new NativeMethods.MSG();
            win32Message.message = msg.Msg;
            win32Message.wParam = msg.WParam;
            win32Message.lParam = msg.LParam;
            win32Message.hwnd = msg.HWnd;

            if (ctlClient != null && ctlClient.Handle != IntPtr.Zero &&
                UnsafeNativeMethods.TranslateMDISysAccel(ctlClient.Handle, ref win32Message)) {

                retValue = true;
            }

            msg.Msg = win32Message.message;
            msg.WParam = win32Message.wParam;
            msg.LParam = win32Message.lParam;
            msg.HWnd = win32Message.hwnd;

            return retValue;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ProcessDialogKey"]/*' />
        /// <devdoc>
        ///     Processes a dialog key. Overrides Control.processDialogKey(). This
        ///     method implements handling of the RETURN, and ESCAPE keys in dialogs.
        /// The method performs no processing on keys that include the ALT or
        ///     CONTROL modifiers.
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData) {
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None) {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                IButtonControl button;

                switch (keyCode) {
                    case Keys.Return:
                        button = (IButtonControl)Properties.GetObject(PropDefaultButton);
                        if (button != null) {
                            //PerformClick now checks for validationcancelled...
                            if (button is Control) {
                                button.PerformClick();
                            }
                            return true;
                        }
                        break;
                    case Keys.Escape:
                        button = (IButtonControl)Properties.GetObject(PropCancelButton);
                        if (button != null) {
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

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ProcessDialogChar"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Processes a dialog character For a MdiChild.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogChar(char charCode) {
#if DEBUG
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Form.ProcessDialogChar [" + charCode.ToString() + "]");
#endif
            // If we're the top-level form or control, we need to do the mnemonic handling
            //
            if (this.IsMdiChild && charCode != ' ') {
                if (ProcessMnemonic(charCode)) {
                    return true;
                }

                // ContainerControl calls ProcessMnemonic starting from the active MdiChild form (this)
                // so let's flag it as processed.
                formStateEx[FormStateExMnemonicProcessed] = 1;
                try{
                    return base.ProcessDialogChar(charCode);
                }
                finally{
                    formStateEx[FormStateExMnemonicProcessed] = 0;
                }
            }

            // Non-MdiChild form, just pass the call to ContainerControl.
            return base.ProcessDialogChar(charCode);
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ProcessKeyPreview"]/*' />
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected override bool ProcessKeyPreview(ref Message m) {
            if (formState[FormStateKeyPreview] != 0 && ProcessKeyEventArgs(ref m))
                return true;
            return base.ProcessKeyPreview(ref m);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ProcessTabKey"]/*' />
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessTabKey(bool forward) {
            if (SelectNextControl(ActiveControl, forward, true, true, true))
                return true;

            // I've added a special case for UserControls because they shouldn't cycle back to the
            // beginning if they don't have a parent form, such as when they're on an ActiveXBridge.
            if (IsMdiChild || ParentFormInternal == null) {
                bool selected = SelectNextControl(null, forward, true, true, false);

                if (selected) {
                    return true;
                }
            }

            return false;
        }


        /// <internalonly/>
        /// <devdoc>
        ///    <para>Raises the FormClosed event for this form when Application.Exit is called.</para>
        /// </devdoc>
        internal void RaiseFormClosedOnAppExit() {
            if (!Modal) {
                /* This is not required because Application.ExitPrivate() loops through all forms in the Application.OpenForms collection
                // Fire FormClosed event on all MDI children
                if (IsMdiContainer) {
                    FormClosedEventArgs fce = new FormClosedEventArgs(CloseReason.MdiFormClosing);
                    foreach(Form mdiChild in MdiChildren) {
                        if (mdiChild.IsHandleCreated) {
                            mdiChild.OnFormClosed(fce);
                        }
                    }
                }
                */

                // Fire FormClosed event on all the forms that this form owns and are not in the Application.OpenForms collection
                // This is to be consistent with what WmClose does.
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                if (ownedFormsCount > 0) {
                    Form[] ownedForms = this.OwnedForms;
                    FormClosedEventArgs fce = new FormClosedEventArgs(CloseReason.FormOwnerClosing);
                    for (int i = ownedFormsCount-1 ; i >= 0; i--) {
                        if (ownedForms[i] != null && !Application.OpenFormsInternal.Contains(ownedForms[i])) {
                            ownedForms[i].OnFormClosed(fce);
                        }
                    }
                }
            }
            OnFormClosed(new FormClosedEventArgs(CloseReason.ApplicationExitCall));
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>Raises the FormClosing event for this form when Application.Exit is called.
        ///          Returns e.Cancel returned by the event handler.</para>
        /// </devdoc>
        internal bool RaiseFormClosingOnAppExit() {
            FormClosingEventArgs e = new FormClosingEventArgs(CloseReason.ApplicationExitCall, false);
            // e.Cancel = !Validate(true);    This would cause a breaking change between v2.0 and v1.0/v1.1 in case validation fails.
            if (!Modal) {
                /* This is not required because Application.ExitPrivate() loops through all forms in the Application.OpenForms collection
                // Fire FormClosing event on all MDI children
                if (IsMdiContainer) {
                    FormClosingEventArgs fce = new FormClosingEventArgs(CloseReason.MdiFormClosing, e.Cancel);
                    foreach(Form mdiChild in MdiChildren) {
                        if (mdiChild.IsHandleCreated) {
                            mdiChild.OnFormClosing(fce);
                            if (fce.Cancel) {
                                e.Cancel = true;
                                break;
                            }
                        }
                    }
                }
                */

                // Fire FormClosing event on all the forms that this form owns and are not in the Application.OpenForms collection
                // This is to be consistent with what WmClose does.
                int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                if (ownedFormsCount > 0) {
                    Form[] ownedForms = this.OwnedForms;
                    FormClosingEventArgs fce = new FormClosingEventArgs(CloseReason.FormOwnerClosing, false);
                    for (int i = ownedFormsCount - 1; i >= 0; i--) {
                        if (ownedForms[i] != null && !Application.OpenFormsInternal.Contains(ownedForms[i])) {
                            ownedForms[i].OnFormClosing(fce);
                            if (fce.Cancel) {
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

        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")
        ]
        internal override void RecreateHandleCore() {
            //Debug.Assert( CanRecreateHandle(), "Recreating handle when form is not ready yet." );
            NativeMethods.WINDOWPLACEMENT wp = new NativeMethods.WINDOWPLACEMENT();
            FormStartPosition oldStartPosition = FormStartPosition.Manual;

            if (!IsMdiChild && (WindowState == FormWindowState.Minimized || WindowState == FormWindowState.Maximized)) {
                wp.length = Marshal.SizeOf(typeof(NativeMethods.WINDOWPLACEMENT));
                UnsafeNativeMethods.GetWindowPlacement(new HandleRef(this, Handle), ref wp);
            }

            if (StartPosition != FormStartPosition.Manual) {
                oldStartPosition = StartPosition;
                // Set the startup postion to manual, to stop the form from
                // changing position each time RecreateHandle() is called.
                StartPosition = FormStartPosition.Manual;
            }

            EnumThreadWindowsCallback etwcb = null;
            SafeNativeMethods.EnumThreadWindowsCallback callback = null;
            if (IsHandleCreated) {
                // First put all the owned windows into a list
                etwcb = new EnumThreadWindowsCallback();
                if (etwcb != null) {
                    callback = new SafeNativeMethods.EnumThreadWindowsCallback(etwcb.Callback);
                    UnsafeNativeMethods.EnumThreadWindows(SafeNativeMethods.GetCurrentThreadId(),
                                                          new NativeMethods.EnumThreadWindowsCallback(callback),
                                                          new HandleRef(this, this.Handle));
                    // Reset the owner of the windows in the list
                    etwcb.ResetOwners();
                }
            }

            base.RecreateHandleCore();


            if (etwcb != null) {
                // Set the owner of the windows in the list back to the new Form's handle
                etwcb.SetOwners(new HandleRef(this, this.Handle));
            }
            
            if (oldStartPosition != FormStartPosition.Manual) {
                StartPosition = oldStartPosition;
            }

            if (wp.length > 0) {
                UnsafeNativeMethods.SetWindowPlacement(new HandleRef(this, Handle), ref wp);
            }

            if (callback != null) {
                GC.KeepAlive(callback);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.RemoveOwnedForm"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Removes a form from the list of owned forms. Also sets the owner of the
        ///       removed form to null.
        ///    </para>
        /// </devdoc>
        public void RemoveOwnedForm(Form ownedForm) {
            if (ownedForm == null)
                return;

            if (ownedForm.OwnerInternal != null) {
                ownedForm.Owner = null; // NOTE: this will call RemoveOwnedForm again, bypassing if.
                return;
            }

            Form[] ownedForms = (Form[])Properties.GetObject(PropOwnedForms);
            int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);

            if (ownedForms != null) {
                for (int i = 0; i < ownedFormsCount; i++) {
                    if (ownedForm.Equals(ownedForms[i])) {

                        // clear out the reference.
                        //
                        ownedForms[i] = null;

                        // compact the array.
                        //
                        if (i + 1 < ownedFormsCount) {
                            Array.Copy(ownedForms, i + 1, ownedForms, i, ownedFormsCount - i - 1);
                            ownedForms[ownedFormsCount - 1] = null;
                        }
                        ownedFormsCount--;
                    }
                }

                Properties.SetInteger(PropOwnedFormsCount, ownedFormsCount);
            }
        }

        /// <devdoc>
        ///     Resets the form's icon the the default value.
        /// </devdoc>
        private void ResetIcon() {
            icon = null;
            if (smallIcon != null) {
                smallIcon.Dispose();
                smallIcon = null;
            }
            formState[FormStateIconSet] = 0;
            UpdateWindowIcon(true);
        }

        void ResetSecurityTip(bool modalOnly) {
            SecurityToolTip secTip = (SecurityToolTip)Properties.GetObject(PropSecurityTip);
            if (secTip != null) {
                if ((modalOnly && secTip.Modal) || !modalOnly) {
                    secTip.Dispose();
                    secTip = null;
                    Properties.SetObject(PropSecurityTip, null);
                }
            }
        }

        /// <devdoc>
        ///     Resets the TransparencyKey to Color.Empty.
        /// </devdoc>
        private void ResetTransparencyKey() {
            TransparencyKey = Color.Empty;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ResizeBegin"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the form enters the sizing modal loop</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.FormOnResizeBeginDescr))]
        public event EventHandler ResizeBegin {
            add {
                Events.AddHandler(EVENT_RESIZEBEGIN, value);
            }
            remove {
                Events.RemoveHandler(EVENT_RESIZEBEGIN, value);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ResizeEnd"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control exits the sizing modal loop.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.FormOnResizeEndDescr))]
        public event EventHandler ResizeEnd {
            add {
                Events.AddHandler(EVENT_RESIZEEND, value);
            }
            remove {
                Events.RemoveHandler(EVENT_RESIZEEND, value);
            }
        }

        /// <devdoc>
        ///     This is called when we have just been restored after being
        ///     minimized.  At this point we resume our layout.
        /// </devdoc>
        private void ResumeLayoutFromMinimize() {
            // If we're currently minimized, resume our layout because we are
            // about to snap out of it.
            if (formState[FormStateWindowState] == (int)FormWindowState.Minimized) {
                ResumeLayout();
            }
        }

        // If someone set Location or Size while the form was maximized or minimized,
        // we had to cache the new value away until after the form was restored to normal size.
        // This function is called after WindowState changes, and handles the above logic.
        // In the normal case where no one sets Location or Size programmatically,
        // Windows does the restoring for us.
        //
        private void RestoreWindowBoundsIfNecessary() {
            if (WindowState == FormWindowState.Normal) {
                Size restoredSize = restoredWindowBounds.Size;
                if ((restoredWindowBoundsSpecified & BoundsSpecified.Size) != 0) {
                    restoredSize = SizeFromClientSize(restoredSize.Width, restoredSize.Height);
                }
                SetBounds(restoredWindowBounds.X, restoredWindowBounds.Y,
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize]==1 ? restoredSize.Width : restoredWindowBounds.Width,
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize]==1 ? restoredSize.Height : restoredWindowBounds.Height,
                          restoredWindowBoundsSpecified);
                restoredWindowBoundsSpecified = 0;
                restoredWindowBounds = new Rectangle(-1, -1, -1, -1);
                formStateEx[FormStateExWindowBoundsHeightIsClientSize] = 0;
                formStateEx[FormStateExWindowBoundsWidthIsClientSize] = 0;
            }
        }

        void RestrictedProcessNcActivate() {
            Debug.Assert(IsRestrictedWindow, "This should only be called for restricted windows");

            // Ignore if tearing down...
            //
            if (IsDisposed || Disposing) {
                return;
            }

            // Note that this.Handle does not get called when the handle hasn't been created yet
            //
            SecurityToolTip secTip = (SecurityToolTip)Properties.GetObject(PropSecurityTip);
            if (secTip == null) {
                if (IsHandleCreated && UnsafeNativeMethods.GetForegroundWindow() == this.Handle) {
                    secTip = new SecurityToolTip(this);
                    Properties.SetObject(PropSecurityTip, secTip);
                }
            }
            else if (!IsHandleCreated || UnsafeNativeMethods.GetForegroundWindow() != this.Handle)
            {
                secTip.Pop(false /*noLongerFirst*/);
            }
            else
            {
                secTip.Show();
            }
        }

        /// <devdoc>
        ///     Decrements updateMenuHandleSuspendCount. If updateMenuHandleSuspendCount
        ///     becomes zero and updateMenuHandlesDeferred is true, updateMenuHandles
        ///     is called.
        /// </devdoc>
        private void ResumeUpdateMenuHandles() {
            int suspendCount = formStateEx[FormStateExUpdateMenuHandlesSuspendCount];
            if (suspendCount <= 0) {
                throw new InvalidOperationException(SR.TooManyResumeUpdateMenuHandles);
            }

            formStateEx[FormStateExUpdateMenuHandlesSuspendCount] = --suspendCount;
            if (suspendCount == 0 && formStateEx[FormStateExUpdateMenuHandlesDeferred] != 0) {
                UpdateMenuHandles();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Select"]/*' />
        /// <devdoc>
        ///     Selects this form, and optionally selects the next/previous control.
        /// </devdoc>
        protected override void Select(bool directed, bool forward) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ModifyFocus Demanded");
            IntSecurity.ModifyFocus.Demand();
            
            SelectInternal(directed, forward);
        }

        /// <devdoc>
        ///     Selects this form, and optionally selects the next/previous control.
        ///     Does the actual work without the security check.
        /// </devdoc>
        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        private void SelectInternal(bool directed, bool forward) {
            // SelectNextControl & set_ActiveControl below demand a ModifyFocus permission, let's revert it.
            // Reviewed : This method is to be called by Form only so asserting here is safe.
            //
            IntSecurity.ModifyFocus.Assert();

            if (directed) {
                SelectNextControl(null, forward, true, true, false);
            }

            if (TopLevel) {
                UnsafeNativeMethods.SetActiveWindow(new HandleRef(this, Handle));
            }
            else if (IsMdiChild) {
                UnsafeNativeMethods.SetActiveWindow(new HandleRef(MdiParentInternal, MdiParentInternal.Handle));
                MdiParentInternal.MdiClient.SendMessage(NativeMethods.WM_MDIACTIVATE, Handle, 0);
            }
            else {
                Form form = ParentFormInternal;
                if (form != null) form.ActiveControl = this;
            }
            
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ScaleCore"]/*' />
        /// <devdoc>
        ///     Base function that performs scaling of the form.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override void ScaleCore(float x, float y) {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, GetType().Name + "::ScaleCore(" + x + ", " + y + ")");
            SuspendLayout();
            try {
                if (WindowState == FormWindowState.Normal) {
                    //Get size values in advance to prevent one change from affecting another.
                    Size clientSize = ClientSize;
                    Size minSize = MinimumSize;
                    Size maxSize = MaximumSize;
                    if (!MinimumSize.IsEmpty) {
                        MinimumSize = ScaleSize(minSize, x, y);
                    }
                    if (!MaximumSize.IsEmpty) {
                        MaximumSize = ScaleSize(maxSize, x, y);
                    }
                    ClientSize = ScaleSize(clientSize, x, y);
                }

                ScaleDockPadding(x, y);

                foreach(Control control in Controls) {
                    if (control != null) {
#pragma warning disable 618
                        control.Scale(x, y);
#pragma warning restore 618
                    }
                }
            }
            finally {
                ResumeLayout();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.GetScaledBounds"]/*' />
        /// <devdoc>
        ///     For overrides this to calculate scaled bounds based on the restored rect
        ///     if it is maximized or minimized.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) {
            // If we're maximized or minimized, scale using the restored bounds, not
            // the real bounds.
            if (WindowState != FormWindowState.Normal) {
                bounds = RestoreBounds;
            }
            return base.GetScaledBounds(bounds, factor, specified);
        }
        
        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ScaleControl"]/*' />
        /// <devdoc>
        ///     Scale this form.  Form overrides this to enforce a maximum / minimum size.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {

            formStateEx[FormStateExInScale] = 1;
            try {
                
                // don't scale the location of MDI child forms
                if (MdiParentInternal != null) {
                    specified &= ~BoundsSpecified.Location;
                }

                base.ScaleControl(factor, specified);
            }
            finally {
                formStateEx[FormStateExInScale] = 0;
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.SetBoundsCore"]/*' />
        /// <devdoc>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {

            if (WindowState != FormWindowState.Normal) {
                // See RestoreWindowBoundsIfNecessary for an explanation of this
                // Only restore position when x,y is not -1,-1
                if (x != -1 || y != -1)
                {
                    restoredWindowBoundsSpecified |= (specified & (BoundsSpecified.X | BoundsSpecified.Y));
                }
                restoredWindowBoundsSpecified |= (specified & (BoundsSpecified.Width | BoundsSpecified.Height));

                if ((specified & BoundsSpecified.X) != 0)
                    restoredWindowBounds.X = x;
                if ((specified & BoundsSpecified.Y) != 0)
                    restoredWindowBounds.Y = y;
                if ((specified & BoundsSpecified.Width) != 0) {
                    restoredWindowBounds.Width = width;
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] = 0;
                }
                if ((specified & BoundsSpecified.Height) != 0) {
                    restoredWindowBounds.Height = height;
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] = 0;
                }
            }

            //Update RestoreBounds
            if ((specified & BoundsSpecified.X) != 0)
                restoreBounds.X = x;
            if ((specified & BoundsSpecified.Y) != 0)
                restoreBounds.Y = y;
            if ((specified & BoundsSpecified.Width) != 0 || restoreBounds.Width == -1)
                restoreBounds.Width = width;
            if ((specified & BoundsSpecified.Height) != 0 || restoreBounds.Height == -1)
                restoreBounds.Height = height;

            // Enforce maximum size...
            //
            if (WindowState == FormWindowState.Normal
                && (this.Height != height || this.Width != width)) {

                Size max = SystemInformation.MaxWindowTrackSize;
                if (height > max.Height) {
                    height = max.Height;
                }
                if (width > max.Width) {
                    width = max.Width;
                }
            }

            // Only enforce the minimum size if the form has a border and is a top
            // level form.
            //
            FormBorderStyle borderStyle = FormBorderStyle;
            if (borderStyle != FormBorderStyle.None
                && borderStyle != FormBorderStyle.FixedToolWindow
                && borderStyle != FormBorderStyle.SizableToolWindow
                && ParentInternal == null) {

                Size min = SystemInformation.MinWindowTrackSize;
                if (height < min.Height) {
                    height = min.Height;
                }
                if (width < min.Width) {
                    width = min.Width;
                }
            }

            if (IsRestrictedWindow) {
                // Check to ensure that the title bar, and all corners of the window, are visible on a monitor
                //

                Rectangle adjustedBounds = ApplyBoundsConstraints(x,y,width,height);
                if (adjustedBounds != new Rectangle(x,y,width,height)) {
                    
                    // 




                    base.SetBoundsCore(adjustedBounds.X, adjustedBounds.Y, adjustedBounds.Width, adjustedBounds.Height, BoundsSpecified.All);
                    return;
                }
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight) {
            // apply min/max size constraints
            Rectangle adjustedBounds = base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, proposedHeight);
            // run through size restrictions in Internet.
            if (IsRestrictedWindow) {
                // Check to ensure that the title bar, and all corners of the window, are visible on a monitor
                //

                Screen[] screens = Screen.AllScreens;
                bool topLeft = false;
                bool topRight = false;
                bool bottomLeft = false;
                bool bottomRight = false;

                for (int i=0; i<screens.Length; i++) {
                    Rectangle current = screens[i].WorkingArea;
                    if (current.Contains(suggestedX, suggestedY)) {
                        topLeft = true;
                    }
                    if (current.Contains(suggestedX + proposedWidth, suggestedY)) {
                        topRight = true;
                    }
                    if (current.Contains(suggestedX, suggestedY + proposedHeight)) {
                        bottomLeft = true;
                    }
                    if (current.Contains(suggestedX + proposedWidth, suggestedY + proposedHeight)) {
                        bottomRight = true;
                    }
                }

                // 


          
                if (!(topLeft && topRight && bottomLeft && bottomRight)) {
                    if (formStateEx[FormStateExInScale] == 1) {
                        // Constrain to screen working area bounds.  concern here is that
                        // an autoscale'ed dialog would validly scale itself larger than the 
                        // screen, but would fail our size/location restrictions - and we'd 
                        // restore back to the original unscaled sized.
                        
                        // Allow AutoScale to expand out to the screen bounds
                        adjustedBounds = WindowsFormsUtils.ConstrainToScreenWorkingAreaBounds(adjustedBounds);
                    }
                    else {
                        // COMPAT with Everett - ignore the size change.
                        // possible programatic attempt to scooch off the screen.
                        // restore the last size/location the user had.
                        adjustedBounds.X = Left;
                        adjustedBounds.Y = Top;
                        adjustedBounds.Width = Width;
                        adjustedBounds.Height = Height;
                    }
                  
                }
            }
            return adjustedBounds;
        }

        /// <devdoc>
        ///     Sets the defaultButton for the form. The defaultButton is "clicked" when
        ///     the user presses Enter.
        /// </devdoc>
        private void SetDefaultButton(IButtonControl button) {
            IButtonControl defaultButton = (IButtonControl)Properties.GetObject(PropDefaultButton);

            if (defaultButton != button) {
                if (defaultButton != null) defaultButton.NotifyDefault(false);
                Properties.SetObject(PropDefaultButton, button);
                if (button != null) button.NotifyDefault(true);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.SetClientSizeCore"]/*' />
        /// <devdoc>
        ///     Sets the clientSize of the form. This will adjust the bounds of the form
        ///     to make the clientSize the requested size.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void SetClientSizeCore(int x, int y) {
            bool hadHScroll = HScroll, hadVScroll = VScroll;
            base.SetClientSizeCore(x, y);

            if (IsHandleCreated) {
                // Adjust for the scrollbars, if they were introduced by
                // the call to base.SetClientSizeCore
                if (VScroll != hadVScroll) {
                    if (VScroll) x += SystemInformation.VerticalScrollBarWidth;
                }
                if (HScroll != hadHScroll) {
                    if (HScroll) y += SystemInformation.HorizontalScrollBarHeight;
                }
                if (x != ClientSize.Width || y != ClientSize.Height) {
                    base.SetClientSizeCore(x, y);
                }
            }
            formState[FormStateSetClientSize] = 1;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.SetDesktopBounds"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the bounds of the form in desktop coordinates.</para>
        /// </devdoc>
        public void SetDesktopBounds(int x, int y, int width, int height) {
            Rectangle workingArea = SystemInformation.WorkingArea;
            SetBounds(x + workingArea.X, y + workingArea.Y, width, height, BoundsSpecified.All);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.SetDesktopLocation"]/*' />
        /// <devdoc>
        ///    <para>Sets the location of the form in desktop coordinates.</para>
        /// </devdoc>
        public void SetDesktopLocation(int x, int y) {
            Rectangle workingArea = SystemInformation.WorkingArea;
            Location = new Point(workingArea.X + x, workingArea.Y + y);
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.Show"]/*' />
        /// <devdoc>
        ///     Makes the control display by setting the visible property to true
        /// </devdoc>
        public void Show(IWin32Window owner) {
            if (owner == this) {
                throw new InvalidOperationException(string.Format(SR.OwnsSelfOrOwner,
                                                  "Show"));
            }
            else if (Visible) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnVisible,
                                                  "Show"));
            }
            else if (!Enabled) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnDisabled,
                                                  "Show"));
            }
            else if (!TopLevel) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnNonTopLevel,
                                                  "Show"));
            }
            else if (!SystemInformation.UserInteractive) {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }
            else if ( (owner != null) && ((int)UnsafeNativeMethods.GetWindowLong(new HandleRef(owner, Control.GetSafeHandle(owner)), NativeMethods.GWL_EXSTYLE)
                     & NativeMethods.WS_EX_TOPMOST) == 0 ) {   // It's not the top-most window
                if (owner is Control) {
                    owner = ((Control)owner).TopLevelControlInternal;
                }
            }
            IntPtr hWndActive = UnsafeNativeMethods.GetActiveWindow();
            IntPtr hWndOwner = owner == null ? hWndActive : Control.GetSafeHandle(owner);
            IntPtr hWndOldOwner = IntPtr.Zero;
            Properties.SetObject(PropDialogOwner, owner);
            Form oldOwner = OwnerInternal;
            if (owner is Form && owner != oldOwner) {
                Owner = (Form)owner;
            }
            if (hWndOwner != IntPtr.Zero && hWndOwner != Handle) {
                // Catch the case of a window trying to own its owner
                if (UnsafeNativeMethods.GetWindowLong(new HandleRef(owner, hWndOwner), NativeMethods.GWL_HWNDPARENT) == Handle) {
                    throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner,
                                                      "show"), "owner");
                }

                // Set the new owner.
                hWndOldOwner = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT);
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT, new HandleRef(owner, hWndOwner));

            }
            Visible = true;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShowDialog"]/*' />
        /// <devdoc>
        ///    <para>Displays this form as a modal dialog box with no owner window.</para>
        /// </devdoc>
        public DialogResult ShowDialog() {
            return ShowDialog(null);
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShowDialog1"]/*' />
        /// <devdoc>
        ///    <para>Shows this form as a modal dialog with the specified owner.</para>
        /// </devdoc>
        public DialogResult ShowDialog(IWin32Window owner) {
            if (owner == this) {
                throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner,
                                                  "showDialog"), "owner");
            }
            else if (Visible) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnVisible,
                                                  "showDialog"));
            }
            else if (!Enabled) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnDisabled,
                                                  "showDialog"));
            }
            else if (!TopLevel) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnNonTopLevel,
                                                  "showDialog"));
            }
            else if (Modal) {
                throw new InvalidOperationException(string.Format(SR.ShowDialogOnModal,
                                                  "showDialog"));
            }
            else if (!SystemInformation.UserInteractive) {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }
            else if ( (owner != null) && ((int)UnsafeNativeMethods.GetWindowLong(new HandleRef(owner, Control.GetSafeHandle(owner)), NativeMethods.GWL_EXSTYLE)
                     & NativeMethods.WS_EX_TOPMOST) == 0 ) {   // It's not the top-most window
                if (owner is Control) {
                    owner = ((Control)owner).TopLevelControlInternal;
                }
            }

            this.CalledOnLoad = false;
            this.CalledMakeVisible = false;

            // for modal dialogs make sure we reset close reason.
            this.CloseReason = CloseReason.None;

            IntPtr hWndCapture = UnsafeNativeMethods.GetCapture();
            if (hWndCapture != IntPtr.Zero) {
                UnsafeNativeMethods.SendMessage(new HandleRef(null, hWndCapture), NativeMethods.WM_CANCELMODE, IntPtr.Zero, IntPtr.Zero);
                SafeNativeMethods.ReleaseCapture();
            }
            IntPtr hWndActive = UnsafeNativeMethods.GetActiveWindow();
            IntPtr hWndOwner = owner == null ? hWndActive : Control.GetSafeHandle(owner);
            IntPtr hWndOldOwner = IntPtr.Zero;
            Properties.SetObject(PropDialogOwner, owner);

            Form oldOwner = OwnerInternal;

            if (owner is Form && owner != oldOwner) {
                Owner = (Form)owner;
            }

            try {
                SetState(STATE_MODAL, true);

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

                if (hWndOwner != IntPtr.Zero && hWndOwner != Handle) {
                    // Catch the case of a window trying to own its owner
                    if (UnsafeNativeMethods.GetWindowLong(new HandleRef(owner, hWndOwner), NativeMethods.GWL_HWNDPARENT) == Handle) {
                        throw new ArgumentException(string.Format(SR.OwnsSelfOrOwner,
                                                          "showDialog"), "owner");
                    }

                    // Set the new owner.
                    hWndOldOwner = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT);
                    UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT, new HandleRef(owner, hWndOwner));
                }

                try {
                    // If the DialogResult was already set, then there's
                    // no need to actually display the dialog.
                    //
                    if (dialogResult == DialogResult.None)
                    {
                        // Application.RunDialog sets this dialog to be visible.
                        Application.RunDialog(this);
                    }
                }
                finally {
                    // Call SetActiveWindow before setting Visible = false.
                    // 

                    if (!UnsafeNativeMethods.IsWindow(new HandleRef(null, hWndActive))) hWndActive = hWndOwner;
                    if (UnsafeNativeMethods.IsWindow(new HandleRef(null, hWndActive)) && SafeNativeMethods.IsWindowVisible(new HandleRef(null, hWndActive))) {
                        UnsafeNativeMethods.SetActiveWindow(new HandleRef(null, hWndActive));
                    }
                    else if (UnsafeNativeMethods.IsWindow(new HandleRef(null, hWndOwner)) && SafeNativeMethods.IsWindowVisible(new HandleRef(null, hWndOwner))){
                        UnsafeNativeMethods.SetActiveWindow(new HandleRef(null, hWndOwner));
                    }

                    SetVisibleCore(false);
                    if (IsHandleCreated) {

                        // If this is a dialog opened from an MDI Container, then invalidate
                        // so that child windows will be properly updated.
                        if (this.OwnerInternal != null &&
                            this.OwnerInternal.IsMdiContainer) {
                            this.OwnerInternal.Invalidate(true);
                            this.OwnerInternal.Update();
                        }

                        // Everett/RTM used to wrap this in an assert for AWP.
                        DestroyHandle();
                    }
                    SetState(STATE_MODAL, false);
                }
            }
            finally {
                Owner = oldOwner;
                Properties.SetObject(PropDialogOwner, null);
            }
            return DialogResult;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShouldSerializeAutoScaleBaseSize"]/*' />
        /// <devdoc>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.Form.AutoScaleBaseSize'/> property should be
        ///    persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeAutoScaleBaseSize() {
            return formState[FormStateAutoScaling] != 0;
        }

        private bool ShouldSerializeClientSize() {
            return true;
        }

        /// <devdoc>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.Form.Icon'/> property should be persisted.</para>
        /// </devdoc>
        private bool ShouldSerializeIcon() {
            return formState[FormStateIconSet] == 1;
        }

        /// <devdoc>
        ///     Determines if the Location property needs to be persisted.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeLocation() {
            return Left != 0 || Top != 0;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShouldSerializeSize"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.Form.Size'/> property should be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal override bool ShouldSerializeSize() {
            return false;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ShouldSerializeTransparencyKey"]/*' />
        /// <devdoc>
        /// <para>Indicates whether the <see cref='System.Windows.Forms.Form.TransparencyKey'/> property should be
        ///    persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeTransparencyKey() {
            return !TransparencyKey.Equals(Color.Empty);
        }

        /// <devdoc>
        ///     This is called when we are about to become minimized.  Laying out
        ///     while minimized can be a problem because the physical dimensions
        ///     of the window are very small.  So, we simply suspend.
        /// </devdoc>
        private void SuspendLayoutForMinimize() {
            // If we're not currently minimized, suspend our layout because we are
            // about to become minimized
            if (formState[FormStateWindowState] != (int)FormWindowState.Minimized) {
                SuspendLayout();
            }
        }

        /// <devdoc>
        ///     Increments updateMenuHandleSuspendCount.
        /// </devdoc>
        private void SuspendUpdateMenuHandles() {
            int suspendCount = formStateEx[FormStateExUpdateMenuHandlesSuspendCount];
            formStateEx[FormStateExUpdateMenuHandlesSuspendCount] = ++suspendCount;
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", Text: " + Text;
        }

        /// <devdoc>
        ///     Updates the autoscalebasesize based on the current font.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateAutoScaleBaseSize() {
            autoScaleBaseSize = Size.Empty;
        }

        private void UpdateRenderSizeGrip() {
            int current = formState[FormStateRenderSizeGrip];
            switch (FormBorderStyle) {
                case FormBorderStyle.None:
                case FormBorderStyle.FixedSingle:
                case FormBorderStyle.Fixed3D:
                case FormBorderStyle.FixedDialog:
                case FormBorderStyle.FixedToolWindow:
                    formState[FormStateRenderSizeGrip] = 0;
                    break;
                case FormBorderStyle.Sizable:
                case FormBorderStyle.SizableToolWindow:
                    switch (SizeGripStyle) {
                        case SizeGripStyle.Show:
                            formState[FormStateRenderSizeGrip] = 1;
                            break;
                        case SizeGripStyle.Hide:
                            formState[FormStateRenderSizeGrip] = 0;
                            break;
                        case SizeGripStyle.Auto:
                            if (GetState(STATE_MODAL)) {
                                formState[FormStateRenderSizeGrip] = 1;
                            }
                            else {
                                formState[FormStateRenderSizeGrip] = 0;
                            }
                            break;
                    }
                    break;
            }

            if (formState[FormStateRenderSizeGrip] != current) {
                Invalidate();
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.UpdateDefaultButton"]/*' />
        /// <devdoc>
        ///     Updates the default button based on current selection, and the
        ///     acceptButton property.
        /// </devdoc>
        /// <internalonly/>
        protected override void UpdateDefaultButton() {
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
                SetDefaultButton((IButtonControl) cc.ActiveControl);
            }
            else
            {
                SetDefaultButton(AcceptButton);
            }
        }

        /// <devdoc>
        ///     Updates the underlying hWnd with the correct parent/owner of the form.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateHandleWithOwner() {
            if (IsHandleCreated && TopLevel) {
                HandleRef ownerHwnd = NativeMethods.NullHandleRef;

                Form owner = (Form)Properties.GetObject(PropOwner);

                if (owner != null) {
                    ownerHwnd = new HandleRef(owner, owner.Handle);
                }
                else {
                    if (!ShowInTaskbar) {
                        ownerHwnd = TaskbarOwner;
                    }
                }

                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_HWNDPARENT, ownerHwnd);
            }
        }

        /// <devdoc>
        ///     Updates the layered window attributes if the control
        ///     is in layered mode.
        /// </devdoc>
        private void UpdateLayered() {
            if ((formState[FormStateLayered] != 0) && IsHandleCreated && TopLevel && OSFeature.Feature.IsPresent(OSFeature.LayeredWindows)) {
                bool result;

                Color transparencyKey = TransparencyKey;

                if (transparencyKey.IsEmpty) {

                    result = UnsafeNativeMethods.SetLayeredWindowAttributes(new HandleRef(this, Handle), 0, OpacityAsByte, NativeMethods.LWA_ALPHA);
                }
                else if (OpacityAsByte == 255) {
                    // Windows doesn't do so well setting colorkey and alpha, so avoid it if we can
                    result = UnsafeNativeMethods.SetLayeredWindowAttributes(new HandleRef(this, Handle), ColorTranslator.ToWin32(transparencyKey), 0, NativeMethods.LWA_COLORKEY);
                }
                else {
                    result = UnsafeNativeMethods.SetLayeredWindowAttributes(new HandleRef(this, Handle), ColorTranslator.ToWin32(transparencyKey),
                                                                OpacityAsByte, NativeMethods.LWA_ALPHA | NativeMethods.LWA_COLORKEY);
                }

                if (!result) {
                    throw new Win32Exception();
                }
            }
        }

        /// <internalonly/>
        private void UpdateMenuHandles() {
            Form form;

            // Forget the current menu.
            if (Properties.GetObject(PropCurMenu) != null) {
                Properties.SetObject(PropCurMenu, null);
            }

            if (IsHandleCreated) {
                if (!TopLevel) {
                    UpdateMenuHandles(null, true);
                }
                else {
                    form = ActiveMdiChildInternal;
                    if (form != null) {
                        UpdateMenuHandles(form.MergedMenuPrivate, true);
                    }
                    else {
                        UpdateMenuHandles(Menu, true);
                    }
                }
            }
        }

        private void UpdateMenuHandles(MainMenu menu, bool forceRedraw) {
            Debug.Assert(IsHandleCreated, "shouldn't call when handle == 0");

            int suspendCount = formStateEx[FormStateExUpdateMenuHandlesSuspendCount];
            if (suspendCount > 0 && menu != null) {
                formStateEx[FormStateExUpdateMenuHandlesDeferred] = 1;
                return;
            }

            MainMenu curMenu = menu;
            if (curMenu != null) {
                curMenu.form = this;
            }

            if (curMenu != null || Properties.ContainsObject(PropCurMenu)) {
                Properties.SetObject(PropCurMenu, curMenu);
            }

            if (ctlClient == null || !ctlClient.IsHandleCreated) {
                if (menu != null) {
                    UnsafeNativeMethods.SetMenu(new HandleRef(this, Handle), new HandleRef(menu, menu.Handle));
                }
                else {
                    UnsafeNativeMethods.SetMenu(new HandleRef(this, Handle), NativeMethods.NullHandleRef);
                }
            }
            else {
                Debug.Assert( IsMdiContainer, "Not an MDI container!" );
                // when both MainMenuStrip and Menu are set, we honor the win32 menu over 
                // the MainMenuStrip as the place to store the system menu controls for the maximized MDI child.

                MenuStrip mainMenuStrip = MainMenuStrip;
                if( mainMenuStrip == null || menu!=null){  // We are dealing with a Win32 Menu; MenuStrip doesn't have control buttons.

                    // We have a MainMenu and we're going to use it
                    
                    // We need to set the "dummy" menu even when a menu is being removed
                    // (set to null) so that duplicate control buttons are not placed on the menu bar when
                    // an ole menu is being removed.
                    // Make MDI forget the mdi item position.
                    MainMenu dummyMenu = (MainMenu)Properties.GetObject(PropDummyMenu);

                    if (dummyMenu == null) {
                        dummyMenu = new MainMenu();
                        dummyMenu.ownerForm = this;
                        Properties.SetObject(PropDummyMenu, dummyMenu);
                    }
                    UnsafeNativeMethods.SendMessage(new HandleRef(ctlClient, ctlClient.Handle), NativeMethods.WM_MDISETMENU, dummyMenu.Handle, IntPtr.Zero);

                    if (menu != null) {

                        // Microsoft, 5/2/1998 - don't use Win32 native Mdi lists...
                        //
                        UnsafeNativeMethods.SendMessage(new HandleRef(ctlClient, ctlClient.Handle), NativeMethods.WM_MDISETMENU, menu.Handle, IntPtr.Zero);
                    }
                }
                
                // (New fix: Only destroy Win32 Menu if using a MenuStrip)
                if( menu == null && mainMenuStrip != null ){ // If MainMenuStrip, we need to remove any Win32 Menu to make room for it.
                    IntPtr hMenu =  UnsafeNativeMethods.GetMenu(new HandleRef(this, this.Handle));
                    if (hMenu != IntPtr.Zero) {

                        // We had a MainMenu and now we're switching over to MainMenuStrip

                        // Remove the current menu.
                        UnsafeNativeMethods.SetMenu(new HandleRef(this, this.Handle), NativeMethods.NullHandleRef);

                        // because we have messed with the child's system menu by shoving in our own dummy menu, 
                        // once we clear the main menu we're in trouble - this eats the close, minimize, maximize gadgets
                        // of the child form. (See WM_MDISETMENU in MSDN)
                        Form activeMdiChild = this.ActiveMdiChildInternal;
                        if (activeMdiChild != null && activeMdiChild.WindowState == FormWindowState.Maximized) {
                            activeMdiChild.RecreateHandle();
                        }
                        
                        // Since we're removing a menu but we possibly had a menu previously,
                        // we need to clear the cached size so that new size calculations will be performed correctly.
                        CommonProperties.xClearPreferredSizeCache(this);
                    }
                }
            }
            if (forceRedraw) {
                SafeNativeMethods.DrawMenuBar(new HandleRef(this, Handle));
            }
            formStateEx[FormStateExUpdateMenuHandlesDeferred] = 0;
        }

        // Call this function instead of UpdateStyles() when the form's client-size must
        // be preserved e.g. when changing the border style.
        //
        internal void UpdateFormStyles() {
            Size previousClientSize = ClientSize;
            base.UpdateStyles();
            if (!ClientSize.Equals(previousClientSize)) {
                ClientSize = previousClientSize;
            }
        }

        private static Type FindClosestStockType(Type type) {
            Type[] stockTypes = new Type[] { typeof (MenuStrip) }; // as opposed to what we had before...
            // simply add other types here from most specific to most generic if we want to merge other types of toolstrips...
            foreach(Type t in stockTypes) {
                if(t.IsAssignableFrom(type)) {
                    return t;
                }
            }
            return null;
        }

        ///<devdoc> ToolStrip MDI Merging support </devdoc>
        private void UpdateToolStrip() {
            //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "\r\n============");
            //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MergeDebug.TraceVerbose, "ToolStripMerging: starting merge operation");

            // try to merge each one of the MDI Child toolstrip with the first toolstrip
            // in the parent form that has the same type NOTE: THESE LISTS ARE ORDERED (See ToolstripManager)
            ToolStrip thisToolstrip = MainMenuStrip;
            ArrayList childrenToolStrips = ToolStripManager.FindMergeableToolStrips(ActiveMdiChildInternal);

            //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MergeDebug.TraceVerbose, "ToolStripMerging: found "+ thisToolStrips.Count +" mergeable toolstrip in this");
            //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MergeDebug.TraceVerbose, "ToolStripMerging: found "+childrenToolStrips.Count+" mergeable toolstrip in children");

            // revert any previous merge
            if(thisToolstrip != null) {
                //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MergeDebug.TraceVerbose, "ToolStripMerging: reverting merge in " + destinationToolStrip.Name);
                ToolStripManager.RevertMerge(thisToolstrip);
            }

            // if someone has a MdiWindowListItem specified we should merge in the
            // names of all the MDI child forms.
            UpdateMdiWindowListStrip(); 

            if(ActiveMdiChildInternal != null) {

                // do the new merging
                foreach(ToolStrip sourceToolStrip in childrenToolStrips) {
                    Type closestMatchingSourceType = FindClosestStockType(sourceToolStrip.GetType());
                    if(thisToolstrip != null) {
                        Type closestMatchingTargetType = FindClosestStockType(thisToolstrip.GetType());
                        if (closestMatchingTargetType != null && closestMatchingSourceType != null &&
                            closestMatchingSourceType == closestMatchingTargetType &&
                            thisToolstrip.GetType().IsAssignableFrom(sourceToolStrip.GetType())) {
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

        private void UpdateMdiControlStrip(bool maximized) {

            if (formStateEx[FormStateExInUpdateMdiControlStrip] != 0) {
                //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiControlStrip: Detected re-entrant call to UpdateMdiControlStrip, returning.");
                return;
            }

            // we dont want to be redundantly called as we could merge in two control menus.
            formStateEx[FormStateExInUpdateMdiControlStrip] = 1;

            try {
                MdiControlStrip mdiControlStrip = this.MdiControlStrip;

                if (MdiControlStrip != null) {
                    if (mdiControlStrip.MergedMenu != null) {
#if DEBUG
                        //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiControlStrip: Calling RevertMerge on MDIControl strip.");
                        int numWindowListItems = 0;
                        if (MdiWindowListStrip != null && MdiWindowListStrip.MergedMenu != null && MdiWindowListStrip.MergedMenu.MdiWindowListItem != null) {
                            numWindowListItems = MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count;
                        }
#endif

                        ToolStripManager.RevertMergeInternal(mdiControlStrip.MergedMenu,mdiControlStrip,/*revertMDIStuff*/true);

#if DEBUG
                        // double check that RevertMerge doesnt accidentally revert more than it should.
                        if (MdiWindowListStrip != null && MdiWindowListStrip.MergedMenu != null && MdiWindowListStrip.MergedMenu.MdiWindowListItem != null) {
                            Debug.Assert(numWindowListItems == MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count, "Calling RevertMerge modified the mdiwindowlistitem");
                        }
#endif
                    }
                    mdiControlStrip.MergedMenu = null;
                    mdiControlStrip.Dispose();
                    MdiControlStrip = null;
                }

                if (ActiveMdiChildInternal != null && maximized) {
                    if (ActiveMdiChildInternal.ControlBox) {
                        Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiControlStrip: Detected ControlBox on ActiveMDI child, adding in MDIControlStrip.");
                        Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose && this.Menu != null, "UpdateMdiControlStrip: Bailing as we detect there's already an HMenu to do this for us.");

                        // determine if we need to add control gadgets into the MenuStrip
                        if (this.Menu == null) {
                            // double check GetMenu incase someone is using interop
                            IntPtr hMenu =  UnsafeNativeMethods.GetMenu(new HandleRef(this, this.Handle));
                            if (hMenu == IntPtr.Zero) {
                                MenuStrip sourceMenuStrip = ToolStripManager.GetMainMenuStrip(this);
                                if (sourceMenuStrip != null) {
                                    this.MdiControlStrip = new MdiControlStrip(ActiveMdiChildInternal);
                                    Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiControlStrip: built up an MDI control strip for " + ActiveMdiChildInternal.Text + " with " + MdiControlStrip.Items.Count.ToString(CultureInfo.InvariantCulture) + " items.");
                                    Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiControlStrip: merging MDI control strip into source menustrip - items before: " + sourceMenuStrip.Items.Count.ToString(CultureInfo.InvariantCulture));
                                    ToolStripManager.Merge(this.MdiControlStrip, sourceMenuStrip);
                                    Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiControlStrip: merging MDI control strip into source menustrip - items after: " + sourceMenuStrip.Items.Count.ToString(CultureInfo.InvariantCulture));
                                    this.MdiControlStrip.MergedMenu = sourceMenuStrip;
                                }
                            }
                        }
                    }
                }
            }
            finally {
                formStateEx[FormStateExInUpdateMdiControlStrip] = 0;
            }
        }

        internal void UpdateMdiWindowListStrip() {
            if (IsMdiContainer) {
                if (MdiWindowListStrip != null && MdiWindowListStrip.MergedMenu != null) {
                    //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiWindowListStrip: Calling RevertMerge on MDIWindowList strip.");
                    //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose && MdiWindowListStrip.MergedMenu.MdiWindowListItem != null, "UpdateMdiWindowListStrip: MdiWindowListItem  dropdown item count before: " + MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count.ToString());
                     ToolStripManager.RevertMergeInternal(MdiWindowListStrip.MergedMenu,MdiWindowListStrip,/*revertMdiStuff*/true);
                    //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose && MdiWindowListStrip.MergedMenu.MdiWindowListItem != null, "UpdateMdiWindowListStrip: MdiWindowListItem  dropdown item count after:  " + MdiWindowListStrip.MergedMenu.MdiWindowListItem.DropDownItems.Count.ToString());
                }
    
                MenuStrip sourceMenuStrip = ToolStripManager.GetMainMenuStrip(this);
                if (sourceMenuStrip != null && sourceMenuStrip.MdiWindowListItem != null) {
                    if (MdiWindowListStrip == null) {
                       MdiWindowListStrip = new MdiWindowListStrip();
                    }
                    int nSubItems = sourceMenuStrip.MdiWindowListItem.DropDownItems.Count;
                    bool shouldIncludeSeparator = (nSubItems > 0 && 
                        !(sourceMenuStrip.MdiWindowListItem.DropDownItems[nSubItems-1] is ToolStripSeparator));
                    //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiWindowListStrip: Calling populate items.");
                    MdiWindowListStrip.PopulateItems(this, sourceMenuStrip.MdiWindowListItem, shouldIncludeSeparator);
                    //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiWindowListStrip: mdiwindowlist dd item count before: " + sourceMenuStrip.MdiWindowListItem.DropDownItems.Count.ToString());
                    ToolStripManager.Merge(MdiWindowListStrip, sourceMenuStrip);
                    //MERGEDEBUG Debug.WriteLineIf(ToolStrip.MDIMergeDebug.TraceVerbose, "UpdateMdiWindowListStrip: mdiwindowlist dd item count after:  " + sourceMenuStrip.MdiWindowListItem.DropDownItems.Count.ToString());
                    MdiWindowListStrip.MergedMenu = sourceMenuStrip;
                }

            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnResizeBegin"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Form.ResizeBegin'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResizeBegin(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                EventHandler handler = (EventHandler)Events[EVENT_RESIZEBEGIN];
                if (handler != null) handler(this, e);
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnResizeEnd"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Form.ResizeEnd'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResizeEnd(EventArgs e)
        {
            if (CanRaiseEvents)
            {
                EventHandler handler = (EventHandler)Events[EVENT_RESIZEEND];
                if (handler != null) handler(this, e);
            }
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnStyleChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnStyleChanged(EventArgs e) {
            base.OnStyleChanged(e);
            AdjustSystemMenu();
        }

        /// <devdoc>
        ///     Updates the window icon.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateWindowIcon(bool redrawFrame) {
            if (IsHandleCreated) {
                Icon icon;

                // Preserve Win32 behavior by keeping the icon we set NULL if
                // the user hasn't specified an icon and we are a dialog frame.
                //
                if ((FormBorderStyle == FormBorderStyle.FixedDialog && formState[FormStateIconSet] == 0 && !IsRestrictedWindow) || !ShowIcon) {
                    icon = null;
                }
                else {
                    icon = Icon;
                }

                if (icon != null) {
                    if (smallIcon == null) {
                        try {
                            smallIcon = new Icon(icon, SystemInformation.SmallIconSize);
                        }
                        catch {
                        }
                    }

                    if (smallIcon != null) {
                        SendMessage(NativeMethods.WM_SETICON,NativeMethods.ICON_SMALL,smallIcon.Handle);
                    }
                    SendMessage(NativeMethods.WM_SETICON,NativeMethods.ICON_BIG,icon.Handle);
                }
                else {

                    SendMessage(NativeMethods.WM_SETICON,NativeMethods.ICON_SMALL,0);
                    SendMessage(NativeMethods.WM_SETICON,NativeMethods.ICON_BIG,0);
                }

                if (redrawFrame) {
                    SafeNativeMethods.RedrawWindow(new HandleRef(this, Handle), null, NativeMethods.NullHandleRef, NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_FRAME);
                }
            }
        }

        /// <devdoc>
        ///     Updated the window state from the handle, if created.
        /// </devdoc>
        /// <internalonly/>
        //
        // This function is called from all over the place, including my personal favorite,
        // WM_ERASEBKGRND.  Seems that's one of the first messages we get when a user clicks the min/max
        // button, even before WM_WINDOWPOSCHANGED.
        private void UpdateWindowState() {
            if (IsHandleCreated) {
                FormWindowState oldState = WindowState;
                NativeMethods.WINDOWPLACEMENT wp = new NativeMethods.WINDOWPLACEMENT();
                wp.length = Marshal.SizeOf(typeof(NativeMethods.WINDOWPLACEMENT));
                UnsafeNativeMethods.GetWindowPlacement(new HandleRef(this, Handle), ref wp);

                switch (wp.showCmd) {
                    case NativeMethods.SW_NORMAL:
                    case NativeMethods.SW_RESTORE:
                    case NativeMethods.SW_SHOW:
                    case NativeMethods.SW_SHOWNA:
                    case NativeMethods.SW_SHOWNOACTIVATE:
                        if (formState[FormStateWindowState] != (int)FormWindowState.Normal) {
                            formState[FormStateWindowState] = (int)FormWindowState.Normal;
                        }
                        break;
                    case NativeMethods.SW_SHOWMAXIMIZED:
                        if (formState[FormStateMdiChildMax] == 0) {
                            formState[FormStateWindowState] = (int)FormWindowState.Maximized;
                        }
                        break;
                    case NativeMethods.SW_SHOWMINIMIZED:
                    case NativeMethods.SW_MINIMIZE:
                    case NativeMethods.SW_SHOWMINNOACTIVE:
                        if (formState[FormStateMdiChildMax] == 0) {
                            formState[FormStateWindowState] = (int)FormWindowState.Minimized;
                        }
                        break;
                    case NativeMethods.SW_HIDE:
                    default:
                        break;
                }

                // If we used to be normal and we just became minimized or maximized,
                // stash off our current bounds so we can properly restore.
                //
                if (oldState == FormWindowState.Normal && WindowState != FormWindowState.Normal) {

                    if (WindowState == FormWindowState.Minimized) {
                        SuspendLayoutForMinimize();
                    }

                    restoredWindowBounds.Size = ClientSize;
                    formStateEx[FormStateExWindowBoundsWidthIsClientSize] = 1;
                    formStateEx[FormStateExWindowBoundsHeightIsClientSize] = 1;
                    restoredWindowBoundsSpecified = BoundsSpecified.Size;
                    restoredWindowBounds.Location = Location;
                    restoredWindowBoundsSpecified |= BoundsSpecified.Location;

                    //stash off restoreBounds As well...
                    restoreBounds.Size = Size;
                    restoreBounds.Location = Location;
                }

                // If we just became normal or maximized resume
                if (oldState == FormWindowState.Minimized && WindowState != FormWindowState.Minimized) {
                    ResumeLayoutFromMinimize();
                }

                switch (WindowState) {
                    case FormWindowState.Normal:
                        SetState(STATE_SIZELOCKEDBYOS, false);
                        break;
                    case FormWindowState.Maximized:
                    case FormWindowState.Minimized:
                        SetState(STATE_SIZELOCKEDBYOS, true);
                        break;
                }

                if (oldState != WindowState) {
                    AdjustSystemMenu();
                }
            }
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ValidateChildren"]/*' />
        /// <devdoc>
        ///     Validates all selectable child controls in the container, including descendants. This is
        ///     equivalent to calling ValidateChildren(ValidationConstraints.Selectable). See <see cref='ValidationConstraints.Selectable'/>
        ///     for details of exactly which child controls will be validated.
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren() {
            return base.ValidateChildren();
        }

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ValidateChildren1"]/*' />
        /// <devdoc>
        ///     Validates all the child controls in the container. Exactly which controls are
        ///     validated and which controls are skipped is determined by <paramref name="flags"/>.
        /// </devdoc>
        [Browsable(true), EditorBrowsable(EditorBrowsableState.Always)]
        public override bool ValidateChildren(ValidationConstraints validationConstraints) {
            return base.ValidateChildren(validationConstraints);
        }

        /// <devdoc>
        ///     WM_ACTIVATE handler
        /// </devdoc>
        /// <internalonly/>
        private void WmActivate(ref Message m) {
            Application.FormActivated(this.Modal, true); // inform MsoComponentManager we're active
            Active = NativeMethods.Util.LOWORD(m.WParam) != NativeMethods.WA_INACTIVE;
            Application.FormActivated(this.Modal, Active); // inform MsoComponentManager we're active
        }

        /// <devdoc>
        ///     WM_ENTERSIZEMOVE handler, so that user can hook up OnResizeBegin event.
        /// </devdoc>
        /// <internalonly/>
        private void WmEnterSizeMove(ref Message m) {
            formStateEx[FormStateExInModalSizingLoop] = 1;
            OnResizeBegin(EventArgs.Empty);
        }

        /// <devdoc>
        ///     WM_EXITSIZEMOVE handler, so that user can hook up OnResizeEnd event.
        /// </devdoc>
        /// <internalonly/>
        private void WmExitSizeMove(ref Message m) {
            formStateEx[FormStateExInModalSizingLoop] = 0;
            OnResizeEnd(EventArgs.Empty);
        }

        /// <devdoc>
        ///     WM_CREATE handler
        /// </devdoc>
        /// <internalonly/>
        private void WmCreate(ref Message m) {
            base.WndProc(ref m);
            NativeMethods.STARTUPINFO_I si = new NativeMethods.STARTUPINFO_I();
            UnsafeNativeMethods.GetStartupInfo(si);

            // If we've been created from explorer, it may
            // force us to show up normal.  Force our current window state to
            // the specified state, unless it's _specified_ max or min
            if (TopLevel && (si.dwFlags & NativeMethods.STARTF_USESHOWWINDOW) != 0) {
                switch (si.wShowWindow) {
                    case NativeMethods.SW_MAXIMIZE:
                        WindowState = FormWindowState.Maximized;
                        break;
                    case NativeMethods.SW_MINIMIZE:
                        WindowState = FormWindowState.Minimized;
                        break;
                }
            }
        }

        /// <devdoc>
        ///     WM_CLOSE, WM_QUERYENDSESSION, and WM_ENDSESSION handler
        /// </devdoc>
        /// <internalonly/>
        private void WmClose(ref Message m) {
            FormClosingEventArgs e = new FormClosingEventArgs(CloseReason, false);

            // Pass 1 (WM_CLOSE & WM_QUERYENDSESSION)... Closing
            //
            if (m.Msg != NativeMethods.WM_ENDSESSION) {
                if (Modal) {
                    if (dialogResult == DialogResult.None) {
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
                else {
                    e.Cancel = !Validate(true);

                    // Call OnClosing/OnFormClosing on all MDI children
                    if (IsMdiContainer) {
                        FormClosingEventArgs fe = new FormClosingEventArgs(CloseReason.MdiFormClosing, e.Cancel);
                        foreach(Form mdiChild in MdiChildren) {
                            if (mdiChild.IsHandleCreated) {                                
                                mdiChild.OnClosing(fe);
                                mdiChild.OnFormClosing(fe);
                                if (fe.Cancel) {
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
                    Form[] ownedForms = this.OwnedForms;
                    int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                    for (int i = ownedFormsCount-1 ; i >= 0; i--) {
                        FormClosingEventArgs cfe = new FormClosingEventArgs(CloseReason.FormOwnerClosing, e.Cancel);
                        if (ownedForms[i] != null) {
                            //Call OnFormClosing on the child forms.
                            ownedForms[i].OnFormClosing(cfe);
                            if (cfe.Cancel) {
                                // Set the cancel flag for the Owner form
                                e.Cancel = true;
                                break;
                            }
                        }
                    }

                    OnClosing(e);
                    OnFormClosing(e);
                }

                if (m.Msg == NativeMethods.WM_QUERYENDSESSION) {
                    m.Result = (IntPtr)(e.Cancel ? 0 : 1);
                }
                else if (e.Cancel && (MdiParent != null)) {
                    // This is the case of an MDI child close event being canceled by the user.
                    CloseReason = CloseReason.None;
                }

                if (Modal) {
                    return;
                }
            }
            else {
                e.Cancel = m.WParam == IntPtr.Zero;
            }

            // Pass 2 (WM_CLOSE & WM_ENDSESSION)... Fire closed
            // event on all mdi children and ourselves
            //
            if (m.Msg != NativeMethods.WM_QUERYENDSESSION) {
                FormClosedEventArgs fc;
                if (!e.Cancel) {
                    IsClosing = true;

                    if (IsMdiContainer) {
                        fc = new FormClosedEventArgs(CloseReason.MdiFormClosing);
                        foreach(Form mdiChild in MdiChildren) {
                            if (mdiChild.IsHandleCreated) {
                                mdiChild.IsTopMdiWindowClosing = IsClosing;
                                mdiChild.OnClosed(fc);
                                mdiChild.OnFormClosed(fc);
                            }
                        }
                    }

                    // Call OnClosed/OnFormClosed on all the forms that current form owns.
                    Form[] ownedForms = this.OwnedForms;
                    int ownedFormsCount = Properties.GetInteger(PropOwnedFormsCount);
                    for (int i = ownedFormsCount-1 ; i >= 0; i--) {
                        fc = new FormClosedEventArgs(CloseReason.FormOwnerClosing);
                        if (ownedForms[i] != null) {
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

        /// <devdoc>
        ///     WM_ENTERMENULOOP handler
        /// </devdoc>
        /// <internalonly/>
        private void WmEnterMenuLoop(ref Message m) {
            OnMenuStart(EventArgs.Empty);
            base.WndProc(ref m);
        }

        /// <devdoc>
        ///     Handles the WM_ERASEBKGND message
        /// </devdoc>
        /// <internalonly/>
        private void WmEraseBkgnd(ref Message m) {
            UpdateWindowState();
            base.WndProc(ref m);
        }

        /// <devdoc>
        ///     WM_EXITMENULOOP handler
        /// </devdoc>
        /// <internalonly/>
        private void WmExitMenuLoop(ref Message m) {
            OnMenuComplete(EventArgs.Empty);
            base.WndProc(ref m);
        }
    
        /// <devdoc>
        ///     WM_GETMINMAXINFO handler
        /// </devdoc>
        /// <internalonly/>
        private void WmGetMinMaxInfo(ref Message m) {

            // Form should gracefully stop at the minimum preferred size.
            // When we're set to AutoSize true, we should take a look at minAutoSize - which is snapped in onlayout.
            // as the form contracts, we should not let it size past here as we're just going to readjust the size
            // back to it later.
            Size minTrack = (AutoSize && formStateEx[FormStateExInModalSizingLoop] == 1) ? LayoutUtils.UnionSizes(minAutoSize, MinimumSize) : MinimumSize;
            
            Size maxTrack = MaximumSize;
            Rectangle maximizedBounds = MaximizedBounds;

            if (!minTrack.IsEmpty
                || !maxTrack.IsEmpty
                || !maximizedBounds.IsEmpty
                || IsRestrictedWindow) {

                WmGetMinMaxInfoHelper(ref m, minTrack, maxTrack, maximizedBounds);
            }
            if (IsMdiChild) {
                base.WndProc(ref m);
                return;
            }
        }

        // PERFTRACK : Microsoft, 2/22/2000 - Refer to MINMAXINFO in a separate method
        //           : to avoid loading the class in the common case.
        //
        private void WmGetMinMaxInfoHelper(ref Message m, Size minTrack, Size maxTrack, Rectangle maximizedBounds) {

            NativeMethods.MINMAXINFO mmi = (NativeMethods.MINMAXINFO)m.GetLParam(typeof(NativeMethods.MINMAXINFO));

            if (!minTrack.IsEmpty) {

                mmi.ptMinTrackSize.x = minTrack.Width;
                mmi.ptMinTrackSize.y = minTrack.Height;

                // When the MinTrackSize is set to a value larger than the screen
                // size but the MaxTrackSize is not set to a value equal to or greater than the
                // MinTrackSize and the user attempts to "grab" a resizing handle, Windows makes
                // the window move a distance equal to either the width when attempting to resize
                // horizontally or the height of the window when attempting to resize vertically.
                // So, the workaround to prevent this problem is to set the MaxTrackSize to something
                // whenever the MinTrackSize is set to a value larger than the respective dimension
                // of the virtual screen.

                if (maxTrack.IsEmpty) {

                    // Only set the max track size dimensions if the min track size dimensions
                    // are larger than the VirtualScreen dimensions.
                    Size virtualScreen = SystemInformation.VirtualScreen.Size;
                    if (minTrack.Height > virtualScreen.Height) {
                        mmi.ptMaxTrackSize.y = int.MaxValue;
                    }
                    if (minTrack.Width > virtualScreen.Width) {
                        mmi.ptMaxTrackSize.x = int.MaxValue;
                    }
                }
            }
            if (!maxTrack.IsEmpty) {
                // Is the specified MaxTrackSize smaller than the smallest allowable Window size?
                Size minTrackWindowSize = SystemInformation.MinWindowTrackSize;
                mmi.ptMaxTrackSize.x = Math.Max(maxTrack.Width, minTrackWindowSize.Width);
                mmi.ptMaxTrackSize.y = Math.Max(maxTrack.Height, minTrackWindowSize.Height);
            }

            if (!maximizedBounds.IsEmpty && !IsRestrictedWindow) {
                mmi.ptMaxPosition.x = maximizedBounds.X;
                mmi.ptMaxPosition.y = maximizedBounds.Y;
                mmi.ptMaxSize.x = maximizedBounds.Width;
                mmi.ptMaxSize.y = maximizedBounds.Height;
            }

            if (IsRestrictedWindow) {
                mmi.ptMinTrackSize.x = Math.Max(mmi.ptMinTrackSize.x, 100);
                mmi.ptMinTrackSize.y = Math.Max(mmi.ptMinTrackSize.y, SystemInformation.CaptionButtonSize.Height * 3);
            }

            Marshal.StructureToPtr(mmi, m.LParam, false);
            m.Result = IntPtr.Zero;
        }

        /// <devdoc>
        ///     WM_INITMENUPOPUP handler
        /// </devdoc>
        /// <internalonly/>
        private void WmInitMenuPopup(ref Message m) {

            MainMenu curMenu = (MainMenu)Properties.GetObject(PropCurMenu);
            if (curMenu != null) {

                //curMenu.UpdateRtl((RightToLeft == RightToLeft.Yes));

                if (curMenu.ProcessInitMenuPopup(m.WParam))
                    return;
            }
            base.WndProc(ref m);
        }

        /// <devdoc>
        ///     Handles the WM_MENUCHAR message
        /// </devdoc>
        /// <internalonly/>
        private void WmMenuChar(ref Message m) {
            MainMenu curMenu = (MainMenu)Properties.GetObject(PropCurMenu);
            if (curMenu == null) {
                
                Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                if (formMdiParent != null && formMdiParent.Menu != null) {
                    UnsafeNativeMethods.PostMessage(new HandleRef(formMdiParent, formMdiParent.Handle), NativeMethods.WM_SYSCOMMAND, new IntPtr(NativeMethods.SC_KEYMENU), m.WParam);
                    m.Result = (IntPtr)NativeMethods.Util.MAKELONG(0, 1);
                    return;
                }
            }
            if (curMenu != null) {
                curMenu.WmMenuChar(ref m);
                if (m.Result != IntPtr.Zero) {
                    // This char is a mnemonic on our menu.
                    return;
                }
            }

            base.WndProc(ref m);
        }

        /// <devdoc>
        ///     WM_MDIACTIVATE handler
        /// </devdoc>
        /// <internalonly/>
        private void WmMdiActivate(ref Message m) {
            base.WndProc(ref m);
            Debug.Assert(Properties.GetObject(PropFormMdiParent) != null, "how is formMdiParent null?");
            Debug.Assert(IsHandleCreated, "how is handle 0?");

            Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);

            if (formMdiParent != null) {
                // This message is propagated twice by the MDIClient window. Once to the 
                // window being deactivated and once to the window being activated.
                if (Handle == m.WParam) {
                    formMdiParent.DeactivateMdiChild();
                }
                else  if (Handle == m.LParam) {
                    formMdiParent.ActivateMdiChildInternal(this);
                }
            }
        }

        private void WmNcButtonDown(ref Message m) {
            if (IsMdiChild) {
                Form formMdiParent = (Form)Properties.GetObject(PropFormMdiParent);
                if (formMdiParent.ActiveMdiChildInternal == this) {
                    if (ActiveControl != null && !ActiveControl.ContainsFocus) {
                        InnerMostActiveContainerControl.FocusActiveControlInternal();
                    }
                }
            }
            base.WndProc(ref m);
        }

        /// <devdoc>
        ///     WM_NCDESTROY handler
        /// </devdoc>
        /// <internalonly/>
        private void WmNCDestroy(ref Message m) {
            MainMenu mainMenu   = Menu;
            MainMenu dummyMenu  = (MainMenu)Properties.GetObject(PropDummyMenu);
            MainMenu curMenu    = (MainMenu)Properties.GetObject(PropCurMenu);
            MainMenu mergedMenu = (MainMenu)Properties.GetObject(PropMergedMenu);

            if (mainMenu != null) {
                mainMenu.ClearHandles();
            }
            if (curMenu != null) {
                curMenu.ClearHandles();
            }
            if (mergedMenu != null) {
                mergedMenu.ClearHandles();
            }
            if (dummyMenu != null) {
                dummyMenu.ClearHandles();
            }

            base.WndProc(ref m);

            // Destroy the owner window, if we created one.  We
            // cannot do this in OnHandleDestroyed, because at
            // that point our handle is not actually destroyed so
            // destroying our parent actually causes a recursive
            // WM_DESTROY.
            if (ownerWindow != null) {
                ownerWindow.DestroyHandle();
                ownerWindow = null;
            }

            if (Modal && dialogResult == DialogResult.None) {
                DialogResult = DialogResult.Cancel;
            }
        }

        /// <devdoc>
        ///     WM_NCHITTEST handler
        /// </devdoc>
        /// <internalonly/>
        private void WmNCHitTest(ref Message m) {
            if (formState[FormStateRenderSizeGrip] != 0 ) {
                int x = NativeMethods.Util.LOWORD(m.LParam);
                int y = NativeMethods.Util.HIWORD(m.LParam);

                // Convert to client coordinates
                //
                NativeMethods.POINT pt = new NativeMethods.POINT(x, y);
                UnsafeNativeMethods.ScreenToClient(new HandleRef(this, this.Handle), pt );

                Size clientSize = ClientSize;

                // If the grip is not fully visible the grip area could overlap with the system control box; we need to disable 
                // the grip area in this case not to get in the way of the control box.  We only need to check for the client's
                // height since the window width will be at least the size of the control box which is always bigger than the 
                // grip width.
                if( pt.x >= (clientSize.Width  - SizeGripSize) && 
                    pt.y >= (clientSize.Height - SizeGripSize) && 
                    clientSize.Height >= SizeGripSize){
                        m.Result = IsMirrored ? (IntPtr)NativeMethods.HTBOTTOMLEFT : (IntPtr)NativeMethods.HTBOTTOMRIGHT;
                        return;
                }
            }

            base.WndProc(ref m);

            // If we got any of the "edge" hits (bottom, top, topleft, etc),
            // and we're AutoSizeMode.GrowAndShrink, return non-resizable border
            // The edge values are the 8 values from HTLEFT (10) to HTBOTTOMRIGHT (17).
            if (AutoSizeMode == AutoSizeMode.GrowAndShrink)
            {
                int result = unchecked( (int) (long)m.Result);
                if (result >= NativeMethods.HTLEFT && 
                    result <= NativeMethods.HTBOTTOMRIGHT) {
                    m.Result = (IntPtr)NativeMethods.HTBORDER;
                }
            }
        }


        /// <devdoc>
        ///     WM_SHOWWINDOW handler
        /// </devdoc>
        /// <internalonly/>
        private void WmShowWindow(ref Message m) {
            formState[FormStateSWCalled] = 1;
            base.WndProc(ref m);
        }


        /// <devdoc>
        ///     WM_SYSCOMMAND handler
        /// </devdoc>
        /// <internalonly/>
        private void WmSysCommand(ref Message m) {
            bool callDefault = true;

            int sc = (NativeMethods.Util.LOWORD(m.WParam) & 0xFFF0);

            switch (sc) {
                case NativeMethods.SC_CLOSE:
                    CloseReason = CloseReason.UserClosing;
                    if (IsMdiChild && !ControlBox) {
                        callDefault = false;
                    }
                    break;
                case NativeMethods.SC_KEYMENU:
                    if (IsMdiChild && !ControlBox) {
                        callDefault = false;
                    }
                    break;
                case NativeMethods.SC_SIZE:
                case NativeMethods.SC_MOVE:
                    // Set this before WM_ENTERSIZELOOP because WM_GETMINMAXINFO can be called before WM_ENTERSIZELOOP.
                    formStateEx[FormStateExInModalSizingLoop] = 1;
                    break;
                case NativeMethods.SC_CONTEXTHELP:
                    CancelEventArgs e = new CancelEventArgs(false);
                    OnHelpButtonClicked(e);
                    if (e.Cancel == true) {
                        callDefault = false;
                    }
                    break;
            }

            if (Command.DispatchID(NativeMethods.Util.LOWORD(m.WParam))) {
                callDefault = false;
            }

            if (callDefault) {

                base.WndProc(ref m);
            }
        }

        /// <devdoc>
        ///     WM_SIZE handler
        /// </devdoc>
        /// <internalonly/>
        private void WmSize(ref Message m) {

            // If this is an MDI parent, don't pass WM_SIZE to the default
            // window proc. We handle resizing the MDIClient window ourselves
            // (using ControlDock.FILL).
            //
            if (ctlClient == null) {
                base.WndProc(ref m);
                if (MdiControlStrip == null && MdiParentInternal != null && MdiParentInternal.ActiveMdiChildInternal == this) {
                    int wParam = m.WParam.ToInt32();
                    MdiParentInternal.UpdateMdiControlStrip(wParam == NativeMethods.SIZE_MAXIMIZED);
                }
            }
        }

        /// <devdoc>
        ///     WM_UNINITMENUPOPUP handler
        /// </devdoc>
        /// <internalonly/>
        private void WmUnInitMenuPopup(ref Message m) {
            if (Menu != null) {
                //Whidbey addition - also raise the MainMenu.Collapse event for the current menu
                Menu.OnCollapse(EventArgs.Empty);
            }
        }

        /// <devdoc>
        ///     WM_WINDOWPOSCHANGED handler
        /// </devdoc>
        /// <internalonly/>
        private void WmWindowPosChanged(ref Message m) {

            //           We must update the windowState, because resize is fired
            //           from here... (in Control)
            UpdateWindowState();
            base.WndProc(ref m);

            RestoreWindowBoundsIfNecessary();
        }        

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.WndProc"]/*' />
        /// <devdoc>
        ///     Base wndProc encapsulation.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_NCACTIVATE:
                    if (IsRestrictedWindow) {
                        BeginInvoke(new MethodInvoker(RestrictedProcessNcActivate));
                    }
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_NCLBUTTONDOWN:
                case NativeMethods.WM_NCRBUTTONDOWN:
                case NativeMethods.WM_NCMBUTTONDOWN:
                case NativeMethods.WM_NCXBUTTONDOWN:
                    WmNcButtonDown(ref m);
                    break;
                case NativeMethods.WM_ACTIVATE:
                    WmActivate(ref m);
                    break;
                case NativeMethods.WM_MDIACTIVATE:
                    WmMdiActivate(ref m);
                    break;
                case NativeMethods.WM_CLOSE:
                    if (CloseReason == CloseReason.None) {
                        CloseReason = CloseReason.TaskManagerClosing;
                    }
                    WmClose(ref m);
                    break;

                case NativeMethods.WM_QUERYENDSESSION:
                case NativeMethods.WM_ENDSESSION:
                    CloseReason = CloseReason.WindowsShutDown;
                    WmClose(ref m);
                    break;
                case NativeMethods.WM_ENTERSIZEMOVE:
                    WmEnterSizeMove(ref m);
                    DefWndProc(ref m);
                    break;
                case NativeMethods.WM_EXITSIZEMOVE:
                    WmExitSizeMove(ref m);
                    DefWndProc(ref m);
                    break;
                case NativeMethods.WM_CREATE:
                    WmCreate(ref m);
                    break;
                case NativeMethods.WM_ERASEBKGND:
                    WmEraseBkgnd(ref m);
                    break;

                case NativeMethods.WM_INITMENUPOPUP:
                    WmInitMenuPopup(ref m);
                    break;
                case NativeMethods.WM_UNINITMENUPOPUP:
                    WmUnInitMenuPopup(ref m);
                    break;
                case NativeMethods.WM_MENUCHAR:
                    WmMenuChar(ref m);
                    break;
                case NativeMethods.WM_NCDESTROY:
                    WmNCDestroy(ref m);
                    break;
                case NativeMethods.WM_NCHITTEST:
                    WmNCHitTest(ref m);
                    break;
                case NativeMethods.WM_SHOWWINDOW:
                    WmShowWindow(ref m);
                    break;
                case NativeMethods.WM_SIZE:
                    WmSize(ref m);
                    break;
                case NativeMethods.WM_SYSCOMMAND:
                    WmSysCommand(ref m);
                    break;
                case NativeMethods.WM_GETMINMAXINFO:
                    WmGetMinMaxInfo(ref m);
                    break;
                case NativeMethods.WM_WINDOWPOSCHANGED:
                    WmWindowPosChanged(ref m);
                    break;
                //case NativeMethods.WM_WINDOWPOSCHANGING:
                //    WmWindowPosChanging(ref m);
                //    break;
                case NativeMethods.WM_ENTERMENULOOP:
                    WmEnterMenuLoop(ref m);
                    break;
                case NativeMethods.WM_EXITMENULOOP:
                    WmExitMenuLoop(ref m);
                    break;
                case NativeMethods.WM_CAPTURECHANGED:
                    base.WndProc(ref m);
                    // This is a work-around for the Win32 scroll bar; it
                    // doesn't release it's capture in response to a CAPTURECHANGED
                    // message, so we force capture away if no button is down.
                    //
                    if (CaptureInternal && MouseButtons == (MouseButtons)0) {
                        CaptureInternal = false;
                    }
                    break;
                case NativeMethods.WM_GETDPISCALEDSIZE:
                    Debug.Assert(NativeMethods.Util.SignedLOWORD(m.WParam) == NativeMethods.Util.SignedHIWORD(m.WParam), "Non-square pixels!");
                    WmGetDpiScaledSize(ref m);
                    break;
                case NativeMethods.WM_DPICHANGED:
                    WmDpiChanged(ref m);
                    break;
                default:
                    base.WndProc(ref m);
                    break;
            }
        }

#if SECURITY_DIALOG
        private class SecurityMenuItem : ICommandExecutor {
            Form owner;
            Command cmd;

            internal SecurityMenuItem(Form owner) {
                this.owner = owner;
                cmd = new Command(this);
            }

            internal int ID {
                get {
                    return cmd.ID;
                }
            }

            [
                ReflectionPermission(SecurityAction.Assert, TypeInformation=true, MemberAccess=true),
                UIPermission(SecurityAction.Assert, Window=UIPermissionWindow.AllWindows),
                EnvironmentPermission(SecurityAction.Assert, Unrestricted=true),
                FileIOPermission(SecurityAction.Assert, Unrestricted=true),
                SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode),
            ]
            void ICommandExecutor.Execute() {
                /// 
                Form information = (Form)Activator.CreateInstance(typeof(Form).Module.Assembly.GetType("System.Windows.Forms.SysInfoForm"), new object[] {owner.IsRestrictedWindow});
                information.ShowDialog();
            }
        }
#endif

        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ControlCollection"]/*' />
        /// <devdoc>
        ///    <para>Represents a collection of controls on the form.</para>
        /// </devdoc>
        [ComVisible(false)]
        public new class ControlCollection : Control.ControlCollection {

            private Form owner;

            /*C#r:protected*/

            /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ControlCollection.ControlCollection"]/*' />
            /// <devdoc>
            /// <para>Initializes a new instance of the ControlCollection class.</para>
            /// </devdoc>
            public ControlCollection(Form owner)
            : base(owner) {
                this.owner = owner;
            }

            /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ControlCollection.Add"]/*' />
            /// <devdoc>
            ///    <para> Adds a control
            ///       to the form.</para>
            /// </devdoc>
            public override void Add(Control value) {
                if (value is MdiClient && owner.ctlClient == null) {
                    if (!owner.TopLevel && !owner.DesignMode) {
                        throw new ArgumentException(SR.MDIContainerMustBeTopLevel, "value");
                    }
                    owner.AutoScroll = false;
                    if (owner.IsMdiChild) {
                        throw new ArgumentException(SR.FormMDIParentAndChild, "value");
                    }
                    owner.ctlClient = (MdiClient)value;
                }

                // make sure we don't add a form that has a valid mdi parent
                //
                if (value is Form && ((Form)value).MdiParentInternal != null) {
                    throw new ArgumentException(SR.FormMDIParentCannotAdd, "value");
                }

                base.Add(value);

                if (owner.ctlClient != null) {
                    owner.ctlClient.SendToBack();
                }
            }

            /// <include file='doc\Form.uex' path='docs/doc[@for="Form.ControlCollection.Remove"]/*' />
            /// <devdoc>
            ///    <para>
            ///       Removes a control from the form.</para>
            /// </devdoc>
            public override void Remove(Control value) {
                if (value == owner.ctlClient) {
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

            internal EnumThreadWindowsCallback()
            {
            }

            internal bool Callback(IntPtr hWnd, IntPtr lParam)
            {
                Debug.Assert(lParam != IntPtr.Zero);
                HandleRef hRef = new HandleRef(null, hWnd);
                IntPtr parent = UnsafeNativeMethods.GetWindowLong(hRef, NativeMethods.GWL_HWNDPARENT);
                if (parent == lParam)
                {
                    // Enumerated window is owned by this Form.
                    // Store it in a list for further treatment.
                    if (this.ownedWindows == null)
                    {
                        this.ownedWindows = new List<HandleRef>();
                    }
                    this.ownedWindows.Add(hRef);
                }
                return true;
            }

            // Resets the owner of all the windows owned by this Form before handle recreation.
            internal void ResetOwners()
            {
                if (this.ownedWindows != null)
                {
                    foreach (HandleRef hRef in this.ownedWindows)
                    {
                        UnsafeNativeMethods.SetWindowLong(hRef, NativeMethods.GWL_HWNDPARENT, NativeMethods.NullHandleRef);
                    }
                }
            }

            // Sets the owner of the windows back to this Form after its handle recreation.
            internal void SetOwners(HandleRef hRefOwner)
            {
                if (this.ownedWindows != null)
                {
                    foreach (HandleRef hRef in this.ownedWindows)
                    {
                        UnsafeNativeMethods.SetWindowLong(hRef, NativeMethods.GWL_HWNDPARENT, hRefOwner);
                    }
                }
            }
        }

        private class SecurityToolTip : IDisposable {
            Form owner;
            string toolTipText;
            bool first = true;
            ToolTipNativeWindow window;

            internal SecurityToolTip(Form owner) {
                this.owner = owner;
                SetupText();
                window = new ToolTipNativeWindow(this);
                SetupToolTip();
                owner.LocationChanged += new EventHandler(FormLocationChanged);
                owner.HandleCreated += new EventHandler(FormHandleCreated);
            }

            CreateParams CreateParams {
                get {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_TAB_CLASSES;
                    SafeNativeMethods.InitCommonControlsEx(icc);

                    CreateParams cp = new CreateParams();
                    cp.Parent = owner.Handle;
                    cp.ClassName = NativeMethods.TOOLTIPS_CLASS;
                    cp.Style |= NativeMethods.TTS_ALWAYSTIP | NativeMethods.TTS_BALLOON;
                    cp.ExStyle = 0;
                    cp.Caption = null;
                    return cp;
                }
            }

            internal bool Modal {
                get {
                    return first;
                }
            }

            public void Dispose() {
                if (owner != null) {
                    owner.LocationChanged -= new EventHandler(FormLocationChanged);
                }
                if (window.Handle != IntPtr.Zero) {
                    window.DestroyHandle();
                    window = null;
                }
            }

            private NativeMethods.TOOLINFO_T GetTOOLINFO() {
                NativeMethods.TOOLINFO_T toolInfo;
                toolInfo = new NativeMethods.TOOLINFO_T();
                toolInfo.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_T));
                toolInfo.uFlags |= NativeMethods.TTF_SUBCLASS;
                toolInfo.lpszText =  this.toolTipText;
                if (owner.RightToLeft == RightToLeft.Yes) {
                    toolInfo.uFlags |= NativeMethods.TTF_RTLREADING;
                }
                if (!first) {
                    toolInfo.uFlags |= NativeMethods.TTF_TRANSPARENT;
                    toolInfo.hwnd = owner.Handle;
                    Size s = SystemInformation.CaptionButtonSize;
                    Rectangle r = new Rectangle(owner.Left, owner.Top, s.Width, SystemInformation.CaptionHeight);
                    r = owner.RectangleToClient(r);
                    r.Width -= r.X;
                    r.Y += 1;
                    toolInfo.rect = NativeMethods.RECT.FromXYWH(r.X, r.Y, r.Width, r.Height);
                    toolInfo.uId = IntPtr.Zero;
                }
                else {
                    toolInfo.uFlags |= NativeMethods.TTF_IDISHWND | NativeMethods.TTF_TRACK;
                    toolInfo.hwnd = IntPtr.Zero;
                    toolInfo.uId = owner.Handle;
                }
                return toolInfo;
            }

            private void SetupText() {
                owner.EnsureSecurityInformation();
                string mainText = SR.SecurityToolTipMainText;
                string sourceInfo = string.Format(SR.SecurityToolTipSourceInformation, owner.securitySite);
                this.toolTipText =  string.Format(SR.SecurityToolTipTextFormat, mainText, sourceInfo);
            }

            private void SetupToolTip() {
                window.CreateHandle(CreateParams);

                SafeNativeMethods.SetWindowPos(new HandleRef(window, window.Handle), NativeMethods.HWND_TOPMOST,
                                  0, 0, 0, 0,
                                  NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
                                  NativeMethods.SWP_NOACTIVATE);

                UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, owner.Width);

                UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_SETTITLE, NativeMethods.TTI_WARNING, SR.SecurityToolTipCaption);

                if (0 == (int)UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_ADDTOOL, 0, GetTOOLINFO())) {
                    Debug.Fail("TTM_ADDTOOL failed for security tip");
                }

                UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_ACTIVATE, 1, 0);
                Show();
            }

            private void RecreateHandle() {
                if (window != null)
                {
                    if (window.Handle != IntPtr.Zero) {
                        window.DestroyHandle();
                    }
                    SetupToolTip();
                }
            }

            private void FormHandleCreated(object sender, EventArgs e) {
                RecreateHandle();
            }

            private void FormLocationChanged(object sender, EventArgs e) {
                if (window != null && first) {
                    Size s = SystemInformation.CaptionButtonSize;

                    if (owner.WindowState == FormWindowState.Minimized) {
                        Pop(true /*noLongerFirst*/);
                    }
                    else {
                        UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_TRACKPOSITION, 0, NativeMethods.Util.MAKELONG(owner.Left + s.Width / 2, owner.Top + SystemInformation.CaptionHeight));
                    }
                }
                else {
                    Pop(true /*noLongerFirst*/);
                }
            }

            internal void Pop(bool noLongerFirst) {
                if (noLongerFirst) {
                    first = false;
                }
                UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_TRACKACTIVATE, 0, GetTOOLINFO());
                UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_DELTOOL, 0, GetTOOLINFO());
                UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_ADDTOOL, 0, GetTOOLINFO());
            }

            internal void Show() {
                if (first) {
                    Size s = SystemInformation.CaptionButtonSize;
                    UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_TRACKPOSITION, 0, NativeMethods.Util.MAKELONG(owner.Left + s.Width / 2, owner.Top + SystemInformation.CaptionHeight));
                    UnsafeNativeMethods.SendMessage(new HandleRef(window, window.Handle), NativeMethods.TTM_TRACKACTIVATE, 1, GetTOOLINFO());
                }
            }

            private void WndProc(ref Message msg) {
                if (first) {
                    if (msg.Msg == NativeMethods.WM_LBUTTONDOWN
                        || msg.Msg == NativeMethods.WM_RBUTTONDOWN
                        || msg.Msg == NativeMethods.WM_MBUTTONDOWN
                        || msg.Msg == NativeMethods.WM_XBUTTONDOWN) {

                        Pop(true /*noLongerFirst*/);
                    }
                }
                window.DefWndProc(ref msg);
            }

            private sealed class ToolTipNativeWindow : NativeWindow {
                SecurityToolTip control;

                internal ToolTipNativeWindow(SecurityToolTip control) {
                    this.control = control;
                }

                protected override void WndProc(ref Message m) {
                    if (control != null) {
                        control.WndProc(ref m);
                    }
                }
            }
        }
    }
}

