// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;

using DesignSurfaceExt;
using Timer = System.Windows.Forms.Timer;
using System.ComponentModel;

namespace TestConsole;

public partial class MainForm : Form
{
    private ISelectionService _selectionService;

    private readonly List<IDesignSurfaceExt> _listOfDesignSurface = [];

    public MainForm()
    {
        InitializeComponent();
    }

    private void InitFormDesigner()
    {
        CreateDesignSurface(1);
        CreateDesignSurface(2);
        CreateDesignSurface(3);
        CreateDesignSurface(4);
        CreateDesignSurface(5);
        CreateDesignSurface(6);

        tabPage1.Text = "Use SnapLines";
        tabPage2.Text = "Use Grid (Snap to the grid)";
        tabPage3.Text = "Use Grid";
        tabPage4.Text = "Align control by hand";
        tabPage5.Text = "TabControl and TableLayoutPanel";
        tabPage6.Text = "ToolStripContainer";

        // - enable the UndoEngines
        for (int i = 0; i < tabControl1.TabCount; i++)
        {
            IDesignSurfaceExt isurf = _listOfDesignSurface[i];
            isurf.GetUndoEngineExt().Enabled = true;
        }

        // - ISelectionService
        // - try to get a ptr to ISelectionService interface
        // - if we obtain it then hook the SelectionChanged event
        for (int i = 0; i < tabControl1.TabCount; i++)
        {
            IDesignSurfaceExt isurf = _listOfDesignSurface[i];
            _selectionService = (ISelectionService)(isurf.GetIDesignerHost().GetService(typeof(ISelectionService)));
            if (_selectionService is not null)
                _selectionService.SelectionChanged += OnSelectionChanged;
        }
    }

    // - When the selection changes this sets the PropertyGrid's selected component
    private void OnSelectionChanged(object sender, EventArgs e)
    {
        if (_selectionService is null)
            return;

        IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
        if (isurf is not null)
        {
            ISelectionService selectionService = isurf.GetIDesignerHost().GetService(typeof(ISelectionService)) as ISelectionService;
            propertyGrid.SelectedObject = selectionService.PrimarySelection;
        }
    }

