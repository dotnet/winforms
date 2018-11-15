// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms.Design;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Versioning;

    /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator"]/*' />
    /// <devdoc>
    ///    <para>
    ///      Called when the background of the winbar is being rendered
    ///    </para>
    /// </devdoc>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.ToolStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    public class ToolStripSeparator : ToolStripItem {
            private const int WINBAR_SEPARATORTHICKNESS = 6;
            private const int WINBAR_SEPARATORHEIGHT = 23;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            public ToolStripSeparator() {
                this.ForeColor = SystemColors.ControlDark;
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.AutoToolTip"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new bool AutoToolTip {
               get { 
                    return base.AutoToolTip;
               }
               set {
                    base.AutoToolTip = value;
               }
            }


            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.Image"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public override Image BackgroundImage {
                get {
                    return base.BackgroundImage;
                     
                }
                set {
                    base.BackgroundImage = value;
                }
            }

            /// <include file='doc\ToolStripItem.uex' path='docs/doc[@for="ToolStripItem.BackgroundImageLayout"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public override ImageLayout BackgroundImageLayout {
              get {
                  return base.BackgroundImageLayout;
              }
              set {
                  base.BackgroundImageLayout = value;
              }
            }
            
            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.CanSelect"]/*' />
            public override bool CanSelect { 
                get  { 
                    return DesignMode; 
                } 
            }


            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.DefaultSize"]/*' />
            /// <devdoc>
            ///     Deriving classes can override this to configure a default size for their control.
            ///     This is more efficient than setting the size in the control's constructor.
            /// </devdoc>
            protected override Size DefaultSize {
                get {
                    return new Size(WINBAR_SEPARATORTHICKNESS, WINBAR_SEPARATORTHICKNESS);
                }
            }


            /// <include file='doc\WinBarSeparator.uex' path='docs/doc[@for="ToolStripSeparator.DefaultMargin"]/*' />
            protected internal override Padding DefaultMargin {
               get {
                   return Padding.Empty;
               }
           }


           [
           Browsable(false), 
           EditorBrowsable(EditorBrowsableState.Never),
           DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
           ]
           public new bool DoubleClickEnabled {
               get {
                   return base.DoubleClickEnabled;
               }
               set {
                   base.DoubleClickEnabled = value;
               }
           }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.Enabled"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public override bool Enabled {
                get {
                  return base.Enabled;
                }
                set {
                  base.Enabled = value;
                }

            }

            /// <include file='doc\TabPage.uex' path='docs/doc[@for="ToolStripSeparator.EnabledChanged"]/*' />
            /// <internalonly/>
            [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
            new public event EventHandler EnabledChanged {
                add {
                    base.EnabledChanged += value;
                }
                remove {
                    base.EnabledChanged -= value;
                }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.DisplayStyle"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new ToolStripItemDisplayStyle DisplayStyle {
                get { 
                    return base.DisplayStyle; 
                }
                set {
                   base.DisplayStyle = value;
                }
            }

            /// <include file='doc\TabPage.uex' path='docs/doc[@for="ToolStripSeparator.DisplayStyleChanged"]/*' />
            /// <internalonly/>
            [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
            new public event EventHandler DisplayStyleChanged {
                add {
                    base.DisplayStyleChanged += value;
                }
                remove {
                    base.DisplayStyleChanged -= value;
                }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.Font"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public override Font Font {
                get { 
                    return base.Font; 
                }
                set {
                   base.Font = value;
                }
            }

            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new ContentAlignment ImageAlign {
                get {
                    return base.ImageAlign;
                }
                set {
                    base.ImageAlign = value;
                }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.Image"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public override Image Image {
                [ResourceExposure(ResourceScope.Machine)]
                [ResourceConsumption(ResourceScope.Machine)]
                get {
                    return base.Image;
                }
                set {
                    base.Image = value;
                }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.ImageIndex"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            RefreshProperties(RefreshProperties.Repaint),            
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new int ImageIndex {
                get {
                    return base.ImageIndex;
                }
                set {
                    base.ImageIndex = value;
                }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.ImageKey"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new string ImageKey {
               get {
                   return base.ImageKey;
               }
               set {
                   base.ImageKey = value;
               }
            }

            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new Color ImageTransparentColor {
                get {
                    return base.ImageTransparentColor;
                }
                set {
                   base.ImageTransparentColor = value;                    
                }
            }

            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new ToolStripItemImageScaling ImageScaling {
                get {
                    return base.ImageScaling;
                }
                set {
                    base.ImageScaling = value;
                }
            }

            
            private bool IsVertical {
                get {
                    ToolStrip parent = this.ParentInternal;
    
                    if (parent == null) {
                        parent = Owner;
                    }
                    ToolStripDropDownMenu dropDownMenu = parent as ToolStripDropDownMenu;
                    if (dropDownMenu != null) {
                        return false;
                    }
                    switch (parent.LayoutStyle) {
                        case ToolStripLayoutStyle.VerticalStackWithOverflow:
                            return false;
                        case ToolStripLayoutStyle.HorizontalStackWithOverflow:  
                        case ToolStripLayoutStyle.Flow:
                        case ToolStripLayoutStyle.Table:
                        default:                            
                            return true;
                    }
                }
            }
            

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.Text"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public override string Text {
              get {
                  return base.Text;
              }
              set {
                  base.Text = value;
              }
            }

             /// <include file='doc\TabPage.uex' path='docs/doc[@for="ToolStripSeparator.TextChanged"]/*' />
            /// <internalonly/>
            [Browsable (false), EditorBrowsable (EditorBrowsableState.Never)]
            new public event EventHandler TextChanged {
                add {
                    base.TextChanged += value;
                }
                remove {
                    base.TextChanged -= value;
                }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.TextAlign"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new ContentAlignment TextAlign {
                 get {
                     return base.TextAlign;
                 }
                 set {
                     base.TextAlign = value;
                 }
            }

            
            [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(ToolStripTextDirection.Horizontal)]
            public override ToolStripTextDirection TextDirection {
                 get {
                     return base.TextDirection;
                 }
                 set {
                     base.TextDirection = value;
                 }
            }

            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.TextImageRelation"]/*' />
            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new TextImageRelation TextImageRelation {
               get {
                   return base.TextImageRelation;
               }
               set {
                   base.TextImageRelation = value;      
               }
            }

            [
            Browsable(false), 
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new string ToolTipText {
                get {
                    return base.ToolTipText;
                }
                set {
                    base.ToolTipText = value;
                }
            }
            
            [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Never),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
            ]
            public new bool RightToLeftAutoMirrorImage {
                get {
                    return base.RightToLeftAutoMirrorImage;
                }
                set {
                    base.RightToLeftAutoMirrorImage = value;
                }
            }

            [EditorBrowsable(EditorBrowsableState.Advanced)]
            protected override AccessibleObject CreateAccessibilityInstance() {
               return new ToolStripSeparatorAccessibleObject(this);
            }


            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.GetPreferredSize"]/*' />
            public override Size GetPreferredSize(Size constrainingSize) {
                ToolStrip parent = this.ParentInternal;

                if (parent == null) {
                    parent = Owner;
                }
                if (parent == null) {
                    return new Size(WINBAR_SEPARATORTHICKNESS, WINBAR_SEPARATORTHICKNESS);
                }
                
                ToolStripDropDownMenu dropDownMenu = parent as ToolStripDropDownMenu;
                if (dropDownMenu != null) {
                    return new Size(parent.Width - (parent.Padding.Horizontal - dropDownMenu.ImageMargin.Width), WINBAR_SEPARATORTHICKNESS);
                }
                else {        
                    if (parent.LayoutStyle != ToolStripLayoutStyle.HorizontalStackWithOverflow || parent.LayoutStyle != ToolStripLayoutStyle.VerticalStackWithOverflow) {
                        // we dont actually know what size to make it, so just keep it a stock size.
                        constrainingSize.Width = WINBAR_SEPARATORHEIGHT;
                        constrainingSize.Height = WINBAR_SEPARATORHEIGHT;
                    }
                    if (IsVertical) {
                        return new Size(WINBAR_SEPARATORTHICKNESS, constrainingSize.Height);                        
                    }
                    else {
                        return new Size(constrainingSize.Width, WINBAR_SEPARATORTHICKNESS);   
                    }
                }
                
            }

            
            /// <include file='doc\ToolStripSeparator.uex' path='docs/doc[@for="ToolStripSeparator.OnPaint"]/*' />
            protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {
                if (this.Owner != null && this.ParentInternal != null) {
                    this.Renderer.DrawSeparator(new ToolStripSeparatorRenderEventArgs(e.Graphics, this, IsVertical));
                }
            }

            [EditorBrowsable(EditorBrowsableState.Never)]
            protected override void OnFontChanged(EventArgs e) {
                // PERF: dont call base, we dont care if the font changes             
                RaiseEvent(EventFontChanged, e);
            }

            [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
            [EditorBrowsable(EditorBrowsableState.Never)]
            internal override bool ShouldSerializeForeColor() {
                 return (ForeColor != SystemColors.ControlDark);
            }

            internal protected override void SetBounds(Rectangle rect) {

                ToolStripDropDownMenu dropDownMenu = this.Owner as ToolStripDropDownMenu;
                if (dropDownMenu != null) {
             
                   // Scooch over by the padding amount.  The padding is added to 
                   // the ToolStripDropDownMenu to keep the non-menu item riffraff
                   // aligned to the text rectangle. When flow layout comes through to set our position
                   // via IArrangedElement DEFY IT!
                   if (dropDownMenu != null) {
                        rect.X = 2;
                        rect.Width = dropDownMenu.Width -4;                          
                   }
                }
                base.SetBounds(rect); 
            }

            /// <devdoc>
            /// An implementation of AccessibleChild for use with ToolStripItems        
            /// </devdoc>
            [System.Runtime.InteropServices.ComVisible(true)]        
            internal class ToolStripSeparatorAccessibleObject : ToolStripItemAccessibleObject {
                private ToolStripSeparator ownerItem = null;
            
                public ToolStripSeparatorAccessibleObject(ToolStripSeparator ownerItem): base(ownerItem) {
                    this.ownerItem = ownerItem;
                }

                public override AccessibleRole Role {
                    get {
                        AccessibleRole role = ownerItem.AccessibleRole;
                        if (role != AccessibleRole.Default) {
                            return role;
                        }
                        else {
                            return AccessibleRole.Separator;
                        }

                    }
                }    
            }
        }
 }
