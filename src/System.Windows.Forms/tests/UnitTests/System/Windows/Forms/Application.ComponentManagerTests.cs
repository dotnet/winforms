// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using Microsoft.Office;
using Windows.Win32.System.Com;

namespace System.Windows.Forms.Tests.Interop_Mso;

public unsafe class IMsoComponentManagerTests
{
    private IMsoComponentManager.Interface CreateComponentManager()
        => (IMsoComponentManager.Interface)Activator.CreateInstance(
            typeof(Application).Assembly.GetType("System.Windows.Forms.Application+ComponentManager")!,
            nonPublic: true)!;

    [Fact]
    public void FDebugMessage_ReturnsTrue()
    {
        var manager = CreateComponentManager();
        Assert.True(manager.FDebugMessage(0, 0, default, default));
    }

    [Fact]
    public void QueryService_HandlesNull()
    {
        var manager = CreateComponentManager();

        // Shouldn't try and deref a null
        Assert.Equal(HRESULT.E_NOINTERFACE, manager.QueryService(null, null, null));

        // Should null out obj pointer
        void* obj = (void*)0xDEADBEEF;
        Assert.Equal(HRESULT.E_NOINTERFACE, manager.QueryService(null, null, &obj));
        Assert.True(obj is null);
    }

    [Fact]
    public void FRegisterComponent_HandlesNull()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));
        MSOCRINFO info = default;
        UIntPtr id = default;

        Assert.False(manager.FRegisterComponent(component, &info, null));
        Assert.False(manager.FRegisterComponent(component, null, &id));
        Assert.Equal(UIntPtr.Zero, id);
    }

    [Fact]
    public void FRegisterComponent_RejectsUnsized()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));
        MSOCRINFO info = default;
        UIntPtr id = default;

        Assert.False(manager.FRegisterComponent(component, &info, &id));
        Assert.Equal(UIntPtr.Zero, id);
    }

    [Fact]
    public void FRegisterComponent_Cookies()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;

        Assert.True(manager.FRegisterComponent(component, &info, &id));
        Assert.NotEqual(UIntPtr.Zero, id);

        UIntPtr newId = default;
        Assert.True(manager.FRegisterComponent(component, &info, &newId));
        Assert.NotEqual(UIntPtr.Zero, newId);

        Assert.NotEqual(id, newId);
    }

    [Fact]
    public void FRevokeComponent()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;

        Assert.False(manager.FRevokeComponent(UIntPtr.Zero));
        Assert.True(manager.FRegisterComponent(component, &info, &id));
        Assert.True(manager.FRevokeComponent(id));
        Assert.False(manager.FRevokeComponent(id));
    }

    [Fact]
    public void FUpdateComponentRegistration_HandlesNull()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;

        Assert.True(manager.FRegisterComponent(component, &info, &id));
        Assert.False(manager.FUpdateComponentRegistration(id, null));
    }

    [Fact]
    public void FUpdateComponentRegistration()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;

        Assert.False(manager.FUpdateComponentRegistration(id, &info));
        Assert.True(manager.FRegisterComponent(component, &info, &id));
        Assert.True(manager.FUpdateComponentRegistration(id, &info));
    }

    [Fact]
    public void FOnComponentActivate_InvalidId()
    {
        var manager = CreateComponentManager();
        Assert.False(manager.FOnComponentActivate(default));
    }

    [Fact]
    public void FSetTrackingComponent_InvalidId()
    {
        var manager = CreateComponentManager();
        Assert.False(manager.FSetTrackingComponent(default, true));
        Assert.False(manager.FSetTrackingComponent(default, false));
    }

    [Fact]
    public void FSetTrackingComponent()
    {
        var manager = CreateComponentManager();

        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;

        Assert.True(manager.FRegisterComponent(component, &info, &id));

        Assert.True(manager.FSetTrackingComponent(id, true));

        // Returns false if we're already tracking
        Assert.False(manager.FSetTrackingComponent(id, true));

        Assert.True(manager.FSetTrackingComponent(id, false));

        // If we aren't tracking, untracking should return false
        Assert.False(manager.FSetTrackingComponent(id, false));
        Assert.True(manager.FSetTrackingComponent(id, true));
    }

    [Fact]
    public void OnComponentEnterState_HandlesNull()
    {
        var manager = CreateComponentManager();
        manager.OnComponentEnterState(default, default, default, default, null, default);
    }

    [Fact]
    public void OnComponentEnterState_Notification()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(new MockWrapper(mock.Object)));

        mock.Setup(m => m.OnEnterState(msocstate.Modal, true));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;
        Assert.True(manager.FRegisterComponent(component, &info, &id));

        // No call on "Others"
        manager.OnComponentEnterState(default, msocstate.Modal, msoccontext.Others, 0, null, 0);
        mock.Verify(m => m.OnEnterState(msocstate.Modal, true), Times.Never);

        manager.OnComponentEnterState(default, msocstate.Modal, msoccontext.All, 0, null, 0);
        mock.Verify(m => m.OnEnterState(msocstate.Modal, true), Times.Once);

        manager.OnComponentEnterState(default, msocstate.Modal, msoccontext.Mine, 0, null, 0);
        mock.Verify(m => m.OnEnterState(msocstate.Modal, true), Times.Exactly(2));
    }

    [Fact]
    public void FOnComponentExitState_HandlesNull()
    {
        var manager = CreateComponentManager();
        Assert.False(manager.FOnComponentExitState(default, default, default, default, null));
    }

    [Fact]
    public void FOnComponentExitState_Notification()
    {
        var manager = CreateComponentManager();
        Mock<IMsoComponent.Interface> mock = new(MockBehavior.Strict);
        using var component = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock.Object));

        mock.Setup(m => m.OnEnterState(msocstate.Modal, false));

        MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
        UIntPtr id = default;
        Assert.True(manager.FRegisterComponent(component, &info, &id));

        // No call on "Others"
        manager.FOnComponentExitState(default, msocstate.Modal, msoccontext.Others, 0, null);
        mock.Verify(m => m.OnEnterState(msocstate.Modal, false), Times.Never);

        manager.FOnComponentExitState(default, msocstate.Modal, msoccontext.All, 0, null);
        mock.Verify(m => m.OnEnterState(msocstate.Modal, false), Times.Once);

        manager.FOnComponentExitState(default, msocstate.Modal, msoccontext.Mine, 0, null);
        mock.Verify(m => m.OnEnterState(msocstate.Modal, false), Times.Exactly(2));
    }

    [Fact]
    public void FInState()
    {
        var manager = CreateComponentManager();
        Assert.True(manager.FInState(0, null));
        manager.OnComponentEnterState(default, msocstate.Modal, default, 0, null, 0);
        Assert.False(manager.FInState(0, null));
        Assert.True(manager.FInState(msocstate.Modal, null));
        manager.OnComponentEnterState(default, msocstate.Recording, default, 0, null, 0);
        Assert.True(manager.FInState(msocstate.Recording, null));
        manager.FOnComponentExitState(default, msocstate.RedrawOff, default, 0, null);
        Assert.True(manager.FInState(0, null));
    }

    [Fact]
    public void FContinueIdle()
    {
        // Making sure we don't crash- there may or may not be messages in the queue
        var manager = CreateComponentManager();
        manager.FContinueIdle();
    }

    [Fact]
    public void FPushMessageLoop_InvalidComponent()
    {
        var manager = CreateComponentManager();
        Assert.False(manager.FPushMessageLoop(default, default, null));
    }

    [Fact]
    public void FCreateSubComponentManager_HandlesNull()
    {
        var manager = CreateComponentManager();

        // Shouldn't try and deref a null
        Assert.False(manager.FCreateSubComponentManager(default, default, null, null));

        // Should null out obj pointer
        void* obj = (void*)0xDEADBEEF;
        Assert.False(manager.FCreateSubComponentManager(default, default, null, &obj));
        Assert.True(obj is null);
    }

    [Fact]
    public void FGetParentComponentManager_HandlesNull()
    {
        var manager = CreateComponentManager();

        // Shouldn't try and deref a null
        Assert.False(manager.FGetParentComponentManager(null));

        // Should null out obj pointer
        void* obj = (void*)0xDEADBEEF;
        Assert.False(manager.FGetParentComponentManager((IMsoComponentManager**)&obj));
        Assert.True(obj is null);
    }

    [Fact]
    public void FGetActiveComponent()
    {
        var manager = CreateComponentManager();
        Assert.False(manager.FGetActiveComponent(msogac.Active, null, null, 0));

        Mock<IMsoComponent.Interface> mock1 = new(MockBehavior.Strict);
        using var component1 = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock1.Object));

        Mock<IMsoComponent.Interface> mock2 = new(MockBehavior.Strict);
        using var component2 = ComHelpers.GetComScope<IMsoComponent>(new MockWrapper(mock2.Object));

        MSOCRINFO info = new MSOCRINFO
        {
            cbSize = (uint)sizeof(MSOCRINFO),
            uIdleTimeInterval = 1
        };

        UIntPtr firstId = default;
        Assert.True(manager.FRegisterComponent(component1, &info, &firstId));
        info.uIdleTimeInterval = 2;
        UIntPtr secondId = default;
        Assert.True(manager.FRegisterComponent(component2, &info, &secondId));

        Assert.False(manager.FGetActiveComponent(msogac.Active, null, null, 0));

        // Just an active component
        Assert.True(manager.FOnComponentActivate(firstId));
        Assert.False(manager.FGetActiveComponent(msogac.Tracking, null, &info, 0));
        Assert.True(manager.FGetActiveComponent(msogac.Active, null, &info, 0));
        Assert.Equal(1u, info.uIdleTimeInterval);
        Assert.True(manager.FGetActiveComponent(msogac.TrackingOrActive, null, &info, 0));
        Assert.Equal(1u, info.uIdleTimeInterval);

        // Active and tracking
        Assert.True(manager.FSetTrackingComponent(secondId, true));
        Assert.True(manager.FGetActiveComponent(msogac.Tracking, null, &info, 0));
        Assert.Equal(2u, info.uIdleTimeInterval);
        Assert.True(manager.FGetActiveComponent(msogac.Active, null, &info, 0));
        Assert.Equal(1u, info.uIdleTimeInterval);
        Assert.True(manager.FGetActiveComponent(msogac.TrackingOrActive, null, &info, 0));
        Assert.Equal(2u, info.uIdleTimeInterval);

        // Now check that we can get the object out
        mock2.Setup(m => m.FQueryTerminate(true)).Returns(true);
        using ComScope<IMsoComponent> component = new(null);
        Assert.True(manager.FGetActiveComponent(msogac.Tracking, component, &info, 0));
        Assert.False(component.IsNull);
        Assert.True(component.Value->FQueryTerminate(true));
    }

    private class MockWrapper : IMsoComponent.Interface, IManagedWrapper<IMsoComponent>
    {
        private readonly IMsoComponent.Interface _mock;
        public MockWrapper(IMsoComponent.Interface mock) => _mock = mock;

        BOOL IMsoComponent.Interface.FDebugMessage(nint hInst, uint msg, WPARAM wParam, LPARAM lParam)
            => _mock.FDebugMessage(hInst, msg, wParam, lParam);

        BOOL IMsoComponent.Interface.FPreTranslateMessage(MSG* msg)
            => _mock.FPreTranslateMessage(msg);

        void IMsoComponent.Interface.OnEnterState(msocstate uStateID, BOOL fEnter)
            => _mock.OnEnterState(uStateID, fEnter);

        void IMsoComponent.Interface.OnAppActivate(BOOL fActive, uint dwOtherThreadID)
            => _mock.OnAppActivate(fActive, dwOtherThreadID);

        void IMsoComponent.Interface.OnLoseActivation() => _mock.OnLoseActivation();

        void IMsoComponent.Interface.OnActivationChange(
            IMsoComponent* pic,
            BOOL fSameComponent,
            MSOCRINFO* pcrinfo,
            BOOL fHostIsActivating,
            nint pchostinfo,
            uint dwReserved) => _mock.OnActivationChange(pic, fSameComponent, pcrinfo, fHostIsActivating, pchostinfo, dwReserved);

        BOOL IMsoComponent.Interface.FDoIdle(msoidlef grfidlef) => _mock.FDoIdle(grfidlef);

        BOOL IMsoComponent.Interface.FContinueMessageLoop(
            msoloop uReason,
            void* pvLoopData,
            MSG* pMsgPeeked) => _mock.FContinueMessageLoop(uReason, pvLoopData, pMsgPeeked);

        BOOL IMsoComponent.Interface.FQueryTerminate(BOOL fPromptUser) => _mock.FQueryTerminate(fPromptUser);

        void IMsoComponent.Interface.Terminate() => _mock.Terminate();

        HWND IMsoComponent.Interface.HwndGetWindow(msocWindow uWhich, uint dwReserved)
            => _mock.HwndGetWindow(uWhich, dwReserved);
    }
}
