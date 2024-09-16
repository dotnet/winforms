// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.System.Com;

internal static class ComSafeArrayScopeExtensions
{
    /// <summary>
    ///  Creates a <see cref="ComSafeArrayScope{T}"/> where T is <typeparamref name="TComStruct"/> from an array of
    ///  <typeparamref name="TComInterface"/>.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   <typeparamref name="TComStruct"/>
    ///    must implement <see cref="IComInterface{T}"/> where T is <typeparamref name="TComInterface"/>.
    ///  </para>
    /// </remarks>
    public static unsafe ComSafeArrayScope<TComStruct> CreateComSafeArrayScope<TComStruct, TComInterface>(this TComInterface[] interfaces)
        where TComStruct : unmanaged, IComIID, IComInterface<TComInterface>
    {
        uint length = (uint)interfaces.Length;
        ComSafeArrayScope<TComStruct> scope = new(length);
        for (int i = 0; i < length; i++)
        {
            // SAFEARRAY will add ref, a using is needed to
            // release to maintain the correct ref count.
            using var pointer = ComHelpers.GetComScope<TComStruct>(interfaces[i]);
            scope[i] = pointer;
        }

        return scope;
    }
}
