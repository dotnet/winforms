// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Moq;
using Xunit;
using static Interop;
using static Interop.Mso;

namespace System.Windows.Forms.Tests.Interop_Mso
{
    public unsafe class IMsoComponentManagerTests : IClassFixture<ThreadExceptionFixture>
    {
        private IMsoComponentManager CreateComponentManager()
            => (IMsoComponentManager)Activator.CreateInstance(
                typeof(Application).Assembly.GetType("System.Windows.Forms.Application+ComponentManager")!,
                nonPublic: true)!;

        [Fact]
        public void FDebugMessage_ReturnsTrue()
        {
            var manager = CreateComponentManager();
            Assert.Equal(BOOL.TRUE, manager.FDebugMessage(IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero));
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
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = default;
            UIntPtr id = default;

            Assert.Equal(BOOL.FALSE, manager.FRegisterComponent(mock.Object, &info, null));
            Assert.Equal(BOOL.FALSE, manager.FRegisterComponent(mock.Object, null, &id));
            Assert.Equal(UIntPtr.Zero, id);
        }

        [Fact]
        public void FRegisterComponent_RejectsUnsized()
        {
            var manager = CreateComponentManager();
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = default;
            UIntPtr id = default;

            Assert.Equal(BOOL.FALSE, manager.FRegisterComponent(mock.Object, &info, &id));
            Assert.Equal(UIntPtr.Zero, id);
        }

        [Fact]
        public void FRegisterComponent_Cookies()
        {
            var manager = CreateComponentManager();
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;

            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));
            Assert.NotEqual(UIntPtr.Zero, id);

