// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;

namespace System.Windows.Forms.Design

{
    internal class DataGridViewCellStyleBuilder : Form
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private PropertyGrid cellStyleProperties;
        private GroupBox previewGroupBox;
        private Button okButton;
        private Button cancelButton;
        private Label label1;
        private DataGridView listenerDataGridView;
        private DataGridView sampleDataGridView;
        private DataGridView sampleDataGridViewSelected;
        private TableLayoutPanel sampleViewTableLayoutPanel;
        private TableLayoutPanel okCancelTableLayoutPanel;
        private TableLayoutPanel overarchingTableLayoutPanel;
        private TableLayoutPanel sampleViewGridsTableLayoutPanel;
        private Container components = null;
        private Label normalLabel = null;
        private Label selectedLabel = null;
        private IHelpService helpService = null;
        private IComponent comp = null;
        private IServiceProvider serviceProvider = null;

        private DataGridViewCellStyle cellStyle;
        private ITypeDescriptorContext context = null;

        public DataGridViewCellStyleBuilder(IServiceProvider serviceProvider, IComponent comp)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
            //
            // Adds columns and rows to the grid, also resizes them
            //
            InitializeGrids();

            listenerDataGridView = new System.Windows.Forms.DataGridView();
            this.serviceProvider = serviceProvider;
            this.comp = comp;

            if (this.serviceProvider != null)
            {
                helpService = (IHelpService)serviceProvider.GetService(typeof(IHelpService));
            }

