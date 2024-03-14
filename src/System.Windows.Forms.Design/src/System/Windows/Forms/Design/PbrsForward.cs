﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class PbrsForward : IWindowTarget
{
    private readonly Control _target;
    private readonly IWindowTarget _oldTarget;

    // We save the last key down so we can recreate the last message if we need to activate
    // the properties window.
    private Message _lastKeyDown;
    private List<BufferedKey> _bufferedChars;

    private const int WM_PRIVATE_POSTCHAR = (int)PInvoke.WM_USER + 0x1598;
    private bool _postCharMessage;

    private IMenuCommandService _menuCommandSvc;

    private readonly IServiceProvider _sp;

    private bool _ignoreMessages;

    public PbrsForward(Control target, IServiceProvider sp)
    {
        _target = target;
        _oldTarget = target.WindowTarget;
        _sp = sp;
        target.WindowTarget = this;
    }

    private IMenuCommandService MenuCommandService
    {
        get
        {
            if (_menuCommandSvc is null && _sp is not null)
            {
                _menuCommandSvc = (IMenuCommandService)_sp.GetService(typeof(IMenuCommandService));
            }

            return _menuCommandSvc;
        }
    }

    private ISupportInSituService InSituSupportService
        => (ISupportInSituService)_sp.GetService(typeof(ISupportInSituService));

    public void Dispose() => _target.WindowTarget = _oldTarget;

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
        _ignoreMessages = false;

        // Here lets query for the ISupportInSituService.
        // If we find the service then ask if it has a designer which is interested
        // in getting the keychars by querying the IgnoreMessages.
        if (m.Msg is >= ((int)PInvoke.WM_KEYFIRST)
            and <= ((int)PInvoke.WM_KEYLAST) or >= ((int)PInvoke.WM_IME_STARTCOMPOSITION)
            and <= ((int)PInvoke.WM_IME_COMPOSITION))
        {
            if (InSituSupportService is not null)
            {
                _ignoreMessages = InSituSupportService.IgnoreMessages;
            }
        }

        switch (m.Msg)
        {
            case WM_PRIVATE_POSTCHAR:

                if (_bufferedChars is null)
                {
                    return;
                }

                // recreate the keystroke to the newly activated window
                HWND hwnd;
                hwnd = !_ignoreMessages || InSituSupportService is null
                    ? PInvoke.GetFocus()
                    : (HWND)InSituSupportService.GetEditWindow();

                if (hwnd != m.HWnd)
                {
                    foreach (BufferedKey bk in _bufferedChars)
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

                _bufferedChars.Clear();
                return;

            case (int)PInvoke.WM_KEYDOWN:
                _lastKeyDown = m;
                break;

            case (int)PInvoke.WM_IME_ENDCOMPOSITION:
            case (int)PInvoke.WM_KEYUP:
                _lastKeyDown.Msg = 0;
                break;

            case (int)PInvoke.WM_CHAR:
            case (int)PInvoke.WM_IME_STARTCOMPOSITION:
            case (int)PInvoke.WM_IME_COMPOSITION:
                if ((Control.ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
                {
                    break;
                }

                _bufferedChars ??= [];
                _bufferedChars.Add(new BufferedKey(_lastKeyDown, m, _lastKeyDown));

                if (!_ignoreMessages && MenuCommandService is not null)
                {
                    // throw the properties window command, we will redo the keystroke when we actually
                    // lose focus
                    _postCharMessage = true;
                    MenuCommandService.GlobalInvoke(StandardCommands.PropertiesWindow);
                }
                else if (_ignoreMessages && m.Msg != (int)PInvoke.WM_IME_COMPOSITION)
                {
                    if (InSituSupportService is not null)
                    {
                        _postCharMessage = true;
                        InSituSupportService.HandleKeyChar();
                    }
                }

                if (_postCharMessage)
                {
                    // If copy of message has been buffered for forwarding, eat the original now
                    return;
                }

                break;

            case (int)PInvoke.WM_KILLFOCUS:
                if (_postCharMessage)
                {
                    // Now that we've actually lost focus, post this message to the queue. This allows any activity
                    // that's in the queue to settle down before our characters are posted to the queue.
                    //
                    // We post because we need to allow the focus to actually happen before we send our strokes so we
                    // know where to send them.
                    //
                    // We can't use the wParam here because it may not be the actual window that needs to pick up
                    // the strokes.
                    PInvoke.PostMessage(_target, (MessageId)WM_PRIVATE_POSTCHAR);
                    _postCharMessage = false;
                }

                break;
        }

        _oldTarget?.OnMessage(ref m);
    }
}
