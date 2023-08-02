using System.ComponentModel.Design;
using System.ComponentModel;
using Moq;

namespace System.Windows.Forms.Design.Tests.Mocks
{
    public class MockSiteUtil
    {
        public static Mock<ISite> CreateMockSiteWithDesignerHost(object designerHost)
        {
            Mock<ISite> mockSite = new(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(designerHost);
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
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(DesignerActionService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(IComponentChangeService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ToolStripKeyboardHandlingService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(ISupportInSituService)))
                .Returns(null);
            mockSite
                .Setup(s => s.GetService(typeof(INestedContainer)))
                .Returns(null);

            Mock<ISelectionService> mockSelectionService = new(MockBehavior.Strict);

            mockSite
                .Setup(s => s.GetService(typeof(ISelectionService)))
                .Returns(mockSelectionService.Object);
            mockSite
                .Setup(s => s.Container)
                .Returns((IContainer)null);
            mockSite
                .Setup(s => s.Name)
                .Returns("Site");
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            mockSite
                .Setup(s => s.GetService(typeof(UndoEngine)))
                .Returns(null);

            return mockSite;
        }
    }
}
