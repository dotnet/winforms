// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;
    using System;
    using System.Security.Permissions;
    using System.Security;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Design;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Collections;
    using Microsoft.Win32;
    using System.Reflection;
    using System.Windows.Forms.Layout;
    using System.Globalization;
    using System.Windows.Forms.VisualStyles;

    /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays a hierarchical list of items, or nodes. Each
    ///       node includes a caption and an optional bitmap. The user can select a node. If
    ///       it has sub-nodes, the user can collapse or expand the node.
    ///
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Nodes)),
    DefaultEvent(nameof(AfterSelect)),
    Docking(DockingBehavior.Ask),
    Designer("System.Windows.Forms.Design.TreeViewDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionTreeView))
    ]
    public class TreeView : Control {

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

        internal TreeNode selectedNode = null;
        private ImageList.Indexer imageIndexer;
        private ImageList.Indexer selectedImageIndexer;
        private bool setOddHeight = false;
        private TreeNode prevHoveredNode = null;
        private bool hoveredAlready  = false;
        private bool rightToLeftLayout = false;
        
        [SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private IntPtr hNodeMouseDown = IntPtr.Zero;//ensures we fire nodeclick on the correct node

        private const int   TREEVIEWSTATE_hideSelection             = 0x00000001;
        private const int   TREEVIEWSTATE_labelEdit                 = 0x00000002;
        private const int   TREEVIEWSTATE_scrollable                = 0x00000004;
        private const int   TREEVIEWSTATE_checkBoxes                = 0x00000008;
        private const int   TREEVIEWSTATE_showLines                 = 0x00000010;
        private const int   TREEVIEWSTATE_showPlusMinus             = 0x00000020;
        private const int   TREEVIEWSTATE_showRootLines             = 0x00000040;
        private const int   TREEVIEWSTATE_sorted                    = 0x00000080;
        private const int   TREEVIEWSTATE_hotTracking               = 0x00000100;
        private const int   TREEVIEWSTATE_fullRowSelect             = 0x00000200;
        private const int   TREEVIEWSTATE_showNodeToolTips          = 0x00000400;
        private const int   TREEVIEWSTATE_doubleclickFired          = 0x00000800;
        private const int   TREEVIEWSTATE_mouseUpFired              = 0x00001000;
        private const int   TREEVIEWSTATE_showTreeViewContextMenu   = 0x00002000;
        private const int   TREEVIEWSTATE_lastControlValidated      = 0x00004000;  
        private const int   TREEVIEWSTATE_stopResizeWindowMsgs      = 0x00008000;
        private const int   TREEVIEWSTATE_ignoreSelects             = 0x00010000;

        // PERF: take all the bools and put them into a state variable
        private System.Collections.Specialized.BitVector32          treeViewState; // see TREEVIEWSTATE_ consts above

        private static bool isScalingInitialized = false;
        private static Size? scaledStateImageSize = null;
        private static Size? ScaledStateImageSize {
            get {
                if (!isScalingInitialized) {
                    if (DpiHelper.IsScalingRequired) {
                        scaledStateImageSize = DpiHelper.LogicalToDeviceUnits(new Size(16, 16));
                    }
                    isScalingInitialized = true;
                }
                return scaledStateImageSize;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ImageIndexer"]/*' />
        /// <internalonly/>
        internal ImageList.Indexer ImageIndexer {
             get {
                if (imageIndexer == null) {
                    imageIndexer = new ImageList.Indexer();
                }
                imageIndexer.ImageList = ImageList;
                return imageIndexer;
             }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.SelectedImageIndexer"]/*' />
        /// <internalonly/>
        internal ImageList.Indexer SelectedImageIndexer {
             get {
                   if (selectedImageIndexer == null) {
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

        internal TreeNodeCollection nodes = null;
        internal TreeNode editNode;
        internal TreeNode root;
        internal Hashtable nodeTable = new Hashtable();
        internal bool nodesCollectionClear = false; //this is set when the treeNodeCollection is getting cleared and used by TreeView
        private MouseButtons downButton;
        private TreeViewDrawMode drawMode = TreeViewDrawMode.Normal;

        //Properties newly added to TreeView....
        private ImageList internalStateImageList;
        private TreeNode topNode;
        private ImageList stateImageList;
        private Color lineColor;
        private string controlToolTipText = null;

        // Sorting
        private IComparer treeViewNodeSorter = null;
             

        //Events
        private TreeNodeMouseClickEventHandler onNodeMouseClick;
        private TreeNodeMouseClickEventHandler onNodeMouseDoubleClick;

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.TreeView"]/*' />
        /// <devdoc>
        ///     Creates a TreeView control
        /// </devdoc>
        public TreeView()
        : base() {

            this.treeViewState = new System.Collections.Specialized.BitVector32(TREEVIEWSTATE_showRootLines |
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

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BackColor"]/*' />
        /// <devdoc>
        ///     The background color for this control. Specifying null for
        ///     this parameter sets the
        ///     control's background color to its parent's background color.
        /// </devdoc>
        public override Color BackColor {
            get {
                if (ShouldSerializeBackColor()) {
                    return base.BackColor;
                }
                else {
                    return SystemColors.Window;
                }
            }

            set {
                base.BackColor = value;
                if (IsHandleCreated) {
                    SendMessage(NativeMethods.TVM_SETBKCOLOR, 0, ColorTranslator.ToWin32(BackColor));

                    // This is to get around a problem in the comctl control where the lines
                    // connecting nodes don't get the new BackColor.  This messages forces
                    // reconstruction of the line bitmaps without changing anything else.
                    SendMessage(NativeMethods.TVM_SETINDENT, Indent, 0);
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BackgroundImage"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BackgroundImageChanged"]/*' />
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


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BackgroundImageLayout"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BackgroundImageLayoutChanged"]/*' />
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

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BorderStyle"]/*' />
        /// <devdoc>
        ///     The border style of the window.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(BorderStyle.Fixed3D),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.borderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                if (borderStyle != value) {
                    //verify that 'value' is a valid enum type...

                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                    }

                    borderStyle = value;
                    UpdateStyles();
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.CheckBoxes"]/*' />
        /// <devdoc>
        ///     The value of the CheckBoxes property. The CheckBoxes
        ///     property determines if check boxes are shown next to node in the
        ///     tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(false),
        SRDescription(nameof(SR.TreeViewCheckBoxesDescr))
        ]
        public bool CheckBoxes {
            get {
                return treeViewState[TREEVIEWSTATE_checkBoxes];
            }

            set {
                if (CheckBoxes != value) {
                    treeViewState[TREEVIEWSTATE_checkBoxes] = value;
                    if (IsHandleCreated) {
                        if (CheckBoxes) {
                            UpdateStyles();
                        } else {
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

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.CreateParams"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = NativeMethods.WC_TREEVIEW;


                // Keep the scrollbar if we are just updating styles...
                //
                if (IsHandleCreated) {
                    int currentStyle = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
                    cp.Style |= (currentStyle & (NativeMethods.WS_HSCROLL | NativeMethods.WS_VSCROLL));
                }
                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }

                if (!Scrollable)
                    cp.Style |= NativeMethods.LVS_NOSCROLL;

                if (!HideSelection)
                    cp.Style |= NativeMethods.TVS_SHOWSELALWAYS;
                if (LabelEdit)
                    cp.Style |= NativeMethods.TVS_EDITLABELS;
                if (ShowLines)
                    cp.Style |= NativeMethods.TVS_HASLINES;
                if (ShowPlusMinus)
                    cp.Style |= NativeMethods.TVS_HASBUTTONS;
                if (ShowRootLines)
                    cp.Style |= NativeMethods.TVS_LINESATROOT;
                if (HotTracking)
                    cp.Style |= NativeMethods.TVS_TRACKSELECT;
                if (FullRowSelect)
                    cp.Style |= NativeMethods.TVS_FULLROWSELECT;
                if (setOddHeight) {
                    cp.Style |= NativeMethods.TVS_NONEVENHEIGHT;
                }

                // Don't set TVS_CHECKBOXES here if the window isn't created yet.
                // See OnHandleCreated for explanation
                if (ShowNodeToolTips && IsHandleCreated && !DesignMode) {
                    cp.Style |= NativeMethods.TVS_INFOTIP;
                }


                // Don't set TVS_CHECKBOXES here if the window isn't created yet.
                // See OnHandleCreated for explanation
                if (CheckBoxes && IsHandleCreated)
                    cp.Style |= NativeMethods.TVS_CHECKBOXES;

                // Don't call IsMirrored from CreateParams. That will lead to some nasty problems, since
                // IsMirrored ends up calling CreateParams - you dig!
                if (RightToLeft == RightToLeft.Yes) {
                    if (RightToLeftLayout == true) {
                        //We want to turn on mirroring for TreeView explicitly.
                        cp.ExStyle |= NativeMethods.WS_EX_LAYOUTRTL;
                        //Don't need these styles when mirroring is turned on.
                        cp.ExStyle &= ~(NativeMethods.WS_EX_RTLREADING | NativeMethods.WS_EX_RIGHT | NativeMethods.WS_EX_LEFTSCROLLBAR);
                    }
                    else {
                        cp.Style |= NativeMethods.TVS_RTLREADING;
                    }
                }

                return cp;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(121, 97);
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.DoubleBuffered"]/*' />
        /// <devdoc>
        ///     This property is overridden and hidden from statement completion
        ///     on controls that are based on Win32 Native Controls.
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Never)]
        protected override bool DoubleBuffered {
            get {
                return base.DoubleBuffered;
            }
            set {
                base.DoubleBuffered = value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ForeColor"]/*' />
        /// <devdoc>
        ///     The current foreground color for this control, which is the
        ///     color the control uses to draw its text.
        /// </devdoc>
        public override Color ForeColor {
            get {
                if (ShouldSerializeForeColor()) {
                    return base.ForeColor;
                }
                else {
                    return SystemColors.WindowText;
                }
            }

            set {
                base.ForeColor = value;
                if (IsHandleCreated)
                    SendMessage(NativeMethods.TVM_SETTEXTCOLOR, 0, ColorTranslator.ToWin32(ForeColor));
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.FullRowSelect"]/*' />
        /// <devdoc>
        ///     Determines whether the selection highlight spans across the width of the TreeView.
        ///     This property will have no effect if ShowLines is true.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TreeViewFullRowSelectDescr))
        ]
        public bool FullRowSelect {
            get { return treeViewState[TREEVIEWSTATE_fullRowSelect];}
            set {
                if (FullRowSelect != value) {
                    treeViewState[TREEVIEWSTATE_fullRowSelect] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.HideSelection"]/*' />
        /// <devdoc>
        ///     The HideSelection property specifies whether the selected node will
        ///     be highlighted even when the TreeView loses focus.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TreeViewHideSelectionDescr))
        ]
        public bool HideSelection {
            get {
                return treeViewState[TREEVIEWSTATE_hideSelection];
            }

            set {
                if (HideSelection != value) {
                    treeViewState[TREEVIEWSTATE_hideSelection] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.HotTracking"]/*' />
        /// <devdoc>
        ///     The value of the HotTracking property. The HotTracking
        ///     property determines if nodes are highlighted as the mousepointer
        ///     passes over them.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TreeViewHotTrackingDescr))
        ]
        public bool HotTracking {
            get {
                return treeViewState[TREEVIEWSTATE_hotTracking];
            }

            set {
                if (HotTracking != value) {
                    treeViewState[TREEVIEWSTATE_hotTracking] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ImageIndex"]/*' />
        /// <devdoc>
        ///     The default image index for nodes in the tree view.
        /// </devdoc>
        [
        DefaultValue(-1),
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        SRDescription(nameof(SR.TreeViewImageIndexDescr)),
        RelatedImageList("ImageList")
        ]
        public int ImageIndex {
            get {
                if (imageList == null) {
                    return -1;
                }
                if (ImageIndexer.Index >= imageList.Images.Count) {
                    return Math.Max(0, imageList.Images.Count - 1);
                }
                return ImageIndexer.Index;
            }

            set {
                // If (none) is selected in the image index editor, we'll just adjust this to
                // mean image index 0. This is because a treeview must always have an image index -
                // even if no imagelist exists we want the image index to be 0.
                //
                if (value == -1) {
                    value = 0;
                }

                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(ImageIndex), string.Format(SR.InvalidLowBoundArgumentEx, "ImageIndex", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }

                if (ImageIndexer.Index != value) {
                    ImageIndexer.Index = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ImageKey"]/*' />
        /// <devdoc>
        ///     The default image index for nodes in the tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DefaultValue(""),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.TreeViewImageKeyDescr)),
        RelatedImageList("ImageList")
        ]
        public string ImageKey {
            get {
                return ImageIndexer.Key;
            }

            set {
                if (ImageIndexer.Key != value) {
                    ImageIndexer.Key = value;
                    if (String.IsNullOrEmpty(value) || value.Equals(SR.toStringNone)) {
                        ImageIndex = (ImageList != null) ? 0:-1;
                    }
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ImageList"]/*' />
        /// <devdoc>
        ///     Returns the image list control that is bound to the tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.TreeViewImageListDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public ImageList ImageList {
            get {
                return imageList;
            }
            set {
                if (value != imageList) {
       
                    DetachImageListHandlers();

                    imageList = value;

                    AttachImageListHandlers();

                    // Update TreeView's images
                    //
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.TVM_SETIMAGELIST, 0,
                                    value==null? IntPtr.Zero: value.Handle);
                        if (StateImageList != null && StateImageList.Images.Count > 0) {
                            SetStateImageList(internalStateImageList.Handle);
                        }
                    }
                    UpdateCheckedState(root, true);
                }
            }
        }

         private void AttachImageListHandlers() {
              if (imageList != null) {
                  //NOTE: any handlers added here should be removed in DetachImageListHandlers
                  imageList.RecreateHandle += new EventHandler(ImageListRecreateHandle);
                  imageList.Disposed += new EventHandler(DetachImageList);
                  imageList.ChangeHandle += new EventHandler(ImageListChangedHandle);
              }
          }
  
          private void DetachImageListHandlers() {
              if (imageList != null) {
                  imageList.RecreateHandle -= new EventHandler(ImageListRecreateHandle);
                  imageList.Disposed -= new EventHandler(DetachImageList);
                  imageList.ChangeHandle -= new EventHandler(ImageListChangedHandle);
              }
          }
  
          private void AttachStateImageListHandlers() {
              if (stateImageList != null) {
                  //NOTE: any handlers added here should be removed in DetachStateImageListHandlers
                  stateImageList.RecreateHandle += new EventHandler(StateImageListRecreateHandle);
                  stateImageList.Disposed += new EventHandler(DetachStateImageList);
                  stateImageList.ChangeHandle += new EventHandler(StateImageListChangedHandle);
              }
          }
  
          private void DetachStateImageListHandlers() {
              if (stateImageList != null) {
                  stateImageList.RecreateHandle -= new EventHandler(StateImageListRecreateHandle);
                  stateImageList.Disposed -= new EventHandler(DetachStateImageList);
                  stateImageList.ChangeHandle -= new EventHandler(StateImageListChangedHandle);
              }
          }
  

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.StateImageList"]/*' />
        /// <devdoc>
        ///     Returns the state image list control that is bound to the tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(null),
        SRDescription(nameof(SR.TreeViewStateImageListDescr))
        ]
        public ImageList StateImageList {
            get {
                return stateImageList;
            }
            set {
                if (value != stateImageList) {

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
                        
                        if((value == null || stateImageList.Images.Count == 0) && CheckBoxes) {
                            // Requires Handle Recreate to force on the checkBoxes and states..
                            RecreateHandle();
                        }
                        else {
                            // The TreeView shows up the state imageList after sending this message even if the nodes dont have any stateImageIndex set.
                            // In order to avoid that we refresh nodes which would "reset" the images to none.
                            // This causes flicker but gives us the right behavior                        
                            RefreshNodes();
                        }
                    }

                    
                }
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Indent"]/*' />
        /// <devdoc>
        ///     The indentation level in pixels.
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeViewIndentDescr))
        ]
        public int Indent {
            get {
                if (indent != -1) {
                    return indent;
                }
                else if (IsHandleCreated) {
                    return unchecked( (int) (long)SendMessage(NativeMethods.TVM_GETINDENT, 0, 0));
                }
                return DefaultTreeViewIndent;
            }

            set {
                if (indent != value) {
                    if (value < 0) {
                        throw new ArgumentOutOfRangeException(nameof(Indent), string.Format(SR.InvalidLowBoundArgumentEx, "Indent", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                    }
                    if (value > MaxIndent) {
                        throw new ArgumentOutOfRangeException(nameof(Indent), string.Format(SR.InvalidHighBoundArgumentEx, "Indent", (value).ToString(CultureInfo.CurrentCulture), (MaxIndent).ToString(CultureInfo.CurrentCulture)));
                    }
                    indent = value;
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.TVM_SETINDENT, value, 0);
                        indent = unchecked( (int) (long)SendMessage(NativeMethods.TVM_GETINDENT, 0, 0));
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ItemHeight"]/*' />
        /// <devdoc>
        ///     The height of every item in the tree view, in pixels.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        SRDescription(nameof(SR.TreeViewItemHeightDescr))
        ]
        public int ItemHeight {
            get {
                if (itemHeight != -1) {
                    return itemHeight;
                }

                if (IsHandleCreated) {
                    return unchecked( (int) (long)SendMessage(NativeMethods.TVM_GETITEMHEIGHT, 0, 0));
                }
                else {
                    if (CheckBoxes && (DrawMode == TreeViewDrawMode.OwnerDrawAll)) {
                       return Math.Max(16, FontHeight + 3);
                    }
                    return FontHeight + 3;
                }
            }

            set {
                if (itemHeight != value) {
                    if (value < 1) {
                        throw new ArgumentOutOfRangeException(nameof(ItemHeight), string.Format(SR.InvalidLowBoundArgumentEx, "ItemHeight", (value).ToString(CultureInfo.CurrentCulture), (1).ToString(CultureInfo.CurrentCulture)));
                    }
                    if (value >= Int16.MaxValue) {
                        throw new ArgumentOutOfRangeException(nameof(ItemHeight), string.Format(SR.InvalidHighBoundArgument, "ItemHeight", (value).ToString(CultureInfo.CurrentCulture), Int16.MaxValue.ToString(CultureInfo.CurrentCulture)));
                    }
                    itemHeight = value;
                    if (IsHandleCreated) {
                        if (itemHeight % 2 != 0) {
                            setOddHeight = true;
                            try {
                                RecreateHandle();
                            }
                            finally {
                                setOddHeight = false;
                            }
                        }

                        SendMessage(NativeMethods.TVM_SETITEMHEIGHT, value, 0);
                        itemHeight = unchecked( (int) (long)SendMessage(NativeMethods.TVM_GETITEMHEIGHT, 0, 0));
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.LabelEdit"]/*' />
        /// <devdoc>
        ///     The LabelEdit property determines if the label text
        ///     of nodes in the tree view is editable.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TreeViewLabelEditDescr))
        ]
        public bool LabelEdit {
            get {
                return treeViewState[TREEVIEWSTATE_labelEdit];
            }
            set {
                if (LabelEdit != value) {
                    treeViewState[TREEVIEWSTATE_labelEdit] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.LineColor"]/*' />
        /// <devdoc>
        ///     This is the color of the lines that connect the nodes of the Treeview.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.TreeViewLineColorDescr)),
        DefaultValue(typeof(Color), "Black")
        ]
        public Color LineColor {
            get {
                if (IsHandleCreated) {
                    int intColor = unchecked((int)(long)SendMessage(NativeMethods.TVM_GETLINECOLOR, 0, 0));
                    return ColorTranslator.FromWin32(intColor);
                }
                return lineColor;
            }
            set {
                if (lineColor != value) {
                    lineColor = value;
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.TVM_SETLINECOLOR, 0, ColorTranslator.ToWin32(lineColor));

                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Nodes"]/*' />
        /// <devdoc>
        ///     The collection of nodes associated with this TreeView control
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.TreeViewNodesDescr)),
        MergableProperty(false)
        ]
        public TreeNodeCollection Nodes {
            get {
                if (nodes == null) {
                    nodes = new TreeNodeCollection(root);
                }
                return nodes;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.DrawMode"]/*' />
        /// <devdoc>
        /// <para>
        /// Indicates the drawing mode for the tree view.
        /// </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(TreeViewDrawMode.Normal),
        SRDescription(nameof(SR.TreeViewDrawModeDescr))
        ]
        public TreeViewDrawMode DrawMode {
            get {
                return drawMode;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TreeViewDrawMode.Normal, (int)TreeViewDrawMode.OwnerDrawAll))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(TreeViewDrawMode));
                }

                if (drawMode != value) {
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

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.PathSeparator"]/*' />
        /// <devdoc>
        ///     The delimeter string used by TreeNode.getFullPath().
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue("\\"),
        SRDescription(nameof(SR.TreeViewPathSeparatorDescr))
        ]
        public string PathSeparator {
            get {
                return pathSeparator;
            }
            set {
                pathSeparator = value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value;}
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged {
            add { base.PaddingChanged += value; }
            remove { base.PaddingChanged -= value; }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.RightToLeftLayout"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        //      and the RightToLeft is true, mirroring will be turned on on the form, and
        ///     control placement and text will be from right to left.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.ControlRightToLeftLayoutDescr))
        ]
        public virtual bool RightToLeftLayout {
            get {

                return rightToLeftLayout;
            }

            set {
                if (value != rightToLeftLayout) {
                    rightToLeftLayout = value;
                    using(new LayoutTransaction(this, this, PropertyNames.RightToLeftLayout)) {
                        OnRightToLeftLayoutChanged(EventArgs.Empty);
                    }
                }
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Scrollable"]/*' />
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TreeViewScrollableDescr))
        ]
        public bool Scrollable {
            get {
                return treeViewState[TREEVIEWSTATE_scrollable];
            }
            set {
                if (Scrollable != value) {
                    treeViewState[TREEVIEWSTATE_scrollable] = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.SelectedImageIndex"]/*' />
        /// <devdoc>
        ///     The image index that a node will display when selected.
        ///     The index applies to the ImageList referred to by the imageList property,
        /// </devdoc>
        [
        DefaultValue(-1),
        SRCategory(nameof(SR.CatBehavior)),
        TypeConverterAttribute(typeof(NoneExcludedImageIndexConverter)),
        Localizable(true),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        SRDescription(nameof(SR.TreeViewSelectedImageIndexDescr)),
        RelatedImageList("ImageList")
        ]
        public int SelectedImageIndex {
            get {
                if (imageList == null) {
                    return -1;
                }
                if (SelectedImageIndexer.Index >= imageList.Images.Count) {
                    return Math.Max(0, imageList.Images.Count - 1);
                }
                return SelectedImageIndexer.Index;
            }
            set {
                // If (none) is selected in the image index editor, we'll just adjust this to
                // mean image index 0. This is because a treeview must always have an image index -
                // even if no imagelist exists we want the image index to be 0.
                //
                if (value == -1) {
                    value = 0;
                }

                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(SelectedImageIndex), string.Format(SR.InvalidLowBoundArgumentEx, "SelectedImageIndex", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                if (SelectedImageIndexer.Index != value) {
                    SelectedImageIndexer.Index = value;
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.SelectedImageKey"]/*' />
        /// <devdoc>
        ///     The default image index for nodes in the tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DefaultValue(""),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.TreeViewSelectedImageKeyDescr)),
        RelatedImageList("ImageList")
        ]
        public string SelectedImageKey {
            get {
                return SelectedImageIndexer.Key;
            }

            set {
                if (SelectedImageIndexer.Key != value) {
                    SelectedImageIndexer.Key = value;

                    if (String.IsNullOrEmpty(value) || value.Equals(SR.toStringNone)) {
                        SelectedImageIndex = (ImageList != null) ? 0 : -1;
                    }
                    if (IsHandleCreated) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.SelectedNode"]/*' />
        /// <devdoc>
        ///     The currently selected tree node, or null if nothing is selected.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TreeViewSelectedNodeDescr))
        ]
        public TreeNode SelectedNode {
            get {
                if (IsHandleCreated) {
                    Debug.Assert(selectedNode == null || selectedNode.TreeView != this, "handle is created, but we're still caching selectedNode");
                    IntPtr hItem = SendMessage(NativeMethods.TVM_GETNEXTITEM, NativeMethods.TVGN_CARET, 0);
                    if (hItem == IntPtr.Zero)
                        return null;
                    return NodeFromHandle(hItem);
                }
                else if (selectedNode != null && selectedNode.TreeView == this) {
                    return selectedNode;
                }
                else {
                    return null;
                }
            }
            set {
                if (IsHandleCreated && (value == null || value.TreeView == this)) {
                    // This class invariant is not quite correct -- if the selected node does not belong to this Treeview,
                    // selectedNode != null even though the handle is created.  We will call set_SelectedNode
                    // to inform the handle that the selected node has been added to the TreeView.
                    Debug.Assert(selectedNode == null || selectedNode.TreeView != this, "handle is created, but we're still caching selectedNode");

                    IntPtr hnode = (value == null ? IntPtr.Zero : value.Handle);
                    SendMessage(NativeMethods.TVM_SELECTITEM, NativeMethods.TVGN_CARET, hnode);
                    selectedNode = null;
                }
                else {
                    selectedNode = value;
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ShowLines"]/*' />
        /// <devdoc>
        ///     The ShowLines property determines if lines are drawn between
        ///     nodes in the tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TreeViewShowLinesDescr))
        ]
        public bool ShowLines {
            get {
                return treeViewState[TREEVIEWSTATE_showLines];
            }
            set {
                if (ShowLines != value) {
                    treeViewState[TREEVIEWSTATE_showLines] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ShowNodeToolTips"]/*' />
        /// <devdoc>
        ///     The ShowLines property determines whether or not the tooltips willbe displayed on the nodes
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TreeViewShowShowNodeToolTipsDescr))
        ]
        public bool ShowNodeToolTips {
            get {
                return treeViewState[TREEVIEWSTATE_showNodeToolTips];
            }
            set {
                if (ShowNodeToolTips != value) {
                    treeViewState[TREEVIEWSTATE_showNodeToolTips] = value;
                    if (ShowNodeToolTips)
                        RecreateHandle();
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ShowPlusMinus"]/*' />
        /// <devdoc>
        ///     The ShowPlusMinus property determines if the "plus/minus"
        ///     expand button is shown next to tree nodes that have children.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TreeViewShowPlusMinusDescr))
        ]
        public bool ShowPlusMinus {
            get {
                return treeViewState[TREEVIEWSTATE_showPlusMinus];
            }
            set {
                if (ShowPlusMinus != value) {
                    treeViewState[TREEVIEWSTATE_showPlusMinus] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ShowRootLines"]/*' />
        /// <devdoc>
        ///     Determines if lines are draw between nodes at the root of
        ///     the tree view.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.TreeViewShowRootLinesDescr))
        ]
        public bool ShowRootLines {
            get { return treeViewState[TREEVIEWSTATE_showRootLines];}
            set {
                if (ShowRootLines != value) {
                    treeViewState[TREEVIEWSTATE_showRootLines] = value;
                    if (IsHandleCreated) {
                        UpdateStyles();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Sorted"]/*' />
        /// <devdoc>
        ///     The Sorted property determines if nodes in the tree view are sorted.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.TreeViewSortedDescr)),
        Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
        ]
        public bool Sorted {
            get {
                return treeViewState[TREEVIEWSTATE_sorted];
            }
            set {
                if (Sorted != value) {
                    treeViewState[TREEVIEWSTATE_sorted] = value;
                    if (Sorted && TreeViewNodeSorter == null && Nodes.Count >= 1) {
                        RefreshNodes();
                    }
                }
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.TreeViewItemSorter"]/*' />
        /// <devdoc>
        ///     The sorting comparer for this TreeView.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TreeViewNodeSorterDescr))
        ]
        public IComparer TreeViewNodeSorter {
            get {
                return treeViewNodeSorter;
            }
            set {
                if (treeViewNodeSorter != value) {
                    treeViewNodeSorter = value;
                    if (value != null) {
                        Sort();
                    }
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Text"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.TextChanged"]/*' />
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

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.TopNode"]/*' />
        /// <devdoc>
        ///     The first visible node in the TreeView. Initially
        ///     the first root node is at the top of the TreeView, but if the
        ///     contents have been scrolled another node may be at the top.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TreeViewTopNodeDescr))
        ]
        public TreeNode TopNode {
            get {
                if (IsHandleCreated) {
                    IntPtr hitem = SendMessage(NativeMethods.TVM_GETNEXTITEM, NativeMethods.TVGN_FIRSTVISIBLE, 0);
                    return(hitem == IntPtr.Zero ? null : NodeFromHandle(hitem));
                }
                return topNode;
            }
            set {
                if (IsHandleCreated && (value == null || value.TreeView == this)) {
                    // This class invariant is not quite correct -- if the selected node does not belong to this Treeview,
                    // selectedNode != null even though the handle is created.  We will call set_SelectedNode
                    // to inform the handle that the selected node has been added to the TreeView.
                    Debug.Assert(topNode == null || topNode.TreeView != this, "handle is created, but we're still caching selectedNode");

                    IntPtr hnode = (value == null ? IntPtr.Zero : value.Handle);
                    SendMessage(NativeMethods.TVM_SELECTITEM, NativeMethods.TVGN_FIRSTVISIBLE, hnode);
                    topNode = null;
                }
                else {
                    topNode = value;
                }

            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.VisibleCount"]/*' />
        /// <devdoc>
        ///     The count of fully visible nodes in the tree view.  This number
        ///     may be greater than the number of nodes in the control.
        ///     The control calculates this value by dividing the height of the
        ///     client window by the height of an item
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.TreeViewVisibleCountDescr))
        ]
        public int VisibleCount {
            get {
                if (IsHandleCreated)
                    return unchecked( (int) (long)SendMessage(NativeMethods.TVM_GETVISIBLECOUNT, 0, 0));

                return 0;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BeforeLabelEdit"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewBeforeEditDescr))]
        public event NodeLabelEditEventHandler BeforeLabelEdit {
            add {
                onBeforeLabelEdit += value;
            }
            remove {
                onBeforeLabelEdit -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.AfterLabelEdit"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewAfterEditDescr))]
        public event NodeLabelEditEventHandler AfterLabelEdit {
            add {
                onAfterLabelEdit += value;
            }
            remove {
                onAfterLabelEdit -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BeforeCheck"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewBeforeCheckDescr))]
        public event TreeViewCancelEventHandler BeforeCheck {
            add {
                onBeforeCheck += value;
            }
            remove {
                onBeforeCheck -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.AfterCheck"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewAfterCheckDescr))]
        public event TreeViewEventHandler AfterCheck {
            add {
                onAfterCheck += value;
            }
            remove {
                onAfterCheck -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BeforeCollapse"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewBeforeCollapseDescr))]
        public event TreeViewCancelEventHandler BeforeCollapse {
            add {
                onBeforeCollapse += value;
            }
            remove {
                onBeforeCollapse -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.AfterCollapse"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewAfterCollapseDescr))]
        public event TreeViewEventHandler AfterCollapse {
            add {
                onAfterCollapse += value;
            }
            remove {
                onAfterCollapse -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BeforeExpand"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewBeforeExpandDescr))]
        public event TreeViewCancelEventHandler BeforeExpand {
            add {
                onBeforeExpand += value;
            }
            remove {
                onBeforeExpand -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.AfterExpand"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewAfterExpandDescr))]
        public event TreeViewEventHandler AfterExpand {
            add {
                onAfterExpand += value;
            }
            remove {
                onAfterExpand -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.DrawNode"]/*' />
        /// <devdoc>
        ///    <para>Fires when a TreeView node needs to be drawn.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewDrawNodeEventDescr))]
        public event DrawTreeNodeEventHandler DrawNode {
            add {
                onDrawNode += value;
            }
            remove {
                onDrawNode -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ItemDrag"]/*' />
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.ListViewItemDragDescr))]
        public event ItemDragEventHandler ItemDrag {
            add {
                onItemDrag += value;
            }
            remove {
                onItemDrag -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.NodeMouseHover"]/*' />
        [SRCategory(nameof(SR.CatAction)), SRDescription(nameof(SR.TreeViewNodeMouseHoverDescr))]
        public event TreeNodeMouseHoverEventHandler NodeMouseHover  {
            add {
                onNodeMouseHover += value;
            }
            remove {
                onNodeMouseHover -= value;
            }
        }



        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BeforeSelect"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewBeforeSelectDescr))]
        public event TreeViewCancelEventHandler BeforeSelect {
            add {
                onBeforeSelect += value;
            }
            remove {
                onBeforeSelect -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.AfterSelect"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewAfterSelectDescr))]
        public event TreeViewEventHandler AfterSelect {
            add {
                onAfterSelect += value;
            }
            remove {
                onAfterSelect -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnPaint"]/*' />
        /// <devdoc>
        ///     TreeView Onpaint.
        /// </devdoc>
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.NodeMouseClick"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewNodeMouseClickDescr))]
        public event TreeNodeMouseClickEventHandler NodeMouseClick {
            add {
                onNodeMouseClick += value;
            }
            remove {
                onNodeMouseClick -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.NodeMouseDoubleClick"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.TreeViewNodeMouseDoubleClickDescr))]
        public event TreeNodeMouseClickEventHandler NodeMouseDoubleClick {
            add {
                onNodeMouseDoubleClick += value;
            }
            remove {
                onNodeMouseDoubleClick -= value;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.RightToLeftLayoutChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.ControlOnRightToLeftLayoutChangedDescr))]
        public event EventHandler RightToLeftLayoutChanged {
            add {
                onRightToLeftLayoutChanged += value;
            }
            remove {
                onRightToLeftLayoutChanged -= value;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.BeginUpdate"]/*' />
        /// <devdoc>
        ///     Disables redrawing of the tree view. A call to beginUpdate() must be
        ///     balanced by a following call to endUpdate(). Following a call to
        ///     beginUpdate(), any redrawing caused by operations performed on the
        ///     tree view is deferred until the call to endUpdate().
        /// </devdoc>
        public void BeginUpdate() {
            BeginUpdateInternal();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.CollapseAll"]/*' />
        /// <devdoc>
        ///     Collapses all nodes at the root level.
        /// </devdoc>
        public void CollapseAll() {
            root.Collapse();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.CreateHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void CreateHandle() {
            if (!RecreatingHandle) {
                IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                try {
                    NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                    icc.dwICC = NativeMethods.ICC_TREEVIEW_CLASSES;
                    SafeNativeMethods.InitCommonControlsEx(icc);
                } finally {
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
                }
            }
            base.CreateHandle();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.DetachImageList"]/*' />
        /// <devdoc>
        ///     Resets the imageList to null.  We wire this method up to the imageList's
        ///     Dispose event, so that we don't hang onto an imageList that's gone away.
        /// </devdoc>
        /// <internalonly/>
        private void DetachImageList(object sender, EventArgs e) {
            ImageList = null;
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.DetachStateImageList"]/*' />
        /// <devdoc>
        ///     Resets the stateimageList to null.  We wire this method up to the stateimageList's
        ///     Dispose event, so that we don't hang onto an stateimageList that's gone away.
        /// </devdoc>
        /// <internalonly/>
        private void DetachStateImageList(object sender, EventArgs e) {
            internalStateImageList = null;
            StateImageList = null;

        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Dispose"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void Dispose(bool disposing) {
            if (disposing) {

                foreach (TreeNode node in Nodes) {
                    node.ContextMenu = null;
                }

                // 
                lock(this) {
                    DetachImageListHandlers();
                    imageList = null;
                    DetachStateImageListHandlers();
                    stateImageList = null;
                }

            }

            base.Dispose(disposing);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.EndUpdate"]/*' />
        /// <devdoc>
        ///     Reenables redrawing of the tree view. A call to beginUpdate() must be
        ///     balanced by a following call to endUpdate(). Following a call to
        ///     beginUpdate(), any redrawing caused by operations performed on the
        ///     combo box is deferred until the call to endUpdate().
        /// </devdoc>
        public void EndUpdate() {
            EndUpdateInternal();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ExpandAll"]/*' />
        /// <devdoc>
        ///     Expands all nodes at the root level.
        /// </devdoc>
        public void ExpandAll() {
            root.ExpandAll();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ForceScrollbarUpdate"]/*' />
        /// <devdoc>
        ///     Forces the TreeView to recalculate all its nodes widths so that it updates the
        ///     scrollbars as appropriate.
        /// </devdoc>
        /// <internalonly/>
        internal void ForceScrollbarUpdate(bool delayed) {

            // ForceScrollbarUpdate call WM_SETREDRAW( FALSE ) followed by WM_SETREDRAW( TRUE )
            // So if TreeView.BeginUpdate is called
            // ForceScrollbarUpdate effectively causes tree view to ignore BeginUpdate and cause control to update on every change.
            // So gaurd against this scenario by using the new internal method on Control.
            if (!IsUpdating())
            {
                if (IsHandleCreated) {
                    SendMessage(NativeMethods.WM_SETREDRAW, 0, 0);
                    if (delayed)
                        UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), NativeMethods.WM_SETREDRAW, (IntPtr)1, IntPtr.Zero);
                    else
                        SendMessage(NativeMethods.WM_SETREDRAW, 1, 0);
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.SetToolTip"]/*' />
        /// <devdoc>
        ///     Called by ToolTip to poke in that Tooltip into this ComCtl so that the Native ChildToolTip is not exposed.
        /// </devdoc>
        /// <internalonly/>
        internal void SetToolTip(ToolTip toolTip, string toolTipText) {
            if (toolTip != null) {
                UnsafeNativeMethods.SendMessage(new HandleRef(toolTip, toolTip.Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TVM_SETTOOLTIPS, new HandleRef(toolTip, toolTip.Handle), 0);
                controlToolTipText = toolTipText;
            }
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.HitTest"]/*' />
        /// <devdoc>
        ///     Gives the information about which part of the treeNode is at the given point.
        /// </devdoc>
        public TreeViewHitTestInfo HitTest(Point pt) {
            return HitTest(pt.X, pt.Y);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.HitTest"]/*' />
        /// <devdoc>
        ///     Gives the information about which part of the treeNode is at the given x, y.
        /// </devdoc>
        public TreeViewHitTestInfo HitTest(int x, int y) {
            NativeMethods.TV_HITTESTINFO tvhi = new NativeMethods.TV_HITTESTINFO();
            tvhi.pt_x = x;
            tvhi.pt_y = y;
            IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhi);
            TreeNode node = (hnode == IntPtr.Zero ? null : NodeFromHandle(hnode));
            TreeViewHitTestLocations loc = (TreeViewHitTestLocations)tvhi.flags;
            return (new TreeViewHitTestInfo(node, loc));
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.TreeViewBeforeAfterCheck"]/*' />
        /// <devdoc>
        ///     Defined so that a  tree node can use it
        ///
        /// </devdoc>
        /// <internalonly/>

        internal bool TreeViewBeforeCheck(TreeNode node, TreeViewAction actionTaken) {
            TreeViewCancelEventArgs tvce = new TreeViewCancelEventArgs(node, false, actionTaken);
            OnBeforeCheck(tvce);
            return (tvce.Cancel);
        }

        internal void TreeViewAfterCheck(TreeNode node, TreeViewAction actionTaken) {
            OnAfterCheck(new TreeViewEventArgs(node, actionTaken));
        }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.GetNodeCount"]/*' />
        /// <devdoc>
        ///     Returns count of nodes at root, optionally including all subtrees.
        /// </devdoc>
        public int GetNodeCount(bool includeSubTrees) {
            return root.GetNodeCount(includeSubTrees);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.GetNodeAt"]/*' />
        /// <devdoc>
        ///     Returns the TreeNode at the given location in tree view coordinates.
        /// </devdoc>
        public TreeNode GetNodeAt(Point pt) {
            return GetNodeAt(pt.X, pt.Y);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.GetNodeAt1"]/*' />
        /// <devdoc>
        ///     Returns the TreeNode at the given location in tree view coordinates.
        /// </devdoc>
        public TreeNode GetNodeAt(int x, int y) {
            NativeMethods.TV_HITTESTINFO tvhi = new NativeMethods.TV_HITTESTINFO();

            tvhi.pt_x = x;
            tvhi.pt_y = y;

            IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhi);

            return(hnode == IntPtr.Zero ? null : NodeFromHandle(hnode));
        }

        private void ImageListRecreateHandle(object sender, EventArgs e) {
            if (IsHandleCreated) {
                IntPtr handle = (ImageList == null) ? IntPtr.Zero : ImageList.Handle;
                SendMessage(NativeMethods.TVM_SETIMAGELIST, 0, handle);
            }
        }

        private void UpdateImagesRecursive ( TreeNode node )
        {
            node.UpdateImage();
            // Iterate only through the Nodes collection rather than the
            // array since an item might have been removed from the collection, and
            // correspondingly "removed" from the array, but still exist in the array
            // since the array isn't actually re-dimensioned down to a smaller size.
            foreach (TreeNode child in node.Nodes) {
                UpdateImagesRecursive(child);
            }
        }

        private void ImageListChangedHandle(object sender, EventArgs e) {
            if ((null != sender) && (sender == imageList) && IsHandleCreated) {
                BeginUpdate();
                foreach (TreeNode node in Nodes) {
                    UpdateImagesRecursive(node);
                }
                EndUpdate();
            }
        }

        private void StateImageListRecreateHandle(object sender, EventArgs e) {
            if (IsHandleCreated) {
                IntPtr handle = IntPtr.Zero;
                if (internalStateImageList != null) {
                    handle = internalStateImageList.Handle;
                }
                SetStateImageList(handle);
            }
        }

        private void StateImageListChangedHandle(object sender, EventArgs e) {
            if ((null != sender) && (sender == stateImageList) && IsHandleCreated) {
                // Since the native treeview requires the state imagelist to be 1-indexed we need to 
                // re add the images if the original collection had changed.
                if (stateImageList != null && stateImageList.Images.Count > 0) {
                    Image[] images = new Image[stateImageList.Images.Count + 1];
                    images[0] = stateImageList.Images[0];
                    for (int i = 1;  i <= stateImageList.Images.Count ; i++) {
                        images[i] = stateImageList.Images[i -1];
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

                        SetStateImageList(internalStateImageList.Handle);
                    }
                }
                else //stateImageList == null || stateImageList.Images.Count = 0; 
                {
                    UpdateCheckedState(root, true);
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.IsInputKey"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Overridden to handle RETURN key.
        ///    </para>
        /// </devdoc>
        protected override bool IsInputKey(Keys keyData) {
            // If in edit mode, treat Return as an input key, so the form doesn't grab it
            // and treat it as clicking the Form.AcceptButton.  Similarly for Escape
            // and Form.CancelButton.
            if (editNode != null && (keyData & Keys.Alt) == 0) {
                switch (keyData & Keys.KeyCode) {
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

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.NodeFromHandle"]/*' />
        /// <devdoc>
        ///    Note this can be null - particularly if any windows messages get generated during
        ///    the insertion of a tree node (TVM_INSERTITEM)
        /// </devdoc>
        /// <internalonly/>
        internal TreeNode NodeFromHandle(IntPtr handle) {
            TreeNode node = (TreeNode)nodeTable[handle];

            return node;
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnDrawNode"]/*' />
        /// <devdoc>
        /// Fires the DrawNode event.
        /// </devdoc>
        protected virtual void OnDrawNode(DrawTreeNodeEventArgs e) {
        if (onDrawNode != null) onDrawNode(this, e);
    }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnHandleCreated"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {

            TreeNode savedSelectedNode = this.selectedNode;
            this.selectedNode = null;

            base.OnHandleCreated(e);

            int version = unchecked((int)(long)SendMessage(NativeMethods.CCM_GETVERSION, 0, 0));
            if (version < 5) {
                SendMessage(NativeMethods.CCM_SETVERSION, 5, 0);
            }

            // Workaround for problem in TreeView where it doesn't recognize the TVS_CHECKBOXES
            // style if it is set before the window is created.  To get around the problem,
            // we set it here after the window is created, and we make sure we don't set it
            // in getCreateParams so that this will actually change the value of the bit.
            // This seems to make the Treeview happy.
            if (CheckBoxes) {
                int style = unchecked((int)(UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
                style |= NativeMethods.TVS_CHECKBOXES;
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)style));
            }

            if (ShowNodeToolTips && !DesignMode) {
                int style = unchecked((int)(UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
                style |= NativeMethods.TVS_INFOTIP;
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)style));
            }

            Color c;
            c = BackColor;
            if (c != SystemColors.Window)
                SendMessage(NativeMethods.TVM_SETBKCOLOR, 0, ColorTranslator.ToWin32(c));
            c = ForeColor;

            if (c != SystemColors.WindowText)
                SendMessage(NativeMethods.TVM_SETTEXTCOLOR, 0, ColorTranslator.ToWin32(c));

            /// put the linecolor into the native control only if Set ...
            if (lineColor != Color.Empty) {
                SendMessage(NativeMethods.TVM_SETLINECOLOR, 0, ColorTranslator.ToWin32(lineColor));
            }

            if (imageList != null)
                SendMessage(NativeMethods.TVM_SETIMAGELIST, 0, imageList.Handle);

            if (stateImageList != null)
            {
                UpdateNativeStateImageList();
            }

            if (indent != -1) {
                SendMessage(NativeMethods.TVM_SETINDENT, indent, 0);
            }

            if (itemHeight != -1) {
                SendMessage(NativeMethods.TVM_SETITEMHEIGHT, ItemHeight, 0);
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
                oldSize = this.Width;
                int flags = NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOMOVE;
                SafeNativeMethods.SetWindowPos(new HandleRef(this, this.Handle), NativeMethods.NullHandleRef, this.Left, this.Top, Int32.MaxValue, this.Height, flags);

                root.Realize(false);

                if (oldSize != 0)
                {
                    SafeNativeMethods.SetWindowPos(new HandleRef(this, this.Handle), NativeMethods.NullHandleRef, this.Left, this.Top, oldSize, this.Height, flags);
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
                SendMessage(NativeMethods.TVM_SETIMAGELIST, NativeMethods.TVSIL_STATE, newImageList.Handle);

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
            IntPtr handleOld = SendMessage(NativeMethods.TVM_SETIMAGELIST, NativeMethods.TVSIL_STATE, handle);
            if ((handleOld != IntPtr.Zero) && (handleOld != handle))
            {
                SafeNativeMethods.ImageList_Destroy_Native(new HandleRef(this, handleOld));
            }
        }
        
        // Destroying the tree-view control does not destroy the native state image list. 
        // We must destroy it explicitly. 
        private void DestroyNativeStateImageList(bool reset)
        {
            IntPtr handle = SendMessage(NativeMethods.TVM_GETIMAGELIST, NativeMethods.TVSIL_STATE, IntPtr.Zero);
            if (handle != IntPtr.Zero)
            {
                SafeNativeMethods.ImageList_Destroy_Native(new HandleRef(this, handle));
                if (reset)
                {
                    SendMessage(NativeMethods.TVM_SETIMAGELIST, NativeMethods.TVSIL_STATE, IntPtr.Zero);
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnHandleDestroyed"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleDestroyed(EventArgs e) {
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


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnMouseHover"]/*' />
        /// <devdoc>
        ///  We keep track of if we've hovered already so we don't fire multiple hover events
        /// </devdoc>
        /// <internalonly>
        protected override void OnMouseLeave(EventArgs e) {
            hoveredAlready = false;
            base.OnMouseLeave(e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnMouseHover"]/*' />
        /// <devdoc>
        ///     In order for the MouseHover event to fire for each item in a TreeView,
        ///     the node the mouse is hovering over is found. Each time a new node is hovered
        ///     over a new event is raised.
        /// </devdoc>
        protected override void OnMouseHover(EventArgs e)  {

            /// Hover events need to be caught for each node
            /// within the TreeView so the appropriate
            /// NodeHovered event can be raised.

            NativeMethods.TV_HITTESTINFO tvhip = new NativeMethods.TV_HITTESTINFO();
            Point pos = Cursor.Position;
            pos = PointToClientInternal(pos);
            tvhip.pt_x = pos.X;
            tvhip.pt_y = pos.Y;
            IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhip);

            if (hnode != IntPtr.Zero && ((tvhip.flags & NativeMethods.TVHT_ONITEM) != 0)) {
                TreeNode tn = NodeFromHandle(hnode);
                if (tn != prevHoveredNode && tn != null) {
                    OnNodeMouseHover( new TreeNodeMouseHoverEventArgs(tn));
                    prevHoveredNode = tn;
                }
            }

            if (!hoveredAlready) {
                base.OnMouseHover(e);
                hoveredAlready = true;
            }

            ResetMouseEventArgs();

        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnBeforeLabelEdit"]/*' />
        /// <devdoc>
        ///     Fires the beforeLabelEdit event.
        /// </devdoc>
        protected virtual void OnBeforeLabelEdit(NodeLabelEditEventArgs e) {
            Contract.Requires(e != null);
            if (onBeforeLabelEdit != null) onBeforeLabelEdit(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnAfterLabelEdit"]/*' />
        /// <devdoc>
        ///     Fires the afterLabelEdit event.
        /// </devdoc>
        protected virtual void OnAfterLabelEdit(NodeLabelEditEventArgs e) {
            Contract.Requires(e != null);
            if (onAfterLabelEdit != null) onAfterLabelEdit(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnBeforeCheck"]/*' />
        /// <devdoc>
        ///     Fires the beforeCheck event.
        /// </devdoc>
        protected virtual void OnBeforeCheck(TreeViewCancelEventArgs e) {
            Contract.Requires(e != null);
            if (onBeforeCheck != null) onBeforeCheck(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnAfterCheck"]/*' />
        /// <devdoc>
        ///     Fires the afterCheck event.
        /// </devdoc>
        protected virtual void OnAfterCheck(TreeViewEventArgs e) {
            Contract.Requires(e != null);
            if (onAfterCheck != null) onAfterCheck(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnBeforeCollapse"]/*' />
        /// <devdoc>
        ///     Fires the beforeCollapse event.
        /// </devdoc>
        protected internal virtual void OnBeforeCollapse(TreeViewCancelEventArgs e) {
            Contract.Requires(e != null);
            if (onBeforeCollapse != null) onBeforeCollapse(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnAfterCollapse"]/*' />
        /// <devdoc>
        ///     Fires the afterCollapse event.
        /// </devdoc>
        protected internal virtual void OnAfterCollapse(TreeViewEventArgs e) {
            Contract.Requires(e != null);
            if (onAfterCollapse != null) onAfterCollapse(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnBeforeExpand"]/*' />
        /// <devdoc>
        ///     Fires the beforeExpand event.
        /// </devdoc>
        protected virtual void OnBeforeExpand(TreeViewCancelEventArgs e) {
            Contract.Requires(e != null);
            if (onBeforeExpand != null) onBeforeExpand(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnAfterExpand"]/*' />
        /// <devdoc>
        ///     Fires the afterExpand event.
        /// </devdoc>
        protected virtual void OnAfterExpand(TreeViewEventArgs e) {
            Contract.Requires(e != null);
            if (onAfterExpand != null) onAfterExpand(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnItemDrag"]/*' />
        /// <devdoc>
        ///     Fires the ItemDrag event.
        /// </devdoc>
        protected virtual void OnItemDrag(ItemDragEventArgs e) {
            Contract.Requires(e != null);
            if (onItemDrag != null) onItemDrag(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnNodeMouseHover"]/*' />
        /// <devdoc>
        ///     Fires the NodeMouseHover event.
        /// </devdoc>
        protected virtual void OnNodeMouseHover(TreeNodeMouseHoverEventArgs e) {
            Contract.Requires(e != null);
            if (onNodeMouseHover != null) onNodeMouseHover(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnBeforeSelect"]/*' />
        /// <devdoc>
        ///     Fires the beforeSelect event.
        /// </devdoc>
        protected virtual void OnBeforeSelect(TreeViewCancelEventArgs e) {
            Contract.Requires(e != null);
            if (onBeforeSelect != null) onBeforeSelect(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnAfterSelect"]/*' />
        /// <devdoc>
        ///     Fires the afterSelect event.
        /// </devdoc>
        protected virtual void OnAfterSelect(TreeViewEventArgs e) {
            Contract.Requires(e != null);
            if (onAfterSelect != null) onAfterSelect(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.onNodeMouseClick"]/*' />
        /// <devdoc>
        ///     Fires the onNodeMouseClick event.
        /// </devdoc>
        protected virtual void OnNodeMouseClick(TreeNodeMouseClickEventArgs e) {
            Contract.Requires(e != null);
            if (onNodeMouseClick != null) onNodeMouseClick(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.onNodeMouseDoubleClick"]/*' />
        /// <devdoc>
        ///     Fires the onNodeMouseDoubleClick event.
        /// </devdoc>
        protected virtual void OnNodeMouseDoubleClick(TreeNodeMouseClickEventArgs e) {
            Contract.Requires(e != null);
            if (onNodeMouseDoubleClick != null) onNodeMouseDoubleClick(this, e);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnKeyDown"]/*' />
        /// <devdoc>
        ///     Handles the OnBeforeCheck / OnAfterCheck for keyboard clicks
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyDown(KeyEventArgs e) {
            base.OnKeyDown(e);
            if (e.Handled) return;
            // if it's a space, send the check notifications and toggle the checkbox if we're not
            // cancelled.
            if (CheckBoxes && (e.KeyData & Keys.KeyCode) == Keys.Space) {
                TreeNode node = this.SelectedNode;
                if (node != null) {
                    bool eventReturn = TreeViewBeforeCheck(node, TreeViewAction.ByKeyboard);
                    if (!eventReturn) {
                        node.CheckedInternal = !node.CheckedInternal;
                        TreeViewAfterCheck(node, TreeViewAction.ByKeyboard);
                    }
                    e.Handled = true;
                    return;
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnKeyUp"]/*' />
        /// <devdoc>
        ///     Handles the OnBeforeCheck / OnAfterCheck for keyboard clicks
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyUp(KeyEventArgs e) {
            base.OnKeyUp(e);
            if (e.Handled) return;
            // eat the space key
            if ((e.KeyData & Keys.KeyCode) == Keys.Space) {
                e.Handled = true;
                return;
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.OnKeyPress"]/*' />
        /// <devdoc>
        ///     Handles the OnBeforeCheck / OnAfterCheck for keyboard clicks
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyPress(KeyPressEventArgs e) {
            base.OnKeyPress(e);
            if (e.Handled) return;
            // eat the space key
            if (e.KeyChar == ' ') e.Handled = true;
        }


        /// <include file='doc\Form.uex' path='docs/doc[@for="Form.OnRightToLeftLayoutChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected virtual void OnRightToLeftLayoutChanged(EventArgs e) {
            Contract.Requires(e != null);
            if (GetAnyDisposingInHierarchy()) {
                return;
            }

            if (RightToLeft == RightToLeft.Yes) {
                RecreateHandle();
            }

            if (onRightToLeftLayoutChanged != null) {
                 onRightToLeftLayoutChanged(this, e);
            }
        }



        // Refresh the nodes by clearing the tree and adding the nodes back again
        //
        private void RefreshNodes() {
            TreeNode[] nodes = new TreeNode[Nodes.Count];
            Nodes.CopyTo(nodes, 0);

            Nodes.Clear();
            Nodes.AddRange(nodes);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ResetIndent"]/*' />
        /// <devdoc>
        ///     This resets the indentation to the system default.
        /// </devdoc>
        private void ResetIndent() {
            indent = -1;
            // is this overkill?
            RecreateHandle();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ResetItemHeight"]/*' />
        /// <devdoc>
        ///     This resets the item height to the system default.
        /// </devdoc>
        private void ResetItemHeight() {
            itemHeight = -1;
            RecreateHandle();
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ShouldSerializeIndent"]/*' />
        /// <devdoc>
        ///     Retrieves true if the indent should be persisted in code gen.
        /// </devdoc>
        private bool ShouldSerializeIndent() {
            return(indent != -1);
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ShouldSerializeItemHeight"]/*' />
        /// <devdoc>
        ///     Retrieves true if the itemHeight should be persisted in code gen.
        /// </devdoc>
        private bool ShouldSerializeItemHeight() {
            return(itemHeight != -1);
        }

        private bool ShouldSerializeSelectedImageIndex() {
             if (imageList != null) {
                 return (SelectedImageIndex != 0);
             }
             return (SelectedImageIndex != -1);
         }


         private bool ShouldSerializeImageIndex() {
             if (imageList != null) {
                 return (ImageIndex != 0);
             }
             return (ImageIndex != -1);
         }


        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.Sort"]/*' />
        /// <devdoc>
        ///      Updated the sorted order
        /// </devdoc>
        public void Sort() {
            Sorted = true;
            RefreshNodes();
        }

        
        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            if (Nodes != null) {
                s += ", Nodes.Count: " + Nodes.Count.ToString(CultureInfo.CurrentCulture);
                if (Nodes.Count > 0)
                    s += ", Nodes[0]: " + Nodes[0].ToString();
            }
            return s;
        }

        private unsafe void TvnBeginDrag(MouseButtons buttons, NativeMethods.NMTREEVIEW* nmtv) {
            NativeMethods.TV_ITEM item = nmtv->itemNew;

            // Check for invalid node handle
            if (item.hItem == IntPtr.Zero) {
                return;
            }

            TreeNode node = NodeFromHandle(item.hItem);

            OnItemDrag(new ItemDragEventArgs(buttons, node));
        }

        private unsafe IntPtr TvnExpanding(NativeMethods.NMTREEVIEW* nmtv) {
            NativeMethods.TV_ITEM item = nmtv->itemNew;

            // Check for invalid node handle
            if (item.hItem == IntPtr.Zero) {
                return IntPtr.Zero;
            }

            TreeViewCancelEventArgs e = null;
            if ((item.state & NativeMethods.TVIS_EXPANDED) == 0) {
                e = new TreeViewCancelEventArgs(NodeFromHandle(item.hItem), false, TreeViewAction.Expand);
                OnBeforeExpand(e);
            }
            else {
                e = new TreeViewCancelEventArgs(NodeFromHandle(item.hItem), false, TreeViewAction.Collapse);
                OnBeforeCollapse(e);
            }
            return (IntPtr)(e.Cancel? 1: 0);
        }

        private unsafe void TvnExpanded(NativeMethods.NMTREEVIEW* nmtv) {
            NativeMethods.TV_ITEM item = nmtv->itemNew;

            // Check for invalid node handle
            if (item.hItem == IntPtr.Zero) {
                return;
            }

            TreeViewEventArgs e;
            TreeNode node = NodeFromHandle(item.hItem);

            // Note that IsExpanded is invalid for the moment, so we use item item.state to branch.
            if ((item.state & NativeMethods.TVIS_EXPANDED) == 0) {
                e = new TreeViewEventArgs(node, TreeViewAction.Collapse);
                OnAfterCollapse(e);
            }
            else {
                e = new TreeViewEventArgs(node, TreeViewAction.Expand);
                OnAfterExpand(e);
            }
        }

        private unsafe IntPtr TvnSelecting(NativeMethods.NMTREEVIEW* nmtv) {
            if (treeViewState[ TREEVIEWSTATE_ignoreSelects])
            {
                return (IntPtr)1;
            }
            // Check for invalid node handle
            if (nmtv->itemNew.hItem == IntPtr.Zero) {
                return IntPtr.Zero;
            }

            TreeNode node = NodeFromHandle(nmtv->itemNew.hItem);

            TreeViewAction action = TreeViewAction.Unknown;
            switch(nmtv->action) {
                case NativeMethods.TVC_BYKEYBOARD:
                    action = TreeViewAction.ByKeyboard;
                    break;
                case NativeMethods.TVC_BYMOUSE:
                    action = TreeViewAction.ByMouse;
                    break;
            }

            TreeViewCancelEventArgs e = new TreeViewCancelEventArgs(node, false, action);
            OnBeforeSelect(e);

            return (IntPtr)(e.Cancel? 1: 0);
        }

        private unsafe void TvnSelected(NativeMethods.NMTREEVIEW* nmtv) {
            if (nodesCollectionClear) //if called thru the Clear( ) of treeNodeCollection then just return...
            {
                return;
            }
            if (nmtv->itemNew.hItem != IntPtr.Zero) {
                TreeViewAction action = TreeViewAction.Unknown;
                switch(nmtv->action) {
                    case NativeMethods.TVC_BYKEYBOARD:
                        action = TreeViewAction.ByKeyboard;
                        break;
                    case NativeMethods.TVC_BYMOUSE:
                        action = TreeViewAction.ByMouse;
                        break;
                }
                OnAfterSelect(new TreeViewEventArgs(NodeFromHandle(nmtv->itemNew.hItem), action));
            }

            // TreeView doesn't properly revert back to the unselected image
            // if the unselected image is blank.
            //
            NativeMethods.RECT rc = new NativeMethods.RECT();
            *((IntPtr *) &rc.left) = nmtv->itemOld.hItem;
            if (nmtv->itemOld.hItem != IntPtr.Zero) {
                if (unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_GETITEMRECT, 1, ref rc)) != 0)
                    SafeNativeMethods.InvalidateRect(new HandleRef(this, Handle), ref rc, true);
            }
        }

        private IntPtr TvnBeginLabelEdit(NativeMethods.NMTVDISPINFO nmtvdi) {

            // Check for invalid node handle
            if (nmtvdi.item.hItem == IntPtr.Zero) {
                return IntPtr.Zero;
            }

            TreeNode editingNode = NodeFromHandle(nmtvdi.item.hItem);
            NodeLabelEditEventArgs e = new NodeLabelEditEventArgs(editingNode);
            OnBeforeLabelEdit(e);
            if (!e.CancelEdit)
                editNode = editingNode;
            return (IntPtr)(e.CancelEdit ? 1 : 0);
        }

        private IntPtr TvnEndLabelEdit(NativeMethods.NMTVDISPINFO nmtvdi) {
            editNode = null;

            // Check for invalid node handle
            if (nmtvdi.item.hItem == IntPtr.Zero) {
                return (IntPtr)1;
            }

            TreeNode node = NodeFromHandle(nmtvdi.item.hItem);
            string newText = (nmtvdi.item.pszText == IntPtr.Zero ? null : Marshal.PtrToStringAuto(nmtvdi.item.pszText));
            NodeLabelEditEventArgs e = new NodeLabelEditEventArgs(node, newText);
            OnAfterLabelEdit(e);
            if (newText != null && !e.CancelEdit && node != null) {
                node.text = newText;
                if (Scrollable)
                    ForceScrollbarUpdate(true);
            }
            return (IntPtr)(e.CancelEdit ? 0 : 1);
        }

        internal override void UpdateStylesCore() {
            base.UpdateStylesCore();
            if (IsHandleCreated && CheckBoxes) {
                if (StateImageList != null) {
                    // Setting the TVS_CHECKBOXES window style also causes the TreeView to display the default checkbox
                    // images rather than the user specified StateImageList.  We send a TVM_SETIMAGELIST to restore the
                    // user's images.
                    if (internalStateImageList != null) {
                        SetStateImageList(internalStateImageList.Handle);
                    }

                }
            }
        }

        // Setting the NativeMethods.TVS_CHECKBOXES style clears the checked state
        private void UpdateCheckedState(TreeNode node, bool update) {
            // This looks funny, but CheckedInternal returns the cached isChecked value and the internal
            // setter will blindly issue TVM_SETITEM so this gets us back in sync.
            if (update)
            {
                node.CheckedInternal = node.CheckedInternal;
                for (int i = node.Nodes.Count - 1; i >= 0; i--) {
                    UpdateCheckedState(node.Nodes[i], update);
                }
            }
            else
            {
                node.CheckedInternal = false;
                for (int i = node.Nodes.Count - 1; i >= 0; i--) {
                    UpdateCheckedState(node.Nodes[i], update);
                }
            }
        }

        private void WmMouseDown(ref Message m, MouseButtons button, int clicks) {
            // Required to put the TreeView in sane-state for painting proper highlighting of selectedNodes.
            // If the user shows the ContextMenu bu overiding the WndProc( ), then the treeview
            // goes into the weird state where the high-light gets locked to the node on which the ContextMenu was shown.
            // So we need to get the native TREEIVEW out of this weird state.
            SendMessage(NativeMethods.TVM_SELECTITEM, NativeMethods.TVGN_DROPHILITE, null);

            // Windows TreeView pushes its own message loop in WM_xBUTTONDOWN, so fire the
            // event before calling defWndProc or else it won't get fired until the button
            // comes back up.
            OnMouseDown(new MouseEventArgs(button, clicks, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));

            //If Validation is cancelled dont fire any events through the Windows TreeView's message loop...
            if (!ValidationCancelled) {
                DefWndProc(ref m);
            }

        }

       

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.CustomDraw"]/*' />
        /// <devdoc>
        ///     Performs custom draw handling
        /// </devdoc>
        /// <internalonly/>
        private void CustomDraw(ref Message m) {
            NativeMethods.NMTVCUSTOMDRAW nmcd = (NativeMethods.NMTVCUSTOMDRAW)m.GetLParam(typeof(NativeMethods.NMTVCUSTOMDRAW));

            // Find out which stage we're drawing
            switch (nmcd.nmcd.dwDrawStage) {
                // Do we want OwnerDraw for this paint cycle?
                case NativeMethods.CDDS_PREPAINT:
                    m.Result = (IntPtr)NativeMethods.CDRF_NOTIFYITEMDRAW; // yes, we do...
                    return;
                    // We've got opt-in on owner draw for items - so handle each one.
                case NativeMethods.CDDS_ITEMPREPAINT:
                    // get the node
                    Debug.Assert(nmcd.nmcd.dwItemSpec != IntPtr.Zero, "Invalid node handle in ITEMPREPAINT");
                    TreeNode node = NodeFromHandle((IntPtr)nmcd.nmcd.dwItemSpec);

                    if (node == null) {
                       // this can happen if we are presently inserting the node - it hasn't yet
                       // been added to the handle table

                       m.Result = (IntPtr)(NativeMethods.CDRF_SKIPDEFAULT);
                       return;
                    }


                    int state = nmcd.nmcd.uItemState;


                    // The commctrl TreeView allows you to draw the whole row of a node
                    // or nothing at all. The way we provide OwnerDrawText is by asking it
                    // to draw everything but the text - to do this, we set text color same
                    // as background color.
                    if (drawMode == TreeViewDrawMode.OwnerDrawText)
                    {
                        nmcd.clrText = nmcd.clrTextBk;
                        Marshal.StructureToPtr(nmcd, m.LParam, false);
                        m.Result = (IntPtr) (NativeMethods.CDRF_NEWFONT | NativeMethods.CDRF_NOTIFYPOSTPAINT);
                        return;
                    }
                    else if (drawMode == TreeViewDrawMode.OwnerDrawAll)
                    {
                        Graphics g = Graphics.FromHdcInternal(nmcd.nmcd.hdc);

                        DrawTreeNodeEventArgs e;

                        try {
                            Rectangle bounds = node.RowBounds;
                            
                            NativeMethods.SCROLLINFO si = new NativeMethods.SCROLLINFO();
                            si.cbSize = Marshal.SizeOf(typeof(NativeMethods.SCROLLINFO));
                            si.fMask = NativeMethods.SIF_POS;
                            if (UnsafeNativeMethods.GetScrollInfo(new HandleRef(this, Handle), NativeMethods.SB_HORZ,si) != false) {

                                // need to get the correct bounds if horizontal scroll bar is shown.
                                // In this case the bounds.X needs to be negative and width needs to be updated to the increased width (scrolled region).
                                int value = si.nPos;
                                if (value > 0)
                                {
                                    bounds.X -= value;
                                    bounds.Width += value;
                                }
                            }   
                            e = new DrawTreeNodeEventArgs(g, node, bounds, (TreeNodeStates) (state));
                            OnDrawNode(e);
                        }
                        finally {
                            g.Dispose();
                        }

                        if (!e.DrawDefault) {
                            m.Result = (IntPtr)(NativeMethods.CDRF_SKIPDEFAULT);
                            return;
                        }
                    }

                    //TreeViewDrawMode.Normal case
#if DEBUGGING
                    // Diagnostic output
                    Debug.WriteLine("Itemstate: "+state);
                    Debug.WriteLine("Itemstate: "+
                                            "\nDISABLED" + (((state & NativeMethods.CDIS_DISABLED) != 0) ? "TRUE" : "FALSE") +
                                            "\nHOT" + (((state & NativeMethods.CDIS_HOT) != 0) ? "TRUE" : "FALSE") +
                                            "\nGRAYED" + (((state & NativeMethods.CDIS_GRAYED) != 0) ? "TRUE" : "FALSE") +
                                            "\nSELECTED" + (((state & NativeMethods.CDIS_SELECTED) != 0) ? "TRUE" : "FALSE") +
                                            "\nFOCUS" + (((state & NativeMethods.CDIS_FOCUS) != 0) ? "TRUE" : "FALSE") +
                                            "\nDEFAULT" + (((state & NativeMethods.CDIS_DEFAULT) != 0) ? "TRUE" : "FALSE") +
                                            "\nMARKED" + (((state & NativeMethods.CDIS_MARKED) != 0) ? "TRUE" : "FALSE") +
                                            "\nINDETERMINATE" + (((state & NativeMethods.CDIS_INDETERMINATE) != 0) ? "TRUE" : "FALSE"));
#endif

                   OwnerDrawPropertyBag renderinfo = GetItemRenderStyles(node,state);

                    // TreeView has problems with drawing items at times; it gets confused
                    // as to which colors apply to which items (see focus rectangle shifting;
                    // when one item is selected, click and hold on another). This needs to be fixed.

                    bool colordelta = false;
                    Color riFore = renderinfo.ForeColor;
                    Color riBack = renderinfo.BackColor;
                    if (renderinfo != null && !riFore.IsEmpty) {
                        nmcd.clrText = ColorTranslator.ToWin32(riFore);
                        colordelta = true;
                    }
                    if (renderinfo != null && !riBack.IsEmpty) {
                        nmcd.clrTextBk = ColorTranslator.ToWin32(riBack);
                        colordelta = true;
                    }
                    if (colordelta) {
                        Marshal.StructureToPtr(nmcd, m.LParam, false);
                    }
                    if (renderinfo != null && renderinfo.Font != null) {
                        // Mess with the DC directly...
                        SafeNativeMethods.SelectObject(new HandleRef(nmcd.nmcd, nmcd.nmcd.hdc), new HandleRef(renderinfo, renderinfo.FontHandle));
                        // There is a problem in winctl that clips node fonts if the fontsize
                        // is larger than the treeview font size. The behavior is much better in comctl 5 and above.
                        m.Result = (IntPtr)NativeMethods.CDRF_NEWFONT;
                        return;
                    }

                    // fall through and do the default drawing work
                    goto default;

                case (NativeMethods.CDDS_ITEMPOSTPAINT):
                    //User draws only the text in OwnerDrawText mode, as explained in comments above
                    if (drawMode == TreeViewDrawMode.OwnerDrawText)
                    {
                        Debug.Assert(nmcd.nmcd.dwItemSpec != IntPtr.Zero, "Invalid node handle in ITEMPOSTPAINT");

                        // Get the node
                        node = NodeFromHandle((IntPtr)nmcd.nmcd.dwItemSpec);

                        if (node == null) {
                            // this can happen if we are presently inserting the node - it hasn't yet
                            // been added to the handle table
                            return;
                        }



                        Graphics g = Graphics.FromHdcInternal(nmcd.nmcd.hdc);

                        DrawTreeNodeEventArgs e;

                        try {
                            Rectangle bounds = node.Bounds;
                            Size textSize = TextRenderer.MeasureText(node.Text, node.TreeView.Font);
                            Point textLoc = new Point(bounds.X -1, bounds.Y); // required to center the text
                            bounds = new Rectangle(textLoc, new Size(textSize.Width, bounds.Height));
                                
                            e = new DrawTreeNodeEventArgs(g, node, bounds, (TreeNodeStates) (nmcd.nmcd.uItemState));
                            OnDrawNode(e);

                            if (e.DrawDefault) {
                                //Simulate default text drawing here
                                TreeNodeStates curState = e.State;

                                Font font = (node.NodeFont != null) ? node.NodeFont : node.TreeView.Font;
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
                                    using (Brush brush = new SolidBrush(BackColor))
                                    {
                                        g.FillRectangle(brush, bounds);
                                    }

                                    TextRenderer.DrawText(g, e.Node.Text, font, bounds, color, TextFormatFlags.Default);
                                }
                            }
                        }
                        finally {
                            g.Dispose();
                        }

                        m.Result = (IntPtr)NativeMethods.CDRF_NOTIFYSUBITEMDRAW;
                        return;
                    }

                    goto default;

                default:
                    // just in case we get a spurious message, tell it to do the right thing
                    m.Result = (IntPtr)NativeMethods.CDRF_DODEFAULT;
                    return;
                }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.GetItemRenderStyles"]/*' />
        /// <devdoc>
        ///     Generates colors for each item. This can be overridden to provide colors on a per state/per node
        ///     basis, rather than using the ForeColor/BackColor/NodeFont properties on TreeNode.
        ///
        /// </devdoc>
        /// <internalonly/>
        protected OwnerDrawPropertyBag GetItemRenderStyles(TreeNode node, int state) {
            OwnerDrawPropertyBag retval = new OwnerDrawPropertyBag();
            if (node == null || node.propBag == null) return retval;

            // we only change colors if we're displaying things normally
            if ((state & (NativeMethods.CDIS_SELECTED | NativeMethods.CDIS_GRAYED | NativeMethods.CDIS_HOT | NativeMethods.CDIS_DISABLED))==0) {
                retval.ForeColor = node.propBag.ForeColor;
                retval.BackColor = node.propBag.BackColor;
            }
            retval.Font = node.propBag.Font;
            return retval;
        }

        private unsafe bool WmShowToolTip(ref Message m)
        {
            NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR*)m.LParam;
            IntPtr tooltipHandle = nmhdr->hwndFrom;
            
                
            NativeMethods.TV_HITTESTINFO tvhip = new NativeMethods.TV_HITTESTINFO();
            Point pos = Cursor.Position;
            pos = PointToClientInternal(pos);
            tvhip.pt_x = pos.X;
            tvhip.pt_y = pos.Y;
            IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhip);

            if (hnode != IntPtr.Zero && ((tvhip.flags & NativeMethods.TVHT_ONITEM) != 0)) {

                TreeNode tn = NodeFromHandle(hnode);
                if (tn != null) 
                {
                    if (!ShowNodeToolTips) // default ToolTips
                    {
                        Rectangle bounds = tn.Bounds;
                        bounds.Location = this.PointToScreen(bounds.Location);

                        UnsafeNativeMethods.SendMessage(new HandleRef(this, tooltipHandle), NativeMethods.TTM_ADJUSTRECT, 1, ref bounds);
                        SafeNativeMethods.SetWindowPos(new HandleRef(this, tooltipHandle),
                                NativeMethods.HWND_TOPMOST, bounds.Left, bounds.Top, 0, 0, NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOZORDER);
                        return true;
                    }
                }
            }
            return false;
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.WmNeedText"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmNeedText(ref Message m) {
            NativeMethods.TOOLTIPTEXT ttt = (NativeMethods.TOOLTIPTEXT) m.GetLParam(typeof(NativeMethods.TOOLTIPTEXT));
            string tipText = controlToolTipText;
            
            NativeMethods.TV_HITTESTINFO tvhip = new NativeMethods.TV_HITTESTINFO();
            Point pos = Cursor.Position;
            pos = PointToClientInternal(pos);
            tvhip.pt_x = pos.X;
            tvhip.pt_y = pos.Y;
            IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhip);
            if (hnode != IntPtr.Zero && ((tvhip.flags & NativeMethods.TVHT_ONITEM) != 0)) {
                TreeNode tn = NodeFromHandle(hnode);
                if (ShowNodeToolTips && tn != null && (!string.IsNullOrEmpty(tn.ToolTipText))) {
                    tipText = tn.ToolTipText;
                }
                else if (tn != null && tn.Bounds.Right > this.Bounds.Right) {
                    tipText = tn.Text;
                }
                else {
                    tipText = null;
                }
            }
            ttt.lpszText = tipText;
            ttt.hinst = IntPtr.Zero;

            // RightToLeft reading order
            //
            if (RightToLeft == RightToLeft.Yes) {
                ttt.uFlags |= NativeMethods.TTF_RTLREADING;
            }
            Marshal.StructureToPtr(ttt, m.LParam, false);
        }


        private unsafe void WmNotify(ref Message m) {
            NativeMethods.NMHDR* nmhdr = (NativeMethods.NMHDR *)m.LParam;
            
            // Custom draw code is handled separately.
            //
            if ((nmhdr->code ==  NativeMethods.NM_CUSTOMDRAW)) {
                CustomDraw(ref m);
            }
            else {
                
                NativeMethods.NMTREEVIEW* nmtv = (NativeMethods.NMTREEVIEW*)m.LParam;

                switch (nmtv->nmhdr.code) {
                    case NativeMethods.TVN_ITEMEXPANDING:
                        m.Result = TvnExpanding(nmtv);
                        break;
                    case NativeMethods.TVN_ITEMEXPANDED:
                        TvnExpanded(nmtv);
                        break;
                    case NativeMethods.TVN_SELCHANGING:
                        m.Result = TvnSelecting(nmtv);
                        break;
                    case NativeMethods.TVN_SELCHANGED:
                        TvnSelected(nmtv);
                        break;
                    case NativeMethods.TVN_BEGINDRAG:
                        TvnBeginDrag(MouseButtons.Left, nmtv);
                        break;
                    case NativeMethods.TVN_BEGINRDRAG:
                        TvnBeginDrag(MouseButtons.Right, nmtv);
                        break;
                    case NativeMethods.TVN_BEGINLABELEDIT:
                        m.Result = TvnBeginLabelEdit((NativeMethods.NMTVDISPINFO)m.GetLParam(typeof(NativeMethods.NMTVDISPINFO)));
                        break;
                    case NativeMethods.TVN_ENDLABELEDIT:
                        m.Result = TvnEndLabelEdit((NativeMethods.NMTVDISPINFO)m.GetLParam(typeof(NativeMethods.NMTVDISPINFO)));
                        break;
                    case NativeMethods.NM_CLICK:
                    case NativeMethods.NM_RCLICK:
                        MouseButtons button = MouseButtons.Left;

                        NativeMethods.TV_HITTESTINFO tvhip = new NativeMethods.TV_HITTESTINFO();
                        Point pos = Cursor.Position;
                        pos = PointToClientInternal(pos);
                        tvhip.pt_x = pos.X;
                        tvhip.pt_y = pos.Y;
                        IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhip);
                        if (nmtv->nmhdr.code != NativeMethods.NM_CLICK
                                    || (tvhip.flags & NativeMethods.TVHT_ONITEM) != 0) {
                                button = nmtv->nmhdr.code == NativeMethods.NM_CLICK
                                    ? MouseButtons.Left : MouseButtons.Right;
                        }

                        // The treeview's WndProc doesn't get the WM_LBUTTONUP messages when
                        // LBUTTONUP happens on TVHT_ONITEM. This is a comctl quirk.
                        // We work around that by calling OnMouseUp here.
                        if (nmtv->nmhdr.code != NativeMethods.NM_CLICK
                            || (tvhip.flags & NativeMethods.TVHT_ONITEM) != 0 || FullRowSelect) {
                            if (hnode != IntPtr.Zero && !ValidationCancelled) {
                                OnNodeMouseClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), button, 1, pos.X, pos.Y));
                                OnClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));
                                OnMouseClick(new MouseEventArgs(button, 1, pos.X, pos.Y, 0));

                            }
                        }
                        if (nmtv->nmhdr.code == NativeMethods.NM_RCLICK) {
                            TreeNode treeNode = NodeFromHandle(hnode);
                            if (treeNode != null && (treeNode.ContextMenu != null || treeNode.ContextMenuStrip != null)) {
                                ShowContextMenu(treeNode);
                            }
                            else {
                                treeViewState[TREEVIEWSTATE_showTreeViewContextMenu] = true;
                                SendMessage(NativeMethods.WM_CONTEXTMENU, Handle, SafeNativeMethods.GetMessagePos());
                            }
                            m.Result = (IntPtr)1;

                        }

                        if (!treeViewState[TREEVIEWSTATE_mouseUpFired]) {
                            if (nmtv->nmhdr.code != NativeMethods.NM_CLICK
                            || (tvhip.flags & NativeMethods.TVHT_ONITEM) != 0) {
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

        /// <include file='doc\Treenode.uex' path='docs/doc[@for="Treenode.ShowContextMenu"]/*' />
        /// <devdoc>
        ///     Shows the context menu for the Treenode.
        /// </devdoc>
        /// <internalonly/>
        private void ShowContextMenu(TreeNode treeNode) {

            if (treeNode.ContextMenu != null || treeNode.ContextMenuStrip != null) {


                ContextMenu contextMenu = treeNode.ContextMenu;
                ContextMenuStrip menu = treeNode.ContextMenuStrip;
                
                if (contextMenu != null)
                {

                    NativeMethods.POINT pt = new NativeMethods.POINT();
                    UnsafeNativeMethods.GetCursorPos(pt);
                                        
                    // Summary: the current window must be made the foreground window
                    // before calling TrackPopupMenuEx, and a task switch must be
                    // forced after the call.

                    UnsafeNativeMethods.SetForegroundWindow(new HandleRef(this, this.Handle));

                    contextMenu.OnPopup( EventArgs.Empty );

                    SafeNativeMethods.TrackPopupMenuEx(new HandleRef(contextMenu, contextMenu.Handle),
                                             NativeMethods.TPM_VERTICAL,
                                             pt.x,
                                             pt.y,
                                             new HandleRef(this, this.Handle),
                                             null);

                    // Force task switch (see above)
                    UnsafeNativeMethods.PostMessage(new HandleRef(this, this.Handle), NativeMethods.WM_NULL, IntPtr.Zero, IntPtr.Zero);
                }
                // Need to send TVM_SELECTITEM to highlight the node while the contextMenuStrip is being shown.
                else if (menu != null)
                {
                    UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), NativeMethods.TVM_SELECTITEM, NativeMethods.TVGN_DROPHILITE, treeNode.Handle);
                    menu.ShowInternal(this, PointToClient(MousePosition),/*keyboardActivated*/false);
                    menu.Closing += new ToolStripDropDownClosingEventHandler(this.ContextMenuStripClosing);
                }
            }
        }

        // Need to send TVM_SELECTITEM to reset the node-highlighting while the contextMenuStrip is being closed so that the treeview reselects the SelectedNode.
        private void ContextMenuStripClosing(object sender, ToolStripDropDownClosingEventArgs e)
        {
            ContextMenuStrip strip = sender as ContextMenuStrip;
            // Unhook the Event.
            strip.Closing -= new ToolStripDropDownClosingEventHandler(this.ContextMenuStripClosing);
            SendMessage(NativeMethods.TVM_SELECTITEM, NativeMethods.TVGN_DROPHILITE, null);
        }

        private void WmPrint(ref Message m) {
            base.WndProc(ref m);

            if ((NativeMethods.PRF_NONCLIENT & (int)m.LParam) != 0 && Application.RenderWithVisualStyles && this.BorderStyle == BorderStyle.Fixed3D) {
                IntSecurity.UnmanagedCode.Assert();
                try {
                    using (Graphics g = Graphics.FromHdc(m.WParam)) {
                        Rectangle rect = new Rectangle(0, 0, this.Size.Width - 1, this.Size.Height - 1);
                        g.DrawRectangle(new Pen(VisualStyleInformation.TextControlBorder), rect);
                        rect.Inflate(-1, -1);
                        g.DrawRectangle(SystemPens.Window, rect);
                    }
                }
                finally {
                    CodeAccessPermission.RevertAssert();
                }
            }
        }

        /// <include file='doc\TreeView.uex' path='docs/doc[@for="TreeView.WndProc"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                case NativeMethods.WM_WINDOWPOSCHANGING:
                case NativeMethods.WM_NCCALCSIZE:
                case NativeMethods.WM_WINDOWPOSCHANGED:
                case NativeMethods.WM_SIZE:
                    // While we are changing size of treeView to avoid the scrollbar; dont respond to the window-sizing messages.
                    if (treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs])
                    {
                        //Debug.WriteLineIf(treeViewState[TREEVIEWSTATE_stopResizeWindowMsgs], "Sending message directly to DefWndProc() : " + m.ToString());
                        DefWndProc(ref m);
                    }
                    else
                    {
                        base.WndProc(ref m);
                    }
                    break;                        
               case NativeMethods.WM_HSCROLL:
                    base.WndProc(ref m);
                    if (DrawMode == TreeViewDrawMode.OwnerDrawAll)
                    {
                        Invalidate();
                    }
                    break;

        case NativeMethods.WM_PRINT:
            WmPrint(ref m);
            break;
                case NativeMethods.TVM_SETITEM:
                    base.WndProc(ref m);
                    if (this.CheckBoxes) {
                        NativeMethods.TV_ITEM item = (NativeMethods.TV_ITEM) m.GetLParam(typeof(NativeMethods.TV_ITEM));
                        // Check for invalid node handle
                        if (item.hItem != IntPtr.Zero) {
                            NativeMethods.TV_ITEM item1 = new NativeMethods.TV_ITEM();
                            item1.mask = NativeMethods.TVIF_HANDLE | NativeMethods.TVIF_STATE;
                            item1.hItem = item.hItem;
                            item1.stateMask = NativeMethods.TVIS_STATEIMAGEMASK;
                            UnsafeNativeMethods.SendMessage(new HandleRef(null, this.Handle), NativeMethods.TVM_GETITEM, 0, ref item1);

                            TreeNode node = NodeFromHandle(item.hItem);
                            node.CheckedStateInternal = ((item1.state >> 12) > 1);
                        }
                    }
                    break;
                case NativeMethods.WM_NOTIFY:
                    NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR) m.GetLParam(typeof(NativeMethods.NMHDR));
                    switch (nmhdr.code) {
                        case NativeMethods.TTN_GETDISPINFO:
                            // MSDN:
                            // Setting the max width has the added benefit of enabling multiline
                            // tool tips!
                            //
                            UnsafeNativeMethods.SendMessage(new HandleRef(nmhdr, nmhdr.hwndFrom), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                            WmNeedText(ref m);
                            m.Result = (IntPtr)1;
                            return;
                        case NativeMethods.TTN_SHOW:
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
                case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                        WmNotify(ref m);
                        break;
                case NativeMethods.WM_LBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Left, 2);
                    //just maintain state and fire double click.. in final mouseUp...
                    treeViewState[TREEVIEWSTATE_doubleclickFired] = true;
                    //fire Up in the Wndproc !!
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    //problem getting the UP... outside the control...
                    //
                    CaptureInternal = true;
                    break;
                case NativeMethods.WM_LBUTTONDOWN:
                    try
                    {
                        treeViewState[TREEVIEWSTATE_ignoreSelects] = true;
                        FocusInternal();
                    }
                    finally
                    {
                       treeViewState[ TREEVIEWSTATE_ignoreSelects] = false;
                    }
                    //Always Reset the MouseupFired....
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    NativeMethods.TV_HITTESTINFO tvhip = new NativeMethods.TV_HITTESTINFO();
                    tvhip.pt_x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    tvhip.pt_y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    hNodeMouseDown = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhip);

                    // This gets around the TreeView behavior of temporarily moving the selection
                    // highlight to a node when the user clicks on its checkbox.
                    if ((tvhip.flags & NativeMethods.TVHT_ONITEMSTATEICON) != 0) {
                        //We donot pass the Message to the Control .. so fire MouseDowm ...
                        OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        if (!ValidationCancelled && CheckBoxes)
                        {
                            TreeNode node = NodeFromHandle(hNodeMouseDown);
                            bool eventReturn = TreeViewBeforeCheck(node, TreeViewAction.ByMouse);
                            if (!eventReturn && node != null) {
                                node.CheckedInternal = !node.CheckedInternal;
                                TreeViewAfterCheck(node, TreeViewAction.ByMouse);
                            }
                        }
                        m.Result = IntPtr.Zero;
                    }
                    else {
                        WmMouseDown(ref m, MouseButtons.Left, 1);
                    }
                    downButton = MouseButtons.Left;
                    break;
                case NativeMethods.WM_LBUTTONUP:
                case NativeMethods.WM_RBUTTONUP:
                    NativeMethods.TV_HITTESTINFO tvhi = new NativeMethods.TV_HITTESTINFO();
                    tvhi.pt_x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    tvhi.pt_y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    IntPtr hnode = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhi);
                    //Important for CheckBoxes ... click needs to be fired ...
                    //
                    if(hnode != IntPtr.Zero) {
                        if (!ValidationCancelled && !treeViewState[TREEVIEWSTATE_doubleclickFired] & !treeViewState[TREEVIEWSTATE_mouseUpFired]) {
                            //OnClick(EventArgs.Empty);

                            //If the hit-tested node here is the same as the node we hit-tested
                            //on mouse down then we will fire our OnNodeMoseClick event.
                            if (hnode == hNodeMouseDown) {
                                OnNodeMouseClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), downButton, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam)));
                            }
                            
                            OnClick(new MouseEventArgs(downButton, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                            OnMouseClick(new MouseEventArgs(downButton, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                        }

                        if (treeViewState[TREEVIEWSTATE_doubleclickFired]) {
                            treeViewState[TREEVIEWSTATE_doubleclickFired] = false;
                            if (!ValidationCancelled) {
                                 //OnDoubleClick(EventArgs.Empty);
                                 OnNodeMouseDoubleClick(new TreeNodeMouseClickEventArgs(NodeFromHandle(hnode), downButton, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam)));
                                 OnDoubleClick(new MouseEventArgs(downButton, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                                 OnMouseDoubleClick(new MouseEventArgs(downButton, 2, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                            }
                        }
                    }
                    if (!treeViewState[TREEVIEWSTATE_mouseUpFired])
                        OnMouseUp(new MouseEventArgs(downButton, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                    treeViewState[TREEVIEWSTATE_doubleclickFired] = false;
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    CaptureInternal = false;

                    //always clear our hit-tested node we cached on mouse down
                    hNodeMouseDown = IntPtr.Zero;
                    break;
                case NativeMethods.WM_MBUTTONDBLCLK:
                    //fire Up in the Wndproc !!
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    WmMouseDown(ref m, MouseButtons.Middle, 2);
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    //Always Reset the MouseupFired....
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    WmMouseDown(ref m, MouseButtons.Middle, 1);
                    downButton = MouseButtons.Middle;
                    break;
                case NativeMethods.WM_MOUSELEAVE:
                    // if the mouse leaves and then reenters the TreeView
                    // NodeHovered events should be raised.
                    prevHoveredNode = null;
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_RBUTTONDBLCLK:
                    WmMouseDown(ref m, MouseButtons.Right, 2);
                    //just maintain state and fire double click.. in final mouseUp...
                    treeViewState[TREEVIEWSTATE_doubleclickFired] = true;
                    //fire Up in the Wndproc !!
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    //problem getting the UP... outside the control...
                    //
                    CaptureInternal = true;
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    //Always Reset the MouseupFired....
                    treeViewState[TREEVIEWSTATE_mouseUpFired] = false;
                    //Cache the hit-tested node for verification when mouse up is fired
                    NativeMethods.TV_HITTESTINFO tvhit = new NativeMethods.TV_HITTESTINFO();
                    tvhit.pt_x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    tvhit.pt_y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    hNodeMouseDown = UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TVM_HITTEST, 0, tvhit);
                    
                    WmMouseDown(ref m, MouseButtons.Right, 1);
                    downButton = MouseButtons.Right;
                    break;
                case NativeMethods.WM_SYSCOLORCHANGE:
                    SendMessage(NativeMethods.TVM_SETINDENT, Indent, 0);
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_SETFOCUS:
                     // If we get focus through the LBUttonDown .. we might have done the validation...
                     // so skip it..
                     if (treeViewState[TREEVIEWSTATE_lastControlValidated])
                     {
                        treeViewState[TREEVIEWSTATE_lastControlValidated] = false;
                        WmImeSetFocus();
                        DefWndProc(ref m);
                        this.InvokeGotFocus(this, EventArgs.Empty);
                     }
                     else 
                     {
                        base.WndProc(ref m);
                     }
                     break;
                case NativeMethods.WM_CONTEXTMENU:
                    if (treeViewState[TREEVIEWSTATE_showTreeViewContextMenu]) {
                        treeViewState[TREEVIEWSTATE_showTreeViewContextMenu] = false;
                        base.WndProc(ref m);
                    }
                    else {
                        // this is the Shift + F10 Case....
                        TreeNode treeNode = SelectedNode;
                        if (treeNode != null && (treeNode.ContextMenu != null || treeNode.ContextMenuStrip !=null)) {
                            Point client;
                            client = new Point(treeNode.Bounds.X , treeNode.Bounds.Y + treeNode.Bounds.Height / 2);
                            // VisualStudio7 # 156, only show the context menu when clicked in the client area
                            if (ClientRectangle.Contains( client )) {
                                if (treeNode.ContextMenu != null) {
                                    treeNode.ContextMenu.Show(this, client);
                                }
                                else if (treeNode.ContextMenuStrip !=null) {
                                    bool keyboardActivated =  (unchecked((int)(long)m.LParam) == -1);
                                    treeNode.ContextMenuStrip.ShowInternal(this, client, keyboardActivated);
                                }
                            }
                        }
                        else {
                            // in this case we dont have a selected node.  The base
                            // will ensure we're constrained to the client area.
                            base.WndProc (ref m);
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
