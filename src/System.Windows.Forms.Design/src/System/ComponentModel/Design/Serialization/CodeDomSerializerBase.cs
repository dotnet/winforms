// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Reflection;
using System.Text;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  This base class is used as a shared base between CodeDomSerializer and TypeCodeDomSerializer.
///  It is not meant to be publicly derived from.
/// </summary>
[EditorBrowsable(EditorBrowsableState.Never)]
public abstract partial class CodeDomSerializerBase
{
    private static readonly Attribute[] s_runTimeProperties = [DesignOnlyAttribute.No];

    /// <summary>
    ///  Internal constructor so only we can derive from this class.
    /// </summary>
    internal CodeDomSerializerBase()
    {
    }

    /// <summary>
    ///  This method is invoked during deserialization to obtain an instance of an object. When this is called, an instance
    ///  of the requested type should be returned. The default implementation invokes manager.CreateInstance.
    /// </summary>
    protected virtual object DeserializeInstance(IDesignerSerializationManager manager, Type type, object?[]? parameters, string? name, bool addToContainer)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(type);

        return manager.CreateInstance(type, parameters, name, addToContainer);
    }

    /// <summary>
    ///  This routine returns the correct typename given a CodeTypeReference. It expands the child typenames
    ///  and builds up the clr formatted generic name. If its not a generic, it just returns BaseType.
    /// </summary>
    internal static string GetTypeNameFromCodeTypeReference(IDesignerSerializationManager manager, CodeTypeReference typeref)
    {
        // we do this to avoid an extra gettype for the usual nongeneric case.
        if (typeref.TypeArguments is null || typeref.TypeArguments.Count == 0)
        {
            return typeref.BaseType;
        }

        StringBuilder typeName = new();
        GetTypeNameFromCodeTypeReferenceHelper(manager, typeref, typeName);
        return typeName.ToString();
    }

    private static void GetTypeNameFromCodeTypeReferenceHelper(IDesignerSerializationManager manager, CodeTypeReference typeref, StringBuilder typeName)
    {
        if (typeref.TypeArguments is null || typeref.TypeArguments.Count == 0)
        {
            Type? t = manager.GetType(typeref.BaseType);
            // we use the assemblyqualifiedname where we can so that GetType will find it correctly.
            if (t is not null)
            {
                // get type which exists in the target framework if any
                typeName.Append(GetReflectionTypeFromTypeHelper(manager, t).AssemblyQualifiedName);
            }
            else
            {
                typeName.Append(typeref.BaseType);
            }
        }
        else
        {
            // create the MyGeneric`2[ part
            if (!typeref.BaseType.Contains('`'))
            {
                typeName.Append($"`{typeref.TypeArguments.Count}");
            }

            typeName.Append('[');

            // now create each sub-argument part.
            foreach (CodeTypeReference childref in typeref.TypeArguments)
            {
                typeName.Append('[');
                GetTypeNameFromCodeTypeReferenceHelper(manager, childref, typeName);
                typeName.Append("],");
            }

            typeName[^1] = ']';
        }
    }

    /// <summary>
    ///  Return a target framework-aware TypeDescriptionProvider which can be used for type filtering
    /// </summary>
    protected static TypeDescriptionProvider? GetTargetFrameworkProvider(IServiceProvider provider, object instance)
    {
        // service will be null outside the VisualStudio
        if (provider.TryGetService(out TypeDescriptionProviderService? service))
        {
            return service.GetProvider(instance);
        }

        return null;
    }

    private static bool TryGetTargetFrameworkProviderAndCheckType(IDesignerSerializationManager manager, object instance, [NotNullWhen(true)] out TypeDescriptionProvider? targetProvider)
    {
        Type type = instance.GetType();
        if (!type.IsValueType)
        {
            targetProvider = null;
            return false;
        }

        targetProvider = GetTargetFrameworkProvider(manager, instance);

        if (targetProvider is null)
        {
            return false;
        }

        if (targetProvider.IsSupportedType(type))
        {
            return true;
        }

        Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, instance.GetType().FullName), SR.SerializerUndeclaredName);
        targetProvider = null;
        return false;
    }

    /// <summary>
    ///  Get a faux type which is generated from the metadata, which is
    ///  looked up on the target framework assembly. Be careful to not use mix
    ///  this type with runtime types in comparisons!
    /// </summary>
    protected static Type GetReflectionTypeFromTypeHelper(IDesignerSerializationManager manager, Type type)
    {
        if (type is null || manager is null)
        {
            Debug.Fail("GetReflectionTypeFromTypeHelper does not accept null arguments.");
            return null;
        }

        if (TryGetTargetFrameworkProviderForType(manager, type, out TypeDescriptionProvider? targetProvider))
        {
            if (targetProvider.IsSupportedType(type))
            {
                return targetProvider.GetReflectionType(type);
            }

            Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, type.FullName), SR.SerializerUndeclaredName);
        }

        return TypeDescriptor.GetReflectionType(type);
    }

    [DoesNotReturn]
    internal static void Error(IDesignerSerializationManager manager, string exceptionText, string? helpLink)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(exceptionText);

        CodeStatement? statement = manager.GetContext<CodeStatement>();
        CodeLinePragma? linePragma = statement?.LinePragma;

        Exception exception = new CodeDomSerializerException(exceptionText, linePragma)
        {
            HelpLink = helpLink
        };
        throw exception;
    }

    private static bool TryGetTargetFrameworkProviderForType(IServiceProvider provider, Type type, [NotNullWhen(true)] out TypeDescriptionProvider? targetProvider)
    {
        // service will be null outside the VisualStudio
        targetProvider = provider.GetService<TypeDescriptionProviderService>()?.GetProvider(type);
        return targetProvider is not null;
    }

    /// <summary>
    ///  Get a faux type which is generated based on the metadata
    ///  looked up on the target framework assembly.
    ///  Never pass a type returned from GetReflectionType to runtime APIs that need a type.
    ///  Call GetRuntimeType first to unwrap the reflection type.
    /// </summary>
    protected static Type GetReflectionTypeHelper(IDesignerSerializationManager manager, object instance)
    {
        if (instance is null || manager is null)
        {
            Debug.Fail("GetReflectionTypeHelper does not accept null arguments.");
            return null;
        }

        if (TryGetTargetFrameworkProviderAndCheckType(manager, instance, out TypeDescriptionProvider? targetProvider))
        {
            return targetProvider.GetReflectionType(instance);
        }

        return TypeDescriptor.GetReflectionType(instance);
    }

    /// <summary>
    ///  Get properties collection as defined in the project target framework
    /// </summary>
    protected static PropertyDescriptorCollection GetPropertiesHelper(IDesignerSerializationManager manager, object instance, Attribute[]? attributes)
    {
        if (instance is null || manager is null)
        {
            Debug.Fail("GetPropertiesHelper does not accept null arguments.");
            return null;
        }

        if (TryGetTargetFrameworkProviderAndCheckType(manager, instance, out TypeDescriptionProvider? targetProvider))
        {
            if (targetProvider.GetTypeDescriptor(instance) is { } targetAwareDescriptor)
            {
                if (attributes is null)
                {
                    return targetAwareDescriptor.GetProperties();
                }

                return targetAwareDescriptor.GetProperties(attributes);
            }
        }

        if (attributes is null)
        {
            return TypeDescriptor.GetProperties(instance);
        }

        return TypeDescriptor.GetProperties(instance, attributes);
    }

    /// <summary>
    ///  Get events collection as defined in the project target framework
    /// </summary>
    protected static EventDescriptorCollection GetEventsHelper(IDesignerSerializationManager manager, object instance, Attribute[]? attributes)
    {
        if (instance is null || manager is null)
        {
            Debug.Fail("GetEventsHelper does not accept null arguments.");
            return null;
        }

        if (TryGetTargetFrameworkProviderAndCheckType(manager, instance, out TypeDescriptionProvider? targetProvider))
        {
            if (targetProvider.GetTypeDescriptor(instance) is { } targetAwareDescriptor)
            {
                if (attributes is null)
                {
                    return targetAwareDescriptor.GetEvents();
                }

                return targetAwareDescriptor.GetEvents(attributes);
            }
        }

        if (attributes is null)
        {
            return TypeDescriptor.GetEvents(instance);
        }

        return TypeDescriptor.GetEvents(instance, attributes);
    }

    /// <summary>
    ///  Get attributes collection as defined in the project target framework
    /// </summary>
    protected static AttributeCollection GetAttributesHelper(IDesignerSerializationManager manager, object instance)
    {
        if (instance is null || manager is null)
        {
            Debug.Fail("GetAttributesHelper does not accept null arguments.");
            return null;
        }

        if (TryGetTargetFrameworkProviderAndCheckType(manager, instance, out TypeDescriptionProvider? targetProvider))
        {
            if (targetProvider.GetTypeDescriptor(instance) is { } targetAwareDescriptor)
            {
                return targetAwareDescriptor.GetAttributes();
            }
        }

        return TypeDescriptor.GetAttributes(instance);
    }

    /// <summary>
    ///  Get attributes collection as defined in the project target framework
    /// </summary>
    protected static AttributeCollection GetAttributesFromTypeHelper(IDesignerSerializationManager manager, Type type)
    {
        if (type is null || manager is null)
        {
            Debug.Fail("GetAttributesFromTypeHelper does not accept null arguments.");
            return null;
        }

        if (type.IsValueType)
        {
            if (TryGetTargetFrameworkProviderForType(manager, type, out TypeDescriptionProvider? targetProvider))
            {
                if (targetProvider.IsSupportedType(type))
                {
                    if (targetProvider.GetTypeDescriptor(type) is { } targetAwareDescriptor)
                    {
                        return targetAwareDescriptor.GetAttributes();
                    }
                }
                else
                {
                    Error(manager, string.Format(SR.TypeNotFoundInTargetFramework, type.FullName), SR.SerializerUndeclaredName);
                }
            }
        }

        return TypeDescriptor.GetAttributes(type);
    }

    /// <summary>
    ///  This method will inspect all of the properties on the given object fitting the filter, and check for that
    ///  property in a resource blob. This is useful for deserializing properties that cannot be represented
    ///  in code, such as design-time properties.
    /// </summary>
    protected void DeserializePropertiesFromResources(IDesignerSerializationManager manager, object value, Attribute[]? filter)
    {
        // It is much faster to dig through the resources first, and then map these resources to properties than it is
        // to filter properties at each turn. Why?  Because filtering properties requires a separate filter call for
        // each object (because designers get a chance to filter, the cache is per-component), while resources are loaded
        // once per document.
        IDictionaryEnumerator? de = ResourceCodeDomSerializer.GetMetadataEnumerator(manager);
        de ??= ResourceCodeDomSerializer.GetEnumerator(manager, CultureInfo.InvariantCulture);

        if (de is not null)
        {
            string? ourObjectName;
            if (manager.TryGetContext(out RootContext? root) && root.Value == value)
            {
                ourObjectName = "$this";
            }
            else
            {
                ourObjectName = manager.GetName(value);
            }

            if (ourObjectName is null)
            {
                return;
            }

            PropertyDescriptorCollection ourProperties = GetPropertiesHelper(manager, value, null);
            while (de.MoveNext())
            {
                string resourceName = (de.Key as string)!;
                Debug.Assert(resourceName is not null, "non-string keys in dictionary entry");
                int dotIndex = resourceName.IndexOf('.');
                if (dotIndex == -1)
                {
                    continue;
                }

                // Skip now if this isn't a value for our object.
                if (!resourceName.AsSpan(0, dotIndex).Equals(ourObjectName, StringComparison.Ordinal))
                {
                    continue;
                }

                string propertyName = resourceName[(dotIndex + 1)..];

                // Now locate the property by this name.
                PropertyDescriptor? property = ourProperties[propertyName];
                if (property is null)
                {
                    continue;
                }

                // This property must have matching attributes.
                if (property.Attributes.Contains(filter))
                {
                    object? resourceObject = de.Value;
                    try
                    {
                        property.SetValue(value, resourceObject);
                    }
                    catch (Exception e)
                    {
                        manager.ReportError(e);
                    }
                }
            }
        }
    }

    /// <summary>
    ///  This is a helper method that will deserialize a given statement. It deserializes
    ///  the statement by interpreting and executing the CodeDom statement.
    /// </summary>
    protected void DeserializeStatement(IDesignerSerializationManager manager, CodeStatement statement)
    {
        // Push this statement onto the context stack.
        // This allows any serializers handling an expression to know what it was connected to.
        manager.Context.Push(statement);
        try
        {
            // Perf: change ordering based on possibility of occurrence
            if (statement is CodeAssignStatement cas)
            {
                DeserializeAssignStatement(manager, cas);
            }
            else if (statement is CodeVariableDeclarationStatement cvds)
            {
                DeserializeVariableDeclarationStatement(manager, cvds);
            }
            else if (statement is CodeCommentStatement)
            {
                // do nothing for comments. This just suppresses the debug warning
            }
            else if (statement is CodeExpressionStatement ces)
            {
                DeserializeExpression(manager, null, ces.Expression);
            }
            else if (statement is CodeMethodReturnStatement cmrs)
            {
                DeserializeExpression(manager, null, cmrs.Expression);
            }
            else if (statement is CodeAttachEventStatement caes)
            {
                DeserializeAttachEventStatement(manager, caes);
            }
            else if (statement is CodeRemoveEventStatement cres)
            {
                DeserializeDetachEventStatement(manager, cres);
            }
            else if (statement is CodeLabeledStatement cls)
            {
                DeserializeStatement(manager, cls.Statement);
            }
        }
        catch (CheckoutException)
        {
            throw; // we want to propagate those all the way up
        }
        catch (Exception e)
        {
            // Since we always go through reflection, don't show what our engine does, show what caused the problem.
            if (e is TargetInvocationException)
            {
                e = e.InnerException!;
            }

            if (e is not CodeDomSerializerException && statement.LinePragma is not null)
            {
                e = new CodeDomSerializerException(e, statement.LinePragma);
            }

            manager.ReportError(e);
        }
        finally
        {
            Debug.Assert(manager.Context.Current == statement, "Someone corrupted the context stack");
            manager.Context.Pop();
        }
    }

    private void DeserializeVariableDeclarationStatement(IDesignerSerializationManager manager, CodeVariableDeclarationStatement statement)
    {
        if (statement.InitExpression is not null)
        {
            DeserializeExpression(manager, statement.Name, statement.InitExpression);
        }
    }

    private void DeserializeDetachEventStatement(IDesignerSerializationManager manager, CodeRemoveEventStatement statement)
    {
        object? eventListener = DeserializeExpression(manager, null, statement.Listener);
        if (eventListener is CodeDelegateCreateExpression delegateCreate)
        {
            // We only support binding methods to the root object.
            object? eventAttachObject = DeserializeExpression(manager, null, delegateCreate.TargetObject);
            RootContext? rootExp = manager.GetContext<RootContext>();
            bool isRoot = rootExp is null || rootExp.Value == eventAttachObject;

            if (isRoot)
            {
                // Now deserialize the LHS of the event attach to discover the guy whose event we are attaching.
                object targetObject = DeserializeExpression(manager, null, statement.Event.TargetObject)!;

                if (targetObject is not CodeExpression)
                {
                    EventDescriptor? evt = GetEventsHelper(manager, targetObject, null)[statement.Event.EventName];
                    if (evt is not null)
                    {
                        IEventBindingService? evtSvc = manager.GetService<IEventBindingService>();
                        if (evtSvc is not null)
                        {
                            PropertyDescriptor prop = evtSvc.GetEventProperty(evt);
                            prop.SetValue(targetObject, null);
                        }
                    }
                    else
                    {
                        Error(manager, string.Format(SR.SerializerNoSuchEvent, targetObject.GetType().FullName, statement.Event.EventName), SR.SerializerNoSuchEvent);
                    }
                }
            }
        }
    }

    private void DeserializeAssignStatement(IDesignerSerializationManager manager, CodeAssignStatement statement)
    {
        // Since we're doing an assignment into something, we need to know what that something is.
        // It can be a property, a variable, or a member. Anything else is invalid.
        // Perf: is -> as changes, change ordering based on possibility of occurrence
        CodeExpression expression = statement.Left;

        if (expression is CodePropertyReferenceExpression propertyReferenceEx)
        {
            DeserializePropertyAssignStatement(manager, statement, propertyReferenceEx, true);
        }
        else if (expression is CodeFieldReferenceExpression fieldReferenceEx)
        {
            object? lhs = DeserializeExpression(manager, fieldReferenceEx.FieldName, fieldReferenceEx.TargetObject);
            if (lhs is not null)
            {
                if (manager.TryGetContext(out RootContext? root) && root.Value == lhs)
                {
                    object? rhs = DeserializeExpression(manager, fieldReferenceEx.FieldName, statement.Right);
                    if (rhs is CodeExpression)
                    {
                        return;
                    }
                }
                else
                {
                    FieldInfo? f;
                    object? instance;

                    if (lhs is Type type)
                    {
                        instance = null;
                        f = GetReflectionTypeFromTypeHelper(manager, type).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
                    }
                    else
                    {
                        instance = lhs;
                        f = GetReflectionTypeHelper(manager, lhs).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
                    }

                    if (f is not null)
                    {
                        object? rhs = DeserializeExpression(manager, fieldReferenceEx.FieldName, statement.Right);
                        if (rhs is CodeExpression)
                        {
                            return;
                        }

                        if (rhs is IConvertible ic)
                        {
                            // f.FieldType is a type from the reflection (or project target) universe,
                            // while rhs is a runtime type (exists in the Visual Studio framework)
                            // they need to be converted to the same universe for comparison to work.
                            // If TargetFrameworkProvider is not available, then we are working with runtime types.
                            Type fieldType = f.FieldType;
                            if (TryGetTargetFrameworkProviderForType(manager, fieldType, out TypeDescriptionProvider? tdp))
                            {
                                fieldType = tdp.GetRuntimeType(fieldType);
                            }

                            if (fieldType != rhs.GetType())
                            {
                                try
                                {
                                    rhs = ic.ToType(fieldType, null);
                                }
                                catch
                                {
                                    // oh well...
                                }
                            }
                        }

                        f.SetValue(instance, rhs);
                    }
                    else
                    {
                        // lets try it as a property:
                        CodePropertyReferenceExpression propRef = new CodePropertyReferenceExpression
                        {
                            TargetObject = fieldReferenceEx.TargetObject,
                            PropertyName = fieldReferenceEx.FieldName
                        };

                        if (!DeserializePropertyAssignStatement(manager, statement, propRef, false))
                        {
                            Error(manager, string.Format(SR.SerializerNoSuchField, lhs.GetType().FullName, fieldReferenceEx.FieldName), SR.SerializerNoSuchField);
                        }
                    }
                }
            }
        }
        else if (expression is CodeVariableReferenceExpression variableReferenceEx)
        {
            // This is the easiest. Just relate the RHS object to the name of the variable.
            object rhs = DeserializeExpression(manager, variableReferenceEx.VariableName, statement.Right)!;
            if (rhs is CodeExpression)
            {
                return;
            }

            manager.SetName(rhs, variableReferenceEx.VariableName);
        }
        else if (expression is CodeArrayIndexerExpression arrayIndexerEx)
        {
            int[] indexes = new int[arrayIndexerEx.Indices.Count];
            object? array = DeserializeExpression(manager, null, arrayIndexerEx.TargetObject);

            // The indexes have to be of type int32. If they're not, then we cannot assign to this array.
            for (int i = 0; i < indexes.Length; i++)
            {
                object? index = DeserializeExpression(manager, null, arrayIndexerEx.Indices[i]);
                if (index is IConvertible ic)
                {
                    indexes[i] = ic.ToInt32(null);
                }
                else
                {
                    return;
                }
            }

            if (array is Array arr)
            {
                object? rhs = DeserializeExpression(manager, null, statement.Right);
                if (rhs is CodeExpression)
                {
                    return;
                }

                arr.SetValue(rhs, indexes);
            }
        }
    }

    /// <summary>
    ///  This is a helper method that will deserialize a given expression. It deserializes
    ///  the statement by interpreting and executing the CodeDom expression, returning
    ///  the results.
    /// </summary>
    protected object? DeserializeExpression(IDesignerSerializationManager manager, string? name, CodeExpression? expression)
    {
        object? result = expression;

        // Perf: is -> as changes, change ordering based on possibility of occurrence
        // If you are adding to this, use as instead of is + cast and order new expressions in order of frequency in typical user code.

        while (result is not null)
        {
            if (result is CodePrimitiveExpression primitiveEx)
            {
                result = primitiveEx.Value;
                break;
            }
            else if (result is CodePropertyReferenceExpression propertyReferenceEx)
            {
                result = DeserializePropertyReferenceExpression(manager, propertyReferenceEx, true);
                break;
            }
            else if (result is CodeThisReferenceExpression)
            {
                // (is -> as doesn't help here, since the cast is different)
                if (manager.TryGetContext(out RootContext? rootExp))
                {
                    result = rootExp.Value;
                }
                else
                {
                    // Last ditch effort. Some things have to code gen against "this", such as event wireups.
                    // Those are always bound against the root component.
                    if (manager.GetService<IDesignerHost>() is { } host)
                    {
                        result = host.RootComponent;
                    }
                }

                if (result is null)
                {
                    Error(manager, SR.SerializerNoRootExpression, SR.SerializerNoRootExpression);
                }

                break;
            }
            else if (result is CodeTypeReferenceExpression typeReferenceEx)
            {
                result = manager.GetType(GetTypeNameFromCodeTypeReference(manager, typeReferenceEx.Type));
                break;
            }
            else if (result is CodeObjectCreateExpression objectCreateEx)
            {
                result = null;
                Type? type = manager.GetType(GetTypeNameFromCodeTypeReference(manager, objectCreateEx.CreateType));
                if (type is not null)
                {
                    object?[] parameters = new object[objectCreateEx.Parameters.Count];
                    bool paramsOk = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = DeserializeExpression(manager, null, objectCreateEx.Parameters[i]);
                        if (parameters[i] is CodeExpression)
                        {
                            // Before we bail on this parameter, see if the type is a delegate.
                            // If we are creating a delegate we may be able to bind to the method after all.
                            if (typeof(Delegate).IsAssignableFrom(type) && parameters is [CodeMethodReferenceExpression methodRef])
                            {
                                // Only do this if our target is not the root context.
                                if (methodRef.TargetObject is not CodeThisReferenceExpression)
                                {
                                    object target = DeserializeExpression(manager, null, methodRef.TargetObject)!;
                                    if (target is not CodeExpression)
                                    {
                                        // Search for a matching method sig. Must be public since we don't own this object
                                        MethodInfo? delegateInvoke = type.GetMethod("Invoke");
                                        if (delegateInvoke is not null)
                                        {
                                            ParameterInfo[] delegateParams = delegateInvoke.GetParameters();
                                            Type[] paramTypes = new Type[delegateParams.Length];
                                            for (int idx = 0; idx < paramTypes.Length; idx++)
                                            {
                                                paramTypes[idx] = delegateParams[i].ParameterType;
                                            }

                                            MethodInfo? mi = GetReflectionTypeHelper(manager, target).GetMethod(methodRef.MethodName, paramTypes);
                                            if (mi is not null)
                                            {
                                                // MethodInfo from the reflection Universe might
                                                // not implement MethodHandle property,
                                                // once we know that the method is available,
                                                // get it from the runtime type.
                                                mi = target.GetType().GetMethod(methodRef.MethodName, paramTypes)!;
                                                result = Activator.CreateInstance(type, [target, mi.MethodHandle.GetFunctionPointer()]);
                                            }
                                        }
                                    }
                                }
                            }

                            // Technically, the parameters are not OK. Our special case above, if successful, would have produced a "result" object for us.
                            paramsOk = false;
                            break;
                        }
                    }

                    if (paramsOk)
                    {
                        // Create an instance of the object. If the caller provided a name, then ask the manager to add this object to the container.
                        result = DeserializeInstance(manager, type, parameters, name, (name is not null));
                    }
                }
                else
                {
                    Error(manager, string.Format(SR.SerializerTypeNotFound, objectCreateEx.CreateType.BaseType), SR.SerializerTypeNotFound);
                }

                break;
            }
            else if (result is CodeArgumentReferenceExpression argumentReferenceEx)
            {
                result = manager.GetInstance(argumentReferenceEx.ParameterName);
                if (result is null)
                {
                    Error(manager, string.Format(SR.SerializerUndeclaredName, argumentReferenceEx.ParameterName), SR.SerializerUndeclaredName);
                }

                break;
            }
            else if (result is CodeFieldReferenceExpression fieldReferenceEx)
            {
                object? target = DeserializeExpression(manager, null, fieldReferenceEx.TargetObject);
                if (target is not null and not CodeExpression)
                {
                    // If the target is the root object, then this won't be found through reflection. Instead, ask the manager for the field by name.
                    RootContext? rootExp = manager.GetContext<RootContext>();
                    if (rootExp is not null && rootExp.Value == target)
                    {
                        object? namedObject = manager.GetInstance(fieldReferenceEx.FieldName);
                        if (namedObject is not null)
                        {
                            result = namedObject;
                        }
                        else
                        {
                            Error(manager, string.Format(SR.SerializerUndeclaredName, fieldReferenceEx.FieldName), SR.SerializerUndeclaredName);
                        }
                    }
                    else
                    {
                        FieldInfo? field;
                        object? instance;
                        if (target is Type t)
                        {
                            instance = null;
                            field = GetReflectionTypeFromTypeHelper(manager, t).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Static | BindingFlags.Public);
                        }
                        else
                        {
                            instance = target;
                            field = GetReflectionTypeHelper(manager, target).GetField(fieldReferenceEx.FieldName, BindingFlags.GetField | BindingFlags.Instance | BindingFlags.Public);
                        }

                        if (field is not null)
                        {
                            result = field.GetValue(instance);
                        }
                        else
                        {
                            // lets try it as a property:
                            CodePropertyReferenceExpression propRef = new CodePropertyReferenceExpression
                            {
                                TargetObject = fieldReferenceEx.TargetObject,
                                PropertyName = fieldReferenceEx.FieldName
                            };

                            result = DeserializePropertyReferenceExpression(manager, propRef, false);
                            if (result == fieldReferenceEx)
                            {
                                Error(manager, string.Format(SR.SerializerUndeclaredName, fieldReferenceEx.FieldName), SR.SerializerUndeclaredName);
                            }
                        }
                    }
                }
                else
                {
                    Error(manager, string.Format(SR.SerializerFieldTargetEvalFailed, fieldReferenceEx.FieldName), SR.SerializerFieldTargetEvalFailed);
                }

                break;
            }
            else if (result is CodeMethodInvokeExpression methodInvokeEx)
            {
                object? targetObject = DeserializeExpression(manager, null, methodInvokeEx.Method.TargetObject);

                if (targetObject is not null)
                {
                    object?[] parameters = new object[methodInvokeEx.Parameters.Count];
                    bool paramsOk = true;

                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = DeserializeExpression(manager, null, methodInvokeEx.Parameters[i]);
                        if (parameters[i] is CodeExpression)
                        {
                            paramsOk = false;
                            break;
                        }
                    }

                    if (paramsOk)
                    {
                        var changeService = manager.GetService<IComponentChangeService>();

                        if (targetObject is Type t)
                        {
                            result = GetReflectionTypeFromTypeHelper(manager, t).InvokeMember(methodInvokeEx.Method.MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Static, null, null, parameters, null, null, null);
                        }
                        else
                        {
                            changeService?.OnComponentChanging(targetObject, null);

                            try
                            {
                                result = GetReflectionTypeHelper(manager, targetObject).InvokeMember(methodInvokeEx.Method.MethodName, BindingFlags.InvokeMethod | BindingFlags.Public | BindingFlags.Instance, null, targetObject, parameters, null, null, null);
                            }
                            catch (MissingMethodException)
                            {
                                // We did not find the method directly. Let's see if we can find it
                                // as an private implemented interface name.

                                if (methodInvokeEx.Method.TargetObject is CodeCastExpression castExpr)
                                {
                                    Type? castType = manager.GetType(GetTypeNameFromCodeTypeReference(manager, castExpr.TargetType));

                                    if (castType is not null && castType.IsInterface)
                                    {
                                        result = GetReflectionTypeFromTypeHelper(manager, castType).InvokeMember(methodInvokeEx.Method.MethodName, BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance, null, targetObject, parameters, null, null, null);
                                    }
                                    else
                                    {
                                        throw;
                                    }
                                }
                                else
                                {
                                    throw;
                                }
                            }

                            changeService?.OnComponentChanged(targetObject);
                        }
                    }
                    else if (parameters is [CodeDelegateCreateExpression codeDelegateCreateExpression])
                    {
                        string methodName = methodInvokeEx.Method.MethodName;

                        if (methodName.StartsWith("add_", StringComparison.Ordinal))
                        {
                            methodName = methodName[4..];
                            DeserializeAttachEventStatement(manager, new CodeAttachEventStatement(methodInvokeEx.Method.TargetObject, methodName, codeDelegateCreateExpression));
                            result = null;
                        }
                    }
                }

                break;
            }
            else if (result is CodeVariableReferenceExpression variableReferenceEx)
            {
                result = manager.GetInstance(variableReferenceEx.VariableName);
                if (result is null)
                {
                    Error(manager, string.Format(SR.SerializerUndeclaredName, variableReferenceEx.VariableName), SR.SerializerUndeclaredName);
                }

                break;
            }
            else if (result is CodeCastExpression castEx)
            {
                result = DeserializeExpression(manager, name, castEx.Expression);
                if (result is IConvertible ic)
                {
                    Type? targetType = manager.GetType(GetTypeNameFromCodeTypeReference(manager, castEx.TargetType));
                    if (targetType is not null)
                    {
                        result = ic.ToType(targetType, null);
                    }
                }

                break;
            }
            else if (result is CodeBaseReferenceExpression)
            {
                // (is -> as doesn't help here, since the cast is different)
                RootContext? rootExp = manager.GetContext<RootContext>();
                result = rootExp?.Value;

                break;
            }
            else if (result is CodeArrayCreateExpression arrayCreateEx)
            {
                Type? arrayType = manager.GetType(GetTypeNameFromCodeTypeReference(manager, arrayCreateEx.CreateType));
                Array? array = null;

                if (arrayType is not null)
                {
                    if (arrayCreateEx.Initializers.Count > 0)
                    {
                        // Passed an array of initializers. Use this to create the array. Note that we use an ArrayList
                        // here and add elements as we create them. It is possible that an element cannot be resolved.
                        // This is an error, but we do not want to tank the entire array. If we kicked out the entire
                        // statement, a missing control would cause all controls on a form to vanish.
                        ArrayList arrayList = new(arrayCreateEx.Initializers.Count);

                        foreach (CodeExpression initializer in arrayCreateEx.Initializers)
                        {
                            try
                            {
                                object? o = DeserializeExpression(manager, null, initializer);

                                if (o is not CodeExpression)
                                {
                                    if (!arrayType.IsInstanceOfType(o))
                                    {
                                        o = Convert.ChangeType(o, arrayType, CultureInfo.InvariantCulture);
                                    }

                                    arrayList.Add(o);
                                }
                            }
                            catch (Exception ex)
                            {
                                manager.ReportError(ex);
                            }
                        }

                        array = Array.CreateInstance(arrayType, arrayList.Count);
                        arrayList.CopyTo(array, 0);
                    }
                    else if (arrayCreateEx.SizeExpression is not null)
                    {
                        object? o = DeserializeExpression(manager, name, arrayCreateEx.SizeExpression);
                        Debug.Assert(o is IConvertible, $"Array size expression could not be resolved to IConvertible: {(o.GetType().Name)}");

                        if (o is IConvertible ic)
                        {
                            int size = ic.ToInt32(null);
                            array = Array.CreateInstance(arrayType, size);
                        }
                    }
                    else
                    {
                        array = Array.CreateInstance(arrayType, arrayCreateEx.Size);
                    }
                }
                else
                {
                    Error(manager, string.Format(SR.SerializerTypeNotFound, arrayCreateEx.CreateType.BaseType), SR.SerializerTypeNotFound);
                }

                result = array;
                if (result is not null && name is not null)
                {
                    manager.SetName(result, name);
                }

                break;
            }
            else if (result is CodeArrayIndexerExpression arrayIndexerEx)
            {
                // For this, assume in any error we return a null. The only errors here should come from a mal-formed expression.
                result = null;

                if (DeserializeExpression(manager, name, arrayIndexerEx.TargetObject) is Array array)
                {
                    int[] indexes = new int[arrayIndexerEx.Indices.Count];

                    bool indexesOK = true;

                    // The indexes have to be of type int32. If they're not, then we cannot assign to this array.
                    for (int i = 0; i < indexes.Length; i++)
                    {
                        object? index = DeserializeExpression(manager, name, arrayIndexerEx.Indices[i]);

                        if (index is IConvertible convertible)
                        {
                            indexes[i] = convertible.ToInt32(null);
                        }
                        else
                        {
                            indexesOK = false;
                            break;
                        }
                    }

                    if (indexesOK)
                    {
                        result = array.GetValue(indexes);
                    }
                }

                break;
            }
            else if (result is CodeBinaryOperatorExpression binaryOperatorEx)
            {
                object? left = DeserializeExpression(manager, null, binaryOperatorEx.Left);
                object? right = DeserializeExpression(manager, null, binaryOperatorEx.Right);

                // We assign the result to an arbitrary value here in case the operation could not be performed.
                result = left;

                if (left is IConvertible icLeft && right is IConvertible icRight)
                {
                    result = ExecuteBinaryExpression(icLeft, icRight, binaryOperatorEx.Operator);
                }

                break;
            }
            else if (result is CodeDelegateInvokeExpression delegateInvokeEx)
            {
                object? targetObject = DeserializeExpression(manager, null, delegateInvokeEx.TargetObject);
                if (targetObject is Delegate del)
                {
                    object?[] parameters = new object[delegateInvokeEx.Parameters.Count];
                    bool paramsOk = true;
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        parameters[i] = DeserializeExpression(manager, null, delegateInvokeEx.Parameters[i]);
                        if (parameters[i] is CodeExpression)
                        {
                            paramsOk = false;
                            break;
                        }
                    }

                    if (paramsOk)
                    {
                        del.DynamicInvoke(parameters);
                    }
                }

                break;
            }
            else if (result is CodeDirectionExpression directionEx)
            {
                result = DeserializeExpression(manager, name, directionEx.Expression);
                break;
            }
            else if (result is CodeIndexerExpression indexerEx)
            {
                // For this, assume in any error we return a null. The only errors here should come from a mal-formed expression.
                result = null;
                object? targetObject = DeserializeExpression(manager, null, indexerEx.TargetObject);
                if (targetObject is not null)
                {
                    object?[] indexes = new object[indexerEx.Indices.Count];
                    bool indexesOK = true;

                    for (int i = 0; i < indexes.Length; i++)
                    {
                        indexes[i] = DeserializeExpression(manager, null, indexerEx.Indices[i]);
                        if (indexes[i] is CodeExpression)
                        {
                            indexesOK = false;
                            break;
                        }
                    }

                    if (indexesOK)
                    {
                        result = GetReflectionTypeHelper(manager, targetObject).InvokeMember("Item", BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance, null, targetObject, indexes, null, null, null);
                    }
                }

                break;
            }
            else if (result is CodeSnippetExpression)
            {
                result = null;
                break;
            }
            else if (result is CodeParameterDeclarationExpression parameterDeclaration)
            {
                result = manager.GetType(GetTypeNameFromCodeTypeReference(manager, parameterDeclaration.Type));
                break;
            }
            else if (result is CodeTypeOfExpression typeOfExpression)
            {
                string type = GetTypeNameFromCodeTypeReference(manager, typeOfExpression.Type);
                // add the array ranks so we get the right type of this thing.
                for (int i = 0; i < typeOfExpression.Type.ArrayRank; i++)
                {
                    type += "[]";
                }

                result = manager.GetType(type);
                if (result is null)
                {
                    Error(manager, string.Format(SR.SerializerTypeNotFound, type), SR.SerializerTypeNotFound);
                }

                break;
            }
            else if (result is CodeEventReferenceExpression or CodeMethodReferenceExpression or CodeDelegateCreateExpression)
            {
                // These are all the expressions we know about, but expect to return to the caller because we cannot simplify them.
                break;
            }
            else
            {
                // All expression evaluation happens above.
                // This codepath indicates that we got some sort of junk in the evaluator,
                // or that someone assigned result to a value without breaking out of the loop.
                Debug.Fail($"Unrecognized expression type: {result.GetType().Name}");
                break;
            }
        }

        return result;
    }

    private void DeserializeAttachEventStatement(IDesignerSerializationManager manager, CodeAttachEventStatement statement)
    {
        string? handlerMethodName = null;
        object? eventAttachObject = null;

        // Get the target information
        object? targetObject = DeserializeExpression(manager, null, statement.Event.TargetObject);
        string eventName = statement.Event.EventName;
        Debug.Assert(targetObject is not null, "Failed to get target object for event attach");
        Debug.Assert(eventName is not null, "Failed to get eventName for event attach");
        if (eventName is null || targetObject is null)
        {
            return;
        }

        if (statement.Listener is CodeObjectCreateExpression objCreate)
        {
            // now walk into the CodeObjectCreateExpression and get the parameters so we can get the name of the method, e.g. button1_Click
            if (objCreate.Parameters.Count == 1)
            {
                // if this is a delegate create (new EventHandler(this.button1_Click)), then the first parameter should be a method ref.
                if (objCreate.Parameters[0] is CodeMethodReferenceExpression methodRef)
                {
                    handlerMethodName = methodRef.MethodName;
                    eventAttachObject = DeserializeExpression(manager, null, methodRef.TargetObject);
                }
            }
            else
            {
                Debug.Fail("Encountered delegate object create with more or less than 1 parameter?");
            }
        }
        else
        {
            object? eventListener = DeserializeExpression(manager, null, statement.Listener);
            if (eventListener is CodeDelegateCreateExpression delegateCreate)
            {
                eventAttachObject = DeserializeExpression(manager, null, delegateCreate.TargetObject);
                handlerMethodName = delegateCreate.MethodName;
            }
        }

        RootContext? rootExp = manager.GetContext<RootContext>();
        bool isRoot = rootExp is null || rootExp.Value == eventAttachObject;

        if (handlerMethodName is null || !isRoot)
        {
            // We only support binding methods to the root object.
            return;
        }

        // Now deserialize the LHS of the event attach to discover the guy whose
        // event we are attaching.
        if (targetObject is not CodeExpression)
        {
            EventDescriptor? evt = GetEventsHelper(manager, targetObject, null)[eventName];

            if (evt is not null)
            {
                IEventBindingService? evtSvc = manager.GetService<IEventBindingService>();

                if (evtSvc is not null)
                {
                    PropertyDescriptor prop = evtSvc.GetEventProperty(evt);
                    prop.SetValue(targetObject, handlerMethodName);
                }
            }
            else
            {
                Error(manager, string.Format(SR.SerializerNoSuchEvent, targetObject.GetType().FullName, eventName), SR.SerializerNoSuchEvent);
            }
        }
    }

    private static object ExecuteBinaryExpression(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
    {
        // "Binary" operator type is actually a combination of several types of operators:
        // boolean, binary and math. Group them into categories here.

        // Figure out what kind of expression we have.
        switch (op)
        {
            case CodeBinaryOperatorType.BitwiseOr:
            case CodeBinaryOperatorType.BitwiseAnd:
                return ExecuteBinaryOperator(left, right, op);

            case CodeBinaryOperatorType.Add:
            case CodeBinaryOperatorType.Subtract:
            case CodeBinaryOperatorType.Multiply:
            case CodeBinaryOperatorType.Divide:
            case CodeBinaryOperatorType.Modulus:
                return ExecuteMathOperator(left, right, op);

            case CodeBinaryOperatorType.IdentityInequality:
            case CodeBinaryOperatorType.IdentityEquality:
            case CodeBinaryOperatorType.ValueEquality:
            case CodeBinaryOperatorType.BooleanOr:
            case CodeBinaryOperatorType.BooleanAnd:
            case CodeBinaryOperatorType.LessThan:
            case CodeBinaryOperatorType.LessThanOrEqual:
            case CodeBinaryOperatorType.GreaterThan:
            case CodeBinaryOperatorType.GreaterThanOrEqual:
                return ExecuteBooleanOperator(left, right, op);

            default:
                Debug.Fail($"Unsupported binary operator type: {op}");
                return left;
        }
    }

    private static object ExecuteBinaryOperator(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
    {
        TypeCode leftType = left.GetTypeCode();
        TypeCode rightType = right.GetTypeCode();

        // The compatible types are listed in order from lowest bitness to highest.
        // We must operate on the highest bitness to keep fidelity.
        ReadOnlySpan<TypeCode> compatibleTypes =
        [
            TypeCode.Byte,
            TypeCode.Char,
            TypeCode.Int16,
            TypeCode.UInt16,
            TypeCode.Int32,
            TypeCode.UInt32,
            TypeCode.Int64,
            TypeCode.UInt64
        ];

        int leftTypeIndex = -1;
        int rightTypeIndex = -1;

        for (int i = 0; i < compatibleTypes.Length; i++)
        {
            if (leftType == compatibleTypes[i])
            {
                leftTypeIndex = i;
            }

            if (rightType == compatibleTypes[i])
            {
                rightTypeIndex = i;
            }

            if (leftTypeIndex != -1 && rightTypeIndex != -1)
            {
                break;
            }
        }

        if (leftTypeIndex == -1 || rightTypeIndex == -1)
        {
            Debug.Fail("Could not convert left or right side to binary-compatible value.");
            return left;
        }

        int maxIndex = Math.Max(leftTypeIndex, rightTypeIndex);
        object result = left;
        switch (compatibleTypes[maxIndex])
        {
            case TypeCode.Byte:
                {
                    byte leftValue = left.ToByte(null);
                    byte rightValue = right.ToByte(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }

            case TypeCode.Char:
                {
                    char leftValue = left.ToChar(null);
                    char rightValue = right.ToChar(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }

            case TypeCode.Int16:
                {
                    short leftValue = left.ToInt16(null);
                    short rightValue = right.ToInt16(null);
                    if (op == CodeBinaryOperatorType.BitwiseOr)
                    {
                        result = (short)((ushort)leftValue | (ushort)rightValue);
                    }
                    else
                    {
                        result = leftValue & rightValue;
                    }

                    break;
                }

            case TypeCode.UInt16:
                {
                    ushort leftValue = left.ToUInt16(null);
                    ushort rightValue = right.ToUInt16(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }

            case TypeCode.Int32:
                {
                    int leftValue = left.ToInt32(null);
                    int rightValue = right.ToInt32(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }

            case TypeCode.UInt32:
                {
                    uint leftValue = left.ToUInt32(null);
                    uint rightValue = right.ToUInt32(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }

            case TypeCode.Int64:
                {
                    long leftValue = left.ToInt64(null);
                    long rightValue = right.ToInt64(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }

            case TypeCode.UInt64:
                {
                    ulong leftValue = left.ToUInt64(null);
                    ulong rightValue = right.ToUInt64(null);
                    result = ExecuteBinaryOperator(leftValue, rightValue, op);
                    break;
                }
        }

        if (result != left && left is Enum)
        {
            // For enums, try to convert back to the original type
            result = Enum.ToObject(left.GetType(), result);
        }

        return result;

        static object ExecuteBinaryOperator<T>(T leftValue, T rightValue, CodeBinaryOperatorType op) where T : IBinaryInteger<T>
        {
            if (op == CodeBinaryOperatorType.BitwiseOr)
            {
                return leftValue | rightValue;
            }
            else
            {
                return leftValue & rightValue;
            }
        }
    }

    private static bool ExecuteBooleanOperator(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
    {
        bool result = false;
        switch (op)
        {
            case CodeBinaryOperatorType.IdentityInequality:
                result = (left != right);
                break;
            case CodeBinaryOperatorType.IdentityEquality:
                result = (left == right);
                break;
            case CodeBinaryOperatorType.ValueEquality:
                result = left.Equals(right);
                break;
            case CodeBinaryOperatorType.BooleanOr:
                result = (left.ToBoolean(null) || right.ToBoolean(null));
                break;
            case CodeBinaryOperatorType.BooleanAnd:
                result = (left.ToBoolean(null) && right.ToBoolean(null));
                break;
            case CodeBinaryOperatorType.LessThan:
                // Not doing these at design time.
                break;
            case CodeBinaryOperatorType.LessThanOrEqual:
                // Not doing these at design time.
                break;
            case CodeBinaryOperatorType.GreaterThan:
                // Not doing these at design time.
                break;
            case CodeBinaryOperatorType.GreaterThanOrEqual:
                // Not doing these at design time.
                break;
            default:
                Debug.Fail("Should never get here!");
                break;
        }

        return result;
    }

    private static object ExecuteMathOperator(IConvertible left, IConvertible right, CodeBinaryOperatorType op)
    {
        if (op == CodeBinaryOperatorType.Add)
        {
            if (left is string or char && right is string or char)
            {
                return $"{left}{right}";
            }
            else
            {
                Debug.Fail("Addition operator not supported for this type");
            }
        }
        else
        {
            Debug.Fail("Math operators are not supported");
        }

        return left;
    }

    private object? DeserializePropertyReferenceExpression(IDesignerSerializationManager manager, CodePropertyReferenceExpression propertyReferenceEx, bool reportError)
    {
        object? result = propertyReferenceEx;
        object? target = DeserializeExpression(manager, null, propertyReferenceEx.TargetObject);

        if (target is not null and not CodeExpression)
        {
            Type? targetAsType = target as Type;
            // if it's a type, then we've got ourselves a static field...
            if (targetAsType is null)
            {
                PropertyDescriptor? prop = GetPropertiesHelper(manager, target, null)[propertyReferenceEx.PropertyName];
                if (prop is not null)
                {
                    result = prop.GetValue(target);
                }
                else
                {
                    // This could be a protected property on the base class.
                    // Make sure we're actually accessing through the base class,
                    // and then get the property directly from reflection.
                    if (GetExpression(manager, target) is CodeThisReferenceExpression)
                    {
                        PropertyInfo? propInfo = GetReflectionTypeHelper(manager, target).GetProperty(propertyReferenceEx.PropertyName, BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (propInfo is not null)
                        {
                            result = propInfo.GetValue(target, null);
                        }
                    }
                }
            }
            else
            {
                PropertyInfo? prop = GetReflectionTypeFromTypeHelper(manager, targetAsType).GetProperty(propertyReferenceEx.PropertyName, BindingFlags.GetProperty | BindingFlags.Static | BindingFlags.Public);
                if (prop is not null)
                {
                    result = prop.GetValue(null, null);
                }
            }

            if (result == propertyReferenceEx && reportError)
            {
                string? typeName = targetAsType is not null ? targetAsType.FullName : GetReflectionTypeHelper(manager, target).FullName;
                Error(manager, string.Format(SR.SerializerNoSuchProperty, typeName, propertyReferenceEx.PropertyName), SR.SerializerNoSuchProperty);
            }
        }

        return result;
    }

    private bool DeserializePropertyAssignStatement(IDesignerSerializationManager manager, CodeAssignStatement statement,
        CodePropertyReferenceExpression propertyReferenceEx, bool reportError)
    {
        object? lhs = DeserializeExpression(manager, null, propertyReferenceEx.TargetObject);

        if (lhs is not null and not CodeExpression)
        {
            // Property assignments must go through our type descriptor system. However, we do not support parameterized
            // properties. If there are any parameters on the property, we do not perform the assignment.
            PropertyDescriptorCollection properties = GetPropertiesHelper(manager, lhs, s_runTimeProperties);
            PropertyDescriptor? p = properties[propertyReferenceEx.PropertyName];
            if (p is not null)
            {
                object? rhs = DeserializeExpression(manager, null, statement.Right);
                if (rhs is CodeExpression)
                {
                    return false;
                }

                if (rhs is IConvertible ic && p.PropertyType != rhs.GetType())
                {
                    try
                    {
                        rhs = ic.ToType(p.PropertyType, null);
                    }
                    catch
                    {
                    }
                }

                // We need to ensure that no virtual types leak into the runtime code
                // So if we ever assign a property value to a Type -- we make sure that the Type is a
                // real System.Type.
                if (rhs is Type rhsType && rhsType.UnderlyingSystemType is not null)
                {
                    rhs = rhsType.UnderlyingSystemType; // unwrap this "type" that came because it was not actually a real bcl type.
                }

                // Next: see if the RHS of this expression was a property reference too. If it was, then
                // we will look for a MemberRelationshipService to record the relationship between these
                // two properties, if supported.
                // We need to setup this MemberRelationship before we actually set the property value.
                // If we do it the other way around the new property value will be pushed into the old
                // relationship, which isn't a problem during normal serialization (since it not very
                // likely the property has already been assigned to), but it does affect undo.
                MemberRelationship oldRelation = MemberRelationship.Empty;
                if (manager.TryGetService(out MemberRelationshipService? relationships))
                {
                    if (statement.Right is CodePropertyReferenceExpression rhsPropRef)
                    {
                        object rhsPropTarget = DeserializeExpression(manager, null, rhsPropRef.TargetObject)!;
                        PropertyDescriptor? rhsProp = GetPropertiesHelper(manager, rhsPropTarget, null)[rhsPropRef.PropertyName];

                        if (rhsProp is not null)
                        {
                            MemberRelationship source = new(lhs, p);
                            MemberRelationship target = new(rhsPropTarget, rhsProp);

                            oldRelation = relationships[source];

                            if (relationships.SupportsRelationship(source, target))
                            {
                                relationships[source] = target;
                            }
                        }
                    }
                    else
                    {
                        oldRelation = relationships[lhs, p];
                        relationships[lhs, p] = MemberRelationship.Empty;
                    }
                }

                try
                {
                    p.SetValue(lhs, rhs);
                }
                catch
                {
                    if (relationships is not null)
                    {
                        relationships[lhs, p] = oldRelation;
                    }

                    throw;
                }

                return true;
            }
            else
            {
                if (reportError)
                {
                    Error(manager, string.Format(SR.SerializerNoSuchProperty, lhs.GetType().FullName, propertyReferenceEx.PropertyName), SR.SerializerNoSuchProperty);
                }
            }
        }

        return false;
    }

    /// <summary>
    ///  This method returns an expression representing the given object. It may return null, indicating that
    ///  no expression has been set that describes the object. Expressions are acquired in one of three ways:
    ///  1. The expression could be the result of a prior SetExpression call.
    ///  2. The expression could have been found in the RootContext.
    ///  3. The expression could be derived through IReferenceService.
    ///  4. The current expression on the context stack has a PresetValue == value.
    ///  To derive expressions through IReferenceService, GetExpression asks the reference service if there
    ///  is a name for the given object. If the expression service returns a valid name, it checks to see if
    ///  there is a '.' in the name. This indicates that the expression service found this object as the return
    ///  value of a read only property on another object. If there is a '.', GetExpression will split the reference
    ///  into sub-parts. The leftmost part is a name that will be evaluated via manager.GetInstance. For each
    ///  subsequent part, a property reference expression will be built. The final expression will then be returned.
    ///  If the object did not have an expression set, or the object was not found in the reference service, null will
    ///  be returned from GetExpression, indicating there is no existing expression for the object.
    /// </summary>
    protected CodeExpression? GetExpression(IDesignerSerializationManager manager, object value)
    {
        CodeExpression? expression = null;

        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        // Is the expression part of a prior SetExpression call?

        if (manager.TryGetContext(out ExpressionTable? table))
        {
            expression = table.GetExpression(value);
        }

        // Check to see if this object represents the root context.
        if (expression is null)
        {
            if (manager.TryGetContext(out RootContext? rootEx) && ReferenceEquals(rootEx.Value, value))
            {
                expression = rootEx.Expression;
            }
        }

        // Now check IReferenceService.
        if (expression is null)
        {
            // perf: first try to retrieve objectName from DesignerSerializationManager
            // only then involve reference service if needed
            // this is done to avoid unnecessary ensuring\creating references

            string? objectName = manager.GetName(value);
            if (objectName is null || objectName.Contains('.'))
            {
                if (manager.GetService<IReferenceService>() is { } refSvc)
                {
                    objectName = refSvc.GetName(value);
                    if (objectName is not null && objectName.Contains('.'))
                    {
                        // This object name is built from sub objects. Assemble the graph of sub objects.
                        string[] nameParts = objectName.Split('.');

                        Debug.Assert(nameParts.Length > 0, "How can we fail to split when IndexOf succeeded?");

                        object? baseInstance = manager.GetInstance(nameParts[0]);

                        if (baseInstance is not null)
                        {
                            CodeExpression? baseExpression = SerializeToExpression(manager, baseInstance);

                            if (baseExpression is not null)
                            {
                                for (int idx = 1; idx < nameParts.Length; idx++)
                                {
                                    baseExpression = new CodePropertyReferenceExpression(baseExpression, nameParts[idx]);
                                }

                                expression = baseExpression;
                            }
                        }
                    }
                }
            }
        }

        // Finally, the expression context.
        if (expression is null)
        {
            if (manager.TryGetContext(out ExpressionContext? ctx) && ReferenceEquals(ctx.PresetValue, value))
            {
                expression = ctx.Expression;
            }
        }

        if (expression is not null)
        {
            // set up cache dependencies
            // we check to see if there is anything on the stack
            // if there is we make the parent entry a dependency of the current entry
            ComponentCache.Entry? parentEntry = manager.GetContext<ComponentCache.Entry>();

            object? parentEntryComponent = parentEntry?.Component;
            if (manager.TryGetContext(out ComponentCache? cache) && parentEntryComponent is not null &&
                parentEntryComponent != value /* don't make ourselves dependent with ourselves */)
            {
                ComponentCache.Entry? entry = cache.GetEntryAll(value);
                entry?.AddDependency(parentEntryComponent);
            }
        }

        return expression;
    }

    /// <summary>
    ///  Returns the serializer for the given value. This is cognizant that instance
    ///  attributes may be different from type attributes and will use a custom serializer
    ///  on the instance if it is present. If not, it will delegate to the serialization
    ///  manager.
    /// </summary>
    protected CodeDomSerializer? GetSerializer(IDesignerSerializationManager manager, object? value)
    {
        ArgumentNullException.ThrowIfNull(manager);

        if (value is not null)
        {
            AttributeCollection valueAttributes = GetAttributesHelper(manager, value);
            AttributeCollection typeAttributes = GetAttributesFromTypeHelper(manager, value.GetType());

            if (valueAttributes.Count != typeAttributes.Count)
            {
                // Ok, someone has stuffed custom attributes on this instance.
                // Since the serialization manager only takes types,
                // we've got to see if one of these custom attributes is a designer serializer attribute.
                string? valueSerializerTypeName = null;
                Type desiredSerializerType = typeof(CodeDomSerializer);
                DesignerSerializationManager? vsManager = manager as DesignerSerializationManager;
                foreach (Attribute a in valueAttributes)
                {
                    if (a is DesignerSerializerAttribute da)
                    {
                        Type? realSerializerType = vsManager is not null
                            ? vsManager.GetRuntimeType(da.SerializerBaseTypeName)
                            : manager.GetType(da.SerializerBaseTypeName!);

                        if (realSerializerType == desiredSerializerType)
                        {
                            valueSerializerTypeName = da.SerializerTypeName;
                            break;
                        }
                    }
                }

                // If we got a value serializer, we've got to do the same thing here for the type serializer.
                // We only care if the two are different
                if (valueSerializerTypeName is not null)
                {
                    foreach (Attribute a in typeAttributes)
                    {
                        if (a is DesignerSerializerAttribute da)
                        {
                            Type? realSerializerType = vsManager is not null
                                ? vsManager.GetRuntimeType(da.SerializerBaseTypeName)
                                : manager.GetType(da.SerializerBaseTypeName!);

                            if (realSerializerType == desiredSerializerType)
                            {
                                // Ok, we found a serializer. If it matches the one we found for the value, then we can still use the default implementation.
                                if (valueSerializerTypeName.Equals(da.SerializerTypeName))
                                {
                                    valueSerializerTypeName = null;
                                }

                                break;
                            }
                        }
                    }
                }

                // Finally, if we got a value serializer, we need to create it and use it.
                if (valueSerializerTypeName is not null)
                {
                    Type? serializerType = vsManager is not null
                        ? vsManager.GetRuntimeType(valueSerializerTypeName)
                        : manager.GetType(valueSerializerTypeName);

                    if (serializerType is not null && desiredSerializerType.IsAssignableFrom(serializerType))
                    {
                        return (CodeDomSerializer?)Activator.CreateInstance(serializerType);
                    }
                }
            }
        }

        // for serializing null, we pass null to the serialization manager otherwise,
        // external IDesignerSerializationProviders wouldn't be given a chance to
        // serialize null their own special way.
        Type? t = value?.GetType();

        return manager.GetSerializer<CodeDomSerializer>(t);
    }

    /// <summary>
    ///  Returns the serializer for the given value. This is cognizant that instance
    ///  attributes may be different from type attributes and will use a custom serializer
    ///  on the instance if it is present. If not, it will delegate to the serialization
    ///  manager.
    /// </summary>
    protected CodeDomSerializer? GetSerializer(IDesignerSerializationManager manager, Type valueType)
    {
        return manager.GetSerializer<CodeDomSerializer>(valueType);
    }

    protected bool IsSerialized(IDesignerSerializationManager manager, object value)
    {
        return IsSerialized(manager, value, false);
    }

    /// <summary>
    ///  This method returns true if the given value has been serialized before. For an object to
    ///  be considered serialized either it or another serializer must have called SetExpression, creating
    ///  a relationship between that object and a referring expression.
    /// </summary>
    protected bool IsSerialized(IDesignerSerializationManager manager, object value, bool honorPreset)
    {
        bool hasExpression = false;
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        // Is the expression part of a prior SetExpression call?
        if (manager.TryGetContext(out ExpressionTable? table))
        {
            hasExpression = honorPreset ? table.ContainsPresetExpression(value) : table.GetExpression(value) is not null;
        }

        return hasExpression;
    }

    /// <summary>
    ///  This method can be used to serialize an expression that represents the creation of the given object.
    ///  It is aware of instance descriptors and will return true for isComplete if the entire configuration for the
    ///  instance could be achieved.
    /// </summary>
    protected CodeExpression? SerializeCreationExpression(IDesignerSerializationManager manager, object value, out bool isComplete)
    {
        isComplete = false;
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        TypeConverter converter = TypeDescriptor.GetConverter(value);

        // See if there is an ExpressionContext with a preset value we're interested in. If so, that will dictate our creation expression.
        if (manager.TryGetContext(out ExpressionContext? ctx) && ReferenceEquals(ctx.PresetValue, value))
        {
            CodeExpression expression = ctx.Expression;
            // Okay, we found a preset creation expression. We just need to find if it isComplete.
            if (converter.CanConvertTo(typeof(InstanceDescriptor)))
            {
                if (converter.ConvertTo(value, typeof(InstanceDescriptor)) is InstanceDescriptor descriptor && descriptor.MemberInfo is not null)
                {
                    isComplete = descriptor.IsComplete;
                }
            }

            return expression;
        }

        // See if there is an instance descriptor for this type.
        if (converter.CanConvertTo(typeof(InstanceDescriptor)))
        {
            if (converter.ConvertTo(value, typeof(InstanceDescriptor)) is InstanceDescriptor descriptor && descriptor.MemberInfo is not null)
            {
                isComplete = descriptor.IsComplete;
                return SerializeInstanceDescriptor(manager, value, descriptor);
            }
        }

        // see if this thing is serializable
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        if (GetReflectionTypeHelper(manager, value).IsSerializable && (value as IComponent)?.Site is null)
        {
            CodeExpression? expression = SerializeToResourceExpression(manager, value);
            if (expression is not null)
            {
                isComplete = true;
                return expression;
            }
        }
#pragma warning restore SYSLIB0050

        // No instance descriptor. See if we can get to a public constructor that takes no arguments
        ConstructorInfo? ctor = GetReflectionTypeHelper(manager, value).GetConstructor([]);
        if (ctor is not null)
        {
            isComplete = false;
            return new CodeObjectCreateExpression(TypeDescriptor.GetClassName(value)!, []);
        }

        // Nothing worked.
        return null;
    }

    private CodeExpression? SerializeInstanceDescriptor(IDesignerSerializationManager manager, object value, InstanceDescriptor descriptor)
    {
        CodeExpression expression;

        // Serialize all of the arguments.
        CodeExpression[] arguments = new CodeExpression[descriptor.Arguments.Count];
        object?[] argumentValues = new object[arguments.Length];
        ParameterInfo[]? parameters = null;

        if (arguments.Length > 0)
        {
            descriptor.Arguments.CopyTo(argumentValues, 0);
            if (descriptor.MemberInfo is MethodBase mi)
            {
                parameters = mi.GetParameters();
            }
        }

        for (int i = 0; i < arguments.Length; i++)
        {
            Debug.Assert(argumentValues is not null && parameters is not null, "These should have been allocated when the argument array was created.");
            object? arg = argumentValues[i];
            CodeExpression? exp = null;
            ExpressionContext? newCtx = null;

            // If there is an ExpressionContext on the stack, we need to fix up its type to be the parameter type,
            // so the argument objects get serialized correctly.
            if (manager.TryGetContext(out ExpressionContext? ctx))
            {
                newCtx = new ExpressionContext(ctx.Expression, parameters[i].ParameterType, ctx.Owner);
                manager.Context.Push(newCtx);
            }

            try
            {
                exp = SerializeToExpression(manager, arg);
            }
            finally
            {
                if (newCtx is not null)
                {
                    Debug.Assert(manager.Context.Current == newCtx, "Context stack corrupted.");
                    manager.Context.Pop();
                }
            }

            if (exp is not null)
            {
                // Assign over. See if we need a cast first.
                if (arg is not null && !parameters[i].ParameterType.IsAssignableFrom(arg.GetType()))
                {
                    exp = new CodeCastExpression(parameters[i].ParameterType, exp);
                }

                arguments[i] = exp;
            }
            else
            {
                return null;
            }
        }

        Type expressionType = descriptor.MemberInfo!.DeclaringType!;
        CodeTypeReference typeRef = new(expressionType);

        if (descriptor.MemberInfo is ConstructorInfo)
        {
            expression = new CodeObjectCreateExpression(typeRef, arguments);
        }
        else if (descriptor.MemberInfo is MethodInfo methodInfo)
        {
            CodeTypeReferenceExpression typeRefExp = new(typeRef);
            CodeMethodReferenceExpression methodRef = new(typeRefExp, methodInfo.Name);
            expression = new CodeMethodInvokeExpression(methodRef, arguments);
            expressionType = methodInfo.ReturnType;
        }
        else if (descriptor.MemberInfo is PropertyInfo propertyInfo)
        {
            CodeTypeReferenceExpression typeRefExp = new(typeRef);
            CodePropertyReferenceExpression propertyRef = new(typeRefExp, propertyInfo.Name);
            Debug.Assert(arguments.Length == 0, "Property serialization does not support arguments");
            expression = propertyRef;
            expressionType = propertyInfo.PropertyType;
        }
        else if (descriptor.MemberInfo is FieldInfo fieldInfo)
        {
            Debug.Assert(arguments.Length == 0, "Field serialization does not support arguments");
            CodeTypeReferenceExpression typeRefExp = new(typeRef);
            expression = new CodeFieldReferenceExpression(typeRefExp, fieldInfo.Name);
            expressionType = fieldInfo.FieldType;
        }
        else
        {
            Debug.Fail($"Unrecognized reflection type in instance descriptor: {descriptor.MemberInfo.GetType().Name}");
            return null;
        }

        // Finally, check to see if our value is assignable from the expression type. If not, then supply a cast.
        // The value may be an internal or protected type; if it is, then walk up its hierarchy until we find one
        // that is public.
        Type targetType = value.GetType();
        while (!targetType.IsPublic)
        {
            targetType = targetType.BaseType!;
        }

        if (!targetType.IsAssignableFrom(expressionType))
        {
            expression = new CodeCastExpression(targetType, expression);
        }

        return expression;
    }

    /// <summary>
    ///  This method returns a unique name for the given object. It first calls GetName from the serialization
    ///  manager, and if this does not return a name if fabricates a name for the object. To fabricate a name
    ///  it uses the INameCreationService to create valid names. If the service is not available instead the
    ///  method will fabricate a name based on the short type name combined with an index number to make
    ///  it unique. The resulting name is associated with the serialization manager by calling SetName before
    ///  the new name is returned.
    /// </summary>
    protected string GetUniqueName(IDesignerSerializationManager manager, object value)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);

        string? name = manager.GetName(value);
        if (name is null)
        {
            string baseName;
            Type targetType = GetReflectionTypeHelper(manager, value);
            INameCreationService? ns = manager.GetService<INameCreationService>();
            if (ns is not null)
            {
                baseName = ns.CreateName(null, targetType);
            }
            else
            {
                baseName = targetType.Name.ToLower(CultureInfo.InvariantCulture);
            }

            int suffixIndex = 1;
            ComponentCache? cache = manager.GetContext<ComponentCache>();

            // Declare this name to the serializer. If there is already a name defined, keep trying.
            do
            {
                name = $"{baseName}{suffixIndex++}";
            }
            while (manager.GetInstance(name) is not null || (cache is not null && cache.ContainsLocalName(name)));

            manager.SetName(value, name);
            if (manager.TryGetContext(out ComponentCache.Entry? entry))
            {
                entry.AddLocalName(name);
            }
        }

        return name;
    }

    /// <summary>
    ///  This serializes a single event for the given object.
    /// </summary>
    protected void SerializeEvent(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, EventDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(statements);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(descriptor);

        // Now look for a MemberCodeDomSerializer for the property. If we can't find one, then we can't serialize the property
        manager.Context.Push(statements);
        manager.Context.Push(descriptor);
        try
        {
            MemberCodeDomSerializer? memberSerializer = manager.GetSerializer<MemberCodeDomSerializer>(descriptor.GetType());

            if (memberSerializer is not null && memberSerializer.ShouldSerialize(manager, value, descriptor))
            {
                memberSerializer.Serialize(manager, value, descriptor, statements);
            }
        }
        finally
        {
            Debug.Assert(manager.Context.Current == descriptor, "Context stack corrupted.");
            manager.Context.Pop();
            manager.Context.Pop();
        }
    }

    /// <summary>
    ///  This serializes all events for the given object.
    /// </summary>
    protected void SerializeEvents(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, params Attribute[]? filter)
    {
        EventDescriptorCollection events = GetEventsHelper(manager, value, filter).Sort();
        foreach (EventDescriptor evt in events)
        {
            SerializeEvent(manager, statements, value, evt);
        }
    }

    /// <summary>
    ///  This serializes all properties for the given object, using the provided filter.
    /// </summary>
    protected void SerializeProperties(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[]? filter)
    {
        PropertyDescriptorCollection properties = GetFilteredProperties(manager, value, filter).Sort();
        InheritanceAttribute? inheritance = (InheritanceAttribute?)GetAttributesHelper(manager, value)[typeof(InheritanceAttribute)];

        inheritance ??= InheritanceAttribute.NotInherited;

        manager.Context.Push(inheritance);
        try
        {
            foreach (PropertyDescriptor property in properties)
            {
                if (!property.Attributes.Contains(DesignerSerializationVisibilityAttribute.Hidden))
                {
                    SerializeProperty(manager, statements, value, property);
                }
            }
        }
        finally
        {
            Debug.Assert(manager.Context.Current == inheritance, "Somebody messed up our context stack.");
            manager.Context.Pop();
        }
    }

    private static PropertyDescriptorCollection GetFilteredProperties(IDesignerSerializationManager manager, object value, Attribute[]? filter)
    {
        PropertyDescriptorCollection props = GetPropertiesHelper(manager, value, filter);
        if (value is IComponent comp)
        {
            if (((IDictionary)props).IsReadOnly)
            {
                PropertyDescriptor[] propArray = new PropertyDescriptor[props.Count];
                props.CopyTo(propArray, 0);
                props = new PropertyDescriptorCollection(propArray);
            }

            PropertyDescriptor? filterProp = manager.Properties["FilteredProperties"];
            if (filterProp?.GetValue(manager) is ITypeDescriptorFilterService filterSvc)
            {
                filterSvc.FilterProperties(comp, props);
            }
        }

        return props;
    }

    /// <summary>
    ///  This method will inspect all of the properties on the given object fitting the filter, and check for that
    ///  property in a resource blob. This is useful for deserializing properties that cannot be represented
    ///  in code, such as design-time properties.
    /// </summary>
    protected void SerializePropertiesToResources(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, Attribute[]? filter)
    {
        PropertyDescriptorCollection props = GetPropertiesHelper(manager, value, filter);
        manager.Context.Push(statements);
        try
        {
            CodeExpression? target = SerializeToExpression(manager, value);
            if (target is not null)
            {
                CodePropertyReferenceExpression propertyRef = new(target, string.Empty);
                foreach (PropertyDescriptor property in props)
                {
                    ExpressionContext tree = new(propertyRef, property.PropertyType, value);
                    manager.Context.Push(tree);
                    try
                    {
                        if (property.Attributes.Contains(DesignerSerializationVisibilityAttribute.Visible))
                        {
                            propertyRef.PropertyName = property.Name;

                            string? name;
                            if (target is CodeThisReferenceExpression)
                            {
                                name = "$this";
                            }
                            else
                            {
                                name = manager.GetName(value);
                            }

                            name = $"{name}.{property.Name}";
                            ResourceCodeDomSerializer.SerializeMetadata(manager, name, property.GetValue(value), property.ShouldSerializeValue(value));
                        }
                    }
                    finally
                    {
                        Debug.Assert(manager.Context.Current == tree, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }
            }
        }
        finally
        {
            Debug.Assert(manager.Context.Current == statements, "Context stack corrupted.");
            manager.Context.Pop();
        }
    }

    /// <summary>
    ///  This serializes the given property for the given object.
    /// </summary>
    protected void SerializeProperty(IDesignerSerializationManager manager, CodeStatementCollection statements, object value, PropertyDescriptor propertyToSerialize)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(propertyToSerialize);
        ArgumentNullException.ThrowIfNull(statements);

        // Now look for a MemberCodeDomSerializer for the property. If we can't find one, then we can't serialize the property
        manager.Context.Push(statements);
        manager.Context.Push(propertyToSerialize);
        try
        {
            MemberCodeDomSerializer? memberSerializer = manager.GetSerializer<MemberCodeDomSerializer>(propertyToSerialize.GetType());
            if (memberSerializer is not null && memberSerializer.ShouldSerialize(manager, value, propertyToSerialize))
            {
                memberSerializer.Serialize(manager, value, propertyToSerialize, statements);
            }
        }
        finally
        {
            Debug.Assert(manager.Context.Current == propertyToSerialize, "Context stack corrupted.");
            manager.Context.Pop();
            manager.Context.Pop();
        }
    }

    /// <summary>
    ///  Writes the given resource value under the given name. The resource is written to the
    ///  current CultureInfo the user is using to design with.
    /// </summary>
    protected void SerializeResource(IDesignerSerializationManager manager, string resourceName, object? value)
    {
        ResourceCodeDomSerializer.WriteResource(manager, resourceName, value);
    }

    /// <summary>
    ///  Writes the given resource value under the given name. The resource is written to the
    ///  invariant culture.
    /// </summary>
    protected void SerializeResourceInvariant(IDesignerSerializationManager manager, string resourceName, object? value)
    {
        ResourceCodeDomSerializer.WriteResourceInvariant(manager, resourceName, value);
    }

    /// <summary>
    ///  This is a helper method that serializes a value to an expression. It will return a CodeExpression if the
    ///  value can be serialized, or null if it can't. SerializeToExpression uses the following rules for serializing
    ///  types:
    ///  1. It first calls GetExpression to see if an expression has already been created for the object. If so, it
    ///  returns the existing expression.
    ///  2. It then locates the object's serializer, and asks it to serialize.
    ///  3. If the return value of the object's serializer is a CodeExpression, the expression is returned.
    ///  4. It finally makes one last call to GetExpression to see if the serializer added an expression.
    ///  5. Finally, it returns null.
    ///  If no expression could be created and no suitable serializer could be found, an error will be
    ///  reported through the serialization manager. No error will be reported if a serializer was found
    ///  but it failed to produce an expression. It is assumed that the serializer either already reported
    ///  the error, or does not wish to serialize the object.
    /// </summary>
    protected CodeExpression? SerializeToExpression(IDesignerSerializationManager manager, object? value)
    {
        CodeExpression? expression = null;

        // We do several things here:
        // First, we check to see if there is already an expression for this object by
        //     calling IsSerialized / GetExpression.
        // Failing that we check GetLegacyExpression to see if we are working with an old serializer.
        // Failing that, we invoke the object's serializer.
        //     If that serializer returned a CodeExpression, we will use it.
        //     If the serializer did not return a code expression, we call GetExpression one last time to see
        //     if the serializer added an expression. If it did, we use it. Otherwise we return null.
        // If the serializer was invoked and it created one or more statements those statements
        //     will be added to a statement collection.
        // Additionally, if there is a statement context that contains a statement table for this object we
        //     will push that statement table onto the context stack in case someone else needs statements.
        if (value is not null)
        {
            if (IsSerialized(manager, value))
            {
                expression = GetExpression(manager, value);
            }
            else
            {
                expression = GetLegacyExpression(manager, value);
                if (expression is not null)
                {
                    SetExpression(manager, value, expression);
                }
            }
        }

        if (expression is null)
        {
            CodeDomSerializer? serializer = GetSerializer(manager, value);
            if (serializer is not null)
            {
                CodeStatementCollection? saveStatements = null;
                if (value is not null)
                {
                    // The Whidbey model for serializing a complex object is to call SetExpression with the object's reference
                    // expression and then call on the various Serialize Property / Event methods. This is incompatible with
                    // legacy code, and if not handled legacy code may serialize incorrectly or even stack fault. To handle
                    // this, we keep a private "Legacy Expression Table". This is a table that we fill in here. We don't fill
                    // in the actual legacy expression here. Rather, we fill it with a marker value and obtain the legacy
                    // expression above in GetLegacyExpression. If we hit this case, we then save the expression in GetExpression
                    // so that future calls to IsSerialized will succeed.
                    SetLegacyExpression(manager, value);
                    if (manager.TryGetContext(out StatementContext? statementCtx))
                    {
                        saveStatements = statementCtx.StatementCollection[value];
                    }

                    if (saveStatements is not null)
                    {
                        manager.Context.Push(saveStatements);
                    }
                }

                object? result = null;
                try
                {
                    result = serializer.Serialize(manager, value!);
                }
                finally
                {
                    if (saveStatements is not null)
                    {
                        Debug.Assert(manager.Context.Current == saveStatements, "Context stack corrupted.");
                        manager.Context.Pop();
                    }
                }

                expression = result as CodeExpression;
                if (expression is null && value is not null)
                {
                    expression = GetExpression(manager, value);
                }

                // If the result is a statement or a group of statements,
                // we need to see if there is a code statement collection on the stack we can push the statements into.
                CodeStatementCollection? statements = result as CodeStatementCollection;
                if (statements is null)
                {
                    if (result is CodeStatement statement)
                    {
                        statements = new CodeStatementCollection
                        {
                            statement
                        };
                    }
                }

                if (statements is not null)
                {
                    // See if we have a place for these statements to be stored. If not, then check the context.
                    saveStatements ??= manager.GetContext<CodeStatementCollection>();

                    if (saveStatements is not null)
                    {
                        Debug.Assert(saveStatements != statements, "The serializer returned the same collection that exists on the context stack.");
                        saveStatements.AddRange(statements);
                    }
                    else
                    {
                        // If we got here we will be losing data because we have no avenue to save these statements. Inform the user.
                        string valueName = "(null)";
                        if (value is not null)
                        {
                            valueName = manager.GetName(value) ?? value.GetType().Name;
                        }

                        manager.ReportError(string.Format(SR.SerializerLostStatements, valueName));
                    }
                }
            }
            else
            {
                manager.ReportError(string.Format(SR.SerializerNoSerializerForComponent, value is null ? "(null)" : value.GetType().FullName));
            }
        }

        return expression;
    }

    private static CodeExpression? GetLegacyExpression(IDesignerSerializationManager manager, object value)
    {
        CodeExpression? expression = null;
        if (manager.TryGetContext(out LegacyExpressionTable? table))
        {
            object? exp = table[value];
            if (exp == value)
            {
                // Sentinel. Compute an actual legacy expression to store.
                string? name = manager.GetName(value);
                bool referenceName = false;
                if (name is null)
                {
                    name = manager.GetService<IReferenceService>()?.GetName(value);
                    referenceName = name is not null;
                }

                if (name is not null)
                {
                    // Check to see if this is a reference to the root component. If it is, then use "this".
                    if (manager.TryGetContext(out RootContext? root))
                    {
                        if (root.Value == value)
                        {
                            expression = root.Expression;
                        }
                        else
                        {
                            int dotIndex = name.IndexOf('.');

                            if (referenceName && dotIndex >= 0)
                            {
                                // if it's a reference name with a dot, we've actually got a property here...
                                expression = new CodePropertyReferenceExpression(new CodeFieldReferenceExpression(root.Expression, name[..dotIndex]), name[(dotIndex + 1)..]);
                            }
                            else
                            {
                                expression = new CodeFieldReferenceExpression(root.Expression, name);
                            }
                        }
                    }
                    else
                    {
                        int dotIndex = name.IndexOf('.');

                        // A variable reference
                        if (referenceName && dotIndex >= 0)
                        {
                            // if it's a reference name with a dot, we've actually got a property here...
                            expression = new CodePropertyReferenceExpression(new CodeVariableReferenceExpression(name[..dotIndex]), name[(dotIndex + 1)..]);
                        }
                        else
                        {
                            expression = new CodeVariableReferenceExpression(name);
                        }
                    }
                }

                table[value] = expression;
            }
            else
            {
                expression = exp as CodeExpression;
            }
        }

        return expression;
    }

    private static void SetLegacyExpression(IDesignerSerializationManager manager, object value)
    {
        if (value is IComponent)
        {
            if (!manager.TryGetContext(out LegacyExpressionTable? table))
            {
                table = new LegacyExpressionTable();
                manager.Context.Append(table);
            }

            table[value] = value;
        }
    }

    private class LegacyExpressionTable : Hashtable
    {
    }

    /// <summary>
    ///  Serializes the given object to a resource and returns a code expression that represents the resource.
    ///  This will return null if the value cannot be serialized. If ensureInvariant is true, this will ensure that
    ///  new values make their way into the invariant culture. Normally, this is desirable. Otherwise a resource
    ///  GetValue call could fail if reading from a culture that doesn't have a value. You should only pass
    ///  false to ensureInvariant when you intend to read resources differently than directly asking for a value.
    ///  The default value of insureInvariant is true.
    /// </summary>
    protected CodeExpression? SerializeToResourceExpression(IDesignerSerializationManager manager, object? value)
    {
        return SerializeToResourceExpression(manager, value, true);
    }

    /// <summary>
    ///  Serializes the given object to a resource and returns a code expression that represents the resource.
    ///  This will return null if the value cannot be serialized. If ensureInvariant is true, this will ensure that
    ///  new values make their way into the invariant culture. Normally, this is desirable. Otherwise a resource
    ///  GetValue call could fail if reading from a culture that doesn't have a value. You should only pass
    ///  false to ensureInvariant when you intend to read resources differently than directly asking for a value.
    ///  The default value of insureInvariant is true.
    /// </summary>
    protected CodeExpression? SerializeToResourceExpression(IDesignerSerializationManager manager, object? value, bool ensureInvariant)
    {
        CodeExpression? result = null;
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        if (value is null || value.GetType().IsSerializable)
        {
            CodeStatementCollection? saveStatements = null;
            if (value is not null)
            {
                if (manager.TryGetContext(out StatementContext? statementCtx))
                {
                    saveStatements = statementCtx.StatementCollection[value];
                }

                if (saveStatements is not null)
                {
                    manager.Context.Push(saveStatements);
                }
            }

            try
            {
                result = ResourceCodeDomSerializer.Default.Serialize(manager, value, false, ensureInvariant) as CodeExpression;
            }
            finally
            {
                if (saveStatements is not null)
                {
                    Debug.Assert(manager.Context.Current == saveStatements, "Context stack corrupted.");
                    manager.Context.Pop();
                }
            }
        }
#pragma warning restore SYSLIB0050

        return result;
    }

    protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression)
    {
        SetExpression(manager, value, expression, false);
    }

    /// <summary>
    ///  This is a helper method that associates a CodeExpression with an object. Objects that have been associated
    ///  with expressions in this way are accessible through the GetExpression method. SetExpression stores its
    ///  expression table as an appended object on the context stack so it is accessible by any serializer's
    ///  GetExpression method.
    /// </summary>
    protected void SetExpression(IDesignerSerializationManager manager, object value, CodeExpression expression, bool isPreset)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(expression);

        if (!manager.TryGetContext(out ExpressionTable? table))
        {
            table = new ExpressionTable();
            manager.Context.Append(table);
        }

        table.SetExpression(value, expression, isPreset);
    }

    internal static void FillStatementTable(IDesignerSerializationManager manager, IDictionary table, CodeStatementCollection statements)
    {
        FillStatementTable(manager, table, null, statements, null);
    }

    internal static void FillStatementTable(IDesignerSerializationManager manager, IDictionary table, Dictionary<string, string>? names, CodeStatementCollection statements, string? className)
    {
        // Look in the method body to try to find statements with a LHS that points to a name in our nametable.
        foreach (CodeStatement statement in statements)
        {
            CodeExpression? expression = null;
            if (statement is CodeAssignStatement assign)
            {
                expression = assign.Left;
            }
            else if (statement is CodeAttachEventStatement attachEvent)
            {
                expression = attachEvent.Event;
            }
            else if (statement is CodeRemoveEventStatement removeEvent)
            {
                expression = removeEvent.Event;
            }
            else if (statement is CodeExpressionStatement expressionStmt)
            {
                expression = expressionStmt.Expression;
            }
            else if (statement is CodeVariableDeclarationStatement variableDecl)
            {
                AddStatement(table, variableDecl.Name, variableDecl);

                if (names is not null && variableDecl.Type is not null && !string.IsNullOrEmpty(variableDecl.Type.BaseType))
                {
                    names[variableDecl.Name] = GetTypeNameFromCodeTypeReference(manager, variableDecl.Type);
                }

                expression = null;
            }

            if (expression is not null)
            {
                // Simplify the expression as much as we can, looking for our target object in the process. If we find an
                // expression that refers to our target object, we're done and can move on to the next statement.
                while (true)
                {
                    if (expression is CodeCastExpression castEx)
                    {
                        expression = castEx.Expression;
                    }
                    else if (expression is CodeDelegateCreateExpression delegateCreateEx)
                    {
                        expression = delegateCreateEx.TargetObject;
                    }
                    else if (expression is CodeDelegateInvokeExpression delegateInvokeEx)
                    {
                        expression = delegateInvokeEx.TargetObject;
                    }
                    else if (expression is CodeDirectionExpression directionEx)
                    {
                        expression = directionEx.Expression;
                    }
                    else if (expression is CodeEventReferenceExpression eventReferenceEx)
                    {
                        expression = eventReferenceEx.TargetObject;
                    }
                    else if (expression is CodeMethodInvokeExpression methodInvokeEx)
                    {
                        expression = methodInvokeEx.Method;
                    }
                    else if (expression is CodeMethodReferenceExpression methodReferenceEx)
                    {
                        expression = methodReferenceEx.TargetObject;
                    }
                    else if (expression is CodeArrayIndexerExpression arrayIndexerEx)
                    {
                        expression = arrayIndexerEx.TargetObject;
                    }
                    else if (expression is CodeFieldReferenceExpression fieldReferenceEx)
                    {
                        // For fields we need to check to see if the field name is equal to the target object.
                        // If it is, then we have the expression we want. We can add the statement here
                        // and then break out of our loop.
                        // Note:  We cannot validate that this is a name in our nametable.
                        // The nametable only contains names we have discovered through
                        // code parsing and will not include data from any inherited objects.
                        // We accept the field now, and then fail later if we try to resolve
                        // it to an object and we can't find it.
                        bool addedStatement = false;
                        if (fieldReferenceEx.TargetObject is CodeThisReferenceExpression)
                        {
                            Type? type = GetType(manager, fieldReferenceEx.FieldName, names);
                            if (type is not null)
                            {
                                if (manager.TryGetSerializer(type, out CodeDomSerializer? serializer))
                                {
                                    string? componentName = serializer.GetTargetComponentName(statement, expression, type);
                                    if (!string.IsNullOrEmpty(componentName))
                                    {
                                        AddStatement(table, componentName, statement);
                                        addedStatement = true;
                                    }
                                }
                            }

                            if (!addedStatement)
                            {
                                // we still want to do this in case of the "Note" above.
                                AddStatement(table, fieldReferenceEx.FieldName, statement);
                            }

                            break;
                        }
                        else
                        {
                            expression = fieldReferenceEx.TargetObject;
                        }
                    }
                    else if (expression is CodePropertyReferenceExpression propertyReferenceEx)
                    {
                        // For properties we need to check to see if the property name is equal to the target object.
                        // If it is, then we have the expression we want. We can add the statement here and
                        // then break out of our loop.
                        if (propertyReferenceEx.TargetObject is CodeThisReferenceExpression && (names is null || names.ContainsKey(propertyReferenceEx.PropertyName)))
                        {
                            AddStatement(table, propertyReferenceEx.PropertyName, statement);
                            break;
                        }
                        else
                        {
                            expression = propertyReferenceEx.TargetObject;
                        }
                    }
                    else if (expression is CodeVariableReferenceExpression variableReferenceEx)
                    {
                        // For variables we need to check to see if the variable name is equal to the target object.
                        // If it is, then we have the expression we want. We can add the statement here and
                        // then break out of our loop.
                        bool statementAdded = false;
                        if (names is not null)
                        {
                            Type? type = GetType(manager, variableReferenceEx.VariableName, names);
                            if (type is not null)
                            {
                                if (manager.TryGetSerializer(type, out CodeDomSerializer? serializer))
                                {
                                    string? componentName = serializer.GetTargetComponentName(statement, expression, type);
                                    if (!string.IsNullOrEmpty(componentName))
                                    {
                                        AddStatement(table, componentName, statement);
                                        statementAdded = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            AddStatement(table, variableReferenceEx.VariableName, statement);
                            statementAdded = true;
                        }

                        if (!statementAdded)
                        {
                            manager.ReportError(new CodeDomSerializerException(string.Format(SR.SerializerUndeclaredName, variableReferenceEx.VariableName), manager));
                        }

                        break;
                    }
                    else if (expression is CodeThisReferenceExpression or CodeBaseReferenceExpression)
                    {
                        // We cannot go any further than "this". So, we break out of the loop. We file this statement under the root object.
                        Debug.Assert(className is not null, "FillStatementTable expected a valid className but received null");
                        if (className is not null)
                        {
                            AddStatement(table, className, statement);
                        }

                        break;
                    }
                    else
                    {
                        // We cannot simplify this expression any further, so we stop looping.
                        break;
                    }
                }
            }
        }
    }

    internal static Type? GetType(IDesignerSerializationManager manager, string name, Dictionary<string, string>? names)
    {
        Type? type = null;
        if (names is not null && names.TryGetValue(name, out string? typeName))
        {
            if (manager is not null && !string.IsNullOrEmpty(typeName))
            {
                type = manager.GetType(typeName);
            }
        }

        return type;
    }

    private static void AddStatement(IDictionary table, string name, CodeStatement statement)
    {
        OrderedCodeStatementCollection? statements;
        if (table.Contains(name))
        {
            statements = (OrderedCodeStatementCollection)table[name]!;
        }
        else
        {
            // push in an order key so we know what position this item was in the list of declarations. this allows us to preserve ZOrder.
            statements = new OrderedCodeStatementCollection(table.Count, name);
            table[name] = statements;
        }

        statements.Add(statement);
    }

    internal class OrderedCodeStatementCollection : CodeStatementCollection
    {
        public readonly int Order;
        public readonly string Name;

        public OrderedCodeStatementCollection(int order, string name)
        {
            Order = order;
            Name = name;
        }
    }
}
