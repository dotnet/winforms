// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Switches = System.Windows.Forms.Primitives.LocalAppContextSwitches;
using TypeInfo = System.Private.Windows.Core.BinaryFormat.TypeInfo;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        /// <summary>
        ///  A type resolver for use in the <see cref="NativeToWinFormsAdapter"/> when processing binary formatted stream
        ///  contained in our <see cref="DataObject"/> class using the typed consumption side APIs, such as
        ///  <see cref="TryGetData{T}(out T)"/>.  This class recognizes primitive types, exchange types from
        ///  System.Drawing.Primitives, <see cref="List{T}"/>s or arrays of primitive types, and common WinForms types.
        ///  The user can provide a custom resolver for additional types.  If the resolver function is not provided,
        ///  the <see cref="Type"/> parameter specified by the user is resolved automatically.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   This class is used in <see cref="BinaryFormatter"/> and NRBF deserialization.
        ///  </para>
        /// </remarks>
        internal sealed class Binder : SerializationBinder, ITypeResolver
        {
            private readonly Func<TypeName, Type>? _resolver;
            private readonly bool _legacyMode;

            // This is needed to resolve primitive fields of the requested type T.
            private static readonly Dictionary<string, Type> s_mscorlibTypeCache = new()
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

            private static readonly Dictionary<(string, string), Type> s_commonTypes = new()
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

            private readonly Dictionary<TypeName, Type> _userTypes = new(TypeNameComparer.Default);

            /// <summary>
            ///  Type resolver for use with <see cref="BinaryFormatter"/> and NRBF deserializers to restrict types
            ///  that can be instantiated.
            /// </summary>
            /// <param name="type"><see cref="Type"/> that the user expects to read from the binary formatted stream.</param>
            /// <param name="resolver">
            ///  Provides the list of custom allowed types that user consideres safe to deserialize from the payload.
            ///  Resolver should recognize the closure of all non-primitive and not known types in the payload,
            ///  such as field types and types in the inheritance hierarchy and the code to match these types to the
            ///  <see cref="TypeName"/>s read from the derialized stream.
            /// </param>
            /// <param name="legacyMode">
            ///  <see langword="true"/> if the user had not requested any specific type, i.e. the call originates from
            ///  <see cref="GetData(string)"/> API family, that returns an <see cref="object"/>.  <see langword="false"/>
            ///  if the user had requested a specific type by calling <see cref="TryGetData{T}(out T)"/> API family.
            /// </param>
            public Binder(Type type, Func<TypeName, Type>? resolver, bool legacyMode)
            {
                Debug.Assert(!legacyMode || (legacyMode && resolver is null), "GetData methods should not provide a resolver.");
                _resolver = resolver;
                _legacyMode = legacyMode;

                if (resolver is null)
                {
                    // resolver was not provided by the user, we will match the T using our default method:
                    // 1. If the type is a Value type and nullable, unwrap it
                    // 2. Check if the type had been forwarded from another assembly
                    // 3. Match assembly name with no version
                    // 4. Match namespace and type name
                    type = Formatter.NullableUnwrap(type.OrThrowIfNull());

                    TypeName typeName = type.TryGetForwardedFromName(out string? name)
                        ? TypeName.Parse($"{type.FullName.OrThrowIfNull()}, {name}")
                        : TypeName.Parse(type.AssemblyQualifiedName.OrThrowIfNull());

                    _userTypes.Add(typeName, type);
                }
            }

            public override Type? BindToType(string assemblyName, string typeName)
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(assemblyName);
                ArgumentException.ThrowIfNullOrWhiteSpace(typeName);

                if (GetCachedType(assemblyName, typeName, typeName: null) is Type type)
                {
                    return type;
                }

                if (_legacyMode)
                {
                    // TODO (TanyaSo): localize string
                    return Switches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization
                        ? null
                        : throw new NotSupportedException(string.Format(
                            "BinaryFormatter is not supported in Clipboard or drag and drop scenarios." +
                                "  Enable it and use 'TryGetData<T>' API with a 'resolver'" +
                                " function that defines a set of allowed types, to deserialize '{0}'.",
                            $"{assemblyName}.{typeName}"));
                }

                TypeName parsed = TypeName.Parse($"{typeName}, {assemblyName}");
                return UseResolver(parsed);
            }

            [RequiresUnreferencedCode("Calls user-provided method that resolves types from names.")]
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            private Type UseResolver(TypeName typeName)
            {
                if (_resolver is null)
                {
                    // TODO (TanyaSo): localize string
                    throw new NotSupportedException(string.Format(
                        "Use 'TryGetData<T>' method with a 'resolver' function that defines a set of allowed types, to deserialize '{0}'.",
                        typeName.AssemblyQualifiedName));
                }

                // This helper method is called after we verified that _resolver is not null.
                Type resolved = _resolver!(typeName)
                    ?? throw new NotSupportedException(string.Format(
                        "'resolver' function provided in 'TryGetData<T>' method should not return a null.  " +
                            "It should throw a 'System.NotSupportedException' when encountering unsupported types, " +
                            "unless type {0} is one of the allowed types, then the 'Type' should be returned.",
                        typeName.AssemblyQualifiedName));

                _userTypes.Add(typeName, resolved);
                return resolved;
            }

            public static Type? GetKnownType(string assemblyName, string fullTypeName, TypeName? typeName)
            {
                // We assume all built-in types are normalized to the mscorlib assembly, as BinaryFormatter
                // is doing it for compatibility with .NET Framework.
                if (assemblyName.Equals(TypeInfo.MscorlibAssemblyName, StringComparison.Ordinal)
                    && s_mscorlibTypeCache.TryGetValue(fullTypeName, out Type? builtIn))
                {
                    return builtIn;
                }

                AssemblyNameInfo assemblyNameInfo = typeName is null
                    ? AssemblyNameInfo.Parse(assemblyName)
                    : typeName.AssemblyName!; // caller validated that TypeName has a not-null AssemblyNameInfo.

                // Ignore version, culture, and public key token and compare the short names.
                string shortAssemblyName = assemblyNameInfo.Name;
                return s_commonTypes.TryGetValue((fullTypeName, shortAssemblyName), out Type? knownType) ? knownType : null;
            }

            private Type? GetCachedType(string assemblyName, string fullTypeName, TypeName? typeName)
            {
                Type? type = GetKnownType(assemblyName, fullTypeName, typeName);
                if (type is not null)
                {
                    return type;
                }

                typeName ??= TypeName.Parse($"{fullTypeName}, {assemblyName}");
                return _userTypes.TryGetValue(typeName, out type) ? type : null;
            }

            [RequiresUnreferencedCode("Calls System.Reflection.Assembly.GetType(String)")]
            [return: DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
            public Type GetType(TypeName typeName)
            {
                typeName.OrThrowIfNull();

                if (typeName.AssemblyName is not AssemblyNameInfo info
                    || info.FullName is not string fullName)
                {
                    throw new ArgumentException(message: null, nameof(typeName));
                }

                return GetCachedType(fullName, typeName.FullName, typeName) is Type type ? type : UseResolver(typeName);
            }
        }
    }
}
