// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ScrollBar
{
    internal sealed class ScrollBarThumbAccessibleObject : ScrollBarChildAccessibleObject
    {
        public ScrollBarThumbAccessibleObject(ScrollBar owningScrollBar) : base(owningScrollBar)
        {
        }

        public override string? DefaultAction => string.Empty;

        private protected override bool IsInternal => true;

        internal override bool CanGetDefaultActionInternal => false;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (!OwningScrollBar.IsHandleCreated)
            {
                return null;
            }

            return direction switch
            {
                NavigateDirection.NavigateDirection_PreviousSibling
                    => ParentInternal.FirstPageButtonAccessibleObject?.IsDisplayed == true
                        ? ParentInternal.FirstPageButtonAccessibleObject
                        : ParentInternal.FirstLineButtonAccessibleObject,
                NavigateDirection.NavigateDirection_NextSibling
                    => ParentInternal.LastPageButtonAccessibleObject?.IsDisplayed == true
                        ? ParentInternal.LastPageButtonAccessibleObject
                        : ParentInternal.LastLineButtonAccessibleObject,
                _ => base.FragmentNavigate(direction)
            };
        }

        internal override int GetChildId() => 3;

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID)
            => propertyID switch
            {
                UIA_PROPERTY_ID.UIA_ControlTypePropertyId => (VARIANT)(int)UIA_CONTROLTYPE_ID.UIA_ThumbControlTypeId,
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
