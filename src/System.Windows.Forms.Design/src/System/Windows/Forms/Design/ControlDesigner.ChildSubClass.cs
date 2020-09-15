// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using static Interop;

namespace System.Windows.Forms.Design
{
    public partial class ControlDesigner
    {
        private class ChildSubClass : NativeWindow, IDesignerTarget
        {
            private ControlDesigner _designer;

            // AssignHandle calls NativeWindow::OnHandleChanged, but we do not override it so we should be okay
            public ChildSubClass(ControlDesigner designer, IntPtr hwnd)
            {
                _designer = designer;
                if (designer != null)
                {
                    designer.DisposingHandler += new EventHandler(OnDesignerDisposing);
                }
                AssignHandle(hwnd);
            }

            void IDesignerTarget.DefWndProc(ref Message m) => base.DefWndProc(ref m);

            public void Dispose() => _designer = null;

            private void OnDesignerDisposing(object sender, EventArgs e) => Dispose();

            protected override void WndProc(ref Message m)
            {
                if (_designer is null)
                {
                    DefWndProc(ref m);
                    return;
                }

                if (m.Msg == (int)User32.WM.DESTROY)
                {
                    _designer.RemoveSubclassedWindow(m.HWnd);
                }
                if (m.Msg == (int)User32.WM.PARENTNOTIFY && PARAM.LOWORD(m.WParam) == (short)User32.WM.CREATE)
                {
                    _designer.HookChildHandles(m.LParam); // they will get removed from the collection just above
                }

                // We want these messages to go through the designer's WndProc method, and we want people to be able
                // to do default processing with the designer's DefWndProc.  So, we stuff ourselves into the designers
                // window target and call their WndProc.
                IDesignerTarget designerTarget = _designer.DesignerTarget;
                _designer.DesignerTarget = this;
                Debug.Assert(m.HWnd == Handle, "Message handle differs from target handle");
                try
                {
                    _designer.WndProc(ref m);
                }
                catch (Exception ex)
                {
                    _designer.SetUnhandledException(Control.FromChildHandle(m.HWnd), ex);
                }
                finally
                {
                    // make sure the designer wasn't destroyed
                    if (_designer != null && _designer.Component != null)
                    {
                        _designer.DesignerTarget = designerTarget;
                    }
                }
            }
        }
    }
}
