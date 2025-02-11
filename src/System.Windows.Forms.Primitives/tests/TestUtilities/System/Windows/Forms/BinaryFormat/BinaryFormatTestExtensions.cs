// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace System.Private.Windows.BinaryFormat;

internal static class BinaryFormatTestExtensions
{
    /// <summary>
    ///  Returns `true` if the <see cref="Type"/> would use the <see cref="BinaryFormatter"/> for the purposes
    ///  of designer serialization (either through Resx or IPropertyBag for ActiveXImpl).
    /// </summary>
    public static bool IsBinaryFormatted(this Type type)
    {
        bool iSerializable = type.IsAssignableTo(typeof(ISerializable));
#pragma warning disable SYSLIB0050 // Type or member is obsolete
        bool serializable = type.IsSerializable;
#pragma warning restore SYSLIB0050

        if (!iSerializable && !serializable)
        {
            return false;
        }

        TypeConverter converter;
        try
        {
            converter = TypeDescriptor.GetConverter(type);
        }
        catch (Exception)
        {
            // No valid type converter.
            return true;
        }

        return !((converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string)))
            || (converter.CanConvertFrom(typeof(byte[])) && converter.CanConvertTo(typeof(byte[]))));
    }
}
