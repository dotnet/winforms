// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public partial class DataFormatsTests
{
    [Collection("Sequential")] // Each registered Clipboard format is an OS singleton,
                               // and we should not run this test at the same time as other tests using the same format.
    [UISettings(MaxAttempts = 3)] // Try up to 3 times before failing.
    public class ClipboardTests
    {
        public static IEnumerable<object[]> GetFormat_Int_TestData()
        {
            uint manuallyRegisteredFormatId = PInvokeCore.RegisterClipboardFormat("ManuallyRegisteredFormat");
            uint longManuallyRegisteredFormatId = PInvokeCore.RegisterClipboardFormat(new string('a', 255));
            yield return new object[] { (int)manuallyRegisteredFormatId, "ManuallyRegisteredFormat" };
            yield return new object[] { (int)longManuallyRegisteredFormatId, new string('a', 255) };
        }

        [Theory]
        [MemberData(nameof(GetFormat_Int_TestData))]
        public void DataFormats_GetFormat_InvokeId_ReturnsExpected(int id, string expectedName)
        {
            DataFormats.Format result = DataFormats.GetFormat(id);
            Assert.Equal(result, DataFormats.GetFormat(id));
            Assert.Equal(expectedName, result.Name);
            Assert.Equal(id & 0xFFFF, result.Id);
        }
    }
}
