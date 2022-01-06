// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class ToolStripOverflowButton
    {
        internal class ToolStripOverflowButtonAccessibleObject : ToolStripDropDownItemAccessibleObject
        {
            public ToolStripOverflowButtonAccessibleObject(ToolStripOverflowButton owner) : base(owner)
            {
            }

            public override string? Name
            {
                get => Owner.AccessibleName ?? SR.ToolStripOptions;
                set => base.Name = value;
            }
        }
    }
}
