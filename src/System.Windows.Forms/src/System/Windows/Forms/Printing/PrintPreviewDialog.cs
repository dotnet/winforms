// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Printing;
using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Represents a
    ///  dialog box form that contains a <see cref='Forms.PrintPreviewControl'/>.
    /// </summary>
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
    public class PrintPreviewDialog : Form
    {
        readonly PrintPreviewControl previewControl;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private ToolStripNumericUpDown pageCounterItem;
        private NumericUpDown pageCounter;
        private ToolStripButton printToolStripButton;
        private ToolStripSplitButton zoomToolStripSplitButton;
        private ToolStripMenuItem autoToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
        private ToolStripMenuItem toolStripMenuItem4;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripMenuItem toolStripMenuItem8;
        private ToolStripSeparator separatorToolStripSeparator;
        private ToolStripButton onepageToolStripButton;
        private ToolStripButton twopagesToolStripButton;
        private ToolStripButton threepagesToolStripButton;
        private ToolStripButton fourpagesToolStripButton;
        private ToolStripButton sixpagesToolStripButton;
        private ToolStripSeparator separatorToolStripSeparator1;
        private ToolStripButton closeToolStripButton;
        private ToolStripLabel pageToolStripLabel;

        readonly ImageList imageList;

        /// <summary>
        ///  Initializes a new instance of the <see cref='PrintPreviewDialog'/> class.
        /// </summary>
        public PrintPreviewDialog()
        {
#pragma warning disable 618
            base.AutoScaleBaseSize = new Size(5, 13);
#pragma warning restore 618

            previewControl = new PrintPreviewControl();
            imageList = new ImageList();
            imageList.Images.AddStrip(DpiHelper.GetBitmapFromIcon(typeof(PrintPreviewDialog), "PrintPreviewStrip"));
            InitForm();
        }

        //subhag addition
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        ///  Indicates the <see cref='Button'/> control on the form that is clicked when
        ///  the user presses the ENTER key.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public IButtonControl AcceptButton
        {
            get
            {
                return base.AcceptButton;
            }
            set
            {
                base.AcceptButton = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether the form will adjust its size
        ///  to fit the height of the font used on the form and scale
        ///  its controls.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool AutoScale
        {
            get
            {
#pragma warning disable 618
                return base.AutoScale;
#pragma warning restore 618
            }
            set
            {
#pragma warning disable 618
                base.AutoScale = value;
#pragma warning restore 618
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the form implements
        ///  autoscrolling.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AutoScroll
        {
            get
            {

                return base.AutoScroll;
            }
            set
            {
                base.AutoScroll = value;
            }
        }

        /// <summary>
        ///  Hide the property
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
                base.AutoSize = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler AutoSizeChanged
        {
            add => base.AutoSizeChanged += value;
            remove => base.AutoSizeChanged -= value;
        }

        /// <summary>
        ///  Hide the property
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AutoValidate AutoValidate
        {
            get
            {
                return base.AutoValidate;
            }
            set
            {
                base.AutoValidate = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler AutoValidateChanged
        {
            add => base.AutoValidateChanged += value;
            remove => base.AutoValidateChanged -= value;
        }

        /// <summary>
        ///  The background color of this control. This is an ambient property and
        ///  will always return a non-null value.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color BackColor
        {
            get
            {
                return base.BackColor;
            }
            set
            {
                base.BackColor = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackColorChanged
        {
            add => base.BackColorChanged += value;
            remove => base.BackColorChanged -= value;
        }
        /// <summary>
        ///  Gets
        ///  or
        ///  sets the button control that will be clicked when the
        ///  user presses the ESC key.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public IButtonControl CancelButton
        {
            get
            {
                return base.CancelButton;
            }
            set
            {
                base.CancelButton = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether a control box is displayed in the
        ///  caption bar of the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool ControlBox
        {
            get
            {
                return base.ControlBox;
            }
            set
            {
                base.ControlBox = value;
            }
        }

        /// <summary>
        ///  Hide the property
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return base.ContextMenuStrip;
            }
            set
            {
                base.ContextMenuStrip = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ContextMenuStripChanged
        {
            add => base.ContextMenuStripChanged += value;
            remove => base.ContextMenuStripChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the border style of the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public FormBorderStyle FormBorderStyle
        {
            get
            {
                return base.FormBorderStyle;
            }
            set
            {
                base.FormBorderStyle = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether a
        ///  help button should be displayed in the caption box of the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool HelpButton
        {
            get
            {
                return base.HelpButton;
            }
            set
            {
                base.HelpButton = value;
            }
        }
        /// <summary>
        ///  Gets or sets the icon for the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Icon Icon
        {
            get
            {
                return base.Icon;
            }
            set
            {
                base.Icon = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether the form is a container for multiple document interface
        ///  (MDI) child forms.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool IsMdiContainer
        {
            get
            {
                return base.IsMdiContainer;
            }
            set
            {
                base.IsMdiContainer = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value
        ///  indicating whether the form will receive key events
        ///  before the event is passed to the control that has focus.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool KeyPreview
        {
            get
            {
                return base.KeyPreview;
            }
            set
            {
                base.KeyPreview = value;
            }
        }
        /// <summary>
        ///  Gets or Sets the maximum size the dialog can be resized to.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size MaximumSize
        {
            get
            {
                return base.MaximumSize;
            }
            set
            {
                base.MaximumSize = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MaximumSizeChanged
        {
            add => base.MaximumSizeChanged += value;
            remove => base.MaximumSizeChanged -= value;
        }
        /// <summary>
        ///  Gets or sets a value indicating whether the maximize button is
        ///  displayed in the caption bar of the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool MaximizeBox
        {
            get
            {
                return base.MaximizeBox;
            }
            set
            {
                base.MaximizeBox = value;
            }
        }

        /// <summary>
        ///  Hide the value
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Padding Margin
        {
            get
            {
                return base.Margin;
            }
            set
            {
                base.Margin = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler MarginChanged
        {
            add => base.MarginChanged += value;
            remove => base.MarginChanged -= value;
        }

        /// <summary>
        ///  Gets the minimum size the form can be resized to.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size MinimumSize
        {
            get
            {
                return base.MinimumSize;
            }
            set
            {
                base.MinimumSize = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler MinimumSizeChanged
        {
            add => base.MinimumSizeChanged += value;
            remove => base.MinimumSizeChanged -= value;
        }

        /// <summary>
        ///  Hide the value
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Padding Padding
        {
            get
            {
                return base.Padding;
            }
            set
            {
                base.Padding = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler PaddingChanged
        {
            add => base.PaddingChanged += value;
            remove => base.PaddingChanged -= value;
        }

        /// <summary>
        ///  Gets or sets the size of the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size Size
        {
            get
            {
                return base.Size;
            }
            set
            {
                base.Size = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler SizeChanged
        {
            add => base.SizeChanged += value;
            remove => base.SizeChanged -= value;
        }
        /// <summary>
        ///  Gets or sets the
        ///  starting position of the form at run time.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public FormStartPosition StartPosition
        {
            get
            {
                return base.StartPosition;
            }
            set
            {
                base.StartPosition = value;
            }
        }
        /// <summary>
        ///  Gets or sets a value indicating whether the form should be displayed as the top-most
        ///  form of your application.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TopMost
        {
            get
            {
                return base.TopMost;
            }
            set
            {
                base.TopMost = value;
            }
        }

        /// <summary>
        ///  Gets or sets the color that will represent transparent areas of the form.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Color TransparencyKey
        {
            get
            {
                return base.TransparencyKey;
            }
            set
            {
                base.TransparencyKey = value;
            }
        }

        /// <summary>
        ///  Hide the value
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool UseWaitCursor
        {
            get
            {
                return base.UseWaitCursor;
            }
            set
            {
                base.UseWaitCursor = value;
            }
        }

        /// <summary>
        ///  Gets or sets the form's window state.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public FormWindowState WindowState
        {
            get
            {
                return base.WindowState;
            }
            set
            {
                base.WindowState = value;
            }
        }
        /// <summary>
        ///  The accessible role of the control
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public AccessibleRole AccessibleRole
        {
            get
            {
                return base.AccessibleRole;
            }
            set
            {
                base.AccessibleRole = value;
            }
        }
        /// <summary>
        ///  The accessible description of the control
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public string AccessibleDescription
        {
            get
            {
                return base.AccessibleDescription;
            }
            set
            {
                base.AccessibleDescription = value;
            }
        }
        /// <summary>
        ///  The accessible name of the control
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public string AccessibleName
        {
            get
            {
                return base.AccessibleName;
            }
            set
            {
                base.AccessibleName = value;
            }
        }
        /// <summary>
        ///
        ///  Indicates whether entering the control causes validation on the controls requiring validation.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool CausesValidation
        {
            get
            {
                return base.CausesValidation;
            }
            set
            {
                base.CausesValidation = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CausesValidationChanged
        {
            add => base.CausesValidationChanged += value;
            remove => base.CausesValidationChanged -= value;
        }
        /// <summary>
        ///  Retrieves the bindings for this control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ControlBindingsCollection DataBindings
        {
            get
            {
                return base.DataBindings;
            }
        }

        protected override Size DefaultMinimumSize
        {
            get { return new Size(375, 250); }
        }

        /// <summary>
        ///  Indicates whether the control is currently enabled.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool Enabled
        {
            get
            {
                return base.Enabled;
            }
            set
            {
                base.Enabled = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler EnabledChanged
        {
            add => base.EnabledChanged += value;
            remove => base.EnabledChanged -= value;
        }
        /// <summary>
        ///  The location of this control.
        /// </summary>
        [Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        new public Point Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                base.Location = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler LocationChanged
        {
            add => base.LocationChanged += value;
            remove => base.LocationChanged -= value;
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public object Tag
        {
            get
            {
                return base.Tag;
            }
            set
            {
                base.Tag = value;
            }
        }
        /// <summary>
        ///  The AllowDrop property. If AllowDrop is set to true then
        ///  this control will allow drag and drop operations and events to be used.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop
        {
            get
            {
                return base.AllowDrop;
            }
            set
            {
                base.AllowDrop = value;
            }
        }
        /// <summary>
        ///  Retrieves the cursor that will be displayed when the mouse is over this
        ///  control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Cursor Cursor
        {
            get
            {
                return base.Cursor;
            }
            set
            {
                base.Cursor = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CursorChanged
        {
            add => base.CursorChanged += value;
            remove => base.CursorChanged -= value;
        }

        /// <summary>
        ///  The background image of the control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get
            {
                return base.BackgroundImage;
            }
            set
            {
                base.BackgroundImage = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        /// <summary>
        ///  The background image layout of the control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout
        {
            get
            {
                return base.BackgroundImageLayout;
            }
            set
            {
                base.BackgroundImageLayout = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }
        /// <summary>
        ///  Specifies a value that determines the IME (Input Method Editor) status of the
        ///  object when that object is selected.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode
        {
            get
            {
                return base.ImeMode;
            }
            set
            {
                base.ImeMode = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged
        {
            add => base.ImeModeChanged += value;
            remove => base.ImeModeChanged -= value;
        }

        /// <summary>
        ///  Gets or
        ///  sets the size of the auto-scroll
        ///  margin.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size AutoScrollMargin
        {
            get
            {
                return base.AutoScrollMargin;
            }
            set
            {
                base.AutoScrollMargin = value;
            }
        }
        /// <summary>
        ///  Gets or sets the mimimum size of the auto-scroll.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public Size AutoScrollMinSize
        {
            get
            {
                return base.AutoScrollMinSize;
            }
            set
            {
                base.AutoScrollMinSize = value;
            }
        }
        /// <summary>
        ///  The current value of the anchor property. The anchor property
        ///  determines which edges of the control are anchored to the container's
        ///  edges.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override AnchorStyles Anchor
        {
            get
            {
                return base.Anchor;
            }
            set
            {
                base.Anchor = value;
            }
        }
        /// <summary>
        ///  Indicates whether the control is visible.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool Visible
        {
            get
            {
                return base.Visible;
            }
            set
            {
                base.Visible = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler VisibleChanged
        {
            add => base.VisibleChanged += value;
            remove => base.VisibleChanged -= value;
        }
        /// <summary>
        ///  The foreground color of the control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged
        {
            add => base.ForeColorChanged += value;
            remove => base.ForeColorChanged -= value;
        }

        /// <summary>
        ///  This is used for international applications where the language
        ///  is written from RightToLeft. When this property is true,
        ///  control placement and text will be from right to left.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override RightToLeft RightToLeft
        {
            get
            {
                return base.RightToLeft;
            }
            set
            {
                base.RightToLeft = value;
            }
        }

        /// <summary>
        ///  This is used for international applications where the language
        ///  is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///  control placement and text will be from right to left.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool RightToLeftLayout
        {
            get
            {

                return base.RightToLeftLayout;
            }

            set
            {
                base.RightToLeftLayout = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftChanged
        {
            add => base.RightToLeftChanged += value;
            remove => base.RightToLeftChanged -= value;
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftLayoutChanged
        {
            add => base.RightToLeftLayoutChanged += value;
            remove => base.RightToLeftLayoutChanged -= value;
        }

        /// <summary>
        ///  Indicates whether the user can give the focus to this control using the TAB
        ///  key. This property is read-only.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TabStop
        {
            get
            {
                return base.TabStop;
            }
            set
            {
                base.TabStop = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged
        {
            add => base.TabStopChanged += value;
            remove => base.TabStopChanged -= value;
        }

        /// <summary>
        ///  The current text associated with this control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get
            {
                return base.Text;
            }
            set
            {
                base.Text = value;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
        {
            add => base.TextChanged += value;
            remove => base.TextChanged -= value;
        }

        /// <summary>
        ///  The dock property. The dock property controls to which edge
        ///  of the container this control is docked to. For example, when docked to
        ///  the top of the container, the control will be displayed flush at the
        ///  top of the container, extending the length of the container.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override DockStyle Dock
        {
            get
            {
                return base.Dock;
            }
            set
            {
                base.Dock = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler DockChanged
        {
            add => base.DockChanged += value;
            remove => base.DockChanged -= value;
        }

        /// <summary>
        ///  Retrieves the current font for this control. This will be the font used
        ///  by default for painting and text in the control.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font
        {
            get
            {
                return base.Font;
            }
            set
            {
                base.Font = value;
            }
        }
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged
        {
            add => base.FontChanged += value;
            remove => base.FontChanged -= value;
        }

        // DockPadding is not relevant to UpDownBase
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public DockPaddingEdges DockPadding
        {
            get
            {
                return base.DockPadding;
            }
        }
        //-------------------------------------------------------------------------------------------------------------
        //end addition

        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.PrintPreviewAntiAliasDescr))
        ]
        public bool UseAntiAlias
        {
            get
            {
                return PrintPreviewControl.UseAntiAlias;
            }
            set
            {
                PrintPreviewControl.UseAntiAlias = value;
            }
        }

        /// <summary>
        ///  PrintPreviewDialog does not support AutoScaleBaseSize.
        /// </summary>
        ///  Keeping implementation of obsoleted AutoScaleBaseSize API
#pragma warning disable 618
        // disable csharp compiler warning #0809: obsolete member overrides non-obsolete member
#pragma warning disable 0809
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("This property has been deprecated. Use the AutoScaleDimensions property instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public override Size AutoScaleBaseSize
        {
            get
            {
                return base.AutoScaleBaseSize;
            }

            set
            {
                // No-op
            }
        }
#pragma warning restore 0809
#pragma warning restore 618

        /// <summary>
        ///  Gets or sets the document to preview.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.PrintPreviewDocumentDescr))
        ]
        public PrintDocument Document
        {
            get
            {
                return previewControl.Document;
            }
            set
            {
                previewControl.Document = value;
            }
        }

        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool MinimizeBox
        {
            get
            {
                return base.MinimizeBox;
            }
            set
            {
                base.MinimizeBox = value;
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating the <see cref='Forms.PrintPreviewControl'/>
        ///  contained in this form.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.PrintPreviewPrintPreviewControlDescr)),
        Browsable(false)
        ]
        public PrintPreviewControl PrintPreviewControl
        {
            get { return previewControl; }
        }

        /// <summary>
        ///  Opacity does not apply to PrintPreviewDialogs.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public new double Opacity
        {
            get
            {
                return base.Opacity;
            }
            set
            {
                base.Opacity = value;
            }
        }

        [Browsable(false), DefaultValue(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool ShowInTaskbar
        {
            get
            {
                return base.ShowInTaskbar;
            }
            set
            {
                base.ShowInTaskbar = value;
            }
        }

        [Browsable(false), DefaultValue(SizeGripStyle.Hide), EditorBrowsable(EditorBrowsableState.Never)]
        public new SizeGripStyle SizeGripStyle
        {
            get
            {
                return base.SizeGripStyle;
            }
            set
            {
                base.SizeGripStyle = value;
            }
        }

        void InitForm()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(PrintPreviewDialog));
            toolStrip1 = new ToolStrip();
            printToolStripButton = new ToolStripButton();
            zoomToolStripSplitButton = new ToolStripSplitButton();
            autoToolStripMenuItem = new ToolStripMenuItem();
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            toolStripMenuItem4 = new ToolStripMenuItem();
            toolStripMenuItem5 = new ToolStripMenuItem();
            toolStripMenuItem6 = new ToolStripMenuItem();
            toolStripMenuItem7 = new ToolStripMenuItem();
            toolStripMenuItem8 = new ToolStripMenuItem();
            separatorToolStripSeparator = new ToolStripSeparator();
            onepageToolStripButton = new ToolStripButton();
            twopagesToolStripButton = new ToolStripButton();
            threepagesToolStripButton = new ToolStripButton();
            fourpagesToolStripButton = new ToolStripButton();
            sixpagesToolStripButton = new ToolStripButton();
            separatorToolStripSeparator1 = new ToolStripSeparator();
            closeToolStripButton = new ToolStripButton();
            pageCounterItem = new ToolStripNumericUpDown();
            pageCounter = pageCounterItem.NumericUpDownControl;
            pageToolStripLabel = new System.Windows.Forms.ToolStripLabel();
            toolStrip1.SuspendLayout();
            SuspendLayout();

            //
            // toolStrip1
            //
            resources.ApplyResources(toolStrip1, "toolStrip1");
            toolStrip1.Items.AddRange(new ToolStripItem[] {
            printToolStripButton,
            zoomToolStripSplitButton,
            separatorToolStripSeparator,
            onepageToolStripButton,
            twopagesToolStripButton,
            threepagesToolStripButton,
            fourpagesToolStripButton,
            sixpagesToolStripButton,
            separatorToolStripSeparator1,
            closeToolStripButton});
            toolStrip1.Name = "toolStrip1";

            // in High Contrast mode the color scheme provided by ToolStripSystemRenderer 
            // is not sufficiently contrast; so disable it in High Contrast mode.
            if (!SystemInformation.HighContrast)
            {
                toolStrip1.RenderMode = ToolStripRenderMode.System;
            }

            toolStrip1.GripStyle = ToolStripGripStyle.Hidden;

            //
            // printToolStripButton
            //
            printToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            printToolStripButton.Name = "printToolStripButton";
            resources.ApplyResources(printToolStripButton, "printToolStripButton");

            //
            // zoomToolStripSplitButton
            //
            zoomToolStripSplitButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            zoomToolStripSplitButton.DoubleClickEnabled = true;
            zoomToolStripSplitButton.DropDownItems.AddRange(new ToolStripItem[] {
            autoToolStripMenuItem,
            toolStripMenuItem1,
            toolStripMenuItem2,
            toolStripMenuItem3,
            toolStripMenuItem4,
            toolStripMenuItem5,
            toolStripMenuItem6,
            toolStripMenuItem7,
            toolStripMenuItem8});
            zoomToolStripSplitButton.Name = "zoomToolStripSplitButton";
            zoomToolStripSplitButton.SplitterWidth = 1;
            resources.ApplyResources(zoomToolStripSplitButton, "zoomToolStripSplitButton");

            //
            // autoToolStripMenuItem
            //
            autoToolStripMenuItem.CheckOnClick = true;
            autoToolStripMenuItem.DoubleClickEnabled = true;
            autoToolStripMenuItem.Checked = true;
            autoToolStripMenuItem.Name = "autoToolStripMenuItem";
            resources.ApplyResources(autoToolStripMenuItem, "autoToolStripMenuItem");

            //
            // toolStripMenuItem1
            //
            toolStripMenuItem1.CheckOnClick = true;
            toolStripMenuItem1.DoubleClickEnabled = true;
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            resources.ApplyResources(toolStripMenuItem1, "toolStripMenuItem1");

            //
            // toolStripMenuItem2
            //
            toolStripMenuItem2.CheckOnClick = true;
            toolStripMenuItem2.DoubleClickEnabled = true;
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            resources.ApplyResources(toolStripMenuItem2, "toolStripMenuItem2");

            //
            // toolStripMenuItem3
            //
            toolStripMenuItem3.CheckOnClick = true;
            toolStripMenuItem3.DoubleClickEnabled = true;
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            resources.ApplyResources(toolStripMenuItem3, "toolStripMenuItem3");

            //
            // toolStripMenuItem4
            //
            toolStripMenuItem4.CheckOnClick = true;
            toolStripMenuItem4.DoubleClickEnabled = true;
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            resources.ApplyResources(toolStripMenuItem4, "toolStripMenuItem4");

            //
            // toolStripMenuItem5
            //
            toolStripMenuItem5.CheckOnClick = true;
            toolStripMenuItem5.DoubleClickEnabled = true;
            toolStripMenuItem5.Name = "toolStripMenuItem5";
            resources.ApplyResources(toolStripMenuItem5, "toolStripMenuItem5");

            //
            // toolStripMenuItem6
            //
            toolStripMenuItem6.CheckOnClick = true;
            toolStripMenuItem6.DoubleClickEnabled = true;
            toolStripMenuItem6.Name = "toolStripMenuItem6";
            resources.ApplyResources(toolStripMenuItem6, "toolStripMenuItem6");

            //
            // toolStripMenuItem7
            //
            toolStripMenuItem7.CheckOnClick = true;
            toolStripMenuItem7.DoubleClickEnabled = true;
            toolStripMenuItem7.Name = "toolStripMenuItem7";
            resources.ApplyResources(toolStripMenuItem7, "toolStripMenuItem7");

            //
            // toolStripMenuItem8
            //
            toolStripMenuItem8.CheckOnClick = true;
            toolStripMenuItem8.DoubleClickEnabled = true;
            toolStripMenuItem8.Name = "toolStripMenuItem8";
            resources.ApplyResources(toolStripMenuItem8, "toolStripMenuItem8");

            //
            // separatorToolStripSeparator
            //
            separatorToolStripSeparator.Name = "separatorToolStripSeparator";

            //
            // onepageToolStripButton
            //
            onepageToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            onepageToolStripButton.Name = "onepageToolStripButton";
            resources.ApplyResources(onepageToolStripButton, "onepageToolStripButton");

            //
            // twopagesToolStripButton
            //
            twopagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            twopagesToolStripButton.Name = "twopagesToolStripButton";
            resources.ApplyResources(twopagesToolStripButton, "twopagesToolStripButton");

            //
            // threepagesToolStripButton
            //
            threepagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            threepagesToolStripButton.Name = "threepagesToolStripButton";
            resources.ApplyResources(threepagesToolStripButton, "threepagesToolStripButton");

            //
            // fourpagesToolStripButton
            //
            fourpagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            fourpagesToolStripButton.Name = "fourpagesToolStripButton";
            resources.ApplyResources(fourpagesToolStripButton, "fourpagesToolStripButton");

            //
            // sixpagesToolStripButton
            //
            sixpagesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            sixpagesToolStripButton.Name = "sixpagesToolStripButton";
            resources.ApplyResources(sixpagesToolStripButton, "sixpagesToolStripButton");

            //
            // separatorToolStripSeparator1
            //
            separatorToolStripSeparator1.Name = "separatorToolStripSeparator1";

            //
            // closeToolStripButton
            //
            closeToolStripButton.DisplayStyle = ToolStripItemDisplayStyle.Text;
            closeToolStripButton.Name = "closeToolStripButton";
            resources.ApplyResources(closeToolStripButton, "closeToolStripButton");

            //
            // pageCounter
            //
            resources.ApplyResources(pageCounter, "pageCounter");
            pageCounter.Text = "1";
            pageCounter.TextAlign = HorizontalAlignment.Right;
            pageCounter.DecimalPlaces = 0;
            pageCounter.Minimum = new decimal(0d);
            pageCounter.Maximum = new decimal(1000d);
            pageCounter.ValueChanged += new EventHandler(UpdownMove);
            pageCounter.Name = "pageCounter";

            //
            // pageToolStripLabel
            //
            pageToolStripLabel.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            pageToolStripLabel.Name = "pageToolStripLabel";
            resources.ApplyResources(pageToolStripLabel, "pageToolStripLabel");

            previewControl.Size = new Size(792, 610);
            previewControl.Location = new Point(0, 43);
            previewControl.Dock = DockStyle.Fill;
            previewControl.StartPageChanged += new EventHandler(previewControl_StartPageChanged);

            //EVENTS and Images ...
            printToolStripButton.Click += new EventHandler(OnprintToolStripButtonClick);
            autoToolStripMenuItem.Click += new EventHandler(ZoomAuto);
            toolStripMenuItem1.Click += new EventHandler(Zoom500);
            toolStripMenuItem2.Click += new EventHandler(Zoom250);
            toolStripMenuItem3.Click += new EventHandler(Zoom150);
            toolStripMenuItem4.Click += new EventHandler(Zoom100);
            toolStripMenuItem5.Click += new EventHandler(Zoom75);
            toolStripMenuItem6.Click += new EventHandler(Zoom50);
            toolStripMenuItem7.Click += new EventHandler(Zoom25);
            toolStripMenuItem8.Click += new EventHandler(Zoom10);
            onepageToolStripButton.Click += new EventHandler(OnonepageToolStripButtonClick);
            twopagesToolStripButton.Click += new EventHandler(OntwopagesToolStripButtonClick);
            threepagesToolStripButton.Click += new EventHandler(OnthreepagesToolStripButtonClick);
            fourpagesToolStripButton.Click += new EventHandler(OnfourpagesToolStripButtonClick);
            sixpagesToolStripButton.Click += new EventHandler(OnsixpagesToolStripButtonClick);
            closeToolStripButton.Click += new EventHandler(OncloseToolStripButtonClick);
            closeToolStripButton.Paint += new PaintEventHandler(OncloseToolStripButtonPaint);
            //Images
            toolStrip1.ImageList = imageList;
            printToolStripButton.ImageIndex = 0;
            zoomToolStripSplitButton.ImageIndex = 1;
            onepageToolStripButton.ImageIndex = 2;
            twopagesToolStripButton.ImageIndex = 3;
            threepagesToolStripButton.ImageIndex = 4;
            fourpagesToolStripButton.ImageIndex = 5;
            sixpagesToolStripButton.ImageIndex = 6;

            //tabIndex
            previewControl.TabIndex = 0;
            toolStrip1.TabIndex = 1;

            //DefaultItem on the Zoom SplitButton
            zoomToolStripSplitButton.DefaultItem = autoToolStripMenuItem;

            //ShowCheckMargin
            if (zoomToolStripSplitButton.DropDown is ToolStripDropDownMenu menu)
            {
                menu.ShowCheckMargin = true;
                menu.ShowImageMargin = false;
                menu.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;

            }

            //Create the ToolStripControlHost
            pageCounterItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;

            toolStrip1.Items.Add(pageCounterItem);
            toolStrip1.Items.Add(pageToolStripLabel);

            //
            // Form1
            //
            resources.ApplyResources(this, "$this");

            Controls.Add(previewControl);
            Controls.Add(toolStrip1);

            ClientSize = new Size(400, 300);
            MinimizeBox = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            toolStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

        }

        /// <summary>
        ///  Forces the preview to be regenerated every time the dialog comes up
        /// </summary>
        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            previewControl.InvalidatePreview();
        }

        /// <summary>
        ///  Creates the handle for the PrintPreviewDialog. If a
        ///  subclass overrides this function,
        ///  it must call the base implementation.
        /// </summary>
        protected override void CreateHandle()
        {
            // We want to check printer settings before we push the modal message loop,
            // so the user has a chance to catch the exception instead of letting go to
            // the windows forms exception dialog.
            if (Document != null && !Document.PrinterSettings.IsValid)
            {
                throw new InvalidPrinterException(Document.PrinterSettings);
            }

            base.CreateHandle();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & (Keys.Alt | Keys.Control)) == Keys.None)
            {
                Keys keyCode = (Keys)keyData & Keys.KeyCode;
                switch (keyCode)
                {
                    case Keys.Left:
                    case Keys.Right:
                    case Keys.Up:
                    case Keys.Down:
                        return false;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        ///  In Everett we used to TAB around the PrintPreviewDialog. Now since the PageCounter is added into the ToolStrip we dont
        ///  This is breaking from Everett.
        /// </summary>
        protected override bool ProcessTabKey(bool forward)
        {
            if (ActiveControl == previewControl)
            {
                pageCounter.Focus();
                return true;
            }
            return false;
        }

        /// <summary>
        ///  AutoScaleBaseSize should never be persisted for PrintPreviewDialogs.
        /// </summary>
        internal override bool ShouldSerializeAutoScaleBaseSize()
        {
            // This method is called when the dialog is "contained" on another form.
            // We should use our own base size, not the base size of our container.
            return false;
        }

        internal override bool ShouldSerializeText()
        {
            return !Text.Equals(SR.PrintPreviewDialog_PrintPreview);
        }

        void OncloseToolStripButtonClick(object sender, EventArgs e)
        {
            Close();
        }

        void previewControl_StartPageChanged(object sender, EventArgs e)
        {
            pageCounter.Value = previewControl.StartPage + 1;
        }

        void CheckZoomMenu(ToolStripMenuItem toChecked)
        {
            foreach (ToolStripMenuItem item in zoomToolStripSplitButton.DropDownItems)
            {
                item.Checked = toChecked == item;
            }
        }

        void ZoomAuto(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.AutoZoom = true;
        }

        void Zoom500(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 5.00;
        }

        void Zoom250(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 2.50;
        }

        void Zoom150(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 1.50;
        }

        void Zoom100(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = 1.00;
        }

        void Zoom75(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .75;
        }

        void Zoom50(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .50;
        }

        void Zoom25(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .25;
        }

        void Zoom10(object sender, EventArgs eventargs)
        {
            ToolStripMenuItem item = sender as ToolStripMenuItem;
            CheckZoomMenu(item);
            previewControl.Zoom = .10;
        }

        void OncloseToolStripButtonPaint(object sender, PaintEventArgs e)
        {
            if (sender is ToolStripItem item && !item.Selected)
            {
                Rectangle rect = new Rectangle(0, 0, item.Bounds.Width - 1, item.Bounds.Height - 1);
                using (Pen pen = new Pen(SystemColors.ControlDark))
                {
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }

        void OnprintToolStripButtonClick(object sender, EventArgs e)
        {
            if (previewControl.Document != null)
            {
                previewControl.Document.Print();
            }
        }

        void OnzoomToolStripSplitButtonClick(object sender, EventArgs e)
        {
            ZoomAuto(null, EventArgs.Empty);
        }

        //--------
        void OnonepageToolStripButtonClick(object sender, EventArgs e)
        {
            previewControl.Rows = 1;
            previewControl.Columns = 1;
        }

        void OntwopagesToolStripButtonClick(object sender, EventArgs e)
        {
            previewControl.Rows = 1;
            previewControl.Columns = 2;
        }

        void OnthreepagesToolStripButtonClick(object sender, EventArgs e)
        {
            previewControl.Rows = 1;
            previewControl.Columns = 3;
        }

        void OnfourpagesToolStripButtonClick(object sender, EventArgs e)
        {
            previewControl.Rows = 2;
            previewControl.Columns = 2;
        }

        void OnsixpagesToolStripButtonClick(object sender, EventArgs e)
        {
            previewControl.Rows = 2;
            previewControl.Columns = 3;
        }
        //----------------------

        void UpdownMove(object sender, EventArgs eventargs)
        {
            int pageNum = ((int)pageCounter.Value) - 1;
            if (pageNum >= 0)
            {
                // -1 because users like to count from one, and programmers from 0
                previewControl.StartPage = pageNum;

                // And previewControl_PropertyChanged will change it again,
                // ensuring it stays within legal bounds.
            }
            else
            {
                pageCounter.Value = previewControl.StartPage + 1;
            }
        }
    }
}

