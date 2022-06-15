// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.ExceptionServices;
using System.Windows.Forms;

namespace System
{
    public class ThreadExceptionFixture : IDisposable
    {
        public ThreadExceptionFixture()
        {
            Application.EnableVisualStyles();

            Application.ThreadException += OnThreadException;
        }

        public virtual void Dispose()
        {
            Application.ThreadException -= OnThreadException;
        }

        private void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ExceptionDispatchInfo.Capture(e.Exception).Throw();
        }
    }
}
