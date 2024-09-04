// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    private class DesignerWindowTarget : IWindowTarget, IDesignerTarget, IDisposable
    {
        internal ControlDesigner _designer;
        internal IWindowTarget _oldTarget;

        public DesignerWindowTarget(ControlDesigner designer)
        {
            Control control = designer.Control;
            _designer = designer;
            _oldTarget = control.WindowTarget;
            control.WindowTarget = this;
        }

        public void DefWndProc(ref Message m)
        {
            _oldTarget.OnMessage(ref m);
        }

        public void Dispose()
        {
            if (_designer is not null)
            {
                _designer.Control.WindowTarget = _oldTarget;
                _designer = null!;
            }
        }

        public void OnHandleChange(IntPtr newHandle)
        {
            _oldTarget.OnHandleChange(newHandle);
            if (newHandle != IntPtr.Zero)
            {
                _designer.OnHandleChange();
            }
        }

        public void OnMessage(ref Message m)
        {
            // We want these messages to go through the designer's WndProc method, and we want people to be able
            // to do default processing with the designer's DefWndProc. So, we stuff ourselves into the designers
            // window target and call their WndProc.
            ControlDesigner currentDesigner = _designer;
            if (currentDesigner is not null)
            {
                IDesignerTarget? designerTarget = currentDesigner.DesignerTarget;
                currentDesigner.DesignerTarget = this;
                try
                {
                    currentDesigner.WndProc(ref m);
                }
                catch (Exception ex)
                {
                    currentDesigner.SetUnhandledException(currentDesigner.Control, ex);
                }
                finally
                {
                    currentDesigner.DesignerTarget = designerTarget;
                }
            }
            else
            {
                DefWndProc(ref m);
            }
        }
    }
}
