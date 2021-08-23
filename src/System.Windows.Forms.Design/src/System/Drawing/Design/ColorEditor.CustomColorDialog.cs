// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static Interop;
using static Interop.User32;

namespace System.Drawing.Design
{
    public partial class ColorEditor
    {
        private class CustomColorDialog : ColorDialog
        {
            private IntPtr hInstance;

            public CustomColorDialog()
            {
                // colordlg.data was copied from VB6's dlg-4300.dlg
                Stream stream = typeof(ColorEditor).Module.Assembly.GetManifestResourceStream(typeof(ColorEditor), "colordlg.data");

                int size = (int)(stream.Length - stream.Position);
                byte[] buffer = new byte[size];
                stream.Read(buffer, 0, size);

                hInstance = Marshal.AllocHGlobal(size);
                Marshal.Copy(buffer, 0, hInstance, size);
            }

            protected override IntPtr Instance
            {
                get
                {
                    Debug.Assert(hInstance != IntPtr.Zero, "Dialog has been disposed");
                    return hInstance;
                }
            }

            protected override int Options => (int)(Comdlg32.CC.FULLOPEN | Comdlg32.CC.ENABLETEMPLATEHANDLE);

            protected override void Dispose(bool disposing)
            {
                try
                {
                    if (hInstance != IntPtr.Zero)
                    {
                        Marshal.FreeHGlobal(hInstance);
                        hInstance = IntPtr.Zero;
                    }
                }
                finally
                {
                    base.Dispose(disposing);
                }
            }

            protected unsafe override IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                switch ((WM)msg)
                {
                    case WM.INITDIALOG:
                        SendDlgItemMessageW(hwnd, (DialogItemID)Comdlg32.COLOR.HUE, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                        SendDlgItemMessageW(hwnd, (DialogItemID)Comdlg32.COLOR.SAT, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                        SendDlgItemMessageW(hwnd, (DialogItemID)Comdlg32.COLOR.LUM, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                        SendDlgItemMessageW(hwnd, (DialogItemID)Comdlg32.COLOR.RED, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                        SendDlgItemMessageW(hwnd, (DialogItemID)Comdlg32.COLOR.GREEN, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                        SendDlgItemMessageW(hwnd, (DialogItemID)Comdlg32.COLOR.BLUE, (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                        IntPtr hwndCtl = GetDlgItem(hwnd, (DialogItemID)Comdlg32.COLOR.MIX);
                        EnableWindow(hwndCtl, BOOL.FALSE);
                        SetWindowPos(
                            hwndCtl,
                            HWND_TOP,
                            flags: SWP.HIDEWINDOW);
                        hwndCtl = GetDlgItem(hwnd, (DialogItemID)ID.OK);
                        EnableWindow(hwndCtl, BOOL.FALSE);
                        SetWindowPos(
                            hwndCtl,
                            HWND_TOP,
                            flags: SWP.HIDEWINDOW);
                        Color = Color.Empty;
                        break;

                    case WM.COMMAND:
                        if (PARAM.LOWORD(wParam) == (int)Comdlg32.COLOR.ADD)
                        {
                            BOOL err = BOOL.FALSE;
                            byte red = (byte)User32.GetDlgItemInt(hwnd, (int)Comdlg32.COLOR.RED, &err, BOOL.FALSE);
                            Debug.Assert(err.IsFalse(), "Couldn't find dialog member COLOR_RED");

                            byte green = (byte)User32.GetDlgItemInt(hwnd, (int)Comdlg32.COLOR.GREEN, &err, BOOL.FALSE);
                            Debug.Assert(err.IsFalse(), "Couldn't find dialog member COLOR_GREEN");

                            byte blue = (byte)User32.GetDlgItemInt(hwnd, (int)Comdlg32.COLOR.BLUE, &err, BOOL.FALSE);
                            Debug.Assert(err.IsFalse(), "Couldn't find dialog member COLOR_BLUE");

                            Color = Color.FromArgb(red, green, blue);
                            PostMessageW(hwnd, WM.COMMAND, PARAM.FromLowHigh((int)ID.OK, 0), GetDlgItem(hwnd, (DialogItemID)ID.OK));
                            break;
                        }

                        break;
                }

                return base.HookProc(hwnd, msg, wParam, lParam);
            }
        }
    }
}
