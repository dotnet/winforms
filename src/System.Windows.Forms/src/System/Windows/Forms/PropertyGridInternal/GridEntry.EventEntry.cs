// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.PropertyGridInternal
{
    internal abstract partial class GridEntry
    {
        private sealed class EventEntry
        {
            internal EventEntry next;
            internal object key;
            internal Delegate handler;

            internal EventEntry(EventEntry next, object key, Delegate handler)
            {
                this.next = next;
                this.key = key;
                this.handler = handler;
            }
        }
    }
}
