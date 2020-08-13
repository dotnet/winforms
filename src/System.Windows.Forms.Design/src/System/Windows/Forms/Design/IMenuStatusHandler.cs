// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  We plug this interface into the designer event service for overriding menu commands.
    /// </summary>
    internal interface IMenuStatusHandler
    {
        /// <summary>
        ///  CommandSet will check with this handler on each status update to see if the handler wants to override the availability of this command.
        /// </summary>
        bool OverrideInvoke(MenuCommand cmd);

        /// <summary>
        ///  CommandSet will check with this handler on each status update to see if the handler wants to override the availability of this command.
        /// </summary>
        bool OverrideStatus(MenuCommand cmd);
    }
}
