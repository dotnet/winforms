// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Dynamic;
using System.Reflection;

namespace System;

/// <summary>
///  Internals (including privates) access wrapper for tests.
/// </summary>
/// <typeparam name="T">The type of the class being accessed.</typeparam>
/// <remarks>
///  <para>
///   Does not allow access to public members- use the object directly.
///  </para>
///  <para>
///   One should strive to *not* access internal state where otherwise avoidable.
///   Ask yourself if you can test the contract of the object in question
///   *without* manipulating internals directly. Often you can.
///  </para>
///  <para>
///   Where internals access is more useful are testing building blocks of more
///   complicated objects, such as internal helper methods or classes.
///  </para>
/// </remarks>
/// <example>
///  This class can also be derived from to create a strongly typed wrapper
///  that can then be associated via an extension method for the given type
///  to provide consistent discovery and access.
///
///  <![CDATA[
///   public class GuidTestAccessor : TestAccessor<Guid>
///   {
///     public TestAccessor(Guid instance) : base(instance) {}
///
///     public int A => Dynamic._a;
///   }
///
///   public static partial class TestAccessors
///   {
///       public static GuidTestAccessor TestAccessor(this Guid guid)
///           => new GuidTestAccessor(guid);
///   }
///  ]]>
/// </example>
public class TestAccessor<T> : ITestAccessor
{
    private static readonly Type s_type = typeof(T);
    private readonly T? _instance;
    private readonly DynamicWrapper _dynamicWrapper;

    /// <param name="instance">The type instance, can be null for statics.</param>
    public TestAccessor(T? instance)
    {
        _instance = instance;
        _dynamicWrapper = new DynamicWrapper(_instance);
    }

    /// <inheritdoc/>
    public TDelegate CreateDelegate<TDelegate>(string? methodName = null)
        where TDelegate : Delegate
    {
        Type type = typeof(TDelegate);
        MethodInfo? invokeMethodInfo = type.GetMethod("Invoke");
        Type[] types = invokeMethodInfo is null ? [] : invokeMethodInfo.GetParameters().Select(pi => pi.ParameterType).ToArray();

        // To make it easier to write a class wrapper with a number of delegates,
        // we'll take the name from the delegate itself when unspecified.
        methodName ??= type.Name;

        MethodInfo? methodInfo = s_type.GetMethod(
            methodName,
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
            binder: null,
            types,
            modifiers: null) ?? throw new ArgumentException($"Could not find non public method {methodName}.");

        return (TDelegate)methodInfo.CreateDelegate(type, methodInfo.IsStatic ? null : _instance);
    }

    /// <inheritdoc/>
    public dynamic Dynamic => _dynamicWrapper;

    private sealed class DynamicWrapper : DynamicObject
    {
        private readonly object? _instance;

        public DynamicWrapper(object? instance)
            => _instance = instance;

        public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
        {
            result = null;
            ArgumentNullException.ThrowIfNull(args);
            ArgumentNullException.ThrowIfNull(binder);

            MethodInfo? methodInfo = null;
            Type? type = s_type;

            do
            {
                try
                {
                    methodInfo = type?.GetMethod(
                        binder.Name,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                }
                catch (AmbiguousMatchException)
                {
                    // More than one match for the name, specify the arguments.
                    // We currently do not have a scenario where we are trying to pass null as an argument
                    // to an overloaded method. This will need to be updated once we have a scenario.
                    methodInfo = type?.GetMethod(
                        binder.Name,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                        binder: null,
                        args.Select(a => a!.GetType()).ToArray(),
                        modifiers: null);
                }

                if (methodInfo is not null || type == typeof(object))
                {
                    // Found something, or already at the top of the type hierarchy
                    break;
                }

                // Walk up the hierarchy
                type = type?.BaseType;
            }
            while (true);

            if (methodInfo is null)
                return false;

            result = methodInfo.Invoke(_instance, args);
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object? value)
        {
            MemberInfo? info = TestAccessor<T>.DynamicWrapper.GetFieldOrPropertyInfo(binder.Name);
            if (info is null)
                return false;

            SetValue(info, value);
            return true;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object? result)
        {
            result = null;

            MemberInfo? info = TestAccessor<T>.DynamicWrapper.GetFieldOrPropertyInfo(binder.Name);
            if (info is null)
                return false;

            result = GetValue(info);
            return true;
        }

        private static MemberInfo? GetFieldOrPropertyInfo(string memberName)
        {
            Type? type = s_type;
            MemberInfo? info;

            do
            {
                info = (MemberInfo?)type?.GetField(
                    memberName,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic)
                    ?? type?.GetProperty(
                        memberName,
                        BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

                if (info is not null || type == typeof(object))
                {
                    // Found something, or already at the top of the type hierarchy
                    break;
                }

                // Walk up the type hierarchy
                type = type?.BaseType;
            }
            while (true);

            return info;
        }

        private object? GetValue(MemberInfo memberInfo)
            => memberInfo switch
            {
                FieldInfo fieldInfo => fieldInfo.GetValue(_instance),
                PropertyInfo propertyInfo => propertyInfo.GetValue(_instance),
                _ => throw new InvalidOperationException()
            };

        private void SetValue(MemberInfo memberInfo, object? value)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    fieldInfo.SetValue(_instance, value);
                    break;
                case PropertyInfo propertyInfo:
                    propertyInfo.SetValue(_instance, value);
                    break;
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
