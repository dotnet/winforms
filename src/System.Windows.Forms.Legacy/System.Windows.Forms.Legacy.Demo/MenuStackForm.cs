using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Demo;

public partial class MenuStackForm : Form
{
    private readonly ContextMenu _surfaceContextMenu;
    private readonly ContextMenu _treeViewContextMenu;
    private int _dynamicMenuGeneration;
    private int _documentsMenuGeneration;
    private readonly DataTable _logDataTable = new DataTable("EventLog");
    private ContextMenu _gridContextMenu = null!;

    public MenuStackForm()
    {
        InitializeComponent();

        Menu = CreateMainMenu();

        _surfaceContextMenu = CreateSurfaceContextMenu();
        _demoSurface.ContextMenu = _surfaceContextMenu;

        _treeViewContextMenu = CreateTreeViewContextMenu();
        _menuTreeView.ContextMenu = _treeViewContextMenu;

        InitializeTreeView();
        SetUpLogDataGrid();
        AppendLog("Menu stack demo ready.");
    }

    private MainMenu CreateMainMenu()
    {
        MenuItem fileMenuItem = new("File");
        MenuItem newMenuItem = new("New");
        MenuItem newProjectItem = new("Project...", OnMenuActionClicked);
        MenuItem newRepositoryItem = new("Repository...", OnMenuActionClicked);
        MenuItem newFileItem = new("File...", OnMenuActionClicked);
        MenuItem openMenuItem = new("Open", OnMenuActionClicked)
        {
            Shortcut = Shortcut.AltF12,
            ShowShortcut = true
        };
        MenuItem saveMenuItem = new("Save", OnMenuActionClicked, Shortcut.CtrlS)
        {
            Checked = true,
            RadioCheck = true
        };
        MenuItem exitMenuItem = new("Close Demo", (_, _) => Close());

        newMenuItem.MenuItems.Add(newProjectItem);
        newMenuItem.MenuItems.Add(newRepositoryItem);
        newMenuItem.MenuItems.Add(newFileItem);

        fileMenuItem.MenuItems.Add(newMenuItem);
        fileMenuItem.MenuItems.Add(openMenuItem);
        fileMenuItem.MenuItems.Add(saveMenuItem);
        fileMenuItem.MenuItems.Add(new MenuItem("-"));
        fileMenuItem.MenuItems.Add(exitMenuItem);

        MenuItem viewMenuItem = new("View");
        viewMenuItem.MenuItems.Add(new MenuItem("Toolbox", OnMenuActionClicked));
        viewMenuItem.MenuItems.Add(new MenuItem("Terminal", OnMenuActionClicked));
        viewMenuItem.MenuItems.Add(new MenuItem("Output", OnMenuActionClicked));

        MenuItem dynamicMenuItem = new("Dynamic");
        dynamicMenuItem.Popup += DynamicMenuItem_Popup;
        dynamicMenuItem.MenuItems.Add(new MenuItem("Dynamic code has not run yet"));

        MenuItem ownerDrawMenuItem = new("Owner Draw Demo");
        ownerDrawMenuItem.MenuItems.Add(new MenuItem("Standard Item", OnMenuActionClicked));
        ownerDrawMenuItem.MenuItems.Add(new MenuItem("-"));
        ownerDrawMenuItem.MenuItems.Add(CreateOwnerDrawMenuItem("Custom Draw Item 1"));
        ownerDrawMenuItem.MenuItems.Add(CreateOwnerDrawMenuItem("Custom Draw Item 2", createNestedItems: true));

        MenuItem contextActionsItem = new("Context Actions");
        contextActionsItem.MenuItems.Add(new MenuItem("Show Surface Menu", ShowSurfaceMenuMenuItem_Click));
        contextActionsItem.MenuItems.Add(new MenuItem("Clear Log", (_, _) => { _logDataTable.Rows.Clear(); AppendLog("Log cleared."); }));

        AttachMenuTracing(fileMenuItem);
        AttachMenuTracing(viewMenuItem);
        AttachMenuTracing(dynamicMenuItem);
        AttachMenuTracing(ownerDrawMenuItem);
        AttachMenuTracing(contextActionsItem);

        MainMenu mainMenu = new();
        mainMenu.MenuItems.Add(fileMenuItem);
        mainMenu.MenuItems.Add(viewMenuItem);
        mainMenu.MenuItems.Add(dynamicMenuItem);
        mainMenu.MenuItems.Add(ownerDrawMenuItem);
        mainMenu.MenuItems.Add(contextActionsItem);

        return mainMenu;
    }

