// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;

namespace System.Windows.Forms.UITests;

internal class DenyExecutionSynchronizationContext : SynchronizationContext
{
    private readonly SynchronizationContext _underlyingContext;
    private readonly Thread _mainThread;
    private readonly StrongBox<ExceptionDispatchInfo> _failedTransfer;

    public DenyExecutionSynchronizationContext(SynchronizationContext underlyingContext)
        : this(underlyingContext, mainThread: null, failedTransfer: null)
    {
    }

    private DenyExecutionSynchronizationContext(SynchronizationContext underlyingContext, Thread? mainThread, StrongBox<ExceptionDispatchInfo>? failedTransfer)
    {
        _underlyingContext = underlyingContext;
        _mainThread = mainThread ?? new Thread(MainThreadStart);
        _failedTransfer = failedTransfer ?? new StrongBox<ExceptionDispatchInfo>();
    }

    internal SynchronizationContext UnderlyingContext => _underlyingContext;

    internal Thread MainThread => _mainThread;

    private static void MainThreadStart() => throw new InvalidOperationException("This thread should never be started.");

    internal void ThrowIfSwitchOccurred()
    {
        if (_failedTransfer.Value is null)
        {
            return;
        }

        _failedTransfer.Value.Throw();
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        try
        {
            if (_failedTransfer.Value is null)
            {
                ThrowFailedTransferExceptionForCapture();
            }
        }
        catch (InvalidOperationException e)
        {
            _failedTransfer.Value = ExceptionDispatchInfo.Capture(e);
        }

#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
        (_underlyingContext ?? new SynchronizationContext()).Post(d, state);
#pragma warning restore VSTHRD001
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        try
        {
            if (_failedTransfer.Value is null)
            {
                ThrowFailedTransferExceptionForCapture();
            }
        }
        catch (InvalidOperationException e)
        {
            _failedTransfer.Value = ExceptionDispatchInfo.Capture(e);
        }

#pragma warning disable VSTHRD001 // Avoid legacy thread switching APIs
        (_underlyingContext ?? new SynchronizationContext()).Send(d, state);
#pragma warning restore VSTHRD001
    }

    public override SynchronizationContext CreateCopy()
    {
        return new DenyExecutionSynchronizationContext(_underlyingContext.CreateCopy(), _mainThread, _failedTransfer);
    }

    private static void ThrowFailedTransferExceptionForCapture()
    {
        throw new InvalidOperationException("Tests cannot use SwitchToMainThreadAsync unless they are marked with ApartmentState.STA.");
    }
}
