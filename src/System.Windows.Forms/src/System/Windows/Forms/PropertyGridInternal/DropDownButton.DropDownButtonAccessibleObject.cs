// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
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

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (direction == UiaCore.NavigateDirection.Parent &&
                    _owningPropertyGrid?.SelectedGridEntry is not null &&
                    _owningDropDownButton.Visible)
                {
                    return _owningPropertyGrid.SelectedGridEntry?.AccessibilityObject;
                }
                else if (direction == UiaCore.NavigateDirection.PreviousSibling)
                {
                    return _owningPropertyGrid?.EditAccessibleObject;
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Returns the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
                => _owningPropertyGrid?.AccessibilityObject;

            /// <summary>
            ///  Request value of specified property from an element.
            /// </summary>
            /// <param name="propertyID">Identifier indicating the property to return</param>
            /// <returns>Returns a ValInfo indicating whether the element supports this property, or has no value for it.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
            {
                UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
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

                RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);

                base.SetFocus();
            }
        }
    }
}