    private void CreateDesignSurface(int n)
    {
        // - step.0
        // - create a DesignSurface and put it inside a Form in DesignTime
        DesignSurfaceExt.DesignSurfaceExt surface = new();
        // -
        // -
        // - store for later use
        _listOfDesignSurface.Add(surface);
        // -
        // -
        // - step.1
        // - choose an alignment mode...
        switch (n)
        {
            case 1:
                surface.UseSnapLines();
                break;
            case 2:
                surface.UseGrid(new Size(16, 16));
                break;
            case 3:
                surface.UseGridWithoutSnapping(new Size(32, 32));
                break;
            case 4:
                surface.UseNoGuides();
                break;
            case 5:
                surface.UseNoGuides();
                break;
            case 6:
                surface.UseNoGuides();
                break;
            default:
                Console.WriteLine("Invalid selection");
                break;
        }

        // -
        // -
        // - step.2
        // - create the Root component, in these cases a Form
        try
        {
            Form rootComponent = null;
            switch (n)
            {
                case 1:
                    {
                        rootComponent = surface.CreateRootComponent<Form>(new Size(400, 400));
                        rootComponent.BackColor = Color.Gray;
                        rootComponent.Text = "Root Component hosted by the DesignSurface N.1";
                        // - step.3
                        // - create some Controls at DesignTime
                        TextBox t1 = surface.CreateControl<TextBox>(new Size(200, 23), new Point(172, 12));
                        Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(172, 63));
                        CustomButton b2 = surface.CreateControl<CustomButton>(new Size(200, 40), new Point(172, 200));
                        b1.Text = "I'm the first Button";
                        b2.Text = "I'm the second Button";
                        b1.BackColor = Color.LightGray;
                        b2.BackColor = Color.LightGreen;

                        RadioButton rb1 = surface.CreateControl<RadioButton>(new Size(120, 22), new Point(12, 10));
                        rb1.Text = "Check me!";
                        RadioButton rb2 = surface.CreateControl<RadioButton>(new Size(120, 22), new Point(12, 35));
                        rb2.Text = "No, check me!";
                        rb2.Checked = true;

                        CheckBox checkbox1 = surface.CreateControl<CheckBox>(new Size(120, 22), new Point(12, 60));
                        checkbox1.Text = "I'm Unchecked!";
                        CheckBox checkbox2 = surface.CreateControl<CheckBox>(new Size(120, 22), new Point(12, 85));
                        checkbox2.Text = "I'm Indeterminate!";
                        checkbox2.AutoSize = true;
                        checkbox2.CheckState = CheckState.Indeterminate;
                        CheckBox checkbox3 = surface.CreateControl<CheckBox>(new Size(120, 22), new Point(12, 110));
                        checkbox3.Text = "I'm Checked!";
                        checkbox3.CheckState = CheckState.Checked;

                        Panel pnl = surface.CreateControl<Panel>(new Size(140, 140), new Point(12, 12));
                        pnl.BackColor = Color.Aquamarine;
                        rb1.Parent = pnl;
                        rb2.Parent = pnl;
                        checkbox1.Parent = pnl;
                        checkbox2.Parent = pnl;
                        checkbox3.Parent = pnl;

                        Label l1 = surface.CreateControl<Label>(new Size(100, 25), new Point(12, 12));
                        Label l2 = surface.CreateControl<Label>(new Size(120, 25), new Point(12, 12));
                        l1.Text = "I'm the first Label";
                        l2.Text = "I'm the second Label";
                        l1.BackColor = Color.Coral;
                        l2.BackColor = Color.LightGreen;

                        SplitContainer sct = surface.CreateControl<SplitContainer>(new Size(400, 100), new Point(0, 0));
                        sct.Dock = DockStyle.Bottom;
                        sct.BackColor = Color.White;
                        l1.Parent = sct.Panel1;
                        l2.Parent = sct.Panel2;

                        PictureBox pb1 = surface.CreateControl<PictureBox>(new Size(64, 64), new Point(12, 176));
                        pb1.Image = new Icon("painter.ico").ToBitmap();

                        ContextMenuStrip cm1 = surface.CreateComponent<ContextMenuStrip>();

                        surface.CreateControl<DateTimePicker>(new Size(200, 23), new Point(172, 150));
                    }

                    break;
                case 2:
                    {
                        rootComponent = surface.CreateRootComponent<Form>(new Size(640, 480));
                        rootComponent.BackColor = Color.Yellow;
                        rootComponent.Text = "Root Component hosted by the DesignSurface N.2";
                        // - step.3
                        // - create some Controls at DesignTime
                        TextBox t1 = surface.CreateControl<TextBox>(new Size(200, 20), new Point(10, 10));
                        Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 40));
                        Label l1 = surface.CreateControl<Label>(new Size(200, 100), new Point(10, 100));
                        t1.Text = "I'm a TextBox";
                        b1.Text = "I'm a Button";
                        b1.BackColor = Color.Coral;
                        l1.Text = "I'm a Label";
                        l1.BackColor = Color.Coral;

                        MaskedTextBox maskTextBox = surface.CreateControl<MaskedTextBox>(new Size(200, 20), new Point(260, 60));

                        ComboBox cb1 = surface.CreateControl<ComboBox>(new Size(200, 20), new Point(260, 16));
                        cb1.Items.AddRange(new string[] { "a1", "b2", "c3" });
                        cb1.SelectedIndex = 1;

                        ListBox lb1 = surface.CreateControl<ListBox>(new Size(200, 130), new Point(260, 100));
                        lb1.Items.AddRange(new string[] { "a1", "b2", "c3" });

                        TreeView tv1 = surface.CreateControl<TreeView>(new Size(200, 160), new Point(10, 220));
                    }

