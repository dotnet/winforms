﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using static System.Windows.Forms.ImageList;

namespace System.Windows.Forms.Tests;

public class ImageListStreamerTests
{
    [WinFormsFact]
    public void ImageListStreamer_BinaryFormatter_Stream_BinaryFormatter_round_trip_equality()
    {
        using BinaryFormatterScope formatterScope = new(enable: true);

        // Create an ImageListStreamer via BinaryFormatter
        using ImageListStreamer streamerFromBf = BinarySerialization.EnsureDeserialize<ImageListStreamer>(ClassicBfImageListStreamer);
        using NativeImageList nativeImageListBf = streamerFromBf.GetNativeImageList();
        Assert.NotEqual(HIMAGELIST.Null, nativeImageListBf.HIMAGELIST);

        // Read as a memory stream
        using MemoryStream ms = new();
        streamerFromBf.GetObjectData(ms);
        string msBase64 = Convert.ToBase64String(ms.ToArray());

        // Create a new ImageListStreamer from the stream
        ms.Position = 0;
        using ImageListStreamer streamerFromMs = new(ms.GetBuffer());
        using NativeImageList nativeImageListMs = streamerFromMs.GetNativeImageList();
        Assert.NotEqual(HIMAGELIST.Null, nativeImageListMs.HIMAGELIST);

        // Compare the two
        using ImageList imageListBf = new();
        imageListBf.ImageStream = streamerFromBf;
        using ImageList imageListMs = new();
        imageListMs.ImageStream = streamerFromMs;

        Assert.Equal(imageListBf.ColorDepth, imageListMs.ColorDepth);
        Assert.Equal(imageListBf.Images.Count, imageListMs.Images.Count);
        Assert.Equal(imageListBf.ImageSize, imageListMs.ImageSize);
    }

    [WinFormsFact]
    public void ImageListStreamer_Stream_BinaryFormatter_compatible()
    {
        using BinaryFormatterScope formatterScope = new(enable: true);

        // Create a new ImageListStreamer from the stream
        byte[] bytes = Convert.FromBase64String(DevMsImageListStreamer);
        using ImageListStreamer streamerFromMs = new(bytes);
        using NativeImageList nativeImageListMs = streamerFromMs.GetNativeImageList();
        Assert.NotEqual(HIMAGELIST.Null, nativeImageListMs.HIMAGELIST);

        // Create an ImageListStreamer via BinaryFormatter
        using ImageListStreamer streamerFromBf = BinarySerialization.EnsureDeserialize<ImageListStreamer>(ClassicBfImageListStreamer);
        using NativeImageList nativeImageListBf = streamerFromBf.GetNativeImageList();
        Assert.NotEqual(HIMAGELIST.Null, nativeImageListBf.HIMAGELIST);

        // Compare the two
        using ImageList imageListBf = new();
        imageListBf.ImageStream = streamerFromBf;
        using ImageList imageListMs = new();
        imageListMs.ImageStream = streamerFromMs;

        Assert.Equal(imageListBf.ColorDepth, imageListMs.ColorDepth);
        Assert.Equal(imageListBf.Images.Count, imageListMs.Images.Count);
        Assert.Equal(imageListBf.ImageSize, imageListMs.ImageSize);
    }

