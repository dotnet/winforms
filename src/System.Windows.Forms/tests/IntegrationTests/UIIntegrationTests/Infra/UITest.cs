// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.UI.IntegrationTests.Infra
{
    public static class UITest
    {
        public static async Task WaitForIdleAsync()
        {
            TaskCompletionSource<VoidResult> idleCompletionSource = new();
            Application.Idle += HandleApplicationIdle;

            // Queue an event to make sure we don't stall if the application was already idle
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            await Task.Yield();

            await idleCompletionSource.Task;
            Application.Idle -= HandleApplicationIdle;

            void HandleApplicationIdle(object? sender, EventArgs e)
            {
                idleCompletionSource.TrySetResult(default);
            }
        }

        public static void RunForm<T>(
            Action showForm,
            Func<T, Task> runTestAsync)
            where T : Form
        {
            Assert.Empty(Application.OpenForms.OfType<T>()); // $"{Application.OpenForms.OfType<T>().Count()} open form(s) before test");

            T? form = null;
            try
            {
                // Start runTestAsync before calling showForm, since the latter might block until the form is closed.
                //
                // Avoid using ThreadHelper.JoinableTaskFactory for the outermost operation because we don't want the task
                // tracked by its collection. Otherwise, test code would not be able to wait for pending operations to
                // complete.
                var test = ThreadHelper.JoinableTaskContext.Factory.RunAsync(async () =>
                {
                    await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                    await WaitForIdleAsync();
                    form = Application.OpenForms.OfType<T>().Single();

                    try
                    {
                        await runTestAsync(form);
                    }
                    finally
                    {
                        // Close the form after the test completes. This will unblock the 'showForm()' call if it's
                        // waiting for the form to close.
                        form.Close();

                        // This should be changed to assert no pending operations once background operations are tied
                        // to the life of the owning dialog - issue #7792.
                        await AsyncTestHelper.JoinPendingOperationsAsync(AsyncTestHelper.UnexpectedTimeout);
                    }
                });

                showForm();

                // Join the asynchronous test operation so any exceptions are rethrown on this thread.
                test.Join();
            }
            finally
            {
                form?.Dispose();
                Assert.Empty(Application.OpenForms.OfType<T>()); // $"{Application.OpenForms.OfType<T>().Count()} open form(s) after test");
            }
        }

        public static void RunControl<T>(
            Func<Form, T> createControl,
            Func<T, Task> runTestAsync)
            where T : Control
        {
            T? control = null;
            RunForm<Form>(
                showForm: () =>
                {
                    Form form = new() { Text = $"Test {typeof(T).Name}" };
                    control = createControl(form);
                    if (control is null)
                    {
                        throw new InvalidOperationException();
                    }

                    Assert.True(form.Controls.Contains(control));
                    Application.EnableVisualStyles();
                    Application.Run(form);
                },
                runTestAsync: form => runTestAsync(control!));
        }

        private readonly struct VoidResult
        {
        }
    }
}
