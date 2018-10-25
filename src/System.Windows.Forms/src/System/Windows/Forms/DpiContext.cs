//------------------------------------------------------------------------------
// <copyright file="DpiContext.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------
namespace System.Windows.Forms {

    using System;

    /// <summary>
    /// CONSIDER: make this public, specifically the accessor to the Dpi value, not the settor.
    /// </summary>
    internal sealed class DpiContext {
        public static IntPtr Invalid = IntPtr.Zero;

        [ThreadStatic]
        private static IntPtr currentContext;

        internal static IntPtr UpdateDpi(IntPtr context) {
            IntPtr previousContext = currentContext;
            currentContext = context;
            return previousContext;
        }

        public static int Dpi {
            get {
                return DpiHelper.GetDpiForWindow(currentContext);
            }
        }
    }
}