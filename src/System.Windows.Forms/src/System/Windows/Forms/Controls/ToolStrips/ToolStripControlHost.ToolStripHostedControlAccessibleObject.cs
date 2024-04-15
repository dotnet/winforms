// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripControlHost
{
    /// <summary>
    ///  Represents the ToolStrip hosted control accessible object which is responsible
    ///  for accessible navigation within the ToolStrip standard items and hosted controls
    ///  like TextBox, ComboBox, ProgressBar, etc.
    /// </summary>
    public class ToolStripHostedControlAccessibleObject : Control.ControlAccessibleObject
    {
        private readonly ToolStripControlHost? _toolStripControlHost;
        private readonly Control _toolStripHostedControl;

        /// <summary>
        ///  Creates the new instance of ToolStripHostedControlAccessibleObject.
        /// </summary>
        /// <param name="toolStripHostedControl">The ToolStrip control hosted in the ToolStripControlHost container.</param>
        /// <param name="toolStripControlHost">The ToolStripControlHost container which hosts the control.</param>
        public ToolStripHostedControlAccessibleObject(Control toolStripHostedControl, ToolStripControlHost? toolStripControlHost) : base(toolStripHostedControl)
        {
            _toolStripControlHost = toolStripControlHost;
            _toolStripHostedControl = toolStripHostedControl;
        }

        internal override IRawElementProviderFragmentRoot.Interface? FragmentRoot =>
            _toolStripHostedControl is not null // Hosted control should not be null.
            && _toolStripControlHost is not null // ToolStripControlHost is a container for ToolStripControl.
            && _toolStripControlHost.Owner is not null
                ? _toolStripControlHost.Owner.AccessibilityObject
                : base.FragmentRoot;

        internal override IRawElementProviderFragment.Interface? FragmentNavigate(NavigateDirection direction)
        {
            if (_toolStripHostedControl is not null &&
                _toolStripControlHost is not null)
            {
                switch (direction)
                {
                    case NavigateDirection.NavigateDirection_Parent:
                    case NavigateDirection.NavigateDirection_PreviousSibling:
                    case NavigateDirection.NavigateDirection_NextSibling:
                        return _toolStripControlHost.AccessibilityObject.FragmentNavigate(direction);
                }
            }

            return base.FragmentNavigate(direction);
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_HasKeyboardFocusPropertyId => (VARIANT)State.HasFlag(AccessibleStates.Focused),
                UIA_PROPERTY_ID.UIA_IsOffscreenPropertyId => (VARIANT)GetIsOffscreenPropertyValue(_toolStripControlHost?.Placement, Bounds),
                _ => base.GetPropertyValue(propertyID)
            };

        internal override bool IsPatternSupported(UIA_PATTERN_ID patternId)
        {
            if (patternId == UIA_PATTERN_ID.UIA_ValuePatternId)
            {
                return true;
            }

            return base.IsPatternSupported(patternId);
        }
    }
}
