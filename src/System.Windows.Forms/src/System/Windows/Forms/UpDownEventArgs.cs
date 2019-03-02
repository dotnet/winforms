﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <devdoc>
    /// Provides data for the UpDownEvent
    /// </devdoc>
    public class UpDownEventArgs : EventArgs
    {
        public UpDownEventArgs(int buttonPushed)
        {
            ButtonID = buttonPushed;
        }

        public int ButtonID { get; }
    }
}
