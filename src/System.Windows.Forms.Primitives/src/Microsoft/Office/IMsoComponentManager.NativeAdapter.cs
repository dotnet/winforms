// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Com;

namespace Microsoft.Office;

internal unsafe partial struct IMsoComponentManager
{
    internal unsafe class NativeAdapter : Interface, IDisposable
    {
        private AgileComPointer<IMsoComponentManager>? _manager;
        public NativeAdapter(IMsoComponentManager* manager) => _manager =
#if DEBUG
            new(manager, takeOwnership: true, trackDisposal: false);
#else
            new(manager, takeOwnership: true);
#endif

        public void Dispose() => DisposeHelper.NullAndDispose(ref _manager);

        HRESULT Interface.QueryService(Guid* guidService, Guid* iid, void** ppvObj)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->QueryService(guidService, iid, ppvObj);
        }

        BOOL Interface.FDebugMessage(nint dwReserved, uint msg, WPARAM wParam, LPARAM lParam)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FDebugMessage(dwReserved, msg, wParam, lParam);
        }

        BOOL Interface.FRegisterComponent(IMsoComponent* piComponent, MSOCRINFO* pcrinfo, nuint* dwComponentID)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FRegisterComponent(piComponent, pcrinfo, dwComponentID);
        }

        BOOL Interface.FRevokeComponent(nuint dwComponentID)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FRevokeComponent(dwComponentID);
        }

        BOOL Interface.FUpdateComponentRegistration(nuint dwComponentID, MSOCRINFO* pcrinfo)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FUpdateComponentRegistration(dwComponentID, pcrinfo);
        }

        BOOL Interface.FOnComponentActivate(nuint dwComponentID)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FOnComponentActivate(dwComponentID);
        }

        BOOL Interface.FSetTrackingComponent(nuint dwComponentID, BOOL fTrack)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FSetTrackingComponent(dwComponentID, fTrack);
        }

        void Interface.OnComponentEnterState(
            nuint dwComponentID,
            msocstate uStateID,
            msoccontext uContext,
            uint cpicmExclude,
            IMsoComponentManager** rgpicmExclude,
            uint dwReserved)
        {
            using var manager = _manager!.GetInterface();
            manager.Value->OnComponentEnterState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude, dwReserved);
        }

        BOOL Interface.FOnComponentExitState(
            nuint dwComponentID,
            msocstate uStateID,
            msoccontext uContext,
            uint cpicmExclude,
            IMsoComponentManager** rgpicmExclude)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FOnComponentExitState(dwComponentID, uStateID, uContext, cpicmExclude, rgpicmExclude);
        }

        BOOL Interface.FInState(msocstate uStateID, void* pvoid)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FInState(uStateID, pvoid);
        }

        BOOL Interface.FContinueIdle()
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FContinueIdle();
        }

        BOOL Interface.FPushMessageLoop(nuint dwComponentID, msoloop uReason, void* pvLoopData)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FPushMessageLoop(dwComponentID, uReason, pvLoopData);
        }

        BOOL Interface.FCreateSubComponentManager(IUnknown* punkOuter, IUnknown* punkServProv, Guid* riid, void** ppvObj)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FCreateSubComponentManager(punkOuter, punkServProv, riid, ppvObj);
        }

        BOOL Interface.FGetParentComponentManager(IMsoComponentManager** ppicm)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FGetParentComponentManager(ppicm);
        }

        BOOL Interface.FGetActiveComponent(msogac dwgac, IMsoComponent** ppic, MSOCRINFO* pcrinfo, uint dwReserved)
        {
            using var manager = _manager!.GetInterface();
            return manager.Value->FGetActiveComponent(dwgac, ppic, pcrinfo, dwReserved);
        }
    }
}
