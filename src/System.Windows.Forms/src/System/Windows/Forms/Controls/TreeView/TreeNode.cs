// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;

namespace System.Windows.Forms;

/// <summary>
///  Implements a node of a <see cref="TreeView"/>.
/// </summary>
[TypeConverter(typeof(TreeNodeConverter))]
[Serializable]  // This class participates in ResX serialization.
[DefaultProperty(nameof(Text))]
[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
public partial class TreeNode : MarshalByRefObject, ICloneable, ISerializable
{
    internal const int SHIFTVAL = 12;
    private const TREE_VIEW_ITEM_STATE_FLAGS CHECKED = (TREE_VIEW_ITEM_STATE_FLAGS)(2 << SHIFTVAL);
    private const TREE_VIEW_ITEM_STATE_FLAGS UNCHECKED = (TREE_VIEW_ITEM_STATE_FLAGS)(1 << SHIFTVAL);
    private const int ALLOWEDIMAGES = 14;

    // the threshold value used to optimize AddRange and Clear operations for a big number of nodes
    internal const int MAX_TREENODES_OPS = 200;

    // we use it to store font and color data in a minimal-memory-cost manner
    // ie. nodes which don't use fancy fonts or colors (ie. that use the TreeView settings for these)
    //     will take up less memory than those that do.
    internal OwnerDrawPropertyBag? _propBag;
    internal string? _text;
    internal string? _name;

    // note: as the checked state of a node is user controlled, and this variable is simply for
    // state caching when a node hasn't yet been realized, you should use the Checked property to
    // find out the check state of a node, and not this member variable.
    // private bool isChecked = false;
    private const int TREENODESTATE_isChecked = 0x00000001;

    private Collections.Specialized.BitVector32 _treeNodeState;

    private TreeNodeImageIndexer? _imageIndexer;
    private TreeNodeImageIndexer? _selectedImageIndexer;
    private TreeNodeImageIndexer? _stateImageIndexer;

    private string _toolTipText = string.Empty;
    private ContextMenuStrip? _contextMenuStrip;
    internal bool _nodesCleared;

    private TreeNodeAccessibleObject? _accessibleObject;

    internal TreeNodeImageIndexer ImageIndexer
    {
        get
        {
            // Demand create the imageIndexer
            _imageIndexer ??= new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);

            return _imageIndexer;
        }
    }

    internal TreeNodeImageIndexer SelectedImageIndexer
    {
        get
        {
            // Demand create the imageIndexer
            _selectedImageIndexer ??= new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);

            return _selectedImageIndexer;
        }
    }

    internal TreeNodeImageIndexer StateImageIndexer
    {
        get
        {
            // Demand create the imageIndexer
            _stateImageIndexer ??= new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.State);

            return _stateImageIndexer;
        }
    }

    internal int _index;                  // our index into our parents child array
    internal int _childCount;

    // this array should not be optimized as a list because we are inserting into the middle of it, not appending.
#pragma warning disable CA5362 // Potential reference cycle in deserialized object graph
    internal TreeNode[] _children = [];
    internal TreeNode? _parent;
