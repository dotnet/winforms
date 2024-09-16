// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.Resources;

namespace System.ComponentModel.Design.Serialization;

/// <summary>
///  Code model serializer for resource managers. This is called
///  in one of two ways. On Deserialization, we are associated
///  with a ResourceManager object. Instead of creating a
///  ResourceManager, however, we create an object called a
///  SerializationResourceManager. This class inherits
///  from ResourceManager, but overrides all of the methods.
///  Instead of letting resource manager maintain resource
///  sets, it uses the designer host's IResourceService
///  for this purpose.
///
///  During serialization, this class will also create
///  a SerializationResourceManager. This will be added
///  to the serialization manager as a service so other
///  resource serializers can get at it. SerializationResourceManager
///  has additional methods on it to support writing data
///  into the resource streams for various cultures.
/// </summary>
internal class LocalizationCodeDomSerializer : CodeDomSerializer
{
    private readonly CodeDomLocalizationModel _model;
    private readonly CodeDomSerializer? _currentSerializer;

    /// <summary>
    ///  Only we can create an instance of this. Everyone else accesses it though
    ///  static properties.
    /// </summary>
    internal LocalizationCodeDomSerializer(CodeDomLocalizationModel model, object currentSerializer)
    {
        _model = model;
        _currentSerializer = currentSerializer as CodeDomSerializer;
    }

    /// <summary>
    ///  Returns true if we should emit an ApplyResources method for this object. We only emit
    ///  this method once during serialization, and we track this by appending an object to
    ///  the context stack.
    /// </summary>
    private static bool EmitApplyMethod(IDesignerSerializationManager manager, object owner)
    {
        ApplyMethodTable? table = (ApplyMethodTable?)manager.Context[typeof(ApplyMethodTable)];

        if (table is null)
        {
            table = new();
            manager.Context.Append(table);
        }

        return table.Add(owner);
    }

    /// <summary>
    ///  Serializes the given object into a CodeDom object. This uses the stock
    ///  resource serialization scheme and retains the expression it provides.
    /// </summary>
    public override object? Serialize(IDesignerSerializationManager manager, object value)
    {
        PropertyDescriptor? descriptor = (PropertyDescriptor?)manager.Context[typeof(PropertyDescriptor)];
        ExpressionContext? tree = (ExpressionContext?)manager.Context[typeof(ExpressionContext)];
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        bool isSerializable = value is null || GetReflectionTypeHelper(manager, value).IsSerializable;
#pragma warning restore SYSLIB0050

        // If value is not serializable, we have no option but to call the original serializer,
        // since we cannot push this into resources.
        bool callExistingSerializer = !isSerializable;

        // Compat: If we are serializing content, we need to skip property reflection to preserve compatibility,
        //         since tools like WinRes expect items in collections (like TreeNodes and ListViewItems)
        //         to be serialized as binary blobs.
        bool serializingContent = (descriptor is not null && descriptor.Attributes.Contains(DesignerSerializationVisibilityAttribute.Content));

        // We also skip back to the original serializer if there is a preset value for this object.
        if (!callExistingSerializer)
        {
            callExistingSerializer = tree is not null && tree.PresetValue is not null && tree.PresetValue == value;
        }

        if (_model == CodeDomLocalizationModel.PropertyReflection && !serializingContent && !callExistingSerializer)
        {
            // For a property reflecting model, we need to do more work. Here we need to find
            // the object we are serializing against and inject an "ApplyResources" method
            // against the object and its name. If any of this machinery fails we will
            // just return the existing expression which will default to the original behavior.
            CodeStatementCollection? statements = (CodeStatementCollection?)manager.Context[typeof(CodeStatementCollection)];

            // In the case of extender properties, we don't want to serialize using the property
            // reflecting model. In this case we'll skip it and fall through to the
            // property assignment model.
            bool skipPropertyReflect = false;

            if (descriptor is not null)
            {
                ExtenderProvidedPropertyAttribute? attribute = descriptor.Attributes[typeof(ExtenderProvidedPropertyAttribute)] as ExtenderProvidedPropertyAttribute;
                if (attribute is not null && attribute.ExtenderProperty is not null)
                {
                    skipPropertyReflect = true;
                }
            }

            if (!skipPropertyReflect && tree is not null && statements is not null)
            {
                string? name = manager.GetName(tree.Owner);
                CodeExpression? ownerExpression = SerializeToExpression(manager, tree.Owner);

                if (name is not null && ownerExpression is not null)
                {
                    RootContext? rootContext = manager.Context[typeof(RootContext)] as RootContext;

                    if (rootContext is not null && rootContext.Value == tree.Owner)
                    {
                        name = "$this";
                    }

                    // Ok, if we got here it means we have enough data to emit
                    // using the reflection model.
                    SerializeToResourceExpression(manager, value, false);

                    if (EmitApplyMethod(manager, tree.Owner))
                    {
                        ResourceManager? resourceManager = manager.Context[typeof(ResourceManager)] as ResourceManager;
                        Debug.Assert(resourceManager is not null, "No resource manager available in context.");
                        CodeExpression? rmExpression = GetExpression(manager, resourceManager);
                        Debug.Assert(rmExpression is not null, "No expression available for resource manager.");

                        CodeMethodInvokeExpression methodInvoke = new()
                        {
                            Method = new CodeMethodReferenceExpression(rmExpression, "ApplyResources")
                        };
                        methodInvoke.Parameters.Add(ownerExpression);
                        methodInvoke.Parameters.Add(new CodePrimitiveExpression(name));
                        statements.Add(methodInvoke);
                    }

                    return null; // we have already worked our statements into the tree.
                }
            }
        }

        return callExistingSerializer
            ? _currentSerializer?.Serialize(manager, value!)
            : SerializeToResourceExpression(manager, value);
    }

    /// <summary>
    ///  This class is used as a table to track which objects we've injected the "ApplyResources" method for.
    /// </summary>
    private class ApplyMethodTable
    {
        private readonly HashSet<object> _table = [];

        internal bool Add(object value) => _table.Add(value);
    }
}
