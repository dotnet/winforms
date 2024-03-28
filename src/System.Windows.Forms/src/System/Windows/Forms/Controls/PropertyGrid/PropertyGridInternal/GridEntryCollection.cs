// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.PropertyGridInternal;

internal sealed class GridEntryCollection : NonNullCollection<GridEntry>, IDisposable
{
    private readonly bool _disposeItems;

    public GridEntryCollection(IEnumerable<GridEntry>? items = null, bool disposeItems = true)
        : base(items ?? [])
    {
        _disposeItems = disposeItems;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (disposing && _disposeItems)
        {
            foreach (GridEntry entry in this)
            {
                entry.Dispose();
            }

            Clear();
        }
    }
}
