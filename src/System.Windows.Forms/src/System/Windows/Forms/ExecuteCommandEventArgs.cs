// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;

namespace System.Windows.Forms
{
    public class CommandEventArgs : CancelEventArgs
    {
        public CommandEventArgs()
        {
        }

        public CommandEventArgs(bool cancel) : base(cancel)
        {
        }

        public CommandEventArgs(bool cancel, object parameter) : base(cancel)
        {
            Parameter = parameter;
        }

        public object Parameter { get; set; }
    }
}
