// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System.Text;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
public class HtmlDocumentDomManipulationTests
{
    private const string ScriptedHtml = """
        <html>
        <head>
            <script>
                function readText(id) { var e = document.getElementById(id); return e ? e.innerText : "missing"; }
                function readAttribute(id, name) { var e = document.getElementById(id); return e ? e.getAttribute(name) : "missing"; }
                function readColor(id) { var e = document.getElementById(id); return e ? e.style.color : "missing"; }
                function countTag(tag) { return document.getElementsByTagName(tag).length; }
                function addItem(text)
                {
                    var item = document.createElement("li");
                    item.id = "item" + countTag("li");
                    item.innerText = text;
                    document.getElementById("list").appendChild(item);
                    return item.id;
                }
                function fail() { throw new Error("expected failure"); }
                function returnString() { return "text"; }
                function returnTrue() { return true; }
                function returnInteger() { return 42; }
                function returnDouble() { return 1.5; }
                function returnNull() { return null; }
                function returnUndefined() { }
                function join(a, b, c) { return a + "|" + b + "|" + c; }
            </script>
        </head>
        <body>
            <div id="first" class="duplicate">One</div>
            <div id="second">Two</div>
            <div id="first" class="duplicate">Three</div>
            <ul id="list"></ul>
        </body>
        </html>
        """;

    // Scenario: when two elements share an id, GetElementById consistently returns the first one in document order,
    // and GetElementsByTagName still exposes both.
    [WinFormsFact]
    public async Task HtmlDocument_GetElementById_DuplicateIds_ReturnsFirstElementInDocumentOrder()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        HtmlElement first = document.GetElementById("first");
        Assert.NotNull(first);
        Assert.Equal("One", first.InnerText);
        Assert.Equal("One", document.GetElementById("first").InnerText);

        int duplicateCount = 0;
        foreach (HtmlElement div in document.GetElementsByTagName("div"))
        {
            if (div.Id == "first")
            {
                duplicateCount++;
            }
        }

