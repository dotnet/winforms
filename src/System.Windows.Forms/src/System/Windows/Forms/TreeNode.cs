// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a node of a <see cref='Forms.TreeView'/>.
    /// </summary>
    [TypeConverterAttribute(typeof(TreeNodeConverter))]
    [Serializable]  // This class participates in resx serialization.
    [DefaultProperty(nameof(Text))]
    public class TreeNode : MarshalByRefObject, ICloneable, ISerializable
    {
        internal const int SHIFTVAL = 12;
        private const TVIS CHECKED = (TVIS)(2 << SHIFTVAL);
        private const TVIS UNCHECKED = (TVIS)(1 << SHIFTVAL);
        private const int ALLOWEDIMAGES = 14;

        //the threshold value used to optimize AddRange and Clear operations for a big number of nodes
        internal const int MAX_TREENODES_OPS = 200;

        // we use it to store font and color data in a minimal-memory-cost manner
        // ie. nodes which don't use fancy fonts or colors (ie. that use the TreeView settings for these)
        //     will take up less memory than those that do.
        internal OwnerDrawPropertyBag propBag;
        internal IntPtr handle;
        internal string text;
        internal string name;

        // note: as the checked state of a node is user controlled, and this variable is simply for
        // state caching when a node hasn't yet been realized, you should use the Checked property to
        // find out the check state of a node, and not this member variable.
        //private bool isChecked = false;
        private const int TREENODESTATE_isChecked = 0x00000001;

        private Collections.Specialized.BitVector32 treeNodeState;

        private TreeNodeImageIndexer imageIndexer;
        private TreeNodeImageIndexer selectedImageIndexer;
        private TreeNodeImageIndexer stateImageIndexer;

        private string toolTipText = string.Empty;
        private ContextMenuStrip contextMenuStrip;
        internal bool nodesCleared;

        // We need a special way to defer to the TreeView's image
        // list for indexing purposes.
        internal class TreeNodeImageIndexer : ImageList.Indexer
        {
            private readonly TreeNode owner;

            public enum ImageListType
            {
                Default,
                State
            }
            private readonly ImageListType imageListType;

            public TreeNodeImageIndexer(TreeNode node, ImageListType imageListType)
            {
                owner = node;
                this.imageListType = imageListType;
            }

            public override ImageList ImageList
            {
                get
                {
                    if (owner.TreeView != null)
                    {
                        if (imageListType == ImageListType.State)
                        {
                            return owner.TreeView.StateImageList;
                        }
                        else
                        {
                            return owner.TreeView.ImageList;
                        }
                    }
                    else
                    {
                        return null;
                    }
                }
                set { Debug.Assert(false, "We should never set the image list"); }
            }
        }

        internal TreeNodeImageIndexer ImageIndexer
        {
            get
            {
                //Demand create the imageIndexer
                if (imageIndexer is null)
                {
                    imageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);
                }
                return imageIndexer;
            }
        }

        internal TreeNodeImageIndexer SelectedImageIndexer
        {
            get
            {
                //Demand create the imageIndexer
                if (selectedImageIndexer is null)
                {
                    selectedImageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);
                }

                return selectedImageIndexer;
            }
        }

        internal TreeNodeImageIndexer StateImageIndexer
        {
            get
            {
                //Demand create the imageIndexer
                if (stateImageIndexer is null)
                {
                    stateImageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.State);
                }
                return stateImageIndexer;
            }
        }

        internal int index;                  // our index into our parents child array
        internal int childCount;
        internal TreeNode[] children;
        internal TreeNode parent;
        internal TreeView treeView;
        private bool expandOnRealization;
        private bool collapseOnRealization;
        private TreeNodeCollection nodes;
        object userData;

        private const TVIF InsertMask =
            TVIF.TEXT
            | TVIF.IMAGE
            | TVIF.SELECTEDIMAGE;

        /// <summary>
        ///  Creates a TreeNode object.
        /// </summary>
        public TreeNode()
        {
            treeNodeState = new Collections.Specialized.BitVector32();
        }

        internal TreeNode(TreeView treeView) : this()
        {
            this.treeView = treeView;
        }

        /// <summary>
        ///  Creates a TreeNode object.
        /// </summary>
        public TreeNode(string text) : this()
        {
            this.text = text;
        }

        /// <summary>
        ///  Creates a TreeNode object.
        /// </summary>
        public TreeNode(string text, TreeNode[] children) : this(text)
        {
            Nodes.AddRange(children);
        }

        /// <summary>
        ///  Creates a TreeNode object.
        /// </summary>
        public TreeNode(string text, int imageIndex, int selectedImageIndex) : this(text)
        {
            ImageIndex = imageIndex;
            SelectedImageIndex = selectedImageIndex;
        }

        /// <summary>
        ///  Creates a TreeNode object.
        /// </summary>
        public TreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children) : this(text, imageIndex, selectedImageIndex)
        {
            Nodes.AddRange(children);
        }

        /**
         * Constructor used in deserialization
         */
        protected TreeNode(SerializationInfo serializationInfo, StreamingContext context) : this()
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
                if (propBag is null)
                {
                    return Color.Empty;
                }

                return propBag.BackColor;
            }
            set
            {
                // get the old value
                Color oldbk = BackColor;
                // If we're setting the color to the default again, delete the propBag if it doesn't contain
                // useful data.
                if (value.IsEmpty)
                {
                    if (propBag != null)
                    {
                        propBag.BackColor = Color.Empty;
                        RemovePropBagIfEmpty();
                    }
                    if (!oldbk.IsEmpty)
                    {
                        InvalidateHostTree();
                    }

                    return;
                }

                // Not the default, so if necessary create a new propBag, and fill it with the backcolor

                if (propBag is null)
                {
                    propBag = new OwnerDrawPropertyBag();
                }

                propBag.BackColor = value;
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
                TreeView tv = TreeView;
                if (tv is null || tv.IsDisposed)
                {
                    return Rectangle.Empty;
                }
                RECT rc = new RECT();
                unsafe
                { *((IntPtr*)&rc.left) = Handle; }
                // wparam: 1=include only text, 0=include entire line
                if ((int)User32.SendMessageW(tv, (User32.WM)TVM.GETITEMRECT, (IntPtr)1, ref rc) == 0)
                {
                    // This means the node is not visible
                    //
                    return Rectangle.Empty;
                }
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
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
                TreeView tv = TreeView;
                RECT rc = new RECT();
                unsafe
                { *((IntPtr*)&rc.left) = Handle; }
                // wparam: 1=include only text, 0=include entire line
                if (tv is null || tv.IsDisposed)
                {
                    return Rectangle.Empty;
                }
                if ((int)User32.SendMessageW(tv, (User32.WM)TVM.GETITEMRECT, IntPtr.Zero, ref rc) == 0)
                {
                    // This means the node is not visible
                    //
                    return Rectangle.Empty;
                }
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }
        }

        internal bool CheckedStateInternal
        {
            get
            {
                return treeNodeState[TREENODESTATE_isChecked];
            }
            set
            {
                treeNodeState[TREENODESTATE_isChecked] = value;
            }
        }

        // Checked does sanity checking and fires Before/AfterCheck events, then forwards to this
        // property to get/set the actual checked value.
        internal unsafe bool CheckedInternal
        {
            get
            {
                return CheckedStateInternal;
            }
            set
            {
                CheckedStateInternal = value;
                if (handle == IntPtr.Zero)
                {
                    return;
                }

                TreeView tv = TreeView;
                if (tv is null || !tv.IsHandleCreated || tv.IsDisposed)
                {
                    return;
                }

                var item = new TVITEMW
                {
                    mask = TVIF.HANDLE | TVIF.STATE,
                    hItem = handle,
                    stateMask = TVIS.STATEIMAGEMASK
                };
                item.state |= value ? CHECKED : UNCHECKED;
                User32.SendMessageW(tv, (User32.WM)TVM.SETITEMW, IntPtr.Zero, ref item);
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
                if (handle != IntPtr.Zero && !treeView.IsDisposed)
                {
                    TreeView tv = TreeView;
                    var item = new TVITEMW
                    {
                        mask = TVIF.HANDLE | TVIF.STATE,
                        hItem = handle,
                        stateMask = TVIS.STATEIMAGEMASK
                    };
                    User32.SendMessageW(tv, (User32.WM)TVM.GETITEMW, IntPtr.Zero, ref item);
                    Debug.Assert(!tv.CheckBoxes || (((int)item.state >> SHIFTVAL) > 1) == CheckedInternal,
                        "isChecked on node '" + Name + "' did not match the state in TVM_GETITEM.");
                }
#endif
                return CheckedInternal;
            }
            set
            {
                TreeView tv = TreeView;
                if (tv != null)
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
        ///  The contextMenu associated with this tree node. The contextMenu
        ///  will be shown when the user right clicks the mouse on the control.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.ControlContextMenuDescr))]
        public virtual ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return contextMenuStrip;
            }
            set
            {
                contextMenuStrip = value;
            }
        }

        /// <summary>
        ///  The first child node of this node.
        /// </summary>
        [Browsable(false)]
        public TreeNode FirstNode
        {
            get
            {
                if (childCount == 0)
                {
                    return null;
                }

                return children[0];
            }
        }

        private TreeNode FirstVisibleParent
        {
            get
            {
                TreeNode node = this;
                while (node != null && node.Bounds.IsEmpty)
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
                if (propBag is null)
                {
                    return Color.Empty;
                }

                return propBag.ForeColor;
            }
            set
            {
                Color oldfc = ForeColor;
                // If we're setting the color to the default again, delete the propBag if it doesn't contain
                // useful data.
                if (value.IsEmpty)
                {
                    if (propBag != null)
                    {
                        propBag.ForeColor = Color.Empty;
                        RemovePropBagIfEmpty();
                    }
                    if (!oldfc.IsEmpty)
                    {
                        InvalidateHostTree();
                    }

                    return;
                }

                // Not the default, so if necessary create a new propBag, and fill it with the new forecolor

                if (propBag is null)
                {
                    propBag = new OwnerDrawPropertyBag();
                }

                propBag.ForeColor = value;
                if (!value.Equals(oldfc))
                {
                    InvalidateHostTree();
                }
            }
        }

        /// <summary>
        ///  Returns the full path of this node.
        ///  The path consists of the labels of each of the nodes from the root to this node,
        ///  each separated by the pathSeperator.
        /// </summary>
        [Browsable(false)]
        public string FullPath
        {
            get
            {
                TreeView tv = TreeView;
                if (tv != null)
                {
                    StringBuilder path = new StringBuilder();
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
        ///  The HTREEITEM handle associated with this node.  If the handle
        ///  has not yet been created, this will force handle creation.
        /// </summary>
        [Browsable(false)]
        public IntPtr Handle
        {
            get
            {
                if (handle == IntPtr.Zero && TreeView != null)
                {
                    TreeView.CreateControl(); // force handle creation
                }
                return handle;
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(ImageList.Indexer.DefaultIndex)]
        [RelatedImageList("TreeView.ImageList")]
        public int ImageIndex
        {
            get
            {
                TreeView tv = TreeView;
                if (ImageIndexer.Index != ImageList.Indexer.DefaultIndex && tv != null && tv.ImageList != null && ImageIndexer.Index >= tv.ImageList.Images.Count)
                {
                    return tv.ImageList.Images.Count - 1;
                }

                return ImageIndexer.Index;
            }
            set
            {
                if (value < ImageList.Indexer.DefaultIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, ImageList.Indexer.DefaultIndex));
                }

                if (ImageIndexer.Index == value && value != ImageList.Indexer.DefaultIndex)
                {
                    return;
                }

                ImageIndexer.Index = value;
                UpdateNode(TVIF.IMAGE);
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [RelatedImageList("TreeView.ImageList")]
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
                UpdateNode(TVIF.IMAGE);
            }
        }

        /// <summary>
        ///  Returns the position of this node in relation to its siblings
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeNodeIndexDescr))]
        public int Index => index;

        /// <summary>
        ///  Specifies whether this node is being edited by the user.
        /// </summary>
        [Browsable(false)]
        public bool IsEditing
        {
            get
            {
                TreeView tv = TreeView;

                if (tv != null)
                {
                    return tv.editNode == this;
                }

                return false;
            }
        }

        /// <summary>
        ///  Specifies whether this node is in the expanded state.
        /// </summary>
        [Browsable(false)]
        public bool IsExpanded
        {
            get
            {
                if (handle == IntPtr.Zero)
                {
                    return expandOnRealization;
                }
                return (State & TVIS.EXPANDED) != 0;
            }
        }

        /// <summary>
        ///  Specifies whether this node is in the selected state.
        /// </summary>
        [Browsable(false)]
        public bool IsSelected
        {
            get
            {
                if (handle == IntPtr.Zero)
                {
                    return false;
                }

                return (State & TVIS.SELECTED) != 0;
            }
        }

        /// <summary>
        ///  Specifies whether this node is visible.
        /// </summary>
        [Browsable(false)]
        public bool IsVisible
        {
            get
            {
                if (handle == IntPtr.Zero)
                {
                    return false;
                }

                TreeView tv = TreeView;
                if (tv.IsDisposed)
                {
                    return false;
                }

                RECT rc = new RECT();
                unsafe
                { *((IntPtr*)&rc.left) = Handle; }

                bool visible = ((int)User32.SendMessageW(tv, (User32.WM)TVM.GETITEMRECT, (IntPtr)1, ref rc) != 0);
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
        public TreeNode LastNode
        {
            get
            {
                if (childCount == 0)
                {
                    return null;
                }

                return children[childCount - 1];
            }
        }

        /// <summary>
        ///  This denotes the depth of nesting of the treenode.
        /// </summary>
        [Browsable(false)]
        public int Level
        {
            get
            {
                if (Parent is null)
                {
                    return 0;
                }
                else
                {
                    return Parent.Level + 1;
                }
            }
        }

        /// <summary>
        ///  The next sibling node.
        /// </summary>
        [Browsable(false)]
        public TreeNode NextNode
        {
            get
            {
                if (parent != null && index + 1 < parent.Nodes.Count)
                {
                    return parent.Nodes[index + 1];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///  The next visible node.  It may be a child, sibling,
        ///  or a node from another branch.
        /// </summary>
        [Browsable(false)]
        public TreeNode NextVisibleNode
        {
            get
            {
                // TVGN_NEXTVISIBLE can only be sent if the specified node is visible.
                // So before sending, we check if this node is visible. If not, we find the first visible parent.
                //
                TreeView tv = TreeView;
                if (tv is null || tv.IsDisposed)
                {
                    return null;
                }

                TreeNode node = FirstVisibleParent;

                if (node != null)
                {
                    IntPtr next = User32.SendMessageW(tv, (User32.WM)TVM.GETNEXTITEM, (IntPtr)TVGN.NEXTVISIBLE, node.Handle);
                    if (next != IntPtr.Zero)
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
        public Font NodeFont
        {
            get
            {
                if (propBag is null)
                {
                    return null;
                }

                return propBag.Font;
            }
            set
            {
                Font oldfont = NodeFont;
                // If we're setting the font to the default again, delete the propBag if it doesn't contain
                // useful data.
                if (value is null)
                {
                    if (propBag != null)
                    {
                        propBag.Font = null;
                        RemovePropBagIfEmpty();
                    }
                    if (oldfont != null)
                    {
                        InvalidateHostTree();
                    }

                    return;
                }

                // Not the default, so if necessary create a new propBag, and fill it with the font

                if (propBag is null)
                {
                    propBag = new OwnerDrawPropertyBag();
                }

                propBag.Font = value;
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
                if (nodes is null)
                {
                    nodes = new TreeNodeCollection(this);
                }
                return nodes;
            }
        }

        /// <summary>
        ///  Retrieves parent node.
        /// </summary>
        [Browsable(false)]
        public TreeNode Parent
        {
            get
            {
                TreeView tv = TreeView;

                // Don't expose the virtual root publicly
                if (tv != null && parent == tv.root)
                {
                    return null;
                }

                return parent;
            }
        }

        /// <summary>
        ///  The previous sibling node.
        /// </summary>
        [Browsable(false)]
        public TreeNode PrevNode
        {
            get
            {
                if (parent is null)
                {
                    return null;
                }

                //fixedIndex is used for perf. optimization in case of adding big ranges of nodes
                int currentInd = index;
                int fixedInd = parent.Nodes.FixedIndex;

                if (fixedInd > 0)
                {
                    currentInd = fixedInd;
                }

                if (currentInd > 0 && currentInd <= parent.Nodes.Count)
                {
                    return parent.Nodes[currentInd - 1];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        ///  The next visible node.  It may be a parent, sibling,
        ///  or a node from another branch.
        /// </summary>
        [Browsable(false)]
        public TreeNode PrevVisibleNode
        {
            get
            {
                // TVGN_PREVIOUSVISIBLE can only be sent if the specified node is visible.
                // So before sending, we check if this node is visible. If not, we find the first visible parent.
                //
                TreeNode node = FirstVisibleParent;
                TreeView tv = TreeView;

                if (node != null)
                {
                    if (tv is null || tv.IsDisposed)
                    {
                        return null;
                    }
                    IntPtr prev = User32.SendMessageW(tv, (User32.WM)TVM.GETNEXTITEM, (IntPtr)TVGN.PREVIOUSVISIBLE, node.Handle);
                    if (prev != IntPtr.Zero)
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RelatedImageList("TreeView.ImageList")]
        public int SelectedImageIndex
        {
            get
            {
                TreeView tv = TreeView;
                if (SelectedImageIndexer.Index != ImageList.Indexer.DefaultIndex && tv != null && tv.ImageList != null && SelectedImageIndexer.Index >= tv.ImageList.Images.Count)
                {
                    return tv.ImageList.Images.Count - 1;
                }

                return SelectedImageIndexer.Index;
            }
            set
            {
                if (value < ImageList.Indexer.DefaultIndex)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectedImageIndex), value, ImageList.Indexer.DefaultIndex));
                }

                if (SelectedImageIndexer.Index == value && value != ImageList.Indexer.DefaultIndex)
                {
                    return;
                }

                SelectedImageIndexer.Index = value;
                UpdateNode(TVIF.SELECTEDIMAGE);
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RelatedImageList("TreeView.ImageList")]
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
                UpdateNode(TVIF.SELECTEDIMAGE);
            }
        }

        /// <summary>
        ///  Retrieve state bits for this node
        /// </summary>
        internal TVIS State
        {
            get
            {
                if (handle == IntPtr.Zero)
                {
                    return 0;
                }

                TreeView tv = TreeView;
                if (tv is null || tv.IsDisposed)
                {
                    return 0;
                }

                var item = new TVITEMW
                {
                    hItem = Handle,
                    mask = TVIF.HANDLE | TVIF.STATE,
                    stateMask = TVIS.SELECTED | TVIS.EXPANDED
                };
                User32.SendMessageW(tv, (User32.WM)TVM.GETITEMW, IntPtr.Zero, ref item);
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [RelatedImageList("TreeView.StateImageList")]
        public string StateImageKey
        {
            get
            {
                return StateImageIndexer.Key;
            }
            set
            {
                if (StateImageIndexer.Key == value && !string.Equals(value, ImageList.Indexer.DefaultKey))
                {
                    return;
                }

                StateImageIndexer.Key = value;
                if (treeView != null && !treeView.CheckBoxes)
                {
                    UpdateNode(TVIF.STATE);
                }
            }
        }

        [Localizable(true)]
        [TypeConverter(typeof(NoneExcludedImageIndexConverter))]
        [DefaultValue(ImageList.Indexer.DefaultIndex)]
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeNodeStateImageIndexDescr))]
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [RefreshProperties(RefreshProperties.Repaint)]
        [RelatedImageList("TreeView.StateImageList")]
        public int StateImageIndex
        {
            get
            {
                TreeView tv = TreeView;
                if (StateImageIndexer.Index != ImageList.Indexer.DefaultIndex && tv != null && tv.StateImageList != null && StateImageIndexer.Index >= tv.StateImageList.Images.Count)
                {
                    return tv.StateImageList.Images.Count - 1;
                }

                return StateImageIndexer.Index;
            }
            set
            {
                if (value < ImageList.Indexer.DefaultIndex || value > ALLOWEDIMAGES)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(StateImageIndex), value));
                }

                if (StateImageIndexer.Index == value && value != ImageList.Indexer.DefaultIndex)
                {
                    return;
                }

                StateImageIndexer.Index = value;
                if (treeView != null && !treeView.CheckBoxes)
                {
                    UpdateNode(TVIF.STATE);
                }
            }
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
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
        ///  The label text for the tree node
        /// </summary>
        [Localizable(true)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.TreeNodeTextDescr))]
        public string Text
        {
            get
            {
                return text ?? "";
            }
            set
            {
                text = value;
                UpdateNode(TVIF.TEXT);
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
            get
            {
                return toolTipText;
            }
            set
            {
                toolTipText = value;
            }
        }

        /// <summary>
        ///  The name for the tree node - useful for indexing.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.TreeNodeNodeNameDescr))]
        public string Name
        {
            get
            {
                return name ?? "";
            }
            set
            {
                name = value;
            }
        }

        /// <summary>
        ///  Return the TreeView control this node belongs to.
        /// </summary>
        [Browsable(false)]
        public TreeView TreeView
        {
            get
            {
                if (treeView is null)
                {
                    treeView = FindTreeView();
                }

                return treeView;
            }
        }

        /// <summary>
        ///  Adds a new child node at the appropriate sorted position
        /// </summary>
        internal int AddSorted(TreeNode node)
        {
            int index = 0;
            int iMin, iLim, iT;
            string nodeText = node.Text;
            TreeView parentTreeView = TreeView;

            if (childCount > 0)
            {
                if (parentTreeView.TreeViewNodeSorter is null)
                {
                    CompareInfo compare = Application.CurrentCulture.CompareInfo;

                    // Optimize for the case where they're already sorted
                    if (compare.Compare(children[childCount - 1].Text, nodeText) <= 0)
                    {
                        index = childCount;
                    }
                    else
                    {
                        // Insert at appropriate sorted spot
                        for (iMin = 0, iLim = childCount; iMin < iLim;)
                        {
                            iT = (iMin + iLim) / 2;
                            if (compare.Compare(children[iT].Text, nodeText) <= 0)
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
                    for (iMin = 0, iLim = childCount; iMin < iLim;)
                    {
                        iT = (iMin + iLim) / 2;
                        if (sorter.Compare(children[iT] /*previous*/, node/*current*/) <= 0)
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
        public static TreeNode FromHandle(TreeView tree, IntPtr handle)
        {
            return tree.NodeFromHandle(handle);
        }

        private void SortChildren(TreeView parentTreeView)
        {
            if (childCount > 0)
            {
                TreeNode[] newOrder = new TreeNode[childCount];
                if (parentTreeView is null || parentTreeView.TreeViewNodeSorter is null)
                {
                    CompareInfo compare = Application.CurrentCulture.CompareInfo;
                    for (int i = 0; i < childCount; i++)
                    {
                        int min = -1;
                        for (int j = 0; j < childCount; j++)
                        {
                            if (children[j] is null)
                            {
                                continue;
                            }

                            if (min == -1)
                            {
                                min = j;
                                continue;
                            }
                            if (compare.Compare(children[j].Text, children[min].Text) <= 0)
                            {
                                min = j;
                            }
                        }

                        Debug.Assert(min != -1, "Bad sorting");
                        newOrder[i] = children[min];
                        children[min] = null;
                        newOrder[i].index = i;
                        newOrder[i].SortChildren(parentTreeView);
                    }
                    children = newOrder;
                }
                else
                {
                    IComparer sorter = parentTreeView.TreeViewNodeSorter;
                    for (int i = 0; i < childCount; i++)
                    {
                        int min = -1;
                        for (int j = 0; j < childCount; j++)
                        {
                            if (children[j] is null)
                            {
                                continue;
                            }

                            if (min == -1)
                            {
                                min = j;
                                continue;
                            }
                            if (sorter.Compare(children[j] /*previous*/, children[min] /*current*/) <= 0)
                            {
                                min = j;
                            }
                        }

                        Debug.Assert(min != -1, "Bad sorting");
                        newOrder[i] = children[min];
                        children[min] = null;
                        newOrder[i].index = i;
                        newOrder[i].SortChildren(parentTreeView);
                    }
                    children = newOrder;
                }
            }
        }

        /// <summary>
        ///  Initiate editing of the node's label.
        ///  Only effective if LabelEdit property is true.
        /// </summary>
        public void BeginEdit()
        {
            if (handle != IntPtr.Zero)
            {
                TreeView tv = TreeView;
                if (tv.LabelEdit == false)
                {
                    throw new InvalidOperationException(SR.TreeNodeBeginEditFailed);
                }

                if (!tv.Focused)
                {
                    tv.Focus();
                }

                User32.SendMessageW(tv, (User32.WM)TVM.EDITLABELW, (IntPtr)0, handle);
            }
        }

        /// <summary>
        ///  Called by the tree node collection to clear all nodes.  We optimize here if
        ///  this is the root node.
        /// </summary>
        internal void Clear()
        {
            // This is a node that is a child of some other node.  We have
            // to selectively remove children here.
            //
            bool isBulkOperation = false;
            TreeView tv = TreeView;

            try
            {
                if (tv != null)
                {
                    tv.nodesCollectionClear = true;

                    if (tv != null && childCount > MAX_TREENODES_OPS)
                    {
                        isBulkOperation = true;
                        tv.BeginUpdate();
                    }
                }

                while (childCount > 0)
                {
                    children[childCount - 1].Remove(true);
                }
                children = null;

                if (tv != null && isBulkOperation)
                {
                    tv.EndUpdate();
                }
            }
            finally
            {
                if (tv != null)
                {
                    tv.nodesCollectionClear = false;
                }
                nodesCleared = true;
            }
        }

        /// <summary>
        ///  Clone the entire subtree rooted at this node.
        /// </summary>
        public virtual object Clone()
        {
            Type clonedType = GetType();
            TreeNode node = null;

            if (clonedType == typeof(TreeNode))
            {
                node = new TreeNode(text, ImageIndexer.Index, SelectedImageIndexer.Index);
            }
            else
            {
                node = (TreeNode)Activator.CreateInstance(clonedType);
            }

            node.Text = text;
            node.Name = name;
            node.ImageIndexer.Index = ImageIndexer.Index;
            node.SelectedImageIndexer.Index = SelectedImageIndexer.Index;

            node.StateImageIndexer.Index = StateImageIndexer.Index;
            node.ToolTipText = toolTipText;
            node.ContextMenuStrip = contextMenuStrip;

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

            if (childCount > 0)
            {
                node.children = new TreeNode[childCount];
                for (int i = 0; i < childCount; i++)
                {
                    node.Nodes.Add((TreeNode)children[i].Clone());
                }
            }

            // Clone properties
            //
            if (propBag != null)
            {
                node.propBag = OwnerDrawPropertyBag.Copy(propBag);
            }
            node.Checked = Checked;
            node.Tag = Tag;

            return node;
        }

        private void CollapseInternal(bool ignoreChildren)
        {
            TreeView tv = TreeView;
            bool setSelection = false;
            collapseOnRealization = false;
            expandOnRealization = false;

            if (tv is null || !tv.IsHandleCreated)
            {
                collapseOnRealization = true;
                return;
            }

            //terminating condition for recursion...
            //
            if (ignoreChildren)
            {
                DoCollapse(tv);
            }
            else
            {
                if (!ignoreChildren && childCount > 0)
                {
                    // Virtual root should collapse all its children
                    for (int i = 0; i < childCount; i++)
                    {
                        if (tv.SelectedNode == children[i])
                        {
                            setSelection = true;
                        }
                        children[i].DoCollapse(tv);
                        children[i].Collapse();
                    }
                }
                DoCollapse(tv);
            }

            if (setSelection)
            {
                tv.SelectedNode = this;
            }

            tv.Invalidate();
            collapseOnRealization = false;
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
            if ((State & TVIS.EXPANDED) != 0)
            {
                TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(this, false, TreeViewAction.Collapse);
                tv.OnBeforeCollapse(e);
                if (!e.Cancel)
                {
                    User32.SendMessageW(tv, (User32.WM)TVM.EXPAND, (IntPtr)TVE.COLLAPSE, (IntPtr)Handle);
                    tv.OnAfterCollapse(new TreeViewEventArgs(this));
                }
            }
        }

        protected virtual void Deserialize(SerializationInfo serializationInfo, StreamingContext context)
        {
            int childCount = 0;
            int imageIndex = ImageList.Indexer.DefaultIndex;
            string imageKey = null;

            int selectedImageIndex = ImageList.Indexer.DefaultIndex;
            string selectedImageKey = null;

            int stateImageIndex = ImageList.Indexer.DefaultIndex;
            string stateImageKey = null;

            foreach (SerializationEntry entry in serializationInfo)
            {
                switch (entry.Name)
                {
                    case "PropBag":
                        // this would throw a InvalidaCastException if improper cast, thus validating the serializationInfo for OwnerDrawPropertyBag
                        propBag = (OwnerDrawPropertyBag)serializationInfo.GetValue(entry.Name, typeof(OwnerDrawPropertyBag));
                        break;
                    case nameof(Text):
                        Text = serializationInfo.GetString(entry.Name);
                        break;
                    case nameof(ToolTipText):
                        ToolTipText = serializationInfo.GetString(entry.Name);
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
                        userData = entry.Value;
                        break;
                }
            }

            // let imagekey take precidence
            if (imageKey != null)
            {
                ImageKey = imageKey;
            }
            else if (imageIndex != ImageList.Indexer.DefaultIndex)
            {
                ImageIndex = imageIndex;
            }

            // let selectedimagekey take precidence
            if (selectedImageKey != null)
            {
                SelectedImageKey = selectedImageKey;
            }
            else if (selectedImageIndex != ImageList.Indexer.DefaultIndex)
            {
                SelectedImageIndex = selectedImageIndex;
            }

            // let stateimagekey take precidence
            if (stateImageKey != null)
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
                    childNodes[i] = (TreeNode)serializationInfo.GetValue("children" + i, typeof(TreeNode));
                }
                Nodes.AddRange(childNodes);
            }
        }

        /// <summary>
        ///  Terminate the editing of any tree view item's label.
        /// </summary>
        public void EndEdit(bool cancel)
        {
            TreeView tv = TreeView;
            if (tv is null || tv.IsDisposed)
            {
                return;
            }
            User32.SendMessageW(tv, (User32.WM)TVM.ENDEDITLABELNOW, PARAM.FromBool(cancel));
        }

        /// <summary>
        ///  Makes sure there is enough room to add n children
        /// </summary>
        internal void EnsureCapacity(int num)
        {
            Debug.Assert(num > 0, "required capacity can not be less than 1");
            int size = num;
            if (size < 4)
            {
                size = 4;
            }
            if (children is null)
            {
                children = new TreeNode[size];
            }
            else if (childCount + num > children.Length)
            {
                int newSize = childCount + num;
                if (num == 1)
                {
                    newSize = childCount * 2;
                }
                TreeNode[] bigger = new TreeNode[newSize];
                System.Array.Copy(children, 0, bigger, 0, childCount);
                children = bigger;
            }
        }

        /// <summary>
        ///  Ensures the the node's StateImageIndex value is properly set.
        /// </summary>
        private void EnsureStateImageValue()
        {
            if (treeView is null)
            {
                return;
            }

            if (treeView.CheckBoxes && treeView.StateImageList != null)
            {
                if (!string.IsNullOrEmpty(StateImageKey))
                {
                    StateImageIndex = (Checked) ? 1 : 0;
                    StateImageKey = treeView.StateImageList.Images.Keys[StateImageIndex];
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
            TreeView tv = TreeView;
            if (tv is null || tv.IsDisposed)
            {
                return;
            }

            User32.SendMessageW(tv, (User32.WM)TVM.ENSUREVISIBLE, IntPtr.Zero, Handle);
        }

        /// <summary>
        ///  Expand the node.
        /// </summary>
        public void Expand()
        {
            TreeView tv = TreeView;
            if (tv is null || !tv.IsHandleCreated)
            {
                expandOnRealization = true;
                return;
            }

            ResetExpandedState(tv);
            if (!IsExpanded)
            {
                User32.SendMessageW(tv, (User32.WM)TVM.EXPAND, (IntPtr)TVE.EXPAND, Handle);
            }
            expandOnRealization = false;
        }

        /// <summary>
        ///  Expand the node.
        /// </summary>
        public void ExpandAll()
        {
            Expand();
            for (int i = 0; i < childCount; i++)
            {
                children[i].ExpandAll();
            }
        }
        /// <summary>
        ///  Locate this tree node's containing tree view control by scanning
        ///  up to the virtual root, whose treeView pointer we know to be
        ///  correct
        /// </summary>
        internal TreeView FindTreeView()
        {
            TreeNode node = this;
            while (node.parent != null)
            {
                node = node.parent;
            }

            return node.treeView;
        }

        /// <summary>
        ///  Helper function for getFullPath().
        /// </summary>
        private void GetFullPath(StringBuilder path, string pathSeparator)
        {
            if (parent != null)
            {
                parent.GetFullPath(path, pathSeparator);
                if (parent.parent != null)
                {
                    path.Append(pathSeparator);
                }

                path.Append(text);
            }
        }

        /// <summary>
        ///  Returns number of child nodes.
        /// </summary>
        public int GetNodeCount(bool includeSubTrees)
        {
            int total = childCount;
            if (includeSubTrees)
            {
                for (int i = 0; i < childCount; i++)
                {
                    total += children[i].GetNodeCount(true);
                }
            }
            return total;
        }

        /// <summary>
        ///  Check for any circular reference in the ancestors chain.
        /// </summary>
        internal void CheckParentingCycle(TreeNode candidateToAdd)
        {
            TreeNode node = this;

            while (node != null)
            {
                if (node == candidateToAdd)
                {
                    throw new ArgumentException(SR.TreeNodeCircularReference);
                }
                node = node.parent;
            }
        }

        /// <summary>
        ///  Helper function to add node at a given index after all validation has been done
        /// </summary>
        internal void InsertNodeAt(int index, TreeNode node)
        {
            EnsureCapacity(1);
            node.parent = this;
            node.index = index;
            for (int i = childCount; i > index; --i)
            {
                (children[i] = children[i - 1]).index = i;
            }
            children[index] = node;
            childCount++;
            node.Realize(false);

            if (TreeView != null && node == TreeView.selectedNode)
            {
                TreeView.SelectedNode = node; // communicate this to the handle
            }
        }

        /// <summary>
        ///  Invalidates the treeview control that is hosting this node
        /// </summary>
        private void InvalidateHostTree()
        {
            if (treeView != null && treeView.IsHandleCreated)
            {
                treeView.Invalidate();
            }
        }

        internal unsafe void Realize(bool insertFirst)
        {
            TreeView tv = TreeView;
            if (tv is null || !tv.IsHandleCreated || tv.IsDisposed)
            {
                return;
            }

            if (parent != null)
            {
                // Never realize the virtual root
                if (tv.InvokeRequired)
                {
                    throw new InvalidOperationException(SR.InvalidCrossThreadControlCall);
                }

                var tvis = new TVINSERTSTRUCTW
                {
                    hParent = parent.handle
                };
                tvis.item.mask = InsertMask;

                TreeNode prev = PrevNode;
                if (insertFirst || prev is null)
                {
                    tvis.hInsertAfter = (IntPtr)TVI.FIRST;
                }
                else
                {
                    tvis.hInsertAfter = prev.handle;
                }

                tvis.item.pszText = Marshal.StringToHGlobalAuto(text);
                tvis.item.iImage = (ImageIndexer.ActualIndex == ImageList.Indexer.DefaultIndex) ? tv.ImageIndexer.ActualIndex : ImageIndexer.ActualIndex;
                tvis.item.iSelectedImage = (SelectedImageIndexer.ActualIndex == ImageList.Indexer.DefaultIndex) ? tv.SelectedImageIndexer.ActualIndex : SelectedImageIndexer.ActualIndex;
                tvis.item.mask = TVIF.TEXT;

                tvis.item.stateMask = 0;
                tvis.item.state = 0;

                if (tv.CheckBoxes)
                {
                    tvis.item.mask |= TVIF.STATE;
                    tvis.item.stateMask |= TVIS.STATEIMAGEMASK;
                    tvis.item.state |= CheckedInternal ? CHECKED : UNCHECKED;
                }
                else if (tv.StateImageList != null && StateImageIndexer.ActualIndex >= 0)
                {
                    tvis.item.mask |= TVIF.STATE;
                    tvis.item.stateMask = TVIS.STATEIMAGEMASK;
                    tvis.item.state = (TVIS)((StateImageIndexer.ActualIndex + 1) << SHIFTVAL);
                }

                if (tvis.item.iImage >= 0)
                {
                    tvis.item.mask |= TVIF.IMAGE;
                }

                if (tvis.item.iSelectedImage >= 0)
                {
                    tvis.item.mask |= TVIF.SELECTEDIMAGE;
                }

                // If you are editing when you add a new node, then the edit control
                // gets placed in the wrong place. You must restore the edit mode
                // asynchronously (PostMessage) after the add is complete
                // to get the expected behavior.
                //
                bool editing = false;
                IntPtr editHandle = User32.SendMessageW(tv, (User32.WM)TVM.GETEDITCONTROL);
                if (editHandle != IntPtr.Zero)
                {
                    // currently editing...
                    //
                    editing = true;
                    User32.SendMessageW(tv, (User32.WM)TVM.ENDEDITLABELNOW, PARAM.FromBool(false));
                }

                handle = User32.SendMessageW(tv, (User32.WM)TVM.INSERTITEMW, IntPtr.Zero, ref tvis);
                tv.nodeTable[handle] = this;

                // Lets update the Lparam to the Handle ....
                UpdateNode(TVIF.PARAM);

                Marshal.FreeHGlobal(tvis.item.pszText);

                if (editing)
                {
                    User32.PostMessageW(tv, (User32.WM)TVM.EDITLABELW, IntPtr.Zero, handle);
                }

                User32.InvalidateRect(new HandleRef(tv, tv.Handle), null, BOOL.FALSE);

                if (parent.nodesCleared && (insertFirst || prev is null) && !tv.Scrollable)
                {
                    // We need to Redraw the TreeView ...
                    // If and only If we are not scrollable ...
                    // and this is the FIRST NODE to get added..
                    // This is Comctl quirk where it just doesn't draw
                    // the first node after a Clear( ) if Scrollable == false.
                    User32.SendMessageW(tv, User32.WM.SETREDRAW, PARAM.FromBool(true));
                    nodesCleared = false;
                }
            }

            for (int i = childCount - 1; i >= 0; i--)
            {
                children[i].Realize(true);
            }

            // If node expansion was requested before the handle was created,
            // we can expand it now.
            if (expandOnRealization)
            {
                Expand();
            }

            // If node collapse was requested before the handle was created,
            // we can expand it now.
            if (collapseOnRealization)
            {
                Collapse();
            }
        }

        /// <summary>
        ///  Remove this node from the TreeView control.  Child nodes are also removed from the
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
            //

            for (int i = 0; i < childCount; i++)
            {
                children[i].Remove(false);
            }
            // children = null;
            // unlink ourself
            if (notify && parent != null)
            {
                for (int i = index; i < parent.childCount - 1; ++i)
                {
                    (parent.children[i] = parent.children[i + 1]).index = i;
                }

                parent.children[parent.childCount - 1] = null;
                parent.childCount--;
                parent = null;
            }
            // Expand when we are realized the next time.
            expandOnRealization = expanded;

            // unrealize ourself
            TreeView tv = TreeView;
            if (tv is null || tv.IsDisposed)
            {
                return;
            }

            if (handle != IntPtr.Zero)
            {
                if (notify && tv.IsHandleCreated)
                {
                    User32.SendMessageW(tv, (User32.WM)TVM.DELETEITEM, IntPtr.Zero, handle);
                }

                treeView.nodeTable.Remove(handle);
                handle = IntPtr.Zero;
            }
            treeView = null;
        }

        /// <summary>
        ///  Removes the propBag object if it's now devoid of useful data
        /// </summary>
        private void RemovePropBagIfEmpty()
        {
            if (propBag is null)
            {
                return;
            }

            if (propBag.IsEmpty())
            {
                propBag = null;
            }

            return;
        }

        private unsafe void ResetExpandedState(TreeView tv)
        {
            Debug.Assert(tv.IsHandleCreated, "nonexistent handle");

            var item = new TVITEMW
            {
                mask = TVIF.HANDLE | TVIF.STATE,
                hItem = handle,
                stateMask = TVIS.EXPANDEDONCE,
                state = 0
            };
            User32.SendMessageW(tv, (User32.WM)TVM.SETITEMW, IntPtr.Zero, ref item);
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
        ///  Review: Changing this would break VB users. so suppresing this message.
        ///
        protected virtual void Serialize(SerializationInfo si, StreamingContext context)
        {
            if (propBag != null)
            {
                si.AddValue("PropBag", propBag, typeof(OwnerDrawPropertyBag));
            }

            si.AddValue(nameof(Text), text);
            si.AddValue(nameof(ToolTipText), toolTipText);
            si.AddValue(nameof(Name), Name);
            si.AddValue("IsChecked", treeNodeState[TREENODESTATE_isChecked]);
            si.AddValue(nameof(ImageIndex), ImageIndexer.Index);
            si.AddValue(nameof(ImageKey), ImageIndexer.Key);
            si.AddValue(nameof(SelectedImageIndex), SelectedImageIndexer.Index);
            si.AddValue(nameof(SelectedImageKey), SelectedImageIndexer.Key);

            if (treeView != null && treeView.StateImageList != null)
            {
                si.AddValue("StateImageIndex", StateImageIndexer.Index);
            }

            if (treeView != null && treeView.StateImageList != null)
            {
                si.AddValue(nameof(StateImageKey), StateImageIndexer.Key);
            }

            si.AddValue("ChildCount", childCount);

            if (childCount > 0)
            {
                for (int i = 0; i < childCount; i++)
                {
                    si.AddValue("children" + i, children[i], typeof(TreeNode));
                }
            }

            if (userData != null && userData.GetType().IsSerializable)
            {
                si.AddValue("UserData", userData, userData.GetType());
            }
        }
        /// <summary>
        ///  Toggle the state of the node. Expand if collapsed or collapse if
        ///  expanded.
        /// </summary>
        public void Toggle()
        {
            Debug.Assert(parent != null, "toggle on virtual root");

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
            return "TreeNode: " + (text ?? "");
        }

        /// <summary>
        ///  Tell the TreeView to refresh this node
        /// </summary>
        private void UpdateNode(TVIF mask)
        {
            if (handle == IntPtr.Zero)
            {
                return;
            }

            TreeView tv = TreeView;
            Debug.Assert(tv != null, "TreeNode has handle but no TreeView");
            if (tv.IsDisposed)
            {
                return;
            }

            var item = new TVITEMW
            {
                mask = TVIF.HANDLE | mask,
                hItem = handle
            };
            if ((mask & TVIF.TEXT) != 0)
            {
                item.pszText = Marshal.StringToHGlobalAuto(text);
            }

            if ((mask & TVIF.IMAGE) != 0)
            {
                item.iImage = (ImageIndexer.ActualIndex == ImageList.Indexer.DefaultIndex) ? tv.ImageIndexer.ActualIndex : ImageIndexer.ActualIndex;
            }

            if ((mask & TVIF.SELECTEDIMAGE) != 0)
            {
                item.iSelectedImage = (SelectedImageIndexer.ActualIndex == ImageList.Indexer.DefaultIndex) ? tv.SelectedImageIndexer.ActualIndex : SelectedImageIndexer.ActualIndex;
            }

            if ((mask & TVIF.STATE) != 0)
            {
                item.stateMask = TVIS.STATEIMAGEMASK;
                if (StateImageIndexer.ActualIndex != ImageList.Indexer.DefaultIndex)
                {
                    item.state = (TVIS)((StateImageIndexer.ActualIndex + 1) << SHIFTVAL);
                }
                // ActualIndex == -1 means "don't use custom image list"
                // so just leave item.state set to zero, that tells the unmanaged control
                // to use no state image for this node.
            }
            if ((mask & TVIF.PARAM) != 0)
            {
                item.lParam = handle;
            }

            User32.SendMessageW(tv, (User32.WM)TVM.SETITEMW, IntPtr.Zero, ref item);
            if ((mask & TVIF.TEXT) != 0)
            {
                Marshal.FreeHGlobal(item.pszText);
                if (tv.Scrollable)
                {
                    tv.ForceScrollbarUpdate(false);
                }
            }
        }

        internal unsafe void UpdateImage()
        {
            TreeView tv = TreeView;
            if (tv.IsDisposed)
            {
                return;
            }

            var item = new TVITEMW
            {
                mask = TVIF.HANDLE | TVIF.IMAGE,
                hItem = Handle,
                iImage = Math.Max(0, ((ImageIndexer.ActualIndex >= tv.ImageList.Images.Count) ? tv.ImageList.Images.Count - 1 : ImageIndexer.ActualIndex))
            };
            User32.SendMessageW(tv, (User32.WM)TVM.SETITEMW, IntPtr.Zero, ref item);
        }

        /// <summary>
        ///  ISerializable private implementation
        /// </summary>
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            Serialize(si, context);
        }
    }
}