    private ContextMenu CreateSurfaceContextMenu()
    {
        ContextMenu contextMenu = new();

        MenuItem inspectItem = new("Inspect Surface", OnContextMenuActionClicked);
        MenuItem toggleHighlightItem = new("Toggle Highlight", ToggleSurfaceHighlightMenuItem_Click);
        MenuItem timestampItem = new("Insert Timestamp", InsertTimestampMenuItem_Click);

        contextMenu.MenuItems.Add(inspectItem);
        contextMenu.MenuItems.Add(toggleHighlightItem);
        contextMenu.MenuItems.Add(new MenuItem("-"));
        contextMenu.MenuItems.Add(timestampItem);

        contextMenu.Popup += (_, _) => AppendLog("Surface context menu opened.");

        return contextMenu;
    }

    private ContextMenu CreateTreeViewContextMenu()
    {
        ContextMenu contextMenu = new();
        MenuItem inspectTreeItem = new("Inspect TreeView", (_, _) => AppendLog("Inspect TreeView context action executed."));
        MenuItem expandAllItem = new("Expand All Nodes", (_, _) =>
        {
            _menuTreeView.ExpandAll();
            AppendLog("TreeView context action: expand all nodes.");
        });
        MenuItem collapseAllItem = new("Collapse All Nodes", (_, _) =>
        {
            _menuTreeView.CollapseAll();
            AppendLog("TreeView context action: collapse all nodes.");
        });

        contextMenu.MenuItems.Add(inspectTreeItem);
        contextMenu.MenuItems.Add(new MenuItem("-"));
        contextMenu.MenuItems.Add(expandAllItem);
        contextMenu.MenuItems.Add(collapseAllItem);
        contextMenu.Popup += (_, _) => AppendLog("TreeView context menu opened.");

        return contextMenu;
    }

    private OwnerDrawMenuItem CreateOwnerDrawMenuItem(string text, bool createNestedItems = false)
    {
        OwnerDrawMenuItem item = new()
        {
            Text = text
        };

        item.Click += OnOwnerDrawItemClicked;

        if (!createNestedItems)
        {
            OwnerDrawMenuItem subItem1 = new()
            {
                Text = "Submenu Item 1-1"
            };
            OwnerDrawMenuItem subItem2 = new()
            {
                Text = "Submenu Item 1-2"
            };

            subItem1.Click += OnOwnerDrawItemClicked;
            subItem2.Click += OnOwnerDrawItemClicked;

            item.MenuItems.Add(subItem1);
            item.MenuItems.Add(subItem2);

            return item;
        }

        OwnerDrawMenuItem nestedItemA = new()
        {
            Text = "Nested Custom Item A"
        };
        OwnerDrawMenuItem nestedItemB = new()
        {
            Text = "Nested Custom Item B"
        };
        MenuItem deepSubmenu = new("Deep Submenu");
        OwnerDrawMenuItem deepItem1 = new()
        {
            Text = "Deep Custom Item 1"
        };
        OwnerDrawMenuItem deepItem2 = new()
        {
            Text = "Deep Custom Item 2"
        };

        nestedItemA.Click += OnOwnerDrawItemClicked;
        nestedItemB.Click += OnOwnerDrawItemClicked;
        deepItem1.Click += OnOwnerDrawItemClicked;
        deepItem2.Click += OnOwnerDrawItemClicked;

        deepSubmenu.MenuItems.Add(deepItem1);
        deepSubmenu.MenuItems.Add(deepItem2);

        item.MenuItems.Add(nestedItemA);
        item.MenuItems.Add(nestedItemB);
        item.MenuItems.Add(new MenuItem("-"));
        item.MenuItems.Add(deepSubmenu);

        return item;
    }

