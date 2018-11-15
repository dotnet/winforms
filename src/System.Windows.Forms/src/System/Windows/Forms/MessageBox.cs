// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {

    using Microsoft.Win32;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Collections;
    
    /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays a
    ///       message box that can contain text, buttons, and symbols that
    ///       inform and instruct the
    ///       user.
    ///    </para>
    /// </devdoc>
    public class MessageBox {
        private const int IDOK             = 1;
        private const int IDCANCEL         = 2;
        private const int IDABORT          = 3;
        private const int IDRETRY          = 4;
        private const int IDIGNORE         = 5;
        private const int IDYES            = 6;
        private const int IDNO             = 7;


        private const int HELP_BUTTON      = 0x00004000;

        [ ThreadStatic ]
        private static HelpInfo[] helpInfoTable;

        

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.MessageBox"]/*' />
        /// <devdoc>
        ///     This constructor is private so people aren't tempted to try and create
        ///     instances of these -- they should just use the static show
        ///     methods.
        /// </devdoc>
        private MessageBox() {
        }

        private static DialogResult Win32ToDialogResult(int value) {
            switch (value) {
                case IDOK:
                    return DialogResult.OK;
                case IDCANCEL:
                    return DialogResult.Cancel;
                case IDABORT:
                    return DialogResult.Abort;
                case IDRETRY:
                    return DialogResult.Retry;
                case IDIGNORE:
                    return DialogResult.Ignore;
                case IDYES:
                    return DialogResult.Yes;
                case IDNO:
                    return DialogResult.No;
                default:
                    return DialogResult.No;
            }
        }

        
        internal static HelpInfo HelpInfo {
            get {
                // unfortunately, there's no easy way to obtain handle of a message box.
                // we'll have to rely on the fact that modal message loops have to pop off in an orderly way.

                if (helpInfoTable != null && helpInfoTable.Length > 0) {
                    // the top of the stack is actually at the end of the array.
                    return helpInfoTable[helpInfoTable.Length - 1];
                }
                
                return null;
            }
        }


        private static void PopHelpInfo() {

             // we roll our own stack here because we want a pretty lightweight implementation.
             // usually there's only going to be one message box shown at a time.  But if
             // someone shows two message boxes (say by launching them via a WM_TIMER message)
             // we've got to gracefully handle the current help info.
             if (helpInfoTable == null) {
                Debug.Fail("Why are we being called when there's nothing to pop?");
                
             } 
             else {
                if (helpInfoTable.Length == 1) {
                    helpInfoTable = null;
                }
                else {
                   int newCount = helpInfoTable.Length -1;
                   HelpInfo[] newTable = new HelpInfo[newCount];
                   Array.Copy(helpInfoTable, newTable, newCount);
                   helpInfoTable = newTable;
                
                }
             }
                
        }
        private static void PushHelpInfo(HelpInfo hpi) {
            
            // we roll our own stack here because we want a pretty lightweight implementation.
            // usually there's only going to be one message box shown at a time.  But if
            // someone shows two message boxes (say by launching them via a WM_TIMER message)
            // we've got to gracefully handle the current help info.

            int lastCount = 0;
            HelpInfo[] newTable; 
   
            if (helpInfoTable == null) {
               newTable = new HelpInfo[lastCount+1];
            }
            else {       
                // if we already have a table - allocate a new slot
                lastCount = helpInfoTable.Length;
                newTable = new HelpInfo[lastCount+1];
                Array.Copy(helpInfoTable, newTable, lastCount);
            }
            newTable[lastCount] = hpi;
            helpInfoTable = newTable;
        
        }

      
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //START WHIDBEY ADDS                                                                                           //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        
        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show12"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style with Help Button.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options,bool displayHelpButton) {
          
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton);
        }


        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show13"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style and Help file Path .
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath) {

            HelpInfo hpi = new HelpInfo(helpFilePath);
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }


        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show14"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style and Help file Path for a IWin32Window.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath) {

            HelpInfo hpi = new HelpInfo(helpFilePath);
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show15"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style, Help file Path and keyword.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword) {

            HelpInfo hpi = new HelpInfo(helpFilePath, keyword);
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }


        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show16"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style, Help file Path and keyword for a IWin32Window.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, string keyword) {

            HelpInfo hpi = new HelpInfo(helpFilePath, keyword);
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }


        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show17"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style, Help file Path and HelpNavigator.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options,string helpFilePath, HelpNavigator navigator) {

            HelpInfo hpi = new HelpInfo(helpFilePath, navigator);
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show18"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style, Help file Path and HelpNavigator for IWin32Window.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator) {

            HelpInfo hpi = new HelpInfo(helpFilePath, navigator);
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show19"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options,string helpFilePath, HelpNavigator navigator, object param) {

            HelpInfo hpi = new HelpInfo(helpFilePath, navigator, param);

            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }


        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show20"]/*' />
        /// <devdoc>
        ///    <para>
        ///      Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object for a IWin32Window.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options, string helpFilePath, HelpNavigator navigator, object param) {

            HelpInfo hpi = new HelpInfo(helpFilePath, navigator, param);
      
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //END ADD                                                                                                      //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options) {
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton) {
            return ShowCore(null, text, caption, buttons, icon, defaultButton, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show2"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
            return ShowCore(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show3"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons) {
            return ShowCore(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show4"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text and caption.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text, string caption) {
            return ShowCore(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show5"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(string text) {
            return ShowCore(null, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show6"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton, MessageBoxOptions options) {
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show7"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, 
                                        MessageBoxDefaultButton defaultButton) {
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show8"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon) {
            return ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show9"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text, caption, and style.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons) {
            return ShowCore(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show10"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text and caption.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text, string caption) {
            return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <include file='doc\MessageBox.uex' path='docs/doc[@for="MessageBox.Show11"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Displays a message box with specified text.
        ///    </para>
        /// </devdoc>
        public static DialogResult Show(IWin32Window owner, string text) {
            return ShowCore(owner, text, String.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        private static DialogResult ShowCore(IWin32Window owner, string text, string caption,   
                                     MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton,
                                     MessageBoxOptions options, HelpInfo hpi) {
            DialogResult result = DialogResult.None;
            try {
                PushHelpInfo(hpi);
                result = ShowCore(owner, text, caption, buttons, icon, defaultButton, options, true);
            }
            finally {
                PopHelpInfo();
            }
            return result;
            
        }

        private static DialogResult ShowCore(IWin32Window owner, string text, string caption,   
                                             MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton,
                                             MessageBoxOptions options, bool showHelp) {
       
            if (!ClientUtils.IsEnumValid(buttons, (int)buttons, (int)MessageBoxButtons.OK, (int)MessageBoxButtons.RetryCancel)){
               throw new InvalidEnumArgumentException("buttons", (int)buttons, typeof(MessageBoxButtons));
            }

            // valid values are 0x0 0x10 0x20 0x30 0x40, chop off the last 4 bits and check that it's between 0 and 4.
            if (!WindowsFormsUtils.EnumValidator.IsEnumWithinShiftedRange(icon, /*numBitsToShift*/4, /*min*/0x0,/*max*/0x4)) {
                throw new InvalidEnumArgumentException("icon", (int)icon, typeof(MessageBoxIcon));
            }
            // valid values are 0x0 0x100, 0x200, chop off the last 8 bits and check that it's between 0 and 2.
            if (!WindowsFormsUtils.EnumValidator.IsEnumWithinShiftedRange(defaultButton, /*numBitsToShift*/8, /*min*/0x0,/*max*/0x2)) {
                throw new InvalidEnumArgumentException("defaultButton", (int)defaultButton, typeof(DialogResult));
            }
            
            // options intentionally not verified because we don't expose all the options Win32 supports.

            if (!SystemInformation.UserInteractive && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0) {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }
            if (owner != null && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0) {
                throw new ArgumentException(SR.CantShowMBServiceWithOwner, "options");
            }
            if (showHelp && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0) {
                throw new ArgumentException(SR.CantShowMBServiceWithHelp, "options");
            }

            // demand if not safe known options.
            if ((options & ~(MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading)) != 0) {                
                IntSecurity.UnmanagedCode.Demand();
            }

            IntSecurity.SafeSubWindows.Demand();

            int style = (showHelp) ? HELP_BUTTON : 0; 
            style |= (int) buttons | (int) icon | (int) defaultButton | (int) options;

            IntPtr handle = IntPtr.Zero;
            if (showHelp || ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0)) {
                if (owner == null) {
                    handle = UnsafeNativeMethods.GetActiveWindow();
                }
                else {
                    handle = Control.GetSafeHandle(owner);
                }
            }

            IntPtr userCookie = IntPtr.Zero;

            if (Application.UseVisualStyles) {
                // CLR4.0 or later, shell32.dll needs to be loaded explicitly.
                if (UnsafeNativeMethods.GetModuleHandle(ExternDll.Shell32) == IntPtr.Zero) {
                    if (UnsafeNativeMethods.LoadLibraryFromSystemPathIfAvailable(ExternDll.Shell32) == IntPtr.Zero) {
                        int lastWin32Error = Marshal.GetLastWin32Error();
                        throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, ExternDll.Shell32));
                    }
                }

                // Activate theming scope to get theming for controls at design time and when hosted in browser.
                // NOTE: If a theming context is already active, this call is very fast, so shouldn't be a perf issue.
                userCookie = UnsafeNativeMethods.ThemingScope.Activate();
            }

            Application.BeginModalMessageLoop();
            DialogResult result;
            try {
                result = Win32ToDialogResult(SafeNativeMethods.MessageBox(new HandleRef(owner, handle), text, caption, style));
            }
            finally {
                Application.EndModalMessageLoop();
                UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
            }

            // Right after the dialog box is closed, Windows sends WM_SETFOCUS back to the previously active control 
            // but since we have disabled this thread main window the message is lost. So we have to send it again after
            // we enable the main window.
            //
            UnsafeNativeMethods.SendMessage(new HandleRef(owner, handle), NativeMethods.WM_SETFOCUS, 0, 0);
            return result;
        }

    }
}

