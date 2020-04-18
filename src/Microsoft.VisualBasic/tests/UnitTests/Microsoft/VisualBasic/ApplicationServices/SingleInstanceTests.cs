// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.VisualBasic.ApplicationServices.Tests
{
    public class SingleInstanceTests
    {
        private const int SendTimeout = 1000;

        private sealed class ReceivedArgs
        {
            private List<string[]> _received = new List<string[]>();

            internal void Add(string[] args)
            {
                _received.Add(args);
            }

            internal ImmutableArray<string[]> Freeze()
            {
                var received = _received;
                Interlocked.CompareExchange(ref _received, null, received);
                return received.ToImmutableArray();
            }
        }

        // Should be able to test internal methods with [InternalsVisibleTo] rather than reflection.
        private static Type GetHelperType()
        {
            var assembly = typeof(Microsoft.VisualBasic.ApplicationServices.WindowsFormsApplicationBase).Assembly;
            return assembly.GetType("Microsoft.VisualBasic.ApplicationServices.SingleInstanceHelpers");
        }

        // Should be able to test internal methods with [InternalsVisibleTo] rather than reflection.
        private static NamedPipeServerStream CreatePipeServer(string pipeName)
        {
            var method = GetHelperType().GetMethod("CreatePipeServer", BindingFlags.NonPublic | BindingFlags.Static);
            return (NamedPipeServerStream)method.Invoke(null, new object[] { pipeName });
        }

        // Should be able to test internal methods with [InternalsVisibleTo] rather than reflection.
        private static Task WaitForClientConnectionsAsync(NamedPipeServerStream pipeServer, Action<string[]> callback, CancellationToken cancellationToken = default)
        {
            var method = GetHelperType().GetMethod("WaitForClientConnectionsAsync", BindingFlags.NonPublic | BindingFlags.Static);
            return (Task)method.Invoke(null, new object[] { pipeServer, callback, cancellationToken });
        }

        // Should be able to test internal methods with [InternalsVisibleTo] rather than reflection.
        private static bool SendSecondInstanceArgs(string pipeName, int timeout, string[] args)
        {
            var method = GetHelperType().GetMethod("SendSecondInstanceArgs", BindingFlags.NonPublic | BindingFlags.Static);
            return (bool)method.Invoke(null, new object[] { pipeName, timeout, args });
        }

        private static string GetUniqueName() => Guid.NewGuid().ToString();

        [Fact]
        public void MultipleDistinctServers()
        {
            var pipeServers = new List<NamedPipeServerStream>();
            int n = 5;
            try
            {
                for (int i = 0; i < n; i++)
                {
                    var pipeServer = CreatePipeServer(GetUniqueName());
                    Assert.NotNull(pipeServer);
                    pipeServers.Add(pipeServer);
                }
            }
            finally
            {
                foreach (var pipeServer in pipeServers)
                {
                    pipeServer.Dispose();
                }
            }
        }

        [Fact]
        public void MultipleServers_Overlapping()
        {
            var pipeName = GetUniqueName();
            const int n = 10;
            int completed = 0;
            int created = 0;
            var tasks = Enumerable.Range(0, n).Select(i => Task.Factory.StartNew(() =>
            {
                Thread.Sleep(100);
                using (var pipeServer = CreatePipeServer(pipeName))
                {
                    if (pipeServer is { })
                    {
                        Interlocked.Increment(ref created);
                    }
                    Thread.Sleep(10);
                }
                Interlocked.Increment(ref completed);
            })).ToArray();
            Task.WaitAll(tasks);
            Assert.Equal(n, completed);
            Assert.True(created >= 1);
        }

        [Fact]
        public void MultipleClients_Sequential()
        {
            var pipeName = GetUniqueName();
            using (var pipeServer = CreatePipeServer(pipeName))
            {
                const int n = 5;
                var sentArgs = Enumerable.Range(0, n).Select(i => Enumerable.Range(0, i).Select(i => i.ToString()).ToArray()).ToArray();
                var receivedArgs = new ReceivedArgs();
                WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
                for (int i = 0; i < n; i++)
                {
                    Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, sentArgs[i]));
                }
                FlushLastConnection(pipeName);
                Assert.Equal(sentArgs, receivedArgs.Freeze());
            }
        }

        [Fact]
        public void MultipleClients_Overlapping()
        {
            var pipeName = GetUniqueName();
            using (var pipeServer = CreatePipeServer(pipeName))
            {
                const int n = 5;
                var sentArgs = Enumerable.Range(0, n).Select(i => Enumerable.Range(0, i).Select(i => i.ToString()).ToArray()).ToArray();
                var receivedArgs = new ReceivedArgs();
                var tasks = Enumerable.Range(0, n).Select(i => Task.Factory.StartNew(() => { Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, sentArgs[i])); })).ToArray();
                WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
                Task.WaitAll(tasks);
                FlushLastConnection(pipeName);
                var receivedSorted = receivedArgs.Freeze().Sort((x, y) => x.Length - y.Length);
                Assert.Equal(sentArgs, receivedSorted);
            }
        }

        [Fact]
        public void ClientConnectionTimeout()
        {
            var pipeName = GetUniqueName();
            using (var pipeServer = CreatePipeServer(pipeName))
            {
                var task = Task.Factory.StartNew<bool>(() => SendSecondInstanceArgs(pipeName, timeout: 300, Array.Empty<string>()));
                bool result = task.Result;
                Assert.False(result);
            }
        }

        // Corresponds to second instance crash sending incomplete args.
        [Fact]
        public void ClientConnectBeforeWaitForClientConnection()
        {
            var pipeName = GetUniqueName();
            using (var pipeServer = CreatePipeServer(pipeName))
            {
                var receivedArgs = new ReceivedArgs();
                var task = Task.Factory.StartNew<bool>(() => SendSecondInstanceArgs(pipeName, SendTimeout, new[] { "1", "ABC" }));
                // Allow time for connection.
                Thread.Sleep(100);
                WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
                task.Wait();
                FlushLastConnection(pipeName);
                Assert.Equal(new[] { new[] { "1", "ABC" } }, receivedArgs.Freeze());
            }
        }

        // Send data other than string[].
        [Fact]
        public void InvalidClientData()
        {
            var pipeName = GetUniqueName();
            using (var pipeServer = CreatePipeServer(pipeName))
            {
                var receivedArgs = new ReceivedArgs();
                WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);

                sendData(pipeName, new string[0]); // valid
                sendData(pipeName, (int)3); // invalid
                sendData(pipeName, new[] { "ABC" }); // valid
                sendData(pipeName, new int[] { 1, 2, 3 }); // invalid
                sendData(pipeName, new[] { "", "" }); // valid
                sendData(pipeName, new char[] { '1', '2', '3' }); // invalid

                FlushLastConnection(pipeName);

                Assert.Equal(new[] { new string[0], new[] { "ABC" }, new[] { "", "" } }, receivedArgs.Freeze());
            }

            static void sendData<T>(string pipeName, T obj)
            {
                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    pipeClient.Connect();
                    var serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(pipeClient, obj);
                }
            }
        }

        // Corresponds to second instance crash sending incomplete args.
        [Fact]
        public void CloseClientAfterClientConnect()
        {
            var pipeName = GetUniqueName();
            using (var pipeServer = CreatePipeServer(pipeName))
            {
                var receivedArgs = new ReceivedArgs();
                WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);

                // Send valid args.
                Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, new string[0]));
                Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, new[] { "1", "ABC" }));

                // Send bad args: close client after connect.
                closeAfterConnect(pipeName);

                // Send invalid args.
                sendUnexpectedArgs(pipeName);

                // Send valid args.
                Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, new[] { "DEF", "2" }));

                FlushLastConnection(pipeName);

                Assert.Equal(new[] { new string[0], new[] { "1", "ABC" }, new[] { "DEF", "2" } }, receivedArgs.Freeze());
            }

            static void closeAfterConnect(string pipeName)
            {
                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    pipeClient.Connect();
                }
            }

            static void sendUnexpectedArgs(string pipeName)
            {
                using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
                {
                    pipeClient.Connect();
                    pipeClient.Write(new byte[] { 1, 2, 3 }, 0, 3);
                }
            }
        }

        // Corresponds to first instance closing while second instance is sending args.
        [Fact]
        public void CloseServerAfterClientConnect()
        {
            var pipeName = GetUniqueName();
            NamedPipeClientStream pipeClient = null;
            try
            {
                var receivedArgs = new ReceivedArgs();
                using (var pipeServer = CreatePipeServer(pipeName))
                {
                    WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);

                    pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out);
                    pipeClient.Connect();
                }
                Thread.Sleep(500);
                Assert.Empty(receivedArgs.Freeze());
            }
            finally
            {
                if (pipeClient != null)
                {
                    pipeClient.Dispose();
                }
            }
        }

        // Ensure the previous client connection has been consumed completely by the
        // server by creating an extra client connection that closes after connecting.
        private static void FlushLastConnection(string pipeName)
        {
            using (var pipeClient = new NamedPipeClientStream(".", pipeName, PipeDirection.Out))
            {
                pipeClient.Connect();
            }
        }
    }
}
