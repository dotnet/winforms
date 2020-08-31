// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Automation;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Control
    {
        /// <summary>
        ///  An implementation of AccessibleChild for use with Controls
        /// </summary>
        public class ControlAccessibleObject : AccessibleObject
        {
            private static IntPtr s_oleAccAvailable = NativeMethods.InvalidIntPtr;

            private IntPtr _handle = IntPtr.Zero;   // Associated window handle (if any)
            private int[]? _runtimeId;              // Used by UIAutomation
            private bool _getOwnerControlHandle;

            public ControlAccessibleObject(Control ownerControl)
            {
                Owner = ownerControl ?? throw new ArgumentNullException(nameof(ownerControl));
                InitHandle(ownerControl);
            }

            internal ControlAccessibleObject(Control ownerControl, int accObjId)
            {
                AccessibleObjectId = accObjId; // ...must set this *before* setting the Handle property
                Owner = ownerControl ?? throw new ArgumentNullException(nameof(ownerControl));
                InitHandle(ownerControl);
            }

            private void InitHandle(Control ownerControl)
            {
                if (ownerControl.IsHandleCreated)
                {
                    Handle = ownerControl.Handle;
                }
                else
                {
                    // If the owner control doesn't have a valid handle, wait until there is either
                    // a request to create it, or the owner control creates a handle, which will
                    // be set via Handle property.
                    _getOwnerControlHandle = true;
                }
            }

            /// <summary>
            ///  For container controls only, return array of child controls sorted into
            ///  tab index order. This gets applies to the list of child accessible objects
            ///  as returned by the system, so that we can present a meaningful order to
            ///  the user. The system defaults to z-order, which is bad for us because
            ///  that is usually the reverse of tab order!
            /// </summary>
            internal override int[]? GetSysChildOrder()
                => Owner.GetStyle(ControlStyles.ContainerControl)
                    ? Owner.GetChildWindowsInTabOrder() : base.GetSysChildOrder();

            /// <summary>
            ///  Perform custom navigation between parent/child/sibling accessible objects,
            ///  using tab index order as the guide, rather than letting the system default
            ///  behavior do navigation based on z-order.
            ///
            ///  For a container control and its child controls, the accessible object tree
            ///  looks like this...
            ///
            ///  [client area of container]
            ///   [non-client area of child #1]
            ///       [random non-client elements]
            ///       [client area of child #1]
            ///       [random non-client elements]
            ///   [non-client area of child #2]
            ///       [random non-client elements]
            ///       [client area of child #2]
            ///       [random non-client elements]
            ///   [non-client area of child #3]
            ///       [random non-client elements]
            ///       [client area of child #3]
            ///       [random non-client elements]
            ///
            ///  We need to intercept first-child / last-child navigation from the container's
            ///  client object, and next-sibling / previous-sibling navigation from each child's
            ///  non-client object. All other navigation operations must be allowed to fall back
            ///  on the system's deafult behavior (provided by OLEACC.DLL).
            ///
            ///  When combined with the re-ordering behavior of GetSysChildOrder() above, this
            ///  allows us to present the end user with the illusion of accessible objects in
            ///  tab index order, even though the system behavior only supports z-order.
            /// </summary>
            internal override bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject? accessibleObject)
            {
                // Clear the out parameter
                accessibleObject = null;

                // Get the owning control's parent, if it has one
                Control parentControl = Owner.ParentInternal;

                // ctrls[index] will indicate the control at the destination of this navigation operation
                int index = -1;
                Control[]? ctrls = null;

                // Now handle any 'appropriate' navigation requests...
                switch (navdir)
                {
                    case AccessibleNavigation.FirstChild:
                        if (IsClientObject)
                        {
                            ctrls = Owner.GetChildControlsInTabOrder(true);
                            index = 0;
                        }
                        break;
                    case AccessibleNavigation.LastChild:
                        if (IsClientObject)
                        {
                            ctrls = Owner.GetChildControlsInTabOrder(true);
                            index = ctrls.Length - 1;
                        }
                        break;
                    case AccessibleNavigation.Previous:
                        if (IsNonClientObject && parentControl != null)
                        {
                            ctrls = parentControl.GetChildControlsInTabOrder(true);
                            index = Array.IndexOf(ctrls, Owner);
                            if (index != -1)
                            {
                                --index;
                            }
                        }
                        break;
                    case AccessibleNavigation.Next:
                        if (IsNonClientObject && parentControl != null)
                        {
                            ctrls = parentControl.GetChildControlsInTabOrder(true);
                            index = Array.IndexOf(ctrls, Owner);
                            if (index != -1)
                            {
                                ++index;
                            }
                        }
                        break;
                }

                // Unsupported navigation operation for this object, or unexpected error.
                // Return false to force fall back on default system navigation behavior.
                if (ctrls is null || ctrls.Length == 0)
                {
                    return false;
                }

                // If ctrls[index] is a valid control, return its non-client accessible object.
                // If index is invalid, return null pointer meaning "end of list reached".
                if (index >= 0 && index < ctrls.Length)
                {
                    accessibleObject = ctrls[index].NcAccessibilityObject;
                }

                // Return true to use the found accessible object and block default system behavior
                return true;
            }

            public override string? DefaultAction => Owner.AccessibleDefaultActionDescription ?? base.DefaultAction;

            // This is used only if control supports IAccessibleEx
            internal override int[]? RuntimeId
            {
                get
                {
                    if (_runtimeId is null)
                    {
                        _runtimeId = new int[] { 0x2a, (int)(long)HandleInternal };
                    }

                    return _runtimeId;
                }
            }

            public override string? Description => Owner.AccessibleDescription ?? base.Description;

            public IntPtr Handle
            {
                get
                {
                    if (_getOwnerControlHandle)
                    {
                        _getOwnerControlHandle = false;
                        _handle = Owner.Handle;
                    }

                    return _handle;
                }
                set
                {
                    if (_handle == value)
                    {
                        return;
                    }

                    _handle = value;
                    _getOwnerControlHandle = false;

                    if (s_oleAccAvailable == IntPtr.Zero || _handle == IntPtr.Zero)
                    {
                        return;
                    }

                    bool freeLib = false;

                    if (s_oleAccAvailable == NativeMethods.InvalidIntPtr)
                    {
                        s_oleAccAvailable = Kernel32.LoadLibraryFromSystemPathIfAvailable("oleacc.dll");
                        freeLib = (s_oleAccAvailable != IntPtr.Zero);
                    }

                    // Update systemIAccessible
                    //
                    // We need to store internally the system provided
                    // IAccessible, because some windows forms controls use it
                    // as the default IAccessible implementation.
                    if (s_oleAccAvailable != IntPtr.Zero)
                    {
                        UseStdAccessibleObjects(_handle);
                    }

                    if (freeLib)
                    {
                        Kernel32.FreeLibrary(s_oleAccAvailable);
                    }
                }
            }

            internal IntPtr HandleInternal => _handle;

            public override string? Help
            {
                get
                {
                    QueryAccessibilityHelpEventHandler? handler = (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent];
                    if (handler != null)
                    {
                        QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                        handler(Owner, args);
                        return args.HelpString;
                    }

                    return base.Help;
                }
            }

            public override string? KeyboardShortcut
            {
                get
                {
                    // For controls, the default keyboard shortcut comes directly from the accessible
                    // name property. This matches the default behavior of OLEACC.DLL exactly.
                    char mnemonic = WindowsFormsUtils.GetMnemonic(TextLabel, false);
                    return (mnemonic == (char)0) ? null : ("Alt+" + mnemonic);
                }
            }

            public override string? Name
            {
                get
                {
                    // Special case: If an explicit name has been set in the AccessibleName property, use that.
                    // Note: Any non-null value in AccessibleName overrides the default accessible name logic,
                    // even an empty string (this is the only way to *force* the accessible name to be blank).
                    string? name = Owner.AccessibleName;
                    if (name != null)
                    {
                        return name;
                    }

                    // Otherwise just return the default label string, minus any mnemonics
                    return WindowsFormsUtils.TextWithoutMnemonics(TextLabel);
                }
                set
                {
                    // If anyone tries to set the accessible name, just cache the value in the control's
                    // AccessibleName property. This value will then end up overriding the normal accessible
                    // name logic, until such time as AccessibleName is set back to null.
                    Owner.AccessibleName = value;
                }
            }

            public override AccessibleObject? Parent => base.Parent;

            // Determine the text that should be used to 'label' this control for accessibility purposes.
            //
            // Prior to Whidbey, we just called 'base.Name' to determine the accessible name. This would end up calling
            // OLEACC.DLL to determine the name. The rules used by OLEACC.DLL are the same as what we now have below,
            // except that OLEACC searches for preceding labels using z-order, and we want to search for labels using
            // TabIndex order.
            internal string? TextLabel
            {
                get
                {
                    // If owning control allows, use its Text property - but only if that Text is not empty
                    if (Owner.GetStyle(ControlStyles.UseTextForAccessibility))
                    {
                        string text = Owner.Text;
                        if (!string.IsNullOrEmpty(text))
                        {
                            return text;
                        }
                    }

                    // Otherwise use the text of the preceding Label control, if there is one
                    Label? previousLabel = PreviousLabel;
                    if (previousLabel != null)
                    {
                        string text = previousLabel.Text;
                        if (!string.IsNullOrEmpty(text))
                        {
                            return text;
                        }
                    }

                    // This control has no discernable MSAA name - return an empty string to indiciate 'nameless'.
                    return null;
                }
            }

            public Control Owner { get; }

            // Look for a label immediately preceeding this control in
            // the tab order, and use its name for the accessible name.
            //
            // This method aims to emulate the equivalent behavior in OLEACC.DLL,
            // but walking controls in TabIndex order rather than native z-order.
            internal Label? PreviousLabel
            {
                get
                {
                    // Try to get to the parent of this control.
                    Control parent = Owner.ParentInternal;

                    if (parent is null)
                    {
                        return null;
                    }

                    // Find this control's containing control
                    if (!(parent.GetContainerControl() is ContainerControl container))
                    {
                        return null;
                    }

                    // Walk backwards through peer controls...
                    for (Control previous = container.GetNextControl(Owner, false);
                         previous != null;
                         previous = container.GetNextControl(previous, false))
                    {
                        // Stop when we hit a Label (whether visible or invisible)
                        if (previous is Label label)
                        {
                            return label;
                        }

                        // Stop at any *visible* tab stop
                        if (previous.Visible && previous.TabStop)
                        {
                            break;
                        }
                    }

                    return null;
                }
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    return role != AccessibleRole.Default
                        ? role : base.Role;
                }
            }

            public override int GetHelpTopic(out string? fileName)
            {
                int topic = 0;

                QueryAccessibilityHelpEventHandler? handler = (QueryAccessibilityHelpEventHandler?)Owner.Events[s_queryAccessibilityHelpEvent];
                if (handler is null)
                {
                    return base.GetHelpTopic(out fileName);
                }

                QueryAccessibilityHelpEventArgs args = new QueryAccessibilityHelpEventArgs();
                handler(Owner, args);

                fileName = args.HelpNamespace;

                try
                {
                    topic = int.Parse(args.HelpKeyword, CultureInfo.InvariantCulture);
                }
                catch (Exception e) when (!ClientUtils.IsCriticalException(e))
                {
                }

                return topic;
            }

            public void NotifyClients(AccessibleEvents accEvent)
            {
                if (HandleInternal == IntPtr.Zero)
                {
                    return;
                }

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo,
                    $"Control.NotifyClients: this = {ToString()}, accEvent = {accEvent}, childID = self");

                User32.NotifyWinEvent((uint)accEvent, new HandleRef(this, HandleInternal), User32.OBJID.CLIENT, 0);
            }

            public void NotifyClients(AccessibleEvents accEvent, int childID)
            {
                if (HandleInternal == IntPtr.Zero)
                {
                    return;
                }

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo,
                    $"Control.NotifyClients: this = {ToString()}, accEvent = {accEvent}, childID = {childID}");

                User32.NotifyWinEvent((uint)accEvent, new HandleRef(this, HandleInternal), User32.OBJID.CLIENT, childID + 1);
            }

            public void NotifyClients(AccessibleEvents accEvent, int objectID, int childID)
            {
                if (HandleInternal == IntPtr.Zero)
                {
                    return;
                }

                Debug.WriteLineIf(CompModSwitches.MSAA.TraceInfo,
                    $"Control.NotifyClients: this = {ToString()}, accEvent = {accEvent}, childID = {childID}");

                User32.NotifyWinEvent((uint)accEvent, new HandleRef(this, HandleInternal), objectID, childID + 1);
            }

            /// <summary>
            ///  Raises the LiveRegionChanged UIA event.
            ///  To make this method effective, the control must implement System.Windows.Forms.Automation.IAutomationLiveRegion interface
            ///  and its LiveSetting property must return either AutomationLiveSetting.Polite or AutomationLiveSetting.Assertive value.
            /// </summary>
            /// <returns>True if operation succeeds, False otherwise.</returns>
            public override bool RaiseLiveRegionChanged()
            {
                if (!(Owner is IAutomationLiveRegion))
                {
                    throw new InvalidOperationException(SR.OwnerControlIsNotALiveRegion);
                }

                return RaiseAutomationEvent(UiaCore.UIA.LiveRegionChangedEventId);
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (Owner.SupportsUiaProviders && patternId == UiaCore.UIA.LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override bool IsIAccessibleExSupported()
                => Owner is IAutomationLiveRegion ? true : base.IsIAccessibleExSupported();

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.LiveSettingPropertyId && Owner is IAutomationLiveRegion)
                {
                    return ((IAutomationLiveRegion)Owner).LiveSetting;
                }

                if (Owner.SupportsUiaProviders)
                {
                    switch (propertyID)
                    {
                        case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                            return Owner.CanSelect;
                        case UiaCore.UIA.IsOffscreenPropertyId:
                        case UiaCore.UIA.IsPasswordPropertyId:
                            return false;
                        case UiaCore.UIA.AccessKeyPropertyId:
                            return string.Empty;
                        case UiaCore.UIA.HelpTextPropertyId:
                            return Help ?? string.Empty;
                    }
                }

                return base.GetPropertyValue(propertyID);
            }

            internal override bool RaiseAutomationEvent(UiaCore.UIA eventId)
            {
                if (!Owner.IsHandleCreated)
                {
                    return false;
                }

                return base.RaiseAutomationEvent(eventId);
            }

            internal override bool RaiseAutomationPropertyChangedEvent(UiaCore.UIA propertyId, object oldValue, object newValue)
            {
                if (!Owner.IsHandleCreated)
                {
                    return false;
                }

                return base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);
            }

            internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
            {
                get
                {
                    if (HandleInternal == IntPtr.Zero)
                    {
                        return null;
                    }

                    UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, HandleInternal), out UiaCore.IRawElementProviderSimple provider);
                    return provider;
                }
            }

            public override string ToString()
                => $"{nameof(ControlAccessibleObject)}: Owner = {Owner?.ToString() ?? "null"}";
        }
    }
}
