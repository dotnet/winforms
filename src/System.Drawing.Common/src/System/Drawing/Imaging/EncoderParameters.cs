// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;

namespace System.Drawing.Imaging;

public sealed class EncoderParameters : IDisposable
{
    public EncoderParameters(int count) => Param = new EncoderParameter[count];

    public EncoderParameters() => Param = new EncoderParameter[1];

    public EncoderParameter[] Param { get; set; }

    internal unsafe GdiPlus.EncoderParameters* ConvertToNative()
    {
        int length = Param.Length;

        // The struct has the first EncoderParameter in it.
        GdiPlus.EncoderParameters* native = (GdiPlus.EncoderParameters*)Marshal.AllocHGlobal(
            sizeof(GdiPlus.EncoderParameters) + ((length - 1) * sizeof(GdiPlus.EncoderParameter)));

        native->Count = (uint)length;
        Span<GdiPlus.EncoderParameter> parameters = native->Parameters;

        for (int i = 0; i < length; i++)
        {
            parameters[i] = Param[i].ToNative();
        }

        return native;
    }

    internal static unsafe EncoderParameters ConvertFromNative(GdiPlus.EncoderParameters* native)
    {
        if (native is null)
        {
            throw Status.InvalidParameter.GetException();
        }

        var nativeParameters = native->Parameters;
        EncoderParameters parameters = new(nativeParameters.Length);
        for (int i = 0; i < nativeParameters.Length; i++)
        {
            parameters.Param[i] = new EncoderParameter(
                new Encoder(nativeParameters[i].Guid),
                (int)nativeParameters[i].NumberOfValues,
                (EncoderParameterValueType)nativeParameters[i].Type,
                (nint)nativeParameters[i].Value);
        }

        return parameters;
    }

    public void Dispose()
    {
        foreach (EncoderParameter p in Param)
        {
            p?.Dispose();
        }

        Param = null!;
    }
}
