// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    // sizeProxy. Takes a strip and return its minSize or maxSize accordingly
    private abstract class SizeProxy
    {
        protected Strip strip;
        public Strip Strip
        {
            get { return strip; }
            set { strip = value; }
        }

        public abstract int Size
        {
            get; set;
        }
    }
}
