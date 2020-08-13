// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripControlHost
    {
        /// <summary>
        ///  Defines the ToolStripControlHost AccessibleObject.
        /// </summary>
        internal class ToolStripControlHostAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripControlHost _ownerItem;

            /// <summary>
            ///  Initializes the new instance of ToolStripControlHostAccessibleObject.
            /// </summary>
            /// <param name="ownerItem">The owning ToolStripControlHost.</param>
            public ToolStripControlHostAccessibleObject(ToolStripControlHost ownerItem) : base(ownerItem)
            {
                _ownerItem = ownerItem;
            }

            /// <summary>
            ///  Gets a description of the default action for an object.
            /// </summary>
            public override string DefaultAction
            {
                get
                {
                    // Note: empty value is provided due to this Accessible object
                    // represents the control container but not the contained control
                    // itself.
                    return string.Empty;
                }
            }

            /// <summary>
            ///  Performs the default action associated with this accessible object.
            /// </summary>
            public override void DoDefaultAction()
            {
                // Do nothing.
            }

            /// <summary>
            ///  Gets the role of this accessible object.
            /// </summary>
            public override AccessibleRole Role
            {
                get
                {
                    AccessibleObject controlAccessibleObject  = _ownerItem.ControlAccessibilityObject;
                    if (controlAccessibleObject != null)
                    {
                        return controlAccessibleObject.Role;
                    }

                    return AccessibleRole.Default;
                }
            }

            /// <summary>
            ///  Request to return the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                if (direction == UiaCore.NavigateDirection.FirstChild ||
                    direction == UiaCore.NavigateDirection.LastChild)
                {
                    return _ownerItem.Control.AccessibilityObject;
                }

                // Handle Parent and other directions in base ToolStripItem.FragmentNavigate() method.
                return base.FragmentNavigate(direction);
            }

            /// <summary>
            ///  Return the element that is the root node of this fragment of UI.
            /// </summary>
            internal override UiaCore.IRawElementProviderFragmentRoot FragmentRoot
                => _ownerItem.RootToolStrip.AccessibilityObject;
        }
    }
}
