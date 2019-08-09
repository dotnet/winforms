// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using Accessibility;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides an implementation for an object that can be inspected by an
    ///  accessibility application.
    /// </summary>
    [ComVisible(true)]
    public class AccessibleObject : StandardOleMarshalObject,
                                    IReflect,
                                    IAccessible,
                                    UnsafeNativeMethods.IAccessibleEx,
                                    UnsafeNativeMethods.IServiceProvider,
                                    UnsafeNativeMethods.IRawElementProviderSimple,
                                    UnsafeNativeMethods.IRawElementProviderFragment,
                                    UnsafeNativeMethods.IRawElementProviderFragmentRoot,
                                    UnsafeNativeMethods.IInvokeProvider,
                                    UnsafeNativeMethods.IValueProvider,
                                    UnsafeNativeMethods.IRangeValueProvider,
                                    UnsafeNativeMethods.IExpandCollapseProvider,
                                    UnsafeNativeMethods.IToggleProvider,
                                    UnsafeNativeMethods.ITableProvider,
                                    UnsafeNativeMethods.ITableItemProvider,
                                    UnsafeNativeMethods.IGridProvider,
                                    UnsafeNativeMethods.IGridItemProvider,
                                    UnsafeNativeMethods.IEnumVariant,
                                    UnsafeNativeMethods.IOleWindow,
                                    UnsafeNativeMethods.ILegacyIAccessibleProvider,
                                    UnsafeNativeMethods.ISelectionProvider,
                                    UnsafeNativeMethods.ISelectionItemProvider,
                                    UnsafeNativeMethods.IRawElementProviderHwndOverride,
                                    UnsafeNativeMethods.IScrollItemProvider
    {
        /// <summary>
        ///  Specifies the <see cref='IAccessible'/> interface used by this <see cref='AccessibleObject'/>.
        /// </summary>
        private IAccessible systemIAccessible = null;

        /// <summary>
        ///  Specifies the <see cref='NativeMethods.IEnumVariant'/> used by this <see cref='AccessibleObject'/>.
        /// </summary>
        private UnsafeNativeMethods.IEnumVariant systemIEnumVariant = null;
        private UnsafeNativeMethods.IEnumVariant enumVariant = null;

        // IOleWindow interface of the 'inner' system IAccessible object that we are wrapping
        private UnsafeNativeMethods.IOleWindow systemIOleWindow = null;

        // Indicates this object is being used ONLY to wrap a system IAccessible
        private readonly bool systemWrapper = false;

        // The support for the UIA Notification event begins in RS3.
        // Assume the UIA Notification event is available until we learn otherwise.
        // If we learn that the UIA Notification event is not available,
        // controls should not attempt to raise it.
        private static bool notificationEventAvailable = true;

        internal const int RuntimeIDFirstItem = 0x2a;

        public AccessibleObject()
        {
        }

        // This constructor is used ONLY for wrapping system IAccessible objects
        private AccessibleObject(IAccessible iAcc)
        {
            systemIAccessible = iAcc;
            systemWrapper = true;
        }

        /// <summary>
        ///  Gets the bounds of the accessible object, in screen coordinates.
        /// </summary>
        public virtual Rectangle Bounds
        {
            get
            {
                // Use the system provided bounds
                if (systemIAccessible != null)
                {
                    try
                    {
                        systemIAccessible.accLocation(out int left, out int top, out int width, out int height, NativeMethods.CHILDID_SELF);
                        return new Rectangle(left, top, width, height);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return Rectangle.Empty;
            }
        }

        /// <summary>
        ///  Gets a description of the default action for an object.
        /// </summary>
        public virtual string DefaultAction
        {
            get
            {
                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.get_accDefaultAction(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                        // Not all objects provide a default action.
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Gets a description of the object's visual appearance to the user.
        /// </summary>
        public virtual string Description
        {
            get
            {
                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.get_accDescription(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return null;
            }
        }

        private UnsafeNativeMethods.IEnumVariant EnumVariant
        {
            get => enumVariant ?? (enumVariant = new EnumVariantObject(this));
        }

        /// <summary>
        ///  Gets a description of what the object does or how the object is used.
        /// </summary>
        public virtual string Help
        {
            get
            {
                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.get_accHelp(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Gets the object shortcut key or access key for an accessible object.
        /// </summary>
        public virtual string KeyboardShortcut
        {
            get
            {
                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.get_accKeyboardShortcut(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Gets or sets the object name.
        /// </summary>
        public virtual string Name
        {
            // Does nothing by default
            get
            {
                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.get_accName(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return null;
            }
            set
            {
                if (systemIAccessible == null)
                {
                    return;
                }

                try
                {
                    systemIAccessible.set_accName(NativeMethods.CHILDID_SELF, value);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }
        }

        /// <summary>
        ///  When overridden in a derived class, gets or sets the parent of an
        ///  accessible object.
        /// </summary>
        public virtual AccessibleObject Parent
        {
            get
            {
                if (systemIAccessible != null)
                {
                    return WrapIAccessible(systemIAccessible.accParent);
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///  Gets the role of this accessible object.
        /// </summary>
        public virtual AccessibleRole Role
        {
            get
            {
                if (systemIAccessible != null)
                {
                    return (AccessibleRole)systemIAccessible.get_accRole(NativeMethods.CHILDID_SELF);
                }
                else
                {
                    return AccessibleRole.None;
                }
            }
        }

        /// <summary>
        ///  Gets the state of this accessible object.
        /// </summary>
        public virtual AccessibleStates State
        {
            get
            {
                if (systemIAccessible != null)
                {
                    return (AccessibleStates)systemIAccessible.get_accState(NativeMethods.CHILDID_SELF);
                }
                else
                {
                    return AccessibleStates.None;
                }
            }
        }

        /// <summary>
        ///  Gets or sets the value of an accessible object.
        /// </summary>
        public virtual string Value
        {
            // Does nothing by default
            get
            {
                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.get_accValue(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return string.Empty;
            }
            set
            {
                if (systemIAccessible == null)
                {
                    return;
                }

                try
                {
                    systemIAccessible.set_accValue(NativeMethods.CHILDID_SELF, value);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }
        }

        /// <summary>
        ///  When overridden in a derived class, gets the accessible child
        ///  corresponding to the specified index.
        /// </summary>
        public virtual AccessibleObject GetChild(int index) => null;

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
        internal virtual int[] GetSysChildOrder() => null;

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
        internal virtual bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject accessibleObject)
        {
            accessibleObject = null;
            return false;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the object that has the
        ///  keyboard focus.
        /// </summary>
        public virtual AccessibleObject GetFocused()
        {
            // Default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject child = GetChild(index);
                    Debug.Assert(child != null, "GetChild(" + index.ToString(CultureInfo.InvariantCulture) + ") returned null!");
                    if (child != null && ((child.State & AccessibleStates.Focused) != 0))
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

            if (systemIAccessible != null)
            {
                try
                {
                    return WrapIAccessible(systemIAccessible.accFocus);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Gets an identifier for a Help topic and the path to the Help file
        ///  associated with this accessible object.
        /// </summary>
        public virtual int GetHelpTopic(out string fileName)
        {
            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accHelpTopic(out fileName, NativeMethods.CHILDID_SELF);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            fileName = null;
            return -1;
        }

        /// <summary>
        ///  When overridden in a derived class, gets the currently selected child.
        /// </summary>
        public virtual AccessibleObject GetSelected()
        {
            // Default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject child = GetChild(index);
                    Debug.Assert(child != null, "GetChild(" + index.ToString(CultureInfo.InvariantCulture) + ") returned null!");
                    if (child != null && ((child.State & AccessibleStates.Selected) != 0))
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

            if (systemIAccessible != null)
            {
                try
                {
                    return WrapIAccessible(systemIAccessible.accSelection);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Return the child object at the given screen coordinates.
        /// </summary>
        public virtual AccessibleObject HitTest(int x, int y)
        {
            // Default behavior for objects with AccessibleObject children
            if (GetChildCount() >= 0)
            {
                int count = GetChildCount();
                for (int index = 0; index < count; ++index)
                {
                    AccessibleObject child = GetChild(index);
                    Debug.Assert(child != null, "GetChild(" + index.ToString(CultureInfo.InvariantCulture) + ") returned null!");
                    if (child != null && child.Bounds.Contains(x, y))
                    {
                        return child;
                    }
                }

                return this;
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return WrapIAccessible(systemIAccessible.accHitTest(x, y));
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
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

        internal virtual bool IsPatternSupported(int patternId)
        {
            // Override this, in your derived class, if you implement UIAutomation patterns
            if (patternId == NativeMethods.UIA_InvokePatternId)
            {
                return IsInvokePatternAvailable;
            }

            return false;
        }

        internal virtual int[] RuntimeId => null;

        internal virtual int ProviderOptions
        {
            get => (int)(UnsafeNativeMethods.ProviderOptions.ServerSideProvider | UnsafeNativeMethods.ProviderOptions.UseComThreading);
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple HostRawElementProvider
        {
            get => null;
        }

        internal virtual object GetPropertyValue(int propertyID)
        {
            if (propertyID == NativeMethods.UIA_IsInvokePatternAvailablePropertyId)
            {
                return IsInvokePatternAvailable;
            }

            return null;
        }

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

        internal virtual UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetEmbeddedFragmentRoots()
        {
            return null;
        }

        internal virtual void SetFocus()
        {
        }

        internal virtual Rectangle BoundingRectangle => Bounds;

        internal virtual UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot
        {
            get => null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
        {
            return this;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderFragment GetFocus()
        {
            return null;
        }

        internal virtual void Expand()
        {
        }

        internal virtual void Collapse()
        {
        }

        internal virtual UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState
        {
            get => UnsafeNativeMethods.ExpandCollapseState.Collapsed;
        }

        internal virtual void Toggle()
        {
        }

        internal virtual UnsafeNativeMethods.ToggleState ToggleState
        {
            get => UnsafeNativeMethods.ToggleState.ToggleState_Indeterminate;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaders()
        {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaders()
        {
            return null;
        }

        internal virtual UnsafeNativeMethods.RowOrColumnMajor RowOrColumnMajor
        {
            get => UnsafeNativeMethods.RowOrColumnMajor.RowOrColumnMajor_RowMajor;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems()
        {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems()
        {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple GetItem(int row, int column)
        {
            return null;
        }

        internal virtual int RowCount => -1;

        internal virtual int ColumnCount => -1;

        internal virtual int Row => -1;

        internal virtual int Column => -1;

        internal virtual int RowSpan => 1;

        internal virtual int ColumnSpan => 1;

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple ContainingGrid
        {
            get => null;
        }

        internal virtual void Invoke()
        {
            // Calling DoDefaultAction here is consistent with MSAA Proxy implementation.
            DoDefaultAction();
        }

        internal virtual bool IsReadOnly => false;

        internal virtual void SetValue(string newValue)
        {
            Value = newValue;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple GetOverrideProviderForHwnd(IntPtr hwnd)
        {
            return null;
        }

        internal virtual void SetValue(double newValue)
        {
        }

        internal virtual double LargeChange => double.NaN;

        internal virtual double Maximum => double.NaN;

        internal virtual double Minimum => double.NaN;

        internal virtual double SmallChange => double.NaN;

        internal virtual double RangeValue => double.NaN;

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetSelection()
        {
            return null;
        }

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

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple ItemSelectionContainer
        {
            get => null;
        }

        /// <summary>
        ///  Sets the parent accessible object for the node which can be added or removed to/from hierachy nodes.
        /// </summary>
        /// <param name="parent">The parent accessible object.</param>
        internal virtual void SetParent(AccessibleObject parent)
        {
        }

        /// <summary>
        ///  Sets the detachable child accessible object which may be added or removed to/from hierachy nodes.
        /// </summary>
        /// <param name="child">The child accessible object.</param>
        internal virtual void SetDetachableChild(AccessibleObject child)
        {
        }

        int UnsafeNativeMethods.IServiceProvider.QueryService(ref Guid service, ref Guid riid, out IntPtr ppvObject)
        {
            int hr = NativeMethods.E_NOINTERFACE;
            ppvObject = IntPtr.Zero;

            if (IsIAccessibleExSupported())
            {
                if (service.Equals(UnsafeNativeMethods.guid_IAccessibleEx) &&
                    riid.Equals(UnsafeNativeMethods.guid_IAccessibleEx))
                {
                    // We want to return the internal, secure, object, which we don't have access here
                    // Return non-null, which will be interpreted in internal method, to mean returning casted object to IAccessibleEx
                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(UnsafeNativeMethods.IAccessibleEx));
                    hr = NativeMethods.S_OK;
                }
            }

            return hr;
        }

        object UnsafeNativeMethods.IAccessibleEx.GetObjectForChild(int idChild)
        {
            // No need to implement this for patterns and properties
            return null;
        }

        // This method is never called
        int UnsafeNativeMethods.IAccessibleEx.GetIAccessiblePair(out object ppAcc, out int pidChild)
        {
            // No need to implement this for patterns and properties
            ppAcc = null;
            pidChild = 0;
            return NativeMethods.E_POINTER;
        }

        int[] UnsafeNativeMethods.IAccessibleEx.GetRuntimeId() => RuntimeId;

        int UnsafeNativeMethods.IAccessibleEx.ConvertReturnedElement(object pIn, out object ppRetValOut)
        {
            // No need to implement this for patterns and properties
            ppRetValOut = null;
            return NativeMethods.E_NOTIMPL;
        }

        UnsafeNativeMethods.ProviderOptions UnsafeNativeMethods.IRawElementProviderSimple.ProviderOptions
        {
            get => (UnsafeNativeMethods.ProviderOptions)ProviderOptions;
        }

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderSimple.HostRawElementProvider
        {
            get => HostRawElementProvider;
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPatternProvider(int patternId)
        {
            if (IsPatternSupported(patternId))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPropertyValue(int propertyID)
        {
            return GetPropertyValue(propertyID);
        }

        object UnsafeNativeMethods.IRawElementProviderFragment.Navigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            return FragmentNavigate(direction);
        }

        int[] UnsafeNativeMethods.IRawElementProviderFragment.GetRuntimeId()
        {
            return RuntimeId;
        }

        object[] UnsafeNativeMethods.IRawElementProviderFragment.GetEmbeddedFragmentRoots()
        {
            return GetEmbeddedFragmentRoots();
        }

        void UnsafeNativeMethods.IRawElementProviderFragment.SetFocus()
        {
            SetFocus();
        }

        NativeMethods.UiaRect UnsafeNativeMethods.IRawElementProviderFragment.BoundingRectangle
        {
            get => new NativeMethods.UiaRect(BoundingRectangle);
        }

        UnsafeNativeMethods.IRawElementProviderFragmentRoot UnsafeNativeMethods.IRawElementProviderFragment.FragmentRoot
        {
            get => FragmentRoot;
        }

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y)
        {
            return ElementProviderFromPoint(x, y);
        }

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.GetFocus()
        {
            return GetFocus();
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.DefaultAction => DefaultAction;

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Description => Description;

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Help => Help;

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.KeyboardShortcut => KeyboardShortcut;

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Name => Name;

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.Role => (uint)Role;

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.State => (uint)State;

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Value => Value;

        int UnsafeNativeMethods.ILegacyIAccessibleProvider.ChildId => GetChildId();

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.DoDefaultAction()
        {
            DoDefaultAction();
        }

        IAccessible UnsafeNativeMethods.ILegacyIAccessibleProvider.GetIAccessible()
        {
            return AsIAccessible(this);
        }

        object[] UnsafeNativeMethods.ILegacyIAccessibleProvider.GetSelection()
        {
            return new UnsafeNativeMethods.IRawElementProviderSimple[]
            {
                GetSelected() as UnsafeNativeMethods.IRawElementProviderSimple
            };
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.Select(int flagsSelect)
        {
            Select((AccessibleSelection)flagsSelect);
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.SetValue(string szValue)
        {
            SetValue(szValue);
        }

        void UnsafeNativeMethods.IExpandCollapseProvider.Expand()
        {
            Expand();
        }

        void UnsafeNativeMethods.IExpandCollapseProvider.Collapse()
        {
            Collapse();
        }

        UnsafeNativeMethods.ExpandCollapseState UnsafeNativeMethods.IExpandCollapseProvider.ExpandCollapseState
        {
            get => ExpandCollapseState;
        }

        void UnsafeNativeMethods.IInvokeProvider.Invoke() => Invoke();

        bool UnsafeNativeMethods.IValueProvider.IsReadOnly => IsReadOnly;

        string UnsafeNativeMethods.IValueProvider.Value => Value;

        void UnsafeNativeMethods.IValueProvider.SetValue(string newValue)
        {
            SetValue(newValue);
        }

        void UnsafeNativeMethods.IToggleProvider.Toggle() => Toggle();

        UnsafeNativeMethods.ToggleState UnsafeNativeMethods.IToggleProvider.ToggleState
        {
            get => ToggleState;
        }

        object[] UnsafeNativeMethods.ITableProvider.GetRowHeaders()
        {
            return GetRowHeaders();
        }

        object[] UnsafeNativeMethods.ITableProvider.GetColumnHeaders()
        {
            return GetColumnHeaders();
        }

        UnsafeNativeMethods.RowOrColumnMajor UnsafeNativeMethods.ITableProvider.RowOrColumnMajor
        {
            get => RowOrColumnMajor;
        }

        object[] UnsafeNativeMethods.ITableItemProvider.GetRowHeaderItems()
        {
            return GetRowHeaderItems();
        }

        object[] UnsafeNativeMethods.ITableItemProvider.GetColumnHeaderItems()
        {
            return GetColumnHeaderItems();
        }

        object UnsafeNativeMethods.IGridProvider.GetItem(int row, int column)
        {
            return GetItem(row, column);
        }

        int UnsafeNativeMethods.IGridProvider.RowCount => RowCount;

        int UnsafeNativeMethods.IGridProvider.ColumnCount => ColumnCount;

        int UnsafeNativeMethods.IGridItemProvider.Row => Row;

        int UnsafeNativeMethods.IGridItemProvider.Column => Column;

        int UnsafeNativeMethods.IGridItemProvider.RowSpan => RowSpan;

        int UnsafeNativeMethods.IGridItemProvider.ColumnSpan => ColumnSpan;

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IGridItemProvider.ContainingGrid
        {
            get => ContainingGrid;
        }

        /// <summary>
        ///  Perform the default action
        /// </summary>
        void IAccessible.accDoDefaultAction(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccDoDefaultAction: this = " +
                    ToString() + ", childID = " + childID.ToString());

                // If the default action is to be performed on self, do it.
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    DoDefaultAction();
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    child.DoDefaultAction();
                    return;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    systemIAccessible.accDoDefaultAction(childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                    // Not all objects provide a default action.
                }
            }
        }

        /// <summary>
        ///  Perform a hit test
        /// </summary>
        object IAccessible.accHitTest(int xLeft, int yTop)
        {
            if (IsClientObject)
            {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccHitTest: this = " +
                    ToString());

                AccessibleObject obj = HitTest(xLeft, yTop);
                if (obj != null)
                {
                    return AsVariant(obj);
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.accHitTest(xLeft, yTop);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
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
            pxLeft = 0;
            pyTop = 0;
            pcxWidth = 0;
            pcyHeight = 0;

            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: this = " +
                    ToString() + ", childID = " + childID.ToString());

                // Use the Location function's return value if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    Rectangle bounds = Bounds;
                    pxLeft = bounds.X;
                    pyTop = bounds.Y;
                    pcxWidth = bounds.Width;
                    pcyHeight = bounds.Height;

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: Returning " +
                        bounds.ToString());

                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    Rectangle bounds = child.Bounds;
                    pxLeft = bounds.X;
                    pyTop = bounds.Y;
                    pcxWidth = bounds.Width;
                    pcyHeight = bounds.Height;

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: Returning " +
                        bounds.ToString());

                    return;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    systemIAccessible.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, childID);

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: Setting " +
                        pxLeft.ToString(CultureInfo.InvariantCulture) + ", " +
                        pyTop.ToString(CultureInfo.InvariantCulture) + ", " +
                        pcxWidth.ToString(CultureInfo.InvariantCulture) + ", " +
                        pcyHeight.ToString(CultureInfo.InvariantCulture));
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }

                return;
            }
        }

        /// <summary>
        ///  Navigate to another accessible object.
        /// </summary>
        object IAccessible.accNavigate(int navDir, object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccNavigate: this = " +
                    ToString() + ", navdir = " + navDir.ToString(CultureInfo.InvariantCulture) + ", childID = " + childID.ToString());

                // Use the Navigate function's return value if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    AccessibleObject newObject = Navigate((AccessibleNavigation)navDir);
                    if (newObject != null)
                    {
                        return AsVariant(newObject);
                    }
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return AsVariant(child.Navigate((AccessibleNavigation)navDir));
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    if (!SysNavigate(navDir, childID, out object retObject))
                    {
                        return systemIAccessible.accNavigate(navDir, childID);
                    }

                    return retObject;
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Select an accessible object.
        /// </summary>
        void IAccessible.accSelect(int flagsSelect, object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccSelect: this = " +
                    ToString() + ", flagsSelect = " + flagsSelect.ToString(CultureInfo.InvariantCulture) + ", childID = " + childID.ToString());

                // If the selection is self, do it.
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    Select((AccessibleSelection)flagsSelect);    // Uses an Enum which matches SELFLAG
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    child.Select((AccessibleSelection)flagsSelect);
                    return;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    systemIAccessible.accSelect(flagsSelect, childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
                return;
            }

        }

        /// <summary>
        ///  Performs the default action associated with this accessible object.
        /// </summary>
        public virtual void DoDefaultAction()
        {
            // By default, just does the system default action if available
            if (systemIAccessible != null)
            {
                try
                {
                    systemIAccessible.accDoDefaultAction(0);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                    // Not all objects provide a default action.
                }
                return;
            }
        }

        /// <summary>
        ///  Returns a child Accessible object
        /// </summary>
        object IAccessible.get_accChild(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccChild: this = " +
                    ToString() + ", childID = " + childID.ToString());

                // Return self for CHILDID_SELF
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return AsIAccessible(this);
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    // Make sure we're not returning ourselves as our own child
                    Debug.Assert(child != this, "An accessible object is returning itself as its own child. This can cause Accessibility client applications to stop responding.");
                    if (child == this)
                    {
                        return null;
                    }

                    return AsIAccessible(child);
                }
            }

            if (systemIAccessible == null || systemIAccessible.accChildCount == 0)
            {
                return null;
            }

            // Otherwise, return the default system child for this control (if any)
            return systemIAccessible.get_accChild(childID);
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
                    if (systemIAccessible != null)
                    {
                        childCount = systemIAccessible.accChildCount;
                    }
                    else
                    {
                        childCount = 0;
                    }
                }

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.accHildCount: this = " + ToString() + ", returning " + childCount.ToString(CultureInfo.InvariantCulture));

                return childCount;
            }
        }

        /// <summary>
        ///  Return the default action
        /// </summary>
        string IAccessible.get_accDefaultAction(object childID)
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
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.DefaultAction;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accDefaultAction(childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                    // Not all objects provide a default action.
                }
            }

            return null;
        }

        /// <summary>
        ///  Return the object or child description
        /// </summary>
        string IAccessible.get_accDescription(object childID)
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
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.Description;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accDescription(childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Returns the appropriate child from the Accessible Child Collection, if available
        /// </summary>
        private AccessibleObject GetAccessibleChild(object childID)
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
        object IAccessible.accFocus
        {
            get
            {
                if (IsClientObject)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccFocus: this = " +
                        ToString());

                    AccessibleObject obj = GetFocused();
                    if (obj != null)
                    {
                        return AsVariant(obj);
                    }
                }

                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.accFocus;
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Return help for this accessible object.
        /// </summary>
        string IAccessible.get_accHelp(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return Help;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.Help;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accHelp(childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Return the object or child help topic
        /// </summary>
        int IAccessible.get_accHelpTopic(out string pszHelpFile, object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return GetHelpTopic(out pszHelpFile);
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.GetHelpTopic(out pszHelpFile);
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accHelpTopic(out pszHelpFile, childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            pszHelpFile = null;
            return -1;
        }

        /// <summary>
        ///  Return the object or child keyboard shortcut
        /// </summary>
        string IAccessible.get_accKeyboardShortcut(object childID)
        {
            return get_accKeyboardShortcutInternal(childID);
        }

        internal virtual string get_accKeyboardShortcutInternal(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return KeyboardShortcut;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.KeyboardShortcut;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accKeyboardShortcut(childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Return the object or child name
        /// </summary>
        string IAccessible.get_accName(object childID)
        {
            return get_accNameInternal(childID);
        }

        internal virtual string get_accNameInternal(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.get_accName: this = " + ToString() +
                    ", childID = " + childID.ToString());

                // Return the name property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return Name;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.Name;
                }
            }

            // Otherwise, use the system provided name
            if (systemIAccessible != null)
            {
                string retval = systemIAccessible.get_accName(childID);

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

            return null;
        }

        /// <summary>
        ///  Return the parent object
        /// </summary>
        object IAccessible.accParent
        {
            get
            {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.accParent: this = " + ToString());
                AccessibleObject parent = Parent;
                if (parent != null)
                {
                    // Some debugging related tests
                    Debug.Assert(parent != this, "An accessible object is returning itself as its own parent. This can cause accessibility clients to stop responding.");
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
        object IAccessible.get_accRole(object childID)
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
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return (int)child.Role;
                }
            }

            if (systemIAccessible == null || systemIAccessible.accChildCount == 0)
            {
                return null;
            }

            return systemIAccessible.get_accRole(childID);
        }

        /// <summary>
        ///  Return the object or child selection
        /// </summary>
        object IAccessible.accSelection
        {
            get
            {
                if (IsClientObject)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccSelection: this = " +
                        ToString());

                    AccessibleObject obj = GetSelected();
                    if (obj != null)
                    {
                        return AsVariant(obj);
                    }
                }

                if (systemIAccessible != null)
                {
                    try
                    {
                        return systemIAccessible.accSelection;
                    }
                    catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                    {
                    }
                }

                return null;
            }
        }

        /// <summary>
        ///  Return the object or child state
        /// </summary>
        object IAccessible.get_accState(object childID)
        {
            if (IsClientObject)
            {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccState: this = " +
                    ToString() + ", childID = " + childID.ToString());

                // Return the state property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF))
                {
                    return (int)State;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return (int)child.State;
                }
            }

            return systemIAccessible?.get_accState(childID);
        }

        /// <summary>
        ///  Return the object or child value
        /// </summary>
        string IAccessible.get_accValue(object childID)
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
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    return child.Value;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    return systemIAccessible.get_accValue(childID);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
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
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    child.Name = newName;
                    return;
                }
            }

            systemIAccessible?.set_accName(childID, newName);
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
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null)
                {
                    child.Value = newValue;
                    return;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    systemIAccessible.set_accValue(childID, newValue);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }
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
        int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd)
        {
            // See if we have an inner object that can provide the window handle
            if (systemIOleWindow != null)
            {
                return systemIOleWindow.GetWindow(out hwnd);
            }

            // Otherwise delegate to the parent object
            AccessibleObject parent = Parent;
            if (parent is UnsafeNativeMethods.IOleWindow parentWindow)
            {
                return parentWindow.GetWindow(out hwnd);
            }

            // Or fail if there is no parent
            hwnd = IntPtr.Zero;
            return NativeMethods.E_FAIL;
        }

        /// <summary>
        ///  See GetWindow() above for details.
        /// </summary>
        void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode)
        {
            // See if we have an inner object that can provide help
            if (systemIOleWindow != null)
            {
                systemIOleWindow.ContextSensitiveHelp(fEnterMode);
                return;
            }

            // Otherwise delegate to the parent object
            AccessibleObject parent = Parent;
            if (parent is UnsafeNativeMethods.IOleWindow parentWindow)
            {
                parentWindow.ContextSensitiveHelp(fEnterMode);
                return;
            }

            // Or do nothing if there is no parent
        }

        /// <summary>
        ///  Clone this accessible object.
        /// </summary>
        void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v)
        {
            EnumVariant.Clone(v);
        }

        /// <summary>
        ///  Obtain the next n children of this accessible object.
        /// </summary>
        int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns)
        {
            return EnumVariant.Next(n, rgvar, ns);
        }

        /// <summary>
        ///  Resets the child accessible object enumerator.
        /// </summary>
        void UnsafeNativeMethods.IEnumVariant.Reset() => EnumVariant.Reset();

        /// <summary>
        ///  Skip the next n child accessible objects
        /// </summary>
        void UnsafeNativeMethods.IEnumVariant.Skip(int n) => EnumVariant.Skip(n);

        /// <summary>
        ///  When overridden in a derived class, navigates to another object.
        /// </summary>
        public virtual AccessibleObject Navigate(AccessibleNavigation navdir)
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
                        if (Parent.GetChildCount() > 0)
                        {
                            return null;
                        }
                        break;
                    case AccessibleNavigation.Next:
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                        if (Parent.GetChildCount() > 0)
                        {
                            return null;
                        }
                        break;
                }
            }

            if (systemIAccessible != null)
            {
                try
                {
                    if (!SysNavigate((int)navdir, NativeMethods.CHILDID_SELF, out object retObject))
                    {
                        retObject = systemIAccessible.accNavigate((int)navdir, NativeMethods.CHILDID_SELF);
                    }

                    return WrapIAccessible(retObject);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                }
            }

            return null;
        }

        /// <summary>
        ///  Selects this accessible object.
        /// </summary>
        public virtual void Select(AccessibleSelection flags)
        {
            // By default, do the system behavior
            if (systemIAccessible != null)
            {
                try
                {
                    systemIAccessible.accSelect((int)flags, 0);
                }
                catch (COMException e) when (e.ErrorCode == NativeMethods.DISP_E_MEMBERNOTFOUND)
                {
                    // Not all objects provide the select function.
                }
            }
        }

        private object AsVariant(AccessibleObject obj)
        {
            if (obj == this)
            {
                return NativeMethods.CHILDID_SELF;
            }

            return AsIAccessible(obj);
        }

        private IAccessible AsIAccessible(AccessibleObject obj)
        {
            if (obj != null && obj.systemWrapper)
            {
                return obj.systemIAccessible;
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
        internal int AccessibleObjectId { get; set; } = NativeMethods.OBJID_CLIENT;

        /// <summary>
        ///  Indicates whether this accessible object represents the client area of
        ///  the window.
        /// </summary>
        internal bool IsClientObject => AccessibleObjectId == NativeMethods.OBJID_CLIENT;

        /// <summary>
        ///  Indicates whether this accessible object represents the non-client
        ///  area of the window.
        /// </summary>
        internal bool IsNonClientObject => AccessibleObjectId == NativeMethods.OBJID_WINDOW;

        internal IAccessible GetSystemIAccessibleInternal() => systemIAccessible;

        protected void UseStdAccessibleObjects(IntPtr handle)
        {
            UseStdAccessibleObjects(handle, AccessibleObjectId);
        }

        protected void UseStdAccessibleObjects(IntPtr handle, int objid)
        {
            // Get a standard accessible Object
            Guid IID_IAccessible = new Guid(NativeMethods.uuid_IAccessible);
            object acc = null;
            int result = UnsafeNativeMethods.CreateStdAccessibleObject(
                            new HandleRef(this, handle),
                            objid,
                            ref IID_IAccessible,
                            ref acc);

            // Get the IEnumVariant interface
            Guid IID_IEnumVariant = new Guid(NativeMethods.uuid_IEnumVariant);
            object en = null;
            result = UnsafeNativeMethods.CreateStdAccessibleObject(
                        new HandleRef(this, handle),
                        objid,
                        ref IID_IEnumVariant,
                        ref en);

            if (acc != null || en != null)
            {
                systemIAccessible = (IAccessible)acc;
                systemIEnumVariant = (UnsafeNativeMethods.IEnumVariant)en;
                systemIOleWindow = acc as UnsafeNativeMethods.IOleWindow;
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
        private bool SysNavigate(int navDir, object childID, out object retObject)
        {
            retObject = null;

            // Only override system navigation relative to ourselves (since we can't interpret OLEACC child ids)
            if (!childID.Equals(NativeMethods.CHILDID_SELF))
            {
                return false;
            }

            // Perform any supported navigation operation (fall back on system for unsupported navigation ops)
            if (!GetSysChild((AccessibleNavigation)navDir, out AccessibleObject newObject))
            {
                return false;
            }

            // If object found, wrap in a VARIANT. Otherwise return null for 'end of list' (OLEACC expects this)
            retObject = (newObject == null) ? null : AsVariant(newObject);

            // Tell caller not to fall back on system behavior now
            return true;
        }

        /// <summary>
        ///  Make sure that the childID is valid.
        /// </summary>
        internal void ValidateChildID(ref object childID)
        {
            // An empty childID is considered to be the same as CHILDID_SELF.
            // Some accessibility programs pass null into our functions, so we
            // need to convert them here.
            if (childID == null)
            {
                childID = NativeMethods.CHILDID_SELF;
            }
            else if (childID.Equals(NativeMethods.DISP_E_PARAMNOTFOUND))
            {
                childID = 0;
            }
            else if (!(childID is int))
            {
                // AccExplorer seems to occasionally pass in objects instead of an int ChildID.
                childID = 0;
            }
        }

        private AccessibleObject WrapIAccessible(object iacc)
        {
            if (!(iacc is IAccessible accessible))
            {
                return null;
            }

            // Check to see if this object already wraps iacc
            if (systemIAccessible == iacc)
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
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(IAccessible).GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        /// <summary>
        ///  Return the requested method if it is implemented by the Reflection object. The
        ///  match is based upon the name of the method. If the object implementes multiple methods
        ///  with the same name an AmbiguousMatchException is thrown.
        /// </summary>
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetMethod(name, bindingAttr);
        }

        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetMethods(bindingAttr);
        }

        /// <summary>
        ///  Return the requestion field if it is implemented by the Reflection
        ///  object. The match is based upon a name. There cannot be more than
        ///  a single field with a name.
        /// </summary>
        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetField(name, bindingAttr);
        }

        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetFields(bindingAttr);
        }

        /// <summary>
        ///  Return the property based upon name. If more than one property has
        ///  the given name an AmbiguousMatchException will be thrown. Returns
        ///  null if no property is found.
        /// </summary>
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetProperty(name, bindingAttr);
        }

        /// <summary>
        ///  Return the property based upon the name and Descriptor info describing
        ///  the property indexing. Return null if no property is found.
        /// </summary>
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return typeof(IAccessible).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        /// <summary>
        ///  Returns an array of PropertyInfos for all the properties defined on
        ///  the Reflection object.
        /// </summary>
        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetProperties(bindingAttr);
        }

        /// <summary>
        ///  Return an array of members which match the passed in name.
        /// </summary>
        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetMember(name, bindingAttr);
        }

        /// <summary>
        ///  Return an array of all of the members defined for this object.
        /// </summary>
        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
        {
            return typeof(IAccessible).GetMembers(bindingAttr);
        }

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
        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            if (args.Length == 0)
            {
                MemberInfo[] member = typeof(IAccessible).GetMember(name);
                if (member != null && member.Length > 0 && member[0] is PropertyInfo)
                {
                    MethodInfo getMethod = ((PropertyInfo)member[0]).GetGetMethod();
                    if (getMethod != null && getMethod.GetParameters().Length > 0)
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

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderHwndOverride.GetOverrideProviderForHwnd(IntPtr hwnd)
        {
            return GetOverrideProviderForHwnd(hwnd);
        }

        bool UnsafeNativeMethods.IRangeValueProvider.IsReadOnly => IsReadOnly;

        double UnsafeNativeMethods.IRangeValueProvider.LargeChange => LargeChange;

        double UnsafeNativeMethods.IRangeValueProvider.Maximum => Maximum;

        double UnsafeNativeMethods.IRangeValueProvider.Minimum => Minimum;

        double UnsafeNativeMethods.IRangeValueProvider.SmallChange => SmallChange;

        double UnsafeNativeMethods.IRangeValueProvider.Value => RangeValue;

        void UnsafeNativeMethods.IRangeValueProvider.SetValue(double value)
        {
            SetValue(value);
        }

        object[] UnsafeNativeMethods.ISelectionProvider.GetSelection()
        {
            return GetSelection();
        }

        bool UnsafeNativeMethods.ISelectionProvider.CanSelectMultiple => CanSelectMultiple;

        bool UnsafeNativeMethods.ISelectionProvider.IsSelectionRequired => IsSelectionRequired;

        void UnsafeNativeMethods.ISelectionItemProvider.Select() => SelectItem();

        void UnsafeNativeMethods.ISelectionItemProvider.AddToSelection()
        {
            AddToSelection();
        }

        void UnsafeNativeMethods.ISelectionItemProvider.RemoveFromSelection()
        {
            RemoveFromSelection();
        }

        bool UnsafeNativeMethods.ISelectionItemProvider.IsSelected => IsItemSelected;

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.ISelectionItemProvider.SelectionContainer
        {
            get => ItemSelectionContainer;
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
        public bool RaiseAutomationNotification(AutomationNotificationKind notificationKind, AutomationNotificationProcessing notificationProcessing, string notificationText)
        {
            if (!notificationEventAvailable)
            {
                return false;
            }

            int result = NativeMethods.S_FALSE;
            try
            {
                // The activityId can be any string. It cannot be null. It is not used currently.
                result = UnsafeNativeMethods.UiaRaiseNotificationEvent(
                    this,
                    notificationKind,
                    notificationProcessing,
                    notificationText,
                    string.Empty);
            }
            catch (EntryPointNotFoundException)
            {
                // The UIA Notification event is not available, so don't attempt to raise it again.
                notificationEventAvailable = false;
            }

            return result == NativeMethods.S_OK;
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

        internal bool RaiseAutomationEvent(int eventId)
        {
            if (UnsafeNativeMethods.UiaClientsAreListening())
            {
                int result = UnsafeNativeMethods.UiaRaiseAutomationEvent(this, eventId);
                return result == NativeMethods.S_OK;
            }

            return false;
        }

        internal bool RaiseAutomationPropertyChangedEvent(int propertyId, object oldValue, object newValue)
        {
            if (UnsafeNativeMethods.UiaClientsAreListening())
            {
                int result = UnsafeNativeMethods.UiaRaiseAutomationPropertyChangedEvent(this, propertyId, oldValue, newValue);
                return result == NativeMethods.S_OK;
            }

            return false;
        }

        internal bool RaiseStructureChangedEvent(UnsafeNativeMethods.StructureChangeType structureChangeType, int[] runtimeId)
        {
            if (UnsafeNativeMethods.UiaClientsAreListening())
            {
                int result = UnsafeNativeMethods.UiaRaiseStructureChangedEvent(this, structureChangeType, runtimeId, runtimeId == null ? 0 : runtimeId.Length);
                return result == NativeMethods.S_OK;
            }

            return false;
        }

        void UnsafeNativeMethods.IScrollItemProvider.ScrollIntoView()
        {
            ScrollIntoView();
        }

        internal virtual void ScrollIntoView()
        {
            Debug.Fail($"{nameof(ScrollIntoView)}() is not overriden");
        }

        private class EnumVariantObject : UnsafeNativeMethods.IEnumVariant
        {
            private int currentChild = 0;
            private readonly AccessibleObject owner;

            public EnumVariantObject(AccessibleObject owner)
            {
                Debug.Assert(owner != null, "Cannot create EnumVariantObject with a null owner");
                this.owner = owner;
            }

            public EnumVariantObject(AccessibleObject owner, int currentChild)
            {
                Debug.Assert(owner != null, "Cannot create EnumVariantObject with a null owner");
                this.owner = owner;
                this.currentChild = currentChild;
            }

            void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v)
            {
                v[0] = new EnumVariantObject(owner, currentChild);
            }

            /// <summary>
            ///  Resets the child accessible object enumerator.
            /// </summary>
            void UnsafeNativeMethods.IEnumVariant.Reset()
            {
                currentChild = 0;
                owner.systemIEnumVariant?.Reset();
            }

            /// <summary>
            ///  Skips the next n child accessible objects.
            /// </summary>
            void UnsafeNativeMethods.IEnumVariant.Skip(int n)
            {
                currentChild += n;
                owner.systemIEnumVariant?.Skip(n);
            }

            /// <summary>
            ///  Gets the next n child accessible objects.
            /// </summary>
            int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns)
            {
                // NOTE: rgvar is a pointer to an array of variants
                if (owner.IsClientObject)
                {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "EnumVariantObject: owner = " + owner.ToString() + ", n = " + n);

                    Debug.Indent();

                    int childCount;
                    int[] newOrder;

                    if ((childCount = owner.GetChildCount()) >= 0)
                    {
                        NextFromChildCollection(n, rgvar, ns, childCount);
                    }
                    else if (owner.systemIEnumVariant == null)
                    {
                        NextEmpty(n, rgvar, ns);
                    }
                    else if ((newOrder = owner.GetSysChildOrder()) != null)
                    {
                        NextFromSystemReordered(n, rgvar, ns, newOrder);
                    }
                    else
                    {
                        NextFromSystem(n, rgvar, ns);
                    }

                    Debug.Unindent();
                }
                else
                {
                    NextFromSystem(n, rgvar, ns);
                }

                // Tell caller whether requested number of items was returned. Once list of items has
                // been exhausted, we return S_FALSE so that caller knows to stop calling this method.
                return (ns[0] == n) ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }

            /// <summary>
            ///  When we have the IEnumVariant of an accessible proxy provided by the system (ie.
            ///  OLEACC.DLL), we can fall back on that to return the children. Generally, the system
            ///  proxy will enumerate the child windows, create a suitable kind of child accessible
            ///  proxy for each one, and return a set of IDispatch interfaces to these proxy objects.
            /// </summary>
            private void NextFromSystem(int n, IntPtr rgvar, int[] ns)
            {
                owner.systemIEnumVariant.Next(n, rgvar, ns);

                currentChild += ns[0];
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: Delegating to systemIEnumVariant");
            }

            /// <summary>
            ///  Sometimes we want to rely on the system-provided behavior to create
            ///  and return child accessible objects, but we want to impose a new
            ///  order on those objects (or even filter some objects out).
            ///
            ///  This method takes an array of ints that dictates the new order.
            ///  It queries the system for each child individually, and inserts the
            ///  result into the correct *new* position.
            ///
            ///  Note: This code has to make certain *assumptions* about OLEACC.DLL
            ///  proxy object behavior. However, this behavior is well documented.
            ///  We *assume* the proxy will return a set of child accessible objects
            ///  that correspond 1:1 with the owning control's child windows, and
            ///  that the default order it returns these objects in is z-order
            ///  (which also happens to be the order that children appear in the
            ///  Control.Controls[] collection).
            /// </summary>
            private void NextFromSystemReordered(int n, IntPtr rgvar, int[] ns, int[] newOrder)
            {
                int i;

                for (i = 0; i < n && currentChild < newOrder.Length; ++i)
                {
                    if (!GotoItem(owner.systemIEnumVariant, newOrder[currentChild], GetAddressOfVariantAtIndex(rgvar, i)))
                    {
                        break;
                    }

                    ++currentChild;
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: adding sys child " + currentChild + " of " + newOrder.Length);
                }

                ns[0] = i;
            }

            /// <summary>
            ///  If we have our own custom accessible child collection, return a set
            ///  of 1-based integer child ids, that the caller will eventually pass
            ///  back to us via IAccessible.get_accChild().
            /// </summary>
            private void NextFromChildCollection(int n, IntPtr rgvar, int[] ns, int childCount)
            {
                int i;

                for (i = 0; i < n && currentChild < childCount; ++i)
                {
                    ++currentChild;
                    Marshal.GetNativeVariantForObject(((object)currentChild), GetAddressOfVariantAtIndex(rgvar, i));
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: adding own child " + currentChild + " of " + childCount);
                }

                ns[0] = i;
            }

            /// <summary>
            ///  Default behavior if there is no custom child collection or
            ///  system-provided proxy to fall back on. In this case, we return
            ///  an empty child collection.
            /// </summary>
            private void NextEmpty(int n, IntPtr rgvar, int[] ns)
            {
                ns[0] = 0;
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: no children to add");
            }

            /// <summary>
            ///  Given an IEnumVariant interface, this method jumps to a specific
            ///  item in the collection and extracts the result for that one item.
            /// </summary>
            private static bool GotoItem(UnsafeNativeMethods.IEnumVariant iev, int index, IntPtr variantPtr)
            {
                int[] ns = new int[1];

                iev.Reset();
                iev.Skip(index);
                iev.Next(1, variantPtr, ns);

                return ns[0] == 1;
            }

            /// <summary>
            ///  Given an array of pointers to variants, calculate address of a given array element.
            /// </summary>
            private static IntPtr GetAddressOfVariantAtIndex(IntPtr variantArrayPtr, int index)
            {
                int variantSize = 8 + (IntPtr.Size * 2);
                return (IntPtr)((ulong)variantArrayPtr + ((ulong)index) * ((ulong)variantSize));
            }

        }

    }

    /// <Summary>
    ///  Internal object passed out to OLEACC clients via WM_GETOBJECT.
    /// </Summary>
    internal sealed class InternalAccessibleObject : StandardOleMarshalObject,
                                    UnsafeNativeMethods.IAccessibleInternal,
                                    IReflect,
                                    UnsafeNativeMethods.IServiceProvider,
                                    UnsafeNativeMethods.IAccessibleEx,
                                    UnsafeNativeMethods.IRawElementProviderSimple,
                                    UnsafeNativeMethods.IRawElementProviderFragment,
                                    UnsafeNativeMethods.IRawElementProviderFragmentRoot,
                                    UnsafeNativeMethods.IInvokeProvider,
                                    UnsafeNativeMethods.IValueProvider,
                                    UnsafeNativeMethods.IRangeValueProvider,
                                    UnsafeNativeMethods.IExpandCollapseProvider,
                                    UnsafeNativeMethods.IToggleProvider,
                                    UnsafeNativeMethods.ITableProvider,
                                    UnsafeNativeMethods.ITableItemProvider,
                                    UnsafeNativeMethods.IGridProvider,
                                    UnsafeNativeMethods.IGridItemProvider,
                                    UnsafeNativeMethods.IEnumVariant,
                                    UnsafeNativeMethods.IOleWindow,
                                    UnsafeNativeMethods.ILegacyIAccessibleProvider,
                                    UnsafeNativeMethods.ISelectionProvider,
                                    UnsafeNativeMethods.ISelectionItemProvider,
                                    UnsafeNativeMethods.IScrollItemProvider,
                                    UnsafeNativeMethods.IRawElementProviderHwndOverride
    {
        private IAccessible publicIAccessible;                       // AccessibleObject as IAccessible
        private readonly UnsafeNativeMethods.IEnumVariant publicIEnumVariant; // AccessibleObject as IEnumVariant
        private readonly UnsafeNativeMethods.IOleWindow publicIOleWindow;     // AccessibleObject as IOleWindow
        private readonly IReflect publicIReflect;                             // AccessibleObject as IReflect

        private readonly UnsafeNativeMethods.IServiceProvider publicIServiceProvider;             // AccessibleObject as IServiceProvider
        private readonly UnsafeNativeMethods.IAccessibleEx publicIAccessibleEx;                   // AccessibleObject as IAccessibleEx

        // UIAutomation
        private readonly UnsafeNativeMethods.IRawElementProviderSimple publicIRawElementProviderSimple;    // AccessibleObject as IRawElementProviderSimple
        private readonly UnsafeNativeMethods.IRawElementProviderFragment publicIRawElementProviderFragment;// AccessibleObject as IRawElementProviderFragment
        private readonly UnsafeNativeMethods.IRawElementProviderFragmentRoot publicIRawElementProviderFragmentRoot;// AccessibleObject as IRawElementProviderFragmentRoot
        private readonly UnsafeNativeMethods.IInvokeProvider publicIInvokeProvider;                        // AccessibleObject as IInvokeProvider
        private readonly UnsafeNativeMethods.IValueProvider publicIValueProvider;                          // AccessibleObject as IValueProvider
        private readonly UnsafeNativeMethods.IRangeValueProvider publicIRangeValueProvider;                // AccessibleObject as IRangeValueProvider
        private readonly UnsafeNativeMethods.IExpandCollapseProvider publicIExpandCollapseProvider;        // AccessibleObject as IExpandCollapseProvider
        private readonly UnsafeNativeMethods.IToggleProvider publicIToggleProvider;                        // AccessibleObject as IToggleProvider
        private readonly UnsafeNativeMethods.ITableProvider publicITableProvider;                          // AccessibleObject as ITableProvider
        private readonly UnsafeNativeMethods.ITableItemProvider publicITableItemProvider;                  // AccessibleObject as ITableItemProvider
        private readonly UnsafeNativeMethods.IGridProvider publicIGridProvider;                            // AccessibleObject as IGridProvider
        private readonly UnsafeNativeMethods.IGridItemProvider publicIGridItemProvider;                    // AccessibleObject as IGridItemProvider
        private readonly UnsafeNativeMethods.ILegacyIAccessibleProvider publicILegacyIAccessibleProvider;   // AccessibleObject as ILegayAccessibleProvider
        private readonly UnsafeNativeMethods.ISelectionProvider publicISelectionProvider;                  // AccessibleObject as ISelectionProvider
        private readonly UnsafeNativeMethods.ISelectionItemProvider publicISelectionItemProvider;          // AccessibleObject as ISelectionItemProvider
        private readonly UnsafeNativeMethods.IScrollItemProvider publicIScrollItemProvider;          // AccessibleObject as IScrollItemProvider
        private readonly UnsafeNativeMethods.IRawElementProviderHwndOverride publicIRawElementProviderHwndOverride; // AccessibleObject as IRawElementProviderHwndOverride

        /// <summary>
        ///  Create a new wrapper.
        /// </summary>
        internal InternalAccessibleObject(AccessibleObject accessibleImplemention)
        {
            // Get all the casts done here to catch any issues early
            publicIAccessible = (IAccessible)accessibleImplemention;
            publicIEnumVariant = (UnsafeNativeMethods.IEnumVariant)accessibleImplemention;
            publicIOleWindow = (UnsafeNativeMethods.IOleWindow)accessibleImplemention;
            publicIReflect = (IReflect)accessibleImplemention;
            publicIServiceProvider = (UnsafeNativeMethods.IServiceProvider)accessibleImplemention;
            publicIAccessibleEx = (UnsafeNativeMethods.IAccessibleEx)accessibleImplemention;
            publicIRawElementProviderSimple = (UnsafeNativeMethods.IRawElementProviderSimple)accessibleImplemention;
            publicIRawElementProviderFragment = (UnsafeNativeMethods.IRawElementProviderFragment)accessibleImplemention;
            publicIRawElementProviderFragmentRoot = (UnsafeNativeMethods.IRawElementProviderFragmentRoot)accessibleImplemention;
            publicIInvokeProvider = (UnsafeNativeMethods.IInvokeProvider)accessibleImplemention;
            publicIValueProvider = (UnsafeNativeMethods.IValueProvider)accessibleImplemention;
            publicIRangeValueProvider = (UnsafeNativeMethods.IRangeValueProvider)accessibleImplemention;
            publicIExpandCollapseProvider = (UnsafeNativeMethods.IExpandCollapseProvider)accessibleImplemention;
            publicIToggleProvider = (UnsafeNativeMethods.IToggleProvider)accessibleImplemention;
            publicITableProvider = (UnsafeNativeMethods.ITableProvider)accessibleImplemention;
            publicITableItemProvider = (UnsafeNativeMethods.ITableItemProvider)accessibleImplemention;
            publicIGridProvider = (UnsafeNativeMethods.IGridProvider)accessibleImplemention;
            publicIGridItemProvider = (UnsafeNativeMethods.IGridItemProvider)accessibleImplemention;
            publicILegacyIAccessibleProvider = (UnsafeNativeMethods.ILegacyIAccessibleProvider)accessibleImplemention;
            publicISelectionProvider = (UnsafeNativeMethods.ISelectionProvider)accessibleImplemention;
            publicISelectionItemProvider = (UnsafeNativeMethods.ISelectionItemProvider)accessibleImplemention;
            publicIScrollItemProvider = (UnsafeNativeMethods.IScrollItemProvider)accessibleImplemention;
            publicIRawElementProviderHwndOverride = (UnsafeNativeMethods.IRawElementProviderHwndOverride)accessibleImplemention;
            // Note: Deliberately not holding onto AccessibleObject to enforce all access through the interfaces
        }

        /// <summary>
        ///  If the given object is an AccessibleObject return it as a InternalAccessibleObject
        ///  This ensures we wrap all AccessibleObjects before handing them out to OLEACC
        /// </summary>
        private object AsNativeAccessible(object accObject)
        {
            if (accObject is AccessibleObject)
            {
                return new InternalAccessibleObject(accObject as AccessibleObject);
            }
            else
            {
                return accObject;
            }
        }

        /// <summary>
        ///  Wraps AccessibleObject elements of a given array into InternalAccessibleObjects
        /// </summary>
        private object[] AsArrayOfNativeAccessibles(object[] accObjectArray)
        {
            if (accObjectArray != null && accObjectArray.Length > 0)
            {
                for (int i = 0; i < accObjectArray.Length; i++)
                {
                    accObjectArray[i] = AsNativeAccessible(accObjectArray[i]);
                }
            }
            return accObjectArray;
        }

        void UnsafeNativeMethods.IAccessibleInternal.accDoDefaultAction(object childID)
        {
            publicIAccessible.accDoDefaultAction(childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.accHitTest(int xLeft, int yTop)
        {
            return AsNativeAccessible(publicIAccessible.accHitTest(xLeft, yTop));
        }

        void UnsafeNativeMethods.IAccessibleInternal.accLocation(out int l, out int t, out int w, out int h, object childID)
        {
            publicIAccessible.accLocation(out l, out t, out w, out h, childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.accNavigate(int navDir, object childID)
        {
            return AsNativeAccessible(publicIAccessible.accNavigate(navDir, childID));
        }

        void UnsafeNativeMethods.IAccessibleInternal.accSelect(int flagsSelect, object childID)
        {
            publicIAccessible.accSelect(flagsSelect, childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accChild(object childID)
        {
            return AsNativeAccessible(publicIAccessible.get_accChild(childID));
        }

        int UnsafeNativeMethods.IAccessibleInternal.get_accChildCount()
        {
            return publicIAccessible.accChildCount;
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accDefaultAction(object childID)
        {
            return publicIAccessible.get_accDefaultAction(childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accDescription(object childID)
        {
            return publicIAccessible.get_accDescription(childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accFocus()
        {
            return AsNativeAccessible(publicIAccessible.accFocus);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accHelp(object childID)
        {
            return publicIAccessible.get_accHelp(childID);
        }

        int UnsafeNativeMethods.IAccessibleInternal.get_accHelpTopic(out string pszHelpFile, object childID)
        {
            return publicIAccessible.get_accHelpTopic(out pszHelpFile, childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accKeyboardShortcut(object childID)
        {
            return publicIAccessible.get_accKeyboardShortcut(childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accName(object childID)
        {
            return publicIAccessible.get_accName(childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accParent()
        {
            return AsNativeAccessible(publicIAccessible.accParent);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accRole(object childID)
        {
            return publicIAccessible.get_accRole(childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accSelection()
        {
            return AsNativeAccessible(publicIAccessible.accSelection);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accState(object childID)
        {
            return publicIAccessible.get_accState(childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accValue(object childID)
        {
            return publicIAccessible.get_accValue(childID);
        }

        void UnsafeNativeMethods.IAccessibleInternal.set_accName(object childID, string newName)
        {
            publicIAccessible.set_accName(childID, newName);
        }

        void UnsafeNativeMethods.IAccessibleInternal.set_accValue(object childID, string newValue)
        {
            publicIAccessible.set_accValue(childID, newValue);
        }

        void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v)
        {
            publicIEnumVariant.Clone(v);
        }

        int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns)
        {
            return publicIEnumVariant.Next(n, rgvar, ns);
        }

        void UnsafeNativeMethods.IEnumVariant.Reset()
        {
            publicIEnumVariant.Reset();
        }

        void UnsafeNativeMethods.IEnumVariant.Skip(int n)
        {
            publicIEnumVariant.Skip(n);
        }

        int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd)
        {
            return publicIOleWindow.GetWindow(out hwnd);
        }

        void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode)
        {
            publicIOleWindow.ContextSensitiveHelp(fEnterMode);
        }

        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
        {
            return publicIReflect.GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr)
        {
            return publicIReflect.GetMethod(name, bindingAttr);
        }

        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr)
        {
            return publicIReflect.GetMethods(bindingAttr);
        }

        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr)
        {
            return publicIReflect.GetField(name, bindingAttr);
        }

        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr)
        {
            return publicIReflect.GetFields(bindingAttr);
        }

        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr)
        {
            return publicIReflect.GetProperty(name, bindingAttr);
        }

        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
        {
            return publicIReflect.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr)
        {
            return publicIReflect.GetProperties(bindingAttr);
        }

        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr)
        {
            return publicIReflect.GetMember(name, bindingAttr);
        }

        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr)
        {
            return publicIReflect.GetMembers(bindingAttr);
        }

        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters)
        {
            return publicIReflect.InvokeMember(name, invokeAttr, binder, publicIAccessible, args, modifiers, culture, namedParameters);
        }

        Type IReflect.UnderlyingSystemType => publicIReflect.UnderlyingSystemType;

        int UnsafeNativeMethods.IServiceProvider.QueryService(ref Guid service, ref Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;
            int hr = publicIServiceProvider.QueryService(ref service, ref riid, out ppvObject);
            if (hr >= NativeMethods.S_OK)
            {
                // we always want to return the internal accessible object
                ppvObject = Marshal.GetComInterfaceForObject(this, typeof(UnsafeNativeMethods.IAccessibleEx));
            }

            return hr;
        }

        object UnsafeNativeMethods.IAccessibleEx.GetObjectForChild(int idChild)
        {
            return publicIAccessibleEx.GetObjectForChild(idChild);
        }

        int UnsafeNativeMethods.IAccessibleEx.GetIAccessiblePair(out object ppAcc, out int pidChild)
        {
            // We always want to return the internal accessible object
            ppAcc = this;
            pidChild = NativeMethods.CHILDID_SELF;
            return NativeMethods.S_OK;
        }

        int[] UnsafeNativeMethods.IAccessibleEx.GetRuntimeId()
        {
            return publicIAccessibleEx.GetRuntimeId();
        }

        int UnsafeNativeMethods.IAccessibleEx.ConvertReturnedElement(object pIn, out object ppRetValOut)
        {
            return publicIAccessibleEx.ConvertReturnedElement(pIn, out ppRetValOut);
        }

        UnsafeNativeMethods.ProviderOptions UnsafeNativeMethods.IRawElementProviderSimple.ProviderOptions
        {
            get => publicIRawElementProviderSimple.ProviderOptions;
        }

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderSimple.HostRawElementProvider
        {
            get => publicIRawElementProviderSimple.HostRawElementProvider;
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPatternProvider(int patternId)
        {
            object obj = publicIRawElementProviderSimple.GetPatternProvider(patternId);
            if (obj != null)
            {
                // we always want to return the internal accessible object
                if (patternId == NativeMethods.UIA_ExpandCollapsePatternId)
                {
                    return (UnsafeNativeMethods.IExpandCollapseProvider)this;
                }
                else if (patternId == NativeMethods.UIA_ValuePatternId)
                {
                    return (UnsafeNativeMethods.IValueProvider)this;
                }
                else if (patternId == NativeMethods.UIA_RangeValuePatternId)
                {
                    return (UnsafeNativeMethods.IRangeValueProvider)this;
                }
                else if (patternId == NativeMethods.UIA_TogglePatternId)
                {
                    return (UnsafeNativeMethods.IToggleProvider)this;
                }
                else if (patternId == NativeMethods.UIA_TablePatternId)
                {
                    return (UnsafeNativeMethods.ITableProvider)this;
                }
                else if (patternId == NativeMethods.UIA_TableItemPatternId)
                {
                    return (UnsafeNativeMethods.ITableItemProvider)this;
                }
                else if (patternId == NativeMethods.UIA_GridPatternId)
                {
                    return (UnsafeNativeMethods.IGridProvider)this;
                }
                else if (patternId == NativeMethods.UIA_GridItemPatternId)
                {
                    return (UnsafeNativeMethods.IGridItemProvider)this;
                }
                else if (patternId == NativeMethods.UIA_InvokePatternId)
                {
                    return (UnsafeNativeMethods.IInvokeProvider)this;
                }
                else if (patternId == NativeMethods.UIA_LegacyIAccessiblePatternId)
                {
                    return (UnsafeNativeMethods.ILegacyIAccessibleProvider)this;
                }
                else if (patternId == NativeMethods.UIA_SelectionPatternId)
                {
                    return (UnsafeNativeMethods.ISelectionProvider)this;
                }
                else if (patternId == NativeMethods.UIA_SelectionItemPatternId)
                {
                    return (UnsafeNativeMethods.ISelectionItemProvider)this;
                }
                else if (patternId == NativeMethods.UIA_ScrollItemPatternId)
                {
                    return (UnsafeNativeMethods.IScrollItemProvider)this;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPropertyValue(int propertyID)
        {
            return publicIRawElementProviderSimple.GetPropertyValue(propertyID);
        }

        object UnsafeNativeMethods.IRawElementProviderFragment.Navigate(UnsafeNativeMethods.NavigateDirection direction)
        {
            return AsNativeAccessible(publicIRawElementProviderFragment.Navigate(direction));
        }

        int[] UnsafeNativeMethods.IRawElementProviderFragment.GetRuntimeId()
        {
            return publicIRawElementProviderFragment.GetRuntimeId();
        }

        object[] UnsafeNativeMethods.IRawElementProviderFragment.GetEmbeddedFragmentRoots()
        {
            return AsArrayOfNativeAccessibles(publicIRawElementProviderFragment.GetEmbeddedFragmentRoots());
        }

        void UnsafeNativeMethods.IRawElementProviderFragment.SetFocus()
        {
            publicIRawElementProviderFragment.SetFocus();
        }

        NativeMethods.UiaRect UnsafeNativeMethods.IRawElementProviderFragment.BoundingRectangle
        {
            get => publicIRawElementProviderFragment.BoundingRectangle;
        }

        UnsafeNativeMethods.IRawElementProviderFragmentRoot UnsafeNativeMethods.IRawElementProviderFragment.FragmentRoot
        {
            get => publicIRawElementProviderFragment.FragmentRoot;
        }

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y)
        {
            return AsNativeAccessible(publicIRawElementProviderFragmentRoot.ElementProviderFromPoint(x, y));
        }

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.GetFocus()
        {
            return AsNativeAccessible(publicIRawElementProviderFragmentRoot.GetFocus());
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.DefaultAction
        {
            get => publicILegacyIAccessibleProvider.DefaultAction;
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Description
        {
            get => publicILegacyIAccessibleProvider.Description;
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Help
        {
            get => publicILegacyIAccessibleProvider.Help;
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.KeyboardShortcut
        {
            get => publicILegacyIAccessibleProvider.KeyboardShortcut;
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Name
        {
            get => publicILegacyIAccessibleProvider.Name;
        }

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.Role
        {
            get => publicILegacyIAccessibleProvider.Role;
        }

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.State
        {
            get => publicILegacyIAccessibleProvider.State;
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Value
        {
            get => publicILegacyIAccessibleProvider.Value;
        }

        int UnsafeNativeMethods.ILegacyIAccessibleProvider.ChildId
        {
            get => publicILegacyIAccessibleProvider.ChildId;
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.DoDefaultAction()
        {
            publicILegacyIAccessibleProvider.DoDefaultAction();
        }

        IAccessible UnsafeNativeMethods.ILegacyIAccessibleProvider.GetIAccessible()
        {
            return publicILegacyIAccessibleProvider.GetIAccessible();
        }

        object[] UnsafeNativeMethods.ILegacyIAccessibleProvider.GetSelection()
        {
            return AsArrayOfNativeAccessibles(publicILegacyIAccessibleProvider.GetSelection());
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.Select(int flagsSelect)
        {
            publicILegacyIAccessibleProvider.Select(flagsSelect);
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.SetValue(string szValue)
        {
            publicILegacyIAccessibleProvider.SetValue(szValue);
        }

        void UnsafeNativeMethods.IInvokeProvider.Invoke()
        {
            publicIInvokeProvider.Invoke();
        }

        bool UnsafeNativeMethods.IValueProvider.IsReadOnly => publicIValueProvider.IsReadOnly;

        string UnsafeNativeMethods.IValueProvider.Value => publicIValueProvider.Value;

        void UnsafeNativeMethods.IValueProvider.SetValue(string newValue)
        {
            publicIValueProvider.SetValue(newValue);
        }

        bool UnsafeNativeMethods.IRangeValueProvider.IsReadOnly => publicIValueProvider.IsReadOnly;

        double UnsafeNativeMethods.IRangeValueProvider.LargeChange => publicIRangeValueProvider.LargeChange;

        double UnsafeNativeMethods.IRangeValueProvider.Maximum => publicIRangeValueProvider.Maximum;

        double UnsafeNativeMethods.IRangeValueProvider.Minimum => publicIRangeValueProvider.Minimum;

        double UnsafeNativeMethods.IRangeValueProvider.SmallChange => publicIRangeValueProvider.SmallChange;

        double UnsafeNativeMethods.IRangeValueProvider.Value => publicIRangeValueProvider.Value;

        void UnsafeNativeMethods.IRangeValueProvider.SetValue(double newValue)
        {
            publicIRangeValueProvider.SetValue(newValue);
        }

        void UnsafeNativeMethods.IExpandCollapseProvider.Expand()
        {
            publicIExpandCollapseProvider.Expand();
        }

        void UnsafeNativeMethods.IExpandCollapseProvider.Collapse()
        {
            publicIExpandCollapseProvider.Collapse();
        }

        UnsafeNativeMethods.ExpandCollapseState UnsafeNativeMethods.IExpandCollapseProvider.ExpandCollapseState
        {
            get => publicIExpandCollapseProvider.ExpandCollapseState;
        }

        void UnsafeNativeMethods.IToggleProvider.Toggle()
        {
            publicIToggleProvider.Toggle();
        }

        UnsafeNativeMethods.ToggleState UnsafeNativeMethods.IToggleProvider.ToggleState
        {
            get => publicIToggleProvider.ToggleState;
        }

        object[] UnsafeNativeMethods.ITableProvider.GetRowHeaders()
        {
            return AsArrayOfNativeAccessibles(publicITableProvider.GetRowHeaders());
        }

        object[] UnsafeNativeMethods.ITableProvider.GetColumnHeaders()
        {
            return AsArrayOfNativeAccessibles(publicITableProvider.GetColumnHeaders());
        }

        UnsafeNativeMethods.RowOrColumnMajor UnsafeNativeMethods.ITableProvider.RowOrColumnMajor
        {
            get => publicITableProvider.RowOrColumnMajor;
        }

        object[] UnsafeNativeMethods.ITableItemProvider.GetRowHeaderItems()
        {
            return AsArrayOfNativeAccessibles(publicITableItemProvider.GetRowHeaderItems());
        }

        object[] UnsafeNativeMethods.ITableItemProvider.GetColumnHeaderItems()
        {
            return AsArrayOfNativeAccessibles(publicITableItemProvider.GetColumnHeaderItems());
        }

        object UnsafeNativeMethods.IGridProvider.GetItem(int row, int column)
        {
            return AsNativeAccessible(publicIGridProvider.GetItem(row, column));
        }

        int UnsafeNativeMethods.IGridProvider.RowCount => publicIGridProvider.RowCount;

        int UnsafeNativeMethods.IGridProvider.ColumnCount => publicIGridProvider.ColumnCount;

        int UnsafeNativeMethods.IGridItemProvider.Row => publicIGridItemProvider.Row;

        int UnsafeNativeMethods.IGridItemProvider.Column => publicIGridItemProvider.Column;

        int UnsafeNativeMethods.IGridItemProvider.RowSpan => publicIGridItemProvider.RowSpan;

        int UnsafeNativeMethods.IGridItemProvider.ColumnSpan => publicIGridItemProvider.ColumnSpan;

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IGridItemProvider.ContainingGrid
        {
            get => publicIGridItemProvider.ContainingGrid;
        }

        /// <summary>
        ///  Get the currently selected elements
        /// </summary>
        /// <returns>An AutomationElement array containing the currently selected elements</returns>
        object[] UnsafeNativeMethods.ISelectionProvider.GetSelection()
        {
            return publicISelectionProvider.GetSelection();
        }

        /// <summary>
        ///  Indicates whether the control allows more than one element to be selected
        /// </summary>
        /// <returns>Boolean indicating whether the control allows more than one element to be selected</returns>
        /// <remarks>If this is false, then the control is a single-select ccntrol</remarks>
        bool UnsafeNativeMethods.ISelectionProvider.CanSelectMultiple
        {
            get => publicISelectionProvider.CanSelectMultiple;
        }

        /// <summary>
        ///  Indicates whether the control requires at least one element to be selected
        /// </summary>
        /// <returns>Boolean indicating whether the control requires at least one element to be selected</returns>
        /// <remarks>If this is false, then the control allows all elements to be unselected</remarks>
        bool UnsafeNativeMethods.ISelectionProvider.IsSelectionRequired
        {
            get => publicISelectionProvider.IsSelectionRequired;
        }

        /// <summary>
        ///  Sets the current element as the selection
        ///  This clears the selection from other elements in the container.
        /// </summary>
        void UnsafeNativeMethods.ISelectionItemProvider.Select()
        {
            publicISelectionItemProvider.Select();
        }

        /// <summary>
        ///  Adds current element to selection.
        /// </summary>
        void UnsafeNativeMethods.ISelectionItemProvider.AddToSelection()
        {
            publicISelectionItemProvider.AddToSelection();
        }

        /// <summary>
        ///  Removes current element from selection.
        /// </summary>
        void UnsafeNativeMethods.ISelectionItemProvider.RemoveFromSelection()
        {
            publicISelectionItemProvider.RemoveFromSelection();
        }

        /// <summary>
        ///  Check whether an element is selected.
        /// </summary>
        /// <returns>Returns true if the element is selected.</returns>
        bool UnsafeNativeMethods.ISelectionItemProvider.IsSelected
        {
            get => publicISelectionItemProvider.IsSelected;
        }

        void UnsafeNativeMethods.IScrollItemProvider.ScrollIntoView()
        {
            publicIScrollItemProvider.ScrollIntoView();
        }

        /// <summary>
        ///  The logical element that supports the SelectionPattern for this Item.
        /// </summary>
        /// <returns>Returns a IRawElementProviderSimple.</returns>
        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.ISelectionItemProvider.SelectionContainer
        {
            get => publicISelectionItemProvider.SelectionContainer;
        }

        /// <summary>
        ///  Request a provider for the specified component. The returned provider can supply additional
        ///  properties or override properties of the specified component.
        /// </summary>
        /// <param name="hwnd">The window handle of the component.</param>
        /// <returns>Return the provider for the specified component, or null if the component is not being overridden.</returns>
        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderHwndOverride.GetOverrideProviderForHwnd(IntPtr hwnd)
        {
            return publicIRawElementProviderHwndOverride.GetOverrideProviderForHwnd(hwnd);
        }
    }
}
