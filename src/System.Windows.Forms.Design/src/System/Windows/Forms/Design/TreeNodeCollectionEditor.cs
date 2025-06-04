// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design;

internal class TreeNodeCollectionEditor : CollectionEditor
{
    public TreeNodeCollectionEditor(Type type) : base(type)
    { }

    /// <summary>
    ///  Creates a new form to show the current collection.
    ///  You may inherit from CollectionForm to provide your own form.
    /// </summary>
    protected override CollectionForm CreateCollectionForm() => new TreeNodeCollectionForm(this);

    /// <summary>
    ///  Gets the help topic to display for the dialog help button or pressing F1.
    ///  Override to display a different help topic.
    /// </summary>
    protected override string HelpTopic => "net.ComponentModel.TreeNodeCollectionEditor";

    private class TreeNodeCollectionForm : CollectionForm
    {
        private int _nextNode;
        private TreeNode _curNode;
        private readonly TreeNodeCollectionEditor _editor;
        private Button _okButton;
        private Button _btnCancel;
        private Button _btnAddChild;
        private Button _btnAddRoot;
        private Button _btnDelete;
        private Button _moveDownButton;
        private Button _moveUpButton;
        private Label _label1;
        private TreeView _treeView1;
        private Label _label2;
        private VsPropertyGrid _propertyGrid1;
        private TableLayoutPanel _okCancelPanel;
        private TableLayoutPanel _nodeControlPanel;
        private TableLayoutPanel _overarchingTableLayoutPanel;
        private TableLayoutPanel _navigationButtonsTableLayoutPanel;

        private static readonly object s_nextNodeKey = new object();
        private readonly int _initialNextNode;

