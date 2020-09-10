// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private partial class GridViewEdit
        {
            protected class GridViewEditAccessibleObject : ControlAccessibleObject
            {
                private readonly PropertyGridView _owningPropertyGridView;
                private readonly TextBoxBaseUiaTextProvider _textProvider;
                private readonly GridViewEdit _owningGridViewEdit;

                public GridViewEditAccessibleObject(GridViewEdit owner) : base(owner)
                {
                    _owningPropertyGridView = owner.psheet;
                    _owningGridViewEdit = owner;
                    _textProvider = new TextBoxBaseUiaTextProvider(owner);
                    UseTextProviders(_textProvider, _textProvider);
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

                internal override bool IsIAccessibleExSupported() => true;

                /// <summary>
                ///  Returns the element in the specified direction.
                /// </summary>
                /// <param name="direction">Indicates the direction in which to navigate.</param>
                /// <returns>Returns the element in the specified direction.</returns>
                internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                {
                    if (direction == UiaCore.NavigateDirection.Parent && _owningPropertyGridView.SelectedGridEntry != null)
                    {
                        return _owningPropertyGridView.SelectedGridEntry.AccessibilityObject;
                    }
                    else if (direction == UiaCore.NavigateDirection.NextSibling)
                    {
                        if (_owningPropertyGridView.DropDownButton.Visible)
                        {
                            return _owningPropertyGridView.DropDownButton.AccessibilityObject;
                        }
                        else if (_owningPropertyGridView.DialogButton.Visible)
                        {
                            return _owningPropertyGridView.DialogButton.AccessibilityObject;
                        }
                    }
                    else if (direction == UiaCore.NavigateDirection.PreviousSibling)
                    {
                        if (_owningPropertyGridView.DropDownVisible)
                        {
                            return _owningPropertyGridView.DropDownControlHolder.AccessibilityObject;
                        }
                    }

                    return base.FragmentNavigate(direction);
                }

                /// <summary>
                ///  Gets the top level element.
                /// </summary>
                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                    => _owningPropertyGridView.AccessibilityObject;

                internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                {
                    switch (propertyID)
                    {
                        case UiaCore.UIA.RuntimeIdPropertyId:
                            return RuntimeId;
                        case UiaCore.UIA.ControlTypePropertyId:
                            return UiaCore.UIA.EditControlTypeId;
                        case UiaCore.UIA.NamePropertyId:
                            return Name;
                        case UiaCore.UIA.HasKeyboardFocusPropertyId:
                            return Owner.Focused;
                        case UiaCore.UIA.IsEnabledPropertyId:
                            return !IsReadOnly;
                        case UiaCore.UIA.ClassNamePropertyId:
                            return Owner.GetType().ToString();
                        case UiaCore.UIA.FrameworkIdPropertyId:
                            return NativeMethods.WinFormFrameworkId;
                        case UiaCore.UIA.IsValuePatternAvailablePropertyId:
                            return IsPatternSupported(UiaCore.UIA.ValuePatternId);
                        case UiaCore.UIA.IsTextPatternAvailablePropertyId:
                            return IsPatternSupported(UiaCore.UIA.TextPatternId);
                        case UiaCore.UIA.IsTextPattern2AvailablePropertyId:
                            return IsPatternSupported(UiaCore.UIA.TextPattern2Id);
                    }

                    return base.GetPropertyValue(propertyID);
                }

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                    => patternId switch
                    {
                        UiaCore.UIA.ValuePatternId => true,
                        UiaCore.UIA.TextPatternId => true,
                        UiaCore.UIA.TextPattern2Id => true,
                        _ => base.IsPatternSupported(patternId)
                    };

                internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
                {
                    get
                    {
                        // Prevent sending same runtime ID for all edit boxes. Individual edit in
                        // each row should have unique runtime ID to prevent incorrect announcement.
                        // For instance screen reader may announce row 2 for the third row edit
                        // as the sme TextBox control is used both in row 2 and row 3.
                        return null;
                    }
                }

                public override string? Name
                {
                    get
                    {
                        string name = Owner.AccessibleName;
                        if (name != null)
                        {
                            return name;
                        }
                        else
                        {
                            GridEntry selectedGridEntry = _owningPropertyGridView.SelectedGridEntry;
                            if (selectedGridEntry != null)
                            {
                                return selectedGridEntry.AccessibilityObject.Name;
                            }
                        }

                        return base.Name;
                    }
                    set => base.Name = value;
                }

                internal override int[]? RuntimeId
                {
                    get
                    {
                        var selectedGridEntryAccessibleRuntimeId =
                            _owningPropertyGridView?.SelectedGridEntry?.AccessibilityObject?.RuntimeId;

                        if (selectedGridEntryAccessibleRuntimeId is null)
                        {
                            return null;
                        }

                        int[] runtimeId = new int[selectedGridEntryAccessibleRuntimeId.Length + 1];
                        for (int i = 0; i < selectedGridEntryAccessibleRuntimeId.Length; i++)
                        {
                            runtimeId[i] = selectedGridEntryAccessibleRuntimeId[i];
                        }

                        runtimeId[runtimeId.Length - 1] = 1;

                        return runtimeId;
                    }
                }

                #region IValueProvider

                internal override bool IsReadOnly
                {
                    get
                    {
                        return !(_owningPropertyGridView.SelectedGridEntry is PropertyDescriptorGridEntry propertyDescriptorGridEntry) || propertyDescriptorGridEntry.IsPropertyReadOnly;
                    }
                }

                #endregion

                internal override void SetFocus()
                {
                    RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);

                    base.SetFocus();
                }
            }
        }
    }
}
