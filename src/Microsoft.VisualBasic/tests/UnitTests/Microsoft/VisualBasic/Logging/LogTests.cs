// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.VisualBasic.Logging.Tests;

public class LogTests : FileCleanupTestBase
{
    [Fact]
    public void Properties()
    {
        Log log = new();
        _ = log.TraceSource;
        _ = log.DefaultFileLogWriter;
    }

    [Fact]
    public void Write()
    {
        Log log = new();
        var listener = log.DefaultFileLogWriter;
        listener.Location = LogFileLocation.Custom;
        listener.CustomLocation = GetTestFilePath();

        log.WriteEntry("WriteEntry");
        log.WriteEntry("WriteEntry", severity: TraceEventType.Warning);
        log.WriteEntry("WriteEntry", severity: TraceEventType.Error, id: 3);

        log.WriteException(new ArgumentException());
        log.WriteException(new ArgumentException(), severity: TraceEventType.Warning, additionalInfo: "AdditionalInfo");
        log.WriteException(new ArgumentException(), severity: TraceEventType.Warning, additionalInfo: "AdditionalInfo", id: 6);
    }
}
