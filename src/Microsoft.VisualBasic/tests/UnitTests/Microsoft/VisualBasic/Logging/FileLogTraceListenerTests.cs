// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using Xunit;

namespace Microsoft.VisualBasic.Logging.Tests
{
    public class FileLogTraceListenerTests : Microsoft.VisualBasic.Tests.FileCleanupTestBase
    {
        [Fact]
        public void Properties()
        {
            var listener = new FileLogTraceListener();
            _ = listener.Location;
            _ = listener.AutoFlush;
            _ = listener.IncludeHostName;
            _ = listener.Append;
            _ = listener.DiskSpaceExhaustedBehavior;
            _ = listener.BaseFileName;
            _ = listener.FullLogFileName;
            _ = listener.LogFileCreationSchedule;
            _ = listener.MaxFileSize;
            _ = listener.ReserveDiskSpace;
            _ = listener.Delimiter;
            _ = listener.Encoding;
            _ = listener.CustomLocation;
        }

        [Fact]
        public void Write()
        {
            var cache = new TraceEventCache();
            var listener = new FileLogTraceListener();
            listener.Location = LogFileLocation.Custom;
            listener.CustomLocation = GetTestFilePath();

            listener.Write("Write");
            listener.WriteLine("WriteLine");
            listener.TraceEvent(eventCache: cache, source: "Source", eventType: TraceEventType.Warning, id: 3, message: "TraceEvent");
            listener.TraceData(eventCache: cache, source: "Source", eventType: TraceEventType.Error, id: 4, data: "TraceData");
            listener.Flush();
            listener.Close();
        }
    }
}
