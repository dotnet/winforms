// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    // this class is basically a NativeWindow that does toolTipping
    // should be one for the entire grid
    internal class DataGridToolTip : MarshalByRefObject
    {
        // the toolTip control
        private NativeWindow tipWindow = null;

        // the dataGrid which contains this toolTip
        private readonly DataGrid dataGrid = null;

        // CONSTRUCTOR
        public DataGridToolTip(DataGrid dataGrid)
        {
            Debug.Assert(dataGrid != null, "can't attach a tool tip to a null grid");
            this.dataGrid = dataGrid;
        }

        // will ensure that the toolTip window was created
        public void CreateToolTipHandle()
        {
            if (tipWindow == null || tipWindow.Handle == IntPtr.Zero)
            {
                NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX
                {
                    dwICC = NativeMethods.ICC_TAB_CLASSES
                };
                icc.dwSize = Marshal.SizeOf(icc);
                SafeNativeMethods.InitCommonControlsEx(icc);
                CreateParams cparams = new CreateParams
                {
                    Parent = dataGrid.Handle,
                    ClassName = NativeMethods.TOOLTIPS_CLASS,
                    Style = NativeMethods.TTS_ALWAYSTIP
                };
                tipWindow = new NativeWindow();
                tipWindow.CreateHandle(cparams);

                User32.SendMessageW(tipWindow, WindowMessages.TTM_SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);
                SafeNativeMethods.SetWindowPos(new HandleRef(tipWindow, tipWindow.Handle), NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOACTIVATE);
                User32.SendMessageW(tipWindow, WindowMessages.TTM_SETDELAYTIME, (IntPtr)ComCtl32.TTDT.INITIAL, (IntPtr)0);
            }
        }

        public void AddToolTip(string toolTipString, IntPtr toolTipId, Rectangle iconBounds)
        {
            Debug.Assert(tipWindow != null && tipWindow.Handle != IntPtr.Zero, "the tipWindow was not initialized, bailing out");
            if (iconBounds.IsEmpty)
                throw new ArgumentNullException(nameof(iconBounds), SR.DataGridToolTipEmptyIcon);

            if (toolTipString == null)
                throw new ArgumentNullException(nameof(toolTipString));

            var info = new ComCtl32.ToolInfoWrapper(dataGrid, toolTipId, ComCtl32.TTF.SUBCLASS, toolTipString, iconBounds);
            info.SendMessage(tipWindow, WindowMessages.TTM_ADDTOOLW);
        }

        public void RemoveToolTip(IntPtr toolTipId)
        {
            var info = new ComCtl32.ToolInfoWrapper(dataGrid, toolTipId);
            info.SendMessage(tipWindow, WindowMessages.TTM_DELTOOLW);
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
