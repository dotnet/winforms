// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using Accessibility;
    using Automation;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Globalization;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject"]/*' />
    /// <devdoc>
    ///    <para>Provides an implementation for an object that can be inspected by an 
    ///       accessibility application.</para>    
    /// </devdoc>
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
                                    UnsafeNativeMethods.ILegacyIAccessibleProvider {

        // Member variables

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.systemIAccessible"]/*' />
        /// <devdoc>
        /// <para>Specifies the <see langword='IAccessible '/>interface used by this <see cref='System.Windows.Forms.AccessibleObject'/>.</para>
        /// </devdoc>
        private IAccessible systemIAccessible = null;

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.systemIEnumVariant"]/*' />
        /// <devdoc>
        ///    <para>Specifies the 
        ///    <see langword='NativeMethods.IEnumVariant '/>used by this <see cref='System.Windows.Forms.AccessibleObject'/> .</para>
        /// </devdoc>
        private UnsafeNativeMethods.IEnumVariant systemIEnumVariant = null;
        private UnsafeNativeMethods.IEnumVariant enumVariant = null;

        // IOleWindow interface of the 'inner' system IAccessible object that we are wrapping
        private UnsafeNativeMethods.IOleWindow systemIOleWindow = null;

        private bool systemWrapper = false;     // Indicates this object is being used ONLY to wrap a system IAccessible

        private int accObjId = NativeMethods.OBJID_CLIENT;    // Indicates what kind of 'inner' system accessible object we are using

        // The support for the UIA Notification event begins in RS3.
        // Assume the UIA Notification event is available until we learn otherwise.
        // If we learn that the UIA Notification event is not available, 
        // controls should not attempt to raise it.
        private static bool notificationEventAvailable = true;

        protected const int RuntimeIDFirstItem = 0x2a;

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.AccessibleObject"]/*' />
        public AccessibleObject() {
        }
        
        // This constructor is used ONLY for wrapping system IAccessible objects
        //
        private AccessibleObject(IAccessible iAcc) {
            this.systemIAccessible = iAcc;            
            this.systemWrapper = true;
        }
        
        // Properties

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Bounds"]/*' />
        /// <devdoc>
        ///    <para> Gets the bounds of the accessible object, in screen coordinates.</para>
        /// </devdoc>
        public virtual Rectangle Bounds {
            get {
                // Use the system provided bounds
                if (systemIAccessible != null) {
                    int left = 0;
                    int top = 0;
                    int width = 0;
                    int height = 0;
                    try {
                        systemIAccessible.accLocation(out left, out top, out width, out height, NativeMethods.CHILDID_SELF);
                        return new Rectangle(left, top, width, height);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }                        
                    }                    
                }
                return Rectangle.Empty;
            }
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.DefaultAction"]/*' />
        /// <devdoc>
        ///    <para>Gets a description of the default action for an object.</para>
        /// </devdoc>
        public virtual string DefaultAction {
            get {
                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.get_accDefaultAction(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) {
                        // Not all objects provide a default action
                        //
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                return null;
            }
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Description"]/*' />
        /// <devdoc>
        ///    <para>Gets a description
        ///       of the object's visual appearance to the user.</para>
        /// </devdoc>
        public virtual string Description {
            get {
                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.get_accDescription(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                return null;
            }
        }
        
        private UnsafeNativeMethods.IEnumVariant EnumVariant {
            get {
                if (enumVariant == null) {
                    enumVariant = new EnumVariantObject(this);
                }
                return enumVariant;                
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Help"]/*' />
        /// <devdoc>
        ///    <para>Gets a description of what the object does or how the object is used.</para>
        /// </devdoc>
        public virtual string Help {
            get {
                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.get_accHelp(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                return null;
            }
        } 
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.KeyboardShortcut"]/*' />
        /// <devdoc>
        ///    <para>Gets the object shortcut key or access key
        ///       for an accessible object.</para>
        /// </devdoc>
        public virtual string KeyboardShortcut {
            get {
                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.get_accKeyboardShortcut(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                return null;
            }
        } 
         
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Name"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or sets the object name.</para>
        /// </devdoc>
        public virtual string Name {
            // Does nothing by default
            get {
                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.get_accName(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                return null;
            }
            
            set {
                if (systemIAccessible != null) {
                    try {
                        systemIAccessible.set_accName(NativeMethods.CHILDID_SELF, value);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
            
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Parent"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, gets or sets the parent of an accessible object.</para>
        /// </devdoc>
        public virtual AccessibleObject Parent {
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get {
                if (systemIAccessible != null) {
                    return WrapIAccessible(systemIAccessible.accParent);
                }
                else {
                    return null;
                }
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Role"]/*' />
        /// <devdoc>
        ///    <para>Gets the role of this accessible object.</para>
        /// </devdoc>
        public virtual AccessibleRole Role {
            get {
                if (systemIAccessible != null) {
                    return (AccessibleRole) systemIAccessible.get_accRole(NativeMethods.CHILDID_SELF);
                }
                else {
                    return AccessibleRole.None;
                }                
            }
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.State"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       the state of this accessible object.</para>
        /// </devdoc>
        public virtual AccessibleStates State {
            get {
                if (systemIAccessible != null) {
                    return (AccessibleStates) systemIAccessible.get_accState(NativeMethods.CHILDID_SELF);
                }
                else {
                    return AccessibleStates.None;
                }
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Value"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the value of an accessible object.</para>
        /// </devdoc>
        public virtual string Value {
            // Does nothing by default
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get {
                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.get_accValue(NativeMethods.CHILDID_SELF);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                return "";
            }

            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            set {
                if (systemIAccessible != null) {
                    try {
                        systemIAccessible.set_accValue(NativeMethods.CHILDID_SELF, value);
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }                    
            }
        }

        // Methods

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.GetChild"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class, gets the accessible child corresponding to the specified 
        ///       index.</para>
        /// </devdoc>
        public virtual AccessibleObject GetChild(int index) {
            return null;
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.GetChildCount"]/*' />
        /// <devdoc>
        ///    <para> When overridden in a derived class, gets the number of children
        ///       belonging to an accessible object.</para>
        /// </devdoc>
        public virtual int GetChildCount() {
            return -1;
        }

        /// <internalonly/>
        /// <devdoc>
        ///     Mechanism for overriding default IEnumVariant behavior of the 'inner' system accessible object
        ///     (IEnumVariant is how a system accessible object exposes its ordered list of child objects).
        ///
        ///     USAGE: Overridden method in derived class should return array of integers representing new order
        ///     to be imposed on the child accessible object collection returned by the system (which
        ///     we assume will be a set of accessible objects that represent the child windows, in
        ///     z-order). Each array element contains the original z-order based rank of the child window
        ///     that is to appear at that position in the new ordering. Note: This array could also be
        ///     used to filter out unwanted child windows too, if necessary (not recommended).
        /// </devdoc>
        internal virtual int[] GetSysChildOrder() {
            return null;
        }

        /// <internalonly/>
        /// <devdoc>
        ///     Mechanism for overriding default IAccessible.accNavigate behavior of the 'inner' system accessible
        ///     object (accNavigate is how you move between parent, child and sibling accessible objects).
        ///
        ///     USAGE: 'navdir' indicates navigation operation to perform, relative to this accessible object.
        ///     If operation is unsupported, return false to allow fall-back to default system behavior. Otherwise
        ///     return destination object in the out parameter, or null to indicate 'off end of list'.
        /// </devdoc>
        internal virtual bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject accessibleObject) {
            accessibleObject = null;
            return false;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.GetFocused"]/*' />
        /// <devdoc>
        ///    <para> When overridden in a derived class,
        ///       gets the object that has the keyboard focus.</para>
        /// </devdoc>
        public virtual AccessibleObject GetFocused() {
        
            // Default behavior for objects with AccessibleObject children
            //
            if (GetChildCount() >= 0) {
                int count = GetChildCount();
                for(int index=0; index < count; ++index) {
                    AccessibleObject child = GetChild(index);
                    Debug.Assert(child != null, "GetChild(" + index.ToString(CultureInfo.InvariantCulture) + ") returned null!");
                    if (child != null && ((child.State & AccessibleStates.Focused) != 0)) {
                        return child;
                    }
                }
                if ((this.State & AccessibleStates.Focused) != 0) {
                    return this;
                }
                return null;
            }
        
            if (systemIAccessible != null) {
                try {
                    return WrapIAccessible(systemIAccessible.accFocus);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.GetHelpTopic"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Gets an identifier for a Help topic and the path to the Help file associated
        ///       with this accessible object.</para>
        /// </devdoc>
        public virtual int GetHelpTopic(out string fileName) {
            if (systemIAccessible != null) {
                try {
                    int retVal = systemIAccessible.get_accHelpTopic(out fileName, NativeMethods.CHILDID_SELF);
                    if (fileName != null && fileName.Length > 0) {
                        IntSecurity.DemandFileIO(FileIOPermissionAccess.PathDiscovery, fileName);
                    }
                    return retVal;
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }                
            } 
            fileName = null;
            return -1;
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.GetSelected"]/*' />
        /// <devdoc>
        ///    <para> When overridden in
        ///       a derived class, gets the currently selected child.</para>
        /// </devdoc>
        public virtual AccessibleObject GetSelected() {
            // Default behavior for objects with AccessibleObject children
            //
            if (GetChildCount() >= 0) {
                int count = GetChildCount();
                for(int index=0; index < count; ++index) {
                    AccessibleObject child = GetChild(index);
                    Debug.Assert(child != null, "GetChild(" + index.ToString(CultureInfo.InvariantCulture) + ") returned null!");
                    if (child != null && ((child.State & AccessibleStates.Selected) != 0)) {
                        return child;
                    }
                }
                if ((this.State & AccessibleStates.Selected) != 0) {
                    return this;
                }
                return null;
            }
        
            if (systemIAccessible != null) {
                try {
                    return WrapIAccessible(systemIAccessible.accSelection);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.HitTest"]/*' />
        /// <devdoc>
        ///    <para>Return the child object at the given screen coordinates.</para>
        /// </devdoc>
        public virtual AccessibleObject HitTest(int x, int y) {
        
            // Default behavior for objects with AccessibleObject children
            //
            if (GetChildCount() >= 0) {
                int count = GetChildCount();
                for(int index=0; index < count; ++index) {
                    AccessibleObject child = GetChild(index);
                    Debug.Assert(child != null, "GetChild(" + index.ToString(CultureInfo.InvariantCulture) + ") returned null!");
                    if (child != null && child.Bounds.Contains(x, y)) {
                        return child;
                    }
                }
                return this;
            }
            
            if (systemIAccessible != null) {
                try {
                    return WrapIAccessible(systemIAccessible.accHitTest(x, y));
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }
            
            if (this.Bounds.Contains(x, y)) {
                return this;
            }
            
            return null;
        }

        //
        // UIAutomation support helpers
        //

        internal virtual bool IsIAccessibleExSupported() {
            // Override this, in your derived class, to enable IAccessibleEx support
            return false;
        }

        internal virtual bool IsPatternSupported(int patternId) {
            // Override this, in your derived class, if you implement UIAutomation patterns 

            if (AccessibilityImprovements.Level3 && patternId == NativeMethods.UIA_InvokePatternId) {
                return IsInvokePatternAvailable;
            }

            return false;
        }

        //
        // Overridable methods for IRawElementProviderSimple interface
        //

        internal virtual int[] RuntimeId {
            get {
                return null;
            }
        }

        internal virtual int ProviderOptions {
            get {
                return (int)(UnsafeNativeMethods.ProviderOptions.ServerSideProvider | UnsafeNativeMethods.ProviderOptions.UseComThreading);
            }
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple HostRawElementProvider {
            get {
                return null;
            }
        }

        internal virtual object GetPropertyValue(int propertyID) {
            if (AccessibilityImprovements.Level3 && propertyID == NativeMethods.UIA_IsInvokePatternAvailablePropertyId) {
                return IsInvokePatternAvailable;
            }

            return null;
        }

        private bool IsInvokePatternAvailable {
            get {
                // MSAA Proxy determines the availability of invoke pattern based on Role/DefaultAction properties.
                // Below code emulates the same rules.
                switch (Role) {
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

        //
        // Overridable methods for ILegacyAccessible interface
        //

        internal virtual int GetChildId() {
            return NativeMethods.CHILDID_SELF;
        }

        //
        // Overridable methods for IRawElementProviderFragment interface
        //

        internal virtual UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetEmbeddedFragmentRoots() {
            return null;
        }

        internal virtual void SetFocus() {
        }

        internal virtual Rectangle BoundingRectangle {
            get {
                return this.Bounds;
            }
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot {
            get {
                return null;
            }
        }

        //
        // Overridable methods for IRawElementProviderFragmentRoot interface
        //

        internal virtual UnsafeNativeMethods.IRawElementProviderFragment ElementProviderFromPoint(double x, double y) {
            return this;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderFragment GetFocus() {
            return null;
        }

        //
        // Overridable methods for IExpandCollapseProvider pattern interface
        //

        internal virtual void Expand() {
        }

        internal virtual void Collapse() {
        }

        internal virtual UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState {
            get {
                return UnsafeNativeMethods.ExpandCollapseState.Collapsed;
            }
        }

        //
        // Overridable methods for IToggleProvider pattern interface
        //

        internal virtual void Toggle() {
        }

        internal virtual UnsafeNativeMethods.ToggleState ToggleState {
            get {
                return UnsafeNativeMethods.ToggleState.ToggleState_Indeterminate;
            }
        }

        //
        // Overridable methods for ITableProvider pattern interface
        //

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaders() {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaders() {
            return null;
        }

        internal virtual UnsafeNativeMethods.RowOrColumnMajor RowOrColumnMajor {
            get {
                return UnsafeNativeMethods.RowOrColumnMajor.RowOrColumnMajor_RowMajor;
            }
        }

        //
        // Overridable methods for ITableItemProvider pattern interface
        //

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetRowHeaderItems() {
            return null;
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple[] GetColumnHeaderItems() {
            return null;
        }

        //
        // Overridable methods for IGridProvider pattern interface
        //

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple GetItem(int row, int column) {
            return null;
        }

        internal virtual int RowCount {
            get {
                return -1;
            }
        }

        internal virtual int ColumnCount {
            get {
                return -1;
            }
        }

        //
        // Overridable methods for IGridItemProvider pattern interface
        //
        
        internal virtual int Row {
            get {
                return -1;
            }
        }

        internal virtual int Column {
            get {
                return -1;
            }
        }

        internal virtual int RowSpan {
            get {
                return 1;
            }
        }

        internal virtual int ColumnSpan {
            get {
                return 1;
            }
        }

        internal virtual UnsafeNativeMethods.IRawElementProviderSimple ContainingGrid {
            get {
                return null;
            }
        }

        //
        // Overridable methods for IInvokeProvider pattern interface
        //

        internal virtual void Invoke() {
            // Calling DoDefaultAction here is consistent with MSAA Proxy implementation.
            DoDefaultAction();
        }

        // Overridable methods for IValueProvider pattern interface

        internal virtual bool IsReadOnly {
            get {
                return false;
            }
        }

        internal virtual void SetValue(string newValue) {
            this.Value = newValue;
        }

        //
        // Overridable methods for IRangeValueProvider pattern interface
        //

        internal virtual void SetValue(double newValue) {
        }

        internal virtual double LargeChange {
            get {
                return double.NaN;
            }
        }

        internal virtual double Maximum {
            get {
                return double.NaN;
            }
        }

        internal virtual double Minimum {
            get {
                return double.NaN;
            }
        }

        internal virtual double SmallChange {
            get {
                return double.NaN;
            }
        }

        internal virtual double RangeValue {
            get {
                return double.NaN;
            }
        }

        int UnsafeNativeMethods.IServiceProvider.QueryService(ref Guid service, ref Guid riid, out IntPtr ppvObject) {

            int hr = NativeMethods.E_NOINTERFACE;
            ppvObject = IntPtr.Zero;

            if (IsIAccessibleExSupported()) {
                if (service.Equals(UnsafeNativeMethods.guid_IAccessibleEx) &&
                    riid.Equals(UnsafeNativeMethods.guid_IAccessibleEx)) {
                    // We want to return the internal, secure, object, which we don't have access here
                    // Return non-null, which will be interpreted in internal method, to mean returning casted object to IAccessibleEx

                    ppvObject = Marshal.GetComInterfaceForObject(this, typeof(UnsafeNativeMethods.IAccessibleEx));
                    hr = NativeMethods.S_OK;
                }
            }

            return hr;
        }

        object UnsafeNativeMethods.IAccessibleEx.GetObjectForChild(int idChild) {
            // No need to implement this for patterns and properties
            return null;
        }

        // This method is never called
        int UnsafeNativeMethods.IAccessibleEx.GetIAccessiblePair(out object ppAcc, out int pidChild) {

            // No need to implement this for patterns and properties
            ppAcc = null;
            pidChild = 0;
            return NativeMethods.E_POINTER;
        }

        int[] UnsafeNativeMethods.IAccessibleEx.GetRuntimeId() {
            return RuntimeId;
        }

        int UnsafeNativeMethods.IAccessibleEx.ConvertReturnedElement(object pIn, out object ppRetValOut) {

            // No need to implement this for patterns and properties
            ppRetValOut = null;
            return NativeMethods.E_NOTIMPL;
        }

        //
        // IRawElementProviderSimple interface
        //

        UnsafeNativeMethods.ProviderOptions UnsafeNativeMethods.IRawElementProviderSimple.ProviderOptions {
            get {
                return (UnsafeNativeMethods.ProviderOptions) ProviderOptions;
            }
        }

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderSimple.HostRawElementProvider {
            get {
                return HostRawElementProvider;
            }
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPatternProvider(int patternId) {
            if (IsPatternSupported(patternId)) {
                return this;
            }
            else {
                return null;
            }
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPropertyValue(int propertyID) {
            return GetPropertyValue(propertyID);
        }

        //
        // IRawElementProviderFragment interface
        //

        object UnsafeNativeMethods.IRawElementProviderFragment.Navigate(UnsafeNativeMethods.NavigateDirection direction) {
            return FragmentNavigate(direction);
        }

        int[] UnsafeNativeMethods.IRawElementProviderFragment.GetRuntimeId() {
            return RuntimeId;
        }

        object[] UnsafeNativeMethods.IRawElementProviderFragment.GetEmbeddedFragmentRoots() {
            return GetEmbeddedFragmentRoots();
        }

        void UnsafeNativeMethods.IRawElementProviderFragment.SetFocus() {
            SetFocus();
        }

        NativeMethods.UiaRect UnsafeNativeMethods.IRawElementProviderFragment.BoundingRectangle {
            get {
                return new NativeMethods.UiaRect(BoundingRectangle);
            }
        }

        UnsafeNativeMethods.IRawElementProviderFragmentRoot UnsafeNativeMethods.IRawElementProviderFragment.FragmentRoot {
            get {
                return FragmentRoot;
            }
        }

        //
        // IRawElementProviderFragmentRoot interface
        //

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y) {
            return ElementProviderFromPoint(x, y);
        }

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.GetFocus() {
            return GetFocus();
        }

        //
        // ILegacyIAccessibleProvider interface
        //

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.DefaultAction {
            get {
                return this.DefaultAction;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Description {
            get {
                return this.Description;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Help {
            get {
                return this.Help;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.KeyboardShortcut {
            get {
                return this.KeyboardShortcut;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Name {
            get {
                return this.Name;
            }
        }

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.Role {
            get {
                return (uint)this.Role;
            }
        }

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.State {
            get {
                return (uint)this.State;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Value {
            get {
                return this.Value;
            }
        }

        int UnsafeNativeMethods.ILegacyIAccessibleProvider.ChildId {
            get {
                return GetChildId();
            }
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.DoDefaultAction() {
            this.DoDefaultAction();
        }

        IAccessible UnsafeNativeMethods.ILegacyIAccessibleProvider.GetIAccessible() {
            return this.AsIAccessible(this);
        }

        object[] UnsafeNativeMethods.ILegacyIAccessibleProvider.GetSelection() {
            return new UnsafeNativeMethods.IRawElementProviderSimple[] {
                this.GetSelected() as UnsafeNativeMethods.IRawElementProviderSimple
            };
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.Select(int flagsSelect) {
            this.Select((AccessibleSelection)flagsSelect);
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.SetValue(string szValue) {
            this.SetValue(szValue);
        }

        //
        // IExpandCollapseProvider interface
        //

        void UnsafeNativeMethods.IExpandCollapseProvider.Expand() {
            Expand();
        }
        void UnsafeNativeMethods.IExpandCollapseProvider.Collapse() {
            Collapse();
        }

        UnsafeNativeMethods.ExpandCollapseState UnsafeNativeMethods.IExpandCollapseProvider.ExpandCollapseState {
            get {
                return ExpandCollapseState;
            }
        }

        //
        // IInvokeProvider interface
        //

        void UnsafeNativeMethods.IInvokeProvider.Invoke() {
            Invoke();
        }

        //
        // IValueProvider interface
        //

        bool UnsafeNativeMethods.IValueProvider.IsReadOnly {
            get {
                return IsReadOnly;
            }
        }

        string UnsafeNativeMethods.IValueProvider.Value {
            get {
                return Value;
            }
        }

        void UnsafeNativeMethods.IValueProvider.SetValue(string newValue) {
            SetValue(newValue);
        }

        //
        // IToggleProvider implementation
        //

        void UnsafeNativeMethods.IToggleProvider.Toggle() {
            Toggle();
        }

        UnsafeNativeMethods.ToggleState UnsafeNativeMethods.IToggleProvider.ToggleState {
            get {
                return ToggleState;
            }
        }

        //
        // ITableProvider implementation
        //

        object[] UnsafeNativeMethods.ITableProvider.GetRowHeaders() {
            return GetRowHeaders();
        }

        object[] UnsafeNativeMethods.ITableProvider.GetColumnHeaders() {
            return GetColumnHeaders();
        }

        UnsafeNativeMethods.RowOrColumnMajor UnsafeNativeMethods.ITableProvider.RowOrColumnMajor {
            get {
                return RowOrColumnMajor;
            }
        }

        //
        // ITableItemProvider implementation
        //

        object[] UnsafeNativeMethods.ITableItemProvider.GetRowHeaderItems() {
            return GetRowHeaderItems();
        }

        object[] UnsafeNativeMethods.ITableItemProvider.GetColumnHeaderItems() {
            return GetColumnHeaderItems();
        }

        //
        // IGridProvider implementation
        //

        object UnsafeNativeMethods.IGridProvider.GetItem(int row, int column) {
            return GetItem(row, column);
        }

        int UnsafeNativeMethods.IGridProvider.RowCount {
            get {
                return RowCount;
            }
        }

        int UnsafeNativeMethods.IGridProvider.ColumnCount {
            get {
                return ColumnCount;
            }
        }

        //
        // IGridItemProvider implementation
        //
        
        int UnsafeNativeMethods.IGridItemProvider.Row {
            get {
                return Row;
            }
        }

        int UnsafeNativeMethods.IGridItemProvider.Column {
            get {
                return Column;
            }
        }

        int UnsafeNativeMethods.IGridItemProvider.RowSpan {
            get {
                return RowSpan;
            }
        }

        int UnsafeNativeMethods.IGridItemProvider.ColumnSpan {
            get {
                return ColumnSpan;
            }
        }

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IGridItemProvider.ContainingGrid {
            get {
                return ContainingGrid;
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accDoDefaultAction"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Perform the default action
        /// </para>
        /// </devdoc>
        void IAccessible.accDoDefaultAction(Object childID) {

            IntSecurity.UnmanagedCode.Demand();

            if (IsClientObject) {
                ValidateChildID(ref childID);
                
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccDoDefaultAction: this = " + 
                    this.ToString() + ", childID = " + childID.ToString());

                // If the default action is to be performed on self, do it.
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    DoDefaultAction();
                    return;
                }
                    
                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    child.DoDefaultAction();
                    return;
                }            
            }

            if (systemIAccessible != null) {
                try {
                    systemIAccessible.accDoDefaultAction(childID);
                }
                catch (COMException e) {
                    // Not all objects provide a default action
                    //
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accHitTest"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Perform a hit test
        /// </para>
        /// </devdoc>
        Object IAccessible.accHitTest(
                                 int xLeft,
                                 int yTop) {

            if (IsClientObject) {
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccHitTest: this = " + 
                    this.ToString());
                
                AccessibleObject obj = HitTest(xLeft, yTop);
                if (obj != null) {
                    return AsVariant(obj);
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.accHitTest(xLeft, yTop);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }
            
            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accLocation"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// The location of the Accessible object
        /// </para>
        /// </devdoc>
        void IAccessible.accLocation(
                               out int pxLeft,
                               out int pyTop,
                               out int pcxWidth,
                               out int pcyHeight,
                               Object childID) {
            
            pxLeft = 0;
            pyTop = 0;
            pcxWidth = 0;
            pcyHeight = 0;

            if (IsClientObject) {
                ValidateChildID(ref childID);
                
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: this = " + 
                    this.ToString() + ", childID = " + childID.ToString());

                // Use the Location function's return value if available            
                //
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    Rectangle bounds = this.Bounds;
                    pxLeft = bounds.X;
                    pyTop = bounds.Y;
                    pcxWidth = bounds.Width;
                    pcyHeight = bounds.Height;
                    
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: Returning " + 
                        bounds.ToString());
                    
                    return;
                }
                
                // If we have an accessible object collection, get the appropriate child
                //
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
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

            if (systemIAccessible != null) {
                try {
                    systemIAccessible.accLocation(out pxLeft, out pyTop, out pcxWidth, out pcyHeight, childID);

                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccLocation: Setting " + 
                        pxLeft.ToString(CultureInfo.InvariantCulture) + ", " +
                        pyTop.ToString(CultureInfo.InvariantCulture) + ", " +
                        pcxWidth.ToString(CultureInfo.InvariantCulture) + ", " +
                        pcyHeight.ToString(CultureInfo.InvariantCulture));
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
                
                return;
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accNavigate"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Navigate to another accessible object.
        /// </para>
        /// </devdoc>
        Object IAccessible.accNavigate(
                                  int navDir,
                                  Object childID) {

            IntSecurity.UnmanagedCode.Demand();

            if (IsClientObject) {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccNavigate: this = " +
                    this.ToString() + ", navdir = " + navDir.ToString(CultureInfo.InvariantCulture) + ", childID = " + childID.ToString());
                               
                // Use the Navigate function's return value if available            
                //
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    AccessibleObject newObject = Navigate((AccessibleNavigation)navDir);
                    if (newObject != null) {
                        return AsVariant(newObject);
                    }
                }
                
                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return AsVariant(child.Navigate((AccessibleNavigation)navDir));
                }
            }

            if (systemIAccessible != null) {
                try {
                    Object retObject;
                    if (!SysNavigate(navDir, childID, out retObject))
                        retObject = systemIAccessible.accNavigate(navDir, childID);
                    return retObject;
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accSelect"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Select an accessible object.
        /// </para>
        /// </devdoc>
        void IAccessible.accSelect(int flagsSelect, Object childID) {

            IntSecurity.UnmanagedCode.Demand();

            if (IsClientObject) {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.AccSelect: this = " +
                    this.ToString() + ", flagsSelect = " + flagsSelect.ToString(CultureInfo.InvariantCulture) + ", childID = " + childID.ToString());
                
                // If the selection is self, do it.
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    Select((AccessibleSelection)flagsSelect);    // Uses an Enum which matches SELFLAG
                    return;
                }
                
                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    child.Select((AccessibleSelection)flagsSelect);
                    return;
                }
            }

            if (systemIAccessible != null) {
                try {
                    systemIAccessible.accSelect(flagsSelect, childID);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
                return;
            }

        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.DoDefaultAction"]/*' />
        /// <devdoc>
        ///      Performs the default action associated with this accessible object.
        /// </devdoc>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public virtual void DoDefaultAction() {
            // By default, just does the system default action if available
            //
            if (systemIAccessible != null) {
                try {
                    systemIAccessible.accDoDefaultAction(0);
                }
                catch (COMException e) {
                    // Not all objects provide a default action
                    //
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }                    
                }
                return;
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accChild"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Returns a child Accessible object
        /// </para>
        /// </devdoc>
        object IAccessible.get_accChild(object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccChild: this = " + 
                    this.ToString() + ", childID = " + childID.ToString());

                // Return self for CHILDID_SELF
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return AsIAccessible(this);
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    // Make sure we're not returning ourselves as our own child
                    //
                    Debug.Assert(child != this, "An accessible object is returning itself as its own child. This can cause Accessibility client applications to stop responding.");
                    if (child == this) {
                        return null;
                    }
                    
                    return AsIAccessible(child);
                }
            }

            // Otherwise, return the default system child for this control (if any)
            if (systemIAccessible != null) {
                return systemIAccessible.get_accChild(childID);
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accChildCount"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the number of children
        /// </devdoc>
        int IAccessible.accChildCount {
            get {
                int childCount = -1;

                if (IsClientObject) {
                    childCount = GetChildCount();
                }

                if (childCount == -1) {
                    if (systemIAccessible != null) {
                        childCount = systemIAccessible.accChildCount;
                    }
                    else {
                        childCount = 0;
                    }
                }
                
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.accHildCount: this = " + this.ToString() + ", returning " + childCount.ToString(CultureInfo.InvariantCulture));    
                
                return childCount;                
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accDefaultAction"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the default action
        /// </para>
        /// </devdoc>
        string IAccessible.get_accDefaultAction(Object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                // Return the default action property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return DefaultAction;
                }
                
                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.DefaultAction;
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.get_accDefaultAction(childID);
                }
                catch (COMException e) {
                    // Not all objects provide a default action
                    //
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accDescription"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the object or child description
        /// </para>
        /// </devdoc>
        string IAccessible.get_accDescription(Object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                // Return the description property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return Description;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.Description;
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.get_accDescription(childID);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.GetAccessibleChild"]/*' />
        /// <devdoc>
        ///      Returns the appropriate child from the Accessible Child Collection, if available
        /// </devdoc>
        private AccessibleObject GetAccessibleChild(object childID) {
            if (!childID.Equals(NativeMethods.CHILDID_SELF)) {
                int index = (int)childID - 1;   // The first child is childID == 1 (index == 0)
                if (index >= 0 && index < GetChildCount()) {
                    return GetChild(index);
                }
            }
            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accFocus"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the object or child focus
        /// </devdoc>
        object IAccessible.accFocus {
            get {

                if (IsClientObject) {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccFocus: this = " + 
                        this.ToString());
        
                    AccessibleObject obj = GetFocused();                
                    if (obj != null) {
                        return AsVariant(obj);
                    }
                }

                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.accFocus;
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                
                return null;
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accHelp"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return help for this accessible object.
        /// </para>
        /// </devdoc>
        string IAccessible.get_accHelp(Object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return Help;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.Help;
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.get_accHelp(childID);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accHelpTopic"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the object or child help topic
        /// </para>
        /// </devdoc>
        int IAccessible.get_accHelpTopic(out string pszHelpFile, Object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return GetHelpTopic(out pszHelpFile);
                }
                
                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.GetHelpTopic(out pszHelpFile);
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.get_accHelpTopic(out pszHelpFile, childID);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }
            
            pszHelpFile = null;
            return -1;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accKeyboardShortcut"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the object or child keyboard shortcut
        /// </para>
        /// </devdoc>
        string IAccessible.get_accKeyboardShortcut(Object childID) {
            return get_accKeyboardShortcutInternal(childID);
        }

        internal virtual string get_accKeyboardShortcutInternal(Object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return KeyboardShortcut;
                } 
                 
                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.KeyboardShortcut;
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.get_accKeyboardShortcut(childID);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accName"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the object or child name
        /// </para>
        /// </devdoc>
        string IAccessible.get_accName(object childID) {
            return get_accNameInternal(childID);
        }

        /// <devdoc>
        /// </devdoc>
        internal virtual string get_accNameInternal(object childID) {
            if (IsClientObject) {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.get_accName: this = " + this.ToString() + 
                    ", childID = " + childID.ToString());                                                                     

                // Return the name property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return Name;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.Name;
                }
            }

            // Otherwise, use the system provided name
            if (systemIAccessible != null) {
                string retval = systemIAccessible.get_accName(childID);

                if (IsClientObject) {
                    if (retval == null || retval.Length == 0) {
                        retval = Name;  // Name the child after its parent
                    }
                }

                return retval;
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accParent"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the parent object
        /// </devdoc>
        object IAccessible.accParent {
            get {
                IntSecurity.UnmanagedCode.Demand();

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.accParent: this = " + this.ToString());
                AccessibleObject parent = Parent;
                if (parent != null) {
                    // Some debugging related tests
                    //
                    Debug.Assert(parent != this, "An accessible object is returning itself as its own parent. This can cause accessibility clients to stop responding.");
                    if (parent == this) {
                        parent = null;  // This should prevent accessibility clients from stop responding
                    }                    
                }
                
                return AsIAccessible(parent);
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accRole"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// The role property describes an object's purpose in terms of its
        /// relationship with sibling or child objects.
        /// </para>
        /// </devdoc>
        object IAccessible.get_accRole(object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                // Return the role property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return (int)Role;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return (int)child.Role;
                }
            }

            if (systemIAccessible != null) {
                return systemIAccessible.get_accRole(childID);
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.accSelection"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the object or child selection
        /// </devdoc>
        object IAccessible.accSelection { 
            get {

                if (IsClientObject) {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccSelection: this = " + 
                        this.ToString());
        
                    AccessibleObject obj = GetSelected();                
                    if (obj != null) {
                        return AsVariant(obj);
                    }
                }

                if (systemIAccessible != null) {
                    try {
                        return systemIAccessible.accSelection;
                    }
                    catch (COMException e) {
                        if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                            throw e;
                        }
                    }
                }
                
                return null;
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accState"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the object or child state
        /// </para>
        /// </devdoc>
        object IAccessible.get_accState(object childID) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.GetAccState: this = " + 
                    this.ToString() + ", childID = " + childID.ToString());

                // Return the state property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return (int)State;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return (int)child.State;
                }
            }

            if (systemIAccessible != null) {
                return systemIAccessible.get_accState(childID);
            }

            return null;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.get_accValue"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Return the object or child value
        /// </para>
        /// </devdoc>
        string IAccessible.get_accValue(object childID) {

            IntSecurity.UnmanagedCode.Demand();

            if (IsClientObject) {
                ValidateChildID(ref childID);

                // Return the value property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    return Value;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    return child.Value;
                }
            }

            if (systemIAccessible != null) {
                try {
                    return systemIAccessible.get_accValue(childID);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }

            return null;
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.set_accName"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Set the object or child name
        /// </para>
        /// </devdoc>
        void IAccessible.set_accName(
                              Object childID,
                              string newName) {

            if (IsClientObject) {
                ValidateChildID(ref childID);

                // Set the name property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    // Attempt to set the name property
                    Name = newName;
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    child.Name = newName;
                    return;
                }
            }

            if (systemIAccessible != null) {
                systemIAccessible.set_accName(childID, newName);
                return;
            }
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IAccessible.set_accValue"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>
        /// Set the object or child value
        /// </para>
        /// </devdoc>
        void IAccessible.set_accValue(
                               Object childID,
                               string newValue) {

            IntSecurity.UnmanagedCode.Demand();

            if (IsClientObject) {
                ValidateChildID(ref childID);

                // Set the value property if available
                if (childID.Equals(NativeMethods.CHILDID_SELF)) {
                    // Attempt to set the value property
                    Value = newValue;
                    return;
                }

                // If we have an accessible object collection, get the appropriate child
                AccessibleObject child = GetAccessibleChild(childID);
                if (child != null) {
                    child.Value = newValue;
                    return;
                }
            }

            if (systemIAccessible != null) {
                try {
                    systemIAccessible.set_accValue(childID, newValue);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
                return;
            }
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UnsafeNativeMethods.IOleWindow.GetWindow"]/*' />
        /// <devdoc>
        ///     Now that AccessibleObject is used to wrap all system-provided (OLEACC.DLL) accessible
        ///     objects, it needs to implement IOleWindow and pass this down to the inner object. This is
        ///     necessary because the OS function WindowFromAccessibleObject() walks up the parent chain
        ///     looking for the first object that implements IOleWindow, and uses that to get the hwnd.
        ///
        ///     But this creates a new problem for AccessibleObjects that do NOT have windows, ie. which
        ///     represent simple elements. To the OS, these simple elements will now appear to implement
        ///     IOleWindow, so it will try to get hwnds from them - which they simply cannot provide.
        ///
        ///     To work around this problem, the AccessibleObject for a simple element will delegate all
        ///     IOleWindow calls up the parent chain itself. This will stop at the first window-based
        ///     accessible object, which will be able to return an hwnd back to the OS. So we are
        ///     effectively 'preempting' what WindowFromAccessibleObject() would do.
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd) {
            // See if we have an inner object that can provide the window handle
            if (systemIOleWindow != null) {
                return systemIOleWindow.GetWindow(out hwnd);
            }

            // Otherwise delegate to the parent object
            AccessibleObject parent = this.Parent;
            if (parent is UnsafeNativeMethods.IOleWindow) {
                return (parent as UnsafeNativeMethods.IOleWindow).GetWindow(out hwnd);
            }

            // Or fail if there is no parent
            hwnd = IntPtr.Zero;
            return NativeMethods.E_FAIL;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp"]/*' />
        /// <devdoc>
        ///     See GetWindow() above for details.
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode) {
            // See if we have an inner object that can provide help
            if (systemIOleWindow != null) {
                systemIOleWindow.ContextSensitiveHelp(fEnterMode);
                return;
            }

            // Otherwise delegate to the parent object
            AccessibleObject parent = this.Parent;
            if (parent is UnsafeNativeMethods.IOleWindow) {
                (parent as UnsafeNativeMethods.IOleWindow).ContextSensitiveHelp(fEnterMode);
                return;
            }

            // Or do nothing if there is no parent
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UnsafeNativeMethods.IEnumVariant.Clone"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Clone this accessible object.
        ///    </para>
        /// </devdoc>
        void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v) {
            EnumVariant.Clone(v);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UnsafeNativeMethods.IEnumVariant.Next"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Obtain the next n children of this accessible object.
        ///    </para>
        /// </devdoc>
        int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns) {
            return EnumVariant.Next(n, rgvar, ns);
        }
                  
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UnsafeNativeMethods.IEnumVariant.Reset"]/*' />
        /// <devdoc>
        ///      Resets the child accessible object enumerator.
        /// </devdoc>
        void UnsafeNativeMethods.IEnumVariant.Reset() {
            EnumVariant.Reset();
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UnsafeNativeMethods.IEnumVariant.Skip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Skip the next n child accessible objects
        ///    </para>
        /// </devdoc>
        void UnsafeNativeMethods.IEnumVariant.Skip(int n) {
            EnumVariant.Skip(n);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Navigate"]/*' />
        /// <devdoc>
        ///    <para>When overridden in a derived class,
        ///       navigates to another object.</para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public virtual AccessibleObject Navigate(AccessibleNavigation navdir) {
        
            // Some default behavior for objects with AccessibleObject children
            //
            if (GetChildCount() >= 0) {
                switch(navdir) {
                    case AccessibleNavigation.FirstChild:
                        return GetChild(0);
                    case AccessibleNavigation.LastChild:
                        return GetChild(GetChildCount() - 1);
                    case AccessibleNavigation.Previous:
                    case AccessibleNavigation.Up:
                    case AccessibleNavigation.Left:
                        if (Parent.GetChildCount() > 0) {
                            return null;
                        }
                        break;
                    case AccessibleNavigation.Next:
                    case AccessibleNavigation.Down:
                    case AccessibleNavigation.Right:
                        if (Parent.GetChildCount() > 0) {
                            return null;
                        }
                        break;
                }
            }
        
            if (systemIAccessible != null) {
                try {
                    object retObject = null;
                    if (!SysNavigate((int)navdir, NativeMethods.CHILDID_SELF, out retObject))
                        retObject = systemIAccessible.accNavigate((int)navdir, NativeMethods.CHILDID_SELF);
                    return WrapIAccessible(retObject);
                }
                catch (COMException e) {
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
            }
            return null;
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.Select"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Selects this accessible object.
        ///    </para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        public virtual void Select(AccessibleSelection flags) {
        
            // By default, do the system behavior
            //
            if (systemIAccessible != null) {
                try {
                    systemIAccessible.accSelect((int)flags, 0);
                }
                catch (COMException e) {
                    // Not all objects provide the select function
                    //
                    if (e.ErrorCode != NativeMethods.DISP_E_MEMBERNOTFOUND) {
                        throw e;
                    }
                }
                return;
            }
        }
        
        private object AsVariant(AccessibleObject obj) {
            if (obj == this) {
                return NativeMethods.CHILDID_SELF;
            }
            return AsIAccessible(obj);
        }
        
        private IAccessible AsIAccessible(AccessibleObject obj) {
            if (obj != null && obj.systemWrapper) {
                return obj.systemIAccessible;
            }
            return obj;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.AccessibleObjectId"]/*' />
        /// <devdoc>
        ///     Indicates what kind of 'inner' system accessible object we are using as our fall-back
        ///     implementation of IAccessible (when the systemIAccessible member is not null). The inner
        ///     object is provided by OLEACC.DLL. Note that although the term 'id' is used, this value
        ///     really represents a category or type of accessible object. Ids are only unique among
        ///     accessible objects associated with the same window handle. Currently supported ids are...
        ///
        ///     OBJID_CLIENT - represents the window's client area (including any child windows)
        ///     OBJID_WINDOW - represents the window's non-client area (including caption, frame controls and scrollbars)
        ///
        ///     NOTE: When the id is OBJID_WINDOW, we short-circuit most of the virtual override behavior of
        ///     AccessibleObject, and turn the object into a simple wrapper around the inner system object. So
        ///     for a *user-defined* accessible object, that has NO inner object, its important that the id is
        ///     left as OBJID_CLIENT, otherwise the object will be short-circuited into a total NOP!
        /// </devdoc>
        /// <internalonly/>
        internal int AccessibleObjectId {
            get {
                return accObjId;
            }
            set {
                accObjId = value;
            }
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IsClientObject"]/*' />
        /// <devdoc>
        ///    Indicates whether this accessible object represents the client area of the window.
        /// </devdoc>
        /// <internalonly/>
        internal bool IsClientObject {
            get {
                return AccessibleObjectId == NativeMethods.OBJID_CLIENT;
            }
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.IsNonClientObject"]/*' />
        /// <devdoc>
        ///    Indicates whether this accessible object represents the non-client area of the window.
        /// </devdoc>
        /// <internalonly/>
        internal bool IsNonClientObject {
            get {
                return AccessibleObjectId == NativeMethods.OBJID_WINDOW;
            }
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        internal IAccessible GetSystemIAccessibleInternal() {
            return this.systemIAccessible;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UseStdAccessibleObjects"]/*' />
        /// <internalonly/>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected void UseStdAccessibleObjects(IntPtr handle) {
            UseStdAccessibleObjects(handle, AccessibleObjectId);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.UseStdAccessibleObjects1"]/*' />
        /// <internalonly/>
        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected void UseStdAccessibleObjects(IntPtr handle, int objid) {
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

            Debug.Assert(acc != null, "SystemIAccessible is null");
            Debug.Assert(en != null, "SystemIEnumVariant is null");

            if (acc != null || en != null) {
                systemIAccessible = (IAccessible)acc;
                systemIEnumVariant = (UnsafeNativeMethods.IEnumVariant)en;
                systemIOleWindow = acc as UnsafeNativeMethods.IOleWindow;
            }
        }

        /// <devdoc>
        ///     Performs custom navigation between parent/child/sibling accessible objects. This is basically
        ///     just a wrapper for GetSysChild(), that does some of the dirty work, such as wrapping the
        ///     returned object in a VARIANT. Usage is similar to GetSysChild(). Called prior to calling
        ///     IAccessible.accNavigate on the 'inner' system accessible object.
        /// </devdoc>
        /// <internalonly/>
        private bool SysNavigate(int navDir, Object childID, out Object retObject) {
            retObject = null;

            // Only override system navigation relative to ourselves (since we can't interpret OLEACC child ids)
            if (!childID.Equals(NativeMethods.CHILDID_SELF))
                return false;

            // Perform any supported navigation operation (fall back on system for unsupported navigation ops)
            AccessibleObject newObject;
            if (!GetSysChild((AccessibleNavigation) navDir, out newObject))
                return false;

            // If object found, wrap in a VARIANT. Otherwise return null for 'end of list' (OLEACC expects this)
            retObject = (newObject == null) ? null : AsVariant(newObject);

            // Tell caller not to fall back on system behavior now
            return true;
        }

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.ValidateChildID"]/*' />
        /// <devdoc>
        ///      Make sure that the childID is valid.
        /// </devdoc>
        internal void ValidateChildID(ref object childID) {
            // An empty childID is considered to be the same as CHILDID_SELF.
            // Some accessibility programs pass null into our functions, so we
            // need to convert them here.
            if (childID == null) {
                childID = NativeMethods.CHILDID_SELF;
            }
            else if (childID.Equals(NativeMethods.DISP_E_PARAMNOTFOUND)) {
                childID = 0;
            }
            else if (!(childID is Int32)) {
                // AccExplorer seems to occasionally pass in objects instead of an int ChildID.
                //
                childID = 0;
            }
        }

        private AccessibleObject WrapIAccessible(object iacc) {
            IAccessible accessible = iacc as IAccessible;
            if (accessible == null) {
                return null;
            }
        
            // Check to see if this object already wraps iacc
            //
            if (this.systemIAccessible == iacc) {
                return this;
            }
        
            return new AccessibleObject(accessible);
        }                             

        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetMethod"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the requested method if it is implemented by the Reflection object.  The
        /// match is based upon the name and DescriptorInfo which describes the signature
        /// of the method. 
        /// </devdoc>
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers) {
            return typeof(IAccessible).GetMethod(name, bindingAttr, binder, types, modifiers);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetMethod1"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the requested method if it is implemented by the Reflection object.  The
        /// match is based upon the name of the method.  If the object implementes multiple methods
        /// with the same name an AmbiguousMatchException is thrown.
        /// </devdoc>
        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr) {
            return typeof(IAccessible).GetMethod(name, bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetMethods"]/*' />
        /// <internalonly/>
        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) {
            return typeof(IAccessible).GetMethods(bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetField"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the requestion field if it is implemented by the Reflection object.  The
        /// match is based upon a name.  There cannot be more than a single field with
        /// a name.
        /// </devdoc>
        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr) {
            return typeof(IAccessible).GetField(name, bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetFields"]/*' />
        /// <internalonly/>
        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) {
            return typeof(IAccessible).GetFields(bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetProperty"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the property based upon name.  If more than one property has the given
        /// name an AmbiguousMatchException will be thrown.  Returns null if no property
        /// is found.
        /// </devdoc>
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr) {
            return typeof(IAccessible).GetProperty(name, bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetProperty1"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the property based upon the name and Descriptor info describing the property
        /// indexing.  Return null if no property is found.
        /// </devdoc>
        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {
            return typeof(IAccessible).GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetProperties"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Returns an array of PropertyInfos for all the properties defined on 
        /// the Reflection object.
        /// </devdoc>
        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) {
            return typeof(IAccessible).GetProperties(bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetMember"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return an array of members which match the passed in name.
        /// </devdoc>
        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr) {
            return typeof(IAccessible).GetMember(name, bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.GetMembers"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return an array of all of the members defined for this object.
        /// </devdoc>
        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) {
            return typeof(IAccessible).GetMembers(bindingAttr);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.InvokeMember"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Description of the Binding Process.
        /// We must invoke a method that is accessable and for which the provided
        /// parameters have the most specific match.  A method may be called if
        /// 1. The number of parameters in the method declaration equals the number of 
        /// arguments provided to the invocation
        /// 2. The type of each argument can be converted by the binder to the
        /// type of the type of the parameter.
        ///
        /// The binder will find all of the matching methods.  These method are found based
        /// upon the type of binding requested (MethodInvoke, Get/Set Properties).  The set
        /// of methods is filtered by the name, number of arguments and a set of search modifiers
        /// defined in the Binder.
        ///
        /// After the method is selected, it will be invoked.  Accessability is checked
        /// at that point.  The search may be control which set of methods are searched based
        /// upon the accessibility attribute associated with the method.
        ///
        /// The BindToMethod method is responsible for selecting the method to be invoked.
        /// For the default binder, the most specific method will be selected.
        ///
        /// This will invoke a specific member...
        /// @exception If <var>invokeAttr</var> is CreateInstance then all other
        /// Access types must be undefined.  If not we throw an ArgumentException.
        /// @exception If the <var>invokeAttr</var> is not CreateInstance then an
        /// ArgumentException when <var>name</var> is null.
        /// @exception ArgumentException when <var>invokeAttr</var> does not specify the type
        /// @exception ArgumentException when <var>invokeAttr</var> specifies both get and set of
        /// a property or field.
        /// @exception ArgumentException when <var>invokeAttr</var> specifies property set and
        /// invoke method.
        /// </devdoc>
        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) {
            
            if (args.Length == 0) {
                MemberInfo[] member = typeof(IAccessible).GetMember(name);
                if (member != null && member.Length > 0 && member[0] is PropertyInfo) {
                    MethodInfo getMethod = ((PropertyInfo)member[0]).GetGetMethod();
                    if (getMethod != null && getMethod.GetParameters().Length > 0) {
                        args = new object[getMethod.GetParameters().Length];
                        for (int i = 0; i < args.Length; i++) {
                            args[i] = NativeMethods.CHILDID_SELF;    
                        }
                    }
                }
            }
            return typeof(IAccessible).InvokeMember(name, invokeAttr, binder, target, args, modifiers, culture, namedParameters);
        }
        
        /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="IReflect.UnderlyingSystemType"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// Return the underlying Type that represents the IReflect Object.  For expando object,
        /// this is the (Object) IReflectInstance.GetType().  For Type object it is this.
        /// </devdoc>
        Type IReflect.UnderlyingSystemType {
            get {
                return typeof(IAccessible);
            }
        }

        bool UnsafeNativeMethods.IRangeValueProvider.IsReadOnly {
            get {
                return IsReadOnly;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.LargeChange {
            get {
                return LargeChange;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.Maximum {
            get {
                return Maximum;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.Minimum {
            get {
                return Minimum;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.SmallChange {
            get {
                return SmallChange;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.Value {
            get {
                return RangeValue;
            }
        }

        void UnsafeNativeMethods.IRangeValueProvider.SetValue(double value) {
            SetValue(value);
        }

        /// <summary>
        /// Raises the UIA Notification event.
        /// The event is available starting with Windows 10, version 1709.
        /// </summary>
        /// <param name="notificationKind">The type of notification</param>
        /// <param name="notificationProcessing">Indicates how to process notifications</param>
        /// <param name="notificationText">Notification text</param>
        /// <returns>
        /// True if operation succeeds.
        /// False if the underlying windows infrastructure is not available or the operation had failed.
        /// Use Marshal.GetLastWin32Error for details.
        /// </returns>
        public bool RaiseAutomationNotification(
            AutomationNotificationKind notificationKind,
            AutomationNotificationProcessing notificationProcessing,
            string notificationText) {

            if (!AccessibilityImprovements.Level3 || !notificationEventAvailable) {
                return false;
            }

            int result = NativeMethods.S_FALSE;
            try {
                // The activityId can be any string. It cannot be null. It is not used currently.
                result = UnsafeNativeMethods.UiaRaiseNotificationEvent(
                    this,
                    notificationKind,
                    notificationProcessing,
                    notificationText,
                    String.Empty);
            }
            catch (EntryPointNotFoundException) {
                // The UIA Notification event is not available, so don't attempt to raise it again.
                notificationEventAvailable = false;
            }

            return result == NativeMethods.S_OK;
        }

        /// <summary>
        /// Raises the LiveRegionChanged UIA event.
        /// This method must be overridden in derived classes that support the UIA live region feature.
        /// </summary>
        /// <returns>True if operation succeeds, False otherwise.</returns>
        public virtual bool RaiseLiveRegionChanged() {
            throw new NotSupportedException(SR.AccessibleObjectLiveRegionNotSupported);
        }

        internal bool RaiseAutomationEvent(int eventId) {
            if (UnsafeNativeMethods.UiaClientsAreListening()) {
                int result = UnsafeNativeMethods.UiaRaiseAutomationEvent(this, eventId);
                return result == NativeMethods.S_OK;
            }

            return false;
        }

        private class EnumVariantObject : UnsafeNativeMethods.IEnumVariant {
        
            private int currentChild = 0;
            private AccessibleObject owner;
            
            public EnumVariantObject(AccessibleObject owner) {
                Debug.Assert(owner != null, "Cannot create EnumVariantObject with a null owner");
                this.owner = owner;
            }
            
            public EnumVariantObject(AccessibleObject owner, int currentChild) {
                Debug.Assert(owner != null, "Cannot create EnumVariantObject with a null owner");
                this.owner = owner;
                this.currentChild = currentChild;
            }

            void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v) {
                v[0] = new EnumVariantObject(owner, currentChild);
            }

            /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.EnumVariantObject.UnsafeNativeMethods.IEnumVariant.Reset"]/*' />
            /// <devdoc>
            ///     Resets the child accessible object enumerator.
            /// </devdoc>
            void UnsafeNativeMethods.IEnumVariant.Reset() {
                currentChild = 0;
    
                // 


                IntSecurity.UnmanagedCode.Assert();
                try {
                    if (owner.systemIEnumVariant != null)
                        owner.systemIEnumVariant.Reset();
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
            
            /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.EnumVariantObject.UnsafeNativeMethods.IEnumVariant.Skip"]/*' />
            /// <devdoc>
            ///     Skips the next n child accessible objects.
            /// </devdoc>
            void UnsafeNativeMethods.IEnumVariant.Skip(int n) {
                currentChild += n;

                // 


                IntSecurity.UnmanagedCode.Assert();
                try {
                    if (owner.systemIEnumVariant != null)
                        owner.systemIEnumVariant.Skip(n);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }    

            /// <include file='doc\AccessibleObject.uex' path='docs/doc[@for="AccessibleObject.EnumVariantObject.UnsafeNativeMethods.IEnumVariant.Next"]/*' />
            /// <devdoc>
            ///     Gets the next n child accessible objects.
            /// </devdoc>
            int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns) {
                // NOTE: rgvar is a pointer to an array of variants

                if (owner.IsClientObject) {
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "EnumVariantObject: owner = " + owner.ToString() + ", n = " + n);

                    Debug.Indent();
                    
                    int childCount;
                    int[] newOrder;
                   
                    if ((childCount = owner.GetChildCount()) >= 0)
                        NextFromChildCollection(n, rgvar, ns, childCount);
                    else if (owner.systemIEnumVariant == null)
                        NextEmpty(n, rgvar, ns);
                    else if ((newOrder = owner.GetSysChildOrder()) != null)
                        NextFromSystemReordered(n, rgvar, ns, newOrder);
                    else
                        NextFromSystem(n, rgvar, ns);

                    Debug.Unindent();
                }
                else {
                    NextFromSystem(n, rgvar, ns);
                }

                // Tell caller whether requested number of items was returned. Once list of items has
                // been exhausted, we return S_FALSE so that caller knows to stop calling this method.
                return (ns[0] == n) ? NativeMethods.S_OK : NativeMethods.S_FALSE;
            }

            /// <devdoc>
            ///     When we have the IEnumVariant of an accessible proxy provided by the system (ie.
            ///     OLEACC.DLL), we can fall back on that to return the children. Generally, the system
            ///     proxy will enumerate the child windows, create a suitable kind of child accessible
            ///     proxy for each one, and return a set of IDispatch interfaces to these proxy objects.
            /// </devdoc>
            private void NextFromSystem(int n, IntPtr rgvar, int[] ns) {

                // 


                IntSecurity.UnmanagedCode.Assert();
                try {
                    owner.systemIEnumVariant.Next(n, rgvar, ns);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                currentChild += ns[0];
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: Delegating to systemIEnumVariant");
            }

            /// <devdoc>
            ///     Sometimes we want to rely on the system-provided behavior to create and return child
            ///     accessible objects, but we want to impose a new order on those objects (or even filter
            ///     some objects out).
            ///
            ///     This method takes an array of ints that dictates the new order. It queries the system
            ///     for each child individually, and inserts the result into the correct *new* position.
            ///
            ///     Note: This code has to make certain *assumptions* about OLEACC.DLL proxy object behavior.
            ///     However, this behavior is well documented. We *assume* the proxy will return a set of
            ///     child accessible objects that correspond 1:1 with the owning control's child windows,
            ///     and that the default order it returns these objects in is z-order (which also happens
            ///     to be the order that children appear in the Control.Controls[] collection).
            /// </devdoc>
            private void NextFromSystemReordered(int n, IntPtr rgvar, int[] ns, int[] newOrder) {
                int i;

                for (i = 0; i < n && currentChild < newOrder.Length; ++i) {
                    if (!GotoItem(owner.systemIEnumVariant, newOrder[currentChild], GetAddressOfVariantAtIndex(rgvar, i)))
                        break;
                    ++currentChild;
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: adding sys child " + currentChild + " of " + newOrder.Length);
                }

                ns[0] = i;
            }

            /// <devdoc>
            ///     If we have our own custom accessible child collection, return a set of 1-based integer
            ///     child ids, that the caller will eventually pass back to us via IAccessible.get_accChild().
            /// </devdoc>
            private void NextFromChildCollection(int n, IntPtr rgvar, int[] ns, int childCount) {
                int i;

                for (i = 0; i < n && currentChild < childCount; ++i) {
                    ++currentChild;
                    Marshal.GetNativeVariantForObject(((object) currentChild), GetAddressOfVariantAtIndex(rgvar, i));
                    Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: adding own child " + currentChild + " of " + childCount);
                }

                ns[0] = i;
            }

            /// <devdoc>
            ///     Default behavior if there is no custom child collection or system-provided
            ///     proxy to fall back on. In this case, we return an empty child collection.
            /// </devdoc>
            private void NextEmpty(int n, IntPtr rgvar, int[] ns) {
                ns[0] = 0;
                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo, "AccessibleObject.IEV.Next: no children to add");
            }

            /// <devdoc>
            ///     Given an IEnumVariant interface, this method jumps to a specific
            ///     item in the collection and extracts the result for that one item.
            /// </devdoc>
            private static bool GotoItem(UnsafeNativeMethods.IEnumVariant iev, int index, IntPtr variantPtr) {
                int[] ns = new int[1];

                // 


                IntSecurity.UnmanagedCode.Assert();
                try {
                    iev.Reset();
                    iev.Skip(index);
                    iev.Next(1, variantPtr, ns);
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }

                return (ns[0] == 1);
            }

            /// <devdoc>
            ///     Given an array of pointers to variants, calculate address of a given array element.
            /// </devdoc>
            private static IntPtr GetAddressOfVariantAtIndex(IntPtr variantArrayPtr, int index) {
                int variantSize = 8 + (IntPtr.Size * 2);
                return (IntPtr) ((ulong) variantArrayPtr + ((ulong) index) * ((ulong) variantSize));
            }
            
        }

    } // end class AccessibleObject

    /// <Summary>
    ///    Internal object passed out to OLEACC clients via WM_GETOBJECT.
    /// </Summary>
    internal sealed class InternalAccessibleObject : StandardOleMarshalObject, 
                                    UnsafeNativeMethods.IAccessibleInternal,
                                    System.Reflection.IReflect,
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
                                    UnsafeNativeMethods.ILegacyIAccessibleProvider {

        private IAccessible publicIAccessible;                       // AccessibleObject as IAccessible
        private UnsafeNativeMethods.IEnumVariant publicIEnumVariant; // AccessibleObject as IEnumVariant
        private UnsafeNativeMethods.IOleWindow publicIOleWindow;     // AccessibleObject as IOleWindow
        private IReflect publicIReflect;                             // AccessibleObject as IReflect

        private UnsafeNativeMethods.IServiceProvider publicIServiceProvider;             // AccessibleObject as IServiceProvider
        private UnsafeNativeMethods.IAccessibleEx publicIAccessibleEx;                   // AccessibleObject as IAccessibleEx

        // UIAutomation
        private UnsafeNativeMethods.IRawElementProviderSimple publicIRawElementProviderSimple;    // AccessibleObject as IRawElementProviderSimple
        private UnsafeNativeMethods.IRawElementProviderFragment publicIRawElementProviderFragment;// AccessibleObject as IRawElementProviderFragment
        private UnsafeNativeMethods.IRawElementProviderFragmentRoot publicIRawElementProviderFragmentRoot;// AccessibleObject as IRawElementProviderFragmentRoot
        private UnsafeNativeMethods.IInvokeProvider publicIInvokeProvider;                        // AccessibleObject as IInvokeProvider
        private UnsafeNativeMethods.IValueProvider publicIValueProvider;                          // AccessibleObject as IValueProvider
        private UnsafeNativeMethods.IRangeValueProvider publicIRangeValueProvider;                // AccessibleObject as IRangeValueProvider
        private UnsafeNativeMethods.IExpandCollapseProvider publicIExpandCollapseProvider;        // AccessibleObject as IExpandCollapseProvider
        private UnsafeNativeMethods.IToggleProvider publicIToggleProvider;                        // AccessibleObject as IToggleProvider
        private UnsafeNativeMethods.ITableProvider publicITableProvider;                          // AccessibleObject as ITableProvider
        private UnsafeNativeMethods.ITableItemProvider publicITableItemProvider;                  // AccessibleObject as ITableItemProvider
        private UnsafeNativeMethods.IGridProvider publicIGridProvider;                            // AccessibleObject as IGridProvider
        private UnsafeNativeMethods.IGridItemProvider publicIGridItemProvider;                    // AccessibleObject as IGridItemProvider
        private UnsafeNativeMethods.ILegacyIAccessibleProvider publicILegacyIAccessibleProvider;   // AccessibleObject as ILegayAccessibleProvider

        /// <summary>
        ///     Create a new wrapper. Protect this with UnmanagedCode Permission
        /// </summary>
        [
        SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)
        ]
        internal InternalAccessibleObject(AccessibleObject accessibleImplemention) {
            // Get all the casts done here to catch any issues early 
            publicIAccessible = (IAccessible) accessibleImplemention;
            publicIEnumVariant = (UnsafeNativeMethods.IEnumVariant) accessibleImplemention;
            publicIOleWindow = (UnsafeNativeMethods.IOleWindow) accessibleImplemention;
            publicIReflect = (IReflect) accessibleImplemention;
            publicIServiceProvider = (UnsafeNativeMethods.IServiceProvider) accessibleImplemention;
            publicIAccessibleEx = (UnsafeNativeMethods.IAccessibleEx) accessibleImplemention;
            publicIRawElementProviderSimple = (UnsafeNativeMethods.IRawElementProviderSimple) accessibleImplemention;
            publicIRawElementProviderFragment = (UnsafeNativeMethods.IRawElementProviderFragment)accessibleImplemention;
            publicIRawElementProviderFragmentRoot = (UnsafeNativeMethods.IRawElementProviderFragmentRoot)accessibleImplemention;
            publicIInvokeProvider = (UnsafeNativeMethods.IInvokeProvider)accessibleImplemention;
            publicIValueProvider = (UnsafeNativeMethods.IValueProvider) accessibleImplemention;
            publicIRangeValueProvider = (UnsafeNativeMethods.IRangeValueProvider)accessibleImplemention;
            publicIExpandCollapseProvider = (UnsafeNativeMethods.IExpandCollapseProvider) accessibleImplemention;
            publicIToggleProvider = (UnsafeNativeMethods.IToggleProvider)accessibleImplemention;
            publicITableProvider = (UnsafeNativeMethods.ITableProvider)accessibleImplemention;
            publicITableItemProvider = (UnsafeNativeMethods.ITableItemProvider)accessibleImplemention;
            publicIGridProvider = (UnsafeNativeMethods.IGridProvider)accessibleImplemention;
            publicIGridItemProvider = (UnsafeNativeMethods.IGridItemProvider)accessibleImplemention;
            publicILegacyIAccessibleProvider = (UnsafeNativeMethods.ILegacyIAccessibleProvider)accessibleImplemention;
            // Note: Deliberately not holding onto AccessibleObject to enforce all access through the interfaces
        }

        /// <summary>
        ///     If the given object is an AccessibleObject return it as a InternalAccessibleObject
        ///     This ensures we wrap all AccessibleObjects before handing them out to OLEACC
        /// </summary>
        private object AsNativeAccessible(object accObject) {
            if (accObject is AccessibleObject) {
                return new InternalAccessibleObject(accObject as AccessibleObject);
            }
            else {
                return accObject;
            }
        }

        /// <summary>
        ///     Wraps AccessibleObject elements of a given array into InternalAccessibleObjects
        /// </summary>
        private object[] AsArrayOfNativeAccessibles(object[] accObjectArray) {
            if (accObjectArray != null && accObjectArray.Length > 0) {
                for (int i = 0; i < accObjectArray.Length; i++) {
                    accObjectArray[i] = AsNativeAccessible(accObjectArray[i]);
                }
            }
            return accObjectArray;
        }

        //
        // IAccessibleInternal implementation...
        //

        void UnsafeNativeMethods.IAccessibleInternal.accDoDefaultAction(object childID) {
            IntSecurity.UnmanagedCode.Assert();
            publicIAccessible.accDoDefaultAction(childID);
        }
        
        object UnsafeNativeMethods.IAccessibleInternal.accHitTest(int xLeft, int yTop) {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIAccessible.accHitTest(xLeft, yTop));
        }

        void UnsafeNativeMethods.IAccessibleInternal.accLocation(out int l, out int t, out int w, out int h, Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            publicIAccessible.accLocation(out l, out t, out w, out h, childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.accNavigate(int navDir, object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIAccessible.accNavigate(navDir, childID));
        }

        void UnsafeNativeMethods.IAccessibleInternal.accSelect(int flagsSelect, Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            publicIAccessible.accSelect(flagsSelect, childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accChild(object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIAccessible.get_accChild(childID));
        }

        int UnsafeNativeMethods.IAccessibleInternal.get_accChildCount() {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.accChildCount;
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accDefaultAction(Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accDefaultAction(childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accDescription(Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accDescription(childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accFocus() {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIAccessible.accFocus);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accHelp(Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accHelp(childID);
        }

        int UnsafeNativeMethods.IAccessibleInternal.get_accHelpTopic(out string pszHelpFile, Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accHelpTopic(out pszHelpFile, childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accKeyboardShortcut(Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accKeyboardShortcut(childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accName(Object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accName(childID);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accParent() {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIAccessible.accParent);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accRole(object childID) {
		    IntSecurity.UnmanagedCode.Assert();
		    return publicIAccessible.get_accRole(childID);
	    }

        object UnsafeNativeMethods.IAccessibleInternal.get_accSelection() {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIAccessible.accSelection);
        }

        object UnsafeNativeMethods.IAccessibleInternal.get_accState(object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accState(childID);
        }

        string UnsafeNativeMethods.IAccessibleInternal.get_accValue(object childID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessible.get_accValue(childID);
        }

        void UnsafeNativeMethods.IAccessibleInternal.set_accName(Object childID, string newName) {
            IntSecurity.UnmanagedCode.Assert();
            publicIAccessible.set_accName(childID, newName);
        }

        void UnsafeNativeMethods.IAccessibleInternal.set_accValue(Object childID, string newValue) {
            IntSecurity.UnmanagedCode.Assert();
            publicIAccessible.set_accValue(childID, newValue);
        }

        //
        // IEnumVariant implementation...
        //

        void UnsafeNativeMethods.IEnumVariant.Clone(UnsafeNativeMethods.IEnumVariant[] v) {
            IntSecurity.UnmanagedCode.Assert();
            publicIEnumVariant.Clone(v);
        }

        int UnsafeNativeMethods.IEnumVariant.Next(int n, IntPtr rgvar, int[] ns) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIEnumVariant.Next(n, rgvar, ns);
        }

        void UnsafeNativeMethods.IEnumVariant.Reset() {
            IntSecurity.UnmanagedCode.Assert();
            publicIEnumVariant.Reset();
        }

        void UnsafeNativeMethods.IEnumVariant.Skip(int n) {
            IntSecurity.UnmanagedCode.Assert();
            publicIEnumVariant.Skip(n);
        }

        //
        // IOleWindow implementation...
        //

        int UnsafeNativeMethods.IOleWindow.GetWindow(out IntPtr hwnd) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIOleWindow.GetWindow(out hwnd);
        }

        void UnsafeNativeMethods.IOleWindow.ContextSensitiveHelp(int fEnterMode) {
            IntSecurity.UnmanagedCode.Assert();
            publicIOleWindow.ContextSensitiveHelp(fEnterMode);
        }

        //
        // IReflect implementation...
        //

        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers) {
            return publicIReflect.GetMethod(name, bindingAttr, binder, types, modifiers);
        }

        MethodInfo IReflect.GetMethod(string name, BindingFlags bindingAttr) {
            return publicIReflect.GetMethod(name, bindingAttr);
        }

        MethodInfo[] IReflect.GetMethods(BindingFlags bindingAttr) {
            return publicIReflect.GetMethods(bindingAttr);
        }

        FieldInfo IReflect.GetField(string name, BindingFlags bindingAttr) {
            return publicIReflect.GetField(name, bindingAttr);
        }

        FieldInfo[] IReflect.GetFields(BindingFlags bindingAttr) {
            return publicIReflect.GetFields(bindingAttr);
        }

        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr) {
            return publicIReflect.GetProperty(name, bindingAttr);
        }

        PropertyInfo IReflect.GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers) {
            return publicIReflect.GetProperty(name, bindingAttr, binder, returnType, types, modifiers);
        }

        PropertyInfo[] IReflect.GetProperties(BindingFlags bindingAttr) {
            return publicIReflect.GetProperties(bindingAttr);
        }

        MemberInfo[] IReflect.GetMember(string name, BindingFlags bindingAttr) {
            return publicIReflect.GetMember(name, bindingAttr);
        }

        MemberInfo[] IReflect.GetMembers(BindingFlags bindingAttr) {
            return publicIReflect.GetMembers(bindingAttr);
        }

        object IReflect.InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters) {
            IntSecurity.UnmanagedCode.Demand();
            return publicIReflect.InvokeMember(name, invokeAttr, binder, publicIAccessible, args, modifiers, culture, namedParameters);
        }

        Type IReflect.UnderlyingSystemType {
            get {
                IReflect r = publicIReflect;
                return publicIReflect.UnderlyingSystemType;
            }
        }

        //
        // IServiceProvider implementation
        //

        int UnsafeNativeMethods.IServiceProvider.QueryService(ref Guid service, ref Guid riid, out IntPtr ppvObject) {
            IntSecurity.UnmanagedCode.Assert();

            ppvObject = IntPtr.Zero;
            int hr = publicIServiceProvider.QueryService(ref service, ref riid, out ppvObject);
            if (hr >= NativeMethods.S_OK) {
                // we always want to return the internal accessible object
                ppvObject = Marshal.GetComInterfaceForObject(this, typeof(UnsafeNativeMethods.IAccessibleEx));
            }

            return hr;
        }

        //
        // IAccessibleEx implementation
        //

        object UnsafeNativeMethods.IAccessibleEx.GetObjectForChild(int idChild) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessibleEx.GetObjectForChild(idChild);
        }

        int UnsafeNativeMethods.IAccessibleEx.GetIAccessiblePair(out object ppAcc, out int pidChild) {

            IntSecurity.UnmanagedCode.Assert();

            // We always want to return the internal accessible object
            ppAcc = this;
            pidChild = NativeMethods.CHILDID_SELF;
            return NativeMethods.S_OK;
        }

        int[] UnsafeNativeMethods.IAccessibleEx.GetRuntimeId() {

            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessibleEx.GetRuntimeId();
        }

        int UnsafeNativeMethods.IAccessibleEx.ConvertReturnedElement(object pIn, out object ppRetValOut) {

            IntSecurity.UnmanagedCode.Assert();
            return publicIAccessibleEx.ConvertReturnedElement(pIn, out ppRetValOut);
        }

        //
        // IRawElementProviderSimple implementation
        //

        UnsafeNativeMethods.ProviderOptions UnsafeNativeMethods.IRawElementProviderSimple.ProviderOptions {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRawElementProviderSimple.ProviderOptions;
            }
        }

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IRawElementProviderSimple.HostRawElementProvider {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRawElementProviderSimple.HostRawElementProvider;
            }
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPatternProvider(int patternId) {
            IntSecurity.UnmanagedCode.Assert();

            object obj = publicIRawElementProviderSimple.GetPatternProvider(patternId);
            if (obj != null) {

                // we always want to return the internal accessible object

                if (patternId == NativeMethods.UIA_ExpandCollapsePatternId) {
                    return (UnsafeNativeMethods.IExpandCollapseProvider)this;
                }
                else if (patternId == NativeMethods.UIA_ValuePatternId) {
                    return (UnsafeNativeMethods.IValueProvider)this;
                }
                else if (AccessibilityImprovements.Level3 && patternId == NativeMethods.UIA_RangeValuePatternId) {
                    return (UnsafeNativeMethods.IRangeValueProvider)this;
                }
                else if (patternId == NativeMethods.UIA_TogglePatternId) {
                    return (UnsafeNativeMethods.IToggleProvider)this;
                }
                else if (patternId == NativeMethods.UIA_TablePatternId) {
                    return (UnsafeNativeMethods.ITableProvider)this;
                }
                else if (patternId == NativeMethods.UIA_TableItemPatternId) {
                    return (UnsafeNativeMethods.ITableItemProvider)this;
                }
                else if (patternId == NativeMethods.UIA_GridPatternId) {
                    return (UnsafeNativeMethods.IGridProvider)this;
                }
                else if (patternId == NativeMethods.UIA_GridItemPatternId) {
                    return (UnsafeNativeMethods.IGridItemProvider)this;
                }
                else if (AccessibilityImprovements.Level3 && patternId == NativeMethods.UIA_InvokePatternId) {
                    return (UnsafeNativeMethods.IInvokeProvider)this;
                }
                else if (AccessibilityImprovements.Level3 && patternId == NativeMethods.UIA_LegacyIAccessiblePatternId) {
                    return (UnsafeNativeMethods.ILegacyIAccessibleProvider)this;
                }
                else {
                    return null;
                }
            }
            else {
                return null;
            }
        }

        object UnsafeNativeMethods.IRawElementProviderSimple.GetPropertyValue(int propertyID) {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRawElementProviderSimple.GetPropertyValue(propertyID);
        }

        //
        // IRawElementProviderFragment implementation
        //

        object UnsafeNativeMethods.IRawElementProviderFragment.Navigate(UnsafeNativeMethods.NavigateDirection direction) {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIRawElementProviderFragment.Navigate(direction));
        }

        int[] UnsafeNativeMethods.IRawElementProviderFragment.GetRuntimeId() {
            IntSecurity.UnmanagedCode.Assert();
            return publicIRawElementProviderFragment.GetRuntimeId();
        }

        object[] UnsafeNativeMethods.IRawElementProviderFragment.GetEmbeddedFragmentRoots() {
            IntSecurity.UnmanagedCode.Assert();
            return AsArrayOfNativeAccessibles(publicIRawElementProviderFragment.GetEmbeddedFragmentRoots());
        }

        void UnsafeNativeMethods.IRawElementProviderFragment.SetFocus() {
            IntSecurity.UnmanagedCode.Assert();
            publicIRawElementProviderFragment.SetFocus();
        }

        NativeMethods.UiaRect UnsafeNativeMethods.IRawElementProviderFragment.BoundingRectangle {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRawElementProviderFragment.BoundingRectangle;
            }
        }

        UnsafeNativeMethods.IRawElementProviderFragmentRoot UnsafeNativeMethods.IRawElementProviderFragment.FragmentRoot {
            get {
                IntSecurity.UnmanagedCode.Assert();
                if (AccessibilityImprovements.Level3) {
                    return publicIRawElementProviderFragment.FragmentRoot;
                }

                return AsNativeAccessible(publicIRawElementProviderFragment.FragmentRoot) as UnsafeNativeMethods.IRawElementProviderFragmentRoot;
            }
        }

        //
        // IRawElementProviderFragmentRoot implementation
        //

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.ElementProviderFromPoint(double x, double y) {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIRawElementProviderFragmentRoot.ElementProviderFromPoint(x, y));
        }

        object UnsafeNativeMethods.IRawElementProviderFragmentRoot.GetFocus() {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIRawElementProviderFragmentRoot.GetFocus());
        }

        //
        // ILegacyIAccessibleProvider implementation
        //

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.DefaultAction {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.DefaultAction;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Description {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.Description;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Help {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.Help;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.KeyboardShortcut {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.KeyboardShortcut;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Name {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.Name;
            }
        }

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.Role {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.Role;
            }
        }

        uint UnsafeNativeMethods.ILegacyIAccessibleProvider.State {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.State;
            }
        }

        string UnsafeNativeMethods.ILegacyIAccessibleProvider.Value {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.Value;
            }
        }

        int UnsafeNativeMethods.ILegacyIAccessibleProvider.ChildId {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicILegacyIAccessibleProvider.ChildId;
            }
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.DoDefaultAction() {
            IntSecurity.UnmanagedCode.Assert();
            publicILegacyIAccessibleProvider.DoDefaultAction();
        }

        IAccessible UnsafeNativeMethods.ILegacyIAccessibleProvider.GetIAccessible() {
            IntSecurity.UnmanagedCode.Assert();
            return publicILegacyIAccessibleProvider.GetIAccessible();
        }

        object[] UnsafeNativeMethods.ILegacyIAccessibleProvider.GetSelection() {
            IntSecurity.UnmanagedCode.Assert();
            return AsArrayOfNativeAccessibles(publicILegacyIAccessibleProvider.GetSelection());
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.Select(int flagsSelect) {
            IntSecurity.UnmanagedCode.Assert();
            publicILegacyIAccessibleProvider.Select(flagsSelect);
        }

        void UnsafeNativeMethods.ILegacyIAccessibleProvider.SetValue(string szValue) {
            IntSecurity.UnmanagedCode.Assert();
            publicILegacyIAccessibleProvider.SetValue(szValue);
        }

        //
        // IInvokeProvider implementation
        //

        void UnsafeNativeMethods.IInvokeProvider.Invoke() {
            IntSecurity.UnmanagedCode.Assert();
            publicIInvokeProvider.Invoke();
        }

        //
        // IValueProvider implementation
        //

        bool UnsafeNativeMethods.IValueProvider.IsReadOnly {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIValueProvider.IsReadOnly;
            }
        }

        string UnsafeNativeMethods.IValueProvider.Value {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIValueProvider.Value;
            }
        }

        void UnsafeNativeMethods.IValueProvider.SetValue(string newValue) {
            IntSecurity.UnmanagedCode.Assert();
            publicIValueProvider.SetValue(newValue);
        }

        //
        // IRangeValueProvider implementation
        //

        bool UnsafeNativeMethods.IRangeValueProvider.IsReadOnly {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIValueProvider.IsReadOnly;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.LargeChange {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRangeValueProvider.LargeChange;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.Maximum {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRangeValueProvider.Maximum;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.Minimum {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRangeValueProvider.Minimum;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.SmallChange {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRangeValueProvider.SmallChange;
            }
        }

        double UnsafeNativeMethods.IRangeValueProvider.Value {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIRangeValueProvider.Value;
            }
        }

        void UnsafeNativeMethods.IRangeValueProvider.SetValue(double newValue) {
            IntSecurity.UnmanagedCode.Assert();
            publicIRangeValueProvider.SetValue(newValue);
        }

        //
        // IExpandCollapseProvider implementation
        //

        void UnsafeNativeMethods.IExpandCollapseProvider.Expand() {
            IntSecurity.UnmanagedCode.Assert();
            publicIExpandCollapseProvider.Expand();
        }

        void UnsafeNativeMethods.IExpandCollapseProvider.Collapse() {
            IntSecurity.UnmanagedCode.Assert();
            publicIExpandCollapseProvider.Collapse();
        }

        UnsafeNativeMethods.ExpandCollapseState UnsafeNativeMethods.IExpandCollapseProvider.ExpandCollapseState {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIExpandCollapseProvider.ExpandCollapseState;
            }
        }

        //
        // IToggleProvider implementation
        //

        void UnsafeNativeMethods.IToggleProvider.Toggle() {
            IntSecurity.UnmanagedCode.Assert();
            publicIToggleProvider.Toggle();
        }

        UnsafeNativeMethods.ToggleState UnsafeNativeMethods.IToggleProvider.ToggleState {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIToggleProvider.ToggleState;
            }
        }

        //
        // ITableProvider implementation
        //

        object[] UnsafeNativeMethods.ITableProvider.GetRowHeaders() {
            IntSecurity.UnmanagedCode.Assert();
            return AsArrayOfNativeAccessibles(publicITableProvider.GetRowHeaders());
        }

        object[] UnsafeNativeMethods.ITableProvider.GetColumnHeaders() {
            IntSecurity.UnmanagedCode.Assert();
            return AsArrayOfNativeAccessibles(publicITableProvider.GetColumnHeaders());
        }

        UnsafeNativeMethods.RowOrColumnMajor UnsafeNativeMethods.ITableProvider.RowOrColumnMajor {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicITableProvider.RowOrColumnMajor;
            }
        }

        //
        // ITableItemProvider implementation
        //

        object[] UnsafeNativeMethods.ITableItemProvider.GetRowHeaderItems() {
            IntSecurity.UnmanagedCode.Assert();
            return AsArrayOfNativeAccessibles(publicITableItemProvider.GetRowHeaderItems());
        }

        object[] UnsafeNativeMethods.ITableItemProvider.GetColumnHeaderItems() {
            IntSecurity.UnmanagedCode.Assert();
            return AsArrayOfNativeAccessibles(publicITableItemProvider.GetColumnHeaderItems());
        }

        //
        // IGridProvider implementation
        //

        object UnsafeNativeMethods.IGridProvider.GetItem(int row, int column) {
            IntSecurity.UnmanagedCode.Assert();
            return AsNativeAccessible(publicIGridProvider.GetItem(row, column));
        }

        int UnsafeNativeMethods.IGridProvider.RowCount {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIGridProvider.RowCount;
            }
        }

        int UnsafeNativeMethods.IGridProvider.ColumnCount {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIGridProvider.ColumnCount;
            }
        }

        //
        // IGridItemProvider implementation
        //
                
        int UnsafeNativeMethods.IGridItemProvider.Row {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIGridItemProvider.Row;
            }
        }

        int UnsafeNativeMethods.IGridItemProvider.Column {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIGridItemProvider.Column;
            }
        }

        int UnsafeNativeMethods.IGridItemProvider.RowSpan {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIGridItemProvider.RowSpan;
            }
        }

        int UnsafeNativeMethods.IGridItemProvider.ColumnSpan {
            get {
                IntSecurity.UnmanagedCode.Assert();
                return publicIGridItemProvider.ColumnSpan;
            }
        }

        UnsafeNativeMethods.IRawElementProviderSimple UnsafeNativeMethods.IGridItemProvider.ContainingGrid {
            get {
                IntSecurity.UnmanagedCode.Assert();

                // Do not wrap returned UIA provider by InternalAccessibleObject in Level 3.
                if (AccessibilityImprovements.Level3) {
                    return publicIGridItemProvider.ContainingGrid;
                }

                return AsNativeAccessible(publicIGridItemProvider.ContainingGrid) as UnsafeNativeMethods.IRawElementProviderSimple;
            }
        }

    } // end class InternalAccessibleObject

}
