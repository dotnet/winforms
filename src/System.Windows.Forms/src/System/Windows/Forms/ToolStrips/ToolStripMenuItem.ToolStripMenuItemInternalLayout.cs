// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public partial class ToolStripMenuItem
{
    private class ToolStripMenuItemInternalLayout : ToolStripItemInternalLayout
    {
        private readonly ToolStripMenuItem _ownerItem;

        public ToolStripMenuItemInternalLayout(ToolStripMenuItem ownerItem)
            : base(ownerItem)
        {
            _ownerItem = ownerItem;
        }

        public bool ShowCheckMargin
        {
            get
            {
                if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                {
                    return menu.ShowCheckMargin;
                }

                return false;
            }
        }

        public bool ShowImageMargin
        {
            get
            {
                if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                {
                    return menu.ShowImageMargin;
                }

                return false;
            }
        }

        public bool PaintCheck
        {
            get
            {
                return ShowCheckMargin || ShowImageMargin;
            }
        }

        public bool PaintImage
        {
            get
            {
                return ShowImageMargin;
            }
        }

        public Rectangle ArrowRectangle
        {
            get
            {
                if (UseMenuLayout)
                {
                    if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                    {
                        // since menuItem.Padding isn't taken into consideration, we've got to recalc the centering of
                        // the arrow rect per item
                        Rectangle arrowRect = menu.ArrowRectangle;
                        arrowRect.Y = LayoutUtils.VAlign(arrowRect.Size, _ownerItem.ClientBounds, ContentAlignment.MiddleCenter).Y;
                        return arrowRect;
                    }
                }

                return Rectangle.Empty;
            }
        }

        public Rectangle CheckRectangle
        {
            get
            {
                if (UseMenuLayout)
                {
                    if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                    {
                        Rectangle checkRectangle = menu.CheckRectangle;
                        if (_ownerItem.CheckedImage is not null)
                        {
                            int imageHeight = _ownerItem.CheckedImage.Height;
                            // make sure we're vertically centered
                            checkRectangle.Y += (checkRectangle.Height - imageHeight) / 2;
                            checkRectangle.Height = imageHeight;
                            return checkRectangle;
                        }
                    }
                }

                return Rectangle.Empty;
            }
        }

        public override Rectangle ImageRectangle
        {
            get
            {
                if (UseMenuLayout)
                {
                    if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                    {
                        // since menuItem.Padding isn't taken into consideration, we've got to recalc the centering of
                        // the image rect per item
                        Rectangle imageRect = menu.ImageRectangle;
                        if (_ownerItem.ImageScaling == ToolStripItemImageScaling.SizeToFit)
                        {
                            imageRect.Size = menu.ImageScalingSize;
                        }
                        else
                        {
                            // If we don't have an image, use the CheckedImage.
                            Image? image = _ownerItem.Image ?? _ownerItem.CheckedImage;
                            Debug.Assert(image is not null);
                            imageRect.Size = image?.Size ?? menu.ImageScalingSize;
                        }

                        imageRect.Y = LayoutUtils.VAlign(imageRect.Size, _ownerItem.ClientBounds, ContentAlignment.MiddleCenter).Y;
                        return imageRect;
                    }
                }

                return base.ImageRectangle;
            }
        }

        public override Rectangle TextRectangle
        {
            get
            {
                if (UseMenuLayout)
                {
                    if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                    {
                        return menu.TextRectangle;
                    }
                }

                return base.TextRectangle;
            }
        }

        public bool UseMenuLayout
        {
            get
            {
                return _ownerItem.Owner is ToolStripDropDownMenu;
            }
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            if (UseMenuLayout)
            {
                if (_ownerItem.Owner is ToolStripDropDownMenu menu)
                {
                    return menu.MaxItemSize;
                }
            }

            return base.GetPreferredSize(constrainingSize);
        }
    }
}
