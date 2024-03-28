// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design.Tests;

public class EventHandlerServiceTests
{
    [Fact]
    public void ctor_should_set_FocusWindow()
    {
        EventHandlerService service = new(null);
        Assert.Null(service.FocusWindow);

        using Control focusWnd = new();
        service = new EventHandlerService(focusWnd);
        Assert.Same(focusWnd, service.FocusWindow);
    }

    [Fact]
    public void GetHandler_should_throw_if_handlerType_null()
    {
        EventHandlerService service = new(null);

        Assert.Throws<ArgumentNullException>(() => service.GetHandler(null));
    }

    [Fact]
    public void GetHandler_should_return_null_if_no_handlers()
    {
        EventHandlerService service = new(null);

        Assert.Null(service.GetHandler(typeof(object)));
    }

    [Fact]
    public void GetHandler_should_return_last_inserted_handler_of_type()
    {
        EventHandlerService service = new(null);
        service.PushHandler(new A());

        object second = new A();
        service.PushHandler(second);

        Assert.Same(second, service.GetHandler(typeof(A)));
    }

    [Fact]
    public void GetHandler_should_not_remove_from_handlers()
    {
        EventHandlerService service = new(null);
        A a = new();
        service.PushHandler(a);

        object foundHandler = service.GetHandler(typeof(A));
        Assert.Same(a, foundHandler);
        foundHandler = service.GetHandler(typeof(A));
        Assert.Same(a, foundHandler);
    }

    [Fact]
    public void GetHandler_should_return_derived_handler_if_found()
    {
        EventHandlerService service = new(null);
        A a = new();
        service.PushHandler(a);
        B b = new();
        service.PushHandler(b);

        object foundHandler = service.GetHandler(typeof(A));

        Assert.Same(b, foundHandler);
    }

    [Fact]
    public void GetHandler_should_return_null_if_handler_type_not_found()
    {
        EventHandlerService service = new(null);
        service.PushHandler("Handler");

        // PopHandler asserts when an item isn't found
        using (new NoAssertContext())
        {
            Assert.Null(service.GetHandler(typeof(int)));
        }
    }

    [Fact]
    public void PopHandler_should_throw_if_handler_null()
    {
        EventHandlerService service = new(null);

        Assert.Throws<ArgumentNullException>(() => service.PopHandler(null));
    }

    [Fact]
    public void PopHandler_should_not_throw_if_stack_empty()
    {
        EventHandlerService service = new(null);

        service.PopHandler(typeof(ComboBox));
    }

    [Fact]
    public void PopHandler_should_not_pop_if_handler_not_found_on_stack()
    {
        EventHandlerService service = new(null);
        A a = new();
        service.PushHandler(a);

        // PopHandler asserts when an item isn't found
        using (new NoAssertContext())
        {
            service.PopHandler(new B());
        }

        Assert.Same(a, service.GetHandler(typeof(A)));
    }

    [Fact]
    public void PopHandler_should_pop_if_handler_found_on_stack()
    {
        EventHandlerService service = new(null);
        A a = new();
        service.PushHandler(a);
        service.PopHandler(a);
        Assert.Null(service.GetHandler(typeof(A)));
    }

    [Fact]
    public void PopHandler_should_raise_changedEvent_if_handler_found_on_stack()
    {
        EventHandlerService service = new(null);

        A a = new();
        service.PushHandler(a);

        int callCount = 0;
        service.EventHandlerChanged += (s, e) => { callCount++; };

        service.PopHandler(a);

        Assert.Equal(1, callCount);
    }

    [Fact]
    public void PushHandler_should_throw_if_handler_null()
    {
        EventHandlerService service = new(null);

        Assert.Throws<ArgumentNullException>(() => service.PushHandler(null));
    }

    [Fact]
    public void PushHandler_should_set_handlerHead_to_new_handler()
    {
        EventHandlerService service = new(null);

        A a1 = new();
        service.PushHandler(a1);
        A a2 = new();
        service.PushHandler(a2);

        Assert.Same(a2, service.GetHandler(typeof(A)));
    }

    [Fact]
    public void PushHandler_should_raise_changedEvent_for_new_handler()
    {
        EventHandlerService service = new(null);
        A a = new();
        int callCount = 0;
        service.EventHandlerChanged += (s, e) => { callCount++; };

        service.PushHandler(a);

        Assert.Equal(1, callCount);
    }

    private class A { }
    private class B : A { }
}
