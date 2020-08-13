// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    public partial class ToolStripMenuItem
    {
        private class ToolStripMenuItemInternalLayout : ToolStripItemInternalLayout
        {
            private readonly ToolStripMenuItem ownerItem;

            public ToolStripMenuItemInternalLayout(ToolStripMenuItem ownerItem) : base(ownerItem)
            {
                this.ownerItem = ownerItem;
            }

            public bool ShowCheckMargin
            {
                get
                {
                    if (ownerItem.Owner is ToolStripDropDownMenu menu)
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
                    if (ownerItem.Owner is ToolStripDropDownMenu menu)
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
                        if (ownerItem.Owner is ToolStripDropDownMenu menu)
                        {
                            // since menuItem.Padding isnt taken into consideration, we've got to recalc the centering of
                            // the arrow rect per item
                            Rectangle arrowRect = menu.ArrowRectangle;
                            arrowRect.Y = LayoutUtils.VAlign(arrowRect.Size, ownerItem.ClientBounds, ContentAlignment.MiddleCenter).Y;
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
                        if (ownerItem.Owner is ToolStripDropDownMenu menu)
                        {
                            Rectangle checkRectangle = menu.CheckRectangle;
                            if (ownerItem.CheckedImage != null)
                            {
                                int imageHeight = ownerItem.CheckedImage.Height;
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
                        if (ownerItem.Owner is ToolStripDropDownMenu menu)
                        {
                            // since menuItem.Padding isnt taken into consideration, we've got to recalc the centering of
                            // the image rect per item
                            Rectangle imageRect = menu.ImageRectangle;
                            if (ownerItem.ImageScaling == ToolStripItemImageScaling.SizeToFit)
                            {
                                imageRect.Size = menu.ImageScalingSize;
                            }
                            else
                            {
                                //If we don't have an image, use the CheckedImage
                                Image image = ownerItem.Image ?? ownerItem.CheckedImage;
                                imageRect.Size = image.Size;
                            }
                            imageRect.Y = LayoutUtils.VAlign(imageRect.Size, ownerItem.ClientBounds, ContentAlignment.MiddleCenter).Y;
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
                        if (ownerItem.Owner is ToolStripDropDownMenu menu)
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
                    return ownerItem.Owner is ToolStripDropDownMenu;
                }
            }

            public override Size GetPreferredSize(Size constrainingSize)
            {
                if (UseMenuLayout)
                {
                    if (ownerItem.Owner is ToolStripDropDownMenu menu)
                    {
                        return menu.MaxItemSize;
                    }
                }
                return base.GetPreferredSize(constrainingSize);
            }
        }
    }
}
