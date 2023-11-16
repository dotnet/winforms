// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

public partial class ToolStripDropDownButton
{
    private protected class ToolStripDropDownButtonInternalLayout : ToolStripItemInternalLayout
    {
        private ToolStripDropDownButton _ownerItem;
        private static readonly Size s_dropDownArrowSizeUnscaled = new(5, 3);
        private static Size s_dropDownArrowSize = s_dropDownArrowSizeUnscaled;
        private const int DROP_DOWN_ARROW_PADDING = 2;
        private static Padding s_dropDownArrowPadding = new(DROP_DOWN_ARROW_PADDING);
        private Padding _scaledDropDownArrowPadding = s_dropDownArrowPadding;
        private Rectangle _dropDownArrowRect = Rectangle.Empty;

        public ToolStripDropDownButtonInternalLayout(ToolStripDropDownButton ownerItem)
            : base(ownerItem)
        {
            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                s_dropDownArrowSize = DpiHelper.LogicalToDeviceUnits(s_dropDownArrowSizeUnscaled, ownerItem.DeviceDpi);
                _scaledDropDownArrowPadding = DpiHelper.LogicalToDeviceUnits(s_dropDownArrowPadding, ownerItem.DeviceDpi);
            }
            else if (DpiHelper.IsScalingRequired)
            {
                // these 2 values are used to calculate size of the clickable drop down button
                // on the right of the image/text
                s_dropDownArrowSize = DpiHelper.LogicalToDeviceUnits(s_dropDownArrowSizeUnscaled);
                _scaledDropDownArrowPadding = DpiHelper.LogicalToDeviceUnits(s_dropDownArrowPadding);
            }

            _ownerItem = ownerItem;
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = base.GetPreferredSize(constrainingSize);
            if (_ownerItem.ShowDropDownArrow)
            {
                if (_ownerItem.TextDirection == ToolStripTextDirection.Horizontal)
                {
                    preferredSize.Width += DropDownArrowRect.Width + _scaledDropDownArrowPadding.Horizontal;
                }
                else
                {
                    preferredSize.Height += DropDownArrowRect.Height + _scaledDropDownArrowPadding.Vertical;
                }
            }

            return preferredSize;
        }

        protected override ToolStripItemLayoutOptions CommonLayoutOptions()
        {
            ToolStripItemLayoutOptions options = base.CommonLayoutOptions();

            if (_ownerItem.ShowDropDownArrow)
            {
                if (_ownerItem.TextDirection == ToolStripTextDirection.Horizontal)
                {
                    // We're rendering horizontal....  make sure to take care of RTL issues.

                    int widthOfDropDown = s_dropDownArrowSize.Width + _scaledDropDownArrowPadding.Horizontal;
                    options.Client.Width -= widthOfDropDown;

                    if (_ownerItem.RightToLeft == RightToLeft.Yes)
                    {
                        // if RightToLeft.Yes: [ v | rest of drop down button ]
                        options.Client.Offset(widthOfDropDown, 0);
                        _dropDownArrowRect = new Rectangle(_scaledDropDownArrowPadding.Left, 0, s_dropDownArrowSize.Width, _ownerItem.Bounds.Height);
                    }
                    else
                    {
                        // if RightToLeft.No [ rest of drop down button | v ]
                        _dropDownArrowRect = new Rectangle(options.Client.Right, 0, s_dropDownArrowSize.Width, _ownerItem.Bounds.Height);
                    }
                }
                else
                {
                    // else we're rendering vertically.
                    int heightOfDropDown = s_dropDownArrowSize.Height + _scaledDropDownArrowPadding.Vertical;

                    options.Client.Height -= heightOfDropDown;

                    //  [ rest of button / v]
                    _dropDownArrowRect = new Rectangle(0, options.Client.Bottom + _scaledDropDownArrowPadding.Top, _ownerItem.Bounds.Width - 1, s_dropDownArrowSize.Height);
                }
            }

            return options;
        }

        public Rectangle DropDownArrowRect
        {
            get
            {
                return _dropDownArrowRect;
            }
        }
    }
}
