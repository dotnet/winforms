﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if DEBUG
using System.Diagnostics;
#endif
using System.Runtime.InteropServices;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Implements a Windows message.
    /// </summary>
    public struct Message
    {
#if DEBUG
        private static readonly TraceSwitch s_allWinMessages = new("AllWinMessages", "Output every received message");
#endif

        // Using prefixed variants of the property names for easier diffing.
#pragma warning disable IDE1006 // Naming Styles
        internal nint _Result;
        internal nint _LParam;
        internal nint _WParam;
        internal User32.WM _Msg;
#pragma warning restore IDE1006 // Naming Styles

        public IntPtr HWnd { get; set; }

        public int Msg
        {
            get => (int)_Msg;
            set => _Msg = (User32.WM)value;
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
        // by adding the property <NoWarn>$(NoWarn),WINFORMSDEV0001</NoWarn> to a property group in your project
        // file (or adding the warning via the project properties pages).
#if DEBUG
        [Obsolete(
            $"Casting to/from IntPtr is unsafe, use {nameof(_WParam)}.",
            DiagnosticId = "WINFORMSDEV0001")]
#endif
        public IntPtr WParam
        {
            get => _WParam;
            set => _WParam = value;
        }

#if DEBUG
        [Obsolete(
            $"Casting to/from IntPtr is unsafe, use {nameof(_LParam)}.",
            DiagnosticId = "WINFORMSDEV0001")]
#endif
        public IntPtr LParam
        {
            get => _LParam;
            set => _LParam = value;
        }

#if DEBUG
        [Obsolete(
            $"Casting to/from IntPtr is unsafe, use {nameof(_Result)}.",
            DiagnosticId = "WINFORMSDEV0001")]
#endif
        public IntPtr Result
        {
            get => _Result;
            set => _Result = value;
        }

        /// <summary>
        ///  Gets the <see cref='LParam'/> value, and converts the value to an object.
        /// </summary>
        public object? GetLParam(Type cls) => Marshal.PtrToStructure(_LParam, cls);

        internal static Message Create(IntPtr hWnd, User32.WM msg, nint wparam, nint lparam)
            => Create(hWnd, (int)msg, wparam, lparam);

        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var m = new Message
            {
                HWnd = hWnd,
                Msg = msg,
                _WParam = wparam,
                _LParam = lparam,
                _Result = IntPtr.Zero
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

            return HWnd == m.HWnd
                && _Msg == m._Msg
                && _WParam == m._WParam
                && _LParam == m._LParam
                && _Result == m._Result;
        }

        public static bool operator ==(Message a, Message b) => a.Equals(b);

        public static bool operator !=(Message a, Message b) => !a.Equals(b);

        public override int GetHashCode() => HashCode.Combine(HWnd, Msg);

        public override string ToString() => MessageDecoder.ToString(this);
    }
}
