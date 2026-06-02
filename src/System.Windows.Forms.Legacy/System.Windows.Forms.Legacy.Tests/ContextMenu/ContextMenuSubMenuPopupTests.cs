// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace System.Windows.Forms.Legacy.Tests;

/// <summary>
///  Regression tests for WM_INITMENUPOPUP dispatch on a plain <see cref="Control"/> that
///  owns a <see cref="ContextMenu"/>.
/// </summary>
/// <remarks>
///  <para>
///   When a <see cref="ContextMenu"/> is shown via TrackPopupMenuEx, Windows posts
///   WM_INITMENUPOPUP to the owner control's HWND each time a submenu is about to open.
///   <see cref="Menu.ProcessInitMenuPopup"/> must be called with the submenu's HMENU so that
///   the matching <see cref="MenuItem.Popup"/> event fires, giving dynamic-menu providers the chance
///   to replace placeholder items with real entries.
///  </para>
///  <para>
///   Without the fix in <see cref="Control.WndProc"/>, WM_INITMENUPOPUP falls through to
///   DefWndProc (grouped with WM_EXITMENULOOP / default), so
///   <see cref="MenuItem.Popup"/> never fires and placeholder items are never replaced.
///  </para>
/// </remarks>
public class ContextMenuSubMenuPopupTests
{
    private const uint WM_INITMENUPOPUP = 0x0117;

    [DllImport("user32.dll")]
    private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    ///  Verifies that sending WM_INITMENUPOPUP to a plain <see cref="Control"/>'s HWND with
    ///  a direct child <see cref="MenuItem"/>'s HMENU fires that item's <see cref="MenuItem.Popup"/> event
    ///  and allows the handler to replace placeholder items with real entries.
    /// </summary>
    [StaFact]
    public void Control_WmInitMenuPopup_DirectSubMenu_FiresPopupEvent()
    {
        // Arrange: a submenu seeded with a placeholder item, mirroring the dynamic-menu pattern
        // where real items are loaded lazily on first hover.
        const string Placeholder = "<PlaceHolder>";

        using Control control = new() { Visible = true };

        MenuItem subMenu = new("SubMenu");
        subMenu.MenuItems.Add(Placeholder);

        bool popupFired = false;
        subMenu.Popup += (sender, e) =>
        {
            popupFired = true;
            subMenu.MenuItems.Clear();
            subMenu.MenuItems.Add("DynamicItem1");
            subMenu.MenuItems.Add("DynamicItem2");
        };

        ContextMenu contextMenu = new(new MenuItem[] { subMenu });
        control.ContextMenu = contextMenu;

        // Force native HWND and the submenu's HMENU so ProcessInitMenuPopup can match by handle.
        IntPtr controlHandle = control.Handle;
        IntPtr subMenuHandle = subMenu.Handle;

        Assert.NotEqual(IntPtr.Zero, controlHandle);
        Assert.NotEqual(IntPtr.Zero, subMenuHandle);

        // Act: simulate Windows delivering WM_INITMENUPOPUP to the control's HWND for the submenu.
        //
        // Before the fix, Control.WndProc groups WM_INITMENUPOPUP with WM_EXITMENULOOP and calls
        // DefWndProc — ContextMenu.ProcessInitMenuPopup is never reached, Popup never fires, and
        // the placeholder is never replaced.
        SendMessage(controlHandle, WM_INITMENUPOPUP, subMenuHandle, IntPtr.Zero);

        // Assert
        Assert.True(
            popupFired,
            "MenuItem.Popup must fire when WM_INITMENUPOPUP is sent to the owning control's HWND. "
            + "Without the fix in Control.WndProc the message is dropped and the placeholder is never replaced.");

        Assert.Equal(2, subMenu.MenuItems.Count);
        Assert.Equal("DynamicItem1", subMenu.MenuItems[0].Text);
        Assert.Equal("DynamicItem2", subMenu.MenuItems[1].Text);
    }

    /// <summary>
    ///  Verifies that WM_INITMENUPOPUP correctly dispatches to a nested (non-direct-child)
    ///  submenu item. <see cref="Menu.FindMenuItem"/> recurses into descendants, so the event must
    ///  fire regardless of nesting depth.
    /// </summary>
    [StaFact]
    public void Control_WmInitMenuPopup_NestedSubMenu_FiresPopupEvent()
    {
        // Arrange: ContextMenu → ParentMenu → NestedSubMenu (nested submenu with placeholder)
        using Control control = new() { Visible = true };

        MenuItem nestedItem = new("NestedSubMenu");
        nestedItem.MenuItems.Add("<PlaceHolder>");

        bool nestedPopupFired = false;
        nestedItem.Popup += (sender, e) =>
        {
            nestedPopupFired = true;
            nestedItem.MenuItems.Clear();
            nestedItem.MenuItems.Add("NestedDynamicItem");
        };

        MenuItem parentMenu = new("ParentMenu");
        parentMenu.MenuItems.Add(nestedItem);

        ContextMenu contextMenu = new(new MenuItem[] { parentMenu });
        control.ContextMenu = contextMenu;

        IntPtr controlHandle = control.Handle;
        IntPtr nestedSubMenuHandle = nestedItem.Handle;

        Assert.NotEqual(IntPtr.Zero, nestedSubMenuHandle);

        // Act
        SendMessage(controlHandle, WM_INITMENUPOPUP, nestedSubMenuHandle, IntPtr.Zero);

        // Assert
        Assert.True(
            nestedPopupFired,
            "MenuItem.Popup must fire for a nested submenu when WM_INITMENUPOPUP targets its HMENU.");

        Assert.Equal(1, nestedItem.MenuItems.Count);
        Assert.Equal("NestedDynamicItem", nestedItem.MenuItems[0].Text);
    }
}
