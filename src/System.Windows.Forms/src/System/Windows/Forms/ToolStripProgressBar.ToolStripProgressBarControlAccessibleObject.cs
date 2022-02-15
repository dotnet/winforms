// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripProgressBar
    {
        internal class ToolStripProgressBarControlAccessibleObject : ProgressBar.ProgressBarAccessibleObject
        {
            private readonly ToolStripProgressBarControl _ownerToolStripProgressBarControl;

            public ToolStripProgressBarControlAccessibleObject(ToolStripProgressBarControl toolStripProgressBarControl) : base(toolStripProgressBarControl)
            {
                _ownerToolStripProgressBarControl = toolStripProgressBarControl;
            }

            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
            {
                get
                {
                    return _ownerToolStripProgressBarControl.Owner.Owner.AccessibilityObject;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                    case UiaCore.NavigateDirection.PreviousSibling:
                    case UiaCore.NavigateDirection.NextSibling:
                        return _ownerToolStripProgressBarControl.Owner.AccessibilityObject.FragmentNavigate(direction);
                }

                return base.FragmentNavigate(direction);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.IsOffscreenPropertyId => GetIsOffscreenPropertyValue(_ownerToolStripProgressBarControl.Owner.Placement, Bounds),
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
