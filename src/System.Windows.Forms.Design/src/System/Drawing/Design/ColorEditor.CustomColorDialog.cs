// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Windows.Win32.UI.Controls.Dialogs;

namespace System.Drawing.Design;

public partial class ColorEditor
{
    private class CustomColorDialog : ColorDialog
    {
        private static readonly Assembly s_assembly = typeof(ColorEditor).Module.Assembly;
        private static readonly string s_resourceName = $"{s_assembly.GetName().Name}.colordlg.data";
        private IntPtr _hInstance;

        public CustomColorDialog()
        {
            // colordlg.data was copied from VB6's dlg-4300.dlg
            using Stream stream = s_assembly.GetManifestResourceStream(s_resourceName)!;

            int size = (int)(stream.Length - stream.Position);
            byte[] buffer = new byte[size];
            stream.ReadExactly(buffer, 0, size);

            _hInstance = Marshal.AllocHGlobal(size);
            Marshal.Copy(buffer, 0, _hInstance, size);
        }

        protected override IntPtr Instance
        {
            get
            {
                Debug.Assert(_hInstance != IntPtr.Zero, "Dialog has been disposed");
                return _hInstance;
            }
        }

        protected override int Options => (int)(CHOOSECOLOR_FLAGS.CC_FULLOPEN | CHOOSECOLOR_FLAGS.CC_ENABLETEMPLATEHANDLE);

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (_hInstance != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_hInstance);
                    _hInstance = IntPtr.Zero;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected override unsafe IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            switch ((MessageId)msg)
            {
                case PInvokeCore.WM_INITDIALOG:
                    PInvoke.SendDlgItemMessage(
                        (HWND)hwnd,
                        (int)PInvoke.COLOR_HUE,
                        PInvokeCore.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                        0);
                    PInvoke.SendDlgItemMessage(
                        (HWND)hwnd,
                        (int)PInvoke.COLOR_SAT,
                        PInvokeCore.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                        0);
                    PInvoke.SendDlgItemMessage(
                        (HWND)hwnd,
                        (int)PInvoke.COLOR_LUM,
                        PInvokeCore.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                        0);
                    PInvoke.SendDlgItemMessage(
                        (HWND)hwnd,
                        (int)PInvoke.COLOR_RED,
                        PInvokeCore.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                        0);
                    PInvoke.SendDlgItemMessage(
                        (HWND)hwnd,
                        (int)PInvoke.COLOR_GREEN,
                        PInvokeCore.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                        0);
                    PInvoke.SendDlgItemMessage(
                        (HWND)hwnd,
                        (int)PInvoke.COLOR_BLUE,
                        PInvokeCore.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                        0);
                    HWND hwndCtl = PInvoke.GetDlgItem((HWND)hwnd, (int)PInvoke.COLOR_MIX);
                    PInvoke.EnableWindow(hwndCtl, false);
                    PInvoke.SetWindowPos(
                        hwndCtl,
                        HWND.HWND_TOP,
                        0, 0, 0, 0,
                        SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW);
                    hwndCtl = PInvoke.GetDlgItem((HWND)hwnd, (int)MESSAGEBOX_RESULT.IDOK);
                    PInvoke.EnableWindow(hwndCtl, false);
                    PInvoke.SetWindowPos(
                        hwndCtl,
                        HWND.HWND_TOP,
                        0, 0, 0, 0,
                        SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW);
                    Color = Color.Empty;
                    break;

                case PInvokeCore.WM_COMMAND:
                    if (PARAM.LOWORD(wParam) == (int)PInvoke.COLOR_ADD)
                    {
                        BOOL success = false;
                        byte red = (byte)PInvoke.GetDlgItemInt((HWND)hwnd, (int)PInvoke.COLOR_RED, &success, false);
                        Debug.Assert(!success, "Couldn't find dialog member COLOR_RED");

                        byte green = (byte)PInvoke.GetDlgItemInt((HWND)hwnd, (int)PInvoke.COLOR_GREEN, &success, false);
                        Debug.Assert(!success, "Couldn't find dialog member COLOR_GREEN");

                        byte blue = (byte)PInvoke.GetDlgItemInt((HWND)hwnd, (int)PInvoke.COLOR_BLUE, &success, false);
                        Debug.Assert(!success, "Couldn't find dialog member COLOR_BLUE");

                        Color = Color.FromArgb(red, green, blue);
                        PInvokeCore.PostMessage(
                            (HWND)hwnd,
                            PInvokeCore.WM_COMMAND,
                            (WPARAM)PARAM.FromLowHigh((int)MESSAGEBOX_RESULT.IDOK, 0),
                            (LPARAM)PInvoke.GetDlgItem((HWND)hwnd, (int)MESSAGEBOX_RESULT.IDOK));
                        break;
                    }

                    break;
            }

            return base.HookProc(hwnd, msg, wParam, lParam);
        }
    }
}
