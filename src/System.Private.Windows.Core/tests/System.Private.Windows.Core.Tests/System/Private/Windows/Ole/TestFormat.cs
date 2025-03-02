// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Private.Windows.Ole;

internal class TestFormat : IDataFormat<TestFormat>
{
    public TestFormat(string name, int id)
    {
        Name = name;
        Id = id;
    }

    public string Name { get; }
    public int Id { get; }
    static TestFormat IDataFormat<TestFormat>.Create(string name, int id) => new(name, id);
}
