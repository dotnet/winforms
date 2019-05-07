﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    [Docking(DockingBehavior.Never)]
    [Designer("System.Windows.Forms.Design.SplitterPanelDesigner, " + AssemblyRef.SystemDesign)]
    [ToolboxItem(false)]
    public sealed class SplitterPanel : Panel
    {
        SplitContainer owner  = null;
        private bool collapsed = false;
        
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

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
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

        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Padding DefaultMargin {
            get {
                return new Padding(0, 0, 0, 0);
            }
        }


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
        
        /// <devdoc>
        ///     The parent of this control.
        /// </devdoc>
        internal SplitContainer Owner {
            get {
                return owner;
            }
        }

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

        /// <devdoc>
        ///     Override VisibleChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler VisibleChanged {
            add => base.VisibleChanged += value;
            remove => base.VisibleChanged -= value;
        }

        /// <devdoc>
        ///     Override DockChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler DockChanged {
            add => base.DockChanged += value;
            remove => base.DockChanged -= value;
        }

        /// <devdoc>
        ///     Override LocationChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler LocationChanged {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }

        /// <devdoc>
        ///     Override TabIndexChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler TabIndexChanged {
            add => base.TabIndexChanged += value;
            remove => base.TabIndexChanged -= value;
        }        

        /// <devdoc>
        ///     Override TabStopChanged to make it hidden from the user in the designer
        /// </devdoc>
        [
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        Browsable(false)    
        ]
        public new event EventHandler TabStopChanged {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

    }
}

