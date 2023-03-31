﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.ImageList;

namespace System.Windows.Forms.Tests
{
    public class ListView_NativeImageListTests
    {
        [WinFormsFact]
        public void NativeImageList_Dispose_releases_native_handle()
        {
            using var formatterScope = new BinaryFormatterScope(enable: true);
            using ImageListStreamer result = BinarySerialization.EnsureDeserialize<ImageListStreamer>(ClassicImageListStreamer);

            NativeImageList nativeImageList = result.GetNativeImageList();
            Assert.NotEqual(HIMAGELIST.Null, nativeImageList.HIMAGELIST);

            nativeImageList.Dispose();
            Assert.Equal(HIMAGELIST.Null, nativeImageList.HIMAGELIST);
        }

        private const string ClassicImageListStreamer =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACZTeXN0ZW0uV2luZG93cy5Gb3Jtcy5JbWFnZUxpc3RTdHJlYW1lcgEAAAAERGF0YQcCAgAAAAkDAAAADwMAAAAqBwAAAk1TRnQBSQFMAwEBAAEIAQABCAEAARABAAEQAQAE/wEJAQAI/wFCAU0BNgEEBgABNgEEAgABKAMAAUADAAEQAwABAQEAAQgGAAEEGAABgAIAAYADAAKAAQABgAMAAYABAAGAAQACgAIAA8ABAAHAAdwBwAEAAfABygGmAQABMwUAATMBAAEzAQABMwEAAjMCAAMWAQADHAEAAyIBAAMpAQADVQEAA00BAANCAQADOQEAAYABfAH/AQACUAH/AQABkwEAAdYBAAH/AewBzAEAAcYB1gHvAQAB1gLnAQABkAGpAa0CAAH/ATMDAAFmAwABmQMAAcwCAAEzAwACMwIAATMBZgIAATMBmQIAATMBzAIAATMB/wIAAWYDAAFmATMCAAJmAgABZgGZAgABZgHMAgABZgH/AgABmQMAAZkBMwIAAZkBZgIAApkCAAGZAcwCAAGZAf8CAAHMAwABzAEzAgABzAFmAgABzAGZAgACzAIAAcwB/wIAAf8BZgIAAf8BmQIAAf8BzAEAATMB/wIAAf8BAAEzAQABMwEAAWYBAAEzAQABmQEAATMBAAHMAQABMwEAAf8BAAH/ATMCAAMzAQACMwFmAQACMwGZAQACMwHMAQACMwH/AQABMwFmAgABMwFmATMBAAEzAmYBAAEzAWYBmQEAATMBZgHMAQABMwFmAf8BAAEzAZkCAAEzAZkBMwEAATMBmQFmAQABMwKZAQABMwGZAcwBAAEzAZkB/wEAATMBzAIAATMBzAEzAQABMwHMAWYBAAEzAcwBmQEAATMCzAEAATMBzAH/AQABMwH/ATMBAAEzAf8BZgEAATMB/wGZAQABMwH/AcwBAAEzAv8BAAFmAwABZgEAATMBAAFmAQABZgEAAWYBAAGZAQABZgEAAcwBAAFmAQAB/wEAAWYBMwIAAWYCMwEAAWYBMwFmAQABZgEzAZkBAAFmATMBzAEAAWYBMwH/AQACZgIAAmYBMwEAA2YBAAJmAZkBAAJmAcwBAAFmAZkCAAFmAZkBMwEAAWYBmQFmAQABZgKZAQABZgGZAcwBAAFmAZkB/wEAAWYBzAIAAWYBzAEzAQABZgHMAZkBAAFmAswBAAFmAcwB/wEAAWYB/wIAAWYB/wEzAQABZgH/AZkBAAFmAf8BzAEAAcwBAAH/AQAB/wEAAcwBAAKZAgABmQEzAZkBAAGZAQABmQEAAZkBAAHMAQABmQMAAZkCMwEAAZkBAAFmAQABmQEzAcwBAAGZAQAB/wEAAZkBZgIAAZkBZgEzAQABmQEzAWYBAAGZAWYBmQEAAZkBZgHMAQABmQEzAf8BAAKZATMBAAKZAWYBAAOZAQACmQHMAQACmQH/AQABmQHMAgABmQHMATMBAAFmAcwBZgEAAZkBzAGZAQABmQLMAQABmQHMAf8BAAGZAf8CAAGZAf8BMwEAAZkBzAFmAQABmQH/AZkBAAGZAf8BzAEAAZkC/wEAAcwDAAGZAQABMwEAAcwBAAFmAQABzAEAAZkBAAHMAQABzAEAAZkBMwIAAcwCMwEAAcwBMwFmAQABzAEzAZkBAAHMATMBzAEAAcwBMwH/AQABzAFmAgABzAFmATMBAAGZAmYBAAHMAWYBmQEAAcwBZgHMAQABmQFmAf8BAAHMAZkCAAHMAZkBMwEAAcwBmQFmAQABzAKZAQABzAGZAcwBAAHMAZkB/wEAAswCAALMATMBAALMAWYBAALMAZkBAAPMAQACzAH/AQABzAH/AgABzAH/ATMBAAGZAf8BZgEAAcwB/wGZAQABzAH/AcwBAAHMAv8BAAHMAQABMwEAAf8BAAFmAQAB/wEAAZkBAAHMATMCAAH/AjMBAAH/ATMBZgEAAf8BMwGZAQAB/wEzAcwBAAH/ATMB/wEAAf8BZgIAAf8BZgEzAQABzAJmAQAB/wFmAZkBAAH/AWYBzAEAAcwBZgH/AQAB/wGZAgAB/wGZATMBAAH/AZkBZgEAAf8CmQEAAf8BmQHMAQAB/wGZAf8BAAH/AcwCAAH/AcwBMwEAAf8BzAFmAQAB/wHMAZkBAAH/AswBAAH/AcwB/wEAAv8BMwEAAcwB/wFmAQAC/wGZAQAC/wHMAQACZgH/AQABZgH/AWYBAAFmAv8BAAH/AmYBAAH/AWYB/wEAAv8BZgEAASEBAAGlAQADXwEAA3cBAAOGAQADlgEAA8sBAAOyAQAD1wEAA90BAAPjAQAD6gEAA/EBAAP4AQAB8AH7Af8BAAGkAqABAAOAAwAB/wIAAf8DAAL/AQAB/wMAAf8BAAH/AQAC/wIAA///AP8A/wD/AAUAAUIBTQE+BwABPgMAASgDAAFAAwABEAMAAQEBAAEBBQABgBcAA/8BAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAL";
    }
}
