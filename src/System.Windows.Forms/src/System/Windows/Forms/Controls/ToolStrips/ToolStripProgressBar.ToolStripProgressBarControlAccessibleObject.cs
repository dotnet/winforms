// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripProgressBar
{
    internal class ToolStripProgressBarControlAccessibleObject : ProgressBar.ProgressBarAccessibleObject
    {
        private readonly ToolStripProgressBarControl _ownerToolStripProgressBarControl;

        public ToolStripProgressBarControlAccessibleObject(ToolStripProgressBarControl toolStripProgressBarControl) : base(toolStripProgressBarControl)
        {
            _ownerToolStripProgressBarControl = toolStripProgressBarControl;
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
            _ownerToolStripProgressBarControl.Owner?.Owner?.AccessibilityObject;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction) =>
            direction switch
            {
                NavigateDirection.NavigateDirection_Parent
                or NavigateDirection.NavigateDirection_PreviousSibling
                or NavigateDirection.NavigateDirection_NextSibling
                    => _ownerToolStripProgressBarControl.Owner?.AccessibilityObject.FragmentNavigate(direction),
                _ => base.FragmentNavigate(direction),
            };

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => (VARIANT)GetIsOffscreenPropertyValue(_ownerToolStripProgressBarControl.Owner?.Placement, Bounds),
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
