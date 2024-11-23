// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Globalization;
using System.Windows.Forms.Automation;
using System.Windows.Forms.Primitives;

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class Control
{
    /// <summary>
    ///  An implementation of AccessibleChild for use with Controls.
    /// </summary>
    public class ControlAccessibleObject : AccessibleObject, IOwnedObject<Control>
    {
        /// <summary>
        ///  Associated window handle (if any).
        /// </summary>
        private HWND _handle = HWND.Null;

        /// <summary>
        ///  Used to lazily grab the owner control handle, if it hasn't been created yet.
        /// </summary>
        private bool _getOwnerControlHandle;

        private readonly WeakReference<Control> _ownerControl;

        public ControlAccessibleObject(Control ownerControl)
            : this(ownerControl, (int)OBJECT_IDENTIFIER.OBJID_CLIENT)
        {
        }

        internal ControlAccessibleObject(Control ownerControl, int accObjId)
        {
            // Must set AccesibleObjectId *before* setting the Handle property as it calls UseStdAccessibleObjects()
            // which needs the Id.
            AccessibleObjectId = accObjId;
            _ownerControl = new(ownerControl.OrThrowIfNull());

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

        private protected override string AutomationId
            => this.TryGetOwnerAs(out Control? owner) ? owner.Name : string.Empty;

        // If the control is used as an item of a ToolStrip via ToolStripControlHost,
        // its accessible object should provide info about the owning ToolStrip and items-siblings
        // to build a correct ToolStrip accessibility tree.
        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!this.TryGetOwnerAs(out Control? owner) || owner.ToolStripControlHost is not ToolStripControlHost host)
            {
                return base.FragmentNavigate(direction);
            }

            return direction is NavigateDirection.NavigateDirection_Parent
                or NavigateDirection.NavigateDirection_PreviousSibling
                or NavigateDirection.NavigateDirection_NextSibling
                ? host.AccessibilityObject.FragmentNavigate(direction)
                : base.FragmentNavigate(direction);
        }

        /// <summary>
        ///  For container controls only, return array of child controls sorted into
        ///  tab index order. This gets applies to the list of child accessible objects
        ///  as returned by the system, so that we can present a meaningful order to
        ///  the user. The system defaults to z-order, which is bad for us because
        ///  that is usually the reverse of tab order!
        /// </summary>
        internal override int[]? GetSysChildOrder()
            => this.TryGetOwnerAs(out Control? owner) && owner.GetStyle(ControlStyles.ContainerControl)
                ? owner.GetChildWindowsInTabOrder()
                : base.GetSysChildOrder();

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
        ///  on the system's default behavior (provided by OLEACC.DLL).
        ///
        ///  When combined with the re-ordering behavior of GetSysChildOrder() above, this
        ///  allows us to present the end user with the illusion of accessible objects in
        ///  tab index order, even though the system behavior only supports z-order.
        /// </summary>
        internal override bool GetSysChild(AccessibleNavigation navdir, out AccessibleObject? accessibleObject)
        {
            if (!this.TryGetOwnerAs(out Control? owner))
            {
                return base.GetSysChild(navdir, out accessibleObject);
            }

            // Clear the out parameter
            accessibleObject = null;

            // Get the owning control's parent, if it has one
            Control? parentControl = owner.ParentInternal;

            // ctrls[index] will indicate the control at the destination of this navigation operation.
            int index = -1;
            Control[]? ctrls = null;

            // Now handle any 'appropriate' navigation requests.
            switch (navdir)
            {
                case AccessibleNavigation.FirstChild:
                    if (IsClientObject)
                    {
                        ctrls = owner.GetChildControlsInTabOrder(handleCreatedOnly: true);
                        index = 0;
                    }

                    break;
                case AccessibleNavigation.LastChild:
                    if (IsClientObject)
                    {
                        ctrls = owner.GetChildControlsInTabOrder(true);
                        index = ctrls.Length - 1;
                    }

                    break;
                case AccessibleNavigation.Previous:
                    if (IsNonClientObject && parentControl is not null)
                    {
                        ctrls = parentControl.GetChildControlsInTabOrder(true);
                        index = Array.IndexOf(ctrls, owner);
                        if (index != -1)
                        {
                            --index;
                        }
                    }

                    break;
                case AccessibleNavigation.Next:
                    if (IsNonClientObject && parentControl is not null)
                    {
                        ctrls = parentControl.GetChildControlsInTabOrder(true);
                        index = Array.IndexOf(ctrls, owner);
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

        public override string? DefaultAction => Owner?.AccessibleDefaultActionDescription ?? base.DefaultAction;

        internal override bool CanGetDefaultActionInternal => IsInternal && Owner?.AccessibleDefaultActionDescription is null;

        /// <remarks>
        ///  <para>
        ///    This is used only if control supports <see cref="IAccessibleEx" />. We need to provide a unique ID.
        ///    Others are implementing this in the same manner. First item is static - <see cref="AccessibleObject.RuntimeIDFirstItem"/>).
        ///    Second item can be anything unique, Win32 uses <see cref="HWND"/>, we copied that.
        ///  </para>
        /// </remarks>
        /// <inheritdoc cref="AccessibleObject.RuntimeId" />
        internal override int[] RuntimeId =>
        [
            RuntimeIDFirstItem,
            (int)HandleInternal,
            GetHashCode()
        ];

        public override string? Description => Owner?.AccessibleDescription ?? base.Description;

        internal override bool CanGetDescriptionInternal => IsInternal && Owner?.AccessibleDescription is null;

        /// <summary>
        ///  Gets or sets the handle of the accessible object's associated <see cref="Owner"/> control.
        /// </summary>
        /// <value>
        ///  An <see cref="IntPtr"/> that represents the handle of the associated <see cref="Owner"/> control.
        /// </value>
        /// <remarks>
        ///  <para>
        ///   The value of the <see cref="Handle"/> property for the <see cref="ControlAccessibleObject"/> is equal to
        ///   the <see cref="Control.Handle"/> property of the <see cref="Owner"/> it is associated with.
        ///  </para>
        /// </remarks>
        public IntPtr Handle
        {
            get
            {
                if (_getOwnerControlHandle)
                {
                    // We haven't gotten the associated Control handle yet, grab it now (this will create the
                    // Control's handle if it hasn't been created yet).
                    _getOwnerControlHandle = false;
                    _handle = Owner?.HWND ?? default;
                }

                return _handle;
            }
            set
            {
                if (_handle == value)
                {
                    return;
                }

                _handle = (HWND)value;
                _getOwnerControlHandle = false;

                if (_handle.IsNull)
                {
                    return;
                }

                // Create the Win32 standard accessible objects. We fall back to these in a number of code paths.
                UseStdAccessibleObjects(_handle);
            }
        }

        internal HWND HandleInternal => _handle;

        public override string? Help
        {
            get
            {
                if (this.TryGetOwnerAs(out Control? owner)
                    && owner.Events[s_queryAccessibilityHelpEvent] is QueryAccessibilityHelpEventHandler handler)
                {
                    QueryAccessibilityHelpEventArgs args = new();
                    handler(Owner, args);
                    return args.HelpString;
                }

                return base.Help;
            }
        }

        internal override bool CanGetHelpInternal =>
            IsInternal
            && (!this.TryGetOwnerAs(out Control? owner)
                || owner.Events[s_queryAccessibilityHelpEvent] is not QueryAccessibilityHelpEventHandler);

        public override string? KeyboardShortcut
        {
            get
            {
                // For controls, the default keyboard shortcut comes directly from the accessible
                // name property. This matches the default behavior of OLEACC.DLL exactly.
                char mnemonic = WindowsFormsUtils.GetMnemonic(TextLabel, false);
                return (mnemonic == (char)0) ? null : $"Alt+{mnemonic}";
            }
        }

        internal override bool CanGetKeyboardShortcutInternal => false;

        public override string? Name
        {
            get
            {
                // Special case: If an explicit name has been set in the AccessibleName property, use that.
                // Note: Any non-null value in AccessibleName overrides the default accessible name logic,
                // even an empty string (this is the only way to *force* the accessible name to be blank).
                if (this.TryGetOwnerAs(out Control? owner) && owner.AccessibleName is { } name)
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
                if (this.TryGetOwnerAs(out Control? owner))
                {
                    owner.AccessibleName = value;
                }
            }
        }

        internal override bool CanGetNameInternal => false;

        internal override bool CanSetNameInternal => false;

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
                if (this.TryGetOwnerAs(out Control? owner) && owner.GetStyle(ControlStyles.UseTextForAccessibility))
                {
                    string text = owner.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        return text;
                    }
                }

                // Otherwise use the text of the preceding Label control, if there is one
                Label? previousLabel = PreviousLabel;
                if (previousLabel is not null)
                {
                    string text = previousLabel.Text;
                    if (!string.IsNullOrEmpty(text))
                    {
                        return text;
                    }
                }

                // This control has no discernable MSAA name - return an empty string to indicate 'nameless'.
                return null;
            }
        }

        /// <summary>
        ///  Gets the owner of the accessible object.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   The only time this will be null is if the owner has been garbage collected.
        ///  </para>
        /// </remarks>
        public Control? Owner => _ownerControl.TryGetTarget(out Control? owner) ? owner : null;

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
                if (!this.TryGetOwnerAs(out Control? owner) || owner.ParentInternal is not { } parent)
                {
                    return null;
                }

                // Find this control's containing control
                if (parent.GetContainerControl() is not ContainerControl container)
                {
                    return null;
                }

                // Walk backwards through peer controls...
                for (Control? previous = container.GetNextControl(owner, false);
                     previous is not null;
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
                AccessibleRole role = Owner?.AccessibleRole ?? AccessibleRole.Default;
                return role != AccessibleRole.Default ? role : base.Role;
            }
        }

        public override int GetHelpTopic(out string? fileName)
        {
            if (!this.TryGetOwnerAs(out Control? owner)
                || owner.Events[s_queryAccessibilityHelpEvent] is not QueryAccessibilityHelpEventHandler handler)
            {
                return base.GetHelpTopic(out fileName);
            }

            QueryAccessibilityHelpEventArgs args = new();
            handler(owner, args);
            fileName = args.HelpNamespace;

            int.TryParse(args.HelpKeyword, NumberStyles.Integer, CultureInfo.InvariantCulture, out int topic);

            return topic;
        }

        internal override bool CanGetHelpTopicInternal =>
            IsInternal
            && (!this.TryGetOwnerAs(out Control? owner)
                || owner.Events[s_queryAccessibilityHelpEvent] is not QueryAccessibilityHelpEventHandler);

        public void NotifyClients(AccessibleEvents accEvent)
            => NotifyClients(accEvent, (int)OBJECT_IDENTIFIER.OBJID_CLIENT, 0);

        public void NotifyClients(AccessibleEvents accEvent, int childID)
            => NotifyClients(accEvent, (int)OBJECT_IDENTIFIER.OBJID_CLIENT, childID);

        public void NotifyClients(AccessibleEvents accEvent, int objectID, int childID)
        {
            if (HandleInternal.IsNull || LocalAppContextSwitches.NoClientNotifications)
            {
                return;
            }

            PInvoke.NotifyWinEvent(
                (uint)accEvent,
                new HandleRef<HWND>(Owner, HandleInternal),
                objectID,
                childID + 1);
        }

        /// <summary>
        ///  Raises the LiveRegionChanged UIA event.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   To make this method effective, the control must implement <see cref="IAutomationLiveRegion"/>
        ///   and its <see cref="IAutomationLiveRegion.LiveSetting"/> property must return either
        ///   <see cref="AutomationLiveSetting.Polite"/> or <see cref="AutomationLiveSetting.Assertive"/>.
        ///  </para>
        /// </remarks>
        /// <returns>True if operation succeeds, False otherwise.</returns>
        public override bool RaiseLiveRegionChanged()
        {
            if (this.TryGetOwnerAs(out Control? owner))
            {
                return owner is not IAutomationLiveRegion
                    ? throw new InvalidOperationException(SR.OwnerControlIsNotALiveRegion)
                    : RaiseAutomationEvent(UIA_EVENT_ID.UIA_LiveRegionChangedEventId);
            }

            return false;
        }

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => (this.TryGetOwnerAs(out Control? owner) && owner.SupportsUiaProviders && patternId == UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)
                || base.IsPatternSupported(patternId);

        internal override bool IsIAccessibleExSupported()
            => Owner is IAutomationLiveRegion || base.IsIAccessibleExSupported();

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId
                    // "ControlType" value depends on owner's AccessibleRole value.
                    // See: docs/accessibility/accessible-role-controltype.md
                    => (VARIANT)(int)AccessibleRoleControlTypeMap.GetControlType(Role),
                UIA_PROPERTY_ID.UIA_IsEnabledPropertyId
                    => Owner?.Enabled == true ? VARIANT.True : VARIANT.False,
                UIA_PROPERTY_ID.UIA_IsKeyboardFocusablePropertyId when
                    Owner?.SupportsUiaProviders ?? false
                    => (VARIANT)Owner.CanSelect,
                UIA_PROPERTY_ID.UIA_LiveSettingPropertyId
                    => Owner is IAutomationLiveRegion owner
                        ? (VARIANT)(int)owner.LiveSetting
                        : base.GetPropertyValue(propertyID),
                UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId
                    => UIAHelper.WindowHandleToVariant(Owner?.InternalHandle ?? HWND.Null),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool RaiseAutomationEvent(UIA_EVENT_ID eventId)
            => this.IsOwnerHandleCreated(out Control? _) && base.RaiseAutomationEvent(eventId);

        internal override bool RaiseAutomationPropertyChangedEvent(UIA_PROPERTY_ID propertyId, VARIANT oldValue, VARIANT newValue)
            => this.IsOwnerHandleCreated(out Control? _)
                && base.RaiseAutomationPropertyChangedEvent(propertyId, oldValue, newValue);

        internal override unsafe IRawElementProviderSimple* HostRawElementProvider
        {
            get
            {
                if (HandleInternal.IsNull)
                {
                    return null;
                }

                PInvoke.UiaHostProviderFromHwnd(new HandleRef<HWND>(this, HandleInternal), out IRawElementProviderSimple* provider);
                return provider;
            }
        }

        // If the control is used as an item of a ToolStrip via ToolStripControlHost,
        // its accessible object should provide info about the owning ToolStrip
        // to build a correct ToolStrip accessibility tree.
        private protected override IRawElementProviderFragmentRoot.Interface? ToolStripFragmentRoot =>
            Owner?.ToolStripControlHost?.Owner?.AccessibilityObject;

        public override string ToString()
            => $"{nameof(ControlAccessibleObject)}: Owner = {Owner?.ToString() ?? "null"}";
    }
}
