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
    internal Size buttonSize = Size.Empty;
    /// <summary>
    ///  This represents the width of the drop down arrow we have if the
    ///  DropDownArrows property is true.  this value is used by the ToolBarButton
    ///  objects to compute their size
    /// </summary>
    internal const int DDARROW_WIDTH = 15;

    /// <summary>
    ///  The array of buttons we're working with.
    /// </summary>
    private ToolBarButton[] buttons;

    /// <summary>
    ///  The number of buttons we're working with
    /// </summary>
    private int buttonCount;

    /// <summary>
    ///  The ImageList object that contains the main images for our control.
    /// </summary>
    private ImageList imageList;

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
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnAutoSizeChangedDescr))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler AutoSizeChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Color BackColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Image BackgroundImage
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler BackgroundImageLayoutChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    ///  Sets the way in which this ToolBar is docked to its parent. We need to
    ///  override this to ensure autoSizing works correctly
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override DockStyle Dock
    {
        get { throw new PlatformNotSupportedException(); }

        set
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }

        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler ForeColorChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public ImeMode ImeMode
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler ImeModeChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public override RightToLeft RightToLeft
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler RightToLeftChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [DefaultValue(false)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public bool TabStop
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
    Bindable(false),
    DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public override string Text
    {
        get
        {
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    new public event EventHandler TextChanged
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
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
            throw new PlatformNotSupportedException();
        }
        set
        {
            throw new PlatformNotSupportedException();
        }
    }

    /// <summary>
    ///  Occurs when a <see cref='ToolBarButton'/> on the <see cref='ToolBar'/> is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event ToolBarButtonClickEventHandler ButtonClick
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  Occurs when a drop-down style <see cref='ToolBarButton'/> or its down arrow is clicked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public event ToolBarButtonClickEventHandler ButtonDropDown
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
    }

    /// <summary>
    ///  ToolBar Onpaint.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler Paint
    {
        add => throw new PlatformNotSupportedException();
        remove => throw new PlatformNotSupportedException();
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
                throw new PlatformNotSupportedException();
            }
            set
            {
                throw new PlatformNotSupportedException();
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
                throw new PlatformNotSupportedException();
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
                throw new PlatformNotSupportedException();
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
                throw new PlatformNotSupportedException();
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
