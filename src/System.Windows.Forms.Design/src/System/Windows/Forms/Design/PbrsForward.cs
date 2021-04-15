// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    internal partial class PbrsForward : IWindowTarget
    {

        private Control target;
        private IWindowTarget oldTarget;


        // we save the last key down so we can recreate the last message if we need to activate
        // the properties window...
        //
        private Message lastKeyDown;
        private ArrayList bufferedChars;

        private const int WM_PRIVATE_POSTCHAR = NativeMethods.WM_USER + 0x1598;
        private bool postCharMessage;

        private IMenuCommandService menuCommandSvc;

        private IServiceProvider sp;

        private bool ignoreMessages = false;

        public PbrsForward(Control target, IServiceProvider sp)
        {
            this.target = target;
            this.oldTarget = target.WindowTarget;
            this.sp = sp;
            target.WindowTarget = this;
        }


        private IMenuCommandService MenuCommandService
        {
            get
            {
                if (menuCommandSvc == null && sp != null)
                {
                    menuCommandSvc = (IMenuCommandService)sp.GetService(typeof(IMenuCommandService));
                }
                return menuCommandSvc;
            }
        }

        private ISupportInSituService InSituSupportService
        {
            get
            {
                return (ISupportInSituService)sp.GetService(typeof(ISupportInSituService));
            }
        }

        public void Dispose()
        {
            target.WindowTarget = oldTarget;
        }


        /// <include file='doc\IWindowTarget.uex' path='docs/doc[@for="IWindowTarget.OnHandleChange"]/*' />
        /// <devdoc>
        ///      Called when the window handle of the control has changed.
        /// </devdoc>
        void IWindowTarget.OnHandleChange(IntPtr newHandle)
        {
        }

        /// <include file='doc\IWindowTarget.uex' path='docs/doc[@for="IWindowTarget.OnMessage"]/*' />
        /// <devdoc>
        ///      Called to do control-specific processing for this window.
        /// </devdoc>
        void IWindowTarget.OnMessage(ref Message m)
        {
            // Get the Designer for the currently selected item on the Designer...
            // SET STATE ..
            ignoreMessages = false;

            // Here lets query for the ISupportInSituService.
            // If we find the service then ask if it has a designer which is interested 
            // in getting the keychars by querring the IgnoreMessages.
            if (m.Msg >= NativeMethods.WM_KEYFIRST && m.Msg <= NativeMethods.WM_KEYLAST
               || (m.Msg >= NativeMethods.WM_IME_STARTCOMPOSITION && m.Msg <= NativeMethods.WM_IME_COMPOSITION))
            {
                if (InSituSupportService != null)
                {
                    ignoreMessages = InSituSupportService.IgnoreMessages;
                }
            }

            switch (m.Msg)
            {
                case WM_PRIVATE_POSTCHAR:

                    if (bufferedChars == null)
                    {
                        return;
                    }

                    // recreate the keystroke to the newly activated window
                    IntPtr hWnd = IntPtr.Zero;

                    if (!ignoreMessages)
                    {
                        hWnd = NativeMethods.GetFocus();
                    }
                    else
                    {
                        if (InSituSupportService != null)
                        {
                            hWnd = InSituSupportService.GetEditWindow();
                        }
                        else
                        {
                            hWnd = NativeMethods.GetFocus();
                        }
                    }
                    if (hWnd != m.HWnd)
                    {
                        foreach (BufferedKey bk in bufferedChars)
                        {
                            if (bk.KeyChar.Msg == NativeMethods.WM_CHAR)
                            {
                                if (bk.KeyDown.Msg != 0)
                                {
                                    NativeMethods.SendMessage(hWnd, NativeMethods.WM_KEYDOWN, bk.KeyDown.WParam, bk.KeyDown.LParam);
                                }
                                NativeMethods.SendMessage(hWnd, NativeMethods.WM_CHAR, bk.KeyChar.WParam, bk.KeyChar.LParam);
                                if (bk.KeyUp.Msg != 0)
                                {
                                    NativeMethods.SendMessage(hWnd, NativeMethods.WM_KEYUP, bk.KeyUp.WParam, bk.KeyUp.LParam);
                                }
                            }
                            else
                            {
                                NativeMethods.SendMessage(hWnd, bk.KeyChar.Msg, bk.KeyChar.WParam, bk.KeyChar.LParam);
                            }
                        }
                    }
                    bufferedChars.Clear();
                    return;

                case NativeMethods.WM_KEYDOWN:
                    this.lastKeyDown = m;
                    break;

                case NativeMethods.WM_IME_ENDCOMPOSITION:
                case NativeMethods.WM_KEYUP:
                    this.lastKeyDown.Msg = 0;
                    break;

                case NativeMethods.WM_CHAR:
                case NativeMethods.WM_IME_STARTCOMPOSITION:
                case NativeMethods.WM_IME_COMPOSITION:
                    if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                    {
                        break;
                    }

                    if (bufferedChars == null)
                    {
                        bufferedChars = new ArrayList();
                    }
                    bufferedChars.Add(new BufferedKey(lastKeyDown, m, lastKeyDown));

                    if (!ignoreMessages && MenuCommandService != null)
                    {
                        // throw the properties window command, we will redo the keystroke when we actually
                        // lose focus
                        postCharMessage = true;
                        MenuCommandService.GlobalInvoke(StandardCommands.PropertiesWindow);
                    }
                    else if (ignoreMessages && m.Msg != NativeMethods.WM_IME_COMPOSITION)
                    {
                        if (InSituSupportService != null)
                        {
                            postCharMessage = true;
                            InSituSupportService.HandleKeyChar();
                        }
                    }

                    if (postCharMessage)
                    {
                        // If copy of message has been buffered for forwarding, eat the original now
                        return;
                    }

                    break;

                case NativeMethods.WM_KILLFOCUS:
                    if (postCharMessage)
                    {
                        // see ASURT 45313
                        // now that we've actually lost focus, post this message to the queue.  This allows
                        // any activity that's in the queue to settle down before our characters are posted.
                        // to the queue.
                        //
                        // we post because we need to allow the focus to actually happen before we send 
                        // our strokes so we know where to send them
                        //
                        // we can't use the wParam here because it may not be the actual window that needs
                        // to pick up the strokes.
                        //
                        UnsafeNativeMethods.PostMessage(target.Handle, WM_PRIVATE_POSTCHAR, IntPtr.Zero, IntPtr.Zero);
                        postCharMessage = false;
                    }
                    break;
            }

            if (this.oldTarget != null)
            {
                oldTarget.OnMessage(ref m);
            }
        }

    }
}
