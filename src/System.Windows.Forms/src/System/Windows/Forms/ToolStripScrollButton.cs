// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A non selectable ToolStrip item
    /// </summary>
    internal class ToolStripScrollButton : ToolStripControlHost
    {
        private readonly bool up = true;

        [ThreadStatic]
        private static Bitmap upScrollImage;

        [ThreadStatic]
        private static Bitmap downScrollImage;

        const int AUTOSCROLL_UPDATE = 50;
        private static readonly int AUTOSCROLL_PAUSE = SystemInformation.DoubleClickTime;

        private Timer mouseDownTimer;

        public ToolStripScrollButton(bool up) : base(CreateControlInstance(up))
        {
            this.up = up;
        }

        private static Control CreateControlInstance(bool up)
        {
            StickyLabel label = new StickyLabel
            {
                ImageAlign = ContentAlignment.MiddleCenter,
                Image = (up) ? UpImage : DownImage
            };
            return label;
        }

        /// <summary>
        ///  Deriving classes can override this to configure a default size for their control.
        ///  This is more efficient than setting the size in the control's constructor.
        /// </summary>
        protected internal override Padding DefaultMargin
        {
            get
            {
                return Padding.Empty;
            }
        }
        protected override Padding DefaultPadding
        {
            get
            {
                return Padding.Empty;
            }
        }

        private static Image DownImage
        {
            get
            {
                if (downScrollImage is null)
                {
                    downScrollImage = DpiHelper.GetBitmapFromIcon(typeof(ToolStripScrollButton), "ScrollButtonDown");
                }
                return downScrollImage;
            }
        }

        internal StickyLabel Label
        {
            get
            {
                return Control as StickyLabel;
            }
        }

        private static Image UpImage
        {
            get
            {
                if (upScrollImage is null)
                {
                    upScrollImage = DpiHelper.GetBitmapFromIcon(typeof(ToolStripScrollButton), "ScrollButtonUp");
                }
                return upScrollImage;
            }
        }

        private Timer MouseDownTimer
        {
            get
            {
                if (mouseDownTimer is null)
                {
                    mouseDownTimer = new Timer();
                }
                return mouseDownTimer;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (mouseDownTimer != null)
                {
                    mouseDownTimer.Enabled = false;
                    mouseDownTimer.Dispose();
                    mouseDownTimer = null;
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            UnsubscribeAll();

            base.OnMouseDown(e);
            Scroll();

            MouseDownTimer.Interval = AUTOSCROLL_PAUSE;
            MouseDownTimer.Tick += new EventHandler(OnInitialAutoScrollMouseDown);
            MouseDownTimer.Enabled = true;
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            UnsubscribeAll();
            base.OnMouseUp(e);
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            UnsubscribeAll();
        }
        private void UnsubscribeAll()
        {
            MouseDownTimer.Enabled = false;
            MouseDownTimer.Tick -= new EventHandler(OnInitialAutoScrollMouseDown);
            MouseDownTimer.Tick -= new EventHandler(OnAutoScrollAccellerate);
        }

        private void OnAutoScrollAccellerate(object sender, EventArgs e)
        {
            Scroll();
        }

        private void OnInitialAutoScrollMouseDown(object sender, EventArgs e)
        {
            MouseDownTimer.Tick -= new EventHandler(OnInitialAutoScrollMouseDown);

            Scroll();
            MouseDownTimer.Interval = AUTOSCROLL_UPDATE;
            MouseDownTimer.Tick += new EventHandler(OnAutoScrollAccellerate);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = Size.Empty;
            preferredSize.Height = (Label.Image != null) ? Label.Image.Height + 4 : 0;
            preferredSize.Width = (ParentInternal != null) ? ParentInternal.Width - 2 : preferredSize.Width; // Two for border
            return preferredSize;
        }

        private void Scroll()
        {
            if (ParentInternal is ToolStripDropDownMenu parent && Label.Enabled)
            {
                parent.ScrollInternal(up);
            }
        }

        internal class StickyLabel : Label
        {
            public StickyLabel()
            {
            }

            public bool FreezeLocationChange
            {
                get => false;
            }

            protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
            {
                if (((specified & BoundsSpecified.Location) != 0) && FreezeLocationChange)
                {
                    return;
                }
                base.SetBoundsCore(x, y, width, height, specified);
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg >= (int)User32.WM.KEYFIRST && m.Msg <= (int)User32.WM.KEYLAST)
                {
                    DefWndProc(ref m);
                    return;
                }

                base.WndProc(ref m);
            }
        }
    }
}
