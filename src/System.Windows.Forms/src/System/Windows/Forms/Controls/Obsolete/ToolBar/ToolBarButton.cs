// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms;

#pragma warning disable RS0016 // Add public types and members to the declared API
#nullable disable
[Obsolete("ToolBarButton has been deprecated. Use ToolStripButton instead.")]
public class ToolBarButton : Component
{
    private string text;
    private string name;
    private string tooltipText;
    private bool enabled = true;
    private bool visible = true;
    private bool pushed;
    private bool partialPush;
    private ToolBarButtonImageIndexer imageIndexer;
    private ToolBarButtonStyle style = ToolBarButtonStyle.PushButton;
    private object userData;

    // These variables below are used by the ToolBar control to help
    // it manage some information about us.

    /// <summary>
    ///  If this button has a string, what it's index is in the ToolBar's
    ///  internal list of strings.  Needs to be package protected.
    /// </summary>
    internal IntPtr stringIndex = (IntPtr)(-1);

    /// <summary>
    ///  Our parent ToolBar control.
    /// </summary>
    internal ToolBar parent;

    /// <summary>
    ///  For DropDown buttons, we can optionally show a
    ///  context menu when the button is dropped down.
    /// </summary>
    internal Menu dropDownMenu;

    /// <summary>
    ///  Initializes a new instance of the <see cref='ToolBarButton'/> class.
    /// </summary>
    public ToolBarButton()
    {
        throw new PlatformNotSupportedException();
    }

    public ToolBarButton(string text) : base()
    {
        throw new PlatformNotSupportedException();
    }

    // We need a special way to defer to the ToolBar's image
    // list for indexing purposes.
    [Obsolete("ToolBarButtonImageIndexer has been deprecated.")]
    internal class ToolBarButtonImageIndexer : ImageList.Indexer
    {
        private readonly ToolBarButton owner;

        public ToolBarButtonImageIndexer(ToolBarButton button)
        {
            owner = button;
            throw new PlatformNotSupportedException();
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageList ImageList
        {
            get
            {
                if ((owner is not null) && (owner.parent is not null))
                {
                    return owner.parent.ImageList;
                }

                return null;
            }
            set { Debug.Assert(false, "We should never set the image list"); }
        }
    }

    /// <summary>
    ///
    ///  Indicates the menu to be displayed in
    ///  the drop-down toolbar button.
    /// </summary>
    [DefaultValue(null), TypeConverter(typeof(ReferenceConverter))]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Menu DropDownMenu
    {
        get
        {
            return dropDownMenu;
        }

        set
        {
            if (value is not null && !(value is ContextMenu))
            {
                throw new ArgumentException();
            }

            dropDownMenu = value;
        }
    }

