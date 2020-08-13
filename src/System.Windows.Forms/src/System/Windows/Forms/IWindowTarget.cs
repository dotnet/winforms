// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  This interface defines the communication layer between a Control object and the
    ///  Win32 API. Each Control object has an internal implementation this interface that
    ///  is called by the Win32 window.
    /// </summary>
    public interface IWindowTarget
    {
        /// <summary>
        ///  Called when the window handle of the control has changed.
        /// </summary>
        void OnHandleChange(IntPtr newHandle);

        /// <summary>
        ///  Called to do control-specific processing for this window.
        /// </summary>
        void OnMessage(ref Message m);
    }
}
