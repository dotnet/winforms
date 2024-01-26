// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public abstract partial class UpDownBase
{
    internal partial class UpDownButtons
    {
        internal partial class UpDownButtonsAccessibleObject : ControlAccessibleObject
        {
            internal sealed class DirectionButtonAccessibleObject : AccessibleObject
            {
                private readonly bool _up;
                private readonly UpDownButtonsAccessibleObject _parent;

                public DirectionButtonAccessibleObject(UpDownButtonsAccessibleObject parent, bool up)
                {
                    _parent = parent.OrThrowIfNull();
                    _up = up;
                }

                public override Rectangle Bounds
                {
                    get
                    {
                        if (!_parent.IsOwnerHandleCreated(out UpDownButtons? owner))
                        {
                            return Rectangle.Empty;
                        }

                        // Get button bounds
                        Rectangle bounds = owner.Bounds;
                        bounds.Height /= 2;

                        if (!_up)
                        {
                            bounds.Y += bounds.Height;
                        }

                        // Convert to screen coords
                        return owner.ParentInternal?.RectangleToScreen(bounds) ?? Rectangle.Empty;
                    }
                }

                public override string DefaultAction => SR.AccessibleActionPress;

                internal override bool CanGetDefaultActionInternal => false;

                public override void DoDefaultAction()
                {
                    if (!_parent.IsOwnerHandleCreated(out UpDownButtons? owner))
                    {
                        return;
                    }

                    int buttonId = _up ? (int)ButtonID.Up : (int)ButtonID.Down;
                    owner.OnUpDown(new UpDownEventArgs(buttonId));
                }

                internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
                    => direction switch
                    {
                        NavigateDirection.NavigateDirection_Parent => Parent,
                        NavigateDirection.NavigateDirection_NextSibling => _up ? Parent.GetChild(1) : null,
                        NavigateDirection.NavigateDirection_PreviousSibling => _up ? null : Parent.GetChild(0),
                        _ => base.FragmentNavigate(direction),
                    };

                internal override IRawElementProviderFragmentRoot.Interface FragmentRoot => Parent;

                internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) => propertyID switch
                {
                    UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ButtonControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

                internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
                {
                    return patternId == UIA_PATTERN_ID.UIA_LegacyIAccessiblePatternId ||
                        patternId == UIA_PATTERN_ID.UIA_InvokePatternId ||
                        base.IsPatternSupported(patternId);
                }

                [AllowNull]
                public override string Name
                {
                    get => _up ? SR.UpDownBaseUpButtonAccName : SR.UpDownBaseDownButtonAccName;
                    set { }
                }

                internal override bool CanGetNameInternal => false;

                internal override bool CanSetNameInternal => false;

                public override AccessibleObject Parent => _parent;

                private protected override bool IsInternal => true;

                public override AccessibleRole Role => AccessibleRole.PushButton;

                internal override int[] RuntimeId
                {
                    get
                    {
                        int[] id = _parent.RuntimeId;
                        return [id[0], id[1], id[2], _up ? 1 : 0];
                    }
                }
            }
        }
    }
}
