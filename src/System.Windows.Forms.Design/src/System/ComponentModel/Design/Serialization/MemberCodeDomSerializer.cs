// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary> This is a special type of code dom serializer that is used to serialize members. For example,
/// when a CodeDomSerializer wishes to serialize a property, it looks for a MemberCodeDomSerializer
/// for the property descriptor, and invokes that serializer to serialize the property.
/// MemberCodeDomSerializers are used both for properties and events and allow serialization decisions
/// to be controlled without changing the code in CodeDomSerializer.
/// </summary>
public abstract class MemberCodeDomSerializer : CodeDomSerializerBase
{
    /// <summary>
    ///  This method actually performs the serialization. When the member is serialized the necessary
    ///  statements will be added to the statements collection.
    /// </summary>
    public abstract void Serialize(
        IDesignerSerializationManager manager,
        object value,
        MemberDescriptor descriptor,
        CodeStatementCollection statements);

    /// <summary>
    ///  This method returns true if the given member descriptor should be serialized,
    ///  or false if there is no need to serialize the member.
    /// </summary>
    public abstract bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor);
}
