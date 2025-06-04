// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

/// <summary>
///  This class is just a conceptual wrapper around ToolStripDropDownMenu.
/// </summary>
[DefaultEvent(nameof(Opening))]
[SRDescription(nameof(SR.DescriptionContextMenuStrip))]
public class ContextMenuStrip : ToolStripDropDownMenu
{
    public ContextMenuStrip(IContainer container) : base()
    {
        // this constructor ensures ContextMenuStrip is disposed properly since its not parented to the form.
        ArgumentNullException.ThrowIfNull(container);

        container.Add(this);
    }

    public ContextMenuStrip()
    {
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
    }

    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ContextMenuStripSourceControlDescr))]
    public Control? SourceControl
    {
        get
        {
            return SourceControlInternal;
        }
    }

    // minimal Clone implementation for DGV support only.
    internal ContextMenuStrip Clone()
    {
        // VERY limited support for cloning.

        ContextMenuStrip contextMenuStrip = new();

        // copy over events
        contextMenuStrip.Events.AddHandlers(Events);

        contextMenuStrip.AutoClose = AutoClose;
        contextMenuStrip.AutoSize = AutoSize;
        contextMenuStrip.Bounds = Bounds;
        contextMenuStrip.ImageList = ImageList;
        contextMenuStrip.ShowCheckMargin = ShowCheckMargin;
        contextMenuStrip.ShowImageMargin = ShowImageMargin;

        // copy over relevant properties

        for (int i = 0; i < Items.Count; i++)
        {
            ToolStripItem item = Items[i];

            if (item is ToolStripSeparator)
            {
                contextMenuStrip.Items.Add(new ToolStripSeparator());
            }
            else if (item is ToolStripMenuItem toolStripMenuItem)
            {
                contextMenuStrip.Items.Add(toolStripMenuItem.Clone());
            }
        }

        return contextMenuStrip;
    }

    // internal overload so we know whether or not to show mnemonics.
    internal void ShowInternal(Control source, Point location, bool isKeyboardActivated)
    {
        Show(source, location);

        // if we were activated by keyboard - show mnemonics.
        if (isKeyboardActivated)
        {
            ToolStripManager.ModalMenuFilter.Instance.ShowUnderlines = true;
        }
    }

    internal void ShowInTaskbar(int x, int y)
    {
        // we need to make ourselves a topmost window
        WorkingAreaConstrained = false;
        Rectangle bounds = CalculateDropDownLocation(new Point(x, y), ToolStripDropDownDirection.AboveLeft);
        Rectangle screenBounds = Screen.FromRectangle(bounds).Bounds;
        if (bounds.Y < screenBounds.Y)
        {
            bounds = CalculateDropDownLocation(new Point(x, y), ToolStripDropDownDirection.BelowLeft);
        }
        else if (bounds.X < screenBounds.X)
        {
            bounds = CalculateDropDownLocation(new Point(x, y), ToolStripDropDownDirection.AboveRight);
        }

        bounds = WindowsFormsUtils.ConstrainToBounds(screenBounds, bounds);

        Show(bounds.X, bounds.Y);
    }

    protected override void SetVisibleCore(bool visible)
    {
        if (!visible)
        {
            WorkingAreaConstrained = true;
        }

        base.SetVisibleCore(visible);

        // There are two problems we're facing when trying to scale ContextMenuStrip:
        //    1. ContextMenuStrip is a top level window and thus only receive "WM_DPICHANGED" message
        //       but not WM_DPICHANGED_BEFOREPARENT/WM_DPICHANGED_AFTERPARENT.
        //       Unfortunately, we do not handle scaling of controls in "WM_DPICHANGED" message.
        //       In WinForms, "WM_DPICHANGED" message is intended for Top-level Forms/ContainerControls.
        //       As a result, the ContextMenuStrip window doesn't scale itself when moved from one monitor
        //       to another on the "PerMonitorV2" process.
        //    2. When ContextMenuStrip changes to invisible(with "visible" set to false), owner of the window
        //       is changed to a ParkingWindow that may possibly parked on primary monitor.
        //       The "GetDpiForWindow()" API on ContextMenuStrip thus returns the DPI of the primary monitor.
        // Because of this inconsistency, we intentionally recreate the handle that triggers scaling according
        // to the new DPI, after setting the "visible" property.
        if (visible
            && IsHandleCreated
            && ScaleHelper.IsThreadPerMonitorV2Aware
            && DeviceDpi != (int)PInvoke.GetDpiForWindow(this))
        {
            RecreateHandle();
        }
    }

    protected override void OnOpened(EventArgs e)
    {
        if (IsHandleCreated && !IsInDesignMode)
        {
            AccessibilityNotifyClients(AccessibleEvents.SystemMenuPopupStart, -1);

            if (IsAccessibilityObjectCreated)
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_MenuOpenedEventId);
            }
        }

        base.OnOpened(e);
    }

    protected override void OnClosed(ToolStripDropDownClosedEventArgs e)
    {
        base.OnClosed(e);

        if (IsHandleCreated && !IsInDesignMode)
        {
            if (IsAccessibilityObjectCreated)
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_MenuClosedEventId);
            }

            AccessibilityNotifyClients(AccessibleEvents.SystemMenuPopupEnd, -1);
        }
    }
}
