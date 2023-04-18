// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

[StructLayout(LayoutKind.Sequential)]
internal unsafe struct EncoderParameterNative
{
    /// <summary>
    ///  GUID of the parameter.
    /// </summary>
    public Guid ParameterGuid;

    /// <summary>
    ///  Number of parameter values.
    /// </summary>
    public uint NumberOfValues;

    public EncoderParameterValueType ParameterValueType;

    /// <summary>
    ///  Pointer to the parameter values.
    /// </summary>
    public void* ParameterValue;
}
