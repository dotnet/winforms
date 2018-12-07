// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define DEBUG_PREFERREDSIZE



[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope="member", Target="System.Windows.Forms.Control+ActiveXFontMarshaler..ctor()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope="member", Target="System.Windows.Forms.Control.PreProcessControlMessageInternal(System.Windows.Forms.Control, System.Windows.Forms.Message):System.Windows.Forms.PreProcessControlState")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Scope="member", Target="System.Windows.Forms.Control.OnPreviewKeyDown(System.Windows.Forms.PreviewKeyDownEventArgs):System.Void")]


namespace System.Windows.Forms {
    using Accessibility;
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Configuration.Assemblies;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Security;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Runtime.Remoting;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Internal;
    using Encoding = System.Text.Encoding;
    using System.Drawing.Imaging;
    using System.Windows.Forms.Layout;
    using System.Runtime.Versioning;
    using Automation;

    using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
    using Collections.Generic;

    /// <include file='doc\Control.uex' path='docs/doc[@for="Control"]/*' />
    /// <devdoc>
    ///    <para>Defines the base class for controls, which are components
    ///       with visual representation.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Text)),
    DefaultEvent(nameof(Click)),
    Designer("System.Windows.Forms.Design.ControlDesigner, " + AssemblyRef.SystemDesign),
    DesignerSerializer("System.Windows.Forms.Design.ControlCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign),
    ToolboxItemFilter("System.Windows.Forms")
    ]
    public partial class Control :
    Component,
    UnsafeNativeMethods.IOleControl,
    UnsafeNativeMethods.IOleObject,
    UnsafeNativeMethods.IOleInPlaceObject,
    UnsafeNativeMethods.IOleInPlaceActiveObject,
    UnsafeNativeMethods.IOleWindow,
    UnsafeNativeMethods.IViewObject,
    UnsafeNativeMethods.IViewObject2,
    UnsafeNativeMethods.IPersist,
    UnsafeNativeMethods.IPersistStreamInit,
    UnsafeNativeMethods.IPersistPropertyBag,
    UnsafeNativeMethods.IPersistStorage,
    UnsafeNativeMethods.IQuickActivate,
    ISupportOleDropSource,
    IDropTarget,
    ISynchronizeInvoke,
    IWin32Window,
    IArrangedElement,
    IBindableComponent,
    IKeyboardToolTip {

#if FINALIZATION_WATCH
        static readonly TraceSwitch ControlFinalization = new TraceSwitch("ControlFinalization", "Tracks the creation and destruction of finalization");
        internal static string GetAllocationStack() {
            if (ControlFinalization.TraceVerbose) {
            //  the operation is safe (data obtained from the CLR). This code is for debugging purposes only.
                new EnvironmentPermission(PermissionState.Unrestricted).Assert();
                try {
                    return Environment.StackTrace;
                }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }
            else {
                return "Enable 'ControlFinalization' switch to see stack of allocation";
            }

        }
        private string allocationSite = Control.GetAllocationStack();
#endif

#if DEBUG
        internal static readonly TraceSwitch PaletteTracing = new TraceSwitch("PaletteTracing", "Debug Palette code");
        internal static readonly TraceSwitch ControlKeyboardRouting = new TraceSwitch("ControlKeyboardRouting", "Debug Keyboard routing for controls");
        internal static readonly TraceSwitch FocusTracing = new TraceSwitch("FocusTracing", "Debug focus/active control/enter/leave");
        private static readonly BooleanSwitch AssertOnControlCreateSwitch = new BooleanSwitch("AssertOnControlCreate", "Assert when anything directly deriving from control is created.");
        internal static readonly BooleanSwitch TraceMnemonicProcessing = new BooleanSwitch("TraceCanProcessMnemonic", "Trace mnemonic processing calls to assure right child-parent call ordering.");

        internal void TraceCanProcessMnemonic(){
            if( TraceMnemonicProcessing.Enabled ){
            string str;
                try {
                    str = string.Format( CultureInfo.CurrentCulture, "{0}<{1}>", this.GetType().Name, this.Text );
                    int maxFrameCount = new StackTrace().FrameCount;
                    if (maxFrameCount > 5) {
                        maxFrameCount = 5;
                    }
                    int frameIndex = 1;
                    while( frameIndex < maxFrameCount) {
                        StackFrame sf = new StackFrame(frameIndex);
                        if( frameIndex == 2 && sf.GetMethod().Name.Equals("CanProcessMnemonic") ){ // log immediate call if in a virtual/recursive call.
                            break;
                        }
                        str += new StackTrace(sf).ToString().TrimEnd();
                        frameIndex++;
                    }
                    if( frameIndex > 2 ){ // new CanProcessMnemonic virtual/recursive call stack.
                        str = "\r\n" + str;
                    }
                }
                catch (Exception ex) {
                    str = ex.ToString();
                }
                Debug.WriteLine( str );
            }
        }
#else
        internal static readonly TraceSwitch ControlKeyboardRouting;
        internal static readonly TraceSwitch PaletteTracing;
        internal static readonly TraceSwitch FocusTracing;
#endif

#if DEBUG
        internal static readonly BooleanSwitch BufferPinkRect = new BooleanSwitch("BufferPinkRect", "Renders a pink rectangle with painting double buffered controls");
        internal static readonly BooleanSwitch BufferDisabled = new BooleanSwitch("BufferDisabled", "Makes double buffered controls non-double buffered");
#else
        internal static readonly BooleanSwitch BufferPinkRect;
#endif

        private static int WM_GETCONTROLNAME;
        private static int WM_GETCONTROLTYPE;

        static Control() {
            WM_GETCONTROLNAME = SafeNativeMethods.RegisterWindowMessage("WM_GETCONTROLNAME");
            WM_GETCONTROLTYPE = SafeNativeMethods.RegisterWindowMessage("WM_GETCONTROLTYPE");

        }

        internal const int STATE_CREATED                = 0x00000001;
        internal const int STATE_VISIBLE                = 0x00000002;
        internal const int STATE_ENABLED                = 0x00000004;
        internal const int STATE_TABSTOP                = 0x00000008;
        internal const int STATE_RECREATE               = 0x00000010;
        internal const int STATE_MODAL                  = 0x00000020;
        internal const int STATE_ALLOWDROP              = 0x00000040;
        internal const int STATE_DROPTARGET             = 0x00000080;
        internal const int STATE_NOZORDER               = 0x00000100;
        internal const int STATE_LAYOUTDEFERRED         = 0x00000200;
        internal const int STATE_USEWAITCURSOR          = 0x00000400;
        internal const int STATE_DISPOSED               = 0x00000800;
        internal const int STATE_DISPOSING              = 0x00001000;
        internal const int STATE_MOUSEENTERPENDING      = 0x00002000;
        internal const int STATE_TRACKINGMOUSEEVENT     = 0x00004000;
        internal const int STATE_THREADMARSHALLPENDING  = 0x00008000;
        internal const int STATE_SIZELOCKEDBYOS         = 0x00010000;
        internal const int STATE_CAUSESVALIDATION       = 0x00020000;
        internal const int STATE_CREATINGHANDLE         = 0x00040000;
        internal const int STATE_TOPLEVEL               = 0x00080000;
        internal const int STATE_ISACCESSIBLE           = 0x00100000;
        internal const int STATE_OWNCTLBRUSH            = 0x00200000;
        internal const int STATE_EXCEPTIONWHILEPAINTING = 0x00400000;
        internal const int STATE_LAYOUTISDIRTY          = 0x00800000;
        internal const int STATE_CHECKEDHOST            = 0x01000000;
        internal const int STATE_HOSTEDINDIALOG         = 0x02000000;
        internal const int STATE_DOUBLECLICKFIRED       = 0x04000000;
        internal const int STATE_MOUSEPRESSED           = 0x08000000;
        internal const int STATE_VALIDATIONCANCELLED    = 0x10000000;
        internal const int STATE_PARENTRECREATING       = 0x20000000;
        internal const int STATE_MIRRORED               = 0x40000000;        

        // When we change RightToLeft, we need to change the scrollbar thumb.
        // We can't do that until after the control has been created, and all the items added
        // back. This is because the system control won't know the nMin and nMax of the scroll
        // bar until the items are added. So in RightToLeftChanged, we set a flag that indicates
        // that we want to set the scroll position. In OnHandleCreated we check this flag,
        // and if set, we BeginInvoke. We have to BeginInvoke since we have to wait until the items
        // are added. We only want to do this when RightToLeft changes thus the flags 
        // STATE2_HAVEINVOKED and STATE2_SETSCROLLPOS. Otherwise we would do this on each HandleCreated.
        private  const int STATE2_HAVEINVOKED                       = 0x00000001;
        private  const int STATE2_SETSCROLLPOS                      = 0x00000002;
        private  const int STATE2_LISTENINGTOUSERPREFERENCECHANGED  = 0x00000004;   // set when the control is listening to SystemEvents.UserPreferenceChanged.
        internal const int STATE2_INTERESTEDINUSERPREFERENCECHANGED = 0x00000008;   // if set, the control will listen to SystemEvents.UserPreferenceChanged when TopLevel is true and handle is created.
        internal const int STATE2_MAINTAINSOWNCAPTUREMODE           = 0x00000010;   // if set, the control DOES NOT necessarily take capture on MouseDown
        private  const int STATE2_BECOMINGACTIVECONTROL             = 0x00000020;   // set to true by ContainerControl when this control is becoming its active control

        private  const int STATE2_CLEARLAYOUTARGS                   = 0x00000040;   // if set, the next time PerformLayout is called, cachedLayoutEventArg will be cleared.
        private  const int STATE2_INPUTKEY                          = 0x00000080;
        private  const int STATE2_INPUTCHAR                         = 0x00000100;
        private  const int STATE2_UICUES                            = 0x00000200;
        private  const int STATE2_ISACTIVEX                         = 0x00000400;
        internal const int STATE2_USEPREFERREDSIZECACHE             = 0x00000800;
        internal const int STATE2_TOPMDIWINDOWCLOSING               = 0x00001000;
        internal const int STATE2_CURRENTLYBEINGSCALED              = 0x00002000;   // if set, the control is being scaled, currently

        private static readonly object EventAutoSizeChanged           = new object();
        private static readonly object EventKeyDown                   = new object();
        private static readonly object EventKeyPress                  = new object();
        private static readonly object EventKeyUp                     = new object();
        private static readonly object EventMouseDown                 = new object();
        private static readonly object EventMouseEnter                = new object();
        private static readonly object EventMouseLeave                = new object();
        private static readonly object EventDpiChangedBeforeParent    = new object();
        private static readonly object EventDpiChangedAfterParent     = new object();
        private static readonly object EventMouseHover                = new object();
        private static readonly object EventMouseMove                 = new object();
        private static readonly object EventMouseUp                   = new object();
        private static readonly object EventMouseWheel                = new object();
        private static readonly object EventClick                     = new object();
        private static readonly object EventClientSize                = new object();
        private static readonly object EventDoubleClick               = new object();
        private static readonly object EventMouseClick                = new object();
        private static readonly object EventMouseDoubleClick          = new object();
        private static readonly object EventMouseCaptureChanged       = new object();
        private static readonly object EventMove                      = new object();
        private static readonly object EventResize                    = new object();
        private static readonly object EventLayout                    = new object();
        private static readonly object EventGotFocus                  = new object();
        private static readonly object EventLostFocus                 = new object();
        private static readonly object EventEnabledChanged            = new object();
        private static readonly object EventEnter                     = new object();
        private static readonly object EventLeave                     = new object();
        private static readonly object EventHandleCreated             = new object();
        private static readonly object EventHandleDestroyed           = new object();
        private static readonly object EventVisibleChanged            = new object();
        private static readonly object EventControlAdded              = new object();
        private static readonly object EventControlRemoved            = new object();
        private static readonly object EventChangeUICues              = new object();
        private static readonly object EventSystemColorsChanged       = new object();
        private static readonly object EventValidating                = new object();
        private static readonly object EventValidated                 = new object();
        private static readonly object EventStyleChanged              = new object();
        private static readonly object EventImeModeChanged            = new object();
        private static readonly object EventHelpRequested             = new object();
        private static readonly object EventPaint                     = new object();
        private static readonly object EventInvalidated               = new object();
        private static readonly object EventQueryContinueDrag         = new object();
        private static readonly object EventGiveFeedback              = new object();
        private static readonly object EventDragEnter                 = new object();
        private static readonly object EventDragLeave                 = new object();
        private static readonly object EventDragOver                  = new object();
        private static readonly object EventDragDrop                  = new object();
        private static readonly object EventQueryAccessibilityHelp    = new object();
        private static readonly object EventBackgroundImage           = new object();
        private static readonly object EventBackgroundImageLayout     = new object();
        private static readonly object EventBindingContext            = new object();
        private static readonly object EventBackColor                 = new object();
        private static readonly object EventParent                    = new object();
        private static readonly object EventVisible                   = new object();
        private static readonly object EventText                      = new object();
        private static readonly object EventTabStop                   = new object();
        private static readonly object EventTabIndex                  = new object();
        private static readonly object EventSize                      = new object();
        private static readonly object EventRightToLeft               = new object();
        private static readonly object EventLocation                  = new object();
        private static readonly object EventForeColor                 = new object();
        private static readonly object EventFont                      = new object();
        private static readonly object EventEnabled                   = new object();
        private static readonly object EventDock                      = new object();
        private static readonly object EventCursor                    = new object();
        private static readonly object EventContextMenu               = new object();
        private static readonly object EventContextMenuStrip          = new object();
        private static readonly object EventCausesValidation          = new object();
        private static readonly object EventRegionChanged             = new object();
        private static readonly object EventMarginChanged             = new object();
        internal static readonly object EventPaddingChanged           = new object();
        private static readonly object EventPreviewKeyDown            = new object();

        #if WIN95_SUPPORT
        private static int mouseWheelMessage = NativeMethods.WM_MOUSEWHEEL;
        private static bool mouseWheelRoutingNeeded;
        private static bool mouseWheelInit;
        #endif

        private static int threadCallbackMessage;

        // Initially check for illegal multithreading based on whether the
        // debugger is attached.
        [ResourceExposure(ResourceScope.Process)]
        private static bool checkForIllegalCrossThreadCalls = Debugger.IsAttached;
        private static ContextCallback invokeMarshaledCallbackHelperDelegate;

        [ ThreadStatic ]
        private static bool inCrossThreadSafeCall = false;

// disable csharp compiler warning #0414: field assigned unused value
#pragma warning disable 0414
        [ ThreadStatic ]
        internal static HelpInfo currentHelpInfo = null;
#pragma warning restore 0414

        private static FontHandleWrapper defaultFontHandleWrapper;

        private const short PaintLayerBackground = 1;
        private const short PaintLayerForeground = 2;

        private const byte RequiredScalingEnabledMask = 0x10;
        private const byte RequiredScalingMask = 0x0F;

        private const byte HighOrderBitMask = 0x80;

        private static Font defaultFont;

        // Property store keys for properties.  The property store allocates most efficiently
        // in groups of four, so we try to lump properties in groups of four based on how
        // likely they are going to be used in a group.
        //
        private static readonly int PropName                                = PropertyStore.CreateKey();
        private static readonly int PropBackBrush                           = PropertyStore.CreateKey();
        private static readonly int PropFontHeight                          = PropertyStore.CreateKey();
        private static readonly int PropCurrentAmbientFont                  = PropertyStore.CreateKey();

        private static readonly int PropControlsCollection                  = PropertyStore.CreateKey();
        private static readonly int PropBackColor                           = PropertyStore.CreateKey();
        private static readonly int PropForeColor                           = PropertyStore.CreateKey();
        private static readonly int PropFont                                = PropertyStore.CreateKey();

        private static readonly int PropBackgroundImage                     = PropertyStore.CreateKey();
        private static readonly int PropFontHandleWrapper                   = PropertyStore.CreateKey();
        private static readonly int PropUserData                            = PropertyStore.CreateKey();
        private static readonly int PropContextMenu                         = PropertyStore.CreateKey();

        private static readonly int PropCursor                              = PropertyStore.CreateKey();
        private static readonly int PropRegion                              = PropertyStore.CreateKey();
        private static readonly int PropRightToLeft                         = PropertyStore.CreateKey();

        private static readonly int PropBindings                            = PropertyStore.CreateKey();
        private static readonly int PropBindingManager                      = PropertyStore.CreateKey();
        private static readonly int PropAccessibleDefaultActionDescription  = PropertyStore.CreateKey();
        private static readonly int PropAccessibleDescription               = PropertyStore.CreateKey();

        private static readonly int PropAccessibility                       = PropertyStore.CreateKey();
        private static readonly int PropNcAccessibility                     = PropertyStore.CreateKey();
        private static readonly int PropAccessibleName                      = PropertyStore.CreateKey();
        private static readonly int PropAccessibleRole                      = PropertyStore.CreateKey();

        private static readonly int PropPaintingException                   = PropertyStore.CreateKey();
        private static readonly int PropActiveXImpl                         = PropertyStore.CreateKey();
        private static readonly int PropControlVersionInfo                  = PropertyStore.CreateKey();
        private static readonly int PropBackgroundImageLayout               = PropertyStore.CreateKey();

        private static readonly int PropAccessibleHelpProvider              = PropertyStore.CreateKey();
        private static readonly int PropContextMenuStrip                    = PropertyStore.CreateKey();
        private static readonly int PropAutoScrollOffset                    = PropertyStore.CreateKey();
        private static readonly int PropUseCompatibleTextRendering          = PropertyStore.CreateKey();

        private static readonly int PropImeWmCharsToIgnore                  = PropertyStore.CreateKey();
        private static readonly int PropImeMode                             = PropertyStore.CreateKey();
        private static readonly int PropDisableImeModeChangedCount          = PropertyStore.CreateKey();
        private static readonly int PropLastCanEnableIme                    = PropertyStore.CreateKey();

        private static readonly int PropCacheTextCount                      = PropertyStore.CreateKey();
        private static readonly int PropCacheTextField                      = PropertyStore.CreateKey();
        private static readonly int PropAmbientPropertiesService            = PropertyStore.CreateKey();

        private static bool needToLoadComCtl = true;

        // This switch determines the default text rendering engine to use by some controls that support switching rendering engine.
        // CheckedListBox, PropertyGrid, GroupBox, Label and LinkLabel, and ButtonBase controls.
        // True means use GDI+, false means use GDI (TextRenderer).
        internal static bool UseCompatibleTextRenderingDefault = true;

        ///////////////////////////////////////////////////////////////////////
        // Control per instance members
        //
        // Note: Do not add anything to this list unless absolutely neccessary.
        //       Every control on a form has the overhead of all of these
        //       variables!
        //
        // Begin Members {

        // List of properties that are generally set, so we keep them directly on
        // control.
        //

        // Resist the temptation to make this variable 'internal' rather than
        // private. Handle access should be tightly controlled, and is in this
        // file.  Making it 'internal' makes controlling it quite difficult.
        private ControlNativeWindow           window;

        private Control                       parent;
        private Control                       reflectParent;
        private CreateParams                  createParams;
        private int                           x;                      // 
        private int                           y;
        private int                           width;
        private int                           height;
        private int                           clientWidth;
        private int                           clientHeight;
        private int                           state;                  // See STATE_ constants above
        private int                           state2;                 // See STATE2_ constants above
        private ControlStyles                 controlStyle;           // User supplied control style
        private int                           tabIndex;
        private string                        text;                   // See ControlStyles.CacheText for usage notes
        private byte                          layoutSuspendCount;
        private byte                          requiredScaling;        // bits 0-4: BoundsSpecified stored in RequiredScaling property.  Bit 5: RequiredScalingEnabled property.
        private PropertyStore                 propertyStore;          // Contains all properties that are not always set.
        private NativeMethods.TRACKMOUSEEVENT trackMouseEvent;
        private short                         updateCount;
        private LayoutEventArgs               cachedLayoutEventArgs;
        private Queue                         threadCallbackList;
        internal int                          deviceDpi;


        // for keeping track of our ui state for focus and keyboard cues.  using a member variable
        // here because we hit this a lot
        //
        private int uiCuesState;

        private const int UISTATE_FOCUS_CUES_MASK               = 0x000F;                 
        private const int UISTATE_FOCUS_CUES_HIDDEN             = 0x0001;
        private const int UISTATE_FOCUS_CUES_SHOW               = 0x0002;
        private const int UISTATE_KEYBOARD_CUES_MASK            = 0x00F0;
        private const int UISTATE_KEYBOARD_CUES_HIDDEN          = 0x0010;
        private const int UISTATE_KEYBOARD_CUES_SHOW            = 0x0020;

        [ThreadStatic]
        private static byte[] tempKeyboardStateArray;

        // } End Members
        ///////////////////////////////////////////////////////////////////////

#if DEBUG
        internal int LayoutSuspendCount {
            get { return layoutSuspendCount; }
        }

        internal void AssertLayoutSuspendCount(int value) {
            Debug.Assert(value == layoutSuspendCount, "Suspend/Resume layout mismatch!");
        }

/*
example usage

#if DEBUG
        int dbgLayoutCheck = LayoutSuspendCount;
#endif
#if DEBUG
        AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
*/

#endif

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Control"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Control'/> class.</para>
        /// </devdoc>
        public Control() : this(true) {
        }

        /// <devdoc>
        /// </devdoc>
        internal Control(bool autoInstallSyncContext) : base() {
#if DEBUG
            if (AssertOnControlCreateSwitch.Enabled) {
                Debug.Assert(this.GetType().BaseType != typeof(Control), "Direct derivative of Control Created: " + this.GetType().FullName);
                Debug.Assert(this.GetType() != typeof(Control), "Control Created!");
            }
#endif
            propertyStore = new PropertyStore();

            DpiHelper.InitializeDpiHelperForWinforms();
            // Initialize DPI to the value on the primary screen, we will have the correct value when the Handle is created.
            deviceDpi = DpiHelper.DeviceDpi;

            window = new ControlNativeWindow(this);
            RequiredScalingEnabled = true;
            RequiredScaling = BoundsSpecified.All;
            tabIndex = -1;

            state = STATE_VISIBLE | STATE_ENABLED | STATE_TABSTOP | STATE_CAUSESVALIDATION;
            state2 = STATE2_INTERESTEDINUSERPREFERENCECHANGED;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick |
                     ControlStyles.UseTextForAccessibility |
                     ControlStyles.Selectable,true);

#if WIN95_SUPPORT
            InitMouseWheelSupport();
#endif

            // We baked the "default default" margin and min size into CommonProperties
            // so that in the common case the PropertyStore would be empty.  If, however,
            // someone overrides these Default* methads, we need to write the default
            // value into the PropertyStore in the ctor.
            //
            if(DefaultMargin != CommonProperties.DefaultMargin) {
                Margin = DefaultMargin;
            }
            if(DefaultMinimumSize != CommonProperties.DefaultMinimumSize) {
                MinimumSize = DefaultMinimumSize;
            }
            if(DefaultMaximumSize != CommonProperties.DefaultMaximumSize) {
                MaximumSize = DefaultMaximumSize;
            }

            // Compute our default size.
            //
            Size defaultSize = DefaultSize;
            width = defaultSize.Width;
            height = defaultSize.Height;

            // DefaultSize may have hit GetPreferredSize causing a PreferredSize to be cached.  The
            // PreferredSize may change as a result of the current size.  Since a  SetBoundsCore did
            // not happen, so we need to clear the preferredSize cache manually.
            CommonProperties.xClearPreferredSizeCache(this);

            if (width != 0 && height != 0) {
                NativeMethods.RECT rect = new NativeMethods.RECT();
                rect.left = rect.right = rect.top = rect.bottom = 0;

                CreateParams cp = CreateParams;

                AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);
                clientWidth = width - (rect.right - rect.left);
                clientHeight = height - (rect.bottom - rect.top);
            }


            // Set up for async operations on this thread.
            if (autoInstallSyncContext) {
                WindowsFormsSynchronizationContext.InstallIfNeeded();
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Control1"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Control'/> class.</para>
        /// </devdoc>
        public Control( string text ) : this( (Control) null, text ) {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Control2"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Control'/> class.</para>
        /// </devdoc>
        public Control( string text, int left, int top, int width, int height ) :
                    this( (Control) null, text, left, top, width, height ) {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Control3"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Control'/> class.</para>
        /// </devdoc>
        public Control( Control parent, string text ) : this() {
            this.Parent = parent;
            this.Text = text;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Control4"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.Control'/> class.</para>
        /// </devdoc>
        public Control( Control parent, string text, int left, int top, int width, int height ) : this( parent, text ) {
            this.Location = new Point( left, top );
            this.Size = new Size( width, height );
        }

        /// <summary>
        /// gets or sets control Dpi awareness context value.
        /// </summary>
        internal DpiAwarenessContext DpiAwarenessContext {
            get {
                return window.DpiAwarenessContext;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibilityObject"]/*' />
        /// <devdoc>
        ///      The Accessibility Object for this Control
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlAccessibilityObjectDescr))
        ]
        public AccessibleObject AccessibilityObject {
            get {
                AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(PropAccessibility);
                if (accessibleObject == null) {
                    accessibleObject = CreateAccessibilityInstance();
                    // this is a security check. we want to enforce that we only return
                    // ControlAccessibleObject and not some other derived class
                    if(!(accessibleObject is ControlAccessibleObject)) {
                        Debug.Fail("Accessible objects for controls must be derived from ControlAccessibleObject.");
                        return null;
                    }
                    Properties.SetObject(PropAccessibility, accessibleObject);
                }

                Debug.Assert(accessibleObject != null, "Failed to create accessibility object");
                return accessibleObject;
            }
        }

        /// <devdoc>
        ///      Private accessibility object for control, used to wrap the object that
        ///      OLEACC.DLL creates to represent the control's non-client (NC) region.
        /// </devdoc>
        private AccessibleObject NcAccessibilityObject {
            get {
                AccessibleObject ncAccessibleObject = (AccessibleObject)Properties.GetObject(PropNcAccessibility);
                if (ncAccessibleObject == null) {
                    ncAccessibleObject = new ControlAccessibleObject(this, NativeMethods.OBJID_WINDOW);
                    Properties.SetObject(PropNcAccessibility, ncAccessibleObject);
                }

                Debug.Assert(ncAccessibleObject != null, "Failed to create NON-CLIENT accessibility object");
                return ncAccessibleObject;
            }
        }

        /// <devdoc>
        ///     Returns a specific AccessibleObject associated with this
        ///     control, based on standard "accessibile object id".
        /// </devdoc>
        /// <internalonly/>
        private AccessibleObject GetAccessibilityObject(int accObjId) {
            AccessibleObject accessibleObject;

            switch (accObjId) {
                case NativeMethods.OBJID_CLIENT:
                    accessibleObject = this.AccessibilityObject;
                    break;
                case NativeMethods.OBJID_WINDOW:
                    accessibleObject = this.NcAccessibilityObject;
                    break;
                default:
                    if (accObjId > 0) {
                        accessibleObject = this.GetAccessibilityObjectById(accObjId);
                    } else {
                        accessibleObject = null;
                    }
                    break;
            }

            return accessibleObject;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetAccessibilityObjectById"]/*' />
        /// <devdoc>
        ///      Returns a specific AccessibleObbject associated w/ the objectID
        /// </devdoc>
        protected virtual AccessibleObject GetAccessibilityObjectById(int objectId) {
            if (AccessibilityImprovements.Level3 && this is IAutomationLiveRegion) {
                return this.AccessibilityObject;
            }

            return null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibleDefaultActionDescription"]/*' />
        /// <devdoc>
        ///      The default action description of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlAccessibleDefaultActionDescr))
        ]
        public string AccessibleDefaultActionDescription {
            get {
                return (string)Properties.GetObject(PropAccessibleDefaultActionDescription);
            }
            set {
                Properties.SetObject(PropAccessibleDefaultActionDescription, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibleDescription"]/*' />
        /// <devdoc>
        ///      The accessible description of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ControlAccessibleDescriptionDescr))
        ]
        public string AccessibleDescription {
            get {
                return (string)Properties.GetObject(PropAccessibleDescription);
            }
            set {
                Properties.SetObject(PropAccessibleDescription, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibleName"]/*' />
        /// <devdoc>
        ///      The accessible name of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ControlAccessibleNameDescr))
        ]
        public string AccessibleName {
            get {
                return (string)Properties.GetObject(PropAccessibleName);
            }

            set {
                Properties.SetObject(PropAccessibleName, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibleRole"]/*' />
        /// <devdoc>
        ///      The accessible role of the control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAccessibility)),
        DefaultValue(AccessibleRole.Default),
        SRDescription(nameof(SR.ControlAccessibleRoleDescr))
        ]
        public AccessibleRole AccessibleRole {

            get {
                bool found;
                int role = Properties.GetInteger(PropAccessibleRole, out found);
                if (found) {
                    return (AccessibleRole)role;
                }
                else {
                    return AccessibleRole.Default;
                }
            }

            set {
                //valid values are -1 to 0x40
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AccessibleRole.Default, (int)AccessibleRole.OutlineButton)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AccessibleRole));
                }
                Properties.SetInteger(PropAccessibleRole, (int)value);
            }
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private Color ActiveXAmbientBackColor {
            get {
                return ActiveXInstance.AmbientBackColor;
            }
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private Color ActiveXAmbientForeColor {
            get {
                return ActiveXInstance.AmbientForeColor;
            }
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private Font ActiveXAmbientFont {
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get {
                return ActiveXInstance.AmbientFont;
            }
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private bool ActiveXEventsFrozen {
            get {
                return ActiveXInstance.EventsFrozen;
            }
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private IntPtr ActiveXHWNDParent {
            get {
                return ActiveXInstance.HWNDParent;
            }
        }

        /// <devdoc>
        ///      Retrieves the ActiveX control implementation for
        ///      this control.  This will demand create the implementation
        ///      if it does not already exist.
        /// </devdoc>
        private ActiveXImpl ActiveXInstance {
            get {
                ActiveXImpl activeXImpl = (ActiveXImpl)Properties.GetObject(PropActiveXImpl);
                if (activeXImpl == null) {

                    // Don't allow top level objects to be hosted
                    // as activeX controls.
                    //
                    if (GetState(STATE_TOPLEVEL)) {
                        throw new NotSupportedException(SR.AXTopLevelSource);
                    }

                    activeXImpl = new ActiveXImpl(this);
                    
                    // PERF: IsActiveX is called quite a bit - checked everywhere from sizing to event raising.  Using a state
                    // bit to track PropActiveXImpl instead of fetching from the property store.
                    SetState2(STATE2_ISACTIVEX, true);
                    Properties.SetObject(PropActiveXImpl, activeXImpl);
                }

                return activeXImpl;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AllowDrop"]/*' />
        /// <devdoc>
        ///     The AllowDrop property. If AllowDrop is set to true then
        ///     this control will allow drag and drop operations and events to be used.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlAllowDropDescr))
        ]
        public virtual bool AllowDrop {
            get {
                return GetState(STATE_ALLOWDROP);
            }

            set {
                if (GetState(STATE_ALLOWDROP) != value) {
                    // Since we won't call SetAcceptDrops without a handle,
                    // we do the security demand here. Without this demand
                    // we are still safe, but you get the exception at an
                    // odd time. This gives a better experience.
                    //
                    if (value && !IsHandleCreated) {
                        IntSecurity.ClipboardRead.Demand();
                    }

                    SetState(STATE_ALLOWDROP, value);

                    if (IsHandleCreated) {
                        try {
                            SetAcceptDrops(value);
                        }
                        catch {
                            // If there is an error, back out the AllowDrop state...
                            //
                            SetState(STATE_ALLOWDROP, !value);
                            throw;
                        }
                    }
                }
            }
        }

        // Queries the Site for AmbientProperties.  May return null.
        // Do not confuse with inheritedProperties -- the service is turned to
        // after we've exhausted inheritedProperties.
        private AmbientProperties AmbientPropertiesService {
            get {
                bool contains;
                AmbientProperties props = (AmbientProperties)Properties.GetObject(PropAmbientPropertiesService, out contains);
                if (!contains) {
                    if (Site != null) {
                        props = (AmbientProperties)Site.GetService(typeof(AmbientProperties));
                    }
                    else {
                        props = (AmbientProperties)GetService(typeof(AmbientProperties));
                    }

                    if (props != null) {
                        Properties.SetObject(PropAmbientPropertiesService, props);
                    }
                }
                return props;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Anchor"]/*' />
        /// <devdoc>
        ///     The current value of the anchor property. The anchor property
        ///     determines which edges of the control are anchored to the container's
        ///     edges.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        DefaultValue(CommonProperties.DefaultAnchor),
        SRDescription(nameof(SR.ControlAnchorDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public virtual AnchorStyles Anchor {
            get {
                return DefaultLayout.GetAnchor(this);
            }
            set {
                DefaultLayout.SetAnchor(ParentInternal, this, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AutoSize"]/*' />
        [SRCategory(nameof(SR.CatLayout))]
        [RefreshProperties(RefreshProperties.All)]
        [Localizable(true)]
        [DefaultValue(CommonProperties.DefaultAutoSize)]
        [SRDescription(nameof(SR.ControlAutoSizeDescr))]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool AutoSize {
            get { return CommonProperties.GetAutoSize(this); }
            set {
                if(value != AutoSize) {
                    CommonProperties.SetAutoSize(this, value);
                    if(ParentInternal != null) {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if(value && ParentInternal.LayoutEngine == DefaultLayout.Instance) {
                            ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                    }

                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AutoSizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler AutoSizeChanged {
            add {
                Events.AddHandler(EventAutoSizeChanged, value);
            }
            remove {
                Events.RemoveHandler(EventAutoSizeChanged, value);
            }
        }

        /// <devdoc>
        /// Controls the location of where this control is scrolled to in ScrollableControl.ScrollControlIntoView.
        /// Default is the upper left hand corner of the control.
        /// </devdoc>
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced),
        DefaultValue(typeof(Point), "0, 0")
        ]
        public virtual Point AutoScrollOffset {
            get {
                if (Properties.ContainsObject(PropAutoScrollOffset)) {
                    return (Point)Properties.GetObject(PropAutoScrollOffset);
                }
                return Point.Empty;
            }
            set {
                if (AutoScrollOffset != value) {
                    Properties.SetObject(PropAutoScrollOffset, value);
                }
            }
        }

        protected void SetAutoSizeMode(AutoSizeMode mode) {
            CommonProperties.SetAutoSizeMode(this, mode);
        }

        protected AutoSizeMode GetAutoSizeMode() {
            return CommonProperties.GetAutoSizeMode(this);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.LayoutEngine"]/*' />
        // Public because this is interesting for ControlDesigners.
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual LayoutEngine LayoutEngine {
            get { return DefaultLayout.Instance; }
        }

        /// <devdoc>
        ///     The GDI brush for our background color.
        ///     Whidbey Note: Made this internal, since we need to use this in ButtonStandardAdapter. Also, renamed
        ///                   from BackBrush to BackColorBrush due to a naming conflict with DataGrid's BackBrush.
        /// </devdoc>
        internal IntPtr BackColorBrush {
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get {
                object customBackBrush = Properties.GetObject(PropBackBrush);
                if (customBackBrush != null) {

                    // We already have a valid brush.  Unbox, and return.
                    //
                    return (IntPtr)customBackBrush;
                }

                if (!Properties.ContainsObject(PropBackColor)) {

                    // No custom back color.  See if we can get to our parent.
                    // The color check here is to account for parents and children who
                    // override the BackColor property.
                    //
                    if (parent != null && parent.BackColor == BackColor) {
                        return parent.BackColorBrush;
                    }
                }

                // No parent, or we have a custom back color.  Either way, we need to
                // create our own.
                //
                Color color = BackColor;
                IntPtr backBrush;

                if (ColorTranslator.ToOle(color) < 0) {
                    backBrush = SafeNativeMethods.GetSysColorBrush(ColorTranslator.ToOle(color) & 0xFF);
                    SetState(STATE_OWNCTLBRUSH, false);
                }
                else {
                    backBrush = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(color));
                    SetState(STATE_OWNCTLBRUSH, true);
                }

                Debug.Assert(backBrush != IntPtr.Zero, "Failed to create brushHandle");
                Properties.SetObject(PropBackBrush, backBrush);

                return backBrush;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BackColor"]/*' />
        /// <devdoc>
        ///     The background color of this control. This is an ambient property and
        ///     will always return a non-null value.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DispId(NativeMethods.ActiveX.DISPID_BACKCOLOR),
        SRDescription(nameof(SR.ControlBackColorDescr))
        ]
        public virtual Color BackColor {
            get {
                Color c = RawBackColor; // inheritedProperties.BackColor
                if (!c.IsEmpty) {
                    return c;
                }

                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties) {
                    c = p.BackColor;
                    if (IsValidBackColor(c))
                    {
                        return c;
                    }

                }

                if (IsActiveX) {
                    c = ActiveXAmbientBackColor;
                }

                if (c.IsEmpty) {
                    AmbientProperties ambient = AmbientPropertiesService;
                    if (ambient != null)
                        c = ambient.BackColor;
                }

                if (!c.IsEmpty && IsValidBackColor(c))
                    return c;
                else
                    return DefaultBackColor;
            }

            set {
                if (!value.Equals(Color.Empty) && !GetStyle(ControlStyles.SupportsTransparentBackColor) && value.A < 255)
                    throw new ArgumentException(SR.TransparentBackColorNotAllowed);

                Color c = BackColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropBackColor)) {
                    Properties.SetColor(PropBackColor, value);
                }

                if (!c.Equals(BackColor)) {
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BackColorChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBackColorChangedDescr))]
        public event EventHandler BackColorChanged {
            add {
                Events.AddHandler(EventBackColor, value);
            }
            remove {
                Events.RemoveHandler(EventBackColor, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BackgroundImage"]/*' />
        /// <devdoc>
        ///     The background image of the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.ControlBackgroundImageDescr))
        ]
        public virtual Image BackgroundImage {
            get {
                return (Image)Properties.GetObject(PropBackgroundImage);
            }
            set {
                if (BackgroundImage != value) {
                    Properties.SetObject(PropBackgroundImage, value);
                    OnBackgroundImageChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BackgroundImageChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBackgroundImageChangedDescr))]
        public event EventHandler BackgroundImageChanged {
            add {
                Events.AddHandler(EventBackgroundImage, value);
            }
            remove {
                Events.RemoveHandler(EventBackgroundImage, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BackgroundImageLayout"]/*' />
        /// <devdoc>
        ///     The BackgroundImageLayout of the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ImageLayout.Tile),
        Localizable(true),
        SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))
        ]
        public virtual ImageLayout BackgroundImageLayout {
            get {
                bool found = Properties.ContainsObject(PropBackgroundImageLayout);
                if (!found) {
                    return ImageLayout.Tile;
                }
                else {
                    return ((ImageLayout)Properties.GetObject(PropBackgroundImageLayout));
               }
            }
            set {
                if (BackgroundImageLayout != value) {
                    //valid values are 0x0 to 0x4
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ImageLayout.None, (int)ImageLayout.Zoom)){
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ImageLayout));
                    }
                    // Check if the value is either center, strech or zoom;
                    if (value == ImageLayout.Center || value == ImageLayout.Zoom || value == ImageLayout.Stretch) {                    
                        SetStyle(ControlStyles.ResizeRedraw, true);
                        // Only for images that support transparency.
                        if (ControlPaint.IsImageTransparent(BackgroundImage))
                        {
                            DoubleBuffered = true;    
                        }
                    }
                    Properties.SetObject(PropBackgroundImageLayout, value);
                    OnBackgroundImageLayoutChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BackgroundImageLayoutChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBackgroundImageLayoutChangedDescr))]
        public event EventHandler BackgroundImageLayoutChanged {
            add {
                Events.AddHandler(EventBackgroundImageLayout, value);
            }
            remove {
                Events.RemoveHandler(EventBackgroundImageLayout, value);
            }
        }

        // Set/reset by ContainerControl.AssignActiveControlInternal
        internal bool BecomingActiveControl
        {
            get
            {
                return GetState2(STATE2_BECOMINGACTIVECONTROL);
            }
            set
            {
                if (value != this.BecomingActiveControl)
                {                    
                    Application.ThreadContext.FromCurrent().ActivatingControl = (value) ? this : null;
                    SetState2(STATE2_BECOMINGACTIVECONTROL, value);
                }
            }
        }

        private bool ShouldSerializeAccessibleName() {
            string accName = this.AccessibleName;
            return accName != null && accName.Length > 0;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetBindings"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBindings() {
            ControlBindingsCollection bindings = (ControlBindingsCollection)Properties.GetObject(PropBindings);
            if (bindings != null) {
                bindings.Clear();
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BindingContextInternal"]/*' />
        /// <devdoc>
        ///    BindingContextInternal provides a mechanism so that controls like SplitContainer that inherit from the
        ///    ContainerControl can bypass the "containerControls" bindingContext property and do what the other simple controls
        ///    do.
        /// </devdoc>
        internal BindingContext BindingContextInternal
        {
            get
            {
                // See if we have locally overridden the binding manager.
                //
                BindingContext context = (BindingContext) Properties.GetObject(PropBindingManager);
                if (context != null) {
                    return context;
                }

                // Otherwise, see if the parent has one for us.
                //
                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties) {
                    return p.BindingContext;
                }

                // Otherwise, we have no binding manager available.
                //
                return null;
            }
            set
            {
                BindingContext oldContext = (BindingContext) Properties.GetObject(PropBindingManager);
                BindingContext newContext = value;

                if (oldContext != newContext) {
                    Properties.SetObject(PropBindingManager, newContext);

                    // the property change will wire up the bindings.
                    //
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BindingContext"]/*' />
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlBindingContextDescr))
        ]
        public virtual BindingContext BindingContext {
            get {
                return BindingContextInternal;
            }
            set {
                BindingContextInternal = value;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BindingContextChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBindingContextChangedDescr))]
        public event EventHandler BindingContextChanged {
            add {
                Events.AddHandler(EventBindingContext, value);
            }
            remove {
                Events.RemoveHandler(EventBindingContext, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Bottom"]/*' />
        /// <devdoc>
        ///    <para>The bottom coordinate of this control.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlBottomDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public int Bottom {
            get {
                return y + height;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Bounds"]/*' />
        /// <devdoc>
        ///     The bounds of this control. This is the window coordinates of the
        ///     control in parent client coordinates.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlBoundsDescr)),
        SRCategory(nameof(SR.CatLayout))
        ]
        public Rectangle Bounds {
            get {
                return new Rectangle(x, y, width, height);
            }

            set {
                SetBounds(value.X, value.Y, value.Width, value.Height, BoundsSpecified.All);
            }
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal virtual bool CanAccessProperties {
            get {
                return true;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CanFocus"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control can receive focus. This
        ///       property is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatFocus)),
        SRDescription(nameof(SR.ControlCanFocusDescr))
        ]
        public bool CanFocus {
            get {
                if (!IsHandleCreated) {
                    return false;
                }
                bool visible = SafeNativeMethods.IsWindowVisible(new HandleRef(window, Handle));
                bool enabled = SafeNativeMethods.IsWindowEnabled(new HandleRef(window, Handle));
                return (visible && enabled);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CanRaiseEvents"]/*' />
        /// <devdoc>
        ///     Determines if events can be fired on the control.  If this control is being
        ///     hosted as an ActiveX control, this property will return false if the ActiveX
        ///     control has its events frozen.
        /// </devdoc>
        protected override bool CanRaiseEvents {
            get {
                if (IsActiveX) {
                    return !ActiveXEventsFrozen;
                }

                return true;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CanSelect"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates whether the control can be selected. This property
        ///       is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatFocus)),
        SRDescription(nameof(SR.ControlCanSelectDescr))
        ]
        public bool CanSelect {
            // We implement this to allow only AxHost to override canSelectCore, but still
            // expose the method publicly
            //
            get {
                return CanSelectCore();
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Capture"]/*' />
        /// <devdoc>
        ///    <para> Indicates whether the control has captured the mouse.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatFocus)),
        SRDescription(nameof(SR.ControlCaptureDescr))
        ]
        public bool Capture {
            get {
                return CaptureInternal;
            }

            set {
                if (value) {
                    Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "GetCapture Demanded");
                    IntSecurity.GetCapture.Demand();
                }
                CaptureInternal = value;
            }
        }
                
        internal bool CaptureInternal {
            get {
                return IsHandleCreated && UnsafeNativeMethods.GetCapture() == Handle;
            }
            set {
                if (CaptureInternal != value) {
                    if (value) {
                        UnsafeNativeMethods.SetCapture(new HandleRef(this, Handle));
                    }
                    else {
                        SafeNativeMethods.ReleaseCapture();
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CausesValidation"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Indicates whether entering the control causes validation on the controls requiring validation.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatFocus)),
        DefaultValue(true),
        SRDescription(nameof(SR.ControlCausesValidationDescr))
        ]
        public bool CausesValidation {
            get {
                return GetState(STATE_CAUSESVALIDATION);
            }
            set {
                if (value != this.CausesValidation) {
                    SetState(STATE_CAUSESVALIDATION, value);
                    OnCausesValidationChanged(EventArgs.Empty);
                }
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CausesValidationChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnCausesValidationChangedDescr))]
        public event EventHandler CausesValidationChanged {
            add {
                Events.AddHandler(EventCausesValidation, value);
            }
            remove {
                Events.RemoveHandler(EventCausesValidation, value);
            }
        }


        /// This is for perf. Turn this property on to temporarily enable text caching.  This is good for
        /// operations such as layout or painting where we don't expect the text to change (we will update the
        /// cache if it does) but prevents us from sending a ton of messages turing layout.  See the PaintWithErrorHandling
        /// function.
        ///
        internal bool CacheTextInternal {
            get {

                // check if we're caching text.
                //
                bool found;
                int cacheTextCounter = Properties.GetInteger(PropCacheTextCount, out found);

                return cacheTextCounter > 0 || GetStyle(ControlStyles.CacheText);
            }
            set {

                // if this control always cachest text or the handle hasn't been created,
                // just bail.
                //
                if (GetStyle(ControlStyles.CacheText) || !IsHandleCreated) {
                    return;
                }

                // otherwise, get the state and update the cache if necessary.
                //
                bool found;
                int cacheTextCounter = Properties.GetInteger(PropCacheTextCount, out found);                
                
                if (value) {
                    if (cacheTextCounter == 0) {
                        Properties.SetObject(PropCacheTextField, text);
                        if (text == null)  {
                            text = WindowText;
                        }                        
                    }
                    cacheTextCounter++;
                }
                else {
                    cacheTextCounter--;
                    if (cacheTextCounter == 0) {
                        text = (string)Properties.GetObject(PropCacheTextField, out found);
                    }
                }                
                Properties.SetInteger(PropCacheTextCount, cacheTextCounter);
            }                
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CheckForIllegalCrossThreadCalls"]/*' />
        [ Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
          SRDescription(nameof(SR.ControlCheckForIllegalCrossThreadCalls)),
          DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        ]
        public static bool CheckForIllegalCrossThreadCalls {
            get { return checkForIllegalCrossThreadCalls; }
            set { checkForIllegalCrossThreadCalls = value; }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ClientRectangle"]/*' />
        /// <devdoc>
        ///     The client rect of the control.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRCategory(nameof(SR.CatLayout)),
        SRDescription(nameof(SR.ControlClientRectangleDescr))
        ]
        public Rectangle ClientRectangle {
            get {
                return new Rectangle(0, 0, clientWidth, clientHeight);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ClientSize"]/*' />
        /// <devdoc>
        ///     The size of the clientRect.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlClientSizeDescr))
        ]
        public Size ClientSize {
            get {
                return new Size(clientWidth, clientHeight);
            }

            set {
                SetClientSizeCore(value.Width, value.Height);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ClientSizeChanged"]/*' />
        /// <devdoc>
        ///    Fired when ClientSize changes.
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnClientSizeChangedDescr))]
        public event EventHandler ClientSizeChanged {
            add {
                Events.AddHandler(EventClientSize, value);
            }
            remove {
                Events.RemoveHandler(EventClientSize, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CompanyName"]/*' />
        /// <devdoc>
        ///     Retrieves the company name of this specific component.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DescriptionAttribute(nameof(SR.ControlCompanyNameDescr))
        ]
        public string CompanyName {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                return VersionInfo.CompanyName;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ContainsFocus"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control or one of its children currently has the system
        ///       focus. This property is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlContainsFocusDescr))
        ]
        public bool ContainsFocus {
            get {
                if (!IsHandleCreated) {
                    return false;
                }

                IntPtr focusHwnd = UnsafeNativeMethods.GetFocus();

                if (focusHwnd == IntPtr.Zero) {
                    return false;
                }       

                if (focusHwnd == this.Handle) {
                    return true;
                }

                if (UnsafeNativeMethods.IsChild(new HandleRef(this, this.Handle), new HandleRef(this, focusHwnd))) {
                    return true;
                }

                return false;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ContextMenu"]/*' />
        /// <devdoc>
        ///     The contextMenu associated with this control. The contextMenu
        ///     will be shown when the user right clicks the mouse on the control.
        ///
        ///     Whidbey: ContextMenu is browsable false.  In all cases where both a context menu
        ///     and a context menu strip are assigned, context menu will be shown instead of context menu strip.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ControlContextMenuDescr)),
        Browsable(false)
        ]
        public virtual ContextMenu ContextMenu {
            get {
                return (ContextMenu)Properties.GetObject(PropContextMenu);
            }
            set {
                ContextMenu oldValue = (ContextMenu)Properties.GetObject(PropContextMenu);

                if (oldValue != value) {
                    EventHandler disposedHandler = new EventHandler(DetachContextMenu);

                    if (oldValue != null) {
                        oldValue.Disposed -= disposedHandler;
                    }

                    Properties.SetObject(PropContextMenu, value);

                    if (value != null) {
                        value.Disposed += disposedHandler;
                    }

                    OnContextMenuChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ContextMenuChanged"]/*' />
        [
        SRCategory(nameof(SR.CatPropertyChanged)),
        SRDescription(nameof(SR.ControlOnContextMenuChangedDescr)),
        Browsable(false)
        ]
        public event EventHandler ContextMenuChanged {
            add {
                Events.AddHandler(EventContextMenu, value);
            }
            remove {
                Events.RemoveHandler(EventContextMenu, value);
            }
        }


       /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ContextMenu"]/*' />
       /// <devdoc>
       ///     The contextMenuStrip associated with this control. The contextMenuStrip
       ///     will be shown when the user right clicks the mouse on the control.
       ///     Note: if a context menu is also assigned, it will take precidence over this property.
       /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ControlContextMenuDescr))
        ]
        public virtual ContextMenuStrip ContextMenuStrip {
            get {
                 return (ContextMenuStrip)Properties.GetObject(PropContextMenuStrip);
             }
             set {
                 ContextMenuStrip oldValue = Properties.GetObject(PropContextMenuStrip) as ContextMenuStrip;

                 if (oldValue != value) {
                     EventHandler disposedHandler = new EventHandler(DetachContextMenuStrip);

                     if (oldValue != null) {
                         oldValue.Disposed -= disposedHandler;
                     }

                     Properties.SetObject(PropContextMenuStrip, value);

                     if (value != null) {
                         value.Disposed += disposedHandler;
                     }

                     OnContextMenuStripChanged(EventArgs.Empty);
                 }
             }

        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ContextMenuStripChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlContextMenuStripChangedDescr))]
        public event EventHandler ContextMenuStripChanged {
            add {
                Events.AddHandler(EventContextMenuStrip, value);
            }
            remove {
                Events.RemoveHandler(EventContextMenuStrip, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Controls"]/*' />
        /// <devdoc>
        ///     Collection of child controls.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRDescription(nameof(SR.ControlControlsDescr))
        ]
        public ControlCollection Controls {
            get {
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                if (controlsCollection == null) {
                    controlsCollection = CreateControlsInstance();
                    Properties.SetObject(PropControlsCollection, controlsCollection);
                }
                return controlsCollection;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Created"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control has been created. This property is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlCreatedDescr))
        ]
        public bool Created {
            get {
                return(state & STATE_CREATED) != 0;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CreateParams"]/*' />
        /// <devdoc>
        ///     Returns the CreateParams used to create the handle for this control.
        ///     Inheriting classes should call base.CreateParams in the manor
        ///     below:
        /// </devdoc>
        protected virtual CreateParams CreateParams {
            [
                SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
                SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
            ]
            get {

                // CLR4.0 or later, comctl32.dll needs to be loaded explicitly.
                if (needToLoadComCtl) {
                    if ((UnsafeNativeMethods.GetModuleHandle(ExternDll.Comctl32) != IntPtr.Zero)
                        || (UnsafeNativeMethods.LoadLibraryFromSystemPathIfAvailable(ExternDll.Comctl32) != IntPtr.Zero)) {
                        needToLoadComCtl = false;
                    } else {
                        int lastWin32Error = Marshal.GetLastWin32Error();
                        throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, ExternDll.Comctl32));
                    }
                }

                // In a typical control this is accessed ten times to create and show a control.
                // It is a net memory savings, then, to maintain a copy on control.
                //
                if (createParams == null) {
                    createParams = new CreateParams();
                }

                CreateParams cp = createParams;
                cp.Style = 0;
                cp.ExStyle = 0;
                cp.ClassStyle = 0;
                cp.Caption = text;

                cp.X = x;
                cp.Y = y;
                cp.Width = width;
                cp.Height = height;

                cp.Style = NativeMethods.WS_CLIPCHILDREN;
                if (GetStyle(ControlStyles.ContainerControl)) {
                    cp.ExStyle |= NativeMethods.WS_EX_CONTROLPARENT;
                }
                cp.ClassStyle = NativeMethods.CS_DBLCLKS;

                if ((state & STATE_TOPLEVEL) == 0) {
                    // When the window is actually created, we will parent WS_CHILD windows to the
                    // parking form if cp.parent == 0.
                    //
                    cp.Parent = parent == null ? IntPtr.Zero : parent.InternalHandle;
                    cp.Style |= NativeMethods.WS_CHILD | NativeMethods.WS_CLIPSIBLINGS;
                }
                else {
                    cp.Parent = IntPtr.Zero;
                }

                if ((state & STATE_TABSTOP) != 0) cp.Style |= NativeMethods.WS_TABSTOP;
                if ((state & STATE_VISIBLE) != 0) cp.Style |= NativeMethods.WS_VISIBLE;

                // Unlike Visible, Windows doesn't correctly inherit disabledness from its parent -- an enabled child
                // of a disabled parent will look enabled but not get mouse events
                if (!Enabled) cp.Style |= NativeMethods.WS_DISABLED;

                // If we are being hosted as an Ax control, try to prevent the parking window
                // from being created by pre-filling the window handle here.
                //
                if (cp.Parent == IntPtr.Zero && IsActiveX) {
                    cp.Parent = ActiveXHWNDParent;
                }

                // Set Rtl bits
                if (RightToLeft == RightToLeft.Yes) {
                    cp.ExStyle |= NativeMethods.WS_EX_RTLREADING;
                    cp.ExStyle |= NativeMethods.WS_EX_RIGHT;
                    cp.ExStyle |= NativeMethods.WS_EX_LEFTSCROLLBAR;
                }

                return cp;
            }
        }

        internal virtual void NotifyValidationResult(object sender, CancelEventArgs ev) {
            this.ValidationCancelled = ev.Cancel ;
        }

        /// <devdoc>
        ///     Helper method...
        ///
        ///     Triggers validation on the active control, and returns bool indicating whether that control was valid.
        ///
        ///     The correct way to do this is to find the common ancestor of the active control and this control,
        ///     then request validation to be performed by that common container control.
        ///
        ///     Used by controls that don't participate in the normal enter/leave/validation process, but which
        ///     want to force form-level validation to occur before they attempt some important action.
        /// </devdoc>
        internal bool ValidateActiveControl(out bool validatedControlAllowsFocusChange) {
            bool valid = true;
            validatedControlAllowsFocusChange = false;
            IContainerControl c = GetContainerControlInternal();
            if (c != null && this.CausesValidation) {
                ContainerControl container = c as ContainerControl;
                if (container != null) {
                    while (container.ActiveControl == null) {
                        ContainerControl cc;
                        Control parent = container.ParentInternal;
                        if (parent != null) {
                            cc = parent.GetContainerControlInternal() as ContainerControl;
                            if (cc != null) {
                                container = cc;
                            }
                            else {
                                break;
                            }
                        }
                        else {
                            break;
                        }
                    }

                    valid = container.ValidateInternal(true, out validatedControlAllowsFocusChange);
                }
            }

            return valid;
        }

        internal bool ValidationCancelled {
            set {
                SetState(STATE_VALIDATIONCANCELLED, value);
            }
            get {
                if (GetState(STATE_VALIDATIONCANCELLED)) {
                    return true;
                }
                else {
                    Control parent = this.ParentInternal;
                    if (parent != null) {
                        return parent.ValidationCancelled;
                    }

                    return false;
                }
            }
        }

        /// <devdoc>
        ///     returns bool indicating whether the Top MDI Window is closing.
        ///     This property is set in the MDI children in WmClose method in form.cs when the top window is closing.
        ///     This property will be used in ActiveControl to determine if we want to skip set focus and window handle re-creation for the control.
        /// </devdoc>
        /// <internalonly/>
        internal bool IsTopMdiWindowClosing {
            set {
                SetState2(STATE2_TOPMDIWINDOWCLOSING, value);
            }
            get {
                return GetState2(STATE2_TOPMDIWINDOWCLOSING);
            }
        }

        /// <devdoc>
        ///     returns bool indicating whether the control is currently being scaled.
        ///     This property is set in ScaleControl method to allow method being called to condition code that should not run for scaling.
        /// </devdoc>
        /// <internalonly/>
        internal bool IsCurrentlyBeingScaled {
            private set {
                SetState2(STATE2_CURRENTLYBEINGSCALED, value);
            }
            get {
                return GetState2(STATE2_CURRENTLYBEINGSCALED);
            }
        }

        /// <devdoc>
        ///     Retrieves the Win32 thread ID of the thread that created the
        ///     handle for this control.  If the control's handle hasn't been
        ///     created yet, this method will return the current thread's ID.
        /// </devdoc>
        /// <internalonly/>
        internal int CreateThreadId {
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            get {
                if (IsHandleCreated) {
                    int pid;
                    return SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, Handle), out pid);
                }
                else {
                    return SafeNativeMethods.GetCurrentThreadId();
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Cursor"]/*' />
        /// <devdoc>
        ///     Retrieves the cursor that will be displayed when the mouse is over this
        ///     control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ControlCursorDescr)),
        AmbientValue(null)
        ]
        public virtual Cursor Cursor {
            get {
                if (GetState(STATE_USEWAITCURSOR))
                {
                    return Cursors.WaitCursor;
                }

                Cursor cursor = (Cursor)Properties.GetObject(PropCursor);
                if (cursor != null) {
                    return cursor;
                }

                // We only do ambients for things with "Cursors.Default"
                // as their default.
                //
                Cursor localDefault = DefaultCursor;
                if (localDefault != Cursors.Default) {
                    return localDefault;
                }

                Control p = ParentInternal;
                if (p != null) {
                    return p.Cursor;
                }

                AmbientProperties ambient = AmbientPropertiesService;
                if (ambient != null && ambient.Cursor != null)
                    return ambient.Cursor;

                return localDefault;
            }
            set {

                Cursor localCursor = (Cursor)Properties.GetObject(PropCursor);
                Cursor resolvedCursor = Cursor;
                if (localCursor != value) {
                    Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ModifyCursor Demanded");
                    IntSecurity.ModifyCursor.Demand();

                    Properties.SetObject(PropCursor, value);
                }

                // Other things can change the cursor... we
                // really want to force the correct cursor always...
                //
                if (IsHandleCreated) {
                    // We want to instantly change the cursor if the mouse is within our bounds.
                    // This includes the case where the mouse is over one of our children
                    NativeMethods.POINT p = new NativeMethods.POINT();
                    NativeMethods.RECT r = new NativeMethods.RECT();
                    UnsafeNativeMethods.GetCursorPos(p);
                    UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);

                    if ((r.left <= p.x && p.x < r.right && r.top <= p.y && p.y < r.bottom) || UnsafeNativeMethods.GetCapture() == Handle)
                        SendMessage(NativeMethods.WM_SETCURSOR, Handle, (IntPtr)NativeMethods.HTCLIENT);
                }

                if (!resolvedCursor.Equals(value)) {
                    OnCursorChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CursorChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnCursorChangedDescr))]
        public event EventHandler CursorChanged {
            add {
                Events.AddHandler(EventCursor, value);
            }
            remove {
                Events.RemoveHandler(EventCursor, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DataBindings"]/*' />
        /// <devdoc>
        ///     Retrieves the bindings for this control.
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        SRCategory(nameof(SR.CatData)),
        SRDescription(nameof(SR.ControlBindingsDescr)),
        RefreshProperties(RefreshProperties.All),
        ParenthesizePropertyName(true)
        ]
        public ControlBindingsCollection DataBindings
        {
            get
            {
                ControlBindingsCollection bindings = (ControlBindingsCollection)Properties.GetObject(PropBindings);
                if (bindings == null)
                {
                    bindings = new ControlBindingsCollection(this);
                    Properties.SetObject(PropBindings, bindings);
                }
                return bindings;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultBackColor"]/*' />
        /// <devdoc>
        ///     The default BackColor of a generic top-level Control.  Subclasses may have
        ///     different defaults.
        /// </devdoc>
        public static Color DefaultBackColor {
            get { return SystemColors.Control;}
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultCursor"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default cursor for their control.
        ///     This is more efficient than setting the cursor in the control's constructor,
        ///     and gives automatic support for ShouldSerialize and Reset in the designer.
        /// </devdoc>
        protected virtual Cursor DefaultCursor {
            get {
                return Cursors.Default;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultFont"]/*' />
        /// <devdoc>
        ///     The default Font of a generic top-level Control.  Subclasses may have
        ///     different defaults.
        /// </devdoc>
        public static Font DefaultFont {
            [ResourceExposure(ResourceScope.None)]
            [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
            get {
                if (defaultFont == null) {
                    defaultFont = SystemFonts.DefaultFont;
                    Debug.Assert(defaultFont != null, "defaultFont wasn't set!");
                }

                return defaultFont;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultForeColor"]/*' />
        /// <devdoc>
        ///     The default ForeColor of a generic top-level Control.  Subclasses may have
        ///     different defaults.
        /// </devdoc>
        public static Color DefaultForeColor {
            get { return SystemColors.ControlText;}
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultMargin"]/*' />
        protected virtual Padding DefaultMargin {
            get { return CommonProperties.DefaultMargin; }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultMaximumSize"]/*' />
        protected virtual Size DefaultMaximumSize {
            get { return CommonProperties.DefaultMaximumSize; }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultMinimumSize"]/*' />
        protected virtual Size DefaultMinimumSize {
            get { return CommonProperties.DefaultMinimumSize; }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultPadding"]/*' />
        protected virtual Padding DefaultPadding {
            get { return Padding.Empty; }
        }

        private RightToLeft DefaultRightToLeft {
            get { return RightToLeft.No; }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected virtual Size DefaultSize {
            get { return Size.Empty; }
        }

        private void DetachContextMenu(object sender, EventArgs e) {
            ContextMenu = null;
        }


        private void DetachContextMenuStrip(object sender, EventArgs e) {
            ContextMenuStrip = null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DeviceDpi"]/*' />
        /// <devdoc>
        ///  DPI value either for the primary screen or for the monitor where the top-level parent is displayed when
        ///  EnableDpiChangedMessageHandling option is on and the application is per-monitor V2 DPI-aware (rs2+)
        /// </devdoc>
        [
            Browsable(false), // don't show in property browser
            EditorBrowsable(EditorBrowsableState.Always), // do show in the intellisense
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden) // do not serialize
        ]
        public int DeviceDpi {
            get {
                if (DpiHelper.IsPerMonitorV2Awareness) {
                    // deviceDpi may change in WmDpiChangedBeforeParent in PmV2 scenarios, so we can't cache statically.
                    return deviceDpi;
                }
                return DpiHelper.DeviceDpi;
            }
        }

        // The color to use when drawing disabled text.  Normally we use BackColor,
        // but that obviously won't work if we're transparent.
        internal Color DisabledColor {
            get {
                Color color = BackColor;
                if (color.A == 0) {
                    Control control = ParentInternal;
                    while (color.A == 0) {
                        if (control == null) {
                            // Don't know what to do, this seems good as anything
                            color = SystemColors.Control;
                            break;
                        }
                        color = control.BackColor;
                        control = control.ParentInternal;
                    }
                }
                return color;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DisplayRectangle"]/*' />
        /// <devdoc>
        ///     Returns the client rect of the display area of the control.
        ///     For the base control class, this is identical to getClientRect.
        ///     However, inheriting controls may want to change this if their client
        ///     area differs from their display area.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlDisplayRectangleDescr))
        ]
        public virtual Rectangle DisplayRectangle {
            [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
            get {
                return new Rectangle(0, 0, clientWidth, clientHeight);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Disposed"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control has been disposed. This
        ///       property is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlDisposedDescr))
        ]
        public bool IsDisposed {
            get {
                return GetState(STATE_DISPOSED);
            }
        }

        /// <devdoc>
        ///     Disposes of the currently selected font handle (if cached).
        /// </devdoc>
        private void DisposeFontHandle() {
            if (Properties.ContainsObject(PropFontHandleWrapper)) {
                FontHandleWrapper fontHandle = Properties.GetObject(PropFontHandleWrapper) as FontHandleWrapper;
                if( fontHandle != null ){
                    fontHandle.Dispose();
                }
                Properties.SetObject(PropFontHandleWrapper, null);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Disposing"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control is in the process of being disposed. This
        ///       property is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlDisposingDescr))
        ]
        public bool Disposing {
            get {
                return GetState(STATE_DISPOSING);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Dock"]/*' />
        /// <devdoc>
        ///     The dock property. The dock property controls to which edge
        ///     of the container this control is docked to. For example, when docked to
        ///     the top of the container, the control will be displayed flush at the
        ///     top of the container, extending the length of the container.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(CommonProperties.DefaultDock),
        SRDescription(nameof(SR.ControlDockDescr))
        ]
        public virtual DockStyle Dock {
            get {
                return DefaultLayout.GetDock(this);
            }
            set {
                if(value != Dock) {
#if DEBUG
                    int dbgLayoutCheck = LayoutSuspendCount;
#endif
                    SuspendLayout();
                    try {
                        DefaultLayout.SetDock(this, value);
                        OnDockChanged(EventArgs.Empty);
                    } finally {
                        ResumeLayout();
                    }
#if DEBUG
                    AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DockChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnDockChangedDescr))]
        public event EventHandler DockChanged {
            add {
                Events.AddHandler(EventDock, value);
            }
            remove {
                Events.RemoveHandler(EventDock, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DoubleBuffered"]/*' />
        /// <devdoc>
        ///    <para>This will enable or disable double buffering.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ControlDoubleBufferedDescr))
        ]
        protected virtual bool DoubleBuffered {
            get {
                return GetStyle(ControlStyles.OptimizedDoubleBuffer);
            }
            set {
                if (value != DoubleBuffered) {
                    
                    if (value) {
                        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, value);
                    }
                    else {
                        SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
                    }
                }
            }
        }

        private bool DoubleBufferingEnabled {
            get {                                
#pragma warning disable 618            
                return GetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint);
#pragma warning restore 618
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Enabled"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control is currently enabled.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        DispId(NativeMethods.ActiveX.DISPID_ENABLED),
        SRDescription(nameof(SR.ControlEnabledDescr))
        ]
        public bool Enabled {
            get {
                // We are only enabled if our parent is enabled
                if (!GetState(STATE_ENABLED))
                    return false;
                else if (ParentInternal == null)
                    return true;
                else
                    return ParentInternal.Enabled;
            }

            set {
                bool oldValue = Enabled;
                SetState(STATE_ENABLED, value);

                if (oldValue != value) {
                    if (!value) {
                        SelectNextIfFocused();
                    }

                    OnEnabledChanged(EventArgs.Empty);
                }
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.EnabledChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is enabled.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnEnabledChangedDescr))]
        public event EventHandler EnabledChanged {
            add {
                Events.AddHandler(EventEnabled, value);
            }
            remove {
                Events.RemoveHandler(EventEnabled, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Focused"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control has focus. This property is read-only.</para>
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlFocusedDescr))
        ]
        public virtual bool Focused {
            get {
                return IsHandleCreated && UnsafeNativeMethods.GetFocus() == Handle;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Font"]/*' />
        /// <devdoc>
        ///     Retrieves the current font for this control. This will be the font used
        ///     by default for painting and text in the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DispId(NativeMethods.ActiveX.DISPID_FONT),
        AmbientValue(null),
        SRDescription(nameof(SR.ControlFontDescr))
        ]
        public virtual Font Font {
            [return : MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(ActiveXFontMarshaler))]
            [ResourceExposure(ResourceScope.None)]
            [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
            get {
                Font font = (Font)Properties.GetObject(PropFont);
                if (font != null) {
                    return font;
                }

                Font f = GetParentFont();
                if (f != null) {
                    return f;
                }

                if (IsActiveX) {
                    f = ActiveXAmbientFont;
                    if (f != null) {
                        return f;
                    }
                }

                AmbientProperties ambient = AmbientPropertiesService;
                if (ambient != null && ambient.Font != null) {
                    return ambient.Font;
                }

                return DefaultFont;
            }
            [param : MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef=typeof(ActiveXFontMarshaler))]
            set {
                Font local = (Font)Properties.GetObject(PropFont);
                Font resolved = Font;

                bool localChanged = false;
                if (value == null) {
                    if (local != null) {
                        localChanged = true;
                    }
                }
                else {
                    if (local == null) {
                        localChanged = true;
                    }
                    else {
                        localChanged = !value.Equals(local);
                    }
                }

                if (localChanged) {
                    // Store new local value
                    //
                    Properties.SetObject(PropFont, value);

                    // We only fire the Changed event if the "resolved" value
                    // changed, however we must update the font if the local
                    // value changed...
                    //
                    if (!resolved.Equals(value)) {
                        // Cleanup any font handle wrapper...
                        //
                        DisposeFontHandle();

                        if (Properties.ContainsInteger(PropFontHeight)) {
                            Properties.SetInteger(PropFontHeight, (value == null) ? -1 : value.Height);
                        }

                        // Font is an ambient property.  We need to layout our parent because Font may
                        // change our size.  We need to layout ourselves because our children may change
                        // size by inheriting the new value.
                        //
                        using(new LayoutTransaction(ParentInternal, this, PropertyNames.Font)) {
                            OnFontChanged(EventArgs.Empty);
                        }
                    }
                    else {
                        if (IsHandleCreated && !GetStyle(ControlStyles.UserPaint)) {
                            DisposeFontHandle();
                            SetWindowFont();
                        }
                    }
                }
            }
        }

        internal void ScaleFont(float factor) {
            Font local = (Font)Properties.GetObject(PropFont);
            Font resolved = Font;
            Font newFont = new Font(this.Font.FontFamily, this.Font.Size * factor, this.Font.Style);

            if  ((local == null) || !local.Equals(newFont)) {
                Properties.SetObject(PropFont, newFont);

                if (!resolved.Equals(newFont)) {
                    DisposeFontHandle();

                    if (Properties.ContainsInteger(PropFontHeight)) {
                        Properties.SetInteger(PropFontHeight, newFont.Height);
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.FontChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnFontChangedDescr))]
        public event EventHandler FontChanged {
            add {
                Events.AddHandler(EventFont, value);
            }
            remove {
                Events.RemoveHandler(EventFont, value);
            }
        }

        internal IntPtr FontHandle {
            get {
                Font font = (Font)Properties.GetObject(PropFont);

                if (font != null) {
                    FontHandleWrapper fontHandle = (FontHandleWrapper)Properties.GetObject(PropFontHandleWrapper);
                    if (fontHandle == null) {
                        fontHandle = new FontHandleWrapper(font);

                        Properties.SetObject(PropFontHandleWrapper, fontHandle);
                    }

                    return fontHandle.Handle;
                }

                if (parent != null) {
                    return parent.FontHandle;
                }

                AmbientProperties ambient = AmbientPropertiesService;

                if (ambient != null && ambient.Font != null) {

                    FontHandleWrapper fontHandle = null;

                    Font currentAmbient = (Font)Properties.GetObject(PropCurrentAmbientFont);

                    if (currentAmbient != null && currentAmbient == ambient.Font) {
                        fontHandle = (FontHandleWrapper)Properties.GetObject(PropFontHandleWrapper);
                    }
                    else {
                        Properties.SetObject(PropCurrentAmbientFont, ambient.Font);
                    }

                    if (fontHandle == null) {
                        font =  ambient.Font;
                        fontHandle = new FontHandleWrapper(font);

                        Properties.SetObject(PropFontHandleWrapper, fontHandle);
                    }

                    return fontHandle.Handle;
                }

                return GetDefaultFontHandleWrapper().Handle;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.FontHeight"]/*' />
        protected int FontHeight {
            get {
                bool found;
                int fontHeight = Properties.GetInteger(PropFontHeight, out found);
                if (found && fontHeight != -1) {
                    return fontHeight;
                }
                else {
                    Font font = (Font)Properties.GetObject(PropFont);
                    if (font != null) {
                        fontHeight = font.Height;
                        Properties.SetInteger(PropFontHeight, fontHeight);
                        return fontHeight;
                    }
                }

                //ask the parent if it has the font height
                int localFontHeight = -1;

                if (ParentInternal != null && ParentInternal.CanAccessProperties) {
                    localFontHeight = ParentInternal.FontHeight;
                }

                //if we still have a bad value, then get the actual font height
                if (localFontHeight == -1) {
                    localFontHeight = Font.Height;
                    Properties.SetInteger(PropFontHeight, localFontHeight);
                }

                return localFontHeight;
            }
            set {
                Properties.SetInteger(PropFontHeight, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ForeColor"]/*' />
        /// <devdoc>
        ///     The foreground color of the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DispId(NativeMethods.ActiveX.DISPID_FORECOLOR),
        SRDescription(nameof(SR.ControlForeColorDescr))
        ]
        public virtual Color ForeColor {
            get {
                Color color = Properties.GetColor(PropForeColor);
                if (!color.IsEmpty) {
                    return color;
                }

                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties) {
                    return p.ForeColor;
                }

                Color c = Color.Empty;

                if (IsActiveX) {
                    c = ActiveXAmbientForeColor;
                }

                if (c.IsEmpty) {
                    AmbientProperties ambient = AmbientPropertiesService;
                    if (ambient != null)
                        c = ambient.ForeColor;
                }

                if (!c.IsEmpty)
                    return c;
                else
                    return DefaultForeColor;
            }

            set {
                Color c = ForeColor;
                if (!value.IsEmpty || Properties.ContainsObject(PropForeColor)) {
                    Properties.SetColor(PropForeColor, value);
                }
                if (!c.Equals(ForeColor)) {
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ForeColorChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnForeColorChangedDescr))]
        public event EventHandler ForeColorChanged {
            add {
                Events.AddHandler(EventForeColor, value);
            }
            remove {
                Events.RemoveHandler(EventForeColor, value);
            }
        }

        private Font GetParentFont() {
            if (ParentInternal != null && ParentInternal.CanAccessProperties)
                return ParentInternal.Font;
            else
                return null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetPreferredSize"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
        public virtual Size GetPreferredSize(Size proposedSize) {
            Size prefSize;

            if (GetState(STATE_DISPOSING| STATE_DISPOSED)) {
                // if someone's asking when we're disposing just return what we last had.
                prefSize = CommonProperties.xGetPreferredSizeCache(this);
            }
            else {
                 // Switch Size.Empty to maximum possible values
                proposedSize = LayoutUtils.ConvertZeroToUnbounded(proposedSize);

                // Force proposedSize to be within the elements constraints.  (This applies
                // minimumSize, maximumSize, etc.)
                proposedSize = ApplySizeConstraints(proposedSize);
                if (GetState2(STATE2_USEPREFERREDSIZECACHE)) {
                    Size cachedSize = CommonProperties.xGetPreferredSizeCache(this);

                    // If the "default" preferred size is being requested, and we have a cached value for it, return it.
                    // 
                    if(!cachedSize.IsEmpty && (proposedSize == LayoutUtils.MaxSize)) {
       
#if DEBUG
#if DEBUG_PREFERREDSIZE
                            Size newPreferredSize = ApplySizeConstraints(GetPreferredSizeCore(proposedSize));
                            bool cacheHitCorrect = (cachedSize == newPreferredSize);
                            if (!cacheHitCorrect && !GetAnyDisposingInHierarchy()) {
                                Debug.Fail(
                                      "Cached PreferredSize " + cachedSize.ToString() 
                                      + " did not match computed: " +newPreferredSize.ToString() 
                                      +". Did we forget to invalidate the cache?\r\n\r\nControl Information: " + WindowsFormsUtils.AssertControlInformation(cacheHitCorrect, this)
                                      + "\r\nChanged Properties\r\n " + CommonProperties.Debug_GetChangedProperties(this));                        
                            }
#endif                      
#endif
                            return cachedSize;
                    }
                }
            
              

                CacheTextInternal = true;
                try {
                    prefSize = GetPreferredSizeCore(proposedSize);
                }
                finally {
                    CacheTextInternal = false;
                }
              
                // There is no guarantee that GetPreferredSizeCore() return something within
                // proposedSize, so we apply the element's constraints again.
                prefSize = ApplySizeConstraints(prefSize);

                // If the "default" preferred size was requested, cache the computed value.
                // 
                if(GetState2(STATE2_USEPREFERREDSIZECACHE) && proposedSize == LayoutUtils.MaxSize) {
                    CommonProperties.xSetPreferredSizeCache(this, prefSize);
                }
            }
            return prefSize;
        }

        // Overriding this method allows us to get the caching and clamping the proposedSize/output to
        // MinimumSize / MaximumSize from GetPreferredSize for free.
        internal virtual Size GetPreferredSizeCore(Size proposedSize) {
            return CommonProperties.GetSpecifiedBounds(this).Size;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Handle"]/*' />
        /// <devdoc>
        ///     The HWND handle that this control is bound to. If the handle
        ///     has not yet been created, this will force handle creation.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        DispId(NativeMethods.ActiveX.DISPID_HWND),
        SRDescription(nameof(SR.ControlHandleDescr))
        ]
        public IntPtr Handle {
            get {
                if (checkForIllegalCrossThreadCalls &&
                    !inCrossThreadSafeCall &&
                    InvokeRequired) {
                    throw new InvalidOperationException(string.Format(SR.IllegalCrossThreadCall,
                                                                     Name));
                }

                if (!IsHandleCreated)
                {
                    CreateHandle();
                }

                return HandleInternal;
            }
        }

        internal IntPtr HandleInternal {
            get {
                return window.Handle;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.HasChildren"]/*' />
        /// <devdoc>
        ///     True if this control has child controls in its collection.  This
        ///     is more efficient than checking for Controls.Count > 0, but has the
        ///     same effect.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlHasChildrenDescr))
        ]
        public bool HasChildren {
            get {
                ControlCollection controls = (ControlCollection)Properties.GetObject(PropControlsCollection);
                return controls != null && controls.Count > 0;
            }
        }

        internal virtual bool HasMenu {
            get {
                return false;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Height"]/*' />
        /// <devdoc>
        ///     The height of this control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlHeightDescr))
        ]
        public int Height {
            get {
                return height;
            }
            set{
                SetBounds(x, y, width, value, BoundsSpecified.Height);
            }
        }

        internal bool HostedInWin32DialogManager {
            get {
                if (!GetState(STATE_CHECKEDHOST)) {
                    Control topMost = TopMostParent;
                    if (this != topMost) {
                        SetState(STATE_HOSTEDINDIALOG, topMost.HostedInWin32DialogManager);
                    }
                    else {
                        IntPtr parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(this, Handle));
                        IntPtr lastParentHandle = parentHandle;

                        StringBuilder sb = new StringBuilder(32);

                        SetState(STATE_HOSTEDINDIALOG, false);

                        while (parentHandle != IntPtr.Zero) {
                            int len = UnsafeNativeMethods.GetClassName(new HandleRef(null, lastParentHandle), null, 0);
                            if (len > sb.Capacity) {
                                sb.Capacity = len + 5;
                            }
                            UnsafeNativeMethods.GetClassName(new HandleRef(null, lastParentHandle), sb, sb.Capacity);

                            if (sb.ToString() == "#32770") {
                                SetState(STATE_HOSTEDINDIALOG, true);
                                break;
                            }

                            lastParentHandle = parentHandle;
                            parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(null, parentHandle));
                        }
                    }

                    SetState(STATE_CHECKEDHOST, true);

                }

                return GetState(STATE_HOSTEDINDIALOG);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IsHandleCreated"]/*' />
        /// <devdoc>
        ///     Whether or not this control has a handle associated with it.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlHandleCreatedDescr))
        ]
        public bool IsHandleCreated {
            get { return window.Handle != IntPtr.Zero; }
        }

        /// <devdoc>
        ///     Determines if layout is currently suspended.
        /// </devdoc>
        internal bool IsLayoutSuspended {
            get {
                return layoutSuspendCount > 0;
            }
        }

        internal bool IsWindowObscured {
            get {
                if (!IsHandleCreated || !Visible) {
                    return false;
                }

                bool emptyRegion = false;

                NativeMethods.RECT temp = new NativeMethods.RECT();
                Region working;
                Control parent = ParentInternal;
                if (parent != null) {
                    while (parent.ParentInternal != null ) {
                        parent = parent.ParentInternal;
                    }
                }

                UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref temp);
                working = new Region(Rectangle.FromLTRB(temp.left, temp.top, temp.right, temp.bottom));

                try {
                    IntPtr prev;
                    IntPtr next;
                    IntPtr start;
                    if (parent != null) {
                        start = parent.Handle;
                    }
                    else {
                        start = Handle;
                    }

                    for (prev = start;
                         (next = UnsafeNativeMethods.GetWindow(new HandleRef(null, prev), NativeMethods.GW_HWNDPREV)) != IntPtr.Zero;
                         prev = next) {

                        UnsafeNativeMethods.GetWindowRect(new HandleRef(null, next), ref temp);
                        Rectangle current = Rectangle.FromLTRB(temp.left, temp.top, temp.right, temp.bottom);

                        if (SafeNativeMethods.IsWindowVisible(new HandleRef(null, next))) {
                            working.Exclude(current);
                        }
                    }


                    Graphics g = CreateGraphics();
                    try {
                        emptyRegion = working.IsEmpty(g);
                    }
                    finally {
                        g.Dispose();
                    }
                }
                finally {
                    working.Dispose();
                }

                return emptyRegion;
            }
        }

        /// <devdoc>
        ///     Returns the current value of the handle. This may be zero if the handle
        ///     has not been created.
        /// </devdoc>
        internal IntPtr InternalHandle {
            get {
                if (!IsHandleCreated)
                {
                    return IntPtr.Zero;
                }
                else
                {
                    return Handle;
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InvokeRequired"]/*' />
        /// <devdoc>
        ///     Determines if the caller must call invoke when making method
        ///     calls to this control.  Controls in windows forms are bound to a specific thread,
        ///     and are not thread safe.  Therefore, if you are calling a control's method
        ///     from a different thread, you must use the control's invoke method
        ///     to marshal the call to the proper thread.  This function can be used to
        ///     determine if you must call invoke, which can be handy if you don't know
        ///     what thread owns a control.
        ///
        ///     There are five functions on a control that are safe to call from any
        ///     thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and
        ///     CreateGraphics.  For all other method calls, you should use one of the
        ///     invoke methods.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlInvokeRequiredDescr))
        ]
        public bool InvokeRequired {
            get {

                using (new MultithreadSafeCallScope())
                {
                    HandleRef hwnd;
                    if (IsHandleCreated) {
                        hwnd = new HandleRef(this, Handle);
                    }
                    else {
                        Control marshalingControl = FindMarshalingControl();

                        if (!marshalingControl.IsHandleCreated) {
                            return false;
                        }

                        hwnd = new HandleRef(marshalingControl, marshalingControl.Handle);
                    }

                    int pid;
                    int hwndThread = SafeNativeMethods.GetWindowThreadProcessId(hwnd, out pid);
                    int currentThread = SafeNativeMethods.GetCurrentThreadId();
                    return(hwndThread != currentThread);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IsAccessible"]/*' />
        /// <devdoc>
        ///      Indicates whether or not this control is an accessible control
        ///      i.e. whether it should be visible to accessibility applications.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlIsAccessibleDescr))
        ]
        public bool IsAccessible {
            get {
                return GetState(STATE_ISACCESSIBLE);
            }
            set {
                SetState(STATE_ISACCESSIBLE, value);
            }
        }

        /// <devdoc>
        ///     Used to tell if this control is being hosted as an ActiveX control.
        /// </devdoc>
        internal bool IsActiveX {
            get {
                return GetState2(STATE2_ISACTIVEX);
            }
        }

        // If the control on which GetContainerControl( ) is called is a ContainerControl, then we dont return the parent 
        // but return the same control. This is Everett behavior so we cannot change this since this would be a breaking change.
        // Hence we have a new internal property IsContainerControl which returns false for all Everett control, but 
        // this property is overidden in SplitContainer to return true so that we skip the SplitContainer 
        // and the correct Parent ContainerControl is returned by GetContainerControl().
        internal virtual bool IsContainerControl
        {
            get
            {
                return false;
            }
        }

        /// <devdoc>
        ///     Used to tell if this control is being hosted in IE.
        /// </devdoc>
        internal bool IsIEParent {
            get {
                return IsActiveX ? ActiveXInstance.IsIE : false;
            }
        }

        /// <devdoc>
        ///     Used to tell if the control is mirrored
        ///     Don't call this from CreateParams. Will lead to nasty problems
        ///     since we might call CreateParams here - you dig!
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),        
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.IsMirroredDescr))        
        ]
        public bool IsMirrored {
            get {
                if (!IsHandleCreated) {
                    CreateParams cp = CreateParams;
                    SetState(STATE_MIRRORED, (cp.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) != 0);
                }
                return GetState(STATE_MIRRORED);
            }
        }

        /// <devdoc>
        ///     Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
        /// </devdoc>
        internal virtual bool IsMnemonicsListenerAxSourced
        {
            get
            {
                return false;
            }
        }
        
        /// <devdoc>
        ///     Used to tell if this BackColor is Supported
        /// </devdoc>
        private bool IsValidBackColor (Color c){
           if (!c.IsEmpty && !GetStyle(ControlStyles.SupportsTransparentBackColor) && c.A < 255)
           {
                return false;
           }
           return true;

        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Left"]/*' />
        /// <devdoc>
        ///     The left coordinate of this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlLeftDescr))
        ]
        public int Left {
            get {
                return x;
            }
            set {
                SetBounds(value, y, width, height, BoundsSpecified.X);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Location"]/*' />
        /// <devdoc>
        ///     The location of this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.ControlLocationDescr))
        ]
        public Point Location {
            get {
                return new Point(x, y);
            }
            set {
                SetBounds(value.X, value.Y, width, height, BoundsSpecified.Location);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.LocationChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnLocationChangedDescr))]
        public event EventHandler LocationChanged {
            add {
                Events.AddHandler(EventLocation, value);
            }
            remove {
                Events.RemoveHandler(EventLocation, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Margin"]/*' />
        [
        SRDescription(nameof(SR.ControlMarginDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true)
        ]
        public Padding Margin
        {
            get { return CommonProperties.GetMargin(this); }
            set
            {
                // This should be done here rather than in the property store as
                // some IArrangedElements actually support negative padding.
                value = LayoutUtils.ClampNegativePaddingToZero(value);

                // SetMargin causes a layout as a side effect.
                if (value != Margin)
                {
                    CommonProperties.SetMargin(this, value);
                    OnMarginChanged(EventArgs.Empty);
                }
                Debug.Assert(Margin == value, "Error detected while setting Margin.");
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MarginChanged"]/*' />
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnMarginChangedDescr))]
        public event EventHandler MarginChanged
        {
            add
            {
                Events.AddHandler(EventMarginChanged, value);
            }
            remove
            {
                Events.RemoveHandler(EventMarginChanged, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MaximumSize"]/*' />
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [SRDescription(nameof(SR.ControlMaximumSizeDescr))]
        [AmbientValue(typeof(Size), "0, 0")]
        public virtual Size MaximumSize {
            get { return CommonProperties.GetMaximumSize(this, DefaultMaximumSize); }
            set {
                if (value == Size.Empty) {
                    CommonProperties.ClearMaximumSize(this);
                    Debug.Assert(MaximumSize == DefaultMaximumSize, "Error detected while resetting MaximumSize.");
                }
                else if (value != MaximumSize) {
                    // SetMaximumSize causes a layout as a side effect.
                    CommonProperties.SetMaximumSize(this, value);
                    Debug.Assert(MaximumSize == value, "Error detected while setting MaximumSize.");
                }

            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MinimumSize"]/*' />
        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [SRDescription(nameof(SR.ControlMinimumSizeDescr))]
        public virtual Size MinimumSize {
            get { return CommonProperties.GetMinimumSize(this, DefaultMinimumSize); }
            set {
                if (value != MinimumSize) {
                    // SetMinimumSize causes a layout as a side effect.
                    CommonProperties.SetMinimumSize(this, value);
                }
                Debug.Assert(MinimumSize == value, "Error detected while setting MinimumSize.");
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ModifierKeys"]/*' />
        /// <devdoc>
        ///     Retrieves the current state of the modifier keys. This will check the
        ///     current state of the shift, control, and alt keys.
        /// </devdoc>
        public static Keys ModifierKeys {
            get {
                Keys modifiers = 0;
                // SECURITYNOTE : only let state of Shift-Control-Alt out...
                //
                if (UnsafeNativeMethods.GetKeyState((int)Keys.ShiftKey) < 0) modifiers |= Keys.Shift;
                if (UnsafeNativeMethods.GetKeyState((int)Keys.ControlKey) < 0) modifiers |= Keys.Control;
                if (UnsafeNativeMethods.GetKeyState((int)Keys.Menu) < 0) modifiers |= Keys.Alt;
                return modifiers;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseButtons"]/*' />
        /// <devdoc>
        ///     The current state of the mouse buttons. This will check the
        ///     current state of the left, right, and middle mouse buttons.
        /// </devdoc>
        public static MouseButtons MouseButtons {
            get {
                MouseButtons buttons = (MouseButtons)0;
                // SECURITYNOTE : only let state of MouseButtons out...
                //
                if (UnsafeNativeMethods.GetKeyState((int)Keys.LButton) < 0) buttons |= MouseButtons.Left;
                if (UnsafeNativeMethods.GetKeyState((int)Keys.RButton) < 0) buttons |= MouseButtons.Right;
                if (UnsafeNativeMethods.GetKeyState((int)Keys.MButton) < 0) buttons |= MouseButtons.Middle;
                if (UnsafeNativeMethods.GetKeyState((int)Keys.XButton1) < 0) buttons |= MouseButtons.XButton1;
                if (UnsafeNativeMethods.GetKeyState((int)Keys.XButton2) < 0) buttons |= MouseButtons.XButton2;
                return buttons;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MousePosition"]/*' />
        /// <devdoc>
        ///     The current position of the mouse in screen coordinates.
        /// </devdoc>
        public static Point MousePosition {
            get {
                // Reviewed: The following demand has always been commented out since v1.0. In Whidbey, we have
                // re-reviewed this method to see if it needs protection, but haven't found any threats, so are
                // going to keep it this way.
                //
                // Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ScreenLocationOfThings Demanded");
                // IntSecurity.ScreenLocationOfThings.Demand();

                NativeMethods.POINT pt = new NativeMethods.POINT();
                UnsafeNativeMethods.GetCursorPos(pt);
                return new Point(pt.x, pt.y);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Name"]/*' />
        /// <devdoc>
        ///     Name of this control. The designer will set this to the same
        ///     as the programatic Id "(name)" of the control.  The name can be
        ///     used as a key into the ControlCollection.
        /// </devdoc>
        [Browsable(false)]
        public string Name {
            get {
                string name = (string)Properties.GetObject(PropName);
                if (String.IsNullOrEmpty(name)) {
                    if (Site != null) {
                        name = Site.Name;
                    }

                    if (name == null) {
                        name = "";
                    }
                }

                return name;
            }
            set {
                if (String.IsNullOrEmpty(value)) {
                    Properties.SetObject(PropName, null);
                }
                else {
                    Properties.SetObject(PropName, value);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Parent"]/*' />
        /// <devdoc>
        ///     The parent of this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlParentDescr))
        ]
        public Control Parent {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "GetParent Demanded");
                IntSecurity.GetParent.Demand();

                return ParentInternal;
            }
            set {
                ParentInternal = value;
            }
        }

        internal virtual Control ParentInternal {
            get {
                return parent;
            }
            set {
                if (parent != value) {
                    if (value != null) {
                        value.Controls.Add(this);
                    }
                    else {
                        parent.Controls.Remove(this);
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProductName"]/*' />
        /// <devdoc>
        ///     Retrieves the product name of this specific component.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlProductNameDescr))
        ]
        public string ProductName {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                return VersionInfo.ProductName;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProductVersion"]/*' />
        /// <devdoc>
        ///     Retrieves the product version of this specific component.
        /// </devdoc>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlProductVersionDescr))
        ]
        public string ProductVersion {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                return VersionInfo.ProductVersion;
            }
        }

        /// <devdoc>
        ///     Retrieves our internal property storage object. If you have a property
        ///     whose value is not always set, you should store it in here to save
        ///     space.
        /// </devdoc>
        internal PropertyStore Properties {
            get {
                return propertyStore;
            }
        }

        // Returns the value of the backColor field -- no asking the parent with its color is, etc.
        internal Color RawBackColor {
            get {
                return Properties.GetColor(PropBackColor);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RecreatingHandle"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control is currently recreating its handle. This
        ///       property is read-only.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlRecreatingHandleDescr))
        ]
        public bool RecreatingHandle {
            get {
                return(state & STATE_RECREATE) != 0;
            }
        }

        internal virtual void AddReflectChild() {
        }

        internal virtual void RemoveReflectChild() {
        }

        private Control ReflectParent {
            get {
                return reflectParent;
            }
            set {
                if (value != null) {
                    value.AddReflectChild();
                }

                Control c = ReflectParent as Control;
                reflectParent = value;
                if (c != null) {
                    c.RemoveReflectChild();
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Region"]/*' />
        /// <devdoc>
        ///     The Region associated with this control.  (defines the
        ///     outline/silhouette/boundary of control)
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlRegionDescr))
        ]
        public Region Region {
            get {
                return (Region)Properties.GetObject(PropRegion);
            }
            set {
                if (GetState(STATE_TOPLEVEL)) {
                    Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ChangeWindowRegion Demanded");
                    IntSecurity.ChangeWindowRegionForTopLevel.Demand();
                }

                Region oldRegion = Region;
                if (oldRegion != value) {
                    Properties.SetObject(PropRegion, value);

                    if (oldRegion != null) {
                        oldRegion.Dispose();
                    }

                    if (IsHandleCreated) {
                        IntPtr regionHandle = IntPtr.Zero;

                        try {
                            if (value != null) {
                                regionHandle = GetHRgn(value);
                            }

                            if (IsActiveX) {
                                regionHandle = ActiveXMergeRegion(regionHandle);
                            }

                            if (UnsafeNativeMethods.SetWindowRgn(new HandleRef(this, Handle), new HandleRef(this, regionHandle), SafeNativeMethods.IsWindowVisible(new HandleRef(this, Handle))) != 0) {
                                //The Hwnd owns the region.
                                regionHandle = IntPtr.Zero;
                            }
                        }
                        finally {
                            if (regionHandle != IntPtr.Zero) {
                                SafeNativeMethods.DeleteObject(new HandleRef(null, regionHandle));
                            }
                        }
                    }

                    OnRegionChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RegionChanged"]/*' />
        /// <devdoc>
        ///     Event fired when the value of Region property is changed on Control
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlRegionChangedDescr))]
        public event EventHandler RegionChanged {
            add {
               Events.AddHandler(EventRegionChanged, value);
            }
            remove {
                Events.RemoveHandler(EventRegionChanged, value);
            }
        }

        // Helper function for Rtl
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RenderRightToLeft"]/*' />
        /// <internalonly/>
        [Obsolete("This property has been deprecated. Please use RightToLeft instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        protected internal bool RenderRightToLeft
        {
            get {
                return true;
            }
        }

        /// <devdoc>
        ///     Determines if the parent's background will be rendered on the label control.
        /// </devdoc>
        internal bool RenderTransparent {
            get {
                return GetStyle(ControlStyles.SupportsTransparentBackColor) && this.BackColor.A < 255;
            }
        }

        /// <devdoc>
        /// </devdoc>
        private bool RenderColorTransparent(Color c) {
            return GetStyle(ControlStyles.SupportsTransparentBackColor) && c.A < 255;
        }


        /// <devdoc>
        /// This property is required by certain controls (TabPage) to render its transparency using theming API.
        /// We dont want all controls (that are have transparent BackColor) to use theming API to render its background because it has  HUGE PERF cost.
        /// </devdoc>
        /// <internalonly/>
        internal virtual bool RenderTransparencyWithVisualStyles {
            get {
                return false;
            }
        }

        /// <devdoc>
        ///     Represents the bounds of the control that need to be scaled.  Control bounds
        ///     need to be scaled until ScaleControl is called.  They need to be scaled again
        ///     if their bounds change after ScaleControl is called.
        /// </devdoc>
        internal BoundsSpecified RequiredScaling {
            get {
                if ((requiredScaling & RequiredScalingEnabledMask) != 0) {
                    return (BoundsSpecified)(requiredScaling & RequiredScalingMask);
                }
                return BoundsSpecified.None;
            }
            set {
                byte enableBit = (byte)(requiredScaling & RequiredScalingEnabledMask);
                requiredScaling = (byte)(((int)value & RequiredScalingMask) | enableBit);
            }
        }

        /// <devdoc>
        ///     Determines if the required scaling property is enabled.  If not,
        ///     RequiredScaling always returns None.
        /// </devdoc>
        internal bool RequiredScalingEnabled {
            get {
                return (requiredScaling & RequiredScalingEnabledMask) != 0;
            }
            set {
                byte scaling = (byte)(requiredScaling & RequiredScalingMask);
                requiredScaling = scaling;
                if (value) requiredScaling |= RequiredScalingEnabledMask;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResizeRedraw"]/*' />
        /// <devdoc>
        ///     Indicates whether the control should redraw itself when resized.
        /// </devdoc>
        [
        SRDescription(nameof(SR.ControlResizeRedrawDescr))
        ]
        protected bool ResizeRedraw {
            get {
                return GetStyle( ControlStyles.ResizeRedraw );
            }
            set {
                SetStyle( ControlStyles.ResizeRedraw, value );
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Right"]/*' />
        /// <devdoc>
        ///    <para>The right coordinate of the control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlRightDescr))
        ]
        public int Right {
            get {
                return x + width;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RightToLeft"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        AmbientValue(RightToLeft.Inherit),
        SRDescription(nameof(SR.ControlRightToLeftDescr))
        ]
        public virtual RightToLeft RightToLeft {
            get {
                bool found;
                int rightToLeft = Properties.GetInteger(PropRightToLeft, out found);
                if (!found) {
                    rightToLeft = (int)RightToLeft.Inherit;
                }

                if (((RightToLeft)rightToLeft) == RightToLeft.Inherit) {
                    Control parent = ParentInternal;
                    if (parent != null) {
                        rightToLeft = (int)parent.RightToLeft;
                    }
                    else {
                        rightToLeft = (int)DefaultRightToLeft;
                    }
                }
                return (RightToLeft)rightToLeft;
            }

            set {
                //valid values are 0x0 to 0x2.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)RightToLeft.No, (int)RightToLeft.Inherit)){
                    throw new InvalidEnumArgumentException(nameof(RightToLeft), (int)value, typeof(RightToLeft));
                }

                RightToLeft oldValue = RightToLeft;

                if (Properties.ContainsInteger(PropRightToLeft) || value != RightToLeft.Inherit) {
                    Properties.SetInteger(PropRightToLeft, (int)value);
                }

                if (oldValue != RightToLeft) {
                    // Setting RTL on a container does not cause the container to change size.
                    // Only the children need to have thier layout updated.
                    using(new LayoutTransaction(this, this, PropertyNames.RightToLeft)) {
                        OnRightToLeftChanged(EventArgs.Empty);
                    }
                }
            }
            }



        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RightToLeftChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged {
            add {
                Events.AddHandler(EventRightToLeft, value);
            }
            remove {
                Events.RemoveHandler(EventRightToLeft, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ScaleChildren"]/*' />
        /// <devdoc>
        ///     This property controls the scaling of child controls.  If true child controls
        ///     will be scaled when the Scale method on this control is called.  If false,
        ///     child controls will not be scaled.  The default is true, and you must override
        ///     this property to provide a different value.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool ScaleChildren {
            get {
                return true;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Site"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override ISite Site {
            get {
                return base.Site;
            }
            set {
                AmbientProperties oldAmbients = AmbientPropertiesService;
                AmbientProperties newAmbients = null;

                if (value != null) {
                    newAmbients = (AmbientProperties)value.GetService(typeof(AmbientProperties));
                }


                // If the ambients changed, compare each property.
                //
                if (oldAmbients != newAmbients) {
                    bool checkFont = !Properties.ContainsObject(PropFont);
                    bool checkBackColor = !Properties.ContainsObject(PropBackColor);
                    bool checkForeColor = !Properties.ContainsObject(PropForeColor);
                    bool checkCursor = !Properties.ContainsObject(PropCursor);

                    Font oldFont = null;
                    Color oldBackColor = Color.Empty;
                    Color oldForeColor = Color.Empty;
                    Cursor oldCursor = null;

                    if (checkFont) {
                        oldFont = Font;
                    }

                    if (checkBackColor) {
                        oldBackColor = BackColor;
                    }

                    if (checkForeColor) {
                        oldForeColor = ForeColor;
                    }

                    if (checkCursor) {
                        oldCursor = Cursor;
                    }

                    Properties.SetObject(PropAmbientPropertiesService, newAmbients);
                    base.Site = value;

                    if (checkFont && !oldFont.Equals(Font)) {
                        OnFontChanged(EventArgs.Empty);
                    }
                    if (checkForeColor && !oldForeColor.Equals(ForeColor)) {
                        OnForeColorChanged(EventArgs.Empty);
                    }
                    if (checkBackColor && !oldBackColor.Equals(BackColor)) {
                        OnBackColorChanged(EventArgs.Empty);
                    }
                    if (checkCursor && oldCursor.Equals(Cursor)) {
                        OnCursorChanged(EventArgs.Empty);
                    }
                }
                else {
                    // If the ambients haven't changed, we just set a new site.
                    //
                    base.Site = value;
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Size"]/*' />
        /// <devdoc>
        ///    <para>The size of the control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.ControlSizeDescr))
        ]
        public Size Size {
            get {
                return new Size(width, height);
            }
            set {
                SetBounds(x, y, value.Width, value.Height, BoundsSpecified.Size);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SizeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnSizeChangedDescr))]
        public event EventHandler SizeChanged {
            add {
                Events.AddHandler(EventSize, value);
            }
            remove {
                Events.RemoveHandler(EventSize, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.TabIndex"]/*' />
        /// <devdoc>
        ///    <para>
        ///       The tab index of
        ///       this control.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        MergableProperty(false),
        SRDescription(nameof(SR.ControlTabIndexDescr))
        ]
        public int TabIndex {
            get {
                return tabIndex == -1 ? 0 : tabIndex;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(TabIndex), string.Format(SR.InvalidLowBoundArgumentEx, "TabIndex", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }

                if (tabIndex != value) {
                    tabIndex = value;
                    OnTabIndexChanged(EventArgs.Empty);
                }
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.TabIndexChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnTabIndexChangedDescr))]
        public event EventHandler TabIndexChanged {
            add {
                Events.AddHandler(EventTabIndex, value);
            }
            remove {
                Events.RemoveHandler(EventTabIndex, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.TabStop"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the user can give the focus to this control using the TAB
        ///       key. This property is read-only.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        DispId(NativeMethods.ActiveX.DISPID_TABSTOP),
        SRDescription(nameof(SR.ControlTabStopDescr))
        ]
        public bool TabStop {
            get {
                return TabStopInternal;
            }
            set {
                if (TabStop != value) {
                    TabStopInternal = value;
                    if (IsHandleCreated) SetWindowStyle(NativeMethods.WS_TABSTOP, value);
                    OnTabStopChanged(EventArgs.Empty);
                }
            }
        }

        // Grab out the logical of setting TABSTOP state, so that derived class could use this.
        internal bool TabStopInternal {
            get {
                return (state & STATE_TABSTOP) != 0;
            }
            set {
                if (TabStopInternal != value) {
                    SetState(STATE_TABSTOP, value);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.TabStopChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnTabStopChangedDescr))]
        public event EventHandler TabStopChanged {
            add {
                Events.AddHandler(EventTabStop, value);
            }
            remove {
                Events.RemoveHandler(EventTabStop, value);
            }
        }



        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return Properties.GetObject(PropUserData);
            }
            set {
                Properties.SetObject(PropUserData, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Text"]/*' />
        /// <devdoc>
        ///     The current text associated with this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        Bindable(true),
        DispId(NativeMethods.ActiveX.DISPID_TEXT),
        SRDescription(nameof(SR.ControlTextDescr))
        ]
        public virtual string Text {
            get {
                if (CacheTextInternal) {
                    return(text == null) ? "" : text;
                }
                else {
                    return WindowText;
                }
            }

            set {
                if (value == null) {
                    value = "";
                }

                if (value == Text) {
                    return;
                }

                if (CacheTextInternal) {
                    text = value;
                }
                WindowText = value;
                OnTextChanged(EventArgs.Empty);

                if( this.IsMnemonicsListenerAxSourced ){
                    for( Control ctl = this; ctl != null; ctl = ctl.ParentInternal ) {
                        ActiveXImpl activeXImpl = (ActiveXImpl)ctl.Properties.GetObject(PropActiveXImpl);
                        if( activeXImpl != null ) {
                            activeXImpl.UpdateAccelTable();
                            break;
                        }
                    }
                }
               
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.TextChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnTextChangedDescr))]
        public event EventHandler TextChanged {
            add {
                Events.AddHandler(EventText, value);
            }
            remove {
                Events.RemoveHandler(EventText, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Top"]/*' />
        /// <devdoc>
        ///     Top coordinate of this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlTopDescr))
        ]
        public int Top {
            get {
                return y;
            }
            set {
                SetBounds(x, value, width, height, BoundsSpecified.Y);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.TopLevelControl"]/*' />
        /// <devdoc>
        ///     The top level control that contains this control. This doesn't
        ///     have to be the same as the value returned from getForm since forms
        ///     can be parented to other controls.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlTopLevelControlDescr))
        ]
        public Control TopLevelControl {
            get {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "GetParent Demanded");
                IntSecurity.GetParent.Demand();
                return TopLevelControlInternal;
            }
        }

        internal Control TopLevelControlInternal {
            get {
                Control control = this;
                while (control != null && !control.GetTopLevel()) {
                    control = control.ParentInternal;
                }
                return control;
            }
        }

        internal Control TopMostParent {
            get {
                Control control = this;
                while (control.ParentInternal != null) {
                    control = control.ParentInternal;
                }
                return control;
            }
        }


        private BufferedGraphicsContext BufferContext {
            get {
                //This auto upgraged v1 client to per-process doublebuffering logic
                //
                return BufferedGraphicsManager.Current;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShowKeyboardCues"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the user interface is in a state to show or hide keyboard
        ///       accelerators. This property is read-only.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal virtual bool ShowKeyboardCues {
            get {
                // Controls in design mode always draw their accellerators.
                if (!IsHandleCreated || DesignMode) {
                    return true;
                    // would be nice to query SystemParametersInfo, but have trouble
                    // getting this to work and this should not really be called before
                    // handle created anyway
                }

                // How this all works
                
                // uiCuesState contains this control's cached state of whether or not it thinks
                // accelerators/focus cues are turned on. the first 16 bits represent focus cues
                // the second represent keyboard cues.  "F" is the UISTATE_FOCUS_CUES_MASK, 
                // "F0" is the UISTATE_KEYBOARD_CUES_MASK

                // We check here if we have cached state.  If we dont, we need to initialize ourself.
                // We do this by checking "MenuAccessKeysUnderlined" - we show if this returns true.

                // If MenuAccessKeysUnderlined returns false, we have to manually call CHANGEUISTATE on the topmost control 
                // Why? Well the way the API seems to work is that it stores in a bit flag for the the hidden 
                // state.

                // Details from the Menu keydown to changed value of uiCuesState...
                
                // When someone does press the ALT (Menu)/F10 key we will
                //   Call ProcessUICues on the control that had focus at the time
                //          ProcessUICues will check the current state of the control using WM_QUERYUISTATE               
                //          If WM_QUERYUISTATE indicates that the accelerators are hidden we will 
                //                  either call WM_UPDATEUISTATE or WM_CHANGEUISTATE depending on whether we're hosted or not.
                //          All controls in the heirarchy will be individually called back on WM_UPDATEUISTATE, which will go into WmUpdateUIState.
                //   In WmUpdateUIState, we will update our uiCuesState cached value, which 
                //   changes the public value of what we return here for ShowKeyboardCues/ShowFocusCues.

                if ((uiCuesState & UISTATE_KEYBOARD_CUES_MASK) == 0) {
                        if (SystemInformation.MenuAccessKeysUnderlined) {
                            uiCuesState |= UISTATE_KEYBOARD_CUES_SHOW;
                        }
                        else {
                            // if we're in the hidden state, we need to manufacture an update message so everyone knows it.
                            //
                            int actionMask = (NativeMethods.UISF_HIDEACCEL | 
                                (AccessibilityImprovements.Level1 ? 0 : NativeMethods.UISF_HIDEFOCUS)) << 16;
                            uiCuesState |= UISTATE_KEYBOARD_CUES_HIDDEN;

                            // The side effect of this initial state is that adding new controls may clear the accelerator
                            // state (has been this way forever)
                            UnsafeNativeMethods.SendMessage(new HandleRef(TopMostParent, TopMostParent.Handle),
                                    NativeMethods.WM_CHANGEUISTATE,
                                    (IntPtr)(actionMask | NativeMethods.UIS_SET),
                                    IntPtr.Zero);
                        }
                 }
                 return (uiCuesState & UISTATE_KEYBOARD_CUES_MASK) == UISTATE_KEYBOARD_CUES_SHOW;                
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShowFocusCues"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the user interface is in a state to show or hide focus
        ///       rectangles. This property is read-only.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal virtual bool ShowFocusCues {
            get {
                if (!IsHandleCreated) {
                    return true;
                    // would be nice to query SystemParametersInfo, but have trouble
                    // getting this to work and this should not really be called before
                    // handle created anyway
                }

                // See "How this all works" in ShowKeyboardCues

                if ((uiCuesState & UISTATE_FOCUS_CUES_MASK) == 0) {
                    if (SystemInformation.MenuAccessKeysUnderlined) {
                        uiCuesState |= UISTATE_FOCUS_CUES_SHOW;
                    }
                    else {
                        uiCuesState |= UISTATE_FOCUS_CUES_HIDDEN;

                        // if we're in the hidden state, we need to manufacture an update message so everyone knows it.
                        //
                        int actionMask = (NativeMethods.UISF_HIDEACCEL | NativeMethods.UISF_HIDEFOCUS) << 16;

                        // The side effect of this initial state is that adding new controls may clear the focus cue state
                        // state (has been this way forever)
                        UnsafeNativeMethods.SendMessage(new HandleRef(TopMostParent, TopMostParent.Handle),
                                NativeMethods.WM_CHANGEUISTATE,
                                (IntPtr)(actionMask | NativeMethods.UIS_SET),
                                IntPtr.Zero);

                    }
                }
                return (uiCuesState & UISTATE_FOCUS_CUES_MASK) == UISTATE_FOCUS_CUES_SHOW;
            }
        }

        // The parameter used in the call to ShowWindow for this control
        //
        internal virtual int ShowParams {
            get {
                return NativeMethods.SW_SHOW;
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UseWaitCursor"]/*' />
        /// <devdoc>
        ///     When this property in true the Cursor Property is set to WaitCursor as well as the Cursor Property
        ///     of all the child controls.
        /// </devdoc>
        [
        DefaultValue(false),
        EditorBrowsable(EditorBrowsableState.Always),
        Browsable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ControlUseWaitCursorDescr)),
        ]
        public bool UseWaitCursor {
            get { return GetState(STATE_USEWAITCURSOR); }
            set {
                if (GetState(STATE_USEWAITCURSOR) != value) {
                    SetState(STATE_USEWAITCURSOR, value);
                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                    if (controlsCollection != null) {
                        // PERFNOTE: This is more efficient than using Foreach.  Foreach
                        // forces the creation of an array subset enum each time we
                        // enumerate
                        for(int i = 0; i < controlsCollection.Count; i++) {
                            controlsCollection[i].UseWaitCursor = value;
                        }
                    }
                }
            }
        }

        /// <devdoc>
        ///     Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        ///     This property overwrites the UseCompatibleTextRenderingDefault switch when set programmatically.
        ///     Exposed publicly only by controls that support GDI text rendering (Label, LinkLabel and some others).
        ///     Observe that this property is NOT virtual (to allow for caching the property value - see LinkLabel)
        ///     and should be used by controls that support it only (see SupportsUseCompatibleTextRendering).
        /// </devdoc>
        internal bool UseCompatibleTextRenderingInt {
            get{
                if (Properties.ContainsInteger(PropUseCompatibleTextRendering)){
                    bool found;
                    int value = Properties.GetInteger(PropUseCompatibleTextRendering, out found);
                    if( found ){
                        return value == 1;
                    }
                }

                return Control.UseCompatibleTextRenderingDefault;
            }
            set{
                if( SupportsUseCompatibleTextRendering && UseCompatibleTextRenderingInt != value ){
                    Properties.SetInteger(PropUseCompatibleTextRendering, value ? 1 : 0);
                    // Update the preferred size cache since we will be rendering text using a different engine.
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.UseCompatibleTextRendering);
                    Invalidate();
                }
            }
        }

        /// <devdoc>
        ///     Determines whether the control supports rendering text using GDI+ and GDI.
        ///     This is provided for container controls (PropertyGrid) to iterate through its children to set 
        ///     UseCompatibleTextRendering to the same value if the child control supports it.
        /// </devdoc>
        internal virtual bool SupportsUseCompatibleTextRendering {
            get {
                return false;
            }
        }

        private ControlVersionInfo VersionInfo {
            get {
                ControlVersionInfo info = (ControlVersionInfo)Properties.GetObject(PropControlVersionInfo);
                if (info == null) {
                    info = new ControlVersionInfo(this);
                    Properties.SetObject(PropControlVersionInfo, info);
                }
                return info;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Visible"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control is visible.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ControlVisibleDescr))
        ]
        public bool Visible {
            get {
                return GetVisibleCore();
            }
            set {
                SetVisibleCore(value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.VisibleChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control becomes visible.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnVisibleChangedDescr))]
        public event EventHandler VisibleChanged {
            add {
                Events.AddHandler(EventVisible, value);
            }
            remove {
                Events.RemoveHandler(EventVisible, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.WaitForWaitHandle"]/*' />
        /// <devdoc>
        ///     Wait for the wait handle to receive a signal: throw an exception if the thread is no longer with us.
        /// </devdoc>
        private void WaitForWaitHandle(WaitHandle waitHandle) {
            int threadId = CreateThreadId;
            Application.ThreadContext ctx = Application.ThreadContext.FromId(threadId);
            if (ctx == null) {
                // Couldn't find the thread context, so we don't know the state.  We shouldn't throw.
                return;
            }
            IntPtr threadHandle = ctx.GetHandle();
            bool processed = false;
            // setting default exitcode to 0, though it won't be accessed in current code below due to short-circuit logic in condition (returnValue will be false when exitCode is undefined)
            uint exitCode = 0;
            bool returnValue = false;
            while (!processed) {
                //Get the thread's exit code, if we found the thread as expected
                if (threadHandle != null) {
                    returnValue = UnsafeNativeMethods.GetExitCodeThread(threadHandle, out exitCode);
                }
                //If we didn't find the thread, or if GetExitCodeThread failed, we don't know the thread's state:
                //if we don't know, we shouldn't throw.
                if ((returnValue && exitCode != NativeMethods.STILL_ACTIVE) || 
                    (!returnValue && Marshal.GetLastWin32Error() == NativeMethods.ERROR_INVALID_HANDLE) ||
                    AppDomain.CurrentDomain.IsFinalizingForUnload()) {
                    if (waitHandle.WaitOne(1, false)) {
                        break;
                    }
                    throw new InvalidAsynchronousStateException(SR.ThreadNoLongerValid);
                }

                if (IsDisposed && threadCallbackList != null && threadCallbackList.Count > 0) {
                    lock (threadCallbackList) {
                        Exception ex = new System.ObjectDisposedException(GetType().Name);
                        while (threadCallbackList.Count > 0) {
                            ThreadMethodEntry entry = (ThreadMethodEntry)threadCallbackList.Dequeue();
                            entry.exception = ex;
                            entry.Complete();
                        }
                    }
                }

                processed = waitHandle.WaitOne(1000, false);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Width"]/*' />
        /// <devdoc>
        ///     The width of this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlWidthDescr))
        ]
        public int Width {
            get {
                return width;
            }
            set {
                SetBounds(x, y, value, height, BoundsSpecified.Width);
            }
        }

        /// <devdoc>
        ///     The current exStyle of the hWnd
        /// </devdoc>
        /// <internalonly/>
        private int WindowExStyle {
            get {
                return unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_EXSTYLE));
            }
            set {
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_EXSTYLE, new HandleRef(null, (IntPtr)value));
            }
        }

        /// <devdoc>
        ///     The current style of the hWnd
        /// </devdoc>
        /// <internalonly/>
        internal int WindowStyle {
            get {
                return unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE));
            }
            set {
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)value));
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.WindowTarget"]/*' />
        /// <devdoc>
        ///     The target of Win32 window messages.
        /// </devdoc>
        /// <internalonly/>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlWindowTargetDescr))
        ]
        public IWindowTarget WindowTarget {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                return window.WindowTarget;
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            set {
                window.WindowTarget = value;
            }
        }

        /// <devdoc>
        ///     The current text of the Window; if the window has not yet been created, stores it in the control.
        ///     If the window has been created, stores the text in the underlying win32 control.
        ///     This property should be used whenever you want to get at the win32 control's text. For all other cases,
        ///     use the Text property - but note that this is overridable, and any of your code that uses it will use
        ///     the overridden version in controls that subclass your own.
        /// </devdoc>
        internal virtual string WindowText {
            get {

                if (!IsHandleCreated) {
                    if (text == null) {
                        return "";
                    }
                    else {
                        return text;
                    }
                }

                using (new MultithreadSafeCallScope()) {

                    // it's okay to call GetWindowText cross-thread.
                    //

                    int textLen = SafeNativeMethods.GetWindowTextLength(new HandleRef(window, Handle));

                    // Check to see if the system supports DBCS character
                    // if so, double the length of the buffer.
                    if (SystemInformation.DbcsEnabled) {
                        textLen = (textLen * 2) + 1;
                    }
                    StringBuilder sb = new StringBuilder(textLen + 1);
                    UnsafeNativeMethods.GetWindowText(new HandleRef(window, Handle), sb, sb.Capacity);
                    return sb.ToString();
                }
            }
            set {
                if (value == null) value = "";
                if (!WindowText.Equals(value)) {
                    if (IsHandleCreated) {
                        UnsafeNativeMethods.SetWindowText(new HandleRef(window, Handle), value);
                    }
                    else {
                        if (value.Length == 0) {
                            text = null;
                        }
                        else {
                            text = value;
                        }
                    }
                }
            }
        }



        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Click"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is clicked.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnClickDescr))]
        public event EventHandler Click {
            add {
                Events.AddHandler(EventClick, value);
            }
            remove {
                Events.RemoveHandler(EventClick, value);
            }
        }



        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAdded"]/*' />
        /// <devdoc>
        ///    <para>Occurs when a new control is added.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.ControlOnControlAddedDescr))
        ]
        public event ControlEventHandler ControlAdded {
            add {
                Events.AddHandler(EventControlAdded, value);
            }
            remove {
                Events.RemoveHandler(EventControlAdded, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlRemoved"]/*' />
        /// <devdoc>
        ///    <para>Occurs when a control is removed.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.ControlOnControlRemovedDescr))
        ]
        public event ControlEventHandler ControlRemoved {
            add {
                Events.AddHandler(EventControlRemoved, value);
            }
            remove {
                Events.RemoveHandler(EventControlRemoved, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DragDrop"]/*' />
        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragDropDescr))]
        public event DragEventHandler DragDrop {
            add {
                Events.AddHandler(EventDragDrop, value);
            }
            remove {
                Events.RemoveHandler(EventDragDrop, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DragEnter"]/*' />
        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragEnterDescr))]
        public event DragEventHandler DragEnter {
            add {
                Events.AddHandler(EventDragEnter, value);
            }
            remove {
                Events.RemoveHandler(EventDragEnter, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DragOver"]/*' />
        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragOverDescr))]
        public event DragEventHandler DragOver {
            add {
                Events.AddHandler(EventDragOver, value);
            }
            remove {
                Events.RemoveHandler(EventDragOver, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DragLeave"]/*' />
        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragLeaveDescr))]
        public event EventHandler DragLeave {
            add {
                Events.AddHandler(EventDragLeave, value);
            }
            remove {
                Events.RemoveHandler(EventDragLeave, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GiveFeedback"]/*' />
        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnGiveFeedbackDescr))]
        public event GiveFeedbackEventHandler GiveFeedback {
            add {
                Events.AddHandler(EventGiveFeedback, value);
            }
            remove {
                Events.RemoveHandler(EventGiveFeedback, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.HandleCreated"]/*' />
        /// <devdoc>
        ///    <para>Occurs when a handle is created for the control.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatPrivate)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), SRDescription(nameof(SR.ControlOnCreateHandleDescr))]
        public event EventHandler HandleCreated {
            add {
                Events.AddHandler(EventHandleCreated, value);
            }
            remove {
                Events.RemoveHandler(EventHandleCreated, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.HandleDestroyed"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control's handle is destroyed.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatPrivate)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), SRDescription(nameof(SR.ControlOnDestroyHandleDescr))]
        public event EventHandler HandleDestroyed {
            add {
                Events.AddHandler(EventHandleDestroyed, value);
            }
            remove {
                Events.RemoveHandler(EventHandleDestroyed, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.HelpRequested"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnHelpDescr))]
        public event HelpEventHandler HelpRequested {
            add {
                Events.AddHandler(EventHelpRequested, value);
            }
            remove {
                Events.RemoveHandler(EventHelpRequested, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidated"]/*' />
        [SRCategory(nameof(SR.CatAppearance)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), SRDescription(nameof(SR.ControlOnInvalidateDescr))]
        public event InvalidateEventHandler Invalidated {
            add {
                Events.AddHandler(EventInvalidated, value);
            }
            remove {
                Events.RemoveHandler(EventInvalidated, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PreferredSize"]/*' />
        [Browsable(false)]
        public Size PreferredSize {
            get { return GetPreferredSize(Size.Empty); }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Padding"]/*' />
        [
        SRDescription(nameof(SR.ControlPaddingDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true)
        ]
        public Padding Padding {
            get { return CommonProperties.GetPadding(this, DefaultPadding); }
            set {
                if (value != Padding) {
                    CommonProperties.SetPadding(this, value);
                    // Ideally we are being laid out by a LayoutEngine that cares about our preferred size.
                    // We set our LAYOUTISDIRTY bit and ask our parent to refresh us.
                    SetState(STATE_LAYOUTISDIRTY, true);
                    using (new LayoutTransaction(ParentInternal, this, PropertyNames.Padding)) {
                        OnPaddingChanged(EventArgs.Empty);
                    }

                    if(GetState(STATE_LAYOUTISDIRTY)) {
                        // The above did not cause our layout to be refreshed.  We explicitly refresh our
                        // layout to ensure that any children are repositioned to account for the change
                        // in padding.
                       LayoutTransaction.DoLayout(this, this, PropertyNames.Padding);
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PaddingChanged"]/*' />
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnPaddingChangedDescr))]
        public event EventHandler PaddingChanged {
            add {
                Events.AddHandler(EventPaddingChanged, value);
            }
            remove {
                Events.RemoveHandler(EventPaddingChanged, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Paint"]/*' />
        [SRCategory(nameof(SR.CatAppearance)), SRDescription(nameof(SR.ControlOnPaintDescr))]
        public event PaintEventHandler Paint {
            add {
                Events.AddHandler(EventPaint, value);
            }
            remove {
                Events.RemoveHandler(EventPaint, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.QueryContinueDrag"]/*' />
        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnQueryContinueDragDescr))]
        public event QueryContinueDragEventHandler QueryContinueDrag {
            add {
                Events.AddHandler(EventQueryContinueDrag, value);
            }
            remove {
                Events.RemoveHandler(EventQueryContinueDrag, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.QueryAccessibilityHelp"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnQueryAccessibilityHelpDescr))]
        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp {
            add {
                Events.AddHandler(EventQueryAccessibilityHelp, value);
            }
            remove {
                Events.RemoveHandler(EventQueryAccessibilityHelp, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DoubleClick"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is double clicked.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick {
            add {
                Events.AddHandler(EventDoubleClick, value);
            }
            remove {
                Events.RemoveHandler(EventDoubleClick, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Enter"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is entered.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnEnterDescr))]
        public event EventHandler Enter {
            add {
                Events.AddHandler(EventEnter, value);
            }
            remove {
                Events.RemoveHandler(EventEnter, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GotFocus"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control receives focus.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnGotFocusDescr)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler GotFocus {
            add {
                Events.AddHandler(EventGotFocus, value);
            }
            remove {
                Events.RemoveHandler(EventGotFocus, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.KeyDown"]/*' />
        /// <devdoc>
        ///    <para>Occurs when a key is pressed down while the control has focus.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyDownDescr))]
        public event KeyEventHandler KeyDown {
            add {
                Events.AddHandler(EventKeyDown, value);
            }
            remove {
                Events.RemoveHandler(EventKeyDown, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.KeyPress"]/*' />
        /// <devdoc>
        ///    <para> Occurs when a key is pressed while the control has focus.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyPressDescr))]
        public event KeyPressEventHandler KeyPress {
            add {
                Events.AddHandler(EventKeyPress, value);
            }
            remove {
                Events.RemoveHandler(EventKeyPress, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.KeyUp"]/*' />
        /// <devdoc>
        ///    <para> Occurs when a key is released while the control has focus.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyUpDescr))]
        public event KeyEventHandler KeyUp {
            add {
                Events.AddHandler(EventKeyUp, value);
            }
            remove {
                Events.RemoveHandler(EventKeyUp, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Layout"]/*' />
        /// <devdoc>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnLayoutDescr))]
        public event LayoutEventHandler Layout {
            add {
                Events.AddHandler(EventLayout, value);
            }
            remove {
                Events.RemoveHandler(EventLayout, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Leave"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is left.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnLeaveDescr))]
        public event EventHandler Leave {
            add {
                Events.AddHandler(EventLeave, value);
            }
            remove {
                Events.RemoveHandler(EventLeave, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.LostFocus"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control loses focus.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnLostFocusDescr)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler LostFocus {
            add {
                Events.AddHandler(EventLostFocus, value);
            }
            remove {
                Events.RemoveHandler(EventLostFocus, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseClick"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is mouse clicked.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnMouseClickDescr))]
        public event MouseEventHandler MouseClick {
            add {
                Events.AddHandler(EventMouseClick, value);
            }
            remove {
                Events.RemoveHandler(EventMouseClick, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseDoubleClick"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is mouse double clicked.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnMouseDoubleClickDescr))]
        public event MouseEventHandler MouseDoubleClick {
            add {
                Events.AddHandler(EventMouseDoubleClick, value);
            }
            remove {
                Events.RemoveHandler(EventMouseDoubleClick, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseCaptureChanged"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control loses mouse Capture.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnMouseCaptureChangedDescr))]
        public event EventHandler MouseCaptureChanged {
            add {
                Events.AddHandler(EventMouseCaptureChanged, value);
            }
            remove {
                Events.RemoveHandler(EventMouseCaptureChanged, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseDown"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the mouse pointer is over the control and a mouse button is
        ///       pressed.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseDownDescr))]
        public event MouseEventHandler MouseDown {
            add {
                Events.AddHandler(EventMouseDown, value);
            }
            remove {
                Events.RemoveHandler(EventMouseDown, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseEnter"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the mouse pointer enters the control.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseEnterDescr))]
        public event EventHandler MouseEnter {
            add {
                Events.AddHandler(EventMouseEnter, value);
            }
            remove {
                Events.RemoveHandler(EventMouseEnter, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseLeave"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the mouse pointer leaves the control.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseLeaveDescr))]
        public event EventHandler MouseLeave {
            add {
                Events.AddHandler(EventMouseLeave, value);
            }
            remove {
                Events.RemoveHandler(EventMouseLeave, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DpiChangedBeforeParent"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the DPI resolution of the screen this control is displayed on changes, 
        ///    either when the top level window is moved between monitors or when the OS settings are changed.
        ///    This event is raised before the top level parent window recieves WM_DPICHANGED message.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnDpiChangedBeforeParentDescr))]
        public event EventHandler DpiChangedBeforeParent {
            add {
                Events.AddHandler(EventDpiChangedBeforeParent, value);
            }
            remove {
                Events.RemoveHandler(EventDpiChangedBeforeParent, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DpiChangedAfterParent"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the DPI resolution of the screen this control is displayed on changes, 
        ///    either when the top level window is moved between monitors or when the OS settings are changed.
        ///    This message is received after the top levet parent window recieves WM_DPICHANGED message.
        ///    </para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnDpiChangedAfterParentDescr))]
        public event EventHandler DpiChangedAfterParent {
            add {
                Events.AddHandler(EventDpiChangedAfterParent, value);
            }
            remove {
                Events.RemoveHandler(EventDpiChangedAfterParent, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseHover"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the mouse pointer hovers over the contro.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseHoverDescr))]
        public event EventHandler MouseHover {
            add {
                Events.AddHandler(EventMouseHover, value);
            }
            remove {
                Events.RemoveHandler(EventMouseHover, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseMove"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the mouse pointer is moved over the control.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseMoveDescr))]
        public event MouseEventHandler MouseMove {
            add {
                Events.AddHandler(EventMouseMove, value);
            }
            remove {
                Events.RemoveHandler(EventMouseMove, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseUp"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the mouse pointer is over the control and a mouse button is released.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseUpDescr))]
        public event MouseEventHandler MouseUp {
            add {
                Events.AddHandler(EventMouseUp, value);
            }
            remove {
                Events.RemoveHandler(EventMouseUp, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.MouseWheel"]/*' />
        /// <devdoc>
        ///    <para> Occurs when the mouse wheel moves while the control has focus.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseWheelDescr)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event MouseEventHandler MouseWheel {
            add {
                Events.AddHandler(EventMouseWheel, value);
            }
            remove {
                Events.RemoveHandler(EventMouseWheel, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Move"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is moved.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnMoveDescr))]
        public event EventHandler Move {
            add {
                Events.AddHandler(EventMove, value);
            }
            remove {
                Events.RemoveHandler(EventMove, value);
            }
        }

        /// <devdoc>
        ///    <para>Raised to preview a key down event</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.PreviewKeyDownDescr))]
        public event PreviewKeyDownEventHandler PreviewKeyDown {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            add {
                Events.AddHandler(EventPreviewKeyDown, value);
            }
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            remove {
                Events.RemoveHandler(EventPreviewKeyDown, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Resize"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is resized.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnResizeDescr)),
         EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Resize {
            add {
                Events.AddHandler(EventResize, value);
            }
            remove {
                Events.RemoveHandler(EventResize, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ChangeUICues"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnChangeUICuesDescr))]
        public event UICuesEventHandler ChangeUICues {
            add {
                Events.AddHandler(EventChangeUICues, value);
            }
            remove {
                Events.RemoveHandler(EventChangeUICues, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.StyleChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnStyleChangedDescr))]
        public event EventHandler StyleChanged {
            add {
                Events.AddHandler(EventStyleChanged, value);
            }
            remove {
                Events.RemoveHandler(EventStyleChanged, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SystemColorsChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnSystemColorsChangedDescr))]
        public event EventHandler SystemColorsChanged {
            add {
                Events.AddHandler(EventSystemColorsChanged, value);
            }
            remove {
                Events.RemoveHandler(EventSystemColorsChanged, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Validating"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is validating.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnValidatingDescr))]
        public event CancelEventHandler Validating {
            add {
                Events.AddHandler(EventValidating, value);
            }
            remove {
                Events.RemoveHandler(EventValidating, value);
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Validated"]/*' />
        /// <devdoc>
        ///    <para>Occurs when the control is done validating.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnValidatedDescr))]
        public event EventHandler Validated {
            add {
                Events.AddHandler(EventValidated, value);
            }
            remove {
                Events.RemoveHandler(EventValidated, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibilityNotifyClients"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal void AccessibilityNotifyClients(AccessibleEvents accEvent, int childID) {
            AccessibilityNotifyClients(accEvent, NativeMethods.OBJID_CLIENT, childID);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AccessibilityNotifyClients1"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void AccessibilityNotifyClients(AccessibleEvents accEvent, int objectID, int childID) {
            if (IsHandleCreated) {
                UnsafeNativeMethods.NotifyWinEvent((int)accEvent, new HandleRef(this, Handle), objectID, childID + 1);
            }
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        private IntPtr ActiveXMergeRegion(IntPtr region) {
            return ActiveXInstance.MergeRegion(region);
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private void ActiveXOnFocus(bool focus) {
            ActiveXInstance.OnFocus(focus);
        }

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private void ActiveXViewChanged() {
            ActiveXInstance.ViewChangedInternal();
        }

        #if ACTIVEX_SOURCING

        //
        // This has been cut from the product.
        //

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXRegister"]/*' />
        /// <devdoc>
        ///     This is called by regasm to register a control as an ActiveX control.
        /// </devdoc>
        [ComRegisterFunction()]
        protected static void ActiveXRegister(Type type) {

            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            // If the user is not registering an AX control, then
            // bail now.
            //
            if (!typeof(Control).IsAssignableFrom(type)) {
                return;
            }

            // Add the flags that define us as a control to the rest of the
            // world.
            //
            RegistryKey clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID", true).CreateSubKey("{" + type.GUID.ToString() + "}");
            RegistryKey key = clsidKey.CreateSubKey("Control");
            key.Close();

            key = clsidKey.CreateSubKey("Implemented Categories");
            RegistryKey subKey = key.CreateSubKey("{40FC6ED4-2438-11CF-A3DB-080036F12502}"); // CATID_Control
            subKey.Close();
            key.Close();

            // Calculate MiscStatus bits.  Note that this is a static version
            // of GetMiscStatus in ActiveXImpl below.  Keep them in sync!
            //
            int miscStatus = NativeMethods.OLEMISC_ACTIVATEWHENVISIBLE |
                             NativeMethods.OLEMISC_INSIDEOUT |
                             NativeMethods.OLEMISC_SETCLIENTSITEFIRST |
                             NativeMethods.OLEMISC_RECOMPOSEONRESIZE;
            if (typeof(IButtonControl).IsAssignableFrom(type)) {
                miscStatus |= NativeMethods.OLEMISC_ACTSLIKEBUTTON;
            }

            key = clsidKey.CreateSubKey("MiscStatus");
            key.SetValue("", miscStatus.ToString());
            key.Close();

            // Now add the typelib information.  Many containers don't
            // host Active X controls without a typelib in the registry
            // (visual basic won't even show it in the control dialog).
            //
            Guid typeLibGuid = Marshal.GetTypeLibGuidForAssembly(type.Assembly);
            Version assemblyVer = type.Assembly.GetName().Version;

            if (typeLibGuid != Guid.Empty) {
                key = clsidKey.CreateSubKey("TypeLib");
                key.SetValue("", "{" + typeLibGuid.ToString() + "}");
                key.Close();
                key = clsidKey.CreateSubKey("Version");
                key.SetValue("", assemblyVer.Major.ToString() + "." + assemblyVer.Minor.ToString());
                key.Close();
            }

            clsidKey.Close();
        }
        #endif

        /// <devdoc>
        ///     Helper method for retrieving an ActiveX property.  We abstract these
        ///     to another method so we do not force JIT the ActiveX codebase.
        /// </devdoc>
        private void ActiveXUpdateBounds(ref int x, ref int y, ref int width, ref int height, int flags) {
            ActiveXInstance.UpdateBounds(ref x, ref y, ref width, ref height, flags);
        }

        #if ACTIVEX_SOURCING

        //
        // This has been cut from the product.
        //

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXUnregister"]/*' />
        /// <devdoc>
        ///     This is called by regasm to un-register a control as an ActiveX control.
        /// </devdoc>
        [ComUnregisterFunction()]
        protected static void ActiveXUnregister(Type type) {

            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            // If the user is not unregistering an AX control, then
            // bail now.
            //
            if (!typeof(Control).IsAssignableFrom(type)) {
                return;
            }

            RegistryKey clsidKey = null;

            // Unregistration should be very robust and unregister what it can.  We eat all exceptions here.
            //
            try {
                clsidKey = Registry.ClassesRoot.OpenSubKey("CLSID", true).CreateSubKey("{" + type.GUID.ToString() + "}");

                try {
                    clsidKey.DeleteSubKeyTree("Control");
                }
                catch {
                }
                try {
                    clsidKey.DeleteSubKeyTree("Implemented Categories");
                }
                catch {
                }
                try {
                    clsidKey.DeleteSubKeyTree("MiscStatus");
                }
                catch {
                }
                try {
                    clsidKey.DeleteSubKeyTree("TypeLib");
                }
                catch {
                }
            }
            finally {
                if (clsidKey != null) {
                    clsidKey.Close();
                }
            }
        }
        #endif

        /// <devdoc>
        ///     Assigns a new parent control. Sends out the appropriate property change
        ///     notifications for properties that are affected by the change of parent.
        /// </devdoc>
        internal virtual void AssignParent(Control value) {

            // Adopt the parent's required scaling bits
            if (value != null) {
                RequiredScalingEnabled = value.RequiredScalingEnabled;
            }

            if (CanAccessProperties) {
                // Store the old values for these properties
                //
                Font oldFont = Font;
                Color oldForeColor = ForeColor;
                Color oldBackColor = BackColor;
                RightToLeft oldRtl = RightToLeft;
                bool oldEnabled = Enabled;
                bool oldVisible = Visible;

                // Update the parent
                //
                parent = value;
                OnParentChanged(EventArgs.Empty);                

                if (GetAnyDisposingInHierarchy()) {
                    return;
                }                

                // Compare property values with new parent to old values
                //
                if (oldEnabled != Enabled) {
                    OnEnabledChanged(EventArgs.Empty);
                }

                // When a control seems to be going from invisible -> visible,
                // yet its parent is being set to null and it's not top level, do not raise OnVisibleChanged.
                bool newVisible = Visible;
                
                if (oldVisible != newVisible && !(!oldVisible && newVisible && parent == null && !GetTopLevel())) {
                    OnVisibleChanged(EventArgs.Empty);
                }
                if (!oldFont.Equals(Font)) {
                    OnFontChanged(EventArgs.Empty);
                }
                if (!oldForeColor.Equals(ForeColor)) {
                    OnForeColorChanged(EventArgs.Empty);
                }
                if (!oldBackColor.Equals(BackColor)) {
                    OnBackColorChanged(EventArgs.Empty);
                }
                if (oldRtl != RightToLeft) {
                    OnRightToLeftChanged(EventArgs.Empty);
                }
                if (Properties.GetObject(PropBindingManager) == null && this.Created) {
                    // We do not want to call our parent's BindingContext property here.
                    // We have no idea if us or any of our children are using data binding,
                    // and invoking the property would just create the binding manager, which
                    // we don't need.  We just blindly notify that the binding manager has
                    // changed, and if anyone cares, they will do the comparison at that time.
                    //
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
            else {
                parent = value;
                OnParentChanged(EventArgs.Empty);
            }

            SetState(STATE_CHECKEDHOST, false);
            if (ParentInternal != null) ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.All);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ParentChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnParentChangedDescr))]
        public event EventHandler ParentChanged {
            add {
                Events.AddHandler(EventParent, value);
            }
            remove {
                Events.RemoveHandler(EventParent, value);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BeginInvoke"]/*' />
        /// <devdoc>
        ///     Executes the given delegate on the thread that owns this Control's
        ///     underlying window handle.  The delegate is called asynchronously and this
        ///     method returns immediately.  You may call this from any thread, even the
        ///     thread that owns the control's handle.  If the control's handle doesn't
        ///     exist yet, this will follow up the control's parent chain until it finds a
        ///     control or form that does have a window handle.  If no appropriate handle
        ///     can be found, BeginInvoke will throw an exception.  Exceptions within the
        ///     delegate method are considered untrapped and will be sent to the
        ///     application's untrapped exception handler.
        ///
        ///     There are five functions on a control that are safe to call from any
        ///     thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///     For all other method calls, you should use one of the invoke methods to marshal
        ///     the call to the control's thread.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginInvoke(Delegate method) {
            return BeginInvoke(method, null);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BeginInvoke1"]/*' />
        /// <devdoc>
        ///     Executes the given delegate on the thread that owns this Control's
        ///     underlying window handle.  The delegate is called asynchronously and this
        ///     method returns immediately.  You may call this from any thread, even the
        ///     thread that owns the control's handle.  If the control's handle doesn't
        ///     exist yet, this will follow up the control's parent chain until it finds a
        ///     control or form that does have a window handle.  If no appropriate handle
        ///     can be found, BeginInvoke will throw an exception.  Exceptions within the
        ///     delegate method are considered untrapped and will be sent to the
        ///     application's untrapped exception handler.
        ///
        ///     There are five functions on a control that are safe to call from any
        ///     thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///     For all other method calls, you should use one of the invoke methods to marshal
        ///     the call to the control's thread.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]        
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public IAsyncResult BeginInvoke(Delegate method, params Object[] args) {
            using (new MultithreadSafeCallScope()) {
                Control marshaler = FindMarshalingControl();
                return(IAsyncResult)marshaler.MarshaledInvoke(this, method, args, false);
            }
        }

        internal void BeginUpdateInternal() {
            if (!IsHandleCreated) {
                return;
            }
            if (updateCount == 0) SendMessage(NativeMethods.WM_SETREDRAW, 0, 0);
            updateCount++;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.BringToFront"]/*' />
        /// <devdoc>
        ///     Brings this control to the front of the zorder.
        /// </devdoc>
        public void BringToFront() {
            if (parent != null) {
                parent.Controls.SetChildIndex(this, 0);
            }
            else if (IsHandleCreated && GetTopLevel() && SafeNativeMethods.IsWindowEnabled(new HandleRef(window, Handle))) {
                SafeNativeMethods.SetWindowPos(new HandleRef(window, Handle), NativeMethods.HWND_TOP, 0, 0, 0, 0,
                                               NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            }
        }

        /// <devdoc>
        ///     Specifies whether this control can process the mnemonic or not.  A condition to process a mnemonic is that
        ///     all controls in the parent chain can do it too, but since the semantics for this function can be overriden,
        ///     we need to call the method on the parent 'recursively' (not exactly since it is not necessarily the same method).
        /// </devdoc>
        internal virtual bool CanProcessMnemonic() {
#if DEBUG
            TraceCanProcessMnemonic();
#endif
            if( !this.Enabled || !this.Visible) {
                return false;
            }

            if( this.parent != null ){
                return this.parent.CanProcessMnemonic();
            }

            return true;
        }

        // Package scope to allow AxHost to override
        //
        internal virtual bool CanSelectCore() {
            if ((controlStyle & ControlStyles.Selectable) != ControlStyles.Selectable) {
                return false;
            }

            for (Control ctl = this; ctl != null; ctl = ctl.parent) {
                if (!ctl.Enabled || !ctl.Visible) {
                    return false;
                }
            }

            return true;
        }

        /// <devdoc>
        ///     Searches the parent/owner tree for bottom to find any instance
        ///     of toFind in the parent/owner tree.
        /// </devdoc>
        internal static void CheckParentingCycle(Control bottom, Control toFind) {
            Form lastOwner = null;
            Control lastParent = null;

            for (Control ctl = bottom; ctl != null; ctl = ctl.ParentInternal) {
                lastParent = ctl;
                if (ctl == toFind) {
                    throw new ArgumentException(SR.CircularOwner);
                }
            }

            if (lastParent != null) {
                if (lastParent is Form) {
                    Form f = (Form)lastParent;
                    for (Form form = f; form != null; form = form.OwnerInternal) {
                        lastOwner = form;
                        if (form == toFind) {
                            throw new ArgumentException(SR.CircularOwner);
                        }
                    }
                }
            }

            if (lastOwner != null) {
                if (lastOwner.ParentInternal != null) {
                    CheckParentingCycle(lastOwner.ParentInternal, toFind);
                }
            }
        }
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void ChildGotFocus(Control child) {
            if (IsActiveX) {
                ActiveXOnFocus(true);
            }
            if (parent != null) {
                parent.ChildGotFocus(child);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Contains"]/*' />
        /// <devdoc>
        ///    <para>Verifies if a control is a child of this control.</para>
        /// </devdoc>
        public bool Contains(Control ctl) {
            while (ctl != null) {
                ctl = ctl.ParentInternal;
                if (ctl == null) {
                    return false;
                }
                if (ctl == this) {
                    return true;
                }
            }
            return false;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CreateAccessibilityInstance"]/*' />
        /// <devdoc>
        ///     constructs the new instance of the accessibility object for this control. Subclasses
        ///     should not call base.CreateAccessibilityObject.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual AccessibleObject CreateAccessibilityInstance() {
            return new ControlAccessibleObject(this);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CreateControlsInstance"]/*' />
        /// <devdoc>
        ///     Constructs the new instance of the Controls collection objects. Subclasses
        ///     should not call base.CreateControlsInstance.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual ControlCollection CreateControlsInstance() {
            return new System.Windows.Forms.Control.ControlCollection(this);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CreateGraphics"]/*' />
        /// <devdoc>
        ///     Creates a Graphics for this control. The control's brush, font, foreground
        ///     color and background color become the default values for the Graphics.
        ///     The returned Graphics must be disposed through a call to its dispose()
        ///     method when it is no longer needed.  The Graphics Object is only valid for
        ///     the duration of the current window's message.
        /// </devdoc>
        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        public System.Drawing.Graphics CreateGraphics() {
            using (new MultithreadSafeCallScope())
            {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "CreateGraphicsForControl Demanded");
                IntSecurity.CreateGraphicsForControl.Demand();
                return CreateGraphicsInternal();
            }
        }
       

        [ResourceExposure(ResourceScope.Process)]
        [ResourceConsumption(ResourceScope.Process)]
        internal System.Drawing.Graphics CreateGraphicsInternal() {
            return Graphics.FromHwndInternal(this.Handle);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CreateHandle"]/*' />
        /// <devdoc>
        ///     Creates a handle for this control. This method is called by the .NET Framework, this should
        ///     not be called. Inheriting classes should always call base.createHandle when
        ///     overriding this method.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)
        ]
        protected virtual void CreateHandle() {
            IntPtr userCookie = IntPtr.Zero;

            if (GetState(STATE_DISPOSED)) {
                throw new System.ObjectDisposedException(GetType().Name);
            }

            if (GetState(STATE_CREATINGHANDLE)) {
                return;
            }

            Rectangle originalBounds;

            try {
                SetState(STATE_CREATINGHANDLE, true);

                originalBounds = this.Bounds;

                if (Application.UseVisualStyles) {
                    // Activate theming scope to get theming for controls at design time and when hosted in browser.
                    // NOTE: If a theming context is already active, this call is very fast, so shouldn't be a perf issue.
                    userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                }

                CreateParams cp = CreateParams;
                SetState(STATE_MIRRORED, (cp.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) != 0);

                // Adjust for scrolling of parent...
                //
                if (parent != null) {
                    Rectangle parentClient = parent.ClientRectangle;

                    if (!parentClient.IsEmpty) {
                        if (cp.X != NativeMethods.CW_USEDEFAULT) {
                            cp.X -= parentClient.X;
                        }
                        if (cp.Y != NativeMethods.CW_USEDEFAULT) {
                            cp.Y -= parentClient.Y;
                        }
                    }
                }

                // And if we are WS_CHILD, ensure we have a parent handle.
                //
                if (cp.Parent == IntPtr.Zero && (cp.Style & NativeMethods.WS_CHILD) != 0) {
                    Debug.Assert((cp.ExStyle & NativeMethods.WS_EX_MDICHILD) == 0, "Can't put MDI child forms on the parking form");
                    Application.ParkHandle(cp);
                }

                window.CreateHandle(cp);

                UpdateReflectParent(true);

            }
            finally {
                SetState(STATE_CREATINGHANDLE, false);
                UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
            }

            // For certain controls (e.g., ComboBox) CreateWindowEx
            // may cause the control to resize.  WM_SETWINDOWPOSCHANGED takes care of
            // the control being resized, but our layout container may need a refresh as well.
            if (this.Bounds != originalBounds) {
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.CreateControl"]/*' />
        /// <devdoc>
        ///     Forces the creation of the control. This includes the creation of the handle,
        ///     and any child controls.
        /// </devdoc>
        public void CreateControl() {
            bool controlIsAlreadyCreated = this.Created;
            CreateControl(false);

            if (Properties.GetObject(PropBindingManager) == null && ParentInternal != null && !controlIsAlreadyCreated) {
                // We do not want to call our parent's BindingContext property here.
                // We have no idea if us or any of our children are using data binding,
                // and invoking the property would just create the binding manager, which
                // we don't need.  We just blindly notify that the binding manager has
                // changed, and if anyone cares, they will do the comparison at that time.
                OnBindingContextChanged(EventArgs.Empty);
            }
        }

        /// <devdoc>
        ///     Forces the creation of the control. This includes the creation of the handle,
        ///     and any child controls.
        /// <param name='fIgnoreVisible'>
        ///     Determines whether we should create the handle after checking the Visible
        ///     property of the control or not.
        /// </param>
        /// </devdoc>
        internal void CreateControl(bool fIgnoreVisible) {
            bool ready = (state & STATE_CREATED) == 0;

            // PERF: Only "create" the control if it is
            //     : visible. This has the effect of delayed handle creation of
            //     : hidden controls.
            //
            ready = ready && Visible;

            if (ready || fIgnoreVisible) {
                state |= STATE_CREATED;
                bool createdOK = false;
                try {
                    if (!IsHandleCreated) CreateHandle();

                    // must snapshot this array because
                    // z-order updates from Windows may rearrange it!
                    //
                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                    if (controlsCollection != null) {
                        Control[] controlSnapshot = new Control[controlsCollection.Count];
                        controlsCollection.CopyTo(controlSnapshot, 0);

                        foreach(Control ctl in controlSnapshot) {
                            if (ctl.IsHandleCreated) {
                                ctl.SetParentHandle(Handle);
                            }
                            ctl.CreateControl(fIgnoreVisible);
                        }
                    }

                    createdOK = true;
                }
                finally {
                    if (!createdOK)
                        state &= (~STATE_CREATED);
                }
                OnCreateControl();
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DefWndProc"]/*' />
        /// <devdoc>
        ///     Sends the message to the default window proc.
        /// </devdoc>
        /* Primarily here for Form to override */
        [
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void DefWndProc(ref Message m) {
            window.DefWndProc(ref m);
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DestroyHandle"]/*' />
        /// <devdoc>
        ///     Destroys the handle associated with this control. Inheriting classes should
        ///     always call base.destroyHandle.
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void DestroyHandle() {

            if (RecreatingHandle) {
                if (threadCallbackList != null) {
                    // See if we have a thread marshaling request pending.  If so, we will need to
                    // re-post it after recreating the handle.
                    //
                    lock (threadCallbackList) {
                        if (threadCallbackMessage != 0) {
                            NativeMethods.MSG msg = new NativeMethods.MSG();
                            if (UnsafeNativeMethods.PeekMessage(ref msg, new HandleRef(this, Handle), threadCallbackMessage,
                                                          threadCallbackMessage, NativeMethods.PM_NOREMOVE)) {

                                SetState(STATE_THREADMARSHALLPENDING, true);
                            }
                        }
                    }
                }
            }

            // If we're not recreating the handle, then any items in the thread callback list will
            // be orphaned.  An orphaned item is bad, because it will cause the thread to never
            // wake up.  So, we put exceptions into all these items and wake up all threads.
            // If we are recreating the handle, then we're fine because recreation will re-post
            // the thread callback message to the new handle for us.
            //
            if (!RecreatingHandle) {                
                if (threadCallbackList != null) {
                    lock (threadCallbackList) {
                        Exception ex = new System.ObjectDisposedException(GetType().Name);

                        while (threadCallbackList.Count > 0) {
                            ThreadMethodEntry entry = (ThreadMethodEntry)threadCallbackList.Dequeue();
                            entry.exception = ex;
                            entry.Complete();
                        }
                    }
                }
            }

            if (0 != (NativeMethods.WS_EX_MDICHILD & (int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(window, InternalHandle), NativeMethods.GWL_EXSTYLE))) {
                UnsafeNativeMethods.DefMDIChildProc(InternalHandle, NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            else {
                window.DestroyHandle();
            }

            trackMouseEvent = null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Dispose"]/*' />
        /// <devdoc>
        ///    <para>Disposes of the resources (other than memory) used by the
        ///    <see cref='System.Windows.Forms.Control'/>
        ///    .</para>
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (GetState(STATE_OWNCTLBRUSH)) {
                object backBrush = Properties.GetObject(PropBackBrush);
                if (backBrush != null) {
                    IntPtr p = (IntPtr)backBrush;
                    if (p != IntPtr.Zero) {
                        SafeNativeMethods.DeleteObject(new HandleRef(this, p));
                    }
                    Properties.SetObject(PropBackBrush, null);
                }
            }

            //set reflectparent = null regardless of whether we are in the finalizer thread or not.
            UpdateReflectParent(false);
            if (disposing) {
                if (GetState(STATE_DISPOSING)) {
                    return;
                }

                if (GetState(STATE_CREATINGHANDLE)) {
                    throw new InvalidOperationException(string.Format(SR.ClosingWhileCreatingHandle, "Dispose"));
                    // I imagine most subclasses will get themselves in a half disposed state
                    // if this exception is thrown, but things will be equally broken if we ignore this error,
                    // and this way at least the user knows what they did wrong.
                }

                SetState(STATE_DISPOSING, true);
                this.SuspendLayout();
                try {
                    DisposeAxControls();

                    ContextMenu contextMenu = (ContextMenu)Properties.GetObject(PropContextMenu);
                    if (contextMenu != null) {
                        contextMenu.Disposed -= new EventHandler(DetachContextMenu);
                    }

                    ResetBindings();

                    if (IsHandleCreated) DestroyHandle();

                    if (parent != null) {
                        parent.Controls.Remove(this);
                    }

                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                    if (controlsCollection != null) {

                        // PERFNOTE: This is more efficient than using Foreach.  Foreach
                        // forces the creation of an array subset enum each time we
                        // enumerate
                        for(int i = 0; i < controlsCollection.Count; i++) {
                            Control ctl = controlsCollection[i];
                            ctl.parent = null;
                            ctl.Dispose();
                        }
                        Properties.SetObject(PropControlsCollection, null);
                    }

                    base.Dispose(disposing);
                }
                finally {
                    this.ResumeLayout(false);
                    SetState(STATE_DISPOSING, false);
                    SetState(STATE_DISPOSED, true);
                }
            }
            else {

#if FINALIZATION_WATCH
                if (!GetState(STATE_DISPOSED)) {
                    Debug.Fail("Control of type '" + GetType().FullName +"' is being finalized that wasn't disposed\n" + allocationSite);
                }
#endif
                // This same post is done in NativeWindow's finalize method, so if you change
                // it, change it there too.
                //
                if (window != null) {
                    window.ForceExitMessageLoop();
                }
                base.Dispose(disposing);
            }
        }

        // Package scope to allow AxHost to override.
        //
        internal virtual void DisposeAxControls() {
            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].DisposeAxControls();
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DoDragDrop"]/*' />
        /// <devdoc>
        ///     Begins a drag operation. The allowedEffects determine which
        ///     drag operations can occur. If the drag operation needs to interop
        ///     with applications in another process, data should either be
        ///     a base managed class (String, Bitmap, or Metafile) or some Object
        ///     that implements System.Runtime.Serialization.ISerializable. data can also be any Object that
        ///     implements System.Windows.Forms.IDataObject.
        /// </devdoc>
        [UIPermission(SecurityAction.Demand, Clipboard=UIPermissionClipboard.OwnClipboard)]
        public DragDropEffects DoDragDrop(Object data, DragDropEffects allowedEffects) {            
            int[] finalEffect = new int[] {(int)DragDropEffects.None};
            UnsafeNativeMethods.IOleDropSource dropSource = new DropSource( this );
            IComDataObject dataObject = null;

            if (data is IComDataObject) {
                dataObject = (IComDataObject)data;
            }
            else {

                DataObject iwdata = null;
                if (data is IDataObject) {
                    iwdata = new DataObject((IDataObject)data);
                }
                else {
                    iwdata = new DataObject();
                    iwdata.SetData(data);
                }
                dataObject = (IComDataObject)iwdata;
            }

            try {
                SafeNativeMethods.DoDragDrop(dataObject, dropSource, (int)allowedEffects, finalEffect);
            }
            catch (Exception e) {
                if (ClientUtils.IsSecurityOrCriticalException(e)) {
                    throw;
                }
            }

            return(DragDropEffects)finalEffect[0];
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.DrawToBitmap"]/*' />
        /// <devdoc>
        //      Trinity are currently calling IViewObject::Draw in order to render controls on Word & Excel
        //      before they are in place active. However this method is private and they need a public way to do this.
        //      This means that we will need to add a public method to control that supports rendering to a Bitmap:
        //      public virtual void DrawToBitmap(Bitmap bmp, RectangleF targetBounds)
        //      where target bounds is the bounds within which the control should render.
        /// </devdoc>
        [
            SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly"),
            SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters"),          // Using Bitmap instead of Image intentionally
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters"), // targetBounds is the name of the param passed in.
                                                                                                         // So we don't have to localize it.
            UIPermission(SecurityAction.Demand, Window=UIPermissionWindow.AllWindows)
        ]
        public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds) {

            if (bitmap == null) {
                throw new ArgumentNullException(nameof(bitmap));
            }

            if (targetBounds.Width <= 0 || targetBounds.Height <= 0
                || targetBounds.X < 0 || targetBounds.Y < 0) {
                throw new ArgumentException(nameof(targetBounds));
            }

            if (!IsHandleCreated) {
                CreateHandle();
            }


            int width = Math.Min(this.Width, targetBounds.Width);
            int height = Math.Min(this.Height, targetBounds.Height);

            using (Bitmap image = new Bitmap(width, height, bitmap.PixelFormat)) {
                using (Graphics g = Graphics.FromImage(image)) {
                    IntPtr hDc = g.GetHdc();
                    //send the actual wm_print message
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.WM_PRINT, (IntPtr)hDc,
                        (IntPtr)(NativeMethods.PRF_CHILDREN | NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND | NativeMethods.PRF_NONCLIENT));

                    //now BLT the result to the destination bitmap.
                    using (Graphics destGraphics = Graphics.FromImage(bitmap)) {
                        IntPtr desthDC = destGraphics.GetHdc();
                        SafeNativeMethods.BitBlt(new HandleRef(destGraphics, desthDC), targetBounds.X, targetBounds.Y, width, height,
                                                 new HandleRef(g, hDc), 0, 0, 0xcc0020);
                        destGraphics.ReleaseHdcInternal(desthDC);
                    }

                    g.ReleaseHdcInternal(hDc);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.EndInvoke"]/*' />
        /// <devdoc>
        ///     Retrieves the return value of the asynchronous operation
        ///     represented by the IAsyncResult interface passed. If the
        ///     async operation has not been completed, this function will
        ///     block until the result is available.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public Object EndInvoke(IAsyncResult asyncResult) {
            using (new MultithreadSafeCallScope())
            {
                if (asyncResult == null) {
                    throw new ArgumentNullException(nameof(asyncResult));
                }

                ThreadMethodEntry entry = asyncResult as ThreadMethodEntry;
                if (entry == null) {
                    throw new ArgumentException(SR.ControlBadAsyncResult,"asyncResult");
                }
                Debug.Assert(this == entry.caller, "Called BeginInvoke on one control, and the corresponding EndInvoke on a different control");

                if (!asyncResult.IsCompleted) {
                    int pid; // ignored
                    Control marshaler = FindMarshalingControl();
                    if (SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(marshaler, marshaler.Handle), out pid) == SafeNativeMethods.GetCurrentThreadId()) {
                        marshaler.InvokeMarshaledCallbacks();
                    }
                    else {
                        marshaler = entry.marshaler;
                        marshaler.WaitForWaitHandle(asyncResult.AsyncWaitHandle);
                    }
                }

                Debug.Assert(asyncResult.IsCompleted, "Why isn't this asyncResult done yet?");
                if (entry.exception != null) {
                    throw entry.exception;
                }
                return entry.retVal;
            }
        }

        internal bool EndUpdateInternal() {
            return EndUpdateInternal(true);
        }

        internal bool EndUpdateInternal(bool invalidate) {
            if (updateCount > 0) {
                Debug.Assert(IsHandleCreated, "Handle should be created by now");
                updateCount--;
                if (updateCount == 0) {
                    SendMessage(NativeMethods.WM_SETREDRAW, -1, 0);
                    if (invalidate) {
                        Invalidate();
                    }

                }
                return true;
            }
            else {
                return false;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.FindForm"]/*' />
        /// <devdoc>
        ///     Retrieves the form that this control is on. The control's parent
        ///     may not be the same as the form.
        /// </devdoc>
        [UIPermission(SecurityAction.Demand, Window=UIPermissionWindow.AllWindows)]
        public Form FindForm() {
            return FindFormInternal();
        }

        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal Form FindFormInternal() {
            Control cur = this;
            while (cur != null && !(cur is Form) ) {
                cur = cur.ParentInternal;
            }
            return (Form)cur;
        }

        /// <devdoc>
        ///     Attempts to find a control Object that we can use to marshal
        ///     calls.  We must marshal calls to a control with a window
        ///     handle, so we traverse up the parent chain until we find one.
        ///     Failing that, we just return ouselves.
        /// </devdoc>
        /// <internalonly/>
        private Control FindMarshalingControl() {
            // 
            lock(this) {
                Control c = this;

                while (c != null && !c.IsHandleCreated) {
                    Control p = c.ParentInternal;
                    c = p;
                }

                if (c == null) {
                    // No control with a created handle.  We
                    // just use our own control.  MarshaledInvoke
                    // will throw an exception because there
                    // is no handle.
                    //
                    c = this;
                }
                else {
                    Debug.Assert(c.IsHandleCreated, "FindMarshalingControl chose a bad control.");
                }

                return(Control)c;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetTopLevel"]/*' />
        protected bool GetTopLevel() {
            return(state & STATE_TOPLEVEL) != 0;
        }

        /// <devdoc>
        ///     Used by AxHost to fire the CreateHandle event.
        /// </devdoc>
        /// <internalonly/>
        internal void RaiseCreateHandleEvent(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EventHandleCreated];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RaiseKeyEvent"]/*' />
        /// <devdoc>
        ///     Raises the event associated with key with the event data of
        ///     e and a sender of this control.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseKeyEvent(object key, KeyEventArgs e) {
            KeyEventHandler handler = (KeyEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RaiseMouseEvent"]/*' />
        /// <devdoc>
        ///     Raises the event associated with key with the event data of
        ///     e and a sender of this control.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseMouseEvent(object key, MouseEventArgs e) {
            MouseEventHandler handler = (MouseEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Focus"]/*' />
        /// <devdoc>
        ///    <para> Sets focus to the control.</para>
        ///    <para>Attempts to set focus to this control.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool Focus() {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Control::Focus - " + this.Name);
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ModifyFocus Demanded");
            IntSecurity.ModifyFocus.Demand();

            //here, we call our internal method (which form overrides)
            //see comments in FocusInternal
            //
            return FocusInternal();
        }

        /// <devdoc>
        ///    Internal method for setting focus to the control.
        ///    Form overrides this method - because MDI child forms
        ///    need to be focused by calling the MDIACTIVATE message.
        /// </devdoc>
        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal virtual bool FocusInternal() {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Control::FocusInternal - " + this.Name);
            if (CanFocus){
                UnsafeNativeMethods.SetFocus(new HandleRef(this, Handle));
            }
            if (Focused && this.ParentInternal != null) {
                IContainerControl c = this.ParentInternal.GetContainerControlInternal();

                if (c != null) {
                    if (c is ContainerControl) {
                        ((ContainerControl)c).SetActiveControlInternal(this);
                    }
                    else {
                        c.ActiveControl = this;
                    }
                }
            }


            return Focused;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.FromChildHandle"]/*' />
        /// <devdoc>
        ///     Returns the control that is currently associated with handle.
        ///     This method will search up the HWND parent chain until it finds some
        ///     handle that is associated with with a control. This method is more
        ///     robust that fromHandle because it will correctly return controls
        ///     that own more than one handle.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Control FromChildHandle(IntPtr handle) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ControlFromHandleOrLocation Demanded");
            IntSecurity.ControlFromHandleOrLocation.Demand();
            return FromChildHandleInternal(handle);
        }

        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal static Control FromChildHandleInternal(IntPtr handle) {
            while (handle != IntPtr.Zero) {
                Control ctl = FromHandleInternal(handle);
                if (ctl != null) return ctl;
                handle = UnsafeNativeMethods.GetAncestor(new HandleRef(null, handle), NativeMethods.GA_PARENT);
            }
            return null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.FromHandle"]/*' />
        /// <devdoc>
        ///     Returns the control that is currently associated with handle.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Control FromHandle(IntPtr handle) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ControlFromHandleOrLocation Demanded");
            IntSecurity.ControlFromHandleOrLocation.Demand();
            return FromHandleInternal(handle);
        }

        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal static Control FromHandleInternal(IntPtr handle) {
            NativeWindow w = NativeWindow.FromHandle(handle);
            while (w != null && !(w is ControlNativeWindow)) {
                w = w.PreviousWindow;
            }

            if (w is ControlNativeWindow) {
                return((ControlNativeWindow)w).GetControl();
            }
            return null;
        }

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal Size ApplySizeConstraints(int width, int height) {
            return ApplyBoundsConstraints(0,0,width, height).Size;
        }


        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal Size ApplySizeConstraints(Size proposedSize) {
            return ApplyBoundsConstraints(0,0,proposedSize.Width,proposedSize.Height).Size;
        }

        internal virtual Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight) {

            // COMPAT: in Everett we would allow you to set negative values in pre-handle mode
            // in Whidbey, if you've set Min/Max size we will constrain you to 0,0.  Everett apps didnt 
            // have min/max size on control, which is why this works.
            if (MaximumSize != Size.Empty || MinimumSize != Size.Empty) {
                Size maximumSize = LayoutUtils.ConvertZeroToUnbounded(MaximumSize);
                Rectangle newBounds = new Rectangle(suggestedX, suggestedY, 0,0);
      
                // Clip the size to maximum and inflate it to minimum as necessary.
                newBounds.Size = LayoutUtils.IntersectSizes(new Size(proposedWidth,proposedHeight), maximumSize);
                newBounds.Size = LayoutUtils.UnionSizes(newBounds.Size, MinimumSize);
      
                return newBounds;
            }

            return new Rectangle(suggestedX, suggestedY, proposedWidth, proposedHeight);
        }

        

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetChildAtPoint"]/*' />
        /// <devdoc>
        ///     Retrieves the child control that is located at the specified client
        ///     coordinates.
        /// </devdoc>
        public Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue) {
            int value = (int)skipValue;
            // Since this is a Flags Enumeration... the only way to validate skipValue is by checking if its within the range.
            if(value < 0 || value > 7)
            {
                throw new InvalidEnumArgumentException(nameof(skipValue), value, typeof(GetChildAtPointSkip));
            }

            IntPtr hwnd = UnsafeNativeMethods.ChildWindowFromPointEx(new HandleRef(null, Handle), pt.X, pt.Y, value);

            // Security Reviewed
            // While doing a security review it was noticed that GetChildAtPoint
            // does work to ensure that you can only gain access to children of your own control,
            // but the methods it uses to determine the children demand all window permission first,
            // negating the extra check.
            // It is OK to return child windows for children within your own control for semitrust.

            // Hence  calling the Internal methods to ByPass the Security Demand...
            // for IntSecurity.ControlFromHandleOrLocation == ALLWindows.

            Control ctl = FromChildHandleInternal(hwnd);
            if (ctl != null && !IsDescendant(ctl)) {
                Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ControlFromHandleOrLocation Demanded");
                IntSecurity.ControlFromHandleOrLocation.Demand();
            }

            return(ctl == this) ? null : ctl;
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetChildAtPoint"]/*' />
        /// <devdoc>
        ///     Retrieves the child control that is located at the specified client
        ///     coordinates.
        /// </devdoc>
        public Control GetChildAtPoint(Point pt) {
            return GetChildAtPoint(pt, GetChildAtPointSkip.None);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetContainerControl"]/*' />
        /// <devdoc>
        ///     Returns the closest ContainerControl in the control's chain of
        ///     parent controls and forms.
        /// </devdoc>
        public IContainerControl GetContainerControl() {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "GetParent Demanded");
            IntSecurity.GetParent.Demand();
            return GetContainerControlInternal();
        }

        private static bool IsFocusManagingContainerControl(Control ctl) {
            return ((ctl.controlStyle & ControlStyles.ContainerControl) == ControlStyles.ContainerControl && ctl is IContainerControl);
        }

    /// <devdoc>
        ///     This new Internal method checks the updateCount to signify that the control is within the "BeginUpdate" and "EndUpdate" cycle.
        ///     Check out : for usage of this. The Treeview tries to ForceUpdate the scrollbars by calling "WM_SETREDRAW"
        ///     even if the control in "Begin - End" update cycle. Using thie Function we can guard against repetitively redrawing the control.
        /// </devdoc>
        internal bool IsUpdating()
        {
            return (updateCount > 0);
        }

        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal IContainerControl GetContainerControlInternal() {
            Control c = this;
            
            // Refer to IsContainerControl property for more details.            
            if (c != null && IsContainerControl) 
            {
                c = c.ParentInternal;
            }
            while (c != null && (!IsFocusManagingContainerControl(c))) {
                c = c.ParentInternal;
            }
            return (IContainerControl)c;
        }

        // Essentially an Hfont; see inner class for details.
        private static FontHandleWrapper GetDefaultFontHandleWrapper() {
            if (defaultFontHandleWrapper == null) {
                defaultFontHandleWrapper = new FontHandleWrapper(DefaultFont);
            }

            return defaultFontHandleWrapper;
        }

        internal IntPtr GetHRgn(Region region) {
            Graphics graphics = CreateGraphicsInternal();
            IntPtr handle = region.GetHrgn(graphics);
            System.Internal.HandleCollector.Add(handle, NativeMethods.CommonHandles.GDI);
            graphics.Dispose();
            return handle;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetScaledBounds"]/*' />
        /// <devdoc>
        ///     This is a helper method that is called by ScaleControl to retrieve the bounds
        ///     that the control should be scaled by.  You may override this method if you
        ///     wish to reuse ScaleControl's scaling logic but you need to supply your own
        ///     bounds.  The default implementation returns scaled bounds that take into
        ///     account the BoundsSpecified, whether the control is top level, and whether
        ///     the control is fixed width or auto size, and any adornments the control may have.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified) {

            // We should not include the window adornments in our calculation,
            // because windows scales them for us.
            NativeMethods.RECT adornments = new NativeMethods.RECT(0, 0, 0, 0);
            CreateParams cp = CreateParams;
            AdjustWindowRectEx(ref adornments, cp.Style, HasMenu, cp.ExStyle);

            float dx = factor.Width;
            float dy = factor.Height;

            int sx = bounds.X;
            int sy = bounds.Y;

            // Don't reposition top level controls.  Also, if we're in
            // design mode, don't reposition the root component.
            bool scaleLoc = !GetState(STATE_TOPLEVEL);
            if (scaleLoc) {
                ISite site = Site;
                if (site != null && site.DesignMode) {
                    IDesignerHost host = site.GetService(typeof(IDesignerHost)) as IDesignerHost;
                    if (host != null && host.RootComponent == this) {
                        scaleLoc = false;
                    }
                }
            }

            if (scaleLoc) {
                if ((specified & BoundsSpecified.X) != 0) {
                    sx = (int)Math.Round(bounds.X * dx);
                }

                if ((specified & BoundsSpecified.Y) != 0) {
                    sy = (int)Math.Round(bounds.Y * dy);
                }
            }

            int sw = bounds.Width;
            int sh = bounds.Height;

            // Do this even for auto sized controls.  They'll "snap back", but it is important to size them in case
            // they are anchored.
            if ((controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth && (specified & BoundsSpecified.Width) != 0) {
                int adornmentWidth = (adornments.right - adornments.left);
                int localWidth = bounds.Width - adornmentWidth;
                sw = (int)Math.Round(localWidth * dx) + adornmentWidth;
            }
            if ((controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight && (specified & BoundsSpecified.Height) != 0) {
                int adornmentHeight = (adornments.bottom - adornments.top);
                int localHeight = bounds.Height - adornmentHeight;
                sh = (int)Math.Round(localHeight * dy) + adornmentHeight;
            }

            return new Rectangle(sx, sy, sw, sh);
        }

        private MouseButtons GetXButton(int wparam) {
            switch (wparam) {
            case NativeMethods.XBUTTON1:
                return MouseButtons.XButton1;
            case NativeMethods.XBUTTON2:
                return MouseButtons.XButton2;
            }
            Debug.Fail("Unknown XButton: " + wparam);
            return MouseButtons.None;
        }

        internal virtual bool GetVisibleCore() {
            // We are only visible if our parent is visible
            if (!GetState(STATE_VISIBLE))
                return false;
            else if (ParentInternal == null)
                return true;
            else
                return ParentInternal.GetVisibleCore();
        }

        internal bool GetAnyDisposingInHierarchy() {
            Control up = this;
            bool isDisposing = false;
            while (up != null) {
                if (up.Disposing) {
                    isDisposing = true;
                    break;
                }
                up = up.parent;
            }
            return isDisposing;
        }

        private MenuItem GetMenuItemFromHandleId(IntPtr hmenu, int item) {
            MenuItem mi = null;
            int id = UnsafeNativeMethods.GetMenuItemID(new HandleRef(null, hmenu), item);
            if (id == unchecked((int)0xFFFFFFFF)) {
                IntPtr childMenu = IntPtr.Zero;
                childMenu = UnsafeNativeMethods.GetSubMenu(new HandleRef(null, hmenu), item);
                int count = UnsafeNativeMethods.GetMenuItemCount(new HandleRef(null, childMenu));
                MenuItem found = null;
                for (int i=0; i<count; i++) {
                    found = GetMenuItemFromHandleId(childMenu, i);
                    if (found != null) {
                        Menu parent = found.Parent;
                        if (parent != null && parent is MenuItem) {
                            found = (MenuItem)parent;
                            break;
                        }
                        found = null;
                    }
                }

                mi = found;
            }
            else {
                Command cmd = Command.GetCommandFromID(id);
                if (cmd != null) {
                    Object reference = cmd.Target;
                    if (reference != null && reference is MenuItem.MenuItemData) {
                        mi = ((MenuItem.MenuItemData)reference).baseItem;
                    }
                }
            }
            return mi;
        }

        /// <devdoc>
        ///     - Returns child controls sorted according to their TabIndex property order.
        ///     - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///     - Returns a TabIndex sorted array of ControlTabOrderHolder objects.
        /// </devdoc>
        private ArrayList GetChildControlsTabOrderList(bool handleCreatedOnly) {
            ArrayList holders = new ArrayList();

            foreach (Control c in Controls) {
                if (!handleCreatedOnly || c.IsHandleCreated) {
                    holders.Add(new ControlTabOrderHolder(holders.Count, c.TabIndex, c));
                }
            }

            holders.Sort(new ControlTabOrderComparer());

            return holders;
        }

        /// <devdoc>
        ///     - Returns native child windows sorted according to their TabIndex property order.
        ///     - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///     - Child windows with no corresponding Control objects (and therefore no discernable TabIndex)
        ///       are sorted to the front of the list (but remain in relative z-order to one another).
        ///     - This version returns a sorted array of integers, representing the original z-order
        ///       based indexes of the native child windows.
        /// </devdoc>
        private int[] GetChildWindowsInTabOrder() {
            ArrayList holders = GetChildWindowsTabOrderList();

            int[] indexes = new int[holders.Count];

            for (int i = 0; i < holders.Count; i++) {
                indexes[i] = ((ControlTabOrderHolder) holders[i]).oldOrder;
            }

            return indexes;
        }

        /// <devdoc>
        ///     - Returns child controls sorted according to their TabIndex property order.
        ///     - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///     - This version returns a sorted array of control references.
        /// </devdoc>
        internal Control[] GetChildControlsInTabOrder(bool handleCreatedOnly) {
            ArrayList holders = GetChildControlsTabOrderList(handleCreatedOnly);

            Control[] ctls = new Control[holders.Count];

            for (int i = 0; i < holders.Count; i++) {
                ctls[i] = ((ControlTabOrderHolder) holders[i]).control;
            }

            return ctls;
        }

        /// <devdoc>
        ///      This class contains a control and associates it with a z-order.
        ///      This is used when sorting controls based on tab index first,
        ///      z-order second.
        /// </devdoc>
        private class ControlTabOrderHolder {
            internal readonly int oldOrder;
            internal readonly int newOrder;
            internal readonly Control control;

            internal ControlTabOrderHolder(int oldOrder, int newOrder, Control control) {
                this.oldOrder = oldOrder;
                this.newOrder = newOrder;
                this.control  = control;
            }
        }

        /// <devdoc>
        ///      Used to sort controls based on tab index and z-order.
        /// </devdoc>
        private class ControlTabOrderComparer : IComparer {
            int IComparer.Compare(Object x, Object y) {
                ControlTabOrderHolder hx = (ControlTabOrderHolder) x;
                ControlTabOrderHolder hy = (ControlTabOrderHolder) y;

                int delta = hx.newOrder - hy.newOrder;
                if (delta == 0)
                    delta = hx.oldOrder - hy.oldOrder;

                return delta;
            }
        }

        /// <devdoc>
        ///     Given a native window handle, returns array of handles to window's children (in z-order).
        /// </devdoc>
        private static ArrayList GetChildWindows(IntPtr hWndParent) {
            ArrayList windows = new ArrayList();

            for (IntPtr hWndChild = UnsafeNativeMethods.GetWindow(new HandleRef(null, hWndParent), NativeMethods.GW_CHILD);
                 hWndChild != IntPtr.Zero;
                 hWndChild = UnsafeNativeMethods.GetWindow(new HandleRef(null, hWndChild), NativeMethods.GW_HWNDNEXT)) {
                windows.Add(hWndChild);
            }

            return windows;
        }

        /// <devdoc>
        ///     - Returns native child windows sorted according to their TabIndex property order.
        ///     - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///     - Child windows with no corresponding Control objects (and therefore no discernable TabIndex)
        ///       are sorted to the front of the list (but remain in relative z-order to one another).
        ///     - Returns a TabIndex sorted array of ControlTabOrderHolder objects.
        /// </devdoc>
        private ArrayList GetChildWindowsTabOrderList() {
            ArrayList holders = new ArrayList();
            ArrayList windows = GetChildWindows(this.Handle);

            foreach (IntPtr hWnd in windows) {
                Control ctl = FromHandleInternal(hWnd);
                int tabIndex = (ctl == null) ? -1 : ctl.TabIndex;
                holders.Add(new ControlTabOrderHolder(holders.Count, tabIndex, ctl));
            }

            holders.Sort(new ControlTabOrderComparer());

            return holders;
        }

        // 

        internal virtual Control GetFirstChildControlInTabOrder(bool forward) {
         
            ControlCollection ctlControls = (ControlCollection)this.Properties.GetObject(PropControlsCollection);
 
            Control found = null;
            if (ctlControls != null) {
                 if (forward) { 
                    for (int c = 0; c < ctlControls.Count; c++) {
                        if (found == null || found.tabIndex > ctlControls[c].tabIndex) {
                            found = ctlControls[c];
                        }
                    }
                 }
                 else {
                    
                    // Cycle through the controls in reverse z-order looking for the one with the highest
                    // tab index.
                    //
                    for (int c = ctlControls.Count - 1; c >= 0; c--) {
                        if (found == null || found.tabIndex < ctlControls[c].tabIndex) {
                            found = ctlControls[c];
                        }
                    }
                 }
            }
            return found;

        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetNextControl"]/*' />
        /// <devdoc>
        ///     Retrieves the next control in the tab order of child controls.
        /// </devdoc>
        public Control GetNextControl(Control ctl, bool forward) {
            if (!Contains(ctl)) {
                ctl = this;
            }

            if (forward) {
                ControlCollection ctlControls = (ControlCollection)ctl.Properties.GetObject(PropControlsCollection);

                if (ctlControls != null && ctlControls.Count > 0 && (ctl == this || !IsFocusManagingContainerControl(ctl))) {
                    Control found = ctl.GetFirstChildControlInTabOrder(/*forward=*/true);
                    if (found != null) {
                        return found;
                    }
                }

                while (ctl != this) {
                    int targetIndex = ctl.tabIndex;
                    bool hitCtl = false;
                    Control found = null;
                    Control p = ctl.parent;

                    // Cycle through the controls in z-order looking for the one with the next highest
                    // tab index.  Because there can be dups, we have to start with the existing tab index and
                    // remember to exclude the current control.
                    //
                    int parentControlCount = 0;

                    ControlCollection parentControls = (ControlCollection)p.Properties.GetObject(PropControlsCollection);

                    if (parentControls != null) {
                        parentControlCount = parentControls.Count;
                    }

                    for (int c = 0; c < parentControlCount; c++) {

                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // caluses:

                        // We are not interested in ourself.
                        //
                        if (parentControls[c] != ctl) {

                            // We are interested in controls with >= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            //
                            if (parentControls[c].tabIndex >= targetIndex) {

                                // Check to see if this control replaces the "best match" we've already
                                // found.
                                //
                                if (found == null || found.tabIndex > parentControls[c].tabIndex) {

                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    //
                                    if (parentControls[c].tabIndex != targetIndex || hitCtl) {
                                        found = parentControls[c];
                                    }
                                }
                            }
                        }
                        else {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            //
                            hitCtl = true;
                        }
                    }

                    if (found != null) {
                        return found;
                    }

                    ctl = ctl.parent;
                }
            }
            else {
                if (ctl != this) {

                    int targetIndex = ctl.tabIndex;
                    bool hitCtl = false;
                    Control found = null;
                    Control p = ctl.parent;

                    // Cycle through the controls in reverse z-order looking for the next lowest tab index.  We must
                    // start with the same tab index as ctl, because there can be dups.
                    //
                    int parentControlCount = 0;

                    ControlCollection parentControls = (ControlCollection)p.Properties.GetObject(PropControlsCollection);

                    if (parentControls != null) {
                        parentControlCount = parentControls.Count;
                    }

                    for (int c = parentControlCount - 1; c >= 0; c--) {

                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // caluses:

                        // We are not interested in ourself.
                        //
                        if (parentControls[c] != ctl) {

                            // We are interested in controls with <= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            //
                            if (parentControls[c].tabIndex <= targetIndex) {

                                // Check to see if this control replaces the "best match" we've already
                                // found.
                                //
                                if (found == null || found.tabIndex < parentControls[c].tabIndex) {

                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    //
                                    if (parentControls[c].tabIndex != targetIndex || hitCtl) {
                                        found = parentControls[c];
                                    }
                                }
                            }
                        }
                        else {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            //
                            hitCtl = true;
                        }
                    }

                    // If we were unable to find a control we should return the control's parent.  However, if that parent is us, return
                    // NULL.
                    //
                    if (found != null) {
                        ctl = found;
                    }
                    else {
                        if (p == this) {
                            return null;
                        }
                        else {
                            return p;
                        }
                    }
                }

                // We found a control.  Walk into this control to find the proper sub control within it to select.
                //
                ControlCollection ctlControls = (ControlCollection)ctl.Properties.GetObject(PropControlsCollection);

                while (ctlControls != null && ctlControls.Count > 0 && (ctl == this || !IsFocusManagingContainerControl(ctl))) {
                    Control found = ctl.GetFirstChildControlInTabOrder(/*forward=*/false);
                    if (found != null) {
                        ctl = found;
                        ctlControls = (ControlCollection)ctl.Properties.GetObject(PropControlsCollection);
                    }
                    else {
                        break;
                    }
                }
            }

            return ctl == this? null: ctl;
        }

        /// <devdoc>
        ///     Return ((Control) window).Handle if window is a Control.
        ///     Otherwise, demands permission for AllWindows and returns window.Handle
        /// </devdoc>
        internal static IntPtr GetSafeHandle(IWin32Window window)
        {
            Debug.Assert(window != null, "window is null in Control.GetSafeHandle");
            IntPtr hWnd = IntPtr.Zero;
            Control control = window as Control;
            if (control != null) {
                hWnd = control.Handle;
                Debug.Assert(hWnd == IntPtr.Zero || UnsafeNativeMethods.IsWindow(new HandleRef(null, hWnd)));
                return hWnd;
            }
            else {
                IntSecurity.AllWindows.Demand();
                hWnd = window.Handle;
                if (hWnd == IntPtr.Zero || UnsafeNativeMethods.IsWindow(new HandleRef(null, hWnd))) {
                    return hWnd;
                }
                else {
                    throw new Win32Exception(NativeMethods.ERROR_INVALID_HANDLE);
                }
            }
        }

        /// <devdoc>
        ///     Retrieves the current value of the specified bit in the control's state.
        /// </devdoc>
        internal bool GetState(int flag) {
            return(state & flag) != 0;
        }

        /// <devdoc>
        ///     Retrieves the current value of the specified bit in the control's state2.
        /// </devdoc>
        private bool GetState2(int flag)
        {
            return (state2 & flag) != 0;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.GetStyle"]/*' />
        /// <devdoc>
        ///     Retrieves the current value of the specified bit in the control's style.
        ///     NOTE: This is control style, not the Win32 style of the hWnd.
        /// </devdoc>
        protected bool GetStyle(ControlStyles flag) {
            return (controlStyle & flag) == flag;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Hide"]/*' />
        /// <devdoc>
        ///     Hides the control by setting the visible property to false;
        /// </devdoc>
        public void Hide() {
            Visible = false;
        }


        /// <devdoc>
        ///     Sets up the TrackMouseEvent for listening for the
        ///     mouse leave event.
        /// </devdoc>
        /// <internalonly/>
        private void HookMouseEvent() {
            if (!GetState(STATE_TRACKINGMOUSEEVENT)) {
                SetState(STATE_TRACKINGMOUSEEVENT, true);

                if (trackMouseEvent == null) {
                    trackMouseEvent = new NativeMethods.TRACKMOUSEEVENT();
                    trackMouseEvent.dwFlags = NativeMethods.TME_LEAVE | NativeMethods.TME_HOVER;
                    trackMouseEvent.hwndTrack = Handle;
                }

                SafeNativeMethods.TrackMouseEvent(trackMouseEvent);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InitLayout"]/*' />
        /// <devdoc>
        ///     Called after the control has been added to another container.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void InitLayout() {
            LayoutEngine.InitLayout(this, BoundsSpecified.All);
        }

        /// <devdoc>
        ///     This method initializes the scaling bits for this control based on
        ///     the bounds.
        /// </devdoc>
        private void InitScaling(BoundsSpecified specified) {
            requiredScaling |= (byte)((int)specified & RequiredScalingMask);
        }

        // Sets the text and background colors of the DC, and returns the background HBRUSH.
        // This gets called from some variation on WM_CTLCOLOR...
        // Virtual because Scrollbar insists on being different.
        //
        // NOTE: this message may not have originally been sent to this HWND.
        //

        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
        internal virtual IntPtr InitializeDCForWmCtlColor (IntPtr dc, int msg) {
            if (!GetStyle(ControlStyles.UserPaint)) {
                SafeNativeMethods.SetTextColor(new HandleRef(null, dc), ColorTranslator.ToWin32(ForeColor));
                SafeNativeMethods.SetBkColor(new HandleRef(null, dc), ColorTranslator.ToWin32(BackColor));
                return BackColorBrush;
            }
            else {
                return UnsafeNativeMethods.GetStockObject(NativeMethods.HOLLOW_BRUSH);
            }
        }

        #if WIN95_SUPPORT
        /// <devdoc>
        ///     Initializes mouse wheel support. This may involve registering some windows
        ///     messages on older operating systems.
        /// </devdoc>
        /// <internalonly/>
        private void InitMouseWheelSupport() {
            if (!mouseWheelInit) {
                // If we are running on less than NT4 or less that Win98 then we must use
                // the manual mousewheel stuff...
                //
                mouseWheelRoutingNeeded = !SystemInformation.NativeMouseWheelSupport;

                if (mouseWheelRoutingNeeded) {
                    IntPtr hwndMouseWheel = IntPtr.Zero;

                    // Check for the MouseZ "service". This is a little app that generated the
                    // MSH_MOUSEWHEEL messages by monitoring the hardware. If this app isn't
                    // found, then there is no support for MouseWheels on the system.
                    //
                    hwndMouseWheel = UnsafeNativeMethods.FindWindow(NativeMethods.MOUSEZ_CLASSNAME, NativeMethods.MOUSEZ_TITLE);

                    if (hwndMouseWheel != IntPtr.Zero) {

                        // Register the MSH_MOUSEWHEEL message... we look for this in the
                        // wndProc, and treat it just like WM_MOUSEWHEEL.
                        //
                        int message = SafeNativeMethods.RegisterWindowMessage(NativeMethods.MSH_MOUSEWHEEL);

                        if (message != 0) {
                            mouseWheelMessage = message;
                        }
                    }
                }
                mouseWheelInit = true;
            }
        }
        #endif

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidate"]/*' />
        /// <devdoc>
        ///     Invalidates a region of the control and causes a paint message
        ///     to be sent to the control. This will not force a synchronous paint to
        ///     occur, calling update after invalidate will force a
        ///     synchronous paint.
        /// </devdoc>
        public void Invalidate(Region region) {
            Invalidate(region, false);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidate1"]/*' />
        /// <devdoc>
        ///     Invalidates a region of the control and causes a paint message
        ///     to be sent to the control. This will not force a synchronous paint to
        ///     occur, calling update after invalidate will force a
        ///     synchronous paint.
        /// </devdoc>
        public void Invalidate(Region region, bool invalidateChildren) {
            if (region == null) {
                Invalidate(invalidateChildren);
            }
            else if (IsHandleCreated) {
                IntPtr regionHandle = GetHRgn(region);

                try {
                    Debug.Assert(regionHandle != IntPtr.Zero, "Region wasn't null but HRGN is?");
                    if (invalidateChildren) {
                        SafeNativeMethods.RedrawWindow(new HandleRef(this, Handle),
                                                       null, new HandleRef(region, regionHandle),
                                                       NativeMethods.RDW_INVALIDATE |
                                                       NativeMethods.RDW_ERASE |
                                                       NativeMethods.RDW_ALLCHILDREN);
                    }
                    else {
                        // It's safe to invoke InvalidateRgn from a separate thread.
                        using (new MultithreadSafeCallScope())
                        {
                            SafeNativeMethods.InvalidateRgn(new HandleRef(this, Handle),
                                                            new HandleRef(region, regionHandle),
                                                            !GetStyle(ControlStyles.Opaque));
                        }
                    }
                }
                finally {
                    SafeNativeMethods.DeleteObject(new HandleRef(region, regionHandle));
                }

                Rectangle bounds = Rectangle.Empty;

                // gpr: We shouldn't have to create a Graphics for this...
                using (Graphics graphics = CreateGraphicsInternal()) {
                    bounds = Rectangle.Ceiling(region.GetBounds(graphics));
                }
                
                OnInvalidated(new InvalidateEventArgs(bounds));
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidate2"]/*' />
        /// <devdoc>
        ///     Invalidates the control and causes a paint message to be sent to the control.
        ///     This will not force a synchronous paint to occur, calling update after
        ///     invalidate will force a synchronous paint.
        /// </devdoc>
        public void Invalidate() {
            Invalidate(false);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidate3"]/*' />
        /// <devdoc>
        ///     Invalidates the control and causes a paint message to be sent to the control.
        ///     This will not force a synchronous paint to occur, calling update after
        ///     invalidate will force a synchronous paint.
        /// </devdoc>
        public void Invalidate(bool invalidateChildren) {
            if (IsHandleCreated) {
                if (invalidateChildren) {
                    SafeNativeMethods.RedrawWindow(new HandleRef(window, Handle),
                                                   null, NativeMethods.NullHandleRef,
                                                   NativeMethods.RDW_INVALIDATE |
                                                   NativeMethods.RDW_ERASE |
                                                   NativeMethods.RDW_ALLCHILDREN);
                }
                else {
                    // It's safe to invoke InvalidateRect from a separate thread.
                    using (new MultithreadSafeCallScope())
                    {
                        SafeNativeMethods.InvalidateRect(new HandleRef(window, Handle),
                                                         null,
                                                         (controlStyle & ControlStyles.Opaque) != ControlStyles.Opaque);
                    }
                }

                NotifyInvalidate(this.ClientRectangle);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidate4"]/*' />
        /// <devdoc>
        ///     Invalidates a rectangular region of the control and causes a paint message
        ///     to be sent to the control. This will not force a synchronous paint to
        ///     occur, calling update after invalidate will force a
        ///     synchronous paint.
        /// </devdoc>
        public void Invalidate(Rectangle rc) {
            Invalidate(rc, false);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invalidate5"]/*' />
        /// <devdoc>
        ///     Invalidates a rectangular region of the control and causes a paint message
        ///     to be sent to the control. This will not force a synchronous paint to
        ///     occur, calling update after invalidate will force a
        ///     synchronous paint.
        /// </devdoc>
        public void Invalidate(Rectangle rc, bool invalidateChildren) {
            if (rc.IsEmpty) {
                Invalidate(invalidateChildren);
            }
            else if (IsHandleCreated) {
                if (invalidateChildren) {
                    NativeMethods.RECT rcArea = NativeMethods.RECT.FromXYWH(rc.X, rc.Y, rc.Width, rc.Height);
                    SafeNativeMethods.RedrawWindow(new HandleRef(window, Handle),
                                                   ref rcArea, NativeMethods.NullHandleRef,
                                                   NativeMethods.RDW_INVALIDATE |
                                                   NativeMethods.RDW_ERASE |
                                                   NativeMethods.RDW_ALLCHILDREN);
                }
                else {
                    NativeMethods.RECT rcArea =
                        NativeMethods.RECT.FromXYWH(rc.X, rc.Y, rc.Width,
                                                    rc.Height);

                    // It's safe to invoke InvalidateRect from a separate thread.
                    using (new MultithreadSafeCallScope())
                    {
                        SafeNativeMethods.InvalidateRect(new HandleRef(window, Handle),
                                                         ref rcArea,
                                                         (controlStyle & ControlStyles.Opaque) != ControlStyles.Opaque);
                    }
                }
                NotifyInvalidate(rc);
            }
        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invoke"]/*' />
        /// <devdoc>
        ///     Executes the given delegate on the thread that owns this Control's
        ///     underlying window handle.  It is an error to call this on the same thread that
        ///     the control belongs to.  If the control's handle doesn't exist yet, this will
        ///     follow up the control's parent chain until it finds a control or form that does
        ///     have a window handle.  If no appropriate handle can be found, invoke will throw
        ///     an exception.  Exceptions that are raised during the call will be
        ///     propapgated back to the caller.
        ///
        ///     There are five functions on a control that are safe to call from any
        ///     thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///     For all other method calls, you should use one of the invoke methods to marshal
        ///     the call to the control's thread.
        /// </devdoc>
        public Object Invoke(Delegate method) {
            return Invoke(method, null);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Invoke1"]/*' />
        /// <devdoc>
        ///     Executes the given delegate on the thread that owns this Control's
        ///     underlying window handle.  It is an error to call this on the same thread that
        ///     the control belongs to.  If the control's handle doesn't exist yet, this will
        ///     follow up the control's parent chain until it finds a control or form that does
        ///     have a window handle.  If no appropriate handle can be found, invoke will throw
        ///     an exception.  Exceptions that are raised during the call will be
        ///     propapgated back to the caller.
        ///
        ///     There are five functions on a control that are safe to call from any
        ///     thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///     For all other method calls, you should use one of the invoke methods to marshal
        ///     the call to the control's thread.
        /// </devdoc>
        public Object Invoke(Delegate method, params Object[] args) {
            using (new MultithreadSafeCallScope()) {
                Control marshaler = FindMarshalingControl();
                return marshaler.MarshaledInvoke(this, method, args, true);
            }
        }

        /// <devdoc>
        ///     Perform the callback of a particular ThreadMethodEntry - called by InvokeMarshaledCallbacks below.
        ///
        ///     If the invoke request originated from another thread, we should have already captured the ExecutionContext
        ///     of that thread. The callback is then invoked using that ExecutionContext (which includes info like the
        ///     compressed security stack).
        ///
        ///     NOTE: The one part of the ExecutionContext that we DONT want applied to the callback is its SyncContext,
        ///     since this is the SyncContext of the other thread. So we grab the SyncContext of OUR thread, and pass
        ///     this through to the callback to use instead.
        ///
        ///     When the invoke request comes from this thread, there won't be an ExecutionContext so we just invoke
        ///     the callback as is.
        /// </devdoc>
        private void InvokeMarshaledCallback(ThreadMethodEntry tme){
            if (tme.executionContext != null) {
                if (invokeMarshaledCallbackHelperDelegate == null) {
                    invokeMarshaledCallbackHelperDelegate = new ContextCallback(InvokeMarshaledCallbackHelper);
                }
                // If there's no ExecutionContext, make sure we have a SynchronizationContext.  There's no
                // direct check for ExecutionContext: this is as close as we can get.
                if (SynchronizationContext.Current == null) {
                    WindowsFormsSynchronizationContext.InstallIfNeeded();
                }

                tme.syncContext = SynchronizationContext.Current;
                ExecutionContext.Run(tme.executionContext, invokeMarshaledCallbackHelperDelegate, tme);
            }
            else {
                InvokeMarshaledCallbackHelper(tme);
            }
        }

        /// <devdoc>
        ///     Worker for invoking marshaled callbacks.
        /// </devdoc>
        private static void InvokeMarshaledCallbackHelper(object obj)
        {
            ThreadMethodEntry tme = (ThreadMethodEntry) obj;

            if (tme.syncContext != null) {
                SynchronizationContext oldContext = SynchronizationContext.Current;

                try {
                    SynchronizationContext.SetSynchronizationContext(tme.syncContext);

                    InvokeMarshaledCallbackDo(tme);
                }
                finally {
                    SynchronizationContext.SetSynchronizationContext(oldContext);
                }
            }
            else {
                InvokeMarshaledCallbackDo(tme);
            }
        }

        /// <devdoc>
        /// </devdoc>
        private static void InvokeMarshaledCallbackDo(ThreadMethodEntry tme)
        {
            // We short-circuit a couple of common cases for speed.
            //
            if (tme.method is EventHandler) {
                if (tme.args == null || tme.args.Length < 1) {
                    ((EventHandler)tme.method)(tme.caller, EventArgs.Empty);
                }
                else if (tme.args.Length < 2) {
                    ((EventHandler)tme.method)(tme.args[0], EventArgs.Empty);
                }
                else {
                    ((EventHandler)tme.method)(tme.args[0], (EventArgs)tme.args[1]);
                }
            }
            else if (tme.method is MethodInvoker) {
                ((MethodInvoker)tme.method)();
            }
            else if (tme.method is WaitCallback)
            {
                Debug.Assert(tme.args.Length == 1,
                             "Arguments are wrong for WaitCallback");
                ((WaitCallback)tme.method)(tme.args[0]);
            }
            else {
                tme.retVal = tme.method.DynamicInvoke(tme.args);
            }
        }

        /// <devdoc>
        ///     Called on the control's owning thread to perform the actual callback.
        ///     This empties this control's callback queue, propagating any exceptions
        ///     back as needed.
        /// </devdoc>
        private void InvokeMarshaledCallbacks() {
            ThreadMethodEntry current = null;
            lock(threadCallbackList) {
                if (threadCallbackList.Count > 0) {
                    current = (ThreadMethodEntry)threadCallbackList.Dequeue();
                }
            }

            // Now invoke on all the queued items.
            //
            while (current != null) {
                if (current.method != null) {

                    try {
                        // If we are running under the debugger, don't wrap asynchronous
                        // calls in a try catch.  It is much better to throw here than pop up
                        // a thread exception dialog below.
                        if (NativeWindow.WndProcShouldBeDebuggable  && !current.synchronous) {
                            InvokeMarshaledCallback(current);
                        }
                        else {
                            try {
                                InvokeMarshaledCallback(current);
                            }
                            catch (Exception t) {
                                current.exception = t.GetBaseException();
                            }
                        }
                    }
                    finally {
                        current.Complete();

                        // This code matches the behavior above.  Basically, if we're debugging, don't
                        // do this because the exception would have been handled above.  If we're
                        // not debugging, raise the exception here.
                        if (!NativeWindow.WndProcShouldBeDebuggable &&
                            current.exception != null && !current.synchronous) {
                            Application.OnThreadException(current.exception);
                        }
                    }
                }

                lock (threadCallbackList) {
                    if (threadCallbackList.Count > 0) {
                        current = (ThreadMethodEntry)threadCallbackList.Dequeue();
                    }
                    else {
                        current = null;
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InvokePaint"]/*' />
        protected void InvokePaint(Control c, PaintEventArgs e) {
            c.OnPaint(e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InvokePaintBackground"]/*' />
        protected void InvokePaintBackground(Control c, PaintEventArgs e) {
           c.OnPaintBackground(e);
        }

        /// <devdoc>
        /// determines whether the font is set
        /// </devdoc>
        /// <internalonly/>
        internal bool IsFontSet() {
            Font font = (Font)Properties.GetObject(PropFont);
            if (font != null) {
                return true;
            }
            return false;
        }

        /// <devdoc>
        ///     WARNING! The meaning of this method is not what it appears.
        ///     The method returns true if "descendant" (the argument) is a descendant
        ///     of "this". I'd expect it to be the other way around, but oh well too late.
        /// </devdoc>
        /// <internalonly/>
        internal bool IsDescendant(Control descendant) {
            Control control = descendant;
            while (control != null) {
                if (control == this)
                    return true;
                control = control.ParentInternal;
            }
            return false;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IsKeyLocked"]/*' />
        /// <devdoc>
        /// This Function will return a Boolean as to whether the Key value passed in is Locked...
        /// </devdoc>
        public static bool IsKeyLocked(Keys keyVal) {
            if (keyVal == Keys.Insert || keyVal == Keys.NumLock || keyVal == Keys.CapsLock || keyVal == Keys.Scroll) {
                int result = UnsafeNativeMethods.GetKeyState((int)keyVal);

                // If the high-order bit is 1, the key is down; otherwise, it is up.
                // If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key,
                // is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0.
                // A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled,
                // and off when the key is untoggled.

                // Toggle keys (only low bit is of interest).
                if( keyVal == Keys.Insert || keyVal == Keys.CapsLock )
                {
                    return (result & 0x1) != 0x0;
                }

                return (result & 0x8001) != 0x0;
            }

            // else - it's an un-lockable key.
            // Actually get the exception string from the system resource.
            throw new NotSupportedException(SR.ControlIsKeyLockedNumCapsScrollLockKeysSupportedOnly);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IsInputChar"]/*' />
        /// <devdoc>
        ///     Determines if charCode is an input character that the control
        ///     wants. This method is called during window message pre-processing to
        ///     determine whether the given input character should be pre-processed or
        ///     sent directly to the control. If isInputChar returns true, the
        ///     given character is sent directly to the control. If isInputChar
        ///     returns false, the character is pre-processed and only sent to the
        ///     control if it is not consumed by the pre-processing phase. The
        ///     pre-processing of a character includes checking whether the character
        ///     is a mnemonic of another control.
        /// </devdoc>
        
        // 


        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
        protected virtual bool IsInputChar(char charCode) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.IsInputChar 0x" + ((int)charCode).ToString("X", CultureInfo.InvariantCulture));

            int mask = 0;
            if (charCode == (char)(int)Keys.Tab) {
                mask = NativeMethods.DLGC_WANTCHARS | NativeMethods.DLGC_WANTALLKEYS | NativeMethods.DLGC_WANTTAB;
            }
            else {
                mask = NativeMethods.DLGC_WANTCHARS | NativeMethods.DLGC_WANTALLKEYS;
            }
            return(unchecked( (int) (long)SendMessage(NativeMethods.WM_GETDLGCODE, 0, 0)) & mask) != 0;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IsInputKey"]/*' />
        /// <devdoc>
        ///     Determines if keyData is an input key that the control wants.
        ///     This method is called during window message pre-processing to determine
        ///     whether the given input key should be pre-processed or sent directly to
        ///     the control. If isInputKey returns true, the given key is sent
        ///     directly to the control. If isInputKey returns false, the key is
        ///     pre-processed and only sent to the control if it is not consumed by the
        ///     pre-processing phase. Keys that are pre-processed include TAB, RETURN,
        ///     ESCAPE, and arrow keys.
        /// </devdoc>

        // 


        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
        protected virtual bool IsInputKey(Keys keyData) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.IsInputKey " + keyData.ToString());

            if ((keyData & Keys.Alt) == Keys.Alt) return false;
            int mask = NativeMethods.DLGC_WANTALLKEYS;
            switch (keyData & Keys.KeyCode) {
                case Keys.Tab:
                    mask = NativeMethods.DLGC_WANTALLKEYS | NativeMethods.DLGC_WANTTAB;
                    break;
                case Keys.Left:
                case Keys.Right:
                case Keys.Up:
                case Keys.Down:
                    mask = NativeMethods.DLGC_WANTALLKEYS | NativeMethods.DLGC_WANTARROWS;
                    break;
            }

            if (IsHandleCreated) {
                return(unchecked( (int) (long)SendMessage(NativeMethods.WM_GETDLGCODE, 0, 0)) & mask) != 0;
            }
            else {
                return false;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IsMnemonic"]/*' />
        /// <devdoc>
        ///     Determines if charCode is the mnemonic character in text.
        ///     The mnemonic character is the character imediately following the first
        ///     instance of "&amp;" in text
        /// </devdoc>
        public static bool IsMnemonic(char charCode, string text) {
#if DEBUG
            if (ControlKeyboardRouting.TraceVerbose) {
                Debug.Write("Control.IsMnemonic(" + charCode.ToString() + ", ");

                if (text != null) {
                    Debug.Write(text);
                }
                else {
                    Debug.Write("null");
                }
                Debug.WriteLine(")");
            }
#endif

            //Special case handling:
            if (charCode=='&') {
                Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "   ...returning false");
                return false;
            } //if


            if (text != null) {
                int pos = -1; // start with -1 to handle double &'s
                char c2 = Char.ToUpper(charCode, CultureInfo.CurrentCulture);
                for (;;) {
                    if (pos + 1 >= text.Length)
                        break;
                    pos = text.IndexOf('&', pos + 1) + 1;
                    if (pos <= 0 || pos >= text.Length)
                        break;
                    char c1 = Char.ToUpper(text[pos], CultureInfo.CurrentCulture);
                    Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "   ...& found... char=" + c1.ToString());
                    if (c1 == c2 || Char.ToLower(c1, CultureInfo.CurrentCulture) == Char.ToLower(c2, CultureInfo.CurrentCulture)) {
                        Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "   ...returning true");
                        return true;
                    }
                }
                Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose && pos == 0, "   ...no & found");
            }
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "   ...returning false");
            return false;
        }

        private void ListenToUserPreferenceChanged(bool listen) {
            if (GetState2(STATE2_LISTENINGTOUSERPREFERENCECHANGED)) {
                if (!listen) {
                    SetState2(STATE2_LISTENINGTOUSERPREFERENCECHANGED, false);
                    SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(this.UserPreferenceChanged);
                }
            }
            else if (listen) {
                SetState2(STATE2_LISTENINGTOUSERPREFERENCECHANGED, true);
                SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(this.UserPreferenceChanged);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.LogicalToDeviceUnits"]/*' />
        /// <devdoc>
        /// Transforms an integer coordinate from logical to device units
        /// by scaling it for the current DPI and rounding down to the nearest integer value.
        /// </devdoc>
        public int LogicalToDeviceUnits(int value) {
            return DpiHelper.LogicalToDeviceUnits(value, DeviceDpi);
        }

        /// <summary>
        /// Transforms size from logical to device units
        /// by scaling it for the current DPI and rounding down to the nearest integer value for width and height.
        /// </summary>
        /// <param name="value"> size to be scaled</param>
        /// <returns> scaled size</returns>
        public Size LogicalToDeviceUnits(Size value) {
            return DpiHelper.LogicalToDeviceUnits(value, DeviceDpi);
        }

        /// <summary>
        /// Create a new bitmap scaled for the device units.
        /// When displayed on the device, the scaled image will have same size as the original image would have when displayed at 96dpi.
        /// </summary>
        /// <param name="logicalBitmap">The image to scale from logical units to device units</param>
        public void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap) {
            DpiHelper.ScaleBitmapLogicalToDevice(ref logicalBitmap, DeviceDpi);
        }

        internal void AdjustWindowRectEx(ref NativeMethods.RECT rect, int style, bool bMenu, int exStyle) {
            if (DpiHelper.IsPerMonitorV2Awareness) {
                SafeNativeMethods.AdjustWindowRectExForDpi(ref rect, style, bMenu, exStyle, (uint)deviceDpi);
            }
            else {
                SafeNativeMethods.AdjustWindowRectEx(ref rect, style, bMenu, exStyle);
            }
        }

        private Object MarshaledInvoke(Control caller, Delegate method, Object[] args, bool synchronous) {

            // Marshaling an invoke occurs in three steps:
            //
            // 1.  Create a ThreadMethodEntry that contains the packet of information
            //     about this invoke.  This TME is placed on a linked list of entries because
            //     we have a gap between the time we PostMessage and the time it actually
            //     gets processed, and this gap may allow other invokes to come in.  Access
            //     to this linked list is always synchronized.
            //
            // 2.  Post ourselves a message.  Our caller has already determined the
            //     best control to call us on, and we should almost always have a handle.
            //
            // 3.  If we're synchronous, wait for the message to get processed.  We don't do
            //     a SendMessage here so we're compatible with OLE, which will abort many
            //     types of calls if we're within a SendMessage.
            //

            if (!IsHandleCreated) {
                throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
            }

            // We have to demand unmanaged code permission here for the control hosted in
            // the browser case. Without this check, we will expose a security hole, because
            // ActiveXImpl.OnMessage() will assert unmanaged code for everyone as part of
            // its implementation.
            // The right fix is to remove the Assert() on top of the ActiveXImpl class, and
            // visit each method to see if it needs unmanaged code permission, and if so, add
            // the permission just to that method(s).
            //
            ActiveXImpl activeXImpl = (ActiveXImpl)Properties.GetObject(PropActiveXImpl);
            if (activeXImpl != null) {
                IntSecurity.UnmanagedCode.Demand();
            }

            // We don't want to wait if we're on the same thread, or else we'll deadlock.
            // It is important that syncSameThread always be false for asynchronous calls.
            //
            bool syncSameThread = false;
            int pid; // ignored
            if (SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, Handle), out pid) == SafeNativeMethods.GetCurrentThreadId()) {
                if (synchronous)
                    syncSameThread = true;
            }

            // Store the compressed stack information from the thread that is calling the Invoke()
            // so we can assign the same security context to the thread that will actually execute
            // the delegate being passed.
            //
            ExecutionContext executionContext = null;
            if (!syncSameThread) {
                executionContext = ExecutionContext.Capture();
            }
            ThreadMethodEntry tme = new ThreadMethodEntry(caller, this, method, args, synchronous, executionContext);

            lock (this) {
                if (threadCallbackList == null) {
                    threadCallbackList = new Queue();
                }
            }

            lock (threadCallbackList) {
                if (threadCallbackMessage == 0) {
                    threadCallbackMessage = SafeNativeMethods.RegisterWindowMessage(Application.WindowMessagesVersion + "_ThreadCallbackMessage");
                }
                threadCallbackList.Enqueue(tme);
            }

            if (syncSameThread) {
                InvokeMarshaledCallbacks();
            }  else {
                // 

                UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), threadCallbackMessage, IntPtr.Zero, IntPtr.Zero);
            }

            if (synchronous) {
                if (!tme.IsCompleted) {
                    WaitForWaitHandle(tme.AsyncWaitHandle);
                }
                if (tme.exception != null) {
                    throw tme.exception;
                }
                return tme.retVal;
            }
            else {
                return(IAsyncResult)tme;
            }
        }

        /// <devdoc>
        ///     This method is used by WM_GETCONTROLNAME and WM_GETCONTROLTYPE
        ///     to marshal a string to a message structure.  It handles
        ///     two cases:  if no buffer was passed it returns the size of
        ///     buffer needed.  If a buffer was passed, it fills the buffer.
        ///     If the passed buffer is not long enough it will return -1.
        /// </devdoc>
        private void MarshalStringToMessage(string value, ref Message m) {
            if (m.LParam == IntPtr.Zero) {
                m.Result = (IntPtr)((value.Length + 1) * sizeof(char));
                return;
            }

            if (unchecked((int)(long)m.WParam) < value.Length + 1) {
                m.Result = (IntPtr)(-1);
                return;
            }

            // Copy the name into the given IntPtr
            //
            char[] nullChar = new char[] {(char)0};
            byte[] nullBytes;
            byte[] bytes;

            bytes = Encoding.Unicode.GetBytes(value);
            nullBytes = Encoding.Unicode.GetBytes(nullChar);

            Marshal.Copy(bytes, 0, m.LParam, bytes.Length);
            Marshal.Copy(nullBytes, 0, unchecked((IntPtr)((long)m.LParam + (long)bytes.Length)), nullBytes.Length);

            m.Result = (IntPtr)((bytes.Length + nullBytes.Length) / sizeof(char));
        }

        // Used by form to notify the control that it has been "entered"
        //
        internal void NotifyEnter() {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Control::NotifyEnter() - " + this.Name);
            OnEnter(EventArgs.Empty);
        }

        // Used by form to notify the control that it has been "left"
        //
        internal void NotifyLeave() {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Control::NotifyLeave() - " + this.Name);
            OnLeave(EventArgs.Empty);
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.NotifyInvalidate"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    Propagates the invalidation event, notifying the control that
        ///    some part of it is being invalidated and will subsequently need
        ///    to repaint.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void NotifyInvalidate(Rectangle invalidatedArea) {
            OnInvalidated(new InvalidateEventArgs(invalidatedArea));
        }

        // Used by form to notify the control that it is validating.
        //
        private bool NotifyValidating() {
            CancelEventArgs ev = new CancelEventArgs();
            OnValidating(ev);
            return ev.Cancel;
        }

        // Used by form to notify the control that it has been validated.
        //
        private void NotifyValidated() {
            OnValidated(EventArgs.Empty);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InvokeOnClick"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Click'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void InvokeOnClick(Control toInvoke, EventArgs e) {
            if (toInvoke != null) {
                toInvoke.OnClick(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnAutoSizeChanged"]/*' />
        protected virtual void OnAutoSizeChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventAutoSizeChanged] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnBackColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackColorChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            object backBrush = Properties.GetObject(PropBackBrush);
            if (backBrush != null) {
                if (GetState(STATE_OWNCTLBRUSH)) {
                    IntPtr p = (IntPtr)backBrush;
                    if (p != IntPtr.Zero) {
                        SafeNativeMethods.DeleteObject(new HandleRef(this, p));
                    }
                }
                Properties.SetObject(PropBackBrush, null);
            }

            Invalidate();

            EventHandler eh = Events[EventBackColor] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentBackColorChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnBackgroundImageChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackgroundImageChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            Invalidate();

            EventHandler eh = Events[EventBackgroundImage] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentBackgroundImageChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnBackgroundImageLayoutChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackgroundImageLayoutChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            Invalidate();

            EventHandler eh = Events[EventBackgroundImageLayout] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnBindingContextChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBindingContextChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (Properties.GetObject(PropBindings) != null) {
                UpdateBindings();
            }

            EventHandler eh = Events[EventBindingContext] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentBindingContextChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnCausesValidationChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCausesValidationChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventCausesValidation] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnChildLayoutResuming"]/*' />
        /// <devdoc>
        ///     Called when a child is about to resume its layout.  The default implementation
        ///     calls OnChildLayoutResuming on the parent.
        /// </devdoc>
        internal virtual void OnChildLayoutResuming(Control child, bool performLayout) {
            if (ParentInternal != null) {
                ParentInternal.OnChildLayoutResuming(child, performLayout);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnContextMenuChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnContextMenuChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventContextMenu] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnContextMenuStripChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnContextMenuStripChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventContextMenuStrip] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnCursorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCursorChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventCursor] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentCursorChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDockChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDockChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventDock] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnEnabledChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Enabled'/> event.</para>
        /// <para>Inheriting classes should override this method to handle this event.
        ///    Call base.OnEnabled to send this event to any registered event listeners.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnEnabledChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (IsHandleCreated) {
                SafeNativeMethods.EnableWindow(new HandleRef(this, Handle), Enabled);

                // User-paint controls should repaint when their enabled state changes
                //
                if (GetStyle(ControlStyles.UserPaint)) {
                    Invalidate();
                    Update();
                }
            }

            EventHandler eh = Events[EventEnabled] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentEnabledChanged(e);
                }
            }
        }

        internal virtual void OnFrameWindowActivate(bool fActivate) {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnFontChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFontChanged(EventArgs e) {
            Contract.Requires(e != null);
            // bail if disposing
            //
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            Invalidate();

            if (Properties.ContainsInteger(PropFontHeight)) {
                Properties.SetInteger(PropFontHeight, -1);
            }


            // Cleanup any font handle wrapper...
            //
            DisposeFontHandle();

            if (IsHandleCreated && !GetStyle(ControlStyles.UserPaint)) {
                SetWindowFont();
            }

            EventHandler eh = Events[EventFont] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            using(new LayoutTransaction(this, this, PropertyNames.Font, false)) {
                if (controlsCollection != null) {
                    // This may have changed the sizes of our children.
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for(int i = 0; i < controlsCollection.Count; i++) {
                        controlsCollection[i].OnParentFontChanged(e);
                    }
                }
            }

            LayoutTransaction.DoLayout(this,this,PropertyNames.Font);            
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnForeColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnForeColorChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            Invalidate();

            EventHandler eh = Events[EventForeColor] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentForeColorChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnRightToLeftChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }
            
            // update the scroll position when the handle has been created
            // MUST SET THIS BEFORE CALLING RecreateHandle!!!
            SetState2(STATE2_SETSCROLLPOS, true);

            RecreateHandle();

            EventHandler eh = Events[EventRightToLeft] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnParentRightToLeftChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnNotifyMessage"]/*' />
        /// <devdoc>
        ///    OnNotifyMessage is called if the ControlStyles.EnableNotifyMessage
        ///    bit is set. This allows for semi-trusted controls to listen to
        ///    window messages, without allowing them to actually modify the
        ///    message.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnNotifyMessage(Message m) {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentBackColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBackColorChanged(EventArgs e) {
            Contract.Requires(e != null);
            Color backColor = Properties.GetColor(PropBackColor);
            if (backColor.IsEmpty) {
                OnBackColorChanged(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentBackgroundImageChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBackgroundImageChanged(EventArgs e) {
            Contract.Requires(e != null);
            OnBackgroundImageChanged(e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentBindingContextChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBindingContextChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (Properties.GetObject(PropBindingManager) == null) {
                OnBindingContextChanged(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentCursorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentCursorChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (Properties.GetObject(PropCursor) == null) {
                OnCursorChanged(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentEnabledChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentEnabledChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetState(STATE_ENABLED)) {
                OnEnabledChanged(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentFontChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentFontChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (Properties.GetObject(PropFont) == null) {
                OnFontChanged(e);
            }
        }


        // occurs when the parent of this control has recreated
        // its handle.
        // presently internal as we feel this will be rare to
        // require override
        internal virtual void OnParentHandleRecreated() {
            // restore ourselves over to the original control.

            // use SetParent directly so as to not raise ParentChanged events
            Control parent = ParentInternal;
            if (parent != null) {
                if (IsHandleCreated) {
                    UnsafeNativeMethods.SetParent(new HandleRef(this, this.Handle), new HandleRef(parent, parent.Handle));
                    UpdateZOrder();
                }
            }
            SetState(STATE_PARENTRECREATING, false);

            // if our parent was initially the the parent who's handle just got recreated, we need
            // to recreate ourselves so that we get notification.  See UpdateReflectParent for more details.
            if (ReflectParent == ParentInternal)  {
                 RecreateHandle();
            }

        }


        // occurs when the parent of this control is recreating
        // its handle.
        // presently internal as we feel this will be rare to
        // require override
        internal virtual void  OnParentHandleRecreating() {
            SetState(STATE_PARENTRECREATING, true);
            // swoop this control over to the parking window.

            // if we left it parented to the parent control, the DestroyWindow
            // would force us to destroy our handle as well... hopefully
            // parenting us over to the parking window and restoring ourselves
            // should help improve recreate perf.

            // use SetParent directly so as to not raise ParentChanged events
            if (IsHandleCreated) {
                Application.ParkHandle(new HandleRef(this, this.Handle));
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentForeColorChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentForeColorChanged(EventArgs e) {
            Contract.Requires(e != null);
            Color foreColor = Properties.GetColor(PropForeColor);
            if (foreColor.IsEmpty) {
                OnForeColorChanged(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentRightToLeftChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentRightToLeftChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (!Properties.ContainsInteger(PropRightToLeft) || ((RightToLeft)Properties.GetInteger(PropRightToLeft)) == RightToLeft.Inherit) {
                OnRightToLeftChanged(e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentVisibleChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentVisibleChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetState(STATE_VISIBLE)) {
                OnVisibleChanged(e);
            }
        }

        // OnVisibleChanged/OnParentVisibleChanged is not called when a parent becomes invisible
        internal virtual void OnParentBecameInvisible() {
            if (GetState(STATE_VISIBLE)) {
                // This control became invisible too - notify its children
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
                if (controlsCollection != null) {
                    for(int i = 0; i < controlsCollection.Count; i++) {
                        Control ctl = controlsCollection[i];
                            ctl.OnParentBecameInvisible();
                    }
                }
            }
        }
        
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnPrint"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPrint(PaintEventArgs e) {
            if (e == null) {
                throw new ArgumentNullException(nameof(e));
            }
            Contract.EndContractBlock();

            if (GetStyle(ControlStyles.UserPaint)) {
                // Theme support on Windows XP requires that we paint the background
                // and foreground to support semi-transparent children
                //
                PaintWithErrorHandling(e, PaintLayerBackground);
                e.ResetGraphics();
                PaintWithErrorHandling(e, PaintLayerForeground);
            }
            else {
                PrintPaintEventArgs ppev = e as PrintPaintEventArgs;
                Message m;
                bool releaseDC = false;
                IntPtr hdc = IntPtr.Zero;

                if (ppev == null) {
                    IntPtr flags = (IntPtr)(NativeMethods.PRF_CHILDREN | NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND | NativeMethods.PRF_NONCLIENT);
                    hdc = e.HDC;

                    if (hdc == IntPtr.Zero) {
                        // a manually created paintevent args
                        hdc = e.Graphics.GetHdc();
                        releaseDC = true;
                    }
                    m = Message.Create(this.Handle, NativeMethods.WM_PRINTCLIENT, hdc, flags);
                }
                else {
                    m = ppev.Message;
                }

                try {
                    DefWndProc(ref m);
                }
                finally {
                    if (releaseDC) {
                        e.Graphics.ReleaseHdcInternal(hdc);
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnTabIndexChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTabIndexChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventTabIndex] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnTabStopChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTabStopChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventTabStop] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnTextChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTextChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventText] as EventHandler;
            if (eh != null) {
                 eh(this, e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnVisibleChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Visible'/> event.</para>
        /// <para>Inheriting classes should override this method to handle this event.
        ///    Call base.OnVisible to send this event to any registered event listeners.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnVisibleChanged(EventArgs e) {
            Contract.Requires(e != null);
            bool visible = this.Visible;
            if (visible) {
                UnhookMouseEvent();
                trackMouseEvent = null;
            }
            if (parent != null && visible && !Created) {
                bool isDisposing = GetAnyDisposingInHierarchy();
                if (!isDisposing) {
                    // Usually the control is created by now, but in a few corner cases
                    // exercised by the PropertyGrid dropdowns, it isn't
                    CreateControl();
                }
            }

            EventHandler eh = Events[EventVisible] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    Control ctl = controlsCollection[i];
                    if (ctl.Visible) {
                        ctl.OnParentVisibleChanged(e);
                    }
                    if (!visible) {
                        ctl.OnParentBecameInvisible();
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnTopMostActiveXParentChanged"]/*' />
        internal virtual void OnTopMostActiveXParentChanged(EventArgs e) {
            Contract.Requires(e != null);
            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnTopMostActiveXParentChanged(e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnParentChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventParent] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }

            //
            // Inform the control that the topmost control is now an ActiveX control
            if (TopMostParent.IsActiveX) {
                OnTopMostActiveXParentChanged(EventArgs.Empty);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnClick"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Click'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClick(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventClick];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnClientSizeChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClientSizeChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler eh = Events[EventClientSize] as EventHandler;
            if (eh != null) {
                eh(this,e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnControlAdded"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.ControlAdded'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnControlAdded(ControlEventArgs e) {
            Contract.Requires(e != null);
            ControlEventHandler handler = (ControlEventHandler)Events[EventControlAdded];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnControlRemoved"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.ControlRemoved'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnControlRemoved(ControlEventArgs e) {
            Contract.Requires(e != null);
            ControlEventHandler handler = (ControlEventHandler)Events[EventControlRemoved];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnCreateControl"]/*' />
        /// <devdoc>
        ///    <para>Called when the control is first created.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCreateControl() {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to find out when the
        ///     handle has been created.
        ///     Call base.OnHandleCreated first.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHandleCreated(EventArgs e) {
            Contract.Requires(e != null);
            if (this.IsHandleCreated) {
                
                // Setting fonts is for some reason incredibly expensive.
                // (Even if you exclude font handle creation)
                if (!GetStyle(ControlStyles.UserPaint)){
                    SetWindowFont();
                }

                if (DpiHelper.IsPerMonitorV2Awareness && !(typeof(Form).IsAssignableFrom(this.GetType()))) {
                    int old = deviceDpi;
                    deviceDpi = (int)UnsafeNativeMethods.GetDpiForWindow(new HandleRef(this, HandleInternal));
                    if (old != deviceDpi) {
                        RescaleConstantsForDpi(old, deviceDpi);
                    }
                }

                // Restore dragdrop status. Ole Initialize happens
                // when the ThreadContext in Application is created
                //
                SetAcceptDrops(AllowDrop);

                Region region = (Region)Properties.GetObject(PropRegion);
                if (region != null) {
                    IntPtr regionHandle = GetHRgn(region);

                    try {
                        if (IsActiveX) {
                            regionHandle = ActiveXMergeRegion(regionHandle);
                        }

                        if (UnsafeNativeMethods.SetWindowRgn(new HandleRef(this, Handle), new HandleRef(this, regionHandle), SafeNativeMethods.IsWindowVisible(new HandleRef(this, Handle))) != 0) {
                            //The HWnd owns the region.
                            regionHandle = IntPtr.Zero;
                        }
                    }
                    finally {
                        if (regionHandle != IntPtr.Zero) {
                            SafeNativeMethods.DeleteObject(new HandleRef(null, regionHandle));
                        }
                    }
                }

                // Update accessbility information.
                //
                ControlAccessibleObject accObj = Properties.GetObject(PropAccessibility) as ControlAccessibleObject;
                ControlAccessibleObject ncAccObj = Properties.GetObject(PropNcAccessibility) as ControlAccessibleObject;

                // Cache Handle in a local before asserting so we minimize code running under the Assert.
                IntPtr handle = Handle;
                
                // Reviewed  : ControlAccessibleObject.set_Handle demands UnmanagedCode permission for public use, it doesn't
                //             expose any security vulnerability indirectly. The sec Assert is safe.
                //
                IntSecurity.UnmanagedCode.Assert();
                
                try {
                    if (accObj != null) {
                        accObj.Handle = handle;
                    }
                    if (ncAccObj != null) {
                        ncAccObj.Handle = handle;
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                // Set the window text from the Text property.
                //
                if (text != null && text.Length != 0) {
                    UnsafeNativeMethods.SetWindowText(new HandleRef(this, Handle), text);
                }

                if (!(this is ScrollableControl) && !IsMirrored && GetState2(STATE2_SETSCROLLPOS) && !GetState2(STATE2_HAVEINVOKED)) {
                    BeginInvoke(new EventHandler(this.OnSetScrollPosition));
                    SetState2(STATE2_HAVEINVOKED, true);
                    SetState2(STATE2_SETSCROLLPOS, false);
                }

                // Listen to UserPreferenceChanged if the control is top level and interested in the notification.
                if (GetState2(STATE2_INTERESTEDINUSERPREFERENCECHANGED)) {
                    ListenToUserPreferenceChanged(GetTopLevel());
                }
            }

            EventHandler handler = (EventHandler)Events[EventHandleCreated];
            if (handler != null) handler(this, e);

            if (this.IsHandleCreated)
            {
                // Now, repost the thread callback message if we found it.  We should do
                // this last, so we're as close to the same state as when the message
                // was placed.
                //
                if (GetState(STATE_THREADMARSHALLPENDING)) {
                    UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), threadCallbackMessage, IntPtr.Zero, IntPtr.Zero);
                    SetState(STATE_THREADMARSHALLPENDING, false);
                }
            }
        }
        
        private void OnSetScrollPosition(object sender, EventArgs e) {
            SetState2(STATE2_HAVEINVOKED, false);
            OnInvokedSetScrollPosition(sender, e);
        }
        
        internal virtual void OnInvokedSetScrollPosition(object sender, EventArgs e) {
            if (!(this is ScrollableControl) && !IsMirrored) {
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
                si.fMask = NativeMethods.SIF_RANGE;
                if (UnsafeNativeMethods.GetScrollInfo(new HandleRef(this, Handle), NativeMethods.SB_HORZ,si) != false) {
                    si.nPos = (RightToLeft == RightToLeft.Yes) ? si.nMax : si.nMin;
                    SendMessage(NativeMethods.WM_HSCROLL, NativeMethods.Util.MAKELPARAM(NativeMethods.SB_THUMBPOSITION, si.nPos), 0);
                }                
            } 
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnLocationChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLocationChanged(EventArgs e) {
            Contract.Requires(e != null);
            OnMove(EventArgs.Empty);

            EventHandler eh = Events[EventLocation] as EventHandler;
            if (eh != null) {
                eh(this,e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnHandleDestroyed"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to find out when the
        ///     handle is about to be destroyed.
        ///     Call base.OnHandleDestroyed last.
        /// <para>Raises the <see cref='System.Windows.Forms.Control.HandleDestroyed'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHandleDestroyed(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventHandleDestroyed];
            if (handler != null) handler(this, e);

            UpdateReflectParent(false);

            if (!RecreatingHandle) {
                if (GetState(STATE_OWNCTLBRUSH)) {
                    object backBrush = Properties.GetObject(PropBackBrush);
                    if (backBrush != null) {
                        IntPtr p = (IntPtr)backBrush;
                        if (p != IntPtr.Zero) {
                            SafeNativeMethods.DeleteObject(new HandleRef(this, p));
                        }
                        Properties.SetObject(PropBackBrush, null);
                    }
                }
                ListenToUserPreferenceChanged(false /*listen*/);
            }

            // this code is important -- it is critical that we stash away
            // the value of the text for controls such as edit, button,
            // label, etc. Without this processing, any time you change a
            // property that forces handle recreation, you lose your text!
            // See the above code in wmCreate
            //

            try {
                if (!GetAnyDisposingInHierarchy()) {
                    text = Text;
                    if (text != null && text.Length == 0) text = null;
                }
                SetAcceptDrops(false);
            }
            catch (Exception ex) {
                if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                    throw;
                }

                // Some ActiveX controls throw exceptions when
                // you ask for the text property after you have destroyed their handle. We
                // don't want those exceptions to bubble all the way to the top, since
                // we leave our state in a mess.
                //
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDoubleClick"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.DoubleClick'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDoubleClick(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventDoubleClick];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDragEnter"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Enter'/> event.</para>
        /// <para>Inheriting classes should override this method to handle this event.
        ///    Call base.onEnter to send this event to any registered event listeners.</para>
        /// </devdoc>
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onDragEnter to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragEnter(DragEventArgs drgevent) {
            Contract.Requires(drgevent != null);
            DragEventHandler handler = (DragEventHandler)Events[EventDragEnter];
            if (handler != null) handler(this,drgevent);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDragOver"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onDragOver to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragOver(DragEventArgs drgevent) {
            Contract.Requires(drgevent != null);
            DragEventHandler handler = (DragEventHandler)Events[EventDragOver];
            if (handler != null) handler(this,drgevent);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDragLeave"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onDragLeave to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragLeave(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventDragLeave];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDragDrop"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onDragDrop to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragDrop(DragEventArgs drgevent) {
            Contract.Requires(drgevent != null);
            DragEventHandler handler = (DragEventHandler)Events[EventDragDrop];
            if (handler != null) handler(this,drgevent);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnGiveFeedback"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onGiveFeedback to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
        protected virtual void OnGiveFeedback(GiveFeedbackEventArgs gfbevent) {
            Contract.Requires(gfbevent != null);
            GiveFeedbackEventHandler handler = (GiveFeedbackEventHandler)Events[EventGiveFeedback];
            if (handler != null) handler(this,gfbevent);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnEnter"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnEnter(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventEnter];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InvokeGotFocus"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.GotFocus'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void InvokeGotFocus(Control toInvoke, EventArgs e) {
            if (toInvoke != null) {
                toInvoke.OnGotFocus(e);
                if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(toInvoke);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnGotFocus"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.GotFocus'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnGotFocus(EventArgs e) {
            Contract.Requires(e != null);
            if (IsActiveX) {
                ActiveXOnFocus(true);
            }
            
            if (parent != null) {
                parent.ChildGotFocus(this);
            }

            EventHandler handler = (EventHandler)Events[EventGotFocus];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnHelpRequested"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onHelp to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHelpRequested(HelpEventArgs hevent) {
            Contract.Requires(hevent != null);
            HelpEventHandler handler = (HelpEventHandler)Events[EventHelpRequested];
            if (handler != null) {
                handler(this,hevent);
                // Set this to true so that multiple events aren't raised to the Form.
                hevent.Handled = true;
            }

            if (!hevent.Handled) {
                if (ParentInternal != null) {
                    ParentInternal.OnHelpRequested(hevent);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnInvalidated"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.OnInvalidate to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnInvalidated(InvalidateEventArgs e) {
            Contract.Requires(e != null);
            // Ask the site to change the view...
            if (IsActiveX) {
                ActiveXViewChanged();
            }
            
            // Transparent control support
            ControlCollection controls = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controls != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controls.Count; i++) {
                    controls[i].OnParentInvalidated(e);
                }
            }

            InvalidateEventHandler handler = (InvalidateEventHandler)Events[EventInvalidated];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnKeyDown"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyDown'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyDown(KeyEventArgs e) {
            Contract.Requires(e != null);
            KeyEventHandler handler = (KeyEventHandler)Events[EventKeyDown];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnKeyPress"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyPress'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyPress(KeyPressEventArgs e) {
            Contract.Requires(e != null);
            KeyPressEventHandler handler = (KeyPressEventHandler)Events[EventKeyPress];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnKeyUp"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.KeyUp'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyUp(KeyEventArgs e) {
            Contract.Requires(e != null);
            KeyEventHandler handler = (KeyEventHandler)Events[EventKeyUp];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnLayout"]/*' />
        /// <devdoc>
        ///     Core layout logic. Inheriting controls should override this function
        ///     to do any custom layout logic. It is not neccessary to call
        ///     base.layoutCore, however for normal docking and anchoring
        ///     functions to work, base.layoutCore must be called.
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Layout'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLayout(LayoutEventArgs levent) {
            Contract.Requires(levent != null);
            // Ask the site to change the view...
            if (IsActiveX) {
                ActiveXViewChanged();
            }
            LayoutEventHandler handler = (LayoutEventHandler)Events[EventLayout];
            if (handler != null) handler(this, levent);

            bool parentRequiresLayout = LayoutEngine.Layout(this, levent);

            // 


            if(parentRequiresLayout && ParentInternal != null) {
                // LayoutEngine.Layout can return true to request that our parent resize us because
                // we did not have enough room for our contents.  We can not just call PerformLayout
                // because this container is currently suspended.  PerformLayout will check this state
                // flag and PerformLayout on our parent.
                ParentInternal.SetState(STATE_LAYOUTISDIRTY, true);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnLayoutResuming"]/*' />
        /// <devdoc>
        ///     Called when the last resume layout call is made.  If performLayout is true
        ///     a layout will occur as soon as this call returns.  Layout is
        ///     still suspended when this call is made.  The default implementation
        ///     calls OnChildLayoutResuming on the parent, if it exists.
        /// </devdoc>
        internal virtual void OnLayoutResuming(bool performLayout) {
            if (ParentInternal != null) {
                ParentInternal.OnChildLayoutResuming(this, performLayout);
            }
        }
        
        internal virtual void OnLayoutSuspended() {
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnLeave"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Leave'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLeave(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventLeave];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.InvokeLostFocus"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void InvokeLostFocus(Control toInvoke, EventArgs e) {
            if (toInvoke != null) {
                toInvoke.OnLostFocus(e);
                if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(toInvoke);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnLostFocus"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.LostFocus'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLostFocus(EventArgs e) {
            Contract.Requires(e != null);
            if (IsActiveX) {
                ActiveXOnFocus(false);
            }

            EventHandler handler = (EventHandler)Events[EventLostFocus];
            if (handler != null) handler(this, e);
        }

        protected virtual void OnMarginChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventMarginChanged];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseDoubleClick"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseDoubleClick'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDoubleClick(MouseEventArgs e) {
            Contract.Requires(e != null);
            MouseEventHandler handler = (MouseEventHandler)Events[EventMouseDoubleClick];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseClick"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.OnMouseClick'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseClick(MouseEventArgs e) {
            Contract.Requires(e != null);
            MouseEventHandler handler = (MouseEventHandler)Events[EventMouseClick];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseCaptureChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseCaptureChanged'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseCaptureChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventMouseCaptureChanged];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseDown"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseDown'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDown(MouseEventArgs e) {
            Contract.Requires(e != null);
            MouseEventHandler handler = (MouseEventHandler)Events[EventMouseDown];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseEnter"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseEnter'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseEnter(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventMouseEnter];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseLeave"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseLeave'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseLeave(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventMouseLeave];
            if (handler != null) handler(this, e);
        }

 
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDpiChangedBeforeParent"]/*' />
        /// <devdoc>
        /// <para>
        /// Raises the <see cref='System.Windows.Forms.Control.DpiChangedBeforeParent'/> event.
        /// Occurs when the form is moved to a monitor with a different resolution (number of dots per inch),
        /// or when scaling level is changed in the windows setting by the user.
        /// This message is not sent to the top level windows.
        /// </para>
        /// </devdoc>
        [
            Browsable(true), 
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        protected virtual void OnDpiChangedBeforeParent(EventArgs e) {
            Contract.Requires(e != null);
            ((EventHandler)Events[EventDpiChangedBeforeParent])?.Invoke(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnDpiChangedAfterParent"]/*' />
        /// <devdoc>
        /// <para>
        /// Raises the <see cref='System.Windows.Forms.Control.DpiChangedAfterParent'/> event.
        /// Occurs when the form is moved to a monitor with a different resolution (number of dots per inch),
        /// or when scaling level is changed in windows setting by the user.
        /// This message is not sent to the top level windows.
        /// </para>
        /// </devdoc>
        [
            Browsable(true), 
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        protected virtual void OnDpiChangedAfterParent(EventArgs e) {
            Contract.Requires(e != null);
            ((EventHandler)Events[EventDpiChangedAfterParent])?.Invoke(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseHover"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseHover'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseHover(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventMouseHover];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseMove"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseMove'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseMove(MouseEventArgs e) {
            Contract.Requires(e != null);
            MouseEventHandler handler = (MouseEventHandler)Events[EventMouseMove];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseUp"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseUp'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseUp(MouseEventArgs e) {
            Contract.Requires(e != null);
            MouseEventHandler handler = (MouseEventHandler)Events[EventMouseUp];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMouseWheel"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.MouseWheel'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseWheel(MouseEventArgs e) {
            Contract.Requires(e != null);
            MouseEventHandler handler = (MouseEventHandler)Events[EventMouseWheel];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnMove"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Move'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMove(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventMove];
            if (handler != null) handler(this, e);
            if (RenderTransparent)
                Invalidate();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnPaint"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onPaint to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPaint(PaintEventArgs e) {
            Contract.Requires(e != null);
            PaintEventHandler handler = (PaintEventHandler)Events[EventPaint];
            if (handler != null) handler(this, e);
        }

        protected virtual void OnPaddingChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetStyle(ControlStyles.ResizeRedraw)) {
                Invalidate();
            }
            EventHandler handler = (EventHandler)Events[EventPaddingChanged];
            if (handler != null) handler(this, e);
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnPaintBackground"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle the erase
        ///     background request from windows. It is not necessary to call
        ///     base.onPaintBackground, however if you do not want the default
        ///     Windows behavior you must set event.handled to true.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPaintBackground(PaintEventArgs pevent) {
            Contract.Requires(pevent != null);
            // We need the true client rectangle as clip rectangle causes
            // problems on "Windows Classic" theme.  
            NativeMethods.RECT rect = new NativeMethods.RECT();
            UnsafeNativeMethods.GetClientRect(new HandleRef(window, InternalHandle), ref rect);
            
            PaintBackground(pevent, new Rectangle(rect.left, rect.top, rect.right, rect.bottom));
        }

        // Transparent control support
        private void OnParentInvalidated(InvalidateEventArgs e) {
            Contract.Requires(e != null);
            if (!RenderTransparent) return;

            if (this.IsHandleCreated) {
                // move invalid rect into child space
                Rectangle cliprect = e.InvalidRect;
                Point offs = this.Location;
                cliprect.Offset(-offs.X,-offs.Y);
                cliprect = Rectangle.Intersect(this.ClientRectangle, cliprect);

                // if we don't intersect at all, do nothing
                if (cliprect.IsEmpty) return;
                this.Invalidate(cliprect);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnQueryContinueDrag"]/*' />
        /// <devdoc>
        ///     Inheriting classes should override this method to handle this event.
        ///     Call base.onQueryContinueDrag to send this event to any registered event listeners.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        [SuppressMessage("Microsoft.Security", "CA2119:SealMethodsThatSatisfyPrivateInterfaces")]
        protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent) {
            Contract.Requires(qcdevent != null);
            QueryContinueDragEventHandler handler = (QueryContinueDragEventHandler)Events[EventQueryContinueDrag];
            if (handler != null) handler(this, qcdevent);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnRegionChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.RegionChanged'/> event when the Region property has changed.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRegionChanged(EventArgs e) {
           Contract.Requires(e != null);
           EventHandler eh = Events[EventRegionChanged] as EventHandler;
           if (eh != null) {
                eh(this, e);
           }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnResize"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Resize'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResize(EventArgs e) {
            Contract.Requires(e != null);
            if ((controlStyle & ControlStyles.ResizeRedraw) == ControlStyles.ResizeRedraw
                || GetState(STATE_EXCEPTIONWHILEPAINTING)) {
                Invalidate();
            }
            LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
            EventHandler handler = (EventHandler)Events[EventResize];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnPreviewKeyDown"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.PreviewKeyDown'/> event.</para>
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Advanced),
        SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
        SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
        SuppressMessage("Microsoft.Security", "CA2109:ReviewVisibleEventHandlers")
        ]
        protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e) {
            Contract.Requires(e != null);
            PreviewKeyDownEventHandler handler = (PreviewKeyDownEventHandler)Events[EventPreviewKeyDown];
            if (handler != null) {
                handler(this, e);
            }
        }
       
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnSizeChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSizeChanged(EventArgs e) {
            Contract.Requires(e != null);
            OnResize(EventArgs.Empty);

            EventHandler eh = Events[EventSize] as EventHandler;
            if (eh != null) {
                eh(this,e);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnChangeUICues"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.ChangeUICues'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnChangeUICues(UICuesEventArgs e) {
            Contract.Requires(e != null);
            UICuesEventHandler handler = (UICuesEventHandler)Events[EventChangeUICues];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnStyleChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.OnStyleChanged'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnStyleChanged(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventStyleChanged];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnSystemColorsChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.SystemColorsChanged'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSystemColorsChanged(EventArgs e) {
            Contract.Requires(e != null);
            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
            if (controlsCollection != null) {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for(int i = 0; i < controlsCollection.Count; i++) {
                    controlsCollection[i].OnSystemColorsChanged(EventArgs.Empty);
                }
            }
            Invalidate();

            EventHandler handler = (EventHandler)Events[EventSystemColorsChanged];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnValidating"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Validating'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidating(CancelEventArgs e) {
            Contract.Requires(e != null);
            CancelEventHandler handler = (CancelEventHandler)Events[EventValidating];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnValidated"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Validated'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidated(EventArgs e) {
            Contract.Requires(e != null);
            EventHandler handler = (EventHandler)Events[EventValidated];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RescaleConstantsForDpi"]/*' />
        /// <devdoc>
        /// Is invoked when the control handle is created or right before the top level parent receives WM_DPICHANGED message.
        /// This method is an opportunity to rescale any constant sizes, glyphs or bitmaps before re-painting.
        /// The derived class can choose to not call the base class implementation.
        /// </devdoc>
        [
            Browsable(true), 
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew) {
        }

        // This is basically OnPaintBackground, put in a separate method for ButtonBase,
        // which does all painting under OnPaint, and tries very hard to avoid double-painting the border pixels.
        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle) {
            PaintBackground(e, rectangle, this.BackColor, Point.Empty);
        }

        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor) {
            PaintBackground(e, rectangle, backColor, Point.Empty);
        }

        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor, Point scrollOffset) {
            if (RenderColorTransparent(backColor)) {
                PaintTransparentBackground(e, rectangle);
            }

            //If the form or mdiclient is mirrored then we do not render the background image due to GDI+ issues.
            bool formRTL = ((this is Form || this is MdiClient) && this.IsMirrored);

            // The rest of this won't do much if BackColor is transparent and there is no BackgroundImage,
            // but we need to call it in the partial alpha case.

            // 

            if (BackgroundImage != null && !DisplayInformation.HighContrast && !formRTL) {

                if (BackgroundImageLayout == ImageLayout.Tile) {
                    if (ControlPaint.IsImageTransparent(BackgroundImage)) {
                        PaintTransparentBackground(e, rectangle);
                    }
                }

                // 

                Point scrollLocation = scrollOffset;
                ScrollableControl scrollControl = this as ScrollableControl;
                if (scrollControl != null && scrollLocation != Point.Empty) {
                    scrollLocation = ((ScrollableControl)this).AutoScrollPosition;
                }

                if (ControlPaint.IsImageTransparent(BackgroundImage)) {
                    PaintBackColor(e, rectangle, backColor);
                }

                ControlPaint.DrawBackgroundImage(e.Graphics, BackgroundImage, backColor, BackgroundImageLayout, ClientRectangle, rectangle, scrollLocation, RightToLeft);
            }
            else {
                PaintBackColor(e, rectangle, backColor);
            }
        }
        private static void PaintBackColor(PaintEventArgs e, Rectangle rectangle, Color backColor) {
            // Common case of just painting the background.  For this, we
            // use GDI because it is faster for simple things than creating
            // a graphics object, brush, etc.  Also, we may be able to
            // use a system brush, avoiding the brush create altogether.
            //
            Color color = backColor;

            // Note: PaintEvent.HDC == 0 if GDI+ has used the HDC -- it wouldn't be safe for us
            // to use it without enough bookkeeping to negate any performance gain of using GDI.
            if (color.A == 255) {
                using (WindowsGraphics wg = (e.HDC != IntPtr.Zero && DisplayInformation.BitsPerPixel > 8) ? 
                                WindowsGraphics.FromHdc(e.HDC) : WindowsGraphics.FromGraphics(e.Graphics)) {
                    color = wg.GetNearestColor(color);

                    using (WindowsBrush brush = new WindowsSolidBrush(wg.DeviceContext, color)) {
                        wg.FillRectangle(brush, rectangle);
                    }
                }
            }
            else {
                // don't paint anything from 100% transparent background
                //
                if (color.A > 0) {
                    // Color has some transparency or we have no HDC, so we must
                    // fall back to using GDI+.
                    //
                    using (Brush brush = new SolidBrush(color)) {
                        e.Graphics.FillRectangle(brush, rectangle);
                    }
                }
            }
        }

        // Paints a red rectangle with a red X, painted on a white background
        private void PaintException(PaintEventArgs e) {
#if false
            StringFormat stringFormat = ControlPaint.StringFormatForAlignment(ContentAlignment.TopLeft);
            string exceptionText = Properties.GetObject(PropPaintingException).ToString();
            stringFormat.SetMeasurableCharacterRanges(new CharacterRange[] {new CharacterRange(0, exceptionText.Length)});

            // rendering calculations...
            //
            int penThickness = 2;
            Size glyphSize = SystemInformation.IconSize;
            int marginX = penThickness * 2;
            int marginY = penThickness * 2;

            Rectangle clientRectangle = ClientRectangle;

            Rectangle borderRectangle = ClientRectangle;
            borderRectangle.X++;
            borderRectangle.Y++;
            borderRectangle.Width -=2;
            borderRectangle.Height-=2;

            Rectangle imageRect = new Rectangle(marginX, marginY, glyphSize.Width, glyphSize.Height);

            Rectangle textRect = clientRectangle;
            textRect.X = imageRect.X + imageRect.Width + 2 * marginX;
            textRect.Y = imageRect.Y;
            textRect.Width -= (textRect.X + marginX + penThickness);
            textRect.Height -= (textRect.Y + marginY + penThickness);

            using (Font errorFont = new Font(Font.FontFamily, Math.Max(SystemInformation.ToolWindowCaptionHeight - SystemInformation.BorderSize.Height - 2, Font.Height), GraphicsUnit.Pixel)) {

                using(Region textRegion = e.Graphics.MeasureCharacterRanges(exceptionText, errorFont, textRect, stringFormat)[0]) {
                    // paint contents... clipping optimizations for less flicker...
                    //
                    Region originalClip = e.Graphics.Clip;

                    e.Graphics.ExcludeClip(textRegion);
                    e.Graphics.ExcludeClip(imageRect);
                    try {
                        e.Graphics.FillRectangle(Brushes.White, clientRectangle);
                    }
                    finally {
                        e.Graphics.Clip = originalClip;
                    }

                    using (Pen pen = new Pen(Color.Red, penThickness)) {
                        e.Graphics.DrawRectangle(pen, borderRectangle);
                    }

                    Icon err = SystemIcons.Error;

                    e.Graphics.FillRectangle(Brushes.White, imageRect);
                    e.Graphics.DrawIcon(err, imageRect.X, imageRect.Y);

                    textRect.X++;
                    e.Graphics.IntersectClip(textRegion);
                    try {
                        e.Graphics.FillRectangle(Brushes.White, textRect);
                        e.Graphics.DrawString(exceptionText, errorFont, new SolidBrush(ForeColor), textRect, stringFormat);
                    }
                    finally {
                        e.Graphics.Clip = originalClip;
                    }
                }
            }
#else
            int penThickness = 2;
            using (Pen pen = new Pen(Color.Red, penThickness)) {
                Rectangle clientRectangle = ClientRectangle;
                Rectangle rectangle = clientRectangle;
                rectangle.X++;
                rectangle.Y++;
                rectangle.Width--;
                rectangle.Height--;
    
                e.Graphics.DrawRectangle(pen, rectangle.X, rectangle.Y, rectangle.Width - 1, rectangle.Height - 1);
                rectangle.Inflate(-1, -1);
                e.Graphics.FillRectangle(Brushes.White, rectangle);
                e.Graphics.DrawLine(pen, clientRectangle.Left, clientRectangle.Top,
                                    clientRectangle.Right, clientRectangle.Bottom);
                e.Graphics.DrawLine(pen, clientRectangle.Left, clientRectangle.Bottom,
                                    clientRectangle.Right, clientRectangle.Top);
            }

#endif
        }

        internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle) {
            PaintTransparentBackground(e,rectangle,null);
        }
        // Trick our parent into painting our background for us, or paint some default
        // color if that doesn't work.
        //
        // This method is the hardest part of implementing transparent controls;
        // call this in your OnPaintBackground method, and away you go.
        // 
        // If you only want a region of the control to be transparent, pass in a region into the
        // last parameter.  A null region implies that you want the entire rectangle to be transparent.
        internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle, Region transparentRegion) {
            Graphics g = e.Graphics;
            Control parent = this.ParentInternal;

            if (parent != null) {
                // We need to use themeing painting for certain controls (like TabPage) when they parent other controls.
                // But we dont want to to this always as this causes serious preformance (at Runtime and DesignTime)
                // so checking for RenderTransparencyWithVisualStyles which is TRUE for TabPage and false by default.
                if (Application.RenderWithVisualStyles && parent.RenderTransparencyWithVisualStyles) {
                    // When we are rendering with visual styles, we can use the cool DrawThemeParentBackground function
                    // that UxTheme provides to render the parent's background. This function is control agnostic, so
                    // we use the wrapper in ButtonRenderer - this should do the right thing for all controls,
                    // not just Buttons.
             
                    GraphicsState graphicsState = null;
                    if (transparentRegion != null) {
                        graphicsState = g.Save();
                    }
                    try {
                        if (transparentRegion != null) {
                            g.Clip = transparentRegion;
                        }
                        ButtonRenderer.DrawParentBackground(g, rectangle, this);
                    }
                    finally {
                        if (graphicsState != null) {
                            g.Restore(graphicsState);   
                        }
                    }
                    
                    
                }
                else {
                    // how to move the rendering area and setup it's size
                    // (we want to translate it to the parent's origin)
                    Rectangle shift = new Rectangle(-this.Left,-this.Top, parent.Width, parent.Height);

                    // moving the clipping rectangle to the parent coordinate system
                    Rectangle newClipRect = new Rectangle(rectangle.Left + this.Left, rectangle.Top + this.Top, rectangle.Width, rectangle.Height);
                             
                    using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g)) {
                        wg.DeviceContext.TranslateTransform(-this.Left, -this.Top);
                        using (PaintEventArgs np = new PaintEventArgs(wg.GetHdc(), newClipRect)) {
                            if (transparentRegion != null) {
                                np.Graphics.Clip = transparentRegion; 
                                np.Graphics.TranslateClip(-shift.X, -shift.Y);
                            }
                            try {
                                InvokePaintBackground(parent, np);
                                InvokePaint(parent, np);
                            }
                            finally {
                                if (transparentRegion != null) {
                                    // restore region back to original state.
                                    np.Graphics.TranslateClip(shift.X, shift.Y);
                                }
                            }
                        }
                    }

                }
                        
            }
            else {
                // For whatever reason, our parent can't paint our background, but we need some kind of background
                // since we're transparent.
                g.FillRectangle(SystemBrushes.Control, rectangle);
            }
        }

        // Exceptions during painting are nasty, because paint events happen so often.
        // So if user painting code has an issue, we make sure never to call it again,
        // so as not to spam the end-user with exception dialogs.
        // 
        private void PaintWithErrorHandling(PaintEventArgs e, short layer) {
            try {
                CacheTextInternal = true;
                if (GetState(STATE_EXCEPTIONWHILEPAINTING)) {
                    if (layer == PaintLayerBackground)
                        PaintException(e);
                }
                else {
                    bool exceptionThrown = true;
                    try {
                        switch (layer) {
                            case PaintLayerForeground:
                                OnPaint(e);
                                break;
                            case PaintLayerBackground:
                                if (!GetStyle(ControlStyles.Opaque)) {
                                    OnPaintBackground(e);
                                }
                                break;
                            default:
                                Debug.Fail("Unknown PaintLayer " + layer);
                                break;
                        }
                        exceptionThrown = false;
                    }
                    finally {
                        if (exceptionThrown) {
                            SetState(STATE_EXCEPTIONWHILEPAINTING, true);
                            Invalidate();
                        }
                    }
                }
            }
            finally {
                CacheTextInternal = false;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ParentContainerControl"]/*' />
        /// <devdoc>
        ///     Find ContainerControl that is the container of this control.
        /// </devdoc>
        internal ContainerControl ParentContainerControl {
            get {
                for (Control c = this.ParentInternal; c != null; c = c.ParentInternal) {
                    if (c is ContainerControl) {
                        return c as ContainerControl;
                    }
                }

                return null;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PerformLayout"]/*' />
        /// <devdoc>
        ///     Forces the control to apply layout logic to all of the child controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void PerformLayout() {
            if (cachedLayoutEventArgs != null) {
                PerformLayout(cachedLayoutEventArgs);
                cachedLayoutEventArgs = null;

                // we need to be careful
                // about which LayoutEventArgs are used in
                // SuspendLayout, PerformLayout, ResumeLayout() sequences.
                SetState2(STATE2_CLEARLAYOUTARGS, false);
            }
            else {
                PerformLayout(null, null);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PerformLayout1"]/*' />
        /// <devdoc>
        ///     Forces the control to apply layout logic to all of the child controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void PerformLayout(Control affectedControl, string affectedProperty) {
            PerformLayout(new LayoutEventArgs(affectedControl, affectedProperty));
        }

        internal void PerformLayout(LayoutEventArgs args) {
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (layoutSuspendCount > 0) {
                SetState(STATE_LAYOUTDEFERRED, true);
                if (cachedLayoutEventArgs == null || (GetState2(STATE2_CLEARLAYOUTARGS) && args != null)) {
                   cachedLayoutEventArgs = args;
                   if (GetState2(STATE2_CLEARLAYOUTARGS)) {
                    SetState2(STATE2_CLEARLAYOUTARGS, false);
                   }
                }
                
                LayoutEngine.ProcessSuspendedLayoutEventArgs(this, args);
                
                return;
            }

            // (Essentially the same as suspending layout while we layout, but we clear differently below.)
            layoutSuspendCount = 1;

            try {
                CacheTextInternal = true;
                OnLayout(args);

#if DEBUG
#if DEBUG_PREFERREDSIZE
                // DO NOT REMOVE - See http://wiki/default.aspx/Microsoft.Projects.DotNetClient.PreferredSize
                // PreferredSize has an assert that will verify that the current cached size matches
                // the computed size.  The calls to PreferredSize below help to catch a mismatch earlier.
                if (!this.GetState(STATE_DISPOSING | STATE_DISPOSED)) {
                    if (GetState2(STATE2_USEPREFERREDSIZECACHE)) {
                        this.PreferredSize.ToString();
                    }

                    if (args.AffectedControl != null && args.AffectedControl != this && args.AffectedControl.GetState2(STATE2_USEPREFERREDSIZECACHE)) {
                        args.AffectedControl.PreferredSize.ToString();
                    }                        
                }
#endif
#endif


            }
            finally {
                CacheTextInternal = false;
                // Rather than resume layout (which will could allow a deferred layout to layout the
                // the container we just finished laying out) we set layoutSuspendCount back to zero
                // and clear the deferred and dirty flags.
                SetState(STATE_LAYOUTDEFERRED | STATE_LAYOUTISDIRTY, false);
                layoutSuspendCount = 0;

                // LayoutEngine.Layout can return true to request that our parent resize us because
                // we did not have enough room for our contents.  Now that we are unsuspended,
                // see if this happened and layout parent if necessary.  (See also OnLayout)
                if(ParentInternal != null && ParentInternal.GetState(STATE_LAYOUTISDIRTY)) {
                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.PreferredSize);
                }
            }
        }

        /// <devdoc>
        ///     Peforms data validation (not paint validation!) on a single control.
        ///
        ///     Returns whether validation failed:
        ///       False = Validation succeeded, control is valid, accept its new value
        ///       True = Validation was cancelled, control is invalid, reject its new value
        ///
        ///     NOTE: This is the lowest possible level of validation. It does not account
        ///     for the context in which the validation is occuring, eg. change of focus
        ///     between controls in a container. Stuff like that is handled by the caller.
        /// </devdoc>
        /// <internalonly/>
        internal bool PerformControlValidation(bool bulkValidation) {

            // Skip validation for controls that don't support it
            if (!this.CausesValidation) {
                return false;
            }

            // Raise the 'Validating' event. Stop now if handler cancels (ie. control is invalid).
            // NOTE: Handler may throw an exception here, but we must not attempt to catch it.
            if (this.NotifyValidating()) {
                return true;
            }

            // Raise the 'Validated' event. Handlers may throw exceptions here too - but
            // convert these to ThreadException events, unless the app is being debugged,
            // or the control is being validated as part of a bulk validation operation.
            if (bulkValidation || NativeWindow.WndProcShouldBeDebuggable) {
                this.NotifyValidated();
            }
            else {
                try {
                    this.NotifyValidated();
                }
                catch (Exception e) {
                    Application.OnThreadException(e);
                }
            }

            return false;
        }

        /// <devdoc>
        ///     Validates all the child controls in a container control. Exactly which controls are
        ///     validated and which controls are skipped is determined by <paramref name="validationConstraints"/>.
        ///     Return value indicates whether validation failed for any of the controls validated.
        ///     Calling function is responsible for checking the correctness of the validationConstraints argument.
        /// </devdoc>
        /// <internalonly/>
        internal bool PerformContainerValidation(ValidationConstraints validationConstraints) {
            bool failed = false;

            // For every child control of this container control...
            foreach (Control c in Controls) {

                // First, if the control is a container, recurse into its descendants.
                if ((validationConstraints & ValidationConstraints.ImmediateChildren) != ValidationConstraints.ImmediateChildren &&
                    c.ShouldPerformContainerValidation() &&
                    c.PerformContainerValidation(validationConstraints)) {
                    failed = true;
                }

                // Next, use input flags to decide whether to validate the control itself
                if ((validationConstraints & ValidationConstraints.Selectable) == ValidationConstraints.Selectable && !c.GetStyle(ControlStyles.Selectable) ||
                    (validationConstraints & ValidationConstraints.Enabled)    == ValidationConstraints.Enabled    && !c.Enabled ||
                    (validationConstraints & ValidationConstraints.Visible)    == ValidationConstraints.Visible    && !c.Visible ||
                    (validationConstraints & ValidationConstraints.TabStop)    == ValidationConstraints.TabStop    && !c.TabStop) {
                    continue;
                }

                // Finally, perform validation on the control itself
                if (c.PerformControlValidation(true)) {
                    failed = true;
                }
            }

            return failed;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PointToClient"]/*' />
        /// <devdoc>
        ///     Computes the location of the screen point p in client coords.
        /// </devdoc>
        public Point PointToClient(Point p) {
            // Reviewed: The following demand has always been commented out since v1.0. In Whidbey, we have
            // re-reviewed this method to see if it needs protection, but haven't found any threats, so are
            // going to keep it this way.
            //
            // Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ScreenLocationOfThings Demanded");
            // IntSecurity.ScreenLocationOfThings.Demand();
            return PointToClientInternal(p);
        }
        internal Point PointToClientInternal(Point p) {            
            // Win9x reports incorrect values if you go outside the 16-bit range.
            // We're not going to do anything about it, though -- it's esoteric, it clutters up the code,
            // and potentially causes problems on systems that do support 32-bit coordinates.

            NativeMethods.POINT point = new NativeMethods.POINT(p.X, p.Y);
            UnsafeNativeMethods.MapWindowPoints(NativeMethods.NullHandleRef, new HandleRef(this, Handle), point, 1);
            return new Point(point.x, point.y);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PointToScreen"]/*' />
        /// <devdoc>
        ///     Computes the location of the client point p in screen coords.
        /// </devdoc>
        public Point PointToScreen(Point p) {
            // Reviewed: The following demand has always been commented out since v1.0. In Whidbey, we have
            // re-reviewed this method to see if it needs protection, but haven't found any threats, so are
            // going to keep it this way. .
            //
            // Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ScreenLocationOfThings Demanded");
            // IntSecurity.ScreenLocationOfThings.Demand();

            NativeMethods.POINT point = new NativeMethods.POINT(p.X, p.Y);
            UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, Handle), NativeMethods.NullHandleRef, point, 1);
            return new Point(point.x, point.y);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.PreProcessMessage"]/*' />
        /// <devdoc>
        ///     <para>
        ///     This method is called by the application's message loop to pre-process
        ///     input messages before they are dispatched. Possible values for the
        ///     msg.message field are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.
        ///     If this method processes the message it must return true, in which case
        ///     the message loop will not dispatch the message.
        ///     </para>
        ///     <para>
        /// For WM_KEYDOWN and WM_SYSKEYDOWN messages, preProcessMessage() first
        ///     calls processCmdKey() to check for command keys such as accelerators and
        ///     menu shortcuts. If processCmdKey() doesn't process the message, then
        ///     isInputKey() is called to check whether the key message represents an
        ///     input key for the control. Finally, if isInputKey() indicates that the
        ///     control isn't interested in the key message, then processDialogKey() is
        ///     called to check for dialog keys such as TAB, arrow keys, and mnemonics.
        ///     </para>
        ///     <para>
        /// For WM_CHAR messages, preProcessMessage() first calls isInputChar() to
        ///     check whether the character message represents an input character for
        ///     the control. If isInputChar() indicates that the control isn't interested
        ///     in the character message, then processDialogChar() is called to check for
        ///     dialog characters such as mnemonics.
        ///     </para>
        ///     <para>
        /// For WM_SYSCHAR messages, preProcessMessage() calls processDialogChar()
        ///     to check for dialog characters such as mnemonics.
        ///     </para>
        ///     <para>
        /// When overriding preProcessMessage(), a control should return true to
        ///     indicate that it has processed the message. For messages that aren't
        ///     processed by the control, the result of "base.preProcessMessage()"
        ///     should be returned. Controls will typically override one of the more
        ///     specialized methods (isInputChar(), isInputKey(), processCmdKey(),
        ///     processDialogChar(), or processDialogKey()) instead of overriding
        ///     preProcessMessage().
        ///     </para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
         SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        public virtual bool PreProcessMessage(ref Message msg) {
         //   Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.PreProcessMessage " + msg.ToString());

            bool ret;

            if (msg.Msg == NativeMethods.WM_KEYDOWN || msg.Msg == NativeMethods.WM_SYSKEYDOWN) {
                if (!GetState2(STATE2_UICUES)) {
                    ProcessUICues(ref msg);
                }

                Keys keyData = (Keys)(unchecked((int)(long)msg.WParam) | (int)ModifierKeys);
                if (ProcessCmdKey(ref msg, keyData)) {
                    ret = true;
                }
                else if (IsInputKey(keyData)) {
                    SetState2(STATE2_INPUTKEY, true);
                    ret = false;
                }
                else {
                    // 



                    IntSecurity.ModifyFocus.Assert();
                    try {
                        ret = ProcessDialogKey(keyData);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }
            }
            else if (msg.Msg == NativeMethods.WM_CHAR || msg.Msg == NativeMethods.WM_SYSCHAR) {
                if (msg.Msg == NativeMethods.WM_CHAR && IsInputChar((char)msg.WParam)) {
                    SetState2(STATE2_INPUTCHAR, true);
                    ret = false;
                }
                else {
                    ret = ProcessDialogChar((char)msg.WParam);
                }
            }
            else {
                ret = false;
            }

            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced),
         SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
         SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly"),
         SuppressMessage("Microsoft.Performance", "CA1801:AvoidUnusedParameters"),
         SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]  // using ref is OK.
        public PreProcessControlState PreProcessControlMessage(ref Message msg) {
            return PreProcessControlMessageInternal(null, ref msg);
        }

        /// <devdoc>
        ///  This method is similar to PreProcessMessage, but instead of indicating
        ///  that the message was either processed or it wasn't, it has three return
        ///  values:
        ///
        ///         MessageProcessed - PreProcessMessage() returns true, and the message
        ///                            needs no further processing
        ///
        ///         MessageNeeded    - PreProcessMessage() returns false, but IsInputKey/Char
        ///                            return true.  This means the message wasn't processed, 
        ///                            but the control is interested in it.
        ///
        ///         MessageNotNeeded - PreProcessMessage() returns false, and IsInputKey/Char
        ///                            return false.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        internal static PreProcessControlState PreProcessControlMessageInternal(Control target, ref Message msg) {
            if (target == null) {
                target = Control.FromChildHandleInternal(msg.HWnd);
            }

            if (target == null) {
                return PreProcessControlState.MessageNotNeeded;
            }

            // reset state that is used to make sure IsInputChar, IsInputKey and 
            // ProcessUICues are not called multiple times.
            // ISSUE: Which control should these state bits be set on? probably the target.
            target.SetState2(STATE2_INPUTKEY,  false);
            target.SetState2(STATE2_INPUTCHAR, false);
            target.SetState2(STATE2_UICUES,    true);

            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.PreProcessControlMessageInternal " + msg.ToString());
            
            try {
                Keys keyData = (Keys)(unchecked((int)(long)msg.WParam) | (int)ModifierKeys);

                // Allow control to preview key down message.
                if (msg.Msg == NativeMethods.WM_KEYDOWN || msg.Msg == NativeMethods.WM_SYSKEYDOWN) {
                    target.ProcessUICues(ref msg);                    

                    PreviewKeyDownEventArgs args = new PreviewKeyDownEventArgs(keyData);
                    target.OnPreviewKeyDown(args);
                    
                    if (args.IsInputKey) {
                        Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "PreviewKeyDown indicated this is an input key.");
                        // Control wants this message - indicate it should be dispatched.
                        return PreProcessControlState.MessageNeeded;
                    }                    
                }

                PreProcessControlState state = PreProcessControlState.MessageNotNeeded;
                
                if (!target.PreProcessMessage(ref msg)) {
                    if (msg.Msg == NativeMethods.WM_KEYDOWN || msg.Msg == NativeMethods.WM_SYSKEYDOWN) {

                        // check if IsInputKey has already procssed this message
                        // or if it is safe to call - we only want it to be called once.
                        if (target.GetState2(STATE2_INPUTKEY) || target.IsInputKey(keyData)) {
                            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control didn't preprocess this message but it needs to be dispatched");
                            state = PreProcessControlState.MessageNeeded;
                        }
                    }
                    else if (msg.Msg == NativeMethods.WM_CHAR || msg.Msg == NativeMethods.WM_SYSCHAR) {

                        // check if IsInputChar has already procssed this message
                        // or if it is safe to call - we only want it to be called once.
                        if (target.GetState2(STATE2_INPUTCHAR) || target.IsInputChar((char)msg.WParam)) {
                            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control didn't preprocess this message but it needs to be dispatched");
                            state = PreProcessControlState.MessageNeeded;
                        }
                    }                  
                }
                else {
                    state = PreProcessControlState.MessageProcessed;
                }

                return state;
            }
            finally {
                target.SetState2(STATE2_UICUES, false);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessCmdKey"]/*' />
        /// <devdoc>
        ///     <para>
        ///     Processes a command key. This method is called during message
        ///     pre-processing to handle command keys. Command keys are keys that always
        ///     take precedence over regular input keys. Examples of command keys
        ///     include accelerators and menu shortcuts. The method must return true to
        ///     indicate that it has processed the command key, or false to indicate
        ///     that the key is not a command key.
        ///     </para>
        ///     <para>
        /// processCmdKey() first checks if the control has a context menu, and if
        ///     so calls the menu's processCmdKey() to check for menu shortcuts. If the
        ///     command key isn't a menu shortcut, and if the control has a parent, the
        ///     key is passed to the parent's processCmdKey() method. The net effect is
        ///     that command keys are "bubbled" up the control hierarchy.
        ///     </para>
        ///     <para>
        /// When overriding processCmdKey(), a control should return true to
        ///     indicate that it has processed the key. For keys that aren't processed by
        ///     the control, the result of "base.processCmdKey()" should be returned.
        ///     </para>
        ///     <para>
        /// Controls will seldom, if ever, need to override this method.
        ///     </para>
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessCmdKey " + msg.ToString());
            ContextMenu contextMenu = (ContextMenu)Properties.GetObject(PropContextMenu);
            if (contextMenu != null && contextMenu.ProcessCmdKey(ref msg, keyData, this)) {
                return true;
            }

            if (parent != null) {
                return parent.ProcessCmdKey(ref msg, keyData);
            }
            return false;

        }

        private void PrintToMetaFile(HandleRef hDC, IntPtr lParam) {
            Debug.Assert(UnsafeNativeMethods.GetObjectType(hDC) == NativeMethods.OBJ_ENHMETADC,
                "PrintToMetaFile() called with a non-Enhanced MetaFile DC.");
            Debug.Assert(((long) lParam & NativeMethods.PRF_CHILDREN) != 0,
                "PrintToMetaFile() called without PRF_CHILDREN.");

            // Strip the PRF_CHILDREN flag.  We will manually walk our children and print them.
            lParam = (IntPtr)((long) lParam & ~NativeMethods.PRF_CHILDREN);

            // We're the root contol, so we need to set up our clipping region.  Retrieve the
            // x-coordinates and y-coordinates of the viewport origin for the specified device context.
            NativeMethods.POINT viewportOrg = new NativeMethods.POINT();
            bool success = SafeNativeMethods.GetViewportOrgEx(hDC, viewportOrg);
            Debug.Assert(success, "GetViewportOrgEx() failed.");

            HandleRef hClippingRegion = new HandleRef(null, SafeNativeMethods.CreateRectRgn(viewportOrg.x,viewportOrg.y, viewportOrg.x + this.Width, viewportOrg.y + this.Height));
            Debug.Assert(hClippingRegion.Handle != IntPtr.Zero, "CreateRectRgn() failed.");

            try {
                // Select the new clipping region; make sure it's a SIMPLEREGION or NULLREGION
                NativeMethods.RegionFlags selectResult = (NativeMethods.RegionFlags)SafeNativeMethods.SelectClipRgn(hDC, hClippingRegion);
                Debug.Assert((selectResult == NativeMethods.RegionFlags.SIMPLEREGION ||
                              selectResult == NativeMethods.RegionFlags.NULLREGION),
                              "SIMPLEREGION or NULLLREGION expected.");

                PrintToMetaFileRecursive(hDC, lParam, new Rectangle(Point.Empty, this.Size));
            }
            finally {
                success = SafeNativeMethods.DeleteObject(hClippingRegion);
                Debug.Assert(success, "DeleteObject() failed.");
            }
        }

        internal virtual void PrintToMetaFileRecursive(HandleRef hDC, IntPtr lParam, Rectangle bounds) {
            // We assume the target does not want us to offset the root control in the metafile.

            using (WindowsFormsUtils.DCMapping mapping = new WindowsFormsUtils.DCMapping(hDC, bounds)) {
                // print the non-client area
                PrintToMetaFile_SendPrintMessage(hDC, (IntPtr)((long)lParam & ~NativeMethods.PRF_CLIENT));

                // figure out mapping for the client area
                NativeMethods.RECT windowRect = new NativeMethods.RECT();
                bool success = UnsafeNativeMethods.GetWindowRect(new HandleRef(null, this.Handle), ref windowRect);
                Debug.Assert(success, "GetWindowRect() failed.");
                Point clientOffset = PointToScreen(Point.Empty);
                clientOffset = new Point(clientOffset.X - windowRect.left, clientOffset.Y - windowRect.top);
                Rectangle clientBounds = new Rectangle(clientOffset, ClientSize);

                using (WindowsFormsUtils.DCMapping clientMapping = new WindowsFormsUtils.DCMapping(hDC, clientBounds)) {
                    // print the client area
                    PrintToMetaFile_SendPrintMessage(hDC, (IntPtr)((long)lParam & ~NativeMethods.PRF_NONCLIENT));

                    // Paint children in reverse Z-Order
                    int count = Controls.Count;
                    for (int i = count - 1; i >= 0; i--) {
                        Control child = Controls[i];
                        if(child.Visible) {
                            child.PrintToMetaFileRecursive(hDC, lParam, child.Bounds);
                        }
                    }
                }
            }
        }

        private void PrintToMetaFile_SendPrintMessage(HandleRef hDC, IntPtr lParam) {
            if(GetStyle(ControlStyles.UserPaint)) {
                // We let user paint controls paint directly into the metafile
                SendMessage(NativeMethods.WM_PRINT, hDC.Handle, lParam);
            }
            else {
                // If a system control has no children in the Controls collection we
                // restore the the PRF_CHILDREN flag because it may internally
                // have nested children we do not know about.  ComboBox is a
                // good example.
                if(Controls.Count == 0) {
                    lParam = (IntPtr)((long) lParam | NativeMethods.PRF_CHILDREN);
                }

                // System controls must be painted into a temporary bitmap
                // which is then copied into the metafile.  (Old GDI line drawing
                // is 1px thin, which causes borders to disappear, etc.)
                using (MetafileDCWrapper dcWrapper = new MetafileDCWrapper(hDC, this.Size)) {
                    SendMessage(NativeMethods.WM_PRINT, dcWrapper.HDC, lParam);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessDialogChar"]/*' />
        /// <devdoc>
        ///     <para>
        ///     Processes a dialog character. This method is called during message
        ///     pre-processing to handle dialog characters, such as control mnemonics.
        ///     This method is called only if the isInputChar() method indicates that
        ///     the control isn't interested in the character.
        ///     </para>
        ///     <para>
        /// processDialogChar() simply sends the character to the parent's
        ///     processDialogChar() method, or returns false if the control has no
        ///     parent. The Form class overrides this method to perform actual
        ///     processing of dialog characters.
        ///     </para>
        ///     <para>
        /// When overriding processDialogChar(), a control should return true to
        ///     indicate that it has processed the character. For characters that aren't
        ///     processed by the control, the result of "base.processDialogChar()"
        ///     should be returned.
        ///     </para>
        ///     <para>
        /// Controls will seldom, if ever, need to override this method.
        ///     </para>
        /// </devdoc>
        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]     
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected virtual bool ProcessDialogChar(char charCode) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessDialogChar [" + charCode.ToString() + "]");
            return parent == null? false: parent.ProcessDialogChar(charCode);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessDialogKey"]/*' />
        /// <devdoc>
        ///     <para>
        ///     Processes a dialog key. This method is called during message
        ///     pre-processing to handle dialog characters, such as TAB, RETURN, ESCAPE,
        ///     and arrow keys. This method is called only if the isInputKey() method
        ///     indicates that the control isn't interested in the key.
        ///     </para>
        ///     <para>
        /// processDialogKey() simply sends the character to the parent's
        ///     processDialogKey() method, or returns false if the control has no
        ///     parent. The Form class overrides this method to perform actual
        ///     processing of dialog keys.
        ///     </para>
        ///     <para>
        /// When overriding processDialogKey(), a control should return true to
        ///     indicate that it has processed the key. For keys that aren't processed
        ///     by the control, the result of "base.processDialogKey(...)" should be
        ///     returned.
        ///     </para>
        ///     <para>
        /// Controls will seldom, if ever, need to override this method.
        ///     </para>
        /// </devdoc>
        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]        
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected virtual bool ProcessDialogKey(Keys keyData) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessDialogKey " + keyData.ToString());
            return parent == null? false: parent.ProcessDialogKey(keyData);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessKeyEventArgs"]/*' />
        /// <devdoc>
        ///     <para>
        ///     Processes a key message. This method is called when a control receives a
        ///     keyboard message. The method is responsible for generating the appropriate
        ///     key events for the message by calling OnKeyPress(), onKeyDown(), or
        ///     onKeyUp(). The m parameter contains the window message that must
        ///     be processed. Possible values for the m.msg field are WM_CHAR,
        ///     WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, WM_SYSKEYUP, and WM_IMECHAR.
        ///     </para>
        ///     <para>
        /// When overriding processKeyEventArgs(), a control should return true to
        ///     indicate that it has processed the key. For keys that aren't processed
        ///     by the control, the result of "base.processKeyEventArgs()" should be
        ///     returned.
        ///     </para>
        ///     <para>
        /// Controls will seldom, if ever, need to override this method.
        ///     </para>
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected virtual bool ProcessKeyEventArgs(ref Message m) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessKeyEventArgs " + m.ToString());
            KeyEventArgs ke = null;
            KeyPressEventArgs kpe = null;
            IntPtr newWParam = IntPtr.Zero;

            if (m.Msg == NativeMethods.WM_CHAR || m.Msg == NativeMethods.WM_SYSCHAR) {
                int charsToIgnore = this.ImeWmCharsToIgnore;

                if (charsToIgnore > 0) {
                    charsToIgnore--;
                    Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "charsToIgnore decreased, new val = " + charsToIgnore + ", this=" + this);

                    this.ImeWmCharsToIgnore = charsToIgnore;
                    return false;
                }
                else {
                    kpe = new KeyPressEventArgs(unchecked((char)(long)m.WParam));
                    OnKeyPress(kpe);
                    newWParam = (IntPtr) kpe.KeyChar;
                }
            }
            else if (m.Msg == NativeMethods.WM_IME_CHAR) {
                int charsToIgnore = this.ImeWmCharsToIgnore;

                charsToIgnore += (3 - sizeof(char));
                this.ImeWmCharsToIgnore = charsToIgnore;

                kpe = new KeyPressEventArgs(unchecked((char)(long)m.WParam));

                char preEventCharacter = kpe.KeyChar;
                OnKeyPress(kpe);

                //If the character wasn't changed, just use the original value rather than round tripping.
                if (kpe.KeyChar == preEventCharacter) {
                    newWParam = m.WParam;
                }
                else {
                    newWParam = (IntPtr)kpe.KeyChar;
                }
            }
            else {
                ke = new KeyEventArgs((Keys)(unchecked((int)(long)m.WParam)) | ModifierKeys);
                if (m.Msg == NativeMethods.WM_KEYDOWN || m.Msg == NativeMethods.WM_SYSKEYDOWN) {
                    OnKeyDown(ke);
                }
                else {
                    OnKeyUp(ke);
                }
            }

            if (kpe != null) {
                Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "    processkeyeventarg returning: " + kpe.Handled);
                m.WParam = newWParam;
                return kpe.Handled;
            }
            else {
                Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "    processkeyeventarg returning: " + ke.Handled);
                if (ke.SuppressKeyPress) {
                    RemovePendingMessages(NativeMethods.WM_CHAR,     NativeMethods.WM_CHAR);
                    RemovePendingMessages(NativeMethods.WM_SYSCHAR,  NativeMethods.WM_SYSCHAR);
                    RemovePendingMessages(NativeMethods.WM_IME_CHAR, NativeMethods.WM_IME_CHAR);
                }
                return ke.Handled;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessKeyMessage"]/*' />
        /// <devdoc>
        ///     Processes a key message. This method is called when a control receives a
        ///     keyboard message. The method first checks if the control has a parent,
        ///     and if so calls the parent's processKeyPreview() method. If the parent's
        ///     processKeyPreview() method doesn't consume the message then
        ///     processKeyEventArgs() is called to generate the appropriate keyboard events.
        ///     The m parameter contains the window message that must be
        ///     processed. Possible values for the m.msg field are WM_CHAR,
        ///     WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and WM_SYSKEYUP.
        /// When overriding processKeyMessage(), a control should return true to
        ///     indicate that it has processed the key. For keys that aren't processed
        ///     by the control, the result of "base.processKeyMessage()" should be
        ///     returned.
        /// Controls will seldom, if ever, need to override this method.
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected internal virtual bool ProcessKeyMessage(ref Message m) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessKeyMessage " + m.ToString());
            if (parent != null && parent.ProcessKeyPreview(ref m)) return true;
            return ProcessKeyEventArgs(ref m);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessKeyPreview"]/*' />
        /// <devdoc>
        ///     <para>
        ///     Previews a keyboard message. This method is called by a child control
        ///     when the child control receives a keyboard message. The child control
        ///     calls this method before generating any keyboard events for the message.
        ///     If this method returns true, the child control considers the message
        ///     consumed and does not generate any keyboard events. The m
        ///     parameter contains the window message to preview. Possible values for
        ///     the m.msg field are WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP,
        ///     and WM_SYSKEYUP.
        ///     </para>
        ///     <para>
        /// processKeyPreview() simply sends the character to the parent's
        ///     processKeyPreview() method, or returns false if the control has no
        ///     parent. The Form class overrides this method to perform actual
        ///     processing of dialog keys.
        ///     </para>
        ///     <para>
        /// When overriding processKeyPreview(), a control should return true to
        ///     indicate that it has processed the key. For keys that aren't processed
        ///     by the control, the result of "base.ProcessKeyPreview(...)" should be
        ///     returned.
        ///     </para>
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected virtual bool ProcessKeyPreview(ref Message m) {
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessKeyPreview " + m.ToString());
            return parent == null? false: parent.ProcessKeyPreview(ref m);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ProcessMnemonic"]/*' />
        /// <devdoc>
        ///     <para>
        ///     Processes a mnemonic character. This method is called to give a control
        ///     the opportunity to process a mnemonic character. The method should check
        ///     if the control is in a state to process mnemonics and if the given
        ///     character represents a mnemonic. If so, the method should perform the
        ///     action associated with the mnemonic and return true. If not, the method
        ///     should return false.
        ///     </para>
        ///     <para>
        /// Implementations of this method often use the isMnemonic() method to
        ///     check if the given character matches a mnemonic in the control's text,
        ///     for example:
        /// <code>
        ///     if (canSelect() &amp;&amp; isMnemonic(charCode, getText()) {
        ///     // perform action associated with mnemonic
        ///     }
        /// </code>
        ///     </para>
        ///     <para>
        /// This default implementation of processMnemonic() simply returns false
        ///     to indicate that the control has no mnemonic.
        ///     </para>
        /// </devdoc>
        [UIPermission(SecurityAction.InheritanceDemand, Window=UIPermissionWindow.AllWindows)]
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal virtual bool ProcessMnemonic(char charCode) {
#if DEBUG
            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.ProcessMnemonic [0x" + ((int)charCode).ToString("X", CultureInfo.InvariantCulture) + "]");
#endif
            return false;
        }

         /// <devdoc>
         ///     Preprocess keys which affect focus indicators and keyboard cues.
         /// </devdoc>
         internal void ProcessUICues(ref Message msg) {
             Keys keyCode = (Keys)((int)msg.WParam) & Keys.KeyCode;
             
             if (keyCode != Keys.F10 && keyCode != Keys.Menu && keyCode != Keys.Tab) {
                 return;  // PERF: dont WM_QUERYUISTATE if we dont have to.
             }
             
             Control topMostParent = null;
             int current = unchecked( (int) (long)SendMessage(NativeMethods.WM_QUERYUISTATE, 0, 0));

             // dont trust when a control says the accelerators are showing.
             // make sure the topmost parent agrees with this as we could be in a mismatched state.
             if (current == 0 /*accelerator and focus cues are showing*/) {
                 topMostParent = this.TopMostParent;
                 current = (int)topMostParent.SendMessage(NativeMethods.WM_QUERYUISTATE, 0, 0);
             }
             int toClear = 0;

            // if we are here, a key or tab has been pressed on this control.
            // now that we know the state of accelerators, check to see if we need
            // to show them.  NOTE: due to the strangeness of the API we OR in 
            // the opposite of what we want to do.  So if we want to show accelerators,
            // we OR in UISF_HIDEACCEL, then call UIS_CLEAR to clear the "hidden" state.
      
             if (keyCode == Keys.F10 || keyCode == Keys.Menu) {
                 if ((current & NativeMethods.UISF_HIDEACCEL) != 0) {
                     // Keyboard accelerators are hidden, they need to be shown
                     toClear |= NativeMethods.UISF_HIDEACCEL;
                 }
             }
      
             if (keyCode == Keys.Tab) {
                 if ((current & NativeMethods.UISF_HIDEFOCUS) != 0) {
                     // Focus indicators are hidden, they need to be shown
                     toClear |= NativeMethods.UISF_HIDEFOCUS;
                 }
             }

             if (toClear != 0) {
                 // We've detected some state we need to unset, usually clearing the hidden state of 
                 // the accelerators.  We need to get the topmost parent and call CHANGEUISTATE so 
                 // that the entire tree of controls is 
                 if (topMostParent == null) {
                     topMostParent = this.TopMostParent;
                 }

                 // A) if we're parented to a native dialog - REFRESH our child states ONLY
                 //       Then we've got to send a WM_UPDATEUISTATE to the topmost managed control (which will be non-toplevel)
                 //           (we assume here the root native window has changed UI state, and we're not to manage the UI state for it)
                 //
                 // B) if we're totally managed - CHANGE the root window state AND REFRESH our child states.
                 //       Then we've got to send a WM_CHANGEUISTATE to the topmost managed control (which will be toplevel)
                 //       According to MSDN, WM_CHANGEUISTATE will generate WM_UPDATEUISTATE messages for all immediate children (via DefWndProc)
                 //           (we're in charge here, we've got to change the state of the root window)
                 UnsafeNativeMethods.SendMessage(
                     new HandleRef(topMostParent, topMostParent.Handle),
                     UnsafeNativeMethods.GetParent(new HandleRef(null, topMostParent.Handle)) == IntPtr.Zero ? NativeMethods.WM_CHANGEUISTATE : NativeMethods.WM_UPDATEUISTATE,
                     (IntPtr)(NativeMethods.UIS_CLEAR | (toClear << 16)),
                     IntPtr.Zero);
             }
         }
      
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RaiseDragEvent"]/*' />
        /// <devdoc>
        ///     Raises the event associated with key with the event data of
        ///     e and a sender of this control.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseDragEvent(Object key, DragEventArgs e) {
            DragEventHandler handler = (DragEventHandler)Events[key];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RaisePaintEvent"]/*' />
        /// <devdoc>
        ///     Raises the event associated with key with the event data of
        ///     e and a sender of this control.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaisePaintEvent(Object key, PaintEventArgs e) {
            PaintEventHandler handler = (PaintEventHandler)Events[EventPaint];
            if (handler != null) handler(this, e);
        }

        private void RemovePendingMessages(int msgMin, int msgMax) {
            if (!this.IsDisposed) {
                NativeMethods.MSG msg = new NativeMethods.MSG();
                IntPtr hwnd = this.Handle;
                while (UnsafeNativeMethods.PeekMessage(ref msg, new HandleRef(this, hwnd),
                                                       msgMin, msgMax,
                                                       NativeMethods.PM_REMOVE)) {
                    ; // NULL loop
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetBackColor"]/*' />
        /// <devdoc>
        ///     Resets the back color to be based on the parent's back color.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetBackColor() {
            BackColor = Color.Empty;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetCursor"]/*' />
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetCursor() {
            Cursor = null;
        }

        private void ResetEnabled() {
            Enabled = true;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetFont"]/*' />
        /// <devdoc>
        ///     Resets the font to be based on the parent's font.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetFont() {
            Font = null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetForeColor"]/*' />
        /// <devdoc>
        ///     Resets the fore color to be based on the parent's fore color.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetForeColor() {
            ForeColor = Color.Empty;
        }

        private void ResetLocation() {
            Location = new Point(0,0);
        }

        private void ResetMargin() {
            Margin = DefaultMargin;
        }

        private void ResetMinimumSize() {
            MinimumSize = DefaultMinimumSize;
        }

       private void ResetPadding() {
           CommonProperties.ResetPadding(this);
       }

       private void ResetSize() {
            Size = DefaultSize;
       }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetRightToLeft"]/*' />
        /// <devdoc>
        ///     Resets the RightToLeft to be the default.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetRightToLeft() {
            RightToLeft = RightToLeft.Inherit;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RecreateHandle"]/*' />
        /// <devdoc>
        ///     Forces the recreation of the handle for this control. Inheriting controls
        ///     must call base.RecreateHandle.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected void RecreateHandle() {
            RecreateHandleCore();
        }

        internal virtual void RecreateHandleCore() {
            // 
            lock(this) {
                if (IsHandleCreated) {

                    bool focused = ContainsFocus;

#if DEBUG
                    if (CoreSwitches.PerfTrack.Enabled) {
                        Debug.Write("RecreateHandle: ");
                        Debug.Write(GetType().FullName);
                        Debug.Write(" [Text=");
                        Debug.Write(Text);
                        Debug.Write("]");
                        Debug.WriteLine("");
                    }
#endif
                    bool created = (state & STATE_CREATED) != 0;
                    if (GetState(STATE_TRACKINGMOUSEEVENT)) {
                        SetState(STATE_MOUSEENTERPENDING, true);
                        UnhookMouseEvent();
                    }

                    HandleRef parentHandle = new HandleRef(this, UnsafeNativeMethods.GetParent(new HandleRef(this, this.Handle)));

                    try {
                        Control [] controlSnapshot = null;
                        state |= STATE_RECREATE;

                        try {
                            // Inform child controls that their parent is recreating handle.

                            // The default behavior is to now SetParent to parking window, then
                            // SetParent back after the parent's handle has been recreated.
                            // This behavior can be overridden in OnParentHandleRecreat* and is in ListView.

                            //fish out control collection w/o demand creating one.
                            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
                            if (controlsCollection != null && controlsCollection.Count > 0) {
                                controlSnapshot = new Control[controlsCollection.Count];
                                for (int i = 0; i < controlsCollection.Count; i++) {
                                    Control childControl = controlsCollection[i];
                                    if (childControl != null && childControl.IsHandleCreated) {
                                        // SetParent to parking window
                                        childControl.OnParentHandleRecreating();

                                        // if we were successful, remember this control
                                        // so we can raise OnParentHandleRecreated
                                        controlSnapshot[i] = childControl;
                                    }
                                    else {
                                        // put in a null slot which we'll skip over later.
                                        controlSnapshot[i] = null;
                                    }
                                }
                            }

                            // do the main work of recreating the handle
                            DestroyHandle();
                            CreateHandle();
                        }
                        finally {
                            state &= ~STATE_RECREATE;

                            // inform children that their parent's handle has recreated
                            if (controlSnapshot != null) {
                                for (int i = 0; i < controlSnapshot.Length; i++) {
                                    Control childControl = controlSnapshot[i];
                                    if (childControl != null && childControl.IsHandleCreated) {
                                        // SetParent back to the new Parent handle
                                        childControl.OnParentHandleRecreated();
                                    }
                                }
                            }
                        }
                        if (created) {
                            CreateControl();
                        }
                    }
                    finally {
                        if (parentHandle.Handle != IntPtr.Zero                               // the parent was not null
                            && (Control.FromHandleInternal(parentHandle.Handle) == null || this.parent == null) // but wasnt a windows forms window
                            && UnsafeNativeMethods.IsWindow(parentHandle)) {                 // and still is a window
                            // correctly parent back up to where we were before.
                            // if we were parented to a proper windows forms control, CreateControl would have properly parented
                            // us back.
                            UnsafeNativeMethods.SetParent(new HandleRef(this, this.Handle), parentHandle);
                        }
                    }
                    

                    // Restore control focus
                    if (focused) {
                        FocusInternal();
                    }
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RectangleToClient"]/*' />
        /// <devdoc>
        ///     Computes the location of the screen rectangle r in client coords.
        /// </devdoc>
        public Rectangle RectangleToClient(Rectangle r) {
            // Reviewed: The following demand has always been commented out since v1.0. In Whidbey, we have
            // re-reviewed this method to see if it needs protection, but haven't found any threats, so are
            // going to keep it this way.
            //
            // Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ScreenLocationOfThings Demanded");
            // IntSecurity.ScreenLocationOfThings.Demand();

            NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH(r.X, r.Y, r.Width, r.Height);
            UnsafeNativeMethods.MapWindowPoints(NativeMethods.NullHandleRef, new HandleRef(this, Handle), ref rect, 2);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RectangleToScreen"]/*' />
        /// <devdoc>
        ///     Computes the location of the client rectangle r in screen coords.
        /// </devdoc>
        public Rectangle RectangleToScreen(Rectangle r) {
            // Reviewed: The following demand has always been commented out since v1.0. In Whidbey, we have
            // re-reviewed this method to see if it needs protection, but haven't found any threats, so are
            // going to keep it this way.
            //
            // Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "ScreenLocationOfThings Demanded");
            // IntSecurity.ScreenLocationOfThings.Demand();

            NativeMethods.RECT rect = NativeMethods.RECT.FromXYWH(r.X, r.Y, r.Width, r.Height);
            UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, Handle), NativeMethods.NullHandleRef, ref rect, 2);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ReflectMessage"]/*' />
        /// <devdoc>
        ///     Reflects the specified message to the control that is bound to hWnd.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static bool ReflectMessage(IntPtr hWnd, ref Message m) {
            Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "SendMessages Demanded");
            IntSecurity.SendMessages.Demand();
            return ReflectMessageInternal(hWnd, ref m);
        }
        
        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal static bool ReflectMessageInternal(IntPtr hWnd, ref Message m) {
            Control control = Control.FromHandleInternal(hWnd);
            if (control == null) return false;
            m.Result = control.SendMessage(NativeMethods.WM_REFLECT + m.Msg, m.WParam, m.LParam);
            return true;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Refresh"]/*' />
        /// <devdoc>
        ///     Forces the control to invalidate and immediately
        ///     repaint itself and any children.
        /// </devdoc>
        public virtual void Refresh() {
            Invalidate(true);
            Update();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetMouseEventArgs"]/*' />
        /// <devdoc>
        ///     Resets the mouse leave listeners.
        /// </devdoc>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void ResetMouseEventArgs() {
            if (GetState(STATE_TRACKINGMOUSEEVENT)) {
                UnhookMouseEvent();
                HookMouseEvent();
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResetText"]/*' />
        /// <devdoc>
        ///     Resets the text to it's default value.
        /// </devdoc>
        public virtual void ResetText() {
            Text = String.Empty;
        }

        private void ResetVisible() {
            Visible = true;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResumeLayout"]/*' />
        /// <devdoc>
        ///     Resumes normal layout logic. This will force a layout immediately
        ///     if there are any pending layout requests.
        /// </devdoc>
        public void ResumeLayout() {
            ResumeLayout(true);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ResumeLayout1"]/*' />
        /// <devdoc>
        ///     Resumes normal layout logic. If performLayout is set to true then
        ///     this will force a layout immediately if there are any pending layout requests.
        /// </devdoc>
        public void ResumeLayout(bool performLayout) {
#if DEBUG        
            if (CompModSwitches.LayoutSuspendResume.TraceInfo) {
                Debug.WriteLine(GetType().Name + "::ResumeLayout( preformLayout = " + performLayout + ", newCount = " + Math.Max(0, layoutSuspendCount - 1) + ")");
            }
#endif             
            Debug.Assert(layoutSuspendCount > 0, "Unbalanance suspend/resume layout.");

            bool performedLayout = false;

            if (layoutSuspendCount > 0) {

                if (layoutSuspendCount == 1) {
                    layoutSuspendCount++;
                    try {
                        OnLayoutResuming(performLayout);
                    }
                    finally {
                        layoutSuspendCount--;
                    }
                }

                layoutSuspendCount--;
                if (layoutSuspendCount == 0
                    && GetState(STATE_LAYOUTDEFERRED)
                    && performLayout) {
                        PerformLayout();
                        performedLayout = true;
                }
            }

            if (!performedLayout) {
                SetState2(STATE2_CLEARLAYOUTARGS, true);
            }
                

            /*

            We've had this since Everett,but it seems wrong, redundant and a performance hit.  The
            correct layout calls are already made when bounds or parenting changes, which is all
            we care about. We may want to call this at layout suspend count == 0, but certainaly
            not for all resumes.  I  tried removing it, and doing it only when suspendCount == 0,
            but we break things at every step.

            */
            if (!performLayout) {
                
                CommonProperties.xClearPreferredSizeCache(this);
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                if (controlsCollection != null) {
                    for (int i = 0; i < controlsCollection.Count; i++) {
                        LayoutEngine.InitLayout(controlsCollection[i], BoundsSpecified.All);
                        CommonProperties.xClearPreferredSizeCache(controlsCollection[i]);
                    }
                }
            }

        }

        /// <devdoc>
        ///     Used to actually register the control as a drop target.
        /// </devdoc>
        /// <internalonly/>
        internal void SetAcceptDrops(bool accept) {
            if (accept != GetState(STATE_DROPTARGET) && IsHandleCreated) {
                try {
                    if (Application.OleRequired() != System.Threading.ApartmentState.STA) {
                        throw new ThreadStateException(SR.ThreadMustBeSTA);
                    }
                    if (accept) {
                        IntSecurity.ClipboardRead.Demand();

                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "Registering as drop target: " + Handle.ToString());
                        // Register
                        int n = UnsafeNativeMethods.RegisterDragDrop(new HandleRef(this, Handle), (UnsafeNativeMethods.IOleDropTarget)(new DropTarget(this)));
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "   ret:" + n.ToString(CultureInfo.CurrentCulture));
                        if (n != 0 && n != NativeMethods.DRAGDROP_E_ALREADYREGISTERED) {
                            throw new Win32Exception(n);
                        }
                    }
                    else {
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "Revoking drop target: " + Handle.ToString());
                        // Revoke
                        int n = UnsafeNativeMethods.RevokeDragDrop(new HandleRef(this, Handle));
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "   ret:" + n.ToString(CultureInfo.InvariantCulture));
                        if (n != 0 && n != NativeMethods.DRAGDROP_E_NOTREGISTERED) {
                            throw new Win32Exception(n);
                        }
                    }
                    SetState(STATE_DROPTARGET, accept);
                }
                catch (Exception e) {
                    throw new InvalidOperationException(SR.DragDropRegFailed, e);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Scale"]/*' />
        /// <devdoc>
        ///     Scales to entire control and any child controls.
        /// </devdoc>
        [Obsolete("This method has been deprecated. Use the Scale(SizeF ratio) method instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Scale(float ratio) {
            ScaleCore(ratio, ratio);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Scale1"]/*' />
        /// <devdoc>
        ///     Scales the entire control and any child controls.
        /// </devdoc>
        [Obsolete("This method has been deprecated. Use the Scale(SizeF ratio) method instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Scale(float dx, float dy) {
#if DEBUG
            int dbgLayoutCheck = LayoutSuspendCount;
#endif
            SuspendLayout();
            try {
                ScaleCore(dx, dy);
            }
            finally {
                ResumeLayout();
#if DEBUG
                AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Scale2"]/*' />
        /// <devdoc>
        ///     Scales a control and its children given a scaling factor.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Scale(SizeF factor) {
            // manually call ScaleControl recursively instead of the internal scale method
            // when someone calls this method, they really do want to do some sort of 
            // zooming feature, as opposed to AutoScale.
            using (new LayoutTransaction(this, this, PropertyNames.Bounds, false)) {
                ScaleControl(factor, factor, this);
                if (ScaleChildren) {
                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
                    if (controlsCollection != null) {
                        // PERFNOTE: This is more efficient than using Foreach.  Foreach
                        // forces the creation of an array subset enum each time we
                        // enumerate
                        for (int i = 0; i < controlsCollection.Count; i++) {
                            Control c = controlsCollection[i];
                            c.Scale(factor);
                        }
                    }
                }
            }       
            
            LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);        

        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Scale3"]/*' />
        /// <devdoc>
        ///     Scales a control and its children given a pair of scaling factors.
        ///     IncludedFactor will be applied to the dimensions of controls based on
        ///     their RequiredScaling property.  For example, if a control's
        ///     RequiredScaling property returns Width, the width of the control will
        ///     be scaled according to the includedFactor value.
        ///
        ///     The excludedFactor parameter is used to scale those control bounds who
        ///     are not included in RequiredScaling.
        ///
        ///     If a factor is empty, it indicates that no scaling of those control
        ///     dimensions should be done.
        ///
        ///     The requestingControl property indicates which control has requested
        ///     the scaling function.
        /// </devdoc>
        internal virtual void Scale(SizeF includedFactor, SizeF excludedFactor, Control requestingControl) {
            // When we scale, we are establishing new baselines for the
            // positions of all controls.  Therefore, we should resume(false).
            using (new LayoutTransaction(this, this, PropertyNames.Bounds, false)) {
                ScaleControl(includedFactor, excludedFactor, requestingControl);
                ScaleChildControls(includedFactor, excludedFactor, requestingControl);
            }
            LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ScaleChildControls"]/*' />
        /// <devdoc>
        ///     Scales the children of this control.  The default implementation recursively
        ///     walks children and calls ScaleControl on each one.
        ///     IncludedFactor will be applied to the dimensions of controls based on
        ///     their RequiredScaling property.  For example, if a control's
        ///     RequiredScaling property returns Width, the width of the control will
        ///     be scaled according to the includedFactor value.
        ///
        ///     The excludedFactor parameter is used to scale those control bounds who
        ///     are not included in RequiredScaling.
        ///
        ///     If a factor is empty, it indicates that no scaling of those control
        ///     dimensions should be done.
        ///
        ///     The requestingControl property indicates which control has requested
        ///     the scaling function.
        ///
        ///     The updateWindowFontIfNeeded parameter indicates if we need to update Window
        ///     font for controls that need it, i.e. controls using default or inherited font,
        ///     that are also not user-painted.
        /// </devdoc>
        internal void ScaleChildControls(SizeF includedFactor, SizeF excludedFactor, Control requestingControl, bool updateWindowFontIfNeeded = false) {

            if (ScaleChildren) {
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                if (controlsCollection != null) {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++) {
                        Control c = controlsCollection[i];

                        // Update window font before scaling, as controls often use font metrics during scaling.
                        if (updateWindowFontIfNeeded) {
                            c.UpdateWindowFontIfNeeded();
                        }

                        c.Scale(includedFactor, excludedFactor, requestingControl);
                    }
                }
            }
        }

        /// <summary>
        /// Calls SetWindowFont if DpiHelper.IsPerMonitorV2Awareness is true,
        /// control uses default or inherited font and is not user-painted.
        /// </summary>
        internal void UpdateWindowFontIfNeeded() {
            if (DpiHelper.IsScalingRequirementMet && !GetStyle(ControlStyles.UserPaint) && (Properties.GetObject(PropFont) == null)) {
                SetWindowFont();
            }
        }

        /// <devdoc>
        ///     Scales the children of this control.  The default implementation walks the controls
        ///     collection for the control and calls Scale on each control.
        ///     IncludedFactor will be applied to the dimensions of controls based on
        ///     their RequiredScaling property.  For example, if a control's
        ///     RequiredScaling property returns Width, the width of the control will
        ///     be scaled according to the includedFactor value.
        ///
        ///     The excludedFactor parameter is used to scale those control bounds who
        ///     are not included in RequiredScaling.
        ///
        ///     If a factor is empty, it indicates that no scaling of those control
        ///     dimensions should be done.
        ///
        ///     The requestingControl property indicates which control has requested
        ///     the scaling function.
        /// </devdoc>
        internal void ScaleControl(SizeF includedFactor, SizeF excludedFactor, Control requestingControl) {
            try {
                IsCurrentlyBeingScaled = true;

                BoundsSpecified includedSpecified = BoundsSpecified.None;
                BoundsSpecified excludedSpecified = BoundsSpecified.None;

                if (!includedFactor.IsEmpty) {
                    includedSpecified = RequiredScaling;
                }

                if (!excludedFactor.IsEmpty) {
                    excludedSpecified |= (~RequiredScaling & BoundsSpecified.All);
                }
#if DEBUG
                if (CompModSwitches.RichLayout.TraceInfo) {
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Scaling {0} Included: {1}, Excluded: {2}",
                                      this, includedFactor, excludedFactor));
                }
#endif

                if (includedSpecified != BoundsSpecified.None) {
                    ScaleControl(includedFactor, includedSpecified);
                }

                if (excludedSpecified != BoundsSpecified.None) {
                    ScaleControl(excludedFactor, excludedSpecified);
                }

                if (!includedFactor.IsEmpty) {
                    RequiredScaling = BoundsSpecified.None;
                }
            }
            finally {
                IsCurrentlyBeingScaled = false;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ScaleControl"]/*' />
        /// <devdoc>
        ///     Scales an individual control's location, size, padding and margin.
        ///     If the control is top level, this will not scale the control's location.
        ///     This does not scale children or the size of auto sized controls.  You can
        ///     omit scaling in any direction by changing BoundsSpecified.
        ///
        ///     After the control is scaled the RequiredScaling property is set to
        ///     BoundsSpecified.None.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void ScaleControl(SizeF factor, BoundsSpecified specified) {

            CreateParams cp = CreateParams;
            NativeMethods.RECT adornments = new NativeMethods.RECT(0, 0, 0, 0);
            AdjustWindowRectEx(ref adornments, cp.Style, HasMenu, cp.ExStyle);
            Size minSize = MinimumSize;
            Size maxSize = MaximumSize;
     
            // clear out min and max size, otherwise this could affect the scaling logic.
            MinimumSize = Size.Empty;
            MaximumSize = Size.Empty;
     
            // this is raw because Min/Max size have been cleared at this point.
            Rectangle rawScaledBounds = GetScaledBounds(Bounds, factor, specified);

            //
            // Scale Padding and Margin
            //
            float dx = factor.Width;
            float dy = factor.Height;

            Padding padding = Padding;
            Padding margins = Margin;

            // Clear off specified bits for 1.0 scaling factors
            if (dx == 1.0F) specified &= ~(BoundsSpecified.X | BoundsSpecified.Width);
            if (dy == 1.0F) specified &= ~(BoundsSpecified.Y | BoundsSpecified.Height);

            if (dx != 1.0F) {
                padding.Left = (int)Math.Round(padding.Left * dx);
                padding.Right = (int)Math.Round(padding.Right * dx);
                margins.Left = (int)Math.Round(margins.Left * dx);
                margins.Right = (int)Math.Round(margins.Right * dx);
            }

            if (dy != 1.0F) {
                padding.Top = (int)Math.Round(padding.Top * dy);
                padding.Bottom = (int)Math.Round(padding.Bottom * dy);
                margins.Top = (int)Math.Round(margins.Top * dy);
                margins.Bottom = (int)Math.Round(margins.Bottom * dy);
            }

            // Apply padding and margins
            Padding = padding;
            Margin = margins;

            //
            // Scale Min/Max size
            //
            
            // make sure we consider the andornments as fixed.  rather than scaling the entire size,
            // we should pull out the fixed things such as the border, scale the rest, then apply the fixed
            // adornment size.
            Size adornmentSize = adornments.Size;
            if (!minSize.IsEmpty) {
                minSize -= adornmentSize;                
                minSize = ScaleSize(LayoutUtils.UnionSizes(Size.Empty, minSize), // make sure we dont go below 0.
                                        factor.Width, 
                                        factor.Height) + adornmentSize;
            }
            if (!maxSize.IsEmpty) {
                maxSize -= adornmentSize;
                maxSize = ScaleSize(LayoutUtils.UnionSizes(Size.Empty, maxSize), // make sure we dont go below 0.
                                        factor.Width, 
                                        factor.Height) + adornmentSize;
            }

        
            // Apply the min/max size constraints - dont call ApplySizeConstraints
            // as MinimumSize/MaximumSize are currently cleared out.
            Size maximumSize = LayoutUtils.ConvertZeroToUnbounded(maxSize);
            Size scaledSize = LayoutUtils.IntersectSizes(rawScaledBounds.Size, maximumSize);
            scaledSize = LayoutUtils.UnionSizes(scaledSize, minSize);

            if (DpiHelper.IsScalingRequirementMet && (ParentInternal != null) && (ParentInternal.LayoutEngine == DefaultLayout.Instance)) {
                // We need to scale AnchorInfo to update distances to container edges
                DefaultLayout.ScaleAnchorInfo((IArrangedElement)this, factor);
            }

            // Set in the scaled bounds as constrained by the newly scaled min/max size.
            SetBoundsCore(rawScaledBounds.X, rawScaledBounds.Y, scaledSize.Width, scaledSize.Height, BoundsSpecified.All);
            
            MaximumSize = maxSize;
            MinimumSize = minSize;
            
            
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ScaleCore"]/*' />
        /// <devdoc>
        ///     Performs the work of scaling the entire control and any child controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void ScaleCore(float dx, float dy) {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, GetType().Name + "::ScaleCore(" + dx + ", " + dy + ")");
#if DEBUG
            int dbgLayoutCheck = LayoutSuspendCount;
#endif
            SuspendLayout();
            try {
                int sx = (int)Math.Round(x * dx);
                int sy = (int)Math.Round(y * dy);

                int sw = width;
                if ((controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth) {
                    sw = (int)(Math.Round((x + width) * dx)) - sx;
                }
                int sh = height;
                if ((controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight) {
                    sh = (int)(Math.Round((y + height) * dy)) - sy;
                }

                SetBounds(sx, sy, sw, sh, BoundsSpecified.All);

                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);

                if (controlsCollection != null) {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++) {
#pragma warning disable 618            
                        controlsCollection[i].Scale(dx, dy);
#pragma warning restore 618
                    }
                }
            }
            finally {
                ResumeLayout();
#if DEBUG
                AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
            }
        }        

        /// <devdoc>
        ///     Scales a given size with the provided values.
        /// </devdoc>
        internal Size ScaleSize(Size startSize, float x, float y) {
            Size size = startSize;
            if (!GetStyle(ControlStyles.FixedWidth)) {
                size.Width = (int)Math.Round((float)size.Width * x);
            }
            if (!GetStyle(ControlStyles.FixedHeight)) {
                size.Height = (int)Math.Round((float)size.Height * y);
            }
            return size;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Select"]/*' />
        /// <devdoc>
        ///     Activates this control.
        /// </devdoc>
        public void Select() {
            Select(false, false);
        }

        // used by Form
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Select1"]/*' />
        protected virtual void Select(bool directed, bool forward) {
            IContainerControl c = GetContainerControlInternal();

            if (c != null) {
                c.ActiveControl = this;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SelectNextControl"]/*' />
        /// <devdoc>
        ///     Selects the next control following ctl.
        /// </devdoc>
        public bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap) {
            Control nextSelectableControl = this.GetNextSelectableControl(ctl, forward, tabStopOnly, nested, wrap);
            if (nextSelectableControl != null) {
                nextSelectableControl.Select(true, forward);
                return true;
            }
            else {
                return false;
            }
        }

        private Control GetNextSelectableControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap) {
            if (!Contains(ctl) || !nested && ctl.parent != this) ctl = null;

            bool alreadyWrapped = false;
            Control start = ctl;
            do {
                ctl = GetNextControl(ctl, forward);
                if (ctl == null) {
                    if (!wrap) break;
                    if (alreadyWrapped) {
                        return null; //prevent infinite wrapping.
                    }
                    alreadyWrapped = true;
                }
                else {
                    if (ctl.CanSelect
                        && (!tabStopOnly || ctl.TabStop)
                        && (nested || ctl.parent == this)) {

                        if (AccessibilityImprovements.Level3 && ctl.parent is ToolStrip) {
                            continue;
                        }
                        return ctl;
                    }
                }
            } while (ctl != start);
            return null;
        }


        /// <devdoc>
        ///     Unsafe internal version of SelectNextControl - Use with caution!
        /// </devdoc>
        internal bool SelectNextControlInternal(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap) {
            return SelectNextControl(ctl, forward, tabStopOnly, nested, wrap);
        }

        /// <devdoc>
        ///     This is called recursively when visibility is changed for a control, this
        ///     forces focus to be moved to a visible control.
        /// </devdoc>
        private void SelectNextIfFocused() {
            //           We want to move focus away from hidden controls, so this
            //           function was added.
            //
            if (ContainsFocus && ParentInternal != null) {
                IContainerControl c = ParentInternal.GetContainerControlInternal();

                if (c != null) {
                    // 



                    ((Control)c).SelectNextControlInternal(this, true, true, true, true);
                }
            }
        }

        /// <devdoc>
        ///     Sends a Win32 message to this control.  If the control does not yet
        ///     have a handle, it will be created.
        /// </devdoc>
        internal IntPtr SendMessage(int msg, int wparam, int lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        /// <devdoc>
        ///     Sends a Win32 message to this control.  If the control does not yet
        ///     have a handle, it will be created.
        /// </devdoc>
        internal IntPtr SendMessage(int msg, ref int wparam, ref int lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, ref wparam, ref lparam);
        }
        internal IntPtr SendMessage(int msg, int wparam, IntPtr lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, (IntPtr)wparam, lparam);
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, IntPtr lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, int lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, (IntPtr)lparam);
        }

        /// <devdoc>
        ///     Sends a Win32 message to this control.  If the control does not yet
        ///     have a handle, it will be created.
        /// </devdoc>
        internal IntPtr SendMessage(int msg, int wparam, ref NativeMethods.RECT lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, ref lparam);
        }

        /// <devdoc>
        ///     Sends a Win32 message to this control.  If the control does not yet
        ///     have a handle, it will be created.
        /// </devdoc>
        internal IntPtr SendMessage(int msg, bool wparam, int lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        /// <devdoc>
        ///     Sends a Win32 message to this control.  If the control does not yet
        ///     have a handle, it will be created.
        /// </devdoc>
        internal IntPtr SendMessage(int msg, int wparam, string lparam) {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SendToBack"]/*' />
        /// <devdoc>
        ///     sends this control to the back of the z-order
        /// </devdoc>
        public void SendToBack() {
            if (parent != null) {
                parent.Controls.SetChildIndex(this, -1);
            }
            else if (IsHandleCreated && GetTopLevel()) {
                SafeNativeMethods.SetWindowPos(new HandleRef(window, Handle), NativeMethods.HWND_BOTTOM, 0, 0, 0, 0,
                                               NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            }

        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetBounds"]/*' />
        /// <devdoc>
        ///     Sets the bounds of the control.
        /// </devdoc>
        public void SetBounds(int x, int y, int width, int height) {
            if (this.x != x || this.y != y || this.width != width ||
                this.height != height) {
                SetBoundsCore(x, y, width, height, BoundsSpecified.All);

                // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
                // have refreshed the interior layout.  We only need to layout the parent.
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
            }
            else {
                // Still need to init scaling.
                InitScaling(BoundsSpecified.All);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetBounds1"]/*' />
        /// <devdoc>
        ///     Sets the bounds of the control.
        /// </devdoc>
        public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified) {
            if ((specified & BoundsSpecified.X) == BoundsSpecified.None) x = this.x;
            if ((specified & BoundsSpecified.Y) == BoundsSpecified.None) y = this.y;
            if ((specified & BoundsSpecified.Width) == BoundsSpecified.None) width = this.width;
            if ((specified & BoundsSpecified.Height) == BoundsSpecified.None) height = this.height;
            if (this.x != x || this.y != y || this.width != width ||
                this.height != height) {
                SetBoundsCore(x, y, width, height, specified);

                // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
                // have refreshed the interior layout or the resized control.  We only need to layout
                // the parent.  This happens after InitLayout has been invoked.
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
            }
            else {
                // Still need to init scaling.
                InitScaling(specified);
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetBoundsCore"]/*' />
        /// <devdoc>
        ///     Performs the work of setting the bounds of this control. Inheriting
        ///     classes can overide this function to add size restrictions. Inheriting
        ///     classes must call base.setBoundsCore to actually cause the bounds
        ///     of the control to change.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
#if DEBUG        
            if (CompModSwitches.SetBounds.TraceInfo) {
                Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0}::SetBoundsCore(x={1} y={2} width={3} height={4} specified={5}", Name, x, y, width, height, specified));
            }
#endif
            // SetWindowPos below sends a WmWindowPositionChanged (not posts) so we immediately
            // end up in WmWindowPositionChanged which may cause the parent to layout.  We need to
            // suspend/resume to defer the parent from laying out until after InitLayout has been called
            // to update the layout engine's state with the new control bounds.
#if DEBUG
            int suspendCount = -44371;      // Arbitrary negative prime to surface bugs.
#endif
            if (ParentInternal != null) {
#if DEBUG
                suspendCount = ParentInternal.LayoutSuspendCount;
#endif
                ParentInternal.SuspendLayout();
            }
            try {
                if (this.x != x || this.y != y || this.width != width || this.height != height) {
                    CommonProperties.UpdateSpecifiedBounds(this, x, y, width, height, specified);

                    // Provide control with an opportunity to apply self imposed constraints on its size.
                    Rectangle adjustedBounds = ApplyBoundsConstraints(x,y, width, height);
                    width = adjustedBounds.Width;
                    height = adjustedBounds.Height;
                    x = adjustedBounds.X;
                    y = adjustedBounds.Y;

                    if (!IsHandleCreated) {
                        // Handle is not created, just record our new position and we're done.
                        UpdateBounds(x, y, width, height);
                    } else {
                        if (!GetState(STATE_SIZELOCKEDBYOS)) {
                            int flags = NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE;

                            if (this.x == x && this.y == y) {
                                flags |= NativeMethods.SWP_NOMOVE;
                            }
                            if (this.width == width && this.height == height) {
                                flags |= NativeMethods.SWP_NOSIZE;
                            }

                            //
                            // Give a chance for derived controls to do what they want, just before we resize.
                            OnBoundsUpdate(x, y, width, height);

                            SafeNativeMethods.SetWindowPos(new HandleRef(window, Handle), NativeMethods.NullHandleRef, x, y, width, height, flags);

                            // NOTE: SetWindowPos causes a WM_WINDOWPOSCHANGED which is processed
                            // synchonously so we effectively end up in UpdateBounds immediately following
                            // SetWindowPos.
                            //
                            //UpdateBounds(x, y, width, height);
                        }
                    }
                }
            }
            finally {

                // Initialize the scaling engine.
                InitScaling(specified);

                if (ParentInternal != null) {
                    // Some layout engines (DefaultLayout) base their PreferredSize on
                    // the bounds of their children.  If we change change the child bounds, we
                    // need to clear their PreferredSize cache.  The semantics of SetBoundsCore
                    // is that it does not cause a layout, so we just clear.
                    CommonProperties.xClearPreferredSizeCache(ParentInternal);

                    // Cause the current control to initialize its layout (e.g., Anchored controls
                    // memorize their distance from their parent's edges).  It is your parent's
                    // LayoutEngine which manages your layout, so we call into the parent's
                    // LayoutEngine.
                    ParentInternal.LayoutEngine.InitLayout(this, specified);
                    ParentInternal.ResumeLayout( /* performLayout = */ true );
#if DEBUG
                    Debug.Assert(ParentInternal.LayoutSuspendCount == suspendCount, "Suspend/Resume layout mismatch!");
#endif
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetClientSizeCore"]/*' />
        /// <devdoc>
        ///     Performs the work of setting the size of the client area of the control.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void SetClientSizeCore(int x, int y) {
            Size = SizeFromClientSize(x, y);
            clientWidth = x;
            clientHeight = y;
            OnClientSizeChanged(EventArgs.Empty);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SizeFromClientSize"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual Size SizeFromClientSize(Size clientSize) {
            return SizeFromClientSize(clientSize.Width, clientSize.Height);
        }

        internal Size SizeFromClientSize(int width, int height) {
            NativeMethods.RECT rect = new NativeMethods.RECT(0, 0, width, height);

            CreateParams cp = CreateParams;
            AdjustWindowRectEx(ref rect, cp.Style, HasMenu, cp.ExStyle);
            return rect.Size;
        }

        private void SetHandle(IntPtr value) {
            if (value == IntPtr.Zero) {
                SetState(STATE_CREATED, false);
            }
            UpdateRoot();
        }

        private void SetParentHandle(IntPtr value) {
            Debug.Assert(value != NativeMethods.InvalidIntPtr, "Outdated call to SetParentHandle");

            if (IsHandleCreated) {
                IntPtr parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(window, Handle));
                bool topLevel = GetTopLevel();
                if (parentHandle != value || (parentHandle == IntPtr.Zero && !topLevel)) {
                    Debug.Assert(Handle != value, "Cycle created in SetParentHandle");
                    
                    bool recreate = (parentHandle == IntPtr.Zero && !topLevel)
                        || (value == IntPtr.Zero && topLevel);

                    if (recreate) {
                        // We will recreate later, when the MdiChild's visibility
                        // is set to true (see 
                        Form f = this as Form;
                        if (f != null) {
                            if (!f.CanRecreateHandle()) {
                                recreate = false;
                                // we don't want to recreate - but our styles may have changed.
                                // before we unpark the window below we need to update
                                UpdateStyles();
                            }
                        }
                    }

                    if (recreate) {
                        RecreateHandle();
                    }
                    if (!GetTopLevel()) {
                        if (value == IntPtr.Zero) {
                            Application.ParkHandle(new HandleRef(window, Handle));
                            UpdateRoot();
                        }
                        else {
                            UnsafeNativeMethods.SetParent(new HandleRef(window, Handle), new HandleRef(null, value));
                            if (parent != null) {
                                parent.UpdateChildZOrder(this);
                            }
                            Application.UnparkHandle(new HandleRef(window, Handle), window.DpiAwarenessContext);
                        }
                    }
                }
                else if (value == IntPtr.Zero && parentHandle == IntPtr.Zero && topLevel) {
                    // The handle was previously parented to the parking window. Its TopLevel property was 
                    // then changed to true so the above call to GetParent returns null even though the parent of the control is 
                    // not null. We need to explicitly set the parent to null.
                    UnsafeNativeMethods.SetParent(new HandleRef(window, Handle), new HandleRef(null, IntPtr.Zero));
                    Application.UnparkHandle(new HandleRef(window, Handle), window.DpiAwarenessContext);
                }
            }
        }

        // Form, UserControl, AxHost usage
        internal void SetState(int flag, bool value) {
            state = value? state | flag: state & ~flag;
        }

        // Application, SKWindow usage
        internal void SetState2(int flag, bool value) {
            state2 = value ? state2 | flag : state2 & ~flag;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetStyle"]/*' />
        /// <devdoc>
        ///     Sets the current value of the specified bit in the control's style.
        ///     NOTE: This is control style, not the Win32 style of the hWnd.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void SetStyle(ControlStyles flag, bool value) {
            // WARNING: if we ever add argument checking to "flag", we will need
            // to move private styles like Layered to State.
            // Calling SetStyle(ControlStyles.EnableNotifyMessage,...) should require UnmanagedCode
            if ((flag & ControlStyles.EnableNotifyMessage) != 0 && value)
            {
                // demand security permission for this condition.
                // this will throw security exception in semi-trust.
                IntSecurity.UnmanagedCode.Demand();
            }
            controlStyle = value? controlStyle | flag: controlStyle & ~flag;
        }

        internal static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette) {
            Debug.WriteLineIf(Control.PaletteTracing.TraceVerbose, "SetUpPalette(force:=" + force + ", ralizePalette:=" + realizePalette + ")");

            IntPtr halftonePalette = Graphics.GetHalftonePalette();

            Debug.WriteLineIf(Control.PaletteTracing.TraceVerbose, "select palette " + !force);
            IntPtr result = SafeNativeMethods.SelectPalette(new HandleRef(null, dc), new HandleRef(null, halftonePalette), (force ? 0 : 1));

            if (result != IntPtr.Zero && realizePalette) {
                SafeNativeMethods.RealizePalette(new HandleRef(null, dc));
            }

            return result;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetTopLevel"]/*' />
        protected void SetTopLevel(bool value) {
            if (value && IsActiveX) {
                throw new InvalidOperationException(SR.TopLevelNotAllowedIfActiveX);
            }
            else {
                if (value) {
                    if (this is Form) {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "TopLevelWindow Demanded");
                        IntSecurity.TopLevelWindow.Demand();
                    }
                    else {
                        Debug.WriteLineIf(IntSecurity.SecurityDemand.TraceVerbose, "UnrestrictedWindows Demanded");
                        IntSecurity.UnrestrictedWindows.Demand();
                    }
                }

                SetTopLevelInternal(value);
            }
        }

        // SECURITY WARNING: This method bypasses a security demand. Use with caution!
        internal void SetTopLevelInternal(bool value) {

            if (GetTopLevel() != value) {
                if (parent != null) {
                    throw new ArgumentException(SR.TopLevelParentedControl, "value");
                }
                SetState(STATE_TOPLEVEL, value);
                // make sure the handle is created before hooking, otherwise a toplevel control that never
                // creates its handle will leak.
                if (IsHandleCreated && GetState2(STATE2_INTERESTEDINUSERPREFERENCECHANGED)) {
                    ListenToUserPreferenceChanged(value);
                }
                UpdateStyles();
                SetParentHandle(IntPtr.Zero);
                if (value && Visible) {
                    CreateControl();
                }
                UpdateRoot();
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SetVisibleCore"]/*' />
        protected virtual void SetVisibleCore(bool value) {
            try {
                System.Internal.HandleCollector.SuspendCollect();

                if (GetVisibleCore() != value) {
                    if (!value) {
                        SelectNextIfFocused();
                    }

                    bool fireChange = false;

                    if (GetTopLevel()) {

                        // The processing of WmShowWindow will set the visibility
                        // bit and call CreateControl()
                        //
                        if (IsHandleCreated || value) {
                                SafeNativeMethods.ShowWindow(new HandleRef(this, Handle), value ? ShowParams : NativeMethods.SW_HIDE);
                        }
                    }
                    else if (IsHandleCreated || value && parent != null && parent.Created) {

                        // We want to mark the control as visible so that CreateControl
                        // knows that we are going to be displayed... however in case
                        // an exception is thrown, we need to back the change out.
                        //
                        SetState(STATE_VISIBLE, value);
                        fireChange = true;
                        try {
                            if (value) CreateControl();
                            SafeNativeMethods.SetWindowPos(new HandleRef(window, Handle),
                                                           NativeMethods.NullHandleRef,
                                                           0, 0, 0, 0,
                                                           NativeMethods.SWP_NOSIZE
                                                           | NativeMethods.SWP_NOMOVE
                                                           | NativeMethods.SWP_NOZORDER
                                                           | NativeMethods.SWP_NOACTIVATE
                                                           | (value ? NativeMethods.SWP_SHOWWINDOW : NativeMethods.SWP_HIDEWINDOW));
                        }
                        catch {
                            SetState(STATE_VISIBLE, !value);
                            throw;
                        }
                    }
                    if (GetVisibleCore() != value) {
                        SetState(STATE_VISIBLE, value);
                        fireChange = true;
                    }

                    if (fireChange) {
                        // We do not do this in the OnPropertyChanged event for visible
                        // Lots of things could cause us to become visible, including a
                        // parent window.  We do not want to indescriminiately layout
                        // due to this, but we do want to layout if the user changed
                        // our visibility.
                        //

                        using (new LayoutTransaction(parent, this, PropertyNames.Visible)) {
                            OnVisibleChanged(EventArgs.Empty);
                        }
                    }
                    UpdateRoot();
                }
                else { // value of Visible property not changed, but raw bit may have

                    if (!GetState(STATE_VISIBLE) && !value && IsHandleCreated) {
                        // PERF - setting Visible=false twice can get us into this else block
                        // which makes us process WM_WINDOWPOS* messages - make sure we've already 
                        // visible=false - if not, make it so.
                         if (!SafeNativeMethods.IsWindowVisible(new HandleRef(this,this.Handle))) {
                            // we're already invisible - bail.
                            return;
                         }
                    }
                    
                    SetState(STATE_VISIBLE, value);

                    // If the handle is already created, we need to update the window style.
                    // This situation occurs when the parent control is not currently visible,
                    // but the child control has already been created.
                    //
                    if (IsHandleCreated) {
                       
                        SafeNativeMethods.SetWindowPos(
                                                          new HandleRef(window, Handle), NativeMethods.NullHandleRef, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE |
                                                          NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE |
                                                          (value ? NativeMethods.SWP_SHOWWINDOW : NativeMethods.SWP_HIDEWINDOW));
                    }
                }
            }
            finally {
                System.Internal.HandleCollector.ResumeCollect();
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="ContainerControl.GetAutoValidateForControl"]/*' />
        /// <devdoc>
        ///     Determine effective auto-validation setting for a given control, based on the AutoValidate property
        ///     of its containing control. Defaults to 'EnablePreventFocusChange' if there is no containing control
        ///     (eg. because this control is a top-level container).
        /// </devdoc>
        /// <internalonly/>
        internal static AutoValidate GetAutoValidateForControl(Control control) {
            ContainerControl parent = control.ParentContainerControl;
            return (parent != null) ? parent.AutoValidate : AutoValidate.EnablePreventFocusChange;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldAutoValidate"]/*' />
        /// <devdoc>
        ///     Is auto-validation currently in effect for this control?
        ///     Depends on the AutoValidate property of the containing control.
        /// </devdoc>
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldAutoValidate {
            get {
                return GetAutoValidateForControl(this) != AutoValidate.Disable;
            }
        }

        // This method is called in PerformContainerValidation to check if this control supports containerValidation.
        // TabControl overrides this method to return true.
        internal virtual bool ShouldPerformContainerValidation() {
            return GetStyle(ControlStyles.ContainerControl);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeBackColor"]/*' />
        /// <devdoc>
        ///     Returns true if the backColor should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeBackColor() {
            Color backColor = Properties.GetColor(PropBackColor);
            return !backColor.IsEmpty;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeCursor"]/*' />
        /// <devdoc>
        ///     Returns true if the cursor should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeCursor() {
            bool found;
            object cursor = Properties.GetObject(PropCursor, out found);
            return (found && cursor != null);
        }

        /// <devdoc>
        ///     Returns true if the enabled property should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEnabled() {
            return (!GetState(STATE_ENABLED));
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeForeColor"]/*' />
        /// <devdoc>
        ///     Returns true if the foreColor should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeForeColor() {
            Color foreColor = Properties.GetColor(PropForeColor);
            return !foreColor.IsEmpty;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeFont"]/*' />
        /// <devdoc>
        ///     Returns true if the font should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeFont() {
            bool found;
            object font = Properties.GetObject(PropFont, out found);
            return (found && font != null);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeRightToLeft"]/*' />
        /// <devdoc>
        ///     Returns true if the RightToLeft should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeRightToLeft() {
            bool found;
            int rtl = Properties.GetInteger(PropRightToLeft, out found);
            return (found && rtl != (int)RightToLeft.Inherit);
        }

        /// <devdoc>
        ///     Returns true if the visible property should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeVisible() {
            return (!GetState(STATE_VISIBLE));
        }

        // Helper function - translates text alignment for Rtl controls
        // Read TextAlign as Left == Near, Right == Far
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RtlTranslateAlignment"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align) {
            return RtlTranslateHorizontal(align);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RtlTranslateAlignment1"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align) {
            return RtlTranslateLeftRight(align);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RtlTranslateAlignment2"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected ContentAlignment RtlTranslateAlignment(ContentAlignment align) {
            return RtlTranslateContent(align);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RtlTranslateHorizontal"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align) {

            if (RightToLeft.Yes == RightToLeft) {
                if (HorizontalAlignment.Left == align) {
                    return HorizontalAlignment.Right;
                }
                else if (HorizontalAlignment.Right == align) {
                    return HorizontalAlignment.Left;
                }
            }

            return align;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RtlTranslateLeftRight"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align) {

            if (RightToLeft.Yes == RightToLeft) {
                if (LeftRightAlignment.Left == align) {
                    return LeftRightAlignment.Right;
                }
                else if (LeftRightAlignment.Right == align) {
                    return LeftRightAlignment.Left;
                }
            }

            return align;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.RtlTranslateContent"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal ContentAlignment RtlTranslateContent(ContentAlignment align) {
            if (RightToLeft.Yes == RightToLeft) {

                if ((align & WindowsFormsUtils.AnyTopAlign) != 0) {
                    switch (align) {
                        case ContentAlignment.TopLeft:
                            return ContentAlignment.TopRight;
                        case ContentAlignment.TopRight:
                            return ContentAlignment.TopLeft;                                                           
                    }
                }
                if ((align & WindowsFormsUtils.AnyMiddleAlign) != 0 ) {
                    switch (align) {
                        case ContentAlignment.MiddleLeft:
                            return ContentAlignment.MiddleRight;
                        case ContentAlignment.MiddleRight:
                            return ContentAlignment.MiddleLeft;                                                           
                    }
                }

                if ((align & WindowsFormsUtils.AnyBottomAlign) != 0 ) {
                    switch (align) {
                        case ContentAlignment.BottomLeft:
                            return ContentAlignment.BottomRight;
                        case ContentAlignment.BottomRight:
                            return ContentAlignment.BottomLeft;                                                           
                    }
                }          
            }
            return align;
        }

        private void SetWindowFont(){
            SendMessage(NativeMethods.WM_SETFONT, FontHandle, 0 /*redraw = false*/);
        }

        private void SetWindowStyle(int flag, bool value) {
            int styleFlags = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
            UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)(value? styleFlags | flag: styleFlags & ~flag)));
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Show"]/*' />
        /// <devdoc>
        ///     Makes the control display by setting the visible property to true
        /// </devdoc>
        public void Show() {
            Visible = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeMargin() {
            return !Margin.Equals(DefaultMargin);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeMaximumSize() {
            return MaximumSize != DefaultMaximumSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeMinimumSize() {
            return MinimumSize != DefaultMinimumSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializePadding() {
            return !Padding.Equals(DefaultPadding);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeSize"]/*' />
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.Control.Size'/> property needs to be persisted.</para>
        /// </devdoc>        
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeSize() {
            // In Whidbey the ControlDesigner class will always serialize size as it replaces the Size
            // property descriptor with its own.  This is here for compat.                    
            Size s = DefaultSize;
            return width != s.Width || height != s.Height;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ShouldSerializeText"]/*' />
        /// <devdoc>
        /// <para>Determines if the <see cref='System.Windows.Forms.Control.Text'/> property needs to be persisted.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeText() {
            return Text.Length != 0;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.SuspendLayout"]/*' />
        /// <devdoc>
        ///     Suspends the layout logic for the control.
        /// </devdoc>
        public void SuspendLayout() {

           
            layoutSuspendCount++;
            if (layoutSuspendCount == 1) {
                OnLayoutSuspended();
            }

#if DEBUG          
            Debug.Assert(layoutSuspendCount > 0, "SuspendLayout: layoutSuspendCount overflowed.");
            if (CompModSwitches.LayoutSuspendResume.TraceInfo) {   
                Debug.WriteLine(GetType().Name + "::SuspendLayout( newCount = " + layoutSuspendCount + ")");
            }
#endif            
        }

        /// <devdoc>
        ///     Stops listening for the mouse leave event.
        /// </devdoc>
        /// <internalonly/>
        private void UnhookMouseEvent() {
            SetState(STATE_TRACKINGMOUSEEVENT, false);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.Update"]/*' />
        /// <devdoc>
        ///     Forces the control to paint any currently invalid areas.
        /// </devdoc>
        public void Update() {
            SafeNativeMethods.UpdateWindow(new HandleRef(window, InternalHandle));
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UpdateBounds"]/*' />
        /// <devdoc>
        ///     Updates the bounds of the control based on the handle the control is
        ///     bound to.
        /// </devdoc>
        // Internal for ScrollableControl
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal void UpdateBounds() {
            NativeMethods.RECT rect = new NativeMethods.RECT();
            UnsafeNativeMethods.GetClientRect(new HandleRef(window, InternalHandle), ref rect);
            int clientWidth = rect.right;
            int clientHeight = rect.bottom;
            UnsafeNativeMethods.GetWindowRect(new HandleRef(window, InternalHandle), ref rect);
            if (!GetTopLevel()) {
                UnsafeNativeMethods.MapWindowPoints(NativeMethods.NullHandleRef, new HandleRef(null, UnsafeNativeMethods.GetParent(new HandleRef(window, InternalHandle))), ref rect, 2);
            }

            UpdateBounds(rect.left, rect.top, rect.right - rect.left,
                         rect.bottom - rect.top, clientWidth, clientHeight);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UpdateBounds1"]/*' />
        /// <devdoc>
        ///     Updates the bounds of the control based on the bounds passed in.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateBounds(int x, int y, int width, int height) {
            Debug.Assert(!IsHandleCreated, "Don't call this method when handle is created!!");

            // reverse-engineer the AdjustWindowRectEx call to figure out
            // the appropriate clientWidth and clientHeight
            NativeMethods.RECT rect = new NativeMethods.RECT();
            rect.left = rect.right = rect.top = rect.bottom = 0;

            CreateParams cp = CreateParams;

            AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);
            int clientWidth = width - (rect.right - rect.left);
            int clientHeight = height - (rect.bottom - rect.top);
            UpdateBounds(x, y, width, height, clientWidth, clientHeight);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UpdateBounds2"]/*' />
        /// <devdoc>
        ///     Updates the bounds of the control based on the bounds passed in.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight) {
#if DEBUG
            if (CompModSwitches.SetBounds.TraceVerbose){
               Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "{0}::UpdateBounds(", Name));
               Debug.Indent();
               Debug.WriteLine(String.Format(
                    CultureInfo.CurrentCulture, "oldBounds={{x={0} y={1} width={2} height={3} clientWidth={4} clientHeight={5}}}",
                    this.x, this.y, this.width, this.height, this.clientWidth, this.clientHeight));
            }
#endif // DEBUG


            bool newLocation = this.x != x || this.y != y;
            bool newSize = this.Width != width || this.Height != height ||
                           this.clientWidth != clientWidth || this.clientHeight != clientHeight;

            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.clientWidth = clientWidth;
            this.clientHeight = clientHeight;

            if (newLocation) {
#if DEBUG
                Rectangle originalBounds = this.Bounds;
#endif 
                OnLocationChanged(EventArgs.Empty);
#if DEBUG
                if (this.Bounds != originalBounds && CompModSwitches.SetBounds.TraceWarning) {
                    Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "WARNING: Bounds changed during OnLocationChanged()\r\nbefore={0} after={1}", originalBounds, this.Bounds));
                }
#endif 
            }
            if (newSize) {
#if DEBUG
                Rectangle originalBounds = this.Bounds;
#endif 
                OnSizeChanged(EventArgs.Empty);
                OnClientSizeChanged(EventArgs.Empty);

                // Clear PreferredSize cache for this control
                CommonProperties.xClearPreferredSizeCache(this);
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);

#if DEBUG
                if (this.Bounds != originalBounds && CompModSwitches.SetBounds.TraceWarning) {
                    Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "WARNING: Bounds changed during OnSizeChanged()\r\nbefore={0} after={1}", originalBounds, this.Bounds));
                }
#endif 
            }

           
#if DEBUG
            if (CompModSwitches.SetBounds.TraceVerbose) {
                Debug.WriteLine(String.Format(CultureInfo.CurrentCulture, "newBounds={{x={0} y={1} width={2} height={3} clientWidth={4} clientHeight={5}}}",x, y, width, height, clientWidth, clientHeight));
                Debug.Unindent();
            }
#endif 
        }

        /// <devdoc>
        ///     Updates the binding manager bindings when the binding proeprty changes.
        ///     We have the code here, rather than in PropertyChagned, so we don't pull
        ///     in the data assembly if it's not used.
        /// </devdoc>
        private void UpdateBindings() {
            for (int i = 0; i < DataBindings.Count; i++) {
                BindingContext.UpdateBinding(BindingContext, DataBindings[i]);
            }
        }

        /// <devdoc>
        ///     Updates the child control's position in the control array to correctly
        ///     reflect it's index.
        /// </devdoc>
        private void UpdateChildControlIndex(Control ctl) {
            // Don't reorder the child control array for tab controls. Implemented as a special case
            // in order to keep the method private.
            if (!LocalAppContextSwitches.AllowUpdateChildControlIndexForTabControls) {
                if (this.GetType().IsAssignableFrom(typeof(TabControl))) {
                    return;
                }
            }

            int newIndex = 0;
            int curIndex = this.Controls.GetChildIndex(ctl);
            IntPtr hWnd = ctl.InternalHandle;
            while ((hWnd = UnsafeNativeMethods.GetWindow(new HandleRef(null, hWnd), NativeMethods.GW_HWNDPREV)) != IntPtr.Zero) {
                Control c = FromHandleInternal(hWnd);
                if (c != null) {
                    newIndex = this.Controls.GetChildIndex(c, false) + 1;
                    break;
                }
            }
            if (newIndex > curIndex) {
                newIndex--;
            }
            if (newIndex != curIndex)
            {
                this.Controls.SetChildIndex(ctl, newIndex);
            }
        }


        // whenever we create our handle, we need to get ahold of the HWND of our parent, 
        // and track if that thing is destroyed, because any messages sent to our parent (e.g. WM_NOTIFY, WM_DRAWITEM, etc)
        // will continue to be sent to that window even after we do a SetParent call until the handle is recreated.
        // So here we keep track of that, and if that window ever gets destroyed, we'll recreate our handle.
        //
        //
        // Scenario is when you've got a control in one parent, you move it to another, then destroy the first parent.  It'll stop 
        // getting any reflected messages because Windows will send them to the original parent.
        //
        private void UpdateReflectParent(bool findNewParent) {
            if (!Disposing && findNewParent && IsHandleCreated) {
                IntPtr parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(this, Handle));
                if (parentHandle != IntPtr.Zero) {
                    ReflectParent = Control.FromHandleInternal(parentHandle);
                    return;
                }
            }
            ReflectParent = null;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UpdateZOrder"]/*' />
        /// <devdoc>
        ///     Updates this control in it's parent's zorder.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateZOrder() {
            if (parent != null) {
                parent.UpdateChildZOrder(this);
            }
        }

        /// <devdoc>
        ///     Syncs the ZOrder of child control to the index we want it to be.
        /// </devdoc>
        private void UpdateChildZOrder(Control ctl) {
            if (!IsHandleCreated || !ctl.IsHandleCreated || ctl.parent != this) return;
            IntPtr prevHandle = (IntPtr)NativeMethods.HWND_TOP;
            for (int i = this.Controls.GetChildIndex(ctl); --i >= 0;) {
                Control c = Controls[i];
                if (c.IsHandleCreated && c.parent == this) {
                    prevHandle = c.Handle;
                    break;
                }
            }
            if (UnsafeNativeMethods.GetWindow(new HandleRef(ctl.window, ctl.Handle), NativeMethods.GW_HWNDPREV) != prevHandle) {
                state |= STATE_NOZORDER;
                try {
                    SafeNativeMethods.SetWindowPos(new HandleRef(ctl.window, ctl.Handle), new HandleRef(null, prevHandle), 0, 0, 0, 0,
                                                   NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                }
                finally {
                    state &= ~STATE_NOZORDER;
                }
            }
        }

        /// <devdoc>
        ///     Updates the rootReference in the bound window.
        ///     (Used to prevent visible top-level controls from being garbage collected)
        /// </devdoc>
        private void UpdateRoot() {
            window.LockReference(GetTopLevel() && Visible);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UpdateStyles"]/*' />
        /// <devdoc>
        ///     Forces styles to be reapplied to the handle. This function will call
        ///     CreateParams to get the styles to apply.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateStyles() {
            UpdateStylesCore();

            OnStyleChanged(EventArgs.Empty);
        }

        internal virtual void UpdateStylesCore() {
            if (IsHandleCreated) {
                CreateParams cp = CreateParams;
                int winStyle = WindowStyle;
                int exStyle = WindowExStyle;

                // resolve the Form's lazy visibility.
                if ((state & STATE_VISIBLE) != 0){
                    cp.Style |= NativeMethods.WS_VISIBLE;
                }
                if (winStyle != cp.Style) {
                    WindowStyle = cp.Style;
                }
                if (exStyle != cp.ExStyle) {
                    WindowExStyle = cp.ExStyle;
                    SetState(STATE_MIRRORED, (cp.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) != 0);
                }

                SafeNativeMethods.SetWindowPos(
                                              new HandleRef(this, Handle), NativeMethods.NullHandleRef, 0, 0, 0, 0,
                                              NativeMethods.SWP_DRAWFRAME | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE
                                              | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER);

                Invalidate(true);
            }
        }
        
        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref) {
            if (pref.Category == UserPreferenceCategory.Color) {
                defaultFont = null;
                OnSystemColorsChanged(EventArgs.Empty);
            }
        }

        //
        // Give a chance for derived controls to do what they want, just before we resize.
        internal virtual void OnBoundsUpdate(int x, int y, int width, int height)
        {
        }

        // These Window* methods allow us to keep access to the "window"
        // property private, which is important for restricting access to the
        // handle.
        internal void WindowAssignHandle(IntPtr handle, bool value)
        {
            window.AssignHandle(handle, value);
        }

        internal void WindowReleaseHandle()
        {
            window.ReleaseHandle();
        }

        private void WmClose(ref Message m) {
            
            if (ParentInternal != null) {
                IntPtr parentHandle = Handle;
                IntPtr lastParentHandle = parentHandle;

                while (parentHandle != IntPtr.Zero) {
                    lastParentHandle = parentHandle;
                    parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(null, parentHandle));

                    int style = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(null, lastParentHandle), NativeMethods.GWL_STYLE)));

                    if ((style & NativeMethods.WS_CHILD) == 0) {
                        break;
                    }

                }

                if (lastParentHandle != IntPtr.Zero) {
                    UnsafeNativeMethods.PostMessage(new HandleRef(null, lastParentHandle), NativeMethods.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }

            DefWndProc(ref m);
        }


        /// <devdoc>
        ///     Handles the WM_CAPTURECHANGED message
        /// </devdoc>
        /// <internalonly/>
        private void WmCaptureChanged(ref Message m) {
            OnMouseCaptureChanged(EventArgs.Empty);
            DefWndProc(ref m);

        }
        /// <devdoc>
        ///     Handles the WM_COMMAND message
        /// </devdoc>
        /// <internalonly/>
        private void WmCommand(ref Message m) {
            if (IntPtr.Zero == m.LParam) {
                if (Command.DispatchID(NativeMethods.Util.LOWORD(m.WParam))) return;
            }
            else {
                if (ReflectMessageInternal(m.LParam, ref m)) {
                    return;
                }
            }
            DefWndProc(ref m);
        }

        // overridable so nested controls can provide a different source control.
        internal virtual void WmContextMenu(ref Message m) {
            WmContextMenu(ref m, this);
        }
        /// <devdoc>
        ///     Handles the WM_CONTEXTMENU message
        /// </devdoc>
        /// <internalonly/>
        internal void WmContextMenu(ref Message m, Control sourceControl) {
            ContextMenu contextMenu             = Properties.GetObject(PropContextMenu) as ContextMenu;
            ContextMenuStrip contextMenuStrip   = (contextMenu != null) ? null /*save ourselves a property fetch*/
                                                                        : Properties.GetObject(PropContextMenuStrip) as ContextMenuStrip;

            if (contextMenu != null || contextMenuStrip != null) {
                int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                Point client;
                bool keyboardActivated = false;
                // lparam will be exactly -1 when the user invokes the context menu
                // with the keyboard.
                //
                if (unchecked((int)(long)m.LParam) == -1) {
                    keyboardActivated = true;
                    client = new Point(Width/2, Height/2);
                }
                else {
                    client = PointToClientInternal(new Point(x, y));
                }

                // 

                // VisualStudio7 # 156, only show the context menu when clicked in the client area
                if (ClientRectangle.Contains( client )) {
                    if (contextMenu != null) {
                        contextMenu.Show(sourceControl, client);
                    }
                    else if (contextMenuStrip != null) {
                        contextMenuStrip.ShowInternal(sourceControl, client, keyboardActivated);
                    }
                    else {
                      Debug.Fail("contextmenu and contextmenustrip are both null... hmm how did we get here?");
                      DefWndProc( ref m );
                    }
                }
                else {
                    DefWndProc( ref m );
                }
            }
            else {
                DefWndProc(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_CTLCOLOR message
        /// </devdoc>
        /// <internalonly/>
        private void WmCtlColorControl(ref Message m) {
            // We could simply reflect the message, but it's faster to handle it here if possible.
            Control control = Control.FromHandleInternal(m.LParam);
            if (control != null) {
                m.Result = control.InitializeDCForWmCtlColor(m.WParam, m.Msg);
                if (m.Result != IntPtr.Zero) {
                    return;
                }
            }  

            DefWndProc(ref m);
        }

        private void WmDisplayChange(ref Message m) {
            BufferedGraphicsManager.Current.Invalidate();
            DefWndProc(ref m);
        }

        /// <devdoc>
        ///     WM_DRAWITEM handler
        /// </devdoc>
        /// <internalonly/>
        private void WmDrawItem(ref Message m) {

            // If the wparam is zero, then the message was sent by a menu.
            // See WM_DRAWITEM in MSDN.
            if (m.WParam == IntPtr.Zero) {
                WmDrawItemMenuItem(ref m);
            }
            else {
                WmOwnerDraw(ref m);
            }
        }

        private void WmDrawItemMenuItem(ref Message m) {
            // Obtain the menu item object
            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));

            // A pointer to the correct MenuItem is stored in the draw item
            // information sent with the message.
            // (See MenuItem.CreateMenuItemInfo)
            MenuItem menuItem = MenuItem.GetMenuItemFromItemData(dis.itemData);

            // Delegate this message to the menu item
            if (menuItem != null) {
                menuItem.WmDrawItem(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_ERASEBKGND message
        /// </devdoc>
        /// <internalonly/>
        private void WmEraseBkgnd(ref Message m) {
            if (GetStyle(ControlStyles.UserPaint)) {
                // When possible, it's best to do all painting directly from WM_PAINT.
                // OptimizedDoubleBuffer is the "same" as turning on AllPaintingInWMPaint
                if (!(GetStyle(ControlStyles.AllPaintingInWmPaint))) {
                    IntPtr dc = m.WParam;
                    if (dc == IntPtr.Zero) {    // This happens under extreme stress conditions
                        m.Result = (IntPtr)0;
                        return;
                    }
                    NativeMethods.RECT rc = new NativeMethods.RECT();
                    UnsafeNativeMethods.GetClientRect(new HandleRef(this, Handle), ref rc);
                    using (PaintEventArgs pevent = new PaintEventArgs(dc, Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom))) {
                        PaintWithErrorHandling(pevent, PaintLayerBackground);
                    }
                }
                m.Result = (IntPtr)1;
            }
            else {
                DefWndProc(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_EXITMENULOOP message. If this control has a context menu, its
        ///     Collapse event is raised.
        /// </devdoc>
        /// <internalonly/>
        private void WmExitMenuLoop(ref Message m) {
            bool isContextMenu = (unchecked((int)(long)m.WParam) == 0) ? false : true;

            if (isContextMenu) {
                ContextMenu contextMenu = (ContextMenu)Properties.GetObject(PropContextMenu);
                if (contextMenu != null) {
                    contextMenu.OnCollapse(EventArgs.Empty);
                }
            }

            DefWndProc(ref m);
        }

        /// <devdoc>
        ///     Handles the WM_GETCONTROLNAME message. Returns the name of the control.
        /// </devdoc>
        /// <internalonly/>
        private void WmGetControlName(ref Message m) {
            string name;

            if (this.Site != null) {
                name = this.Site.Name;
            }
            else {
                name = this.Name;
            }

            if (name == null)
                name = "";


            MarshalStringToMessage(name, ref m);
        }

        /// <devdoc>
        ///     Handles the WM_GETCONTROLTYPE message. Returns the name of the control.
        /// </devdoc>
        /// <internalonly/>
        private void WmGetControlType(ref Message m) {
            string type = GetType().AssemblyQualifiedName;
            MarshalStringToMessage(type, ref m);
        }

        /// <devdoc>
        ///     Handles the WM_GETOBJECT message. Used for accessibility.
        /// </devdoc>
        /// <internalonly/>
        private void WmGetObject(ref Message m) {
            Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "In WmGetObject, this = " + this.GetType().FullName + ", lParam = " + m.LParam.ToString());

            InternalAccessibleObject intAccessibleObject = null;

            if (m.Msg == NativeMethods.WM_GETOBJECT && m.LParam == (IntPtr)NativeMethods.UiaRootObjectId && this.SupportsUiaProviders) {
                // If the requested object identifier is UiaRootObjectId, 
                // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                IntSecurity.UnmanagedCode.Assert();
                try {
                    intAccessibleObject = new InternalAccessibleObject(this.AccessibilityObject);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
                m.Result = UnsafeNativeMethods.UiaReturnRawElementProvider(
                    new HandleRef(this, Handle),
                    m.WParam,
                    m.LParam,
                    intAccessibleObject);

                return;
            }

            AccessibleObject ctrlAccessibleObject = GetAccessibilityObject(unchecked((int)(long)m.LParam));

            if (ctrlAccessibleObject != null) {
                // 


                IntSecurity.UnmanagedCode.Assert();
                try {
                    intAccessibleObject = new InternalAccessibleObject(ctrlAccessibleObject);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }

            // See "How to Handle WM_GETOBJECT" in MSDN
            if (intAccessibleObject != null) {

                // Get the IAccessible GUID
                //
                Guid IID_IAccessible = new Guid(NativeMethods.uuid_IAccessible);

                // Get an Lresult for the accessibility Object for this control
                //
                IntPtr punkAcc;
                try {
                    // It is critical that we never pass out a raw IAccessible object here,
                    // but always pass out an IAccessibleInternal wrapper. This may not be possible in the current implementation
                    // of WmGetObject - but its important enough to keep this check here in case the code changes in the future in
                    // a way that breaks this assumption.

                    object tempObject = intAccessibleObject;
                    IAccessible iAccCheck = tempObject as IAccessible;
                    if (iAccCheck != null) {
                        throw new InvalidOperationException(SR.ControlAccessibileObjectInvalid);
                    }

                    // Check that we have an IAccessibleInternal implementation and return this
                    UnsafeNativeMethods.IAccessibleInternal iacc = (UnsafeNativeMethods.IAccessibleInternal) intAccessibleObject;

                    if (iacc == null) {
                        // Accessibility is not supported on this control
                        //
                        Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibilityObject returned null");
                        m.Result = (IntPtr)0;
                    }
                    else {
                        // Obtain the Lresult
                        //
                        punkAcc = Marshal.GetIUnknownForObject(iacc);

                        // 




                        IntSecurity.UnmanagedCode.Assert();
                        try {
                            m.Result = UnsafeNativeMethods.LresultFromObject(ref IID_IAccessible, m.WParam, new HandleRef(ctrlAccessibleObject, punkAcc));
                            Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "LresultFromObject returned " + m.Result.ToString());
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                            Marshal.Release(punkAcc);
                        }
                    }
                }
                catch (Exception e) {
                    throw new InvalidOperationException(SR.RichControlLresult, e);
                }
            }
            else {  // some accessible object requested that we don't care about, so do default message processing
                DefWndProc(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_HELP message
        /// </devdoc>
        /// <internalonly/>
        private void WmHelp(ref Message m) {

            // if there's currently a message box open - grab the help info from it.
            HelpInfo hpi = MessageBox.HelpInfo;
            if (hpi != null) {
                switch (hpi.Option) {
                    case NativeMethods.HLP_FILE  : Help.ShowHelp (this, hpi.HelpFilePath);
                             break;
                    case NativeMethods.HLP_KEYWORD : Help.ShowHelp (this, hpi.HelpFilePath, hpi.Keyword);
                             break;
                    case NativeMethods.HLP_NAVIGATOR : Help.ShowHelp (this, hpi.HelpFilePath, hpi.Navigator);
                             break;
                    case NativeMethods.HLP_OBJECT : Help.ShowHelp (this, hpi.HelpFilePath, hpi.Navigator, hpi.Param);
                             break;
                }
            }

            // Note: info.hItemHandle is the handle of the window that sent the help message.
            NativeMethods.HELPINFO info = (NativeMethods.HELPINFO)m.GetLParam(typeof(NativeMethods.HELPINFO));

            HelpEventArgs hevent = new HelpEventArgs(new Point(info.MousePos.x, info.MousePos.y));
            OnHelpRequested(hevent);
            if (!hevent.Handled) {
                DefWndProc(ref m);
            }

        }

        /// <devdoc>
        ///     Handles the WM_INITMENUPOPUP message
        /// </devdoc>
        /// <internalonly/>
        private void WmInitMenuPopup(ref Message m) {
            ContextMenu contextMenu = (ContextMenu)Properties.GetObject(PropContextMenu);
            if (contextMenu != null) {

                if (contextMenu.ProcessInitMenuPopup(m.WParam))
                    return;
            }
            DefWndProc(ref m);
        }

        /// <devdoc>
        ///     WM_MEASUREITEM handler
        /// </devdoc>
        /// <internalonly/>
        private void WmMeasureItem(ref Message m) {

            // If the wparam is zero, then the message was sent by a menu.
            // See WM_MEASUREITEM in MSDN.
            if (m.WParam == IntPtr.Zero) {

                // Obtain the menu item object
                NativeMethods.MEASUREITEMSTRUCT mis = (NativeMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(NativeMethods.MEASUREITEMSTRUCT));

                Debug.Assert(m.LParam != IntPtr.Zero, "m.lparam is null");

                // A pointer to the correct MenuItem is stored in the measure item
                // information sent with the message.
                // (See MenuItem.CreateMenuItemInfo)
                MenuItem menuItem = MenuItem.GetMenuItemFromItemData(mis.itemData);
                Debug.Assert(menuItem != null, "UniqueID is not associated with a menu item");

                // Delegate this message to the menu item
                if (menuItem != null) {
                    menuItem.WmMeasureItem(ref m);
                }
            }
            else {
                WmOwnerDraw(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_MENUCHAR message
        /// </devdoc>
        /// <internalonly/>
        private void WmMenuChar(ref Message m) {
            Menu menu = ContextMenu;
            if (menu != null) {
                menu.WmMenuChar(ref m);
                if (m.Result != IntPtr.Zero) {
                    // This char is a mnemonic on our menu.
                    return;
                }
            }
        }

        /// <devdoc>
        ///     Handles the WM_MENUSELECT message
        /// </devdoc>
        /// <internalonly/>
        private void WmMenuSelect(ref Message m) {
            int item = NativeMethods.Util.LOWORD(m.WParam);
            int flags = NativeMethods.Util.HIWORD(m.WParam);
            IntPtr hmenu = m.LParam;
            MenuItem mi = null;

            if ((flags & NativeMethods.MF_SYSMENU) != 0) {
                // nothing
            }
            else if ((flags & NativeMethods.MF_POPUP) == 0) {
                Command cmd = Command.GetCommandFromID(item);
                if (cmd != null) {
                    Object reference = cmd.Target;
                    if (reference != null && reference is MenuItem.MenuItemData) {
                        mi = ((MenuItem.MenuItemData)reference).baseItem;
                    }
                }
            }
            else {
                mi = GetMenuItemFromHandleId(hmenu, item);
            }

            if (mi != null) {
                mi.PerformSelect();
            }

            DefWndProc(ref m);
        }

        /// <devdoc>
        ///     Handles the WM_CREATE message
        /// </devdoc>
        /// <internalonly/>
        private void WmCreate(ref Message m) {

            DefWndProc(ref m);

            if (parent != null) {
                parent.UpdateChildZOrder(this);
            }
            UpdateBounds();

            // Let any interested sites know that we've now created a handle
            //
            OnHandleCreated(EventArgs.Empty);

            // this code is important -- it is critical that we stash away
            // the value of the text for controls such as edit, button,
            // label, etc. Without this processing, any time you change a
            // property that forces handle recreation, you lose your text!
            // See the below code in wmDestroy
            //
            if (!GetStyle(ControlStyles.CacheText)) {
                text = null;
            }
        }

        /// <devdoc>
        ///     Handles the WM_DESTROY message
        /// </devdoc>
        /// <internalonly/>
        private void WmDestroy(ref Message m) {
            // Let any interested sites know that we're destroying our handle
            //

            //Debug.Assert(IsHandleCreated, "Need to have the handle here");

            if (!RecreatingHandle && !Disposing && !IsDisposed && GetState(STATE_TRACKINGMOUSEEVENT))
            {
                // Raise the MouseLeave event for the control below the mouse
                // when a modal dialog is discarded.
                OnMouseLeave(EventArgs.Empty);
                UnhookMouseEvent();
            }

            OnHandleDestroyed(EventArgs.Empty);

            if (!Disposing) {
                // If we are not recreating the handle, set our created state
                // back to false so we can be rebuilt if we need to be.
                //
                if (!RecreatingHandle) {
                    SetState(STATE_CREATED, false);
                }
            }
            else {
                SetState(STATE_VISIBLE, false);
            }

            DefWndProc(ref m);
        }

        /// <devdoc>
        ///     Handles the WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and
        ///     WM_SYSKEYUP messages.
        /// </devdoc>
        /// <internalonly/>
        private void WmKeyChar(ref Message m) {
            if (ProcessKeyMessage(ref m)) return;
            DefWndProc(ref m);
        }

        /// <devdoc>
        ///     Handles the WM_KILLFOCUS message
        /// </devdoc>
        /// <internalonly/>
        private void WmKillFocus(ref Message m) {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Control::WmKillFocus - " + this.Name);
            WmImeKillFocus();
            DefWndProc(ref m);
            this.InvokeLostFocus(this, EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the WM_MOUSEDOWN message
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseDown(ref Message m, MouseButtons button, int clicks) {
            // If this is a "real" mouse event (not just WM_LBUTTONDOWN, etc) then
            // we need to see if something happens during processing of
            // user code that changed the state of the buttons (i.e. bringing up
            // a dialog) to keep the control in a consistent state...
            //
            MouseButtons realState = MouseButtons;
            SetState(STATE_MOUSEPRESSED, true);

            // If the UserMouse style is set, the control does its own processing
            // of mouse messages
            //
            if (!GetStyle(ControlStyles.UserMouse)) {
                DefWndProc(ref m);
                // we might had re-entered the message loop and processed a WM_CLOSE message
                if (IsDisposed) {
                     return;
                }  
            }
            else {
                // DefWndProc would normally set the focus to this control, but
                // since we're skipping DefWndProc, we need to do it ourselves.
                if (button == MouseButtons.Left && GetStyle(ControlStyles.Selectable)) {
                    FocusInternal();
                }
            }

            if (realState != MouseButtons) {
                return;
            }

            if (!GetState2(STATE2_MAINTAINSOWNCAPTUREMODE)) {
                //CaptureInternal is set usually in MouseDown (ToolStrip main exception)
                CaptureInternal = true;
            }

            if (realState != MouseButtons) {
                return;
            }

            // control should be enabled when this method is entered, but may have become
            // disabled during its lifetime (e.g. through a Click or Focus listener)
            if (Enabled) {
                OnMouseDown(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
            }
        }

        /// <devdoc>
        ///     Handles the WM_MOUSEENTER message
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseEnter(ref Message m) {
            DefWndProc(ref m);
            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                KeyboardToolTipStateMachine.Instance.NotifyAboutMouseEnter(this);
            }
            OnMouseEnter(EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the WM_MOUSELEAVE message
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseLeave(ref Message m) {
            DefWndProc(ref m);
            OnMouseLeave(EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the WM_DPICHANGED_BEFOREPARENT message. This message is not sent to top level windows.
        /// </devdoc>
        private void WmDpiChangedBeforeParent(ref Message m) {
            DefWndProc(ref m);

            if (IsHandleCreated) {
                int deviceDpiOld = deviceDpi;
                deviceDpi = (int)UnsafeNativeMethods.GetDpiForWindow(new HandleRef(this, HandleInternal));

                // Controls are by default font scaled. 
                // Dpi change requires font to be recalculated inorder to get controls scaled with right dpi.
                if (deviceDpiOld != deviceDpi) {
                    // Checking if font was inherited from parent. Font inherited from parent will receive OnParentFontChanged() events to scale those controls.
                    Font local = (Font)Properties.GetObject(PropFont);
                    if (local != null) {
                        var factor = (float)deviceDpi / deviceDpiOld;
                        this.Font = new Font(local.FontFamily, local.Size * factor, local.Style, local.Unit, local.GdiCharSet, local.GdiVerticalFont);
                    }

                    RescaleConstantsForDpi(deviceDpiOld, deviceDpi);
                }
            }

            OnDpiChangedBeforeParent(EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the WM_DPICHANGED_AFTERPARENT message
        /// </devdoc>
        private void WmDpiChangedAfterParent(ref Message m) {
            DefWndProc(ref m);

            uint dpi = UnsafeNativeMethods.GetDpiForWindow(new HandleRef(this, HandleInternal));

            OnDpiChangedAfterParent(EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the "WM_MOUSEHOVER" message... until we get actuall OS support
        ///     for this, it is implemented as a custom message.
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseHover(ref Message m) {
            DefWndProc(ref m);
            OnMouseHover(EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the WM_MOUSEMOVE message
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseMove(ref Message m) {
            // If the UserMouse style is set, the control does its own processing
            // of mouse messages
            //
            if (!GetStyle(ControlStyles.UserMouse)) {
                DefWndProc(ref m);
            }
            OnMouseMove(new MouseEventArgs(MouseButtons, 0, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
        }

        /// <devdoc>
        ///     Handles the WM_MOUSEUP message
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseUp(ref Message m, MouseButtons button, int clicks) {
            // Get the mouse location
            //
            try {
                int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                Point pt = new Point(x,y);
                pt = PointToScreen(pt);

                // If the UserMouse style is set, the control does its own processing
                // of mouse messages
                //
                if (!GetStyle(ControlStyles.UserMouse)) {
                    DefWndProc(ref m);
                }
                else {
                    // DefWndProc would normally trigger a context menu here
                    // (for a right button click), but since we're skipping DefWndProc
                    // we have to do it ourselves.
                    if (button == MouseButtons.Right) {
                        SendMessage(NativeMethods.WM_CONTEXTMENU, this.Handle, NativeMethods.Util.MAKELPARAM(pt.X, pt.Y));
                    }
                }

                bool fireClick = false;

                if ((controlStyle & ControlStyles.StandardClick) == ControlStyles.StandardClick) {
                    if (GetState(STATE_MOUSEPRESSED) && !IsDisposed && UnsafeNativeMethods.WindowFromPoint(pt.X, pt.Y) == Handle) {
                    fireClick = true;
                    }
                }

                if (fireClick && !ValidationCancelled) {
                    if (!GetState(STATE_DOUBLECLICKFIRED)) {
                        //OnClick(EventArgs.Empty);
                        //In Whidbey .. if the click in by MOUSE then pass the MouseEventArgs...
                        OnClick(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        OnMouseClick(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));

                    }

                    else {
                        //OnDoubleClick(EventArgs.Empty);
                        OnDoubleClick(new MouseEventArgs(button, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        OnMouseDoubleClick(new MouseEventArgs(button, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                    }

                }
                //call the MouseUp Finally...
                OnMouseUp(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
            }
            finally {
                //Always Reset the STATE_DOUBLECLICKFIRED in UP.. Since we get UP - DOWN - DBLCLK - UP sequqnce
                //The Flag is set in L_BUTTONDBLCLK in the controls WndProc() ...
                //
                SetState(STATE_DOUBLECLICKFIRED, false);
                SetState(STATE_MOUSEPRESSED, false);
                SetState(STATE_VALIDATIONCANCELLED, false);
                //CaptureInternal is Resetted while exiting the MouseUp
                CaptureInternal = false;
            }
        }

        /// <devdoc>
        ///     Handles the WM_MOUSEWHEEL message
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseWheel(ref Message m) {
            Point p = new Point(NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam));
            p = PointToClient(p);
            HandledMouseEventArgs e = new HandledMouseEventArgs(MouseButtons.None,
                                                                0,
                                                                p.X,
                                                                p.Y,
                                                                NativeMethods.Util.SignedHIWORD(m.WParam));
            OnMouseWheel(e);
            m.Result = (IntPtr) (e.Handled ? 0 : 1);
            if (!e.Handled)
            {
                // Forwarding the message to the parent window
                DefWndProc(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_MOVE message.  We must do this in
        ///     addition to WM_WINDOWPOSCHANGED because windows may
        ///     send WM_MOVE directly.
        /// </devdoc>
        /// <internalonly/>
        private void WmMove(ref Message m) {
            DefWndProc(ref m);
            UpdateBounds();
        }

        /// <devdoc>
        ///     Handles the WM_NOTIFY message
        /// </devdoc>
        /// <internalonly/>
        private unsafe void WmNotify(ref Message m) {
            NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR*)m.LParam;
            if (!ReflectMessageInternal(nmhdr->hwndFrom,ref m)) {
                if(nmhdr->code == NativeMethods.TTN_SHOW) {
                    m.Result = UnsafeNativeMethods.SendMessage(new HandleRef(null, nmhdr->hwndFrom), NativeMethods.WM_REFLECT + m.Msg, m.WParam, m.LParam);
                    return;
                }
                if(nmhdr->code == NativeMethods.TTN_POP) {
                    UnsafeNativeMethods.SendMessage(new HandleRef(null, nmhdr->hwndFrom), NativeMethods.WM_REFLECT + m.Msg, m.WParam, m.LParam);
                }
                
                DefWndProc(ref m);
            }

        }

        /// <devdoc>
        ///     Handles the WM_NOTIFYFORMAT message
        /// </devdoc>
        /// <internalonly/>
        private void WmNotifyFormat(ref Message m) {
            if (!ReflectMessageInternal(m.WParam, ref m)) {
                DefWndProc(ref m);
            }
        }


        /// <devdoc>
        ///     Handles the WM_DRAWITEM\WM_MEASUREITEM messages for controls other than menus
        /// </devdoc>
        /// <internalonly/>
        private void WmOwnerDraw(ref Message m) {          
            bool reflectCalled = false;

            int ctrlId = unchecked((int)(long)m.WParam);
            IntPtr p = UnsafeNativeMethods.GetDlgItem(new HandleRef(null, m.HWnd), ctrlId);
            if (p == IntPtr.Zero) {
                // On 64-bit platforms wParam is already 64 bit but the control ID stored in it is only 32-bit
                // Empirically, we have observed that the 64 bit HWND is just a sign extension of the 32-bit ctrl ID
                // Since WParam is already 64-bit, we need to discard the high dword first and then re-extend the 32-bit value
                // treating it as signed
                p = (IntPtr)(long)ctrlId;
            }
            if (!ReflectMessageInternal(p, ref m)) {
                //Additional Check For Control .... TabControl truncates the Hwnd value...
                IntPtr handle = window.GetHandleFromID((short)NativeMethods.Util.LOWORD(m.WParam));
                if (handle != IntPtr.Zero) {
                    Control control = Control.FromHandleInternal(handle);
                    if (control != null) {
                        m.Result = control.SendMessage(NativeMethods.WM_REFLECT + m.Msg, handle, m.LParam);
                        reflectCalled = true;
                    }
                }
            }
            else  {
                reflectCalled = true;
            }


            if (!reflectCalled) {
              DefWndProc(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_PAINT messages.  This should only be called
        ///     for userpaint controls.
        /// </devdoc>
        /// <internalonly/>
        private void WmPaint(ref Message m) {
            bool doubleBuffered = DoubleBuffered || (GetStyle(ControlStyles.AllPaintingInWmPaint) && DoubleBufferingEnabled);
#if DEBUG
            if (BufferDisabled.Enabled) {
                doubleBuffered = false;
            }
#endif
            IntPtr hWnd = IntPtr.Zero;
            IntPtr dc;
            Rectangle clip;
            NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
            bool needDisposeDC = false;

            try {
                if (m.WParam == IntPtr.Zero) {
                    // Cache Handle not only for perf but to avoid object disposed exception in case the window
                    // is destroyed in an event handler.
                    hWnd = this.Handle;
                    dc = UnsafeNativeMethods.BeginPaint(new HandleRef(this, hWnd), ref ps);
                    if (dc == IntPtr.Zero) {
                        return;
                    }
                    needDisposeDC = true;
                    clip = new Rectangle(ps.rcPaint_left, ps.rcPaint_top,
                                         ps.rcPaint_right - ps.rcPaint_left,
                                         ps.rcPaint_bottom - ps.rcPaint_top);
                }
                else {
                    dc = m.WParam;
                    clip = ClientRectangle;
                }

                // Consider: Why don't check the clip condition when non-doubleBuffered?
                //           we should probably get rid of the !doubleBuffered condition.
                if (!doubleBuffered || (clip.Width > 0 && clip.Height > 0)) {
                    IntPtr oldPal = IntPtr.Zero;
                    BufferedGraphics bufferedGraphics = null;
                    PaintEventArgs pevent = null;
                    GraphicsState state = null;

                    try {
                        if (doubleBuffered || m.WParam == IntPtr.Zero) {
                            oldPal = SetUpPalette(dc, false, false);
                        }

                        if (doubleBuffered) {
                            try {
                                bufferedGraphics = BufferContext.Allocate(dc, ClientRectangle);
#if DEBUG
                                if (BufferPinkRect.Enabled) {
                                        Rectangle band = ClientRectangle;
                                        using( BufferedGraphics bufferedGraphics2 = BufferContext.Allocate( dc, band ) ) {
                                            bufferedGraphics2.Graphics.FillRectangle( new SolidBrush( Color.Red ), band );
                                            bufferedGraphics2.Render();
                                    }
                                    Thread.Sleep(50);
                                }
#endif
                            }
                            catch (Exception ex) {
                                // BufferContext.Allocate will throw out of memory exceptions
                                // when it fails to create a device dependent bitmap while trying to 
                                // get information about the device we are painting on.
                                // That is not the same as a system running out of memory and there is a 
                                // very good chance that we can continue to paint successfully. We cannot
                                // check whether double buffering is supported in this case, and we will disable it.
                                // We could set a specific string when throwing the exception and check for it here
                                // to distinguish between that case and real out of memory exceptions but we
                                // see no reasons justifying the additional complexity.
                                if (ClientUtils.IsCriticalException(ex) && !(ex is OutOfMemoryException)) {
                                    throw;
                                }
#if DEBUG
                                if( BufferPinkRect.Enabled ) {
                                    Debug.WriteLine("Could not create buffered graphics, will paint in the surface directly" );
                                }
#endif
                                doubleBuffered = false; // paint directly on the window DC.
                            }
                        }

                        if (bufferedGraphics != null) {
                            bufferedGraphics.Graphics.SetClip(clip);
                            pevent = new PaintEventArgs(bufferedGraphics.Graphics, clip);
                            state = pevent.Graphics.Save();
                        }
                        else {
                            pevent = new PaintEventArgs(dc, clip);
                        }

                        using (pevent) {
                            try {
                                if ((m.WParam == IntPtr.Zero) && GetStyle(ControlStyles.AllPaintingInWmPaint) || doubleBuffered) {
                                    PaintWithErrorHandling(pevent, PaintLayerBackground);
                                    // Consider: This condition could be elimiated, 
                                    //           do we have to save/restore the state of the buffered graphics? 

                                }
                            }
                            finally {
                                if (state != null) {
                                    pevent.Graphics.Restore(state);
                                }
                                else {
                                    pevent.ResetGraphics();
                                }
                            }
                            PaintWithErrorHandling(pevent, PaintLayerForeground);

                            if (bufferedGraphics != null) {
                                bufferedGraphics.Render();
                            }
                        }
                    }
                    finally {
                        if (oldPal != IntPtr.Zero) {
                            SafeNativeMethods.SelectPalette(new HandleRef(null, dc), new HandleRef(null, oldPal), 0);
                        }
                        if (bufferedGraphics != null) {
                            bufferedGraphics.Dispose();
                        }

                    }
                }
            }
            finally {
                if (needDisposeDC) {
                    UnsafeNativeMethods.EndPaint(new HandleRef(this, hWnd), ref ps);
                }
            }
        }

        /// <devdoc>
        ///     Handles the WM_PRINTCLIENT messages.  
        /// </devdoc>
        /// <internalonly/>
        private void WmPrintClient(ref Message m) {
            using (PaintEventArgs e = new PrintPaintEventArgs(m, m.WParam, ClientRectangle)) {
                OnPrint(e);
            }
        }

        private void WmQueryNewPalette(ref Message m) {
            Debug.WriteLineIf(Control.PaletteTracing.TraceVerbose, Handle + ": WM_QUERYNEWPALETTE");
            IntPtr dc = UnsafeNativeMethods.GetDC(new HandleRef(this, Handle));
            try {
                SetUpPalette(dc, true /*force*/, true/*realize*/);
            }
            finally {
                // Let WmPaletteChanged do any necessary invalidation
                UnsafeNativeMethods.ReleaseDC(new HandleRef(this, Handle), new HandleRef(null, dc));
            }
            Invalidate(true);
            m.Result = (IntPtr)1;
            DefWndProc(ref m);
        }


        /// <devdoc>
        ///     Handles the WM_SETCURSOR message
        /// </devdoc>
        /// <internalonly/>
        private void WmSetCursor(ref Message m) {

            // Accessing through the Handle property has side effects that break this
            // logic. You must use InternalHandle.
            //
            if (m.WParam == InternalHandle && NativeMethods.Util.LOWORD(m.LParam) == NativeMethods.HTCLIENT) {
                Cursor.CurrentInternal = Cursor;
            }
            else {
                DefWndProc(ref m);
            }

        }

        /// <devdoc>
        ///     Handles the WM_WINDOWPOSCHANGING message
        /// </devdoc>
        /// <internalonly/>
        private unsafe void WmWindowPosChanging(ref Message m) {

            // We let this fall through to defwndproc unless we are being surfaced as
            // an ActiveX control.  In that case, we must let the ActiveX side of things
            // manipulate our bounds here.
            //
            if (IsActiveX) {
                NativeMethods.WINDOWPOS* wp = (NativeMethods.WINDOWPOS *)m.LParam;
                // Only call UpdateBounds if the new bounds are different.
                //
                bool different = false;

                if ((wp->flags & NativeMethods.SWP_NOMOVE) == 0 && (wp->x != Left || wp->y != Top)) {
                    different = true;
                }
                if ((wp->flags & NativeMethods.SWP_NOSIZE) == 0 && (wp->cx != Width || wp->cy != Height)) {
                    different = true;
                }

                if (different) {
                    ActiveXUpdateBounds(ref wp->x, ref wp->y, ref wp->cx, ref wp->cy, wp->flags);
                }
            }

            DefWndProc(ref m);
        }


        /// <devdoc>
        ///     Handles the WM_PARENTNOTIFY message
        /// </devdoc>
        /// <internalonly/>
        private void WmParentNotify(ref Message m) {
            int msg = NativeMethods.Util.LOWORD(m.WParam);
            IntPtr hWnd = IntPtr.Zero;
            switch (msg) {
                case NativeMethods.WM_CREATE:
                    hWnd = m.LParam;
                    break;
                case NativeMethods.WM_DESTROY:
                    break;
                default:
                    hWnd = UnsafeNativeMethods.GetDlgItem(new HandleRef(this, Handle), NativeMethods.Util.HIWORD(m.WParam));
                    break;
            }
            if (hWnd == IntPtr.Zero || !ReflectMessageInternal(hWnd, ref m)) {
                DefWndProc(ref m);
            }
        }

        /// <devdoc>
        ///     Handles the WM_SETFOCUS message
        /// </devdoc>
        /// <internalonly/>
        private void WmSetFocus(ref Message m) {
            Debug.WriteLineIf(Control.FocusTracing.TraceVerbose, "Control::WmSetFocus - " + this.Name);
            WmImeSetFocus();

            if (!HostedInWin32DialogManager) {
                IContainerControl c = GetContainerControlInternal();
                if (c != null) {
                    bool activateSucceed;

                    ContainerControl knowncontainer = c as ContainerControl;
                    if (knowncontainer != null) {
                        activateSucceed = knowncontainer.ActivateControlInternal(this);
                    }
                    else {
                        // Reviewed : Taking focus and activating a control in response
                        //          : to a user gesture (WM_SETFOCUS) is OK.
                        //
                        IntSecurity.ModifyFocus.Assert();
                        try {
                            activateSucceed = c.ActivateControl(this);
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                    }

                    if (!activateSucceed) {
                        return;
                    }
                }
            }

            DefWndProc(ref m);
            this.InvokeGotFocus(this, EventArgs.Empty);
        }

        /// <devdoc>
        ///     Handles the WM_SHOWWINDOW message
        /// </devdoc>
        /// <internalonly/>
        private void WmShowWindow(ref Message m) {
            // We get this message for each control, even if their parent is not visible.

            DefWndProc(ref m);

            if ((state & STATE_RECREATE) == 0) {

                bool visible = m.WParam != IntPtr.Zero;
                bool oldVisibleProperty = Visible;

                if (visible) {
                    bool oldVisibleBit = GetState(STATE_VISIBLE);
                    SetState(STATE_VISIBLE, true);
                    bool executedOk = false;
                    try {
                        CreateControl();
                        executedOk = true;
                    }

                    finally {
                        if (!executedOk) {
                            // We do it this way instead of a try/catch because catching and rethrowing
                            // an exception loses call stack information
                            SetState(STATE_VISIBLE, oldVisibleBit);
                        }
                    }
                }
                else { // not visible
                    // If Windows tells us it's visible, that's pretty unambiguous.
                    // But if it tells us it's not visible, there's more than one explanation --
                    // maybe the container control became invisible.  So we look at the parent
                    // and take a guess at the reason.

                    // We do not want to update state if we are on the parking window.
                    bool parentVisible = GetTopLevel();
                    if (ParentInternal != null) {
                        parentVisible = ParentInternal.Visible;
                    }

                    if (parentVisible) {
                        SetState(STATE_VISIBLE, false);
                    }
                }

                if (!GetState(STATE_PARENTRECREATING) && (oldVisibleProperty != visible)) {
                    OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        /// <devdoc>
        ///     Handles the WM_UPDATEUISTATE message
        /// </devdoc>
        /// <internalonly/>
        private void WmUpdateUIState(ref Message m) {

            // See "How this all works" in ShowKeyboardCues

            bool keyboard = false;
            bool focus = false;

            // check the cached values in uiCuesState to see if we've ever set in the UI state
            bool keyboardInitialized = (uiCuesState & UISTATE_KEYBOARD_CUES_MASK) != 0;
            bool focusInitialized = (uiCuesState & UISTATE_FOCUS_CUES_MASK) != 0;

            if (keyboardInitialized) {
                keyboard = ShowKeyboardCues;
            }

            if (focusInitialized) {
                focus = ShowFocusCues;
            }            

            DefWndProc(ref m);

            int cmd = NativeMethods.Util.LOWORD(m.WParam);

            // if we're initializing, dont bother updating the uiCuesState/Firing the event.
            
            if (cmd == NativeMethods.UIS_INITIALIZE) {
                return;
            }

            // Set in the cached value for uiCuesState...
            
            // Windows stores the opposite of what you would think, it has bit
            // flags for the "Hidden" state, the presence of this flag means its
            // hidden, the absence thereof means it's shown.
            //
            // When we're called here with a UIS_CLEAR and the hidden state is set
            // that means we want to show the accelerator.
            UICues UIcues = UICues.None;
            if ((NativeMethods.Util.HIWORD(m.WParam) & NativeMethods.UISF_HIDEACCEL) != 0) {

                // yes, clear means show.  nice api, guys.
                //
                bool showKeyboard = (cmd == NativeMethods.UIS_CLEAR);

                if (showKeyboard != keyboard || !keyboardInitialized ) {

                    UIcues |= UICues.ChangeKeyboard;                    

                    // clear the old state.
                    //
                    uiCuesState &= ~UISTATE_KEYBOARD_CUES_MASK;                    
                    uiCuesState |= (showKeyboard ? UISTATE_KEYBOARD_CUES_SHOW : UISTATE_KEYBOARD_CUES_HIDDEN) ;
                }

                if (showKeyboard) {
                    UIcues |= UICues.ShowKeyboard;                    
                }      
            }

            // Same deal for the Focus cues as the keyboard cues.
            if ((NativeMethods.Util.HIWORD(m.WParam) & NativeMethods.UISF_HIDEFOCUS) != 0) {

                // yes, clear means show.  nice api, guys.
                //
                bool showFocus = (cmd == NativeMethods.UIS_CLEAR);

                if (showFocus != focus || !focusInitialized) {
                    UIcues |= UICues.ChangeFocus;                    

                    // clear the old state.
                    //
                    uiCuesState &= ~UISTATE_FOCUS_CUES_MASK;                    
                    uiCuesState |= (showFocus ? UISTATE_FOCUS_CUES_SHOW : UISTATE_FOCUS_CUES_HIDDEN) ;
                }

                if (showFocus) {
                    UIcues |= UICues.ShowFocus;                    
                }
            }


            // fire the UI cues state changed event.
            if ((UIcues & UICues.Changed) != 0) {
                OnChangeUICues(new UICuesEventArgs(UIcues));
                Invalidate(true);
            }
        }

        /// <devdoc>
        ///     Handles the WM_WINDOWPOSCHANGED message
        /// </devdoc>
        /// <internalonly/>
        private unsafe void WmWindowPosChanged(ref Message m) {
            DefWndProc(ref m);
            // Update new size / position
            UpdateBounds();
            if (parent != null && UnsafeNativeMethods.GetParent(new HandleRef(window, InternalHandle)) == parent.InternalHandle &&
                (state & STATE_NOZORDER) == 0) {

                NativeMethods.WINDOWPOS* wp = (NativeMethods.WINDOWPOS *)m.LParam;
                if ((wp->flags & NativeMethods.SWP_NOZORDER) == 0) {
                    parent.UpdateChildControlIndex(this);
                }
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.WndProc"]/*' />
        /// <devdoc>
        ///     Base wndProc. All messages are sent to wndProc after getting filtered
        ///     through the preProcessMessage function. Inheriting controls should
        ///     call base.wndProc for any messages that they don't handle.
        /// </devdoc>
        [
            SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
            SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected virtual void WndProc(ref Message m) {
            //
            // 




            /*
            if( GetState(STATE_DISPOSED))
            {
                Debug.Fail("Attempting to process a windows message in a disposed control.  This may be OK if the app domain is being unloaded.");
                DefWndProc(ref m);
                return;
            }
            */

            // inlined code from GetStyle(...) to ensure no perf hit
            // for a method call...
            //
            if ((controlStyle & ControlStyles.EnableNotifyMessage) == ControlStyles.EnableNotifyMessage) {
                // pass message *by value* to avoid the possibility
                // of the OnNotifyMessage modifying the message.
                //
                OnNotifyMessage(m);
            }

            /*
            * If you add any new messages below (or change the message handling code for any messages)
            * please make sure that you also modify AxHost.wndProc to do the right thing and intercept
            * messages which the Ocx would own before passing them onto Control.wndProc.
            */
            switch (m.Msg) {
                case NativeMethods.WM_CAPTURECHANGED:
                     WmCaptureChanged(ref m);
                     break;

                case NativeMethods.WM_GETOBJECT:
                    WmGetObject(ref m);
                    break;

                case NativeMethods.WM_COMMAND:
                    WmCommand(ref m);
                    break;
                case NativeMethods.WM_CLOSE:
                    WmClose(ref m);
                    break;
                case NativeMethods.WM_CONTEXTMENU:
                    WmContextMenu(ref m);
                    break;
                case NativeMethods.WM_DISPLAYCHANGE:
                    WmDisplayChange(ref m);
                    break;
                case NativeMethods.WM_DRAWITEM:
                    WmDrawItem(ref m);
                    break;
                case NativeMethods.WM_ERASEBKGND:
                    WmEraseBkgnd(ref m);
                    break;

                case NativeMethods.WM_EXITMENULOOP:
                    WmExitMenuLoop(ref m);
                    break;

                case NativeMethods.WM_HELP:
                    WmHelp(ref m);
                    break;

                case NativeMethods.WM_PAINT:
                    if (GetStyle(ControlStyles.UserPaint)) {
                        WmPaint(ref m);
                    }
                    else {
                        DefWndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_PRINTCLIENT:
                    if (GetStyle(ControlStyles.UserPaint)) {
                        WmPrintClient(ref m);
                    }
                    else {
                        DefWndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_INITMENUPOPUP:
                    WmInitMenuPopup(ref m);
                    break;

                case NativeMethods.WM_SYSCOMMAND:
                    if ((unchecked((int)(long)m.WParam) & 0xFFF0) == NativeMethods.SC_KEYMENU) {
                        Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.WndProc processing " + m.ToString());

                        if (ToolStripManager.ProcessMenuKey(ref m)) {                            
                            Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.WndProc ToolStripManager.ProcessMenuKey returned true" + m.ToString());
                            m.Result = IntPtr.Zero;
                            return;
                        }
                    }
                    DefWndProc(ref m);
                break;
                case NativeMethods.WM_INPUTLANGCHANGE:
                    WmInputLangChange(ref m);
                    break;

                case NativeMethods.WM_INPUTLANGCHANGEREQUEST:
                    WmInputLangChangeRequest(ref m);
                    break;

                case NativeMethods.WM_MEASUREITEM:
                    WmMeasureItem(ref m);
                    break;
                case NativeMethods.WM_MENUCHAR:
                    WmMenuChar(ref m);
                    break;

                case NativeMethods.WM_MENUSELECT:
                    WmMenuSelect(ref m);
                    break;

                case NativeMethods.WM_SETCURSOR:
                    WmSetCursor(ref m);
                    break;

                case NativeMethods.WM_WINDOWPOSCHANGING:
                    WmWindowPosChanging(ref m);
                    break;

                case NativeMethods.WM_CHAR:
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                case NativeMethods.WM_KEYUP:
                case NativeMethods.WM_SYSKEYUP:
                    WmKeyChar(ref m);
                    break;
                case NativeMethods.WM_CREATE:
                    WmCreate(ref m);
                    break;
                case NativeMethods.WM_DESTROY:
                    WmDestroy(ref m);
                    break;


                case NativeMethods.WM_CTLCOLOR:
                case NativeMethods.WM_CTLCOLORBTN:
                case NativeMethods.WM_CTLCOLORDLG:
                case NativeMethods.WM_CTLCOLORMSGBOX:
                case NativeMethods.WM_CTLCOLORSCROLLBAR:
                case NativeMethods.WM_CTLCOLOREDIT:
                case NativeMethods.WM_CTLCOLORLISTBOX:
                case NativeMethods.WM_CTLCOLORSTATIC:

                // this is for the trinity guys.  The case is if you've got a windows
                // forms edit or something hosted as an AX control somewhere, there isn't anyone to reflect
                // these back.  If they went ahead and just sent them back, some controls don't like that 
                // and end up recursing.  Our code handles it fine because we just pick the HWND out of the LPARAM.
                //
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLOR:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORBTN:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORDLG:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORMSGBOX:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORSCROLLBAR:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLOREDIT:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORLISTBOX:
                case NativeMethods.WM_REFLECT + NativeMethods.WM_CTLCOLORSTATIC:                    
                    WmCtlColorControl(ref m);
                    break;
                case NativeMethods.WM_HSCROLL:
                case NativeMethods.WM_VSCROLL:
                case NativeMethods.WM_DELETEITEM:
                case NativeMethods.WM_VKEYTOITEM:
                case NativeMethods.WM_CHARTOITEM:
                case NativeMethods.WM_COMPAREITEM:
                    if (!ReflectMessageInternal(m.LParam, ref m)) {
                        DefWndProc(ref m);
                    }
                    break;
                case NativeMethods.WM_IME_CHAR:
                    WmImeChar(ref m);
                    break;
                case NativeMethods.WM_IME_STARTCOMPOSITION:
                    WmImeStartComposition(ref m);
                    break;
                case NativeMethods.WM_IME_ENDCOMPOSITION:
                    WmImeEndComposition(ref m);
                    break;
                case NativeMethods.WM_IME_NOTIFY:
                    WmImeNotify(ref m);
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    WmKillFocus(ref m);
                    break;
                case NativeMethods.WM_LBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Left, 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick)) {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;
                case NativeMethods.WM_LBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Left, 1);
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    WmMouseUp(ref m, MouseButtons.Left, 1);
                    break;
                case NativeMethods.WM_MBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Middle, 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick)) {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Middle, 1);
                    break;
                case NativeMethods.WM_MBUTTONUP:
                    WmMouseUp(ref m, MouseButtons.Middle, 1);
                    break;
                case NativeMethods.WM_XBUTTONDOWN:
                    WmMouseDown(ref m, GetXButton(NativeMethods.Util.HIWORD(m.WParam)), 1);
                    break;
                case NativeMethods.WM_XBUTTONUP:
                    WmMouseUp(ref m, GetXButton(NativeMethods.Util.HIWORD(m.WParam)), 1);
                    break;
                case NativeMethods.WM_XBUTTONDBLCLK:
                    WmMouseDown(ref m, GetXButton(NativeMethods.Util.HIWORD(m.WParam)), 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick)) {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;
                case NativeMethods.WM_MOUSELEAVE:
                    WmMouseLeave(ref m);
                    break;
                case NativeMethods.WM_DPICHANGED_BEFOREPARENT:
                    WmDpiChangedBeforeParent(ref m);
                    m.Result = IntPtr.Zero;
                    break;
                case NativeMethods.WM_DPICHANGED_AFTERPARENT:
                    WmDpiChangedAfterParent(ref m);
                    m.Result = IntPtr.Zero;
                    break;
                case NativeMethods.WM_MOUSEMOVE:
                    WmMouseMove(ref m);
                    break;
                case NativeMethods.WM_MOUSEWHEEL:
                    WmMouseWheel(ref m);
                    break;
                case NativeMethods.WM_MOVE:
                    WmMove(ref m);
                    break;
                case NativeMethods.WM_NOTIFY:
                    WmNotify(ref m);
                    break;
                case NativeMethods.WM_NOTIFYFORMAT:
                    WmNotifyFormat(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFYFORMAT:
                    m.Result = (IntPtr)(NativeMethods.NFR_UNICODE);
                    break;
                case NativeMethods.WM_SHOWWINDOW:
                    WmShowWindow(ref m);
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Right, 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick)) {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Right, 1);
                    break;
                case NativeMethods.WM_RBUTTONUP:
                    WmMouseUp(ref m, MouseButtons.Right, 1);
                    break;
                case NativeMethods.WM_SETFOCUS:
                    WmSetFocus(ref m);
                    break;
                case NativeMethods.WM_MOUSEHOVER:
                    WmMouseHover(ref m);
                    break;
                case NativeMethods.WM_WINDOWPOSCHANGED:
                    WmWindowPosChanged(ref m);
                    break;
                case NativeMethods.WM_QUERYNEWPALETTE:
                    WmQueryNewPalette(ref m);
                    break;
                case NativeMethods.WM_UPDATEUISTATE:
                    WmUpdateUIState(ref m);
                    break;
                case NativeMethods.WM_PARENTNOTIFY:
                    WmParentNotify(ref m);
                    break;
                default:
                    // If we received a thread execute message, then execute it.
                    //
                    if (m.Msg == threadCallbackMessage && m.Msg != 0) {
                        InvokeMarshaledCallbacks();
                        return;
                    }
                    else if (m.Msg == Control.WM_GETCONTROLNAME) {
                        WmGetControlName(ref m);
                        return;
                    }
                    else if (m.Msg == Control.WM_GETCONTROLTYPE) {
                        WmGetControlType(ref m);
                        return;
                    }

                    #if WIN95_SUPPORT
                    // If we have to route the mousewheel messages, do it (this logic was taken
                    // from the MFC sources...)
                    //
                    if (mouseWheelRoutingNeeded) {
                        if (m.Msg == mouseWheelMessage) {
                            Keys keyState = Keys.None;
                            keyState |= (Keys)((UnsafeNativeMethods.GetKeyState((int)Keys.ControlKey) < 0) ? NativeMethods.MK_CONTROL : 0);
                            keyState |= (Keys)((UnsafeNativeMethods.GetKeyState((int)Keys.ShiftKey) < 0) ? NativeMethods.MK_SHIFT : 0);

                            IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();

                            if (hwndFocus == IntPtr.Zero) {
                                SendMessage(m.Msg, (IntPtr)((unchecked((int)(long)m.WParam) << 16) | (int)keyState), m.LParam);
                            }
                            else {
                                IntPtr result = IntPtr.Zero;
                                IntPtr hwndDesktop = UnsafeNativeMethods.GetDesktopWindow();

                                while (result == IntPtr.Zero && hwndFocus != IntPtr.Zero && hwndFocus != hwndDesktop) {
                                    result = UnsafeNativeMethods.SendMessage(new HandleRef(null, hwndFocus),
                                                                       NativeMethods.WM_MOUSEWHEEL,
                                                                       (unchecked((int)(long)m.WParam) << 16) | (int)keyState,
                                                                       m.LParam);
                                    hwndFocus = UnsafeNativeMethods.GetParent(new HandleRef(null, hwndFocus));
                                }
                            }
                        }
                    }
                    #endif

                    if (m.Msg == NativeMethods.WM_MOUSEENTER) {
                        WmMouseEnter(ref m);
                        break;
                    }

                    DefWndProc(ref m);
                    break;
            }

        }

        /// <devdoc>
        ///      Called when an exception occurs in dispatching messages through
        ///      the main window procedure.
        /// </devdoc>
        private void WndProcException(Exception e) {
            Application.OnThreadException(e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IArrangedElement.Children"]/*' />
        /// <internalonly/>
        ArrangedElementCollection IArrangedElement.Children {
            get {
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(PropControlsCollection);
                if (controlsCollection == null) {
                    return ArrangedElementCollection.Empty;
                }
                return controlsCollection;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IArrangedElement.Container"]/*' />
        /// <internalonly/>
        IArrangedElement IArrangedElement.Container {
            get {
                // This is safe because the IArrangedElement interface is internal
                return ParentInternal;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IArrangedElement.ParticipatesInLayout"]/*' />
        /// <internalonly/>
        bool IArrangedElement.ParticipatesInLayout {
            get { return GetState(STATE_VISIBLE); }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IArrangedElement.PerformLayout"]/*' />
        /// <internalonly/>
        void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string affectedProperty) {
            PerformLayout(new LayoutEventArgs(affectedElement, affectedProperty));
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IArrangedElement.Properties"]/*' />
        /// <internalonly/>
        PropertyStore IArrangedElement.Properties {
            get { return Properties; }
        }

        // CAREFUL: This really calls SetBoundsCore, not SetBounds.
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IArrangedElement.SetBounds"]/*' />
        /// <internalonly/>
        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified) {
            ISite site = Site;
            IComponentChangeService ccs = null;
            PropertyDescriptor sizeProperty = null;
            PropertyDescriptor locationProperty = null;
            bool sizeChanged = false;
            bool locationChanged = false;

            if (site != null && site.DesignMode) {
                ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                if(ccs != null) {
                    sizeProperty = TypeDescriptor.GetProperties(this)[PropertyNames.Size];
                    locationProperty = TypeDescriptor.GetProperties(this)[PropertyNames.Location];
                    Debug.Assert(sizeProperty != null && locationProperty != null, "Error retrieving Size/Location properties on Control.");

                    try {
                        if (sizeProperty != null && !sizeProperty.IsReadOnly && (bounds.Width != this.Width || bounds.Height != this.Height)) {
                            if (!(site is INestedSite))
                            {
                                ccs.OnComponentChanging(this, sizeProperty);
                            }
                            sizeChanged = true;
                        }
                        if (locationProperty != null && !locationProperty.IsReadOnly && (bounds.X != this.x || bounds.Y != this.y)) {
                            if (!(site is INestedSite))
                            {
                                ccs.OnComponentChanging(this, locationProperty);
                            }
                            locationChanged = true;
                        }
                    }
                    catch (InvalidOperationException) {
                        // The component change events can throw InvalidOperationException if a change is
                        // currently not allowed (typically because the doc data in VS is locked). 
                        // When this happens, we just eat the exception and proceed with the change.
                    }
                }
            }

            SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);

            if (site != null && ccs != null) {
                try {
                    if (sizeChanged)  ccs.OnComponentChanged(this, sizeProperty, null, null);
                    if (locationChanged) ccs.OnComponentChanged(this, locationProperty, null, null);
                }
                catch (InvalidOperationException) {
                        // The component change events can throw InvalidOperationException if a change is
                        // currently not allowed (typically because the doc data in VS is locked). 
                        // When this happens, we just eat the exception and proceed with the change.
                }
            }
        }

        /// <summary>
        /// Indicates whether or not the control supports UIA Providers via
        /// IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces
        /// </summary>
        internal virtual bool SupportsUiaProviders {
            get {
                return false;
            }
        }
        
        /// <devdoc>
        /// </devdoc>
        internal sealed class ControlNativeWindow : NativeWindow, IWindowTarget {
            private Control         control;
            private GCHandle        rootRef;   // We will root the control when we do not want to be elligible for garbage collection.
            internal IWindowTarget  target;

            internal ControlNativeWindow(Control control) {
                this.control = control;
                target = this;
            }

            internal Control GetControl() {
                return control;
            }            
           
            protected override void OnHandleChange() {
                target.OnHandleChange(this.Handle);
            }

            // IWindowTarget method
            public void OnHandleChange(IntPtr newHandle) {
                control.SetHandle(newHandle);
            }

            internal void LockReference(bool locked) {
                if (locked) {
                    if (!rootRef.IsAllocated) {
                        rootRef = GCHandle.Alloc(GetControl(), GCHandleType.Normal);
                    }
                }
                else {
                    if (rootRef.IsAllocated) {
                        rootRef.Free();
                    }
                }
            }

            protected override void OnThreadException(Exception e) {
                control.WndProcException(e);
            }

            // IWindowTarget method
            public void OnMessage(ref Message m) {
                control.WndProc(ref m);
            }

            internal IWindowTarget WindowTarget {
                get {
                    return target;
                }
                set {
                    target = value;
                }
            }

            #if DEBUG
            // We override ToString so in debug asserts that fire for
            // non-released controls will show what control wasn't released.
            //
            public override string ToString() {
                if (control != null) {
                    return control.GetType().FullName;
                }
                return base.ToString();
            }
            #endif

            protected override void WndProc(ref Message m) {
                // There are certain messages that we want to process
                // regardless of what window target we are using.  These
                // messages cause other messages or state transitions
                // to occur within control.
                //
                switch (m.Msg) {
                    case NativeMethods.WM_MOUSELEAVE:
                        control.UnhookMouseEvent();
                        break;

                    case NativeMethods.WM_MOUSEMOVE:
                        if (!control.GetState(Control.STATE_TRACKINGMOUSEEVENT)) {
                            control.HookMouseEvent();
                            if (!control.GetState(Control.STATE_MOUSEENTERPENDING)) {
                                control.SendMessage(NativeMethods.WM_MOUSEENTER, 0, 0);
                            }
                            else {
                                control.SetState(Control.STATE_MOUSEENTERPENDING, false);
                            }
                        }
                        break;

                    case NativeMethods.WM_MOUSEWHEEL:
                        // TrackMouseEvent's mousehover implementation doesn't watch the wheel
                        // correctly...
                        //
                        control.ResetMouseEventArgs();
                        break;                   
                }

                target.OnMessage(ref m);
            }
        }

        ///
        /// Explicit support of DropTarget
        ///
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IDropTarget.OnDragEnter"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragEnter(DragEventArgs drgEvent) {
                OnDragEnter(drgEvent);
        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IDropTarget.OnDragOver"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragOver(DragEventArgs drgEvent) {
                OnDragOver(drgEvent);
        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IDropTarget.OnDragLeave"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragLeave(EventArgs e) {
                OnDragLeave(e);
        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.IDropTarget.OnDragDrop"]/*' />
        /// <internalonly/>
        void IDropTarget.OnDragDrop(DragEventArgs drgEvent) {
                OnDragDrop(drgEvent);
        }

        ///
        /// Explicit support of DropSource
        ///
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ISupportOleDropSource.OnGiveFeedback"]/*' />
        /// <internalonly/>
        void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs)  {
                OnGiveFeedback(giveFeedbackEventArgs);
        }
        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ISupportOleDropSource.OnQueryContinueDrag"]/*' />
        /// <internalonly/>
        void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs) {
                OnQueryContinueDrag(queryContinueDragEventArgs);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection"]/*' />
        /// <devdoc>
        ///     Collection of controls...
        /// </devdoc>
        [
            ListBindable(false), ComVisible(false)
        ]
        public class ControlCollection : ArrangedElementCollection, IList, ICloneable {

            private Control owner;

            /// A caching mechanism for key accessor
            /// We use an index here rather than control so that we don't have lifetime
            /// issues by holding on to extra references.
            /// Note this is not Thread Safe - but WinForms has to be run in a STA anyways.
            private int lastAccessedIndex = -1;


            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.ControlCollection"]/*' />
            public ControlCollection(Control owner) {
                this.owner = owner;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.ContainsKey"]/*' />
            /// <devdoc>
            ///     <para>Returns true if the collection contains an item with the specified key, false otherwise.</para>
            /// </devdoc>
            public virtual bool ContainsKey(string key) {
               return IsValidIndex(IndexOfKey(key));
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.Add"]/*' />
            /// <devdoc>
            ///    <para>Adds a child control to this control. The control becomes the last control in
            ///       the child control list. If the control is already a child of another control it
            ///       is first removed from that control.</para>
            /// </devdoc>
            public virtual void Add(Control value) {
                if (value == null)
                    return;
                if (value.GetTopLevel()) {
                    throw new ArgumentException(SR.TopLevelControlAdd);
                }

                // Verify that the control being added is on the same thread as
                // us...or our parent chain.
                //
                if (owner.CreateThreadId != value.CreateThreadId) {
                    throw new ArgumentException(SR.AddDifferentThreads);
                }

                CheckParentingCycle(owner, value);

                if (value.parent == owner) {
                    value.SendToBack();
                    return;
                }

                // Remove the new control from its old parent (if any)
                //
                if (value.parent != null) {
                    value.parent.Controls.Remove(value);
                }

                // Add the control
                //
                InnerList.Add(value);

                if (value.tabIndex == -1) {

                    // Find the next highest tab index
                    //
                    int nextTabIndex = 0;
                    for (int c = 0; c < (Count - 1); c++) {
                        int t = this[c].TabIndex;
                        if (nextTabIndex <= t) {
                            nextTabIndex = t + 1;
                        }
                    }
                    value.tabIndex = nextTabIndex;
                }

                // if we don't suspend layout, AssignParent will indirectly trigger a layout event
                // before we're ready (AssignParent will fire a PropertyChangedEvent("Visible"), which calls PerformLayout)
                //
#if DEBUG
                int dbgLayoutCheck = owner.LayoutSuspendCount;
#endif
                owner.SuspendLayout();

                try {
                    Control oldParent = value.parent;
                    try {
                        // AssignParent calls into user code - this could throw, which
                        // would make us short-circuit the rest of the reparenting logic.
                        // you could end up with a control half reparented.
                        value.AssignParent(owner);
                    }
                    finally {
                        if (oldParent != value.parent && (owner.state & STATE_CREATED) != 0) {
                            value.SetParentHandle(owner.InternalHandle);
                            if (value.Visible) {
                                value.CreateControl();
                            }
                        }
                    }

                    value.InitLayout();
                }
                finally {
                    owner.ResumeLayout(false);
#if DEBUG
                    owner.AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
                    
                }

                // Not putting in the finally block, as it would eat the original
                // exception thrown from AssignParent if the following throws an exception.
                LayoutTransaction.DoLayout(owner, value, PropertyNames.Parent);
                owner.OnControlAdded(new ControlEventArgs(value));

                
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="ControlCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object control) {
                if (control is Control) {
                    Add((Control)control);
                    return IndexOf((Control)control);
                }
                else {
                    throw new ArgumentException(SR.ControlBadControl, "control");
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.AddRange"]/*' />
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual void AddRange(Control[] controls) {
                if (controls == null) {
                    throw new ArgumentNullException(nameof(controls));
                }
                if (controls.Length > 0) {
#if DEBUG
                    int dbgLayoutCheck = owner.LayoutSuspendCount;
#endif
                    owner.SuspendLayout();
                    try {
                        for(int i=0;i < controls.Length; ++i) {
                            Add(controls[i]);
                        }
                    }
                    finally {
                        owner.ResumeLayout(true);
                    }
#if DEBUG
                    owner.AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="ControlCollection.ICloneable.Clone"]/*' />
            /// <internalonly/>
            object ICloneable.Clone() {
                // Use CreateControlInstance so we get the same type of ControlCollection, but whack the
                // owner so adding controls to this new collection does not affect the control we cloned from.
                ControlCollection ccOther = owner.CreateControlsInstance();

                // We add using InnerList to prevent unnecessary parent cycle checks, etc.
                ccOther.InnerList.AddRange(this);
                return ccOther;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.Contains"]/*' />
            public bool Contains(Control control) {
                return InnerList.Contains(control);
            }


            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.Find"]/*' />
            /// <devdoc>
            ///     <para>Searches for Controls by their Name property, builds up an array
            ///           of all the controls that match.
            ///     </para>
            /// </devdoc>
            public Control[] Find(string key, bool searchAllChildren) {
                if (String.IsNullOrEmpty(key)) {
                   throw new System.ArgumentNullException(nameof(key), SR.FindKeyMayNotBeEmptyOrNull);
                }

                ArrayList foundControls =  FindInternal(key, searchAllChildren, this, new ArrayList());

                // Make this a stongly typed collection.
                Control[] stronglyTypedFoundControls = new Control[foundControls.Count];
                foundControls.CopyTo(stronglyTypedFoundControls, 0);

                return stronglyTypedFoundControls;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.FindInternal"]/*' />
            /// <devdoc>
            ///     <para>Searches for Controls by their Name property, builds up an array list
            ///           of all the controls that match.
            ///     </para>
            /// </devdoc>
            /// <internalonly/>
            private ArrayList FindInternal(string key, bool searchAllChildren, ControlCollection controlsToLookIn, ArrayList foundControls) {
                if ((controlsToLookIn == null) || (foundControls == null)) {
                    return null;
                }

                try {
                    // Perform breadth first search - as it's likely people will want controls belonging
                    // to the same parent close to each other.

                    for (int i = 0; i < controlsToLookIn.Count; i++) {
                          if (controlsToLookIn[i] == null){
                              continue;
                          }

                          if (WindowsFormsUtils.SafeCompareStrings(controlsToLookIn[i].Name, key, /* ignoreCase = */ true)) {
                               foundControls.Add(controlsToLookIn[i]);
                          }
                    }

                    // Optional recurive search for controls in child collections.

                    if (searchAllChildren){
                        for (int i = 0; i < controlsToLookIn.Count; i++) {
                            if (controlsToLookIn[i] == null){
                                continue;
                            }
                            if ((controlsToLookIn[i].Controls != null) && controlsToLookIn[i].Controls.Count > 0){
                                // if it has a valid child collecion, append those results to our collection
                                foundControls = FindInternal(key, searchAllChildren, controlsToLookIn[i].Controls, foundControls);
                            }
                        }
                    }
                }
                catch (Exception e) {
                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                        throw;
                    }
                }
                return foundControls;
            }
          
            public override IEnumerator GetEnumerator() { 
                return new ControlCollectionEnumerator(this); 
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.IndexOf"]/*' />
            public int IndexOf(Control control) {
                return InnerList.IndexOf(control);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.this"]/*' />
            /// <devdoc>
            ///     <para>The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.</para>
            /// </devdoc>
            public virtual int IndexOfKey(String key) {
                // Step 0 - Arg validation
                if (String.IsNullOrEmpty(key)) {
                    return -1; // we dont support empty or null keys.
                }

                // step 1 - check the last cached item
                if (IsValidIndex(lastAccessedIndex))
                {
                    if (WindowsFormsUtils.SafeCompareStrings(this[lastAccessedIndex].Name, key, /* ignoreCase = */ true)) {
                        return lastAccessedIndex;
                    }
                }

                // step 2 - search for the item
                for (int i = 0; i < this.Count; i ++) {
                    if (WindowsFormsUtils.SafeCompareStrings(this[i].Name, key, /* ignoreCase = */ true)) {
                        lastAccessedIndex = i;
                        return i;
                    }
                }

                // step 3 - we didn't find it.  Invalidate the last accessed index and return -1.
                lastAccessedIndex = -1;
                return -1;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.IsValidIndex"]/*' />
            /// <devdoc>
            ///     <para>Determines if the index is valid for the collection.</para>
            /// </devdoc>
            /// <internalonly/>
            private bool IsValidIndex(int index) {
                return ((index >= 0) && (index < this.Count));
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.Owner"]/*' />
            /// <devdoc>
            ///     <para>Who owns this control collection.</para>
            /// </devdoc>
            /// <internalonly/>
            public Control Owner {
                get {
                    return owner;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.Remove"]/*' />
            /// <devdoc>
            ///     Removes control from this control. Inheriting controls should call
            ///     base.remove to ensure that the control is removed.
            /// </devdoc>
            public virtual void Remove(Control value) {

                // Sanity check parameter
                //
                if (value == null) {
                    return;     // Don't do anything
                }

                if (value.ParentInternal == owner) {
                    Debug.Assert(owner != null);

                    value.SetParentHandle(IntPtr.Zero);

                    // Remove the control from the internal control array
                    //
                    InnerList.Remove(value);
                    value.AssignParent(null);
                    LayoutTransaction.DoLayout(owner, value, PropertyNames.Parent);
                    owner.OnControlRemoved(new ControlEventArgs(value));

                    // ContainerControl needs to see it needs to find a new ActiveControl.
                    ContainerControl cc = owner.GetContainerControlInternal() as ContainerControl;
                    if (cc != null)
                    {
                        cc.AfterControlRemoved(value, owner);
                    }
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="ControlCollection.IList.Remove"]/*' />
            /// <internalonly/>
            void IList.Remove(object control) {
                if (control is Control) {
                    Remove((Control)control);
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.RemoveAt"]/*' />
            public void RemoveAt(int index) {
                Remove(this[index]);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.RemoveByKey"]/*' />
            /// <devdoc>
            ///     <para>Removes the child control with the specified key.</para>
            /// </devdoc>
            public virtual void RemoveByKey(string key) {
                int index = IndexOfKey(key);
                if (IsValidIndex(index)) {
                    RemoveAt(index);
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the child control with the specified index.
            /// </devdoc>
            public new virtual Control this[int index] {
                get {
                    //do some bounds checking here...
                    if (index < 0 || index >= Count) {
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.IndexOutOfRange, index.ToString(CultureInfo.CurrentCulture)));
                    }

                    Control control = (Control) InnerList[index];
                    Debug.Assert(control != null, "Why are we returning null controls from a valid index?");
                    return control;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.this1"]/*' />
            /// <devdoc>
            ///     <para>Retrieves the child control with the specified key.</para>
            /// </devdoc>
            public virtual Control this[string key] {
                get {
                    // We do not support null and empty string as valid keys.
                    if (String.IsNullOrEmpty(key)) {
                        return null;
                    }

                    // Search for the key in our collection
                    int index = IndexOfKey(key);
                    if (IsValidIndex(index)) {
                        return this[index];
                    }
                    else {
                        return null;
                    }

                }
            }


            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.Clear"]/*' />
            public virtual void Clear() {
#if DEBUG
                int layoutSuspendCount = owner.LayoutSuspendCount;
#endif
                owner.SuspendLayout();
                // clear all preferred size caches in the tree - 
                // inherited fonts could go away, etc.
                CommonProperties.xClearAllPreferredSizeCaches(owner);

                try {
                    while (Count != 0)
                        RemoveAt(Count - 1);
                } finally {
                    owner.ResumeLayout();
#if DEBUG
                    Debug.Assert(owner.LayoutSuspendCount == layoutSuspendCount, "Suspend/Resume layout mismatch!");
#endif
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.GetChildIndex"]/*' />
            /// <devdoc>
            ///     Retrieves the index of the specified
            ///     child control in this array.  An ArgumentException
            ///     is thrown if child is not parented to this
            ///     Control.
            /// </devdoc>
            public int GetChildIndex(Control child) {
                return GetChildIndex(child, true);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.GetChildIndex1"]/*' />
            /// <devdoc>
            ///     Retrieves the index of the specified
            ///     child control in this array.  An ArgumentException
            ///     is thrown if child is not parented to this
            ///     Control.
            /// </devdoc>
            public virtual int GetChildIndex(Control child, bool throwException) {
                int index = IndexOf(child);
                if (index == -1 && throwException) {
                    throw new ArgumentException(SR.ControlNotChild);
                }
                return index;
            }

            /// <devdoc>
            ///     This is internal virtual method so that "Readonly Collections" can override this and throw as they should not allow changing
            ///     the child control indices.
            /// </devdoc>
            /// <internalonly/>
            internal virtual void SetChildIndexInternal(Control child, int newIndex)
            {
                // Sanity check parameters
                //
                if (child == null) {
                    throw new ArgumentNullException(nameof(child));
                }

                int currentIndex = GetChildIndex(child);

                if (currentIndex == newIndex) {
                    return;
                }

                if (newIndex >= Count || newIndex == -1) {
                    newIndex = Count - 1;
                }

                MoveElement(child, currentIndex, newIndex);
                child.UpdateZOrder();

                LayoutTransaction.DoLayout(owner, child, PropertyNames.ChildIndex);

            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlCollection.SetChildIndex"]/*' />
            /// <devdoc>
            ///     Sets the index of the specified
            ///     child control in this array.  An ArgumentException
            ///     is thrown if child is not parented to this
            ///     Control.
            /// </devdoc>
            public virtual void SetChildIndex(Control child, int newIndex) {
                SetChildIndexInternal(child, newIndex);
            }

            // This is the same as WinformsUtils.ArraySubsetEnumerator 
            // however since we're no longer an array, we've gotta employ a 
            // special version of this.
            private class ControlCollectionEnumerator : IEnumerator {
                private ControlCollection controls; 
                private int current;
                private int originalCount;
    
                public ControlCollectionEnumerator(ControlCollection controls) {
                    this.controls = controls;
                    this.originalCount = controls.Count;
                    current = -1;
                }
    
                public bool MoveNext() {
                    // We have to use Controls.Count here because someone could have deleted 
                    // an item from the array. 
                    //
                    // this can happen if someone does:
                    //     foreach (Control c in Controls) { c.Dispose(); }
                    // 
                    // We also dont want to iterate past the original size of the collection
                    //
                    // this can happen if someone does
                    //     foreach (Control c in Controls) { c.Controls.Add(new Label()); }

                    if (current < controls.Count - 1 && current < originalCount - 1) {
                        current++;
                        return true;
                    }
                    else {
                        return false;
                    }
                }
    
                public void Reset() {
                    current = -1;
                }

                public object Current {
                    get {
                        if (current == -1) {
                            return null;
                        }
                        else {
                            return controls[current];
                        }
                    }
                }
            }

        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleControl.GetControlInfo"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControl.GetControlInfo(NativeMethods.tagCONTROLINFO pCI) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetControlInfo");

            pCI.cb = Marshal.SizeOf(typeof(NativeMethods.tagCONTROLINFO));
            pCI.hAccel = IntPtr.Zero;
            pCI.cAccel = 0;
            pCI.dwFlags = 0;

            if (IsInputKey(Keys.Return)) {
                pCI.dwFlags |= NativeMethods.CTRLINFO_EATS_RETURN;
            }
            if (IsInputKey(Keys.Escape)) {
                pCI.dwFlags |= NativeMethods.CTRLINFO_EATS_ESCAPE;
            }

            ActiveXInstance.GetControlInfo(pCI);
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleControl.OnMnemonic"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControl.OnMnemonic(ref NativeMethods.MSG pMsg) {

            // If we got a mnemonic here, then the appropriate control will focus itself which
            // will cause us to become UI active.
            //
            bool processed = ProcessMnemonic((char)pMsg.wParam);
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnMnemonic processed: " + processed.ToString());
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleControl.OnAmbientPropertyChange"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControl.OnAmbientPropertyChange(int dispID) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnAmbientPropertyChange.  Dispid: " + dispID);
            Debug.Indent();
            ActiveXInstance.OnAmbientPropertyChange(dispID);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleControl.FreezeEvents"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleControl.FreezeEvents(int bFreeze) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:FreezeEvents.  Freeze: " + bFreeze);
            ActiveXInstance.EventsFrozen = (bFreeze != 0);
            Debug.Assert(ActiveXInstance.EventsFrozen == (bFreeze != 0), "Failed to set EventsFrozen correctly");
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.GetWindow"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceActiveObject.GetWindow(out IntPtr hwnd) {
            return((UnsafeNativeMethods.IOleInPlaceObject)this).GetWindow(out hwnd);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.ContextSensitiveHelp"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceActiveObject.ContextSensitiveHelp(int fEnterMode) {
            ((UnsafeNativeMethods.IOleInPlaceObject)this).ContextSensitiveHelp(fEnterMode);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.TranslateAccelerator"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceActiveObject.TranslateAccelerator(ref NativeMethods.MSG lpmsg) {
            return ActiveXInstance.TranslateAccelerator(ref lpmsg);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.OnFrameWindowActivate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceActiveObject.OnFrameWindowActivate(bool fActivate) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnFrameWindowActivate");
            OnFrameWindowActivate(fActivate);
            // return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.OnDocWindowActivate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceActiveObject.OnDocWindowActivate(int fActivate) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnDocWindowActivate.  Activate: " + fActivate.ToString(CultureInfo.InvariantCulture));
            Debug.Indent();
            ActiveXInstance.OnDocWindowActivate(fActivate);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.ResizeBorder"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceActiveObject.ResizeBorder(NativeMethods.COMRECT prcBorder, UnsafeNativeMethods.IOleInPlaceUIWindow pUIWindow, bool fFrameWindow) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:ResizesBorder");
            // return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceActiveObject.EnableModeless"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceActiveObject.EnableModeless(int fEnable) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:EnableModeless");
            // return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceObject.GetWindow"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceObject.GetWindow(out IntPtr hwnd) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetWindow");
            int hr = ActiveXInstance.GetWindow(out hwnd);
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "\twin == " + hwnd);
            return hr;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceObject.ContextSensitiveHelp"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceObject.ContextSensitiveHelp(int fEnterMode) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:ContextSensitiveHelp.  Mode: " + fEnterMode.ToString(CultureInfo.InvariantCulture));
            if (fEnterMode != 0) {
                OnHelpRequested(new HelpEventArgs(Control.MousePosition));
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceObject.InPlaceDeactivate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceObject.InPlaceDeactivate() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:InPlaceDeactivate");
            Debug.Indent();
            ActiveXInstance.InPlaceDeactivate();
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceObject.UIDeactivate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleInPlaceObject.UIDeactivate() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:UIDeactivate");
            return ActiveXInstance.UIDeactivate();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceObject.SetObjectRects"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceObject.SetObjectRects(NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetObjectRects(" + lprcClipRect.left + ", " + lprcClipRect.top + ", " + lprcClipRect.right + ", " + lprcClipRect.bottom + ")");
            Debug.Indent();
            ActiveXInstance.SetObjectRects(lprcPosRect, lprcClipRect);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleInPlaceObject.ReactivateAndUndo"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleInPlaceObject.ReactivateAndUndo() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:ReactivateAndUndo");
            // return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.SetClientSite"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.SetClientSite(UnsafeNativeMethods.IOleClientSite pClientSite) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetClientSite");
            ActiveXInstance.SetClientSite(pClientSite);
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetClientSite"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        UnsafeNativeMethods.IOleClientSite UnsafeNativeMethods.IOleObject.GetClientSite() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetClientSite");
            return ActiveXInstance.GetClientSite();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.SetHostNames"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.SetHostNames(string szContainerApp, string szContainerObj) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetHostNames");
            // Since ActiveX controls never "open" for editing, we shouldn't need
            // to store these.
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.Close"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.Close(int dwSaveOption) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Close. Save option: " + dwSaveOption);
            ActiveXInstance.Close(dwSaveOption);
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.SetMoniker"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.SetMoniker(int dwWhichMoniker, Object pmk) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetMoniker");
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetMoniker"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.GetMoniker(int dwAssign, int dwWhichMoniker, out Object moniker) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMoniker");
            moniker = null;
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.InitFromData"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.InitFromData(IComDataObject pDataObject, int fCreation, int dwReserved) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:InitFromData");
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetClipboardData"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.GetClipboardData(int dwReserved, out IComDataObject data) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetClipboardData");
            data = null;
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.DoVerb"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.DoVerb(int iVerb, IntPtr lpmsg, UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, NativeMethods.COMRECT lprcPosRect) {

            // In Office they are internally casting an iverb to a short and not
            // doing the proper sign extension.  So, we do it here.
            //
            short sVerb = unchecked((short)iVerb);
            iVerb = (int)sVerb;

#if DEBUG
            if (CompModSwitches.ActiveX.TraceInfo) {
                Debug.WriteLine("AxSource:DoVerb {");
                Debug.WriteLine("     verb: " + iVerb);
                Debug.WriteLine("     msg: " + lpmsg);
                Debug.WriteLine("     activeSite: " + pActiveSite);
                Debug.WriteLine("     index: " + lindex);
                Debug.WriteLine("     hwndParent: " + hwndParent);
                Debug.WriteLine("     posRect: " + lprcPosRect);
            }
#endif
            Debug.Indent();
            try {
                ActiveXInstance.DoVerb(iVerb, lpmsg, pActiveSite, lindex, hwndParent, lprcPosRect);
            }
            catch (Exception e) {
                Debug.Fail("Exception occurred during DoVerb(" + iVerb + ") " + e.ToString());
                throw;
            }
            finally {
                Debug.Unindent();
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "}");
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.EnumVerbs"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.EnumVerbs(out UnsafeNativeMethods.IEnumOLEVERB e) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:EnumVerbs");
            return ActiveXImpl.EnumVerbs(out e);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.OleUpdate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.OleUpdate() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OleUpdate");
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.IsUpToDate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.IsUpToDate() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IsUpToDate");
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetUserClassID"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.GetUserClassID(ref Guid pClsid) {
            pClsid = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetUserClassID.  ClassID: " + pClsid.ToString());
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetUserType"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.GetUserType(int dwFormOfType, out string userType) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetUserType");
            if (dwFormOfType == NativeMethods.USERCLASSTYPE_FULL) {
                userType = GetType().FullName;
            }
            else {
                userType = GetType().Name;
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.SetExtent"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.SetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetExtent(" + pSizel.cx + ", " + pSizel.cy + ")");
            Debug.Indent();
            ActiveXInstance.SetExtent(dwDrawAspect, pSizel);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetExtent"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.GetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetExtent.  Aspect: " + dwDrawAspect.ToString(CultureInfo.InvariantCulture));
            Debug.Indent();
            ActiveXInstance.GetExtent(dwDrawAspect, pSizel);
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "value: " + pSizel.cx + ", " + pSizel.cy);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.Advise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.Advise(IAdviseSink pAdvSink, out int cookie) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Advise");
            cookie = ActiveXInstance.Advise(pAdvSink);
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.Unadvise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.Unadvise(int dwConnection) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Unadvise");
            Debug.Indent();
            ActiveXInstance.Unadvise(dwConnection);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.EnumAdvise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.EnumAdvise(out IEnumSTATDATA e) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:EnumAdvise");
            e = null;
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.GetMiscStatus"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.GetMiscStatus(int dwAspect, out int cookie) {
            if ((dwAspect & NativeMethods.DVASPECT_CONTENT) != 0) {
                int status = NativeMethods.OLEMISC_ACTIVATEWHENVISIBLE | NativeMethods.OLEMISC_INSIDEOUT | NativeMethods.OLEMISC_SETCLIENTSITEFIRST;

                if (GetStyle(ControlStyles.ResizeRedraw)) {
                    status |= NativeMethods.OLEMISC_RECOMPOSEONRESIZE;
                }

                if (this is IButtonControl) {
                    status |= NativeMethods.OLEMISC_ACTSLIKEBUTTON;
                }

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMiscStatus. Status: " + status.ToString(CultureInfo.InvariantCulture));
                cookie = status;
            }
            else {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMiscStatus.  Status: ERROR, wrong aspect.");
                cookie = 0;
                return NativeMethods.DV_E_DVASPECT;
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleObject.SetColorScheme"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleObject.SetColorScheme(NativeMethods.tagLOGPALETTE pLogpal) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetColorScheme");
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleWindow.GetWindow"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd) {
            return((UnsafeNativeMethods.IOleInPlaceObject)this).GetWindow(out hwnd);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode) {
            ((UnsafeNativeMethods.IOleInPlaceObject)this).ContextSensitiveHelp(fEnterMode);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersist.GetClassID"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersist.GetClassID(out Guid pClassID) {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersist.GetClassID.  ClassID: " + pClassID.ToString());
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistPropertyBag.InitNew"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistPropertyBag.InitNew() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistPropertyBag.InitNew");
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistPropertyBag.GetClassID"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistPropertyBag.GetClassID(out Guid pClassID) {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistPropertyBag.GetClassID.  ClassID: " + pClassID.ToString());
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistPropertyBag.Load"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistPropertyBag.Load(UnsafeNativeMethods.IPropertyBag pPropBag, UnsafeNativeMethods.IErrorLog pErrorLog) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Load (IPersistPropertyBag)");
            Debug.Indent();
            ActiveXInstance.Load(pPropBag, pErrorLog);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistPropertyBag.Save"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistPropertyBag.Save(UnsafeNativeMethods.IPropertyBag pPropBag, bool fClearDirty, bool fSaveAllProperties) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Save (IPersistPropertyBag)");
            Debug.Indent();
            ActiveXInstance.Save(pPropBag, fClearDirty, fSaveAllProperties);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.GetClassID"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStorage.GetClassID(out Guid pClassID) {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.GetClassID.  ClassID: " + pClassID.ToString());
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.IsDirty"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IPersistStorage.IsDirty() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.IsDirty");
            return ActiveXInstance.IsDirty();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.InitNew"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStorage.InitNew(UnsafeNativeMethods.IStorage pstg) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.InitNew");
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.Load"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IPersistStorage.Load(UnsafeNativeMethods.IStorage pstg) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.Load");
            Debug.Indent();
            ActiveXInstance.Load(pstg);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.Save"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStorage.Save(UnsafeNativeMethods.IStorage pstg, bool fSameAsLoad) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.Save");
            Debug.Indent();
            ActiveXInstance.Save(pstg, fSameAsLoad);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.SaveCompleted"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStorage.SaveCompleted(UnsafeNativeMethods.IStorage pStgNew) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.SaveCompleted");
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStorage.HandsOffStorage"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStorage.HandsOffStorage() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.HandsOffStorage");
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStreamInit.GetClassID"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStreamInit.GetClassID(out Guid pClassID) {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.GetClassID.  ClassID: " + pClassID.ToString());
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStreamInit.IsDirty"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IPersistStreamInit.IsDirty() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.IsDirty");
            return ActiveXInstance.IsDirty();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStreamInit.Load"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStreamInit.Load(UnsafeNativeMethods.IStream pstm) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.Load");
            Debug.Indent();
            ActiveXInstance.Load(pstm);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStreamInit.Save"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStreamInit.Save(UnsafeNativeMethods.IStream pstm, bool fClearDirty) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.Save");
            Debug.Indent();
            ActiveXInstance.Save(pstm, fClearDirty);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStreamInit.GetSizeMax"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStreamInit.GetSizeMax(long pcbSize) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetSizeMax");
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IPersistStreamInit.InitNew"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IPersistStreamInit.InitNew() {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.InitNew");
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IQuickActivate.QuickActivate"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IQuickActivate.QuickActivate(UnsafeNativeMethods.tagQACONTAINER pQaContainer, UnsafeNativeMethods.tagQACONTROL pQaControl) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:QuickActivate");
            Debug.Indent();
            ActiveXInstance.QuickActivate(pQaContainer, pQaControl);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IQuickActivate.SetContentExtent"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IQuickActivate.SetContentExtent(NativeMethods.tagSIZEL pSizel) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetContentExtent");
            Debug.Indent();
            ActiveXInstance.SetExtent(NativeMethods.DVASPECT_CONTENT, pSizel);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IQuickActivate.GetContentExtent"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IQuickActivate.GetContentExtent(NativeMethods.tagSIZEL pSizel) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetContentExtent");
            Debug.Indent();
            ActiveXInstance.GetExtent(NativeMethods.DVASPECT_CONTENT, pSizel);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject.Draw"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject.Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                            IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds,
                                            IntPtr pfnContinue, int dwContinue) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Draw");


            Debug.Indent();
            try {
                ActiveXInstance.Draw(dwDrawAspect, lindex, pvAspect, ptd, hdcTargetDev,
                                 hdcDraw, lprcBounds, lprcWBounds, pfnContinue, dwContinue);

            }
            catch(ExternalException ex) {
                return ex.ErrorCode;
            }
            finally {
                Debug.Unindent();
            }
            return NativeMethods.S_OK;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject.GetColorSet"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject.GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                                   IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet) {

            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetColorSet");

            // GDI+ doesn't do palettes.
            //
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject.Freeze"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject.Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Freezes");
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject.Unfreeze"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject.Unfreeze(int dwFreeze) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Unfreeze");
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject.SetAdvise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IViewObject.SetAdvise(int aspects, int advf, IAdviseSink pAdvSink) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetAdvise");
            ActiveXInstance.SetAdvise(aspects, advf, pAdvSink);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject.GetAdvise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IViewObject.GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAdvise");
            ActiveXInstance.GetAdvise(paspects, padvf, pAdvSink);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.Draw"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IViewObject2.Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                             IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds,
                                             IntPtr pfnContinue, int dwContinue) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Draw");
            Debug.Indent();
            ActiveXInstance.Draw(dwDrawAspect, lindex, pvAspect, ptd, hdcTargetDev,
                                 hdcDraw, lprcBounds, lprcWBounds, pfnContinue, dwContinue);
            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.GetColorSet"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject2.GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                                    IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet) {

            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetColorSet");

            // GDI+ doesn't do palettes.
            //
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.Freeze"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject2.Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Freezes");
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.Unfreeze"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        int UnsafeNativeMethods.IViewObject2.Unfreeze(int dwFreeze) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Unfreeze");
            return NativeMethods.E_NOTIMPL;
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.SetAdvise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IViewObject2.SetAdvise(int aspects, int advf, IAdviseSink pAdvSink) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetAdvise");
            ActiveXInstance.SetAdvise(aspects, advf, pAdvSink);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.GetAdvise"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IViewObject2.GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAdvise");
            ActiveXInstance.GetAdvise(paspects, padvf, pAdvSink);
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.UnsafeNativeMethods.IViewObject2.GetExtent"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        void UnsafeNativeMethods.IViewObject2.GetExtent(int dwDrawAspect, int lindex, NativeMethods.tagDVTARGETDEVICE ptd, NativeMethods.tagSIZEL lpsizel) {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetExtent (IViewObject2)");
            // we already have an implementation of this [from IOleObject]
            //
            ((UnsafeNativeMethods.IOleObject)this).GetExtent(dwDrawAspect, lpsizel);
        }

        #region IKeyboardToolTip implementation

        bool IKeyboardToolTip.CanShowToolTipsNow() {
            IKeyboardToolTip host = this.ToolStripControlHost;
            return this.IsHandleCreated && this.Visible && (host == null || host.CanShowToolTipsNow());
        }

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle() {
            return this.GetToolNativeScreenRectangle();
        }

        IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles() {
            IKeyboardToolTip host = this.ToolStripControlHost;
            if (host == null) {
                return this.GetOwnNeighboringToolsRectangles();
            }
            else {
                return host.GetNeighboringToolsRectangles();
            }
        }

        bool IKeyboardToolTip.IsHoveredWithMouse() {
            return this.ClientRectangle.Contains(this.PointToClient(Control.MousePosition));
        }

        bool IKeyboardToolTip.HasRtlModeEnabled() {
            Control topLevelControl = TopLevelControlInternal;
            return topLevelControl != null && topLevelControl.RightToLeft == RightToLeft.Yes && !this.IsMirrored;
        }

        bool IKeyboardToolTip.AllowsToolTip() {
            IKeyboardToolTip host = this.ToolStripControlHost;
            return (host == null || host.AllowsToolTip()) && this.AllowsKeyboardToolTip();
        }

        IWin32Window IKeyboardToolTip.GetOwnerWindow() {
            return this;
        }

        void IKeyboardToolTip.OnHooked(ToolTip toolTip) {
            this.OnKeyboardToolTipHook(toolTip);
        }

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip) {
            this.OnKeyboardToolTipUnhook(toolTip);
        }

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip) {
            IKeyboardToolTip host  = this.ToolStripControlHost;
            if (host == null) {
                return toolTip.GetCaptionForTool(this);
            }
            else {
                return host.GetCaptionForTool(toolTip);
            }
        }

        bool IKeyboardToolTip.ShowsOwnToolTip() {
            IKeyboardToolTip host = this.ToolStripControlHost;
            return (host == null || host.ShowsOwnToolTip()) && this.ShowsOwnKeyboardToolTip();
        }

        bool IKeyboardToolTip.IsBeingTabbedTo() {
            return Control.AreCommonNavigationalKeysDown();
        }

        bool IKeyboardToolTip.AllowsChildrenToShowToolTips() {
            return this.AllowsChildrenToShowToolTips();
        }

        #endregion

        private IList<Rectangle> GetOwnNeighboringToolsRectangles() {
            Control controlParent = this.ParentInternal;
            if (controlParent != null) {
                Control[] neighboringControls = new Control[4] {
                    // Next and previous control which are accessible with Tab and Shift+Tab
                    controlParent.GetNextSelectableControl(this, true, true, true, false),
                    controlParent.GetNextSelectableControl(this, false, true, true, false),
                    // Next and previous control which are accessible with arrow keys
                    controlParent.GetNextSelectableControl(this, true, false, false, true),
                    controlParent.GetNextSelectableControl(this, false, false, false, true)
                };

                List<Rectangle> neighboringControlsRectangles = new List<Rectangle>(4);
                foreach (Control neighboringControl in neighboringControls) {
                    if (neighboringControl != null && neighboringControl.IsHandleCreated) {
                        neighboringControlsRectangles.Add(((IKeyboardToolTip)neighboringControl).GetNativeScreenRectangle());
                    }
                }

                return neighboringControlsRectangles;
            }
            else {
                return new Rectangle[0];
            }
        }

        internal virtual bool ShowsOwnKeyboardToolTip() {
            return true;
        }

        internal virtual void OnKeyboardToolTipHook(ToolTip toolTip) {
        }

        internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip) {
        }

        internal virtual Rectangle GetToolNativeScreenRectangle() {
            NativeMethods.RECT rectangle = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, this.Handle), ref rectangle);
            return Rectangle.FromLTRB(rectangle.left, rectangle.top, rectangle.right, rectangle.bottom);
        }

        internal virtual bool AllowsKeyboardToolTip() {
            // This internal method enables keyboard ToolTips for all controls including the foreign descendants of Control unless this method is overridden in a child class belonging to this assembly.
            // ElementHost is one such control which is located in a different assembly. 
            // This control doesn't show a mouse ToolTip when hovered and thus should not have a keyboard ToolTip as well.
            // We are not going to fix it now since it seems unlikely that someone would set ToolTip on such special container control as ElementHost.
            return true;
        }

        private static bool IsKeyDown(Keys key) {
            return (tempKeyboardStateArray[(int)key] & Control.HighOrderBitMask) != 0;
        }

        internal static bool AreCommonNavigationalKeysDown() {
            if(tempKeyboardStateArray == null) {
                tempKeyboardStateArray = new byte[256];
            }
            UnsafeNativeMethods.GetKeyboardState(tempKeyboardStateArray);
            return IsKeyDown(Keys.Tab)
                || IsKeyDown(Keys.Up)
                || IsKeyDown(Keys.Down)
                || IsKeyDown(Keys.Left)
                || IsKeyDown(Keys.Right)
                // receiving focus from the ToolStrip
                || IsKeyDown(Keys.Menu)
                || IsKeyDown(Keys.F10)
                || IsKeyDown(Keys.Escape);
        }

        private readonly WeakReference<ToolStripControlHost> toolStripControlHostReference = new WeakReference<ToolStripControlHost>(null);

        internal ToolStripControlHost ToolStripControlHost {
            get {
                ToolStripControlHost value;
                this.toolStripControlHostReference.TryGetTarget(out value);
                return value;
            }
            set {
                this.toolStripControlHostReference.SetTarget(value);
            }
        }

        internal virtual bool AllowsChildrenToShowToolTips() {
            return true;
        }

        /// <devdoc>
        ///      This class holds all of the state data for an ActiveX control and
        ///      supplies the implementation for many of the non-trivial methods.
        /// </devdoc>
        /// <internalonly/>
        private class ActiveXImpl : MarshalByRefObject, IWindowTarget {
            // SECUNDONE : This call is private and is only used to expose a WinForm control as
            //           : an ActiveX control. This class needs more review.
            //

            private static readonly int     hiMetricPerInch = 2540;
            private static readonly int     viewAdviseOnlyOnce = BitVector32.CreateMask();
            private static readonly int     viewAdvisePrimeFirst = BitVector32.CreateMask(viewAdviseOnlyOnce);
            private static readonly int     eventsFrozen = BitVector32.CreateMask(viewAdvisePrimeFirst);
            private static readonly int     changingExtents = BitVector32.CreateMask(eventsFrozen);
            private static readonly int     saving = BitVector32.CreateMask(changingExtents);
            private static readonly int     isDirty = BitVector32.CreateMask(saving);
            private static readonly int     inPlaceActive = BitVector32.CreateMask(isDirty);
            private static readonly int     inPlaceVisible = BitVector32.CreateMask(inPlaceActive);
            private static readonly int     uiActive = BitVector32.CreateMask(inPlaceVisible);
            private static readonly int     uiDead = BitVector32.CreateMask(uiActive);
            private static readonly int     adjustingRect = BitVector32.CreateMask(uiDead);

            private static Point            logPixels = Point.Empty;
            private static NativeMethods.tagOLEVERB[]     axVerbs;

            private static int              globalActiveXCount = 0;
            private static bool             checkedIE;
            private static bool             isIE;


            #if ACTIVEX_SOURCING

            //
            // This has been cut from the product.
            //

            private static ActiveXPropPage  propPage;

            #endif

            private Control                 control;
            private IWindowTarget           controlWindowTarget;
            private IntPtr                  clipRegion;
            private UnsafeNativeMethods.IOleClientSite          clientSite;
            private UnsafeNativeMethods.IOleInPlaceUIWindow     inPlaceUiWindow;
            private UnsafeNativeMethods.IOleInPlaceFrame        inPlaceFrame;
            private ArrayList               adviseList;
            private IAdviseSink             viewAdviseSink;
            private BitVector32             activeXState;
            private AmbientProperty[]       ambientProperties;
            private IntPtr                  hwndParent;
            private IntPtr                  accelTable;
            private short                   accelCount = -1;
            private NativeMethods.COMRECT   adjustRect; // temporary rect used during OnPosRectChange && SetObjectRects

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.ActiveXImpl"]/*' />
            /// <devdoc>
            ///      Creates a new ActiveXImpl.
            /// </devdoc>
            internal ActiveXImpl(Control control) {
                this.control = control;

                // We replace the control's window target with our own.  We
                // do this so we can handle the UI Dead ambient property.
                //
                controlWindowTarget = control.WindowTarget;
                control.WindowTarget = this;

                adviseList = new ArrayList();
                activeXState = new BitVector32();
                ambientProperties = new AmbientProperty[] {
                    new AmbientProperty("Font", NativeMethods.ActiveX.DISPID_AMBIENT_FONT),
                    new AmbientProperty("BackColor", NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR),
                    new AmbientProperty("ForeColor", NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR)
                };
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.AmbientBackColor"]/*' />
            /// <devdoc>
            ///      Retrieves the ambient back color for the control.
            /// </devdoc>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Color AmbientBackColor {
                get {
                    AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR);

                    if (prop.Empty) {
                        Object obj = null;
                        if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR, ref obj)) {
                            if (obj != null) {
                                try {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object color type=" + obj.GetType().FullName);
                                    prop.Value = ColorTranslator.FromOle(Convert.ToInt32(obj, CultureInfo.InvariantCulture));
                                }
                                catch (Exception e) {
                                    Debug.Fail("Failed to massage ambient back color to a Color", e.ToString());

                                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                    if (prop.Value == null) {
                        return Color.Empty;
                    }
                    else {
                        return(Color)prop.Value;
                    }
                }
            }

            /// <devdoc>
            ///      Retrieves the ambient font for the control.
            /// </devdoc>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Font AmbientFont {
                [ResourceExposure(ResourceScope.None)]
                [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
                get {
                    AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FONT);

                    if (prop.Empty) {
                        Object obj = null;
                        if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_FONT, ref obj)) {
                            try {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object font type=" + obj.GetType().FullName);
                                Debug.Assert(obj != null, "GetAmbientProperty failed");
                                IntPtr hfont = IntPtr.Zero;


                                UnsafeNativeMethods.IFont ifont = (UnsafeNativeMethods.IFont)obj;
                                IntSecurity.ObjectFromWin32Handle.Assert();
                                Font font = null;
                                try {
                                    hfont = ifont.GetHFont();
                                    font = Font.FromHfont(hfont);
                                }
                                finally {
                                    CodeAccessPermission.RevertAssert();
                                }
                                prop.Value = font;
                            }
                            catch (Exception e) {
                                if (ClientUtils.IsSecurityOrCriticalException(e)) {
                                    throw;
                                }

                                // Do NULL, so we just defer to the default font
                                prop.Value = null;
                            }
                        }
                    }

                    return(Font)prop.Value;
                }
            }

            /// <devdoc>
            ///      Retrieves the ambient back color for the control.
            /// </devdoc>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal Color AmbientForeColor {
                get {
                    AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR);

                    if (prop.Empty) {
                        Object obj = null;
                        if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR, ref obj)) {
                            if (obj != null) {
                                try {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Object color type=" + obj.GetType().FullName);
                                    prop.Value = ColorTranslator.FromOle(Convert.ToInt32(obj, CultureInfo.InvariantCulture));
                                }
                                catch (Exception e) {
                                    Debug.Fail("Failed to massage ambient fore color to a Color", e.ToString());

                                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                                        throw;
                                    }
                                }
                            }
                        }
                    }

                    if (prop.Value == null) {
                        return Color.Empty;
                    }
                    else {
                        return(Color)prop.Value;
                    }
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.EventsFrozen"]/*' />
            /// <devdoc>
            ///      Determines if events should be frozen.
            /// </devdoc>
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            internal bool EventsFrozen {
                get {
                    return activeXState[eventsFrozen];
                }
                set {
                    activeXState[eventsFrozen] = value;
                }
            }

            /// <devdoc>
            ///     Provides access to the parent window handle
            ///     when we are UI active
            /// </devdoc>
            internal IntPtr HWNDParent {
                get {
                    return hwndParent;
                }
            }

            /// <devdoc>
            ///     Returns true if this app domain is running inside of IE.  The
            ///     control must be sited for this to succeed (it will assert and
            ///     return false if the control is not sited).
            /// </devdoc>
            internal bool IsIE {
                [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
                get {
                    if (!checkedIE) {
                        if (clientSite == null) {
                            Debug.Fail("Do not reference IsIE property unless control is sited.");
                            return false;
                        }

                        // First, is this a managed EXE?  If so, it will correctly shut down
                        // the runtime.
                        if (Assembly.GetEntryAssembly() == null) {
                            // Now check for IHTMLDocument2
                            UnsafeNativeMethods.IOleContainer container;

                            if (NativeMethods.Succeeded(clientSite.GetContainer(out container)) && container is NativeMethods.IHTMLDocument) {
                                isIE = true;
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "AxSource:IsIE running under IE");
                            }

                            if (container != null && UnsafeNativeMethods.IsComObject(container)) {
                                UnsafeNativeMethods.ReleaseComObject(container);
                            }
                        }

                        checkedIE = true;
                    }

                    return isIE;
                }
            }

            /// <devdoc>
            ///      Retrieves the number of logical pixels per inch on the
            ///      primary monitor.
            /// </devdoc>
            private Point LogPixels {
                get {
                    if (logPixels.IsEmpty) {
                        logPixels = new Point();
                        IntPtr dc = UnsafeNativeMethods.GetDC(NativeMethods.NullHandleRef);
                        logPixels.X = UnsafeNativeMethods.GetDeviceCaps(new HandleRef(null, dc), NativeMethods.LOGPIXELSX);
                        logPixels.Y = UnsafeNativeMethods.GetDeviceCaps(new HandleRef(null, dc), NativeMethods.LOGPIXELSY);
                        UnsafeNativeMethods.ReleaseDC(NativeMethods.NullHandleRef, new HandleRef(null, dc));
                    }
                    return logPixels;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Advise"]/*' />
            /// <devdoc>
            ///      Implements IOleObject::Advise
            /// </devdoc>
            internal int Advise(IAdviseSink pAdvSink) {
                adviseList.Add(pAdvSink);
                return adviseList.Count;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Close"]/*' />
            /// <devdoc>
            ///      Implements IOleObject::Close
            /// </devdoc>
            internal void Close(int dwSaveOption) {
                if (activeXState[inPlaceActive]) {
                    InPlaceDeactivate();
                }

                if ((dwSaveOption == NativeMethods.OLECLOSE_SAVEIFDIRTY ||
                     dwSaveOption == NativeMethods.OLECLOSE_PROMPTSAVE) &&
                    activeXState[isDirty]) {

                    if (clientSite != null) {
                        clientSite.SaveObject();
                    }
                    SendOnSave();
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.DoVerb"]/*' />
            /// <devdoc>
            ///      Implements IOleObject::DoVerb
            /// </devdoc>
            internal void DoVerb(int iVerb, IntPtr lpmsg, UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, NativeMethods.COMRECT lprcPosRect) {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "AxSource:ActiveXImpl:DoVerb(" + iVerb + ")");
                switch (iVerb) {
                    case NativeMethods.OLEIVERB_SHOW:
                    case NativeMethods.OLEIVERB_INPLACEACTIVATE:
                    case NativeMethods.OLEIVERB_UIACTIVATE:
                    case NativeMethods.OLEIVERB_PRIMARY:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Show, InPlaceActivate, UIActivate");
                        InPlaceActivate(iVerb);

                        // Now that we're active, send the lpmsg to the control if it
                        // is valid.
                        //
                        if (lpmsg != IntPtr.Zero) {
                            NativeMethods.MSG msg = (NativeMethods.MSG)UnsafeNativeMethods.PtrToStructure(lpmsg, typeof(NativeMethods.MSG));
                            Control target = control;

                            if (msg.hwnd != control.Handle && msg.message >= NativeMethods.WM_MOUSEFIRST && msg.message <= NativeMethods.WM_MOUSELAST) {

                                // Must translate message coordniates over to our HWND.  We first try
                                //
                                IntPtr hwndMap = msg.hwnd == IntPtr.Zero ? hwndParent : msg.hwnd;
                                NativeMethods.POINT pt = new NativeMethods.POINT();
                                pt.x = NativeMethods.Util.LOWORD(msg.lParam);
                                pt.y = NativeMethods.Util.HIWORD(msg.lParam);
                                UnsafeNativeMethods.MapWindowPoints(new HandleRef(null, hwndMap), new HandleRef(control, control.Handle), pt, 1);

                                // check to see if this message should really go to a child
                                //  control, and if so, map the point into that child's window
                                //  coordinates
                                //
                                Control realTarget = target.GetChildAtPoint(new Point(pt.x, pt.y));
                                if (realTarget != null && realTarget != target) {
                                    UnsafeNativeMethods.MapWindowPoints(new HandleRef(target, target.Handle), new HandleRef(realTarget, realTarget.Handle), pt, 1);
                                    target = realTarget;
                                }

                                msg.lParam = NativeMethods.Util.MAKELPARAM(pt.x, pt.y);
                            }

#if DEBUG
                            if (CompModSwitches.ActiveX.TraceVerbose) {
                                Message m = Message.Create(msg.hwnd, msg.message, msg.wParam, msg.lParam);
                                Debug.WriteLine("Valid message pointer passed, sending to control: " + m.ToString());
                            }
#endif // DEBUG

                            if (msg.message == NativeMethods.WM_KEYDOWN && msg.wParam == (IntPtr)NativeMethods.VK_TAB) {
                                target.SelectNextControl(null, Control.ModifierKeys != Keys.Shift, true, true, true);
                            }
                            else {
                                target.SendMessage(msg.message, msg.wParam, msg.lParam);
                            }
                        }
                        break;

                        // These affect our visibility
                        //
                    case NativeMethods.OLEIVERB_HIDE:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Hide");
                        UIDeactivate();
                        InPlaceDeactivate();
                        if (activeXState[inPlaceVisible]) {
                            SetInPlaceVisible(false);
                        }
                        break;

                    #if ACTIVEX_SOURCING

                    //
                    // This has been cut from the product.
                    //

                        // Show our property pages
                        //
                    case NativeMethods.OLEIVERB_PROPERTIES:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Primary, Properties");
                        ShowProperties();
                        break;

                    #endif

                        // All other verbs are notimpl.
                        //
                    default:
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "DoVerb:Other");
                        ThrowHr(NativeMethods.E_NOTIMPL);
                        break;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Draw"]/*' />
            /// <devdoc>
            ///      Implements IViewObject2::Draw.
            /// </devdoc>
            internal void Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                             IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT prcBounds, NativeMethods.COMRECT lprcWBounds,
                             IntPtr pfnContinue, int dwContinue) {

                // support the aspects required for multi-pass drawing
                //
                switch (dwDrawAspect) {
                    case NativeMethods.DVASPECT_CONTENT:
                    case NativeMethods.DVASPECT_OPAQUE:
                    case NativeMethods.DVASPECT_TRANSPARENT:
                        break;
                    default:
                        ThrowHr(NativeMethods.DV_E_DVASPECT);
                        break;
                }

                // We can paint to an enhanced metafile, but not all GDI / GDI+ is
                // supported on classic metafiles.  We throw VIEW_E_DRAW in the hope that
                // the caller figures it out and sends us a different DC.
                //
                int hdcType = UnsafeNativeMethods.GetObjectType(new HandleRef(null, hdcDraw));
                if (hdcType == NativeMethods.OBJ_METADC) {
                    ThrowHr(NativeMethods.VIEW_E_DRAW);
                }

                NativeMethods.RECT rc;
                NativeMethods.POINT pVp = new NativeMethods.POINT();
                NativeMethods.POINT pW = new NativeMethods.POINT();
                NativeMethods.SIZE  sWindowExt = new NativeMethods.SIZE();
                NativeMethods.SIZE  sViewportExt = new NativeMethods.SIZE();
                int   iMode = NativeMethods.MM_TEXT;

                if (!control.IsHandleCreated) {
                    control.CreateHandle();
                }

                // if they didn't give us a rectangle, just copy over ours
                //
                if (prcBounds != null) {
                    rc = new NativeMethods.RECT(prcBounds.left, prcBounds.top, prcBounds.right, prcBounds.bottom);

                    // To draw to a given rect, we scale the DC in such a way as to
                    // make the values it takes match our own happy MM_TEXT.  Then,
                    // we back-convert prcBounds so that we convert it to this coordinate
                    // system.  This puts us in the most similar coordinates as we currently
                    // use.
                    //
                    SafeNativeMethods.LPtoDP(new HandleRef(null, hdcDraw), ref rc, 2);

                    iMode = SafeNativeMethods.SetMapMode(new HandleRef(null, hdcDraw), NativeMethods.MM_ANISOTROPIC);
                    SafeNativeMethods.SetWindowOrgEx(new HandleRef(null, hdcDraw), 0, 0, pW);
                    SafeNativeMethods.SetWindowExtEx(new HandleRef(null, hdcDraw), control.Width, control.Height, sWindowExt);
                    SafeNativeMethods.SetViewportOrgEx(new HandleRef(null, hdcDraw), rc.left, rc.top, pVp);
                    SafeNativeMethods.SetViewportExtEx(new HandleRef(null, hdcDraw), rc.right - rc.left, rc.bottom - rc.top, sViewportExt);
                }

                // Now do the actual drawing.  We must ask all of our children to draw as well.
                try {
                    IntPtr flags = (IntPtr)(NativeMethods.PRF_CHILDREN
                                    | NativeMethods.PRF_CLIENT
                                    | NativeMethods.PRF_ERASEBKGND
                                    | NativeMethods.PRF_NONCLIENT);
                                  //| NativeMethods.PRF_CHECKVISIBLE
                                  //| NativeMethods.PRF_OWNED));

                    if(hdcType != NativeMethods.OBJ_ENHMETADC) {
                        control.SendMessage(NativeMethods.WM_PRINT, hdcDraw, flags);
                    } else {
                        control.PrintToMetaFile(new HandleRef(null, hdcDraw), flags);
                    }
                }
                finally {
                    // And clean up the DC
                    //
                    if (prcBounds != null) {
                        SafeNativeMethods.SetWindowOrgEx(new HandleRef(null, hdcDraw), pW.x, pW.y, null);
                        SafeNativeMethods.SetWindowExtEx(new HandleRef(null, hdcDraw), sWindowExt.cx, sWindowExt.cy, null);
                        SafeNativeMethods.SetViewportOrgEx(new HandleRef(null, hdcDraw), pVp.x, pVp.y, null);
                        SafeNativeMethods.SetViewportExtEx(new HandleRef(null, hdcDraw), sViewportExt.cx, sViewportExt.cy, null);
                        SafeNativeMethods.SetMapMode(new HandleRef(null, hdcDraw), iMode);
                    }
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.EnumVerbs"]/*' />
            /// <devdoc>
            ///     Returns a new verb enumerator.
            /// </devdoc>
            internal static int EnumVerbs(out UnsafeNativeMethods.IEnumOLEVERB e) {
                if (axVerbs == null) {
                    NativeMethods.tagOLEVERB verbShow             = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbInplaceActivate  = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbUIActivate       = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbHide             = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbPrimary          = new NativeMethods.tagOLEVERB();
                    NativeMethods.tagOLEVERB verbProperties       = new NativeMethods.tagOLEVERB();

                    verbShow.lVerb = NativeMethods.OLEIVERB_SHOW;
                    verbInplaceActivate.lVerb = NativeMethods.OLEIVERB_INPLACEACTIVATE;
                    verbUIActivate.lVerb = NativeMethods.OLEIVERB_UIACTIVATE;
                    verbHide.lVerb = NativeMethods.OLEIVERB_HIDE;
                    verbPrimary.lVerb = NativeMethods.OLEIVERB_PRIMARY;
                    verbProperties.lVerb = NativeMethods.OLEIVERB_PROPERTIES;
                    verbProperties.lpszVerbName = SR.AXProperties;
                    verbProperties.grfAttribs = NativeMethods.ActiveX.OLEVERBATTRIB_ONCONTAINERMENU;

                    axVerbs = new NativeMethods.tagOLEVERB[] {
                        verbShow,
                        verbInplaceActivate,
                        verbUIActivate,
                        verbHide,
                        verbPrimary
                    #if ACTIVEX_SOURCING
                        ,verbProperties
                    #endif
                    };
                }


                e = new ActiveXVerbEnum(axVerbs);
                return NativeMethods.S_OK;
            }

            /// <devdoc>
            ///     Converts the given string to a byte array.
            /// </devdoc>
            private static byte[] FromBase64WrappedString(string text) {
                if (text.IndexOfAny(new char[] {' ', '\r', '\n'}) != -1) {
                    StringBuilder sb = new StringBuilder(text.Length);
                    for (int i=0; i<text.Length; i++) {
                        switch (text[i]) {
                            case ' ':
                            case '\r':
                            case '\n':
                                break;
                            default:
                                sb.Append(text[i]);
                                break;
                        }
                    }
                    return Convert.FromBase64String(sb.ToString());
                }
                else {
                    return Convert.FromBase64String(text);
                }
            }

            /// <devdoc>
            ///      Implements IViewObject2::GetAdvise.
            /// </devdoc>
            internal void GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink) {
                // if they want it, give it to them
                //
                if (paspects != null) {
                    paspects[0] = NativeMethods.DVASPECT_CONTENT;
                }

                if (padvf != null) {
                    padvf[0] = 0;

                    if (activeXState[viewAdviseOnlyOnce]) padvf[0] |= NativeMethods.ADVF_ONLYONCE;
                    if (activeXState[viewAdvisePrimeFirst]) padvf[0] |= NativeMethods.ADVF_PRIMEFIRST;
                }

                if (pAdvSink != null) {
                    pAdvSink[0] = viewAdviseSink;
                }
            }

            /// <devdoc>
            ///      Helper function to retrieve an ambient property.  Returns false if the
            ///      property wasn't found.
            /// </devdoc>
            private bool GetAmbientProperty(int dispid, ref Object obj) {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAmbientProperty");
                Debug.Indent();

                if (clientSite is UnsafeNativeMethods.IDispatch) {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "clientSite implements IDispatch");

                    UnsafeNativeMethods.IDispatch disp = (UnsafeNativeMethods.IDispatch)clientSite;
                    Object[] pvt = new Object[1];
                    Guid g = Guid.Empty;
                    int hr = NativeMethods.E_FAIL;

                    IntSecurity.UnmanagedCode.Assert();
                    try {
                        hr = disp.Invoke(dispid, ref g, NativeMethods.LOCALE_USER_DEFAULT,
                                         NativeMethods.DISPATCH_PROPERTYGET, new NativeMethods.tagDISPPARAMS(),
                                         pvt, null, null);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                    if (NativeMethods.Succeeded(hr)) {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "IDispatch::Invoke succeeded. VT=" + pvt[0].GetType().FullName);
                        obj = pvt[0];
                        Debug.Unindent();
                        return true;
                    }
                    else {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "IDispatch::Invoke failed. HR: 0x" + String.Format(CultureInfo.CurrentCulture, "{0:X}", hr));
                    }
                }

                Debug.Unindent();
                return false;
            }

            /// <devdoc>
            ///      Implements IOleObject::GetClientSite.
            /// </devdoc>
            internal UnsafeNativeMethods.IOleClientSite GetClientSite() {
                return clientSite;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.GetControlInfo"]/*' />
            /// <devdoc>
            /// </devdoc>
            /// <internalonly/>
            [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
            internal int GetControlInfo(NativeMethods.tagCONTROLINFO pCI) {
                if (accelCount == -1) {
                    ArrayList mnemonicList = new ArrayList();
                    GetMnemonicList(control, mnemonicList);

                    accelCount = (short)mnemonicList.Count;

                    if (accelCount > 0) {
                        int accelSize = UnsafeNativeMethods.SizeOf(typeof(NativeMethods.ACCEL));

                        // In the worst case we may have two accelerators per mnemonic:  one lower case and
                        // one upper case, hence the * 2 below.
                        //
                        IntPtr accelBlob = Marshal.AllocHGlobal(accelSize * accelCount * 2);

                        try {
                            NativeMethods.ACCEL accel = new NativeMethods.ACCEL();
                            accel.cmd = 0;

                            Debug.Indent();

                            accelCount = 0;

                            foreach(char ch in mnemonicList) {
                                IntPtr structAddr = (IntPtr)((long)accelBlob + accelCount * accelSize);

                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Mnemonic: " + ch.ToString());

                                if (ch >= 'A' && ch <= 'Z') {

                                    // Lower case letter
                                    //
                                    accel.fVirt = NativeMethods.FALT | NativeMethods.FVIRTKEY;
                                    accel.key = (short)(UnsafeNativeMethods.VkKeyScan(ch) & 0x00FF);
                                    Marshal.StructureToPtr(accel, structAddr, false);

                                    // Upper case letter
                                    //
                                    accelCount++;
                                    structAddr = (IntPtr)((long)structAddr + accelSize);
                                    accel.fVirt = NativeMethods.FALT | NativeMethods.FVIRTKEY | NativeMethods.FSHIFT;
                                    Marshal.StructureToPtr(accel, structAddr, false);
                                }
                                else {
                                    // Some non-printable character.
                                    //
                                    accel.fVirt = NativeMethods.FALT | NativeMethods.FVIRTKEY;
                                    short scan = (short)(UnsafeNativeMethods.VkKeyScan(ch));
                                    if ((scan & 0x0100) != 0) {
                                        accel.fVirt |= NativeMethods.FSHIFT;
                                    }
                                    accel.key = (short)(scan & 0x00FF);
                                    Marshal.StructureToPtr(accel, structAddr, false);
                                }

                                accel.cmd++;
                                accelCount++;
                            }

                            Debug.Unindent();

                            // Now create an accelerator table and then free our memory.
                            //

	                    if (accelTable != IntPtr.Zero) {
	                        UnsafeNativeMethods.DestroyAcceleratorTable(new HandleRef(this, accelTable));
				   accelTable = IntPtr.Zero;
	                    	}
                            
                            accelTable = UnsafeNativeMethods.CreateAcceleratorTable(new HandleRef(null, accelBlob), accelCount);
                        }
                        finally {
                            if (accelBlob != IntPtr.Zero) {
                                Marshal.FreeHGlobal(accelBlob);
                            }
                        }
                    }
                }

                pCI.cAccel = accelCount;
                pCI.hAccel = accelTable;
                return NativeMethods.S_OK;
            }

            /// <devdoc>
            ///      Implements IOleObject::GetExtent.
            /// </devdoc>
            internal void GetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel) {
                if ((dwDrawAspect & NativeMethods.DVASPECT_CONTENT) != 0) {
                    Size size = control.Size;

                    Point pt = PixelToHiMetric(size.Width, size.Height);
                    pSizel.cx = pt.X;
                    pSizel.cy = pt.Y;
                }
                else {
                    ThrowHr(NativeMethods.DV_E_DVASPECT);
                }
            }

            /// <devdoc>
            ///     Searches the control hierarchy of the given control and adds
            ///     the mnemonics for each control to mnemonicList.  Each mnemonic
            ///     is added as a char to the list.
            /// </devdoc>
            private void GetMnemonicList(Control control, ArrayList mnemonicList) {
                // Get the mnemonic for our control
                //
                char mnemonic = WindowsFormsUtils.GetMnemonic(control.Text, true);
                if (mnemonic != 0) {
                    mnemonicList.Add(mnemonic);
                }

                // And recurse for our children.
                //
                foreach(Control c in control.Controls) {
                    if (c != null) GetMnemonicList(c, mnemonicList);
                }
            }

            /// <devdoc>
            ///     Name to use for a stream: use the control's type name (max 31 chars, use the end chars
            ///     if it's longer than that)
            /// </devdoc>
            private string GetStreamName()
            {
                string streamName = control.GetType().FullName;
                int len = streamName.Length;
                if (len > 31)
                { // The max allowed length of the stream name is 31.
                    streamName = streamName.Substring(len - 31);
                }
                return streamName;
            }

            /// <devdoc>
            ///      Implements IOleWindow::GetWindow
            /// </devdoc>
            internal int GetWindow(out IntPtr hwnd) {
                if (!activeXState[inPlaceActive]) {
                    hwnd = IntPtr.Zero;
                    return NativeMethods.E_FAIL;
                }
                hwnd = control.Handle;
                return NativeMethods.S_OK;
            }

            /// <devdoc>
            ///      Converts coordinates in HiMetric to pixels.  Used for ActiveX sourcing.
            /// </devdoc>
            /// <internalonly/>
            private Point HiMetricToPixel(int x, int y) {
                Point pt = new Point();
                pt.X = (LogPixels.X * x + hiMetricPerInch / 2) / hiMetricPerInch;
                pt.Y = (LogPixels.Y * y + hiMetricPerInch / 2) / hiMetricPerInch;
                return pt;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.InPlaceActivate"]/*' />
            /// <devdoc>
            ///      In place activates this Object.
            /// </devdoc>
            [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
            internal void InPlaceActivate(int verb)
            {
                // If we don't have a client site, then there's not much to do.
                // We also punt if this isn't an in-place site, since we can't
                // go active then.
                //
                UnsafeNativeMethods.IOleInPlaceSite inPlaceSite = clientSite as UnsafeNativeMethods.IOleInPlaceSite;
                if (inPlaceSite == null)
                {
                    return;
                }

                // If we're not already active, go and do it.
                //
                if (!activeXState[inPlaceActive]) {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> inplaceactive");

                    int hr = inPlaceSite.CanInPlaceActivate();

                    if (hr != NativeMethods.S_OK) {
                        if (NativeMethods.Succeeded(hr)) {
                            hr = NativeMethods.E_FAIL;
                        }
                        ThrowHr(hr);
                    }

                    inPlaceSite.OnInPlaceActivate();

                    activeXState[inPlaceActive] = true;
                }


                // And if we're not visible, do that too.
                //
                if (!activeXState[inPlaceVisible]) {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> inplacevisible");
                    NativeMethods.tagOIFI inPlaceFrameInfo = new NativeMethods.tagOIFI();
                    inPlaceFrameInfo.cb = UnsafeNativeMethods.SizeOf(typeof(NativeMethods.tagOIFI));
                    IntPtr hwndParent = IntPtr.Zero;

                    // We are entering a secure context here.
                    //
                    hwndParent = inPlaceSite.GetWindow();

                    UnsafeNativeMethods.IOleInPlaceFrame pFrame;
                    UnsafeNativeMethods.IOleInPlaceUIWindow pWindow;
                    NativeMethods.COMRECT posRect = new NativeMethods.COMRECT();
                    NativeMethods.COMRECT clipRect = new NativeMethods.COMRECT();

                    if (inPlaceUiWindow != null && UnsafeNativeMethods.IsComObject(inPlaceUiWindow)) {
                        UnsafeNativeMethods.ReleaseComObject(inPlaceUiWindow);
                        inPlaceUiWindow = null;
                    }

                    if (inPlaceFrame != null && UnsafeNativeMethods.IsComObject(inPlaceFrame)) {
                        UnsafeNativeMethods.ReleaseComObject(inPlaceFrame);
                        inPlaceFrame = null;
                    }

                    inPlaceSite.GetWindowContext(out pFrame, out pWindow, posRect, clipRect, inPlaceFrameInfo);

                    SetObjectRects(posRect, clipRect);

                    inPlaceFrame = pFrame;
                    inPlaceUiWindow = pWindow;

                    // We are parenting ourselves
                    // directly to the host window.  The host must
                    // implement the ambient property
                    // DISPID_AMBIENT_MESSAGEREFLECT.
                    // If it doesn't, that means that the host
                    // won't reflect messages back to us.
                    //
                    this.hwndParent = hwndParent;
                    UnsafeNativeMethods.SetParent(new HandleRef(control, control.Handle), new HandleRef(null, hwndParent));

                    // Now create our handle if it hasn't already been done. Note that because
                    // this raises events to the user that it CANNOT be done with a security assertion
                    // in place!
                    //
                    control.CreateControl();

                    clientSite.ShowObject();

                    SetInPlaceVisible(true);
                    Debug.Assert(activeXState[inPlaceVisible], "Failed to set inplacevisible");
                }

                // if we weren't asked to UIActivate, then we're done.
                //
                if (verb != NativeMethods.OLEIVERB_PRIMARY && verb != NativeMethods.OLEIVERB_UIACTIVATE) {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> not becoming UIActive");
                    return;
                }

                // if we're not already UI active, do sow now.
                //
                if (!activeXState[uiActive]) {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> uiactive");
                    activeXState[uiActive] = true;

                    // inform the container of our intent
                    //
                    inPlaceSite.OnUIActivate();

                    // take the focus  [which is what UI Activation is all about !]
                    //
                    if (!control.ContainsFocus) control.FocusInternal();

                    // set ourselves up in the host.
                    //
                    Debug.Assert(inPlaceFrame != null, "Setting us to visible should have created the in place frame");
                    inPlaceFrame.SetActiveObject(control, null);
                    if (inPlaceUiWindow != null) {
                        inPlaceUiWindow.SetActiveObject(control, null);
                    }

                    // we have to explicitly say we don't wany any border space.
                    //
                    int hr = inPlaceFrame.SetBorderSpace(null);
                    if (NativeMethods.Failed(hr) && hr != NativeMethods.OLE_E_INVALIDRECT &&
                        hr != NativeMethods.INPLACE_E_NOTOOLSPACE && hr != NativeMethods.E_NOTIMPL) {
                        UnsafeNativeMethods.ThrowExceptionForHR(hr);
                    }

                    if (inPlaceUiWindow != null) {
                        hr = inPlaceFrame.SetBorderSpace(null);
                        if (NativeMethods.Failed(hr) && hr != NativeMethods.OLE_E_INVALIDRECT &&
                            hr != NativeMethods.INPLACE_E_NOTOOLSPACE && hr != NativeMethods.E_NOTIMPL) {
                            UnsafeNativeMethods.ThrowExceptionForHR(hr);
                        }
                    }
                }
                else {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceVerbose, "\tActiveXImpl:InPlaceActivate --> already uiactive");
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.InPlaceDeactivate"]/*' />
            /// <devdoc>
            ///      Implements IOleInPlaceObject::InPlaceDeactivate.
            /// </devdoc>
            internal void InPlaceDeactivate() {

                // Only do this if we're already in place active.
                //
                if (!activeXState[inPlaceActive]) {
                    return;
                }

                // Deactivate us if we're UI active
                //
                if (activeXState[uiActive]) {
                    UIDeactivate();
                }

                // Some containers may call into us to save, and if we're still
                // active we will try to deactivate and recurse back into the container.
                // So, set the state bits here first.
                //
                activeXState[inPlaceActive] = false;
                activeXState[inPlaceVisible] = false;

                // Notify our site of our deactivation.
                //
                UnsafeNativeMethods.IOleInPlaceSite oleClientSite = clientSite as UnsafeNativeMethods.IOleInPlaceSite;
                if (oleClientSite != null) {
                    IntSecurity.UnmanagedCode.Assert();
                    try {
                        oleClientSite.OnInPlaceDeactivate();
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }

                control.Visible = false;
                hwndParent = IntPtr.Zero;

                if (inPlaceUiWindow != null && UnsafeNativeMethods.IsComObject(inPlaceUiWindow)) {
                    UnsafeNativeMethods.ReleaseComObject(inPlaceUiWindow);
                    inPlaceUiWindow = null;
                }

                if (inPlaceFrame != null && UnsafeNativeMethods.IsComObject(inPlaceFrame)) {
                    UnsafeNativeMethods.ReleaseComObject(inPlaceFrame);
                    inPlaceFrame = null;
                }
            }

            /// <devdoc>
            ///      Implements IPersistStreamInit::IsDirty.
            /// </devdoc>
            internal int IsDirty() {
                if (activeXState[isDirty]) {
                    return NativeMethods.S_OK;
                }
                else {
                    return NativeMethods.S_FALSE;
                }
            }

            /// <devdoc>
            ///      Looks at the property to see if it should be loaded / saved as a resource or
            ///      through a type converter.
            /// </devdoc>
            private bool IsResourceProp(PropertyDescriptor prop) {
                TypeConverter converter = prop.Converter;
                Type[] convertTypes = new Type[] {
                    typeof(string),
                    typeof(byte[])
                    };

                foreach (Type t in convertTypes) {
                    if (converter.CanConvertTo(t) && converter.CanConvertFrom(t)) {
                        return false;
                    }
                }

                // Finally, if the property can be serialized, it is a resource property.
                //
                return (prop.GetValue(control) is ISerializable);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Load"]/*' />
            /// <devdoc>
            ///      Implements IPersistStorage::Load
            /// </devdoc>
            internal void Load(UnsafeNativeMethods.IStorage stg) {
                UnsafeNativeMethods.IStream stream = null;

                IntSecurity.UnmanagedCode.Assert();
                try {
                    stream = stg.OpenStream(this.GetStreamName(), IntPtr.Zero, NativeMethods.STGM_READ | NativeMethods.STGM_SHARE_EXCLUSIVE, 0);
                }
                catch (COMException e) {
                    if (e.ErrorCode == NativeMethods.STG_E_FILENOTFOUND) {
                        // For backward compatibility: We were earlier using GetType().FullName
                        // as the stream name in v1. Lets see if a stream by that name exists.
                        stream = stg.OpenStream(GetType().FullName, IntPtr.Zero, NativeMethods.STGM_READ | NativeMethods.STGM_SHARE_EXCLUSIVE, 0);
                    }
                    else {
                        throw;
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                Load(stream);
                stream = null;
                if (UnsafeNativeMethods.IsComObject(stg)) {
                    UnsafeNativeMethods.ReleaseComObject(stg);
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Load1"]/*' />
            /// <devdoc>
            ///      Implements IPersistStreamInit::Load
            /// </devdoc>
            internal void Load(UnsafeNativeMethods.IStream stream) {
                // We do everything through property bags because we support full fidelity
                // in them.  So, load through that method.
                //
                PropertyBagStream bag = new PropertyBagStream();
                bag.Read(stream);
                Load(bag, null);

                if (UnsafeNativeMethods.IsComObject(stream)) {
                    UnsafeNativeMethods.ReleaseComObject(stream);
                }
            }

            /// <devdoc>
            ///      Implements IPersistPropertyBag::Load
            /// </devdoc>
            internal void Load(UnsafeNativeMethods.IPropertyBag pPropBag, UnsafeNativeMethods.IErrorLog pErrorLog) {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(control,
                    new Attribute[] {DesignerSerializationVisibilityAttribute.Visible});

                for (int i = 0; i < props.Count; i++) {
                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Loading property " + props[i].Name);

                    try {
                        object obj = null;
                        int hr = NativeMethods.E_FAIL;

                        IntSecurity.UnmanagedCode.Assert();
                        try {
                            hr = pPropBag.Read(props[i].Name, ref obj, pErrorLog);
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }

                        if (NativeMethods.Succeeded(hr) && obj != null) {
                            Debug.Indent();
                            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Property was in bag");

                            String errorString = null;
                            int errorCode = 0;

                            try {
                                if (obj.GetType() != typeof(string)) {
                                    Debug.Fail("Expected property " + props[i].Name + " to be stored in IPropertyBag as a string.  Attempting to coerce");
                                    obj = Convert.ToString(obj, CultureInfo.InvariantCulture);
                                }

                                // Determine if this is a resource property or was persisted via a type converter.
                                //
                                if (IsResourceProp(props[i])) {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "It's a resource property");

                                    // Resource property.  We encode these as base 64 strings.  To load them, we convert
                                    // to a binary blob and then de-serialize.
                                    //
                                    byte[] bytes = Convert.FromBase64String(obj.ToString());
                                    MemoryStream stream = new MemoryStream(bytes);
                                    BinaryFormatter formatter = new BinaryFormatter();
                                    props[i].SetValue(control, formatter.Deserialize(stream));
                                }
                                else {
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "It's a standard property");

                                    // Not a resource property.  Use TypeConverters to convert the string back to the data type.  We do
                                    // not check for CanConvertFrom here -- we the conversion fails the type converter will throw,
                                    // and we will log it into the COM error log.
                                    //
                                    TypeConverter converter = props[i].Converter;
                                    Debug.Assert(converter != null, "No type converter for property '" + props[i].Name + "' on class " + control.GetType().FullName);

                                    // Check to see if the type converter can convert from a string.  If it can,.
                                    // use that as it is the best format for IPropertyBag.  Otherwise, check to see
                                    // if it can convert from a byte array.  If it can, get the string, decode it
                                    // to a byte array, and then set the value.
                                    //
                                    object value = null;

                                    if (converter.CanConvertFrom(typeof(string))) {
                                        value = converter.ConvertFromInvariantString(obj.ToString());
                                    }
                                    else if (converter.CanConvertFrom(typeof(byte[]))) {
                                        string objString = obj.ToString();
                                        value = converter.ConvertFrom(null, CultureInfo.InvariantCulture, FromBase64WrappedString(objString));
                                    }
                                    Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Converter returned " + value);
                                    props[i].SetValue(control, value);
                                }
                            }
                            catch (Exception e) {
                                errorString = e.ToString();
                                if (e is ExternalException) {
                                    errorCode = ((ExternalException) e).ErrorCode;
                                }
                                else {
                                    errorCode = NativeMethods.E_FAIL;
                                }
                            }
                            if (errorString != null) {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Exception converting property: " + errorString);
                                if (pErrorLog != null) {
                                    NativeMethods.tagEXCEPINFO err = new NativeMethods.tagEXCEPINFO();
                                    err.bstrSource = control.GetType().FullName;
                                    err.bstrDescription = errorString;
                                    err.scode = errorCode;
                                    pErrorLog.AddError(props[i].Name, err);
                                }
                            }
                            Debug.Unindent();
                        }
                    }
                    catch (Exception ex) {
                        Debug.Fail("Unexpected failure reading property", ex.ToString());

                        if (ClientUtils.IsSecurityOrCriticalException(ex)) {
                            throw;
                        }
                    }
                }
                if (UnsafeNativeMethods.IsComObject(pPropBag)) {
                    UnsafeNativeMethods.ReleaseComObject(pPropBag);
                }
            }

            /// <devdoc>
            ///      Simple lookup to find the AmbientProperty corresponding to the given
            ///      dispid.
            /// </devdoc>
            private AmbientProperty LookupAmbient(int dispid) {
                for (int i = 0; i < ambientProperties.Length; i++) {
                    if (ambientProperties[i].DispID == dispid) {
                        return ambientProperties[i];
                    }
                }
                Debug.Fail("No ambient property for dispid " + dispid.ToString(CultureInfo.InvariantCulture));
                return ambientProperties[0];
            }

            /// <devdoc>
            ///      Merges the input region with the current clipping region.
            ///      The output is always a region that can be fed directly
            ///      to SetWindowRgn.  The region does not have to be destroyed.
            ///      The original region is destroyed if a new region is returned.
            /// </devdoc>
            [ResourceExposure(ResourceScope.Process)]
            [ResourceConsumption(ResourceScope.Process)]
            internal IntPtr MergeRegion(IntPtr region) {
                if (clipRegion == IntPtr.Zero) {
                    return region;
                }

                if (region == IntPtr.Zero) {
                    return clipRegion;
                }

                try {
                    // 

                    IntPtr newRegion = SafeNativeMethods.CreateRectRgn(0, 0, 0, 0);
                    try {
                        SafeNativeMethods.CombineRgn(new HandleRef(null, newRegion), new HandleRef(null, region), new HandleRef(this, clipRegion), NativeMethods.RGN_DIFF);
                        SafeNativeMethods.DeleteObject(new HandleRef(null, region));
                    }
                    catch {
                        SafeNativeMethods.DeleteObject(new HandleRef(null, newRegion));
                        throw;
                    }
                    return newRegion;
                }
                catch (Exception e) {
                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                        throw;
                    }

                    // If something goes wrong, use the original region.
                    return region;
                }
            }

            private void CallParentPropertyChanged(Control control, string propName) {
                switch (propName) {
                    case "BackColor" :
                        control.OnParentBackColorChanged(EventArgs.Empty);
                        break;

                    case "BackgroundImage" :
                        control.OnParentBackgroundImageChanged(EventArgs.Empty);
                        break;

                    case "BindingContext" :
                        control.OnParentBindingContextChanged(EventArgs.Empty);
                        break;

                    case "Enabled" :
                        control.OnParentEnabledChanged(EventArgs.Empty);
                        break;

                    case "Font" :
                        control.OnParentFontChanged(EventArgs.Empty);
                        break;

                    case "ForeColor" :
                        control.OnParentForeColorChanged(EventArgs.Empty);
                        break;

                    case "RightToLeft" :
                        control.OnParentRightToLeftChanged(EventArgs.Empty);
                        break;

                    case "Visible" :
                        control.OnParentVisibleChanged(EventArgs.Empty);
                        break;

                    default:
                        Debug.Fail("There is no property change notification for: " + propName + " on Control.");
                        break;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.OnAmbientPropertyChange"]/*' />
            /// <devdoc>
            ///      Implements IOleControl::OnAmbientPropertyChanged
            /// </devdoc>
            internal void OnAmbientPropertyChange(int dispID) {
                if (dispID != NativeMethods.ActiveX.DISPID_UNKNOWN) {

                    // Look for a specific property that has changed.
                    //
                    for (int i = 0; i < ambientProperties.Length; i++) {
                        if (ambientProperties[i].DispID == dispID) {
                            ambientProperties[i].ResetValue();
                            CallParentPropertyChanged(control, ambientProperties[i].Name);
                            return;
                        }
                    }

                    // Special properties that we care about
                    //
                    Object obj = new Object();

                    switch (dispID) {
                        case NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD:
                            if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD, ref obj)) {
                                activeXState[uiDead] = (bool)obj;
                            }
                            break;

                        case NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYASDEFAULT:
                            IButtonControl ibuttonControl = control as IButtonControl;
                            if (ibuttonControl != null && GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_DISPLAYASDEFAULT, ref obj)) {
                                ibuttonControl.NotifyDefault((bool)obj);
                            }
                            break;
                    }
                }
                else {
                    // Invalidate all properties.  Ideally we should be checking each one, but
                    // that's pretty expensive too.
                    //
                    for (int i = 0; i < ambientProperties.Length; i++) {
                        ambientProperties[i].ResetValue();
                        CallParentPropertyChanged(control, ambientProperties[i].Name);
                    }
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.OnDocWindowActivate"]/*' />
            /// <devdoc>
            ///      Implements IOleInPlaceActiveObject::OnDocWindowActivate.
            /// </devdoc>
            internal void OnDocWindowActivate(int fActivate) {
                if (activeXState[uiActive] && fActivate != 0 && inPlaceFrame != null) {
                    // we have to explicitly say we don't wany any border space.
                    //
                    int hr;
                    IntSecurity.UnmanagedCode.Assert();
                    try {
                        hr = inPlaceFrame.SetBorderSpace(null);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                    if (NativeMethods.Failed(hr) && hr != NativeMethods.INPLACE_E_NOTOOLSPACE && hr != NativeMethods.E_NOTIMPL) {
                        UnsafeNativeMethods.ThrowExceptionForHR(hr);
                    }
                }
            }

            /// <devdoc>
            ///     Called by Control when it gets the focus.
            /// </devdoc>
            internal void OnFocus(bool focus) {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AXSource: SetFocus:  " + focus.ToString());
                if (activeXState[inPlaceActive] && clientSite is UnsafeNativeMethods.IOleControlSite) {
                    IntSecurity.UnmanagedCode.Assert();
                    try {
                        ((UnsafeNativeMethods.IOleControlSite)clientSite).OnFocus(focus ? 1 : 0);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }

                if (focus && activeXState[inPlaceActive] && !activeXState[uiActive]) {
                    InPlaceActivate(NativeMethods.OLEIVERB_UIACTIVATE);
                }
            }

            /// <devdoc>
            ///      Converts coordinates in pixels to HiMetric.
            /// </devdoc>
            /// <internalonly/>
            private Point PixelToHiMetric(int x, int y) {
                Point pt = new Point();
                pt.X = (hiMetricPerInch * x + (LogPixels.X >> 1)) / LogPixels.X;
                pt.Y = (hiMetricPerInch * y + (LogPixels.Y >> 1)) / LogPixels.Y;
                return pt;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.QuickActivate"]/*' />
            /// <devdoc>
            ///     Our implementation of IQuickActivate::QuickActivate
            /// </devdoc>
            internal void QuickActivate(UnsafeNativeMethods.tagQACONTAINER pQaContainer, UnsafeNativeMethods.tagQACONTROL pQaControl) {
                // Hookup our ambient colors
                //
                AmbientProperty prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_BACKCOLOR);
                prop.Value = ColorTranslator.FromOle(unchecked((int)pQaContainer.colorBack));

                prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FORECOLOR);
                prop.Value = ColorTranslator.FromOle(unchecked((int)pQaContainer.colorFore));

                // And our ambient font
                //
                if (pQaContainer.pFont != null) {

                    prop = LookupAmbient(NativeMethods.ActiveX.DISPID_AMBIENT_FONT);

                    IntSecurity.UnmanagedCode.Assert();
                    try
                    {
                        IntPtr hfont = IntPtr.Zero;
                        object objfont = pQaContainer.pFont;
                        UnsafeNativeMethods.IFont ifont = (UnsafeNativeMethods.IFont)objfont;
                        hfont = ifont.GetHFont();
                        Font font = Font.FromHfont(hfont);
                        prop.Value = font;
                    }
                    catch (Exception e) {
                        if (ClientUtils.IsSecurityOrCriticalException(e)) {
                            throw;
                        }

                        // Do NULL, so we just defer to the default font
                        prop.Value = null;
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }

                // Now use the rest of the goo that we got passed in.
                //
                int status;

                pQaControl.cbSize = UnsafeNativeMethods.SizeOf(typeof(UnsafeNativeMethods.tagQACONTROL));

                SetClientSite(pQaContainer.pClientSite);

                if (pQaContainer.pAdviseSink != null) {
                    SetAdvise(NativeMethods.DVASPECT_CONTENT, 0, (IAdviseSink)pQaContainer.pAdviseSink);
                }
                
                IntSecurity.UnmanagedCode.Assert();
                try {
                    ((UnsafeNativeMethods.IOleObject)control).GetMiscStatus(NativeMethods.DVASPECT_CONTENT, out status);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
                pQaControl.dwMiscStatus = status;

                // Advise the event sink so VB6 can catch events raised from UserControls.
                // VB6 expects the control to do this during IQuickActivate, otherwise it will not hook events at runtime.
                // We will do this if all of the following are true:
                //  1. The container (e.g., vb6) has supplied an event sink
                //  2. The control is a UserControl (this is only to limit the scope of the changed behavior)
                //  3. The UserControl has indicated it wants to expose events to COM via the ComSourceInterfacesAttribute
                // Note that the AdviseHelper handles some non-standard COM interop that is required in order to access
                // the events on the CLR-supplied CCW (COM-callable Wrapper.

                if ((pQaContainer.pUnkEventSink != null) && (control is UserControl)) {
                    // Check if this control exposes events to COM. 
                    Type eventInterface = GetDefaultEventsInterface(control.GetType());

                    if (eventInterface != null) {

                        IntSecurity.UnmanagedCode.Assert();
                        try {
                            // For the default source interface, call IConnectionPoint.Advise with the supplied event sink.
                            // This is easier said than done. See notes in AdviseHelper.AdviseConnectionPoint.
                            AdviseHelper.AdviseConnectionPoint(control, pQaContainer.pUnkEventSink, eventInterface, out pQaControl.dwEventCookie);
                        }
                        catch (Exception e) {
                            if (ClientUtils.IsSecurityOrCriticalException(e)) {
                                throw;
                            }
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                    }
                }

                if (pQaContainer.pPropertyNotifySink != null && UnsafeNativeMethods.IsComObject(pQaContainer.pPropertyNotifySink)) {
                    UnsafeNativeMethods.ReleaseComObject(pQaContainer.pPropertyNotifySink);
                }

                if (pQaContainer.pUnkEventSink != null && UnsafeNativeMethods.IsComObject(pQaContainer.pUnkEventSink)) {
                    UnsafeNativeMethods.ReleaseComObject(pQaContainer.pUnkEventSink);
                }
            }

            /// <devdoc>
            ///     Helper class. Calls IConnectionPoint.Advise to hook up a native COM event sink 
            ///     to a manage .NET event interface. 
            ///     The events are exposed to COM by the CLR-supplied COM-callable Wrapper (CCW).
            /// </devdoc>
            internal static class AdviseHelper {
                /// <devdoc>
                ///     Get the COM connection point container from the CLR's CCW and advise for the given event id.
                /// </devdoc>
                public static bool AdviseConnectionPoint(object connectionPoint, object sink, Type eventInterface, out int cookie) {
                    
                    // Note that we cannot simply cast the connectionPoint object to 
                    // System.Runtime.InteropServices.ComTypes.IConnectionPointContainer because the .NET
                    // object doesn't implement it directly. When the object is exposed to COM, the CLR 
                    // implements IConnectionPointContainer on the proxy object called the CCW or COM-callable wrapper.
                    // We use the helper class ComConnectionPointContainer to get to the CCW directly
                    // to to call the interface. 
                    // It is critical to call Dispose to ensure that the IUnknown is released.

                    using (ComConnectionPointContainer cpc = new ComConnectionPointContainer(connectionPoint, true)) {
                        return AdviseConnectionPoint(cpc, sink, eventInterface, out cookie);
                    }
                }

                /// <devdoc>
                ///     Find the COM connection point and call Advise for the given event id.
                /// </devdoc>
                [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Runtime.InteropServices.SafeHandle.DangerousGetHandle")]
                internal static bool AdviseConnectionPoint(ComConnectionPointContainer cpc, object sink, Type eventInterface, out int cookie) {
                    
                    // Note that we cannot simply cast the returned IConnectionPoint to 
                    // System.Runtime.InteropServices.ComTypes.IConnectionPoint because the .NET
                    // object doesn't implement it directly. When the object is exposed to COM, the CLR 
                    // implements IConnectionPoint for the proxy object via the CCW or COM-callable wrapper.
                    // We use the helper class ComConnectionPoint to get to the CCW directly to to call the interface. 
                    // It is critical to call Dispose to ensure that the IUnknown is released.

                    using (ComConnectionPoint cp = cpc.FindConnectionPoint(eventInterface)) {
                        using (SafeIUnknown punkEventsSink = new SafeIUnknown(sink, true)) {
                            // Finally...we can call IConnectionPoint.Advise to hook up a native COM event sink
                            // to a managed .NET event interface.
                            return cp.Advise(punkEventsSink.DangerousGetHandle(), out cookie);
                        }
                    }
                }

                /// <devdoc>
                ///     Wraps a native IUnknown in a SafeHandle. 
                ///     See similar implementaton in the <see cref='System.Transactions.SafeIUnknown'/> class.
                /// </devdoc>
                internal class SafeIUnknown : SafeHandle {

                    /// <devdoc>
                    ///     Wrap an incomoing unknown or get the unknown for the CCW (COM-callable wrapper).
                    /// </devdoc>
                    public SafeIUnknown(object obj, bool addRefIntPtr)
                        : this(obj, addRefIntPtr, Guid.Empty) {
                    }

                    /// <devdoc>
                    ///     Wrap an incomoing unknown or get the unknown for the CCW (COM-callable wrapper).
                    ///     If an iid is supplied, QI for the interface and wrap that unknonwn instead.
                    /// </devdoc>
                    public SafeIUnknown(object obj, bool addRefIntPtr, Guid iid)
                        : base(IntPtr.Zero, true) {

                        System.Runtime.CompilerServices.RuntimeHelpers.PrepareConstrainedRegions();
                        try {

                            // Set this.handle in a finally block to ensure the com ptr is set in the SafeHandle
                            // even if the runtime throws a exception (such as ThreadAbortException) during the call.
                            // This ensures that the finalizer will clean up the COM reference.
                        }
                        finally {
                            
                            // Get a raw IUnknown for this object. 
                            // We are responsible for releasing the IUnknown ourselves.
                            IntPtr unknown;
                            
                            if (obj is IntPtr) {
                                unknown = (IntPtr)obj;

                                // The incoming IntPtr may already be reference counted or not, depending on 
                                // where it came from. The caller needs to tell us whether to add-ref or not.
                                if (addRefIntPtr) {
                                    Marshal.AddRef(unknown);
                                }

                            } else {
                                // GetIUnknownForObject will return a reference-counted object
                                unknown = Marshal.GetIUnknownForObject(obj);
                            }
                            
                            // Attempt QueryInterface if an iid is specified.
                            if (iid != Guid.Empty) {
                                IntPtr oldUnknown = unknown;
                                try {
                                    unknown = InternalQueryInterface(unknown, ref iid);
                                }
                                finally {
                                    // It is critical to release the original unknown if 
                                    // InternalQueryInterface throws out so we don't leak ref counts.
                                    Marshal.Release(oldUnknown);
                                }
                            }

                            // Preserve the com ptr in the SafeHandle.
                            this.handle = unknown;
                        }
                    }

                    /// <devdoc>
                    ///     Helper function for QueryInterface.
                    /// </devdoc>
                    private static IntPtr InternalQueryInterface(IntPtr pUnk, ref Guid iid) {
                        IntPtr ppv;
                        int hresult = Marshal.QueryInterface(pUnk, ref iid, out ppv);
                        if (hresult != 0 || ppv == IntPtr.Zero) {
                            throw new InvalidCastException(SR.AxInterfaceNotSupported);
                        }
                        return ppv;
                    }

                    /// <devdoc>
                    ///     Return whether the handle is invalid.
                    /// </devdoc>
                    public sealed override bool IsInvalid {
                        get {
                            if (!base.IsClosed) {
                                return (IntPtr.Zero == base.handle);
                            }
                            return true;
                        }
                    }

                    /// <devdoc>
                    ///     Release the IUnknown.
                    /// </devdoc>
                    protected sealed override bool ReleaseHandle() {
                        IntPtr ptr1 = base.handle;
                        base.handle = IntPtr.Zero;
                        if (IntPtr.Zero != ptr1) {
                            Marshal.Release(ptr1);
                        }
                        return true;
                    }

                    /// <devdoc>
                    ///     Helper function to load a COM v-table from a com object pointer.
                    /// </devdoc>
                    protected V LoadVtable<V>() {
                        IntPtr vtblptr = Marshal.ReadIntPtr(this.handle, 0);
                        return (V)Marshal.PtrToStructure(vtblptr, typeof(V));
                    }
                }

                /// <devdoc>
                ///     Helper class to access IConnectionPointContainer from a .NET COM-callable wrapper. 
                ///     The IConnectionPointContainer COM pointer is wrapped in a SafeHandle.
                /// </devdoc>
                internal sealed class ComConnectionPointContainer 
                    : SafeIUnknown {

                    public ComConnectionPointContainer(object obj, bool addRefIntPtr)
                        : base(obj, addRefIntPtr, typeof(System.Runtime.InteropServices.ComTypes.IConnectionPointContainer).GUID) {
                        this.vtbl = base.LoadVtable<VTABLE>();
                    }

                    private VTABLE vtbl;

                    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
                    [StructLayout(LayoutKind.Sequential)]
                    private class VTABLE {
                        public IntPtr QueryInterfacePtr;
                        public IntPtr AddRefPtr;
                        public IntPtr ReleasePtr;
                        public IntPtr EnumConnectionPointsPtr;
                        public IntPtr FindConnectionPointPtr;
                    }

                    /// <devdoc>
                    ///     Call IConnectionPointContainer.FindConnectionPoint using Delegate.Invoke on the v-table slot.
                    /// </devdoc>
                    public ComConnectionPoint FindConnectionPoint(Type eventInterface) {
                        FindConnectionPointD findConnectionPoint = (FindConnectionPointD)Marshal.GetDelegateForFunctionPointer(this.vtbl.FindConnectionPointPtr, typeof(FindConnectionPointD));
                        IntPtr result = IntPtr.Zero;

                        Guid iid = eventInterface.GUID;
                        int hresult = findConnectionPoint.Invoke(this.handle, ref iid, out result);
                        if (hresult != 0 || result == IntPtr.Zero) {
                            throw new ArgumentException(string.Format(SR.AXNoConnectionPoint, eventInterface.Name));
                        }

                        return new ComConnectionPoint(result, false);   // result is already ref-counted as an out-param so pass in false
                    }

                    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
                    private delegate int FindConnectionPointD(IntPtr This, ref Guid iid, out IntPtr ppv);
                }

                /// <devdoc>
                ///     Helper class to access IConnectionPoint from a .NET COM-callable wrapper. 
                ///     The IConnectionPoint COM pointer is wrapped in a SafeHandle.
                /// </devdoc>
                internal sealed class ComConnectionPoint 
                    : SafeIUnknown {

                    public ComConnectionPoint(object obj, bool addRefIntPtr)
                        : base(obj, addRefIntPtr, typeof(System.Runtime.InteropServices.ComTypes.IConnectionPoint).GUID) {
                        this.vtbl = this.LoadVtable<VTABLE>();
                    }

                    [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
                    [StructLayout(LayoutKind.Sequential)]
                    private class VTABLE {
                        public IntPtr QueryInterfacePtr;
                        public IntPtr AddRefPtr;
                        public IntPtr ReleasePtr;
                        public IntPtr GetConnectionInterfacePtr;
                        public IntPtr GetConnectionPointContainterPtr;
                        public IntPtr AdvisePtr;
                        public IntPtr UnadvisePtr;
                        public IntPtr EnumConnectionsPtr;
                    }

                    private VTABLE vtbl;

                    /// <devdoc>
                    ///     Call IConnectioinPoint.Advise using Delegate.Invoke on the v-table slot.
                    /// </devdoc>
                    public bool Advise(IntPtr punkEventSink, out int cookie) {
                        AdviseD advise = (AdviseD)Marshal.GetDelegateForFunctionPointer(this.vtbl.AdvisePtr, typeof(AdviseD));
                        if (advise.Invoke(this.handle, punkEventSink, out cookie) == 0) {
                            return true;
                        }
                        return false;
                    }

                    [UnmanagedFunctionPointerAttribute(CallingConvention.StdCall)]
                    private delegate int AdviseD(IntPtr This, IntPtr punkEventSink, out int cookie);
                }

            }

            /// <devdoc>
            ///     Return the default COM events interface declared on a .NET class.
            ///     This looks for the ComSourceInterfacesAttribute and returns the .NET 
            ///     interface type of the first interface declared.
            /// </devdoc>
            private static Type GetDefaultEventsInterface(Type controlType) {
                
                Type eventInterface = null;
                object[] custom = controlType.GetCustomAttributes(typeof(ComSourceInterfacesAttribute), false);

                if (custom.Length > 0) {
                    ComSourceInterfacesAttribute coms = (ComSourceInterfacesAttribute)custom[0];
                    string eventName = coms.Value.Split(new char[] { '\0' })[0];
                    eventInterface = controlType.Module.Assembly.GetType(eventName, false);
                    if (eventInterface == null) {
                        eventInterface = Type.GetType(eventName, false);
                    }
                }

                return eventInterface;
            }


            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Save"]/*' />
            /// <devdoc>
            ///      Implements IPersistStorage::Save
            /// </devdoc>
            internal void Save(UnsafeNativeMethods.IStorage stg, bool fSameAsLoad) {
                UnsafeNativeMethods.IStream stream = null;

                IntSecurity.UnmanagedCode.Assert();
                try {
                    stream = stg.CreateStream(this.GetStreamName(), NativeMethods.STGM_WRITE | NativeMethods.STGM_SHARE_EXCLUSIVE | NativeMethods.STGM_CREATE, 0, 0);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
                Debug.Assert(stream != null, "Stream should be non-null, or an exception should have been thrown.");
                Save(stream, true);
                UnsafeNativeMethods.ReleaseComObject(stream);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Save1"]/*' />
            /// <devdoc>
            ///      Implements IPersistStreamInit::Save
            /// </devdoc>
            internal void Save(UnsafeNativeMethods.IStream stream, bool fClearDirty) {
                // We do everything through property bags because we support full fidelity
                // in them.  So, save through that method.
                //
                PropertyBagStream bag = new PropertyBagStream();
                Save(bag, fClearDirty, false);
                IntSecurity.UnmanagedCode.Assert();
                try {
                    bag.Write(stream);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
                if (UnsafeNativeMethods.IsComObject(stream)) {
                    UnsafeNativeMethods.ReleaseComObject(stream);
                }
            }

            /// <devdoc>
            ///      Implements IPersistPropertyBag::Save
            /// </devdoc>
            [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
            internal void Save(UnsafeNativeMethods.IPropertyBag pPropBag, bool fClearDirty, bool fSaveAllProperties) {
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(control,
                    new Attribute[] {DesignerSerializationVisibilityAttribute.Visible});

                for (int i = 0; i < props.Count; i++) {
                    if (fSaveAllProperties || props[i].ShouldSerializeValue(control)) {
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Saving property " + props[i].Name);

                        object propValue;

                        if (IsResourceProp(props[i])) {

                            // Resource property.  Save this to the bag as a 64bit encoded string.
                            //
                            MemoryStream stream = new MemoryStream();
                            BinaryFormatter formatter = new BinaryFormatter();
                            formatter.Serialize(stream, props[i].GetValue(control));
                            byte[] bytes = new byte[(int)stream.Length];
                            stream.Position = 0;
                            stream.Read(bytes, 0, bytes.Length);
                            propValue = Convert.ToBase64String(bytes);
                            pPropBag.Write(props[i].Name, ref propValue);
                        }
                        else {

                            // Not a resource property.  Persist this using standard type converters.
                            //
                            TypeConverter converter = props[i].Converter;
                            Debug.Assert(converter != null, "No type converter for property '" + props[i].Name + "' on class " + control.GetType().FullName);

                            if (converter.CanConvertFrom(typeof(string))) {
                                propValue = converter.ConvertToInvariantString(props[i].GetValue(control));
                                pPropBag.Write(props[i].Name, ref propValue);
                            }
                            else if (converter.CanConvertFrom(typeof(byte[]))) {
                                byte[] data = (byte[])converter.ConvertTo(null, CultureInfo.InvariantCulture, props[i].GetValue(control), typeof(byte[]));
                                propValue = Convert.ToBase64String(data);
                                pPropBag.Write(props[i].Name, ref propValue);
                            }
                        }
                    }
                }

                if (UnsafeNativeMethods.IsComObject(pPropBag)) {
                    UnsafeNativeMethods.ReleaseComObject(pPropBag);
                }

                if (fClearDirty) {
                    activeXState[isDirty] = false;
                }
            }

            /// <devdoc>
            ///      Fires the OnSave event to all of our IAdviseSink
            ///      listeners.  Used for ActiveXSourcing.
            /// </devdoc>
            /// <internalonly/>
            private void SendOnSave() {
                int cnt = adviseList.Count;
                IntSecurity.UnmanagedCode.Assert();
                for (int i = 0; i < cnt; i++) {
                    IAdviseSink s = (IAdviseSink)adviseList[i];
                    Debug.Assert(s != null, "NULL in our advise list");
                    s.OnSave();
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.SetAdvise"]/*' />
            /// <devdoc>
            ///      Implements IViewObject2::SetAdvise.
            /// </devdoc>
            internal void SetAdvise(int aspects, int advf, IAdviseSink pAdvSink) {
                // if it's not a content aspect, we don't support it.
                //
                if ((aspects & NativeMethods.DVASPECT_CONTENT) == 0) {
                    ThrowHr(NativeMethods.DV_E_DVASPECT);
                }

                // set up some flags  [we gotta stash for GetAdvise ...]
                //
                activeXState[viewAdvisePrimeFirst] = (advf & NativeMethods.ADVF_PRIMEFIRST) != 0 ? true : false;
                activeXState[viewAdviseOnlyOnce] = (advf & NativeMethods.ADVF_ONLYONCE) != 0 ? true : false;

                if (viewAdviseSink != null && UnsafeNativeMethods.IsComObject(viewAdviseSink)) {
                    UnsafeNativeMethods.ReleaseComObject(viewAdviseSink);
                }

                viewAdviseSink = pAdvSink;

                // prime them if they want it [we need to store this so they can get flags later]
                //
                if (activeXState[viewAdvisePrimeFirst]) {
                    ViewChanged();
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.SetClientSite"]/*' />
            /// <devdoc>
            ///      Implements IOleObject::SetClientSite.
            /// </devdoc>
            internal void SetClientSite(UnsafeNativeMethods.IOleClientSite value) {
                if (clientSite != null) {

                    if (value == null) {
                        globalActiveXCount--;

                        if (globalActiveXCount == 0 && IsIE) {

                            // This the last ActiveX control and we are
                            // being hosted in IE.  Use private reflection
                            // to ask SystemEvents to shutdown.  This is to
                            // prevent a crash.

                            // 





                            new PermissionSet(PermissionState.Unrestricted).Assert();
                            try {
                                MethodInfo method = typeof(SystemEvents).GetMethod("Shutdown",
                                                                                  BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                                                                                  null, new Type[0], new ParameterModifier[0]);
                                Debug.Assert(method != null, "No Shutdown method on SystemEvents");
                                if (method != null) {
                                    method.Invoke(null, null);
                                }
                            }
                            finally {
                                CodeAccessPermission.RevertAssert();
                            }
                        }
                    }

                    if (UnsafeNativeMethods.IsComObject(clientSite)) {
                        IntSecurity.UnmanagedCode.Assert();
                        try {
                            Marshal.FinalReleaseComObject(clientSite);
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                    }
                }

                clientSite = value;

                if (clientSite != null) {
                    control.Site = new AxSourcingSite(control, clientSite, "ControlAxSourcingSite");
                }
                else {
                    control.Site = null;
                }

                // Get the ambient properties that effect us...
                //
                Object obj = new Object();
                if (GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD, ref obj)) {
                    activeXState[uiDead] = (bool)obj;
                }

                if (control is IButtonControl && GetAmbientProperty(NativeMethods.ActiveX.DISPID_AMBIENT_UIDEAD, ref obj)) {
                    ((IButtonControl)control).NotifyDefault((bool)obj);
                }

                if (clientSite == null) {
                    if (accelTable != IntPtr.Zero) {
                        UnsafeNativeMethods.DestroyAcceleratorTable(new HandleRef(this, accelTable));
                        accelTable = IntPtr.Zero;
                        accelCount = -1;
                    }

                    if (IsIE) {
                        this.control.Dispose();
                    }
                }
                else {
                    globalActiveXCount++;

                    if (globalActiveXCount == 1 && IsIE) {

                        // This the first ActiveX control and we are
                        // being hosted in IE.  Use private reflection
                        // to ask SystemEvents to start.  Startup will only
                        // restart system events if we previously shut it down.
                        // This is to prevent a crash.
                        //
                        // 





                        new PermissionSet(PermissionState.Unrestricted).Assert();
                        try {
                            MethodInfo method = typeof(SystemEvents).GetMethod("Startup",
                                                                              BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.InvokeMethod,
                                                                              null, new Type[0], new ParameterModifier[0]);
                            Debug.Assert(method != null, "No Startup method on SystemEvents");
                            if (method != null) {
                                method.Invoke(null, null);
                            }
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                    }
                }
                control.OnTopMostActiveXParentChanged(EventArgs.Empty);
            }

            /// <devdoc>
            ///      Implements IOleObject::SetExtent
            /// </devdoc>
            [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
            internal void SetExtent(int dwDrawAspect, NativeMethods.tagSIZEL pSizel) {
                if ((dwDrawAspect & NativeMethods.DVASPECT_CONTENT) != 0) {

                    if (activeXState[changingExtents]) {
                        return;
                    }

                    activeXState[changingExtents] = true;

                    try {
                        Size size = new Size(HiMetricToPixel(pSizel.cx, pSizel.cy));
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : new size:" + size.ToString());

                        // If we're in place active, let the in place site set our bounds.
                        // Otherwise, just set it on our control directly.
                        //
                        if (activeXState[inPlaceActive]) {
                            UnsafeNativeMethods.IOleInPlaceSite ioleClientSite = clientSite as UnsafeNativeMethods.IOleInPlaceSite;
                            if (ioleClientSite != null) {
                                Rectangle bounds = control.Bounds;
                                bounds.Location = new Point(bounds.X, bounds.Y);
                                Size adjusted = new Size(size.Width, size.Height);
                                bounds.Width = adjusted.Width;
                                bounds.Height = adjusted.Height;
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Announcing to in place site that our rect has changed.");
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "            Announcing rect = " + bounds);
                                Debug.Assert(clientSite != null, "How can we setextent before we are sited??");

                                ioleClientSite.OnPosRectChange(NativeMethods.COMRECT.FromXYWH(bounds.X, bounds.Y, bounds.Width, bounds.Height));
                            }
                        }

                        control.Size = size;

                        // Check to see if the control overwrote our size with
                        // its own values.
                        //
                        if (!control.Size.Equals(size)) {
                            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Control has changed size.  Setting dirty bit");
                            activeXState[isDirty] = true;

                            // If we're not inplace active, then anounce that the view changed.
                            //
                            if (!activeXState[inPlaceActive]) {
                                ViewChanged();
                            }

                            // We need to call RequestNewObjectLayout
                            // here so we visually display our new extents.
                            //
                            if (!activeXState[inPlaceActive] && clientSite != null) {
                                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "SetExtent : Requesting new Object layout.");
                                clientSite.RequestNewObjectLayout();
                            }
                        }
                    }
                    finally {
                        activeXState[changingExtents] = false;
                    }
                }
                else {
                    // We don't support any other aspects
                    //
                    ThrowHr(NativeMethods.DV_E_DVASPECT);
                }
            }

            /// <devdoc>
            ///      Marks our state as in place visible.
            /// </devdoc>
            private void SetInPlaceVisible(bool visible) {
                activeXState[inPlaceVisible] = visible;
                control.Visible = visible;
            }

            /// <devdoc>
            ///      Implements IOleInPlaceObject::SetObjectRects.
            /// </devdoc>
            internal void SetObjectRects(NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect) {

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo) {
                    Debug.WriteLine("SetObjectRects:");
                    Debug.Indent();

                    if (lprcPosRect != null) {
                        Debug.WriteLine("PosLeft:    " + lprcPosRect.left.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("PosTop:     " + lprcPosRect.top.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("PosRight:   " + lprcPosRect.right.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("PosBottom:  " + lprcPosRect.bottom.ToString(CultureInfo.InvariantCulture));
                    }

                    if (lprcClipRect != null) {
                        Debug.WriteLine("ClipLeft:   " + lprcClipRect.left.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("ClipTop:    " + lprcClipRect.top.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("ClipRight:  " + lprcClipRect.right.ToString(CultureInfo.InvariantCulture));
                        Debug.WriteLine("ClipBottom: " + lprcClipRect.bottom.ToString(CultureInfo.InvariantCulture));
                    }
                    Debug.Unindent();
                }
#endif

                Rectangle posRect = Rectangle.FromLTRB(lprcPosRect.left, lprcPosRect.top, lprcPosRect.right, lprcPosRect.bottom);

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Set control bounds: " + posRect.ToString());

                // ActiveX expects to be notified when a control's bounds change, and also
                // intends to notify us through SetObjectRects when we report that the
                // bounds are about to change.  We implement this all on a control's Bounds
                // property, which doesn't use this callback mechanism.  The adjustRect
                // member handles this. If it is non-null, then we are being called in
                // response to an OnPosRectChange call.  In this case we do not
                // set the control bounds but set the bounds on the adjustRect.  When
                // this returns from the container and comes back to our OnPosRectChange
                // implementation, these new bounds will be handed back to the control
                // for the actual window change.
                //
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Old Control Bounds: " + control.Bounds);
                if (activeXState[adjustingRect]) {
                    adjustRect.left = posRect.X;
                    adjustRect.top = posRect.Y;
                    adjustRect.right = posRect.Width + posRect.X;
                    adjustRect.bottom = posRect.Height + posRect.Y;
                }
                else {
                    activeXState[adjustingRect] = true;
                    try {
                        control.Bounds = posRect;
                    }
                    finally {
                        activeXState[adjustingRect] = false;
                    }
                }

                bool setRegion = false;

                if (clipRegion != IntPtr.Zero) {
                    // Bad -- after calling SetWindowReg, windows owns the region.
                    //SafeNativeMethods.DeleteObject(clipRegion);
                    clipRegion = IntPtr.Zero;
                    setRegion = true;
                }

                if (lprcClipRect != null) {

                    // The container wants us to clip, so figure out if we really
                    // need to.
                    //
                    Rectangle clipRect = Rectangle.FromLTRB(lprcClipRect.left, lprcClipRect.top, lprcClipRect.right, lprcClipRect.bottom);

                    Rectangle intersect;

                    // Trident always sends an empty ClipRect... and so, we check for that and not do an
                    // intersect in that case.
                    //
                    if (!clipRect.IsEmpty)
                        intersect = Rectangle.Intersect(posRect, clipRect);
                    else
                        intersect = posRect;

                    if (!intersect.Equals(posRect)) {

                        // Offset the rectangle back to client coordinates
                        //
                        NativeMethods.RECT rcIntersect = NativeMethods.RECT.FromXYWH(intersect.X, intersect.Y, intersect.Width, intersect.Height);
                        IntPtr hWndParent = UnsafeNativeMethods.GetParent(new HandleRef(control, control.Handle));

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Old Intersect: " + new Rectangle(rcIntersect.left, rcIntersect.top, rcIntersect.right-rcIntersect.left, rcIntersect.bottom-rcIntersect.top));
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "New Control Bounds: " + posRect);

                        UnsafeNativeMethods.MapWindowPoints(new HandleRef(null, hWndParent), new HandleRef(control, control.Handle), ref rcIntersect, 2);

                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "New Intersect: " + new Rectangle(rcIntersect.left, rcIntersect.top, rcIntersect.right-rcIntersect.left, rcIntersect.bottom-rcIntersect.top));

                        // Create a Win32 region for it
                        //
                        clipRegion = SafeNativeMethods.CreateRectRgn(rcIntersect.left, rcIntersect.top,
                                                                 rcIntersect.right, rcIntersect.bottom);
                        setRegion = true;
                        Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "Created clipping region");
                    }
                }

                // If our region has changed, set the new value.  We only do this if
                // the handle has been created, since otherwise the control will
                // merge our region automatically.
                //
                if (setRegion && control.IsHandleCreated) {
                    IntPtr finalClipRegion = clipRegion;

                    Region controlRegion = control.Region;
                    if (controlRegion != null) {
                        IntPtr rgn = control.GetHRgn(controlRegion);
                        finalClipRegion = MergeRegion(rgn);
                    }

                    UnsafeNativeMethods.SetWindowRgn(new HandleRef(control, control.Handle), new HandleRef(this, finalClipRegion), SafeNativeMethods.IsWindowVisible(new HandleRef(control, control.Handle)));
                }

                // Yuck.  Forms^3 uses transparent overlay windows that appear to cause
                // painting artifacts.  Flicker like a banshee.
                //
                control.Invalidate();
            }

            #if ACTIVEX_SOURCING

            //
            // This has been cut from the product.
            //

            /// <devdoc>
            ///      Shows a property page dialog.
            /// </devdoc>
            private void ShowProperties() {
                if (propPage == null) {
                    propPage = new ActiveXPropPage(control);
                }

                if (inPlaceFrame != null) {
                    inPlaceFrame.EnableModeless(0);
                }
                try {
                    propPage.Edit(control);
                }
                finally {
                    if (inPlaceFrame != null) {
                        inPlaceFrame.EnableModeless(1);
                    }
                }
            }
            #endif

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.ThrowHr"]/*' />
            /// <devdoc>
            ///      Throws the given hresult.  This is used by ActiveX sourcing.
            /// </devdoc>
            internal static void ThrowHr(int hr) {
                ExternalException e = new ExternalException(SR.ExternalException, hr);
                throw e;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.TranslateAccelerator"]/*' />
            /// <devdoc>
            ///     Handles IOleControl::TranslateAccelerator
            /// </devdoc>
            internal int TranslateAccelerator(ref NativeMethods.MSG lpmsg) {
               

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo) {
                    if (!control.IsHandleCreated) {
                        Debug.WriteLine("AxSource: TranslateAccelerator before handle creation");
                    }
                    else {
                        Message m = Message.Create(lpmsg.hwnd, lpmsg.message, lpmsg.wParam, lpmsg.lParam);
                        Debug.WriteLine("AxSource: TranslateAccelerator : " + m.ToString());
                    }
                }
#endif // DEBUG

                bool needPreProcess = false;

                switch (lpmsg.message) {
                    case NativeMethods.WM_KEYDOWN:
                    case NativeMethods.WM_SYSKEYDOWN:
                    case NativeMethods.WM_CHAR:
                    case NativeMethods.WM_SYSCHAR:
                        needPreProcess = true;
                        break;
                }

                Message msg = Message.Create(lpmsg.hwnd, lpmsg.message, lpmsg.wParam, lpmsg.lParam);

                if (needPreProcess) {
                    Control target = Control.FromChildHandleInternal(lpmsg.hwnd);
                    if (target != null && (control == target || control.Contains(target))) {
                        PreProcessControlState messageState = PreProcessControlMessageInternal(target, ref msg);
                        switch (messageState) {
                            case PreProcessControlState.MessageProcessed:
                                // someone returned true from PreProcessMessage
                                // no need to dispatch the message, its already been coped with.
                                lpmsg.message = msg.Msg;
                                lpmsg.wParam = msg.WParam;
                                lpmsg.lParam = msg.LParam;
                                return NativeMethods.S_OK;
                            case PreProcessControlState.MessageNeeded:
                                // Here we need to dispatch the message ourselves
                                // otherwise the host may never send the key to our wndproc.
                                
                                // Someone returned true from IsInputKey or IsInputChar
                                UnsafeNativeMethods.TranslateMessage(ref lpmsg);
                                if (SafeNativeMethods.IsWindowUnicode(new HandleRef(null, lpmsg.hwnd))) {
                                    UnsafeNativeMethods.DispatchMessageW(ref lpmsg);
                                }
                                else {
                                    UnsafeNativeMethods.DispatchMessageA(ref lpmsg);
                                }
                                return NativeMethods.S_OK;
                            case PreProcessControlState.MessageNotNeeded:
                                // in this case we'll check the site to see if it wants the message.
                                break;
                        }
                    }
                }


                // SITE processing.  We're not interested in the message, but the site may be.
                
                int hr = NativeMethods.S_FALSE;

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource: Control did not process accelerator, handing to site");
    
                UnsafeNativeMethods.IOleControlSite ioleClientSite = clientSite as UnsafeNativeMethods.IOleControlSite;
                if (ioleClientSite != null) {
                    int keyState = 0;
    
                    if (UnsafeNativeMethods.GetKeyState(NativeMethods.VK_SHIFT) < 0)     keyState |= 1;
                    if (UnsafeNativeMethods.GetKeyState(NativeMethods.VK_CONTROL) < 0)   keyState |= 2;
                    if (UnsafeNativeMethods.GetKeyState(NativeMethods.VK_MENU) < 0)      keyState |= 4;
                    IntSecurity.UnmanagedCode.Assert();
                    try {
                        hr = ioleClientSite.TranslateAccelerator(ref lpmsg, keyState);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }
                
                return hr;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.UIDeactivate"]/*' />
            /// <devdoc>
            ///      Implements IOleInPlaceObject::UIDeactivate.
            /// </devdoc>
            internal int UIDeactivate() {
                // Only do this if we're UI active
                //
                if (!activeXState[uiActive]) {
                    return NativeMethods.S_OK;
                }

                activeXState[uiActive] = false;

                // Notify frame windows, if appropriate, that we're no longer ui-active.
                //
                if (inPlaceUiWindow != null) {
                    inPlaceUiWindow.SetActiveObject(null, null);
                }

                //May need this for SetActiveObject & OnUIDeactivate, so leave until function return
                IntSecurity.UnmanagedCode.Assert();
                Debug.Assert(inPlaceFrame != null, "No inplace frame -- how dod we go UI active?");
                inPlaceFrame.SetActiveObject(null, null);

                UnsafeNativeMethods.IOleInPlaceSite ioleClientSite = clientSite as UnsafeNativeMethods.IOleInPlaceSite;
                if (ioleClientSite != null) {
                    ioleClientSite.OnUIDeactivate(0);
                }

                return NativeMethods.S_OK;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ActiveXImpl.Unadvise"]/*' />
            /// <devdoc>
            ///      Implements IOleObject::Unadvise
            /// </devdoc>
            internal void Unadvise(int dwConnection) {
                if (dwConnection > adviseList.Count || adviseList[dwConnection - 1] == null) {
                    ThrowHr(NativeMethods.OLE_E_NOCONNECTION);
                }

                IAdviseSink sink = (IAdviseSink)adviseList[dwConnection - 1];
                adviseList.RemoveAt(dwConnection - 1);
                if (sink != null && UnsafeNativeMethods.IsComObject(sink)) {
                    UnsafeNativeMethods.ReleaseComObject(sink);
                }
            }

            /// <devdoc>
            ///     Notifies our site that we have changed our size and location.
            /// </devdoc>
            /// <internalonly/>
            internal void UpdateBounds(ref int x, ref int y, ref int width, ref int height, int flags) {
                if (!activeXState[adjustingRect] && activeXState[inPlaceVisible]) {
                    UnsafeNativeMethods.IOleInPlaceSite ioleClientSite = clientSite as UnsafeNativeMethods.IOleInPlaceSite;
                    if (ioleClientSite != null) {
                        NativeMethods.COMRECT rc = new NativeMethods.COMRECT();

                        if ((flags & NativeMethods.SWP_NOMOVE) != 0) {
                            rc.left = control.Left;
                            rc.top = control.Top;
                        }
                        else {
                            rc.left = x;
                            rc.top = y;
                        }

                        if ((flags & NativeMethods.SWP_NOSIZE) != 0) {
                            rc.right = rc.left + control.Width;
                            rc.bottom = rc.top + control.Height;
                        }
                        else {
                            rc.right = rc.left + width;
                            rc.bottom = rc.top + height;
                        }

                        // This member variable may be modified by SetObjectRects by the container.
                        adjustRect = rc;
                        activeXState[adjustingRect] = true;

                        IntSecurity.UnmanagedCode.Assert();
                        try {
                            ioleClientSite.OnPosRectChange(rc);
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                            adjustRect = null;
                            activeXState[adjustingRect] = false;
                        }

                        // On output, the new bounds will be reflected in  rc
                        if ((flags & NativeMethods.SWP_NOMOVE) == 0) {
                            x = rc.left;
                            y = rc.top;
                        }
                        if ((flags & NativeMethods.SWP_NOSIZE) == 0) {
                            width = rc.right - rc.left;
                            height = rc.bottom - rc.top;
                        }
                    }
                }
            }

            /// <devdoc>
            ///      Notifies that the accelerator table needs to be updated due to a change in a control mnemonic.
            /// </devdoc>
            [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")]
            internal void UpdateAccelTable(){
                // Setting the count to -1 will recreate the table on demand (when GetControlInfo is called).
                this.accelCount = -1;

                UnsafeNativeMethods.IOleControlSite ioleClientSite = clientSite as UnsafeNativeMethods.IOleControlSite;
                if (ioleClientSite != null) {
                    IntSecurity.UnmanagedCode.Assert();
                    ioleClientSite.OnControlInfoChanged();
                }
            }

            // Since this method is used by Reflection .. dont change the "signature"
            [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")]
            internal void ViewChangedInternal()
            {
                ViewChanged();
            }

            /// <devdoc>
            ///      Notifies our view advise sink (if it exists) that the view has
            ///      changed.
            /// </devdoc>
            /// <internalonly/>
            [SuppressMessage("Microsoft.Security", "CA2106:SecureAsserts")]
            private void ViewChanged() {
                // send the view change notification to anybody listening.
                //
                // Note: Word2000 won't resize components correctly if an OnViewChange notification
                //       is sent while the component is persisting it's state.  The !m_fSaving check
                //       is to make sure we don't call OnViewChange in this case.
                //
                if (viewAdviseSink != null && !activeXState[saving]) {
                    IntSecurity.UnmanagedCode.Assert();
                    try {
                        viewAdviseSink.OnViewChange(NativeMethods.DVASPECT_CONTENT, -1);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }

                    if (activeXState[viewAdviseOnlyOnce]) {
                        if (UnsafeNativeMethods.IsComObject(viewAdviseSink)) {
                            UnsafeNativeMethods.ReleaseComObject(viewAdviseSink);
                        }
                        viewAdviseSink = null;
                    }
                }
            }

            /// <devdoc>
            ///      Called when the window handle of the control has changed.
            /// </devdoc>
            void IWindowTarget.OnHandleChange(IntPtr newHandle) {
                controlWindowTarget.OnHandleChange(newHandle);
            }

            /// <devdoc>
            ///      Called to do control-specific processing for this window.
            /// </devdoc>
            void IWindowTarget.OnMessage(ref Message m) {
                if (activeXState[uiDead]) {
                    if (m.Msg >= NativeMethods.WM_MOUSEFIRST && m.Msg <= NativeMethods.WM_MOUSELAST) {
                        return;
                    }
                    if (m.Msg >= NativeMethods.WM_NCLBUTTONDOWN && m.Msg <= NativeMethods.WM_NCMBUTTONDBLCLK) {
                        return;
                    }
                    if (m.Msg >= NativeMethods.WM_KEYFIRST && m.Msg <= NativeMethods.WM_KEYLAST) {
                        return;
                    }
                }

                IntSecurity.UnmanagedCode.Assert();
                controlWindowTarget.OnMessage(ref m);
            }

            /// <devdoc>
            ///      This is a property bag implementation that sits on a stream.  It can
            ///      read and write the bag to the stream.
            /// </devdoc>
            private class PropertyBagStream : UnsafeNativeMethods.IPropertyBag {
                private Hashtable bag = new Hashtable();

                [SecurityPermission(SecurityAction.Assert, Flags=SecurityPermissionFlag.UnmanagedCode)]
                internal void Read(UnsafeNativeMethods.IStream istream) {
                    // visual basic's memory streams don't support seeking, so we have to
                    // work around this limitation here.  We do this by copying
                    // the contents of the stream into a MemoryStream object.
                    //
                    Stream stream = new DataStreamFromComStream(istream);
                    const int PAGE_SIZE = 0x1000; // one page (4096b)
                    byte[] streamData = new byte[PAGE_SIZE];
                    int offset = 0;

                    int count = stream.Read(streamData, offset, PAGE_SIZE);
                    int totalCount = count;

                    while (count == PAGE_SIZE) {
                        byte[] newChunk = new byte[streamData.Length + PAGE_SIZE];
                        Array.Copy(streamData, newChunk, streamData.Length);
                        streamData = newChunk;

                        offset += PAGE_SIZE;
                        count = stream.Read(streamData, offset, PAGE_SIZE);
                        totalCount += count;
                    }

                    stream = new MemoryStream(streamData);

                    BinaryFormatter formatter = new BinaryFormatter();
                    try {
                        bag = (Hashtable)formatter.Deserialize(stream);
                    }
                    catch (Exception e) {
                        if (ClientUtils.IsSecurityOrCriticalException(e)) {
                            throw;
                        }

                        // Error reading.  Just init an empty hashtable.
                        bag = new Hashtable();
                    }
                }

                [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
                int UnsafeNativeMethods.IPropertyBag.Read(string pszPropName, ref object pVar, UnsafeNativeMethods.IErrorLog pErrorLog)
                {
                    if (!bag.Contains(pszPropName))
                        return NativeMethods.E_INVALIDARG;

                    pVar = bag[pszPropName];
                    return NativeMethods.S_OK;
                }

                [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
                int UnsafeNativeMethods.IPropertyBag.Write(string pszPropName, ref object pVar)
                {
                    bag[pszPropName] = pVar;
                    return NativeMethods.S_OK;
                }

                [SecurityPermission(SecurityAction.Assert, Flags = SecurityPermissionFlag.UnmanagedCode)]
                internal void Write(UnsafeNativeMethods.IStream istream)
                {
                    Stream stream = new DataStreamFromComStream(istream);
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(stream, bag);
                }
            }
        }

        private class AxSourcingSite : ISite {
            private IComponent component;
            private UnsafeNativeMethods.IOleClientSite clientSite;
            private string name;
            private HtmlShimManager shimManager;


            internal AxSourcingSite(IComponent component, UnsafeNativeMethods.IOleClientSite clientSite, string name) {
                this.component = component;
                this.clientSite = clientSite;
                this.name = name;
            }

            /** The component sited by this component site. */
            public IComponent Component {
                get {
                    return component;
                }
            }

            /** The container in which the component is sited. */
            public IContainer Container {
                get {
                    return null;
                }
            }

            public Object GetService(Type service) {
                object retVal = null;

                if (service == typeof(HtmlDocument)) {

                    UnsafeNativeMethods.IOleContainer iOlecontainer;
                    int hr;
                    try {
                        // 



                        IntSecurity.UnmanagedCode.Assert();
                        hr = clientSite.GetContainer(out iOlecontainer);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }


                    if (NativeMethods.Succeeded(hr)
                            && (iOlecontainer is UnsafeNativeMethods.IHTMLDocument)) {
                        if (shimManager == null) {
                            shimManager = new HtmlShimManager();
                        }

                        // Security note: There is a demand for FullTrust on HtmlDocument that should apply
                        // to this call. So we don't need to demand here.
                        retVal = new HtmlDocument(shimManager, iOlecontainer as UnsafeNativeMethods.IHTMLDocument);
                    }

                }
                else if (clientSite.GetType().IsAssignableFrom(service)) {
                    IntSecurity.UnmanagedCode.Demand();
                    retVal = clientSite;
                }

                return retVal;
            }


            /** Indicates whether the component is in design mode. */
            public bool DesignMode {
                get {
                    return false;
                }
            }

            /**
             * The name of the component.
             */
            public String Name {
                get { return name;}
                set {
                    if (value == null || name == null) {
                        name = value;
                    }
                }
            }
        }

        /// <devdoc>
        ///     This is a marshaler object that knows how to marshal IFont to Font
        ///     and back.
        /// </devdoc>
        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        private class ActiveXFontMarshaler : ICustomMarshaler {

            private static ActiveXFontMarshaler instance;

            public void CleanUpManagedData(object obj) {
            }

            public void CleanUpNativeData(IntPtr pObj) {
                Marshal.Release(pObj);
            }

            internal static ICustomMarshaler GetInstance(string cookie) {
                if (instance == null) {
                    instance = new ActiveXFontMarshaler();
                }
                return instance;
            }
            public int GetNativeDataSize() {
                return -1; // not a value type, so use -1
            }

            public IntPtr MarshalManagedToNative(object obj) {
                Font font = (Font)obj;
                NativeMethods.tagFONTDESC fontDesc = new NativeMethods.tagFONTDESC();
                NativeMethods.LOGFONT logFont = new NativeMethods.LOGFONT();

                // 


                IntSecurity.ObjectFromWin32Handle.Assert();
                try {
                    font.ToLogFont(logFont);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                fontDesc.lpstrName = font.Name;
                fontDesc.cySize = (long)(font.SizeInPoints * 10000);
                fontDesc.sWeight = (short)logFont.lfWeight;
                fontDesc.sCharset = logFont.lfCharSet;
                fontDesc.fItalic = font.Italic;
                fontDesc.fUnderline = font.Underline;
                fontDesc.fStrikethrough = font.Strikeout;

                Guid iid = typeof(UnsafeNativeMethods.IFont).GUID;

                UnsafeNativeMethods.IFont oleFont = UnsafeNativeMethods.OleCreateFontIndirect(fontDesc, ref iid);
                IntPtr pFont = Marshal.GetIUnknownForObject(oleFont);
                IntPtr pIFont;

                int hr = Marshal.QueryInterface(pFont, ref iid, out pIFont);

                Marshal.Release(pFont);

                if (NativeMethods.Failed(hr)) {
                    Marshal.ThrowExceptionForHR(hr);
                }

                return pIFont;
            }

            public object MarshalNativeToManaged(IntPtr pObj) {
                UnsafeNativeMethods.IFont nativeFont = (UnsafeNativeMethods.IFont)Marshal.GetObjectForIUnknown(pObj);
                IntPtr hfont = IntPtr.Zero;

                IntSecurity.UnmanagedCode.Assert();
                try {
                    hfont = nativeFont.GetHFont();
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                Font font = null;

                IntSecurity.ObjectFromWin32Handle.Assert();
                try {
                    font = Font.FromHfont(hfont);
                }
                catch (Exception e) {
                    if (ClientUtils.IsSecurityOrCriticalException(e)) {
                        throw;
                    }

                    font = Control.DefaultFont;
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                return font;
            }
        }

        /// <devdoc>
        ///      Simple verb enumerator.
        /// </devdoc>
        private class ActiveXVerbEnum : UnsafeNativeMethods.IEnumOLEVERB {
            private NativeMethods.tagOLEVERB[] verbs;
            private int current;

            internal ActiveXVerbEnum(NativeMethods.tagOLEVERB[] verbs) {
                this.verbs = verbs;
                current = 0;
            }

            public int Next(int celt, NativeMethods.tagOLEVERB rgelt, int[] pceltFetched) {
                int fetched = 0;

                if (celt != 1) {
                    Debug.Fail("Caller of IEnumOLEVERB requested celt > 1, but clr marshalling does not support this.");
                    celt = 1;
                }

                while (celt > 0 && current < verbs.Length) {
                    rgelt.lVerb = verbs[current].lVerb;
                    rgelt.lpszVerbName = verbs[current].lpszVerbName;
                    rgelt.fuFlags = verbs[current].fuFlags;
                    rgelt.grfAttribs = verbs[current].grfAttribs;
                    celt--;
                    current++;
                    fetched++;
                }

                if (pceltFetched != null) {
                    pceltFetched[0] = fetched;
                }

#if DEBUG
                if (CompModSwitches.ActiveX.TraceInfo) {
                    Debug.WriteLine("AxSource:IEnumOLEVERB::Next returning " + fetched.ToString(CultureInfo.InvariantCulture) + " verbs:");
                    Debug.Indent();
                    for (int i = current - fetched; i < current; i++) {
                        Debug.WriteLine(i.ToString(CultureInfo.InvariantCulture) + ": " + verbs[i].lVerb + " " + (verbs[i].lpszVerbName == null ? string.Empty : verbs[i].lpszVerbName));
                    }
                    Debug.Unindent();
                }
#endif
                return(celt == 0 ? NativeMethods.S_OK : NativeMethods.S_FALSE);
            }

            public int Skip(int celt) {
                if (current + celt < verbs.Length) {
                    current += celt;
                    return NativeMethods.S_OK;
                }
                else {
                    current = verbs.Length;
                    return NativeMethods.S_FALSE;
                }
            }

            public void Reset() {
                current = 0;
            }

            public void Clone(out UnsafeNativeMethods.IEnumOLEVERB ppenum) {
                ppenum = new ActiveXVerbEnum(verbs);
            }
        }

        #if ACTIVEX_SOURCING

        //
        // This has been cut from the product.
        //

        /// <devdoc>
        ///     The properties window we display.
        /// </devdoc>
        private class ActiveXPropPage {
            private Form form;
            private PropertyGrid grid;
            private ComponentEditor compEditor;

            internal ActiveXPropPage(object component) {
                compEditor = (ComponentEditor)TypeDescriptor.GetEditor(component, typeof(ComponentEditor));

                if (compEditor == null) {
                    form = new Form();
                    grid = new PropertyGrid();

                    form.Text = SR.AXProperties;
                    form.StartPosition = FormStartPosition.CenterParent;
                    form.Size = new Size(300, 350);
                    form.FormBorderStyle = FormBorderStyle.Sizable;
                    form.MinimizeBox = false;
                    form.MaximizeBox = false;
                    form.ControlBox = true;
                    form.SizeGripStyle = SizeGripStyle.Show;
                    form.DockPadding.Bottom = 16; // size grip size

                    Bitmap bitmap = new Bitmap(grid.GetType(), "PropertyGrid.bmp");
                    bitmap.MakeTransparent();
                    form.Icon = Icon.FromHandle(bitmap.GetHicon());

                    grid.Dock = DockStyle.Fill;

                    form.Controls.Add(grid);
                }
            }

            internal void Edit(object editingObject) {
                if (compEditor != null) {
                    compEditor.EditComponent(null, editingObject);
                }
                else {
                    grid.SelectedObject = editingObject;
                    form.ShowDialog();
                }
            }
        }

        #endif

        /// <devdoc>
        ///      Contains a single ambient property, including DISPID, name and value.
        /// </devdoc>
        private class AmbientProperty {
            private string name;
            private int dispID;
            private Object value;
            private bool empty;

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AmbientProperty.AmbientProperty"]/*' />
            /// <devdoc>
            ///      Creates a new, empty ambient property.
            /// </devdoc>
            internal AmbientProperty(string name, int dispID) {
                this.name = name;
                this.dispID = dispID;
                this.value = null;
                this.empty = true;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AmbientProperty.Name"]/*' />
            /// <devdoc>
            ///      The windows forms property name.
            /// </devdoc>
            internal string Name {
                get {
                    return name;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AmbientProperty.DispID"]/*' />
            /// <devdoc>
            ///      The DispID for the property.
            /// </devdoc>
            internal int DispID {
                get {
                    return dispID;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AmbientProperty.Empty"]/*' />
            /// <devdoc>
            ///      Returns true if this property has not been set.
            /// </devdoc>
            internal bool Empty {
                get {
                    return empty;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AmbientProperty.Value"]/*' />
            /// <devdoc>
            ///      The current value of the property.
            /// </devdoc>
            internal Object Value {
                get {
                    return value;
                }
                set {
                    this.value = value;
                    empty = false;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.AmbientProperty.ResetValue"]/*' />
            /// <devdoc>
            ///      Resets the property.
            /// </devdoc>
            internal void ResetValue() {
                empty = true;
                value = null;
            }
        }

        /// <devdoc>
        ///     MetafileDCWrapper is used to wrap a metafile DC so that subsequent
        ///     paint operations are rendered to a temporary bitmap.  When the
        ///     wrapper is disposed, it copies the bitmap back to the metafile DC.
        ///
        ///     Example:
        ///
        ///     using(MetafileDCWrapper dcWrapper = new MetafileDCWrapper(hDC, size) {
        ///         // ...use dcWrapper.HDC to do painting
        ///     }
        ///
        /// </devdoc>
        private class MetafileDCWrapper : IDisposable {

            HandleRef hBitmapDC     = NativeMethods.NullHandleRef;
            HandleRef hBitmap       = NativeMethods.NullHandleRef;
            HandleRef hOriginalBmp  = NativeMethods.NullHandleRef;
            HandleRef hMetafileDC   = NativeMethods.NullHandleRef;
            NativeMethods.RECT destRect;

            [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")]
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Process | ResourceScope.Machine, ResourceScope.Process | ResourceScope.Machine)]
            internal MetafileDCWrapper(HandleRef hOriginalDC, Size size) {
                Debug.Assert(UnsafeNativeMethods.GetObjectType(hOriginalDC) == NativeMethods.OBJ_ENHMETADC,
                    "Why wrap a non-Enhanced MetaFile DC?");

                // Security fix: make sure the size has non-negative width and height.
                if (size.Width < 0 || size.Height < 0) {
                    throw new ArgumentException(nameof(size), SR.ControlMetaFileDCWrapperSizeInvalid);
                }

                hMetafileDC = hOriginalDC;
                destRect = new NativeMethods.RECT(0, 0, size.Width, size.Height);
                hBitmapDC = new HandleRef(this, UnsafeNativeMethods.CreateCompatibleDC(NativeMethods.NullHandleRef));

                int planes = UnsafeNativeMethods.GetDeviceCaps(hBitmapDC, NativeMethods.PLANES);
                int bitsPixel = UnsafeNativeMethods.GetDeviceCaps(hBitmapDC, NativeMethods.BITSPIXEL);
                hBitmap = new HandleRef(this, SafeNativeMethods.CreateBitmap(size.Width, size.Height, planes, bitsPixel, IntPtr.Zero));

                hOriginalBmp = new HandleRef(this, SafeNativeMethods.SelectObject(hBitmapDC, hBitmap));
            }

            ~MetafileDCWrapper() {
                ((IDisposable) this).Dispose();
            }

            void IDisposable.Dispose() {
                if (hBitmapDC.Handle == IntPtr.Zero || hMetafileDC.Handle == IntPtr.Zero || hBitmap.Handle == IntPtr.Zero) {
                    return;
                }

                bool success;

                try {
                    success = DICopy(hMetafileDC, hBitmapDC, destRect, true);
                    Debug.Assert(success, "DICopy() failed.");
                    SafeNativeMethods.SelectObject(hBitmapDC, hOriginalBmp);
                    success = SafeNativeMethods.DeleteObject(hBitmap);
                    Debug.Assert(success, "DeleteObject() failed.");
                    success = UnsafeNativeMethods.DeleteCompatibleDC(hBitmapDC);
                    Debug.Assert(success, "DeleteObject() failed.");
                } 
                finally {

                    // Dispose is done. Set all the handles to IntPtr.Zero so this way the Dispose method executes only once.
                    hBitmapDC = NativeMethods.NullHandleRef;
                    hBitmap = NativeMethods.NullHandleRef;
                    hOriginalBmp = NativeMethods.NullHandleRef;

                    GC.SuppressFinalize(this);
                }
            }

            internal IntPtr HDC {
                get { return hBitmapDC.Handle; }
            }

            // ported form VB6 (Ctls\PortUtil\StdCtl.cpp:6176)
            private unsafe bool DICopy(HandleRef hdcDest, HandleRef hdcSrc, NativeMethods.RECT rect, bool bStretch) {
                long i;

                bool fSuccess = false;  // Assume failure

                //
                // Get the bitmap from the DC by selecting in a 1x1 pixel temp bitmap
                HandleRef hNullBitmap = new HandleRef(this, SafeNativeMethods.CreateBitmap(1, 1, 1, 1, IntPtr.Zero));
                if (hNullBitmap.Handle == IntPtr.Zero) return fSuccess;

                try {
                    HandleRef hBitmap = new HandleRef(this, SafeNativeMethods.SelectObject(hdcSrc, hNullBitmap));
                    if (hBitmap.Handle == IntPtr.Zero) return fSuccess;

                    //
                    // Restore original bitmap
                    SafeNativeMethods.SelectObject(hdcSrc, hBitmap);


                    NativeMethods.BITMAP bmp = new NativeMethods.BITMAP();
                    if (UnsafeNativeMethods.GetObject(hBitmap, Marshal.SizeOf(bmp), bmp) == 0) {
                        return fSuccess;
                    }

                    NativeMethods.BITMAPINFO_FLAT lpbmi = new NativeMethods.BITMAPINFO_FLAT();
                    lpbmi.bmiHeader_biSize = Marshal.SizeOf(typeof(NativeMethods.BITMAPINFOHEADER));
                    lpbmi.bmiHeader_biWidth = bmp.bmWidth;
                    lpbmi.bmiHeader_biHeight = bmp.bmHeight;
                    lpbmi.bmiHeader_biPlanes = 1;
                    lpbmi.bmiHeader_biBitCount = bmp.bmBitsPixel;
                    lpbmi.bmiHeader_biCompression = NativeMethods.BI_RGB;
                    lpbmi.bmiHeader_biSizeImage = 0;               //Not needed since using BI_RGB
                    lpbmi.bmiHeader_biXPelsPerMeter = 0;
                    lpbmi.bmiHeader_biYPelsPerMeter = 0;
                    lpbmi.bmiHeader_biClrUsed = 0;
                    lpbmi.bmiHeader_biClrImportant = 0;
                    lpbmi.bmiColors = new byte[NativeMethods.BITMAPINFO_MAX_COLORSIZE*4];

                    //
                    // Include the palette for 256 color bitmaps
                    long iColors = 1 << (bmp.bmBitsPixel * bmp.bmPlanes);
                    if (iColors <= 256) {
                        byte[] aj = new byte[Marshal.SizeOf(typeof(NativeMethods.PALETTEENTRY)) * 256];
                        SafeNativeMethods.GetSystemPaletteEntries(hdcSrc, 0, (int)iColors, aj);

                        fixed (byte* pcolors = lpbmi.bmiColors) {
                            fixed (byte* ppal = aj) {
                                NativeMethods.RGBQUAD* prgb = (NativeMethods.RGBQUAD*)pcolors;
                                NativeMethods.PALETTEENTRY* lppe = (NativeMethods.PALETTEENTRY*)ppal;
                                //
                                // Convert the palette entries to RGB quad entries
                                for (i = 0; i < (int)iColors; i++) {
                                    prgb[i].rgbRed = lppe[i].peRed;
                                    prgb[i].rgbBlue = lppe[i].peBlue;
                                    prgb[i].rgbGreen = lppe[i].peGreen;
                                }
                            }
                        }
                    }

                    //
                    // Allocate memory to hold the bitmap bits
                    long bitsPerScanLine = bmp.bmBitsPixel * (long)bmp.bmWidth;
                    long bytesPerScanLine = (bitsPerScanLine + 7) / 8;
                    long totalBytesReqd = bytesPerScanLine * bmp.bmHeight;
                    byte[] lpBits = new byte[totalBytesReqd];

                    //
                    // Get the bitmap bits
                    int diRet = SafeNativeMethods.GetDIBits(hdcSrc, hBitmap, 0, bmp.bmHeight, lpBits,
                            ref lpbmi, NativeMethods.DIB_RGB_COLORS);
                    if (diRet == 0) return fSuccess;

                    //
                    // Set the destination coordiates depending on whether stretch-to-fit was chosen
                    int xDest, yDest, cxDest, cyDest;
                    if (bStretch)
                    {
                        xDest = rect.left;
                        yDest = rect.top;
                        cxDest = rect.right - rect.left;
                        cyDest = rect.bottom - rect.top;
                    }
                    else
                    {
                        xDest = rect.left;
                        yDest = rect.top;
                        cxDest = bmp.bmWidth;
                        cyDest = bmp.bmHeight;
                    }

                    // Paint the bitmap
                    int iRet = SafeNativeMethods.StretchDIBits(hdcDest,
                            xDest, yDest, cxDest, cyDest, 0, 0, bmp.bmWidth, bmp.bmHeight,
                            lpBits, ref lpbmi, NativeMethods.DIB_RGB_COLORS, NativeMethods.SRCCOPY);
                    if (iRet == NativeMethods.GDI_ERROR) return fSuccess;

                    fSuccess = true;
                }
                finally {
                    SafeNativeMethods.DeleteObject(hNullBitmap);
                }
                return fSuccess;
            }
        }


        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject"]/*' />
        /// <devdoc>
        ///      An implementation of AccessibleChild for use with Controls
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]
        public class ControlAccessibleObject : AccessibleObject {

            private static IntPtr oleAccAvailable = NativeMethods.InvalidIntPtr;

            // Member variables

            private IntPtr handle = IntPtr.Zero; // Associated window handle (if any)
            private Control ownerControl = null; // The associated Control for this AccessibleChild (if any)
            private int[] runtimeId = null; // Used by UIAutomation

            // constructors

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.ControlAccessibleObject"]/*' />
            public ControlAccessibleObject(Control ownerControl) {

                Debug.Assert(ownerControl != null, "Cannot construct a ControlAccessibleObject with a null ownerControl");
                if (ownerControl == null) {
                    throw new ArgumentNullException(nameof(ownerControl));
                }

                this.ownerControl = ownerControl;

                // call get_Handle outside the UnmanagedCode permission because if the handle is not created yet,
                // we will invoke 3rd party HandleCreated event handlers
                IntPtr handle = ownerControl.Handle;

                // Reviewed  : ControlAccessibleObject.set_Handle demands UnmanagedCode permission for public use, it doesn't
                //             expose any security vulnerability indirectly. The sec Assert is safe.
                //
                IntSecurity.UnmanagedCode.Assert();
                try {
                    this.Handle = handle;
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.ControlAccessibleObject1"]/*' />
            internal ControlAccessibleObject(Control ownerControl, int accObjId) {

                Debug.Assert(ownerControl != null, "Cannot construct a ControlAccessibleObject with a null ownerControl");
                if (ownerControl == null) {
                    throw new ArgumentNullException(nameof(ownerControl));
                }

                this.AccessibleObjectId = accObjId; // ...must set this *before* setting the Handle property

                this.ownerControl = ownerControl;
                IntPtr handle = ownerControl.Handle;

                // Reviewed  : ControlAccessibleObject.set_Handle demands UnmanagedCode permission for public use, it doesn't
                //             expose any security vulnerability indirectly. The sec Assert is safe.
                //
                IntSecurity.UnmanagedCode.Assert();
                try {
                    this.Handle = handle;
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }

            /// <devdoc>
            ///     For container controls only, return array of child controls sorted into
            ///     tab index order. This gets applies to the list of child accessible objects
            ///     as returned by the system, so that we can present a meaningful order to
            ///     the user. The system defaults to z-order, which is bad for us because
            ///     that is usually the reverse of tab order!
            /// </devdoc>
            internal override int[] GetSysChildOrder() {
                if (ownerControl.GetStyle(ControlStyles.ContainerControl))
                    return ownerControl.GetChildWindowsInTabOrder();

                return base.GetSysChildOrder();
            }

            /// <devdoc>
            ///     Perform custom navigation between parent/child/sibling accessible objects,
            ///     using tab index order as the guide, rather than letting the system default
            ///     behavior do navigation based on z-order.
            ///
            ///     For a container control and its child controls, the accessible object tree
            ///     looks like this...
            ///
            ///         [client area of container]
            ///             [non-client area of child #1]
            ///                 [random non-client elements]
            ///                 [client area of child #1]
            ///                 [random non-client elements]
            ///             [non-client area of child #2]
            ///                 [random non-client elements]
            ///                 [client area of child #2]
            ///                 [random non-client elements]
            ///             [non-client area of child #3]
            ///                 [random non-client elements]
            ///                 [client area of child #3]
            ///                 [random non-client elements]
            ///
            ///     We need to intercept first-child / last-child navigation from the container's
            ///     client object, and next-sibling / previous-sibling navigation from each child's
            ///     non-client object. All other navigation operations must be allowed to fall back
            ///     on the system's deafult behavior (provided by OLEACC.DLL).
            ///
            ///     When combined with the re-ordering behavior of GetSysChildOrder() above, this
            ///     allows us to present the end user with the illusion of accessible objects in
            ///     tab index order, even though the system behavior only supports z-order.
            /// </devdoc>
            internal override bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject accessibleObject) {

                // Clear the out parameter
                accessibleObject = null;

                // Get the owning control's parent, if it has one
                Control parentControl = ownerControl.ParentInternal;                

                // ctrls[index] will indicate the control at the destination of this navigation operation
                int index = -1;
                Control[] ctrls = null;

                // Now handle any 'appropriate' navigation requests...
                switch (navdir) {

                    case AccessibleNavigation.FirstChild:
                        if (IsClientObject) {
                            ctrls = ownerControl.GetChildControlsInTabOrder(true);
                            index = 0;
                        }
                        break;

                    case AccessibleNavigation.LastChild:
                        if (IsClientObject) {
                            ctrls = ownerControl.GetChildControlsInTabOrder(true);
                            index = ctrls.Length - 1;
                        }
                        break;

                    case AccessibleNavigation.Previous:
                        if (IsNonClientObject && parentControl != null) {
                            ctrls = parentControl.GetChildControlsInTabOrder(true);
                            index = Array.IndexOf(ctrls, ownerControl);
                            if (index != -1)
                                --index;
                        }
                        break;

                    case AccessibleNavigation.Next:
                        if (IsNonClientObject && parentControl != null) {
                            ctrls = parentControl.GetChildControlsInTabOrder(true);
                            index = Array.IndexOf(ctrls, ownerControl);
                            if (index != -1)
                                ++index;
                        }
                        break;
                }

                // Unsupported navigation operation for this object, or unexpected error.
                // Return false to force fall back on default system navigation behavior.
                if (ctrls == null || ctrls.Length == 0)
                    return false;

                // If ctrls[index] is a valid control, return its non-client accessible object.
                // If index is invalid, return null pointer meaning "end of list reached".
                if (index >= 0 && index < ctrls.Length)
                    accessibleObject = ctrls[index].NcAccessibilityObject;

                // Return true to use the found accessible object and block default system behavior
                return true;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction {
                get {
                    string defaultAction = ownerControl.AccessibleDefaultActionDescription;
                    if (defaultAction != null) {
                        return defaultAction;
                    }
                    else {
                        return base.DefaultAction;
                    }
                }
            }

            // This is used only if control supports IAccessibleEx
            internal override int[] RuntimeId {
                get {
                    if (runtimeId == null) {
                        runtimeId = new int[2];
                        runtimeId[0] = 0x2a;
                        runtimeId[1] = (int)(long)this.Handle;
                    }

                    return runtimeId;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.Description"]/*' />
            public override string Description {
                get {
                    string description = ownerControl.AccessibleDescription;
                    if (description != null) {
                        return description;
                    }
                    else {
                        return base.Description;
                    }
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.Handle"]/*' />
            public IntPtr Handle {

                get {
                    return handle;
                }

                set {
                    // Demand security permission for this!
                    IntSecurity.UnmanagedCode.Demand();

                    if (handle != value) {
                        handle = value;

                        if (oleAccAvailable == IntPtr.Zero) {
                            return;
                        }

                        bool freeLib = false;

                        if (oleAccAvailable == NativeMethods.InvalidIntPtr) {
                            oleAccAvailable = UnsafeNativeMethods.LoadLibraryFromSystemPathIfAvailable("oleacc.dll");
                            freeLib = (oleAccAvailable != IntPtr.Zero);
                        }

                        // Update systemIAccessible
                        // We need to store internally the system provided
                        // IAccessible, because some windows forms controls use it
                        // as the default IAccessible implementation.
                        if (handle != IntPtr.Zero && oleAccAvailable != IntPtr.Zero) {
                            UseStdAccessibleObjects(handle);
                        }

                        if (freeLib) {
                            UnsafeNativeMethods.FreeLibrary(new HandleRef(null, oleAccAvailable));
                        }

                    }
                } // end Handle.Set

            } // end Handle

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.Help"]/*' />
            public override string Help {
                get {
                    QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Owner.Events[EventQueryAccessibilityHelp];

                    if (handler != null) {
                        QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                        handler(Owner, args);
                        return args.HelpString;
                    }
                    else {
                        return base.Help;
                    }
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="ControlAccessibleObject.KeyboardShortcut"]/*' />
            public override string KeyboardShortcut {
                get {
                    // For controls, the default keyboard shortcut comes directly from the accessible
                    // name property. This matches the default behavior of OLEACC.DLL exactly.
                    char mnemonic = WindowsFormsUtils.GetMnemonic(this.TextLabel, false);
                    return (mnemonic == (char) 0) ? null : ("Alt+" + mnemonic);
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.Name"]/*' />
            public override string Name {
                get {
                    // Special case: If an explicit name has been set in the AccessibleName property, use that.
                    // Note: Any non-null value in AccessibleName overrides the default accessible name logic,
                    // even an empty string (this is the only way to *force* the accessible name to be blank).
                    string name = ownerControl.AccessibleName;
                    if (name != null) {
                        return name;
                    }

                    // Otherwise just return the default label string, minus any mnemonics
                    return WindowsFormsUtils.TextWithoutMnemonics(TextLabel);
                }

                set {
                    // If anyone tries to set the accessible name, just cache the value in the control's
                    // AccessibleName property. This value will then end up overriding the normal accessible
                    // name logic, until such time as AccessibleName is set back to null.
                    ownerControl.AccessibleName = value;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="ControlAccessibleObject.Parent"]/*' />
            public override AccessibleObject Parent {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                [SecurityPermission(SecurityAction.InheritanceDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get {
                    return base.Parent;
                }
            }

            // Determine the text that should be used to 'label' this control for accessibility purposes.
            //
            // Prior to Whidbey, we just called 'base.Name' to determine the accessible name. This would end up calling
            // OLEACC.DLL to determine the name. The rules used by OLEACC.DLL are the same as what we now have below,
            // except that OLEACC searches for preceding labels using z-order, and we want to search for labels using
            // TabIndex order.
            //
            internal string TextLabel {
                get {
                    // If owning control allows, use its Text property - but only if that Text is not empty
                    if (ownerControl.GetStyle(ControlStyles.UseTextForAccessibility)) {
                        string text = ownerControl.Text;
                        if (!string.IsNullOrEmpty(text)) {
                            return text;
                        }
                    }

                    // Otherwise use the text of the preceding Label control, if there is one
                    Label previousLabel = PreviousLabel;
                    if (previousLabel != null) {
                        string text = previousLabel.Text;
                        if (!string.IsNullOrEmpty(text)) {
                            return text;
                        }
                    }

                    // This control has no discernable MSAA name - return an empty string to indiciate 'nameless'.
                    return null;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.Owner"]/*' />
            public Control Owner {
                get {
                    return ownerControl;
                }
            }

            // Look for a label immediately preceeding this control in
            // the tab order, and use its name for the accessible name.
            //
            // This method aims to emulate the equivalent behavior in OLEACC.DLL,
            // but walking controls in TabIndex order rather than native z-order.
            //
            internal Label PreviousLabel {
                get {
                    // Try to get to the parent of this control.
                    Control parent = Owner.ParentInternal;                    

                    if (parent == null) {
                        return null;
                    }

                    // Find this control's containing control
                    ContainerControl container = parent.GetContainerControlInternal() as ContainerControl;
                    if (container == null) {
                        return null;
                    }

                    // Walk backwards through peer controls...
                    for (Control previous = container.GetNextControl(Owner, false);
                         previous != null;
                         previous = container.GetNextControl(previous, false)) {

                        // Stop when we hit a Label (whether visible or invisible)
                        if (previous is Label) {
                            return previous as Label;
                        }

                        // Stop at any *visible* tab stop
                        if (previous.Visible && previous.TabStop) {
                            break;
                        }
                    }

                    return null;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.Role"]/*' />
            public override AccessibleRole Role {
                get {
                    AccessibleRole role = ownerControl.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    else {
                        return base.Role;
                    }

                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.GetHelpTopic"]/*' />
            public override int GetHelpTopic(out string fileName) {
                int topic = 0;

                QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)Owner.Events[EventQueryAccessibilityHelp];

                if (handler != null) {
                    QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                    handler(Owner, args);

                    fileName = args.HelpNamespace;
                    if (!String.IsNullOrEmpty(fileName)) {
                        IntSecurity.DemandFileIO(FileIOPermissionAccess.PathDiscovery, fileName);
                    }

                    try {
                        topic = Int32.Parse(args.HelpKeyword, CultureInfo.InvariantCulture);
                    }
                    catch (Exception e) {
                        if (ClientUtils.IsSecurityOrCriticalException(e)) {
                            throw;
                        }
                    }

                    return topic;
                }
                else {
                    return base.GetHelpTopic(out fileName);
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.NotifyClients"]/*' />
            public void NotifyClients(AccessibleEvents accEvent) {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "Control.NotifyClients: this = " +
                                  this.ToString() + ", accEvent = " + accEvent.ToString() + ", childID = self");

                UnsafeNativeMethods.NotifyWinEvent((int)accEvent, new HandleRef(this, Handle), NativeMethods.OBJID_CLIENT, 0);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.NotifyClients1"]/*' />
            public void NotifyClients(AccessibleEvents accEvent, int childID) {

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "Control.NotifyClients: this = " +
                                  this.ToString() + 
                                  ", accEvent = " + accEvent.ToString() + 
                                  ", childID = " + childID.ToString(CultureInfo.InvariantCulture));

                UnsafeNativeMethods.NotifyWinEvent((int)accEvent, new HandleRef(this, Handle), NativeMethods.OBJID_CLIENT, childID + 1);
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.NotifyClients2"]/*' />
            public void NotifyClients(AccessibleEvents accEvent, int objectID, int childID) {

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "Control.NotifyClients: this = " +
                                  this.ToString() + 
                                  ", accEvent = " + accEvent.ToString() + 
                                  ", childID = " + childID.ToString(CultureInfo.InvariantCulture));

                UnsafeNativeMethods.NotifyWinEvent((int)accEvent, new HandleRef(this, Handle), objectID, childID + 1);
            }

            /// <summary>
            /// Raises the LiveRegionChanged UIA event.
            /// To make this method effective, the control must implement System.Windows.Forms.Automation.IAutomationLiveRegion interface
            /// and its LiveSetting property must return either AutomationLiveSetting.Polite or AutomationLiveSetting.Assertive value.
            /// In addition, the applications must be recompiled to target .NET Framework 4.7.3 or opt in into this feature using compatibility switches.
            /// </summary>
            /// <returns>True if operation succeeds, False otherwise.</returns>
            public override bool RaiseLiveRegionChanged() {
                if (!(this.Owner is IAutomationLiveRegion)) {
                    throw new InvalidOperationException(SR.OwnerControlIsNotALiveRegion);
                }

                return RaiseAutomationEvent(NativeMethods.UIA_LiveRegionChangedEventId);
            }

            internal override bool IsIAccessibleExSupported() {
                if (AccessibilityImprovements.Level3 && this.Owner is IAutomationLiveRegion) {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override object GetPropertyValue(int propertyID) {
                if (AccessibilityImprovements.Level3 && propertyID == NativeMethods.UIA_LiveSettingPropertyId && Owner is IAutomationLiveRegion) {
                    return ((IAutomationLiveRegion)Owner).LiveSetting;
                }

                if (Owner.SupportsUiaProviders) {
                    switch (propertyID) {
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                            return Owner.CanSelect;
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                        case NativeMethods.UIA_IsPasswordPropertyId:
                            return false;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return string.Empty;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            var help = Help;
                            return AccessibilityImprovements.Level3 ? (help ?? string.Empty) : help;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override UnsafeNativeMethods.IRawElementProviderSimple HostRawElementProvider {
                get {
                    if (AccessibilityImprovements.Level3) {
                        UnsafeNativeMethods.IRawElementProviderSimple provider;
                        UnsafeNativeMethods.UiaHostProviderFromHwnd(new HandleRef(this, Handle), out provider);
                        return provider;
                    }

                    return base.HostRawElementProvider;
                }
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlAccessibleObject.ToString"]/*' />
            public override string ToString() {
                if (Owner != null) {
                    return "ControlAccessibleObject: Owner = " + Owner.ToString();
                }
                else {
                    return "ControlAccessibleObject: Owner = null";
                }
            }
        }


        // Fonts can be a pain to track, so we wrap Hfonts in this class to get a Finalize method.
        // 
        internal sealed class FontHandleWrapper : MarshalByRefObject, IDisposable {
#if DEBUG
            private string stackOnCreate = null;
            private string stackOnDispose = null;
            private bool finalizing = false;
#endif
            private IntPtr handle;

            internal FontHandleWrapper(Font font) {
#if DEBUG
                if (CompModSwitches.LifetimeTracing.Enabled) stackOnCreate = new System.Diagnostics.StackTrace().ToString();
#endif
                handle = font.ToHfont();
                System.Internal.HandleCollector.Add(handle, NativeMethods.CommonHandles.GDI);
            }

            internal IntPtr Handle {
                get {
                    Debug.Assert(handle != IntPtr.Zero, "FontHandleWrapper disposed, but still being accessed");
                    return handle;
                }
            }

            public void Dispose() {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing) {

#if DEBUG
                Debug.Assert(finalizing || this != defaultFontHandleWrapper, "Don't dispose the defaultFontHandleWrapper");
#endif

                if (handle != IntPtr.Zero) {
#if DEBUG
                    if (CompModSwitches.LifetimeTracing.Enabled) stackOnDispose = new System.Diagnostics.StackTrace().ToString();
#endif
                    SafeNativeMethods.DeleteObject(new HandleRef(this, handle));
                    handle = IntPtr.Zero;
                }

            }

            ~FontHandleWrapper() {
#if DEBUG
                finalizing = true;
#endif
                Dispose(false);
            }
        }

        /// <devdoc>
        ///     Used with BeginInvoke/EndInvoke
        /// </devdoc>
        private class ThreadMethodEntry : IAsyncResult {
            internal Control   caller;
            internal Control   marshaler;
            internal Delegate  method;
            internal Object[] args;
            internal Object    retVal;
            internal Exception exception;
            internal bool   synchronous;
            private bool isCompleted;
            private ManualResetEvent resetEvent;
            private object invokeSyncObject = new object();

            // Store the execution context associated with the caller thread, and
            // information about which thread actually got the stack applied to it.
            //
            internal ExecutionContext executionContext;

            // Optionally store the synchronization context associated with the callee thread.
            // This overrides the sync context in the execution context of the caller thread.
            //
            internal SynchronizationContext syncContext = null;

            internal ThreadMethodEntry(Control caller, Control marshaler, Delegate method, Object[] args, bool synchronous, ExecutionContext executionContext) {
                this.caller = caller;
                this.marshaler = marshaler;
                this.method = method;
                this.args = args;
                this.exception = null;
                this.retVal = null;
                this.synchronous = synchronous;
                this.isCompleted = false;
                this.resetEvent = null;
                this.executionContext = executionContext;
            }

            ~ThreadMethodEntry()
            {
                if (this.resetEvent != null)
                {
                    this.resetEvent.Close();
                }
            }

            public  Object AsyncState {
                get {
                    return null;
                }
            }

            public WaitHandle AsyncWaitHandle {
                get {
                    if (this.resetEvent == null) {
                        // Locking 'this' here is ok since this is an internal class.
                        lock (invokeSyncObject) {
                            // BeginInvoke hangs on Multi-proc system:
                            // taking the lock prevents a race condition between IsCompleted
                            // boolean flag and resetEvent mutex in multiproc scenarios.
                             if (this.resetEvent == null) {
                                this.resetEvent = new ManualResetEvent(false);
                                if (this.isCompleted) {
                                    this.resetEvent.Set();
                                }
                             }
                        }
                    }
                    return(WaitHandle)this.resetEvent;
                }
            }

            public bool CompletedSynchronously {
                get {
                    if (this.isCompleted && this.synchronous)
                        return true;

                    return false;
                }
            }

            public bool IsCompleted {
                get {
                    return this.isCompleted;
                }
            }

            internal void Complete() {
                lock (invokeSyncObject) {
                    this.isCompleted = true;
                    if (this.resetEvent != null) {
                        this.resetEvent.Set();
                    }
                }
            }
        }

        private class ControlVersionInfo {
            private string companyName = null;
            private string productName = null;
            private string productVersion = null;
            private FileVersionInfo versionInfo = null;
            private Control owner;

            internal ControlVersionInfo(Control owner) {
                this.owner = owner;
            }

            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlVersionInfo.CompanyName"]/*' />
            /// <devdoc>
            ///     The company name associated with the component.
            /// </devdoc>
            internal string CompanyName {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    if (companyName == null) {
                        object[] attrs = owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                        if (attrs != null && attrs.Length > 0) {
                            companyName = ((AssemblyCompanyAttribute)attrs[0]).Company;
                        }

                        if (companyName == null || companyName.Length == 0) {
                            companyName = GetFileVersionInfo().CompanyName;
                            if (companyName != null) {
                                companyName = companyName.Trim();
                            }
                        }

                        if (companyName == null || companyName.Length == 0) {
                            string ns = owner.GetType().Namespace;

                            if (ns == null) {
                                ns = String.Empty;
                            }

                            int firstDot = ns.IndexOf("/");
                            if (firstDot != -1) {
                                companyName = ns.Substring(0, firstDot);
                            }
                            else {
                                companyName = ns;
                            }
                        }
                    }
                    return companyName;
                }
            }


            /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ControlVersionInfo.ProductName"]/*' />
            /// <devdoc>
            ///     The product name associated with this component.
            /// </devdoc>
            internal string ProductName {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    if (productName == null) {
                        object[] attrs = owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                        if (attrs != null && attrs.Length > 0) {
                            productName = ((AssemblyProductAttribute)attrs[0]).Product;
                        }

                        if (productName == null || productName.Length == 0) {
                            productName = GetFileVersionInfo().ProductName;
                            if (productName != null) {
                                productName = productName.Trim();
                            }
                        }

                        if (productName == null || productName.Length == 0) {
                            string ns = owner.GetType().Namespace;

                            if (ns == null) {
                                ns = String.Empty;
                            }
                            int firstDot = ns.IndexOf(".");
                            if (firstDot != -1) {
                                productName = ns.Substring(firstDot+1);
                            }
                            else {
                                productName = ns;
                            }
                        }
                    }

                    return productName;
                }
            }


            /// <devdoc>
            ///     The product version associated with this component.
            /// </devdoc>
            internal string ProductVersion {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    if (productVersion == null) {
                        // custom attribute
                        //
                        object[] attrs = owner.GetType().Module.Assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
                        if (attrs != null && attrs.Length > 0) {
                            productVersion = ((AssemblyInformationalVersionAttribute)attrs[0]).InformationalVersion;
                        }

                        // win32 version info
                        //
                        if (productVersion == null || productVersion.Length == 0) {
                            productVersion = GetFileVersionInfo().ProductVersion;
                            if (productVersion != null) {
                                productVersion = productVersion.Trim();
                            }
                        }

                        // fake it
                        //
                        if (productVersion == null || productVersion.Length == 0) {
                            productVersion = "1.0.0.0";
                        }
                    }
                    return productVersion;
                }
            }


            /// <devdoc>
            ///     Retrieves the FileVersionInfo associated with the main module for
            ///     the component.
            /// </devdoc>
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            private FileVersionInfo GetFileVersionInfo() {
                if (versionInfo == null) {
                    string path;

                    // 

                    FileIOPermission fiop = new FileIOPermission( PermissionState.None );
                    fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
                    fiop.Assert();
                    try {
                        path = owner.GetType().Module.FullyQualifiedName;
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }

                    // 

                    new FileIOPermission(FileIOPermissionAccess.Read, path).Assert();
                    try {
                        versionInfo = FileVersionInfo.GetVersionInfo(path);
                    }
                    finally {
                        CodeAccessPermission.RevertAssert();
                    }
                }

                return versionInfo;

            }

        }

        private sealed class MultithreadSafeCallScope : IDisposable
        {
            // Use local stack variable rather than a refcount since we're
            // guaranteed that these 'scopes' are properly nested.
            private bool resultedInSet;

            internal MultithreadSafeCallScope()
            {
                // Only access the thread-local stuff if we're going to be
                // checking for illegal thread calling (no need to incur the
                // expense otherwise).
                if (checkForIllegalCrossThreadCalls && !inCrossThreadSafeCall)
                {
                    inCrossThreadSafeCall = true;
                    resultedInSet = true;
                }
                else
                {
                    resultedInSet = false;
                }
            }

            void IDisposable.Dispose()
            {
                if (resultedInSet)
                {
                    inCrossThreadSafeCall = false;
                }
            }
        }

        private sealed class PrintPaintEventArgs : PaintEventArgs {
            Message m;

            internal PrintPaintEventArgs(Message m, IntPtr dc, Rectangle clipRect)
                : base(dc, clipRect) {
                this.m = m;
            }

            internal Message Message {
                get {
                    return m;
                }
            }
        }

    } // end class Control

} // end namespace System.Windows.Forms





