// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal partial class ItemArray
{
    /// <summary>
    ///  This is a single entry in our item array.
    /// </summary>
    internal class Entry
    {
        public Entry(object item)
        {
            Item = item;
            State = 0;
        }

        public object Item { get; set; }

        // This field is used by a CheckList to store data that an item is checked or indeterminate
        public int State { get; set; }
    }
}
