// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  The is a base class that can be used to serialize an object graph to a series of
///  CodeDom statements.
/// </summary>
[DefaultSerializationProvider(typeof(CodeDomSerializationProvider))]
public class CodeDomSerializer : CodeDomSerializerBase
{
    private static CodeDomSerializer? s_default;
    private static readonly Attribute[] s_runTimeFilter = [DesignOnlyAttribute.No];
    private static readonly Attribute[] s_designTimeFilter = [DesignOnlyAttribute.Yes];
    private static readonly Attribute[] s_deserializeFilter = [BrowsableAttribute.Yes];
    private static readonly CodeThisReferenceExpression s_thisRef = new();

    internal static CodeDomSerializer Default => s_default ??= new CodeDomSerializer();

    /// <summary>
    ///  Determines which statement group the given statement should belong to. The expression parameter
    ///  is an expression that the statement has been reduced to, and targetType represents the type
    ///  of this statement. This method returns the name of the component this statement should be grouped
    ///  with.
    /// </summary>
    public virtual string? GetTargetComponentName(CodeStatement? statement, CodeExpression? expression, Type? targetType)
    {
        return expression switch
        {
            CodeVariableReferenceExpression variableReferenceEx => variableReferenceEx.VariableName,
            CodeFieldReferenceExpression fieldReferenceEx => fieldReferenceEx.FieldName,
            _ => null,
        };
    }

