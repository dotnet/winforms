// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Reflection;
using System.Text;

namespace System.Diagnostics;

internal sealed class DebuggerAttributeInfo
{
    public object Instance { get; set; }
    public IEnumerable<PropertyInfo> Properties { get; set; }
}

internal static class DebuggerAttributes
{
    internal static object GetFieldValue(object obj, string fieldName)
    {
        return GetField(obj, fieldName).GetValue(obj);
    }

    internal static void InvokeDebuggerTypeProxyProperties(object obj)
    {
        DebuggerAttributeInfo info = ValidateDebuggerTypeProxyProperties(obj.GetType(), obj);
        foreach (PropertyInfo pi in info.Properties)
        {
            pi.GetValue(info.Instance, null);
        }
    }

    internal static DebuggerAttributeInfo ValidateDebuggerTypeProxyProperties(object obj)
    {
        return ValidateDebuggerTypeProxyProperties(obj.GetType(), obj);
    }

    internal static DebuggerAttributeInfo ValidateDebuggerTypeProxyProperties(Type type, object obj)
    {
        return ValidateDebuggerTypeProxyProperties(type, type.GenericTypeArguments, obj);
    }

    internal static DebuggerAttributeInfo ValidateDebuggerTypeProxyProperties(Type type, Type[] genericTypeArguments, object obj)
    {
        Type proxyType = GetProxyType(type, genericTypeArguments);

        // Create an instance of the proxy type, and make sure we can access all of the instance properties
        // on the type without exception
        object proxyInstance = Activator.CreateInstance(proxyType, obj);
        IEnumerable<PropertyInfo> properties = GetDebuggerVisibleProperties(proxyType);
        return new DebuggerAttributeInfo
        {
            Instance = proxyInstance,
            Properties = properties
        };
    }

    public static DebuggerBrowsableState? GetDebuggerBrowsableState(MemberInfo info)
    {
        CustomAttributeData debuggerBrowsableAttribute = info.CustomAttributes
            .SingleOrDefault(a => a.AttributeType == typeof(DebuggerBrowsableAttribute));
        // Enums in attribute constructors are boxed as ints, so cast to int? first.
        return (DebuggerBrowsableState?)(int?)debuggerBrowsableAttribute?.ConstructorArguments.Single().Value;
    }

    public static IEnumerable<FieldInfo> GetDebuggerVisibleFields(Type debuggerAttributeType)
    {
        // The debugger doesn't evaluate non-public members of type proxies.
        IEnumerable<FieldInfo> visibleFields = debuggerAttributeType.GetFields()
            .Where(fi => fi.IsPublic && GetDebuggerBrowsableState(fi) != DebuggerBrowsableState.Never);
        return visibleFields;
    }

    public static IEnumerable<PropertyInfo> GetDebuggerVisibleProperties(Type debuggerAttributeType)
    {
        // The debugger doesn't evaluate non-public members of type proxies. GetGetMethod returns null if the getter is non-public.
        IEnumerable<PropertyInfo> visibleProperties = debuggerAttributeType.GetProperties()
            .Where(pi => pi.GetGetMethod() is not null && GetDebuggerBrowsableState(pi) != DebuggerBrowsableState.Never);
        return visibleProperties;
    }

    public static object GetProxyObject(object obj) => Activator.CreateInstance(GetProxyType(obj), obj);

    public static Type GetProxyType(object obj) => GetProxyType(obj.GetType());

    public static Type GetProxyType(Type type) => GetProxyType(type, type.GenericTypeArguments);

    private static Type GetProxyType(Type type, Type[] genericTypeArguments)
    {
        // Get the DebuggerTypeProxyAttribute for obj
        CustomAttributeData[] attrs =
            type.GetTypeInfo().CustomAttributes
            .Where(a => a.AttributeType == typeof(DebuggerTypeProxyAttribute))
            .ToArray();
        if (attrs.Length != 1)
        {
            throw new InvalidOperationException($"Expected one DebuggerTypeProxyAttribute on {type}.");
        }

        CustomAttributeData cad = attrs[0];

        Type proxyType = cad.ConstructorArguments[0].ArgumentType == typeof(Type) ?
            (Type)cad.ConstructorArguments[0].Value :
            Type.GetType((string)cad.ConstructorArguments[0].Value);
        if (genericTypeArguments.Length > 0)
        {
            proxyType = proxyType.MakeGenericType(genericTypeArguments);
        }

        return proxyType;
    }

    internal static string ValidateDebuggerDisplayReferences(object obj)
    {
        // Get the DebuggerDisplayAttribute for obj
        Type objType = obj.GetType();
        CustomAttributeData[] attrs =
            objType.GetTypeInfo().CustomAttributes
            .Where(a => a.AttributeType == typeof(DebuggerDisplayAttribute))
            .ToArray();
        if (attrs.Length != 1)
        {
            throw new InvalidOperationException($"Expected one DebuggerDisplayAttribute on {objType}.");
        }

        CustomAttributeData cad = attrs[0];

        // Get the text of the DebuggerDisplayAttribute
        string attrText = (string)cad.ConstructorArguments[0].Value;

        string[] segments = attrText.Split(['{', '}']);

        if (segments.Length % 2 == 0)
        {
            throw new InvalidOperationException($"The DebuggerDisplayAttribute for {objType} lacks a closing brace.");
        }

        if (segments.Length == 1)
        {
            throw new InvalidOperationException($"The DebuggerDisplayAttribute for {objType} doesn't reference any expressions.");
        }

        StringBuilder sb = new();

        for (int i = 0; i < segments.Length; i += 2)
        {
            string literal = segments[i];
            sb.Append(literal);

            if (i + 1 < segments.Length)
            {
                string reference = segments[i + 1];
                bool noQuotes = reference.EndsWith(",nq", StringComparison.Ordinal);

                reference = reference.Replace(",nq", string.Empty);

                // Evaluate the reference.
                if (!TryEvaluateReference(obj, reference, out object member))
                {
                    throw new InvalidOperationException($"The DebuggerDisplayAttribute for {objType} contains the expression \"{reference}\".");
                }

                string memberString = GetDebuggerMemberString(member, noQuotes);

                sb.Append(memberString);
            }
        }

        return sb.ToString();
    }

    private static string GetDebuggerMemberString(object member, bool noQuotes)
    {
        return member switch
        {
            null => "null",
            byte or sbyte or short or ushort or int or uint or long or ulong or float or double => member.ToString(),
            string when noQuotes => member.ToString(),
            string => $"\"{member}\"",
            _ => $"{{{member}}}"
        };
    }

    private static bool TryEvaluateReference(object obj, string reference, out object member)
    {
        if (GetProperty(obj, reference) is { } property)
        {
            member = property.GetValue(obj);
            return true;
        }

        if (GetField(obj, reference) is { } field)
        {
            member = field.GetValue(obj);
            return true;
        }

        member = null;
        return false;
    }

    private static FieldInfo GetField(object obj, string fieldName)
    {
        for (Type t = obj.GetType(); t is not null; t = t.GetTypeInfo().BaseType)
        {
            if (t.GetTypeInfo().GetDeclaredField(fieldName) is { } field)
            {
                return field;
            }
        }

        return null;
    }

    private static PropertyInfo GetProperty(object obj, string propertyName)
    {
        for (Type t = obj.GetType(); t is not null; t = t.GetTypeInfo().BaseType)
        {
            if (t.GetTypeInfo().GetDeclaredProperty(propertyName) is { } property)
            {
                return property;
            }
        }

        return null;
    }
}
