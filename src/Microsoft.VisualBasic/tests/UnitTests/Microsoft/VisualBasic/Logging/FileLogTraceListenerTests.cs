// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Logging.Tests;

public class FileLogTraceListenerTests : FileCleanupTestBase
{
    [Fact]
    public void Properties()
    {
        using FileLogTraceListener listener = new();
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

    [Theory]
    [BoolData]
    public void Write(bool includeHostName)
    {
        TraceEventCache cache = new();
        using FileLogTraceListener listener = new()
        {
            Location = LogFileLocation.Custom,
            CustomLocation = GetTestFilePath(),
            IncludeHostName = includeHostName
        };

        listener.Write("Write");
        listener.WriteLine("WriteLine");
        listener.TraceOutputOptions = TraceOptions.LogicalOperationStack | TraceOptions.DateTime | TraceOptions.Timestamp | TraceOptions.ProcessId | TraceOptions.ThreadId;
        listener.TraceEvent(eventCache: cache, source: "Source", eventType: TraceEventType.Warning, id: 3, message: "TraceEvent");
        listener.AutoFlush = true;
        listener.TraceData(eventCache: cache, source: "Source", eventType: TraceEventType.Error, id: 4, data: "TraceData");
        listener.Flush();
        listener.Close();
    }
}
