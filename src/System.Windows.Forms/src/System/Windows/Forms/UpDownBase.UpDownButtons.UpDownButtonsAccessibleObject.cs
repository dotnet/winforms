﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal partial class UpDownButtons
    {
        internal partial class UpDownButtonsAccessibleObject : ControlAccessibleObject
        {
            private int[]? _runtimeId;

            private DirectionButtonAccessibleObject? _upButton;
            private DirectionButtonAccessibleObject? _downButton;

            public UpDownButtonsAccessibleObject(UpDownButtons owner) : base(owner)
            {
            }

            internal override UiaCore.IRawElementProviderFragment? ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject? element = HitTest((int)x, (int)y);

                if (element is not null)
                {
                    return element;
                }

                return base.ElementProviderFromPoint(x, y);
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(
                UiaCore.NavigateDirection direction) => direction switch
                {
                    UiaCore.NavigateDirection.FirstChild => GetChild(0),
                    UiaCore.NavigateDirection.LastChild => GetChild(1),
                    _ => base.FragmentNavigate(direction),
                };

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => this;

            private DirectionButtonAccessibleObject UpButton
                => _upButton ??= new DirectionButtonAccessibleObject(this, true);

            private DirectionButtonAccessibleObject DownButton
                => _downButton ??= new DirectionButtonAccessibleObject(this, false);

            public override AccessibleObject? GetChild(int index) => index switch
            {
                0 => UpButton,
                1 => DownButton,
                _ => null,
            };

            public override int GetChildCount() => 2;

            public override AccessibleObject? HitTest(int x, int y)
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

            internal override UiaCore.IRawElementProviderSimple? HostRawElementProvider
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

            [AllowNull]
            public override string Name
            {
                get
                {
                    string? baseName = base.Name;
                    return string.IsNullOrEmpty(baseName) ? SR.DefaultUpDownButtonsAccessibleName : baseName;
                }
                set => base.Name = value;
            }

            public override AccessibleObject? Parent
                => this.TryGetOwnerAs(out UpDownButtons? owner) ? owner.AccessibilityObject : null;

            internal void ReleaseChildUiaProviders()
            {
                UiaCore.UiaDisconnectProvider(_upButton);
                _upButton = null;

                UiaCore.UiaDisconnectProvider(_downButton);
                _downButton = null;
            }

            public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.SpinButton);

            internal override int[] RuntimeId
                => _runtimeId ??= !this.TryGetOwnerAs(out UpDownButtons? owner) ? base.RuntimeId : new int[]
                {
                    RuntimeIDFirstItem,
                    PARAM.ToInt(owner.InternalHandle),
                    owner.GetHashCode()
                };
        }
    }
}
