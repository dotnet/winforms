// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  A MemberCodeDomSerializer for properties.
/// </summary>
internal sealed class PropertyMemberCodeDomSerializer : MemberCodeDomSerializer
{
    private static PropertyMemberCodeDomSerializer? s_default;

    internal static PropertyMemberCodeDomSerializer Default => s_default ??= new PropertyMemberCodeDomSerializer();

    /// <summary>
    ///  This retrieves the value of this property. If the property returns false
    ///  from ShouldSerializeValue (indicating the ambient value for this property)
    ///  This will look for an AmbientValueAttribute and use it if it can.
    /// </summary>
    private static object? GetPropertyValue(IDesignerSerializationManager manager, PropertyDescriptor property, object value, out bool validValue)
    {
        object? propertyValue = null;
        validValue = true;
        try
        {
            if (!property.ShouldSerializeValue(value))
            {
                // We aren't supposed to be serializing this property, but we decided to do
                // it anyway. Check the property for an AmbientValue attribute and if we
                // find one, use it's value to serialize.
                if (property.TryGetAttribute(out AmbientValueAttribute? attr))
                {
                    return attr.Value;
                }
                else if (property.TryGetAttribute(out DefaultValueAttribute? defAttr))
                {
                    return defAttr.Value;
                }
                else
                {
                    // nope, we're not valid...
                    //
                    validValue = false;
                }
            }

            propertyValue = property.GetValue(value);
        }
        catch (Exception e)
        {
            // something failed -- we don't have a valid value
            validValue = false;

            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerPropertyGenFailed, property.Name, e.Message), manager));
        }

        if (propertyValue is not null and not Type && (!propertyValue.GetType().IsValueType))
        {
            // DevDiv2 (Dev11) bug 187766 : property whose type implements ISupportInitialize is not
            // serialized with Begin/EndInit.
            Type type = TypeDescriptor.GetProvider(propertyValue).GetReflectionType(typeof(object));
            if (!type.IsDefined(typeof(ProjectTargetFrameworkAttribute), false))
            {
                // TargetFrameworkProvider is not attached
                TypeDescriptionProvider? typeProvider = GetTargetFrameworkProvider(manager, propertyValue);
                if (typeProvider is not null)
                {
                    TypeDescriptor.AddProvider(typeProvider, propertyValue);
                }
            }
        }

        return propertyValue;
    }

    /// <summary>
    ///  This method actually performs the serialization. When the member is serialized
    ///  the necessary statements will be added to the statements collection.
    /// </summary>
    public override void Serialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor, CodeStatementCollection statements)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(statements);

        if (descriptor is not PropertyDescriptor propertyToSerialize)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        try
        {
            ExtenderProvidedPropertyAttribute? exAttr = propertyToSerialize.GetAttribute<ExtenderProvidedPropertyAttribute>();
            bool isExtender = exAttr?.Provider is not null;
            bool serializeContents = propertyToSerialize.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content);

            if (serializeContents)
            {
                SerializeContentProperty(manager, value, propertyToSerialize, isExtender, statements);
            }
            else if (isExtender)
            {
                SerializeExtenderProperty(manager, value, propertyToSerialize, statements);
            }
            else
            {
                SerializeNormalProperty(manager, value, propertyToSerialize, statements);
            }
        }
        catch (Exception e)
        {
            // Since we usually go through reflection, don't
            // show what our engine does, show what caused
            // the problem.
            if (e is TargetInvocationException)
            {
                e = e.InnerException!;
            }

            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerPropertyGenFailed, propertyToSerialize.Name, e.Message), manager));
        }
    }

    /// <summary>
    ///  This serializes the given property on this object as a content property.
    /// </summary>
    private void SerializeContentProperty(IDesignerSerializationManager manager, object value, PropertyDescriptor property, bool isExtender, CodeStatementCollection statements)
    {
        object? propertyValue = GetPropertyValue(manager, property, value, out _);

        // For persist contents objects, we don't just serialize the properties on the object; we serialize everything.
        if (propertyValue is null)
        {
            string? name = manager.GetName(value);

            name ??= value.GetType().FullName;

            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerNullNestedProperty, name, property.Name), manager));
        }
        else if (manager.TryGetSerializer(propertyValue.GetType(), out CodeDomSerializer? serializer))
        {
            // Create a property reference expression and push it on the context stack.
            // This allows the serializer to gain some context as to what it should be
            // serializing.
            CodeExpression? target = SerializeToExpression(manager, value);

            if (target is not null)
            {
                CodeExpression? propertyRef = null;

                if (isExtender)
                {
                    ExtenderProvidedPropertyAttribute exAttr = property.GetAttribute<ExtenderProvidedPropertyAttribute>()!;

                    // Extender properties are method invokes on a target "extender" object.
                    CodeExpression? extender = SerializeToExpression(manager, exAttr.Provider);
                    CodeExpression? extended = SerializeToExpression(manager, value);

                    if (extender is not null && extended is not null)
                    {
                        CodeMethodReferenceExpression methodRef = new(extender, $"Get{property.Name}");
                        CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression
                        {
                            Method = methodRef
                        };
                        methodInvoke.Parameters.Add(extended);
                        propertyRef = methodInvoke;
                    }
                }
                else
                {
                    propertyRef = new CodePropertyReferenceExpression(target, property.Name);
                }

                if (propertyRef is not null)
                {
                    ExpressionContext tree = new(propertyRef, property.PropertyType, value, propertyValue);
                    manager.Context.Push(tree);

                    object? result = null;

                    try
                    {
                        SerializeAbsoluteContext absolute = manager.GetContext<SerializeAbsoluteContext>()!;

                        if (IsSerialized(manager, propertyValue, absolute is not null))
                        {
                            result = GetExpression(manager, propertyValue);
                        }
                        else
                        {
                            result = serializer.Serialize(manager, propertyValue);
                        }
                    }
                    finally
                    {
                        Debug.Assert(manager.Context.Current == tree, "Serializer added a context it didn't remove.");
                        manager.Context.Pop();
                    }

                    if (result is CodeStatementCollection csc)
                    {
                        statements.AddRange(csc);
                    }
                    else if (result is CodeStatement cs)
                    {
                        statements.Add(cs);
                    }
                }
            }
        }
        else
        {
            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerNoSerializerForComponent, property.PropertyType.FullName), manager));
        }
    }

    /// <summary>
    ///  This serializes the given property on this object.
    /// </summary>
    private void SerializeExtenderProperty(IDesignerSerializationManager manager, object value, PropertyDescriptor property, CodeStatementCollection statements)
    {
        ExtenderProvidedPropertyAttribute exAttr = property.GetAttribute<ExtenderProvidedPropertyAttribute>()!;

        // Extender properties are method invokes on a target "extender" object.
        CodeExpression? extender = SerializeToExpression(manager, exAttr.Provider);
        CodeExpression? extended = SerializeToExpression(manager, value);

        if (extender is not null && extended is not null)
        {
            CodeMethodReferenceExpression methodRef = new(extender, $"Set{property.Name}");
            object? propValue = GetPropertyValue(manager, property, value, out bool validValue);
            CodeExpression? serializedPropertyValue = null;

            // Serialize the value of this property into a code expression. If we can't get one,
            // then we won't serialize the property.
            if (validValue)
            {
                ExpressionContext? tree = null;

                if (propValue != value)
                {
                    // make sure the value isn't the object or we'll end up printing
                    // this property instead of the value.
                    tree = new ExpressionContext(methodRef, property.PropertyType, value);
                    manager.Context.Push(tree);
                }

                try
                {
                    serializedPropertyValue = SerializeToExpression(manager, propValue);
                }
                finally
                {
                    if (tree is not null)
                    {
                        Debug.Assert(manager.Context.Current == tree, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }
            }

            if (serializedPropertyValue is not null)
            {
                CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression
                {
                    Method = methodRef
                };
                methodInvoke.Parameters.Add(extended);
                methodInvoke.Parameters.Add(serializedPropertyValue);
                statements.Add(methodInvoke);
            }
        }
    }

    /// <summary>
    ///  This serializes the given property on this object.
    /// </summary>
    private void SerializeNormalProperty(IDesignerSerializationManager manager, object value, PropertyDescriptor property, CodeStatementCollection statements)
    {
        CodeExpression? target = SerializeToExpression(manager, value);

        if (target is not null)
        {
            CodeExpression propertyRef = new CodePropertyReferenceExpression(target, property.Name);

            CodeExpression? serializedPropertyValue = null;

            // First check for a member relationship service to see if this property
            // is related to another member. If it is, then we will use that
            // relationship to construct the property assign statement. if
            // it isn't, then we're serialize ourselves.

            MemberRelationshipService? relationships = manager.GetService<MemberRelationshipService>();

            if (relationships is not null)
            {
                MemberRelationship relationship = relationships[value, property];

                if (relationship != MemberRelationship.Empty)
                {
                    CodeExpression? rhsTarget = SerializeToExpression(manager, relationship.Owner);

                    if (rhsTarget is not null)
                    {
                        serializedPropertyValue = new CodePropertyReferenceExpression(rhsTarget, relationship.Member.Name);
                    }
                }
            }

            if (serializedPropertyValue is null)
            {
                // Serialize the value of this property into a code expression. If we can't get one,
                // then we won't serialize the property.
                //
                object? propValue = GetPropertyValue(manager, property, value, out bool validValue);

                if (validValue)
                {
                    ExpressionContext? tree = null;

                    if (propValue != value)
                    {
                        // make sure the value isn't the object or we'll end up printing
                        // this property instead of the value.
                        tree = new ExpressionContext(propertyRef, property.PropertyType, value);
                        manager.Context.Push(tree);
                    }

                    try
                    {
                        serializedPropertyValue = SerializeToExpression(manager, propValue);
                    }
                    finally
                    {
                        if (tree is not null)
                        {
                            Debug.Assert(manager.Context.Current == tree, "Context stack corrupted.");
                            manager.Context.Pop();
                        }
                    }
                }
            }

            if (serializedPropertyValue is not null)
            {
                CodeAssignStatement assign = new(propertyRef, serializedPropertyValue);
                statements.Add(assign);
            }
        }
    }

    /// <summary>
    ///  This method returns true if the given member descriptor should be serialized,
    ///  or false if there is no need to serialize the member.
    /// </summary>
    public override bool ShouldSerialize(IDesignerSerializationManager manager, object value, MemberDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        if (descriptor is not PropertyDescriptor propertyToSerialize)
        {
            throw new ArgumentNullException(nameof(descriptor));
        }

        bool shouldSerializeProperty = propertyToSerialize.ShouldSerializeValue(value);

        if (!shouldSerializeProperty)
        {
            if (manager.TryGetContext(out SerializeAbsoluteContext? absolute) && absolute.ShouldSerialize(propertyToSerialize))
            {
                // For ReadOnly properties, we only want to override the value returned from
                // ShouldSerializeValue() if the property is marked with DesignerSerializationVisibilityAttribute(Content).
                // Consider the case of a property with just a getter - we only want to serialize those
                // if they're marked in this way (see ReflectPropertyDescriptor::ShouldSerializeValue())
                if (!propertyToSerialize.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content))
                {
                    shouldSerializeProperty = false; // it's already false at this point, but this is clearer.
                }
                else
                {
                    shouldSerializeProperty = true; // Always serialize difference properties
                }
            }
        }

        if (shouldSerializeProperty)
        {
            bool isDesignTime = propertyToSerialize.Attributes.Contains(DesignOnlyAttribute.Yes);
            if (!isDesignTime)
            {
                return true;
            }
        }

        // If we don't have to serialize, we need to make sure there isn't a member
        // relationship with this property. If there is, we still need to serialize.

        MemberRelationshipService? relationships = manager.GetService<MemberRelationshipService>();

        if (relationships is not null)
        {
            MemberRelationship relationship = relationships[value, descriptor];

            if (relationship != MemberRelationship.Empty)
            {
                return true;
            }
        }

        return false;
    }
}
