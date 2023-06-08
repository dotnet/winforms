// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Windows.Win32.System.Com;

namespace Windows.Win32;

internal static unsafe partial class ComHelpers
{
    // Note that ComScope<T> needs to be the return value to faciliate using in a `using`.
    //
    //  using var stream = GetComScope<IStream>(obj, out bool success);

    /// <summary>
    ///  Gets a pointer for the specified <typeparamref name="T"/> for the given <paramref name="object"/>. Throws if
    ///  the desired pointer can not be obtained.
    /// </summary>
    internal static ComScope<T> GetComScope<T>(object? @object) where T : unmanaged, IComIID
        => new(GetComPointer<T>(@object));

    /// <summary>
    ///  Attempts to get a pointer for the specified <typeparamref name="T"/> for the given <paramref name="object"/>.
    /// </summary>
    internal static ComScope<T> TryGetComScope<T>(object? @object) where T : unmanaged, IComIID
        => TryGetComScope<T>(@object, out _);

    /// <summary>
    ///  Attempts to get a pointer for the specified <typeparamref name="T"/> for the given <paramref name="object"/>.
    /// </summary>
    internal static ComScope<T> TryGetComScope<T>(object? @object, out HRESULT hr) where T : unmanaged, IComIID
        => new(TryGetComPointer<T>(@object, out hr));

    /// <summary>
    ///  Gets the specified <typeparamref name="T"/> interface for the given <paramref name="object"/>. Throws if
    ///  the desired pointer can not be obtained.
    /// </summary>
    internal static T* GetComPointer<T>(object? @object) where T : unmanaged, IComIID
    {
        T* result = TryGetComPointer<T>(@object, out HRESULT hr);
        hr.ThrowOnFailure();
        return result;
    }

    /// <summary>
    ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="object"/>.
    /// </summary>
    /// <returns>The requested pointer or <see langword="null"/> if unsuccessful.</returns>
    internal static T* TryGetComPointer<T>(object? @object) where T : unmanaged, IComIID
        => TryGetComPointer<T>(@object, out _);

    /// <summary>
    ///  Queries for the given interface and releases it.
    /// </summary>
    internal static bool SupportsInterface<T>(object? @object) where T : unmanaged, IComIID
    {
        using var scope = TryGetComScope<T>(@object, out HRESULT hr);
        return hr.Succeeded;
    }

    /// <summary>
    ///  Attempts to get the specified <typeparamref name="T"/> interface for the given <paramref name="object"/>.
    /// </summary>
    /// <param name="result">
    ///  Typically either <see cref="HRESULT.S_OK"/> or <see cref="HRESULT.E_POINTER"/>. Check for success, not
    ///  specific results.
    /// </param>
    /// <returns>The requested pointer or <see langword="null"/> if unsuccessful.</returns>
    internal static T* TryGetComPointer<T>(object? @object, out HRESULT result) where T : unmanaged, IComIID
    {
        if (@object is null)
        {
            result = HRESULT.E_POINTER;
            return null;
        }

        IUnknown* ccw = null;
        if (@object is IManagedWrapper)
        {
            // One of our classes that we can generate a CCW for.
            ccw = (IUnknown*)Interop.WinFormsComWrappers.Instance.GetOrCreateComInterfaceForObject(@object, CreateComInterfaceFlags.None);

            if (ccw is not null && @object is IWrapperInitialize initialize)
            {
                initialize.OnInitialized(ccw);
            }
        }
        else if (ComWrappers.TryGetComInstance(@object, out nint unknown))
        {
            // A ComWrappers generated RCW.
            ccw = (IUnknown*)unknown;
        }
        else
        {
            // Fall back to COM interop if possible. Note that this will use the globally registered ComWrappers
            // if that exists (so it won't always fall into legacy COM interop).
            try
            {
                ccw = (IUnknown*)Marshal.GetIUnknownForObject(@object);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Did not find IUnknown for {@object.GetType().Name}. {ex.Message}");
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
    /// <devdoc>
    ///  <para>
    ///   This should remain internal to this class and will ultimately mostly be replaced by
    ///   https://github.com/dotnet/runtime/issues/79674.
    ///  </para>
    /// </devdoc>
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
