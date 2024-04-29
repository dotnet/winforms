// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal sealed partial class MdiWindowDialog : Form
{
    private ListBox _itemList;
    private Button _okButton;
    private Button _cancelButton;
    private TableLayoutPanel _okCancelTableLayoutPanel;
    private Form? _active;

    public MdiWindowDialog()
        : base()
    {
        InitializeComponent();
    }

    public Form? ActiveChildForm
    {
        get
        {
#if DEBUG
            ListItem? item = (ListItem?)_itemList.SelectedItem;
            Debug.Assert(item is not null, "No item selected!");
#endif
            return _active;
        }
    }

    public void SetItems(Form? active, Form[] all)
    {
        int selIndex = 0;
        for (int i = 0; i < all.Length; i++)
        {
            // Don't list non-visible windows
            if (all[i].Visible)
            {
                int n = _itemList.Items.Add(new ListItem(all[i]));
                if (all[i].Equals(active))
                {
                    selIndex = n;
                }
            }
        }

        _active = active;
        _itemList.SelectedIndex = selIndex;
    }

    private void ItemList_doubleClick(object? source, EventArgs e)
    {
        _okButton.PerformClick();
    }

    private void ItemList_selectedIndexChanged(object? source, EventArgs e)
    {
        ListItem? item = (ListItem?)_itemList.SelectedItem;
        if (item is not null)
        {
            _active = item.Form;
        }
    }

    /// <summary>
    ///  NOTE: The following code is required by the Windows Forms
    ///  designer.  It can be modified using the form editor.  Do not
    ///  modify it using the code editor.
    /// </summary>
    [MemberNotNull(nameof(_itemList))]
    [MemberNotNull(nameof(_okButton))]
    [MemberNotNull(nameof(_cancelButton))]
    [MemberNotNull(nameof(_okCancelTableLayoutPanel))]
    private void InitializeComponent()
    {
        if (!Control.EnableFeaturesNotSupportedWithTrimming)
        {
            throw new NotSupportedException(SR.BindingNotSupported);
        }

        System.ComponentModel.ComponentResourceManager resources = new(typeof(MdiWindowDialog));
        _itemList = new ListBox();
        _okButton = new Button();
        _cancelButton = new Button();
        _okCancelTableLayoutPanel = new TableLayoutPanel();
        _okCancelTableLayoutPanel.SuspendLayout();
        _itemList.DoubleClick += new EventHandler(ItemList_doubleClick);
        _itemList.SelectedIndexChanged += new EventHandler(ItemList_selectedIndexChanged);
        SuspendLayout();
        //
        // _itemList
        //
        // TypeDescriptor.RegisterType<ListBox>();
        resources.ApplyResourcesToRegisteredType(_itemList, "itemList", null);
        _itemList.FormattingEnabled = true;
        _itemList.Name = "itemList";
        //
        // _okButton
        //
        // TypeDescriptor.RegisterType<Button>();
        resources.ApplyResourcesToRegisteredType(_okButton, "okButton", null);
        _okButton.DialogResult = DialogResult.OK;
        _okButton.Margin = new Padding(0, 0, 3, 0);
        _okButton.Name = "okButton";
        //
        // _cancelButton
        //
        // TypeDescriptor.RegisterType<Button>();
        resources.ApplyResourcesToRegisteredType(_cancelButton, "cancelButton", null);
        _cancelButton.DialogResult = DialogResult.Cancel;
        _cancelButton.Margin = new Padding(3, 0, 0, 0);
        _cancelButton.Name = "cancelButton";
        //
        // _okCancelTableLayoutPanel
        //
        // TypeDescriptor.RegisterType<TableLayoutPanel>();
        resources.ApplyResourcesToRegisteredType(_okCancelTableLayoutPanel, "okCancelTableLayoutPanel", null);
        _okCancelTableLayoutPanel.ColumnCount = 2;
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        _okCancelTableLayoutPanel.Controls.Add(_okButton, 0, 0);
        _okCancelTableLayoutPanel.Controls.Add(_cancelButton, 1, 0);
        _okCancelTableLayoutPanel.Name = "okCancelTableLayoutPanel";
        _okCancelTableLayoutPanel.RowCount = 1;
        _okCancelTableLayoutPanel.RowStyles.Add(new RowStyle());
        //
        // MdiWindowDialog
        //
        // TypeDescriptor.RegisterType<MdiWindowDialog>();
        resources.ApplyResourcesToRegisteredType(this, "$this", null);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(_okCancelTableLayoutPanel);
        Controls.Add(_itemList);
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "MdiWindowDialog";
        ShowIcon = false;
        _okCancelTableLayoutPanel.ResumeLayout(false);
        _okCancelTableLayoutPanel.PerformLayout();
        AcceptButton = _okButton;
        CancelButton = _cancelButton;

        ResumeLayout(false);
        PerformLayout();
    }
}