                    break;
                case 3:
                    {
                        rootComponent = surface.CreateRootComponent<Form>(new Size(800, 600));
                        rootComponent.BackColor = Color.YellowGreen;
                        rootComponent.Text = "Root Component hosted by the DesignSurface N.3";
                        // - step.3
                        // - create some Controls at DesignTime
                        Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 10));
                        Button b2 = surface.CreateControl<Button>(new Size(200, 40), new Point(100, 100));
                        Button b3 = surface.CreateControl<Button>(new Size(200, 40), new Point(22, 22));
                        b1.Text = "I'm the first Button";
                        b2.Text = "I'm the second Button";
                        b3.Text = "I'm the third Button (belonging to the GroupBox)";
                        GroupBox gb = surface.CreateControl<GroupBox>(new Size(300, 180), new Point(100, 200));
                        b3.Parent = gb;
                        b3.BackColor = Color.LightGray;

                        ListView lb1 = surface.CreateControl<ListView>(new Size(290, 160), new Point(320, 30));
                        ImageList im1 = surface.CreateComponent<ImageList>();
                    }

                    break;
                case 4:
                    {
                        rootComponent = surface.CreateRootComponent<Form>(new Size(800, 600));
                        rootComponent.BackColor = Color.Orange;
                        rootComponent.Text = "Root Component hosted by the DesignSurface N.4";       // - step.1
                                                                                                     // - step.3
                                                                                                     // - create some Controls at DesignTime
                        Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 10));
                        Button b2 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 60));
                        b1.Text = "I'm the first Button";
                        b2.Text = "I'm the second Button";
                        b1.BackColor = Color.Gold;
                        b2.BackColor = Color.LightGreen;

                        Timer tm11 = surface.CreateComponent<Timer>();
                        FontDialog fd1 = surface.CreateComponent<FontDialog>();
                        PrintDialog pd1 = surface.CreateComponent<PrintDialog>();

                        MonthCalendar monthCalendar1 = surface.CreateControl<MonthCalendar>(new Size(230, 170), new Point(10, 110));

                        Button subButton1OfLayoutPanel = surface.CreateControl<Button>(new Size(100, 40), new Point(10, 10));
                        Button subButton2OfLayoutPanel = surface.CreateControl<Button>(new Size(100, 40), new Point(10, 10));
                        FlowLayoutPanel layoutPanel = surface.CreateControl<FlowLayoutPanel>(new Size(430, 200), new Point(250, 10));
                        layoutPanel.Controls.Add(subButton1OfLayoutPanel);
                        layoutPanel.Controls.Add(subButton2OfLayoutPanel);

                        TrackBar trackBar = surface.CreateControl<TrackBar>(new Size(200, 50), new Point(250, 220));

                        FolderBrowserDialog folderBrowserDialog = surface.CreateComponent<FolderBrowserDialog>();
                        SaveFileDialog saveFileDialog = surface.CreateComponent<SaveFileDialog>();

                        ToolStripContainer toolStripContainer = surface.CreateControl<ToolStripContainer>(new Size(200, 180), new Point(250, 280));
                    }

                    break;
                case 5:
                    {
                        rootComponent = surface.CreateRootComponent<Form>(new Size(800, 600));
                        rootComponent.BackColor = Color.Orange;
                        rootComponent.Text = "Root Component hosted by the DesignSurface N.5";

                        surface.CreateControl<TabControl>(new Size(400, 100), new Point(12, 21));
                        surface.CreateControl<TableLayoutPanel>(new Size(290, 160), new Point(20, 150));
                        surface.CreateControl<PropertyGrid>(new Size(200, 150), new Point(430, 23));
                        surface.CreateComponent<NotifyIcon>();

                        ListBox listBox = surface.CreateControl<ListBox>(new Size(120, 94), new Point(337, 217));
                        BindingSource bindingSource = surface.CreateComponent<BindingSource>();
                        bindingSource.DataSource = new List<string> { "a1", "b2", "c3", "d4", "e5", "f6" };
                        listBox.DataSource = bindingSource;
                        DataGridView dataGridView = surface.CreateControl<DataGridView>(new Size(200, 150), new Point(470, 220));
                        DataGridViewComboBoxColumn comboBoxColumn = surface.CreateComponent<DataGridViewComboBoxColumn>();
                        comboBoxColumn.HeaderText = "Column1";
                        dataGridView.Columns.AddRange([comboBoxColumn]);
                    }

                    break;
                case 6:
                    {
                        rootComponent = surface.CreateRootComponent<Form>(new Size(800, 600));
                        rootComponent.BackColor = Color.Pink;
                        rootComponent.Text = "Root Component hosted by the DesignSurface N.6";

                        ToolStripContainer toolStripContainer = surface.CreateControl<ToolStripContainer>(new Size(800, 200), new Point(0, 0));
                        toolStripContainer.Dock = DockStyle.Fill;

                        MenuStrip menuStrip1 = new();
                        MenuStrip menuStrip2 = new();

                        ToolStripMenuItem toolStripMenuItem1 = new("TopMenuItem1");
                        ToolStripMenuItem toolStripMenuItem2 = new("TopMenuItem2");
                        ToolStripMenuItem menu1 = new("BottomMenuItem1");
                        ToolStripMenuItem menuNew1 = new("BottomMenuItem2");

                        menuStrip1.Items.Add(toolStripMenuItem1);
                        menuStrip1.Items.Add(toolStripMenuItem2);
                        menuStrip2.Items.Add(menu1);
                        menuStrip2.Items.Add(menuNew1);

                        toolStripMenuItem1.DropDownItems.Add("DropDownItem1");
                        toolStripMenuItem2.DropDownItems.Add("DropDownItem12");

                        ToolStripPanel topToolStripPanel = surface.CreateControl<ToolStripPanel>(new(50, 50), new(0, 0));
                        topToolStripPanel = toolStripContainer.TopToolStripPanel;
                        topToolStripPanel.Join(menuStrip1);
                        ToolStripPanel bottomToolStripPanel = surface.CreateControl<ToolStripPanel>(new(50, 50), new(0, 0));
                        bottomToolStripPanel = toolStripContainer.BottomToolStripPanel;
                        bottomToolStripPanel.Join(menuStrip2);

                        SplitContainer splitContainer = surface.CreateControl<SplitContainer>(new(0, 0), new(0, 0));
                        splitContainer.Dock = DockStyle.Fill;
                        splitContainer.BackColor = Color.Red;

                        RichTextBox richTextBox = surface.CreateControl<RichTextBox>(new Size(0, 0), new Point(0, 0));
                        richTextBox.Dock = DockStyle.Fill;
                        richTextBox.Width = toolStripContainer.Width;
                        richTextBox.Text = "I'm a RichTextBox";

                        MyUserControl userControl = surface.CreateControl<MyUserControl>(new Size(0, 0), new Point(0, 0));
                        userControl.Dock = DockStyle.Fill;
                        userControl.BackColor = Color.LightSkyBlue;

                        MyScrollableControl scrollableControl = surface.CreateControl<MyScrollableControl>(new Size(0, 0), new Point(0, 0));
                        scrollableControl.Dock = DockStyle.Fill;
                        scrollableControl.InjectControl(userControl);

                        SplitterPanel splitterPanel1 = splitContainer.Panel1;
                        SplitterPanel splitterPanel2 = splitContainer.Panel2;
                        splitterPanel1.Controls.Add(richTextBox);
                        splitterPanel2.Controls.Add(scrollableControl);

                        toolStripContainer.ContentPanel.Controls.AddRange(splitContainer);

                        Component component = surface.CreateComponent<Component>();

                        Splitter splitter = surface.CreateControl<Splitter>(new(5, 0), new(0, 0));
                        splitter.BackColor = Color.Green;
                        splitter.Dock = DockStyle.Bottom;

                        Panel panel = surface.CreateControl<Panel>(new(0, tabPage6.Height / 2), new(0, 0));
                        panel.Dock = DockStyle.Bottom;
                        NumericUpDown numericUpDown = surface.CreateControl<NumericUpDown>(new(50, 10), new(10, 10));
                        panel.Controls.Add(numericUpDown);

                        BindingNavigator bindingNavigator = surface.CreateControl<BindingNavigator>(new(0, 0), new(0, 0));

                        BindingSource bindingSource = new()
                        {
                            DataSource = new List<string> { "Item 1", "Item 2", "Item 3" }
                        };

                        bindingNavigator.Dock = DockStyle.Bottom;
                        bindingNavigator.BindingSource = bindingSource;

                        richTextBox.DataBindings.Add(new Binding("Text", bindingSource, "Text", true, DataSourceUpdateMode.OnPropertyChanged));

                        panel.Controls.Add(bindingNavigator);
                    }

                    break;

                default:
                    Console.WriteLine("Invalid selection");
                    break;
            }

            // -
            // -
            // - step.4
            // - display the DesignSurface
            Control view = surface.GetView();
            if (view is null)
                return;
            // - change some properties
            view.Text = $"Test Form N. {n}";
            view.Dock = DockStyle.Fill;
            // - Note these assignments
            switch (n)
            {
                case 1:
                    view.Parent = tabPage1;
                    break;
                case 2:
                    view.Parent = tabPage2;
                    break;
                case 3:
                    view.Parent = tabPage3;
                    break;
                case 4:
                    view.Parent = tabPage4;
                    break;
                case 5:
                    view.Parent = tabPage5;
                    break;
                case 6:
                    view.Parent = tabPage6;
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    break;
            }
        }
        catch (Exception)
        {
            Console.WriteLine($"{Name} the DesignSurface N. {n} has generated errors during loading!");
            return;
        }
    }

    private void SelectRootComponent()
    {
        // - find out the DesignSurfaceExt control hosted by the TabPage
        IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
        if (isurf is not null)
        {
            splitContainer.Panel2.Controls.Remove(propertyGrid);
            propertyGrid.Dispose();
            propertyGrid = new()
            {
                DesignerHost = isurf.GetIDesignerHost(),
                Dock = DockStyle.Fill,
                Location = new Point(0, 0),
                Margin = new Padding(4),
                Name = "propertyGrid",
                Size = new Size(226, 502),
                TabIndex = 0,
                SelectedObject = isurf.GetIDesignerHost().RootComponent
            };

            splitContainer.Panel2.Controls.Add(propertyGrid);
        }
    }

    private void undoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
        isurf?.GetUndoEngineExt().Undo();
    }

    private void redoToolStripMenuItem_Click(object sender, EventArgs e)
    {
        IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
        isurf?.GetUndoEngineExt().Redo();
    }

    private void OnAbout(object sender, EventArgs e)
    {
        MessageBox.Show("Tiny Form Designer coded by Paolo Foti", "Tiny Form Designer", MessageBoxButtons.OK, MessageBoxIcon.Question);
    }

    private void toolStripMenuItemTabOrder_Click(object sender, EventArgs e)
    {
        // - find out the DesignSurfaceExt control hosted by the TabPage
        IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
        isurf?.SwitchTabOrder();
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        InitFormDesigner();

        tabControl1.Selected += OnTabPageSelected;

        // - select into the propertygrid the current Form
        SelectRootComponent();
    }

    private void OnTabPageSelected(object sender, TabControlEventArgs e)
    {
        SelectRootComponent();
    }

    private void OnMenuClick(object sender, EventArgs e)
    {
        IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
        isurf?.DoAction((sender as ToolStripMenuItem).Text);
    }
}
