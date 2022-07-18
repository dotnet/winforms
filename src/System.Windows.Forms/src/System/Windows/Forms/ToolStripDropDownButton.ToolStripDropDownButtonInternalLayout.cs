// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    public partial class ToolStripDropDownButton
    {
        private protected class ToolStripDropDownButtonInternalLayout : ToolStripItemInternalLayout
        {
            private ToolStripDropDownButton ownerItem;
            private static readonly Size dropDownArrowSizeUnscaled = new Size(5, 3);
            private static Size dropDownArrowSize = dropDownArrowSizeUnscaled;
            private const int DROP_DOWN_ARROW_PADDING = 2;
            private static Padding dropDownArrowPadding = new Padding(DROP_DOWN_ARROW_PADDING);
            private Padding scaledDropDownArrowPadding = dropDownArrowPadding;
            private Rectangle dropDownArrowRect = Rectangle.Empty;

            public ToolStripDropDownButtonInternalLayout(ToolStripDropDownButton ownerItem) : base(ownerItem)
            {
                if (DpiHelper.IsPerMonitorV2Awareness)
                {
                    dropDownArrowSize = DpiHelper.LogicalToDeviceUnits(dropDownArrowSizeUnscaled, ownerItem.DeviceDpi);
                    scaledDropDownArrowPadding = DpiHelper.LogicalToDeviceUnits(dropDownArrowPadding, ownerItem.DeviceDpi);
                }
                else if (DpiHelper.IsScalingRequired)
                {
                    // these 2 values are used to calculate size of the clickable drop down button
                    // on the right of the image/text
                    dropDownArrowSize = DpiHelper.LogicalToDeviceUnits(dropDownArrowSizeUnscaled);
                    scaledDropDownArrowPadding = DpiHelper.LogicalToDeviceUnits(dropDownArrowPadding);
                }

                this.ownerItem = ownerItem;
            }

            public override Size GetPreferredSize(Size constrainingSize)
            {
                Size preferredSize = base.GetPreferredSize(constrainingSize);
                if (ownerItem.ShowDropDownArrow)
                {
                    if (ownerItem.TextDirection == ToolStripTextDirection.Horizontal)
                    {
                        preferredSize.Width += DropDownArrowRect.Width + scaledDropDownArrowPadding.Horizontal;
                    }
                    else
                    {
                        preferredSize.Height += DropDownArrowRect.Height + scaledDropDownArrowPadding.Vertical;
                    }
                }

                return preferredSize;
            }

            protected override ToolStripItemLayoutOptions CommonLayoutOptions()
            {
                ToolStripItemLayoutOptions options = base.CommonLayoutOptions();

                if (ownerItem.ShowDropDownArrow)
                {
                    if (ownerItem.TextDirection == ToolStripTextDirection.Horizontal)
                    {
                        // We're rendering horizontal....  make sure to take care of RTL issues.

                        int widthOfDropDown = dropDownArrowSize.Width + scaledDropDownArrowPadding.Horizontal;
                        options.Client.Width -= widthOfDropDown;

                        if (ownerItem.RightToLeft == RightToLeft.Yes)
                        {
                            // if RightToLeft.Yes: [ v | rest of drop down button ]
                            options.Client.Offset(widthOfDropDown, 0);
                            dropDownArrowRect = new Rectangle(scaledDropDownArrowPadding.Left, 0, dropDownArrowSize.Width, ownerItem.Bounds.Height);
                        }
                        else
                        {
                            // if RightToLeft.No [ rest of drop down button | v ]
                            dropDownArrowRect = new Rectangle(options.Client.Right, 0, dropDownArrowSize.Width, ownerItem.Bounds.Height);
                        }
                    }
                    else
                    {
                        // else we're rendering vertically.
                        int heightOfDropDown = dropDownArrowSize.Height + scaledDropDownArrowPadding.Vertical;

                        options.Client.Height -= heightOfDropDown;

                        //  [ rest of button / v]
                        dropDownArrowRect = new Rectangle(0, options.Client.Bottom + scaledDropDownArrowPadding.Top, ownerItem.Bounds.Width - 1, dropDownArrowSize.Height);
                    }
                }

                return options;
            }

            public Rectangle DropDownArrowRect
            {
                get
                {
                    return dropDownArrowRect;
                }
            }
        }
    }
}
