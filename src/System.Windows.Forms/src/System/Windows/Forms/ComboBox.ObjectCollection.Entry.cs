// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
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
            }
        }
    }
}
