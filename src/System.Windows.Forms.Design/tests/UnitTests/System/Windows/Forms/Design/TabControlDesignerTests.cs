// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using System.Collections;
using System.ComponentModel.Design;
using Moq;

namespace System.Windows.Forms.Design.Tests
{
    public class TabControlDesignerTests
    {
        private (TabControlDesigner designer, TabControl tabControl) CreateInitializedDesignerWithServiceProvider(out Mock<IServiceProvider> serviceProvider, out Mock<IDesignerHost> designerHost)
        {
            TabControl tabControl = new();
            TabControlDesigner designer = new();
            serviceProvider = new();
            designerHost = new();
            serviceProvider.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(designerHost.Object);
            designer.Initialize(tabControl);
            return (designer, tabControl);
        }

        private TabControlDesigner CreateDesignerWithMockedHost()
        {
            TabControlDesigner designer = new();
            TabControl tabControl = new();
            TabPage tabPage = new();
            Mock<IServiceProvider> serviceProvider = new();
            Mock<IDesignerHost> designerHost = new();
            serviceProvider.Setup(s => s.GetService(typeof(IDesignerHost))).Returns(designerHost.Object);
            designerHost.Setup(h => h.CreateComponent(typeof(TabPage))).Returns(tabPage);
            designer.Initialize(tabControl);
            return designer;
        }

        private void SetForwardOnDrag(TabControlDesigner designer, bool value)
        {
            designer.GetType().GetField("_forwardOnDrag", Reflection.BindingFlags.NonPublic | Reflection.BindingFlags.Instance)?.SetValue(designer, value);
        }

        [Fact]
        public void InitializeNewComponent_ShouldAddTabPageToTabControl()
        {
            var (designer, tabControl) = CreateInitializedDesignerWithServiceProvider(out _, out var designerHost);
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);
            designerHost.Setup(h => h.CreateComponent(typeof(TabPage))).Returns(tabPage);

            designer.InitializeNewComponent(new Hashtable());

            tabControl.TabPages.Count.Should().Be(1);
        }

        [Theory]
        [InlineData(typeof(TabPage), true)]
        [InlineData(typeof(Button), false)]
        public void CanParent_ReturnsExpectedResult(Type controlType, bool expected)
        {
            using TabControl tabControl = new();
            using TabControlDesigner designer = new();
            designer.Initialize(tabControl);
            Control? control = Activator.CreateInstance(controlType) as Control;

            if (control is TabPage tabPage && expected == false)
            {
                tabControl.TabPages.Add(tabPage);
            }

            bool result = designer.CanParent(control);

            result.Should().Be(expected);
        }

        [Fact]
        public void Verbs_ContainsAddAndRemoveVerbs()
        {
            var (designer, _) = CreateInitializedDesignerWithServiceProvider(out _, out _);

            var verbs = designer.Verbs.Cast<DesignerVerb>();

            verbs.Should().Contain(v => v.Text == SR.TabControlAdd);
            verbs.Should().Contain(v => v.Text == SR.TabControlRemove);
        }

        [Fact]
        public void Verbs_RemoveVerbEnabledWhenTabPagesExist()
        {
            var (designer, tabControl) = CreateInitializedDesignerWithServiceProvider(out _, out _);
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);

            var verbs = designer.Verbs;

            var removeVerb = verbs.Cast<DesignerVerb>().FirstOrDefault(v => v.Text == SR.TabControlRemove);
            removeVerb.Should().NotBeNull();
            removeVerb!.Enabled.Should().BeTrue();
        }

        [Fact]
        public void Verbs_RemoveVerbDisabledWhenNoTabPagesExist()
        {
            var (designer, _) = CreateInitializedDesignerWithServiceProvider(out _, out _);

            var verbs = designer.Verbs;

            var removeVerb = verbs.Cast<DesignerVerb>().FirstOrDefault(v => v.Text == SR.TabControlRemove);
            removeVerb.Should().NotBeNull();
            removeVerb!.Enabled.Should().BeFalse();
        }

        [Fact]
        public void ParticipatesWithSnapLines_ReturnsFalseWhenNotForwardOnDrag()
        {
            using var designer = CreateDesignerWithMockedHost();

            bool result = designer.ParticipatesWithSnapLines;

            result.Should().BeFalse();
        }

        [Fact]
        public void ParticipatesWithSnapLines_ReturnsTrueWhenForwardOnDragAndPageDesignerParticipates()
        {
            using var designer = CreateDesignerWithMockedHost();
            var tabControl = (TabControl)designer.Component;
            using TabPage tabPage = new();
            tabControl.TabPages.Add(tabPage);

            Mock<TabPageDesigner> pageDesignerMock = new(MockBehavior.Strict);
            pageDesignerMock.Setup(p => p.ParticipatesWithSnapLines).Returns(true);

            SetForwardOnDrag(designer, true);

            bool result = designer.ParticipatesWithSnapLines;

            result.Should().BeTrue();
        }

        [Fact]
        public void ParticipatesWithSnapLines_ReturnsTrueWhenForwardOnDragAndPageDesignerDoesNotParticipate()
        {
            using var designer = CreateDesignerWithMockedHost();

            Mock<TabPageDesigner> pageDesignerMock = new(MockBehavior.Strict);
            pageDesignerMock.Setup(p => p.ParticipatesWithSnapLines).Returns(false);

            SetForwardOnDrag(designer, true);

            bool result = designer.ParticipatesWithSnapLines;

            result.Should().BeTrue();
        }
    }
}
