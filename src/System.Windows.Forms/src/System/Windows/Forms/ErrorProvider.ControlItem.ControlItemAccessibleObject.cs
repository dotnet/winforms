﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.UI.Accessibility;
using static Interop;

namespace System.Windows.Forms;

public partial class ErrorProvider
{
    internal partial class ControlItem
    {
        private class ControlItemAccessibleObject : AccessibleObject
        {
            private readonly ControlItem _controlItem;
            private readonly ErrorWindow? _window;
            private readonly Control _control;
            private readonly ErrorProvider _provider;

            public ControlItemAccessibleObject(
                ControlItem controlItem,
                ErrorWindow? window,
                Control control,
                ErrorProvider provider)
            {
                _controlItem = controlItem;
                _window = window;
                _control = control;
                _provider = provider;
            }

            public override Rectangle Bounds
                => _control.ParentInternal is not null && _control.ParentInternal.IsHandleCreated
                    ? _control.ParentInternal.RectangleToScreen(_controlItem.GetIconBounds(_provider.Region.Size))
                    : Rectangle.Empty;

            private int ControlItemsCount => _window?.ControlItems.Count ?? 0;

            private int CurrentIndex => _window?.ControlItems.IndexOf(_controlItem) ?? -1;

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            {
                switch (direction)
                {
                    case NavigateDirection.NavigateDirection_Parent:
                        return Parent;
                    case NavigateDirection.NavigateDirection_NextSibling:
                        return GetNextSibling();
                    case NavigateDirection.NavigateDirection_PreviousSibling:
                        return GetPreviousSibling();
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot => Parent;

            internal override int GetChildId()
            {
                return CurrentIndex + 1;
            }

            private AccessibleObject? GetNextSibling()
            {
                int currentIndex = CurrentIndex;

                //Is a last child
                if (currentIndex >= ControlItemsCount - 1 || currentIndex < 0)
                {
                    return null;
                }

                return _window?.ControlItems[currentIndex + 1].AccessibilityObject;
            }

            private AccessibleObject? GetPreviousSibling()
            {
                int currentIndex = CurrentIndex;

                //Is a first child
                if (currentIndex <= 0 || currentIndex > ControlItemsCount - 1)
                {
                    return null;
                }

                return _window?.ControlItems[currentIndex - 1].AccessibilityObject;
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
                propertyID switch
                {
                    UIA_PROPERTY_ID.UIA_ControlTypePropertyId => UIA_CONTROLTYPE_ID.UIA_ImageControlTypeId,
                    UIA_PROPERTY_ID.UIA_NativeWindowHandlePropertyId => _window?.Handle,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsIAccessibleExSupported() => true;

            internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            {
                if (patternId == UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override bool IsReadOnly => true;

            [AllowNull]
            public override string Name
            {
                get => string.IsNullOrEmpty(base.Name) ? _controlItem.Error : base.Name;
                set => base.Name = value;
            }

            public override AccessibleObject? Parent => _window?.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.Alert;

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    if (_window is null)
                    {
                        return new int[]
                        {
                            _controlItem.GetHashCode()
                        };
                    }

                    Debug.Assert(_window.AccessibilityObject.RuntimeId.Length >= 3);

                    return new int[]
                    {
                        _window.AccessibilityObject.RuntimeId[0],
                        _window.AccessibilityObject.RuntimeId[1],
                        _window.AccessibilityObject.RuntimeId[2],
                        _controlItem.GetHashCode()
                    };
                }
            }

            public override AccessibleStates State => AccessibleStates.HasPopup | AccessibleStates.ReadOnly;
        }
    }
}