    private const string ClassicBfImageListStreamer =
        "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACZTeXN0ZW0uV2luZG93cy5Gb3Jtcy5JbWFnZUxpc3RTdHJlYW1lcgEAAAAERGF0YQcCAgAAAAkDAAAADwMAAAAqBwAAAk1TRnQBSQFMAwEBAAEIAQABCAEAARABAAEQAQAE/wEJAQAI/wFCAU0BNgEEBgABNgEEAgABKAMAAUADAAEQAwABAQEAAQgGAAEEGAABgAIAAYADAAKAAQABgAMAAYABAAGAAQACgAIAA8ABAAHAAdwBwAEAAfABygGmAQABMwUAATMBAAEzAQABMwEAAjMCAAMWAQADHAEAAyIBAAMpAQADVQEAA00BAANCAQADOQEAAYABfAH/AQACUAH/AQABkwEAAdYBAAH/AewBzAEAAcYB1gHvAQAB1gLnAQABkAGpAa0CAAH/ATMDAAFmAwABmQMAAcwCAAEzAwACMwIAATMBZgIAATMBmQIAATMBzAIAATMB/wIAAWYDAAFmATMCAAJmAgABZgGZAgABZgHMAgABZgH/AgABmQMAAZkBMwIAAZkBZgIAApkCAAGZAcwCAAGZAf8CAAHMAwABzAEzAgABzAFmAgABzAGZAgACzAIAAcwB/wIAAf8BZgIAAf8BmQIAAf8BzAEAATMB/wIAAf8BAAEzAQABMwEAAWYBAAEzAQABmQEAATMBAAHMAQABMwEAAf8BAAH/ATMCAAMzAQACMwFmAQACMwGZAQACMwHMAQACMwH/AQABMwFmAgABMwFmATMBAAEzAmYBAAEzAWYBmQEAATMBZgHMAQABMwFmAf8BAAEzAZkCAAEzAZkBMwEAATMBmQFmAQABMwKZAQABMwGZAcwBAAEzAZkB/wEAATMBzAIAATMBzAEzAQABMwHMAWYBAAEzAcwBmQEAATMCzAEAATMBzAH/AQABMwH/ATMBAAEzAf8BZgEAATMB/wGZAQABMwH/AcwBAAEzAv8BAAFmAwABZgEAATMBAAFmAQABZgEAAWYBAAGZAQABZgEAAcwBAAFmAQAB/wEAAWYBMwIAAWYCMwEAAWYBMwFmAQABZgEzAZkBAAFmATMBzAEAAWYBMwH/AQACZgIAAmYBMwEAA2YBAAJmAZkBAAJmAcwBAAFmAZkCAAFmAZkBMwEAAWYBmQFmAQABZgKZAQABZgGZAcwBAAFmAZkB/wEAAWYBzAIAAWYBzAEzAQABZgHMAZkBAAFmAswBAAFmAcwB/wEAAWYB/wIAAWYB/wEzAQABZgH/AZkBAAFmAf8BzAEAAcwBAAH/AQAB/wEAAcwBAAKZAgABmQEzAZkBAAGZAQABmQEAAZkBAAHMAQABmQMAAZkCMwEAAZkBAAFmAQABmQEzAcwBAAGZAQAB/wEAAZkBZgIAAZkBZgEzAQABmQEzAWYBAAGZAWYBmQEAAZkBZgHMAQABmQEzAf8BAAKZATMBAAKZAWYBAAOZAQACmQHMAQACmQH/AQABmQHMAgABmQHMATMBAAFmAcwBZgEAAZkBzAGZAQABmQLMAQABmQHMAf8BAAGZAf8CAAGZAf8BMwEAAZkBzAFmAQABmQH/AZkBAAGZAf8BzAEAAZkC/wEAAcwDAAGZAQABMwEAAcwBAAFmAQABzAEAAZkBAAHMAQABzAEAAZkBMwIAAcwCMwEAAcwBMwFmAQABzAEzAZkBAAHMATMBzAEAAcwBMwH/AQABzAFmAgABzAFmATMBAAGZAmYBAAHMAWYBmQEAAcwBZgHMAQABmQFmAf8BAAHMAZkCAAHMAZkBMwEAAcwBmQFmAQABzAKZAQABzAGZAcwBAAHMAZkB/wEAAswCAALMATMBAALMAWYBAALMAZkBAAPMAQACzAH/AQABzAH/AgABzAH/ATMBAAGZAf8BZgEAAcwB/wGZAQABzAH/AcwBAAHMAv8BAAHMAQABMwEAAf8BAAFmAQAB/wEAAZkBAAHMATMCAAH/AjMBAAH/ATMBZgEAAf8BMwGZAQAB/wEzAcwBAAH/ATMB/wEAAf8BZgIAAf8BZgEzAQABzAJmAQAB/wFmAZkBAAH/AWYBzAEAAcwBZgH/AQAB/wGZAgAB/wGZATMBAAH/AZkBZgEAAf8CmQEAAf8BmQHMAQAB/wGZAf8BAAH/AcwCAAH/AcwBMwEAAf8BzAFmAQAB/wHMAZkBAAH/AswBAAH/AcwB/wEAAv8BMwEAAcwB/wFmAQAC/wGZAQAC/wHMAQACZgH/AQABZgH/AWYBAAFmAv8BAAH/AmYBAAH/AWYB/wEAAv8BZgEAASEBAAGlAQADXwEAA3cBAAOGAQADlgEAA8sBAAOyAQAD1wEAA90BAAPjAQAD6gEAA/EBAAP4AQAB8AH7Af8BAAGkAqABAAOAAwAB/wIAAf8DAAL/AQAB/wMAAf8BAAH/AQAC/wIAA///AP8A/wD/AAUAAUIBTQE+BwABPgMAASgDAAFAAwABEAMAAQEBAAEBBQABgBcAA/8BAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAL";

    // Base64-reencoded streamer using GetObjectData(Stream) from a decoded ClassicBfImageListStreamer
    private const string DevMsImageListStreamer =
        "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQk0+AAAAAAAAAD4AAAAoAAAAQAAAABAAAAABAAEAAAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP///wD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";
}
