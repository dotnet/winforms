// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Collections;
using System.Globalization;
using System.Resources;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
/// Code model serializer for resource managers.
/// This is called in one of two ways.
/// On Deserialization, we are associated with a ResourceManager object.
/// Instead of creating a ResourceManager, however, we create an object called a SerializationResourceManager.
/// This class inherits from ResourceManager, but overrides all of the methods.
/// Instead of letting resource manager maintain resource sets, it uses the designer host's IResourceService for this purpose.
/// During serialization, this class will also create a SerializationResourceManager.
/// This will be added to the serialization manager as a service so other resource serializers can get at it.
/// SerializationResourceManager has additional methods on it to support writing data into
/// the resource streams for various cultures.
/// </summary>
internal partial class ResourceCodeDomSerializer : CodeDomSerializer
{
    private static ResourceCodeDomSerializer? s_defaultSerializer;

    /// <summary>
    ///  Retrieves a default static instance of this serializer.
    /// </summary>
    internal static new ResourceCodeDomSerializer Default => s_defaultSerializer ??= new ResourceCodeDomSerializer();

    public override string? GetTargetComponentName(CodeStatement? statement, CodeExpression? expression, Type? type)
    {
        string? name = null;
        if (statement is CodeExpressionStatement { Expression: CodeMethodInvokeExpression methodInvokeEx })
        {
            if (string.Equals(methodInvokeEx.Method?.MethodName, "ApplyResources", StringComparison.OrdinalIgnoreCase))
            {
                name = methodInvokeEx.Parameters switch
                {
                // We've found a call to the ApplyResources method on a ComponentResourceManager object. now we just need to figure out
                // which component ApplyResources is being called for, and put it into that component's bucket.
                [CodeFieldReferenceExpression { TargetObject: CodeThisReferenceExpression } fieldReferenceEx, ..] => fieldReferenceEx.FieldName,
                [CodeVariableReferenceExpression variableReferenceEx, ..] => variableReferenceEx.VariableName,
                    _ => null
                };
            }
        }

        if (string.IsNullOrEmpty(name))
        {
            name = base.GetTargetComponentName(statement, expression, type);
        }

        return name;
    }

    /// <summary>
    ///  This is the name of the resource manager object we declare on the component surface.
    /// </summary>
    private const string ResourceManagerName = "resources";

