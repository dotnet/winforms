// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Drawing.Design;
using System.Data;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  OVERVIEW:
    ///
    ///  Multi-purpose data binding picker control. Used for picking data sources,
    ///  data members, and data bindings. Invoked using the Pick() method.
    ///
    ///  Data bindable items are displayed in tree form, with the following general
    ///  structure, consisting of data sources and two kinds of data member...
    ///
    ///      Data source
    ///          Field member
    ///          Field member
    ///          List member
    ///              Field member
    ///              Field member
    ///          List member
    ///              Field member
    ///              Field member
    ///      Data source
    ///          Field member
    ///          Field member
    ///          List member
    ///              Field member
    ///              Field member
    ///          List member
    ///              Field member
    ///              Field member
    ///
    ///  ...where data sources are only top-level items, and list members can be
    ///  nested to any depth. The tree can also be scoped to just show the members
    ///  of a specific data source. List members and field members can also be
    ///  filtered out. The user can only select the 'deepest' kind of item being
    ///  shown (ie. field members, then list members, then data sources).
    ///
    ///  COMMON USES:
    ///
    ///  Example property       UITypeEditor          Pick(showDS, showDM, selLists, rootDS)
    ///  ---------------------- --------------------- --------------------------------------
    ///  DataGrid.DataSource    DataSourceListEditor  Pick(1, 0, -, null)
    ///  TextBox.Text           DesignBindingEditor   Pick(1, 1, 0, null)
    ///  ComboBox.DisplayMember DataMemberFieldEditor Pick(0, 1, 0, ComboBox.DataSource)
    ///  DataGrid.DataMember    DataMemberListEditor  Pick(0, 1, 1, DataGrid.DataSource)
    ///
    ///  NEW FOR WHIDBEY:
    ///
    ///  When data sources are included, the above tree structure is now organized
    ///  as shown below. Binding sources appear first, with other form-level data
    ///  sources relegated to a sub-node. There is also a sub-node that exposes
    ///  project-level data sources.
    ///
    ///      ["None"]
    ///      Binding source
    ///          {{{data members}}}
    ///      ["Other Data Sources"]
    ///          ["Project Data Sources"]
    ///              Project data source group
    ///                  Project data source
    ///                      {{{data members}}}
    ///          ["Form List Instances"]
    ///              Data source
    ///                  {{{data members}}}
    ///
    ///  ...data members shown under each BindingSource are fine-tuned to remove
    ///  the redundancy that confused Everett users; immediate child members
    ///  are shown, but nothing deeper than that.
    ///
    ///  List members under either BindingSources or project-level data sources
    ///  count as data sources. When one is picked, a new 'related' BindingSource
    ///  is created that refers to that list member.
    ///  OVERVIEW:
    /// </summary>
    [
    ToolboxItem(false),
    DesignTimeVisible(false)
    ]
    internal class DesignBindingPicker : ContainerControl
    {
        private BindingPickerTree? _treeViewCtrl;   // Tree view that shows the available data sources and data members
        private BindingPickerLink _addNewCtrl;     // Link that invokes the "Add Project Data Source" wizard
        private readonly Panel _addNewPanel;    // Panel containing the "Add Project Data Source" link
        private HelpTextLabel _helpTextCtrl;   // Label that displays helpful text as user mouses over tree view nodes
        private readonly Panel _helpTextPanel;  // Panel containing the help text label

        private IServiceProvider? _serviceProvider; // Current VS service provider
        private IWindowsFormsEditorService? _windowsFormsEditorService; // Service used to invoke the picker inside a modal dropdown
        private DataSourceProviderService? _dataSourceProviderService; // Service that provides project level data sources and related commands
        private ITypeResolutionService? _typeResolutionService;   // Service that can return Type info for types in the user's project, at design time
        private IDesignerHost? _designerHost;            // Service that provides access to current WinForms designer session

        private bool _showDataSources;   // True to show all data sources, false to just show contents of root data source
        private bool _showDataMembers;   // True to show data members of every data source, false to omit data members
        private bool _selectListMembers; // True to allow selection of list members, false to allow selection of field members

        private object? _rootDataSource;    // Root data source used to build tree (set when picker is invoked)
        private string? _rootDataMember;    // Root data member used to build tree (set when picker is invoked)

        private DesignBinding? _selectedItem;      // Describes the initial selection on open, and the final selection on close
        private TreeNode? _selectedNode;      // Tree node that matches the initial selected item (selectedItem)
        private bool _inSelectNode;      // Prevents processing of node expansion events when auto-selecting a tree node

        private NoneNode? _noneNode;          // "None" tree node
        private OtherNode? _otherNode;         // "Other Data Sources" tree node
        private ProjectNode? _projectNode;       // "Project Data Sources" tree node
        private InstancesNode? _instancesNode;     // "Form List Instances" tree node

        private const int MinimumDimension = 250;
        private static int s_minimumHeight = MinimumDimension;
        private static int s_minimumWidth = MinimumDimension;
        private static bool s_isScalingInitialized;
        private ITypeDescriptorContext? _context;   // Context of the current 'pick' operation

        private Size _initialSize;
        private readonly BindingContext _bindingContext = new();

        // The type of RuntimeType.
        // When binding to a business object, the DesignBindingPicker needs to create an instance of the business object.
        // However, Activator.CreateInstance works only with RuntimeType - it does not work w/ Virtual Types.
        // We use the runtimeType static to determine if the type of business object is a runtime type or not.
        private static readonly Type s_runtimeType = typeof(object).GetType().GetType();

        /// <summary>
        /// Rebuilding binding picker according to new dpi received.
        /// </summary>
        private void BuildBindingPicker(int newDpi, int oldDpi)
        {
            double scalePercent = ((double)newDpi) / oldDpi;
            Label addNewDiv = new()
            {
                Height = ScaleHelper.ScaleToDpi(1, newDpi),
                BackColor = SystemColors.ControlDark,
                Dock = DockStyle.Top
            };

            _addNewCtrl = new BindingPickerLink
            {
                Text = SR.DesignBindingPickerAddProjDataSourceLabel,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                LinkBehavior = LinkBehavior.HoverUnderline
            };

            _addNewCtrl.LinkClicked += addNewCtrl_Click;

            // BindingPickerLink always initialize to primary monitor Dpi. Resizing to current Dpi.
            _addNewCtrl.Height = ScaleHelper.ScaleToPercent(_addNewCtrl.Height, scalePercent);

            Bitmap addNewBitmap = new(
                BitmapSelector.GetResourceStream(typeof(DesignBindingPicker), "AddNewDataSource.bmp")
                ?? throw new InvalidOperationException());

            addNewBitmap.MakeTransparent(Color.Magenta);
            addNewBitmap = ScaleHelper.ScaleToDpi(addNewBitmap, newDpi, disposeBitmap: true);

            PictureBox addNewIcon = new()
            {
                Image = addNewBitmap,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                Width = _addNewCtrl.Height,
                Height = _addNewCtrl.Height,
                Dock = DockStyle.Left,
                SizeMode = PictureBoxSizeMode.CenterImage,
                AccessibleRole = AccessibleRole.Graphic
            };

            Label helpTextDiv = new()
            {
                Height = ScaleHelper.ScaleToDpi(1, newDpi),
                BackColor = SystemColors.ControlDark,
                Dock = DockStyle.Top
            };

            _helpTextCtrl = new HelpTextLabel
            {
                TextAlign = ContentAlignment.TopLeft,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText
            };

            _helpTextCtrl.Height *= 2;

            int helpTextHeight = ScaleHelper.ScaleToPercent(_helpTextCtrl.Height, scalePercent);

            _addNewPanel.Height = addNewIcon.Height + 1;
            _addNewPanel.Controls.Add(_addNewCtrl);
            _addNewPanel.Controls.Add(addNewIcon);
            _addNewPanel.Controls.Add(addNewDiv);

            _helpTextPanel.Controls.Add(_helpTextCtrl);
            _helpTextPanel.Controls.Add(helpTextDiv);
            _helpTextPanel.Height = helpTextHeight + 1;
            ResetStyles(false);

            Controls.Add(_addNewPanel);
            Controls.Add(_helpTextPanel);
        }

        private void ResetStyles(bool toNone)
        {
            if (toNone)
            {
                _treeViewCtrl!.Dock = DockStyle.None;
                _addNewCtrl.Dock = DockStyle.None;
                _addNewPanel.Dock = DockStyle.None;
                _helpTextCtrl.Dock = DockStyle.None;
                _helpTextPanel.Dock = DockStyle.None;
            }
            else
            {
                _treeViewCtrl!.Dock = DockStyle.Fill;
                _addNewCtrl.Dock = DockStyle.Fill;
                _addNewPanel.Dock = DockStyle.Bottom;
                _helpTextCtrl.Dock = DockStyle.Fill;
                _helpTextPanel.Dock = DockStyle.Bottom;
            }
        }

        private void InitTreeViewCtl()
        {
            _treeViewCtrl = new BindingPickerTree
            {
                HotTracking = true,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                BorderStyle = BorderStyle.None
            };
            _initialSize = _treeViewCtrl.Size;
            _treeViewCtrl.Dock = DockStyle.Fill;
            _treeViewCtrl.MouseMove += treeViewCtrl_MouseMove;
            _treeViewCtrl.MouseLeave += treeViewCtrl_MouseLeave;
            _treeViewCtrl.AfterExpand += treeViewCtrl_AfterExpand;
            _treeViewCtrl.AccessibleName = (SR.DesignBindingPickerTreeViewAccessibleName);

            // enable explorer tree view style
            DesignerUtils.ApplyTreeViewThemeStyles(_treeViewCtrl);
        }

        /// <devdoc>
        ///  Constructor - Initializes child controls and window layout
        /// </devdoc>
        public DesignBindingPicker()
        {
            SuspendLayout();
            if (!s_isScalingInitialized)
            {
                s_minimumHeight = ScaleHelper.ScaleToInitialSystemDpi(MinimumDimension);
                s_minimumWidth = ScaleHelper.ScaleToInitialSystemDpi(MinimumDimension);
                s_isScalingInitialized = true;
            }

            InitTreeViewCtl();

            Label addNewDiv = new()
            {
                Height = 1,
                BackColor = SystemColors.ControlDark,
                Dock = DockStyle.Top
            };

            _addNewCtrl = new BindingPickerLink
            {
                Text = (SR.DesignBindingPickerAddProjDataSourceLabel),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                LinkBehavior = LinkBehavior.HoverUnderline
            };

            // use height of text for both dimensions of the Icon
            int addNewHeight = _addNewCtrl.Height;
            int addNewWidth = _addNewCtrl.Height;
            _addNewCtrl.Dock = DockStyle.Fill;
            _addNewCtrl.LinkClicked += addNewCtrl_Click;

            Bitmap addNewBitmap = new(typeof(DesignBindingPicker), "AddNewDataSource.bmp");
            addNewBitmap.MakeTransparent(Color.Magenta);
            addNewBitmap = ScaleHelper.ScaleToDpi(addNewBitmap, ScaleHelper.InitialSystemDpi, disposeBitmap: true);

            PictureBox addNewIcon = new()
            {
                Image = addNewBitmap,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText,
                Width = addNewWidth,
                Height = addNewHeight,
                Dock = DockStyle.Left,
                SizeMode = PictureBoxSizeMode.CenterImage,
                AccessibleRole = AccessibleRole.Graphic
            };

            _addNewPanel = new Panel();
            _addNewPanel.Controls.Add(_addNewCtrl);
            _addNewPanel.Controls.Add(addNewIcon);
            _addNewPanel.Controls.Add(addNewDiv);
            _addNewPanel.Height = addNewHeight + 1;
            _addNewPanel.Dock = DockStyle.Bottom;

            Label helpTextDiv = new()
            {
                Height = 1,
                BackColor = SystemColors.ControlDark,
                Dock = DockStyle.Top
            };

            _helpTextCtrl = new HelpTextLabel
            {
                TextAlign = ContentAlignment.TopLeft,
                BackColor = SystemColors.Window,
                ForeColor = SystemColors.WindowText
            };

            _helpTextCtrl.Height *= 2;
            int helpTextHeight = ScaleHelper.ScaleToInitialSystemDpi(_helpTextCtrl.Height);

            _helpTextCtrl.Dock = DockStyle.Fill;

            _helpTextPanel = new Panel();
            _helpTextPanel.Controls.Add(_helpTextCtrl);
            _helpTextPanel.Controls.Add(helpTextDiv);
            _helpTextPanel.Height = helpTextHeight + 1;
            _helpTextPanel.Dock = DockStyle.Bottom;

            Controls.Add(_treeViewCtrl);
            Controls.Add(_addNewPanel);
            Controls.Add(_helpTextPanel);

            ResumeLayout(performLayout: false);

            Size = _initialSize;
            BackColor = SystemColors.Control;
            ActiveControl = _treeViewCtrl;
            AccessibleName = SR.DesignBindingPickerAccessibleName;

            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint, true);
        }

        /// <devdoc>
        ///
        ///  Invokes picker as a dropdown control, allowing user to pick a data source
        ///  or data member to apply to some property of some component or control.
        ///  This is a modal call - it doesn't return until the dropdown closes.
        ///
        ///  Arguments:
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
        ///  Return value:
        ///
        ///     Returns a DesignBinding that describes the binding
        ///     the user picked, or null if no selection was made.
        ///
        /// </devdoc>
        public DesignBinding? Pick(ITypeDescriptorContext? context,
                                  IServiceProvider provider,
                                  bool showDataSources,
                                  bool showDataMembers,
                                  bool selectListMembers,
                                  object? rootDataSource,
                                  string rootDataMember,
                                  DesignBinding initialSelectedItem)
        {
            // Get services
            _serviceProvider = provider;
            _windowsFormsEditorService = _serviceProvider.GetService<IWindowsFormsEditorService>();
            _dataSourceProviderService = _serviceProvider.GetService<DataSourceProviderService>();
            _typeResolutionService = _serviceProvider.GetService<ITypeResolutionService>();
            _designerHost = _serviceProvider.GetService<IDesignerHost>();

            if (_windowsFormsEditorService is null)
            {
                return null;
            }

            // Record basic settings
            _context = context;
            _showDataSources = showDataSources;
            _showDataMembers = showDataMembers;
            _selectListMembers = !showDataMembers || selectListMembers;
            _rootDataSource = rootDataSource;
            _rootDataMember = rootDataMember;

            // Attempt to adjust the linklabel colors if we can get our ui service
            IUIService? uiService = _serviceProvider?.GetService(typeof(IUIService)) as IUIService;
            if (uiService is not null)
            {
                if (uiService.Styles["VsColorPanelHyperLink"] is Color color1)
                {
                    _addNewCtrl.LinkColor = color1;
                }

                if (uiService.Styles["VsColorPanelHyperLinkPressed"] is Color color2)
                {
                    _addNewCtrl.ActiveLinkColor = color2;
                }
            }

            // Fill the tree with lots of juicy stuff
            FillTree(initialSelectedItem);

            // Set initial state of the various sub-panels
            // addNewPanel.Visible = (showDataSources && dspSvc is not null && dspSvc.SupportsAddNewDataSource);
            _helpTextPanel.Visible = (showDataSources);

            // Set initial help text in help pane
            UpdateHelpText(null);

            // Invoke the modal dropdown via the editor service (returns once CloseDropDown has been called)
            _windowsFormsEditorService.DropDownControl(this);

            // Record any final selection
            DesignBinding? finalSelectedItem = _selectedItem;
            _selectedItem = null;

            // Clean up tree (remove nodes and clear node references)
            EmptyTree();

            // Clean up references
            _serviceProvider = null;
            _windowsFormsEditorService = null;
            _dataSourceProviderService = null;
            _designerHost = null;

            // Return final selection to caller
            return finalSelectedItem;
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

            double scalePercent = (double)deviceDpiNew / deviceDpiOld;
            s_minimumWidth = ScaleHelper.ScaleToDpi(MinimumDimension, deviceDpiNew);
            s_minimumHeight = ScaleHelper.ScaleToDpi(MinimumDimension, deviceDpiNew);
            Size = new Size(
                ScaleHelper.ScaleToPercent(_initialSize.Width, scalePercent),
                ScaleHelper.ScaleToPercent(_initialSize.Height, scalePercent));

            SuspendLayout();
            try
            {
                ResetStyles(true);
                _addNewPanel.Controls.Clear();
                _helpTextPanel.Controls.Clear();
                Controls.Remove(_addNewPanel);
                Controls.Remove(_helpTextPanel);
                BuildBindingPicker(deviceDpiNew, deviceDpiOld);
            }
            finally
            {
                ResumeLayout(false);
            }
        }

        /// <devdoc>
        ///  If control is open as a dropdown, and a value has been picked
        ///  by the user, close the dropdown and end the picking session.
        /// </devdoc>
        private void CloseDropDown()
        {
            // VSWhidbey#256272. If the object being edited is a BindingSource, then tell its designer to notify
            // the DataSourceProviderService of this change, once the new DataSource/DataMember value has taken
            // effect. This allows the service to generate any adapter components or fill statements needed to
            // set up whatever data source the BindingSource is now bound to. Scenario: Advanced user manually
            // configuring a BindingSource.
            //
            if (_context?.Instance is BindingSource instance && _designerHost is not null)
            {
                BindingSourceDesigner? designer = _designerHost.GetDesigner(instance) as BindingSourceDesigner;
                if (designer is not null)
                {
                    designer.BindingUpdatedByUser = true;
                }
            }

            // Tell the editor service to close the dropdown
            _windowsFormsEditorService?.CloseDropDown();
        }

        /// <devdoc>
        ///  Resets tree view to empty state.
        /// </devdoc>
        private void EmptyTree()
        {
            _noneNode = null;
            _otherNode = null;
            _projectNode = null;
            _instancesNode = null;
            _selectedNode = null;

            _treeViewCtrl?.Nodes.Clear();
        }

        /// <devdoc>
        ///  Initializes and populates the tree view.
        /// </devdoc>
        private void FillTree(DesignBinding initialSelectedItem)
        {
            // Set the initial selected item
            _selectedItem = initialSelectedItem;

            // Force tree into empty state
            EmptyTree();

            // Create the 'special' nodes
            _noneNode = new NoneNode();
            _otherNode = new OtherNode();
            _projectNode = new ProjectNode(this);
            if (_designerHost is not null && _designerHost.RootComponent is not null && _designerHost.RootComponent.Site is not null)
            {
                _instancesNode = new InstancesNode(_designerHost?.RootComponent.Site.Name);
            }
            else
            {
                _instancesNode = new InstancesNode(string.Empty);
            }

            // Add the 'None' node at the top
            _treeViewCtrl?.Nodes.Add(_noneNode);

            if (_showDataSources)
            {
                // Add form-level data sources
                AddFormDataSources();

                // Add project-level data sources
                AddProjectDataSources();

                // Add the remaining 'special' nodes, if they are required
                if (_projectNode.Nodes.Count > 0)
                {
                    _otherNode.Nodes.Add(_projectNode);
                }

                if (_instancesNode.Nodes.Count > 0)
                {
                    _otherNode.Nodes.Add(_instancesNode);
                }

                if (_otherNode.Nodes.Count > 0)
                {
                    _treeViewCtrl?.Nodes.Add(_otherNode);
                }
            }
            else
            {
                // Add contents of one specific data source
                AddDataSourceContents(_treeViewCtrl?.Nodes, _rootDataSource, _rootDataMember, null);
            }

            // If no node was matched to the selected item, just select the 'None' node
            _selectedNode ??= _noneNode;

            // Selected node should be recorded now, so clear the selected item.
            _selectedItem = null;

            // Set default width (based on items in tree)
            Width = Math.Max(Width, _treeViewCtrl is null ? 0 : _treeViewCtrl.PreferredWidth + (SystemInformation.VerticalScrollBarWidth * 2));
        }

        /// <devdoc>
        ///  Fills the tree view with top-level data source nodes.
        /// </devdoc>
        private void AddFormDataSources()
        {
            // VSWhidbey#455147. If the ITypeDescriptorContext does not have a container, grab the container from the
            // IDesignerHost.
            IContainer? container = null;
            if (_context is not null)
            {
                container = _context.Container;
            }

            if (container is null && _designerHost is not null)
            {
                container = _designerHost.Container;
            }

            // Bail if we have no container to work with
            if (container is null)
            {
                return;
            }

            container = DesignerUtils.CheckForNestedContainer(container)!; // ...necessary to support SplitterPanel components

            ComponentCollection components = container.Components;

            // Enumerate the components of the container (eg. the Form)
            foreach (IComponent comp in components)
            {
                // Don't add component to tree if it is the very object who's property the picker
                // is setting (ie. don't let a BindingSource's DataSource property point to itself).
                if (comp == _context?.Instance)
                {
                    continue;
                }

                // Don't add a DataTable to the tree if its parent DataSet is gonna be in the tree.
                // (...new redundancy-reducing measure for Whidbey)
                if (comp is DataTable && FindComponent(components, (comp as DataTable)?.DataSet))
                {
                    continue;
                }

                // Add tree node for this data source
                if (comp is BindingSource)
                {
                    AddDataSource(_treeViewCtrl?.Nodes, comp, null);
                }
                else
                {
                    AddDataSource(_instancesNode?.Nodes, comp, null);
                }
            }
        }

        /// <devdoc>
        ///  Adds a tree node representing a data source. Also adds the data source's immediate
        ///  child data members, so that the node has the correct +/- state by default.
        /// </devdoc>
        private void AddDataSource(TreeNodeCollection? nodes, IComponent dataSource, string? dataMember)
        {
            // Don't add node if not showing data sources
            if (!_showDataSources)
            {
                return;
            }

            // Don't add node if this is not a valid bindable data source
            if (!IsBindableDataSource(dataSource))
            {
                return;
            }

            // Get properties of this data source
            string? getPropsError = null;
            PropertyDescriptorCollection? properties = null;
            try
            {
                properties = GetItemProperties(dataSource, dataMember);
                if (properties is null)
                {
                    return;
                }
            }
            catch (ArgumentException e)
            {
                // Exception can occur trying to get list item properties from a data source that's
                // in a badly configured state (eg. its data member refers to a property on its
                // parent data source that's invalid because the parent's metadata has changed).
                getPropsError = e.Message;
            }

            // If data source has no properties, and we are in member-picking mode rather than
            // source-picking mode, just omit the data source altogether - its useless.
            if (_showDataMembers && properties?.Count == 0)
            {
                return;
            }

            // Create node and add to specified nodes collection
            DataSourceNode dataSourceNode = new(this, dataSource, dataSource.Site?.Name);
            nodes?.Add(dataSourceNode);

            // If this node matches the selected item, make it the selected node
            if (_selectedItem is not null && _selectedItem.Equals(dataSource, ""))
            {
                _selectedNode = dataSourceNode;
            }

            // Since a data source is added directly to the top level of the tree, rather than
            // revealed by user expansion, we need to fill in its children and grand-children,
            // and mark it as 'filled'.
            if (getPropsError is null)
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
        ///  Adds a set of tree nodes representing the immediate child data members of a data source.
        /// </devdoc>
        private void AddDataSourceContents(TreeNodeCollection? nodes, object? dataSource, string? dataMember, PropertyDescriptorCollection? properties)
        {
            // Don't add nodes if not showing data members (except for BindingSources, we always want to show list members)
            if (!_showDataMembers && !(dataSource is BindingSource))
            {
                return;
            }

            // Special case: Data source is a list type (or list item type) rather than a list instance.
            //    Arises when some component's DataSource property is bound to a Type, and the user opens
            //    the dropdown for the DataMember property.
            //    We need to create a temporary instance of the correct list type,
            //    and use that as our data source for the purpose of determining data members.
            //    Since only BindingSource supports type binding, we bind a temporary BindingSource
            //    to the specified type - it will create an instance of the correct list type for us.
            //    Fixes VSWhidbey bugs 302757 and 280708.
            if (dataSource is Type)
            {
                try
                {
                    BindingSource bindingSource = [];
                    bindingSource.DataSource = dataSource;
                    dataSource = bindingSource.List;
                }
                catch (Exception ex)
                {
                    if (ExceptionExtensions.IsCriticalException(ex))
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
            if (properties is null)
            {
                properties = GetItemProperties(dataSource, dataMember);
                if (properties is null)
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
        ///  Adds a tree node representing a data member. Also adds the data member's immediate
        ///  child data members, so that the node has the correct +/- state by default.
        /// </devdoc>
        private void AddDataMember(TreeNodeCollection? nodes, object? dataSource, string dataMember, string propertyName, bool isList)
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
            bool pickingFieldMembers = _showDataMembers && !_selectListMembers;
            bool omitMember = isBindingSourceListMember && pickingFieldMembers;
            bool omitMemberContents = (isBindingSourceListMember && !pickingFieldMembers) || _context?.Instance is BindingSource;

            // Just omit this member when necessary
            if (omitMember)
            {
                return;
            }

            // Don't add node if its not a list but we only want lists
            if (_selectListMembers && !isList)
            {
                return;
            }

            // Create node and add to specified nodes collection
            DataMemberNode dataMemberNode = new(this, dataSource, dataMember, propertyName, isList);
            nodes?.Add(dataMemberNode);

            // If this node matches the selected item, make it the selected node
            if (_selectedItem is not null && _selectedItem.Equals(dataSource, dataMember) && dataMemberNode is not null)
            {
                _selectedNode = dataMemberNode;
            }

            // Add contents of data member underneath the new node
            if (!omitMemberContents && dataMemberNode is not null)
            {
                AddDataMemberContents(dataMemberNode);
            }
        }

        /// <devdoc>
        ///  Adds a set of tree nodes representing the immediate child data members of a data member.
        ///
        ///  Note: If one of the nodes lies in the path to the selected item, we recursively start
        ///  adding its sub-nodes, and so on, until we reach the node for that item. This is needed
        ///  to allow that node to be auto-selected and expanded when the dropdown first appears.
        /// </devdoc>
        private void AddDataMemberContents(TreeNodeCollection nodes, object? dataSource, string dataMember, bool isList)
        {
            // Sanity check for correct use of the SubNodesFilled mechanism
            Debug.Assert(nodes.Count == 0, "We only add data member content sub-nodes once.");

            // Don't add nodes for a data member that isn't a list
            if (!isList)
            {
                return;
            }

            // Get properties of this data member
            PropertyDescriptorCollection? properties = GetItemProperties(dataSource, dataMember);
            if (properties is null)
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
                if (_selectListMembers && !isSubList)
                {
                    continue;
                }

                // Add a data member sub-node for this property
                DataMemberNode dataMemberNode = new(this, dataSource, dataMember + "." + property.Name, property.Name, isSubList);
                nodes.Add(dataMemberNode);

                // Auto-select support.
                if (_selectedItem is not null && _selectedItem.DataSource == dataMemberNode.DataSource)
                {
                    if (_selectedItem.Equals(dataSource, dataMemberNode.DataMember))
                    {
                        // If this node matches the selected item, make it the selected node
                        _selectedNode = dataMemberNode;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(_selectedItem.DataMember)
                            && _selectedItem.DataMember.StartsWith(dataMemberNode.DataMember, StringComparison.Ordinal))
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
        ///  AddDataMemberContents overload. This version supplies the information
        ///  about the data member from an existing data member tree node.
        /// </devdoc>
        private void AddDataMemberContents(TreeNodeCollection nodes, DataMemberNode dataMemberNode)
        {
            AddDataMemberContents(nodes, dataMemberNode.DataSource, dataMemberNode.DataMember, dataMemberNode.IsList);
        }

        /// <devdoc>
        ///  AddDataMemberContents overload. This version supplies the information
        ///  about the data member from an existing data member tree node, and adds
        ///  the contents to that node.
        /// </devdoc>
        private void AddDataMemberContents(DataMemberNode dataMemberNode)
        {
            AddDataMemberContents(dataMemberNode.Nodes, dataMemberNode);
        }

        /// <devdoc>
        ///  Add project level data sources under the special 'Project' tree node
        /// </devdoc>
        private void AddProjectDataSources()
        {
            if (_dataSourceProviderService is null)
            {
                return;
            }

            // Get the entire set of project-level data sources
            DataSourceGroupCollection groups = _dataSourceProviderService.GetDataSources();
            if (groups is null)
            {
                return;
            }

            // If we're gonna be expanding the Project node tree to select a specific
            // project data source or data member, just build the entire tree up front
            bool addMembers = (_selectedItem is not null && _selectedItem.DataSource is DataSourceDescriptor);

            // Create nodes for every project-level data source
            foreach (DataSourceGroup g in groups)
            {
                if (g is not null)
                {
                    if (g.IsDefault)
                    {
                        // Data sources in project's default namespace go directly under 'Project' node
                        AddProjectGroupContents(_projectNode?.Nodes, g);
                    }
                    else
                    {
                        // All other data sources are organized into groups
                        AddProjectGroup(_projectNode?.Nodes, g, addMembers);
                    }
                }
            }

            // If required, force top-level data sources to fill in their data members now
            if (addMembers)
            {
                _projectNode?.FillSubNodes();
            }
        }

        /// <devdoc>
        ///  Add node for a given project level data source 'group'.
        /// </devdoc>
        private void AddProjectGroup(TreeNodeCollection? nodes, DataSourceGroup group, bool addMembers)
        {
            // Create the group node, add its data sources, and wire it up
            ProjectGroupNode groupNode = new(this, group.Name, group.Image);
            AddProjectGroupContents(groupNode.Nodes, group);
            nodes?.Add(groupNode);

            // If required, force data sources in this group to fill in their data members now
            if (addMembers)
            {
                groupNode.FillSubNodes();
            }
        }

        /// <devdoc>
        ///  Add nodes for data sources in a given project level data source 'group'.
        /// </devdoc>
        private void AddProjectGroupContents(TreeNodeCollection? nodes, DataSourceGroup group)
        {
            DataSourceDescriptorCollection dataSources = group.DataSources;
            if (dataSources is null)
            {
                return;
            }

            foreach (DataSourceDescriptor dataSourceDescriptor in dataSources)
            {
                if (dataSourceDescriptor is not null && nodes is not null)
                {
                    AddProjectDataSource(nodes, dataSourceDescriptor);
                }
            }
        }

        /// <devdoc>
        ///  Add a node for a single project level data source.
        /// </devdoc>
        private void AddProjectDataSource(TreeNodeCollection nodes, DataSourceDescriptor descriptor)
        {
            // Create and add the project data source tree node
            //

            // vsw 477085: don't add the project data source if it points to a virtual type.
            Type? type = GetType(descriptor.TypeName, true, true);
            if (type is not null && type.GetType() != s_runtimeType)
            {
                return;
            }

            ProjectDataSourceNode projectDataSourceNode = new(this, descriptor, descriptor.Name, descriptor.Image);
            nodes.Add(projectDataSourceNode);

            // Auto-select this new node if it corresponds to the current selection (ie. current value)
            //
            if (_selectedItem is not null && string.IsNullOrEmpty(_selectedItem.DataMember))
            {
                // If the current selection is a project-level data source, see if this node has the same name.
                // - The current selection normally refers to a form-level instance of a data source; the only
                //   time the current selection will be a project-level data source is when the user has created
                //   a new one using the 'Add' wizard and we want to show it selected afterwards.
                //
                if (_selectedItem.DataSource is DataSourceDescriptor && _selectedItem.DataSource is DataSourceDescriptor dataSourceDescriptor &&
                    string.Equals(descriptor.Name, dataSourceDescriptor.Name, StringComparison.OrdinalIgnoreCase))
                {
                    _selectedNode = projectDataSourceNode;
                }

                // If the current selection is a simple type, see if this node refers to the same type.
                // - Bindable components can specify an item type as their data source at design time, which
                //   provides the necessary metadata info for the designer. The assumption is that the 'real'
                //   data source instance (that actually returns items of that type) gets supplied at run-time
                //   by customer code.
                else if (_selectedItem.DataSource is Type && _selectedItem.DataSource is Type type_ &&
                    string.Equals(descriptor.TypeName, type_.FullName, StringComparison.OrdinalIgnoreCase))
                {
                    _selectedNode = projectDataSourceNode;
                }
            }
        }

        /// <devdoc>
        ///  Add the data member nodes for a project level data source.
        /// </devdoc>
        private void AddProjectDataSourceContents(TreeNodeCollection nodes, DataSourceNode projectDataSourceNode)
        {
            if (projectDataSourceNode.DataSource is not DataSourceDescriptor dataSourceDescriptor)
            {
                return;
            }

            // Get data source type
            Type? dataSourceType = GetType(dataSourceDescriptor.TypeName, false, false);
            if (dataSourceType is null)
            {
                return;
            }

            // If data source type is instancable, create an instance of it, otherwise just use the type itself
            object? dataSourceInstance = dataSourceType;
            try
            {
                dataSourceInstance = Activator.CreateInstance(dataSourceType);
            }
            catch (Exception ex)
            {
                if (ExceptionExtensions.IsCriticalException(ex))
                {
                    throw;
                }
            }

            // Is this data source just a "list of lists"? (eg. DataSet is just a set of DataTables)
            bool isListofLists = (dataSourceInstance is IListSource listSource) && listSource.ContainsListCollection;

            // Fix for VSWhidbey#223724:
            // When offering choices for the DataSource of a BindingSource, we want to stop the user from being able to pick a table under
            // a data set, since this implies a DS/DM combination, requiring us to create a new 'related' BindingSource. We'd rather the
            // user just picked the data set as the DS, and then set the DM to the table, and avoid creating a redundant BindingSource.
            if (isListofLists && _context?.Instance is BindingSource)
            {
                return;
            }

            // Determine the properties of the data source
            PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(dataSourceInstance);
            if (properties is null)
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
                if (_selectListMembers && !isSubList)
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
                AddProjectDataMember(nodes, dataSourceDescriptor, pd, dataSourceInstance, isSubList);
            }
        }

        /// <devdoc>
        ///  AddProjectDataSourceContents overload.
        /// </devdoc>
        private void AddProjectDataSourceContents(DataSourceNode projectDataSourceNode)
        {
            AddProjectDataSourceContents(projectDataSourceNode.Nodes, projectDataSourceNode);
        }

        /// <devdoc>
        ///  Add a node for a single data member of a project level data source.
        /// </devdoc>
        private void AddProjectDataMember(TreeNodeCollection nodes,
                                          DataSourceDescriptor dataSourceDescriptor,
                                          PropertyDescriptor propertyDescriptor,
                                          object? dataSourceInstance,
                                          bool isList)
        {
            // vsw 477085: don't add the project data source if it points to a virtual type.
            Type? dsType = GetType(dataSourceDescriptor.TypeName, true, true);
            if (dsType is not null && dsType.GetType() != s_runtimeType)
            {
                return;
            }

            DataMemberNode projectDataMemberNode = new ProjectDataMemberNode(this, dataSourceDescriptor, propertyDescriptor.Name, propertyDescriptor.Name, isList);
            nodes.Add(projectDataMemberNode);
            AddProjectDataMemberContents(projectDataMemberNode, dataSourceDescriptor, propertyDescriptor, dataSourceInstance);
        }

        /// <devdoc>
        ///  Add nodes for the sub-members of a data member under a project level data source.
        /// </devdoc>
        private void AddProjectDataMemberContents(TreeNodeCollection nodes,
                                                  DataMemberNode projectDataMemberNode,
                                                  DataSourceDescriptor dataSourceDescriptor,
                                                  PropertyDescriptor propertyDescriptor,
                                                  object? dataSourceInstance)
        {
            // List members under project data sources are only shown to a certain depth,
            // and should already have all been created by the time we get here. So if
            // we're not adding field members, there's nothing more to do.
            if (_selectListMembers)
            {
                return;
            }

            // If its not a list member, it can't have any sub-members
            if (!projectDataMemberNode.IsList)
            {
                return;
            }

            // Need data source instance or data source type to determine properties of list member
            if (dataSourceInstance is null)
            {
                return;
            }

            // Determine properties of list member
            PropertyDescriptorCollection properties = ListBindingHelper.GetListItemProperties(dataSourceInstance, [propertyDescriptor]);
            if (properties is null)
            {
                return;
            }

            // Add field member for each property
            foreach (PropertyDescriptor descriptor in properties)
            {
                // Skip properties that do not represent bindable data members
                if (!IsBindableDataMember(descriptor))
                {
                    continue;
                }

                // Skip properties that are not browsable
                if (!descriptor.IsBrowsable)
                {
                    continue;
                }

                // We only add field members (no nesting of list members under project data sources)
                bool isSubList = IsListMember(descriptor);
                if (isSubList)
                {
                    continue;
                }

                // Add the field member (without contents)
                AddProjectDataMember(nodes, dataSourceDescriptor, descriptor, dataSourceInstance, isSubList);
            }
        }

        /// <devdoc>
        ///  AddProjectDataMemberContents overload.
        /// </devdoc>
        private void AddProjectDataMemberContents(DataMemberNode projectDataMemberNode,
                                                  DataSourceDescriptor dataSourceDescriptor,
                                                  PropertyDescriptor propertyDescriptor,
                                                  object? dataSourceInstance)
        {
            AddProjectDataMemberContents(projectDataMemberNode.Nodes, projectDataMemberNode, dataSourceDescriptor, propertyDescriptor, dataSourceInstance);
        }

        /// <devdoc>
        ///  Puts a new BindingSource on the form, with the specified DataSource and DataMember values.
        /// </devdoc>
        private BindingSource? CreateNewBindingSource(object dataSource, string dataMember)
        {
            if (_designerHost is null || _dataSourceProviderService is null)
            {
                return null;
            }

            // Create the BindingSource
            BindingSource bs = [];
            try
            {
                bs.DataSource = dataSource;
                bs.DataMember = dataMember;
            }
            catch (Exception ex)
            {
                IUIService? uiService = _serviceProvider?.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                return null;
            }

            // Give it a name
            string bindingSourceName = GetBindingSourceNamePrefix(dataSource, dataMember);
            // If we have a service provider then use it to get the camel notation from ToolStripDesigner.NameFromText
            if (_serviceProvider is not null)
            {
                bindingSourceName = ToolStripDesigner.NameFromText(bindingSourceName, bs.GetType(), _serviceProvider);
            }
            else
            {
                bindingSourceName += bs.GetType().Name;
            }

            // Make sure the name is unique.
            string? uniqueSiteName = DesignerUtils.GetUniqueSiteName(_designerHost, bindingSourceName);

            DesignerTransaction? trans = _designerHost.CreateTransaction(string.Format(SR.DesignerBatchCreateTool, uniqueSiteName));

            try
            {
                // Put it on the form
                try
                {
                    _designerHost.Container.Add(bs, uniqueSiteName);
                }

                catch (InvalidOperationException ex)
                {
                    trans?.Cancel();

                    IUIService? uiService = _serviceProvider?.GetService(typeof(IUIService)) as IUIService;
                    DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                    return null;
                }

                catch (CheckoutException ex)
                {
                    trans?.Cancel();

                    IUIService? uiService = _serviceProvider?.GetService(typeof(IUIService)) as IUIService;
                    DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                    return null;
                }

                // Notify the provider service that a new form object is referencing this project-level data source
                _dataSourceProviderService.NotifyDataSourceComponentAdded(bs);
                if (trans is not null)
                {
                    trans.Commit();
                    trans = null;
                }
            }

            finally
            {
                trans?.Cancel();
            }

            return bs;
        }

        /// <devdoc>
        ///  CreateNewBindingSource overload, for project-level data sources.
        /// </devdoc>
        private BindingSource? CreateNewBindingSource(DataSourceDescriptor dataSourceDescriptor, string dataMember)
        {
            if (_designerHost is null || _dataSourceProviderService is null)
            {
                return null;
            }

            // Find or create a form-level instance of this project-level data source
            object? dataSource = GetProjectDataSourceInstance(dataSourceDescriptor);
            if (dataSource is null)
            {
                return null;
            }

            // Create a BindingSource that points to the form-level instance
            return CreateNewBindingSource(dataSource, dataMember);
        }

        /// <devdoc>
        ///  Chooses the best name prefix for a new BindingSource, based on the
        ///  data source and data member that the binding source is bound to.
        /// </devdoc>
        private static string GetBindingSourceNamePrefix(object dataSource, string dataMember)
        {
            // Always use the data member string, if one is available
            if (!string.IsNullOrEmpty(dataMember))
            {
                return dataMember;
            }

            // Data source should never be null
            if (dataSource is null)
            {
                return "";
            }

            // If data source is a type, use the name of the type
            Type? type = (dataSource as Type);
            if (type is not null)
            {
                return type.Name;
            }

            // If data source is a form component, use its sited name
            IComponent? component = (dataSource as IComponent);
            if (component is not null)
            {
                ISite? site = component.Site;

                if (site is not null && !string.IsNullOrEmpty(site.Name))
                {
                    return site.Name;
                }
            }

            // Otherwise just use the type name of the data source
            return dataSource.GetType().Name;
        }

        /// <devdoc>
        ///  Get the Type with the specified name. If TypeResolutionService is available,
        ///  use that in preference to using the Type class (since this service can more
        ///  reliably instantiate project level types).
        /// </devdoc>
        [SuppressMessage(
            "Trimming",
            "IL2096:Call to 'Type.GetType' method can perform case insensitive lookup of the type, currently trimming can not guarantee presence of all the matching types.",
            Justification = "No known workaround.")]
        private Type? GetType(string name, bool throwOnError, bool ignoreCase)
        {
            if (_typeResolutionService is not null)
            {
                return _typeResolutionService.GetType(name, throwOnError, ignoreCase);
            }
            else
            {
                return Type.GetType(name, throwOnError, ignoreCase);
            }
        }

        /// <devdoc>
        ///  Finds the form-level instance of a project-level data source. Looks for form components
        ///  who's type matches that of the project-level data source. If none are found, ask the
        ///  provider service to add one for us.
        ///
        ///  Note: If the project-level data source is not instance-able, just return its type as
        ///  the data source to bind to ("simple type binding" case).
        /// </devdoc>
        private object? GetProjectDataSourceInstance(DataSourceDescriptor dataSourceDescriptor)
        {
            Type? dsType = GetType(dataSourceDescriptor.TypeName, true, true);

            // Not an instance-able type, so just return the type
            if (!dataSourceDescriptor.IsDesignable)
            {
                return dsType;
            }

            // Enumerate the components of the container (eg. the Form)
            IContainer? container = _designerHost?.Container;
            if (container is not null)
            {
                foreach (IComponent comp in container.Components)
                {
                    // Return the first matching component we find
                    if (dsType is not null && dsType.Equals(comp.GetType()))
                    {
                        return comp;
                    }
                }
            }

            // No existing instances found, so ask provider service to create a new one
            try
            {
                return _dataSourceProviderService?.AddDataSourceInstance(_designerHost, dataSourceDescriptor);
            }
            catch (InvalidOperationException ex)
            {
                IUIService? uiService = _serviceProvider?.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                return null;
            }
            catch (CheckoutException ex)
            {
                IUIService? uiService = _serviceProvider?.GetService(typeof(IUIService)) as IUIService;
                DataGridViewDesigner.ShowErrorDialog(uiService, ex, this);
                return null;
            }
        }

        /// <devdoc>
        ///  See if a component collection contains a given component (simple linear search).
        /// </devdoc>
        private static bool FindComponent(ComponentCollection components, IComponent? targetComponent)
        {
            foreach (IComponent component in components)
            {
                if (component == targetComponent)
                {
                    return true;
                }
            }

            return false;
        }

        /// <devdoc>
        ///  See if the given object is a valid bindable data source.
        /// </devdoc>
        private static bool IsBindableDataSource(object? dataSource)
        {
            // Check for expected interfaces (require at least one)
            if (dataSource is not (IListSource or IList or Array))
            {
                return false;
            }

            // Check for [ListBindable(false)] attribute
            ListBindableAttribute? listBindable = TypeDescriptor.GetAttributes(dataSource)[typeof(ListBindableAttribute)] as ListBindableAttribute;
            if (listBindable is not null && !listBindable.ListBindable)
            {
                return false;
            }

            return true;
        }

        /// <devdoc>
        ///  See if the given property represents a bindable data member.
        ///
        ///  [IainHe] Oddly, we always check the [ListBindable] attribute on the property. This makes sense for
        ///  list members, but seems pretty meaningless for field members. But that's what we've always done,
        ///  so let's continue to do it.
        /// </devdoc>
        private static bool IsBindableDataMember(PropertyDescriptor property)
        {
            // Special case: We want byte arrays to appear as bindable field members.
            if (typeof(byte[]).IsAssignableFrom(property.PropertyType))
            {
                return true;
            }

            // Check for [ListBindable(false)] attribute
            ListBindableAttribute? listBindable = property.Attributes[typeof(ListBindableAttribute)] as ListBindableAttribute;
            if (listBindable is not null && !listBindable.ListBindable)
            {
                return false;
            }

            return true;
        }

        /// <devdoc>
        ///  See if the given property represents a list member rather than a field member.
        /// </devdoc>
        private static bool IsListMember(PropertyDescriptor property)
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
        ///  For a data source, or a data member that's a list, this method returns a
        ///  description of the properties possessed by items in the underlying list.
        /// </devdoc>
        private PropertyDescriptorCollection? GetItemProperties(object? dataSource, string? dataMember)
        {
            if (dataSource is null)
            {
                return null;
            }

            CurrencyManager? listManager = _bindingContext?[dataSource, dataMember] as CurrencyManager;
            return listManager?.GetItemProperties();
        }

        /// <devdoc>
        ///  Update roll-over help text as user mouses from tree node to tree node.
        ///
        ///  Basic rules...
        ///  - If the mouse is over a node, the node usually supplies its own help text.
        ///  - Else if there is an existing selection, report that as the current binding.
        ///  - Else just display some general picker help text.
        ///
        ///  The goal of the general text is to provide help in 'initial use and set up' scenarios,
        ///  to guide the user through the set of steps needed to create their first binding. The
        ///  general text cases below get progressively further "back in time" as you go down.
        ///
        ///  Note: All help text strings are geared specifically towards the context of a picker
        ///  that is showing data sources. So when the picker is just showing data members (scoped
        ///  to a specific data source), the help text area will be hidden.
        /// </devdoc>
        private void UpdateHelpText(BindingPickerNode? mouseNode)
        {
            if (_instancesNode is null)
            {
                return;
            }

            // See if node under mouse wants to supply its own help text
            string? mouseNodeHelpText = mouseNode?.HelpText;
            string? mouseNodeErrorText = mouseNode?.Error;

            // Set the colors...
            if (mouseNodeHelpText is not null || mouseNodeErrorText is not null)
            {
                _helpTextCtrl.BackColor = SystemColors.Info;
                _helpTextCtrl.ForeColor = SystemColors.InfoText;
            }
            else
            {
                _helpTextCtrl.BackColor = SystemColors.Window;
                _helpTextCtrl.ForeColor = SystemColors.WindowText;
            }

            // Set the text...
            if (mouseNodeErrorText is not null)
            {
                // This node has an ERROR associated with it
                _helpTextCtrl.Text = mouseNodeErrorText;
            }
            else if (mouseNodeHelpText is not null)
            {
                // Node specific help text
                _helpTextCtrl.Text = mouseNodeHelpText;
            }
            else if (_selectedNode is not null && _selectedNode != _noneNode)
            {
                // Already bound to something (user has experience)
                _helpTextCtrl.Text = string.Format(CultureInfo.CurrentCulture, (SR.DesignBindingPickerHelpGenCurrentBinding), _selectedNode.Text);
            }
            else if (!_showDataSources)
            {
                // No data sources, so this is just a simple data member pick list
                _helpTextCtrl.Text = (_treeViewCtrl?.Nodes.Count > 1) ? (SR.DesignBindingPickerHelpGenPickMember) : "";
            }
            else if (_treeViewCtrl?.Nodes.Count > 1 && _treeViewCtrl.Nodes[1] is DataSourceNode)
            {
                // BindingSources exist - tell user to pick one
                _helpTextCtrl.Text = (SR.DesignBindingPickerHelpGenPickBindSrc);
            }
            else if (_instancesNode.Nodes.Count > 0 || _projectNode?.Nodes.Count > 0)
            {
                // Data sources exist - tell user to pick one
                _helpTextCtrl.Text = (SR.DesignBindingPickerHelpGenPickDataSrc);
            }
            else if (_addNewPanel.Visible)
            {
                // No data sources - tell user how to create one
                _helpTextCtrl.Text = (SR.DesignBindingPickerHelpGenAddDataSrc);
            }
            else
            {
                // No data sources, and no way to create one!
                _helpTextCtrl.Text = "";
            }
        }

        /// <devdoc>
        ///  Always pass focus down to tree control (so that selection is always visible)
        /// </devdoc>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            _treeViewCtrl?.Focus();
        }

        /// <devdoc>
        ///  Updates the state of the control when shown or hidden. When shown, we make sure
        ///  the current selection is visible, and start listening to node expand events (so
        ///  we can fill the tree as the user drills down).
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
        ///  Enforces the control's minimum width and height.
        /// </devdoc>
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
        {
            if ((specified & BoundsSpecified.Width) == BoundsSpecified.Width)
            {
                width = Math.Max(width, s_minimumWidth);
            }

            if ((specified & BoundsSpecified.Height) == BoundsSpecified.Height)
            {
                height = Math.Max(height, s_minimumHeight);
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <devdoc>
        ///  Handle click on the "Add Project Data Source" link label.
        /// </devdoc>
        private void addNewCtrl_Click(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            // No provider service, or provider won't allow creation of new data sources right now
            if (_dataSourceProviderService is null || !_dataSourceProviderService.SupportsAddNewDataSource)
            {
                return;
            }

            // Invoke the 'Add' wizard
            DataSourceGroup newProjectDataSources = _dataSourceProviderService.InvokeAddNewDataSource(this, FormStartPosition.CenterScreen);

            // Wizard was cancelled or did not create any new data sources
            if (newProjectDataSources is null || newProjectDataSources.DataSources.Count == 0)
            {
                return;
            }

            // Rule: If multiple data sources were created, just use the first one.
            DataSourceDescriptor newProjectDataSource = newProjectDataSources.DataSources[0];

            // Update tree to include the new data source (and select it)
            FillTree(new DesignBinding(newProjectDataSource, ""));

            // If we weren't able to select the node representing the new data
            // source, then something has gone horribly wrong - bail out now!
            if (_selectedNode is null)
            {
                Debug.Fail("Failed to select new project-level data source in DesignBindingPicker tree.");
                return;
            }

            // Count the number of data members under this data source
            int dataMemberCount = _selectedNode.Nodes.Count;

            //
            // Decide what to do with the new data source...
            //

            if (_context?.Instance is BindingSource)
            {
                // Bindable object is a BindingSource - no choice, must bind to data source
                _treeViewCtrl?.SetSelectedItem(_selectedNode);
            }

            if (dataMemberCount == 0 || _context?.Instance is BindingSource)
            {
                // Zero data members - bind to the data source
                _treeViewCtrl?.SetSelectedItem(_selectedNode);
            }
            else if (dataMemberCount == 1)
            {
                // One data member - bind to that data member
                _treeViewCtrl?.SetSelectedItem(_selectedNode.Nodes[0]);
            }
            else
            {
                // Multiple data members - stay open and show them all
                ShowSelectedNode();
                _selectedNode.Expand();
                _selectedNode = null;
                UpdateHelpText(null);
            }
        }

        /// <devdoc>
        ///  Update roll-over help text as user mouses from tree node to tree node.
        /// </devdoc>
        private void treeViewCtrl_MouseMove(object? sender, MouseEventArgs e)
        {
            // Get the tree node under the mouse
            Point pt = new(e.X, e.Y);
            TreeNode? node = _treeViewCtrl?.GetNodeAt(pt);

            // Make sure point is over the node label, since GetNodeAt() will return
            // a node even when the mouse is way off to the far right of that node.
            if (node is not null && !node.Bounds.Contains(pt))
            {
                node = null;
            }

            // Update the help text
            UpdateHelpText(node as BindingPickerNode);
        }

        /// <devdoc>
        ///  Reset roll-over help text if user mouses away from the tree view.
        /// </devdoc>
        private void treeViewCtrl_MouseLeave(object? sender, EventArgs e)
        {
            UpdateHelpText(null);
        }

        /// <devdoc>
        ///  When user expands a tree node to reveal its sub-nodes, we fill in the contents
        ///  of those sub-nodes, so that their +/- states are correct. In other words, we
        ///  fill the tree "one level ahead" of what the user has revealed.
        /// </devdoc>
        private void treeViewCtrl_AfterExpand(object? sender, TreeViewEventArgs tvcevent)
        {
            // Ignore expansion caused by something other than direct user action (eg. auto-selection)
            if (_inSelectNode || !Visible)
            {
                return;
            }

            // Let the node do whatever it wants
            (tvcevent.Node as BindingPickerNode)?.OnExpand();
        }

        /// <devdoc>
        ///  Ensure the initial selection is visible (ie. select the corresponding
        ///  tree node, which also causes auto-expand of all ancestor nodes).
        ///
        ///  Note: Posting has to be used here because the tree view control won't
        ///  let us select nodes until all the underlying Win32 HTREEITEMs have
        ///  been created.
        /// </devdoc>
        private void ShowSelectedNode()
        {
            PostSelectTreeNode(_selectedNode);
        }

        /// <devdoc>
        ///  Selects a given node in the tree view. Because the tree view will auto-expand any
        ///  ancestor nodes, in order to make the selected node visible, we have to temporarily
        ///  turn off a couple of things until selection is finished: (a) painting; (b) processing
        ///  of 'node expand' events.
        /// </devdoc>
        private void SelectTreeNode(TreeNode? node)
        {
            if (_inSelectNode)
            {
                return;
            }

            try
            {
                _inSelectNode = true;
                _treeViewCtrl?.BeginUpdate();
                _treeViewCtrl!.SelectedNode = node;
                _treeViewCtrl?.EndUpdate();
            }
            finally
            {
                _inSelectNode = false;
            }
        }

        /// <summary>
        ///  The following methods exist to support posted (ie. delayed) selection of tree nodes...
        /// </summary>
        private delegate void PostSelectTreeNodeDelegate(TreeNode node);

        private void PostSelectTreeNodeCallback(TreeNode node)
        {
            SelectTreeNode(null);
            SelectTreeNode(node);
        }

        private void PostSelectTreeNode(TreeNode? node)
        {
            if (node is not null && IsHandleCreated)
            {
                BeginInvoke(PostSelectTreeNodeCallback, [node]);
            }
        }

        /// <summary>
        /// Label control that renders its text with both word wrapping, end ellipsis and partial line clipping.
        /// </summary>
        internal class HelpTextLabel : Label
        {
            protected override void OnPaint(PaintEventArgs e)
            {
                TextFormatFlags formatFlags =
                    TextFormatFlags.WordBreak |
                    TextFormatFlags.EndEllipsis |
                    TextFormatFlags.TextBoxControl |
                    TextFormatFlags.PreserveGraphicsClipping |
                    TextFormatFlags.PreserveGraphicsTranslateTransform;
                Rectangle rect = new(ClientRectangle.Location, ClientRectangle.Size);
                rect.Inflate(-2, -2);
                TextRenderer.DrawText(e.Graphics, Text, Font, rect, ForeColor, formatFlags);
            }
        }

        /// <summary>
        ///  Link label used by the DesignBindingPicker to display links.
        /// </summary>
        internal class BindingPickerLink : LinkLabel
        {
            /// <devdoc>
            ///  Allow "Return" as an input key (so it allows the link to fire, instead of closing the parent dropdown).
            /// </devdoc>
            protected override bool IsInputKey(Keys key)
            {
                return (key == Keys.Return) || base.IsInputKey(key);
            }

            private bool _showFocusCues;

            protected override bool ShowFocusCues => _showFocusCues;

            protected override void OnGotFocus(EventArgs e)
            {
                _showFocusCues = true;
                base.OnGotFocus(e);
            }

            protected override void OnLostFocus(EventArgs e)
            {
                _showFocusCues = false;
                base.OnLostFocus(e);
            }
        }

        /// <summary>
        ///  Tree view used by the DesignBindingPicker to display data sources and data members.
        /// </summary>
        internal class BindingPickerTree : TreeView
        {
            // ImageList containing default tree node images, of default unscaled size.
            protected internal static readonly ImageList s_defaultImages = CreateUnscaledDefaultImages();

            private static ImageList CreateUnscaledDefaultImages()
            {
                Bitmap images = new(typeof(DesignBindingPicker), "DataPickerImages.bmp");
                images.MakeTransparent(Color.Magenta);

                ImageList defaultImages = new()
                {
                    TransparentColor = Color.Magenta,
                    ColorDepth = ColorDepth.Depth24Bit
                };
                defaultImages.Images.AddStrip(images);

                return defaultImages;
            }

            private static ImageList CreateCopy(ImageList imageList)
            {
                ImageList copy = new()
                {
                    TransparentColor = Color.Magenta,
                    ColorDepth = ColorDepth.Depth24Bit
                };

                foreach (Image image in imageList.Images)
                {
                    copy.Images.Add(image);
                }

                return copy;
            }

            private static ImageList CreateScaledCopy(ImageList imageList, int dpi)
            {
                Size scaledSize = ScaleHelper.ScaleToDpi(imageList.ImageSize, dpi);

                ImageList copy = new()
                {
                    TransparentColor = Color.Magenta,
                    ColorDepth = ColorDepth.Depth24Bit,
                    ImageSize = scaledSize
                };

                foreach (Image image in imageList.Images)
                {
                    Bitmap scaledImage = ScaleHelper.CopyAndScaleToSize((Bitmap)image, scaledSize);
                    copy.Images.Add(scaledImage);
                }

                return copy;
            }

            // Cache of ImageList-s for each DPI to which this tree was scaled.
            // Cleared every time DesignBindingPicker dropdown is closed.
            // Every instance of BindingPickerTree has it's own cache,
            // but the basic set of images is shared, see s_defaultImages.
            private readonly Dictionary<int, ImageList> _imageListCacheByDPI = [];
            private int _dpi = ScaleHelper.OneHundredPercentLogicalDpi;

            internal BindingPickerTree()
            {
                ResetImages();
            }

            internal void ResetImages()
            {
                // reset current DPI to logical (96)
                _dpi = ScaleHelper.OneHundredPercentLogicalDpi;

                // Clear scaled images cache
                foreach (var imageList in _imageListCacheByDPI.Values)
                {
                    imageList.Dispose();
                }

                _imageListCacheByDPI.Clear();

                // Set new ImageList containing only unscaled default images
                ImageList?.Dispose();
                ImageList = CreateCopy(s_defaultImages);

                // Cache current ImageList instance as default for scaling
                _imageListCacheByDPI.Add(ScaleHelper.OneHundredPercentLogicalDpi, ImageList);
            }

            internal void RescaleImages(int dpi)
            {
                if (!IsHandleCreated)
                {
                    return;
                }

                if (dpi == _dpi)
                {
                    return;
                }

                _dpi = dpi;

                // Get ImageList from cache or create new one from unscaled
                if (!_imageListCacheByDPI.TryGetValue(dpi, out ImageList? scaledImageList))
                {
                    ImageList unscaledImageList = _imageListCacheByDPI[ScaleHelper.OneHundredPercentLogicalDpi];
                    scaledImageList = CreateScaledCopy(unscaledImageList, dpi);
                    _imageListCacheByDPI.Add(dpi, scaledImageList);
                }

                ImageList = scaledImageList;
            }

            internal int PreferredWidth
                => GetMaxItemWidth(Nodes);

            /// <summary>
            ///  Calculate the maximum width of the nodes in the collection recursively.
            ///  Only walks the existing set of expanded visible nodes. Does NOT expand
            ///  unexpanded nodes, since tree may contain endless cyclic relationships.
            /// </summary>
            private static int GetMaxItemWidth(TreeNodeCollection nodes)
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

            /// <summary>
            ///  Processes user selection of tree node. If node is selectable, notifies
            ///  node of selection, retrieves data source and data member info for the
            ///  caller, and closes the dropdown.
            /// </summary>
            public void SetSelectedItem(TreeNode? node)
            {
                if (Parent is not DesignBindingPicker picker)
                {
                    return;
                }

                var pickerNode = node as BindingPickerNode;
                picker._selectedItem = pickerNode is not null && pickerNode.CanSelect && pickerNode.Error is null
                    ? pickerNode.OnSelect()
                    : null;

                if (picker._selectedItem is not null)
                {
                    picker.CloseDropDown();
                }
            }

            /// <summary>
            ///  Process a mouse click on a node.
            ///
            ///  NOTE: Overriding OnAfterSelect() to handle selection changes is not sufficient because of a ComCtl32 quirk:
            ///  Clicking on the *current* selection does not trigger a selection change notification. And we need to support
            ///  re-selection of the current selection in certain scenarios. So instead of using OnAfterSelect(), we use
            ///  OnNodeMouseClick(), and use hit-testing to see whether the node's image or label were clicked.
            /// </summary>
            protected override void OnNodeMouseClick(TreeNodeMouseClickEventArgs e)
            {
                TreeViewHitTestInfo treeViewHitTestInfo = HitTest(new Point(e.X, e.Y));
                if (treeViewHitTestInfo.Node == e.Node &&
                    (treeViewHitTestInfo.Location == TreeViewHitTestLocations.Image ||
                     treeViewHitTestInfo.Location == TreeViewHitTestLocations.Label))
                {
                    SetSelectedItem(e.Node);
                }

                base.OnNodeMouseClick(e);
            }

            /// <summary>
            ///  Treat "Return" as a mouse click select of a node.
            /// </summary>
            protected override void OnKeyUp(KeyEventArgs e)
            {
                base.OnKeyUp(e);

                if (e.KeyData == Keys.Return && SelectedNode is not null)
                {
                    SetSelectedItem(SelectedNode);
                }
            }

            /// <summary>
            ///  Allow "Return" as an input key.
            /// </summary>
            protected override bool IsInputKey(Keys key)
            {
                return key == Keys.Return || base.IsInputKey(key);
            }
        }

        /// <summary>
        ///  Base class for all nodes in the tree view.
        /// </summary>
        internal class BindingPickerNode : TreeNode
        {
            private string? _error;
            private bool _subNodesFilled;
            protected DesignBindingPicker? _picker;

            public BindingPickerNode(DesignBindingPicker picker, string? nodeName) : base(nodeName)
            {
                _picker = picker;
            }

            public BindingPickerNode(DesignBindingPicker? picker, string nodeName, BindingImage index) : base(nodeName)
            {
                _picker = picker;
                BindingImageIndex = (int)index;
            }

            /// <devdoc>
            ///  Given a data source, return the corresponding BindingImageIndex.
            /// </devdoc>
            public static BindingImage BindingImageIndexForDataSource(object? dataSource)
            {
                if (dataSource is BindingSource)
                {
                    return BindingImage.BindingSource;
                }

                IListSource? ils = dataSource as IListSource;
                if (ils is not null)
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
            public virtual DesignBinding? OnSelect()
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
            public virtual string? Error
            {
                get
                {
                    return _error;
                }
                set
                {
                    _error = value;
                }
            }

            // Mouse-over help text for this node
            public virtual string? HelpText
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
                        ImageList.ImageCollection? images = _picker?._treeViewCtrl?.ImageList?.Images;
                        if (images is not null)
                        {
                            images.Add(value, Color.Transparent);
                            BindingImageIndex = images.Count - 1;
                        }
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
                    return _subNodesFilled;
                }
                set
                {
                    Debug.Assert(!_subNodesFilled && value, "we can only set this bit to true once");
                    _subNodesFilled = true;
                }
            }
        }

        /// <summary>
        ///  Node representing a data source.
        /// </summary>
        internal class DataSourceNode : BindingPickerNode
        {
            private readonly object? _dataSource;

            public DataSourceNode(DesignBindingPicker picker, object? dataSource, string? nodeName) : base(picker, nodeName)
            {
                _dataSource = dataSource;
                BindingImageIndex = (int)BindingImageIndexForDataSource(dataSource);
            }

            public object? DataSource
            {
                get
                {
                    return _dataSource;
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
                    return _picker is not null && !_picker._showDataMembers;
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

                    if (this is not DataMemberNode)
                        nodeType = "DS";
                    else if (this is DataMemberNode dataMemberNode && dataMemberNode.IsList)
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

        /// <summary>
        ///  Node representing a data member.
        ///  Note: Inherits from DataSourceNode, so be careful when trying to distinguish between these two types.
        /// </summary>

        internal class DataMemberNode : DataSourceNode
        {
            private readonly bool _isList;
            private readonly string _dataMember;

            public DataMemberNode(
                DesignBindingPicker picker,
                object? dataSource,
                string dataMember,
                string dataField,
                bool isList) : base(picker, dataSource, dataField)
            {
                _dataMember = dataMember;
                _isList = isList;
                BindingImageIndex = (int)(isList ? BindingImage.ListMember : BindingImage.FieldMember);
            }

            public string DataMember
            {
                get
                {
                    return _dataMember;
                }
            }

            // List member or field member?
            public bool IsList
            {
                get
                {
                    return _isList;
                }
            }

            public override void Fill()
            {
                _picker?.AddDataMemberContents(this);
            }

            public override DesignBinding OnSelect()
            {
                if (_picker is not null && _picker._showDataMembers)
                {
                    // Data member picking mode: Return data member info
                    return new DesignBinding(DataSource, DataMember);
                }
                else
                {
                    // Data source picking mode: Return data member wrapped in a BindingSource
                    BindingSource? newBindingSource = _picker?.CreateNewBindingSource(DataSource!, DataMember);
                    return (newBindingSource is null) ? DesignBinding.Null : new DesignBinding(newBindingSource, "");
                }
            }

            public override bool CanSelect
            {
                get
                {
                    // Only pick list members in 'list mode', field members in 'field mode'
                    return (_picker is not null && _picker._selectListMembers == IsList);
                }
            }
        }

        /// <summary>
        ///  Node representing the "None" choice.
        /// </summary>
        internal class NoneNode : BindingPickerNode
        {
            public NoneNode() : base(null, (SR.DesignBindingPickerNodeNone), BindingImage.None)
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
                    return (SR.DesignBindingPickerHelpNodeNone);
                }
            }
        }

        /// <summary>
        ///  Node representing the "Other Data Sources" branch.
        /// </summary>
        internal class OtherNode : BindingPickerNode
        {
            public OtherNode() : base(null, (SR.DesignBindingPickerNodeOther), BindingImage.Other)
            {
            }

            public override string HelpText
            {
                get
                {
                    return (SR.DesignBindingPickerHelpNodeOther);
                }
            }
        }

        /// <summary>
        ///  Node representing the "Form List Instances" branch.
        /// </summary>
        internal class InstancesNode : BindingPickerNode
        {
            public InstancesNode(string? rootComponentName) : base(null, string.Format(CultureInfo.CurrentCulture, (SR.DesignBindingPickerNodeInstances), rootComponentName), BindingImage.Instances)
            {
            }

            public override string HelpText
            {
                get
                {
                    return (SR.DesignBindingPickerHelpNodeInstances);
                }
            }
        }

        /// <summary>
        ///  Node representing the "Project Data Sources" branch.
        /// </summary>
        internal class ProjectNode : BindingPickerNode
        {
            public ProjectNode(DesignBindingPicker picker) : base(picker, (SR.DesignBindingPickerNodeProject), BindingImage.Project)
            {
            }

            public override string HelpText
            {
                get
                {
                    return (SR.DesignBindingPickerHelpNodeProject);
                }
            }
        }

        /// <summary>
        ///  Node representing a group of data sources under the "Project Data Sources" branch.
        /// </summary>
        internal class ProjectGroupNode : BindingPickerNode
        {
            public ProjectGroupNode(DesignBindingPicker picker, string nodeName, Image image) : base(picker, nodeName, BindingImage.Project)
            {
                if (image is not null)
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

        /// <summary>
        ///  Node representing a project level data source.
        ///  Note: dataSource is always a DataSourceDescriptor.
        /// </summary>
        internal class ProjectDataSourceNode : DataSourceNode
        {
            public ProjectDataSourceNode(DesignBindingPicker picker, object dataSource, string nodeName, Image image) : base(picker, dataSource, nodeName)
            {
                if (image is not null)
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
                _picker?.AddProjectDataSourceContents(this);
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

                DataSourceDescriptor? dataSourceDescriptor = DataSource as DataSourceDescriptor;
                ITypeDescriptorContext? context = _picker?._context;
                if (context is not null && context.Instance is BindingSource && dataSourceDescriptor is not null)
                {
                    object? newDataSource = _picker?.GetProjectDataSourceInstance(dataSourceDescriptor);
                    if (newDataSource is not null)
                    {
                        return new DesignBinding(newDataSource, "");
                    }
                    else
                    {
                        return DesignBinding.Null;
                    }
                }
                else
                {
                    if (dataSourceDescriptor is not null)
                    {
                        BindingSource? newBindingSource = _picker?.CreateNewBindingSource(dataSourceDescriptor, "");
                        return (newBindingSource is null) ? DesignBinding.Null : new DesignBinding(newBindingSource, "");
                    }

                    return DesignBinding.Null;
                }
            }
        }

        /// <summary>
        ///  Node representing a data member under a project level data source.
        ///  Note: dataSource is always a DataSourceDescriptor.
        /// </summary>
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

                ProjectDataMemberNode? parentListMember = (Parent as ProjectDataMemberNode);

                if (parentListMember is not null)
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

                if (DataSource is not DataSourceDescriptor dataSourceDescriptor)
                {
                    return DesignBinding.Null;
                }

                BindingSource? newBindingSource = _picker?.CreateNewBindingSource(dataSourceDescriptor, bindingSourceMember);
                return (newBindingSource is null) ? DesignBinding.Null : new DesignBinding(newBindingSource, designBindingMember);
            }
        }
    }
}
