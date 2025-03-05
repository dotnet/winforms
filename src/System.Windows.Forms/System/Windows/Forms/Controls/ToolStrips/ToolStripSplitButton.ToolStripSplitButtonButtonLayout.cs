// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripSplitButton
{
    /// <summary>
    ///  This class performs internal layout for the "split button button" portion of a split button.
    ///  Its main job is to make sure the inner button has the same parent as the split button, so
    ///  that layout can be performed using the correct graphics context.
    /// </summary>
    private class ToolStripSplitButtonButtonLayout : ToolStripItemInternalLayout
    {
        private readonly ToolStripSplitButton _owner;

        public ToolStripSplitButtonButtonLayout(ToolStripSplitButton owner)
            : base(owner.SplitButtonButton)
        {
            _owner = owner;
        }

        protected override ToolStripItem Owner
        {
            get { return _owner; }
        }

        protected override ToolStrip? ParentInternal
        {
            get
            {
                return _owner.ParentInternal;
            }
        }

        public override Rectangle ImageRectangle
        {
            get
            {
                Rectangle imageRect = base.ImageRectangle;
                // translate to ToolStripItem coordinates
                imageRect.Offset(_owner.SplitButtonButton.Bounds.Location);
                return imageRect;
            }
        }

        public override Rectangle TextRectangle
        {
            get
            {
                Rectangle textRect = base.TextRectangle;
                // translate to ToolStripItem coordinates
                textRect.Offset(_owner.SplitButtonButton.Bounds.Location);
                return textRect;
            }
        }
    }
}
