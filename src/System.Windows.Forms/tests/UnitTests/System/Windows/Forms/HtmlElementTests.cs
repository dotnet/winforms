// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using Windows.Win32.System.Variant;
using Windows.Win32.Web.MsHtml;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
public class HtmlElementTests
{
    [WinFormsFact]
    public async Task HtmlElement_All_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElementCollection collection = element.All;
        Assert.NotSame(collection, element.All);
        Assert.Equal(3, collection.Count);
        Assert.Equal("H1", collection[0].TagName);
        Assert.Equal("STRONG", collection[1].TagName);
        Assert.Equal("H2", collection[2].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_All_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElementCollection collection = element.All;
        Assert.NotSame(collection, element.All);
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public async Task HtmlElement_Document_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        HtmlDocument result = element.Document;
        Assert.NotSame(result, element.Document);
        Assert.NotSame(document, result);
        Assert.Equal(document.Body.InnerHtml, result.Body.InnerHtml);
    }

    [WinFormsFact]
    public async Task HtmlElement_CanHaveChildren_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.True(element.CanHaveChildren);
    }

    [WinFormsFact]
    public async Task HtmlElement_CanHaveChildren_GetCantHave_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><img id=\"id\" /></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.False(element.CanHaveChildren);
    }

    [WinFormsFact]
    public async Task HtmlElement_ClientRectangle_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Rectangle result = element.ClientRectangle;
        Assert.Equal(Rectangle.Empty, result);
    }

    [WinFormsFact]
    public async Task HtmlElement_Children_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElementCollection collection = element.Children;
        Assert.NotSame(collection, element.Children);
        Assert.Equal(2, collection.Count);
        Assert.Equal("H1", collection[0].TagName);
        Assert.Equal("H2", collection[1].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_Children_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElementCollection collection = element.Children;
        Assert.NotSame(collection, element.Children);
        Assert.Empty(collection);
    }

    [WinFormsFact]
    public async Task HtmlElement_DomElement_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        object domElement = element.DomElement;
        Assert.Same(domElement, element.DomElement);
        Assert.True(domElement.GetType().IsCOMObject);
        Assert.True(domElement is IHTMLDOMNode.Interface);
        Assert.True(domElement is IHTMLElement.Interface);
        Assert.True(domElement is IHTMLElement2.Interface);
        Assert.True(domElement is IHTMLElement3.Interface);
    }

    [WinFormsFact]
    public async Task HtmlElement_Enabled_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.True(element.Enabled);
    }

    [WinFormsTheory]
    [InlineData("true", false)]
    [InlineData("false", false)]
    [InlineData("Nothing", false)]
    public async Task HtmlElement_Enabled_GetCustomValueOnAttribute_ReturnsExpected(string value, bool expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        string html = $"<html><body><div disabled={value} id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal(expected, element.Enabled);
    }

    [WinFormsTheory]
    [BoolData]
    public async Task HtmlElement_Enabled_GetCustomValueSet_ReturnsExpected(bool disabled)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            Assert.True(iHTMLElement3.Value->put_disabled(disabled).Succeeded);
            Assert.Equal(!disabled, element.Enabled);
        }
    }

    [WinFormsTheory]
    [BoolData]
    public async Task HtmlElement_Enabled_Set_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        string html = $"<html><body><div disabled={value} id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, html);
        HtmlElement element = document.GetElementById("id");

        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            element.Enabled = value;
            Assert.Equal(value, element.Enabled);
            VARIANT_BOOL disabled;
            Assert.True(iHTMLElement3.Value->get_disabled(&disabled).Succeeded);
            Assert.Equal(!value, (bool)disabled);

            // Set same.
            element.Enabled = value;
            Assert.Equal(value, element.Enabled);
            VARIANT_BOOL disabled2;
            Assert.True(iHTMLElement3.Value->get_disabled(&disabled2).Succeeded);
            Assert.Equal(!value, (bool)disabled2);

            // Set different.
            element.Enabled = !value;
            Assert.Equal(!value, element.Enabled);
            VARIANT_BOOL disabled3;
            Assert.True(iHTMLElement3.Value->get_disabled(&disabled3).Succeeded);
            Assert.Equal(value, (bool)disabled3);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_FirstChild_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Null(element.FirstChild);
    }

    [WinFormsFact]
    public async Task HtmlElement_FirstChild_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2><h3>Header3</h3></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        HtmlElement result = element.FirstChild;
        Assert.NotSame(result, element.FirstChild);
        Assert.Equal("H1", result.TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_Id_GetWithoutId_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body;
        Assert.Null(element.Id);
    }

    [WinFormsFact]
    public async Task HtmlElement_Id_GetOnAttribute_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal("id", element.Id);
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("id", "id")]
    public async Task HtmlElement_Id_GetCustomValueSet_ReturnsExpected(string id, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR bstrId = new(id);
            Assert.True(iHTMLElement.Value->put_id(bstrId).Succeeded);
            Assert.Equal(expected, element.Id);
        }
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("id", "id")]
    public async Task HtmlElement_Id_Set_GetReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            element.Id = value;
            Assert.Equal(expected, element.Id);
            using BSTR id = default;
            Assert.True(iHTMLElement.Value->get_id(&id).Succeeded);
            Assert.Equal(expected, id.ToString());

            // Set same.
            element.Id = value;
            Assert.Equal(expected, element.Id);
            using BSTR id2 = default;
            Assert.True(iHTMLElement.Value->get_id(&id2).Succeeded);
            Assert.Equal(expected, id2.ToString());
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerHtml_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Null(element.InnerHtml);
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerHtml_GetOnAttribute_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal("<P>InnerText</P>", element.InnerHtml);
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("InnerText", "InnerText")]
    [InlineData("<p>InnerText</p>", "<P>InnerText</P>")]
    public async Task HtmlElement_InnerHtml_GetCustomValueSet_ReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR innerHtml = new(value);
            Assert.True(iHTMLElement.Value->put_innerHTML(innerHtml).Succeeded);
            Assert.Equal(expected, element.InnerHtml);
        }
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("InnerText", "InnerText")]
    [InlineData("<p>InnerText</p>", "<P>InnerText</P>")]
    public async Task HtmlElement_InnerHtml_Set_GetReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            element.InnerHtml = value;
            Assert.Equal(expected, element.InnerHtml);
            using BSTR innerHtml = default;
            Assert.True(iHTMLElement.Value->get_innerHTML(&innerHtml).Succeeded);
            Assert.Equal(expected, innerHtml.ToString());

            // Set same.
            element.InnerHtml = value;
            Assert.Equal(expected, element.InnerHtml);
            using BSTR innerHtml2 = default;
            Assert.True(iHTMLElement.Value->get_innerHTML(&innerHtml2).Succeeded);
            Assert.Equal(expected, innerHtml.ToString());
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerHtml_SetCantHaveChildren_ThrowsNotSupportedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><img id=\"id\" /></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<NotSupportedException>(() => element.InnerHtml = "InnerText");
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerHtml_SetDocumentElement_ThrowsNotSupportedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body.Parent;
        Assert.Throws<NotSupportedException>(() => element.InnerHtml = "InnerHtml");
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerText_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Null(element.InnerText);
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerText_GetWithInnerText_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p> <h1>MoreText</h1></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal($"InnerText{Environment.NewLine}MoreText", element.InnerText);
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("InnerText", "InnerText")]
    [InlineData("<p>InnerText</p>", "InnerText")]
    public async Task HtmlElement_InnerText_GetCustomValueSet_ReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR innerHtml = new(value);
            Assert.True(iHTMLElement.Value->put_innerHTML(innerHtml).Succeeded);
            Assert.Equal(expected, element.InnerText);
        }
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("InnerText", "InnerText")]
    [InlineData("<p>InnerText</p>", "<p>InnerText</p>")]
    public async Task HtmlElement_InnerText_Set_GetReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            element.InnerText = value;
            Assert.Equal(expected, element.InnerText);
            using BSTR innerText = default;
            Assert.True(iHTMLElement.Value->get_innerText(&innerText).Succeeded);
            Assert.Equal(expected, innerText.ToString());

            // Set same.
            element.InnerText = value;
            Assert.Equal(expected, element.InnerText);
            using BSTR innerText2 = default;
            Assert.True(iHTMLElement.Value->get_innerText(&innerText2).Succeeded);
            Assert.Equal(expected, innerText2.ToString());
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerText_SetCantHaveChildren_ThrowsNotSupportedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><img id=\"id\" /></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<NotSupportedException>(() => element.InnerText = "InnerText");
    }

    [WinFormsFact]
    public async Task HtmlElement_InnerText_SetDocumentElement_ThrowsNotSupportedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body.Parent;
        Assert.Throws<NotSupportedException>(() => element.InnerText = "InnerText");
    }

    [WinFormsFact]
    public async Task HtmlElement_Name_GetWithoutName_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body;
        Assert.Empty(element.Name);
    }

    [WinFormsFact]
    public async Task HtmlElement_Name_GetOnAttribute_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\" name=\"name\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal("name", element.Name);
    }

    [WinFormsTheory]
    [NormalizedStringData]
    public async Task HtmlElement_Name_GetCustomValueSet_ReturnsExpected(string id, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR name = new("name");
            using var variantId = (VARIANT)id;
            iHTMLElement.Value->setAttribute(name, variantId, 0);
            Assert.Equal(expected, element.Name);
        }
    }

    [WinFormsTheory]
    [InlineData(null, "", null)]
    [InlineData("", "", null)]
    [InlineData("value", "value", "value")]
    public async Task HtmlElement_Name_Set_GetReturnsExpected(string value, string expected, string expectedNative)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR name = new("name");

            element.Name = value;
            Assert.Equal(expected, element.Name);
            Assert.True(iHTMLElement.Value->getAttribute(name, 0, out VARIANT attribute).Succeeded);
            Assert.Equal(expectedNative, attribute.ToObject());
            attribute.Dispose();

            // Set same.
            element.Name = value;
            Assert.Equal(expected, element.Name);
            Assert.True(iHTMLElement.Value->getAttribute(name, 0, out attribute).Succeeded);
            Assert.Equal(expectedNative, attribute.ToObject());
            attribute.Dispose();
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_NextSibling_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Null(element.NextSibling);
    }

    [WinFormsFact]
    public async Task HtmlElement_NextSibling_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div><h1 id=\"id\">Header1 <strong>Strong</strong></h1><h2>Header2</h2><h3>Header3</h3></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        HtmlElement result = element.NextSibling;
        Assert.NotSame(result, element.NextSibling);
        Assert.Equal("H2", result.TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_OffsetParent_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement result = element.OffsetParent;
        Assert.NotSame(result, element.OffsetParent);
        Assert.Equal("BODY", result.TagName);
        Assert.Null(result.OffsetParent);
    }

    [WinFormsFact]
    public async Task HtmlElement_OffsetParent_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2><h3>Header3</h3></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        HtmlElement result = element.OffsetParent;
        Assert.NotSame(result, element.OffsetParent);
        Assert.Equal("BODY", result.TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_OffsetRectangle_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Rectangle result = element.OffsetRectangle;
        Assert.NotEqual(Rectangle.Empty, result);
    }

    [WinFormsFact]
    public async Task HtmlElement_OuterHtml_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal($"{Environment.NewLine}<DIV id=id></DIV>", element.OuterHtml);
    }

    [WinFormsFact]
    public async Task HtmlElement_OuterHtml_GetOnAttribute_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>OuterText</p></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal($"{Environment.NewLine}<DIV id=id><P>OuterText</P></DIV>", element.OuterHtml);
    }

    public static IEnumerable<object[]> OuterHtml_CustomValueSet_TestData()
    {
        yield return new object[] { null, $"{Environment.NewLine}<DIV id=id></DIV>" };
        yield return new object[] { "", $"{Environment.NewLine}<DIV id=id></DIV>" };
        yield return new object[] { "OuterText", $"{Environment.NewLine}<DIV id=id>OuterText</DIV>" };
        yield return new object[] { "<p>OuterText</p>", $"{Environment.NewLine}<DIV id=id><P>OuterText</P></DIV>" };
    }

    [WinFormsTheory]
    [MemberData(nameof(OuterHtml_CustomValueSet_TestData))]
    public async Task HtmlElement_OuterHtml_GetCustomValueSet_ReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR innerHtml = new(value);
            Assert.True(iHTMLElement.Value->put_innerHTML(innerHtml).Succeeded);
            Assert.Equal(expected, element.OuterHtml);
        }
    }

    public static IEnumerable<object[]> OuterHtml_Set_TestData()
    {
        yield return new object[] { null, $"{Environment.NewLine}<DIV id=id></DIV>" };
        yield return new object[] { "", $"{Environment.NewLine}<DIV id=id></DIV>" };
        yield return new object[] { "OuterText", $"{Environment.NewLine}<DIV id=id></DIV>" };
        yield return new object[] { "<p>OuterText</p>", $"{Environment.NewLine}<DIV id=id></DIV>" };
    }

    [WinFormsTheory]
    [MemberData(nameof(OuterHtml_Set_TestData))]
    public async Task HtmlElement_OuterHtml_Set_GetReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            element.OuterHtml = value;
            Assert.Equal(expected, element.OuterHtml);
            using BSTR outerHTML = default;
            Assert.True(iHTMLElement.Value->get_outerHTML(&outerHTML).Succeeded);
            Assert.Equal(expected, outerHTML.ToString());

            // Set same.
            element.OuterHtml = value;
            Assert.Equal(expected, element.OuterHtml);
            using BSTR outerHTML2 = default;
            Assert.True(iHTMLElement.Value->get_outerHTML(&outerHTML2).Succeeded);
            Assert.Equal(expected, outerHTML2.ToString());
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_OuterHtml_SetDocumentElement_ThrowsNotSupportedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body.Parent;
        Assert.Throws<NotSupportedException>(() => element.OuterHtml = "OuterHtml");
    }

    [WinFormsFact]
    public async Task HtmlElement_OuterText_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Null(element.OuterText);
    }

    [WinFormsFact]
    public async Task HtmlElement_OuterText_GetWithOuterText_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>OuterText</p> <h1>MoreText</h1></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal($"OuterText{Environment.NewLine}MoreText", element.OuterText);
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("OuterText", "OuterText")]
    [InlineData("<p>OuterText</p>", "OuterText")]
    public async Task HtmlElement_OuterText_GetCustomValueSet_ReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR innerHtml = new(value);
            Assert.True(iHTMLElement.Value->put_innerHTML(innerHtml).Succeeded);
            Assert.Equal(expected, element.OuterText);
        }
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("OuterText", null)]
    [InlineData("<p>OuterText</p>", null)]
    public async Task HtmlElement_OuterText_Set_GetReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            element.OuterText = value;
            Assert.Equal(expected, element.OuterText);
            using BSTR outerText = default;
            Assert.True(iHTMLElement.Value->get_outerText(&outerText).Succeeded);
            Assert.Equal(expected, outerText.ToString());

            // Set same.
            element.OuterText = value;
            Assert.Equal(expected, element.OuterText);
            using BSTR outerText2 = default;
            Assert.True(iHTMLElement.Value->get_outerText(&outerText2).Succeeded);
            Assert.Equal(expected, outerText2.ToString());
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_OuterText_SetDocumentElement_ThrowsNotSupportedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body.Parent;
        Assert.Throws<NotSupportedException>(() => element.OuterText = "OuterText");
    }

    [WinFormsFact]
    public async Task HtmlElement_Parent_GetEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement result = element.Parent;
        Assert.NotSame(result, element.Parent);
        Assert.Equal("BODY", result.TagName);

        HtmlElement resultParent = result.Parent;
        Assert.Equal("HTML", resultParent.TagName);
        Assert.Null(resultParent.Parent);
    }

    [WinFormsFact]
    public async Task HtmlElement_Parent_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2><h3>Header3</h3></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        HtmlElement result = element.Parent;
        Assert.NotSame(result, element.Parent);
        Assert.Equal("BODY", result.TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_ScrollLeft_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal(0, element.ScrollLeft);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task HtmlElement_ScrollLeft_GetCustomValueSet_ReturnsExpected(int value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement2 = ComHelpers.GetComScope<IHTMLElement2>(element.DomElement);
            Assert.True(iHTMLElement2.Value->put_scrollLeft(value).Succeeded);
            Assert.Equal(0, element.ScrollLeft);
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task HtmlElement_ScrollLeft_Set_GetReturnsExpected(int value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement2 = ComHelpers.GetComScope<IHTMLElement2>(element.DomElement);
            element.ScrollLeft = value;
            Assert.Equal(0, element.ScrollLeft);
            int scrollLeft;
            Assert.True(iHTMLElement2.Value->get_scrollLeft(&scrollLeft).Succeeded);
            Assert.Equal(0, scrollLeft);

            // Set same.
            element.ScrollLeft = value;
            Assert.Equal(0, element.ScrollLeft);
            Assert.True(iHTMLElement2.Value->get_scrollLeft(&scrollLeft).Succeeded);
            Assert.Equal(0, scrollLeft);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_ScrollRectangle_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Rectangle result = element.ScrollRectangle;
        Assert.NotEqual(Rectangle.Empty, result);
        Assert.Equal(element.ScrollLeft, result.X);
        Assert.Equal(element.ScrollTop, result.Y);
    }

    [WinFormsFact]
    public async Task HtmlElement_ScrollTop_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal(0, element.ScrollTop);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task HtmlElement_ScrollTop_GetCustomValueSet_ReturnsExpected(int value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement2 = ComHelpers.GetComScope<IHTMLElement2>(element.DomElement);
            Assert.True(iHTMLElement2.Value->put_scrollTop(value).Succeeded);
            Assert.Equal(0, element.ScrollTop);
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task HtmlElement_ScrollTop_Set_GetReturnsExpected(int value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement2 = ComHelpers.GetComScope<IHTMLElement2>(element.DomElement);
            element.ScrollTop = value;
            Assert.Equal(0, element.ScrollTop);
            int scrollTop;
            Assert.True(iHTMLElement2.Value->get_scrollTop(&scrollTop).Succeeded);
            Assert.Equal(0, scrollTop);

            // Set same.
            element.ScrollTop = value;
            Assert.Equal(0, element.ScrollTop);
            Assert.True(iHTMLElement2.Value->get_scrollTop(&scrollTop).Succeeded);
            Assert.Equal(0, scrollTop);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_Style_GetWithoutStyle_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body;
        Assert.Null(element.Style);
    }

    [WinFormsTheory]
    [InlineData("", null)]
    [InlineData("style", null)]
    [InlineData("name:value", "name: value")]
    [InlineData("name1:value1;name2:value2", "name1: value1; name2: value2")]
    public async Task HtmlElement_Style_GetOnAttribute_ReturnsExpected(string style, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        string html = $"<html><body><div id=\"id\" style=\"{style}\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal(expected, element.Style);
    }

    [WinFormsTheory]
    [InlineData("", null)]
    [InlineData("style", null)]
    [InlineData("name:value", "name: value")]
    [InlineData("name1:value1;name2:value2", "name1: value1; name2: value2")]
    public async Task HtmlElement_Style_GetCustomValueSet_ReturnsExpected(string style, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using ComScope<IHTMLStyle> htmlStyle = new(null);
            Assert.True(iHTMLElement.Value->get_style(htmlStyle).Succeeded);
            using BSTR bstrStyle = new(style);
            Assert.True(htmlStyle.Value->put_cssText(bstrStyle).Succeeded);
            Assert.Equal(expected, element.Style);
        }
    }

    [WinFormsTheory]
    [InlineData(null, null)]
    [InlineData("", null)]
    [InlineData("value", null)]
    [InlineData("name:value", "name: value")]
    [InlineData("name1:value1;name2:value2", "name1: value1; name2: value2")]
    public async Task HtmlElement_Style_Set_GetReturnsExpected(string value, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        element.Style = value;
        Assert.Equal(expected, element.Style);
        Assert.Equal(expected, GetStyleCssText());

        // Set same.
        element.Style = value;
        Assert.Equal(expected, element.Style);
        Assert.Equal(expected, GetStyleCssText());

        unsafe string GetStyleCssText()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using ComScope<IHTMLStyle> htmlStyle = new(null);
            Assert.True(iHTMLElement.Value->get_style(htmlStyle).Succeeded);
            using BSTR cssText = default;
            Assert.True(htmlStyle.Value->get_cssText(&cssText).Succeeded);
            return cssText.ToString();
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_TabIndex_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal(0, element.TabIndex);
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task HtmlElement_TabIndex_GetCustomValueSet_ReturnsExpected(short value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement2 = ComHelpers.GetComScope<IHTMLElement2>(element.DomElement);
            Assert.True(iHTMLElement2.Value->put_tabIndex(value).Succeeded);
            Assert.Equal(value, element.TabIndex);
        }
    }

    [WinFormsTheory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    public async Task HtmlElement_TabIndex_Set_GetReturnsExpected(short value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        validate();
        unsafe void validate()
        {
            using var iHTMLElement2 = ComHelpers.GetComScope<IHTMLElement2>(element.DomElement);
            element.TabIndex = value;
            Assert.Equal(value, element.TabIndex);
            short tabIndex;
            Assert.True(iHTMLElement2.Value->get_tabIndex(&tabIndex).Succeeded);
            Assert.Equal(value, tabIndex);

            // Set same.
            element.TabIndex = value;
            Assert.Equal(value, element.TabIndex);
            Assert.True(iHTMLElement2.Value->get_tabIndex(&tabIndex).Succeeded);
            Assert.Equal(value, tabIndex);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_TagName_Get_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal("DIV", element.TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_AppendChild_InvokeEmpty_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.AppendChild(newElement1);
        Assert.Single(element.All);
        Assert.Equal("H1", element.All[0].TagName);

        element.AppendChild(newElement2);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("H1", element.All[0].TagName);
        Assert.Equal("H2", element.All[1].TagName);

        element.AppendChild(newElement3);
        Assert.Equal(3, element.All.Count);
        Assert.Equal("H1", element.All[0].TagName);
        Assert.Equal("H2", element.All[1].TagName);
        Assert.Equal("H3", element.All[2].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_AppendChild_InvokeNotEmptyEmpty_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p><strong>StrongText</strong></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.AppendChild(newElement1);
        Assert.Equal(3, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);

        element.AppendChild(newElement2);
        Assert.Equal(4, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Equal("H2", element.All[3].TagName);

        element.AppendChild(newElement3);
        Assert.Equal(5, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Equal("H2", element.All[3].TagName);
        Assert.Equal("H3", element.All[4].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_AppendChild_NullNewElement_ThrowsNullReferenceException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<NullReferenceException>(() => element.AppendChild(null));
    }

    [WinFormsTheory]
    [InlineData("eventName")]
    [InlineData("onclick")]
    public async Task HtmlElement_AttachEventHandler_AttachDetach_Success(string eventName)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        element.AttachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);

        // Attach again.
        element.AttachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);

        document.DetachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData("onclick", true)]
    [InlineData("ondblclick", true)]
    [InlineData("onkeypress", true)]
    [InlineData("onkeydown", true)]
    [InlineData("onkeyup", true)]
    [InlineData("onmouseover", false)]
    [InlineData("onmousemove", true)]
    [InlineData("onmousedown", true)]
    [InlineData("onmouseup", true)]
    [InlineData("onmouseenter", true)]
    [InlineData("onmouseleave", true)]
    [InlineData("onerrorupdate", true)]
    [InlineData("onfocus", true)]
    [InlineData("ondrag", true)]
    [InlineData("ondragend", true)]
    [InlineData("ondragleave", true)]
    [InlineData("ondragover", true)]
    [InlineData("onfocusin", true)]
    [InlineData("onfocusout", true)]
    [InlineData("onblur", true)]
    [InlineData("onresizestart", true)]
    [InlineData("onhelp", true)]
    [InlineData("onselectstart", true)]
    [InlineData("ondragstart", true)]
    [InlineData("onbeforeupdate", true)]
    [InlineData("onrowexit", true)]
    [InlineData("ondragenter", true)]
    [InlineData("ondrop", true)]
    [InlineData("onbeforecut", true)]
    [InlineData("oncut", true)]
    [InlineData("onbeforecopy", true)]
    [InlineData("oncopy", true)]
    [InlineData("onbeforepaste", true)]
    [InlineData("onpaste", true)]
    [InlineData("oncontextmenu", true)]
    [InlineData("onbeforedeactivate", true)]
    [InlineData("onbeforeactivate", true)]
    [InlineData("oncontrolselect", true)]
    [InlineData("onmovestart", true)]
    [InlineData("onmousewheel", true)]
    public async Task HtmlElement_AttachEventHandler_InvokeNormalElement_Success(string eventName, bool expectedResult)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.NotSame(document, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        element.AttachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);
        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR name = new(eventName);
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(name, &eventObj, &cancelled).Succeeded);
            Assert.Equal(expectedResult, (bool)cancelled);
            Assert.Equal(1, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsTheory]
    [InlineData("onsubmit")]
    [InlineData("onreset")]
    public async Task HtmlElement_AttachEventHandler_InvokeForm_Success(string eventName)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><form id=\"id\"></form></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.NotSame(document, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        element.AttachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);

        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);

            using BSTR name = new(eventName);
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(name, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsTheory]
    [InlineData("onchange")]
    public async Task HtmlElement_AttachEventHandler_InvokeSelect_Success(string eventName)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><select id=\"id\"></select></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.NotSame(document, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        }

        element.AttachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);
        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR name = new(eventName);
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(name, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_AttachEventHandler_EmptyEventName_ThrowsCOMException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        COMException ex = Assert.Throws<COMException>(() => element.AttachEventHandler(string.Empty, handler));
        Assert.Equal(HRESULT.DISP_E_UNKNOWNNAME, (HRESULT)ex.HResult);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public async Task HtmlElement_AttachEventHandler_NullEventName_ThrowsArgumentException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        element.AttachEventHandler(null, handler);
        Assert.Equal(0, callCount);
    }

    [WinFormsTheory]
    [InlineData("eventName")]
    [InlineData("onclick")]
    public async Task HtmlElement_DetachEventHandler_AttachDetach_Success(string eventName)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;

        element.DetachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);

        element.DetachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);

        element.DetachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);

        // Detach again.
        element.DetachEventHandler(eventName, handler);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public async Task HtmlElement_DetachEventHandler_EmptyEventName_ThrowsCOMException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        element.DetachEventHandler(string.Empty, handler);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public async Task HtmlElement_DetachEventHandler_NullEventName_ThrowsArgumentException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        element.DetachEventHandler(null, handler);
        Assert.Equal(0, callCount);
    }

    [WinFormsFact]
    public async Task HtmlElement_Equals_Invoke_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element1 = document.GetElementById("id1");
        HtmlElement element2 = document.GetElementById("id1");
        HtmlElement element3 = document.GetElementById("id2");

        Assert.True(element1.Equals(element1));
        Assert.True(element1.Equals(element2));
        Assert.False(element1.Equals(element3));
        Assert.False(element1.Equals(new object()));
        Assert.False(element1.Equals(null));
    }

    [WinFormsFact]
    public async Task HtmlElement_Focus_Invoke_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        element.Focus();

        // Call again.
        element.Focus();
    }

    [WinFormsFact]
    public async Task HtmlElement_GetAttribute_InvokeEmpty_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body;
        Assert.Empty(element.GetAttribute("NoSuchAttribute"));
        Assert.Empty(element.GetAttribute(""));
    }

    [WinFormsFact]
    public async Task HtmlElement_GetAttribute_InvokeCustomAttributeSet_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.Body;
        setup();
        unsafe void setup()
        {
            using var iHTMLElement = ComHelpers.GetComScope<IHTMLElement>(element.DomElement);
            using BSTR id = new("id");
            using var attribute = (VARIANT)"id";
            iHTMLElement.Value->setAttribute(id, attribute, 1);
            using BSTR customAttribute = new("customAttribute");
            using var value = (VARIANT)"value";
            iHTMLElement.Value->setAttribute(customAttribute, value, 1);
        }

        Assert.Equal("id", element.GetAttribute("id"));
        Assert.Equal("value", element.GetAttribute("customAttribute"));
        Assert.Equal("value", element.GetAttribute("CUSTOMATTRIBUTE"));
        Assert.Empty(element.GetAttribute("noValue"));
        Assert.Empty(element.GetAttribute("NoSuchAttribute"));
        Assert.Empty(element.GetAttribute(""));
    }

    [WinFormsFact]
    public async Task HtmlElement_GetAttribute_InvokeWithAttributes_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\" customAttribute=\"value\" noValue=\"\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Equal("id", element.GetAttribute("id"));
        Assert.Equal("value", element.GetAttribute("customAttribute"));
        Assert.Equal("value", element.GetAttribute("CUSTOMATTRIBUTE"));
        Assert.Empty(element.GetAttribute("noValue"));
        Assert.Empty(element.GetAttribute("NoSuchAttribute"));
        Assert.Empty(element.GetAttribute(""));
    }

    [WinFormsFact]
    public async Task HtmlElement_GetAttribute_NullAttributeName_ThrowsArgumentException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<ArgumentException>(() => element.GetAttribute(null));
    }

    [WinFormsFact]
    public async Task HtmlElement_GetElementsByTagName_Invoke_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><img id=\"img1\" /><img id=\"img2\" /><a id=\"link1\">Href</a><a id=\"link2\">Href</a><form id=\"form1\"></form><form id=\"form2\"></form></div><form id=\"form3\"></form></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        HtmlElementCollection collection = element.GetElementsByTagName("form");
        Assert.NotSame(collection, element.GetElementsByTagName("form"));
        Assert.Equal(2, collection.Count);
        Assert.Equal("FORM", collection[0].TagName);
        Assert.Equal("form1", collection[0].GetAttribute("id"));
        Assert.Equal("FORM", collection[1].TagName);
        Assert.Equal("form2", collection[1].GetAttribute("id"));

        Assert.Empty(element.GetElementsByTagName("NoSuchTagName"));
        Assert.Empty(element.GetElementsByTagName(""));
    }

    [WinFormsFact]
    public async Task HtmlElement_GetElementsByTagName_NullTagName_ThrowsArgumentException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<ArgumentException>(() => element.GetElementsByTagName(null));
    }

    [WinFormsFact]
    public async Task HtmlElement_GetHashCode_Invoke_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element1 = document.GetElementById("id1");
        HtmlElement element2 = document.GetElementById("id1");
        HtmlElement element3 = document.GetElementById("id2");

        Assert.True(element1 == element2);
        Assert.True(element2 != element3);
        Assert.NotEqual(0, element1.GetHashCode());
        Assert.Equal(element1.GetHashCode(), element1.GetHashCode());
        Assert.Equal(element1.GetHashCode(), element2.GetHashCode());
        Assert.NotEqual(element1.GetHashCode(), element3.GetHashCode());
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeEmptyBeforeBegin_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, newElement1);
        Assert.Empty(element.All);
        Assert.Equal(2, element.Parent.Children.Count);
        Assert.Equal("H1", element.Parent.Children[0].TagName);
        Assert.Equal("DIV", element.Parent.Children[1].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, newElement2);
        Assert.Empty(element.All);
        Assert.Equal(3, element.Parent.Children.Count);
        Assert.Equal("H1", element.Parent.Children[0].TagName);
        Assert.Equal("H2", element.Parent.Children[1].TagName);
        Assert.Equal("DIV", element.Parent.Children[2].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, newElement3);
        Assert.Empty(element.All);
        Assert.Equal(4, element.Parent.Children.Count);
        Assert.Equal("H1", element.Parent.Children[0].TagName);
        Assert.Equal("H2", element.Parent.Children[1].TagName);
        Assert.Equal("H3", element.Parent.Children[2].TagName);
        Assert.Equal("DIV", element.Parent.Children[3].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeNotEmptyBeforeBegin_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p><strong>StrongText</strong></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, newElement1);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal(2, element.Parent.Children.Count);
        Assert.Equal("H1", element.Parent.Children[0].TagName);
        Assert.Equal("DIV", element.Parent.Children[1].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, newElement2);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal(3, element.Parent.Children.Count);
        Assert.Equal("H1", element.Parent.Children[0].TagName);
        Assert.Equal("H2", element.Parent.Children[1].TagName);
        Assert.Equal("DIV", element.Parent.Children[2].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeBegin, newElement3);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal(4, element.Parent.Children.Count);
        Assert.Equal("H1", element.Parent.Children[0].TagName);
        Assert.Equal("H2", element.Parent.Children[1].TagName);
        Assert.Equal("H3", element.Parent.Children[2].TagName);
        Assert.Equal("DIV", element.Parent.Children[3].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeEmptyBeforeEnd_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement1);
        Assert.Single(element.All);
        Assert.Equal("H1", element.All[0].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement2);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("H1", element.All[0].TagName);
        Assert.Equal("H2", element.All[1].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement3);
        Assert.Equal(3, element.All.Count);
        Assert.Equal("H1", element.All[0].TagName);
        Assert.Equal("H2", element.All[1].TagName);
        Assert.Equal("H3", element.All[2].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeNotEmptyBeforeEnd_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p><strong>StrongText</strong></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement1);
        Assert.Equal(3, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement2);
        Assert.Equal(4, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Equal("H2", element.All[3].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, newElement3);
        Assert.Equal(5, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Equal("H2", element.All[3].TagName);
        Assert.Equal("H3", element.All[4].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeEmptyAfterBegin_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, newElement1);
        Assert.Single(element.All);
        Assert.Equal("H1", element.All[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, newElement2);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("H2", element.All[0].TagName);
        Assert.Equal("H1", element.All[1].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, newElement3);
        Assert.Equal(3, element.All.Count);
        Assert.Equal("H3", element.All[0].TagName);
        Assert.Equal("H2", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeNotEmptyAfterBegin_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p><strong>StrongText</strong></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, newElement1);
        Assert.Equal(3, element.All.Count);
        Assert.Equal("H1", element.All[0].TagName);
        Assert.Equal("P", element.All[1].TagName);
        Assert.Equal("STRONG", element.All[2].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, newElement2);
        Assert.Equal(4, element.All.Count);
        Assert.Equal("H2", element.All[0].TagName);
        Assert.Equal("H1", element.All[1].TagName);
        Assert.Equal("P", element.All[2].TagName);
        Assert.Equal("STRONG", element.All[3].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterBegin, newElement3);
        Assert.Equal(5, element.All.Count);
        Assert.Equal("H3", element.All[0].TagName);
        Assert.Equal("H2", element.All[1].TagName);
        Assert.Equal("H1", element.All[2].TagName);
        Assert.Equal("P", element.All[3].TagName);
        Assert.Equal("STRONG", element.All[4].TagName);
        Assert.Single(element.Parent.Children);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeEmptyAfterEnd_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, newElement1);
        Assert.Equal(2, element.Parent.Children.Count);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
        Assert.Equal("H1", element.Parent.Children[1].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, newElement2);
        Assert.Equal(3, element.Parent.Children.Count);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
        Assert.Equal("H2", element.Parent.Children[1].TagName);
        Assert.Equal("H1", element.Parent.Children[2].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, newElement3);
        Assert.Equal(4, element.Parent.Children.Count);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
        Assert.Equal("H3", element.Parent.Children[1].TagName);
        Assert.Equal("H2", element.Parent.Children[2].TagName);
        Assert.Equal("H1", element.Parent.Children[3].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_InvokeNotEmptyAfterEnd_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><p>InnerText</p><strong>StrongText</strong></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement1 = document.CreateElement("h1");
        HtmlElement newElement2 = document.CreateElement("h2");
        HtmlElement newElement3 = document.CreateElement("h3");

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, newElement1);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal(2, element.Parent.Children.Count);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
        Assert.Equal("H1", element.Parent.Children[1].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, newElement2);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal(3, element.Parent.Children.Count);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
        Assert.Equal("H2", element.Parent.Children[1].TagName);
        Assert.Equal("H1", element.Parent.Children[2].TagName);

        element.InsertAdjacentElement(HtmlElementInsertionOrientation.AfterEnd, newElement3);
        Assert.Equal(2, element.All.Count);
        Assert.Equal("P", element.All[0].TagName);
        Assert.Equal("STRONG", element.All[1].TagName);
        Assert.Equal(4, element.Parent.Children.Count);
        Assert.Equal("DIV", element.Parent.Children[0].TagName);
        Assert.Equal("H3", element.Parent.Children[1].TagName);
        Assert.Equal("H2", element.Parent.Children[2].TagName);
        Assert.Equal("H1", element.Parent.Children[3].TagName);
    }

    [WinFormsFact]
    public async Task HtmlElement_InsertAdjacentElement_NullNewElement_ThrowsNullReferenceException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"><h1>Header1 <strong>Strong</strong></h1><h2>Header2</h2></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<NullReferenceException>(() => element.InsertAdjacentElement(HtmlElementInsertionOrientation.BeforeEnd, null));
    }

    [WinFormsTheory]
    [InvalidEnumData<HtmlElementInsertionOrientation>]
    public async Task HtmlElement_InsertAdjacentElement_InvalidOrient_ThrowsArgumentException(HtmlElementInsertionOrientation orient)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        HtmlElement newElement = document.CreateElement("H1");
        Assert.Throws<ArgumentException>(() => element.InsertAdjacentElement(orient, newElement));
    }

    [WinFormsFact]
    public async Task HtmlElement_InvokeMember_MemberExists_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><body><p id=\"target\" attribute=\"value\">Paragraph</p></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("target");
        Assert.NotNull(element);

        Assert.Equal("value", element.InvokeMember("getAttribute", "attribute"));
        Assert.Equal("value", element.InvokeMember("getAttribute", "attribute", 1));
        Assert.Equal(Convert.DBNull, element.InvokeMember("getAttribute", "ATTRIBUTE", 1));
        Assert.Equal(Convert.DBNull, element.InvokeMember("getAttribute", "NoSuchAttribute"));
        Assert.Null(element.InvokeMember("getAttribute", default(TimeSpan)));
    }

    [WinFormsFact]
    public async Task HtmlElement_InvokeMember_NoSuchMember_ReturnsNull()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><body><p id=\"target\" attribute=\"value\">Paragraph</p></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("target");
        Assert.NotNull(element);

        Assert.Null(element.InvokeMember("NoSuchMember"));
        Assert.Null(element.InvokeMember("NoSuchMember", null));
        Assert.Null(element.InvokeMember("NoSuchMember", Array.Empty<object>()));
        Assert.Null(element.InvokeMember("NoSuchMember", [1]));
    }

    [WinFormsFact]
    public async Task HtmlElement_RemoveFocus_Invoke_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        element.RemoveFocus();

        // Call again.
        element.RemoveFocus();
    }

    [WinFormsFact]
    public async Task HtmlElement_RemoveFocus_InvokeFocused_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        element.Focus();

        element.RemoveFocus();

        // Call again.
        element.RemoveFocus();
    }

    [WinFormsTheory]
    [BoolData]
    public async Task HtmlElement_ScrollIntoView_Invoke_Success(bool alignWithTop)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        element.ScrollIntoView(alignWithTop);

        // Call again.
        element.ScrollIntoView(alignWithTop);
    }

    [WinFormsTheory]
    [StringData]
    public async Task HtmlElement_SetAttribute_Invoke_GetAttributeReturnsExpected(string value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        element.SetAttribute("customAttribute", value);
        Assert.Equal(value, element.GetAttribute("customAttribute"));

        // Set same.
        element.SetAttribute("customAttribute", value);
        Assert.Equal(value, element.GetAttribute("customAttribute"));

        // Override.
        element.SetAttribute("customAttribute", "newValue");
        Assert.Equal("newValue", element.GetAttribute("customAttribute"));
    }

    [WinFormsFact]
    public async Task HtmlElement_SetAttribute_NullAttributeName_ThrowsArgumentException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        Assert.Throws<ArgumentException>(() => element.SetAttribute(null, "value"));
    }

    [WinFormsFact]
    public async Task HtmlElement_SetAttribute_EmptyAttributeName_ThrowsArgumentException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");
        COMException ex = Assert.Throws<COMException>(() => element.SetAttribute(string.Empty, "value"));
        Assert.Equal(HRESULT.DISP_E_UNKNOWNNAME, (HRESULT)ex.HResult);
    }

