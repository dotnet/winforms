// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Formats.Nrbf;

namespace System.Windows.Forms.BinaryFormat.Deserializer;

internal sealed class ArrayUpdater : ValueUpdater
{
    private readonly int _index;

    internal ArrayUpdater(SerializationRecordId objectId, SerializationRecordId valueId, int index) : base(objectId, valueId)
    {
        _index = index;
    }

    internal override void UpdateValue(IDictionary<SerializationRecordId, object> objects)
    {
        object value = objects[ValueId];
        Array array = (Array)objects[ObjectId];
        array.SetArrayValueByFlattenedIndex(value, _index);
    }
}