    private void AttachMenuTracing(MenuItem rootItem)
    {
        rootItem.Select += (_, _) => AppendLog($"Selected menu item: {rootItem.Text}");

        if (rootItem.MenuItems.Count > 0)
        {
            rootItem.Popup += (_, _) => AppendLog($"Popup menu opened: {rootItem.Text}");
        }

        foreach (MenuItem childItem in rootItem.MenuItems)
        {
            AttachMenuTracing(childItem);
        }
    }

    private void InitializeTreeView()
    {
        TreeNode rootNode = new("Menu Demo Root")
        {
            Nodes =
            {
                new TreeNode("Context Menu Node"),
                new TreeNode("Owner Draw Node")
                {
                    Nodes =
                    {
                        new TreeNode("Nested Node A"),
                        new TreeNode("Nested Node B")
                    }
                },
                new TreeNode("Shortcut Node")
            }
        };

        _menuTreeView.Nodes.Clear();
        _menuTreeView.Nodes.Add(rootNode);
        rootNode.Expand();

        AddContextMenusToNodes(_menuTreeView.Nodes);
    }

    private void AddContextMenusToNodes(TreeNodeCollection nodes)
    {
        foreach (TreeNode node in nodes)
        {
            ContextMenu nodeContextMenu = new();
            MenuItem inspectNodeItem = new($"Inspect {node.Text}", (_, _) =>
            {
                _surfaceMessageLabel.Text = "Inspecting node: " + node.Text;
                AppendLog($"TreeNode.ContextMenu action: inspect {node.Text}");
            });
            MenuItem toggleCheckedItem = new($"Toggle check state for {node.Text}", (_, _) =>
            {
                node.Checked = !node.Checked;
                AppendLog($"TreeNode.ContextMenu action: {(node.Checked ? "checked" : "unchecked")} {node.Text}");
            });
            MenuItem runNodeItem = new($"Run action for {node.Text}", (_, _) => MessageBox.Show(this, $"Action executed for {node.Text}", Text));

            nodeContextMenu.MenuItems.Add(inspectNodeItem);
            nodeContextMenu.MenuItems.Add(toggleCheckedItem);
            nodeContextMenu.MenuItems.Add(runNodeItem);
            nodeContextMenu.Popup += (_, _) => AppendLog($"TreeNode.ContextMenu opened for {node.Text}");
            node.ContextMenu = nodeContextMenu;

            if (node.Nodes.Count > 0)
            {
                AddContextMenusToNodes(node.Nodes);
            }
        }
    }

    private void DynamicMenuItem_Popup(object? sender, EventArgs e)
    {
        if (sender is not MenuItem dynamicMenuItem)
        {
            return;
        }

        _dynamicMenuGeneration++;
        dynamicMenuItem.MenuItems.Clear();

        for (int i = 1; i <= 5; i++)
        {
            MenuItem item = new($"Dynamic Item {_dynamicMenuGeneration}.{i}");
            item.Click += OnMenuActionClicked;
            dynamicMenuItem.MenuItems.Add(item);
        }

        AppendLog($"Dynamic menu rebuilt #{_dynamicMenuGeneration}.");
    }

    private void OnMenuActionClicked(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        menuItem.Checked = !menuItem.Checked && menuItem.Text.EndsWith("...", StringComparison.Ordinal);
        AppendLog($"Menu click: {menuItem.Text}");
        MessageBox.Show(this, $"{menuItem.Text} clicked.", Text);
    }

