// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;
using ReflectTools;
using WFCTestLib.Log;
using static Interop;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiImageListTests : ReflectBase
    {
        public MauiImageListTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiImageListTests(args));
        }

        [Scenario(true)]
        public ScenarioResult NativeImageList_finalizer_releases_native_handle(TParams p)
        {
            // warm up to create any GDI handles that are necessary, e.g. fonts, brushes, etc.
            using (Form form = CreateForm())
            {
                form.Show();
            }

            int startGdiHandleCount = GetGdiHandles();
            p.log.WriteLine($"GDI handles before: {startGdiHandleCount}");

            // Now test for real
            using (Form form = CreateForm())
            {
                form.Show();
            }

            int endGdiHandleCount = GetGdiHandles();
            p.log.WriteLine($"GDI handles after: {endGdiHandleCount}");

            return new ScenarioResult(startGdiHandleCount == endGdiHandleCount, $"GDI handles before: {startGdiHandleCount} != GDI handles after: {endGdiHandleCount}");

            static int GetGdiHandles()
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect(0);

                int result = User32.GetGuiResources(Process.GetCurrentProcess().Handle, User32.GR.GDIOBJECTS);
                if (result == 0)
                {
                    int lastWin32Error = Marshal.GetLastWin32Error();
                    throw new Win32Exception(lastWin32Error, "Failed to retrieves the count of GDI handles");
                }

                return result;
            }
        }

        private Form CreateForm()
        {
            ListView listView1 = new();
            listView1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            listView1.Location = new Drawing.Point(12, 33);
            listView1.Name = "listView1";
            listView1.Size = new Drawing.Size(439, 59);
            listView1.TabIndex = 0;
            listView1.UseCompatibleStateImageBehavior = false;

            Form form = new();
            form.AutoScaleMode = AutoScaleMode.Font;
            form.Controls.Add(listView1);
            form.Name = "ListViewTest";
            form.Text = "ListView Test";

            ImageList imageList1 = new();
            imageList1.ImageStream = (ImageListStreamer)FromBase64String(ClassicImageListStreamer);
            listView1.SmallImageList = imageList1;

            form.Disposed += (s, e) => imageList1.Dispose();

            return form;
        }

        private static object FromBase64String(string base64String)
        {
            byte[] raw = Convert.FromBase64String(base64String);
            return FromByteArray(raw);
        }

        private static object FromByteArray(byte[] raw)
        {
            var binaryFormatter = new BinaryFormatter
            {
                AssemblyFormat = /* FormatterAssemblyStyle.Simple */0
            };

            using (var serializedStream = new MemoryStream(raw))
            {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
                return binaryFormatter.Deserialize(serializedStream);
#pragma warning restore SYSLIB0011 // Type or member is obsolete
            }
        }

        private const string ClassicImageListStreamer =
            "AAEAAAD/////AQAAAAAAAAAMAgAAAFdTeXN0ZW0uV2luZG93cy5Gb3JtcywgVmVyc2lvbj00LjAuMC4wLCBDdWx0dXJlPW5ldXRyYWwsIFB1YmxpY0tleVRva2VuPWI3N2E1YzU2MTkzNGUwODkFAQAAACZTeXN0ZW0uV2luZG93cy5Gb3Jtcy5JbWFnZUxpc3RTdHJlYW1lcgEAAAAERGF0YQcCAgAAAAkDAAAADwMAAAAqBwAAAk1TRnQBSQFMAwEBAAEIAQABCAEAARABAAEQAQAE/wEJAQAI/wFCAU0BNgEEBgABNgEEAgABKAMAAUADAAEQAwABAQEAAQgGAAEEGAABgAIAAYADAAKAAQABgAMAAYABAAGAAQACgAIAA8ABAAHAAdwBwAEAAfABygGmAQABMwUAATMBAAEzAQABMwEAAjMCAAMWAQADHAEAAyIBAAMpAQADVQEAA00BAANCAQADOQEAAYABfAH/AQACUAH/AQABkwEAAdYBAAH/AewBzAEAAcYB1gHvAQAB1gLnAQABkAGpAa0CAAH/ATMDAAFmAwABmQMAAcwCAAEzAwACMwIAATMBZgIAATMBmQIAATMBzAIAATMB/wIAAWYDAAFmATMCAAJmAgABZgGZAgABZgHMAgABZgH/AgABmQMAAZkBMwIAAZkBZgIAApkCAAGZAcwCAAGZAf8CAAHMAwABzAEzAgABzAFmAgABzAGZAgACzAIAAcwB/wIAAf8BZgIAAf8BmQIAAf8BzAEAATMB/wIAAf8BAAEzAQABMwEAAWYBAAEzAQABmQEAATMBAAHMAQABMwEAAf8BAAH/ATMCAAMzAQACMwFmAQACMwGZAQACMwHMAQACMwH/AQABMwFmAgABMwFmATMBAAEzAmYBAAEzAWYBmQEAATMBZgHMAQABMwFmAf8BAAEzAZkCAAEzAZkBMwEAATMBmQFmAQABMwKZAQABMwGZAcwBAAEzAZkB/wEAATMBzAIAATMBzAEzAQABMwHMAWYBAAEzAcwBmQEAATMCzAEAATMBzAH/AQABMwH/ATMBAAEzAf8BZgEAATMB/wGZAQABMwH/AcwBAAEzAv8BAAFmAwABZgEAATMBAAFmAQABZgEAAWYBAAGZAQABZgEAAcwBAAFmAQAB/wEAAWYBMwIAAWYCMwEAAWYBMwFmAQABZgEzAZkBAAFmATMBzAEAAWYBMwH/AQACZgIAAmYBMwEAA2YBAAJmAZkBAAJmAcwBAAFmAZkCAAFmAZkBMwEAAWYBmQFmAQABZgKZAQABZgGZAcwBAAFmAZkB/wEAAWYBzAIAAWYBzAEzAQABZgHMAZkBAAFmAswBAAFmAcwB/wEAAWYB/wIAAWYB/wEzAQABZgH/AZkBAAFmAf8BzAEAAcwBAAH/AQAB/wEAAcwBAAKZAgABmQEzAZkBAAGZAQABmQEAAZkBAAHMAQABmQMAAZkCMwEAAZkBAAFmAQABmQEzAcwBAAGZAQAB/wEAAZkBZgIAAZkBZgEzAQABmQEzAWYBAAGZAWYBmQEAAZkBZgHMAQABmQEzAf8BAAKZATMBAAKZAWYBAAOZAQACmQHMAQACmQH/AQABmQHMAgABmQHMATMBAAFmAcwBZgEAAZkBzAGZAQABmQLMAQABmQHMAf8BAAGZAf8CAAGZAf8BMwEAAZkBzAFmAQABmQH/AZkBAAGZAf8BzAEAAZkC/wEAAcwDAAGZAQABMwEAAcwBAAFmAQABzAEAAZkBAAHMAQABzAEAAZkBMwIAAcwCMwEAAcwBMwFmAQABzAEzAZkBAAHMATMBzAEAAcwBMwH/AQABzAFmAgABzAFmATMBAAGZAmYBAAHMAWYBmQEAAcwBZgHMAQABmQFmAf8BAAHMAZkCAAHMAZkBMwEAAcwBmQFmAQABzAKZAQABzAGZAcwBAAHMAZkB/wEAAswCAALMATMBAALMAWYBAALMAZkBAAPMAQACzAH/AQABzAH/AgABzAH/ATMBAAGZAf8BZgEAAcwB/wGZAQABzAH/AcwBAAHMAv8BAAHMAQABMwEAAf8BAAFmAQAB/wEAAZkBAAHMATMCAAH/AjMBAAH/ATMBZgEAAf8BMwGZAQAB/wEzAcwBAAH/ATMB/wEAAf8BZgIAAf8BZgEzAQABzAJmAQAB/wFmAZkBAAH/AWYBzAEAAcwBZgH/AQAB/wGZAgAB/wGZATMBAAH/AZkBZgEAAf8CmQEAAf8BmQHMAQAB/wGZAf8BAAH/AcwCAAH/AcwBMwEAAf8BzAFmAQAB/wHMAZkBAAH/AswBAAH/AcwB/wEAAv8BMwEAAcwB/wFmAQAC/wGZAQAC/wHMAQACZgH/AQABZgH/AWYBAAFmAv8BAAH/AmYBAAH/AWYB/wEAAv8BZgEAASEBAAGlAQADXwEAA3cBAAOGAQADlgEAA8sBAAOyAQAD1wEAA90BAAPjAQAD6gEAA/EBAAP4AQAB8AH7Af8BAAGkAqABAAOAAwAB/wIAAf8DAAL/AQAB/wMAAf8BAAH/AQAC/wIAA///AP8A/wD/AAUAAUIBTQE+BwABPgMAASgDAAFAAwABEAMAAQEBAAEBBQABgBcAA/8BAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAC/wYAAv8GAAL/BgAL";
    }
}
