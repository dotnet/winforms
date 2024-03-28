// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class Splitter
{
    /// <summary>
    ///  Return value holder.
    /// </summary>
    private class SplitData
    {
        public int dockWidth = -1;
        public int dockHeight = -1;
        internal Control? _target;
    }
}
