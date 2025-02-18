// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Drawing;
using System.Collections;

namespace System.Windows.Forms.Design;

internal class BindingFormattingDialog : System.Windows.Forms.Form
{
    // we need the context for the HELP service provider
    private ITypeDescriptorContext context = null;

    private ControlBindingsCollection bindings;

    private BindingFormattingWindowsFormsEditorService dataSourcePicker;
    private System.Windows.Forms.Label explanationLabel;
    private System.Windows.Forms.TableLayoutPanel mainTableLayoutPanel;
    private System.Windows.Forms.Label propertyLabel;
    private System.Windows.Forms.TreeView propertiesTreeView;
    private System.Windows.Forms.Label bindingLabel;
    private System.Windows.Forms.ComboBox bindingUpdateDropDown;
    private System.Windows.Forms.Label updateModeLabel;
    private FormatControl formatControl1;
    private System.Windows.Forms.TableLayoutPanel okCancelTableLayoutPanel;
    private System.Windows.Forms.Button okButton;
    private System.Windows.Forms.Button cancelButton;
    private bool inLoad = false;

    private bool dirty = false;

    private const int BOUNDIMAGEINDEX = 0;
    private const int UNBOUNDIMAGEINDEX = 1;

    // static because there will be only one instance of this dialog shown at any time
    private static Bitmap boundBitmap;
    private static Bitmap unboundBitmap;

    // We have to cache the current tree node because the WinForms TreeView control
    // doesn't tell use what the previous node is when we receive the BeforeSelect event
    private BindingTreeNode currentBindingTreeNode = null;
    private IDesignerHost host = null;

    public BindingFormattingDialog()
    {
        InitializeComponent();
    }

    public ControlBindingsCollection Bindings
    {
        set
        {
            this.bindings = value;
        }
    }

    private static Bitmap ScaleBitmapIfNeeded(string resourceName)
    {
        Bitmap bitmap = new Bitmap(typeof(BindingFormattingDialog), resourceName);
        bitmap.MakeTransparent(System.Drawing.Color.Red);
        if (DpiHelper.IsScalingRequired)
        {
            // ImageList imposes size limit of 256x256 to conserve memory
            int height = bitmap.Size.Height;
            height = Math.Min(256, DpiHelper.LogicalToDeviceUnitsY(height));
            int width = bitmap.Size.Width;
            width = Math.Min(256, DpiHelper.LogicalToDeviceUnitsX(width));
            Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(bitmap, new Drawing.Size(width, height));
            if (scaledBitmap is not null)
            {
                bitmap.Dispose();
                bitmap = scaledBitmap;
            }
        }
        return bitmap;
    }

    private static Bitmap BoundBitmap
    {
        get
        {
            if (boundBitmap is null)
            {
                boundBitmap = ScaleBitmapIfNeeded("BindingFormattingDialog.Bound.bmp");
            }
            return boundBitmap;
        }
    }

    public ITypeDescriptorContext Context
    {
        get
        {
            return this.context;
        }

        set
        {
            this.context = value;
            dataSourcePicker.Context = value;
        }
    }

    public bool Dirty
    {
        get
        {
            return this.dirty || this.formatControl1.Dirty;
        }
    }

    public IDesignerHost Host
    {
        set
        {
            this.host = value;
        }
    }

    private static Bitmap UnboundBitmap
    {
        get
        {
            if (unboundBitmap is null)
            {
                unboundBitmap = ScaleBitmapIfNeeded("BindingFormattingDialog.Unbound.bmp");
            }
            return unboundBitmap;
        }
    }

    private void BindingFormattingDialog_Closing(object sender, CancelEventArgs e)
    {
        this.currentBindingTreeNode = null;
        this.dataSourcePicker.OwnerComponent = null;

        this.formatControl1.ResetFormattingInfo();
    }

    private void BindingFormattingDialog_HelpRequested(object sender, HelpEventArgs e)
    {
        BindingFormattingDialog_HelpRequestHandled();
        e.Handled = true;
    }

    private void BindingFormattingDialog_HelpButtonClicked(object sender, CancelEventArgs e)
    {
        BindingFormattingDialog_HelpRequestHandled();
        e.Cancel = true;
    }

