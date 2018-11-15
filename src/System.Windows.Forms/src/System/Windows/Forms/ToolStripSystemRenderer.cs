// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Windows.Forms.VisualStyles;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Imaging;
    using System.ComponentModel;
    using System.Windows.Forms.Layout;
    
    /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer"]/*' />
    public class ToolStripSystemRenderer : ToolStripRenderer {
    
        [ThreadStatic()]
        private static VisualStyleRenderer renderer = null; 
        private ToolStripRenderer toolStripHighContrastRenderer;

        public ToolStripSystemRenderer() {
        }
        internal ToolStripSystemRenderer(bool isDefault) : base(isDefault) {
        }
        internal override ToolStripRenderer RendererOverride {
              get {
                  if (DisplayInformation.HighContrast) {
                      return HighContrastRenderer;
                  }
                  return null;
              }
          }
        
        internal ToolStripRenderer HighContrastRenderer {
            get {
                if (toolStripHighContrastRenderer == null) {
                    toolStripHighContrastRenderer = new ToolStripHighContrastRenderer(/*renderLikeSystem*/true);
                }
                return toolStripHighContrastRenderer;
            }
        }


        /// <devdoc>
        ///     Draw the background color
        /// </devdoc>
        private static VisualStyleRenderer VisualStyleRenderer {
            get {  
                if (Application.RenderWithVisualStyles) {
                    if (renderer == null && VisualStyleRenderer.IsElementDefined(VisualStyleElement.ToolBar.Button.Normal)) {
                        renderer = new VisualStyleRenderer(VisualStyleElement.ToolBar.Button.Normal);
                    }
                }
                else {
                    renderer = null;
                }
                return renderer;
            }

        }

        /// <devdoc>
        ///     Fill the item's background as bounded by the rectangle
        /// </devdoc>
        private static void FillBackground(Graphics g, Rectangle bounds, Color backColor) {
            // Fill the background with the item's back color
            if (backColor.IsSystemColor) {
                g.FillRectangle(SystemBrushes.FromSystemColor(backColor), bounds);
            }
            else {           
                using (Brush backBrush = new SolidBrush(backColor)) {
                    g.FillRectangle(backBrush, bounds);
                }
            }
        }

        /// <devdoc>
        ///     returns true if you are required to dispose the pen
        /// </devdoc>
        private static bool GetPen(Color color, ref Pen pen) {
              if (color.IsSystemColor) {
                  pen = SystemPens.FromSystemColor(color);
                  return false;
              }
              else{
                  pen = new Pen(color);
                  return true;
              }
        }
        /// <devdoc>
        ///     translates the winbar item state into a toolbar state, which is something the renderer understands
        /// </devdoc>
        private static int GetItemState(ToolStripItem item) {
            return (int)GetToolBarState(item);
        }
        /// <devdoc>
        ///     translates the winbar item state into a toolbar state, which is something the renderer understands
        /// </devdoc>
        private static int GetSplitButtonDropDownItemState(ToolStripSplitButton item) {
            return (int)GetSplitButtonToolBarState(item, true);
        }
        /// <devdoc>
        ///     translates the winbar item state into a toolbar state, which is something the renderer understands
        /// </devdoc>
        private static int GetSplitButtonItemState(ToolStripSplitButton item) {
            return (int)GetSplitButtonToolBarState(item, false);
        }
        /// <devdoc>
        ///     translates the winbar item state into a toolbar state, which is something the renderer understands
        /// </devdoc>
        private static ToolBarState GetSplitButtonToolBarState(ToolStripSplitButton button, bool dropDownButton) {
            ToolBarState state  =  ToolBarState.Normal;
            
            if (button != null) {
                if (!button.Enabled) {
                    state = ToolBarState.Disabled;
                }
                else if (dropDownButton){
                    if (button.DropDownButtonPressed || button.ButtonPressed) {
                        state = ToolBarState.Pressed;
                    }
                    else if (button.DropDownButtonSelected || button.ButtonSelected) {
                       state = ToolBarState.Hot;
                    }
                }
                else{
                    if (button.ButtonPressed) {
                        state = ToolBarState.Pressed;
                    }
                    else if (button.ButtonSelected) {
                       state = ToolBarState.Hot;
                    }
                }
            }
            return state;
        }

        /// <devdoc>
        ///     translates the winbar item state into a toolbar state, which is something the renderer understands
        /// </devdoc>
        private static ToolBarState GetToolBarState(ToolStripItem item) {
            ToolBarState state  =  ToolBarState.Normal;
            if (item != null) {
                if (!item.Enabled) {
                    state = ToolBarState.Disabled;
                }
                if (item is ToolStripButton && ((ToolStripButton)item).Checked) {
                    if (((ToolStripButton)item).Selected && AccessibilityImprovements.Level1) {
                        state = ToolBarState.Hot; // we'd prefer HotChecked here, but Color Theme uses the same color as Checked
                    }
                    else {
                        state = ToolBarState.Checked;
                    }
                }
                else if (item.Pressed) {
                    state = ToolBarState.Pressed;
                }
                else if (item.Selected) {
                   state = ToolBarState.Hot;
                }
            }
            return state;
        }   
  

                
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderBackground"]/*' />
        /// <devdoc>
        /// Draw the winbar background.  ToolStrip users should override this if they want to draw differently.
        /// </devdoc>
        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e) {  
            ToolStrip toolStrip = e.ToolStrip;
            Graphics g = e.Graphics;
            Rectangle bounds=  e.AffectedBounds;

            if (!ShouldPaintBackground(toolStrip)) {
                return;
            }

            if (toolStrip is StatusStrip) {
                RenderStatusStripBackground(e);
            }
            else {
                if (DisplayInformation.HighContrast) {
                    FillBackground(g, bounds, SystemColors.ButtonFace);
                }
                else if (DisplayInformation.LowResolution) {
                    FillBackground(g, bounds, (toolStrip is ToolStripDropDown) ? SystemColors.ControlLight : e.BackColor);
                }
                else if (toolStrip.IsDropDown) {
                    FillBackground(g, bounds, (!ToolStripManager.VisualStylesEnabled) ? 
                                         e.BackColor : SystemColors.Menu);
                }
                else if (toolStrip is MenuStrip) {
                    FillBackground(g, bounds, (!ToolStripManager.VisualStylesEnabled) ?
                                               e.BackColor : SystemColors.MenuBar);
                }
                else if (ToolStripManager.VisualStylesEnabled && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Band.Normal)) {
                    VisualStyleRenderer vsRenderer = VisualStyleRenderer;

                    vsRenderer.SetParameters(VisualStyleElement.ToolBar.Bar.Normal);
                    vsRenderer.DrawBackground(g, bounds);
                }
                else {
                    FillBackground(g, bounds, (!ToolStripManager.VisualStylesEnabled) ?
                                               e.BackColor : SystemColors.MenuBar);
                }
            }

        }
        
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderBorder"]/*' />
        /// <devdoc>
        /// Draw the border around the ToolStrip.  This should be done as the last step.
        /// </devdoc>
        protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e) {  
           ToolStrip toolStrip = e.ToolStrip;
           Graphics g = e.Graphics;
           Rectangle bounds = e.ToolStrip.ClientRectangle;

           if (toolStrip is StatusStrip) {
                RenderStatusStripBorder(e);
           }
           else if (toolStrip is ToolStripDropDown) {
                ToolStripDropDown toolStripDropDown = toolStrip as ToolStripDropDown;
               
                // Paint the border for the window depending on whether or not we have a drop shadow effect.
                if (toolStripDropDown.DropShadowEnabled && ToolStripManager.VisualStylesEnabled) {
                    bounds.Width -= 1;
                    bounds.Height -= 1;
                    e.Graphics.DrawRectangle(new Pen(SystemColors.ControlDark), bounds);
                }
                else {
                    ControlPaint.DrawBorder3D(e.Graphics, bounds, Border3DStyle.Raised);
                }
           }
           else {                
               if (ToolStripManager.VisualStylesEnabled) {
                   e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0,bounds.Bottom-1,bounds.Width, bounds.Bottom-1);
                   e.Graphics.DrawLine(SystemPens.InactiveBorder, 0,bounds.Bottom-2,bounds.Width,bounds.Bottom-2);
               }
               else {
                   e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0,bounds.Bottom-1,bounds.Width, bounds.Bottom-1);
                   e.Graphics.DrawLine(SystemPens.ButtonShadow, 0,bounds.Bottom-2,bounds.Width,bounds.Bottom-2);
               }
           }
        }
        
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderGrip"]/*' />
        /// <devdoc>
        /// Draw the grip.  ToolStrip users should override this if they want to draw differently.
        /// </devdoc>
        protected override void OnRenderGrip(ToolStripGripRenderEventArgs e) {
            
            Graphics g = e.Graphics;
            Rectangle bounds = new Rectangle(Point.Empty, e.GripBounds.Size);
            bool verticalGrip = e.GripDisplayStyle == ToolStripGripDisplayStyle.Vertical;
               
            if (ToolStripManager.VisualStylesEnabled  && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Gripper.Normal)) {

                VisualStyleRenderer vsRenderer = VisualStyleRenderer;
        
                if (verticalGrip) {
                    vsRenderer.SetParameters(VisualStyleElement.Rebar.Gripper.Normal);  
                   
                    bounds.Height = ((bounds.Height -2/*number of pixels for border*/) / 4) * 4; // make sure height is an even interval of 4. 
                    bounds.Y = Math.Max(0,(e.GripBounds.Height - bounds.Height -2/*number of pixels for border*/) / 2);
                }
                else {
                    vsRenderer.SetParameters(VisualStyleElement.Rebar.GripperVertical.Normal);    
                }
                vsRenderer.DrawBackground(g, bounds);
            }
            else {
                // do some fixup so that we dont paint from end to end.
                Color backColor = e.ToolStrip.BackColor;
                FillBackground(g, bounds, backColor);

                if (verticalGrip) {
                    if (bounds.Height >= 4) {
                      bounds.Inflate(0,-2);     // scoot down 2PX and start drawing
                    }
                    bounds.Width = 3;
                }
                else {
                    if (bounds.Width >= 4) {
                      bounds.Inflate(-2,0);        // scoot over 2PX and start drawing
                    }
                    bounds.Height = 3;
                }
          
                RenderSmall3DBorderInternal(g, bounds, ToolBarState.Hot, (e.ToolStrip.RightToLeft == RightToLeft.Yes));
               
            }
        }

        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderItem"]/*' />
        /// <devdoc>
        /// Draw the items background
        /// </devdoc>
        protected override void OnRenderItemBackground(ToolStripItemRenderEventArgs e) {
        }

        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderImageMargin"]/*' />
        /// <devdoc>
        /// Draw the items background
        /// </devdoc>
        protected override void OnRenderImageMargin(ToolStripRenderEventArgs e) {
        }
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderButton"]/*' />
        /// <devdoc>
        /// Draw the button background
        /// </devdoc>
        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e) {
            RenderItemInternal(e);
        }
   
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderDropDownButton"]/*' />
        /// <devdoc>
        /// Draw the button background
        /// </devdoc>
        protected override void OnRenderDropDownButtonBackground(ToolStripItemRenderEventArgs e) {
            RenderItemInternal(e);
        }

        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderOverflowButton"]/*' />
        /// <devdoc>
        /// Draw the button background
        /// </devdoc>
        protected override void OnRenderOverflowButtonBackground(ToolStripItemRenderEventArgs e) {
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
           
                
            if (ToolStripManager.VisualStylesEnabled && VisualStyleRenderer.IsElementDefined(VisualStyleElement.Rebar.Chevron.Normal)) {
                VisualStyleElement chevronElement = VisualStyleElement.Rebar.Chevron.Normal;
                VisualStyleRenderer vsRenderer = VisualStyleRenderer;
                vsRenderer.SetParameters(chevronElement.ClassName, chevronElement.Part, GetItemState(item));
                vsRenderer.DrawBackground(g, new Rectangle(Point.Empty, item.Size));
            }
            else {
                RenderItemInternal(e);
                Color arrowColor =  item.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;

                DrawArrow(new ToolStripArrowRenderEventArgs(g, item, new Rectangle(Point.Empty, item.Size), arrowColor, ArrowDirection.Down));
            } 
        }
       
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderLabel"]/*' />
        /// <devdoc>
        /// Draw the button background
        /// </devdoc>
        protected override void OnRenderLabelBackground(ToolStripItemRenderEventArgs e) {
            RenderLabelInternal(e);
        }
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderMenuItem"]/*' />
        /// <devdoc>
        /// Draw the items background
        /// </devdoc>
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e) {
         
           ToolStripMenuItem item = e.Item as ToolStripMenuItem;
           Graphics g = e.Graphics;
           
           if (item is MdiControlStrip.SystemMenuItem) {
               return; // no highlights are painted behind a system menu item
           }

           // 
           
           if (item != null) {
               Rectangle bounds = new Rectangle(Point.Empty, item.Size);
               if (item.IsTopLevel && !ToolStripManager.VisualStylesEnabled) {
                    // CLASSIC MODE (3D edges)
                    // Draw box highlight for toplevel items in downlevel platforms
                    if (item.BackgroundImage != null) {
                        ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, item.ContentRectangle, item.ContentRectangle);
                    }
                    else if (item.RawBackColor != Color.Empty) {                        
                        FillBackground(g, item.ContentRectangle, item.BackColor);
                    }
                    // Toplevel menu items do 3D borders.
                    ToolBarState state = GetToolBarState(item);
                    RenderSmall3DBorderInternal(g, bounds, state, (item.RightToLeft == RightToLeft.Yes));
               }
               else {
                   // XP++ MODE (no 3D edges)
                   // Draw blue filled highlight for toplevel items in themed platforms
                   // or items parented to a drop down
                   Rectangle fillRect = new Rectangle(Point.Empty, item.Size);
                   if (item.IsOnDropDown) {
                       // Scoot in by 2 pixels when selected
                       fillRect.X += 2;
                       fillRect.Width -= 3; //its already 1 away from the right edge
                   }

                    if (item.Selected || item.Pressed) {
                        // Legacy behavior is to always paint the menu item background.
                        // The correct behavior is to only paint the background if the menu item is
                        // enabled.
                        if (!AccessibilityImprovements.Level1 || item.Enabled) {
                            g.FillRectangle(SystemBrushes.Highlight, fillRect);
                        }

                        if (AccessibilityImprovements.Level1) {
                            Color borderColor = ToolStripManager.VisualStylesEnabled ?
                                SystemColors.Highlight : ProfessionalColors.MenuItemBorder;

                            // draw selection border - always drawn regardless of Enabled.
                            using (Pen p = new Pen(borderColor)) {
                                g.DrawRectangle(p, bounds.X, bounds.Y, bounds.Width - 1, bounds.Height - 1);
                            }
                        }
                    }
                    else {
                        if (item.BackgroundImage != null) {
                            ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, item.ContentRectangle, fillRect);
                        }
                        else if (!ToolStripManager.VisualStylesEnabled && (item.RawBackColor != Color.Empty)) {
                            FillBackground(g, fillRect, item.BackColor);
                        }
                    }
                }

            }
           
        }
        
        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderSeparator"]/*' />
        /// <devdoc>
        /// Draws a toolbar separator. ToolStrip users should override this function to change the 
        /// drawing of all separators.
        /// </devdoc>
        protected override void OnRenderSeparator(ToolStripSeparatorRenderEventArgs e) {
           RenderSeparatorInternal(e.Graphics, e.Item, new Rectangle(Point.Empty, e.Item.Size), e.Vertical);
        }

        
        /// <include file='doc\WinBarRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderToolStripStatusLabel"]/*' />
        protected override void OnRenderToolStripStatusLabelBackground(ToolStripItemRenderEventArgs e) {
            RenderLabelInternal(e);
            ToolStripStatusLabel item = e.Item as ToolStripStatusLabel;

            ControlPaint.DrawBorder3D(e.Graphics, new Rectangle(0,0,item.Width-1, item.Height-1), item.BorderStyle, (Border3DSide)item.BorderSides);
        }

        /// <include file='doc\ToolStripRenderer.uex' path='docs/doc[@for="ToolStripRenderer.OnRenderSplitButton"]/*' />
        /// <devdoc>
        /// Draw the item's background.
        /// </devdoc>
        protected override void OnRenderSplitButtonBackground(ToolStripItemRenderEventArgs e) {

            ToolStripSplitButton splitButton = e.Item as ToolStripSplitButton;
            Graphics g = e.Graphics;

            bool rightToLeft = (splitButton.RightToLeft == RightToLeft.Yes);
            Color arrowColor =  splitButton.Enabled ? SystemColors.ControlText : SystemColors.ControlDark;

       
            // in right to left - we need to swap the parts so we dont draw  v][ toolStripSplitButton
            VisualStyleElement splitButtonDropDownPart = (rightToLeft) ? VisualStyleElement.ToolBar.SplitButton.Normal : VisualStyleElement.ToolBar.SplitButtonDropDown.Normal;
            VisualStyleElement splitButtonPart         = (rightToLeft) ? VisualStyleElement.ToolBar.DropDownButton.Normal : VisualStyleElement.ToolBar.SplitButton.Normal;
       
            Rectangle bounds =  new Rectangle(Point.Empty, splitButton.Size);
            if (ToolStripManager.VisualStylesEnabled
                && VisualStyleRenderer.IsElementDefined(splitButtonDropDownPart)
                && VisualStyleRenderer.IsElementDefined(splitButtonPart)) {

                  VisualStyleRenderer vsRenderer = VisualStyleRenderer;
          
                   
                  // Draw the SplitButton Button portion of it.
                  vsRenderer.SetParameters(splitButtonPart.ClassName, splitButtonPart.Part, GetSplitButtonItemState(splitButton));


                  // the lovely Windows theming for split button comes in three pieces:
                  //  SplitButtonDropDown: [ v |
                  //  Separator:                |
                  //  SplitButton:               |  ]
                  // this is great except if you want to swap the button in RTL.  In this case we need
                  // to use the DropDownButton instead of the SplitButtonDropDown and paint the arrow ourselves.
                  Rectangle splitButtonBounds = splitButton.ButtonBounds;
                  if (rightToLeft) {
                        // scoot to the left so we dont draw double shadow like so: ][
                        splitButtonBounds.Inflate(2,0);
                  }
                  // Draw the button portion of it.
                  vsRenderer.DrawBackground(g, splitButtonBounds);

                  // Draw the SplitButton DropDownButton portion of it.
                  vsRenderer.SetParameters(splitButtonDropDownPart.ClassName, splitButtonDropDownPart.Part, GetSplitButtonDropDownItemState(splitButton));

                  // Draw the drop down button portion
                  vsRenderer.DrawBackground(g, splitButton.DropDownButtonBounds);

                  // fill in the background image
                  Rectangle fillRect =  splitButton.ContentRectangle;
                  if (splitButton.BackgroundImage != null) {
                     ControlPaint.DrawBackgroundImage(g, splitButton.BackgroundImage, splitButton.BackColor, splitButton.BackgroundImageLayout, fillRect, fillRect);
                  }

                  // draw the separator over it.
                  RenderSeparatorInternal(g,splitButton, splitButton.SplitterBounds, true);

                  // and of course, now if we're in RTL we now need to paint the arrow
                  // because we're no longer using a part that has it built in.
                  if (rightToLeft || splitButton.BackgroundImage != null) {
                       DrawArrow(new ToolStripArrowRenderEventArgs(g, splitButton, splitButton.DropDownButtonBounds, arrowColor, ArrowDirection.Down));
                  }
            }
            else {
            
                // Draw the split button button
                Rectangle splitButtonButtonRect = splitButton.ButtonBounds;

                if (splitButton.BackgroundImage != null) {
                     // fill in the background image
                    Rectangle fillRect = (splitButton.Selected) ? splitButton.ContentRectangle :bounds;
                    if (splitButton.BackgroundImage != null) {
                       ControlPaint.DrawBackgroundImage(g, splitButton.BackgroundImage, splitButton.BackColor, splitButton.BackgroundImageLayout, bounds, fillRect);
                    }
                }
                else {
                    FillBackground(g,splitButtonButtonRect,  splitButton.BackColor);
                }
                
                ToolBarState state = GetSplitButtonToolBarState(splitButton, false);

                RenderSmall3DBorderInternal(g, splitButtonButtonRect, state, rightToLeft);
                
                // draw the split button drop down
                Rectangle dropDownRect = splitButton.DropDownButtonBounds;

                // fill the color in the dropdown button
                if (splitButton.BackgroundImage == null) {
                    FillBackground(g, dropDownRect, splitButton.BackColor);
                }
                
                state = GetSplitButtonToolBarState(splitButton, true);
                
                if ((state == ToolBarState.Pressed) || (state == ToolBarState.Hot)) {
                    RenderSmall3DBorderInternal(g, dropDownRect, state, rightToLeft);
                }
                
                DrawArrow(new ToolStripArrowRenderEventArgs(g, splitButton, dropDownRect, arrowColor, ArrowDirection.Down));
            

            }
          
        }
   
        /// <devdoc>
        ///    This exists mainly so that buttons, labels and items, etc can share the same implementation.
        ///    If OnRenderButton called OnRenderItem we would never be able to change the implementation
        ///    as it would be a breaking change. If in v1, the user overrode OnRenderItem to draw green triangles
        ///    and in v2 we decided to add a feature to button that would require us to no longer call OnRenderItem - 
        ///    the user's version of OnRenderItem would not get called when he upgraded his framework.  Hence
        ///    everyone should just call this private shared method.  Users need to override each item they want
        ///    to change the look and feel of.
        ///  </devdoc>
        private void RenderItemInternal(ToolStripItemRenderEventArgs e) {
            ToolStripItem item = e.Item;
            Graphics g = e.Graphics;
           
            ToolBarState state = GetToolBarState(item);
            VisualStyleElement toolBarElement = VisualStyleElement.ToolBar.Button.Normal;
                          

            if (ToolStripManager.VisualStylesEnabled 
                && (VisualStyleRenderer.IsElementDefined(toolBarElement))) {
                
                VisualStyleRenderer vsRenderer = VisualStyleRenderer;
                vsRenderer.SetParameters(toolBarElement.ClassName, toolBarElement.Part, (int)state);
                vsRenderer.DrawBackground(g, new Rectangle(Point.Empty, item.Size));
            }
            else {
                RenderSmall3DBorderInternal(g, new Rectangle(Point.Empty, item.Size), state, (item.RightToLeft == RightToLeft.Yes));
            } 

            Rectangle fillRect = item.ContentRectangle;

            if (item.BackgroundImage != null) {
                ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, fillRect, fillRect);
            }
            else {
                ToolStrip parent = item.GetCurrentParent();
                if ((parent != null) && (state != ToolBarState.Checked) && (item.BackColor != parent.BackColor)) {
                    FillBackground(g, fillRect, item.BackColor);
                }
            }

        }

        
        /// <devdoc>
        ///  </devdoc>
        private void RenderSeparatorInternal(Graphics g, ToolStripItem item, Rectangle bounds, bool vertical) {
            VisualStyleElement separator = (vertical) ? VisualStyleElement.ToolBar.SeparatorHorizontal.Normal : VisualStyleElement.ToolBar.SeparatorVertical.Normal;
               
            if (ToolStripManager.VisualStylesEnabled
                && (VisualStyleRenderer.IsElementDefined(separator))){
                VisualStyleRenderer vsRenderer = VisualStyleRenderer;
     
                vsRenderer.SetParameters(separator.ClassName, separator.Part, GetItemState(item));
                vsRenderer.DrawBackground(g, bounds);
            }
            else {

               Color foreColor =  item.ForeColor;
               Color backColor =  item.BackColor;
         
               Pen foreColorPen = SystemPens.ControlDark;
               bool disposeForeColorPen = GetPen(foreColor, ref foreColorPen);
               
               try {
                    if (vertical) {
                       if (bounds.Height >= 4) {
                           bounds.Inflate(0,-2);     // scoot down 2PX and start drawing
                       }
                       
                       bool rightToLeft =  (item.RightToLeft == RightToLeft.Yes);
                       Pen leftPen  = (rightToLeft) ?  SystemPens.ButtonHighlight : foreColorPen;
                       Pen rightPen = (rightToLeft) ?  foreColorPen : SystemPens.ButtonHighlight;
                                          
                       // Draw dark line
                       int startX = bounds.Width / 2;
                       g.DrawLine(leftPen, startX, bounds.Top, startX, bounds.Bottom);

                       // Draw highlight one pixel to the right
                       startX++;
                       g.DrawLine(rightPen, startX, bounds.Top, startX, bounds.Bottom);
                       
                   }
                    else { 
                       //
                       // horizontal separator
                       if (bounds.Width >= 4) {
                           bounds.Inflate(-2,0);        // scoot over 2PX and start drawing
                       }

                        // Draw dark line
                        int startY = bounds.Height / 2;
                        g.DrawLine(foreColorPen, bounds.Left, startY, bounds.Right, startY);

                        // Draw highlight one pixel to the right
                        startY++;
                        g.DrawLine(SystemPens.ButtonHighlight, bounds.Left, startY, bounds.Right, startY);
                    }
                   
               }
               finally {
                    if (disposeForeColorPen && foreColorPen != null) {
                       foreColorPen.Dispose(); 
                    }
               }
            }
        }

        private void RenderSmall3DBorderInternal (Graphics g, Rectangle bounds, ToolBarState state, bool rightToLeft) {
            if ((state == ToolBarState.Hot) ||(state == ToolBarState.Pressed) || (state == ToolBarState.Checked)) {
                    Pen leftPen, topPen, rightPen,bottomPen;
        			topPen = (state == ToolBarState.Hot) ?   SystemPens.ButtonHighlight : SystemPens.ButtonShadow;
        			bottomPen = (state == ToolBarState.Hot)?  SystemPens.ButtonShadow : SystemPens.ButtonHighlight;

                    leftPen =  (rightToLeft) ? bottomPen : topPen;
                    rightPen = (rightToLeft) ? topPen : bottomPen;

                    g.DrawLine(topPen,  bounds.Left, bounds.Top, bounds.Right -1, bounds.Top);
                    g.DrawLine(leftPen,  bounds.Left, bounds.Top, bounds.Left, bounds.Bottom-1);
                    g.DrawLine(rightPen, bounds.Right-1, bounds.Top, bounds.Right -1, bounds.Bottom-1);
                    g.DrawLine(bottomPen, bounds.Left, bounds.Bottom-1, bounds.Right -1, bounds.Bottom -1);
                    
            }
        }

        private void RenderStatusStripBorder(ToolStripRenderEventArgs e) {
            if (!Application.RenderWithVisualStyles) {
                e.Graphics.DrawLine(SystemPens.ButtonHighlight, 0,0,e.ToolStrip.Width, 0);
            }
        }

        private static void RenderStatusStripBackground(ToolStripRenderEventArgs e) {
            if (Application.RenderWithVisualStyles) {
                VisualStyleRenderer vsRenderer = VisualStyleRenderer;
                vsRenderer.SetParameters(VisualStyleElement.Status.Bar.Normal);
                vsRenderer.DrawBackground(e.Graphics,new Rectangle(0,0,e.ToolStrip.Width-1, e.ToolStrip.Height-1));
            }
            else {
                if (!SystemInformation.InLockedTerminalSession()) {
                    e.Graphics.Clear(e.BackColor);
                }
            }
        }
        
        private static void RenderLabelInternal(ToolStripItemRenderEventArgs e) {
             // dont call RenderItemInternal, as we NEVER want to paint hot.
             ToolStripItem item = e.Item;
             Graphics g = e.Graphics;

             Rectangle fillRect = item.ContentRectangle;
   
             if (item.BackgroundImage != null) {
                ControlPaint.DrawBackgroundImage(g, item.BackgroundImage, item.BackColor, item.BackgroundImageLayout, fillRect, fillRect);
             }
             else {
                 VisualStyleRenderer vsRenderer = VisualStyleRenderer;
 
                 if (vsRenderer == null || (item.BackColor != SystemColors.Control)) {
                     FillBackground(g, fillRect, item.BackColor);
                 }
             }
        }
    }
}
