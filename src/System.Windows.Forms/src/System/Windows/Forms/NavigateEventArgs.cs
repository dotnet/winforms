// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    [ComVisible(true)]
    public class NavigateEventArgs : EventArgs
    {
        public NavigateEventArgs(bool isForward)
        {
            Forward = isForward;
        }

        public bool Forward { get; }
    }
}
