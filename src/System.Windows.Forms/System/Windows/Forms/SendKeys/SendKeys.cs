// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace System.Windows.Forms;

/// <summary>
///  Provides methods for sending keystrokes to an application.
/// </summary>
public partial class SendKeys
{
    // It is unclear what significance the value 10 has, but it seems to make sense to make this a constant rather
    // than have 10 sprinkled throughout the code. It appears to be a sentinel value of some sort - indicating an
    // unknown grouping level.
    private const int UnknownGrouping = 10;

    private static readonly KeywordVk[] s_keywords = [
        new("ENTER",      Keys.Return),
        new("TAB",        Keys.Tab),
        new("ESC",        Keys.Escape),
        new("ESCAPE",     Keys.Escape),
        new("HOME",       Keys.Home),
        new("END",        Keys.End),
        new("LEFT",       Keys.Left),
        new("RIGHT",      Keys.Right),
        new("UP",         Keys.Up),
        new("DOWN",       Keys.Down),
        new("PGUP",       Keys.Prior),
        new("PGDN",       Keys.Next),
        new("NUMLOCK",    Keys.NumLock),
        new("SCROLLLOCK", Keys.Scroll),
        new("PRTSC",      Keys.PrintScreen),
        new("BREAK",      Keys.Cancel),
        new("BACKSPACE",  Keys.Back),
        new("BKSP",       Keys.Back),
        new("BS",         Keys.Back),
        new("CLEAR",      Keys.Clear),
        new("CAPSLOCK",   Keys.Capital),
        new("INS",        Keys.Insert),
        new("INSERT",     Keys.Insert),
        new("DEL",        Keys.Delete),
        new("DELETE",     Keys.Delete),
        new("HELP",       Keys.Help),
        new("F1",         Keys.F1),
        new("F2",         Keys.F2),
        new("F3",         Keys.F3),
        new("F4",         Keys.F4),
        new("F5",         Keys.F5),
        new("F6",         Keys.F6),
        new("F7",         Keys.F7),
        new("F8",         Keys.F8),
        new("F9",         Keys.F9),
        new("F10",        Keys.F10),
        new("F11",        Keys.F11),
        new("F12",        Keys.F12),
        new("F13",        Keys.F13),
        new("F14",        Keys.F14),
        new("F15",        Keys.F15),
        new("F16",        Keys.F16),
        new("MULTIPLY",   Keys.Multiply),
        new("ADD",        Keys.Add),
        new("SUBTRACT",   Keys.Subtract),
        new("DIVIDE",     Keys.Divide),
        new("+",          Keys.Add),
        new("%",          (Keys.D5 | Keys.Shift)),
        new("^",          (Keys.D6 | Keys.Shift)),
    ];

    // For use with VkKeyScanW()
    private const int ShiftKeyPressed = 0x0100;
    private const int CtrlKeyPressed = 0x0200;
    private const int AltKeyPressed = 0x0400;

    private static bool s_stopHook;
    private static HHOOK s_hhook;

    /// <summary>
    ///  Vector of events that we have yet to post to the journaling hook.
    /// </summary>
    private static readonly Queue<SKEvent> s_events = new();

    private static readonly Lock s_lock = new();
    private static bool s_startNewChar;
    private static readonly SKWindow s_messageWindow;

    private static SendMethodTypes? s_sendMethod;
    private static bool? s_hookSupported;

    // Used only for SendInput because SendInput alters the global state of the keyboard
    private static bool s_capslockChanged;
    private static bool s_numlockChanged;
    private static bool s_scrollLockChanged;
    private static bool s_kanaChanged;

    static SendKeys()
    {
        Application.ThreadExit += OnThreadExit;
        s_messageWindow = new SKWindow();
        s_messageWindow.CreateControl();
    }

    /// <summary>
    ///  Private constructor to prevent people from creating one of these. They should use public static methods.
    /// </summary>
    private SendKeys()
    {
    }

    /// <summary>
    ///  Adds an event to our list of events for the hook.
    /// </summary>
    private static void AddEvent(SKEvent skevent)
    {
        s_events.Enqueue(skevent);
    }

