﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms.PropertyGridInternal {

    using System.Diagnostics;
    using System;
    using System.ComponentModel.Design;
    using Microsoft.Win32;

    /// <devdoc>
    ///     This class contains the set of menu commands our property browser
    ///     uses.
    /// </devdoc>
    public class PropertyGridCommands{

        /// <devdoc>
        ///      This guid corresponds to the menu grouping windows forms will use for its menus.  This is
        ///      defined in the windows forms menu CTC file, but we need it here so we can define what
        ///      context menus to use.
        /// </devdoc>
        protected static readonly Guid wfcMenuGroup = new Guid("{a72bd644-1979-4cbc-a620-ea4112198a66}");

        /// <devdoc>
        ///     This guid corresponds to the windows forms command set.
        /// </devdoc>
        protected static readonly Guid wfcMenuCommand = new Guid("{5a51cf82-7619-4a5d-b054-47f438425aa7}");

        public static readonly CommandID Reset          = new CommandID(wfcMenuCommand, 0x3000);
        public static readonly CommandID Description    = new CommandID(wfcMenuCommand, 0x3001);
        public static readonly CommandID Hide           = new CommandID(wfcMenuCommand, 0x3002);
        public static readonly CommandID Commands       = new CommandID(wfcMenuCommand, 0x3010);
    }

}
