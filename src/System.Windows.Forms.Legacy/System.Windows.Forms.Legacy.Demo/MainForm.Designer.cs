using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

#nullable disable

namespace Demo;

partial class MainForm
{
    private IContainer components = null;
    private Label _titleLabel = null!;
    private Label _descriptionLabel = null!;
    private Button _toolBarButton = null!;
    private Label _toolBarLabel = null!;
    private Button _menuStackButton = null!;
    private Label _menuStackDescriptionLabel = null!;
    private Button _dataGridButton = null!;
    private Label _dataGridDescriptionLabel = null!;
    private Button _statusBarButton = null!;
    private Label _statusBarDescriptionLabel = null!;

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
        _titleLabel = new Label();
        _descriptionLabel = new Label();
        _toolBarButton = new Button();
        _toolBarLabel = new Label();
        _menuStackButton = new Button();
        _menuStackDescriptionLabel = new Label();
        _dataGridButton = new Button();
        _dataGridDescriptionLabel = new Label();
        _statusBarButton = new Button();
        _statusBarDescriptionLabel = new Label();
        SuspendLayout();
        // 
        // _titleLabel
        // 
        _titleLabel.Font = new Font("Segoe UI", 14F, FontStyle.Bold, GraphicsUnit.Point);
        _titleLabel.Location = new Point(20, 20);
        _titleLabel.Name = "_titleLabel";
        _titleLabel.Size = new Size(680, 30);
        _titleLabel.TabIndex = 0;
        _titleLabel.Text = "WinForms Legacy Controls Demo";
        _titleLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _descriptionLabel
        // 
        _descriptionLabel.Location = new Point(20, 58);
        _descriptionLabel.Name = "_descriptionLabel";
        _descriptionLabel.Size = new Size(680, 42);
        _descriptionLabel.TabIndex = 1;
        _descriptionLabel.Text = "Use this launcher to validate legacy control surfaces under active migration. Menu Stack remains the main branch focus, while DataGrid and StatusBar cover separate recovery and regression paths.";
        _descriptionLabel.TextAlign = ContentAlignment.MiddleCenter;
        // 
        // _toolBarButton
        // 
        _toolBarButton.Location = new Point(40, 126);
        _toolBarButton.Name = "_toolBarButton";
        _toolBarButton.Size = new Size(300, 60);
        _toolBarButton.TabIndex = 2;
        _toolBarButton.Text = "ToolBar";
        _toolBarButton.Click += ToolBarButton_Click;
        // 
        // _toolBarLabel
        // 
        _toolBarLabel.Location = new Point(40, 190);
        _toolBarLabel.Name = "_toolBarLabel";
        _toolBarLabel.Size = new Size(300, 40);
        _toolBarLabel.TabIndex = 3;
        _toolBarLabel.Text = "Legacy command bars, drop-down menus, layout settings, and state-driven button behavior.";
        _toolBarLabel.TextAlign = ContentAlignment.TopCenter;
        // 
        // _menuStackButton
        // 
        _menuStackButton.Location = new Point(380, 126);
        _menuStackButton.Name = "_menuStackButton";
        _menuStackButton.Size = new Size(300, 60);
        _menuStackButton.TabIndex = 4;
        _menuStackButton.Text = "Menu Stack";
        _menuStackButton.Click += MenuStackButton_Click;
        // 
        // _menuStackDescriptionLabel
        // 
        _menuStackDescriptionLabel.Location = new Point(380, 190);
        _menuStackDescriptionLabel.Name = "_menuStackDescriptionLabel";
        _menuStackDescriptionLabel.Size = new Size(300, 40);
        _menuStackDescriptionLabel.TabIndex = 5;
        _menuStackDescriptionLabel.Text = "Nested menus, popup routing, shortcut processing, and stack behavior across the legacy menu surface.";
        _menuStackDescriptionLabel.TextAlign = ContentAlignment.TopCenter;
        // 
        // _dataGridButton
        // 
        _dataGridButton.Location = new Point(40, 252);
        _dataGridButton.Name = "_dataGridButton";
        _dataGridButton.Size = new Size(300, 60);
        _dataGridButton.TabIndex = 6;
        _dataGridButton.Text = "DataGrid";
        _dataGridButton.Click += DataGridButton_Click;
        // 
        // _dataGridDescriptionLabel
        // 
        _dataGridDescriptionLabel.Location = new Point(40, 316);
        _dataGridDescriptionLabel.Name = "_dataGridDescriptionLabel";
        _dataGridDescriptionLabel.Size = new Size(300, 40);
        _dataGridDescriptionLabel.TabIndex = 7;
        _dataGridDescriptionLabel.Text = "Legacy editing, navigation, and bound or unbound grid scenarios that still need targeted recovery coverage.";
        _dataGridDescriptionLabel.TextAlign = ContentAlignment.TopCenter;
        // 
        // _statusBarButton
        // 
        _statusBarButton.Location = new Point(380, 252);
        _statusBarButton.Name = "_statusBarButton";
        _statusBarButton.Size = new Size(300, 60);
        _statusBarButton.TabIndex = 8;
        _statusBarButton.Text = "StatusBar";
        _statusBarButton.Click += StatusBarButton_Click;
        // 
        // _statusBarDescriptionLabel
        // 
        _statusBarDescriptionLabel.Location = new Point(380, 316);
        _statusBarDescriptionLabel.Name = "_statusBarDescriptionLabel";
        _statusBarDescriptionLabel.Size = new Size(300, 40);
        _statusBarDescriptionLabel.TabIndex = 9;
        _statusBarDescriptionLabel.Text = "Simple text mode, panel layout, owner-draw rendering, border styles, and sizing grip behavior.";
        _statusBarDescriptionLabel.TextAlign = ContentAlignment.TopCenter;
        // 
        // MainForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(720, 380);
        Controls.Add(_statusBarDescriptionLabel);
        Controls.Add(_statusBarButton);
        Controls.Add(_toolBarLabel);
        Controls.Add(_toolBarButton);
        Controls.Add(_dataGridDescriptionLabel);
        Controls.Add(_dataGridButton);
        Controls.Add(_menuStackDescriptionLabel);
        Controls.Add(_menuStackButton);
        Controls.Add(_descriptionLabel);
        Controls.Add(_titleLabel);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Demo Launcher";
        ResumeLayout(false);
    }
}
