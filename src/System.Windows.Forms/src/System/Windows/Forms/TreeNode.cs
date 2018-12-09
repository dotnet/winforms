// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {
    using System.Text;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Security;
    using System.Security.Permissions;

    using System;
    using System.Drawing.Design;    
    using System.Collections;
    using System.Globalization;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.IO;
    using System.Drawing;
    using Microsoft.Win32;
    

    /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Implements a node of a <see cref='System.Windows.Forms.TreeView'/>.
    ///
    ///    </para>
    /// </devdoc>
    [
    TypeConverterAttribute(typeof(TreeNodeConverter)), Serializable,
    DefaultProperty(nameof(Text)),    
    SuppressMessage("Microsoft.Usage", "CA2240:ImplementISerializableCorrectly")
    ]
    public class TreeNode : MarshalByRefObject, ICloneable, ISerializable {
        private const int SHIFTVAL = 12;
        private const int CHECKED = 2 << SHIFTVAL;
        private const int UNCHECKED = 1 << SHIFTVAL;
        private const int ALLOWEDIMAGES = 14;
        
        //the threshold value used to optimize AddRange and Clear operations for a big number of nodes
        internal const int MAX_TREENODES_OPS = 200;

        // we use it to store font and color data in a minimal-memory-cost manner
        // ie. nodes which don't use fancy fonts or colors (ie. that use the TreeView settings for these)
        //     will take up less memory than those that do.
        internal OwnerDrawPropertyBag propBag = null;
        internal IntPtr handle;
        internal string text;
        internal string name; 

        // note: as the checked state of a node is user controlled, and this variable is simply for
        // state caching when a node hasn't yet been realized, you should use the Checked property to
        // find out the check state of a node, and not this member variable.
        //private bool isChecked = false;
        private const int   TREENODESTATE_isChecked     = 0x00000001;

        private System.Collections.Specialized.BitVector32  treeNodeState;

        private TreeNodeImageIndexer imageIndexer;
        private TreeNodeImageIndexer selectedImageIndexer;
        private TreeNodeImageIndexer stateImageIndexer;

        private string toolTipText = "";
        private ContextMenu contextMenu = null;
        private ContextMenuStrip contextMenuStrip = null;
        internal bool nodesCleared = false;

        // We need a special way to defer to the TreeView's image
        // list for indexing purposes.
        internal class TreeNodeImageIndexer : ImageList.Indexer {
           private TreeNode owner;
           
           /// <include file='doc\TreeNode.uex' path='docs/doc[@for="ImageListType"]/*' />
           public enum ImageListType {
               /// <include file='doc\TreeNode.uex' path='docs/doc[@for="ImageListType.Default"]/*' />
               Default,
               /// <include file='doc\TreeNode.uex' path='docs/doc[@for="ImageListType.State"]/*' />
               State
           }
           private ImageListType imageListType;

           /// <include file='doc\TreeNode.uex' path='docs/doc[@for="ImageListType.TreeNodeImageIndexer"]/*' />
           public TreeNodeImageIndexer(TreeNode node, ImageListType imageListType) {
              owner = node;
              this.imageListType = imageListType;
           }

           /// <include file='doc\TreeNode.uex' path='docs/doc[@for="ImageListType.ImageList"]/*' />
           public override ImageList ImageList {
                get {
                    if (owner.TreeView != null) {
                        if (imageListType == ImageListType.State) {
                            return owner.TreeView.StateImageList;
                        }
                        else {
                            return owner.TreeView.ImageList;
                        }
                    }
                    else {
                        return null;
                    }
                }
                set { Debug.Assert(false, "We should never set the image list"); }
            }
           
        }



        internal TreeNodeImageIndexer ImageIndexer {
            get { 
                //Demand create the imageIndexer
                if (imageIndexer == null) {
                      imageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);
                }
                return imageIndexer; 
            }
        }  

        internal TreeNodeImageIndexer SelectedImageIndexer {
            get { 
                //Demand create the imageIndexer
                if (selectedImageIndexer == null) {
                      selectedImageIndexer = new TreeNodeImageIndexer(this, TreeNodeImageIndexer.ImageListType.Default);
                }

                return selectedImageIndexer;

            }
        }

        internal TreeNodeImageIndexer StateImageIndexer {
            get { 
                //Demand create the imageIndexer
                if (stateImageIndexer == null) {
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
        private bool expandOnRealization = false;
        private bool collapseOnRealization = false;
        private TreeNodeCollection nodes = null;
        object userData;
        
        private readonly static int insertMask = 
                               NativeMethods.TVIF_TEXT
                             | NativeMethods.TVIF_IMAGE
                             | NativeMethods.TVIF_SELECTEDIMAGE;

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeNode"]/*' />
        /// <devdoc>
        ///     Creates a TreeNode object.
        /// </devdoc>
        public TreeNode() {
            treeNodeState = new System.Collections.Specialized.BitVector32();
        }

        internal TreeNode(TreeView treeView) : this() {
            this.treeView = treeView;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeNode1"]/*' />
        /// <devdoc>
        ///     Creates a TreeNode object.
        /// </devdoc>
        public TreeNode(string text) : this() {
            this.text = text;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeNode2"]/*' />
        /// <devdoc>
        ///     Creates a TreeNode object.
        /// </devdoc>
        public TreeNode(string text, TreeNode[] children) : this() {
            this.text = text;
            this.Nodes.AddRange(children);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeNode3"]/*' />
        /// <devdoc>
        ///     Creates a TreeNode object.
        /// </devdoc>
        public TreeNode(string text, int imageIndex, int selectedImageIndex) : this() {
            this.text = text;
            this.ImageIndexer.Index = imageIndex;
            this.SelectedImageIndexer.Index = selectedImageIndex;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeNode4"]/*' />
        /// <devdoc>
        ///     Creates a TreeNode object.
        /// </devdoc>
        public TreeNode(string text, int imageIndex, int selectedImageIndex, TreeNode[] children) : this() {
            this.text = text;
            this.ImageIndexer.Index = imageIndex;
            this.SelectedImageIndexer.Index = selectedImageIndex;
            this.Nodes.AddRange(children);
        }

        /**
         * Constructor used in deserialization
         */
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeNode5"]/*' />
        // PM team has reviewed and decided on naming changes already
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        [
            SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")  // Changing Deserialize to be non-virtual
                                                                                                    // would be a breaking change.
        ]
        protected TreeNode(SerializationInfo serializationInfo, StreamingContext context) : this() {
            Deserialize(serializationInfo, context);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.BackColor"]/*' />
        /// <devdoc>
        ///     The background color of this node.
        ///     If null, the color used will be the default color from the TreeView control that this
        ///     node is attached to
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeNodeBackColorDescr))
        ]
        public Color BackColor {
            get {
                if (propBag==null) return Color.Empty;
                return propBag.BackColor;
            }
            set {
                // get the old value
                Color oldbk = this.BackColor;
                // If we're setting the color to the default again, delete the propBag if it doesn't contain
                // useful data.
                if (value.IsEmpty) {
                    if (propBag!=null) {
                        propBag.BackColor = Color.Empty;
                        RemovePropBagIfEmpty();
                    }
                    if (!oldbk.IsEmpty) InvalidateHostTree();
                    return;
                }

                // Not the default, so if necessary create a new propBag, and fill it with the backcolor

                if (propBag==null) propBag = new OwnerDrawPropertyBag();
                propBag.BackColor = value;
                if (!value.Equals(oldbk)) InvalidateHostTree();
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Bounds"]/*' />
        /// <devdoc>
        ///     The bounding rectangle for the node (text area only). The coordinates
        ///     are relative to the upper left corner of the TreeView control.
        /// </devdoc>
        [Browsable(false)]
        public Rectangle Bounds {
            get {
                TreeView tv = this.TreeView;
                if (tv == null || tv.IsDisposed) {
                    return Rectangle.Empty;
                }
                NativeMethods.RECT rc = new NativeMethods.RECT();
                unsafe { *((IntPtr *) &rc.left) = Handle; }
                // wparam: 1=include only text, 0=include entire line
                if ((int)UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_GETITEMRECT, 1, ref rc) == 0) {
                    // This means the node is not visible
                    //
                    return Rectangle.Empty;
                }
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Bounds"]/*' />
        /// <devdoc>
        ///     The bounding rectangle for the node (full row). The coordinates
        ///     are relative to the upper left corner of the TreeView control.
        /// </devdoc>
        internal Rectangle RowBounds {
            get {
                TreeView tv = this.TreeView;
                NativeMethods.RECT rc = new NativeMethods.RECT();
                unsafe { *((IntPtr *) &rc.left) = Handle; }
                // wparam: 1=include only text, 0=include entire line
                if (tv == null || tv.IsDisposed) {
                    return Rectangle.Empty;
                }
                if ((int)UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_GETITEMRECT, 0, ref rc) == 0) {
                    // This means the node is not visible
                    //
                    return Rectangle.Empty;
                }
                return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
            }
        }

        internal bool CheckedStateInternal {
            get {
                return treeNodeState[TREENODESTATE_isChecked];
            }
            set {
                treeNodeState[TREENODESTATE_isChecked] = value;
            }
        }
        
        // Checked does sanity checking and fires Before/AfterCheck events, then forwards to this
        // property to get/set the actual checked value.
        internal bool CheckedInternal {
            get {
                return CheckedStateInternal;
            }
            set {
                CheckedStateInternal = value;
                if (handle == IntPtr.Zero)
                    return;

                TreeView tv = this.TreeView;
                if (tv == null || !tv.IsHandleCreated || tv.IsDisposed)
                    return;

                NativeMethods.TV_ITEM item = new NativeMethods.TV_ITEM();
                item.mask = NativeMethods.TVIF_HANDLE | NativeMethods.TVIF_STATE;
                item.hItem = handle;
                item.stateMask = NativeMethods.TVIS_STATEIMAGEMASK;
                item.state |= value ? CHECKED : UNCHECKED;
                UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_SETITEM, 0, ref item);
                
                
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Checked"]/*' />
        /// <devdoc>	
        ///     Indicates whether the node's checkbox is checked.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeCheckedDescr)),
        DefaultValue(false)
        ]
        public bool Checked {
            get {
#if DEBUG
                if(handle != IntPtr.Zero) {
                    NativeMethods.TV_ITEM item = new NativeMethods.TV_ITEM();
                    item.mask = NativeMethods.TVIF_HANDLE | NativeMethods.TVIF_STATE;
                    item.hItem = handle;                         
                    item.stateMask = NativeMethods.TVIS_STATEIMAGEMASK;
                    UnsafeNativeMethods.SendMessage(new HandleRef(null, TreeView.Handle), NativeMethods.TVM_GETITEM, 0, ref item);
                    Debug.Assert(!TreeView.CheckBoxes || ((item.state >> SHIFTVAL) > 1) == CheckedInternal,
                        "isChecked on node '" + Name + "' did not match the state in TVM_GETITEM.");
                }
#endif
                return CheckedInternal;
            }
            set {
                TreeView tv = TreeView;
                if (tv != null) {
                    bool eventReturn = tv.TreeViewBeforeCheck(this, TreeViewAction.Unknown);
                    if (!eventReturn) {
                        CheckedInternal = value;
                        tv.TreeViewAfterCheck(this, TreeViewAction.Unknown);
                    }
                }
                else {
                    CheckedInternal = value;
                }
            }
        }
        
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ContextMenu"]/*' />
        /// <devdoc>
        ///     The contextMenu associated with this tree node. The contextMenu
        ///     will be shown when the user right clicks the mouse on the control.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ControlContextMenuDescr))
        ]
        public virtual ContextMenu ContextMenu {
            get {
                return contextMenu;
            }
            set {
                contextMenu = value;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ContextMenu"]/*' />
        /// <devdoc>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.ControlContextMenuDescr))
        ]
        public virtual ContextMenuStrip ContextMenuStrip {
            get {
                return contextMenuStrip;
            }
            set {
                contextMenuStrip = value;
            }
        }
        
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.FirstNode"]/*' />
        /// <devdoc>
        ///     The first child node of this node.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode FirstNode {
            get {
                if (childCount == 0) return null;
                return children[0];
            }
        }
        
        private TreeNode FirstVisibleParent {
            get {
                TreeNode node = this;
                while (node != null && node.Bounds.IsEmpty) {
                    node = node.Parent;
                }
                return node;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ForeColor"]/*' />
        /// <devdoc>
        ///     The foreground color of this node.
        ///     If null, the color used will be the default color from the TreeView control that this
        ///     node is attached to
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeNodeForeColorDescr))
        ]
        public Color ForeColor {
            get {
                if (propBag == null) return Color.Empty;
                return propBag.ForeColor;
            }
            set {
                Color oldfc = this.ForeColor;
                // If we're setting the color to the default again, delete the propBag if it doesn't contain
                // useful data.
                if (value.IsEmpty) {
                    if (propBag != null) {
                        propBag.ForeColor = Color.Empty;
                        RemovePropBagIfEmpty();
                    }
                    if (!oldfc.IsEmpty) InvalidateHostTree();
                    return;
                }

                // Not the default, so if necessary create a new propBag, and fill it with the new forecolor

                if (propBag == null) propBag = new OwnerDrawPropertyBag();
                propBag.ForeColor = value;
                if (!value.Equals(oldfc)) InvalidateHostTree();
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.FullPath"]/*' />
        /// <devdoc>
        ///     Returns the full path of this node.
        ///     The path consists of the labels of each of the nodes from the root to this node,
        ///     each separated by the pathSeperator.
        /// </devdoc>
        [Browsable(false)]
        public string FullPath {
            get {
                TreeView tv = TreeView;
                if (tv != null) {
                    StringBuilder path = new StringBuilder();
                    GetFullPath(path, tv.PathSeparator);
                    return path.ToString();
                }
                else 
                    throw new InvalidOperationException(SR.TreeNodeNoParent);
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Handle"]/*' />
        /// <devdoc>
        ///     The HTREEITEM handle associated with this node.  If the handle
        ///     has not yet been created, this will force handle creation.
        /// </devdoc>
       [Browsable(false)]
        public IntPtr Handle {
            get {
                if (handle == IntPtr.Zero) {
                    TreeView.CreateControl(); // force handle creation
                }
                return handle;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ImageIndex"]/*' />
        /// <devdoc>
        ///     The index of the image to be displayed when the node is in the unselected state.
        ///     The image is contained in the ImageList referenced by the imageList property.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeImageIndexDescr)),
        TypeConverterAttribute(typeof(TreeViewImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        DefaultValue(-1),
        RelatedImageList("TreeView.ImageList")
        ]
        public int ImageIndex {
            get { return ImageIndexer.Index;}
            set {
                ImageIndexer.Index = value;
                UpdateNode(NativeMethods.TVIF_IMAGE);
            }
        }


        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ImageIndex"]/*' />
        /// <devdoc>
        ///     The index of the image to be displayed when the node is in the unselected state.
        ///     The image is contained in the ImageList referenced by the imageList property.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeImageKeyDescr)),
        TypeConverterAttribute(typeof(TreeViewImageKeyConverter)),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        RelatedImageList("TreeView.ImageList")
        ]
        public string ImageKey {
            get {return ImageIndexer.Key;}
            set {
                ImageIndexer.Key = value;
                UpdateNode(NativeMethods.TVIF_IMAGE);
            }
        }


        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Index"]/*' />
        /// <devdoc>
        ///     Returns the position of this node in relation to its siblings
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeIndexDescr)),
        ]
        public int Index {
            get { return index;}
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.IsEditing"]/*' />
        /// <devdoc>
        ///     Specifies whether this node is being edited by the user.
        /// </devdoc>
       [Browsable(false)]
        public bool IsEditing {
            get {
                TreeView tv = TreeView;

                if (tv != null)
                    return tv.editNode == this;

                return false;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.IsExpanded"]/*' />
        /// <devdoc>
        ///     Specifies whether this node is in the expanded state.
        /// </devdoc>
        [Browsable(false)]
        public bool IsExpanded {
            get {
                if (handle == IntPtr.Zero) {
                    return expandOnRealization;
                }
                return(State & NativeMethods.TVIS_EXPANDED) != 0;
            }
        }
        
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.IsSelected"]/*' />
        /// <devdoc>
        ///     Specifies whether this node is in the selected state.
        /// </devdoc>
        [Browsable(false)]
        public bool IsSelected {
            get {
                if (handle == IntPtr.Zero) return false;
                return(State & NativeMethods.TVIS_SELECTED) != 0;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.IsVisible"]/*' />
        /// <devdoc>
        ///     Specifies whether this node is visible.
        /// </devdoc>
        [Browsable(false)]
        public bool IsVisible {
            get {
                if (handle == IntPtr.Zero) return false;
                TreeView tv = this.TreeView;
                if (tv.IsDisposed) {
                    return false;
                }

                NativeMethods.RECT rc = new NativeMethods.RECT();
                unsafe { *((IntPtr *) &rc.left) = Handle; }

                bool visible = ((int)UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_GETITEMRECT, 1, ref rc) != 0);
                if (visible) {
                    Size size = tv.ClientSize;
                    visible = (rc.bottom > 0 && rc.right > 0 && rc.top < size.Height && rc.left < size.Width);
                }
                return visible;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.LastNode"]/*' />
        /// <devdoc>
        ///     The last child node of this node.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode LastNode {
            get {
                if (childCount == 0) return null;
                return children[childCount-1];
            }
        }


        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Level"]/*' />
        /// <devdoc>
        ///     This denotes the depth of nesting of the treenode.
        /// </devdoc>
        [Browsable(false)]
        public int Level {
            get {
                if (this.Parent == null) {
                    return 0;
                }
                else {
                    return Parent.Level + 1;
                }
            }
        }



        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.NextNode"]/*' />
        /// <devdoc>
        ///     The next sibling node.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode NextNode {
            get {
                if (index+1 < parent.Nodes.Count) {
                    return parent.Nodes[index+1];
                }
                else {
                    return null;
                }
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.NextVisibleNode"]/*' />
        /// <devdoc>
        ///     The next visible node.  It may be a child, sibling,
        ///     or a node from another branch.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode NextVisibleNode {
            get {
                // TVGN_NEXTVISIBLE can only be sent if the specified node is visible.
                // So before sending, we check if this node is visible. If not, we find the first visible parent.
                //
                TreeView tv = this.TreeView;
                if (tv == null || tv.IsDisposed) {
                    return null;
                }

                TreeNode node = FirstVisibleParent;
                
                if (node != null) {
                    IntPtr next = UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle),
                                               NativeMethods.TVM_GETNEXTITEM, NativeMethods.TVGN_NEXTVISIBLE, node.Handle);
                    if (next != IntPtr.Zero) {
                        return tv.NodeFromHandle(next);
                    }
                }
                
                return null;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.NodeFont"]/*' />
        /// <devdoc>
        ///     The font that will be used to draw this node
        ///     If null, the font used will be the default font from the TreeView control that this
        ///     node is attached to.
        ///     NOTE: If the node font is larger than the default font from the TreeView control, then
        ///     the node will be clipped.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeNodeNodeFontDescr)),
        DefaultValue(null)
        ]
        public Font NodeFont {
            get {
                if (propBag==null) return null;
                return propBag.Font;
            }
            set {
                Font oldfont = this.NodeFont;
                // If we're setting the font to the default again, delete the propBag if it doesn't contain
                // useful data.
                if (value==null) {
                    if (propBag!=null) {
                        propBag.Font = null;
                        RemovePropBagIfEmpty();
                    }
                    if (oldfont != null) InvalidateHostTree();
                    return;
                }

                // Not the default, so if necessary create a new propBag, and fill it with the font

                if (propBag==null) propBag = new OwnerDrawPropertyBag();
                propBag.Font = value;
                if (!value.Equals(oldfont)) InvalidateHostTree();
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Nodes"]/*' />
        [
        ListBindable(false), 
        Browsable(false)
        ]
        public TreeNodeCollection Nodes {
            get {
                if (nodes == null) {
                    nodes = new TreeNodeCollection(this);
                }
                return nodes;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Parent"]/*' />
        /// <devdoc>
        ///     Retrieves parent node.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode Parent {
            get {
                TreeView tv = TreeView;

                // Don't expose the virtual root publicly
                if (tv != null && parent == tv.root) {
                    return null;
                }

                return parent;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.PrevNode"]/*' />
        /// <devdoc>
        ///     The previous sibling node.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode PrevNode {
            get {
                //fixedIndex is used for perf. optimization in case of adding big ranges of nodes
                int currentInd = index;
                int fixedInd = parent.Nodes.FixedIndex;
                
                if (fixedInd > 0) {
	                currentInd = fixedInd;
                }
	
                if (currentInd > 0 && currentInd <= parent.Nodes.Count) {
	                return parent.Nodes[currentInd-1];
                }
                else {  
	                return null;
                }
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.PrevVisibleNode"]/*' />
        /// <devdoc>
        ///     The next visible node.  It may be a parent, sibling,
        ///     or a node from another branch.
        /// </devdoc>
        [Browsable(false)]
        public TreeNode PrevVisibleNode {
            get {
                // TVGN_PREVIOUSVISIBLE can only be sent if the specified node is visible.
                // So before sending, we check if this node is visible. If not, we find the first visible parent.
                //
                TreeNode node = FirstVisibleParent;
                TreeView tv = this.TreeView;
                
                if (node != null) {
                    if (tv == null || tv.IsDisposed) {
                        return null;
                    }
                    IntPtr prev = UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle),
                                               NativeMethods.TVM_GETNEXTITEM,
                                               NativeMethods.TVGN_PREVIOUSVISIBLE, node.Handle);
                    if (prev != IntPtr.Zero) {
                        return tv.NodeFromHandle(prev);
                    }
                }
                
                return null;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.SelectedImageIndex"]/*' />
        /// <devdoc>
        ///     The index of the image displayed when the node is in the selected state.
        ///     The image is contained in the ImageList referenced by the imageList property.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeSelectedImageIndexDescr)),
        TypeConverterAttribute(typeof(TreeViewImageIndexConverter)),
        DefaultValue(-1),
        RefreshProperties(RefreshProperties.Repaint),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RelatedImageList("TreeView.ImageList")
        ]
        public int SelectedImageIndex {
            get {
                return SelectedImageIndexer.Index;
            }
            set {
                SelectedImageIndexer.Index = value;
                UpdateNode(NativeMethods.TVIF_SELECTEDIMAGE);
            }
        }

  	    /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.SelectedImageKey"]/*' />
        /// <devdoc>
        ///     The index of the image displayed when the node is in the selected state.
        ///     The image is contained in the ImageList referenced by the imageList property.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeSelectedImageKeyDescr)),
        TypeConverterAttribute(typeof(TreeViewImageKeyConverter)),
        DefaultValue(""),
        RefreshProperties(RefreshProperties.Repaint),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RelatedImageList("TreeView.ImageList")
        ]
        public string SelectedImageKey {
            get { 
                return SelectedImageIndexer.Key; 
            }
            set {
                SelectedImageIndexer.Key = value;
                UpdateNode(NativeMethods.TVIF_SELECTEDIMAGE);
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.State"]/*' />
        /// <devdoc>
        ///     Retrieve state bits for this node
        /// </devdoc>
        /// <internalonly/>
        internal int State {
            get {
                if (handle == IntPtr.Zero)
                    return 0;

                TreeView tv = this.TreeView;
                if (tv == null || tv.IsDisposed) {
                    return 0;
                }
                NativeMethods.TV_ITEM item = new NativeMethods.TV_ITEM();
                item.hItem = Handle;
                item.mask = NativeMethods.TVIF_HANDLE | NativeMethods.TVIF_STATE;
                item.stateMask = NativeMethods.TVIS_SELECTED | NativeMethods.TVIS_EXPANDED;
                UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_GETITEM, 0, ref item);
                return item.state;
            }
        }



        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ImageIndex"]/*' />
        /// <devdoc>
        ///     The key of the StateImage that the user want to display.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeStateImageKeyDescr)),
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        DefaultValue(""),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        RelatedImageList("TreeView.StateImageList")
        ]
        public string StateImageKey {
            get { 
                return StateImageIndexer.Key; }
            set {
                if (StateImageIndexer.Key != value) {
                    StateImageIndexer.Key = value;
                    if (treeView != null && !treeView.CheckBoxes)
                    {
                        UpdateNode(NativeMethods.TVIF_STATE);
                    }
                }
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.StateImageIndex"]/*' />
        [
        Localizable(true),
        TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter)),
        DefaultValue(-1),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeNodeStateImageIndexDescr)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        RefreshProperties(RefreshProperties.Repaint),
        RelatedImageList("TreeView.StateImageList")
        ]
        public int StateImageIndex {
            get {
                return (treeView == null || treeView.StateImageList == null) ? -1:StateImageIndexer.Index;
            }
            set {
                if (value < -1 || value > ALLOWEDIMAGES) {
                    throw new ArgumentOutOfRangeException(nameof(StateImageIndex), string.Format(SR.InvalidArgument, "StateImageIndex", (value).ToString(CultureInfo.CurrentCulture)));
                }
                StateImageIndexer.Index = value;
                if (treeView != null && !treeView.CheckBoxes)
                {
                    UpdateNode(NativeMethods.TVIF_STATE);
                }
            }
        }

        // <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Tag"]/*' />
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Text"]/*' />
        /// <devdoc>
        ///     The label text for the tree node
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeNodeTextDescr))
        ]
        public string Text {
            get {
                return text == null ? "" : text;
            }
            set {
                this.text = value;
                UpdateNode(NativeMethods.TVIF_TEXT);
            }
        }


        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ToolTipText"]/*' />
        /// <devdoc>
        ///     The ToolTip text that will be displayed when the mouse hovers over the node.
        /// </devdoc>
        [
        Localizable(false),
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeNodeToolTipTextDescr)),
        DefaultValue("")
        ]
        public string ToolTipText {
            get {
                return toolTipText;
            }
            set {
                toolTipText = value;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Name"]/*' />
        /// <devdoc>
        ///     The name for the tree node - useful for indexing.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeNodeNodeNameDescr))
        ]
        public string Name {
            get {
                return name == null ? "" : name;
            }
            set {
                this.name = value;
            }
        }


        

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.TreeView"]/*' />
        /// <devdoc>
        ///     Return the TreeView control this node belongs to.
        /// </devdoc>
        [Browsable(false)]
        public TreeView TreeView {
            get {
                if (treeView == null)
                    treeView = FindTreeView();
                return treeView;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.AddSorted"]/*' />
        /// <devdoc>
        ///     Adds a new child node at the appropriate sorted position
        /// </devdoc>
        /// <internalonly/>
        internal int AddSorted(TreeNode node) {
            int index = 0;
            int iMin, iLim, iT;
            string nodeText = node.Text;
            TreeView parentTreeView = TreeView;
            
            if (childCount > 0) {
                if (parentTreeView.TreeViewNodeSorter == null)
                {
                    CompareInfo compare = Application.CurrentCulture.CompareInfo;

                    // Optimize for the case where they're already sorted
                    if (compare.Compare(children[childCount-1].Text, nodeText) <= 0)
                        index = childCount;
                    else {
                        // Insert at appropriate sorted spot
                        for (iMin = 0, iLim = childCount; iMin < iLim;) {
                            iT = (iMin + iLim) / 2;
                            if (compare.Compare(children[iT].Text, nodeText) <= 0)
                                iMin = iT + 1;
                            else
                                iLim = iT;
                        }
                        index = iMin;
                    }
                }
                else 
                {
                    IComparer sorter = parentTreeView.TreeViewNodeSorter;
                    // Insert at appropriate sorted spot
                    for (iMin = 0, iLim = childCount; iMin < iLim;) {
                        iT = (iMin + iLim) / 2;
                        if (sorter.Compare(children[iT] /*previous*/, node/*current*/) <= 0)
                            iMin = iT + 1;
                        else
                            iLim = iT;
                    }
                    index = iMin;
                }
            }
           
            node.SortChildren(parentTreeView);
            InsertNodeAt(index, node);
            
            return index;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.FromHandle"]/*' />
        /// <devdoc>
        ///     Returns a TreeNode object for the given HTREEITEM handle
        /// </devdoc>
        public static TreeNode FromHandle(TreeView tree, IntPtr handle) {
            // 

            IntSecurity.ControlFromHandleOrLocation.Demand();
            return tree.NodeFromHandle(handle);
        }

        private void SortChildren(TreeView parentTreeView) {
            // 
            if (childCount > 0) {
                TreeNode[] newOrder = new TreeNode[childCount];
                if (parentTreeView == null || parentTreeView.TreeViewNodeSorter == null)
                {
                    CompareInfo compare = Application.CurrentCulture.CompareInfo;
                    for (int i = 0; i < childCount; i++) {
                        int min = -1;
                        for (int j = 0; j < childCount; j++) {
                            if (children[j] == null)
                                continue;
                            if (min == -1) {
                                min = j;
                                continue;
                            }
                            if (compare.Compare(children[j].Text, children[min].Text) <= 0)
                                min = j;
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
                    for (int i = 0; i < childCount; i++) {
                        int min = -1;
                        for (int j = 0; j < childCount; j++) {
                            if (children[j] == null)
                                continue;
                            if (min == -1) {
                                min = j;
                                continue;
                            }
                            if (sorter.Compare(children[j] /*previous*/, children[min] /*current*/) <= 0)
                                min = j;
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


        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.BeginEdit"]/*' />
        /// <devdoc>
        ///     Initiate editing of the node's label.
        ///     Only effective if LabelEdit property is true.
        /// </devdoc>
        public void BeginEdit() {
            if (handle != IntPtr.Zero) {
                TreeView tv = TreeView;
                if (tv.LabelEdit == false)
                    throw new InvalidOperationException(SR.TreeNodeBeginEditFailed);
                if (!tv.Focused)
                    tv.FocusInternal();
                UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_EDITLABEL, 0, handle);
            }
        }
        
        /// <devdoc>
        ///     Called by the tree node collection to clear all nodes.  We optimize here if
        ///     this is the root node.
        /// </devdoc>
        internal void Clear() {
            
            
            // This is a node that is a child of some other node.  We have
            // to selectively remove children here.
            //
            bool isBulkOperation = false;
            TreeView tv = TreeView;


            
            try                     
            {  

                if (tv != null) {
                    tv.nodesCollectionClear = true;
                    
                    if (tv != null && childCount > MAX_TREENODES_OPS) {
                        isBulkOperation = true;
                        tv.BeginUpdate();
                    }
                }

                while(childCount > 0) {
                    children[childCount - 1].Remove(true);
                }
                children = null;


                if (tv != null && isBulkOperation) {
                    tv.EndUpdate();                        
                }
            }
            finally
            {
                if (tv != null) {
                    tv.nodesCollectionClear = false;
                }
                nodesCleared = true;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Clone"]/*' />
        /// <devdoc>
        ///     Clone the entire subtree rooted at this node.
        /// </devdoc>
        public virtual object Clone() {
            Type clonedType = this.GetType();
            TreeNode node = null;

            if (clonedType == typeof(TreeNode)){ 
                node = new TreeNode(text, ImageIndexer.Index, SelectedImageIndexer.Index);
            }
            else {
                node = (TreeNode)Activator.CreateInstance(clonedType);
            }
            
            node.Text = text;
            node.Name = name;
            node.ImageIndexer.Index = ImageIndexer.Index;
            node.SelectedImageIndexer.Index = SelectedImageIndexer.Index;
            
            node.StateImageIndexer.Index = StateImageIndexer.Index;
            node.ToolTipText = toolTipText;
            node.ContextMenu = contextMenu;
            node.ContextMenuStrip = contextMenuStrip;

            // only set the key if it's set to something useful
            if ( ! (string.IsNullOrEmpty(ImageIndexer.Key))) {
                node.ImageIndexer.Key = ImageIndexer.Key;
            }

            // only set the key if it's set to something useful
            if (!(string.IsNullOrEmpty(SelectedImageIndexer.Key))) {
                node.SelectedImageIndexer.Key = SelectedImageIndexer.Key;
            }

            // only set the key if it's set to something useful
            if (!(string.IsNullOrEmpty(StateImageIndexer.Key))) {
                node.StateImageIndexer.Key = StateImageIndexer.Key;
            }
            
            if (childCount > 0) {
                node.children = new TreeNode[childCount];
                for (int i = 0; i < childCount; i++)
                    node.Nodes.Add((TreeNode)children[i].Clone());
            }
            
            // Clone properties
            //
            if (propBag != null) {                 
                node.propBag = OwnerDrawPropertyBag.Copy(propBag);
            }
            node.Checked = this.Checked;
            node.Tag = this.Tag;
            
            return node;
        }

        private void CollapseInternal(bool ignoreChildren)
        {
            TreeView tv = TreeView;
            bool setSelection = false;
            collapseOnRealization = false;
            expandOnRealization = false;

            if (tv == null || !tv.IsHandleCreated) {
                collapseOnRealization = true;
                return;
            }

            //terminating condition for recursion...
            //
            if (ignoreChildren)
            {
                DoCollapse(tv);
            }
            else {
                if (!ignoreChildren && childCount > 0) {
                    // Virtual root should collapse all its children
                    for (int i = 0; i < childCount; i++) {
                        if (tv.SelectedNode == children[i]) {
                            setSelection = true;
                        }
                    children[i].DoCollapse(tv);
                    children[i].Collapse();
                    }
                }
                DoCollapse(tv);
            }

            if (setSelection)
                tv.SelectedNode = this;
            tv.Invalidate();
            collapseOnRealization = false;
            
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Collapse"]/*' />
        /// <devdoc>
        ///     Collapse the node ignoring its children while collapsing the parent
        /// </devdoc>
        public void Collapse(bool ignoreChildren)
        {
            CollapseInternal(ignoreChildren);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Collapse"]/*' />
        /// <devdoc>
        ///     Collapse the node.
        /// </devdoc>
        public void Collapse() {
            CollapseInternal(false);    
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.DoCollapse"]/*' />
        /// <devdoc>
        ///     Windows TreeView doesn't send the proper notifications on collapse, so we do it manually.
        /// </devdoc>
        private void DoCollapse(TreeView tv) {
            if ((State & NativeMethods.TVIS_EXPANDED) != 0) {
                TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(this, false, TreeViewAction.Collapse);
                tv.OnBeforeCollapse(e);
                if (!e.Cancel) {
                    UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_EXPAND, NativeMethods.TVE_COLLAPSE, Handle);
                    tv.OnAfterCollapse(new TreeViewEventArgs(this));
                }
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Deserialize"]/*' />
        protected virtual void Deserialize(SerializationInfo serializationInfo, StreamingContext context) {

            int childCount = 0;
            int imageIndex = -1;
            string imageKey = null;

            int selectedImageIndex = -1;
            string selectedImageKey = null;

            int stateImageIndex = -1;
            string stateImageKey = null;
            
            foreach (SerializationEntry entry in serializationInfo) {
                switch (entry.Name) {
                    case "PropBag":
                        // this would throw a InvalidaCastException if improper cast, thus validating the serializationInfo for OwnerDrawPropertyBag
                        propBag = (OwnerDrawPropertyBag)serializationInfo.GetValue(entry.Name, typeof(OwnerDrawPropertyBag));
                        break;
                    case "Text":
                        Text = serializationInfo.GetString(entry.Name);
                        break;
                    case "ToolTipText":
                        ToolTipText = serializationInfo.GetString(entry.Name);
                        break;
                    case "Name":
                        Name = serializationInfo.GetString(entry.Name);
                        break;
                    case "IsChecked":
                        CheckedStateInternal = serializationInfo.GetBoolean(entry.Name);
                        break;
                    case "ImageIndex":
                        imageIndex = serializationInfo.GetInt32(entry.Name);
                        break;
                    case "SelectedImageIndex":
                        selectedImageIndex = serializationInfo.GetInt32(entry.Name);
                        break;
                    case "ImageKey":
                        imageKey = serializationInfo.GetString(entry.Name);
                        break;
                    case "SelectedImageKey":
                        selectedImageKey= serializationInfo.GetString(entry.Name);
                        break;                    
                    case "StateImageKey":
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
            if (imageKey != null) {
                ImageKey = imageKey;    
            } 
            else if (imageIndex != -1) {
                ImageIndex = imageIndex;
            }

            // let selectedimagekey take precidence
            if (selectedImageKey != null) {
                SelectedImageKey = selectedImageKey;    
            } 
            else if (selectedImageIndex != -1) {
                SelectedImageIndex = selectedImageIndex;
            }

            // let stateimagekey take precidence
            if (stateImageKey != null) {
                StateImageKey = stateImageKey;    
            } 
            else if (stateImageIndex != -1) {
                StateImageIndex = stateImageIndex;
            }

            if (childCount > 0) {
                TreeNode[] childNodes = new TreeNode[childCount];

                for (int i = 0; i < childCount; i++) {
                    childNodes[i] = (TreeNode)serializationInfo.GetValue("children" + i, typeof(TreeNode));
                }
                Nodes.AddRange(childNodes);
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.EndEdit"]/*' />
        /// <devdoc>
        ///     Terminate the editing of any tree view item's label.
        /// </devdoc>
        public void EndEdit(bool cancel) {
            TreeView tv = this.TreeView;
            if (tv == null || tv.IsDisposed) {
                return;
            }
            UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_ENDEDITLABELNOW, cancel?1:0, 0);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.EnsureCapacity"]/*' />
        /// <devdoc>
        ///     Makes sure there is enough room to add n children
        /// </devdoc>
        /// <internalonly/>
        internal void EnsureCapacity(int num) {
            Debug.Assert(num > 0,"required capacity can not be less than 1");
            int size = num;
            if (size < 4) {
                size = 4;
            }
            if (children == null) {
                children = new TreeNode[size];
            }
            else if (childCount + num > children.Length) {
                int newSize =  childCount + num;
                if (num == 1) {
                    newSize =  childCount * 2;
                }
                TreeNode[] bigger = new TreeNode[newSize];
                System.Array.Copy(children, 0, bigger, 0, childCount);
                children = bigger;
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.EnsureStateImageValue"]/*' />
        /// <devdoc>
        ///     Ensures the the node's StateImageIndex value is properly set.
        /// </devdoc>
        /// <internalonly/>
        private void EnsureStateImageValue()
        {
            if (treeView == null) {
                return;
            }

            if (treeView.CheckBoxes && treeView.StateImageList != null) {

               if (!String.IsNullOrEmpty(this.StateImageKey)) {
                  this.StateImageIndex = (this.Checked) ? 1 : 0;
                  this.StateImageKey = treeView.StateImageList.Images.Keys[this.StateImageIndex];
               }
               else {
                  this.StateImageIndex = (this.Checked) ? 1 : 0;
               }
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.EnsureVisible"]/*' />
        /// <devdoc>
        ///     Ensure that the node is visible, expanding nodes and scrolling the
        ///     TreeView control as necessary.
        /// </devdoc>
        public void EnsureVisible() {
            TreeView tv = this.TreeView;
            if (tv == null || tv.IsDisposed) {
                return;
            }
            UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_ENSUREVISIBLE, 0, Handle);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Expand"]/*' />
        /// <devdoc>
        ///     Expand the node.
        /// </devdoc>
        public void Expand() {
            TreeView tv = TreeView;
            if (tv == null || !tv.IsHandleCreated) {
                expandOnRealization = true;
                return;
            }

            ResetExpandedState(tv);
            if (!IsExpanded) {
                UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_EXPAND, NativeMethods.TVE_EXPAND, Handle);
            }
            expandOnRealization = false;
        }


        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ExpandAll"]/*' />
        /// <devdoc>
        ///     Expand the node.
        /// </devdoc>
        public void ExpandAll() {
            Expand();
            for (int i = 0; i < childCount; i++) {
                 children[i].ExpandAll();
            }
            
        }
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.FindTreeView"]/*' />
        /// <devdoc>
        ///     Locate this tree node's containing tree view control by scanning
        ///     up to the virtual root, whose treeView pointer we know to be
        ///     correct
        /// </devdoc>
        internal TreeView FindTreeView() {
            TreeNode node = this;
            while (node.parent != null)
                node = node.parent;
            return node.treeView;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.GetFullPath"]/*' />
        /// <devdoc>
        ///     Helper function for getFullPath().
        /// </devdoc>
        private void GetFullPath(StringBuilder path, string pathSeparator) {
            if (parent != null) {
                parent.GetFullPath(path, pathSeparator);
                if (parent.parent != null)
                    path.Append(pathSeparator);
                path.Append(this.text);
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.GetNodeCount"]/*' />
        /// <devdoc>
        ///     Returns number of child nodes.
        /// </devdoc>
        public int GetNodeCount(bool includeSubTrees) {
            int total = childCount;
            if (includeSubTrees) {
                for (int i = 0; i < childCount; i++)
                    total += children[i].GetNodeCount(true);
            }
            return total;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.InsertNodeAt"]/*' />
        /// <devdoc>
        ///     Helper function to add node at a given index after all validation has been done
        /// </devdoc>
        /// <internalonly/>
        internal void InsertNodeAt(int index, TreeNode node) {
            EnsureCapacity(1);
            node.parent = this;
            node.index = index;
            for (int i = childCount; i > index; --i) {
                (children[i] = children[i-1]).index = i;
            }
            children[index] = node;
            childCount++;
            node.Realize(false);

            if (TreeView != null && node == TreeView.selectedNode)
                TreeView.SelectedNode = node; // communicate this to the handle
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.InvalidateHostTree"]/*' />
        /// <devdoc>
        ///     Invalidates the treeview control that is hosting this node
        /// </devdoc>
        private void InvalidateHostTree() {
            if (treeView != null && treeView.IsHandleCreated) treeView.Invalidate();
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Realize"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void Realize(bool insertFirst) {
            // Debug.assert(handle == 0, "Node already realized");
            TreeView tv = this.TreeView;
            if (tv == null || !tv.IsHandleCreated || tv.IsDisposed)
                return;

            if (parent != null) { // Never realize the virtual root

                if (tv.InvokeRequired) {
                    throw new InvalidOperationException(SR.InvalidCrossThreadControlCall);
                }

                NativeMethods.TV_INSERTSTRUCT tvis = new NativeMethods.TV_INSERTSTRUCT();
                tvis.item_mask = insertMask;
                tvis.hParent = parent.handle;
                TreeNode prev = PrevNode;
                if (insertFirst || prev == null) {
                    tvis.hInsertAfter = (IntPtr)NativeMethods.TVI_FIRST;
                }
                else {
                    tvis.hInsertAfter = prev.handle;
                    // Debug.assert(tvis.hInsertAfter != 0);
                }

                tvis.item_pszText = Marshal.StringToHGlobalAuto(text);
                tvis.item_iImage = (ImageIndexer.ActualIndex == -1) ? tv.ImageIndexer.ActualIndex : ImageIndexer.ActualIndex;
                tvis.item_iSelectedImage = (SelectedImageIndexer.ActualIndex == -1) ? tv.SelectedImageIndexer.ActualIndex : SelectedImageIndexer.ActualIndex;
                tvis.item_mask = NativeMethods.TVIF_TEXT;

                tvis.item_stateMask = 0;
                tvis.item_state = 0;

                if (tv.CheckBoxes) {
                    tvis.item_mask |= NativeMethods.TVIF_STATE;
                    tvis.item_stateMask |= NativeMethods.TVIS_STATEIMAGEMASK;
                    tvis.item_state |= CheckedInternal ? CHECKED : UNCHECKED;
                }
                else if (tv.StateImageList != null && StateImageIndexer.ActualIndex >= 0) {
                    tvis.item_mask |= NativeMethods.TVIF_STATE;
                    tvis.item_stateMask = NativeMethods.TVIS_STATEIMAGEMASK;
                    tvis.item_state = ((StateImageIndexer.ActualIndex + 1) << SHIFTVAL);
                }


                if (tvis.item_iImage >= 0) tvis.item_mask |= NativeMethods.TVIF_IMAGE;
                if (tvis.item_iSelectedImage >= 0) tvis.item_mask |= NativeMethods.TVIF_SELECTEDIMAGE;

                // If you are editing when you add a new node, then the edit control
                // gets placed in the wrong place. You must restore the edit mode
                // asynchronously (PostMessage) after the add is complete
                // to get the expected behavior.
                //
                bool editing = false;
                IntPtr editHandle = UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_GETEDITCONTROL, 0, 0);
                if (editHandle != IntPtr.Zero) {
                    // currently editing...
                    //
                    editing = true;
                    UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_ENDEDITLABELNOW, 0 /* fCancel==FALSE */, 0);
                }

                handle = UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_INSERTITEM, 0, ref tvis);
                tv.nodeTable[handle] = this;

                // Lets update the Lparam to the Handle ....
                UpdateNode(NativeMethods.TVIF_PARAM);

                Marshal.FreeHGlobal(tvis.item_pszText);

                if (editing) {
                    UnsafeNativeMethods.PostMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_EDITLABEL, IntPtr.Zero, handle);
                }

                SafeNativeMethods.InvalidateRect(new HandleRef(tv, tv.Handle), null, false);

                if (parent.nodesCleared && (insertFirst || prev == null) && !tv.Scrollable) {
                    // We need to Redraw the TreeView ...
                    // If and only If we are not scrollable ... 
                    // and this is the FIRST NODE to get added..
                    // This is Comctl quirk where it just doesn't draw
                    // the first node after a Clear( ) if Scrollable == false.
                    UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.WM_SETREDRAW, 1, 0);
                    nodesCleared = false;
                }

            }

            for (int i = childCount - 1; i >= 0; i--)
                children[i].Realize(true);

            // If node expansion was requested before the handle was created,
            // we can expand it now.
            if (expandOnRealization) {
                Expand();
            }

            // If node collapse was requested before the handle was created,
            // we can expand it now.
            if (collapseOnRealization) {
                Collapse();
            }
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Remove"]/*' />
        /// <devdoc>
        ///     Remove this node from the TreeView control.  Child nodes are also removed from the
        ///     TreeView, but are still attached to this node.
        /// </devdoc>
        public void Remove() {
            Remove(true);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Remove1"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal void Remove(bool notify) {
            bool expanded = IsExpanded;

            // unlink our children
            // 

            for (int i = 0; i < childCount; i++)
                children[i].Remove(false);
            // children = null;
            // unlink ourself
            if (notify && parent != null) {
                for (int i = index; i < parent.childCount-1; ++i) {
                    (parent.children[i] = parent.children[i+1]).index = i;
                }

                parent.children[parent.childCount - 1] = null;
                parent.childCount--;
                parent = null;
            }
            // Expand when we are realized the next time.
            expandOnRealization = expanded;

            // unrealize ourself
            TreeView tv = this.TreeView;
            if (tv == null || tv.IsDisposed) {
                return;
            }

            if (handle != IntPtr.Zero) {
                if (notify && tv.IsHandleCreated)
                    UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_DELETEITEM, 0, handle);
                treeView.nodeTable.Remove(handle);
                handle = IntPtr.Zero;
            }
            treeView = null;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.RemovePropBagIfEmpty"]/*' />
        /// <devdoc>
        ///     Removes the propBag object if it's now devoid of useful data
        /// </devdoc>
        /// <internalonly/>
        private void RemovePropBagIfEmpty() {
            if (propBag==null) return;
            if (propBag.IsEmpty()) propBag = null;
            return;
        }

        private void ResetExpandedState(TreeView tv) {
            Debug.Assert(tv.IsHandleCreated, "nonexistent handle");

            NativeMethods.TV_ITEM item = new NativeMethods.TV_ITEM();
            item.mask = NativeMethods.TVIF_HANDLE | NativeMethods.TVIF_STATE;
            item.hItem = handle;
            item.stateMask = NativeMethods.TVIS_EXPANDEDONCE;
            item.state = 0;
            UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_SETITEM, 0, ref item);
        }

        private bool ShouldSerializeBackColor() {
            return BackColor != Color.Empty;
        }

        private bool ShouldSerializeForeColor() {
            return ForeColor != Color.Empty;
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Serialize"]/*' />
        /// <devdoc>
        ///     Saves this TreeNode object to the given data stream.
        /// </devdoc>
        /// Review: Changing this would break VB users. so suppresing this message.
        /// 
     	[SecurityPermissionAttribute(SecurityAction.Demand, Flags=SecurityPermissionFlag.SerializationFormatter), 		
         SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
        protected virtual void Serialize(SerializationInfo si, StreamingContext context) {
            if (propBag != null) {
                si.AddValue("PropBag", propBag, typeof(OwnerDrawPropertyBag));
            }

            si.AddValue("Text", text);
            si.AddValue("ToolTipText", toolTipText);
            si.AddValue("Name", Name);
            si.AddValue("IsChecked", treeNodeState[TREENODESTATE_isChecked]);
            si.AddValue("ImageIndex", ImageIndexer.Index);
            si.AddValue("ImageKey", ImageIndexer.Key);
            si.AddValue("SelectedImageIndex", SelectedImageIndexer.Index);
            si.AddValue("SelectedImageKey", SelectedImageIndexer.Key);

            if (this.treeView != null && this.treeView.StateImageList != null) {
               si.AddValue("StateImageIndex", StateImageIndexer.Index);
            }

            if (this.treeView != null && this.treeView.StateImageList != null) {
               si.AddValue("StateImageKey", StateImageIndexer.Key);
            }

            si.AddValue("ChildCount",  childCount);
            
            if (childCount > 0) {
                for (int i = 0; i < childCount; i++) {
                    si.AddValue("children" + i, children[i], typeof(TreeNode));
                }
            }
            
            if (userData != null && userData.GetType().IsSerializable) {
                si.AddValue("UserData", userData, userData.GetType());
            }
        }
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.Toggle"]/*' />
        /// <devdoc>
        ///     Toggle the state of the node. Expand if collapsed or collapse if
        ///     expanded.
        /// </devdoc>
        public void Toggle() {
            Debug.Assert(parent != null, "toggle on virtual root");

            // I don't use the TVE_TOGGLE message 'cuz Windows TreeView doesn't send the appropriate
            // notifications when collapsing.
            if (IsExpanded) {
                Collapse();
            }
            else {
                Expand();
            }
        }

        
        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ToString"]/*' />
        /// <devdoc>
        ///     Returns the label text for the tree node
        /// </devdoc>
        public override string ToString() {
            return "TreeNode: " + (text == null ? "" : text);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.UpdateNode"]/*' />
        /// <devdoc>
        ///     Tell the TreeView to refresh this node
        /// </devdoc>
        private void UpdateNode(int mask) {
            if (handle == IntPtr.Zero) return;
            TreeView tv = TreeView;
            Debug.Assert(tv != null, "TreeNode has handle but no TreeView");

            NativeMethods.TV_ITEM item = new NativeMethods.TV_ITEM();
            item.mask = NativeMethods.TVIF_HANDLE | mask;
            item.hItem = handle;
            if ((mask & NativeMethods.TVIF_TEXT) != 0)
                item.pszText = Marshal.StringToHGlobalAuto(text);
            if ((mask & NativeMethods.TVIF_IMAGE) != 0)
                item.iImage = (ImageIndexer.ActualIndex == -1) ? tv.ImageIndexer.ActualIndex : ImageIndexer.ActualIndex;
            if ((mask & NativeMethods.TVIF_SELECTEDIMAGE) != 0)
                item.iSelectedImage = (SelectedImageIndexer.ActualIndex == -1) ? tv.SelectedImageIndexer.ActualIndex : SelectedImageIndexer.ActualIndex;
            if ((mask & NativeMethods.TVIF_STATE) != 0) {
                item.stateMask = NativeMethods.TVIS_STATEIMAGEMASK;
                if (StateImageIndexer.ActualIndex != -1) {
                    item.state = ((StateImageIndexer.ActualIndex + 1) << SHIFTVAL);
                }
                // ActualIndex == -1 means "don't use custom image list"
                // so just leave item.state set to zero, that tells the unmanaged control
                // to use no state image for this node.
            }
            if ((mask & NativeMethods.TVIF_PARAM) != 0) {
                item.lParam = handle; 
            }

            UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_SETITEM, 0, ref item);
            if ((mask & NativeMethods.TVIF_TEXT) != 0) {
                Marshal.FreeHGlobal(item.pszText);
                if (tv.Scrollable)
                    tv.ForceScrollbarUpdate(false);
            }
        }

        internal void UpdateImage ()
        {
            TreeView tv = this.TreeView;
            if (tv.IsDisposed) {
                return;
            }

            NativeMethods.TV_ITEM item = new NativeMethods.TV_ITEM();

            item.mask = NativeMethods.TVIF_HANDLE | NativeMethods.TVIF_IMAGE;
            item.hItem = Handle;
            item.iImage = Math.Max(0, ((ImageIndexer.ActualIndex >= tv.ImageList.Images.Count) ? tv.ImageList.Images.Count - 1 : ImageIndexer.ActualIndex));
            UnsafeNativeMethods.SendMessage(new HandleRef(tv, tv.Handle), NativeMethods.TVM_SETITEM, 0, ref item);
        }

        /// <include file='doc\TreeNode.uex' path='docs/doc[@for="TreeNode.ISerializable.GetObjectData"]/*' />
        /// <devdoc>
        /// ISerializable private implementation
        /// </devdoc>
        /// <internalonly/>
    	[SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)] 		
        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context) {
             Serialize(si, context);
        }
    }
}

