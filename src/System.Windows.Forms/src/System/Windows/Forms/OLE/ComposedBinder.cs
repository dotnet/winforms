// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Windows.Forms.BinaryFormat;

namespace System.Windows.Forms;

internal sealed class ComposedBinder : SerializationBinder
{
    private readonly Func<TypeName, Type> _resolver;
    private bool _initialized;
    private readonly Type _type;
    private readonly bool _legacyMode;
    private readonly Dictionary<string, Type> _mscorlibTypeCache = new()
    {
        { "System.Byte", typeof(byte) },
        { "System.SByte", typeof(sbyte) },
        { "System.Int16", typeof(short) },
        { "System.UInt16", typeof(ushort) },
        { "System.Int32", typeof(int) },
        { "System.UInt32", typeof(uint) },
        { "System.Int64", typeof(long) },
        { "System.UInt64", typeof(ulong) },
        { "System.Double", typeof(double) },
        { "System.Single", typeof(float) },
        { "System.Char", typeof(char) },
        { "System.Boolean", typeof(bool) },
        { "System.String", typeof(string) },
        { "System.Decimal", typeof(decimal) },
        { "System.DateTime", typeof(DateTime) },
        { "System.TimeSpan", typeof(TimeSpan) },
        { "System.IntPtr", typeof(IntPtr) },
        { "System.UIntPtr", typeof(UIntPtr) },
        { "System.Array", typeof(Array) },
        { "System.Collections.ArrayList", typeof(ArrayList) },
        { "System.Collections.Hashtable", typeof(Hashtable) },
        { TypeInfo.NotSupportedExceptionType, typeof(NotSupportedException) },
        { "System.Collections.Generic.List`1[[System.Byte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<byte>) },
        { "System.Collections.Generic.List`1[[System.SByte, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<sbyte>) },
        { "System.Collections.Generic.List`1[[System.Int16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<short>) },
        { "System.Collections.Generic.List`1[[System.UInt16, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<ushort>) },
        { "System.Collections.Generic.List`1[[System.Int32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<int>) },
        { "System.Collections.Generic.List`1[[System.UInt32, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<uint>) },
        { "System.Collections.Generic.List`1[[System.Int64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<long>) },
        { "System.Collections.Generic.List`1[[System.UInt64, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<ulong>) },
        { "System.Collections.Generic.List`1[[System.Single, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<float>) },
        { "System.Collections.Generic.List`1[[System.Double, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<double>) },
        { "System.Collections.Generic.List`1[[System.Char, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<char>) },
        { "System.Collections.Generic.List`1[[System.Boolean, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<bool>) },
        { "System.Collections.Generic.List`1[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<string>) },
        { "System.Collections.Generic.List`1[[System.Decimal, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<decimal>) },
        { "System.Collections.Generic.List`1[[System.DateTime, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<DateTime>) },
        { "System.Collections.Generic.List`1[[System.TimeSpan, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<TimeSpan>) }
    };

    private readonly Dictionary<(string, string), Type> _knownTypes = new()
    {
        { ("System.Windows.Forms.ImageListStreamer", "System.Windows.Forms"), typeof(ImageListStreamer)},
        { ("System.Drawing.Bitmap", "System.Drawing"), typeof(Drawing.Bitmap)},
         // The following are exchange types, they are serialized with the .NET Framework assembly name. In .NET they are located in System.Drawing.Primitives.
        { ("System.Drawing.RectangleF", "System.Drawing"), typeof(Drawing.RectangleF)},
        { ("System.Drawing.PointF", "System.Drawing"), typeof(Drawing.PointF)},
        { ("System.Drawing.SizeF", "System.Drawing"), typeof(Drawing.SizeF)},
        { ("System.Drawing.Rectangle", "System.Drawing"), typeof(Drawing.Rectangle)},
        { ("System.Drawing.Point", "System.Drawing"), typeof(Drawing.Point)},
        { ("System.Drawing.Size", "System.Drawing"), typeof(Drawing.Size)},
        { ("System.Drawing.Color", "System.Drawing"), typeof(Drawing.Color)}
    };

    public ComposedBinder(Type type, Func<TypeName, Type> resolver, bool legacyMode)
    {
        _resolver = resolver.OrThrowIfNull();
        _type = type.OrThrowIfNull();

        _legacyMode = legacyMode;
    }

    public override Type? BindToType(string assemblyName, string typeName)
    {
        if (string.IsNullOrWhiteSpace(assemblyName))
        {
            throw new ArgumentException(nameof(assemblyName));
        }

        if (string.IsNullOrWhiteSpace(typeName))
        {
            throw new ArgumentException(nameof(typeName));
        }

        // We assume all built-in types are normalized to mscorlib assembly, as BF and BFO are doing so for compat with FX.
        if (assemblyName.Equals(TypeInfo.MscorlibAssemblyName, StringComparison.Ordinal)
            && _mscorlibTypeCache.TryGetValue(typeName, out Type? builtIn))
        {
            return builtIn;
        }

        // Ignore version, culture, and public key token and compare the short names.
        string shortAssemblyName = assemblyName.Split(',')[0].Trim();
        if (_knownTypes.TryGetValue((typeName, shortAssemblyName), out Type? knownType))
        {
            return knownType;
        }

        if (!_initialized)
        {
            AddToKnownTypes(_type);
            Type unwrapped = Formatter.NullableUnwrap(_type);
            if (_type != unwrapped)
            {
                AddToKnownTypes(unwrapped);
            }

            _initialized = true;
            if (_knownTypes.TryGetValue((typeName, shortAssemblyName), out knownType))
            {
                return knownType;
            }
        }

        Type type = _resolver(TypeName.Parse($"{typeName}, {assemblyName}"));
        if (!_legacyMode && type is null)
        {
            throw new NotSupportedException($"'resolver' function provided in '{nameof(Clipboard.TryGetData)}'" +
                $" method should never return a null.  It should throw a '{nameof(NotSupportedException)}' when encountering unsupported types.");
        }

        return type;
    }

    private void AddToKnownTypes(Type type)
    {
        if (!type.TryGetForwardedFromName(out string? name))
        {
            name = type.Assembly.FullName!.Split(',')[0].Trim();
        }

        // Ignore version, culture, and public key token and compare the short names.
        _knownTypes[(type.FullName!, name)] = type;
    }
}
