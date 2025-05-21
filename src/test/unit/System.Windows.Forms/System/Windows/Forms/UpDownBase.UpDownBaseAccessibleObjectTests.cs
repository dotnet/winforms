// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class UpDownBase_UpDownBaseAccessibleObjectTests
{
    [WinFormsFact]
    public void GetChild_WithValidIndices_ReturnsExpectedChildren()
    {
        using TestUpDownBase upDown = new();
        UpDownBase.UpDownBaseAccessibleObject accObj = new(upDown);

        accObj.GetChild(0).Should().BeSameAs(upDown.TextBox.AccessibilityObject.Parent);
        accObj.GetChild(1).Should().BeSameAs(upDown.UpDownButtonsInternal.AccessibilityObject.Parent);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(2)]
    [InlineData(100)]
    public void GetChild_WithInvalidIndices_ReturnsNull(int index)
    {
        using TestUpDownBase upDown = new();
        UpDownBase.UpDownBaseAccessibleObject accObj = new(upDown);

        accObj.GetChild(index).Should().BeNull();
    }

    [WinFormsFact]
    public void GetChildCount_Returns2()
    {
        using TestUpDownBase upDown = new();
        UpDownBase.UpDownBaseAccessibleObject accObj = new(upDown);

        accObj.GetChildCount().Should().Be(2);
    }

    private class TestUpDownBase : UpDownBase
    {
        public AccessibleObject CustomTextBoxAO { get; }
        public AccessibleObject CustomButtonsAO { get; }

        public TestUpDownBase()
        {
            AccessibleObject textBoxParent = new();
            AccessibleObject buttonsParent = new();

            CustomTextBoxAO = new MyAccessibleObject(textBoxParent);
            CustomButtonsAO = new MyAccessibleObject(buttonsParent);

            TextBox.AccessibilityObject.SetParent(textBoxParent);
            UpDownButtonsInternal.AccessibilityObject.SetParent(buttonsParent);
        }

        public override void DownButton() => throw new NotImplementedException();
        public override void UpButton() => throw new NotImplementedException();
        protected override AccessibleObject CreateAccessibilityInstance()
            => new UpDownBaseAccessibleObject(this);
        protected override void UpdateEditText() => throw new NotImplementedException();
    }

    private class MyAccessibleObject : AccessibleObject
    {
        private readonly AccessibleObject _parent;

        public MyAccessibleObject(AccessibleObject parent)
        {
            _parent = parent;
        }

        public override AccessibleObject? Parent => _parent;
    }
}
