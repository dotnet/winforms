// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests.AccessibleObjects;

public class UpDownBase_UpDownBaseAccessibleObjectTests : IDisposable
{
    private readonly TestUpDownBase _upDown;
    private readonly UpDownBase.UpDownBaseAccessibleObject _accObj;

    public UpDownBase_UpDownBaseAccessibleObjectTests()
    {
        _upDown = new();
        _accObj = new(_upDown);
    }

    public void Dispose() => _upDown.Dispose();

    [WinFormsFact]
    public void GetChild_WithValidIndices_ReturnsExpectedChildren()
    {
        _accObj.GetChild(0).Should().BeSameAs(_upDown.TextBox.AccessibilityObject.Parent);
        _accObj.GetChild(1).Should().BeSameAs(_upDown.UpDownButtonsInternal.AccessibilityObject.Parent);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(100)]
    public void GetChild_WithInvalidIndices_ReturnsNull(int index) =>
        _accObj.GetChild(index).Should().BeNull();

    [WinFormsFact]
    public void GetChildCount_WithCustomParents_ReturnsTwo() =>
        _accObj.GetChildCount().Should().Be(2);

    [WinFormsFact]
    public void Role_WithCustomParents_ReturnsSpinButton() =>
        _accObj.Role.Should().Be(AccessibleRole.SpinButton);

    private class TestUpDownBase : UpDownBase
    {
        public TestUpDownBase() : base() { }

        public override void DownButton() => throw new NotImplementedException();
        public override void UpButton() => throw new NotImplementedException();
        protected override void UpdateEditText() => throw new NotImplementedException();
    }
}
