// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

public partial class ControlDesigner
{
    private class ChildWindowTarget : IWindowTarget, IDesignerTarget
    {
        private readonly ControlDesigner _designer;
        private readonly Control _childControl;
        private HWND _handle = HWND.Null;

        public ChildWindowTarget(ControlDesigner designer, Control childControl, IWindowTarget oldWindowTarget)
        {
            _designer = designer;
            _childControl = childControl;
            OldWindowTarget = oldWindowTarget;
        }

        public IWindowTarget OldWindowTarget { get; }

        public void DefWndProc(ref Message m) => OldWindowTarget.OnMessage(ref m);

        public void Dispose()
        {
            // Do nothing. We will pick this up through a null DesignerTarget property when we come out of the message loop.
        }

        public void OnHandleChange(IntPtr newHandle)
        {
            _handle = (HWND)newHandle;
            OldWindowTarget.OnHandleChange(newHandle);
        }

        public void OnMessage(ref Message m)
        {
            // If the designer has jumped ship, the continue partying on messages, but send them back to the original control.
            if (_designer.Component is null)
            {
                OldWindowTarget.OnMessage(ref m);
                return;
            }

            // We want these messages to go through the designer's WndProc method, and we want people to be able
            // to do default processing with the designer's DefWndProc. So, we stuff the old window target into
            // the designer's target and then call their WndProc.
            IDesignerTarget? designerTarget = _designer.DesignerTarget;
            _designer.DesignerTarget = this;

            try
            {
                _designer.WndProc(ref m);
            }
            catch (Exception ex)
            {
                _designer.SetUnhandledException(_childControl, ex);
            }
            finally
            {
                // If the designer disposed us, then we should follow suit.
                if (_designer.DesignerTarget is null)
                {
                    designerTarget?.Dispose();
                }
                else
                {
                    _designer.DesignerTarget = designerTarget;
                }

                // Controls (primarily RichEdit) will register themselves as drag-drop source/targets when they
                // are instantiated. Normally, when they are being designed, we will RevokeDragDrop() in their
                // designers. The problem occurs when these controls are inside a UserControl. At that time, we
                // do not have a designer for these controls, and they prevent the ParentControlDesigner's
                // drag-drop from working. What we do is to loop through all child controls that do not have a
                // designer (in HookChildControls()), and RevokeDragDrop() after their handles have been created.
                if (m.Msg == (int)PInvokeCore.WM_CREATE)
                {
                    Debug.Assert(_handle != IntPtr.Zero, "Handle for control not created");
                    PInvoke.RevokeDragDrop(_handle);
                }
            }
        }
    }
}
