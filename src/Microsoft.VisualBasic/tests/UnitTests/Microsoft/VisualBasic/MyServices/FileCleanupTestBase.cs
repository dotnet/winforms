// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;

namespace Microsoft.VisualBasic.Tests
{
    public abstract class FileCleanupTestBase : IDisposable
    {
        internal readonly string TestDirectory;

        protected FileCleanupTestBase()
        {
            TestDirectory = Path.Combine(Path.GetTempPath(), GetUniqueName());
            Directory.CreateDirectory(TestDirectory);
        }

        ~FileCleanupTestBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            try
            {
                Directory.Delete(TestDirectory);
            }
            catch
            {
            }
        }

        internal string GetTestFilePath() => Path.Combine(TestDirectory, GetTestFileName());

        internal string GetTestFileName() => GetUniqueName();

        private static string GetUniqueName() => Guid.NewGuid().ToString("D");
    }
}
