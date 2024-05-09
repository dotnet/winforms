// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;

namespace FormatTests.Common.TestTypes;

[Serializable]
public class ObjectReferenceISerializable : ISerializable, IObjectReference, IDeserializationCallback
{
    public ObjectReferenceISerializable() { }

    protected ObjectReferenceISerializable(SerializationInfo serializationInfo, StreamingContext streamingContext)
    {
        BinaryTreeNodeWithEventsTracker.DeserializationOrder.Add($"objects");
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context) { }

    public object GetRealObject(StreamingContext context) => new InnerObject();

    public void OnDeserialization(object? sender) => BinaryTreeNodeWithEventsTracker.DeserializationOrder.Add($"objecti");

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => BinaryTreeNodeWithEventsTracker.DeserializationOrder.Add($"objectp");
}

public class InnerObject : IDeserializationCallback
{
    public void OnDeserialization(object? sender) => BinaryTreeNodeWithEventsTracker.DeserializationOrder.Add($"inneri");

    [OnDeserialized]
    private void OnDeserialized(StreamingContext context) => BinaryTreeNodeWithEventsTracker.DeserializationOrder.Add($"innerp");
}
