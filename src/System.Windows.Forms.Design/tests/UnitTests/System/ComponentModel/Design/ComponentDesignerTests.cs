// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Windows.Forms;
using Moq;
using Moq.Protected;
using WinForms.Common.Tests;
using Xunit;

namespace System.ComponentModel.Design.Tests
{
    public class ComponentDesignerTests
    {
        [Fact]
        public void ComponentDesigner_Ctor_Default()
        {
            var designer = new SubComponentDesigner();
            Assert.Empty(designer.ActionLists);
            Assert.Same(designer.ActionLists, designer.ActionLists);
            Assert.Empty(designer.AssociatedComponents);
            Assert.Null(designer.Component);
            Assert.Same(InheritanceAttribute.Default, designer.InheritanceAttribute);
            Assert.Same(designer.InheritanceAttribute, designer.InheritanceAttribute);
            Assert.False(designer.Inherited);
            Assert.Null(designer.ParentComponent);
            Assert.NotNull(designer.ShadowProperties);
            Assert.Same(designer.ShadowProperties, designer.ShadowProperties);
            Assert.Empty(designer.Verbs);
            Assert.Same(designer.Verbs, designer.Verbs);
        }

        [Fact]
        public void ComponentDesigner_Children_GetDefault_ReturnsExpected()
        {
            var designer = new ComponentDesigner();
            ITreeDesigner treeDesigner = designer;
            Assert.Empty(treeDesigner.Children);
        }