        public TreeNodeCollectionForm(CollectionEditor editor)
            : base(editor)
        {
            _editor = (TreeNodeCollectionEditor)editor;
            InitializeComponent();
            HookEvents();

            // Cache in the initial value before add so that we can put this value back
            // if the operation is cancelled.
            _initialNextNode = NextNode;
            SetButtonsState();

            // Enable explorer tree view style.
            DesignerUtils.ApplyTreeViewThemeStyles(_treeView1);

            if (_moveDownButton.Image is Bitmap moveDown)
            {
                _moveDownButton.Image = ScaleHelper.ScaleToDpi(moveDown, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
            }

            if (_moveUpButton.Image is Bitmap moveUp)
            {
                _moveUpButton.Image = ScaleHelper.ScaleToDpi(moveUp, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
            }

            if (_btnDelete.Image is Bitmap delete)
            {
                _btnDelete.Image = ScaleHelper.ScaleToDpi(delete, ScaleHelper.InitialSystemDpi, disposeBitmap: true);
            }
        }

        private TreeNode LastNode
        {
            get
            {
                // Big-O of this loop == #levels in the tree.
                TreeNode lastNode = _treeView1.Nodes[_treeView1.Nodes.Count - 1];
                while (lastNode.Nodes.Count > 0)
                {
                    lastNode = lastNode.Nodes[lastNode.Nodes.Count - 1];
                }

                return lastNode;
            }
        }

        private TreeView TreeView
        {
            get
            {
                if (Context is not null && Context.Instance is TreeView view)
                {
                    return view;
                }
                else
                {
                    Debug.Assert(false, "TreeNodeCollectionEditor couldn't find the TreeView being designed");
                    return null;
                }
            }
        }

        private int NextNode
        {
            get
            {
                if (TreeView is not null && TreeView.Site is not null)
                {
                    IDictionaryService dictionaryService = (IDictionaryService)TreeView.Site.GetService(typeof(IDictionaryService));
                    Debug.Assert(dictionaryService is not null, "TreeNodeCollectionEditor relies on IDictionaryService, which is not available.");
                    if (dictionaryService is not null)
                    {
                        object dictionaryValue = dictionaryService.GetValue(s_nextNodeKey);
                        if (dictionaryValue is not null)
                        {
                            _nextNode = (int)dictionaryValue;
                        }
                        else
                        {
                            _nextNode = 0;
                            dictionaryService.SetValue(s_nextNodeKey, 0);
                        }
                    }
                }

                return _nextNode;
            }
            set
            {
                _nextNode = value;
                if (TreeView is not null && TreeView.Site is not null)
                {
                    TreeView.Site.TryGetService(out IDictionaryService dictionaryService);
                    dictionaryService?.SetValue(s_nextNodeKey, _nextNode);
                }
            }
        }

        private void Add(TreeNode parent)
        {
            TreeNode newNode;
            string baseNodeName = SR.BaseNodeName;

            if (parent is null)
            {
                newNode = _treeView1.Nodes.Add(baseNodeName + NextNode++.ToString(CultureInfo.InvariantCulture));
                newNode.Name = newNode.Text;
            }
            else
            {
                newNode = parent.Nodes.Add(baseNodeName + NextNode++.ToString(CultureInfo.InvariantCulture));
                newNode.Name = newNode.Text;

                parent.Expand();
            }

            if (parent is not null)
            {
                _treeView1.SelectedNode = parent;
            }
            else
            {
                _treeView1.SelectedNode = newNode;

                // We are adding a Root Node at Level 1, so show the properties in the PropertyGrid.
                SetNodeProps(newNode);
            }
        }

        private void HookEvents()
        {
            _okButton.Click += BtnOK_click;
            _btnCancel.Click += BtnCancel_click;
            _btnAddChild.Click += BtnAddChild_click;
            _btnAddRoot.Click += BtnAddRoot_click;
            _btnDelete.Click += BtnDelete_click;
            _propertyGrid1.PropertyValueChanged += PropertyGrid_propertyValueChanged;
            _treeView1.AfterSelect += treeView1_afterSelect;
            _treeView1.DragEnter += treeView1_DragEnter;
            _treeView1.ItemDrag += treeView1_ItemDrag;
            _treeView1.DragDrop += treeView1_DragDrop;
            _treeView1.DragOver += treeView1_DragOver;
            HelpButtonClicked += TreeNodeCollectionEditor_HelpButtonClicked;
            _moveDownButton.Click += moveDownButton_Click;
            _moveUpButton.Click += moveUpButton_Click;
        }

        /// <summary>
        ///  The following code is required by the form designer.
        ///  It can be modified using the form editor.
        ///  Do not modify it using the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(TreeNodeCollectionEditor));

            _okCancelPanel = new TableLayoutPanel();
            _okButton = new Button();
            _btnCancel = new Button();
            _nodeControlPanel = new TableLayoutPanel();
            _btnAddRoot = new Button();
            _btnAddChild = new Button();
            _btnDelete = new Button();
            _moveDownButton = new Button();
            _moveUpButton = new Button();
            _propertyGrid1 = new VsPropertyGrid();
            _label2 = new Label();
            _treeView1 = new TreeView();
            _label1 = new Label();
            _overarchingTableLayoutPanel = new TableLayoutPanel();
            _navigationButtonsTableLayoutPanel = new TableLayoutPanel();
            _okCancelPanel.SuspendLayout();
            _nodeControlPanel.SuspendLayout();
            _overarchingTableLayoutPanel.SuspendLayout();
            _navigationButtonsTableLayoutPanel.SuspendLayout();
            SuspendLayout();

            // okCancelPanel
            resources.ApplyResources(_okCancelPanel, "okCancelPanel");
            _okCancelPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelPanel.Controls.Add(_okButton, 0, 0);
            _okCancelPanel.Controls.Add(_btnCancel, 1, 0);
            _okCancelPanel.Name = "okCancelPanel";
            _okCancelPanel.RowStyles.Add(new RowStyle());

            // okButton
            resources.ApplyResources(_okButton, "okButton");
            _okButton.DialogResult = DialogResult.OK;
            _okButton.Name = "okButton";

            // btnCancel
            resources.ApplyResources(_btnCancel, "btnCancel");
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Name = "btnCancel";

            // nodeControlPanel
            resources.ApplyResources(_nodeControlPanel, "nodeControlPanel");
            _nodeControlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _nodeControlPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _nodeControlPanel.Controls.Add(_btnAddRoot, 0, 0);
            _nodeControlPanel.Controls.Add(_btnAddChild, 1, 0);
            _nodeControlPanel.Name = "nodeControlPanel";
            _nodeControlPanel.RowStyles.Add(new RowStyle());

            // btnAddRoot
            resources.ApplyResources(_btnAddRoot, "btnAddRoot");
            _btnAddRoot.Name = "btnAddRoot";

            // btnAddChild
            resources.ApplyResources(_btnAddChild, "btnAddChild");
            _btnAddChild.Name = "btnAddChild";

            // btnDelete
            resources.ApplyResources(_btnDelete, "btnDelete");
            _btnDelete.Name = "btnDelete";

            // moveDownButton
            resources.ApplyResources(_moveDownButton, "moveDownButton");
            _moveDownButton.Name = "moveDownButton";

            // moveUpButton
            resources.ApplyResources(_moveUpButton, "moveUpButton");
            _moveUpButton.Name = "moveUpButton";
            // propertyGrid1
            resources.ApplyResources(_propertyGrid1, "propertyGrid1");

            // LineColor assigned here is causing issues in the HC mode. Going with runtime default for HC mode.
            if (!SystemInformation.HighContrast)
            {
                _propertyGrid1.LineColor = SystemColors.ScrollBar;
            }

            _propertyGrid1.Name = "propertyGrid1";
            _overarchingTableLayoutPanel.SetRowSpan(_propertyGrid1, 2);

            // label2
            resources.ApplyResources(_label2, "label2");
            _label2.Name = "label2";

            // treeView1
            _treeView1.AllowDrop = true;
            resources.ApplyResources(_treeView1, "treeView1");
            _treeView1.HideSelection = false;
            _treeView1.Name = "treeView1";

            // label1
            resources.ApplyResources(_label1, "label1");
            _label1.Name = "label1";

            // overarchingTableLayoutPanel
            resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

            _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());

            // Initializes with primary monitor always.
            ResetMarginsOnTheForm(true, 1);

            // navigationButtonsTableLayoutPanel
            resources.ApplyResources(_navigationButtonsTableLayoutPanel, "navigationButtonsTableLayoutPanel");
            _navigationButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            _navigationButtonsTableLayoutPanel.Controls.Add(_moveUpButton, 0, 0);
            _navigationButtonsTableLayoutPanel.Controls.Add(_btnDelete, 0, 2);
            _navigationButtonsTableLayoutPanel.Controls.Add(_moveDownButton, 0, 1);
            _navigationButtonsTableLayoutPanel.Name = "navigationButtonsTableLayoutPanel";
            _navigationButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
            _navigationButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
            _navigationButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());

