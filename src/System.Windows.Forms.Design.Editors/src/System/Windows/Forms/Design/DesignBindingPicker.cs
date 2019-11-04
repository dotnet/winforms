// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /*
    [
    ToolboxItem(false),
    DesignTimeVisible(false)
    ]
    internal class DesignBindingPicker : ContainerControl
    {

        private BindingPickerTree treeViewCtrl;   // Tree view that shows the available data sources and data members
        private BindingPickerLink addNewCtrl;     // Link that invokes the "Add Project Data Source" wizard
        private Panel addNewPanel;    // Panel containing the "Add Project Data Source" link
        private HelpTextLabel helpTextCtrl;   // Label that displays helpful text as user mouses over tree view nodes
        private Panel helpTextPanel;  // Panel containing the help text label

        private IServiceProvider serviceProvider; // Current VS service provider
        private IWindowsFormsEditorService edSvc; // Service used to invoke the picker inside a modal dropdown
        private DataSourceProviderService dspSvc; // Service that provides project level data sources and related commands
        private ITypeResolutionService typeSvc;   // Service that can return Type info for types in the user's project, at design time
        private IDesignerHost hostSvc;            // Service that provides access to current WinForms designer session

        private bool showDataSources;   // True to show all data sources, false to just show contents of root data source
        private bool showDataMembers;   // True to show data members of every data source, false to omit data members
        private bool selectListMembers; // True to allow selection of list members, false to allow selection of field members

        private object rootDataSource;    // Root data source used to build tree (set when picker is invoked)
        private string rootDataMember;    // Root data member used to build tree (set when picker is invoked)

        private DesignBinding selectedItem;      // Describes the initial selection on open, and the final selection on close
        private TreeNode selectedNode;      // Tree node that matches the initial selected item (selectedItem)
        private bool inSelectNode;      // Prevents processing of node expansion events when auot-selecting a tree node

        private NoneNode noneNode;          // "None" tree node
        private OtherNode otherNode;         // "Other Data Sources" tree node
        private ProjectNode projectNode;       // "Project Data Sources" tree node
        private InstancesNode instancesNode;     // "Form List Instances" tree node

        private const int minimumDimension = 250;
        private static int minimumHeight = minimumDimension;
        private static int minimumWidth = minimumDimension;
        private static bool isScalingInitialized = false;
        private ITypeDescriptorContext context;   // Context of the current 'pick' operation

        private int pixel_1 = 1;
        private Size initialSize;
        private BindingContext bindingContext = new BindingContext();

        // The type of RuntimeType.
        // When binding to a business object, the DesignBindingPicker needs to create an instance of the business object.
        // However, Activator.CreateInstance works only with RuntimeType - it does not work w/ Virtual Types.
        // We use the runtimeType static to determine if the type of business object is a runtime type or not.
        private static Type runtimeType = typeof(object).GetType().GetType();

        /// <summary>
        /// Rebuilding binding picker according to new dpi received.
        /// </summary>
        private void BuildBindingPicker(int newDpi, int oldDpi)
        {

            var factor = ((double)newDpi) / oldDpi;
            Label addNewDiv = new Label();
            addNewDiv.Height = DpiHelper.ConvertToGivenDpiPixel(pixel_1, factor);
            addNewDiv.BackColor = SystemColors.ControlDark;
            addNewDiv.Dock = DockStyle.Top;

            addNewCtrl = new BindingPickerLink();
            addNewCtrl.Text = SR.DesignBindingPickerAddProjDataSourceLabel;
            addNewCtrl.TextAlign = ContentAlignment.MiddleLeft;
            addNewCtrl.BackColor = SystemColors.Window;
            addNewCtrl.ForeColor = SystemColors.WindowText;
            addNewCtrl.LinkBehavior = LinkBehavior.HoverUnderline;
            addNewCtrl.LinkClicked += new LinkLabelLinkClickedEventHandler(addNewCtrl_Click);

            // BindingPickerLink always initialize to primary monitor Dpi. Resizing to current Dpi.
            addNewCtrl.Height = DpiHelper.ConvertToGivenDpiPixel(addNewCtrl.Height, factor);


            Bitmap addNewBitmap = new Bitmap(BitmapSelector.GetResourceStream(typeof(DesignBindingPicker), "AddNewDataSource.bmp"));
            addNewBitmap.MakeTransparent(Color.Magenta);
            DpiHelper.ScaleBitmapLogicalToDevice(ref addNewBitmap, newDpi);


            PictureBox addNewIcon = new PictureBox();
            addNewIcon.Image = addNewBitmap;
            addNewIcon.BackColor = SystemColors.Window;
            addNewIcon.ForeColor = SystemColors.WindowText;
            addNewIcon.Width = addNewCtrl.Height;
            addNewIcon.Height = addNewCtrl.Height;
            addNewIcon.Dock = DockStyle.Left;
            addNewIcon.SizeMode = PictureBoxSizeMode.CenterImage;
            addNewIcon.AccessibleRole = AccessibleRole.Graphic;

            Label helpTextDiv = new Label();
            helpTextDiv.Height = DpiHelper.ConvertToGivenDpiPixel(pixel_1, factor);
            helpTextDiv.BackColor = SystemColors.ControlDark;
            helpTextDiv.Dock = DockStyle.Top;

            helpTextCtrl = new HelpTextLabel();
            helpTextCtrl.TextAlign = ContentAlignment.TopLeft;
            helpTextCtrl.BackColor = SystemColors.Window;
            helpTextCtrl.ForeColor = SystemColors.WindowText;
            helpTextCtrl.Height *= 2;
            int helpTextHeight = DpiHelper.ConvertToGivenDpiPixel(helpTextCtrl.Height, factor);


            addNewPanel.Height = addNewIcon.Height + pixel_1;
            addNewPanel.Controls.Add(addNewCtrl);
            addNewPanel.Controls.Add(addNewIcon);
            addNewPanel.Controls.Add(addNewDiv);

            helpTextPanel.Controls.Add(helpTextCtrl);
            helpTextPanel.Controls.Add(helpTextDiv);
            helpTextPanel.Height = helpTextHeight + pixel_1;
            ResetStyles(false);

            Controls.Add(addNewPanel);
            Controls.Add(helpTextPanel);
        }

        private void ResetStyles(bool toNone)
        {
            if (toNone)
            {
                treeViewCtrl.Dock = DockStyle.None;
                addNewCtrl.Dock = DockStyle.None;
                addNewPanel.Dock = DockStyle.None;
                helpTextCtrl.Dock = DockStyle.None;
                helpTextPanel.Dock = DockStyle.None;
            }
            else
            {
                treeViewCtrl.Dock = DockStyle.Fill;
                addNewCtrl.Dock = DockStyle.Fill;
                addNewPanel.Dock = DockStyle.Bottom;
                helpTextCtrl.Dock = DockStyle.Fill;
                helpTextPanel.Dock = DockStyle.Bottom;
            }
        }

        private void InitTreeViewCtl()
        {
            treeViewCtrl = new BindingPickerTree();
            treeViewCtrl.HotTracking = true;
            treeViewCtrl.BackColor = SystemColors.Window;
            treeViewCtrl.ForeColor = SystemColors.WindowText;
            treeViewCtrl.BorderStyle = BorderStyle.None;
            initialSize = treeViewCtrl.Size;
            treeViewCtrl.Dock = DockStyle.Fill;
            treeViewCtrl.MouseMove += new MouseEventHandler(treeViewCtrl_MouseMove);
            treeViewCtrl.MouseLeave += new EventHandler(treeViewCtrl_MouseLeave);
            treeViewCtrl.AfterExpand += new TreeViewEventHandler(treeViewCtrl_AfterExpand);
            treeViewCtrl.AccessibleName = SR.DesignBindingPickerTreeViewAccessibleName;

            // enable explorer tree view style
            DesignerUtils.ApplyTreeViewThemeStyles(treeViewCtrl);
        }

        /// <devdoc>
        ///
        /// Constructor - Initializes child controls and window layout
        ///
        /// </devdoc>
        public DesignBindingPicker()
        {
            SuspendLayout();
            if (!isScalingInitialized)
            {
                if (DpiHelper.IsScalingRequired)
                {
                    minimumHeight = DpiHelper.LogicalToDeviceUnitsY(minimumDimension);
                    minimumWidth = DpiHelper.LogicalToDeviceUnitsX(minimumDimension);
                }
                isScalingInitialized = true;
            }

            if (!DpiHelper.EnableDpiChangedHighDpiImprovements)
            {

                InitTreeViewCtl();

                Label addNewDiv = new Label();
                addNewDiv.Height = 1;
                addNewDiv.BackColor = SystemColors.ControlDark;
                addNewDiv.Dock = DockStyle.Top;

                addNewCtrl = new BindingPickerLink();
                addNewCtrl.Text = SR.DesignBindingPickerAddProjDataSourceLabel;
                addNewCtrl.TextAlign = ContentAlignment.MiddleLeft;
                addNewCtrl.BackColor = SystemColors.Window;
                addNewCtrl.ForeColor = SystemColors.WindowText;
                addNewCtrl.LinkBehavior = LinkBehavior.HoverUnderline;

                // use height of text for both dimensions of the Icon
                int addNewHeight = addNewCtrl.Height;
                int addNewWidth = addNewCtrl.Height;
                addNewCtrl.Dock = DockStyle.Fill;
                addNewCtrl.LinkClicked += new LinkLabelLinkClickedEventHandler(addNewCtrl_Click);

                Bitmap addNewBitmap = new Bitmap(typeof(DesignBindingPicker), "AddNewDataSource.bmp");
                addNewBitmap.MakeTransparent(Color.Magenta);
                if (DpiHelper.IsScalingRequired)
                {
                    DpiHelper.ScaleBitmapLogicalToDevice(ref addNewBitmap);
                    addNewHeight = DpiHelper.LogicalToDeviceUnitsY(addNewCtrl.Height);
                    addNewWidth = DpiHelper.LogicalToDeviceUnitsX(addNewCtrl.Height);
                }

                PictureBox addNewIcon = new PictureBox();
                addNewIcon.Image = addNewBitmap;
                addNewIcon.BackColor = SystemColors.Window;
                addNewIcon.ForeColor = SystemColors.WindowText;
                addNewIcon.Width = addNewWidth;
                addNewIcon.Height = addNewHeight;
                addNewIcon.Dock = DockStyle.Left;
                addNewIcon.SizeMode = PictureBoxSizeMode.CenterImage;
                addNewIcon.AccessibleRole = AccessibleRole.Graphic;

                addNewPanel = new Panel();
                addNewPanel.Controls.Add(addNewCtrl);
                addNewPanel.Controls.Add(addNewIcon);
                addNewPanel.Controls.Add(addNewDiv);
                addNewPanel.Height = addNewHeight + 1;
                addNewPanel.Dock = DockStyle.Bottom;

                Label helpTextDiv = new Label();
                helpTextDiv.Height = 1;
                helpTextDiv.BackColor = SystemColors.ControlDark;
                helpTextDiv.Dock = DockStyle.Top;

                helpTextCtrl = new HelpTextLabel();
                helpTextCtrl.TextAlign = ContentAlignment.TopLeft;
                helpTextCtrl.BackColor = SystemColors.Window;
                helpTextCtrl.ForeColor = SystemColors.WindowText;
                helpTextCtrl.Height *= 2;
                int helpTextHeight = helpTextCtrl.Height;
                if (DpiHelper.IsScalingRequired)
                {
                    helpTextHeight = DpiHelper.LogicalToDeviceUnitsY(helpTextHeight);
                }
                helpTextCtrl.Dock = DockStyle.Fill;

                helpTextPanel = new Panel();
                helpTextPanel.Controls.Add(helpTextCtrl);
                helpTextPanel.Controls.Add(helpTextDiv);
                helpTextPanel.Height = helpTextHeight + 1;
                helpTextPanel.Dock = DockStyle.Bottom;

                Controls.Add(treeViewCtrl);
                Controls.Add(addNewPanel);
                Controls.Add(helpTextPanel);
            }
            else
            {
                InitTreeViewCtl();
                Controls.Add(treeViewCtrl);
                addNewPanel = new Panel();
                helpTextPanel = new Panel();
                //Build binding picker according to DpiFactor
                BuildBindingPicker(DpiHelper.DeviceDpi, (int)DpiHelper.LogicalDpi);
            }

            ResumeLayout(false);

            Size = initialSize;
            BackColor = SystemColors.Control;
            ActiveControl = treeViewCtrl;
            AccessibleName = SR.DesignBindingPickerAccessibleName;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        /// <devdoc>
        /// 
        /// Invokes picker as a dropdown control, allowing user to pick a data source
        /// or data member to apply to some property of some component or control.
        /// This is a modal call - it doesn't return until the dropdown closes.
        /// 
        /// Arguments:
        /// 
        ///     context              - Context of operation (ie. which property of which object is being set)
        ///     provider             - VS service provider (for IWindowsFormsEditorService and DataSourceProviderService)
        ///     showDataSources      - True to show all data sources, false to just show contents of root data source
        ///     showDataMembers      - True to show data members of every data source, false to omit data members
        ///     selectListMembers    - True to allow selection of list members, false to allow selection of field members
        ///     rootObjectDataSource - Root data source, who's members we want to show (ignored if showDataSources = true)
        ///     rootObjectDataMember - Optional: For identifying root data source through data member of another data source
        ///     initialSelectedItem  - Optional: Describes which binding to show as the initial selection
        /// 
        /// Return value:
        /// 
        ///     Returns a DesignBinding that describes the binding
        ///     the user picked, or null if no selection was made.
        /// 
        /// </devdoc>

        /// FXCOP suggests we use generics to avoid boxing of value types when referencing
        /// values in the uiService.Styles hashtable.  However, the values contained within
        /// can be of differing types - so we cannot do this.  Hence the suppression.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")]
        public DesignBinding Pick(ITypeDescriptorContext context,
                                  IServiceProvider provider,
                                  bool showDataSources,
                                  bool showDataMembers,
                                  bool selectListMembers,
                                  object rootDataSource,
                                  string rootDataMember,
                                  DesignBinding initialSelectedItem)
        {
            // Get services
            serviceProvider = provider;
            edSvc = (IWindowsFormsEditorService)serviceProvider.GetService(typeof(IWindowsFormsEditorService));
            dspSvc = (DataSourceProviderService)serviceProvider.GetService(typeof(DataSourceProviderService));
            typeSvc = (ITypeResolutionService)serviceProvider.GetService(typeof(ITypeResolutionService));
            hostSvc = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));

            if (edSvc == null)
            {
                return null;
            }

            // Record basic settings
            this.context = context;
            this.showDataSources = showDataSources;
            this.showDataMembers = showDataMembers;
            this.selectListMembers = showDataMembers ? selectListMembers : true;
            this.rootDataSource = rootDataSource;
            this.rootDataMember = rootDataMember;


            //Attempt to adjust the linklabel colors if we can get our ui service
            IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
            if (uiService != null)
            {
                if (uiService.Styles["VsColorPanelHyperLink"] is Color)
                {
                    addNewCtrl.LinkColor = (Color)uiService.Styles["VsColorPanelHyperLink"];
                }
                if (uiService.Styles["VsColorPanelHyperLinkPressed"] is Color)
                {
                    addNewCtrl.ActiveLinkColor = (Color)uiService.Styles["VsColorPanelHyperLinkPressed"];
                }
            }

            // Fill the tree with lots of juicy stuff
            FillTree(initialSelectedItem);

            // Set initial state of the various sub-panels
            addNewPanel.Visible = (showDataSources && dspSvc != null && dspSvc.SupportsAddNewDataSource);
            helpTextPanel.Visible = (showDataSources);

            // Set initial help text in help pane
            UpdateHelpText(null);

            // Invoke the modal dropdown via the editor service (returns once CloseDropDown has been called)
            edSvc.DropDownControl(this);

            // Record any final selection
            DesignBinding finalSelectedItem = selectedItem;
            selectedItem = null;

            // Clean up tree (remove nodes and clear node references)
            EmptyTree();

            // Clean up references
            serviceProvider = null;
            edSvc = null;
            dspSvc = null;
            hostSvc = null;
            context = null;

            // Return final selection to caller
            return finalSelectedItem;
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            if (!DpiHelper.EnableDpiChangedHighDpiImprovements || deviceDpiOld == deviceDpiNew)
            {
                return;
            }

            var factor = (double)deviceDpiNew / deviceDpiOld;
            minimumWidth = DpiHelper.ConvertToGivenDpiPixel(minimumWidth, factor);
            minimumHeight = DpiHelper.ConvertToGivenDpiPixel(minimumHeight, factor);
            Size = new Size(DpiHelper.ConvertToGivenDpiPixel(initialSize.Width, factor), DpiHelper.ConvertToGivenDpiPixel(initialSize.Height, factor));
            SuspendLayout();
            try
            {
                ResetStyles(true);
                addNewPanel.Controls.Clear();
                helpTextPanel.Controls.Clear();
                Controls.Remove(addNewPanel);
                Controls.Remove(helpTextPanel);
                BuildBindingPicker(deviceDpiNew, deviceDpiOld);
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        /// <devdoc>
        ///
        /// If control is open as a dropdown, and a value has been picked
        /// by the user, close the dropdown and end the picking session.
        ///
        /// </devdoc>
        private void CloseDropDown()
        {
            // VSWhidbey#256272. If the object being edited is a BindingSource, then tell its designer to notify
            // the DataSourceProviderService of this change, once the new DataSource/DataMember value has taken
            // effect. This allows the service to generate any adapter components or fill statements needed to
            // set up whatever data source the BindingSource is now bound to. Scenario: Advanced user manually
            // configuring a BindingSource.
            //
            if (context.Instance is BindingSource && hostSvc != null)
            {
                BindingSourceDesigner designer = hostSvc.GetDesigner(context.Instance as IComponent) as BindingSourceDesigner;
                if (designer != null)
                {
                    designer.BindingUpdatedByUser = true;
                }
            }

            // Tell the editor service to close the dropdown
            if (edSvc != null)
            {
                edSvc.CloseDropDown();
            }
        }

        /// <devdoc>
        ///
        /// Resets tree view to empty state.
        ///
        /// </devdoc>
        private void EmptyTree()
        {
            noneNode = null;
            otherNode = null;
            projectNode = null;
            instancesNode = null;
            selectedNode = null;

            treeViewCtrl.Nodes.Clear();
        }

        /// <devdoc>
        ///
        /// Initializes and populates the tree view.
        ///
        /// </devdoc>
        private void FillTree(DesignBinding initialSelectedItem)
        {
            // Set the initial selected item
            selectedItem = initialSelectedItem;

            // Force tree into empty state
            EmptyTree();

            // Create the 'special' nodes
            noneNode = new NoneNode();
            otherNode = new OtherNode();
            projectNode = new ProjectNode(this);
            if (hostSvc != null && hostSvc.RootComponent != null && hostSvc.RootComponent.Site != null)
            {
                instancesNode = new InstancesNode(hostSvc.RootComponent.Site.Name);
            }
            else
            {
                instancesNode = new InstancesNode(string.Empty);
            }

            // Add the 'None' node at the top
            treeViewCtrl.Nodes.Add(noneNode);

            if (showDataSources)
            {
                // Add form-level data sources
                AddFormDataSources();

                // Add project-level data sources
                AddProjectDataSources();

                // Add the remaining 'special' nodes, if they are required
                if (projectNode.Nodes.Count > 0)
                {
                    otherNode.Nodes.Add(projectNode);
                }
                if (instancesNode.Nodes.Count > 0)
                {
                    otherNode.Nodes.Add(instancesNode);
                }
                if (otherNode.Nodes.Count > 0)
                {
                    treeViewCtrl.Nodes.Add(otherNode);
                }
            }
            else
            {
                // Add contents of one specific data source
                AddDataSourceContents(treeViewCtrl.Nodes, rootDataSource, rootDataMember, null);
            }

            // If no node was matched to the selected item, just select the 'None' node
            if (selectedNode == null)
            {
                selectedNode = noneNode;
            }

            // Selected node should be recorded now, so clear the selected item.
            selectedItem = null;

            // Set default width (based on items in tree)
            Width = Math.Max(Width, treeViewCtrl.PreferredWidth + (SystemInformation.VerticalScrollBarWidth * 2));
        }

        /// <devdoc>
        ///
        /// Fills the tree view with top-level data source nodes.
        ///
        /// </devdoc>
        private void AddFormDataSources()
        {
            // VSWhidbey#455147. If the ITypeDescriptorContext does not have a container, grab the container from the 
            // IDesignerHost.
            IContainer container = null;
            if (context != null)
            {
                container = context.Container;
            }

            if (container == null && hostSvc != null)
            {
                container = hostSvc.Container;
            }

            // Bail if we have no container to work with
            if (container == null)
            {
                return;
            }

            container = DesignerUtils.CheckForNestedContainer(container); // ...necessary to support SplitterPanel components

            ComponentCollection components = container.Components;

            // Enumerate the components of the container (eg. the Form)
            foreach (IComponent comp in components)
            {

                // Don't add component to tree if it is the very object who's property the picker
                // is setting (ie. don't let a BindingSource's DataSource property point to itself).
                if (comp == context.Instance)
                {
                    continue;
                }

                // Don't add a DataTable to the tree if its parent DataSet is gonna be in the tree.
                // (...new redundancy-reducing measure for Whidbey)
                if (comp is DataTable && FindComponent(components, ((comp as DataTable).DataSet as IComponent)))
                {
                    continue;
                }

                // Add tree node for this data source
                if (comp is BindingSource)
                {
                    AddDataSource(treeViewCtrl.Nodes, comp, null);
                }
                else
                {
                    AddDataSource(instancesNode.Nodes, comp, null);
                }
            }
        }

        /// <devdoc>
        ///
        /// Adds a tree node representing a data source. Also adds the data source's immediate
        /// child data members, so that the node has the correct +/- state by default.
        ///
        /// </devdoc>
        private void AddDataSource(TreeNodeCollection nodes, IComponent dataSource, string dataMember)
        {
            // Don't add node if not showing data sources
            if (!showDataSources)
            {
                return;
            }

            // Don't add node if this is not a valid bindable data source
            if (!IsBindableDataSource(dataSource))
            {
                return;
            }

            // Get properties of this data source
            string getPropsError = null;
            PropertyDescriptorCollection properties = null;
            try
            {
                properties = GetItemProperties(dataSource, dataMember);
                if (properties == null)
                {
                    return;
                }
            }
            catch (System.ArgumentException e)
            {
                // Exception can occur trying to get list item properties from a data source that's
                // in a badly configured state (eg. its data member refers to a property on its
                // parent data source that's invalid because the parent's metadata has changed).
                getPropsError = e.Message;
            }

            // If data source has no properties, and we are in member-picking mode rather than
            // source-picking mode, just omit the data source altogether - its useless.
            if (showDataMembers && properties.Count == 0)
            {
                return;
            }

            // Create node and add to specified nodes collection
            DataSourceNode dataSourceNode = new DataSourceNode(this, dataSource, dataSource.Site.Name);
            nodes.Add(dataSourceNode);

            // If this node matches the selected item, make it the selected node
            if (selectedItem != null && selectedItem.Equals(dataSource, ""))
            {
                selectedNode = dataSourceNode;
            }

            // Since a data source is added directly to the top level of the tree, rather than
            // revealed by user expansion, we need to fill in its children and grand-children,
            // and mark it as 'filled'.
            if (getPropsError == null)
            {
                // Properties were good: Add them underneath the data source node now
                AddDataSourceContents(dataSourceNode.Nodes, dataSource, dataMember, properties);
                dataSourceNode.SubNodesFilled = true;
            }
            else
            {
                // Properties were bad: Tag the data source with the error message and show the
                // data source as grayed. Error message will appear in help text for that data
                // source, and will prevent user from being able to select the data source.
                dataSourceNode.Error = getPropsError;
                dataSourceNode.ForeColor = SystemColors.GrayText;
            }
        }

        /// <devdoc>
        ///
        /// Adds a set of tree nodes representing the immediate child data members of a data source.
        ///
        /// </devdoc>
        private void AddDataSourceContents(TreeNodeCollection nodes, object dataSource, string dataMember, PropertyDescriptorCollection properties)
        {
            // Don't add nodes if not showing data members (except for BindingSources, we always want to show list members)
            if (!showDataMembers && !(dataSource is BindingSource))
            {
                return;
            }

            // Special case: Data source is a list type (or list item type) rather than a list instance.
            //    Arises when some component's DataSource property is bound to a Type, and the user opens the dropdown for the DataMember property.
            //    We need to create a temporary instance of the correct list type, and use that as our data source for the purpose of determining
            //    data members. Since only BindingSource supports type binding, we bind a temporary BindingSource to the specified type - it will
            //    create an instance of the correct list type for us. Fixes VSWhidbey bugs 302757 and 280708.
            if (dataSource is Type)
            {
                try
                {
                    BindingSource bs = new BindingSource();
                    bs.DataSource = dataSource;
                    dataSource = bs.List;
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
            }

            // Don't add nodes if this is not a valid bindable data source
            if (!IsBindableDataSource(dataSource))
            {
                return;
            }

            // Get properties of this data source (unless already supplied by caller)
            if (properties == null)
            {
                properties = GetItemProperties(dataSource, dataMember);
                if (properties == null)
                {
                    return;
                }
            }

            // Enumerate the properties of the data source
            for (int i = 0; i < properties.Count; ++i)
            {
                PropertyDescriptor property = properties[i];

                // Skip properties that do not represent bindable data members
                if (!IsBindableDataMember(property))
                {
                    continue;
                }

                // Add a data member node for this property
                string dataField = string.IsNullOrEmpty(dataMember) ? property.Name : dataMember + "." + property.Name;
                AddDataMember(nodes, dataSource, dataField, property.Name, IsListMember(property));
            }
        }

        /// <devdoc>
        ///
        /// Adds a tree node representing a data member. Also adds the data member's immediate
        /// child data members, so that the node has the correct +/- state by default.
        ///
        /// </devdoc>
        private void AddDataMember(TreeNodeCollection nodes, object dataSource, string dataMember, string propertyName, bool isList)
        {

            // Special rules for BindingSources...
            //
            // - Standard control bindings access data through a BindingContext, which supports 'dot' notation
            // in the DataMember property (eg. "Customers.Orders.Quantity") to indicate sub-lists (for complex
            // binding) or fields in sub-lists (for simple bindings).
            //
            // - BindingSources so not go through a BindingContext and so do not support the 'dot' notation.
            // Sub-lists are accessed by 'chaining' BindingSources together.
            //
            // So we must prevent the user from being able to create a binding that would result in the
            // DataMember property of a BindingSource containing 'dot' notation. To achieve this, we must
            // flatten certain parts of the tree so that nested sub-members cannot be reached. Specifically...
            //
            // (a) We flatten the tree under every node that represents a BindingSource
            // (b) If the edited object is a BindingSource, we flatten the tree under every data source node
            //
            bool isBindingSourceListMember = isList && dataSource is BindingSource;
            bool pickingFieldMembers = showDataMembers && !selectListMembers;
            bool omitMember = isBindingSourceListMember && pickingFieldMembers;
            bool omitMemberContents = (isBindingSourceListMember && !pickingFieldMembers) || context.Instance is BindingSource;

            // Just omit this member when necessary
            if (omitMember)
            {
                return;
            }

            // Don't add node if its not a list but we only want lists
            if (selectListMembers && !isList)
            {
                return;
            }

            // Create node and add to specified nodes collection
            DataMemberNode dataMemberNode = new DataMemberNode(this, dataSource, dataMember, propertyName, isList);
            nodes.Add(dataMemberNode);

            // If this node matches the selected item, make it the selected node
            if (selectedItem != null && selectedItem.Equals(dataSource, dataMember) && dataMemberNode != null)
            {
                selectedNode = dataMemberNode;
            }

            // Add contents of data member underneath the new node
            if (!omitMemberContents)
            {
                AddDataMemberContents(dataMemberNode);
            }
        }

        /// <devdoc>
        ///
        /// Adds a set of tree nodes representing the immediate child data members of a data member.
        ///
        /// Note: If one of the nodes lies in the path to the selected item, we recursively start
        /// adding its sub-nodes, and so on, until we reach the node for that item. This is needed
        /// to allow that node to be auto-selected and expanded when the dropdown first appears.
        ///
        /// </devdoc>
        private void AddDataMemberContents(TreeNodeCollection nodes, object dataSource, string dataMember, bool isList)
        {
            // Sanity check for correct use of the SubNodesFilled mechanism
            Debug.Assert(nodes.Count == 0, "We only add data member content sub-nodes once.");

            // Don't add nodes for a data member that isn't a list
            if (!isList)
            {
                return;
            }

            // Get properties of this data member
            PropertyDescriptorCollection properties = GetItemProperties(dataSource, dataMember);
            if (properties == null)
            {
                return;
            }

            // Enumerate the properties of the data source
            for (int i = 0; i < properties.Count; ++i)
            {
                PropertyDescriptor property = properties[i];

                // Skip properties that do not represent bindable data members
                if (!IsBindableDataMember(property))
                {
                    continue;
                }

                // Don't add sub-node if sub-member is not a list but we only want lists
                bool isSubList = IsListMember(property);
                if (selectListMembers && !isSubList)
                {
                    continue;
                }

                // Add a data member sub-node for this property
                DataMemberNode dataMemberNode = new DataMemberNode(this, dataSource, dataMember + "." + property.Name, property.Name, isSubList);
                nodes.Add(dataMemberNode);

                // Auto-select support...
                if (selectedItem != null && selectedItem.DataSource == dataMemberNode.DataSource)
                {
                    if (selectedItem.Equals(dataSource, dataMemberNode.DataMember))
                    {
                        // If this node matches the selected item, make it the selected node
                        selectedNode = dataMemberNode;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(selectedItem.DataMember) &&
                            selectedItem.DataMember.IndexOf(dataMemberNode.DataMember) == 0)
                        {
                            // If this node is an ancestor of the selected item, recursively start
                            // filling out sub-member tree (so that node for selected item will
                            // end up being created and selected).
                            AddDataMemberContents(dataMemberNode);
                        }
                    }
                }
            }
        }

        /// <devdoc>
        ///
        /// AddDataMemberContents overload. This version supplies the information
        /// about the data member from an existing data member tree node.
        ///
        /// </devdoc>
        private void AddDataMemberContents(TreeNodeCollection nodes, DataMemberNode dataMemberNode)
        {
            AddDataMemberContents(nodes, dataMemberNode.DataSource, dataMemberNode.DataMember, dataMemberNode.IsList);
        }

        /// <devdoc>
        ///
        /// AddDataMemberContents overload. This version supplies the information
        /// about the data member from an existing data member tree node, and adds
        /// the contents to that node.
        ///
        /// </devdoc>
        private void AddDataMemberContents(DataMemberNode dataMemberNode)
        {
            AddDataMemberContents(dataMemberNode.Nodes, dataMemberNode);
        }

        /// <devdoc>
        ///
        /// Add project level data sources under the special 'Project' tree node
        ///
        /// </devdoc>
        private void AddProjectDataSources()
        {
            if (dspSvc == null)
            {
                return;
            }

            // Get the entire set of project-level data sources
            DataSourceGroupCollection groups = dspSvc.GetDataSources();
            if (groups == null)
            {
                return;
            }

            // If we're gonna be expanding the Project node tree to select a specific
            // project data source or data member, just build the entire tree up front
            bool addMembers = (selectedItem != null && selectedItem.DataSource is DataSourceDescriptor);

            // Create nodes for every project-level data source
            foreach (DataSourceGroup g in groups)
            {
                if (g != null)
                {
                    if (g.IsDefault)
                    {
                        // Data sources in project's default namespace go directly under 'Project' node
                        AddProjectGroupContents(projectNode.Nodes, g);
                    }
                    else
                    {
                        // All other data sources are organized into groups
                        AddProjectGroup(projectNode.Nodes, g, addMembers);
                    }
                }
            }

            // If required, force top-level data sources to fill in their data members now
            if (addMembers)
            {
                projectNode.FillSubNodes();
            }
        }

        /// <devdoc>
        ///
        /// Add node for a given project level data source 'group'.
        ///
        /// </devdoc>
        private void AddProjectGroup(TreeNodeCollection nodes, DataSourceGroup group, bool addMembers)
        {
            // Create the group node, add its data sources, and wire it up
            ProjectGroupNode groupNode = new ProjectGroupNode(this, group.Name, group.Image);
            AddProjectGroupContents(groupNode.Nodes, group);
            nodes.Add(groupNode);

            // If required, force data sources in this group to fill in their data members now
            if (addMembers)
            {
                groupNode.FillSubNodes();
            }
        }

        /// <devdoc>
        ///
        /// Add nodes for data sources in a given project level data source 'group'.
        ///
        /// </devdoc>
        private void AddProjectGroupContents(TreeNodeCollection nodes, DataSourceGroup group)
        {
            DataSourceDescriptorCollection dataSources = group.DataSources;
            if (dataSources == null)
            {
                return;
            }

            foreach (DataSourceDescriptor dsd in dataSources)
            {
                if (dsd != null)
                {
                    AddProjectDataSource(nodes, dsd);
                }
            }
        }

        /// <devdoc>
        ///
        /// Add a node for a single project level data source.
        ///
        /// </devdoc>
        private void AddProjectDataSource(TreeNodeCollection nodes, DataSourceDescriptor dsd)
        {
            // Create and add the project data source tree node
            //

            // vsw 477085: don't add the project data source if it points to a virtual type.
            Type dsType = this.GetType(dsd.TypeName, true, true);
            if (dsType != null && dsType.GetType() != runtimeType)
            {
                return;
            }

            ProjectDataSourceNode projectDataSourceNode = new ProjectDataSourceNode(this, dsd, dsd.Name, dsd.Image);
            nodes.Add(projectDataSourceNode);

            // Auto-select this new node if it corresponds to the current selection (ie. current value)
            //
            if (selectedItem != null && string.IsNullOrEmpty(selectedItem.DataMember))
            {
                // If the current selection is a project-level data source, see if this node has the same name.
                // - The current selection normally refers to a form-level instance of a data source; the only
                //   time the current selection will be a project-level data source is when the user has created
                //   a new one using the 'Add' wizard and we want to show it selected afterwards.
                //
                if (selectedItem.DataSource is DataSourceDescriptor &&
                    string.Equals(dsd.Name, (selectedItem.DataSource as DataSourceDescriptor).Name, StringComparison.OrdinalIgnoreCase))
                {
                    selectedNode = projectDataSourceNode;
                }
                // If the current selection is a simple type, see if this node refers to the same type.
                // - Bindable components can specify an item type as their data source at design time, which
                //   provides the necessary metadata info for the designer. The assumption is that the 'real'
                //   data source instance (that actually returns items of that type) gets supplied at run-time
                //   by customer code.
                // 
                else if (selectedItem.DataSource is Type &&
                    string.Equals(dsd.TypeName, (selectedItem.DataSource as Type).FullName, StringComparison.OrdinalIgnoreCase))
                {
                    selectedNode = projectDataSourceNode;
                }
            }
        }

        /// <devdoc>
        ///
        /// Add the data member nodes for a project level data source.
        ///
        /// </devdoc>
        private void AddProjectDataSourceContents(TreeNodeCollection nodes, DataSourceNode projectDataSourceNode)
        {
            DataSourceDescriptor dsd = (projectDataSourceNode.DataSource as DataSourceDescriptor);
            if (dsd == null)
            {
                return;
            }

            // Get data source type
            Type dataSourceType = this.GetType(dsd.TypeName, false, false);
            if (dataSourceType == null)
            {
                return;
            }

            // If data source type is instancable, create an instance of it, otherwise just use the type itself
            object dataSourceInstance = dataSourceType;
            try
            {
                dataSourceInstance = Activator.CreateInstance(dataSourceType);
            }
            catch (Exception ex)
            {
                if (ClientUtils.IsCriticalException(ex))
                {
                    throw;
                }
            }

            // Is this data source just a "list of lists"? (eg. DataSet is just a set of DataTables)
            bool isListofLists = (dataSourceInstance is IListSource) && (dataSourceInstance as IListSource).ContainsListCollection;

            // Fix for VSWhidbey#223724:
            // When offering choices for the DataSource of a BindingSource, we want to stop the user from being able to pick a table under
            // a data set, since this implies a DS/DM combination, requiring us to create a new 'related' BindingSource. We'd rather the
            // user just picked the data set as the DS, and then set the DM to the table, and avoid creating a redundant BindingSource.
            if (isListofLists && context.Instance is BindingSource)
            {
                return;
            }

            // Determine the properties of the data source
            PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(dataSourceInstance);
            if (properties == null)
            {
                return;
            }

            // Add data members for each property
            foreach (PropertyDescriptor pd in properties)
            {
                // Skip properties that do not represent bindable data members
                if (!IsBindableDataMember(pd))
                {
                    continue;
                }

                // Skip properties that are not browsable
                if (!pd.IsBrowsable)
                {
                    continue;
                }

                // Don't add sub-node if member is not a list but we only want lists
                bool isSubList = IsListMember(pd);
                if (selectListMembers && !isSubList)
                {
                    continue;
                }

                // If data source is a "list of lists", then include list members
                // representing its sub-lists. Otherwise only include field members.
                if (!isListofLists && isSubList)
                {
                    continue;
                }

                // Add data member and also its contents (ie. sub-members)
                AddProjectDataMember(nodes, dsd, pd, dataSourceInstance, isSubList);
            }
        }

        /// <devdoc>
        ///
        /// AddProjectDataSourceContents overload.
        ///
        /// </devdoc>
        private void AddProjectDataSourceContents(DataSourceNode projectDataSourceNode)
        {
            AddProjectDataSourceContents(projectDataSourceNode.Nodes, projectDataSourceNode);
        }

        /// <devdoc>
        ///
        /// Add a node for a single data member of a project level data source.
        ///
        /// </devdoc>
        private void AddProjectDataMember(TreeNodeCollection nodes,
                                          DataSourceDescriptor dsd,
                                          PropertyDescriptor pd,
                                          object dataSourceInstance,
                                          bool isList)
        {
            // vsw 477085: don't add the project data source if it points to a virtual type.
            Type dsType = this.GetType(dsd.TypeName, true, true);
            if (dsType != null && dsType.GetType() != runtimeType)
            {
                return;
            }

            DataMemberNode projectDataMemberNode = new ProjectDataMemberNode(this, dsd, pd.Name, pd.Name, isList);
            nodes.Add(projectDataMemberNode);
            AddProjectDataMemberContents(projectDataMemberNode, dsd, pd, dataSourceInstance);
        }

        /// <devdoc>
        ///
        /// Add nodes for the sub-members of a data member under a project level data source.
        ///
        /// </devdoc>
        private void AddProjectDataMemberContents(TreeNodeCollection nodes,
                                                  DataMemberNode projectDataMemberNode,
                                                  DataSourceDescriptor dsd,
                                                  PropertyDescriptor propDesc,
                                                  object dataSourceInstance)
        {
            // List members under project data sources are only shown to a certain depth,
            // and should already have all been created by the time we get here. So if
            // we're not adding field members, there's nothing more to do.
            if (selectListMembers)
            {
                return;
            }

            // If its not a list member, it can't have any sub-members
            if (!projectDataMemberNode.IsList)
            {
                return;
            }

            // Need data source instance or data source type to determine properties of list member
            if (dataSourceInstance == null)
            {
                return;
            }

            // Determine properties of list member
            PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(dataSourceInstance, new PropertyDescriptor[] { propDesc });
            if (properties == null)
            {
                return;
            }

            // Add field member for each property
            foreach (PropertyDescriptor pd in properties)
            {
                // Skip properties that do not represent bindable data members
                if (!IsBindableDataMember(pd))
                {
                    continue;
                }

                // Skip properties that are not browsable
                if (!pd.IsBrowsable)
                {
                    continue;
                }

                // We only add field members (no nesting of list members under project data sources)
                bool isSubList = IsListMember(pd);
                if (isSubList)
                {
                    continue;
                }

                // Add the field member (without contents)
                AddProjectDataMember(nodes, dsd, pd, dataSourceInstance, isSubList);
            }
        }

        /// <devdoc>
        ///
        /// AddProjectDataMemberContents overload.
        ///
        /// </devdoc>
        private void AddProjectDataMemberContents(DataMemberNode projectDataMemberNode,
                                                  DataSourceDescriptor dsd,
                                                  PropertyDescriptor pd,
                                                  object dataSourceInstance)
        {
            AddProjectDataMemberContents(projectDataMemberNode.Nodes, projectDataMemberNode, dsd, pd, dataSourceInstance);
        }

        /// <devdoc>
        ///
        /// Puts a new BindingSource on the form, with the specified DataSource and DataMember values.
        ///
        /// </devdoc>
        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private BindingSource CreateNewBindingSource(object dataSource, string dataMember)
        {
            if (hostSvc == null || dspSvc == null)
            {
                return null;
            }

            // Create the BindingSource
            BindingSource bs = new BindingSource();
            try
            {
                bs.DataSource = dataSource;
                bs.DataMember = dataMember;
            }
            catch (Exception ex)
            {
                IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                return null;
            }

            // Give it a name
            string bindingSourceName = GetBindingSourceNamePrefix(dataSource, dataMember);
            // If we have a service provider then use it to get the camel notation from ToolStripDesigner.NameFromText
            if (serviceProvider != null)
            {
                bindingSourceName = ToolStripDesigner.NameFromText(bindingSourceName, bs.GetType(), serviceProvider);
            }
            else
            {
                bindingSourceName = bindingSourceName + bs.GetType().Name;
            }

            // Make sure the name is unique.
            string uniqueSiteName = DesignerUtils.GetUniqueSiteName(hostSvc, bindingSourceName);

            DesignerTransaction trans = hostSvc.CreateTransaction(SR.DesignerBatchCreateTool, uniqueSiteName);

            try
            {
                // Put it on the form
                try
                {
                    hostSvc.Container.Add(bs, uniqueSiteName);
                }

                catch (System.InvalidOperationException ex)
                {
                    if (trans != null)
                    {
                        trans.Cancel();
                    }
                    IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
                    DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                    return null;
                }

                catch (CheckoutException ex)
                {
                    if (trans != null)
                    {
                        trans.Cancel();
                    }
                    IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
                    DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                    return null;
                }

                // Notify the provider service that a new form object is referencing this project-level data source
                dspSvc.NotifyDataSourceComponentAdded(bs);
                if (trans != null)
                {
                    trans.Commit();
                    trans = null;
                }
            }

            finally
            {
                if (trans != null)
                {
                    trans.Cancel();
                }
            }

            return bs;
        }

        /// <devdoc>
        ///
        /// CreateNewBindingSource overload, for project-level data sources.
        ///
        /// </devdoc>
        private BindingSource CreateNewBindingSource(DataSourceDescriptor dataSourceDescriptor, string dataMember)
        {
            if (hostSvc == null || dspSvc == null)
            {
                return null;
            }

            // Find or create a form-level instance of this project-level data source
            object dataSource = GetProjectDataSourceInstance(dataSourceDescriptor);
            if (dataSource == null)
            {
                return null;
            }

            // Create a BindingSource that points to the form-level instance
            return CreateNewBindingSource(dataSource, dataMember);
        }

        /// <devdoc>
        ///
        /// Chooses the best name prefix for a new BindingSource, based on the
        /// data source and data member that the binding source is bound to.
        ///
        /// </devdoc>
        private string GetBindingSourceNamePrefix(object dataSource, string dataMember)
        {
            // Always use the data member string, if one is available
            if (!string.IsNullOrEmpty(dataMember))
            {
                return dataMember;
            }

            // Data source should never be null
            if (dataSource == null)
            {
                return "";
            }

            // If data source is a type, use the name of the type
            Type type = (dataSource as Type);
            if (type != null)
            {
                return type.Name;
            }

            // If data source is a form component, use its sited name
            IComponent comp = (dataSource as IComponent);
            if (comp != null)
            {
                ISite site = comp.Site;

                if (site != null && !string.IsNullOrEmpty(site.Name))
                {
                    return site.Name;
                }
            }

            // Otherwise just use the type name of the data source
            return dataSource.GetType().Name;
        }

        /// <devdoc>
        ///
        /// Get the Type with the specified name. If TypeResolutionService is available,
        /// use that in preference to using the Type class (since this service can more
        /// reliably instantiate project level types).
        ///
        /// </devdoc>
        private Type GetType(string name, bool throwOnError, bool ignoreCase)
        {
            if (typeSvc != null)
            {
                return typeSvc.GetType(name, throwOnError, ignoreCase);
            }
            else
            {
                return Type.GetType(name, throwOnError, ignoreCase);
            }
        }

        /// <devdoc>
        ///
        /// Finds the form-level instance of a project-level data source. Looks for form components
        /// who's type matches that of the project-level data source. If none are found, ask the
        /// provider service to add one for us.
        ///
        /// Note: If the project-level data source is not instance-able, just return its type as
        /// the data source to bind to ("simple type binding" case).
        ///
        /// </devdoc>
        private object GetProjectDataSourceInstance(DataSourceDescriptor dataSourceDescriptor)
        {
            Type dsType = this.GetType(dataSourceDescriptor.TypeName, true, true);

            // Not an instance-able type, so just return the type
            if (!dataSourceDescriptor.IsDesignable)
            {
                return dsType;
            }

            // Enumerate the components of the container (eg. the Form)
            foreach (IComponent comp in hostSvc.Container.Components)
            {

                // Return the first matching component we find
                if (dsType.Equals(comp.GetType()))
                {
                    return comp;
                }
            }

            // No existing instances found, so ask provider service to create a new one
            try
            {
                return dspSvc.AddDataSourceInstance(hostSvc, dataSourceDescriptor);
            }
            catch (System.InvalidOperationException ex)
            {
                IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                return null;
            }
            catch (CheckoutException ex)
            {
                IUIService uiService = serviceProvider.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                return null;
            }
        }

        /// <devdoc>
        ///
        /// See if a component collection contains a given component (simple linear search).
        ///
        /// </devdoc>
        private bool FindComponent(ComponentCollection components, IComponent targetComponent)
        {
            foreach (IComponent c in components)
            {
                if (c == targetComponent)
                {
                    return true;
                }
            }
            return false;
        }

        /// <devdoc>
        ///
        /// See if the given object is a valid bindable data source.
        ///
        /// </devdoc>
        private bool IsBindableDataSource(object dataSource)
        {
            // Check for expected interfaces (require at least one)
            if (!(dataSource is IListSource || dataSource is IList || dataSource is Array))
            {
                return false;
            }

            // Check for [ListBindable(false)] attribute
            ListBindableAttribute listBindable = (ListBindableAttribute)TypeDescriptor.GetAttributes(dataSource)[typeof(ListBindableAttribute)];
            if (listBindable != null && !listBindable.ListBindable)
            {
                return false;
            }

            return true;
        }

        /// <devdoc>
        ///
        /// See if the given property represents a bindable data member.
        ///
        /// [IainHe] Oddly, we always check the [ListBindable] attribute on the property. This makes sense for
        /// list members, but seems pretty meaningless for field members. But that's what we've always done,
        /// so let's continue to do it.
        ///
        /// </devdoc>
        private bool IsBindableDataMember(PropertyDescriptor property)
        {
            // Special case: We want byte arrays to appear as bindable field members.
            if (typeof(byte[]).IsAssignableFrom(property.PropertyType))
            {
                return true;
            }

            // Check for [ListBindable(false)] attribute
            ListBindableAttribute listBindable = (ListBindableAttribute)property.Attributes[typeof(ListBindableAttribute)];
            if (listBindable != null && !listBindable.ListBindable)
            {
                return false;
            }

            return true;
        }

        /// <devdoc>
        ///
        /// See if the given property represents a list member rather than a field member.
        ///
        /// </devdoc>
        private bool IsListMember(PropertyDescriptor property)
        {
            // Special case: We want byte arrays to appear as bindable field members
            if (typeof(byte[]).IsAssignableFrom(property.PropertyType))
            {
                return false;
            }

            // If you assign an IList to it, then its a list member
            if (typeof(IList).IsAssignableFrom(property.PropertyType))
            {
                return true;
            }

            return false;
        }

        /// <devdoc>
        ///
        /// For a data source, or a data member that's a list, this method returns a
        /// description of the properties possessed by items in the underlying list.
        ///
        /// </devdoc>
        private PropertyDescriptorCollection GetItemProperties(object dataSource, string dataMember)
        {
            CurrencyManager listManager = (CurrencyManager)bindingContext[dataSource, dataMember];
            return (listManager == null) ? null : listManager.GetItemProperties();
        }

        /// <devdoc>
        ///
        /// Update roll-over help text as user mouses from tree node to tree node.
        ///
        /// Basic rules...
        /// - If the mouse is over a node, the node usually supplies its own help text.
        /// - Else if there is an existing selection, report that as the current binding.
        /// - Else just display some general picker help text.
        ///
        /// The goal of the general text is to provide help in 'initial use and set up' scenarios,
        /// to guide the user through the set of steps needed to create their first binding. The
        /// general text cases below get progressively further "back in time" as you go down.
        ///
        /// Note: All help text strings are geared specifically towards the context of a picker
        /// that is showing data sources. So when the picker is just showing data members (scoped
        /// to a specific data source), the help text area will be hidden.
        ///
        /// </devdoc>
        private void UpdateHelpText(BindingPickerNode mouseNode)
        {
            if (instancesNode == null)
            {
                return;
            }

            // See if node under mouse wants to supply its own help text
            string mouseNodeHelpText = (mouseNode == null) ? null : mouseNode.HelpText;
            string mouseNodeErrorText = (mouseNode == null) ? null : mouseNode.Error;

            // Set the colors...
            if (mouseNodeHelpText != null || mouseNodeErrorText != null)
            {
                helpTextCtrl.BackColor = SystemColors.Info;
                helpTextCtrl.ForeColor = SystemColors.InfoText;
            }
            else
            {
                helpTextCtrl.BackColor = SystemColors.Window;
                helpTextCtrl.ForeColor = SystemColors.WindowText;
            }

            // Set the text...
            if (mouseNodeErrorText != null)
            {
                // This node has an ERROR associated with it
                helpTextCtrl.Text = mouseNodeErrorText;
            }
            else if (mouseNodeHelpText != null)
            {
                // Node specific help text
                helpTextCtrl.Text = mouseNodeHelpText;
            }
            else if (selectedNode != null && selectedNode != noneNode)
            {
                // Already bound to something (user has experience)
                helpTextCtrl.Text = string.Format(CultureInfo.CurrentCulture, SR.DesignBindingPickerHelpGenCurrentBinding, selectedNode.Text);
            }
            else if (!showDataSources)
            {
                // No data sources, so this is just a simple data member pick list
                helpTextCtrl.Text = (treeViewCtrl.Nodes.Count > 1) ? SR.DesignBindingPickerHelpGenPickMember : "";
            }
            else if (treeViewCtrl.Nodes.Count > 1 && treeViewCtrl.Nodes[1] is DataSourceNode)
            {
                // BindingSources exist - tell user to pick one
                helpTextCtrl.Text = SR.DesignBindingPickerHelpGenPickBindSrc;
            }
            else if (instancesNode.Nodes.Count > 0 || projectNode.Nodes.Count > 0)
            {
                // Data sources exist - tell user to pick one
                helpTextCtrl.Text = SR.DesignBindingPickerHelpGenPickDataSrc;
            }
            else if (addNewPanel.Visible)
            {
                // No data sources - tell user how to create one
                helpTextCtrl.Text = SR.DesignBindingPickerHelpGenAddDataSrc;
            }
            else
            {
                // No data sources, and no way to create one!
                helpTextCtrl.Text = "";
            }
        }

        /// <devdoc>
        ///
        /// Always pass focus down to tree control (so that selection is always visible)
        ///
        /// </devdoc>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            treeViewCtrl.Focus();
        }

        /// <devdoc>
        ///
        /// Updates the state of the control when shown or hidden. When shown, we make sure
        /// the current selection is visible, and start listening to node expand events (so
        /// we can fill the tree as the user drills down).
        ///
        /// </devdoc>
        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            if (Visible)
            {
                ShowSelectedNode();
            }
        }

        /// <devdoc>
        ///
        /// Enforces the control's minimum width and height.
        ///
        /// </devdoc>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width)
            {
                width = Math.Max(width, minimumWidth);
            }
            if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {
                height = Math.Max(height, minimumHeight);
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <devdoc>
        ///
        /// Handle click on the "Add Project Data Source" link label.
        ///
        /// </devdoc>
        private void addNewCtrl_Click(object sender, LinkLabelLinkClickedEventArgs e)
        {
            // No provider service, or provider won't allow creation of new data sources right now
            if (dspSvc == null || !dspSvc.SupportsAddNewDataSource)
            {
                return;
            }

            // Invoke the 'Add' wizard
            DataSourceGroup newProjectDataSources = dspSvc.InvokeAddNewDataSource(this, FormStartPosition.CenterScreen);

            // Wizard was cancelled or did not create any new data sources
            if (newProjectDataSources == null || newProjectDataSources.DataSources.Count == 0)
            {
                return;
            }

            // Rule: If multiple data sources were created, just use the first one.
            DataSourceDescriptor newProjectDataSource = newProjectDataSources.DataSources[0];

            // Update tree to include the new data source (and select it)
            FillTree(new DesignBinding(newProjectDataSource, ""));

            // If we weren't able to select the node representing the new data
            // source, then something has gone horribly wrong - bail out now!
            if (selectedNode == null)
            {
                Debug.Fail("Failed to select new project-level data source in DesignBindingPicker tree.");
                return;
            }

            // Count the number of data members under this data source
            int dataMemberCount = selectedNode.Nodes.Count;

            //
            // Decide what to do with the new data source...
            //

            if (context.Instance is BindingSource)
            {
                // Bindable object is a BindingSource - no choice, must bind to data source
                treeViewCtrl.SetSelectedItem(selectedNode);
            }
            if (dataMemberCount == 0 || context.Instance is BindingSource)
            {
                // Zero data members - bind to the data source
                treeViewCtrl.SetSelectedItem(selectedNode);
            }
            else if (dataMemberCount == 1)
            {
                // One data member - bind to that data member
                treeViewCtrl.SetSelectedItem(selectedNode.Nodes[0]);
            }
            else
            {
                // Multiple data members - stay open and show them all
                ShowSelectedNode();
                selectedNode.Expand();
                selectedNode = null;
                UpdateHelpText(null);
            }
        }

        /// <devdoc>
        ///
        /// Update roll-over help text as user mouses from tree node to tree node.
        ///
        /// </devdoc>
        private void treeViewCtrl_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the tree node under the mouse
            Point pt = new Point(e.X, e.Y);
            TreeNode node = treeViewCtrl.GetNodeAt(pt);

            // Make sure point is over the node label, since GetNodeAt() will return
            // a node even when the mouse is way off to the far right of that node.
            if (node != null && !node.Bounds.Contains(pt))
            {
                node = null;
            }

            // Update the help text
            UpdateHelpText(node as BindingPickerNode);
        }

        /// <devdoc>
        ///
        /// Reset roll-over help text if user mouses away from the tree view.
        ///
        /// </devdoc>
        private void treeViewCtrl_MouseLeave(object sender, EventArgs e)
        {
            UpdateHelpText(null);
        }

        /// <devdoc>
        ///
        /// When user expands a tree node to reveal its sub-nodes, we fill in the contents
        /// of those sub-nodes, so that their +/- states are correct. In other words, we
        /// fill the tree "one level ahead" of what the user has revealed.
        ///
        /// </devdoc>
        private void treeViewCtrl_AfterExpand(object sender, TreeViewEventArgs tvcevent)
        {
            // Ignore expansion caused by something other than direct user action (eg. auto-selection)
            if (inSelectNode || !Visible)
            {
                return;
            }

            // Let the node do whatever it wants
            (tvcevent.Node as BindingPickerNode).OnExpand();
        }

        /// <devdoc>
        ///
        /// Ensure the initial selection is visible (ie. select the corresponding
        /// tree node, which also causes auto-expand of all ancestor nodes).
        ///
        /// Note: Posting has to be used here because the tree view control won't
        /// let us select nodes until all the underlying Win32 HTREEITEMs have
        /// been created.
        ///
        /// </devdoc>
        private void ShowSelectedNode()
        {
            PostSelectTreeNode(selectedNode);
        }

        /// <devdoc>
        ///
        /// Selects a given node in the tree view. Because the tree view will auto-expand any
        /// ancestor nodes, in order to make the selected node visible, we have to temporarily
        /// turn off a couple of things until selection is finished: (a) painting; (b) processing
        /// of 'node expand' events.
        ///
        /// </devdoc>
        private void SelectTreeNode(TreeNode node)
        {
            if (inSelectNode)
            {
                return;
            }

            try
            {
                inSelectNode = true;
                treeViewCtrl.BeginUpdate();
                treeViewCtrl.SelectedNode = node;
                treeViewCtrl.EndUpdate();
            }
            finally
            {
                inSelectNode = false;
            }
        }

        //
        // The following methods exist to support posted (ie. delayed) selection of tree nodes...
        //

        delegate void PostSelectTreeNodeDelegate(TreeNode node);

        private void PostSelectTreeNodeCallback(TreeNode node)
        {
            SelectTreeNode(null);
            SelectTreeNode(node);
        }

        private void PostSelectTreeNode(TreeNode node)
        {
            if (node != null && IsHandleCreated)
            {
                BeginInvoke(new PostSelectTreeNodeDelegate(PostSelectTreeNodeCallback), new object[] { node });
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.HelpTextLabel"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Label control that renders its text with both word wrapping, end ellipsis and partial line clipping.
        ///
        /// </devdoc>
        internal class HelpTextLabel : Label
        {

            protected override void OnPaint(PaintEventArgs e)
            {
                TextFormatFlags formatFlags = TextFormatFlags.WordBreak | TextFormatFlags.EndEllipsis | TextFormatFlags.TextBoxControl;
                Rectangle rect = new Rectangle(ClientRectangle.Location, ClientRectangle.Size);
                rect.Inflate(-2, -2);
                TextRenderer.DrawText(e.Graphics, Text, Font, rect, ForeColor, formatFlags);
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.BindingPickerLink"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Link label used by the DesignBindingPicker to display links.
        ///
        /// </devdoc>
        internal class BindingPickerLink : LinkLabel
        {

            /// <devdoc>
            ///
            /// Allow "Return" as an input key (so it allows the link to fire, instead of closing the parent dropdown).
            ///
            /// </devdoc>
            protected override bool IsInputKey(Keys key)
            {
                return (key == Keys.Return) || base.IsInputKey(key);
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.BindingPickerTree"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Tree view used by the DesignBindingPicker to display data sources and data members.
        ///
        /// </devdoc>
        internal class BindingPickerTree : TreeView
        {

            internal BindingPickerTree()
            {

                Bitmap images = new Bitmap(typeof(DesignBindingPicker), "DataPickerImages.bmp");
                ImageList imageList = new ImageList();

                imageList.TransparentColor = Color.Magenta;
                imageList.Images.AddStrip(images);

                if (DpiHelper.IsScalingRequired)
                {

                    images.MakeTransparent(Color.Magenta);

                    ImageList scaledImageList = new ImageList();
                    Size scaledSize = DpiHelper.LogicalToDeviceUnits(imageList.ImageSize);

                    foreach (Image image in imageList.Images)
                    {
                        Bitmap scaledImage = DpiHelper.EnableDpiChangedHighDpiImprovements ? DpiHelper.CreateResizedBitmap((Bitmap)image, scaledSize, true) : DpiHelper.CreateResizedBitmap((Bitmap)image, scaledSize);
                        scaledImageList.Images.Add(scaledImage);
                    }

                    imageList.Dispose();
                    scaledImageList.ImageSize = scaledSize;

                    ImageList = scaledImageList;
                }
                else
                {
                    ImageList = imageList;
                }

                ImageList.ColorDepth = ColorDepth.Depth24Bit;
            }

            internal int PreferredWidth
            {
                get
                {
                    return GetMaxItemWidth(Nodes);
                }
            }

            protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
            {
                base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
                if (!DpiHelper.EnableDpiChangedHighDpiImprovements)
                {
                    return;
                }

                var factor = (double)deviceDpiNew / deviceDpiOld;
                ImageList scaledImageList = new ImageList();
                scaledImageList.TransparentColor = Color.Magenta;
                Size scaledSize = new Size((int)(ImageList.ImageSize.Width * factor), (int)(ImageList.ImageSize.Height * factor));
                Bitmap imageBits = new Bitmap(typeof(DesignBindingPicker), "DataPickerImages.bmp");
                SuspendLayout();
                try
                {
                    var tempImageList = new ImageList();
                    tempImageList.Images.AddStrip(imageBits);
                    foreach (Image image in tempImageList.Images)
                    {
                        Bitmap scaledImage = DpiHelper.CreateResizedBitmap((Bitmap)image, scaledSize, true);
                        scaledImageList.Images.Add(scaledImage);
                    }

                    ImageList.Dispose();
                    scaledImageList.ImageSize = scaledSize;
                    ImageList = scaledImageList;
                }
                finally
                {
                    ResumeLayout();
                }

            }

            /// <devdoc>
            ///
            /// Calculate the maximum width of the nodes in the collection recursively.
            /// Only walks the existing set of expanded visible nodes. Does NOT expand
            /// unexpanded nodes, since tree may contain endless cyclic relationships.
            ///
            /// </devdoc>
            private int GetMaxItemWidth(TreeNodeCollection nodes)
            {
                int maxWidth = 0;

                foreach (TreeNode node in nodes)
                {
                    Rectangle bounds = node.Bounds;
                    int w = bounds.Left + bounds.Width;
                    maxWidth = Math.Max(w, maxWidth);

                    if (node.IsExpanded)
                        maxWidth = Math.Max(maxWidth, GetMaxItemWidth(node.Nodes));
                }

                return maxWidth;
            }

            /// <devdoc>
            ///
            /// Processes user selection of tree node. If node is selectable, notifies
            /// node of selection, retrieves data source and data member info for the
            /// caller, and closes the dropdown.
            ///
            /// </devdoc>
            public void SetSelectedItem(TreeNode node)
            {
                DesignBindingPicker picker = Parent as DesignBindingPicker;
                if (picker == null)
                {
                    return;
                }

                BindingPickerNode pickerNode = node as BindingPickerNode;
                picker.selectedItem = (pickerNode.CanSelect && pickerNode.Error == null) ? pickerNode.OnSelect() : null;

                if (picker.selectedItem != null)
                {
                    picker.CloseDropDown();
                }
            }

            /// <devdoc>
            ///
            /// Process a mouse click on a node.
            ///
            /// NOTE: Overriding OnAfterSelect() to handle selection changes is not sufficient because of a ComCtl32 quirk:
            /// Clicking on the *current* selection does not trigger a selection change notification. And we need to support
            /// re-selection of the current selection in certain scenarios. So instead of using OnAfterSelect(), we use
            /// OnNodeMouseClick(), and use hit-testing to see whether the node's image or label were clicked.
            ///
            /// </devdoc>
            protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
            {
                TreeViewHitTestInfo tvhti = HitTest(new Point(e.X, e.Y));
                if (tvhti.Node == e.Node &&
                    (tvhti.Location == TreeViewHitTestLocations.Image ||
                     tvhti.Location == TreeViewHitTestLocations.Label))
                {
                    SetSelectedItem(e.Node);
                }

                base.OnNodeMouseClick(e);
            }

            /// <devdoc>
            ///
            /// Treat "Return" as a mouse click select of a node.
            ///
            /// </devdoc>
            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);

                if (e.KeyData == Keys.Return && SelectedNode != null)
                {
                    SetSelectedItem(SelectedNode);
                }
            }

            /// <devdoc>
            ///
            /// Allow "Return" as an input key.
            ///
            /// </devdoc>
            protected override bool IsInputKey(Keys key)
            {
                return (key == Keys.Return) || base.IsInputKey(key);
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.BindingPickerNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Base class for all nodes in the tree view.
        ///
        /// </devdoc>
        internal class BindingPickerNode : TreeNode
        {

            private string error = null;

            private bool subNodesFilled = false;

            protected DesignBindingPicker picker = null;

            public BindingPickerNode(DesignBindingPicker picker, string nodeName) : base(nodeName)
            {
                this.picker = picker;
            }

            public BindingPickerNode(DesignBindingPicker picker, string nodeName, BindingImage index) : base(nodeName)
            {
                this.picker = picker;
                BindingImageIndex = (int)index;
            }

            /// <devdoc>
            ///
            /// Given a data source, return the corresponding BindingImageIndex.
            ///
            /// </devdoc>
            public static BindingImage BindingImageIndexForDataSource(object dataSource)
            {
                if (dataSource is BindingSource)
                {
                    return BindingImage.BindingSource;
                }

                IListSource ils = dataSource as IListSource;
                if (ils != null)
                {
                    if (ils.ContainsListCollection)
                    {
                        return BindingImage.DataSource;
                    }
                    else
                    {
                        return BindingImage.ListMember;
                    }
                }
                else if (dataSource is IList)
                {
                    return BindingImage.ListMember;
                }
                else
                {
                    return BindingImage.FieldMember;
                }
            }

            // Called when a node is expanded by the user
            public virtual void OnExpand()
            {
                FillSubNodes();
            }

            // Forces the node's children to populate themeselves, so that their +/- states are correct
            // when parent node is first expanded. If children have already been filled, does nothing.
            public virtual void FillSubNodes()
            {
                // Sub-nodes already filled - nothing more to do here
                if (SubNodesFilled)
                {
                    return;
                }

                // Fill in the contents of each sub-node
                foreach (BindingPickerNode node in Nodes)
                {
                    node.Fill();
                }

                // Mark the expanded node as filled
                SubNodesFilled = true;
            }

            // Fills node with its child nodes (usually called by parent node's OnExpand method)
            public virtual void Fill()
            {
            }

            // Called when node is selected by user. Should only be called if node has
            // returned 'true' for CanSelect. Node returns a DesignBinding representing
            // the data source + data member that it represents.
            public virtual DesignBinding OnSelect()
            {
                return null;
            }

            // Determines whether selecting this node will close the dropdown
            public virtual bool CanSelect
            {
                get
                {
                    return false;
                }
            }

            // Error message associated with this node
            public virtual string Error
            {
                get
                {
                    return error;
                }
                set
                {
                    error = value;
                }
            }

            // Mouse-over help text for this node
            public virtual string HelpText
            {
                get
                {
                    return null;
                }
            }

            // Indexes of images in the tree view's image list
            public enum BindingImage
            {
                None = 0,
                Other = 1,
                Project = 2,
                Instances = 3,
                BindingSource = 4,
                ListMember = 5,
                FieldMember = 6,
                DataSource = 7,
            }

            // Sets both the selected and unselected images to the same thing
            public int BindingImageIndex
            {
                set
                {
                    ImageIndex = value;
                    SelectedImageIndex = value;
                }
            }

            // Let's you assign a custom image to a specific tree node.
            // The image is automatically added to the tree view's image list.
            public Image CustomBindingImage
            {
                set
                {
                    try
                    {
                        ImageList.ImageCollection images = picker.treeViewCtrl.ImageList.Images;
                        images.Add(value, Color.Transparent);
                        BindingImageIndex = images.Count - 1;
                    }
                    catch (Exception)
                    {
                        Debug.Assert(false, "DesignBindingPicker failed to add custom image to image list.");
                    }
                }
            }

            // Indicates whether this node's child nodes have had their children
            // added (we populate the tree one level deeper than the user can see,
            // so that the +/- states are correct.
            public bool SubNodesFilled
            {
                get
                {
                    return subNodesFilled;
                }
                set
                {
                    Debug.Assert(!subNodesFilled && value, "we can only set this bit to true once");
                    subNodesFilled = true;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.DataSourceNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing a data source.
        ///
        /// </devdoc>
        internal class DataSourceNode : BindingPickerNode
        {

            private object dataSource;

            public DataSourceNode(DesignBindingPicker picker, object dataSource, string nodeName) : base(picker, nodeName)
            {
                this.dataSource = dataSource;
                BindingImageIndex = (int)BindingImageIndexForDataSource(dataSource);
            }

            public object DataSource
            {
                get
                {
                    return dataSource;
                }
            }

            public override DesignBinding OnSelect()
            {
                return new DesignBinding(DataSource, "");
            }

            public override bool CanSelect
            {
                get
                {
                    // If data members are included in tree, only
                    // they can be selected, not data sources.
                    return !picker.showDataMembers;
                }
            }

            // For any data source or data member derived node, we pick the mouse-over text
            // from one of 12 possible string resources, based on the node's particular
            // combination of data source type (BindingSource, Project data source, or Form
            // list instance), node type (data source, list member or field member) and current
            // selectability (true or false).
            public override string HelpText
            {
                get
                {
                    string dsType, nodeType, resName, resValue;

                    if (DataSource is DataSourceDescriptor)
                        dsType = "Project";
                    else if (DataSource is BindingSource)
                        dsType = "BindSrc";
                    else
                        dsType = "FormInst";

                    if (!(this is DataMemberNode))
                        nodeType = "DS";
                    else if ((this as DataMemberNode).IsList)
                        nodeType = "LM";
                    else
                        nodeType = "DM";

                    try
                    {
                        resName = string.Format(CultureInfo.CurrentCulture, "DesignBindingPickerHelpNode{0}{1}{2}", dsType, nodeType, (CanSelect ? "1" : "0"));
                        resValue = resName;
                    }
                    catch
                    {
                        resValue = "";
                    }

                    return resValue;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.DataMemberNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing a data member.
        ///
        /// Note: Inherits from DataSourceNode, so be careful when trying to distinguish between these two types.
        ///
        /// </devdoc>
        internal class DataMemberNode : DataSourceNode
        {

            private bool isList;
            private string dataMember;

            public DataMemberNode(DesignBindingPicker picker,
                                  object dataSource,
                                  string dataMember,
                                  string dataField,
                                  bool isList) : base(picker, dataSource, dataField)
            {
                this.dataMember = dataMember;
                this.isList = isList;
                BindingImageIndex = (int)(isList ? BindingImage.ListMember : BindingImage.FieldMember);
            }

            public string DataMember
            {
                get
                {
                    return dataMember;
                }
            }

            // List member or field member?
            public bool IsList
            {
                get
                {
                    return isList;
                }
            }

            public override void Fill()
            {
                picker.AddDataMemberContents(this);
            }

            public override DesignBinding OnSelect()
            {
                if (picker.showDataMembers)
                {
                    // Data member picking mode: Return data member info
                    return new DesignBinding(DataSource, DataMember);
                }
                else
                {
                    // Data source picking mode: Return data member wrapped in a BindingSource
                    BindingSource newBindingSource = picker.CreateNewBindingSource(DataSource, DataMember);
                    return (newBindingSource == null) ? null : new DesignBinding(newBindingSource, "");
                }
            }

            public override bool CanSelect
            {
                get
                {
                    // Only pick list members in 'list mode', field members in 'field mode'
                    return (picker.selectListMembers == IsList);
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.NoneNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing the "None" choice.
        ///
        /// </devdoc>
        internal class NoneNode : BindingPickerNode
        {

            public NoneNode() : base(null, SR.DesignBindingPickerNodeNone, BindingImage.None)
            {
            }

            public override DesignBinding OnSelect()
            {
                return DesignBinding.Null;
            }

            public override bool CanSelect
            {
                get
                {
                    return true;
                }
            }

            public override string HelpText
            {
                get
                {
                    return SR.DesignBindingPickerHelpNodeNone;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.OtherNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing the "Other Data Sources" branch.
        ///
        /// </devdoc>
        internal class OtherNode : BindingPickerNode
        {

            public OtherNode() : base(null, SR.DesignBindingPickerNodeOther, BindingImage.Other)
            {
            }

            public override string HelpText
            {
                get
                {
                    return SR.DesignBindingPickerHelpNodeOther;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.InstancesNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing the "Form List Instances" branch.
        ///
        /// </devdoc>
        internal class InstancesNode : BindingPickerNode
        {

            public InstancesNode(string rootComponentName) : base(null, string.Format(CultureInfo.CurrentCulture, SR.DesignBindingPickerNodeInstances, rootComponentName), BindingImage.Instances)
            {
            }

            public override string HelpText
            {
                get
                {
                    return SR.DesignBindingPickerHelpNodeInstances;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.ProjectNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing the "Project Data Sources" branch.
        ///
        /// </devdoc>
        internal class ProjectNode : BindingPickerNode
        {

            public ProjectNode(DesignBindingPicker picker) : base(picker, SR.DesignBindingPickerNodeProject, BindingImage.Project)
            {
            }

            public override string HelpText
            {
                get
                {
                    return SR.DesignBindingPickerHelpNodeProject;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.ProjectGroupNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing a group of data sources under the "Project Data Sources" branch.
        ///
        /// </devdoc>
        internal class ProjectGroupNode : BindingPickerNode
        {

            public ProjectGroupNode(DesignBindingPicker picker, string nodeName, Image image) : base(picker, nodeName, BindingImage.Project)
            {
                if (image != null)
                {
                    CustomBindingImage = image;
                }
            }

            public override string HelpText
            {
                get
                {
                    return SR.DesignBindingPickerHelpNodeProjectGroup;
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.ProjectDataSourceNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing a project level data source.
        ///
        /// Note: dataSource is always a DataSourceDescriptor.
        ///
        /// </devdoc>
        internal class ProjectDataSourceNode : DataSourceNode
        {

            public ProjectDataSourceNode(DesignBindingPicker picker, object dataSource, string nodeName, Image image) : base(picker, dataSource, nodeName)
            {
                if (image != null)
                {
                    CustomBindingImage = image;
                }
            }

            public override void OnExpand()
            {
                // Do nothing (not even call base class). Project data source
                // nodes are full populated when added to the tree.
            }

            public override void Fill()
            {
                picker.AddProjectDataSourceContents(this);
            }

            public override DesignBinding OnSelect()
            {
                // When user selects a project-level data source (in data source picking mode),
                // we (a) create a form-level instance of the data source, (b) create a new
                // BindingSource that points to that instance, and (c) return the new BindingSource
                // as the data source to bind to.
                //
                // EXCEPTION: If we are setting the DataSource property of a BindingSource, then
                // there is no need to create an intermediate BindingSource. Just return the
                // true data source instance for the BindingSource to bind to.

                DataSourceDescriptor dataSourceDescriptor = (DataSourceDescriptor)DataSource;

                if (picker.context.Instance is BindingSource)
                {
                    object newDataSource = picker.GetProjectDataSourceInstance(dataSourceDescriptor);
                    if (newDataSource != null)
                    {
                        return new DesignBinding(newDataSource, "");
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    BindingSource newBindingSource = picker.CreateNewBindingSource(dataSourceDescriptor, "");
                    return (newBindingSource == null) ? null : new DesignBinding(newBindingSource, "");
                }
            }
        }

        /// <include file='doc\DesignBindingPicker.uex' path='docs/doc[@for="DesignBindingPicker.ProjectDataMemberNode"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///
        /// Node representing a data member under a project level data source.
        ///
        /// Note: dataSource is always a DataSourceDescriptor.
        ///
        /// </devdoc>
        internal class ProjectDataMemberNode : DataMemberNode
        {

            public ProjectDataMemberNode(DesignBindingPicker picker,
                                         object dataSource,
                                         string dataMember,
                                         string dataField,
                                         bool isList) : base(picker, dataSource, dataMember, dataField, isList)
            {
            }

            public override void OnExpand()
            {
                // Do nothing (not even call base class). All project data
                // members get added when project data source is populated.
            }

            public override DesignBinding OnSelect()
            {
                string bindingSourceMember;
                string designBindingMember;

                ProjectDataMemberNode parentListMember = (Parent as ProjectDataMemberNode);

                if (parentListMember != null)
                {
                    // Field member under list member: Point the BindingSource at list member, and the binding at the field member 
                    bindingSourceMember = parentListMember.DataMember;
                    designBindingMember = DataMember;
                }
                else if (IsList)
                {
                    // List member under data source: Point the BindingSource at list member, and the binding at the list member
                    bindingSourceMember = DataMember;
                    designBindingMember = "";
                }
                else
                {
                    // Field member under data source: Point the BindingSource at the data source, and the binding at the field member
                    bindingSourceMember = "";
                    designBindingMember = DataMember;
                }

                // Instance the project data source on the form, and point a BindingSource
                // at the appropriate list member of the form instance
                DataSourceDescriptor dataSourceDescriptor = (DataSourceDescriptor)DataSource;
                BindingSource newBindingSource = picker.CreateNewBindingSource(dataSourceDescriptor, bindingSourceMember);
                return (newBindingSource == null) ? null : new DesignBinding(newBindingSource, designBindingMember);
            }
        }
    }*/
}
