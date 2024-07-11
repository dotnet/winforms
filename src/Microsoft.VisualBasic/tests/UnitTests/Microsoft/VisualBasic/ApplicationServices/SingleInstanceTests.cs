// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Immutable;
using System.IO.Pipes;
using System.Runtime.Serialization;
using System.Text;

namespace Microsoft.VisualBasic.ApplicationServices.Tests;

public class SingleInstanceTests
{
    private const int SendTimeout = 10000;

    private sealed class ReceivedArgs
    {
        private List<string[]> _received = [];

        internal void Add(string[] args)
        {
            _received.Add(args);
        }

        internal ImmutableArray<string[]> Freeze()
        {
            var received = _received;
            Interlocked.CompareExchange(ref _received, null, received);
            return [.. received];
        }
    }

    private readonly dynamic _testAccessor = GetTestHelper();

    private static dynamic GetTestHelper()
    {
        var assembly = typeof(WindowsFormsApplicationBase).Assembly;
        var type = assembly.GetType("Microsoft.VisualBasic.ApplicationServices.SingleInstanceHelpers");
        return type.TestAccessor().Dynamic;
    }

    private bool TryCreatePipeServer(string pipeName, out NamedPipeServerStream pipeServer)
    {
        return _testAccessor.TryCreatePipeServer(pipeName, out pipeServer);
    }

    private Task WaitForClientConnectionsAsync(NamedPipeServerStream pipeServer, Action<string[]> callback, CancellationToken cancellationToken = default)
    {
        return _testAccessor.WaitForClientConnectionsAsync(pipeServer, callback, cancellationToken);
    }

    private Task SendSecondInstanceArgsAsync(string pipeName, string[] args, CancellationToken cancellationToken)
    {
        return _testAccessor.SendSecondInstanceArgsAsync(pipeName, args, cancellationToken);
    }

    private bool SendSecondInstanceArgs(string pipeName, int timeout, string[] args)
    {
        CancellationTokenSource tokenSource = new();
        tokenSource.CancelAfter(timeout);
        try
        {
            var awaitable = SendSecondInstanceArgsAsync(pipeName, args, tokenSource.Token).ConfigureAwait(false);
            awaitable.GetAwaiter().GetResult();
        }
        catch (Exception)
        {
            return false;
        }

        return true;
    }

    private static string GetUniqueName() => Guid.NewGuid().ToString();