        [Fact]
        public void ComponentDesigner_Children_GetWithValidHostValidResult_ReturnsExpected()
        {
            var mockComponent1 = new Mock<IComponent>(MockBehavior.Strict);
            var mockComponent2 = new Mock<IComponent>(MockBehavior.Strict);
            var mockDesigner1 = new Mock<IDesigner>(MockBehavior.Strict);
            var mockDesigner2 = new Mock<IDesigner>(MockBehavior.Strict);
            var designer = new CustomAssociatedComponentsComponentDesigner(new object[] { mockComponent1.Object, mockComponent2.Object });
            ITreeDesigner treeDesigner = designer;
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.GetDesigner(mockComponent1.Object))
                .Returns(mockDesigner1.Object)
                .Verifiable();
            mockDesignerHost
                .Setup(h => h.GetDesigner(mockComponent2.Object))
                .Returns(mockDesigner2.Object)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            Assert.Equal(new object[] { mockDesigner1.Object, mockDesigner2.Object }, treeDesigner.Children);
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent1.Object), Times.Once());
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent2.Object), Times.Once());

            // Call again to test caching behavior.
            Assert.Equal(new object[] { mockDesigner1.Object, mockDesigner2.Object }, treeDesigner.Children);
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent1.Object), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent2.Object), Times.Exactly(2));
        }

        [Fact]
        public void ComponentDesigner_Children_GetWithValidHostInvalidResult_ReturnsExpected()
        {
            var mockComponent1 = new Mock<IComponent>(MockBehavior.Strict);
            var mockComponent2 = new Mock<IComponent>(MockBehavior.Strict);
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            var designer = new CustomAssociatedComponentsComponentDesigner(new object[] { new object(), null, mockComponent1.Object, mockComponent2.Object });
            ITreeDesigner treeDesigner = designer;
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.GetDesigner(mockComponent1.Object))
                .Returns(mockDesigner.Object)
                .Verifiable();
            mockDesignerHost
                .Setup(h => h.GetDesigner(mockComponent2.Object))
                .Returns<IDesigner>(null)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            Assert.Equal(new object[] { mockDesigner.Object }, treeDesigner.Children);
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent1.Object), Times.Once());
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent2.Object), Times.Once());

            // Call again to test caching behavior.
            Assert.Equal(new object[] { mockDesigner.Object }, treeDesigner.Children);
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent1.Object), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.GetDesigner(mockComponent2.Object), Times.Exactly(2));
        }

        public static IEnumerable<object[]> Children_GetInvalidService_TestData()
        {
            foreach (ICollection associatedComponents in new object[] { null, new object[0], new object[] { new Component() }})
            {
                yield return new object[] { associatedComponents, null };
                yield return new object[] { associatedComponents, new object() };
            }
        }

        [Theory]
        [MemberData(nameof(Children_GetInvalidService_TestData))]
        public void ComponentDesigner_Children_GetWithInvalidDesignerHost_ReturnsEmpty(ICollection associatedComponents, object host)
        {
            var designer = new CustomAssociatedComponentsComponentDesigner(associatedComponents);
            ITreeDesigner treeDesigner = designer;
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(host);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            Assert.Empty(treeDesigner.Children);
        }

        public static IEnumerable<object[]> InheritanceAttribute_GetValidService_TestData()
        {
            yield return new object[] { null, 3, 5 };
            yield return new object[] { new InheritanceAttribute(), 1, 1 };
        }

        [Theory]
        [MemberData(nameof(InheritanceAttribute_GetValidService_TestData))]
        public void ComponentDesigner_InheritanceAttribute_GetWithValidService_ReturnsDefault(InheritanceAttribute attributeResult, int expectedCallCount1, int expectedCallCount2)
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            var mockInheritanceService = new Mock<IInheritanceService>(MockBehavior.Strict);
            mockInheritanceService
                .Setup(s => s.GetInheritanceAttribute(component))
                .Returns(attributeResult)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(mockInheritanceService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IInheritanceService)), Times.Exactly(expectedCallCount1));

            InheritanceAttribute attribute = designer.InheritanceAttribute;
            Assert.Same(attributeResult, attribute);
            Assert.Same(attribute, designer.InheritanceAttribute);
            mockSite.Verify(s => s.GetService(typeof(IInheritanceService)), Times.Exactly(expectedCallCount2));
        }

        public static IEnumerable<object[]> InheritanceAttribute_GetInvalidService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(InheritanceAttribute_GetInvalidService_TestData))]
        public void ComponentDesigner_InheritanceAttribute_GetWithInvalidService_ReturnsDefault(object inheritanceService)
        {
            var designer = new SubComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(inheritanceService)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IInheritanceService)), Times.Once());

            InheritanceAttribute attribute = designer.InheritanceAttribute;
            Assert.Same(InheritanceAttribute.Default, attribute);
            Assert.Same(attribute, designer.InheritanceAttribute);
            mockSite.Verify(s => s.GetService(typeof(IInheritanceService)), Times.Once());
        }

        public static IEnumerable<object[]> Inherited_Get_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { InheritanceAttribute.Default, false };
            yield return new object[] { InheritanceAttribute.Inherited, true };
            yield return new object[] { InheritanceAttribute.InheritedReadOnly, true };
            yield return new object[] { InheritanceAttribute.NotInherited, false };
        }

        [Theory]
        [MemberData(nameof(Inherited_Get_TestData))]
        public void ComponentDesigner_Inherited_Get_ReturnsExpected(InheritanceAttribute inheritanceAttribute, bool expected)
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(inheritanceAttribute);
            Assert.Equal(expected, designer.Inherited);
        }

        public static IEnumerable<object[]> ParentComponent_ValidService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Component() };
        }

        [Theory]
        [MemberData(nameof(ParentComponent_ValidService_TestData))]
        public void ComponentDesigner_ParentComponent_GetWithValidService_ReturnsExpected(Component rootComponent)
        {
            var designer = new SubComponentDesigner();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(rootComponent);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockDesignerHost.Verify(h => h.RootComponent, Times.Once());

            Assert.Same(rootComponent, designer.ParentComponent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(2));

            // Get again.
            Assert.Same(rootComponent, designer.ParentComponent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(3));
        }

        [Fact]
        public void ComponentDesigner_ParentComponent_GetWithValidServiceRootComponentEqual_ReturnsNull()
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(component);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockDesignerHost.Verify(h => h.RootComponent, Times.Once());

            Assert.Null(designer.ParentComponent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(2));

            // Get again.
            Assert.Null(designer.ParentComponent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(3));
        }

        public static IEnumerable<object[]> ParentComponent_InvalidService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(ParentComponent_InvalidService_TestData))]
        public void ComponentDesigner_ParentComponent_GetWithInvalidService_ReturnsNull(object host)
        {
            var designer = new SubComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(host);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());

            Assert.Null(designer.ParentComponent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));

            // Get again.
            Assert.Null(designer.ParentComponent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
        }

        [Theory]
        [MemberData(nameof(ParentComponent_ValidService_TestData))]
        public void ComponentDesigner_ITreeDesignerParent_GetWithValidService_ReturnsExpected(Component rootComponent)
        {
            var designer = new SubComponentDesigner();
            ITreeDesigner treeDesigner = designer;
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(rootComponent);
            mockDesignerHost
                .Setup(h => h.GetDesigner(rootComponent))
                .Returns(designer)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockDesignerHost.Verify(h => h.RootComponent, Times.Once());
            mockDesignerHost.Verify(h => h.GetDesigner(rootComponent), Times.Never());

            Assert.Same(rootComponent == null ? null : designer, treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(rootComponent == null ? 2 : 3));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(2));
            mockDesignerHost.Verify(h => h.GetDesigner(rootComponent), Times.Exactly(rootComponent == null ? 0 : 1));

            // Get again.
            Assert.Same(rootComponent == null ? null : designer, treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(rootComponent == null ? 3 : 5));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(3));
            mockDesignerHost.Verify(h => h.GetDesigner(rootComponent), Times.Exactly(rootComponent == null ? 0 : 2));
        }

        [Fact]
        public void ComponentDesigner_ITreeDesignerParent_GetWithValidServiceRootComponentEqual_ReturnsNull()
        {
            var designer = new ComponentDesigner();
            ITreeDesigner treeDesigner = designer;
            var component = new Component();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(component);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockDesignerHost.Verify(h => h.RootComponent, Times.Once());

            Assert.Null(treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(2));

            // Get again.
            Assert.Null(treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockDesignerHost.Verify(h => h.RootComponent, Times.Exactly(3));
        }

        [Theory]
        [MemberData(nameof(ParentComponent_InvalidService_TestData))]
        public void ComponentDesigner_ITreeDesignerParent_GetWithInvalidServiceFirstCall_ReturnsNull(object host)
        {
            var designer = new SubComponentDesigner();
            ITreeDesigner treeDesigner = designer;
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(host);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());

            Assert.Null(treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));

            // Get again.
            Assert.Null(treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
        }

        [Theory]
        [MemberData(nameof(ParentComponent_InvalidService_TestData))]
        public void ComponentDesigner_ITreeDesignerParent_GetWithInvalidServiceSecondCall_ReturnsNull(object host)
        {
            var designer = new SubComponentDesigner();
            ITreeDesigner treeDesigner = designer;
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(new Component());
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            int callCount = 0;
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount < 3)
                    {
                        return mockDesignerHost.Object;
                    }
                    else
                    {
                        return host;
                    }
                });
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());

            Assert.Null(treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));

            // Get again.
            Assert.Null(treeDesigner.Parent);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(4));
        }

        [Fact]
        public void ComponentDesigner_Dispose_InvokeWithComponent_Success()
        {
            var designer = new ComponentDesigner();
            var mockComponent = new Mock<IComponent>(MockBehavior.Strict);
            mockComponent
                .Setup(c => c.Site)
                .Returns((ISite)null);
            mockComponent
                .Setup(c => c.Dispose())
                .Verifiable();
            designer.Initialize(mockComponent.Object);
            Assert.Same(mockComponent.Object, designer.Component);

            designer.Dispose();
            Assert.Null(designer.Component);
            mockComponent.Verify(c => c.Dispose(), Times.Never());

            // Dispose again.
            designer.Dispose();
            Assert.Null(designer.Component);
            mockComponent.Verify(c => c.Dispose(), Times.Never());
        }

        public static IEnumerable<object[]> Dispose_InvokeWithComponentChangeService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new Mock<IComponentChangeService>(MockBehavior.Strict).Object };
        }

        [Theory]
        [MemberData(nameof(Dispose_InvokeWithComponentChangeService_TestData))]
        public void ComponentDesigner_Dispose_InvokeWithComponentChangeService_Success(object componentChangeService)
        {
            var designer = new ComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(componentChangeService);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            Assert.Same(component, designer.Component);

            designer.Dispose();
            Assert.Null(designer.Component);

            // Dispose again.
            designer.Dispose();
            Assert.Null(designer.Component);
        }

        [Fact]
        public void ComponentDesigner_Dispose_InvokeWithoutComponent_Success()
        {
            var designer = new ComponentDesigner();
            designer.Dispose();
            Assert.Null(designer.Component);

            // Dispose again.
            designer.Dispose();
            Assert.Null(designer.Component);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentDesigner_Dispose_InvokeBoolWithComponent_Success(bool disposing)
        {
            var designer = new SubComponentDesigner();
            var mockComponent = new Mock<IComponent>(MockBehavior.Strict);
            mockComponent
                .Setup(c => c.Site)
                .Returns((ISite)null);
            mockComponent
                .Setup(c => c.Dispose())
                .Verifiable();
            designer.Initialize(mockComponent.Object);
            Assert.Same(mockComponent.Object, designer.Component);

            designer.Dispose(disposing);
            Assert.Same(disposing ? null : mockComponent.Object, designer.Component);
            mockComponent.Verify(c => c.Dispose(), Times.Never());

            // Dispose again.
            designer.Dispose(disposing);
            Assert.Same(disposing ? null : mockComponent.Object, designer.Component);
            mockComponent.Verify(c => c.Dispose(), Times.Never());
        }

        public static IEnumerable<object[]> Dispose_InvokeBoolWithComponentChangeService_TestData()
        {
            foreach (bool disposing in new bool[] { true, false })
            {
                yield return new object[] { null, disposing };
                yield return new object[] { new object(), disposing };
                yield return new object[] { new Mock<IComponentChangeService>(MockBehavior.Strict), disposing };
            }
        }

        [Theory]
        [MemberData(nameof(Dispose_InvokeBoolWithComponentChangeService_TestData))]
        public void ComponentDesigner_Dispose_InvokeBoolWithComponentChangeService_Success(object componentChangeService, bool disposing)
        {
            var designer = new SubComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(componentChangeService);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);
            Assert.Same(component, designer.Component);

            designer.Dispose(disposing);
            Assert.Same(disposing ? null : component, designer.Component);

            // Dispose again.
            designer.Dispose(disposing);
            Assert.Same(disposing ? null : component, designer.Component);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ComponentDesigner_Dispose_InvokeBoolWithoutComponent_Success(bool disposing)
        {
            var designer = new SubComponentDesigner();
            designer.Dispose(disposing);
            Assert.Null(designer.Component);

            // Dispose again.
            designer.Dispose(disposing);
            Assert.Null(designer.Component);
        }

        [Fact]
        public void ComponentDesigner_DoDefaultAction_InvokeDefault_Nop()
        {
            var designer = new ComponentDesigner();
            designer.DoDefaultAction();

            // Call again.
            designer.DoDefaultAction();
        }

        public static IEnumerable<object[]> DoDefaultAction_ValidProperty_TestData()
        {
            foreach (string property in new string[] { nameof(DefaultEventComponent.StringProperty) })
            {
                yield return new object[] { property, null, 1, null };
                yield return new object[] { property, null, 1, string.Empty };
                yield return new object[] { property, null, 1, "UniqueMethod" };

                yield return new object[] { property, new object[0], 1, null };
                yield return new object[] { property, new object[0], 1, string.Empty };
                yield return new object[] { property, new object[0], 1, "UniqueMethod" };

                yield return new object[] { property, new object[] { null, new object(), "NoSuchStringValue" }, 1, null };
                yield return new object[] { property, new object[] { null, new object(), "NoSuchStringValue" }, 1, string.Empty };
                yield return new object[] { property, new object[] { null, new object(), "NoSuchStringValue" }, 1, "UniqueMethod" };

                // Should not call if in the compatible methods list.
                yield return new object[] { property, new object[] { "StringValue" }, 0, null };
                yield return new object[] { property, new object[] { "StringValue" }, 0, string.Empty };
                yield return new object[] { property, new object[] { "StringValue" }, 0, "UniqueMethod" };
            }
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_ValidProperty_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeWithComponentWithHostValidProperty_Success(string property, ICollection compatibleMethods, int expectedSetCallCount, string uniqueMethodName)
        {
            var designer = new ComponentDesigner();
            var component1 = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var component2 = new DefaultEventComponent
            {
                StringProperty = string.Empty
            };
            var component3 = new DefaultEventComponent
            {
                StringProperty = null
            };
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[property])
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.CreateUniqueMethodName(component3, It.IsAny<EventDescriptor>()))
                .Returns(uniqueMethodName)
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()))
                .Returns(compatibleMethods)
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component1, component2, component3 });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Returns<DesignerTransaction>(null)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component1.StringPropertySetCount = 0;
            component2.StringPropertySetCount = 0;
            component3.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(3));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(3));
            mockEventBindingService.Verify(s => s.CreateUniqueMethodName(component3, It.IsAny<EventDescriptor>()), Times.Once());
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            Assert.Equal("StringValue", component1.StringProperty);
            Assert.Equal(expectedSetCallCount, component1.StringPropertySetCount);
            Assert.Empty(component2.StringProperty);
            Assert.Equal(1, component2.StringPropertySetCount);
            Assert.Equal(uniqueMethodName, component3.StringProperty);
            Assert.Equal(1, component3.StringPropertySetCount);

            // Call again.
            component1.StringProperty = "StringValue";
            component2.StringProperty = string.Empty;
            component3.StringProperty = null;
            component1.StringPropertySetCount = 0;
            component2.StringPropertySetCount = 0;
            component3.StringPropertySetCount = 0;
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(6));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(6));
            mockEventBindingService.Verify(s => s.CreateUniqueMethodName(component3, It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(4));
            Assert.Equal("StringValue", component1.StringProperty);
            Assert.Equal(expectedSetCallCount, component1.StringPropertySetCount);
            Assert.Empty(component2.StringProperty);
            Assert.Equal(1, component2.StringPropertySetCount);
            Assert.Equal(uniqueMethodName, component3.StringProperty);
            Assert.Equal(1, component3.StringPropertySetCount);
        }

        [Theory]
        [InlineData(null, null, 0)]
        [InlineData(null, "", 1)]
        [InlineData(null, "name", 1)]
        [InlineData("", "", 1)]
        [InlineData("name", "name", 1)]
        public void ComponentDesigner_DoDefaultAction_InvokeWithRootComponent_CallsShowCode(string value, string uniqueName, int expectedCallCount)
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = value
            };
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()))
                .Returns(new object[0]);
            mockEventBindingService
                .Setup(s => s.CreateUniqueMethodName(component, It.IsAny<EventDescriptor>()))
                .Returns(uniqueName);
            mockEventBindingService
                .Setup(s => s.ShowCode(component, It.IsAny<EventDescriptor>()))
                .Returns(false)
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Returns<DesignerTransaction>(null)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.Name)
                .Returns("Name");
            component.Site = mockSite.Object;
            designer.Initialize(component);
            component.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            mockEventBindingService.Verify(s => s.ShowCode(component, It.IsAny<EventDescriptor>()), Times.Exactly(expectedCallCount));
            Assert.Equal(uniqueName, component.StringProperty);
            Assert.Equal(1, component.StringPropertySetCount);

            // Call again.
            component.StringPropertySetCount = 0;
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.ShowCode(component, It.IsAny<EventDescriptor>()), Times.Exactly(expectedCallCount * 2));
            Assert.Equal(uniqueName, component.StringProperty);
            Assert.Equal(1, component.StringPropertySetCount);
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_ValidProperty_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeWithComponentWithHostValidPropertyWithTransaction_Success(string property, ICollection compatibleMethods, int expectedSetCallCount, string uniqueMethodName)
        {
            var designer = new ComponentDesigner();
            var component1 = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var component2 = new DefaultEventComponent
            {
                StringProperty = string.Empty
            };
            var component3 = new DefaultEventComponent
            {
                StringProperty = null
            };
            var mockTransaction = new Mock<DesignerTransaction>(MockBehavior.Strict);
            mockTransaction
                .Protected()
                .Setup("OnCommit")
                .Verifiable();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[property])
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.CreateUniqueMethodName(component3, It.IsAny<EventDescriptor>()))
                .Returns(uniqueMethodName)
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()))
                .Returns(compatibleMethods)
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { null, new object(), component1, component2, component3 });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Returns(mockTransaction.Object)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component1.StringPropertySetCount = 0;
            component2.StringPropertySetCount = 0;
            component3.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(3));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            mockEventBindingService.Verify(s => s.CreateUniqueMethodName(component3, It.IsAny<EventDescriptor>()), Times.Once());
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockTransaction.Protected().Verify("OnCommit", Times.Once());
            Assert.Equal("StringValue", component1.StringProperty);
            Assert.Equal(expectedSetCallCount, component1.StringPropertySetCount);
            Assert.Empty(component2.StringProperty);
            Assert.Equal(1, component2.StringPropertySetCount);
            Assert.Equal(uniqueMethodName, component3.StringProperty);
            Assert.Equal(1, component3.StringPropertySetCount);

            // Call again.
            component1.StringProperty = "StringValue";
            component2.StringProperty = string.Empty;
            component3.StringProperty = null;
            component1.StringPropertySetCount = 0;
            component2.StringPropertySetCount = 0;
            component3.StringPropertySetCount = 0;
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(6));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.CreateUniqueMethodName(component3, It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(4));
            mockTransaction.Protected().Verify("OnCommit", Times.Once());
            Assert.Equal("StringValue", component1.StringProperty);
            Assert.Equal(expectedSetCallCount, component1.StringPropertySetCount);
            Assert.Empty(component2.StringProperty);
            Assert.Equal(1, component2.StringPropertySetCount);
            Assert.Equal(uniqueMethodName, component3.StringProperty);
            Assert.Equal(1, component3.StringPropertySetCount);
        }

        [Fact]
        public void ComponentDesigner_DoDefaultAction_InvokeWithCanceledThrowingCreateTransaction_Success()
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component, new DefaultEventComponent() });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Throws(CheckoutException.Canceled)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);
        }

        public static IEnumerable<object[]> DoDefaultAction_NotCanceledException_TestData()
        {
            yield return new object[] { new CheckoutException() };
            yield return new object[] { new DivideByZeroException() };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_NotCanceledException_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeWithNotCanceledThrowingCreateTransaction_Rethrows(Exception exception)
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component, new DefaultEventComponent() });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Throws(exception)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            Assert.Throws(exception.GetType(), () => designer.DoDefaultAction());
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);

            // Call again.
            Assert.Throws(exception.GetType(), () => designer.DoDefaultAction());
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);
        }

        [Fact]
        public void ComponentDesigner_DoDefaultAction_InvokeWithInvalidOperationExceptionThrowingCreateTransaction_Catches()
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var mockTransaction = new Mock<DesignerTransaction>(MockBehavior.Strict);
            mockTransaction
                .Protected()
                .Setup("OnCancel")
                .Verifiable();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component, new DefaultEventComponent() });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Throws(new InvalidOperationException())
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);
        }

        [Fact]
        public void ComponentDesigner_DoDefaultAction_InvokeThrowsInvalidOperationException_CallsCancel()
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var mockTransaction = new Mock<DesignerTransaction>(MockBehavior.Strict);
            mockTransaction
                .Protected()
                .Setup("OnCancel")
                .Verifiable();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()))
                .Throws(new InvalidOperationException())
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component, new DefaultEventComponent() });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Returns(mockTransaction.Object)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Once());
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            mockTransaction.Protected().Verify("OnCancel", Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            mockTransaction.Protected().Verify("OnCancel", Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);
        }

        public static IEnumerable<object[]> DoDefaultAction_NonInvalidOperationException_TestData()
        {
            yield return new object[] { new CheckoutException() };
            yield return new object[] { CheckoutException.Canceled };
            yield return new object[] { new DivideByZeroException() };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_NonInvalidOperationException_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeThrowsNonInvalidOperationException_CallsCommit(Exception exception)
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var mockTransaction = new Mock<DesignerTransaction>(MockBehavior.Strict);
            mockTransaction
                .Protected()
                .Setup("OnCommit")
                .Verifiable();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()))
                .Throws(exception)
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component, new DefaultEventComponent() });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Returns(mockTransaction.Object)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            Assert.Throws(exception.GetType(), () => designer.DoDefaultAction());
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Once());
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Once());
            mockTransaction.Protected().Verify("OnCommit", Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);

            // Call again.
            Assert.Throws(exception.GetType(), () => designer.DoDefaultAction());
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockDesignerHost.Verify(h => h.CreateTransaction(It.IsAny<string>()), Times.Exactly(2));
            mockTransaction.Protected().Verify("OnCommit", Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);
        }

        public static IEnumerable<object[]> DoDefaultAction_InvalidProperty_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.ReadOnlyProperty)] };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.IntProperty)] };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_InvalidProperty_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeWithComponentWithHostInvalidProperty_Success(PropertyDescriptor property)
        {
            var designer = new ComponentDesigner();
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(property)
                .Verifiable();
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component });
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            mockDesignerHost
                .Setup(h => h.CreateTransaction(It.IsAny<string>()))
                .Returns<DesignerTransaction>(null)
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(0, component.StringPropertySetCount);
        }

        public static IEnumerable<object[]> DoDefaultAction_InvalidSelectedComponents_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object[0] };
            yield return new object[] { new object[] { null, new object() } };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_InvalidSelectedComponents_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeWithComponentWithHostInvalidSelectedComponents_Success(ICollection selectedComponents)
        {
            var designer = new ComponentDesigner();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(selectedComponents);
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns<IComponent>(null);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
        }

        public static IEnumerable<object[]> DoDefaultAction_InvalidEventBindingService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_InvalidEventBindingService_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeInvalidEventBindingService_Nop(object eventBindingService)
        {
            var designer = new ComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(eventBindingService)
                .Verifiable();
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
        }

        public static IEnumerable<object[]> DoDefaultAction_InvalidSelectionService_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_InvalidSelectionService_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeInvalidSelectionService_Nop(object selectionService)
        {
            var designer = new ComponentDesigner();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(selectionService)
                .Verifiable();
            var component = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(component);

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());

            // Call again.
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
        }

        public static IEnumerable<object[]> DoDefaultAction_InvalidDesignerHost_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
        }

        [Theory]
        [MemberData(nameof(DoDefaultAction_InvalidDesignerHost_TestData))]
        public void ComponentDesigner_DoDefaultAction_InvokeInvalidDesignerHost_Success(object designerHost)
        {
            var component = new DefaultEventComponent
            {
                StringProperty = "StringValue"
            };
            var designer = new ComponentDesigner();
            var mockEventBindingService = new Mock<IEventBindingService>(MockBehavior.Strict);
            var mockSelectionService = new Mock<ISelectionService>(MockBehavior.Strict);
            mockSelectionService
                .Setup(s => s.GetSelectedComponents())
                .Returns(new object[] { component });
            mockEventBindingService
                .Setup(s => s.GetEventProperty(It.IsAny<EventDescriptor>()))
                .Returns(TypeDescriptor.GetProperties(typeof(DefaultEventComponent))[nameof(DefaultEventComponent.StringProperty)])
                .Verifiable();
            mockEventBindingService
                .Setup(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()))
                .Returns(new object[0])
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(designerHost);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IEventBindingService)))
                .Returns(mockEventBindingService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object)
                .Verifiable();
            var rootComponent = new Component
            {
                Site = mockSite.Object
            };
            designer.Initialize(rootComponent);
            component.StringPropertySetCount = 0;

            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(2));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Once());
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Once());
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Once());
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(1, component.StringPropertySetCount);

            // Call again.
            component.StringPropertySetCount = 0;
            designer.DoDefaultAction();
            mockSite.Verify(s => s.GetService(typeof(IEventBindingService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(ISelectionService)), Times.Exactly(2));
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Exactly(3));
            mockSelectionService.Verify(s => s.GetSelectedComponents(), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetEventProperty(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            mockEventBindingService.Verify(s => s.GetCompatibleMethods(It.IsAny<EventDescriptor>()), Times.Exactly(2));
            Assert.Equal("StringValue", component.StringProperty);
            Assert.Equal(1, component.StringPropertySetCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void ComponentDesigner_GetService_InvokeWithComponentWithSite_ReturnsNull(Type serviceType)
        {
            var service = new object();
            var designer = new SubComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(serviceType))
                .Returns(service)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            var component = new Component
            {
                Site = mockSite.Object
            };

            designer.Initialize(component);
            Assert.Same(service, designer.GetService(serviceType));
            mockSite.Verify(s => s.GetService(serviceType), Times.Once());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void ComponentDesigner_GetService_InvokeWithComponentWithoutSite_ReturnsNull(Type serviceType)
        {
            var designer = new SubComponentDesigner();
            designer.Initialize(new Component());
            Assert.Null(designer.GetService(serviceType));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetTypeWithNullTheoryData))]
        public void ComponentDesigner_GetService_InvokeWithoutComponent_ReturnsNull(Type serviceType)
        {
            var designer = new SubComponentDesigner();
            Assert.Null(designer.GetService(serviceType));
        }

        [Fact]
        public void ComponentDesigner_Initialize_Invoke_Success()
        {
            var designer = new ComponentDesigner();
            var component = new Component();
            designer.Initialize(component);
            Assert.Same(component, designer.Component);
            Assert.Empty(designer.AssociatedComponents);

            // Override with null.
            designer.Initialize(null);
            Assert.Null(designer.Component);
            Assert.Empty(designer.AssociatedComponents);
        }

        [Fact]
        public void ComponentDesigner_Initialize_RootComponent_Success()
        {
            var component = new Component();
            var designer = new ComponentDesigner();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(component);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null)
                .Verifiable();
            component.Site = mockSite.Object;

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IInheritanceService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IDictionaryService)), Times.Exactly(4));
            mockSite.Verify(s => s.GetService(typeof(IExtenderListService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(ITypeDescriptorFilterService)), Times.Once());
        }

        public static IEnumerable<object[]> Initialize_NonRootComponent_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new object(), new object() };

            var mockNullDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockNullDesignerHost
                .Setup(h => h.RootComponent)
                .Returns((IComponent)null);
            yield return new object[] { mockNullDesignerHost.Object, null };

            var mockNonNullDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockNonNullDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(new Component());
            yield return new object[] { mockNonNullDesignerHost.Object, null };
        }

        [Theory]
        [MemberData(nameof(Initialize_NonRootComponent_TestData))]
        public void ComponentDesigner_Initialize_NonRootComponent_Success(object host, object componentChangeService)
        {
            var designer = new ComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(host);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(componentChangeService);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(DesignerCommandSet)))
                .Returns(null)
                .Verifiable();
            var component = new Component
            {
                Site = mockSite.Object
            };

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(DesignerCommandSet)), Times.Never());
        }

        [Fact]
        public void ComponentDesigner_Initialize_NullInheritanceAttribute_Success()
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(null);
            var component = new Component();
            designer.Initialize(component);
            Assert.Same(component, designer.Component);
            Assert.Empty(designer.AssociatedComponents);
        }

        [Fact]
        public void ComponentDesigner_Initialize_InvokeIServiceContainerSiteWithNullDesignerCommandSet_CallsAddService()
        {
            var designer = new ComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(DesignerCommandSet)))
                .Returns(null)
                .Verifiable();
            DesignerCommandSet set = null;
            mockSite
                .As<IServiceContainer>()
                .Setup(c => c.AddService(typeof(DesignerCommandSet), It.IsAny<DesignerCommandSet>()))
                .Callback<Type, object>((t, s) => set = Assert.IsAssignableFrom<DesignerCommandSet>(s))
                .Verifiable();
            var component = new Component
            {
                Site = mockSite.Object
            };

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(DesignerCommandSet)), Times.Once());
            mockSite.As<IServiceContainer>().Verify(s => s.AddService(typeof(DesignerCommandSet), set), Times.Once());
            Assert.Same(designer.ActionLists, set.GetCommands("ActionLists"));
            Assert.Same(designer.Verbs, set.GetCommands("Verbs"));
            Assert.Null(set.GetCommands("Other"));
            Assert.Null(set.GetCommands(string.Empty));
            Assert.Null(set.GetCommands(null));
        }

        public static IEnumerable<object[]> Initialize_NonNullDesignerCommandSet_TestData()
        {
            yield return new object[] { new object() };
            yield return new object[] { new DesignerCommandSet() };
        }

        [Theory]
        [MemberData(nameof(Initialize_NonNullDesignerCommandSet_TestData))]
        public void ComponentDesigner_Initialize_InvokeIServiceContainerSiteWithNonNullDesignerCommandSet_DoesNotCallAddService(object designerCommandSet)
        {
            var designer = new ComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(DesignerCommandSet)))
                .Returns(designerCommandSet)
                .Verifiable();
            mockSite
                .As<IServiceContainer>()
                .Setup(c => c.AddService(typeof(DesignerCommandSet), It.IsAny<DesignerCommandSet>()))
                .Verifiable();
            var component = new Component
            {
                Site = mockSite.Object
            };

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            mockSite.Verify(s => s.GetService(typeof(DesignerCommandSet)), Times.Once());
            mockSite.As<IServiceContainer>().Verify(s => s.AddService(typeof(DesignerCommandSet), It.IsAny<DesignerCommandSet>()), Times.Never());
        }

        [Fact]
        public void ComponentDesigner_ParentComponent_GetWithHost_ReturnsExpected()
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            designer.Initialize(component);
        }

        public static IEnumerable<object[]> IDictionary_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Dictionary<int, int>() };
        }

        [Theory]
        [MemberData(nameof(IDictionary_TestData))]
        public void ComponentDesigner_InitializeExistingComponent_Invoke_ThrowsNotImplementedException(IDictionary defaultValues)
        {
            var designer = new ComponentDesigner();
            Assert.Throws<NotImplementedException>(() => designer.InitializeExistingComponent(defaultValues));
        }

        [Theory]
        [MemberData(nameof(IDictionary_TestData))]
        public void ComponentDesigner_InitializeNewComponent_Invoke_Nop(IDictionary defaultValues)
        {
            var designer = new ComponentDesigner();
            designer.InitializeNewComponent(defaultValues);
        }

