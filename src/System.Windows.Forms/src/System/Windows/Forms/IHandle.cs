// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Used to abstract access to classes that contain a handle.
    /// </summary>
    internal interface IHandle
    {
        public IntPtr Handle { get; }
    }
}
