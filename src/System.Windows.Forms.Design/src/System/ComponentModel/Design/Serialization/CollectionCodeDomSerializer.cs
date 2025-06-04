// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.Reflection;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This serializer serializes collections. This can either create statements or expressions.
///  It will create an expression and assign it to the statement in the current context stack if the object is an array.
///  If it is a collection with an add range or similar method, it will create a statement calling the method.
/// </summary>
public class CollectionCodeDomSerializer : CodeDomSerializer
{
    private static CollectionCodeDomSerializer? s_defaultSerializer;

    /// <summary>
    ///  Retrieves a default static instance of this serializer.
    /// </summary>
    internal static new CollectionCodeDomSerializer Default => s_defaultSerializer ??= new CollectionCodeDomSerializer();

    /// <summary>
    ///  Computes the delta between an existing collection and a modified one.
    ///  This is for the case of inherited items that have collection properties so we only
    ///  generate Add/AddRange calls for the items that have been added.
    ///  It works by Hashing up the items in the original collection and then walking the modified collection
    ///  and only returning those items which do not exist in the base collection.
    /// </summary>
    [return: NotNullIfNotNull(nameof(modified))]
    private static ICollection? GetCollectionDelta(ICollection? original, ICollection? modified)
    {
        if (original is null || modified is null || original.Count == 0)
        {
            return modified;
        }

        IEnumerator modifiedEnum = modified.GetEnumerator();
        if (modifiedEnum is null)
        {
            Debug.Fail($"Collection of type {modified.GetType().FullName} doesn't return an enumerator");
            return modified;
        }

        // first hash up the values so we can quickly decide if it's a new one or not
        Dictionary<object, int> originalValues = [];
        foreach (object originalValue in original)
        {
            // the array could contain multiple copies of the same value (think of a string collection), so we need to be sensitive of that.
            if (originalValues.TryGetValue(originalValue, out int count))
            {
                originalValues[originalValue] = count + 1;
            }
            else
            {
                originalValues.Add(originalValue, 1);
            }
        }

        // now walk through and delete existing values
        List<object>? result = null;
        // now compute the delta.
        for (int i = 0; i < modified.Count && modifiedEnum.MoveNext(); i++)
        {
            object value = modifiedEnum.Current!;

            if (originalValues.TryGetValue(value, out int count))
            {
                // we've got one we need to remove, so create our array list, and push all the values we've passed into it.
                if (result is null)
                {
                    result = [];
                    modifiedEnum.Reset();
                    for (int n = 0; n < i && modifiedEnum.MoveNext(); n++)
                    {
                        result.Add(modifiedEnum.Current!);
                    }

                    // and finally skip the one we're on
                    modifiedEnum.MoveNext();
                }

                // decrement the count if we've got more than one...
                if (--count == 0)
                {
                    originalValues.Remove(value);
                }
                else
                {
                    originalValues[value] = count;
                }
            }
            else // this one isn't in the old list, so add it to our result list.
            {
                result?.Add(value);
            }

            // this item isn't in the list and we haven't yet created our array list so just keep on going.
        }

        return result ?? modified;
    }

    /// <summary>
    ///  Checks the attributes on this method to see if they support serialization.
    /// </summary>
    protected bool MethodSupportsSerialization(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);

