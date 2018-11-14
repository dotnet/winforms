// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Threading;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Drawing;   
    using System.Windows.Forms;    
    using System.Windows.Forms.Design;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;

    /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Specifies the base class used for displaying
    ///       dialog boxes on the screen.
    ///    </para>
    /// </devdoc>
    [
    ToolboxItemFilter("System.Windows.Forms"),
    System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1012:AbstractTypesShouldNotHaveConstructors") // Shipped in Everett
    ]
    public abstract class CommonDialog : Component {
        private static readonly object EventHelpRequest = new object();
        private const int CDM_SETDEFAULTFOCUS = NativeMethods.WM_USER + 0x51;
        private static int helpMsg;

        private IntPtr defOwnerWndProc;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2006:UseSafeHandleToEncapsulateNativeResources")]
        private IntPtr hookedWndProc;

        private IntPtr  defaultControlHwnd;

        object userData;

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.CommonDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.CommonDialog'/> class.
        ///    </para>
        /// </devdoc>
        public CommonDialog() {
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.HelpRequest"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Occurs when the user clicks the Help button on a common
        ///       dialog box.
        ///    </para>
        /// </devdoc>
        [SRDescription(nameof(SR.CommonDialogHelpRequested))]
        public event EventHandler HelpRequest {
            add {
                Events.AddHandler(EventHelpRequest, value);
            }
            remove {
                Events.RemoveHandler(EventHelpRequest, value);
            }
        }

        // Generate meaningful result from Windows.CommDlgExtendedError()

        /* Only used in PageSetupDialog.cs in a line that is commented out.
            Commenting out until we need it again.
        
        internal static string CommonDialogErrorToString(int error) {
            switch (error) {
                case NativeMethods.CDERR_DIALOGFAILURE: return "dialogfailure";
                case NativeMethods.CDERR_FINDRESFAILURE: return "findresfailure";
                case NativeMethods.CDERR_INITIALIZATION: return "initialization";
                case NativeMethods.CDERR_LOADRESFAILURE: return "loadresfailure";
                case NativeMethods.CDERR_LOADSTRFAILURE: return "loadstrfailure";
                case NativeMethods.CDERR_LOCKRESFAILURE: return "lockresfailure";
                case NativeMethods.CDERR_MEMALLOCFAILURE: return "memallocfailure";
                case NativeMethods.CDERR_MEMLOCKFAILURE: return "memlockfailure";
                case NativeMethods.CDERR_NOHINSTANCE: return "nohinstance";
                case NativeMethods.CDERR_NOHOOK: return "nohook";
                case NativeMethods.CDERR_NOTEMPLATE: return "notemplate";
                case NativeMethods.CDERR_REGISTERMSGFAIL: return "registermsgfail";
                case NativeMethods.CDERR_STRUCTSIZE: return "structsize";
                case NativeMethods.PDERR_CREATEICFAILURE: return "createicfailure";
                case NativeMethods.PDERR_DEFAULTDIFFERENT: return "defaultdifferent";
                case NativeMethods.PDERR_DNDMMISMATCH: return "dndmmismatch";
                case NativeMethods.PDERR_GETDEVMODEFAIL: return "getdevmodefail";
                case NativeMethods.PDERR_INITFAILURE: return "initfailure";
                case NativeMethods.PDERR_LOADDRVFAILURE: return "loaddrvfailure";
                case NativeMethods.PDERR_NODEFAULTPRN: return "nodefaultprn";
                case NativeMethods.PDERR_NODEVICES: return "nodevices";
                case NativeMethods.PDERR_PARSEFAILURE: return "parsefailure";
                case NativeMethods.PDERR_PRINTERNOTFOUND: return "printernotfound";
                case NativeMethods.PDERR_RETDEFFAILURE: return "retdeffailure";
                case NativeMethods.PDERR_SETUPFAILURE: return "setupfailure";
                case NativeMethods.CFERR_MAXLESSTHANMIN: return "maxlessthanmin";
                case NativeMethods.CFERR_NOFONTS: return "nofonts";
                case NativeMethods.FNERR_BUFFERTOOSMALL: return "buffertoosmall";
                case NativeMethods.FNERR_INVALIDFILENAME: return "invalidfilename";
                case NativeMethods.FNERR_SUBCLASSFAILURE: return "subclassfailure";
                case NativeMethods.FRERR_BUFFERLENGTHZERO : return "bufferlengthzero";
                default: return "unknown error";
            }
        }

        */

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.HookProc"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Defines the common dialog box hook
        ///       procedure that is overridden to add specific functionality to a common dialog
        ///       box.
        ///    </para>
        /// </devdoc>
        [SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
        SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
        protected virtual IntPtr HookProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
            if (msg == NativeMethods.WM_INITDIALOG) {
                MoveToScreenCenter(hWnd);

                // Under some circumstances, the dialog
                // does not initially focus on any control. We fix that by explicitly
                // setting focus ourselves.
                //
                this.defaultControlHwnd = wparam;
                UnsafeNativeMethods.SetFocus(new HandleRef(null, wparam));
            }
            else if (msg == NativeMethods.WM_SETFOCUS) {
                UnsafeNativeMethods.PostMessage(new HandleRef(null, hWnd), CDM_SETDEFAULTFOCUS, 0, 0);
            }
            else if (msg == CDM_SETDEFAULTFOCUS) {
                // If the dialog box gets focus, bounce it to the default control.
                // so we post a message back to ourselves to wait for the focus change then push it to the default
                // control.
                //
                UnsafeNativeMethods.SetFocus(new HandleRef(this, defaultControlHwnd));
            }
            return IntPtr.Zero;
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.MoveToScreenCenter"]/*' />
        /// <devdoc>
        ///     Centers the given window on the screen. This method is used by the default
        ///     common dialog hook procedure to center the dialog on the screen before it
        ///     is shown.
        /// </devdoc>
        internal static void MoveToScreenCenter(IntPtr hWnd) {
            NativeMethods.RECT r = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(null, hWnd), ref r);
            Rectangle screen = Screen.GetWorkingArea(Control.MousePosition);
            int x = screen.X + (screen.Width - r.right + r.left) / 2;
            int y = screen.Y + (screen.Height - r.bottom + r.top) / 3;
            SafeNativeMethods.SetWindowPos(new HandleRef(null, hWnd), NativeMethods.NullHandleRef, x, y, 0, 0, NativeMethods.SWP_NOSIZE |
                                 NativeMethods.SWP_NOZORDER | NativeMethods.SWP_NOACTIVATE);
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.OnHelpRequest"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Raises the <see cref='System.Windows.Forms.CommonDialog.HelpRequest'/>
        ///       event.
        ///    </para>
        /// </devdoc>
        protected virtual void OnHelpRequest(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EventHelpRequest];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.OwnerWndProc"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Defines the owner window procedure that is
        ///       overridden to add specific functionality to a common dialog box.
        ///    </para>
        /// </devdoc>
        [
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.LinkDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode),
            System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
        ]
        protected virtual IntPtr OwnerWndProc(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam) {
            if (msg == helpMsg) {
                if (NativeWindow.WndProcShouldBeDebuggable) {
                    OnHelpRequest(EventArgs.Empty);
                }
                else {
                    try {
                        OnHelpRequest(EventArgs.Empty);
                    }
                    catch (Exception e) {
                        Application.OnThreadException(e);
                    }
                }
                return IntPtr.Zero;
            }
            return UnsafeNativeMethods.CallWindowProc(defOwnerWndProc, hWnd, msg, wparam, lparam);         
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.Reset"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class,
        ///       resets the properties of a common dialog to their default
        ///       values.
        ///    </para>
        /// </devdoc>
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        public abstract void Reset();

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.RunDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When overridden in a derived class,
        ///       specifies a common dialog box.
        ///    </para>
        /// </devdoc>
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.InheritanceDemand, Flags=System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected abstract bool RunDialog(IntPtr hwndOwner);

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.ShowDialog"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Runs a common dialog box.
        ///    </para>
        /// </devdoc>
        public DialogResult ShowDialog() {
            return ShowDialog(null);
        }

        /// <include file='doc\CommonDialog.uex' path='docs/doc[@for="CommonDialog.ShowDialog1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Runs a common dialog box, parented to the given IWin32Window.
        ///    </para>
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2004:RemoveCallsToGCKeepAlive")]
        public DialogResult ShowDialog( IWin32Window owner ) {

            IntSecurity.SafeSubWindows.Demand();

            if (!SystemInformation.UserInteractive) {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }

            NativeWindow native = null;//This will be used if there is no owner or active window (declared here so it can be kept alive)

            IntPtr hwndOwner = IntPtr.Zero;
            DialogResult result = DialogResult.Cancel;
            try {
                if (owner != null) {
                    hwndOwner = Control.GetSafeHandle(owner);
                }
    
                if (hwndOwner == IntPtr.Zero) {
                    hwndOwner = UnsafeNativeMethods.GetActiveWindow();
                }
    
                if (hwndOwner == IntPtr.Zero) {
                    //We will have to create our own Window
                    native = new NativeWindow();
                    native.CreateHandle(new CreateParams());
                    hwndOwner = native.Handle;
                }
                
                if (helpMsg == 0) {
                    helpMsg = SafeNativeMethods.RegisterWindowMessage("commdlg_help");
                }
    
                NativeMethods.WndProc ownerProc = new NativeMethods.WndProc(this.OwnerWndProc);
                hookedWndProc = System.Runtime.InteropServices.Marshal.GetFunctionPointerForDelegate(ownerProc);
                System.Diagnostics.Debug.Assert(IntPtr.Zero == defOwnerWndProc, "The previous subclass wasn't properly cleaned up");

                IntPtr userCookie = IntPtr.Zero;
                try {
                    //UnsafeNativeMethods.[Get|Set]WindowLong is smart enough to call SetWindowLongPtr on 64-bit OS
                    defOwnerWndProc = UnsafeNativeMethods.SetWindowLong(new HandleRef(this, hwndOwner), NativeMethods.GWL_WNDPROC, ownerProc);

                    if (Application.UseVisualStyles) {
                        userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                    }
                    
                    Application.BeginModalMessageLoop();
                    try {
                        result = RunDialog(hwndOwner) ? DialogResult.OK : DialogResult.Cancel;
                    }
                    finally {
                        Application.EndModalMessageLoop();
                    }
                }
                finally {
                    IntPtr currentSubClass = UnsafeNativeMethods.GetWindowLong(new HandleRef(this, hwndOwner), NativeMethods.GWL_WNDPROC);
                    if ( IntPtr.Zero != defOwnerWndProc || currentSubClass != hookedWndProc) {
                        UnsafeNativeMethods.SetWindowLong(new HandleRef(this, hwndOwner), NativeMethods.GWL_WNDPROC, new HandleRef(this, defOwnerWndProc));
                    }
                    UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);

                    defOwnerWndProc = IntPtr.Zero;
                    hookedWndProc = IntPtr.Zero;
                    //Ensure that the subclass delegate will not be GC collected until after it has been subclassed
                    GC.KeepAlive(ownerProc);
                }
            }
            finally {
                if (null != native) {
                    native.DestroyHandle();
                }
            }

            return result;
        }
    }
}
