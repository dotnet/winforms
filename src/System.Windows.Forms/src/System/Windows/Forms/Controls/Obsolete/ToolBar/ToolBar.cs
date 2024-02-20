// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
#nullable disable
[Obsolete("ToolBar has been deprecated. Use ToolStrip instead.")]
public class ToolBar : Control
{
    private readonly ToolBarButtonCollection buttonsCollection;

    /// <summary>
    ///  The size of a button in the ToolBar
    /// </summary>
    internal Size buttonSize = System.Drawing.Size.Empty;
    /// <summary>
    ///  This represents the width of the drop down arrow we have if the
    ///  DropDownArrows property is true.  this value is used by the ToolBarButton
    ///  objects to compute their size
    /// </summary>
    internal const int DDARROW_WIDTH = 15;

    /// <summary>
    ///  Indicates what our appearance will be.  This will either be normal
    ///  or flat.
    /// </summary>
    private ToolBarAppearance appearance = ToolBarAppearance.Normal;

    /// <summary>
    ///  Indicates whether or not we have a border
    /// </summary>
    private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;

    /// <summary>
    ///  The array of buttons we're working with.
    /// </summary>
    private ToolBarButton[] buttons;

    /// <summary>
    ///  The number of buttons we're working with
    /// </summary>
    private int buttonCount;

    /// <summary>
    ///  Indicates if text captions should go underneath images in buttons or
    ///  to the right of them
    /// </summary>
    private ToolBarTextAlign textAlign = ToolBarTextAlign.Underneath;

    /// <summary>
    ///  The ImageList object that contains the main images for our control.
    /// </summary>
    private ImageList imageList;

    private const int TOOLBARSTATE_wrappable = 0x00000001;
    private const int TOOLBARSTATE_dropDownArrows = 0x00000002;
    private const int TOOLBARSTATE_divider = 0x00000004;
    private const int TOOLBARSTATE_showToolTips = 0x00000008;
    private const int TOOLBARSTATE_autoSize = 0x00000010;

    // PERF: take all the bools and put them into a state variable
    private Collections.Specialized.BitVector32 toolBarState; // see TOOLBARSTATE_ consts above

    // event handlers
    //
    private ToolBarButtonClickEventHandler onButtonClick;
    private ToolBarButtonClickEventHandler onButtonDropDown;