            UIntPtr newId = default;
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &newId));
            Assert.NotEqual(UIntPtr.Zero, newId);

            Assert.NotEqual(id, newId);
        }

        [Fact]
        public void FRevokeComponent()
        {
            var manager = CreateComponentManager();
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;

            Assert.Equal(BOOL.FALSE, manager.FRevokeComponent(UIntPtr.Zero));
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));
            Assert.Equal(BOOL.TRUE, manager.FRevokeComponent(id));
            Assert.Equal(BOOL.FALSE, manager.FRevokeComponent(id));
        }

        [Fact]
        public void FUpdateComponentRegistration_HandlesNull()
        {
            var manager = CreateComponentManager();
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;

            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));
            Assert.Equal(BOOL.FALSE, manager.FUpdateComponentRegistration(id, null));
        }

        [Fact]
        public void FUpdateComponentRegistration()
        {
            var manager = CreateComponentManager();
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;

            Assert.Equal(BOOL.FALSE, manager.FUpdateComponentRegistration(id, &info));
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));
            Assert.Equal(BOOL.TRUE, manager.FUpdateComponentRegistration(id, &info));
        }

        [Fact]
        public void FOnComponentActivate_InvalidId()
        {
            var manager = CreateComponentManager();
            Assert.Equal(BOOL.FALSE, manager.FOnComponentActivate(default));
        }

        [Fact]
        public void FSetTrackingComponent_InvalidId()
        {
            var manager = CreateComponentManager();
            Assert.Equal(BOOL.FALSE, manager.FSetTrackingComponent(default, BOOL.TRUE));
            Assert.Equal(BOOL.FALSE, manager.FSetTrackingComponent(default, BOOL.FALSE));
        }

        [Fact]
        public void FSetTrackingComponent()
        {
            var manager = CreateComponentManager();

            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;

            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));

            Assert.Equal(BOOL.TRUE, manager.FSetTrackingComponent(id, BOOL.TRUE));

            // Returns false if we're already tracking
            Assert.Equal(BOOL.FALSE, manager.FSetTrackingComponent(id, BOOL.TRUE));

            Assert.Equal(BOOL.TRUE, manager.FSetTrackingComponent(id, BOOL.FALSE));

            // If we aren't tracking, untracking should return false
            Assert.Equal(BOOL.FALSE, manager.FSetTrackingComponent(id, BOOL.FALSE));
            Assert.Equal(BOOL.TRUE, manager.FSetTrackingComponent(id, BOOL.TRUE));
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
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            mock.Setup(m => m.OnEnterState(msocstate.Modal, BOOL.TRUE));

            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));

            // No call on "Others"
            manager.OnComponentEnterState(default, msocstate.Modal, msoccontext.Others, 0, null, 0);
            mock.Verify(m => m.OnEnterState(msocstate.Modal, BOOL.TRUE), Times.Never);

            manager.OnComponentEnterState(default, msocstate.Modal, msoccontext.All, 0, null, 0);
            mock.Verify(m => m.OnEnterState(msocstate.Modal, BOOL.TRUE), Times.Once);

            manager.OnComponentEnterState(default, msocstate.Modal, msoccontext.Mine, 0, null, 0);
            mock.Verify(m => m.OnEnterState(msocstate.Modal, BOOL.TRUE), Times.Exactly(2));
        }

        [Fact]
        public void FOnComponentExitState_HandlesNull()
        {
            var manager = CreateComponentManager();
            Assert.Equal(BOOL.FALSE, manager.FOnComponentExitState(default, default, default, default, null));
        }

        [Fact]
        public void FOnComponentExitState_Notification()
        {
            var manager = CreateComponentManager();
            var mock = new Mock<IMsoComponent>(MockBehavior.Strict);
            mock.Setup(m => m.OnEnterState(msocstate.Modal, BOOL.FALSE));

            MSOCRINFO info = new MSOCRINFO { cbSize = (uint)sizeof(MSOCRINFO) };
            UIntPtr id = default;
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock.Object, &info, &id));

            // No call on "Others"
            manager.FOnComponentExitState(default, msocstate.Modal, msoccontext.Others, 0, null);
            mock.Verify(m => m.OnEnterState(msocstate.Modal, BOOL.FALSE), Times.Never);

            manager.FOnComponentExitState(default, msocstate.Modal, msoccontext.All, 0, null);
            mock.Verify(m => m.OnEnterState(msocstate.Modal, BOOL.FALSE), Times.Once);

            manager.FOnComponentExitState(default, msocstate.Modal, msoccontext.Mine, 0, null);
            mock.Verify(m => m.OnEnterState(msocstate.Modal, BOOL.FALSE), Times.Exactly(2));
        }

        [Fact]
        public void FInState()
        {
            var manager = CreateComponentManager();
            Assert.Equal(BOOL.TRUE, manager.FInState(0, null));
            manager.OnComponentEnterState(default, msocstate.Modal, default, 0, null, 0);
            Assert.Equal(BOOL.FALSE, manager.FInState(0, null));
            Assert.Equal(BOOL.TRUE, manager.FInState(msocstate.Modal, null));
            manager.OnComponentEnterState(default, msocstate.Recording, default, 0, null, 0);
            Assert.Equal(BOOL.TRUE, manager.FInState(msocstate.Recording, null));
            manager.FOnComponentExitState(default, msocstate.RedrawOff, default, 0, null);
            Assert.Equal(BOOL.TRUE, manager.FInState(0, null));
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
            Assert.Equal(BOOL.FALSE, manager.FPushMessageLoop(default, default, null));
        }

        [Fact]
        public void FCreateSubComponentManager_HandlesNull()
        {
            var manager = CreateComponentManager();

            // Shouldn't try and deref a null
            Assert.Equal(BOOL.FALSE, manager.FCreateSubComponentManager(default, default, null, null));

            // Should null out obj pointer
            void* obj = (void*)0xDEADBEEF;
            Assert.Equal(BOOL.FALSE, manager.FCreateSubComponentManager(default, default, null, &obj));
            Assert.True(obj is null);
        }

        [Fact]
        public void FGetParentComponentManager_HandlesNull()
        {
            var manager = CreateComponentManager();

            // Shouldn't try and deref a null
            Assert.Equal(BOOL.FALSE, manager.FGetParentComponentManager(null));

            // Should null out obj pointer
            void* obj = (void*)0xDEADBEEF;
            Assert.Equal(BOOL.FALSE, manager.FGetParentComponentManager(&obj));
            Assert.True(obj is null);
        }

        [Fact]
        public void FGetActiveComponent()
        {
            var manager = CreateComponentManager();
            Assert.Equal(BOOL.FALSE, manager.FGetActiveComponent(msogac.Active, null, null, 0));

            var mock1 = new Mock<IMsoComponent>(MockBehavior.Strict);
            var mock2 = new Mock<IMsoComponent>(MockBehavior.Strict);

            MSOCRINFO info = new MSOCRINFO
            {
                cbSize = (uint)sizeof(MSOCRINFO),
                uIdleTimeInterval = 1
            };

            UIntPtr firstId = default;
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock1.Object, &info, &firstId));
            info.uIdleTimeInterval = 2;
            UIntPtr secondId = default;
            Assert.Equal(BOOL.TRUE, manager.FRegisterComponent(mock2.Object, &info, &secondId));

            Assert.Equal(BOOL.FALSE, manager.FGetActiveComponent(msogac.Active, null, null, 0));

            // Just an active component
            Assert.Equal(BOOL.TRUE, manager.FOnComponentActivate(firstId));
            Assert.Equal(BOOL.FALSE, manager.FGetActiveComponent(msogac.Tracking, null, &info, 0));
            Assert.Equal(BOOL.TRUE, manager.FGetActiveComponent(msogac.Active, null, &info, 0));
            Assert.Equal(1u, info.uIdleTimeInterval);
            Assert.Equal(BOOL.TRUE, manager.FGetActiveComponent(msogac.TrackingOrActive, null, &info, 0));
            Assert.Equal(1u, info.uIdleTimeInterval);

            // Active and tracking
            Assert.Equal(BOOL.TRUE, manager.FSetTrackingComponent(secondId, BOOL.TRUE));
            Assert.Equal(BOOL.TRUE, manager.FGetActiveComponent(msogac.Tracking, null, &info, 0));
            Assert.Equal(2u, info.uIdleTimeInterval);
            Assert.Equal(BOOL.TRUE, manager.FGetActiveComponent(msogac.Active, null, &info, 0));
            Assert.Equal(1u, info.uIdleTimeInterval);
            Assert.Equal(BOOL.TRUE, manager.FGetActiveComponent(msogac.TrackingOrActive, null, &info, 0));
            Assert.Equal(2u, info.uIdleTimeInterval);

            // Now check that we can get the object out
            mock2.Setup(m => m.FQueryTerminate(BOOL.TRUE)).Returns(BOOL.TRUE);
            void* pUnk = default;
            Assert.Equal(BOOL.TRUE, manager.FGetActiveComponent(msogac.Tracking, &pUnk, &info, 0));
            Assert.True(pUnk != null);
            try
            {
                var component = (IMsoComponent)Marshal.GetObjectForIUnknown((IntPtr)pUnk);
                Assert.Equal(BOOL.TRUE, component.FQueryTerminate(BOOL.TRUE));
            }
            finally
            {
                Marshal.Release((IntPtr)pUnk);
            }
        }
    }
}
