// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Primitives;

using Windows.Win32.System.Com;
using Windows.Win32.System.Ole;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
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
    UIA.IAccessible.Interface,
    IAccessible,
    IAccessibleEx.Interface,
    ComIServiceProvider.Interface,
    IRawElementProviderSimple.Interface,
    IRawElementProviderFragment.Interface,
    IRawElementProviderFragmentRoot.Interface,
    IInvokeProvider.Interface,
    IValueProvider.Interface,
    IRangeValueProvider.Interface,
    IExpandCollapseProvider.Interface,
    IToggleProvider.Interface,
    ITableProvider.Interface,
    ITableItemProvider.Interface,
    IGridProvider.Interface,
    IGridItemProvider.Interface,
    IEnumVARIANT.Interface,
    IOleWindow.Interface,
    ILegacyIAccessibleProvider.Interface,
    ISelectionProvider.Interface,
    ISelectionItemProvider.Interface,
    IRawElementProviderHwndOverride.Interface,
    IScrollItemProvider.Interface,
    IMultipleViewProvider.Interface,
    ITextProvider.Interface,
    ITextProvider2.Interface,
    IDispatch.Interface,
    IDispatchEx.Interface,
    IManagedWrapper<
        IDispatch,
        IDispatchEx,
        UIA.IAccessible,
        IAccessibleEx,
        ComIServiceProvider,
        IRawElementProviderSimple,
        IRawElementProviderFragment,
        IRawElementProviderFragmentRoot,
        IInvokeProvider,
        IValueProvider,
        IRangeValueProvider,
        IExpandCollapseProvider,
        IToggleProvider,
        ITableProvider,
        ITableItemProvider,
        IGridProvider,
        IGridItemProvider,
        IEnumVARIANT,
        IOleWindow,
        ILegacyIAccessibleProvider,
        ISelectionProvider,
        ISelectionItemProvider,
        IRawElementProviderHwndOverride,
        IScrollItemProvider,
        IMultipleViewProvider,
        ITextProvider,
        ITextProvider2>
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

    private static readonly AccessibleObject s_parentFlag = new();
    private bool _inCallback;

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

    /// <summary>
    ///  Gets a description of the default action for an object.
    /// </summary>
    public virtual string? DefaultAction => GetDefaultActionInternal().ToNullableStringAndFree();

    /// <summary>
    ///  Determines if <see cref="GetDefaultActionInternal"/> can be called without calling <see cref="DefaultAction"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetDefaultActionInternal => IsInternal;

    /// <summary>
    ///  Unwraps the System IAccessible default action.
    /// </summary>
    internal virtual BSTR GetDefaultActionInternal() => SystemIAccessible.TryGetDefaultAction(CHILDID_SELF);

    /// <summary>
    ///  Gets a description of the object's visual appearance to the user.
    /// </summary>
    public virtual string? Description => GetDescriptionInternal().ToNullableStringAndFree();

    /// <summary>
    ///  Determines if <see cref="GetDescriptionInternal"/> can be called without calling <see cref="Description"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetDescriptionInternal => IsInternal;

    /// <summary>
    ///  Unwraps the System IAccessible description.
    /// </summary>
    internal virtual BSTR GetDescriptionInternal() => SystemIAccessible.TryGetDescription(CHILDID_SELF);

    private IEnumVARIANT.Interface EnumVariant => _enumVariant ??= new EnumVariantObject(this);

    /// <summary>
    ///  Gets a description of what the object does or how the object is used.
    /// </summary>
    public virtual string? Help => GetHelpInternal().ToNullableStringAndFree();

    /// <summary>
    ///  Determines if <see cref="GetHelpInternal"/> can be called without calling <see cref="Help"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetHelpInternal => IsInternal;

    /// <summary>
    ///  Unwraps the System IAccessible help.
    /// </summary>
    internal virtual BSTR GetHelpInternal() => SystemIAccessible.TryGetHelp(CHILDID_SELF);

    /// <summary>
    ///  Gets the object shortcut key or access key for an accessible object.
    /// </summary>
    public virtual string? KeyboardShortcut => GetKeyboardShortcutInternal(CHILDID_SELF).ToNullableStringAndFree();

    /// <summary>
    ///  Determines if <see cref="GetKeyboardShortcutInternal(VARIANT)"/> can be called without calling <see cref="KeyboardShortcut"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetKeyboardShortcutInternal => IsInternal;

    /// <summary>
    ///  Unwraps the System IAccessible keyboard shortcut of the <paramref name="childID"/>
    /// </summary>
    internal virtual BSTR GetKeyboardShortcutInternal(VARIANT childID) => SystemIAccessible.TryGetKeyboardShortcut(childID);

    /// <summary>
    ///  Gets or sets the object name.
    /// </summary>
    public virtual string? Name
    {
        get => GetNameInternal().ToNullableStringAndFree();
        set
        {
            using BSTR set = new(value);
            SetNameInternal(set);
        }
    }

    /// <summary>
    ///  Determines if <see cref="GetNameInternal"/> can be called without calling <see cref="Name"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetNameInternal => IsInternal;

    /// <summary>
    ///  Unwraps the System IAccessible name.
    /// </summary>
    internal virtual BSTR GetNameInternal() => SystemIAccessible.TryGetName(CHILDID_SELF);

    /// <summary>
    ///  Determines if <see cref="SetNameInternal(BSTR)"/> can be called without calling <see cref="Name"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanSetNameInternal => IsInternal;

    /// <summary>
    ///  Unwraps and sets the System IAccessible name to <paramref name="value"/>.
    /// </summary>
    internal virtual void SetNameInternal(BSTR value) => SystemIAccessible.TrySetName(CHILDID_SELF, value);

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
    ///  accessibility object standpoint).
    /// </devdoc>
    public virtual AccessibleObject? Parent =>
        _inCallback && IsInternal ? s_parentFlag : TryGetAccessibleObject(GetSystemAccessibleParent());

    /// <summary>
    ///  Unwraps and returns the system IAccessible.
    /// </summary>
    private IDispatch* GetSystemAccessibleParent()
    {
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Succeeded)
        {
            IDispatch* dispatch;
            result = accessible.Value->get_accParent(&dispatch);
            return dispatch;
        }

        return null;
    }

    /// <summary>
    ///  Determines whether or not this object is internal.
    /// </summary>
    private protected virtual bool IsInternal
    {
        get
        {
            RuntimeTypeHandle type = GetType().TypeHandle;
            return type.Equals(typeof(AccessibleObject).TypeHandle)
                || type.GetModuleHandle().Equals(typeof(AccessibleObject).TypeHandle.GetModuleHandle());
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
        get => GetValueInternal().ToNullableStringAndFree();
        set
        {
            using BSTR set = new(value);
            SetValueInternal(set);
        }
    }

    /// <summary>
    ///  Determines if <see cref="GetValueInternal"/> can be called without calling <see cref="Value"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetValueInternal => IsInternal;

    internal virtual BSTR GetValueInternal() => SystemIAccessible is null ? new(string.Empty) : SystemIAccessible.TryGetValue(CHILDID_SELF);

    /// <summary>
    ///  Determines if <see cref="SetValueInternal(BSTR)"/> can be called without calling <see cref="Value"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanSetValueInternal => IsInternal;

    /// <summary>
    ///  Unwraps and sets the System IAccessible value to <paramref name="value"/>.
    /// </summary>
    internal virtual void SetValueInternal(BSTR value) => SystemIAccessible.TrySetValue(CHILDID_SELF, value);

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
                if (child is not null && child.State.HasFlag(AccessibleStates.Focused))
                {
                    return child;
                }
            }

            return State.HasFlag(AccessibleStates.Focused) ? this : null;
        }

        return TryGetAccessibleObject(GetSystemIAccessibleFocus());
    }

    private VARIANT GetSystemIAccessibleFocus()
    {
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return VARIANT.Empty;
        }

        result = accessible.Value->get_accFocus(out VARIANT focus);
        if (result.Failed)
        {
            Debug.Assert(result == HRESULT.DISP_E_MEMBERNOTFOUND, $"{nameof(GetSystemIAccessibleFocus)} failed with {result}");
            return VARIANT.Empty;
        }

        return focus;
    }

    /// <summary>
    ///  Gets an identifier for a Help topic and the path to the Help file associated with this accessible object.
    /// </summary>
    public virtual int GetHelpTopic(out string? fileName)
    {
        (int topic, BSTR file) = SystemIAccessible.TryGetHelpTopic(CHILDID_SELF);
        fileName = file.ToNullableStringAndFree();
        return topic;
    }

    /// <summary>
    ///  Determines if <see cref="GetHelpTopicInternal"/> can be called without calling <see cref="GetHelpTopic(out string?)"/>
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This is an optimization to avoid unnecessary allocations when being called from native code
    ///  </para>
    /// </remarks>
    internal virtual bool CanGetHelpTopicInternal => IsInternal;

    /// <summary>
    ///  Unwraps the System IAccessible help topic string and ID.
    /// </summary>
    internal virtual (int topic, BSTR helpFile) GetHelpTopicInternal() => SystemIAccessible.TryGetHelpTopic(CHILDID_SELF);

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

        return TryGetAccessibleObject(TryGetSystemIAccessibleSelection());
    }

    private VARIANT TryGetSystemIAccessibleSelection()
    {
        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return VARIANT.Empty;
        }

        result = accessible.Value->get_accSelection(out VARIANT selection);
        if (result.Failed)
        {
            Debug.Assert(result == HRESULT.DISP_E_MEMBERNOTFOUND, $"{nameof(TryGetSystemIAccessibleSelection)} failed with {result}");
            return VARIANT.Empty;
        }

        return selection;
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
    ///  Gets the runtime identifier. This value must be unique within the parent window.
    /// </summary>
    internal virtual int[] RuntimeId
    {
        get
        {
            if (_isSystemWrapper)
            {
                return [RuntimeIDFirstItem, GetHashCode()];
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
    ///  Returns the value of the specified <paramref name="propertyID"/> from the
    ///  element in the form of a <see cref="VARIANT"/>. See
    ///  <see href="https://learn.microsoft.com/windows/win32/winauto/uiauto-automation-element-propids">
    ///   which outlines how the <see cref="VARIANT"/> should be defined for each <see cref="UIA_PROPERTY_ID"/>
    ///  </see>
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
            return Role switch
            {
                AccessibleRole.MenuItem
                    or AccessibleRole.Link
                    or AccessibleRole.PushButton
                    or AccessibleRole.ButtonDropDown
                    or AccessibleRole.ButtonMenu
                    or AccessibleRole.ButtonDropDownGrid
                    or AccessibleRole.Clock
                    or AccessibleRole.SplitButton
                    or AccessibleRole.CheckButton
                    or AccessibleRole.Cell
                    or AccessibleRole.ListItem => true,
                AccessibleRole.Default
                    or AccessibleRole.None
                    or AccessibleRole.Sound
                    or AccessibleRole.Cursor
                    or AccessibleRole.Caret
                    or AccessibleRole.Alert
                    or AccessibleRole.Client
                    or AccessibleRole.Chart
                    or AccessibleRole.Dialog
                    or AccessibleRole.Border
                    or AccessibleRole.Column
                    or AccessibleRole.Row
                    or AccessibleRole.HelpBalloon
                    or AccessibleRole.Character
                    or AccessibleRole.PageTab
                    or AccessibleRole.PropertyPage
                    or AccessibleRole.DropList
                    or AccessibleRole.Dial
                    or AccessibleRole.HotkeyField
                    or AccessibleRole.Diagram
                    or AccessibleRole.Animation
                    or AccessibleRole.Equation
                    or AccessibleRole.WhiteSpace
                    or AccessibleRole.IpAddress
                    or AccessibleRole.OutlineButton => false,
                _ => !string.IsNullOrEmpty(DefaultAction),
            };
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
    internal virtual IRawElementProviderFragmentRoot.Interface? FragmentRoot => null;

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

    internal virtual ExpandCollapseState ExpandCollapseState => ExpandCollapseState.ExpandCollapseState_Collapsed;

    internal virtual void Toggle()
    {
    }

    internal virtual ToggleState ToggleState => ToggleState.ToggleState_Indeterminate;

    private protected virtual IRawElementProviderFragmentRoot.Interface? ToolStripFragmentRoot => null;

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

    internal virtual int GetMultiViewProviderCurrentView() => 0;

    internal virtual int[]? GetMultiViewProviderSupportedViews() => [];

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

    HRESULT ComIServiceProvider.Interface.QueryService(Guid* service, Guid* riid, void** ppvObject)
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
            Guid IID_IAccessibleEx = *IID.Get<IAccessibleEx>();
            if (service->Equals(IID_IAccessibleEx) && riid->Equals(IID_IAccessibleEx))
            {
                // We want to return the internal, secure, object, which we don't have access here
                // Return non-null, which will be interpreted in internal method, to mean returning casted object to IAccessibleEx
                *ppvObject = ComHelpers.GetComPointer<IAccessibleEx>(this);
                return HRESULT.S_OK;
            }
        }

        return HRESULT.E_NOINTERFACE;
    }

    HRESULT IAccessibleEx.Interface.GetObjectForChild(int idChild, IAccessibleEx** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = default;
        return HRESULT.S_OK;
    }

    HRESULT IAccessibleEx.Interface.GetIAccessiblePair(UIA.IAccessible** ppAcc, int* pidChild)
    {
        if (ppAcc is null)
        {
            return HRESULT.E_POINTER;
        }

        if (pidChild is null)
        {
            *ppAcc = default;
            return HRESULT.E_POINTER;
        }

        *ppAcc = ComHelpers.GetComPointer<UIA.IAccessible>(this);
        *pidChild = (int)PInvoke.CHILDID_SELF;
        return HRESULT.S_OK;
    }

    HRESULT IAccessibleEx.Interface.GetRuntimeId(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        try
        {
            *pRetVal = new SafeArrayScope<int>(RuntimeId);
        }
        catch (NotSupportedException)
        {
            *pRetVal = default;
            return HRESULT.COR_E_NOTSUPPORTED;
        }

        return HRESULT.S_OK;
    }

    HRESULT IAccessibleEx.Interface.ConvertReturnedElement(IRawElementProviderSimple* pIn, IAccessibleEx** ppRetValOut)
    {
        if (ppRetValOut is null)
        {
            return HRESULT.E_POINTER;
        }

        // No need to implement this for patterns and properties
        *ppRetValOut = default;
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
        *pRetVal = fragmentRoots is null
            ? default
            : fragmentRoots.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();

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

    HRESULT IRawElementProviderFragmentRoot.Interface.ElementProviderFromPoint(double x, double y, IRawElementProviderFragment** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.TryGetComPointer<IRawElementProviderFragment>(ElementProviderFromPoint(x, y));
        return HRESULT.S_OK;
    }

    HRESULT IRawElementProviderFragmentRoot.Interface.GetFocus(IRawElementProviderFragment** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.TryGetComPointer<IRawElementProviderFragment>(GetFocus());
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.Select(int flagsSelect)
    {
        Select((AccessibleSelection)flagsSelect);
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.DoDefaultAction()
    {
        DoDefaultAction();
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.SetValue(PCWSTR szValue)
    {
        SetValue(szValue.ToString());
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.GetIAccessible(UIA.IAccessible** ppAccessible)
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

    HRESULT ILegacyIAccessibleProvider.Interface.get_ChildId(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = GetChildId();
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_Name(BSTR* pszName)
    {
        if (pszName is null)
        {
            return HRESULT.E_POINTER;
        }

        *pszName = CanGetNameInternal
            ? GetNameInternal()
            : new(Name);
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_Value(BSTR* pszValue)
    {
        {
            if (pszValue is null)
            {
                return HRESULT.E_POINTER;
            }

            *pszValue = CanGetValueInternal
                ? GetValueInternal()
                : new(Value);
            return HRESULT.S_OK;
        }
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_Description(BSTR* pszDescription)
    {
        if (pszDescription is null)
        {
            return HRESULT.E_POINTER;
        }

        *pszDescription = CanGetDescriptionInternal
            ? GetDescriptionInternal()
            : new(Description);
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_Role(uint* pdwRole)
    {
        if (pdwRole is null)
        {
            return HRESULT.E_POINTER;
        }

        *pdwRole = (uint)Role;
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_State(uint* pdwState)
    {
        if (pdwState is null)
        {
            return HRESULT.E_POINTER;
        }

        *pdwState = (uint)State;
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_Help(BSTR* pszHelp)
    {
        if (pszHelp is null)
        {
            return HRESULT.E_POINTER;
        }

        *pszHelp = CanGetHelpInternal
            ? GetHelpInternal()
            : new(Help);
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_KeyboardShortcut(BSTR* pszKeyboardShortcut)
    {
        if (pszKeyboardShortcut is null)
        {
            return HRESULT.E_POINTER;
        }

        *pszKeyboardShortcut = new(KeyboardShortcut);
        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.GetSelection(SAFEARRAY** pvarSelectedChildren)
    {
        if (pvarSelectedChildren is null)
        {
            return HRESULT.E_POINTER;
        }

        if (GetSelected() is not IRawElementProviderSimple.Interface selected)
        {
            *pvarSelectedChildren = SAFEARRAY.CreateEmpty(VARENUM.VT_UNKNOWN);
        }
        else
        {
            ComSafeArrayScope<IRawElementProviderSimple> scope = new(1);
            // Adding to the SAFEARRAY adds a reference
            using var selection = ComHelpers.GetComScope<IRawElementProviderSimple>(selected);
            scope[0] = selection;
            *pvarSelectedChildren = scope;
        }

        return HRESULT.S_OK;
    }

    HRESULT ILegacyIAccessibleProvider.Interface.get_DefaultAction(BSTR* pszDefaultAction)
    {
        if (pszDefaultAction is null)
        {
            return HRESULT.E_POINTER;
        }

        *pszDefaultAction = new(DefaultAction);
        return HRESULT.S_OK;
    }

    HRESULT IExpandCollapseProvider.Interface.Expand()
    {
        Expand();
        return HRESULT.S_OK;
    }

    HRESULT IExpandCollapseProvider.Interface.Collapse()
    {
        Collapse();
        return HRESULT.S_OK;
    }

    HRESULT IExpandCollapseProvider.Interface.get_ExpandCollapseState(ExpandCollapseState* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ExpandCollapseState;
        return HRESULT.S_OK;
    }

    HRESULT IInvokeProvider.Interface.Invoke()
    {
        Invoke();
        return HRESULT.S_OK;
    }

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

    HRESULT IValueProvider.Interface.SetValue(PCWSTR val)
    {
        SetValue(val.ToString());
        return HRESULT.S_OK;
    }

    HRESULT IValueProvider.Interface.get_Value(BSTR* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = Value is null ? default : new(Value);
        return HRESULT.S_OK;
    }

    HRESULT IValueProvider.Interface.get_IsReadOnly(BOOL* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = IsReadOnly;
        return HRESULT.S_OK;
    }

    HRESULT IToggleProvider.Interface.Toggle()
    {
        Toggle();
        return HRESULT.S_OK;
    }

    HRESULT IToggleProvider.Interface.get_ToggleState(ToggleState* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ToggleState;
        return HRESULT.S_OK;
    }

    HRESULT ITableProvider.Interface.GetRowHeaders(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderSimple.Interface[]? rowHeaders = GetRowHeaders();
        *pRetVal = rowHeaders is null
            ? default
            : rowHeaders.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();

        return HRESULT.S_OK;
    }

    HRESULT ITableProvider.Interface.GetColumnHeaders(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderSimple.Interface[]? columnHeaders = GetColumnHeaders();
        *pRetVal = columnHeaders is null
            ? default
            : columnHeaders.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();

        return HRESULT.S_OK;
    }

    HRESULT ITableProvider.Interface.get_RowOrColumnMajor(RowOrColumnMajor* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = RowOrColumnMajor;
        return HRESULT.S_OK;
    }

    HRESULT ITableItemProvider.Interface.GetRowHeaderItems(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderSimple.Interface[]? rowHeaderItems = GetRowHeaderItems();
        *pRetVal = rowHeaderItems is null
            ? default
            : rowHeaderItems.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();

        return HRESULT.S_OK;
    }

    HRESULT ITableItemProvider.Interface.GetColumnHeaderItems(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderSimple.Interface[]? columnHeaderItems = GetColumnHeaderItems();
        *pRetVal = columnHeaderItems is null
            ? default
            : columnHeaderItems.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();

        return HRESULT.S_OK;
    }

    HRESULT IGridProvider.Interface.GetItem(int row, int column, IRawElementProviderSimple** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.TryGetComPointer<IRawElementProviderSimple>(GetItem(row, column));
        return HRESULT.S_OK;
    }

    HRESULT IGridProvider.Interface.get_RowCount(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = RowCount;
        return HRESULT.S_OK;
    }

    HRESULT IGridProvider.Interface.get_ColumnCount(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ColumnCount;
        return HRESULT.S_OK;
    }

    HRESULT IGridItemProvider.Interface.get_Row(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = Row;
        return HRESULT.S_OK;
    }

    HRESULT IGridItemProvider.Interface.get_Column(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = Column;
        return HRESULT.S_OK;
    }

    HRESULT IGridItemProvider.Interface.get_RowSpan(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = RowSpan;
        return HRESULT.S_OK;
    }

    HRESULT IGridItemProvider.Interface.get_ColumnSpan(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ColumnSpan;
        return HRESULT.S_OK;
    }

    HRESULT IGridItemProvider.Interface.get_ContainingGrid(IRawElementProviderSimple** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.TryGetComPointer<IRawElementProviderSimple>(ContainingGrid);
        return HRESULT.S_OK;
    }

    void IAccessible.accDoDefaultAction(object childID) =>
        ((UIA.IAccessible.Interface)this).accDoDefaultAction(ChildIdToVARIANT(childID));

    HRESULT UIA.IAccessible.Interface.accDoDefaultAction(VARIANT varChild)
    {
        if (IsClientObject)
        {
            // If the default action is to be performed on self, do it.
            if (IsValidSelfChildID(varChild))
            {
                DoDefaultAction();
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                child.DoDefaultAction();
                return HRESULT.S_OK;
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return result;
        }

        return accessible.Value->accDoDefaultAction(varChild);
    }

    private static VARIANT ChildIdToVARIANT(object childId)
    {
        if (childId is int integer)
        {
            return (VARIANT)integer;
        }

        if (childId is null)
        {
            return VARIANT.Empty;
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

        return VARIANT.Empty;
    }

    object? IAccessible.accHitTest(int xLeft, int yTop)
    {
        VARIANT result = default;
        ((UIA.IAccessible.Interface)this).accHitTest(xLeft, yTop, &result);
        return result.ToObject();
    }

    HRESULT UIA.IAccessible.Interface.accHitTest(int xLeft, int yTop, VARIANT* pvarChild)
    {
        if (pvarChild is null)
        {
            return HRESULT.E_POINTER;
        }

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
            AccessibleObject? obj = HitTest(xLeft, yTop);
            if (obj is not null)
            {
                *pvarChild = AsChildIdVariant(obj);
                return HRESULT.S_OK;
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            *pvarChild = VARIANT.Empty;
            return HRESULT.S_OK;
        }

        return accessible.Value->accHitTest(xLeft, yTop, pvarChild);
    }

    void IAccessible.accLocation(
        out int pxLeft,
        out int pyTop,
        out int pcxWidth,
        out int pcyHeight,
        object childID) => this.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, ChildIdToVARIANT(childID));

    HRESULT UIA.IAccessible.Interface.accLocation(int* pxLeft, int* pyTop, int* pcxWidth, int* pcyHeight, VARIANT varChild)
    {
        if (pxLeft is null || pyTop is null || pcxWidth is null || pcyHeight is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            // Use the Location function's return value if available
            if (IsValidSelfChildID(varChild))
            {
                Rectangle bounds = Bounds;
                *pxLeft = bounds.X;
                *pyTop = bounds.Y;
                *pcxWidth = bounds.Width;
                *pcyHeight = bounds.Height;

                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                Rectangle bounds = child.Bounds;
                *pxLeft = bounds.X;
                *pyTop = bounds.Y;
                *pcxWidth = bounds.Width;
                *pcyHeight = bounds.Height;

                return HRESULT.S_OK;
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            *pxLeft = *pyTop = *pcxWidth = *pcyHeight = 0;
            return HRESULT.S_OK;
        }

        return accessible.Value->accLocation(pxLeft, pyTop, pcxWidth, pcyHeight, varChild);
    }

    object? IAccessible.accNavigate(int navDir, object childID)
    {
        using VARIANT result = default;
        ((UIA.IAccessible.Interface)this).accNavigate(navDir, ChildIdToVARIANT(childID), &result).AssertSuccess();
        return result.ToObject();
    }

    HRESULT UIA.IAccessible.Interface.accNavigate(int navDir, VARIANT varStart, VARIANT* pvarEndUpAt)
    {
        if (pvarEndUpAt is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            // Use the Navigate function's return value if available
            if (IsValidSelfChildID(varStart))
            {
                AccessibleObject? newObject = Navigate((AccessibleNavigation)navDir);
                if (newObject is not null)
                {
                    *pvarEndUpAt = AsChildIdVariant(newObject);
                    return HRESULT.S_OK;
                }
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varStart);
            if (child is not null)
            {
                *pvarEndUpAt = AsChildIdVariant(child.Navigate((AccessibleNavigation)navDir));
                return HRESULT.S_OK;
            }
        }

        if (SysNavigate((AccessibleNavigation)navDir, varStart, out AccessibleObject? accessibleObject))
        {
            *pvarEndUpAt = AsChildIdVariant(accessibleObject);
            return HRESULT.S_OK;
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            *pvarEndUpAt = VARIANT.Empty;
            return HRESULT.S_OK;
        }

        return accessible.Value->accNavigate(navDir, varStart, pvarEndUpAt);
    }

    /// <summary>
    ///  Select an accessible object.
    /// </summary>
    void IAccessible.accSelect(int flagsSelect, object childID) =>
        ((UIA.IAccessible.Interface)this).accSelect(flagsSelect, ChildIdToVARIANT(childID));

    HRESULT UIA.IAccessible.Interface.accSelect(int flagsSelect, VARIANT varChild)
    {
        if (IsClientObject)
        {
            // If the selection is self, do it.
            if (IsValidSelfChildID(varChild))
            {
                Select((AccessibleSelection)flagsSelect);    // Uses an Enum which matches SELFLAG
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                child.Select((AccessibleSelection)flagsSelect);
                return HRESULT.S_OK;
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            return result;
        }

        return accessible.Value->accSelect(flagsSelect, varChild);
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
        ComScope<IDispatch> child = new(null);
        ((UIA.IAccessible.Interface)this).get_accChild(ChildIdToVARIANT(childID), child);
        return child.IsNull ? null : ComHelpers.GetObjectForIUnknown(child);
    }

    HRESULT UIA.IAccessible.Interface.get_accChild(VARIANT varChild, IDispatch** ppdispChild)
    {
        if (ppdispChild is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild))
            {
                // Return self for invalid child ID
                *ppdispChild = GetIDispatch(this);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                // Make sure we're not returning ourselves as our own child
                Debug.Assert(
                    child != this,
                    "An accessible object is returning itself as its own child. This can cause Accessibility client applications to stop responding.");
                if (child == this)
                {
                    *ppdispChild = null;
                    return HRESULT.S_OK;
                }

                *ppdispChild = GetIDispatch(child);
                return HRESULT.S_OK;
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            *ppdispChild = null;
            return HRESULT.S_OK;
        }

        result = accessible.Value->get_accChildCount(out int count);

        if (result.Failed || count == 0)
        {
            *ppdispChild = null;
            return HRESULT.S_OK;
        }

        return accessible.Value->get_accChild(varChild, ppdispChild);
    }

    int IAccessible.accChildCount
    {
        get
        {
            int childCount = -1;
            ((UIA.IAccessible.Interface)this).get_accChildCount(&childCount);
            return childCount;
        }
    }

    HRESULT UIA.IAccessible.Interface.get_accChildCount(int* pcountChildren)
    {
        if (pcountChildren is null)
        {
            return HRESULT.E_POINTER;
        }

        *pcountChildren = -1;

        if (IsClientObject)
        {
            *pcountChildren = GetChildCount();
        }

        if (*pcountChildren == -1)
        {
            using ComScope<UIA.IAccessible> accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
            if (result.Failed)
            {
                *pcountChildren = 0;
            }
            else
            {
                result = accessible.Value->get_accChildCount(pcountChildren);
            }
        }

        return HRESULT.S_OK;
    }

    string? IAccessible.get_accDefaultAction(object childID)
    {
        using BSTR result = default;
        ((UIA.IAccessible.Interface)this).get_accDefaultAction(ChildIdToVARIANT(childID), &result);
        return result.ToString();
    }

    HRESULT UIA.IAccessible.Interface.get_accDefaultAction(VARIANT varChild, BSTR* pszDefaultAction)
    {
        if (pszDefaultAction is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild))
            {
                // Return the default action property on this.
                *pszDefaultAction = CanGetDefaultActionInternal
                    ? GetDefaultActionInternal()
                    : new(DefaultAction);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pszDefaultAction = child.CanGetDefaultActionInternal
                    ? child.GetDefaultActionInternal()
                    : new(child.DefaultAction);
                return HRESULT.S_OK;
            }
        }

        *pszDefaultAction = SystemIAccessible.TryGetDefaultAction(varChild);
        return HRESULT.S_OK;
    }

    string? IAccessible.get_accDescription(object childID)
    {
        using BSTR description = default;
        ((UIA.IAccessible.Interface)this).get_accDescription(ChildIdToVARIANT(childID), &description);
        return description.ToString();
    }

    HRESULT UIA.IAccessible.Interface.get_accDescription(VARIANT varChild, BSTR* pszDescription)
    {
        if (pszDescription is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild))
            {
                // Return self description property
                *pszDescription = CanGetDescriptionInternal
                    ? GetDescriptionInternal()
                    : new(Description);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pszDescription = child.CanGetDescriptionInternal
                    ? child.GetDescriptionInternal()
                    : new(child.Description);
                return HRESULT.S_OK;
            }
        }

        *pszDescription = SystemIAccessible.TryGetDescription(varChild);
        return HRESULT.S_OK;
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

    /// <summary>
    ///  Returns the appropriate child from the Accessible Child Collection, if available.
    /// </summary>
    private AccessibleObject? GetAccessibleChild(VARIANT childID)
    {
        if (childID.vt is not VARENUM.VT_INT and not VARENUM.VT_I4)
        {
            return null;
        }

        int index = childID.data.intVal - 1;
        return index >= 0 && index < GetChildCount() ? GetChild(index) : null;
    }

    object? IAccessible.accFocus =>
        GetFocusedObject() is { } focused
        ? focused == this
            ? (int)PInvoke.CHILDID_SELF
            : focused
        : TryGetAccessibleObject(GetSystemIAccessibleFocus());

    private AccessibleObject? GetFocusedObject() => IsClientObject ? GetFocused() : null;

    HRESULT UIA.IAccessible.Interface.get_accFocus(VARIANT* pvarChild)
    {
        if (pvarChild is null)
        {
            return HRESULT.E_POINTER;
        }

        *pvarChild = GetFocusedObject() is { } focused ? AsChildIdVariant(focused) : GetSystemIAccessibleFocus();
        return HRESULT.S_OK;
    }

    string? IAccessible.get_accHelp(object childID)
    {
        using BSTR result = default;
        ((UIA.IAccessible.Interface)this).get_accHelp(ChildIdToVARIANT(childID), &result);
        return result.ToString();
    }

    HRESULT UIA.IAccessible.Interface.get_accHelp(VARIANT varChild, BSTR* pszHelp)
    {
        if (pszHelp is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild))
            {
                *pszHelp = CanGetHelpInternal
                    ? GetHelpInternal()
                    : new(Help);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pszHelp = child.CanGetHelpInternal
                    ? child.GetHelpInternal()
                    : new(child.Help);
                return HRESULT.S_OK;
            }
        }

        *pszHelp = SystemIAccessible.TryGetHelp(varChild);
        return HRESULT.S_OK;
    }

    int IAccessible.get_accHelpTopic(out string? pszHelpFile, object childID)
    {
        using BSTR helpFile = default;
        int topic = -1;
        ((UIA.IAccessible.Interface)this).get_accHelpTopic(&helpFile, ChildIdToVARIANT(childID), &topic).AssertSuccess();
        pszHelpFile = helpFile.ToString();
        return topic;
    }

    HRESULT UIA.IAccessible.Interface.get_accHelpTopic(BSTR* pszHelpFile, VARIANT varChild, int* pidTopic)
    {
        if (pidTopic is null || pszHelpFile is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild))
            {
                if (CanGetHelpTopicInternal)
                {
                    (*pidTopic, *pszHelpFile) = GetHelpTopicInternal();
                    return HRESULT.S_OK;
                }

                *pidTopic = GetHelpTopic(out string? helpFile);
                *pszHelpFile = new(helpFile);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                if (child.CanGetHelpTopicInternal)
                {
                    (*pidTopic, *pszHelpFile) = child.GetHelpTopicInternal();
                    return HRESULT.S_OK;
                }

                *pidTopic = child.GetHelpTopic(out string? helpFile);
                *pszHelpFile = new(helpFile);
                return HRESULT.S_OK;
            }
        }

        (*pidTopic, *pszHelpFile) = SystemIAccessible.TryGetHelpTopic(varChild);
        return HRESULT.S_OK;
    }

    string? IAccessible.get_accKeyboardShortcut(object childID)
    {
        using BSTR shortcut = default;
        ((UIA.IAccessible.Interface)this).get_accKeyboardShortcut(ChildIdToVARIANT(childID), &shortcut).AssertSuccess();
        return shortcut.ToString();
    }

    HRESULT UIA.IAccessible.Interface.get_accKeyboardShortcut(VARIANT varChild, BSTR* pszKeyboardShortcut)
    {
        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild) || IsValidSelfChildIDAdditionalCheck(varChild))
            {
                *pszKeyboardShortcut = CanGetKeyboardShortcutInternal
                    ? GetKeyboardShortcutInternal(varChild)
                    : new(KeyboardShortcut);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pszKeyboardShortcut = child.CanGetKeyboardShortcutInternal
                    ? child.GetKeyboardShortcutInternal(varChild)
                    : new(child.KeyboardShortcut);
                return HRESULT.S_OK;
            }
        }

        *pszKeyboardShortcut = SystemIAccessible.TryGetKeyboardShortcut(varChild);
        return HRESULT.S_OK;
    }

    string? IAccessible.get_accName(object childID)
    {
        using BSTR name = default;
        ((UIA.IAccessible.Interface)this).get_accName(ChildIdToVARIANT(childID), &name).AssertSuccess();
        return name.ToString();
    }

    HRESULT UIA.IAccessible.Interface.get_accName(VARIANT varChild, BSTR* pszName)
    {
        if (pszName is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild) || IsValidSelfChildIDAdditionalCheck(varChild))
            {
                *pszName = CanGetNameInternal
                    ? GetNameInternal()
                    : new(Name);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pszName = child.CanGetNameInternal
                    ? child.GetNameInternal()
                    : new(child.Name);
                return HRESULT.S_OK;
            }
        }

        BSTR systemName = SystemIAccessible.TryGetName(varChild);

        if (IsClientObject && systemName.IsNullOrEmpty)
        {
            // Name the child after its parent
            systemName = CanGetNameInternal
                ? GetNameInternal()
                : new(Name);
        }

        *pszName = systemName;
        return HRESULT.S_OK;
    }

    object? IAccessible.accParent
    {
        get
        {
            ComScope<IDispatch> dispatch = new(null);
            ((UIA.IAccessible.Interface)this).get_accParent(dispatch).AssertSuccess();
            return dispatch.IsNull ? null : ComHelpers.GetObjectForIUnknown(dispatch);
        }
    }

    HRESULT UIA.IAccessible.Interface.get_accParent(IDispatch** ppdispParent)
    {
        if (ppdispParent is null)
        {
            return HRESULT.E_POINTER;
        }

        using BoolScope scope = new(ref _inCallback);
        AccessibleObject? accessibleObject = Parent;
        if (ReferenceEquals(accessibleObject, s_parentFlag))
        {
            *ppdispParent = GetSystemAccessibleParent();
            return HRESULT.S_OK;
        }

        if (accessibleObject is not null)
        {
            // Some debugging related tests
            Debug.Assert(
                accessibleObject != this,
                "An accessible object is returning itself as its own parent. This can cause accessibility clients to stop responding.");
            if (accessibleObject == this)
            {
                // This should prevent accessibility clients from stop responding
                accessibleObject = null;
            }
        }

        *ppdispParent = GetIDispatch(accessibleObject);
        return HRESULT.S_OK;
    }

    object? IAccessible.get_accRole(object childID)
    {
        using VARIANT result = default;
        ((UIA.IAccessible.Interface)this).get_accRole(ChildIdToVARIANT(childID), &result);
        return result.ToObject();
    }

    HRESULT UIA.IAccessible.Interface.get_accRole(VARIANT varChild, VARIANT* pvarRole)
    {
        if (pvarRole is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            // Return the role property if available
            if (IsValidSelfChildID(varChild))
            {
                *pvarRole = (VARIANT)(int)Role;
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pvarRole = (VARIANT)(int)child.Role;
                return HRESULT.S_OK;
            }
        }

        int count = SystemIAccessible.TryGetChildCount();

        // Unclear why this returns null for no children.
        *pvarRole = count == 0 ? VARIANT.Empty : (VARIANT)(int)SystemIAccessible.TryGetRole(varChild);
        return HRESULT.S_OK;
    }

    object? IAccessible.accSelection
    {
        get
        {
            VARIANT result = default;
            ((UIA.IAccessible.Interface)this).get_accSelection(&result);
            return result.ToObject();
        }
    }

    HRESULT UIA.IAccessible.Interface.get_accSelection(VARIANT* pvarChildren)
    {
        if (pvarChildren is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            AccessibleObject? obj = GetSelected();
            if (obj is not null)
            {
                *pvarChildren = AsChildIdVariant(obj);
                return HRESULT.S_OK;
            }
        }

        using var accessible = SystemIAccessible.TryGetIAccessible(out HRESULT result);
        if (result.Failed)
        {
            *pvarChildren = VARIANT.Empty;
            return HRESULT.S_OK;
        }

        return accessible.Value->get_accSelection(pvarChildren);
    }

    object? IAccessible.get_accState(object childID)
    {
        using VARIANT result = default;
        ((UIA.IAccessible.Interface)this).get_accState(ChildIdToVARIANT(childID), &result).AssertSuccess();
        return result.ToObject();
    }

    HRESULT UIA.IAccessible.Interface.get_accState(VARIANT varChild, VARIANT* pvarState)
    {
        if (pvarState is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            // Return the state property if available
            if (IsValidSelfChildID(varChild))
            {
                *pvarState = (VARIANT)(int)State;
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pvarState = (VARIANT)(int)child.State;
                return HRESULT.S_OK;
            }
        }

        // Perhaps would be better to return AccessibleStates.None instead of null here.
        *pvarState = SystemIAccessible?.TryGetState(varChild) is { } state ? (VARIANT)(int)state : VARIANT.Empty;
        return HRESULT.S_OK;
    }

    string? IAccessible.get_accValue(object childID)
    {
        using BSTR value = default;
        ((UIA.IAccessible.Interface)this).get_accValue(ChildIdToVARIANT(childID), &value);
        return value.ToString();
    }

    HRESULT UIA.IAccessible.Interface.get_accValue(VARIANT varChild, BSTR* pszValue)
    {
        if (pszValue is null)
        {
            return HRESULT.E_POINTER;
        }

        if (IsClientObject)
        {
            if (IsValidSelfChildID(varChild))
            {
                // Return self value property.
                *pszValue = CanGetValueInternal
                    ? GetValueInternal()
                    : new(Value);
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                *pszValue = child.CanGetValueInternal
                    ? child.GetValueInternal()
                    : new(child.Value);
                return HRESULT.S_OK;
            }
        }

        *pszValue = SystemIAccessible.TryGetValue(varChild);
        return HRESULT.S_OK;
    }

    void IAccessible.set_accName(object childID, string newName) =>
        ((UIA.IAccessible.Interface)this).put_accName(ChildIdToVARIANT(childID), new(newName));

    HRESULT UIA.IAccessible.Interface.put_accName(VARIANT varChild, BSTR szName)
    {
        if (IsClientObject)
        {
            // Set the name property if available
            if (IsValidSelfChildID(varChild))
            {
                if (CanSetNameInternal)
                {
                    SetNameInternal(szName);
                    return HRESULT.S_OK;
                }

                // Attempt to set the name property
                Name = szName.ToString();
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                if (CanSetNameInternal)
                {
                    child.SetNameInternal(szName);
                    return HRESULT.S_OK;
                }

                child.Name = szName.ToString();
                return HRESULT.S_OK;
            }
        }

        SystemIAccessible.TrySetName(varChild, szName);
        return HRESULT.S_OK;
    }

    void IAccessible.set_accValue(object childID, string newValue) =>
        ((UIA.IAccessible.Interface)this).put_accValue(ChildIdToVARIANT(childID), new(newValue));

    HRESULT UIA.IAccessible.Interface.put_accValue(VARIANT varChild, BSTR szValue)
    {
        if (IsClientObject)
        {
            // Set the value property if available
            if (IsValidSelfChildID(varChild))
            {
                // Attempt to set the value property
                if (CanSetValueInternal)
                {
                    SetValueInternal(szValue);
                    return HRESULT.S_OK;
                }

                Value = szValue.ToString();
                return HRESULT.S_OK;
            }

            // If we have an accessible object collection, get the appropriate child
            AccessibleObject? child = GetAccessibleChild(varChild);
            if (child is not null)
            {
                if (child.CanSetValueInternal)
                {
                    child.SetValueInternal(szValue);
                    return HRESULT.S_OK;
                }

                child.Value = szValue.ToString();
                return HRESULT.S_OK;
            }
        }

        SystemIAccessible.TrySetValue(varChild, szValue);
        return HRESULT.S_OK;
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

        if (SysNavigate(navdir, (VARIANT)(int)PInvoke.CHILDID_SELF, out AccessibleObject? accessibleObject))
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

    private VARIANT AsChildIdVariant(AccessibleObject? obj)
    {
        // https://learn.microsoft.com/windows/win32/winauto/how-child-ids-are-used-in-parameters
        if (obj == this)
        {
            return (VARIANT)(int)PInvoke.CHILDID_SELF;
        }

        if (obj is null)
        {
            return VARIANT.Empty;
        }

        return new VARIANT()
        {
            vt = VARENUM.VT_DISPATCH,
            data = new() { pdispVal = GetIDispatch(obj) }
        };
    }

    /// <summary>
    ///  Returns the IDispatch pointer to the system IAccessible if <paramref name="obj"/> is a system wrapper.
    ///  Otherwise just returns the IDispatch pointer of <paramref name="obj"/>.
    /// </summary>
    private static IDispatch* GetIDispatch(AccessibleObject? obj)
    {
        // We are always wrapping SystemIAccessible.
        if (obj is not null && obj._isSystemWrapper && obj.SystemIAccessible is { } accessible)
        {
            // We're just a simple system wrapper, return the pointer.
            return accessible.GetInterface<IDispatch>().Value;
        }

        return ComHelpers.TryGetComPointer<IDispatch>(obj);
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
    ///   that does some of the dirty work. Usage is similar to
    ///   <see cref="GetSysChild(AccessibleNavigation, out AccessibleObject?)"/>.
    ///   Called prior to calling <see cref="UIA.IAccessible.get_accName(VARIANT, BSTR*)"/> on the 'inner' system
    ///   accessible object.
    ///  </para>
    /// </remarks>
    private bool SysNavigate(AccessibleNavigation direction, VARIANT childID, out AccessibleObject? accessibleObject)
    {
        accessibleObject = null;

        // Only override system navigation relative to ourselves (since we can't interpret OLEACC child ids)
        if ((int)childID != (int)PInvoke.CHILDID_SELF)
        {
            return false;
        }

        // Perform any supported navigation operation (fall back on system for unsupported navigation ops)
        return GetSysChild(direction, out accessibleObject);
    }

    /// <summary>
    ///  Checks if the <paramref name="childID"/> is representative of self.
    /// </summary>
    private static bool IsValidSelfChildID(VARIANT childID) =>
        childID.IsEmpty
        || childID.vt is not VARENUM.VT_I4 and not VARENUM.VT_INT
        || childID.data.intVal == (int)HRESULT.DISP_E_PARAMNOTFOUND
        || childID.data.intVal == (int)PInvoke.CHILDID_SELF;

    /// <inheritdoc cref="IsValidSelfChildID(VARIANT)"/>
    /// <remarks>
    ///  <para>
    ///   Derived classes may override this to provide additional terms to determine if
    ///   <paramref name="childId"/> is representative of self. This method should then be
    ///   called in the appropriate <see cref="IAccessible"/> interface method implementation
    ///   where the additional terms is to be respected alongside <see cref="IsValidSelfChildID(VARIANT)"/>.
    ///  </para>
    /// </remarks>
    internal virtual bool IsValidSelfChildIDAdditionalCheck(VARIANT childId) => false;

    /// <summary>
    ///  Tries to get corresponding AccessibleObject that is represented by <paramref name="variant"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   Calling this will dispose <paramref name="variant"/> when finished.
    ///  </para>
    /// </remarks>
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
            args = [(int)PInvoke.CHILDID_SELF];
        }

        return typeof(IAccessible).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
    }

    Type IReflect.UnderlyingSystemType => typeof(IAccessible);

    HRESULT IRawElementProviderHwndOverride.Interface.GetOverrideProviderForHwnd(HWND hwnd, IRawElementProviderSimple** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = default;
        return HRESULT.S_OK;
    }

    HRESULT IMultipleViewProvider.Interface.get_CurrentView(int* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = GetMultiViewProviderCurrentView();
        return HRESULT.S_OK;
    }

    HRESULT IMultipleViewProvider.Interface.GetSupportedViews(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        int[]? result = GetMultiViewProviderSupportedViews();

        *pRetVal = result is null ? SAFEARRAY.CreateEmpty(VARENUM.VT_I4) : new SafeArrayScope<int>(result);

        return HRESULT.S_OK;
    }

    HRESULT IMultipleViewProvider.Interface.GetViewName(int viewId, BSTR* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = new BSTR(GetMultiViewProviderViewName(viewId));
        return HRESULT.S_OK;
    }

    HRESULT IMultipleViewProvider.Interface.SetCurrentView(int viewId)
    {
        SetMultiViewProviderCurrentView(viewId);
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.SetValue(double val)
    {
        SetValue(val);
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.get_Value(double* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = RangeValue;
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.get_IsReadOnly(BOOL* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = IsReadOnly;
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.get_Maximum(double* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = Maximum;
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.get_Minimum(double* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = Minimum;
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.get_LargeChange(double* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = LargeChange;
        return HRESULT.S_OK;
    }

    HRESULT IRangeValueProvider.Interface.get_SmallChange(double* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = SmallChange;
        return HRESULT.S_OK;
    }

    HRESULT ISelectionProvider.Interface.GetSelection(SAFEARRAY** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        IRawElementProviderSimple.Interface[]? selection = GetSelection();
        *pRetVal = selection is null
            ? default
            : selection.CreateComSafeArrayScope<IRawElementProviderSimple, IRawElementProviderSimple.Interface>();

        return HRESULT.S_OK;
    }

    HRESULT ISelectionProvider.Interface.get_CanSelectMultiple(BOOL* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = CanSelectMultiple ? BOOL.TRUE : BOOL.FALSE;
        return HRESULT.S_OK;
    }

    HRESULT ISelectionProvider.Interface.get_IsSelectionRequired(BOOL* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = IsSelectionRequired ? BOOL.TRUE : BOOL.FALSE;
        return HRESULT.S_OK;
    }

    HRESULT ISelectionItemProvider.Interface.Select()
    {
        SelectItem();
        return HRESULT.S_OK;
    }

    HRESULT ISelectionItemProvider.Interface.AddToSelection()
    {
        AddToSelection();
        return HRESULT.S_OK;
    }

    HRESULT ISelectionItemProvider.Interface.RemoveFromSelection()
    {
        RemoveFromSelection();
        return HRESULT.S_OK;
    }

    HRESULT ISelectionItemProvider.Interface.get_IsSelected(BOOL* pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = IsItemSelected ? BOOL.TRUE : BOOL.FALSE;
        return HRESULT.S_OK;
    }

    HRESULT ISelectionItemProvider.Interface.get_SelectionContainer(IRawElementProviderSimple** pRetVal)
    {
        if (pRetVal is null)
        {
            return HRESULT.E_POINTER;
        }

        *pRetVal = ComHelpers.TryGetComPointer<IRawElementProviderSimple>(ItemSelectionContainer);
        return HRESULT.S_OK;
    }

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
        => !LocalAppContextSwitches.NoClientNotifications
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
        if (PInvoke.UiaClientsAreListening() && !LocalAppContextSwitches.NoClientNotifications)
        {
            using var provider = ComHelpers.GetComScope<IRawElementProviderSimple>(this);
            HRESULT result = PInvoke.UiaRaiseAutomationEvent(provider, eventId);
            return result == HRESULT.S_OK;
        }

        return false;
    }

    internal virtual bool RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID propertyId, VARIANT oldValue, VARIANT newValue)
    {
        if (PInvoke.UiaClientsAreListening() && !LocalAppContextSwitches.NoClientNotifications)
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
        if (PInvoke.UiaClientsAreListening() && !LocalAppContextSwitches.NoClientNotifications)
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

    HRESULT IScrollItemProvider.Interface.ScrollIntoView()
    {
        ScrollIntoView();
        return HRESULT.S_OK;
    }

    internal virtual void ScrollIntoView() => Debug.Fail($"{nameof(ScrollIntoView)}() is not overridden");

    private ComScope<IOleWindow> TryGetOleWindow(out HRESULT result)
    {
        if (_systemIOleWindow is { } agile)
        {
            return agile.TryGetInterface(out result);
        }

        result = HRESULT.E_NOINTERFACE;
        return default;
    }

    HRESULT IDispatch.Interface.GetTypeInfoCount(uint* pctinfo)
        => ((IDispatch.Interface)_dispatchAdapter).GetTypeInfoCount(pctinfo);

    HRESULT IDispatch.Interface.GetTypeInfo(uint iTInfo, uint lcid, ITypeInfo** ppTInfo)
        => ((IDispatch.Interface)_dispatchAdapter).GetTypeInfo(iTInfo, lcid, ppTInfo);

    HRESULT IDispatch.Interface.GetIDsOfNames(Guid* riid, PWSTR* rgszNames, uint cNames, uint lcid, int* rgDispId)
        => ((IDispatch.Interface)_dispatchAdapter).GetIDsOfNames(riid, rgszNames, cNames, lcid, rgDispId);

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
