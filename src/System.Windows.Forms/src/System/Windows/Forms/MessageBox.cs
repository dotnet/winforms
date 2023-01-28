// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays a
    ///  message box that can contain text, buttons, and symbols that
    ///  inform and instruct the
    ///  user.
    /// </summary>
    public class MessageBox
    {
        [ThreadStatic]
        private static HelpInfo[]? helpInfoTable;

        /// <summary>
        ///  This constructor is private so people aren't tempted to try and create
        ///  instances of these -- they should just use the static show
        ///  methods.
        /// </summary>
        private MessageBox()
        {
        }

        internal static HelpInfo? HelpInfo
        {
            get
            {
                // unfortunately, there's no easy way to obtain handle of a message box.
                // we'll have to rely on the fact that modal message loops have to pop off in an orderly way.

                if (helpInfoTable is not null && helpInfoTable.Length > 0)
                {
                    // the top of the stack is actually at the end of the array.
                    return helpInfoTable[helpInfoTable.Length - 1];
                }

                return null;
            }
        }

        private static MB GetMessageBoxStyle(
            IWin32Window? owner,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            bool showHelp)
        {
            SourceGenerated.EnumValidator.Validate(buttons, nameof(buttons));
            SourceGenerated.EnumValidator.Validate(icon, nameof(icon));
            SourceGenerated.EnumValidator.Validate(defaultButton, nameof(defaultButton));

            // options intentionally not verified because we don't expose all the options Win32 supports.

            if (!SystemInformation.UserInteractive && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0)
            {
                throw new InvalidOperationException(SR.CantShowModalOnNonInteractive);
            }

            if (owner is not null && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0)
            {
                throw new ArgumentException(SR.CantShowMBServiceWithOwner, nameof(options));
            }

            if (showHelp && (options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) != 0)
            {
                throw new ArgumentException(SR.CantShowMBServiceWithHelp, nameof(options));
            }

            MB style = (showHelp) ? MB.HELP : 0;
            style |= (MB)buttons | (MB)icon | (MB)defaultButton | (MB)options;
            return style;
        }

        private static void PopHelpInfo()
        {
            // we roll our own stack here because we want a pretty lightweight implementation.
            // usually there's only going to be one message box shown at a time.  But if
            // someone shows two message boxes (say by launching them via a WM_TIMER message)
            // we've got to gracefully handle the current help info.
            if (helpInfoTable is null)
            {
                Debug.Fail("Why are we being called when there's nothing to pop?");
            }
            else
            {
                if (helpInfoTable.Length == 1)
                {
                    helpInfoTable = null;
                }
                else
                {
                    int newCount = helpInfoTable.Length - 1;
                    HelpInfo[] newTable = new HelpInfo[newCount];
                    Array.Copy(helpInfoTable, newTable, newCount);
                    helpInfoTable = newTable;
                }
            }
        }

        private static void PushHelpInfo(HelpInfo hpi)
        {
            // we roll our own stack here because we want a pretty lightweight implementation.
            // usually there's only going to be one message box shown at a time.  But if
            // someone shows two message boxes (say by launching them via a WM_TIMER message)
            // we've got to gracefully handle the current help info.

            int lastCount = 0;
            HelpInfo[] newTable;

            if (helpInfoTable is null)
            {
                newTable = new HelpInfo[lastCount + 1];
            }
            else
            {
                // if we already have a table - allocate a new slot
                lastCount = helpInfoTable.Length;
                newTable = new HelpInfo[lastCount + 1];
                Array.Copy(helpInfoTable, newTable, lastCount);
            }

            newTable[lastCount] = hpi;
            helpInfoTable = newTable;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //START WHIDBEY ADDS                                                                                           //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Displays a message box with specified text, caption, and style with Help Button.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            bool displayHelpButton)
        {
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, displayHelpButton);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style and Help file Path .
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath);
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style and Help file Path for a IWin32Window.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath);
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style, Help file Path and keyword.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath,
            string keyword)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath, keyword);
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style, Help file Path and keyword for a IWin32Window.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath,
            string keyword)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath, keyword);
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style, Help file Path and HelpNavigator.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath,
            HelpNavigator navigator)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath, navigator);
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style, Help file Path and HelpNavigator for IWin32Window.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath,
            HelpNavigator navigator)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath, navigator);
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath,
            HelpNavigator navigator,
            object? param)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath, navigator, param);

            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, style, Help file Path ,HelpNavigator and object for a IWin32Window.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            string helpFilePath,
            HelpNavigator navigator,
            object? param)
        {
            HelpInfo hpi = new HelpInfo(helpFilePath, navigator, param);

            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, hpi);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //END ADD                                                                                                      //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options)
        {
            return ShowCore(null, text, caption, buttons, icon, defaultButton, options, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            return ShowCore(null, text, caption, buttons, icon, defaultButton, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon)
        {
            return ShowCore(null, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(string? text, string? caption, MessageBoxButtons buttons)
        {
            return ShowCore(null, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text and caption.
        /// </summary>
        public static DialogResult Show(string? text, string? caption)
        {
            return ShowCore(null, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text.
        /// </summary>
        public static DialogResult Show(string? text)
        {
            return ShowCore(null, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options)
        {
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, options, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton)
        {
            return ShowCore(owner, text, caption, buttons, icon, defaultButton, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon)
        {
            return ShowCore(owner, text, caption, buttons, icon, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text, caption, and style.
        /// </summary>
        public static DialogResult Show(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons)
        {
            return ShowCore(owner, text, caption, buttons, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text and caption.
        /// </summary>
        public static DialogResult Show(IWin32Window? owner, string? text, string? caption)
        {
            return ShowCore(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        /// <summary>
        ///  Displays a message box with specified text.
        /// </summary>
        public static DialogResult Show(IWin32Window? owner, string? text)
        {
            return ShowCore(owner, text, string.Empty, MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1, 0, false);
        }

        private static DialogResult ShowCore(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            HelpInfo hpi)
        {
            DialogResult result = DialogResult.None;
            try
            {
                PushHelpInfo(hpi);
                result = ShowCore(owner, text, caption, buttons, icon, defaultButton, options, true);
            }
            finally
            {
                PopHelpInfo();
            }

            return result;
        }

        private static DialogResult ShowCore(
            IWin32Window? owner,
            string? text,
            string? caption,
            MessageBoxButtons buttons,
            MessageBoxIcon icon,
            MessageBoxDefaultButton defaultButton,
            MessageBoxOptions options,
            bool showHelp)
        {
            MB style = GetMessageBoxStyle(owner, buttons, icon, defaultButton, options, showHelp);

            HandleRef<HWND> handle = default;
            if (showHelp || ((options & (MessageBoxOptions.ServiceNotification | MessageBoxOptions.DefaultDesktopOnly)) == 0))
            {
                if (owner is null)
                {
                    handle = Control.GetHandleRef(PInvoke.GetActiveWindow());
                }
                else
                {
                    handle = Control.GetSafeHandle(owner);
                }
            }

            IntPtr userCookie = IntPtr.Zero;

            if (Application.UseVisualStyles)
            {
                // CLR4.0 or later, shell32.dll needs to be loaded explicitly.
                if (PInvoke.GetModuleHandle(Libraries.Shell32) == 0)
                {
                    if (PInvoke.LoadLibraryFromSystemPathIfAvailable(Libraries.Shell32) == IntPtr.Zero)
                    {
                        int lastWin32Error = Marshal.GetLastWin32Error();
                        throw new Win32Exception(lastWin32Error, string.Format(SR.LoadDLLError, Libraries.Shell32));
                    }
                }

                // Activate theming scope to get theming for controls at design time and when hosted in browser.
                // NOTE: If a theming context is already active, this call is very fast, so shouldn't be a perf issue.
                userCookie = ThemingScope.Activate(Application.UseVisualStyles);
            }

            Application.BeginModalMessageLoop();
            try
            {
                return (DialogResult)MessageBoxW(handle.Handle, text, caption, style);
            }
            finally
            {
                Application.EndModalMessageLoop();
                ThemingScope.Deactivate(userCookie);

                // Right after the dialog box is closed, Windows sends WM_SETFOCUS back to the previously active control
                // but since we have disabled this thread main window the message is lost. So we have to send it again after
                // we enable the main window.
                PInvoke.SendMessage(handle.Handle, User32.WM.SETFOCUS);
                GC.KeepAlive(handle.Wrapper);
            }
        }
    }
}