            // TreeNodeCollectionEditor
            AcceptButton = _okButton;
            resources.ApplyResources(this, "$this");
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = _btnCancel;
            Controls.Add(_overarchingTableLayoutPanel);
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "TreeNodeCollectionEditor";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Show;
            _okCancelPanel.ResumeLayout(false);
            _okCancelPanel.PerformLayout();
            _nodeControlPanel.ResumeLayout(false);
            _nodeControlPanel.PerformLayout();
            _overarchingTableLayoutPanel.ResumeLayout(false);
            _overarchingTableLayoutPanel.PerformLayout();
            _navigationButtonsTableLayoutPanel.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void ResetMarginsOnTheForm(bool fromConstructor, double pixelFactor = 0)
        {
            if (pixelFactor == 0)
            {
                return;
            }

            int PIXEL_3 = 3;
            int PIXEL_18 = 18;

            _okCancelPanel.Margin = new Padding(PIXEL_3, PIXEL_3, 0, 0);
            _okButton.Margin = new Padding(0, 0, PIXEL_3, 0);
            _btnCancel.Margin = new Padding(PIXEL_3, 0, 0, 0);
            _nodeControlPanel.Margin = new Padding(0, PIXEL_3, PIXEL_3, PIXEL_3);
            _btnAddRoot.Margin = new Padding(0, 0, PIXEL_3, 0);
            _btnAddChild.Margin = new Padding(PIXEL_3, 0, 0, 0);
            _btnDelete.Margin = new Padding(0, PIXEL_3, 0, 0);
            _moveDownButton.Margin = new Padding(0, PIXEL_3, 0, PIXEL_3);
            _moveUpButton.Margin = new Padding(0, 0, 0, PIXEL_3);
            _propertyGrid1.Margin = new Padding(PIXEL_3, PIXEL_3, 0, PIXEL_3);
            _label2.Margin = new Padding(PIXEL_3, PIXEL_3, 0, PIXEL_3);
            _treeView1.Margin = new Padding(0, PIXEL_3, PIXEL_3, PIXEL_3);
            _label1.Margin = new Padding(0, PIXEL_3, PIXEL_3, PIXEL_3);
            _navigationButtonsTableLayoutPanel.Margin = new Padding(PIXEL_3, PIXEL_3, PIXEL_18, PIXEL_3);
            if (fromConstructor)
            {
                _overarchingTableLayoutPanel.Controls.Clear();
                _overarchingTableLayoutPanel.Controls.Add(_label1, 0, 0);
                _overarchingTableLayoutPanel.Controls.Add(_treeView1, 0, 1);
                _overarchingTableLayoutPanel.Controls.Add(_nodeControlPanel, 0, 2);
                _overarchingTableLayoutPanel.Controls.Add(_navigationButtonsTableLayoutPanel, 1, 1);
                _overarchingTableLayoutPanel.Controls.Add(_label2, 2, 0);
                _overarchingTableLayoutPanel.Controls.Add(_propertyGrid1, 2, 1);
                _overarchingTableLayoutPanel.Controls.Add(_okCancelPanel, 2, 3);
            }
            else
            {
                // TreeView nodes doesn't get font propagation in DPIChanged scenario because it inherits font from parent
                // and DpiChanged event handling of the parent happens later and doesn't raise onFontChanges() event when
                // the control or its parent tree is derived from Form . We special handled Form class for DpiChanged event
                // to avoid double scaling issues. One solution here is, to raise onParentChange() event to get the right font.

                // This is a special handling and may need to revisit
                _overarchingTableLayoutPanel.Controls.Remove(_treeView1);
                _overarchingTableLayoutPanel.Controls.Add(_treeView1, 0, 1);
            }
        }

