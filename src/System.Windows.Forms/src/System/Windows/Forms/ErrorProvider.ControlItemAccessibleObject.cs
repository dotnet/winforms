// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    partial class ErrorProvider
    {
        private class ControlItemAccessibleObject : AccessibleObject
        {
            private readonly ControlItem _controlItem;
            private readonly ErrorWindow _window;
            private readonly Control _control;
            private readonly ErrorProvider _provider;

            public ControlItemAccessibleObject(ControlItem controlItem, ErrorWindow window, Control control, ErrorProvider provider)
            {
                _controlItem = controlItem;
                _window = window;
                _control = control;
                _provider = provider;
            }

            internal override Rectangle BoundingRectangle => Bounds;

            public override Rectangle Bounds => _control.IsHandleCreated ?
                                                _control.RectangleToScreen(_controlItem.GetIconBounds(_provider.Region.Size)) : Rectangle.Empty;

            private int ControlItemsCount => _window.ControlItems.Count;

            private int CurrentIndex => _window.ControlItems.IndexOf(_controlItem);

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return Parent;
                    case UiaCore.NavigateDirection.NextSibling:
                        return GetNextSibling();
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return GetPreviousSibling();
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => Parent;

            internal override int GetChildId()
            {
                return CurrentIndex + 1;
            }

            private AccessibleObject GetNextSibling()
            {
                int currentIndex = CurrentIndex;

                //Is a last child
                if (currentIndex >= ControlItemsCount - 1 || currentIndex < 0)
                {
                    return null;
                }

                return _window.ControlItems[currentIndex + 1].AccessibilityObject;
            }

            private AccessibleObject GetPreviousSibling()
            {
                int currentIndex = CurrentIndex;

                //Is a first child
                if (currentIndex <= 0 || currentIndex > ControlItemsCount - 1)
                {
                    return null;
                }

                return _window.ControlItems[currentIndex - 1].AccessibilityObject;
            }

            /// <summary>
            ///  Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.RuntimeIdPropertyId:
                        return RuntimeId;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ImageControlTypeId;
                    case UiaCore.UIA.BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help;
                    case UiaCore.UIA.LegacyIAccessibleStatePropertyId:
                        return State;
                    case UiaCore.UIA.NativeWindowHandlePropertyId:
                        return _window.Handle;
                    case UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_controlItem != null)
                {
                    return true;
                }

                return base.IsIAccessibleExSupported();
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override bool IsReadOnly => true;

            public override string Name
            {
                get => string.IsNullOrEmpty(base.Name) ? _controlItem.Error : base.Name;
                set => base.Name = value;
            }

            public override AccessibleObject Parent => _window.AccessibilityObject;

            public override AccessibleRole Role => AccessibleRole.Alert;

            /// <summary>
            ///  Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId
            {
                get
                {
                    var runtimeId = new int[4];

                    runtimeId[0] = _window.AccessibilityObject.RuntimeId[0];
                    runtimeId[1] = _window.AccessibilityObject.RuntimeId[1];
                    runtimeId[2] = _window.AccessibilityObject.RuntimeId[2];
                    runtimeId[3] = _controlItem.GetHashCode();

                    return runtimeId;
                }
            }

            public override AccessibleStates State => AccessibleStates.HasPopup | AccessibleStates.ReadOnly;
        }
    }
}
