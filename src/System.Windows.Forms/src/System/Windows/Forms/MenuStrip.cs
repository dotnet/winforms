// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;

    /// <include file='doc\MenuStrip.uex' path='docs/doc[@for="MenuStrip"]/*' />
    [ComVisible(true),
     ClassInterface(ClassInterfaceType.AutoDispatch),
     SRDescription(nameof(SR.DescriptionMenuStrip))
    ]
    public class MenuStrip : ToolStrip {


        private ToolStripMenuItem mdiWindowListItem = null;

        private static readonly object EventMenuActivate = new object();
        private static readonly object EventMenuDeactivate = new object();



        /// <include file='doc\MenuStrip.uex' path='docs/doc[@for="MenuStrip.MenuStrip"]/*' />
        public MenuStrip() {
            this.CanOverflow = false;
            this.GripStyle = ToolStripGripStyle.Hidden;
            this.Stretch = true;

        }

        internal override bool KeyboardActive {
            get { return base.KeyboardActive; }
            set {
                if (base.KeyboardActive != value) {
                    base.KeyboardActive = value;
                    if (value) {
                        OnMenuActivate(EventArgs.Empty);
                    }
                    else {
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
        public new bool CanOverflow {
            get {
                return base.CanOverflow;
            }
            set {
                base.CanOverflow = value;
            }
        }
        protected override bool DefaultShowItemToolTips {
            get {
                return false;
            }
        }
        protected override Padding DefaultGripMargin {
            get {
                // MenuStrip control is scaled by Control::ScaleControl()
                // Ensure grip aligns properly when set visible.
                return new Padding(2, 2, 0, 2);
            }
        }

        /// <include file='doc\MenuStrip.uex' path='docs/doc[@for="MenuStrip.DefaultSize"]/*' />
        protected override Size DefaultSize {
            get {
                return new Size(200, 24);
            }
        }

        protected override Padding DefaultPadding {
            get {
                // MenuStrip control is scaled by Control::ScaleControl()
                // Scoot the grip over when present
                if (GripStyle == ToolStripGripStyle.Visible) {
                    return new Padding(3, 2, 0, 2);
                }
                return new Padding(6, 2, 0, 2);
            }
        }

        [DefaultValue(ToolStripGripStyle.Hidden)]
        public new ToolStripGripStyle GripStyle {
            get {
                return base.GripStyle;
            }
            set {
                base.GripStyle = value;
            }
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.MenuStripMenuActivateDescr))]
        public event EventHandler MenuActivate {
            add {
                Events.AddHandler(EventMenuActivate, value);
            }
            remove {
                Events.RemoveHandler(EventMenuActivate, value);
            }
        }

        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.MenuStripMenuDeactivateDescr))]
        public event EventHandler MenuDeactivate {
            add {
                Events.AddHandler(EventMenuDeactivate, value);
            }
            remove {
                Events.RemoveHandler(EventMenuDeactivate, value);
            }
        }

        [DefaultValue(false)]
        [SRDescription(nameof(SR.ToolStripShowItemToolTipsDescr))]
        [SRCategory(nameof(SR.CatBehavior))]
        public new bool ShowItemToolTips {
            get {
                return base.ShowItemToolTips;
            }
            set {
                base.ShowItemToolTips = value;
            }
        }

        [DefaultValue(true)]
        [SRCategory(nameof(SR.CatLayout))]
        [SRDescription(nameof(SR.ToolStripStretchDescr))]
        public new bool Stretch {
            get {
                return base.Stretch;
            }
            set {
                base.Stretch = value;
            }
        }

        [DefaultValue(null)]
        [MergableProperty(false)]
        [SRDescription(nameof(SR.MenuStripMdiWindowListItem))]
        [SRCategory(nameof(SR.CatBehavior))]
        [TypeConverterAttribute(typeof(MdiWindowListItemConverter))]
        public ToolStripMenuItem MdiWindowListItem {
            get {
                return mdiWindowListItem;
            }
            set {
                mdiWindowListItem = value;
            }
        }

        /// <include file='doc\MenuStrip.uex' path='docs/doc[@for="MenuStrip.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new MenuStripAccessibleObject(this);
        }
        protected internal override ToolStripItem CreateDefaultItem(string text, Image image, EventHandler onClick) {
            if (text == "-") {
                return new ToolStripSeparator();
            }
            else {
                return new ToolStripMenuItem(text, image, onClick);
            }
        }

        internal override ToolStripItem GetNextItem(ToolStripItem start, ArrowDirection direction, bool rtlAware) {
            ToolStripItem nextItem = base.GetNextItem(start, direction, rtlAware);
            if (nextItem is MdiControlStrip.SystemMenuItem && AccessibilityImprovements.Level2) {
                nextItem = base.GetNextItem(nextItem, direction, rtlAware);
            }
            return nextItem;
        }

        protected virtual void OnMenuActivate(EventArgs e) {
            if (IsHandleCreated) {
                AccessibilityNotifyClients(AccessibleEvents.SystemMenuStart, -1);
            }
            EventHandler handler = (EventHandler)Events[EventMenuActivate];
            if (handler != null) handler(this, e);
        }

        protected virtual void OnMenuDeactivate(EventArgs e) {
            if (IsHandleCreated) {
                AccessibilityNotifyClients(AccessibleEvents.SystemMenuEnd, -1);
            }
            EventHandler handler = (EventHandler)Events[EventMenuDeactivate];
            if (handler != null) handler(this, e);
        }

        /// <devdoc>
        /// Called from ToolStripManager.ProcessMenuKey.  Fires MenuActivate event and sets focus.
        /// </devdoc>
        internal bool OnMenuKey() {
            if (!(Focused || ContainsFocus)) {
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] set focus to menustrip");
                ToolStripManager.ModalMenuFilter.SetActiveToolStrip(this, /*menuKeyPressed=*/true);

                if (DisplayedItems.Count > 0) {
                    if (DisplayedItems[0] is MdiControlStrip.SystemMenuItem) {
                        SelectNextToolStripItem(DisplayedItems[0], /*forward=*/true);
                    }
                    else {
                        // first alt should select "File".  Future keydowns of alt should restore focus.
                        SelectNextToolStripItem(null, /*forward=*/(RightToLeft == RightToLeft.No));
                    }
                }

                return true;
            }
            return false;
        }

        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessCmdKey(ref Message m, Keys keyData) {

            if (ToolStripManager.ModalMenuFilter.InMenuMode) {
                // ALT, then space should dismiss the menu and activate the system menu.
                if (keyData == Keys.Space) {
                    // if we're focused it's ok to activate system menu
                    // if we're not focused - we should not activate if we contain focus - this means a text box or something
                    // has focus.
                    if (Focused || !ContainsFocus) {
                        NotifySelectionChange(null);
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[MenuStrip.ProcessCmdKey] Rolling up the menu and invoking the system menu");
                        ToolStripManager.ModalMenuFilter.ExitMenuMode();
                        // send a WM_SYSCOMMAND SC_KEYMENU + Space to activate the system menu.
                        UnsafeNativeMethods.PostMessage(WindowsFormsUtils.GetRootHWnd(this), NativeMethods.WM_SYSCOMMAND, NativeMethods.SC_KEYMENU, (int)Keys.Space);
                        return true;
                    }
                }
            }
            return base.ProcessCmdKey(ref m, keyData);


        }
        /// <include file='doc\ToolStrip.uex' path='docs/doc[@for="ToolStrip.WndProc"]/*' />
        /// <devdoc>
        /// Summary of WndProc.
        /// </devdoc>
        /// <param name=m></param>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {

            if (m.Msg == NativeMethods.WM_MOUSEACTIVATE && (ActiveDropDowns.Count == 0)) {
                // call menu activate before we actually take focus.
                Point pt = PointToClient(WindowsFormsUtils.LastCursorPoint);
                ToolStripItem item = GetItemAt(pt);
                if (item != null && !(item is ToolStripControlHost)) {
                    // verify the place where we've clicked is a place where we have to do "fake" focus
                    // e.g. an item that isnt a control.
                    KeyboardActive = true;
                }
            }

            base.WndProc(ref m);
        }

        [System.Runtime.InteropServices.ComVisible(true)]
        internal class MenuStripAccessibleObject : ToolStripAccessibleObject {

            public MenuStripAccessibleObject(MenuStrip owner)
                : base(owner) {
            }

            public override AccessibleRole Role {
                get {
                    AccessibleRole role = Owner.AccessibleRole;
                    if (role != AccessibleRole.Default) {
                        return role;
                    }
                    return AccessibleRole.MenuBar;
                }
            }
        }

    }
}
