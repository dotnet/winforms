// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Tests;

public class FileDialogCustomPlacesCollectionTests
{
    public static TheoryData<string?> GetStringWithNullTheoryData()
    {
        var data = new TheoryData<string?>
        {
            null,
            string.Empty,
            "reasonable"
        };
        return data;
    }

    [Theory]
    [MemberData(nameof(GetStringWithNullTheoryData))]
    public void FileDialogCustomPlacesCollection_Add_String_Success(string? path)
    {
        FileDialogCustomPlacesCollection collection = new()
        {
            path
        };
        FileDialogCustomPlace place = Assert.Single(collection);
        Assert.Equal(Guid.Empty, place.KnownFolderGuid);
        Assert.Same(path ?? string.Empty, place.Path);
    }

    public static TheoryData<Guid> GetGuidTheoryData()
    {
        TheoryData<Guid> data = new()
        {
            Guid.Empty,
            Guid.NewGuid()
        };
        return data;
    }

    [Theory]
    [MemberData(nameof(GetGuidTheoryData))]
    public void FileDialogCustomPlacesCollection_Add_Guid_Success(Guid knownFolderGuid)
    {
        FileDialogCustomPlacesCollection collection = new()
        {
            knownFolderGuid
        };
        FileDialogCustomPlace place = Assert.Single(collection);
        Assert.Equal(knownFolderGuid, place.KnownFolderGuid);
        Assert.Empty(place.Path);
    }
}
