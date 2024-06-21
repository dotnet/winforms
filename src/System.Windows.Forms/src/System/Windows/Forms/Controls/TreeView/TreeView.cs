// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.TreeNode;

namespace System.Windows.Forms;

/// <summary>
///  Displays a hierarchical list of items, or nodes. Each
///  node includes a caption and an optional bitmap. The user can select a node. If
///  it has sub-nodes, the user can collapse or expand the node.
/// </summary>
[DefaultProperty(nameof(Nodes))]
[DefaultEvent(nameof(AfterSelect))]
[Docking(DockingBehavior.Ask)]
[Designer($"System.Windows.Forms.Design.TreeViewDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionTreeView))]
public partial class TreeView : Control
{
    private const int MaxIndent = 32000;      // Maximum allowable TreeView indent
    private const string BackSlash = "\\";
    private const int DefaultTreeViewIndent = 19;

    private DrawTreeNodeEventHandler? _onDrawNode;
    private NodeLabelEditEventHandler? _onBeforeLabelEdit;
    private NodeLabelEditEventHandler? _onAfterLabelEdit;
    private TreeViewCancelEventHandler? _onBeforeCheck;
    private TreeViewEventHandler? _onAfterCheck;
    private TreeViewCancelEventHandler? _onBeforeCollapse;
    private TreeViewEventHandler? _onAfterCollapse;
    private TreeViewCancelEventHandler? _onBeforeExpand;
    private TreeViewEventHandler? _onAfterExpand;
    private TreeViewCancelEventHandler? _onBeforeSelect;
    private TreeViewEventHandler? _onAfterSelect;
    private ItemDragEventHandler? _onItemDrag;
    private TreeNodeMouseHoverEventHandler? _onNodeMouseHover;
    private EventHandler? _onRightToLeftLayoutChanged;

    internal TreeNode? _selectedNode;
    private ImageList.Indexer? _imageIndexer;
    private ImageList.Indexer? _selectedImageIndexer;
    private bool _setOddHeight;
    private TreeNode? _prevHoveredNode;
    private bool _hoveredAlready;
    private bool _rightToLeftLayout;

    private nint _mouseDownNode = 0; // ensures we fire nodeClick on the correct node

    private const int TREEVIEWSTATE_hideSelection = 0x00000001;
    private const int TREEVIEWSTATE_labelEdit = 0x00000002;
    private const int TREEVIEWSTATE_scrollable = 0x00000004;
    private const int TREEVIEWSTATE_checkBoxes = 0x00000008;
    private const int TREEVIEWSTATE_showLines = 0x00000010;
    private const int TREEVIEWSTATE_showPlusMinus = 0x00000020;
    private const int TREEVIEWSTATE_showRootLines = 0x00000040;
    private const int TREEVIEWSTATE_sorted = 0x00000080;
    private const int TREEVIEWSTATE_hotTracking = 0x00000100;
    private const int TREEVIEWSTATE_fullRowSelect = 0x00000200;
    private const int TREEVIEWSTATE_showNodeToolTips = 0x00000400;
    private const int TREEVIEWSTATE_doubleclickFired = 0x00000800;
    private const int TREEVIEWSTATE_mouseUpFired = 0x00001000;
    private const int TREEVIEWSTATE_showTreeViewContextMenu = 0x00002000;
    private const int TREEVIEWSTATE_lastControlValidated = 0x00004000;
    private const int TREEVIEWSTATE_stopResizeWindowMsgs = 0x00008000;
    private const int TREEVIEWSTATE_ignoreSelects = 0x00010000;
    private const int TREEVIEWSTATE_doubleBufferedPropertySet = 0x00020000;

    // PERF: take all the bools and put them into a state variable
    private Collections.Specialized.BitVector32 _treeViewState; // see TREEVIEWSTATE_ consts above

    private static bool s_isScalingInitialized;
    private static Size? s_stateImageSize;
    private static Size? StateImageSize
    {
        get
        {
            if (!s_isScalingInitialized)
            {
                const int LogicalStateImageSize = 16;
                int imageSize = ScaleHelper.ScaleToInitialSystemDpi(LogicalStateImageSize);
                s_stateImageSize = new(imageSize, imageSize);
                s_isScalingInitialized = true;
            }

            return s_stateImageSize;
        }
    }

    internal ImageList.Indexer ImageIndexer
    {
        get
        {
            _imageIndexer ??= new ImageList.Indexer();
            _imageIndexer.ImageList = ImageList;
            return _imageIndexer;
        }
    }

    internal ImageList.Indexer SelectedImageIndexer
    {
        get
        {
            _selectedImageIndexer ??= new ImageList.Indexer();

            _selectedImageIndexer.ImageList = ImageList;

            return _selectedImageIndexer;
        }
    }

    private ImageList? _imageList;
    private int _indent = -1;
    private int _itemHeight = -1;
    private string _pathSeparator = BackSlash;
    private BorderStyle _borderStyle = BorderStyle.Fixed3D;

    internal TreeNodeCollection? _nodes;
    internal TreeNode? _editNode;
    internal TreeNode _root;
    internal Dictionary<IntPtr, TreeNode> _nodesByHandle = [];
    internal bool _nodesCollectionClear; // this is set when the treeNodeCollection is getting cleared and used by TreeView
    internal TreeViewLabelEditNativeWindow? _labelEdit;
    private MouseButtons _downButton;
    private TreeViewDrawMode _drawMode = TreeViewDrawMode.Normal;

    // Properties newly added to TreeView....
    private ImageList? _internalStateImageList;
    private TreeNode? _topNode;
    private ImageList? _stateImageList;
    private Color _lineColor;
    private string? _controlToolTipText;

    // Sorting
    private IComparer? _treeViewNodeSorter;

    // Events
    private TreeNodeMouseClickEventHandler? _onNodeMouseClick;
    private TreeNodeMouseClickEventHandler? _onNodeMouseDoubleClick;

    private ToolTipBuffer _toolTipBuffer;

    /// <summary>
    ///  Creates a TreeView control
    /// </summary>
    public TreeView()
        : base()
    {
        _treeViewState = new Collections.Specialized.BitVector32(
            TREEVIEWSTATE_showRootLines |
            TREEVIEWSTATE_showPlusMinus |
            TREEVIEWSTATE_showLines |
            TREEVIEWSTATE_scrollable |
            TREEVIEWSTATE_hideSelection);

        _root = new TreeNode(this);

        // TreeView must always have an ImageIndex.
        SelectedImageIndexer.Index = 0;
        ImageIndexer.Index = 0;

        SetStyle(ControlStyles.UserPaint, false);
        SetStyle(ControlStyles.StandardClick, false);
        SetStyle(ControlStyles.UseTextForAccessibility, false);
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        foreach (TreeNode rootNode in Nodes)
        {
            foreach (TreeNode node in rootNode.GetSelfAndChildNodes())
            {
                node.ReleaseUiaProvider();
            }
        }

        base.ReleaseUiaProvider(handle);
    }

    /// <summary>
    ///  The background color for this control. Specifying null for
    ///  this parameter sets the
    ///  control's background color to its parent's background color.
    /// </summary>
    public override Color BackColor
    {
        get => ShouldSerializeBackColor() ? base.BackColor : Application.SystemColors.Window;
        set
        {
            base.BackColor = value;
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.TVM_SETBKCOLOR, 0, BackColor.ToWin32());

                // This is to get around a problem in the comctl control where the lines
                // connecting nodes don't get the new BackColor.  This messages forces
                // reconstruction of the line bitmaps without changing anything else.
                PInvoke.SendMessage(this, PInvoke.TVM_SETINDENT, (WPARAM)Indent);
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    /// <summary>
    ///  The border style of the window.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(BorderStyle.Fixed3D)]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.borderStyleDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            if (_borderStyle != value)
            {
                SourceGenerated.EnumValidator.Validate(value);

                _borderStyle = value;
                UpdateStyles();
            }
        }
    }

    /// <summary>
    ///  The value of the CheckBoxes property. The CheckBoxes
    ///  property determines if check boxes are shown next to node in the
    ///  tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TreeViewCheckBoxesDescr))]
    public bool CheckBoxes
    {
        get => _treeViewState[TREEVIEWSTATE_checkBoxes];

        set
        {
            if (CheckBoxes != value)
            {
                _treeViewState[TREEVIEWSTATE_checkBoxes] = value;
                if (IsHandleCreated)
                {
                    if (CheckBoxes)
                    {
                        UpdateStyles();
                    }
                    else
                    {
                        // Going from true to false requires recreation

                        // Reset the Checked state after setting the checkboxes (this was Everett behavior)
                        // The implementation of the TreeNode.Checked property has changed in Whidbey
                        // So we need to explicit set the Checked state to false to keep the everett behavior.
                        UpdateCheckedState(_root, false);
                        RecreateHandle();
                    }
                }
            }
        }
    }

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_TREEVIEW;

            // Keep the scrollbar if we are just updating styles...
            //
            if (IsHandleCreated)
            {
                int currentStyle = unchecked((int)((long)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE)));
                cp.Style |= currentStyle & (int)(WINDOW_STYLE.WS_HSCROLL | WINDOW_STYLE.WS_VSCROLL);
            }

            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                    break;
                case BorderStyle.FixedSingle:
                    cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                    break;
            }

            if (!Scrollable)
            {
                cp.Style |= (int)PInvoke.LVS_NOSCROLL;
            }

            if (!HideSelection)
            {
                cp.Style |= (int)PInvoke.TVS_SHOWSELALWAYS;
            }

            if (LabelEdit)
            {
                cp.Style |= (int)PInvoke.TVS_EDITLABELS;
            }

            if (ShowLines)
            {
                cp.Style |= (int)PInvoke.TVS_HASLINES;
            }

            if (ShowPlusMinus)
            {
                cp.Style |= (int)PInvoke.TVS_HASBUTTONS;
            }

            if (ShowRootLines)
            {
                cp.Style |= (int)PInvoke.TVS_LINESATROOT;
            }

            if (HotTracking)
            {
                cp.Style |= (int)PInvoke.TVS_TRACKSELECT;
            }

            if (FullRowSelect)
            {
                cp.Style |= (int)PInvoke.TVS_FULLROWSELECT;
            }

            if (_setOddHeight)
            {
                cp.Style |= (int)PInvoke.TVS_NONEVENHEIGHT;
            }

            // Don't set TVS_CHECKBOXES here if the window isn't created yet.
            // See OnHandleCreated for explanation
            if (ShowNodeToolTips && IsHandleCreated && !DesignMode)
            {
                cp.Style |= (int)PInvoke.TVS_INFOTIP;
            }

            // Don't set TVS_CHECKBOXES here if the window isn't created yet.
            // See OnHandleCreated for explanation
            if (CheckBoxes && IsHandleCreated)
            {
                cp.Style |= (int)PInvoke.TVS_CHECKBOXES;
            }

            // Don't call IsMirrored from CreateParams. That will lead to some nasty problems, since
            // IsMirrored ends up calling CreateParams - you dig!
            if (RightToLeft == RightToLeft.Yes)
            {
                if (RightToLeftLayout)
                {
                    // We want to turn on mirroring for TreeView explicitly.
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_LAYOUTRTL;
                    // Don't need these styles when mirroring is turned on.
                    cp.ExStyle &= ~(int)(WINDOW_EX_STYLE.WS_EX_RTLREADING | WINDOW_EX_STYLE.WS_EX_RIGHT | WINDOW_EX_STYLE.WS_EX_LEFTSCROLLBAR);
                }
                else
                {
                    cp.Style |= (int)PInvoke.TVS_RTLREADING;
                }
            }

            return cp;
        }
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize => new(121, 97);

    /// <summary>
    ///  This property is overridden and hidden from statement completion
    ///  on controls that are based on Win32 Native Controls.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    protected override bool DoubleBuffered
    {
        get => base.DoubleBuffered;
        set
        {
            if (DoubleBuffered != value)
            {
                base.DoubleBuffered = value;
                _treeViewState[TREEVIEWSTATE_doubleBufferedPropertySet] = true;
                UpdateTreeViewExtendedStyles();
            }
        }
    }

    /// <summary>
    ///  The current foreground color for this control, which is the
    ///  color the control uses to draw its text.
    /// </summary>
    public override Color ForeColor
    {
        get => ShouldSerializeForeColor() ? base.ForeColor : Application.SystemColors.WindowText;
        set
        {
            base.ForeColor = value;
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.TVM_SETTEXTCOLOR, 0, ForeColor.ToWin32());
            }
        }
    }

    /// <summary>
    ///  Determines whether the selection highlight spans across the width of the TreeView.
    ///  This property will have no effect if ShowLines is true.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TreeViewFullRowSelectDescr))]
    public bool FullRowSelect
    {
        get => _treeViewState[TREEVIEWSTATE_fullRowSelect];
        set
        {
            if (FullRowSelect != value)
            {
                _treeViewState[TREEVIEWSTATE_fullRowSelect] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  The HideSelection property specifies whether the selected node will
    ///  be highlighted even when the TreeView loses focus.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TreeViewHideSelectionDescr))]
    public bool HideSelection
    {
        get => _treeViewState[TREEVIEWSTATE_hideSelection];
        set
        {
            if (HideSelection != value)
            {
                _treeViewState[TREEVIEWSTATE_hideSelection] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  The value of the HotTracking property. The HotTracking
    ///  property determines if nodes are highlighted as the mousePointer
    ///  passes over them.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TreeViewHotTrackingDescr))]
    public bool HotTracking
    {
        get => _treeViewState[TREEVIEWSTATE_hotTracking];
        set
        {
            if (HotTracking != value)
            {
                _treeViewState[TREEVIEWSTATE_hotTracking] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  The default image index for nodes in the tree view.
    /// </summary>
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.TreeViewImageIndexDescr))]
    [RelatedImageList("ImageList")]
    public int ImageIndex
    {
        get
        {
            if (_imageList is null)
            {
                return ImageList.Indexer.DefaultIndex;
            }

            if (ImageIndexer.Index >= _imageList.Images.Count)
            {
                return Math.Max(0, _imageList.Images.Count - 1);
            }

            return ImageIndexer.Index;
        }

        set
        {
            // If (none) is selected in the image index editor, we'll just adjust this to
            // mean image index 0. This is because a treeView must always have an image index -
            // even if no imageList exists we want the image index to be 0.
            if (value == ImageList.Indexer.DefaultIndex)
            {
                value = 0;
            }

            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (ImageIndexer.Index != value)
            {
                ImageIndexer.Index = value;
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  The default image index for nodes in the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.TreeViewImageKeyDescr))]
    [RelatedImageList("ImageList")]
    [AllowNull]
    public string ImageKey
    {
        get => ImageIndexer.Key;
        set
        {
            if (ImageIndexer.Key != value)
            {
                ImageIndexer.Key = value;
                if (string.IsNullOrEmpty(value) || value.Equals(SR.toStringNone))
                {
                    ImageIndex = (ImageList is not null) ? 0 : ImageList.Indexer.DefaultIndex;
                }

                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  Returns the image list control that is bound to the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.TreeViewImageListDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public ImageList? ImageList
    {
        get => _imageList;
        set
        {
            if (value != _imageList)
            {
                DetachImageListHandlers();

                _imageList = value;

                AttachImageListHandlers();

                // Update TreeView's images
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.TVM_SETIMAGELIST, 0, value is null ? 0 : value.Handle);
                    if (StateImageList is not null && StateImageList.Images.Count > 0 && _internalStateImageList is not null)
                    {
                        SetStateImageList(_internalStateImageList.Handle);
                    }
                }

                UpdateCheckedState(_root, true);
            }
        }
    }

    private void AttachImageListHandlers()
    {
        if (_imageList is not null)
        {
            // NOTE: any handlers added here should be removed in DetachImageListHandlers
            _imageList.RecreateHandle += new EventHandler(ImageListRecreateHandle);
            _imageList.Disposed += new EventHandler(DetachImageList);
            _imageList.ChangeHandle += new EventHandler(ImageListChangedHandle);
        }
    }

    private void DetachImageListHandlers()
    {
        if (_imageList is not null)
        {
            _imageList.RecreateHandle -= new EventHandler(ImageListRecreateHandle);
            _imageList.Disposed -= new EventHandler(DetachImageList);
            _imageList.ChangeHandle -= new EventHandler(ImageListChangedHandle);
        }
    }

    private void AttachStateImageListHandlers()
    {
        if (_stateImageList is not null)
        {
            // NOTE: any handlers added here should be removed in DetachStateImageListHandlers
            _stateImageList.RecreateHandle += new EventHandler(StateImageListRecreateHandle);
            _stateImageList.Disposed += new EventHandler(DetachStateImageList);
            _stateImageList.ChangeHandle += new EventHandler(StateImageListChangedHandle);
        }
    }

    private void DetachStateImageListHandlers()
    {
        if (_stateImageList is not null)
        {
            _stateImageList.RecreateHandle -= new EventHandler(StateImageListRecreateHandle);
            _stateImageList.Disposed -= new EventHandler(DetachStateImageList);
            _stateImageList.ChangeHandle -= new EventHandler(StateImageListChangedHandle);
        }
    }

    /// <summary>
    ///  Returns the state image list control that is bound to the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.TreeViewStateImageListDescr))]
    public ImageList? StateImageList
    {
        get => _stateImageList;
        set
        {
            if (value != _stateImageList)
            {
                DetachStateImageListHandlers();
                _stateImageList = value;
                AttachStateImageListHandlers();

                // Update TreeView's images.
                if (IsHandleCreated)
                {
                    UpdateNativeStateImageList();

                    // We need to update the checks
                    // and stateImage value for each node.
                    UpdateCheckedState(_root, true);

                    if ((_stateImageList is null || _stateImageList.Images.Count == 0) && CheckBoxes)
                    {
                        // Requires Handle Recreate to force on the checkBoxes and states..
                        RecreateHandle();
                    }
                    else
                    {
                        // The TreeView shows up the state imageList after sending this message even if the nodes don't have any stateImageIndex set.
                        // In order to avoid that we refresh nodes which would "reset" the images to none.
                        // This causes flicker but gives us the right behavior
                        RefreshNodes();
                    }
                }
            }
        }
    }

    /// <summary>
    ///  The indentation level in pixels.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewIndentDescr))]
    public int Indent
    {
        get
        {
            if (_indent != -1)
            {
                return _indent;
            }
            else if (IsHandleCreated)
            {
                return (int)PInvoke.SendMessage(this, PInvoke.TVM_GETINDENT);
            }

            return DefaultTreeViewIndent;
        }
        set
        {
            if (_indent != value)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxIndent);

                _indent = value;
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.TVM_SETINDENT, (WPARAM)value);
                    _indent = (int)PInvoke.SendMessage(this, PInvoke.TVM_GETINDENT);
                }
            }
        }
    }

    /// <summary>
    ///  The height of every item in the tree view, in pixels.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeViewItemHeightDescr))]
    public int ItemHeight
    {
        get
        {
            if (_itemHeight != -1)
            {
                return _itemHeight;
            }

            if (IsHandleCreated)
            {
                return (int)PInvoke.SendMessage(this, PInvoke.TVM_GETITEMHEIGHT);
            }
            else
            {
                if (CheckBoxes && (DrawMode == TreeViewDrawMode.OwnerDrawAll))
                {
                    return Math.Max(16, FontHeight + 3);
                }

                return FontHeight + 3;
            }
        }
        set
        {
            if (_itemHeight != value)
            {
                ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
                ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, short.MaxValue);

                _itemHeight = value;
                if (IsHandleCreated)
                {
                    if (_itemHeight % 2 != 0)
                    {
                        _setOddHeight = true;
                        try
                        {
                            RecreateHandle();
                        }
                        finally
                        {
                            _setOddHeight = false;
                        }
                    }

                    PInvoke.SendMessage(this, PInvoke.TVM_SETITEMHEIGHT, (WPARAM)value);
                    _itemHeight = (int)PInvoke.SendMessage(this, PInvoke.TVM_GETITEMHEIGHT);
                }
            }
        }
    }

    internal ToolTip KeyboardToolTip { get; } = new();

    /// <summary>
    ///  The LabelEdit property determines if the label text
    ///  of nodes in the tree view is editable.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TreeViewLabelEditDescr))]
    public bool LabelEdit
    {
        get => _treeViewState[TREEVIEWSTATE_labelEdit];
        set
        {
            if (LabelEdit != value)
            {
                _treeViewState[TREEVIEWSTATE_labelEdit] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  This is the color of the lines that connect the nodes of the TreeView.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewLineColorDescr))]
    [DefaultValue(typeof(Color), "Black")]
    public Color LineColor
    {
        get
        {
            if (IsHandleCreated)
            {
                int intColor = (int)PInvoke.SendMessage(this, PInvoke.TVM_GETLINECOLOR);
                return ColorTranslator.FromWin32(intColor);
            }

            return _lineColor;
        }
        set
        {
            if (_lineColor != value)
            {
                _lineColor = value;
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.TVM_SETLINECOLOR, 0, _lineColor.ToWin32());
                }
            }
        }
    }

    /// <summary>
    ///  The collection of nodes associated with this TreeView control
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.TreeViewNodesDescr))]
    [MergableProperty(false)]
    public TreeNodeCollection Nodes
    {
        get
        {
            _nodes ??= new TreeNodeCollection(_root);

            return _nodes;
        }
    }

    /// <summary>
    ///  Indicates the drawing mode for the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(TreeViewDrawMode.Normal)]
    [SRDescription(nameof(SR.TreeViewDrawModeDescr))]
    public TreeViewDrawMode DrawMode
    {
        get => _drawMode;
        set
        {
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            if (_drawMode != value)
            {
                _drawMode = value;
                Invalidate();
                // We need to invalidate when the Control resizes when the we support custom draw.
                if (DrawMode == TreeViewDrawMode.OwnerDrawAll)
                {
                    SetStyle(ControlStyles.ResizeRedraw, true);
                }
            }
        }
    }

    /// <summary>
    ///  The delimiter string used by TreeNode.getFullPath().
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue("\\")]
    [SRDescription(nameof(SR.TreeViewPathSeparatorDescr))]
    public string PathSeparator
    {
        get => _pathSeparator;
        set => _pathSeparator = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  This is used for international applications where the language is written from RightToLeft.
    ///  When this property is true, and the RightToLeft is true, mirroring will be turned on on
    ///  the form, and control placement and text will be from right to left.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))]
    public virtual bool RightToLeftLayout
    {
        get => _rightToLeftLayout;
        set
        {
            if (value != _rightToLeftLayout)
            {
                _rightToLeftLayout = value;
                using (new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout))
                {
                    OnRightToLeftLayoutChanged(EventArgs.Empty);
                }
            }
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TreeViewScrollableDescr))]
    public bool Scrollable
    {
        get => _treeViewState[TREEVIEWSTATE_scrollable];
        set
        {
            if (Scrollable != value)
            {
                _treeViewState[TREEVIEWSTATE_scrollable] = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  The image index that a node will display when selected.
    ///  The index applies to the ImageList referred to by the imageList property,
    /// </summary>
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [SRCategory(nameof(SR.CatBehavior))]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    [Localizable(true)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [SRDescription(nameof(SR.TreeViewSelectedImageIndexDescr))]
    [RelatedImageList("ImageList")]
    public int SelectedImageIndex
    {
        get
        {
            if (_imageList is null)
            {
                return ImageList.Indexer.DefaultIndex;
            }

            if (SelectedImageIndexer.Index >= _imageList.Images.Count)
            {
                return Math.Max(0, _imageList.Images.Count - 1);
            }

            return SelectedImageIndexer.Index;
        }
        set
        {
            // If (none) is selected in the image index editor, we'll just adjust this to
            // mean image index 0. This is because a treeView must always have an image index -
            // even if no imageList exists we want the image index to be 0.
            //
            if (value == ImageList.Indexer.DefaultIndex)
            {
                value = 0;
            }

            ArgumentOutOfRangeException.ThrowIfNegative(value);

            if (SelectedImageIndexer.Index != value)
            {
                SelectedImageIndexer.Index = value;
                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  The default image index for nodes in the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [TypeConverter(typeof(ImageKeyConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [SRDescription(nameof(SR.TreeViewSelectedImageKeyDescr))]
    [RelatedImageList("ImageList")]
    [AllowNull]
    public string SelectedImageKey
    {
        get => SelectedImageIndexer.Key;
        set
        {
            if (SelectedImageIndexer.Key != value)
            {
                SelectedImageIndexer.Key = value;

                if (string.IsNullOrEmpty(value) || value.Equals(SR.toStringNone))
                {
                    SelectedImageIndex = (ImageList is not null) ? 0 : ImageList.Indexer.DefaultIndex;
                }

                if (IsHandleCreated)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  The currently selected tree node, or null if nothing is selected.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TreeViewSelectedNodeDescr))]
    public TreeNode? SelectedNode
    {
        get
        {
            if (IsHandleCreated)
            {
                IntPtr hItem = PInvoke.SendMessage(this, PInvoke.TVM_GETNEXTITEM, (WPARAM)PInvoke.TVGN_CARET);
                if (hItem == IntPtr.Zero)
                {
                    return null;
                }

                return NodeFromHandle(hItem);
            }
            else if (_selectedNode is not null && _selectedNode.TreeView == this)
            {
                return _selectedNode;
            }
            else
            {
                return null;
            }
        }
        set
        {
            if (IsHandleCreated && (value is null || value.TreeView == this))
            {
                // This class invariant is not quite correct -- if the selected node does not belong to this TreeView,
                // selectedNode is not null even though the handle is created.  We will call set_SelectedNode
                // to inform the handle that the selected node has been added to the TreeView.
                Debug.Assert(_selectedNode is null || _selectedNode.TreeView != this, "handle is created, but we're still caching selectedNode");

                nint hnode = (value is null ? 0 : value.Handle);
                PInvoke.SendMessage(this, PInvoke.TVM_SELECTITEM, (WPARAM)PInvoke.TVGN_CARET, (LPARAM)hnode);
                _selectedNode = null;
            }
            else
            {
                _selectedNode = value;
            }
        }
    }

    /// <summary>
    ///  The ShowLines property determines if lines are drawn between
    ///  nodes in the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TreeViewShowLinesDescr))]
    public bool ShowLines
    {
        get => _treeViewState[TREEVIEWSTATE_showLines];
        set
        {
            if (ShowLines != value)
            {
                _treeViewState[TREEVIEWSTATE_showLines] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  The ShowLines property determines whether or not the tooltips will be displayed on the nodes
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TreeViewShowShowNodeToolTipsDescr))]
    public bool ShowNodeToolTips
    {
        get => _treeViewState[TREEVIEWSTATE_showNodeToolTips];
        set
        {
            if (ShowNodeToolTips != value)
            {
                _treeViewState[TREEVIEWSTATE_showNodeToolTips] = value;
                if (ShowNodeToolTips)
                {
                    RecreateHandle();
                }
            }
        }
    }

    /// <summary>
    ///  The ShowPlusMinus property determines if the "plus/minus"
    ///  expand button is shown next to tree nodes that have children.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TreeViewShowPlusMinusDescr))]
    public bool ShowPlusMinus
    {
        get => _treeViewState[TREEVIEWSTATE_showPlusMinus];
        set
        {
            if (ShowPlusMinus != value)
            {
                _treeViewState[TREEVIEWSTATE_showPlusMinus] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  Determines if lines are draw between nodes at the root of
    ///  the tree view.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [SRDescription(nameof(SR.TreeViewShowRootLinesDescr))]
    public bool ShowRootLines
    {
        get => _treeViewState[TREEVIEWSTATE_showRootLines];
        set
        {
            if (ShowRootLines != value)
            {
                _treeViewState[TREEVIEWSTATE_showRootLines] = value;
                if (IsHandleCreated)
                {
                    UpdateStyles();
                }
            }
        }
    }

    /// <summary>
    ///  The Sorted property determines if nodes in the tree view are sorted.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.TreeViewSortedDescr))]
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool Sorted
    {
        get => _treeViewState[TREEVIEWSTATE_sorted];
        set
        {
            if (Sorted != value)
            {
                _treeViewState[TREEVIEWSTATE_sorted] = value;
                if (Sorted && TreeViewNodeSorter is null && Nodes.Count >= 1)
                {
                    RefreshNodes();
                }
            }
        }
    }

    /// <summary>
    ///  The sorting comparer for this TreeView.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TreeViewNodeSorterDescr))]
    public IComparer? TreeViewNodeSorter
    {
        get => _treeViewNodeSorter;
        set
        {
            if (_treeViewNodeSorter != value)
            {
                _treeViewNodeSorter = value;
                if (value is not null)
                {
                    Sort();
                }
                else
                {
                    Sorted = false;
                    RefreshNodes();
                }
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    /// <summary>
    ///  The first visible node in the TreeView. Initially
    ///  the first root node is at the top of the TreeView, but if the
    ///  contents have been scrolled another node may be at the top.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TreeViewTopNodeDescr))]
    public TreeNode? TopNode
    {
        get
        {
            if (IsHandleCreated)
            {
                IntPtr hitem = PInvoke.SendMessage(this, PInvoke.TVM_GETNEXTITEM, (WPARAM)PInvoke.TVGN_FIRSTVISIBLE);
                return (hitem == IntPtr.Zero ? null : NodeFromHandle(hitem));
            }

            return _topNode;
        }
        set
        {
            if (IsHandleCreated && (value is null || value.TreeView == this))
            {
                // This class invariant is not quite correct -- if the selected node does not belong to this TreeView,
                // selectedNode is not null even though the handle is created.  We will call set_SelectedNode
                // to inform the handle that the selected node has been added to the TreeView.
                Debug.Assert(_topNode is null || _topNode.TreeView != this, "handle is created, but we're still caching selectedNode");

                nint hnode = (value is null ? 0 : value.Handle);
                PInvoke.SendMessage(this, PInvoke.TVM_SELECTITEM, (WPARAM)PInvoke.TVGN_FIRSTVISIBLE, (LPARAM)hnode);
                _topNode = null;
            }
            else
            {
                _topNode = value;
            }
        }
    }

    /// <summary>
    ///  The count of fully visible nodes in the tree view.  This number
    ///  may be greater than the number of nodes in the control.
    ///  The control calculates this value by dividing the height of the
    ///  client window by the height of an item
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.TreeViewVisibleCountDescr))]
    public int VisibleCount => IsHandleCreated ? (int)PInvoke.SendMessage(this, PInvoke.TVM_GETVISIBLECOUNT) : 0;

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewBeforeEditDescr))]
    public event NodeLabelEditEventHandler? BeforeLabelEdit
    {
        add => _onBeforeLabelEdit += value;
        remove => _onBeforeLabelEdit -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewAfterEditDescr))]
    public event NodeLabelEditEventHandler? AfterLabelEdit
    {
        add => _onAfterLabelEdit += value;
        remove => _onAfterLabelEdit -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewBeforeCheckDescr))]
    public event TreeViewCancelEventHandler? BeforeCheck
    {
        add => _onBeforeCheck += value;
        remove => _onBeforeCheck -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewAfterCheckDescr))]
    public event TreeViewEventHandler? AfterCheck
    {
        add => _onAfterCheck += value;
        remove => _onAfterCheck -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewBeforeCollapseDescr))]
    public event TreeViewCancelEventHandler? BeforeCollapse
    {
        add => _onBeforeCollapse += value;
        remove => _onBeforeCollapse -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewAfterCollapseDescr))]
    public event TreeViewEventHandler? AfterCollapse
    {
        add => _onAfterCollapse += value;
        remove => _onAfterCollapse -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewBeforeExpandDescr))]
    public event TreeViewCancelEventHandler? BeforeExpand
    {
        add => _onBeforeExpand += value;
        remove => _onBeforeExpand -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewAfterExpandDescr))]
    public event TreeViewEventHandler? AfterExpand
    {
        add => _onAfterExpand += value;
        remove => _onAfterExpand -= value;
    }

    /// <summary>
    ///  Fires when a TreeView node needs to be drawn.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewDrawNodeEventDescr))]
    public event DrawTreeNodeEventHandler? DrawNode
    {
        add => _onDrawNode += value;
        remove => _onDrawNode -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.ListViewItemDragDescr))]
    public event ItemDragEventHandler? ItemDrag
    {
        add => _onItemDrag += value;
        remove => _onItemDrag -= value;
    }

    [SRCategory(nameof(SR.CatAction))]
    [SRDescription(nameof(SR.TreeViewNodeMouseHoverDescr))]
    public event TreeNodeMouseHoverEventHandler? NodeMouseHover
    {
        add => _onNodeMouseHover += value;
        remove => _onNodeMouseHover -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewBeforeSelectDescr))]
    public event TreeViewCancelEventHandler? BeforeSelect
    {
        add => _onBeforeSelect += value;
        remove => _onBeforeSelect -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewAfterSelectDescr))]
    public event TreeViewEventHandler? AfterSelect
    {
        add => _onAfterSelect += value;
        remove => _onAfterSelect -= value;
    }

    /// <summary>
    ///  TreeView OnPaint.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewNodeMouseClickDescr))]
    public event TreeNodeMouseClickEventHandler? NodeMouseClick
    {
        add => _onNodeMouseClick += value;
        remove => _onNodeMouseClick -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeViewNodeMouseDoubleClickDescr))]
    public event TreeNodeMouseClickEventHandler? NodeMouseDoubleClick
    {
        add => _onNodeMouseDoubleClick += value;
        remove => _onNodeMouseDoubleClick -= value;
    }

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
    public event EventHandler? RightToLeftLayoutChanged
    {
        add => _onRightToLeftLayoutChanged += value;
        remove => _onRightToLeftLayoutChanged -= value;
    }

    /// <summary>
    ///  Disables redrawing of the tree view. A call to beginUpdate() must be
    ///  balanced by a following call to endUpdate(). Following a call to
    ///  beginUpdate(), any redrawing caused by operations performed on the
    ///  tree view is deferred until the call to endUpdate().
    /// </summary>
    public void BeginUpdate()
    {
        BeginUpdateInternal();
    }

    /// <summary>
    ///  Collapses all nodes at the root level.
    /// </summary>
    public void CollapseAll()
    {
        _root.Collapse();
    }

    /// <summary>
    ///  Creates the new instance of AccessibleObject for this TreeView control.
    /// </summary>
    /// <returns>
    ///  The AccessibleObject for this TreeView instance.
    /// </returns>
    protected override AccessibleObject CreateAccessibilityInstance()
        => new TreeViewAccessibleObject(this);

    protected override unsafe void CreateHandle()
    {
        if (!RecreatingHandle)
        {
            using ThemingScope scope = new(Application.UseVisualStyles);
            PInvoke.InitCommonControlsEx(new INITCOMMONCONTROLSEX
            {
                dwSize = (uint)sizeof(INITCOMMONCONTROLSEX),
                dwICC = INITCOMMONCONTROLSEX_ICC.ICC_TREEVIEW_CLASSES
            });
        }

        base.CreateHandle();
    }

    /// <summary>
    ///  Resets the imageList to null.  We wire this method up to the imageList's
    ///  Dispose event, so that we don't hang onto an imageList that's gone away.
    /// </summary>
    private void DetachImageList(object? sender, EventArgs e)
    {
        ImageList = null;
    }

    /// <summary>
    ///  Resets the stateImageList to null.  We wire this method up to the stateImageList's
    ///  Dispose event, so that we don't hang onto an stateImageList that's gone away.
    /// </summary>
    private void DetachStateImageList(object? sender, EventArgs e)
    {
        _internalStateImageList = null;
        StateImageList = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            lock (this)
            {
                DetachImageListHandlers();
                _imageList = null;
                DetachStateImageListHandlers();
                _stateImageList = null;
            }
        }

        // Dispose unmanaged resources.
        UnhookNodes();
        KeyboardToolTip.Dispose();

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Reenables redrawing of the tree view. A call to beginUpdate() must be
    ///  balanced by a following call to endUpdate(). Following a call to
    ///  beginUpdate(), any redrawing caused by operations performed on the
    ///  combo box is deferred until the call to endUpdate().
    /// </summary>
    public void EndUpdate()
    {
        EndUpdateInternal();
    }

    /// <summary>
    ///  Expands all nodes at the root level.
    /// </summary>
    public void ExpandAll()
    {
        _root.ExpandAll();
    }

    /// <summary>
    ///  Forces the TreeView to recalculate all its nodes widths so that it updates the
    ///  scrollbars as appropriate.
    /// </summary>
    internal void ForceScrollbarUpdate(bool delayed)
    {
        // ForceScrollbarUpdate call WM_SETREDRAW( FALSE ) followed by WM_SETREDRAW( TRUE )
        // So if TreeView.BeginUpdate is called
        // ForceScrollbarUpdate effectively causes tree view to ignore BeginUpdate and cause control to update on every change.
        // So guard against this scenario by using the new internal method on Control.
        if (!IsUpdating())
        {
            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)false);
                if (delayed)
                {
                    PInvoke.PostMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)true);
                }
                else
                {
                    PInvoke.SendMessage(this, PInvoke.WM_SETREDRAW, (WPARAM)(BOOL)true);
                }
            }
        }
    }

    /// <summary>
    ///  Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
    /// </summary>
    internal override void SetToolTip(ToolTip toolTip)
    {
        if (toolTip is null || !ShowNodeToolTips)
        {
            return;
        }

        PInvoke.SendMessage(toolTip, PInvoke.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
        PInvoke.SendMessage(this, PInvoke.TVM_SETTOOLTIPS, (WPARAM)toolTip.Handle);
        _controlToolTipText = toolTip.GetToolTip(this);
    }

    /// <summary>
    ///  Gives the information about which part of the treeNode is at the given point.
    /// </summary>
    public TreeViewHitTestInfo HitTest(Point pt) => HitTest(pt.X, pt.Y);

    /// <summary>
    ///  Gives the information about which part of the treeNode is at the given x, y.
    /// </summary>
    public TreeViewHitTestInfo HitTest(int x, int y)
    {
        TVHITTESTINFO tvhi = new()
        {
            pt = new Point(x, y)
        };

        nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhi);
        TreeNode? node = hnode == 0 ? null : NodeFromHandle(hnode);
        TreeViewHitTestLocations loc = (TreeViewHitTestLocations)tvhi.flags;
        return new TreeViewHitTestInfo(node, loc);
    }

    /// <summary>
    ///  Defined so that a  tree node can use it
    /// </summary>
    internal bool TreeViewBeforeCheck(TreeNode? node, TreeViewAction actionTaken)
    {
        TreeViewCancelEventArgs viewCancelEventArgs = new(node, false, actionTaken);
        OnBeforeCheck(viewCancelEventArgs);
        return viewCancelEventArgs.Cancel;
    }

    internal void TreeViewAfterCheck(TreeNode node, TreeViewAction actionTaken)
    {
        OnAfterCheck(new TreeViewEventArgs(node, actionTaken));
    }

    /// <summary>
    ///  Returns count of nodes at root, optionally including all subtrees.
    /// </summary>
    public int GetNodeCount(bool includeSubTrees) => _root.GetNodeCount(includeSubTrees);

    /// <summary>
    ///  Returns the TreeNode at the given location in tree view coordinates.
    /// </summary>
    public TreeNode? GetNodeAt(Point pt) => GetNodeAt(pt.X, pt.Y);

    /// <summary>
    ///  Returns the TreeNode at the given location in tree view coordinates.
    /// </summary>
    public TreeNode? GetNodeAt(int x, int y)
    {
        TVHITTESTINFO tvhi = new()
        {
            pt = new Point(x, y)
        };

        nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhi);
        return (hnode == 0 ? null : NodeFromHandle(hnode));
    }

    private void ImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (IsHandleCreated)
        {
            IntPtr handle = (ImageList is null) ? IntPtr.Zero : ImageList.Handle;
            PInvoke.SendMessage(this, PInvoke.TVM_SETIMAGELIST, 0, handle);
        }
    }

    private static void UpdateImagesRecursive(TreeNode node)
    {
        node.UpdateImage();

        // Iterate only through the Nodes collection rather than the
        // array since an item might have been removed from the collection, and
        // correspondingly "removed" from the array, but still exist in the array
        // since the array isn't actually re-dimensioned down to a smaller size.
        foreach (TreeNode child in node.Nodes)
        {
            UpdateImagesRecursive(child);
        }
    }

    private void ImageListChangedHandle(object? sender, EventArgs e)
    {
        if ((sender is not null) && (sender == _imageList) && IsHandleCreated)
        {
            BeginUpdate();
            foreach (TreeNode node in Nodes)
            {
                UpdateImagesRecursive(node);
            }

            EndUpdate();
        }
    }

    private static void NotifyAboutGotFocus(TreeNode? treeNode)
    {
        if (treeNode is not null)
        {
            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(treeNode);
        }
    }

    private static void NotifyAboutLostFocus(TreeNode? treeNode)
    {
        if (treeNode is not null)
        {
            KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(treeNode);
        }
    }

    private void StateImageListRecreateHandle(object? sender, EventArgs e)
    {
        if (IsHandleCreated)
        {
            IntPtr handle = IntPtr.Zero;
            if (_internalStateImageList is not null)
            {
                handle = _internalStateImageList.Handle;
            }

            SetStateImageList(handle);
        }
    }

    private void StateImageListChangedHandle(object? sender, EventArgs e)
    {
        if (sender is null || sender != _stateImageList || !IsHandleCreated)
        {
            return;
        }

        // Since the native treeView requires the state imageList to be 1-indexed we need to
        // re add the images if the original collection had changed.
        if (_stateImageList is null || _stateImageList.Images.Count <= 0)
        {
            UpdateCheckedState(_root, true);
            return;
        }

        Image[] images = new Image[_stateImageList.Images.Count + 1];
        images[0] = _stateImageList.Images[0];
        for (int i = 1; i <= _stateImageList.Images.Count; i++)
        {
            images[i] = _stateImageList.Images[i - 1];
        }

        if (_internalStateImageList is not null)
        {
            _internalStateImageList.Images.Clear();
            _internalStateImageList.Images.AddRange(images);
        }
        else
        {
            _internalStateImageList = new ImageList();
            _internalStateImageList.Images.AddRange(images);
        }

        Debug.Assert(_internalStateImageList is not null, "Why are changing images when the Imagelist is null?");
        if (_internalStateImageList is not null)
        {
            if (StateImageSize is not null)
            {
                _internalStateImageList.ImageSize = (Size)StateImageSize;
            }

            SetStateImageList(_internalStateImageList.Handle);
        }
    }

    /// <summary>
    ///  Overridden to handle RETURN key.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        // If in edit mode, treat Return as an input key, so the form doesn't grab it
        // and treat it as clicking the Form.AcceptButton.  Similarly for Escape
        // and Form.CancelButton.
        if (_editNode is not null && (keyData & Keys.Alt) == 0)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Return:
                case Keys.Escape:
                case Keys.PageUp:
                case Keys.PageDown:
                case Keys.Home:
                case Keys.End:
                    return true;
            }
        }

        return base.IsInputKey(keyData);
    }

    /// <summary>
    ///  Note this can be null - particularly if any windows messages get generated during
    ///  the insertion of a tree node (TVM_INSERTITEM)
    /// </summary>
    internal TreeNode? NodeFromHandle(IntPtr handle)
    {
        _nodesByHandle.TryGetValue(handle, out TreeNode? treeNode);
        return treeNode;
    }

    /// <summary>
    ///  Raises the DrawNode event.
    /// </summary>
    protected virtual void OnDrawNode(DrawTreeNodeEventArgs e) =>
        _onDrawNode?.Invoke(this, e);

    protected override void OnHandleCreated(EventArgs e)
    {
        if (!IsHandleCreated)
        {
            base.OnHandleCreated(e);
            return;
        }

        TreeNode? savedSelectedNode = _selectedNode;
        _selectedNode = null;

        base.OnHandleCreated(e);

        // The TreeView extended styles are independent of the window extended styles.
        UpdateTreeViewExtendedStyles();

        int version = (int)PInvoke.SendMessage(this, PInvoke.CCM_GETVERSION);
        if (version < 5)
        {
            PInvoke.SendMessage(this, PInvoke.CCM_SETVERSION, 5);
        }

        // Workaround for problem in TreeView where it doesn't recognize the TVS_CHECKBOXES
        // style if it is set before the window is created. To get around the problem,
        // we set it here after the window is created, and we make sure we don't set it
        // in getCreateParams so that this will actually change the value of the bit.
        // This seems to make the TreeView happy.
        if (CheckBoxes)
        {
            int style = (int)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            style |= (int)PInvoke.TVS_CHECKBOXES;
            PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
        }

        if (ShowNodeToolTips && !DesignMode)
        {
            int style = (int)PInvoke.GetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE);
            style |= (int)PInvoke.TVS_INFOTIP;
            PInvoke.SetWindowLong(this, WINDOW_LONG_PTR_INDEX.GWL_STYLE, style);
        }

        Color c = BackColor;
        if (c != Application.SystemColors.Window || IsDarkModeEnabled)
        {
            PInvoke.SendMessage(this, PInvoke.TVM_SETBKCOLOR, 0, c.ToWin32());
        }

        c = ForeColor;

        if (c != Application.SystemColors.WindowText || IsDarkModeEnabled)
        {
            PInvoke.SendMessage(this, PInvoke.TVM_SETTEXTCOLOR, 0, c.ToWin32());
        }

        // Put the LineColor into the native control only if set.
        if (_lineColor != Color.Empty)
        {
            PInvoke.SendMessage(this, PInvoke.TVM_SETLINECOLOR, 0, _lineColor.ToWin32());
        }

        if (_imageList is not null)
        {
            PInvoke.SendMessage(this, PInvoke.TVM_SETIMAGELIST, 0, _imageList.Handle);
        }

        if (_stateImageList is not null)
        {
            UpdateNativeStateImageList();
        }

        if (_indent != -1)
        {
            PInvoke.SendMessage(this, PInvoke.TVM_SETINDENT, (WPARAM)_indent);
        }

        if (_itemHeight != -1)
        {
            PInvoke.SendMessage(this, PInvoke.TVM_SETITEMHEIGHT, (WPARAM)ItemHeight);
        }

        // Essentially we are setting the width to be infinite so that the
        // TreeView never thinks it needs a scrollbar when the first node is created
        // during the first handle creation.
        //
        // This is set back to the oldSize after the Realize method.
        try
        {
            _treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs] = true;
            int oldSize = Width;
            SET_WINDOW_POS_FLAGS flags = SET_WINDOW_POS_FLAGS.SWP_NOZORDER
                | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE
                | SET_WINDOW_POS_FLAGS.SWP_NOMOVE;

            PInvoke.SetWindowPos(
                this,
                HWND.HWND_TOP,
                Left,
                Top,
                int.MaxValue,
                Height,
                flags);

            _root.Realize(insertFirst: false);

            if (oldSize != 0)
            {
                PInvoke.SetWindowPos(
                    this,
                    HWND.HWND_TOP,
                    Left,
                    Top,
                    oldSize,
                    Height,
                    flags);
            }
        }
        finally
        {
            _treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs] = false;
        }

        SelectedNode = savedSelectedNode;
    }

    // Replace the native control's ImageList with our current stateImageList
    // set the value of internalStateImageList to the new list
    private void UpdateNativeStateImageList()
    {
        if (_stateImageList is null || _stateImageList.Images.Count <= 0)
        {
            return;
        }

        ImageList newImageList = new();
        if (StateImageSize is not null)
        {
            newImageList.ImageSize = (Size)StateImageSize;
        }

        Image[] images = new Image[_stateImageList.Images.Count + 1];
        images[0] = _stateImageList.Images[0];
        for (int i = 1; i <= _stateImageList.Images.Count; i++)
        {
            images[i] = _stateImageList.Images[i - 1];
        }

        newImageList.Images.AddRange(images);
        PInvoke.SendMessage(this, PInvoke.TVM_SETIMAGELIST, (WPARAM)PInvoke.TVSIL_STATE, (LPARAM)newImageList.Handle);

        _internalStateImageList?.Dispose();
        _internalStateImageList = newImageList;
    }

    private void SetStateImageList(IntPtr handle)
    {
        // In certain cases (TREEVIEWSTATE_checkBoxes) e.g., the Native TreeView leaks the imageList
        // even if set by us. To prevent any leaks, we always destroy what was there after setting a new list.
        IntPtr handleOld = PInvoke.SendMessage(this, PInvoke.TVM_SETIMAGELIST, (WPARAM)PInvoke.TVSIL_STATE, (LPARAM)handle);
        if ((handleOld != IntPtr.Zero) && (handleOld != handle))
        {
            PInvoke.ImageList.Destroy(new HandleRef<HIMAGELIST>(this, (HIMAGELIST)handleOld));
        }
    }

    // Destroying the tree-view control does not destroy the native state image list.
    // We must destroy it explicitly.
    private void DestroyNativeStateImageList(bool reset)
    {
        IntPtr handle = PInvoke.SendMessage(this, PInvoke.TVM_GETIMAGELIST, (WPARAM)PInvoke.TVSIL_STATE);
        if (handle != IntPtr.Zero)
        {
            PInvoke.ImageList.Destroy(new HandleRef<HIMAGELIST>(this, (HIMAGELIST)handle));
            if (reset)
            {
                PInvoke.SendMessage(this, PInvoke.TVM_SETIMAGELIST, (WPARAM)PInvoke.TVSIL_STATE);
            }
        }
    }

    protected override void OnHandleDestroyed(EventArgs e)
    {
        _selectedNode = SelectedNode;

        // Unfortunately, to avoid the native tree view leaking it's State Image List, we need to
        // destroy it ourselves here.
        DestroyNativeStateImageList(true);

        // for the case when we are NOT being disposed, we'll be recreating the internal state imageList
        // in OnHandleCreate, so it is ok to completely Dispose here
        if (_internalStateImageList is not null)
        {
            _internalStateImageList.Dispose();
            _internalStateImageList = null;
        }

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    ///  We keep track of if we've hovered already so we don't fire multiple hover events
    /// </summary>
    protected override void OnMouseLeave(EventArgs e)
    {
        _hoveredAlready = false;
        base.OnMouseLeave(e);
    }

    /// <summary>
    ///  In order for the MouseHover event to fire for each item in a TreeView,
    ///  the node the mouse is hovering over is found. Each time a new node is hovered
    ///  over a new event is raised.
    /// </summary>
    protected override void OnMouseHover(EventArgs e)
    {
        // Hover events need to be caught for each node within the TreeView so
        // the appropriate NodeHovered event can be raised.
        TVHITTESTINFO tvhip = new()
        {
            pt = PointToClient(Cursor.Position)
        };

        nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhip);
        if (hnode != 0 && ((tvhip.flags & TVHITTESTINFO_FLAGS.TVHT_ONITEM) != 0))
        {
            TreeNode? tn = NodeFromHandle(hnode);
            if (tn != _prevHoveredNode && tn is not null)
            {
                OnNodeMouseHover(new TreeNodeMouseHoverEventArgs(tn));
                _prevHoveredNode = tn;
                NotifyAboutLostFocus(SelectedNode);
            }
        }

        if (!_hoveredAlready)
        {
            base.OnMouseHover(e);
            _hoveredAlready = true;
        }

        ResetMouseEventArgs();
    }

    /// <summary>
    ///  Raises the beforeLabelEdit event.
    /// </summary>
    protected virtual void OnBeforeLabelEdit(NodeLabelEditEventArgs e) =>
        _onBeforeLabelEdit?.Invoke(this, e);

    /// <summary>
    ///  Raises the afterLabelEdit event.
    /// </summary>
    protected virtual void OnAfterLabelEdit(NodeLabelEditEventArgs e)
    {
        _onAfterLabelEdit?.Invoke(this, e);

        // Raise an event to highlight & announce the edited node
        // if editing hasn't been canceled.
        if (IsAccessibilityObjectCreated && !e.CancelEdit && e.Node is not null)
        {
            e.Node.AccessibilityObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    /// <summary>
    ///  Raises the beforeCheck event.
    /// </summary>
    protected virtual void OnBeforeCheck(TreeViewCancelEventArgs e) =>
        _onBeforeCheck?.Invoke(this, e);

    /// <summary>
    ///  Raises the afterCheck event.
    /// </summary>
    protected virtual void OnAfterCheck(TreeViewEventArgs e)
    {
        _onAfterCheck?.Invoke(this, e);

        // Raise an event to announce a toggle state change.
        if (IsAccessibilityObjectCreated && e.Node is not null)
        {
            TreeNodeAccessibleObject? nodeAccessibleObject = e.Node.AccessibilityObject;
            if (nodeAccessibleObject is null)
            {
                return;
            }

            ToggleState newState = nodeAccessibleObject.ToggleState;
            ToggleState oldState = newState == ToggleState.ToggleState_On
                ? ToggleState.ToggleState_Off
                : ToggleState.ToggleState_On;

            nodeAccessibleObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ToggleToggleStatePropertyId,
                oldValue: (VARIANT)(int)oldState,
                newValue: (VARIANT)(int)newState);
        }
    }

    /// <summary>
    ///  Raises the beforeCollapse event.
    /// </summary>
    protected internal virtual void OnBeforeCollapse(TreeViewCancelEventArgs e) =>
        _onBeforeCollapse?.Invoke(this, e);

    /// <summary>
    ///  Raises the afterCollapse event.
    /// </summary>
    protected internal virtual void OnAfterCollapse(TreeViewEventArgs e)
    {
        _onAfterCollapse?.Invoke(this, e);

        // Raise an event to announce the expand-collapse state change.
        if (IsAccessibilityObjectCreated && e.Node is not null)
        {
            e.Node.AccessibilityObject?.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                oldValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded,
                newValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed);
        }
    }

    /// <summary>
    ///  Raises the beforeExpand event.
    /// </summary>
    protected virtual void OnBeforeExpand(TreeViewCancelEventArgs e) =>
        _onBeforeExpand?.Invoke(this, e);

    /// <summary>
    ///  Raises the afterExpand event.
    /// </summary>
    protected virtual void OnAfterExpand(TreeViewEventArgs e)
    {
        _onAfterExpand?.Invoke(this, e);

        // Raise an event to announce the expand-collapse state change.
        if (IsAccessibilityObjectCreated && e.Node is not null)
        {
            e.Node.AccessibilityObject?.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                oldValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed,
                newValue: (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded);
        }
    }

    /// <summary>
    ///  Raises the ItemDrag event.
    /// </summary>
    protected virtual void OnItemDrag(ItemDragEventArgs e) =>
        _onItemDrag?.Invoke(this, e);

    /// <summary>
    ///  Raises the NodeMouseHover event.
    /// </summary>
    protected virtual void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e) =>
        _onNodeMouseHover?.Invoke(this, e);

    /// <summary>
    ///  Raises the beforeSelect event.
    /// </summary>
    protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e) =>
        _onBeforeSelect?.Invoke(this, e);

    /// <summary>
    ///  Raises the afterSelect event.
    /// </summary>
    protected virtual void OnAfterSelect(TreeViewEventArgs e)
    {
        _onAfterSelect?.Invoke(this, e);

        // Raise an event to highlight & announce the selected node.
        if (IsAccessibilityObjectCreated && e.Node is not null)
        {
            TreeNodeAccessibleObject? nodeAccessibleObject = e.Node.AccessibilityObject;
            if (nodeAccessibleObject is null)
            {
                return;
            }

            nodeAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            nodeAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_SelectionItem_ElementSelectedEventId);

            // Raise to say "Selected" after announcing the node.
            nodeAccessibleObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_SelectionItemIsSelectedPropertyId,
                oldValue: (VARIANT)!nodeAccessibleObject.IsItemSelected,
                newValue: (VARIANT)nodeAccessibleObject.IsItemSelected);
        }
    }

    /// <summary>
    ///  Raises the onNodeMouseClick event.
    /// </summary>
    protected virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e) =>
        _onNodeMouseClick?.Invoke(this, e);

    /// <summary>
    ///  Raises the onNodeMouseDoubleClick event.
    /// </summary>
    protected virtual void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e) =>
        _onNodeMouseDoubleClick?.Invoke(this, e);

    /// <summary>
    ///  Handles the OnBeforeCheck / OnAfterCheck for keyboard clicks
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.Handled)
        {
            return;
        }

        // if it's a space, send the check notifications and toggle the checkbox if we're not
        // cancelled.
        if (CheckBoxes && (e.KeyData & Keys.KeyCode) == Keys.Space)
        {
            TreeNode? node = SelectedNode;
            if (node is not null)
            {
                bool eventReturn = TreeViewBeforeCheck(node, TreeViewAction.ByKeyboard);
                if (!eventReturn)
                {
                    node.CheckedInternal = !node.CheckedInternal;
                    TreeViewAfterCheck(node, TreeViewAction.ByKeyboard);
                }

                e.Handled = true;
                return;
            }
        }
    }

    /// <summary>
    ///  Handles the OnBeforeCheck / OnAfterCheck for keyboard clicks
    /// </summary>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Handled)
        {
            return;
        }

        // eat the space key
        if ((e.KeyData & Keys.KeyCode) == Keys.Space)
        {
            e.Handled = true;
            return;
        }
    }

    /// <summary>
    ///  Handles the OnBeforeCheck / OnAfterCheck for keyboard clicks
    /// </summary>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);
        if (e.Handled)
        {
            return;
        }

        // eat the space key
        if (e.KeyChar == ' ')
        {
            e.Handled = true;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected virtual void OnRightToLeftLayoutChanged(EventArgs e)
    {
        if (GetAnyDisposingInHierarchy())
        {
            return;
        }

        if (RightToLeft == RightToLeft.Yes)
        {
            RecreateHandle();
        }

        _onRightToLeftLayoutChanged?.Invoke(this, e);
    }

    // Refresh the nodes by clearing the tree and adding the nodes back again
    //
    private void RefreshNodes()
    {
        TreeNode[] nodes = new TreeNode[Nodes.Count];
        Nodes.CopyTo(nodes, 0);

        Nodes.Clear();
        Nodes.AddRange(nodes);
    }

    /// <summary>
    ///  This resets the indentation to the system default.
    /// </summary>
    private void ResetIndent()
    {
        _indent = -1;
        // is this overkill?
        RecreateHandle();
    }

    /// <summary>
    ///  This resets the item height to the system default.
    /// </summary>
    private void ResetItemHeight()
    {
        _itemHeight = -1;
        RecreateHandle();
    }

    /// <summary>
    ///  Retrieves true if the indent should be persisted in code gen.
    /// </summary>
    private bool ShouldSerializeIndent() => (_indent != -1);

    /// <summary>
    ///  Retrieves true if the itemHeight should be persisted in code gen.
    /// </summary>
    private bool ShouldSerializeItemHeight() => (_itemHeight != -1);

    private bool ShouldSerializeSelectedImageIndex()
    {
        if (_imageList is not null)
        {
            return SelectedImageIndex != 0;
        }

        return SelectedImageIndex != ImageList.Indexer.DefaultIndex;
    }

    private bool ShouldSerializeImageIndex()
    {
        if (_imageList is not null)
        {
            return ImageIndex != 0;
        }

        return ImageIndex != ImageList.Indexer.DefaultIndex;
    }

    /// <summary>
    ///  Updated the sorted order
    /// </summary>
    public void Sort()
    {
        Sorted = true;
        RefreshNodes();
    }

    internal override bool SupportsUiaProviders => true;

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        if (Nodes is not null)
        {
            s += $", Nodes.Count: {Nodes.Count}";
            if (Nodes.Count > 0)
            {
                s += $", Nodes[0]: {Nodes[0]}";
            }
        }

        return s;
    }

    private unsafe void TvnBeginDrag(MouseButtons buttons, NMTREEVIEWW* nmtv)
    {
        TVITEMW item = nmtv->itemNew;

        // Check for invalid node handle
        if (item.hItem == IntPtr.Zero)
        {
            return;
        }

        TreeNode? node = NodeFromHandle(item.hItem);

        OnItemDrag(new ItemDragEventArgs(buttons, node));
    }

    private unsafe nint TvnExpanding(NMTREEVIEWW* nmtv)
    {
        TVITEMW item = nmtv->itemNew;

        // Check for invalid node handle
        if (item.hItem == IntPtr.Zero)
        {
            return 0;
        }

        TreeViewCancelEventArgs? e = null;
        if ((item.state & TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED) == 0)
        {
            e = new TreeViewCancelEventArgs(NodeFromHandle(item.hItem), false, TreeViewAction.Expand);
            OnBeforeExpand(e);
        }
        else
        {
            e = new TreeViewCancelEventArgs(NodeFromHandle(item.hItem), false, TreeViewAction.Collapse);
            OnBeforeCollapse(e);
        }

        return e.Cancel ? 1 : 0;
    }

    private unsafe void TvnExpanded(NMTREEVIEWW* nmtv)
    {
        TVITEMW item = nmtv->itemNew;

        // Check for invalid node handle
        if (item.hItem == IntPtr.Zero)
        {
            return;
        }

        TreeViewEventArgs e;
        TreeNode? node = NodeFromHandle(item.hItem);

        // Note that IsExpanded is invalid for the moment, so we use item item.state to branch.
        if ((item.state & TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED) == 0)
        {
            e = new TreeViewEventArgs(node, TreeViewAction.Collapse);
            OnAfterCollapse(e);
        }
        else
        {
            e = new TreeViewEventArgs(node, TreeViewAction.Expand);
            OnAfterExpand(e);
        }
    }

    private unsafe nint TvnSelecting(NMTREEVIEWW* nmtv)
    {
        if (_treeViewState[TREEVIEWSTATE_ignoreSelects])
        {
            return 1;
        }

        // Check for invalid node handle
        if (nmtv->itemNew.hItem == IntPtr.Zero)
        {
            return 0;
        }

        TreeNode? node = NodeFromHandle(nmtv->itemNew.hItem);

        TreeViewAction action = TreeViewAction.Unknown;
        switch (nmtv->action)
        {
            case NM_TREEVIEW_ACTION.TVC_BYKEYBOARD:
                action = TreeViewAction.ByKeyboard;
                NotifyAboutLostFocus(SelectedNode);
                break;
            case NM_TREEVIEW_ACTION.TVC_BYMOUSE:
                action = TreeViewAction.ByMouse;
                break;
        }

        TreeViewCancelEventArgs e = new(node, false, action);
        OnBeforeSelect(e);

        return e.Cancel ? 1 : 0;
    }

    private unsafe void TvnSelected(NMTREEVIEWW* nmtv)
    {
        // If called from the TreeNodeCollection.Clear() then return.
        if (_nodesCollectionClear)
        {
            return;
        }

        if (nmtv->itemNew.hItem != IntPtr.Zero)
        {
            TreeNode? node = NodeFromHandle(nmtv->itemNew.hItem);
            TreeViewAction action = TreeViewAction.Unknown;
            switch (nmtv->action)
            {
                case NM_TREEVIEW_ACTION.TVC_BYKEYBOARD:
                    action = TreeViewAction.ByKeyboard;
                    NotifyAboutGotFocus(node);
                    break;
                case NM_TREEVIEW_ACTION.TVC_BYMOUSE:
                    action = TreeViewAction.ByMouse;
                    break;
            }

            OnAfterSelect(new TreeViewEventArgs(node, action));
        }

        // TreeView doesn't properly revert back to the unselected image if the unselected image is blank.
        RECT rc = default;
        *((IntPtr*)&rc.left) = nmtv->itemOld.hItem;
        if (nmtv->itemOld.hItem != IntPtr.Zero)
        {
            if (PInvoke.SendMessage(this, PInvoke.TVM_GETITEMRECT, 1, ref rc) != 0)
            {
                PInvoke.InvalidateRect(this, &rc, bErase: true);
            }
        }
    }

    private LRESULT TvnBeginLabelEdit(NMTVDISPINFOW nmtvdi)
    {
        // Check for invalid node handle
        if (nmtvdi.item.hItem == IntPtr.Zero)
        {
            return (LRESULT)IntPtr.Zero;
        }

        Debug.Assert(_labelEdit is null,
            "A new label editing shouldn't start before the previous one ended");

        if (_labelEdit is not null)
        {
            _labelEdit.ReleaseHandle();
            _labelEdit = null;
        }

        TreeNode? editingNode = NodeFromHandle(nmtvdi.item.hItem);
        NodeLabelEditEventArgs e = new(editingNode);
        OnBeforeLabelEdit(e);
        if (!e.CancelEdit)
        {
            _editNode = editingNode;
        }

        if (!e.CancelEdit)
        {
            _labelEdit = new TreeViewLabelEditNativeWindow(this);
            _labelEdit.AssignHandle(PInvoke.SendMessage(this, PInvoke.TVM_GETEDITCONTROL));
        }

        return (LRESULT)(e.CancelEdit ? 1 : 0);
    }

    private LRESULT TvnEndLabelEdit(NMTVDISPINFOW nmtvdi)
    {
        _editNode = null;

        // Check for invalid node handle
        if (nmtvdi.item.hItem == IntPtr.Zero)
        {
            return (LRESULT)1;
        }

        if (_labelEdit is not null)
        {
            _labelEdit.ReleaseHandle();
            _labelEdit = null;
        }

        TreeNode? node = NodeFromHandle(nmtvdi.item.hItem);
        string newText = nmtvdi.item.pszText.ToString();
        NodeLabelEditEventArgs e = new(node, newText);
        OnAfterLabelEdit(e);
        if (newText is not null && !e.CancelEdit && node is not null)
        {
            node._text = newText;
            if (Scrollable)
            {
                ForceScrollbarUpdate(true);
            }
        }

        return (LRESULT)(e.CancelEdit ? 0 : 1);
    }

    internal override void UpdateStylesCore()
    {
        base.UpdateStylesCore();
        if (IsHandleCreated && CheckBoxes)
        {
            if (StateImageList is not null)
            {
                // Setting the TVS_CHECKBOXES window style also causes the TreeView to display the default checkbox
                // images rather than the user specified StateImageList.  We send a TVM_SETIMAGELIST to restore the
                // user's images.
                if (_internalStateImageList is not null)
                {
                    SetStateImageList(_internalStateImageList.Handle);
                }
            }
        }
    }

    private void UpdateTreeViewExtendedStyles()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        // Only set the TVS_EX_DOUBLEBUFFER style if the DoubleBuffered property setter has been executed.
        // This stops the style from being removed for any derived classes that set it using P/Invoke.
        if (_treeViewState[TREEVIEWSTATE_doubleBufferedPropertySet])
        {
            PInvoke.SendMessage(this,
                PInvoke.TVM_SETEXTENDEDSTYLE,
                (WPARAM)(nint)PInvoke.TVS_EX_DOUBLEBUFFER,
                (LPARAM)(nint)(DoubleBuffered ? PInvoke.TVS_EX_DOUBLEBUFFER : 0));
        }
    }

    /// <remarks>
    ///  <para>Setting the PInvoke.TVS_CHECKBOXES style clears the checked state</para>
    /// </remarks>
    private static void UpdateCheckedState(TreeNode node, bool update)
    {
        // This looks funny, but CheckedInternal returns the cached isChecked value and the internal
        // setter will blindly issue TVM_SETITEM so this gets us back in sync.
        if (update)
        {
            node.CheckedInternal = node.CheckedInternal;
            for (int i = node.Nodes.Count - 1; i >= 0; i--)
            {
                UpdateCheckedState(node.Nodes[i], update);
            }
        }
        else
        {
            node.CheckedInternal = false;
            for (int i = node.Nodes.Count - 1; i >= 0; i--)
            {
                UpdateCheckedState(node.Nodes[i], update);
            }
        }
    }

    private void WmMouseDown(ref Message m, MouseButtons button, int clicks)
    {
        // Required to put the TreeView in sane-state for painting proper highlighting of selectedNodes.
        // If the user shows the ContextMenu bu overriding the WndProc( ), then the treeView
        // goes into the weird state where the high-light gets locked to the node on which the ContextMenu was shown.
        // So we need to get the native TREEVIEW out of this weird state.
        PInvoke.SendMessage(this, PInvoke.TVM_SELECTITEM, (WPARAM)PInvoke.TVGN_DROPHILITE);

        // Windows TreeView pushes its own message loop in WM_xBUTTONDOWN, so fire the
        // event before calling defWndProc or else it won't get fired until the button
        // comes back up.
        OnMouseDown(new MouseEventArgs(button, clicks, PARAM.ToPoint(m.LParamInternal)));

        // If Validation is cancelled don't fire any events through the Windows TreeView's message loop.
        if (!ValidationCancelled)
        {
            DefWndProc(ref m);
        }
    }

    /// <summary>
    ///  Performs custom draw handling
    /// </summary>
    private unsafe void CustomDraw(ref Message m)
    {
        NMTVCUSTOMDRAW* nmtvcd = (NMTVCUSTOMDRAW*)(nint)m.LParamInternal;

        // Find out which stage we're drawing
        switch (nmtvcd->nmcd.dwDrawStage)
        {
            // Do we want OwnerDraw for this paint cycle?
            case NMCUSTOMDRAW_DRAW_STAGE.CDDS_PREPAINT:
                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NOTIFYITEMDRAW; // yes, we do...
                return;
            // We've got opt-in on owner draw for items - so handle each one.
            case NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPREPAINT:
                // get the node
                Debug.Assert(nmtvcd->nmcd.dwItemSpec != 0, "Invalid node handle in ITEMPREPAINT");
                TreeNode? node = NodeFromHandle((nint)nmtvcd->nmcd.dwItemSpec);

                if (node is null)
                {
                    // this can happen if we are presently inserting the node - it hasn't yet
                    // been added to the handle table
                    m.ResultInternal = (LRESULT)(nint)(PInvoke.CDRF_SKIPDEFAULT);
                    return;
                }

                NMCUSTOMDRAW_DRAW_STATE_FLAGS state = nmtvcd->nmcd.uItemState;

                // The commctrl TreeView allows you to draw the whole row of a node
                // or nothing at all. The way we provide OwnerDrawText is by asking it
                // to draw everything but the text - to do this, we set text color same
                // as background color.
                if (_drawMode == TreeViewDrawMode.OwnerDrawText)
                {
                    nmtvcd->clrText = nmtvcd->clrTextBk;
                    m.ResultInternal = (LRESULT)(nint)(PInvoke.CDRF_NEWFONT | PInvoke.CDRF_NOTIFYPOSTPAINT);
                    return;
                }
                else if (_drawMode == TreeViewDrawMode.OwnerDrawAll)
                {
                    DrawTreeNodeEventArgs e;
                    using (Graphics g = nmtvcd->nmcd.hdc.CreateGraphics())
                    {
                        Rectangle bounds = node.RowBounds;

                        SCROLLINFO si = new()
                        {
                            cbSize = (uint)sizeof(SCROLLINFO),
                            fMask = SCROLLINFO_MASK.SIF_POS
                        };

                        if (PInvoke.GetScrollInfo(this, SCROLLBAR_CONSTANTS.SB_HORZ, ref si))
                        {
                            // need to get the correct bounds if horizontal scroll bar is shown.
                            // In this case the bounds.X needs to be negative and width needs to be updated to the increased width (scrolled region).
                            int value = si.nPos;
                            if (value > 0)
                            {
                                bounds.X -= value;
                                bounds.Width += value;
                            }
                        }

                        e = new DrawTreeNodeEventArgs(g, node, bounds, (TreeNodeStates)(state));
                        OnDrawNode(e);
                    }

                    if (!e.DrawDefault)
                    {
                        m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_SKIPDEFAULT;
                        return;
                    }
                }

                // TreeViewDrawMode.Normal case
                OwnerDrawPropertyBag renderinfo = GetItemRenderStyles(node, (int)state);

                // TreeView has problems with drawing items at times; it gets confused
                // as to which colors apply to which items (see focus rectangle shifting;
                // when one item is selected, click and hold on another). This needs to be fixed.
                Color riFore = renderinfo.ForeColor;
                Color riBack = renderinfo.BackColor;
                if (renderinfo is not null && !riFore.IsEmpty)
                {
                    nmtvcd->clrText = ColorTranslator.ToWin32(riFore);
                }

                if (renderinfo is not null && !riBack.IsEmpty)
                {
                    nmtvcd->clrTextBk = ColorTranslator.ToWin32(riBack);
                }

                if (renderinfo is not null && renderinfo.Font is not null)
                {
                    // Mess with the DC directly...
                    PInvoke.SelectObject(nmtvcd->nmcd.hdc, renderinfo.FontHandle);

                    // There is a problem in winctl that clips node fonts if the fontSize
                    // is larger than the treeView font size. The behavior is much better in comctl 5 and above.
                    m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NEWFONT;
                    return;
                }

                // fall through and do the default drawing work
                goto default;

            case NMCUSTOMDRAW_DRAW_STAGE.CDDS_ITEMPOSTPAINT:
                // User draws only the text in OwnerDrawText mode, as explained in comments above
                if (_drawMode == TreeViewDrawMode.OwnerDrawText)
                {
                    Debug.Assert(nmtvcd->nmcd.dwItemSpec != 0, "Invalid node handle in ITEMPOSTPAINT");

                    // Get the node
                    node = NodeFromHandle((nint)nmtvcd->nmcd.dwItemSpec);

                    if (node is null)
                    {
                        // this can happen if we are presently inserting the node - it hasn't yet
                        // been added to the handle table
                        return;
                    }

                    using (Graphics g = nmtvcd->nmcd.hdc.CreateGraphics())
                    {
                        Rectangle bounds = node.Bounds;
                        Size textSize = TextRenderer.MeasureText(node.Text, node.TreeView!.Font);
                        Point textLoc = new(bounds.X - 1, bounds.Y); // required to center the text
                        bounds = new Rectangle(textLoc, new Size(textSize.Width, bounds.Height));

                        DrawTreeNodeEventArgs e = new(g, node, bounds, (TreeNodeStates)(nmtvcd->nmcd.uItemState));
                        OnDrawNode(e);

                        if (e.DrawDefault)
                        {
                            // Simulate default text drawing here
                            TreeNodeStates curState = e.State;

                            Font font = node.NodeFont ?? node.TreeView.Font;
                            Color color = (((curState & TreeNodeStates.Selected) == TreeNodeStates.Selected) && node.TreeView.Focused) ? Application.SystemColors.HighlightText : (node.ForeColor != Color.Empty) ? node.ForeColor : node.TreeView.ForeColor;

                            // Draw the actual node.
                            if ((curState & TreeNodeStates.Selected) == TreeNodeStates.Selected)
                            {
                                g.FillRectangle(SystemBrushes.Highlight, bounds);
                                ControlPaint.DrawFocusRectangle(g, bounds, color, Application.SystemColors.Highlight);
                                TextRenderer.DrawText(g, node.Text, font, bounds, color, TextFormatFlags.Default);
                            }
                            else
                            {
                                using var brush = BackColor.GetCachedSolidBrushScope();
                                g.FillRectangle(brush, bounds);

                                TextRenderer.DrawText(g, node.Text, font, bounds, color, TextFormatFlags.Default);
                            }
                        }
                    }

                    m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_NOTIFYSUBITEMDRAW;
                    return;
                }

                goto default;

            default:
                // just in case we get a spurious message, tell it to do the right thing
                m.ResultInternal = (LRESULT)(nint)PInvoke.CDRF_DODEFAULT;
                return;
        }
    }

    /// <summary>
    ///  Generates colors for each item. This can be overridden to provide colors on a per state/per node
    ///  basis, rather than using the ForeColor/BackColor/NodeFont properties on TreeNode.
    /// </summary>
    protected OwnerDrawPropertyBag GetItemRenderStyles(TreeNode? node, int state)
    {
        OwnerDrawPropertyBag retval = new();
        if (node is null || node._propBag is null)
        {
            return retval;
        }

        // we only change colors if we're displaying things normally
        if ((state &
            (int)(NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_SELECTED |
            NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_GRAYED |
            NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_HOT |
            NMCUSTOMDRAW_DRAW_STATE_FLAGS.CDIS_DISABLED)) == 0)
        {
            retval.ForeColor = node._propBag.ForeColor;
            retval.BackColor = node._propBag.BackColor;
        }

        retval.Font = node._propBag.Font;
        return retval;
    }

    internal override unsafe ToolInfoWrapper<Control> GetToolInfoWrapper(TOOLTIP_FLAGS flags, string? caption, ToolTip tooltip)
    {
        // The "ShowNodeToolTips" flag is required so that when the user hovers over the TreeNode,
        // their own tooltip is displayed, not the TreeView tooltip.
        // The second condition is necessary for the correct display of the keyboard tooltip,
        // since the logic of the external tooltip blocks its display
        bool isExternalTooltip = ShowNodeToolTips && tooltip != KeyboardToolTip;
        ToolInfoWrapper<Control> wrapper = new(this, flags, isExternalTooltip ? null : caption);
        if (isExternalTooltip)
        {
            wrapper.Info.lpszText = (char*)(-1);
        }

        return wrapper;
    }

    private unsafe bool WmShowToolTip(ref Message m)
    {
        NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
        HWND tooltipHandle = nmhdr->hwndFrom;

        TVHITTESTINFO tvhip = new()
        {
            pt = PointToClient(Cursor.Position)
        };

        nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhip);
        if (hnode != 0 && tvhip.flags.HasFlag(TVHITTESTINFO_FLAGS.TVHT_ONITEM) && NodeFromHandle(hnode) is { } tn && !ShowNodeToolTips)
        {
            Rectangle bounds = tn.Bounds;
            bounds.Location = PointToScreen(bounds.Location);

            PInvoke.SendMessage(tooltipHandle, PInvoke.TTM_ADJUSTRECT, (WPARAM)(BOOL)true, ref bounds);
            PInvoke.SetWindowPos(
                tooltipHandle,
                HWND.HWND_TOPMOST,
                bounds.Left,
                bounds.Top,
                0,
                0,
                SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE | SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);
            return true;
        }

        return false;
    }

    private unsafe void WmNeedText(ref Message m)
    {
        NMTTDISPINFOW* ttt = (NMTTDISPINFOW*)(nint)m.LParamInternal;
        string? tipText = _controlToolTipText;

        TVHITTESTINFO tvhip = new()
        {
            pt = PointToClient(Cursor.Position)
        };

        nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhip);
        if (hnode != 0 && ((tvhip.flags & TVHITTESTINFO_FLAGS.TVHT_ONITEM) != 0))
        {
            TreeNode? tn = NodeFromHandle(hnode);
            if (ShowNodeToolTips && tn is not null && (!string.IsNullOrEmpty(tn.ToolTipText)))
            {
                tipText = tn.ToolTipText;
            }
            else if (tn is not null && tn.Bounds.Right > Bounds.Right)
            {
                tipText = tn.Text;
            }
            else
            {
                tipText = null;
            }
        }

        _toolTipBuffer.SetText(tipText);
        ttt->lpszText = (char*)_toolTipBuffer.Buffer;
        ttt->hinst = HINSTANCE.Null;

        // RightToLeft reading order
        if (RightToLeft == RightToLeft.Yes)
        {
            ttt->uFlags |= TOOLTIP_FLAGS.TTF_RTLREADING;
        }
    }

    private unsafe void WmNotify(ref Message m)
    {
        NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;

        // Custom draw code is handled separately.
        if (nmhdr->code == PInvoke.NM_CUSTOMDRAW)
        {
            CustomDraw(ref m);
        }
        else
        {
            NMTREEVIEWW* nmtv = (NMTREEVIEWW*)(nint)m.LParamInternal;

            switch (nmtv->hdr.code)
            {
                case PInvoke.TVN_ITEMEXPANDINGW:
                    m.ResultInternal = (LRESULT)TvnExpanding(nmtv);
                    break;
                case PInvoke.TVN_ITEMEXPANDEDW:
                    TvnExpanded(nmtv);
                    break;
                case PInvoke.TVN_SELCHANGINGW:
                    m.ResultInternal = (LRESULT)TvnSelecting(nmtv);
                    break;
                case PInvoke.TVN_SELCHANGEDW:
                    TvnSelected(nmtv);
                    break;
                case PInvoke.TVN_BEGINDRAGW:
                    TvnBeginDrag(MouseButtons.Left, nmtv);
                    break;
                case PInvoke.TVN_BEGINRDRAGW:
                    TvnBeginDrag(MouseButtons.Right, nmtv);
                    break;
                case PInvoke.TVN_BEGINLABELEDITW:
                    m.ResultInternal = TvnBeginLabelEdit(*(NMTVDISPINFOW*)(nint)m.LParamInternal);
                    break;
                case PInvoke.TVN_ENDLABELEDITW:
                    m.ResultInternal = TvnEndLabelEdit(*(NMTVDISPINFOW*)(nint)m.LParamInternal);
                    break;
                case PInvoke.NM_CLICK:
                case PInvoke.NM_RCLICK:
                    MouseButtons button = MouseButtons.Left;
                    Point pos = PointToClient(Cursor.Position);
                    TVHITTESTINFO tvhip = new()
                    {
                        pt = pos
                    };

                    nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhip);
                    if (nmtv->hdr.code != PInvoke.NM_CLICK || (tvhip.flags & TVHITTESTINFO_FLAGS.TVHT_ONITEM) != 0)
                    {
                        button = nmtv->hdr.code == PInvoke.NM_CLICK ? MouseButtons.Left : MouseButtons.Right;
                    }

                    // The treeView's WndProc doesn't get the WM_LBUTTONUP messages when
                    // LBUTTONUP happens on TVHT_ONITEM. This is a comctl quirk.
                    // We work around that by calling OnMouseUp here.
                    if (nmtv->hdr.code != PInvoke.NM_CLICK
                        || (tvhip.flags & TVHITTESTINFO_FLAGS.TVHT_ONITEM) != 0 || FullRowSelect)
                    {
                        if (hnode != 0 && !ValidationCancelled)
                        {
                            OnNodeMouseClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), button, 1, pos.X, pos.Y));
                            OnClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                            OnMouseClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                        }
                    }

                    if (nmtv->hdr.code == PInvoke.NM_RCLICK)
                    {
                        TreeNode? treeNode = NodeFromHandle(hnode);
                        if (treeNode is not null && treeNode.ContextMenuStrip is not null)
                        {
                            ShowContextMenu(treeNode);
                        }
                        else
                        {
                            _treeViewState[TREEVIEWSTATE_showTreeViewContextMenu] = true;
                            PInvoke.SendMessage(this, PInvoke.WM_CONTEXTMENU, (WPARAM)HWND, (LPARAM)PInvoke.GetMessagePos());
                        }

                        m.ResultInternal = (LRESULT)1;
                    }

                    if (!_treeViewState[TREEVIEWSTATE_mouseUpFired])
                    {
                        if (nmtv->hdr.code != PInvoke.NM_CLICK
                        || (tvhip.flags & TVHITTESTINFO_FLAGS.TVHT_ONITEM) != 0)
                        {
                            // The treeView's WndProc doesn't get the WM_LBUTTONUP messages when
                            // LBUTTONUP happens on TVHT_ONITEM. This is a comctl quirk.
                            // We work around that by calling OnMouseUp here.
                            OnMouseUp(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                            _treeViewState[TREEVIEWSTATE_mouseUpFired] = true;
                        }
                    }

                    break;
            }
        }
    }

    protected override void OnGotFocus(EventArgs e)
    {
        base.OnGotFocus(e);
        NotifyAboutGotFocus(SelectedNode);

        // Raise an event to highlight & announce the selected node.
        if (IsAccessibilityObjectCreated)
        {
            SelectedNode?.AccessibilityObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
        }
    }

    protected override void OnLostFocus(EventArgs e)
    {
        base.OnLostFocus(e);
        NotifyAboutLostFocus(SelectedNode);
    }

    /// <summary>
    ///  Shows the context menu for the <see cref="TreeNode"/>.
    /// </summary>
    private void ShowContextMenu(TreeNode treeNode)
    {
        if (treeNode.ContextMenuStrip is not null)
        {
            ContextMenuStrip menu = treeNode.ContextMenuStrip;

            // Need to send TVM_SELECTITEM to highlight the node while the contextMenuStrip is being shown.
            PInvoke.PostMessage(this, PInvoke.TVM_SELECTITEM, (WPARAM)PInvoke.TVGN_DROPHILITE, (LPARAM)treeNode.Handle);
            menu.ShowInternal(this, PointToClient(MousePosition), /*keyboardActivated*/false);
            menu.Closing += new ToolStripDropDownClosingEventHandler(ContextMenuStripClosing);
        }
    }

    // Need to send TVM_SELECTITEM to reset the node-highlighting while the contextMenuStrip is being closed so that the treeView reselects the SelectedNode.
    private void ContextMenuStripClosing(object? sender, ToolStripDropDownClosingEventArgs e)
    {
        ContextMenuStrip strip = (ContextMenuStrip)sender!;
        // Unhook the Event.
        strip.Closing -= new ToolStripDropDownClosingEventHandler(ContextMenuStripClosing);
        PInvoke.SendMessage(this, PInvoke.TVM_SELECTITEM, (WPARAM)PInvoke.TVGN_DROPHILITE);
    }

    private void UnhookNodes()
    {
        foreach (TreeNode rootNode in Nodes)
        {
            foreach (TreeNode node in rootNode.GetSelfAndChildNodes())
            {
                KeyboardToolTipStateMachine.Instance.Unhook(node, KeyboardToolTip);
            }
        }
    }

    private void WmPrint(ref Message m)
    {
        base.WndProc(ref m);

        if (((nint)m.LParamInternal & PInvoke.PRF_NONCLIENT) != 0
            && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
        {
            using Graphics g = Graphics.FromHdc((HDC)m.WParamInternal);
            Rectangle rect = new(0, 0, Size.Width - 1, Size.Height - 1);
            using var pen = VisualStyleInformation.TextControlBorder.GetCachedPenScope();
            g.DrawRectangle(pen, rect);
            rect.Inflate(-1, -1);
            g.DrawRectangle(SystemPens.Window, rect);
        }
    }

    protected override unsafe void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_WINDOWPOSCHANGING:
            case PInvoke.WM_NCCALCSIZE:
            case PInvoke.WM_WINDOWPOSCHANGED:
            case PInvoke.WM_SIZE:
                // While we are changing size of treeView to avoid the scrollbar; don't respond to the window-sizing messages.
                if (_treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs])
                {
                    DefWndProc(ref m);
                }
                else
                {
                    base.WndProc(ref m);
                }

                break;
            case PInvoke.WM_HSCROLL:
                base.WndProc(ref m);
                if (DrawMode == TreeViewDrawMode.OwnerDrawAll)
                {
                    Invalidate();
                }

                break;

            case PInvoke.WM_PRINT:
                WmPrint(ref m);
                break;
            case PInvoke.TVM_SETITEMW:
                base.WndProc(ref m);
                if (CheckBoxes)
                {
                    TVITEMW* item = (TVITEMW*)(nint)m.LParamInternal;

                    // Check for invalid node handle
                    if (item->hItem != IntPtr.Zero)
                    {
                        TVITEMW item1 = new()
                        {
                            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
                            hItem = item->hItem,
                            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK
                        };

                        PInvoke.SendMessage(this, PInvoke.TVM_GETITEMW, 0, ref item1);

                        TreeNode node = NodeFromHandle(item->hItem)!;
                        node.CheckedStateInternal = (((int)item1.state >> SHIFTVAL) > 1);
                    }
                }

                break;
            case PInvoke.WM_NOTIFY:
                NMHDR* nmhdr = (NMHDR*)(nint)m.LParamInternal;
                switch (nmhdr->code)
                {
                    case PInvoke.TTN_GETDISPINFOW:
                        // Setting the max width has the added benefit of enabling multiline tool tips
                        PInvoke.SendMessage(nmhdr->hwndFrom, PInvoke.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                        WmNeedText(ref m);
                        m.ResultInternal = (LRESULT)1;
                        return;
                    case PInvoke.TTN_SHOW:
                        if (WmShowToolTip(ref m))
                        {
                            m.ResultInternal = (LRESULT)1;
                            return;
                        }
                        else
                        {
                            base.WndProc(ref m);
                            break;
                        }

                    default:
                        base.WndProc(ref m);
                        break;
                }

                break;
            case MessageId.WM_REFLECT_NOTIFY:
                WmNotify(ref m);
                break;
            case PInvoke.WM_LBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Left, 2);

                // Just maintain state and fire double click in final mouseUp.
                _treeViewState[TREEVIEWSTATE_doubleclickFired] = true;

                // Fire mouse up in the Wndproc.
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;

                // Make sure we get the mouse up if it happens outside the control.
                Capture = true;
                break;
            case PInvoke.WM_LBUTTONDOWN:
                try
                {
                    _treeViewState[TREEVIEWSTATE_ignoreSelects] = true;
                    Focus();
                }
                finally
                {
                    _treeViewState[TREEVIEWSTATE_ignoreSelects] = false;
                }

                // Always reset the MouseUpFired.
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                TVHITTESTINFO tvhip = new()
                {
                    pt = PARAM.ToPoint(m.LParamInternal)
                };

                _mouseDownNode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhip);

                // This gets around the TreeView behavior of temporarily moving the selection
                // highlight to a node when the user clicks on its checkbox.
                if ((tvhip.flags & TVHITTESTINFO_FLAGS.TVHT_ONITEMSTATEICON) != 0)
                {
                    // We do not pass the Message to the Control so fire MouseDown.
                    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, PARAM.ToPoint(m.LParamInternal)));
                    if (!ValidationCancelled && CheckBoxes)
                    {
                        TreeNode? node = NodeFromHandle(_mouseDownNode);
                        bool eventReturn = TreeViewBeforeCheck(node, TreeViewAction.ByMouse);
                        if (!eventReturn && node is not null)
                        {
                            node.CheckedInternal = !node.CheckedInternal;
                            TreeViewAfterCheck(node, TreeViewAction.ByMouse);
                        }
                    }

                    m.ResultInternal = (LRESULT)0;
                }
                else
                {
                    WmMouseDown(ref m, MouseButtons.Left, 1);
                }

                _downButton = MouseButtons.Left;
                break;
            case PInvoke.WM_LBUTTONUP:
            case PInvoke.WM_RBUTTONUP:
                Point point = PARAM.ToPoint(m.LParamInternal);

                TVHITTESTINFO tvhi = new()
                {
                    pt = point
                };

                nint hnode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhi);

                // Important for CheckBoxes. Click needs to be fired.
                if (hnode != 0)
                {
                    if (!ValidationCancelled && !_treeViewState[TREEVIEWSTATE_doubleclickFired] & !_treeViewState[TREEVIEWSTATE_mouseUpFired])
                    {
                        // If the hit-tested node here is the same as the node we hit-tested
                        // on mouse down then we will fire our OnNodeMouseClick event.
                        if (hnode == _mouseDownNode)
                        {
                            OnNodeMouseClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), _downButton, 1, point.X, point.Y));
                        }

                        OnClick(new MouseEventArgs(_downButton, 1, point));
                        OnMouseClick(new MouseEventArgs(_downButton, 1, point));
                    }

                    if (_treeViewState[TREEVIEWSTATE_doubleclickFired])
                    {
                        _treeViewState[TREEVIEWSTATE_doubleclickFired] = false;
                        if (!ValidationCancelled)
                        {
                            OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), _downButton, 2, point.X, point.Y));
                            OnDoubleClick(new MouseEventArgs(_downButton, 2, point));
                            OnMouseDoubleClick(new MouseEventArgs(_downButton, 2, point));
                        }
                    }
                }

                if (!_treeViewState[TREEVIEWSTATE_mouseUpFired])
                {
                    OnMouseUp(new MouseEventArgs(_downButton, 1, point));
                }

                _treeViewState[TREEVIEWSTATE_doubleclickFired] = false;
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                Capture = false;

                // Always clear our hit-tested node we cached on mouse down
                _mouseDownNode = IntPtr.Zero;
                break;
            case PInvoke.WM_MBUTTONDBLCLK:
                // Fire mouse up in the Wndproc.
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                WmMouseDown(ref m, MouseButtons.Middle, 2);
                break;
            case PInvoke.WM_MBUTTONDOWN:
                // Always reset MouseUpFired.
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                WmMouseDown(ref m, MouseButtons.Middle, 1);
                _downButton = MouseButtons.Middle;
                break;
            case PInvoke.WM_MOUSELEAVE:
                // if the mouse leaves and then reenters the TreeView
                // NodeHovered events should be raised.
                _prevHoveredNode = null;
                base.WndProc(ref m);
                break;
            case PInvoke.WM_RBUTTONDBLCLK:
                WmMouseDown(ref m, MouseButtons.Right, 2);

                // Just maintain state and fire double click in the final mouseUp.
                _treeViewState[TREEVIEWSTATE_doubleclickFired] = true;

                // Fire mouse up in the Wndproc
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;

                // Make sure we get the mouse up if it happens outside the control.
                Capture = true;
                break;
            case PInvoke.WM_RBUTTONDOWN:
                // Always Reset the MouseUpFired....
                _treeViewState[TREEVIEWSTATE_mouseUpFired] = false;

                // Cache the hit-tested node for verification when mouse up is fired
                TVHITTESTINFO tvhit = new()
                {
                    pt = PARAM.ToPoint(m.LParamInternal)
                };

                _mouseDownNode = PInvoke.SendMessage(this, PInvoke.TVM_HITTEST, 0, ref tvhit);

                WmMouseDown(ref m, MouseButtons.Right, 1);
                _downButton = MouseButtons.Right;
                break;
            case PInvoke.WM_SYSCOLORCHANGE:
                PInvoke.SendMessage(this, PInvoke.TVM_SETINDENT, (WPARAM)Indent);
                base.WndProc(ref m);
                break;
            case PInvoke.WM_SETFOCUS:
                // If we get focus through the LButtonDown .. we might have done the validation...
                // so skip it..
                if (_treeViewState[TREEVIEWSTATE_lastControlValidated])
                {
                    _treeViewState[TREEVIEWSTATE_lastControlValidated] = false;
                    WmImeSetFocus();
                    DefWndProc(ref m);
                    InvokeGotFocus(this, EventArgs.Empty);
                }
                else
                {
                    base.WndProc(ref m);
                }

                break;
            case PInvoke.WM_CONTEXTMENU:
                if (_treeViewState[TREEVIEWSTATE_showTreeViewContextMenu])
                {
                    _treeViewState[TREEVIEWSTATE_showTreeViewContextMenu] = false;
                    base.WndProc(ref m);
                }
                else
                {
                    // this is the Shift + F10 Case....
                    TreeNode? treeNode = SelectedNode;
                    if (treeNode is not null && treeNode.ContextMenuStrip is not null)
                    {
                        Point client;
                        client = new Point(treeNode.Bounds.X, treeNode.Bounds.Y + treeNode.Bounds.Height / 2);
                        // VisualStudio7 # 156, only show the context menu when clicked in the client area
                        if (ClientRectangle.Contains(client) && treeNode.ContextMenuStrip is not null)
                        {
                            bool keyboardActivated = m.LParamInternal == -1;
                            treeNode.ContextMenuStrip.ShowInternal(this, client, keyboardActivated);
                        }
                    }
                    else
                    {
                        // in this case we don't have a selected node.  The base
                        // will ensure we're constrained to the client area.
                        base.WndProc(ref m);
                    }
                }

                break;
            case PInvoke.WM_GETOBJECT:
                // When TreeView is the first control in TabOrder on a form, selection will not be announced when the forms is first opened
                // because TreeView's AccessibilityObject has not been created yet. We create this object proactively to support that case.
                // You can also refer to this PR. https://github.com/dotnet/winforms/pull/9925
                if (m.LParamInternal == PInvoke.UiaRootObjectId && SupportsUiaProviders && !IsAccessibilityObjectCreated && Focused)
                {
                    base.WndProc(ref m);
                    SelectedNode?.AccessibilityObject?.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
                }
                else
                {
                    base.WndProc(ref m);
                }

                break;
            default:
                base.WndProc(ref m);
                break;
        }
    }
}
