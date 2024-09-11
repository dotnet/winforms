// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  The root context is added by a type code dom serializer to provide a
///  definiton of the "root" object.
/// </summary>
public sealed class RootContext
{
    /// <summary>
    ///  This object can be placed on the context stack to represent the
    ///  object that is the root of the serialization hierarchy. In addition
    ///  to this instance, the RootContext also contains an expression that
    ///  can be used to reference the RootContext.
    /// </summary>
    public RootContext(CodeExpression expression, object value)
    {
        Expression = expression.OrThrowIfNull();
        Value = value.OrThrowIfNull();
    }

    /// <summary>
    ///  The expression representing the root object in the object graph.
    /// </summary>
    public CodeExpression Expression { get; }

    /// <summary>
    ///  The root object of the object graph.
    /// </summary>
    public object Value { get; }
}
