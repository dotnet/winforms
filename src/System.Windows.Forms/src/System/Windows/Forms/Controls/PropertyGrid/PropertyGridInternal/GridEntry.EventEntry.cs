// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal abstract partial class GridEntry
{
    private sealed class EventEntry
    {
        public Delegate? Handler { get; set; }
        public object Key { get; }
        public EventEntry? Next { get; set; }

        internal EventEntry(EventEntry? next, object key, Delegate? handler)
        {
            Next = next;
            Key = key;
            Handler = handler;
        }
    }
}