        object[] attributes = method.GetCustomAttributes(typeof(DesignerSerializationVisibilityAttribute), true);
        if (attributes.Length > 0)
        {
            DesignerSerializationVisibilityAttribute visibility = (DesignerSerializationVisibilityAttribute)attributes[0];
            if (visibility is { Visibility: DesignerSerializationVisibility.Hidden })
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    /// </summary>
    public override object? Serialize(IDesignerSerializationManager manager, object value)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        object? result = null;

        // We serialize collections as follows:
        // If the collection is an array, we write out the array.
        // If the collection has a method called AddRange, we will call that, providing an array.
        // If the collection has an Add method, we will call it repeatedly.
        // If the collection is an IList, we will cast to IList and add to it.
        // If the collection has no add method, but is marked with PersistContents,
        // we will enumerate the collection and serialize each element.
        // Check to see if there is a CodePropertyReferenceExpression on the stack.
        // If there is, we can use it as a guide for serialization.
        CodeExpression? target;
        if (manager.TryGetContext(out ExpressionContext? context) && context.PresetValue == value &&
            manager.TryGetContext(out PropertyDescriptor? property) && property.PropertyType == context.ExpressionType)
        {
            // We only want to give out an expression target if this is our context
            // (we find this out by comparing types above) and if the context type is not an array. If it is an array,
            // we will just return the array create expression.
            target = context.Expression;
        }
        else
        {
            // This context is either the wrong context or doesn't match the property descriptor we found.
            target = null;
            context = null;
            property = null;
        }

        // If we have a target expression see if we can create a delta for the collection. We want to do this only if the
        // property the collection is associated with is inherited, and if the collection is not an array.
        if (value is ICollection collection)
        {
            ICollection subset = collection;
            Type collectionType = context?.ExpressionType ?? collection.GetType();
            bool isArray = typeof(Array).IsAssignableFrom(collectionType);

            // If we don't have a target expression and this isn't an array, let's try to create one.
            if (target is null && !isArray)
            {
                target = SerializeCreationExpression(manager, collection, out bool isComplete);
                if (isComplete)
                {
                    return target;
                }
            }

            if (target is not null || isArray)
            {
                if (property is InheritedPropertyDescriptor inheritedDesc && !isArray)
                {
                    subset = GetCollectionDelta(inheritedDesc.OriginalValue as ICollection, collection);
                }

                result = SerializeCollection(manager, target, collectionType, collection, subset);

                // See if we should emit a clear for this collection.
                if (target is not null && ShouldClearCollection(manager, collection))
                {
                    CodeStatementCollection? resultCollection = result as CodeStatementCollection;

                    // If non empty collection is being serialized, but no statements were generated, there is no need to clear.
                    if (collection.Count > 0 && (result is null || (resultCollection is not null && resultCollection.Count == 0)))
                    {
                        return null;
                    }

                    if (resultCollection is null)
                    {
                        resultCollection = [];
                        if (result is CodeStatement resultStatement)
                        {
                            resultCollection.Add(resultStatement);
                        }

                        result = resultCollection;
                    }

                    CodeMethodInvokeExpression clearMethod = new(target, "Clear");
                    CodeExpressionStatement clearStatement = new(clearMethod);
                    resultCollection.Insert(0, clearStatement);
                }
            }
        }
        else
        {
            Debug.Fail($"Collection serializer invoked for non-collection: {(value is null ? "(null)" : value.GetType().Name)}");
        }

        return result;
    }

    /// <summary>
    ///  Given a set of methods and objects, determines the method with the correct of parameter type for all objects.
    /// </summary>
    private static MethodInfo? ChooseMethodByType(TypeDescriptionProvider provider, List<MethodInfo> methods, ICollection values)
    {
        // Note that this method uses reflection types which may not be compatible with runtime types. objType must be
        // obtained from the same provider as the methods were to ensure that the reflection types all belong to the
        // same type universe.
        MethodInfo? final = null;
        Type? finalType = null;
        foreach (object obj in values)
        {
            Type objType = provider.GetReflectionType(obj);
            MethodInfo? candidate = null;
            Type? candidateType = null;
            if (final is null || (finalType is not null && !finalType.IsAssignableFrom(objType)))
            {
                foreach (MethodInfo method in methods)
                {
                    ParameterInfo parameter = method.GetParameters()[0];
                    if (parameter is not null)
                    {
                        Type type = parameter.ParameterType;
                        if (type.IsArray)
                        {
                            type = type.GetElementType()!;
                        }

                        if (type.IsAssignableFrom(objType))
                        {
                            if (final is not null)
                            {
                                if (type.IsAssignableFrom(finalType))
                                {
                                    final = method;
                                    finalType = type;
                                    break;
                                }
                            }
                            else if (candidate is null)
                            {
                                candidate = method;
                                candidateType = type;
                            }
                            else
                            {
                                // we found another method. Pick the one that uses the most derived type.
                                Debug.Assert(candidateType!.IsAssignableFrom(type) || type.IsAssignableFrom(candidateType), "These two types are not related, how were they chosen based on the base type");
                                bool assignable = candidateType.IsAssignableFrom(type);
                                candidate = assignable ? method : candidate;
                                candidateType = assignable ? type : candidateType;
                            }
                        }
                    }
                }
            }

            if (final is null)
            {
                final = candidate;
                finalType = candidateType;
            }
        }

        return final;
    }

