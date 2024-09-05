// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This class is used to serialize things of type "IContainer". We route all containers
///  to the designer host's container.
/// </summary>
internal class ContainerCodeDomSerializer : CodeDomSerializer
{
    private const string ContainerName = "components";
    private static ContainerCodeDomSerializer? s_defaultSerializer;

    /// <summary>
    ///  Retrieves a default static instance of this serializer.
    /// </summary>
    internal static new ContainerCodeDomSerializer Default => s_defaultSerializer ??= new ContainerCodeDomSerializer();

    /// <summary>
    ///  We override this so we can always provide the correct container as a reference.
    /// </summary>
    protected override object DeserializeInstance(IDesignerSerializationManager manager, Type type, object?[]? parameters, string? name, bool addToContainer)
    {
        if (typeof(IContainer).IsAssignableFrom(type))
        {
            object? obj = manager.GetService(typeof(IContainer));

            if (obj is not null)
            {
                manager.SetName(obj, name!);
                return obj;
            }
        }

        return base.DeserializeInstance(manager, type, parameters, name, addToContainer);
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object. We serialize an IContainer by
    ///  declaring an IContainer member variable and then assigning a Container into it.
    /// </summary>
    public override object Serialize(IDesignerSerializationManager manager, object value)
    {
        CodeStatementCollection statements = [];
        CodeExpression lhs;

        if (manager.TryGetContext(out CodeTypeDeclaration? typeDecl) && manager.TryGetContext(out RootContext? rootCtx))
        {
            CodeMemberField field = new(typeof(IContainer), ContainerName)
            {
                Attributes = MemberAttributes.Private
            };
            typeDecl.Members.Add(field);
            lhs = new CodeFieldReferenceExpression(rootCtx.Expression, ContainerName);
        }
        else
        {
            CodeVariableDeclarationStatement var = new(typeof(IContainer), ContainerName);

            statements.Add(var);
            lhs = new CodeVariableReferenceExpression(ContainerName);
        }

        // Now create the container
        SetExpression(manager, value, lhs);
        CodeObjectCreateExpression objCreate = new(typeof(Container));
        CodeAssignStatement assign = new(lhs, objCreate);

        assign.UserData[nameof(IContainer)] = nameof(IContainer);

        statements.Add(assign);
        return statements;
    }
}
