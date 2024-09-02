// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Base class for all effects.
/// </summary>
public abstract unsafe class Effect : IDisposable
{
    private CGpEffect* _nativeEffect;

    internal CGpEffect* NativeEffect => _nativeEffect;

    private protected Effect(Guid guid)
    {
        CGpEffect* nativeEffect;
        PInvoke.GdipCreateEffect(guid, &nativeEffect).ThrowIfFailed();
        _nativeEffect = nativeEffect;
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private protected void SetParameters<T>(ref T parameters) where T : unmanaged
    {
        fixed (T* p = &parameters)
        {
            PInvoke.GdipSetEffectParameters(NativeEffect, p, (uint)sizeof(T)).ThrowIfFailed();
            GC.KeepAlive(this);
        }
    }

    ~Effect() => Dispose(disposing: false);

    public virtual void Dispose(bool disposing)
    {
        if (_nativeEffect is not null)
        {
            PInvoke.GdipDeleteEffect(_nativeEffect);
            _nativeEffect = null;
        }
    }
}
#endif
