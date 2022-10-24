﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  A non selectable ToolStrip item
    /// </summary>
    internal partial class ToolStripScrollButton : ToolStripControlHost
    {
        private readonly bool _up = true;

        private static readonly Size defaultBitmapSize = new(16, 16);

        [ThreadStatic]
        private static Bitmap? t_upScrollImage;

        [ThreadStatic]
        private static Bitmap? t_downScrollImage;

        const int AUTOSCROLL_UPDATE = 50;
        private static readonly int AUTOSCROLL_PAUSE = SystemInformation.DoubleClickTime;

        private Timer? _mouseDownTimer;

        public ToolStripScrollButton(bool up)
            : base(CreateControlInstance(up))
        {
            if (Control is StickyLabel stickyLabel)
            {
                stickyLabel.OwnerScrollButton = this;
            }

            _up = up;
        }

        protected override AccessibleObject CreateAccessibilityInstance()
           => Control.AccessibilityObject;

        private static Control CreateControlInstance(bool up)
            => new StickyLabel(up)
            {
                ImageAlign = ContentAlignment.MiddleCenter,
                Image = (up) ? UpImage : DownImage
            };

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
                t_downScrollImage ??= DpiHelper.GetScaledBitmapFromIcon(typeof(ToolStripScrollButton), "ScrollButtonDown", defaultBitmapSize);

                return t_downScrollImage;
            }
        }

        internal StickyLabel Label
            => (StickyLabel)Control;

        private static Image UpImage
        {
            get
            {
                t_upScrollImage ??= DpiHelper.GetScaledBitmapFromIcon(typeof(ToolStripScrollButton), "ScrollButtonUp", defaultBitmapSize);

                return t_upScrollImage;
            }
        }

        private Timer MouseDownTimer
        {
            get
            {
                _mouseDownTimer ??= new Timer();

                return _mouseDownTimer;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_mouseDownTimer is not null)
                {
                    _mouseDownTimer.Enabled = false;
                    _mouseDownTimer.Dispose();
                    _mouseDownTimer = null;
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
            MouseDownTimer.Tick -= new EventHandler(OnAutoScrollAccelerate);
        }

        private void OnAutoScrollAccelerate(object? sender, EventArgs e)
        {
            Scroll();
        }

        private void OnInitialAutoScrollMouseDown(object? sender, EventArgs e)
        {
            MouseDownTimer.Tick -= new EventHandler(OnInitialAutoScrollMouseDown);

            Scroll();
            MouseDownTimer.Interval = AUTOSCROLL_UPDATE;
            MouseDownTimer.Tick += new EventHandler(OnAutoScrollAccelerate);
        }

        public override Size GetPreferredSize(Size constrainingSize)
        {
            Size preferredSize = Size.Empty;
            preferredSize.Height = (Label.Image is not null) ? Label.Image.Height + 4 : 0;
            preferredSize.Width = (ParentInternal is not null) ? ParentInternal.Width - 2 : preferredSize.Width; // Two for border
            return preferredSize;
        }

        private void Scroll()
        {
            if (ParentInternal is ToolStripDropDownMenu parent && Label.Enabled)
            {
                parent.ScrollInternal(_up);
            }
        }
    }
}
