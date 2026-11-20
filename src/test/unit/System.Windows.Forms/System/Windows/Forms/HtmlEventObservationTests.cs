// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Drawing;
using System.Text;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
public class HtmlEventObservationTests
{
    // The page exposes helpers that raise native events carrying populated event objects, so the managed callbacks
    // receive real mouse, keyboard and window data instead of an empty event object.
    private const string EventHtml = """
        <html>
        <head>
            <script>
                function resolveTarget(target)
                {
                    if (target == "document") { return document; }
                    if (target == "window") { return window; }
                    return document.getElementById(target);
                }
                function fire(target, type) { return resolveTarget(target).fireEvent(type); }
                function fireMouse(target, type, button, clientX, clientY, shift, ctrl, alt)
                {
                    var e = document.createEventObject();
                    e.button = button; e.clientX = clientX; e.clientY = clientY;
                    e.shiftKey = shift; e.ctrlKey = ctrl; e.altKey = alt;
                    return resolveTarget(target).fireEvent(type, e);
                }
                function fireKey(target, type, keyCode, shift, ctrl, alt)
                {
                    var e = document.createEventObject();
                    e.keyCode = keyCode; e.shiftKey = shift; e.ctrlKey = ctrl; e.altKey = alt;
                    return resolveTarget(target).fireEvent(type, e);
                }
                function raiseError() { missingFunction(); }
                function raiseDeferredError() { setTimeout(function() { throw new Error("deferred failure"); }, 0); }
            </script>
        </head>
        <body>
            <div id="outer"><button id="inner">Inner</button></div>
            <div style="height: 5000px">Tall content so the window can scroll</div>
        </body>
        </html>
        """;

    private sealed record EventSnapshot(
        object Sender,
        string EventType,
        MouseButtons Buttons,
        Point Client,
        bool NoFromOrToElement,
        int KeyCode,
        bool Shift,
        bool Ctrl,
        bool Alt)
    {
        public static EventSnapshot From(object sender, HtmlElementEventArgs e) =>
            new(
                sender,
                e.EventType,
                e.MouseButtonsPressed,
                e.ClientMousePosition,
                e.FromElement is null && e.ToElement is null,
                e.KeyPressedCode,
                e.ShiftKeyPressed,
                e.CtrlKeyPressed,
                e.AltKeyPressed);
    }

    public static TheoryData<string, int, MouseButtons> MouseEventData => new()
    {
        { "mousedown", 1, MouseButtons.Left },
        { "mouseup", 2, MouseButtons.Right },
        { "mousemove", 4, MouseButtons.Middle },
        { "mouseover", 3, MouseButtons.Left | MouseButtons.Right },
        { "click", 1, MouseButtons.Left },
        { "dblclick", 2, MouseButtons.Right },
    };

    // Scenario: element mouse events deliver the native button, client position, modifier keys and event type to the
    // managed handler, with the element as sender; after the handler is removed nothing is delivered.
    [WinFormsTheory]
    [MemberData(nameof(MouseEventData))]
    public async Task HtmlElement_MouseEvents_PropagateNativeEventData(string eventName, int nativeButton, MouseButtons expectedButtons)
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlElement element = document.GetElementById("inner");
        List<EventSnapshot> events = [];
        HtmlElementEventHandler handler = (sender, e) => events.Add(EventSnapshot.From(sender, e));

        Subscribe(element, eventName, handler, add: true);
        document.InvokeScript("fireMouse", ["inner", $"on{eventName}", nativeButton, 12, 34, true, false, true]);

        EventSnapshot snapshot = Assert.Single(events);
        Assert.Same(element, snapshot.Sender);
        Assert.Equal(eventName, snapshot.EventType);
        Assert.Equal(expectedButtons, snapshot.Buttons);
        Assert.Equal(new Point(12, 34), snapshot.Client);
        Assert.True(snapshot.NoFromOrToElement);
        Assert.True(snapshot.Shift);
        Assert.False(snapshot.Ctrl);
        Assert.True(snapshot.Alt);

