// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Defines a message filter interface.
    /// </summary>
    public interface IMessageFilter
    {
        /// <summary>
        ///  Filters out a message before it is dispatched.
        /// </summary>
        bool PreFilterMessage(ref Message m);
    }
}
