// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;
using System.Runtime.Serialization;

namespace System.Windows.Forms;

internal sealed class ComposedBinder : SerializationBinder
{
    private readonly Func<TypeName, Type> _resolver;
    private readonly string _typeName;
    private readonly string _assemblyName;
    private readonly Type _type;

    public ComposedBinder(Type type, Func<TypeName, Type> resolver)
    {
        _resolver = resolver.OrThrowIfNull();
        _type = type.OrThrowIfNull();
        TypeName parsed = TypeName.Parse($"{type.FullName}, {type.Assembly.FullName}");

        _typeName = parsed.FullName;
        // Ignore version, culture, and public key token and compare the short names.
        _assemblyName = parsed.AssemblyName!.Name;
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        TypeName parsed = TypeName.Parse($"{typeName}, {assemblyName}");

        if (parsed.AssemblyName is { } assembly)
        {
            // TypeName.FullName is a namespace-qualified name.
            if (_typeName == parsed.FullName && _assemblyName == assembly.Name)
            {
                return _type;
            }
        }

        // TanyaSo: Should we throw ArgumentException?
        return _resolver(parsed)
            ?? throw new NotSupportedException($"resolver function provided in {nameof(Clipboard.TryGetData)} method should never return a null.");
    }
}
