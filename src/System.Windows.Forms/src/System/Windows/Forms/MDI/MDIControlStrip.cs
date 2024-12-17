// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms;

/// <summary>
///  This is the toolstrip used for merging the [:)]    [_][#][X] buttons onto an
///  mdi parent when an MDI child is maximized.
/// </summary>
internal partial class MdiControlStrip : MenuStrip
{
    private readonly ToolStripMenuItem _system;
    private readonly ToolStripMenuItem _close;
    private readonly ToolStripMenuItem _minimize;
    private readonly ToolStripMenuItem _restore;
    private IWin32Window _target;

    /// <summary>
    ///  <paramref name="target"/> is ideally the MDI Child to send the system commands to.
    ///  Although there's nothing MDI child specific to it- it could be a top level window.
    /// </summary>
    public MdiControlStrip(IWin32Window target)
    {
        HMENU hMenu = PInvoke.GetSystemMenu(GetSafeHandle(target), bRevert: false);
        _target = target;

        // The menu item itself takes care of enabledness and sending WM_SYSCOMMAND messages to the target.
        _minimize = new ControlBoxMenuItem(hMenu, PInvoke.SC_MINIMIZE, target);
        _close = new ControlBoxMenuItem(hMenu, PInvoke.SC_CLOSE, target);
        _restore = new ControlBoxMenuItem(hMenu, PInvoke.SC_RESTORE, target);

        // The dropDown of the system menu is the one that talks to native.
        _system = new SystemMenuItem();

        // However in the event that the target handle changes we have to push the new handle into everyone.
        if (target is Control controlTarget)
        {
            controlTarget.HandleCreated += OnTargetWindowHandleRecreated;
            controlTarget.Disposed += OnTargetWindowDisposed;
        }

        // add in opposite order to how you want it merged
        Items.AddRange(_minimize, _restore, _close, _system);

        SuspendLayout();
        foreach (ToolStripItem item in Items)
        {
            item.DisplayStyle = ToolStripItemDisplayStyle.Image;
            item.MergeIndex = 0;
            item.MergeAction = MergeAction.Insert;
            item.Overflow = ToolStripItemOverflow.Never;
            item.Alignment = ToolStripItemAlignment.Right;
            item.Padding = Padding.Empty;
            // image is not scaled well on high dpi devices. Setting property to fit to size.
            item.ImageScaling = ToolStripItemImageScaling.SizeToFit;
        }

        // set up the system menu

        _system.Image = GetTargetWindowIcon();
        _system.Visible = GetTargetWindowIconVisibility();
        _system.Alignment = ToolStripItemAlignment.Left;
        _system.DropDownOpening += OnSystemMenuDropDownOpening;
        _system.ImageScaling = ToolStripItemImageScaling.None;
        _system.DoubleClickEnabled = true;
        _system.DoubleClick += OnSystemMenuDoubleClick;
        _system.Padding = Padding.Empty;
        _system.ShortcutKeys = Keys.Alt | Keys.OemMinus;
        ResumeLayout(false);
    }

    public ToolStripMenuItem Close => _close;

    internal MenuStrip? MergedMenu { get; set; }

    private Bitmap GetTargetWindowIcon()
    {
        HICON hIcon = (HICON)PInvokeCore.SendMessage(GetSafeHandle(_target), PInvokeCore.WM_GETICON, (WPARAM)PInvoke.ICON_SMALL);
        Icon icon = !hIcon.IsNull ? Icon.FromHandle(hIcon) : Form.DefaultIcon;
        using Icon smallIcon = new(icon, SystemInformation.SmallIconSize);
        return smallIcon.ToBitmap();
    }

    private bool GetTargetWindowIconVisibility() => _target is not Form formTarget || formTarget.ShowIcon;

    public void updateIcon()
    {
        _system.Image = GetTargetWindowIcon();
        _system.Visible = GetTargetWindowIconVisibility();
    }

    protected internal override void OnItemAdded(ToolStripItemEventArgs e)
    {
        base.OnItemAdded(e);
        Debug.Assert(Items.Count <= 4, "Too many items in the MDIControlStrip. How did we get into this situation?");
    }

    private void OnTargetWindowDisposed(object? sender, EventArgs e)
    {
        UnhookTarget();
        _target = null!;
    }

    private void OnTargetWindowHandleRecreated(object? sender, EventArgs e)
    {
        // in the case that the handle for the form is recreated we need to set
        // up the handles to point to the new window handle for the form.

        _system.SetNativeTargetWindow(_target);
        _minimize.SetNativeTargetWindow(_target);
        _close.SetNativeTargetWindow(_target);
        _restore.SetNativeTargetWindow(_target);

        HMENU hmenu = PInvoke.GetSystemMenu(GetSafeHandle(_target), bRevert: false);
        _system.SetNativeTargetMenu(hmenu);
        _minimize.SetNativeTargetMenu(hmenu);
        _close.SetNativeTargetMenu(hmenu);
        _restore.SetNativeTargetMenu(hmenu);

        // clear off the System DropDown.
        if (_system.HasDropDownItems)
        {
            // next time we need one we'll just fetch it fresh.
            _system.DropDown.Items.Clear();
            _system.DropDown.Dispose();
        }

        _system.Image = GetTargetWindowIcon();
        _system.Visible = GetTargetWindowIconVisibility();
    }

    private void OnSystemMenuDropDownOpening(object? sender, EventArgs e)
    {
        if (!_system.HasDropDownItems && (_target is not null))
        {
            _system.DropDown = ToolStripDropDownMenu.FromHMenu(PInvoke.GetSystemMenu(GetSafeHandle(_target), bRevert: false), _target);
        }
        else if (MergedMenu is null)
        {
            _system.DropDown.Dispose();
        }
    }

    private void OnSystemMenuDoubleClick(object? sender, EventArgs e)
    {
        Close.PerformClick();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnhookTarget();
            _target = null!;
        }

        base.Dispose(disposing);
    }

    private void UnhookTarget()
    {
        if (_target is not null)
        {
            if (_target is Control controlTarget)
            {
                controlTarget.HandleCreated -= OnTargetWindowHandleRecreated;
                controlTarget.Disposed -= OnTargetWindowDisposed;
            }

            _target = null!;
        }
    }
}
