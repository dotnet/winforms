﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public partial class ToolStripSplitButton
    {
        public class ToolStripSplitButtonAccessibleObject : ToolStripItemAccessibleObject
        {
            private readonly ToolStripSplitButton _owner;

            public ToolStripSplitButtonAccessibleObject(ToolStripSplitButton item) : base(item)
            {
                _owner = item;
            }

            public override void DoDefaultAction()
            {
                _owner.PerformButtonClick();
            }
        }
    }
}
