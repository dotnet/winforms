// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a systematic way to manage event handlers for the current document.
    /// </summary>
    public sealed class EventHandlerService : IEventHandlerService
    {
        // We cache the last requested handler for speed.
        private object _lastHandler;
        private Type _lastHandlerType;
        private EventHandler _changedEvent;

        private readonly LinkedList<object> _handlers = new LinkedList<object>();

        /// <summary>
        ///  Initializes a new instance of the EventHandlerService class.
        /// </summary>
        /// <param name="focusWnd">The <see cref="Control"/> which is being designed.</param>
        public EventHandlerService(Control focusWnd)
        {
            FocusWindow = focusWnd;
        }

        /// <summary>
        ///  Fires an OnEventHandlerChanged event.
        /// </summary>
        public event EventHandler EventHandlerChanged
        {
            add => _changedEvent += value;
            remove => _changedEvent -= value;
        }

        public Control FocusWindow { get; }

        /// <summary>
        ///  Gets the currently active event handler of the specified type.
        /// </summary>
        public object GetHandler(Type handlerType)
        {
            if (handlerType is null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (_lastHandlerType is null)
            {
                return null;
            }

            if (handlerType == _lastHandlerType)
            {
                return _lastHandler;
            }

            Debug.Assert(_handlers.Count > 0, "Should have handlers to look through.");

            object handler = _handlers.FirstOrDefault(h => handlerType.IsInstanceOfType(h));

            if (handler != null)
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
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            var node = _handlers.Find(handler);
            if (node != null)
            {
                _handlers.Remove(node);
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
            if (handler is null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

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
}
