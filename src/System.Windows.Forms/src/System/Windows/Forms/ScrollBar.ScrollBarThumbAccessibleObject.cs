// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop.UiaCore;

namespace System.Windows.Forms
{
    public partial class ScrollBar
    {
        internal class ScrollBarThumbAccessibleObject : ScrollBarChildAccessibleObject
        {
            public ScrollBarThumbAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
            {
            }

            public override string? DefaultAction => string.Empty;

            internal override IRawElementProviderFragment? FragmentNavigate(NavigateDirection direction)
            {
                if (!OwningScrollBar.IsHandleCreated)
                {
                    return null;
                }

                return direction switch
                {
                    NavigateDirection.PreviousSibling
                        => ParentInternal.FirstPageButtonAccessibleObject.IsDisplayed
                            ? ParentInternal.FirstPageButtonAccessibleObject
                            : ParentInternal.FirstLineButtonAccessibleObject,
                    NavigateDirection.NextSibling
                        => ParentInternal.LastPageButtonAccessibleObject.IsDisplayed
                            ? ParentInternal.LastPageButtonAccessibleObject
                            : ParentInternal.LastLineButtonAccessibleObject,
                    _ => base.FragmentNavigate(direction)
                };
            }

            internal override int GetChildId() => 3;

            internal override object? GetPropertyValue(UIA propertyID)
                => propertyID switch
                {
                    UIA.ControlTypePropertyId => UIA.ThumbControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UIA patternId)
                => patternId switch
                {
                    UIA.InvokePatternId => false,
                    _ => base.IsPatternSupported(patternId)
                };
        }
    }
}
