// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Windows.Win32.System.Threading;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class ImageListTests : ControlTestBase
{
    // Base64-reencoded streamer using GetObjectData(Stream) from a decoded ClassicImageListStreamer
    private const string DevMsImageListStreamer =
        "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AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAQk0+AAAAAAAAAD4AAAAoAAAAQAAAABAAAAABAAEAAAAAAIAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAP///wD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAP//AAAAAAAA//8AAAAAAAD//wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA=";

    public ImageListTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public void ImageList_FinalizerReleasesNativeHandle_ReturnsExpected()
    {
        // Call GetGdiHandles at the start of the test to attempt to clear out any leftovers from previous tests
        uint referenceGdiHandleCount = GetGdiHandles();
        TestOutputHelper.WriteLine($"Reference GDI handle count at start of test: {referenceGdiHandleCount}");

        // Warm up to create any GDI handles that are necessary, e.g. fonts, brushes, etc.
        ShowForm();

        uint startGdiHandleCount = GetGdiHandles();
        TestOutputHelper.WriteLine($"GDI handles before: {startGdiHandleCount}");

        // Now test for real
        ShowForm();

        uint endGdiHandleCount = GetGdiHandles();
        TestOutputHelper.WriteLine($"GDI handles after: {endGdiHandleCount}");

        Assert.Equal(startGdiHandleCount, endGdiHandleCount);

        [MethodImpl(MethodImplOptions.NoInlining)]
        static uint GetGdiHandles()
        {
            uint result = GetGdiHandlesOnce();
            while (true)
            {
                uint updatedResult = GetGdiHandlesOnce();
                if (updatedResult > result)
                {
                    // Nothing should be creating new GDI handles during this call, so throw an exception if this method
                    // ever fails to make progress.
                    throw new InvalidOperationException("Unexpected GDI handle count increase during cleanup.");
                }

                if (updatedResult == result)
                {
                    // When two invocations return the same value, it's assumed to have stabilized
                    return result;
                }

                result = updatedResult;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        static uint GetGdiHandlesOnce()
        {
            GC.GetTotalMemory(true);

            uint result = PInvokeCore.GetGuiResources(
                (HANDLE)Process.GetCurrentProcess().Handle,
                GET_GUI_RESOURCES_FLAGS.GR_GDIOBJECTS);

            if (result == 0)
            {
                int lastWin32Error = Marshal.GetLastWin32Error();
                if (lastWin32Error != 0)
                    throw new Win32Exception(lastWin32Error, "Failed to retrieves the count of GDI handles");
            }

            return result;
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private void ShowForm()
    {
        using Form form = CreateForm();
        form.Show();
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private Form CreateForm()
    {
        ListView listView1 = new()
        {
            Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left,
            Location = new Point(12, 33),
            Name = "listView1",
            Size = new Size(439, 59),
            TabIndex = 0,
            UseCompatibleStateImageBehavior = false
        };

        Form form = new()
        {
            AutoScaleMode = AutoScaleMode.Font
        };
        form.Controls.Add(listView1);
        form.Name = "ListViewTest";
        form.Text = "ListView Test";

        ImageList imageList1 = new()
        {
            ImageStream = DeserializeStreamer(DevMsImageListStreamer)
        };
        listView1.SmallImageList = imageList1;

        return form;
    }

    private static ImageListStreamer DeserializeStreamer(string base64String)
    {
        byte[] bytes = Convert.FromBase64String(base64String);
        return new ImageListStreamer(bytes);
    }
}
