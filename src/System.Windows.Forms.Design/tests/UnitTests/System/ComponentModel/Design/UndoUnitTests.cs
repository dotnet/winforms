// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design.Serialization;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class UndoUnitTests : UndoEngine, IClassFixture<ThreadExceptionFixture>
    {
        public UndoUnitTests() : base(GetServiceProvider())
        {
        }

        private static IServiceProvider GetServiceProvider()
        {
            var mockServiceProvider = new Mock<IServiceProvider>();
            var mockDesignerHost = new Mock<IDesignerHost>();
            var mockComponentChangeService = new Mock<IComponentChangeService>();
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(IComponentChangeService)))
                .Returns(mockComponentChangeService.Object);
            mockServiceProvider
                .Setup(p => p.GetService(typeof(ComponentSerializationService)))
                .Returns(new object());
            return mockServiceProvider.Object;
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void UndoUnit_Ctor_UndoEngine_String(string name, string expectedName)
        {
            var unit = new SubUndoUnit(this, name);
            Assert.Same(this, unit.UndoEngine);
            Assert.Equal(expectedName, unit.Name);
            Assert.True(unit.IsEmpty);
        }

        [Fact]
        public void UndoUnit_NullEngine_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("engine", () => new UndoUnit(null, "name"));
        }

        protected override void AddUndoUnit(UndoUnit unit)
        {
        }

        protected class SubUndoUnit : UndoUnit
        {
            public SubUndoUnit(UndoEngine engine, string name) : base(engine, name)
            {
            }

            public new UndoEngine UndoEngine => base.UndoEngine;
        }
    }
}
