// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal partial class UpDownButtons
    {
        internal sealed partial class UpDownButtonsAccessibleObject : ControlAccessibleObject
        {
            private int[]? _runtimeId;

            private DirectionButtonAccessibleObject? _upButton;
            private DirectionButtonAccessibleObject? _downButton;

            public UpDownButtonsAccessibleObject(UpDownButtons owner) : base(owner)
            {
            }

            internal override IRawElementProviderFragment.Interface? ElementProviderFromPoint(double x, double y)
            {
                AccessibleObject? element = HitTest((int)x, (int)y);

                if (element is not null)
                {
                    return element;
                }

                return base.ElementProviderFromPoint(x, y);
            }

            internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
                => direction switch
                {
                    NavigateDirection.NavigateDirection_FirstChild => GetChild(0),
                    NavigateDirection.NavigateDirection_LastChild => GetChild(1),
                    _ => base.FragmentNavigate(direction),
                };

            internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => this;

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

            [AllowNull]
            public override string Name
            {
                get
                {
                    string? baseName = base.Name;
                    return string.IsNullOrEmpty(baseName) ? SR.DefaultUpDownButtonsAccessibleName : baseName;
                }
            }

            public override AccessibleObject? Parent
                => this.TryGetOwnerAs(out UpDownButtons? owner) ? owner.AccessibilityObject : null;

            private protected override bool IsInternal => true;

            internal void ReleaseChildUiaProviders()
            {
                PInvoke.UiaDisconnectProvider(_upButton, skipOSCheck: true);
                _upButton = null;

                PInvoke.UiaDisconnectProvider(_downButton, skipOSCheck: true);
                _downButton = null;
            }

            public override AccessibleRole Role => this.GetOwnerAccessibleRole(AccessibleRole.SpinButton);

            internal override int[] RuntimeId => _runtimeId ??= base.RuntimeId;
        }
    }
}
