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

            // This is done to avoid the effect of the mouse cursor on some tests with invalidates count: https://github.com/dotnet/winforms/pull/7031
            Cursor.Position = new Drawing.Point(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height);

            Application.ThreadException += OnThreadException;
        }

        public virtual void Dispose()
        {
            Application.ThreadException -= OnThreadException;
            //Xunit.Assert.Equal(new Drawing.Point(Screen.PrimaryScreen.WorkingArea.Width, Screen.PrimaryScreen.WorkingArea.Height), Cursor.Position);
        }

        private void OnThreadException(object sender, ThreadExceptionEventArgs e)
        {
            ExceptionDispatchInfo.Capture(e.Exception).Throw();
        }
    }
}
