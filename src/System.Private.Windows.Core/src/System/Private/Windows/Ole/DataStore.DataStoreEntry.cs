// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal sealed partial class DataStore<TOleServices>
{
    private class DataStoreEntry
    {
        public object? Data { get; }
        public bool AutoConvert { get; }

        public DataStoreEntry(object? data, bool autoConvert)
        {
            Data = data;
            AutoConvert = autoConvert;
        }
    }
}
