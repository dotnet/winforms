// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Windows.Win32.System.Com;

/// <summary>
///  Wraps an <see cref="IClassFactory"/> from a dynamically loaded assembly.
/// </summary>
internal unsafe class ComClassFactory : IDisposable
{
    private readonly string _filePath;
    public Guid ClassId { get; }
    private readonly HINSTANCE _instance;
    private readonly IClassFactory* _classFactory;

    private const string ExportMethodName = "DllGetClassObject";

    public ComClassFactory(
        string filePath,
        Guid classId)
    {
        _filePath = filePath;
        ClassId = classId;
        _instance = PInvoke.LoadLibraryEx(filePath, default);
        if (_instance.IsNull)
        {
            throw new Win32Exception();
        }

        // Dynamically get the class factory method.

        // HRESULT DllGetClassObject(
        //   [in] REFCLSID rclsid,
        //   [in] REFIID riid,
        //   [out] LPVOID* ppv
        // );

        FARPROC proc = PInvoke.GetProcAddress(_instance, ExportMethodName);
        IClassFactory* classFactory;
        ((delegate* unmanaged<Guid*, Guid*, void**, HRESULT>)proc.Value)(
            &classId, IID.Get<IClassFactory>(),
            (void**)&classFactory).ThrowOnFailure();
        _classFactory = classFactory;
    }

    internal HRESULT CreateInstance(out IUnknown* unknown)
    {
        unknown = default;
        fixed (IUnknown** u = &unknown)
        {
            return _classFactory->CreateInstance(null, IID.Get<IUnknown>(), (void**)u);
        }
    }

    internal HRESULT CreateInstance(out object unknown)
    {
        HRESULT result = CreateInstance(out IUnknown* punk);
        unknown = punk is null ? null : Marshal.GetObjectForIUnknown((nint)punk);
        return result;
    }

    public void Dispose()
    {
        _classFactory->Release();
        PInvokeCore.FreeLibrary(_instance);
    }
}
