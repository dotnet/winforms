// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using Switches = System.Windows.Forms.Primitives.LocalAppContextSwitches;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        internal sealed class Binder : SerializationBinder
        {
            private readonly Func<TypeName, Type>? _resolver;
            private readonly Type _type;
            private readonly bool _legacyMode;
            // this is needed to handle fields of the desired type T unless we can feed them through GetCommonObject<T>
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
                { "System.Collections.Generic.List`1[[System.TimeSpan, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(List<TimeSpan>) },
                { "System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(byte[]) },
                { "System.SByte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(sbyte[]) },
                { "System.Int16[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(short[]) },
                { "System.UInt16[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(ushort[]) },
                { "System.Int32[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(int[]) },
                { "System.UInt32[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(uint[]) },
                { "System.Int64[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(long[]) },
                { "System.UInt64[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(ulong[]) },
                { "System.Single[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(float[]) },
                { "System.Double[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(double[]) },
                { "System.Char[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(char[]) },
                { "System.Boolean[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(bool[]) },
                { "System.String[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(string[]) },
                { "System.Decimal[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(decimal[]) },
                { "System.DateTime[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(DateTime[]) },
                { "System.TimeSpan[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]]", typeof(TimeSpan[]) }
            };

            private readonly Dictionary<(string, string), Type> _commonTypes = new()
            {
                { ("System.Windows.Forms.ImageListStreamer", "System.Windows.Forms"), typeof(ImageListStreamer) },
                { ("System.Drawing.Bitmap", "System.Drawing"), typeof(Drawing.Bitmap) },
                // The following are exchange types, they are serialized with the .NET Framework assembly name.
                // In .NET they are located in System.Drawing.Primitives.
                { ("System.Drawing.RectangleF", "System.Drawing"), typeof(Drawing.RectangleF) },
                { ("System.Drawing.PointF", "System.Drawing"), typeof(Drawing.PointF) },
                { ("System.Drawing.SizeF", "System.Drawing"), typeof(Drawing.SizeF) },
                { ("System.Drawing.Rectangle", "System.Drawing"), typeof(Drawing.Rectangle) },
                { ("System.Drawing.Point", "System.Drawing"), typeof(Drawing.Point) },
                { ("System.Drawing.Size", "System.Drawing"), typeof(Drawing.Size) },
                { ("System.Drawing.Color", "System.Drawing"), typeof(Drawing.Color) }
            };

            public Binder(Type type, Func<TypeName, Type>? resolver, bool legacyMode)
            {
                _resolver = resolver;
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

                // We assume all built-in types are normalized to the mscorlib assembly, as BinaryFormatter and
                // BinaryFormattedObject are doing so for compatibility with .NET Framework.
                if (assemblyName.Equals(TypeInfo.MscorlibAssemblyName, StringComparison.Ordinal)
                    && _mscorlibTypeCache.TryGetValue(typeName, out Type? builtIn))
                {
                    return builtIn;
                }

                // Ignore version, culture, and public key token and compare the short names.
                string shortAssemblyName = assemblyName.Split(',')[0].Trim();
                if (_commonTypes.TryGetValue((typeName, shortAssemblyName), out Type? knownType))
                {
                    return knownType;
                }

                var parsed = TypeName.Parse($"{typeName}, {assemblyName}");
                if (Matches(_type, parsed))
                {
                    _commonTypes.Add((typeName, shortAssemblyName), _type);
                    return _type;
                }

                if (_legacyMode)
                {
                    return Switches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization
                        ? null
                        : throw new NotSupportedException($"Using BinaryFormatter is not supported in WinForms Clipboard" +
                            $" or drag and drop scenarios.");
                }

                if (_resolver is null)
                {
                    throw new NotSupportedException($"'resolver' function is required in '{nameof(Clipboard.TryGetData)}'" +
                        $" method to resolve '{typeName}' from '{assemblyName}'");
                }

                Type type = _resolver(parsed)
                    ?? throw new NotSupportedException($"'resolver' function provided in '{nameof(Clipboard.TryGetData)}'" +
                        $" method should never return a null. It should throw a '{nameof(NotSupportedException)}' when encountering unsupported types.");

                _commonTypes.Add((typeName, shortAssemblyName), type);
                return type;
            }

            // Copied from https://github.com/dotnet/runtime/blob/79a71fc750652191eba18e19b3f98492e882cb5f/src/libraries/System.Formats.Nrbf/src/System/Formats/Nrbf/SerializationRecord.cs#L68
            private static bool Matches(Type type, TypeName typeName)
            {
                // We don't need to check for pointers and references to arrays,
                // as it's impossible to serialize them with BF.
                if (type.IsPointer || type.IsByRef)
                {
                    return false;
                }

                if (type.IsArray != typeName.IsArray
                    || type.IsConstructedGenericType != typeName.IsConstructedGenericType
                    || type.IsNested != typeName.IsNested
                    || (type.IsArray && type.GetArrayRank() != typeName.GetArrayRank())
                    || type.IsSZArray != typeName.IsSZArray // int[] vs int[*]
                    )
                {
                    return false;
                }

                if (type.FullName == typeName.FullName)
                {
                    return true; // The happy path with no type forwarding
                }
                else if (typeName.IsArray)
                {
                    return Matches(type.GetElementType()!, typeName.GetElementType());
                }
                else if (type.IsConstructedGenericType)
                {
                    if (!Matches(type.GetGenericTypeDefinition(), typeName.GetGenericTypeDefinition()))
                    {
                        return false;
                    }

                    ImmutableArray<TypeName> genericNames = typeName.GetGenericArguments();
                    Type[] genericTypes = type.GetGenericArguments();

                    if (genericNames.Length != genericTypes.Length)
                    {
                        return false;
                    }

                    for (int i = 0; i < genericTypes.Length; i++)
                    {
                        if (!Matches(genericTypes[i], genericNames[i]))
                        {
                            return false;
                        }
                    }

                    return true;
                }

                return false;
            }
        }
    }
}
