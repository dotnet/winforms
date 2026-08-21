// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;
using Microsoft.DotNet.RemoteExecutor;
using Windows.Win32.Graphics.GdiPlus;

namespace System.Drawing.Tests;

/// <summary>
///  Tests concurrent first use of GDI+ image operations.
/// </summary>
public class ImageConcurrencyTests
{
    private const int WorkerCount = 16;

    public static bool IsRemoteExecutorSupported => RemoteExecutor.IsSupported;

    [Fact(Skip = "Condition not met", SkipUnless = nameof(IsRemoteExecutorSupported))]
    public void GdiPlusInitialization_ConcurrentFirstUse_Succeeds()
    {
        RemoteExecutor.Invoke(() =>
        {
            using ManualResetEventSlim start = new(initialState: false);
            Task<bool>[] tasks = new Task<bool>[WorkerCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    start.Wait();
                    return GdiPlusInitialization.EnsureInitialized();
                });
            }

            start.Set();

            Assert.All(tasks, task => Assert.True(task.GetAwaiter().GetResult()));
            Assert.True(GdiPlusInitialization.IsInitialized);
        }).Dispose();
    }

    [Fact(Skip = "Condition not met", SkipUnless = nameof(IsRemoteExecutorSupported))]
    public void Save_ConcurrentFirstUseOfEncoderCache_Succeeds()
    {
        RemoteExecutor.Invoke(() =>
        {
            using ManualResetEventSlim start = new(initialState: false);
            Task<long>[] tasks = new Task<long>[WorkerCount];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = Task.Run(() =>
                {
                    using Bitmap bitmap = new(100, 100);
                    using MemoryStream stream = new();

                    start.Wait();
                    bitmap.Save(stream, ImageFormat.Png);

                    return stream.Length;
                });
            }

            start.Set();

            Assert.All(tasks, task => Assert.True(task.GetAwaiter().GetResult() > 0));
        }).Dispose();
    }
}
