// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

/// <summary>
///  Provides a systematic way to manage event handlers for the current document.
/// </summary>
public sealed class EventHandlerService : IEventHandlerService
{
    // We cache the last requested handler for speed.
    private object? _lastHandler;
    private Type? _lastHandlerType;
    private EventHandler? _changedEvent;

    private readonly LinkedList<object> _handlers = new();

    /// <summary>
    ///  Initializes a new instance of the EventHandlerService class.
    /// </summary>
    /// <param name="focusWnd">The <see cref="Control"/> which is being designed.</param>
    public EventHandlerService(Control? focusWnd)
    {
        FocusWindow = focusWnd;
    }

    /// <summary>
    ///  Fires an OnEventHandlerChanged event.
    /// </summary>
    public event EventHandler? EventHandlerChanged
    {
        add => _changedEvent += value;
        remove => _changedEvent -= value;
    }

    public Control? FocusWindow { get; }

    /// <summary>
    ///  Gets the currently active event handler of the specified type.
    /// </summary>
    public object? GetHandler(Type handlerType)
    {
        ArgumentNullException.ThrowIfNull(handlerType);

        if (_lastHandlerType is null)
        {
            return null;
        }

        if (handlerType == _lastHandlerType)
        {
            return _lastHandler;
        }

        Debug.Assert(_handlers.Count > 0, "Should have handlers to look through.");

        object? handler = _handlers.FirstOrDefault(handlerType.IsInstanceOfType);

        if (handler is not null)
        {
            _lastHandler = handler;
            _lastHandlerType = handlerType;
        }

        return handler;
    }

    /// <summary>
    ///  Pops the given handler off of the stack.
    /// </summary>
    public void PopHandler(object handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        if (_handlers.Remove(handler))
        {
            _lastHandler = null;
            _lastHandlerType = null;
            OnEventHandlerChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  Pushes a new event handler on the stack.
    /// </summary>
    public void PushHandler(object handler)
    {
        ArgumentNullException.ThrowIfNull(handler);

        _handlers.AddFirst(handler);
        _lastHandlerType = handler.GetType();
        _lastHandler = handler;

        OnEventHandlerChanged(EventArgs.Empty);
    }

    /// <summary>
    ///  Fires an OnEventHandlerChanged event.
    /// </summary>
    private void OnEventHandlerChanged(EventArgs e)
    {
        _changedEvent?.Invoke(this, e);
    }
}
