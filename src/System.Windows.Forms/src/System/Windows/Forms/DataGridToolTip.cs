// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.Drawing;
    
    using System.Windows.Forms;
    using Microsoft.Win32;
    using System.Diagnostics;
    using System.ComponentModel;

    // this class is basically a NativeWindow that does toolTipping
    // should be one for the entire grid
    internal class DataGridToolTip : MarshalByRefObject {
        // the toolTip control
        private NativeWindow tipWindow = null;

        // the dataGrid which contains this toolTip
        private DataGrid dataGrid = null;

        // CONSTRUCTOR
        public DataGridToolTip(DataGrid dataGrid)
        {
            Debug.Assert(dataGrid!= null, "can't attach a tool tip to a null grid");
            this.dataGrid = dataGrid;
        }

        // will ensure that the toolTip window was created
        public void CreateToolTipHandle()
        {
            if (tipWindow == null || tipWindow.Handle == IntPtr.Zero)
            {
                NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                icc.dwICC = NativeMethods.ICC_TAB_CLASSES;
                icc.dwSize = Marshal.SizeOf(icc);
                SafeNativeMethods.InitCommonControlsEx(icc);
                CreateParams cparams = new CreateParams();
                cparams.Parent = dataGrid.Handle;
                cparams.ClassName = NativeMethods.TOOLTIPS_CLASS;
                cparams.Style = NativeMethods.TTS_ALWAYSTIP;
                tipWindow = new NativeWindow();
                tipWindow.CreateHandle(cparams);

                UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
                SafeNativeMethods.SetWindowPos(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOACTIVATE);
                UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_SETDELAYTIME, NativeMethods.TTDT_INITIAL, 0);
            }
        }

        // this function will add a toolTip to the
        // windows system
        public void AddToolTip(String toolTipString, IntPtr toolTipId, Rectangle iconBounds)
        {
            Debug.Assert(tipWindow != null && tipWindow.Handle != IntPtr.Zero, "the tipWindow was not initialized, bailing out");

            if (toolTipString == null)
                throw new ArgumentNullException(nameof(toolTipString));
            if (iconBounds.IsEmpty)
                throw new ArgumentNullException(nameof(iconBounds), SR.DataGridToolTipEmptyIcon);

            NativeMethods.TOOLINFO_T toolInfo = new NativeMethods.TOOLINFO_T();
            toolInfo.cbSize = Marshal.SizeOf(toolInfo);
            toolInfo.hwnd = dataGrid.Handle;
            toolInfo.uId = toolTipId;
            toolInfo.lpszText = toolTipString;
            toolInfo.rect = NativeMethods.RECT.FromXYWH(iconBounds.X, iconBounds.Y, iconBounds.Width, iconBounds.Height);
            toolInfo.uFlags = NativeMethods.TTF_SUBCLASS;
            UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_ADDTOOL, 0, toolInfo);
        }

        public void RemoveToolTip(IntPtr toolTipId)
        {
            NativeMethods.TOOLINFO_T toolInfo = new NativeMethods.TOOLINFO_T();
            toolInfo.cbSize = Marshal.SizeOf(toolInfo);
            toolInfo.hwnd = dataGrid.Handle;
            toolInfo.uId = toolTipId;
            UnsafeNativeMethods.SendMessage(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.TTM_DELTOOL, 0, toolInfo);
        }

        // will destroy the tipWindow
        public void Destroy()
        {
            Debug.Assert(tipWindow != null, "how can one destroy a null window");
            tipWindow.DestroyHandle();
            tipWindow = null;
        }
    }
}
