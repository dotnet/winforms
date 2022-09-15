﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripControlHost
    {
        /// <summary>
        ///  Represents the ToolStrip hosted control accessible object which is responsible
        ///  for accessible navigation within the ToolStrip standard items and hosted controls
        ///  like TextBox, ComboBox, ProgressBar, etc.
        /// </summary>
        public class ToolStripHostedControlAccessibleObject : Control.ControlAccessibleObject
        {
            private ToolStripControlHost? _toolStripControlHost;
            private Control _toolStripHostedControl;

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

            internal override UiaCore.IRawElementProviderFragmentRoot? FragmentRoot
            {
                get
                {
                    if (_toolStripHostedControl is not null // Hosted control should not be null.
                        && _toolStripControlHost is not null // ToolStripControlHost is a container for ToolStripControl.
                        && _toolStripControlHost.Owner is not null) // Owner is the ToolStrip.
                    {
                        return _toolStripControlHost.Owner.AccessibilityObject;
                    }

                    return base.FragmentRoot;
                }
            }

            internal override UiaCore.IRawElementProviderFragment? FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (_toolStripHostedControl is not null &&
                    _toolStripControlHost is not null)
                {
                    switch (direction)
                    {
                        case UiaCore.NavigateDirection.Parent:
                        case UiaCore.NavigateDirection.PreviousSibling:
                        case UiaCore.NavigateDirection.NextSibling:
                            return _toolStripControlHost.AccessibilityObject.FragmentNavigate(direction);
                    }
                }

                return base.FragmentNavigate(direction);
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    UiaCore.UIA.HasKeyboardFocusPropertyId => (State & AccessibleStates.Focused) == AccessibleStates.Focused,
                    UiaCore.UIA.IsOffscreenPropertyId => GetIsOffscreenPropertyValue(_toolStripControlHost?.Placement, Bounds),
                    _ => base.GetPropertyValue(propertyID)
                };

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }
        }
    }
}
