// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripSplitButton
{
    /// <summary>
    ///  This class represents the item to the left of the dropdown [ A |v]  (e.g the "A")
    ///  It exists so that we can use our existing methods for text and image layout
    ///  and have a place to stick certain state information like pushed and selected
    ///  Note since this is NOT an actual item hosted on the ToolStrip - it won't get things
    ///  like MouseOver, won't be laid out by the ToolStrip, etc etc. This is purely internal
    ///  convenience.
    /// </summary>
    private class ToolStripSplitButtonButton : ToolStripButton
    {
        private readonly ToolStripSplitButton _owner;

        public ToolStripSplitButtonButton(ToolStripSplitButton owner)
        {
            _owner = owner;
        }

        public override bool Enabled
        {
            get
            {
                return _owner.Enabled;
            }
            set
            {
                // do nothing
            }
        }

        public override ToolStripItemDisplayStyle DisplayStyle
        {
            get
            {
                return _owner.DisplayStyle;
            }
            set
            {
                // do nothing
            }
        }

        public override Padding Padding
        {
            get
            {
                return _owner.Padding;
            }
            set
            {
                // do nothing
            }
        }

        public override ToolStripTextDirection TextDirection
        {
            get
            {
                return _owner.TextDirection;
            }
        }

        public override Image? Image
        {
            get
            {
                if ((_owner.DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image)
                {
                    return _owner.Image;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // do nothing
            }
        }

        public override bool Selected
        {
            get
            {
                if (_owner is not null)
                {
                    return _owner.Selected;
                }

                return base.Selected;
            }
        }

        public override string? Text
        {
            get
            {
                if ((_owner.DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text)
                {
                    return _owner.Text;
                }
                else
                {
                    return null;
                }
            }
            set
            {
                // do nothing
            }
        }
    }
}
