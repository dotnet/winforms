// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms.ButtonInternal;
    using System.Security.Permissions;
    using System.Security;
    using System.Windows.Forms.Design;    

    /// <include file='doc\ToolStripLabel.uex' path='docs/doc[@for="ToolStripLabel"]/*' />
    /// <devdoc>
    /// A non selectable winbar item
    /// </devdoc>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip)]
    public class ToolStripLabel : ToolStripItem {

        private LinkBehavior linkBehavior = LinkBehavior.SystemDefault;
        private bool isLink = false, linkVisited = false;
   
        private Color linkColor = Color.Empty;
        private Color activeLinkColor = Color.Empty;
        private Color visitedLinkColor = Color.Empty;
        private Font hoverLinkFont, linkFont;
        private Cursor lastCursor;
        
                     
              

        /// <include file='doc\ToolStripLabel.uex' path='docs/doc[@for="ToolStripLabel.ToolStripLabel"]/*' />
        /// <devdoc>
        /// A non selectable winbar item
        /// </devdoc>
        public ToolStripLabel() {
        }
        public ToolStripLabel(string text):base(text,null,null) {
        }
        public ToolStripLabel(Image image):base(null,image,null) {
        }
        public ToolStripLabel(string text, Image image):base(text,image,null) {
        }
        public ToolStripLabel(string text, Image image, bool isLink):this(text,image,isLink, null) {
        }
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ToolStripLabel(string text, Image image, bool isLink, EventHandler onClick):this(text,image,isLink,onClick,null) {         
        }
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ToolStripLabel(string text, Image image, bool isLink, EventHandler onClick, string name) : base(text,image,onClick,name) {
            IsLink = isLink;
        }

        /// <include file='doc\ToolStripLabel.uex' path='docs/doc[@for="ToolStripLabel.CanSelect"]/*' />
        public override bool CanSelect {
            get { return (IsLink || DesignMode); }
        }

        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ToolStripLabelIsLinkDescr))
        ]
        public bool IsLink {
          get {
              return isLink;
          }
          set {
              if (isLink != value) {
                  isLink = value;
                  Invalidate();
              }
          }
        }
        
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripLabelActiveLinkColorDescr))
        ]
        public Color ActiveLinkColor {
            get {
                if (activeLinkColor.IsEmpty) {
                    return IEActiveLinkColor;
                }
                else {
                    return activeLinkColor;
                }
            }
            set {
                if (activeLinkColor != value) {
                    activeLinkColor = value;
                    Invalidate();
                }
            }
        }
        private Color IELinkColor {
          get {
              return LinkUtilities.IELinkColor;
          }
        }

        private Color IEActiveLinkColor {
          get {
              return LinkUtilities.IEActiveLinkColor;
          }
        }
        private Color IEVisitedLinkColor {
          get {
              return LinkUtilities.IEVisitedLinkColor;
          }
        }

        [
        DefaultValue(LinkBehavior.SystemDefault),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ToolStripLabelLinkBehaviorDescr))
        ]
        public LinkBehavior LinkBehavior {
            get {
                return linkBehavior;
            }
            set {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)LinkBehavior.SystemDefault, (int)LinkBehavior.NeverUnderline))
                {
                    throw new InvalidEnumArgumentException(nameof(LinkBehavior), (int)value, typeof(LinkBehavior));
                }
                if (linkBehavior != value) {
                    linkBehavior = value;
                    InvalidateLinkFonts();
                    Invalidate();
                }
            }
        }

        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripLabelLinkColorDescr))
        ]
        public Color LinkColor {
            get {
                if (linkColor.IsEmpty) {
                    return IELinkColor;
                }
                else {
                    return linkColor;
                }
            }
            set {
                if (linkColor != value) {
                    linkColor = value;
                    Invalidate();
                }
            }
        }

        [
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripLabelLinkVisitedDescr))
        ]
        public bool LinkVisited {
            get {
                return linkVisited;
            }
            set {
                if (linkVisited != value) {
                    linkVisited = value;
                    Invalidate();
                }
            }
        }

        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.ToolStripLabelVisitedLinkColorDescr))
        ]
        public Color VisitedLinkColor {
            get {
                if (visitedLinkColor.IsEmpty) {
                    return IEVisitedLinkColor;
                }
                else {
                    return visitedLinkColor;
                }
            }
            set {
                if (visitedLinkColor != value) {
                    visitedLinkColor = value;
                    Invalidate();
                }
            }
        }
        
        /// <include file='doc\LinkLabel.uex' path='docs/doc[@for="LinkLabel.InvalidateLinkFonts"]/*' />
        /// <devdoc>
        ///     Invalidates the current set of fonts we use when painting
        ///     links.  The fonts will be recreated when needed.
        /// </devdoc>
        private void InvalidateLinkFonts() {
  
            if (linkFont != null) {
                linkFont.Dispose();
            }
  
            if (hoverLinkFont != null && hoverLinkFont != linkFont) {
                hoverLinkFont.Dispose();
            }
  
            linkFont = null;
            hoverLinkFont = null;
        }
  
        protected override void OnFontChanged(EventArgs e) {
           InvalidateLinkFonts();
           base.OnFontChanged(e);
        }

    
        protected override void OnMouseEnter(EventArgs e) {
            if (IsLink) {
                ToolStrip parent = this.Parent;
                if (parent != null) {
                    lastCursor = parent.Cursor;
                    parent.Cursor = Cursors.Hand;
                }
            }
            base.OnMouseEnter(e);
  
        }
  
        protected override void OnMouseLeave(EventArgs e) {
            if (IsLink) {
                ToolStrip parent = this.Parent;
                if (parent != null) {
                    parent.Cursor = lastCursor;
                }
            }
            base.OnMouseLeave(e);
  
        }

       private void ResetActiveLinkColor()
       {
            ActiveLinkColor = IEActiveLinkColor;
       }

       private void ResetLinkColor()
       {
            LinkColor = IELinkColor;
       }

       private void ResetVisitedLinkColor()
       {
            VisitedLinkColor = IEVisitedLinkColor;
       }
  
       [EditorBrowsable(EditorBrowsableState.Never)]
       private bool ShouldSerializeActiveLinkColor() {
           return !activeLinkColor.IsEmpty;
       }
       
       [EditorBrowsable(EditorBrowsableState.Never)]
       private bool ShouldSerializeLinkColor() {
           return !linkColor.IsEmpty;
       }

       [EditorBrowsable(EditorBrowsableState.Never)]
       private bool ShouldSerializeVisitedLinkColor() {
           return !visitedLinkColor.IsEmpty;
       }

       

        /// <devdoc>
        /// Creates an instance of the object that defines how image and text
        /// gets laid out in the ToolStripItem
        /// </devdoc>
        internal override ToolStripItemInternalLayout CreateInternalLayout() {
            return new ToolStripLabelLayout(this);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new ToolStripLabelAccessibleObject(this);
        }

        /// <include file='doc\ToolStripLabel.uex' path='docs/doc[@for="ToolStripLabel.OnPaint"]/*' />
        /// <devdoc>
        /// Inheriting classes should override this method to handle this event.
        /// </devdoc>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {

            if (this.Owner != null) {
                ToolStripRenderer renderer = this.Renderer;
                  
                renderer.DrawLabelBackground(new ToolStripItemRenderEventArgs(e.Graphics, this));

                if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image) { 
                    renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(e.Graphics, this, InternalLayout.ImageRectangle));
                }
                PaintText(e.Graphics);
            }
        }

        internal void PaintText(Graphics g) {
            ToolStripRenderer renderer = this.Renderer;
              
            if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) { 
                 Font font = this.Font;
                 Color textColor = this.ForeColor;
                 if (IsLink) {
                      LinkUtilities.EnsureLinkFonts(font, this.LinkBehavior, ref this.linkFont, ref this.hoverLinkFont);
        
                      if (this.Pressed) {
                          font = hoverLinkFont;
                          textColor = this.ActiveLinkColor;
                      }
                      else if (this.Selected) {
                          font = hoverLinkFont;
                          textColor = (this.LinkVisited) ? this.VisitedLinkColor : this.LinkColor;
                      }
                      else {
                          font = linkFont;
                          textColor = (this.LinkVisited) ? this.VisitedLinkColor : this.LinkColor;
                      }
                  }
                  Rectangle textRect = InternalLayout.TextRectangle;
                  renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, this.Text, textRect, textColor, font, InternalLayout.TextFormat));
             }
        }
        
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'charCode' matches control.cs
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected internal override bool ProcessMnemonic(char charCode) {
            // checking IsMnemonic is not necessary - toolstrip does this for us.
            if (ParentInternal != null) {
                if (!CanSelect) {
                    ParentInternal.SetFocusUnsafe();
                    ParentInternal.SelectNextToolStripItem(this, /*forward=*/true);
                }
                else {
                    FireEvent(ToolStripItemEventType.Click);
                }
                return true;
           
            }
            return false;
        }

        
        [System.Runtime.InteropServices.ComVisible(true)]        
        [SuppressMessage("Microsoft.Performance", "CA1812:AvoidUninstantiatedInternalClasses")]
        internal class ToolStripLabelAccessibleObject : ToolStripItemAccessibleObject {
            private ToolStripLabel ownerItem = null;

            public ToolStripLabelAccessibleObject(ToolStripLabel ownerItem) : base(ownerItem) {
              this.ownerItem = ownerItem;
            }

            public override string DefaultAction {
                get {
                    if (ownerItem.IsLink) {
                        return SR.AccessibleActionClick;
                    }
                    else {
                        return string.Empty;
                    }
                }
            }

            public override void DoDefaultAction() {
                if (ownerItem.IsLink) {
                    base.DoDefaultAction();
                }
            }
         
            public override AccessibleRole Role {
               get {
                   AccessibleRole role = Owner.AccessibleRole;
                   if (role != AccessibleRole.Default) {
                       return role;
                   }
                   return (ownerItem.IsLink) ? AccessibleRole.Link : AccessibleRole.StaticText;
               }
            }

            public override AccessibleStates State {
                get {
                    return base.State | AccessibleStates.ReadOnly;
                }
            }
        }
        /// <devdoc>
        ///  This class performs internal layout for the "split button button" portion of a split button.
        ///  Its main job is to make sure the inner button has the same parent as the split button, so
        ///  that layout can be performed using the correct graphics context.
        /// </devdoc>
        private class ToolStripLabelLayout : ToolStripItemInternalLayout {

             ToolStripLabel owner;

            public ToolStripLabelLayout(ToolStripLabel owner) : base(owner) {
                this.owner = owner;
            }

            protected override ToolStripItemLayoutOptions CommonLayoutOptions() {
               ToolStripItemLayoutOptions layoutOptions = base.CommonLayoutOptions();
               layoutOptions.borderSize = 0;
               return layoutOptions;
            }
        }
        
    }

}
    


