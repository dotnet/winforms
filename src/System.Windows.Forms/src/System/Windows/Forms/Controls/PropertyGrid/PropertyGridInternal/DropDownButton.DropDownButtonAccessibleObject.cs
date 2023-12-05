// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry;

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed partial class DropDownButton : Button
{
    /// <summary>
    ///  Represents the accessibility object for the PropertyGrid DropDown button.
    ///  This DropDownButtonAccessibleObject is available in Level3 only.
    /// </summary>
    internal class DropDownButtonAccessibleObject : ControlAccessibleObject
    {
        private readonly DropDownButton _owningDropDownButton;
        private readonly PropertyGridView? _owningPropertyGrid;

        /// <summary>
        ///  Constructs the new instance of DropDownButtonAccessibleObject.
        /// </summary>
        /// <param name="owningDropDownButton"></param>
        public DropDownButtonAccessibleObject(DropDownButton owningDropDownButton) : base(owningDropDownButton)
        {
            _owningDropDownButton = owningDropDownButton;
            _owningPropertyGrid = owningDropDownButton.Parent as PropertyGridView;

            if (owningDropDownButton.IsHandleCreated)
            {
                UseStdAccessibleObjects(owningDropDownButton.Handle);
            }
        }

        public override void DoDefaultAction()
        {
            if (_owningDropDownButton.IsHandleCreated)
            {
                _owningDropDownButton.PerformButtonClick();
            }
        }

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!_owningDropDownButton.Visible
                || _owningPropertyGrid?.SelectedGridEntry?.AccessibilityObject is not PropertyDescriptorGridEntryAccessibleObject parent)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_Parent => parent,
                NavigateDirection.NavigateDirection_NextSibling => parent.GetNextChild(this),
                NavigateDirection.NavigateDirection_PreviousSibling => parent.GetPreviousChild(this),
                _ => base.FragmentNavigate(direction),
            };
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot
            => _owningPropertyGrid?.AccessibilityObject;

        /// <summary>
        ///  Request value of specified property from an element.
        /// </summary>
        /// <param name="propertyID">Identifier indicating the property to return</param>
        /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                _ => base.GetPropertyValue(propertyID),
            };

        /// <summary>
        ///  Gets the accessible role.
        /// </summary>
        public override AccessibleRole Role => AccessibleRole.PushButton;

        /// <summary>
        ///  Request that focus is set to this item.
        ///  The UIAutomation framework will ensure that the UI hosting this fragment is already
        ///  focused before calling this method, so this method should only update its internal
        ///  focus state; it should not attempt to give its own HWND the focus, for example.
        /// </summary>
        internal override void SetFocus()
        {
            if (!_owningDropDownButton.IsHandleCreated)
            {
                return;
            }

            RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);

            base.SetFocus();
        }
    }
}
