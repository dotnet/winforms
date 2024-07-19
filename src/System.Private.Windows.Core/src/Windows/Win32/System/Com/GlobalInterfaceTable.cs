// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices.Marshalling;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation;

/// <summary>
///  Wrapper for the COM global interface table.
/// </summary>
internal static unsafe partial class GlobalInterfaceTable
{
    private static readonly IGlobalInterfaceTable* s_globalInterfaceTable;

    static GlobalInterfaceTable()
    {
        Guid guid = CLSID.StdGlobalInterfaceTable;

        fixed (IGlobalInterfaceTable** git = &s_globalInterfaceTable)
        {
            PInvokeCore.CoCreateInstance(
                &guid,
                pUnkOuter: null,
                CLSCTX.CLSCTX_INPROC_SERVER,
                IID.Get<IGlobalInterfaceTable>(),
                (void**)git).ThrowOnFailure();
        }
    }

    /// <summary>
    ///  Registers the given <paramref name="interface"/> in the global interface table. This decrements the
    ///  ref count so that the entry in the table will "own" the interface (as it increments the ref count).
    /// </summary>
    /// <returns>The cookie used to refer to the interface in the table.</returns>
    public static uint RegisterInterface<TInterface>(TInterface* @interface)
        where TInterface : unmanaged, IComIID
    {
        uint cookie;
        HRESULT hr = s_globalInterfaceTable->RegisterInterfaceInGlobal(
            (IUnknown*)@interface,
            IID.Get<TInterface>(),
            &cookie);
        hr.ThrowOnFailure();
        return cookie;
    }

    /// <summary>
    ///  Gets an agile interface for the <paramref name="cookie"/> that was given back by
    ///  <see cref="RegisterInterface{TInterface}(TInterface*)"/>
    /// </summary>
    public static ComScope<TInterface> GetInterface<TInterface>(uint cookie, out HRESULT result)
        where TInterface : unmanaged, IComIID
    {
        ComScope<TInterface> @interface = new(null);
        result = s_globalInterfaceTable->GetInterfaceFromGlobal(cookie, IID.Get<TInterface>(), @interface);
        return @interface;
    }

    /// <summary>
    ///  Revokes the interface registered with <see cref="RegisterInterface{TInterface}(TInterface*)"/>.
    ///  This will decrement the ref count for the interface.
    /// </summary>
    public static HRESULT RevokeInterface(uint cookie)
    {
        HRESULT hr = s_globalInterfaceTable->RevokeInterfaceFromGlobal(cookie);
        Debug.WriteLineIf(hr.Failed, $"{nameof(GlobalInterfaceTable)}: Failed to revoke interface.");
        return hr;
    }

    /// <summary>
    ///  Creates a new instance of an <see cref="IIUnknownStrategy"/> for <see cref="StrategyBasedComWrappers"/>
    ///  that uses the Global Interface Table.
    /// </summary>
    /// <remarks>
    ///  <para>
    ///   The returned instance should not be cached.
    ///  </para>
    /// </remarks>
    public static IIUnknownStrategy CreateUnknownStrategy() => new UnknownStrategy();
}
