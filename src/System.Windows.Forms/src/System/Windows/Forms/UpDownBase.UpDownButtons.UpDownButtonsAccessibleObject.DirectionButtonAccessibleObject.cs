// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

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
                        _parent = parent;
                        _up = up;
                    }

                    /// <summary>
                    ///  Gets the runtime ID.
                    /// </summary>
                    internal override int[] RuntimeId => new int[]
                    {
                        _parent.RuntimeId[0],
                        _parent.RuntimeId[1],
                        _parent.RuntimeId[2],
                        _up ? 1 : 0
                    };

                    internal override object GetPropertyValue(UiaCore.UIA propertyID) => propertyID switch
                    {
                        UiaCore.UIA.NamePropertyId => Name,
                        UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                        UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                        UiaCore.UIA.BoundingRectanglePropertyId => Bounds,
                        UiaCore.UIA.LegacyIAccessibleStatePropertyId => State,
                        UiaCore.UIA.LegacyIAccessibleRolePropertyId => Role,
                        _ => base.GetPropertyValue(propertyID),
                    };

                    internal override UiaCore.IRawElementProviderFragment FragmentNavigate(
                        UiaCore.NavigateDirection direction) => direction switch
                        {
                            UiaCore.NavigateDirection.Parent => Parent,
                            UiaCore.NavigateDirection.NextSibling => _up ? Parent.GetChild(1) : null,
                            UiaCore.NavigateDirection.PreviousSibling => _up ? null : Parent.GetChild(0),
                            _ => base.FragmentNavigate(direction),
                        };

                    internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => Parent;

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
                            return (((UpDownButtons)_parent.Owner).ParentInternal).RectangleToScreen(bounds);
                        }
                    }

                    public override string Name
                    {
                        get => _up ? SR.UpDownBaseUpButtonAccName : SR.UpDownBaseDownButtonAccName;
                        set { }
                    }

                    public override AccessibleObject Parent => _parent;

                    public override AccessibleRole Role => AccessibleRole.PushButton;
                }
            }
        }
    }
}
