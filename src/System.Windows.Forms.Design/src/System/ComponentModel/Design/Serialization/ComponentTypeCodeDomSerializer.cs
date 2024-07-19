// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This class performs the same tasks as a CodeDomSerializer only serializing an object through this class defines a new type.
/// </summary>
internal class ComponentTypeCodeDomSerializer : TypeCodeDomSerializer
{
    private static readonly object s_initMethodKey = new();
    private const string InitMethodName = "InitializeComponent";
    private static ComponentTypeCodeDomSerializer? s_default;

    internal static new ComponentTypeCodeDomSerializer Default => s_default ??= new ComponentTypeCodeDomSerializer();

    /// <summary>
    ///  This method returns the method to emit all of the initialization code to for the given member.
    ///  The default implementation returns an empty constructor.
    /// </summary>
    protected override CodeMemberMethod GetInitializeMethod(IDesignerSerializationManager manager, CodeTypeDeclaration typeDecl, object value)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(typeDecl);
        ArgumentNullException.ThrowIfNull(value);

        if (typeDecl.UserData[s_initMethodKey] is not CodeMemberMethod method)
        {
            method = new CodeMemberMethod
            {
                Name = InitMethodName,
                Attributes = MemberAttributes.Private
            };
            typeDecl.UserData[s_initMethodKey] = method;

            // Now create a ctor that calls this method.
            CodeConstructor ctor = new();

            ctor.Statements.Add(new CodeMethodInvokeExpression(new CodeThisReferenceExpression(), InitMethodName));
            typeDecl.Members.Add(ctor);
        }

        return method;
    }

    /// <summary>
    ///  This method returns an array of methods that need to be interpreted during deserialization.
    ///  The default implementation returns a single element array with the constructor in it.
    /// </summary>
    protected override CodeMemberMethod[] GetInitializeMethods(IDesignerSerializationManager manager, CodeTypeDeclaration typeDecl)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(typeDecl);

        foreach (CodeTypeMember member in typeDecl.Members)
        {
            // Note: the order is important here for performance!
            // method.Parameters causes OnMethodPopulateParameters callback and therefore it is much more
            // expensive than method.Name.Equals

            if (member is CodeMemberMethod method && method.Name.Equals(InitMethodName) && method.Parameters.Count == 0)
            {
                return
                [
                    method
                ];
            }
        }

        return [];
    }
}
