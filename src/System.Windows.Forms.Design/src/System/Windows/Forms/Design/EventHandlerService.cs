// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

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

        // The handler stack
        private HandlerEntry _handlerHead;

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
            if (handlerType == null)
            {
                throw new ArgumentNullException(nameof(handlerType));
            }

            if (_lastHandlerType == null)
            {
                return null;
            }

            if (handlerType == _lastHandlerType)
            {
                return _lastHandler;
            }

            Debug.Assert(_handlerHead != null, "Failed to locate handler to iterate from list.");

            for (HandlerEntry entry = _handlerHead; entry != null; entry = entry.next)
            {
                if (entry.handler != null && handlerType.IsInstanceOfType(entry.handler))
                {
                    _lastHandlerType = handlerType;
                    _lastHandler = entry.handler;
                    return entry.handler;
                }
            }

            return null;
        }

        /// <summary>
        ///  Pops the given handler off of the stack.
        /// </summary>
        public void PopHandler(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            for (HandlerEntry entry = _handlerHead; entry != null; entry = entry.next)
            {
                if (entry.handler == handler)
                {
                    _handlerHead = entry.next;
                    _lastHandler = null;
                    _lastHandlerType = null;
                    OnEventHandlerChanged(EventArgs.Empty);
                    return;
                }
            }

            Debug.Assert(handler == null || _handlerHead == null, "Failed to locate handler to remove from list.");
        }

        /// <summary>
        ///  Pushes a new event handler on the stack.
        /// </summary>
        public void PushHandler(object handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlerHead = new HandlerEntry(handler, _handlerHead);

            // Update the handlerType if the Handler pushed is the same type as the last one ....
            // This is true when SplitContainer is on the form and Edit Properties pushed another handler.
            _lastHandlerType = handler.GetType();
            _lastHandler = _handlerHead.handler;

            OnEventHandlerChanged(EventArgs.Empty);
        }

        /// <summary>
        ///  Fires an OnEventHandlerChanged event.
        /// </summary>
        private void OnEventHandlerChanged(EventArgs e)
        {
            _changedEvent?.Invoke(this, e);
        }

        /// <summary>
        ///  Contains a single node of our handler stack.  We typically
        ///  have very few handlers, and the handlers are long-living, so
        ///  I just implemented this as a linked list.
        /// </summary>
        internal sealed class HandlerEntry
        {
            public object handler;
            public HandlerEntry next;

            /// <summary>
            ///  Creates a new handler entry objet.
            /// </summary>
            public HandlerEntry(object handler, HandlerEntry next)
            {
                this.handler = handler;
                this.next = next;
            }
        }

        internal TestAccessor GetTestAccessor() => new TestAccessor(this);

        internal readonly struct TestAccessor
        {
            private readonly EventHandlerService _service;

            public TestAccessor(EventHandlerService service)
            {
                _service = service;
            }

            public ref object LastHandler => ref _service._lastHandler;

            public ref Type LastHandlerType => ref _service._lastHandlerType;

            public ref HandlerEntry HandlerHead => ref _service._handlerHead;
        }
    }
}
