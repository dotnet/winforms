// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel.Design;
using static Interop;

namespace System.Windows.Forms.Design
{
    internal partial class PbrsForward : IWindowTarget
    {
        private readonly Control target;
        private readonly IWindowTarget oldTarget;

        // we save the last key down so we can recreate the last message if we need to activate
        // the properties window...
        //
        private Message lastKeyDown;
        private ArrayList bufferedChars;

        private const int WM_PRIVATE_POSTCHAR = (int)User32.WM.USER + 0x1598;
        private bool postCharMessage;

        private IMenuCommandService menuCommandSvc;

        private readonly IServiceProvider sp;

        private bool ignoreMessages;

        public PbrsForward(Control target, IServiceProvider sp)
        {
            this.target = target;
            oldTarget = target.WindowTarget;
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

        /// <summary>
        ///  Called when the window handle of the control has changed.
        /// </summary>
        void IWindowTarget.OnHandleChange(IntPtr newHandle)
        {
        }

        /// <summary>
        ///  Called to do control-specific processing for this window.
        /// </summary>
        void IWindowTarget.OnMessage(ref Message m)
        {
            // Get the Designer for the currently selected item on the Designer...
            // SET STATE ..
            ignoreMessages = false;

            // Here lets query for the ISupportInSituService.
            // If we find the service then ask if it has a designer which is interested
            // in getting the keychars by querying the IgnoreMessages.
            if ((m.Msg >= (int)User32.WM.KEYFIRST && m.Msg <= (int)User32.WM.KEYLAST)
               || (m.Msg >= (int)User32.WM.IME_STARTCOMPOSITION && m.Msg <= (int)User32.WM.IME_COMPOSITION))
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
                    IntPtr hWnd;
                    if (!ignoreMessages)
                    {
                        hWnd = User32.GetFocus();
                    }
                    else
                    {
                        if (InSituSupportService != null)
                        {
                            hWnd = InSituSupportService.GetEditWindow();
                        }
                        else
                        {
                            hWnd = User32.GetFocus();
                        }
                    }

                    if (hWnd != m.HWnd)
                    {
                        foreach (BufferedKey bk in bufferedChars)
                        {
                            if (bk.KeyChar.Msg == (int)User32.WM.CHAR)
                            {
                                if (bk.KeyDown.Msg != 0)
                                {
                                    User32.SendMessageW(hWnd, User32.WM.KEYDOWN, bk.KeyDown.WParam, bk.KeyDown.LParam);
                                }

                                User32.SendMessageW(hWnd, User32.WM.CHAR, bk.KeyChar.WParam, bk.KeyChar.LParam);
                                if (bk.KeyUp.Msg != 0)
                                {
                                    User32.SendMessageW(hWnd, User32.WM.KEYUP, bk.KeyUp.WParam, bk.KeyUp.LParam);
                                }
                            }
                            else
                            {
                                User32.SendMessageW(hWnd, (User32.WM)bk.KeyChar.Msg, bk.KeyChar.WParam, bk.KeyChar.LParam);
                            }
                        }
                    }

                    bufferedChars.Clear();
                    return;

                case (int)User32.WM.KEYDOWN:
                    lastKeyDown = m;
                    break;

                case (int)User32.WM.IME_ENDCOMPOSITION:
                case (int)User32.WM.KEYUP:
                    lastKeyDown.Msg = 0;
                    break;

                case (int)User32.WM.CHAR:
                case (int)User32.WM.IME_STARTCOMPOSITION:
                case (int)User32.WM.IME_COMPOSITION:
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
                    else if (ignoreMessages && m.Msg != (int)User32.WM.IME_COMPOSITION)
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

                case (int)User32.WM.KILLFOCUS:
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
                        User32.PostMessageW(target.Handle, (User32.WM)WM_PRIVATE_POSTCHAR, IntPtr.Zero, IntPtr.Zero);
                        postCharMessage = false;
                    }

                    break;
            }

            if (oldTarget != null)
            {
                oldTarget.OnMessage(ref m);
            }
        }
    }
}
