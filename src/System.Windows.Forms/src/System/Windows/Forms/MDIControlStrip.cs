// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is the toolstrip used for merging the [:)]    [_][#][X] buttons onto an
    ///  mdi parent when an MDI child is maximized.
    /// </summary>
    internal class MdiControlStrip : MenuStrip
    {
        private readonly ToolStripMenuItem system;
        private readonly ToolStripMenuItem close;
        private readonly ToolStripMenuItem minimize;
        private readonly ToolStripMenuItem restore;
        private MenuStrip mergedMenu;

        private IWin32Window target;

        /// <summary> target is ideally the MDI Child to send the system commands to.
        ///  although there's nothing MDI child specific to it... you could have this
        ///  a toplevel window.
        /// </summary>
        public MdiControlStrip(IWin32Window target)
        {
            IntPtr hMenu = UnsafeNativeMethods.GetSystemMenu(new HandleRef(this, Control.GetSafeHandle(target)), /*bRevert=*/false);
            this.target = target;

            // The menu item itself takes care of enabledness and sending WM_SYSCOMMAND messages to the target.
            minimize = new ControlBoxMenuItem(hMenu, NativeMethods.SC_MINIMIZE, target);
            close = new ControlBoxMenuItem(hMenu, NativeMethods.SC_CLOSE, target);
            restore = new ControlBoxMenuItem(hMenu, NativeMethods.SC_RESTORE, target);

            // The dropDown of the system menu is the one that talks to native.
            system = new SystemMenuItem();

            // However in the event that the target handle changes we have to push the new handle into everyone.
            if (target is Control controlTarget)
            {
                controlTarget.HandleCreated += new EventHandler(OnTargetWindowHandleRecreated);
                controlTarget.Disposed += new EventHandler(OnTargetWindowDisposed);
            }

            // add in opposite order to how you want it merged
            Items.AddRange(new ToolStripItem[] { minimize, restore, close, system });
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

            // set up the sytem menu

            system.Image = GetTargetWindowIcon();
            system.Alignment = ToolStripItemAlignment.Left;
            system.DropDownOpening += new EventHandler(OnSystemMenuDropDownOpening);
            system.ImageScaling = ToolStripItemImageScaling.None;
            system.DoubleClickEnabled = true;
            system.DoubleClick += new EventHandler(OnSystemMenuDoubleClick);
            system.Padding = Padding.Empty;
            system.ShortcutKeys = Keys.Alt | Keys.OemMinus;
            ResumeLayout(false);
        }

        #region Buttons
        /* Unused
        public ToolStripMenuItem System {
            get { return system; }
        }
        */

        public ToolStripMenuItem Close
        {
            get { return close; }
        }

        /* Unused
        public ToolStripMenuItem Minimize {
            get { return minimize; }
        }

        public ToolStripMenuItem Restore {
            get { return restore; }
        }
        */
        #endregion

        internal MenuStrip MergedMenu
        {
            get
            {
                return mergedMenu;
            }
            set
            {
                mergedMenu = value;
            }
        }

        /* PERF: consider shutting off layout
        #region ShutOffLayout
                    protected override void OnLayout(LayoutEventArgs e) {
                        return;  // if someone attempts
                    }

                    protected override Size GetPreferredSize(Size proposedSize) {
                        return Size.Empty;
                    }
        #endregion
        */

        private Image GetTargetWindowIcon()
        {
            Image systemIcon = null;
            IntPtr hIcon = UnsafeNativeMethods.SendMessage(new HandleRef(this, Control.GetSafeHandle(target)), WindowMessages.WM_GETICON, NativeMethods.ICON_SMALL, 0);
            Icon icon = (hIcon != IntPtr.Zero) ? Icon.FromHandle(hIcon) : Form.DefaultIcon;
            Icon smallIcon = new Icon(icon, SystemInformation.SmallIconSize);

            systemIcon = smallIcon.ToBitmap();
            smallIcon.Dispose();

            return systemIcon;
        }

        protected internal override void OnItemAdded(ToolStripItemEventArgs e)
        {
            base.OnItemAdded(e);
            Debug.Assert(Items.Count <= 4, "Too many items in the MDIControlStrip.  How did we get into this situation?");
        }

        private void OnTargetWindowDisposed(object sender, EventArgs e)
        {
            UnhookTarget();
            target = null;
        }

        private void OnTargetWindowHandleRecreated(object sender, EventArgs e)
        {
            // in the case that the handle for the form is recreated we need to set
            // up the handles to point to the new window handle for the form.

            system.SetNativeTargetWindow(target);
            minimize.SetNativeTargetWindow(target);
            close.SetNativeTargetWindow(target);
            restore.SetNativeTargetWindow(target);

            IntPtr hMenu = UnsafeNativeMethods.GetSystemMenu(new HandleRef(this, Control.GetSafeHandle(target)), /*bRevert=*/false);
            system.SetNativeTargetMenu(hMenu);
            minimize.SetNativeTargetMenu(hMenu);
            close.SetNativeTargetMenu(hMenu);
            restore.SetNativeTargetMenu(hMenu);

            // clear off the System DropDown.
            if (system.HasDropDownItems)
            {
                // next time we need one we'll just fetch it fresh.
                system.DropDown.Items.Clear();
                system.DropDown.Dispose();
            }

            system.Image = GetTargetWindowIcon();
        }

        private void OnSystemMenuDropDownOpening(object sender, EventArgs e)
        {
            if (!system.HasDropDownItems && (target != null))
            {
                system.DropDown = ToolStripDropDownMenu.FromHMenu(UnsafeNativeMethods.GetSystemMenu(new HandleRef(this, Control.GetSafeHandle(target)), /*bRevert=*/false), target);
            }
            else if (MergedMenu == null)
            {
                system.DropDown.Dispose();
            }
        }

        private void OnSystemMenuDoubleClick(object sender, EventArgs e)
        {
            Close.PerformClick();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnhookTarget();
                target = null;
            }
            base.Dispose(disposing);
        }

        private void UnhookTarget()
        {
            if (target != null)
            {
                if (target is Control controlTarget)
                {
                    controlTarget.HandleCreated -= new EventHandler(OnTargetWindowHandleRecreated);
                    controlTarget.Disposed -= new EventHandler(OnTargetWindowDisposed);
                }
                target = null;
            }

        }

        // when the system menu item shortcut is evaluated - pop the dropdown
        internal class ControlBoxMenuItem : ToolStripMenuItem
        {
            internal ControlBoxMenuItem(IntPtr hMenu, int nativeMenuCommandId, IWin32Window targetWindow) :
                                        base(hMenu, nativeMenuCommandId, targetWindow)
            {
            }

            internal override bool CanKeyboardSelect
            {
                get
                {
                    return false;
                }
            }
        }

        // when the system menu item shortcut is evaluated - pop the dropdown
        internal class SystemMenuItem : ToolStripMenuItem
        {
            public SystemMenuItem()
            {
                AccessibleName = SR.MDIChildSystemMenuItemAccessibleName;
            }
            protected internal override bool ProcessCmdKey(ref Message m, Keys keyData)
            {
                if (Visible && ShortcutKeys == keyData)
                {
                    ShowDropDown();
                    DropDown.SelectNextToolStripItem(null, true);
                    return true;
                }
                return base.ProcessCmdKey(ref m, keyData);
            }
            protected override void OnOwnerChanged(EventArgs e)
            {
                if (HasDropDownItems && DropDown.Visible)
                {
                    HideDropDown();
                }
                base.OnOwnerChanged(e);
            }

        }
    }
}
