// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ErrorProvider
    {
        internal partial class ErrorWindow
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
                internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
                {
                    AccessibleObject? element = HitTest((int)x, (int)y);

                    if (element is not null)
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
                internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
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

                public override AccessibleObject? GetChild(int index)
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
                internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                    propertyID switch
                    {
                        UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.GroupControlTypeId,
                        UiaCore.UIA.NativeWindowHandlePropertyId => _owner.Handle,
                        _ => base.GetPropertyValue(propertyID)
                    };

                public override AccessibleObject? HitTest(int x, int y)
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

                internal override bool IsIAccessibleExSupported() => true;

                internal override bool IsPatternSupported(UiaCore.UIA patternId)
                {
                    if (patternId == UiaCore.UIA.LegacyIAccessiblePatternId)
                    {
                        return true;
                    }

                    return base.IsPatternSupported(patternId);
                }

                internal override bool IsReadOnly => true;

                [AllowNull]
                public override string Name
                {
                    get => string.IsNullOrEmpty(base.Name) ? SR.ErrorProviderDefaultAccessibleName : base.Name;
                    set => base.Name = value;
                }

                public override AccessibleRole Role => AccessibleRole.Grouping;

                // We need to provide a unique ID. Others are implementing this in the same manner. First item is static - 0x2a (RuntimeIDFirstItem).
                // Second item can be anything, but it's good to supply HWND.
                internal override int[] RuntimeId
                    => new int[]
                    {
                        RuntimeIDFirstItem,
                        PARAM.ToInt(_owner.Handle),
                        _owner.GetHashCode()
                    };

                public override AccessibleStates State => AccessibleStates.ReadOnly;
            }
        }
    }
}
