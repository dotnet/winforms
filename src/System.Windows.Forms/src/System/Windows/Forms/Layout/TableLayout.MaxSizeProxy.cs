// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Layout
{
    internal partial class TableLayout
    {
        private class MaxSizeProxy : SizeProxy
        {
            private static readonly MaxSizeProxy instance = new MaxSizeProxy();
            public override int Size
            {
                get { return strip.MaxSize; }
                set { strip.MaxSize = value; }
            }

            public static MaxSizeProxy GetInstance
            {
                get
                {
                    return instance;
                }
            }
        }
    }
}
