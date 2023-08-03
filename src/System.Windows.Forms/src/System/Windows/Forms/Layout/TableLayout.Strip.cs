// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    internal struct Strip
    {
        private int _maxSize;
        private int _minSize;
        private bool _isStart;  //whether there is an element starting in this strip

        public int MinSize
        {
            get { return _minSize; }
            set { _minSize = value; }
        }

        public int MaxSize
        {
            get { return _maxSize; }
            set { _maxSize = value; }
        }

        public bool IsStart
        {
            get { return _isStart; }
            set { _isStart = value; }
        }
    }
}
