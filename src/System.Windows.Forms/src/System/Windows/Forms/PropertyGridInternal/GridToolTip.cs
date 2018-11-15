// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Message = System.Windows.Forms.Message;

    internal class GridToolTip : Control {

        Control[]    controls;
        string       toolTipText;
        NativeMethods.TOOLINFO_T[]   toolInfos;
        bool         dontShow;
        Point        lastMouseMove = Point.Empty;
        private int maximumToolTipLength = 1000;

        internal GridToolTip(Control[] controls) {
            this.controls = controls;
            SetStyle(ControlStyles.UserPaint, false);
            this.Font = controls[0].Font;
            this.toolInfos = new NativeMethods.TOOLINFO_T[controls.Length];
            
            for (int i = 0; i < controls.Length; i++) {
                  controls[i].HandleCreated += new EventHandler(this.OnControlCreateHandle);
                  controls[i].HandleDestroyed += new EventHandler(this.OnControlDestroyHandle);
                  
                  if (controls[i].IsHandleCreated) {
                     SetupToolTip(controls[i]);
                  }
            }
        }

        public string ToolTip{
            get {
               return toolTipText;
            }
            set {
                  if (this.IsHandleCreated || !String.IsNullOrEmpty(value)) {
                      this.Reset();
                  }

                  if (value != null && value.Length > maximumToolTipLength)
                  {
					  //Let the user know the text was truncated by throwing on an ellipsis
                      value = value.Substring(0, maximumToolTipLength) + "...";
                  }
                  this.toolTipText = value;

                  if (this.IsHandleCreated) {

                     bool visible = this.Visible;

                     if (visible){
                        this.Visible = false;
                     }

                     // here's a workaround.  if we give
                     // the tooltip an empty string, it won't come back
                     // so we just force it hidden instead
                     //
                     if (value == null || value.Length == 0){
                        dontShow = true;
                        value = "";
                     }
                     else{
                        dontShow = false;
                     }
                     
                     for (int i = 0; i < controls.Length; i++) {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_UPDATETIPTEXT, 0, GetTOOLINFO(controls[i]));
                     }

                     if (visible && !dontShow){
                        this.Visible = true;
                     }
                  
               }
            }
        }


        /// <include file='doc\GridToolTip.uex' path='docs/doc[@for="GridToolTip.CreateParams"]/*' />
        /// <devdoc>
        ///     The createParams to create the window.
        /// </devdoc>
        /// <internalonly/>
        protected override  CreateParams CreateParams {
            get {
                NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                icc.dwICC = NativeMethods.ICC_TAB_CLASSES;
                SafeNativeMethods.InitCommonControlsEx(icc);
                CreateParams cp = new CreateParams();
                cp.Parent = IntPtr.Zero;
                cp.ClassName = NativeMethods.TOOLTIPS_CLASS;
                cp.Style |= (NativeMethods.TTS_ALWAYSTIP | NativeMethods.TTS_NOPREFIX);
                cp.ExStyle = 0;
                cp.Caption = this.ToolTip;
                return cp;
            }
        }

        private NativeMethods.TOOLINFO_T GetTOOLINFO(Control c) {
        
            int index = Array.IndexOf(controls, c);
            
            Debug.Assert(index != -1, "Failed to find control in tooltip array");
            
            if (toolInfos[index] == null){
               toolInfos[index] = new NativeMethods.TOOLINFO_T();
               toolInfos[index].cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_T));
               toolInfos[index].uFlags |= NativeMethods.TTF_IDISHWND | NativeMethods.TTF_TRANSPARENT | NativeMethods.TTF_SUBCLASS;
            }
            toolInfos[index].lpszText = this.toolTipText;
            toolInfos[index].hwnd = c.Handle;
            toolInfos[index].uId = c.Handle;
            return toolInfos[index];
        }

        /*
        private bool MouseMoved(Message msg){
            bool moved = true;

            Point newMove = new Point(NativeMethods.Util.LOWORD(msg.LParam), NativeMethods.Util.HIWORD(msg.LParam));

            // check if the mouse has actually moved...
            if (lastMouseMove == newMove){
                  moved = false;
            }

            lastMouseMove = newMove;
            return moved;
        }
        */

        private void OnControlCreateHandle(object sender, EventArgs e){
            SetupToolTip((Control)sender);
        }

        private void OnControlDestroyHandle(object sender, EventArgs e){
            if (IsHandleCreated) {
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_DELTOOL, 0, GetTOOLINFO((Control)sender));
            }
        }
        
        protected override void OnHandleCreated(EventArgs e) {
            base.OnHandleCreated(e);
            for (int i = 0; i < controls.Length; i++) {
                  if (controls[i].IsHandleCreated) {
                     SetupToolTip(controls[i]);
                  }
            }
        }

        private void SetupToolTip(Control c) {
            if (this.IsHandleCreated) {
               SafeNativeMethods.SetWindowPos(new HandleRef(this, Handle), NativeMethods.HWND_TOPMOST,
                                 0, 0, 0, 0,
                                 NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOSIZE |
                                 NativeMethods.SWP_NOACTIVATE);

               if (0 == (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_ADDTOOL, 0, GetTOOLINFO(c))) {
                     Debug.Fail("TTM_ADDTOOL failed for " + c.GetType().Name);
               }

               // Setting the max width has the added benefit of enabling multiline
               // tool tips!)
               //
               UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);
            }
        }

        public void Reset(){
            // okay, this resets the tooltip state,
            // which can get broken when we leave the window
            // then reenter.  So we set the tooltip to null,
            // update the text, then it back to what it was, so the tooltip
            // thinks it's back in the regular state again
            //
            string oldText = this.ToolTip;
            this.toolTipText = "";
            for (int i = 0; i < controls.Length; i++) {
               if (0 == (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_UPDATETIPTEXT, 0, GetTOOLINFO(controls[i]))) {
                    //Debug.Fail("TTM_UPDATETIPTEXT failed for " + controls[i].GetType().Name);
               }
            }
            this.toolTipText = oldText;
            this.SendMessage(NativeMethods.TTM_UPDATE, 0, 0);
        }

        protected override void WndProc(ref Message msg) {
            switch (msg.Msg) {
               case NativeMethods.WM_SHOWWINDOW:
                  if (unchecked( (int) (long)msg.WParam) != 0 && dontShow){
                     msg.WParam = IntPtr.Zero;
                  }
                  break;
               case NativeMethods.WM_NCHITTEST:
                  // When using v6 common controls, the native
                  // tooltip does not end up returning HTTRANSPARENT all the time, so its TTF_TRANSPARENT
                  // behavior does not work, ie. mouse events do not fall thru to controls underneath. This
                  // is due to a combination of old app-specific code in comctl32, functional changes between
                  // v5 and v6, and the specfic way the property grid drives its tooltip. Workaround is to just
                  // force HTTRANSPARENT all the time.
                  msg.Result = (IntPtr) NativeMethods.HTTRANSPARENT;
                  return;
            }
            base.WndProc(ref msg);
        }
    }
}
