// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu"]/*' />
    /// <devdoc>
    ///     This class is used to put context menus on your form and show them for
    ///     controls at runtime.  It basically acts like a regular Menu control,
    ///     but can be set for the ContextMenu property that most controls have.
    /// </devdoc>
    [
    DefaultEvent(nameof(Popup)),
    ]
    public class ContextMenu : Menu {

        private EventHandler onPopup;
        private EventHandler onCollapse;
        internal Control sourceControl;
        
        private RightToLeft rightToLeft = System.Windows.Forms.RightToLeft.Inherit;
    
        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.ContextMenu"]/*' />
        /// <devdoc>
        ///     Creates a new ContextMenu object with no items in it by default.
        /// </devdoc>
        public ContextMenu()
            : base(null) {
        }

        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.ContextMenu1"]/*' />
        /// <devdoc>
        ///     Creates a ContextMenu object with the given MenuItems.
        /// </devdoc>
        public ContextMenu(MenuItem[] menuItems)
            : base(menuItems) {
        }

        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.SourceControl"]/*' />
        /// <devdoc>
        ///     The last control that was acted upon that resulted in this context
        ///     menu being displayed.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ContextMenuSourceControlDescr))
        ]
        public Control SourceControl {
            [UIPermission(SecurityAction.Demand, Window=UIPermissionWindow.AllWindows)]
            get {
                return sourceControl;
            }
        }

        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.Popup"]/*' />
        [SRDescription(nameof(SR.MenuItemOnInitDescr))]
        public event EventHandler Popup {
            add {
                onPopup += value;
            }
            remove {
                onPopup -= value;
            }
        }
        
        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.Collapse"]/*' />
        /// <devdoc>
        ///    Fires when the context menu collapses.
        /// </devdoc>
        [SRDescription(nameof(SR.ContextMenuCollapseDescr))]
        public event EventHandler Collapse {
            add {
                onCollapse += value;
            }
            remove {
                onCollapse -= value;
            }
        }
        
        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.RightToLeft"]/*' />
        /// <devdoc>
        ///     This is used for international applications where the language
        ///     is written from RightToLeft. When this property is true,
        ///     text alignment and reading order will be from right to left.
        /// </devdoc>
        // Add a DefaultValue attribute so that the Reset context menu becomes
        // available in the Property Grid but the default value remains No.
        [
        Localizable(true),
        DefaultValue(RightToLeft.No),
        SRDescription(nameof(SR.MenuRightToLeftDescr))
        ]
        public virtual RightToLeft RightToLeft {
            get {
                if (System.Windows.Forms.RightToLeft.Inherit == rightToLeft) {
                    if (sourceControl != null) {
                        return ((Control)sourceControl).RightToLeft;
                    }
                    else {
                        return RightToLeft.No;
                    }
                }
                else {
                    return rightToLeft;
                }
            }
            set {
            
                //valid values are 0x0 to 0x2.
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)RightToLeft.No, (int)RightToLeft.Inherit)){
                    throw new InvalidEnumArgumentException(nameof(RightToLeft), (int)value, typeof(RightToLeft));
                }
                if (RightToLeft != value) {
                    rightToLeft = value;
                    UpdateRtl((value == System.Windows.Forms.RightToLeft.Yes));
                }

            }
        } 

        internal override bool RenderIsRightToLeft {
            get {
                return (rightToLeft == System.Windows.Forms.RightToLeft.Yes);
            }
        }
        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.OnPopup"]/*' />
        /// <devdoc>
        ///     Fires the popup event
        /// </devdoc>
        protected internal virtual void OnPopup(EventArgs e) {
            if (onPopup != null) {
                onPopup(this, e);
            }
        }

        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.OnCollapse"]/*' />
        /// <devdoc>
        ///     Fires the collapse event
        /// </devdoc>
        protected internal virtual void OnCollapse(EventArgs e) {
            if (onCollapse != null) {
                onCollapse(this, e);
            }
        }

        /// <include file='doc\Menu.uex' path='docs/doc[@for="ContextMenu.ProcessCmdKey"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        [
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode),
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
        ]
        protected internal virtual bool ProcessCmdKey(ref Message msg, Keys keyData, Control control) {
            sourceControl = control;
            return ProcessCmdKey(ref msg, keyData);
        }

        private void ResetRightToLeft() {
        	RightToLeft = RightToLeft.No;	
        }
        
        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.ShouldSerializeRightToLeft"]/*' />
        /// <devdoc>
        ///     Returns true if the RightToLeft should be persisted in code gen.
        /// </devdoc>
        internal virtual bool ShouldSerializeRightToLeft() {
            if (System.Windows.Forms.RightToLeft.Inherit == rightToLeft) {
                return false;
            }
            return true;
        }

        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.Show"]/*' />
        /// <devdoc>
        ///     Displays the context menu at the specified position.  This method
        ///     doesn't return until the menu is dismissed.
        /// </devdoc>
        public void Show(Control control, Point pos) {
            Show(control, pos, NativeMethods.TPM_VERTICAL | NativeMethods.TPM_RIGHTBUTTON);
        }

        /// <include file='doc\ContextMenu.uex' path='docs/doc[@for="ContextMenu.Show2"]/*' />
        /// <devdoc>
        ///     Displays the context menu at the specified position.  This method
        ///     doesn't return until the menu is dismissed.
        /// </devdoc>
        public void Show(Control control, Point pos, LeftRightAlignment alignment)  {

            // This code below looks wrong but it's correct. 
            // WinForms Left alignment means we want the menu to show up left of the point it is invoked from.
            // We specify TPM_RIGHTALIGN which tells win32 to align the right side of this 
            // menu with the point (which aligns it Left visually)
            if (alignment == LeftRightAlignment.Left) {
                Show(control, pos, NativeMethods.TPM_VERTICAL | NativeMethods.TPM_RIGHTBUTTON | NativeMethods.TPM_RIGHTALIGN);
            }
            else {
                Show(control, pos, NativeMethods.TPM_VERTICAL | NativeMethods.TPM_RIGHTBUTTON | NativeMethods.TPM_LEFTALIGN);
            }
        }

        private void Show(Control control, Point pos, int flags) {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            if (!control.IsHandleCreated || !control.Visible)
                throw new ArgumentException(SR.ContextMenuInvalidParent, "control");

            sourceControl = control;

            OnPopup(EventArgs.Empty);
            pos = control.PointToScreen(pos);
            SafeNativeMethods.TrackPopupMenuEx(new HandleRef(this, Handle),
                flags,
                pos.X,
                pos.Y,
                new HandleRef(control, control.Handle),
                null);
        }
            
    }
}