    private void BindingFormattingDialog_HelpRequestHandled()
    {
        IHelpService helpService = this.context.GetService(typeof(IHelpService)) as IHelpService;
        if (helpService is not null)
        {
            helpService.ShowHelpFromKeyword("vs.BindingFormattingDialog");
        }
    }

    private void BindingFormattingDialog_Load(object sender, EventArgs e)
    {
        this.inLoad = true;

        try
        {
            //
            // start a new transaction
            //
            this.dirty = false;

            //
            // get the dialog font
            //
            System.Drawing.Font uiFont = Control.DefaultFont;
            IUIService uiService = null;
            if (this.bindings.BindableComponent.Site is not null)
            {
                uiService = (IUIService)this.bindings.BindableComponent.Site.GetService(typeof(IUIService));
            }

            if (uiService is not null)
            {
                uiFont = (System.Drawing.Font)uiService.Styles["DialogFont"];
            }

            this.Font = uiFont;

            //
            // enable explorer tree style
            //
            DesignerUtils.ApplyTreeViewThemeStyles(this.propertiesTreeView);

            //
            // push the image list in the tree view
            //
            if (this.propertiesTreeView.ImageList is null)
            {
                ImageList il = new ImageList();
                il.Images.Add(BoundBitmap);
                il.Images.Add(UnboundBitmap);
                if (DpiHelper.IsScalingRequired)
                {
                    il.ImageSize = BoundBitmap.Size;
                }
                this.propertiesTreeView.ImageList = il;
            }

            //
            // get the defaultBindingProperty and / or defaultProperty
            //
            BindingTreeNode defaultBindingPropertyNode = null;
            BindingTreeNode defaultPropertyNode = null;
            string defaultBindingPropertyName = null;
            string defaultPropertyName = null;
            AttributeCollection compAttrs = TypeDescriptor.GetAttributes(bindings.BindableComponent);
            foreach (Attribute attr in compAttrs)
            {
                if (attr is DefaultBindingPropertyAttribute)
                {
                    defaultBindingPropertyName = ((DefaultBindingPropertyAttribute)attr).Name;
                    break;
                }
                else if (attr is DefaultPropertyAttribute)
                {
                    defaultPropertyName = ((DefaultPropertyAttribute)attr).Name;
                }
            }

            //
            // populate the control bindings tree view
            //
            this.propertiesTreeView.Nodes.Clear();
            TreeNode commonNode = new TreeNode(SR.GetString(SR.BindingFormattingDialogCommonTreeNode));
            TreeNode allNode = new TreeNode(SR.GetString(SR.BindingFormattingDialogAllTreeNode));

            this.propertiesTreeView.Nodes.Add(commonNode);
            this.propertiesTreeView.Nodes.Add(allNode);

            IBindableComponent bindableComp = bindings.BindableComponent;

            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(bindableComp);
            for (int i = 0; i < props.Count; i++)
            {
                if (props[i].IsReadOnly)
                {
                    continue;
                }

                BindableAttribute bindableAttr = (BindableAttribute)props[i].Attributes[typeof(BindableAttribute)];
                BrowsableAttribute browsable = (BrowsableAttribute)props[i].Attributes[typeof(BrowsableAttribute)];

                // Filter the non Browsable properties but only if they are non Bindable, too.
                // vsWhidbey 371995
                if (browsable is not null && !browsable.Browsable && (bindableAttr is null || !bindableAttr.Bindable))
                {
                    continue;
                }

                BindingTreeNode treeNode = new BindingTreeNode(props[i].Name);

                treeNode.Binding = this.FindBinding(props[i].Name);

                // Make a reasonable guess as to what the FormatType is
                if (treeNode.Binding is not null)
                {
                    treeNode.FormatType = FormatControl.FormatTypeStringFromFormatString(treeNode.Binding.FormatString);
                }
                else
                {
                    treeNode.FormatType = SR.GetString(SR.BindingFormattingDialogFormatTypeNoFormatting);
                }

                if (bindableAttr is not null && bindableAttr.Bindable)
                {
                    commonNode.Nodes.Add(treeNode);
                }
                else
                {
                    allNode.Nodes.Add(treeNode);
                }

                if (defaultBindingPropertyNode is null &&
                    !String.IsNullOrEmpty(defaultBindingPropertyName) &&
                    String.Compare(props[i].Name, defaultBindingPropertyName, false /*caseInsensitive*/, CultureInfo.CurrentCulture) == 0)
                {
                    defaultBindingPropertyNode = treeNode;
                }
                else if (defaultPropertyNode is null &&
                         !String.IsNullOrEmpty(defaultPropertyName) &&
                         String.Compare(props[i].Name, defaultPropertyName, false /*caseInsensitive*/, CultureInfo.CurrentCulture) == 0)
                {
                    defaultPropertyNode = treeNode;
                }
            }

            commonNode.Expand();
            allNode.Expand();

            this.propertiesTreeView.Sort();

            // set the default node
            // 1. if we have a DefaultBindingProperty then select it; else
            // 2. if we have a DefaultProperty then select it
            // 3. select the first node in "All" nodes
            // 4. select the first node in "Common" nodes
            BindingTreeNode selectedNode;
            if (defaultBindingPropertyNode is not null)
            {
                selectedNode = defaultBindingPropertyNode;
            }
            else if (defaultPropertyNode is not null)
            {
                selectedNode = defaultPropertyNode;
            }
            else if (commonNode.Nodes.Count > 0)
            {
                selectedNode = FirstNodeInAlphabeticalOrder(commonNode.Nodes) as BindingTreeNode;
            }
            else if (allNode.Nodes.Count > 0)
            {
                selectedNode = FirstNodeInAlphabeticalOrder(allNode.Nodes) as BindingTreeNode;
            }
            else
            {
                // DANIELHE: so there are no properties for this component.  should we throw an exception?
                //
                selectedNode = null;
            }

            this.propertiesTreeView.SelectedNode = selectedNode;
            if (selectedNode is not null)
            {
                selectedNode.EnsureVisible();
            }

            this.dataSourcePicker.PropertyName = selectedNode.Text;
            this.dataSourcePicker.Binding = selectedNode is not null ? selectedNode.Binding : null;
            this.dataSourcePicker.Enabled = true;
            this.dataSourcePicker.OwnerComponent = this.bindings.BindableComponent;
            this.dataSourcePicker.DefaultDataSourceUpdateMode = bindings.DefaultDataSourceUpdateMode;

            if (selectedNode is not null && selectedNode.Binding is not null)
            {
                bindingUpdateDropDown.Enabled = true;
                this.bindingUpdateDropDown.SelectedItem = selectedNode.Binding.DataSourceUpdateMode;
                this.updateModeLabel.Enabled = true;
                this.formatControl1.Enabled = true;

                // setup the format control
                this.formatControl1.FormatType = selectedNode.FormatType;
                FormatControl.FormatTypeClass formatTypeItem = this.formatControl1.FormatTypeItem;
                Debug.Assert(formatTypeItem is not null, "The FormatString and FormatProvider was not persisted corectly for this binding");

                formatTypeItem.PushFormatStringIntoFormatType(selectedNode.Binding.FormatString);
                if (selectedNode.Binding.NullValue is not null)
                {
                    this.formatControl1.NullValue = selectedNode.Binding.NullValue.ToString();
                }
                else
                {
                    this.formatControl1.NullValue = String.Empty;
                }
            }
            else
            {
                this.bindingUpdateDropDown.Enabled = false;
                this.bindingUpdateDropDown.SelectedItem = bindings.DefaultDataSourceUpdateMode;
                this.updateModeLabel.Enabled = false;
                this.formatControl1.Enabled = false;
                this.formatControl1.FormatType = String.Empty;
            }

            // tell the format control that we start a new transaction
            // we have to do this after we set the formatControl
            this.formatControl1.Dirty = false;

            // set the currentBindingTreeNode
            this.currentBindingTreeNode = this.propertiesTreeView.SelectedNode as BindingTreeNode;

        }
        finally
        {
            this.inLoad = false;
        }

        //
        // Done
        //
    }

