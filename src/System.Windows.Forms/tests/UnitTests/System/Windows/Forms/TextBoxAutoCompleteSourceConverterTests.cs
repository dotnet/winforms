// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Moq;
using System.ComponentModel;
using static System.ComponentModel.TypeConverter;

namespace System.Windows.Forms.Tests;

public class TextBoxAutoCompleteSourceConverterTests
{
    [Fact]
    public void TextBoxAutoCompleteSourceConverter_GetStandardValues_HasContext_ReturnsExpected()
    {
        TextBoxAutoCompleteSourceConverter converter = new(typeof(AutoCompleteSource));
        Mock<ITypeDescriptorContext> mockContext = new(MockBehavior.Strict);
        StandardValuesCollection valuesCollection = converter.GetStandardValues(mockContext.Object);

        valuesCollection.Count.Should().Be(8);

        List<object> value = valuesCollection.TestAccessor().Dynamic._values;
        value.Should().Contain(AutoCompleteSource.AllUrl);
        value.Should().Contain(AutoCompleteSource.AllSystemSources);
        value.Should().Contain(AutoCompleteSource.CustomSource);
        value.Should().Contain(AutoCompleteSource.HistoryList);
        value.Should().Contain(AutoCompleteSource.RecentlyUsedList);
        value.Should().Contain(AutoCompleteSource.FileSystem);
        value.Should().Contain(AutoCompleteSource.FileSystemDirectories);
        value.Should().Contain(AutoCompleteSource.None);
    }
}
