// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Diagnostics.CodeAnalysis;
using System.Reflection.Metadata;

namespace System.Windows.Forms.TestUtilities;

internal class TypedDataObject : ManagedDataObject, ITypedDataObject
{
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>([MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        throw new NotImplementedException();
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, [MaybeNullWhen(false), NotNullWhen(true)] out T data)
    {
        data = default;
        if (format == s_format && _data is T t)
        {
            data = t;
            return true;
        }

        return false;
    }

    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(string format, bool autoConvert, [MaybeNullWhen(false), NotNullWhen(true)] out T data) =>
        throw new NotImplementedException();
    public bool TryGetData<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>(
        string format,
        Func<TypeName, Type?> resolver,
        bool autoConvert,
        [MaybeNullWhen(false), NotNullWhen(true)] out T data) => TryGetData(format, out data);
}
