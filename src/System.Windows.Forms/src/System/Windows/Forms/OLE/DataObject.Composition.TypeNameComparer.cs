// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection.Metadata;

namespace System.Windows.Forms;
public unsafe partial class DataObject
{
    internal unsafe partial class Composition
    {
        /// <summary>
        ///  Match <see cref="TypeName"/>s by matching full namespace-qualified type names and full assembly names,
        ///  including the version.
        /// </summary>
        internal sealed class TypeNameComparer : IEqualityComparer<TypeName>
        {
            private TypeNameComparer()
            {
            }

            internal static IEqualityComparer<TypeName> Default { get; } = new TypeNameComparer();

            public bool Equals(TypeName? x, TypeName? y)
            {
                if (x is null && y is null)
                {
                    return true;
                }

                if (x is null || y is null)
                {
                    return false;
                }

                return x.Matches(y);
            }

            public int GetHashCode(TypeName obj)
            {
                if (obj is null)
                {
                    return 0;
                }

                if (obj.IsArray)
                {
                    return true.GetHashCode() ^ obj.GetArrayRank() ^ GetHashCode(obj.GetElementType());
                }

                int hashCode;
                if (obj.IsConstructedGenericType)
                {
                    hashCode = "constructed".GetHashCode() ^ GetHashCode(obj.GetGenericTypeDefinition());
                    foreach (TypeName genericName in obj.GetGenericArguments())
                    {
                        hashCode ^= GetHashCode(genericName);
                    }

                    return hashCode;
                }

                hashCode = obj.FullName.GetHashCode();
                if (obj.AssemblyName is AssemblyNameInfo info)
                {
                    hashCode ^= info.Name.GetHashCode();
                    if (info.Version is not null)
                    {
                        hashCode ^= info.Version.GetHashCode();
                    }

                    if (info.CultureName is not null)
                    {
                        hashCode ^= info.CultureName.GetHashCode();
                    }

                    if (!info.PublicKeyOrToken.IsDefaultOrEmpty)
                    {
                        foreach (byte b in info.PublicKeyOrToken)
                        {
                            hashCode ^= b.GetHashCode();
                        }
                    }
                }

                return hashCode;
            }
        }
    }
}
