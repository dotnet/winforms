// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Resources.Tests;

// NB: doesn't require thread affinity
public class ResXResourceSetTests
{
    [Theory]
    [InlineData("TestResources.resx", "SomeMissingTest1", null)]
    [InlineData("TestResources.resx", "SomeMetadataTest1", "Some text for MetaData 1 node")]
    [InlineData("TestResources.resx", "SomeMetadataTest2", "Some text for MetaData 2 node")]
    [InlineData("TestResources.resx", "SomeDataTest1", "Some text for Data 1 node")]
    [InlineData("TestResources.resx", "SomeDataTest2", "Some text for Data 2 node")]
    [InlineData("TestResources.resx", "text.ansi", "Text")]
    [InlineData("TestResources.resx", "text.utf8", "Привет")]
    public void ResXResourceSet_TestFile(string resxFileName, string resourceName, string expected)
    {
        Assert.True(File.Exists(resxFileName), $@"RESX file ""{resxFileName}"" not found, make sure it's in the root folder of the unit test project");

        using ResXResourceSet resxSet = new(resxFileName);
        Assert.NotNull(resxSet);

        string strResXValue = resxSet.GetString(resourceName);

        Assert.Equal(expected, strResXValue);
    }

    [Theory]
    [InlineData("TestResources.resx", "SomeMissingTest1", null)]
    [InlineData("TestResources.resx", "SomeMetadataTest1", "Some text for MetaData 1 node")]
    [InlineData("TestResources.resx", "SomeMetadataTest2", "Some text for MetaData 2 node")]
    [InlineData("TestResources.resx", "SomeDataTest1", "Some text for Data 1 node")]
    [InlineData("TestResources.resx", "SomeDataTest2", "Some text for Data 2 node")]
    [InlineData("TestResources.resx", "text.ansi", "Text")]
    [InlineData("TestResources.resx", "text.utf8", "Привет")]
    public void ResXResourceSet_TestStream(string resxFileName, string resourceName, string expected)
    {
        Assert.True(File.Exists(resxFileName), $@"RESX file ""{resxFileName}"" not found, make sure it's in the root folder of the unit test project");

        using FileStream fs = new(resxFileName, FileMode.Open);
        using ResXResourceSet resxSet = new(fs);
        Assert.NotNull(resxSet);

        string strResXValue = resxSet.GetString(resourceName);

        Assert.Equal(expected, strResXValue);
    }
}
