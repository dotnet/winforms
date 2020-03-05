// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Threading;
using WFCTestLib.Log;
using ReflectTools;
using System.Windows.Forms.IntegrationTests.Common;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiListViewTests : ReflectBase
    {
        private readonly ListView _listView;

        public MauiListViewTests(string[] args) : base(args)
        {
            this.BringToForeground();
            _listView = new ListView { Size = new System.Drawing.Size(439, 103) };
            Controls.Add(_listView);
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiListViewTests(args));
        }

        [Scenario(true)]
        public ScenarioResult Click_On_Second_Column_Does_Not_Alter_Checkboxes(TParams p)
        {
            InitializeItems(_listView, p);

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.StateImageIndex != 0)
                    return new ScenarioResult(false, "Precondition failed: all checkboxes must be unmarked");
            }

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.Selected)
                    return new ScenarioResult(false, "Precondition failed: all items must be unselected");
            }

            SendKey(Keys.ShiftKey, true);

            var pt = GetCenter(_listView.RectangleToScreen(_listView.Items[0].SubItems[1].Bounds));
            MouseHelper.RaiseMouseClick(pt.x, pt.y);

            pt = GetCenter(_listView.RectangleToScreen(_listView.Items[2].SubItems[1].Bounds));
            MouseHelper.RaiseMouseClick(pt.x, pt.y);

            SendKey(Keys.ShiftKey, false);
            Application.DoEvents();

            foreach (ListViewItem item in _listView.Items)
            {
                if (item.StateImageIndex != 0)
                    return new ScenarioResult(false, "All checkboxes must be unmarked");
            }

            foreach (ListViewItem item in _listView.Items)
            {
                if (!item.Selected)
                    return new ScenarioResult(false, "All items must be selected");
            }

            return new ScenarioResult(true);
        }

        private unsafe void SendKey(Keys key, bool down)
        {
            var input = new User32.INPUT();
            input.type                      = User32.INPUTENUM.KEYBOARD;
            input.inputUnion.ki.wVk         = (ushort)key;
            input.inputUnion.ki.dwFlags     = 0;
            input.inputUnion.ki.dwExtraInfo = IntPtr.Zero;
            input.inputUnion.ki.time        = 0;
            input.inputUnion.ki.wScan       = 0;

            if (!down)
                input.inputUnion.ki.dwFlags |= User32.KEYEVENTF.KEYUP;

            User32.SendInput(1, &input, Marshal.SizeOf(input));
        }

        private (int x, int y) GetCenter(System.Drawing.Rectangle rect)
        {
            int x = rect.Left + ((rect.Right - rect.Left) / 2);
            int y = rect.Top + ((rect.Bottom - rect.Top) / 2);
            return (x, y);
        }

        private void InitializeItems(ListView listView, TParams p)
        {
            listView.Items.Clear();
            listView.CheckBoxes = true;
            listView.FullRowSelect = true;
            listView.View = View.Details;

            var columnHeader1 = new ColumnHeader { Text = "ColumnHeader1", Width = 140 };
            var columnHeader2 = new ColumnHeader { Text = "ColumnHeader2", Width = 140 };
            var columnHeader3 = new ColumnHeader { Text = "ColumnHeader3", Width = 140 };
            listView.Columns.AddRange(new[] { columnHeader1, columnHeader2, columnHeader3 });

            var listViewItem1 = new ListViewItem(new[] { "row1", "row1Col2", "row1Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem2 = new ListViewItem(new[] { "row2", "row2Col2", "row2Col3" }, -1) { StateImageIndex = 0 };
            var listViewItem3 = new ListViewItem(new[] { "row3", "row3Col2", "row3Col3" }, -1) { StateImageIndex = 0 };
            listView.Items.AddRange(new[] { listViewItem1, listViewItem2, listViewItem3 });
        }
    }

    internal static class MouseHelper
    {
        public static void RaiseMouseClick(int x, int y)
        {
            POINT previousPosition = new POINT();
            bool setOldCursorPos = UnsafeNativeMethods.GetPhysicalCursorPos(ref previousPosition);

            bool mouseSwapped = User32.GetSystemMetrics(User32.SystemMetric.SM_SWAPBUTTON) != 0;

            SendMouseInput(x, y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
            SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTDOWN : User32.MOUSEEVENTF.LEFTDOWN);
            SendMouseInput(0, 0, mouseSwapped ? User32.MOUSEEVENTF.RIGHTUP : User32.MOUSEEVENTF.LEFTUP);

            Threading.Thread.Sleep(50);

            // Set back the mouse position where it was.
            if (setOldCursorPos)
            {
                SendMouseInput(previousPosition.x, previousPosition.y, User32.MOUSEEVENTF.MOVE | User32.MOUSEEVENTF.ABSOLUTE);
            }
        }

        private unsafe static void SendMouseInput(int x, int y, User32.MOUSEEVENTF flags)
        {
            if ((flags & User32.MOUSEEVENTF.ABSOLUTE) != 0)
            {
                int vscreenWidth = User32.GetSystemMetrics(User32.SystemMetric.SM_CXVIRTUALSCREEN);
                int vscreenHeight = User32.GetSystemMetrics(User32.SystemMetric.SM_CYVIRTUALSCREEN);
                int vscreenLeft = User32.GetSystemMetrics(User32.SystemMetric.SM_XVIRTUALSCREEN);
                int vscreenTop = User32.GetSystemMetrics(User32.SystemMetric.SM_YVIRTUALSCREEN);

                const int DesktopNormilizedMax = 65536;

                // Absolute input requires that input is in 'normalized' coords - with the entire
                // desktop being (0,0)...(65535,65536). Need to convert input x,y coords to this
                // first.
                //
                // In this normalized world, any pixel on the screen corresponds to a block of values
                // of normalized coords - eg. on a 1024x768 screen,
                // y pixel 0 corresponds to range 0 to 85.333,
                // y pixel 1 corresponds to range 85.333 to 170.666,
                // y pixel 2 correpsonds to range 170.666 to 256 - and so on.
                // Doing basic scaling math - (x-top)*65536/Width - gets us the start of the range.
                // However, because int math is used, this can end up being rounded into the wrong
                // pixel. For example, if we wanted pixel 1, we'd get 85.333, but that comes out as
                // 85 as an int, which falls into pixel 0's range - and that's where the pointer goes.
                // To avoid this, we add on half-a-"screen pixel"'s worth of normalized coords - to
                // push us into the middle of any given pixel's range - that's the 65536/(Width*2)
                // part of the formula. So now pixel 1 maps to 85+42 = 127 - which is comfortably
                // in the middle of that pixel's block.
                // The key ting here is that unlike points in coordinate geometry, pixels take up
                // space, so are often better treated like rectangles - and if you want to target
                // a particular pixel, target its rectangle's midpoint, not its edge.
                x = ((x - vscreenLeft) * DesktopNormilizedMax) / vscreenWidth + DesktopNormilizedMax / (vscreenWidth * 2);
                y = ((y - vscreenTop) * DesktopNormilizedMax) / vscreenHeight + DesktopNormilizedMax / (vscreenHeight * 2);

                flags |= User32.MOUSEEVENTF.VIRTUALDESK;
            }

            var mouseInput = new User32.INPUT();
            mouseInput.type = User32.INPUTENUM.MOUSE;
            mouseInput.inputUnion.mi.dx = x;
            mouseInput.inputUnion.mi.dy = y;
            mouseInput.inputUnion.mi.mouseData = 0;
            mouseInput.inputUnion.mi.dwFlags = flags;
            mouseInput.inputUnion.mi.time = 0;
            mouseInput.inputUnion.mi.dwExtraInfo = IntPtr.Zero;

            User32.SendInput(1, &mouseInput, Marshal.SizeOf(mouseInput));
        }
    }
}
