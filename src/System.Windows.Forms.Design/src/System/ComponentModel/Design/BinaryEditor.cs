//------------------------------------------------------------------------------
// <copyright file="BinaryEditor.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ComponentModel.Design {

    using System.Design;
    using System;
    using System.Text;
    using System.IO;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms;
    using System.Drawing;
    using System.Drawing.Design;
    using Microsoft.Win32;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.ComponentModel;

    /// <internalonly/>
    internal class BinaryUI : System.Windows.Forms.Form {
        private BinaryEditor editor;
        object value;

        private RadioButton radioAuto;
        private Button buttonSave;
        private Button buttonOK;
        private ByteViewer byteViewer;
        private GroupBox groupBoxMode;
        private RadioButton radioHex;
        private RadioButton radioAnsi;
        private TableLayoutPanel radioButtonsTableLayoutPanel;
        private TableLayoutPanel okSaveTableLayoutPanel;
        private TableLayoutPanel overarchingTableLayoutPanel;
        private RadioButton radioUnicode;

        public BinaryUI(BinaryEditor editor) {
            this.editor = editor;
            InitializeComponent();
        }

        public object Value {
            get {
                return value;
            }
            set {
                this.value = value;
                byte[] bytes = null;

                if (value != null) {
                    bytes = editor.ConvertToBytes(value);
                }

                if (bytes != null) {
                    byteViewer.SetBytes(bytes);
                    byteViewer.Enabled = true;
                } else {
                    byteViewer.SetBytes(new byte[0]);
                    byteViewer.Enabled = false;
                }
            }
        }

        private void RadioAuto_checkedChanged(object source, EventArgs e) {
            if (radioAuto.Checked)
                byteViewer.SetDisplayMode(DisplayMode.Auto);
        }

        private void RadioHex_checkedChanged(object source, EventArgs e) {
            if (radioHex.Checked)
                byteViewer.SetDisplayMode(DisplayMode.Hexdump);
        }

        private void RadioAnsi_checkedChanged(object source, EventArgs e) {
            if (radioAnsi.Checked)
                byteViewer.SetDisplayMode(DisplayMode.Ansi);
        }

        private void RadioUnicode_checkedChanged(object source, EventArgs e) {
            if (radioUnicode.Checked)
                byteViewer.SetDisplayMode(DisplayMode.Unicode);
        }

        private void ButtonOK_click(object source, EventArgs e) {
            object localValue = value;
            editor.ConvertToValue(byteViewer.GetBytes(), ref localValue);
            value = localValue;
        }

        private void ButtonSave_click(object source, EventArgs e) {
            try {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = SR.GetString(SR.BinaryEditorFileName);
                sfd.Title = SR.GetString(SR.BinaryEditorSaveFile);
                sfd.Filter = SR.GetString(SR.BinaryEditorAllFiles) + " (*.*)|*.*";

                DialogResult result = sfd.ShowDialog();
                if (result == DialogResult.OK) {
                    byteViewer.SaveToFile(sfd.FileName);
                }
            } catch (IOException x) {
                RTLAwareMessageBox.Show(null, SR.GetString(SR.BinaryEditorFileError, x.Message),
                                SR.GetString(SR.BinaryEditorTitle), MessageBoxButtons.OK,
                                MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        private void Form_HelpRequested(object sender, HelpEventArgs e) {
            editor.ShowHelp();
        }

        private void Form_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            editor.ShowHelp();
        }

        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BinaryEditor));
            this.byteViewer = new ByteViewer(); 
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonSave = new System.Windows.Forms.Button();
            this.groupBoxMode = new System.Windows.Forms.GroupBox();
            this.radioButtonsTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.radioUnicode = new System.Windows.Forms.RadioButton();
            this.radioAuto = new System.Windows.Forms.RadioButton();
            this.radioAnsi = new System.Windows.Forms.RadioButton();
            this.radioHex = new System.Windows.Forms.RadioButton();
            this.okSaveTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.overarchingTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.byteViewer.SuspendLayout();
            this.groupBoxMode.SuspendLayout();
            this.radioButtonsTableLayoutPanel.SuspendLayout();
            this.okSaveTableLayoutPanel.SuspendLayout();
            this.overarchingTableLayoutPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // byteViewer
            // 
            resources.ApplyResources(this.byteViewer, "byteViewer");
            this.byteViewer.SetDisplayMode(DisplayMode.Auto);
            this.byteViewer.Name = "byteViewer";
            this.byteViewer.Margin = Padding.Empty;
            this.byteViewer.Dock = DockStyle.Fill;  
            // 
            // buttonOK
            // 
            resources.ApplyResources(this.buttonOK, "buttonOK");
            this.buttonOK.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.buttonOK.MinimumSize = new System.Drawing.Size(75, 23);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.buttonOK.Click += new System.EventHandler(this.ButtonOK_click);
            // 
            // buttonSave
            // 
            resources.ApplyResources(this.buttonSave, "buttonSave");
            this.buttonSave.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonSave.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.buttonSave.MinimumSize = new System.Drawing.Size(75, 23);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_click);
            // 
            // groupBoxMode
            // 
            resources.ApplyResources(this.groupBoxMode, "groupBoxMode");
            this.groupBoxMode.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.groupBoxMode.Controls.Add(this.radioButtonsTableLayoutPanel);
            this.groupBoxMode.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
            this.groupBoxMode.Name = "groupBoxMode";
            this.groupBoxMode.Padding = new System.Windows.Forms.Padding(0);
            this.groupBoxMode.TabStop = false;
            // 
            // radioButtonsTableLayoutPanel
            // 
            resources.ApplyResources(this.radioButtonsTableLayoutPanel, "radioButtonsTableLayoutPanel");
            this.radioButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.radioButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.radioButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.radioButtonsTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.radioButtonsTableLayoutPanel.Controls.Add(this.radioUnicode, 3, 0);
            this.radioButtonsTableLayoutPanel.Controls.Add(this.radioAuto, 0, 0);
            this.radioButtonsTableLayoutPanel.Controls.Add(this.radioAnsi, 2, 0);
            this.radioButtonsTableLayoutPanel.Controls.Add(this.radioHex, 1, 0);
            this.radioButtonsTableLayoutPanel.Margin = new System.Windows.Forms.Padding(9);
            this.radioButtonsTableLayoutPanel.Name = "radioButtonsTableLayoutPanel";
            this.radioButtonsTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            // 
            // radioUnicode
            // 
            resources.ApplyResources(this.radioUnicode, "radioUnicode");
            this.radioUnicode.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.radioUnicode.Name = "radioUnicode";
            this.radioUnicode.CheckedChanged += new System.EventHandler(this.RadioUnicode_checkedChanged);
            // 
            // radioAuto
            // 
            resources.ApplyResources(this.radioAuto, "radioAuto");
            this.radioAuto.Checked = true;
            this.radioAuto.Margin = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.radioAuto.Name = "radioAuto";
            this.radioAuto.CheckedChanged += new System.EventHandler(this.RadioAuto_checkedChanged);
            // 
            // radioAnsi
            // 
            resources.ApplyResources(this.radioAnsi, "radioAnsi");
            this.radioAnsi.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioAnsi.Name = "radioAnsi";
            this.radioAnsi.CheckedChanged += new System.EventHandler(this.RadioAnsi_checkedChanged);
            // 
            // radioHex
            // 
            resources.ApplyResources(this.radioHex, "radioHex");
            this.radioHex.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.radioHex.Name = "radioHex";
            this.radioHex.CheckedChanged += new System.EventHandler(this.RadioHex_checkedChanged);
            // 
            // okSaveTableLayoutPanel
            // 
            resources.ApplyResources(this.okSaveTableLayoutPanel, "okSaveTableLayoutPanel");
            this.okSaveTableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.okSaveTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okSaveTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.okSaveTableLayoutPanel.Controls.Add(this.buttonOK, 0, 0);
            this.okSaveTableLayoutPanel.Controls.Add(this.buttonSave, 1, 0);
            this.okSaveTableLayoutPanel.Margin = new System.Windows.Forms.Padding(0, 9, 0, 0);
            this.okSaveTableLayoutPanel.Name = "okSaveTableLayoutPanel";
            this.okSaveTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            // 
            // overarchingTableLayoutPanel
            // 
            resources.ApplyResources(this.overarchingTableLayoutPanel, "overarchingTableLayoutPanel");  
            this.overarchingTableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.overarchingTableLayoutPanel.Controls.Add(this.byteViewer, 0, 0);
            this.overarchingTableLayoutPanel.Controls.Add(this.groupBoxMode, 0, 1);
            this.overarchingTableLayoutPanel.Controls.Add(this.okSaveTableLayoutPanel, 0, 2);
            this.overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.overarchingTableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            
            // 
            // BinaryUI
            // 
            this.AcceptButton = this.buttonOK;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonOK;
            this.Controls.Add(this.overarchingTableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.HelpButton = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BinaryUI";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.HelpRequested += new System.Windows.Forms.HelpEventHandler(this.Form_HelpRequested);
            this.HelpButtonClicked += new System.ComponentModel.CancelEventHandler(this.Form_HelpButtonClicked);
            this.byteViewer.ResumeLayout(false);
            this.byteViewer.PerformLayout();
            this.groupBoxMode.ResumeLayout(false);
            this.groupBoxMode.PerformLayout();
            this.radioButtonsTableLayoutPanel.ResumeLayout(false);
            this.radioButtonsTableLayoutPanel.PerformLayout();
            this.okSaveTableLayoutPanel.ResumeLayout(false);
            this.okSaveTableLayoutPanel.PerformLayout();
            this.overarchingTableLayoutPanel.ResumeLayout(false);
            this.overarchingTableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }
    }

    /// <include file='doc\BinaryEditor.uex' path='docs/doc[@for="BinaryEditor"]/*' />
    /// <devdoc>
    ///      Generic editor for editing binary data.  This presents
    ///      a hex editing window to the user.
    /// </devdoc>
    public sealed class BinaryEditor : UITypeEditor {
        private static readonly string HELP_KEYWORD = "System.ComponentModel.Design.BinaryEditor";
        private ITypeDescriptorContext context;
        private BinaryUI binaryUI;

        internal object GetService(Type serviceType) {
            if (this.context != null) {
                IDesignerHost host = this.context.GetService(typeof(IDesignerHost)) as IDesignerHost;
                if (host == null)
                    return this.context.GetService(serviceType);
                else
                    return host.GetService(serviceType);
            }
            return null;
        }

        /// <include file='doc\BinaryEditor.uex' path='docs/doc[@for="BinaryEditor.ConvertToBytes"]/*' />
        /// <devdoc>
        ///      Converts the given object to an array of bytes to be manipulated
        ///      by the editor.  The default implementation of this supports
        ///      byte[] and stream objects.
        /// </devdoc>
        internal byte[] ConvertToBytes(object value) {
            if (value is Stream) {
                Stream s = (Stream)value;
                s.Position = 0;
                int byteCount = (int)(s.Length - s.Position);
                byte[] bytes = new byte[byteCount];
                s.Read(bytes, 0, byteCount);
                return bytes;
            }

            if (value is byte[]) {
                return (byte[])value;
            }

            if (value is string) {
                int size = ((string)value).Length * 2;
                byte[] buffer = new byte[size];
                Encoding.Unicode.GetBytes(((string)value).ToCharArray(), 0, size / 2, buffer, 0);
                return buffer;
            }

            Debug.Fail("No conversion from " + value == null ? "null" : value.GetType().FullName + " to byte[]");
            return null;
        }

        /// <include file='doc\BinaryEditor.uex' path='docs/doc[@for="BinaryEditor.ConvertToValue"]/*' />
        /// <devdoc>
        ///      Converts the given byte array back into a native object.  If
        ///      the object itself needs to be replace (as is the case for arrays),
        ///      then a new object may be assigned out through the parameter.
        /// </devdoc>
        internal void ConvertToValue(byte[] bytes, ref object value) {

            if (value is Stream) {
                Stream s = (Stream)value;
                s.Position = 0;
                s.Write(bytes, 0, bytes.Length);
            } else if (value is byte[]) {
                value = bytes;
            } else if (value is string) {
                value = BitConverter.ToString(bytes);
            } else {
                Debug.Fail("No conversion from byte[] to " + value == null ? "null" : value.GetType().FullName);
            }
        }

        /// <include file='doc\BinaryEditor.uex' path='docs/doc[@for="BinaryEditor.EditValue"]/*' />
        /// <devdoc>
        ///      Edits the given object value using the editor style provided by
        ///      GetEditorStyle.  A service provider is provided so that any
        ///      required editing services can be obtained.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
            if (provider != null) {
                this.context = context;

                IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

                if (edSvc != null) {
                    if (binaryUI == null) {

                        // child modal dialog -launching in System Aware mode
                        binaryUI = DpiHelper.CreateInstanceInSystemAwareContext(() => new BinaryUI(this));
                    }

                    binaryUI.Value = value;

                    if (edSvc.ShowDialog(binaryUI) == DialogResult.OK) {
                        value = binaryUI.Value;
                    }

                    binaryUI.Value = null;
                }
            }

            return value;
        }

        /// <include file='doc\BinaryEditor.uex' path='docs/doc[@for="BinaryEditor.GetEditStyle"]/*' />
        /// <devdoc>
        ///      Retrieves the editing style of the Edit method.  If the method
        ///      is not supported, this will return None.
        /// </devdoc>
        [SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")] // everything in this assembly is full trust.
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
            return UITypeEditorEditStyle.Modal;
        }

        internal void ShowHelp() {
            IHelpService helpService = GetService(typeof(IHelpService)) as IHelpService;
            if (helpService != null) {
                helpService.ShowHelpFromKeyword(HELP_KEYWORD);
            } else {
                Debug.Fail("Unable to get IHelpService.");
            }
        }
    }
}


