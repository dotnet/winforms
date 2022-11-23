// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

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
    private AgileComPointer<IClassFactory> _classFactory;

    private const string ExportMethodName = "DllGetClassObject";

    public ComClassFactory(
        string filePath,
        Guid classId)
    {
        HRESULT result = PInvoke.OleInitialize(pvReserved: (void*)null);

        _filePath = filePath;
        ClassId = classId;
        _instance = PInvoke.LoadLibraryEx(filePath, HANDLE.Null, default);
        if (_instance.IsNull)
        {
            throw new Win32Exception();
        }

        string name = PInvoke.GetModuleFileNameLongPath(_instance);

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
        _classFactory = new(classFactory);
    }

    internal HRESULT CreateInstance(out IUnknown* unknown)
    {
        unknown = default;
        fixed (IUnknown** u = &unknown)
        {
            using var factory = _classFactory.GetInterface();
            return factory.Value->CreateInstance(null, IID.Get<IUnknown>(), (void**)u);
        }
    }

    internal HRESULT CreateInstance(out object unknown)
    {
        HRESULT result = CreateInstance(out IUnknown* punk);
        unknown = result.Failed || punk is null ? null : Marshal.GetObjectForIUnknown((nint)punk);
        return result;
    }

    public void Dispose()
    {
        _classFactory.Dispose();
        PInvoke.FreeLibrary(_instance);
    }
}
