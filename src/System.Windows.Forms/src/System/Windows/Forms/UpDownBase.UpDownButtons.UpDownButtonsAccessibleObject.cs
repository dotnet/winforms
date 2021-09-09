// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class UpDownBase
    {
        internal partial class UpDownButtons
        {
            internal partial class UpDownButtonsAccessibleObject : ControlAccessibleObject
            {
                private DirectionButtonAccessibleObject upButton;
                private DirectionButtonAccessibleObject downButton;

                private UpDownButtons _owner;

                public UpDownButtonsAccessibleObject(UpDownButtons owner) : base(owner)
                {
                    _owner = owner;
                }

                internal override UiaCore.IRawElementProviderFragment ElementProviderFromPoint(double x, double y)
                {
                    AccessibleObject element = HitTest((int)x, (int)y);

                    if (element is not null)
                    {
                        return element;
                    }

                    return base.ElementProviderFromPoint(x, y);
                }

                internal override UiaCore.IRawElementProviderFragment FragmentNavigate(
                    UiaCore.NavigateDirection direction) => direction switch
                    {
                        UiaCore.NavigateDirection.FirstChild => GetChild(0),
                        UiaCore.NavigateDirection.LastChild => GetChild(1),
                        _ => base.FragmentNavigate(direction),
                    };

                internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

                private DirectionButtonAccessibleObject UpButton
                    => upButton ??= new DirectionButtonAccessibleObject(this, true);

                private DirectionButtonAccessibleObject DownButton
                    => downButton ??= new DirectionButtonAccessibleObject(this, false);

                public override AccessibleObject GetChild(int index) => index switch
                {
                    0 => UpButton,
                    1 => DownButton,
                    _ => null,
                };

                public override int GetChildCount() => 2;

                internal override object GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
                {
                    UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                    UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                    _ => base.GetPropertyValue(propertyID),
                };

                public override AccessibleObject HitTest(int x, int y)
                {
                    if (UpButton.Bounds.Contains(x, y))
                    {
                        return UpButton;
                    }

                    if (DownButton.Bounds.Contains(x, y))
                    {
                        return DownButton;
                    }

                    return null;
                }

                internal override UiaCore.IRawElementProviderSimple HostRawElementProvider
                {
                    get
                    {
                        if (HandleInternal == IntPtr.Zero)
                        {
                            return null;
                        }

                        UiaCore.UiaHostProviderFromHwnd(new HandleRef(this, HandleInternal), out UiaCore.IRawElementProviderSimple provider);
                        return provider;
                    }
                }

                public override string Name
                {
                    get
                    {
                        string baseName = base.Name;
                        if (string.IsNullOrEmpty(baseName))
                        {
                            return SR.DefaultUpDownButtonsAccessibleName;
                        }

                        return baseName;
                    }
                    set => base.Name = value;
                }

                public override AccessibleObject Parent => _owner.AccessibilityObject;

                public override AccessibleRole Role
                {
                    get
                    {
                        AccessibleRole role = Owner.AccessibleRole;
                        if (role != AccessibleRole.Default)
                        {
                            return role;
                        }

                        return AccessibleRole.SpinButton;
                    }
                }

                /// <summary>
                ///  Gets the runtime ID. We need to provide a unique ID others are implementing this in the same manner first item
                ///  is static - 0x2a (RuntimeIDFirstItem) second item can be anything, but it's good to supply HWND.
                /// </summary>
                internal override int[] RuntimeId
                    => new int[]
                    {
                        RuntimeIDFirstItem,
                        PARAM.ToInt(_owner.InternalHandle),
                        _owner.GetHashCode()
                    };
            }
        }
    }
}