        Assert.Equal(2, duplicateCount);
    }

    // Scenario: a missing id yields null, and the "*" tag name returns every element in the native document.
    [WinFormsFact]
    public async Task HtmlDocument_GetElementsByTagName_Wildcard_ReturnsAllElements()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        Assert.Null(document.GetElementById("missing"));

        HtmlElementCollection all = document.GetElementsByTagName("*");
        Assert.Equal((int)document.InvokeScript("countTag", ["*"]), all.Count);
        Assert.Contains(all.Cast<HtmlElement>(), e => e.TagName == "BODY");
        Assert.Contains(all.Cast<HtmlElement>(), e => e.TagName == "UL");
    }

    // Scenario: an element made with CreateElement is detached (no parent, not found by id) until it is appended,
    // after which it is discoverable by id and tag name and visible to the document's script.
    [WinFormsFact]
    public async Task HtmlDocument_CreateElement_DetachedUntilAppended_BecomesVisibleToDocument()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        int divCount = document.GetElementsByTagName("div").Count;
        HtmlElement element = document.CreateElement("div");
        element.Id = "created";
        element.InnerText = "Created text";
        element.SetAttribute("data-kind", "detached");

        Assert.Null(element.Parent);
        Assert.Null(document.GetElementById("created"));
        Assert.Equal(divCount, document.GetElementsByTagName("div").Count);
        Assert.Equal("missing", document.InvokeScript("readText", ["created"]));
        Assert.Equal("detached", element.GetAttribute("data-kind"));

        document.Body.AppendChild(element);

        HtmlElement found = document.GetElementById("created");
        Assert.NotNull(found);
        Assert.Equal("Created text", found.InnerText);
        Assert.Equal(divCount + 1, document.GetElementsByTagName("div").Count);
        Assert.Equal("Created text", document.InvokeScript("readText", ["created"]));
        Assert.Equal("detached", document.InvokeScript("readAttribute", ["created", "data-kind"]));
    }

    // Scenario: managed changes to attributes, InnerText, InnerHtml and Style on a live element are visible
    // to script running in the native document.
    [WinFormsFact]
    public async Task HtmlElement_ManagedChanges_AreVisibleToNativeScript()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        HtmlElement element = document.GetElementById("second");
        element.SetAttribute("title", "Managed title");
        element.InnerText = "Managed text";
        element.Style = "color: red";

        Assert.Equal("Managed title", document.InvokeScript("readAttribute", ["second", "title"]));
        Assert.Equal("Managed text", document.InvokeScript("readText", ["second"]));
        Assert.Equal("red", document.InvokeScript("readColor", ["second"]));

        element.InnerHtml = "<span id=\"inner\">Replaced</span>";
        Assert.Equal("Replaced", document.InvokeScript("readText", ["inner"]));
        Assert.Equal("Replaced", document.GetElementById("inner").InnerText);
    }

    // Scenario: changing an element's id attribute updates what GetElementById finds, and replacing InnerHtml
    // removes the previous child elements from lookups.
    [WinFormsFact]
    public async Task HtmlElement_IdChangeAndInnerHtmlReplacement_UpdateLookups()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        HtmlElement second = document.GetElementById("second");
        second.SetAttribute("id", "renamed");

        Assert.Null(document.GetElementById("second"));
        Assert.Equal("Two", document.GetElementById("renamed").InnerText);

        document.GetElementById("list").InnerHtml = "<li id=\"a\">A</li><li id=\"b\">B</li>";
        Assert.Equal(2, document.GetElementsByTagName("li").Count);
        document.GetElementById("list").InnerHtml = "<li id=\"c\">C</li>";
        Assert.Null(document.GetElementById("a"));
        Assert.Null(document.GetElementById("b"));
        Assert.Equal("C", document.GetElementById("c").InnerText);
        Assert.Single(document.GetElementsByTagName("li"));
    }

    // Scenario: DOM changes made by script in the native document are visible through the managed wrappers.
    [WinFormsFact]
    public async Task HtmlDocument_ScriptChanges_AreVisibleToManagedWrappers()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        HtmlElementCollection items = document.GetElementsByTagName("li");
        Assert.Empty(items);

        Assert.Equal("item0", document.InvokeScript("addItem", ["Added by script"]));
        Assert.Equal("item1", document.InvokeScript("addItem", ["Second"]));

        Assert.Equal(2, document.GetElementsByTagName("li").Count);
        Assert.Equal("Added by script", document.GetElementById("item0").InnerText);
        Assert.Equal("Second", document.GetElementById("item1").InnerText);
        Assert.Equal("list", document.GetElementById("item0").Parent.Id);
    }

    // Scenario: InvokeScript converts native script results (string, boolean, integer, double, null, undefined)
    // to the expected managed values.
    [WinFormsTheory]
    [InlineData("returnString", "text")]
    [InlineData("returnTrue", true)]
    [InlineData("returnInteger", 42)]
    [InlineData("returnDouble", 1.5)]
    [InlineData("returnNull", typeof(DBNull))]
    [InlineData("returnUndefined", null)]
    public async Task HtmlDocument_InvokeScript_ReturnValue_ConvertsToManagedValue(string scriptName, object expected)
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        object result = document.InvokeScript(scriptName);
        if (expected is Type type)
        {
            Assert.IsType(type, result);
        }
        else
        {
            Assert.Equal(expected, result);
        }
    }

    // Scenario: arguments of mixed managed types reach the script in natural left-to-right order.
    [WinFormsFact]
    public async Task HtmlDocument_InvokeScript_MixedArguments_ArrivesInOrder()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        Assert.Equal("text|true|7", document.InvokeScript("join", ["text", true, 7]));
    }

    // Scenario: a script that throws is reported as a null result and leaves the document usable.
    [WinFormsFact]
    public async Task HtmlDocument_InvokeScript_ScriptThrows_ReturnsNullAndDocumentStaysUsable()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);

        Assert.Null(document.InvokeScript("fail"));
        Assert.Equal("text", document.InvokeScript("returnString"));
        Assert.Equal("One", document.GetElementById("first").InnerText);
    }

    // Scenario: after the browser navigates to a new document, that document has independent content and does not
    // expose the previous document's elements or scripts.
    [WinFormsFact]
    public async Task HtmlDocument_AfterNavigation_NewDocumentIsIndependentOfPreviousContent()
    {
        using Control parent = new();
        using WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument first = await GetDocument(control, ScriptedHtml);
        Assert.NotNull(first.GetElementById("second"));

        HtmlDocument second = await GetDocument(control, "<html><body><p id=\"only\">Only</p></body></html>");

        Assert.Null(second.GetElementById("second"));
        Assert.Equal("Only", second.GetElementById("only").InnerText);
        Assert.Null(second.InvokeScript("readText", ["second"]));
    }

    // Scenario: once the browser is disposed, WebBrowser.Document throws ObjectDisposedException; earlier DOM
    // wrappers keep their own native references and stay usable, but script invocation returns null.
    [WinFormsFact]
    public async Task HtmlDocument_AfterBrowserDisposed_OperationsHaveExplicitOutcome()
    {
        using Control parent = new();
        WebBrowser control = new() { Parent = parent, ScriptErrorsSuppressed = true };
        HtmlDocument document = await GetDocument(control, ScriptedHtml);
        HtmlElement element = document.GetElementById("second");
        Assert.NotNull(element);

        control.Dispose();

        Assert.Throws<ObjectDisposedException>(() => control.Document);

        // Wrappers obtained earlier hold their own native references, so they remain usable after disposal.
        Assert.Equal("Two", document.GetElementById("second").InnerText);
        Assert.Null(document.InvokeScript("returnString"));
        element.InnerText = "Changed";
        Assert.Equal("Changed", element.InnerText);
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html)
    {
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.TrySetResult(true);

        using var file = TempFile.Create(Encoding.UTF8.GetBytes(html));
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task.WaitAsync(TimeSpan.FromSeconds(30)));

        return control.Document;
    }
}
