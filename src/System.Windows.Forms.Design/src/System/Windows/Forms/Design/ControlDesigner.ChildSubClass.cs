// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    private class ChildSubClass : NativeWindow, IDesignerTarget
    {
        private ControlDesigner _designer;

        // AssignHandle calls NativeWindow::OnHandleChanged, but we do not override it so we should be okay
        public ChildSubClass(ControlDesigner designer, IntPtr hwnd)
        {
            _designer = designer;
            if (designer is not null)
            {
                designer.DisposingHandler += OnDesignerDisposing;
            }

            AssignHandle(hwnd);
        }

        void IDesignerTarget.DefWndProc(ref Message m) => DefWndProc(ref m);

        public void Dispose() => _designer = null!;

        private void OnDesignerDisposing(object? sender, EventArgs e) => Dispose();

        protected override void WndProc(ref Message m)
        {
            if (_designer is null)
            {
                DefWndProc(ref m);
                return;
            }

            if (m.MsgInternal == PInvokeCore.WM_DESTROY)
            {
                _designer.RemoveSubclassedWindow(m.HWnd);
            }

            if (m.MsgInternal == PInvokeCore.WM_PARENTNOTIFY && m.WParamInternal.LOWORD == PInvokeCore.WM_CREATE)
            {
                _designer.HookChildHandles((HWND)(nint)m.LParamInternal); // they will get removed from the collection just above
            }

            // We want these messages to go through the designer's WndProc method, and we want people to be able
            // to do default processing with the designer's DefWndProc. So, we stuff ourselves into the designers
            // window target and call their WndProc.
            IDesignerTarget? designerTarget = _designer.DesignerTarget;
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
                if (_designer is not null && _designer.Component is not null)
                {
                    _designer.DesignerTarget = designerTarget;
                }
            }
        }
    }
}
