// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Logging.Tests;

public class FileLogTraceListenerTests : FileCleanupTestBase
{
    [Fact]
    public void Properties()
    {
        FileLogTraceListener listener = new();
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
        TraceEventCache cache = new();
        FileLogTraceListener listener = new()
        {
            Location = LogFileLocation.Custom,
            CustomLocation = GetTestFilePath()
        };

        listener.Write("Write");
        listener.WriteLine("WriteLine");
        listener.TraceEvent(eventCache: cache, source: "Source", eventType: TraceEventType.Warning, id: 3, message: "TraceEvent");
        listener.TraceData(eventCache: cache, source: "Source", eventType: TraceEventType.Error, id: 4, data: "TraceData");
        listener.Flush();
        listener.Close();
    }
}
