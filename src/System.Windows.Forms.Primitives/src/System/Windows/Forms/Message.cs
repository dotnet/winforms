// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Windows.Forms;

/// <summary>
///  Implements a Windows message.
/// </summary>
public struct Message : IEquatable<Message>, IHandle<HWND>
{
    // Keep HWND, WM, WPARAM, and LPARAM in this order so that they match the MSG struct.
    // This struct shouldn't be used as a direct mapping against MSG, but if someone does already do this
    // it will allow their code to continue to work.

    public IntPtr HWnd { get; set; }

    // Using prefixed variants of the property names for easier diffing.
#pragma warning disable IDE1006 // Naming Styles
    internal MessageId MsgInternal;
    internal WPARAM WParamInternal;
    internal LPARAM LParamInternal;
    internal LRESULT ResultInternal;
    internal readonly HWND HWND => (HWND)HWnd;
#pragma warning restore IDE1006

    public int Msg
    {
        readonly get => (int)MsgInternal;
        set => MsgInternal = (MessageId)value;
    }

    // NOTE: This behavior has changed in .NET 8. IntPtr casts are no longer "checked" by default. We still want
    // to use LPARAM/WPARAM directly for clarity and proper casting.
    //
    // It is particularly dangerous to cast to/from IntPtr on 64 bit platforms as casts are checked.
    // Doing so leads to hard-to-find overflow exceptions. We've mitigated this historically by trying
    // to first cast to long when casting out of IntPtr and first casting to int when casting into an
    // IntPtr. That pattern, unfortunately, is difficult to audit and has negative performance implications,
    // particularly on 32bit.
    //
    // Using the new nint/nuint types allows casting to follow with the default project settings, which
    // is unchecked. Casting works just like casting between other intrinsic types (short, int, long, etc.).
    //
    // Marking it as obsolete in DEBUG to fail the build. In consuming projects you can skip this obsoletion
    // by adding the property <NoWarn>$(NoWarn),WFDEV001</NoWarn> to a property group in your project
    // file (or adding the warning via the project properties pages).
#if DEBUG
    [Obsolete(
        $"Casting to/from IntPtr is unsafe, use {nameof(WParamInternal)}.",
        DiagnosticId = "WFDEV001")]
#endif
    public IntPtr WParam
    {
        readonly get => (nint)(nuint)WParamInternal;
        set => WParamInternal = (nuint)value;
    }

#if DEBUG
    [Obsolete(
        $"Casting to/from IntPtr is unsafe, use {nameof(LParamInternal)}.",
        DiagnosticId = "WFDEV001")]
#endif
    public IntPtr LParam
    {
        readonly get => LParamInternal;
        set => LParamInternal = value;
    }

#if DEBUG
    [Obsolete(
        $"Casting to/from IntPtr is unsafe, use {nameof(ResultInternal)}.",
        DiagnosticId = "WFDEV001")]
#endif
    public IntPtr Result
    {
        readonly get => ResultInternal;
        set => ResultInternal = (LRESULT)value;
    }

    readonly HWND IHandle<HWND>.Handle => HWND;

    /// <summary>
    ///  Gets the <see cref="LParam"/> value, and converts the value to an object.
    /// </summary>
    public readonly object? GetLParam(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
        Type cls) => Marshal.PtrToStructure(LParamInternal, cls);

    internal static unsafe Message Create(MSG* msg)
        => Create(msg->hwnd, msg->message, msg->wParam, msg->lParam);

    internal static Message Create(HWND hWnd, uint msg, WPARAM wparam, LPARAM lparam)
        => Create(hWnd, (int)msg, (nint)(nuint)wparam, (nint)lparam);

    internal static Message Create(HWND hWnd, MessageId msg, WPARAM wparam, LPARAM lparam)
        => Create(hWnd, (int)msg, (nint)(nuint)wparam, (nint)lparam);

    public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
    {
        Message m = new()
        {
            HWnd = hWnd,
            Msg = msg,
            WParamInternal = (WPARAM)(nuint)wparam,
            LParamInternal = lparam,
            ResultInternal = (LRESULT)0
        };

        return m;
    }

    public override readonly bool Equals(object? o)
    {
        if (o is not Message m)
        {
            return false;
        }

        return Equals(m);
    }

    public readonly bool Equals(Message other)
        => HWnd == other.HWnd
            && MsgInternal == other.MsgInternal
            && WParamInternal == other.WParamInternal
            && LParamInternal == other.LParamInternal
            && ResultInternal == other.ResultInternal;

    public static bool operator ==(Message a, Message b) => a.Equals(b);

    public static bool operator !=(Message a, Message b) => !a.Equals(b);

    public override readonly int GetHashCode() => HashCode.Combine(HWnd, Msg);

    public override readonly string ToString() => MessageDecoder.ToString(this);

    internal MSG ToMSG() => new()
    {
        hwnd = HWND,
        message = (uint)MsgInternal,
        wParam = (nuint)WParamInternal,
        lParam = LParamInternal
    };
}
