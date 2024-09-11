// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Resources;

namespace System.ComponentModel.Design.Serialization;

public sealed partial class CodeDomComponentSerializationService
{
    private sealed partial class CodeDomSerializationStore
    {
        /// <summary>
        ///  Our private resource manager...it just pushes all the data into a hashtable and
        ///  then we serialize the hashtable. On deserialization, the hashtable is rebuilt
        ///  for us and we have all the data we saved out.
        /// </summary>
        private class LocalResourceManager : ResourceManager, IResourceWriter, IResourceReader
        {
            private Hashtable? _hashtable;

            internal LocalResourceManager() { }
            internal LocalResourceManager(Hashtable? data) { _hashtable = data; }

            internal Hashtable Data => _hashtable ??= [];

            // IResourceWriter
            public void AddResource(string name, object? value) { Data[name] = value; }
            public void AddResource(string name, string? value) { Data[name] = value; }
            public void AddResource(string name, byte[]? value) { Data[name] = value; }
            public void Close() { }
            public void Dispose() { Data.Clear(); }
            public void Generate() { }

            // IResourceReader / ResourceManager
            public override object? GetObject(string name) { return Data[name]; }
            public override string? GetString(string name) { return Data[name] as string; }
            public IDictionaryEnumerator GetEnumerator() { return Data.GetEnumerator(); }
            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
