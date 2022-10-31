// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Interop;

namespace System.Drawing.Design
{
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
                using Stream stream = s_assembly.GetManifestResourceStream(s_resourceName);

                int size = (int)(stream.Length - stream.Position);
                byte[] buffer = new byte[size];
                stream.Read(buffer, 0, size);

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

            protected override int Options => (int)(Comdlg32.CC.FULLOPEN | Comdlg32.CC.ENABLETEMPLATEHANDLE);

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
                switch ((User32.WM)msg)
                {
                    case User32.WM.INITDIALOG:
                        PInvoke.SendDlgItemMessage(
                            (HWND)hwnd,
                            (int)PInvoke.COLOR_HUE,
                            PInvoke.EM_SETMARGINS,
                            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                            0);
                        PInvoke.SendDlgItemMessage(
                            (HWND)hwnd,
                            (int)PInvoke.COLOR_SAT,
                            PInvoke.EM_SETMARGINS,
                            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                            0);
                        PInvoke.SendDlgItemMessage(
                            (HWND)hwnd,
                            (int)PInvoke.COLOR_LUM,
                            PInvoke.EM_SETMARGINS,
                            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                            0);
                        PInvoke.SendDlgItemMessage(
                            (HWND)hwnd,
                            (int)PInvoke.COLOR_RED,
                            PInvoke.EM_SETMARGINS,
                            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                            0);
                        PInvoke.SendDlgItemMessage(
                            (HWND)hwnd,
                            (int)PInvoke.COLOR_GREEN,
                            PInvoke.EM_SETMARGINS,
                            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                            0);
                        PInvoke.SendDlgItemMessage(
                            (HWND)hwnd,
                            (int)PInvoke.COLOR_BLUE,
                            PInvoke.EM_SETMARGINS,
                            (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN),
                            0);
                        HWND hwndCtl = PInvoke.GetDlgItem((HWND)hwnd, (int)PInvoke.COLOR_MIX);
                        PInvoke.EnableWindow(hwndCtl, false);
                        PInvoke.SetWindowPos(
                            hwndCtl,
                            HWND.HWND_TOP,
                            0, 0, 0, 0,
                            SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW);
                        hwndCtl = PInvoke.GetDlgItem((HWND)hwnd, (int)User32.ID.OK);
                        PInvoke.EnableWindow(hwndCtl, false);
                        PInvoke.SetWindowPos(
                            hwndCtl,
                            HWND.HWND_TOP,
                            0, 0, 0, 0,
                            SET_WINDOW_POS_FLAGS.SWP_HIDEWINDOW);
                        Color = Color.Empty;
                        break;

                    case User32.WM.COMMAND:
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
                            User32.PostMessageW(
                                hwnd,
                                User32.WM.COMMAND,
                                PARAM.FromLowHigh((int)User32.ID.OK, 0),
                                PInvoke.GetDlgItem((HWND)hwnd, (int)User32.ID.OK));
                            break;
                        }

                        break;
                }

                return base.HookProc(hwnd, msg, wParam, lParam);
            }
        }
    }
}