#pragma warning disable CS1718, CSIsNull001, CSIsNull002 // Disable "Comparison made to same variable" warning.
    [WinFormsFact]
    public async Task HtmlElement_OperatorEquals_Invoke_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element1 = document.GetElementById("id1");
        HtmlElement element2 = document.GetElementById("id1");
        HtmlElement element3 = document.GetElementById("id2");

        Assert.True(element1 == element1);
        Assert.True(element1 == element2);
        Assert.False(element1 == element3);
        Assert.NotNull(element1);
        Assert.NotNull(element1);
        Assert.Null((HtmlElement)null);
    }

    [WinFormsFact]
    public async Task HtmlElement_OperatorNotEquals_Invoke_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element1 = document.GetElementById("id1");
        HtmlElement element2 = document.GetElementById("id1");
        HtmlElement element3 = document.GetElementById("id2");

        Assert.False(element1 != element1);
        Assert.False(element1 != element2);
        Assert.True(element1 != element3);
        Assert.NotNull(element1);
        Assert.NotNull(element1);
        Assert.Null((HtmlElement)null);
    }
#pragma warning restore CS1718, CSIsNull001, CSIsNull002

    [WinFormsFact]
    public async Task HtmlElement_Click_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            element.Click += handler;
            using BSTR onClick = new("onclick");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onClick, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.Click -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onClick, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_DoubleClick_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            element.DoubleClick += handler;
            using BSTR ondblclick = new("ondblclick");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled;
            Assert.True(iHTMLElement3.Value->fireEvent(ondblclick, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.DoubleClick -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(ondblclick, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_Drag_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            element.Drag += handler;
            using BSTR onDrag = new("ondrag");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onDrag, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.Drag -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onDrag, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_DragEnd_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            element.DragEnd += handler;
            using BSTR onDragEnd = new("ondragend");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onDragEnd, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.DragEnd -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onDragEnd, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_DragLeave_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.DragLeave += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onDragLeave = new("ondragleave");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onDragLeave, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.DragLeave -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onDragLeave, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_DragOver_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.DragOver += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onDragOver = new("ondragover");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onDragOver, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.DragOver -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onDragOver, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_GotFocus_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.GotFocus += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onFocus = new("onfocus");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onFocus, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.GotFocus -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onFocus, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_Focusing_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.Focusing += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onFocusin = new("onfocusin");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onFocusin, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.Focusing -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onFocusin, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_LosingFocus_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.LosingFocus += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onFocusOut = new("onfocusout");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onFocusOut, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.LosingFocus -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onFocusOut, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_LostFocus_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.LostFocus += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onBlur = new("onblur");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onBlur, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.LostFocus -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onBlur, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_KeyDown_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.KeyDown += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onKeyDown = new("onkeydown");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onKeyDown, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.KeyDown -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onKeyDown, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_KeyPress_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.KeyPress += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onKeyPress = new("onkeypress");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onKeyPress, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.KeyPress -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onKeyPress, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_KeyUp_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.KeyUp += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onKeyUp = new("onkeyup");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onKeyUp, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.KeyUp -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onKeyUp, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_MouseDown_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.MouseDown += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onMouseDown = new("onmousedown");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseDown, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.MouseDown -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseDown, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_MouseEnter_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.MouseEnter += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onMouseEnter = new("onmouseenter");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseEnter, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.MouseEnter -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseEnter, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_MouseLeave_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.MouseLeave += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onMouseLeave = new("onmouseleave");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseLeave, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.MouseLeave -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseLeave, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_MouseMove_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.MouseMove += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            using BSTR onMouseMove = new("onmousemove");
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseMove, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.MouseMove -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseMove, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_MouseOver_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.MouseOver += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onMouseOver = new("onmouseover");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseOver, &eventObj, &cancelled).Succeeded);
            Assert.False(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.MouseOver -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseOver, &eventObj, &cancelled).Succeeded);
            Assert.False(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    [WinFormsFact]
    public async Task HtmlElement_MouseUp_InvokeEvent_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        const string Html = "<html><body><div id=\"id\"></div></body></html>";
        HtmlDocument document = await GetDocument(control, Html);
        HtmlElement element = document.GetElementById("id");

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Same(element, sender);
            Assert.IsType<HtmlElementEventArgs>(e);
            callCount++;
        }

        validate();
        unsafe void validate()
        {
            element.MouseUp += handler;
            using var iHTMLElement3 = ComHelpers.GetComScope<IHTMLElement3>(element.DomElement);
            using BSTR onMouseUp = new("onmouseup");
            VARIANT eventObj = default;
            VARIANT_BOOL cancelled = default;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseUp, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);

            // Remove handler.
            element.MouseUp -= handler;
            Assert.True(iHTMLElement3.Value->fireEvent(onMouseUp, &eventObj, &cancelled).Succeeded);
            Assert.True(cancelled);
            Assert.Equal(1, callCount);
        }
    }

    private static async Task<HtmlDocument> GetDocument(WebBrowser control, string html)
    {
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        using var file = CreateTempFile(html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        return control.Document;
    }

    private static TempFile CreateTempFile(string html)
    {
        byte[] data = Encoding.UTF8.GetBytes(html);
        return TempFile.Create(data);
    }
}