    /// <summary>
    ///  Helper function for ParseKeys for doing simple, self-describing characters.
    /// </summary>
    private static bool AddSimpleKey(
        char character,
        int repeat,
        HWND hwnd,
        (int HaveShift, int HaveCtrl, int HaveAlt) haveKeys,
        bool startNewChar,
        int cGrp)
    {
        int vk = PInvoke.VkKeyScan(character);

        if (vk != -1)
        {
            if (haveKeys.HaveShift == 0 && (vk & ShiftKeyPressed) != 0)
            {
                AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.ShiftKey, startNewChar, hwnd));
                startNewChar = false;
                haveKeys.HaveShift = UnknownGrouping;
            }

            if (haveKeys.HaveCtrl == 0 && (vk & CtrlKeyPressed) != 0)
            {
                AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.ControlKey, startNewChar, hwnd));
                startNewChar = false;
                haveKeys.HaveCtrl = UnknownGrouping;
            }

            if (haveKeys.HaveAlt == 0 && (vk & AltKeyPressed) != 0)
            {
                AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.Menu, startNewChar, hwnd));
                startNewChar = false;
                haveKeys.HaveAlt = UnknownGrouping;
            }

            AddMsgsForVK(vk & 0xff, repeat, haveKeys.HaveAlt > 0 && haveKeys.HaveCtrl == 0, hwnd);
            CancelMods(ref haveKeys, UnknownGrouping, hwnd);
        }
        else
        {
            uint oemVal = PInvoke.OemKeyScan((ushort)(0xFF & character));
            for (int i = 0; i < repeat; i++)
            {
                AddEvent(new SKEvent(PInvokeCore.WM_CHAR, character, (oemVal & 0xFFFF), hwnd));
            }
        }

        if (cGrp != 0)
        {
            startNewChar = true;
        }

        return startNewChar;
    }

    /// <summary>
    ///  Given the vk, add the appropriate messages for it.
    /// </summary>
    private static void AddMsgsForVK(int vk, int repeat, bool altnoctrldown, HWND hwnd)
    {
        for (int i = 0; i < repeat; i++)
        {
            AddEvent(new SKEvent(altnoctrldown ? PInvokeCore.WM_SYSKEYDOWN : PInvokeCore.WM_KEYDOWN, (uint)vk, s_startNewChar, hwnd));
            AddEvent(new SKEvent(altnoctrldown ? PInvokeCore.WM_SYSKEYUP : PInvokeCore.WM_KEYUP, (uint)vk, s_startNewChar, hwnd));
        }
    }

    /// <summary>
    ///  Called whenever there is a closing parenthesis, or the end of a character. This generates events for the
    ///  end of a modifier.
    /// </summary>
    private static void CancelMods(ref (int HaveShift, int HaveCtrl, int HaveAlt) haveKeys, int level, HWND hwnd)
    {
        if (haveKeys.HaveShift == level)
        {
            AddEvent(new SKEvent(PInvokeCore.WM_KEYUP, (int)Keys.ShiftKey, false, hwnd));
            haveKeys.HaveShift = 0;
        }

        if (haveKeys.HaveCtrl == level)
        {
            AddEvent(new SKEvent(PInvokeCore.WM_KEYUP, (int)Keys.ControlKey, false, hwnd));
            haveKeys.HaveCtrl = 0;
        }

        if (haveKeys.HaveAlt == level)
        {
            AddEvent(new SKEvent(PInvokeCore.WM_SYSKEYUP, (int)Keys.Menu, false, hwnd));
            haveKeys.HaveAlt = 0;
        }
    }

    /// <summary>
    ///  Install the hook.
    /// </summary>
    private static unsafe void InstallHook()
    {
        if (s_hhook.IsNull)
        {
            s_stopHook = false;
            s_hhook = PInvoke.SetWindowsHookEx(
                WINDOWS_HOOK_ID.WH_JOURNALPLAYBACK,
                &SendKeysHookProc.Callback,
                PInvoke.GetModuleHandle((PCWSTR)null),
                0);

            if (s_hhook.IsNull)
            {
                throw new SecurityException(SR.SendKeysHookFailed);
            }
        }
    }

    private static unsafe void TestHook()
    {
        s_hookSupported = false;
        try
        {
            HHOOK hookHandle = PInvoke.SetWindowsHookEx(
                WINDOWS_HOOK_ID.WH_JOURNALPLAYBACK,
                &EmptyHookCallback,
                PInvoke.GetModuleHandle((PCWSTR)null),
                0);

            s_hookSupported = !hookHandle.IsNull;

            if (!hookHandle.IsNull)
            {
                PInvoke.UnhookWindowsHookEx(hookHandle);
            }
        }
        catch
        {
            // Ignore any exceptions to keep existing SendKeys behavior
        }
    }

