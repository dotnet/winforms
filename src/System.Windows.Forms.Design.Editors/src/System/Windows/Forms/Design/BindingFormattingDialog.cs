// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /*
    internal class BindingFormattingDialog : Form
    {
        // we need the context for the HELP service provider
        private ITypeDescriptorContext context = null;

        private ControlBindingsCollection bindings;

        private BindingFormattingWindowsFormsEditorService dataSourcePicker;
        private Label explanationLabel;
        private TableLayoutPanel mainTableLayoutPanel;
        private Label propertyLabel;
        private TreeView propertiesTreeView;
        private Label bindingLabel;
        private ComboBox bindingUpdateDropDown;
        private Label updateModeLabel;
        private FormatControl formatControl1;
        private TableLayoutPanel okCancelTableLayoutPanel;
        private Button okButton;
        private Button cancelButton;
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
                bindings = value;
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
                Bitmap scaledBitmap = DpiHelper.ScaleBitmapToSize(bitmap, new Size(width, height));
                if (scaledBitmap != null)
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
                if (boundBitmap == null)
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
                return context;
            }

            set
            {
                context = value;
                dataSourcePicker.Context = value;
            }
        }

        public bool Dirty
        {
            get
            {
                return dirty || formatControl1.Dirty;
            }
        }

        public IDesignerHost Host
        {
            set
            {
                host = value;
            }
        }

        private static Bitmap UnboundBitmap
        {
            get
            {
                if (unboundBitmap == null)
                {
                    unboundBitmap = ScaleBitmapIfNeeded("BindingFormattingDialog.Unbound.bmp");
                }
                return unboundBitmap;
            }
        }

        private void BindingFormattingDialog_Closing(object sender, CancelEventArgs e)
        {
            currentBindingTreeNode = null;
            dataSourcePicker.OwnerComponent = null;

            formatControl1.ResetFormattingInfo();
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
            IHelpService helpService = context.GetService(typeof(IHelpService)) as IHelpService;
            if (helpService != null)
            {
                helpService.ShowHelpFromKeyword("vs.BindingFormattingDialog");
            }
        }

        private void BindingFormattingDialog_Load(object sender, EventArgs e)
        {
            inLoad = true;

            try
            {
                // start a new transaction
                dirty = false;

                // get the dialog font
                System.Drawing.Font uiFont = Control.DefaultFont;
                IUIService uiService = null;
                if (bindings.BindableComponent.Site != null)
                {
                    uiService = (IUIService)bindings.BindableComponent.Site.GetService(typeof(IUIService));
                }

                if (uiService != null)
                {
                    uiFont = (System.Drawing.Font)uiService.Styles["DialogFont"];
                }

                Font = uiFont;

                // enable explorer tree style
                DesignerUtils.ApplyTreeViewThemeStyles(propertiesTreeView);

                // push the image list in the tree view
                if (propertiesTreeView.ImageList == null)
                {
                    ImageList il = new ImageList();
                    il.Images.Add(BoundBitmap);
                    il.Images.Add(UnboundBitmap);
                    if (DpiHelper.IsScalingRequired)
                    {
                        il.ImageSize = BoundBitmap.Size;
                    }
                    propertiesTreeView.ImageList = il;
                }

                // get the defaultBindingProperty and / or defaultProperty
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
                propertiesTreeView.Nodes.Clear();
                TreeNode commonNode = new TreeNode(SR.BindingFormattingDialogCommonTreeNode);
                TreeNode allNode = new TreeNode(SR.BindingFormattingDialogAllTreeNode);

                propertiesTreeView.Nodes.Add(commonNode);
                propertiesTreeView.Nodes.Add(allNode);

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
                    if (browsable != null && !browsable.Browsable && (bindableAttr == null || !bindableAttr.Bindable))
                    {
                        continue;
                    }

                    BindingTreeNode treeNode = new BindingTreeNode(props[i].Name);

                    treeNode.Binding = FindBinding(props[i].Name);

                    // Make a reasonable guess as to what the FormatType is
                    if (treeNode.Binding != null)
                    {
                        treeNode.FormatType = FormatControl.FormatTypeStringFromFormatString(treeNode.Binding.FormatString);
                    }
                    else
                    {
                        treeNode.FormatType = SR.BindingFormattingDialogFormatTypeNoFormatting;
                    }

                    if (bindableAttr != null && bindableAttr.Bindable)
                    {
                        commonNode.Nodes.Add(treeNode);
                    }
                    else
                    {
                        allNode.Nodes.Add(treeNode);
                    }

                    if (defaultBindingPropertyNode == null &&
                        !string.IsNullOrEmpty(defaultBindingPropertyName) &&
                        string.Compare(props[i].Name, defaultBindingPropertyName, false, CultureInfo.CurrentCulture) == 0)
                    {
                        defaultBindingPropertyNode = treeNode;
                    }
                    else if (defaultPropertyNode == null &&
                             !string.IsNullOrEmpty(defaultPropertyName) &&
                             string.Compare(props[i].Name, defaultPropertyName, false, CultureInfo.CurrentCulture) == 0)
                    {
                        defaultPropertyNode = treeNode;
                    }
                }

                commonNode.Expand();
                allNode.Expand();

                propertiesTreeView.Sort();

                // set the default node
                // 1. if we have a DefaultBindingProperty then select it; else
                // 2. if we have a DefaultProperty then select it
                // 3. select the first node in "All" nodes
                // 4. select the first node in "Common" nodes
                BindingTreeNode selectedNode;
                if (defaultBindingPropertyNode != null)
                {
                    selectedNode = defaultBindingPropertyNode;
                }
                else if (defaultPropertyNode != null)
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

                propertiesTreeView.SelectedNode = selectedNode;
                if (selectedNode != null)
                {
                    selectedNode.EnsureVisible();
                }

                dataSourcePicker.PropertyName = selectedNode.Text;
                dataSourcePicker.Binding = selectedNode != null ? selectedNode.Binding : null;
                dataSourcePicker.Enabled = true;
                dataSourcePicker.OwnerComponent = bindings.BindableComponent;
                dataSourcePicker.DefaultDataSourceUpdateMode = bindings.DefaultDataSourceUpdateMode;

                if (selectedNode != null && selectedNode.Binding != null)
                {
                    bindingUpdateDropDown.Enabled = true;
                    bindingUpdateDropDown.SelectedItem = selectedNode.Binding.DataSourceUpdateMode;
                    updateModeLabel.Enabled = true;
                    formatControl1.Enabled = true;

                    // setup the format control
                    formatControl1.FormatType = selectedNode.FormatType;
                    FormatControl.FormatTypeClass formatTypeItem = formatControl1.FormatTypeItem;
                    Debug.Assert(formatTypeItem != null, "The FormatString and FormatProvider was not persisted corectly for this binding");

                    formatTypeItem.PushFormatStringIntoFormatType(selectedNode.Binding.FormatString);
                    if (selectedNode.Binding.NullValue != null)
                    {
                        formatControl1.NullValue = selectedNode.Binding.NullValue.ToString();
                    }
                    else
                    {
                        formatControl1.NullValue = string.Empty;
                    }
                }
                else
                {
                    bindingUpdateDropDown.Enabled = false;
                    bindingUpdateDropDown.SelectedItem = bindings.DefaultDataSourceUpdateMode;
                    updateModeLabel.Enabled = false;
                    formatControl1.Enabled = false;
                    formatControl1.FormatType = string.Empty;
                }

                // tell the format control that we start a new transaction
                // we have to do this after we set the formatControl
                formatControl1.Dirty = false;

                // set the currentBindingTreeNode
                currentBindingTreeNode = propertiesTreeView.SelectedNode as BindingTreeNode;

            }
            finally
            {
                inLoad = false;
            }

            //
            // Done
            //
        }

        // given the property name, this function will return the binding, if there is any
        private Binding FindBinding(string propertyName)
        {
            for (int i = 0; i < bindings.Count; i++)
            {
                if (string.Equals(propertyName, bindings[i].PropertyName, StringComparison.OrdinalIgnoreCase))
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
                if (string.Compare(result.Text, nodes[i].Text, false, CultureInfo.CurrentCulture) > 0)
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
            explanationLabel = new System.Windows.Forms.Label();
            mainTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            propertiesTreeView = new System.Windows.Forms.TreeView();
            propertyLabel = new System.Windows.Forms.Label();
            dataSourcePicker = new BindingFormattingWindowsFormsEditorService();
            bindingLabel = new System.Windows.Forms.Label();
            updateModeLabel = new System.Windows.Forms.Label();
            bindingUpdateDropDown = new System.Windows.Forms.ComboBox();
            formatControl1 = new FormatControl();
            okCancelTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            okButton = new System.Windows.Forms.Button();
            cancelButton = new System.Windows.Forms.Button();
            mainTableLayoutPanel.SuspendLayout();
            okCancelTableLayoutPanel.SuspendLayout();
            ShowIcon = false;
            SuspendLayout();
            // 
            // explanationLabel
            // 
            resources.ApplyResources(explanationLabel, "explanationLabel");
            mainTableLayoutPanel.SetColumnSpan(explanationLabel, 3);
            explanationLabel.Name = "explanationLabel";
            // 
            // mainTableLayoutPanel
            // 
            resources.ApplyResources(mainTableLayoutPanel, "mainTableLayoutPanel");
            mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            mainTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            mainTableLayoutPanel.Controls.Add(okCancelTableLayoutPanel, 2, 4);
            mainTableLayoutPanel.Controls.Add(formatControl1, 1, 3);
            mainTableLayoutPanel.Controls.Add(bindingUpdateDropDown, 2, 2);
            mainTableLayoutPanel.Controls.Add(propertiesTreeView, 0, 2);
            mainTableLayoutPanel.Controls.Add(updateModeLabel, 2, 1);
            mainTableLayoutPanel.Controls.Add(dataSourcePicker, 1, 2);
            mainTableLayoutPanel.Controls.Add(explanationLabel, 0, 0);
            mainTableLayoutPanel.Controls.Add(bindingLabel, 1, 1);
            mainTableLayoutPanel.Controls.Add(propertyLabel, 0, 1);
            mainTableLayoutPanel.MinimumSize = new System.Drawing.Size(542, 283);
            mainTableLayoutPanel.Name = "mainTableLayoutPanel";
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            mainTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // propertiesTreeView
            // 
            resources.ApplyResources(propertiesTreeView, "propertiesTreeView");
            propertiesTreeView.Name = "propertiesTreeView";
            propertiesTreeView.HideSelection = false;
            propertiesTreeView.TreeViewNodeSorter = new TreeNodeComparer();
            mainTableLayoutPanel.SetRowSpan(propertiesTreeView, 2);
            propertiesTreeView.BeforeSelect += new TreeViewCancelEventHandler(propertiesTreeView_BeforeSelect);
            propertiesTreeView.AfterSelect += new TreeViewEventHandler(propertiesTreeView_AfterSelect);
            // 
            // propertyLabel
            // 
            resources.ApplyResources(propertyLabel, "propertyLabel");
            propertyLabel.Name = "propertyLabel";
            // 
            // dataSourcePicker
            // 
            resources.ApplyResources(dataSourcePicker, "dataSourcePicker");
            dataSourcePicker.Name = "dataSourcePicker";
            dataSourcePicker.PropertyValueChanged += new System.EventHandler(dataSourcePicker_PropertyValueChanged);
            // 
            // bindingLabel
            // 
            resources.ApplyResources(bindingLabel, "bindingLabel");
            bindingLabel.Name = "bindingLabel";
            // 
            // updateModeLabel
            // 
            resources.ApplyResources(updateModeLabel, "updateModeLabel");
            updateModeLabel.Name = "updateModeLabel";
            // 
            // bindingUpdateDropDown
            // 
            bindingUpdateDropDown.FormattingEnabled = true;
            resources.ApplyResources(bindingUpdateDropDown, "bindingUpdateDropDown");
            bindingUpdateDropDown.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            bindingUpdateDropDown.Name = "bindingUpdateDropDown";
            bindingUpdateDropDown.Items.AddRange(new object[] { DataSourceUpdateMode.Never, DataSourceUpdateMode.OnPropertyChanged, DataSourceUpdateMode.OnValidation });
            bindingUpdateDropDown.SelectedIndexChanged += new System.EventHandler(bindingUpdateDropDown_SelectedIndexChanged);
            // 
            // formatControl1
            // 
            mainTableLayoutPanel.SetColumnSpan(formatControl1, 2);
            resources.ApplyResources(formatControl1, "formatControl1");
            formatControl1.MinimumSize = new System.Drawing.Size(390, 237);
            formatControl1.Name = "formatControl1";
            formatControl1.NullValueTextBoxEnabled = true;
            // 
            // okCancelTableLayoutPanel
            // 
            resources.ApplyResources(okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            okCancelTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            okCancelTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            okCancelTableLayoutPanel.Controls.Add(cancelButton, 1, 0);
            okCancelTableLayoutPanel.Controls.Add(okButton, 0, 0);
            okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            // 
            // okButton
            // 
            resources.ApplyResources(okButton, "okButton");
            okButton.Name = "okButton";
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Click += new EventHandler(okButton_Click);
            // 
            // cancelButton
            // 
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.Name = "cancelButton";
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Click += new EventHandler(cancelButton_Click);
            // 
            // BindingFormattingDialog
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            CancelButton = cancelButton;
            AcceptButton = okButton;
            Controls.Add(mainTableLayoutPanel);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            Name = "BindingFormattingDialog";
            mainTableLayoutPanel.ResumeLayout(false);
            mainTableLayoutPanel.PerformLayout();
            okCancelTableLayoutPanel.ResumeLayout(false);
            HelpButton = true;
            ShowInTaskbar = false;
            MinimizeBox = false;
            MaximizeBox = false;
            Load += new EventHandler(BindingFormattingDialog_Load);
            Closing += new CancelEventHandler(BindingFormattingDialog_Closing);
            HelpButtonClicked += new CancelEventHandler(BindingFormattingDialog_HelpButtonClicked);
            HelpRequested += new HelpEventHandler(BindingFormattingDialog_HelpRequested);
            ResumeLayout(false);
            PerformLayout();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            dirty = false;
        }

        // this will consolidate the information from the form in the currentBindingTreeNode member variable

        [
        SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")      // We can't avoid casting from a ComboBox.
        ]
        private void ConsolidateBindingInformation()
        {
            Debug.Assert(currentBindingTreeNode != null, "we need a binding tree node to consolidate this information");

            Binding binding = dataSourcePicker.Binding;

            if (binding == null)
            {
                return;
            }

            // Whidbey Data Binding will have FormattingEnabled set to true
            binding.FormattingEnabled = true;
            currentBindingTreeNode.Binding = binding;
            currentBindingTreeNode.FormatType = formatControl1.FormatType;

            FormatControl.FormatTypeClass formatTypeItem = formatControl1.FormatTypeItem;

            if (formatTypeItem != null)
            {
                binding.FormatString = formatTypeItem.FormatString;
                binding.NullValue = formatControl1.NullValue;
            }

            binding.DataSourceUpdateMode = (DataSourceUpdateMode)bindingUpdateDropDown.SelectedItem;

        }

        private void dataSourcePicker_PropertyValueChanged(object sender, System.EventArgs e)
        {
            if (inLoad)
            {
                return;
            }

            BindingTreeNode bindingTreeNode = propertiesTreeView.SelectedNode as BindingTreeNode;
            Debug.Assert(bindingTreeNode != null, " the data source drop down is active only when the user is editing a binding tree node");

            if (dataSourcePicker.Binding == bindingTreeNode.Binding)
            {
                return;
            }

            Binding binding = dataSourcePicker.Binding;

            if (binding != null)
            {
                binding.FormattingEnabled = true;

                Binding currentBinding = bindingTreeNode.Binding;
                if (currentBinding != null)
                {
                    binding.FormatString = currentBinding.FormatString;
                    binding.NullValue = currentBinding.NullValue;
                    binding.FormatInfo = currentBinding.FormatInfo;
                }
            }

            bindingTreeNode.Binding = binding;

            // enable/disable the format control
            if (binding != null)
            {
                formatControl1.Enabled = true;
                updateModeLabel.Enabled = true;
                bindingUpdateDropDown.Enabled = true;
                bindingUpdateDropDown.SelectedItem = binding.DataSourceUpdateMode;

                if (!string.IsNullOrEmpty(formatControl1.FormatType))
                {
                    // push the current user control into the format control type
                    formatControl1.FormatType = formatControl1.FormatType;
                }
                else
                {
                    formatControl1.FormatType = SR.BindingFormattingDialogFormatTypeNoFormatting;
                }
            }
            else
            {
                formatControl1.Enabled = false;
                updateModeLabel.Enabled = false;
                bindingUpdateDropDown.Enabled = false;
                bindingUpdateDropDown.SelectedItem = bindings.DefaultDataSourceUpdateMode;

                formatControl1.FormatType = SR.BindingFormattingDialogFormatTypeNoFormatting;
            }

            // dirty the form
            dirty = true;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            // save the information for the current binding
            if (currentBindingTreeNode != null)
            {
                ConsolidateBindingInformation();
            }

            // push the changes
            PushChanges();
        }

        private void propertiesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (inLoad)
            {
                return;
            }

            BindingTreeNode bindingTreeNode = e.Node as BindingTreeNode;

            if (bindingTreeNode == null)
            {
                // disable the data source drop down when the active tree node is not a binding node
                dataSourcePicker.Binding = null;
                bindingLabel.Enabled = dataSourcePicker.Enabled = false;
                updateModeLabel.Enabled = bindingUpdateDropDown.Enabled = false;
                formatControl1.Enabled = false;
                return;
            }

            // make sure the the drop down is enabled
            bindingLabel.Enabled = dataSourcePicker.Enabled = true;
            dataSourcePicker.PropertyName = bindingTreeNode.Text;

            // enable the update mode drop down only if the user is editing a binding;
            updateModeLabel.Enabled = bindingUpdateDropDown.Enabled = false;

            // enable the format control only if the user is editing a binding
            formatControl1.Enabled = false;

            if (bindingTreeNode.Binding != null)
            {
                // this is not the first time we visit this binding
                // restore the binding information from the last time the user touched this binding
                formatControl1.Enabled = true;
                formatControl1.FormatType = bindingTreeNode.FormatType;
                Debug.Assert(formatControl1.FormatTypeItem != null, "FormatType did not persist well for this binding");

                FormatControl.FormatTypeClass formatTypeItem = formatControl1.FormatTypeItem;

                dataSourcePicker.Binding = bindingTreeNode.Binding;

                formatTypeItem.PushFormatStringIntoFormatType(bindingTreeNode.Binding.FormatString);
                if (bindingTreeNode.Binding.NullValue != null)
                {
                    formatControl1.NullValue = bindingTreeNode.Binding.NullValue.ToString();
                }
                else
                {
                    formatControl1.NullValue = string.Empty;
                }

                bindingUpdateDropDown.SelectedItem = bindingTreeNode.Binding.DataSourceUpdateMode;
                Debug.Assert(bindingUpdateDropDown.SelectedItem != null, "Binding.UpdateMode was not persisted corectly for this binding");
                updateModeLabel.Enabled = bindingUpdateDropDown.Enabled = true;
            }
            else
            {
                bool currentDirtyState = dirty;
                dataSourcePicker.Binding = null;

                formatControl1.FormatType = bindingTreeNode.FormatType;
                bindingUpdateDropDown.SelectedItem = bindings.DefaultDataSourceUpdateMode;

                formatControl1.NullValue = null;
                dirty = currentDirtyState;
            }

            formatControl1.Dirty = false;

            // now save this node so that when we get the BeforeSelect event we know which node was affected
            currentBindingTreeNode = bindingTreeNode;
        }

        private void propertiesTreeView_BeforeSelect(object sender, TreeViewCancelEventArgs e)
        {
            if (inLoad)
            {
                return;
            }

            if (currentBindingTreeNode == null)
            {
                return;
            }

            // if there is no selected field quit
            if (dataSourcePicker.Binding == null)
            {
                return;
            }

            // if the format control was not touched quit
            if (!formatControl1.Enabled)
            {
                return;
            }

            ConsolidateBindingInformation();

            // dirty the form
            dirty = dirty || formatControl1.Dirty;
        }

        private void PushChanges()
        {
            if (!Dirty)
            {
                return;
            }

            IComponentChangeService ccs = host.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
            PropertyDescriptor prop = null;
            IBindableComponent control = bindings.BindableComponent;
            if (ccs != null && control != null)
            {
                prop = TypeDescriptor.GetProperties(control)["DataBindings"];
                if (prop != null)
                {
                    ccs.OnComponentChanging(control, prop);
                }
            }

            // clear the bindings collection and insert the new bindings
            bindings.Clear();

            // get the bindings from the "Common" tree nodes
            TreeNode commonTreeNode = propertiesTreeView.Nodes[0];
            Debug.Assert(commonTreeNode.Text.Equals(SR.BindingFormattingDialogCommonTreeNode), "the first node in the tree view should be the COMMON node");
            for (int i = 0; i < commonTreeNode.Nodes.Count; i++)
            {
                BindingTreeNode bindingTreeNode = commonTreeNode.Nodes[i] as BindingTreeNode;
                Debug.Assert(bindingTreeNode != null, "we only put bindingTreeNodes in the COMMON node");
                if (bindingTreeNode.Binding != null)
                {
                    bindings.Add(bindingTreeNode.Binding);
                }
            }

            // get the bindings from the "All" tree nodes
            TreeNode allTreeNode = propertiesTreeView.Nodes[1];
            Debug.Assert(allTreeNode.Text.Equals(SR.BindingFormattingDialogAllTreeNode), "the second node in the tree view should be the ALL node");
            for (int i = 0; i < allTreeNode.Nodes.Count; i++)
            {
                BindingTreeNode bindingTreeNode = allTreeNode.Nodes[i] as BindingTreeNode;
                Debug.Assert(bindingTreeNode != null, "we only put bindingTreeNodes in the ALL node");
                if (bindingTreeNode.Binding != null)
                {
                    bindings.Add(bindingTreeNode.Binding);
                }
            }

            if (ccs != null && control != null && prop != null)
            {
                ccs.OnComponentChanged(control, prop, null, null);
            }
        }

        private void bindingUpdateDropDown_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (inLoad)
            {
                return;
            }

            dirty = true;
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
                    return binding;
                }
                set
                {
                    binding = value;
                    ImageIndex = binding != null ? BindingFormattingDialog.BOUNDIMAGEINDEX : BindingFormattingDialog.UNBOUNDIMAGEINDEX;
                    SelectedImageIndex = binding != null ? BindingFormattingDialog.BOUNDIMAGEINDEX : BindingFormattingDialog.UNBOUNDIMAGEINDEX;
                }
            }

            // one of the "General", "Numeric", "Currency", "DateTime", "Percentage", "Scientific", "Custom" strings
            public string FormatType
            {
                get
                {
                    return formatType;
                }
                set
                {
                    formatType = value;
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

                Debug.Assert(treeNode1 != null && treeNode2 != null, "this method only compares tree nodes");

                BindingTreeNode bindingTreeNode1 = treeNode1 as BindingTreeNode;
                BindingTreeNode bindingTreeNode2 = treeNode2 as BindingTreeNode;
                if (bindingTreeNode1 != null)
                {
                    Debug.Assert(bindingTreeNode2 != null, "we compare nodes at the same level. and at the BindingTreeNode level are only BindingTreeNodes");
                    return string.Compare(bindingTreeNode1.Text, bindingTreeNode2.Text, false, CultureInfo.CurrentCulture);
                }
                else
                {
                    Debug.Assert(bindingTreeNode2 == null, "we compare nodes at the same level. and at the BindingTreeNode level are only BindingTreeNodes");
                    if (string.Compare(treeNode1.Text, SR.BindingFormattingDialogAllTreeNode, false, CultureInfo.CurrentCulture) == 0)
                    {
                        if (string.Compare(treeNode2.Text, SR.BindingFormattingDialogAllTreeNode, false, CultureInfo.CurrentCulture) == 0)
                        {
                            return 0;
                        }
                        else
                        {
                            // we want to show "Common" before "All"
                            Debug.Assert(string.Compare(treeNode2.Text, SR.BindingFormattingDialogCommonTreeNode, false, CultureInfo.CurrentCulture) == 0, " we only have All and Common at this level");
                            return 1;
                        }
                    }
                    else
                    {
                        Debug.Assert(string.Compare(treeNode1.Text, SR.BindingFormattingDialogCommonTreeNode, false , CultureInfo.CurrentCulture) == 0, " we only have All and Common at this level");

                        if (string.Compare(treeNode2.Text, SR.BindingFormattingDialogCommonTreeNode, false, CultureInfo.CurrentCulture) == 0)
                        {
                            return 0;
                        }
                        else
                        {
                            // we want to show "Common" before "All"
                            Debug.Assert(string.Compare(treeNode2.Text, SR.BindingFormattingDialogAllTreeNode, false, CultureInfo.CurrentCulture) == 0, " we only have All and Common at this level");
                            return -1;
                        }
                    }
                }
            }
        }
    }*/
}
