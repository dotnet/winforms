﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// This event is thown by the DesignerActionListservice when a shortcut is either added or removed to/from the related object.
    /// </summary>
    [System.Runtime.InteropServices.ComVisible(true)]
    public delegate void DesignerActionListsChangedEventHandler(object sender, DesignerActionListsChangedEventArgs e);
}
