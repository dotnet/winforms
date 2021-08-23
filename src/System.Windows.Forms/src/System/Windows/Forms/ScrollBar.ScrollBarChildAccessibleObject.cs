﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ScrollBar
    {
        internal class ScrollBarChildAccessibleObject : AccessibleObject
        {
            internal ScrollBar OwningScrollBar { get; private set; }

            public ScrollBarChildAccessibleObject(ScrollBar owningScrollBar)
            {
                OwningScrollBar = owningScrollBar;
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!OwningScrollBar.IsHandleCreated || !IsDisplayed)
                    {
                        return Rectangle.Empty;
                    }

                    int left = 0;
                    int top = 0;
                    int width = 0;
                    int height = 0;

                    // The "GetChildId" method returns to the id of the ScrollBar element,
                    // which allows to use the native "accLocation" method to get the "Bounds" property
                    ParentInternal.GetSystemIAccessibleInternal()?.accLocation(out left, out top, out width, out height, GetChildId());

                    return new Rectangle(left, top, width, height);
                }
            }

            public override string? DefaultAction
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accDefaultAction(GetChildId());

            public override string? Description
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accDescription(GetChildId());

            public override string? Name
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accName(GetChildId());

            public override AccessibleRole Role
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accRole(GetChildId()) is object accRole
                    ? (AccessibleRole)accRole
                    : AccessibleRole.None;

            public override AccessibleStates State
                => ParentInternal.GetSystemIAccessibleInternal()?.get_accState(GetChildId()) is object accState
                    ? (AccessibleStates)accState
                    : AccessibleStates.None;

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot => ParentInternal;

            internal virtual bool IsDisplayed => OwningScrollBar.Visible;

            internal ScrollBarAccessibleObject ParentInternal => (ScrollBarAccessibleObject)OwningScrollBar.AccessibilityObject;

            internal override int[]? RuntimeId
            {
                get
                {
                    if (!OwningScrollBar.IsHandleCreated)
                    {
                        return base.RuntimeId;
                    }

                    var runtimeId = new int[3];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = PARAM.ToInt(OwningScrollBar.InternalHandle);
                    runtimeId[2] = GetChildId();

                    return runtimeId;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
                => direction switch
                {
                    UiaCore.NavigateDirection.Parent => ParentInternal,
                    _ => base.FragmentNavigate(direction)
                };

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
                => propertyID switch
                {
                    UiaCore.UIA.RuntimeIdPropertyId => RuntimeId,
                    UiaCore.UIA.AccessKeyPropertyId => string.Empty,
                    UiaCore.UIA.ControlTypePropertyId => UiaCore.UIA.ButtonControlTypeId,
                    UiaCore.UIA.IsEnabledPropertyId => OwningScrollBar.Enabled,
                    UiaCore.UIA.NamePropertyId => Name,
                    UiaCore.UIA.HelpTextPropertyId => Help ?? string.Empty,
                    UiaCore.UIA.IsPasswordPropertyId => false,
                    UiaCore.UIA.IsOffscreenPropertyId => (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen,
                    UiaCore.UIA.HasKeyboardFocusPropertyId => false,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId => false,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override void Invoke()
            {
                if (OwningScrollBar.IsHandleCreated && IsDisplayed)
                {
                    // The "GetChildId" method returns to the id of the ScrollBar element,
                    // which allows to use the native "accDoDefaultAction" method when the "Invoke" method is called
                    ParentInternal.GetSystemIAccessibleInternal()?.accDoDefaultAction(GetChildId());
                }
            }

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
                => patternId switch
                {
                    UiaCore.UIA.LegacyIAccessiblePatternId => true,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}
