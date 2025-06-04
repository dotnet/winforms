// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.BinaryFormat;
using System.Private.Windows.Nrbf;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.Ole;

/// <summary>
///  A type resolver for use when processing binary formatted streams.
/// </summary>
/// <remarks>
///  <para>
///   If the resolver function is not provided, a resolver is created that handles the specified root <see cref="Type"/>.
///  </para>
///  <para>
///   If the resolver returns <see langword="null"/>, the <typeparamref name="TNrbfSerializer"/> is used to attempt to
///   bind the type. If the type is not fully supported, the type is not bound. If users want to override this behavior,
///   they should throw in their resolver.
///  </para>
///  <para>
///   Unbound types will throw a <see cref="NotSupportedException"/> when attempting to bind.
///  </para>
///  <para>
///   This class is used in <see cref="BinaryFormatter"/> and NRBF deserialization.
///  </para>
/// </remarks>
internal sealed class TypeBinder<TNrbfSerializer> : SerializationBinder, ITypeResolver
    where TNrbfSerializer : INrbfSerializer
{
    private readonly Type _rootType;
    private readonly Func<TypeName, Type?>? _resolver;
    private readonly bool _isTypedRequest;

    private Dictionary<FullyQualifiedTypeName, TypeName>? _cachedTypeNames;

    /// <summary>
    ///  Type resolver for use with <see cref="BinaryFormatter"/> and NRBF deserializers to restrict types
    ///  that can be instantiated.
    /// </summary>
    /// <param name="rootType"><see cref="Type"/> that the user expects to read from the binary formatted stream.</param>
    public TypeBinder(Type rootType, ref readonly DataRequest request)
    {
        Debug.Assert(request.TypedRequest || request.Resolver is null, "Untyped methods should not provide a resolver.");
        Debug.Assert(request.TypedRequest || rootType == typeof(object), "Untyped requests should always be asking for object");

        _isTypedRequest = request.TypedRequest;
        _rootType = rootType;
        _resolver = request.Resolver;
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
        ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

        if (!_isTypedRequest)
        {
            Debug.Assert(CoreAppContextSwitches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization);
            Debug.Assert(FeatureSwitches.EnableUnsafeBinaryFormatterSerialization);

            // With the switch enabled, we'll let the BinaryFormatter allow any type to be deserialized
            // when coming in from the untyped APIs.
            return null;
        }

        // Should never fall through to the BinaryFormatter from a typed API without an explicit resolver.
        if (_resolver is null)
        {
            throw new InvalidOperationException();
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
            _resolver is null ? SR.ClipboardOrDragDrop_UseTypedAPI : SR.ClipboardOrDragDrop_TypedAPI_InvalidResolver,
            typeName.AssemblyQualifiedName));

    public bool TryBindToType(TypeName typeName, [NotNullWhen(true)] out Type? type)
    {
        ArgumentNullException.ThrowIfNull(typeName);

        if (!_isTypedRequest)
        {
            type = typeof(object);
            return true;
        }

        if (_resolver is null)
        {
            if (_rootType.Matches(typeName, TypeNameComparison.AllButAssemblyVersion))
            {
                type = _rootType;
                return true;
            }

            // If we fall back here to the TNrbfSerializer all types would match for the root. This has the side
            // effect of asking for `int?` and matching `int` data. We need to do `IsAssignableTo` for resolved
            // types so we can allow resolvers to bind to derived classes- `int` is assignable to `int?`.
            //
            // Asking for `List<int?>` will not match `List<int>` data, even though the CoreNrbfSerializer supports
            // `List<int>`. `List<int>` is not assignable to `List<int?>`.
            type = null;
            return false;
        }

        try
        {
            type = _resolver(typeName);
        }
        catch (Exception ex) when (!ex.IsCriticalException())
        {
            type = null;
            return false;
        }

        // Explicit resolver returned null, fall back to implicit support.
        if (type is null
            && _rootType != typeof(object)
            && !_rootType.IsInterface
            && TNrbfSerializer.TryBindToType(typeName, out type)
            && !TNrbfSerializer.IsFullySupportedType(type))
        {
            // Don't allow automatic binding for open-ended root types. This is to prevent surprising behavior
            // with "primitive" types such as `int` binding to `IComparable`, etc. It also prevents leaking out
            // JSON DOM with our IJsonData types.
            //
            // Don't allow automatic binding for types that aren't fully supported.
            type = null;
        }

        return type is not null;
    }
}
