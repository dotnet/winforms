// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Design;
    using System.Diagnostics;
    using System.Windows.Forms.ButtonInternal;
    using System.Security.Permissions;
    using System.Security;
        

    /// <include file='doc\ToolStripScrollButton.uex' path='docs/doc[@for="ToolStripScrollButton"]/*' />
    /// <devdoc>
    /// A non selectable winbar item
    /// </devdoc>
    internal class ToolStripScrollButton : ToolStripControlHost {
        private bool up = true;

        [ThreadStatic]
        private static Bitmap upScrollImage;

        [ThreadStatic]
        private static Bitmap downScrollImage;

        const int AUTOSCROLL_UPDATE = 50;
        private static readonly int AUTOSCROLL_PAUSE = SystemInformation.DoubleClickTime;
              

        private Timer mouseDownTimer;
        
        public ToolStripScrollButton(bool up) : base(CreateControlInstance(up)) {
            this.up = up;
        }

        
        private static Control CreateControlInstance(bool up) {
            StickyLabel label = new StickyLabel();
            label.ImageAlign = ContentAlignment.MiddleCenter;
            label.Image = (up) ? UpImage : DownImage;
            return label;
        }

         /// <devdoc>
         /// Deriving classes can override this to configure a default size for their control.
         /// This is more efficient than setting the size in the control's constructor.
         /// </devdoc>
         protected internal override Padding DefaultMargin {
             get {
                 return Padding.Empty;
             }
         }
         protected override Padding DefaultPadding {
             get {
                 return Padding.Empty;
             }
         }

         private static Image DownImage {
            get { 
                if (downScrollImage == null) {
                      downScrollImage = new Bitmap(typeof(ToolStripScrollButton), "ScrollButtonDown.bmp"); 
                      downScrollImage.MakeTransparent(Color.White);

                }
                return downScrollImage;
            }
        }

        
        internal StickyLabel Label {
            get{
                return Control as StickyLabel;
            }
        }
        
        private static Image UpImage {
            get { 
                if (upScrollImage == null) {
                      upScrollImage = new Bitmap(typeof(ToolStripScrollButton), "ScrollButtonUp.bmp"); 
                      upScrollImage.MakeTransparent(Color.White);

                }
                return upScrollImage;
            }
        }

        private Timer MouseDownTimer {
            get{
                if (mouseDownTimer == null) {
                    mouseDownTimer = new Timer();
                }
                return mouseDownTimer;
            }
        }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (mouseDownTimer != null) {
                    mouseDownTimer.Enabled = false;
                    mouseDownTimer.Dispose();
                    mouseDownTimer = null;
                }
            }
            base.Dispose(disposing);
        }
        protected override void OnMouseDown (MouseEventArgs e) {
            UnsubscribeAll();

            base.OnMouseDown(e);
            Scroll();
            
            MouseDownTimer.Interval =  AUTOSCROLL_PAUSE;
            MouseDownTimer.Tick += new EventHandler(OnInitialAutoScrollMouseDown);
            MouseDownTimer.Enabled = true;
        }

        protected override void OnMouseUp (MouseEventArgs e) {
            UnsubscribeAll();
            base.OnMouseUp(e);
        }

        protected override void  OnMouseLeave (EventArgs e) {
            UnsubscribeAll();
        }
        private void UnsubscribeAll() {
            MouseDownTimer.Enabled = false;
            MouseDownTimer.Tick -= new EventHandler(OnInitialAutoScrollMouseDown);
            MouseDownTimer.Tick -= new EventHandler(OnAutoScrollAccellerate);
        }

        private void OnAutoScrollAccellerate(object sender, EventArgs e) {
            Scroll();
        }
        
        private void OnInitialAutoScrollMouseDown(object sender, EventArgs e)  {
            MouseDownTimer.Tick -= new EventHandler(OnInitialAutoScrollMouseDown);

            Scroll();
            MouseDownTimer.Interval =  AUTOSCROLL_UPDATE;
            MouseDownTimer.Tick += new EventHandler(OnAutoScrollAccellerate);
        }

        public override Size GetPreferredSize(Size constrainingSize) {
            Size preferredSize = Size.Empty;
            preferredSize.Height = (Label.Image != null) ? Label.Image.Height + 4 : 0;
            preferredSize.Width = (ParentInternal != null) ? ParentInternal.Width - 2 : preferredSize.Width; // Two for border
            return preferredSize;
        }

        private void Scroll() {
            ToolStripDropDownMenu parent = this.ParentInternal as ToolStripDropDownMenu;
            if (parent != null && Label.Enabled) {
                parent.ScrollInternal(up);
            }
        }

        internal class StickyLabel : Label {

            public StickyLabel() {
            }
            private bool freezeLocationChange = false;
            
            public bool FreezeLocationChange {
              get { return freezeLocationChange; }
            }

            protected override void SetBoundsCore(int x,int y,int width, int height, BoundsSpecified specified) 
            {
                if (((specified & BoundsSpecified.Location) != 0) && FreezeLocationChange) {
                    return;
                }
                base.SetBoundsCore(x, y, width, height, specified);
            }

            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            protected override void WndProc(ref Message m) {

                if (m.Msg >= NativeMethods.WM_KEYFIRST && m.Msg <= NativeMethods.WM_KEYLAST) {
                    // 

                    DefWndProc(ref m);
                    return;
                }
                
                base.WndProc(ref m);
            }
        }
    }
}


