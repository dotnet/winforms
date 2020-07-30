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
using System.Windows.Forms.Layout;
using System.Windows.Forms.VisualStyles;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays a hierarchical list of items, or nodes. Each
    ///  node includes a caption and an optional bitmap. The user can select a node. If
    ///  it has sub-nodes, the user can collapse or expand the node.
    /// </summary>
    [DefaultProperty(nameof(Nodes))]
    [DefaultEvent(nameof(AfterSelect))]
    [Docking(DockingBehavior.Ask)]
    [Designer("System.Windows.Forms.Design.TreeViewDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionTreeView))]
    public class TreeView : Control
    {
        private const int MaxIndent = 32000;      // Maximum allowable TreeView indent
        private const string backSlash = "\\";
        private const int DefaultTreeViewIndent = 19;

        private DrawTreeNodeEventHandler onDrawNode;
        private NodeLabelEditEventHandler onBeforeLabelEdit;
        private NodeLabelEditEventHandler onAfterLabelEdit;
        private TreeViewCancelEventHandler onBeforeCheck;
        private TreeViewEventHandler onAfterCheck;
        private TreeViewCancelEventHandler onBeforeCollapse;
        private TreeViewEventHandler onAfterCollapse;
        private TreeViewCancelEventHandler onBeforeExpand;
        private TreeViewEventHandler onAfterExpand;
        private TreeViewCancelEventHandler onBeforeSelect;
        private TreeViewEventHandler onAfterSelect;
        private ItemDragEventHandler onItemDrag;
        private TreeNodeMouseHoverEventHandler onNodeMouseHover;
        private EventHandler onRightToLeftLayoutChanged;

        internal TreeNode selectedNode;
        private ImageList.Indexer imageIndexer;
        private ImageList.Indexer selectedImageIndexer;
        private bool setOddHeight;
        private TreeNode prevHoveredNode;
        private bool hoveredAlready;
        private bool rightToLeftLayout;

        private IntPtr hNodeMouseDown = IntPtr.Zero;//ensures we fire nodeclick on the correct node

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

        // PERF: take all the bools and put them into a state variable
        private Collections.Specialized.BitVector32 treeViewState; // see TREEVIEWSTATE_ consts above

        private static bool isScalingInitialized;
        private static Size? scaledStateImageSize;
        private static Size? ScaledStateImageSize
        {
            get
            {
                if (!isScalingInitialized)
                {
                    if (DpiHelper.IsScalingRequired)
                    {
                        scaledStateImageSize = DpiHelper.LogicalToDeviceUnits(new Size(16, 16));
                    }
                    isScalingInitialized = true;
                }
                return scaledStateImageSize;
            }
        }

        internal ImageList.Indexer ImageIndexer
        {
            get
            {
                if (imageIndexer is null)
                {
                    imageIndexer = new ImageList.Indexer();
                }
                imageIndexer.ImageList = ImageList;
                return imageIndexer;
            }
        }

        internal ImageList.Indexer SelectedImageIndexer
        {
            get
            {
                if (selectedImageIndexer is null)
                {
                    selectedImageIndexer = new ImageList.Indexer();
                }
                selectedImageIndexer.ImageList = ImageList;

                return selectedImageIndexer;
            }
        }

        private ImageList imageList;
        private int indent = -1;
        private int itemHeight = -1;
        private string pathSeparator = backSlash;
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.Fixed3D;

        internal TreeNodeCollection nodes;
        internal TreeNode editNode;
        internal TreeNode root;
        internal Hashtable nodeTable = new Hashtable();
        internal bool nodesCollectionClear; //this is set when the treeNodeCollection is getting cleared and used by TreeView
        private MouseButtons downButton;
        private TreeViewDrawMode drawMode = TreeViewDrawMode.Normal;

        //Properties newly added to TreeView....
        private ImageList internalStateImageList;
        private TreeNode topNode;
        private ImageList stateImageList;
        private Color lineColor;
        private string controlToolTipText;

        // Sorting
        private IComparer treeViewNodeSorter;

        //Events
        private TreeNodeMouseClickEventHandler onNodeMouseClick;
        private TreeNodeMouseClickEventHandler onNodeMouseDoubleClick;

        private ToolTipBuffer _toolTipBuffer;

        /// <summary>
        ///  Creates a TreeView control
        /// </summary>
        public TreeView()
        : base()
        {
            treeViewState = new Collections.Specialized.BitVector32(TREEVIEWSTATE_showRootLines |
                                                                                TREEVIEWSTATE_showPlusMinus |
                                                                                TREEVIEWSTATE_showLines |
                                                                                TREEVIEWSTATE_scrollable |
                                                                                TREEVIEWSTATE_hideSelection);

            root = new TreeNode(this);

            // TreeView must always have an ImageIndex.
            SelectedImageIndexer.Index = 0;
            ImageIndexer.Index = 0;

            SetStyle(ControlStyles.UserPaint, false);
            SetStyle(ControlStyles.StandardClick, false);
            SetStyle(ControlStyles.UseTextForAccessibility, false);
        }

        /// <summary>
        ///  The background color for this control. Specifying null for
        ///  this parameter sets the
        ///  control's background color to its parent's background color.
        /// </summary>
        public override Color BackColor
        {
            get
            {
                if (ShouldSerializeBackColor())
                {
                    return base.BackColor;
                }
                else
                {
                    return SystemColors.Window;
                }
            }

            set
            {
                base.BackColor = value;
                if (IsHandleCreated)
                {
                    User32.SendMessageW(this, (User32.WM)TVM.SETBKCOLOR, IntPtr.Zero, PARAM.FromColor(BackColor));

                    // This is to get around a problem in the comctl control where the lines
                    // connecting nodes don't get the new BackColor.  This messages forces
                    // reconstruction of the line bitmaps without changing anything else.
                    User32.SendMessageW(this, (User32.WM)TVM.SETINDENT, (IntPtr)Indent);
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set => base.BackgroundImage = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged
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
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        /// <summary>
        ///  The border style of the window.
        /// </summary>
        [SRCategory(nameof(SR.CatAppearance))]
        [DefaultValue(BorderStyle.Fixed3D)]
        [DispId((int)Ole32.DispatchID.BORDERSTYLE)]
        [SRDescription(nameof(SR.borderStyleDescr))]
        public BorderStyle BorderStyle
        {
            get => borderStyle;
            set
            {
                if (borderStyle != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    borderStyle = value;
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
            get
            {
                return treeViewState[TREEVIEWSTATE_checkBoxes];
            }

            set
            {
                if (CheckBoxes != value)
                {
                    treeViewState[TREEVIEWSTATE_checkBoxes] = value;
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
                            UpdateCheckedState(root, false);
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
                cp.ClassName = WindowClasses.WC_TREEVIEW;

                // Keep the scrollbar if we are just updating styles...
                //
                if (IsHandleCreated)
                {
                    int currentStyle = unchecked((int)((long)User32.GetWindowLong(this, User32.GWL.STYLE)));
                    cp.Style |= currentStyle & (int)(User32.WS.HSCROLL | User32.WS.VSCROLL);
                }
                switch (borderStyle)
                {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= (int)User32.WS_EX.CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= (int)User32.WS.BORDER;
                        break;
                }

                if (!Scrollable)
                {
                    cp.Style |= (int)LVS.NOSCROLL;
                }

                if (!HideSelection)
                {
                    cp.Style |= (int)TVS.SHOWSELALWAYS;
                }

                if (LabelEdit)
                {
                    cp.Style |= (int)TVS.EDITLABELS;
                }

                if (ShowLines)
                {
                    cp.Style |= (int)TVS.HASLINES;
                }

                if (ShowPlusMinus)
                {
                    cp.Style |= (int)TVS.HASBUTTONS;
                }

                if (ShowRootLines)
                {
                    cp.Style |= (int)TVS.LINESATROOT;
                }

                if (HotTracking)
                {
                    cp.Style |= (int)TVS.TRACKSELECT;
                }

                if (FullRowSelect)
                {
                    cp.Style |= (int)TVS.FULLROWSELECT;
                }

                if (setOddHeight)
                {
                    cp.Style |= (int)TVS.NONEVENHEIGHT;
                }

                // Don't set TVS_CHECKBOXES here if the window isn't created yet.
                // See OnHandleCreated for explanation
                if (ShowNodeToolTips && IsHandleCreated && !DesignMode)
                {
                    cp.Style |= (int)TVS.INFOTIP;
                }

                // Don't set TVS_CHECKBOXES here if the window isn't created yet.
                // See OnHandleCreated for explanation
                if (CheckBoxes && IsHandleCreated)
                {
                    cp.Style |= (int)TVS.CHECKBOXES;
                }

                // Don't call IsMirrored from CreateParams. That will lead to some nasty problems, since
                // IsMirrored ends up calling CreateParams - you dig!
                if (RightToLeft == RightToLeft.Yes)
                {
                    if (RightToLeftLayout)
                    {
                        //We want to turn on mirroring for TreeView explicitly.
                        cp.ExStyle |= (int)User32.WS_EX.LAYOUTRTL;
                        //Don't need these styles when mirroring is turned on.
                        cp.ExStyle &= ~(int)(User32.WS_EX.RTLREADING | User32.WS_EX.RIGHT | User32.WS_EX.LEFTSCROLLBAR);
                    }
                    else
                    {
                        cp.Style |= (int)TVS.RTLREADING;
                    }
                }

                return cp;
            }
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected override Size DefaultSize
        {
            get
            {
                return new Size(121, 97);
            }
        }

        /// <summary>
        ///  This property is overridden and hidden from statement completion
        ///  on controls that are based on Win32 Native Controls.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        /// <summary>
        ///  The current foreground color for this control, which is the
        ///  color the control uses to draw its text.
        /// </summary>
        public override Color ForeColor
        {
            get
            {
                if (ShouldSerializeForeColor())
                {
                    return base.ForeColor;
                }
                else
                {
                    return SystemColors.WindowText;
                }
            }

            set
            {
                base.ForeColor = value;
                if (IsHandleCreated)
                {
                    User32.SendMessageW(this, (User32.WM)TVM.SETTEXTCOLOR, IntPtr.Zero, PARAM.FromColor(ForeColor));
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
            get { return treeViewState[TREEVIEWSTATE_fullRowSelect]; }
            set
            {
                if (FullRowSelect != value)
                {
                    treeViewState[TREEVIEWSTATE_fullRowSelect] = value;
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
            get
            {
                return treeViewState[TREEVIEWSTATE_hideSelection];
            }

            set
            {
                if (HideSelection != value)
                {
                    treeViewState[TREEVIEWSTATE_hideSelection] = value;
                    if (IsHandleCreated)
                    {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <summary>
        ///  The value of the HotTracking property. The HotTracking
        ///  property determines if nodes are highlighted as the mousepointer
        ///  passes over them.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TreeViewHotTrackingDescr))]
        public bool HotTracking
        {
            get
            {
                return treeViewState[TREEVIEWSTATE_hotTracking];
            }

            set
            {
                if (HotTracking != value)
                {
                    treeViewState[TREEVIEWSTATE_hotTracking] = value;
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.TreeViewImageIndexDescr))]
        [RelatedImageList("ImageList")]
        public int ImageIndex
        {
            get
            {
                if (imageList is null)
                {
                    return ImageList.Indexer.DefaultIndex;
                }
                if (ImageIndexer.Index >= imageList.Images.Count)
                {
                    return Math.Max(0, imageList.Images.Count - 1);
                }
                return ImageIndexer.Index;
            }

            set
            {
                // If (none) is selected in the image index editor, we'll just adjust this to
                // mean image index 0. This is because a treeview must always have an image index -
                // even if no imagelist exists we want the image index to be 0.
                //
                if (value == ImageList.Indexer.DefaultIndex)
                {
                    value = 0;
                }

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ImageIndex), value, 0));
                }

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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(ImageList.Indexer.DefaultKey)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.TreeViewImageKeyDescr))]
        [RelatedImageList("ImageList")]
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
                    if (string.IsNullOrEmpty(value) || value.Equals(SR.toStringNone))
                    {
                        ImageIndex = (ImageList != null) ? 0 : ImageList.Indexer.DefaultIndex;
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
                    DetachImageListHandlers();

                    imageList = value;

                    AttachImageListHandlers();

                    // Update TreeView's images
                    //
                    if (IsHandleCreated)
                    {
                        User32.SendMessageW(this, (User32.WM)TVM.SETIMAGELIST, IntPtr.Zero,
                                    value is null ? IntPtr.Zero : value.Handle);
                        if (StateImageList != null && StateImageList.Images.Count > 0 && internalStateImageList != null)
                        {
                            SetStateImageList(internalStateImageList.CreateUniqueHandle());
                        }
                    }
                    UpdateCheckedState(root, true);
                }
            }
        }

        private void AttachImageListHandlers()
        {
            if (imageList != null)
            {
                //NOTE: any handlers added here should be removed in DetachImageListHandlers
                imageList.RecreateHandle += new EventHandler(ImageListRecreateHandle);
                imageList.Disposed += new EventHandler(DetachImageList);
                imageList.ChangeHandle += new EventHandler(ImageListChangedHandle);
            }
        }

        private void DetachImageListHandlers()
        {
            if (imageList != null)
            {
                imageList.RecreateHandle -= new EventHandler(ImageListRecreateHandle);
                imageList.Disposed -= new EventHandler(DetachImageList);
                imageList.ChangeHandle -= new EventHandler(ImageListChangedHandle);
            }
        }

        private void AttachStateImageListHandlers()
        {
            if (stateImageList != null)
            {
                //NOTE: any handlers added here should be removed in DetachStateImageListHandlers
                stateImageList.RecreateHandle += new EventHandler(StateImageListRecreateHandle);
                stateImageList.Disposed += new EventHandler(DetachStateImageList);
                stateImageList.ChangeHandle += new EventHandler(StateImageListChangedHandle);
            }
        }

        private void DetachStateImageListHandlers()
        {
            if (stateImageList != null)
            {
                stateImageList.RecreateHandle -= new EventHandler(StateImageListRecreateHandle);
                stateImageList.Disposed -= new EventHandler(DetachStateImageList);
                stateImageList.ChangeHandle -= new EventHandler(StateImageListChangedHandle);
            }
        }

        /// <summary>
        ///  Returns the state image list control that is bound to the tree view.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(null)]
        [SRDescription(nameof(SR.TreeViewStateImageListDescr))]
        public ImageList StateImageList
        {
            get
            {
                return stateImageList;
            }
            set
            {
                if (value != stateImageList)
                {
                    DetachStateImageListHandlers();
                    stateImageList = value;
                    AttachStateImageListHandlers();

                    // Update TreeView's images
                    //
                    if (IsHandleCreated)
                    {
                        UpdateNativeStateImageList();

                        // We need to update the checks
                        // and stateimage value for each node.
                        UpdateCheckedState(root, true);

                        if ((value is null || stateImageList.Images.Count == 0) && CheckBoxes)
                        {
                            // Requires Handle Recreate to force on the checkBoxes and states..
                            RecreateHandle();
                        }
                        else
                        {
                            // The TreeView shows up the state imageList after sending this message even if the nodes dont have any stateImageIndex set.
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
                if (indent != -1)
                {
                    return indent;
                }
                else if (IsHandleCreated)
                {
                    return unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETINDENT));
                }
                return DefaultTreeViewIndent;
            }

            set
            {
                if (indent != value)
                {
                    if (value < 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(Indent), value, 0));
                    }
                    if (value > MaxIndent)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgumentEx, nameof(Indent), value, MaxIndent));
                    }
                    indent = value;
                    if (IsHandleCreated)
                    {
                        User32.SendMessageW(this, (User32.WM)TVM.SETINDENT, (IntPtr)value);
                        indent = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETINDENT));
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
                if (itemHeight != -1)
                {
                    return itemHeight;
                }

                if (IsHandleCreated)
                {
                    return unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETITEMHEIGHT));
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
                if (itemHeight != value)
                {
                    if (value < 1)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ItemHeight), value, 1));
                    }
                    if (value >= short.MaxValue)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidHighBoundArgument, nameof(ItemHeight), value, short.MaxValue));
                    }

                    itemHeight = value;
                    if (IsHandleCreated)
                    {
                        if (itemHeight % 2 != 0)
                        {
                            setOddHeight = true;
                            try
                            {
                                RecreateHandle();
                            }
                            finally
                            {
                                setOddHeight = false;
                            }
                        }

                        User32.SendMessageW(this, (User32.WM)TVM.SETITEMHEIGHT, (IntPtr)value);
                        itemHeight = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETITEMHEIGHT));
                    }
                }
            }
        }

        /// <summary>
        ///  The LabelEdit property determines if the label text
        ///  of nodes in the tree view is editable.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TreeViewLabelEditDescr))]
        public bool LabelEdit
        {
            get
            {
                return treeViewState[TREEVIEWSTATE_labelEdit];
            }
            set
            {
                if (LabelEdit != value)
                {
                    treeViewState[TREEVIEWSTATE_labelEdit] = value;
                    if (IsHandleCreated)
                    {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <summary>
        ///  This is the color of the lines that connect the nodes of the Treeview.
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
                    int intColor = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETLINECOLOR));
                    return ColorTranslator.FromWin32(intColor);
                }
                return lineColor;
            }
            set
            {
                if (lineColor != value)
                {
                    lineColor = value;
                    if (IsHandleCreated)
                    {
                        User32.SendMessageW(this, (User32.WM)TVM.SETLINECOLOR, IntPtr.Zero, PARAM.FromColor(lineColor));
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
                if (nodes is null)
                {
                    nodes = new TreeNodeCollection(root);
                }
                return nodes;
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
            get
            {
                return drawMode;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TreeViewDrawMode.Normal, (int)TreeViewDrawMode.OwnerDrawAll))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TreeViewDrawMode));
                }

                if (drawMode != value)
                {
                    drawMode = value;
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
        ///  The delimeter string used by TreeNode.getFullPath().
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue("\\")]
        [SRDescription(nameof(SR.TreeViewPathSeparatorDescr))]
        public string PathSeparator
        {
            get
            {
                return pathSeparator;
            }
            set
            {
                pathSeparator = value;
            }
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
        public new event EventHandler PaddingChanged
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
            get
            {
                return rightToLeftLayout;
            }

            set
            {
                if (value != rightToLeftLayout)
                {
                    rightToLeftLayout = value;
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
            get
            {
                return treeViewState[TREEVIEWSTATE_scrollable];
            }
            set
            {
                if (Scrollable != value)
                {
                    treeViewState[TREEVIEWSTATE_scrollable] = value;
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.TreeViewSelectedImageIndexDescr))]
        [RelatedImageList("ImageList")]
        public int SelectedImageIndex
        {
            get
            {
                if (imageList is null)
                {
                    return ImageList.Indexer.DefaultIndex;
                }
                if (SelectedImageIndexer.Index >= imageList.Images.Count)
                {
                    return Math.Max(0, imageList.Images.Count - 1);
                }
                return SelectedImageIndexer.Index;
            }
            set
            {
                // If (none) is selected in the image index editor, we'll just adjust this to
                // mean image index 0. This is because a treeview must always have an image index -
                // even if no imagelist exists we want the image index to be 0.
                //
                if (value == ImageList.Indexer.DefaultIndex)
                {
                    value = 0;
                }

                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(SelectedImageIndex), value, 0));
                }
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
        [Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [DefaultValue(ImageList.Indexer.DefaultKey)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [SRDescription(nameof(SR.TreeViewSelectedImageKeyDescr))]
        [RelatedImageList("ImageList")]
        public string SelectedImageKey
        {
            get
            {
                return SelectedImageIndexer.Key;
            }

            set
            {
                if (SelectedImageIndexer.Key != value)
                {
                    SelectedImageIndexer.Key = value;

                    if (string.IsNullOrEmpty(value) || value.Equals(SR.toStringNone))
                    {
                        SelectedImageIndex = (ImageList != null) ? 0 : ImageList.Indexer.DefaultIndex;
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
        public TreeNode SelectedNode
        {
            get
            {
                if (IsHandleCreated)
                {
                    IntPtr hItem = User32.SendMessageW(this, (User32.WM)TVM.GETNEXTITEM, (IntPtr)TVGN.CARET);
                    if (hItem == IntPtr.Zero)
                    {
                        return null;
                    }

                    return NodeFromHandle(hItem);
                }
                else if (selectedNode != null && selectedNode.TreeView == this)
                {
                    return selectedNode;
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
                    // This class invariant is not quite correct -- if the selected node does not belong to this Treeview,
                    // selectedNode != null even though the handle is created.  We will call set_SelectedNode
                    // to inform the handle that the selected node has been added to the TreeView.
                    Debug.Assert(selectedNode is null || selectedNode.TreeView != this, "handle is created, but we're still caching selectedNode");

                    IntPtr hnode = (value is null ? IntPtr.Zero : value.Handle);
                    User32.SendMessageW(this, (User32.WM)TVM.SELECTITEM, (IntPtr)TVGN.CARET, hnode);
                    selectedNode = null;
                }
                else
                {
                    selectedNode = value;
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
            get
            {
                return treeViewState[TREEVIEWSTATE_showLines];
            }
            set
            {
                if (ShowLines != value)
                {
                    treeViewState[TREEVIEWSTATE_showLines] = value;
                    if (IsHandleCreated)
                    {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <summary>
        ///  The ShowLines property determines whether or not the tooltips willbe displayed on the nodes
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.TreeViewShowShowNodeToolTipsDescr))]
        public bool ShowNodeToolTips
        {
            get
            {
                return treeViewState[TREEVIEWSTATE_showNodeToolTips];
            }
            set
            {
                if (ShowNodeToolTips != value)
                {
                    treeViewState[TREEVIEWSTATE_showNodeToolTips] = value;
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
            get
            {
                return treeViewState[TREEVIEWSTATE_showPlusMinus];
            }
            set
            {
                if (ShowPlusMinus != value)
                {
                    treeViewState[TREEVIEWSTATE_showPlusMinus] = value;
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
            get { return treeViewState[TREEVIEWSTATE_showRootLines]; }
            set
            {
                if (ShowRootLines != value)
                {
                    treeViewState[TREEVIEWSTATE_showRootLines] = value;
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
            get
            {
                return treeViewState[TREEVIEWSTATE_sorted];
            }
            set
            {
                if (Sorted != value)
                {
                    treeViewState[TREEVIEWSTATE_sorted] = value;
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
        public IComparer TreeViewNodeSorter
        {
            get
            {
                return treeViewNodeSorter;
            }
            set
            {
                if (treeViewNodeSorter != value)
                {
                    treeViewNodeSorter = value;
                    if (value != null)
                    {
                        Sort();
                    }
                }
            }
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Bindable(false)]
        public override string Text
        {
            get => base.Text;
            set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged
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
        public TreeNode TopNode
        {
            get
            {
                if (IsHandleCreated)
                {
                    IntPtr hitem = User32.SendMessageW(this, (User32.WM)TVM.GETNEXTITEM, (IntPtr)TVGN.FIRSTVISIBLE);
                    return (hitem == IntPtr.Zero ? null : NodeFromHandle(hitem));
                }
                return topNode;
            }
            set
            {
                if (IsHandleCreated && (value is null || value.TreeView == this))
                {
                    // This class invariant is not quite correct -- if the selected node does not belong to this Treeview,
                    // selectedNode != null even though the handle is created.  We will call set_SelectedNode
                    // to inform the handle that the selected node has been added to the TreeView.
                    Debug.Assert(topNode is null || topNode.TreeView != this, "handle is created, but we're still caching selectedNode");

                    IntPtr hnode = (value is null ? IntPtr.Zero : value.Handle);
                    User32.SendMessageW(this, (User32.WM)TVM.SELECTITEM, (IntPtr)TVGN.FIRSTVISIBLE, hnode);
                    topNode = null;
                }
                else
                {
                    topNode = value;
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
        public int VisibleCount
        {
            get
            {
                if (IsHandleCreated)
                {
                    return unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETVISIBLECOUNT));
                }

                return 0;
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewBeforeEditDescr))]
        public event NodeLabelEditEventHandler BeforeLabelEdit
        {
            add => onBeforeLabelEdit += value;
            remove => onBeforeLabelEdit -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewAfterEditDescr))]
        public event NodeLabelEditEventHandler AfterLabelEdit
        {
            add => onAfterLabelEdit += value;
            remove => onAfterLabelEdit -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewBeforeCheckDescr))]
        public event TreeViewCancelEventHandler BeforeCheck
        {
            add => onBeforeCheck += value;
            remove => onBeforeCheck -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewAfterCheckDescr))]
        public event TreeViewEventHandler AfterCheck
        {
            add => onAfterCheck += value;
            remove => onAfterCheck -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewBeforeCollapseDescr))]
        public event TreeViewCancelEventHandler BeforeCollapse
        {
            add => onBeforeCollapse += value;
            remove => onBeforeCollapse -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewAfterCollapseDescr))]
        public event TreeViewEventHandler AfterCollapse
        {
            add => onAfterCollapse += value;
            remove => onAfterCollapse -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewBeforeExpandDescr))]
        public event TreeViewCancelEventHandler BeforeExpand
        {
            add => onBeforeExpand += value;
            remove => onBeforeExpand -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewAfterExpandDescr))]
        public event TreeViewEventHandler AfterExpand
        {
            add => onAfterExpand += value;
            remove => onAfterExpand -= value;
        }

        /// <summary>
        ///  Fires when a TreeView node needs to be drawn.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewDrawNodeEventDescr))]
        public event DrawTreeNodeEventHandler DrawNode
        {
            add => onDrawNode += value;
            remove => onDrawNode -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.ListViewItemDragDescr))]
        public event ItemDragEventHandler ItemDrag
        {
            add => onItemDrag += value;
            remove => onItemDrag -= value;
        }

        [SRCategory(nameof(SR.CatAction))]
        [SRDescription(nameof(SR.TreeViewNodeMouseHoverDescr))]
        public event TreeNodeMouseHoverEventHandler NodeMouseHover
        {
            add => onNodeMouseHover += value;
            remove => onNodeMouseHover -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewBeforeSelectDescr))]
        public event TreeViewCancelEventHandler BeforeSelect
        {
            add => onBeforeSelect += value;
            remove => onBeforeSelect -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewAfterSelectDescr))]
        public event TreeViewEventHandler AfterSelect
        {
            add => onAfterSelect += value;
            remove => onAfterSelect -= value;
        }

        /// <summary>
        ///  TreeView Onpaint.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
        {
            add => base.Paint += value;
            remove => base.Paint -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewNodeMouseClickDescr))]
        public event TreeNodeMouseClickEventHandler NodeMouseClick
        {
            add => onNodeMouseClick += value;
            remove => onNodeMouseClick -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.TreeViewNodeMouseDoubleClickDescr))]
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick
        {
            add => onNodeMouseDoubleClick += value;
            remove => onNodeMouseDoubleClick -= value;
        }

        [SRCategory(nameof(SR.CatPropertyChanged))]
        [SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged
        {
            add => onRightToLeftLayoutChanged += value;
            remove => onRightToLeftLayoutChanged -= value;
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
            root.Collapse();
        }

        protected override void CreateHandle()
        {
            if (!RecreatingHandle)
            {
                IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);
                try
                {
                    var icc = new INITCOMMONCONTROLSEX
                    {
                        dwICC = ICC.TREEVIEW_CLASSES
                    };
                    InitCommonControlsEx(ref icc);
                }
                finally
                {
                    ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }

        /// <summary>
        ///  Resets the imageList to null.  We wire this method up to the imageList's
        ///  Dispose event, so that we don't hang onto an imageList that's gone away.
        /// </summary>
        private void DetachImageList(object sender, EventArgs e)
        {
            ImageList = null;
        }

        /// <summary>
        ///  Resets the stateimageList to null.  We wire this method up to the stateimageList's
        ///  Dispose event, so that we don't hang onto an stateimageList that's gone away.
        /// </summary>
        private void DetachStateImageList(object sender, EventArgs e)
        {
            internalStateImageList = null;
            StateImageList = null;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    DetachImageListHandlers();
                    imageList = null;
                    DetachStateImageListHandlers();
                    stateImageList = null;
                }
            }

            // Dispose unmanaged resources.
            _toolTipBuffer.Dispose();

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
            root.ExpandAll();
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
                    User32.SendMessageW(this, User32.WM.SETREDRAW, PARAM.FromBool(false));
                    if (delayed)
                    {
                        User32.PostMessageW(this, User32.WM.SETREDRAW, (IntPtr)1, IntPtr.Zero);
                    }
                    else
                    {
                        User32.SendMessageW(this, User32.WM.SETREDRAW, PARAM.FromBool(true));
                    }
                }
            }
        }

        /// <summary>
        ///  Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
        /// </summary>
        internal void SetToolTip(ToolTip toolTip, string toolTipText)
        {
            if (toolTip != null)
            {
                User32.SendMessageW(toolTip, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
                User32.SendMessageW(this, (User32.WM)TVM.SETTOOLTIPS, toolTip.Handle);
                controlToolTipText = toolTipText;
            }
        }

        /// <summary>
        ///  Gives the information about which part of the treeNode is at the given point.
        /// </summary>
        public TreeViewHitTestInfo HitTest(Point pt)
        {
            return HitTest(pt.X, pt.Y);
        }

        /// <summary>
        ///  Gives the information about which part of the treeNode is at the given x, y.
        /// </summary>
        public TreeViewHitTestInfo HitTest(int x, int y)
        {
            var tvhi = new TVHITTESTINFO
            {
                pt = new Point(x, y)
            };
            IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhi);
            TreeNode node = (hnode == IntPtr.Zero ? null : NodeFromHandle(hnode));
            TreeViewHitTestLocations loc = (TreeViewHitTestLocations)tvhi.flags;
            return (new TreeViewHitTestInfo(node, loc));
        }

        /// <summary>
        ///  Defined so that a  tree node can use it
        /// </summary>
        internal bool TreeViewBeforeCheck(TreeNode node, TreeViewAction actionTaken)
        {
            TreeViewCancelEventArgs tvce = new TreeViewCancelEventArgs(node, false, actionTaken);
            OnBeforeCheck(tvce);
            return (tvce.Cancel);
        }

        internal void TreeViewAfterCheck(TreeNode node, TreeViewAction actionTaken)
        {
            OnAfterCheck(new TreeViewEventArgs(node, actionTaken));
        }

        /// <summary>
        ///  Returns count of nodes at root, optionally including all subtrees.
        /// </summary>
        public int GetNodeCount(bool includeSubTrees)
        {
            return root.GetNodeCount(includeSubTrees);
        }

        /// <summary>
        ///  Returns the TreeNode at the given location in tree view coordinates.
        /// </summary>
        public TreeNode GetNodeAt(Point pt)
        {
            return GetNodeAt(pt.X, pt.Y);
        }

        /// <summary>
        ///  Returns the TreeNode at the given location in tree view coordinates.
        /// </summary>
        public TreeNode GetNodeAt(int x, int y)
        {
            var tvhi = new TVHITTESTINFO
            {
                pt = new Point(x, y)
            };

            IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhi);
            return (hnode == IntPtr.Zero ? null : NodeFromHandle(hnode));
        }

        private void ImageListRecreateHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                IntPtr handle = (ImageList is null) ? IntPtr.Zero : ImageList.Handle;
                User32.SendMessageW(this, (User32.WM)TVM.SETIMAGELIST, IntPtr.Zero, handle);
            }
        }

        private void UpdateImagesRecursive(TreeNode node)
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

        private void ImageListChangedHandle(object sender, EventArgs e)
        {
            if ((null != sender) && (sender == imageList) && IsHandleCreated)
            {
                BeginUpdate();
                foreach (TreeNode node in Nodes)
                {
                    UpdateImagesRecursive(node);
                }
                EndUpdate();
            }
        }

        private void StateImageListRecreateHandle(object sender, EventArgs e)
        {
            if (IsHandleCreated)
            {
                IntPtr handle = IntPtr.Zero;
                if (internalStateImageList != null)
                {
                    handle = internalStateImageList.CreateUniqueHandle();
                }
                SetStateImageList(handle);
            }
        }

        private void StateImageListChangedHandle(object sender, EventArgs e)
        {
            if ((null != sender) && (sender == stateImageList) && IsHandleCreated)
            {
                // Since the native treeview requires the state imagelist to be 1-indexed we need to
                // re add the images if the original collection had changed.
                if (stateImageList != null && stateImageList.Images.Count > 0)
                {
                    Image[] images = new Image[stateImageList.Images.Count + 1];
                    images[0] = stateImageList.Images[0];
                    for (int i = 1; i <= stateImageList.Images.Count; i++)
                    {
                        images[i] = stateImageList.Images[i - 1];
                    }

                    if (internalStateImageList != null)
                    {
                        internalStateImageList.Images.Clear();
                        internalStateImageList.Images.AddRange(images);
                    }
                    else
                    {
                        internalStateImageList = new ImageList();
                        internalStateImageList.Images.AddRange(images);
                    }

                    Debug.Assert(internalStateImageList != null, "Why are changing images when the Imagelist is null?");
                    if (internalStateImageList != null)
                    {
                        if (ScaledStateImageSize != null)
                        {
                            internalStateImageList.ImageSize = (Size)ScaledStateImageSize;
                        }

                        SetStateImageList(internalStateImageList.CreateUniqueHandle());
                    }
                }
                else //stateImageList is null || stateImageList.Images.Count = 0;
                {
                    UpdateCheckedState(root, true);
                }
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
            if (editNode != null && (keyData & Keys.Alt) == 0)
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
        internal TreeNode NodeFromHandle(IntPtr handle)
        {
            TreeNode node = (TreeNode)nodeTable[handle];

            return node;
        }

        /// <summary>
        ///  Fires the DrawNode event.
        /// </summary>
        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            onDrawNode?.Invoke(this, e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            if (!IsHandleCreated)
            {
                base.OnHandleCreated(e);
                return;
            }

            TreeNode savedSelectedNode = selectedNode;
            selectedNode = null;

            base.OnHandleCreated(e);

            int version = unchecked((int)(long)User32.SendMessageW(this, (User32.WM)CCM.GETVERSION));
            if (version < 5)
            {
                User32.SendMessageW(this, (User32.WM)CCM.SETVERSION, (IntPtr)5);
            }

            // Workaround for problem in TreeView where it doesn't recognize the TVS_CHECKBOXES
            // style if it is set before the window is created.  To get around the problem,
            // we set it here after the window is created, and we make sure we don't set it
            // in getCreateParams so that this will actually change the value of the bit.
            // This seems to make the Treeview happy.
            if (CheckBoxes)
            {
                int style = unchecked((int)User32.GetWindowLong(this, User32.GWL.STYLE));
                style |= (int)TVS.CHECKBOXES;
                User32.SetWindowLong(this, User32.GWL.STYLE, (IntPtr)style);
            }

            if (ShowNodeToolTips && !DesignMode)
            {
                int style = unchecked((int)User32.GetWindowLong(this, User32.GWL.STYLE));
                style |= (int)TVS.INFOTIP;
                User32.SetWindowLong(this, User32.GWL.STYLE, (IntPtr)style);
            }

            Color c = BackColor;
            if (c != SystemColors.Window)
            {
                User32.SendMessageW(this, (User32.WM)TVM.SETBKCOLOR, IntPtr.Zero, PARAM.FromColor(c));
            }

            c = ForeColor;

            if (c != SystemColors.WindowText)
            {
                User32.SendMessageW(this, (User32.WM)TVM.SETTEXTCOLOR, IntPtr.Zero, PARAM.FromColor(c));
            }

            // Put the linecolor into the native control only if set.
            if (lineColor != Color.Empty)
            {
                User32.SendMessageW(this, (User32.WM)TVM.SETLINECOLOR, IntPtr.Zero, PARAM.FromColor(lineColor));
            }

            if (imageList != null)
            {
                User32.SendMessageW(this, (User32.WM)TVM.SETIMAGELIST, IntPtr.Zero, imageList.Handle);
            }

            if (stateImageList != null)
            {
                UpdateNativeStateImageList();
            }

            if (indent != -1)
            {
                User32.SendMessageW(this, (User32.WM)TVM.SETINDENT, (IntPtr)indent);
            }

            if (itemHeight != -1)
            {
                User32.SendMessageW(this, (User32.WM)TVM.SETITEMHEIGHT, (IntPtr)ItemHeight);
            }

            // Essentially we are setting the width to be infinite so that the
            // TreeView never thinks it needs a scrollbar when the first node is created
            // during the first handle creation.
            //
            // This is set back to the oldSize after the Realize method.
            int oldSize = 0;
            try
            {
                treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs] = true;
                oldSize = Width;
                User32.SWP flags = User32.SWP.NOZORDER | User32.SWP.NOACTIVATE | User32.SWP.NOMOVE;
                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOP,
                    Left,
                    Top,
                    int.MaxValue,
                    Height,
                    flags);

                root.Realize(false);

                if (oldSize != 0)
                {
                    User32.SetWindowPos(
                        new HandleRef(this, Handle),
                        User32.HWND_TOP,
                        Left,
                        Top,
                        oldSize,
                        Height,
                        flags);
                }
            }
            finally
            {
                treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs] = false;
            }

            SelectedNode = savedSelectedNode;
        }

        // Replace the native control's ImageList with our current stateImageList
        // set the value of internalStateImageList to the new list
        private void UpdateNativeStateImageList()
        {
            if (stateImageList != null && stateImageList.Images.Count > 0)
            {
                ImageList newImageList = new ImageList();
                if (ScaledStateImageSize != null)
                {
                    newImageList.ImageSize = (Size)ScaledStateImageSize;
                }

                Image[] images = new Image[stateImageList.Images.Count + 1];
                images[0] = stateImageList.Images[0];
                for (int i = 1; i <= stateImageList.Images.Count; i++)
                {
                    images[i] = stateImageList.Images[i - 1];
                }
                newImageList.Images.AddRange(images);
                User32.SendMessageW(this, (User32.WM)TVM.SETIMAGELIST, (IntPtr)TVSIL.STATE, newImageList.CreateUniqueHandle());

                if (internalStateImageList != null)
                {
                    internalStateImageList.Dispose();
                }
                internalStateImageList = newImageList;
            }
        }

        private void SetStateImageList(IntPtr handle)
        {
            // In certain cases (TREEVIEWSTATE_checkBoxes) e.g., the Native TreeView leaks the imagelist
            // even if set by us. To prevent any leaks, we always destroy what was there after setting a new list.
            IntPtr handleOld = User32.SendMessageW(this, (User32.WM)TVM.SETIMAGELIST, (IntPtr)TVSIL.STATE, handle);
            if ((handleOld != IntPtr.Zero) && (handleOld != handle))
            {
                var result = ComCtl32.ImageList.Destroy(new HandleRef(this, handleOld));
                Debug.Assert(result.IsTrue());
            }
        }

        // Destroying the tree-view control does not destroy the native state image list.
        // We must destroy it explicitly.
        private void DestroyNativeStateImageList(bool reset)
        {
            IntPtr handle = User32.SendMessageW(this, (User32.WM)TVM.GETIMAGELIST, (IntPtr)TVSIL.STATE);
            if (handle != IntPtr.Zero)
            {
                var result = ComCtl32.ImageList.Destroy(new HandleRef(this, handle));
                Debug.Assert(result.IsTrue());
                if (reset)
                {
                    User32.SendMessageW(this, (User32.WM)TVM.SETIMAGELIST, (IntPtr)TVSIL.STATE);
                }
            }
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            selectedNode = SelectedNode;

            // Unfortunately, to avoid the native tree view leaking it's State Image List, we need to
            // destroy it ourselves here.
            DestroyNativeStateImageList(true);

            // for the case when we are NOT being disposed, we'll be recreating the internal state imagelist
            // in OnHandleCreate, so it is ok to completely Dispose here
            if (internalStateImageList != null)
            {
                internalStateImageList.Dispose();
                internalStateImageList = null;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        ///  We keep track of if we've hovered already so we don't fire multiple hover events
        /// </summary>
        protected override void OnMouseLeave(EventArgs e)
        {
            hoveredAlready = false;
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
            var tvhip = new TVHITTESTINFO
            {
                pt = PointToClient(Cursor.Position)
            };

            IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhip);
            if (hnode != IntPtr.Zero && ((tvhip.flags & TVHT.ONITEM) != 0))
            {
                TreeNode tn = NodeFromHandle(hnode);
                if (tn != prevHoveredNode && tn != null)
                {
                    OnNodeMouseHover(new TreeNodeMouseHoverEventArgs(tn));
                    prevHoveredNode = tn;
                }
            }

            if (!hoveredAlready)
            {
                base.OnMouseHover(e);
                hoveredAlready = true;
            }

            ResetMouseEventArgs();
        }

        /// <summary>
        ///  Fires the beforeLabelEdit event.
        /// </summary>
        protected virtual void OnBeforeLabelEdit(NodeLabelEditEventArgs e)
        {
            onBeforeLabelEdit?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the afterLabelEdit event.
        /// </summary>
        protected virtual void OnAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            onAfterLabelEdit?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the beforeCheck event.
        /// </summary>
        protected virtual void OnBeforeCheck(TreeViewCancelEventArgs e)
        {
            onBeforeCheck?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the afterCheck event.
        /// </summary>
        protected virtual void OnAfterCheck(TreeViewEventArgs e)
        {
            onAfterCheck?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the beforeCollapse event.
        /// </summary>
        protected internal virtual void OnBeforeCollapse(TreeViewCancelEventArgs e)
        {
            onBeforeCollapse?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the afterCollapse event.
        /// </summary>
        protected internal virtual void OnAfterCollapse(TreeViewEventArgs e)
        {
            onAfterCollapse?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the beforeExpand event.
        /// </summary>
        protected virtual void OnBeforeExpand(TreeViewCancelEventArgs e)
        {
            onBeforeExpand?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the afterExpand event.
        /// </summary>
        protected virtual void OnAfterExpand(TreeViewEventArgs e)
        {
            onAfterExpand?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the ItemDrag event.
        /// </summary>
        protected virtual void OnItemDrag(ItemDragEventArgs e)
        {
            onItemDrag?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the NodeMouseHover event.
        /// </summary>
        protected virtual void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e)
        {
            onNodeMouseHover?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the beforeSelect event.
        /// </summary>
        protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e)
        {
            onBeforeSelect?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the afterSelect event.
        /// </summary>
        protected virtual void OnAfterSelect(TreeViewEventArgs e)
        {
            onAfterSelect?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the onNodeMouseClick event.
        /// </summary>
        protected virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
        {
            onNodeMouseClick?.Invoke(this, e);
        }

        /// <summary>
        ///  Fires the onNodeMouseDoubleClick event.
        /// </summary>
        protected virtual void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e)
        {
            onNodeMouseDoubleClick?.Invoke(this, e);
        }

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
                TreeNode node = SelectedNode;
                if (node != null)
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

            onRightToLeftLayoutChanged?.Invoke(this, e);
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
            indent = -1;
            // is this overkill?
            RecreateHandle();
        }

        /// <summary>
        ///  This resets the item height to the system default.
        /// </summary>
        private void ResetItemHeight()
        {
            itemHeight = -1;
            RecreateHandle();
        }

        /// <summary>
        ///  Retrieves true if the indent should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeIndent()
        {
            return (indent != -1);
        }

        /// <summary>
        ///  Retrieves true if the itemHeight should be persisted in code gen.
        /// </summary>
        private bool ShouldSerializeItemHeight()
        {
            return (itemHeight != -1);
        }

        private bool ShouldSerializeSelectedImageIndex()
        {
            if (imageList != null)
            {
                return (SelectedImageIndex != 0);
            }
            return (SelectedImageIndex != ImageList.Indexer.DefaultIndex);
        }

        private bool ShouldSerializeImageIndex()
        {
            if (imageList != null)
            {
                return (ImageIndex != 0);
            }
            return (ImageIndex != ImageList.Indexer.DefaultIndex);
        }

        /// <summary>
        ///  Updated the sorted order
        /// </summary>
        public void Sort()
        {
            Sorted = true;
            RefreshNodes();
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            if (Nodes != null)
            {
                s += ", Nodes.Count: " + Nodes.Count.ToString(CultureInfo.CurrentCulture);
                if (Nodes.Count > 0)
                {
                    s += ", Nodes[0]: " + Nodes[0].ToString();
                }
            }
            return s;
        }

        private unsafe void TvnBeginDrag(MouseButtons buttons, NMTREEVIEW* nmtv)
        {
            TVITEMW item = nmtv->itemNew;

            // Check for invalid node handle
            if (item.hItem == IntPtr.Zero)
            {
                return;
            }

            TreeNode node = NodeFromHandle(item.hItem);

            OnItemDrag(new ItemDragEventArgs(buttons, node));
        }

        private unsafe IntPtr TvnExpanding(NMTREEVIEW* nmtv)
        {
            TVITEMW item = nmtv->itemNew;

            // Check for invalid node handle
            if (item.hItem == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            TreeViewCancelEventArgs e = null;
            if ((item.state & TVIS.EXPANDED) == 0)
            {
                e = new TreeViewCancelEventArgs(NodeFromHandle(item.hItem), false, TreeViewAction.Expand);
                OnBeforeExpand(e);
            }
            else
            {
                e = new TreeViewCancelEventArgs(NodeFromHandle(item.hItem), false, TreeViewAction.Collapse);
                OnBeforeCollapse(e);
            }
            return (IntPtr)(e.Cancel ? 1 : 0);
        }

        private unsafe void TvnExpanded(NMTREEVIEW* nmtv)
        {
            TVITEMW item = nmtv->itemNew;

            // Check for invalid node handle
            if (item.hItem == IntPtr.Zero)
            {
                return;
            }

            TreeViewEventArgs e;
            TreeNode node = NodeFromHandle(item.hItem);

            // Note that IsExpanded is invalid for the moment, so we use item item.state to branch.
            if ((item.state & TVIS.EXPANDED) == 0)
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

        private unsafe IntPtr TvnSelecting(NMTREEVIEW* nmtv)
        {
            if (treeViewState[TREEVIEWSTATE_ignoreSelects])
            {
                return (IntPtr)1;
            }
            // Check for invalid node handle
            if (nmtv->itemNew.hItem == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            TreeNode node = NodeFromHandle(nmtv->itemNew.hItem);

            TreeViewAction action = TreeViewAction.Unknown;
            switch (nmtv->action)
            {
                case TVC.BYKEYBOARD:
                    action = TreeViewAction.ByKeyboard;
                    break;
                case TVC.BYMOUSE:
                    action = TreeViewAction.ByMouse;
                    break;
            }

            TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(node, false, action);
            OnBeforeSelect(e);

            return (IntPtr)(e.Cancel ? 1 : 0);
        }

        private unsafe void TvnSelected(NMTREEVIEW* nmtv)
        {
            // If called from the TreeNodeCollection.Clear() then return.
            if (nodesCollectionClear)
            {
                return;
            }

            if (nmtv->itemNew.hItem != IntPtr.Zero)
            {
                TreeViewAction action = TreeViewAction.Unknown;
                switch (nmtv->action)
                {
                    case TVC.BYKEYBOARD:
                        action = TreeViewAction.ByKeyboard;
                        break;
                    case TVC.BYMOUSE:
                        action = TreeViewAction.ByMouse;
                        break;
                }

                OnAfterSelect(new TreeViewEventArgs(NodeFromHandle(nmtv->itemNew.hItem), action));
            }

            // TreeView doesn't properly revert back to the unselected image
            // if the unselected image is blank.
            var rc = new RECT();
            *((IntPtr*)&rc.left) = nmtv->itemOld.hItem;
            if (nmtv->itemOld.hItem != IntPtr.Zero)
            {
                if (unchecked((int)(long)User32.SendMessageW(this, (User32.WM)TVM.GETITEMRECT, (IntPtr)1, ref rc)) != 0)
                {
                    User32.InvalidateRect(new HandleRef(this, Handle), &rc, BOOL.TRUE);
                }
            }
        }

        private IntPtr TvnBeginLabelEdit(NMTVDISPINFOW nmtvdi)
        {
            // Check for invalid node handle
            if (nmtvdi.item.hItem == IntPtr.Zero)
            {
                return IntPtr.Zero;
            }

            TreeNode editingNode = NodeFromHandle(nmtvdi.item.hItem);
            NodeLabelEditEventArgs e = new NodeLabelEditEventArgs(editingNode);
            OnBeforeLabelEdit(e);
            if (!e.CancelEdit)
            {
                editNode = editingNode;
            }

            return (IntPtr)(e.CancelEdit ? 1 : 0);
        }

        private IntPtr TvnEndLabelEdit(NMTVDISPINFOW nmtvdi)
        {
            editNode = null;

            // Check for invalid node handle
            if (nmtvdi.item.hItem == IntPtr.Zero)
            {
                return (IntPtr)1;
            }

            TreeNode node = NodeFromHandle(nmtvdi.item.hItem);
            string newText = (nmtvdi.item.pszText == IntPtr.Zero ? null : Marshal.PtrToStringAuto(nmtvdi.item.pszText));
            NodeLabelEditEventArgs e = new NodeLabelEditEventArgs(node, newText);
            OnAfterLabelEdit(e);
            if (newText != null && !e.CancelEdit && node != null)
            {
                node.text = newText;
                if (Scrollable)
                {
                    ForceScrollbarUpdate(true);
                }
            }
            return (IntPtr)(e.CancelEdit ? 0 : 1);
        }

        internal override void UpdateStylesCore()
        {
            base.UpdateStylesCore();
            if (IsHandleCreated && CheckBoxes)
            {
                if (StateImageList != null)
                {
                    // Setting the TVS_CHECKBOXES window style also causes the TreeView to display the default checkbox
                    // images rather than the user specified StateImageList.  We send a TVM_SETIMAGELIST to restore the
                    // user's images.
                    if (internalStateImageList != null)
                    {
                        SetStateImageList(internalStateImageList.CreateUniqueHandle());
                    }
                }
            }
        }

        /// <remarks>
        ///  Setting the TVS.CHECKBOXES style clears the checked state
        /// </remarks>
        private void UpdateCheckedState(TreeNode node, bool update)
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
            // If the user shows the ContextMenu bu overiding the WndProc( ), then the treeview
            // goes into the weird state where the high-light gets locked to the node on which the ContextMenu was shown.
            // So we need to get the native TREEIVEW out of this weird state.
            User32.SendMessageW(this, (User32.WM)TVM.SELECTITEM, (IntPtr)TVGN.DROPHILITE);

            // Windows TreeView pushes its own message loop in WM_xBUTTONDOWN, so fire the
            // event before calling defWndProc or else it won't get fired until the button
            // comes back up.
            OnMouseDown(new MouseEventArgs(button, clicks, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));

            //If Validation is cancelled dont fire any events through the Windows TreeView's message loop...
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
            NMTVCUSTOMDRAW* nmtvcd = (NMTVCUSTOMDRAW*)m.LParam;

            // Find out which stage we're drawing
            switch (nmtvcd->nmcd.dwDrawStage)
            {
                // Do we want OwnerDraw for this paint cycle?
                case CDDS.PREPAINT:
                    m.Result = (IntPtr)CDRF.NOTIFYITEMDRAW; // yes, we do...
                    return;
                // We've got opt-in on owner draw for items - so handle each one.
                case CDDS.ITEMPREPAINT:
                    // get the node
                    Debug.Assert(nmtvcd->nmcd.dwItemSpec != IntPtr.Zero, "Invalid node handle in ITEMPREPAINT");
                    TreeNode node = NodeFromHandle((IntPtr)nmtvcd->nmcd.dwItemSpec);

                    if (node is null)
                    {
                        // this can happen if we are presently inserting the node - it hasn't yet
                        // been added to the handle table
                        m.Result = (IntPtr)(CDRF.SKIPDEFAULT);
                        return;
                    }

                    CDIS state = nmtvcd->nmcd.uItemState;

                    // The commctrl TreeView allows you to draw the whole row of a node
                    // or nothing at all. The way we provide OwnerDrawText is by asking it
                    // to draw everything but the text - to do this, we set text color same
                    // as background color.
                    if (drawMode == TreeViewDrawMode.OwnerDrawText)
                    {
                        nmtvcd->clrText = nmtvcd->clrTextBk;
                        m.Result = (IntPtr)(CDRF.NEWFONT | CDRF.NOTIFYPOSTPAINT);
                        return;
                    }
                    else if (drawMode == TreeViewDrawMode.OwnerDrawAll)
                    {
                        Graphics g = nmtvcd->nmcd.hdc.CreateGraphics();

                        DrawTreeNodeEventArgs e;

                        try
                        {
                            Rectangle bounds = node.RowBounds;

                            var si = new User32.SCROLLINFO
                            {
                                cbSize = (uint)Marshal.SizeOf<User32.SCROLLINFO>(),
                                fMask = User32.SIF.POS
                            };
                            if (User32.GetScrollInfo(this, User32.SB.HORZ, ref si).IsTrue())
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
                        finally
                        {
                            g.Dispose();
                        }

                        if (!e.DrawDefault)
                        {
                            m.Result = (IntPtr)(CDRF.SKIPDEFAULT);
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
                    if (renderinfo != null && !riFore.IsEmpty)
                    {
                        nmtvcd->clrText = ColorTranslator.ToWin32(riFore);
                    }
                    if (renderinfo != null && !riBack.IsEmpty)
                    {
                        nmtvcd->clrTextBk = ColorTranslator.ToWin32(riBack);
                    }

                    if (renderinfo != null && renderinfo.Font != null)
                    {
                        // Mess with the DC directly...
                        Gdi32.SelectObject(nmtvcd->nmcd.hdc, renderinfo.FontHandle);

                        // There is a problem in winctl that clips node fonts if the fontsize
                        // is larger than the treeview font size. The behavior is much better in comctl 5 and above.
                        m.Result = (IntPtr)CDRF.NEWFONT;
                        return;
                    }

                    // fall through and do the default drawing work
                    goto default;

                case (CDDS.ITEMPOSTPAINT):
                    //User draws only the text in OwnerDrawText mode, as explained in comments above
                    if (drawMode == TreeViewDrawMode.OwnerDrawText)
                    {
                        Debug.Assert(nmtvcd->nmcd.dwItemSpec != IntPtr.Zero, "Invalid node handle in ITEMPOSTPAINT");

                        // Get the node
                        node = NodeFromHandle(nmtvcd->nmcd.dwItemSpec);

                        if (node is null)
                        {
                            // this can happen if we are presently inserting the node - it hasn't yet
                            // been added to the handle table
                            return;
                        }

                        using (Graphics g = nmtvcd->nmcd.hdc.CreateGraphics())
                        {
                            Rectangle bounds = node.Bounds;
                            Size textSize = TextRenderer.MeasureText(node.Text, node.TreeView.Font);
                            Point textLoc = new Point(bounds.X - 1, bounds.Y); // required to center the text
                            bounds = new Rectangle(textLoc, new Size(textSize.Width, bounds.Height));

                            DrawTreeNodeEventArgs e = new DrawTreeNodeEventArgs(g, node, bounds, (TreeNodeStates)(nmtvcd->nmcd.uItemState));
                            OnDrawNode(e);

                            if (e.DrawDefault)
                            {
                                //Simulate default text drawing here
                                TreeNodeStates curState = e.State;

                                Font font = node.NodeFont ?? node.TreeView.Font;
                                Color color = (((curState & TreeNodeStates.Selected) == TreeNodeStates.Selected) && node.TreeView.Focused) ? SystemColors.HighlightText : (node.ForeColor != Color.Empty) ? node.ForeColor : node.TreeView.ForeColor;

                                // Draw the actual node.
                                if ((curState & TreeNodeStates.Selected) == TreeNodeStates.Selected)
                                {
                                    g.FillRectangle(SystemBrushes.Highlight, bounds);
                                    ControlPaint.DrawFocusRectangle(g, bounds, color, SystemColors.Highlight);
                                    TextRenderer.DrawText(g, e.Node.Text, font, bounds, color, TextFormatFlags.Default);
                                }
                                else
                                {
                                    using var brush = BackColor.GetCachedSolidBrushScope();
                                    g.FillRectangle(brush, bounds);

                                    TextRenderer.DrawText(g, e.Node.Text, font, bounds, color, TextFormatFlags.Default);
                                }
                            }
                        }

                        m.Result = (IntPtr)CDRF.NOTIFYSUBITEMDRAW;
                        return;
                    }

                    goto default;

                default:
                    // just in case we get a spurious message, tell it to do the right thing
                    m.Result = (IntPtr)CDRF.DODEFAULT;
                    return;
            }
        }

        /// <summary>
        ///  Generates colors for each item. This can be overridden to provide colors on a per state/per node
        ///  basis, rather than using the ForeColor/BackColor/NodeFont properties on TreeNode.
        /// </summary>
        protected OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state)
        {
            OwnerDrawPropertyBag retval = new OwnerDrawPropertyBag();
            if (node is null || node.propBag is null)
            {
                return retval;
            }

            // we only change colors if we're displaying things normally
            if ((state & (int)(CDIS.SELECTED | CDIS.GRAYED | CDIS.HOT | CDIS.DISABLED)) == 0)
            {
                retval.ForeColor = node.propBag.ForeColor;
                retval.BackColor = node.propBag.BackColor;
            }
            retval.Font = node.propBag.Font;
            return retval;
        }

        private unsafe bool WmShowToolTip(ref Message m)
        {
            User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
            IntPtr tooltipHandle = nmhdr->hwndFrom;

            var tvhip = new TVHITTESTINFO
            {
                pt = PointToClient(Cursor.Position)
            };
            IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhip);
            if (hnode != IntPtr.Zero && ((tvhip.flags & TVHT.ONITEM) != 0))
            {
                TreeNode tn = NodeFromHandle(hnode);
                if (tn != null)
                {
                    if (!ShowNodeToolTips) // default ToolTips
                    {
                        Rectangle bounds = tn.Bounds;
                        bounds.Location = PointToScreen(bounds.Location);

                        User32.SendMessageW(tooltipHandle, (User32.WM)TTM.ADJUSTRECT, PARAM.FromBool(true), ref bounds);
                        User32.SetWindowPos(
                            new HandleRef(this, tooltipHandle),
                            User32.HWND_TOPMOST,
                            bounds.Left,
                            bounds.Top,
                            flags: User32.SWP.NOACTIVATE | User32.SWP.NOSIZE | User32.SWP.NOZORDER);
                        return true;
                    }
                }
            }
            return false;
        }

        private unsafe void WmNeedText(ref Message m)
        {
            NMTTDISPINFOW* ttt = (NMTTDISPINFOW*)m.LParam;
            string tipText = controlToolTipText;

            var tvhip = new TVHITTESTINFO
            {
                pt = PointToClient(Cursor.Position)
            };
            IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhip);
            if (hnode != IntPtr.Zero && ((tvhip.flags & TVHT.ONITEM) != 0))
            {
                TreeNode tn = NodeFromHandle(hnode);
                if (ShowNodeToolTips && tn != null && (!string.IsNullOrEmpty(tn.ToolTipText)))
                {
                    tipText = tn.ToolTipText;
                }
                else if (tn != null && tn.Bounds.Right > Bounds.Right)
                {
                    tipText = tn.Text;
                }
                else
                {
                    tipText = null;
                }
            }

            _toolTipBuffer.SetText(tipText);
            ttt->lpszText = _toolTipBuffer.Buffer;
            ttt->hinst = IntPtr.Zero;

            // RightToLeft reading order
            if (RightToLeft == RightToLeft.Yes)
            {
                ttt->uFlags |= TTF.RTLREADING;
            }
        }

        private unsafe void WmNotify(ref Message m)
        {
            User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;

            // Custom draw code is handled separately.
            if ((nmhdr->code == (int)NM.CUSTOMDRAW))
            {
                CustomDraw(ref m);
            }
            else
            {
                NMTREEVIEW* nmtv = (NMTREEVIEW*)m.LParam;

                switch (nmtv->nmhdr.code)
                {
                    case (int)TVN.ITEMEXPANDINGW:
                        m.Result = TvnExpanding(nmtv);
                        break;
                    case (int)TVN.ITEMEXPANDEDW:
                        TvnExpanded(nmtv);
                        break;
                    case (int)TVN.SELCHANGINGW:
                        m.Result = TvnSelecting(nmtv);
                        break;
                    case (int)TVN.SELCHANGEDW:
                        TvnSelected(nmtv);
                        break;
                    case (int)TVN.BEGINDRAGW:
                        TvnBeginDrag(MouseButtons.Left, nmtv);
                        break;
                    case (int)TVN.BEGINRDRAGW:
                        TvnBeginDrag(MouseButtons.Right, nmtv);
                        break;
                    case (int)TVN.BEGINLABELEDITW:
                        m.Result = TvnBeginLabelEdit(*(NMTVDISPINFOW*)m.LParam);
                        break;
                    case (int)TVN.ENDLABELEDITW:
                        m.Result = TvnEndLabelEdit(*(NMTVDISPINFOW*)m.LParam);
                        break;
                    case (int)NM.CLICK:
                    case (int)NM.RCLICK:
                        MouseButtons button = MouseButtons.Left;
                        Point pos = PointToClient(Cursor.Position);
                        var tvhip = new TVHITTESTINFO
                        {
                            pt = pos
                        };
                        IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhip);
                        if (nmtv->nmhdr.code != (int)NM.CLICK
                                    || (tvhip.flags & TVHT.ONITEM) != 0)
                        {
                            button = nmtv->nmhdr.code == (int)NM.CLICK
                                ? MouseButtons.Left : MouseButtons.Right;
                        }

                        // The treeview's WndProc doesn't get the WM_LBUTTONUP messages when
                        // LBUTTONUP happens on TVHT_ONITEM. This is a comctl quirk.
                        // We work around that by calling OnMouseUp here.
                        if (nmtv->nmhdr.code != (int)NM.CLICK
                            || (tvhip.flags & TVHT.ONITEM) != 0 || FullRowSelect)
                        {
                            if (hnode != IntPtr.Zero && !ValidationCancelled)
                            {
                                OnNodeMouseClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), button, 1, pos.X, pos.Y));
                                OnClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                                OnMouseClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                            }
                        }
                        if (nmtv->nmhdr.code == (int)NM.RCLICK)
                        {
                            TreeNode treeNode = NodeFromHandle(hnode);
                            if (treeNode != null && treeNode.ContextMenuStrip != null)
                            {
                                ShowContextMenu(treeNode);
                            }
                            else
                            {
                                treeViewState[TREEVIEWSTATE_showTreeViewContextMenu] = true;
                                User32.SendMessageW(this, User32.WM.CONTEXTMENU, Handle, (IntPtr)User32.GetMessagePos());
                            }
                            m.Result = (IntPtr)1;
                        }

                        if (!treeViewState[TREEVIEWSTATE_mouseUpFired])
                        {
                            if (nmtv->nmhdr.code != (int)NM.CLICK
                            || (tvhip.flags & TVHT.ONITEM) != 0)
                            {
                                // The treeview's WndProc doesn't get the WM_LBUTTONUP messages when
                                // LBUTTONUP happens on TVHT_ONITEM. This is a comctl quirk.
                                // We work around that by calling OnMouseUp here.
                                OnMouseUp(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                                treeViewState[TREEVIEWSTATE_mouseUpFired] = true;
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        ///  Shows the context menu for the Treenode.
        /// </summary>
        private void ShowContextMenu(TreeNode treeNode)
        {
            if (treeNode.ContextMenuStrip != null)
            {
                ContextMenuStrip menu = treeNode.ContextMenuStrip;

                // Need to send TVM_SELECTITEM to highlight the node while the contextMenuStrip is being shown.
                User32.PostMessageW(this, (User32.WM)TVM.SELECTITEM, (IntPtr)TVGN.DROPHILITE, treeNode.Handle);
                menu.ShowInternal(this, PointToClient(MousePosition),/*keyboardActivated*/false);
                menu.Closing += new ToolStripDropDownClosingEventHandler(ContextMenuStripClosing);
            }
        }

        // Need to send TVM_SELECTITEM to reset the node-highlighting while the contextMenuStrip is being closed so that the treeview reselects the SelectedNode.
        private void ContextMenuStripClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ContextMenuStrip strip = sender as ContextMenuStrip;
            // Unhook the Event.
            strip.Closing -= new ToolStripDropDownClosingEventHandler(ContextMenuStripClosing);
            User32.SendMessageW(this, (User32.WM)TVM.SELECTITEM, (IntPtr)TVGN.DROPHILITE);
        }

        private void WmPrint(ref Message m)
        {
            base.WndProc(ref m);

            if (((User32.PRF)m.LParam & User32.PRF.NONCLIENT) != 0 && Application.RenderWithVisualStyles && BorderStyle == BorderStyle.Fixed3D)
            {
                using Graphics g = Graphics.FromHdc(m.WParam);
                Rectangle rect = new Rectangle(0, 0, Size.Width - 1, Size.Height - 1);
                using var pen = VisualStyleInformation.TextControlBorder.GetCachedPenScope();
                g.DrawRectangle(pen, rect);
                rect.Inflate(-1, -1);
                g.DrawRectangle(SystemPens.Window, rect);
            }
        }

        protected unsafe override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)User32.WM.WINDOWPOSCHANGING:
                case (int)User32.WM.NCCALCSIZE:
                case (int)User32.WM.WINDOWPOSCHANGED:
                case (int)User32.WM.SIZE:
                    // While we are changing size of treeView to avoid the scrollbar; dont respond to the window-sizing messages.
                    if (treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs])
                    {
                        DefWndProc(ref m);
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case (int)User32.WM.HSCROLL:
                    base.WndProc(ref m);
                    if (DrawMode == TreeViewDrawMode.OwnerDrawAll)
                    {
                        Invalidate();
                    }
                    break;

                case (int)User32.WM.PRINT:
                    WmPrint(ref m);
                    break;
                case (int)TVM.SETITEMW:
                    base.WndProc(ref m);
                    if (CheckBoxes)
                    {
                        TVITEMW* item = (TVITEMW*)m.LParam;
                        // Check for invalid node handle
                        if (item->hItem != IntPtr.Zero)
                        {
                            var item1 = new TVITEMW
                            {
                                mask = TVIF.HANDLE | TVIF.STATE,
                                hItem = item->hItem,
                                stateMask = TVIS.STATEIMAGEMASK
                            };
                            User32.SendMessageW(this, (User32.WM)TVM.GETITEMW, IntPtr.Zero, ref item1);

                            TreeNode node = NodeFromHandle(item->hItem);
                            node.CheckedStateInternal = (((int)item1.state >> TreeNode.SHIFTVAL) > 1);
                        }
                    }
                    break;
                case (int)User32.WM.NOTIFY:
                    User32.NMHDR* nmhdr = (User32.NMHDR*)m.LParam;
                    switch ((TTN)nmhdr->code)
                    {
                        case TTN.GETDISPINFOW:
                            // Setting the max width has the added benefit of enabling multiline tool tips
                            User32.SendMessageW(nmhdr->hwndFrom, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
                            WmNeedText(ref m);
                            m.Result = (IntPtr)1;
                            return;
                        case TTN.SHOW:
                            if (WmShowToolTip(ref m))
                            {
                                m.Result = (IntPtr)1;
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
                case (int)(User32.WM.REFLECT_NOTIFY):
                    WmNotify(ref m);
                    break;
                case (int)User32.WM.LBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Left, 2);

                    // Just maintain state and fire double click in final mouseUp.
                    treeViewState[TREEVIEWSTATE_doubleclickFired] = true;

                    // Fire mouse up in the Wndproc.
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;

                    // Make sure we get the mouse up if it happens outside the control.
                    Capture = true;
                    break;
                case (int)User32.WM.LBUTTONDOWN:
                    try
                    {
                        treeViewState[TREEVIEWSTATE_ignoreSelects] = true;
                        Focus();
                    }
                    finally
                    {
                        treeViewState[TREEVIEWSTATE_ignoreSelects] = false;
                    }

                    // Always reset the MouseupFired.
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    var tvhip = new TVHITTESTINFO
                    {
                        pt = new Point(PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam))
                    };
                    hNodeMouseDown = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhip);

                    // This gets around the TreeView behavior of temporarily moving the selection
                    // highlight to a node when the user clicks on its checkbox.
                    if ((tvhip.flags & TVHT.ONITEMSTATEICON) != 0)
                    {
                        //We donot pass the Message to the Control .. so fire MouseDowm ...
                        OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                        if (!ValidationCancelled && CheckBoxes)
                        {
                            TreeNode node = NodeFromHandle(hNodeMouseDown);
                            bool eventReturn = TreeViewBeforeCheck(node, TreeViewAction.ByMouse);
                            if (!eventReturn && node != null)
                            {
                                node.CheckedInternal = !node.CheckedInternal;
                                TreeViewAfterCheck(node, TreeViewAction.ByMouse);
                            }
                        }
                        m.Result = IntPtr.Zero;
                    }
                    else
                    {
                        WmMouseDown(ref m, MouseButtons.Left, 1);
                    }
                    downButton = MouseButtons.Left;
                    break;
                case (int)User32.WM.LBUTTONUP:
                case (int)User32.WM.RBUTTONUP:
                    var tvhi = new TVHITTESTINFO
                    {
                        pt = new Point(PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam))
                    };
                    IntPtr hnode = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhi);

                    // Important for CheckBoxes. Click needs to be fired.
                    if (hnode != IntPtr.Zero)
                    {
                        if (!ValidationCancelled && !treeViewState[TREEVIEWSTATE_doubleclickFired] & !treeViewState[TREEVIEWSTATE_mouseUpFired])
                        {
                            //If the hit-tested node here is the same as the node we hit-tested
                            //on mouse down then we will fire our OnNodeMoseClick event.
                            if (hnode == hNodeMouseDown)
                            {
                                OnNodeMouseClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), downButton, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam)));
                            }

                            OnClick(new MouseEventArgs(downButton, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                            OnMouseClick(new MouseEventArgs(downButton, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                        }

                        if (treeViewState[TREEVIEWSTATE_doubleclickFired])
                        {
                            treeViewState[TREEVIEWSTATE_doubleclickFired] = false;
                            if (!ValidationCancelled)
                            {
                                OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), downButton, 2, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam)));
                                OnDoubleClick(new MouseEventArgs(downButton, 2, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                                OnMouseDoubleClick(new MouseEventArgs(downButton, 2, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                            }
                        }
                    }
                    if (!treeViewState[TREEVIEWSTATE_mouseUpFired])
                    {
                        OnMouseUp(new MouseEventArgs(downButton, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                    }

                    treeViewState[TREEVIEWSTATE_doubleclickFired] = false;
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    Capture = false;

                    // Always clear our hit-tested node we cached on mouse down
                    hNodeMouseDown = IntPtr.Zero;
                    break;
                case (int)User32.WM.MBUTTONDBLCLK:
                    // Fire mouse up in the Wndproc.
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    WmMouseDown(ref m, MouseButtons.Middle, 2);
                    break;
                case (int)User32.WM.MBUTTONDOWN:
                    // Always reset MouseupFired.
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    WmMouseDown(ref m, MouseButtons.Middle, 1);
                    downButton = MouseButtons.Middle;
                    break;
                case (int)User32.WM.MOUSELEAVE:
                    // if the mouse leaves and then reenters the TreeView
                    // NodeHovered events should be raised.
                    prevHoveredNode = null;
                    base.WndProc(ref m);
                    break;
                case (int)User32.WM.RBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Right, 2);

                    // Just maintain state and fire double click in the final mouseUp.
                    treeViewState[TREEVIEWSTATE_doubleclickFired] = true;

                    // Fire mouse up in the Wndproc
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;

                    // Make sure we get the mouse up if it happens outside the control.
                    Capture = true;
                    break;
                case (int)User32.WM.RBUTTONDOWN:
                    //Always Reset the MouseupFired....
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    //Cache the hit-tested node for verification when mouse up is fired
                    var tvhit = new TVHITTESTINFO
                    {
                        pt = new Point(PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam))
                    };
                    hNodeMouseDown = User32.SendMessageW(this, (User32.WM)TVM.HITTEST, IntPtr.Zero, ref tvhit);

                    WmMouseDown(ref m, MouseButtons.Right, 1);
                    downButton = MouseButtons.Right;
                    break;
                case (int)User32.WM.SYSCOLORCHANGE:
                    User32.SendMessageW(this, (User32.WM)TVM.SETINDENT, (IntPtr)Indent);
                    base.WndProc(ref m);
                    break;
                case (int)User32.WM.SETFOCUS:
                    // If we get focus through the LBUttonDown .. we might have done the validation...
                    // so skip it..
                    if (treeViewState[TREEVIEWSTATE_lastControlValidated])
                    {
                        treeViewState[TREEVIEWSTATE_lastControlValidated] = false;
                        WmImeSetFocus();
                        DefWndProc(ref m);
                        InvokeGotFocus(this, EventArgs.Empty);
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;
                case (int)User32.WM.CONTEXTMENU:
                    if (treeViewState[TREEVIEWSTATE_showTreeViewContextMenu])
                    {
                        treeViewState[TREEVIEWSTATE_showTreeViewContextMenu] = false;
                        base.WndProc(ref m);
                    }
                    else
                    {
                        // this is the Shift + F10 Case....
                        TreeNode treeNode = SelectedNode;
                        if (treeNode != null && treeNode.ContextMenuStrip != null)
                        {
                            Point client;
                            client = new Point(treeNode.Bounds.X, treeNode.Bounds.Y + treeNode.Bounds.Height / 2);
                            // VisualStudio7 # 156, only show the context menu when clicked in the client area
                            if (ClientRectangle.Contains(client) && treeNode.ContextMenuStrip != null)
                            {
                                bool keyboardActivated = (unchecked((int)(long)m.LParam) == -1);
                                treeNode.ContextMenuStrip.ShowInternal(this, client, keyboardActivated);
                            }
                        }
                        else
                        {
                            // in this case we dont have a selected node.  The base
                            // will ensure we're constrained to the client area.
                            base.WndProc(ref m);
                        }
                    }
                    break;

                default:
                    base.WndProc(ref m);
                    break;
            }
        }
    }
}
