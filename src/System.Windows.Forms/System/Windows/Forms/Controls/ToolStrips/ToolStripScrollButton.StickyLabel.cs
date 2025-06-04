// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal partial class ToolStripScrollButton
{
    internal class StickyLabel : Label
    {
        private readonly bool _upDirection;

        internal StickyLabel(bool up)
        {
            _upDirection = up;
        }

        internal ToolStripScrollButton? OwnerScrollButton { get; set; }

        internal bool UpDirection
            => _upDirection;

        public static bool FreezeLocationChange
            => false;

        protected override AccessibleObject CreateAccessibilityInstance()
            => new StickyLabelAccessibleObject(this);

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
            if (m.Msg is >= ((int)PInvokeCore.WM_KEYFIRST) and <= ((int)PInvokeCore.WM_KEYLAST))
            {
                DefWndProc(ref m);
                return;
            }

            base.WndProc(ref m);
        }
    }
}
