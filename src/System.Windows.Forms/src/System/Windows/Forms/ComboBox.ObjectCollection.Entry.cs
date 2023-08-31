// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ComboBox
{
    public partial class ObjectCollection
    {
        /// <summary>
        ///  This is a single entry in ObjectCollection.
        /// </summary>
        internal class Entry
        {
            public Entry(object item)
            {
                Item = item;
            }

            public object Item { get; set; }

            public override string? ToString()
            {
                return Item.ToString();
            }
        }
    }
}
