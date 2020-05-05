// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Dynamic;
using System.Linq;
using System.Reflection;

namespace System
{
    /// <summary>
    ///  Internals (including privates) access wrapper for tests.
    /// </summary>
    /// <typeparam name="T">The type of the class being accessed.</typeparam>
    /// <remarks>
    ///  Does not allow access to public members- use the object directly.
    ///
    ///  One should strive to *not* access internal state where otherwise avoidable.
    ///  Ask yourself if you can test the contract of the object in question
    ///  *without* manipulating internals directly. Often you can.
    ///
    ///  Where internals access is more useful are testing building blocks of more
    ///  complicated objects, such as internal helper methods or classes.
    ///
    ///  This can be used to access private/internal objects as well via
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
        protected readonly T _instance;
        private readonly DynamicWrapper _dynamicWrapper;

        /// <param name="instance">The type instance, can be null for statics.</param>
        public TestAccessor(T instance)
        {
            _instance = instance;
            _dynamicWrapper = new DynamicWrapper(_instance);
        }

        /// <inheritdoc/>
        public TDelegate CreateDelegate<TDelegate>(string methodName = null)
            where TDelegate : Delegate
        {
            Type type = typeof(TDelegate);
            Type[] types = type.GetMethod("Invoke").GetParameters().Select(pi => pi.ParameterType).ToArray();

            // To make it easier to write a class wrapper with a number of delegates,
            // we'll take the name from the delegate itself when unspecified.
            if (methodName == null)
                methodName = type.Name;

            MethodInfo methodInfo = s_type.GetMethod(
                methodName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                binder: null,
                types,
                modifiers: null);

            if (methodInfo == null)
                throw new ArgumentException($"Could not find non public method {methodName}.");

            return (TDelegate)methodInfo.CreateDelegate(type, methodInfo.IsStatic ? (object)null : _instance);
        }

        /// <inheritdoc/>
        public dynamic Dynamic => _dynamicWrapper;

        private class DynamicWrapper : DynamicObject
        {
            private readonly object _instance;

            public DynamicWrapper(object instance)
                => _instance = instance;

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                result = null;

                MethodInfo methodInfo = null;

                try
                {
                    methodInfo = s_type.GetMethod(
                        binder.Name,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                }
                catch (AmbiguousMatchException)
                {
                    // More than one match for the name, specify the arguments
                    methodInfo = s_type.GetMethod(
                        binder.Name,
                        BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static,
                        binder: null,
                        args.Select(a => a.GetType()).ToArray(),
                        modifiers: null);
                }

                if (methodInfo == null)
                    return false;

                result = methodInfo.Invoke(_instance, args);
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                FieldInfo fieldInfo = GetFieldInfo(binder.Name);
                if (fieldInfo != null)
                {
                    fieldInfo.SetValue(_instance, value);
                    return true;
                }

                PropertyInfo propertyInfo = GetPropertyInfo(binder.Name);
                if (propertyInfo == null)
                    return false;

                propertyInfo.SetValue(_instance, value);
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                FieldInfo fieldInfo = GetFieldInfo(binder.Name);
                if (fieldInfo != null)
                {
                    result = fieldInfo.GetValue(_instance);
                    return true;
                }

                PropertyInfo propertyInfo = GetPropertyInfo(binder.Name);
                if (propertyInfo == null)
                    return false;

                result = propertyInfo.GetValue(_instance);
                return true;
            }

            private PropertyInfo GetPropertyInfo(string propertyName)
                => s_type.GetProperty(
                    propertyName,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);

            private FieldInfo GetFieldInfo(string fieldName)
                => s_type.GetField(
                    fieldName,
                    BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
        }
    }
}
