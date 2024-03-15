// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  Code model serializer for primitive types.
/// </summary>
internal class PrimitiveCodeDomSerializer : CodeDomSerializer
{
    private static PrimitiveCodeDomSerializer? s_defaultSerializer;

    /// <summary>
    ///  Retrieves a default static instance of this serializer.
    /// </summary>
    internal static new PrimitiveCodeDomSerializer Default => s_defaultSerializer ??= new PrimitiveCodeDomSerializer();

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    /// </summary>
    public override object Serialize(IDesignerSerializationManager manager, object? value) => value switch
    {
        string { Length: > 200 } stringValue => SerializeToResourceExpression(manager, stringValue)!,
        null or bool or char or int or float or double or string => new CodePrimitiveExpression(value),
        // Generate a cast for all other types because we won't parse them properly otherwise
        // because we won't know to convert them to the narrow form.
        _ => new CodeCastExpression(new CodeTypeReference(value.GetType()), new CodePrimitiveExpression(value)),
    };
}
