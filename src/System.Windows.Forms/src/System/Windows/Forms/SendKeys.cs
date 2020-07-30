// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Security;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides methods for sending keystrokes to an application.
    /// </summary>
    public class SendKeys
    {
        // It is unclear what significance the value 10 has, but it seems to make sense to make this a constant rather
        // than have 10 sprinkled throughout the code. It appears to be a sentinel value of some sort - indicating an
        // unknown grouping level.
        private const int UnknownGrouping = 10;

        private static readonly KeywordVk[] s_keywords = new KeywordVk[]
        {
            new KeywordVk("ENTER",      Keys.Return),
            new KeywordVk("TAB",        Keys.Tab),
            new KeywordVk("ESC",        Keys.Escape),
            new KeywordVk("ESCAPE",     Keys.Escape),
            new KeywordVk("HOME",       Keys.Home),
            new KeywordVk("END",        Keys.End),
            new KeywordVk("LEFT",       Keys.Left),
            new KeywordVk("RIGHT",      Keys.Right),
            new KeywordVk("UP",         Keys.Up),
            new KeywordVk("DOWN",       Keys.Down),
            new KeywordVk("PGUP",       Keys.Prior),
            new KeywordVk("PGDN",       Keys.Next),
            new KeywordVk("NUMLOCK",    Keys.NumLock),
            new KeywordVk("SCROLLLOCK", Keys.Scroll),
            new KeywordVk("PRTSC",      Keys.PrintScreen),
            new KeywordVk("BREAK",      Keys.Cancel),
            new KeywordVk("BACKSPACE",  Keys.Back),
            new KeywordVk("BKSP",       Keys.Back),
            new KeywordVk("BS",         Keys.Back),
            new KeywordVk("CLEAR",      Keys.Clear),
            new KeywordVk("CAPSLOCK",   Keys.Capital),
            new KeywordVk("INS",        Keys.Insert),
            new KeywordVk("INSERT",     Keys.Insert),
            new KeywordVk("DEL",        Keys.Delete),
            new KeywordVk("DELETE",     Keys.Delete),
            new KeywordVk("HELP",       Keys.Help),
            new KeywordVk("F1",         Keys.F1),
            new KeywordVk("F2",         Keys.F2),
            new KeywordVk("F3",         Keys.F3),
            new KeywordVk("F4",         Keys.F4),
            new KeywordVk("F5",         Keys.F5),
            new KeywordVk("F6",         Keys.F6),
            new KeywordVk("F7",         Keys.F7),
            new KeywordVk("F8",         Keys.F8),
            new KeywordVk("F9",         Keys.F9),
            new KeywordVk("F10",        Keys.F10),
            new KeywordVk("F11",        Keys.F11),
            new KeywordVk("F12",        Keys.F12),
            new KeywordVk("F13",        Keys.F13),
            new KeywordVk("F14",        Keys.F14),
            new KeywordVk("F15",        Keys.F15),
            new KeywordVk("F16",        Keys.F16),
            new KeywordVk("MULTIPLY",   Keys.Multiply),
            new KeywordVk("ADD",        Keys.Add),
            new KeywordVk("SUBTRACT",   Keys.Subtract),
            new KeywordVk("DIVIDE",     Keys.Divide),
            new KeywordVk("+",          Keys.Add),
            new KeywordVk("%",          (Keys.D5 | Keys.Shift)),
            new KeywordVk("^",          (Keys.D6 | Keys.Shift)),
        };

        // For use with VkKeyScanW()
        private const int ShiftKeyPressed = 0x0100;
        private const int CtrlKeyPressed = 0x0200;
        private const int AltKeyPressed = 0x0400;

        private static bool s_stopHook;
        private static IntPtr s_hhook;
        private static User32.HOOKPROC s_hook;

        /// <summary>
        ///  Vector of events that we have yet to post to the journaling hook.
        /// </summary>
        private static Queue<SKEvent> s_events;

        private static object s_lock = new object();
        private static bool s_startNewChar;
        private static readonly SKWindow s_messageWindow;

        private enum SendMethodTypes
        {
            Default = 1,
            JournalHook = 2,
            SendInput = 3
        }

        private static SendMethodTypes? s_sendMethod;
        private static bool? s_hookSupported;

        // Used only for SendInput because SendInput alters the global state of the keyboard
        private static bool s_capslockChanged;
        private static bool s_numlockChanged;
        private static bool s_scrollLockChanged;
        private static bool s_kanaChanged;

#pragma warning disable CA1810 // Initialize reference type static fields inline (False positive: https://github.com/dotnet/roslyn-analyzers/issues/3852)
        static SendKeys()
#pragma warning restore CA1810 // Initialize reference type static fields inline
        {
            Application.ThreadExit += new EventHandler(OnThreadExit);
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
            s_events ??= new Queue<SKEvent>();
            s_events.Enqueue(skevent);
        }

        /// <summary>
        ///  Helper function for ParseKeys for doing simple, self-describing characters.
        /// </summary>
        private static bool AddSimpleKey(
            char character,
            int repeat,
            IntPtr hwnd,
            (int HaveShift, int HaveCtrl, int HaveAlt) haveKeys,
            bool startNewChar,
            int cGrp)
        {
            int vk = User32.VkKeyScanW(character);

            if (vk != -1)
            {
                if (haveKeys.HaveShift == 0 && (vk & ShiftKeyPressed) != 0)
                {
                    AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.ShiftKey, startNewChar, hwnd));
                    startNewChar = false;
                    haveKeys.HaveShift = UnknownGrouping;
                }

                if (haveKeys.HaveCtrl == 0 && (vk & CtrlKeyPressed) != 0)
                {
                    AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.ControlKey, startNewChar, hwnd));
                    startNewChar = false;
                    haveKeys.HaveCtrl = UnknownGrouping;
                }

                if (haveKeys.HaveAlt == 0 && (vk & AltKeyPressed) != 0)
                {
                    AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.Menu, startNewChar, hwnd));
                    startNewChar = false;
                    haveKeys.HaveAlt = UnknownGrouping;
                }

                AddMsgsForVK(vk & 0xff, repeat, haveKeys.HaveAlt > 0 && haveKeys.HaveCtrl == 0, hwnd);
                CancelMods(haveKeys, UnknownGrouping, hwnd);
            }
            else
            {
                uint oemVal = User32.OemKeyScan((ushort)(0xFF & character));
                for (int i = 0; i < repeat; i++)
                {
                    AddEvent(new SKEvent(User32.WM.CHAR, character, (oemVal & 0xFFFF), hwnd));
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
        private static void AddMsgsForVK(int vk, int repeat, bool altnoctrldown, IntPtr hwnd)
        {
            for (int i = 0; i < repeat; i++)
            {
                AddEvent(new SKEvent(altnoctrldown ? User32.WM.SYSKEYDOWN : User32.WM.KEYDOWN, (uint)vk, s_startNewChar, hwnd));
                AddEvent(new SKEvent(altnoctrldown ? User32.WM.SYSKEYUP : User32.WM.KEYUP, (uint)vk, s_startNewChar, hwnd));
            }
        }

        /// <summary>
        ///  Called whenever there is a closing parenthesis, or the end of a character. This generates events for the
        ///  end of a modifier.
        /// </summary>
        private static void CancelMods((int HaveShift, int HaveCtrl, int HaveAlt) haveKeys, int level, IntPtr hwnd)
        {
            if (haveKeys.HaveShift == level)
            {
                AddEvent(new SKEvent(User32.WM.KEYUP, (int)Keys.ShiftKey, false, hwnd));
                haveKeys.HaveShift = 0;
            }

            if (haveKeys.HaveCtrl == level)
            {
                AddEvent(new SKEvent(User32.WM.KEYUP, (int)Keys.ControlKey, false, hwnd));
                haveKeys.HaveCtrl = 0;
            }

            if (haveKeys.HaveAlt == level)
            {
                AddEvent(new SKEvent(User32.WM.SYSKEYUP, (int)Keys.Menu, false, hwnd));
                haveKeys.HaveAlt = 0;
            }
        }

        /// <summary>
        ///  Install the hook.
        /// </summary>
        private static void InstallHook()
        {
            if (s_hhook == IntPtr.Zero)
            {
                s_hook = new User32.HOOKPROC(new SendKeysHookProc().Callback);
                s_stopHook = false;
                s_hhook = User32.SetWindowsHookExW(
                    User32.WH.JOURNALPLAYBACK,
                    s_hook,
                    Kernel32.GetModuleHandleW(null),
                    0);

                if (s_hhook == IntPtr.Zero)
                {
                    throw new SecurityException(SR.SendKeysHookFailed);
                }
            }
        }

        private static void TestHook()
        {
            s_hookSupported = false;
            try
            {
                var hookProc = new User32.HOOKPROC(EmptyHookCallback);
                IntPtr hookHandle = User32.SetWindowsHookExW(
                    User32.WH.JOURNALPLAYBACK,
                    hookProc,
                    Kernel32.GetModuleHandleW(null),
                    0);
                s_hookSupported = (hookHandle != IntPtr.Zero);

                if (hookHandle != IntPtr.Zero)
                {
                    User32.UnhookWindowsHookEx(hookHandle);
                }
            }
            catch
            {
                // Ignore any exceptions to keep existing SendKeys behavior
            }
        }

        private static IntPtr EmptyHookCallback(User32.HC nCode, IntPtr wparam, IntPtr lparam) => IntPtr.Zero;

        private static void LoadSendMethodFromConfig()
        {
            if (!s_sendMethod.HasValue)
            {
                s_sendMethod = SendMethodTypes.Default;

                try
                {
                    // Read SendKeys value from config file, not case sensitive.
                    string value = Configuration.ConfigurationManager.AppSettings.Get("SendKeys");

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
            if (s_hhook != IntPtr.Zero)
            {
                s_stopHook = false;
                if (s_events != null)
                {
                    s_events.Clear();
                }
                s_hhook = IntPtr.Zero;
            }
        }

        private unsafe static void GetKeyboardState(Span<byte> keystate)
        {
            if (keystate.Length < 256)
                throw new InvalidOperationException();

            fixed (byte* b = keystate)
            {
                User32.GetKeyboardState(b);
            }
        }

        private unsafe static void SetKeyboardState(ReadOnlySpan<byte> keystate)
        {
            if (keystate.Length < 256)
                throw new InvalidOperationException();

            fixed (byte* b = keystate)
            {
                User32.SetKeyboardState(b);
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
        ///  This event is raised from Application when each window thread termiantes. It gives us a chance to
        ///  uninstall our journal hook if we had one installed.
        /// </summary>
        private static void OnThreadExit(object sender, EventArgs e)
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
        private static void ParseKeys(string keys, IntPtr hwnd)
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
                        string keyName = keys.Substring(i + 1, j - (i + 1));

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
                                repeat = int.Parse(keys.Substring(digit, j - digit), CultureInfo.InvariantCulture);
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
                                AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.ShiftKey, s_startNewChar, hwnd));
                                s_startNewChar = false;
                                haveKeys.HaveShift = UnknownGrouping;
                            }

                            if (haveKeys.HaveCtrl == 0 && (vk & (int)Keys.Control) != 0)
                            {
                                AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.ControlKey, s_startNewChar, hwnd));
                                s_startNewChar = false;
                                haveKeys.HaveCtrl = UnknownGrouping;
                            }

                            if (haveKeys.HaveAlt == 0 && (vk & (int)Keys.Alt) != 0)
                            {
                                AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.Menu, s_startNewChar, hwnd));
                                s_startNewChar = false;
                                haveKeys.HaveAlt = UnknownGrouping;
                            }

                            AddMsgsForVK(vk, repeat, haveKeys.HaveAlt > 0 && haveKeys.HaveCtrl == 0, hwnd);
                            CancelMods(haveKeys, UnknownGrouping, hwnd);
                        }
                        else if (keyName.Length == 1)
                        {
                            s_startNewChar = AddSimpleKey(keyName[0], repeat, hwnd, haveKeys, s_startNewChar, cGrp);
                        }
                        else
                        {
                            throw new ArgumentException(string.Format(SR.InvalidSendKeysKeyword, keys.Substring(i + 1, j - (i + 1))));
                        }

                        // don't forget to position ourselves at the end of the {...} group
                        i = j;
                        break;

                    case '+':
                        if (haveKeys.HaveShift != 0)
                        {
                            throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                        }

                        AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.ShiftKey, s_startNewChar, hwnd));
                        s_startNewChar = false;
                        haveKeys.HaveShift = UnknownGrouping;
                        break;

                    case '^':
                        if (haveKeys.HaveCtrl != 0)
                        {
                            throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                        }

                        AddEvent(new SKEvent(User32.WM.KEYDOWN, (uint)Keys.ControlKey, s_startNewChar, hwnd));
                        s_startNewChar = false;
                        haveKeys.HaveCtrl = UnknownGrouping;
                        break;

                    case '%':
                        if (haveKeys.HaveAlt != 0)
                        {
                            throw new ArgumentException(string.Format(SR.InvalidSendKeysString, keys));
                        }

                        AddEvent(new SKEvent(
                            haveKeys.HaveCtrl != 0 ? User32.WM.KEYDOWN : User32.WM.SYSKEYDOWN,
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

                        CancelMods(haveKeys, cGrp, hwnd);
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

            CancelMods(haveKeys, UnknownGrouping, hwnd);
        }

        /// <summary>
        ///  Uses User32 SendInput to send keystrokes.
        /// </summary>
        private unsafe static void SendInput(ReadOnlySpan<byte> oldKeyboardState, SKEvent[] previousEvents)
        {
            // Should be a no-op most of the time
            AddCancelModifiersForPreviousEvents(previousEvents);

            // SKEvents are sent as 1 or 2 inputs:
            //
            //   currentInput[0] represents the SKEvent
            //   currentInput[1] is a KeyUp to prevent all identical WM_CHARs to be sent as one message
            Span<User32.INPUT> currentInput = stackalloc User32.INPUT[2];

            // All events are Keyboard events.
            currentInput[0].type = User32.INPUTENUM.KEYBOARD;
            currentInput[1].type = User32.INPUTENUM.KEYBOARD;

            // Set KeyUp values for currentInput[1].
            currentInput[1].inputUnion.ki.wVk = 0;
            currentInput[1].inputUnion.ki.dwFlags = User32.KEYEVENTF.UNICODE | User32.KEYEVENTF.KEYUP;

            // Initialize unused members.
            currentInput[0].inputUnion.ki.dwExtraInfo = IntPtr.Zero;
            currentInput[0].inputUnion.ki.time = 0;
            currentInput[1].inputUnion.ki.dwExtraInfo = IntPtr.Zero;
            currentInput[1].inputUnion.ki.time = 0;

            // Send each of our SKEvents using SendInput.
            int INPUTSize = sizeof(User32.INPUT);

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
                BOOL blockInputSuccess = User32.BlockInput(BOOL.TRUE);

                try
                {
                    eventsTotal = s_events.Count;
                    ClearGlobalKeys();

                    for (int i = 0; i < eventsTotal; i++)
                    {
                        SKEvent skEvent = (SKEvent)s_events.Dequeue();

                        currentInput[0].inputUnion.ki.dwFlags = 0;

                        if (skEvent.WM == User32.WM.CHAR)
                        {
                            // For WM_CHAR, send a KEYEVENTF_UNICODE instead of a Keyboard event to support extended
                            // ASCII characters with no keyboard equivalent. Send currentInput[1] in this case.
                            currentInput[0].inputUnion.ki.wVk = 0;
                            currentInput[0].inputUnion.ki.wScan = (ushort)skEvent.ParamL;
                            currentInput[0].inputUnion.ki.dwFlags = User32.KEYEVENTF.UNICODE;
                            currentInput[1].inputUnion.ki.wScan = (ushort)skEvent.ParamL;

                            // Call SendInput, increment the eventsSent but subtract 1 for the extra one sent.
                            eventsSent += User32.SendInput(2, currentInput, INPUTSize) - 1;
                        }
                        else
                        {
                            // Just need to send currentInput[0] for skEvent.
                            currentInput[0].inputUnion.ki.wScan = 0;

                            // Add KeyUp flag if we have a KeyUp.
                            if (skEvent.WM == User32.WM.KEYUP || skEvent.WM == User32.WM.SYSKEYUP)
                            {
                                currentInput[0].inputUnion.ki.dwFlags |= User32.KEYEVENTF.KEYUP;
                            }

                            // Sets KEYEVENTF_EXTENDEDKEY flag if necessary.
                            if (IsExtendedKey(skEvent))
                            {
                                currentInput[0].inputUnion.ki.dwFlags |= User32.KEYEVENTF.EXTENDEDKEY;
                            }

                            currentInput[0].inputUnion.ki.wVk = (ushort)skEvent.ParamL;

                            // Send only currentInput[0].
                            eventsSent += User32.SendInput(1, currentInput, INPUTSize);

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
                    if (blockInputSuccess.IsTrue())
                    {
                        User32.BlockInput(BOOL.FALSE);
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
        private static void AddCancelModifiersForPreviousEvents(SKEvent[] previousEvents)
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
                if ((skEvent.WM == User32.WM.KEYUP) || (skEvent.WM == User32.WM.SYSKEYUP))
                {
                    isOn = false;
                }
                else if ((skEvent.WM == User32.WM.KEYDOWN) || (skEvent.WM == User32.WM.SYSKEYDOWN))
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
                AddEvent(new SKEvent(User32.WM.KEYUP, (int)Keys.ShiftKey, false, IntPtr.Zero));
            }
            else if (ctrl)
            {
                AddEvent(new SKEvent(User32.WM.KEYUP, (int)Keys.ControlKey, false, IntPtr.Zero));
            }
            else if (alt)
            {
                AddEvent(new SKEvent(User32.WM.SYSKEYUP, (int)Keys.Menu, false, IntPtr.Zero));
            }
        }

        private static bool IsExtendedKey(SKEvent skEvent)
        {
            return (skEvent.ParamL == User32.VK.UP)
                || (skEvent.ParamL == User32.VK.DOWN)
                || (skEvent.ParamL == User32.VK.LEFT)
                || (skEvent.ParamL == User32.VK.RIGHT)
                || (skEvent.ParamL == User32.VK.PRIOR)
                || (skEvent.ParamL == User32.VK.NEXT)
                || (skEvent.ParamL == User32.VK.HOME)
                || (skEvent.ParamL == User32.VK.END)
                || (skEvent.ParamL == User32.VK.INSERT)
                || (skEvent.ParamL == User32.VK.DELETE);
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
            if (skEvent.WM == User32.WM.KEYDOWN)
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
            Span<User32.INPUT> keyboardInput = stackalloc User32.INPUT[2];

            keyboardInput[0].type = User32.INPUTENUM.KEYBOARD;
            keyboardInput[0].inputUnion.ki.dwFlags = 0;

            keyboardInput[1].type = User32.INPUTENUM.KEYBOARD;
            keyboardInput[1].inputUnion.ki.dwFlags = User32.KEYEVENTF.KEYUP;

            // SendInputs to turn on or off these keys.  Inputs are pairs because KeyUp is sent for each one.
            if (s_capslockChanged)
            {
                keyboardInput[0].inputUnion.ki.wVk = User32.VK.CAPITAL;
                keyboardInput[1].inputUnion.ki.wVk = User32.VK.CAPITAL;
                User32.SendInput(2, keyboardInput, INPUTSize);
            }

            if (s_numlockChanged)
            {
                keyboardInput[0].inputUnion.ki.wVk = User32.VK.NUMLOCK;
                keyboardInput[1].inputUnion.ki.wVk = User32.VK.NUMLOCK;
                User32.SendInput(2, keyboardInput, INPUTSize);
            }

            if (s_scrollLockChanged)
            {
                keyboardInput[0].inputUnion.ki.wVk = User32.VK.SCROLL;
                keyboardInput[1].inputUnion.ki.wVk = User32.VK.SCROLL;
                User32.SendInput(2, keyboardInput, INPUTSize);
            }

            if (s_kanaChanged)
            {
                keyboardInput[0].inputUnion.ki.wVk = User32.VK.KANA;
                keyboardInput[1].inputUnion.ki.wVk = User32.VK.KANA;
                User32.SendInput(2, keyboardInput, INPUTSize);
            }
        }

        /// <summary>
        ///  Sends keystrokes to the active application.
        /// </summary>
        public static void Send(string keys) => Send(keys, null, false);

        private static void Send(string keys, Control control, bool wait)
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
            SKEvent[] previousEvents = null;
            if ((s_events != null) && (s_events.Count != 0))
            {
                previousEvents = s_events.ToArray();
            }

            // Generate the list of events that we're going to fire off with the hook.
            ParseKeys(keys, (control != null) ? control.Handle : IntPtr.Zero);

            // If there weren't any events posted as a result, we're done!
            if (s_events is null)
            {
                return;
            }

            LoadSendMethodFromConfig();

            Span<byte> oldState = stackalloc byte[256];
            GetKeyboardState(oldState);

            if (s_sendMethod.Value != SendMethodTypes.SendInput)
            {
                if (!s_hookSupported.HasValue && s_sendMethod.Value == SendMethodTypes.Default)
                {
                    // We don't know if the JournalHook will work, test it out so we know whether or not to call
                    // ClearKeyboardState. ClearKeyboardState does nothing for JournalHooks but inversely affects
                    // SendInput.
                    TestHook();
                }

                if (s_sendMethod.Value == SendMethodTypes.JournalHook || s_hookSupported.Value)
                {
                    ClearKeyboardState();
                    InstallHook();
                    SetKeyboardState(oldState);
                }
            }

            if (s_sendMethod.Value == SendMethodTypes.SendInput ||
                (s_sendMethod.Value == SendMethodTypes.Default && !s_hookSupported.Value))
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
        ///  WARNING: this method will never work if control != null, because while Windows journaling *looks* like it
        ///  can be directed to a specific HWND, it can't.
        /// </remarks>
        private static void SendWait(string keys, Control control)
        {
            Send(keys, control, true);
        }

        /// <summary>
        ///  Processes all the Windows messages currently in the message queue.
        /// </summary>
        public static void Flush()
        {
            Application.DoEvents();
            while (s_events != null && s_events.Count > 0)
            {
                Application.DoEvents();
            }
        }

        /// <summary>
        ///  Cleans up and uninstalls the hook.
        /// </summary>
        private static void UninstallJournalingHook()
        {
            if (s_hhook != IntPtr.Zero)
            {
                s_stopHook = false;
                s_events?.Clear();

                User32.UnhookWindowsHookEx(s_hhook);
                s_hhook = IntPtr.Zero;
            }
        }

        /// <summary>
        ///  SendKeys creates a window to monitor WM_CANCELJOURNAL messages.
        /// </summary>
        private class SKWindow : Control
        {
            public SKWindow()
            {
                SetState(States.TopLevel, true);
                SetExtendedState(ExtendedStates.InterestedInUserPreferenceChanged, false);
                SetBounds(-1, -1, 0, 0);
                Visible = false;
            }

            protected override void WndProc(ref Message m)
            {
                if (m.Msg == (int)User32.WM.CANCELJOURNAL)
                {
                    try
                    {
                        JournalCancel();
                    }
                    catch
                    {
                    }
                }
            }
        }

        /// <summary>
        ///  Helps us hold information about the various events we're going to journal.
        /// </summary>
        private readonly struct SKEvent
        {
            public readonly User32.WM WM;
            public readonly uint ParamL;
            public readonly uint ParamH;
            public readonly IntPtr HWND;

            public SKEvent(User32.WM wm, uint paramL, bool paramH, IntPtr hwnd)
            {
                WM = wm;
                ParamL = paramL;
                ParamH = paramH ? 1u : 0;
                HWND = hwnd;
            }

            public SKEvent(User32.WM wm, uint paramL, uint paramH, IntPtr hwnd)
            {
                WM = wm;
                ParamL = paramL;
                ParamH = paramH;
                HWND = hwnd;
            }
        }

        /// <summary>
        ///  Holds a keyword and the associated VK_ for it.
        /// </summary>
        private readonly struct KeywordVk
        {
            public readonly string Keyword;
            public readonly int VK;

            public KeywordVk(string keyword, Keys key)
            {
                Keyword = keyword;
                VK = (int)key;
            }
        }

        /// <summary>
        ///  This class is our callback for the journaling hook we install.
        /// </summary>
        private class SendKeysHookProc
        {
            // There appears to be a timing issue where setting and removing and then setting these hooks via
            // SetWindowsHookEx / UnhookWindowsHookEx can cause messages to be left in the queue and sent after the
            // re-hookup happens. This puts us in a bad state as we get an HC_SKIP before an HC_GETNEXT. So in that
            // case, we just ignore the HC_SKIP calls until we get an HC_GETNEXT. We also sleep a bit in the Unhook.

            private bool _gotNextEvent;

            public unsafe virtual IntPtr Callback(User32.HC nCode, IntPtr wparam, IntPtr lparam)
            {
                User32.EVENTMSG* eventmsg = (User32.EVENTMSG*)lparam;

                if (User32.GetAsyncKeyState((int)Keys.Pause) != 0)
                {
                    s_stopHook = true;
                }

                switch (nCode)
                {
                    case User32.HC.SKIP:
                        if (_gotNextEvent)
                        {
                            if (s_events != null && s_events.Count > 0)
                            {
                                s_events.Dequeue();
                            }
                            s_stopHook = s_events is null || s_events.Count == 0;
                            break;
                        }

                        break;
                    case User32.HC.GETNEXT:
                        _gotNextEvent = true;

                        Debug.Assert(
                            s_events != null && s_events.Count > 0 && !s_stopHook,
                            "HC_GETNEXT when queue is empty!");

                        SKEvent evt = (SKEvent)s_events.Peek();
                        eventmsg->message = evt.WM;
                        eventmsg->paramL = evt.ParamL;
                        eventmsg->paramH = evt.ParamH;
                        eventmsg->hwnd = evt.HWND;
                        eventmsg->time = Kernel32.GetTickCount();
                        break;
                    default:
                        if (nCode < 0)
                        {
                            User32.CallNextHookEx(s_hhook, nCode, wparam, lparam);
                        }

                        break;
                }

                if (s_stopHook)
                {
                    UninstallJournalingHook();
                    _gotNextEvent = false;
                }
                return IntPtr.Zero;
            }
        }
    }
}
