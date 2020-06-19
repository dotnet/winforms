// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    partial class ErrorProvider
    {
        private class ErrorWindowAccessibleObject : AccessibleObject
        {
            private readonly ErrorWindow _owner;

            public ErrorWindowAccessibleObject(ErrorWindow owner)
            {
                _owner = owner;
            }

            /// <summary>
            ///  Return the child object at the given screen coordinates.
            /// </summary>
            /// <param name="x">X coordinate.</param>
            /// <param name="y">Y coordinate.</param>
            /// <returns>The accessible object of corresponding element in the provided coordinates.</returns>
            internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject element = HitTest((int)x, (int)y);

                if (element != null)
                {
                    return element;
                }

                return base.ElementProviderFromPoint(x, y);
            }

            /// <summary>
            ///  Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.FirstChild:
                        return GetChild(0);
                    case UiaCore.NavigateDirection.LastChild:
                        return GetChild(GetChildCount() - 1);
                }

                return base.FragmentNavigate(direction);
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            public override AccessibleObject GetChild(int index)
            {
                if (index >= 0 && index <= GetChildCount() - 1)
                {
                    return _owner.ControlItems[index].AccessibilityObject;
                }

                return base.GetChild(index);
            }

            public override int GetChildCount() => _owner.ControlItems.Count;

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
                        return UiaCore.UIA.GroupControlTypeId;
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help;
                    case UiaCore.UIA.LegacyIAccessibleStatePropertyId:
                        return State;
                    case UiaCore.UIA.NativeWindowHandlePropertyId:
                        return _owner.Handle;
                    case UiaCore.UIA.IsLegacyIAccessiblePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.LegacyIAccessiblePatternId);
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            public override AccessibleObject HitTest(int x, int y)
            {
                foreach (ControlItem control in _owner.ControlItems)
                {
                    if (control.AccessibilityObject.Bounds.Contains(x, y))
                    {
                        return control.AccessibilityObject;
                    }
                }

                return null;
            }

            internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
            {
                get
                {
                    UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, _owner.Handle), out UiaCore.IRawElementProviderSimple provider);
                    return provider;
                }
            }

            internal override bool IsIAccessibleExSupported()
            {
                if (_owner != null)
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
                get => string.IsNullOrEmpty(base.Name) ? SR.ErrorProviderDefaultAccessibleName : base.Name;
                set => base.Name = value;
            }

            public override AccessibleRole Role => AccessibleRole.Grouping;

            internal override int[] RuntimeId
            {
                get
                {
                    if (_owner is null)
                    {
                        return base.RuntimeId;
                    }

                    // we need to provide a unique ID
                    // others are implementing this in the same manner
                    // first item is static - 0x2a (RuntimeIDFirstItem)
                    // second item can be anything, but here it is a hash

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owner.Handle;
                    runtimeId[2] = _owner.GetHashCode();

                    return runtimeId;
                }
            }

            public override AccessibleStates State => AccessibleStates.ReadOnly;
        }
    }
}