    private void OnOwnerDrawItemClicked(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        AppendLog($"Owner-draw click: {menuItem.Text}");
        MessageBox.Show(this, $"Owner-draw item clicked: {menuItem.Text}", Text);
    }

    private void OnContextMenuActionClicked(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        AppendLog($"Context action: {menuItem.Text}");
        MessageBox.Show(this, $"{menuItem.Text} executed.", Text);
    }

    private void ToggleSurfaceHighlightMenuItem_Click(object? sender, EventArgs e)
    {
        bool useHighlight = _demoSurface.BackColor != Color.LightGoldenrodYellow;
        _demoSurface.BackColor = useHighlight ? Color.LightGoldenrodYellow : Color.WhiteSmoke;

        AppendLog(useHighlight ? "Surface highlight enabled." : "Surface highlight cleared.");
    }

    private void InsertTimestampMenuItem_Click(object? sender, EventArgs e)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        _surfaceMessageLabel.Text = "Last context action at " + timestamp;
        AppendLog("Timestamp inserted: " + timestamp);
    }

    private void ShowSurfaceMenuMenuItem_Click(object? sender, EventArgs e)
    {
        ShowSurfaceContextMenu();
    }

    private void ShowSurfaceContextMenuButton_Click(object? sender, EventArgs e)
    {
        ShowSurfaceContextMenu();
    }

    private void ClearLogButton_Click(object? sender, EventArgs e)
    {
        _logDataTable.Rows.Clear();
        AppendLog("Log cleared.");
    }

    private void MenuTreeView_NodeMouseClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node is null)
        {
            return;
        }

        _menuTreeView.SelectedNode = e.Node;
        AppendLog($"Tree node clicked: {e.Node.Text}");
    }

    private void ShowSurfaceContextMenu()
    {
        Point screenPoint = _demoSurface.PointToClient(MousePosition);
        Point menuPoint = screenPoint.X >= 0 && screenPoint.Y >= 0
            ? screenPoint
            : new Point(_demoSurface.Width / 2, _demoSurface.Height / 2);

        _surfaceContextMenu.Show(_demoSurface, menuPoint);
    }

    private void SetUpLogDataGrid()
    {
        _logDataTable.Columns.Add(new DataColumn("Time", typeof(string)));
        _logDataTable.Columns.Add(new DataColumn("Event", typeof(string)));

        DataGridTableStyle tableStyle = new() { MappingName = "EventLog" };
        tableStyle.GridColumnStyles.Add(new DataGridTextBoxColumn { MappingName = "Time", HeaderText = "Time", Width = 72, ReadOnly = true });
        tableStyle.GridColumnStyles.Add(new DataGridTextBoxColumn { MappingName = "Event", HeaderText = "Event", Width = 680, ReadOnly = true });
        _demoDataGrid.TableStyles.Add(tableStyle);
        _demoDataGrid.DataSource = _logDataTable;

        _gridContextMenu = CreateGridContextMenu();
        _demoDataGrid.ContextMenu = _gridContextMenu;
    }

    // Repro for WM_INITMENUPOPUP not firing on a non-Form control (DataGrid):
    // Right-click the grid and hover any dynamic submenu. Without the fix in Control.WmInitMenuPopup,
    // WM_INITMENUPOPUP is never dispatched to ContextMenu.ProcessInitMenuPopup, so Popup never fires
    // and the submenu always shows "<PlaceHolder>" — exactly as seen in the product.
    private ContextMenu CreateGridContextMenu()
    {
        ContextMenu contextMenu = new();

        contextMenu.MenuItems.Add(new MenuItem("View", OnGridContextMenuActionClicked));
        contextMenu.MenuItems.Add(new MenuItem("Copy Row", OnGridContextMenuActionClicked));
        contextMenu.MenuItems.Add(new MenuItem("-"));
        contextMenu.MenuItems.Add(new MenuItem("Customize Columns", OnGridContextMenuActionClicked));
        contextMenu.MenuItems.Add(new MenuItem("-"));

        MenuItem actionsItem = new("Actions");
        actionsItem.Popup += ActionsMenuItem_Popup;
        actionsItem.MenuItems.Add(new MenuItem("<PlaceHolder>"));
        contextMenu.MenuItems.Add(actionsItem);

        MenuItem tasksItem = new("Tasks");
        tasksItem.Popup += TasksMenuItem_Popup;
        tasksItem.MenuItems.Add(new MenuItem("<PlaceHolder>"));
        contextMenu.MenuItems.Add(tasksItem);

        MenuItem supportTasksItem = new("Support's Tasks");
        supportTasksItem.Popup += SupportTasksMenuItem_Popup;
        supportTasksItem.MenuItems.Add(new MenuItem("<PlaceHolder>"));
        contextMenu.MenuItems.Add(supportTasksItem);

        // Primary repro target: hover "Documents" and the Popup event should replace <PlaceHolder>
        // with real items. Without Control.WmInitMenuPopup dispatching the message, it stays as-is.
        MenuItem documentsItem = new("Documents");
        documentsItem.Popup += DocumentsMenuItem_Popup;
        documentsItem.MenuItems.Add(new MenuItem("<PlaceHolder>"));
        contextMenu.MenuItems.Add(documentsItem);

        return contextMenu;
    }

    private void ActionsMenuItem_Popup(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        menuItem.MenuItems.Clear();
        menuItem.MenuItems.Add(new MenuItem("Clear Log", (_, _) => { _logDataTable.Rows.Clear(); AppendLog("Log cleared via context menu."); }));
        menuItem.MenuItems.Add(new MenuItem("Export Log", OnGridContextMenuActionClicked));
        AppendLog("Actions submenu rebuilt.");
    }

    private void TasksMenuItem_Popup(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        menuItem.MenuItems.Clear();
        menuItem.MenuItems.Add(new MenuItem("My Tasks", OnGridContextMenuActionClicked));
        menuItem.MenuItems.Add(new MenuItem("All Tasks", OnGridContextMenuActionClicked));
        AppendLog("Tasks submenu rebuilt.");
    }

    private void SupportTasksMenuItem_Popup(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        menuItem.MenuItems.Clear();
        menuItem.MenuItems.Add(new MenuItem("Open Support Ticket", OnGridContextMenuActionClicked));
        menuItem.MenuItems.Add(new MenuItem("View Support History", OnGridContextMenuActionClicked));
        AppendLog("Support's Tasks submenu rebuilt.");
    }

    private void DocumentsMenuItem_Popup(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        _documentsMenuGeneration++;
        menuItem.MenuItems.Clear();
        menuItem.MenuItems.Add(new MenuItem($"Document {_documentsMenuGeneration}.1", OnGridContextMenuActionClicked));
        menuItem.MenuItems.Add(new MenuItem($"Document {_documentsMenuGeneration}.2", OnGridContextMenuActionClicked));
        menuItem.MenuItems.Add(new MenuItem($"Document {_documentsMenuGeneration}.3", OnGridContextMenuActionClicked));
        AppendLog($"Documents submenu rebuilt #{_documentsMenuGeneration}.");
    }

    private void OnGridContextMenuActionClicked(object? sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem)
        {
            return;
        }

        AppendLog($"Grid context action: {menuItem.Text}");
    }

    private void AppendLog(string message)
    {
        DataRow row = _logDataTable.NewRow();
        row["Time"] = DateTime.Now.ToString("HH:mm:ss");
        row["Event"] = message;
        _logDataTable.Rows.InsertAt(row, 0);

        if (_logDataTable.Rows.Count > 200)
        {
            _logDataTable.Rows.RemoveAt(_logDataTable.Rows.Count - 1);
        }
    }
}