    /// <summary>
    ///  Initializes a new instance of the <see cref='ToolBar'/> class.
    /// </summary>
    public ToolBar()
    : base()
    {
        buttonsCollection = new ToolBarButtonCollection(this);
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Gets or sets the appearance of the toolbar
    ///  control and its buttons.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior)), DefaultValue(ToolBarAppearance.Normal),
    Localizable(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBarAppearance Appearance
    {
        get
        {
            return appearance;
        }

        set
        {
            if (value != appearance)
            {
                appearance = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Indicates whether the toolbar
    ///  adjusts its size automatically based on the size of the buttons and the
    ///  dock style.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior)),
    DefaultValue(true),
    Localizable(true),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
    public override bool AutoSize
    {
        get
        {
            return toolBarState[TOOLBARSTATE_autoSize];
        }

        set
        {
            // Note that we intentionally do not call base.  Toolbars size themselves by
            // overriding SetBoundsCore (old RTM code).  We let CommonProperties.GetAutoSize
            // continue to return false to keep our LayoutEngines from messing with TextBoxes.
            // This is done for backwards compatibility since the new AutoSize behavior differs.
            if (AutoSize != value)
            {
                toolBarState[TOOLBARSTATE_autoSize] = value;
                if (Dock == DockStyle.Left || Dock == DockStyle.Right)
                {
                    SetStyle(ControlStyles.FixedWidth, AutoSize);
                    SetStyle(ControlStyles.FixedHeight, false);
                }
                else
                {
                    SetStyle(ControlStyles.FixedHeight, AutoSize);
                    SetStyle(ControlStyles.FixedWidth, false);
                }

                OnAutoSizeChanged(EventArgs.Empty);
            }
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler AutoSizeChanged
    {
        add => base.AutoSizeChanged += value;
        remove => base.AutoSizeChanged -= value;
    }

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
    ///  Gets or sets
    ///  the border style of the toolbar control.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public BorderStyle BorderStyle
    {
        get
        {
            return borderStyle;
        }

        set
        {
            if (borderStyle != value)
            {
                borderStyle = value;

                RecreateHandle();   // Looks like we need to recreate the handle to avoid painting glitches
            }
        }
    }

    /// <summary>
    ///  A collection of <see cref='ToolBarButton'/> controls assigned to the
    ///  toolbar control. The property is read-only.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBarButtonCollection Buttons
    {
        get
        {
            return buttonsCollection;
        }
    }

    /// <summary>
    ///  Gets or sets
    ///  the size of the buttons on the toolbar control.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Size ButtonSize
    {
        get
        {
            if (buttonSize.IsEmpty)
            {
                if (TextAlign == ToolBarTextAlign.Underneath)
                {
                    return new Size(39, 36);    // Default button size
                }
                else
                {
                    return new Size(23, 22);    // Default button size
                }
            }
            else
            {
                return buttonSize;
            }
        }

        set
        {
            if (value.Width < 0 || value.Height < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(ButtonSize), value));
            }

            if (buttonSize != value)
            {
                buttonSize = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating
    ///  whether the toolbar displays a divider.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool Divider
    {
        get
        {
            return toolBarState[TOOLBARSTATE_divider];
        }

        set
        {
            if (Divider != value)
            {
                toolBarState[TOOLBARSTATE_divider] = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Sets the way in which this ToolBar is docked to its parent. We need to
    ///  override this to ensure autoSizing works correctly
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override DockStyle Dock
    {
        get { return base.Dock; }

        set
        {
            if (Dock != value)
            {
                if (value == DockStyle.Left || value == DockStyle.Right)
                {
                    SetStyle(ControlStyles.FixedWidth, AutoSize);
                    SetStyle(ControlStyles.FixedHeight, false);
                }
                else
                {
                    SetStyle(ControlStyles.FixedHeight, AutoSize);
                    SetStyle(ControlStyles.FixedWidth, false);
                }

                base.Dock = value;
            }
        }
    }

    /// <summary>
    ///  Gets or sets a value indicating whether drop-down buttons on a
    ///  toolbar display down arrows.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool DropDownArrows
    {
        get
        {
            return toolBarState[TOOLBARSTATE_dropDownArrows];
        }

        set
        {
            if (DropDownArrows != value)
            {
                toolBarState[TOOLBARSTATE_dropDownArrows] = value;
                RecreateHandle();
            }
        }
    }

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
    ///  Gets or sets the collection of images available to the toolbar button
    ///  controls.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ImageList ImageList
    {
        get
        {
            return imageList;
        }
        set
        {
            if (value != imageList)
            {
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  Gets the size of the images in the image list assigned to the
    ///  toolbar.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Size ImageSize
    {
        get
        {
            if (imageList is not null)
            {
                return imageList.ImageSize;
            }
            else
            {
                return new Size(0, 0);
            }
        }
    }

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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler RightToLeftChanged
    {
        add => base.RightToLeftChanged += value;
        remove => base.RightToLeftChanged -= value;
    }

    /// <summary>
    ///  Gets or sets a value indicating whether the toolbar displays a
    ///  tool tip for each button.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior)),
    DefaultValue(false),
    Localizable(true)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool ShowToolTips
    {
        get
        {
            return toolBarState[TOOLBARSTATE_showToolTips];
        }
        set
        {
            if (ShowToolTips != value)
            {
                toolBarState[TOOLBARSTATE_showToolTips] = value;
                RecreateHandle();
            }
        }
    }

    [DefaultValue(false)]
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

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    Bindable(false),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
    ///  Gets or sets the alignment of text in relation to each
    ///  image displayed on
    ///  the toolbar button controls.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance)),
    DefaultValue(ToolBarTextAlign.Underneath),
    Localizable(true)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBarTextAlign TextAlign
    {
        get
        {
            return textAlign;
        }
        set
        {
            if (textAlign == value)
            {
                return;
            }

            textAlign = value;
            RecreateHandle();
        }
    }

    /// <summary>
    ///  Gets
    ///  or sets a value
    ///  indicating whether the toolbar buttons wrap to the next line if the
    ///  toolbar becomes too small to display all the buttons
    ///  on the same line.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior)),
    DefaultValue(true),
    Localizable(true)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool Wrappable
    {
        get
        {
            return toolBarState[TOOLBARSTATE_wrappable];
        }
        set
        {
            if (Wrappable != value)
            {
                toolBarState[TOOLBARSTATE_wrappable] = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Occurs when a <see cref='ToolBarButton'/> on the <see cref='ToolBar'/> is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event ToolBarButtonClickEventHandler ButtonClick
    {
        add => onButtonClick += value;
        remove => onButtonClick -= value;
    }

    /// <summary>
    ///  Occurs when a drop-down style <see cref='ToolBarButton'/> or its down arrow is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event ToolBarButtonClickEventHandler ButtonDropDown
    {
        add => onButtonDropDown += value;
        remove => onButtonDropDown -= value;
    }

    /// <summary>
    ///  ToolBar Onpaint.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        buttonCount++;
        buttons = new ToolBarButton[buttonCount];
        imageList = new ImageList();
        throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Encapsulates a collection of <see cref='ToolBarButton'/> controls for use by the
    /// <see cref='ToolBar'/> class.
    /// </summary>
    [Obsolete("ToolBarButtonCollection has been deprecated.")]
    public class ToolBarButtonCollection : IList
    {
        private readonly ToolBar owner;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ToolBarButtonCollection'/> class and assigns it to the specified toolbar.
        /// </summary>
        public ToolBarButtonCollection(ToolBar owner)
        {
            this.owner = owner;
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Gets or sets the toolbar button at the specified indexed location in the
        ///  toolbar button collection.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ToolBarButton this[int index]
        {
            get
            {
                if (index < 0 || ((owner.buttons is not null) && (index >= owner.buttonCount)))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                return owner.buttons[index];
            }
            set
            {
                if (index < 0 || ((owner.buttons is not null) && index >= owner.buttonCount))
                {
                    throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
                }

                if (value is null)
                {
                    throw new ArgumentNullException(nameof(value));
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        object IList.this[int index]
        {
            get
            {
                return this[index];
            }
            set
            {
                if (value is ToolBarButton)
                {
                    this[index] = (ToolBarButton)value;
                }
                else
                {
                    throw new ArgumentException();
                }
            }
        }

        /// <summary>
        ///  Retrieves the child control with the specified key.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public virtual ToolBarButton this[string key]
        {
            get
            {
                // We do not support null and empty string as valid keys.
                if (string.IsNullOrEmpty(key))
                {
                    return null;
                }

                // Search for the key in our collection
                int index = IndexOfKey(key);
                return this[index];
            }
        }

        /// <summary>
        ///  Gets the number of buttons in the toolbar button collection.
        /// </summary>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public int Count
        {
            get
            {
                return owner.buttonCount;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        object ICollection.SyncRoot
        {
            get
            {
                return this;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool ICollection.IsSynchronized
        {
            get
            {
                return false;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        bool IList.IsFixedSize
        {
            get
            {
                return false;
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        ///  Adds a new toolbar button to
        ///  the end of the toolbar button collection.
        /// </summary>
        public int Add(ToolBarButton button)
        {
            throw new PlatformNotSupportedException();
        }

        public int Add(string text)
        {
            throw new PlatformNotSupportedException();
        }

        int IList.Add(object button)
        {
            throw new PlatformNotSupportedException();
        }

        public void AddRange(ToolBarButton[] buttons)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes
        ///  all buttons from the toolbar button collection.
        /// </summary>
        public void Clear()
        {
            throw new PlatformNotSupportedException();
        }

        public bool Contains(ToolBarButton button)
        {
            throw new PlatformNotSupportedException();
        }

        bool IList.Contains(object button)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Returns true if the collection contains an item with the specified key, false otherwise.
        /// </summary>
        public virtual bool ContainsKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        void ICollection.CopyTo(Array dest, int index)
        {
            throw new PlatformNotSupportedException();
        }

        public int IndexOf(ToolBarButton button)
        {
            throw new PlatformNotSupportedException();
        }

        int IList.IndexOf(object button)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  The zero-based index of the first occurrence of value within the entire CollectionBase, if found; otherwise, -1.
        /// </summary>
        public virtual int IndexOfKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        public void Insert(int index, ToolBarButton button)
        {
            throw new PlatformNotSupportedException();
        }

        void IList.Insert(int index, object button)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes
        ///  a given button from the toolbar button collection.
        /// </summary>
        public void RemoveAt(int index)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Removes the child control with the specified key.
        /// </summary>
        public virtual void RemoveByKey(string key)
        {
            throw new PlatformNotSupportedException();
        }

        public void Remove(ToolBarButton button)
        {
            throw new PlatformNotSupportedException();
        }

        void IList.Remove(object button)
        {
            throw new PlatformNotSupportedException();
        }

        /// <summary>
        ///  Returns an enumerator that can be used to iterate
        ///  through the toolbar button collection.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            throw new PlatformNotSupportedException();
        }
    }
}
