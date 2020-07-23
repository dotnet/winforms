// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     SRDescription(nameof(SR.DescriptionMenuStrip))
    ]
    public class MenuStrip : ToolStrip
    {
        private ToolStripMenuItem mdiWindowListItem = null;

        private static readonly object EventMenuActivate = new object();
        private static readonly object EventMenuDeactivate = new object();

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

        [
        DefaultValue(false),
        SRDescription(nameof(SR.ToolStripCanOverflowDescr)),
        SRCategory(nameof(SR.CatLayout)),
        Browsable(false)
        ]
        public new bool CanOverflow
        {
            get => base.CanOverflow;
            set => base.CanOverflow = value;
        }
        protected override bool DefaultShowItemToolTips
            => false;

        protected override Padding DefaultGripMargin
            =>
                // MenuStrip control is scaled by Control::ScaleControl()
                // Ensure grip aligns properly when set visible.
                DpiHelper.IsPerMonitorV2Awareness ?
                       DpiHelper.LogicalToDeviceUnits(new Padding(2, 2, 0, 2), DeviceDpi) :
                       new Padding(2, 2, 0, 2);

        /// <include file='doc\MenuStrip.uex' path='docs/doc[@for="MenuStrip.DefaultSize"]/*' />
        protected override Size DefaultSize
            => DpiHelper.IsPerMonitorV2Awareness ?
               DpiHelper.LogicalToDeviceUnits(new Size(200, 24), DeviceDpi) :
               new Size(200, 24);

        protected override Padding DefaultPadding
        {
            get
            {
                // MenuStrip control is scaled by Control::ScaleControl()
                // Scoot the grip over when present
                if (GripStyle == ToolStripGripStyle.Visible)
                {
                    return DpiHelper.IsPerMonitorV2Awareness ?
                           DpiHelper.LogicalToDeviceUnits(new Padding(3, 2, 0, 2), DeviceDpi) :
                           new Padding(3, 2, 0, 2);
                }
                return DpiHelper.IsPerMonitorV2Awareness ?
                       DpiHelper.LogicalToDeviceUnits(new Padding(6, 2, 0, 2), DeviceDpi) :
                       new Padding(6, 2, 0, 2);
            }
        }

        [DefaultValue(ToolStripGripStyle.Hidden)]
        public new ToolStripGripStyle GripStyle
        {
            get => base.GripStyle;
            set => base.GripStyle = value;
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.MenuStripMenuActivateDescr))]
        public event EventHandler MenuActivate
        {
            add => Events.AddHandler(EventMenuActivate, value);
            remove => Events.RemoveHandler(EventMenuActivate, value);
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.MenuStripMenuDeactivateDescr))]
        public event EventHandler MenuDeactivate
        {
            add => Events.AddHandler(EventMenuDeactivate, value);
            remove => Events.RemoveHandler(EventMenuDeactivate, value);
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
        public ToolStripMenuItem MdiWindowListItem
        {
            get => mdiWindowListItem;
            set => mdiWindowListItem = value;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
            => new MenuStripAccessibleObject(this);

        protected internal override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick)
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

        internal override ToolStripItem GetNextItem(ToolStripItem start, ArrowDirection direction, bool rtlAware)
        {
            ToolStripItem nextItem = base.GetNextItem(start, direction, rtlAware);
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
                AccessibilityNotifyClients(AccessibleEvents.SystemMenuStart, -1);
            } ((EventHandler)Events[EventMenuActivate])?.Invoke(this, e);
        }

        protected virtual void OnMenuDeactivate(EventArgs e)
        {
            if (IsHandleCreated)
            {
                AccessibilityNotifyClients(AccessibleEvents.SystemMenuEnd, -1);
            } ((EventHandler)Events[EventMenuDeactivate])?.Invoke(this, e);
        }

        /// <summary>
        ///  Called from ToolStripManager.ProcessMenuKey.  Fires MenuActivate event and sets focus.
        /// </summary>
        internal bool OnMenuKey()
        {
            if (!(Focused || ContainsFocus))
            {
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] set focus to menustrip");
                ToolStripManager.ModalMenuFilter.SetActiveToolStrip(this, /*menuKeyPressed=*/true);

                if (DisplayedItems.Count > 0)
                {
                    if (DisplayedItems[0] is MdiControlStrip.SystemMenuItem)
                    {
                        SelectNextToolStripItem(DisplayedItems[0], /*forward=*/true);
                    }
                    else
                    {
                        // first alt should select "File".  Future keydowns of alt should restore focus.
                        SelectNextToolStripItem(null, /*forward=*/(RightToLeft == RightToLeft.No));
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
                    // if we're focused it's ok to activate system menu
                    // if we're not focused - we should not activate if we contain focus - this means a text box or something
                    // has focus.
                    if (Focused || !ContainsFocus)
                    {
                        NotifySelectionChange(null);
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[MenuStrip.ProcessCmdKey] Rolling up the menu and invoking the system menu");
                        ToolStripManager.ModalMenuFilter.ExitMenuMode();
                        // send a WM_SYSCOMMAND SC_KEYMENU + Space to activate the system menu.
                        UnsafeNativeMethods.PostMessage(WindowsFormsUtils.GetRootHWnd(this), WindowMessages.WM_SYSCOMMAND, NativeMethods.SC_KEYMENU, (int)Keys.Space);
                        return true;
                    }
                }
            }
            return base.ProcessCmdKey(ref m, keyData);

        }
        /// <summary>
        ///  Summary of WndProc.
        /// </summary>
        /// <param name=m></param>
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WindowMessages.WM_MOUSEACTIVATE && (ActiveDropDowns.Count == 0))
            {
                // call menu activate before we actually take focus.
                Point pt = PointToClient(WindowsFormsUtils.LastCursorPoint);
                ToolStripItem item = GetItemAt(pt);
                if (item != null && !(item is ToolStripControlHost))
                {
                    // verify the place where we've clicked is a place where we have to do "fake" focus
                    // e.g. an item that isnt a control.
                    KeyboardActive = true;
                }
            }

            base.WndProc(ref m);
        }

        [ComVisible(true)]
        internal class MenuStripAccessibleObject : ToolStripAccessibleObject
        {
            public MenuStripAccessibleObject(MenuStrip owner)
                : base(owner)
            {
            }

            public override AccessibleRole Role
            {
                get
                {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default)
                    {
                        return role;
                    }
                    return AccessibleRole.MenuBar;
                }
            }

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_MenuBarControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
