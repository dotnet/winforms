// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class PbrsForward : IWindowTarget
{
    private readonly Control target;
    private readonly IWindowTarget oldTarget;

    // we save the last key down so we can recreate the last message if we need to activate
    // the properties window...
    //
    private Message lastKeyDown;
    private List<BufferedKey> bufferedChars;

    private const int WM_PRIVATE_POSTCHAR = (int)PInvoke.WM_USER + 0x1598;
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
            if (menuCommandSvc is null && sp is not null)
            {
                menuCommandSvc = (IMenuCommandService)sp.GetService(typeof(IMenuCommandService));
            }

            return menuCommandSvc;
        }
    }

    private ISupportInSituService InSituSupportService
        => (ISupportInSituService)sp.GetService(typeof(ISupportInSituService));

    public void Dispose() => target.WindowTarget = oldTarget;

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
        if ((m.Msg >= (int)PInvoke.WM_KEYFIRST && m.Msg <= (int)PInvoke.WM_KEYLAST)
           || (m.Msg >= (int)PInvoke.WM_IME_STARTCOMPOSITION && m.Msg <= (int)PInvoke.WM_IME_COMPOSITION))
        {
            if (InSituSupportService is not null)
            {
                ignoreMessages = InSituSupportService.IgnoreMessages;
            }
        }

        switch (m.Msg)
        {
            case WM_PRIVATE_POSTCHAR:

                if (bufferedChars is null)
                {
                    return;
                }

                // recreate the keystroke to the newly activated window
                HWND hwnd;
                hwnd = !ignoreMessages || InSituSupportService is null
                    ? PInvoke.GetFocus()
                    : (HWND)InSituSupportService.GetEditWindow();

                if (hwnd != m.HWnd)
                {
                    foreach (BufferedKey bk in bufferedChars)
                    {
                        if (bk.KeyChar.MsgInternal == PInvoke.WM_CHAR)
                        {
                            if (bk.KeyDown.MsgInternal != 0)
                            {
                                PInvoke.SendMessage(hwnd, PInvoke.WM_KEYDOWN, bk.KeyDown.WParamInternal, bk.KeyDown.LParamInternal);
                            }

                            PInvoke.SendMessage(hwnd, PInvoke.WM_CHAR, bk.KeyChar.WParamInternal, bk.KeyChar.LParamInternal);
                            if (bk.KeyUp.MsgInternal != 0)
                            {
                                PInvoke.SendMessage(hwnd, PInvoke.WM_KEYUP, bk.KeyUp.WParamInternal, bk.KeyUp.LParamInternal);
                            }
                        }
                        else
                        {
                            PInvoke.SendMessage(hwnd, bk.KeyChar.MsgInternal, bk.KeyChar.WParamInternal, bk.KeyChar.LParamInternal);
                        }
                    }
                }

                bufferedChars.Clear();
                return;

            case (int)PInvoke.WM_KEYDOWN:
                lastKeyDown = m;
                break;

            case (int)PInvoke.WM_IME_ENDCOMPOSITION:
            case (int)PInvoke.WM_KEYUP:
                lastKeyDown.Msg = 0;
                break;

            case (int)PInvoke.WM_CHAR:
            case (int)PInvoke.WM_IME_STARTCOMPOSITION:
            case (int)PInvoke.WM_IME_COMPOSITION:
                if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                {
                    break;
                }

                bufferedChars ??= new();
                bufferedChars.Add(new BufferedKey(lastKeyDown, m, lastKeyDown));

                if (!ignoreMessages && MenuCommandService is not null)
                {
                    // throw the properties window command, we will redo the keystroke when we actually
                    // lose focus
                    postCharMessage = true;
                    MenuCommandService.GlobalInvoke(StandardCommands.PropertiesWindow);
                }
                else if (ignoreMessages && m.Msg != (int)PInvoke.WM_IME_COMPOSITION)
                {
                    if (InSituSupportService is not null)
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

            case (int)PInvoke.WM_KILLFOCUS:
                if (postCharMessage)
                {
                    // Now that we've actually lost focus, post this message to the queue. This allows any activity
                    // that's in the queue to settle down before our characters are posted to the queue.
                    //
                    // We post because we need to allow the focus to actually happen before we send our strokes so we
                    // know where to send them.
                    //
                    // We can't use the wParam here because it may not be the actual window that needs to pick up
                    // the strokes.
                    PInvoke.PostMessage(target, (MessageId)WM_PRIVATE_POSTCHAR);
                    postCharMessage = false;
                }

                break;
        }

        oldTarget?.OnMessage(ref m);
    }
}
