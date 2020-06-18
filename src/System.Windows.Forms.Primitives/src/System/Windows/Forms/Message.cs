// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
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
        private static readonly TraceSwitch s_allWinMessages = new TraceSwitch("AllWinMessages", "Output every received message");
#endif

        public IntPtr HWnd { get; set; }

        public int Msg { get; set; }

        public IntPtr WParam { get; set; }

        public IntPtr LParam { get; set; }

        public IntPtr Result { get; set; }

        /// <summary>
        ///  Gets the <see cref='LParam'/> value, and converts the value to an object.
        /// </summary>
        public object? GetLParam(Type cls) => Marshal.PtrToStructure(LParam, cls);

        internal static Message Create(IntPtr hWnd, User32.WM msg, IntPtr wparam, IntPtr lparam)
            => Create(hWnd, (int)msg, wparam, lparam);

        public static Message Create(IntPtr hWnd, int msg, IntPtr wparam, IntPtr lparam)
        {
            var m = new Message
            {
                HWnd = hWnd,
                Msg = msg,
                WParam = wparam,
                LParam = lparam,
                Result = IntPtr.Zero
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
            if (!(o is Message m))
            {
                return false;
            }

            return HWnd == m.HWnd &&
                   Msg == m.Msg &&
                   WParam == m.WParam &&
                   LParam == m.LParam &&
                   Result == m.Result;
        }

        public static bool operator ==(Message a, Message b) => a.Equals(b);

        public static bool operator !=(Message a, Message b) => !a.Equals(b);

        public override int GetHashCode() => HashCode.Combine(HWnd, Msg);

        public override string ToString() => MessageDecoder.ToString(this);
    }
}
