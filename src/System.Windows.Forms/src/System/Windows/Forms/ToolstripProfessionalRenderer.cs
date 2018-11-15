// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
   
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Drawing.Drawing2D;
    using System.Diagnostics;
    using System.Windows.Forms.Layout;

    /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer"]/*' />
    /// <summary>
    /// Summary description for ProfessionalToolStripRenderer.
    /// </summary>
    public class ToolStripProfessionalRenderer : ToolStripRenderer {
        private const int GRIP_PADDING = 4;
        private int gripPadding = GRIP_PADDING;

        private const int ICON_WELL_GRADIENT_WIDTH = 12;
        private int iconWellGradientWidth = ICON_WELL_GRADIENT_WIDTH;

        private static readonly Size onePix = new Size(1, 1);

        private bool isScalingInitialized = false;
        private const int OVERFLOW_BUTTON_WIDTH = 12;
        private const int OVERFLOW_ARROW_WIDTH = 9;
        private const int OVERFLOW_ARROW_HEIGHT = 5;
        private const int OVERFLOW_ARROW_OFFSETY = 8;
        private int overflowButtonWidth = OVERFLOW_BUTTON_WIDTH;
        private int overflowArrowWidth = OVERFLOW_ARROW_WIDTH;
        private int overflowArrowHeight = OVERFLOW_ARROW_HEIGHT;
        private int overflowArrowOffsetY = OVERFLOW_ARROW_OFFSETY;

        private const int DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE = 1;
        private Padding scaledDropDownMenuItemPaintPadding = new Padding(DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE + 1, 0, DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE, 0);
        private ProfessionalColorTable professionalColorTable;
        private bool roundedEdges = true;
        private ToolStripRenderer toolStripHighContrastRenderer;
        private ToolStripRenderer toolStripLowResolutionRenderer;

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.ToolStripProfessionalRenderer"]/*' />
        public ToolStripProfessionalRenderer() {
        }
        internal ToolStripProfessionalRenderer(bool isDefault) : base(isDefault) {
        }

        public ToolStripProfessionalRenderer(ProfessionalColorTable professionalColorTable) {
            this.professionalColorTable =   professionalColorTable; 
        }

        public ProfessionalColorTable ColorTable {
            get { 
                if (professionalColorTable == null) {
                    return ProfessionalColors.ColorTable;
                }
                return professionalColorTable;
            }
        }

        internal override ToolStripRenderer RendererOverride {
            get {
                if (DisplayInformation.HighContrast) {
                    return HighContrastRenderer;
                }
                if (DisplayInformation.LowResolution) {
                    return LowResolutionRenderer;
                }
                return null;
            }
        }

        internal ToolStripRenderer HighContrastRenderer {
            get {
                if (toolStripHighContrastRenderer == null) {
                    toolStripHighContrastRenderer = new ToolStripHighContrastRenderer(/*renderLikeSystem*/false);
                }
                return toolStripHighContrastRenderer;
            }
        }

        internal ToolStripRenderer LowResolutionRenderer {
            get {
                if (toolStripLowResolutionRenderer == null) {
                    toolStripLowResolutionRenderer = new ToolStripProfessionalLowResolutionRenderer();
                }
                return toolStripLowResolutionRenderer;
            }
        }
        
        
    
        public bool RoundedEdges {
            get {
                return roundedEdges;
            }
            set {
                roundedEdges = value;
            }
        }

        
        private bool UseSystemColors {
            get { return (ColorTable.UseSystemColors || !ToolStripManager.VisualStylesEnabled); }
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderBackground"]/*' />
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderToolStripBackground(e);
                return;
            }

            ToolStrip toolStrip = e.ToolStrip;

            if (!ShouldPaintBackground(toolStrip)) {
                return;
            }
       
            if (toolStrip is ToolStripDropDown) {
                RenderToolStripDropDownBackground(e);
            }
            else if (toolStrip is MenuStrip) {
                RenderMenuStripBackground(e);
            }
            else if (toolStrip is StatusStrip) {
                RenderStatusStripBackground(e);
            }
            else {
                RenderToolStripBackgroundInternal(e);
            }
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderOverflowButton"]/*' />
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e) {
            ScaleObjectSizesIfNeeded(); 
            
            if (RendererOverride != null) {
                base.OnRenderOverflowButtonBackground(e);
                return;
            }

            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
                        
            // fill in the background colors
            bool rightToLeft = (item.RightToLeft == RightToLeft.Yes);
            RenderOverflowBackground(e, rightToLeft);

            bool horizontal = (e.ToolStrip.Orientation == Orientation.Horizontal);
          
            Rectangle overflowArrowRect = Rectangle.Empty;
            if (rightToLeft) {
                overflowArrowRect = new Rectangle(0, item.Height - overflowArrowOffsetY, overflowArrowWidth, overflowArrowHeight);        
            }
            else {
                overflowArrowRect = new Rectangle(item.Width - overflowButtonWidth, item.Height - overflowArrowOffsetY, overflowArrowWidth, overflowArrowHeight);
            }

            ArrowDirection direction = (horizontal) ? ArrowDirection.Down : ArrowDirection.Right;

            // in RTL the white highlight goes BEFORE the black triangle.
            int rightToLeftShift = (rightToLeft && horizontal) ? -1: 1;

            // draw highlight	
            overflowArrowRect.Offset(1*rightToLeftShift, 1);
            RenderArrowInternal(g, overflowArrowRect, direction, SystemBrushes.ButtonHighlight);

            // draw black triangle
            overflowArrowRect.Offset(-1*rightToLeftShift, -1);
            Point middle = RenderArrowInternal(g, overflowArrowRect, direction, SystemBrushes.ControlText);

            // draw lines
            if (horizontal) {
                rightToLeftShift = (rightToLeft) ? -2 : 0;
                // width of the both lines is 1 pixel and lines are drawn next to each other, this the highlight line is 1 pixel below the black line 
                g.DrawLine(SystemPens.ControlText, 
                    middle.X - ToolStripRenderer.Offset2X, 
                    overflowArrowRect.Y - ToolStripRenderer.Offset2Y, 
                    middle.X + ToolStripRenderer.Offset2X, 
                    overflowArrowRect.Y - ToolStripRenderer.Offset2Y);
                g.DrawLine(SystemPens.ButtonHighlight, 
                    middle.X - ToolStripRenderer.Offset2X + 1 + rightToLeftShift, 
                    overflowArrowRect.Y - ToolStripRenderer.Offset2Y + 1, 
                    middle.X + ToolStripRenderer.Offset2X + 1 + rightToLeftShift, 
                    overflowArrowRect.Y - ToolStripRenderer.Offset2Y + 1);
            }
            else {
                g.DrawLine(SystemPens.ControlText, 
                    overflowArrowRect.X, 
                    middle.Y - ToolStripRenderer.Offset2Y, 
                    overflowArrowRect.X, 
                    middle.Y + ToolStripRenderer.Offset2Y);
                g.DrawLine(SystemPens.ButtonHighlight, 
                    overflowArrowRect.X + 1, 
                    middle.Y - ToolStripRenderer.Offset2Y + 1, 
                    overflowArrowRect.X + 1, 
                    middle.Y + ToolStripRenderer.Offset2Y + 1);
            }
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderDropDownButton"]/*' />
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderDropDownButtonBackground(e);
                return;
            }

            ToolStripDropDownItem item = e.Item as ToolStripDropDownItem;

            if (item != null && item.Pressed && item.HasDropDownItems) {
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);

                RenderPressedGradient(e.Graphics, bounds);
            }
            else {
                RenderItemInternal(e, /*useHotBorder =*/true);
            }
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderSeparator"]/*' />
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderSeparator(e);
                return;
            }

            RenderSeparatorInternal(e.Graphics, e.Item, new Rectangle(Point.Empty, e.Item.Size), e.Vertical);
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderSplitButton"]/*' />
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderSplitButtonBackground(e);
                return;
            }

            ToolStripSplitButton item = e.Item as ToolStripSplitButton;
            Graphics g = e.Graphics;

            if (item != null) {
                
                Rectangle bounds = new Rectangle(Point.Empty, item.Size);
                if (item.BackgroundImage != null) {
                    Rectangle fillRect = (item.Selected) ? item.ContentRectangle : bounds;
                    ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                }

                bool buttonPressedOrSelected = (item.Pressed || item.ButtonPressed || item.Selected || item.ButtonSelected);

                if (buttonPressedOrSelected) {
                    RenderItemInternal(e, /*useHotBorder =*/true);
                }

                if (item.ButtonPressed) {
                    Rectangle buttonBounds = item.ButtonBounds;
                    // We subtract 1 from each side except the right.
                    // This is because we've already drawn the border, and we don't
                    // want to draw over it.  We don't do the right edge, because we
                    // drew the border around the whole control, not the button.
                    Padding deflatePadding = item.RightToLeft == RightToLeft.Yes ? new Padding(0, 1, 1, 1) : new Padding(1, 1, 0, 1);
                    buttonBounds = LayoutUtils.DeflateRect(buttonBounds, deflatePadding);
                    RenderPressedButtonFill(g, buttonBounds);
                }
                else if (item.Pressed) {
                    RenderPressedGradient(e.Graphics, bounds);
                }
                Rectangle dropDownRect = item.DropDownButtonBounds;

                if (buttonPressedOrSelected && !item.Pressed) {
                    using (Brush b = new SolidBrush(ColorTable.ButtonSelectedBorder)) {
                        g.FillRectangle(b, item.SplitterBounds);
                    }
                }
                DrawArrow(new ToolStripArrowRenderEventArgs(g, item, dropDownRect, SystemColors.ControlText, ArrowDirection.Down));
            }
        }

        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderToolStripStatusLabelBackground(e);
                return;
            }

            RenderLabelInternal(e);
            ToolStripStatusLabel item = e.Item as ToolStripStatusLabel;
            ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0,0,item.Width, item.Height), item.BorderStyle, (Border3DSide)item.BorderSides);
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderLabel"]/*' />
        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderLabelBackground(e);
                return;
            }

            RenderLabelInternal(e);
        }

   
        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderButton"]/*' />
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderButtonBackground(e);
                return;
            }

            ToolStripButton item = e.Item as ToolStripButton;
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            
            if (item.CheckState == CheckState.Unchecked) {
                RenderItemInternal(e, /*useHotBorder = */ true);
            }
            else {
                Rectangle fillRect = (item.Selected) ? item.ContentRectangle :bounds;
                
                if (item.BackgroundImage != null) {
                   ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                }

                if (UseSystemColors) {
                    if (item.Selected) {
                        RenderPressedButtonFill(g, bounds);
                    }
                    else {
                        RenderCheckedButtonFill(g, bounds);
                    }
                 
                    using (Pen p = new Pen(ColorTable.ButtonSelectedBorder)) {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }

                }
                else {
                   if (item.Selected) {
                      RenderPressedButtonFill(g,bounds);
                   }
                   else {
                      RenderCheckedButtonFill(g,bounds);
                   }
                   using (Pen p = new Pen(ColorTable.ButtonSelectedBorder)) {
                      g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                   }
                
                }
            }

        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderBorder"]/*' />
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderToolStripBorder(e);
                return;
            }

            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;

            
            if (toolStrip is ToolStripDropDown) {
                RenderToolStripDropDownBorder(e);
            }
            else if (toolStrip is MenuStrip) {
            }
            else if (toolStrip is StatusStrip) {
               RenderStatusStripBorder(e);
            }
            else {
                Rectangle bounds = new Rectangle(Point.Empty, toolStrip.Size);

                // draw the shadow lines on the bottom and right
                using (Pen p = new Pen(ColorTable.ToolStripBorder)) {
                    if (toolStrip.Orientation == Orientation.Horizontal) {
                        // horizontal line at bottom
                        g.DrawLine(p, bounds.Left, bounds.Height - 1, bounds.Right, bounds.Height - 1);
                        if (RoundedEdges) {
                            // one pix corner rounding (right bottom)
                            g.DrawLine(p, bounds.Width - 2, bounds.Height - 2, bounds.Width - 1, bounds.Height - 3);
                        }
                    }
                    else {
                        // draw vertical line on the right
                        g.DrawLine(p, bounds.Width -1, 0,bounds.Width -1, bounds.Height - 1);
                        if (RoundedEdges) {
                            // one pix corner rounding (right bottom)
                            g.DrawLine(p, bounds.Width - 2, bounds.Height - 2, bounds.Width - 1, bounds.Height - 3);
                        }

                    }
                }

                if (RoundedEdges) {
                    // OverflowButton rendering
                    if (toolStrip.OverflowButton.Visible) {
                        RenderOverflowButtonEffectsOverBorder(e);
                    }
                    else {

                        // Draw 1PX edging to round off the toolStrip
                        Rectangle edging = Rectangle.Empty;
                        if (toolStrip.Orientation == Orientation.Horizontal) {
                           edging = new Rectangle(bounds.Width - 1, 3, 1, bounds.Height - 3);
                        }
                        else {
                            edging = new Rectangle(3, bounds.Height -1, bounds.Width -3, bounds.Height - 1);

                        }
                        ScaleObjectSizesIfNeeded();
                        FillWithDoubleGradient(ColorTable.OverflowButtonGradientBegin, ColorTable.OverflowButtonGradientMiddle, ColorTable.OverflowButtonGradientEnd, e.Graphics, edging, iconWellGradientWidth, iconWellGradientWidth, LinearGradientMode.Vertical, /*flipHorizontal=*/false);
                        RenderToolStripCurve(e);
                    }
                }
            }
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderGrip"]/*' />
        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderGrip(e);
                return;
            }

            ScaleObjectSizesIfNeeded();

            Graphics g = e.Graphics;
            Rectangle bounds = e.GripBounds;
            ToolStrip toolStrip = e.ToolStrip;

            bool rightToLeft = (e.ToolStrip.RightToLeft == RightToLeft.Yes);

            int height = (toolStrip.Orientation == Orientation.Horizontal) ? bounds.Height : bounds.Width;
            int width = (toolStrip.Orientation == Orientation.Horizontal) ? bounds.Width : bounds.Height;
            
            int numRectangles =  (height - (gripPadding * 2)) / 4;
            

            if (numRectangles > 0) {
                // a MenuStrip starts its grip lower and has fewer grip rectangles.
                int yOffset =  (toolStrip is MenuStrip) ? 2 : 0;
                
                Rectangle[] shadowRects = new Rectangle[numRectangles];
                int startY = gripPadding + 1 + yOffset;
                int startX = (width / 2);

                for (int i = 0; i < numRectangles; i++) {
                    shadowRects[i] = (toolStrip.Orientation == Orientation.Horizontal) ?
                                        new Rectangle(startX, startY, 2, 2) :
                                        new Rectangle(startY, startX, 2,2);
                                        
                    startY += 4;
                }

                // in RTL the GripLight rects should paint to the left of the GripDark rects.
                int xOffset = (rightToLeft) ? 1 : -1;
        
                if (rightToLeft) { 
                    // scoot over the rects in RTL so they fit within the bounds.
                    for (int i = 0; i < numRectangles; i++) {
                        shadowRects[i].Offset(-xOffset, 0);
                    }
                }

                using (Brush b = new SolidBrush(ColorTable.GripLight)) {
                    g.FillRectangles(b, shadowRects);
                }

                for (int i = 0; i < numRectangles; i++) {
                    shadowRects[i].Offset(xOffset, -1);
                }

                using (Brush b = new SolidBrush(ColorTable.GripDark)) {
                    g.FillRectangles(b, shadowRects);
                }
            }
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderMenuItem"]/*' />
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderMenuItemBackground(e);
                return;
            }

            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);

            if ((bounds.Width == 0) || (bounds.Height == 0)) {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            if (item is MdiControlStrip.SystemMenuItem) {
                return; // no highlights are painted behind a system menu item
            }


            if (item.IsOnDropDown) {
                ScaleObjectSizesIfNeeded();
                
                bounds = LayoutUtils.DeflateRect(bounds, scaledDropDownMenuItemPaintPadding);

                if (item.Selected) {
                    Color borderColor = ColorTable.MenuItemBorder;
                    if (item.Enabled) {
                        if (UseSystemColors) {
                           borderColor = SystemColors.Highlight;
                           RenderSelectedButtonFill(g, bounds);
                        }
                        else {
                            using (Brush b = new SolidBrush(ColorTable.MenuItemSelected)) {
                                g.FillRectangle(b, bounds);
                            }
                        }
                    }
                    // draw selection border - always drawn regardless of Enabled.
                    using (Pen p = new Pen(borderColor)) {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                }
                else {
                    Rectangle fillRect = bounds;
                    
                    if (item.BackgroundImage != null) {
                       ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                    }
                    else if (item.Owner != null && item.BackColor != item.Owner.BackColor) {
                        using (Brush b = new SolidBrush(item.BackColor)) {
                            g.FillRectangle(b, fillRect);
                        }
                    }
                }
            }
            else {
             
                if (item.Pressed) {
                    // Toplevel toolstrip rendering
                   RenderPressedGradient(g, bounds);
                    
                }
                else if (item.Selected) {
                    //Hot, Pressed behavior 
                    // Fill with orange
                    Color borderColor = ColorTable.MenuItemBorder;

                    if (item.Enabled) {

                        if (UseSystemColors) {
                           borderColor = SystemColors.Highlight;
                           RenderSelectedButtonFill(g, bounds);
                        }
                        else {
                            using (Brush b = new LinearGradientBrush(bounds, ColorTable.MenuItemSelectedGradientBegin, ColorTable.MenuItemSelectedGradientEnd, LinearGradientMode.Vertical)) {
                                g.FillRectangle(b, bounds);
                            }
                        }
                    }
               
                    // draw selection border - always drawn regardless of Enabled.
                    using (Pen p = new Pen(borderColor)) {
                        g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                    }
                }
                else {
                    Rectangle fillRect = bounds;
                    
                    if (item.BackgroundImage != null) {
                       ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
                    }
                    else if (item.Owner != null && item.BackColor != item.Owner.BackColor) {
                        using (Brush b = new SolidBrush(item.BackColor)) {
                            g.FillRectangle(b, fillRect);
                        }
                    }
                }
            }
            
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderArrow"]/*' />
        protected override void OnRenderArrow(ToolStripArrowRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderArrow(e);
                return;
            }

            ToolStripItem item = e.Item;

            if (item is ToolStripDropDownItem) {
                e.DefaultArrowColor = (item.Enabled) ? SystemColors.ControlText : SystemColors.ControlDark;
            }

            base.OnRenderArrow(e);
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderImageMargin"]/*' />
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderImageMargin(e);
                return;
            }

            ScaleObjectSizesIfNeeded();

            Graphics g = e.Graphics;
            Rectangle bounds = e.AffectedBounds;
            bounds.Y += 2;
            bounds.Height -= 4; /*shrink to accomodate 1PX line*/
            RightToLeft rightToLeft = e.ToolStrip.RightToLeft;
            Color begin = (rightToLeft == RightToLeft.No) ? ColorTable.ImageMarginGradientBegin :  ColorTable.ImageMarginGradientEnd;
            Color end = (rightToLeft == RightToLeft.No) ? ColorTable.ImageMarginGradientEnd :  ColorTable.ImageMarginGradientBegin;
            
            FillWithDoubleGradient(begin, ColorTable.ImageMarginGradientMiddle, end, e.Graphics, bounds, iconWellGradientWidth, iconWellGradientWidth, LinearGradientMode.Horizontal, /*flipHorizontal=*/(e.ToolStrip.RightToLeft == RightToLeft.Yes));
        }

        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderItemText"]/*' />
        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderItemText(e);
                return;
            }

            if (e.Item is ToolStripMenuItem && (e.Item.Selected || e.Item.Pressed)) {
                e.DefaultTextColor = e.Item.ForeColor;
            }

            base.OnRenderItemText(e);
        }
        
        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderItemCheck"]/*' />
        protected override void OnRenderItemCheck(ToolStripItemImageRenderEventArgs e)  {
            if (RendererOverride != null) {
                base.OnRenderItemCheck(e);
                return;
            }

            RenderCheckBackground(e);
            base.OnRenderItemCheck(e);
        }
        /// <include file='doc\ToolStripProfessionalRenderer.uex' path='docs/doc[@for="ToolStripProfessionalRenderer.OnRenderItemImage"]/*' />
        protected override void OnRenderItemImage(ToolStripItemImageRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderItemImage(e);
                return;
            }

            Rectangle imageRect = e.ImageRectangle;
            Image image = e.Image;

            if (e.Item is ToolStripMenuItem) {
                ToolStripMenuItem item = e.Item as ToolStripMenuItem;
                if (item.CheckState != CheckState.Unchecked) {
                    ToolStripDropDownMenu dropDownMenu = item.ParentInternal as  ToolStripDropDownMenu;
                    if (dropDownMenu != null && !dropDownMenu.ShowCheckMargin && dropDownMenu.ShowImageMargin) {
                         RenderCheckBackground(e);
                    }
                }
            }
    
            if (imageRect != Rectangle.Empty && image != null) {
                if (!e.Item.Enabled) {
                    base.OnRenderItemImage(e);
                    return;
                }

                // Since office images dont scoot one px we have to override all painting but enabled = false;
                if (e.Item.ImageScaling == ToolStripItemImageScaling.None) {
                    e.Graphics.DrawImage(image, imageRect, new Rectangle(Point.Empty, imageRect.Size), GraphicsUnit.Pixel);
                }
                else {
                    e.Graphics.DrawImage(image, imageRect);
                }
            }
        }

        protected override void OnRenderToolStripPanelBackground(ToolStripPanelRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderToolStripPanelBackground(e);
                return;
            }

            ToolStripPanel toolStripPanel = e.ToolStripPanel;
           
            if (!ShouldPaintBackground(toolStripPanel)) {
                return;
            }
            // dont paint background effects
            e.Handled = true;

            RenderBackgroundGradient(e.Graphics, toolStripPanel, ColorTable.ToolStripPanelGradientBegin, ColorTable.ToolStripPanelGradientEnd);
        }

        protected override void OnRenderToolStripContentPanelBackground(ToolStripContentPanelRenderEventArgs e) {
            if (RendererOverride != null) {
                base.OnRenderToolStripContentPanelBackground(e);
                return;
            }

            ToolStripContentPanel toolStripContentPanel = e.ToolStripContentPanel;

            if (!ShouldPaintBackground(toolStripContentPanel)) {
                return;
            }

            if( SystemInformation.InLockedTerminalSession() ) {
                return;
            }

            // dont paint background effects
            e.Handled = true;
            
            e.Graphics.Clear(ColorTable.ToolStripContentPanelGradientEnd);
            
    //        RenderBackgroundGradient(e.Graphics, toolStripContentPanel, ColorTable.ToolStripContentPanelGradientBegin, ColorTable.ToolStripContentPanelGradientEnd);
        }

		#region PrivatePaintHelpers 

        // consider make public
        internal override Region GetTransparentRegion(ToolStrip toolStrip) {
            if (toolStrip is ToolStripDropDown || toolStrip is MenuStrip || toolStrip is StatusStrip) {
                return null;
            }
            if (!RoundedEdges) {
                return null;
            }
            Rectangle bounds = new Rectangle(Point.Empty, toolStrip.Size);

            // Render curve
            // eat away at the corners by drawing the parent background
            // 
            if (toolStrip.ParentInternal != null) {
                //
                // Paint pieces of the parent here to give toolStrip rounded effect
                //
                Point topLeft = Point.Empty;
                Point topRight = new Point(bounds.Width - 1, 0);
                Point bottomLeft = new Point(0, bounds.Height - 1);
                Point bottomRight = new Point(bounds.Width - 1, bounds.Height - 1);

                // Pixels to eat away with the parent background
                // Grip side
                Rectangle topLeftParentHorizontalPixels = new Rectangle(topLeft, onePix);
                Rectangle bottomLeftParentHorizontalPixels = new Rectangle(bottomLeft, new Size(2, 1));
                Rectangle bottomLeftParentVerticalPixels = new Rectangle(bottomLeft.X, bottomLeft.Y - 1, 1, 2);

                // OverflowSide
                Rectangle bottomRightHorizontalPixels = new Rectangle(bottomRight.X - 1, bottomRight.Y, 2, 1);
                Rectangle bottomRightVerticalPixels = new Rectangle(bottomRight.X, bottomRight.Y - 1, 1, 2);

                // TopSide
                Rectangle topRightHorizontalPixels, topRightVerticalPixels;

                if (toolStrip.OverflowButton.Visible) {
                    topRightHorizontalPixels = new Rectangle(topRight.X - 1, topRight.Y, 1, 1);
                    topRightVerticalPixels = new Rectangle(topRight.X, topRight.Y, 1, 2);
                }
                else {
                    topRightHorizontalPixels = new Rectangle(topRight.X - 2, topRight.Y, 2, 1);
                    topRightVerticalPixels = new Rectangle(topRight.X, topRight.Y, 1, 3);
                }

                Region parentRegionToPaint = new Region(topLeftParentHorizontalPixels);
                parentRegionToPaint.Union(topLeftParentHorizontalPixels);
                parentRegionToPaint.Union(bottomLeftParentHorizontalPixels);
                parentRegionToPaint.Union(bottomLeftParentVerticalPixels);
                parentRegionToPaint.Union(bottomRightHorizontalPixels);
                parentRegionToPaint.Union(bottomRightVerticalPixels);
                parentRegionToPaint.Union(topRightHorizontalPixels);
                parentRegionToPaint.Union(topRightVerticalPixels);

                return parentRegionToPaint;
            }
            return null;
        }
    

        // </devdoc>
        // We want to make sure the overflow button looks like it's the last thing on 
        // the toolbar.  This touches up the few pixels that get clobbered by painting the
        // border.
        // </devdoc>
        private void RenderOverflowButtonEffectsOverBorder(ToolStripRenderEventArgs e) {
            
            ToolStrip toolStrip = e.ToolStrip;
            ToolStripItem item = toolStrip.OverflowButton;
            if (!item.Visible) {
                return;
            }
                
            Graphics g = e.Graphics;
            
            Color overflowBottomLeftShadow, overflowTopShadow;

            if (item.Pressed) {
                overflowBottomLeftShadow = ColorTable.ButtonPressedGradientBegin;
                overflowTopShadow = overflowBottomLeftShadow;
            }
            else if (item.Selected) {
                overflowBottomLeftShadow = ColorTable.ButtonSelectedGradientMiddle;
                overflowTopShadow = overflowBottomLeftShadow;
            }
            else {
                overflowBottomLeftShadow = ColorTable.ToolStripBorder;
                overflowTopShadow = ColorTable.ToolStripGradientMiddle;
            }

            // Extend the gradient color over the border.
            using (Brush b = new SolidBrush(overflowBottomLeftShadow)) {
                g.FillRectangle(b, toolStrip.Width - 1, toolStrip.Height - 2, 1, 1);
                g.FillRectangle(b, toolStrip.Width - 2, toolStrip.Height - 1, 1, 1);
            }


            using (Brush b = new SolidBrush(overflowTopShadow)) {
                g.FillRectangle(b, toolStrip.Width - 2, 0, 1, 1);
                g.FillRectangle(b, toolStrip.Width - 1, 1, 1, 1);
            }

        }

        ///<devdoc>
        ///  This function paints with three colors, beginning, middle, and end.
        ///   it paints:
        ///     (1)the entire bounds in the middle color
        ///     (2)gradient from beginning to middle of width firstGradientWidth
        ///     (3)gradient from middle to end of width secondGradientWidth
        ///      
        ///     if there isnt enough room to do (2) and (3) it merges into a single gradient from beginning to end.
        ///</devdoc>
        private void FillWithDoubleGradient(Color beginColor, Color middleColor, Color endColor, Graphics g, Rectangle bounds, int firstGradientWidth, int secondGradientWidth, LinearGradientMode mode, bool flipHorizontal) {
            if ((bounds.Width == 0) || (bounds.Height == 0)) {
                return;  // can't new up a linear gradient brush with no dimension.
            }
            Rectangle endGradient = bounds;
            Rectangle beginGradient = bounds;
            bool useDoubleGradient = true;

            if (mode == LinearGradientMode.Horizontal) {
                if (flipHorizontal) {
                    Color temp = endColor;
                    endColor = beginColor;
                    beginColor = temp;
                }
                
                beginGradient.Width = firstGradientWidth;
                endGradient.Width = secondGradientWidth + 1;
                endGradient.X = bounds.Right - endGradient.Width;
                useDoubleGradient = (bounds.Width > (firstGradientWidth + secondGradientWidth));
            }
            else {
                beginGradient.Height = firstGradientWidth;
                endGradient.Height = secondGradientWidth + 1;
                endGradient.Y = bounds.Bottom - endGradient.Height;
                useDoubleGradient = (bounds.Height > (firstGradientWidth + secondGradientWidth));
            }

            if (useDoubleGradient) {
                // Fill with middleColor
                using (Brush b = new SolidBrush(middleColor)) {
                    g.FillRectangle(b, bounds);
                }

                // draw first gradient
                using (Brush b = new LinearGradientBrush(beginGradient, beginColor, middleColor, mode)) {
                    g.FillRectangle(b, beginGradient);
                }

                // draw second gradient
                using (LinearGradientBrush b = new LinearGradientBrush(endGradient, middleColor, endColor, mode)) {
                    if (mode == LinearGradientMode.Horizontal) {
                        endGradient.X += 1;
                        endGradient.Width -= 1;
                    }
                    else {
                        endGradient.Y += 1;
                        endGradient.Height -= 1;
                    }

                    g.FillRectangle(b, endGradient);
                }
            }
            else {
                // not big enough for a swath in the middle.  lets just do a single gradient.
                using (Brush b = new LinearGradientBrush(bounds, beginColor, endColor, mode)) {
                    g.FillRectangle(b, bounds);
                }
            }
        }


        private void RenderStatusStripBorder(ToolStripRenderEventArgs e) {
             e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0,0,e.ToolStrip.Width, 0);     
        }
    
        private void RenderStatusStripBackground(ToolStripRenderEventArgs e) {
            StatusStrip statusStrip = e.ToolStrip as StatusStrip;
            RenderBackgroundGradient(e.Graphics, statusStrip, ColorTable.StatusStripGradientBegin, ColorTable.StatusStripGradientEnd, statusStrip.Orientation);
        }


        private void RenderCheckBackground(ToolStripItemImageRenderEventArgs e) {
            // 
            Rectangle bounds = DpiHelper.IsScalingRequired ? new Rectangle(e.ImageRectangle.Left-2, (e.Item.Height - e.ImageRectangle.Height )/2- 1, e.ImageRectangle.Width+4, e.ImageRectangle.Height +2) :
                                new Rectangle(e.ImageRectangle.Left - 2, 1, e.ImageRectangle.Width + 4, e.Item.Height - 2); 
            Graphics g = e.Graphics;

            if (!UseSystemColors) {
                Color fill = (e.Item.Selected) ? ColorTable.CheckSelectedBackground : ColorTable.CheckBackground;
                fill = (e.Item.Pressed) ? ColorTable.CheckPressedBackground : fill;
                using (Brush b = new SolidBrush(fill)) {
                    g.FillRectangle(b, bounds);
                }

                using (Pen p = new Pen(ColorTable.ButtonSelectedBorder)) {
                    g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
            else {
                if (e.Item.Pressed) {
                    RenderPressedButtonFill(g, bounds);
                }
                else {
                    RenderSelectedButtonFill(g, bounds);
                }
                g.DrawRectangle(SystemPens.Highlight, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }
        
        private void RenderPressedGradient(Graphics g, Rectangle bounds) {
            if ((bounds.Width == 0) || (bounds.Height == 0)) {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            // Paints a horizontal gradient similar to the image margin.
            using (Brush b = new LinearGradientBrush(bounds, ColorTable.MenuItemPressedGradientBegin, ColorTable.MenuItemPressedGradientEnd, LinearGradientMode.Vertical)) {
                g.FillRectangle(b, bounds);
            }

            // draw a box around the gradient
            using (Pen p = new Pen(ColorTable.MenuBorder)) {
                g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
            }
        }

        private void RenderMenuStripBackground(ToolStripRenderEventArgs e) {
            RenderBackgroundGradient(e.Graphics, e.ToolStrip, ColorTable.MenuStripGradientBegin, ColorTable.MenuStripGradientEnd, e.ToolStrip.Orientation);
        }

        private static void RenderLabelInternal(ToolStripItemRenderEventArgs e) {
            Graphics g = e.Graphics;
            ToolStripItem item = e.Item;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
        
            Rectangle fillRect = (item.Selected) ? item.ContentRectangle :bounds;
       
            if (item.BackgroundImage != null) {
               ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
            }

        }

        private void RenderBackgroundGradient(Graphics g, Control control, Color beginColor, Color endColor) {
            RenderBackgroundGradient(g,control,beginColor,endColor,Orientation.Horizontal);
        }
        // renders the overall gradient
        private void RenderBackgroundGradient(Graphics g, Control control, Color beginColor, Color endColor, Orientation orientation) {
        
           if (control.RightToLeft == RightToLeft.Yes) {
               Color temp = beginColor;
               beginColor = endColor;
               endColor = temp;
           }
  
           if (orientation == Orientation.Horizontal) {
               Control parent = control.ParentInternal;
               if (parent != null) {
                   Rectangle gradientBounds = new Rectangle(Point.Empty, parent.Size);
                   if (!LayoutUtils.IsZeroWidthOrHeight(gradientBounds)) {
                       using (LinearGradientBrush b = new LinearGradientBrush(gradientBounds, beginColor, endColor, LinearGradientMode.Horizontal)){
                           b.TranslateTransform(parent.Width - control.Location.X, parent.Height -control.Location.Y);
                           g.FillRectangle(b, new Rectangle(Point.Empty, control.Size));
                       }
                   }
               }
               else {
                   Rectangle gradientBounds = new Rectangle(Point.Empty, control.Size);
                   if (!LayoutUtils.IsZeroWidthOrHeight(gradientBounds)) {
                       // dont have a parent that we know about so go ahead and paint the gradient as if there wasnt another container.
                       using (LinearGradientBrush b = new LinearGradientBrush(gradientBounds, beginColor, endColor, LinearGradientMode.Horizontal)){
                           g.FillRectangle(b, gradientBounds);
                       }
                   }
               }
           }
           else {
               using (Brush b = new SolidBrush(beginColor)) {
                   g.FillRectangle(b, new Rectangle(Point.Empty, control.Size));
               }
           }
       }


        private void RenderToolStripBackgroundInternal(ToolStripRenderEventArgs e) {
            ScaleObjectSizesIfNeeded();

            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, e.ToolStrip.Size);

            // fill up the background
            LinearGradientMode mode = (toolStrip.Orientation == Orientation.Horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;
            FillWithDoubleGradient(ColorTable.ToolStripGradientBegin, ColorTable.ToolStripGradientMiddle, ColorTable.ToolStripGradientEnd, e.Graphics, bounds, iconWellGradientWidth, iconWellGradientWidth, mode, /*flipHorizontal=*/false);

        }

        private void RenderToolStripDropDownBackground(ToolStripRenderEventArgs e) {
            ToolStrip toolStrip = e.ToolStrip;
            Rectangle bounds = new Rectangle(Point.Empty, e.ToolStrip.Size);

            using (Brush b = new SolidBrush(ColorTable.ToolStripDropDownBackground)) {
                e.Graphics.FillRectangle(b, bounds);
            }
        }

        private void RenderToolStripDropDownBorder(ToolStripRenderEventArgs e) {
            ToolStripDropDown toolStripDropDown = e.ToolStrip as ToolStripDropDown;
            Graphics g = e.Graphics;

            if (toolStripDropDown != null) {
                Rectangle bounds = new Rectangle(Point.Empty, toolStripDropDown.Size);

                using (Pen p = new Pen(ColorTable.MenuBorder)) {
                    g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }

                if (!(toolStripDropDown is ToolStripOverflow)) {
                    // make the neck connected.
                    using (Brush b = new SolidBrush(ColorTable.ToolStripDropDownBackground)) {
                        g.FillRectangle(b, e.ConnectedArea);
                    }
                }
            }
        }

        private void RenderOverflowBackground(ToolStripItemRenderEventArgs e, bool rightToLeft) {
            ScaleObjectSizesIfNeeded();

            Graphics g = e.Graphics;
            ToolStripOverflowButton item = e.Item as ToolStripOverflowButton;
            Rectangle overflowBoundsFill = new Rectangle(Point.Empty, e.Item.Size);
            Rectangle bounds = overflowBoundsFill;
        
            bool drawCurve = (RoundedEdges && (!(item.GetCurrentParent() is MenuStrip)));
            bool horizontal = (e.ToolStrip.Orientation == Orientation.Horizontal);
            // undone RTL

            if (horizontal) {
                overflowBoundsFill.X += overflowBoundsFill.Width - overflowButtonWidth + 1;
                overflowBoundsFill.Width = overflowButtonWidth;
                if (rightToLeft) {
                    overflowBoundsFill = LayoutUtils.RTLTranslate(overflowBoundsFill,  bounds);
                }
            }
            else {
                overflowBoundsFill.Y = overflowBoundsFill.Height - overflowButtonWidth + 1;
                overflowBoundsFill.Height = overflowButtonWidth;
            }
            
            Color overflowButtonGradientBegin, overflowButtonGradientMiddle, overflowButtonGradientEnd, overflowBottomLeftShadow, overflowTopShadow;

            if (item.Pressed) {
                overflowButtonGradientBegin = ColorTable.ButtonPressedGradientBegin;
                overflowButtonGradientMiddle = ColorTable.ButtonPressedGradientMiddle;
                overflowButtonGradientEnd = ColorTable.ButtonPressedGradientEnd;
                overflowBottomLeftShadow = ColorTable.ButtonPressedGradientBegin;
                overflowTopShadow = overflowBottomLeftShadow;
            }
            else if (item.Selected) {
                overflowButtonGradientBegin = ColorTable.ButtonSelectedGradientBegin;
                overflowButtonGradientMiddle = ColorTable.ButtonSelectedGradientMiddle;
                overflowButtonGradientEnd = ColorTable.ButtonSelectedGradientEnd;
                overflowBottomLeftShadow = ColorTable.ButtonSelectedGradientMiddle;
                overflowTopShadow = overflowBottomLeftShadow;
            }
            else {
                overflowButtonGradientBegin = ColorTable.OverflowButtonGradientBegin;
                overflowButtonGradientMiddle = ColorTable.OverflowButtonGradientMiddle;
                overflowButtonGradientEnd = ColorTable.OverflowButtonGradientEnd;
                overflowBottomLeftShadow = ColorTable.ToolStripBorder;
                overflowTopShadow = (horizontal) ? ColorTable.ToolStripGradientMiddle : ColorTable.ToolStripGradientEnd;
            }

            if (drawCurve) {
                // draw shadow pixel on bottom left +1, +1
                using (Pen p = new Pen(overflowBottomLeftShadow/*Color.HotPink*/)) {
                   
                    Point start = new Point(overflowBoundsFill.Left - 1, overflowBoundsFill.Height - 2);
                    Point end = new Point(overflowBoundsFill.Left, overflowBoundsFill.Height - 2);
                    if (rightToLeft) {
                        start.X = overflowBoundsFill.Right +1;
                        end.X   = overflowBoundsFill.Right;
                    }
                    g.DrawLine(p, start, end);
                }
            }
            LinearGradientMode mode = (horizontal) ? LinearGradientMode.Vertical : LinearGradientMode.Horizontal;

            // fill main body 
            FillWithDoubleGradient(overflowButtonGradientBegin, overflowButtonGradientMiddle, overflowButtonGradientEnd, g, overflowBoundsFill, iconWellGradientWidth, iconWellGradientWidth, mode, false);

            // render shadow pixels (ToolStrip only)
            if (drawCurve) {
                // top left and top right shadow pixels 
                using (Brush b = new SolidBrush(overflowTopShadow/*Color.Orange*/)) {
                    if (horizontal) {
                        Point top1 = new Point(overflowBoundsFill.X - 2, 0);
                        Point top2 = new Point(overflowBoundsFill.X - 1, 1);

                        if (rightToLeft) {
                            top1.X = overflowBoundsFill.Right + 1;
                            top2.X = overflowBoundsFill.Right;
                        }
                        g.FillRectangle(b, top1.X, top1.Y, 1, 1);
                        g.FillRectangle(b, top2.X, top2.Y, 1, 1);
                    }
                    else {
                        g.FillRectangle(b, overflowBoundsFill.Width - 3, overflowBoundsFill.Top - 1, 1, 1);
                        g.FillRectangle(b, overflowBoundsFill.Width - 2, overflowBoundsFill.Top - 2, 1, 1);
                    }
                }

                using (Brush b = new SolidBrush(overflowButtonGradientBegin/*Color.Green*/)) {
                    if (horizontal) {
                        Rectangle fillRect = new Rectangle(overflowBoundsFill.X - 1, 0, 1, 1);
                        if (rightToLeft) {
                            fillRect.X = overflowBoundsFill.Right;
                        }
                        g.FillRectangle(b, fillRect);
                    }
                    else {
                        g.FillRectangle(b, overflowBoundsFill.X, overflowBoundsFill.Top - 1, 1, 1);
                    }
                }
            }
          
        }

        private void RenderToolStripCurve(ToolStripRenderEventArgs e) {
            Rectangle bounds = new Rectangle(Point.Empty, e.ToolStrip.Size);
            ToolStrip toolStrip = e.ToolStrip;
            Rectangle displayRect = toolStrip.DisplayRectangle;
          
            Graphics g = e.Graphics;

            Point topLeft = Point.Empty;
            Point topRight = new Point(bounds.Width - 1, 0);
            Point bottomLeft = new Point(0, bounds.Height - 1);

            //
            // Add in shadow pixels - the detail that makes them look round
            //
            // Draw in rounded shadow pixels on the top left & right
            // consider: if this is slow use precanned corners.
            using (Brush b = new SolidBrush(ColorTable.ToolStripGradientMiddle)) {
                // there are two shadow rects (one pixel wide) on the top
                Rectangle topLeftShadowRect = new Rectangle(topLeft, onePix);
                topLeftShadowRect.X += 1;

                // second shadow rect
                Rectangle topLeftShadowRect2 = new Rectangle(topLeft, onePix);
                topLeftShadowRect2.Y += 1;

                // on the right there are two more shadow rects
                Rectangle topRightShadowRect = new Rectangle(topRight, onePix);
                topRightShadowRect.X -= 2; // was 2?

                // second top right shadow pix
                Rectangle topRightShadowRect2 = topRightShadowRect;
                topRightShadowRect2.Y += 1;
                topRightShadowRect2.X += 1;

                Rectangle[] paintRects = new Rectangle[] { topLeftShadowRect, topLeftShadowRect2, topRightShadowRect, topRightShadowRect2 };

                // prevent the painting of anything that would obscure an item.
                for (int i = 0; i < paintRects.Length; i++) {
                    if (displayRect.IntersectsWith(paintRects[i])) {
                        paintRects[i] = Rectangle.Empty;
                    }                   
                }
                g.FillRectangles(b, paintRects);
            
            }

            // Draw in rounded shadow pixels on the bottom left
            using (Brush b = new SolidBrush(ColorTable.ToolStripGradientEnd)) {

                // this gradient is the one just before the dark shadow line starts on pixel #3.
                Point gradientCopyPixel = bottomLeft;
                gradientCopyPixel.Offset(1, -1);
                if (!displayRect.Contains(gradientCopyPixel)) {
                    g.FillRectangle(b, new Rectangle(gradientCopyPixel, onePix));                    
                }

                // set the one dark pixel in the bottom left hand corner
                Rectangle otherBottom = new Rectangle(bottomLeft.X, bottomLeft.Y - 2, 1, 1);
                if (!displayRect.IntersectsWith(otherBottom)) {            
                    g.FillRectangle(b, otherBottom);
                 }
            }
        }

        private void RenderSelectedButtonFill(Graphics g, Rectangle bounds) {
            if ((bounds.Width == 0) || (bounds.Height == 0)) {
                return;  // can't new up a linear gradient brush with no dimension.
            }

            if (!UseSystemColors) {
                using (Brush b = new LinearGradientBrush(bounds, ColorTable.ButtonSelectedGradientBegin, ColorTable.ButtonSelectedGradientEnd, LinearGradientMode.Vertical)) {
                    g.FillRectangle(b, bounds);
                }
            }
            else {
                Color fillColor = ColorTable.ButtonSelectedHighlight;
                using (Brush b = new SolidBrush(fillColor)) {
                    g.FillRectangle(b, bounds);
                }
            }
        }
        private void RenderCheckedButtonFill(Graphics g, Rectangle bounds) {
              if ((bounds.Width == 0) || (bounds.Height == 0)) {
                  return;  // can't new up a linear gradient brush with no dimension.
              }
        
              if (!UseSystemColors) {
                  using (Brush b = new LinearGradientBrush(bounds, ColorTable.ButtonCheckedGradientBegin, ColorTable.ButtonCheckedGradientEnd, LinearGradientMode.Vertical)) {
                      g.FillRectangle(b, bounds);
                  }
              }
              else {
        
                  Color fillColor = ColorTable.ButtonCheckedHighlight;
                  
                  using (Brush b = new SolidBrush(fillColor)) {
                      g.FillRectangle(b, bounds);
        
                  }
              }
          }

        private void RenderSeparatorInternal(Graphics g, ToolStripItem item, Rectangle bounds, bool vertical) {
            Color foreColor = ColorTable.SeparatorDark;
            Color highlightColor = ColorTable.SeparatorLight;
            Pen foreColorPen = new Pen(foreColor);
            Pen highlightColorPen = new Pen(highlightColor);

            // undone emplore caching.
            bool disposeForeColorPen = true;
            bool disposeHighlightColorColorPen = true;
            bool isASeparator = item is ToolStripSeparator;
            bool isAHorizontalSeparatorNotOnDropDownMenu = false;
            
            if (isASeparator) {
                if (vertical) {
                    if (!item.IsOnDropDown) {
                        // center so that it matches office
                        bounds.Y +=3;
                        bounds.Height = Math.Max(0, bounds.Height -6);
                    }
                }
                else {
                    // offset after the image margin
                    ToolStripDropDownMenu dropDownMenu = item.GetCurrentParent() as ToolStripDropDownMenu;
                    if (dropDownMenu != null) {
                        if (dropDownMenu.RightToLeft == RightToLeft.No) {
                            // scoot over by the padding (that will line you up with the text - but go two PX before so that it visually looks
                            // like the line meets up with the text).
                            bounds.X += dropDownMenu.Padding.Left -2;
                            bounds.Width = dropDownMenu.Width - bounds.X; 
                        }
                        else {
                           // scoot over by the padding (that will line you up with the text - but go two PX before so that it visually looks
                           // like the line meets up with the text).
                           bounds.X += 2;
                           bounds.Width = dropDownMenu.Width - bounds.X - dropDownMenu.Padding.Right; 

                        }
                    }
                    else {
                        isAHorizontalSeparatorNotOnDropDownMenu = true;

                    }
                }
            }
            try {
                if (vertical) {
                    if (bounds.Height >= 4) {
                        bounds.Inflate(0, -2);     // scoot down 2PX and start drawing
                    }

                    bool rightToLeft = (item.RightToLeft == RightToLeft.Yes);
                    Pen leftPen = (rightToLeft) ? highlightColorPen : foreColorPen;
                    Pen rightPen = (rightToLeft) ? foreColorPen : highlightColorPen;

                    // Draw dark line
                    int startX = bounds.Width / 2;

                    g.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom - 1);

                    // Draw highlight one pixel to the right
                    startX++;
                    g.DrawLine(rightPen, startX, bounds.Top + 1, startX, bounds.Bottom);
                }
                else {
                    //
                    // horizontal separator
                    // Draw dark line

                    if (isAHorizontalSeparatorNotOnDropDownMenu && bounds.Width >= 4) {
                        bounds.Inflate(-2, 0);     // scoot down 2PX and start drawing
                    }
                    int startY = bounds.Height / 2;

                    g.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right - 1, startY);

                    if (!isASeparator || isAHorizontalSeparatorNotOnDropDownMenu) {
                        // Draw highlight one pixel to the right
                        startY++;
                        g.DrawLine(highlightColorPen, bounds.Left + 1, startY, bounds.Right - 1, startY);
                    }
                }
            }
            finally {
                if (disposeForeColorPen && foreColorPen != null) {
                    foreColorPen.Dispose();
                }

                if (disposeHighlightColorColorPen && highlightColorPen != null) {
                    highlightColorPen.Dispose();
                }
            }
        }

        private void RenderPressedButtonFill(Graphics g, Rectangle bounds) {

            if ((bounds.Width == 0) || (bounds.Height == 0)) {
                return;  // can't new up a linear gradient brush with no dimension.
            }
            if (!UseSystemColors) {
                using (Brush b = new LinearGradientBrush(bounds, ColorTable.ButtonPressedGradientBegin, ColorTable.ButtonPressedGradientEnd, LinearGradientMode.Vertical)) {
                    g.FillRectangle(b, bounds);
                }
            }
            else {
                
                Color fillColor = ColorTable.ButtonPressedHighlight;
                using (Brush b = new SolidBrush(fillColor)) {
                    g.FillRectangle(b, bounds);
                }
            }
        }

        private void RenderItemInternal(ToolStripItemRenderEventArgs e, bool useHotBorder) {
            Graphics g = e.Graphics;
            ToolStripItem item = e.Item;
            Rectangle bounds = new Rectangle(Point.Empty, item.Size);
            bool drawHotBorder = false;

            Rectangle fillRect = (item.Selected) ? item.ContentRectangle :bounds;

            if (item.BackgroundImage != null) {
               ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, bounds, fillRect);
            }

            if (item.Pressed) {
                RenderPressedButtonFill(g, bounds);
                drawHotBorder = useHotBorder;
            }
            else if (item.Selected) {
                RenderSelectedButtonFill(g, bounds);
                drawHotBorder = useHotBorder;
            }
            else if (item.Owner != null && item.BackColor != item.Owner.BackColor) {
                using (Brush b = new SolidBrush(item.BackColor)) {
                    g.FillRectangle(b, bounds);
                }
            }

            if (drawHotBorder) {
                using (Pen p = new Pen(ColorTable.ButtonSelectedBorder)) {
                    g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                }
            }
        }

        private void ScaleObjectSizesIfNeeded() {
            if (isScalingInitialized) {
                return;
            }          
            if (DpiHelper.IsScalingRequired) {
                ToolStripRenderer.ScaleArrowOffsetsIfNeeded();
                overflowButtonWidth = DpiHelper.LogicalToDeviceUnitsX(OVERFLOW_BUTTON_WIDTH);
                overflowArrowWidth = DpiHelper.LogicalToDeviceUnitsX(OVERFLOW_ARROW_WIDTH);
                overflowArrowHeight = DpiHelper.LogicalToDeviceUnitsY(OVERFLOW_ARROW_HEIGHT);
                overflowArrowOffsetY = DpiHelper.LogicalToDeviceUnitsY(OVERFLOW_ARROW_OFFSETY);

                if (DpiHelper.IsScalingRequirementMet) {
                    gripPadding = DpiHelper.LogicalToDeviceUnitsY(GRIP_PADDING);
                    iconWellGradientWidth = DpiHelper.LogicalToDeviceUnitsX(ICON_WELL_GRADIENT_WIDTH);
                    int scaledSize = DpiHelper.LogicalToDeviceUnitsX(DROP_DOWN_MENU_ITEM_PAINT_PADDING_SIZE);
                    scaledDropDownMenuItemPaintPadding = new Padding(scaledSize + 1, 0, scaledSize, 0);
                }
            }
            isScalingInitialized = true;
        }

        // This draws differently sized arrows than the base one... 
        // used only for drawing the overflow button madness.
        private Point RenderArrowInternal(Graphics g, Rectangle dropDownRect, ArrowDirection direction, Brush brush) {

            Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);

            // if the width is odd - favor pushing it over one pixel right.
            middle.X += (dropDownRect.Width % 2);

            Point[] arrow = null;

            switch (direction) {
                case ArrowDirection.Up:
                    arrow = new Point[] {
                        new Point(middle.X - ToolStripRenderer.Offset2X, middle.Y + 1), 
                        new Point(middle.X + ToolStripRenderer.Offset2X + 1, middle.Y + 1), 
                        new Point(middle.X, middle.Y - ToolStripRenderer.Offset2Y)
                    };
                    break;

                case ArrowDirection.Left:
                    arrow = new Point[] {
                        new Point(middle.X + ToolStripRenderer.Offset2X, middle.Y - ToolStripRenderer.Offset2Y - 1), 
                        new Point(middle.X + ToolStripRenderer.Offset2X, middle.Y + ToolStripRenderer.Offset2Y + 1), 
                        new Point(middle.X - 1, middle.Y)
                    };
                    break;

                case ArrowDirection.Right:
                    arrow = new Point[] {
                        new Point(middle.X - ToolStripRenderer.Offset2X, middle.Y - ToolStripRenderer.Offset2Y - 1), 
                        new Point(middle.X - ToolStripRenderer.Offset2X, middle.Y + ToolStripRenderer.Offset2Y + 1), 
                        new Point(middle.X + 1, middle.Y)
                    };
                    break;

                case ArrowDirection.Down:
                default:
                    arrow = new Point[] {
                        new Point(middle.X - ToolStripRenderer.Offset2X, middle.Y - 1), 
                        new Point(middle.X + ToolStripRenderer.Offset2X + 1, middle.Y - 1), 
                        new Point(middle.X, middle.Y + ToolStripRenderer.Offset2Y)
                    };
                    break;
            }
            g.FillPolygon(brush, arrow);

            return middle;
        }

        #endregion PrivatePaintHelpers
    }
}
