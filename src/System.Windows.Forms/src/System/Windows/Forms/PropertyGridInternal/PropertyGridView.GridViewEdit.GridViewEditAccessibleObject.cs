// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal partial class PropertyGridView
    {
        private partial class GridViewEdit
        {
            protected class GridViewEditAccessibleObject : ControlAccessibleObject
            {
                private readonly PropertyGridView propertyGridView;

                public GridViewEditAccessibleObject(GridViewEdit owner) : base(owner)
                {
                    propertyGridView = owner.psheet;
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
                internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
                {
                    if (direction == UiaCore.NavigateDirection.Parent && propertyGridView.SelectedGridEntry != null)
                    {
                        return propertyGridView.SelectedGridEntry.AccessibilityObject;
                    }
                    else if (direction == UiaCore.NavigateDirection.NextSibling)
                    {
                        if (propertyGridView.DropDownButton.Visible)
                        {
                            return propertyGridView.DropDownButton.AccessibilityObject;
                        }
                        else if (propertyGridView.DialogButton.Visible)
                        {
                            return propertyGridView.DialogButton.AccessibilityObject;
                        }
                    }
                    else if (direction == UiaCore.NavigateDirection.PreviousSibling)
                    {
                        if (propertyGridView.DropDownVisible)
                        {
                            return propertyGridView.DropDownControlHolder.AccessibilityObject;
                        }
                    }

                    return base.FragmentNavigate(direction);
                }

                /// <summary>
                ///  Gets the top level element.
                /// </summary>
                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                    => propertyGridView.AccessibilityObject;

                internal override object GetPropertyValue(UiaCore.UIA propertyID)
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
                    }

                    return base.GetPropertyValue(propertyID);
                }

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                {
                    if (patternId == UiaCore.UIA.ValuePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }

                internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
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

                public override string Name
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
                            GridEntry selectedGridEntry = propertyGridView.SelectedGridEntry;
                            if (selectedGridEntry != null)
                            {
                                return selectedGridEntry.AccessibilityObject.Name;
                            }
                        }

                        return base.Name;
                    }
                    set => base.Name = value;
                }

                internal override int[] RuntimeId
                {
                    get
                    {
                        var selectedGridEntryAccessibleRuntimeId =
                            propertyGridView?.SelectedGridEntry?.AccessibilityObject?.RuntimeId;

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
                        return !(propertyGridView.SelectedGridEntry is PropertyDescriptorGridEntry propertyDescriptorGridEntry) || propertyDescriptorGridEntry.IsPropertyReadOnly;
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
