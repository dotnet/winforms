// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ComboBox
    {
        /// <summary>
        ///  Represents the ComboBox's child (inner) edit native window control accessible object with UI Automation provider functionality.
        /// </summary>
        internal class ComboBoxChildEditUiaProvider : ChildAccessibleObject
        {
            private const string COMBO_BOX_EDIT_AUTOMATION_ID = "1001";

            private readonly ComboBox _owningComboBox;
            private readonly ComboBoxUiaTextProvider _textProvider;
            private readonly IntPtr _handle;

            /// <summary>
            ///  Initializes new instance of ComboBoxChildEditUiaProvider.
            /// </summary>
            /// <param name="owner">The ComboBox owning control.</param>
            /// <param name="childEditControlhandle">The child edit native window handle.</param>
            public ComboBoxChildEditUiaProvider(ComboBox owner, IntPtr childEditControlhandle) : base(owner, childEditControlhandle)
            {
                _owningComboBox = owner;
                _handle = childEditControlhandle;
                _textProvider = new ComboBoxUiaTextProvider(owner);
            }

            private protected override string AutomationId => COMBO_BOX_EDIT_AUTOMATION_ID;

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (!_owningComboBox.IsHandleCreated)
                {
                    return null;
                }

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return _owningComboBox.AccessibilityObject;
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return _owningComboBox.DroppedDown
                            ? _owningComboBox.ChildListAccessibleObject
                            : null;
                    case UiaCore.NavigateDirection.NextSibling:
                        return _owningComboBox.DropDownStyle != ComboBoxStyle.Simple
                            && _owningComboBox.AccessibilityObject is ComboBoxAccessibleObject comboBoxAccessibleObject
                                ? comboBoxAccessibleObject.DropDownButtonUiaProvider
                                : null;
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            /// <summary>
            ///  Gets the top level element.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _owningComboBox.AccessibilityObject;

            public override string Name => base.Name ?? SR.ComboBoxEditDefaultAccessibleName;

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => _owningComboBox.Focused,
                    UiaCore.UIA.IsEnabledPropertyId => _owningComboBox.Enabled,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => (State & AccessibleStates.Focusable) == AccessibleStates.Focusable,
                    UiaCore.UIA.IsOffscreenPropertyId => false,
                    UiaCore.UIA.NativeWindowHandlePropertyId => _handle,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
            {
                get
                {
                    UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _handle), out UiaCore.IRawElementProviderSimple provider);
                    return provider;
                }
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UiaCore.UIA patternId) =>
                patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    UiaCore.UIA.TextPatternId => true,
                    UiaCore.UIA.TextPattern2Id => true,
                    _ => base.IsPatternSupported(patternId)
                };

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId => new int[] { RuntimeIDFirstItem, GetHashCode() };

            internal override UiaCore.ITextRangeProvider DocumentRangeInternal
                => _textProvider.DocumentRange;

            internal override UiaCore.ITextRangeProvider[]? GetTextSelection()
                => _textProvider.GetSelection();

            internal override UiaCore.ITextRangeProvider[]? GetTextVisibleRanges()
                => _textProvider.GetVisibleRanges();

            internal override UiaCore.ITextRangeProvider? GetTextRangeFromChild(UiaCore.IRawElementProviderSimple childElement)
                => _textProvider.RangeFromChild(childElement);

            internal override UiaCore.ITextRangeProvider? GetTextRangeFromPoint(Point screenLocation)
                => _textProvider.RangeFromPoint(screenLocation);

            internal override UiaCore.SupportedTextSelection SupportedTextSelectionInternal
                => _textProvider.SupportedTextSelection;

            internal override UiaCore.ITextRangeProvider? GetTextCaretRange(out BOOL isActive)
                => _textProvider.GetCaretRange(out isActive);

            internal override UiaCore.ITextRangeProvider GetRangeFromAnnotation(UiaCore.IRawElementProviderSimple annotationElement)
                => _textProvider.RangeFromAnnotation(annotationElement);
        }
    }
}
