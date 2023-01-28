// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if DEBUG
using System.Diagnostics;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a Windows message.
    /// </summary>
    public struct Message : IEquatable<Message>, IHandle<HWND>
    {
#if DEBUG
        private static readonly TraceSwitch s_allWinMessages = new("AllWinMessages", "Output every received message");
#endif

        // Keep HWND, WM, WPARAM, and LPARAM in this order so that they match the MSG struct.
        // This struct shouldn't be used as a direct mapping against MSG, but if someone does already do this
        // it will allow their code to continue to work.

        public IntPtr HWnd { get; set; }

        // Using prefixed variants of the property names for easier diffing.
#pragma warning disable IDE1006 // Naming Styles
        internal User32.WM MsgInternal;
        internal WPARAM WParamInternal;
        internal LPARAM LParamInternal;
        internal LRESULT ResultInternal;
        internal HWND HWND => (HWND)HWnd;
#pragma warning restore IDE1006 // Naming Styles

        public int Msg
        {
            get => (int)MsgInternal;
            set => MsgInternal = (User32.WM)value;
        }

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
            get => (nint)(nuint)WParamInternal;
            set => WParamInternal = (nuint)value;
        }

#if DEBUG
        [Obsolete(
            $"Casting to/from IntPtr is unsafe, use {nameof(LParamInternal)}.",
            DiagnosticId = "WFDEV001")]
#endif
        public IntPtr LParam
        {
            get => LParamInternal;
            set => LParamInternal = value;
        }

#if DEBUG
        [Obsolete(
            $"Casting to/from IntPtr is unsafe, use {nameof(ResultInternal)}.",
            DiagnosticId = "WFDEV001")]
#endif
        public IntPtr Result
        {
            get => ResultInternal;
            set => ResultInternal = (LRESULT)value;
        }

        HWND IHandle<HWND>.Handle => HWND;

        /// <summary>
        ///  Gets the <see cref="LParam"/> value, and converts the value to an object.
        /// </summary>
        public object? GetLParam(
            [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.NonPublicConstructors)]
            Type cls) => Marshal.PtrToStructure(LParamInternal, cls);

        internal static unsafe Message Create(MSG* msg)
            => Create(msg->hwnd, msg->message, msg->wParam, msg->lParam);

        internal static Message Create(HWND hWnd, uint msg, WPARAM wparam, LPARAM lparam)
            => Create(hWnd, (int)msg, (nint)(nuint)wparam, (nint)lparam);

        internal static Message Create(HWND hWnd, User32.WM msg, WPARAM wparam, LPARAM lparam)
            => Create(hWnd, (int)msg, (nint)(nuint)wparam, (nint)lparam);

        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var m = new Message
            {
                HWnd = hWnd,
                Msg = msg,
                WParamInternal = (WPARAM)(nuint)wparam,
                LParamInternal = lparam,
                ResultInternal = (LRESULT)0
            };

#if DEBUG
            if (s_allWinMessages.TraceVerbose)
            {
                Debug.WriteLine(m.ToString());
            }
#endif
            return m;
        }

        public override bool Equals(object? o)
        {
            if (o is not Message m)
            {
                return false;
            }

            return Equals(m);
        }

        public bool Equals(Message other)
            => HWnd == other.HWnd
                && MsgInternal == other.MsgInternal
                && WParamInternal == other.WParamInternal
                && LParamInternal == other.LParamInternal
                && ResultInternal == other.ResultInternal;

        public static bool operator ==(Message a, Message b) => a.Equals(b);

        public static bool operator !=(Message a, Message b) => !a.Equals(b);

        public override int GetHashCode() => HashCode.Combine(HWnd, Msg);

        public override string ToString() => MessageDecoder.ToString(this);

        internal MSG ToMSG() => new()
        {
            hwnd = HWND,
            message = (uint)MsgInternal,
            wParam = (nuint)WParamInternal,
            lParam = LParamInternal
        };
    }
}
