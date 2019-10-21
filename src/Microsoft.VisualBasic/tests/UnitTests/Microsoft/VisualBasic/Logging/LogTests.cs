// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace Microsoft.VisualBasic.Logging.Tests
{
    public class LogTests : Microsoft.VisualBasic.Tests.FileCleanupTestBase
    {
        [Fact]
        public void Properties()
        {
            var log = new Log();
            _ = log.TraceSource;
            _ = log.DefaultFileLogWriter;
        }

        [Fact]
        public void Write()
        {
            var log = new Log();
            var listener = log.DefaultFileLogWriter;
            listener.Location = LogFileLocation.Custom;
            listener.CustomLocation = GetTestFilePath();

            log.WriteEntry("WriteEntry");
            log.WriteEntry("WriteEntry", severity: System.Diagnostics.TraceEventType.Warning);
            log.WriteEntry("WriteEntry", severity: System.Diagnostics.TraceEventType.Error, id: 3);

            log.WriteException(new System.ArgumentException());
            log.WriteException(new System.ArgumentException(), severity: System.Diagnostics.TraceEventType.Warning, additionalInfo: "AdditionalInfo");
            log.WriteException(new System.ArgumentException(), severity: System.Diagnostics.TraceEventType.Warning, additionalInfo: "AdditionalInfo", id: 6);
        }
    }
}
