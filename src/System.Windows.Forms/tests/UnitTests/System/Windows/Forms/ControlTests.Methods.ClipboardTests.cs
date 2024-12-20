// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable enable

using Moq;
using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;

namespace System.Windows.Forms.Tests;

public partial class ControlTests
{
    // Each registered Clipboard format is an OS singleton,
    // and we should not run this test at the same time as other tests using the same format.
    [Collection("Sequential")]
    public class ClipboardTests
    {
        public static IEnumerable<object[]> DoDragDrop_TestData()
        {
            foreach (DragDropEffects allowedEffects in Enum.GetValues(typeof(DragDropEffects)))
            {
                yield return new object[] { "text", allowedEffects };
                yield return new object[] { new DataObject(), allowedEffects };
                yield return new object[] { new DataObject("data"), allowedEffects };
                yield return new object[] { new Mock<IDataObject>(MockBehavior.Strict).Object, allowedEffects };
                yield return new object[] { new Mock<IComDataObject>(MockBehavior.Strict).Object, allowedEffects };
            }
        }

        [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
        [MemberData(nameof(DoDragDrop_TestData))]
        public void Control_DoDragDrop_Invoke_ReturnsNone(object data, DragDropEffects allowedEffects)
        {
            using Control control = new();
            Assert.Equal(DragDropEffects.None, control.DoDragDrop(data, allowedEffects));
            Assert.False(control.IsHandleCreated);
        }

        [WinFormsTheory(Skip = "hangs CI, see https://github.com/dotnet/winforms/issues/3336")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3336")]
        [MemberData(nameof(DoDragDrop_TestData))]
        public void Control_DoDragDrop_InvokeWithHandle_ReturnsNone(object data, DragDropEffects allowedEffects)
        {
            using Control control = new();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            int styleChangedCallCount = 0;
            control.StyleChanged += (sender, e) => styleChangedCallCount++;
            int createdCallCount = 0;
            control.HandleCreated += (sender, e) => createdCallCount++;

            Assert.Equal(DragDropEffects.None, control.DoDragDrop(data, allowedEffects));
            Assert.True(control.IsHandleCreated);
            Assert.Equal(0, invalidatedCallCount);
            Assert.Equal(0, styleChangedCallCount);
            Assert.Equal(0, createdCallCount);
        }
    }
}
