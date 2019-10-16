// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  Generic editor for editing binary data.  This presents a hex editing window to the user.
    /// </summary>
    public sealed class BinaryEditor : UITypeEditor
    {
        private const string HELP_KEYWORD = "System.ComponentModel.Design.BinaryEditor";
        private ITypeDescriptorContext _context;
        private BinaryUI _binaryUI;

        internal object GetService(Type serviceType)
        {
            if (_context == null)
            {
                return null;
            }

            IDesignerHost host = _context.GetService(typeof(IDesignerHost)) as IDesignerHost;
            if (host == null)
            {
                return _context.GetService(serviceType);
            }

            return host.GetService(serviceType);
        }

        /// <summary>
        ///  Converts the given object to an array of bytes to be manipulated
        ///  by the editor.  The default implementation of this supports
        ///  byte[] and stream objects.
        /// </summary>
        internal byte[] ConvertToBytes(object value)
        {
            if (value is Stream s)
            {
                s.Position = 0;
                int byteCount = (int)(s.Length - s.Position);
                byte[] bytes = new byte[byteCount];
                s.Read(bytes, 0, byteCount);
                return bytes;
            }

            if (value is byte[] b)
            {
                return b;
            }

            if (value is string str)
            {
                int size = str.Length * 2;
                byte[] buffer = new byte[size];
                Encoding.Unicode.GetBytes(str.ToCharArray(), 0, size / 2, buffer, 0);
                return buffer;
            }

            Debug.Fail("No conversion from " + value == null ? "null" : value.GetType().FullName + " to byte[]");
            return null;
        }

        /// <summary>
        ///  Converts the given byte array back into a native object.  If
        ///  the object itself needs to be replace (as is the case for arrays),
        ///  then a new object may be assigned out through the parameter.
        /// </summary>
        internal void ConvertToValue(byte[] bytes, ref object value)
        {
            if (value is Stream s)
            {
                s.Position = 0;
                s.Write(bytes, 0, bytes.Length);
            }
            else if (value is byte[])
            {
                value = bytes;
            }
            else if (value is string)
            {
                value = BitConverter.ToString(bytes);
            }
            else
            {
                Debug.Fail("No conversion from byte[] to " + value == null ? "null" : value.GetType().FullName);
            }
        }

        /// <summary>
        ///  Edits the given object value using the editor style provided by GetEditorStyle.
        ///  A service provider is provided so that any required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }

            _context = context;

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc == null)
            {
                return value;
            }

            if (_binaryUI == null)
            {
                // child modal dialog -launching in System Aware mode
                _binaryUI = DpiHelper.CreateInstanceInSystemAwareContext(() => new BinaryUI(this));
            }

            _binaryUI.Value = value;
            if (edSvc.ShowDialog(_binaryUI) == DialogResult.OK)
            {
                value = _binaryUI.Value;
            }
            _binaryUI.Value = null;

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.  If the method is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        internal void ShowHelp()
        {
            IHelpService helpService = GetService(typeof(IHelpService)) as IHelpService;
            if (helpService != null)
            {
                helpService.ShowHelpFromKeyword(HELP_KEYWORD);
            }
            else
            {
                Debug.Fail("Unable to get IHelpService.");
            }
        }

        private class BinaryUI : Form
        {
            private readonly BinaryEditor _editor;
            private object _value;

            private RadioButton _radioAuto;
            private Button _buttonSave;
            private Button _buttonOK;
            private ByteViewer _byteViewer;
            private GroupBox _groupBoxMode;
            private RadioButton _radioHex;
            private RadioButton _radioAnsi;
            private TableLayoutPanel _radioButtonsTableLayoutPanel;
            private TableLayoutPanel _okSaveTableLayoutPanel;
            private TableLayoutPanel _overarchingTableLayoutPanel;
            private RadioButton _radioUnicode;

            public BinaryUI(BinaryEditor editor)
            {
                _editor = editor;
                InitializeComponent();
            }

            public object Value
            {
                get
                {
                    return _value;
                }
                set
                {
                    _value = value;
                    byte[] bytes = null;

                    if (value != null)
                    {
                        bytes = _editor.ConvertToBytes(value);
                    }

                    if (bytes != null)
                    {
                        _byteViewer.SetBytes(bytes);
                        _byteViewer.Enabled = true;
                    }
                    else
                    {
                        _byteViewer.SetBytes(new byte[0]);
                        _byteViewer.Enabled = false;
                    }
                }
            }

            private void RadioAuto_checkedChanged(object source, EventArgs e)
            {
                if (_radioAuto.Checked)
                    _byteViewer.SetDisplayMode(DisplayMode.Auto);
            }

            private void RadioHex_checkedChanged(object source, EventArgs e)
            {
                if (_radioHex.Checked)
                    _byteViewer.SetDisplayMode(DisplayMode.Hexdump);
            }

            private void RadioAnsi_checkedChanged(object source, EventArgs e)
            {
                if (_radioAnsi.Checked)
                    _byteViewer.SetDisplayMode(DisplayMode.Ansi);
            }

            private void RadioUnicode_checkedChanged(object source, EventArgs e)
            {
                if (_radioUnicode.Checked)
                    _byteViewer.SetDisplayMode(DisplayMode.Unicode);
            }

            private void ButtonOK_click(object source, EventArgs e)
            {
                object localValue = _value;
                _editor.ConvertToValue(_byteViewer.GetBytes(), ref localValue);
                _value = localValue;
            }

            private void ButtonSave_click(object source, EventArgs e)
            {
                try
                {
                    SaveFileDialog sfd = new SaveFileDialog();
                    sfd.FileName = SR.BinaryEditorFileName;
                    sfd.Title = SR.BinaryEditorSaveFile;
                    sfd.Filter = SR.BinaryEditorAllFiles + " (*.*)|*.*";

                    DialogResult result = sfd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        _byteViewer.SaveToFile(sfd.FileName);
                    }
                }
                catch (IOException x)
                {
                    RTLAwareMessageBox.Show(null, string.Format(SR.BinaryEditorFileError, x.Message),
                                    SR.BinaryEditorTitle, MessageBoxButtons.OK,
                                    MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
                }
            }

            private void Form_HelpRequested(object sender, HelpEventArgs e)
            {
                _editor.ShowHelp();
            }

            private void Form_HelpButtonClicked(object sender, CancelEventArgs e)
            {
                e.Cancel = true;
                _editor.ShowHelp();
            }

            private void InitializeComponent()
            {
                ComponentResourceManager resources = new ComponentResourceManager(typeof(BinaryEditor));
                _byteViewer = new ByteViewer();
                _buttonOK = new Button();
                _buttonSave = new Button();
                _groupBoxMode = new GroupBox();
                _radioButtonsTableLayoutPanel = new TableLayoutPanel();
                _radioUnicode = new RadioButton();
                _radioAuto = new RadioButton();
                _radioAnsi = new RadioButton();
                _radioHex = new RadioButton();
                _okSaveTableLayoutPanel = new TableLayoutPanel();
                _overarchingTableLayoutPanel = new TableLayoutPanel();
                _byteViewer.SuspendLayout();
                _groupBoxMode.SuspendLayout();
                _radioButtonsTableLayoutPanel.SuspendLayout();
                _okSaveTableLayoutPanel.SuspendLayout();
                _overarchingTableLayoutPanel.SuspendLayout();
                SuspendLayout();

                // 
                // byteViewer
                // 
                resources.ApplyResources(_byteViewer, "byteViewer");
                _byteViewer.SetDisplayMode(DisplayMode.Auto);
                _byteViewer.Name = "byteViewer";
                _byteViewer.Margin = Padding.Empty;
                _byteViewer.Dock = DockStyle.Fill;
                // 
                // buttonOK
                // 
                resources.ApplyResources(_buttonOK, "buttonOK");
                _buttonOK.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _buttonOK.DialogResult = DialogResult.OK;
                _buttonOK.Margin = new Padding(0, 0, 3, 0);
                _buttonOK.MinimumSize = new Size(75, 23);
                _buttonOK.Name = "buttonOK";
                _buttonOK.Padding = new Padding(10, 0, 10, 0);
                _buttonOK.Click += new EventHandler(ButtonOK_click);
                // 
                // buttonSave
                // 
                resources.ApplyResources(_buttonSave, "buttonSave");
                _buttonSave.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _buttonSave.Margin = new Padding(3, 0, 0, 0);
                _buttonSave.MinimumSize = new Size(75, 23);
                _buttonSave.Name = "buttonSave";
                _buttonSave.Padding = new Padding(10, 0, 10, 0);
                _buttonSave.Click += new EventHandler(ButtonSave_click);
                // 
                // groupBoxMode
                // 
                resources.ApplyResources(_groupBoxMode, "groupBoxMode");
                _groupBoxMode.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _groupBoxMode.Controls.Add(_radioButtonsTableLayoutPanel);
                _groupBoxMode.Margin = new Padding(0, 3, 0, 3);
                _groupBoxMode.Name = "groupBoxMode";
                _groupBoxMode.Padding = new Padding(0);
                _groupBoxMode.TabStop = false;
                // 
                // radioButtonsTableLayoutPanel
                // 
                resources.ApplyResources(_radioButtonsTableLayoutPanel, "radioButtonsTableLayoutPanel");
                _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                _radioButtonsTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
                _radioButtonsTableLayoutPanel.Controls.Add(_radioUnicode, 3, 0);
                _radioButtonsTableLayoutPanel.Controls.Add(_radioAuto, 0, 0);
                _radioButtonsTableLayoutPanel.Controls.Add(_radioAnsi, 2, 0);
                _radioButtonsTableLayoutPanel.Controls.Add(_radioHex, 1, 0);
                _radioButtonsTableLayoutPanel.Margin = new Padding(9);
                _radioButtonsTableLayoutPanel.Name = "radioButtonsTableLayoutPanel";
                _radioButtonsTableLayoutPanel.RowStyles.Add(new RowStyle());
                // 
                // radioUnicode
                // 
                resources.ApplyResources(_radioUnicode, "radioUnicode");
                _radioUnicode.Margin = new Padding(3, 0, 0, 0);
                _radioUnicode.Name = "radioUnicode";
                _radioUnicode.CheckedChanged += new EventHandler(RadioUnicode_checkedChanged);
                // 
                // radioAuto
                // 
                resources.ApplyResources(_radioAuto, "radioAuto");
                _radioAuto.Checked = true;
                _radioAuto.Margin = new Padding(0, 0, 3, 0);
                _radioAuto.Name = "radioAuto";
                _radioAuto.CheckedChanged += new EventHandler(RadioAuto_checkedChanged);
                // 
                // radioAnsi
                // 
                resources.ApplyResources(_radioAnsi, "radioAnsi");
                _radioAnsi.Margin = new Padding(3, 0, 3, 0);
                _radioAnsi.Name = "radioAnsi";
                _radioAnsi.CheckedChanged += new EventHandler(RadioAnsi_checkedChanged);
                // 
                // radioHex
                // 
                resources.ApplyResources(_radioHex, "radioHex");
                _radioHex.Margin = new Padding(3, 0, 3, 0);
                _radioHex.Name = "radioHex";
                _radioHex.CheckedChanged += new EventHandler(RadioHex_checkedChanged);
                // 
                // okSaveTableLayoutPanel
                // 
                resources.ApplyResources(_okSaveTableLayoutPanel, "okSaveTableLayoutPanel");
                _okSaveTableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                _okSaveTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _okSaveTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _okSaveTableLayoutPanel.Controls.Add(_buttonOK, 0, 0);
                _okSaveTableLayoutPanel.Controls.Add(_buttonSave, 1, 0);
                _okSaveTableLayoutPanel.Margin = new Padding(0, 9, 0, 0);
                _okSaveTableLayoutPanel.Name = "okSaveTableLayoutPanel";
                _okSaveTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
                // 
                // overarchingTableLayoutPanel
                // 
                resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
                _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
                _overarchingTableLayoutPanel.Controls.Add(_byteViewer, 0, 0);
                _overarchingTableLayoutPanel.Controls.Add(_groupBoxMode, 0, 1);
                _overarchingTableLayoutPanel.Controls.Add(_okSaveTableLayoutPanel, 0, 2);
                _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
                _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
                _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
                _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());

                // 
                // BinaryUI
                // 
                AcceptButton = _buttonOK;
                resources.ApplyResources(this, "$this");
                AutoScaleMode = AutoScaleMode.Font;
                CancelButton = _buttonOK;
                Controls.Add(_overarchingTableLayoutPanel);
                FormBorderStyle = FormBorderStyle.FixedDialog;
                HelpButton = true;
                MaximizeBox = false;
                MinimizeBox = false;
                Name = "BinaryUI";
                ShowIcon = false;
                ShowInTaskbar = false;
                HelpRequested += new HelpEventHandler(Form_HelpRequested);
                HelpButtonClicked += new CancelEventHandler(Form_HelpButtonClicked);
                _byteViewer.ResumeLayout(false);
                _byteViewer.PerformLayout();
                _groupBoxMode.ResumeLayout(false);
                _groupBoxMode.PerformLayout();
                _radioButtonsTableLayoutPanel.ResumeLayout(false);
                _radioButtonsTableLayoutPanel.PerformLayout();
                _okSaveTableLayoutPanel.ResumeLayout(false);
                _okSaveTableLayoutPanel.PerformLayout();
                _overarchingTableLayoutPanel.ResumeLayout(false);
                _overarchingTableLayoutPanel.PerformLayout();
                ResumeLayout(false);

            }
        }
    }
}


