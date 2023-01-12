// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Windows.Win32;

internal static unsafe partial class ComHelpers
{
    // Note that ComScope<T> needs to be the return value to faciliate using in a `using`.
    //
    //  using var stream = GetComScope<IStream>(obj, out bool success);

    /// <summary>
    ///  Gets a pointer for the specified <typeparamref name="T"/> for the given <paramref name="obj"/>. Throws if
    ///  the desired pointer can not be obtained.
    /// </summary>
    internal static ComScope<T> GetComScope<T>(object obj) where T : unmanaged, IComIID
        => new(GetComPointer<T>(obj));

    /// <summary>
    ///  Attempts to get a pointer for the specified <typeparamref name="T"/> for the given <paramref name="obj"/>.
    /// </summary>
    internal static ComScope<T> TryGetComScope<T>(object obj, out HRESULT hr) where T : unmanaged, IComIID
        => new(TryGetComPointer<T>(obj, out hr));

    /// <summary>
    ///  Gets the specified <typeparamref name="T"/> interface for the given <paramref name="obj"/>. Throws if
    ///  the desired pointer can not be obtained.
    /// </summary>
    internal static T* GetComPointer<T>(object? obj) where T : unmanaged, IComIID
    {
        T* result = TryGetComPointer<T>(obj, out HRESULT hr);
        hr.ThrowOnFailure();
        return result;
    }

    /// <summary>
    ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="obj"/>.
    /// </summary>
    /// <returns>The requested pointer or <see langword="null"/> if unsuccessful.</returns>
    internal static T* TryGetComPointer<T>(object? obj) where T : unmanaged, IComIID
        => TryGetComPointer<T>(obj, out _);

    /// <summary>
    ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="obj"/>.
    /// </summary>
    /// <param name="result">
    ///  Typically either <see cref="HRESULT.S_OK"/> or <see cref="HRESULT.E_POINTER"/>. Check for success, not
    ///  specific results.
    /// </param>
    /// <returns>The requested pointer or <see langword="null"/> if unsuccessful.</returns>
    internal static T* TryGetComPointer<T>(object? obj, out HRESULT result) where T : unmanaged, IComIID
    {
        if (obj is null)
        {
            result = HRESULT.E_POINTER;
            return null;
        }

        IUnknown* ccw = null;
        if (obj is IManagedWrapper)
        {
            ccw = (IUnknown*)Interop.WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(obj, CreateComInterfaceFlags.None);

            if (ccw is not null && obj is IWrapperInitialize initialize)
            {
                initialize.OnInitialized(ccw);
            }
        }
        else
        {
            // Fall back to COM interop if possible.
            try
            {
                ccw = (IUnknown*)Marshal.GetIUnknownForObject(obj);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Did not find IUnknown for {obj.GetType().Name}. {ex.Message}");
            }
        }

        if (ccw is null)
        {
            result = HRESULT.E_NOINTERFACE;
            return null;
        }

        if (typeof(T) == typeof(IUnknown))
        {
            // No need to query if we wanted IUnknown.
            result = HRESULT.S_OK;
            return (T*)ccw;
        }

        // Now query out the requested interface
        result = ccw->QueryInterface(IID.GetRef<T>(), out void* ppvObject);
        ccw->Release();
        return (T*)ppvObject;
    }

    /// <summary>
    ///  Attempts to unwrap one of our ComWrapper CCWs as a particular managed object.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   This should remain internal to this class and will ultimately mostly be replaced by
    ///   https://github.com/dotnet/runtime/issues/79674.
    ///  </para>
    /// </remarks>
    private static bool TryUnwrapComWrapperCCW<TWrapper>(
        IUnknown* unknown,
        [NotNullWhen(true)] out TWrapper? @interface) where TWrapper : class
    {
        using var wrapper = ComScope<IComCallableWrapper>.TryQueryFrom(unknown, out HRESULT hr);
        if (hr.Succeeded)
        {
            object obj = ComWrappers.ComInterfaceDispatch.GetInstance<object>((ComWrappers.ComInterfaceDispatch*)unknown);
            if (obj is TWrapper desired)
            {
                @interface = desired;
                return true;
            }
            else
            {
                Debug.WriteLine($"{nameof(TryGetManagedInterface)}: Found a manual CCW, but couldn't unwrap to {typeof(TWrapper).Name}");
            }
        }

        @interface = default;
        return false;
    }

    /// <summary>
    ///  Attempts to get a managed wrapper of the specified type for the given COM interface.
    /// </summary>
    /// <param name="takeOwnership">
    ///  When <see langword="true"/>, releases the original <paramref name="unknown"/> whether successful or not.
    /// </param>
    internal static bool TryGetManagedInterface<TWrapper>(
        IUnknown* unknown,
        bool takeOwnership,
        [NotNullWhen(true)] out TWrapper? @interface) where TWrapper : class
    {
        @interface = null;
        if (unknown is null)
        {
            return false;
        }

        try
        {
            // Check to see if we're one of our own CCWs and unwrap.
            if (TryUnwrapComWrapperCCW(unknown, out @interface))
            {
                return true;
            }

            // Fall back to Marshal.
            @interface = (TWrapper)Marshal.GetObjectForIUnknown((nint)unknown);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"{nameof(TryGetManagedInterface)}: Failed to get RCW for {typeof(TWrapper).Name}. {ex.Message}");
            return false;
        }
        finally
        {
            if (takeOwnership)
            {
                uint count = unknown->Release();
                Debug.WriteLineIf(count > 0, $"{nameof(TryGetManagedInterface)}: Count for {typeof(TWrapper).Name} is {count} after release.");
            }
        }
    }

    /// <summary>
    ///  Returns <see langword="true"/> if the given <paramref name="object"/> is projected as the given <paramref name="unknown"/>.
    /// </summary>
    internal static bool WrapsManagedObject(object @object, IUnknown* unknown)
    {
        if (TryUnwrapComWrapperCCW(unknown, out object? foundObject))
        {
            return @object == foundObject;
        }

        using ComScope<IUnknown> ccw = new((IUnknown*)(void*)Marshal.GetIUnknownForObject(@object));
        return ccw.Value == unknown;
    }
}
