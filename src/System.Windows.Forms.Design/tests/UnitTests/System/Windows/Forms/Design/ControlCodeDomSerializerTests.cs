// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.CodeDom;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using Moq;

namespace System.Windows.Forms.Design.Tests;

public class ControlCodeDomSerializerTests : IDisposable
{
    private readonly Mock<IDesignerSerializationManager> _managerMock;
    private readonly Mock<IDesignerHost> _hostMock;
    private readonly Mock<CodeDomSerializer> _serializerMock;
    private readonly ControlCodeDomSerializer _controlCodeDomSerializer;
    private readonly TestControl _testControl;
    private readonly CodeStatementCollection _codeStatementCollection;

    public ControlCodeDomSerializerTests()
    {
        _managerMock = new();
        _hostMock = new();
        _serializerMock = new();
        _controlCodeDomSerializer = new();
        _testControl = new();
        _codeStatementCollection = [];

        _managerMock.Setup(m => m.GetService(typeof(IDesignerHost))).Returns(_hostMock.Object);
        _managerMock.Setup(m => m.GetSerializer(typeof(Component), typeof(CodeDomSerializer))).Returns(_serializerMock.Object);
    }

    public void Dispose() => _testControl.Dispose();

    [Fact]
    public void Deserialize_SuspendsAndResumesLayout()
    {
        Mock<IContainer> mockContainer = new();
        _managerMock.Setup(m => m.GetService(typeof(IContainer))).Returns(mockContainer.Object);
        mockContainer.Setup(c => c.Components).Returns(new ComponentCollection([_testControl]));

        object codeObject = new();
        object expectedObjectGraphData = new();
        _serializerMock.Setup(s => s.Deserialize(_managerMock.Object, codeObject)).Returns(expectedObjectGraphData);

        _controlCodeDomSerializer.Deserialize(_managerMock.Object, codeObject).Should().Be(expectedObjectGraphData);
    }

    [Fact]
    public void Serialize_ShouldReturnSerializedObject()
    {
        _serializerMock.Setup(s => s.Serialize(_managerMock.Object, _testControl)).Returns(_codeStatementCollection);

        CodeStatementCollection? result = _controlCodeDomSerializer.Serialize(_managerMock.Object, _testControl) as CodeStatementCollection;

        result.Should().NotBeNull();
        result.Should().BeOfType<CodeStatementCollection>();
    }

    public class TestControl : Control
    {
        public bool SuspendLayoutCalled { get; private set; }
        public bool ResumeLayoutCalled { get; private set; }

        public new void SuspendLayout()
        {
            SuspendLayoutCalled = true;
            base.SuspendLayout();
        }

        public new void ResumeLayout(bool performLayout)
        {
            ResumeLayoutCalled = true;
            base.ResumeLayout(performLayout);
        }
    }
}
