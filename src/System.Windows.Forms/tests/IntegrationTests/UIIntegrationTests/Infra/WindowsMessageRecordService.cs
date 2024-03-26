// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.UI.WindowsAndMessaging;

namespace System.Windows.Forms.UITests;

internal class WindowsMessageRecordService : IDisposable, IMessageFilter
{
    private static readonly List<(uint message, string source, string? detail)> s_messages = [];
    private static ImmutableDictionary<uint, string> s_messageNames;

    private static WindowsMessageRecordService? s_currentInstance;
    private static HHOOK s_messageProcHook;
    private static HHOOK s_getMsgProcHook;
    private Form? _form;

    unsafe static WindowsMessageRecordService()
    {
        var messageNames = ImmutableDictionary.CreateBuilder<uint, string>();
        foreach (var field in typeof(PInvoke).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            if (!field.IsLiteral)
            {
                // This isn't a constant
                continue;
            }

            if (!field.Name.StartsWith("WM_", StringComparison.Ordinal))
                continue;

            if (field.Name is nameof(PInvoke.WM_KEYFIRST) or nameof(PInvoke.WM_MOUSEFIRST))
            {
                // Use the actual names of the messages instead of this alias
                continue;
            }

            try
            {
                if (field.GetRawConstantValue() is uint message)
                {
                    messageNames.TryAdd(message, field.Name);
                }
            }
            catch
            {
                // Ignore any cases where we can't get the constant value
            }
        }

        s_messageNames = messageNames.ToImmutable();

        DataCollectionService.RegisterCustomLogger(
            static fullPath =>
            {
                (uint message, string source, string? detail)[] messages;
                lock (s_messages)
                {
                    if (s_messages.Count == 0)
                    {
                        return;
                    }

                    messages = [.. s_messages];
                }

                var builder = new StringBuilder();
                foreach (var (message, source, detail) in messages)
                {
                    string messageName = ImmutableInterlocked.GetOrAdd(
                        ref s_messageNames,
                        message,
                        static message =>
                        {
                            // The max length of the name of clipboard formats is equal to the max length
                            // of a Win32 Atom of 255 chars. An additional null terminator character is added,
                            // giving a required capacity of 256 chars.
                            Span<char> formatName = stackalloc char[256];
                            fixed (char* pFormatName = formatName)
                            {
                                int length = PInvoke.GetClipboardFormatName(message, pFormatName, formatName.Length);
                                if (length != 0)
                                {
                                    return formatName[..length].ToString();
                                }
                            }

                            return message.ToString();
                        });

                    if (detail is not null)
                    {
                        builder.AppendLine($"{messageName} ({source}) ({detail})");
                    }
                    else
                    {
                        builder.AppendLine($"{messageName} ({source})");
                    }
                }

                File.WriteAllText(fullPath, builder.ToString(), new UTF8Encoding(true));
            },
            logId: "MSG",
            "log");
    }

    public WindowsMessageRecordService()
    {
        // Release the previous instance, if any
        s_currentInstance?.UnregisterEvents();
        s_currentInstance = this;

        lock (s_messages)
        {
            s_messages.Clear();
        }
    }

    public void Dispose()
    {
        UnregisterEvents();
        s_currentInstance = null;
    }

    public unsafe void RegisterEvents(Form form)
    {
        ArgumentNullException.ThrowIfNull(form);

        Assert.Null(_form);

        _form = form;

        lock (this)
        {
            if (!s_messageProcHook.IsNull || !s_getMsgProcHook.IsNull)
                throw new InvalidOperationException();

            Application.AddMessageFilter(this);

            s_messageProcHook = PInvoke.SetWindowsHookEx(
                WINDOWS_HOOK_ID.WH_MSGFILTER,
                &MessageProc,
                HINSTANCE.Null,
                PInvoke.GetCurrentThreadId());

            s_getMsgProcHook = PInvoke.SetWindowsHookEx(
                WINDOWS_HOOK_ID.WH_GETMESSAGE,
                &GetMsgProc,
                HINSTANCE.Null,
                PInvoke.GetCurrentThreadId());
        }
    }

    public void UnregisterEvents()
    {
        lock (this)
        {
            if (s_messageProcHook.IsNull || s_getMsgProcHook.IsNull)
                throw new InvalidOperationException();

            Application.RemoveMessageFilter(this);

            PInvoke.UnhookWindowsHookEx(s_messageProcHook);
            s_messageProcHook = HHOOK.Null;

            PInvoke.UnhookWindowsHookEx(s_getMsgProcHook);
            s_getMsgProcHook = HHOOK.Null;
        }
    }

    public unsafe bool PreFilterMessage(ref Message m)
    {
        var msg = m.ToMSG();
        s_messages.Add(((uint)m.Msg, nameof(PreFilterMessage), GetDetail(&msg)));

        return false;
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT MessageProc(int nCode, WPARAM wparam, LPARAM lparam)
    {
        if (nCode >= 0)
        {
            MSG* msg = (MSG*)(nint)lparam;
            if (msg is not null)
            {
                s_messages.Add((msg->message, nameof(MessageProc), GetDetail(msg)));
            }
        }

        return PInvoke.CallNextHookEx(s_messageProcHook, nCode, wparam, lparam);
    }

    [UnmanagedCallersOnly(CallConvs = [typeof(CallConvStdcall)])]
    private static unsafe LRESULT GetMsgProc(int nCode, WPARAM wparam, LPARAM lparam)
    {
        if (nCode == PInvoke.HC_ACTION)
        {
            MSG* msg = (MSG*)(nint)lparam;
            if (msg is not null)
            {
                s_messages.Add((msg->message, nameof(GetMsgProc), GetDetail(msg)));
            }
        }

        return PInvoke.CallNextHookEx(s_getMsgProcHook, nCode, wparam, lparam);
    }

    private static unsafe string? GetDetail(MSG* m)
    {
        if (m->IsKeyMessage())
        {
            return ((Keys)(int)m->wParam).ToString();
        }

        return null;
    }
}
