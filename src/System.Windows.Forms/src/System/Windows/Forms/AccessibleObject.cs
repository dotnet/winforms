// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using Accessibility;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides an implementation for an object that can be inspected by an
    ///  accessibility application.
    /// </summary>
    public partial class AccessibleObject :
        StandardOleMarshalObject,
        IReflect,
        IAccessible,
        UiaCore.IAccessibleEx,
        Ole32.IServiceProvider,
        UiaCore.IRawElementProviderSimple,
        UiaCore.IRawElementProviderFragment,
        UiaCore.IRawElementProviderFragmentRoot,
        UiaCore.IInvokeProvider,
        UiaCore.IValueProvider,
        UiaCore.IRangeValueProvider,
        UiaCore.IExpandCollapseProvider,
        UiaCore.IToggleProvider,
        UiaCore.ITableProvider,
        UiaCore.ITableItemProvider,
        UiaCore.IGridProvider,
        UiaCore.IGridItemProvider,
        Oleaut32.IEnumVariant,
        Ole32.IOleWindow,
        UiaCore.ILegacyIAccessibleProvider,
        UiaCore.ISelectionProvider,
        UiaCore.ISelectionItemProvider,
        UiaCore.IRawElementProviderHwndOverride,
        UiaCore.IScrollItemProvider,
        UiaCore.IMultipleViewProvider,
        UiaCore.ITextProvider,
        UiaCore.ITextProvider2
    {
        /// <summary>
        ///  Specifies the <see cref="IAccessible"/> interface used by this <see cref="AccessibleObject"/>.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   This is also set by <see cref="UseStdAccessibleObjects(IntPtr, int)"/>.
        ///  </para>
        /// </remarks>
        private SystemIAccessibleWrapper _systemIAccessible = new(
            null /* Prevents throwing exception when call to null-value system IAccessible */);

        /// <summary>
        ///  Specifies the <see cref="Oleaut32.IEnumVariant"/> used by this
        /// <see cref="AccessibleObject"/> .
        /// </summary>
        private Oleaut32.IEnumVariant? _systemIEnumVariant;
        private Oleaut32.IEnumVariant? _enumVariant;

        // IOleWindow interface of the 'inner' system IAccessible object that we are wrapping
        private Ole32.IOleWindow? _systemIOleWindow;

        // Indicates this object is being used ONLY to wrap a system IAccessible
        private readonly bool _systemWrapper;

        // The support for the UIA Notification event begins in RS3.
        // Assume the UIA Notification event is available until we learn otherwise.
        // If we learn that the UIA Notification event is not available,
        // controls should not attempt to raise it.
        private static bool s_notificationEventAvailable = true;
        private static bool? s_canNotifyClients;

        internal const int InvalidIndex = -1;

        internal const int RuntimeIDFirstItem = 0x2a;

        public AccessibleObject()
        {
        }

        // This constructor is used ONLY for wrapping system IAccessible objects
        private AccessibleObject(IAccessible? accessible)
        {
            _systemIAccessible = new SystemIAccessibleWrapper(accessible);
            _systemWrapper = true;
        }

        private protected virtual string? AutomationId => null;

        /// <summary>
        ///  Gets the bounds of the accessible object, in screen coordinates.
        /// </summary>
        public virtual Rectangle Bounds
        {
            get
            {
                // Use the system provided bounds
                _systemIAccessible.accLocation(out int left, out int top, out int width, out int height, NativeMethods.CHILDID_SELF);
                return new Rectangle(left, top, width, height);
            }
        }

        internal static bool CanNotifyClients => s_canNotifyClients ??= InitializeCanNotifyClients();

        private static bool InitializeCanNotifyClients()
        {
            // While handling accessibility events, accessibility clients (JAWS, Inspect),
            // can access AccessibleObject associated with the event. In the designer scenario, controls are not
            // receiving messages directly and might not respond to messages while in the notification call.
            // This will make the server process unresponsive and will cause VisualStudio to become unresponsive.
            //
            // The following compat switch is set in the designer server process to prevent controls from sending notification.
            if (AppContext.TryGetSwitch("Switch.System.Windows.Forms.AccessibleObject.NoClientNotifications", out bool isEnabled))
            {
                return !isEnabled;
            }

            return true;
        }

        /// <summary>
        ///  Gets a description of the default action for an object.
        /// </summary>
        public virtual string? DefaultAction => _systemIAccessible.get_accDefaultAction(NativeMethods.CHILDID_SELF);

        /// <summary>
        ///  Gets a description of the object's visual appearance to the user.
        /// </summary>
        public virtual string? Description => _systemIAccessible.get_accDescription(NativeMethods.CHILDID_SELF);

        private Oleaut32.IEnumVariant EnumVariant => _enumVariant ??= new EnumVariantObject(this);

        /// <summary>
        ///  Gets a description of what the object does or how the object is used.
        /// </summary>
        public virtual string? Help => _systemIAccessible.get_accHelp(NativeMethods.CHILDID_SELF);

        /// <summary>
        ///  Gets the object shortcut key or access key for an accessible object.
        /// </summary>
        public virtual string? KeyboardShortcut => _systemIAccessible.get_accKeyboardShortcut(NativeMethods.CHILDID_SELF);

        /// <summary>
        ///  Gets or sets the object name.
        /// </summary>
        public virtual string? Name
        {
            get => _systemIAccessible.get_accName(NativeMethods.CHILDID_SELF);
            set => _systemIAccessible.set_accName(NativeMethods.CHILDID_SELF, value);
        }

        /// <summary>
        ///  When overridden in a derived class, gets or sets the parent of an accessible object.
        /// </summary>
        /// <devdoc>
        ///  Note that the default behavior for <see cref="Control"/> is that it calls base from its override in
        ///  <see cref="Control.ControlAccessibleObject"/>. <see cref="Control.ControlAccessibleObject"/> always
        ///  creates the Win32 standard accessible objects so it will hit the Windows implementation of
        ///  <see cref="IAccessible.accParent"/>.
        ///
        ///  For the non-client area (OBJID_WINDOW), the Windows accParent implementation simply calls
        ///  GetAncestor(GA_PARENT) to find the window it will call WM_GETOBJECT on with OBJID_CLIENT.
        ///
        ///  For the client area (OBJID_CLIENT), the Windows accParent implementation calls WM_GETOBJECT directly
        ///  with OBJID_WINDOW.
        ///
        ///  What this means, effectively, is that the non-client area is the parent of the client area, and the parent
        ///  window's client area is the parent of the non-client area of the current window (at least from an
        ///  accessiblity object standpoint).
        /// </devdoc>
        public virtual AccessibleObject? Parent => WrapIAccessible(_systemIAccessible.accParent);

        /// <summary>
        ///  Gets the role of this accessible object.
        /// </summary>
        public virtual AccessibleRole Role
        {
            get
            {
                var accRole = _systemIAccessible.get_accRole(NativeMethods.CHILDID_SELF);
                return accRole is not null
                    ? (AccessibleRole)accRole
                    : AccessibleRole.None;
            }
        }

        /// <summary>
        ///  Gets the state of this accessible object.
        /// </summary>
        public virtual AccessibleStates State
        {
            get
            {
                var accState = _systemIAccessible.get_accState(NativeMethods.CHILDID_SELF);
                return accState is not null
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;
            }
        }

        /// <summary>
        ///  Gets or sets the value of an accessible object.
        /// </summary>
        public virtual string? Value
        {
            get
            {
                if (_systemIAccessible.IsIAccessibleCreated)
                {
                    return _systemIAccessible.get_accValue(NativeMethods.CHILDID_SELF);
                }

                return string.Empty;
            }
            set => _systemIAccessible.set_accValue(NativeMethods.CHILDID_SELF, value);
        }

        /// <summary>
        ///  When overridden in a derived class, gets the accessible child
        ///  corresponding to the specified index.
        /// </summary>
        public virtual AccessibleObject? GetChild(int index) => null;

        internal virtual int GetChildIndex(AccessibleObject? child) => InvalidIndex;

        /// <summary>
        ///  When overridden in a derived class, gets the number of children
        ///  belonging to an accessible object.
        /// </summary>
        public virtual int GetChildCount() => -1;

        /// <summary>
        ///  Mechanism for overriding default IEnumVariant behavior of the 'inner'
        ///  system accessible object (IEnumVariant is how a system accessible
        ///  object exposes its ordered list of child objects).
        ///
        ///  USAGE: Overridden method in derived class should return array of
        ///  integers representing new order to be imposed on the child accessible
        ///  object collection returned by the system (which we assume will be a
        ///  set of accessible objects that represent the child windows, in z-order).
        ///  Each array element contains the original z-order based rank of the
        ///  child window that is to appear at that position in the new ordering.
        ///  Note: This array could also be used to filter out unwanted child
        ///  windows too, if necessary (not recommended).
        /// </summary>
        internal virtual int[]? GetSysChildOrder() => null;

        /// <summary>
        ///  Mechanism for overriding default IAccessible.accNavigate behavior of
        ///  the 'inner' system accessible object (accNavigate is how you move
        ///  between parent, child and sibling accessible objects).
        ///
        ///  USAGE: 'navdir' indicates navigation operation to perform, relative to
        ///  this accessible object.
        ///  If operation is unsupported, return false to allow fall-back to default
        ///  system behavior. Otherwise return destination object in the out
        ///  parameter, or null to indicate 'off end of list'.
        /// </summary>
        internal virtual bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject? accessibleObject)
        {
            accessibleObject = null;
            return false;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the object that has the
        ///  keyboard focus.
        /// </summary>
        public virtual AccessibleObject? GetFocused()
        {
            // Default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject? child = GetChild(index);
                    Debug.Assert(child is not null, $"GetChild({index}) returned null!");
                    if (child is not null && ((child.State & AccessibleStates.Focused) != 0))
                    {
                        return child;
                    }
                }

                if ((State & AccessibleStates.Focused) != 0)
                {
                    return this;
                }

                return null;
            }

            return WrapIAccessible(_systemIAccessible.accFocus);
        }

        /// <summary>
        ///  Gets an identifier for a Help topic and the path to the Help file
        ///  associated with this accessible object.
        /// </summary>
        public virtual int GetHelpTopic(out string? fileName) =>
            _systemIAccessible.get_accHelpTopic(out fileName, NativeMethods.CHILDID_SELF);

        /// <summary>
        ///  When overridden in a derived class, gets the currently selected child.
        /// </summary>
        public virtual AccessibleObject? GetSelected()
        {
            // Default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject? child = GetChild(index);
                    Debug.Assert(child is not null, $"GetChild({index}) returned null!");
                    if (child is not null && ((child.State & AccessibleStates.Selected) != 0))
                    {
                        return child;
                    }
                }

                if ((State & AccessibleStates.Selected) != 0)
                {
                    return this;
                }

                return null;
            }

            return WrapIAccessible(_systemIAccessible.accSelection);
        }

        /// <summary>
        ///  Return the child object at the given screen coordinates.
        /// </summary>
        public virtual AccessibleObject? HitTest(int x, int y)
        {
            // Default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject? child = GetChild(index);
                    Debug.Assert(child is not null, $"GetChild({index}) returned null!");
                    if (child is not null && child.Bounds.Contains(x, y))
                    {
                        return child;
                    }
                }

                return this;
            }

            if (_systemIAccessible.IsIAccessibleCreated)
            {
                return WrapIAccessible(_systemIAccessible.accHitTest(x, y));
            }

            if (Bounds.Contains(x, y))
            {
                return this;
            }

            return null;
        }

        internal virtual bool IsIAccessibleExSupported()
        {
            // Override this, in your derived class, to enable IAccessibleEx support
            return false;
        }

        internal virtual bool IsPatternSupported(UiaCore.UIA patternId)
        {
            // Override this, in your derived class, if you implement UIAutomation patterns
            if (patternId == UiaCore.UIA.InvokePatternId)
            {
                return IsInvokePatternAvailable;
            }

            return false;
        }

        internal virtual int[] RuntimeId
        {
            get
            {
                if (_systemWrapper)
                {
                    return new int[] { RuntimeIDFirstItem, GetHashCode() };
                }

                string message = string.Format(SR.AccessibleObjectRuntimeIdNotSupported, nameof(AccessibleObject), nameof(RuntimeId));

                Debug.Fail(message);

                throw new NotSupportedException(message);
            }
        }

        internal virtual int ProviderOptions
            => (int)(UiaCore.ProviderOptions.ServerSideProvider | UiaCore.ProviderOptions.UseComThreading);

        internal virtual UiaCore.IRawElementProviderSimple? HostRawElementProvider => null;

        /// <summary>
        ///  Returns the value of the specified <paramref name="propertyID"/> from the element.
        /// </summary>
        /// <param name="propertyID">Identifier indicating the property to return.</param>
        /// <returns>The requested value if supported or null if it is not.</returns>
        internal virtual object? GetPropertyValue(UiaCore.UIA propertyID) =>
            propertyID switch
            {
                UiaCore.UIA.IsInvokePatternAvailablePropertyId => IsInvokePatternAvailable,
                UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                UiaCore.UIA.NamePropertyId => Name,
                UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                UiaCore.UIA.IsExpandCollapsePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ExpandCollapsePatternId),
                UiaCore.UIA.IsGridItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.GridItemPatternId),
                UiaCore.UIA.IsGridPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.GridPatternId),
                UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId),
                UiaCore.UIA.IsMultipleViewPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.MultipleViewPatternId),
                UiaCore.UIA.IsScrollItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ScrollItemPatternId),
                UiaCore.UIA.IsScrollPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ScrollPatternId),
                UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.SelectionItemPatternId),
                UiaCore.UIA.IsSelectionPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.SelectionPatternId),
                UiaCore.UIA.IsTableItemPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TableItemPatternId),
                UiaCore.UIA.IsTablePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TablePatternId),
                UiaCore.UIA.IsTextPattern2AvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPattern2Id),
                UiaCore.UIA.IsTextPatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TextPatternId),
                UiaCore.UIA.IsTogglePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.TogglePatternId),
                UiaCore.UIA.IsValuePatternAvailablePropertyId => IsPatternSupported(UiaCore.UIA.ValuePatternId),
                UiaCore.UIA.HelpTextPropertyId => Help ?? string.Empty,
                UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                UiaCore.UIA.LegacyIAccessibleDefaultActionPropertyId => !string.IsNullOrEmpty(DefaultAction) ? DefaultAction : null,
                UiaCore.UIA.LegacyIAccessibleNamePropertyId => !string.IsNullOrEmpty(Name) ? Name : null,
                UiaCore.UIA.ValueValuePropertyId => !string.IsNullOrEmpty(Value) ? Value : null,
                UiaCore.UIA.IsPasswordPropertyId => false,
                UiaCore.UIA.FrameworkIdPropertyId => NativeMethods.WinFormFrameworkId,
                UiaCore.UIA.AccessKeyPropertyId => KeyboardShortcut ?? string.Empty,
                UiaCore.UIA.AutomationIdPropertyId => AutomationId,
                _ => null
            };

        private bool IsInvokePatternAvailable
        {
            get
            {
                // MSAA Proxy determines the availability of invoke pattern based
                // on Role/DefaultAction properties.
                // Below code emulates the same rules.
                switch (Role)
                {
                    case AccessibleRole.MenuItem:
                    case AccessibleRole.Link:
                    case AccessibleRole.PushButton:
                    case AccessibleRole.ButtonDropDown:
                    case AccessibleRole.ButtonMenu:
                    case AccessibleRole.ButtonDropDownGrid:
                    case AccessibleRole.Clock:
                    case AccessibleRole.SplitButton:
                    case AccessibleRole.CheckButton:
                    case AccessibleRole.Cell:
                    case AccessibleRole.ListItem:
                        return true;

                    case AccessibleRole.Default:
                    case AccessibleRole.None:
                    case AccessibleRole.Sound:
                    case AccessibleRole.Cursor:
                    case AccessibleRole.Caret:
                    case AccessibleRole.Alert:
                    case AccessibleRole.Client:
                    case AccessibleRole.Chart:
                    case AccessibleRole.Dialog:
                    case AccessibleRole.Border:
                    case AccessibleRole.Column:
                    case AccessibleRole.Row:
                    case AccessibleRole.HelpBalloon:
                    case AccessibleRole.Character:
                    case AccessibleRole.PageTab:
                    case AccessibleRole.PropertyPage:
                    case AccessibleRole.DropList:
                    case AccessibleRole.Dial:
                    case AccessibleRole.HotkeyField:
                    case AccessibleRole.Diagram:
                    case AccessibleRole.Animation:
                    case AccessibleRole.Equation:
                    case AccessibleRole.WhiteSpace:
                    case AccessibleRole.IpAddress:
                    case AccessibleRole.OutlineButton:
                        return false;

                    default:
                        return !string.IsNullOrEmpty(DefaultAction);
                }
            }
        }

        internal virtual int GetChildId() => NativeMethods.CHILDID_SELF;

        /// <summary>
        ///  Returns the element in the specified <paramref name="direction"/>.
        /// </summary>
        /// <param name="direction">Indicates the direction in which to navigate.</param>
        /// <returns>The element in the specified direction if it exists.</returns>
        internal virtual UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction) => null;

        internal virtual UiaCore.IRawElementProviderSimple[]? GetEmbeddedFragmentRoots() => null;

        internal virtual void SetFocus()
        {
        }

        internal virtual Rectangle BoundingRectangle => Bounds;

        internal virtual UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => null;

        internal virtual UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y) => this;

        internal virtual UiaCore.IRawElementProviderFragment? GetFocus() => null;

        internal virtual void Expand()
        {
        }

        internal virtual void Collapse()
        {
        }

        internal virtual UiaCore.ExpandCollapseState ExpandCollapseState => UiaCore.ExpandCollapseState.Collapsed;

        internal virtual void Toggle()
        {
        }

        internal virtual UiaCore.ToggleState ToggleState => UiaCore.ToggleState.Indeterminate;

        private protected virtual UiaCore.IRawElementProviderFragmentRoot? ToolStripFragmentRoot => null;

        internal virtual UiaCore.IRawElementProviderSimple[]? GetRowHeaders() => null;

        internal virtual UiaCore.IRawElementProviderSimple[]? GetColumnHeaders() => null;

        internal virtual UiaCore.RowOrColumnMajor RowOrColumnMajor => UiaCore.RowOrColumnMajor.RowMajor;

        internal virtual UiaCore.IRawElementProviderSimple[]? GetRowHeaderItems() => null;

        internal virtual UiaCore.IRawElementProviderSimple[]? GetColumnHeaderItems() => null;

        internal virtual UiaCore.IRawElementProviderSimple? GetItem(int row, int column) => null;

        internal virtual int RowCount => -1;

        internal virtual int ColumnCount => -1;

        internal virtual int Row => -1;

        internal virtual int Column => -1;

        internal virtual int RowSpan => 1;

        internal virtual int ColumnSpan => 1;

        internal virtual UiaCore.IRawElementProviderSimple? ContainingGrid => null;

        internal virtual void Invoke() => DoDefaultAction();

        internal virtual UiaCore.ITextRangeProvider? DocumentRangeInternal
        {
            get
            {
                Debug.Fail("Not implemented. DocumentRangeInternal property should be overridden.");
                return null;
            }
        }

        internal virtual UiaCore.ITextRangeProvider[]? GetTextSelection()
        {
            Debug.Fail("Not implemented. GetTextSelection method should be overridden.");
            return null;
        }

        internal virtual UiaCore.ITextRangeProvider[]? GetTextVisibleRanges()
        {
            Debug.Fail("Not implemented. GetTextVisibleRanges method should be overridden.");
            return null;
        }

        internal virtual UiaCore.ITextRangeProvider? GetTextRangeFromChild(UiaCore.IRawElementProviderSimple childElement)
        {
            Debug.Fail("Not implemented. GetTextRangeFromChild method should be overridden.");
            return null;
        }

        internal virtual UiaCore.ITextRangeProvider? GetTextRangeFromPoint(Point screenLocation)
        {
            Debug.Fail("Not implemented. GetTextRangeFromPoint method should be overridden.");
            return null;
        }

        internal virtual UiaCore.SupportedTextSelection SupportedTextSelectionInternal
        {
            get
            {
                Debug.Fail("Not implemented. SupportedTextSelectionInternal property should be overridden.");
                return UiaCore.SupportedTextSelection.None;
            }
        }

        internal virtual UiaCore.ITextRangeProvider? GetTextCaretRange(out BOOL isActive)
        {
            isActive = false;
            Debug.Fail("Not implemented. GetTextCaretRange method should be overridden.");
            return null;
        }

        internal virtual UiaCore.ITextRangeProvider? GetRangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement)
        {
            Debug.Fail("Not implemented. GetRangeFromAnnotation method should be overridden.");
            return null;
        }

        internal virtual bool IsReadOnly => false;

        internal virtual void SetValue(string? newValue)
        {
            Value = newValue;
        }

        internal virtual UiaCore.IRawElementProviderSimple? GetOverrideProviderForHwnd(IntPtr hwnd) => null;

        internal virtual int GetMultiViewProviderCurrentView() => 0;

        internal virtual int[]? GetMultiViewProviderSupportedViews() => Array.Empty<int>();

        internal virtual string GetMultiViewProviderViewName(int viewId) => string.Empty;

        internal virtual void SetMultiViewProviderCurrentView(int viewId)
        {
        }

        internal virtual void SetValue(double newValue)
        {
        }

        internal virtual double LargeChange => double.NaN;

        internal virtual double Maximum => double.NaN;

        internal virtual double Minimum => double.NaN;

        internal virtual double SmallChange => double.NaN;

        internal virtual double RangeValue => double.NaN;

        internal virtual UiaCore.IRawElementProviderSimple[]? GetSelection() => null;

        internal virtual bool CanSelectMultiple => false;

        internal virtual bool IsSelectionRequired => false;

        internal virtual void SelectItem()
        {
        }

        internal virtual void AddToSelection()
        {
        }

        internal virtual void RemoveFromSelection()
        {
        }

        internal virtual bool IsItemSelected => false;

        internal virtual UiaCore.IRawElementProviderSimple? ItemSelectionContainer => null;

        /// <summary>
        ///  Sets the parent accessible object for the node which can be added or removed to/from hierarchy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal virtual void SetParent(AccessibleObject? parent)
        {
        }

        /// <summary>
        ///  Sets the detachable child accessible object which may be added or removed to/from hierarchy nodes.
        /// </summary>
        /// <param name="child">The child accessible object.</param>
        internal virtual void SetDetachableChild(AccessibleObject? child)
        {
        }

        unsafe HRESULT Ole32.IServiceProvider.QueryService(Guid* service, Guid* riid, IntPtr* ppvObject)
        {
            if (service is null || riid is null)
            {
                return HRESULT.Values.E_NOINTERFACE;
            }

            if (ppvObject is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            if (IsIAccessibleExSupported())
            {
                Guid IID_IAccessibleEx = typeof(UiaCore.IAccessibleEx).GUID;
                if (service->Equals(IID_IAccessibleEx) && riid->Equals(IID_IAccessibleEx))
                {
                    // We want to return the internal, secure, object, which we don't have access here
                    // Return non-null, which will be interpreted in internal method, to mean returning casted object to IAccessibleEx
                    *ppvObject = Marshal.GetComInterfaceForObject(this, typeof(UiaCore.IAccessibleEx));
                    return HRESULT.Values.S_OK;
                }
            }

            return HRESULT.Values.E_NOINTERFACE;
        }

        UiaCore.IAccessibleEx? UiaCore.IAccessibleEx.GetObjectForChild(int idChild) => null;

        unsafe HRESULT UiaCore.IAccessibleEx.GetIAccessiblePair(out object? ppAcc, int* pidChild)
        {
            if (pidChild is null)
            {
                ppAcc = null;
                return HRESULT.Values.E_INVALIDARG;
            }

            ppAcc = this;
            *pidChild = NativeMethods.CHILDID_SELF;
            return HRESULT.Values.S_OK;
        }

        int[]? UiaCore.IAccessibleEx.GetRuntimeId() => RuntimeId;

        unsafe HRESULT UiaCore.IAccessibleEx.ConvertReturnedElement(UiaCore.IRawElementProviderSimple pIn, IntPtr* ppRetValOut)
        {
            if (ppRetValOut == null)
            {
                return HRESULT.Values.E_POINTER;
            }

            // No need to implement this for patterns and properties
            *ppRetValOut = IntPtr.Zero;
            return HRESULT.Values.E_NOTIMPL;
        }

        UiaCore.ProviderOptions UiaCore.IRawElementProviderSimple.ProviderOptions => (UiaCore.ProviderOptions)ProviderOptions;

        UiaCore.IRawElementProviderSimple? UiaCore.IRawElementProviderSimple.HostRawElementProvider => HostRawElementProvider;

        object? UiaCore.IRawElementProviderSimple.GetPatternProvider(UiaCore.UIA patternId)
        {
            if (IsPatternSupported(patternId))
            {
                return this;
            }

            return null;
        }

        object? UiaCore.IRawElementProviderSimple.GetPropertyValue(UiaCore.UIA propertyID) => GetPropertyValue(propertyID);

        object? UiaCore.IRawElementProviderFragment.Navigate(UiaCore.NavigateDirection direction) => FragmentNavigate(direction);

        int[]? UiaCore.IRawElementProviderFragment.GetRuntimeId() => RuntimeId;

        object[]? UiaCore.IRawElementProviderFragment.GetEmbeddedFragmentRoots() => GetEmbeddedFragmentRoots();

        void UiaCore.IRawElementProviderFragment.SetFocus() => SetFocus();

        UiaCore.UiaRect UiaCore.IRawElementProviderFragment.BoundingRectangle => new(BoundingRectangle);

        // An accessible object should provide info about its correct root object,
        // even its owner is used like a ToolStrip item via ToolStripControlHost.
        // This change was made here to not to rework FragmentRoot implementations
        // for all accessible object. Moreover, this change will work for new accessible object
        // classes, where it is enough to implement FragmentRoot for a common case.
        UiaCore.IRawElementProviderFragmentRoot? UiaCore.IRawElementProviderFragment.FragmentRoot
            => ToolStripFragmentRoot ?? FragmentRoot;

        object? UiaCore.IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y) => ElementProviderFromPoint(x, y);

        object? UiaCore.IRawElementProviderFragmentRoot.GetFocus() => GetFocus();

        string? UiaCore.ILegacyIAccessibleProvider.DefaultAction => DefaultAction;

        string? UiaCore.ILegacyIAccessibleProvider.Description => Description;

        string? UiaCore.ILegacyIAccessibleProvider.Help => Help;

        string? UiaCore.ILegacyIAccessibleProvider.KeyboardShortcut => KeyboardShortcut;

        string? UiaCore.ILegacyIAccessibleProvider.Name => Name;

        uint UiaCore.ILegacyIAccessibleProvider.Role => (uint)Role;

        uint UiaCore.ILegacyIAccessibleProvider.State => (uint)State;

        string? UiaCore.ILegacyIAccessibleProvider.Value => Value;

        int UiaCore.ILegacyIAccessibleProvider.ChildId => GetChildId();

        void UiaCore.ILegacyIAccessibleProvider.DoDefaultAction() => DoDefaultAction();

        IAccessible? UiaCore.ILegacyIAccessibleProvider.GetIAccessible() => AsIAccessible(this);

        UiaCore.IRawElementProviderSimple[] UiaCore.ILegacyIAccessibleProvider.GetSelection()
        {
            if (GetSelected() is UiaCore.IRawElementProviderSimple selected)
            {
                return new UiaCore.IRawElementProviderSimple[] { selected };
            }

            return Array.Empty<UiaCore.IRawElementProviderSimple>();
        }

        void UiaCore.ILegacyIAccessibleProvider.Select(int flagsSelect) => Select((AccessibleSelection)flagsSelect);

        void UiaCore.ILegacyIAccessibleProvider.SetValue(string szValue) => SetValue(szValue);

        void UiaCore.IExpandCollapseProvider.Expand() => Expand();

        void UiaCore.IExpandCollapseProvider.Collapse() => Collapse();

        UiaCore.ExpandCollapseState UiaCore.IExpandCollapseProvider.ExpandCollapseState => ExpandCollapseState;

        void UiaCore.IInvokeProvider.Invoke() => Invoke();

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider.DocumentRange => DocumentRangeInternal;

        UiaCore.ITextRangeProvider[]? UiaCore.ITextProvider.GetSelection() => GetTextSelection();

        UiaCore.ITextRangeProvider[]? UiaCore.ITextProvider.GetVisibleRanges() => GetTextVisibleRanges();

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider.RangeFromChild(UiaCore.IRawElementProviderSimple childElement) =>
            GetTextRangeFromChild(childElement);

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider.RangeFromPoint(Point screenLocation) => GetTextRangeFromPoint(screenLocation);

        UiaCore.SupportedTextSelection UiaCore.ITextProvider.SupportedTextSelection => SupportedTextSelectionInternal;

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider2.DocumentRange => DocumentRangeInternal;

        UiaCore.ITextRangeProvider[]? UiaCore.ITextProvider2.GetSelection() => GetTextSelection();

        UiaCore.ITextRangeProvider[]? UiaCore.ITextProvider2.GetVisibleRanges() => GetTextVisibleRanges();

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider2.RangeFromChild(UiaCore.IRawElementProviderSimple childElement) =>
            GetTextRangeFromChild(childElement);

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider2.RangeFromPoint(Point screenLocation) => GetTextRangeFromPoint(screenLocation);

        UiaCore.SupportedTextSelection UiaCore.ITextProvider2.SupportedTextSelection => SupportedTextSelectionInternal;

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider2.GetCaretRange(out BOOL isActive) => GetTextCaretRange(out isActive);

        UiaCore.ITextRangeProvider? UiaCore.ITextProvider2.RangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement) =>
            GetRangeFromAnnotation(annotationElement);

        BOOL UiaCore.IValueProvider.IsReadOnly => IsReadOnly ? true : false;

        string? UiaCore.IValueProvider.Value => Value;

        void UiaCore.IValueProvider.SetValue(string? newValue) => SetValue(newValue);

        void UiaCore.IToggleProvider.Toggle() => Toggle();

        UiaCore.ToggleState UiaCore.IToggleProvider.ToggleState => ToggleState;

        object[]? UiaCore.ITableProvider.GetRowHeaders() => GetRowHeaders();

        object[]? UiaCore.ITableProvider.GetColumnHeaders() => GetColumnHeaders();

        UiaCore.RowOrColumnMajor UiaCore.ITableProvider.RowOrColumnMajor => RowOrColumnMajor;

        object[]? UiaCore.ITableItemProvider.GetRowHeaderItems() => GetRowHeaderItems();

        object[]? UiaCore.ITableItemProvider.GetColumnHeaderItems() => GetColumnHeaderItems();

        object? UiaCore.IGridProvider.GetItem(int row, int column) => GetItem(row, column);

        int UiaCore.IGridProvider.RowCount => RowCount;

        int UiaCore.IGridProvider.ColumnCount => ColumnCount;

        int UiaCore.IGridItemProvider.Row => Row;

        int UiaCore.IGridItemProvider.Column => Column;

        int UiaCore.IGridItemProvider.RowSpan => RowSpan;

        int UiaCore.IGridItemProvider.ColumnSpan => ColumnSpan;

        UiaCore.IRawElementProviderSimple? UiaCore.IGridItemProvider.ContainingGrid => ContainingGrid;

        /// <summary>
        ///  Perform the default action
        /// </summary>
        void IAccessible.accDoDefaultAction(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.AccDoDefaultAction: this = {ToString()}, childID = {childID}");

                // If the default action is to be performed on self, do it.
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    DoDefaultAction();
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    child.DoDefaultAction();
                    return;
                }
            }

            _systemIAccessible.accDoDefaultAction(childID);
        }

        /// <summary>
        ///  Perform a hit test
        /// </summary>
        object? IAccessible.accHitTest(int xLeft, int yTop)
        {
            // When the AccessibleObjectFromPoint() is called it calls WindowFromPhysicalPoint() to find the window
            // under the given point. It then walks up parent windows with GetAncestor(hwnd, GA_PARENT) until it can't
            // find a parent, or the parent is the desktop. This "root" window is used to get the initial OBJID_WINDOW
            // (non client) IAccessible object (via WM_GETOBJECT).
            //
            // This starting IAccessible object gets the initial accHitTest call. AccessibleObjectFromPoint() will
            // keep recursively calling accHitTest on any new IAccessible objects that are returned. Once CHILDID_SELF
            // is returned from accHitTest, that IAccessible object is returned from AccessibleObjectFromPoint().
            //
            // The default Windows IAccessible behavior is for the OBJID_WINDOW object to check to see if the given
            // point is in the client area of the window and return that IAccessible object (OBJID_CLIENT). The default
            // OBJID_CLIENT behavior is to look for child windows that have bounds that contain the point (via
            // ChildWindowFromPoint()) and return the OBJID_WINDOW IAccessible object for any such window. In the
            // process of doing this, transparency is considered and WM_NCHITTEST is sent to the relevant windows to
            // assist in this check.

            if (IsClientObject)
            {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, $"AccessibleObject.AccHitTest: this = {ToString()}");

                AccessibleObject? obj = HitTest(xLeft, yTop);
                if (obj is not null)
                {
                    return AsVariant(obj);
                }
            }

            return _systemIAccessible.accHitTest(xLeft, yTop);
        }

        /// <summary>
        ///  The location of the Accessible object
        /// </summary>
        void IAccessible.accLocation(
            out int pxLeft,
            out int pyTop,
            out int pcxWidth,
            out int pcyHeight,
            object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.AccLocation: this = {ToString()}, childID = {childID}");

                // Use the Location function's return value if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    Rectangle bounds = Bounds;
                    pxLeft = bounds.X;
                    pyTop = bounds.Y;
                    pcxWidth = bounds.Width;
                    pcyHeight = bounds.Height;

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, $"AccessibleObject.AccLocation: Returning {bounds}");

                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    Rectangle bounds = child.Bounds;
                    pxLeft = bounds.X;
                    pyTop = bounds.Y;
                    pcxWidth = bounds.Width;
                    pcyHeight = bounds.Height;

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, $"AccessibleObject.AccLocation: Returning {bounds}");

                    return;
                }
            }

            _systemIAccessible.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, childID);
        }

        /// <summary>
        ///  Navigate to another accessible object.
        /// </summary>
        object? IAccessible.accNavigate(int navDir, object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.AccNavigate: this = {ToString()}, navdir = {navDir}, childID = {childID}");

                // Use the Navigate function's return value if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    AccessibleObject? newObject = Navigate((AccessibleNavigation)navDir);
                    if (newObject is not null)
                    {
                        return AsVariant(newObject);
                    }
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return AsVariant(child.Navigate((AccessibleNavigation)navDir));
                }
            }

            if (!SysNavigate(navDir, childID, out object? retObject))
            {
                return _systemIAccessible.accNavigate(navDir, childID);
            }

            return retObject;
        }

        /// <summary>
        ///  Select an accessible object.
        /// </summary>
        void IAccessible.accSelect(int flagsSelect, object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.AccSelect: this = {ToString()}, flagsSelect = {flagsSelect}, childID = {childID}");

                // If the selection is self, do it.
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    Select((AccessibleSelection)flagsSelect);    // Uses an Enum which matches SELFLAG
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    child.Select((AccessibleSelection)flagsSelect);
                    return;
                }
            }

            _systemIAccessible.accSelect(flagsSelect, childID);
        }

        /// <summary>
        ///  Performs the default action associated with this accessible object.
        /// </summary>
        public virtual void DoDefaultAction()
        {
            // By default, just does the system default action if available
            _systemIAccessible.accDoDefaultAction(0);
        }

        /// <summary>
        ///  Returns a child Accessible object
        /// </summary>
        object? IAccessible.get_accChild(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.GetAccChild: this = {ToString()}, childID = {childID}");

                // Return self for CHILDID_SELF
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return AsIAccessible(this);
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    // Make sure we're not returning ourselves as our own child
                    Debug.Assert(
                        child != this,
                        "An accessible object is returning itself as its own child. This can cause Accessibility client applications to stop responding.");

                    if (child == this)
                    {
                        return null;
                    }

                    return AsIAccessible(child);
                }
            }

            if (_systemIAccessible is null || _systemIAccessible.accChildCount == 0)
            {
                return null;
            }

            // Otherwise, return the default system child for this control (if any)
            return _systemIAccessible.get_accChild(childID);
        }

        /// <summary>
        ///  Return the number of children
        /// </summary>
        int IAccessible.accChildCount
        {
            get
            {
                int childCount = -1;

                if (IsClientObject)
                {
                    childCount = GetChildCount();
                }

                if (childCount == -1)
                {
                    childCount = _systemIAccessible.accChildCount;
                }

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.accChildCount: this = {ToString()}, returning {childCount}");

                return childCount;
            }
        }

        /// <summary>
        ///  Return the default action
        /// </summary>
        string? IAccessible.get_accDefaultAction(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                // Return the default action property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return DefaultAction;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.DefaultAction;
                }
            }

            return _systemIAccessible.get_accDefaultAction(childID);
        }

        /// <summary>
        ///  Return the object or child description
        /// </summary>
        string? IAccessible.get_accDescription(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                // Return the description property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return Description;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.Description;
                }
            }

            return _systemIAccessible.get_accDescription(childID);
        }

        /// <summary>
        ///  Returns the appropriate child from the Accessible Child Collection, if available
        /// </summary>
        private AccessibleObject? GetAccessibleChild(object childID)
        {
            if (!childID.Equals(NativeMethods.CHILDID_SELF))
            {
                // The first child is childID == 1 (index == 0)
                int index = (int)childID - 1;
                if (index >= 0 && index < GetChildCount())
                {
                    return GetChild(index);
                }
            }

            return null;
        }

        /// <summary>
        ///  Return the object or child focus
        /// </summary>
        object? IAccessible.accFocus
        {
            get
            {
                if (IsClientObject)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, $"AccessibleObject.GetAccFocus: this = {ToString()}");

                    AccessibleObject? obj = GetFocused();
                    if (obj is not null)
                    {
                        return AsVariant(obj);
                    }
                }

                return _systemIAccessible.accFocus;
            }
        }

        /// <summary>
        ///  Return help for this accessible object.
        /// </summary>
        string? IAccessible.get_accHelp(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return Help;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.Help;
                }
            }

            return _systemIAccessible.get_accHelp(childID);
        }

        /// <summary>
        ///  Return the object or child help topic
        /// </summary>
        int IAccessible.get_accHelpTopic(out string? pszHelpFile, object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return GetHelpTopic(out pszHelpFile);
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.GetHelpTopic(out pszHelpFile);
                }
            }

            return _systemIAccessible.get_accHelpTopic(out pszHelpFile, childID);
        }

        /// <summary>
        ///  Return the object or child keyboard shortcut
        /// </summary>
        string? IAccessible.get_accKeyboardShortcut(object childID)
        {
            return get_accKeyboardShortcutInternal(childID);
        }

        internal virtual string? get_accKeyboardShortcutInternal(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return KeyboardShortcut;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.KeyboardShortcut;
                }
            }

            return _systemIAccessible.get_accKeyboardShortcut(childID);
        }

        /// <summary>
        ///  Return the object or child name
        /// </summary>
        string? IAccessible.get_accName(object childID)
        {
            return get_accNameInternal(childID);
        }

        internal virtual string? get_accNameInternal(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.get_accName: this = {ToString()}, childID = {childID}");

                // Return the name property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return Name;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.Name;
                }
            }

            string? retval = _systemIAccessible.get_accName(childID);

            if (IsClientObject)
            {
                if (string.IsNullOrEmpty(retval))
                {
                    // Name the child after its parent
                    retval = Name;
                }
            }

            return retval;
        }

        /// <summary>
        ///  Return the parent object
        /// </summary>
        object? IAccessible.accParent
        {
            get
            {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, $"AccessibleObject.accParent: this = {ToString()}");
                AccessibleObject? parent = Parent;
                if (parent is not null)
                {
                    // Some debugging related tests
                    Debug.Assert(
                        parent != this,
                        "An accessible object is returning itself as its own parent. This can cause accessibility clients to stop responding.");
                    if (parent == this)
                    {
                        // This should prevent accessibility clients from stop responding
                        parent = null;
                    }
                }

                return AsIAccessible(parent);
            }
        }

        /// <summary>
        ///  The role property describes an object's purpose in terms of its
        ///  relationship with sibling or child objects.
        /// </summary>
        object? IAccessible.get_accRole(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                // Return the role property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return (int)Role;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return (int)child.Role;
                }
            }

            if (_systemIAccessible is null || _systemIAccessible.accChildCount == 0)
            {
                return null;
            }

            return _systemIAccessible.get_accRole(childID);
        }

        /// <summary>
        ///  Return the object or child selection
        /// </summary>
        object? IAccessible.accSelection
        {
            get
            {
                if (IsClientObject)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, $"AccessibleObject.GetAccSelection: this = {ToString()}");

                    AccessibleObject? obj = GetSelected();
                    if (obj is not null)
                    {
                        return AsVariant(obj);
                    }
                }

                return _systemIAccessible.accSelection;
            }
        }

        /// <summary>
        ///  Return the object or child state
        /// </summary>
        object? IAccessible.get_accState(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(
                    CompModSwitches.MSAA.TraceInfo,
                    $"AccessibleObject.GetAccState: this = {ToString()}, childID = {childID}");

                // Return the state property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return (int)State;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return (int)child.State;
                }
            }

            return _systemIAccessible?.get_accState(childID);
        }

        /// <summary>
        ///  Return the object or child value
        /// </summary>
        string? IAccessible.get_accValue(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                // Return the value property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return Value;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    return child.Value;
                }
            }

            return _systemIAccessible.get_accValue(childID);
        }

        /// <summary>
        ///  Set the object or child name
        /// </summary>
        void IAccessible.set_accName(object childID, string newName)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                // Set the name property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    // Attempt to set the name property
                    Name = newName;
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    child.Name = newName;
                    return;
                }
            }

            _systemIAccessible?.set_accName(childID, newName);
        }

        /// <summary>
        ///  Set the object or child value
        /// </summary>
        void IAccessible.set_accValue(object childID, string newValue)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                // Set the value property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    // Attempt to set the value property
                    Value = newValue;
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject? child = GetAccessibleChild(childID);
                if (child is not null)
                {
                    child.Value = newValue;
                    return;
                }
            }

            _systemIAccessible?.set_accValue(childID, newValue);
        }

        /// <summary>
        ///  Now that AccessibleObject is used to wrap all system-provided (OLEACC.DLL) accessible
        ///  objects, it needs to implement IOleWindow and pass this down to the inner object. This is
        ///  necessary because the OS function WindowFromAccessibleObject() walks up the parent chain
        ///  looking for the first object that implements IOleWindow, and uses that to get the hwnd.
        ///
        ///  But this creates a new problem for AccessibleObjects that do NOT have windows, ie. which
        ///  represent simple elements. To the OS, these simple elements will now appear to implement
        ///  IOleWindow, so it will try to get hwnds from them - which they simply cannot provide.
        ///
        ///  To work around this problem, the AccessibleObject for a simple element will delegate all
        ///  IOleWindow calls up the parent chain itself. This will stop at the first window-based
        ///  accessible object, which will be able to return an hwnd back to the OS. So we are
        ///  effectively 'preempting' what WindowFromAccessibleObject() would do.
        /// </summary>
        unsafe HRESULT Ole32.IOleWindow.GetWindow(HWND* phwnd)
        {
            // See if we have an inner object that can provide the window handle
            if (_systemIOleWindow is not null)
            {
                return _systemIOleWindow.GetWindow(phwnd);
            }

            // Otherwise delegate to the parent object
            AccessibleObject? parent = Parent;
            if (parent is Ole32.IOleWindow parentWindow)
            {
                return parentWindow.GetWindow(phwnd);
            }

            // Or fail if there is no parent
            if (phwnd is null)
            {
                return HRESULT.Values.E_POINTER;
            }

            *phwnd = HWND.Null;
            return HRESULT.Values.E_FAIL;
        }

        /// <summary>
        ///  See GetWindow() above for details.
        /// </summary>
        HRESULT Ole32.IOleWindow.ContextSensitiveHelp(BOOL fEnterMode)
        {
            // See if we have an inner object that can provide help
            if (_systemIOleWindow is not null)
            {
                return _systemIOleWindow.ContextSensitiveHelp(fEnterMode);
            }

            // Otherwise delegate to the parent object
            AccessibleObject? parent = Parent;
            if (parent is Ole32.IOleWindow parentWindow)
            {
                return parentWindow.ContextSensitiveHelp(fEnterMode);
            }

            // Or do nothing if there is no parent
            return HRESULT.Values.S_OK;
        }

        /// <summary>
        ///  Clone this accessible object.
        /// </summary>
        HRESULT Oleaut32.IEnumVariant.Clone(Oleaut32.IEnumVariant[]? ppEnum) => EnumVariant.Clone(ppEnum);

        /// <summary>
        ///  Obtain the next n children of this accessible object.
        /// </summary>
        unsafe HRESULT Oleaut32.IEnumVariant.Next(uint celt, IntPtr rgVar, uint* pCeltFetched)
        {
            return EnumVariant.Next(celt, rgVar, pCeltFetched);
        }

        /// <summary>
        ///  Resets the child accessible object enumerator.
        /// </summary>
        HRESULT Oleaut32.IEnumVariant.Reset() => EnumVariant.Reset();

        /// <summary>
        ///  Skip the next n child accessible objects
        /// </summary>
        HRESULT Oleaut32.IEnumVariant.Skip(uint celt) => EnumVariant.Skip(celt);

        /// <summary>
        ///  When overridden in a derived class, navigates to another object.
        /// </summary>
        public virtual AccessibleObject? Navigate(AccessibleNavigation navdir)
        {
            // Some default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                switch (navdir)
                {
                    case AccessibleNavigation.FirstChild:
                        return GetChild(0);
                    case AccessibleNavigation.LastChild:
                        return GetChild(GetChildCount() - 1);
                    case AccessibleNavigation.Previous:
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                        if (Parent?.GetChildCount() > 0)
                        {
                            return null;
                        }

                        break;
                    case AccessibleNavigation.Next:
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                        if (Parent?.GetChildCount() > 0)
                        {
                            return null;
                        }

                        break;
                }
            }

            if (_systemIAccessible.IsIAccessibleCreated)
            {
                if (!SysNavigate((int)navdir, NativeMethods.CHILDID_SELF, out object? retObject))
                {
                    retObject = _systemIAccessible.accNavigate((int)navdir, NativeMethods.CHILDID_SELF);
                }

                return WrapIAccessible(retObject);
            }

            return null;
        }

        /// <summary>
        ///  Selects this accessible object.
        /// </summary>
        public virtual void Select(AccessibleSelection flags)
        {
            _systemIAccessible.accSelect((int)flags, 0);
        }

        private object? AsVariant(AccessibleObject? obj)
        {
            if (obj == this)
            {
                return NativeMethods.CHILDID_SELF;
            }

            return AsIAccessible(obj);
        }

        private static IAccessible? AsIAccessible(AccessibleObject? obj)
        {
            if (obj is not null && obj._systemWrapper)
            {
                return obj._systemIAccessible.SystemIAccessibleInternal;
            }

            return obj;
        }

        /// <summary>
        ///  Indicates what kind of 'inner' system accessible object we are using as our fall-back
        ///  implementation of IAccessible (when the systemIAccessible member is not null). The inner
        ///  object is provided by OLEACC.DLL. Note that although the term 'id' is used, this value
        ///  really represents a category or type of accessible object. Ids are only unique among
        ///  accessible objects associated with the same window handle. Currently supported ids are...
        ///
        ///  OBJID_CLIENT - represents the window's client area (including any child windows)
        ///  OBJID_WINDOW - represents the window's non-client area (including caption, frame controls and scrollbars)
        ///
        ///  NOTE: When the id is OBJID_WINDOW, we short-circuit most of the virtual override behavior of
        ///  AccessibleObject, and turn the object into a simple wrapper around the inner system object. So
        ///  for a *user-defined* accessible object, that has NO inner object, its important that the id is
        ///  left as OBJID_CLIENT, otherwise the object will be short-circuited into a total NOP!
        /// </summary>
        internal int AccessibleObjectId { get; set; } = User32.OBJID.CLIENT;

        /// <summary>
        ///  Indicates whether this accessible object represents the client area of
        ///  the window.
        /// </summary>
        internal bool IsClientObject => AccessibleObjectId == User32.OBJID.CLIENT;

        /// <summary>
        ///  Indicates whether this accessible object represents the non-client
        ///  area of the window.
        /// </summary>
        internal bool IsNonClientObject => AccessibleObjectId == User32.OBJID.WINDOW;

        internal IAccessible? GetSystemIAccessibleInternal() => _systemIAccessible.SystemIAccessibleInternal;

        protected void UseStdAccessibleObjects(IntPtr handle)
        {
            UseStdAccessibleObjects(handle, AccessibleObjectId);
        }

        protected void UseStdAccessibleObjects(IntPtr handle, int objid)
        {
            object? acc = null;
            UnsafeNativeMethods.CreateStdAccessibleObject(
                new HandleRef(this, handle),
                objid,
                ref IID.IAccessible,
                ref acc);

            Guid IID_IEnumVariant = IID.IEnumVariant;
            Oleacc.CreateStdAccessibleObject(
                new HandleRef(this, handle),
                objid,
                ref IID_IEnumVariant,
                out var enumVariantPtr);

            if (enumVariantPtr != IntPtr.Zero)
            {
                _systemIEnumVariant = (WinFormsComWrappers.EnumVariantWrapper)WinFormsComWrappers.Instance
                    .GetOrCreateObjectForComInstance(enumVariantPtr, CreateObjectFlags.None);
            }

            if (acc is not null)
            {
                _systemIAccessible = new SystemIAccessibleWrapper((IAccessible?)acc);
                _systemIOleWindow = (Ole32.IOleWindow?)acc;
            }
        }

        /// <summary>
        ///  Performs custom navigation between parent/child/sibling accessible
        ///  objects. This is basically just a wrapper for GetSysChild(), that
        ///  does some of the dirty work, such as wrapping the returned object
        ///  in a VARIANT. Usage is similar to GetSysChild(). Called prior to
        ///  calling IAccessible.accNavigate on the 'inner' system accessible
        ///  object.
        /// </summary>
        private bool SysNavigate(int navDir, object childID, out object? retObject)
        {
            retObject = null;

            // Only override system navigation relative to ourselves (since we can't interpret OLEACC child ids)
            if (!childID.Equals(NativeMethods.CHILDID_SELF))
            {
                return false;
            }

            // Perform any supported navigation operation (fall back on system for unsupported navigation ops)
            if (!GetSysChild((AccessibleNavigation)navDir, out AccessibleObject? newObject))
            {
                return false;
            }

            // If object found, wrap in a VARIANT. Otherwise return null for 'end of list' (OLEACC expects this)
            retObject = (newObject is null) ? null : AsVariant(newObject);

            // Tell caller not to fall back on system behavior now
            return true;
        }

        /// <summary>
        ///  Make sure that the childID is valid.
        /// </summary>
        internal static void ValidateChildID(ref object childID)
        {
            // An empty childID is considered to be the same as CHILDID_SELF.
            // Some accessibility programs pass null into our functions, so we
            // need to convert them here.
            if (childID is null)
            {
                childID = NativeMethods.CHILDID_SELF;
            }
            else if (childID.Equals((int)HRESULT.Values.DISP_E_PARAMNOTFOUND))
            {
                childID = 0;
            }
            else if (childID is not int)
            {
                // AccExplorer seems to occasionally pass in objects instead of an int ChildID.
                childID = 0;
            }
        }

        private AccessibleObject? WrapIAccessible(object? iacc)
        {
            if (iacc is not IAccessible accessible)
            {
                return null;
            }

            // Check to see if this object already wraps iacc
            if (_systemIAccessible.SystemIAccessibleInternal == iacc)
            {
                return this;
            }

            return new AccessibleObject(accessible);
        }

        /// <summary>
        ///  Return the requested method if it is implemented by the Reflection object. The
        ///  match is based upon the name and DescriptorInfo which describes the signature
        ///  of the method.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder? binder, Type[] types, ParameterModifier[]? modifiers)
            => typeof(IAccessible).GetMethod(name, bindingAttr, binder, types, modifiers);

        /// <summary>
        ///  Return the requested method if it is implemented by the Reflection object. The
        ///  match is based upon the name of the method. If the object implemented multiple methods
        ///  with the same name an AmbiguousMatchException is thrown.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr)
            => typeof(IAccessible).GetMethod(name, bindingAttr);

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
            => typeof(IAccessible).GetMethods(bindingAttr);

        /// <summary>
        ///  Return the requestion field if it is implemented by the Reflection
        ///  object. The match is based upon a name. There cannot be more than
        ///  a single field with a name.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr)
            => typeof(IAccessible).GetField(name, bindingAttr);

        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
            => typeof(IAccessible).GetFields(bindingAttr);

        /// <summary>
        ///  Return the property based upon name. If more than one property has
        ///  the given name an AmbiguousMatchException will be thrown. Returns
        ///  null if no property is found.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
        PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr)
            => typeof(IAccessible).GetProperty(name, bindingAttr);

        /// <summary>
        ///  Return the property based upon the name and Descriptor info describing
        ///  the property indexing. Return null if no property is found.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
        PropertyInfo? IReflect.GetProperty(
            string name,
            BindingFlags bindingAttr,
            Binder? binder,
            Type? returnType,
            Type[] types,
            ParameterModifier[]? modifiers)
            => typeof(IAccessible).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

        /// <summary>
        ///  Returns an array of PropertyInfos for all the properties defined on
        ///  the Reflection object.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
            => typeof(IAccessible).GetProperties(bindingAttr);

        /// <summary>
        ///  Return an array of members which match the passed in name.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields |
            DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods |
            DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents |
            DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties |
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors |
            DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
            => typeof(IAccessible).GetMember(name, bindingAttr);

        /// <summary>
        ///  Return an array of all of the members defined for this object.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields |
            DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods |
            DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents |
            DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties |
            DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors |
            DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
            => typeof(IAccessible).GetMembers(bindingAttr);

        /// <summary>
        ///  Description of the Binding Process.
        ///  We must invoke a method that is accessable and for which the provided
        ///  parameters have the most specific match. A method may be called if
        ///  1. The number of parameters in the method declaration equals the number of
        ///  arguments provided to the invocation
        ///  2. The type of each argument can be converted by the binder to the
        ///  type of the type of the parameter.
        ///
        ///  The binder will find all of the matching methods. These method are found based
        ///  upon the type of binding requested (MethodInvoke, Get/Set Properties). The set
        ///  of methods is filtered by the name, number of arguments and a set of search modifiers
        ///  defined in the Binder.
        ///
        ///  After the method is selected, it will be invoked. Accessability is checked
        ///  at that point. The search may be control which set of methods are searched based
        ///  upon the accessibility attribute associated with the method.
        ///
        ///  The BindToMethod method is responsible for selecting the method to be invoked.
        ///  For the default binder, the most specific method will be selected.
        ///
        ///  This will invoke a specific member...
        ///  @exception If <var>invokeAttr</var> is CreateInstance then all other
        ///  Access types must be undefined. If not we throw an ArgumentException.
        ///  @exception If the <var>invokeAttr</var> is not CreateInstance then an
        ///  ArgumentException when <var>name</var> is null.
        ///  @exception ArgumentException when <var>invokeAttr</var> does not specify the type
        ///  @exception ArgumentException when <var>invokeAttr</var> specifies both get and set of
        ///  a property or field.
        ///  @exception ArgumentException when <var>invokeAttr</var> specifies property set and
        ///  invoke method.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
        object? IReflect.InvokeMember(
            string name,
            BindingFlags invokeAttr,
            Binder? binder,
            object? target,
            object?[]? args,
            ParameterModifier[]? modifiers,
            CultureInfo? culture,
            string[]? namedParameters)
        {
            if (args?.Length == 0)
            {
                MemberInfo[] member = typeof(IAccessible).GetMember(name);
                if (member is not null && member.Length > 0 && member[0] is PropertyInfo info)
                {
                    MethodInfo? getMethod = info.GetGetMethod();
                    if (getMethod is not null && getMethod.GetParameters().Length > 0)
                    {
                        args = new object[getMethod.GetParameters().Length];
                        for (int i = 0; i < args.Length; i++)
                        {
                            args[i] = NativeMethods.CHILDID_SELF;
                        }
                    }
                }
            }

            return typeof(IAccessible).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }

        /// <summary>
        ///  Return the underlying Type that represents the IReflect Object. For
        ///  expando object, this is the (Object) IReflectInstance.GetType().
        ///  For Type object it is this.
        /// </summary>
        Type IReflect.UnderlyingSystemType => typeof(IAccessible);

        UiaCore.IRawElementProviderSimple? UiaCore.IRawElementProviderHwndOverride.GetOverrideProviderForHwnd(IntPtr hwnd)
            => GetOverrideProviderForHwnd(hwnd);

        int UiaCore.IMultipleViewProvider.CurrentView => GetMultiViewProviderCurrentView();

        int[]? UiaCore.IMultipleViewProvider.GetSupportedViews() => GetMultiViewProviderSupportedViews();

        string? UiaCore.IMultipleViewProvider.GetViewName(int viewId) => GetMultiViewProviderViewName(viewId);

        void UiaCore.IMultipleViewProvider.SetCurrentView(int viewId) => SetMultiViewProviderCurrentView(viewId);

        BOOL UiaCore.IRangeValueProvider.IsReadOnly => IsReadOnly ? true : false;

        double UiaCore.IRangeValueProvider.LargeChange => LargeChange;

        double UiaCore.IRangeValueProvider.Maximum => Maximum;

        double UiaCore.IRangeValueProvider.Minimum => Minimum;

        double UiaCore.IRangeValueProvider.SmallChange => SmallChange;

        double UiaCore.IRangeValueProvider.Value => RangeValue;

        void UiaCore.IRangeValueProvider.SetValue(double value) => SetValue(value);

        object[]? UiaCore.ISelectionProvider.GetSelection() => GetSelection();

        BOOL UiaCore.ISelectionProvider.CanSelectMultiple => CanSelectMultiple ? true : false;

        BOOL UiaCore.ISelectionProvider.IsSelectionRequired => IsSelectionRequired ? true : false;

        void UiaCore.ISelectionItemProvider.Select() => SelectItem();

        void UiaCore.ISelectionItemProvider.AddToSelection() => AddToSelection();

        void UiaCore.ISelectionItemProvider.RemoveFromSelection() => RemoveFromSelection();

        BOOL UiaCore.ISelectionItemProvider.IsSelected => IsItemSelected ? true : false;

        UiaCore.IRawElementProviderSimple? UiaCore.ISelectionItemProvider.SelectionContainer => ItemSelectionContainer;

        /// <summary>
        ///  Raises the UIA Notification event.
        ///  The event is available starting with Windows 10, version 1709.
        /// </summary>
        /// <param name="notificationKind">The type of notification</param>
        /// <param name="notificationProcessing">Indicates how to process notifications</param>
        /// <param name="notificationText">Notification text</param>
        /// <returns>
        ///  True if operation succeeds.
        ///  False if the underlying windows infrastructure is not available or the operation had failed.
        ///  Use Marshal.GetLastWin32Error for details.
        /// </returns>
        public bool RaiseAutomationNotification(
            AutomationNotificationKind notificationKind,
            AutomationNotificationProcessing notificationProcessing,
            string notificationText)
        {
            if (!s_notificationEventAvailable || !CanNotifyClients)
            {
                return false;
            }

            try
            {
                // The activityId can be any string. It cannot be null. It is not used currently.
                HRESULT result = UiaCore.UiaRaiseNotificationEvent(
                    this,
                    notificationKind,
                    notificationProcessing,
                    notificationText,
                    string.Empty);
                return result == HRESULT.Values.S_OK;
            }
            catch (EntryPointNotFoundException)
            {
                // The UIA Notification event is not available, so don't attempt to raise it again.
                s_notificationEventAvailable = false;
                return false;
            }
        }

        /// <summary>
        ///  Raises the LiveRegionChanged UIA event.
        ///  This method must be overridden in derived classes that support the UIA live region feature.
        /// </summary>
        /// <returns>True if operation succeeds, False otherwise.</returns>
        public virtual bool RaiseLiveRegionChanged()
        {
            throw new NotSupportedException(SR.AccessibleObjectLiveRegionNotSupported);
        }

        internal virtual bool RaiseAutomationEvent(UiaCore.UIA eventId)
        {
            if (UiaCore.UiaClientsAreListening() && CanNotifyClients)
            {
                HRESULT result = UiaCore.UiaRaiseAutomationEvent(this, eventId);
                return result == HRESULT.Values.S_OK;
            }

            return false;
        }

        internal virtual bool RaiseAutomationPropertyChangedEvent(UiaCore.UIA propertyId, object? oldValue, object? newValue)
        {
            if (UiaCore.UiaClientsAreListening() && CanNotifyClients)
            {
                HRESULT result = UiaCore.UiaRaiseAutomationPropertyChangedEvent(this, propertyId, oldValue, newValue);
                return result == HRESULT.Values.S_OK;
            }

            return false;
        }

        internal virtual bool InternalRaiseAutomationNotification(
            AutomationNotificationKind notificationKind,
            AutomationNotificationProcessing notificationProcessing,
            string notificationText)
        {
            if (UiaCore.UiaClientsAreListening())
            {
                return RaiseAutomationNotification(notificationKind, notificationProcessing, notificationText);
            }

            return s_notificationEventAvailable;
        }

        internal bool RaiseStructureChangedEvent(UiaCore.StructureChangeType structureChangeType, int[] runtimeId)
        {
            if (UiaCore.UiaClientsAreListening() && CanNotifyClients)
            {
                HRESULT result = UiaCore.UiaRaiseStructureChangedEvent(this, structureChangeType, runtimeId, runtimeId is null ? 0 : runtimeId.Length);
                return result == HRESULT.Values.S_OK;
            }

            return false;
        }

        void UiaCore.IScrollItemProvider.ScrollIntoView() => ScrollIntoView();

        internal virtual void ScrollIntoView()
        {
            Debug.Fail($"{nameof(ScrollIntoView)}() is not overriden");
        }
    }
}
