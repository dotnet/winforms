// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

[SRDescription(nameof(SR.DescriptionMenuStrip))]
public partial class MenuStrip : ToolStrip
{
    private ToolStripMenuItem? _mdiWindowListItem;

    private static readonly object s_menuActivateEvent = new();
    private static readonly object s_menuDeactivateEvent = new();

    public MenuStrip()
    {
        CanOverflow = false;
        GripStyle = ToolStripGripStyle.Hidden;
        Stretch = true;
    }

    internal override bool KeyboardActive
    {
        get => base.KeyboardActive;

        set
        {
            if (base.KeyboardActive != value)
            {
                base.KeyboardActive = value;
                if (value)
                {
                    OnMenuActivate(EventArgs.Empty);
                }
                else
                {
                    OnMenuDeactivate(EventArgs.Empty);
                }
            }
        }
    }

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripCanOverflowDescr))]
    [SRCategory(nameof(SR.CatLayout))]
    [Browsable(false)]
    public new bool CanOverflow
    {
        get => base.CanOverflow;
        set => base.CanOverflow = value;
    }

    protected override bool DefaultShowItemToolTips
        => false;

    protected override Padding DefaultGripMargin =>
        // MenuStrip control is scaled by Control::ScaleControl()
        // Ensure grip aligns properly when set visible.
        ScaleHelper.IsThreadPerMonitorV2Aware ?
                ScaleHelper.ScaleToDpi(new Padding(2, 2, 0, 2), DeviceDpi) :
                new Padding(2, 2, 0, 2);

    protected override Size DefaultSize =>
        ScaleHelper.IsThreadPerMonitorV2Aware ?
           ScaleHelper.ScaleToDpi(new Size(200, 24), DeviceDpi) :
           new Size(200, 24);

    protected override Padding DefaultPadding
    {
        get
        {
            // MenuStrip control is scaled by Control::ScaleControl()
            // Scoot the grip over when present
            return GripStyle == ToolStripGripStyle.Visible
                ? ScaleHelper.IsThreadPerMonitorV2Aware
                    ? ScaleHelper.ScaleToDpi(new Padding(3, 2, 0, 2), DeviceDpi)
                    : new Padding(3, 2, 0, 2)
                : ScaleHelper.IsThreadPerMonitorV2Aware
                    ? ScaleHelper.ScaleToDpi(new Padding(6, 2, 0, 2), DeviceDpi)
                    : new Padding(6, 2, 0, 2);
        }
    }

    [DefaultValue(ToolStripGripStyle.Hidden)]
    public new ToolStripGripStyle GripStyle
    {
        get => base.GripStyle;
        set => base.GripStyle = value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MenuStripMenuActivateDescr))]
    public event EventHandler? MenuActivate
    {
        add => Events.AddHandler(s_menuActivateEvent, value);
        remove => Events.RemoveHandler(s_menuActivateEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.MenuStripMenuDeactivateDescr))]
    public event EventHandler? MenuDeactivate
    {
        add => Events.AddHandler(s_menuDeactivateEvent, value);
        remove => Events.RemoveHandler(s_menuDeactivateEvent, value);
    }

    [DefaultValue(false)]
    [SRDescription(nameof(SR.ToolStripShowItemToolTipsDescr))]
    [SRCategory(nameof(SR.CatBehavior))]
    public new bool ShowItemToolTips
    {
        get => base.ShowItemToolTips;
        set => base.ShowItemToolTips = value;
    }

    [DefaultValue(true)]
    [SRCategory(nameof(SR.CatLayout))]
    [SRDescription(nameof(SR.ToolStripStretchDescr))]
    public new bool Stretch
    {
        get => base.Stretch;
        set => base.Stretch = value;
    }

    [DefaultValue(null)]
    [MergableProperty(false)]
    [SRDescription(nameof(SR.MenuStripMdiWindowListItem))]
    [SRCategory(nameof(SR.CatBehavior))]
    [TypeConverter(typeof(MdiWindowListItemConverter))]
    public ToolStripMenuItem? MdiWindowListItem
    {
        get => _mdiWindowListItem;
        set => _mdiWindowListItem = value;
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new MenuStripAccessibleObject(this);

    protected internal override ToolStripItem CreateDefaultItem(string? text, Image? image, EventHandler? onClick)
    {
        if (text == "-")
        {
            return new ToolStripSeparator();
        }
        else
        {
            return new ToolStripMenuItem(text, image, onClick);
        }
    }

    internal override ToolStripItem? GetNextItem(ToolStripItem? start, ArrowDirection direction, bool rtlAware)
    {
        ToolStripItem? nextItem = base.GetNextItem(start, direction, rtlAware);
        if (nextItem is MdiControlStrip.SystemMenuItem)
        {
            nextItem = base.GetNextItem(nextItem, direction, rtlAware);
        }

        return nextItem;
    }

    protected virtual void OnMenuActivate(EventArgs e)
    {
        if (IsHandleCreated)
        {
            if (!TabStop && !DesignMode && IsAccessibilityObjectCreated)
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_MenuModeStartEventId);
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_MenuOpenedEventId);
            }

            AccessibilityNotifyClients(AccessibleEvents.SystemMenuStart, (int)OBJECT_IDENTIFIER.OBJID_MENU, -1);
        }

        ((EventHandler?)Events[s_menuActivateEvent])?.Invoke(this, e);
    }

    protected virtual void OnMenuDeactivate(EventArgs e)
    {
        if (IsHandleCreated)
        {
            AccessibilityNotifyClients(AccessibleEvents.SystemMenuEnd, (int)OBJECT_IDENTIFIER.OBJID_MENU, -1);

            if (!TabStop && !DesignMode && IsAccessibilityObjectCreated)
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_MenuClosedEventId);
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_MenuModeEndEventId);
            }
        }

        ((EventHandler?)Events[s_menuDeactivateEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  Called from ToolStripManager.ProcessMenuKey. Fires MenuActivate event and sets focus.
    /// </summary>
    internal bool OnMenuKey()
    {
        if (!(Focused || ContainsFocus))
        {
            ToolStripManager.ModalMenuFilter.SetActiveToolStrip(this, menuKeyPressed: true);

            if (DisplayedItems.Count > 0)
            {
                if (DisplayedItems[0] is MdiControlStrip.SystemMenuItem)
                {
                    SelectNextToolStripItem(DisplayedItems[0], forward: true);
                }
                else
                {
                    // first alt should select "File". Future keydowns of alt should restore focus.
                    SelectNextToolStripItem(null, forward: RightToLeft == RightToLeft.No);
                }
            }

            return true;
        }

        return false;
    }

    protected override bool ProcessCmdKey(ref Message m, Keys keyData)
    {
        if (ToolStripManager.ModalMenuFilter.InMenuMode)
        {
            // ALT, then space should dismiss the menu and activate the system menu.
            if (keyData == Keys.Space)
            {
                // If we're focused it's ok to activate system menu. If we're not focused - we should not activate if
                // we contain focus - this means a text box or something has focus.
                if (Focused || !ContainsFocus)
                {
                    NotifySelectionChange(item: null);
                    ToolStripManager.ModalMenuFilter.ExitMenuMode();

                    // Send a WM_SYSCOMMAND SC_KEYMENU + Space to activate the system menu.
                    HWND ancestor = PInvoke.GetAncestor(this, GET_ANCESTOR_FLAGS.GA_ROOT);
                    PInvokeCore.PostMessage(ancestor, PInvokeCore.WM_SYSCOMMAND, (WPARAM)PInvoke.SC_KEYMENU, (LPARAM)(int)Keys.Space);
                    return true;
                }
            }
        }

        return base.ProcessCmdKey(ref m, keyData);
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == (int)PInvokeCore.WM_MOUSEACTIVATE && (ActiveDropDowns.Count == 0))
        {
            // call menu activate before we actually take focus.
            Point pt = PointToClient(WindowsFormsUtils.LastCursorPoint);
            ToolStripItem? item = GetItemAt(pt);
            if (item is not null && !(item is ToolStripControlHost))
            {
                // verify the place where we've clicked is a place where we have to do "fake" focus
                // e.g. an item that isn't a control.
                KeyboardActive = true;
            }
        }

        base.WndProc(ref m);
    }
}