#pragma warning disable 0618
        [Fact]
        public void ComponentDesigner_InitializeNonDefault_Invoke_Nop()
        {
            var designer = new ComponentDesigner();
            designer.InitializeNonDefault();
        }
#pragma warning restore 0618

        [Fact]
        public void ComponentDesigner_InvokeGetInheritanceAttribute_InvokeNonNullToInvoke_ReturnsExpected()
        {
            var designer = new SubComponentDesigner();
            Assert.Same(designer.InheritanceAttribute, designer.InvokeGetInheritanceAttribute(designer));
        }

        [Fact]
        public void ComponentDesigner_InvokeGetInheritanceAttribute_InvokeNullToInvoke_ReturnsNull()
        {
            var designer = new SubComponentDesigner();
            Assert.Null(designer.InvokeGetInheritanceAttribute(null));
        }

        public static IEnumerable<object[]> PreFilterProperties_ComponentWithoutKey_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Dictionary<string, object>() };
            yield return new object[] { new Dictionary<string, object> { { "SettingsKey", new object() } } };
            yield return new object[] { new Dictionary<string, object> { { "SettingsKey", null } } };
        }

        [Fact]
        public void ComponentDesigner_PreFilterProperties_WithComponentWithKey_Success()
        {
            var designer = new SubComponentDesigner();
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            var properties = new Dictionary<string, PropertyDescriptor>
            {
                { "SettingsKey", descriptor }
            };
            var component = new IPersistComponentSettingsComponent();
            designer.Initialize(component);
            designer.PreFilterProperties(properties);
            PropertyDescriptor result = (PropertyDescriptor)properties["SettingsKey"];
            Assert.NotSame(descriptor, result);
            Assert.Equal(typeof(ComponentDesigner), result.ComponentType);
            Assert.Equal(descriptor.Name, result.Name);
            Assert.Equal(7, descriptor.Attributes.Count);
            Assert.Equal(8, result.Attributes.Count);
        }

        [Fact]
        public void ComponentDesigner_PreFilterProperties_WithIPersistComponentSettingsComponentWithKey_Success()
        {
            var designer = new SubComponentDesigner();
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            var properties = new Dictionary<string, PropertyDescriptor>
            {
                { "SettingsKey", descriptor }
            };
            var component = new IPersistComponentSettingsComponent();
            designer.Initialize(component);
            designer.PreFilterProperties(properties);
            PropertyDescriptor result = (PropertyDescriptor)properties["SettingsKey"];
            Assert.NotSame(descriptor, result);
            Assert.Equal(typeof(ComponentDesigner), result.ComponentType);
            Assert.Equal(descriptor.Name, result.Name);
            Assert.Equal(7, descriptor.Attributes.Count);
            Assert.Equal(8, result.Attributes.Count);
        }

        [Theory]
        [MemberData(nameof(PreFilterProperties_ComponentWithoutKey_TestData))]
        public void ComponentDesigner_PreFilterProperties_WithIPersistComponentSettingsComponentWithoutKey_Success(IDictionary properties)
        {
            var designer = new SubComponentDesigner();
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            object oldValue = properties?["SettingsKey"];
            var component = new IPersistComponentSettingsComponent();
            designer.Initialize(component);
            designer.PreFilterProperties(properties);
            Assert.Same(oldValue, properties?["SettingsKey"]);
        }

        [Fact]
        public void ComponentDesigner_PreFilterProperties_WithNonIPersistComponentSettingsComponent_Nop()
        {
            var designer = new SubComponentDesigner();
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            var properties = new Dictionary<string, PropertyDescriptor>
            {
                { "SettingsKey", descriptor }
            };
            var component = new Component();
            designer.Initialize(component);
            designer.PreFilterProperties(properties);
            Assert.Same(descriptor, properties["SettingsKey"]);
        }

        [Theory]
        [MemberData(nameof(IDictionary_TestData))]
        public void ComponentDesigner_PreFilterProperties_WithoutComponent_Nop(IDictionary properties)
        {
            var designer = new SubComponentDesigner();
            designer.PreFilterProperties(properties);
        }

        public static IEnumerable<object[]> PostFilterAttributes_NoInheritanceAttribute_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { null, new Dictionary<Type, object>(), null };
            yield return new object[] { InheritanceAttribute.Default, null, null };
            yield return new object[] { InheritanceAttribute.Default, new Dictionary<Type, object>(), null };
            yield return new object[] { InheritanceAttribute.Inherited, null, null };
            yield return new object[] { InheritanceAttribute.Inherited, new Dictionary<Type, object>(), InheritanceAttribute.Inherited };
            yield return new object[] { InheritanceAttribute.InheritedReadOnly, null, null };
            yield return new object[] { InheritanceAttribute.InheritedReadOnly, new Dictionary<Type, object>(), InheritanceAttribute.InheritedReadOnly };
            yield return new object[] { InheritanceAttribute.NotInherited, null, null };
            yield return new object[] { InheritanceAttribute.NotInherited, new Dictionary<Type, object>(), null };
        }

        [Theory]
        [MemberData(nameof(PostFilterAttributes_NoInheritanceAttribute_TestData))]
        public void ComponentDesigner_PostFilterAttributes_NoInheritanceAttribute_AddsToAttributes(InheritanceAttribute attribute, IDictionary attributes, object expected)
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(attribute);
            designer.PostFilterAttributes(attributes);
            Assert.Same(expected, attributes?[typeof(InheritanceAttribute)]);
        }

        public static IEnumerable<object[]> PostFilterAttributes_TestData()
        {
            yield return new object[] { null, InheritanceAttribute.Default };
            yield return new object[] { new Dictionary<Type, object>(), InheritanceAttribute.Default };
            yield return new object[] { new Dictionary<Type, object> { { typeof(InheritanceAttribute), null } }, InheritanceAttribute.Default };
            yield return new object[] { new Dictionary<Type, object> { { typeof(InheritanceAttribute), new object() } }, InheritanceAttribute.Default };
            var attribute = new InheritanceAttribute();
            yield return new object[] { new Dictionary<Type, object> { { typeof(InheritanceAttribute), attribute } }, attribute };
        }

        [Theory]
        [MemberData(nameof(PostFilterAttributes_TestData))]
        public void ComponentDesigner_PostFilterAttributes_HasInheritanceAttributeKey_Sets(IDictionary attributes, object expected)
        {
            var designer = new SubComponentDesigner();
            designer.PostFilterAttributes(attributes);
            Assert.Same(expected, designer.InheritanceAttribute);
        }

        public static IEnumerable<object[]> PostFilterEvents_HasEvents_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { InheritanceAttribute.Default, false };
            yield return new object[] { InheritanceAttribute.Inherited, false };
            yield return new object[] { InheritanceAttribute.InheritedReadOnly, true };
            yield return new object[] { InheritanceAttribute.NotInherited, false };
        }

        [Theory]
        [MemberData(nameof(PostFilterEvents_HasEvents_TestData))]
        public void ComponentDesigner_PostFilterEvents_InvokeWithEvents_Success(InheritanceAttribute inheritanceAttribute, bool valid)
        {
            EventDescriptor descriptor = TypeDescriptor.GetEvents(typeof(CustomComponent))[0];
            var events = new Dictionary<object, object> { { "key1", descriptor }, { "Key2", null } };
            var designer = new CustomInheritanceAttributeComponentDesigner(inheritanceAttribute);
            designer.PostFilterEvents(events);
            if (valid)
            {
                EventDescriptor result = Assert.IsAssignableFrom<EventDescriptor>(events["Event"]);
                Assert.Equal(typeof(CustomComponent), result.ComponentType);
                Assert.Equal("Event", result.Name);
                Assert.True(Assert.IsType<ReadOnlyAttribute>(result.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
                Assert.Equal(new Dictionary<object, object> { { "key1", descriptor }, { "Key2", null }, { "Event", result } }, events);
            }
            else
            {
                Assert.Equal(new Dictionary<object, object> { { "key1", descriptor }, { "Key2", null } }, events);
            }
        }

        public static IEnumerable<object[]> PostFilterEvents_NoEvents_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { null, new Dictionary<string, object>(), new Dictionary<string, object>() };
            yield return new object[] { InheritanceAttribute.Default, new Dictionary<string, object>(), new Dictionary<string, object>() };
            yield return new object[] { InheritanceAttribute.Default, new Dictionary<string, object>(), new Dictionary<string, object>() };
            yield return new object[] { InheritanceAttribute.Inherited, null, null };
            yield return new object[] { InheritanceAttribute.Inherited, new Dictionary<string, object>(), new Dictionary<string, object>() };
            yield return new object[] { InheritanceAttribute.InheritedReadOnly, null, null };
            yield return new object[] { InheritanceAttribute.InheritedReadOnly, new Dictionary<string, object>(), new Dictionary<string, object>() };
            yield return new object[] { InheritanceAttribute.NotInherited, null, null };
            yield return new object[] { InheritanceAttribute.NotInherited, new Dictionary<string, object>(), new Dictionary<string, object>() };
        }

        [Theory]
        [MemberData(nameof(PostFilterEvents_NoEvents_TestData))]
        public void ComponentDesigner_PostFilterEvents_InvokeWithoutEvents_Success(InheritanceAttribute inheritanceAttribute, IDictionary events, IDictionary expected)
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(inheritanceAttribute);
            designer.PostFilterEvents(events);
            Assert.Equal(expected, events);
        }

        [Fact]
        public void ComponentDesigner_PostFilterEvents_InvokeWithInvalidEvents_ThrowsArrayTypeMismatchException()
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(InheritanceAttribute.InheritedReadOnly);
            Assert.Throws<ArrayTypeMismatchException>(() => designer.PostFilterEvents(new Dictionary<object, object> { { "key", new object() } } ));
        }

        public static IEnumerable<object[]> RaiseComponentChanged_TestData()
        {
            yield return new object[] { null, null, null };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(CustomComponent))[0], new object(), new object() };
        }

        [Theory]
        [MemberData(nameof(RaiseComponentChanged_TestData))]
        public void ComponentDesigner_RaiseComponentChanged_InvokeWithValidService_CallsOnOnComponentChanged(MemberDescriptor member, object oldValue, object newValue)
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            var mockComponentChangeService = new Mock<IComponentChangeService>(MockBehavior.Strict);
            mockComponentChangeService
                .Setup(s => s.OnComponentChanged(component, member, oldValue, newValue))
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(mockComponentChangeService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            designer.RaiseComponentChanged(member, oldValue, newValue);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Exactly(2));
            mockComponentChangeService.Verify(s => s.OnComponentChanged(component, member, oldValue, newValue), Times.Once());
        }

        public static IEnumerable<object[]> RaiseComponentChanged_InvalidService_TestData()
        {
            foreach (object componentChangeService in new object[] { null, new object() })
            {
                yield return new object[] { componentChangeService, null, null, null };
                yield return new object[] { componentChangeService, TypeDescriptor.GetProperties(typeof(CustomComponent))[0], new object(), new object() };
            }
        }

        [Theory]
        [MemberData(nameof(RaiseComponentChanged_InvalidService_TestData))]
        public void ComponentDesigner_RaiseComponentChanged_InvokeWithInvalidService_CallsOnOnComponentChanged(object componentChangeService, MemberDescriptor member, object oldValue, object newValue)
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(componentChangeService)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            designer.RaiseComponentChanged(member, oldValue, newValue);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(RaiseComponentChanged_TestData))]
        public void ComponentDesigner_RaiseComponentChanged_InvokeWithoutComponent_Nop(MemberDescriptor member, object oldValue, object newValue)
        {
            var designer = new SubComponentDesigner();
            designer.RaiseComponentChanged(member, oldValue, newValue);
        }

        public static IEnumerable<object[]> RaiseComponentChanging_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { TypeDescriptor.GetProperties(typeof(CustomComponent))[0] };
        }

        [Theory]
        [MemberData(nameof(RaiseComponentChanging_TestData))]
        public void ComponentDesigner_RaiseComponentChanging_InvokeWithValidService_CallsOnOnComponentChanged(MemberDescriptor member)
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            var mockComponentChangeService = new Mock<IComponentChangeService>(MockBehavior.Strict);
            mockComponentChangeService
                .Setup(s => s.OnComponentChanging(component, member))
                .Verifiable();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(mockComponentChangeService.Object)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            designer.RaiseComponentChanging(member);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Exactly(2));
            mockComponentChangeService.Verify(s => s.OnComponentChanging(component, member), Times.Once());
        }

        public static IEnumerable<object[]> RaiseComponentChanging_InvalidService_TestData()
        {
            foreach (object componentChangeService in new object[] { null, new object() })
            {
                yield return new object[] { componentChangeService, null };
                yield return new object[] { componentChangeService, TypeDescriptor.GetProperties(typeof(CustomComponent))[0] };
            }
        }

        [Theory]
        [MemberData(nameof(RaiseComponentChanging_InvalidService_TestData))]
        public void ComponentDesigner_RaiseComponentChanging_InvokeWithInvalidService_CallsOnOnComponentChanged(object componentChangeService, MemberDescriptor member)
        {
            var designer = new SubComponentDesigner();
            var component = new Component();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(componentChangeService)
                .Verifiable();
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            component.Site = mockSite.Object;

            designer.Initialize(component);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Once());
            designer.RaiseComponentChanging(member);
            mockSite.Verify(s => s.GetService(typeof(IComponentChangeService)), Times.Exactly(2));
        }

        [Theory]
        [MemberData(nameof(RaiseComponentChanging_TestData))]
        public void ComponentDesigner_RaiseComponentChanging_InvokeWithoutComponent_Nop(MemberDescriptor member)
        {
            var designer = new SubComponentDesigner();
            designer.RaiseComponentChanging(member);
        }

