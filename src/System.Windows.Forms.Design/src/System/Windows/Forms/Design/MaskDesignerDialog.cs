// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// UI for the MaskTypeEditor (Design time).
    /// </summary>
    internal class MaskDesignerDialog : Form
    {
        private Label _lblHeader;
        private ListView _listViewCannedMasks;
        private CheckBox _checkBoxUseValidatingType;
        private ColumnHeader _maskDescriptionHeader;
        private ColumnHeader _dataFormatHeader;
        private ColumnHeader _validatingTypeHeader;
        private TableLayoutPanel _maskTryItTable;
        private Label _lblMask;
        private TextBox _txtBoxMask;
        private Label _lblTryIt;
        private readonly MaskedTextBox _maskedTextBox;
        private TableLayoutPanel _okCancelTableLayoutPanel;
        private TableLayoutPanel _overarchingTableLayoutPanel;
        private Button _btnOK;
        private Button _btnCancel;
        private ErrorProvider _errorProvider;

        private readonly List<MaskDescriptor> _maskDescriptors = new List<MaskDescriptor>();
        private MaskDescriptor _customMaskDescriptor;
        private SortOrder _listViewSortOrder = SortOrder.Ascending;
        private IContainer _components;
        private readonly IHelpService _helpService = null;

        /// <summary>
        /// Constructor receiving a clone of the MaskedTextBox control under design.
        /// </summary>
        public MaskDesignerDialog(MaskedTextBox instance, IHelpService helpService)
        {
            if (instance == null)
            {
                Debug.Fail("Null masked text box, creating default.");
                _maskedTextBox = new MaskedTextBox();
            }
            else
            {
                _maskedTextBox = MaskedTextBoxDesigner.GetDesignMaskedTextBox(instance);
            }

            _helpService = helpService;

            InitializeComponent();

            // Enable Vista Explorer listview style
            DesignerUtils.ApplyListViewThemeStyles(_listViewCannedMasks);

            // Non-designer-handled stuff.
            SuspendLayout();

            _txtBoxMask.Text = _maskedTextBox.Mask;

            // Add default mask descriptors to the mask description list.
            AddDefaultMaskDescriptors(_maskedTextBox.Culture);

            // 
            // maskDescriptionHeader
            // 
            _maskDescriptionHeader.Text = SR.MaskDesignerDialogMaskDescription;
            _maskDescriptionHeader.Width = _listViewCannedMasks.Width / 3;
            // 
            // dataFormatHeader
            // 
            _dataFormatHeader.Text = SR.MaskDesignerDialogDataFormat;
            _dataFormatHeader.Width = _listViewCannedMasks.Width / 3;
            // 
            // validatingTypeHeader
            // 
            _validatingTypeHeader.Text = SR.MaskDesignerDialogValidatingType;
            _validatingTypeHeader.Width = (_listViewCannedMasks.Width / 3) - SystemInformation.VerticalScrollBarWidth - 4;	// so no h-scrollbar.
            ResumeLayout(false);

            HookEvents();
        }

        private void HookEvents()
        {
            _listViewCannedMasks.SelectedIndexChanged += new EventHandler(listViewCannedMasks_SelectedIndexChanged);
            _listViewCannedMasks.ColumnClick += new ColumnClickEventHandler(listViewCannedMasks_ColumnClick);
            _listViewCannedMasks.Enter += new EventHandler(listViewCannedMasks_Enter);
            _btnOK.Click += new EventHandler(btnOK_Click);
            _txtBoxMask.TextChanged += new EventHandler(txtBoxMask_TextChanged);
            _txtBoxMask.Validating += new CancelEventHandler(txtBoxMask_Validating);
            _maskedTextBox.KeyDown += new KeyEventHandler(maskedTextBox_KeyDown);
            _maskedTextBox.MaskInputRejected += new MaskInputRejectedEventHandler(maskedTextBox_MaskInputRejected);
            Load += new EventHandler(MaskDesignerDialog_Load);
            HelpButtonClicked += new CancelEventHandler(MaskDesignerDialog_HelpButtonClicked);
        }

        private void InitializeComponent()
        {
            _components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(MaskDesignerDialog));
            _lblHeader = new Label();
            _listViewCannedMasks = new ListView();
            _maskDescriptionHeader = new ColumnHeader(resources.GetString("listViewCannedMasks.Columns"));
            _dataFormatHeader = new ColumnHeader(resources.GetString("listViewCannedMasks.Columns1"));
            _validatingTypeHeader = new ColumnHeader(resources.GetString("listViewCannedMasks.Columns2"));
            _btnOK = new Button();
            _btnCancel = new Button();
            _checkBoxUseValidatingType = new CheckBox();
            _maskTryItTable = new TableLayoutPanel();
            _lblMask = new Label();
            _txtBoxMask = new TextBox();
            _lblTryIt = new Label();
            _overarchingTableLayoutPanel = new TableLayoutPanel();
            _okCancelTableLayoutPanel = new TableLayoutPanel();
            _errorProvider = new ErrorProvider(_components);
            _maskTryItTable.SuspendLayout();
            _overarchingTableLayoutPanel.SuspendLayout();
            _okCancelTableLayoutPanel.SuspendLayout();
            ((ISupportInitialize)(_errorProvider)).BeginInit();
            SuspendLayout();
            // 
            // maskedTextBox
            // 
            resources.ApplyResources(_maskedTextBox, "maskedTextBox");
            _maskedTextBox.Margin = new Padding(3, 3, 18, 0);
            _maskedTextBox.Name = "maskedTextBox";
            // 
            // lblHeader
            // 
            resources.ApplyResources(_lblHeader, "lblHeader");
            _lblHeader.Margin = new Padding(0, 0, 0, 3);
            _lblHeader.Name = "lblHeader";
            // 
            // listViewCannedMasks
            // 
            resources.ApplyResources(_listViewCannedMasks, "listViewCannedMasks");
            _listViewCannedMasks.Columns.AddRange(new ColumnHeader[] {
            _maskDescriptionHeader,
            _dataFormatHeader,
            _validatingTypeHeader});
            _listViewCannedMasks.FullRowSelect = true;
            _listViewCannedMasks.HideSelection = false;
            _listViewCannedMasks.Margin = new Padding(0, 3, 0, 3);
            _listViewCannedMasks.MultiSelect = false;
            _listViewCannedMasks.Name = "listViewCannedMasks";
            _listViewCannedMasks.Sorting = SortOrder.None; // We'll do the sorting ourselves.
            _listViewCannedMasks.View = View.Details;
            // 
            // maskDescriptionHeader
            // 
            resources.ApplyResources(_maskDescriptionHeader, "maskDescriptionHeader");
            // 
            // dataFormatHeader
            // 
            resources.ApplyResources(_dataFormatHeader, "dataFormatHeader");
            // 
            // validatingTypeHeader
            // 
            resources.ApplyResources(_validatingTypeHeader, "validatingTypeHeader");
            // 
            // btnOK
            // 
            resources.ApplyResources(_btnOK, "btnOK");
            _btnOK.DialogResult = DialogResult.OK;
            _btnOK.Margin = new Padding(0, 0, 3, 0);
            _btnOK.MinimumSize = new Drawing.Size(75, 23);
            _btnOK.Name = "btnOK";
            _btnOK.Padding = new Padding(10, 0, 10, 0);
            // 
            // btnCancel
            // 
            resources.ApplyResources(_btnCancel, "btnCancel");
            _btnCancel.DialogResult = DialogResult.Cancel;
            _btnCancel.Margin = new Padding(3, 0, 0, 0);
            _btnCancel.MinimumSize = new Drawing.Size(75, 23);
            _btnCancel.Name = "btnCancel";
            _btnCancel.Padding = new Padding(10, 0, 10, 0);
            // 
            // checkBoxUseValidatingType
            // 
            resources.ApplyResources(_checkBoxUseValidatingType, "checkBoxUseValidatingType");
            _checkBoxUseValidatingType.Checked = true;
            _checkBoxUseValidatingType.CheckState = CheckState.Checked;
            _checkBoxUseValidatingType.Margin = new Padding(0, 0, 0, 3);
            _checkBoxUseValidatingType.Name = "checkBoxUseValidatingType";
            // 
            // maskTryItTable
            // 
            resources.ApplyResources(_maskTryItTable, "maskTryItTable");
            _maskTryItTable.ColumnStyles.Add(new ColumnStyle());
            _maskTryItTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            _maskTryItTable.ColumnStyles.Add(new ColumnStyle());
            _maskTryItTable.Controls.Add(_checkBoxUseValidatingType, 2, 0);
            _maskTryItTable.Controls.Add(_lblMask, 0, 0);
            _maskTryItTable.Controls.Add(_txtBoxMask, 1, 0);
            _maskTryItTable.Controls.Add(_lblTryIt, 0, 1);
            _maskTryItTable.Controls.Add(_maskedTextBox, 1, 1);
            _maskTryItTable.Margin = new Padding(0, 3, 0, 3);
            _maskTryItTable.Name = "maskTryItTable";
            _maskTryItTable.RowStyles.Add(new RowStyle());
            _maskTryItTable.RowStyles.Add(new RowStyle());
            // 
            // lblMask
            // 
            resources.ApplyResources(_lblMask, "lblMask");
            _lblMask.Margin = new Padding(0, 0, 3, 3);
            _lblMask.Name = "lblMask";
            // 
            // txtBoxMask
            // 
            resources.ApplyResources(_txtBoxMask, "txtBoxMask");
            _txtBoxMask.Margin = new Padding(3, 0, 18, 3);
            _txtBoxMask.Name = "txtBoxMask";
            // 
            // lblTryIt
            // 
            resources.ApplyResources(_lblTryIt, "lblTryIt");
            _lblTryIt.Margin = new Padding(0, 3, 3, 0);
            _lblTryIt.Name = "lblTryIt";
            // 
            // overarchingTableLayoutPanel
            // 
            resources.ApplyResources(_overarchingTableLayoutPanel, "overarchingTableLayoutPanel");
            _overarchingTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _overarchingTableLayoutPanel.Controls.Add(_maskTryItTable, 0, 3);
            _overarchingTableLayoutPanel.Controls.Add(_okCancelTableLayoutPanel, 0, 4);
            _overarchingTableLayoutPanel.Controls.Add(_lblHeader, 0, 1);
            _overarchingTableLayoutPanel.Controls.Add(_listViewCannedMasks, 0, 2);
            _overarchingTableLayoutPanel.Name = "overarchingTableLayoutPanel";
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            _overarchingTableLayoutPanel.RowStyles.Add(new RowStyle());
            // 
            // okCancelTableLayoutPanel
            // 
            resources.ApplyResources(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel");
            _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            _okCancelTableLayoutPanel.Controls.Add(_btnCancel, 1, 0);
            _okCancelTableLayoutPanel.Controls.Add(_btnOK, 0, 0);
            _okCancelTableLayoutPanel.Margin = new Padding(0, 6, 0, 0);
            _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
            _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
            // 
            // errorProvider
            // 
            _errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            _errorProvider.ContainerControl = this;
            // 
            // MaskDesignerDialog
            // 
            resources.ApplyResources(this, "$this");
            AcceptButton = _btnOK;
            CancelButton = _btnCancel;
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(_overarchingTableLayoutPanel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            HelpButton = true;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "MaskDesignerDialog";
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            _maskTryItTable.ResumeLayout(false);
            _maskTryItTable.PerformLayout();
            _overarchingTableLayoutPanel.ResumeLayout(false);
            _overarchingTableLayoutPanel.PerformLayout();
            _okCancelTableLayoutPanel.ResumeLayout(false);
            _okCancelTableLayoutPanel.PerformLayout();
            ((ISupportInitialize)(_errorProvider)).EndInit();
            ResumeLayout(false);

        }

        /// <summary>
        /// The current text (mask) in the txtBoxMask control.
        /// </summary>
        public string Mask => _maskedTextBox.Mask;

        /// <summary>
        /// The current text (mask) in the txtBoxMask control.
        /// </summary>
        public Type ValidatingType { get; private set; }

        /// <summary>
        /// A collection of MaskDescriptor objects represented in the ListView with the canned mask
        /// descriptions.
        /// </summary>
        public IEnumerator MaskDescriptors => _maskDescriptors.GetEnumerator();

        /// <summary>
        /// Adds the default mask descriptors to the mask description list.
        /// We need to add the deafult descriptors explicitly because the DiscoverMaskDescriptors method only adds
        /// public descriptors and these are internal.
        /// </summary>
        private void AddDefaultMaskDescriptors(CultureInfo culture)
        {
            _customMaskDescriptor = new MaskDescriptorTemplate(null, SR.MaskDesignerDialogCustomEntry, null, null, null, true);

            List<MaskDescriptor> maskDescriptors = MaskDescriptorTemplate.GetLocalizedMaskDescriptors(culture);

            // Need to pass false for validateDescriptor param since the custom mask will fail validation
            // because the mask is empty.
            InsertMaskDescriptor(0, _customMaskDescriptor, /*validate*/ false);

            foreach (MaskDescriptor maskDescriptor in maskDescriptors)
            {
                InsertMaskDescriptor(0, maskDescriptor);
            }
        }

        /// <summary>
        /// Determines whether the specified MaskDescriptor object is in the MaskDescriptor collection or not.
        /// </summary>
        private bool ContainsMaskDescriptor(MaskDescriptor maskDescriptor)
        {
            Debug.Assert(maskDescriptor != null, "Null mask descriptor.");

            foreach (MaskDescriptor descriptor in _maskDescriptors)
            {
                Debug.Assert(descriptor != null, "Null mask descriptor in the collection.");

                if (maskDescriptor.Equals(descriptor) || maskDescriptor.Name.Trim() == descriptor.Name.Trim())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Uses the specified ITypeDiscoveryService service provider to discover MaskDescriptor objects from
        /// the referenced assemblies.
        /// </summary>
        public void DiscoverMaskDescriptors(ITypeDiscoveryService discoveryService)
        {
            if (discoveryService == null)
            {
                return;
            }

            ICollection descriptors = DesignerUtils.FilterGenericTypes(discoveryService.GetTypes(typeof(MaskDescriptor), false /* excludeGlobalTypes */));

            // Note: This code assumes DesignerUtils.FilterGenericTypes return a valid ICollection (collection of MaskDescriptor types).
            foreach (Type t in descriptors)
            {
                if (t.IsAbstract || !t.IsPublic)
                {
                    continue;
                }

                // Since mask descriptors can be provided from external sources, we need to guard against
                // possible exceptions when accessing an external descriptor.
                try
                {
                    MaskDescriptor maskDescriptor = (MaskDescriptor)Activator.CreateInstance(t);
                    InsertMaskDescriptor(0, maskDescriptor);
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

        /// <summary>
        /// Gets the index of a mask descriptor in the mask descriptor table.
        /// </summary>
        private int GetMaskDescriptorIndex(MaskDescriptor maskDescriptor)
        {
            for (int index = 0; index < _maskDescriptors.Count; index++)
            {
                MaskDescriptor descriptor = _maskDescriptors[index];

                if (descriptor == maskDescriptor)
                {
                    return index;
                }
            }

            Debug.Fail("Could not find mask descriptor.");
            return -1;
        }

        /// <summary>
        /// Selects the mask descriptor corresponding to the current MaskedTextBox.Mask if any, otherwise the custom entry.
        /// </summary>
        private void SelectMtbMaskDescriptor()
        {
            int selectedItemIdx = -1;

            if (!string.IsNullOrEmpty(_maskedTextBox.Mask))
            {
                for (int selectedIndex = 0; selectedIndex < _maskDescriptors.Count; selectedIndex++)
                {
                    MaskDescriptor descriptor = _maskDescriptors[selectedIndex];

                    if (descriptor.Mask == _maskedTextBox.Mask && descriptor.ValidatingType == _maskedTextBox.ValidatingType)
                    {
                        selectedItemIdx = selectedIndex;
                        break;
                    }
                }
            }

            if (selectedItemIdx == -1) // select custom mask.
            {
                selectedItemIdx = GetMaskDescriptorIndex(_customMaskDescriptor);

                if (selectedItemIdx == -1)
                {
                    Debug.Fail("Could not find custom mask descriptor.");
                }
            }

            if (selectedItemIdx != -1)
            {
                SetSelectedMaskDescriptor(selectedItemIdx);
            }
        }

        /// <summary>
        /// Selects the specified item in the ListView.
        /// </summary>
        private void SetSelectedMaskDescriptor(MaskDescriptor maskDex)
        {
            int maskDexIndex = GetMaskDescriptorIndex(maskDex);
            SetSelectedMaskDescriptor(maskDexIndex);
        }
        private void SetSelectedMaskDescriptor(int maskDexIndex)
        {
            if (maskDexIndex < 0 || _listViewCannedMasks.Items.Count <= maskDexIndex)
            {
                return;
            }

            _listViewCannedMasks.Items[maskDexIndex].Selected = true;
            _listViewCannedMasks.FocusedItem = _listViewCannedMasks.Items[maskDexIndex];
            _listViewCannedMasks.EnsureVisible(maskDexIndex);
        }

        /// <summary>
        /// Sorts the maskDescriptors and the list view items.
        /// </summary>
        private void UpdateSortedListView(MaskDescriptorComparer.SortType sortType)
        {
            if (!_listViewCannedMasks.IsHandleCreated)
            {
                return;
            }

            MaskDescriptor selectedMaskDex = null;

            // Save current selected entry to restore it after sorting.
            if (_listViewCannedMasks.SelectedItems.Count > 0)
            {
                int selectedIndex = _listViewCannedMasks.SelectedIndices[0];
                selectedMaskDex = _maskDescriptors[selectedIndex];
            }

            // Custom mask descriptor should always be the last entry - remove it before sorting array.
            _maskDescriptors.RemoveAt(_maskDescriptors.Count - 1);

            // Sort MaskDescriptor collection.
            _maskDescriptors.Sort(new MaskDescriptorComparer(sortType, _listViewSortOrder));

            // Sorting the ListView items forces handle recreation, since we have the items sorted and know what item to select
            // it is better for us to replace the items ourselves.  This way also avoids problems with the selected item  and 
            // the custom entry not getting properly added.
            // this.listViewCannedMasks.Sort();

            // Since we need to pre-process each item before inserting it in the ListView, it is better to remove all items 
            // from it first and then add the sorted ones back (no replace).  Stop redrawing while we change the list.

            UnsafeNativeMethods.SendMessage(_listViewCannedMasks.Handle, Interop.WindowMessages.WM_SETREDRAW, false, /* unused = */ 0);

            try
            {
                _listViewCannedMasks.Items.Clear();

                string nullEntry = SR.MaskDescriptorValidatingTypeNone;

                foreach (MaskDescriptor maskDescriptor in _maskDescriptors)
                {
                    string validatingType = maskDescriptor.ValidatingType != null ? maskDescriptor.ValidatingType.Name : nullEntry;

                    // Make sure the sample displays literals.
                    MaskedTextProvider mtp = new MaskedTextProvider(maskDescriptor.Mask, maskDescriptor.Culture);
                    bool success = mtp.Add(maskDescriptor.Sample);
                    Debug.Assert(success, "BadBad: Could not add MaskDescriptor.Sample even it was validated, something is wrong!");
                    // Don't include prompt.
                    string sample = mtp.ToString(false, true);

                    _listViewCannedMasks.Items.Add(new ListViewItem(new string[] { maskDescriptor.Name, sample, validatingType }));
                }

                // Add the custom mask descriptor as the last entry.
                _maskDescriptors.Add(_customMaskDescriptor);
                _listViewCannedMasks.Items.Add(new ListViewItem(new string[] { _customMaskDescriptor.Name, "", nullEntry }));

                if (selectedMaskDex != null)
                {
                    SetSelectedMaskDescriptor(selectedMaskDex);
                }
            }
            finally
            {
                // Resume redraw.
                UnsafeNativeMethods.SendMessage(_listViewCannedMasks.Handle, Interop.WindowMessages.WM_SETREDRAW, true, /* unused = */ 0);
                _listViewCannedMasks.Invalidate();
            }
        }

        /// <summary>
        /// Inserts a MaskDescriptor object in the specified position in the internal MaskDescriptor collection.
        /// </summary>
        private void InsertMaskDescriptor(int index, MaskDescriptor maskDescriptor)
        {
            InsertMaskDescriptor(index, maskDescriptor, true);
        }
        private void InsertMaskDescriptor(int index, MaskDescriptor maskDescriptor, bool validateDescriptor)
        {
            string errorMessage;

            if (validateDescriptor && !MaskDescriptor.IsValidMaskDescriptor(maskDescriptor, out errorMessage))
            {
                return;
            }

            if (!ContainsMaskDescriptor(maskDescriptor))
            {
                _maskDescriptors.Insert(index, maskDescriptor);
            }
        }

        /// <summary>
        /// Canned masks list view Column click event handler.  Sorts the items.
        /// </summary>
        private void listViewCannedMasks_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Switch sorting order.
            switch (_listViewSortOrder)
            {
                case SortOrder.None:
                case SortOrder.Descending:
                    _listViewSortOrder = SortOrder.Ascending;
                    break;
                case SortOrder.Ascending:
                    _listViewSortOrder = SortOrder.Descending;
                    break;
            }

            UpdateSortedListView((MaskDescriptorComparer.SortType)e.Column);
        }

        /// <summary>
        /// OK button Click event handler.  Updates the validating type.
        /// </summary>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_checkBoxUseValidatingType.Checked)
            {
                ValidatingType = _maskedTextBox.ValidatingType;
            }
            else
            {
                ValidatingType = null;
            }
        }

        /// <summary>
        /// Canned masks list view Enter event handler.  Sets focus in the first item if none has it.
        /// </summary>
        private void listViewCannedMasks_Enter(object sender, EventArgs e)
        {
            if (_listViewCannedMasks.FocusedItem != null || _listViewCannedMasks.Items.Count <= 0)
            {
                return;
            }

            _listViewCannedMasks.Items[0].Focused = true;
        }

        /// <summary>
        /// Canned masks list view SelectedIndexChanged event handler.  Gets the selected canned mask
        /// information.
        /// </summary>
        private void listViewCannedMasks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_listViewCannedMasks.SelectedItems.Count == 0)
            {
                return;
            }

            int selectedIndex = _listViewCannedMasks.SelectedIndices[0];
            MaskDescriptor maskDescriptor = (MaskDescriptor)_maskDescriptors[selectedIndex];

            // If one of the canned mask descriptors chosen, update test control.
            if (maskDescriptor != _customMaskDescriptor)
            {
                _txtBoxMask.Text = maskDescriptor.Mask;
                _maskedTextBox.Mask = maskDescriptor.Mask;
                _maskedTextBox.ValidatingType = maskDescriptor.ValidatingType;
            }
            else
            {
                _maskedTextBox.ValidatingType = null;
            }
        }

        private void MaskDesignerDialog_Load(object sender, EventArgs e)
        {
            UpdateSortedListView(MaskDescriptorComparer.SortType.ByName);
            SelectMtbMaskDescriptor();
            _btnCancel.Select();
        }

        private void maskedTextBox_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            _errorProvider.SetError(_maskedTextBox, MaskedTextBoxDesigner.GetMaskInputRejectedErrorMessage(e));
        }

        private string HelpTopic
        {
            get
            {
                return "net.ComponentModel.MaskPropertyEditor";
            }
        }

        /// <summary>
        ///    <para>
        ///   Called when the help button is clicked.
        ///    </para>
        /// </summary>
        private void ShowHelp()
        {
            if (_helpService != null)
            {
                _helpService.ShowHelpFromKeyword(HelpTopic);
            }
            else
            {
                Debug.Fail("Unable to get IHelpService.");
            }
        }

        private void MaskDesignerDialog_HelpButtonClicked(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            ShowHelp();
        }

        private void maskedTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            _errorProvider.Clear();
        }

        /// <summary>
        /// Mask text box Leave event handler.
        /// </summary>
        private void txtBoxMask_Validating(object sender, CancelEventArgs e)
        {
            try
            {
                _maskedTextBox.Mask = _txtBoxMask.Text;
            }
            catch (ArgumentException)
            {
                // The text in the TextBox may contain invalid characters so we just ignore the exception.
            }
        }

        /// <summary>
        /// Mask text box TextChanged event handler.
        /// </summary>
        private void txtBoxMask_TextChanged(object sender, EventArgs e)
        {
            // If the change in the text box is performed by the user, we need to select the 'Custom' item in
            // the list view, which is the last item.

            MaskDescriptor selectedMaskDex = null;

            if (_listViewCannedMasks.SelectedItems.Count != 0)
            {
                int selectedIndex = _listViewCannedMasks.SelectedIndices[0];
                selectedMaskDex = _maskDescriptors[selectedIndex];
            }

            if (selectedMaskDex == null || (selectedMaskDex != _customMaskDescriptor && selectedMaskDex.Mask != _txtBoxMask.Text))
            {
                SetSelectedMaskDescriptor(_customMaskDescriptor);
            }
        }
    }
}
