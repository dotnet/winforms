// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolStripSeparator
{
    /// <summary>
    ///  An implementation of AccessibleChild for use with ToolStripItems
    /// </summary>
    internal class ToolStripSeparatorAccessibleObject : ToolStripItemAccessibleObject
    {
        private readonly ToolStripSeparator _ownerItem;

        public ToolStripSeparatorAccessibleObject(ToolStripSeparator ownerItem) : base(ownerItem)
        {
            _ownerItem = ownerItem;
        }

        public override AccessibleRole Role
        {
            get
            {
                AccessibleRole role = _ownerItem.AccessibleRole;
                if (role != AccessibleRole.Default)
                {
                    return role;
                }

                return AccessibleRole.Separator;
            }
        }
    }
}