#pragma warning disable 0618
        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, "", "")]
        [InlineData(null, "NewValue", "NewValue")]
        [InlineData("", null, null)]
        [InlineData("", "", "")]
        [InlineData("", "NewValue", "NewValue")]
        [InlineData("OldValue", null, "OldValue")]
        [InlineData("OldValue", "", "OldValue")]
        [InlineData("OldValue", "NewValue", "OldValue")]
        public void ComponentDesigner_OnSetComponentDefaults_InvokeWithComponentWithDefaultProperty_Nop(string oldValue, string siteName, string expectedValue)
        {
            var designer = new ComponentDesigner();
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Name)
                .Returns(siteName);
            var component = new StringDefaultPropertyComponent
            {
                Site = mockSite.Object,
                Value = oldValue
            };
            designer.Initialize(component);
            designer.OnSetComponentDefaults();
            Assert.Equal(expectedValue, component.Value);
        }

        public static IEnumerable<object[]> OnSetComponentDefaults_InvalidComponent_TestData()
        {
            yield return new object[] { new Component() };
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IInheritanceService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IDictionaryService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IExtenderListService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ITypeDescriptorFilterService)))
                .Returns(null);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            yield return new object[] { new Component { Site = mockSite.Object } };
            yield return new object[] { new IntDefaultPropertyComponent { Site = mockSite.Object } };
        }

        [Theory]
        [MemberData(nameof(OnSetComponentDefaults_InvalidComponent_TestData))]
        public void ComponentDesigner_OnSetComponentDefaults_InvokeWithInvalidComponent_Nop(Component component)
        {
            var designer = new ComponentDesigner();
            designer.Initialize(component);
            designer.OnSetComponentDefaults();
        }

        [Fact]
        public void ComponentDesigner_OnSetComponentDefaults_InvokeWithComponentWithoutSite_Nop()
        {
            var designer = new ComponentDesigner();
            var component = new Component();
            designer.Initialize(component);
            designer.OnSetComponentDefaults();
        }

        [Fact]
        public void ComponentDesigner_OnSetComponentDefaults_InvokeWithoutComponent_Nop()
        {
            var designer = new ComponentDesigner();
            designer.OnSetComponentDefaults();
        }
