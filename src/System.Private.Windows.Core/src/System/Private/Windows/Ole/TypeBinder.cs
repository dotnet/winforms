﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.BinaryFormat;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Ole;

/// <summary>
///  A type resolver for use in the <see cref="Composition{TRuntime, TDataFormat}.NativeToManagedAdapter"/>
///  when processing binary formatted streams.
/// </summary>
/// <remarks>
///  <para>
///   This class recognizes primitive types, exchange types from System.Drawing.Primitives, <see cref="List{T}"/>s
///   or arrays of primitive types, and common WinForms types. The user can provide a custom resolver for additional
///   types. If the resolver function is not provided, the <see cref="Type"/> parameter specified by the user is
///   resolved automatically.
///  </para>
///  <para>
///   This class is used in <see cref="BinaryFormatter"/> and NRBF deserialization.
///  </para>
/// </remarks>
internal sealed class TypeBinder<TNrbfSerializer> : SerializationBinder, ITypeResolver
    where TNrbfSerializer : INrbfSerializer
{
    private readonly Type _rootType;
    private Func<TypeName, Type?>? _resolver;
    private readonly bool _isUntypedRequest;
    private readonly bool _hasCustomResolver;

    private Dictionary<FullyQualifiedTypeName, TypeName>? _cachedTypeNames;

    /// <summary>
    ///  Type resolver for use with <see cref="BinaryFormatter"/> and NRBF deserializers to restrict types
    ///  that can be instantiated.
    /// </summary>
    /// <param name="rootType"><see cref="Type"/> that the user expects to read from the binary formatted stream.</param>
    public TypeBinder(Type rootType, ref readonly DataRequest request)
    {
        Debug.Assert(
            !request.UntypedRequest || (request.UntypedRequest && request.Resolver is null),
            "Untyped methods should not provide a resolver.");

        _isUntypedRequest = request.UntypedRequest;
        _hasCustomResolver = request.Resolver is not null;
        _rootType = rootType;
        _resolver = request.Resolver;
    }

    private Func<TypeName, Type?> Resolver
    {
        get
        {
            // Lazily create a resolver when needed to potentially avoid the cost of parsing the TypeName.
            return _resolver ??= CreateResolver(_rootType);

            static Func<TypeName, Type?> CreateResolver(Type type)
            {
                // Resolver was not provided by the user, we will match the T using our default method:
                //
                //  1. If the type is a Value type and nullable, unwrap it
                //  2. Check if the type had been forwarded from another assembly
                //  3. Match assembly name with no version
                //  4. Match namespace and type name
                //
                // Provide a custom resolver function to supports different type matching logic.

                TypeName knownType = type.ToTypeName();
                return typeName => knownType.Matches(typeName) ? type : null;
            }
        }
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

        if (_isUntypedRequest)
        {
            Debug.Assert(CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization);
            Debug.Assert(FeatureSwitches.EnableUnsafeBinaryFormatterSerialization);

            // With the switch enabled, we'll let the BinaryFormatter allow any type to be deserialized
            // when coming in from the untyped APIs.
            return null;
        }

        FullyQualifiedTypeName fullName = new(typeName, assemblyName);

        _cachedTypeNames ??= [];
        if (!_cachedTypeNames.TryGetValue(fullName, out TypeName? parsed))
        {
            parsed = TypeName.Parse(fullName.ToString());
            _cachedTypeNames.Add(fullName, parsed);
        }

        return BindToType(parsed);
    }

    public Type BindToType(TypeName typeName) => TryBindToType(typeName, out Type? type)
        ? type
        : throw new NotSupportedException(string.Format(
            _hasCustomResolver ? SR.ClipboardOrDragDrop_TypedAPI_InvalidResolver : SR.ClipboardOrDragDrop_UseTypedAPI,
            typeName.AssemblyQualifiedName));

    public bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
    {
        ArgumentNullException.ThrowIfNull(typeName);

        if (!TNrbfSerializer.TryBindToType(typeName, out type))
        {
            type = Resolver(typeName);
        }

        return type is not null;
    }
}