    /// <summary>
    ///  Serializes the given collection. targetExpression will refer to the expression used
    ///  to refer to the collection, but it can be null.
    /// </summary>
    protected virtual object? SerializeCollection(IDesignerSerializationManager manager, CodeExpression? targetExpression, Type targetType, ICollection originalCollection, ICollection valuesToSerialize)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(targetType);
        ArgumentNullException.ThrowIfNull(originalCollection);
        ArgumentNullException.ThrowIfNull(valuesToSerialize);

        object? result = null;
        bool serialized = false;
        if (typeof(Array).IsAssignableFrom(targetType))
        {
            CodeArrayCreateExpression? arrayCreate = SerializeArray(manager, targetType, (Array)originalCollection, valuesToSerialize);
            if (arrayCreate is not null)
            {
                if (targetExpression is not null)
                {
                    result = new CodeAssignStatement(targetExpression, arrayCreate);
                }
                else
                {
                    result = arrayCreate;
                }
            }
        }
        else if (valuesToSerialize.Count > 0)
        {
            // Use the TargetFrameworkProviderService to create a provider, or use the default for the collection if the
            // service is not available. Since TargetFrameworkProvider reflection types are not compatible with RuntimeTypes,
            // they can only be used with other reflection types from the same provider.
            TypeDescriptionProvider? provider = GetTargetFrameworkProvider(manager, originalCollection);
            provider ??= TypeDescriptor.GetProvider(originalCollection);

            MethodInfo[] methods = provider.GetReflectionType(originalCollection).GetMethods(BindingFlags.Public | BindingFlags.Instance);
            List<MethodInfo> addRangeMethods = [];
            List<MethodInfo> addMethods = [];
            foreach (MethodInfo method in methods)
            {
                switch (method.Name)
                {
                    case "AddRange":
                        ParameterInfo[] parameters = method.GetParameters();
                        if (parameters is [{ ParameterType.IsArray: true }] && MethodSupportsSerialization(method))
                        {
                            addRangeMethods.Add(method);
                        }

                        break;

                    case "Add":
                        if (method.GetParameters().Length == 1 && MethodSupportsSerialization(method))
                        {
                            addMethods.Add(method);
                        }

                        break;
                }
            }

            MethodInfo? addRangeMethodToUse = ChooseMethodByType(provider, addRangeMethods, valuesToSerialize);
            if (addRangeMethodToUse is not null)
            {
                Type elementType = provider.GetRuntimeType(addRangeMethodToUse.GetParameters()[0].ParameterType.GetElementType()!);
                result = SerializeViaAddRange(manager, targetExpression, elementType, valuesToSerialize);
                serialized = true;
            }
            else
            {
                MethodInfo? addMethodToUse = ChooseMethodByType(provider, addMethods, valuesToSerialize);
                if (addMethodToUse is not null)
                {
                    Type elementType = provider.GetRuntimeType(addMethodToUse.GetParameters()[0].ParameterType);
                    result = SerializeViaAdd(manager, targetExpression, elementType, valuesToSerialize);
                    serialized = true;
                }
            }

#pragma warning disable SYSLIB0050 // Type or member is obsolete
            if (!serialized && originalCollection.GetType().IsSerializable)
            {
                result = SerializeToResourceExpression(manager, originalCollection, false);
            }
#pragma warning restore SYSLIB0050
        }