#pragma warning restore CA5362

    internal TreeView? _treeView;
    private bool _expandOnRealization;
    private bool _collapseOnRealization;
    private TreeNodeCollection? _nodes;
    private object? _userData;

    private const TVITEM_MASK InsertMask =
        TVITEM_MASK.TVIF_TEXT
        | TVITEM_MASK.TVIF_IMAGE
        | TVITEM_MASK.TVIF_SELECTEDIMAGE;

    /// <summary>
    ///  Creates a TreeNode object.
    /// </summary>
    public TreeNode()
    {
        _treeNodeState = default;
    }

    internal TreeNode(TreeView treeView)
        : this()
    {
        _treeView = treeView;
    }

    /// <summary>
    ///  Creates a TreeNode object.
    /// </summary>
    public TreeNode(string? text)
        : this()
    {
        _text = text;
    }

    /// <summary>
    ///  Creates a TreeNode object.
    /// </summary>
    public TreeNode(string? text, TreeNode[] children)
        : this(text)
    {
        Nodes.AddRange(children);
    }

    /// <summary>
    ///  Creates a TreeNode object.
    /// </summary>
    public TreeNode(string? text, int imageIndex, int selectedImageIndex)
        : this(text)
    {
        ImageIndex = imageIndex;
        SelectedImageIndex = selectedImageIndex;
    }

    /// <summary>
    ///  Creates a TreeNode object.
    /// </summary>
    public TreeNode(string? text, int imageIndex, int selectedImageIndex, TreeNode[] children)
        : this(text, imageIndex, selectedImageIndex)
    {
        Nodes.AddRange(children);
    }

    /// <summary>
    ///  Constructor used in deserialization from resources.
    /// </summary>
    protected TreeNode(SerializationInfo serializationInfo, StreamingContext context)
        : this()
    {
        Deserialize(serializationInfo, context);
    }

    /// <summary>
    ///  The background color of this node.
    ///  If null, the color used will be the default color from the TreeView control that this
    ///  node is attached to
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeNodeBackColorDescr))]
    public Color BackColor
    {
        get
        {
            if (_propBag is null)
            {
                return Color.Empty;
            }

            return _propBag.BackColor;
        }
        set
        {
            // get the old value
            Color oldbk = BackColor;
            // If we're setting the color to the default again, delete the propBag if it doesn't contain
            // useful data.
            if (value.IsEmpty)
            {
                if (_propBag is not null)
                {
                    _propBag.BackColor = Color.Empty;
                    RemovePropBagIfEmpty();
                }

                if (!oldbk.IsEmpty)
                {
                    InvalidateHostTree();
                }

                return;
            }

            // Not the default, so if necessary create a new propBag, and fill it with the BackColor

            _propBag ??= new OwnerDrawPropertyBag();

            _propBag.BackColor = value;
            if (!value.Equals(oldbk))
            {
                InvalidateHostTree();
            }
        }
    }

    /// <summary>
    ///  The bounding rectangle for the node (text area only). The coordinates
    ///  are relative to the upper left corner of the TreeView control.
    /// </summary>
    [Browsable(false)]
    public Rectangle Bounds
    {
        get
        {
            TreeView? tv = TreeView;
            if (tv is null || tv.IsDisposed)
            {
                return Rectangle.Empty;
            }

            RECT rc = default;
            unsafe
            { *((IntPtr*)&rc.left) = Handle; }
            // wparam: 1=include only text, 0=include entire line
            if (PInvokeCore.SendMessage(tv, PInvoke.TVM_GETITEMRECT, 1, ref rc) == 0)
            {
                // This means the node is not visible
                return Rectangle.Empty;
            }

            return rc;
        }
    }

    /// <summary>
    ///  The bounding rectangle for the node (full row). The coordinates
    ///  are relative to the upper left corner of the TreeView control.
    /// </summary>
    internal Rectangle RowBounds
    {
        get
        {
            TreeView? tv = TreeView;
            RECT rc = default;
            unsafe
            { *((IntPtr*)&rc.left) = Handle; }

            // wparam: 1=include only text, 0=include entire line
            if (tv is null || tv.IsDisposed)
            {
                return Rectangle.Empty;
            }

            if (PInvokeCore.SendMessage(tv, PInvoke.TVM_GETITEMRECT, 0, ref rc) == 0)
            {
                // This means the node is not visible
                return Rectangle.Empty;
            }

            return rc;
        }
    }

    internal bool CheckedStateInternal
    {
        get => _treeNodeState[TREENODESTATE_isChecked];
        set => _treeNodeState[TREENODESTATE_isChecked] = value;
    }

    // Checked does sanity checking and fires Before/AfterCheck events, then forwards to this
    // property to get/set the actual checked value.
    internal unsafe bool CheckedInternal
    {
        get => CheckedStateInternal;
        set
        {
            CheckedStateInternal = value;
            if (HTREEITEMInternal == IntPtr.Zero)
            {
                return;
            }

            TreeView? tv = TreeView;
            if (tv is null || !tv.IsHandleCreated || tv.IsDisposed)
            {
                return;
            }

            TVITEMW item = new()
            {
                mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
                hItem = HTREEITEMInternal,
                stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK
            };

            item.state |= value ? CHECKED : UNCHECKED;
            PInvokeCore.SendMessage(tv, PInvoke.TVM_SETITEMW, 0, ref item);
        }
    }

    /// <summary>
    ///  Indicates whether the node's checkbox is checked.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeCheckedDescr))]
    [DefaultValue(false)]
    public bool Checked
    {
        get
        {
#if DEBUG
            if (HTREEITEMInternal != IntPtr.Zero && _treeView is not null && !_treeView.IsDisposed)
            {
                TVITEMW item = new()
                {
                    mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
                    hItem = HTREEITEMInternal,
                    stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK
                };

                PInvokeCore.SendMessage(_treeView, PInvoke.TVM_GETITEMW, 0, ref item);
                Debug.Assert(
                    !_treeView.CheckBoxes || (((int)item.state >> SHIFTVAL) > 1) == CheckedInternal,
                    $"isChecked on node '{Name}' did not match the state in TVM_GETITEM.");
            }
#endif
            return CheckedInternal;
        }
        set
        {
            TreeView? tv = TreeView;
            if (tv is not null)
            {
                bool eventReturn = tv.TreeViewBeforeCheck(this, TreeViewAction.Unknown);
                if (!eventReturn)
                {
                    CheckedInternal = value;
                    tv.TreeViewAfterCheck(this, TreeViewAction.Unknown);
                }
            }
            else
            {
                CheckedInternal = value;
            }
        }
    }

    /// <summary>
    ///  The <see cref="Forms.ContextMenuStrip"/> associated with this tree node. This menu
    ///  will be shown when the user right clicks the mouse on the control.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(null)]
    [SRDescription(nameof(SR.ControlContextMenuDescr))]
    public virtual ContextMenuStrip? ContextMenuStrip
    {
        get => _contextMenuStrip;
        set => _contextMenuStrip = value;
    }

    /// <summary>
    ///  The first child node of this node.
    /// </summary>
    [Browsable(false)]
    public TreeNode? FirstNode => _childCount == 0 ? null : _children[0];

    private TreeNode? FirstVisibleParent
    {
        get
        {
            TreeNode? node = this;
            while (node is not null && node.Bounds.IsEmpty)
            {
                node = node.Parent;
            }

            return node;
        }
    }

    /// <summary>
    ///  The foreground color of this node.
    ///  If null, the color used will be the default color from the TreeView control that this
    ///  node is attached to
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeNodeForeColorDescr))]
    public Color ForeColor
    {
        get
        {
            if (_propBag is null)
            {
                return Color.Empty;
            }

            return _propBag.ForeColor;
        }
        set
        {
            Color oldfc = ForeColor;
            // If we're setting the color to the default again, delete the propBag if it doesn't contain
            // useful data.
            if (value.IsEmpty)
            {
                if (_propBag is not null)
                {
                    _propBag.ForeColor = Color.Empty;
                    RemovePropBagIfEmpty();
                }

                if (!oldfc.IsEmpty)
                {
                    InvalidateHostTree();
                }

                return;
            }

            // Not the default, so if necessary create a new propBag, and fill it with the new forecolor

            _propBag ??= new OwnerDrawPropertyBag();

            _propBag.ForeColor = value;
            if (!value.Equals(oldfc))
            {
                InvalidateHostTree();
            }
        }
    }

    /// <summary>
    ///  Returns the full path of this node.
    ///  The path consists of the labels of each of the nodes from the root to this node,
    ///  each separated by the pathSeparator.
    /// </summary>
    [Browsable(false)]
    public string FullPath
    {
        get
        {
            TreeView? tv = TreeView;
            if (tv is not null)
            {
                StringBuilder path = new();
                GetFullPath(path, tv.PathSeparator);
                return path.ToString();
            }
            else
            {
                throw new InvalidOperationException(SR.TreeNodeNoParent);
            }
        }
    }

    /// <summary>
    ///  The HTREEITEM handle associated with this node. If the handle
    ///  has not yet been created, this will force handle creation.
    /// </summary>
    [Browsable(false)]
    public IntPtr Handle => (nint)HTREEITEM;

    internal HTREEITEM HTREEITEMInternal { get; private set; }
    internal HTREEITEM HTREEITEM
    {
        get
        {
            if (HTREEITEMInternal == IntPtr.Zero && TreeView is not null)
            {
                TreeView.CreateControl(); // force handle creation
            }

            return HTREEITEMInternal;
        }
    }

    /// <summary>
    ///  The index of the image to be displayed when the node is in the unselected state.
    ///  The image is contained in the ImageList referenced by the imageList property.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeImageIndexDescr))]
    [TypeConverter(typeof(TreeViewImageIndexConverter))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [RelatedImageList("TreeView.ImageList")]
    public int ImageIndex
    {
        get
        {
            TreeView? tv = TreeView;
            if (ImageIndexer.Index != ImageList.Indexer.NoneIndex
                && ImageIndexer.Index != ImageList.Indexer.DefaultIndex
                && tv?.ImageList is not null
                && ImageIndexer.Index >= tv.ImageList.Images.Count)
            {
                return tv.ImageList.Images.Count - 1;
            }

            return ImageIndexer.Index;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.NoneIndex);

            if (ImageIndexer.Index == value
                && value != ImageList.Indexer.NoneIndex
                && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            ImageIndexer.Index = value;
            UpdateNode(TVITEM_MASK.TVIF_IMAGE);
        }
    }

    /// <summary>
    ///  The index of the image to be displayed when the node is in the unselected state.
    ///  The image is contained in the ImageList referenced by the imageList property.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeImageKeyDescr))]
    [TypeConverter(typeof(TreeViewImageKeyConverter))]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [RelatedImageList("TreeView.ImageList")]
    [AllowNull]
    public string ImageKey
    {
        get => ImageIndexer.Key;
        set
        {
            if (value == ImageIndexer.Key && !string.Equals(value, ImageList.Indexer.DefaultKey))
            {
                return;
            }

            ImageIndexer.Key = value;
            UpdateNode(TVITEM_MASK.TVIF_IMAGE);
        }
    }

    /// <summary>
    ///  Returns the position of this node in relation to its siblings
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeIndexDescr))]
    public int Index => _index;

    /// <summary>
    ///  Specifies whether this node is being edited by the user.
    /// </summary>
    [Browsable(false)]
    public bool IsEditing
    {
        get
        {
            TreeView? tv = TreeView;

            if (tv is not null)
            {
                return tv._editNode == this;
            }

            return false;
        }
    }

    /// <summary>
    ///  Specifies whether this node is in the expanded state.
    /// </summary>
    [Browsable(false)]
    public bool IsExpanded => HTREEITEMInternal == 0
        ? _expandOnRealization
        : (State & TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED) != 0;

    /// <summary>
    ///  Specifies whether this node is in the selected state.
    /// </summary>
    [Browsable(false)]
    public bool IsSelected => HTREEITEMInternal != 0
        && (State & TREE_VIEW_ITEM_STATE_FLAGS.TVIS_SELECTED) != 0;

    /// <summary>
    ///  Specifies whether this node is visible.
    /// </summary>
    [Browsable(false)]
    public bool IsVisible
    {
        get
        {
            if (HTREEITEMInternal == IntPtr.Zero)
            {
                return false;
            }

            TreeView tv = TreeView!;
            if (tv.IsDisposed)
            {
                return false;
            }

            RECT rc = default;
            unsafe
            { *((IntPtr*)&rc.left) = Handle; }

            bool visible = PInvokeCore.SendMessage(tv, PInvoke.TVM_GETITEMRECT, 1, ref rc) != 0;
            if (visible)
            {
                Size size = tv.ClientSize;
                visible = (rc.bottom > 0 && rc.right > 0 && rc.top < size.Height && rc.left < size.Width);
            }

            return visible;
        }
    }

    /// <summary>
    ///  The last child node of this node.
    /// </summary>
    [Browsable(false)]
    public TreeNode? LastNode
    {
        get
        {
            if (_childCount == 0)
            {
                return null;
            }

            return _children[_childCount - 1];
        }
    }

    /// <summary>
    ///  This denotes the depth of nesting of the TreeNode.
    /// </summary>
    [Browsable(false)]
    public int Level => Parent is null ? 0 : Parent.Level + 1;

    /// <summary>
    ///  The next sibling node.
    /// </summary>
    [Browsable(false)]
    public TreeNode? NextNode
    {
        get
        {
            if (_parent is not null && _index + 1 < _parent.Nodes.Count)
            {
                return _parent.Nodes[_index + 1];
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    ///  The next visible node. It may be a child, sibling,
    ///  or a node from another branch.
    /// </summary>
    [Browsable(false)]
    public TreeNode? NextVisibleNode
    {
        get
        {
            // TVGN_NEXTVISIBLE can only be sent if the specified node is visible.
            // So before sending, we check if this node is visible. If not, we find the first visible parent.
            TreeView? tv = TreeView;
            if (tv is null || tv.IsDisposed)
            {
                return null;
            }

            TreeNode? node = FirstVisibleParent;

            if (node is not null)
            {
                LRESULT next = PInvokeCore.SendMessage(
                    tv,
                    PInvoke.TVM_GETNEXTITEM,
                    (WPARAM)PInvoke.TVGN_NEXTVISIBLE,
                    (LPARAM)node.Handle);

                if (next != 0)
                {
                    return tv.NodeFromHandle(next);
                }
            }

            return null;
        }
    }

    /// <summary>
    ///  The font that will be used to draw this node
    ///  If null, the font used will be the default font from the TreeView control that this
    ///  node is attached to.
    ///  NOTE: If the node font is larger than the default font from the TreeView control, then
    ///  the node will be clipped.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeNodeNodeFontDescr))]
    [DefaultValue(null)]
    public Font? NodeFont
    {
        get
        {
            if (_propBag is null)
            {
                return null;
            }

            return _propBag.Font;
        }
        set
        {
            Font? oldfont = NodeFont;

            // If we're setting the font to the default again, delete the propBag if it doesn't contain
            // useful data.
            if (value is null)
            {
                if (_propBag is not null)
                {
                    _propBag.Font = null;
                    RemovePropBagIfEmpty();
                }

                if (oldfont is not null)
                {
                    InvalidateHostTree();
                }

                return;
            }

            // Not the default, so if necessary create a new propBag, and fill it with the font

            _propBag ??= new OwnerDrawPropertyBag();

            _propBag.Font = value;
            if (!value.Equals(oldfont))
            {
                InvalidateHostTree();
            }
        }
    }

    [ListBindable(false)]
    [Browsable(false)]
    public TreeNodeCollection Nodes
    {
        get
        {
            _nodes ??= new TreeNodeCollection(this);

            return _nodes;
        }
    }

    /// <summary>
    ///  Retrieves parent node.
    /// </summary>
    [Browsable(false)]
    public TreeNode? Parent
    {
        get
        {
            TreeView? tv = TreeView;

            // Don't expose the virtual root publicly
            if (tv is not null && _parent == tv._root)
            {
                return null;
            }

            return _parent;
        }
    }

    /// <summary>
    ///  The previous sibling node.
    /// </summary>
    [Browsable(false)]
    public TreeNode? PrevNode
    {
        get
        {
            if (_parent is null)
            {
                return null;
            }

            // fixedIndex is used for perf. optimization in case of adding big ranges of nodes
            int currentInd = _index;
            int fixedInd = _parent.Nodes.FixedIndex;

            if (fixedInd > 0)
            {
                currentInd = fixedInd;
            }

            if (currentInd > 0 && currentInd <= _parent.Nodes.Count)
            {
                return _parent.Nodes[currentInd - 1];
            }
            else
            {
                return null;
            }
        }
    }

    /// <summary>
    ///  The next visible node. It may be a parent, sibling,
    ///  or a node from another branch.
    /// </summary>
    [Browsable(false)]
    public TreeNode? PrevVisibleNode
    {
        get
        {
            // TVGN_PREVIOUSVISIBLE can only be sent if the specified node is visible.
            // So before sending, we check if this node is visible. If not, we find the first visible parent.
            TreeNode? node = FirstVisibleParent;
            TreeView? tv = TreeView;

            if (node is not null)
            {
                if (tv is null || tv.IsDisposed)
                {
                    return null;
                }

                LRESULT prev = PInvokeCore.SendMessage(
                    tv,
                    PInvoke.TVM_GETNEXTITEM,
                    (WPARAM)PInvoke.TVGN_PREVIOUSVISIBLE,
                    (LPARAM)node.Handle);

                if (prev != 0)
                {
                    return tv.NodeFromHandle(prev);
                }
            }

            return null;
        }
    }

    /// <summary>
    ///  The index of the image displayed when the node is in the selected state.
    ///  The image is contained in the ImageList referenced by the imageList property.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeSelectedImageIndexDescr))]
    [TypeConverter(typeof(TreeViewImageIndexConverter))]
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RelatedImageList("TreeView.ImageList")]
    public int SelectedImageIndex
    {
        get
        {
            TreeView? tv = TreeView;
            if (SelectedImageIndexer.Index != ImageList.Indexer.NoneIndex
                && SelectedImageIndexer.Index != ImageList.Indexer.DefaultIndex
                && tv?.ImageList is not null
                && SelectedImageIndexer.Index >= tv.ImageList.Images.Count)
            {
                return tv.ImageList.Images.Count - 1;
            }

            return SelectedImageIndexer.Index;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.NoneIndex);

            if (SelectedImageIndexer.Index == value
                && value != ImageList.Indexer.NoneIndex
                && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            SelectedImageIndexer.Index = value;
            UpdateNode(TVITEM_MASK.TVIF_SELECTEDIMAGE);
        }
    }

    /// <summary>
    ///  The index of the image displayed when the node is in the selected state.
    ///  The image is contained in the ImageList referenced by the imageList property.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeSelectedImageKeyDescr))]
    [TypeConverter(typeof(TreeViewImageKeyConverter))]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [RefreshProperties(RefreshProperties.Repaint)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RelatedImageList("TreeView.ImageList")]
    [AllowNull]
    public string SelectedImageKey
    {
        get => SelectedImageIndexer.Key;
        set
        {
            if (SelectedImageIndexer.Key == value && !string.Equals(value, ImageList.Indexer.DefaultKey))
            {
                return;
            }

            SelectedImageIndexer.Key = value;
            UpdateNode(TVITEM_MASK.TVIF_SELECTEDIMAGE);
        }
    }

    /// <summary>
    ///  Retrieve state bits for this node
    /// </summary>
    internal TREE_VIEW_ITEM_STATE_FLAGS State
    {
        get
        {
            if (HTREEITEMInternal == IntPtr.Zero)
            {
                return 0;
            }

            TreeView? tv = TreeView;
            if (tv is null || tv.IsDisposed)
            {
                return 0;
            }

            TVITEMW item = new()
            {
                hItem = HTREEITEM,
                mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
                stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_SELECTED | TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED
            };

            PInvokeCore.SendMessage(tv, PInvoke.TVM_GETITEMW, 0, ref item);
            return item.state;
        }
    }

    /// <summary>
    ///  The key of the StateImage that the user want to display.
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeStateImageKeyDescr))]
    [TypeConverter(typeof(ImageKeyConverter))]
    [DefaultValue(ImageList.Indexer.DefaultKey)]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [RelatedImageList("TreeView.StateImageList")]
    [AllowNull]
    public string StateImageKey
    {
        get => StateImageIndexer.Key;
        set
        {
            if (StateImageIndexer.Key == value && !string.Equals(value, ImageList.Indexer.DefaultKey))
            {
                return;
            }

            StateImageIndexer.Key = value;
            if (_treeView is not null && !_treeView.CheckBoxes)
            {
                UpdateNode(TVITEM_MASK.TVIF_STATE);
            }
        }
    }

    [Localizable(true)]
    [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
    [DefaultValue(ImageList.Indexer.DefaultIndex)]
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.TreeNodeStateImageIndexDescr))]
    [Editor($"System.Windows.Forms.Design.ImageIndexEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [RefreshProperties(RefreshProperties.Repaint)]
    [RelatedImageList("TreeView.StateImageList")]
    public int StateImageIndex
    {
        get
        {
            TreeView? tv = TreeView;
            if (StateImageIndexer.Index != ImageList.Indexer.DefaultIndex &&
                tv?.StateImageList is not null &&
                StateImageIndexer.Index >= tv.StateImageList.Images.Count)
            {
                return tv.StateImageList.Images.Count - 1;
            }

            return StateImageIndexer.Index;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfLessThan(value, ImageList.Indexer.DefaultIndex);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, ALLOWEDIMAGES);

            if (StateImageIndexer.Index == value && value != ImageList.Indexer.DefaultIndex)
            {
                return;
            }

            StateImageIndexer.Index = value;
            if (_treeView is not null && !_treeView.CheckBoxes)
            {
                UpdateNode(TVITEM_MASK.TVIF_STATE);
            }
        }
    }

    [SRCategory(nameof(SR.CatData))]
    [Localizable(false)]
    [Bindable(true)]
    [SRDescription(nameof(SR.ControlTagDescr))]
    [DefaultValue(null)]
    [TypeConverter(typeof(StringConverter))]
    public object? Tag
    {
        get => _userData;
        set => _userData = value;
    }

    /// <summary>
    ///  The label text for the tree node
    /// </summary>
    [Localizable(true)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeNodeTextDescr))]
    [AllowNull]
    public string Text
    {
        get => _text ?? string.Empty;
        set
        {
            _text = value;
            UpdateNode(TVITEM_MASK.TVIF_TEXT);
        }
    }

    /// <summary>
    ///  The ToolTip text that will be displayed when the mouse hovers over the node.
    /// </summary>
    [Localizable(false)]
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeNodeToolTipTextDescr))]
    [DefaultValue("")]
    public string ToolTipText
    {
        get => _toolTipText;
        set => _toolTipText = value;
    }

    /// <summary>
    ///  The name for the tree node - useful for indexing.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [SRDescription(nameof(SR.TreeNodeNodeNameDescr))]
    [AllowNull]
    public string Name
    {
        get => _name ?? string.Empty;
        set => _name = value;
    }

    /// <summary>
    ///  Return the TreeView control this node belongs to.
    /// </summary>
    [Browsable(false)]
    public TreeView? TreeView
    {
        get
        {
            _treeView ??= FindTreeView();

            return _treeView;
        }
    }

    internal TreeNodeAccessibleObject? AccessibilityObject =>
        _accessibleObject ??= TreeView is null
            ? null
            : new TreeNodeAccessibleObject(this, TreeView);

    /// <summary>
    ///  Adds a new child node at the appropriate sorted position
    /// </summary>
    internal int AddSorted(TreeView parentTreeView, TreeNode node)
    {
        int index = 0;
        int iMin;
        int iLim;
        int iT;
        string nodeText = node.Text;

        if (_childCount > 0)
        {
            if (parentTreeView.TreeViewNodeSorter is null)
            {
                CompareInfo compare = Application.CurrentCulture.CompareInfo;

                // Optimize for the case where they're already sorted
                if (compare.Compare(_children[_childCount - 1].Text, nodeText) <= 0)
                {
                    index = _childCount;
                }
                else
                {
                    // Insert at appropriate sorted spot
                    for (iMin = 0, iLim = _childCount; iMin < iLim;)
                    {
                        iT = (iMin + iLim) / 2;
                        if (compare.Compare(_children[iT].Text, nodeText) <= 0)
                        {
                            iMin = iT + 1;
                        }
                        else
                        {
                            iLim = iT;
                        }
                    }

                    index = iMin;
                }
            }
            else
            {
                IComparer sorter = parentTreeView.TreeViewNodeSorter;
                // Insert at appropriate sorted spot
                for (iMin = 0, iLim = _childCount; iMin < iLim;)
                {
                    iT = (iMin + iLim) / 2;
                    if (sorter.Compare(_children[iT] /*previous*/, node/*current*/) <= 0)
                    {
                        iMin = iT + 1;
                    }
                    else
                    {
                        iLim = iT;
                    }
                }

                index = iMin;
            }
        }

        node.SortChildren(parentTreeView);
        InsertNodeAt(index, node);

        return index;
    }

    /// <summary>
    ///  Returns a TreeNode object for the given HTREEITEM handle
    /// </summary>
    public static TreeNode? FromHandle(TreeView tree, IntPtr handle) => tree.NodeFromHandle(handle);

    private void SortChildren(TreeView? parentTreeView)
    {
        if (_childCount <= 0)
        {
            return;
        }

        TreeNode[] newOrder = new TreeNode[_childCount];
        if (parentTreeView is null || parentTreeView.TreeViewNodeSorter is null)
        {
            CompareInfo compare = Application.CurrentCulture.CompareInfo;
            for (int i = 0; i < _childCount; i++)
            {
                int min = -1;
                for (int j = 0; j < _childCount; j++)
                {
                    if (_children[j] is null)
                    {
                        continue;
                    }

                    if (min == -1)
                    {
                        min = j;
                        continue;
                    }

                    if (compare.Compare(_children[j].Text, _children[min].Text) <= 0)
                    {
                        min = j;
                    }
                }

                Debug.Assert(min != -1, "Bad sorting");
                newOrder[i] = _children[min];
                _children[min] = null!;
                newOrder[i]._index = i;
                newOrder[i].SortChildren(parentTreeView);
            }

            _children = newOrder;
        }
        else
        {
            IComparer sorter = parentTreeView.TreeViewNodeSorter;
            for (int i = 0; i < _childCount; i++)
            {
                int min = -1;
                for (int j = 0; j < _childCount; j++)
                {
                    if (_children[j] is null)
                    {
                        continue;
                    }

                    if (min == -1)
                    {
                        min = j;
                        continue;
                    }

                    if (sorter.Compare(_children[j] /*previous*/, _children[min] /*current*/) <= 0)
                    {
                        min = j;
                    }
                }

                Debug.Assert(min != -1, "Bad sorting");
                newOrder[i] = _children[min];
                _children[min] = null!;
                newOrder[i]._index = i;
                newOrder[i].SortChildren(parentTreeView);
            }

            _children = newOrder;
        }
    }

    /// <summary>
    ///  Initiate editing of the node's label.
    ///  Only effective if LabelEdit property is true.
    /// </summary>
    public void BeginEdit()
    {
        if (HTREEITEMInternal != IntPtr.Zero)
        {
            TreeView tv = TreeView!;
            if (!tv.LabelEdit)
            {
                throw new InvalidOperationException(SR.TreeNodeBeginEditFailed);
            }

            if (!tv.Focused)
            {
                tv.Focus();
            }

            PInvokeCore.SendMessage(tv, PInvoke.TVM_EDITLABELW, 0, (LPARAM)HTREEITEMInternal);
        }
    }

    /// <summary>
    ///  Called by the tree node collection to clear all nodes. We optimize here if
    ///  this is the root node.
    /// </summary>
    internal void Clear()
    {
        // This is a node that is a child of some other node. We have
        // to selectively remove children here.
        bool isBulkOperation = false;
        TreeView? tv = TreeView;

        try
        {
            if (tv is not null)
            {
                tv._nodesCollectionClear = true;

                if (_childCount > MAX_TREENODES_OPS)
                {
                    isBulkOperation = true;
                    tv.BeginUpdate();
                }
            }

            while (_childCount > 0)
            {
                _children[_childCount - 1].Remove(true);
            }

            _children = [];

            if (tv is not null && isBulkOperation)
            {
                tv.EndUpdate();
            }
        }
        finally
        {
            if (tv is not null)
            {
                tv._nodesCollectionClear = false;
            }

            _nodesCleared = true;
        }
    }

    /// <summary>
    ///  Clone the entire subtree rooted at this node.
    /// </summary>
    public virtual object Clone()
    {
        Type clonedType = GetType();

        TreeNode node = clonedType == typeof(TreeNode)
            ? new TreeNode(_text, ImageIndexer.Index, SelectedImageIndexer.Index)
            : (TreeNode)Activator.CreateInstance(clonedType)!;

        node.Text = _text;
        node.Name = _name;
        node.ImageIndexer.Index = ImageIndexer.Index;
        node.SelectedImageIndexer.Index = SelectedImageIndexer.Index;

        node.StateImageIndexer.Index = StateImageIndexer.Index;
        node.ToolTipText = _toolTipText;
        node.ContextMenuStrip = _contextMenuStrip;

        // only set the key if it's set to something useful
        if (!(string.IsNullOrEmpty(ImageIndexer.Key)))
        {
            node.ImageIndexer.Key = ImageIndexer.Key;
        }

        // only set the key if it's set to something useful
        if (!(string.IsNullOrEmpty(SelectedImageIndexer.Key)))
        {
            node.SelectedImageIndexer.Key = SelectedImageIndexer.Key;
        }

        // only set the key if it's set to something useful
        if (!(string.IsNullOrEmpty(StateImageIndexer.Key)))
        {
            node.StateImageIndexer.Key = StateImageIndexer.Key;
        }

        if (_childCount > 0)
        {
            node._children = new TreeNode[_childCount];
            for (int i = 0; i < _childCount; i++)
            {
                node.Nodes.Add((TreeNode)_children[i].Clone());
            }
        }

        // Clone properties
        //
        if (_propBag is not null)
        {
            node._propBag = OwnerDrawPropertyBag.Copy(_propBag);
        }

        node.Checked = Checked;
        node.Tag = Tag;

        return node;
    }

    private void CollapseInternal(bool ignoreChildren)
    {
        TreeView? tv = TreeView;
        bool setSelection = false;
        _collapseOnRealization = false;
        _expandOnRealization = false;

        if (tv is null || !tv.IsHandleCreated)
        {
            _collapseOnRealization = true;
            return;
        }

        // terminating condition for recursion...
        //
        if (ignoreChildren)
        {
            DoCollapse(tv);
        }
        else
        {
            if (!ignoreChildren && _childCount > 0)
            {
                // Virtual root should collapse all its children
                for (int i = 0; i < _childCount; i++)
                {
                    if (tv.SelectedNode == _children[i])
                    {
                        setSelection = true;
                    }

                    _children[i].DoCollapse(tv);
                    _children[i].Collapse();
                }
            }

            DoCollapse(tv);
        }

        if (setSelection)
        {
            tv.SelectedNode = this;
        }

        tv.Invalidate();
        _collapseOnRealization = false;
    }

    /// <summary>
    ///  Collapse the node ignoring its children while collapsing the parent
    /// </summary>
    public void Collapse(bool ignoreChildren)
    {
        CollapseInternal(ignoreChildren);
    }

    /// <summary>
    ///  Collapse the node.
    /// </summary>
    public void Collapse()
    {
        CollapseInternal(false);
    }

    /// <summary>
    ///  Windows TreeView doesn't send the proper notifications on collapse, so we do it manually.
    /// </summary>
    private void DoCollapse(TreeView tv)
    {
        if ((State & TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDED) != 0)
        {
            TreeViewCancelEventArgs e = new(this, false, TreeViewAction.Collapse);
            tv.OnBeforeCollapse(e);
            if (!e.Cancel)
            {
                PInvokeCore.SendMessage(tv, PInvoke.TVM_EXPAND, (WPARAM)(uint)NM_TREEVIEW_ACTION.TVE_COLLAPSE, (LPARAM)Handle);
                tv.OnAfterCollapse(new TreeViewEventArgs(this));
            }
        }
    }

    protected virtual void Deserialize(SerializationInfo serializationInfo, StreamingContext context)
    {
        int childCount = 0;
        int imageIndex = ImageList.Indexer.DefaultIndex;
        string? imageKey = null;

        int selectedImageIndex = ImageList.Indexer.DefaultIndex;
        string? selectedImageKey = null;

        int stateImageIndex = ImageList.Indexer.DefaultIndex;
        string? stateImageKey = null;

        foreach (SerializationEntry entry in serializationInfo)
        {
            switch (entry.Name)
            {
                case "PropBag":
                    // this would throw a InvalidCastException if improper cast, thus validating the serializationInfo for OwnerDrawPropertyBag
                    _propBag = (OwnerDrawPropertyBag?)serializationInfo.GetValue(entry.Name, typeof(OwnerDrawPropertyBag));
                    break;
                case nameof(Text):
                    Text = serializationInfo.GetString(entry.Name);
                    break;
                case nameof(ToolTipText):
                    ToolTipText = serializationInfo.GetString(entry.Name)!;
                    break;
                case nameof(Name):
                    Name = serializationInfo.GetString(entry.Name);
                    break;
                case "IsChecked":
                    CheckedStateInternal = serializationInfo.GetBoolean(entry.Name);
                    break;
                case nameof(ImageIndex):
                    imageIndex = serializationInfo.GetInt32(entry.Name);
                    break;
                case nameof(SelectedImageIndex):
                    selectedImageIndex = serializationInfo.GetInt32(entry.Name);
                    break;
                case nameof(ImageKey):
                    imageKey = serializationInfo.GetString(entry.Name);
                    break;
                case nameof(SelectedImageKey):
                    selectedImageKey = serializationInfo.GetString(entry.Name);
                    break;
                case nameof(StateImageKey):
                    stateImageKey = serializationInfo.GetString(entry.Name);
                    break;
                case "StateImageIndex":
                    stateImageIndex = serializationInfo.GetInt32(entry.Name);
                    break;
                case "ChildCount":
                    childCount = serializationInfo.GetInt32(entry.Name);
                    break;
                case "UserData":
                    _userData = entry.Value;
                    break;
            }
        }

        // let imagekey take precedence
        if (imageKey is not null)
        {
            ImageKey = imageKey;
        }
        else if (imageIndex != ImageList.Indexer.DefaultIndex)
        {
            ImageIndex = imageIndex;
        }

        // let selectedimagekey take precedence
        if (selectedImageKey is not null)
        {
            SelectedImageKey = selectedImageKey;
        }
        else if (selectedImageIndex != ImageList.Indexer.DefaultIndex)
        {
            SelectedImageIndex = selectedImageIndex;
        }

        // let stateimagekey take precedence
        if (stateImageKey is not null)
        {
            StateImageKey = stateImageKey;
        }
        else if (stateImageIndex != ImageList.Indexer.DefaultIndex)
        {
            StateImageIndex = stateImageIndex;
        }

        if (childCount > 0)
        {
            TreeNode[] childNodes = new TreeNode[childCount];

            for (int i = 0; i < childCount; i++)
            {
                childNodes[i] = (TreeNode)serializationInfo.GetValue($"children{i}", typeof(TreeNode))!;
            }

            Nodes.AddRange(childNodes);
        }
    }

    /// <summary>
    ///  Terminate the editing of any tree view item's label.
    /// </summary>
    public void EndEdit(bool cancel)
    {
        TreeView? tv = TreeView;
        if (tv is null || tv.IsDisposed)
        {
            return;
        }

        PInvokeCore.SendMessage(tv, PInvoke.TVM_ENDEDITLABELNOW, (WPARAM)(BOOL)cancel);
    }

    /// <summary>
    ///  Makes sure there is enough room to add <paramref name="num" /> children.
    /// </summary>
    internal void EnsureCapacity(int num)
    {
        Debug.Assert(num > 0, "required capacity can not be less than 1");
        int size = num;
        if (size < 4)
        {
            size = 4;
        }

        if (_children is null || _children.Length == 0)
        {
            _children = new TreeNode[size];
        }
        else if (_childCount + num > _children.Length)
        {
            int newSize = _childCount + num;
            if (num == 1)
            {
                newSize = _childCount * 2;
            }

            TreeNode[] bigger = new TreeNode[newSize];
            Array.Copy(_children, 0, bigger, 0, _childCount);
            _children = bigger;
        }
    }

    /// <summary>
    ///  Ensures the node's StateImageIndex value is properly set.
    /// </summary>
    private void EnsureStateImageValue()
    {
        if (_treeView is null)
        {
            return;
        }

        if (_treeView.CheckBoxes && _treeView.StateImageList is not null)
        {
            if (!string.IsNullOrEmpty(StateImageKey))
            {
                StateImageIndex = (Checked) ? 1 : 0;
                StateImageKey = _treeView.StateImageList.Images.Keys[StateImageIndex];
            }
            else
            {
                StateImageIndex = (Checked) ? 1 : 0;
            }
        }
    }

    /// <summary>
    ///  Ensure that the node is visible, expanding nodes and scrolling the
    ///  TreeView control as necessary.
    /// </summary>
    public void EnsureVisible()
    {
        TreeView? tv = TreeView;
        if (tv is null || tv.IsDisposed)
        {
            return;
        }

        PInvokeCore.SendMessage(tv, PInvoke.TVM_ENSUREVISIBLE, 0, Handle);
    }

    /// <summary>
    ///  Expand the node.
    /// </summary>
    public void Expand()
    {
        TreeView? tv = TreeView;
        if (tv is null || !tv.IsHandleCreated)
        {
            _expandOnRealization = true;
            return;
        }

        ResetExpandedState(tv);
        if (!IsExpanded)
        {
            PInvokeCore.SendMessage(tv, PInvoke.TVM_EXPAND, (WPARAM)(uint)NM_TREEVIEW_ACTION.TVE_EXPAND, (LPARAM)Handle);
        }

        _expandOnRealization = false;
    }

    /// <summary>
    ///  Expand the node.
    /// </summary>
    public void ExpandAll()
    {
        Expand();
        for (int i = 0; i < _childCount; i++)
        {
            _children[i].ExpandAll();
        }
    }

    /// <summary>
    ///  Locate this tree node's containing tree view control by scanning
    ///  up to the virtual root, whose treeView pointer we know to be
    ///  correct
    /// </summary>
    internal TreeView? FindTreeView()
    {
        TreeNode node = this;
        while (node._parent is not null)
        {
            node = node._parent;
        }

        return node._treeView;
    }

    internal List<TreeNode> GetSelfAndChildNodes()
    {
        List<TreeNode> nodes = [this];
        AggregateChildNodesToList(this);
        return nodes;

        void AggregateChildNodesToList(TreeNode parentNode)
        {
            foreach (TreeNode child in parentNode.Nodes)
            {
                nodes.Add(child);
                AggregateChildNodesToList(child);
            }
        }
    }

    /// <summary>
    ///  Helper function for getFullPath().
    /// </summary>
    private void GetFullPath(StringBuilder path, string pathSeparator)
    {
        if (_parent is not null)
        {
            _parent.GetFullPath(path, pathSeparator);
            if (_parent._parent is not null)
            {
                path.Append(pathSeparator);
            }

            path.Append(_text);
        }
    }

    /// <summary>
    ///  Returns number of child nodes.
    /// </summary>
    public int GetNodeCount(bool includeSubTrees)
    {
        int total = _childCount;
        if (includeSubTrees)
        {
            for (int i = 0; i < _childCount; i++)
            {
                total += _children[i].GetNodeCount(true);
            }
        }

        return total;
    }

    /// <summary>
    ///  Check for any circular reference in the ancestors chain.
    /// </summary>
    internal void CheckParentingCycle(TreeNode candidateToAdd)
    {
        TreeNode? node = this;

        while (node is not null)
        {
            if (node == candidateToAdd)
            {
                throw new ArgumentException(SR.TreeNodeCircularReference);
            }

            node = node._parent;
        }
    }

    /// <summary>
    ///  Helper function to add node at a given index after all validation has been done
    /// </summary>
    internal void InsertNodeAt(int index, TreeNode node)
    {
        EnsureCapacity(1);
        node._parent = this;
        node._index = index;
        for (int i = _childCount; i > index; --i)
        {
            (_children[i] = _children[i - 1])._index = i;
        }

        _children[index] = node;
        _childCount++;
        node.Realize(false);

        if (TreeView is not null && node == TreeView._selectedNode)
        {
            TreeView.SelectedNode = node; // communicate this to the handle
        }
    }

    /// <summary>
    ///  Invalidates the treeview control that is hosting this node
    /// </summary>
    private void InvalidateHostTree()
    {
        if (_treeView is not null && _treeView.IsHandleCreated)
        {
            _treeView.Invalidate();
        }
    }

    internal unsafe void Realize(bool insertFirst)
    {
        TreeView? tv = TreeView;
        if (tv is null || !tv.IsHandleCreated || tv.IsDisposed)
        {
            return;
        }

        if (_parent is not null)
        {
            // Never realize the virtual root
            if (tv.InvokeRequired)
            {
                throw new InvalidOperationException(SR.InvalidCrossThreadControlCall);
            }

            TVINSERTSTRUCTW tvis = new()
            {
                hParent = _parent.HTREEITEMInternal
            };

            tvis.item.mask = InsertMask;

            TreeNode? prev = PrevNode;
            tvis.hInsertAfter = insertFirst || prev is null ? HTREEITEM.TVI_FIRST : prev.HTREEITEMInternal;

            tvis.item.pszText = (char*)Marshal.StringToHGlobalUni(_text);
            tvis.item.iImage = (ImageIndexer.ActualIndex == ImageList.Indexer.DefaultIndex) ? tv.ImageIndexer.ActualIndex : ImageIndexer.ActualIndex;
            tvis.item.iSelectedImage = (SelectedImageIndexer.ActualIndex == ImageList.Indexer.DefaultIndex) ? tv.SelectedImageIndexer.ActualIndex : SelectedImageIndexer.ActualIndex;
            tvis.item.mask = TVITEM_MASK.TVIF_TEXT;

            tvis.item.stateMask = 0;
            tvis.item.state = 0;

            if (tv.CheckBoxes)
            {
                tvis.item.mask |= TVITEM_MASK.TVIF_STATE;
                tvis.item.stateMask |= TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK;
                tvis.item.state |= CheckedInternal ? CHECKED : UNCHECKED;
            }
            else if (tv.StateImageList is not null && StateImageIndexer.ActualIndex >= 0)
            {
                tvis.item.mask |= TVITEM_MASK.TVIF_STATE;
                tvis.item.stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK;
                tvis.item.state = (TREE_VIEW_ITEM_STATE_FLAGS)((StateImageIndexer.ActualIndex + 1) << SHIFTVAL);
            }

            if (tvis.item.iImage >= 0)
            {
                tvis.item.mask |= TVITEM_MASK.TVIF_IMAGE;
            }

            if (tvis.item.iSelectedImage >= 0)
            {
                tvis.item.mask |= TVITEM_MASK.TVIF_SELECTEDIMAGE;
            }

            // If you are editing when you add a new node, then the edit control
            // gets placed in the wrong place. You must restore the edit mode
            // asynchronously (PostMessage) after the add is complete
            // to get the expected behavior.
            bool editing = false;
            nint editHandle = PInvokeCore.SendMessage(tv, PInvoke.TVM_GETEDITCONTROL);
            if (editHandle != 0)
            {
                // Currently editing.
                editing = true;
                PInvokeCore.SendMessage(tv, PInvoke.TVM_ENDEDITLABELNOW, (WPARAM)(BOOL)false);
            }

            HTREEITEMInternal = (HTREEITEM)PInvokeCore.SendMessage(tv, PInvoke.TVM_INSERTITEMW, 0, ref tvis);
            tv._nodesByHandle[HTREEITEMInternal] = this;

            // Lets update the Lparam to the Handle.
            UpdateNode(TVITEM_MASK.TVIF_PARAM);

            Marshal.FreeCoTaskMem((nint)tvis.item.pszText.Value);

            if (editing)
            {
                PInvokeCore.PostMessage(tv, PInvoke.TVM_EDITLABELW, default, (LPARAM)HTREEITEMInternal);
            }

            PInvoke.InvalidateRect(tv, lpRect: null, bErase: false);

            if (_parent._nodesCleared && (insertFirst || prev is null) && !tv.Scrollable)
            {
                // We need to Redraw the TreeView ...
                // If and only If we are not scrollable ...
                // and this is the FIRST NODE to get added..
                // This is Comctl quirk where it just doesn't draw
                // the first node after a Clear( ) if Scrollable == false.
                PInvokeCore.SendMessage(tv, PInvokeCore.WM_SETREDRAW, (WPARAM)(BOOL)true);
                _nodesCleared = false;
            }
        }

        for (int i = _childCount - 1; i >= 0; i--)
        {
            _children[i].Realize(true);
        }

        // If node expansion was requested before the handle was created,
        // we can expand it now.
        if (_expandOnRealization)
        {
            Expand();
        }

        // If node collapse was requested before the handle was created,
        // we can expand it now.
        if (_collapseOnRealization)
        {
            Collapse();
        }
    }

    /// <summary>
    ///  Remove this node from the TreeView control. Child nodes are also removed from the
    ///  TreeView, but are still attached to this node.
    /// </summary>
    public void Remove()
    {
        Remove(true);
    }

    internal void Remove(bool notify)
    {
        bool expanded = IsExpanded;

        // unlink our children
        for (int i = 0; i < _childCount; i++)
        {
            _children[i].Remove(false);
        }

        // children = null;
        // unlink ourself
        if (notify && _parent is not null)
        {
            for (int i = _index; i < _parent._childCount - 1; ++i)
            {
                (_parent._children[i] = _parent._children[i + 1])._index = i;
            }

#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
            _parent._children[_parent._childCount - 1] = null;
#pragma warning restore CS8625
            _parent._childCount--;

            _parent = null;
        }

        // Expand when we are realized the next time.
        _expandOnRealization = expanded;

        // unrealize ourself
        TreeView? tv = TreeView;
        if (tv is null || tv.IsDisposed)
        {
            return;
        }

        KeyboardToolTipStateMachine.Instance.Unhook(this, tv.KeyboardToolTip);

        if (HTREEITEMInternal != IntPtr.Zero)
        {
            if (notify && tv.IsHandleCreated)
            {
                PInvokeCore.SendMessage(tv, PInvoke.TVM_DELETEITEM, 0, (LPARAM)HTREEITEMInternal);
            }

            tv._nodesByHandle.Remove(HTREEITEMInternal);
            HTREEITEMInternal = (HTREEITEM)IntPtr.Zero;
        }

        ReleaseUiaProvider();

        _treeView = null;
    }

    internal virtual void ReleaseUiaProvider()
    {
        PInvoke.UiaDisconnectProvider(_accessibleObject);
        _accessibleObject = null;
    }

    /// <summary>
    ///  Removes the propBag object if it's now devoid of useful data
    /// </summary>
    private void RemovePropBagIfEmpty()
    {
        if (_propBag is null)
        {
            return;
        }

        if (_propBag.IsEmpty())
        {
            _propBag = null;
        }

        return;
    }

    private unsafe void ResetExpandedState(TreeView tv)
    {
        Debug.Assert(tv.IsHandleCreated, "nonexistent handle");

        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_STATE,
            hItem = HTREEITEMInternal,
            stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_EXPANDEDONCE,
            state = 0
        };

        PInvokeCore.SendMessage(tv, PInvoke.TVM_SETITEMW, 0, ref item);
    }

    private bool ShouldSerializeBackColor()
    {
        return BackColor != Color.Empty;
    }

    private bool ShouldSerializeForeColor()
    {
        return ForeColor != Color.Empty;
    }

    /// <summary>
    ///  Saves this TreeNode object to the given data stream.
    /// </summary>
    ///  Review: Changing this would break VB users. so suppressing this message.
    ///
    protected virtual void Serialize(SerializationInfo si, StreamingContext context)
    {
        if (_propBag is not null)
        {
            si.AddValue("PropBag", _propBag, typeof(OwnerDrawPropertyBag));
        }

        si.AddValue(nameof(Text), _text);
        si.AddValue(nameof(ToolTipText), _toolTipText);
        si.AddValue(nameof(Name), Name);
        si.AddValue("IsChecked", _treeNodeState[TREENODESTATE_isChecked]);
        si.AddValue(nameof(ImageIndex), ImageIndexer.Index);
        si.AddValue(nameof(ImageKey), ImageIndexer.Key);
        si.AddValue(nameof(SelectedImageIndex), SelectedImageIndexer.Index);
        si.AddValue(nameof(SelectedImageKey), SelectedImageIndexer.Key);

        if (_treeView is not null && _treeView.StateImageList is not null)
        {
            si.AddValue("StateImageIndex", StateImageIndexer.Index);
        }

        if (_treeView is not null && _treeView.StateImageList is not null)
        {
            si.AddValue(nameof(StateImageKey), StateImageIndexer.Key);
        }

        si.AddValue("ChildCount", _childCount);

        if (_childCount > 0)
        {
            for (int i = 0; i < _childCount; i++)
            {
                si.AddValue($"children{i}", _children[i], typeof(TreeNode));
            }
        }

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        if (_userData is not null && _userData.GetType().IsSerializable)
        {
            si.AddValue("UserData", _userData, _userData.GetType());
        }
#pragma warning restore SYSLIB0050
    }

    /// <summary>
    ///  Toggle the state of the node. Expand if collapsed or collapse if
    ///  expanded.
    /// </summary>
    public void Toggle()
    {
        Debug.Assert(_parent is not null, "toggle on virtual root");

        // I don't use the TVE_TOGGLE message 'cuz Windows TreeView doesn't send the appropriate
        // notifications when collapsing.
        if (IsExpanded)
        {
            Collapse();
        }
        else
        {
            Expand();
        }
    }

    /// <summary>
    ///  Returns the label text for the tree node
    /// </summary>
    public override string ToString()
    {
        return $"TreeNode: {_text ?? ""}";
    }

    /// <summary>
    ///  Tell the TreeView to refresh this node
    /// </summary>
    private unsafe void UpdateNode(TVITEM_MASK mask)
    {
        if (HTREEITEMInternal == IntPtr.Zero)
        {
            return;
        }

        TreeView tv = TreeView!;
        Debug.Assert(tv is not null, "TreeNode has handle but no TreeView");
        if (tv.IsDisposed)
        {
            return;
        }

        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | mask,
            hItem = HTREEITEMInternal
        };

        if ((mask & TVITEM_MASK.TVIF_TEXT) != 0)
        {
            item.pszText = (char*)Marshal.StringToHGlobalUni(_text);
        }

        if ((mask & TVITEM_MASK.TVIF_IMAGE) != 0)
        {
            item.iImage = IsSpecialImageIndex(ImageIndexer.ActualIndex)
                ? tv.ImageIndexer.ActualIndex
                : ImageIndexer.ActualIndex;
        }

        if ((mask & TVITEM_MASK.TVIF_SELECTEDIMAGE) != 0)
        {
            item.iSelectedImage = IsSpecialImageIndex(SelectedImageIndexer.ActualIndex)
                ? tv.SelectedImageIndexer.ActualIndex
                : SelectedImageIndexer.ActualIndex;
        }

        if ((mask & TVITEM_MASK.TVIF_STATE) != 0)
        {
            item.stateMask = TREE_VIEW_ITEM_STATE_FLAGS.TVIS_STATEIMAGEMASK;

            // ActualIndex == -1 means "don't use custom image list"
            // so just leave item.state set to zero, that tells the unmanaged control
            // to use no state image for this node.
            if (StateImageIndexer.ActualIndex != ImageList.Indexer.DefaultIndex)
            {
                item.state = (TREE_VIEW_ITEM_STATE_FLAGS)((StateImageIndexer.ActualIndex + 1) << SHIFTVAL);
            }
        }

        if ((mask & TVITEM_MASK.TVIF_PARAM) != 0)
        {
            item.lParam = (LPARAM)HTREEITEMInternal;
        }

        PInvokeCore.SendMessage(tv, PInvoke.TVM_SETITEMW, 0, ref item);
        if ((mask & TVITEM_MASK.TVIF_TEXT) != 0)
        {
            Marshal.FreeCoTaskMem((nint)item.pszText.Value);
            if (tv.Scrollable)
            {
                tv.ForceScrollbarUpdate(false);
            }
        }

        return;

        static bool IsSpecialImageIndex(int actualIndex)
            => actualIndex is ImageList.Indexer.NoneIndex or ImageList.Indexer.DefaultIndex;
    }

    internal unsafe void UpdateImage()
    {
        TreeView tv = TreeView!;
        if (tv.IsDisposed)
        {
            return;
        }

        TVITEMW item = new()
        {
            mask = TVITEM_MASK.TVIF_HANDLE | TVITEM_MASK.TVIF_IMAGE,
            hItem = HTREEITEM,
            iImage = Math.Max(
                0,
                tv.ImageList is { } imageList && ImageIndexer.ActualIndex >= imageList.Images.Count
                    ? imageList.Images.Count - 1
                    : ImageIndexer.ActualIndex)
        };

        PInvokeCore.SendMessage(tv, PInvoke.TVM_SETITEMW, 0, ref item);
    }

    /// <summary>
    ///  ISerializable private implementation
    /// </summary>
    void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) => Serialize(si, context);
}
