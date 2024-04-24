// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Serialization;

namespace System.Windows.Forms.BinaryFormat;

internal sealed class SerializationEvents
{
    private static readonly ConcurrentDictionary<Type, SerializationEvents> s_cache = new();

    private static readonly SerializationEvents s_noEvents = new();

    private readonly List<MethodInfo>? _onDeserializingMethods;
    private readonly List<MethodInfo>? _onDeserializedMethods;

    private SerializationEvents() { }

    private SerializationEvents(
        List<MethodInfo>? onDeserializingMethods,
        List<MethodInfo>? onDeserializedMethods)
    {
        _onDeserializingMethods = onDeserializingMethods;
        _onDeserializedMethods = onDeserializedMethods;
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2111:UnrecognizedReflectionPattern",
        Justification = "The Type is annotated correctly, it just can't pass through the lambda method.")]
    internal static SerializationEvents GetSerializationEventsForType(
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type t) =>
        s_cache.GetOrAdd(t, CreateSerializationEvents);

    private static SerializationEvents CreateSerializationEvents([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type type)
    {
        List<MethodInfo>? onDeserializingMethods = GetMethodsWithAttribute(typeof(OnDeserializingAttribute), type);
        List<MethodInfo>? onDeserializedMethods = GetMethodsWithAttribute(typeof(OnDeserializedAttribute), type);

        return onDeserializingMethods is null && onDeserializedMethods is null
            ? s_noEvents
            : new SerializationEvents(onDeserializingMethods, onDeserializedMethods);
    }

    private static List<MethodInfo>? GetMethodsWithAttribute(
        Type attribute,
        // Currently the only way to preserve base, non-public methods is to use All
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] Type? type)
    {
        List<MethodInfo>? attributedMethods = null;

        // Traverse the hierarchy to find all methods with the particular attribute
        Type? baseType = type;
        while (baseType is not null && baseType != typeof(object))
        {
            // Get all methods which are declared on this type, instance and public or nonpublic
            MethodInfo[] methods = baseType.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            foreach (MethodInfo method in methods)
            {
                // For each method find if attribute is present, the return type is void and the method is not virtual
                if (method.IsDefined(attribute, inherit: false))
                {
                    attributedMethods ??= [];
                    attributedMethods.Add(method);
                }
            }

            baseType = baseType.BaseType;
        }

        // We should invoke the methods starting from base
        attributedMethods?.Reverse();

        return attributedMethods;
    }

    internal void InvokeOnDeserializing(object obj, StreamingContext context) =>
        InvokeOnDelegate(obj, context, _onDeserializingMethods);

    internal void InvokeOnDeserialized(object obj, StreamingContext context) =>
        InvokeOnDelegate(obj, context, _onDeserializedMethods);

    internal Action<StreamingContext>? AddOnDeserialized(object obj, Action<StreamingContext>? handler) =>
        AddOnDelegate(obj, handler, _onDeserializedMethods);

    /// <summary>Invoke all methods.</summary>
    private static void InvokeOnDelegate(object obj, StreamingContext context, List<MethodInfo>? methods)
    {
        Debug.Assert(obj is not null, "object should have been initialized");
        AddOnDelegate(obj, null, methods)?.Invoke(context);
    }

    /// <summary>Add all methods to a delegate.</summary>
    private static Action<StreamingContext>? AddOnDelegate(object obj, Action<StreamingContext>? handler, List<MethodInfo>? methods)
    {
        if (methods is not null)
        {
            foreach (MethodInfo method in methods)
            {
                Action<StreamingContext> onDeserialized = method.CreateDelegate<Action<StreamingContext>>(obj);
                handler += onDeserialized;
            }
        }

        return handler;
    }
}