#pragma warning restore 0618

        [Fact]
        public void ComponentDesigner_IDesignerFilterPreFilterAttributes_Invoke_CallsProtectedVirtualMethod()
        {
            var mockDesigner = new Mock<ComponentDesigner>(MockBehavior.Strict);
            mockDesigner
                .Protected()
                .Setup("PreFilterAttributes", ItExpr.IsAny<IDictionary>())
                .Verifiable();
            IDesignerFilter filter = mockDesigner.Object;

            filter.PreFilterAttributes(new Dictionary<string, object>());

            mockDesigner.Protected().Verify("PreFilterAttributes", Times.Once(), ItExpr.IsAny<IDictionary>());
        }

        [Fact]
        public void ComponentDesigner_IDesignerFilterPreFilterEvents_Invoke_CallsProtectedVirtualMethod()
        {
            var mockDesigner = new Mock<ComponentDesigner>(MockBehavior.Strict);
            mockDesigner
                .Protected()
                .Setup("PreFilterEvents", ItExpr.IsAny<IDictionary>())
                .Verifiable();
            IDesignerFilter filter = mockDesigner.Object;

            filter.PreFilterEvents(new Dictionary<string, object>());

            mockDesigner.Protected().Verify("PreFilterEvents", Times.Once(), ItExpr.IsAny<IDictionary>());
        }

        [Fact]
        public void ComponentDesigner_IDesignerFilterPreFilterProperties_WithComponentWithKey_Success()
        {
            //Debugger.Launch();
            var designer = new SubComponentDesigner();
            IDesignerFilter filter = designer;
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            var properties = new Dictionary<string, PropertyDescriptor>
            {
                { "SettingsKey", descriptor }
            };
            var component = new IPersistComponentSettingsComponent();
            designer.Initialize(component);
            filter.PreFilterProperties(properties);
            PropertyDescriptor result = (PropertyDescriptor)properties["SettingsKey"];
            Assert.NotSame(descriptor, result);
            Assert.Equal(typeof(ComponentDesigner), result.ComponentType);
            Assert.Equal(descriptor.Name, result.Name);
            Assert.Equal(7, descriptor.Attributes.Count);
            Assert.Equal(8, result.Attributes.Count);
        }

        [Fact]
        public void ComponentDesigner_IDesignerFilterPreFilterProperties_WithIPersistComponentSettingsComponentWithKey_Success()
        {
            var designer = new SubComponentDesigner();
            IDesignerFilter filter = designer;
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            var properties = new Dictionary<string, PropertyDescriptor>
            {
                { "SettingsKey", descriptor }
            };
            var component = new IPersistComponentSettingsComponent();
            designer.Initialize(component);
            filter.PreFilterProperties(properties);
            PropertyDescriptor result = (PropertyDescriptor)properties["SettingsKey"];
            Assert.NotSame(descriptor, result);
            Assert.Equal(typeof(ComponentDesigner), result.ComponentType);
            Assert.Equal(descriptor.Name, result.Name);
            Assert.Equal(7, descriptor.Attributes.Count);
            Assert.Equal(8, result.Attributes.Count);
        }

        [Theory]
        [MemberData(nameof(PreFilterProperties_ComponentWithoutKey_TestData))]
        public void ComponentDesigner_IDesignerFilterPreFilterProperties_WithIPersistComponentSettingsComponentWithoutKey_Success(IDictionary properties)
        {
            var designer = new SubComponentDesigner();
            IDesignerFilter filter = designer;
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            object oldValue = properties?["SettingsKey"];
            var component = new IPersistComponentSettingsComponent();
            designer.Initialize(component);
            filter.PreFilterProperties(properties);
            Assert.Same(oldValue, properties?["SettingsKey"]);
        }

        [Fact]
        public void ComponentDesigner_IDesignerFilterPreFilterProperties_WithNonIPersistComponentSettingsComponent_Nop()
        {
            var designer = new ComponentDesigner();
            IDesignerFilter filter = designer;
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(typeof(CustomComponent))[0];
            var properties = new Dictionary<string, PropertyDescriptor>
            {
                { "SettingsKey", descriptor }
            };
            var component = new Component();
            designer.Initialize(component);
            filter.PreFilterProperties(properties);
            Assert.Same(descriptor, properties["SettingsKey"]);
        }

        [Theory]
        [MemberData(nameof(IDictionary_TestData))]
        public void ComponentDesigner_IDesignerFilterPreFilterProperties_WithoutComponent_Nop(IDictionary properties)
        {
            var designer = new ComponentDesigner();
            IDesignerFilter filter = designer;
            filter.PreFilterProperties(properties);
        }

        [Fact]
        public void IDesignerFilterPreFilterProperties_Invoke_CallsProtectedVirtualMethod()
        {
            var mockDesigner = new Mock<ComponentDesigner>(MockBehavior.Strict);
            mockDesigner
                .Protected()
                .Setup("PreFilterProperties", ItExpr.IsAny<IDictionary>())
                .Verifiable();
            IDesignerFilter filter = mockDesigner.Object;

            filter.PreFilterProperties(new Dictionary<string, object>());

            mockDesigner.Protected().Verify("PreFilterProperties", Times.Once(), ItExpr.IsAny<IDictionary>());
        }

        [Theory]
        [MemberData(nameof(PostFilterAttributes_NoInheritanceAttribute_TestData))]
        public void ComponentDesigner_IDesignerFilterPostFilterAttributes_NoInheritanceAttribute_AddsToAttributes(InheritanceAttribute attribute, IDictionary attributes, object expected)
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(attribute);
            IDesignerFilter filter = designer;
            filter.PostFilterAttributes(attributes);
            Assert.Same(expected, attributes?[typeof(InheritanceAttribute)]);
        }

        [Theory]
        [MemberData(nameof(PostFilterAttributes_TestData))]
        public void ComponentDesigner_IDesignerFilterPostFilterAttributes_HasInheritanceAttributeKey_Sets(IDictionary attributes, object expected)
        {
            var designer = new SubComponentDesigner();
            IDesignerFilter filter = designer;
            filter.PostFilterAttributes(attributes);
            Assert.Same(expected, designer.InheritanceAttribute);
        }

        [Fact]
        public void ComponentDesigner_IDesignerFilterPostFilterAttributes_Invoke_CallsProtectedVirtualMethod()
        {
            var mockDesigner = new Mock<ComponentDesigner>(MockBehavior.Strict);
            mockDesigner
                .Protected()
                .SetupGet<InheritanceAttribute>("InheritanceAttribute")
                .Returns(InheritanceAttribute.Default);
            mockDesigner
                .Protected()
                .Setup("PostFilterAttributes", ItExpr.IsAny<IDictionary>())
                .Verifiable();
            IDesignerFilter filter = mockDesigner.Object;

            filter.PostFilterAttributes(new Dictionary<string, object>());

            mockDesigner.Protected().Verify("PostFilterAttributes", Times.Once(), ItExpr.IsAny<IDictionary>());
        }

        [Theory]
        [MemberData(nameof(PostFilterEvents_HasEvents_TestData))]
        public void ComponentDesigner_IDesignerFilterPostFilterEvents_InvokeWithEvents_Success(InheritanceAttribute inheritanceAttribute, bool valid)
        {
            EventDescriptor descriptor = TypeDescriptor.GetEvents(typeof(CustomComponent))[0];
            var events = new Dictionary<object, object> { { "key1", descriptor }, { "Key2", null } };
            var designer = new CustomInheritanceAttributeComponentDesigner(inheritanceAttribute);
            IDesignerFilter filter = designer;
            filter.PostFilterEvents(events);
            if (valid)
            {
                EventDescriptor result = Assert.IsAssignableFrom<EventDescriptor>(events["Event"]);
                Assert.Equal(typeof(CustomComponent), result.ComponentType);
                Assert.Equal("Event", result.Name);
                Assert.True(Assert.IsType<ReadOnlyAttribute>(result.Attributes[typeof(ReadOnlyAttribute)]).IsReadOnly);
                Assert.Equal(new Dictionary<object, object> { { "key1", descriptor }, { "Key2", null }, { "Event", result } }, events);
            }
            else
            {
                Assert.Equal(new Dictionary<object, object> { { "key1", descriptor }, { "Key2", null } }, events);
            }
        }

        [Theory]
        [MemberData(nameof(PostFilterEvents_NoEvents_TestData))]
        public void ComponentDesigner_IDesignerFilterPostFilterEvents_InvokeWithoutEvents_Success(InheritanceAttribute inheritanceAttribute, IDictionary events, IDictionary expected)
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(inheritanceAttribute);
            IDesignerFilter filter = designer;
            filter.PostFilterEvents(events);
            Assert.Equal(expected, events);
        }

        [Fact]
        public void ComponentDesigner_IDesignerFilterPostFilterEvents_InvokeWithInvalidEvents_ThrowsArrayTypeMismatchException()
        {
            var designer = new CustomInheritanceAttributeComponentDesigner(InheritanceAttribute.InheritedReadOnly);
            IDesignerFilter filter = designer;
            Assert.Throws<ArrayTypeMismatchException>(() => filter.PostFilterEvents(new Dictionary<object, object> { { "key", new object() } } ));
        }

        [Fact]
        public void PostFilterEvents_Invoke_CallsProtectedVirtualMethod()
        {
            var mockDesigner = new Mock<ComponentDesigner>(MockBehavior.Strict);
            mockDesigner
                .Protected()
                .SetupGet<InheritanceAttribute>("InheritanceAttribute")
                .Returns(InheritanceAttribute.Default);
            mockDesigner
                .Protected()
                .Setup("PostFilterEvents", ItExpr.IsAny<IDictionary>())
                .Verifiable();
            IDesignerFilter filter = mockDesigner.Object;

            filter.PostFilterEvents(new Dictionary<string, object>());

            mockDesigner.Protected().Verify("PostFilterEvents", Times.Once(), ItExpr.IsAny<IDictionary>());
        }

        private class SubComponentDesigner : ComponentDesigner
        {
            public new InheritanceAttribute InheritanceAttribute => base.InheritanceAttribute;

            public new bool Inherited => base.Inherited;

            public new IComponent ParentComponent => base.ParentComponent;

            public new object ShadowProperties => base.ShadowProperties;

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new object GetService(Type serviceType) => base.GetService(serviceType);

            public new InheritanceAttribute InvokeGetInheritanceAttribute(ComponentDesigner toInvoke) => base.InvokeGetInheritanceAttribute(toInvoke);

            public new void PreFilterAttributes(IDictionary attributes) => base.PreFilterAttributes(attributes);

            public new void PreFilterEvents(IDictionary events) => base.PreFilterEvents(events);

            public new void PreFilterProperties(IDictionary properties) => base.PreFilterProperties(properties);

            public new void PostFilterAttributes(IDictionary attributes) => base.PostFilterAttributes(attributes);

            public new void PostFilterEvents(IDictionary events) => base.PostFilterEvents(events);

            public new void PostFilterProperties(IDictionary events) => base.PostFilterProperties(events);

            public new void RaiseComponentChanged(MemberDescriptor member, object oldValue, object newValue)
            {
                base.RaiseComponentChanged(member, oldValue, newValue);
            }

            public new void RaiseComponentChanging(MemberDescriptor member) => base.RaiseComponentChanging(member);
        }

        private class CustomAssociatedComponentsComponentDesigner : ComponentDesigner
        {
            private readonly ICollection _associatedComponents;

            public CustomAssociatedComponentsComponentDesigner(ICollection associatedComponents)
            {
                _associatedComponents = associatedComponents;
            }

            public override ICollection AssociatedComponents => _associatedComponents;
        }

        private class CustomInheritanceAttributeComponentDesigner : ComponentDesigner
        {
            private InheritanceAttribute _inheritanceAttribute;

            public CustomInheritanceAttributeComponentDesigner(InheritanceAttribute inheritanceAttribute) : base()
            {
                _inheritanceAttribute = inheritanceAttribute;
            }

            protected override InheritanceAttribute InheritanceAttribute => _inheritanceAttribute;

            public new bool Inherited => base.Inherited;

            public new void PostFilterAttributes(IDictionary attributes) => base.PostFilterAttributes(attributes);

            public new void PostFilterEvents(IDictionary events) => base.PostFilterEvents(events);

            public new void PostFilterProperties(IDictionary events) => base.PostFilterProperties(events);
        }

        private class IPersistComponentSettingsComponent : Component, IPersistComponentSettings
        {
            public bool SaveSettings { get; set; }

            public string SettingsKey { get; set; }

            public void LoadComponentSettings() => throw new NotImplementedException();

            public void ResetComponentSettings() => throw new NotImplementedException();

            public void SaveComponentSettings() => throw new NotImplementedException();
        }

        private class CustomComponent : Component
        {
            [EditorBrowsable(EditorBrowsableState.Advanced)]
            public int Property { get; set; }

            public event EventHandler Event
            {
                add { }
                remove { }
            }
        }

        [DefaultProperty(nameof(StringDefaultPropertyComponent.Value))]
        private class StringDefaultPropertyComponent : Component
        {
            public string Value { get; set; }
        }

        [DefaultProperty(nameof(IntDefaultPropertyComponent.Value))]
        private class IntDefaultPropertyComponent : Component
        {
            public int Value { get; set; }
        }

        [DefaultEvent(nameof(DefaultEventComponent.Event))]
        private class DefaultEventComponent : Component
        {
            private string _stringProperty = null;

            public event EventHandler Event
            {
                add { }
                remove { }
            }

            public object ObjectProperty
            {
                get => StringProperty;
                set => StringProperty = (string)value;
            }

            public string StringProperty
            {
                get => _stringProperty;
                set
                {
                    StringPropertySetCount++;
                    _stringProperty = value;
                }
            }

            public int StringPropertySetCount { get; set; } = 0;

            public int IntProperty { get; set; }

            public object ReadOnlyProperty { get; }
        }
    }
}