#pragma warning disable CS3016 // Arrays as attribute arguments is not CLS-compliant
    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
#pragma warning restore CS3016
    private static unsafe LRESULT EmptyHookCallback(int nCode, WPARAM wparam, LPARAM lparam) => (LRESULT)0;

    private static void LoadSendMethodFromConfig()
    {
        if (!s_sendMethod.HasValue)
        {
            s_sendMethod = SendMethodTypes.Default;

            try
            {
                // Read SendKeys value from config file, not case sensitive.
                string? value = Configuration.ConfigurationManager.AppSettings.Get("SendKeys");

                if (string.IsNullOrEmpty(value))
                {
                    return;
                }

                if (value.Equals("JournalHook", StringComparison.OrdinalIgnoreCase))
                {
                    s_sendMethod = SendMethodTypes.JournalHook;
                }
                else if (value.Equals("SendInput", StringComparison.OrdinalIgnoreCase))
                {
                    s_sendMethod = SendMethodTypes.SendInput;
                }
            }
            catch { } // ignore any exceptions to keep existing SendKeys behavior
        }
    }

    /// <summary>
    ///  Tells us to shut down the server, perhaps if we're shutting down and the hook is still running
    /// </summary>
    private static void JournalCancel()
    {
        if (!s_hhook.IsNull)
        {
            s_stopHook = false;
            s_events.Clear();

            s_hhook = default;
        }
    }

    private static unsafe void GetKeyboardState(Span<byte> keystate)
    {
        if (keystate.Length < 256)
            throw new InvalidOperationException();

        fixed (byte* b = keystate)
        {
            PInvoke.GetKeyboardState(b);
        }
    }

    private static unsafe void SetKeyboardState(ReadOnlySpan<byte> keystate)
    {
        if (keystate.Length < 256)
            throw new InvalidOperationException();

        fixed (byte* b = keystate)
        {
            PInvoke.SetKeyboardState(b);
        }
    }

    /// <summary>
    ///  Before we do a sendkeys, we want to clear the state of a couple of keys [capslock, numlock, scrolllock]
    ///  so they don't interfere.
    /// </summary>
    private static void ClearKeyboardState()
    {
        Span<byte> keystate = stackalloc byte[256];
        GetKeyboardState(keystate);
        keystate[(int)Keys.Capital] = 0;
        keystate[(int)Keys.NumLock] = 0;
        keystate[(int)Keys.Scroll] = 0;
        SetKeyboardState(keystate);
    }

    /// <summary>
    ///  Given the string, match the keyword to a VK. Return -1 if we don't match anything.
    /// </summary>
    private static int MatchKeyword(string keyword)
    {
        for (int i = 0; i < s_keywords.Length; i++)
        {
            if (string.Equals(s_keywords[i].Keyword, keyword, StringComparison.OrdinalIgnoreCase))
            {
                return s_keywords[i].VK;
            }
        }

        return -1;
    }

    /// <summary>
    ///  This event is raised from Application when each window thread terminates. It gives us a chance to
    ///  uninstall our journal hook if we had one installed.
    /// </summary>
    private static void OnThreadExit(object? sender, EventArgs e)
    {
        try
        {
            UninstallJournalingHook();
        }
        catch
        {
        }
    }

    /// <summary>
    ///  Parse the string the user has given us, and generate the appropriate events for the journaling hook.
    /// </summary>
    private static void ParseKeys(string keys, HWND hwnd)
    {
        int i = 0;

        // These four variables are used for grouping
        (int HaveShift, int HaveCtrl, int HaveAlt) haveKeys = default;
        int cGrp = 0;

        // startNewChar indicates that the next msg will be the first of a char or char group. This is needed for
        // IntraApp Nesting of SendKeys.

        s_startNewChar = true;

        // Start walking through the characters one at a time.

        int keysLen = keys.Length;
        while (i < keysLen)
        {
            int repeat = 1;
            char ch = keys[i];
            int vk;

            switch (ch)
            {
                case '}':
                    // If these appear at this point they are out of context, so return an error. KeyStart
                    // processes ochKeys up to the appropriate KeyEnd.
                    throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));

                case '{':
                    int j = i + 1;

                    // There's a unique class of strings of the form "{} n}" where n is an integer - in this case
                    // we want to send n copies of the '}' character. Here we test for the possibility of this
                    // class of problems, and skip the first '}' in the string if necessary.
                    if (j + 1 < keysLen && keys[j] == '}')
                    {
                        // Scan for the final '}' character
                        int final = j + 1;
                        while (final < keysLen && keys[final] != '}')
                        {
                            final++;
                        }

                        if (final < keysLen)
                        {
                            // Found the special case, so skip the first '}' in the string. The remainder of the
                            // code will attempt to find the repeat count.
                            j++;
                        }
                    }

                    // We're in a {<KEYWORD>...} situation. Look for the keyword.
                    while (j < keysLen && keys[j] != '}'
                           && !char.IsWhiteSpace(keys[j]))
                    {
                        j++;
                    }

                    if (j >= keysLen)
                    {
                        throw new ArgumentException(SR.SendKeysKeywordDelimError);
                    }

                    // Have our KEYWORD. Verify it's one we know about.
                    string keyName = keys[(i + 1)..j];

                    // See if we have a space, which would mean a repeat count.
                    if (char.IsWhiteSpace(keys[j]))
                    {
                        int digit;
                        while (j < keysLen && char.IsWhiteSpace(keys[j]))
                        {
                            j++;
                        }

                        if (j >= keysLen)
                        {
                            throw new ArgumentException(SR.SendKeysKeywordDelimError);
                        }

                        if (char.IsDigit(keys[j]))
                        {
                            digit = j;
                            while (j < keysLen && char.IsDigit(keys[j]))
                            {
                                j++;
                            }

                            repeat = int.Parse(keys.AsSpan(digit, j - digit), CultureInfo.InvariantCulture);
                        }
                    }

                    if (j >= keysLen)
                    {
                        throw new ArgumentException(SR.SendKeysKeywordDelimError);
                    }

                    if (keys[j] != '}')
                    {
                        throw new ArgumentException(SR.InvalidSendKeysRepeat);
                    }

                    vk = MatchKeyword(keyName);
                    if (vk != -1)
                    {
                        // Unlike AddSimpleKey, the bit mask uses Keys, rather than scan keys
                        if (haveKeys.HaveShift == 0 && (vk & (int)Keys.Shift) != 0)
                        {
                            AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.ShiftKey, s_startNewChar, hwnd));
                            s_startNewChar = false;
                            haveKeys.HaveShift = UnknownGrouping;
                        }

                        if (haveKeys.HaveCtrl == 0 && (vk & (int)Keys.Control) != 0)
                        {
                            AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.ControlKey, s_startNewChar, hwnd));
                            s_startNewChar = false;
                            haveKeys.HaveCtrl = UnknownGrouping;
                        }

                        if (haveKeys.HaveAlt == 0 && (vk & (int)Keys.Alt) != 0)
                        {
                            AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.Menu, s_startNewChar, hwnd));
                            s_startNewChar = false;
                            haveKeys.HaveAlt = UnknownGrouping;
                        }

                        AddMsgsForVK(vk, repeat, haveKeys.HaveAlt > 0 && haveKeys.HaveCtrl == 0, hwnd);
                        CancelMods(ref haveKeys, UnknownGrouping, hwnd);
                    }
                    else if (keyName.Length == 1)
                    {
                        s_startNewChar = AddSimpleKey(keyName[0], repeat, hwnd, haveKeys, s_startNewChar, cGrp);
                    }
                    else
                    {
                        throw new ArgumentException(string.Format(SR.InvalidSendKeysKeyword, keys[(i + 1)..j]));
                    }

                    // don't forget to position ourselves at the end of the {...} group
                    i = j;
                    break;

                case '+':
                    if (haveKeys.HaveShift != 0)
                    {
                        throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                    }

                    AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.ShiftKey, s_startNewChar, hwnd));
                    s_startNewChar = false;
                    haveKeys.HaveShift = UnknownGrouping;
                    break;

                case '^':
                    if (haveKeys.HaveCtrl != 0)
                    {
                        throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                    }

                    AddEvent(new SKEvent(PInvokeCore.WM_KEYDOWN, (uint)Keys.ControlKey, s_startNewChar, hwnd));
                    s_startNewChar = false;
                    haveKeys.HaveCtrl = UnknownGrouping;
                    break;

                case '%':
                    if (haveKeys.HaveAlt != 0)
                    {
                        throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                    }

                    AddEvent(new SKEvent(
                        haveKeys.HaveCtrl != 0 ? PInvokeCore.WM_KEYDOWN : PInvokeCore.WM_SYSKEYDOWN,
                        (int)Keys.Menu,
                        s_startNewChar,
                        hwnd));

                    s_startNewChar = false;
                    haveKeys.HaveAlt = UnknownGrouping;
                    break;

                case '(':
                    // Convert all immediate mode states to group mode. Allows multiple keys with the same shift,
                    // etc. state. Nests three deep.
                    cGrp++;
                    if (cGrp > 3)
                    {
                        throw new ArgumentException(SR.SendKeysNestingError);
                    }

                    if (haveKeys.HaveShift == UnknownGrouping)
                    {
                        haveKeys.HaveShift = cGrp;
                    }

                    if (haveKeys.HaveCtrl == UnknownGrouping)
                    {
                        haveKeys.HaveCtrl = cGrp;
                    }

                    if (haveKeys.HaveAlt == UnknownGrouping)
                    {
                        haveKeys.HaveAlt = cGrp;
                    }

                    break;

                case ')':
                    if (cGrp < 1)
                    {
                        throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                    }

                    CancelMods(ref haveKeys, cGrp, hwnd);
                    cGrp--;
                    if (cGrp == 0)
                    {
                        s_startNewChar = true;
                    }

                    break;

                case '~':
                    vk = (int)Keys.Return;
                    AddMsgsForVK(vk, repeat, haveKeys.HaveAlt > 0 && haveKeys.HaveCtrl == 0, hwnd);
                    break;

                default:
                    s_startNewChar = AddSimpleKey(keys[i], repeat, hwnd, haveKeys, s_startNewChar, cGrp);
                    break;
            }

            // Next element in the string.
            i++;
        }

        if (cGrp != 0)
        {
            throw new ArgumentException(SR.SendKeysGroupDelimError);
        }

        CancelMods(ref haveKeys, UnknownGrouping, hwnd);
    }

    /// <summary>
    ///  Uses User32 SendInput to send keystrokes.
    /// </summary>
    private static unsafe void SendInput(ReadOnlySpan<byte> oldKeyboardState, SKEvent[]? previousEvents)
    {
        // Should be a no-op most of the time
        AddCancelModifiersForPreviousEvents(previousEvents);

        // SKEvents are sent as 1 or 2 inputs:
        //
        //   currentInput[0] represents the SKEvent
        //   currentInput[1] is a KeyUp to prevent all identical WM_CHARs to be sent as one message
        Span<INPUT> currentInput = stackalloc INPUT[2];

        // All events are Keyboard events.
        currentInput[0].type = INPUT_TYPE.INPUT_KEYBOARD;
        currentInput[1].type = INPUT_TYPE.INPUT_KEYBOARD;

        // Set KeyUp values for currentInput[1].
        currentInput[1].Anonymous.ki.wVk = 0;
        currentInput[1].Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_UNICODE | KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;

        // Initialize unused members.
        currentInput[0].Anonymous.ki.dwExtraInfo = UIntPtr.Zero;
        currentInput[0].Anonymous.ki.time = 0;
        currentInput[1].Anonymous.ki.dwExtraInfo = UIntPtr.Zero;
        currentInput[1].Anonymous.ki.time = 0;

        // Send each of our SKEvents using SendInput.
        int INPUTSize = sizeof(INPUT);

        // Need these outside the lock below.
        uint eventsSent = 0;
        int eventsTotal;

        // A lock here will allow multiple threads to SendInput at the same time. This mimics the JournalHook
        // method of using the message loop to mitigate threading issues. There is still a theoretical thread
        // issue with adding to the events Queue (both JournalHook and SendInput), but we do not want to alter
        // the timings of the existing shipped behavior. Tested with 2 threads on a multiproc machine.
        lock (s_lock)
        {
            // Block keyboard and mouse input events from reaching applications.
            bool blockInputSuccess = PInvoke.BlockInput(true);

            try
            {
                eventsTotal = s_events.Count;
                ClearGlobalKeys();

                for (int i = 0; i < eventsTotal; i++)
                {
                    SKEvent skEvent = s_events.Dequeue();

                    currentInput[0].Anonymous.ki.dwFlags = 0;

                    if (skEvent.WM == PInvokeCore.WM_CHAR)
                    {
                        // For WM_CHAR, send a KEYEVENTF_UNICODE instead of a Keyboard event to support extended
                        // ASCII characters with no keyboard equivalent. Send currentInput[1] in this case.
                        currentInput[0].Anonymous.ki.wVk = 0;
                        currentInput[0].Anonymous.ki.wScan = (ushort)skEvent.ParamL;
                        currentInput[0].Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_UNICODE;
                        currentInput[1].Anonymous.ki.wScan = (ushort)skEvent.ParamL;

                        // Call SendInput, increment the eventsSent but subtract 1 for the extra one sent.
                        eventsSent += PInvoke.SendInput(currentInput, INPUTSize) - 1;
                    }
                    else
                    {
                        // Just need to send currentInput[0] for skEvent.
                        currentInput[0].Anonymous.ki.wScan = 0;

                        // Add KeyUp flag if we have a KeyUp.
                        if (skEvent.WM == PInvokeCore.WM_KEYUP || skEvent.WM == PInvokeCore.WM_SYSKEYUP)
                        {
                            currentInput[0].Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;
                        }

                        // Sets KEYEVENTF_EXTENDEDKEY flag if necessary.
                        if (IsExtendedKey(skEvent))
                        {
                            currentInput[0].Anonymous.ki.dwFlags |= KEYBD_EVENT_FLAGS.KEYEVENTF_EXTENDEDKEY;
                        }

                        currentInput[0].Anonymous.ki.wVk = (VIRTUAL_KEY)skEvent.ParamL;

                        // Send only currentInput[0].
                        eventsSent += PInvoke.SendInput(currentInput[..1], INPUTSize);

                        CheckGlobalKeys(skEvent);
                    }

                    // We need this slight delay here for Alt-Tab to work on Vista when the Aero theme is running.
                    // Although this does not look good, a delay here actually more closely resembles the old
                    // JournalHook that processes each event individually in the hook callback.
                    Thread.Sleep(1);
                }

                // Reset the keyboard back to what it was before inputs were sent, SendInput modifies the global
                // lights on the keyboard (caps, scroll..) so we need to call it again to undo those changes.
                ResetKeyboardUsingSendInput(INPUTSize);
            }
            finally
            {
                SetKeyboardState(oldKeyboardState);

                // Unblock input if it was previously blocked.
                if (blockInputSuccess)
                {
                    PInvoke.BlockInput(false);
                }
            }
        }

        // Check to see if we sent the number of events we're supposed to
        if (eventsSent != eventsTotal)
        {
            // Should try to move this up to where we fail out as the last error is likely to get overwritten.
            throw new Win32Exception();
        }
    }

    /// <summary>
    ///  For SendInput, these previous events that stick around if an Exception was thrown could modify the state
    ///  of the keyboard modifiers (alt, ctrl, shift). We must send a KeyUp for those, JournalHook doesn't
    ///  permanently set the state so it's ok.
    /// </summary>
    private static void AddCancelModifiersForPreviousEvents(SKEvent[]? previousEvents)
    {
        if (previousEvents is null)
        {
            return;
        }

        bool shift = false;
        bool ctrl = false;
        bool alt = false;

        foreach (SKEvent skEvent in previousEvents)
        {
            bool isOn;
            if ((skEvent.WM == PInvokeCore.WM_KEYUP) || (skEvent.WM == PInvokeCore.WM_SYSKEYUP))
            {
                isOn = false;
            }
            else if ((skEvent.WM == PInvokeCore.WM_KEYDOWN) || (skEvent.WM == PInvokeCore.WM_SYSKEYDOWN))
            {
                isOn = true;
            }
            else
            {
                continue;
            }

            if (skEvent.ParamL == (int)Keys.ShiftKey)
            {
                shift = isOn;
            }
            else if (skEvent.ParamL == (int)Keys.ControlKey)
            {
                ctrl = isOn;
            }
            else if (skEvent.ParamL == (int)Keys.Menu)
            {
                alt = isOn;
            }
        }

        if (shift)
        {
            AddEvent(new SKEvent(PInvokeCore.WM_KEYUP, (int)Keys.ShiftKey, false, default));
        }
        else if (ctrl)
        {
            AddEvent(new SKEvent(PInvokeCore.WM_KEYUP, (int)Keys.ControlKey, false, default));
        }
        else if (alt)
        {
            AddEvent(new SKEvent(PInvokeCore.WM_SYSKEYUP, (int)Keys.Menu, false, default));
        }
    }

    private static bool IsExtendedKey(SKEvent skEvent)
    {
        return (VIRTUAL_KEY)skEvent.ParamL is VIRTUAL_KEY.VK_UP
            or VIRTUAL_KEY.VK_DOWN
            or VIRTUAL_KEY.VK_LEFT
            or VIRTUAL_KEY.VK_RIGHT
            or VIRTUAL_KEY.VK_PRIOR
            or VIRTUAL_KEY.VK_NEXT
            or VIRTUAL_KEY.VK_HOME
            or VIRTUAL_KEY.VK_END
            or VIRTUAL_KEY.VK_INSERT
            or VIRTUAL_KEY.VK_DELETE;
    }

    private static void ClearGlobalKeys()
    {
        s_capslockChanged = false;
        s_numlockChanged = false;
        s_scrollLockChanged = false;
        s_kanaChanged = false;
    }

    private static void CheckGlobalKeys(SKEvent skEvent)
    {
        if (skEvent.WM == PInvokeCore.WM_KEYDOWN)
        {
            switch (skEvent.ParamL)
            {
                case (int)Keys.CapsLock:
                    s_capslockChanged = !s_capslockChanged;
                    break;
                case (int)Keys.NumLock:
                    s_numlockChanged = !s_numlockChanged;
                    break;
                case (int)Keys.Scroll:
                    s_scrollLockChanged = !s_scrollLockChanged;
                    break;
                case (int)Keys.KanaMode:
                    s_kanaChanged = !s_kanaChanged;
                    break;
            }
        }
    }

    private static void ResetKeyboardUsingSendInput(int INPUTSize)
    {
        // If the new state is the same, we don't need to do anything.
        if (!(s_capslockChanged || s_numlockChanged || s_scrollLockChanged || s_kanaChanged))
        {
            return;
        }

        // INPUT struct for resetting the keyboard.
        Span<INPUT> keyboardInput = stackalloc INPUT[2];

        keyboardInput[0].type = INPUT_TYPE.INPUT_KEYBOARD;
        keyboardInput[0].Anonymous.ki.dwFlags = 0;

        keyboardInput[1].type = INPUT_TYPE.INPUT_KEYBOARD;
        keyboardInput[1].Anonymous.ki.dwFlags = KEYBD_EVENT_FLAGS.KEYEVENTF_KEYUP;

        // SendInputs to turn on or off these keys. Inputs are pairs because KeyUp is sent for each one.
        if (s_capslockChanged)
        {
            keyboardInput[0].Anonymous.ki.wVk = VIRTUAL_KEY.VK_CAPITAL;
            keyboardInput[1].Anonymous.ki.wVk = VIRTUAL_KEY.VK_CAPITAL;
            PInvoke.SendInput(keyboardInput, INPUTSize);
        }

        if (s_numlockChanged)
        {
            keyboardInput[0].Anonymous.ki.wVk = VIRTUAL_KEY.VK_NUMLOCK;
            keyboardInput[1].Anonymous.ki.wVk = VIRTUAL_KEY.VK_NUMLOCK;
            PInvoke.SendInput(keyboardInput, INPUTSize);
        }

        if (s_scrollLockChanged)
        {
            keyboardInput[0].Anonymous.ki.wVk = VIRTUAL_KEY.VK_SCROLL;
            keyboardInput[1].Anonymous.ki.wVk = VIRTUAL_KEY.VK_SCROLL;
            PInvoke.SendInput(keyboardInput, INPUTSize);
        }

        if (s_kanaChanged)
        {
            keyboardInput[0].Anonymous.ki.wVk = VIRTUAL_KEY.VK_KANA;
            keyboardInput[1].Anonymous.ki.wVk = VIRTUAL_KEY.VK_KANA;
            PInvoke.SendInput(keyboardInput, INPUTSize);
        }
    }

    /// <summary>
    ///  Sends keystrokes to the active application.
    /// </summary>
    public static void Send(string keys) => Send(keys, null, false);

    private static void Send(string keys, Control? control, bool wait)
    {
        if (string.IsNullOrEmpty(keys))
        {
            return;
        }

        // If we're not going to wait, make sure there is a pump.
        if (!wait && !Application.MessageLoop)
        {
            throw new InvalidOperationException(SR.SendKeysNoMessageLoop);
        }

        // For SendInput only, see AddCancelModifiersForPreviousEvents for details.
        SKEvent[]? previousEvents = null;
        if (s_events.Count != 0)
        {
            previousEvents = [.. s_events];
        }

        // Generate the list of events that we're going to fire off with the hook.
        ParseKeys(keys, (control is not null) ? (HWND)control.Handle : default);

        // If there weren't any events posted as a result, we're done!
        if (s_events.Count == 0)
        {
            return;
        }

        LoadSendMethodFromConfig();

        Span<byte> oldState = stackalloc byte[256];
        GetKeyboardState(oldState);

        if (s_sendMethod!.Value != SendMethodTypes.SendInput)
        {
            if (!s_hookSupported.HasValue && s_sendMethod.Value == SendMethodTypes.Default)
            {
                // We don't know if the JournalHook will work, test it out so we know whether or not to call
                // ClearKeyboardState. ClearKeyboardState does nothing for JournalHooks but inversely affects
                // SendInput.
                TestHook();
            }

            if (s_sendMethod.Value == SendMethodTypes.JournalHook || s_hookSupported!.Value)
            {
                ClearKeyboardState();
                InstallHook();
                SetKeyboardState(oldState);
            }
        }

        if (s_sendMethod.Value == SendMethodTypes.SendInput ||
            (s_sendMethod.Value == SendMethodTypes.Default && !s_hookSupported!.Value))
        {
            // Either SendInput is configured or JournalHooks failed by default, call SendInput
            SendInput(oldState, previousEvents);
        }

        if (wait)
        {
            Flush();
        }
    }

    /// <summary>
    ///  Sends the given keys to the active application, and then waits for  the messages to be processed.
    /// </summary>
    public static void SendWait(string keys) => SendWait(keys, null);

    /// <summary>
    ///  Sends the given keys to the active application, and then waits for the messages to be processed.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   WARNING: this method will never work if control is not null, because while Windows journaling *looks* like it
    ///   can be directed to a specific HWND, it can't.
    ///  </para>
    /// </remarks>
    private static void SendWait(string keys, Control? control)
    {
        Send(keys, control, true);
    }

    /// <summary>
    ///  Processes all the Windows messages currently in the message queue.
    /// </summary>
    public static void Flush()
    {
        Application.DoEvents();
        while (s_events.Count > 0)
        {
            Application.DoEvents();
        }
    }

    /// <summary>
    ///  Cleans up and uninstalls the hook.
    /// </summary>
    private static void UninstallJournalingHook()
    {
        if (!s_hhook.IsNull)
        {
            s_stopHook = false;
            s_events.Clear();

            PInvoke.UnhookWindowsHookEx(s_hhook);
            s_hhook = default;
        }
    }
}
