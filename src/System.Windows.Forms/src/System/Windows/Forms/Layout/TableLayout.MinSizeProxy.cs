// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Layout;

internal partial class TableLayout
{
    private class MinSizeProxy : SizeProxy
    {
        private static readonly MinSizeProxy instance = new();
        public override int Size
        {
            get { return strip.MinSize; }
            set { strip.MinSize = value; }
        }

        public static MinSizeProxy GetInstance
        {
            get
            {
                return instance;
            }
        }
    }
}
