// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripButton
    {
        /// <summary>
        ///  An implementation of AccessibleChild for use with ToolStripItems
        /// </summary>
        internal class ToolStripButtonAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripButton _ownerItem;

            public ToolStripButtonAccessibleObject(ToolStripButton ownerItem) : base(ownerItem)
            {
                _ownerItem = ownerItem;
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID) =>
                propertyID switch
                {
                    // If we don't set a default role for the accessible object
                    // it will be retrieved from Windows.
                    // And we don't have a 100% guarantee it will be correct, hence set it ourselves.
                    UiaCore.UIA.ControlTypePropertyId when
                        _ownerItem.CheckOnClick
                        => UiaCore.UIA.ButtonControlTypeId,
                    _ => base.GetPropertyValue(propertyID)
                };

            public override AccessibleRole Role
            {
                get
                {
                    if (_ownerItem.CheckOnClick)
                    {
                        return AccessibleRole.CheckButton;
                    }

                    return base.Role;
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    if (_ownerItem.Enabled && _ownerItem.Checked)
                    {
                        return base.State | AccessibleStates.Checked;
                    }

                    // Disabled ToolStripButton, that is selected, must have focus state so that
                    // Narrator can announce it
                    if (!_ownerItem.Enabled && _ownerItem.Selected)
                    {
                        return base.State | AccessibleStates.Focused;
                    }

                    return base.State;
                }
            }
        }
    }
}
