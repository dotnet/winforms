using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;

using DesignSurfaceExt;

namespace TestConsole
{
    public partial class MainForm : Form
    {
        ISelectionService _selectionService;

        private List<IDesignSurfaceExt> _listOfDesignSurface = new List<IDesignSurfaceExt>();

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

            tabPage1.Text = "Use SnapLines";
            tabPage2.Text = "Use Grid (Snap to the grid)";
            tabPage3.Text = "Use Grid";
            tabPage4.Text = "Align control by hand";

            //- enable the UndoEngines
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                IDesignSurfaceExt isurf = _listOfDesignSurface[i];
                isurf.GetUndoEngineExt().Enabled = true;
            }

            //- ISelectionService
            //- try to get a ptr to ISelectionService interface
            //- if we obtain it then hook the SelectionChanged event
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                IDesignSurfaceExt isurf = _listOfDesignSurface[i];
                _selectionService = (ISelectionService)(isurf.GetIDesignerHost().GetService(typeof(ISelectionService)));
                if (null != _selectionService)
                    _selectionService.SelectionChanged += new System.EventHandler(OnSelectionChanged);
            }
        }

        //- When the selection changes this sets the PropertyGrid's selected component
        private void OnSelectionChanged(object sender, System.EventArgs e)
        {
            if (_selectionService == null)
                return;

            IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
            if (null != isurf)
            {
                ISelectionService selectionService = isurf.GetIDesignerHost().GetService(typeof(ISelectionService)) as ISelectionService;
                propertyGrid.SelectedObject = selectionService.PrimarySelection;
            }
        }

        private void CreateDesignSurface(int n)
        {
            //- step.0
            //- create a DesignSurface and put it inside a Form in DesignTime
            DesignSurfaceExt.DesignSurfaceExt surface = new DesignSurfaceExt.DesignSurfaceExt();
            //-
            //-
            //- store for later use
            _listOfDesignSurface.Add(surface);
            //-
            //-
            //- step.1
            //- choose an alignment mode...
            switch (n)
            {
                case 1:
                    surface.UseSnapLines();
                    break;
                case 2:
                    surface.UseGrid(new System.Drawing.Size(16, 16));
                    break;
                case 3:
                    surface.UseGridWithoutSnapping(new System.Drawing.Size(32, 32));
                    break;
                case 4:
                    surface.UseNoGuides();
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    break;
            }

            //-
            //-
            //- step.2
            //- create the Root component, in these cases a Form
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
                            //- step.3
                            //- create some Controls at DesignTime
                            TextBox t1 = surface.CreateControl<TextBox>(new Size(200, 23), new Point(172, 12));
                            Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(172, 63));
                            CustomButton b2 = surface.CreateControl<CustomButton>(new Size(200, 40), new Point(100, 200));
                            b1.Text = "I'm the first Button";
                            b2.Text = "I'm the second Button";
                            b1.BackColor = Color.LightGray;
                            b2.BackColor = Color.LightGreen;

                            RadioButton rb1 = surface.CreateControl<RadioButton>(new Size(120, 22), new Point(12, 21));
                            rb1.Text = "Check me!";
                            RadioButton rb2 = surface.CreateControl<RadioButton>(new Size(120, 22), new Point(12, 50));
                            rb2.Text = "No, check me!";
                            rb2.Checked = true;

                            Panel pnl = surface.CreateControl<Panel>(new Size(130, 100), new Point(12, 21));
                            pnl.BackColor = Color.Aquamarine;
                            rb1.Parent = pnl;
                            rb2.Parent = pnl;

                            PictureBox pb1 = surface.CreateControl<PictureBox>(new Size(64, 64), new Point(24, 166));
                            pb1.Image = new Icon("painter.ico").ToBitmap();

                            ContextMenuStrip cm1 = surface.CreateComponent<ContextMenuStrip>();
                        }

                        break;
                    case 2:
                        {
                            rootComponent = surface.CreateRootComponent<Form>(new Size(640, 480));
                            rootComponent.BackColor = Color.Yellow;
                            rootComponent.Text = "Root Component hosted by the DesignSurface N.2";
                            //- step.3
                            //- create some Controls at DesignTime
                            TextBox t1 = surface.CreateControl<TextBox>(new Size(200, 20), new Point(10, 10));
                            Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 40));
                            Label l1 = surface.CreateControl<Label>(new Size(200, 120), new Point(48, 100));
                            t1.Text = "I'm a TextBox";
                            b1.Text = "I'm a Button";
                            b1.BackColor = Color.Coral;
                            l1.Text = "I'm a Label";
                            l1.BackColor = Color.Coral;

                            ComboBox cb1 = surface.CreateControl<ComboBox>(new Size(200, 20), new Point(288, 16));
                            cb1.Items.AddRange(new string[] { "a1", "b2", "c3" });
                            cb1.SelectedIndex = 1;

                            ListBox lb1 = surface.CreateControl<ListBox>(new Size(200, 130), new Point(288, 100));
                            lb1.Items.AddRange(new string[] { "a1", "b2", "c3" });

                            TreeView tv1 = surface.CreateControl<TreeView>(new Size(290, 160), new Point(20, 220));
                        }

                        break;
                    case 3:
                        {
                            rootComponent = surface.CreateRootComponent<Form>(new Size(800, 600));
                            rootComponent.BackColor = Color.YellowGreen;
                            rootComponent.Text = "Root Component hosted by the DesignSurface N.3";
                            //- step.3
                            //- create some Controls at DesignTime
                            Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 10));
                            Button b2 = surface.CreateControl<Button>(new Size(200, 40), new Point(100, 100));
                            Button b3 = surface.CreateControl<Button>(new Size(200, 40), new Point(22, 22));
                            b1.Text = "I'm the first Button";
                            b2.Text = "I'm the second Button";
                            b3.Text = "I'm the third Button (belonging to the BroupBox)";
                            GroupBox gb = surface.CreateControl<GroupBox>(new Size(300, 180), new Point(100, 200));
                            b3.Parent = gb;
                            b3.BackColor = Color.LightGray;

                            ListView lb1 = surface.CreateControl<ListView>(new Size(290, 160), new Point(320, 30));
                            ImageList im1 = surface.CreateComponent<ImageList>();
                        }

                        break;
                    case 4:
                        {
                            rootComponent = surface.CreateRootComponent<Form>(new Size(320, 200));
                            rootComponent.BackColor = Color.Orange;
                            rootComponent.Text = "Root Component hosted by the DesignSurface N.4";       //- step.1
                                                                                                         //- step.3
                                                                                                         //- create some Controls at DesignTime
                            Button b1 = surface.CreateControl<Button>(new Size(200, 40), new Point(10, 10));
                            Button b2 = surface.CreateControl<Button>(new Size(200, 40), new Point(100, 100));
                            b1.Text = "I'm the first Button";
                            b2.Text = "I'm the second Button";
                            b1.BackColor = Color.Gold;
                            b2.BackColor = Color.LightGreen;

                            Timer tm11 = surface.CreateComponent<Timer>();
                            FontDialog fd1 = surface.CreateComponent<FontDialog>();
                        }

                        break;
                    default:
                        Console.WriteLine("Invalid selection");
                        break;
                }

                //-
                //-
                //- step.4
                //- display the DesignSurface
                Control view = surface.GetView();
                if (null == view)
                    return;
                //- change some properties
                view.Text = "Test Form N. " + n.ToString();
                view.Dock = DockStyle.Fill;
                //- Note these assignments
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
                    default:
                        Console.WriteLine("Invalid selection");
                        break;
                }
            }//end_try
            catch (Exception)
            {
                Console.WriteLine(Name + " the DesignSurface N. " + n.ToString() + " has generated errors during loading!");
                return;
            }//end_catch
        }

        private void SelectRootComponent()
        {
            //- find out the DesignSurfaceExt control hosted by the TabPage
            IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
            if (null != isurf)
                propertyGrid.SelectedObject = isurf.GetIDesignerHost().RootComponent;
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
            if (null != isurf)
                isurf.GetUndoEngineExt().Undo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
            if (null != isurf)
                isurf.GetUndoEngineExt().Redo();
        }

        private void OnAbout(object sender, EventArgs e)
        {
            MessageBox.Show("Tiny Form Designer coded by Paolo Foti", "Tiny Form Designer", MessageBoxButtons.OK, MessageBoxIcon.Question);
        }

        private void toolStripMenuItemTabOrder_Click(object sender, EventArgs e)
        {
            //- find out the DesignSurfaceExt control hosted by the TabPage
            IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
            if (null != isurf)
                isurf.SwitchTabOrder();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            InitFormDesigner();

            tabControl1.Selected += new System.Windows.Forms.TabControlEventHandler(OnTabPageSelected);

            //- select into the propertygrid the current Form
            SelectRootComponent();
        }

        private void OnTabPageSelected(object sender, TabControlEventArgs e)
        {
            SelectRootComponent();
        }

        private void OnMenuClick(object sender, EventArgs e)
        {
            IDesignSurfaceExt isurf = _listOfDesignSurface[tabControl1.SelectedIndex];
            if (null != isurf)
                isurf.DoAction((sender as ToolStripMenuItem).Text);
        }
    }
}
