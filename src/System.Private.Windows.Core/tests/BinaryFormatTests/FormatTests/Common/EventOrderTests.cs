﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.Serialization;
using FormatTests.Common.TestTypes;
using ValueType = FormatTests.Common.TestTypes.ValueType;

namespace FormatTests.Common;

[Collection("Sequential")]
public abstract class EventOrderTests<T> : SerializationTest<T> where T : ISerializer
{
    #region Depth0
    #region NoCycle
    [Fact]
    public void Depth0_NoCycle_ObjectReference_AsRoot()
    {
        ObjectReferenceISerializable root = new();

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("objects", "objectp", "objecti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ObjectReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["objectp", "objects", "rootp", "rooti", "objecti"]
            : ["objects", "objectp", "rootp", "objecti", "rooti"];
        BinaryTreeNodeWithEvents root = new() { Name = "root", ObjectReference = new ObjectReferenceISerializable() };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ISerializable_ObjectReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["objectp", "objects", "roots", "rootp", "rooti", "objecti"]
            : ["objects", "roots", "objectp", "rootp", "objecti", "rooti"];
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", ObjectReference = new ObjectReferenceISerializable() };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ISerializable()
    {
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root" };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("roots", "rootp", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ISerializable_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "roots", "valuep", "valuei", "rooti"]
            : ["roots", "valuep", "rootp", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Value = new ValueType() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ISerializable_WithValueTypeISerializable()
    {
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("values", "roots", "valuep", "rootp", "valuei", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle()
    {
        BinaryTreeNodeWithEvents root = new() { Name = "root" };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("rootp", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_Surrogate()
    {
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents root = new() { Name = "root" };

        try
        {
            Stream stream = Serialize(root);
            Deserialize(stream, surrogateSelector: selector);
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("roots", "rootp", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["rootp", "valuep", "valuei", "rooti"]
            : ["valuep", "rootp", "valuei", "rooti"];
        BinaryTreeNodeWithEvents root = new() { Name = "root", Value = new ValueType() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_WithValueTypeISerializable()
    {
        BinaryTreeNodeWithEvents root = new() { Name = "root", Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("values", "valuep", "rootp", "valuei", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ValueTypeWithSurrogate()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "roots", "valuep", "valuei", "rooti"]
            : ["roots", "valuep", "rootp", "valuei", "rooti"];
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents root = new() { Name = "root", Value = new ValueType() { Name = "value" } };

        try
        {
            Stream stream = Serialize(root);
            Deserialize(stream, surrogateSelector: selector);
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_NoCycle_ValueTypeISerializableWithSurrogate()
    {;
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents root = new() { Name = "root", Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Stream stream = Serialize(root);
            Deserialize(stream, surrogateSelector: selector);
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("values", "roots", "valuep", "rootp", "valuei", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    #endregion NoCycle

    #region Cycle
    [Fact]
    public void Depth0_SelfCycle_ISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "roots", "rooti"]
            : ["roots", "rootp", "rooti"];
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root" };
        root.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_SelfCycle_ISerializable_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "roots", "valuep", "valuei", "rooti"]
            : ["roots", "valuep", "rootp", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Value = new ValueType() { Name = "value" } };
        root.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_SelfCycle_ISerializable_WithValueTypeISerializable()
    {
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Value = new ValueTypeISerializable() { Name = "value" } };
        root.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("values", "roots", "valuep", "rootp", "valuei", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_SelfCycle()
    {
        BinaryTreeNodeWithEvents root = new() { Name = "root" };
        root.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("rootp", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_SelfCycle_Surrogate()
    {
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents root = new() { Name = "root" };
        root.Left = root;

        try
        {
            Stream stream = Serialize(root);
            if (IsBinaryFormatterDeserializer)
            {
                Action action = () => Deserialize(stream, surrogateSelector: selector);
            }
            else
            {
                Deserialize(stream, surrogateSelector: selector);
                List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
                deserializeOrder.Should().Equal("roots", "rootp", "rooti");
            }
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_SelfCycle_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["rootp", "valuep", "valuei", "rooti"]
            : ["valuep", "rootp", "valuei", "rooti"];
        BinaryTreeNodeWithEvents root = new() { Name = "root", Value = new ValueType() { Name = "value" } };
        root.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth0_SelfCycle_WithValueTypeISerializable()
    {
        BinaryTreeNodeWithEvents root = new() { Name = "root", Value = new ValueTypeISerializable() { Name = "value" } };
        root.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal("values", "valuep", "rootp", "valuei", "rooti");
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }
    #endregion Cycle
    #endregion Depth0

    #region Depth1
    #region NoCycle
    [Fact]
    public void Depth1_NoCycle_ObjectReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["objectp", "objects", "rootp", "childp", "rooti", "childi", "objecti"]
            : ["objects", "objectp", "childp", "rootp", "objecti", "childi", "rooti"];
        BinaryTreeNodeWithEvents child = new() { Name = "child", ObjectReference = new ObjectReferenceISerializable() };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_ISerializable_ObjectReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["objectp", "objects", "childs", "roots", "rootp", "childp", "rooti", "childi", "objecti"]
            : ["objects", "childs", "roots", "objectp", "childp", "rootp", "objecti", "childi", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child", ObjectReference = new ObjectReferenceISerializable() };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_ISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "childs", "roots", "rootp", "rooti", "childi"]
            : ["childs", "roots", "childp", "rootp", "childi", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child" };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_ISerializable_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "childs", "roots", "valuep", "rootp", "valuei", "rooti", "childi"]
            : ["childs", "roots", "childp", "valuep", "rootp", "childi", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child" };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child, Value = new ValueType() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_ISerializable_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "values", "childs", "roots", "valuep", "rootp", "valuei", "rooti", "childi"]
            : ["childs", "values", "roots", "childp", "valuep", "rootp", "childi", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child" };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child, Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_ISerializable_WithValueTypeISerializable_WithReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "childs", "values", "roots", "valuep", "rootp", "valuei", "rooti", "childi"]
            : ["childs", "values", "roots", "childp", "valuep", "rootp", "childi", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child" };
        BinaryTreeNodeWithEventsISerializable root = new()
        {
            Name = "root",
            Left = child,
            Value = new ValueTypeISerializable() { Name = "value", Reference = child }
        };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["childp", "rootp", "rooti", "childi"]
            : ["childp", "rootp", "childi", "rooti"];
        BinaryTreeNodeWithEvents child = new() { Name = "child" };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_Surrogate()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "childs", "roots", "rootp", "rooti", "childi"]
            : ["childs", "roots", "childp", "rootp", "childi", "rooti"];
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents child = new() { Name = "child" };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child };

        try
        {
            Stream stream = Serialize(root);
            Deserialize(stream, surrogateSelector: selector);
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["childp", "valuep", "rootp", "valuei", "rooti", "childi"]
            : ["childp", "valuep", "rootp", "childi", "valuei", "rooti"];
        BinaryTreeNodeWithEvents child = new() { Name = "child" };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child, Value = new ValueType() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_NoCycle_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["childp", "values", "valuep", "rootp", "valuei", "rooti", "childi"]
            : ["values", "childp", "valuep", "rootp", "childi", "valuei", "rooti" ];
        BinaryTreeNodeWithEvents child = new() { Name = "child" };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child, Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }
    #endregion NoCycle

    #region Cycle
    [Fact]
    public void Depth1_Cycle_ISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "childs", "roots", "rootp", "rooti", "childi"]
            : ["childs", "roots", "childp", "rootp", "childi", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child" };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child };
        child.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_Cycle_ISerializable_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "childs", "roots", "value2p", "rootp", "value1p", "value2i", "rooti", "value1i", "childi"]
            : ["childs", "roots", "value1p", "childp", "value2p", "rootp", "value1i", "childi", "value2i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child", Value = new ValueType() { Name = "value1" } };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child, Value = new ValueType() { Name = "value2" } };
        child.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_Cycle_ISerializable_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["value2s", "value1s", "childs", "roots", "value2p", "rootp", "value1p", "childp", "value2i", "rooti", "value1i", "childi"]
            : ["value1s", "childs", "value2s", "roots", "value1p", "childp", "value2p", "rootp", "value1i", "childi", "value2i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child = new() { Name = "child", Value = new ValueTypeISerializable() { Name = "value1" } };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child, Value = new ValueTypeISerializable() { Name = "value2" } };
        child.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_Cycle()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["childp", "rootp", "rooti", "childi"]
            : ["childp", "rootp", "childi", "rooti"];
        BinaryTreeNodeWithEvents child = new() { Name = "child" };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child };
        child.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_Cycle_Surrogate()
    {
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents child = new() { Name = "child" };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child };
        child.Left = root;

        try
        {
            Stream stream = Serialize(root);
            if (IsBinaryFormatterDeserializer)
            {
                Action action = () => Deserialize(stream, surrogateSelector: selector);
                action.Should().Throw<SerializationException>();
            }
            else
            {
                Deserialize(stream, surrogateSelector: selector);
                List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
                deserializeOrder.Should().Equal("childs", "roots", "childp", "rootp", "childi", "rooti");
            }
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_Cycle_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["childp", "value2p", "rootp", "value1p", "value2i", "rooti", "value1i", "childi"]
            : ["value1p", "childp", "value2p", "rootp", "value1i", "childi", "value2i", "rooti"];
        BinaryTreeNodeWithEvents child = new() { Name = "child", Value = new ValueType() { Name = "value1" } };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child, Value = new ValueType() { Name = "value2" } };
        child.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth1_Cycle_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["value2s", "value1s", "value2p", "rootp", "value1p", "childp", "value2i", "rooti", "value1i", "childi"]
            : ["value1s", "value2s", "value1p", "childp", "value2p", "rootp", "value1i", "childi", "value2i", "rooti"];
        BinaryTreeNodeWithEvents child = new() { Name = "child", Value = new ValueTypeISerializable() { Name = "value1" } };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child, Value = new ValueTypeISerializable() { Name = "value2" } };
        child.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }
    #endregion Cycle
    #endregion Depth1

    #region Depth2
    #region NoCycle
    [Fact]
    public void Depth2_NoCycle_ObjectReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["objectp", "objects", "rootp", "child1p", "child2p", "rooti", "child1i", "child2i", "objecti"]
            : ["objects", "objectp", "child2p", "child1p", "rootp", "objecti", "child2i", "child1i", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2", ObjectReference = new ObjectReferenceISerializable() };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1 };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_ISerializable_ObjectReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["objectp", "objects", "child2s", "child1s", "roots", "rootp", "child1p", "child2p", "rooti", "child1i", "child2i", "objecti"]
            : ["objects", "child2s", "child1s", "roots", "objectp", "child2p", "child1p", "rootp", "objecti", "child2i", "child1i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2", ObjectReference = new ObjectReferenceISerializable() };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1 };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_ISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "child2s", "child1s", "roots", "rootp", "child1p", "rooti", "child1i", "child2i"]
            : ["child2s", "child1s", "roots", "child2p", "child1p", "rootp", "child2i", "child1i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1 };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_ISerializable_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "child2s", "child1s", "roots", "valuep", "rootp", "child1p", "valuei", "rooti", "child1i", "child2i"]
            : ["child2s", "child1s", "roots", "child2p", "child1p", "valuep", "rootp", "child2i", "child1i", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1, Value = new ValueType() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_ISerializable_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "values", "child2s", "child1s", "roots", "valuep", "rootp", "child1p", "valuei", "rooti", "child1i", "child2i"]
            : ["child2s", "child1s", "values", "roots", "child2p", "child1p", "valuep", "rootp", "child2i", "child1i", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1, Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_ISerializable_WithValueTypeISerializable_WithReference()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "child2s", "values", "child1s", "roots", "valuep", "rootp", "child1p", "valuei", "rooti", "child1i", "child2i"]
            : ["child2s", "child1s", "values", "roots", "child2p", "child1p", "valuep", "rootp", "child2i", "child1i", "valuei", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEventsISerializable root = new()
        {
            Name = "root",
            Left = child1,
            Value = new ValueTypeISerializable() { Name = "value", Reference = child2 }
        };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["child2p", "rootp", "child1p", "rooti", "child1i", "child2i"]
            : ["child2p", "child1p", "rootp", "child2i", "child1i", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1 };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_Surrogate()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "child2s", "child1s", "roots", "rootp", "child1p", "rooti", "child1i", "child2i"]
            : ["child2s", "child1s", "roots", "child2p", "child1p", "rootp", "child2i", "child1i", "rooti"];
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1 };

        try
        {
            Stream stream = Serialize(root);
            Deserialize(stream, surrogateSelector: selector);
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["child2p", "valuep", "rootp", "child1p", "valuei", "rooti", "child1i", "child2i"]
            : ["child2p", "child1p", "valuep", "rootp", "child2i", "child1i", "valuei", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1, Value = new ValueType() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_NoCycle_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["child2p", "values", "valuep", "rootp", "child1p", "valuei", "rooti", "child1i", "child2i"]
            : ["values", "child2p", "child1p", "valuep", "rootp", "child2i", "child1i", "valuei", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1, Value = new ValueTypeISerializable() { Name = "value" } };

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }
    #endregion NoCycle

    #region Cycle
    [Fact]
    public void Depth2_Cycle_ISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "child2s", "child1s", "roots", "rootp", "child1p", "rooti", "child1i", "child2i"]
            : ["child2s", "child1s", "roots", "child2p", "child1p", "rootp", "child2i", "child1i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1 };
        child2.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_Cycle_ISerializable_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["p", "child2s", "child1s", "roots", "value3p", "rootp", "value2p", "child1p", "value1p", "value3i", "rooti", "value2i", "child1i", "value1i", "child2i"]
            : ["child2s", "child1s", "roots", "value1p", "child2p", "value2p", "child1p", "value3p", "rootp", "value1i", "child2i", "value2i", "child1i", "value3i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2", Value = new ValueType() { Name = "value1" } };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2, Value = new ValueType() { Name = "value2" } };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1, Value = new ValueType() { Name = "value3" } };
        child2.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_Cycle_ISerializable_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["value3s", "value2s", "value1s", "child2s", "child1s", "roots", "value3p", "rootp", "value2p", "child1p", "value1p", "child2p", "value3i", "rooti", "value2i", "child1i", "value1i", "child2i" ]
            : ["value1s", "child2s", "value2s", "child1s", "value3s", "roots", "value1p", "child2p", "value2p", "child1p", "value3p", "rootp", "value1i", "child2i", "value2i", "child1i", "value3i", "rooti"];
        BinaryTreeNodeWithEventsISerializable child2 = new() { Name = "child2", Value = new ValueTypeISerializable() { Name = "value1" } };
        BinaryTreeNodeWithEventsISerializable child1 = new() { Name = "child1", Left = child2, Value = new ValueTypeISerializable() { Name = "value2" } };
        BinaryTreeNodeWithEventsISerializable root = new() { Name = "root", Left = child1, Value = new ValueTypeISerializable() { Name = "value3" } };
        child2.Left = root;
        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_Cycle()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["child2p", "rootp", "child1p", "rooti", "child1i", "child2i"]
            : ["child2p", "child1p", "rootp", "child2i", "child1i", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1 };
        child2.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_Cycle_Surrogate()
    {
        SurrogateSelector selector = CreateSurrogateSelector<BinaryTreeNodeWithEvents>(new BinaryTreeNodeWithEventsSurrogate());
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2" };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2 };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1 };
        child2.Left = root;

        try
        {
            Stream stream = Serialize(root);
            if (IsBinaryFormatterDeserializer)
            {
                Action action = () => Deserialize(stream, surrogateSelector: selector);
                action.Should().Throw<SerializationException>();
            }
            else
            {
                Deserialize(stream, surrogateSelector: selector);
                List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
                deserializeOrder.Should().Equal("child2s", "child1s", "roots", "child2p", "child1p", "rootp", "child2i", "child1i", "rooti");
            }
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_Cycle_WithValueType()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["child2p", "value3p", "rootp", "value2p", "child1p", "value1p", "value3i", "rooti", "value2i", "child1i", "value1i", "child2i"]
            : ["value1p", "child2p", "value2p", "child1p", "value3p", "rootp", "value1i", "child2i", "value2i", "child1i", "value3i", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2", Value = new ValueType() { Name = "value1" } };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2, Value = new ValueType() { Name = "value2" } };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1, Value = new ValueType() { Name = "value3" } };
        child2.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }

    [Fact]
    public void Depth2_Cycle_WithValueTypeISerializable()
    {
        string[] expected = IsBinaryFormatterDeserializer
            ? ["value3s", "value2s", "value1s", "value3p", "rootp", "value2p", "child1p", "value1p", "child2p", "value3i", "rooti", "value2i", "child1i", "value1i", "child2i"]
            : ["value1s", "value2s", "value3s", "value1p", "child2p", "value2p", "child1p", "value3p", "rootp", "value1i", "child2i", "value2i", "child1i", "value3i", "rooti"];
        BinaryTreeNodeWithEvents child2 = new() { Name = "child2", Value = new ValueTypeISerializable() { Name = "value1" } };
        BinaryTreeNodeWithEvents child1 = new() { Name = "child1", Left = child2, Value = new ValueTypeISerializable() { Name = "value2" } };
        BinaryTreeNodeWithEvents root = new() { Name = "root", Left = child1, Value = new ValueTypeISerializable() { Name = "value3" } };
        child2.Left = root;

        try
        {
            Deserialize(Serialize(root));
            List<string> deserializeOrder = BinaryTreeNodeWithEventsTracker.DeserializationOrder;
            deserializeOrder.Should().Equal(expected);
        }
        finally
        {
            BinaryTreeNodeWithEventsTracker.DeserializationOrder.Clear();
        }
    }
    #endregion Cycle
    #endregion Depth2
}