        /// <summary>
        ///  This is called when the value property in the CollectionForm has changed.
        ///  In it you should update your user interface to reflect the current value.
        /// </summary>
        protected override void OnEditValueChanged()
        {
            if (EditValue is not null)
            {
                object[] items = Items;

                _propertyGrid1.Site = new PropertyGridSite(Context, _propertyGrid1);

                TreeNode[] nodes = new TreeNode[items.Length];

                for (int i = 0; i < items.Length; i++)
                {
                    // We need to copy the nodes into our editor TreeView, not move them.
                    // We overwrite the passed-in array with the new roots.
                    nodes[i] = (TreeNode)((TreeNode)items[i]).Clone();
                }

                _treeView1.Nodes.Clear();
                _treeView1.Nodes.AddRange(nodes);

                // Update current node related UI
                _curNode = null;
                _btnAddChild.Enabled = false;
                _btnDelete.Enabled = false;

                // The image list for the editor TreeView must be updated to be the same
                // as the image list for the actual TreeView.
                TreeView actualTV = TreeView;
                if (actualTV is not null)
                {
                    SetImageProps(actualTV);
                }

                if (items.Length > 0 && nodes[0] is not null)
                {
                    _treeView1.SelectedNode = nodes[0];
                }
            }
        }

        /// <summary>
        ///  When something in the properties window changes, we update pertinent text here.
        /// </summary>
        private void PropertyGrid_propertyValueChanged(object sender, PropertyValueChangedEventArgs e)
        {
            // Update the string above the grid.
            _label2.Text = string.Format(SR.CollectionEditorProperties, _treeView1.SelectedNode.Text);
        }

        private void SetImageProps(TreeView actualTreeView)
        {
            if (actualTreeView.ImageList is not null)
            {
                // Update the treeView image-related properties.
                _treeView1.ImageList = actualTreeView.ImageList;
                _treeView1.ImageIndex = actualTreeView.ImageIndex;
                _treeView1.SelectedImageIndex = actualTreeView.SelectedImageIndex;
            }
            else
            {
                // Update the treeView image-related properties.
                _treeView1.ImageList = null;
                _treeView1.ImageIndex = -1;
                _treeView1.SelectedImageIndex = -1;
            }

            if (actualTreeView.StateImageList is not null)
            {
                _treeView1.StateImageList = actualTreeView.StateImageList;
            }
            else
            {
                _treeView1.StateImageList = null;
            }

            // Also set the CheckBoxes from the actual TreeView.
            _treeView1.CheckBoxes = actualTreeView.CheckBoxes;
        }

        private void SetNodeProps(TreeNode node)
        {
            if (node is not null)
            {
                _label2.Text = string.Format(SR.CollectionEditorProperties, node.Name.ToString());
            }

            // No node is selected. Revert back the Text of the label to Properties.
            else
            {
                _label2.Text = string.Format(SR.CollectionEditorPropertiesNone);
            }

            _propertyGrid1.SelectedObject = node;
        }

        private void treeView1_afterSelect(object sender, TreeViewEventArgs e)
        {
            _curNode = e.Node;
            SetNodeProps(_curNode);
            SetButtonsState();
        }

        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode item = (TreeNode)e.Item;
            DoDragDrop(item, DragDropEffects.Move);
        }

        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            TreeNode dragNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
            Point position = new Point(0, 0)
            {
                X = e.X,
                Y = e.Y
            };

            position = _treeView1.PointToClient(position);
            TreeNode dropNode = _treeView1.GetNodeAt(position);

