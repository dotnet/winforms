// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public abstract partial class UpDownBase
    {
        internal partial class UpDownButtons
        {
            internal partial class UpDownButtonsAccessibleObject : ControlAccessibleObject
            {
                internal class DirectionButtonAccessibleObject : AccessibleObject
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
                            if (!_parent.Owner.IsHandleCreated)
                            {
                                return Rectangle.Empty;
                            }

                            // Get button bounds
                            Rectangle bounds = ((UpDownButtons)_parent.Owner).Bounds;
                            bounds.Height /= 2;

                            if (!_up)
                            {
                                bounds.Y += bounds.Height;
                            }

                            // Convert to screen coords
                            return ((UpDownButtons)_parent.Owner).ParentInternal?.RectangleToScreen(bounds) ?? Rectangle.Empty;
                        }
                    }

                    public override string DefaultAction => SR.AccessibleActionPress;

                    public override void DoDefaultAction()
                    {
                        UpDownButtons ownerControl = _parent._owner;
                        if (!ownerControl.IsHandleCreated)
                        {
                            return;
                        }

                        int buttonId = _up ? (int)ButtonID.Up : (int)ButtonID.Down;
                        ownerControl.OnUpDown(new UpDownEventArgs(buttonId));
                    }

                    internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(
                        UiaCore.NavigateDirection direction) => direction switch
                        {
                            UiaCore.NavigateDirection.Parent => Parent,
                            UiaCore.NavigateDirection.NextSibling => _up ? Parent.GetChild(1) : null,
                            UiaCore.NavigateDirection.PreviousSibling => _up ? null : Parent.GetChild(0),
                            _ => base.FragmentNavigate(direction),
                        };

                    internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => Parent;

                    internal override object? GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
                    {
                        UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                        _ => base.GetPropertyValue(propertyID),
                    };

                    internal override bool IsPatternSupported(UiaCore.UIA patternId)
                    {
                        return patternId == UiaCore.UIA.LegacyIAccessiblePatternId ||
                            patternId == UiaCore.UIA.InvokePatternId ||
                            base.IsPatternSupported(patternId);
                    }

                    [AllowNull]
                    public override string Name
                    {
                        get => _up ? SR.UpDownBaseUpButtonAccName : SR.UpDownBaseDownButtonAccName;
                        set { }
                    }

                    public override AccessibleObject Parent => _parent;

                    public override AccessibleRole Role => AccessibleRole.PushButton;

                    /// <summary>
                    ///  Gets the runtime ID.
                    /// </summary>
                    internal override int[] RuntimeId
                        => new int[]
                        {
                            _parent.RuntimeId[0],
                            _parent.RuntimeId[1],
                            _parent.RuntimeId[2],
                            _up ? 1 : 0
                        };
                }
            }
        }
    }
}
