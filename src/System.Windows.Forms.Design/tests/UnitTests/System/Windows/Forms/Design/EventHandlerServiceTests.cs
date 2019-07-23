// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Diagnostics;
using WinForms.Common.Tests;
using Xunit;
using static System.Windows.Forms.Design.EventHandlerService;

namespace System.Windows.Forms.Design.Tests
{
    public class EventHandlerServiceTests
    {
        [Fact]
        public void ctor_should_create_object()
        {
            var service = new EventHandlerService(null);

            Assert.NotNull(service);
        }

        [Fact]
        public void ctor_should_set_FocusWindow()
        {
            var service = new EventHandlerService(null);
            Assert.Null(service.FocusWindow);

            var focusWnd = new Control();
            service = new EventHandlerService(focusWnd);
            Assert.Same(focusWnd, service.FocusWindow);
        }

        [Fact]
        public void GetHandler_should_throw_if_handlerType_null()
        {
            var service = new EventHandlerService(null);

            Assert.Throws<ArgumentNullException>(() => service.GetHandler(null));
        }

        [Fact]
        public void GetHandler_should_return_null_if_lastHandlerType_null()
        {
            var service = new EventHandlerService(null);

            Assert.Null(service.GetHandler(typeof(object)));
        }

        [Fact]
        public void GetHandler_should_return_lastHandlerType_if_matches()
        {
            var service = new EventHandlerService(null);
            var handler = new Button();
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            service.GetTestAccessor().LastHandler = handler;

            Assert.Same(handler, service.GetHandler(typeof(Button)));
        }

        [Fact]
        public void GetHandler_should_return_handler_from_stack_if_found()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            var stackHead = CreateStack();
            var handler = stackHead.next.handler;
            service.GetTestAccessor().HandlerHead = stackHead;

            var foundHandler = service.GetHandler(handler.GetType());

            Assert.Same(handler, foundHandler);
        }

        [Fact]
        public void GetHandler_should_set_lastHandlerType_if_handler_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            service.GetTestAccessor().HandlerHead = CreateStack();

            var foundHandler = service.GetHandler(typeof(TextBox));

            Assert.Same(typeof(TextBox), service.GetTestAccessor().LastHandlerType);
        }

        [Fact]
        public void GetHandler_should_set_lastHandler_if_handler_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            var stackHead = CreateStack();
            var handler = stackHead.next.handler;
            service.GetTestAccessor().HandlerHead = stackHead;

            var foundHandler = service.GetHandler(handler.GetType());

            Assert.Same(handler, service.GetTestAccessor().LastHandler);
        }

        [Fact]
        public void GetHandler_should_return_null_if_handler_not_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            service.GetTestAccessor().HandlerHead = CreateStack();

            Assert.Null(service.GetHandler(typeof(ComboBox)));
        }

        [Fact]
        public void PopHandler_should_throw_if_handler_null()
        {
            var service = new EventHandlerService(null);

            Assert.Throws<ArgumentNullException>(() => service.PopHandler(null));
        }

        [Fact]
        public void PopHandler_should_not_throw_if_stack_empty()
        {
            var service = new EventHandlerService(null);

            service.PopHandler(typeof(ComboBox));
        }

        [Fact]
        public void PopHandler_should_not_pop_if_handler_not_found_on_stack()
        {
            // we expect to hit Debug.Assert and unless we clear listeners we will crash to xUnit runner:
            //  "The active test run was aborted. Reason: Test host process crashed : Assertion Failed"
            using (new TraceListenerlessContext())
            {
                var service = new EventHandlerService(null);
                service.GetTestAccessor().LastHandlerType = typeof(Button);
                var stackHead = CreateStack();
                service.GetTestAccessor().HandlerHead = stackHead;

                service.PopHandler(typeof(ComboBox));

                var depth = 0;
                for (HandlerEntry entry = service.GetTestAccessor().HandlerHead; entry != null; entry = entry.next)
                {
                    depth++;
                }

                Assert.Equal(3, depth);
            }
        }

        [Fact]
        public void PopHandler_should_pop_if_handler_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            var stackHead = CreateStack();
            var handler = stackHead.next.handler;
            service.GetTestAccessor().HandlerHead = stackHead;

            service.PopHandler(handler);

            var depth = 0;
            for (HandlerEntry entry = service.GetTestAccessor().HandlerHead; entry != null; entry = entry.next)
            {
                depth++;
            }

            Assert.Equal(1, depth);
        }

        [Fact]
        public void PopHandler_should_set_lastHandler_null_if_handler_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            var stackHead = CreateStack();
            var handler = stackHead.next.handler;
            service.GetTestAccessor().HandlerHead = stackHead;

            service.PopHandler(handler);

            Assert.Null(service.GetTestAccessor().LastHandler);
        }

        [Fact]
        public void PopHandler_should_set_lastHandlerType_null_if_handler_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            var stackHead = CreateStack();
            var handler = stackHead.next.handler;
            service.GetTestAccessor().HandlerHead = stackHead;

            service.PopHandler(handler);

            Assert.Null(service.GetTestAccessor().LastHandlerType);
        }

        [Fact]
        public void PopHandler_should_raise_changedEvent_if_handler_found_on_stack()
        {
            var service = new EventHandlerService(null);
            service.GetTestAccessor().LastHandlerType = typeof(Button);
            var stackHead = CreateStack();
            var handler = stackHead.next.handler;
            service.GetTestAccessor().HandlerHead = stackHead;
            int callCount = 0;
            service.EventHandlerChanged += (s, e) => { callCount++; };

            service.PopHandler(handler);

            Assert.Equal(1, callCount);
        }

        [Fact]
        public void PushHandler_should_throw_if_handler_null()
        {
            var service = new EventHandlerService(null);

            Assert.Throws<ArgumentNullException>(() => service.PushHandler(null));
        }

        [Fact]
        public void PushHandler_should_set_handlerHead_to_new_handler()
        {
            var service = new EventHandlerService(null);
            var handler = new Label();

            service.PushHandler(handler);

            Assert.Same(handler, service.GetTestAccessor().HandlerHead.handler);
        }

        [Fact]
        public void PushHandler_should_set_lastHandlerType_to_new_handler()
        {
            var service = new EventHandlerService(null);
            var handler = new Label();

            service.PushHandler(handler);

            Assert.Same(handler.GetType(), service.GetTestAccessor().LastHandlerType);
        }

        [Fact]
        public void PushHandler_should_set_lastHandler_to_new_handler()
        {
            var service = new EventHandlerService(null);
            var handler = new Label();

            service.PushHandler(handler);

            Assert.Same(handler, service.GetTestAccessor().LastHandler);
        }

        [Fact]
        public void PushHandler_should_raise_changedEvent_for_new_handler()
        {
            var service = new EventHandlerService(null);
            var handler = new Label();
            int callCount = 0;
            service.EventHandlerChanged += (s, e) => { callCount++; };

            service.PushHandler(handler);

            Assert.Equal(1, callCount);
        }

        private HandlerEntry CreateStack()
        {
            var item1 = new HandlerEntry(new Button { Text = "I'm a button" }, null);
            var handler = new TextBox { Text = "I'm a textbox" };
            var item2 = new HandlerEntry(handler, item1);
            var item3 = new HandlerEntry(new Label { Text = "I'm a label" }, item2);

            return item3;
        }
    }
}
