// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// #define DEBUG_PREFERREDSIZE

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Internal;
using System.Windows.Forms.Layout;
using Accessibility;
using Microsoft.Win32;
using static Interop;
using Encoding = System.Text.Encoding;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Defines the base class for controls, which are components with visual representation.
    /// </summary>
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
        Ole32.IPersistStreamInit,
        UnsafeNativeMethods.IPersistPropertyBag,
        Ole32.IPersistStorage,
        UnsafeNativeMethods.IQuickActivate,
        ISupportOleDropSource,
        IDropTarget,
        ISynchronizeInvoke,
        IWin32Window,
        IArrangedElement,
        IBindableComponent,
        IKeyboardToolTip,
        IHandle
    {
#if FINALIZATION_WATCH
        static readonly TraceSwitch ControlFinalization = new TraceSwitch("ControlFinalization", "Tracks the creation and destruction of finalization");
        internal static string GetAllocationStack() {
            if (ControlFinalization.TraceVerbose) {
            //  the operation is safe (data obtained from the CLR). This code is for debugging purposes only.
                return Environment.StackTrace;
            }
            else {
                return "Enable 'ControlFinalization' switch to see stack of allocation";
            }

        }
        private string allocationSite = Control.GetAllocationStack();
#endif

#if DEBUG
        internal static readonly TraceSwitch s_paletteTracing = new TraceSwitch("PaletteTracing", "Debug Palette code");
        internal static readonly TraceSwitch s_controlKeyboardRouting = new TraceSwitch("ControlKeyboardRouting", "Debug Keyboard routing for controls");
        private protected static readonly TraceSwitch s_focusTracing = new TraceSwitch("FocusTracing", "Debug focus/active control/enter/leave");
        private static readonly BooleanSwitch s_assertOnControlCreateSwitch = new BooleanSwitch("AssertOnControlCreate", "Assert when anything directly deriving from control is created.");
        private protected static readonly BooleanSwitch s_traceMnemonicProcessing = new BooleanSwitch("TraceCanProcessMnemonic", "Trace mnemonic processing calls to assure right child-parent call ordering.");

        private protected void TraceCanProcessMnemonic()
        {
            if (s_traceMnemonicProcessing.Enabled)
            {
                string str;
                try
                {
                    str = string.Format(CultureInfo.CurrentCulture, "{0}<{1}>", GetType().Name, Text);
                    int maxFrameCount = new StackTrace().FrameCount;
                    if (maxFrameCount > 5)
                    {
                        maxFrameCount = 5;
                    }
                    int frameIndex = 1;
                    while (frameIndex < maxFrameCount)
                    {
                        StackFrame sf = new StackFrame(frameIndex);
                        if (frameIndex == 2 && sf.GetMethod().Name.Equals("CanProcessMnemonic"))
                        {
                            // log immediate call if in a virtual/recursive call.
                            break;
                        }
                        str += new StackTrace(sf).ToString().TrimEnd();
                        frameIndex++;
                    }
                    if (frameIndex > 2)
                    {
                        // new CanProcessMnemonic virtual/recursive call stack.
                        str = "\r\n" + str;
                    }
                }
                catch (Exception ex)
                {
                    str = ex.ToString();
                }
                Debug.WriteLine(str);
            }
        }
#else
        internal static readonly TraceSwitch s_paletteTracing;
        internal static readonly TraceSwitch s_controlKeyboardRouting;
        private protected readonly TraceSwitch s_focusTracing;
#endif

#if DEBUG
        private static readonly BooleanSwitch s_bufferPinkRect = new BooleanSwitch("BufferPinkRect", "Renders a pink rectangle with painting double buffered controls");
        private static readonly BooleanSwitch s_bufferDisabled = new BooleanSwitch("BufferDisabled", "Makes double buffered controls non-double buffered");
#endif

        private static readonly int WM_GETCONTROLNAME = SafeNativeMethods.RegisterWindowMessage("WM_GETCONTROLNAME");
        private static readonly int WM_GETCONTROLTYPE = SafeNativeMethods.RegisterWindowMessage("WM_GETCONTROLTYPE");

        internal const int STATE_CREATED = 0x00000001;
        internal const int STATE_VISIBLE = 0x00000002;
        internal const int STATE_ENABLED = 0x00000004;
        internal const int STATE_TABSTOP = 0x00000008;
        internal const int STATE_RECREATE = 0x00000010;
        internal const int STATE_MODAL = 0x00000020;
        internal const int STATE_ALLOWDROP = 0x00000040;
        internal const int STATE_DROPTARGET = 0x00000080;
        internal const int STATE_NOZORDER = 0x00000100;
        internal const int STATE_LAYOUTDEFERRED = 0x00000200;
        internal const int STATE_USEWAITCURSOR = 0x00000400;
        internal const int STATE_DISPOSED = 0x00000800;
        internal const int STATE_DISPOSING = 0x00001000;
        internal const int STATE_MOUSEENTERPENDING = 0x00002000;
        internal const int STATE_TRACKINGMOUSEEVENT = 0x00004000;
        internal const int STATE_THREADMARSHALLPENDING = 0x00008000;
        internal const int STATE_SIZELOCKEDBYOS = 0x00010000;
        internal const int STATE_CAUSESVALIDATION = 0x00020000;
        internal const int STATE_CREATINGHANDLE = 0x00040000;
        internal const int STATE_TOPLEVEL = 0x00080000;
        internal const int STATE_ISACCESSIBLE = 0x00100000;
        internal const int STATE_OWNCTLBRUSH = 0x00200000;
        internal const int STATE_EXCEPTIONWHILEPAINTING = 0x00400000;
        internal const int STATE_LAYOUTISDIRTY = 0x00800000;
        internal const int STATE_CHECKEDHOST = 0x01000000;
        internal const int STATE_HOSTEDINDIALOG = 0x02000000;
        internal const int STATE_DOUBLECLICKFIRED = 0x04000000;
        internal const int STATE_MOUSEPRESSED = 0x08000000;
        internal const int STATE_VALIDATIONCANCELLED = 0x10000000;
        internal const int STATE_PARENTRECREATING = 0x20000000;
        internal const int STATE_MIRRORED = 0x40000000;

        // When we change RightToLeft, we need to change the scrollbar thumb.
        // We can't do that until after the control has been created, and all the items added
        // back. This is because the system control won't know the nMin and nMax of the scroll
        // bar until the items are added. So in RightToLeftChanged, we set a flag that indicates
        // that we want to set the scroll position. In OnHandleCreated we check this flag,
        // and if set, we BeginInvoke. We have to BeginInvoke since we have to wait until the items
        // are added. We only want to do this when RightToLeft changes thus the flags
        // STATE2_HAVEINVOKED and STATE2_SETSCROLLPOS. Otherwise we would do this on each HandleCreated.
        private const int STATE2_HAVEINVOKED = 0x00000001;
        private const int STATE2_SETSCROLLPOS = 0x00000002;
        private const int STATE2_LISTENINGTOUSERPREFERENCECHANGED = 0x00000004;   // set when the control is listening to SystemEvents.UserPreferenceChanged.
        internal const int STATE2_INTERESTEDINUSERPREFERENCECHANGED = 0x00000008;   // if set, the control will listen to SystemEvents.UserPreferenceChanged when TopLevel is true and handle is created.
        internal const int STATE2_MAINTAINSOWNCAPTUREMODE = 0x00000010;   // if set, the control DOES NOT necessarily take capture on MouseDown
        private const int STATE2_BECOMINGACTIVECONTROL = 0x00000020;   // set to true by ContainerControl when this control is becoming its active control

        private const int STATE2_CLEARLAYOUTARGS = 0x00000040;   // if set, the next time PerformLayout is called, cachedLayoutEventArg will be cleared.
        private const int STATE2_INPUTKEY = 0x00000080;
        private const int STATE2_INPUTCHAR = 0x00000100;
        private const int STATE2_UICUES = 0x00000200;
        private const int STATE2_ISACTIVEX = 0x00000400;
        internal const int STATE2_USEPREFERREDSIZECACHE = 0x00000800;
        internal const int STATE2_TOPMDIWINDOWCLOSING = 0x00001000;
        internal const int STATE2_CURRENTLYBEINGSCALED = 0x00002000;   // if set, the control is being scaled, currently

        private static readonly object s_autoSizeChangedEvent = new object();
        private static readonly object s_keyDownEvent = new object();
        private static readonly object s_keyPressEvent = new object();
        private static readonly object s_keyUpEvent = new object();
        private static readonly object s_mouseDownEvent = new object();
        private static readonly object s_mouseEnterEvent = new object();
        private static readonly object s_mouseLeaveEvent = new object();
        private static readonly object s_dpiChangedBeforeParentEvent = new object();
        private static readonly object s_dpiChangedAfterParentEvent = new object();
        private static readonly object s_mouseHoverEvent = new object();
        private static readonly object s_mouseMoveEvent = new object();
        private static readonly object s_mouseUpEvent = new object();
        private static readonly object s_mouseWheelEvent = new object();
        private static readonly object s_clickEvent = new object();
        private static readonly object s_clientSizeEvent = new object();
        private static readonly object s_doubleClickEvent = new object();
        private static readonly object s_mouseClickEvent = new object();
        private static readonly object s_mouseDoubleClickEvent = new object();
        private static readonly object s_mouseCaptureChangedEvent = new object();
        private static readonly object s_moveEvent = new object();
        private static readonly object s_resizeEvent = new object();
        private static readonly object s_layoutEvent = new object();
        private static readonly object s_gotFocusEvent = new object();
        private static readonly object s_lostFocusEvent = new object();
        private static readonly object s_enabledChangedEvent = new object();
        private static readonly object s_enterEvent = new object();
        private static readonly object s_leaveEvent = new object();
        private static readonly object s_handleCreatedEvent = new object();
        private static readonly object s_handleDestroyedEvent = new object();
        private static readonly object s_visibleChangedEvent = new object();
        private static readonly object s_controlAddedEvent = new object();
        private static readonly object s_controlRemovedEvent = new object();
        private static readonly object s_changeUICuesEvent = new object();
        private static readonly object s_systemColorsChangedEvent = new object();
        private static readonly object s_validatingEvent = new object();
        private static readonly object s_validatedEvent = new object();
        private static readonly object s_styleChangedEvent = new object();
        private static readonly object s_imeModeChangedEvent = new object();
        private static readonly object s_helpRequestedEvent = new object();
        private static readonly object s_paintEvent = new object();
        private static readonly object s_invalidatedEvent = new object();
        private static readonly object s_queryContinueDragEvent = new object();
        private static readonly object s_giveFeedbackEvent = new object();
        private static readonly object s_dragEnterEvent = new object();
        private static readonly object s_dragLeaveEvent = new object();
        private static readonly object s_dragOverEvent = new object();
        private static readonly object s_dragDropEvent = new object();
        private static readonly object s_queryAccessibilityHelpEvent = new object();
        private static readonly object s_backgroundImageEvent = new object();
        private static readonly object s_backgroundImageLayoutEvent = new object();
        private static readonly object s_bindingContextEvent = new object();
        private static readonly object s_backColorEvent = new object();
        private static readonly object s_parentEvent = new object();
        private static readonly object s_visibleEvent = new object();
        private static readonly object s_textEvent = new object();
        private static readonly object s_tabStopEvent = new object();
        private static readonly object s_tabIndexEvent = new object();
        private static readonly object s_sizeEvent = new object();
        private static readonly object s_rightToLeftEvent = new object();
        private static readonly object s_locationEvent = new object();
        private static readonly object s_foreColorEvent = new object();
        private static readonly object s_fontEvent = new object();
        private static readonly object s_enabledEvent = new object();
        private static readonly object s_dockEvent = new object();
        private static readonly object s_cursorEvent = new object();
        private static readonly object s_contextMenuStripEvent = new object();
        private static readonly object s_causesValidationEvent = new object();
        private static readonly object s_regionChangedEvent = new object();
        private static readonly object s_marginChangedEvent = new object();
        private protected static readonly object s_paddingChangedEvent = new object();
        private static readonly object s_previewKeyDownEvent = new object();

        private static int s_threadCallbackMessage;
        private static ContextCallback s_invokeMarshaledCallbackHelperDelegate;

#pragma warning disable IDE1006 // Naming Styles
        [ThreadStatic]
        private static bool t_inCrossThreadSafeCall = false;

        [ThreadStatic]
        internal static HelpInfo t_currentHelpInfo = null;

        [ThreadStatic]
        private static byte[] t_tempKeyboardStateArray;
#pragma warning restore IDE1006

        private static FontHandleWrapper s_defaultFontHandleWrapper;

        private const short PaintLayerBackground = 1;
        private const short PaintLayerForeground = 2;

        private const byte RequiredScalingEnabledMask = 0x10;
        private const byte RequiredScalingMask = 0x0F;

        private const byte HighOrderBitMask = 0x80;

        private static Font s_defaultFont;

        // Property store keys for properties.  The property store allocates most efficiently
        // in groups of four, so we try to lump properties in groups of four based on how
        // likely they are going to be used in a group.
        private static readonly int s_namePropertyProperty = PropertyStore.CreateKey();
        private static readonly int s_backBrushProperty = PropertyStore.CreateKey();
        private static readonly int s_fontHeightProperty = PropertyStore.CreateKey();
        private static readonly int s_currentAmbientFontProperty = PropertyStore.CreateKey();

        private static readonly int s_controlsCollectionProperty = PropertyStore.CreateKey();
        private static readonly int s_backColorProperty = PropertyStore.CreateKey();
        private static readonly int s_foreColorProperty = PropertyStore.CreateKey();
        private static readonly int s_fontProperty = PropertyStore.CreateKey();

        private static readonly int s_backgroundImageProperty = PropertyStore.CreateKey();
        private static readonly int s_fontHandleWrapperProperty = PropertyStore.CreateKey();
        private static readonly int s_userDataProperty = PropertyStore.CreateKey();

        private static readonly int s_cursorProperty = PropertyStore.CreateKey();
        private static readonly int s_regionProperty = PropertyStore.CreateKey();
        private static readonly int s_rightToLeftProperty = PropertyStore.CreateKey();

        private static readonly int s_bindingsProperty = PropertyStore.CreateKey();
        private static readonly int s_bindingManagerProperty = PropertyStore.CreateKey();
        private static readonly int s_accessbileDefaultActionProperty = PropertyStore.CreateKey();
        private static readonly int s_accessibleDescriptionProperty = PropertyStore.CreateKey();

        private static readonly int s_accessibilityProperty = PropertyStore.CreateKey();
        private static readonly int s_ncAccessibilityProperty = PropertyStore.CreateKey();
        private static readonly int s_accessibleNameProperty = PropertyStore.CreateKey();
        private static readonly int s_accessibleRoleProperty = PropertyStore.CreateKey();

        private static readonly int s_paintingExceptionProperty = PropertyStore.CreateKey();
        private static readonly int s_activeXImplProperty = PropertyStore.CreateKey();
        private static readonly int s_controlVersionInfoProperty = PropertyStore.CreateKey();
        private static readonly int s_backgroundImageLayoutProperty = PropertyStore.CreateKey();

        private static readonly int s_accessibleHelpProviderProperty = PropertyStore.CreateKey();
        private static readonly int s_contextMenuStripProperty = PropertyStore.CreateKey();
        private static readonly int s_autoScrollOffsetProperty = PropertyStore.CreateKey();
        private static readonly int s_useCompatibleTextRenderingProperty = PropertyStore.CreateKey();

        private static readonly int s_imeWmCharsToIgnoreProperty = PropertyStore.CreateKey();
        private static readonly int s_imeModeProperty = PropertyStore.CreateKey();
        private static readonly int s_disableImeModeChangedCountProperty = PropertyStore.CreateKey();
        private static readonly int s_lastCanEnableImeProperty = PropertyStore.CreateKey();

        private static readonly int s_cacheTextCountProperty = PropertyStore.CreateKey();
        private static readonly int s_acheTextFieldProperty = PropertyStore.CreateKey();
        private static readonly int s_ambientPropertiesServiceProperty = PropertyStore.CreateKey();

        private static bool s_needToLoadComCtl = true;

        // This switch determines the default text rendering engine to use by some controls that support switching rendering engine.
        // CheckedListBox, PropertyGrid, GroupBox, Label and LinkLabel, and ButtonBase controls.
        // True means use GDI+, false means use GDI (TextRenderer).
        internal static bool UseCompatibleTextRenderingDefault { get; set; } = true;

        // Control instance members
        //
        // Note: Do not add anything to this list unless absolutely neccessary.
        //       Every control on a form has the overhead of all of these
        //       variables!

        // Resist the temptation to make this variable 'internal' rather than
        // private. Handle access should be tightly controlled, and is in this
        // file.  Making it 'internal' makes controlling it quite difficult.
        private readonly ControlNativeWindow _window;

        private Control _parent;
        private Control _reflectParent;
        private CreateParams _createParams;
        private int _x;
        private int _y;
        private int _width;
        private int _height;
        private int _clientWidth;
        private int _clientHeight;
        private int _state;                         // See STATE_ constants above
        private int _state2;                        // See STATE2_ constants above
        private ControlStyles _controlStyle;        // User supplied control style
        private int _tabIndex;
        private string _text;                       // See ControlStyles.CacheText for usage notes
        private byte _layoutSuspendCount;
        private byte _requiredScaling;              // bits 0-4: BoundsSpecified stored in RequiredScaling property.  Bit 5: RequiredScalingEnabled property.
        private NativeMethods.TRACKMOUSEEVENT _trackMouseEvent;
        private short _updateCount;
        private LayoutEventArgs _cachedLayoutEventArgs;
        private Queue _threadCallbackList;
        internal int _deviceDpi;

        // for keeping track of our ui state for focus and keyboard cues.  using a member variable
        // here because we hit this a lot
        private int _uiCuesState;

        private const int UISTATE_FOCUS_CUES_MASK = 0x000F;
        private const int UISTATE_FOCUS_CUES_HIDDEN = 0x0001;
        private const int UISTATE_FOCUS_CUES_SHOW = 0x0002;
        private const int UISTATE_KEYBOARD_CUES_MASK = 0x00F0;
        private const int UISTATE_KEYBOARD_CUES_HIDDEN = 0x0010;
        private const int UISTATE_KEYBOARD_CUES_SHOW = 0x0020;

#if DEBUG
        internal int LayoutSuspendCount
        {
            get { return _layoutSuspendCount; }
        }

        internal void AssertLayoutSuspendCount(int value)
        {
            Debug.Assert(value == _layoutSuspendCount, "Suspend/Resume layout mismatch!");
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

        /// <summary>
        ///  Initializes a new instance of the <see cref='Control'/> class.
        /// </summary>
        public Control() : this(true)
        {
        }

        internal Control(bool autoInstallSyncContext) : base()
        {
#if DEBUG
            if (s_assertOnControlCreateSwitch.Enabled)
            {
                Debug.Assert(GetType().BaseType != typeof(Control), "Direct derivative of Control Created: " + GetType().FullName);
                Debug.Assert(GetType() != typeof(Control), "Control Created!");
            }
#endif
            Properties = new PropertyStore();

            DpiHelper.InitializeDpiHelperForWinforms();
            // Initialize DPI to the value on the primary screen, we will have the correct value when the Handle is created.
            _deviceDpi = DpiHelper.DeviceDpi;

            _window = new ControlNativeWindow(this);
            RequiredScalingEnabled = true;
            RequiredScaling = BoundsSpecified.All;
            _tabIndex = -1;

            _state = STATE_VISIBLE | STATE_ENABLED | STATE_TABSTOP | STATE_CAUSESVALIDATION;
            _state2 = STATE2_INTERESTEDINUSERPREFERENCECHANGED;
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint |
                     ControlStyles.StandardClick |
                     ControlStyles.StandardDoubleClick |
                     ControlStyles.UseTextForAccessibility |
                     ControlStyles.Selectable, true);

            // We baked the "default default" margin and min size into CommonProperties
            // so that in the common case the PropertyStore would be empty.  If, however,
            // someone overrides these Default* methads, we need to write the default
            // value into the PropertyStore in the ctor.

            if (DefaultMargin != CommonProperties.DefaultMargin)
            {
                Margin = DefaultMargin;
            }
            if (DefaultMinimumSize != CommonProperties.DefaultMinimumSize)
            {
                MinimumSize = DefaultMinimumSize;
            }
            if (DefaultMaximumSize != CommonProperties.DefaultMaximumSize)
            {
                MaximumSize = DefaultMaximumSize;
            }

            // Compute our default size.
            Size defaultSize = DefaultSize;
            _width = defaultSize.Width;
            _height = defaultSize.Height;

            // DefaultSize may have hit GetPreferredSize causing a PreferredSize to be cached.  The
            // PreferredSize may change as a result of the current size.  Since a  SetBoundsCore did
            // not happen, so we need to clear the preferredSize cache manually.
            CommonProperties.xClearPreferredSizeCache(this);

            if (_width != 0 && _height != 0)
            {
                RECT rect = new RECT();
                rect.left = rect.right = rect.top = rect.bottom = 0;

                CreateParams cp = CreateParams;

                AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);
                _clientWidth = _width - (rect.right - rect.left);
                _clientHeight = _height - (rect.bottom - rect.top);
            }

            // Set up for async operations on this thread.
            if (autoInstallSyncContext)
            {
                WindowsFormsSynchronizationContext.InstallIfNeeded();
            }
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Control'/> class.
        /// </summary>
        public Control(string text) : this(null, text)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Control'/> class.
        /// </summary>
        public Control(string text, int left, int top, int width, int height) :
                    this(null, text, left, top, width, height)
        {
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Control'/> class.
        /// </summary>
        public Control(Control parent, string text) : this()
        {
            Parent = parent;
            Text = text;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref='Control'/> class.
        /// </summary>
        public Control(Control parent, string text, int left, int top, int width, int height) : this(parent, text)
        {
            Location = new Point(left, top);
            Size = new Size(width, height);
        }

        /// <summary>
        ///  Gets control Dpi awareness context value.
        /// </summary>
        internal DpiAwarenessContext DpiAwarenessContext => _window.DpiAwarenessContext;

        /// <summary>
        ///  The Accessibility Object for this Control
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlAccessibilityObjectDescr))
        ]
        public AccessibleObject AccessibilityObject
        {
            get
            {
                AccessibleObject accessibleObject = (AccessibleObject)Properties.GetObject(s_accessibilityProperty);
                if (accessibleObject == null)
                {
                    accessibleObject = CreateAccessibilityInstance();
                    // this is a security check. we want to enforce that we only return
                    // ControlAccessibleObject and not some other derived class
                    if (!(accessibleObject is ControlAccessibleObject))
                    {
                        Debug.Fail("Accessible objects for controls must be derived from ControlAccessibleObject.");
                        return null;
                    }
                    Properties.SetObject(s_accessibilityProperty, accessibleObject);
                }

                Debug.Assert(accessibleObject != null, "Failed to create accessibility object");
                return accessibleObject;
            }
        }

        /// <summary>
        ///  Private accessibility object for control, used to wrap the object that
        ///  OLEACC.DLL creates to represent the control's non-client (NC) region.
        /// </summary>
        private AccessibleObject NcAccessibilityObject
        {
            get
            {
                AccessibleObject ncAccessibleObject = (AccessibleObject)Properties.GetObject(s_ncAccessibilityProperty);
                if (ncAccessibleObject == null)
                {
                    ncAccessibleObject = new ControlAccessibleObject(this, NativeMethods.OBJID_WINDOW);
                    Properties.SetObject(s_ncAccessibilityProperty, ncAccessibleObject);
                }

                Debug.Assert(ncAccessibleObject != null, "Failed to create NON-CLIENT accessibility object");
                return ncAccessibleObject;
            }
        }

        /// <summary>
        ///  Returns a specific AccessibleObject associated with this
        ///  control, based on standard "accessibile object id".
        /// </summary>
        private AccessibleObject GetAccessibilityObject(int accObjId)
        {
            AccessibleObject accessibleObject;

            switch (accObjId)
            {
                case NativeMethods.OBJID_CLIENT:
                    accessibleObject = AccessibilityObject;
                    break;
                case NativeMethods.OBJID_WINDOW:
                    accessibleObject = NcAccessibilityObject;
                    break;
                default:
                    if (accObjId > 0)
                    {
                        accessibleObject = GetAccessibilityObjectById(accObjId);
                    }
                    else
                    {
                        accessibleObject = null;
                    }
                    break;
            }

            return accessibleObject;
        }

        /// <summary>
        ///  Returns a specific AccessibleObbject associated w/ the objectID
        /// </summary>
        protected virtual AccessibleObject GetAccessibilityObjectById(int objectId)
        {
            if (this is IAutomationLiveRegion)
            {
                return AccessibilityObject;
            }

            return null;
        }

        /// <summary>
        ///  The default action description of the control
        /// </summary>
        [
            SRCategory(nameof(SR.CatAccessibility)),
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlAccessibleDefaultActionDescr))
        ]
        public string AccessibleDefaultActionDescription
        {
            get => (string)Properties.GetObject(s_accessbileDefaultActionProperty);
            set => Properties.SetObject(s_accessbileDefaultActionProperty, value);
        }

        /// <summary>
        ///  The accessible description of the control
        /// </summary>
        [
            SRCategory(nameof(SR.CatAccessibility)),
            DefaultValue(null),
            Localizable(true),
            SRDescription(nameof(SR.ControlAccessibleDescriptionDescr))
        ]
        public string AccessibleDescription
        {
            get => (string)Properties.GetObject(s_accessibleDescriptionProperty);
            set => Properties.SetObject(s_accessibleDescriptionProperty, value);
        }

        /// <summary>
        ///  The accessible name of the control
        /// </summary>
        [
            SRCategory(nameof(SR.CatAccessibility)),
            DefaultValue(null),
            Localizable(true),
            SRDescription(nameof(SR.ControlAccessibleNameDescr))
        ]
        public string AccessibleName
        {
            get => (string)Properties.GetObject(s_accessibleNameProperty);
            set => Properties.SetObject(s_accessibleNameProperty, value);
        }

        /// <summary>
        ///  The accessible role of the control
        /// </summary>
        [
            SRCategory(nameof(SR.CatAccessibility)),
            DefaultValue(AccessibleRole.Default),
            SRDescription(nameof(SR.ControlAccessibleRoleDescr))
        ]
        public AccessibleRole AccessibleRole
        {
            get
            {
                int role = Properties.GetInteger(s_accessibleRoleProperty, out bool found);
                if (found)
                {
                    return (AccessibleRole)role;
                }
                else
                {
                    return AccessibleRole.Default;
                }
            }
            set
            {
                //valid values are -1 to 0x40
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AccessibleRole.Default, (int)AccessibleRole.OutlineButton))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AccessibleRole));
                }
                Properties.SetInteger(s_accessibleRoleProperty, (int)value);
            }
        }

        // Helper methods for retrieving an ActiveX properties.
        // We abstract these to another method so we do not force JIT the ActiveX codebase.

        private Color ActiveXAmbientBackColor => ActiveXInstance.AmbientBackColor;
        private Color ActiveXAmbientForeColor => ActiveXInstance.AmbientForeColor;
        private Font ActiveXAmbientFont => ActiveXInstance.AmbientFont;
        private bool ActiveXEventsFrozen => ActiveXInstance.EventsFrozen;
        private IntPtr ActiveXHWNDParent => ActiveXInstance.HWNDParent;

        /// <summary>
        ///  Retrieves the ActiveX control implementation for
        ///  this control.  This will demand create the implementation
        ///  if it does not already exist.
        /// </summary>
        private ActiveXImpl ActiveXInstance
        {
            get
            {
                ActiveXImpl activeXImpl = (ActiveXImpl)Properties.GetObject(s_activeXImplProperty);
                if (activeXImpl == null)
                {
                    // Don't allow top level objects to be hosted
                    // as activeX controls.
                    if (GetState(STATE_TOPLEVEL))
                    {
                        throw new NotSupportedException(SR.AXTopLevelSource);
                    }

                    activeXImpl = new ActiveXImpl(this);

                    // PERF: IsActiveX is called quite a bit - checked everywhere from sizing to event raising.  Using a state
                    // bit to track PropActiveXImpl instead of fetching from the property store.
                    SetState2(STATE2_ISACTIVEX, true);
                    Properties.SetObject(s_activeXImplProperty, activeXImpl);
                }

                return activeXImpl;
            }
        }

        /// <summary>
        ///  The AllowDrop property. If AllowDrop is set to true then
        ///  this control will allow drag and drop operations and events to be used.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(false),
            SRDescription(nameof(SR.ControlAllowDropDescr))
        ]
        public virtual bool AllowDrop
        {
            get
            {
                return GetState(STATE_ALLOWDROP);
            }
            set
            {
                if (GetState(STATE_ALLOWDROP) != value)
                {
                    SetState(STATE_ALLOWDROP, value);

                    if (IsHandleCreated)
                    {
                        try
                        {
                            SetAcceptDrops(value);
                        }
                        catch
                        {
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
        private AmbientProperties AmbientPropertiesService
        {
            get
            {
                AmbientProperties props = (AmbientProperties)Properties.GetObject(s_ambientPropertiesServiceProperty, out bool contains);
                if (!contains)
                {
                    if (Site != null)
                    {
                        props = Site.GetService(typeof(AmbientProperties)) as AmbientProperties;
                    }
                    else
                    {
                        props = (AmbientProperties)GetService(typeof(AmbientProperties));
                    }

                    if (props != null)
                    {
                        Properties.SetObject(s_ambientPropertiesServiceProperty, props);
                    }
                }
                return props;
            }
        }

        /// <summary>
        ///  The current value of the anchor property. The anchor property
        ///  determines which edges of the control are anchored to the container's
        ///  edges.
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Localizable(true),
            DefaultValue(CommonProperties.DefaultAnchor),
            SRDescription(nameof(SR.ControlAnchorDescr)),
            RefreshProperties(RefreshProperties.Repaint)
        ]
        public virtual AnchorStyles Anchor
        {
            get => DefaultLayout.GetAnchor(this);
            set => DefaultLayout.SetAnchor(ParentInternal, this, value);
        }

        [SRCategory(nameof(SR.CatLayout))]
        [RefreshProperties(RefreshProperties.All)]
        [Localizable(true)]
        [DefaultValue(CommonProperties.DefaultAutoSize)]
        [SRDescription(nameof(SR.ControlAutoSizeDescr))]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public virtual bool AutoSize
        {
            get { return CommonProperties.GetAutoSize(this); }
            set
            {
                if (value != AutoSize)
                {
                    CommonProperties.SetAutoSize(this, value);
                    if (ParentInternal != null)
                    {
                        // DefaultLayout does not keep anchor information until it needs to.  When
                        // AutoSize became a common property, we could no longer blindly call into
                        // DefaultLayout, so now we do a special InitLayout just for DefaultLayout.
                        if (value && ParentInternal.LayoutEngine == DefaultLayout.Instance)
                        {
                            ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.Size);
                        }
                        LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.AutoSize);
                    }

                    OnAutoSizeChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public event EventHandler AutoSizeChanged
        {
            add => Events.AddHandler(s_autoSizeChangedEvent, value);
            remove => Events.RemoveHandler(s_autoSizeChangedEvent, value);
        }

        /// <summary>
        ///  Controls the location of where this control is scrolled to in ScrollableControl.ScrollControlIntoView.
        ///  Default is the upper left hand corner of the control.
        /// </summary>
        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DefaultValue(typeof(Point), "0, 0")
        ]
        public virtual Point AutoScrollOffset
        {
            get
            {
                if (Properties.ContainsObject(s_autoScrollOffsetProperty))
                {
                    return (Point)Properties.GetObject(s_autoScrollOffsetProperty);
                }
                return Point.Empty;
            }
            set
            {
                if (AutoScrollOffset != value)
                {
                    Properties.SetObject(s_autoScrollOffsetProperty, value);
                }
            }
        }

        protected void SetAutoSizeMode(AutoSizeMode mode) => CommonProperties.SetAutoSizeMode(this, mode);

        protected AutoSizeMode GetAutoSizeMode() => CommonProperties.GetAutoSizeMode(this);

        // Public because this is interesting for ControlDesigners.
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual LayoutEngine LayoutEngine => DefaultLayout.Instance;

        /// <summary>
        ///  The GDI brush for our background color.
        ///  Whidbey Note: Made this internal, since we need to use this in ButtonStandardAdapter. Also, renamed
        ///         from BackBrush to BackColorBrush due to a naming conflict with DataGrid's BackBrush.
        /// </summary>
        internal IntPtr BackColorBrush
        {
            get
            {
                object customBackBrush = Properties.GetObject(s_backBrushProperty);
                if (customBackBrush != null)
                {
                    // We already have a valid brush.  Unbox, and return.
                    return (IntPtr)customBackBrush;
                }

                if (!Properties.ContainsObject(s_backColorProperty))
                {
                    // No custom back color.  See if we can get to our parent.
                    // The color check here is to account for parents and children who
                    // override the BackColor property.
                    if (_parent != null && _parent.BackColor == BackColor)
                    {
                        return _parent.BackColorBrush;
                    }
                }

                // No parent, or we have a custom back color.  Either way, we need to
                // create our own.
                Color color = BackColor;
                IntPtr backBrush;

                if (ColorTranslator.ToOle(color) < 0)
                {
                    backBrush = SafeNativeMethods.GetSysColorBrush(ColorTranslator.ToOle(color) & 0xFF);
                    SetState(STATE_OWNCTLBRUSH, false);
                }
                else
                {
                    backBrush = SafeNativeMethods.CreateSolidBrush(ColorTranslator.ToWin32(color));
                    SetState(STATE_OWNCTLBRUSH, true);
                }

                Debug.Assert(backBrush != IntPtr.Zero, "Failed to create brushHandle");
                Properties.SetObject(s_backBrushProperty, backBrush);

                return backBrush;
            }
        }

        /// <summary>
        ///  The background color of this control. This is an ambient property and
        ///  will always return a non-null value.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DispId(NativeMethods.ActiveX.DISPID_BACKCOLOR),
            SRDescription(nameof(SR.ControlBackColorDescr))
        ]
        public virtual Color BackColor
        {
            get
            {
                Color c = RawBackColor; // inheritedProperties.BackColor
                if (!c.IsEmpty)
                {
                    return c;
                }

                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties)
                {
                    c = p.BackColor;
                    if (IsValidBackColor(c))
                    {
                        return c;
                    }

                }

                if (IsActiveX)
                {
                    c = ActiveXAmbientBackColor;
                }

                if (c.IsEmpty)
                {
                    AmbientProperties ambient = AmbientPropertiesService;
                    if (ambient != null)
                    {
                        c = ambient.BackColor;
                    }
                }

                if (!c.IsEmpty && IsValidBackColor(c))
                {
                    return c;
                }
                else
                {
                    return DefaultBackColor;
                }
            }
            set
            {
                if (!value.Equals(Color.Empty) && !GetStyle(ControlStyles.SupportsTransparentBackColor) && value.A < 255)
                {
                    throw new ArgumentException(SR.TransparentBackColorNotAllowed);
                }

                Color c = BackColor;
                if (!value.IsEmpty || Properties.ContainsObject(s_backColorProperty))
                {
                    Properties.SetColor(s_backColorProperty, value);
                }

                if (!c.Equals(BackColor))
                {
                    OnBackColorChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBackColorChangedDescr))]
        public event EventHandler BackColorChanged
        {
            add => Events.AddHandler(s_backColorEvent, value);
            remove => Events.RemoveHandler(s_backColorEvent, value);
        }

        /// <summary>
        ///  The background image of the control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(null),
            Localizable(true),
            SRDescription(nameof(SR.ControlBackgroundImageDescr))
        ]
        public virtual Image BackgroundImage
        {
            get
            {
                return (Image)Properties.GetObject(s_backgroundImageProperty);
            }
            set
            {
                if (BackgroundImage != value)
                {
                    Properties.SetObject(s_backgroundImageProperty, value);
                    OnBackgroundImageChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBackgroundImageChangedDescr))]
        public event EventHandler BackgroundImageChanged
        {
            add => Events.AddHandler(s_backgroundImageEvent, value);
            remove => Events.RemoveHandler(s_backgroundImageEvent, value);
        }

        /// <summary>
        ///  The BackgroundImageLayout of the control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DefaultValue(ImageLayout.Tile),
            Localizable(true),
            SRDescription(nameof(SR.ControlBackgroundImageLayoutDescr))
        ]
        public virtual ImageLayout BackgroundImageLayout
        {
            get
            {
                bool found = Properties.ContainsObject(s_backgroundImageLayoutProperty);
                if (!found)
                {
                    return ImageLayout.Tile;
                }
                else
                {
                    return ((ImageLayout)Properties.GetObject(s_backgroundImageLayoutProperty));
                }
            }
            set
            {
                if (BackgroundImageLayout != value)
                {
                    // Valid values are 0x0 to 0x4
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ImageLayout.None, (int)ImageLayout.Zoom))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ImageLayout));
                    }

                    // Check if the value is either center, strech or zoom;
                    if (value == ImageLayout.Center || value == ImageLayout.Zoom || value == ImageLayout.Stretch)
                    {
                        SetStyle(ControlStyles.ResizeRedraw, true);

                        // Only for images that support transparency.
                        if (ControlPaint.IsImageTransparent(BackgroundImage))
                        {
                            DoubleBuffered = true;
                        }
                    }
                    Properties.SetObject(s_backgroundImageLayoutProperty, value);
                    OnBackgroundImageLayoutChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBackgroundImageLayoutChangedDescr))]
        public event EventHandler BackgroundImageLayoutChanged
        {
            add => Events.AddHandler(s_backgroundImageLayoutEvent, value);
            remove => Events.RemoveHandler(s_backgroundImageLayoutEvent, value);
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
                if (value != BecomingActiveControl)
                {
                    Application.ThreadContext.FromCurrent().ActivatingControl = (value) ? this : null;
                    SetState2(STATE2_BECOMINGACTIVECONTROL, value);
                }
            }
        }

        private bool ShouldSerializeAccessibleName() => !string.IsNullOrEmpty(AccessibleName);

        [EditorBrowsable(EditorBrowsableState.Never)]
        public void ResetBindings()
        {
            ControlBindingsCollection bindings = (ControlBindingsCollection)Properties.GetObject(s_bindingsProperty);
            if (bindings != null)
            {
                bindings.Clear();
            }
        }

        /// <summary>
        ///  BindingContextInternal provides a mechanism so that controls like SplitContainer that inherit from the
        ///  ContainerControl can bypass the "containerControls" bindingContext property and do what the other simple controls
        ///  do.
        /// </summary>
        internal BindingContext BindingContextInternal
        {
            get
            {
                // See if we have locally overridden the binding manager.
                BindingContext context = (BindingContext)Properties.GetObject(s_bindingManagerProperty);
                if (context != null)
                {
                    return context;
                }

                // Otherwise, see if the parent has one for us.
                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties)
                {
                    return p.BindingContext;
                }

                // Otherwise, we have no binding manager available.
                return null;
            }
            set
            {
                BindingContext oldContext = (BindingContext)Properties.GetObject(s_bindingManagerProperty);
                BindingContext newContext = value;

                if (oldContext != newContext)
                {
                    Properties.SetObject(s_bindingManagerProperty, newContext);

                    // the property change will wire up the bindings.
                    //
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
        }

        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlBindingContextDescr))
        ]
        public virtual BindingContext BindingContext
        {
            get => BindingContextInternal;
            set => BindingContextInternal = value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnBindingContextChangedDescr))]
        public event EventHandler BindingContextChanged
        {
            add => Events.AddHandler(s_bindingContextEvent, value);
            remove => Events.RemoveHandler(s_bindingContextEvent, value);
        }

        /// <summary>
        ///  The bottom coordinate of this control.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlBottomDescr)),
            SRCategory(nameof(SR.CatLayout))
        ]
        public int Bottom => _y + _height;

        /// <summary>
        ///  The bounds of this control. This is the window coordinates of the
        ///  control in parent client coordinates.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlBoundsDescr)),
            SRCategory(nameof(SR.CatLayout))
        ]
        public Rectangle Bounds
        {
            get => new Rectangle(_x, _y, _width, _height);
            set => SetBounds(value.X, value.Y, value.Width, value.Height, BoundsSpecified.All);
        }

        internal virtual bool CanAccessProperties => true;

        /// <summary>
        ///  Indicates whether the control can receive focus. This
        ///  property is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.ControlCanFocusDescr))
        ]
        public bool CanFocus
        {
            get
            {
                if (!IsHandleCreated)
                {
                    return false;
                }
                bool visible = SafeNativeMethods.IsWindowVisible(new HandleRef(_window, Handle));
                bool enabled = SafeNativeMethods.IsWindowEnabled(new HandleRef(_window, Handle));
                return (visible && enabled);
            }
        }

        /// <summary>
        ///  Determines if events can be fired on the control.  If this control is being
        ///  hosted as an ActiveX control, this property will return false if the ActiveX
        ///  control has its events frozen.
        /// </summary>
        protected override bool CanRaiseEvents => !IsActiveX ? true : !ActiveXEventsFrozen;

        /// <summary>
        ///  Indicates whether the control can be selected. This property
        ///  is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.ControlCanSelectDescr))
        ]
        public bool CanSelect => CanSelectCore();

        /// <summary>
        ///  Indicates whether the control has captured the mouse.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRCategory(nameof(SR.CatFocus)),
            SRDescription(nameof(SR.ControlCaptureDescr))
        ]
        public bool Capture
        {
            get => CaptureInternal;
            set => CaptureInternal = value;
        }

        internal bool CaptureInternal
        {
            get => IsHandleCreated && UnsafeNativeMethods.GetCapture() == Handle;
            set
            {
                if (CaptureInternal != value)
                {
                    if (value)
                    {
                        UnsafeNativeMethods.SetCapture(new HandleRef(this, Handle));
                    }
                    else
                    {
                        SafeNativeMethods.ReleaseCapture();
                    }
                }
            }
        }

        /// <summary>
        ///  Indicates whether entering the control causes validation on the controls requiring validation.
        /// </summary>
        [
            SRCategory(nameof(SR.CatFocus)),
            DefaultValue(true),
            SRDescription(nameof(SR.ControlCausesValidationDescr))
        ]
        public bool CausesValidation
        {
            get => GetState(STATE_CAUSESVALIDATION);
            set
            {
                if (value != CausesValidation)
                {
                    SetState(STATE_CAUSESVALIDATION, value);
                    OnCausesValidationChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnCausesValidationChangedDescr))]
        public event EventHandler CausesValidationChanged
        {
            add => Events.AddHandler(s_causesValidationEvent, value);
            remove => Events.RemoveHandler(s_causesValidationEvent, value);
        }

        ///  This is for perf. Turn this property on to temporarily enable text caching.  This is good for
        ///  operations such as layout or painting where we don't expect the text to change (we will update the
        ///  cache if it does) but prevents us from sending a ton of messages turing layout.  See the PaintWithErrorHandling
        ///  function.
        ///
        internal bool CacheTextInternal
        {
            get
            {
                // check if we're caching text.
                int cacheTextCounter = Properties.GetInteger(s_cacheTextCountProperty, out bool _);

                return cacheTextCounter > 0 || GetStyle(ControlStyles.CacheText);
            }
            set
            {
                // Ff this control always cachest text or the handle hasn't been created,
                // just bail.
                if (GetStyle(ControlStyles.CacheText) || !IsHandleCreated)
                {
                    return;
                }

                // otherwise, get the state and update the cache if necessary.
                int cacheTextCounter = Properties.GetInteger(s_cacheTextCountProperty, out bool found);

                if (value)
                {
                    if (cacheTextCounter == 0)
                    {
                        Properties.SetObject(s_acheTextFieldProperty, _text);
                        if (_text == null)
                        {
                            _text = WindowText;
                        }
                    }
                    cacheTextCounter++;
                }
                else
                {
                    cacheTextCounter--;
                    if (cacheTextCounter == 0)
                    {
                        _text = (string)Properties.GetObject(s_acheTextFieldProperty, out found);
                    }
                }
                Properties.SetInteger(s_cacheTextCountProperty, cacheTextCounter);
            }
        }

        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            SRDescription(nameof(SR.ControlCheckForIllegalCrossThreadCalls)),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public static bool CheckForIllegalCrossThreadCalls { get; set; } = Debugger.IsAttached;

        /// <summary>
        ///  The client rect of the control.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRCategory(nameof(SR.CatLayout)),
            SRDescription(nameof(SR.ControlClientRectangleDescr))
        ]
        public Rectangle ClientRectangle
        {
            get
            {
                return new Rectangle(0, 0, _clientWidth, _clientHeight);
            }
        }

        /// <summary>
        ///  The size of the clientRect.
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlClientSizeDescr))
        ]
        public Size ClientSize
        {
            get => new Size(_clientWidth, _clientHeight);
            set => SetClientSizeCore(value.Width, value.Height);
        }

        /// <summary>
        ///  Fired when ClientSize changes.
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnClientSizeChangedDescr))]
        public event EventHandler ClientSizeChanged
        {
            add => Events.AddHandler(s_clientSizeEvent, value);
            remove => Events.RemoveHandler(s_clientSizeEvent, value);
        }

        /// <summary>
        ///  Retrieves the company name of this specific component.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            Description(nameof(SR.ControlCompanyNameDescr))
        ]
        public string CompanyName => VersionInfo.CompanyName;

        /// <summary>
        ///  Indicates whether the control or one of its children currently has the system
        ///  focus. This property is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlContainsFocusDescr))
        ]
        public bool ContainsFocus
        {
            get
            {
                if (!IsHandleCreated)
                {
                    return false;
                }

                IntPtr focusHwnd = UnsafeNativeMethods.GetFocus();

                if (focusHwnd == IntPtr.Zero)
                {
                    return false;
                }

                if (focusHwnd == Handle)
                {
                    return true;
                }

                if (UnsafeNativeMethods.IsChild(new HandleRef(this, Handle), new HandleRef(this, focusHwnd)))
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        ///  The contextMenuStrip associated with this control. The contextMenuStrip
        ///  will be shown when the user right clicks the mouse on the control.
        ///  Note: if a context menu is also assigned, it will take precidence over this property.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            DefaultValue(null),
            SRDescription(nameof(SR.ControlContextMenuDescr))
        ]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get => (ContextMenuStrip)Properties.GetObject(s_contextMenuStripProperty);
            set
            {
                ContextMenuStrip oldValue = Properties.GetObject(s_contextMenuStripProperty) as ContextMenuStrip;

                if (oldValue != value)
                {
                    EventHandler disposedHandler = new EventHandler(DetachContextMenuStrip);

                    if (oldValue != null)
                    {
                        oldValue.Disposed -= disposedHandler;
                    }

                    Properties.SetObject(s_contextMenuStripProperty, value);

                    if (value != null)
                    {
                        value.Disposed += disposedHandler;
                    }

                    OnContextMenuStripChanged(EventArgs.Empty);
                }
            }

        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlContextMenuStripChangedDescr))]
        public event EventHandler ContextMenuStripChanged
        {
            add => Events.AddHandler(s_contextMenuStripEvent, value);
            remove => Events.RemoveHandler(s_contextMenuStripEvent, value);
        }

        /// <summary>
        ///  Collection of child controls.
        /// </summary>
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
            SRDescription(nameof(SR.ControlControlsDescr))
        ]
        public ControlCollection Controls
        {
            get
            {
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                if (controlsCollection == null)
                {
                    controlsCollection = CreateControlsInstance();
                    Properties.SetObject(s_controlsCollectionProperty, controlsCollection);
                }
                return controlsCollection;
            }
        }

        /// <summary>
        ///  Indicates whether the control has been created. This property is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlCreatedDescr))
        ]
        public bool Created => (_state & STATE_CREATED) != 0;

        /// <summary>
        ///  Returns the CreateParams used to create the handle for this control.
        ///  Inheriting classes should call base.CreateParams in the manor
        ///  below:
        /// </summary>
        protected virtual CreateParams CreateParams
        {
            get
            {
                // CLR4.0 or later, comctl32.dll needs to be loaded explicitly.
                if (s_needToLoadComCtl)
                {
                    if ((Kernel32.GetModuleHandleW(Libraries.Comctl32) != IntPtr.Zero)
                        || (Kernel32.LoadLibraryFromSystemPathIfAvailable(Libraries.Comctl32) != IntPtr.Zero))
                    {
                        s_needToLoadComCtl = false;
                    }
                    else
                    {
                        int lastWin32Error = Marshal.GetLastWin32Error();
                        throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, Libraries.Comctl32));
                    }
                }

                // In a typical control this is accessed ten times to create and show a control.
                // It is a net memory savings, then, to maintain a copy on control.
                if (_createParams == null)
                {
                    _createParams = new CreateParams();
                }

                CreateParams cp = _createParams;
                cp.Style = 0;
                cp.ExStyle = 0;
                cp.ClassStyle = 0;
                cp.Caption = _text;

                cp.X = _x;
                cp.Y = _y;
                cp.Width = _width;
                cp.Height = _height;

                cp.Style = NativeMethods.WS_CLIPCHILDREN;
                if (GetStyle(ControlStyles.ContainerControl))
                {
                    cp.ExStyle |= NativeMethods.WS_EX_CONTROLPARENT;
                }
                cp.ClassStyle = (int)NativeMethods.ClassStyle.CS_DBLCLKS;

                if ((_state & STATE_TOPLEVEL) == 0)
                {
                    // When the window is actually created, we will parent WS_CHILD windows to the
                    // parking form if cp.parent == 0.
                    //
                    cp.Parent = _parent == null ? IntPtr.Zero : _parent.InternalHandle;
                    cp.Style |= NativeMethods.WS_CHILD | NativeMethods.WS_CLIPSIBLINGS;
                }
                else
                {
                    cp.Parent = IntPtr.Zero;
                }

                if ((_state & STATE_TABSTOP) != 0)
                {
                    cp.Style |= NativeMethods.WS_TABSTOP;
                }

                if ((_state & STATE_VISIBLE) != 0)
                {
                    cp.Style |= NativeMethods.WS_VISIBLE;
                }

                // Unlike Visible, Windows doesn't correctly inherit disabledness from its parent -- an enabled child
                // of a disabled parent will look enabled but not get mouse events
                if (!Enabled)
                {
                    cp.Style |= NativeMethods.WS_DISABLED;
                }

                // If we are being hosted as an Ax control, try to prevent the parking window
                // from being created by pre-filling the window handle here.
                if (cp.Parent == IntPtr.Zero && IsActiveX)
                {
                    cp.Parent = ActiveXHWNDParent;
                }

                // Set Rtl bits
                if (RightToLeft == RightToLeft.Yes)
                {
                    cp.ExStyle |= NativeMethods.WS_EX_RTLREADING;
                    cp.ExStyle |= NativeMethods.WS_EX_RIGHT;
                    cp.ExStyle |= NativeMethods.WS_EX_LEFTSCROLLBAR;
                }

                return cp;
            }
        }

        internal virtual void NotifyValidationResult(object sender, CancelEventArgs ev)
        {
            ValidationCancelled = ev.Cancel;
        }

        /// <summary>
        ///  Helper method...
        ///
        ///  Triggers validation on the active control, and returns bool indicating whether that control was valid.
        ///
        ///  The correct way to do this is to find the common ancestor of the active control and this control,
        ///  then request validation to be performed by that common container control.
        ///
        ///  Used by controls that don't participate in the normal enter/leave/validation process, but which
        ///  want to force form-level validation to occur before they attempt some important action.
        /// </summary>
        internal bool ValidateActiveControl(out bool validatedControlAllowsFocusChange)
        {
            bool valid = true;
            validatedControlAllowsFocusChange = false;
            IContainerControl c = GetContainerControl();
            if (c != null && CausesValidation)
            {
                if (c is ContainerControl container)
                {
                    while (container.ActiveControl == null)
                    {
                        ContainerControl cc;
                        Control parent = container.ParentInternal;
                        if (parent != null)
                        {
                            cc = parent.GetContainerControl() as ContainerControl;
                            if (cc != null)
                            {
                                container = cc;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                    valid = container.ValidateInternal(true, out validatedControlAllowsFocusChange);
                }
            }

            return valid;
        }

        internal bool ValidationCancelled
        {
            set => SetState(STATE_VALIDATIONCANCELLED, value);
            get
            {
                if (GetState(STATE_VALIDATIONCANCELLED))
                {
                    return true;
                }
                else
                {
                    Control parent = ParentInternal;
                    if (parent != null)
                    {
                        return parent.ValidationCancelled;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        ///  returns bool indicating whether the Top MDI Window is closing.
        ///  This property is set in the MDI children in WmClose method in form.cs when the top window is closing.
        ///  This property will be used in ActiveControl to determine if we want to skip set focus and window handle re-creation for the control.
        /// </summary>
        internal bool IsTopMdiWindowClosing
        {
            get => GetState2(STATE2_TOPMDIWINDOWCLOSING);
            set => SetState2(STATE2_TOPMDIWINDOWCLOSING, value);
        }

        /// <summary>
        ///  returns bool indicating whether the control is currently being scaled.
        ///  This property is set in ScaleControl method to allow method being called to condition code that should not run for scaling.
        /// </summary>
        internal bool IsCurrentlyBeingScaled
        {
            get => GetState2(STATE2_CURRENTLYBEINGSCALED);
            private set => SetState2(STATE2_CURRENTLYBEINGSCALED, value);
        }

        /// <summary>
        ///  Retrieves the Win32 thread ID of the thread that created the
        ///  handle for this control.  If the control's handle hasn't been
        ///  created yet, this method will return the current thread's ID.
        /// </summary>
        internal int CreateThreadId => IsHandleCreated
            ? SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, Handle), out int _)
            : SafeNativeMethods.GetCurrentThreadId();

        /// <summary>
        ///  Retrieves the cursor that will be displayed when the mouse is over this
        ///  control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            SRDescription(nameof(SR.ControlCursorDescr)),
            AmbientValue(null)
        ]
        public virtual Cursor Cursor
        {
            get
            {
                if (GetState(STATE_USEWAITCURSOR))
                {
                    return Cursors.WaitCursor;
                }

                Cursor cursor = (Cursor)Properties.GetObject(s_cursorProperty);
                if (cursor != null)
                {
                    return cursor;
                }

                // We only do ambients for things with "Cursors.Default"
                // as their default.
                Cursor localDefault = DefaultCursor;
                if (localDefault != Cursors.Default)
                {
                    return localDefault;
                }

                Control p = ParentInternal;
                if (p != null)
                {
                    return p.Cursor;
                }

                AmbientProperties ambient = AmbientPropertiesService;
                if (ambient != null && ambient.Cursor != null)
                {
                    return ambient.Cursor;
                }

                return localDefault;
            }
            set
            {
                Cursor localCursor = (Cursor)Properties.GetObject(s_cursorProperty);
                Cursor resolvedCursor = Cursor;
                if (localCursor != value)
                {
                    Properties.SetObject(s_cursorProperty, value);
                }

                // Other things can change the cursor... we
                // really want to force the correct cursor always...
                if (IsHandleCreated)
                {
                    // We want to instantly change the cursor if the mouse is within our bounds.
                    // This includes the case where the mouse is over one of our children
                    var r = new RECT();
                    UnsafeNativeMethods.GetCursorPos(out Point p);
                    UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);

                    if ((r.left <= p.X && p.X < r.right && r.top <= p.Y && p.Y < r.bottom) || UnsafeNativeMethods.GetCapture() == Handle)
                    {
                        SendMessage(WindowMessages.WM_SETCURSOR, Handle, (IntPtr)NativeMethods.HTCLIENT);
                    }
                }

                if (!resolvedCursor.Equals(value))
                {
                    OnCursorChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnCursorChangedDescr))]
        public event EventHandler CursorChanged
        {
            add => Events.AddHandler(s_cursorEvent, value);
            remove => Events.RemoveHandler(s_cursorEvent, value);
        }

        /// <summary>
        ///  Retrieves the bindings for this control.
        /// </summary>
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
                ControlBindingsCollection bindings = (ControlBindingsCollection)Properties.GetObject(s_bindingsProperty);
                if (bindings == null)
                {
                    bindings = new ControlBindingsCollection(this);
                    Properties.SetObject(s_bindingsProperty, bindings);
                }
                return bindings;
            }
        }

        /// <summary>
        ///  The default BackColor of a generic top-level Control.  Subclasses may have
        ///  different defaults.
        /// </summary>
        public static Color DefaultBackColor => SystemColors.Control;

        /// <summary>
        ///  Deriving classes can override this to configure a default cursor for their control.
        ///  This is more efficient than setting the cursor in the control's constructor,
        ///  and gives automatic support for ShouldSerialize and Reset in the designer.
        /// </summary>
        protected virtual Cursor DefaultCursor => Cursors.Default;

        /// <summary>
        ///  The default Font of a generic top-level Control.  Subclasses may have
        ///  different defaults.
        /// </summary>
        public static Font DefaultFont
        {
            get
            {
                if (s_defaultFont == null)
                {
                    s_defaultFont = SystemFonts.MessageBoxFont;
                    Debug.Assert(s_defaultFont != null, "defaultFont wasn't set!");
                }

                return s_defaultFont;
            }
        }

        /// <summary>
        ///  The default ForeColor of a generic top-level Control.  Subclasses may have
        ///  different defaults.
        /// </summary>
        public static Color DefaultForeColor => SystemColors.ControlText;

        protected virtual Padding DefaultMargin => CommonProperties.DefaultMargin;

        protected virtual Size DefaultMaximumSize => CommonProperties.DefaultMaximumSize;

        protected virtual Size DefaultMinimumSize => CommonProperties.DefaultMinimumSize;

        protected virtual Padding DefaultPadding => Padding.Empty;

        private RightToLeft DefaultRightToLeft => RightToLeft.No;

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected virtual Size DefaultSize => Size.Empty;

        private void DetachContextMenuStrip(object sender, EventArgs e) => ContextMenuStrip = null;

        /// <summary>
        ///  DPI value either for the primary screen or for the monitor where the top-level parent is displayed when
        ///  EnableDpiChangedMessageHandling option is on and the application is per-monitor V2 DPI-aware (rs2+)
        /// </summary>
        [
            Browsable(false), // don't show in property browser
            EditorBrowsable(EditorBrowsableState.Always), // do show in the intellisense
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden) // do not serialize
        ]
        public int DeviceDpi
            // deviceDpi may change in WmDpiChangedBeforeParent in PmV2 scenarios, so we can't cache statically.
            => DpiHelper.IsPerMonitorV2Awareness ? _deviceDpi : DpiHelper.DeviceDpi;

        // The color to use when drawing disabled text.  Normally we use BackColor,
        // but that obviously won't work if we're transparent.
        internal Color DisabledColor
        {
            get
            {
                Color color = BackColor;
                if (color.A == 0)
                {
                    Control control = ParentInternal;
                    while (color.A == 0)
                    {
                        if (control == null)
                        {
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

        /// <summary>
        ///  Returns the client rect of the display area of the control.
        ///  For the base control class, this is identical to getClientRect.
        ///  However, inheriting controls may want to change this if their client
        ///  area differs from their display area.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlDisplayRectangleDescr))
        ]
        public virtual Rectangle DisplayRectangle
            => new Rectangle(0, 0, _clientWidth, _clientHeight);

        /// <summary>
        ///  Indicates whether the control has been disposed. This
        ///  property is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlDisposedDescr))
        ]
        public bool IsDisposed => GetState(STATE_DISPOSED);

        /// <summary>
        ///  Disposes of the currently selected font handle (if cached).
        /// </summary>
        private void DisposeFontHandle()
        {
            if (Properties.ContainsObject(s_fontHandleWrapperProperty))
            {
                if (Properties.GetObject(s_fontHandleWrapperProperty) is FontHandleWrapper fontHandle)
                {
                    fontHandle.Dispose();
                }
                Properties.SetObject(s_fontHandleWrapperProperty, null);
            }
        }

        /// <summary>
        ///  Indicates whether the control is in the process of being disposed. This
        ///  property is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlDisposingDescr))
        ]
        public bool Disposing => GetState(STATE_DISPOSING);

        /// <summary>
        ///  The dock property. The dock property controls to which edge
        ///  of the container this control is docked to. For example, when docked to
        ///  the top of the container, the control will be displayed flush at the
        ///  top of the container, extending the length of the container.
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Localizable(true),
            RefreshProperties(RefreshProperties.Repaint),
            DefaultValue(CommonProperties.DefaultDock),
            SRDescription(nameof(SR.ControlDockDescr))
        ]
        public virtual DockStyle Dock
        {
            get => DefaultLayout.GetDock(this);
            set
            {
                if (value != Dock)
                {
#if DEBUG
                    int dbgLayoutCheck = LayoutSuspendCount;
#endif
                    SuspendLayout();
                    try
                    {
                        DefaultLayout.SetDock(this, value);
                        OnDockChanged(EventArgs.Empty);
                    }
                    finally
                    {
                        ResumeLayout();
                    }
#if DEBUG
                    AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnDockChangedDescr))]
        public event EventHandler DockChanged
        {
            add => Events.AddHandler(s_dockEvent, value);
            remove => Events.RemoveHandler(s_dockEvent, value);
        }

        /// <summary>
        ///  This will enable or disable double buffering.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            SRDescription(nameof(SR.ControlDoubleBufferedDescr))
        ]
        protected virtual bool DoubleBuffered
        {
            get => GetStyle(ControlStyles.OptimizedDoubleBuffer);
            set
            {
                if (value != DoubleBuffered)
                {
                    if (value)
                    {
                        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.AllPaintingInWmPaint, value);
                    }
                    else
                    {
                        SetStyle(ControlStyles.OptimizedDoubleBuffer, value);
                    }
                }
            }
        }

        private bool DoubleBufferingEnabled => GetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint);

        /// <summary>
        ///  Indicates whether the control is currently enabled.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            Localizable(true),
            DispId(NativeMethods.ActiveX.DISPID_ENABLED),
            SRDescription(nameof(SR.ControlEnabledDescr))
        ]
        public bool Enabled
        {
            get
            {
                // We are only enabled if our parent is enabled
                if (!GetState(STATE_ENABLED))
                {
                    return false;
                }
                else if (ParentInternal == null)
                {
                    return true;
                }
                else
                {
                    return ParentInternal.Enabled;
                }
            }
            set
            {
                bool oldValue = Enabled;
                SetState(STATE_ENABLED, value);

                if (oldValue != value)
                {
                    if (!value)
                    {
                        SelectNextIfFocused();
                    }

                    OnEnabledChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Occurs when the control is enabled.
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnEnabledChangedDescr))]
        public event EventHandler EnabledChanged
        {
            add => Events.AddHandler(s_enabledEvent, value);
            remove => Events.RemoveHandler(s_enabledEvent, value);
        }

        /// <summary>
        ///  Indicates whether the control has focus. This property is read-only.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlFocusedDescr))
        ]
        public virtual bool Focused
            => IsHandleCreated && UnsafeNativeMethods.GetFocus() == Handle;

        /// <summary>
        ///  Retrieves the current font for this control. This will be the font used
        ///  by default for painting and text in the control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            Localizable(true),
            DispId(NativeMethods.ActiveX.DISPID_FONT),
            AmbientValue(null),
            SRDescription(nameof(SR.ControlFontDescr))
        ]
        public virtual Font Font
        {
            [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ActiveXFontMarshaler))]
            get
            {
                Font font = (Font)Properties.GetObject(s_fontProperty);
                if (font != null)
                {
                    return font;
                }

                Font f = GetParentFont();
                if (f != null)
                {
                    return f;
                }

                if (IsActiveX)
                {
                    f = ActiveXAmbientFont;
                    if (f != null)
                    {
                        return f;
                    }
                }

                AmbientProperties ambient = AmbientPropertiesService;
                if (ambient != null && ambient.Font != null)
                {
                    return ambient.Font;
                }

                return DefaultFont;
            }
            [param: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(ActiveXFontMarshaler))]
            set
            {
                Font local = (Font)Properties.GetObject(s_fontProperty);
                Font resolved = Font;

                bool localChanged = false;
                if (value == null)
                {
                    if (local != null)
                    {
                        localChanged = true;
                    }
                }
                else
                {
                    if (local == null)
                    {
                        localChanged = true;
                    }
                    else
                    {
                        localChanged = !value.Equals(local);
                    }
                }

                if (localChanged)
                {
                    // Store new local value
                    Properties.SetObject(s_fontProperty, value);

                    // We only fire the Changed event if the "resolved" value
                    // changed, however we must update the font if the local
                    // value changed...
                    if (!resolved.Equals(value))
                    {
                        // Cleanup any font handle wrapper...
                        DisposeFontHandle();

                        if (Properties.ContainsInteger(s_fontHeightProperty))
                        {
                            Properties.SetInteger(s_fontHeightProperty, (value == null) ? -1 : value.Height);
                        }

                        // Font is an ambient property.  We need to layout our parent because Font may
                        // change our size.  We need to layout ourselves because our children may change
                        // size by inheriting the new value.
                        using (new LayoutTransaction(ParentInternal, this, PropertyNames.Font))
                        {
                            OnFontChanged(EventArgs.Empty);
                        }
                    }
                    else
                    {
                        if (IsHandleCreated && !GetStyle(ControlStyles.UserPaint))
                        {
                            DisposeFontHandle();
                            SetWindowFont();
                        }
                    }
                }
            }
        }

        internal void ScaleFont(float factor)
        {
            Font local = (Font)Properties.GetObject(s_fontProperty);
            Font resolved = Font;
            Font newFont = new Font(Font.FontFamily, Font.Size * factor, Font.Style);

            if ((local == null) || !local.Equals(newFont))
            {
                Properties.SetObject(s_fontProperty, newFont);

                if (!resolved.Equals(newFont))
                {
                    DisposeFontHandle();

                    if (Properties.ContainsInteger(s_fontHeightProperty))
                    {
                        Properties.SetInteger(s_fontHeightProperty, newFont.Height);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnFontChangedDescr))]
        public event EventHandler FontChanged
        {
            add => Events.AddHandler(s_fontEvent, value);
            remove => Events.RemoveHandler(s_fontEvent, value);
        }

        internal IntPtr FontHandle
        {
            get
            {
                Font font = (Font)Properties.GetObject(s_fontProperty);

                if (font != null)
                {
                    FontHandleWrapper fontHandle = (FontHandleWrapper)Properties.GetObject(s_fontHandleWrapperProperty);
                    if (fontHandle == null)
                    {
                        fontHandle = new FontHandleWrapper(font);

                        Properties.SetObject(s_fontHandleWrapperProperty, fontHandle);
                    }

                    return fontHandle.Handle;
                }

                if (_parent != null)
                {
                    return _parent.FontHandle;
                }

                AmbientProperties ambient = AmbientPropertiesService;

                if (ambient != null && ambient.Font != null)
                {

                    FontHandleWrapper fontHandle = null;

                    Font currentAmbient = (Font)Properties.GetObject(s_currentAmbientFontProperty);

                    if (currentAmbient != null && currentAmbient == ambient.Font)
                    {
                        fontHandle = (FontHandleWrapper)Properties.GetObject(s_fontHandleWrapperProperty);
                    }
                    else
                    {
                        Properties.SetObject(s_currentAmbientFontProperty, ambient.Font);
                    }

                    if (fontHandle == null)
                    {
                        font = ambient.Font;
                        fontHandle = new FontHandleWrapper(font);

                        Properties.SetObject(s_fontHandleWrapperProperty, fontHandle);
                    }

                    return fontHandle.Handle;
                }

                return GetDefaultFontHandleWrapper().Handle;
            }
        }

        protected int FontHeight
        {
            get
            {
                int fontHeight = Properties.GetInteger(s_fontHeightProperty, out bool found);
                if (found && fontHeight != -1)
                {
                    return fontHeight;
                }
                else
                {
                    Font font = (Font)Properties.GetObject(s_fontProperty);
                    if (font != null)
                    {
                        fontHeight = font.Height;
                        Properties.SetInteger(s_fontHeightProperty, fontHeight);
                        return fontHeight;
                    }
                }

                //ask the parent if it has the font height
                int localFontHeight = -1;

                if (ParentInternal != null && ParentInternal.CanAccessProperties)
                {
                    localFontHeight = ParentInternal.FontHeight;
                }

                //if we still have a bad value, then get the actual font height
                if (localFontHeight == -1)
                {
                    localFontHeight = Font.Height;
                    Properties.SetInteger(s_fontHeightProperty, localFontHeight);
                }

                return localFontHeight;
            }
            set
            {
                Properties.SetInteger(s_fontHeightProperty, value);
            }
        }

        /// <summary>
        ///  The foreground color of the control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatAppearance)),
            DispId(NativeMethods.ActiveX.DISPID_FORECOLOR),
            SRDescription(nameof(SR.ControlForeColorDescr))
        ]
        public virtual Color ForeColor
        {
            get
            {
                Color color = Properties.GetColor(s_foreColorProperty);
                if (!color.IsEmpty)
                {
                    return color;
                }

                Control p = ParentInternal;
                if (p != null && p.CanAccessProperties)
                {
                    return p.ForeColor;
                }

                Color c = Color.Empty;

                if (IsActiveX)
                {
                    c = ActiveXAmbientForeColor;
                }

                if (c.IsEmpty)
                {
                    AmbientProperties ambient = AmbientPropertiesService;
                    if (ambient != null)
                    {
                        c = ambient.ForeColor;
                    }
                }

                if (!c.IsEmpty)
                {
                    return c;
                }
                else
                {
                    return DefaultForeColor;
                }
            }

            set
            {
                Color c = ForeColor;
                if (!value.IsEmpty || Properties.ContainsObject(s_foreColorProperty))
                {
                    Properties.SetColor(s_foreColorProperty, value);
                }
                if (!c.Equals(ForeColor))
                {
                    OnForeColorChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnForeColorChangedDescr))]
        public event EventHandler ForeColorChanged
        {
            add => Events.AddHandler(s_foreColorEvent, value);
            remove => Events.RemoveHandler(s_foreColorEvent, value);
        }

        private Font GetParentFont()
        {
            if (ParentInternal != null && ParentInternal.CanAccessProperties)
            {
                return ParentInternal.Font;
            }
            else
            {
                return null;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual Size GetPreferredSize(Size proposedSize)
        {
            Size prefSize;

            if (GetState(STATE_DISPOSING | STATE_DISPOSED))
            {
                // if someone's asking when we're disposing just return what we last had.
                prefSize = CommonProperties.xGetPreferredSizeCache(this);
            }
            else
            {
                // Switch Size.Empty to maximum possible values
                proposedSize = LayoutUtils.ConvertZeroToUnbounded(proposedSize);

                // Force proposedSize to be within the elements constraints.  (This applies
                // minimumSize, maximumSize, etc.)
                proposedSize = ApplySizeConstraints(proposedSize);
                if (GetState2(STATE2_USEPREFERREDSIZECACHE))
                {
                    Size cachedSize = CommonProperties.xGetPreferredSizeCache(this);

                    // If the "default" preferred size is being requested, and we have a cached value for it, return it.
                    if (!cachedSize.IsEmpty && (proposedSize == LayoutUtils.MaxSize))
                    {
                        return cachedSize;
                    }
                }

                CacheTextInternal = true;
                try
                {
                    prefSize = GetPreferredSizeCore(proposedSize);
                }
                finally
                {
                    CacheTextInternal = false;
                }

                // There is no guarantee that GetPreferredSizeCore() return something within
                // proposedSize, so we apply the element's constraints again.
                prefSize = ApplySizeConstraints(prefSize);

                // If the "default" preferred size was requested, cache the computed value.
                if (GetState2(STATE2_USEPREFERREDSIZECACHE) && proposedSize == LayoutUtils.MaxSize)
                {
                    CommonProperties.xSetPreferredSizeCache(this, prefSize);
                }
            }
            return prefSize;
        }

        // Overriding this method allows us to get the caching and clamping the proposedSize/output to
        // MinimumSize / MaximumSize from GetPreferredSize for free.
        internal virtual Size GetPreferredSizeCore(Size proposedSize)
        {
            return CommonProperties.GetSpecifiedBounds(this).Size;
        }

        /// <summary>
        ///  The HWND handle that this control is bound to. If the handle
        ///  has not yet been created, this will force handle creation.
        /// </summary>
        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            DispId(NativeMethods.ActiveX.DISPID_HWND),
            SRDescription(nameof(SR.ControlHandleDescr))
        ]
        public IntPtr Handle
        {
            get
            {
                if (CheckForIllegalCrossThreadCalls &&
                    !t_inCrossThreadSafeCall &&
                    InvokeRequired)
                {
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

        internal IntPtr HandleInternal => _window.Handle;

        /// <summary>
        ///  True if this control has child controls in its collection.  This
        ///  is more efficient than checking for Controls.Count > 0, but has the
        ///  same effect.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlHasChildrenDescr))
        ]
        public bool HasChildren
        {
            get
            {
                ControlCollection controls = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
                return controls != null && controls.Count > 0;
            }
        }

        internal virtual bool HasMenu => false;

        /// <summary>
        ///  The height of this control
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlHeightDescr))
        ]
        public int Height
        {
            get => _height;
            set => SetBounds(_x, _y, _width, value, BoundsSpecified.Height);
        }

        internal bool HostedInWin32DialogManager
        {
            get
            {
                if (!GetState(STATE_CHECKEDHOST))
                {
                    Control topMost = TopMostParent;
                    if (this != topMost)
                    {
                        SetState(STATE_HOSTEDINDIALOG, topMost.HostedInWin32DialogManager);
                    }
                    else
                    {
                        IntPtr parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(this, Handle));
                        IntPtr lastParentHandle = parentHandle;

                        StringBuilder sb = new StringBuilder(32);

                        SetState(STATE_HOSTEDINDIALOG, false);

                        while (parentHandle != IntPtr.Zero)
                        {
                            int len = UnsafeNativeMethods.GetClassName(new HandleRef(null, lastParentHandle), null, 0);
                            if (len > sb.Capacity)
                            {
                                sb.Capacity = len + 5;
                            }
                            UnsafeNativeMethods.GetClassName(new HandleRef(null, lastParentHandle), sb, sb.Capacity);

                            if (sb.ToString() == "#32770")
                            {
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

        /// <summary>
        ///  Whether or not this control has a handle associated with it.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlHandleCreatedDescr))
        ]
        public bool IsHandleCreated => _window.Handle != IntPtr.Zero;

        /// <summary>
        ///  Determines if layout is currently suspended.
        /// </summary>
        internal bool IsLayoutSuspended => _layoutSuspendCount > 0;

        internal bool IsWindowObscured
        {
            get
            {
                if (!IsHandleCreated || !Visible)
                {
                    return false;
                }

                bool emptyRegion = false;

                RECT temp = new RECT();
                Region working;
                Control parent = ParentInternal;
                if (parent != null)
                {
                    while (parent.ParentInternal != null)
                    {
                        parent = parent.ParentInternal;
                    }
                }

                UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref temp);
                working = new Region(Rectangle.FromLTRB(temp.left, temp.top, temp.right, temp.bottom));

                try
                {
                    IntPtr prev;
                    IntPtr next;
                    IntPtr start;
                    if (parent != null)
                    {
                        start = parent.Handle;
                    }
                    else
                    {
                        start = Handle;
                    }

                    for (prev = start;
                         (next = UnsafeNativeMethods.GetWindow(new HandleRef(null, prev), NativeMethods.GW_HWNDPREV)) != IntPtr.Zero;
                         prev = next)
                    {

                        UnsafeNativeMethods.GetWindowRect(new HandleRef(null, next), ref temp);
                        Rectangle current = Rectangle.FromLTRB(temp.left, temp.top, temp.right, temp.bottom);

                        if (SafeNativeMethods.IsWindowVisible(new HandleRef(null, next)))
                        {
                            working.Exclude(current);
                        }
                    }

                    Graphics g = CreateGraphics();
                    try
                    {
                        emptyRegion = working.IsEmpty(g);
                    }
                    finally
                    {
                        g.Dispose();
                    }
                }
                finally
                {
                    working.Dispose();
                }

                return emptyRegion;
            }
        }

        /// <summary>
        ///  Returns the current value of the handle. This may be zero if the handle
        ///  has not been created.
        /// </summary>
        internal IntPtr InternalHandle => !IsHandleCreated ? IntPtr.Zero : Handle;

        /// <summary>
        ///  Determines if the caller must call invoke when making method
        ///  calls to this control.  Controls in windows forms are bound to a specific thread,
        ///  and are not thread safe.  Therefore, if you are calling a control's method
        ///  from a different thread, you must use the control's invoke method
        ///  to marshal the call to the proper thread.  This function can be used to
        ///  determine if you must call invoke, which can be handy if you don't know
        ///  what thread owns a control.
        ///
        ///  There are five functions on a control that are safe to call from any
        ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and
        ///  CreateGraphics.  For all other method calls, you should use one of the
        ///  invoke methods.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlInvokeRequiredDescr))
        ]
        public bool InvokeRequired
        {
            get
            {
                using (new MultithreadSafeCallScope())
                {
                    HandleRef hwnd;
                    if (IsHandleCreated)
                    {
                        hwnd = new HandleRef(this, Handle);
                    }
                    else
                    {
                        Control marshalingControl = FindMarshalingControl();

                        if (!marshalingControl.IsHandleCreated)
                        {
                            return false;
                        }

                        hwnd = new HandleRef(marshalingControl, marshalingControl.Handle);
                    }

                    int hwndThread = SafeNativeMethods.GetWindowThreadProcessId(hwnd, out int pid);
                    int currentThread = SafeNativeMethods.GetCurrentThreadId();
                    return (hwndThread != currentThread);
                }
            }
        }

        /// <summary>
        ///  Indicates whether or not this control is an accessible control
        ///  i.e. whether it should be visible to accessibility applications.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlIsAccessibleDescr))
        ]
        public bool IsAccessible
        {
            get => GetState(STATE_ISACCESSIBLE);
            set => SetState(STATE_ISACCESSIBLE, value);
        }

        /// <summary>
        ///  Used to tell if this control is being hosted as an ActiveX control.
        /// </summary>
        internal bool IsActiveX => GetState2(STATE2_ISACTIVEX);

        // If the control on which GetContainerControl( ) is called is a ContainerControl, then we dont return the parent
        // but return the same control. This is Everett behavior so we cannot change this since this would be a breaking change.
        // Hence we have a new internal property IsContainerControl which returns false for all Everett control, but
        // this property is overidden in SplitContainer to return true so that we skip the SplitContainer
        // and the correct Parent ContainerControl is returned by GetContainerControl().
        internal virtual bool IsContainerControl => false;

        /// <summary>
        ///  Used to tell if this control is being hosted in IE.
        /// </summary>
        internal bool IsIEParent => IsActiveX ? ActiveXInstance.IsIE : false;

        /// <summary>
        ///  Used to tell if the control is mirrored
        ///  Don't call this from CreateParams. Will lead to nasty problems
        ///  since we might call CreateParams here - you dig!
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.IsMirroredDescr))
        ]
        public bool IsMirrored
        {
            get
            {
                if (!IsHandleCreated)
                {
                    CreateParams cp = CreateParams;
                    SetState(STATE_MIRRORED, (cp.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) != 0);
                }
                return GetState(STATE_MIRRORED);
            }
        }

        /// <summary>
        ///  Specifies whether the control is willing to process mnemonics when hosted in an container ActiveX (Ax Sourcing).
        /// </summary>
        internal virtual bool IsMnemonicsListenerAxSourced => false;

        /// <summary>
        ///  Used to tell if this BackColor is Supported
        /// </summary>
        private bool IsValidBackColor(Color c)
        {
            if (!c.IsEmpty && !GetStyle(ControlStyles.SupportsTransparentBackColor) && c.A < 255)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        ///  Stores information about the last button or combination pressed by the user.
        /// </summary>
        private protected static Keys LastKeyData { get; set; }

        /// <summary>
        ///  The left coordinate of this control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlLeftDescr))
        ]
        public int Left
        {
            get => _x;
            set => SetBounds(value, _y, _width, _height, BoundsSpecified.X);
        }

        /// <summary>
        ///  The location of this control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatLayout)),
            Localizable(true),
            SRDescription(nameof(SR.ControlLocationDescr))
        ]
        public Point Location
        {
            get => new Point(_x, _y);
            set => SetBounds(value.X, value.Y, _width, _height, BoundsSpecified.Location);
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnLocationChangedDescr))]
        public event EventHandler LocationChanged
        {
            add => Events.AddHandler(s_locationEvent, value);
            remove => Events.RemoveHandler(s_locationEvent, value);
        }

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

        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnMarginChangedDescr))]
        public event EventHandler MarginChanged
        {
            add => Events.AddHandler(s_marginChangedEvent, value);
            remove => Events.RemoveHandler(s_marginChangedEvent, value);
        }

        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [SRDescription(nameof(SR.ControlMaximumSizeDescr))]
        [AmbientValue(typeof(Size), "0, 0")]
        public virtual Size MaximumSize
        {
            get { return CommonProperties.GetMaximumSize(this, DefaultMaximumSize); }
            set
            {
                if (value == Size.Empty)
                {
                    CommonProperties.ClearMaximumSize(this);
                    Debug.Assert(MaximumSize == DefaultMaximumSize, "Error detected while resetting MaximumSize.");
                }
                else if (value != MaximumSize)
                {
                    // SetMaximumSize causes a layout as a side effect.
                    CommonProperties.SetMaximumSize(this, value);
                    Debug.Assert(MaximumSize == value, "Error detected while setting MaximumSize.");
                }
            }
        }

        [SRCategory(nameof(SR.CatLayout))]
        [Localizable(true)]
        [SRDescription(nameof(SR.ControlMinimumSizeDescr))]
        public virtual Size MinimumSize
        {
            get { return CommonProperties.GetMinimumSize(this, DefaultMinimumSize); }
            set
            {
                if (value != MinimumSize)
                {
                    // SetMinimumSize causes a layout as a side effect.
                    CommonProperties.SetMinimumSize(this, value);
                }
                Debug.Assert(MinimumSize == value, "Error detected while setting MinimumSize.");
            }
        }

        /// <summary>
        ///  Retrieves the current state of the modifier keys. This will check the
        ///  current state of the shift, control, and alt keys.
        /// </summary>
        public static Keys ModifierKeys
        {
            get
            {
                Keys modifiers = 0;

                if (UnsafeNativeMethods.GetKeyState((int)Keys.ShiftKey) < 0)
                {
                    modifiers |= Keys.Shift;
                }

                if (UnsafeNativeMethods.GetKeyState((int)Keys.ControlKey) < 0)
                {
                    modifiers |= Keys.Control;
                }

                if (UnsafeNativeMethods.GetKeyState((int)Keys.Menu) < 0)
                {
                    modifiers |= Keys.Alt;
                }

                return modifiers;
            }
        }

        /// <summary>
        ///  The current state of the mouse buttons. This will check the
        ///  current state of the left, right, and middle mouse buttons.
        /// </summary>
        public static MouseButtons MouseButtons
        {
            get
            {
                MouseButtons buttons = (MouseButtons)0;

                if (UnsafeNativeMethods.GetKeyState((int)Keys.LButton) < 0)
                {
                    buttons |= MouseButtons.Left;
                }

                if (UnsafeNativeMethods.GetKeyState((int)Keys.RButton) < 0)
                {
                    buttons |= MouseButtons.Right;
                }

                if (UnsafeNativeMethods.GetKeyState((int)Keys.MButton) < 0)
                {
                    buttons |= MouseButtons.Middle;
                }

                if (UnsafeNativeMethods.GetKeyState((int)Keys.XButton1) < 0)
                {
                    buttons |= MouseButtons.XButton1;
                }

                if (UnsafeNativeMethods.GetKeyState((int)Keys.XButton2) < 0)
                {
                    buttons |= MouseButtons.XButton2;
                }

                return buttons;
            }
        }

        /// <summary>
        ///  The current position of the mouse in screen coordinates.
        /// </summary>
        public static Point MousePosition
        {
            get
            {
                UnsafeNativeMethods.GetCursorPos(out Point pt);
                return pt;
            }
        }

        /// <summary>
        ///  Name of this control. The designer will set this to the same
        ///  as the programatic Id "(name)" of the control.  The name can be
        ///  used as a key into the ControlCollection.
        /// </summary>
        [Browsable(false)]
        public string Name
        {
            get
            {
                string name = (string)Properties.GetObject(s_namePropertyProperty);
                if (string.IsNullOrEmpty(name))
                {
                    if (Site != null)
                    {
                        name = Site.Name;
                    }

                    if (name == null)
                    {
                        name = string.Empty;
                    }
                }

                return name;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    Properties.SetObject(s_namePropertyProperty, null);
                }
                else
                {
                    Properties.SetObject(s_namePropertyProperty, value);
                }
            }
        }

        /// <summary>
        ///  The parent of this control.
        /// </summary>
        [
            SRCategory(nameof(SR.CatBehavior)),
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlParentDescr))
        ]
        public Control Parent
        {
            get => ParentInternal;
            set => ParentInternal = value;
        }

        internal virtual Control ParentInternal
        {
            get => _parent;
            set
            {
                if (_parent != value)
                {
                    if (value != null)
                    {
                        value.Controls.Add(this);
                    }
                    else
                    {
                        _parent.Controls.Remove(this);
                    }
                }
            }
        }

        /// <summary>
        ///  Retrieves the product name of this specific component.
        /// </summary>
        [
            Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
            SRDescription(nameof(SR.ControlProductNameDescr))
        ]
        public string ProductName => VersionInfo.ProductName;

        /// <summary>
        ///  Retrieves the product version of this specific component.
        /// </summary>
        [
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlProductVersionDescr))
        ]
        public string ProductVersion => VersionInfo.ProductVersion;

        /// <summary>
        ///  Retrieves our internal property storage object. If you have a property
        ///  whose value is not always set, you should store it in here to save
        ///  space.
        /// </summary>
        internal PropertyStore Properties { get; }

        // Returns the value of the backColor field -- no asking the parent with its color is, etc.
        internal Color RawBackColor => Properties.GetColor(s_backColorProperty);

        /// <summary>
        ///  Indicates whether the control is currently recreating its handle. This
        ///  property is read-only.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlRecreatingHandleDescr))
        ]
        public bool RecreatingHandle => (_state & STATE_RECREATE) != 0;

        internal virtual void AddReflectChild()
        {
        }

        internal virtual void RemoveReflectChild()
        {
        }

        private Control ReflectParent
        {
            get => _reflectParent;
            set
            {
                if (value != null)
                {
                    value.AddReflectChild();
                }

                Control existing = _reflectParent as Control;
                _reflectParent = value;
                existing?.RemoveReflectChild();
            }
        }

        /// <summary>
        ///  The Region associated with this control.  (defines the
        ///  outline/silhouette/boundary of control)
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlRegionDescr))
        ]
        public Region Region
        {
            get => (Region)Properties.GetObject(s_regionProperty);
            set
            {
                Region oldRegion = Region;
                if (oldRegion != value)
                {
                    Properties.SetObject(s_regionProperty, value);

                    if (oldRegion != null)
                    {
                        oldRegion.Dispose();
                    }

                    if (IsHandleCreated)
                    {
                        IntPtr regionHandle = IntPtr.Zero;

                        try
                        {
                            if (value != null)
                            {
                                regionHandle = GetHRgn(value);
                            }

                            if (IsActiveX)
                            {
                                regionHandle = ActiveXMergeRegion(regionHandle);
                            }

                            if (UnsafeNativeMethods.SetWindowRgn(new HandleRef(this, Handle), new HandleRef(this, regionHandle), SafeNativeMethods.IsWindowVisible(new HandleRef(this, Handle))) != 0)
                            {
                                //The Hwnd owns the region.
                                regionHandle = IntPtr.Zero;
                            }
                        }
                        finally
                        {
                            if (regionHandle != IntPtr.Zero)
                            {
                                Gdi32.DeleteObject(regionHandle);
                            }
                        }
                    }

                    OnRegionChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Event fired when the value of Region property is changed on Control
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlRegionChangedDescr))]
        public event EventHandler RegionChanged
        {
            add => Events.AddHandler(s_regionChangedEvent, value);
            remove => Events.RemoveHandler(s_regionChangedEvent, value);
        }

        // Helper function for Rtl
        [Obsolete("This property has been deprecated. Please use RightToLeft instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        protected internal bool RenderRightToLeft => true;

        /// <summary>
        ///  Determines if the parent's background will be rendered on the label control.
        /// </summary>
        internal bool RenderTransparent
            => GetStyle(ControlStyles.SupportsTransparentBackColor) && BackColor.A < 255;

        private bool RenderColorTransparent(Color c)
            => GetStyle(ControlStyles.SupportsTransparentBackColor) && c.A < 255;

        /// <summary>
        ///  This property is required by certain controls (TabPage) to render its transparency using theming API.
        ///  We dont want all controls (that are have transparent BackColor) to use theming API to render its background because it has  HUGE PERF cost.
        /// </summary>
        internal virtual bool RenderTransparencyWithVisualStyles => false;

        /// <summary>
        ///  Represents the bounds of the control that need to be scaled.  Control bounds
        ///  need to be scaled until ScaleControl is called.  They need to be scaled again
        ///  if their bounds change after ScaleControl is called.
        /// </summary>
        internal BoundsSpecified RequiredScaling
        {
            get
            {
                if ((_requiredScaling & RequiredScalingEnabledMask) != 0)
                {
                    return (BoundsSpecified)(_requiredScaling & RequiredScalingMask);
                }
                return BoundsSpecified.None;
            }
            set
            {
                byte enableBit = (byte)(_requiredScaling & RequiredScalingEnabledMask);
                _requiredScaling = (byte)(((int)value & RequiredScalingMask) | enableBit);
            }
        }

        /// <summary>
        ///  Determines if the required scaling property is enabled.  If not,
        ///  RequiredScaling always returns None.
        /// </summary>
        internal bool RequiredScalingEnabled
        {
            get => (_requiredScaling & RequiredScalingEnabledMask) != 0;
            set
            {
                byte scaling = (byte)(_requiredScaling & RequiredScalingMask);
                _requiredScaling = scaling;
                if (value)
                {
                    _requiredScaling |= RequiredScalingEnabledMask;
                }
            }
        }

        /// <summary>
        ///  Indicates whether the control should redraw itself when resized.
        /// </summary>
        [SRDescription(nameof(SR.ControlResizeRedrawDescr))]
        protected bool ResizeRedraw
        {
            get => GetStyle(ControlStyles.ResizeRedraw);
            set => SetStyle(ControlStyles.ResizeRedraw, value);
        }

        /// <summary>
        ///  The right coordinate of the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlRightDescr))
        ]
        public int Right => _x + _width;

        /// <summary>
        ///  This is used for international applications where the language
        ///  is written from RightToLeft. When this property is true,
        ///  control placement and text will be from right to left.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        AmbientValue(RightToLeft.Inherit),
        SRDescription(nameof(SR.ControlRightToLeftDescr))
        ]
        public virtual RightToLeft RightToLeft
        {
            get
            {
                int rightToLeft = Properties.GetInteger(s_rightToLeftProperty, out bool found);
                if (!found)
                {
                    rightToLeft = (int)RightToLeft.Inherit;
                }

                if (((RightToLeft)rightToLeft) == RightToLeft.Inherit)
                {
                    Control parent = ParentInternal;
                    if (parent != null)
                    {
                        rightToLeft = (int)parent.RightToLeft;
                    }
                    else
                    {
                        rightToLeft = (int)DefaultRightToLeft;
                    }
                }
                return (RightToLeft)rightToLeft;
            }
            set
            {
                //valid values are 0x0 to 0x2.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)RightToLeft.No, (int)RightToLeft.Inherit))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(RightToLeft));
                }

                RightToLeft oldValue = RightToLeft;

                if (Properties.ContainsInteger(s_rightToLeftProperty) || value != RightToLeft.Inherit)
                {
                    Properties.SetInteger(s_rightToLeftProperty, (int)value);
                }

                if (oldValue != RightToLeft)
                {
                    // Setting RTL on a container does not cause the container to change size.
                    // Only the children need to have thier layout updated.
                    using (new LayoutTransaction(this, this, PropertyNames.RightToLeft))
                    {
                        OnRightToLeftChanged(EventArgs.Empty);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftChangedDescr))]
        public event EventHandler RightToLeftChanged
        {
            add => Events.AddHandler(s_rightToLeftEvent, value);
            remove => Events.RemoveHandler(s_rightToLeftEvent, value);
        }

        /// <summary>
        ///  This property controls the scaling of child controls.  If true child controls
        ///  will be scaled when the Scale method on this control is called.  If false,
        ///  child controls will not be scaled.  The default is true, and you must override
        ///  this property to provide a different value.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual bool ScaleChildren => true;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public override ISite Site
        {
            get => base.Site;
            set
            {
                AmbientProperties oldAmbients = AmbientPropertiesService;
                AmbientProperties newAmbients = null;

                if (value != null)
                {
                    newAmbients = value.GetService(typeof(AmbientProperties)) as AmbientProperties;
                }

                // If the ambients changed, compare each property.
                if (oldAmbients != newAmbients)
                {
                    bool checkFont = !Properties.ContainsObject(s_fontProperty);
                    bool checkBackColor = !Properties.ContainsObject(s_backColorProperty);
                    bool checkForeColor = !Properties.ContainsObject(s_foreColorProperty);
                    bool checkCursor = !Properties.ContainsObject(s_cursorProperty);

                    Font oldFont = null;
                    Color oldBackColor = Color.Empty;
                    Color oldForeColor = Color.Empty;
                    Cursor oldCursor = null;

                    if (checkFont)
                    {
                        oldFont = Font;
                    }

                    if (checkBackColor)
                    {
                        oldBackColor = BackColor;
                    }

                    if (checkForeColor)
                    {
                        oldForeColor = ForeColor;
                    }

                    if (checkCursor)
                    {
                        oldCursor = Cursor;
                    }

                    Properties.SetObject(s_ambientPropertiesServiceProperty, newAmbients);
                    base.Site = value;

                    if (checkFont && !oldFont.Equals(Font))
                    {
                        OnFontChanged(EventArgs.Empty);
                    }
                    if (checkForeColor && !oldForeColor.Equals(ForeColor))
                    {
                        OnForeColorChanged(EventArgs.Empty);
                    }
                    if (checkBackColor && !oldBackColor.Equals(BackColor))
                    {
                        OnBackColorChanged(EventArgs.Empty);
                    }
                    if (checkCursor && oldCursor.Equals(Cursor))
                    {
                        OnCursorChanged(EventArgs.Empty);
                    }
                }
                else
                {
                    // If the ambients haven't changed, we just set a new site.
                    base.Site = value;
                }
            }
        }

        /// <summary>
        ///  The size of the control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true),
        SRDescription(nameof(SR.ControlSizeDescr))
        ]
        public Size Size
        {
            get => new Size(_width, _height);
            set => SetBounds(_x, _y, value.Width, value.Height, BoundsSpecified.Size);
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnSizeChangedDescr))]
        public event EventHandler SizeChanged
        {
            add => Events.AddHandler(s_sizeEvent, value);
            remove => Events.RemoveHandler(s_sizeEvent, value);
        }

        /// <summary>
        ///  The tab index of this control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        MergableProperty(false),
        SRDescription(nameof(SR.ControlTabIndexDescr))
        ]
        public int TabIndex
        {
            get => _tabIndex == -1 ? 0 : _tabIndex;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(TabIndex), value, 0));
                }

                if (_tabIndex != value)
                {
                    _tabIndex = value;
                    OnTabIndexChanged(EventArgs.Empty);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnTabIndexChangedDescr))]
        public event EventHandler TabIndexChanged
        {
            add => Events.AddHandler(s_tabIndexEvent, value);
            remove => Events.RemoveHandler(s_tabIndexEvent, value);
        }

        /// <summary>
        ///  Indicates whether the user can give the focus to this control using the TAB
        ///  key. This property is read-only.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        DispId(NativeMethods.ActiveX.DISPID_TABSTOP),
        SRDescription(nameof(SR.ControlTabStopDescr))
        ]
        public bool TabStop
        {
            get => TabStopInternal;
            set
            {
                if (TabStop != value)
                {
                    TabStopInternal = value;
                    if (IsHandleCreated)
                    {
                        SetWindowStyle(NativeMethods.WS_TABSTOP, value);
                    }

                    OnTabStopChanged(EventArgs.Empty);
                }
            }
        }

        // Grab out the logical of setting TABSTOP state, so that derived class could use this.
        internal bool TabStopInternal
        {
            get => (_state & STATE_TABSTOP) != 0;
            set
            {
                if (TabStopInternal != value)
                {
                    SetState(STATE_TABSTOP, value);
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnTabStopChangedDescr))]
        public event EventHandler TabStopChanged
        {
            add => Events.AddHandler(s_tabStopEvent, value);
            remove => Events.RemoveHandler(s_tabStopEvent, value);
        }

        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag
        {
            get => Properties.GetObject(s_userDataProperty);
            set => Properties.SetObject(s_userDataProperty, value);
        }

        /// <summary>
        ///  The current text associated with this control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        Bindable(true),
        DispId(NativeMethods.ActiveX.DISPID_TEXT),
        SRDescription(nameof(SR.ControlTextDescr))
        ]
        public virtual string Text
        {
            get => CacheTextInternal ? _text ?? "" : WindowText;
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (value == Text)
                {
                    return;
                }

                if (CacheTextInternal)
                {
                    _text = value;
                }

                WindowText = value;
                OnTextChanged(EventArgs.Empty);

                if (IsMnemonicsListenerAxSourced)
                {
                    for (Control ctl = this; ctl != null; ctl = ctl.ParentInternal)
                    {
                        ActiveXImpl activeXImpl = (ActiveXImpl)ctl.Properties.GetObject(s_activeXImplProperty);
                        if (activeXImpl != null)
                        {
                            activeXImpl.UpdateAccelTable();
                            break;
                        }
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnTextChangedDescr))]
        public event EventHandler TextChanged
        {
            add => Events.AddHandler(s_textEvent, value);
            remove => Events.RemoveHandler(s_textEvent, value);
        }

        /// <summary>
        ///  Top coordinate of this control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlTopDescr))
        ]
        public int Top
        {
            get => _y;
            set => SetBounds(_x, value, _width, _height, BoundsSpecified.Y);
        }

        /// <summary>
        ///  The top level control that contains this control. This doesn't
        ///  have to be the same as the value returned from getForm since forms
        ///  can be parented to other controls.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlTopLevelControlDescr))
        ]
        public Control TopLevelControl => TopLevelControlInternal;

        internal Control TopLevelControlInternal
        {
            get
            {
                Control control = this;
                while (control != null && !control.GetTopLevel())
                {
                    control = control.ParentInternal;
                }
                return control;
            }
        }

        internal Control TopMostParent
        {
            get
            {
                Control control = this;
                while (control.ParentInternal != null)
                {
                    control = control.ParentInternal;
                }
                return control;
            }
        }

        // This auto upgraged v1 client to per-process doublebuffering logic
        private BufferedGraphicsContext BufferContext => BufferedGraphicsManager.Current;

        /// <summary>
        ///  Indicates whether the user interface is in a state to show or hide keyboard
        ///  accelerators. This property is read-only.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal virtual bool ShowKeyboardCues
        {
            get
            {
                // Controls in design mode always draw their accellerators.
                if (!IsHandleCreated || DesignMode)
                {
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

                if ((_uiCuesState & UISTATE_KEYBOARD_CUES_MASK) == 0)
                {
                    if (SystemInformation.MenuAccessKeysUnderlined)
                    {
                        _uiCuesState |= UISTATE_KEYBOARD_CUES_SHOW;
                    }
                    else
                    {
                        // if we're in the hidden state, we need to manufacture an update message so everyone knows it.
                        //
                        int actionMask = NativeMethods.UISF_HIDEACCEL << 16;
                        _uiCuesState |= UISTATE_KEYBOARD_CUES_HIDDEN;

                        // The side effect of this initial state is that adding new controls may clear the accelerator
                        // state (has been this way forever)
                        UnsafeNativeMethods.SendMessage(new HandleRef(TopMostParent, TopMostParent.Handle),
                                WindowMessages.WM_CHANGEUISTATE,
                                (IntPtr)(actionMask | NativeMethods.UIS_SET),
                                IntPtr.Zero);
                    }
                }
                return (_uiCuesState & UISTATE_KEYBOARD_CUES_MASK) == UISTATE_KEYBOARD_CUES_SHOW;
            }
        }

        /// <summary>
        ///  Indicates whether the user interface is in a state to show or hide focus
        ///  rectangles. This property is read-only.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        protected internal virtual bool ShowFocusCues
        {
            get
            {
                if (!IsHandleCreated)
                {
                    return true;
                    // would be nice to query SystemParametersInfo, but have trouble
                    // getting this to work and this should not really be called before
                    // handle created anyway
                }

                // See "How this all works" in ShowKeyboardCues

                if ((_uiCuesState & UISTATE_FOCUS_CUES_MASK) == 0)
                {
                    if (SystemInformation.MenuAccessKeysUnderlined)
                    {
                        _uiCuesState |= UISTATE_FOCUS_CUES_SHOW;
                    }
                    else
                    {
                        _uiCuesState |= UISTATE_FOCUS_CUES_HIDDEN;

                        // if we're in the hidden state, we need to manufacture an update message so everyone knows it.
                        int actionMask = (NativeMethods.UISF_HIDEACCEL | NativeMethods.UISF_HIDEFOCUS) << 16;

                        // The side effect of this initial state is that adding new controls may clear the focus cue state
                        // state (has been this way forever)
                        UnsafeNativeMethods.SendMessage(new HandleRef(TopMostParent, TopMostParent.Handle),
                                WindowMessages.WM_CHANGEUISTATE,
                                (IntPtr)(actionMask | NativeMethods.UIS_SET),
                                IntPtr.Zero);

                    }
                }
                return (_uiCuesState & UISTATE_FOCUS_CUES_MASK) == UISTATE_FOCUS_CUES_SHOW;
            }
        }

        // The parameter used in the call to ShowWindow for this control
        internal virtual int ShowParams => NativeMethods.SW_SHOW;

        /// <summary>
        ///  When this property in true the Cursor Property is set to WaitCursor as well as the Cursor Property
        ///  of all the child controls.
        /// </summary>
        [
        DefaultValue(false),
        EditorBrowsable(EditorBrowsableState.Always),
        Browsable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ControlUseWaitCursorDescr)),
        ]
        public bool UseWaitCursor
        {
            get => GetState(STATE_USEWAITCURSOR);
            set
            {
                if (GetState(STATE_USEWAITCURSOR) != value)
                {
                    SetState(STATE_USEWAITCURSOR, value);
                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                    if (controlsCollection != null)
                    {
                        // PERFNOTE: This is more efficient than using Foreach.  Foreach
                        // forces the creation of an array subset enum each time we
                        // enumerate
                        for (int i = 0; i < controlsCollection.Count; i++)
                        {
                            controlsCollection[i].UseWaitCursor = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Determines whether to use compatible text rendering engine (GDI+) or not (GDI).
        ///  This property overwrites the UseCompatibleTextRenderingDefault switch when set programmatically.
        ///  Exposed publicly only by controls that support GDI text rendering (Label, LinkLabel and some others).
        ///  Observe that this property is NOT virtual (to allow for caching the property value - see LinkLabel)
        ///  and should be used by controls that support it only (see SupportsUseCompatibleTextRendering).
        /// </summary>
        internal bool UseCompatibleTextRenderingInt
        {
            get
            {
                if (Properties.ContainsInteger(s_useCompatibleTextRenderingProperty))
                {
                    int value = Properties.GetInteger(s_useCompatibleTextRenderingProperty, out bool found);
                    if (found)
                    {
                        return value == 1;
                    }
                }

                return UseCompatibleTextRenderingDefault;
            }
            set
            {
                if (SupportsUseCompatibleTextRendering && UseCompatibleTextRenderingInt != value)
                {
                    Properties.SetInteger(s_useCompatibleTextRenderingProperty, value ? 1 : 0);
                    // Update the preferred size cache since we will be rendering text using a different engine.
                    LayoutTransaction.DoLayoutIf(AutoSize, ParentInternal, this, PropertyNames.UseCompatibleTextRendering);
                    Invalidate();
                }
            }
        }

        /// <summary>
        ///  Determines whether the control supports rendering text using GDI+ and GDI.
        ///  This is provided for container controls (PropertyGrid) to iterate through its children to set
        ///  UseCompatibleTextRendering to the same value if the child control supports it.
        /// </summary>
        internal virtual bool SupportsUseCompatibleTextRendering => false;

        private ControlVersionInfo VersionInfo
        {
            get
            {
                ControlVersionInfo info = (ControlVersionInfo)Properties.GetObject(s_controlVersionInfoProperty);
                if (info == null)
                {
                    info = new ControlVersionInfo(this);
                    Properties.SetObject(s_controlVersionInfoProperty, info);
                }
                return info;
            }
        }

        /// <summary>
        ///  Indicates whether the control is visible.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ControlVisibleDescr))
        ]
        public bool Visible
        {
            get => GetVisibleCore();
            set => SetVisibleCore(value);
        }

        /// <summary>
        ///  Occurs when the control becomes visible.
        /// </summary>
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnVisibleChangedDescr))]
        public event EventHandler VisibleChanged
        {
            add => Events.AddHandler(s_visibleEvent, value);
            remove => Events.RemoveHandler(s_visibleEvent, value);
        }

        /// <summary>
        ///  Wait for the wait handle to receive a signal: throw an exception if the thread is no longer with us.
        /// </summary>
        private void WaitForWaitHandle(WaitHandle waitHandle)
        {
            int threadId = CreateThreadId;
            Application.ThreadContext ctx = Application.ThreadContext.FromId(threadId);
            if (ctx == null)
            {
                // Couldn't find the thread context, so we don't know the state.  We shouldn't throw.
                return;
            }
            IntPtr threadHandle = ctx.GetHandle();
            bool processed = false;
            // setting default exitcode to 0, though it won't be accessed in current code below due to short-circuit logic in condition (returnValue will be false when exitCode is undefined)
            uint exitCode = 0;
            bool returnValue = false;
            while (!processed)
            {
                //Get the thread's exit code, if we found the thread as expected
                if (threadHandle != null)
                {
                    returnValue = UnsafeNativeMethods.GetExitCodeThread(threadHandle, out exitCode);
                }
                //If we didn't find the thread, or if GetExitCodeThread failed, we don't know the thread's state:
                //if we don't know, we shouldn't throw.
                if ((returnValue && exitCode != NativeMethods.STILL_ACTIVE) ||
                    (!returnValue && Marshal.GetLastWin32Error() == NativeMethods.ERROR_INVALID_HANDLE) ||
                    AppDomain.CurrentDomain.IsFinalizingForUnload())
                {
                    if (waitHandle.WaitOne(1, false))
                    {
                        break;
                    }
                    throw new InvalidAsynchronousStateException(SR.ThreadNoLongerValid);
                }

                if (IsDisposed && _threadCallbackList != null && _threadCallbackList.Count > 0)
                {
                    lock (_threadCallbackList)
                    {
                        Exception ex = new ObjectDisposedException(GetType().Name);
                        while (_threadCallbackList.Count > 0)
                        {
                            ThreadMethodEntry entry = (ThreadMethodEntry)_threadCallbackList.Dequeue();
                            entry._exception = ex;
                            entry.Complete();
                        }
                    }
                }

                processed = waitHandle.WaitOne(1000, false);
            }
        }

        /// <summary>
        ///  The width of this control.
        /// </summary>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlWidthDescr))
        ]
        public int Width
        {
            get => _width;
            set => SetBounds(_x, _y, value, _height, BoundsSpecified.Width);
        }

        /// <summary>
        ///  The current exStyle of the hWnd
        /// </summary>
        private int WindowExStyle
        {
            get
            {
                return unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_EXSTYLE));
            }
            set
            {
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_EXSTYLE, new HandleRef(null, (IntPtr)value));
            }
        }

        /// <summary>
        ///  The current style of the hWnd
        /// </summary>
        internal int WindowStyle
        {
            get
            {
                return unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE));
            }
            set
            {
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)value));
            }
        }

        /// <summary>
        ///  The target of Win32 window messages.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlWindowTargetDescr))
        ]
        public IWindowTarget WindowTarget
        {
            get => _window.WindowTarget;
            set => _window.WindowTarget = value;
        }

        /// <summary>
        ///  The current text of the Window; if the window has not yet been created, stores it in the control.
        ///  If the window has been created, stores the text in the underlying win32 control.
        ///  This property should be used whenever you want to get at the win32 control's text. For all other cases,
        ///  use the Text property - but note that this is overridable, and any of your code that uses it will use
        ///  the overridden version in controls that subclass your own.
        /// </summary>
        internal virtual string WindowText
        {
            get
            {
                if (!IsHandleCreated)
                {
                    if (_text == null)
                    {
                        return "";
                    }
                    else
                    {
                        return _text;
                    }
                }

                using (new MultithreadSafeCallScope())
                {
                    return User32.GetWindowText(new HandleRef(_window, Handle));
                }
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (!WindowText.Equals(value))
                {
                    if (IsHandleCreated)
                    {
                        User32.SetWindowTextW(new HandleRef(_window, Handle), value);
                    }
                    else
                    {
                        if (value.Length == 0)
                        {
                            _text = null;
                        }
                        else
                        {
                            _text = value;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///  Occurs when the control is clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnClickDescr))]
        public event EventHandler Click
        {
            add => Events.AddHandler(s_clickEvent, value);
            remove => Events.RemoveHandler(s_clickEvent, value);
        }

        /// <summary>
        ///  Occurs when a new control is added.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.ControlOnControlAddedDescr))
        ]
        public event ControlEventHandler ControlAdded
        {
            add => Events.AddHandler(s_controlAddedEvent, value);
            remove => Events.RemoveHandler(s_controlAddedEvent, value);
        }

        /// <summary>
        ///  Occurs when a control is removed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(true),
        EditorBrowsable(EditorBrowsableState.Advanced),
        SRDescription(nameof(SR.ControlOnControlRemovedDescr))
        ]
        public event ControlEventHandler ControlRemoved
        {
            add => Events.AddHandler(s_controlRemovedEvent, value);
            remove => Events.RemoveHandler(s_controlRemovedEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragDropDescr))]
        public event DragEventHandler DragDrop
        {
            add => Events.AddHandler(s_dragDropEvent, value);
            remove => Events.RemoveHandler(s_dragDropEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragEnterDescr))]
        public event DragEventHandler DragEnter
        {
            add => Events.AddHandler(s_dragEnterEvent, value);
            remove => Events.RemoveHandler(s_dragEnterEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragOverDescr))]
        public event DragEventHandler DragOver
        {
            add => Events.AddHandler(s_dragOverEvent, value);
            remove => Events.RemoveHandler(s_dragOverEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnDragLeaveDescr))]
        public event EventHandler DragLeave
        {
            add => Events.AddHandler(s_dragLeaveEvent, value);
            remove => Events.RemoveHandler(s_dragLeaveEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnGiveFeedbackDescr))]
        public event GiveFeedbackEventHandler GiveFeedback
        {
            add => Events.AddHandler(s_giveFeedbackEvent, value);
            remove => Events.RemoveHandler(s_giveFeedbackEvent, value);
        }

        /// <summary>
        ///  Occurs when a handle is created for the control.
        /// </summary>
        [SRCategory(nameof(SR.CatPrivate)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), SRDescription(nameof(SR.ControlOnCreateHandleDescr))]
        public event EventHandler HandleCreated
        {
            add => Events.AddHandler(s_handleCreatedEvent, value);
            remove => Events.RemoveHandler(s_handleCreatedEvent, value);
        }

        /// <summary>
        ///  Occurs when the control's handle is destroyed.
        /// </summary>
        [SRCategory(nameof(SR.CatPrivate)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), SRDescription(nameof(SR.ControlOnDestroyHandleDescr))]
        public event EventHandler HandleDestroyed
        {
            add => Events.AddHandler(s_handleDestroyedEvent, value);
            remove => Events.RemoveHandler(s_handleDestroyedEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnHelpDescr))]
        public event HelpEventHandler HelpRequested
        {
            add => Events.AddHandler(s_helpRequestedEvent, value);
            remove => Events.RemoveHandler(s_helpRequestedEvent, value);
        }

        [SRCategory(nameof(SR.CatAppearance)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced), SRDescription(nameof(SR.ControlOnInvalidateDescr))]
        public event InvalidateEventHandler Invalidated
        {
            add => Events.AddHandler(s_invalidatedEvent, value);
            remove => Events.RemoveHandler(s_invalidatedEvent, value);
        }

        [Browsable(false)]
        public Size PreferredSize
        {
            get { return GetPreferredSize(Size.Empty); }
        }

        [
        SRDescription(nameof(SR.ControlPaddingDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Localizable(true)
        ]
        public Padding Padding
        {
            get { return CommonProperties.GetPadding(this, DefaultPadding); }
            set
            {
                if (value != Padding)
                {
                    CommonProperties.SetPadding(this, value);
                    // Ideally we are being laid out by a LayoutEngine that cares about our preferred size.
                    // We set our LAYOUTISDIRTY bit and ask our parent to refresh us.
                    SetState(STATE_LAYOUTISDIRTY, true);
                    using (new LayoutTransaction(ParentInternal, this, PropertyNames.Padding))
                    {
                        OnPaddingChanged(EventArgs.Empty);
                    }

                    if (GetState(STATE_LAYOUTISDIRTY))
                    {
                        // The above did not cause our layout to be refreshed.  We explicitly refresh our
                        // layout to ensure that any children are repositioned to account for the change
                        // in padding.
                        LayoutTransaction.DoLayout(this, this, PropertyNames.Padding);
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnPaddingChangedDescr))]
        public event EventHandler PaddingChanged
        {
            add => Events.AddHandler(s_paddingChangedEvent, value);
            remove => Events.RemoveHandler(s_paddingChangedEvent, value);
        }

        [SRCategory(nameof(SR.CatAppearance)), SRDescription(nameof(SR.ControlOnPaintDescr))]
        public event PaintEventHandler Paint
        {
            add => Events.AddHandler(s_paintEvent, value);
            remove => Events.RemoveHandler(s_paintEvent, value);
        }

        [SRCategory(nameof(SR.CatDragDrop)), SRDescription(nameof(SR.ControlOnQueryContinueDragDescr))]
        public event QueryContinueDragEventHandler QueryContinueDrag
        {
            add => Events.AddHandler(s_queryContinueDragEvent, value);
            remove => Events.RemoveHandler(s_queryContinueDragEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnQueryAccessibilityHelpDescr))]
        public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp
        {
            add => Events.AddHandler(s_queryAccessibilityHelpEvent, value);
            remove => Events.RemoveHandler(s_queryAccessibilityHelpEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is double clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnDoubleClickDescr))]
        public event EventHandler DoubleClick
        {
            add => Events.AddHandler(s_doubleClickEvent, value);
            remove => Events.RemoveHandler(s_doubleClickEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is entered.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnEnterDescr))]
        public event EventHandler Enter
        {
            add => Events.AddHandler(s_enterEvent, value);
            remove => Events.RemoveHandler(s_enterEvent, value);
        }

        /// <summary>
        ///  Occurs when the control receives focus.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnGotFocusDescr)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler GotFocus
        {
            add => Events.AddHandler(s_gotFocusEvent, value);
            remove => Events.RemoveHandler(s_gotFocusEvent, value);
        }

        /// <summary>
        ///  Occurs when a key is pressed down while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyDownDescr))]
        public event KeyEventHandler KeyDown
        {
            add => Events.AddHandler(s_keyDownEvent, value);
            remove => Events.RemoveHandler(s_keyDownEvent, value);
        }

        /// <summary>
        ///  Occurs when a key is pressed while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyPressDescr))]
        public event KeyPressEventHandler KeyPress
        {
            add => Events.AddHandler(s_keyPressEvent, value);
            remove => Events.RemoveHandler(s_keyPressEvent, value);
        }

        /// <summary>
        ///  Occurs when a key is released while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.ControlOnKeyUpDescr))]
        public event KeyEventHandler KeyUp
        {
            add => Events.AddHandler(s_keyUpEvent, value);
            remove => Events.RemoveHandler(s_keyUpEvent, value);
        }

        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnLayoutDescr))]
        public event LayoutEventHandler Layout
        {
            add => Events.AddHandler(s_layoutEvent, value);
            remove => Events.RemoveHandler(s_layoutEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is left.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnLeaveDescr))]
        public event EventHandler Leave
        {
            add => Events.AddHandler(s_leaveEvent, value);
            remove => Events.RemoveHandler(s_leaveEvent, value);
        }

        /// <summary>
        ///  Occurs when the control loses focus.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnLostFocusDescr)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler LostFocus
        {
            add => Events.AddHandler(s_lostFocusEvent, value);
            remove => Events.RemoveHandler(s_lostFocusEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is mouse clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnMouseClickDescr))]
        public event MouseEventHandler MouseClick
        {
            add => Events.AddHandler(s_mouseClickEvent, value);
            remove => Events.RemoveHandler(s_mouseClickEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is mouse double clicked.
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnMouseDoubleClickDescr))]
        public event MouseEventHandler MouseDoubleClick
        {
            add => Events.AddHandler(s_mouseDoubleClickEvent, value);
            remove => Events.RemoveHandler(s_mouseDoubleClickEvent, value);
        }

        /// <summary>
        ///  Occurs when the control loses mouse Capture.
        /// </summary>
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ControlOnMouseCaptureChangedDescr))]
        public event EventHandler MouseCaptureChanged
        {
            add => Events.AddHandler(s_mouseCaptureChangedEvent, value);
            remove => Events.RemoveHandler(s_mouseCaptureChangedEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is
        ///  pressed.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseDownDescr))]
        public event MouseEventHandler MouseDown
        {
            add => Events.AddHandler(s_mouseDownEvent, value);
            remove => Events.RemoveHandler(s_mouseDownEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer enters the control.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseEnterDescr))]
        public event EventHandler MouseEnter
        {
            add => Events.AddHandler(s_mouseEnterEvent, value);
            remove => Events.RemoveHandler(s_mouseEnterEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer leaves the control.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseLeaveDescr))]
        public event EventHandler MouseLeave
        {
            add => Events.AddHandler(s_mouseLeaveEvent, value);
            remove => Events.RemoveHandler(s_mouseLeaveEvent, value);
        }

        /// <summary>
        ///  Occurs when the DPI resolution of the screen this control is displayed on changes,
        ///  either when the top level window is moved between monitors or when the OS settings are changed.
        ///  This event is raised before the top level parent window recieves WM_DPICHANGED message.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnDpiChangedBeforeParentDescr))]
        public event EventHandler DpiChangedBeforeParent
        {
            add => Events.AddHandler(s_dpiChangedBeforeParentEvent, value);
            remove => Events.RemoveHandler(s_dpiChangedBeforeParentEvent, value);
        }

        /// <summary>
        ///  Occurs when the DPI resolution of the screen this control is displayed on changes,
        ///  either when the top level window is moved between monitors or when the OS settings are changed.
        ///  This message is received after the top levet parent window recieves WM_DPICHANGED message.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnDpiChangedAfterParentDescr))]
        public event EventHandler DpiChangedAfterParent
        {
            add => Events.AddHandler(s_dpiChangedAfterParentEvent, value);
            remove => Events.RemoveHandler(s_dpiChangedAfterParentEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer hovers over the contro.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseHoverDescr))]
        public event EventHandler MouseHover
        {
            add => Events.AddHandler(s_mouseHoverEvent, value);
            remove => Events.RemoveHandler(s_mouseHoverEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is moved over the control.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseMoveDescr))]
        public event MouseEventHandler MouseMove
        {
            add => Events.AddHandler(s_mouseMoveEvent, value);
            remove => Events.RemoveHandler(s_mouseMoveEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse pointer is over the control and a mouse button is released.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseUpDescr))]
        public event MouseEventHandler MouseUp
        {
            add => Events.AddHandler(s_mouseUpEvent, value);
            remove => Events.RemoveHandler(s_mouseUpEvent, value);
        }

        /// <summary>
        ///  Occurs when the mouse wheel moves while the control has focus.
        /// </summary>
        [SRCategory(nameof(SR.CatMouse)), SRDescription(nameof(SR.ControlOnMouseWheelDescr)), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public event MouseEventHandler MouseWheel
        {
            add => Events.AddHandler(s_mouseWheelEvent, value);
            remove => Events.RemoveHandler(s_mouseWheelEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is moved.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnMoveDescr))]
        public event EventHandler Move
        {
            add => Events.AddHandler(s_moveEvent, value);
            remove => Events.RemoveHandler(s_moveEvent, value);
        }

        /// <summary>
        ///  Raised to preview a key down event
        /// </summary>
        [SRCategory(nameof(SR.CatKey)), SRDescription(nameof(SR.PreviewKeyDownDescr))]
        public event PreviewKeyDownEventHandler PreviewKeyDown
        {
            add => Events.AddHandler(s_previewKeyDownEvent, value);
            remove => Events.RemoveHandler(s_previewKeyDownEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is resized.
        /// </summary>
        [SRCategory(nameof(SR.CatLayout)), SRDescription(nameof(SR.ControlOnResizeDescr)),
         EditorBrowsable(EditorBrowsableState.Advanced)]
        public event EventHandler Resize
        {
            add => Events.AddHandler(s_resizeEvent, value);
            remove => Events.RemoveHandler(s_resizeEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnChangeUICuesDescr))]
        public event UICuesEventHandler ChangeUICues
        {
            add => Events.AddHandler(s_changeUICuesEvent, value);
            remove => Events.RemoveHandler(s_changeUICuesEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnStyleChangedDescr))]
        public event EventHandler StyleChanged
        {
            add => Events.AddHandler(s_styleChangedEvent, value);
            remove => Events.RemoveHandler(s_styleChangedEvent, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ControlOnSystemColorsChangedDescr))]
        public event EventHandler SystemColorsChanged
        {
            add => Events.AddHandler(s_systemColorsChangedEvent, value);
            remove => Events.RemoveHandler(s_systemColorsChangedEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is validating.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnValidatingDescr))]
        public event CancelEventHandler Validating
        {
            add => Events.AddHandler(s_validatingEvent, value);
            remove => Events.RemoveHandler(s_validatingEvent, value);
        }

        /// <summary>
        ///  Occurs when the control is done validating.
        /// </summary>
        [SRCategory(nameof(SR.CatFocus)), SRDescription(nameof(SR.ControlOnValidatedDescr))]
        public event EventHandler Validated
        {
            add => Events.AddHandler(s_validatedEvent, value);
            remove => Events.RemoveHandler(s_validatedEvent, value);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal void AccessibilityNotifyClients(AccessibleEvents accEvent, int childID)
        {
            AccessibilityNotifyClients(accEvent, NativeMethods.OBJID_CLIENT, childID);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void AccessibilityNotifyClients(AccessibleEvents accEvent, int objectID, int childID)
        {
            if (IsHandleCreated)
            {
                UnsafeNativeMethods.NotifyWinEvent((int)accEvent, new HandleRef(this, Handle), objectID, childID + 1);
            }
        }

        /// <summary>
        ///  Helper method for retrieving an ActiveX property.  We abstract these
        ///  to another method so we do not force JIT the ActiveX codebase.
        /// </summary>
        private IntPtr ActiveXMergeRegion(IntPtr region)
        {
            return ActiveXInstance.MergeRegion(region);
        }

        /// <summary>
        ///  Helper method for retrieving an ActiveX property.  We abstract these
        ///  to another method so we do not force JIT the ActiveX codebase.
        /// </summary>
        private void ActiveXOnFocus(bool focus)
        {
            ActiveXInstance.OnFocus(focus);
        }

        /// <summary>
        ///  Helper method for retrieving an ActiveX property.  We abstract these
        ///  to another method so we do not force JIT the ActiveX codebase.
        /// </summary>
        private void ActiveXViewChanged()
        {
            ActiveXInstance.ViewChangedInternal();
        }

        /// <summary>
        ///  Helper method for retrieving an ActiveX property.  We abstract these
        ///  to another method so we do not force JIT the ActiveX codebase.
        /// </summary>
        private void ActiveXUpdateBounds(ref int x, ref int y, ref int width, ref int height, int flags)
        {
            ActiveXInstance.UpdateBounds(ref x, ref y, ref width, ref height, flags);
        }

        /// <summary>
        ///  Assigns a new parent control. Sends out the appropriate property change
        ///  notifications for properties that are affected by the change of parent.
        /// </summary>
        internal virtual void AssignParent(Control value)
        {
            // Adopt the parent's required scaling bits
            if (value != null)
            {
                RequiredScalingEnabled = value.RequiredScalingEnabled;
            }

            if (CanAccessProperties)
            {
                // Store the old values for these properties
                Font oldFont = Font;
                Color oldForeColor = ForeColor;
                Color oldBackColor = BackColor;
                RightToLeft oldRtl = RightToLeft;
                bool oldEnabled = Enabled;
                bool oldVisible = Visible;

                // Update the parent
                _parent = value;
                OnParentChanged(EventArgs.Empty);

                if (GetAnyDisposingInHierarchy())
                {
                    return;
                }

                // Compare property values with new parent to old values
                if (oldEnabled != Enabled)
                {
                    OnEnabledChanged(EventArgs.Empty);
                }

                // When a control seems to be going from invisible -> visible,
                // yet its parent is being set to null and it's not top level, do not raise OnVisibleChanged.
                bool newVisible = Visible;

                if (oldVisible != newVisible && !(!oldVisible && newVisible && _parent == null && !GetTopLevel()))
                {
                    OnVisibleChanged(EventArgs.Empty);
                }
                if (!oldFont.Equals(Font))
                {
                    OnFontChanged(EventArgs.Empty);
                }
                if (!oldForeColor.Equals(ForeColor))
                {
                    OnForeColorChanged(EventArgs.Empty);
                }
                if (!oldBackColor.Equals(BackColor))
                {
                    OnBackColorChanged(EventArgs.Empty);
                }
                if (oldRtl != RightToLeft)
                {
                    OnRightToLeftChanged(EventArgs.Empty);
                }
                if (Properties.GetObject(s_bindingManagerProperty) == null && Created)
                {
                    // We do not want to call our parent's BindingContext property here.
                    // We have no idea if us or any of our children are using data binding,
                    // and invoking the property would just create the binding manager, which
                    // we don't need.  We just blindly notify that the binding manager has
                    // changed, and if anyone cares, they will do the comparison at that time.
                    //
                    OnBindingContextChanged(EventArgs.Empty);
                }
            }
            else
            {
                _parent = value;
                OnParentChanged(EventArgs.Empty);
            }

            SetState(STATE_CHECKEDHOST, false);
            if (ParentInternal != null)
            {
                ParentInternal.LayoutEngine.InitLayout(this, BoundsSpecified.All);
            }
        }

        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnParentChangedDescr))]
        public event EventHandler ParentChanged
        {
            add => Events.AddHandler(s_parentEvent, value);
            remove => Events.RemoveHandler(s_parentEvent, value);
        }

        /// <summary>
        ///  Executes the given delegate on the thread that owns this Control's
        ///  underlying window handle.  The delegate is called asynchronously and this
        ///  method returns immediately.  You may call this from any thread, even the
        ///  thread that owns the control's handle.  If the control's handle doesn't
        ///  exist yet, this will follow up the control's parent chain until it finds a
        ///  control or form that does have a window handle.  If no appropriate handle
        ///  can be found, BeginInvoke will throw an exception.  Exceptions within the
        ///  delegate method are considered untrapped and will be sent to the
        ///  application's untrapped exception handler.
        ///
        ///  There are five functions on a control that are safe to call from any
        ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///  For all other method calls, you should use one of the invoke methods to marshal
        ///  the call to the control's thread.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginInvoke(Delegate method)
        {
            return BeginInvoke(method, null);
        }

        /// <summary>
        ///  Executes the given delegate on the thread that owns this Control's
        ///  underlying window handle.  The delegate is called asynchronously and this
        ///  method returns immediately.  You may call this from any thread, even the
        ///  thread that owns the control's handle.  If the control's handle doesn't
        ///  exist yet, this will follow up the control's parent chain until it finds a
        ///  control or form that does have a window handle.  If no appropriate handle
        ///  can be found, BeginInvoke will throw an exception.  Exceptions within the
        ///  delegate method are considered untrapped and will be sent to the
        ///  application's untrapped exception handler.
        ///
        ///  There are five functions on a control that are safe to call from any
        ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///  For all other method calls, you should use one of the invoke methods to marshal
        ///  the call to the control's thread.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public IAsyncResult BeginInvoke(Delegate method, params object[] args)
        {
            using (new MultithreadSafeCallScope())
            {
                Control marshaler = FindMarshalingControl();
                return (IAsyncResult)marshaler.MarshaledInvoke(this, method, args, false);
            }
        }

        internal void BeginUpdateInternal()
        {
            if (!IsHandleCreated)
            {
                return;
            }
            if (_updateCount == 0)
            {
                SendMessage(WindowMessages.WM_SETREDRAW, 0, 0);
            }

            _updateCount++;
        }

        /// <summary>
        ///  Brings this control to the front of the zorder.
        /// </summary>
        public void BringToFront()
        {
            if (_parent != null)
            {
                _parent.Controls.SetChildIndex(this, 0);
            }
            else if (IsHandleCreated && GetTopLevel() && SafeNativeMethods.IsWindowEnabled(new HandleRef(_window, Handle)))
            {
                SafeNativeMethods.SetWindowPos(
                    new HandleRef(_window, Handle),
                    NativeMethods.HWND_TOP,
                    flags: NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            }
        }

        /// <summary>
        ///  Specifies whether this control can process the mnemonic or not.  A condition to process a mnemonic is that
        ///  all controls in the parent chain can do it too, but since the semantics for this function can be overriden,
        ///  we need to call the method on the parent 'recursively' (not exactly since it is not necessarily the same method).
        /// </summary>
        internal virtual bool CanProcessMnemonic()
        {
#if DEBUG
            TraceCanProcessMnemonic();
#endif
            if (!Enabled || !Visible)
            {
                return false;
            }

            if (_parent != null)
            {
                return _parent.CanProcessMnemonic();
            }

            return true;
        }

        // Package scope to allow AxHost to override
        internal virtual bool CanSelectCore()
        {
            if ((_controlStyle & ControlStyles.Selectable) != ControlStyles.Selectable)
            {
                return false;
            }

            for (Control ctl = this; ctl != null; ctl = ctl._parent)
            {
                if (!ctl.Enabled || !ctl.Visible)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Searches the parent/owner tree for bottom to find any instance
        ///  of toFind in the parent/owner tree.
        /// </summary>
        internal static void CheckParentingCycle(Control bottom, Control toFind)
        {
            Form lastOwner = null;
            Control lastParent = null;

            for (Control ctl = bottom; ctl != null; ctl = ctl.ParentInternal)
            {
                lastParent = ctl;
                if (ctl == toFind)
                {
                    throw new ArgumentException(SR.CircularOwner);
                }
            }

            if (lastParent != null)
            {
                if (lastParent is Form f)
                {
                    for (Form form = f; form != null; form = form.OwnerInternal)
                    {
                        lastOwner = form;
                        if (form == toFind)
                        {
                            throw new ArgumentException(SR.CircularOwner);
                        }
                    }
                }
            }

            if (lastOwner != null)
            {
                if (lastOwner.ParentInternal != null)
                {
                    CheckParentingCycle(lastOwner.ParentInternal, toFind);
                }
            }
        }

        private void ChildGotFocus(Control child)
        {
            if (IsActiveX)
            {
                ActiveXOnFocus(true);
            }
            if (_parent != null)
            {
                _parent.ChildGotFocus(child);
            }
        }

        /// <summary>
        ///  Verifies if a control is a child of this control.
        /// </summary>
        public bool Contains(Control ctl)
        {
            while (ctl != null)
            {
                ctl = ctl.ParentInternal;
                if (ctl == null)
                {
                    return false;
                }
                if (ctl == this)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///  constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual AccessibleObject CreateAccessibilityInstance()
        {
            return new ControlAccessibleObject(this);
        }

        /// <summary>
        ///  Constructs the new instance of the Controls collection objects. Subclasses
        ///  should not call base.CreateControlsInstance.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual ControlCollection CreateControlsInstance()
        {
            return new ControlCollection(this);
        }

        /// <summary>
        ///  Creates a Graphics for this control. The control's brush, font, foreground
        ///  color and background color become the default values for the Graphics.
        ///  The returned Graphics must be disposed through a call to its dispose()
        ///  method when it is no longer needed.  The Graphics Object is only valid for
        ///  the duration of the current window's message.
        /// </summary>
        public Graphics CreateGraphics()
        {
            using (new MultithreadSafeCallScope())
            {
                return CreateGraphicsInternal();
            }
        }

        internal Graphics CreateGraphicsInternal()
        {
            return Graphics.FromHwndInternal(Handle);
        }

        /// <summary>
        ///  Creates a handle for this control. This method is called by the framework, this should
        ///  not be called directly. Inheriting classes should always call <c>base.CreateHandle()</c> when
        ///  overriding this method.
        /// </summary>
        [
        EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void CreateHandle()
        {
            IntPtr userCookie = IntPtr.Zero;

            if (GetState(STATE_DISPOSED))
            {
                throw new ObjectDisposedException(GetType().Name);
            }

            if (GetState(STATE_CREATINGHANDLE))
            {
                return;
            }

            Rectangle originalBounds;

            try
            {
                SetState(STATE_CREATINGHANDLE, true);

                originalBounds = Bounds;

                if (Application.UseVisualStyles)
                {
                    // Activate theming scope to get theming for controls at design time and when hosted in browser.
                    // NOTE: If a theming context is already active, this call is very fast, so shouldn't be a perf issue.
                    userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                }

                CreateParams cp = CreateParams;
                SetState(STATE_MIRRORED, (cp.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) != 0);

                // Adjust for scrolling of parent...
                if (_parent != null)
                {
                    Rectangle parentClient = _parent.ClientRectangle;

                    if (!parentClient.IsEmpty)
                    {
                        if (cp.X != NativeMethods.CW_USEDEFAULT)
                        {
                            cp.X -= parentClient.X;
                        }
                        if (cp.Y != NativeMethods.CW_USEDEFAULT)
                        {
                            cp.Y -= parentClient.Y;
                        }
                    }
                }

                // And if we are WS_CHILD, ensure we have a parent handle.
                if (cp.Parent == IntPtr.Zero && (cp.Style & NativeMethods.WS_CHILD) != 0)
                {
                    Debug.Assert((cp.ExStyle & NativeMethods.WS_EX_MDICHILD) == 0, "Can't put MDI child forms on the parking form");
                    Application.ParkHandle(cp);
                }

                _window.CreateHandle(cp);

                UpdateReflectParent(true);

            }
            finally
            {
                SetState(STATE_CREATINGHANDLE, false);
                UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
            }

            // For certain controls (e.g., ComboBox) CreateWindowEx
            // may cause the control to resize.  WM_SETWINDOWPOSCHANGED takes care of
            // the control being resized, but our layout container may need a refresh as well.
            if (Bounds != originalBounds)
            {
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
            }
        }

        /// <summary>
        ///  Forces the creation of the control. This includes the creation of the handle,
        ///  and any child controls.
        /// </summary>
        public void CreateControl()
        {
            bool controlIsAlreadyCreated = Created;
            CreateControl(false);

            if (Properties.GetObject(s_bindingManagerProperty) == null && ParentInternal != null && !controlIsAlreadyCreated)
            {
                // We do not want to call our parent's BindingContext property here.
                // We have no idea if us or any of our children are using data binding,
                // and invoking the property would just create the binding manager, which
                // we don't need.  We just blindly notify that the binding manager has
                // changed, and if anyone cares, they will do the comparison at that time.
                OnBindingContextChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Forces the creation of the control. This includes the creation of the handle,
        ///  and any child controls.
        /// <param name='fIgnoreVisible'>
        ///  Determines whether we should create the handle after checking the Visible
        ///  property of the control or not.
        /// </param>
        /// </summary>
        internal void CreateControl(bool fIgnoreVisible)
        {
            bool ready = (_state & STATE_CREATED) == 0;

            // PERF: Only "create" the control if it is
            //     : visible. This has the effect of delayed handle creation of
            //     : hidden controls.
            ready = ready && Visible;

            if (ready || fIgnoreVisible)
            {
                _state |= STATE_CREATED;
                bool createdOK = false;
                try
                {
                    if (!IsHandleCreated)
                    {
                        CreateHandle();
                    }

                    // must snapshot this array because
                    // z-order updates from Windows may rearrange it!
                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                    if (controlsCollection != null)
                    {
                        Control[] controlSnapshot = new Control[controlsCollection.Count];
                        controlsCollection.CopyTo(controlSnapshot, 0);

                        foreach (Control ctl in controlSnapshot)
                        {
                            if (ctl.IsHandleCreated)
                            {
                                ctl.SetParentHandle(Handle);
                            }
                            ctl.CreateControl(fIgnoreVisible);
                        }
                    }

                    createdOK = true;
                }
                finally
                {
                    if (!createdOK)
                    {
                        _state &= (~STATE_CREATED);
                    }
                }
                OnCreateControl();
            }
        }

        /// <summary>
        ///  Sends the message to the default window proc.
        /// </summary>
        /// <remarks>Primarily here for Form to override.</remarks>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void DefWndProc(ref Message m)
        {
            _window.DefWndProc(ref m);
        }

        /// <summary>
        ///  Destroys the handle associated with this control. Inheriting classes should
        ///  always call base.destroyHandle.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void DestroyHandle()
        {
            if (RecreatingHandle)
            {
                if (_threadCallbackList != null)
                {
                    // See if we have a thread marshaling request pending.  If so, we will need to
                    // re-post it after recreating the handle.
                    //
                    lock (_threadCallbackList)
                    {
                        if (s_threadCallbackMessage != 0)
                        {
                            NativeMethods.MSG msg = new NativeMethods.MSG();
                            if (UnsafeNativeMethods.PeekMessage(ref msg, new HandleRef(this, Handle), s_threadCallbackMessage,
                                                          s_threadCallbackMessage, NativeMethods.PM_NOREMOVE))
                            {

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
            if (!RecreatingHandle)
            {
                if (_threadCallbackList != null)
                {
                    lock (_threadCallbackList)
                    {
                        Exception ex = new ObjectDisposedException(GetType().Name);

                        while (_threadCallbackList.Count > 0)
                        {
                            ThreadMethodEntry entry = (ThreadMethodEntry)_threadCallbackList.Dequeue();
                            entry._exception = ex;
                            entry.Complete();
                        }
                    }
                }
            }

            if (0 != (NativeMethods.WS_EX_MDICHILD & (int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(_window, InternalHandle), NativeMethods.GWL_EXSTYLE)))
            {
                UnsafeNativeMethods.DefMDIChildProc(InternalHandle, WindowMessages.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            }
            else
            {
                _window.DestroyHandle();
            }

            _trackMouseEvent = null;
        }

        /// <summary>
        ///  Disposes of the resources (other than memory) used by the
        ///  <see cref='Control'/>.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (GetState(STATE_OWNCTLBRUSH))
            {
                object backBrush = Properties.GetObject(s_backBrushProperty);
                if (backBrush != null)
                {
                    IntPtr p = (IntPtr)backBrush;
                    if (p != IntPtr.Zero)
                    {
                        Gdi32.DeleteObject(p);
                    }
                    Properties.SetObject(s_backBrushProperty, null);
                }
            }

            //set reflectparent = null regardless of whether we are in the finalizer thread or not.
            UpdateReflectParent(false);
            if (disposing)
            {
                if (GetState(STATE_DISPOSING))
                {
                    return;
                }

                if (GetState(STATE_CREATINGHANDLE))
                {
                    throw new InvalidOperationException(string.Format(SR.ClosingWhileCreatingHandle, "Dispose"));
                    // I imagine most subclasses will get themselves in a half disposed state
                    // if this exception is thrown, but things will be equally broken if we ignore this error,
                    // and this way at least the user knows what they did wrong.
                }

                SetState(STATE_DISPOSING, true);
                SuspendLayout();
                try
                {
                    DisposeAxControls();

                    ResetBindings();

                    if (IsHandleCreated)
                    {
                        DestroyHandle();
                    }

                    if (_parent != null)
                    {
                        _parent.Controls.Remove(this);
                    }

                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                    if (controlsCollection != null)
                    {

                        // PERFNOTE: This is more efficient than using Foreach.  Foreach
                        // forces the creation of an array subset enum each time we
                        // enumerate
                        for (int i = 0; i < controlsCollection.Count; i++)
                        {
                            Control ctl = controlsCollection[i];
                            ctl._parent = null;
                            ctl.Dispose();
                        }
                        Properties.SetObject(s_controlsCollectionProperty, null);
                    }

                    base.Dispose(disposing);
                }
                finally
                {
                    ResumeLayout(false);
                    SetState(STATE_DISPOSING, false);
                    SetState(STATE_DISPOSED, true);
                }
            }
            else
            {

#if FINALIZATION_WATCH
                if (!GetState(STATE_DISPOSED)) {
                    Debug.Fail("Control of type '" + GetType().FullName +"' is being finalized that wasn't disposed\n" + allocationSite);
                }
#endif
                // This same post is done in NativeWindow's finalize method, so if you change
                // it, change it there too.
                if (_window != null)
                {
                    _window.ForceExitMessageLoop();
                }
                base.Dispose(disposing);
            }
        }

        // Package scope to allow AxHost to override.
        internal virtual void DisposeAxControls()
        {
            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].DisposeAxControls();
                }
            }
        }

        /// <summary>
        ///  Begins a drag operation. The allowedEffects determine which
        ///  drag operations can occur. If the drag operation needs to interop
        ///  with applications in another process, data should either be
        ///  a base managed class (String, Bitmap, or Metafile) or some Object
        ///  that implements System.Runtime.Serialization.ISerializable. data can also be any Object that
        ///  implements System.Windows.Forms.IDataObject.
        /// </summary>
        public DragDropEffects DoDragDrop(object data, DragDropEffects allowedEffects)
        {
            int[] finalEffect = new int[] { (int)DragDropEffects.None };
            UnsafeNativeMethods.IOleDropSource dropSource = new DropSource(this);

            IComDataObject dataObject = null;

            if (data is IComDataObject)
            {
                dataObject = (IComDataObject)data;
            }
            else
            {
                DataObject iwdata = null;
                if (data is IDataObject)
                {
                    iwdata = new DataObject((IDataObject)data);
                }
                else
                {
                    iwdata = new DataObject();
                    iwdata.SetData(data);
                }
                dataObject = (IComDataObject)iwdata;
            }

            try
            {
                SafeNativeMethods.DoDragDrop(dataObject, dropSource, (int)allowedEffects, finalEffect);
            }
            catch (Exception e)
            {
                if (ClientUtils.IsSecurityOrCriticalException(e))
                {
                    throw;
                }
            }

            return (DragDropEffects)finalEffect[0];
        }

        /// <summary>
        //   Trinity are currently calling IViewObject::Draw in order to render controls on Word & Excel
        //   before they are in place active. However this method is private and they need a public way to do this.
        //   This means that we will need to add a public method to control that supports rendering to a Bitmap:
        //   public virtual void DrawToBitmap(Bitmap bmp, RectangleF targetBounds)
        //   where target bounds is the bounds within which the control should render.
        /// </summary>
        public void DrawToBitmap(Bitmap bitmap, Rectangle targetBounds)
        {
            if (bitmap == null)
            {
                throw new ArgumentNullException(nameof(bitmap));
            }

            if (targetBounds.Width <= 0 || targetBounds.Height <= 0
                || targetBounds.X < 0 || targetBounds.Y < 0)
            {
                throw new ArgumentException(nameof(targetBounds));
            }

            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            int width = Math.Min(Width, targetBounds.Width);
            int height = Math.Min(Height, targetBounds.Height);

            using (Bitmap image = new Bitmap(width, height, bitmap.PixelFormat))
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    IntPtr hDc = g.GetHdc();
                    //send the actual wm_print message
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), WindowMessages.WM_PRINT, (IntPtr)hDc,
                        (IntPtr)(NativeMethods.PRF_CHILDREN | NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND | NativeMethods.PRF_NONCLIENT));

                    //now BLT the result to the destination bitmap.
                    using (Graphics destGraphics = Graphics.FromImage(bitmap))
                    {
                        IntPtr desthDC = destGraphics.GetHdc();
                        SafeNativeMethods.BitBlt(new HandleRef(destGraphics, desthDC), targetBounds.X, targetBounds.Y, width, height,
                                                 new HandleRef(g, hDc), 0, 0, 0xcc0020);
                        destGraphics.ReleaseHdcInternal(desthDC);
                    }

                    g.ReleaseHdcInternal(hDc);
                }
            }
        }

        /// <summary>
        ///  Retrieves the return value of the asynchronous operation
        ///  represented by the IAsyncResult interface passed. If the
        ///  async operation has not been completed, this function will
        ///  block until the result is available.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public object EndInvoke(IAsyncResult asyncResult)
        {
            using (new MultithreadSafeCallScope())
            {
                if (asyncResult == null)
                {
                    throw new ArgumentNullException(nameof(asyncResult));
                }

                if (!(asyncResult is ThreadMethodEntry entry))
                {
                    throw new ArgumentException(SR.ControlBadAsyncResult, "asyncResult");
                }
                Debug.Assert(this == entry._caller, "Called BeginInvoke on one control, and the corresponding EndInvoke on a different control");

                if (!asyncResult.IsCompleted)
                {
                    // ignored
                    Control marshaler = FindMarshalingControl();
                    if (SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(marshaler, marshaler.Handle), out int pid) == SafeNativeMethods.GetCurrentThreadId())
                    {
                        marshaler.InvokeMarshaledCallbacks();
                    }
                    else
                    {
                        marshaler = entry._marshaler;
                        marshaler.WaitForWaitHandle(asyncResult.AsyncWaitHandle);
                    }
                }

                Debug.Assert(asyncResult.IsCompleted, "Why isn't this asyncResult done yet?");
                if (entry._exception != null)
                {
                    throw entry._exception;
                }
                return entry._retVal;
            }
        }

        internal bool EndUpdateInternal()
        {
            return EndUpdateInternal(true);
        }

        internal bool EndUpdateInternal(bool invalidate)
        {
            if (_updateCount > 0)
            {
                Debug.Assert(IsHandleCreated, "Handle should be created by now");
                _updateCount--;
                if (_updateCount == 0)
                {
                    SendMessage(WindowMessages.WM_SETREDRAW, -1, 0);
                    if (invalidate)
                    {
                        Invalidate();
                    }

                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Retrieves the form that this control is on. The control's parent may not be
        ///  the same as the form.
        /// </summary>
        public Form FindForm()
        {
            Control cur = this;
            while (cur != null && !(cur is Form))
            {
                cur = cur.ParentInternal;
            }

            return (Form)cur;
        }

        /// <summary>
        ///  Attempts to find a control Object that we can use to marshal
        ///  calls.  We must marshal calls to a control with a window
        ///  handle, so we traverse up the parent chain until we find one.
        ///  Failing that, we just return ouselves.
        /// </summary>
        private Control FindMarshalingControl()
        {
            lock (this)
            {
                Control c = this;

                while (c != null && !c.IsHandleCreated)
                {
                    Control p = c.ParentInternal;
                    c = p;
                }

                if (c == null)
                {
                    // No control with a created handle.  We
                    // just use our own control.  MarshaledInvoke
                    // will throw an exception because there
                    // is no handle.
                    c = this;
                }
                else
                {
                    Debug.Assert(c.IsHandleCreated, "FindMarshalingControl chose a bad control.");
                }

                return c;
            }
        }

        protected bool GetTopLevel()
        {
            return (_state & STATE_TOPLEVEL) != 0;
        }

        /// <summary>
        ///  Used by AxHost to fire the CreateHandle event.
        /// </summary>
        internal void RaiseCreateHandleEvent(EventArgs e)
        {
            ((EventHandler)Events[s_handleCreatedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the event associated with key with the event data of
        ///  e and a sender of this control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseKeyEvent(object key, KeyEventArgs e)
        {
            ((KeyEventHandler)Events[key])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the event associated with key with the event data of
        ///  e and a sender of this control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseMouseEvent(object key, MouseEventArgs e)
        {
            ((MouseEventHandler)Events[key])?.Invoke(this, e);
        }

        /// <summary>
        ///  Attempts to set focus to this control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool Focus()
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Control::Focus - " + Name);

            // Call the internal method (which form overrides)
            return FocusInternal();
        }

        /// <summary>
        ///  Internal method for setting focus to the control.
        ///  Form overrides this method - because MDI child forms
        ///  need to be focused by calling the MDIACTIVATE message.
        /// </summary>
        private protected virtual bool FocusInternal()
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Control::FocusInternal - " + Name);
            if (CanFocus)
            {
                UnsafeNativeMethods.SetFocus(new HandleRef(this, Handle));
            }
            if (Focused && ParentInternal != null)
            {
                IContainerControl c = ParentInternal.GetContainerControl();

                if (c != null)
                {
                    if (c is ContainerControl)
                    {
                        ((ContainerControl)c).SetActiveControl(this);
                    }
                    else
                    {
                        c.ActiveControl = this;
                    }
                }
            }

            return Focused;
        }

        /// <summary>
        ///  Returns the control that is currently associated with handle.
        ///  This method will search up the HWND parent chain until it finds some
        ///  handle that is associated with with a control. This method is more
        ///  robust that fromHandle because it will correctly return controls
        ///  that own more than one handle.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Control FromChildHandle(IntPtr handle)
        {
            while (handle != IntPtr.Zero)
            {
                Control ctl = FromHandle(handle);
                if (ctl != null)
                {
                    return ctl;
                }

                handle = UnsafeNativeMethods.GetAncestor(new HandleRef(null, handle), NativeMethods.GA_PARENT);
            }
            return null;
        }

        /// <summary>
        ///  Returns the control that is currently associated with handle.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public static Control FromHandle(IntPtr handle)
        {
            NativeWindow w = NativeWindow.FromHandle(handle);
            while (w != null && !(w is ControlNativeWindow))
            {
                w = w.PreviousWindow;
            }

            if (w is ControlNativeWindow controlNativeWindow)
            {
                return controlNativeWindow.GetControl();
            }

            return null;
        }

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal Size ApplySizeConstraints(int width, int height)
        {
            return ApplyBoundsConstraints(0, 0, width, height).Size;
        }

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal Size ApplySizeConstraints(Size proposedSize)
        {
            return ApplyBoundsConstraints(0, 0, proposedSize.Width, proposedSize.Height).Size;
        }

        internal virtual Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
        {
            // COMPAT: in Everett we would allow you to set negative values in pre-handle mode
            // in Whidbey, if you've set Min/Max size we will constrain you to 0,0.  Everett apps didnt
            // have min/max size on control, which is why this works.
            if (MaximumSize != Size.Empty || MinimumSize != Size.Empty)
            {
                Size maximumSize = LayoutUtils.ConvertZeroToUnbounded(MaximumSize);
                Rectangle newBounds = new Rectangle(suggestedX, suggestedY, 0, 0)
                {
                    // Clip the size to maximum and inflate it to minimum as necessary.
                    Size = LayoutUtils.IntersectSizes(new Size(proposedWidth, proposedHeight), maximumSize)
                };
                newBounds.Size = LayoutUtils.UnionSizes(newBounds.Size, MinimumSize);

                return newBounds;
            }

            return new Rectangle(suggestedX, suggestedY, proposedWidth, proposedHeight);
        }

        /// <summary>
        ///  Retrieves the child control that is located at the specified client
        ///  coordinates.
        /// </summary>
        public Control GetChildAtPoint(Point pt, GetChildAtPointSkip skipValue)
        {
            int value = (int)skipValue;
            // Since this is a Flags Enumeration... the only way to validate skipValue is by checking if its within the range.
            if (value < 0 || value > 7)
            {
                throw new InvalidEnumArgumentException(nameof(skipValue), value, typeof(GetChildAtPointSkip));
            }

            IntPtr hwnd = UnsafeNativeMethods.ChildWindowFromPointEx(Handle, pt, value);
            Control ctl = FromChildHandle(hwnd);

            return (ctl == this) ? null : ctl;
        }

        /// <summary>
        ///  Retrieves the child control that is located at the specified client
        ///  coordinates.
        /// </summary>
        public Control GetChildAtPoint(Point pt)
        {
            return GetChildAtPoint(pt, GetChildAtPointSkip.None);
        }

        /// <summary>
        ///  Returns the closest ContainerControl in the control's chain of parent controls
        ///  and forms.
        /// </summary>
        public IContainerControl GetContainerControl()
        {
            Control c = this;

            // Refer to IsContainerControl property for more details.
            if (c != null && IsContainerControl)
            {
                c = c.ParentInternal;
            }
            while (c != null && !IsFocusManagingContainerControl(c))
            {
                c = c.ParentInternal;
            }

            return (IContainerControl)c;
        }

        private static bool IsFocusManagingContainerControl(Control ctl)
        {
            return ((ctl._controlStyle & ControlStyles.ContainerControl) == ControlStyles.ContainerControl && ctl is IContainerControl);
        }

        /// <summary>
        ///  This new Internal method checks the updateCount to signify that the control is within the "BeginUpdate" and "EndUpdate" cycle.
        ///  Check out : for usage of this. The Treeview tries to ForceUpdate the scrollbars by calling "WM_SETREDRAW"
        ///  even if the control in "Begin - End" update cycle. Using thie Function we can guard against repetitively redrawing the control.
        /// </summary>
        internal bool IsUpdating()
        {
            return (_updateCount > 0);
        }

        // Essentially an Hfont; see inner class for details.
        private static FontHandleWrapper GetDefaultFontHandleWrapper()
        {
            if (s_defaultFontHandleWrapper == null)
            {
                s_defaultFontHandleWrapper = new FontHandleWrapper(DefaultFont);
            }

            return s_defaultFontHandleWrapper;
        }

        internal IntPtr GetHRgn(Region region)
        {
            Graphics graphics = CreateGraphicsInternal();
            IntPtr handle = region.GetHrgn(graphics);
            graphics.Dispose();
            return handle;
        }

        /// <summary>
        ///  This is a helper method that is called by ScaleControl to retrieve the bounds
        ///  that the control should be scaled by.  You may override this method if you
        ///  wish to reuse ScaleControl's scaling logic but you need to supply your own
        ///  bounds.  The default implementation returns scaled bounds that take into
        ///  account the BoundsSpecified, whether the control is top level, and whether
        ///  the control is fixed width or auto size, and any adornments the control may have.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual Rectangle GetScaledBounds(Rectangle bounds, SizeF factor, BoundsSpecified specified)
        {
            // We should not include the window adornments in our calculation,
            // because windows scales them for us.
            RECT adornments = new RECT(0, 0, 0, 0);
            CreateParams cp = CreateParams;
            AdjustWindowRectEx(ref adornments, cp.Style, HasMenu, cp.ExStyle);

            float dx = factor.Width;
            float dy = factor.Height;

            int sx = bounds.X;
            int sy = bounds.Y;

            // Don't reposition top level controls.  Also, if we're in
            // design mode, don't reposition the root component.
            bool scaleLoc = !GetState(STATE_TOPLEVEL);
            if (scaleLoc)
            {
                ISite site = Site;
                if (site != null && site.DesignMode)
                {
                    if (site.GetService(typeof(IDesignerHost)) is IDesignerHost host && host.RootComponent == this)
                    {
                        scaleLoc = false;
                    }
                }
            }

            if (scaleLoc)
            {
                if ((specified & BoundsSpecified.X) != 0)
                {
                    sx = (int)Math.Round(bounds.X * dx);
                }

                if ((specified & BoundsSpecified.Y) != 0)
                {
                    sy = (int)Math.Round(bounds.Y * dy);
                }
            }

            int sw = bounds.Width;
            int sh = bounds.Height;

            // Do this even for auto sized controls.  They'll "snap back", but it is important to size them in case
            // they are anchored.
            if ((_controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth && (specified & BoundsSpecified.Width) != 0)
            {
                int adornmentWidth = (adornments.right - adornments.left);
                int localWidth = bounds.Width - adornmentWidth;
                sw = (int)Math.Round(localWidth * dx) + adornmentWidth;
            }
            if ((_controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight && (specified & BoundsSpecified.Height) != 0)
            {
                int adornmentHeight = (adornments.bottom - adornments.top);
                int localHeight = bounds.Height - adornmentHeight;
                sh = (int)Math.Round(localHeight * dy) + adornmentHeight;
            }

            return new Rectangle(sx, sy, sw, sh);
        }

        private MouseButtons GetXButton(int wparam)
        {
            switch (wparam)
            {
                case NativeMethods.XBUTTON1:
                    return MouseButtons.XButton1;
                case NativeMethods.XBUTTON2:
                    return MouseButtons.XButton2;
            }
            Debug.Fail("Unknown XButton: " + wparam);
            return MouseButtons.None;
        }

        internal virtual bool GetVisibleCore()
        {
            // We are only visible if our parent is visible
            if (!GetState(STATE_VISIBLE))
            {
                return false;
            }
            else if (ParentInternal == null)
            {
                return true;
            }
            else
            {
                return ParentInternal.GetVisibleCore();
            }
        }

        internal bool GetAnyDisposingInHierarchy()
        {
            Control up = this;
            bool isDisposing = false;
            while (up != null)
            {
                if (up.Disposing)
                {
                    isDisposing = true;
                    break;
                }
                up = up._parent;
            }
            return isDisposing;
        }

        /// <summary>
        ///  - Returns child controls sorted according to their TabIndex property order.
        ///  - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///  - Returns a TabIndex sorted array of ControlTabOrderHolder objects.
        /// </summary>
        private ArrayList GetChildControlsTabOrderList(bool handleCreatedOnly)
        {
            ArrayList holders = new ArrayList();

            foreach (Control c in Controls)
            {
                if (!handleCreatedOnly || c.IsHandleCreated)
                {
                    holders.Add(new ControlTabOrderHolder(holders.Count, c.TabIndex, c));
                }
            }

            holders.Sort(new ControlTabOrderComparer());

            return holders;
        }

        /// <summary>
        ///  - Returns native child windows sorted according to their TabIndex property order.
        ///  - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///  - Child windows with no corresponding Control objects (and therefore no discernable TabIndex)
        ///  are sorted to the front of the list (but remain in relative z-order to one another).
        ///  - This version returns a sorted array of integers, representing the original z-order
        ///  based indexes of the native child windows.
        /// </summary>
        private int[] GetChildWindowsInTabOrder()
        {
            ArrayList holders = GetChildWindowsTabOrderList();

            int[] indexes = new int[holders.Count];

            for (int i = 0; i < holders.Count; i++)
            {
                indexes[i] = ((ControlTabOrderHolder)holders[i])._oldOrder;
            }

            return indexes;
        }

        /// <summary>
        ///  - Returns child controls sorted according to their TabIndex property order.
        ///  - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///  - This version returns a sorted array of control references.
        /// </summary>
        internal Control[] GetChildControlsInTabOrder(bool handleCreatedOnly)
        {
            ArrayList holders = GetChildControlsTabOrderList(handleCreatedOnly);

            Control[] ctls = new Control[holders.Count];

            for (int i = 0; i < holders.Count; i++)
            {
                ctls[i] = ((ControlTabOrderHolder)holders[i])._control;
            }

            return ctls;
        }

        /// <summary>
        ///  Given a native window handle, returns array of handles to window's children (in z-order).
        /// </summary>
        private static ArrayList GetChildWindows(IntPtr hWndParent)
        {
            ArrayList windows = new ArrayList();

            for (IntPtr hWndChild = UnsafeNativeMethods.GetWindow(new HandleRef(null, hWndParent), NativeMethods.GW_CHILD);
                 hWndChild != IntPtr.Zero;
                 hWndChild = UnsafeNativeMethods.GetWindow(new HandleRef(null, hWndChild), NativeMethods.GW_HWNDNEXT))
            {
                windows.Add(hWndChild);
            }

            return windows;
        }

        /// <summary>
        ///  - Returns native child windows sorted according to their TabIndex property order.
        ///  - Controls with the same TabIndex remain in original relative child index order (= z-order).
        ///  - Child windows with no corresponding Control objects (and therefore no discernable TabIndex)
        ///  are sorted to the front of the list (but remain in relative z-order to one another).
        ///  - Returns a TabIndex sorted array of ControlTabOrderHolder objects.
        /// </summary>
        private ArrayList GetChildWindowsTabOrderList()
        {
            ArrayList holders = new ArrayList();
            ArrayList windows = GetChildWindows(Handle);

            foreach (IntPtr hWnd in windows)
            {
                Control ctl = FromHandle(hWnd);
                int tabIndex = (ctl == null) ? -1 : ctl.TabIndex;
                holders.Add(new ControlTabOrderHolder(holders.Count, tabIndex, ctl));
            }

            holders.Sort(new ControlTabOrderComparer());

            return holders;
        }

        internal virtual Control GetFirstChildControlInTabOrder(bool forward)
        {
            ControlCollection ctlControls = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

            Control found = null;
            if (ctlControls != null)
            {
                if (forward)
                {
                    for (int c = 0; c < ctlControls.Count; c++)
                    {
                        if (found == null || found._tabIndex > ctlControls[c]._tabIndex)
                        {
                            found = ctlControls[c];
                        }
                    }
                }
                else
                {
                    // Cycle through the controls in reverse z-order looking for the one with the highest
                    // tab index.
                    for (int c = ctlControls.Count - 1; c >= 0; c--)
                    {
                        if (found == null || found._tabIndex < ctlControls[c]._tabIndex)
                        {
                            found = ctlControls[c];
                        }
                    }
                }
            }

            return found;
        }

        /// <summary>
        ///  Retrieves the next control in the tab order of child controls.
        /// </summary>
        public Control GetNextControl(Control ctl, bool forward)
        {
            if (!Contains(ctl))
            {
                ctl = this;
            }

            if (forward)
            {
                ControlCollection ctlControls = (ControlCollection)ctl.Properties.GetObject(s_controlsCollectionProperty);

                if (ctlControls != null && ctlControls.Count > 0 && (ctl == this || !IsFocusManagingContainerControl(ctl)))
                {
                    Control found = ctl.GetFirstChildControlInTabOrder(/*forward=*/true);
                    if (found != null)
                    {
                        return found;
                    }
                }

                while (ctl != this)
                {
                    int targetIndex = ctl._tabIndex;
                    bool hitCtl = false;
                    Control found = null;
                    Control p = ctl._parent;

                    // Cycle through the controls in z-order looking for the one with the next highest
                    // tab index.  Because there can be dups, we have to start with the existing tab index and
                    // remember to exclude the current control.
                    //
                    int parentControlCount = 0;

                    ControlCollection parentControls = (ControlCollection)p.Properties.GetObject(s_controlsCollectionProperty);

                    if (parentControls != null)
                    {
                        parentControlCount = parentControls.Count;
                    }

                    for (int c = 0; c < parentControlCount; c++)
                    {
                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // clauses:

                        // We are not interested in ourself.
                        if (parentControls[c] != ctl)
                        {
                            // We are interested in controls with >= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            if (parentControls[c]._tabIndex >= targetIndex)
                            {
                                // Check to see if this control replaces the "best match" we've already
                                // found.
                                if (found == null || found._tabIndex > parentControls[c]._tabIndex)
                                {
                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    if (parentControls[c]._tabIndex != targetIndex || hitCtl)
                                    {
                                        found = parentControls[c];
                                    }
                                }
                            }
                        }
                        else
                        {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            hitCtl = true;
                        }
                    }

                    if (found != null)
                    {
                        return found;
                    }

                    ctl = ctl._parent;
                }
            }
            else
            {
                if (ctl != this)
                {
                    int targetIndex = ctl._tabIndex;
                    bool hitCtl = false;
                    Control found = null;
                    Control p = ctl._parent;

                    // Cycle through the controls in reverse z-order looking for the next lowest tab index.  We must
                    // start with the same tab index as ctl, because there can be dups.
                    int parentControlCount = 0;

                    ControlCollection parentControls = (ControlCollection)p.Properties.GetObject(s_controlsCollectionProperty);

                    if (parentControls != null)
                    {
                        parentControlCount = parentControls.Count;
                    }

                    for (int c = parentControlCount - 1; c >= 0; c--)
                    {
                        // The logic for this is a bit lengthy, so I have broken it into separate
                        // clauses:

                        // We are not interested in ourself.
                        if (parentControls[c] != ctl)
                        {
                            // We are interested in controls with <= tab indexes to ctl.  We must include those
                            // controls with equal indexes to account for duplicate indexes.
                            if (parentControls[c]._tabIndex <= targetIndex)
                            {
                                // Check to see if this control replaces the "best match" we've already
                                // found.
                                if (found == null || found._tabIndex < parentControls[c]._tabIndex)
                                {
                                    // Finally, check to make sure that if this tab index is the same as ctl,
                                    // that we've already encountered ctl in the z-order.  If it isn't the same,
                                    // than we're more than happy with it.
                                    if (parentControls[c]._tabIndex != targetIndex || hitCtl)
                                    {
                                        found = parentControls[c];
                                    }
                                }
                            }
                        }
                        else
                        {
                            // We track when we have encountered "ctl".  We never want to select ctl again, but
                            // we want to know when we've seen it in case we find another control with the same tab index.
                            hitCtl = true;
                        }
                    }

                    // If we were unable to find a control we should return the control's parent.  However, if that parent is us, return
                    // NULL.
                    if (found != null)
                    {
                        ctl = found;
                    }
                    else
                    {
                        if (p == this)
                        {
                            return null;
                        }
                        else
                        {
                            return p;
                        }
                    }
                }

                // We found a control.  Walk into this control to find the proper sub control within it to select.
                ControlCollection ctlControls = (ControlCollection)ctl.Properties.GetObject(s_controlsCollectionProperty);

                while (ctlControls != null && ctlControls.Count > 0 && (ctl == this || !IsFocusManagingContainerControl(ctl)))
                {
                    Control found = ctl.GetFirstChildControlInTabOrder(/*forward=*/false);
                    if (found != null)
                    {
                        ctl = found;
                        ctlControls = (ControlCollection)ctl.Properties.GetObject(s_controlsCollectionProperty);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return ctl == this ? null : ctl;
        }

        /// <summary>
        ///  Return ((Control) window).Handle if window is a Control.
        ///  Otherwise, returns window.Handle
        /// </summary>
        internal static IntPtr GetSafeHandle(IWin32Window window)
        {
            Debug.Assert(window != null, "window is null in Control.GetSafeHandle");
            if (window is Control control)
            {
                return control.Handle;
            }
            else
            {
                IntPtr hWnd = window.Handle;
                if (hWnd == IntPtr.Zero || UnsafeNativeMethods.IsWindow(new HandleRef(null, hWnd)))
                {
                    return hWnd;
                }
                else
                {
                    throw new Win32Exception(NativeMethods.ERROR_INVALID_HANDLE);
                }
            }
        }

        /// <summary>
        ///  Retrieves the current value of the specified bit in the control's state.
        /// </summary>
        internal bool GetState(int flag)
        {
            return (_state & flag) != 0;
        }

        /// <summary>
        ///  Retrieves the current value of the specified bit in the control's state2.
        /// </summary>
        private bool GetState2(int flag)
        {
            return (_state2 & flag) != 0;
        }

        /// <summary>
        ///  Retrieves the current value of the specified bit in the control's style.
        ///  NOTE: This is control style, not the Win32 style of the hWnd.
        /// </summary>
        protected bool GetStyle(ControlStyles flag)
        {
            return (_controlStyle & flag) == flag;
        }

        /// <summary>
        ///  Hides the control by setting the visible property to false;
        /// </summary>
        public void Hide()
        {
            Visible = false;
        }

        /// <summary>
        ///  Sets up the TrackMouseEvent for listening for the
        ///  mouse leave event.
        /// </summary>
        private void HookMouseEvent()
        {
            if (!GetState(STATE_TRACKINGMOUSEEVENT))
            {
                SetState(STATE_TRACKINGMOUSEEVENT, true);

                if (_trackMouseEvent == null)
                {
                    _trackMouseEvent = new NativeMethods.TRACKMOUSEEVENT
                    {
                        dwFlags = NativeMethods.TME_LEAVE | NativeMethods.TME_HOVER,
                        hwndTrack = Handle
                    };
                }

                SafeNativeMethods.TrackMouseEvent(_trackMouseEvent);
            }
        }

        /// <summary>
        ///  Called after the control has been added to another container.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void InitLayout()
        {
            LayoutEngine.InitLayout(this, BoundsSpecified.All);
        }

        /// <summary>
        ///  This method initializes the scaling bits for this control based on
        ///  the bounds.
        /// </summary>
        private void InitScaling(BoundsSpecified specified)
        {
            _requiredScaling |= (byte)((int)specified & RequiredScalingMask);
        }

        // Sets the text and background colors of the DC, and returns the background HBRUSH.
        // This gets called from some variation on WM_CTLCOLOR...
        // Virtual because Scrollbar insists on being different.
        //
        // NOTE: this message may not have originally been sent to this HWND.
        //
        internal virtual IntPtr InitializeDCForWmCtlColor(IntPtr dc, int msg)
        {
            if (!GetStyle(ControlStyles.UserPaint))
            {
                SafeNativeMethods.SetTextColor(new HandleRef(null, dc), ColorTranslator.ToWin32(ForeColor));
                SafeNativeMethods.SetBkColor(new HandleRef(null, dc), ColorTranslator.ToWin32(BackColor));
                return BackColorBrush;
            }

            return Gdi32.GetStockObject(Gdi32.StockObject.HOLLOW_BRUSH);
        }

        /// <summary>
        ///  Invalidates a region of the control and causes a paint message
        ///  to be sent to the control. This will not force a synchronous paint to
        ///  occur, calling update after invalidate will force a
        ///  synchronous paint.
        /// </summary>
        public void Invalidate(Region region)
        {
            Invalidate(region, false);
        }

        /// <summary>
        ///  Invalidates a region of the control and causes a paint message
        ///  to be sent to the control. This will not force a synchronous paint to
        ///  occur, calling update after invalidate will force a
        ///  synchronous paint.
        /// </summary>
        public void Invalidate(Region region, bool invalidateChildren)
        {
            if (region == null)
            {
                Invalidate(invalidateChildren);
            }
            else if (IsHandleCreated)
            {
                IntPtr regionHandle = GetHRgn(region);

                try
                {
                    Debug.Assert(regionHandle != IntPtr.Zero, "Region wasn't null but HRGN is?");
                    if (invalidateChildren)
                    {
                        SafeNativeMethods.RedrawWindow(new HandleRef(this, Handle),
                                                       null, new HandleRef(region, regionHandle),
                                                       NativeMethods.RDW_INVALIDATE |
                                                       NativeMethods.RDW_ERASE |
                                                       NativeMethods.RDW_ALLCHILDREN);
                    }
                    else
                    {
                        // It's safe to invoke InvalidateRgn from a separate thread.
                        using (new MultithreadSafeCallScope())
                        {
                            SafeNativeMethods.InvalidateRgn(new HandleRef(this, Handle),
                                                            new HandleRef(region, regionHandle),
                                                            !GetStyle(ControlStyles.Opaque));
                        }
                    }
                }
                finally
                {
                    Gdi32.DeleteObject(regionHandle);
                }

                Rectangle bounds = Rectangle.Empty;

                // gpr: We shouldn't have to create a Graphics for this...
                using (Graphics graphics = CreateGraphicsInternal())
                {
                    bounds = Rectangle.Ceiling(region.GetBounds(graphics));
                }

                OnInvalidated(new InvalidateEventArgs(bounds));
            }
        }

        /// <summary>
        ///  Invalidates the control and causes a paint message to be sent to the control.
        ///  This will not force a synchronous paint to occur, calling update after
        ///  invalidate will force a synchronous paint.
        /// </summary>
        public void Invalidate()
        {
            Invalidate(false);
        }

        /// <summary>
        ///  Invalidates the control and causes a paint message to be sent to the control.
        ///  This will not force a synchronous paint to occur, calling update after
        ///  invalidate will force a synchronous paint.
        /// </summary>
        public void Invalidate(bool invalidateChildren)
        {
            if (IsHandleCreated)
            {
                if (invalidateChildren)
                {
                    SafeNativeMethods.RedrawWindow(new HandleRef(_window, Handle),
                                                   null, NativeMethods.NullHandleRef,
                                                   NativeMethods.RDW_INVALIDATE |
                                                   NativeMethods.RDW_ERASE |
                                                   NativeMethods.RDW_ALLCHILDREN);
                }
                else
                {
                    // It's safe to invoke InvalidateRect from a separate thread.
                    using (new MultithreadSafeCallScope())
                    {
                        SafeNativeMethods.InvalidateRect(new HandleRef(_window, Handle),
                                                         null,
                                                         (_controlStyle & ControlStyles.Opaque) != ControlStyles.Opaque);
                    }
                }

                NotifyInvalidate(ClientRectangle);
            }
        }

        /// <summary>
        ///  Invalidates a rectangular region of the control and causes a paint message
        ///  to be sent to the control. This will not force a synchronous paint to
        ///  occur, calling update after invalidate will force a
        ///  synchronous paint.
        /// </summary>
        public void Invalidate(Rectangle rc)
        {
            Invalidate(rc, false);
        }

        /// <summary>
        ///  Invalidates a rectangular region of the control and causes a paint message
        ///  to be sent to the control. This will not force a synchronous paint to
        ///  occur, calling update after invalidate will force a
        ///  synchronous paint.
        /// </summary>
        public void Invalidate(Rectangle rc, bool invalidateChildren)
        {
            if (rc.IsEmpty)
            {
                Invalidate(invalidateChildren);
            }
            else if (IsHandleCreated)
            {
                if (invalidateChildren)
                {
                    RECT rcArea = rc;
                    SafeNativeMethods.RedrawWindow(
                        new HandleRef(_window, Handle),
                        ref rcArea,
                        IntPtr.Zero,
                        NativeMethods.RDW_INVALIDATE | NativeMethods.RDW_ERASE | NativeMethods.RDW_ALLCHILDREN);
                }
                else
                {
                    RECT rcArea = new RECT(rc);

                    // It's safe to invoke InvalidateRect from a separate thread.
                    using (new MultithreadSafeCallScope())
                    {
                        SafeNativeMethods.InvalidateRect(
                            new HandleRef(_window, Handle),
                            ref rcArea,
                            (_controlStyle & ControlStyles.Opaque) != ControlStyles.Opaque);
                    }
                }
                NotifyInvalidate(rc);
            }
        }
        /// <summary>
        ///  Executes the given delegate on the thread that owns this Control's
        ///  underlying window handle.  It is an error to call this on the same thread that
        ///  the control belongs to.  If the control's handle doesn't exist yet, this will
        ///  follow up the control's parent chain until it finds a control or form that does
        ///  have a window handle.  If no appropriate handle can be found, invoke will throw
        ///  an exception.  Exceptions that are raised during the call will be
        ///  propapgated back to the caller.
        ///
        ///  There are five functions on a control that are safe to call from any
        ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///  For all other method calls, you should use one of the invoke methods to marshal
        ///  the call to the control's thread.
        /// </summary>
        public object Invoke(Delegate method)
        {
            return Invoke(method, null);
        }

        /// <summary>
        ///  Executes the given delegate on the thread that owns this Control's
        ///  underlying window handle.  It is an error to call this on the same thread that
        ///  the control belongs to.  If the control's handle doesn't exist yet, this will
        ///  follow up the control's parent chain until it finds a control or form that does
        ///  have a window handle.  If no appropriate handle can be found, invoke will throw
        ///  an exception.  Exceptions that are raised during the call will be
        ///  propapgated back to the caller.
        ///
        ///  There are five functions on a control that are safe to call from any
        ///  thread:  GetInvokeRequired, Invoke, BeginInvoke, EndInvoke and CreateGraphics.
        ///  For all other method calls, you should use one of the invoke methods to marshal
        ///  the call to the control's thread.
        /// </summary>
        public object Invoke(Delegate method, params object[] args)
        {
            using (new MultithreadSafeCallScope())
            {
                Control marshaler = FindMarshalingControl();
                return marshaler.MarshaledInvoke(this, method, args, true);
            }
        }

        /// <summary>
        ///  Perform the callback of a particular ThreadMethodEntry - called by InvokeMarshaledCallbacks below.
        ///
        ///  If the invoke request originated from another thread, we should have already captured the ExecutionContext
        ///  of that thread. The callback is then invoked using that ExecutionContext (which includes info like the
        ///  compressed security stack).
        ///
        ///  NOTE: The one part of the ExecutionContext that we DONT want applied to the callback is its SyncContext,
        ///  since this is the SyncContext of the other thread. So we grab the SyncContext of OUR thread, and pass
        ///  this through to the callback to use instead.
        ///
        ///  When the invoke request comes from this thread, there won't be an ExecutionContext so we just invoke
        ///  the callback as is.
        /// </summary>
        private void InvokeMarshaledCallback(ThreadMethodEntry tme)
        {
            if (tme._executionContext != null)
            {
                if (s_invokeMarshaledCallbackHelperDelegate == null)
                {
                    s_invokeMarshaledCallbackHelperDelegate = new ContextCallback(InvokeMarshaledCallbackHelper);
                }
                // If there's no ExecutionContext, make sure we have a SynchronizationContext.  There's no
                // direct check for ExecutionContext: this is as close as we can get.
                if (SynchronizationContext.Current == null)
                {
                    WindowsFormsSynchronizationContext.InstallIfNeeded();
                }

                tme._syncContext = SynchronizationContext.Current;
                ExecutionContext.Run(tme._executionContext, s_invokeMarshaledCallbackHelperDelegate, tme);
            }
            else
            {
                InvokeMarshaledCallbackHelper(tme);
            }
        }

        /// <summary>
        ///  Worker for invoking marshaled callbacks.
        /// </summary>
        private static void InvokeMarshaledCallbackHelper(object obj)
        {
            ThreadMethodEntry tme = (ThreadMethodEntry)obj;

            if (tme._syncContext != null)
            {
                SynchronizationContext oldContext = SynchronizationContext.Current;

                try
                {
                    SynchronizationContext.SetSynchronizationContext(tme._syncContext);

                    InvokeMarshaledCallbackDo(tme);
                }
                finally
                {
                    SynchronizationContext.SetSynchronizationContext(oldContext);
                }
            }
            else
            {
                InvokeMarshaledCallbackDo(tme);
            }
        }

        private static void InvokeMarshaledCallbackDo(ThreadMethodEntry tme)
        {
            // We short-circuit a couple of common cases for speed.
            if (tme._method is EventHandler)
            {
                if (tme._args == null || tme._args.Length < 1)
                {
                    ((EventHandler)tme._method)(tme._caller, EventArgs.Empty);
                }
                else if (tme._args.Length < 2)
                {
                    ((EventHandler)tme._method)(tme._args[0], EventArgs.Empty);
                }
                else
                {
                    ((EventHandler)tme._method)(tme._args[0], (EventArgs)tme._args[1]);
                }
            }
            else if (tme._method is MethodInvoker)
            {
                ((MethodInvoker)tme._method)();
            }
            else if (tme._method is WaitCallback)
            {
                Debug.Assert(tme._args.Length == 1,
                             "Arguments are wrong for WaitCallback");
                ((WaitCallback)tme._method)(tme._args[0]);
            }
            else
            {
                tme._retVal = tme._method.DynamicInvoke(tme._args);
            }
        }

        /// <summary>
        ///  Called on the control's owning thread to perform the actual callback.
        ///  This empties this control's callback queue, propagating any exceptions
        ///  back as needed.
        /// </summary>
        private void InvokeMarshaledCallbacks()
        {
            ThreadMethodEntry current = null;
            lock (_threadCallbackList)
            {
                if (_threadCallbackList.Count > 0)
                {
                    current = (ThreadMethodEntry)_threadCallbackList.Dequeue();
                }
            }

            // Now invoke on all the queued items.
            while (current != null)
            {
                if (current._method != null)
                {
                    try
                    {
                        // If we are running under the debugger, don't wrap asynchronous
                        // calls in a try catch.  It is much better to throw here than pop up
                        // a thread exception dialog below.
                        if (NativeWindow.WndProcShouldBeDebuggable && !current._synchronous)
                        {
                            InvokeMarshaledCallback(current);
                        }
                        else
                        {
                            try
                            {
                                InvokeMarshaledCallback(current);
                            }
                            catch (Exception t)
                            {
                                current._exception = t.GetBaseException();
                            }
                        }
                    }
                    finally
                    {
                        current.Complete();

                        // This code matches the behavior above.  Basically, if we're debugging, don't
                        // do this because the exception would have been handled above.  If we're
                        // not debugging, raise the exception here.
                        if (!NativeWindow.WndProcShouldBeDebuggable &&
                            current._exception != null && !current._synchronous)
                        {
                            Application.OnThreadException(current._exception);
                        }
                    }
                }

                lock (_threadCallbackList)
                {
                    if (_threadCallbackList.Count > 0)
                    {
                        current = (ThreadMethodEntry)_threadCallbackList.Dequeue();
                    }
                    else
                    {
                        current = null;
                    }
                }
            }
        }

        protected void InvokePaint(Control c, PaintEventArgs e)
        {
            c.OnPaint(e);
        }

        protected void InvokePaintBackground(Control c, PaintEventArgs e)
        {
            c.OnPaintBackground(e);
        }

        /// <summary>
        ///  Determines whether the font is set.
        /// </summary>
        internal bool IsFontSet()
        {
            Font font = (Font)Properties.GetObject(s_fontProperty);
            if (font != null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///  WARNING! The meaning of this method is not what it appears.
        ///  The method returns true if "descendant" (the argument) is a descendant
        ///  of "this". I'd expect it to be the other way around, but oh well too late.
        /// </summary>
        internal bool IsDescendant(Control descendant)
        {
            Control control = descendant;
            while (control != null)
            {
                if (control == this)
                {
                    return true;
                }

                control = control.ParentInternal;
            }
            return false;
        }

        /// <summary>
        ///  This Function will return a Boolean as to whether the Key value passed in is Locked...
        /// </summary>
        public static bool IsKeyLocked(Keys keyVal)
        {
            if (keyVal == Keys.Insert || keyVal == Keys.NumLock || keyVal == Keys.CapsLock || keyVal == Keys.Scroll)
            {
                int result = UnsafeNativeMethods.GetKeyState((int)keyVal);

                // If the high-order bit is 1, the key is down; otherwise, it is up.
                // If the low-order bit is 1, the key is toggled. A key, such as the CAPS LOCK key,
                // is toggled if it is turned on. The key is off and untoggled if the low-order bit is 0.
                // A toggle key's indicator light (if any) on the keyboard will be on when the key is toggled,
                // and off when the key is untoggled.

                // Toggle keys (only low bit is of interest).
                if (keyVal == Keys.Insert || keyVal == Keys.CapsLock)
                {
                    return (result & 0x1) != 0x0;
                }

                return (result & 0x8001) != 0x0;
            }

            // else - it's an un-lockable key.
            // Actually get the exception string from the system resource.
            throw new NotSupportedException(SR.ControlIsKeyLockedNumCapsScrollLockKeysSupportedOnly);
        }

        /// <summary>
        ///  Determines if charCode is an input character that the control
        ///  wants. This method is called during window message pre-processing to
        ///  determine whether the given input character should be pre-processed or
        ///  sent directly to the control. If isInputChar returns true, the
        ///  given character is sent directly to the control. If isInputChar
        ///  returns false, the character is pre-processed and only sent to the
        ///  control if it is not consumed by the pre-processing phase. The
        ///  pre-processing of a character includes checking whether the character
        ///  is a mnemonic of another control.
        /// </summary>
        protected virtual bool IsInputChar(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.IsInputChar 0x" + ((int)charCode).ToString("X", CultureInfo.InvariantCulture));

            int mask = 0;
            if (charCode == (char)(int)Keys.Tab)
            {
                mask = NativeMethods.DLGC_WANTCHARS | NativeMethods.DLGC_WANTALLKEYS | NativeMethods.DLGC_WANTTAB;
            }
            else
            {
                mask = NativeMethods.DLGC_WANTCHARS | NativeMethods.DLGC_WANTALLKEYS;
            }
            return (unchecked((int)(long)SendMessage(WindowMessages.WM_GETDLGCODE, 0, 0)) & mask) != 0;
        }

        /// <summary>
        ///  Determines if keyData is an input key that the control wants.
        ///  This method is called during window message pre-processing to determine
        ///  whether the given input key should be pre-processed or sent directly to
        ///  the control. If isInputKey returns true, the given key is sent
        ///  directly to the control. If isInputKey returns false, the key is
        ///  pre-processed and only sent to the control if it is not consumed by the
        ///  pre-processing phase. Keys that are pre-processed include TAB, RETURN,
        ///  ESCAPE, and arrow keys.
        /// </summary>
        protected virtual bool IsInputKey(Keys keyData)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.IsInputKey " + keyData.ToString());

            if ((keyData & Keys.Alt) == Keys.Alt)
            {
                return false;
            }

            int mask = NativeMethods.DLGC_WANTALLKEYS;
            switch (keyData & Keys.KeyCode)
            {
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

            if (IsHandleCreated)
            {
                return (unchecked((int)(long)SendMessage(WindowMessages.WM_GETDLGCODE, 0, 0)) & mask) != 0;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        ///  Determines if charCode is the mnemonic character in text.
        ///  The mnemonic character is the character imediately following the first
        ///  instance of "&amp;" in text
        /// </summary>
        public static bool IsMnemonic(char charCode, string text)
        {
#if DEBUG
            if (s_controlKeyboardRouting.TraceVerbose)
            {
                Debug.Write("Control.IsMnemonic(" + charCode.ToString() + ", ");

                if (text != null)
                {
                    Debug.Write(text);
                }
                else
                {
                    Debug.Write("null");
                }
                Debug.WriteLine(")");
            }
#endif

            //Special case handling:
            if (charCode == '&')
            {
                Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "   ...returning false");
                return false;
            }

            if (text != null)
            {
                int pos = -1; // start with -1 to handle double &'s
                char c2 = char.ToUpper(charCode, CultureInfo.CurrentCulture);
                for (; ; )
                {
                    if (pos + 1 >= text.Length)
                    {
                        break;
                    }

                    pos = text.IndexOf('&', pos + 1) + 1;
                    if (pos <= 0 || pos >= text.Length)
                    {
                        break;
                    }

                    char c1 = char.ToUpper(text[pos], CultureInfo.CurrentCulture);
                    Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "   ...& found... char=" + c1.ToString());
                    if (c1 == c2 || char.ToLower(c1, CultureInfo.CurrentCulture) == char.ToLower(c2, CultureInfo.CurrentCulture))
                    {
                        Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "   ...returning true");
                        return true;
                    }
                }
                Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose && pos == 0, "   ...no & found");
            }
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "   ...returning false");
            return false;
        }

        private void ListenToUserPreferenceChanged(bool listen)
        {
            if (GetState2(STATE2_LISTENINGTOUSERPREFERENCECHANGED))
            {
                if (!listen)
                {
                    SetState2(STATE2_LISTENINGTOUSERPREFERENCECHANGED, false);
                    SystemEvents.UserPreferenceChanged -= new UserPreferenceChangedEventHandler(UserPreferenceChanged);
                }
            }
            else if (listen)
            {
                SetState2(STATE2_LISTENINGTOUSERPREFERENCECHANGED, true);
                SystemEvents.UserPreferenceChanged += new UserPreferenceChangedEventHandler(UserPreferenceChanged);
            }
        }

        /// <summary>
        ///  Transforms an integer coordinate from logical to device units
        ///  by scaling it for the current DPI and rounding down to the nearest integer value.
        /// </summary>
        public int LogicalToDeviceUnits(int value)
        {
            return DpiHelper.LogicalToDeviceUnits(value, DeviceDpi);
        }

        /// <summary>
        ///  Transforms size from logical to device units by scaling it for the current
        ///  DPI and rounding down to the nearest integer value for width and height.
        /// </summary>
        /// <param name="value"> size to be scaled</param>
        /// <returns> scaled size</returns>
        public Size LogicalToDeviceUnits(Size value)
        {
            return DpiHelper.LogicalToDeviceUnits(value, DeviceDpi);
        }

        /// <summary>
        ///  Create a new bitmap scaled for the device units. When displayed on the device,
        ///  the scaled image will have same size as the original image would have when
        ///  displayed at 96dpi.
        /// </summary>
        /// <param name="logicalBitmap">The image to scale from logical units to device units</param>
        public void ScaleBitmapLogicalToDevice(ref Bitmap logicalBitmap)
        {
            DpiHelper.ScaleBitmapLogicalToDevice(ref logicalBitmap, DeviceDpi);
        }

        internal void AdjustWindowRectEx(ref RECT rect, int style, bool bMenu, int exStyle)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                SafeNativeMethods.AdjustWindowRectExForDpi(ref rect, style, bMenu, exStyle, (uint)_deviceDpi);
            }
            else
            {
                SafeNativeMethods.AdjustWindowRectEx(ref rect, style, bMenu, exStyle);
            }
        }

        private object MarshaledInvoke(Control caller, Delegate method, object[] args, bool synchronous)
        {
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

            if (!IsHandleCreated)
            {
                throw new InvalidOperationException(SR.ErrorNoMarshalingThread);
            }

            ActiveXImpl activeXImpl = (ActiveXImpl)Properties.GetObject(s_activeXImplProperty);

            // We don't want to wait if we're on the same thread, or else we'll deadlock.
            // It is important that syncSameThread always be false for asynchronous calls.
            bool syncSameThread = false;

            // ignored
            if (SafeNativeMethods.GetWindowThreadProcessId(new HandleRef(this, Handle), out int pid) == SafeNativeMethods.GetCurrentThreadId())
            {
                if (synchronous)
                {
                    syncSameThread = true;
                }
            }

            // Store the compressed stack information from the thread that is calling the Invoke()
            // so we can assign the same security context to the thread that will actually execute
            // the delegate being passed.
            ExecutionContext executionContext = null;
            if (!syncSameThread)
            {
                executionContext = ExecutionContext.Capture();
            }
            ThreadMethodEntry tme = new ThreadMethodEntry(caller, this, method, args, synchronous, executionContext);

            lock (this)
            {
                if (_threadCallbackList == null)
                {
                    _threadCallbackList = new Queue();
                }
            }

            lock (_threadCallbackList)
            {
                if (s_threadCallbackMessage == 0)
                {
                    s_threadCallbackMessage = SafeNativeMethods.RegisterWindowMessage(Application.WindowMessagesVersion + "_ThreadCallbackMessage");
                }
                _threadCallbackList.Enqueue(tme);
            }

            if (syncSameThread)
            {
                InvokeMarshaledCallbacks();
            }
            else
            {
                UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), s_threadCallbackMessage, IntPtr.Zero, IntPtr.Zero);
            }

            if (synchronous)
            {
                if (!tme.IsCompleted)
                {
                    WaitForWaitHandle(tme.AsyncWaitHandle);
                }
                if (tme._exception != null)
                {
                    throw tme._exception;
                }
                return tme._retVal;
            }
            else
            {
                return tme;
            }
        }

        /// <summary>
        ///  This method is used by WM_GETCONTROLNAME and WM_GETCONTROLTYPE
        ///  to marshal a string to a message structure.  It handles
        ///  two cases:  if no buffer was passed it returns the size of
        ///  buffer needed.  If a buffer was passed, it fills the buffer.
        ///  If the passed buffer is not long enough it will return -1.
        /// </summary>
        private void MarshalStringToMessage(string value, ref Message m)
        {
            if (m.LParam == IntPtr.Zero)
            {
                m.Result = (IntPtr)((value.Length + 1) * sizeof(char));
                return;
            }

            if (unchecked((int)(long)m.WParam) < value.Length + 1)
            {
                m.Result = (IntPtr)(-1);
                return;
            }

            // Copy the name into the given IntPtr
            char[] nullChar = new char[] { (char)0 };
            byte[] nullBytes;
            byte[] bytes;

            bytes = Encoding.Unicode.GetBytes(value);
            nullBytes = Encoding.Unicode.GetBytes(nullChar);

            Marshal.Copy(bytes, 0, m.LParam, bytes.Length);
            Marshal.Copy(nullBytes, 0, unchecked((IntPtr)((long)m.LParam + (long)bytes.Length)), nullBytes.Length);

            m.Result = (IntPtr)((bytes.Length + nullBytes.Length) / sizeof(char));
        }

        // Used by form to notify the control that it has been "entered"
        internal void NotifyEnter()
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Control::NotifyEnter() - " + Name);
            OnEnter(EventArgs.Empty);
        }

        // Used by form to notify the control that it has been "left"
        internal void NotifyLeave()
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Control::NotifyLeave() - " + Name);
            OnLeave(EventArgs.Empty);
        }

        /// <summary>
        ///  Propagates the invalidation event, notifying the control that
        ///  some part of it is being invalidated and will subsequently need
        ///  to repaint.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void NotifyInvalidate(Rectangle invalidatedArea)
        {
            OnInvalidated(new InvalidateEventArgs(invalidatedArea));
        }

        // Used by form to notify the control that it is validating.
        private bool NotifyValidating()
        {
            CancelEventArgs ev = new CancelEventArgs();
            OnValidating(ev);
            return ev.Cancel;
        }

        // Used by form to notify the control that it has been validated.
        private void NotifyValidated()
        {
            OnValidated(EventArgs.Empty);
        }

        /// <summary>
        ///  Raises the <see cref='Click'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void InvokeOnClick(Control toInvoke, EventArgs e)
        {
            if (toInvoke != null)
            {
                toInvoke.OnClick(e);
            }
        }

        protected virtual void OnAutoSizeChanged(EventArgs e)
        {
            if (Events[s_autoSizeChangedEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackColorChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            object backBrush = Properties.GetObject(s_backBrushProperty);
            if (backBrush != null)
            {
                if (GetState(STATE_OWNCTLBRUSH))
                {
                    IntPtr p = (IntPtr)backBrush;
                    if (p != IntPtr.Zero)
                    {
                        Gdi32.DeleteObject(p);
                    }
                }
                Properties.SetObject(s_backBrushProperty, null);
            }

            Invalidate();

            if (Events[s_backColorEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentBackColorChanged(e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackgroundImageChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            Invalidate();

            if (Events[s_backgroundImageEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentBackgroundImageChanged(e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBackgroundImageLayoutChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            Invalidate();

            if (Events[s_backgroundImageLayoutEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnBindingContextChanged(EventArgs e)
        {
            if (Properties.GetObject(s_bindingsProperty) != null)
            {
                UpdateBindings();
            }

            if (Events[s_bindingContextEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentBindingContextChanged(e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCausesValidationChanged(EventArgs e)
        {
            if (Events[s_causesValidationEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Called when a child is about to resume its layout.  The default implementation
        ///  calls OnChildLayoutResuming on the parent.
        /// </summary>
        internal virtual void OnChildLayoutResuming(Control child, bool performLayout)
        {
            if (ParentInternal != null)
            {
                ParentInternal.OnChildLayoutResuming(child, performLayout);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnContextMenuStripChanged(EventArgs e)
        {
            if (Events[s_contextMenuStripEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCursorChanged(EventArgs e)
        {
            if (Events[s_cursorEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentCursorChanged(e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDockChanged(EventArgs e)
        {
            if (Events[s_dockEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Enabled'/> event.
        ///
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnEnabled to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnEnabledChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            if (IsHandleCreated)
            {
                SafeNativeMethods.EnableWindow(new HandleRef(this, Handle), Enabled);

                // User-paint controls should repaint when their enabled state changes
                if (GetStyle(ControlStyles.UserPaint))
                {
                    Invalidate();
                    Update();
                }
            }

            if (Events[s_enabledEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentEnabledChanged(e);
                }
            }
        }

        internal virtual void OnFrameWindowActivate(bool fActivate)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnFontChanged(EventArgs e)
        {
            // bail if disposing
            //
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            Invalidate();

            if (Properties.ContainsInteger(s_fontHeightProperty))
            {
                Properties.SetInteger(s_fontHeightProperty, -1);
            }

            // Cleanup any font handle wrapper...
            DisposeFontHandle();

            if (IsHandleCreated && !GetStyle(ControlStyles.UserPaint))
            {
                SetWindowFont();
            }

            if (Events[s_fontEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            using (new LayoutTransaction(this, this, PropertyNames.Font, false))
            {
                if (controlsCollection != null)
                {
                    // This may have changed the sizes of our children.
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        controlsCollection[i].OnParentFontChanged(e);
                    }
                }
            }

            LayoutTransaction.DoLayout(this, this, PropertyNames.Font);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnForeColorChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            Invalidate();

            if (Events[s_foreColorEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentForeColorChanged(e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftChanged(EventArgs e)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            // update the scroll position when the handle has been created
            // MUST SET THIS BEFORE CALLING RecreateHandle!!!
            SetState2(STATE2_SETSCROLLPOS, true);

            RecreateHandle();

            if (Events[s_rightToLeftEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnParentRightToLeftChanged(e);
                }
            }
        }

        /// <summary>
        ///  OnNotifyMessage is called if the ControlStyles.EnableNotifyMessage bit is set.
        ///  This allows for controls to listen to window messages, without allowing them to
        ///  actually modify the message.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnNotifyMessage(Message m)
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBackColorChanged(EventArgs e)
        {
            Color backColor = Properties.GetColor(s_backColorProperty);
            if (backColor.IsEmpty)
            {
                OnBackColorChanged(e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBackgroundImageChanged(EventArgs e)
        {
            OnBackgroundImageChanged(e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentBindingContextChanged(EventArgs e)
        {
            if (Properties.GetObject(s_bindingManagerProperty) == null)
            {
                OnBindingContextChanged(e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentCursorChanged(EventArgs e)
        {
            if (Properties.GetObject(s_cursorProperty) == null)
            {
                OnCursorChanged(e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentEnabledChanged(EventArgs e)
        {
            if (GetState(STATE_ENABLED))
            {
                OnEnabledChanged(e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentFontChanged(EventArgs e)
        {
            if (Properties.GetObject(s_fontProperty) == null)
            {
                OnFontChanged(e);
            }
        }

        /// <summary>
        ///  Occurs when the parent of this control has recreated
        ///  its handle.
        /// </summary>
        internal virtual void OnParentHandleRecreated()
        {
            // restore ourselves over to the original control.
            // use SetParent directly so as to not raise ParentChanged events
            Control parent = ParentInternal;
            if (parent != null)
            {
                if (IsHandleCreated)
                {
                    UnsafeNativeMethods.SetParent(new HandleRef(this, Handle), new HandleRef(parent, parent.Handle));
                    UpdateZOrder();
                }
            }
            SetState(STATE_PARENTRECREATING, false);

            // if our parent was initially the the parent who's handle just got recreated, we need
            // to recreate ourselves so that we get notification.  See UpdateReflectParent for more details.
            if (ReflectParent == ParentInternal)
            {
                RecreateHandle();
            }
        }

        /// <summary>
        ///   Occurs when the parent of this control is recreating
        ///   its handle.
        /// </summary>
        internal virtual void OnParentHandleRecreating()
        {
            SetState(STATE_PARENTRECREATING, true);

            // swoop this control over to the parking window.

            // if we left it parented to the parent control, the DestroyWindow
            // would force us to destroy our handle as well... hopefully
            // parenting us over to the parking window and restoring ourselves
            // should help improve recreate perf.

            // use SetParent directly so as to not raise ParentChanged events
            if (IsHandleCreated)
            {
                Application.ParkHandle(new HandleRef(this, Handle));
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentForeColorChanged(EventArgs e)
        {
            Color foreColor = Properties.GetColor(s_foreColorProperty);
            if (foreColor.IsEmpty)
            {
                OnForeColorChanged(e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentRightToLeftChanged(EventArgs e)
        {
            if (!Properties.ContainsInteger(s_rightToLeftProperty) || ((RightToLeft)Properties.GetInteger(s_rightToLeftProperty)) == RightToLeft.Inherit)
            {
                OnRightToLeftChanged(e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentVisibleChanged(EventArgs e)
        {
            if (GetState(STATE_VISIBLE))
            {
                OnVisibleChanged(e);
            }
        }

        // OnVisibleChanged/OnParentVisibleChanged is not called when a parent becomes invisible
        internal virtual void OnParentBecameInvisible()
        {
            if (GetState(STATE_VISIBLE))
            {
                // This control became invisible too - notify its children
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
                if (controlsCollection != null)
                {
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        Control ctl = controlsCollection[i];
                        ctl.OnParentBecameInvisible();
                    }
                }
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPrint(PaintEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException(nameof(e));
            }

            if (GetStyle(ControlStyles.UserPaint))
            {
                // Theme support requires that we paint the background
                // and foreground to support semi-transparent children
                PaintWithErrorHandling(e, PaintLayerBackground);
                e.ResetGraphics();
                PaintWithErrorHandling(e, PaintLayerForeground);
            }
            else
            {
                Message m;
                bool releaseDC = false;
                IntPtr hdc = IntPtr.Zero;

                if (!(e is PrintPaintEventArgs ppev))
                {
                    IntPtr flags = (IntPtr)(NativeMethods.PRF_CHILDREN | NativeMethods.PRF_CLIENT | NativeMethods.PRF_ERASEBKGND | NativeMethods.PRF_NONCLIENT);
                    hdc = e.HDC;

                    if (hdc == IntPtr.Zero)
                    {
                        // a manually created paintevent args
                        hdc = e.Graphics.GetHdc();
                        releaseDC = true;
                    }
                    m = Message.Create(Handle, WindowMessages.WM_PRINTCLIENT, hdc, flags);
                }
                else
                {
                    m = ppev.Message;
                }

                try
                {
                    DefWndProc(ref m);
                }
                finally
                {
                    if (releaseDC)
                    {
                        e.Graphics.ReleaseHdcInternal(hdc);
                    }
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTabIndexChanged(EventArgs e)
        {
            if (Events[s_tabIndexEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTabStopChanged(EventArgs e)
        {
            if (Events[s_tabStopEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnTextChanged(EventArgs e)
        {
            if (Events[s_textEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Visible'/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnVisible to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnVisibleChanged(EventArgs e)
        {
            bool visible = Visible;
            if (visible)
            {
                UnhookMouseEvent();
                _trackMouseEvent = null;
            }
            if (_parent != null && visible && !Created)
            {
                bool isDisposing = GetAnyDisposingInHierarchy();
                if (!isDisposing)
                {
                    // Usually the control is created by now, but in a few corner cases
                    // exercised by the PropertyGrid dropdowns, it isn't
                    CreateControl();
                }
            }

            if (Events[s_visibleEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    Control ctl = controlsCollection[i];
                    if (ctl.Visible)
                    {
                        ctl.OnParentVisibleChanged(e);
                    }
                    if (!visible)
                    {
                        ctl.OnParentBecameInvisible();
                    }
                }
            }
        }

        internal virtual void OnTopMostActiveXParentChanged(EventArgs e)
        {
            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnTopMostActiveXParentChanged(e);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnParentChanged(EventArgs e)
        {
            if (Events[s_parentEvent] is EventHandler eh)
            {
                eh(this, e);
            }

            // Inform the control that the topmost control is now an ActiveX control
            if (TopMostParent.IsActiveX)
            {
                OnTopMostActiveXParentChanged(EventArgs.Empty);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Click'/>
        ///  event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClick(EventArgs e)
        {
            ((EventHandler)Events[s_clickEvent])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnClientSizeChanged(EventArgs e)
        {
            if (Events[s_clientSizeEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='ControlAdded'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnControlAdded(ControlEventArgs e)
        {
            ((ControlEventHandler)Events[s_controlAddedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='ControlRemoved'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnControlRemoved(ControlEventArgs e)
        {
            ((ControlEventHandler)Events[s_controlRemovedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Called when the control is first created.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnCreateControl()
        {
        }

        /// <summary>
        ///  Inheriting classes should override this method to find out when the
        ///  handle has been created.
        ///  Call base.OnHandleCreated first.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHandleCreated(EventArgs e)
        {
            if (IsHandleCreated)
            {
                // Setting fonts is for some reason incredibly expensive.
                // (Even if you exclude font handle creation)
                if (!GetStyle(ControlStyles.UserPaint))
                {
                    SetWindowFont();
                }

                if (DpiHelper.IsPerMonitorV2Awareness && !(typeof(Form).IsAssignableFrom(GetType())))
                {
                    int old = _deviceDpi;
                    _deviceDpi = (int)UnsafeNativeMethods.GetDpiForWindow(new HandleRef(this, HandleInternal));
                    if (old != _deviceDpi)
                    {
                        RescaleConstantsForDpi(old, _deviceDpi);
                    }
                }

                // Restore dragdrop status. Ole Initialize happens
                // when the ThreadContext in Application is created
                SetAcceptDrops(AllowDrop);

                Region region = (Region)Properties.GetObject(s_regionProperty);
                if (region != null)
                {
                    IntPtr regionHandle = GetHRgn(region);

                    try
                    {
                        if (IsActiveX)
                        {
                            regionHandle = ActiveXMergeRegion(regionHandle);
                        }

                        if (UnsafeNativeMethods.SetWindowRgn(new HandleRef(this, Handle), new HandleRef(this, regionHandle), SafeNativeMethods.IsWindowVisible(new HandleRef(this, Handle))) != 0)
                        {
                            //The HWnd owns the region.
                            regionHandle = IntPtr.Zero;
                        }
                    }
                    finally
                    {
                        if (regionHandle != IntPtr.Zero)
                        {
                            Gdi32.DeleteObject(regionHandle);
                        }
                    }
                }

                // Cache Handle in a local before asserting so we minimize code running under the Assert.
                IntPtr handle = Handle;

                if (Properties.GetObject(s_accessibilityProperty) is ControlAccessibleObject accObj)
                {
                    accObj.Handle = handle;
                }
                if (Properties.GetObject(s_ncAccessibilityProperty) is ControlAccessibleObject ncAccObj)
                {
                    ncAccObj.Handle = handle;
                }

                // Set the window text from the Text property.
                if (_text != null && _text.Length != 0)
                {
                    User32.SetWindowTextW(new HandleRef(this, Handle), _text);
                }

                if (!(this is ScrollableControl) && !IsMirrored && GetState2(STATE2_SETSCROLLPOS) && !GetState2(STATE2_HAVEINVOKED))
                {
                    BeginInvoke(new EventHandler(OnSetScrollPosition));
                    SetState2(STATE2_HAVEINVOKED, true);
                    SetState2(STATE2_SETSCROLLPOS, false);
                }

                // Listen to UserPreferenceChanged if the control is top level and interested in the notification.
                if (GetState2(STATE2_INTERESTEDINUSERPREFERENCECHANGED))
                {
                    ListenToUserPreferenceChanged(GetTopLevel());
                }
            } ((EventHandler)Events[s_handleCreatedEvent])?.Invoke(this, e);

            if (IsHandleCreated)
            {
                // Now, repost the thread callback message if we found it.  We should do
                // this last, so we're as close to the same state as when the message
                // was placed.
                //
                if (GetState(STATE_THREADMARSHALLPENDING))
                {
                    UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), s_threadCallbackMessage, IntPtr.Zero, IntPtr.Zero);
                    SetState(STATE_THREADMARSHALLPENDING, false);
                }
            }
        }

        private void OnSetScrollPosition(object sender, EventArgs e)
        {
            SetState2(STATE2_HAVEINVOKED, false);
            OnInvokedSetScrollPosition(sender, e);
        }

        internal virtual void OnInvokedSetScrollPosition(object sender, EventArgs e)
        {
            if (!(this is ScrollableControl) && !IsMirrored)
            {
                NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO
                {
                    cbSize = Marshal.SizeOf<NativeMethods.SCROLLINFO>(),
                    fMask = NativeMethods.SIF_RANGE
                };
                if (UnsafeNativeMethods.GetScrollInfo(new HandleRef(this, Handle), NativeMethods.SB_HORZ, si) != false)
                {
                    si.nPos = (RightToLeft == RightToLeft.Yes) ? si.nMax : si.nMin;
                    SendMessage(WindowMessages.WM_HSCROLL, NativeMethods.Util.MAKELPARAM(NativeMethods.SB_THUMBPOSITION, si.nPos), 0);
                }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLocationChanged(EventArgs e)
        {
            OnMove(e);
            if (Events[s_locationEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to find out when the
        ///  handle is about to be destroyed.
        ///  Call base.OnHandleDestroyed last.
        ///  Raises the <see cref='HandleDestroyed'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHandleDestroyed(EventArgs e)
        {
            ((EventHandler)Events[s_handleDestroyedEvent])?.Invoke(this, e);

            UpdateReflectParent(false);

            if (!RecreatingHandle)
            {
                if (GetState(STATE_OWNCTLBRUSH))
                {
                    object backBrush = Properties.GetObject(s_backBrushProperty);
                    if (backBrush != null)
                    {
                        Properties.SetObject(s_backBrushProperty, null);
                        IntPtr p = (IntPtr)backBrush;
                        if (p != IntPtr.Zero)
                        {
                            Gdi32.DeleteObject(p);
                        }
                    }
                }
                ListenToUserPreferenceChanged(false /*listen*/);
            }

            // this code is important -- it is critical that we stash away
            // the value of the text for controls such as edit, button,
            // label, etc. Without this processing, any time you change a
            // property that forces handle recreation, you lose your text!
            // See the above code in wmCreate
            try
            {
                if (!GetAnyDisposingInHierarchy())
                {
                    _text = Text;
                    if (_text != null && _text.Length == 0)
                    {
                        _text = null;
                    }
                }
                SetAcceptDrops(false);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsSecurityOrCriticalException(ex))
                {
                    throw;
                }

                // Some ActiveX controls throw exceptions when
                // you ask for the text property after you have destroyed their handle. We
                // don't want those exceptions to bubble all the way to the top, since
                // we leave our state in a mess.
            }
        }

        /// <summary>
        ///  Raises the <see cref='DoubleClick'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDoubleClick(EventArgs e)
        {
            ((EventHandler)Events[s_doubleClickEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='Enter'/> event.
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onEnter to send this event to any registered event listeners.
        /// </summary>
        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragEnter to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragEnter(DragEventArgs drgevent)
        {
            ((DragEventHandler)Events[s_dragEnterEvent])?.Invoke(this, drgevent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragOver to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragOver(DragEventArgs drgevent)
        {
            ((DragEventHandler)Events[s_dragOverEvent])?.Invoke(this, drgevent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragLeave to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragLeave(EventArgs e)
        {
            ((EventHandler)Events[s_dragLeaveEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onDragDrop to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnDragDrop(DragEventArgs drgevent)
        {
            ((DragEventHandler)Events[s_dragDropEvent])?.Invoke(this, drgevent);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onGiveFeedback to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnGiveFeedback(GiveFeedbackEventArgs gfbevent)
        {
            ((GiveFeedbackEventHandler)Events[s_giveFeedbackEvent])?.Invoke(this, gfbevent);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnEnter(EventArgs e)
        {
            ((EventHandler)Events[s_enterEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='GotFocus'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void InvokeGotFocus(Control toInvoke, EventArgs e)
        {
            if (toInvoke != null)
            {
                toInvoke.OnGotFocus(e);
                KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(toInvoke);
            }
        }

        /// <summary>
        ///  Raises the <see cref='GotFocus'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnGotFocus(EventArgs e)
        {
            if (IsActiveX)
            {
                ActiveXOnFocus(true);
            }

            if (_parent != null)
            {
                _parent.ChildGotFocus(this);
            } ((EventHandler)Events[s_gotFocusEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onHelp to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnHelpRequested(HelpEventArgs hevent)
        {
            HelpEventHandler handler = (HelpEventHandler)Events[s_helpRequestedEvent];
            if (handler != null)
            {
                handler(this, hevent);

                // Mark the event as handled so that the event isn't raised for the
                // control's parent.
                if (hevent != null)
                {
                    hevent.Handled = true;
                }
            }

            if (hevent != null && !hevent.Handled)
            {
                ParentInternal?.OnHelpRequested(hevent);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.OnInvalidate to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnInvalidated(InvalidateEventArgs e)
        {
            // Ask the site to change the view...
            if (IsActiveX)
            {
                ActiveXViewChanged();
            }

            // Transparent control support
            ControlCollection controls = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controls != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controls.Count; i++)
                {
                    controls[i].OnParentInvalidated(e);
                }
            } ((InvalidateEventHandler)Events[s_invalidatedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='KeyDown'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyDown(KeyEventArgs e)
        {
            ((KeyEventHandler)Events[s_keyDownEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='KeyPress'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyPress(KeyPressEventArgs e)
        {
            ((KeyPressEventHandler)Events[s_keyPressEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='KeyUp'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnKeyUp(KeyEventArgs e)
        {
            ((KeyEventHandler)Events[s_keyUpEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Core layout logic. Inheriting controls should override this function to do any custom
        ///  layout logic. It is not neccessary to call base.OnLayout, however for normal docking
        ///  an functions to work, base.OnLayout must be called.
        ///  Raises the <see cref='Layout'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLayout(LayoutEventArgs levent)
        {
            // Ask the site to change the view.
            if (IsActiveX)
            {
                ActiveXViewChanged();
            }

            ((LayoutEventHandler)Events[s_layoutEvent])?.Invoke(this, levent);

            bool parentRequiresLayout = LayoutEngine.Layout(this, levent);
            if (parentRequiresLayout && ParentInternal != null)
            {
                // LayoutEngine.Layout can return true to request that our parent resize us because
                // we did not have enough room for our contents. We can not just call PerformLayout
                // because this container is currently suspended. PerformLayout will check this state
                // flag and PerformLayout on our parent.
                ParentInternal.SetState(STATE_LAYOUTISDIRTY, true);
            }
        }

        /// <summary>
        ///  Called when the last resume layout call is made. If performLayout is true a layout will
        ///  occur as soon as this call returns. Layout is still suspended when this call is made.
        ///  The default implementation calls OnChildLayoutResuming on the parent, if it exists.
        /// </summary>
        internal virtual void OnLayoutResuming(bool performLayout)
        {
            ParentInternal?.OnChildLayoutResuming(this, performLayout);
        }

        internal virtual void OnLayoutSuspended()
        {
        }

        /// <summary>
        ///  Raises the <see cref='Leave'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLeave(EventArgs e)
        {
            ((EventHandler)Events[s_leaveEvent])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void InvokeLostFocus(Control toInvoke, EventArgs e)
        {
            if (toInvoke != null)
            {
                toInvoke.OnLostFocus(e);
                KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(toInvoke);
            }
        }

        /// <summary>
        ///  Raises the <see cref='LostFocus'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnLostFocus(EventArgs e)
        {
            if (IsActiveX)
            {
                ActiveXOnFocus(false);
            } ((EventHandler)Events[s_lostFocusEvent])?.Invoke(this, e);
        }

        protected virtual void OnMarginChanged(EventArgs e)
        {
            ((EventHandler)Events[s_marginChangedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseDoubleClick'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDoubleClick(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[s_mouseDoubleClickEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='OnMouseClick'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseClick(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[s_mouseClickEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseCaptureChanged'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseCaptureChanged(EventArgs e)
        {
            ((EventHandler)Events[s_mouseCaptureChangedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseDown'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseDown(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[s_mouseDownEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseEnter'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseEnter(EventArgs e)
        {
            ((EventHandler)Events[s_mouseEnterEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseLeave'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseLeave(EventArgs e)
        {
            ((EventHandler)Events[s_mouseLeaveEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='DpiChangedBeforeParent'/> event.
        ///  Occurs when the form is moved to a monitor with a different resolution (number of dots per inch),
        ///  or when scaling level is changed in the windows setting by the user.
        ///  This message is not sent to the top level windows.
        /// </summary>
        [
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        protected virtual void OnDpiChangedBeforeParent(EventArgs e)
        {
            ((EventHandler)Events[s_dpiChangedBeforeParentEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='DpiChangedAfterParent'/> event.
        ///  Occurs when the form is moved to a monitor with a different resolution (number of dots per inch),
        ///  or when scaling level is changed in windows setting by the user.
        ///  This message is not sent to the top level windows.
        /// </summary>
        [
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Always)
        ]
        protected virtual void OnDpiChangedAfterParent(EventArgs e)
        {
            ((EventHandler)Events[s_dpiChangedAfterParentEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseHover'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseHover(EventArgs e)
        {
            ((EventHandler)Events[s_mouseHoverEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseMove'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseMove(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[s_mouseMoveEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseUp'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseUp(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[s_mouseUpEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='MouseWheel'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMouseWheel(MouseEventArgs e)
        {
            ((MouseEventHandler)Events[s_mouseWheelEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='Move'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnMove(EventArgs e)
        {
            ((EventHandler)Events[s_moveEvent])?.Invoke(this, e);

            if (RenderTransparent)
            {
                Invalidate();
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onPaint to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPaint(PaintEventArgs e)
        {
            ((PaintEventHandler)Events[s_paintEvent])?.Invoke(this, e);
        }

        protected virtual void OnPaddingChanged(EventArgs e)
        {
            if (GetStyle(ControlStyles.ResizeRedraw))
            {
                Invalidate();
            } ((EventHandler)Events[s_paddingChangedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle the erase
        ///  background request from windows. It is not necessary to call
        ///  base.onPaintBackground, however if you do not want the default
        ///  Windows behavior you must set event.handled to true.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPaintBackground(PaintEventArgs pevent)
        {
            // We need the true client rectangle as clip rectangle causes
            // problems on "Windows Classic" theme.
            RECT rect = new RECT();
            UnsafeNativeMethods.GetClientRect(new HandleRef(_window, InternalHandle), ref rect);

            PaintBackground(pevent, new Rectangle(rect.left, rect.top, rect.right, rect.bottom));
        }

        // Transparent control support
        private void OnParentInvalidated(InvalidateEventArgs e)
        {
            if (!RenderTransparent)
            {
                return;
            }

            if (IsHandleCreated)
            {
                // move invalid rect into child space
                Rectangle cliprect = e.InvalidRect;
                Point offs = Location;
                cliprect.Offset(-offs.X, -offs.Y);
                cliprect = Rectangle.Intersect(ClientRectangle, cliprect);

                // if we don't intersect at all, do nothing
                if (cliprect.IsEmpty)
                {
                    return;
                }

                Invalidate(cliprect);
            }
        }

        /// <summary>
        ///  Inheriting classes should override this method to handle this event.
        ///  Call base.onQueryContinueDrag to send this event to any registered event listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnQueryContinueDrag(QueryContinueDragEventArgs qcdevent)
        {
            ((QueryContinueDragEventHandler)Events[s_queryContinueDragEvent])?.Invoke(this, qcdevent);
        }

        /// <summary>
        ///  Raises the <see cref='RegionChanged'/> event when the Region property has changed.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRegionChanged(EventArgs e)
        {
            if (Events[s_regionChangedEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='Resize'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnResize(EventArgs e)
        {
            if ((_controlStyle & ControlStyles.ResizeRedraw) == ControlStyles.ResizeRedraw
                || GetState(STATE_EXCEPTIONWHILEPAINTING))
            {
                Invalidate();
            }
            LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
            ((EventHandler)Events[s_resizeEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='PreviewKeyDown'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnPreviewKeyDown(PreviewKeyDownEventArgs e)
        {
            ((PreviewKeyDownEventHandler)Events[s_previewKeyDownEvent])?.Invoke(this, e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSizeChanged(EventArgs e)
        {
            OnResize(EventArgs.Empty);

            if (Events[s_sizeEvent] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Raises the <see cref='ChangeUICues'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnChangeUICues(UICuesEventArgs e)
        {
            ((UICuesEventHandler)Events[s_changeUICuesEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='OnStyleChanged'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnStyleChanged(EventArgs e)
        {
            ((EventHandler)Events[s_styleChangedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='SystemColorsChanged'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnSystemColorsChanged(EventArgs e)
        {
            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
            if (controlsCollection != null)
            {
                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                for (int i = 0; i < controlsCollection.Count; i++)
                {
                    controlsCollection[i].OnSystemColorsChanged(EventArgs.Empty);
                }
            }
            Invalidate();

            ((EventHandler)Events[s_systemColorsChangedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='Validating'/>
        ///  event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidating(CancelEventArgs e)
        {
            ((CancelEventHandler)Events[s_validatingEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the <see cref='Validated'/> event.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnValidated(EventArgs e)
        {
            ((EventHandler)Events[s_validatedEvent])?.Invoke(this, e);
        }

        /// <summary>
        ///  Is invoked when the control handle is created or right before the top level parent receives WM_DPICHANGED message.
        ///  This method is an opportunity to rescale any constant sizes, glyphs or bitmaps before re-painting.
        ///  The derived class can choose to not call the base class implementation.
        /// </summary>
        [
            Browsable(true),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        protected virtual void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
        }

        // This is basically OnPaintBackground, put in a separate method for ButtonBase,
        // which does all painting under OnPaint, and tries very hard to avoid double-painting the border pixels.
        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle)
        {
            PaintBackground(e, rectangle, BackColor, Point.Empty);
        }

        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor)
        {
            PaintBackground(e, rectangle, backColor, Point.Empty);
        }

        internal void PaintBackground(PaintEventArgs e, Rectangle rectangle, Color backColor, Point scrollOffset)
        {
            if (RenderColorTransparent(backColor))
            {
                PaintTransparentBackground(e, rectangle);
            }

            //If the form or mdiclient is mirrored then we do not render the background image due to GDI+ issues.
            bool formRTL = ((this is Form || this is MdiClient) && IsMirrored);

            // The rest of this won't do much if BackColor is transparent and there is no BackgroundImage,
            // but we need to call it in the partial alpha case.

            if (BackgroundImage != null && !DisplayInformation.HighContrast && !formRTL)
            {
                if (BackgroundImageLayout == ImageLayout.Tile)
                {
                    if (ControlPaint.IsImageTransparent(BackgroundImage))
                    {
                        PaintTransparentBackground(e, rectangle);
                    }
                }

                Point scrollLocation = scrollOffset;
                if (this is ScrollableControl scrollControl && scrollLocation != Point.Empty)
                {
                    scrollLocation = ((ScrollableControl)this).AutoScrollPosition;
                }

                if (ControlPaint.IsImageTransparent(BackgroundImage))
                {
                    PaintBackColor(e, rectangle, backColor);
                }

                ControlPaint.DrawBackgroundImage(e.Graphics, BackgroundImage, backColor, BackgroundImageLayout, ClientRectangle, rectangle, scrollLocation, RightToLeft);
            }
            else
            {
                PaintBackColor(e, rectangle, backColor);
            }
        }
        private static void PaintBackColor(PaintEventArgs e, Rectangle rectangle, Color backColor)
        {
            // Common case of just painting the background.  For this, we
            // use GDI because it is faster for simple things than creating
            // a graphics object, brush, etc.  Also, we may be able to
            // use a system brush, avoiding the brush create altogether.
            //
            Color color = backColor;

            // Note: PaintEvent.HDC == 0 if GDI+ has used the HDC -- it wouldn't be safe for us
            // to use it without enough bookkeeping to negate any performance gain of using GDI.
            if (color.A == 255)
            {
                using (WindowsGraphics wg = (e.HDC != IntPtr.Zero && DisplayInformation.BitsPerPixel > 8) ?
                                WindowsGraphics.FromHdc(e.HDC) : WindowsGraphics.FromGraphics(e.Graphics))
                {
                    color = wg.GetNearestColor(color);

                    using (WindowsBrush brush = new WindowsSolidBrush(wg.DeviceContext, color))
                    {
                        wg.FillRectangle(brush, rectangle);
                    }
                }
            }
            else
            {
                // don't paint anything from 100% transparent background
                if (color.A > 0)
                {
                    // Color has some transparency or we have no HDC, so we must
                    // fall back to using GDI+.
                    using (Brush brush = new SolidBrush(color))
                    {
                        e.Graphics.FillRectangle(brush, rectangle);
                    }
                }
            }
        }

        // Paints a red rectangle with a red X, painted on a white background
        private void PaintException(PaintEventArgs e)
        {
            int penThickness = 2;
            using (Pen pen = new Pen(Color.Red, penThickness))
            {
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
        }

        internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle)
        {
            PaintTransparentBackground(e, rectangle, null);
        }

        // Trick our parent into painting our background for us, or paint some default
        // color if that doesn't work.
        //
        // This method is the hardest part of implementing transparent controls;
        // call this in your OnPaintBackground method, and away you go.
        //
        // If you only want a region of the control to be transparent, pass in a region into the
        // last parameter.  A null region implies that you want the entire rectangle to be transparent.
        internal void PaintTransparentBackground(PaintEventArgs e, Rectangle rectangle, Region transparentRegion)
        {
            Graphics g = e.Graphics;
            Control parent = ParentInternal;

            if (parent != null)
            {
                // We need to use themeing painting for certain controls (like TabPage) when they parent other controls.
                // But we dont want to to this always as this causes serious preformance (at Runtime and DesignTime)
                // so checking for RenderTransparencyWithVisualStyles which is TRUE for TabPage and false by default.
                if (Application.RenderWithVisualStyles && parent.RenderTransparencyWithVisualStyles)
                {
                    // When we are rendering with visual styles, we can use the cool DrawThemeParentBackground function
                    // that UxTheme provides to render the parent's background. This function is control agnostic, so
                    // we use the wrapper in ButtonRenderer - this should do the right thing for all controls,
                    // not just Buttons.

                    GraphicsState graphicsState = null;
                    if (transparentRegion != null)
                    {
                        graphicsState = g.Save();
                    }
                    try
                    {
                        if (transparentRegion != null)
                        {
                            g.Clip = transparentRegion;
                        }
                        ButtonRenderer.DrawParentBackground(g, rectangle, this);
                    }
                    finally
                    {
                        if (graphicsState != null)
                        {
                            g.Restore(graphicsState);
                        }
                    }
                }
                else
                {
                    // how to move the rendering area and setup it's size
                    // (we want to translate it to the parent's origin)
                    Rectangle shift = new Rectangle(-Left, -Top, parent.Width, parent.Height);

                    // moving the clipping rectangle to the parent coordinate system
                    Rectangle newClipRect = new Rectangle(rectangle.Left + Left, rectangle.Top + Top, rectangle.Width, rectangle.Height);

                    using (WindowsGraphics wg = WindowsGraphics.FromGraphics(g))
                    {
                        wg.DeviceContext.TranslateTransform(-Left, -Top);
                        using (PaintEventArgs np = new PaintEventArgs(wg.GetHdc(), newClipRect))
                        {
                            if (transparentRegion != null)
                            {
                                np.Graphics.Clip = transparentRegion;
                                np.Graphics.TranslateClip(-shift.X, -shift.Y);
                            }
                            try
                            {
                                InvokePaintBackground(parent, np);
                                InvokePaint(parent, np);
                            }
                            finally
                            {
                                if (transparentRegion != null)
                                {
                                    // restore region back to original state.
                                    np.Graphics.TranslateClip(shift.X, shift.Y);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // For whatever reason, our parent can't paint our background, but we need some kind of background
                // since we're transparent.
                g.FillRectangle(SystemBrushes.Control, rectangle);
            }
        }

        // Exceptions during painting are nasty, because paint events happen so often.
        // So if user painting code has an issue, we make sure never to call it again,
        // so as not to spam the end-user with exception dialogs.
        private void PaintWithErrorHandling(PaintEventArgs e, short layer)
        {
            try
            {
                CacheTextInternal = true;
                if (GetState(STATE_EXCEPTIONWHILEPAINTING))
                {
                    if (layer == PaintLayerBackground)
                    {
                        PaintException(e);
                    }
                }
                else
                {
                    bool exceptionThrown = true;
                    try
                    {
                        switch (layer)
                        {
                            case PaintLayerForeground:
                                OnPaint(e);
                                break;
                            case PaintLayerBackground:
                                if (!GetStyle(ControlStyles.Opaque))
                                {
                                    OnPaintBackground(e);
                                }
                                break;
                            default:
                                Debug.Fail("Unknown PaintLayer " + layer);
                                break;
                        }
                        exceptionThrown = false;
                    }
                    finally
                    {
                        if (exceptionThrown)
                        {
                            SetState(STATE_EXCEPTIONWHILEPAINTING, true);
                            Invalidate();
                        }
                    }
                }
            }
            finally
            {
                CacheTextInternal = false;
            }
        }

        /// <summary>
        ///  Find ContainerControl that is the container of this control.
        /// </summary>
        internal ContainerControl ParentContainerControl
        {
            get
            {
                for (Control c = ParentInternal; c != null; c = c.ParentInternal)
                {
                    if (c is ContainerControl)
                    {
                        return c as ContainerControl;
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Forces the control to apply layout logic to all of the child controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void PerformLayout()
        {
            if (_cachedLayoutEventArgs != null)
            {
                PerformLayout(_cachedLayoutEventArgs);
                _cachedLayoutEventArgs = null;

                // we need to be careful
                // about which LayoutEventArgs are used in
                // SuspendLayout, PerformLayout, ResumeLayout() sequences.
                SetState2(STATE2_CLEARLAYOUTARGS, false);
            }
            else
            {
                PerformLayout(null, null);
            }
        }

        /// <summary>
        ///  Forces the control to apply layout logic to all of the child controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void PerformLayout(Control affectedControl, string affectedProperty)
        {
            PerformLayout(new LayoutEventArgs(affectedControl, affectedProperty));
        }

        internal void PerformLayout(LayoutEventArgs args)
        {
            if (GetAnyDisposingInHierarchy())
            {
                return;
            }

            if (_layoutSuspendCount > 0)
            {
                SetState(STATE_LAYOUTDEFERRED, true);
                if (_cachedLayoutEventArgs == null || (GetState2(STATE2_CLEARLAYOUTARGS) && args != null))
                {
                    _cachedLayoutEventArgs = args;
                    if (GetState2(STATE2_CLEARLAYOUTARGS))
                    {
                        SetState2(STATE2_CLEARLAYOUTARGS, false);
                    }
                }

                LayoutEngine.ProcessSuspendedLayoutEventArgs(this, args);

                return;
            }

            // (Essentially the same as suspending layout while we layout, but we clear differently below.)
            _layoutSuspendCount = 1;

            try
            {
                CacheTextInternal = true;
                OnLayout(args);
            }
            finally
            {
                CacheTextInternal = false;
                // Rather than resume layout (which will could allow a deferred layout to layout the
                // the container we just finished laying out) we set layoutSuspendCount back to zero
                // and clear the deferred and dirty flags.
                SetState(STATE_LAYOUTDEFERRED | STATE_LAYOUTISDIRTY, false);
                _layoutSuspendCount = 0;

                // LayoutEngine.Layout can return true to request that our parent resize us because
                // we did not have enough room for our contents.  Now that we are unsuspended,
                // see if this happened and layout parent if necessary.  (See also OnLayout)
                if (ParentInternal != null && ParentInternal.GetState(STATE_LAYOUTISDIRTY))
                {
                    LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.PreferredSize);
                }
            }
        }

        /// <summary>
        ///  Peforms data validation (not paint validation!) on a single control.
        ///
        ///  Returns whether validation failed:
        ///  False = Validation succeeded, control is valid, accept its new value
        ///  True = Validation was cancelled, control is invalid, reject its new value
        ///
        ///  NOTE: This is the lowest possible level of validation. It does not account
        ///  for the context in which the validation is occuring, eg. change of focus
        ///  between controls in a container. Stuff like that is handled by the caller.
        /// </summary>
        internal bool PerformControlValidation(bool bulkValidation)
        {
            // Skip validation for controls that don't support it
            if (!CausesValidation)
            {
                return false;
            }

            // Raise the 'Validating' event. Stop now if handler cancels (ie. control is invalid).
            // NOTE: Handler may throw an exception here, but we must not attempt to catch it.
            if (NotifyValidating())
            {
                return true;
            }

            // Raise the 'Validated' event. Handlers may throw exceptions here too - but
            // convert these to ThreadException events, unless the app is being debugged,
            // or the control is being validated as part of a bulk validation operation.
            if (bulkValidation || NativeWindow.WndProcShouldBeDebuggable)
            {
                NotifyValidated();
            }
            else
            {
                try
                {
                    NotifyValidated();
                }
                catch (Exception e)
                {
                    Application.OnThreadException(e);
                }
            }

            return false;
        }

        /// <summary>
        ///  Validates all the child controls in a container control. Exactly which controls are
        ///  validated and which controls are skipped is determined by <paramref name="validationConstraints"/>.
        ///  Return value indicates whether validation failed for any of the controls validated.
        ///  Calling function is responsible for checking the correctness of the validationConstraints argument.
        /// </summary>
        internal bool PerformContainerValidation(ValidationConstraints validationConstraints)
        {
            bool failed = false;

            // For every child control of this container control...
            foreach (Control c in Controls)
            {

                // First, if the control is a container, recurse into its descendants.
                if ((validationConstraints & ValidationConstraints.ImmediateChildren) != ValidationConstraints.ImmediateChildren &&
                    c.ShouldPerformContainerValidation() &&
                    c.PerformContainerValidation(validationConstraints))
                {
                    failed = true;
                }

                // Next, use input flags to decide whether to validate the control itself
                if ((validationConstraints & ValidationConstraints.Selectable) == ValidationConstraints.Selectable && !c.GetStyle(ControlStyles.Selectable) ||
                    (validationConstraints & ValidationConstraints.Enabled) == ValidationConstraints.Enabled && !c.Enabled ||
                    (validationConstraints & ValidationConstraints.Visible) == ValidationConstraints.Visible && !c.Visible ||
                    (validationConstraints & ValidationConstraints.TabStop) == ValidationConstraints.TabStop && !c.TabStop)
                {
                    continue;
                }

                // Finally, perform validation on the control itself
                if (c.PerformControlValidation(true))
                {
                    failed = true;
                }
            }

            return failed;
        }

        /// <summary>
        ///  Computes the location of the screen point p in client coords.
        /// </summary>
        public Point PointToClient(Point p)
        {
            UnsafeNativeMethods.MapWindowPoints(NativeMethods.NullHandleRef, new HandleRef(this, Handle), ref p, 1);
            return p;
        }

        /// <summary>
        ///  Computes the location of the client point p in screen coords.
        /// </summary>
        public Point PointToScreen(Point p)
        {
            UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, Handle), NativeMethods.NullHandleRef, ref p, 1);
            return p;
        }

        /// <summary>
        ///  This method is called by the application's message loop to pre-process
        ///  input messages before they are dispatched. Possible values for the
        ///  msg.message field are WM_KEYDOWN, WM_SYSKEYDOWN, WM_CHAR, and WM_SYSCHAR.
        ///  If this method processes the message it must return true, in which case
        ///  the message loop will not dispatch the message.
        ///  For WM_KEYDOWN and WM_SYSKEYDOWN messages, preProcessMessage() first
        ///  calls processCmdKey() to check for command keys such as accelerators and
        ///  menu shortcuts. If processCmdKey() doesn't process the message, then
        ///  isInputKey() is called to check whether the key message represents an
        ///  input key for the control. Finally, if isInputKey() indicates that the
        ///  control isn't interested in the key message, then processDialogKey() is
        ///  called to check for dialog keys such as TAB, arrow keys, and mnemonics.
        ///  For WM_CHAR messages, preProcessMessage() first calls isInputChar() to
        ///  check whether the character message represents an input character for
        ///  the control. If isInputChar() indicates that the control isn't interested
        ///  in the character message, then processDialogChar() is called to check for
        ///  dialog characters such as mnemonics.
        ///  For WM_SYSCHAR messages, preProcessMessage() calls processDialogChar()
        ///  to check for dialog characters such as mnemonics.
        ///  When overriding preProcessMessage(), a control should return true to
        ///  indicate that it has processed the message. For messages that aren't
        ///  processed by the control, the result of "base.preProcessMessage()"
        ///  should be returned. Controls will typically override one of the more
        ///  specialized methods (isInputChar(), isInputKey(), processCmdKey(),
        ///  processDialogChar(), or processDialogKey()) instead of overriding
        ///  preProcessMessage().
        /// </summary>
        public virtual bool PreProcessMessage(ref Message msg)
        {
            //   Debug.WriteLineIf(ControlKeyboardRouting.TraceVerbose, "Control.PreProcessMessage " + msg.ToString());

            bool ret;

            if (msg.Msg == WindowMessages.WM_KEYDOWN || msg.Msg == WindowMessages.WM_SYSKEYDOWN)
            {
                if (!GetState2(STATE2_UICUES))
                {
                    ProcessUICues(ref msg);
                }

                Keys keyData = (Keys)(unchecked((int)(long)msg.WParam) | (int)ModifierKeys);
                if (ProcessCmdKey(ref msg, keyData))
                {
                    ret = true;
                }
                else if (IsInputKey(keyData))
                {
                    SetState2(STATE2_INPUTKEY, true);
                    ret = false;
                }
                else
                {
                    ret = ProcessDialogKey(keyData);
                }
            }
            else if (msg.Msg == WindowMessages.WM_CHAR || msg.Msg == WindowMessages.WM_SYSCHAR)
            {
                if (msg.Msg == WindowMessages.WM_CHAR && IsInputChar((char)msg.WParam))
                {
                    SetState2(STATE2_INPUTCHAR, true);
                    ret = false;
                }
                else
                {
                    ret = ProcessDialogChar((char)msg.WParam);
                }
            }
            else
            {
                ret = false;
            }

            return ret;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public PreProcessControlState PreProcessControlMessage(ref Message msg)
        {
            return PreProcessControlMessageInternal(null, ref msg);
        }

        /// <summary>
        ///  This method is similar to PreProcessMessage, but instead of indicating
        ///  that the message was either processed or it wasn't, it has three return
        ///  values:
        ///
        ///  MessageProcessed - PreProcessMessage() returns true, and the message
        ///                  needs no further processing
        ///
        ///  MessageNeeded    - PreProcessMessage() returns false, but IsInputKey/Char
        ///                  return true.  This means the message wasn't processed,
        ///                  but the control is interested in it.
        ///
        ///  MessageNotNeeded - PreProcessMessage() returns false, and IsInputKey/Char
        ///                  return false.
        /// </summary>
        internal static PreProcessControlState PreProcessControlMessageInternal(Control target, ref Message msg)
        {
            if (target == null)
            {
                target = FromChildHandle(msg.HWnd);
            }

            if (target == null)
            {
                return PreProcessControlState.MessageNotNeeded;
            }

            // reset state that is used to make sure IsInputChar, IsInputKey and
            // ProcessUICues are not called multiple times.
            // ISSUE: Which control should these state bits be set on? probably the target.
            target.SetState2(STATE2_INPUTKEY, false);
            target.SetState2(STATE2_INPUTCHAR, false);
            target.SetState2(STATE2_UICUES, true);

            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.PreProcessControlMessageInternal " + msg.ToString());

            try
            {
                Keys keyData = (Keys)(unchecked((int)(long)msg.WParam) | (int)ModifierKeys);

                // Allow control to preview key down message.
                if (msg.Msg == WindowMessages.WM_KEYDOWN || msg.Msg == WindowMessages.WM_SYSKEYDOWN)
                {
                    target.ProcessUICues(ref msg);

                    PreviewKeyDownEventArgs args = new PreviewKeyDownEventArgs(keyData);
                    target.OnPreviewKeyDown(args);

                    if (args.IsInputKey)
                    {
                        Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "PreviewKeyDown indicated this is an input key.");
                        // Control wants this message - indicate it should be dispatched.
                        return PreProcessControlState.MessageNeeded;
                    }
                }

                PreProcessControlState state = PreProcessControlState.MessageNotNeeded;

                if (!target.PreProcessMessage(ref msg))
                {
                    if (msg.Msg == WindowMessages.WM_KEYDOWN || msg.Msg == WindowMessages.WM_SYSKEYDOWN)
                    {
                        // check if IsInputKey has already procssed this message
                        // or if it is safe to call - we only want it to be called once.
                        if (target.GetState2(STATE2_INPUTKEY) || target.IsInputKey(keyData))
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control didn't preprocess this message but it needs to be dispatched");
                            state = PreProcessControlState.MessageNeeded;
                        }
                    }
                    else if (msg.Msg == WindowMessages.WM_CHAR || msg.Msg == WindowMessages.WM_SYSCHAR)
                    {
                        // check if IsInputChar has already procssed this message
                        // or if it is safe to call - we only want it to be called once.
                        if (target.GetState2(STATE2_INPUTCHAR) || target.IsInputChar((char)msg.WParam))
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control didn't preprocess this message but it needs to be dispatched");
                            state = PreProcessControlState.MessageNeeded;
                        }
                    }
                }
                else
                {
                    state = PreProcessControlState.MessageProcessed;
                }

                return state;
            }
            finally
            {
                target.SetState2(STATE2_UICUES, false);
            }
        }

        /// <summary>
        ///  Processes a command key. This method is called during message
        ///  pre-processing to handle command keys. Command keys are keys that always
        ///  take precedence over regular input keys. Examples of command keys
        ///  include accelerators and menu shortcuts. The method must return true to
        ///  indicate that it has processed the command key, or false to indicate
        ///  that the key is not a command key.
        ///  processCmdKey() first checks if the control has a context menu, and if
        ///  so calls the menu's processCmdKey() to check for menu shortcuts. If the
        ///  command key isn't a menu shortcut, and if the control has a parent, the
        ///  key is passed to the parent's processCmdKey() method. The net effect is
        ///  that command keys are "bubbled" up the control hierarchy.
        ///  When overriding processCmdKey(), a control should return true to
        ///  indicate that it has processed the key. For keys that aren't processed by
        ///  the control, the result of "base.processCmdKey()" should be returned.
        ///  Controls will seldom, if ever, need to override this method.
        /// </summary>
        protected virtual bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessCmdKey " + msg.ToString());

            if (_parent != null)
            {
                return _parent.ProcessCmdKey(ref msg, keyData);
            }

            return false;
        }

        private void PrintToMetaFile(HandleRef hDC, IntPtr lParam)
        {
            Debug.Assert(Gdi32.GetObjectType(hDC) == Gdi32.ObjectType.OBJ_ENHMETADC,
                "PrintToMetaFile() called with a non-Enhanced MetaFile DC.");
            Debug.Assert(((long)lParam & NativeMethods.PRF_CHILDREN) != 0,
                "PrintToMetaFile() called without PRF_CHILDREN.");

            // Strip the PRF_CHILDREN flag.  We will manually walk our children and print them.
            lParam = (IntPtr)((long)lParam & ~NativeMethods.PRF_CHILDREN);

            // We're the root contol, so we need to set up our clipping region.  Retrieve the
            // x-coordinates and y-coordinates of the viewport origin for the specified device context.
            bool success = SafeNativeMethods.GetViewportOrgEx(hDC, out Point viewportOrg);
            Debug.Assert(success, "GetViewportOrgEx() failed.");

            IntPtr hClippingRegion = Gdi32.CreateRectRgn(viewportOrg.X, viewportOrg.Y, viewportOrg.X + Width, viewportOrg.Y + Height);
            Debug.Assert(hClippingRegion != IntPtr.Zero, "CreateRectRgn() failed.");

            try
            {
                // Select the new clipping region; make sure it's a SIMPLEREGION or NULLREGION
                RegionType selectResult = Gdi32.SelectClipRgn(hDC, hClippingRegion);
                Debug.Assert((selectResult == RegionType.SIMPLEREGION ||
                              selectResult == RegionType.NULLREGION),
                              "SIMPLEREGION or NULLLREGION expected.");

                PrintToMetaFileRecursive(hDC, lParam, new Rectangle(Point.Empty, Size));
            }
            finally
            {
                success = Gdi32.DeleteObject(hClippingRegion) != BOOL.FALSE;
                Debug.Assert(success, "DeleteObject() failed.");
            }
        }

        internal virtual void PrintToMetaFileRecursive(HandleRef hDC, IntPtr lParam, Rectangle bounds)
        {
            // We assume the target does not want us to offset the root control in the metafile.

            using (WindowsFormsUtils.DCMapping mapping = new WindowsFormsUtils.DCMapping(hDC, bounds))
            {
                // print the non-client area
                PrintToMetaFile_SendPrintMessage(hDC, (IntPtr)((long)lParam & ~NativeMethods.PRF_CLIENT));

                // figure out mapping for the client area
                RECT windowRect = new RECT();
                bool success = UnsafeNativeMethods.GetWindowRect(new HandleRef(null, Handle), ref windowRect);
                Debug.Assert(success, "GetWindowRect() failed.");
                Point clientOffset = PointToScreen(Point.Empty);
                clientOffset = new Point(clientOffset.X - windowRect.left, clientOffset.Y - windowRect.top);
                Rectangle clientBounds = new Rectangle(clientOffset, ClientSize);

                using (WindowsFormsUtils.DCMapping clientMapping = new WindowsFormsUtils.DCMapping(hDC, clientBounds))
                {
                    // print the client area
                    PrintToMetaFile_SendPrintMessage(hDC, (IntPtr)((long)lParam & ~NativeMethods.PRF_NONCLIENT));

                    // Paint children in reverse Z-Order
                    int count = Controls.Count;
                    for (int i = count - 1; i >= 0; i--)
                    {
                        Control child = Controls[i];
                        if (child.Visible)
                        {
                            child.PrintToMetaFileRecursive(hDC, lParam, child.Bounds);
                        }
                    }
                }
            }
        }

        private void PrintToMetaFile_SendPrintMessage(HandleRef hDC, IntPtr lParam)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                // We let user paint controls paint directly into the metafile
                SendMessage(WindowMessages.WM_PRINT, hDC.Handle, lParam);
            }
            else
            {
                // If a system control has no children in the Controls collection we
                // restore the the PRF_CHILDREN flag because it may internally
                // have nested children we do not know about.  ComboBox is a
                // good example.
                if (Controls.Count == 0)
                {
                    lParam = (IntPtr)((long)lParam | NativeMethods.PRF_CHILDREN);
                }

                // System controls must be painted into a temporary bitmap
                // which is then copied into the metafile.  (Old GDI line drawing
                // is 1px thin, which causes borders to disappear, etc.)
                using (MetafileDCWrapper dcWrapper = new MetafileDCWrapper(hDC, Size))
                {
                    SendMessage(WindowMessages.WM_PRINT, dcWrapper.HDC, lParam);
                }
            }
        }

        /// <summary>
        ///  Processes a dialog character. This method is called during message
        ///  pre-processing to handle dialog characters, such as control mnemonics.
        ///  This method is called only if the isInputChar() method indicates that
        ///  the control isn't interested in the character.
        ///  processDialogChar() simply sends the character to the parent's
        ///  processDialogChar() method, or returns false if the control has no
        ///  parent. The Form class overrides this method to perform actual
        ///  processing of dialog characters.
        ///  When overriding processDialogChar(), a control should return true to
        ///  indicate that it has processed the character. For characters that aren't
        ///  processed by the control, the result of "base.processDialogChar()"
        ///  should be returned.
        ///  Controls will seldom, if ever, need to override this method.
        /// </summary>
        protected virtual bool ProcessDialogChar(char charCode)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessDialogChar [" + charCode.ToString() + "]");
            return _parent == null ? false : _parent.ProcessDialogChar(charCode);
        }

        /// <summary>
        ///  Processes a dialog key. This method is called during message
        ///  pre-processing to handle dialog characters, such as TAB, RETURN, ESCAPE,
        ///  and arrow keys. This method is called only if the isInputKey() method
        ///  indicates that the control isn't interested in the key.
        ///  processDialogKey() simply sends the character to the parent's
        ///  processDialogKey() method, or returns false if the control has no
        ///  parent. The Form class overrides this method to perform actual
        ///  processing of dialog keys.
        ///  When overriding processDialogKey(), a control should return true to
        ///  indicate that it has processed the key. For keys that aren't processed
        ///  by the control, the result of "base.processDialogKey(...)" should be
        ///  returned.
        ///  Controls will seldom, if ever, need to override this method.
        /// </summary>
        protected virtual bool ProcessDialogKey(Keys keyData)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessDialogKey " + keyData.ToString());
            return _parent == null ? false : _parent.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  Processes a key message. This method is called when a control receives a
        ///  keyboard message. The method is responsible for generating the appropriate
        ///  key events for the message by calling OnKeyPress(), onKeyDown(), or
        ///  onKeyUp(). The m parameter contains the window message that must
        ///  be processed. Possible values for the m.msg field are WM_CHAR,
        ///  WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, WM_SYSKEYUP, and WM_IMECHAR.
        ///  When overriding processKeyEventArgs(), a control should return true to
        ///  indicate that it has processed the key. For keys that aren't processed
        ///  by the control, the result of "base.processKeyEventArgs()" should be
        ///  returned.
        ///  Controls will seldom, if ever, need to override this method.
        /// </summary>
        protected virtual bool ProcessKeyEventArgs(ref Message m)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessKeyEventArgs " + m.ToString());
            KeyEventArgs ke = null;
            KeyPressEventArgs kpe = null;
            IntPtr newWParam = IntPtr.Zero;

            if (m.Msg == WindowMessages.WM_CHAR || m.Msg == WindowMessages.WM_SYSCHAR)
            {
                int charsToIgnore = ImeWmCharsToIgnore;

                if (charsToIgnore > 0)
                {
                    charsToIgnore--;
                    Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "charsToIgnore decreased, new val = " + charsToIgnore + ", this=" + this);

                    ImeWmCharsToIgnore = charsToIgnore;
                    return false;
                }
                else
                {
                    kpe = new KeyPressEventArgs(unchecked((char)(long)m.WParam));
                    OnKeyPress(kpe);
                    newWParam = (IntPtr)kpe.KeyChar;
                }
            }
            else if (m.Msg == WindowMessages.WM_IME_CHAR)
            {
                int charsToIgnore = ImeWmCharsToIgnore;

                charsToIgnore += (3 - sizeof(char));
                ImeWmCharsToIgnore = charsToIgnore;

                kpe = new KeyPressEventArgs(unchecked((char)(long)m.WParam));

                char preEventCharacter = kpe.KeyChar;
                OnKeyPress(kpe);

                //If the character wasn't changed, just use the original value rather than round tripping.
                if (kpe.KeyChar == preEventCharacter)
                {
                    newWParam = m.WParam;
                }
                else
                {
                    newWParam = (IntPtr)kpe.KeyChar;
                }
            }
            else
            {
                ke = new KeyEventArgs((Keys)(unchecked((int)(long)m.WParam)) | ModifierKeys);
                if (m.Msg == WindowMessages.WM_KEYDOWN || m.Msg == WindowMessages.WM_SYSKEYDOWN)
                {
                    OnKeyDown(ke);
                }
                else
                {
                    OnKeyUp(ke);
                }
            }

            if (kpe != null)
            {
                Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "    processkeyeventarg returning: " + kpe.Handled);
                m.WParam = newWParam;
                return kpe.Handled;
            }
            else
            {
                Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "    processkeyeventarg returning: " + ke.Handled);
                if (ke.SuppressKeyPress)
                {
                    RemovePendingMessages(WindowMessages.WM_CHAR, WindowMessages.WM_CHAR);
                    RemovePendingMessages(WindowMessages.WM_SYSCHAR, WindowMessages.WM_SYSCHAR);
                    RemovePendingMessages(WindowMessages.WM_IME_CHAR, WindowMessages.WM_IME_CHAR);
                }
                return ke.Handled;
            }
        }

        /// <summary>
        ///  Processes a key message. This method is called when a control receives a
        ///  keyboard message. The method first checks if the control has a parent,
        ///  and if so calls the parent's processKeyPreview() method. If the parent's
        ///  processKeyPreview() method doesn't consume the message then
        ///  processKeyEventArgs() is called to generate the appropriate keyboard events.
        ///  The m parameter contains the window message that must be
        ///  processed. Possible values for the m.msg field are WM_CHAR,
        ///  WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and WM_SYSKEYUP.
        ///  When overriding processKeyMessage(), a control should return true to
        ///  indicate that it has processed the key. For keys that aren't processed
        ///  by the control, the result of "base.processKeyMessage()" should be
        ///  returned.
        ///  Controls will seldom, if ever, need to override this method.
        /// </summary>
        protected internal virtual bool ProcessKeyMessage(ref Message m)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessKeyMessage " + m.ToString());
            if (_parent != null && _parent.ProcessKeyPreview(ref m))
            {
                return true;
            }

            return ProcessKeyEventArgs(ref m);
        }

        /// <summary>
        ///  Previews a keyboard message. This method is called by a child control
        ///  when the child control receives a keyboard message. The child control
        ///  calls this method before generating any keyboard events for the message.
        ///  If this method returns true, the child control considers the message
        ///  consumed and does not generate any keyboard events. The m
        ///  parameter contains the window message to preview. Possible values for
        ///  the m.msg field are WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP,
        ///  and WM_SYSKEYUP.
        ///  processKeyPreview() simply sends the character to the parent's
        ///  processKeyPreview() method, or returns false if the control has no
        ///  parent. The Form class overrides this method to perform actual
        ///  processing of dialog keys.
        ///  When overriding processKeyPreview(), a control should return true to
        ///  indicate that it has processed the key. For keys that aren't processed
        ///  by the control, the result of "base.ProcessKeyPreview(...)" should be
        ///  returned.
        /// </summary>
        protected virtual bool ProcessKeyPreview(ref Message m)
        {
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessKeyPreview " + m.ToString());
            return _parent == null ? false : _parent.ProcessKeyPreview(ref m);
        }

        /// <summary>
        ///  Processes a mnemonic character. This method is called to give a control
        ///  the opportunity to process a mnemonic character. The method should check
        ///  if the control is in a state to process mnemonics and if the given
        ///  character represents a mnemonic. If so, the method should perform the
        ///  action associated with the mnemonic and return true. If not, the method
        ///  should return false.
        ///  Implementations of this method often use the isMnemonic() method to
        ///  check if the given character matches a mnemonic in the control's text,
        ///  for example:
        /// <code>
        ///  if (canSelect() &amp;&amp; isMnemonic(charCode, getText()) {
        ///  // perform action associated with mnemonic
        ///  }
        /// </code>
        ///  This default implementation of processMnemonic() simply returns false
        ///  to indicate that the control has no mnemonic.
        /// </summary>
        protected internal virtual bool ProcessMnemonic(char charCode)
        {
#if DEBUG
            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.ProcessMnemonic [0x" + ((int)charCode).ToString("X", CultureInfo.InvariantCulture) + "]");
#endif
            return false;
        }

        /// <summary>
        ///  Preprocess keys which affect focus indicators and keyboard cues.
        /// </summary>
        internal void ProcessUICues(ref Message msg)
        {
            Keys keyCode = (Keys)((int)msg.WParam) & Keys.KeyCode;

            if (keyCode != Keys.F10 && keyCode != Keys.Menu && keyCode != Keys.Tab)
            {
                return;  // PERF: dont WM_QUERYUISTATE if we dont have to.
            }

            Control topMostParent = null;
            int current = unchecked((int)(long)SendMessage(WindowMessages.WM_QUERYUISTATE, 0, 0));

            // dont trust when a control says the accelerators are showing.
            // make sure the topmost parent agrees with this as we could be in a mismatched state.
            if (current == 0 /*accelerator and focus cues are showing*/)
            {
                topMostParent = TopMostParent;
                current = (int)topMostParent.SendMessage(WindowMessages.WM_QUERYUISTATE, 0, 0);
            }
            int toClear = 0;

            // if we are here, a key or tab has been pressed on this control.
            // now that we know the state of accelerators, check to see if we need
            // to show them.  NOTE: due to the strangeness of the API we OR in
            // the opposite of what we want to do.  So if we want to show accelerators,
            // we OR in UISF_HIDEACCEL, then call UIS_CLEAR to clear the "hidden" state.

            if (keyCode == Keys.F10 || keyCode == Keys.Menu)
            {
                if ((current & NativeMethods.UISF_HIDEACCEL) != 0)
                {
                    // Keyboard accelerators are hidden, they need to be shown
                    toClear |= NativeMethods.UISF_HIDEACCEL;
                }
            }

            if (keyCode == Keys.Tab)
            {
                if ((current & NativeMethods.UISF_HIDEFOCUS) != 0)
                {
                    // Focus indicators are hidden, they need to be shown
                    toClear |= NativeMethods.UISF_HIDEFOCUS;
                }
            }

            if (toClear != 0)
            {
                // We've detected some state we need to unset, usually clearing the hidden state of
                // the accelerators.  We need to get the topmost parent and call CHANGEUISTATE so
                // that the entire tree of controls is
                if (topMostParent == null)
                {
                    topMostParent = TopMostParent;
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
                    UnsafeNativeMethods.GetParent(new HandleRef(null, topMostParent.Handle)) == IntPtr.Zero ? WindowMessages.WM_CHANGEUISTATE : WindowMessages.WM_UPDATEUISTATE,
                    (IntPtr)(NativeMethods.UIS_CLEAR | (toClear << 16)),
                    IntPtr.Zero);
            }
        }

        /// <summary>
        ///  Raises the event associated with key with the event data of
        ///  e and a sender of this control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaiseDragEvent(object key, DragEventArgs e)
        {
            ((DragEventHandler)Events[key])?.Invoke(this, e);
        }

        /// <summary>
        ///  Raises the event associated with key with the event data of
        ///  e and a sender of this control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RaisePaintEvent(object key, PaintEventArgs e)
        {
            ((PaintEventHandler)Events[s_paintEvent])?.Invoke(this, e);
        }

        private void RemovePendingMessages(int msgMin, int msgMax)
        {
            if (!IsDisposed)
            {
                NativeMethods.MSG msg = new NativeMethods.MSG();
                IntPtr hwnd = Handle;
                while (UnsafeNativeMethods.PeekMessage(ref msg, new HandleRef(this, hwnd),
                                                       msgMin, msgMax,
                                                       NativeMethods.PM_REMOVE))
                {
                    ; // NULL loop
                }
            }
        }

        /// <summary>
        ///  Resets the back color to be based on the parent's back color.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetBackColor()
        {
            BackColor = Color.Empty;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetCursor()
        {
            Cursor = null;
        }

        private void ResetEnabled()
        {
            Enabled = true;
        }

        /// <summary>
        ///  Resets the font to be based on the parent's font.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetFont()
        {
            Font = null;
        }

        /// <summary>
        ///  Resets the fore color to be based on the parent's fore color.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetForeColor()
        {
            ForeColor = Color.Empty;
        }

        private void ResetLocation()
        {
            Location = new Point(0, 0);
        }

        private void ResetMargin()
        {
            Margin = DefaultMargin;
        }

        private void ResetMinimumSize()
        {
            MinimumSize = DefaultMinimumSize;
        }

        private void ResetPadding()
        {
            CommonProperties.ResetPadding(this);
        }

        private void ResetSize()
        {
            Size = DefaultSize;
        }

        /// <summary>
        ///  Resets the RightToLeft to be the default.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual void ResetRightToLeft()
        {
            RightToLeft = RightToLeft.Inherit;
        }

        /// <summary>
        ///  Forces the recreation of the handle for this control. Inheriting controls
        ///  must call base.RecreateHandle.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void RecreateHandle()
        {
            RecreateHandleCore();
        }

        internal virtual void RecreateHandleCore()
        {
            //
            lock (this)
            {
                if (IsHandleCreated)
                {

                    bool focused = ContainsFocus;

#if DEBUG
                    if (CoreSwitches.PerfTrack.Enabled)
                    {
                        Debug.Write("RecreateHandle: ");
                        Debug.Write(GetType().FullName);
                        Debug.Write(" [Text=");
                        Debug.Write(Text);
                        Debug.Write("]");
                        Debug.WriteLine("");
                    }
#endif
                    bool created = (_state & STATE_CREATED) != 0;
                    if (GetState(STATE_TRACKINGMOUSEEVENT))
                    {
                        SetState(STATE_MOUSEENTERPENDING, true);
                        UnhookMouseEvent();
                    }

                    HandleRef parentHandle = new HandleRef(this, UnsafeNativeMethods.GetParent(new HandleRef(this, Handle)));

                    try
                    {
                        Control[] controlSnapshot = null;
                        _state |= STATE_RECREATE;

                        try
                        {
                            // Inform child controls that their parent is recreating handle.

                            // The default behavior is to now SetParent to parking window, then
                            // SetParent back after the parent's handle has been recreated.
                            // This behavior can be overridden in OnParentHandleRecreat* and is in ListView.

                            //fish out control collection w/o demand creating one.
                            ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
                            if (controlsCollection != null && controlsCollection.Count > 0)
                            {
                                controlSnapshot = new Control[controlsCollection.Count];
                                for (int i = 0; i < controlsCollection.Count; i++)
                                {
                                    Control childControl = controlsCollection[i];
                                    if (childControl != null && childControl.IsHandleCreated)
                                    {
                                        // SetParent to parking window
                                        childControl.OnParentHandleRecreating();

                                        // if we were successful, remember this control
                                        // so we can raise OnParentHandleRecreated
                                        controlSnapshot[i] = childControl;
                                    }
                                    else
                                    {
                                        // put in a null slot which we'll skip over later.
                                        controlSnapshot[i] = null;
                                    }
                                }
                            }

                            // do the main work of recreating the handle
                            DestroyHandle();
                            CreateHandle();
                        }
                        finally
                        {
                            _state &= ~STATE_RECREATE;

                            // inform children that their parent's handle has recreated
                            if (controlSnapshot != null)
                            {
                                for (int i = 0; i < controlSnapshot.Length; i++)
                                {
                                    Control childControl = controlSnapshot[i];
                                    if (childControl != null && childControl.IsHandleCreated)
                                    {
                                        // SetParent back to the new Parent handle
                                        childControl.OnParentHandleRecreated();
                                    }
                                }
                            }
                        }
                        if (created)
                        {
                            CreateControl();
                        }
                    }
                    finally
                    {
                        if (parentHandle.Handle != IntPtr.Zero                               // the parent was not null
                            && (FromHandle(parentHandle.Handle) == null || _parent == null) // but wasnt a windows forms window
                            && UnsafeNativeMethods.IsWindow(parentHandle))
                        {                 // and still is a window
                            // correctly parent back up to where we were before.
                            // if we were parented to a proper windows forms control, CreateControl would have properly parented
                            // us back.
                            UnsafeNativeMethods.SetParent(new HandleRef(this, Handle), parentHandle);
                        }
                    }

                    // Restore control focus
                    if (focused)
                    {
                        Focus();
                    }
                }
            }
        }

        /// <summary>
        ///  Computes the location of the screen rectangle r in client coords.
        /// </summary>
        public Rectangle RectangleToClient(Rectangle r)
        {
            RECT rect = r;
            UnsafeNativeMethods.MapWindowPoints(NativeMethods.NullHandleRef, new HandleRef(this, Handle), ref rect, 2);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  Computes the location of the client rectangle r in screen coords.
        /// </summary>
        public Rectangle RectangleToScreen(Rectangle r)
        {
            RECT rect = r;
            UnsafeNativeMethods.MapWindowPoints(new HandleRef(this, Handle), NativeMethods.NullHandleRef, ref rect, 2);
            return Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom);
        }

        /// <summary>
        ///  Reflects the specified message to the control that is bound to hWnd.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected static bool ReflectMessage(IntPtr hWnd, ref Message m)
        {
            Control control = FromHandle(hWnd);
            if (control == null)
            {
                return false;
            }

            m.Result = control.SendMessage(WindowMessages.WM_REFLECT + m.Msg, m.WParam, m.LParam);
            return true;
        }

        /// <summary>
        ///  Forces the control to invalidate and immediately
        ///  repaint itself and any children.
        /// </summary>
        public virtual void Refresh()
        {
            Invalidate(true);
            Update();
        }

        /// <summary>
        ///  /Releases UI Automation provinder for specified window.
        /// </summary>
        /// <param name="handle">The window handle.</param>
        internal virtual void ReleaseUiaProvider(IntPtr handle)
        {
            // When a window that previously returned providers has been destroyed,
            // you should notify UI Automation by calling the UiaReturnRawElementProvider
            // as follows: UiaReturnRawElementProvider(hwnd, 0, 0, NULL). This call tells
            // UI Automation that it can safely remove all map entries that refer to the specified window.
            UnsafeNativeMethods.UiaReturnRawElementProvider(new HandleRef(this, handle), new IntPtr(0), new IntPtr(0), null);
        }

        /// <summary>
        ///  Resets the mouse leave listeners.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void ResetMouseEventArgs()
        {
            if (GetState(STATE_TRACKINGMOUSEEVENT))
            {
                UnhookMouseEvent();
                HookMouseEvent();
            }
        }

        /// <summary>
        ///  Resets the text to it's default value.
        /// </summary>
        public virtual void ResetText()
        {
            Text = string.Empty;
        }

        private void ResetVisible()
        {
            Visible = true;
        }

        /// <summary>
        ///  Resumes normal layout logic. This will force a layout immediately
        ///  if there are any pending layout requests.
        /// </summary>
        public void ResumeLayout()
        {
            ResumeLayout(true);
        }

        /// <summary>
        ///  Resumes normal layout logic. If performLayout is set to true then
        ///  this will force a layout immediately if there are any pending layout requests.
        /// </summary>
        public void ResumeLayout(bool performLayout)
        {
#if DEBUG
            if (CompModSwitches.LayoutSuspendResume.TraceInfo)
            {
                Debug.WriteLine(GetType().Name + "::ResumeLayout( preformLayout = " + performLayout + ", newCount = " + Math.Max(0, _layoutSuspendCount - 1) + ")");
            }
#endif
            Debug.Assert(_layoutSuspendCount > 0, "Unbalanance suspend/resume layout.");

            bool performedLayout = false;

            if (_layoutSuspendCount > 0)
            {
                if (_layoutSuspendCount == 1)
                {
                    _layoutSuspendCount++;
                    try
                    {
                        OnLayoutResuming(performLayout);
                    }
                    finally
                    {
                        _layoutSuspendCount--;
                    }
                }

                _layoutSuspendCount--;
                if (_layoutSuspendCount == 0
                    && GetState(STATE_LAYOUTDEFERRED)
                    && performLayout)
                {
                    PerformLayout();
                    performedLayout = true;
                }
            }

            if (!performedLayout)
            {
                SetState2(STATE2_CLEARLAYOUTARGS, true);
            }

            /*

            We've had this since Everett,but it seems wrong, redundant and a performance hit.  The
            correct layout calls are already made when bounds or parenting changes, which is all
            we care about. We may want to call this at layout suspend count == 0, but certainaly
            not for all resumes.  I  tried removing it, and doing it only when suspendCount == 0,
            but we break things at every step.

            */
            if (!performLayout)
            {

                CommonProperties.xClearPreferredSizeCache(this);
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                // PERFNOTE: This is more efficient than using Foreach.  Foreach
                // forces the creation of an array subset enum each time we
                // enumerate
                if (controlsCollection != null)
                {
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        LayoutEngine.InitLayout(controlsCollection[i], BoundsSpecified.All);
                        CommonProperties.xClearPreferredSizeCache(controlsCollection[i]);
                    }
                }
            }

        }

        /// <summary>
        ///  Used to actually register the control as a drop target.
        /// </summary>
        internal void SetAcceptDrops(bool accept)
        {
            if (accept != GetState(STATE_DROPTARGET) && IsHandleCreated)
            {
                try
                {
                    if (Application.OleRequired() != ApartmentState.STA)
                    {
                        throw new ThreadStateException(SR.ThreadMustBeSTA);
                    }
                    if (accept)
                    {
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "Registering as drop target: " + Handle.ToString());

                        // Register
                        HRESULT n = Ole32.RegisterDragDrop(new HandleRef(this, Handle), (Ole32.IDropTarget)new DropTarget(this));
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "   ret:" + n.ToString(CultureInfo.CurrentCulture));
                        if (n != HRESULT.S_OK && n != HRESULT.DRAGDROP_E_ALREADYREGISTERED)
                        {
                            throw Marshal.GetExceptionForHR((int)n);
                        }
                    }
                    else
                    {
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "Revoking drop target: " + Handle.ToString());

                        // Revoke
                        HRESULT n = Ole32.RevokeDragDrop(new HandleRef(this, Handle));
                        Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "   ret:" + n.ToString(CultureInfo.InvariantCulture));
                        if (n != HRESULT.S_OK && n != HRESULT.DRAGDROP_E_NOTREGISTERED)
                        {
                            throw Marshal.GetExceptionForHR((int)n);
                        }
                    }

                    SetState(STATE_DROPTARGET, accept);
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(SR.DragDropRegFailed, e);
                }
            }
        }

        /// <summary>
        ///  Scales to entire control and any child controls.
        /// </summary>
        [Obsolete("This method has been deprecated. Use the Scale(SizeF ratio) method instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Scale(float ratio)
        {
            ScaleCore(ratio, ratio);
        }

        /// <summary>
        ///  Scales the entire control and any child controls.
        /// </summary>
        [Obsolete("This method has been deprecated. Use the Scale(SizeF ratio) method instead. http://go.microsoft.com/fwlink/?linkid=14202")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void Scale(float dx, float dy)
        {
#if DEBUG
            int dbgLayoutCheck = LayoutSuspendCount;
#endif
            SuspendLayout();
            try
            {
                ScaleCore(dx, dy);
            }
            finally
            {
                ResumeLayout();
#if DEBUG
                AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
            }
        }

        /// <summary>
        ///  Scales a control and its children given a scaling factor.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public void Scale(SizeF factor)
        {
            // manually call ScaleControl recursively instead of the internal scale method
            // when someone calls this method, they really do want to do some sort of
            // zooming feature, as opposed to AutoScale.
            using (new LayoutTransaction(this, this, PropertyNames.Bounds, false))
            {
                ScaleControl(factor, factor, this);
                if (ScaleChildren)
                {
                    ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
                    if (controlsCollection != null)
                    {
                        // PERFNOTE: This is more efficient than using Foreach.  Foreach
                        // forces the creation of an array subset enum each time we
                        // enumerate
                        for (int i = 0; i < controlsCollection.Count; i++)
                        {
                            Control c = controlsCollection[i];
                            c.Scale(factor);
                        }
                    }
                }
            }

            LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);

        }

        /// <summary>
        ///  Scales a control and its children given a pair of scaling factors.
        ///  IncludedFactor will be applied to the dimensions of controls based on
        ///  their RequiredScaling property.  For example, if a control's
        ///  RequiredScaling property returns Width, the width of the control will
        ///  be scaled according to the includedFactor value.
        ///
        ///  The excludedFactor parameter is used to scale those control bounds who
        ///  are not included in RequiredScaling.
        ///
        ///  If a factor is empty, it indicates that no scaling of those control
        ///  dimensions should be done.
        ///
        ///  The requestingControl property indicates which control has requested
        ///  the scaling function.
        /// </summary>
        internal virtual void Scale(SizeF includedFactor, SizeF excludedFactor, Control requestingControl)
        {
            // When we scale, we are establishing new baselines for the
            // positions of all controls.  Therefore, we should resume(false).
            using (new LayoutTransaction(this, this, PropertyNames.Bounds, false))
            {
                ScaleControl(includedFactor, excludedFactor, requestingControl);
                ScaleChildControls(includedFactor, excludedFactor, requestingControl);
            }
            LayoutTransaction.DoLayout(this, this, PropertyNames.Bounds);
        }

        /// <summary>
        ///  Scales the children of this control.  The default implementation recursively
        ///  walks children and calls ScaleControl on each one.
        ///  IncludedFactor will be applied to the dimensions of controls based on
        ///  their RequiredScaling property.  For example, if a control's
        ///  RequiredScaling property returns Width, the width of the control will
        ///  be scaled according to the includedFactor value.
        ///
        ///  The excludedFactor parameter is used to scale those control bounds who
        ///  are not included in RequiredScaling.
        ///
        ///  If a factor is empty, it indicates that no scaling of those control
        ///  dimensions should be done.
        ///
        ///  The requestingControl property indicates which control has requested
        ///  the scaling function.
        ///
        ///  The updateWindowFontIfNeeded parameter indicates if we need to update Window
        ///  font for controls that need it, i.e. controls using default or inherited font,
        ///  that are also not user-painted.
        /// </summary>
        internal void ScaleChildControls(SizeF includedFactor, SizeF excludedFactor, Control requestingControl, bool updateWindowFontIfNeeded = false)
        {
            if (ScaleChildren)
            {
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                if (controlsCollection != null)
                {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
                        Control c = controlsCollection[i];

                        // Update window font before scaling, as controls often use font metrics during scaling.
                        if (updateWindowFontIfNeeded)
                        {
                            c.UpdateWindowFontIfNeeded();
                        }

                        c.Scale(includedFactor, excludedFactor, requestingControl);
                    }
                }
            }
        }

        /// <summary>
        ///  Calls SetWindowFont if DpiHelper.IsPerMonitorV2Awareness is true,
        ///  control uses default or inherited font and is not user-painted.
        /// </summary>
        internal void UpdateWindowFontIfNeeded()
        {
            if (DpiHelper.IsScalingRequirementMet && !GetStyle(ControlStyles.UserPaint) && (Properties.GetObject(s_fontProperty) == null))
            {
                SetWindowFont();
            }
        }

        /// <summary>
        ///  Scales the children of this control.  The default implementation walks the controls
        ///  collection for the control and calls Scale on each control.
        ///  IncludedFactor will be applied to the dimensions of controls based on
        ///  their RequiredScaling property.  For example, if a control's
        ///  RequiredScaling property returns Width, the width of the control will
        ///  be scaled according to the includedFactor value.
        ///
        ///  The excludedFactor parameter is used to scale those control bounds who
        ///  are not included in RequiredScaling.
        ///
        ///  If a factor is empty, it indicates that no scaling of those control
        ///  dimensions should be done.
        ///
        ///  The requestingControl property indicates which control has requested
        ///  the scaling function.
        /// </summary>
        internal void ScaleControl(SizeF includedFactor, SizeF excludedFactor, Control requestingControl)
        {
            try
            {
                IsCurrentlyBeingScaled = true;

                BoundsSpecified includedSpecified = BoundsSpecified.None;
                BoundsSpecified excludedSpecified = BoundsSpecified.None;

                if (!includedFactor.IsEmpty)
                {
                    includedSpecified = RequiredScaling;
                }

                if (!excludedFactor.IsEmpty)
                {
                    excludedSpecified |= (~RequiredScaling & BoundsSpecified.All);
                }
#if DEBUG
                if (CompModSwitches.RichLayout.TraceInfo)
                {
                    Debug.WriteLine(string.Format(CultureInfo.InvariantCulture, "Scaling {0} Included: {1}, Excluded: {2}",
                                      this, includedFactor, excludedFactor));
                }
#endif

                if (includedSpecified != BoundsSpecified.None)
                {
                    ScaleControl(includedFactor, includedSpecified);
                }

                if (excludedSpecified != BoundsSpecified.None)
                {
                    ScaleControl(excludedFactor, excludedSpecified);
                }

                if (!includedFactor.IsEmpty)
                {
                    RequiredScaling = BoundsSpecified.None;
                }
            }
            finally
            {
                IsCurrentlyBeingScaled = false;
            }
        }

        /// <summary>
        ///  Scales an individual control's location, size, padding and margin.
        ///  If the control is top level, this will not scale the control's location.
        ///  This does not scale children or the size of auto sized controls.  You can
        ///  omit scaling in any direction by changing BoundsSpecified.
        ///
        ///  After the control is scaled the RequiredScaling property is set to
        ///  BoundsSpecified.None.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void ScaleControl(SizeF factor, BoundsSpecified specified)
        {
            CreateParams cp = CreateParams;
            RECT adornments = new RECT(0, 0, 0, 0);
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
            if (dx == 1.0F)
            {
                specified &= ~(BoundsSpecified.X | BoundsSpecified.Width);
            }

            if (dy == 1.0F)
            {
                specified &= ~(BoundsSpecified.Y | BoundsSpecified.Height);
            }

            if (dx != 1.0F)
            {
                padding.Left = (int)Math.Round(padding.Left * dx);
                padding.Right = (int)Math.Round(padding.Right * dx);
                margins.Left = (int)Math.Round(margins.Left * dx);
                margins.Right = (int)Math.Round(margins.Right * dx);
            }

            if (dy != 1.0F)
            {
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
            if (!minSize.IsEmpty)
            {
                minSize -= adornmentSize;
                minSize = ScaleSize(LayoutUtils.UnionSizes(Size.Empty, minSize), // make sure we dont go below 0.
                                        factor.Width,
                                        factor.Height) + adornmentSize;
            }
            if (!maxSize.IsEmpty)
            {
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

            if (DpiHelper.IsScalingRequirementMet && (ParentInternal != null) && (ParentInternal.LayoutEngine == DefaultLayout.Instance))
            {
                // We need to scale AnchorInfo to update distances to container edges
                DefaultLayout.ScaleAnchorInfo((IArrangedElement)this, factor);
            }

            // Set in the scaled bounds as constrained by the newly scaled min/max size.
            SetBoundsCore(rawScaledBounds.X, rawScaledBounds.Y, scaledSize.Width, scaledSize.Height, BoundsSpecified.All);

            MaximumSize = maxSize;
            MinimumSize = minSize;
        }

        /// <summary>
        ///  Performs the work of scaling the entire control and any child controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected virtual void ScaleCore(float dx, float dy)
        {
            Debug.WriteLineIf(CompModSwitches.RichLayout.TraceInfo, GetType().Name + "::ScaleCore(" + dx + ", " + dy + ")");
#if DEBUG
            int dbgLayoutCheck = LayoutSuspendCount;
#endif
            SuspendLayout();
            try
            {
                int sx = (int)Math.Round(_x * dx);
                int sy = (int)Math.Round(_y * dy);

                int sw = _width;
                if ((_controlStyle & ControlStyles.FixedWidth) != ControlStyles.FixedWidth)
                {
                    sw = (int)(Math.Round((_x + _width) * dx)) - sx;
                }
                int sh = _height;
                if ((_controlStyle & ControlStyles.FixedHeight) != ControlStyles.FixedHeight)
                {
                    sh = (int)(Math.Round((_y + _height) * dy)) - sy;
                }

                SetBounds(sx, sy, sw, sh, BoundsSpecified.All);

                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);

                if (controlsCollection != null)
                {
                    // PERFNOTE: This is more efficient than using Foreach.  Foreach
                    // forces the creation of an array subset enum each time we
                    // enumerate
                    for (int i = 0; i < controlsCollection.Count; i++)
                    {
#pragma warning disable 618
                        controlsCollection[i].Scale(dx, dy);
#pragma warning restore 618
                    }
                }
            }
            finally
            {
                ResumeLayout();
#if DEBUG
                AssertLayoutSuspendCount(dbgLayoutCheck);
#endif
            }
        }

        /// <summary>
        ///  Scales a given size with the provided values.
        /// </summary>
        internal Size ScaleSize(Size startSize, float x, float y)
        {
            Size size = startSize;
            if (!GetStyle(ControlStyles.FixedWidth))
            {
                size.Width = (int)Math.Round((float)size.Width * x);
            }
            if (!GetStyle(ControlStyles.FixedHeight))
            {
                size.Height = (int)Math.Round((float)size.Height * y);
            }
            return size;
        }

        /// <summary>
        ///  Activates this control.
        /// </summary>
        public void Select()
        {
            Select(false, false);
        }

        // used by Form
        protected virtual void Select(bool directed, bool forward)
        {
            IContainerControl c = GetContainerControl();

            if (c != null)
            {
                c.ActiveControl = this;
            }
        }

        /// <summary>
        ///  Selects the next control following ctl.
        /// </summary>
        public bool SelectNextControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            Control nextSelectableControl = GetNextSelectableControl(ctl, forward, tabStopOnly, nested, wrap);
            if (nextSelectableControl != null)
            {
                nextSelectableControl.Select(true, forward);
                return true;
            }
            else
            {
                return false;
            }
        }

        private Control GetNextSelectableControl(Control ctl, bool forward, bool tabStopOnly, bool nested, bool wrap)
        {
            if (!Contains(ctl) || !nested && ctl._parent != this)
            {
                ctl = null;
            }

            bool alreadyWrapped = false;
            Control start = ctl;
            do
            {
                ctl = GetNextControl(ctl, forward);
                if (ctl == null)
                {
                    if (!wrap)
                    {
                        break;
                    }

                    if (alreadyWrapped)
                    {
                        return null; //prevent infinite wrapping.
                    }
                    alreadyWrapped = true;
                }
                else
                {
                    if (ctl.CanSelect
                        && (!tabStopOnly || ctl.TabStop)
                        && (nested || ctl._parent == this))
                    {

                        if (ctl._parent is ToolStrip)
                        {
                            continue;
                        }
                        return ctl;
                    }
                }
            } while (ctl != start);
            return null;
        }

        /// <summary>
        ///  This is called recursively when visibility is changed for a control, this
        ///  forces focus to be moved to a visible control.
        /// </summary>
        private void SelectNextIfFocused()
        {
            //           We want to move focus away from hidden controls, so this
            //           function was added.
            //
            if (ContainsFocus && ParentInternal != null)
            {
                IContainerControl c = ParentInternal.GetContainerControl();

                if (c != null)
                {
                    ((Control)c).SelectNextControl(this, true, true, true, true);
                }
            }
        }

        /// <summary>
        ///  Sends a Win32 message to this control.  If the control does not yet
        ///  have a handle, it will be created.
        /// </summary>
        internal IntPtr SendMessage(int msg, int wparam, int lparam)
        {
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        /// <summary>
        ///  Sends a Win32 message to this control.  If the control does not yet
        ///  have a handle, it will be created.
        /// </summary>
        internal IntPtr SendMessage(int msg, ref int wparam, ref int lparam)
        {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, ref wparam, ref lparam);
        }

        internal IntPtr SendMessage(int msg, int wparam, IntPtr lparam)
        {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, (IntPtr)wparam, lparam);
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, IntPtr lparam)
        {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        internal IntPtr SendMessage(int msg, IntPtr wparam, int lparam)
        {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, (IntPtr)lparam);
        }

        /// <summary>
        ///  Sends a Win32 message to this control.  If the control does not yet
        ///  have a handle, it will be created.
        /// </summary>
        internal IntPtr SendMessage(int msg, int wparam, ref RECT lparam)
        {
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, ref lparam);
        }

        /// <summary>
        ///  Sends a Win32 message to this control.  If the control does not yet
        ///  have a handle, it will be created.
        /// </summary>
        internal IntPtr SendMessage(int msg, bool wparam, int lparam)
        {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        /// <summary>
        ///  Sends a Win32 message to this control.  If the control does not yet
        ///  have a handle, it will be created.
        /// </summary>
        internal IntPtr SendMessage(int msg, int wparam, string lparam)
        {
            Debug.Assert(IsHandleCreated, "Performance alert!  Calling Control::SendMessage and forcing handle creation.  Re-work control so handle creation is not required to set properties.  If there is no work around, wrap the call in an IsHandleCreated check.");
            return UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), msg, wparam, lparam);
        }

        /// <summary>
        ///  sends this control to the back of the z-order
        /// </summary>
        public void SendToBack()
        {
            if (_parent != null)
            {
                _parent.Controls.SetChildIndex(this, -1);
            }
            else if (IsHandleCreated && GetTopLevel())
            {
                SafeNativeMethods.SetWindowPos(
                    new HandleRef(_window, Handle),
                    NativeMethods.HWND_BOTTOM,
                    flags: NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
            }
        }

        /// <summary>
        ///  Sets the bounds of the control.
        /// </summary>
        public void SetBounds(int x, int y, int width, int height)
        {
            if (_x != x || _y != y || _width != width ||
                _height != height)
            {
                SetBoundsCore(x, y, width, height, BoundsSpecified.All);

                // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
                // have refreshed the interior layout.  We only need to layout the parent.
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
            }
            else
            {
                // Still need to init scaling.
                InitScaling(BoundsSpecified.All);
            }
        }

        /// <summary>
        ///  Sets the bounds of the control.
        /// </summary>
        public void SetBounds(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if ((specified & BoundsSpecified.X) == BoundsSpecified.None)
            {
                x = _x;
            }

            if ((specified & BoundsSpecified.Y) == BoundsSpecified.None)
            {
                y = _y;
            }

            if ((specified & BoundsSpecified.Width) == BoundsSpecified.None)
            {
                width = _width;
            }

            if ((specified & BoundsSpecified.Height) == BoundsSpecified.None)
            {
                height = _height;
            }

            if (_x != x || _y != y || _width != width ||
                _height != height)
            {
                SetBoundsCore(x, y, width, height, specified);

                // WM_WINDOWPOSCHANGED will trickle down to an OnResize() which will
                // have refreshed the interior layout or the resized control.  We only need to layout
                // the parent.  This happens after InitLayout has been invoked.
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);
            }
            else
            {
                // Still need to init scaling.
                InitScaling(specified);
            }
        }

        /// <summary>
        ///  Performs the work of setting the bounds of this control. Inheriting
        ///  classes can overide this function to add size restrictions. Inheriting
        ///  classes must call base.setBoundsCore to actually cause the bounds
        ///  of the control to change.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
#if DEBUG
            if (CompModSwitches.SetBounds.TraceInfo)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0}::SetBoundsCore(x={1} y={2} width={3} height={4} specified={5}", Name, x, y, width, height, specified));
            }
#endif
            // SetWindowPos below sends a WmWindowPositionChanged (not posts) so we immediately
            // end up in WmWindowPositionChanged which may cause the parent to layout.  We need to
            // suspend/resume to defer the parent from laying out until after InitLayout has been called
            // to update the layout engine's state with the new control bounds.
#if DEBUG
            int suspendCount = -44371;      // Arbitrary negative prime to surface bugs.
#endif
            if (ParentInternal != null)
            {
#if DEBUG
                suspendCount = ParentInternal.LayoutSuspendCount;
#endif
                ParentInternal.SuspendLayout();
            }
            try
            {
                if (_x != x || _y != y || _width != width || _height != height)
                {
                    CommonProperties.UpdateSpecifiedBounds(this, x, y, width, height, specified);

                    // Provide control with an opportunity to apply self imposed constraints on its size.
                    Rectangle adjustedBounds = ApplyBoundsConstraints(x, y, width, height);
                    width = adjustedBounds.Width;
                    height = adjustedBounds.Height;
                    x = adjustedBounds.X;
                    y = adjustedBounds.Y;

                    if (!IsHandleCreated)
                    {
                        // Handle is not created, just record our new position and we're done.
                        UpdateBounds(x, y, width, height);
                    }
                    else
                    {
                        if (!GetState(STATE_SIZELOCKEDBYOS))
                        {
                            int flags = NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE;

                            if (_x == x && _y == y)
                            {
                                flags |= NativeMethods.SWP_NOMOVE;
                            }
                            if (_width == width && _height == height)
                            {
                                flags |= NativeMethods.SWP_NOSIZE;
                            }

                            //
                            // Give a chance for derived controls to do what they want, just before we resize.
                            OnBoundsUpdate(x, y, width, height);

                            SafeNativeMethods.SetWindowPos(
                                new HandleRef(_window, Handle),
                                NativeMethods.NullHandleRef,
                                x,
                                y,
                                width,
                                height,
                                flags);

                            // NOTE: SetWindowPos causes a WM_WINDOWPOSCHANGED which is processed
                            // synchonously so we effectively end up in UpdateBounds immediately following
                            // SetWindowPos.
                            //
                            //UpdateBounds(x, y, width, height);
                        }
                    }
                }
            }
            finally
            {

                // Initialize the scaling engine.
                InitScaling(specified);

                if (ParentInternal != null)
                {
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
                    ParentInternal.ResumeLayout( /* performLayout = */ true);
#if DEBUG
                    Debug.Assert(ParentInternal.LayoutSuspendCount == suspendCount, "Suspend/Resume layout mismatch!");
#endif
                }
            }
        }

        /// <summary>
        ///  Performs the work of setting the size of the client area of the control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void SetClientSizeCore(int x, int y)
        {
            Size = SizeFromClientSize(x, y);
            _clientWidth = x;
            _clientHeight = y;
            OnClientSizeChanged(EventArgs.Empty);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual Size SizeFromClientSize(Size clientSize)
        {
            return SizeFromClientSize(clientSize.Width, clientSize.Height);
        }

        internal Size SizeFromClientSize(int width, int height)
        {
            RECT rect = new RECT(0, 0, width, height);
            CreateParams cp = CreateParams;
            AdjustWindowRectEx(ref rect, cp.Style, HasMenu, cp.ExStyle);
            return rect.Size;
        }

        private void SetHandle(IntPtr value)
        {
            if (value == IntPtr.Zero)
            {
                SetState(STATE_CREATED, false);
            }
            UpdateRoot();
        }

        private void SetParentHandle(IntPtr value)
        {
            Debug.Assert(value != NativeMethods.InvalidIntPtr, "Outdated call to SetParentHandle");

            if (IsHandleCreated)
            {
                IntPtr parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(_window, Handle));
                bool topLevel = GetTopLevel();
                if (parentHandle != value || (parentHandle == IntPtr.Zero && !topLevel))
                {
                    Debug.Assert(Handle != value, "Cycle created in SetParentHandle");

                    bool recreate = (parentHandle == IntPtr.Zero && !topLevel)
                        || (value == IntPtr.Zero && topLevel);

                    if (recreate)
                    {
                        // We will recreate later, when the MdiChild's visibility
                        // is set to true (see
                        if (this is Form f)
                        {
                            if (!f.CanRecreateHandle())
                            {
                                recreate = false;
                                // we don't want to recreate - but our styles may have changed.
                                // before we unpark the window below we need to update
                                UpdateStyles();
                            }
                        }
                    }

                    if (recreate)
                    {
                        RecreateHandle();
                    }
                    if (!GetTopLevel())
                    {
                        if (value == IntPtr.Zero)
                        {
                            Application.ParkHandle(new HandleRef(_window, Handle));
                            UpdateRoot();
                        }
                        else
                        {
                            UnsafeNativeMethods.SetParent(new HandleRef(_window, Handle), new HandleRef(null, value));
                            if (_parent != null)
                            {
                                _parent.UpdateChildZOrder(this);
                            }
                            Application.UnparkHandle(new HandleRef(_window, Handle), _window.DpiAwarenessContext);
                        }
                    }
                }
                else if (value == IntPtr.Zero && parentHandle == IntPtr.Zero && topLevel)
                {
                    // The handle was previously parented to the parking window. Its TopLevel property was
                    // then changed to true so the above call to GetParent returns null even though the parent of the control is
                    // not null. We need to explicitly set the parent to null.
                    UnsafeNativeMethods.SetParent(new HandleRef(_window, Handle), new HandleRef(null, IntPtr.Zero));
                    Application.UnparkHandle(new HandleRef(_window, Handle), _window.DpiAwarenessContext);
                }
            }
        }

        // Form, UserControl, AxHost usage
        internal void SetState(int flag, bool value)
        {
            _state = value ? _state | flag : _state & ~flag;
        }

        // Application, SKWindow usage
        internal void SetState2(int flag, bool value)
        {
            _state2 = value ? _state2 | flag : _state2 & ~flag;
        }

        /// <summary>
        ///  Sets the current value of the specified bit in the control's style.
        ///  NOTE: This is control style, not the Win32 style of the hWnd.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void SetStyle(ControlStyles flag, bool value)
        {
            // WARNING: if we ever add argument checking to "flag", we will need
            // to move private styles like Layered to State.
            _controlStyle = value ? _controlStyle | flag : _controlStyle & ~flag;
        }

        internal static IntPtr SetUpPalette(IntPtr dc, bool force, bool realizePalette)
        {
            Debug.WriteLineIf(s_paletteTracing.TraceVerbose, "SetUpPalette(force:=" + force + ", ralizePalette:=" + realizePalette + ")");

            IntPtr halftonePalette = Graphics.GetHalftonePalette();

            Debug.WriteLineIf(s_paletteTracing.TraceVerbose, "select palette " + !force);
            IntPtr result = SafeNativeMethods.SelectPalette(new HandleRef(null, dc), new HandleRef(null, halftonePalette), (force ? 0 : 1));

            if (result != IntPtr.Zero && realizePalette)
            {
                SafeNativeMethods.RealizePalette(new HandleRef(null, dc));
            }

            return result;
        }

        protected void SetTopLevel(bool value)
        {
            if (value && IsActiveX)
            {
                throw new InvalidOperationException(SR.TopLevelNotAllowedIfActiveX);
            }
            else
            {
                SetTopLevelInternal(value);
            }
        }

        private protected void SetTopLevelInternal(bool value)
        {
            if (GetTopLevel() != value)
            {
                if (_parent != null)
                {
                    throw new ArgumentException(SR.TopLevelParentedControl, "value");
                }
                SetState(STATE_TOPLEVEL, value);
                // make sure the handle is created before hooking, otherwise a toplevel control that never
                // creates its handle will leak.
                if (IsHandleCreated && GetState2(STATE2_INTERESTEDINUSERPREFERENCECHANGED))
                {
                    ListenToUserPreferenceChanged(value);
                }
                UpdateStyles();
                SetParentHandle(IntPtr.Zero);
                if (value && Visible)
                {
                    CreateControl();
                }
                UpdateRoot();
            }
        }

        protected virtual void SetVisibleCore(bool value)
        {
            if (GetVisibleCore() != value)
            {
                if (!value)
                {
                    SelectNextIfFocused();
                }

                bool fireChange = false;

                if (GetTopLevel())
                {
                    // The processing of WmShowWindow will set the visibility
                    // bit and call CreateControl()

                    if (IsHandleCreated || value)
                    {
                        SafeNativeMethods.ShowWindow(new HandleRef(this, Handle), value ? ShowParams : NativeMethods.SW_HIDE);
                    }
                }
                else if (IsHandleCreated || value && _parent != null && _parent.Created)
                {
                    // We want to mark the control as visible so that CreateControl
                    // knows that we are going to be displayed... however in case
                    // an exception is thrown, we need to back the change out.

                    SetState(STATE_VISIBLE, value);
                    fireChange = true;
                    try
                    {
                        if (value)
                        {
                            CreateControl();
                        }

                        SafeNativeMethods.SetWindowPos(
                            new HandleRef(_window, Handle),
                            NativeMethods.NullHandleRef,
                            flags: NativeMethods.SWP_NOSIZE
                                | NativeMethods.SWP_NOMOVE
                                | NativeMethods.SWP_NOZORDER
                                | NativeMethods.SWP_NOACTIVATE
                                | (value ? NativeMethods.SWP_SHOWWINDOW : NativeMethods.SWP_HIDEWINDOW));
                    }
                    catch
                    {
                        SetState(STATE_VISIBLE, !value);
                        throw;
                    }
                }

                if (GetVisibleCore() != value)
                {
                    SetState(STATE_VISIBLE, value);
                    fireChange = true;
                }

                if (fireChange)
                {
                    // We do not do this in the OnPropertyChanged event for visible
                    // Lots of things could cause us to become visible, including a
                    // parent window.  We do not want to indescriminiately layout
                    // due to this, but we do want to layout if the user changed
                    // our visibility.
                    using (new LayoutTransaction(_parent, this, PropertyNames.Visible))
                    {
                        OnVisibleChanged(EventArgs.Empty);
                    }
                }
                UpdateRoot();
            }
            else
            {
                // value of Visible property not changed, but raw bit may have

                if (!GetState(STATE_VISIBLE) && !value && IsHandleCreated)
                {
                    // PERF - setting Visible=false twice can get us into this else block
                    // which makes us process WM_WINDOWPOS* messages - make sure we've already 
                    // visible=false - if not, make it so.
                    if (!SafeNativeMethods.IsWindowVisible(new HandleRef(this, Handle)))
                    {
                        // we're already invisible - bail.
                        return;
                    }
                }

                SetState(STATE_VISIBLE, value);

                // If the handle is already created, we need to update the window style.
                // This situation occurs when the parent control is not currently visible,
                // but the child control has already been created.
                if (IsHandleCreated)
                {
                    SafeNativeMethods.SetWindowPos(
                        new HandleRef(_window, Handle),
                        NativeMethods.NullHandleRef,
                        flags: NativeMethods.SWP_NOSIZE
                            | NativeMethods.SWP_NOMOVE
                            | NativeMethods.SWP_NOZORDER
                            | NativeMethods.SWP_NOACTIVATE
                            | (value ? NativeMethods.SWP_SHOWWINDOW : NativeMethods.SWP_HIDEWINDOW));
                }
            }
        }

        /// <summary>
        ///  Determine effective auto-validation setting for a given control, based on the AutoValidate property
        ///  of its containing control. Defaults to 'EnablePreventFocusChange' if there is no containing control
        ///  (eg. because this control is a top-level container).
        /// </summary>
        internal static AutoValidate GetAutoValidateForControl(Control control)
        {
            ContainerControl parent = control.ParentContainerControl;
            return (parent != null) ? parent.AutoValidate : AutoValidate.EnablePreventFocusChange;
        }

        /// <summary>
        ///  Is auto-validation currently in effect for this control?
        ///  Depends on the AutoValidate property of the containing control.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldAutoValidate
        {
            get
            {
                return GetAutoValidateForControl(this) != AutoValidate.Disable;
            }
        }

        // This method is called in PerformContainerValidation to check if this control supports containerValidation.
        // TabControl overrides this method to return true.
        internal virtual bool ShouldPerformContainerValidation()
        {
            return GetStyle(ControlStyles.ContainerControl);
        }

        /// <summary>
        ///  Returns true if the backColor should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeBackColor()
        {
            Color backColor = Properties.GetColor(s_backColorProperty);
            return !backColor.IsEmpty;
        }

        /// <summary>
        ///  Returns true if the cursor should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeCursor()
        {
            object cursor = Properties.GetObject(s_cursorProperty, out bool found);
            return (found && cursor != null);
        }

        /// <summary>
        ///  Returns true if the enabled property should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeEnabled()
        {
            return (!GetState(STATE_ENABLED));
        }

        /// <summary>
        ///  Returns true if the foreColor should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeForeColor()
        {
            Color foreColor = Properties.GetColor(s_foreColorProperty);
            return !foreColor.IsEmpty;
        }

        /// <summary>
        ///  Returns true if the font should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeFont()
        {
            object font = Properties.GetObject(s_fontProperty, out bool found);
            return (found && font != null);
        }

        /// <summary>
        ///  Returns true if the RightToLeft should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeRightToLeft()
        {
            int rtl = Properties.GetInteger(s_rightToLeftProperty, out bool found);
            return (found && rtl != (int)RightToLeft.Inherit);
        }

        /// <summary>
        ///  Returns true if the visible property should be persisted in code gen.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        private bool ShouldSerializeVisible()
        {
            return (!GetState(STATE_VISIBLE));
        }

        // Helper function - translates text alignment for Rtl controls
        // Read TextAlign as Left == Near, Right == Far
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected HorizontalAlignment RtlTranslateAlignment(HorizontalAlignment align)
        {
            return RtlTranslateHorizontal(align);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected LeftRightAlignment RtlTranslateAlignment(LeftRightAlignment align)
        {
            return RtlTranslateLeftRight(align);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected ContentAlignment RtlTranslateAlignment(ContentAlignment align)
        {
            return RtlTranslateContent(align);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected HorizontalAlignment RtlTranslateHorizontal(HorizontalAlignment align)
        {
            if (RightToLeft.Yes == RightToLeft)
            {
                if (HorizontalAlignment.Left == align)
                {
                    return HorizontalAlignment.Right;
                }
                else if (HorizontalAlignment.Right == align)
                {
                    return HorizontalAlignment.Left;
                }
            }

            return align;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected LeftRightAlignment RtlTranslateLeftRight(LeftRightAlignment align)
        {
            if (RightToLeft.Yes == RightToLeft)
            {
                if (LeftRightAlignment.Left == align)
                {
                    return LeftRightAlignment.Right;
                }
                else if (LeftRightAlignment.Right == align)
                {
                    return LeftRightAlignment.Left;
                }
            }

            return align;
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal ContentAlignment RtlTranslateContent(ContentAlignment align)
        {
            if (RightToLeft.Yes == RightToLeft)
            {

                if ((align & WindowsFormsUtils.AnyTopAlign) != 0)
                {
                    switch (align)
                    {
                        case ContentAlignment.TopLeft:
                            return ContentAlignment.TopRight;
                        case ContentAlignment.TopRight:
                            return ContentAlignment.TopLeft;
                    }
                }
                if ((align & WindowsFormsUtils.AnyMiddleAlign) != 0)
                {
                    switch (align)
                    {
                        case ContentAlignment.MiddleLeft:
                            return ContentAlignment.MiddleRight;
                        case ContentAlignment.MiddleRight:
                            return ContentAlignment.MiddleLeft;
                    }
                }

                if ((align & WindowsFormsUtils.AnyBottomAlign) != 0)
                {
                    switch (align)
                    {
                        case ContentAlignment.BottomLeft:
                            return ContentAlignment.BottomRight;
                        case ContentAlignment.BottomRight:
                            return ContentAlignment.BottomLeft;
                    }
                }
            }
            return align;
        }

        private void SetWindowFont()
        {
            SendMessage(WindowMessages.WM_SETFONT, FontHandle, 0 /*redraw = false*/);
        }

        private void SetWindowStyle(int flag, bool value)
        {
            int styleFlags = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
            UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)(value ? styleFlags | flag : styleFlags & ~flag)));
        }

        /// <summary>
        ///  Makes the control display by setting the visible property to true
        /// </summary>
        public void Show()
        {
            Visible = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializeMargin()
        {
            return !Margin.Equals(DefaultMargin);
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeMaximumSize()
        {
            return MaximumSize != DefaultMaximumSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeMinimumSize()
        {
            return MinimumSize != DefaultMinimumSize;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        internal bool ShouldSerializePadding()
        {
            return !Padding.Equals(DefaultPadding);
        }

        /// <summary>
        ///  Determines if the <see cref='Size'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeSize()
        {
            // In Whidbey the ControlDesigner class will always serialize size as it replaces the Size
            // property descriptor with its own.  This is here for compat.
            Size s = DefaultSize;
            return _width != s.Width || _height != s.Height;
        }

        /// <summary>
        ///  Determines if the <see cref='Text'/> property needs to be persisted.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        internal virtual bool ShouldSerializeText()
        {
            return Text.Length != 0;
        }

        /// <summary>
        ///  Suspends the layout logic for the control.
        /// </summary>
        public void SuspendLayout()
        {
            _layoutSuspendCount++;
            if (_layoutSuspendCount == 1)
            {
                OnLayoutSuspended();
            }

#if DEBUG
            Debug.Assert(_layoutSuspendCount > 0, "SuspendLayout: layoutSuspendCount overflowed.");
            if (CompModSwitches.LayoutSuspendResume.TraceInfo)
            {
                Debug.WriteLine(GetType().Name + "::SuspendLayout( newCount = " + _layoutSuspendCount + ")");
            }
#endif
        }

        /// <summary>
        ///  Stops listening for the mouse leave event.
        /// </summary>
        private void UnhookMouseEvent()
        {
            SetState(STATE_TRACKINGMOUSEEVENT, false);
        }

        /// <summary>
        ///  Forces the control to paint any currently invalid areas.
        /// </summary>
        public void Update()
        {
            SafeNativeMethods.UpdateWindow(new HandleRef(_window, InternalHandle));
        }

        /// <summary>
        ///  Updates the bounds of the control based on the handle the control is
        ///  bound to.
        /// </summary>
        // Internal for ScrollableControl
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected internal void UpdateBounds()
        {
            RECT rect = new RECT();
            UnsafeNativeMethods.GetClientRect(new HandleRef(_window, InternalHandle), ref rect);
            int clientWidth = rect.right;
            int clientHeight = rect.bottom;
            UnsafeNativeMethods.GetWindowRect(new HandleRef(_window, InternalHandle), ref rect);
            if (!GetTopLevel())
            {
                UnsafeNativeMethods.MapWindowPoints(NativeMethods.NullHandleRef, new HandleRef(null, UnsafeNativeMethods.GetParent(new HandleRef(_window, InternalHandle))), ref rect, 2);
            }

            UpdateBounds(rect.left, rect.top, rect.right - rect.left,
                         rect.bottom - rect.top, clientWidth, clientHeight);
        }

        /// <summary>
        ///  Updates the bounds of the control based on the bounds passed in.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateBounds(int x, int y, int width, int height)
        {
            Debug.Assert(!IsHandleCreated, "Don't call this method when handle is created!!");

            // reverse-engineer the AdjustWindowRectEx call to figure out
            // the appropriate clientWidth and clientHeight
            RECT rect = new RECT();
            rect.left = rect.right = rect.top = rect.bottom = 0;

            CreateParams cp = CreateParams;

            AdjustWindowRectEx(ref rect, cp.Style, false, cp.ExStyle);
            int clientWidth = width - (rect.right - rect.left);
            int clientHeight = height - (rect.bottom - rect.top);
            UpdateBounds(x, y, width, height, clientWidth, clientHeight);
        }

        /// <summary>
        ///  Updates the bounds of the control based on the bounds passed in.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateBounds(int x, int y, int width, int height, int clientWidth, int clientHeight)
        {
#if DEBUG
            if (CompModSwitches.SetBounds.TraceVerbose)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "{0}::UpdateBounds(", Name));
                Debug.Indent();
                Debug.WriteLine(string.Format(
                     CultureInfo.CurrentCulture, "oldBounds={{x={0} y={1} width={2} height={3} clientWidth={4} clientHeight={5}}}",
                     _x, _y, _width, _height, _clientWidth, _clientHeight));
            }
#endif // DEBUG

            bool newLocation = _x != x || _y != y;
            bool newSize = Width != width || Height != height ||
                           _clientWidth != clientWidth || _clientHeight != clientHeight;

            _x = x;
            _y = y;
            _width = width;
            _height = height;
            _clientWidth = clientWidth;
            _clientHeight = clientHeight;

            if (newLocation)
            {
#if DEBUG
                Rectangle originalBounds = Bounds;
#endif
                OnLocationChanged(EventArgs.Empty);
#if DEBUG
                if (Bounds != originalBounds && CompModSwitches.SetBounds.TraceWarning)
                {
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "WARNING: Bounds changed during OnLocationChanged()\r\nbefore={0} after={1}", originalBounds, Bounds));
                }
#endif
            }
            if (newSize)
            {
#if DEBUG
                Rectangle originalBounds = Bounds;
#endif
                OnSizeChanged(EventArgs.Empty);
                OnClientSizeChanged(EventArgs.Empty);

                // Clear PreferredSize cache for this control
                CommonProperties.xClearPreferredSizeCache(this);
                LayoutTransaction.DoLayout(ParentInternal, this, PropertyNames.Bounds);

#if DEBUG
                if (Bounds != originalBounds && CompModSwitches.SetBounds.TraceWarning)
                {
                    Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "WARNING: Bounds changed during OnSizeChanged()\r\nbefore={0} after={1}", originalBounds, Bounds));
                }
#endif
            }

#if DEBUG
            if (CompModSwitches.SetBounds.TraceVerbose)
            {
                Debug.WriteLine(string.Format(CultureInfo.CurrentCulture, "newBounds={{x={0} y={1} width={2} height={3} clientWidth={4} clientHeight={5}}}", x, y, width, height, clientWidth, clientHeight));
                Debug.Unindent();
            }
#endif
        }

        /// <summary>
        ///  Updates the binding manager bindings when the binding proeprty changes.
        ///  We have the code here, rather than in PropertyChagned, so we don't pull
        ///  in the data assembly if it's not used.
        /// </summary>
        private void UpdateBindings()
        {
            for (int i = 0; i < DataBindings.Count; i++)
            {
                BindingContext.UpdateBinding(BindingContext, DataBindings[i]);
            }
        }

        /// <summary>
        ///  Updates the child control's position in the control array to correctly
        ///  reflect its index.
        /// </summary>
        private void UpdateChildControlIndex(Control ctl)
        {
            // Don't reorder the child control array for tab controls. Implemented as a special case
            // in order to keep the method private.
            if (GetType().IsAssignableFrom(typeof(TabControl)))
            {
                return;
            }

            int newIndex = 0;
            int curIndex = Controls.GetChildIndex(ctl);
            IntPtr hWnd = ctl.InternalHandle;
            while ((hWnd = UnsafeNativeMethods.GetWindow(new HandleRef(null, hWnd), NativeMethods.GW_HWNDPREV)) != IntPtr.Zero)
            {
                Control c = FromHandle(hWnd);
                if (c != null)
                {
                    newIndex = Controls.GetChildIndex(c, false) + 1;
                    break;
                }
            }
            if (newIndex > curIndex)
            {
                newIndex--;
            }
            if (newIndex != curIndex)
            {
                Controls.SetChildIndex(ctl, newIndex);
            }
        }

        // whenever we create our handle, we need to get ahold of the HWND of our parent,
        // and track if that thing is destroyed, because any messages sent to our parent (e.g. WM_NOTIFY, WM_DRAWITEM, etc)
        // will continue to be sent to that window even after we do a SetParent call until the handle is recreated.
        // So here we keep track of that, and if that window ever gets destroyed, we'll recreate our handle.
        //
        // Scenario is when you've got a control in one parent, you move it to another, then destroy the first parent.  It'll stop
        // getting any reflected messages because Windows will send them to the original parent.
        private void UpdateReflectParent(bool findNewParent)
        {
            if (!Disposing && findNewParent && IsHandleCreated)
            {
                IntPtr parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(this, Handle));
                if (parentHandle != IntPtr.Zero)
                {
                    ReflectParent = FromHandle(parentHandle);
                    return;
                }
            }
            ReflectParent = null;
        }

        /// <summary>
        ///  Updates this control in it's parent's zorder.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateZOrder()
        {
            if (_parent != null)
            {
                _parent.UpdateChildZOrder(this);
            }
        }

        /// <summary>
        ///  Syncs the ZOrder of child control to the index we want it to be.
        /// </summary>
        private void UpdateChildZOrder(Control ctl)
        {
            if (!IsHandleCreated || !ctl.IsHandleCreated || ctl._parent != this)
            {
                return;
            }

            IntPtr prevHandle = (IntPtr)NativeMethods.HWND_TOP;
            for (int i = Controls.GetChildIndex(ctl); --i >= 0;)
            {
                Control c = Controls[i];
                if (c.IsHandleCreated && c._parent == this)
                {
                    prevHandle = c.Handle;
                    break;
                }
            }
            if (UnsafeNativeMethods.GetWindow(new HandleRef(ctl._window, ctl.Handle), NativeMethods.GW_HWNDPREV) != prevHandle)
            {
                _state |= STATE_NOZORDER;
                try
                {
                    SafeNativeMethods.SetWindowPos(
                        new HandleRef(ctl._window, ctl.Handle),
                        new HandleRef(null, prevHandle),
                        flags: NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE);
                }
                finally
                {
                    _state &= ~STATE_NOZORDER;
                }
            }
        }

        /// <summary>
        ///  Updates the rootReference in the bound window.
        ///  (Used to prevent visible top-level controls from being garbage collected)
        /// </summary>
        private void UpdateRoot()
        {
            _window.LockReference(GetTopLevel() && Visible);
        }

        /// <summary>
        ///  Forces styles to be reapplied to the handle. This function will call
        ///  CreateParams to get the styles to apply.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected void UpdateStyles()
        {
            UpdateStylesCore();

            OnStyleChanged(EventArgs.Empty);
        }

        internal virtual void UpdateStylesCore()
        {
            if (IsHandleCreated)
            {
                CreateParams cp = CreateParams;
                int winStyle = WindowStyle;
                int exStyle = WindowExStyle;

                // resolve the Form's lazy visibility.
                if ((_state & STATE_VISIBLE) != 0)
                {
                    cp.Style |= NativeMethods.WS_VISIBLE;
                }
                if (winStyle != cp.Style)
                {
                    WindowStyle = cp.Style;
                }
                if (exStyle != cp.ExStyle)
                {
                    WindowExStyle = cp.ExStyle;
                    SetState(STATE_MIRRORED, (cp.ExStyle & NativeMethods.WS_EX_LAYOUTRTL) != 0);
                }

                SafeNativeMethods.SetWindowPos(
                    new HandleRef(this, Handle),
                    NativeMethods.NullHandleRef,
                    flags: NativeMethods.SWP_DRAWFRAME
                        | NativeMethods.SWP_NOACTIVATE
                        | NativeMethods.SWP_NOMOVE
                        | NativeMethods.SWP_NOSIZE
                        | NativeMethods.SWP_NOZORDER);

                Invalidate(true);
            }
        }

        private void UserPreferenceChanged(object sender, UserPreferenceChangedEventArgs pref)
        {
            if (pref.Category == UserPreferenceCategory.Color)
            {
                s_defaultFont = null;
                OnSystemColorsChanged(EventArgs.Empty);
            }
        }

        // Give a chance for derived controls to do what they want, just before we resize.
        internal virtual void OnBoundsUpdate(int x, int y, int width, int height)
        {
        }

        // These Window* methods allow us to keep access to the "window"
        // property private, which is important for restricting access to the
        // handle.
        internal void WindowAssignHandle(IntPtr handle, bool value)
        {
            _window.AssignHandle(handle, value);
        }

        internal void WindowReleaseHandle()
        {
            _window.ReleaseHandle();
        }

        private void WmClose(ref Message m)
        {
            if (ParentInternal != null)
            {
                IntPtr parentHandle = Handle;
                IntPtr lastParentHandle = parentHandle;

                while (parentHandle != IntPtr.Zero)
                {
                    lastParentHandle = parentHandle;
                    parentHandle = UnsafeNativeMethods.GetParent(new HandleRef(null, parentHandle));

                    int style = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(null, lastParentHandle), NativeMethods.GWL_STYLE)));

                    if ((style & NativeMethods.WS_CHILD) == 0)
                    {
                        break;
                    }

                }

                if (lastParentHandle != IntPtr.Zero)
                {
                    UnsafeNativeMethods.PostMessage(new HandleRef(null, lastParentHandle), WindowMessages.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }

            DefWndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_CAPTURECHANGED message
        /// </summary>
        private void WmCaptureChanged(ref Message m)
        {
            OnMouseCaptureChanged(EventArgs.Empty);
            DefWndProc(ref m);

        }

        /// <summary>
        ///  Handles the WM_COMMAND message
        /// </summary>
        private void WmCommand(ref Message m)
        {
            if (IntPtr.Zero == m.LParam)
            {
                if (Command.DispatchID(NativeMethods.Util.LOWORD(m.WParam)))
                {
                    return;
                }
            }
            else
            {
                if (ReflectMessage(m.LParam, ref m))
                {
                    return;
                }
            }
            DefWndProc(ref m);
        }

        // overridable so nested controls can provide a different source control.
        internal virtual void WmContextMenu(ref Message m)
        {
            WmContextMenu(ref m, this);
        }

        /// <summary>
        ///  Handles the WM_CONTEXTMENU message
        /// </summary>
        internal void WmContextMenu(ref Message m, Control sourceControl)
        {
            var contextMenuStrip = (ContextMenuStrip)Properties.GetObject(s_contextMenuStripProperty);
            if (contextMenuStrip != null)
            {
                int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                Point client;
                bool keyboardActivated = false;
                // lparam will be exactly -1 when the user invokes the context menu
                // with the keyboard.
                //
                if (unchecked((int)(long)m.LParam) == -1)
                {
                    keyboardActivated = true;
                    client = new Point(Width / 2, Height / 2);
                }
                else
                {
                    client = PointToClient(new Point(x, y));
                }

                if (ClientRectangle.Contains(client))
                {
                    contextMenuStrip.ShowInternal(sourceControl, client, keyboardActivated);
                }
                else
                {
                    DefWndProc(ref m);
                }
            }
            else
            {
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_CTLCOLOR message
        /// </summary>
        private void WmCtlColorControl(ref Message m)
        {
            // We could simply reflect the message, but it's faster to handle it here if possible.
            Control control = FromHandle(m.LParam);
            if (control != null)
            {
                m.Result = control.InitializeDCForWmCtlColor(m.WParam, m.Msg);
                if (m.Result != IntPtr.Zero)
                {
                    return;
                }
            }

            DefWndProc(ref m);
        }

        private void WmDisplayChange(ref Message m)
        {
            BufferedGraphicsManager.Current.Invalidate();
            DefWndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_ERASEBKGND message
        /// </summary>
        private void WmEraseBkgnd(ref Message m)
        {
            if (GetStyle(ControlStyles.UserPaint))
            {
                // When possible, it's best to do all painting directly from WM_PAINT.
                // OptimizedDoubleBuffer is the "same" as turning on AllPaintingInWMPaint
                if (!(GetStyle(ControlStyles.AllPaintingInWmPaint)))
                {
                    IntPtr dc = m.WParam;
                    if (dc == IntPtr.Zero)
                    {    // This happens under extreme stress conditions
                        m.Result = (IntPtr)0;
                        return;
                    }
                    RECT rc = new RECT();
                    UnsafeNativeMethods.GetClientRect(new HandleRef(this, Handle), ref rc);
                    using (PaintEventArgs pevent = new PaintEventArgs(dc, Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom)))
                    {
                        PaintWithErrorHandling(pevent, PaintLayerBackground);
                    }
                }
                m.Result = (IntPtr)1;
            }
            else
            {
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_GETCONTROLNAME message. Returns the name of the control.
        /// </summary>
        private void WmGetControlName(ref Message m)
        {
            string name;

            if (Site != null)
            {
                name = Site.Name;
            }
            else
            {
                name = Name;
            }

            if (name == null)
            {
                name = string.Empty;
            }

            MarshalStringToMessage(name, ref m);
        }

        /// <summary>
        ///  Handles the WM_GETCONTROLTYPE message. Returns the name of the control.
        /// </summary>
        private void WmGetControlType(ref Message m)
        {
            string type = GetType().AssemblyQualifiedName;
            MarshalStringToMessage(type, ref m);
        }

        /// <summary>
        ///  Handles the WM_GETOBJECT message. Used for accessibility.
        /// </summary>
        private void WmGetObject(ref Message m)
        {
            Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "In WmGetObject, this = " + GetType().FullName + ", lParam = " + m.LParam.ToString());

            InternalAccessibleObject intAccessibleObject = null;

            if (m.Msg == WindowMessages.WM_GETOBJECT && m.LParam == (IntPtr)NativeMethods.UiaRootObjectId && SupportsUiaProviders)
            {
                // If the requested object identifier is UiaRootObjectId,
                // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                intAccessibleObject = new InternalAccessibleObject(AccessibilityObject);
                m.Result = UnsafeNativeMethods.UiaReturnRawElementProvider(
                    new HandleRef(this, Handle),
                    m.WParam,
                    m.LParam,
                    intAccessibleObject);

                return;
            }

            AccessibleObject ctrlAccessibleObject = GetAccessibilityObject(unchecked((int)(long)m.LParam));

            if (ctrlAccessibleObject != null)
            {
                intAccessibleObject = new InternalAccessibleObject(ctrlAccessibleObject);
            }

            // See "How to Handle WM_GETOBJECT" in MSDN
            if (intAccessibleObject != null)
            {
                // Get the IAccessible GUID
                Guid IID_IAccessible = new Guid(NativeMethods.uuid_IAccessible);

                // Get an Lresult for the accessibility Object for this control
                IntPtr punkAcc;
                try
                {
                    // It is critical that we never pass out a raw IAccessible object here,
                    // but always pass out an IAccessibleInternal wrapper. This may not be possible in the current implementation
                    // of WmGetObject - but its important enough to keep this check here in case the code changes in the future in
                    // a way that breaks this assumption.

                    object tempObject = intAccessibleObject;
                    if (tempObject is IAccessible iAccCheck)
                    {
                        throw new InvalidOperationException(SR.ControlAccessibileObjectInvalid);
                    }

                    // Check that we have an IAccessibleInternal implementation and return this
                    UnsafeNativeMethods.IAccessibleInternal iacc = (UnsafeNativeMethods.IAccessibleInternal)intAccessibleObject;

                    if (iacc == null)
                    {
                        // Accessibility is not supported on this control
                        Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibilityObject returned null");
                        m.Result = (IntPtr)0;
                    }
                    else
                    {
                        // Obtain the Lresult
                        punkAcc = Marshal.GetIUnknownForObject(iacc);

                        try
                        {
                            m.Result = UnsafeNativeMethods.LresultFromObject(ref IID_IAccessible, m.WParam, new HandleRef(ctrlAccessibleObject, punkAcc));
                            Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "LresultFromObject returned " + m.Result.ToString());
                        }
                        finally
                        {
                            Marshal.Release(punkAcc);
                        }
                    }
                }
                catch (Exception e)
                {
                    throw new InvalidOperationException(SR.RichControlLresult, e);
                }
            }
            else
            {  // some accessible object requested that we don't care about, so do default message processing
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_HELP message
        /// </summary>
        private void WmHelp(ref Message m)
        {
            // if there's currently a message box open - grab the help info from it.
            HelpInfo hpi = MessageBox.HelpInfo;
            if (hpi != null)
            {
                switch (hpi.Option)
                {
                    case NativeMethods.HLP_FILE:
                        Help.ShowHelp(this, hpi.HelpFilePath);
                        break;
                    case NativeMethods.HLP_KEYWORD:
                        Help.ShowHelp(this, hpi.HelpFilePath, hpi.Keyword);
                        break;
                    case NativeMethods.HLP_NAVIGATOR:
                        Help.ShowHelp(this, hpi.HelpFilePath, hpi.Navigator);
                        break;
                    case NativeMethods.HLP_OBJECT:
                        Help.ShowHelp(this, hpi.HelpFilePath, hpi.Navigator, hpi.Param);
                        break;
                }
            }

            // Note: info.hItemHandle is the handle of the window that sent the help message.
            NativeMethods.HELPINFO info = (NativeMethods.HELPINFO)m.GetLParam(typeof(NativeMethods.HELPINFO));

            HelpEventArgs hevent = new HelpEventArgs(info.MousePos);
            OnHelpRequested(hevent);
            if (!hevent.Handled)
            {
                DefWndProc(ref m);
            }

        }

        /// <summary>
        ///  Handles the WM_CREATE message
        /// </summary>
        private void WmCreate(ref Message m)
        {
            DefWndProc(ref m);

            if (_parent != null)
            {
                _parent.UpdateChildZOrder(this);
            }
            UpdateBounds();

            // Let any interested sites know that we've now created a handle
            OnHandleCreated(EventArgs.Empty);

            // this code is important -- it is critical that we stash away
            // the value of the text for controls such as edit, button,
            // label, etc. Without this processing, any time you change a
            // property that forces handle recreation, you lose your text!
            // See the below code in wmDestroy
            if (!GetStyle(ControlStyles.CacheText))
            {
                _text = null;
            }
        }

        /// <summary>
        ///  Handles the WM_DESTROY message
        /// </summary>
        private void WmDestroy(ref Message m)
        {
            // Let any interested sites know that we're destroying our handle

            if (!RecreatingHandle && !Disposing && !IsDisposed && GetState(STATE_TRACKINGMOUSEEVENT))
            {
                // Raise the MouseLeave event for the control below the mouse
                // when a modal dialog is discarded.
                OnMouseLeave(EventArgs.Empty);
                UnhookMouseEvent();
            }

            if (SupportsUiaProviders)
            {
                ReleaseUiaProvider(Handle);
            }

            OnHandleDestroyed(EventArgs.Empty);

            if (!Disposing)
            {
                // If we are not recreating the handle, set our created state
                // back to false so we can be rebuilt if we need to be.
                if (!RecreatingHandle)
                {
                    SetState(STATE_CREATED, false);
                }
            }
            else
            {
                SetState(STATE_VISIBLE, false);
            }

            DefWndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_CHAR, WM_KEYDOWN, WM_SYSKEYDOWN, WM_KEYUP, and
        ///  WM_SYSKEYUP messages.
        /// </summary>
        private void WmKeyChar(ref Message m)
        {
            if (ProcessKeyMessage(ref m))
            {
                return;
            }

            DefWndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_KILLFOCUS message
        /// </summary>
        private void WmKillFocus(ref Message m)
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Control::WmKillFocus - " + Name);
            WmImeKillFocus();
            DefWndProc(ref m);
            InvokeLostFocus(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the WM_MOUSEDOWN message
        /// </summary>
        private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
        {
            // If this is a "real" mouse event (not just WM_LBUTTONDOWN, etc) then
            // we need to see if something happens during processing of
            // user code that changed the state of the buttons (i.e. bringing up
            // a dialog) to keep the control in a consistent state...
            MouseButtons realState = MouseButtons;
            SetState(STATE_MOUSEPRESSED, true);

            // If the UserMouse style is set, the control does its own processing
            // of mouse messages
            if (!GetStyle(ControlStyles.UserMouse))
            {
                DefWndProc(ref m);
                // we might had re-entered the message loop and processed a WM_CLOSE message
                if (IsDisposed)
                {
                    return;
                }
            }
            else
            {
                // DefWndProc would normally set the focus to this control, but
                // since we're skipping DefWndProc, we need to do it ourselves.
                if (button == MouseButtons.Left && GetStyle(ControlStyles.Selectable))
                {
                    Focus();
                }
            }

            if (realState != MouseButtons)
            {
                return;
            }

            if (!GetState2(STATE2_MAINTAINSOWNCAPTUREMODE))
            {
                //CaptureInternal is set usually in MouseDown (ToolStrip main exception)
                CaptureInternal = true;
            }

            if (realState != MouseButtons)
            {
                return;
            }

            // control should be enabled when this method is entered, but may have become
            // disabled during its lifetime (e.g. through a Click or Focus listener)
            if (Enabled)
            {
                OnMouseDown(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
            }
        }

        /// <summary>
        ///  Handles the WM_MOUSEENTER message
        /// </summary>
        private void WmMouseEnter(ref Message m)
        {
            DefWndProc(ref m);
            KeyboardToolTipStateMachine.Instance.NotifyAboutMouseEnter(this);
            OnMouseEnter(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the WM_MOUSELEAVE message
        /// </summary>
        private void WmMouseLeave(ref Message m)
        {
            DefWndProc(ref m);
            OnMouseLeave(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the WM_DPICHANGED_BEFOREPARENT message. This message is not sent to top level windows.
        /// </summary>
        private void WmDpiChangedBeforeParent(ref Message m)
        {
            DefWndProc(ref m);

            if (IsHandleCreated)
            {
                int deviceDpiOld = _deviceDpi;
                _deviceDpi = (int)UnsafeNativeMethods.GetDpiForWindow(new HandleRef(this, HandleInternal));

                // Controls are by default font scaled.
                // Dpi change requires font to be recalculated inorder to get controls scaled with right dpi.
                if (deviceDpiOld != _deviceDpi)
                {
                    // Checking if font was inherited from parent. Font inherited from parent will receive OnParentFontChanged() events to scale those controls.
                    Font local = (Font)Properties.GetObject(s_fontProperty);
                    if (local != null)
                    {
                        var factor = (float)_deviceDpi / deviceDpiOld;
                        Font = new Font(local.FontFamily, local.Size * factor, local.Style, local.Unit, local.GdiCharSet, local.GdiVerticalFont);
                    }

                    RescaleConstantsForDpi(deviceDpiOld, _deviceDpi);
                }
            }

            OnDpiChangedBeforeParent(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the WM_DPICHANGED_AFTERPARENT message
        /// </summary>
        private void WmDpiChangedAfterParent(ref Message m)
        {
            DefWndProc(ref m);

            OnDpiChangedAfterParent(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the "WM_MOUSEHOVER" message... until we get actuall OS support
        ///  for this, it is implemented as a custom message.
        /// </summary>
        private void WmMouseHover(ref Message m)
        {
            DefWndProc(ref m);
            OnMouseHover(EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the WM_MOUSEMOVE message
        /// </summary>
        private void WmMouseMove(ref Message m)
        {
            // If the UserMouse style is set, the control does its own processing
            // of mouse messages
            if (!GetStyle(ControlStyles.UserMouse))
            {
                DefWndProc(ref m);
            }
            OnMouseMove(new MouseEventArgs(MouseButtons, 0, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
        }

        /// <summary>
        ///  Handles the WM_MOUSEUP message
        /// </summary>
        private void WmMouseUp(ref Message m, MouseButtons button, int clicks)
        {
            // Get the mouse location
            try
            {
                int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                Point pt = new Point(x, y);
                pt = PointToScreen(pt);

                // If the UserMouse style is set, the control does its own processing
                // of mouse messages
                if (!GetStyle(ControlStyles.UserMouse))
                {
                    DefWndProc(ref m);
                }
                else
                {
                    // DefWndProc would normally trigger a context menu here
                    // (for a right button click), but since we're skipping DefWndProc
                    // we have to do it ourselves.
                    if (button == MouseButtons.Right)
                    {
                        SendMessage(WindowMessages.WM_CONTEXTMENU, Handle, NativeMethods.Util.MAKELPARAM(pt.X, pt.Y));
                    }
                }

                bool fireClick = false;

                if ((_controlStyle & ControlStyles.StandardClick) == ControlStyles.StandardClick)
                {
                    if (GetState(STATE_MOUSEPRESSED) && !IsDisposed && UnsafeNativeMethods.WindowFromPoint(pt) == Handle)
                    {
                        fireClick = true;
                    }
                }

                if (fireClick && !ValidationCancelled)
                {
                    if (!GetState(STATE_DOUBLECLICKFIRED))
                    {
                        //OnClick(EventArgs.Empty);
                        //In Whidbey .. if the click in by MOUSE then pass the MouseEventArgs...
                        OnClick(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        OnMouseClick(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));

                    }

                    else
                    {
                        //OnDoubleClick(EventArgs.Empty);
                        OnDoubleClick(new MouseEventArgs(button, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        OnMouseDoubleClick(new MouseEventArgs(button, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                    }

                }

                //call the MouseUp Finally...
                OnMouseUp(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
            }
            finally
            {
                //Always Reset the STATE_DOUBLECLICKFIRED in UP.. Since we get UP - DOWN - DBLCLK - UP sequqnce
                //The Flag is set in L_BUTTONDBLCLK in the controls WndProc() ...
                SetState(STATE_DOUBLECLICKFIRED, false);
                SetState(STATE_MOUSEPRESSED, false);
                SetState(STATE_VALIDATIONCANCELLED, false);
                //CaptureInternal is Resetted while exiting the MouseUp
                CaptureInternal = false;
            }
        }

        /// <summary>
        ///  Handles the WM_MOUSEWHEEL message
        /// </summary>
        private void WmMouseWheel(ref Message m)
        {
            Point p = new Point(NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam));
            p = PointToClient(p);
            HandledMouseEventArgs e = new HandledMouseEventArgs(MouseButtons.None,
                                                                0,
                                                                p.X,
                                                                p.Y,
                                                                NativeMethods.Util.SignedHIWORD(m.WParam));
            OnMouseWheel(e);
            m.Result = (IntPtr)(e.Handled ? 0 : 1);
            if (!e.Handled)
            {
                // Forwarding the message to the parent window
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_MOVE message.  We must do this in
        ///  addition to WM_WINDOWPOSCHANGED because windows may
        ///  send WM_MOVE directly.
        /// </summary>
        private void WmMove(ref Message m)
        {
            DefWndProc(ref m);
            UpdateBounds();
        }

        /// <summary>
        ///  Handles the WM_NOTIFY message
        /// </summary>
        private unsafe void WmNotify(ref Message m)
        {
            NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR*)m.LParam;
            if (!ReflectMessage(nmhdr->hwndFrom, ref m))
            {
                if (nmhdr->code == NativeMethods.TTN_SHOW)
                {
                    m.Result = UnsafeNativeMethods.SendMessage(new HandleRef(null, nmhdr->hwndFrom), WindowMessages.WM_REFLECT + m.Msg, m.WParam, m.LParam);
                    return;
                }
                if (nmhdr->code == NativeMethods.TTN_POP)
                {
                    UnsafeNativeMethods.SendMessage(new HandleRef(null, nmhdr->hwndFrom), WindowMessages.WM_REFLECT + m.Msg, m.WParam, m.LParam);
                }

                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_NOTIFYFORMAT message
        /// </summary>
        private void WmNotifyFormat(ref Message m)
        {
            if (!ReflectMessage(m.WParam, ref m))
            {
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_DRAWITEM\WM_MEASUREITEM messages for controls other than menus
        /// </summary>
        private void WmOwnerDraw(ref Message m)
        {
            bool reflectCalled = false;

            int ctrlId = unchecked((int)(long)m.WParam);
            IntPtr p = UnsafeNativeMethods.GetDlgItem(new HandleRef(null, m.HWnd), ctrlId);
            if (p == IntPtr.Zero)
            {
                // On 64-bit platforms wParam is already 64 bit but the control ID stored in it is only 32-bit
                // Empirically, we have observed that the 64 bit HWND is just a sign extension of the 32-bit ctrl ID
                // Since WParam is already 64-bit, we need to discard the high dword first and then re-extend the 32-bit value
                // treating it as signed
                p = (IntPtr)(long)ctrlId;
            }
            if (!ReflectMessage(p, ref m))
            {
                //Additional Check For Control .... TabControl truncates the Hwnd value...
                IntPtr handle = _window.GetHandleFromID((short)NativeMethods.Util.LOWORD(m.WParam));
                if (handle != IntPtr.Zero)
                {
                    Control control = FromHandle(handle);
                    if (control != null)
                    {
                        m.Result = control.SendMessage(WindowMessages.WM_REFLECT + m.Msg, handle, m.LParam);
                        reflectCalled = true;
                    }
                }
            }
            else
            {
                reflectCalled = true;
            }

            if (!reflectCalled)
            {
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_PAINT messages.  This should only be called
        ///  for userpaint controls.
        /// </summary>
        private void WmPaint(ref Message m)
        {
            bool doubleBuffered = DoubleBuffered || (GetStyle(ControlStyles.AllPaintingInWmPaint) && DoubleBufferingEnabled);
#if DEBUG
            if (s_bufferDisabled.Enabled)
            {
                doubleBuffered = false;
            }
#endif
            IntPtr hWnd = IntPtr.Zero;
            IntPtr dc;
            Rectangle clip;
            NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
            bool needDisposeDC = false;

            try
            {
                if (m.WParam == IntPtr.Zero)
                {
                    // Cache Handle not only for perf but to avoid object disposed exception in case the window
                    // is destroyed in an event handler.
                    hWnd = Handle;
                    dc = UnsafeNativeMethods.BeginPaint(new HandleRef(this, hWnd), ref ps);
                    if (dc == IntPtr.Zero)
                    {
                        return;
                    }
                    needDisposeDC = true;
                    clip = new Rectangle(ps.rcPaint_left, ps.rcPaint_top,
                                         ps.rcPaint_right - ps.rcPaint_left,
                                         ps.rcPaint_bottom - ps.rcPaint_top);
                }
                else
                {
                    dc = m.WParam;
                    clip = ClientRectangle;
                }

                // Consider: Why don't check the clip condition when non-doubleBuffered?
                //           we should probably get rid of the !doubleBuffered condition.
                if (!doubleBuffered || (clip.Width > 0 && clip.Height > 0))
                {
                    IntPtr oldPal = IntPtr.Zero;
                    BufferedGraphics bufferedGraphics = null;
                    PaintEventArgs pevent = null;
                    GraphicsState state = null;

                    try
                    {
                        if (doubleBuffered || m.WParam == IntPtr.Zero)
                        {
                            oldPal = SetUpPalette(dc, false, false);
                        }

                        if (doubleBuffered)
                        {
                            try
                            {
                                bufferedGraphics = BufferContext.Allocate(dc, ClientRectangle);
#if DEBUG
                                if (s_bufferPinkRect.Enabled)
                                {
                                    Rectangle band = ClientRectangle;
                                    using (BufferedGraphics bufferedGraphics2 = BufferContext.Allocate(dc, band))
                                    {
                                        bufferedGraphics2.Graphics.FillRectangle(new SolidBrush(Color.Red), band);
                                        bufferedGraphics2.Render();
                                    }
                                    Thread.Sleep(50);
                                }
#endif
                            }
                            catch (Exception ex)
                            {
                                // BufferContext.Allocate will throw out of memory exceptions
                                // when it fails to create a device dependent bitmap while trying to
                                // get information about the device we are painting on.
                                // That is not the same as a system running out of memory and there is a
                                // very good chance that we can continue to paint successfully. We cannot
                                // check whether double buffering is supported in this case, and we will disable it.
                                // We could set a specific string when throwing the exception and check for it here
                                // to distinguish between that case and real out of memory exceptions but we
                                // see no reasons justifying the additional complexity.
                                if (ClientUtils.IsCriticalException(ex) && !(ex is OutOfMemoryException))
                                {
                                    throw;
                                }
#if DEBUG
                                if (s_bufferPinkRect.Enabled)
                                {
                                    Debug.WriteLine("Could not create buffered graphics, will paint in the surface directly");
                                }
#endif
                                doubleBuffered = false; // paint directly on the window DC.
                            }
                        }

                        if (bufferedGraphics != null)
                        {
                            bufferedGraphics.Graphics.SetClip(clip);
                            pevent = new PaintEventArgs(bufferedGraphics.Graphics, clip);
                            state = pevent.Graphics.Save();
                        }
                        else
                        {
                            pevent = new PaintEventArgs(dc, clip);
                        }

                        using (pevent)
                        {
                            try
                            {
                                if ((m.WParam == IntPtr.Zero) && GetStyle(ControlStyles.AllPaintingInWmPaint) || doubleBuffered)
                                {
                                    PaintWithErrorHandling(pevent, PaintLayerBackground);
                                    // Consider: This condition could be elimiated,
                                    //           do we have to save/restore the state of the buffered graphics?

                                }
                            }
                            finally
                            {
                                if (state != null)
                                {
                                    pevent.Graphics.Restore(state);
                                }
                                else
                                {
                                    pevent.ResetGraphics();
                                }
                            }
                            PaintWithErrorHandling(pevent, PaintLayerForeground);

                            if (bufferedGraphics != null)
                            {
                                bufferedGraphics.Render();
                            }
                        }
                    }
                    finally
                    {
                        if (oldPal != IntPtr.Zero)
                        {
                            SafeNativeMethods.SelectPalette(new HandleRef(null, dc), new HandleRef(null, oldPal), 0);
                        }
                        if (bufferedGraphics != null)
                        {
                            bufferedGraphics.Dispose();
                        }

                    }
                }
            }
            finally
            {
                if (needDisposeDC)
                {
                    UnsafeNativeMethods.EndPaint(new HandleRef(this, hWnd), ref ps);
                }
            }
        }

        /// <summary>
        ///  Handles the WM_PRINTCLIENT messages.
        /// </summary>
        private void WmPrintClient(ref Message m)
        {
            using (PaintEventArgs e = new PrintPaintEventArgs(m, m.WParam, ClientRectangle))
            {
                OnPrint(e);
            }
        }

        private void WmQueryNewPalette(ref Message m)
        {
            Debug.WriteLineIf(s_paletteTracing.TraceVerbose, Handle + ": WM_QUERYNEWPALETTE");
            IntPtr dc = User32.GetDC(new HandleRef(this, Handle));
            try
            {
                SetUpPalette(dc, true /*force*/, true/*realize*/);
            }
            finally
            {
                // Let WmPaletteChanged do any necessary invalidation
                User32.ReleaseDC(new HandleRef(this, Handle), dc);
            }
            Invalidate(true);
            m.Result = (IntPtr)1;
            DefWndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_SETCURSOR message
        /// </summary>
        private void WmSetCursor(ref Message m)
        {
            // Accessing through the Handle property has side effects that break this
            // logic. You must use InternalHandle.
            //
            if (m.WParam == InternalHandle && NativeMethods.Util.LOWORD(m.LParam) == NativeMethods.HTCLIENT)
            {
                Cursor.Current = Cursor;
            }
            else
            {
                DefWndProc(ref m);
            }

        }

        /// <summary>
        ///  Handles the WM_WINDOWPOSCHANGING message
        /// </summary>
        private unsafe void WmWindowPosChanging(ref Message m)
        {
            // We let this fall through to defwndproc unless we are being surfaced as
            // an ActiveX control.  In that case, we must let the ActiveX side of things
            // manipulate our bounds here.
            //
            if (IsActiveX)
            {
                NativeMethods.WINDOWPOS* wp = (NativeMethods.WINDOWPOS*)m.LParam;
                // Only call UpdateBounds if the new bounds are different.
                //
                bool different = false;

                if ((wp->flags & NativeMethods.SWP_NOMOVE) == 0 && (wp->x != Left || wp->y != Top))
                {
                    different = true;
                }
                if ((wp->flags & NativeMethods.SWP_NOSIZE) == 0 && (wp->cx != Width || wp->cy != Height))
                {
                    different = true;
                }

                if (different)
                {
                    ActiveXUpdateBounds(ref wp->x, ref wp->y, ref wp->cx, ref wp->cy, wp->flags);
                }
            }

            DefWndProc(ref m);
        }

        /// <summary>
        ///  Handles the WM_PARENTNOTIFY message
        /// </summary>
        private void WmParentNotify(ref Message m)
        {
            int msg = NativeMethods.Util.LOWORD(m.WParam);
            IntPtr hWnd = IntPtr.Zero;
            switch (msg)
            {
                case WindowMessages.WM_CREATE:
                    hWnd = m.LParam;
                    break;
                case WindowMessages.WM_DESTROY:
                    break;
                default:
                    hWnd = UnsafeNativeMethods.GetDlgItem(new HandleRef(this, Handle), NativeMethods.Util.HIWORD(m.WParam));
                    break;
            }
            if (hWnd == IntPtr.Zero || !ReflectMessage(hWnd, ref m))
            {
                DefWndProc(ref m);
            }
        }

        /// <summary>
        ///  Handles the WM_SETFOCUS message
        /// </summary>
        private void WmSetFocus(ref Message m)
        {
            Debug.WriteLineIf(s_focusTracing.TraceVerbose, "Control::WmSetFocus - " + Name);
            WmImeSetFocus();

            if (!HostedInWin32DialogManager)
            {
                IContainerControl c = GetContainerControl();
                if (c != null)
                {
                    bool activateSucceed;

                    if (c is ContainerControl knowncontainer)
                    {
                        activateSucceed = knowncontainer.ActivateControl(this);
                    }
                    else
                    {
                        // Reviewed : Taking focus and activating a control in response
                        //          : to a user gesture (WM_SETFOCUS) is OK.
                        //
                        activateSucceed = c.ActivateControl(this);
                    }

                    if (!activateSucceed)
                    {
                        return;
                    }
                }
            }

            DefWndProc(ref m);
            InvokeGotFocus(this, EventArgs.Empty);
        }

        /// <summary>
        ///  Handles the WM_SHOWWINDOW message
        /// </summary>
        private void WmShowWindow(ref Message m)
        {
            // We get this message for each control, even if their parent is not visible.

            DefWndProc(ref m);

            if ((_state & STATE_RECREATE) == 0)
            {

                bool visible = m.WParam != IntPtr.Zero;
                bool oldVisibleProperty = Visible;

                if (visible)
                {
                    bool oldVisibleBit = GetState(STATE_VISIBLE);
                    SetState(STATE_VISIBLE, true);
                    bool executedOk = false;
                    try
                    {
                        CreateControl();
                        executedOk = true;
                    }

                    finally
                    {
                        if (!executedOk)
                        {
                            // We do it this way instead of a try/catch because catching and rethrowing
                            // an exception loses call stack information
                            SetState(STATE_VISIBLE, oldVisibleBit);
                        }
                    }
                }
                else
                {
                    // not visible
                    // If Windows tells us it's visible, that's pretty unambiguous.
                    // But if it tells us it's not visible, there's more than one explanation --
                    // maybe the container control became invisible.  So we look at the parent
                    // and take a guess at the reason.

                    // We do not want to update state if we are on the parking window.
                    bool parentVisible = GetTopLevel();
                    if (ParentInternal != null)
                    {
                        parentVisible = ParentInternal.Visible;
                    }

                    if (parentVisible)
                    {
                        SetState(STATE_VISIBLE, false);
                    }
                }

                if (!GetState(STATE_PARENTRECREATING) && (oldVisibleProperty != visible))
                {
                    OnVisibleChanged(EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Handles the WM_UPDATEUISTATE message
        /// </summary>
        private void WmUpdateUIState(ref Message m)
        {
            // See "How this all works" in ShowKeyboardCues

            bool keyboard = false;
            bool focus = false;

            // check the cached values in uiCuesState to see if we've ever set in the UI state
            bool keyboardInitialized = (_uiCuesState & UISTATE_KEYBOARD_CUES_MASK) != 0;
            bool focusInitialized = (_uiCuesState & UISTATE_FOCUS_CUES_MASK) != 0;

            if (keyboardInitialized)
            {
                keyboard = ShowKeyboardCues;
            }

            if (focusInitialized)
            {
                focus = ShowFocusCues;
            }

            DefWndProc(ref m);

            int cmd = NativeMethods.Util.LOWORD(m.WParam);

            // if we're initializing, dont bother updating the uiCuesState/Firing the event.

            if (cmd == NativeMethods.UIS_INITIALIZE)
            {
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
            if ((NativeMethods.Util.HIWORD(m.WParam) & NativeMethods.UISF_HIDEACCEL) != 0)
            {

                // yes, clear means show.  nice api, guys.
                //
                bool showKeyboard = (cmd == NativeMethods.UIS_CLEAR);

                if (showKeyboard != keyboard || !keyboardInitialized)
                {

                    UIcues |= UICues.ChangeKeyboard;

                    // clear the old state.
                    //
                    _uiCuesState &= ~UISTATE_KEYBOARD_CUES_MASK;
                    _uiCuesState |= (showKeyboard ? UISTATE_KEYBOARD_CUES_SHOW : UISTATE_KEYBOARD_CUES_HIDDEN);
                }

                if (showKeyboard)
                {
                    UIcues |= UICues.ShowKeyboard;
                }
            }

            // Same deal for the Focus cues as the keyboard cues.
            if ((NativeMethods.Util.HIWORD(m.WParam) & NativeMethods.UISF_HIDEFOCUS) != 0)
            {
                // yes, clear means show.  nice api, guys.
                bool showFocus = (cmd == NativeMethods.UIS_CLEAR);

                if (showFocus != focus || !focusInitialized)
                {
                    UIcues |= UICues.ChangeFocus;

                    // clear the old state.
                    _uiCuesState &= ~UISTATE_FOCUS_CUES_MASK;
                    _uiCuesState |= (showFocus ? UISTATE_FOCUS_CUES_SHOW : UISTATE_FOCUS_CUES_HIDDEN);
                }

                if (showFocus)
                {
                    UIcues |= UICues.ShowFocus;
                }
            }

            // fire the UI cues state changed event.
            if ((UIcues & UICues.Changed) != 0)
            {
                OnChangeUICues(new UICuesEventArgs(UIcues));
                Invalidate(true);
            }
        }

        /// <summary>
        ///  Handles the WM_WINDOWPOSCHANGED message
        /// </summary>
        private unsafe void WmWindowPosChanged(ref Message m)
        {
            DefWndProc(ref m);
            // Update new size / position
            UpdateBounds();
            if (_parent != null && UnsafeNativeMethods.GetParent(new HandleRef(_window, InternalHandle)) == _parent.InternalHandle &&
                (_state & STATE_NOZORDER) == 0)
            {

                NativeMethods.WINDOWPOS* wp = (NativeMethods.WINDOWPOS*)m.LParam;
                if ((wp->flags & NativeMethods.SWP_NOZORDER) == 0)
                {
                    _parent.UpdateChildControlIndex(this);
                }
            }
        }

        /// <summary>
        ///  Base wndProc. All messages are sent to wndProc after getting filtered
        ///  through the preProcessMessage function. Inheriting controls should
        ///  call base.wndProc for any messages that they don't handle.
        /// </summary>
        protected virtual void WndProc(ref Message m)
        {
            // inlined code from GetStyle(...) to ensure no perf hit
            // for a method call...

            if ((_controlStyle & ControlStyles.EnableNotifyMessage) == ControlStyles.EnableNotifyMessage)
            {
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
            switch (m.Msg)
            {
                case WindowMessages.WM_CAPTURECHANGED:
                    WmCaptureChanged(ref m);
                    break;

                case WindowMessages.WM_GETOBJECT:
                    WmGetObject(ref m);
                    break;

                case WindowMessages.WM_COMMAND:
                    WmCommand(ref m);
                    break;

                case WindowMessages.WM_CLOSE:
                    WmClose(ref m);
                    break;

                case WindowMessages.WM_CONTEXTMENU:
                    WmContextMenu(ref m);
                    break;

                case WindowMessages.WM_DISPLAYCHANGE:
                    WmDisplayChange(ref m);
                    break;

                case WindowMessages.WM_DRAWITEM:
                    if (m.WParam != IntPtr.Zero)
                    {
                        WmOwnerDraw(ref m);
                    }
                    break;

                case WindowMessages.WM_ERASEBKGND:
                    WmEraseBkgnd(ref m);
                    break;

                case WindowMessages.WM_HELP:
                    WmHelp(ref m);
                    break;

                case WindowMessages.WM_PAINT:
                    if (GetStyle(ControlStyles.UserPaint))
                    {
                        WmPaint(ref m);
                    }
                    else
                    {
                        DefWndProc(ref m);
                    }
                    break;

                case WindowMessages.WM_PRINTCLIENT:
                    if (GetStyle(ControlStyles.UserPaint))
                    {
                        WmPrintClient(ref m);
                    }
                    else
                    {
                        DefWndProc(ref m);
                    }
                    break;

                case WindowMessages.WM_SYSCOMMAND:
                    if ((unchecked((int)(long)m.WParam) & 0xFFF0) == NativeMethods.SC_KEYMENU)
                    {
                        Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.WndProc processing " + m.ToString());

                        if (ToolStripManager.ProcessMenuKey(ref m))
                        {
                            Debug.WriteLineIf(s_controlKeyboardRouting.TraceVerbose, "Control.WndProc ToolStripManager.ProcessMenuKey returned true" + m.ToString());
                            m.Result = IntPtr.Zero;
                            return;
                        }
                    }
                    DefWndProc(ref m);
                    break;

                case WindowMessages.WM_INPUTLANGCHANGE:
                    WmInputLangChange(ref m);
                    break;

                case WindowMessages.WM_INPUTLANGCHANGEREQUEST:
                    WmInputLangChangeRequest(ref m);
                    break;

                case WindowMessages.WM_MEASUREITEM:
                    if (m.WParam != IntPtr.Zero)
                    {
                        WmOwnerDraw(ref m);
                    }
                    break;

                case WindowMessages.WM_SETCURSOR:
                    WmSetCursor(ref m);
                    break;

                case WindowMessages.WM_WINDOWPOSCHANGING:
                    WmWindowPosChanging(ref m);
                    break;

                case WindowMessages.WM_CHAR:
                case WindowMessages.WM_KEYDOWN:
                case WindowMessages.WM_SYSKEYDOWN:
                case WindowMessages.WM_KEYUP:
                case WindowMessages.WM_SYSKEYUP:
                    WmKeyChar(ref m);
                    break;

                case WindowMessages.WM_CREATE:
                    WmCreate(ref m);
                    break;

                case WindowMessages.WM_DESTROY:
                    WmDestroy(ref m);
                    break;

                case WindowMessages.WM_CTLCOLOR:
                case WindowMessages.WM_CTLCOLORBTN:
                case WindowMessages.WM_CTLCOLORDLG:
                case WindowMessages.WM_CTLCOLORMSGBOX:
                case WindowMessages.WM_CTLCOLORSCROLLBAR:
                case WindowMessages.WM_CTLCOLOREDIT:
                case WindowMessages.WM_CTLCOLORLISTBOX:
                case WindowMessages.WM_CTLCOLORSTATIC:

                // this is for the trinity guys.  The case is if you've got a windows
                // forms edit or something hosted as an AX control somewhere, there isn't anyone to reflect
                // these back.  If they went ahead and just sent them back, some controls don't like that
                // and end up recursing.  Our code handles it fine because we just pick the HWND out of the LPARAM.
                //
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLOR:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLORBTN:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLORDLG:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLORMSGBOX:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLORSCROLLBAR:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLOREDIT:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLORLISTBOX:
                case WindowMessages.WM_REFLECT + WindowMessages.WM_CTLCOLORSTATIC:
                    WmCtlColorControl(ref m);
                    break;

                case WindowMessages.WM_HSCROLL:
                case WindowMessages.WM_VSCROLL:
                case WindowMessages.WM_DELETEITEM:
                case WindowMessages.WM_VKEYTOITEM:
                case WindowMessages.WM_CHARTOITEM:
                case WindowMessages.WM_COMPAREITEM:
                    if (!ReflectMessage(m.LParam, ref m))
                    {
                        DefWndProc(ref m);
                    }
                    break;

                case WindowMessages.WM_IME_CHAR:
                    WmImeChar(ref m);
                    break;

                case WindowMessages.WM_IME_STARTCOMPOSITION:
                    WmImeStartComposition(ref m);
                    break;

                case WindowMessages.WM_IME_ENDCOMPOSITION:
                    WmImeEndComposition(ref m);
                    break;

                case WindowMessages.WM_IME_NOTIFY:
                    WmImeNotify(ref m);
                    break;

                case WindowMessages.WM_KILLFOCUS:
                    WmKillFocus(ref m);
                    break;

                case WindowMessages.WM_LBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Left, 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick))
                    {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;

                case WindowMessages.WM_LBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Left, 1);
                    break;

                case WindowMessages.WM_LBUTTONUP:
                    WmMouseUp(ref m, MouseButtons.Left, 1);
                    break;

                case WindowMessages.WM_MBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Middle, 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick))
                    {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;

                case WindowMessages.WM_MBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Middle, 1);
                    break;

                case WindowMessages.WM_MBUTTONUP:
                    WmMouseUp(ref m, MouseButtons.Middle, 1);
                    break;

                case WindowMessages.WM_XBUTTONDOWN:
                    WmMouseDown(ref m, GetXButton(NativeMethods.Util.HIWORD(m.WParam)), 1);
                    break;

                case WindowMessages.WM_XBUTTONUP:
                    WmMouseUp(ref m, GetXButton(NativeMethods.Util.HIWORD(m.WParam)), 1);
                    break;

                case WindowMessages.WM_XBUTTONDBLCLK:
                    WmMouseDown(ref m, GetXButton(NativeMethods.Util.HIWORD(m.WParam)), 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick))
                    {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;

                case WindowMessages.WM_MOUSELEAVE:
                    WmMouseLeave(ref m);
                    break;

                case WindowMessages.WM_DPICHANGED_BEFOREPARENT:
                    WmDpiChangedBeforeParent(ref m);
                    m.Result = IntPtr.Zero;
                    break;

                case WindowMessages.WM_DPICHANGED_AFTERPARENT:
                    WmDpiChangedAfterParent(ref m);
                    m.Result = IntPtr.Zero;
                    break;

                case WindowMessages.WM_MOUSEMOVE:
                    WmMouseMove(ref m);
                    break;

                case WindowMessages.WM_MOUSEWHEEL:
                    WmMouseWheel(ref m);
                    break;

                case WindowMessages.WM_MOVE:
                    WmMove(ref m);
                    break;

                case WindowMessages.WM_NOTIFY:
                    WmNotify(ref m);
                    break;

                case WindowMessages.WM_NOTIFYFORMAT:
                    WmNotifyFormat(ref m);
                    break;

                case WindowMessages.WM_REFLECT + WindowMessages.WM_NOTIFYFORMAT:
                    m.Result = (IntPtr)(NativeMethods.NFR_UNICODE);
                    break;

                case WindowMessages.WM_SHOWWINDOW:
                    WmShowWindow(ref m);
                    break;

                case WindowMessages.WM_RBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Right, 2);
                    if (GetStyle(ControlStyles.StandardDoubleClick))
                    {
                        SetState(STATE_DOUBLECLICKFIRED, true);
                    }
                    break;

                case WindowMessages.WM_RBUTTONDOWN:
                    WmMouseDown(ref m, MouseButtons.Right, 1);
                    break;

                case WindowMessages.WM_RBUTTONUP:
                    WmMouseUp(ref m, MouseButtons.Right, 1);
                    break;

                case WindowMessages.WM_SETFOCUS:
                    WmSetFocus(ref m);
                    break;

                case WindowMessages.WM_MOUSEHOVER:
                    WmMouseHover(ref m);
                    break;

                case WindowMessages.WM_WINDOWPOSCHANGED:
                    WmWindowPosChanged(ref m);
                    break;

                case WindowMessages.WM_QUERYNEWPALETTE:
                    WmQueryNewPalette(ref m);
                    break;

                case WindowMessages.WM_UPDATEUISTATE:
                    WmUpdateUIState(ref m);
                    break;

                case WindowMessages.WM_PARENTNOTIFY:
                    WmParentNotify(ref m);
                    break;

                case WindowMessages.WM_EXITMENULOOP:
                case WindowMessages.WM_INITMENUPOPUP:
                case WindowMessages.WM_MENUSELECT:
                default:

                    // If we received a thread execute message, then execute it.
                    if (m.Msg == s_threadCallbackMessage && m.Msg != 0)
                    {
                        InvokeMarshaledCallbacks();
                        return;
                    }
                    else if (m.Msg == WM_GETCONTROLNAME)
                    {
                        WmGetControlName(ref m);
                        return;
                    }
                    else if (m.Msg == WM_GETCONTROLTYPE)
                    {
                        WmGetControlType(ref m);
                        return;
                    }

                    if (m.Msg == NativeMethods.WM_MOUSEENTER)
                    {
                        WmMouseEnter(ref m);
                        break;
                    }

                    DefWndProc(ref m);
                    break;
            }
        }

        /// <summary>
        ///  Called when an exception occurs in dispatching messages through
        ///  the main window procedure.
        /// </summary>
        private void WndProcException(Exception e)
        {
            Application.OnThreadException(e);
        }

        ArrangedElementCollection IArrangedElement.Children
        {
            get
            {
                ControlCollection controlsCollection = (ControlCollection)Properties.GetObject(s_controlsCollectionProperty);
                if (controlsCollection == null)
                {
                    return ArrangedElementCollection.Empty;
                }
                return controlsCollection;
            }
        }

        IArrangedElement IArrangedElement.Container
        {
            get
            {
                // This is safe because the IArrangedElement interface is internal
                return ParentInternal;
            }
        }

        bool IArrangedElement.ParticipatesInLayout
        {
            get { return GetState(STATE_VISIBLE); }
        }

        void IArrangedElement.PerformLayout(IArrangedElement affectedElement, string affectedProperty)
        {
            PerformLayout(new LayoutEventArgs(affectedElement, affectedProperty));
        }

        PropertyStore IArrangedElement.Properties
        {
            get { return Properties; }
        }

        // CAREFUL: This really calls SetBoundsCore, not SetBounds.
        void IArrangedElement.SetBounds(Rectangle bounds, BoundsSpecified specified)
        {
            ISite site = Site;
            IComponentChangeService ccs = null;
            PropertyDescriptor sizeProperty = null;
            PropertyDescriptor locationProperty = null;
            bool sizeChanged = false;
            bool locationChanged = false;

            if (site != null && site.DesignMode)
            {
                ccs = (IComponentChangeService)site.GetService(typeof(IComponentChangeService));
                if (ccs != null)
                {
                    sizeProperty = TypeDescriptor.GetProperties(this)[PropertyNames.Size];
                    locationProperty = TypeDescriptor.GetProperties(this)[PropertyNames.Location];
                    Debug.Assert(sizeProperty != null && locationProperty != null, "Error retrieving Size/Location properties on Control.");

                    try
                    {
                        if (sizeProperty != null && !sizeProperty.IsReadOnly && (bounds.Width != Width || bounds.Height != Height))
                        {
                            if (!(site is INestedSite))
                            {
                                ccs.OnComponentChanging(this, sizeProperty);
                            }
                            sizeChanged = true;
                        }
                        if (locationProperty != null && !locationProperty.IsReadOnly && (bounds.X != _x || bounds.Y != _y))
                        {
                            if (!(site is INestedSite))
                            {
                                ccs.OnComponentChanging(this, locationProperty);
                            }
                            locationChanged = true;
                        }
                    }
                    catch (InvalidOperationException)
                    {
                        // The component change events can throw InvalidOperationException if a change is
                        // currently not allowed (typically because the doc data in VS is locked).
                        // When this happens, we just eat the exception and proceed with the change.
                    }
                }
            }

            SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, specified);

            if (site != null && ccs != null)
            {
                try
                {
                    if (sizeChanged)
                    {
                        ccs.OnComponentChanged(this, sizeProperty, null, null);
                    }

                    if (locationChanged)
                    {
                        ccs.OnComponentChanged(this, locationProperty, null, null);
                    }
                }
                catch (InvalidOperationException)
                {
                    // The component change events can throw InvalidOperationException if a change is
                    // currently not allowed (typically because the doc data in VS is locked).
                    // When this happens, we just eat the exception and proceed with the change.
                }
            }
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces
        /// </summary>
        internal virtual bool SupportsUiaProviders
        {
            get
            {
                return false;
            }
        }


        ///
        ///  Explicit support of DropTarget
        ///
        void IDropTarget.OnDragEnter(DragEventArgs drgEvent)
        {
            OnDragEnter(drgEvent);
        }

        void IDropTarget.OnDragOver(DragEventArgs drgEvent)
        {
            OnDragOver(drgEvent);
        }

        void IDropTarget.OnDragLeave(EventArgs e)
        {
            OnDragLeave(e);
        }

        void IDropTarget.OnDragDrop(DragEventArgs drgEvent)
        {
            OnDragDrop(drgEvent);
        }

        ///
        ///  Explicit support of DropSource
        ///
        void ISupportOleDropSource.OnGiveFeedback(GiveFeedbackEventArgs giveFeedbackEventArgs)
        {
            OnGiveFeedback(giveFeedbackEventArgs);
        }

        void ISupportOleDropSource.OnQueryContinueDrag(QueryContinueDragEventArgs queryContinueDragEventArgs)
        {
            OnQueryContinueDrag(queryContinueDragEventArgs);
        }

        int UnsafeNativeMethods.IOleControl.GetControlInfo(NativeMethods.tagCONTROLINFO pCI)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetControlInfo");

            pCI.cb = Marshal.SizeOf<NativeMethods.tagCONTROLINFO>();
            pCI.hAccel = IntPtr.Zero;
            pCI.cAccel = 0;
            pCI.dwFlags = 0;

            if (IsInputKey(Keys.Return))
            {
                pCI.dwFlags |= NativeMethods.CTRLINFO_EATS_RETURN;
            }
            if (IsInputKey(Keys.Escape))
            {
                pCI.dwFlags |= NativeMethods.CTRLINFO_EATS_ESCAPE;
            }

            ActiveXInstance.GetControlInfo(pCI);
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleControl.OnMnemonic(ref NativeMethods.MSG pMsg)
        {
            // If we got a mnemonic here, then the appropriate control will focus itself which
            // will cause us to become UI active.
            //
            bool processed = ProcessMnemonic((char)pMsg.wParam);
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnMnemonic processed: " + processed.ToString());
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleControl.OnAmbientPropertyChange(int dispID)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnAmbientPropertyChange.  Dispid: " + dispID);
            Debug.Indent();
            ActiveXInstance.OnAmbientPropertyChange(dispID);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleControl.FreezeEvents(int bFreeze)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:FreezeEvents.  Freeze: " + bFreeze);
            ActiveXInstance.EventsFrozen = (bFreeze != 0);
            Debug.Assert(ActiveXInstance.EventsFrozen == (bFreeze != 0), "Failed to set EventsFrozen correctly");
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleInPlaceActiveObject.GetWindow(out IntPtr hwnd)
        {
            return ((UnsafeNativeMethods.IOleInPlaceObject)this).GetWindow(out hwnd);
        }

        void UnsafeNativeMethods.IOleInPlaceActiveObject.ContextSensitiveHelp(int fEnterMode)
        {
            ((UnsafeNativeMethods.IOleInPlaceObject)this).ContextSensitiveHelp(fEnterMode);
        }

        int UnsafeNativeMethods.IOleInPlaceActiveObject.TranslateAccelerator(ref NativeMethods.MSG lpmsg)
        {
            return ActiveXInstance.TranslateAccelerator(ref lpmsg);
        }

        void UnsafeNativeMethods.IOleInPlaceActiveObject.OnFrameWindowActivate(bool fActivate)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnFrameWindowActivate");
            OnFrameWindowActivate(fActivate);
        }

        void UnsafeNativeMethods.IOleInPlaceActiveObject.OnDocWindowActivate(int fActivate)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OnDocWindowActivate.  Activate: " + fActivate.ToString(CultureInfo.InvariantCulture));
            Debug.Indent();
            ActiveXInstance.OnDocWindowActivate(fActivate);
            Debug.Unindent();
        }

        void UnsafeNativeMethods.IOleInPlaceActiveObject.ResizeBorder(NativeMethods.COMRECT prcBorder, UnsafeNativeMethods.IOleInPlaceUIWindow pUIWindow, bool fFrameWindow)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:ResizesBorder");
            // return NativeMethods.S_OK;
        }

        void UnsafeNativeMethods.IOleInPlaceActiveObject.EnableModeless(int fEnable)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:EnableModeless");
        }

        int UnsafeNativeMethods.IOleInPlaceObject.GetWindow(out IntPtr hwnd)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetWindow");
            int hr = ActiveXInstance.GetWindow(out hwnd);
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "\twin == " + hwnd);
            return hr;
        }

        void UnsafeNativeMethods.IOleInPlaceObject.ContextSensitiveHelp(int fEnterMode)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:ContextSensitiveHelp.  Mode: " + fEnterMode.ToString(CultureInfo.InvariantCulture));
            if (fEnterMode != 0)
            {
                OnHelpRequested(new HelpEventArgs(MousePosition));
            }
        }

        void UnsafeNativeMethods.IOleInPlaceObject.InPlaceDeactivate()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:InPlaceDeactivate");
            Debug.Indent();
            ActiveXInstance.InPlaceDeactivate();
            Debug.Unindent();
        }

        int UnsafeNativeMethods.IOleInPlaceObject.UIDeactivate()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:UIDeactivate");
            return ActiveXInstance.UIDeactivate();
        }

        void UnsafeNativeMethods.IOleInPlaceObject.SetObjectRects(NativeMethods.COMRECT lprcPosRect, NativeMethods.COMRECT lprcClipRect)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetObjectRects(" + lprcClipRect.left + ", " + lprcClipRect.top + ", " + lprcClipRect.right + ", " + lprcClipRect.bottom + ")");
            Debug.Indent();
            ActiveXInstance.SetObjectRects(lprcPosRect, lprcClipRect);
            Debug.Unindent();
        }

        void UnsafeNativeMethods.IOleInPlaceObject.ReactivateAndUndo()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:ReactivateAndUndo");
        }

        int UnsafeNativeMethods.IOleObject.SetClientSite(UnsafeNativeMethods.IOleClientSite pClientSite)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetClientSite");
            ActiveXInstance.SetClientSite(pClientSite);
            return NativeMethods.S_OK;
        }

        UnsafeNativeMethods.IOleClientSite UnsafeNativeMethods.IOleObject.GetClientSite()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetClientSite");
            return ActiveXInstance.GetClientSite();
        }

        int UnsafeNativeMethods.IOleObject.SetHostNames(string szContainerApp, string szContainerObj)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetHostNames");
            // Since ActiveX controls never "open" for editing, we shouldn't need
            // to store these.
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.Close(int dwSaveOption)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Close. Save option: " + dwSaveOption);
            ActiveXInstance.Close(dwSaveOption);
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.SetMoniker(int dwWhichMoniker, object pmk)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetMoniker");
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleObject.GetMoniker(int dwAssign, int dwWhichMoniker, out object moniker)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMoniker");
            moniker = null;
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleObject.InitFromData(IComDataObject pDataObject, int fCreation, int dwReserved)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:InitFromData");
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleObject.GetClipboardData(int dwReserved, out IComDataObject data)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetClipboardData");
            data = null;
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleObject.DoVerb(int iVerb, IntPtr lpmsg, UnsafeNativeMethods.IOleClientSite pActiveSite, int lindex, IntPtr hwndParent, NativeMethods.COMRECT lprcPosRect)
        {
            // In Office they are internally casting an iverb to a short and not
            // doing the proper sign extension.  So, we do it here.
            //
            short sVerb = unchecked((short)iVerb);
            iVerb = (int)sVerb;

#if DEBUG
            if (CompModSwitches.ActiveX.TraceInfo)
            {
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
            try
            {
                ActiveXInstance.DoVerb(iVerb, lpmsg, pActiveSite, lindex, hwndParent, lprcPosRect);
            }
            catch (Exception e)
            {
                Debug.Fail("Exception occurred during DoVerb(" + iVerb + ") " + e.ToString());
                throw;
            }
            finally
            {
                Debug.Unindent();
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "}");
            }
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.EnumVerbs(out UnsafeNativeMethods.IEnumOLEVERB e)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:EnumVerbs");
            return ActiveXImpl.EnumVerbs(out e);
        }

        int UnsafeNativeMethods.IOleObject.OleUpdate()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:OleUpdate");
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.IsUpToDate()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IsUpToDate");
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.GetUserClassID(ref Guid pClsid)
        {
            pClsid = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetUserClassID.  ClassID: " + pClsid.ToString());
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.GetUserType(int dwFormOfType, out string userType)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetUserType");
            if (dwFormOfType == NativeMethods.USERCLASSTYPE_FULL)
            {
                userType = GetType().FullName;
            }
            else
            {
                userType = GetType().Name;
            }
            return NativeMethods.S_OK;
        }

        unsafe Interop.HRESULT UnsafeNativeMethods.IOleObject.SetExtent(uint dwDrawAspect, Size* pSizel)
        {
            if (pSizel == null)
            {
                return Interop.HRESULT.E_INVALIDARG;
            }

            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetExtent(" + pSizel->Width + ", " + pSizel->Height + ")");
            Debug.Indent();
            ActiveXInstance.SetExtent(dwDrawAspect, pSizel);
            Debug.Unindent();
            return Interop.HRESULT.S_OK;
        }

        unsafe Interop.HRESULT UnsafeNativeMethods.IOleObject.GetExtent(uint dwDrawAspect, Size* pSizel)
        {
            if (pSizel == null)
            {
                return Interop.HRESULT.E_INVALIDARG;
            }

            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetExtent.  Aspect: " + dwDrawAspect.ToString(CultureInfo.InvariantCulture));
            Debug.Indent();
            ActiveXInstance.GetExtent(dwDrawAspect, pSizel);
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "value: " + pSizel->Width + ", " + pSizel->Height);
            Debug.Unindent();
            return Interop.HRESULT.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.Advise(IAdviseSink pAdvSink, out int cookie)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Advise");
            cookie = ActiveXInstance.Advise(pAdvSink);
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.Unadvise(int dwConnection)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Unadvise");
            Debug.Indent();
            ActiveXInstance.Unadvise(dwConnection);
            Debug.Unindent();
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.EnumAdvise(out IEnumSTATDATA e)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:EnumAdvise");
            e = null;
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IOleObject.GetMiscStatus(int dwAspect, out int cookie)
        {
            if ((dwAspect & NativeMethods.DVASPECT_CONTENT) != 0)
            {
                int status = NativeMethods.OLEMISC_ACTIVATEWHENVISIBLE | NativeMethods.OLEMISC_INSIDEOUT | NativeMethods.OLEMISC_SETCLIENTSITEFIRST;

                if (GetStyle(ControlStyles.ResizeRedraw))
                {
                    status |= NativeMethods.OLEMISC_RECOMPOSEONRESIZE;
                }

                if (this is IButtonControl)
                {
                    status |= NativeMethods.OLEMISC_ACTSLIKEBUTTON;
                }

                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMiscStatus. Status: " + status.ToString(CultureInfo.InvariantCulture));
                cookie = status;
            }
            else
            {
                Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetMiscStatus.  Status: ERROR, wrong aspect.");
                cookie = 0;
                return NativeMethods.DV_E_DVASPECT;
            }
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleObject.SetColorScheme(NativeMethods.tagLOGPALETTE pLogpal)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetColorScheme");
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd)
        {
            return ((UnsafeNativeMethods.IOleInPlaceObject)this).GetWindow(out hwnd);
        }

        void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode)
        {
            ((UnsafeNativeMethods.IOleInPlaceObject)this).ContextSensitiveHelp(fEnterMode);
        }

        void UnsafeNativeMethods.IPersist.GetClassID(out Guid pClassID)
        {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersist.GetClassID.  ClassID: " + pClassID.ToString());
        }

        void UnsafeNativeMethods.IPersistPropertyBag.InitNew()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistPropertyBag.InitNew");
        }

        void UnsafeNativeMethods.IPersistPropertyBag.GetClassID(out Guid pClassID)
        {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistPropertyBag.GetClassID.  ClassID: " + pClassID.ToString());
        }

        void UnsafeNativeMethods.IPersistPropertyBag.Load(UnsafeNativeMethods.IPropertyBag pPropBag, UnsafeNativeMethods.IErrorLog pErrorLog)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Load (IPersistPropertyBag)");
            Debug.Indent();
            ActiveXInstance.Load(pPropBag, pErrorLog);
            Debug.Unindent();
        }

        void UnsafeNativeMethods.IPersistPropertyBag.Save(UnsafeNativeMethods.IPropertyBag pPropBag, BOOL fClearDirty, BOOL fSaveAllProperties)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Save (IPersistPropertyBag)");
            Debug.Indent();
            ActiveXInstance.Save(pPropBag, fClearDirty, fSaveAllProperties);
            Debug.Unindent();
        }

        void Ole32.IPersistStorage.GetClassID(out Guid pClassID)
        {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.GetClassID.  ClassID: " + pClassID.ToString());
        }

        HRESULT Ole32.IPersistStorage.IsDirty()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.IsDirty");
            return ActiveXInstance.IsDirty();
        }

        void Ole32.IPersistStorage.InitNew(Ole32.IStorage pstg)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.InitNew");
        }

        HRESULT Ole32.IPersistStorage.Load(Ole32.IStorage pstg)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.Load");
            Debug.Indent();
            ActiveXInstance.Load(pstg);
            Debug.Unindent();
            return HRESULT.S_OK;
        }

        void Ole32.IPersistStorage.Save(Ole32.IStorage pstg, BOOL fSameAsLoad)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.Save");
            Debug.Indent();
            ActiveXInstance.Save(pstg, fSameAsLoad);
            Debug.Unindent();
        }

        void Ole32.IPersistStorage.SaveCompleted(Ole32.IStorage pStgNew)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.SaveCompleted");
        }

        void Ole32.IPersistStorage.HandsOffStorage()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStorage.HandsOffStorage");
        }

        void Ole32.IPersistStreamInit.GetClassID(out Guid pClassID)
        {
            pClassID = GetType().GUID;
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.GetClassID.  ClassID: " + pClassID.ToString());
        }

        HRESULT Ole32.IPersistStreamInit.IsDirty()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.IsDirty");
            return ActiveXInstance.IsDirty();
        }

        void Ole32.IPersistStreamInit.Load(Ole32.IStream pstm)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.Load");
            Debug.Indent();
            ActiveXInstance.Load(pstm);
            Debug.Unindent();
        }

        void Ole32.IPersistStreamInit.Save(Ole32.IStream pstm, BOOL fClearDirty)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.Save");
            Debug.Indent();
            ActiveXInstance.Save(pstm, fClearDirty);
            Debug.Unindent();
        }

        unsafe void Ole32.IPersistStreamInit.GetSizeMax(ulong* pcbSize)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetSizeMax");
        }

        void Ole32.IPersistStreamInit.InitNew()
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:IPersistStreamInit.InitNew");
        }

        void UnsafeNativeMethods.IQuickActivate.QuickActivate(UnsafeNativeMethods.tagQACONTAINER pQaContainer, UnsafeNativeMethods.tagQACONTROL pQaControl)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:QuickActivate");
            Debug.Indent();
            ActiveXInstance.QuickActivate(pQaContainer, pQaControl);
            Debug.Unindent();
        }

        unsafe Interop.HRESULT UnsafeNativeMethods.IQuickActivate.SetContentExtent(Size* pSizel)
        {
            if (pSizel == null)
            {
                return Interop.HRESULT.E_INVALIDARG;
            }

            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetContentExtent");
            Debug.Indent();
            ActiveXInstance.SetExtent(NativeMethods.DVASPECT_CONTENT, pSizel);
            Debug.Unindent();
            return Interop.HRESULT.S_OK;
        }

        unsafe Interop.HRESULT UnsafeNativeMethods.IQuickActivate.GetContentExtent(Size* pSizel)
        {
            if (pSizel == null)
            {
                return Interop.HRESULT.E_INVALIDARG;
            }

            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetContentExtent");
            Debug.Indent();
            ActiveXInstance.GetExtent(NativeMethods.DVASPECT_CONTENT, pSizel);
            Debug.Unindent();
            return Interop.HRESULT.S_OK;
        }

        int UnsafeNativeMethods.IViewObject.Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                            IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds,
                                            IntPtr pfnContinue, int dwContinue)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Draw");

            Debug.Indent();
            try
            {
                ActiveXInstance.Draw(dwDrawAspect, lindex, pvAspect, ptd, hdcTargetDev,
                                 hdcDraw, lprcBounds, lprcWBounds, pfnContinue, dwContinue);

            }
            catch (ExternalException ex)
            {
                return ex.ErrorCode;
            }
            finally
            {
                Debug.Unindent();
            }
            return NativeMethods.S_OK;
        }

        int UnsafeNativeMethods.IViewObject.GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                                   IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetColorSet");

            // GDI+ doesn't do palettes.
            //
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IViewObject.Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Freezes");
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IViewObject.Unfreeze(int dwFreeze)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Unfreeze");
            return NativeMethods.E_NOTIMPL;
        }

        void UnsafeNativeMethods.IViewObject.SetAdvise(int aspects, int advf, IAdviseSink pAdvSink)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetAdvise");
            ActiveXInstance.SetAdvise(aspects, advf, pAdvSink);
        }

        void UnsafeNativeMethods.IViewObject.GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAdvise");
            ActiveXInstance.GetAdvise(paspects, padvf, pAdvSink);
        }

        void UnsafeNativeMethods.IViewObject2.Draw(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                             IntPtr hdcTargetDev, IntPtr hdcDraw, NativeMethods.COMRECT lprcBounds, NativeMethods.COMRECT lprcWBounds,
                                             IntPtr pfnContinue, int dwContinue)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Draw");
            Debug.Indent();
            ActiveXInstance.Draw(dwDrawAspect, lindex, pvAspect, ptd, hdcTargetDev,
                                 hdcDraw, lprcBounds, lprcWBounds, pfnContinue, dwContinue);
            Debug.Unindent();
        }

        int UnsafeNativeMethods.IViewObject2.GetColorSet(int dwDrawAspect, int lindex, IntPtr pvAspect, NativeMethods.tagDVTARGETDEVICE ptd,
                                                    IntPtr hicTargetDev, NativeMethods.tagLOGPALETTE ppColorSet)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetColorSet");

            // GDI+ doesn't do palettes.
            //
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IViewObject2.Freeze(int dwDrawAspect, int lindex, IntPtr pvAspect, IntPtr pdwFreeze)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Freezes");
            return NativeMethods.E_NOTIMPL;
        }

        int UnsafeNativeMethods.IViewObject2.Unfreeze(int dwFreeze)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:Unfreeze");
            return NativeMethods.E_NOTIMPL;
        }

        void UnsafeNativeMethods.IViewObject2.SetAdvise(int aspects, int advf, IAdviseSink pAdvSink)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:SetAdvise");
            ActiveXInstance.SetAdvise(aspects, advf, pAdvSink);
        }

        void UnsafeNativeMethods.IViewObject2.GetAdvise(int[] paspects, int[] padvf, IAdviseSink[] pAdvSink)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetAdvise");
            ActiveXInstance.GetAdvise(paspects, padvf, pAdvSink);
        }

        unsafe Interop.HRESULT UnsafeNativeMethods.IViewObject2.GetExtent(uint dwDrawAspect, int lindex, NativeMethods.tagDVTARGETDEVICE ptd, Size* lpsizel)
        {
            Debug.WriteLineIf(CompModSwitches.ActiveX.TraceInfo, "AxSource:GetExtent (IViewObject2)");
            // we already have an implementation of this [from IOleObject]
            return ((UnsafeNativeMethods.IOleObject)this).GetExtent(dwDrawAspect, lpsizel);
        }

        #region IKeyboardToolTip implementation

        bool IKeyboardToolTip.CanShowToolTipsNow()
        {
            IKeyboardToolTip host = ToolStripControlHost;
            return IsHandleCreated && Visible && (host == null || host.CanShowToolTipsNow());
        }

        Rectangle IKeyboardToolTip.GetNativeScreenRectangle()
        {
            return GetToolNativeScreenRectangle();
        }

        IList<Rectangle> IKeyboardToolTip.GetNeighboringToolsRectangles()
        {
            IKeyboardToolTip host = ToolStripControlHost;
            if (host == null)
            {
                return GetOwnNeighboringToolsRectangles();
            }
            else
            {
                return host.GetNeighboringToolsRectangles();
            }
        }

        bool IKeyboardToolTip.IsHoveredWithMouse()
        {
            return ClientRectangle.Contains(PointToClient(MousePosition));
        }

        bool IKeyboardToolTip.HasRtlModeEnabled()
        {
            Control topLevelControl = TopLevelControlInternal;
            return topLevelControl != null && topLevelControl.RightToLeft == RightToLeft.Yes && !IsMirrored;
        }

        bool IKeyboardToolTip.AllowsToolTip()
        {
            IKeyboardToolTip host = ToolStripControlHost;
            return (host == null || host.AllowsToolTip()) && AllowsKeyboardToolTip();
        }

        IWin32Window IKeyboardToolTip.GetOwnerWindow()
        {
            return this;
        }

        void IKeyboardToolTip.OnHooked(ToolTip toolTip)
        {
            OnKeyboardToolTipHook(toolTip);
        }

        void IKeyboardToolTip.OnUnhooked(ToolTip toolTip)
        {
            OnKeyboardToolTipUnhook(toolTip);
        }

        string IKeyboardToolTip.GetCaptionForTool(ToolTip toolTip)
        {
            IKeyboardToolTip host = ToolStripControlHost;
            if (host == null)
            {
                return toolTip.GetCaptionForTool(this);
            }
            else
            {
                return host.GetCaptionForTool(toolTip);
            }
        }

        bool IKeyboardToolTip.ShowsOwnToolTip()
        {
            IKeyboardToolTip host = ToolStripControlHost;
            return (host == null || host.ShowsOwnToolTip()) && ShowsOwnKeyboardToolTip();
        }

        bool IKeyboardToolTip.IsBeingTabbedTo()
        {
            return AreCommonNavigationalKeysDown();
        }

        bool IKeyboardToolTip.AllowsChildrenToShowToolTips()
        {
            return AllowsChildrenToShowToolTips();
        }

        #endregion

        private IList<Rectangle> GetOwnNeighboringToolsRectangles()
        {
            Control controlParent = ParentInternal;
            if (controlParent != null)
            {
                Control[] neighboringControls = new Control[4] {
                    // Next and previous control which are accessible with Tab and Shift+Tab
                    controlParent.GetNextSelectableControl(this, true, true, true, false),
                    controlParent.GetNextSelectableControl(this, false, true, true, false),
                    // Next and previous control which are accessible with arrow keys
                    controlParent.GetNextSelectableControl(this, true, false, false, true),
                    controlParent.GetNextSelectableControl(this, false, false, false, true)
                };

                List<Rectangle> neighboringControlsRectangles = new List<Rectangle>(4);
                foreach (Control neighboringControl in neighboringControls)
                {
                    if (neighboringControl != null && neighboringControl.IsHandleCreated)
                    {
                        neighboringControlsRectangles.Add(((IKeyboardToolTip)neighboringControl).GetNativeScreenRectangle());
                    }
                }

                return neighboringControlsRectangles;
            }
            else
            {
                return Array.Empty<Rectangle>();
            }
        }

        internal virtual bool ShowsOwnKeyboardToolTip()
        {
            return true;
        }

        internal virtual void OnKeyboardToolTipHook(ToolTip toolTip)
        {
        }

        internal virtual void OnKeyboardToolTipUnhook(ToolTip toolTip)
        {
        }

        internal virtual Rectangle GetToolNativeScreenRectangle()
        {
            RECT rectangle = new RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref rectangle);
            return Rectangle.FromLTRB(rectangle.left, rectangle.top, rectangle.right, rectangle.bottom);
        }

        internal virtual bool AllowsKeyboardToolTip()
        {
            // This internal method enables keyboard ToolTips for all controls including the foreign descendants of Control unless this method is overridden in a child class belonging to this assembly.
            // ElementHost is one such control which is located in a different assembly.
            // This control doesn't show a mouse ToolTip when hovered and thus should not have a keyboard ToolTip as well.
            // We are not going to fix it now since it seems unlikely that someone would set ToolTip on such special container control as ElementHost.
            return true;
        }

        private static bool IsKeyDown(Keys key)
        {
            return (t_tempKeyboardStateArray[(int)key] & HighOrderBitMask) != 0;
        }

        internal static bool AreCommonNavigationalKeysDown()
        {
            if (t_tempKeyboardStateArray == null)
            {
                t_tempKeyboardStateArray = new byte[256];
            }
            UnsafeNativeMethods.GetKeyboardState(t_tempKeyboardStateArray);
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

        internal ToolStripControlHost ToolStripControlHost
        {
            get
            {
                toolStripControlHostReference.TryGetTarget(out ToolStripControlHost value);
                return value;
            }
            set
            {
                toolStripControlHostReference.SetTarget(value);
            }
        }

        internal virtual bool AllowsChildrenToShowToolTips()
        {
            return true;
        }
    }
}
