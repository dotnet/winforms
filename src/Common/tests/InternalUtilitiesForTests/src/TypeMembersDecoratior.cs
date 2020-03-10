// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit.Abstractions;

namespace System
{
    public static class TypeMembersDecoratior
    {
        public static Dictionary<string, string> EnumerateAttributes(Assembly assemblyUnderTest)
        {
            var dic = new Dictionary<string, string>();

            foreach (Type type in assemblyUnderTest.GetTypes())
            {
                var typeName = FormatType(type);

                // get all type's attributes
                var attributes = Attribute.GetCustomAttributes(type);
                if (attributes.Length > 0)
                {
                    dic.Add(typeName, string.Join(", ", attributes.Select(a => a.GetType().FullName).OrderBy(a => a)));
                }

                // get all type's members and their attributes
                BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;
                foreach (MemberInfo mi in type.GetMembers(flags).OrderBy(m => m.Name))
                {
                    // exclude internal reflection plumbing and exclude properties as they get represented as respective get_*/set_* methods
                    if (mi.GetType().Name == "RtFieldInfo" || mi is PropertyInfo)
                    {
                        continue;
                    }

                    attributes = Attribute.GetCustomAttributes(mi);
                    if (attributes.Length > 0)
                    {
                        string memberSignature = FormatMethod(mi as ConstructorInfo) ?? FormatMethod(mi as MethodInfo) ?? mi.Name;
                        dic.Add($"{typeName}.{memberSignature}", string.Join(", ", attributes.Select(a => a.GetType().FullName).OrderBy(a => a)));
                    }
                }
            }

            return dic;
        }
        private static string FormatMethod(ConstructorInfo mi)
        {
            if (mi == null)
            {
                return null;
            }

            StringBuilder signature = new StringBuilder(mi.Name);
            signature.Append("(");
            signature.Append(string.Join(", ", mi.GetParameters().Select(p => FormatType(p.ParameterType))));
            signature.Append(")");
            return signature.ToString();
        }

        private static string FormatMethod(MethodInfo mi)
        {
            if (mi == null)
            {
                return null;
            }

            StringBuilder signature = new StringBuilder(mi.Name);

            if (mi.IsGenericMethod)
            {
                signature.Append($"<{string.Join(", ", mi.GetGenericArguments().Select(FormatType))}>");
            }
            signature.Append("(");
            signature.Append(string.Join(", ", mi.GetParameters().Select(p => FormatType(p.ParameterType))));
            signature.Append(")");
            return signature.ToString();
        }

        private static string FormatType(Type type)
        {
            if (type == null)
            {
                return "";
            }

            StringBuilder signature = new StringBuilder();

            if (type.IsGenericType)
            {
                signature.Append(type.Name);
                signature.Append($"<{string.Join(", ", type.GetGenericArguments().Select(FormatType))}>");
            }
            else
            {
                signature.Append(type.FullName ?? type.Name);
            }

            return signature.ToString();
        }
    }
}
