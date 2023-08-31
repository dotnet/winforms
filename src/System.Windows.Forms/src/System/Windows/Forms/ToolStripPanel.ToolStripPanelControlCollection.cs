// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripPanel
{
    internal partial class ToolStripPanelControlCollection : TypedControlCollection
    {
        private readonly ToolStripPanel _owner;

        public ToolStripPanelControlCollection(ToolStripPanel owner)
            : base(owner, typeof(ToolStrip))
        {
            _owner = owner;
        }

        internal override void AddInternal(Control? value)
        {
            if (value is not null)
            {
                using (new LayoutTransaction(value, value, PropertyNames.Parent))
                {
                    base.AddInternal(value);
                }
            }
            else
            {
                base.AddInternal(value);
            }
        }

        internal void Sort()
        {
            if (_owner.Orientation == Orientation.Horizontal)
            {
                InnerList.Sort(new YXComparer());
            }
            else
            {
                InnerList.Sort(new XYComparer());
            }
        }
    }
}
