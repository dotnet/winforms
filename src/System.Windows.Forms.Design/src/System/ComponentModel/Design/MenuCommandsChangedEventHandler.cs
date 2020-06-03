// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    ///     This event is thown by the MenuCommandService when a shortcut
    ///     is either added or removed to/from the related object.
    /// </summary>
    public delegate void MenuCommandsChangedEventHandler(object sender, MenuCommandsChangedEventArgs e);
}