        Subscribe(element, eventName, handler, add: false);
        // fireEvent returns false for some events (for example mouseover); a null result means the script call failed.
        Assert.IsType<bool>(document.InvokeScript("fireMouse", ["inner", $"on{eventName}", nativeButton, 12, 34, true, false, true]));
        Assert.Single(events);
    }

    // Scenario: element key events deliver the native key code and Shift/Ctrl/Alt state with the element as sender.
    [WinFormsTheory]
    [InlineData("keydown", 65, true, false, false)]
    [InlineData("keypress", 97, false, true, false)]
    [InlineData("keyup", 13, false, false, true)]
    public async Task HtmlElement_KeyEvents_PropagateNativeEventData(string eventName, int keyCode, bool shift, bool ctrl, bool alt)
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlElement element = document.GetElementById("inner");
        List<EventSnapshot> events = [];
        HtmlElementEventHandler handler = (sender, e) => events.Add(EventSnapshot.From(sender, e));

        Subscribe(element, eventName, handler, add: true);
        document.InvokeScript("fireKey", ["inner", $"on{eventName}", keyCode, shift, ctrl, alt]);

        EventSnapshot snapshot = Assert.Single(events);
        Assert.Same(element, snapshot.Sender);
        Assert.Equal(eventName, snapshot.EventType);
        Assert.Equal(keyCode, snapshot.KeyCode);
        Assert.Equal(shift, snapshot.Shift);
        Assert.Equal(ctrl, snapshot.Ctrl);
        Assert.Equal(alt, snapshot.Alt);

        Subscribe(element, eventName, handler, add: false);
        Assert.Equal(true, document.InvokeScript("fireKey", ["inner", $"on{eventName}", keyCode, shift, ctrl, alt]));
        Assert.Single(events);
    }

    // Scenario: document-level click and mouse-down handlers receive the native mouse data, with the document as sender.
    [WinFormsFact]
    public async Task HtmlDocument_MouseEvents_PropagateNativeEventData()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        List<EventSnapshot> events = [];
        HtmlElementEventHandler handler = (sender, e) => events.Add(EventSnapshot.From(sender, e));
        document.Click += handler;
        document.MouseDown += handler;

        document.InvokeScript("fireMouse", ["document", "onmousedown", 2, 5, 6, false, true, false]);
        document.InvokeScript("fireMouse", ["document", "onclick", 1, 7, 8, false, false, false]);

        Assert.Equal(2, events.Count);
        Assert.All(events, e => Assert.Same(document, e.Sender));
        Assert.Equal("mousedown", events[0].EventType);
        Assert.Equal(MouseButtons.Right, events[0].Buttons);
        Assert.Equal(new Point(5, 6), events[0].Client);
        Assert.True(events[0].Ctrl);
        Assert.Equal("click", events[1].EventType);
        Assert.Equal(MouseButtons.Left, events[1].Buttons);
        Assert.Equal(new Point(7, 8), events[1].Client);

        document.Click -= handler;
        document.MouseDown -= handler;
    }

    // Scenario: a click raised on a child reaches the document handler (bubbling), and the native fireEvent reports
    // the event as not cancelled when no handler changes ReturnValue.
    [WinFormsFact]
    public async Task HtmlDocument_Click_BubblesFromElementAndIsNotCancelledByDefault()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        int documentCalls = 0;
        int elementCalls = 0;
        document.Click += (_, _) => documentCalls++;
        document.GetElementById("inner").Click += (_, _) => elementCalls++;

        object notCancelled = document.InvokeScript("fire", ["inner", "onclick"]);

        Assert.Equal(true, notCancelled);
        Assert.Equal(1, elementCalls);
        Assert.Equal(1, documentCalls);
    }

    // Scenario: setting ReturnValue to false in an element click handler cancels the native event, which the
    // script observes as fireEvent returning false.
    [WinFormsFact]
    public async Task HtmlElement_Click_ReturnValueFalse_CancelsNativeEvent()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlElement element = document.GetElementById("inner");
        bool observedDefault = false;
        HtmlElementEventHandler handler = (_, e) =>
        {
            observedDefault = e.ReturnValue;
            e.ReturnValue = false;
        };
        element.Click += handler;

        object result = document.InvokeScript("fire", ["inner", "onclick"]);

        Assert.True(observedDefault);
        Assert.Equal(false, result);

        element.Click -= handler;
        Assert.Equal(true, document.InvokeScript("fire", ["inner", "onclick"]));
    }

    // Scenario: BubbleEvent defaults to true; setting it to false in the element handler stops the event from
    // reaching the document handler.
    [WinFormsFact]
    public async Task HtmlElement_Click_BubbleEventFalse_StopsPropagationToDocument()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlElement element = document.GetElementById("inner");
        bool bubbleDefault = false;
        int documentCalls = 0;
        element.Click += (_, e) =>
        {
            bubbleDefault = e.BubbleEvent;
            e.BubbleEvent = false;
        };
        document.Click += (_, _) => documentCalls++;

        document.InvokeScript("fire", ["inner", "onclick"]);

        Assert.True(bubbleDefault);
        Assert.Equal(0, documentCalls);
    }

    // Scenario: window resize, scroll, focus and blur events reach the managed handlers with the window as sender and
    // the native event type; after the handler is removed a later occurrence of the same event is no longer delivered
    // to it (a second sentinel handler proves the later event was raised).
    [WinFormsTheory]
    [InlineData("resize")]
    [InlineData("scroll")]
    [InlineData("focus")]
    [InlineData("blur")]
    public async Task HtmlWindow_Events_AreDeliveredWithEventArgs(string eventName)
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlWindow window = document.Window;
        List<EventSnapshot> events = [];
        TaskCompletionSource signal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        HtmlElementEventHandler handler = (sender, e) => events.Add(EventSnapshot.From(sender, e));
        HtmlElementEventHandler sentinel = (_, _) => signal.TrySetResult();
        SubscribeWindow(window, eventName, handler, add: true);
        SubscribeWindow(window, eventName, sentinel, add: true);

        TriggerWindowEvent(control, window, eventName, round: 0);
        await signal.Task.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.NotEmpty(events);
        Assert.All(events, snapshot =>
        {
            Assert.Same(window, snapshot.Sender);
            Assert.Equal(eventName, snapshot.EventType);
        });

        // Native resize can be raised more than once for a single size change, so compare against the count
        // captured at the moment the handler is removed.
        SubscribeWindow(window, eventName, handler, add: false);
        int countAtRemoval = events.Count;
        signal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TriggerWindowEvent(control, window, eventName, round: 1);
        await signal.Task.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.Equal(countAtRemoval, events.Count);
        SubscribeWindow(window, eventName, sentinel, add: false);
    }

    // Scenario: an uncaught script error (raised synchronously or from a timer callback) raises the window Error
    // event with the error description, the document URL and a line number; the event is not marked handled by default.
    [WinFormsTheory]
    [InlineData("raiseError", "missingFunction")]
    [InlineData("raiseDeferredError", "deferred failure")]
    public async Task HtmlWindow_Error_ScriptError_ReportsDescriptionUrlAndLine(string scriptName, string expectedDescription)
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        using TempFile file = TempFile.Create(Encoding.UTF8.GetBytes(EventHtml));
        HtmlDocument document = await GetDocument(control, file);
        HtmlWindow window = document.Window;
        TaskCompletionSource<(string Description, Uri Url, int Line, bool Handled)> source = new(TaskCreationOptions.RunContinuationsAsynchronously);
        HtmlElementErrorEventHandler handler = (sender, e) =>
        {
            bool handledByDefault = e.Handled;
            e.Handled = true;
            source.TrySetResult((e.Description, e.Url, e.LineNumber, handledByDefault));
        };
        window.Error += handler;

        document.InvokeScript(scriptName);
        (string description, Uri url, int line, bool handledByDefault) = await source.Task.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.Contains(expectedDescription, description);
        Assert.Equal(new Uri(file.Path), url);
        Assert.True(line > 0);
        Assert.False(handledByDefault);

        window.Error -= handler;
    }

    // Scenario: handlers attached by event name to a document or element are called through the native event with a
    // null sender and empty event data, once per raise, and are not called after DetachEventHandler.
    [WinFormsTheory]
    [InlineData("document")]
    [InlineData("inner")]
    public async Task AttachEventHandler_RaisedNativeEvent_InvokesHandlerWithEmptyEventArgs(string target)
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        List<(object Sender, EventArgs Args)> calls = [];
        EventHandler handler = (sender, e) => calls.Add((sender, e));
        HtmlElement element = document.GetElementById("inner");
        Action<string, EventHandler> attach = target == "document" ? document.AttachEventHandler : element.AttachEventHandler;
        Action<string, EventHandler> detach = target == "document" ? document.DetachEventHandler : element.DetachEventHandler;

        attach("onclick", handler);
        document.InvokeScript("fire", [target, "onclick"]);

        (object sender, EventArgs args) = Assert.Single(calls);
        Assert.Null(sender);
        Assert.Same(EventArgs.Empty, args);

        detach("onclick", handler);
        Assert.Equal(true, document.InvokeScript("fire", [target, "onclick"]));
        Assert.Single(calls);
    }

    // Scenario: a handler attached by event name to the window is called when the window is resized, with a null
    // sender and empty event data, and is not called for a later resize after DetachEventHandler (a sentinel
    // handler proves the later resize was raised).
    [WinFormsFact]
    public async Task HtmlWindow_AttachEventHandler_Resize_InvokesHandlerWithEmptyEventArgs()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlWindow window = document.Window;
        List<(object Sender, EventArgs Args)> calls = [];
        TaskCompletionSource signal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        EventHandler handler = (sender, e) => calls.Add((sender, e));
        HtmlElementEventHandler sentinel = (_, _) => signal.TrySetResult();
        window.AttachEventHandler("onresize", handler);
        window.Resize += sentinel;

        TriggerWindowEvent(control, window, "resize", round: 0);
        await signal.Task.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.NotEmpty(calls);
        Assert.All(calls, call =>
        {
            Assert.Null(call.Sender);
            Assert.Same(EventArgs.Empty, call.Args);
        });

        window.DetachEventHandler("onresize", handler);
        int countAtDetach = calls.Count;
        signal = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TriggerWindowEvent(control, window, "resize", round: 1);
        await signal.Task.WaitAsync(TimeSpan.FromSeconds(30));

        Assert.Equal(countAtDetach, calls.Count);
        window.Resize -= sentinel;
    }

    // Scenario: a handler that unsubscribes itself while running still completes that delivery and is not called
    // for later events.
    [WinFormsFact]
    public async Task HtmlElement_Click_HandlerUnsubscribesItself_IsNotCalledAgain()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, EventHtml);
        HtmlElement element = document.GetElementById("inner");
        int calls = 0;
        HtmlElementEventHandler handler = null;
        handler = (_, _) =>
        {
            calls++;
            element.Click -= handler;
        };
        element.Click += handler;

        Assert.Equal(true, document.InvokeScript("fire", ["inner", "onclick"]));
        Assert.Equal(true, document.InvokeScript("fire", ["inner", "onclick"]));

        Assert.Equal(1, calls);
    }

    private static void Subscribe(HtmlElement element, string eventName, HtmlElementEventHandler handler, bool add)
    {
        switch (eventName)
        {
            case "mousedown": if (add) { element.MouseDown += handler; } else { element.MouseDown -= handler; } break;
            case "mouseup": if (add) { element.MouseUp += handler; } else { element.MouseUp -= handler; } break;
            case "mousemove": if (add) { element.MouseMove += handler; } else { element.MouseMove -= handler; } break;
            case "mouseover": if (add) { element.MouseOver += handler; } else { element.MouseOver -= handler; } break;
            case "click": if (add) { element.Click += handler; } else { element.Click -= handler; } break;
            case "dblclick": if (add) { element.DoubleClick += handler; } else { element.DoubleClick -= handler; } break;
            case "keydown": if (add) { element.KeyDown += handler; } else { element.KeyDown -= handler; } break;
            case "keypress": if (add) { element.KeyPress += handler; } else { element.KeyPress -= handler; } break;
            case "keyup": if (add) { element.KeyUp += handler; } else { element.KeyUp -= handler; } break;
            default: throw new ArgumentOutOfRangeException(nameof(eventName));
        }
    }

    private static void SubscribeWindow(HtmlWindow window, string eventName, HtmlElementEventHandler handler, bool add)
    {
        switch (eventName)
        {
            case "resize": if (add) { window.Resize += handler; } else { window.Resize -= handler; } break;
            case "scroll": if (add) { window.Scroll += handler; } else { window.Scroll -= handler; } break;
            case "focus": if (add) { window.GotFocus += handler; } else { window.GotFocus -= handler; } break;
            case "blur": if (add) { window.LostFocus += handler; } else { window.LostFocus -= handler; } break;
            default: throw new ArgumentOutOfRangeException(nameof(eventName));
        }
    }

    // Window events are raised by genuine native activity; each round must produce a new occurrence of the event.
    private static void TriggerWindowEvent(WebBrowser control, HtmlWindow window, string eventName, int round)
    {
        switch (eventName)
        {
            case "resize":
                control.Size = new Size(300 + (20 * round), 200 + (20 * round));
                break;
            case "scroll":
                window.ScrollTo(0, 100 * (round + 1));
                break;
            case "focus":
                window.RemoveFocus();
                window.Focus();
                break;
            case "blur":
                window.Focus();
                window.RemoveFocus();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(eventName));
        }
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html)
    {
        using TempFile file = TempFile.Create(Encoding.UTF8.GetBytes(html));
        return await GetDocument(control, file);
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, TempFile file)
    {
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.TrySetResult(true);

        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task.WaitAsync(TimeSpan.FromSeconds(30)));

        return control.Document;
    }
}
