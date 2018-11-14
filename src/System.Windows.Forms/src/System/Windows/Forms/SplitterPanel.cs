// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Imaging;
    using System.Runtime.InteropServices;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;

    /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel"]/*' />
    /// <devdoc>
    ///     TBD.
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Docking(DockingBehavior.Never),
    Designer("System.Windows.Forms.Design.SplitterPanelDesigner, " + AssemblyRef.SystemDesign),
    ToolboxItem(false)
    ]
    public sealed class SplitterPanel : Panel {

        SplitContainer owner  = null;
        private bool collapsed = false;
        
        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.SplitterPanel"]/*' />
        public SplitterPanel(SplitContainer owner)
        : base() {
              this.owner =   owner;
              SetStyle(ControlStyles.ResizeRedraw, true);
          
        }

        internal bool Collapsed {
            get {
                return collapsed;
            }
            set {
                collapsed = value;
            }
        }

        
        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.AutoSize"]/*' />
        /// <devdoc>
        ///    Override AutoSize to make it hidden from the user in the designer 
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new bool AutoSize {
            get {
                return base.AutoSize;
            }
            set {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.AutoSizeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged {
            add {
                base.AutoSizeChanged += value;
            }
            remove {
                base.AutoSizeChanged -= value;
            }
        }

        /// <devdoc>
        ///     Allows the control to optionally shrink when AutoSize is true.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false),
        Localizable(false)
        ]
        public override AutoSizeMode AutoSizeMode {
            get {
                return AutoSizeMode.GrowOnly;
            }
            set {
            }
        }
        
        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Anchor"]/*' />
        /// <devdoc>
        ///    Override Anchor to make it hidden from the user in the designer 
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new AnchorStyles Anchor {
            get {
                return base.Anchor;
            }
            set {
                base.Anchor = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.BorderStyle"]/*' />
        /// <devdoc>
        ///     Indicates what type of border the Splitter control has.  This value
        ///     comes from the System.Windows.Forms.BorderStyle enumeration.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new BorderStyle BorderStyle {
            get {
                return base.BorderStyle;
            }
            set {
                base.BorderStyle = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Dock"]/*' />
        /// <devdoc>
        ///     The dock property. The dock property controls to which edge
        ///     of the container this control is docked to. For example, when docked to
        ///     the top of the container, the control will be displayed flush at the
        ///     top of the container, extending the length of the container.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new DockStyle Dock {
            get {
                return base.Dock;
            }
            set {
                base.Dock = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.DockPadding"]/*' />
        /// <devdoc>
        ///    Override DockPadding to make it hidden from the user in the designer 
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        new public DockPaddingEdges DockPadding {
            get {
                return base.DockPadding;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Height"]/*' />
        /// <devdoc>
        ///     The height of this SplitterPanel
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlHeightDescr))
        ]
        public new int Height {
            get {
                if (Collapsed) {
                    return 0;
                }
                return base.Height;
            }
            set {
                throw new NotSupportedException(SR.SplitContainerPanelHeight); 
            }
        }

        internal int HeightInternal {
            get {
                return ((Panel)this).Height;
            }
            set {
                ((Panel)this).Height = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Location"]/*' />
        /// <devdoc>
        ///     Override Location to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new Point Location {
            get {
                return base.Location;
            }
            set {
                base.Location = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.DefaultMargin"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Padding DefaultMargin {
            get {
                return new Padding(0, 0, 0, 0);
            }
        }


        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.MinimumSize"]/*' />
        /// <devdoc>
        ///    Override AutoSize to make it hidden from the user in the designer 
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new Size MinimumSize {
            get {
                return base.MinimumSize;
            }
            set {
                base.MinimumSize = value;
            }
        }


        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.MaximumSize"]/*' />
        /// <devdoc>
        ///    Override AutoSize to make it hidden from the user in the designer 
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new Size MaximumSize {
            get {
                return base.MaximumSize;
            }
            set {
                base.MaximumSize = value;
            }
        }
        
        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Name"]/*' />
        /// <devdoc>
        ///     Name of this control. The designer will set this to the same
        ///     as the programatic Id "(name)" of the control.  The name can be
        ///     used as a key into the ControlCollection.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new string Name {
            get {
                return base.Name;
            }
            set {
                base.Name = value;
            }
        }
        
        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Parent"]/*' />
        /// <devdoc>
        ///     The parent of this control.
        /// </devdoc>
        internal SplitContainer Owner {
            get {
                return owner;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Parent"]/*' />
        /// <devdoc>
        ///     The parent of this control.
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new Control Parent {
            get {
                return base.Parent;
            }
            set {
                base.Parent = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Size"]/*' />
        /// <devdoc>
        ///     Override Size to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new Size Size {
            get {
                if (Collapsed) {
                    return Size.Empty;
                }
                return base.Size;
            }
            set {
                base.Size = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.TabIndex"]/*' />
        /// <devdoc>
        ///     Override TabIndex to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new int TabIndex {
            get {
                return base.TabIndex;
            }
            set {
                base.TabIndex = value;
            }
        }


        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.TabStop"]/*' />
        /// <devdoc>
        ///     Override TabStop to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Visible"]/*' />
        /// <devdoc>
        ///     Override Visible to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new bool Visible {
            get {
                return base.Visible;
            }
            set {
                base.Visible = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.Width"]/*' />
        /// <devdoc>
        ///     The width of this control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Always),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ControlWidthDescr))
        ]
        public new int Width {
            get {
                if (Collapsed) {
                    return 0;
                }
                return base.Width;
            }
            set {
                throw new NotSupportedException(SR.SplitContainerPanelWidth);
            }
        }

        internal int WidthInternal {
            get {
                return ((Panel)this).Width;
            }
            set {
                ((Panel)this).Width = value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.VisibleChanged"]/*' />
        /// <devdoc>
        ///     Override VisibleChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler VisibleChanged {
            add {
                base.VisibleChanged += value;
            }
            remove {
                base.VisibleChanged -= value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.DockChanged"]/*' />
        /// <devdoc>
        ///     Override DockChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler DockChanged {
            add {
                base.DockChanged += value;
            }
            remove {
                base.DockChanged -= value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.LocationChanged"]/*' />
        /// <devdoc>
        ///     Override LocationChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler LocationChanged {
            add {
                base.LocationChanged += value;
            }
            remove {
                base.LocationChanged -= value;
            }
        }

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.TabIndexChanged"]/*' />
        /// <devdoc>
        ///     Override TabIndexChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler TabIndexChanged {
            add {
                base.TabIndexChanged += value;
            }
            remove {
                base.TabIndexChanged -= value;
            }
        }        

        /// <include file='doc\SplitterPanel.uex' path='docs/doc[@for="SplitterPanel.TabStopChanged"]/*' />
        /// <devdoc>
        ///     Override TabStopChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

    }
}

