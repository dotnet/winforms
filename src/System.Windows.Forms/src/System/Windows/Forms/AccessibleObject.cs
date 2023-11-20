// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static Interop;
using ComIServiceProvider = Windows.Win32.System.Com.IServiceProvider;
using IAccessible = Accessibility.IAccessible;
using UIA = Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  Provides an implementation for an object that can be inspected by an accessibility application.
/// </summary>
public unsafe partial class AccessibleObject :
    StandardOleMarshalObject,
    IReflect,
    IAccessible,
    UiaCore.IAccessibleEx,
    ComIServiceProvider.Interface,
    IRawElementProviderSimple.Interface,
    IRawElementProviderFragment.Interface,
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
    IEnumVARIANT.Interface,
    IOleWindow.Interface,
    UiaCore.ILegacyIAccessibleProvider,
    UiaCore.ISelectionProvider,
    UiaCore.ISelectionItemProvider,
    UiaCore.IRawElementProviderHwndOverride,
    UiaCore.IScrollItemProvider,
    UiaCore.IMultipleViewProvider,
    ITextProvider.Interface,
    ITextProvider2.Interface,
    // This IAccessible needs moved first once IManagedWrapper is implemented so it gets chosen by built-in COM interop.
    UIA.IAccessible.Interface,
    IDispatch.Interface,
    IDispatchEx.Interface
{
    private static readonly string[] s_propertiesWithArguments =
    [
        "accChild",
        "accName",
        "accValue",
        "accDescription",
        "accRole",
        "accState",
        "accHelp",
        "accKeyboardShortcut",
        "accDefaultAction"
    ];

    private readonly AccessibleDispatchAdapter _dispatchAdapter;

    /// <summary>
    ///  The <see cref="UIA.IAccessible"/> as passed in or generated from requesting the standard implementation
    ///  from Windows. Used for default <see cref="UIA.IAccessible"/> behavior.
    /// </summary>
    internal AgileComPointer<UIA.IAccessible>? SystemIAccessible { get; private set; }

    private protected static VARIANT CHILDID_SELF { get; } = (VARIANT)(int)PInvoke.CHILDID_SELF;

    /// <summary>
    ///  Specifies the <see cref="IEnumVARIANT"/> used by this <see cref="AccessibleObject"/>.
    /// </summary>
    private AgileComPointer<IEnumVARIANT>? _systemIEnumVariant;
    private IEnumVARIANT.Interface? _enumVariant;

    // IOleWindow interface of the 'inner' system IAccessible object that we are wrapping
    private AgileComPointer<IOleWindow>? _systemIOleWindow;

    // Indicates this object is being used ONLY to wrap a system IAccessible
    private readonly bool _isSystemWrapper;

    private static bool? s_canNotifyClients;

    internal const int InvalidIndex = -1;

    internal const int RuntimeIDFirstItem = 0x2a;

    public AccessibleObject() => _dispatchAdapter = new(this);

    /// <devdoc>
    ///  This constructor is used ONLY for wrapping system IAccessible objects
    ///  that are returned by the IAccessible methods.
    /// </devdoc>
    private AccessibleObject(AgileComPointer<UIA.IAccessible> accessible)
        : this()
    {
        SystemIAccessible = accessible;
        _isSystemWrapper = true;
    }

    private protected virtual string? AutomationId => null;

    /// <summary>
    ///  Gets the bounds of the accessible object, in screen coordinates.
    /// </summary>
    public virtual Rectangle Bounds => SystemIAccessible.TryGetLocation(CHILDID_SELF);

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
    public virtual string? DefaultAction => SystemIAccessible.TryGetDefaultAction(CHILDID_SELF);

    /// <summary>
    ///  Gets a description of the object's visual appearance to the user.
    /// </summary>
    public virtual string? Description => SystemIAccessible.TryGetDescription(CHILDID_SELF);

    private IEnumVARIANT.Interface EnumVariant => _enumVariant ??= new EnumVariantObject(this);

    /// <summary>
    ///  Gets a description of what the object does or how the object is used.
    /// </summary>
    public virtual string? Help => SystemIAccessible.TryGetHelp(CHILDID_SELF);

    /// <summary>
    ///  Gets the object shortcut key or access key for an accessible object.
    /// </summary>
    public virtual string? KeyboardShortcut => SystemIAccessible.TryGetKeyboardShortcut(CHILDID_SELF);

    /// <summary>
    ///  Gets or sets the object name.
    /// </summary>
    public virtual string? Name
    {
        get => SystemIAccessible.TryGetName(CHILDID_SELF);
        set => SystemIAccessible.TrySetName(CHILDID_SELF, value);
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
    public virtual AccessibleObject? Parent
    {
        get
        {
            using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
            if (result.Succeeded)
            {
                IDispatch* dispatch;
                result = accessible.Value->get_accParent(&dispatch);
                return TryGetAccessibleObject(dispatch);
            }

            return null;
        }
    }

    /// <summary>
    ///  Gets the role of this accessible object.
    /// </summary>
    public virtual AccessibleRole Role => SystemIAccessible.TryGetRole(CHILDID_SELF);

    /// <summary>
    ///  Gets the state of this accessible object.
    /// </summary>
    public virtual AccessibleStates State => SystemIAccessible.TryGetState(CHILDID_SELF);

    /// <summary>
    ///  Gets or sets the value of an accessible object.
    /// </summary>
    public virtual string? Value
    {
        // This might be better to never return null or return null instead of string.Empty?
        get => SystemIAccessible is null ? string.Empty : SystemIAccessible.TryGetValue(CHILDID_SELF);
        set => SystemIAccessible.TrySetValue(CHILDID_SELF, value);
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
    ///  Mechanism for overriding default <see cref="UIA.IAccessible.accNavigate(int, VARIANT, VARIANT*)"/>
    ///  behavior of the 'inner' system accessible object (accNavigate is how you move between parent, child and
    ///  sibling accessible objects).
    /// </summary>
    /// <param name="navdir">
    ///  Navigation operation to perform, relative to this accessible object.
    /// </param>
    /// <param name="accessibleObject">
    ///  The destination object or <see langword="null"/> to indicate 'off end of list'.
    /// </param>
    /// <returns>
    ///  <see langword="false"/> to allow fall-back to default system behavior.
    /// </returns>
    internal virtual bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject? accessibleObject)
    {
        accessibleObject = null;
        return false;
    }

    /// <summary>
    ///  When overridden in a derived class, gets the object that has the keyboard focus.
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

            return State.HasFlag(AccessibleStates.Focused) ? this : null;
        }

        return TryGetFocus();
    }

    private AccessibleObject? TryGetFocus()
    {
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return null;
        }

        result = accessible.Value->get_accFocus(out VARIANT focus);
        if (result.Failed)
        {
            Debug.Assert(result == HRESULT.DISP_E_MEMBERNOTFOUND, $"{nameof(TryGetFocus)} failed with {result}");
            return null;
        }

        return TryGetAccessibleObject(focus);
    }

    /// <summary>
    ///  Gets an identifier for a Help topic and the path to the Help file associated with this accessible object.
    /// </summary>
    public virtual int GetHelpTopic(out string? fileName) => SystemIAccessible.TryGetHelpTopic(CHILDID_SELF, out fileName);

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
                if (child is not null && child.State.HasFlag(AccessibleStates.Selected))
                {
                    return child;
                }
            }

            return State.HasFlag(AccessibleStates.Selected) ? this : null;
        }

        return TryGetSelection();
    }

    private AccessibleObject? TryGetSelection()
    {
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return null;
        }

        result = accessible.Value->get_accSelection(out VARIANT selection);
        if (result.Failed)
        {
            Debug.Assert(result == HRESULT.DISP_E_MEMBERNOTFOUND, $"{nameof(TryGetSelection)} failed with {result}");
            return null;
        }

        return TryGetAccessibleObject(selection);
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

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Succeeded)
        {
            result = accessible.Value->accHitTest(x, y, out VARIANT child);
            return result.Failed || result == HRESULT.S_FALSE ? null : TryGetAccessibleObject(child);
        }

        return Bounds.Contains(x, y) ? this : null;
    }

    internal virtual bool IsIAccessibleExSupported()
    {
        // Override this, in your derived class, to enable IAccessibleEx support.
        return false;
    }

    /// <summary>
    ///  Indicates whether specified pattern is supported.
    /// </summary>
    /// <param name="patternId">The pattern ID.</param>
    /// <returns><see langword="true"/> if <paramref name="patternId"/> is supported.</returns>
    internal virtual bool IsPatternSupported(UIA_PATTERN_ID patternId)
        => patternId == UIA_PATTERN_ID.UIA_InvokePatternId && IsInvokePatternAvailable;

    /// <summary>
    ///  Gets the runtime ID.
    /// </summary>
    internal virtual int[] RuntimeId
    {
        get
        {
            if (_isSystemWrapper)
            {
                return new int[] { RuntimeIDFirstItem, GetHashCode() };
            }

            string message = string.Format(SR.AccessibleObjectRuntimeIdNotSupported, nameof(AccessibleObject), nameof(RuntimeId));
            Debug.Fail(message);
            throw new NotSupportedException(message);
        }
    }

    internal virtual int ProviderOptions
        => (int)(UIA.ProviderOptions.ProviderOptions_ServerSideProvider | UIA.ProviderOptions.ProviderOptions_UseComThreading);

    internal virtual IRawElementProviderSimple* HostRawElementProvider => null;

    /// <summary>
    ///  Returns the value of the specified <paramref name="propertyID"/> from the element in the form of a <see cref="VARIANT"/>.
    ///  See <see href="https://learn.microsoft.com/windows/win32/winauto/uiauto-automation-element-propids"/> which outlines how the <see cref="VARIANT"/> should be defined for
    ///  each <see cref="UIA_PROPERTY_ID"/>
    /// </summary>
    /// <param name="propertyID">Identifier indicating the property to return.</param>
    /// <returns>The requested value if supported or <see cref="VARIANT.Empty"/> if it is not.</returns>
    internal virtual VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
        propertyID switch
        {
            UIA_PROPERTY_ID.UIA_AccessKeyPropertyId => (VARIANT)(KeyboardShortcut ?? string.Empty),
            UIA_PROPERTY_ID.UIA_AutomationIdPropertyId => AutomationId is null ? VARIANT.Empty : (VARIANT)AutomationId,
            UIA_PROPERTY_ID.UIA_BoundingRectanglePropertyId => UiaTextProvider.BoundingRectangleAsVariant(Bounds),
            UIA_PROPERTY_ID.UIA_FrameworkIdPropertyId => (VARIANT)"WinForm",
            UIA_PROPERTY_ID.UIA_IsExpandCollapsePatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_ExpandCollapsePatternId),
            UIA_PROPERTY_ID.UIA_IsGridItemPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_GridItemPatternId),
            UIA_PROPERTY_ID.UIA_IsGridPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_GridPatternId),
            UIA_PROPERTY_ID.UIA_IsInvokePatternAvailablePropertyId => (VARIANT)IsInvokePatternAvailable,
            UIA_PROPERTY_ID.UIA_IsLegacyIAccessiblePatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId),
            UIA_PROPERTY_ID.UIA_IsMultipleViewPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_MultipleViewPatternId),
            UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => (VARIANT)((State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen),
            UIA_PROPERTY_ID.UIA_IsPasswordPropertyId => VARIANT.False,
            UIA_PROPERTY_ID.UIA_IsScrollItemPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollItemPatternId),
            UIA_PROPERTY_ID.UIA_IsScrollPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_ScrollPatternId),
            UIA_PROPERTY_ID.UIA_IsSelectionItemPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_SelectionItemPatternId),
            UIA_PROPERTY_ID.UIA_IsSelectionPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_SelectionPatternId),
            UIA_PROPERTY_ID.UIA_IsTableItemPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_TableItemPatternId),
            UIA_PROPERTY_ID.UIA_IsTablePatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_TablePatternId),
            UIA_PROPERTY_ID.UIA_IsTextPattern2AvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_TextPattern2Id),
            UIA_PROPERTY_ID.UIA_IsTextPatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_TextPatternId),
            UIA_PROPERTY_ID.UIA_IsTogglePatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_TogglePatternId),
            UIA_PROPERTY_ID.UIA_IsValuePatternAvailablePropertyId => (VARIANT)IsPatternSupported(UIA_PATTERN_ID.UIA_ValuePatternId),
            UIA_PROPERTY_ID.UIA_HelpTextPropertyId => (VARIANT)(Help ?? string.Empty),
            UIA_PROPERTY_ID.UIA_LegacyIAccessibleDefaultActionPropertyId => !string.IsNullOrEmpty(DefaultAction) ? (VARIANT)DefaultAction : VARIANT.Empty,
            UIA_PROPERTY_ID.UIA_LegacyIAccessibleNamePropertyId => !string.IsNullOrEmpty(Name) ? (VARIANT)Name : VARIANT.Empty,
            UIA_PROPERTY_ID.UIA_LegacyIAccessibleRolePropertyId => (VARIANT)(int)Role,
            UIA_PROPERTY_ID.UIA_LegacyIAccessibleStatePropertyId => (VARIANT)(int)State,
            UIA_PROPERTY_ID.UIA_NamePropertyId => Name is null ? VARIANT.Empty : (VARIANT)Name,
            UIA_PROPERTY_ID.UIA_RuntimeIdPropertyId => (VARIANT)new SafeArrayScope<int>(RuntimeId),
            UIA_PROPERTY_ID.UIA_SelectionCanSelectMultiplePropertyId => (VARIANT)CanSelectMultiple,
            UIA_PROPERTY_ID.UIA_SelectionIsSelectionRequiredPropertyId => (VARIANT)IsSelectionRequired,
            UIA_PROPERTY_ID.UIA_ValueValuePropertyId => !string.IsNullOrEmpty(Value) ? (VARIANT)Value : VARIANT.Empty,
            _ => VARIANT.Empty
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

    /// <summary>
    ///  Gets the child accessible object ID.
    /// </summary>
    /// <returns>The child accessible object ID.</returns>
    internal virtual int GetChildId() => (int)PInvoke.CHILDID_SELF;

    /// <summary>
    ///  Returns the element in the specified <paramref name="direction"/>.
    /// </summary>
    /// <param name="direction">Indicates the direction in which to navigate.</param>
    /// <returns>The element in the specified direction if it exists.</returns>
    internal virtual IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction) => null;

    internal virtual IRawElementProviderSimple.Interface[]? GetEmbeddedFragmentRoots() => null;

    internal virtual void SetFocus()
    {
    }

    internal virtual Rectangle BoundingRectangle => Bounds;

    /// <summary>
    ///  Gets the top level element.
    /// </summary>
    internal virtual UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => null;

    /// <summary>
    ///  Return the child object at the given screen coordinates.
    /// </summary>
    /// <param name="x">X coordinate.</param>
    /// <param name="y">Y coordinate.</param>
    /// <returns>The accessible object of corresponding element in the provided coordinates.</returns>
    internal virtual IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y) => this;

    internal virtual IRawElementProviderFragment.Interface? GetFocus() => null;

    internal virtual void Expand()
    {
    }

    internal virtual void Collapse()
    {
    }

    internal virtual UIA.ExpandCollapseState ExpandCollapseState => UIA.ExpandCollapseState.ExpandCollapseState_Collapsed;

    internal virtual void Toggle()
    {
    }

    internal virtual ToggleState ToggleState => ToggleState.ToggleState_Indeterminate;

    private protected virtual UiaCore.IRawElementProviderFragmentRoot? ToolStripFragmentRoot => null;

    internal virtual IRawElementProviderSimple.Interface[]? GetRowHeaders() => null;

    internal virtual IRawElementProviderSimple.Interface[]? GetColumnHeaders() => null;

    internal virtual RowOrColumnMajor RowOrColumnMajor => RowOrColumnMajor.RowOrColumnMajor_RowMajor;

    internal virtual IRawElementProviderSimple.Interface[]? GetRowHeaderItems() => null;

    internal virtual IRawElementProviderSimple.Interface[]? GetColumnHeaderItems() => null;

    internal virtual IRawElementProviderSimple.Interface? GetItem(int row, int column) => null;

    internal virtual int RowCount => -1;

    internal virtual int ColumnCount => -1;

    internal virtual int Row => -1;

    internal virtual int Column => -1;

    internal virtual int RowSpan => 1;

    internal virtual int ColumnSpan => 1;

    internal virtual IRawElementProviderSimple.Interface? ContainingGrid => null;

    internal virtual void Invoke() => DoDefaultAction();

    internal virtual ITextRangeProvider* DocumentRangeInternal
    {
        get
        {
            Debug.Fail("Not implemented. DocumentRangeInternal property should be overridden.");
            return null;
        }
    }

    internal virtual HRESULT GetTextSelection(SAFEARRAY** pRetVal) => HRESULT.E_NOTIMPL;

    internal virtual HRESULT GetTextVisibleRanges(SAFEARRAY** pRetVal) => HRESULT.E_NOTIMPL;

    internal virtual HRESULT GetTextRangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal) => HRESULT.E_NOTIMPL;

    internal virtual HRESULT GetTextRangeFromPoint(UiaPoint screenLocation, ITextRangeProvider** pRetVal) => HRESULT.E_NOTIMPL;

    internal virtual SupportedTextSelection SupportedTextSelectionInternal
    {
        get
        {
            Debug.Fail("Not implemented. SupportedTextSelectionInternal property should be overridden.");
            return SupportedTextSelection.SupportedTextSelection_None;
        }
    }

    internal virtual HRESULT GetTextCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal) => HRESULT.E_NOTIMPL;

    internal virtual HRESULT GetRangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal) => HRESULT.E_NOTIMPL;

    internal virtual bool IsReadOnly => false;

    internal virtual void SetValue(string? newValue)
    {
        Value = newValue;
    }

    internal virtual IRawElementProviderSimple.Interface? GetOverrideProviderForHwnd(IntPtr hwnd) => null;

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

    internal virtual IRawElementProviderSimple.Interface[]? GetSelection() => null;

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

    internal virtual IRawElementProviderSimple.Interface? ItemSelectionContainer => null;

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

    unsafe HRESULT ComIServiceProvider.Interface.QueryService(Guid* service, Guid* riid, void** ppvObject)
    {
        if (service is null || riid is null)
        {
            return HRESULT.E_NOINTERFACE;
        }

        if (ppvObject is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsIAccessibleExSupported())
        {
            Guid IID_IAccessibleEx = typeof(UiaCore.IAccessibleEx).GUID;
            if (service->Equals(IID_IAccessibleEx) && riid->Equals(IID_IAccessibleEx))
            {
                // We want to return the internal, secure, object, which we don't have access here
                // Return non-null, which will be interpreted in internal method, to mean returning casted object to IAccessibleEx
                *ppvObject = (void*)Marshal.GetComInterfaceForObject(this, typeof(UiaCore.IAccessibleEx));
                return HRESULT.S_OK;
            }
        }

        return HRESULT.E_NOINTERFACE;
    }

    UiaCore.IAccessibleEx? UiaCore.IAccessibleEx.GetObjectForChild(int idChild) => null;

    unsafe HRESULT UiaCore.IAccessibleEx.GetIAccessiblePair(out object? ppAcc, int* pidChild)
    {
        if (pidChild is null)
        {
            ppAcc = null;
            return HRESULT.E_INVALIDARG;
        }

        ppAcc = this;
        *pidChild = (int)PInvoke.CHILDID_SELF;
        return HRESULT.S_OK;
    }

    int[]? UiaCore.IAccessibleEx.GetRuntimeId() => RuntimeId;

    unsafe HRESULT UiaCore.IAccessibleEx.ConvertReturnedElement(IRawElementProviderSimple.Interface pIn, IntPtr* ppRetValOut)
    {
        if (ppRetValOut == null)
        {
            return HRESULT.E_POINTER;
        }

        // No need to implement this for patterns and properties
        *ppRetValOut = IntPtr.Zero;
        return HRESULT.E_NOTIMPL;
    }

    HRESULT IRawElementProviderSimple.Interface.get_ProviderOptions(ProviderOptions* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = (ProviderOptions)ProviderOptions;
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderSimple.Interface.get_HostRawElementProvider(IRawElementProviderSimple** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = HostRawElementProvider;
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderSimple.Interface.GetPatternProvider(UIA_PATTERN_ID patternId, IUnknown** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = null;

        if (IsPatternSupported(patternId))
        {
            *pRetVal = ComHelpers.GetComPointer<IUnknown>(this);
        }

        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderSimple.Interface.GetPropertyValue(UIA_PROPERTY_ID propertyId, VARIANT* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        VARIANT result = GetPropertyValue(propertyId);

#if DEBUG
        if (propertyId == UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId)
        {
            Debug.Assert(result.IsEmpty || result.vt == VARENUM.VT_I4);
        }
#endif

        *pRetVal = result;
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderFragment.Interface.Navigate(NavigateDirection direction, IRawElementProviderFragment** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderFragment.Interface? fragment = FragmentNavigate(direction);
        *pRetVal = ComHelpers.TryGetComPointer<IRawElementProviderFragment>(fragment);
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderFragment.Interface.GetRuntimeId(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = new SafeArrayScope<int>(RuntimeId);
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderFragment.Interface.get_BoundingRectangle(UiaRect* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = new()
        {
            left = BoundingRectangle.Left,
            top = BoundingRectangle.Top,
            height = BoundingRectangle.Height,
            width = BoundingRectangle.Width
        };

        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderFragment.Interface.GetEmbeddedFragmentRoots(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderSimple.Interface[]? fragmentRoots = GetEmbeddedFragmentRoots();
        if (fragmentRoots is null)
        {
            *pRetVal = default;
            return HRESULT.S_OK;
        }

        ComSafeArrayScope<IRawElementProviderSimple> scope = new((uint)fragmentRoots.Length);
        for (int i = 0; i < fragmentRoots.Length; i++)
        {
            scope[i] = ComHelpers.GetComPointer<IRawElementProviderSimple>(fragmentRoots[i]);
        }

        *pRetVal = scope;
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderFragment.Interface.SetFocus()
    {
        SetFocus();
        return HRESULT.S_OK;
    }

    // An accessible object should provide info about its correct root object,
    // even its owner is used like a ToolStrip item via ToolStripControlHost.
    // This change was made here to not to rework FragmentRoot implementations
    // for all accessible object. Moreover, this change will work for new accessible object
    // classes, where it is enough to implement FragmentRoot for a common case.
    HRESULT IRawElementProviderFragment.Interface.get_FragmentRoot(IRawElementProviderFragmentRoot** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ToolStripFragmentRoot is not null
                ? ComHelpers.GetComPointer<IRawElementProviderFragmentRoot>(ToolStripFragmentRoot)
                : ComHelpers.TryGetComPointer<IRawElementProviderFragmentRoot>(FragmentRoot);
        return HRESULT.S_OK;
    }

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

    HRESULT UiaCore.ILegacyIAccessibleProvider.GetIAccessible(UIA.IAccessible** ppAccessible)
    {
        if (ppAccessible is null)
        {
            return HRESULT.E_POINTER;
        }

        if (_isSystemWrapper)
        {
            // If all we were doing was wrapping a provided IAccessible, there is no need to marshal
            // our wrapper.
            *ppAccessible = SystemIAccessible is { } accessible
                ? accessible.GetInterface().Value
                : null;
        }
        else
        {
            // Ideally we'll implement UIA.IAccessible directly on this class. Currently there is a [ComImport]
            // on the CsWin32 generated interface so we'll need to figure out how collisions are handled or if
            // we need a feature from CsWin32 to skip the attribute.
            *ppAccessible = (UIA.IAccessible*)Marshal.GetComInterfaceForObject<AccessibleObject, IAccessible>(this);
        }

        return HRESULT.S_OK;
    }

    IRawElementProviderSimple.Interface[] UiaCore.ILegacyIAccessibleProvider.GetSelection()
        => GetSelected() is IRawElementProviderSimple.Interface selected
            ? [selected]
            : [];

    void UiaCore.ILegacyIAccessibleProvider.Select(int flagsSelect) => Select((AccessibleSelection)flagsSelect);

    void UiaCore.ILegacyIAccessibleProvider.SetValue(string szValue) => SetValue(szValue);

    void UiaCore.IExpandCollapseProvider.Expand() => Expand();

    void UiaCore.IExpandCollapseProvider.Collapse() => Collapse();

    UIA.ExpandCollapseState UiaCore.IExpandCollapseProvider.ExpandCollapseState => ExpandCollapseState;

    void UiaCore.IInvokeProvider.Invoke() => Invoke();

    ITextRangeProvider* ITextProvider.Interface.DocumentRange => DocumentRangeInternal;

    HRESULT ITextProvider.Interface.GetSelection(SAFEARRAY** pRetVal) => GetTextSelection(pRetVal);

    HRESULT ITextProvider.Interface.GetVisibleRanges(SAFEARRAY** pRetVal) => GetTextVisibleRanges(pRetVal);

    HRESULT ITextProvider.Interface.RangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal)
        => GetTextRangeFromChild(childElement, pRetVal);

    HRESULT ITextProvider.Interface.RangeFromPoint(UiaPoint point, ITextRangeProvider** pRetVal)
        => GetTextRangeFromPoint(point, pRetVal);

    SupportedTextSelection ITextProvider.Interface.SupportedTextSelection => SupportedTextSelectionInternal;

    ITextRangeProvider* ITextProvider2.Interface.DocumentRange => DocumentRangeInternal;

    HRESULT ITextProvider2.Interface.GetSelection(SAFEARRAY** pRetVal) => GetTextSelection(pRetVal);

    HRESULT ITextProvider2.Interface.GetVisibleRanges(SAFEARRAY** pRetVal) => GetTextVisibleRanges(pRetVal);

    HRESULT ITextProvider2.Interface.RangeFromChild(IRawElementProviderSimple* childElement, ITextRangeProvider** pRetVal)
        => GetTextRangeFromChild(childElement, pRetVal);

    HRESULT ITextProvider2.Interface.RangeFromPoint(UiaPoint point, ITextRangeProvider** pRetVal)
        => GetTextRangeFromPoint(point, pRetVal);

    SupportedTextSelection ITextProvider2.Interface.SupportedTextSelection => SupportedTextSelectionInternal;

    HRESULT ITextProvider2.Interface.GetCaretRange(BOOL* isActive, ITextRangeProvider** pRetVal) => GetTextCaretRange(isActive, pRetVal);

    HRESULT ITextProvider2.Interface.RangeFromAnnotation(IRawElementProviderSimple* annotationElement, ITextRangeProvider** pRetVal)
        => GetRangeFromAnnotation(annotationElement, pRetVal);

    BOOL UiaCore.IValueProvider.IsReadOnly => IsReadOnly ? true : false;

    string? UiaCore.IValueProvider.Value => Value;

    void UiaCore.IValueProvider.SetValue(string? newValue) => SetValue(newValue);

    void UiaCore.IToggleProvider.Toggle() => Toggle();

    ToggleState UiaCore.IToggleProvider.ToggleState => ToggleState;

    object[]? UiaCore.ITableProvider.GetRowHeaders() => GetRowHeaders();

    object[]? UiaCore.ITableProvider.GetColumnHeaders() => GetColumnHeaders();

    RowOrColumnMajor UiaCore.ITableProvider.RowOrColumnMajor => RowOrColumnMajor;

    object[]? UiaCore.ITableItemProvider.GetRowHeaderItems() => GetRowHeaderItems();

    object[]? UiaCore.ITableItemProvider.GetColumnHeaderItems() => GetColumnHeaderItems();

    object? UiaCore.IGridProvider.GetItem(int row, int column) => GetItem(row, column);

    int UiaCore.IGridProvider.RowCount => RowCount;

    int UiaCore.IGridProvider.ColumnCount => ColumnCount;

    int UiaCore.IGridItemProvider.Row => Row;

    int UiaCore.IGridItemProvider.Column => Column;

    int UiaCore.IGridItemProvider.RowSpan => RowSpan;

    int UiaCore.IGridItemProvider.ColumnSpan => ColumnSpan;

    IRawElementProviderSimple.Interface? UiaCore.IGridItemProvider.ContainingGrid => ContainingGrid;

    void IAccessible.accDoDefaultAction(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            Debug.WriteLineIf(
                CompModSwitches.MSAA.TraceInfo,
                $"AccessibleObject.AccDoDefaultAction: this = {ToString()}, childID = {childID}");

            // If the default action is to be performed on self, do it.
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->accDoDefaultAction(ChildIdToVARIANT(childID));
    }

    private VARIANT ChildIdToVARIANT(object childId)
    {
        if (childId is int integer)
        {
            return (VARIANT)integer;
        }

        if (childId is null)
        {
            return default;
        }

        IDispatch* dispatch = ComHelpers.TryGetComPointer<IDispatch>(childId);
        if (dispatch is not null)
        {
            return new VARIANT()
            {
                vt = VARENUM.VT_DISPATCH,
                data = new() { pdispVal = dispatch }
            };
        }

        Debug.Fail($"{nameof(ChildIdToVARIANT)} got {childId.GetType().Name}");
        return default;
    }

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
                return AsChildId(obj);
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return null;
        }

        result = accessible.Value->accHitTest(xLeft, yTop, out VARIANT child);
        return result.Failed ? null : child.ToObject();
    }

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
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            pxLeft = pyTop = pcxWidth = pcyHeight = 0;
            return;
        }

        result = accessible.Value->accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, ChildIdToVARIANT(childID));
    }

    object? IAccessible.accNavigate(int navDir, object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            Debug.WriteLineIf(
                CompModSwitches.MSAA.TraceInfo,
                $"AccessibleObject.AccNavigate: this = {ToString()}, navdir = {navDir}, childID = {childID}");

            // Use the Navigate function's return value if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
            {
                AccessibleObject? newObject = Navigate((AccessibleNavigation)navDir);
                if (newObject is not null)
                {
                    return AsChildId(newObject);
                }
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(childID);
            if (child is not null)
            {
                return AsChildId(child.Navigate((AccessibleNavigation)navDir));
            }
        }

        if (SysNavigate((AccessibleNavigation)navDir, childID, out AccessibleObject? accessibleObject))
        {
            return AsChildId(accessibleObject);
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return null;
        }

        result = accessible.Value->accNavigate(navDir, ChildIdToVARIANT(childID), out VARIANT endUpAt);
        return endUpAt.ToObject();
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
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->accSelect(flagsSelect, ChildIdToVARIANT(childID));
    }

    /// <summary>
    ///  Performs the default action associated with this accessible object.
    /// </summary>
    public virtual void DoDefaultAction()
    {
        // By default, does the system default action if available.
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return;
        }

        result = accessible.Value->accDoDefaultAction((VARIANT)(int)PInvoke.CHILDID_SELF);
    }

    object? IAccessible.get_accChild(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            Debug.WriteLineIf(
                CompModSwitches.MSAA.TraceInfo,
                $"AccessibleObject.GetAccChild: this = {ToString()}, childID = {childID}");

            // Return self for CHILDID_SELF.
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

                return child == this ? null : AsIAccessible(child);
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return null;
        }

        result = accessible.Value->get_accChildCount(out int count);

        if (result.Failed || count == 0)
        {
            return null;
        }

        IDispatch* dispatch;
        result = accessible.Value->get_accChild(ChildIdToVARIANT(childID), &dispatch);

        return result.Failed || dispatch is null
            ? null
            : new VARIANT()
            {
                vt = VARENUM.VT_DISPATCH,
                data = new() { pdispVal = dispatch }
            }.ToObject();
    }

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
                using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
                if (result.Failed)
                {
                    childCount = 0;
                }
                else
                {
                    result = accessible.Value->get_accChildCount(out childCount);
                }
            }

            Debug.WriteLineIf(
                CompModSwitches.MSAA.TraceInfo,
                $"AccessibleObject.accChildCount: this = {ToString()}, returning {childCount}");

            return childCount;
        }
    }

    string? IAccessible.get_accDefaultAction(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            // Return the default action property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        return SystemIAccessible.TryGetDefaultAction(ChildIdToVARIANT(childID));
    }

    string? IAccessible.get_accDescription(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            // Return the description property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        return SystemIAccessible.TryGetDescription(ChildIdToVARIANT(childID))?.ToString();
    }

    /// <summary>
    ///  Returns the appropriate child from the Accessible Child Collection, if available.
    /// </summary>
    private AccessibleObject? GetAccessibleChild(object childID)
    {
        if (!childID.Equals((int)PInvoke.CHILDID_SELF))
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
                    return AsChildId(obj);
                }
            }

            return TryGetFocus();
        }
    }

    string? IAccessible.get_accHelp(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        return SystemIAccessible.TryGetHelp(ChildIdToVARIANT(childID));
    }

    int IAccessible.get_accHelpTopic(out string? pszHelpFile, object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        return SystemIAccessible.TryGetHelpTopic(ChildIdToVARIANT(childID), out pszHelpFile);
    }

    string? IAccessible.get_accKeyboardShortcut(object childID) => get_accKeyboardShortcutInternal(childID);

    internal virtual string? get_accKeyboardShortcutInternal(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        return SystemIAccessible.TryGetKeyboardShortcut(ChildIdToVARIANT(childID));
    }

    string? IAccessible.get_accName(object childID) => get_accNameInternal(childID);

    internal virtual string? get_accNameInternal(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            Debug.WriteLineIf(
                CompModSwitches.MSAA.TraceInfo,
                $"AccessibleObject.get_accName: this = {ToString()}, childID = {childID}");

            // Return the name property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        string? retval = SystemIAccessible.TryGetName(ChildIdToVARIANT(childID));

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

    object? IAccessible.get_accRole(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            // Return the role property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        int count = SystemIAccessible.TryGetChildCount();

        // Unclear why this returns null for no children.
        return count == 0 ? null : SystemIAccessible.TryGetRole(ChildIdToVARIANT(childID));
    }

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
                    return AsChildId(obj);
                }
            }

            using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
            if (result.Failed)
            {
                return null;
            }

            result = accessible.Value->get_accSelection(out VARIANT selection);
            return selection.ToObject();
        }
    }

    object? IAccessible.get_accState(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            Debug.WriteLineIf(
                CompModSwitches.MSAA.TraceInfo,
                $"AccessibleObject.GetAccState: this = {ToString()}, childID = {childID}");

            // Return the state property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        // Perhaps would be better to return AccessibleStates.None instead of null here.
        return SystemIAccessible?.TryGetState(ChildIdToVARIANT(childID));
    }

    string? IAccessible.get_accValue(object childID)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            // Return the value property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        return SystemIAccessible.TryGetValue(ChildIdToVARIANT(childID));
    }

    void IAccessible.set_accName(object childID, string newName)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            // Set the name property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        SystemIAccessible.TrySetName(ChildIdToVARIANT(childID), newName);
    }

    void IAccessible.set_accValue(object childID, string newValue)
    {
        if (IsClientObject)
        {
            ValidateChildID(ref childID);

            // Set the value property if available
            if (childID.Equals((int)PInvoke.CHILDID_SELF))
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

        SystemIAccessible.TrySetValue(ChildIdToVARIANT(childID), newValue);
    }

    /// <inheritdoc cref="IOleWindow.GetWindow(HWND*)"/>
    /// <devdoc>
    ///  <para>
    ///   Now that AccessibleObject is used to wrap all system-provided (OLEACC.DLL) accessible
    ///   objects, it needs to implement IOleWindow and pass this down to the inner object. This is
    ///   necessary because the OS function WindowFromAccessibleObject() walks up the parent chain
    ///   looking for the first object that implements IOleWindow, and uses that to get the hwnd.
    ///  </para>
    ///  <para>
    ///   But this creates a new problem for AccessibleObjects that do NOT have windows, ie. which
    ///   represent simple elements. To the OS, these simple elements will now appear to implement
    ///   IOleWindow, so it will try to get hwnds from them - which they simply cannot provide.
    ///  </para>
    ///  <para>
    ///   To work around this problem, the AccessibleObject for a simple element will delegate all
    ///   IOleWindow calls up the parent chain itself. This will stop at the first window-based
    ///   accessible object, which will be able to return an hwnd back to the OS. So we are
    ///   effectively 'preempting' what WindowFromAccessibleObject() would do.
    ///  </para>
    /// </devdoc>
    HRESULT IOleWindow.Interface.GetWindow(HWND* phwnd)
    {
        if (phwnd is null)
        {
            return HRESULT.E_POINTER;
        }

        // See if we have an inner object that can provide the window handle
        using var oleWindow = TryGetOleWindow(out HRESULT result);
        if (result.Succeeded)
        {
            return oleWindow.Value->GetWindow(phwnd);
        }

        // Otherwise delegate to the parent object
        AccessibleObject? parent = Parent;
        if (parent is IOleWindow.Interface parentWindow)
        {
            return parentWindow.GetWindow(phwnd);
        }

        // Or fail if there is no parent
        *phwnd = HWND.Null;
        return HRESULT.E_FAIL;
    }

    /// <inheritdoc cref="IOleWindow.ContextSensitiveHelp(BOOL)"/>
    /// <devdoc>
    ///  See GetWindow() above for further details.
    /// </devdoc>
    HRESULT IOleWindow.Interface.ContextSensitiveHelp(BOOL fEnterMode)
    {
        // See if we have an inner object that can provide help
        using var oleWindow = TryGetOleWindow(out HRESULT result);
        if (result.Succeeded)
        {
            return oleWindow.Value->ContextSensitiveHelp(fEnterMode);
        }

        // Otherwise delegate to the parent object
        AccessibleObject? parent = Parent;
        if (parent is IOleWindow.Interface parentWindow)
        {
            return parentWindow.ContextSensitiveHelp(fEnterMode);
        }

        // Or do nothing if there is no parent
        return HRESULT.S_OK;
    }

    HRESULT IEnumVARIANT.Interface.Clone(IEnumVARIANT** ppEnum) => EnumVariant.Clone(ppEnum);

    HRESULT IEnumVARIANT.Interface.Next(uint celt, VARIANT* rgVar, uint* pCeltFetched)
        => EnumVariant.Next(celt, rgVar, pCeltFetched);

    HRESULT IEnumVARIANT.Interface.Reset() => EnumVariant.Reset();

    HRESULT IEnumVARIANT.Interface.Skip(uint celt) => EnumVariant.Skip(celt);

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

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return null;
        }

        if (SysNavigate(navdir, (int)PInvoke.CHILDID_SELF, out AccessibleObject? accessibleObject))
        {
            return accessibleObject;
        }
        else
        {
            result = accessible.Value->accNavigate((int)navdir, CHILDID_SELF, out VARIANT endUpAt);
            return TryGetAccessibleObject(endUpAt);
        }
    }

    /// <summary>
    ///  Selects this accessible object.
    /// </summary>
    public virtual void Select(AccessibleSelection flags)
    {
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Succeeded)
        {
            result = accessible.Value->accSelect((int)flags, CHILDID_SELF);
        }
    }

    private object? AsChildId(AccessibleObject? obj)
    {
        // https://learn.microsoft.com/windows/win32/winauto/how-child-ids-are-used-in-parameters
        if (obj == this)
        {
            return (int)PInvoke.CHILDID_SELF;
        }

        return AsIAccessible(obj);
    }

    private static object? AsIAccessible(AccessibleObject? obj)
    {
        if (obj is not null && obj._isSystemWrapper && obj.SystemIAccessible is { } accessible)
        {
            // We're just a simple system wrapper, return the pointer.
            return new VARIANT()
            {
                vt = VARENUM.VT_DISPATCH,
                data = new() { pdispVal = accessible.GetInterface<IDispatch>().Value }
            }.ToObject();
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
    internal int AccessibleObjectId { get; set; } = (int)OBJECT_IDENTIFIER.OBJID_CLIENT;

    /// <summary>
    ///  Indicates whether this accessible object represents the client area of
    ///  the window.
    /// </summary>
    internal bool IsClientObject => AccessibleObjectId == (int)OBJECT_IDENTIFIER.OBJID_CLIENT;

    /// <summary>
    ///  Indicates whether this accessible object represents the non-client
    ///  area of the window.
    /// </summary>
    internal bool IsNonClientObject => AccessibleObjectId == (int)OBJECT_IDENTIFIER.OBJID_WINDOW;

    protected void UseStdAccessibleObjects(IntPtr handle)
    {
        UseStdAccessibleObjects(handle, AccessibleObjectId);
    }

    protected unsafe void UseStdAccessibleObjects(IntPtr handle, int objid)
    {
        UIA.IAccessible* accessible = null;
        HRESULT result = PInvoke.CreateStdAccessibleObject(
            (HWND)handle,
            objid,
            IID.Get<UIA.IAccessible>(),
            (void**)&accessible);

        if (result.Succeeded && accessible is not null)
        {
#if DEBUG
            // AccessibleObject is not set up for deterministic disposal (we can't create them in a using block),
            // As such, these handles will always be finalized.
            SystemIAccessible = new(accessible, takeOwnership: true, trackDisposal: false);
#else
            SystemIAccessible = new(accessible, takeOwnership: true);
#endif
            IOleWindow* window = null;
            result = accessible->QueryInterface(IID.Get<IOleWindow>(), (void**)&window);
            if (result.Succeeded && window is not null)
            {
#if DEBUG
                _systemIOleWindow = new(window, takeOwnership: true, trackDisposal: false);
#else
                _systemIOleWindow = new(window, takeOwnership: true);
#endif
            }
        }

        IEnumVARIANT* enumVariant = null;
        result = PInvoke.CreateStdAccessibleObject(
            (HWND)handle,
            objid,
            IID.Get<IEnumVARIANT>(),
            (void**)&enumVariant);

        if (result.Succeeded && enumVariant is not null)
        {
#if DEBUG
            _systemIEnumVariant = new(enumVariant, takeOwnership: true, trackDisposal: false);
#else
            _systemIEnumVariant = new(enumVariant, takeOwnership: true);
#endif
        }

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Performs custom navigation between parent, child, and sibling accessible objects.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is basically just a wrapper for <see cref="GetSysChild(AccessibleNavigation, out AccessibleObject?)"/>
    ///   that does some of the dirty work. Usage is similar to <see cref="GetSysChild(AccessibleNavigation, out AccessibleObject?)"/>.
    ///   Called prior to calling <see cref="UIA.IAccessible.get_accName(VARIANT, BSTR*)"/> on the 'inner' system
    ///   accessible object.
    ///  </para>
    /// </remarks>
    private bool SysNavigate(AccessibleNavigation direction, object childID, out AccessibleObject? accessibleObject)
    {
        accessibleObject = null;

        // Only override system navigation relative to ourselves (since we can't interpret OLEACC child ids)
        if (!childID.Equals((int)PInvoke.CHILDID_SELF))
        {
            return false;
        }

        // Perform any supported navigation operation (fall back on system for unsupported navigation ops)
        return GetSysChild(direction, out accessibleObject);
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
            childID = (int)PInvoke.CHILDID_SELF;
        }
        else if (childID.Equals((int)HRESULT.DISP_E_PARAMNOTFOUND))
        {
            childID = 0;
        }
        else if (childID is not int)
        {
            // AccExplorer seems to occasionally pass in objects instead of an int ChildID.
            childID = 0;
        }
    }

    private AccessibleObject? TryGetAccessibleObject(VARIANT variant)
    {
        switch (variant.vt)
        {
            case VARENUM.VT_I4:
                int id = variant.data.lVal;
                if (id == (int)PInvoke.CHILDID_SELF)
                {
                    return this;
                }
                else
                {
                    // We never handled this case before. If it comes up should we make the call to get
                    // the actual child object from the id?
                    Debug.Fail($"{nameof(TryGetAccessibleObject)} got a child id of {id}");
                    return null;
                }

            case VARENUM.VT_DISPATCH:
                return TryGetAccessibleObject(variant.data.pdispVal);
            case VARENUM.VT_UNKNOWN:
                // Another thing we haven't handled. This is a multi-selection returned as IEnumVARIANT.
                Debug.Fail($"{nameof(TryGetAccessibleObject)} got a multi selection");
                variant.Dispose();
                return null;
            case VARENUM.VT_EMPTY:
            default:
                variant.Dispose();
                return null;
        }
    }

    private AccessibleObject? TryGetAccessibleObject(IDispatch* dispatch)
    {
        if (dispatch is null)
        {
            return null;
        }

        UIA.IAccessible* accessible;
        if (dispatch->QueryInterface(IID.Get<UIA.IAccessible>(), (void**)&accessible).Failed)
        {
            Debug.Fail("This should never happen");
            dispatch->Release();
            return null;
        }

        dispatch->Release();
        return TryGetAccessibleObject(accessible);
    }

    private AccessibleObject? TryGetAccessibleObject(UIA.IAccessible* accessible)
    {
        if (accessible is null)
        {
            return null;
        }

        AgileComPointer<UIA.IAccessible> agileAccessible =
#if DEBUG
            // AccessibleObject is not disposable so we shouldn't be tracking it.
            new(accessible, takeOwnership: true, trackDisposal: false);
#else
            new(accessible, takeOwnership: true);
#endif
        // Check to see if this object already wraps the given pointer.
        if (SystemIAccessible is { } systemAccessible
            && systemAccessible.IsSameNativeObject(agileAccessible))
        {
            agileAccessible.Dispose();
            return this;
        }

        return new AccessibleObject(agileAccessible);
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder? binder, Type[] types, ParameterModifier[]? modifiers)
        => typeof(IAccessible).GetMethod(name, bindingAttr, binder, types, modifiers);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    MethodInfo? IReflect.GetMethod(string name, BindingFlags bindingAttr)
        => typeof(IAccessible).GetMethod(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)]
    MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
        => typeof(IAccessible).GetMethods(bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
    FieldInfo? IReflect.GetField(string name, BindingFlags bindingAttr)
        => typeof(IAccessible).GetField(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
    FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
        => typeof(IAccessible).GetFields(bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    PropertyInfo? IReflect.GetProperty(string name, BindingFlags bindingAttr)
        => typeof(IAccessible).GetProperty(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    PropertyInfo? IReflect.GetProperty(
        string name,
        BindingFlags bindingAttr,
        Binder? binder,
        Type? returnType,
        Type[] types,
        ParameterModifier[]? modifiers)
        => typeof(IAccessible).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)]
    PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
        => typeof(IAccessible).GetProperties(bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields |
        DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods |
        DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents |
        DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties |
        DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors |
        DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
    MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
        => typeof(IAccessible).GetMember(name, bindingAttr);

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields |
        DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods |
        DynamicallyAccessedMemberTypes.PublicEvents | DynamicallyAccessedMemberTypes.NonPublicEvents |
        DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties |
        DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors |
        DynamicallyAccessedMemberTypes.PublicNestedTypes | DynamicallyAccessedMemberTypes.NonPublicNestedTypes)]
    MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
        => typeof(IAccessible).GetMembers(bindingAttr);

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
        // In the interface definition there are parameters that take an argument such as `accChild`. In IL this is
        // defined as follows:
        //
        //   .property object accChild(object)
        //   {
        //      .custom instance void [INTEROP_ASSEMBLY]DispIdAttribute::.ctor(int32) = (01 00 76 EC FF FF 00 00 )
        //      .get instance object Accessibility.IAccessible::get_accChild(object)
        //   }
        //
        // This isn't representable in C#. `object get_accChild(object)` is defined as well and will be returned when
        // asking for the `.GetGetMethod` for the `accChild` property when reflecting through members.
        //
        // Here we try to assume that the caller intended to indicate CHILDID_SELF if they pass no parameters for these
        // cases. Rather than walk through everything here as we've done historically, just hard-code the known cases.
        //
        // It was never documented when this would happen. While you can see that there is a property with the name
        // `accChild` .NET Core only ever exposes the ITypeInfo for IUnknown.
        if (args?.Length == 0 && s_propertiesWithArguments.Contains(name))
        {
            args = new object[] { (int)PInvoke.CHILDID_SELF };
        }

        return typeof(IAccessible).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
    }

    Type IReflect.UnderlyingSystemType => typeof(IAccessible);

    IRawElementProviderSimple.Interface? UiaCore.IRawElementProviderHwndOverride.GetOverrideProviderForHwnd(IntPtr hwnd)
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

    IRawElementProviderSimple.Interface? UiaCore.ISelectionItemProvider.SelectionContainer => ItemSelectionContainer;

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
        string? notificationText)
        => CanNotifyClients
            && PInvoke.UiaRaiseNotificationEvent(
                this,
                notificationKind,
                notificationProcessing,
                notificationText).Succeeded;

    /// <summary>
    ///  Raises the LiveRegionChanged UIA event.
    ///  This method must be overridden in derived classes that support the UIA live region feature.
    /// </summary>
    /// <returns>True if operation succeeds, False otherwise.</returns>
    public virtual bool RaiseLiveRegionChanged()
    {
        throw new NotSupportedException(SR.AccessibleObjectLiveRegionNotSupported);
    }

    internal virtual bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
    {
        if (PInvoke.UiaClientsAreListening() && CanNotifyClients)
        {
            using var provider = ComHelpers.GetComScope<IRawElementProviderSimple>(this);
            HRESULT result = PInvoke.UiaRaiseAutomationEvent(provider, eventId);
            return result == HRESULT.S_OK;
        }

        return false;
    }

    internal virtual bool RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID propertyId, VARIANT oldValue, VARIANT newValue)
    {
        if (PInvoke.UiaClientsAreListening() && CanNotifyClients)
        {
            using var provider = ComHelpers.GetComScope<IRawElementProviderSimple>(this);
            HRESULT result = PInvoke.UiaRaiseAutomationPropertyChangedEvent(provider, propertyId, oldValue, newValue);
            return result == HRESULT.S_OK;
        }

        return false;
    }

    internal virtual bool InternalRaiseAutomationNotification(
        AutomationNotificationKind notificationKind,
        AutomationNotificationProcessing notificationProcessing,
        string notificationText)
        => PInvoke.UiaClientsAreListening()
            ? RaiseAutomationNotification(notificationKind, notificationProcessing, notificationText)
            : false;

    internal bool RaiseStructureChangedEvent(StructureChangeType structureChangeType, int[] runtimeId)
    {
        if (PInvoke.UiaClientsAreListening() && CanNotifyClients)
        {
            using var provider = ComHelpers.GetComScope<IRawElementProviderSimple>(this);
            int length = runtimeId.Length;
            HRESULT result = HRESULT.S_OK;
            if (length == 0)
            {
                result = PInvoke.UiaRaiseStructureChangedEvent(provider, structureChangeType, default, length);
            }
            else
            {
                fixed (int* pRuntimeId = &runtimeId[0])
                {
                    result = PInvoke.UiaRaiseStructureChangedEvent(provider, structureChangeType, pRuntimeId, length);
                }
            }

            return result == HRESULT.S_OK;
        }

        return false;
    }

    void UiaCore.IScrollItemProvider.ScrollIntoView() => ScrollIntoView();

    internal virtual void ScrollIntoView() => Debug.Fail($"{nameof(ScrollIntoView)}() is not overriden");

    private ComScope<IOleWindow> TryGetOleWindow(out HRESULT result)
    {
        if (_systemIOleWindow is { } agile)
        {
            return agile.TryGetInterface(out result);
        }

        result = HRESULT.E_NOINTERFACE;
        return default;
    }

    HRESULT UIA.IAccessible.Interface.get_accParent(IDispatch** ppdispParent) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accChildCount(int* pcountChildren) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accChild(VARIANT varChild, IDispatch** ppdispChild) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accName(VARIANT varChild, BSTR* pszName) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accValue(VARIANT varChild, BSTR* pszValue) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accDescription(VARIANT varChild, BSTR* pszDescription) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accRole(VARIANT varChild, VARIANT* pvarRole) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accState(VARIANT varChild, VARIANT* pvarState) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accHelp(VARIANT varChild, BSTR* pszHelp) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accHelpTopic(BSTR* pszHelpFile, VARIANT varChild, int* pidTopic) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accKeyboardShortcut(VARIANT varChild, BSTR* pszKeyboardShortcut) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accFocus(VARIANT* pvarChild) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accSelection(VARIANT* pvarChildren) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.get_accDefaultAction(VARIANT varChild, BSTR* pszDefaultAction) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.accSelect(int flagsSelect, VARIANT varChild) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, VARIANT varChild) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.accNavigate(int navDir, VARIANT varStart, VARIANT* pvarEndUpAt) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.accHitTest(int xLeft, int yTop, VARIANT* pvarChild) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.accDoDefaultAction(VARIANT varChild) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.put_accName(VARIANT varChild, BSTR szName) => HRESULT.E_NOTIMPL;
    HRESULT UIA.IAccessible.Interface.put_accValue(VARIANT varChild, BSTR szValue) => HRESULT.E_NOTIMPL;

    HRESULT IDispatch.Interface.GetTypeInfoCount(uint* pctinfo)
        => ((IDispatch.Interface)_dispatchAdapter).GetTypeInfoCount(pctinfo);

    HRESULT IDispatch.Interface.GetTypeInfo(uint iTInfo, uint lcid, ITypeInfo** ppTInfo)
        => ((IDispatch.Interface)_dispatchAdapter).GetTypeInfo(iTInfo, lcid, ppTInfo);

    HRESULT IDispatch.Interface.GetIDsOfNames(Guid* riid, PWSTR* rgszNames, uint cNames, uint lcid, int* rgDispId)
        => ((IDispatch.Interface) _dispatchAdapter).GetIDsOfNames(riid, rgszNames, cNames, lcid, rgDispId);

    HRESULT IDispatch.Interface.Invoke(
        int dispIdMember,
        Guid* riid,
        uint lcid,
        DISPATCH_FLAGS dwFlags,
        DISPPARAMS* pDispParams,
        VARIANT* pVarResult,
        EXCEPINFO* pExcepInfo,
        uint* pArgErr)
        => ((IDispatch.Interface)_dispatchAdapter).Invoke(dispIdMember, riid, lcid, dwFlags, pDispParams, pVarResult, pExcepInfo, pArgErr);

    HRESULT IDispatchEx.Interface.GetDispID(BSTR bstrName, uint grfdex, int* pid)
        => ((IDispatchEx.Interface)_dispatchAdapter).GetDispID(bstrName, grfdex, pid);

    HRESULT IDispatchEx.Interface.InvokeEx(
        int id,
        uint lcid,
        ushort wFlags,
        DISPPARAMS* pdp,
        VARIANT* pvarRes,
        EXCEPINFO* pei,
        ComIServiceProvider* pspCaller)
        => ((IDispatchEx.Interface)_dispatchAdapter).InvokeEx(id, lcid, wFlags, pdp, pvarRes, pei, pspCaller);

    HRESULT IDispatchEx.Interface.DeleteMemberByName(BSTR bstrName, uint grfdex)
        => ((IDispatchEx.Interface)_dispatchAdapter).DeleteMemberByName(bstrName, grfdex);

    HRESULT IDispatchEx.Interface.DeleteMemberByDispID(int id)
        => ((IDispatchEx.Interface)_dispatchAdapter).DeleteMemberByDispID(id);

    HRESULT IDispatchEx.Interface.GetMemberProperties(int id, uint grfdexFetch, FDEX_PROP_FLAGS* pgrfdex)
        => ((IDispatchEx.Interface)_dispatchAdapter).GetMemberProperties(id, grfdexFetch, pgrfdex);

    HRESULT IDispatchEx.Interface.GetMemberName(int id, BSTR* pbstrName)
        => ((IDispatchEx.Interface)_dispatchAdapter).GetMemberName(id, pbstrName);

    HRESULT IDispatchEx.Interface.GetNextDispID(uint grfdex, int id, int* pid)
        => ((IDispatchEx.Interface)_dispatchAdapter).GetNextDispID(grfdex, id, pid);

    HRESULT IDispatchEx.Interface.GetNameSpaceParent(IUnknown** ppunk)
        => ((IDispatchEx.Interface)_dispatchAdapter).GetNameSpaceParent(ppunk);
}
