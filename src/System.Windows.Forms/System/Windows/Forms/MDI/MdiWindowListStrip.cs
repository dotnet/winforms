// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary> this is the menu that merges into the MdiWindowListItem
///  in an MDI parent when an MDI child is maximized.
/// </summary>
internal class MdiWindowListStrip : MenuStrip
{
    private Form? _mdiParent;
    private ToolStripMenuItem? _mergeItem;
    private MenuStrip? _mergedMenu;

    public MdiWindowListStrip()
    {
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _mdiParent = null;
        }

        base.Dispose(disposing);
    }

    internal ToolStripMenuItem MergeItem
    {
        get
        {
            _mergeItem ??= new ToolStripMenuItem
            {
                MergeAction = MergeAction.MatchOnly
            };

            if (_mergeItem.Owner is null)
            {
                Items.Add(_mergeItem);
            }

            return _mergeItem;
        }
    }

    internal MenuStrip? MergedMenu
    {
        get
        {
            return _mergedMenu;
        }
        set
        {
            _mergedMenu = value;
        }
    }

    /// <summary> Given a form, the items on this toolstrip populate with the mdi children
    ///  with mnemonics 1-9 and More Windows menu item.
    ///  These items can then be merged into a menustrip.
    ///
    ///  Based on similar code in MenuItem.cs::PopulateMdiList(), which is unfortunately just different
    ///  enough in its working environment that we can't readily combine the two.
    ///  But if you're fixing something here, chances are that the same issue will need scrutiny over there.
    /// </summary>
    public void PopulateItems(Form mdiParent, ToolStripMenuItem mdiMergeItem, bool includeSeparator)
    {
        _mdiParent = mdiParent;
        SuspendLayout();
        MergeItem.DropDown.SuspendLayout();
        try
        {
            ToolStripMenuItem mergeItem = MergeItem;
            mergeItem.DropDownItems.Clear();
            mergeItem.Text = mdiMergeItem.Text;

            Form[] forms = mdiParent.MdiChildren;
            if (forms is not null && forms.Length != 0)
            {
                if (includeSeparator)
                {
                    ToolStripSeparator separator = new ToolStripSeparator
                    {
                        MergeAction = MergeAction.Append,
                        MergeIndex = -1
                    };
                    mergeItem.DropDownItems.Add(separator);
                }

                Form? activeMdiChild = mdiParent.ActiveMdiChild;

                const int maxMenuForms = 9;  // max number of Window menu items for forms
                int visibleChildren = 0;     // number of visible child forms (so we know if we need to show More Windows...
                int accel = 1;               // prefix the form name with this digit, underlined, as an accelerator
                int formsAddedToMenu = 0;
                bool activeFormAdded = false;

                for (int i = 0; i < forms.Length; i++)
                {
                    // we need to check close reason here because we could be getting called
                    // here in the midst of a WM_CLOSE - WM_MDIDESTROY eventually fires a WM_MDIACTIVATE
                    if (forms[i].Visible && (forms[i].CloseReason == CloseReason.None))
                    {
                        visibleChildren++;
                        if ((activeFormAdded && (formsAddedToMenu < maxMenuForms)) ||   // don't exceed max
                            (!activeFormAdded && (formsAddedToMenu < (maxMenuForms - 1))) ||   // save room for active if it's not in yet
                            forms[i].Equals(activeMdiChild))
                        {
                            // there's always room for activeMdiChild
                            string? text = WindowsFormsUtils.EscapeTextWithAmpersands(mdiParent.MdiChildren[i].Text);
                            text ??= string.Empty;
                            ToolStripMenuItem windowListItem = new(mdiParent.MdiChildren[i])
                            {
                                Text = $"&{accel} {text}",
                                MergeAction = MergeAction.Append,
                                MergeIndex = accel
                            };

                            windowListItem.Click += OnWindowListItemClick;

                            if (forms[i].Equals(activeMdiChild))
                            {
                                // If this the active one, check it off.
                                windowListItem.Checked = true;
                                activeFormAdded = true;
                            }

                            accel++;
                            formsAddedToMenu++;
                            mergeItem.DropDownItems.Add(windowListItem);
                        }
                    }
                }

                // Show the "More Windows..." item if necessary.
                if (visibleChildren > maxMenuForms)
                {
                    ToolStripMenuItem moreWindowsMenuItem = new ToolStripMenuItem
                    {
                        Text = SR.MDIMenuMoreWindows
                    };

                    moreWindowsMenuItem.Click += OnMoreWindowsMenuItemClick;
                    moreWindowsMenuItem.MergeAction = MergeAction.Append;
                    mergeItem.DropDownItems.Add(moreWindowsMenuItem);
                }
            }
        }
        finally
        {
            // This is an invisible toolstrip don't even bother doing layout.
            ResumeLayout(false);
            MergeItem.DropDown.ResumeLayout(false);
        }
    }

    /// <summary> handler for More Windows... This is similar to MenuItem.cs</summary>
    private void OnMoreWindowsMenuItemClick(object? sender, EventArgs e)
    {
        Form[]? forms = _mdiParent?.MdiChildren;

        if (forms is not null)
        {
            using MdiWindowDialog dialog = new();
            dialog.SetItems(_mdiParent?.ActiveMdiChild, forms);
            DialogResult result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                // AllWindows Assert above allows this...
                dialog.ActiveChildForm?.Activate();
                if (dialog.ActiveChildForm?.ActiveControl is not null && !dialog.ActiveChildForm.ActiveControl.Focused)
                {
                    dialog.ActiveChildForm.ActiveControl.Focus();
                }
            }
        }
    }

    /// <summary> handler for 1 - 9. This is similar to MenuItem.cs</summary>
    private void OnWindowListItemClick(object? sender, EventArgs e)
    {
        if (sender is ToolStripMenuItem windowListItem)
        {
            Form? boundForm = windowListItem.MdiForm;

            if (boundForm is not null)
            {
                boundForm.Activate();
                if (boundForm.ActiveControl is not null && !boundForm.ActiveControl.Focused)
                {
                    boundForm.ActiveControl.Focus();
                }
            }
        }
    }
}