        return result;
    }

    /// <summary>
    ///  Serializes the given array.
    /// </summary>
    private CodeArrayCreateExpression? SerializeArray(IDesignerSerializationManager manager, Type targetType, Array array, ICollection valuesToSerialize)
    {
        CodeArrayCreateExpression? result = null;

        if (array.Rank != 1)
        {
            manager.ReportError(string.Format(SR.SerializerInvalidArrayRank, array.Rank.ToString(CultureInfo.InvariantCulture)));
        }
        else
        {
            // For an array, we need an array create expression. First, get the array type
            Type elementType = targetType.GetElementType()!;
            CodeTypeReference elementTypeRef = new(elementType);

            // Now create an ArrayCreateExpression, and fill its initializers.
            CodeArrayCreateExpression arrayCreate = new CodeArrayCreateExpression
            {
                CreateType = elementTypeRef
            };

            bool arrayOk = true;
            foreach (object o in valuesToSerialize)
            {
                // If this object is being privately inherited, it cannot be inside this collection.
                // Since we're writing an entire array here, we cannot write any of it.
                if (o is IComponent && TypeDescriptor.GetAttributes(o).Contains(InheritanceAttribute.InheritedReadOnly))
                {
                    arrayOk = false;
                    break;
                }

                CodeExpression? expression = null;
                // If there is an expression context on the stack at this point, we need to fix up the ExpressionType
                // on it to be the array element type.
                ExpressionContext? newContext = null;
                if (manager.TryGetContext(out ExpressionContext? context))
                {
                    newContext = new ExpressionContext(context.Expression, elementType, context.Owner);
                    manager.Context.Push(newContext);
                }

                try
                {
                    expression = SerializeToExpression(manager, o);
                }
                finally
                {
                    if (newContext is not null)
                    {
                        Debug.Assert(manager.Context.Current == newContext, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }

                if (expression is not null)
                {
                    if (o is not null && o.GetType() != elementType)
                    {
                        expression = new CodeCastExpression(elementType, expression);
                    }

                    arrayCreate.Initializers.Add(expression);
                }
                else
                {
                    arrayOk = false;
                    break;
                }
            }

            if (arrayOk)
            {
                result = arrayCreate;
            }
        }

        return result;
    }

    /// <summary>
    ///  Serializes the given collection by creating multiple calls to an Add method.
    /// </summary>
    private CodeStatementCollection SerializeViaAdd(
        IDesignerSerializationManager manager,
        CodeExpression? targetExpression,
        Type elementType,
        ICollection valuesToSerialize)
    {
        CodeStatementCollection statements = [];

        // Here we need to invoke Add once for each and every item in the collection. We can re-use the property
        // reference and method reference, but we will need to recreate the invoke statement each time.
        CodeMethodReferenceExpression methodRef = new(targetExpression!, "Add");

        if (valuesToSerialize.Count == 0)
        {
            return statements;
        }

        foreach (object o in valuesToSerialize)
        {
            // If this object is being privately inherited, it cannot be inside this collection.
            bool genCode = o is not IComponent;
            if (!genCode)
            {
                if (TypeDescriptorHelper.TryGetAttribute(o, out InheritanceAttribute? ia))
                {
                    genCode = ia.InheritanceLevel != InheritanceLevel.InheritedReadOnly;
                }
                else
                {
                    genCode = true;
                }
            }

            Debug.Assert(genCode, "Why didn't GetCollectionDelta calculate the same thing?");
            if (genCode)
            {
                CodeMethodInvokeExpression statement = new CodeMethodInvokeExpression
                {
                    Method = methodRef
                };

                CodeExpression? serializedObject = null;

                // If there is an expression context on the stack at this point,
                // we need to fix up the ExpressionType on it to be the element type.
                ExpressionContext? newCtx = null;

                if (manager.TryGetContext(out ExpressionContext? ctx))
                {
                    newCtx = new ExpressionContext(ctx.Expression, elementType, ctx.Owner);
                    manager.Context.Push(newCtx);
                }

                try
                {
                    serializedObject = SerializeToExpression(manager, o);
                }
                finally
                {
                    if (newCtx is not null)
                    {
                        Debug.Assert(manager.Context.Current == newCtx, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }

                if (o is not null && !elementType.IsAssignableFrom(o.GetType()) && o.GetType().IsPrimitive)
                {
                    serializedObject = new CodeCastExpression(elementType, serializedObject!);
                }

                if (serializedObject is not null)
                {
                    statement.Parameters.Add(serializedObject);
                    statements.Add(statement);
                }
            }
        }

        return statements;
    }

    /// <summary>
    ///  Serializes the given collection by creating an array and passing it to the AddRange method.
    /// </summary>
    private CodeStatementCollection SerializeViaAddRange(
        IDesignerSerializationManager manager,
        CodeExpression? targetExpression,
        Type elementType,
        ICollection valuesToSerialize)
    {
        CodeStatementCollection statements = [];

        if (valuesToSerialize.Count == 0)
        {
            return statements;
        }

        List<CodeExpression> arrayList = new(valuesToSerialize.Count);
        foreach (object o in valuesToSerialize)
        {
            // If this object is being privately inherited, it cannot be inside this collection.
            bool genCode = o is not IComponent;
            if (!genCode)
            {
                if (TypeDescriptorHelper.TryGetAttribute(o, out InheritanceAttribute? ia))
                {
                    genCode = ia.InheritanceLevel != InheritanceLevel.InheritedReadOnly;
                }
                else
                {
                    genCode = true;
                }
            }

            Debug.Assert(genCode, "Why didn't GetCollectionDelta calculate the same thing?");
            if (genCode)
            {
                CodeExpression? expression = null;
                // If there is an expression context on the stack at this point, we need to fix up the ExpressionType
                // on it to be the element type.
                ExpressionContext? newContext = null;

                if (manager.TryGetContext(out ExpressionContext? ctx))
                {
                    newContext = new ExpressionContext(ctx.Expression, elementType, ctx.Owner);
                    manager.Context.Push(newContext);
                }

                try
                {
                    expression = SerializeToExpression(manager, o);
                }
                finally
                {
                    if (newContext is not null)
                    {
                        Debug.Assert(manager.Context.Current == newContext, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }

                if (expression is not null)
                {
                    // Check to see if we need a cast
                    if (o is not null && !elementType.IsAssignableFrom(o.GetType()))
                    {
                        expression = new CodeCastExpression(elementType, expression);
                    }

                    arrayList.Add(expression);
                }
            }
        }

        if (arrayList.Count > 0)
        {
            // Now convert the array list into an array create expression.
            CodeTypeReference elementTypeRef = new(elementType);

            // Now create an ArrayCreateExpression, and fill its initializers.
            CodeArrayCreateExpression arrayCreate = new CodeArrayCreateExpression
            {
                CreateType = elementTypeRef
            };

            foreach (CodeExpression expression in arrayList)
            {
                arrayCreate.Initializers.Add(expression);
            }

            CodeMethodReferenceExpression methodRef = new(targetExpression!, "AddRange");
            CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression
            {
                Method = methodRef
            };

            methodInvoke.Parameters.Add(arrayCreate);
            statements.Add(new CodeExpressionStatement(methodInvoke));
        }

        return statements;
    }

    /// <summary>
    ///  Returns true if we should clear the collection contents.
    /// </summary>
    private bool ShouldClearCollection(IDesignerSerializationManager manager, ICollection collection)
    {
        bool shouldClear = false;
        PropertyDescriptor? clearProperty = manager.Properties["ClearCollections"];
        if (clearProperty is not null && clearProperty.TryGetValue(manager, out bool b) && b)
        {
            shouldClear = true;
        }

        if (!shouldClear)
        {
            PropertyDescriptor? property = manager.GetContext<PropertyDescriptor>();
            if (manager.TryGetContext(out SerializeAbsoluteContext? absolute) && absolute.ShouldSerialize(property))
            {
                shouldClear = true;
            }
        }

        if (shouldClear)
        {
            MethodInfo? clearMethod = TypeDescriptor.GetReflectionType(collection).GetMethod("Clear", BindingFlags.Public | BindingFlags.Instance, null, [], null);
            if (clearMethod is null || !MethodSupportsSerialization(clearMethod))
            {
                shouldClear = false;
            }
        }

        return shouldClear;
    }
}
