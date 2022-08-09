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

            protected unsafe override IntPtr HookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                switch ((User32.WM)msg)
                {
                    case User32.WM.INITDIALOG:
                        User32.SendDlgItemMessageW(
                            hwnd,
                            (User32.DialogItemID)Comdlg32.COLOR.HUE,
                            (User32.WM)User32.EM.SETMARGINS,
                            (IntPtr)(User32.EC.LEFTMARGIN | User32.EC.RIGHTMARGIN));
                        User32.SendDlgItemMessageW(
                            hwnd,
                            (User32.DialogItemID)Comdlg32.COLOR.SAT,
                            (User32.WM)User32.EM.SETMARGINS,
                            (IntPtr)(User32.EC.LEFTMARGIN | User32.EC.RIGHTMARGIN));
                        User32.SendDlgItemMessageW(
                            hwnd,
                            (User32.DialogItemID)Comdlg32.COLOR.LUM,
                            (User32.WM)User32.EM.SETMARGINS,
                            (IntPtr)(User32.EC.LEFTMARGIN | User32.EC.RIGHTMARGIN));
                        User32.SendDlgItemMessageW(
                            hwnd,
                            (User32.DialogItemID)Comdlg32.COLOR.RED,
                            (User32.WM)User32.EM.SETMARGINS,
                            (IntPtr)(User32.EC.LEFTMARGIN | User32.EC.RIGHTMARGIN));
                        User32.SendDlgItemMessageW(
                            hwnd,
                            (User32.DialogItemID)Comdlg32.COLOR.GREEN,
                            (User32.WM)User32.EM.SETMARGINS,
                            (IntPtr)(User32.EC.LEFTMARGIN | User32.EC.RIGHTMARGIN));
                        User32.SendDlgItemMessageW(
                            hwnd,
                            (User32.DialogItemID)Comdlg32.COLOR.BLUE,
                            (User32.WM)User32.EM.SETMARGINS,
                            (IntPtr)(User32.EC.LEFTMARGIN | User32.EC.RIGHTMARGIN));
                        IntPtr hwndCtl = User32.GetDlgItem(hwnd, (User32.DialogItemID)Comdlg32.COLOR.MIX);
                        User32.EnableWindow(hwndCtl, false);
                        User32.SetWindowPos(
                            hwndCtl,
                            User32.HWND_TOP,
                            flags: User32.SWP.HIDEWINDOW);
                        hwndCtl = User32.GetDlgItem(hwnd, (User32.DialogItemID)User32.ID.OK);
                        User32.EnableWindow(hwndCtl, false);
                        User32.SetWindowPos(
                            hwndCtl,
                            User32.HWND_TOP,
                            flags: User32.SWP.HIDEWINDOW);
                        Color = Color.Empty;
                        break;

                    case User32.WM.COMMAND:
                        if (PARAM.LOWORD(wParam) == (int)Comdlg32.COLOR.ADD)
                        {
                            BOOL success = false;
                            byte red = (byte)User32.GetDlgItemInt(hwnd, (int)Comdlg32.COLOR.RED, &success, false);
                            Debug.Assert(!success, "Couldn't find dialog member COLOR_RED");

                            byte green = (byte)User32.GetDlgItemInt(hwnd, (int)Comdlg32.COLOR.GREEN, &success, false);
                            Debug.Assert(!success, "Couldn't find dialog member COLOR_GREEN");

                            byte blue = (byte)User32.GetDlgItemInt(hwnd, (int)Comdlg32.COLOR.BLUE, &success, false);
                            Debug.Assert(!success, "Couldn't find dialog member COLOR_BLUE");

                            Color = Color.FromArgb(red, green, blue);
                            User32.PostMessageW(
                                hwnd,
                                User32.WM.COMMAND,
                                PARAM.FromLowHigh((int)User32.ID.OK, 0),
                                User32.GetDlgItem(hwnd, (User32.DialogItemID)User32.ID.OK));
                            break;
                        }

                        break;
                }

                return base.HookProc(hwnd, msg, wParam, lParam);
            }
        }
    }
}
