// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripLabel
{
    internal class ToolStripLabelAccessibleObject : ToolStripItemAccessibleObject
    {
        private readonly ToolStripLabel _owningToolStripLabel;

        public ToolStripLabelAccessibleObject(ToolStripLabel ownerItem) : base(ownerItem)
        {
            _owningToolStripLabel = ownerItem;
        }

        public override string DefaultAction => _owningToolStripLabel.IsLink ? SR.AccessibleActionClick : string.Empty;

        private protected override bool IsInternal => true;

        internal override bool CanGetDefaultActionInternal => false;

        public override void DoDefaultAction()
        {
            if (_owningToolStripLabel.IsLink)
            {
                base.DoDefaultAction();
            }
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
