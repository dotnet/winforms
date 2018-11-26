// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    public sealed class EventHandlerService : IEventHandlerService
    {
        public EventHandlerService(Control focusWnd)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public event EventHandler EventHandlerChanged
        {
            add => throw new NotImplementedException(SR.NotImplementedByDesign);
            remove => throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        public Control FocusWindow { get; }

        /// <summary>
        ///     <para>
        ///         Gets the currently active event handler of the specified type.
        ///     </para>
        /// </summary>
        public object GetHandler(Type handlerType)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>
        ///         Pops
        ///         the given handler off of the stack.
        ///     </para>
        /// </summary>
        public void PopHandler(object handler)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     <para>Pushes a new event handler on the stack.</para>
        /// </summary>
        public void PushHandler(object handler)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }

        /// <summary>
        ///     Fires an OnEventHandlerChanged event.
        /// </summary>
        private void OnEventHandlerChanged(EventArgs e)
        {
            throw new NotImplementedException(SR.NotImplementedByDesign);
        }
    }
}