    /// <summary>
    ///  Deserializes the given CodeDom object into a real object.
    ///  This will use the serialization manager to create objects and resolve data types.
    ///  The root of the object graph is returned.
    /// </summary>
    public override object? Deserialize(IDesignerSerializationManager manager, object codeObject)
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
                // We create the resource manager ourselves here because it's not just a straight parse of the code.
                // Do special parsing of the resources statement
                if (element is CodeVariableDeclarationStatement statement)
                {
                    if (statement.Name.Equals(ResourceManagerName))
                    {
                        instance = CreateResourceManager(manager);
                    }
                }
                else
                {
                    // If we do not yet have an instance,
                    // we will need to pick through the statements and see if we can find one.
                    if (instance is null)
                    {
                        instance = DeserializeStatementToInstance(manager, element);
                    }
                    else
                    {
                        DeserializeStatement(manager, element);
                    }
                }
            }
        }
        else if (codeObject is not CodeStatement)
        {
            Debug.Fail("ResourceCodeDomSerializer::Deserialize requires a CodeExpression, CodeStatement or CodeStatementCollection to parse");
            string supportedTypes = $"{nameof(CodeExpression)}, {nameof(CodeStatement)}, {nameof(CodeStatementCollection)}";
            throw new ArgumentException(string.Format(SR.SerializerBadElementTypes, codeObject.GetType().Name, supportedTypes));
        }

        return instance;
    }

    private static SerializationResourceManager CreateResourceManager(IDesignerSerializationManager manager)
    {
        SerializationResourceManager sm = GetResourceManager(manager);

        if (!sm.DeclarationAdded)
        {
            sm.DeclarationAdded = true;
            manager.SetName(sm, ResourceManagerName);
        }

        return sm;
    }

    /// <summary>
    ///  This method is invoked during deserialization to obtain an instance of an object. When this is called, an
    ///  instance of the requested type should be returned. Our implementation provides a design time resource manager.
    /// </summary>
    protected override object DeserializeInstance(IDesignerSerializationManager manager, Type type, object?[]? parameters, string? name, bool addToContainer)
    {
        ArgumentNullException.ThrowIfNull(manager);
        ArgumentNullException.ThrowIfNull(type);

        if (name is not null && name.Equals(ResourceManagerName) && typeof(ResourceManager).IsAssignableFrom(type))
        {
            return CreateResourceManager(manager);
        }
        else
        {
            // if it isn't our special resource manager, just create it.
            return manager.CreateInstance(type, parameters, name, addToContainer);
        }
    }

    /// <summary>
    ///  Deserializes the given CodeDom object into a real object. This will use the serialization manager to create
    ///  objects and resolve data types. It uses the invariant resource blob to obtain resources.
    /// </summary>
    public static object? DeserializeInvariant(IDesignerSerializationManager manager, string resourceName)
    {
        SerializationResourceManager resources = GetResourceManager(manager);
        return resources.GetObject(resourceName, true);
    }

    /// <summary>
    ///  Try to discover the data type we should apply a cast for. To do this, we first search the context stack for
    ///  an ExpressionContext to decrypt, and if we fail that we try the actual object. If we can't find a cast type
    ///  we return null.
    /// </summary>
    [return: NotNullIfNotNull(nameof(value))]
    private static Type? GetCastType(IDesignerSerializationManager manager, object? value)
    {
        // Is there an ExpressionContext we can work with?
        if (manager.TryGetContext(out ExpressionContext? tree))
        {
            return tree.ExpressionType;
        }

        // Party on the object, if we can. It is the best identity we can get.
        if (value is not null)
        {
            Type castTo = value.GetType();
            while (!castTo.IsPublic && !castTo.IsNestedPublic)
            {
                castTo = castTo.BaseType!;
            }

            return castTo;
        }

        // Object is null. Nothing we can do
        return null;
    }

    /// <summary>
    ///  Retrieves a dictionary enumerator for the requested culture, or null if no resources for that culture exist.
    /// </summary>
    public static IDictionaryEnumerator? GetEnumerator(IDesignerSerializationManager manager, CultureInfo culture)
    {
        SerializationResourceManager resources = GetResourceManager(manager);
        return resources.GetEnumerator(culture);
    }

    /// <summary>
    ///  Retrieves a dictionary enumerator for the requested culture, or null if no resources for that culture exist.
    /// </summary>
    public static IDictionaryEnumerator? GetMetadataEnumerator(IDesignerSerializationManager manager)
    {
        SerializationResourceManager resources = GetResourceManager(manager);
        return resources.GetMetadataEnumerator();
    }

    /// <summary>
    ///  Demand creates the serialization resource manager. Stores the manager as an appended context value.
    /// </summary>
    private static SerializationResourceManager GetResourceManager(IDesignerSerializationManager manager)
    {
        if (!manager.TryGetContext(out SerializationResourceManager? sm))
        {
            sm = new SerializationResourceManager(manager);
            manager.Context.Append(sm);
        }

        return sm;
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    ///  This expects the following values to be available on the context stack:
    ///  A CodeStatementCollection that we can add our resource declaration to, if necessary.
    ///  An ExpressionContext that contains the property, field or method that is being serialized,
    ///  along with the object being serialized. We need this so we can create a unique resource name for the object.
    /// </summary>
    public override object Serialize(IDesignerSerializationManager manager, object value)
    {
        return Serialize(manager, value, false, false, true);
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object. This expects the following values
    ///  to be available on the context stack: A CodeStatementCollection that we can add our resource declaration to,
    ///  if necessary. An ExpressionContext that contains the property, field or method that is being serialized,
    ///  along with the object being serialized. We need this so we can create a unique resource name for the object.
    /// </summary>
    public object Serialize(IDesignerSerializationManager manager, object value, bool shouldSerializeInvariant)
    {
        return Serialize(manager, value, false, shouldSerializeInvariant, true);
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object.
    ///  This expects the following values to be available on the context stack:
    ///  A CodeStatementCollection that we can add our resource declaration to, if necessary.
    ///  An ExpressionContext that contains the property, field or method that is being serialized,
    ///  along with the object being serialized. We need this so we can create a unique resource name for the object.
    /// </summary>
    public object Serialize(IDesignerSerializationManager manager, object? value, bool shouldSerializeInvariant, bool ensureInvariant)
    {
        return Serialize(manager, value, false, shouldSerializeInvariant, ensureInvariant);
    }

    /// <summary>
    ///  This performs the actual work of serialization between Serialize and SerializeInvariant.
    /// </summary>
    private CodeExpression Serialize(IDesignerSerializationManager manager, object? value, bool forceInvariant, bool shouldSerializeInvariant, bool ensureInvariant)
    {
        // Resource serialization is a little inconsistent. We deserialize our own resource manager creation statement,
        // but we will never be asked to serialize a resource manager,
        // because it doesn't exist as a product of the design container; it is purely an artifact of serializing.
        // Some not-so-obvious side effects of this are:
        //     This method will never ever be called by the serialization system directly.
        //     There is no attribute or metadata that will invoke it.
        //     Instead, other serializers will call this method to see if we should serialize to resources.
        //     We need a way to inject the local variable declaration into the method body
        //     for the resource manager if we actually do emit a resource,which we shove into the statements collection.
        SerializationResourceManager sm = GetResourceManager(manager);
        CodeStatementCollection? statements = manager.GetContext<CodeStatementCollection>();
        // If this serialization resource manager has never been used to output culture-sensitive statements,
        // then we must emit the local variable hookup. Culture invariant statements are used to save
        // random data that is not representable in code, so there is no need to emit a declaration.
        if (!forceInvariant)
        {
            if (!sm.DeclarationAdded)
            {
                sm.DeclarationAdded = true;

                // If we have a root context, then we can write out a reasonable resource manager constructor.
                // If not, then we're a bit hobbled because we have to guess at the resource name.
                if (statements is not null)
                {
                    CodeExpression[] parameters;
                    if (manager.TryGetContext(out RootContext? rootCtx))
                    {
                        string? baseType = manager.GetName(rootCtx.Value);
                        parameters = [new CodeTypeOfExpression(baseType!)];
                    }
                    else
                    {
                        parameters = [new CodePrimitiveExpression(ResourceManagerName)];
                    }

                    CodeExpression initExpression = new CodeObjectCreateExpression(typeof(ComponentResourceManager), parameters);
                    statements.Add(new CodeVariableDeclarationStatement(typeof(ComponentResourceManager), ResourceManagerName, initExpression));
                    SetExpression(manager, sm, new CodeVariableReferenceExpression(ResourceManagerName));
                    sm.ExpressionAdded = true;
                }
            }
            else
            {
                // Check to see if we have an expression for SM yet.
                // If we have cached the declaration in the component cache,
                // the expression may not be setup so we should re-apply it.
                if (!sm.ExpressionAdded)
                {
                    if (GetExpression(manager, sm) is null)
                    {
                        SetExpression(manager, sm, new CodeVariableReferenceExpression(ResourceManagerName));
                    }

                    sm.ExpressionAdded = true;
                }
            }
        }

        // Retrieve the ExpressionContext on the context stack, and save the value as a resource.
        ExpressionContext? tree = manager.GetContext<ExpressionContext>();
        string resourceName = sm.SetValue(manager, tree, value, forceInvariant, shouldSerializeInvariant, ensureInvariant, false);

        // Now the next step is to discover the type of the given value. If it is a string,
        // we will invoke "GetString"  Otherwise, we will invoke "GetObject" and supply a cast to the proper value.
        bool needCast;
        string methodName;

        if (value is string || (tree is not null && tree.ExpressionType == typeof(string)))
        {
            needCast = false;
            methodName = "GetString";
        }
        else
        {
            needCast = true;
            methodName = "GetObject";
        }

        // Finally, all we need to do is create a CodeExpression that represents the resource manager method invoke.
        CodeMethodInvokeExpression methodInvoke = new CodeMethodInvokeExpression
        {
            Method = new CodeMethodReferenceExpression(new CodeVariableReferenceExpression(ResourceManagerName), methodName)
        };

        methodInvoke.Parameters.Add(new CodePrimitiveExpression(resourceName));
        if (needCast)
        {
            Type? castTo = GetCastType(manager, value);
            if (castTo is not null)
            {
                return new CodeCastExpression(castTo, methodInvoke);
            }
        }

        return methodInvoke;
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object saving resources in the invariant culture,
    ///  rather than the current culture. This expects the following values to be available on the context stack:
    ///  A CodeStatementCollection that we can add our resource declaration to, if necessary.
    ///  An ExpressionContext that contains the property, field or method that is being serialized,
    ///  along with the object being serialized. We need this so we can create a unique resource name for the object.
    /// </summary>
    public object SerializeInvariant(IDesignerSerializationManager manager, object value, bool shouldSerializeValue)
    {
        return Serialize(manager, value, true, shouldSerializeValue, true);
    }

    /// <summary>
    ///  Writes out the given metadata.
    /// </summary>
    public static void SerializeMetadata(IDesignerSerializationManager manager, string name, object? value, bool shouldSerializeValue)
    {
        SerializationResourceManager sm = GetResourceManager(manager);
        sm.SetMetadata(manager, name, value, shouldSerializeValue, false);
    }

    /// <summary>
    ///  Serializes the given resource value into the resource set. This does not effect the code dom values.
    ///  The resource is written into the current culture.
    /// </summary>
    public static void WriteResource(IDesignerSerializationManager manager, string name, object? value)
    {
        SerializationResourceManager sm = GetResourceManager(manager);
        sm.SetValue(manager, name, value, forceInvariant: false, shouldSerializeInvariant: false, ensureInvariant: true, applyingCachedResources: false);
    }

    /// <summary>
    ///  Serializes the given resource value into the resource set. This does not effect the code dom values.
    ///  The resource is written into the invariant culture.
    /// </summary>
    public static void WriteResourceInvariant(IDesignerSerializationManager manager, string name, object? value)
    {
        SerializationResourceManager sm = GetResourceManager(manager);
        sm.SetValue(manager, name, value, forceInvariant: true, shouldSerializeInvariant: true, ensureInvariant: true, applyingCachedResources: false);
    }

    /// <summary>
    ///  This is called by the component code dom serializer's caching logic to save cached resource data back into the resx files.
    /// </summary>
    internal static void ApplyCacheEntry(IDesignerSerializationManager manager, ComponentCache.Entry entry)
    {
        SerializationResourceManager sm = GetResourceManager(manager);
        if (entry.Metadata is not null)
        {
            foreach (ComponentCache.ResourceEntry re in entry.Metadata)
            {
                sm.SetMetadata(manager, re.Name, re.Value, re.ShouldSerializeValue, true);
            }
        }

        if (entry.Resources is not null)
        {
            foreach (ComponentCache.ResourceEntry re in entry.Resources)
            {
                // All ResourceEntry objects added to the Resources collection should have a PropertyDescriptor and an ExpressionContext
                manager.Context.Push(re.PropertyDescriptor!);
                manager.Context.Push(re.ExpressionContext!);
                try
                {
                    sm.SetValue(manager, re.Name, re.Value, re.ForceInvariant, re.ShouldSerializeValue, re.EnsureInvariant, true);
                }
                finally
                {
                    Debug.Assert(manager.Context.Current == re.ExpressionContext, "Someone corrupted the context stack");
                    manager.Context.Pop();
                    Debug.Assert(manager.Context.Current == re.PropertyDescriptor, "Someone corrupted the context stack");
                    manager.Context.Pop();
                }
            }
        }
    }
}
