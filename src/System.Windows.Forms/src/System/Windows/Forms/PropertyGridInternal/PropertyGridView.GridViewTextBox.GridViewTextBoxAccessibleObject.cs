// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static System.Windows.Forms.PropertyGridInternal.PropertyDescriptorGridEntry;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private partial class GridViewTextBox
        {
            private class GridViewTextBoxAccessibleObject : TextBoxBaseAccessibleObject
            {
                private readonly PropertyGridView _owningPropertyGridView;

                public GridViewTextBoxAccessibleObject(GridViewTextBox owner) : base(owner)
                {
                    _owningPropertyGridView = owner.PropertyGridView;
                }

                public override AccessibleStates State
                {
                    get
                    {
                        AccessibleStates states = base.State;
                        if (IsReadOnly)
                        {
                            states |= AccessibleStates.ReadOnly;
                        }
                        else
                        {
                            states &= ~AccessibleStates.ReadOnly;
                        }

                        return states;
                    }
                }

                /// <summary>
                ///  Returns the element in the specified direction.
                /// </summary>
                /// <param name="direction">Indicates the direction in which to navigate.</param>
                /// <returns>Returns the element in the specified direction.</returns>
                internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                {
                    if (!_owningPropertyGridView.IsEditTextBoxCreated
                        || _owningPropertyGridView.SelectedGridEntry?.AccessibilityObject is not PropertyDescriptorGridEntryAccessibleObject parent)
                    {
                        return null;
                    }

                    return direction switch
                    {
                        UiaCore.NavigateDirection.Parent => parent,
                        UiaCore.NavigateDirection.NextSibling => parent.GetNextChild(this),
                        UiaCore.NavigateDirection.PreviousSibling => parent.GetPreviousChild(this),
                        _ => base.FragmentNavigate(direction),
                    };
                }

                /// <summary>
                ///  Gets the top level element.
                /// </summary>
                internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
                    => _owningPropertyGridView.OwnerGrid?.AccessibilityObject;

                internal override object? GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
                {
                    UiaCore.UIA.ClassNamePropertyId => Owner.GetType().ToString(),
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.EditControlTypeId,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => Owner.Focused,
                    UiaCore.UIA.IsEnabledPropertyId => !IsReadOnly,
                    _ => base.GetPropertyValue(propertyID)
                };

                internal override bool IsPatternSupported(UiaCore.UIA patternId) => patternId switch
                {
                    UiaCore.UIA.ValuePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };

                internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
                {
                    get
                    {
                        // Prevent sending same runtime ID for all edit boxes. Individual edit in
                        // each row should have unique runtime ID to prevent incorrect announcement.
                        // For instance screen reader may announce row 2 for the third row edit
                        // as the same TextBox control is used both in row 2 and row 3.
                        return null;
                    }
                }

                public override string? Name
                {
                    get
                    {
                        string? name = Owner.AccessibleName;
                        if (name is not null)
                        {
                            return name;
                        }
                        else
                        {
                            GridEntry selectedGridEntry = _owningPropertyGridView.SelectedGridEntry;
                            if (selectedGridEntry is not null)
                            {
                                return selectedGridEntry.AccessibilityObject.Name;
                            }
                        }

                        return base.Name;
                    }
                    set => base.Name = value;
                }

                internal override int[] RuntimeId
                    => new int[]
                    {
                        RuntimeIDFirstItem,
                        PARAM.ToInt(Owner.InternalHandle),
                        GetHashCode()
                    };

                internal override bool IsReadOnly
                    => _owningPropertyGridView.SelectedGridEntry is not PropertyDescriptorGridEntry propertyDescriptorGridEntry
                        || propertyDescriptorGridEntry.IsPropertyReadOnly;

                internal override void SetFocus()
                {
                    RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                    base.SetFocus();
                }
            }
        }
    }
}