    // given the property name, this function will return the binding, if there is any
    private Binding FindBinding(string propertyName)
    {
        for (int i = 0; i < this.bindings.Count; i++)
        {
            if (String.Equals(propertyName, bindings[i].PropertyName, StringComparison.OrdinalIgnoreCase))
            {
                return bindings[i];
            }
        }

        return null;
    }

    private static TreeNode FirstNodeInAlphabeticalOrder(TreeNodeCollection nodes)
    {
        if (nodes.Count == 0)
        {
            return null;
        }

        TreeNode result = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            if (String.Compare(result.Text, nodes[i].Text, false /*ignoreCase*/, CultureInfo.CurrentCulture) > 0)
            {
                result = nodes[i];
            }
        }

        return result;
    }

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BindingFormattingDialog));
        this.explanationLabel = new System.Windows.Forms.Label();
        this.mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.propertiesTreeView = new System.Windows.Forms.TreeView();
        this.propertyLabel = new System.Windows.Forms.Label();
        this.dataSourcePicker = new BindingFormattingWindowsFormsEditorService();
        this.bindingLabel = new System.Windows.Forms.Label();
        this.updateModeLabel = new System.Windows.Forms.Label();
        this.bindingUpdateDropDown = new System.Windows.Forms.ComboBox();
        this.formatControl1 = new FormatControl();
        this.okCancelTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
        this.okButton = new System.Windows.Forms.Button();
        this.cancelButton = new System.Windows.Forms.Button();
        this.mainTableLayoutPanel.SuspendLayout();
        this.okCancelTableLayoutPanel.SuspendLayout();
        this.ShowIcon = false;
        this.SuspendLayout();
        // 
        // explanationLabel
        // 
        resources.ApplyResources(this.explanationLabel, "explanationLabel");
        this.mainTableLayoutPanel.SetColumnSpan(this.explanationLabel, 3);
        this.explanationLabel.Name = "explanationLabel";
        // 
        // mainTableLayoutPanel
        // 
        resources.ApplyResources(this.mainTableLayoutPanel, "mainTableLayoutPanel");
        this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
        this.mainTableLayoutPanel.Controls.Add(this.okCancelTableLayoutPanel, 2, 4);
        this.mainTableLayoutPanel.Controls.Add(this.formatControl1, 1, 3);
        this.mainTableLayoutPanel.Controls.Add(this.bindingUpdateDropDown, 2, 2);
        this.mainTableLayoutPanel.Controls.Add(this.propertiesTreeView, 0, 2);
        this.mainTableLayoutPanel.Controls.Add(this.updateModeLabel, 2, 1);
        this.mainTableLayoutPanel.Controls.Add(this.dataSourcePicker, 1, 2);
        this.mainTableLayoutPanel.Controls.Add(this.explanationLabel, 0, 0);
        this.mainTableLayoutPanel.Controls.Add(this.bindingLabel, 1, 1);
        this.mainTableLayoutPanel.Controls.Add(this.propertyLabel, 0, 1);
        this.mainTableLayoutPanel.MinimumSize = new System.Drawing.Size(542, 283);
        this.mainTableLayoutPanel.Name = "mainTableLayoutPanel";
        this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
        this.mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
        // 
        // propertiesTreeView
        // 
        resources.ApplyResources(this.propertiesTreeView, "propertiesTreeView");
        this.propertiesTreeView.Name = "propertiesTreeView";
        this.propertiesTreeView.HideSelection = false;
        this.propertiesTreeView.TreeViewNodeSorter = new TreeNodeComparer();
        this.mainTableLayoutPanel.SetRowSpan(this.propertiesTreeView, 2);
        this.propertiesTreeView.BeforeSelect += new TreeViewCancelEventHandler(this.propertiesTreeView_BeforeSelect);
        this.propertiesTreeView.AfterSelect += new TreeViewEventHandler(this.propertiesTreeView_AfterSelect);
        // 
        // propertyLabel
        // 
        resources.ApplyResources(this.propertyLabel, "propertyLabel");
        this.propertyLabel.Name = "propertyLabel";
        // 
        // dataSourcePicker
        // 
        resources.ApplyResources(this.dataSourcePicker, "dataSourcePicker");
        this.dataSourcePicker.Name = "dataSourcePicker";
        this.dataSourcePicker.PropertyValueChanged += new System.EventHandler(dataSourcePicker_PropertyValueChanged);
        // 
        // bindingLabel
        // 
        resources.ApplyResources(this.bindingLabel, "bindingLabel");
        this.bindingLabel.Name = "bindingLabel";
        // 
        // updateModeLabel
        // 
        resources.ApplyResources(this.updateModeLabel, "updateModeLabel");
        this.updateModeLabel.Name = "updateModeLabel";
        // 
        // bindingUpdateDropDown
        // 
        this.bindingUpdateDropDown.FormattingEnabled = true;
        resources.ApplyResources(this.bindingUpdateDropDown, "bindingUpdateDropDown");
        this.bindingUpdateDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        this.bindingUpdateDropDown.Name = "bindingUpdateDropDown";
        this.bindingUpdateDropDown.Items.AddRange(new object[] { DataSourceUpdateMode.Never, DataSourceUpdateMode.OnPropertyChanged, DataSourceUpdateMode.OnValidation });
        this.bindingUpdateDropDown.SelectedIndexChanged += new System.EventHandler(this.bindingUpdateDropDown_SelectedIndexChanged);
        // 
        // formatControl1
        // 
        this.mainTableLayoutPanel.SetColumnSpan(this.formatControl1, 2);
        resources.ApplyResources(this.formatControl1, "formatControl1");
        this.formatControl1.MinimumSize = new System.Drawing.Size(390, 237);
        this.formatControl1.Name = "formatControl1";
        this.formatControl1.NullValueTextBoxEnabled = true;
        // 
        // okCancelTableLayoutPanel
        // 
        resources.ApplyResources(this.okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
        this.okCancelTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
        this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
        this.okCancelTableLayoutPanel.Controls.Add(this.cancelButton, 1, 0);
        this.okCancelTableLayoutPanel.Controls.Add(this.okButton, 0, 0);
        this.okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        this.okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
        // 
        // okButton
        // 
        resources.ApplyResources(this.okButton, "okButton");
        this.okButton.Name = "okButton";
        this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.okButton.Click += new EventHandler(this.okButton_Click);
        // 
        // cancelButton
        // 
        resources.ApplyResources(this.cancelButton, "cancelButton");
        this.cancelButton.Name = "cancelButton";
        this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.cancelButton.Click += new EventHandler(this.cancelButton_Click);
        // 
        // BindingFormattingDialog
        // 
        resources.ApplyResources(this, "$this");
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.CancelButton = cancelButton;
        this.AcceptButton = okButton;
        this.Controls.Add(this.mainTableLayoutPanel);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        this.Name = "BindingFormattingDialog";
        this.mainTableLayoutPanel.ResumeLayout(false);
        this.mainTableLayoutPanel.PerformLayout();
        this.okCancelTableLayoutPanel.ResumeLayout(false);
        this.HelpButton = true;
        this.ShowInTaskbar = false;
        this.MinimizeBox = false;
        this.MaximizeBox = false;
        this.Load += new EventHandler(BindingFormattingDialog_Load);
        this.Closing += new CancelEventHandler(BindingFormattingDialog_Closing);
        this.HelpButtonClicked += new CancelEventHandler(this.BindingFormattingDialog_HelpButtonClicked);
        this.HelpRequested += new HelpEventHandler(this.BindingFormattingDialog_HelpRequested);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private void cancelButton_Click(object sender, EventArgs e)
    {
        this.dirty = false;
    }

    // this will consolidate the information from the form in the currentBindingTreeNode member variable

    [
    SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")      // We can't avoid casting from a ComboBox.
    ]
    private void ConsolidateBindingInformation()
    {
        Debug.Assert(this.currentBindingTreeNode is not null, "we need a binding tree node to consolidate this information");

        Binding binding = this.dataSourcePicker.Binding;

        if (binding is null)
        {
            return;
        }

        // Whidbey Data Binding will have FormattingEnabled set to true
        binding.FormattingEnabled = true;
        this.currentBindingTreeNode.Binding = binding;
        this.currentBindingTreeNode.FormatType = this.formatControl1.FormatType;

        FormatControl.FormatTypeClass formatTypeItem = this.formatControl1.FormatTypeItem;

        if (formatTypeItem is not null)
        {
            binding.FormatString = formatTypeItem.FormatString;
            binding.NullValue = this.formatControl1.NullValue;
        }

        binding.DataSourceUpdateMode = (DataSourceUpdateMode)this.bindingUpdateDropDown.SelectedItem;

    }

    private void dataSourcePicker_PropertyValueChanged(object sender, System.EventArgs e)
    {
        if (this.inLoad)
        {
            return;
        }

        BindingTreeNode bindingTreeNode = this.propertiesTreeView.SelectedNode as BindingTreeNode;
        Debug.Assert(bindingTreeNode is not null, " the data source drop down is active only when the user is editing a binding tree node");

        if (this.dataSourcePicker.Binding == bindingTreeNode.Binding)
        {
            return;
        }

        Binding binding = this.dataSourcePicker.Binding;

        if (binding is not null)
        {
            binding.FormattingEnabled = true;

            Binding currentBinding = bindingTreeNode.Binding;
            if (currentBinding is not null)
            {
                binding.FormatString = currentBinding.FormatString;
                binding.NullValue = currentBinding.NullValue;
                binding.FormatInfo = currentBinding.FormatInfo;
            }
        }

        bindingTreeNode.Binding = binding;

        // enable/disable the format control
        if (binding is not null)
        {
            this.formatControl1.Enabled = true;
            this.updateModeLabel.Enabled = true;
            this.bindingUpdateDropDown.Enabled = true;
            this.bindingUpdateDropDown.SelectedItem = binding.DataSourceUpdateMode;

            if (!String.IsNullOrEmpty(this.formatControl1.FormatType))
            {
                // push the current user control into the format control type
                this.formatControl1.FormatType = this.formatControl1.FormatType;
            }
            else
            {
                this.formatControl1.FormatType = SR.GetString(SR.BindingFormattingDialogFormatTypeNoFormatting);
            }
        }
        else
        {
            this.formatControl1.Enabled = false;
            this.updateModeLabel.Enabled = false;
            this.bindingUpdateDropDown.Enabled = false;
            this.bindingUpdateDropDown.SelectedItem = bindings.DefaultDataSourceUpdateMode;

            this.formatControl1.FormatType = SR.GetString(SR.BindingFormattingDialogFormatTypeNoFormatting);
        }

        // dirty the form
        this.dirty = true;
    }

    private void okButton_Click(object sender, EventArgs e)
    {
        // save the information for the current binding
        if (this.currentBindingTreeNode is not null)
        {
            this.ConsolidateBindingInformation();
        }

        // push the changes
        this.PushChanges();
    }

    private void propertiesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
    {
        if (this.inLoad)
        {
            return;
        }

        BindingTreeNode bindingTreeNode = e.Node as BindingTreeNode;

        if (bindingTreeNode is null)
        {
            // disable the data source drop down when the active tree node is not a binding node
            this.dataSourcePicker.Binding = null;
            this.bindingLabel.Enabled = this.dataSourcePicker.Enabled = false;
            this.updateModeLabel.Enabled = this.bindingUpdateDropDown.Enabled = false;
            this.formatControl1.Enabled = false;
            return;
        }

        // make sure the the drop down is enabled
        this.bindingLabel.Enabled = this.dataSourcePicker.Enabled = true;
        this.dataSourcePicker.PropertyName = bindingTreeNode.Text;

        // enable the update mode drop down only if the user is editing a binding;
        this.updateModeLabel.Enabled = this.bindingUpdateDropDown.Enabled = false;

        // enable the format control only if the user is editing a binding
        this.formatControl1.Enabled = false;

        if (bindingTreeNode.Binding is not null)
        {
            // this is not the first time we visit this binding
            // restore the binding information from the last time the user touched this binding
            this.formatControl1.Enabled = true;
            this.formatControl1.FormatType = bindingTreeNode.FormatType;
            Debug.Assert(this.formatControl1.FormatTypeItem is not null, "FormatType did not persist well for this binding");

            FormatControl.FormatTypeClass formatTypeItem = this.formatControl1.FormatTypeItem;

            this.dataSourcePicker.Binding = bindingTreeNode.Binding;

            formatTypeItem.PushFormatStringIntoFormatType(bindingTreeNode.Binding.FormatString);
            if (bindingTreeNode.Binding.NullValue is not null)
            {
                this.formatControl1.NullValue = bindingTreeNode.Binding.NullValue.ToString();
            }
            else
            {
                this.formatControl1.NullValue = String.Empty;
            }

            this.bindingUpdateDropDown.SelectedItem = bindingTreeNode.Binding.DataSourceUpdateMode;
            Debug.Assert(this.bindingUpdateDropDown.SelectedItem is not null, "Binding.UpdateMode was not persisted corectly for this binding");
            this.updateModeLabel.Enabled = this.bindingUpdateDropDown.Enabled = true;
        }
        else
        {
            bool currentDirtyState = this.dirty;
            this.dataSourcePicker.Binding = null;

            this.formatControl1.FormatType = bindingTreeNode.FormatType;
            this.bindingUpdateDropDown.SelectedItem = bindings.DefaultDataSourceUpdateMode;

            this.formatControl1.NullValue = null;
            this.dirty = currentDirtyState;
        }

        this.formatControl1.Dirty = false;

        // now save this node so that when we get the BeforeSelect event we know which node was affected
        this.currentBindingTreeNode = bindingTreeNode;
    }

    private void propertiesTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
    {
        if (this.inLoad)
        {
            return;
        }

        if (this.currentBindingTreeNode is null)
        {
            return;
        }

        // if there is no selected field quit
        if (this.dataSourcePicker.Binding is null)
        {
            return;
        }

        // if the format control was not touched quit
        if (!this.formatControl1.Enabled)
        {
            return;
        }

        ConsolidateBindingInformation();

        // dirty the form
        this.dirty = this.dirty || this.formatControl1.Dirty;
    }

    private void PushChanges()
    {
        if (!this.Dirty)
        {
            return;
        }

        IComponentChangeService ccs = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
        PropertyDescriptor prop = null;
        IBindableComponent control = bindings.BindableComponent;
        if (ccs is not null && control is not null)
        {
            prop = TypeDescriptor.GetProperties(control)["DataBindings"];
            if (prop is not null)
            {
                ccs.OnComponentChanging(control, prop);
            }
        }

        // clear the bindings collection and insert the new bindings
        this.bindings.Clear();

        // get the bindings from the "Common" tree nodes
        TreeNode commonTreeNode = this.propertiesTreeView.Nodes[0];
        Debug.Assert(commonTreeNode.Text.Equals(SR.GetString(SR.BindingFormattingDialogCommonTreeNode)), "the first node in the tree view should be the COMMON node");
        for (int i = 0; i < commonTreeNode.Nodes.Count; i++)
        {
            BindingTreeNode bindingTreeNode = commonTreeNode.Nodes[i] as BindingTreeNode;
            Debug.Assert(bindingTreeNode is not null, "we only put bindingTreeNodes in the COMMON node");
            if (bindingTreeNode.Binding is not null)
            {
                this.bindings.Add(bindingTreeNode.Binding);
            }
        }

        // get the bindings from the "All" tree nodes
        TreeNode allTreeNode = this.propertiesTreeView.Nodes[1];
        Debug.Assert(allTreeNode.Text.Equals(SR.GetString(SR.BindingFormattingDialogAllTreeNode)), "the second node in the tree view should be the ALL node");
        for (int i = 0; i < allTreeNode.Nodes.Count; i++)
        {
            BindingTreeNode bindingTreeNode = allTreeNode.Nodes[i] as BindingTreeNode;
            Debug.Assert(bindingTreeNode is not null, "we only put bindingTreeNodes in the ALL node");
            if (bindingTreeNode.Binding is not null)
            {
                this.bindings.Add(bindingTreeNode.Binding);
            }
        }

        if (ccs is not null && control is not null && prop is not null)
        {
            ccs.OnComponentChanged(control, prop, null, null);
        }
    }

    private void bindingUpdateDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (this.inLoad)
        {
            return;
        }

        this.dirty = true;
    }

    // will hold all the information in the tree node
    private class BindingTreeNode : TreeNode
    {
        Binding binding;
        // one of the "General", "Numeric", "Currency", "DateTime", "Percentage", "Scientific", "Custom" strings
        string formatType;

        public BindingTreeNode(string name) : base(name)
        {
        }

        public Binding Binding
        {
            get
            {
                return this.binding;
            }
            set
            {
                this.binding = value;
                this.ImageIndex = this.binding is not null ? BindingFormattingDialog.BOUNDIMAGEINDEX : BindingFormattingDialog.UNBOUNDIMAGEINDEX;
                this.SelectedImageIndex = this.binding is not null ? BindingFormattingDialog.BOUNDIMAGEINDEX : BindingFormattingDialog.UNBOUNDIMAGEINDEX;
            }
        }

        // one of the "General", "Numeric", "Currency", "DateTime", "Percentage", "Scientific", "Custom" strings
        public string FormatType
        {
            get
            {
                return this.formatType;
            }
            set
            {
                this.formatType = value;
            }
        }
    }

    private class TreeNodeComparer : IComparer
    {
        public TreeNodeComparer() { }

        int IComparer.Compare(object o1, object o2)
        {
            TreeNode treeNode1 = o1 as TreeNode;
            TreeNode treeNode2 = o2 as TreeNode;

            Debug.Assert(treeNode1 is not null && treeNode2 is not null, "this method only compares tree nodes");

            BindingTreeNode bindingTreeNode1 = treeNode1 as BindingTreeNode;
            BindingTreeNode bindingTreeNode2 = treeNode2 as BindingTreeNode;
            if (bindingTreeNode1 is not null)
            {
                Debug.Assert(bindingTreeNode2 is not null, "we compare nodes at the same level. and at the BindingTreeNode level are only BindingTreeNodes");
                return String.Compare(bindingTreeNode1.Text, bindingTreeNode2.Text, false /*ignoreCase*/, CultureInfo.CurrentCulture);
            }
            else
            {
                Debug.Assert(bindingTreeNode2 is null, "we compare nodes at the same level. and at the BindingTreeNode level are only BindingTreeNodes");
                if (String.Compare(treeNode1.Text, SR.GetString(SR.BindingFormattingDialogAllTreeNode), false /*ignoreCase*/, CultureInfo.CurrentCulture) == 0)
                {
                    if (String.Compare(treeNode2.Text, SR.GetString(SR.BindingFormattingDialogAllTreeNode), false /*ignoreCase*/, CultureInfo.CurrentCulture) == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        // we want to show "Common" before "All"
                        Debug.Assert(String.Compare(treeNode2.Text, SR.GetString(SR.BindingFormattingDialogCommonTreeNode), false /*ignoreCase*/, CultureInfo.CurrentCulture) == 0, " we only have All and Common at this level");
                        return 1;
                    }
                }
                else
                {
                    Debug.Assert(String.Compare(treeNode1.Text, SR.GetString(SR.BindingFormattingDialogCommonTreeNode), false /*ignoreCase*/, CultureInfo.CurrentCulture) == 0, " we only have All and Common at this level");

                    if (String.Compare(treeNode2.Text, SR.GetString(SR.BindingFormattingDialogCommonTreeNode), false /*ignoreCase*/, CultureInfo.CurrentCulture) == 0)
                    {
                        return 0;
                    }
                    else
                    {
                        // we want to show "Common" before "All"
                        Debug.Assert(String.Compare(treeNode2.Text, SR.GetString(SR.BindingFormattingDialogAllTreeNode), false /*ignoreCase*/, CultureInfo.CurrentCulture) == 0, " we only have All and Common at this level");
                        return -1;
                    }
                }
            }
        }
    }
}
