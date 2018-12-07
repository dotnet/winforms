// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design.Serialization;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Drawing.Printing;
    using System.Runtime.Remoting;
    using System.Windows.Forms.Design;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.Versioning;

    /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog"]/*' />
    /// <devdoc>
    ///    <para> Represents a
    ///       dialog box form that contains a <see cref='System.Windows.Forms.PrintPreviewControl'/>.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    Designer("System.ComponentModel.Design.ComponentDesigner, " + AssemblyRef.SystemDesign),
    DesignTimeVisible(true),
    DefaultProperty(nameof(Document)),
    ToolboxItemFilter("System.Windows.Forms.Control.TopLevel"),
    ToolboxItem(true),
    SRDescription(nameof(SR.DescriptionPrintPreviewDialog))
    ]
    public class PrintPreviewDialog : Form {
        PrintPreviewControl previewControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.NumericUpDown pageCounter;
        private System.Windows.Forms.ToolStripButton printToolStripButton;
        private System.Windows.Forms.ToolStripSplitButton zoomToolStripSplitButton;
        private System.Windows.Forms.ToolStripMenuItem autoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem6;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem7;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem8;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripSeparator;
        private System.Windows.Forms.ToolStripButton onepageToolStripButton;
        private System.Windows.Forms.ToolStripButton twopagesToolStripButton;
        private System.Windows.Forms.ToolStripButton threepagesToolStripButton;
        private System.Windows.Forms.ToolStripButton fourpagesToolStripButton;
        private System.Windows.Forms.ToolStripButton sixpagesToolStripButton;
        private System.Windows.Forms.ToolStripSeparator separatorToolStripSeparator1;
        private System.Windows.Forms.ToolStripButton closeToolStripButton;
        private System.Windows.Forms.ToolStripLabel pageToolStripLabel;
        ImageList imageList;
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.PrintPreviewDialog"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.PrintPreviewDialog'/> class.</para>
        /// </devdoc>
        public PrintPreviewDialog() {

            
            #pragma warning disable 618
            base.AutoScaleBaseSize = new Size(5, 13);
            #pragma warning restore 618
            
            

            this.previewControl = new PrintPreviewControl();
            this.imageList = new ImageList();
            
            Bitmap bitmaps = new Bitmap(typeof(PrintPreviewDialog), "PrintPreviewStrip.bmp");
            bitmaps.MakeTransparent();
            imageList.Images.AddStrip(bitmaps);

            InitForm();

            
        }

        //subhag addition
        //-------------------------------------------------------------------------------------------------------------
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AcceptButton"]/*' />
        /// <devdoc>
        /// <para>Indicates the <see cref='System.Windows.Forms.Button'/> control on the form that is clicked when
        ///    the user presses the ENTER key.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public IButtonControl AcceptButton {
            get {
                return base.AcceptButton;
            }
            set {
                base.AcceptButton = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoScale"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the form will adjust its size
        ///       to fit the height of the font used on the form and scale
        ///       its controls.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool AutoScale {
            get {
                #pragma warning disable 618
                return base.AutoScale;
                #pragma warning restore 618
            }
            set {
                #pragma warning disable 618
                base.AutoScale = value;
                #pragma warning restore 618
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoScroll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the form implements
        ///       autoscrolling.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll {
            get { 
                
                return base.AutoScroll;
            }
            set {
                base.AutoScroll = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Hide the property
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize {
            get { 
                return base.AutoSize;
            }
            set {
                base.AutoSize = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoSizeChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged {
            add {
                base.AutoSizeChanged += value;
            }
            remove {
                base.AutoSizeChanged -= value;
            }
        }                

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoValidate"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Hide the property
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AutoValidate AutoValidate {
            get {
                return base.AutoValidate;
            }
            set {
                base.AutoValidate = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoValidateChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler AutoValidateChanged {
            add {
                base.AutoValidateChanged += value;
            }
            remove {
                base.AutoValidateChanged -= value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.BackColor"]/*' />
        /// <devdoc>
        ///     The background color of this control. This is an ambient property and
        ///     will always return a non-null value.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor {
            get {
                return base.BackColor;
            }
            set {
                base.BackColor = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.BackColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged {
            add {
                base.BackColorChanged += value;
            }
            remove {
                base.BackColorChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.CancelButton"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       or
        ///       sets the button control that will be clicked when the
        ///       user presses the ESC key.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public IButtonControl CancelButton {
            get {
                return base.CancelButton;
            }
            set {
                base.CancelButton = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ControlBox"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether a control box is displayed in the
        ///       caption bar of the form.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool ControlBox {
            get {
                return base.ControlBox;
            }
            set {
                base.ControlBox = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ContextMenuStrip"]/*' />
        /// <devdoc>
        ///    Hide the property
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ContextMenuStrip ContextMenuStrip {
            get {
                 return base.ContextMenuStrip;
             }
             set {
                 base.ContextMenuStrip = value;
             }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ContextMenuStripChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ContextMenuStripChanged {
            add {
                base.ContextMenuStripChanged += value;
            }
            remove {
                base.ContextMenuStripChanged -= value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.FormBorderStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the border style of the form.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public FormBorderStyle FormBorderStyle {
            get {
                return base.FormBorderStyle;
            }
            set {
                base.FormBorderStyle = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.HelpButton"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether a
        ///       help button should be displayed in the caption box of the form.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool HelpButton {
            get {
                return base.HelpButton;
            }
            set {
                base.HelpButton = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Icon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the icon for the form.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Icon Icon {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                return base.Icon;
            }
            set {
                base.Icon = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.IsMdiContainer"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the form is a container for multiple document interface
        ///       (MDI) child forms.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool IsMdiContainer {
            get {
                return base.IsMdiContainer;
            }
            set {
                base.IsMdiContainer = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.KeyPreview"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value
        ///       indicating whether the form will receive key events
        ///       before the event is passed to the control that has focus.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool KeyPreview {
            get {
                return base.KeyPreview;
            }
            set {
                base.KeyPreview = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MaximumSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or Sets the maximum size the dialog can be resized to.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size MaximumSize {
            get {
                return base.MaximumSize;
            }
            set {
                base.MaximumSize = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MaximumSizeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MaximumSizeChanged {
            add {
                base.MaximumSizeChanged += value;
            }
            remove {
                base.MaximumSizeChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MaximizeBox"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the maximize button is
        ///       displayed in the caption bar of the form.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool MaximizeBox {
            get {
                return base.MaximizeBox;
            }
            set {
                base.MaximizeBox = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Margin"]/*' />
        /// <devdoc>
        ///    Hide the value
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Padding Margin {
            get {
                return base.Margin;
            }
            set {
                base.Margin = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MarginChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MarginChanged {
            add {
                base.MarginChanged += value;
            }
            remove {
                base.MarginChanged -= value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Menu"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the <see cref='System.Windows.Forms.MainMenu'/>
        ///       that is displayed in the form.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public MainMenu Menu {
            get {
                return base.Menu;
            }
            set {
                base.Menu = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MinimumSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the minimum size the form can be resized to.
        ///    </para>
        /// </devdoc>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size MinimumSize {
            get {
                return base.MinimumSize;
            }
            set {
                base.MinimumSize = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MinimumSizeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MinimumSizeChanged {
            add {
                base.MinimumSizeChanged += value;
            }
            remove {
                base.MinimumSizeChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Padding"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Hide the value
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Padding Padding {
            get {
                return base.Padding;
            }
            set {
                base.Padding = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.PaddingChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler PaddingChanged {
            add {
                base.PaddingChanged += value;
            }
            remove {
                base.PaddingChanged -= value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Size"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the size of the form.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size Size {
            get {
                return base.Size;
            }
            set {
                base.Size = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.SizeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler SizeChanged {
            add {
                base.SizeChanged += value;
            }
            remove {
                base.SizeChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.StartPosition"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the
        ///       starting position of the form at run time.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public FormStartPosition StartPosition {
            get {
                return base.StartPosition;
            }
            set {
                base.StartPosition = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.TopMost"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether the form should be displayed as the top-most
        ///       form of your application.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TopMost {
            get {
                return base.TopMost;
            }
            set {
                base.TopMost = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.TransparencyKey"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the color that will represent transparent areas of the form.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Color TransparencyKey {
            get {
                return base.TransparencyKey;
            }
            set {
                base.TransparencyKey = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.UseWaitCursor"]/*' />
        /// <devdoc>
        ///    <para>Hide the value</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool UseWaitCursor{
            get {
                return base.UseWaitCursor;
            }
            set {
                base.UseWaitCursor = value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.WindowState"]/*' />
        /// <devdoc>
        ///    <para> Gets or sets the form's window state.
        ///       </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public FormWindowState WindowState {
            get {
                return base.WindowState;
            }
            set {
                base.WindowState = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AccessibleRole"]/*' />
        /// <devdoc>
        ///      The accessible role of the control
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public AccessibleRole AccessibleRole {
            get {
                return base.AccessibleRole;
            }
            set {
                base.AccessibleRole = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AccessibleDescription"]/*' />
        /// <devdoc>
        ///      The accessible description of the control
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public string AccessibleDescription {
             get {
                return base.AccessibleDescription;
            }
            set {
                base.AccessibleDescription = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AccessibleName"]/*' />
        /// <devdoc>
        ///      The accessible name of the control
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public string AccessibleName {
             get {
                return base.AccessibleName;
            }
            set {
                base.AccessibleName = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.CausesValidation"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Indicates whether entering the control causes validation on the controls requiring validation.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool CausesValidation {
             get {
                return base.CausesValidation;
            }
            set {
                base.CausesValidation = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.CausesValidationChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CausesValidationChanged {
            add {
                base.CausesValidationChanged += value;
            }
            remove {
                base.CausesValidationChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.DataBindings"]/*' />
        /// <devdoc>
        ///     Retrieves the bindings for this control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ControlBindingsCollection DataBindings {
            get {
                return base.DataBindings;
            }
        }

        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.DefaultMinimumSize"]/*' />
        protected override Size DefaultMinimumSize {
            get { return new Size(375, 250); }
        }
        
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Enabled"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control is currently enabled.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool Enabled {
            get {
                return base.Enabled;
            }
            set {
                base.Enabled = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.EnabledChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged {
            add {
                base.EnabledChanged += value;
            }
            remove {
                base.EnabledChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Location"]/*' />
        /// <devdoc>
        ///     The location of this control.
        /// </devdoc>
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Point Location {
            get {
                return base.Location;
            }
            set {
                base.Location = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.LocationChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler LocationChanged {
            add {
                base.LocationChanged += value;
            }
            remove {
                base.LocationChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Tag"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public object Tag {
            get {
                return base.Tag;
            }
            set {
                base.Tag = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AllowDrop"]/*' />
        /// <devdoc>
        ///     The AllowDrop property. If AllowDrop is set to true then
        ///     this control will allow drag and drop operations and events to be used.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop {
            get {
                return base.AllowDrop;
            }
            set {
                base.AllowDrop = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Cursor"]/*' />
        /// <devdoc>
        ///     Retrieves the cursor that will be displayed when the mouse is over this
        ///     control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Cursor Cursor {
            get {
                return base.Cursor;
            }
            set {
                base.Cursor = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.CursorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CursorChanged {
            add {
                base.CursorChanged += value;
            }
            remove {
                base.CursorChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.BackgroundImage"]/*' />
        /// <devdoc>
        ///     The background image of the control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.BackgroundImageLayout"]/*' />
        /// <devdoc>
        ///     The background image layout of the control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ImeMode"]/*' />
        /// <devdoc>
        ///     Specifies a value that determines the IME (Input Method Editor) status of the 
        ///     object when that object is selected.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode {
            get {
                return base.ImeMode;
            }
            set {
                base.ImeMode = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ImeModeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged {
            add {
                base.ImeModeChanged += value;
            }
            remove {
                base.ImeModeChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoScrollMargin"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets the size of the auto-scroll
        ///       margin.
        ///    </para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size AutoScrollMargin {
            get {
                return base.AutoScrollMargin;
            }
            set {
                base.AutoScrollMargin = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoScrollMinSize"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the mimimum size of the auto-scroll.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size AutoScrollMinSize {
            get {
                return base.AutoScrollMinSize;
            }
            set {
                base.AutoScrollMinSize = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Anchor"]/*' />
        /// <devdoc>
        ///     The current value of the anchor property. The anchor property
        ///     determines which edges of the control are anchored to the container's
        ///     edges.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AnchorStyles Anchor {
            get {
                return base.Anchor;
            }
            set {
                base.Anchor = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Visible"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the control is visible.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool Visible {
            get {
                return base.Visible;
            }
            set {
                base.Visible = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.VisibleChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler VisibleChanged {
            add {
                base.VisibleChanged += value;
            }
            remove {
                base.VisibleChanged -= value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ForeColor"]/*' />
        /// <devdoc>
        ///     The foreground color of the control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ForeColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                base.ForeColorChanged += value;
            }
            remove {
                base.ForeColorChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.RightToLeft"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override RightToLeft RightToLeft {
            get {
                return base.RightToLeft;
            }
            set {
                base.RightToLeft = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.RightToLeftLayout"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool RightToLeftLayout {
            get {

                return base.RightToLeftLayout;
            }

            set {
                base.RightToLeftLayout = value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.RightToLeftChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftChanged {
            add {
                base.RightToLeftChanged += value;
            }
            remove {
                base.RightToLeftChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.RightToLeftLayoutChanged"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftLayoutChanged {
            add {
                base.RightToLeftLayoutChanged += value;
            }
            remove {
                base.RightToLeftLayoutChanged -= value;
            }
        }
        

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.TabStop"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the user can give the focus to this control using the TAB 
        ///       key. This property is read-only.</para>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.TabStopChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Text"]/*' />
        /// <devdoc>
        ///     The current text associated with this control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Dock"]/*' />
        /// <devdoc>
        ///     The dock property. The dock property controls to which edge
        ///     of the container this control is docked to. For example, when docked to
        ///     the top of the container, the control will be displayed flush at the
        ///     top of the container, extending the length of the container.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override DockStyle Dock {
            get {
                return base.Dock;
            }
            set {
                base.Dock = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.DockChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DockChanged {
            add {
                base.DockChanged += value;
            }
            remove {
                base.DockChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Font"]/*' />
        /// <devdoc>
        ///     Retrieves the current font for this control. This will be the font used
        ///     by default for painting and text in the control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.FontChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged {
            add {
                base.FontChanged += value;
            }
            remove {
                base.FontChanged -= value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ContextMenu"]/*' />
        /// <devdoc>
        ///     The contextMenu associated with this control. The contextMenu
        ///     will be shown when the user right clicks the mouse on the control.
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ContextMenu ContextMenu {
            get {
                return base.ContextMenu;
            }
            set {
                base.ContextMenu = value;
            }
        }
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ContextMenuChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ContextMenuChanged {
            add {
                base.ContextMenuChanged += value;
            }
            remove {
                base.ContextMenuChanged -= value;
            }
        }

        // DockPadding is not relevant to UpDownBase
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.DockPadding"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public DockPaddingEdges DockPadding {
            get {
                return base.DockPadding;
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        //end addition
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.UseAntiAlias"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)), 
        DefaultValue(false),
        SRDescription(nameof(SR.PrintPreviewAntiAliasDescr))
        ]
        public bool UseAntiAlias {
            get {
                return PrintPreviewControl.UseAntiAlias;
            }
            set {
                PrintPreviewControl.UseAntiAlias = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.AutoScaleBaseSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       PrintPreviewDialog does not support AutoScaleBaseSize.
        ///    </para>
        /// </devdoc>
        /// Keeping implementation of obsoleted AutoScaleBaseSize API
        #pragma warning disable 618
        // disable csharp compiler warning #0809: obsolete member overrides non-obsolete member
        #pragma warning disable 0809
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property has been deprecated. Use the AutoScaleDimensions property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public override Size AutoScaleBaseSize {
            get {
                return base.AutoScaleBaseSize;
            }

            set {
                // No-op
            }
        }
        #pragma warning restore 0809
        #pragma warning restore 618

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Document"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the document to preview.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.PrintPreviewDocumentDescr))
        ]
        public PrintDocument Document {
            get { 
                return previewControl.Document;
            }
            set {
                previewControl.Document = value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.MinimizeBox"]/*' />
        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool MinimizeBox {
            get {
                return base.MinimizeBox;
            }
            set {
                base.MinimizeBox = value;
            }
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.PrintPreviewControl"]/*' />
        /// <devdoc>
        /// <para>Gets or sets a value indicating the <see cref='System.Windows.Forms.PrintPreviewControl'/> 
        /// contained in this form.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.PrintPreviewPrintPreviewControlDescr)),
        Browsable(false)
        ]
        public PrintPreviewControl PrintPreviewControl {
            get { return previewControl;}
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.Opacity"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Opacity does not apply to PrintPreviewDialogs.
        ///    </para>
        /// </devdoc>
        [Browsable(false),EditorBrowsable(EditorBrowsableState.Advanced)]
        public new double Opacity {
            get {
                return base.Opacity;
            }
            set {
                base.Opacity = value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ShowInTaskbar"]/*' />
        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool ShowInTaskbar {
            get {
                return base.ShowInTaskbar;
            }
            set {
                base.ShowInTaskbar = value;
            }
        }
        
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.SizeGripStyle"]/*' />
        [Browsable(false), DefaultValue(SizeGripStyle.Hide), EditorBrowsable(EditorBrowsableState.Never)]
        public new SizeGripStyle SizeGripStyle {
            get {
                return base.SizeGripStyle;
            }
            set {
                base.SizeGripStyle = value;
            }
        }

        [
            SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters") // The default page count is 1.
                                                                                                        // So we don't have to localize it.
        ]
        void InitForm() {
            

            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintPreviewDialog));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.printToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomToolStripSplitButton = new System.Windows.Forms.ToolStripSplitButton();
            this.autoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.separatorToolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.onepageToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.twopagesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.threepagesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.fourpagesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.sixpagesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.separatorToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.pageCounter = new System.Windows.Forms.NumericUpDown();
            this.pageToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pageCounter)).BeginInit();
            this.SuspendLayout();

            // 
            // toolStrip1
            // 
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.printToolStripButton,
            this.zoomToolStripSplitButton,
            this.separatorToolStripSeparator,
            this.onepageToolStripButton,
            this.twopagesToolStripButton,
            this.threepagesToolStripButton,
            this.fourpagesToolStripButton,
            this.sixpagesToolStripButton,
            this.separatorToolStripSeparator1,
            this.closeToolStripButton});
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.toolStrip1.GripStyle = ToolStripGripStyle.Hidden;

            // 
            // printToolStripButton
            // 
            this.printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.printToolStripButton.Name = "printToolStripButton";
            resources.ApplyResources(this.printToolStripButton, "printToolStripButton");

            // 
            // zoomToolStripSplitButton
            // 
            this.zoomToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomToolStripSplitButton.DoubleClickEnabled = true;
            this.zoomToolStripSplitButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoToolStripMenuItem,
            this.toolStripMenuItem1,
            this.toolStripMenuItem2,
            this.toolStripMenuItem3,
            this.toolStripMenuItem4,
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem7,
            this.toolStripMenuItem8});
            this.zoomToolStripSplitButton.Name = "zoomToolStripSplitButton";
            this.zoomToolStripSplitButton.SplitterWidth = 1;
            resources.ApplyResources(this.zoomToolStripSplitButton, "zoomToolStripSplitButton");


            // 
            // autoToolStripMenuItem
            // 
            this.autoToolStripMenuItem.CheckOnClick = true;
            this.autoToolStripMenuItem.DoubleClickEnabled = true;
            this.autoToolStripMenuItem.Checked = true;
            this.autoToolStripMenuItem.Name = "autoToolStripMenuItem";
            resources.ApplyResources(this.autoToolStripMenuItem, "autoToolStripMenuItem");

            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.CheckOnClick = true;
            this.toolStripMenuItem1.DoubleClickEnabled = true;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(this.toolStripMenuItem1, "toolStripMenuItem1");

            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.CheckOnClick = true;
            this.toolStripMenuItem2.DoubleClickEnabled = true;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(this.toolStripMenuItem2, "toolStripMenuItem2");

            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.CheckOnClick = true;
            this.toolStripMenuItem3.DoubleClickEnabled = true;
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(this.toolStripMenuItem3, "toolStripMenuItem3");

            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.CheckOnClick = true;
            this.toolStripMenuItem4.DoubleClickEnabled = true;
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(this.toolStripMenuItem4, "toolStripMenuItem4");

            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.CheckOnClick = true;
            this.toolStripMenuItem5.DoubleClickEnabled = true;
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(this.toolStripMenuItem5, "toolStripMenuItem5");

            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.CheckOnClick = true;
            this.toolStripMenuItem6.DoubleClickEnabled = true;
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(this.toolStripMenuItem6, "toolStripMenuItem6");

            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.CheckOnClick = true;
            this.toolStripMenuItem7.DoubleClickEnabled = true;
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            resources.ApplyResources(this.toolStripMenuItem7, "toolStripMenuItem7");

            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.CheckOnClick = true;
            this.toolStripMenuItem8.DoubleClickEnabled = true;
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            resources.ApplyResources(this.toolStripMenuItem8, "toolStripMenuItem8");

            // 
            // separatorToolStripSeparator
            // 
            this.separatorToolStripSeparator.Name = "separatorToolStripSeparator";

            // 
            // onepageToolStripButton
            // 
            this.onepageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.onepageToolStripButton.Name = "onepageToolStripButton";
            resources.ApplyResources(this.onepageToolStripButton, "onepageToolStripButton");

            // 
            // twopagesToolStripButton
            // 
            this.twopagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.twopagesToolStripButton.Name = "twopagesToolStripButton";
            resources.ApplyResources(this.twopagesToolStripButton, "twopagesToolStripButton");

            // 
            // threepagesToolStripButton
            // 
            this.threepagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.threepagesToolStripButton.Name = "threepagesToolStripButton";
            resources.ApplyResources(this.threepagesToolStripButton, "threepagesToolStripButton");

            // 
            // fourpagesToolStripButton
            // 
            this.fourpagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.fourpagesToolStripButton.Name = "fourpagesToolStripButton";
            resources.ApplyResources(this.fourpagesToolStripButton, "fourpagesToolStripButton");

            // 
            // sixpagesToolStripButton
            // 
            this.sixpagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.sixpagesToolStripButton.Name = "sixpagesToolStripButton";
            resources.ApplyResources(this.sixpagesToolStripButton, "sixpagesToolStripButton");

            // 
            // separatorToolStripSeparator1
            // 
            this.separatorToolStripSeparator1.Name = "separatorToolStripSeparator1";

            // 
            // closeToolStripButton
            // 
            this.closeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            this.closeToolStripButton.Name = "closeToolStripButton";
            resources.ApplyResources(this.closeToolStripButton, "closeToolStripButton");

            // 
            // pageCounter
            // 
            resources.ApplyResources(this.pageCounter, "pageCounter");
            pageCounter.Text = "1";
            pageCounter.TextAlign = HorizontalAlignment.Right;
            pageCounter.DecimalPlaces = 0;
            pageCounter.Minimum = new Decimal(0d);
            pageCounter.Maximum = new Decimal(1000d);
            pageCounter.ValueChanged += new EventHandler(UpdownMove);
            this.pageCounter.Name = "pageCounter";

            // 
            // pageToolStripLabel
            // 
            this.pageToolStripLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.pageToolStripLabel.Name = "pageToolStripLabel";
            resources.ApplyResources(this.pageToolStripLabel, "pageToolStripLabel");

            previewControl.Size = new Size(792, 610);
            previewControl.Location = new Point(0, 43);
            previewControl.Dock = DockStyle.Fill;
            previewControl.StartPageChanged += new EventHandler(previewControl_StartPageChanged);

            //EVENTS and Images ...
            this.printToolStripButton.Click += new System.EventHandler(this.OnprintToolStripButtonClick);
            this.autoToolStripMenuItem.Click += new System.EventHandler(ZoomAuto);
            this.toolStripMenuItem1.Click += new System.EventHandler(Zoom500);
            this.toolStripMenuItem2.Click += new System.EventHandler(Zoom250);
            this.toolStripMenuItem3.Click += new System.EventHandler(Zoom150);
            this.toolStripMenuItem4.Click += new System.EventHandler(Zoom100);
            this.toolStripMenuItem5.Click += new System.EventHandler(Zoom75);
            this.toolStripMenuItem6.Click += new System.EventHandler(Zoom50);
            this.toolStripMenuItem7.Click += new System.EventHandler(Zoom25);
            this.toolStripMenuItem8.Click += new System.EventHandler(Zoom10);
            this.onepageToolStripButton.Click += new System.EventHandler(this.OnonepageToolStripButtonClick);
            this.twopagesToolStripButton.Click += new System.EventHandler(this.OntwopagesToolStripButtonClick);
            this.threepagesToolStripButton.Click += new System.EventHandler(this.OnthreepagesToolStripButtonClick);
            this.fourpagesToolStripButton.Click += new System.EventHandler(this.OnfourpagesToolStripButtonClick);
            this.sixpagesToolStripButton.Click += new System.EventHandler(this.OnsixpagesToolStripButtonClick);
            this.closeToolStripButton.Click += new System.EventHandler(this.OncloseToolStripButtonClick);
            this.closeToolStripButton.Paint += new PaintEventHandler(this.OncloseToolStripButtonPaint);
            //Images
            this.toolStrip1.ImageList = imageList;
            this.printToolStripButton.ImageIndex = 0;
            this.zoomToolStripSplitButton.ImageIndex = 1;
            this.onepageToolStripButton.ImageIndex = 2;
            this.twopagesToolStripButton.ImageIndex = 3;
            this.threepagesToolStripButton.ImageIndex = 4;
            this.fourpagesToolStripButton.ImageIndex = 5;
            this.sixpagesToolStripButton.ImageIndex = 6;

            //tabIndex
            previewControl.TabIndex = 0;
            toolStrip1.TabIndex = 1;

            //DefaultItem on the Zoom SplitButton
            zoomToolStripSplitButton.DefaultItem = autoToolStripMenuItem;

            //ShowCheckMargin
            ToolStripDropDownMenu menu = this.zoomToolStripSplitButton.DropDown as ToolStripDropDownMenu;
            if (menu != null)
            {
                menu.ShowCheckMargin = true;
                menu.ShowImageMargin = false;
                menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
                
            }

            //Create the ToolStripControlHost
            ToolStripControlHost pageCounterItem = new ToolStripControlHost(pageCounter);
            pageCounterItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            this.toolStrip1.Items.Add(pageCounterItem);
            this.toolStrip1.Items.Add(this.pageToolStripLabel);

            // 
            // Form1
            // 
            resources.ApplyResources(this, "$this");

            this.Controls.Add(previewControl);
            this.Controls.Add(this.toolStrip1);

            this.ClientSize = new Size(400, 300);
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = SizeGripStyle.Hide;
            this.toolStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pageCounter)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();


        }
        

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.OnClosing"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Forces the preview to be regenerated every time the dialog comes up
        ///    </para>
        /// </devdoc>
        protected override void OnClosing(CancelEventArgs e) {
            base.OnClosing(e);
            previewControl.InvalidatePreview();
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.CreateHandle"]/*' />
        /// <devdoc>
        ///    <para>Creates the handle for the PrintPreviewDialog. If a
        ///       subclass overrides this function,
        ///       it must call the base implementation.</para>
        /// </devdoc>
        protected override void CreateHandle() {
            // We want to check printer settings before we push the modal message loop,
            // so the user has a chance to catch the exception instead of letting go to
            // the windows forms exception dialog.
            if (Document != null && !Document.PrinterSettings.IsValid)
                throw new InvalidPrinterException(Document.PrinterSettings);
            
            base.CreateHandle();
        }

        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessDialogKey(Keys keyData) {
           if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None) {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                switch (keyCode) {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        return false;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <devdoc>
        ///    <para>
        ///       In Everett we used to TAB around the PrintPreviewDialog. Now since the PageCounter is added into the ToolStrip we dont
        ///       This is breaking from Everett.
        ///    </para>
        /// </devdoc>
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        protected override bool ProcessTabKey(bool forward) {
            if (this.ActiveControl == this.previewControl)
            {
                this.pageCounter.FocusInternal();
                return true;
            }
            return false;
        }

        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ShouldSerializeAutoScaleBaseSize"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       AutoScaleBaseSize should never be persisted for PrintPreviewDialogs.
        ///    </para>
        /// </devdoc>
        internal override bool ShouldSerializeAutoScaleBaseSize() {
            // This method is called when the dialog is "contained" on another form.
            // We should use our own base size, not the base size of our container.
            return false;
        }
                
        /// <include file='doc\PrintPreviewDialog.uex' path='docs/doc[@for="PrintPreviewDialog.ShouldSerializeText"]/*' />
        internal override bool ShouldSerializeText() {
            return !Text.Equals(string.Format(SR.PrintPreviewDialog_PrintPreview));
        }

        void OncloseToolStripButtonClick(object sender, System.EventArgs e) {
            this.Close();
        }

        void previewControl_StartPageChanged(object sender, EventArgs e) {
            pageCounter.Value = previewControl.StartPage + 1;
        }

        
        void CheckZoomMenu(ToolStripMenuItem toChecked) {
            foreach (ToolStripMenuItem item in zoomToolStripSplitButton.DropDownItems) {
                item.Checked = toChecked == item;
            }
        }
        

        void ZoomAuto(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.AutoZoom = true;
        }

        void Zoom500(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 5.00;
        }

        void Zoom250(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 2.50;
        }

        void Zoom150(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 1.50;
        }

        void Zoom100(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 1.00;
        }

        void Zoom75(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .75;
        }

        void Zoom50(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .50;
        }

        void Zoom25(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .25;
        }

        void Zoom10(object sender, EventArgs eventargs) {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .10;
        }

        void OncloseToolStripButtonPaint(object sender, PaintEventArgs e)
        {
            ToolStripItem item = sender as ToolStripItem;
            if (item != null && !item.Selected)
            {
                Rectangle rect = new Rectangle (0, 0 , item.Bounds.Width - 1, item.Bounds.Height - 1);
                using (Pen pen = new Pen(SystemColors.ControlDark)) 
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }
            

        void OnprintToolStripButtonClick(object sender, System.EventArgs e) {
             if (previewControl.Document != null)
             {
                previewControl.Document.Print();
             }
        }
        
        void OnzoomToolStripSplitButtonClick(object sender, System.EventArgs e) {
            ZoomAuto(null, EventArgs.Empty);
        }

        //--------
        void OnonepageToolStripButtonClick(object sender, System.EventArgs e) {
             previewControl.Rows = 1;
             previewControl.Columns = 1;
        }

        void OntwopagesToolStripButtonClick(object sender, System.EventArgs e) {
             previewControl.Rows = 1;
             previewControl.Columns = 2;
        }

        void OnthreepagesToolStripButtonClick(object sender, System.EventArgs e) {
            previewControl.Rows = 1;
            previewControl.Columns = 3;
        }

        void OnfourpagesToolStripButtonClick(object sender, System.EventArgs e) {
             previewControl.Rows = 2;
             previewControl.Columns = 2;
        }

        void OnsixpagesToolStripButtonClick(object sender, System.EventArgs e) {
            previewControl.Rows = 2;
            previewControl.Columns = 3;
        }
        //----------------------


        void UpdownMove(object sender, EventArgs eventargs) {
            int pageNum = ((int)pageCounter.Value) - 1;
            if (pageNum >= 0) {
                // -1 because users like to count from one, and programmers from 0
                previewControl.StartPage = pageNum;

                // And previewControl_PropertyChanged will change it again,
                // ensuring it stays within legal bounds.
            }
            else {
                pageCounter.Value = previewControl.StartPage + 1;
            }
        }
    }
}

