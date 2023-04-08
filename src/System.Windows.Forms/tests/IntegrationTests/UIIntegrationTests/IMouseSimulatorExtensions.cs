// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Windows.Forms.UITests.Input;

// Work around for global usings breaking things
using ThreadState = System.Diagnostics.ThreadState;

namespace System.Windows.Forms.UITests
{
    internal static class IMouseSimulatorExtensions
    {
        public static MouseSimulator DragMouseBy(this MouseSimulator simulator, int pixelDeltaX, int pixelDeltaY)
        {
            simulator.MoveMouseBy(pixelDeltaX, pixelDeltaY);

            // Wait for the message to be processed. Since this is a drag operation, it is possible for the main thread
            // to busy-wait for additional mouse messages during the handling of the previous message, so we block for
            // a short timeout before silently allowing more messages to be sent.
            simulator.WaitForInputIdle(throwOnTimeOut: false, TimeSpan.FromMilliseconds(200));

            return simulator;
        }

        public static MouseSimulator DragMouseTo(this MouseSimulator simulator, double absoluteX, double absoluteY)
        {
            simulator.MoveMouseTo(absoluteX, absoluteY);

            // Wait for the message to be processed. Since this is a drag operation, it is possible for the main thread
            // to busy-wait for additional mouse messages during the handling of the previous message, so we block for
            // a short timeout before silently allowing more messages to be sent.
            simulator.WaitForInputIdle(throwOnTimeOut: false, TimeSpan.FromMilliseconds(200));

            return simulator;
        }

        private static MouseSimulator WaitForInputIdle(this MouseSimulator simulator, bool throwOnTimeOut, TimeSpan timeout)
        {
            using var process = Process.GetCurrentProcess();
            return WaitForInputIdle(simulator, throwOnTimeOut, (HWND)process.MainWindowHandle, timeout);
        }

        private static MouseSimulator WaitForInputIdle(this MouseSimulator simulator, bool throwOnTimeOut, HWND activeWindow, TimeSpan timeout)
        {
            var stopwatch = Stopwatch.StartNew();
            var threadId = PInvoke.GetWindowThreadProcessId(activeWindow, out var processId);
            if (threadId == 0)
                return simulator;

            while (true)
            {
                if (IsThreadIdle(processId, threadId))
                    return simulator;

                if (stopwatch.Elapsed >= timeout)
                {
                    break;
                }

                Thread.Sleep(TimeSpan.FromMilliseconds(15));
            }

            if (throwOnTimeOut)
                throw new TimeoutException();

            return simulator;
        }

        private static bool IsThreadIdle(uint processId, uint threadId)
        {
            using var process = Process.GetProcessById((int)processId);
            var thread = process.Threads.Cast<ProcessThread>().FirstOrDefault(t => threadId == t.Id);
            if (thread is null)
                return true;

            return thread is null
                || thread is { ThreadState: ThreadState.Wait, WaitReason: ThreadWaitReason.UserRequest };
        }
    }
}
