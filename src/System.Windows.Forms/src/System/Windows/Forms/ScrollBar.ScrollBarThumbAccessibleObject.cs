// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Accessibility;
using static Interop.UiaCore;

namespace System.Windows.Forms;

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
                    => ParentInternal.FirstPageButtonAccessibleObject?.IsDisplayed == true
                        ? ParentInternal.FirstPageButtonAccessibleObject
                        : ParentInternal.FirstLineButtonAccessibleObject,
                NavigateDirection.NextSibling
                    => ParentInternal.LastPageButtonAccessibleObject?.IsDisplayed == true
                        ? ParentInternal.LastPageButtonAccessibleObject
                        : ParentInternal.LastLineButtonAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 3;

        internal override object? GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId,
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
            => patternId switch
            {
                UIA_PATTERN_ID.UIA_InvokePatternId => false,
                _ => base.IsPatternSupported(patternId)
            };
    }
}
