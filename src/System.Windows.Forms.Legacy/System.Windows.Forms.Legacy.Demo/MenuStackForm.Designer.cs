using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable

namespace Demo;

partial class MenuStackForm
{
    private IContainer components = null;
    private Label _summaryLabel = null!;
    private Button _showSurfaceContextMenuButton = null!;
    private Button _clearLogButton = null!;
    private Panel _demoSurface = null!;
    private Label _surfaceMessageLabel = null!;
    private TreeView _menuTreeView = null!;
    private DataGrid _demoDataGrid = null!;
    private Label _treeViewLabel = null!;
    private Label _logLabel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new Container();
        _summaryLabel = new Label();
        _showSurfaceContextMenuButton = new Button();
        _clearLogButton = new Button();
        _demoSurface = new Panel();
        _surfaceMessageLabel = new Label();
        _menuTreeView = new TreeView();
        _demoDataGrid = new DataGrid();
        _treeViewLabel = new Label();
        _logLabel = new Label();
        _demoSurface.SuspendLayout();
        SuspendLayout();
        // 
        // _summaryLabel
        // 
        _summaryLabel.Location = new Point(18, 16);
        _summaryLabel.Name = "_summaryLabel";
        _summaryLabel.Size = new Size(774, 40);
        _summaryLabel.TabIndex = 0;
        _summaryLabel.Text = "Menu stack demo exercising the legacy MainMenu, MenuItem, ContextMenu, dynamic popups, owner-draw items, and TreeNode.ContextMenu.";
        // 
        // _showSurfaceContextMenuButton
        // 
        _showSurfaceContextMenuButton.Location = new Point(18, 65);
        _showSurfaceContextMenuButton.Name = "_showSurfaceContextMenuButton";
        _showSurfaceContextMenuButton.Size = new Size(188, 30);
        _showSurfaceContextMenuButton.TabIndex = 1;
        _showSurfaceContextMenuButton.Text = "Show Surface Context Menu";
        _showSurfaceContextMenuButton.Click += ShowSurfaceContextMenuButton_Click;
        // 
        // _clearLogButton
        // 
        _clearLogButton.Location = new Point(218, 65);
        _clearLogButton.Name = "_clearLogButton";
        _clearLogButton.Size = new Size(120, 30);
        _clearLogButton.TabIndex = 2;
        _clearLogButton.Text = "Clear Log";
        _clearLogButton.Click += ClearLogButton_Click;
        // 
        // _demoSurface
        // 
        _demoSurface.BackColor = Color.WhiteSmoke;
        _demoSurface.BorderStyle = BorderStyle.FixedSingle;
        _demoSurface.Controls.Add(_surfaceMessageLabel);
        _demoSurface.Location = new Point(18, 112);
        _demoSurface.Name = "_demoSurface";
        _demoSurface.Size = new Size(384, 220);
        _demoSurface.TabIndex = 5;
        // 
        // _surfaceMessageLabel
        // 
        _surfaceMessageLabel.Dock = DockStyle.Fill;
        _surfaceMessageLabel.Location = new Point(0, 0);
        _surfaceMessageLabel.Name = "_surfaceMessageLabel";
        _surfaceMessageLabel.Size = new Size(382, 218);
        _surfaceMessageLabel.TabIndex = 0;
        _surfaceMessageLabel.Text = "Right-click this surface or the TreeView nodes to exercise ContextMenu and TreeNode.ContextMenu support. Use the MainMenu above for menu actions.";
        _surfaceMessageLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _menuTreeView
        // 
        _menuTreeView.HideSelection = false;
        _menuTreeView.Location = new Point(424, 134);
        _menuTreeView.Name = "_menuTreeView";
        _menuTreeView.Size = new Size(368, 198);
        _menuTreeView.TabIndex = 7;
        _menuTreeView.NodeMouseClick += MenuTreeView_NodeMouseClick;
        // 
        // _demoDataGrid
        // 
        _demoDataGrid.CaptionVisible = false;
        _demoDataGrid.Location = new Point(18, 368);
        _demoDataGrid.Name = "_demoDataGrid";
        _demoDataGrid.ReadOnly = true;
        _demoDataGrid.Size = new Size(774, 184);
        _demoDataGrid.TabIndex = 9;
        // 
        // _treeViewLabel
        // 
        _treeViewLabel.Location = new Point(424, 112);
        _treeViewLabel.Name = "_treeViewLabel";
        _treeViewLabel.Size = new Size(210, 18);
        _treeViewLabel.TabIndex = 6;
        _treeViewLabel.Text = "TreeView and TreeNode.ContextMenu demo";
        // 
        // _logLabel
        // 
        _logLabel.Location = new Point(18, 346);
        _logLabel.Name = "_logLabel";
        _logLabel.Size = new Size(600, 18);
        _logLabel.TabIndex = 8;
        _logLabel.Text = "DataGrid context menu repro — right-click a row for dynamic ContextMenu (submenus show <PlaceHolder> without the fix)";
        // 
        // MenuStackForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(810, 572);
        Controls.Add(_logLabel);
        Controls.Add(_treeViewLabel);
        Controls.Add(_demoDataGrid);
        Controls.Add(_menuTreeView);
        Controls.Add(_demoSurface);
        Controls.Add(_clearLogButton);
        Controls.Add(_showSurfaceContextMenuButton);
        Controls.Add(_summaryLabel);
        MinimumSize = new Size(826, 611);
        Name = "MenuStackForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Menu Stack";
        _demoSurface.ResumeLayout(false);
        ResumeLayout(false);
    }
}