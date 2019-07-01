// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;

namespace System.Windows.Forms.Design
{
    public sealed class EventHandlerService : IEventHandlerService
    {
        // We cache the last requested handler for speed.
        //
        private object lastHandler;
        private Type lastHandlerType;

        // The handler stack
        //
        private HandlerEntry handlerHead;

        // Our change event
        //
        private EventHandler changedEvent;

        private readonly Control focusWnd;

        public EventHandlerService(Control focusWnd)
        {
            this.focusWnd = focusWnd;
        }

        public event EventHandler EventHandlerChanged
        {
            add => changedEvent += value;
            remove => changedEvent -= value;
        }

        public Control FocusWindow
        {
            get => focusWnd;
        }

        /// <summary>
        ///     <para>
        ///         Gets the currently active event handler of the specified type.
        ///     </para>
        /// </summary>
        public object GetHandler(Type handlerType)
        {
            if (handlerType == lastHandlerType)
            {
                return lastHandler;
            }

            for (HandlerEntry entry = handlerHead; entry != null; entry = entry.next)
            {
                if (entry.handler != null && handlerType.IsInstanceOfType(entry.handler))
                {
                    lastHandlerType = handlerType;
                    lastHandler = entry.handler;
                    return entry.handler;
                }
            }
            return null;
        }

        /// <summary>
        ///     <para>
        ///         Pops
        ///         the given handler off of the stack.
        ///     </para>
        /// </summary>
        public void PopHandler(object handler)
        {
            for (HandlerEntry entry = handlerHead; entry != null; entry = entry.next)
            {
                if (entry.handler == handler)
                {
                    handlerHead = entry.next;
                    lastHandler = null;
                    lastHandlerType = null;
                    OnEventHandlerChanged(EventArgs.Empty);
                    return;
                }
            }

            Debug.Assert(handler == null || handlerHead == null, "Failed to locate handler to remove from list.");
        }

        /// <summary>
        ///     <para>Pushes a new event handler on the stack.</para>
        /// </summary>
        public void PushHandler(object handler)
        {
            handlerHead = new HandlerEntry(handler, handlerHead);
            // Update the handlerType if the Handler pushed is the same type as the last one ....
            // This is true when SplitContainer is on the form and Edit Properties pushed another handler.
            lastHandlerType = handler.GetType();
            lastHandler = handlerHead.handler;
            OnEventHandlerChanged(EventArgs.Empty);
        }

        /// <summary>
        ///     Fires an OnEventHandlerChanged event.
        /// </summary>
        private void OnEventHandlerChanged(EventArgs e)
        {
            if (changedEvent != null)
            {
                ((EventHandler)changedEvent)(this, e);
            }
        }

        /// <summary>
        ///     Contains a single node of our handler stack.  We typically
        ///     have very few handlers, and the handlers are long-living, so
        ///     I just implemented this as a linked list.
        /// </summary>
        private sealed class HandlerEntry
        {
            public object handler;
            public HandlerEntry next;

            /// <include file='doc\EventHandlerService.uex' path='docs/doc[@for="EventHandlerService.HandlerEntry.HandlerEntry"]/*' />
            /// <devdoc>
            ///     Creates a new handler entry objet.
            /// </devdoc>
            public HandlerEntry(object handler, HandlerEntry next)
            {
                this.handler = handler;
                this.next = next;
            }
        }
    }
}
