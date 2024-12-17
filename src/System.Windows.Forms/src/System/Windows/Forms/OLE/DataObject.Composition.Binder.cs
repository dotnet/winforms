// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Private.Windows.Core.BinaryFormat;
using System.Reflection.Metadata;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Switches = System.Windows.Forms.Primitives.LocalAppContextSwitches;

namespace System.Windows.Forms;

public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        /// <summary>
        ///  A type resolver for use in the <see cref="NativeToWinFormsAdapter"/> when processing binary formatted stream
        ///  contained in our <see cref="DataObject"/> class using the typed consumption side APIs, such as
        ///  <see cref="TryGetData{T}(out T)"/>. This class recognizes primitive types, exchange types from
        ///  System.Drawing.Primitives, <see cref="List{T}"/>s or arrays of primitive types, and common WinForms types.
        ///  The user can provide a custom resolver for additional types. If the resolver function is not provided,
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

            // These types are read from and written to serialized stream manually, accessing record field by field.
            // Thus they are re-hydrated with no formatters and are safe. The default resolver should recognize them
            // to resolve primitive types or fields of the specified type T.
            private static readonly Type[] s_intrinsicTypes =
            [
                // Primitive types.
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                typeof(double),
                typeof(float),
                typeof(char),
                typeof(bool),
                typeof(string),
                typeof(decimal),
                typeof(DateTime),
                typeof(TimeSpan),
                typeof(IntPtr),
                typeof(UIntPtr),
                // Special type we use to report that binary formatting is disabled.
                typeof(NotSupportedException),
                // Lists of primitive types
                typeof(List<byte>),
                typeof(List<sbyte>),
                typeof(List<short>),
                typeof(List<ushort>),
                typeof(List<int>),
                typeof(List<uint>),
                typeof(List<long>),
                typeof(List<ulong>),
                typeof(List<float>),
                typeof(List<double>),
                typeof(List<char>),
                typeof(List<bool>),
                typeof(List<string>),
                typeof(List<decimal>),
                typeof(List<DateTime>),
                typeof(List<TimeSpan>),
                // Arrays of primitive types.
                typeof(byte[]),
                typeof(sbyte[]),
                typeof(short[]),
                typeof(ushort[]),
                typeof(int[]),
                typeof(uint[]),
                typeof(long[]),
                typeof(ulong[]),
                typeof(float[]),
                typeof(double[]),
                typeof(char[]),
                typeof(bool[]),
                typeof(string[]),
                typeof(decimal[]),
                typeof(DateTime[]),
                typeof(TimeSpan[]),
                // Common WinForms types.
                typeof(ImageListStreamer),
                typeof(Drawing.Bitmap),
                // Exchange types, they are serialized with the .NET Framework assembly name.
                // In .NET they are located in System.Drawing.Primitives.
                typeof(Drawing.RectangleF),
                typeof(PointF),
                typeof(Drawing.SizeF),
                typeof(Drawing.Rectangle),
                typeof(Point),
                typeof(Drawing.Size),
                typeof(Color)
            ];

            private static Dictionary<TypeName, Type>? s_knownTypes;

            private readonly Dictionary<TypeName, Type> _userTypes = new(TypeNameComparer.Default);

            /// <summary>
            ///  Type resolver for use with <see cref="BinaryFormatter"/> and NRBF deserializers to restrict types
            ///  that can be instantiated.
            /// </summary>
            /// <param name="type"><see cref="Type"/> that the user expects to read from the binary formatted stream.</param>
            /// <param name="resolver">
            ///  Provides the list of custom allowed types that user considers safe to deserialize from the payload.
            ///  Resolver should recognize the closure of all non-primitive and not known types in the payload,
            ///  such as field types and types in the inheritance hierarchy and the code to match these types to the
            ///  <see cref="TypeName"/>s read from the deserialized stream.
            /// </param>
            /// <param name="legacyMode">
            ///  <see langword="true"/> if the user had not requested any specific type, i.e. the call originates from
            ///  <see cref="GetData(string)"/> API family, that returns an <see cref="object"/>. <see langword="false"/>
            ///  if the user had requested a specific type by calling <see cref="TryGetData{T}(out T)"/> API family.
            /// </param>
            public Binder(Type type, Func<TypeName, Type>? resolver, bool legacyMode)
            {
                Debug.Assert(!legacyMode || (legacyMode && resolver is null), "GetData methods should not provide a resolver.");
                _resolver = resolver;
                _legacyMode = legacyMode;

                if (resolver is null)
                {
                    // Resolver was not provided by the user, we will match the T using our default method:
                    // 1. If the type is a Value type and nullable, unwrap it
                    // 2. Check if the type had been forwarded from another assembly
                    // 3. Match assembly name with no version
                    // 4. Match namespace and type name
                    // Provide a custom resolver function to supports different type matching logic.

                    TypeName typeName = type.ToTypeName();

                    _userTypes.Add(typeName, type);
                }
            }

            [MemberNotNull(nameof(s_knownTypes))]
            private static void InitializeCommonTypes()
            {
                if (s_knownTypes is not null)
                {
                    return;
                }

                s_knownTypes = new(TypeNameComparer.Default);

                foreach (Type type in s_intrinsicTypes)
                {
                    s_knownTypes.Add(type.ToTypeName(), type);
                }
            }

            public static bool IsKnownType<T>() =>
                typeof(T) == typeof(byte)
                    || typeof(T) == typeof(sbyte)
                    || typeof(T) == typeof(short)
                    || typeof(T) == typeof(ushort)
                    || typeof(T) == typeof(int)
                    || typeof(T) == typeof(uint)
                    || typeof(T) == typeof(long)
                    || typeof(T) == typeof(ulong)
                    || typeof(T) == typeof(double)
                    || typeof(T) == typeof(float)
                    || typeof(T) == typeof(char)
                    || typeof(T) == typeof(bool)
                    || typeof(T) == typeof(string)
                    || typeof(T) == typeof(decimal)
                    || typeof(T) == typeof(DateTime)
                    || typeof(T) == typeof(TimeSpan)
                    || typeof(T) == typeof(IntPtr)
                    || typeof(T) == typeof(UIntPtr)
                    || typeof(T) == typeof(NotSupportedException)
                    || typeof(T) == typeof(List<byte>)
                    || typeof(T) == typeof(List<sbyte>)
                    || typeof(T) == typeof(List<short>)
                    || typeof(T) == typeof(List<ushort>)
                    || typeof(T) == typeof(List<int>)
                    || typeof(T) == typeof(List<uint>)
                    || typeof(T) == typeof(List<long>)
                    || typeof(T) == typeof(List<ulong>)
                    || typeof(T) == typeof(List<float>)
                    || typeof(T) == typeof(List<double>)
                    || typeof(T) == typeof(List<char>)
                    || typeof(T) == typeof(List<bool>)
                    || typeof(T) == typeof(List<string>)
                    || typeof(T) == typeof(List<decimal>)
                    || typeof(T) == typeof(List<DateTime>)
                    || typeof(T) == typeof(List<TimeSpan>)
                    || typeof(T) == typeof(byte[])
                    || typeof(T) == typeof(sbyte[])
                    || typeof(T) == typeof(short[])
                    || typeof(T) == typeof(ushort[])
                    || typeof(T) == typeof(int[])
                    || typeof(T) == typeof(uint[])
                    || typeof(T) == typeof(long[])
                    || typeof(T) == typeof(ulong[])
                    || typeof(T) == typeof(float[])
                    || typeof(T) == typeof(double[])
                    || typeof(T) == typeof(char[])
                    || typeof(T) == typeof(bool[])
                    || typeof(T) == typeof(string[])
                    || typeof(T) == typeof(decimal[])
                    || typeof(T) == typeof(DateTime[])
                    || typeof(T) == typeof(TimeSpan[])
                    || typeof(T) == typeof(ImageListStreamer)
                    || typeof(T) == typeof(Drawing.Bitmap)
                    || typeof(T) == typeof(Drawing.RectangleF)
                    || typeof(T) == typeof(PointF)
                    || typeof(T) == typeof(Drawing.SizeF)
                    || typeof(T) == typeof(Drawing.Rectangle)
                    || typeof(T) == typeof(Point)
                    || typeof(T) == typeof(Drawing.Size)
                    || typeof(T) == typeof(Color);

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
                    return Switches.ClipboardDragDropEnableUnsafeBinaryFormatterSerialization
                        ? null
                        : throw new NotSupportedException(string.Format(
                            SR.BinaryFormatter_NotSupported_InClipboardOrDragDrop_UseTypedAPI,
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
                    throw new NotSupportedException(string.Format(
                        SR.ClipboardOrDragDrop_UseTypedAPI,
                        typeName.AssemblyQualifiedName));
                }

                Type resolved = _resolver(typeName)
                    ?? throw new NotSupportedException(string.Format(
                        SR.ClipboardOrDragDrop_TypedAPI_InvalidResolver,
                        typeName.AssemblyQualifiedName));

                _userTypes.Add(typeName, resolved);
                return resolved;
            }

            private Type? GetCachedType(string assemblyName, string fullTypeName, TypeName? typeName)
            {
                InitializeCommonTypes();

                typeName ??= TypeName.Parse($"{fullTypeName}, {assemblyName}");

                return s_knownTypes.TryGetValue(typeName, out Type? type)
                    ? type
                    : _userTypes.TryGetValue(typeName, out type) ? type : null;
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