    [Fact]
    public void MultipleDistinctServers()
    {
        List<NamedPipeServerStream> pipeServers = [];
        int n = 5;
        try
        {
            for (int i = 0; i < n; i++)
            {
                Assert.True(TryCreatePipeServer(GetUniqueName(), out var pipeServer));
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
    public async Task MultipleServers_Overlapping()
    {
        string pipeName = GetUniqueName();
        const int n = 10;
        int completed = 0;
        int created = 0;
        var tasks = Enumerable.Range(0, n).Select(i => Task.Factory.StartNew(() =>
        {
            Thread.Sleep(100);
            if (TryCreatePipeServer(pipeName, out var pipeServer))
            {
                Interlocked.Increment(ref created);
            }

            using (pipeServer)
            {
                Thread.Sleep(10);
            }

            Interlocked.Increment(ref completed);
        }, cancellationToken: default, creationOptions: default, scheduler: TaskScheduler.Default)).ToArray();
        await Task.WhenAll(tasks);
        Assert.Equal(n, completed);
        Assert.True(created >= 1);
    }

    [Fact]
    public void MultipleClients_Sequential()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            const int n = 5;
            string[][] sentArgs = Enumerable.Range(0, n).Select(i => Enumerable.Range(0, i).Select(i => i.ToString()).ToArray()).ToArray();
            ReceivedArgs receivedArgs = new();
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
    public async Task MultipleClients_Overlapping()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            const int n = 5;
            string[][] sentArgs = Enumerable.Range(0, n).Select(i => Enumerable.Range(0, i).Select(i => i.ToString()).ToArray()).ToArray();
            ReceivedArgs receivedArgs = new();
            _ = WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
            var tasks = Enumerable.Range(0, n).Select(i => Task.Factory.StartNew(() => { Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, sentArgs[i])); }, cancellationToken: default, creationOptions: default, scheduler: TaskScheduler.Default)).ToArray();
            await Task.WhenAll(tasks);
            FlushLastConnection(pipeName);
            var receivedSorted = receivedArgs.Freeze().Sort((x, y) => x.Length - y.Length);
            Assert.Equal(sentArgs, receivedSorted);
        }
    }

    // Message that exceeds the buffer size in SingleInstanceHelpers.ReadArgsAsync.
    [Fact]
    public void ManyArgs()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            string[] expectedArgs = getStrings(20000).ToArray();
            ReceivedArgs receivedArgs = new();
            WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
            Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, expectedArgs));
            FlushLastConnection(pipeName);
            string[] actualArgs = receivedArgs.Freeze().Single();
            Assert.Equal(expectedArgs, actualArgs);
        }

        static IEnumerable<string> getStrings(int maxTotalLength)
        {
            Random r = new();
            int n = 0;
            while (n < maxTotalLength)
            {
                string str = getString(r);
                n += str.Length;
                yield return str;
            }
        }

        static string getString(Random r)
        {
            int n = r.Next(1000);
            StringBuilder builder = new();
            for (int i = 0; i < n; i++)
            {
                builder.Append((char)('a' + r.Next(26)));
            }

            return builder.ToString();
        }
    }

    [Fact]
    public async Task ClientConnectionTimeout()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            var task = Task.Factory.StartNew(() => SendSecondInstanceArgs(pipeName, timeout: 300, []), cancellationToken: default, creationOptions: default, scheduler: TaskScheduler.Default);
            bool result = await task;
            Assert.False(result);
        }
    }

    // Corresponds to second instance crash sending incomplete args.
    [Fact]
    public async Task ClientConnectBeforeWaitForClientConnection()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            ReceivedArgs receivedArgs = new();
            var task = Task.Factory.StartNew(() => SendSecondInstanceArgs(pipeName, SendTimeout, ["1", "ABC"]), cancellationToken: default, creationOptions: default, scheduler: TaskScheduler.Default);
            // Allow time for connection.
            Thread.Sleep(100);
            _ = WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
            await task;
            FlushLastConnection(pipeName);
            Assert.Equal(new[] { new[] { "1", "ABC" } }, receivedArgs.Freeze());
        }
    }

    // Send data other than string[].
    [Fact]
    public void InvalidClientData()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            ReceivedArgs receivedArgs = new();
            WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);

            sendData(pipeName, Array.Empty<string>()); // valid
            sendData(pipeName, 3); // invalid
            sendData(pipeName, new[] { "ABC" }); // valid
            sendData(pipeName, new int[] { 1, 2, 3 }); // invalid
            sendData(pipeName, new[] { "", "" }); // valid
            sendData(pipeName, new char[] { '1', '2', '3' }); // invalid

            FlushLastConnection(pipeName);

            Assert.Equal(new[] { (string[])[], ["ABC"], ["", ""] }, receivedArgs.Freeze());
        }

        static void sendData<T>(string pipeName, T obj)
        {
            var pipeClient = CreateClientConnection(pipeName, SendTimeout);
            if (pipeClient is null)
            {
                return;
            }

            using (pipeClient)
            {
                DataContractSerializer serializer = new(typeof(T));
                serializer.WriteObject(pipeClient, obj);
            }
        }
    }

    // Corresponds to second instance crash sending incomplete args.
    [Fact]
    public void CloseClientAfterClientConnect()
    {
        string pipeName = GetUniqueName();
        Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
        using (pipeServer)
        {
            ReceivedArgs receivedArgs = new();
            WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);

            // Send valid args.
            Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, []));
            Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, ["1", "ABC"]));

            // Send bad args: close client after connect.
            closeAfterConnect(pipeName);

            // Send invalid args.
            sendUnexpectedArgs(pipeName);

            // Send valid args.
            Assert.True(SendSecondInstanceArgs(pipeName, SendTimeout, ["DEF", "2"]));

            FlushLastConnection(pipeName);

            Assert.Equal(new[] { (string[])[], ["1", "ABC"], ["DEF", "2"] }, receivedArgs.Freeze());
        }

        static void closeAfterConnect(string pipeName)
        {
            using var pipeClient = CreateClientConnection(pipeName, SendTimeout);
        }

        static void sendUnexpectedArgs(string pipeName)
        {
            var pipeClient = CreateClientConnection(pipeName, SendTimeout);
            if (pipeClient is null)
            {
                return;
            }

            using (pipeClient)
            {
                pipeClient.Write([1, 2, 3], 0, 3);
            }
        }
    }

    // Corresponds to first instance closing while second instance is sending args.
    [Fact]
    public void CloseServerAfterClientConnect()
    {
        string pipeName = GetUniqueName();
        NamedPipeClientStream pipeClient = null;
        try
        {
            ReceivedArgs receivedArgs = new();
            Assert.True(TryCreatePipeServer(pipeName, out var pipeServer));
            using (pipeServer)
            {
                WaitForClientConnectionsAsync(pipeServer, receivedArgs.Add);
                pipeClient = CreateClientConnection(pipeName, SendTimeout);
            }

            Thread.Sleep(500);
            Assert.Empty(receivedArgs.Freeze());
        }
        finally
        {
            pipeClient?.Dispose();
        }
    }

    // Ensure the previous client connection has been consumed completely by the
    // server by creating an extra client connection that closes after connecting.
    private static void FlushLastConnection(string pipeName)
    {
        using var _ = CreateClientConnection(pipeName, SendTimeout);
    }

    private static NamedPipeClientStream CreateClientConnection(string pipeName, int timeout)
    {
        NamedPipeClientStream pipeClient = new(".", pipeName, PipeDirection.Out);
        try
        {
            pipeClient.Connect(timeout);
        }
        catch (TimeoutException)
        {
            pipeClient.Dispose();
            return null;
        }

        return pipeClient;
    }
}
