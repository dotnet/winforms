// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Provides an editor that can be used to visually select and configure the link area of a link
    /// label.
    /// </summary>
    internal class LinkAreaEditor : UITypeEditor
    {
        private LinkAreaUI _linkAreaUI;

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider is null)
            {
                return value;
            }

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc is null)
            {
                return value;
            }

            if (_linkAreaUI is null)
            {
                IHelpService helpService = (IHelpService)provider.GetService(typeof(IHelpService));

                // child modal dialog -launching in System Aware mode
                _linkAreaUI = DpiHelper.CreateInstanceInSystemAwareContext(() => new LinkAreaUI(this, helpService));
            }

            string text = string.Empty;
            PropertyDescriptor prop = null;

            if (context != null && context.Instance != null)
            {
                prop = TypeDescriptor.GetProperties(context.Instance)["Text"];
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    text = (string)prop.GetValue(context.Instance);
                }
            }

            string originalText = text;
            _linkAreaUI.SampleText = text;
            _linkAreaUI.Start(edSvc, value);

            if (edSvc.ShowDialog(_linkAreaUI) == DialogResult.OK)
            {
                value = _linkAreaUI.Value;

                text = _linkAreaUI.SampleText;
                if (!originalText.Equals(text) && prop != null && prop.PropertyType == typeof(string))
                {
                    prop.SetValue(context.Instance, text);
                }
            }

            _linkAreaUI.End();

            return value;
        }

        /// <summary>
        /// Gets the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;

        /// <summary>
        ///      Dialog box for the link area.
        /// </summary>
        internal class LinkAreaUI : Form
        {
            private Label _caption = new Label();
            private TextBox _sampleEdit = new TextBox();
            private Button _okButton = new Button();
            private Button _cancelButton = new Button();
            private TableLayoutPanel _okCancelTableLayoutPanel;
            private readonly LinkAreaEditor _editor;
            private IWindowsFormsEditorService _edSvc;
            private readonly IHelpService _helpService;

            public LinkAreaUI(LinkAreaEditor editor, IHelpService helpService)
            {
                _editor = editor;
                _helpService = helpService;
                InitializeComponent();
            }

            public string SampleText
            {
                get
                {
                    return _sampleEdit.Text;
                }
                set
                {
                    _sampleEdit.Text = value;
                    UpdateSelection();
                }
            }

            public object Value { get; private set; }

            public void End()
            {
                _edSvc = null;
                Value = null;
            }

            private void InitializeComponent()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(LinkAreaEditor));
                _caption = new Label();
                _sampleEdit = new TextBox();
                _okButton = new Button();
                _cancelButton = new Button();
                _okCancelTableLayoutPanel = new TableLayoutPanel();
                _okCancelTableLayoutPanel.SuspendLayout();
                SuspendLayout();
                _okButton.Click += new EventHandler(okButton_click);
                //
                // caption
                //
                resources.ApplyResources(_caption, "caption");
                _caption.Margin = new Padding(3, 1, 3, 0);
                _caption.Name = "caption";
                //
                // sampleEdit
                //
                resources.ApplyResources(_sampleEdit, "sampleEdit");
                _sampleEdit.Margin = new Padding(3, 2, 3, 3);
                _sampleEdit.Name = "sampleEdit";
                _sampleEdit.HideSelection = false;
                _sampleEdit.ScrollBars = ScrollBars.Vertical;
                //
                // okButton
                //
                resources.ApplyResources(_okButton, "okButton");
                _okButton.DialogResult = DialogResult.OK;
                _okButton.Margin = new Padding(0, 0, 2, 0);
                _okButton.Name = "okButton";
                //
                // cancelButton
                //
                resources.ApplyResources(_cancelButton, "cancelButton");
                _cancelButton.DialogResult = DialogResult.Cancel;
                _cancelButton.Margin = new Padding(3, 0, 0, 0);
                _cancelButton.Name = "cancelButton";
                //
                // okCancelTableLayoutPanel
                //
                resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
                _okCancelTableLayoutPanel.ColumnCount = 2;
                _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
                _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
                _okCancelTableLayoutPanel.Margin = new Padding(3, 1, 3, 3);
                _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
                _okCancelTableLayoutPanel.RowCount = 1;
                _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
                _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
                //
                // LinkAreaEditor
                //
                resources.ApplyResources(this, "$this");
                AutoScaleMode = AutoScaleMode.Font;
                CancelButton = _cancelButton;
                Controls.Add(_okCancelTableLayoutPanel);
                Controls.Add(_sampleEdit);
                Controls.Add(_caption);
                HelpButton = true;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "LinkAreaEditor";
                ShowIcon = false;
                ShowInTaskbar = false;
                HelpButtonClicked += new CancelEventHandler(LinkAreaEditor_HelpButtonClicked);
                _okCancelTableLayoutPanel.ResumeLayout(false);
                _okCancelTableLayoutPanel.PerformLayout();
                ResumeLayout(false);
                PerformLayout();
            }

            private void okButton_click(object sender, EventArgs e)
            {
                Value = new LinkArea(_sampleEdit.SelectionStart, _sampleEdit.SelectionLength);
            }

            private string HelpTopic => "net.ComponentModel.LinkAreaEditor";

            /// <summary>
            /// Called when the help button is clicked.
            /// </summary>
            private void ShowHelp()
                => _helpService?.ShowHelpFromKeyword(HelpTopic);

            private void LinkAreaEditor_HelpButtonClicked(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                ShowHelp();
            }

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                _edSvc = edSvc;
                Value = value;
                UpdateSelection();
                ActiveControl = _sampleEdit;
            }

            private void UpdateSelection()
            {
                if (!(Value is LinkArea))
                {
                    return;
                }

                LinkArea pt = (LinkArea)Value;
                try
                {
                    _sampleEdit.SelectionStart = pt.Start;
                    _sampleEdit.SelectionLength = pt.Length;
                }
                catch (Exception ex)
                {
                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                }
            }
        }
    }
}