            if (dragNode != dropNode)
            {
                // Remove this node after finding the new root
                // but before re-adding the node to the collection.
                _treeView1.Nodes.Remove(dragNode);

                if (dropNode is not null && !CheckParent(dropNode, dragNode)) // DROPPED ON LEVEL > 0
                {
                    dropNode.Nodes.Add(dragNode);
                }
                else // DROPPED ON LEVEL 0
                {
                    _treeView1.Nodes.Add(dragNode);
                }
            }
        }

        private static bool CheckParent(TreeNode child, TreeNode parent)
        {
            while (child is not null)
            {
                if (parent == child.Parent)
                {
                    return true;
                }

                child = child.Parent;
            }

            return false;
        }

        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            Point position = new Point(0, 0)
            {
                X = e.X,
                Y = e.Y
            };

            position = _treeView1.PointToClient(position);
            TreeNode currentNode = _treeView1.GetNodeAt(position);
            _treeView1.SelectedNode = currentNode;
        }

        private void BtnAddChild_click(object sender, EventArgs e)
        {
            Add(_curNode);
            SetButtonsState();
        }

        private void BtnAddRoot_click(object sender, EventArgs e)
        {
            Add(null);
            SetButtonsState();
        }

        private void BtnDelete_click(object sender, EventArgs e)
        {
            _curNode.Remove();
            if (_treeView1.Nodes.Count == 0)
            {
                _curNode = null;
                SetNodeProps(null);
            }

            SetButtonsState();
        }

        private void BtnOK_click(object sender, EventArgs e)
        {
            object[] values = new object[_treeView1.Nodes.Count];
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = _treeView1.Nodes[i].Clone();
            }

            Items = values;

            // Now treeView is not required. Dispose it.
            _treeView1.Dispose();
            _treeView1 = null;
        }

        private void moveDownButton_Click(object sender, EventArgs e)
        {
            TreeNode tempNode = _curNode;
            TreeNode parent = _curNode.Parent;

            if (parent is null)
            {
                _treeView1.Nodes.RemoveAt(tempNode.Index);
                _treeView1.Nodes[tempNode.Index].Nodes.Insert(0, tempNode);
            }
            else
            {
                parent.Nodes.RemoveAt(tempNode.Index);
                if (tempNode.Index < parent.Nodes.Count)
                {
                    parent.Nodes[tempNode.Index].Nodes.Insert(0, tempNode);
                }
                else
                {
                    if (parent.Parent is null)
                    {
                        _treeView1.Nodes.Insert(parent.Index + 1, tempNode);
                    }
                    else
                    {
                        parent.Parent.Nodes.Insert(parent.Index + 1, tempNode);
                    }
                }
            }

            _treeView1.SelectedNode = tempNode;
            _curNode = tempNode;
        }

        private void moveUpButton_Click(object sender, EventArgs e)
        {
            TreeNode tempNode = _curNode;
            TreeNode parent = _curNode.Parent;

            if (parent is null)
            {
                _treeView1.Nodes.RemoveAt(tempNode.Index);
                _treeView1.Nodes[tempNode.Index - 1].Nodes.Add(tempNode);
            }
            else
            {
                parent.Nodes.RemoveAt(tempNode.Index);
                if (tempNode.Index == 0)
                {
                    if (parent.Parent is null)
                    {
                        _treeView1.Nodes.Insert(parent.Index, tempNode);
                    }
                    else
                    {
                        parent.Parent.Nodes.Insert(parent.Index, tempNode);
                    }
                }
                else
                {
                    parent.Nodes[tempNode.Index - 1].Nodes.Add(tempNode);
                }
            }

            _treeView1.SelectedNode = tempNode;
            _curNode = tempNode;
        }

        private void SetButtonsState()
        {
            bool nodesExist = _treeView1.Nodes.Count > 0;

            _btnAddChild.Enabled = nodesExist;
            _btnDelete.Enabled = nodesExist;
            _moveDownButton.Enabled =
                nodesExist
                && (_curNode != LastNode || _curNode.Level > 0)
                && _curNode != _treeView1.Nodes[_treeView1.Nodes.Count - 1];
            _moveUpButton.Enabled = nodesExist && _curNode != _treeView1.Nodes[0];
        }

        private void TreeNodeCollectionEditor_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            _editor.ShowHelp();
        }

        private void BtnCancel_click(object sender, EventArgs e)
        {
            if (NextNode != _initialNextNode)
            {
                NextNode = _initialNextNode;
            }
        }
    }
}
