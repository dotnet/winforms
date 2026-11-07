// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.ExceptionServices;

namespace System.Windows.Forms.Tests;

public partial class ClipboardPublicationTests
{
    /// <summary>
    ///  Assigns only a RemoteExecutor process to a private clipboard and desktop.
    /// </summary>
    private sealed class WindowStation : IDisposable
    {
        private readonly nint _original = GetProcessWindowStation();
        private readonly nint _station;

        public WindowStation(string name, bool create)
        {
            Name = name;
            _station = create
                // CREATE_ONLY prevents accidentally using an existing station.
                ? CreateWindowStation(name, 1, 0x37F, 0)
                : OpenWindowStation(name, false, 0x37F);
            Check(_station != 0);
            try
            {
                Check(SetProcessWindowStation(_station));
                Desktop = create
                    ? CreateDesktop(DesktopName, 0, 0, 0, 0x01FF, 0)
                    : OpenDesktop(DesktopName, 0, false, 0x01FF);
                Check(Desktop != 0);
            }
            catch (Exception exception)
            {
                RunWithCleanup(
                    () => ExceptionDispatchInfo.Capture(exception).Throw(),
                    () => Check(SetProcessWindowStation(_original)),
                    () => Check(CloseWindowStation(_station)));
                throw;
            }
        }

        public string Name { get; }
        public nint Desktop { get; }
        public bool WorkerRunning { get; set; }

        public void Dispose()
        {
            if (WorkerRunning)
            {
                throw new InvalidOperationException("The timed-out STA still uses the desktop; the failed isolated process must release it on exit.");
            }

            RunWithCleanup(
                () => Check(CloseDesktop(Desktop)),
                () => Check(SetProcessWindowStation(_original)),
                () => Check(CloseWindowStation(_station)));
        }
    }
}
