// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class ListBox
    {
        internal partial class ItemArray
        {
            /// <summary>
            ///  This is a single entry in our item array.
            /// </summary>
            internal class Entry
            {
                public object item;
                public int state;

                public Entry(object item)
                {
                    this.item = item;
                    state = 0;
                }
            }
        }
    }
}
