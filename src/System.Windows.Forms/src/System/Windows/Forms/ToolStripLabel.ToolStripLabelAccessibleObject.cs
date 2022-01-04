// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripLabel
    {
        internal class ToolStripLabelAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripLabel _owningToolStripLabel;

            public ToolStripLabelAccessibleObject(ToolStripLabel ownerItem) : base(ownerItem)
            {
                _owningToolStripLabel = ownerItem;
            }

            public override string DefaultAction
            {
                get
                {
                    if (_owningToolStripLabel.IsLink)
                    {
                        return SR.AccessibleActionClick;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override void DoDefaultAction()
            {
                if (_owningToolStripLabel.IsLink)
                {
                    base.DoDefaultAction();
                }
            }

            internal override object? GetPropertyValue(UiaCore.UIA propertyID)
            {
                if (propertyID == UiaCore.UIA.LegacyIAccessibleStatePropertyId)
                {
                    return State;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }

                    return _owningToolStripLabel.IsLink ? AccessibleRole.Link : AccessibleRole.StaticText;
                }
            }

            public override AccessibleStates State
            {
                get => base.State | AccessibleStates.ReadOnly;
            }
        }
    }
}