    /// <summary>
    ///  Deserializes the given CodeDom object into a real object. This
    ///  will use the serialization manager to create objects and resolve
    ///  data types. The root of the object graph is returned.
    /// </summary>
    public virtual object? Deserialize(IDesignerSerializationManager manager, object codeObject)
    {
        object? instance = null;
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(codeObject);

        // What is the code object?  We support an expression, a statement or a collection of statements
        if (codeObject is CodeExpression expression)
        {
            instance = DeserializeExpression(manager, null, expression);
        }
        else if (codeObject is CodeStatementCollection statements)
        {
            foreach (CodeStatement element in statements)
            {
                // If we do not yet have an instance, we will need to pick through the statements
                // and see if we can find one.
                if (instance is null)
                {
                    instance = DeserializeStatementToInstance(manager, element);
                    if (instance is not null)
                    {
                        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance, s_deserializeFilter);
                        foreach (PropertyDescriptor prop in props)
                        {
                            if (!prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden) &&
                                prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content) &&
                                manager.GetSerializer(prop.PropertyType, typeof(CodeDomSerializer)) is not CollectionCodeDomSerializer)
                            {
                                ResetBrowsableProperties(prop.GetValue(instance));
                            }
                        }
                    }
                }
                else
                {
                    DeserializeStatement(manager, element);
                }
            }
        }
        else if (codeObject is not CodeStatement)
        {
            Debug.Fail("CodeDomSerializer::Deserialize requires a CodeExpression, CodeStatement or CodeStatementCollection to parse");
            string supportedTypes = $"{nameof(CodeExpression)}, {nameof(CodeStatement)}, {nameof(CodeStatementCollection)}";
            throw new ArgumentException(string.Format(SR.SerializerBadElementTypes, codeObject.GetType().Name, supportedTypes));
        }

        return instance;
    }

    /// <summary>
    ///  This method deserializes a single statement. It is equivalent of calling
    ///  DeserializeStatement, except that it returns an object instance if the
    ///  resulting statement was a variable assign statement, a variable
    ///  declaration with an init expression, or a field assign statement.
    /// </summary>
    protected object? DeserializeStatementToInstance(IDesignerSerializationManager manager, CodeStatement statement)
    {
        object? instance = null;
        if (statement is CodeAssignStatement assign)
        {
            if (assign.Left is CodeFieldReferenceExpression fieldRef)
            {
                instance = DeserializeExpression(manager, fieldRef.FieldName, assign.Right);
            }
            else if (assign.Left is CodeVariableReferenceExpression varRef)
            {
                instance = DeserializeExpression(manager, varRef.VariableName, assign.Right);
            }
            else
            {
                DeserializeStatement(manager, assign);
            }
        }
        else if (statement is CodeVariableDeclarationStatement { InitExpression: not null } varDecl)
        {
            instance = DeserializeExpression(manager, varDecl.Name, varDecl.InitExpression);
        }
        else
        {
            // This statement isn't one that will return a named object. Deserialize it normally.
            DeserializeStatement(manager, statement);
        }

        return instance;
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    /// </summary>
    public virtual object? Serialize(IDesignerSerializationManager manager, object value)
    {
        object? result = null;
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        if (value is Type type)
        {
            result = new CodeTypeOfExpression(type);
        }
        else
        {
            bool isComplete = false;
            CodeExpression? expression = SerializeCreationExpression(manager, value, out bool isCompleteExpression);

            // If the object is not a component we will honor the return value from SerializeCreationExpression.
            // For compat reasons we ignore the value if the object is a component.
            if (value is not IComponent)
            {
                isComplete = isCompleteExpression;
            }

            // Short circuit common cases
            if (expression is not null)
            {
                if (isComplete)
                {
                    result = expression;
                }
                else
                {
                    // Ok, we have an incomplete expression. That means we've created the object but we will need to set
                    // properties on it to configure it. Therefore, we need to have a variable reference to it unless we
                    // were given a preset expression already.
                    CodeStatementCollection statements = [];

                    // We need to find out if SerializeCreationExpression returned a preset expression.
                    bool isPreset = manager.TryGetContext(out ExpressionContext? ctx) && ReferenceEquals(ctx.PresetValue, value);

                    if (isPreset)
                    {
                        SetExpression(manager, value, expression, true);
                    }
                    else
                    {
                        string varName = GetUniqueName(manager, value);
                        string? varTypeName = TypeDescriptor.GetClassName(value);

                        CodeVariableDeclarationStatement varDecl = new(varTypeName!, varName)
                        {
                            InitExpression = expression
                        };
                        statements.Add(varDecl);
                        CodeExpression variableReference = new CodeVariableReferenceExpression(varName);
                        SetExpression(manager, value, variableReference);
                    }

                    // Finally, we need to walk properties and events for this object
                    SerializePropertiesToResources(manager, statements, value, s_designTimeFilter);
                    SerializeProperties(manager, statements, value, s_runTimeFilter);
                    SerializeEvents(manager, statements, value, s_runTimeFilter);
                    result = statements;
                }
            }
        }

        return result;
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    /// </summary>
    public virtual object? SerializeAbsolute(IDesignerSerializationManager manager, object value)
    {
        object? data;
        SerializeAbsoluteContext abs = new();
        manager.Context.Push(abs);
        try
        {
            data = Serialize(manager, value);
        }
        finally
        {
            Debug.Assert(manager.Context.Current == abs, "Serializer added a context it didn't remove.");
            manager.Context.Pop();
        }

        return data;
    }

    /// <summary>
    ///  This serializes the given member on the given object.
    /// </summary>
    public virtual CodeStatementCollection SerializeMember(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(owningObject);
        ArgumentNullException.ThrowIfNull(member);

        CodeStatementCollection statements = [];

        // See if we have an existing expression for this member. If not, fabricate one
        CodeExpression? expression = GetExpression(manager, owningObject);
        if (expression is null)
        {
            string name = GetUniqueName(manager, owningObject);
            expression = new CodeVariableReferenceExpression(name);
            SetExpression(manager, owningObject, expression);
        }

        if (member is PropertyDescriptor property)
        {
            SerializeProperty(manager, statements, owningObject, property);
        }
        else if (member is EventDescriptor evt)
        {
            SerializeEvent(manager, statements, owningObject, evt);
        }
        else
        {
            throw new NotSupportedException(string.Format(SR.SerializerMemberTypeNotSerializable, member.GetType().FullName));
        }

        return statements;
    }

    /// <summary>
    ///  This serializes the given member on the given object.
    /// </summary>
    public virtual CodeStatementCollection SerializeMemberAbsolute(IDesignerSerializationManager manager, object owningObject, MemberDescriptor member)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(owningObject);
        ArgumentNullException.ThrowIfNull(member);

        CodeStatementCollection statements;
        SerializeAbsoluteContext abs = new(member);
        manager.Context.Push(abs);

        try
        {
            statements = SerializeMember(manager, owningObject, member);
        }
        finally
        {
            Debug.Assert(manager.Context.Current == abs, "Serializer added a context it didn't remove.");
            manager.Context.Pop();
        }

        return statements;
    }

    /// <summary>
    ///  This serializes the given value to an expression. It will return null if the value could not be
    ///  serialized. This is similar to SerializeToExpression, except that it will stop
    ///  if it cannot obtain a simple reference expression for the value. Call this method
    ///  when you expect the resulting expression to be used as a parameter or target
    ///  of a statement.
    /// </summary>
    [Obsolete("This method has been deprecated. Use SerializeToExpression or GetExpression instead.  https://go.microsoft.com/fwlink/?linkid=14202")]
    protected CodeExpression? SerializeToReferenceExpression(IDesignerSerializationManager manager, object value)
    {
        // First - try GetExpression
        if (GetExpression(manager, value) is { } expression)
        {
            return expression;
        }

        if (value is not IComponent)
        {
            return null;
        }

        // Next, we check for a named IComponent, and return a reference to it.
        string? name = manager.GetName(value);
        bool referenceName = false;
        if (name is null)
        {
            IReferenceService? referenceService = manager.GetService<IReferenceService>();
            if (referenceService is not null)
            {
                name = referenceService.GetName(value);
                referenceName = name is not null;
            }
        }

        if (name is null)
        {
            return null;
        }

        // Check to see if this is a reference to the root component. If it is, then use "this".
        if (manager.TryGetContext(out RootContext? root) && root.Value == value)
        {
            return root.Expression;
        }

        // If it's a reference name with a dot, we've actually got a property.

        int dotIndex = name.IndexOf('.');
        return referenceName && dotIndex != -1
            ? new CodePropertyReferenceExpression(
                new CodeFieldReferenceExpression(s_thisRef, name[..dotIndex]),
                name[(dotIndex + 1)..])
            : new CodeFieldReferenceExpression(s_thisRef, name);
    }

    private static void ResetBrowsableProperties(object? instance)
    {
        if (instance is null)
        {
            return;
        }

        PropertyDescriptorCollection props = TypeDescriptor.GetProperties(instance, [BrowsableAttribute.Yes]);
        foreach (PropertyDescriptor prop in props)
        {
            if (!prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
            {
                if (prop.CanResetValue(instance))
                {
                    try
                    {
                        prop.ResetValue(instance);
                    }
                    catch (ArgumentException e)
                    {
                        Debug.Assert(false, e.Message);
                    }
                }
                else if (prop.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
                {
                    ResetBrowsableProperties(prop.GetValue(instance));
                }
            }
        }
    }
}