            cellStyleProperties.Site = new DataGridViewComponentPropertyGridSite(serviceProvider, comp);
        }

        private void InitializeGrids()
        {
            sampleDataGridViewSelected.Size = new System.Drawing.Size(100, Font.Height + 9);
            sampleDataGridView.Size = new System.Drawing.Size(100, Font.Height + 9);
            sampleDataGridView.AccessibilityObject.Name = SR.CellStyleBuilderNormalPreviewAccName;

            DataGridViewRow row = new DataGridViewRow();
            row.Cells.Add(new DialogDataGridViewCell());
            row.Cells[0].Value = "####";
            row.Cells[0].AccessibilityObject.Name = SR.CellStyleBuilderSelectedPreviewAccName;

            sampleDataGridViewSelected.Columns.Add(new DataGridViewTextBoxColumn());
            sampleDataGridViewSelected.Rows.Add(row);
            sampleDataGridViewSelected.Rows[0].Selected = true;
            sampleDataGridViewSelected.AccessibilityObject.Name = SR.CellStyleBuilderSelectedPreviewAccName;


            row = new DataGridViewRow();
            row.Cells.Add(new DialogDataGridViewCell());
            row.Cells[0].Value = "####";
            row.Cells[0].AccessibilityObject.Name = SR.CellStyleBuilderNormalPreviewAccName;

            sampleDataGridView.Columns.Add(new DataGridViewTextBoxColumn());
            sampleDataGridView.Rows.Add(row);
        }

        public DataGridViewCellStyle CellStyle
        {
            get
            {
                return cellStyle;
            }
            set
            {
                cellStyle = new DataGridViewCellStyle(value);
                cellStyleProperties.SelectedObject = cellStyle;
                ListenerDataGridViewDefaultCellStyleChanged(null, EventArgs.Empty);
                listenerDataGridView.DefaultCellStyle = cellStyle;
                listenerDataGridView.DefaultCellStyleChanged += new EventHandler(ListenerDataGridViewDefaultCellStyleChanged);
            }
        }

        public ITypeDescriptorContext Context
        {
            set
            {
                context = value;
            }
        }

        private void ListenerDataGridViewDefaultCellStyleChanged(object sender, EventArgs e)
        {
            DataGridViewCellStyle cellStyleTmp = new DataGridViewCellStyle(cellStyle);
            sampleDataGridView.DefaultCellStyle = cellStyleTmp;
            sampleDataGridViewSelected.DefaultCellStyle = cellStyleTmp;
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            ComponentResourceManager resources = new ComponentResourceManager(typeof(DataGridViewCellStyleBuilder));
            cellStyleProperties = new PropertyGrid();
            sampleViewTableLayoutPanel = new TableLayoutPanel();
            sampleViewGridsTableLayoutPanel = new TableLayoutPanel();
            normalLabel = new Label();
            sampleDataGridView = new DataGridView();
            selectedLabel = new Label();
            sampleDataGridViewSelected = new DataGridView();
            label1 = new Label();
            okButton = new Button();
            cancelButton = new Button();
            okCancelTableLayoutPanel = new TableLayoutPanel();
            previewGroupBox = new GroupBox();
            overarchingTableLayoutPanel = new TableLayoutPanel();
            sampleViewTableLayoutPanel.SuspendLayout();
            sampleViewGridsTableLayoutPanel.SuspendLayout();
            ((ISupportInitialize)(sampleDataGridView)).BeginInit();
            ((ISupportInitialize)(sampleDataGridViewSelected)).BeginInit();
            okCancelTableLayoutPanel.SuspendLayout();
            previewGroupBox.SuspendLayout();
            overarchingTableLayoutPanel.SuspendLayout();
            SuspendLayout();
            // 
            // cellStyleProperties
            // 
            resources.ApplyResources(cellStyleProperties, "cellStyleProperties");

            // Linecolor assigned here is causing issues in the HC mode. Going with runtime default for HC mode.
            if (!SystemInformation.HighContrast)
            {
                cellStyleProperties.LineColor = Drawing.SystemColors.ScrollBar;
            }

            cellStyleProperties.Margin = new Padding(0, 0, 0, 3);
            cellStyleProperties.Name = "cellStyleProperties";
            cellStyleProperties.ToolbarVisible = false;
            // 
            // sampleViewTableLayoutPanel
            // 
            resources.ApplyResources(sampleViewTableLayoutPanel, "sampleViewTableLayoutPanel");
            sampleViewTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 423F));
            sampleViewTableLayoutPanel.Controls.Add(sampleViewGridsTableLayoutPanel, 0, 1);
            sampleViewTableLayoutPanel.Controls.Add(label1, 0, 0);
            sampleViewTableLayoutPanel.Name = "sampleViewTableLayoutPanel";
            sampleViewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            sampleViewTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // sampleViewGridsTableLayoutPanel
            // 
            resources.ApplyResources(sampleViewGridsTableLayoutPanel, "sampleViewGridsTableLayoutPanel");
            sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            sampleViewGridsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 10F));
            sampleViewGridsTableLayoutPanel.Controls.Add(normalLabel, 1, 0);
            sampleViewGridsTableLayoutPanel.Controls.Add(sampleDataGridView, 1, 1);
            sampleViewGridsTableLayoutPanel.Controls.Add(selectedLabel, 3, 0);
            sampleViewGridsTableLayoutPanel.Controls.Add(sampleDataGridViewSelected, 3, 1);
            sampleViewGridsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            sampleViewGridsTableLayoutPanel.Name = "sampleViewGridsTableLayoutPanel";
            sampleViewGridsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            sampleViewGridsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // normalLabel
            // 
            resources.ApplyResources(normalLabel, "normalLabel");
            normalLabel.Margin = new Padding(0);
            normalLabel.Name = "normalLabel";
            // 
            // sampleDataGridView
            // 
            sampleDataGridView.AllowUserToAddRows = false;
            resources.ApplyResources(sampleDataGridView, "sampleDataGridView");
            sampleDataGridView.ColumnHeadersVisible = false;
            sampleDataGridView.Margin = new Padding(0);
            sampleDataGridView.Name = "sampleDataGridView";
            sampleDataGridView.ReadOnly = true;
            sampleDataGridView.RowHeadersVisible = false;
            sampleDataGridView.CellStateChanged += new DataGridViewCellStateChangedEventHandler(sampleDataGridView_CellStateChanged);
            // 
            // selectedLabel
            // 
            resources.ApplyResources(selectedLabel, "selectedLabel");
            selectedLabel.Margin = new Padding(0);
            selectedLabel.Name = "selectedLabel";
            // 
            // sampleDataGridViewSelected
            // 
            sampleDataGridViewSelected.AllowUserToAddRows = false;
            resources.ApplyResources(sampleDataGridViewSelected, "sampleDataGridViewSelected");
            sampleDataGridViewSelected.ColumnHeadersVisible = false;
            sampleDataGridViewSelected.Margin = new Padding(0);
            sampleDataGridViewSelected.Name = "sampleDataGridViewSelected";
            sampleDataGridViewSelected.ReadOnly = true;
            sampleDataGridViewSelected.RowHeadersVisible = false;
            // 
            // label1
            // 
            resources.ApplyResources(label1, "label1");
            label1.Margin = new Padding(0, 0, 0, 3);
            label1.Name = "label1";
            // 
            // okButton
            // 
            resources.ApplyResources(okButton, "okButton");
            okButton.DialogResult = DialogResult.OK;
            okButton.Margin = new Padding(0, 0, 3, 0);
            okButton.Name = "okButton";
            // 
            // cancelButton
            // 
            resources.ApplyResources(cancelButton, "cancelButton");
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Margin = new Padding(3, 0, 0, 0);
            cancelButton.Name = "cancelButton";
            // 
            // okCancelTableLayoutPanel
            // 
            resources.ApplyResources(okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            okCancelTableLayoutPanel.Controls.Add(okButton, 0, 0);
            okCancelTableLayoutPanel.Controls.Add(cancelButton, 1, 0);
            okCancelTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 3, 0, 0);
            okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            okCancelTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // previewGroupBox
            // 
            resources.ApplyResources(previewGroupBox, "previewGroupBox");
            previewGroupBox.Controls.Add(sampleViewTableLayoutPanel);
            previewGroupBox.Margin = new Padding(0, 3, 0, 3);
            previewGroupBox.Name = "previewGroupBox";
            previewGroupBox.TabStop = false;
            // 
            // overarchingTableLayoutPanel
            // 
            resources.ApplyResources(overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
            overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            overarchingTableLayoutPanel.Controls.Add(cellStyleProperties, 0, 0);
            overarchingTableLayoutPanel.Controls.Add(okCancelTableLayoutPanel, 0, 2);
            overarchingTableLayoutPanel.Controls.Add(previewGroupBox, 0, 1);
            overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            // 
            // DataGridViewCellStyleBuilder
            // 
            resources.ApplyResources(this, "$this");
            AutoScaleDimensions = new Drawing.SizeF(6F, 13F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(overarchingTableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DataGridViewCellStyleBuilder";
            ShowIcon = false;
            ShowInTaskbar = false;
            HelpButtonClicked += new System.ComponentModel.CancelEventHandler(DataGridViewCellStyleBuilder_HelpButtonClicked);
            HelpRequested += new System.Windows.Forms.HelpEventHandler(DataGridViewCellStyleBuilder_HelpRequested);
            Load += new System.EventHandler(DataGridViewCellStyleBuilder_Load);
            sampleViewTableLayoutPanel.ResumeLayout(false);
            sampleViewTableLayoutPanel.PerformLayout();
            sampleViewGridsTableLayoutPanel.ResumeLayout(false);
            sampleViewGridsTableLayoutPanel.PerformLayout();
            ((ISupportInitialize)(sampleDataGridView)).EndInit();
            ((ISupportInitialize)(sampleDataGridViewSelected)).EndInit();
            okCancelTableLayoutPanel.ResumeLayout(false);
            okCancelTableLayoutPanel.PerformLayout();
            previewGroupBox.ResumeLayout(false);
            previewGroupBox.PerformLayout();
            overarchingTableLayoutPanel.ResumeLayout(false);
            overarchingTableLayoutPanel.PerformLayout();
            ResumeLayout(false);
        }
        #endregion

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.Modifiers) == 0 && (keyData & Keys.KeyCode) == Keys.Escape)
            {
                Close();
                return true;
            }
            else
            {
                return base.ProcessDialogKey(keyData);
            }
        }

        private void DataGridViewCellStyleBuilder_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            DataGridViewCellStyleBuilder_HelpRequestHandled();
        }

        private void DataGridViewCellStyleBuilder_HelpRequested(object sender, System.Windows.Forms.HelpEventArgs e)
        {
            e.Handled = true;
            DataGridViewCellStyleBuilder_HelpRequestHandled();
        }

        private void DataGridViewCellStyleBuilder_HelpRequestHandled()
        {
            IHelpService helpService = context.GetService(typeof(IHelpService)) as IHelpService;
            if (helpService != null)
            {
                helpService.ShowHelpFromKeyword("vs.CellStyleDialog");
            }
        }

        private void DataGridViewCellStyleBuilder_Load(object sender, System.EventArgs e)
        {
            // The cell inside the sampleDataGridView should not be selected.
            sampleDataGridView.ClearSelection();

            // make sure that the cell inside the sampleDataGridView and sampleDataGridViewSelected fill their
            // respective dataGridView's
            sampleDataGridView.Rows[0].Height = sampleDataGridView.Height;
            sampleDataGridView.Columns[0].Width = sampleDataGridView.Width;

            sampleDataGridViewSelected.Rows[0].Height = sampleDataGridViewSelected.Height;
            sampleDataGridViewSelected.Columns[0].Width = sampleDataGridViewSelected.Width;

            // sync the Layout event for both sample DataGridView's
            // so that when the sample DataGridView's are laid out we know to change the size of their cells
            sampleDataGridView.Layout += new System.Windows.Forms.LayoutEventHandler(sampleDataGridView_Layout);
            sampleDataGridViewSelected.Layout += new System.Windows.Forms.LayoutEventHandler(sampleDataGridView_Layout);
        }

        private void sampleDataGridView_CellStateChanged(object sender, System.Windows.Forms.DataGridViewCellStateChangedEventArgs e)
        {
            Debug.Assert(e.Cell == sampleDataGridView.Rows[0].Cells[0], "the sample data grid view has only one cell");
            Debug.Assert(sender == sampleDataGridView, "did we forget to unhook notification");
            if ((e.StateChanged & DataGridViewElementStates.Selected) != 0 && (e.Cell.State & DataGridViewElementStates.Selected) != 0)
            {
                // The cell inside the sample data grid view became selected.
                // We don't want that to happen
                sampleDataGridView.ClearSelection();
            }
        }

        private void sampleDataGridView_Layout(object sender, System.Windows.Forms.LayoutEventArgs e)
        {
            DataGridView dataGridView = (DataGridView)sender;
            dataGridView.Rows[0].Height = dataGridView.Height;
            dataGridView.Columns[0].Width = dataGridView.Width;
        }

        private class DialogDataGridViewCell : DataGridViewTextBoxCell
        {
            DialogDataGridViewCellAccessibleObject accObj = null;
            protected override AccessibleObject CreateAccessibilityInstance()
            {
                if (accObj == null)
                {
                    accObj = new DialogDataGridViewCellAccessibleObject(this);
                }

                return accObj;
            }

            private class DialogDataGridViewCellAccessibleObject : DataGridViewCell.DataGridViewCellAccessibleObject
            {
                public DialogDataGridViewCellAccessibleObject(DataGridViewCell owner) : base(owner)
                {
                }

                string name = "";
                public override string Name
                {
                    get
                    {
                        return name;
                    }
                    set
                    {
                        name = value;
                    }

                }
            }
        }
    }
}