    /// <summary>
    ///  Indicates whether the button is enabled or not.
    /// </summary>
    [DefaultValue(true), Localizable(true)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool Enabled
    {
        get
        {
            return enabled;
        }

        set
        {
            if (enabled != value)
            {
                enabled = value;
            }
        }
    }

    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    internal ToolBarButtonImageIndexer ImageIndexer
    {
        get
        {
            if (imageIndexer is null)
            {
                imageIndexer = new ToolBarButtonImageIndexer(this);
            }

            return imageIndexer;
        }
    }

    /// <summary>
    ///  Indicates the index
    ///  value of the image assigned to the button.
    /// </summary>
    [TypeConverter(typeof(ImageIndexConverter)),
    DefaultValue(-1),
    RefreshProperties(RefreshProperties.Repaint), Localizable(true)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public int ImageIndex
    {
        get
        {
            return ImageIndexer.Index;
        }
        set
        {
            if (ImageIndexer.Index != value)
            {
                if (value < -1)
                {
                    throw new ArgumentOutOfRangeException();
                }

                ImageIndexer.Index = value;
            }
        }
    }

    /// <summary>
    ///  Indicates the index
    ///  value of the image assigned to the button.
    /// </summary>
    [TypeConverter(typeof(ImageKeyConverter)),
    DefaultValue(""), Localizable(true),
    RefreshProperties(RefreshProperties.Repaint)]
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string ImageKey
    {
        get
        {
            return ImageIndexer.Key;
        }
        set
        {
            if (ImageIndexer.Key != value)
            {
                ImageIndexer.Key = value;
            }
        }
    }

    /// <summary>
    ///  Name of this control. The designer will set this to the same
    ///  as the programatic Id "(name)" of the control - however this
    ///  property has no bearing on the runtime aspects of this control.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string Name
    {
        get
        {
            return WindowsFormsUtils.GetComponentName(this, name);
        }
        set
        {
            if (value is null || value.Length == 0)
            {
                name = null;
            }
            else
            {
                name = value;
            }

            if (Site is not null)
            {
                Site.Name = name;
            }
        }
    }

    /// <summary>
    ///  Indicates the toolbar control that the toolbar button is assigned to. This property is
    ///  read-only.
    /// </summary>
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBar Parent
    {
        get
        {
            return parent;
        }
    }

    /// <summary>
    ///
    ///  Indicates whether a toggle-style toolbar button
    ///  is partially pushed.
    /// </summary>
    [DefaultValue(false), Browsable(false),
    EditorBrowsable(EditorBrowsableState.Never)]
    public bool PartialPush
    {
        get
        {
            if (parent is null || !parent.IsHandleCreated)
            {
                return partialPush;
            }
            else
            {
                return partialPush;
            }
        }
        set
        {
            if (partialPush != value)
            {
                partialPush = value;
            }
        }
    }

    /// <summary>
    ///  Indicates whether a toggle-style toolbar button is currently in the pushed state.
    /// </summary>
    [DefaultValue(false), Browsable(false),
    EditorBrowsable(EditorBrowsableState.Never)]
    public bool Pushed
    {
        get
        {
            if (parent is null || !parent.IsHandleCreated)
            {
                return pushed;
            }
            else
            {
                return false;
            }
        }
        set
        {
            if (value != Pushed)
            { // Getting property Pushed updates pushed member variable
                pushed = value;
            }
        }
    }

    /// <summary>
    ///  Indicates the bounding rectangle for a toolbar button. This property is
    ///  read-only.
    /// </summary> Browsable(false),
    [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public Rectangle Rectangle
    {
        get
        {
            if (parent is not null)
            {
                RECT rc = default(RECT);
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }

            return Rectangle.Empty;
        }
    }

    /// <summary>
    ///  Indicates the style of the
    ///  toolbar button.
    /// </summary>
    [DefaultValue(ToolBarButtonStyle.PushButton),
    RefreshProperties(RefreshProperties.Repaint),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public ToolBarButtonStyle Style
    {
        get
        {
            return style;
        }
        set
        {
            if (style == value)
            {
                return;
            }

            style = value;
        }
    }

    [SRCategory(nameof(SR.CatData)),
    Localizable(false),
    Bindable(true),
    SRDescription(nameof(SR.ControlTagDescr)),
    DefaultValue(null),
    TypeConverter(typeof(StringConverter)),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public object Tag
    {
        get
        {
            return userData;
        }
        set
        {
            userData = value;
        }
    }

    /// <summary>
    ///  Indicates the text that is displayed on the toolbar button.
    /// </summary>
    [Localizable(true), DefaultValue(""),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string Text
    {
        get
        {
            return text ?? "";
        }
        set
        {
            if (string.IsNullOrEmpty(value))
            {
                value = null;
            }

            if ((value is null && text is not null) ||
                 (value is not null && (text is null || !text.Equals(value))))
            {
                text = value;
            }
        }
    }

    /// <summary>
    ///
    ///  Indicates
    ///  the text that appears as a tool tip for a control.
    /// </summary>
    [Localizable(true), DefaultValue(""),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public string ToolTipText
    {
        get
        {
            return tooltipText ?? "";
        }
        set
        {
            tooltipText = value;
        }
    }

    /// <summary>
    ///
    ///  Indicates whether the toolbar button
    ///  is visible.
    /// </summary>
    [DefaultValue(true), Localizable(true),
    Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
    public bool Visible
    {
        get
        {
            return visible;
        }
        set
        {
            if (visible != value)
            {
                visible = value;
            }
        }
    }

    public override string ToString()
    {
        throw new PlatformNotSupportedException();
    }
}
