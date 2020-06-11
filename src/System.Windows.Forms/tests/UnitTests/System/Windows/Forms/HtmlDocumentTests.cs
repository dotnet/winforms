// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using WinForms.Common.Tests;
using static Interop;
using static Interop.Mshtml;

namespace System.Windows.Forms.Tests
{
    [Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
    public class HtmlDocumentTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public async Task HtmlDocument_ActiveLinkColor_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), document.ActiveLinkColor);
        }

        public static IEnumerable<object[]> ActiveLinkColor_GetCustomValueOnBody_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x11, 0x30, 0x60) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x30, 0x41, 0x89) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ActiveLinkColor_GetCustomValueOnBody_TestData))]
        public async Task HtmlDocument_ActiveLinkColor_GetCustomValueOnBody_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html><body alink={value}></html>";
            HtmlDocument document = await GetDocument(control, html);
            Assert.Equal(expected, document.ActiveLinkColor);
        }

        public static IEnumerable<object[]> ActiveLinkColor_GetCustomValueSet_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ActiveLinkColor_GetCustomValueSet_TestData))]
        public async Task HtmlDocument_ActiveLinkColor_GetCustomValueSet_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);

            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetAlinkColor(value);
            Assert.Equal(expected, document.ActiveLinkColor);
        }

        [WinFormsTheory]
        [MemberData(nameof(Color_Set_TestData))]
        public async Task HtmlDocument_ActiveLinkColor_Set_GetReturnsExpected(Color value, Color expected, string expectedNative)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.ActiveLinkColor = value;
            Assert.Equal(expected, document.ActiveLinkColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetAlinkColor());

            // Set same.
            document.ActiveLinkColor = value;
            Assert.Equal(expected, document.ActiveLinkColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetAlinkColor());
        }

        [WinFormsFact]
        public async Task HtmlDocument_All_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>NewDocument</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.All;
            Assert.NotSame(collection, document.All);
            Assert.Equal(4, collection.Count);
            Assert.Equal("HTML", collection[0].TagName);
            Assert.Equal("HEAD", collection[1].TagName);
            Assert.Equal("TITLE", collection[2].TagName);
            Assert.Equal("BODY", collection[3].TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_All_GetEmpty_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.All;
            Assert.NotSame(collection, document.All);
            Assert.Equal(4, collection.Count);
            Assert.Equal("HTML", collection[0].TagName);
            Assert.Equal("HEAD", collection[1].TagName);
            Assert.Equal("TITLE", collection[2].TagName);
            Assert.Equal("BODY", collection[3].TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_ActiveElement_GetNotActiveWithBody_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement element = document.ActiveElement;
            Assert.NotSame(element, document.ActiveElement);
            Assert.Equal("InnerText", element.InnerText);
            Assert.Equal("BODY", element.TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_ActiveElement_GetNotActiveWithoutBody_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement element = document.ActiveElement;
            Assert.NotSame(element, document.ActiveElement);
            Assert.Null(element.InnerText);
            Assert.Equal("BODY", document.ActiveElement.TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_ActiveElement_GetNoActiveElement_ReturnsNull()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><h1>Title</h1><p id=\"target\">InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement target = document.GetElementById("target");

            HtmlElement active = document.ActiveElement;
            IHTMLElement2 iHtmlElement2 = (IHTMLElement2)active.DomElement;
            iHtmlElement2.Blur();

            HtmlElement element = document.ActiveElement;
            Assert.Null(document.ActiveElement);
        }

        [WinFormsFact]
        public async Task HtmlDocument_BackColor_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), document.BackColor);
        }

        public static IEnumerable<object[]> BackColor_GetCustomValueOnBody_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x11, 0x30, 0x60) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x30, 0x41, 0x89) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_GetCustomValueOnBody_TestData))]
        public async Task HtmlDocument_BackColor_GetCustomValueOnBody_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html><body bgcolor={value}></body></html>";
            HtmlDocument document = await GetDocument(control, html);
            Assert.Equal(expected, document.BackColor);
        }

        public static IEnumerable<object[]> BackColor_GetCustomValueSet_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_GetCustomValueSet_TestData))]
        public async Task HtmlDocument_BackColor_GetCustomValueSet_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);

            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetBgColor(value);
            Assert.Equal(expected, document.BackColor);
        }

        public static IEnumerable<object[]> Color_Set_TestData()
        {
            yield return new object[] { Color.Empty, Color.FromArgb(0xFF, 0x00, 0x00, 0x00), "#000000" };
            yield return new object[] { Color.Red, Color.FromArgb(0xFF, 0xFF, 0x00, 0x00), "#ff0000" };
            yield return new object[] { Color.White, Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), "#ffffff" };
            yield return new object[] { Color.FromArgb(0x12, 0x34, 0x56, 0x78), Color.FromArgb(0xFF, 0x34, 0x56, 0x78), "#345678" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Color_Set_TestData))]
        public async Task HtmlDocument_BackColor_Set_GetReturnsExpected(Color value, Color expected, string expectedNative)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.BackColor = value;
            Assert.Equal(expected, document.BackColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetBgColor());

            // Set same.
            document.BackColor = value;
            Assert.Equal(expected, document.BackColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetBgColor());
        }

        [WinFormsFact]
        public async Task HtmlDocument_Body_GetWithBody_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement element = document.Body;
            Assert.NotSame(element, document.Body);
            Assert.Equal("InnerText", element.InnerText);
            Assert.Equal("BODY", element.TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Body_GetWithoutBody_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement element = document.Body;
            Assert.NotSame(element, document.Body);
            Assert.Null(element.InnerText);
            Assert.Equal("BODY", element.TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Body_GetNoBody_ReturnsNull()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement element = document.Body;
            IHTMLDOMNode iHtmlDomNode = (IHTMLDOMNode)element.Parent.DomElement;
            iHtmlDomNode.RemoveChild((IHTMLDOMNode)element.DomElement);

            Assert.Null(document.Body);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Cookie_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal(document.Cookie, document.Cookie);
        }

        [WinFormsTheory]
        [InlineData(null, null)]
        [InlineData("", null)]
        [InlineData("cookie", "cookie")]
        public async Task HtmlDocument_Cookie_Set_GetReturnsExpected(string value, string expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.Cookie = value;
            Assert.Equal(expected, document.Cookie);
            Assert.Equal(expected, iHTMLDocument2.GetCookie());

            // Set same.
            document.Cookie = value;
            Assert.Equal(expected, document.Cookie);
            Assert.Equal(expected, iHTMLDocument2.GetCookie());
        }

        [WinFormsFact]
        public async Task HtmlDocument_DefaultEncoding_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.NotEmpty(document.DefaultEncoding);
            Assert.DoesNotContain('\0', document.DefaultEncoding);
        }

        [WinFormsFact]
        public async Task HtmlDocument_DefaultEncoding_GetWithCustomValueOnMeta_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><meta charset=\"UTF-8\" /></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.NotEmpty(document.DefaultEncoding);
            Assert.DoesNotContain('\0', document.DefaultEncoding);
        }

        [WinFormsFact]
        public async Task HtmlDocument_DefaultEncoding_GetCustomValueSet_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetDefaultCharset("UTF-8");
            Assert.NotEmpty(document.DefaultEncoding);
            Assert.DoesNotContain('\0', document.DefaultEncoding);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Domain_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Empty(document.Domain);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public async Task HtmlDocument_Domain_Set_ThrowsCOMException(string value)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);

            Assert.Throws<COMException>(() => document.Domain = value);
            Assert.Empty(document.Domain);
        }

        [WinFormsFact]
        public async Task HtmlDocument_DomDocument_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);

            object domDocument = document.DomDocument;
            Assert.Same(domDocument, document.DomDocument);
            Assert.True(domDocument.GetType().IsCOMObject);
            Assert.False(domDocument is IHTMLDOMNode);
            Assert.True(domDocument is IHTMLDocument);
            Assert.True(domDocument is IHTMLDocument2);
            Assert.True(domDocument is IHTMLDocument3);
            Assert.True(domDocument is IHTMLDocument4);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Encoding_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.NotEmpty(document.Encoding);
            Assert.DoesNotContain('\0', document.Encoding);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Encoding_GetWithCustomValueOnMeta_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><meta charset=\"UTF-8\" /></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal("utf-8", document.Encoding);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Encoding_GetCustomValueSet_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetCharset("UTF-8");
            Assert.Equal("utf-8", document.Encoding);
        }

        [WinFormsTheory]
        [InlineData("utf-8", "utf-8")]
        [InlineData("UTF-8", "utf-8")]
        public async Task HtmlDocument_Encoding_Set_GetReturnsExpected(string value, string expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.Encoding = value;
            Assert.Equal(expected, document.Encoding);
            Assert.Equal(expected, iHTMLDocument2.GetCharset());

            // Set same.
            document.Encoding = value;
            Assert.Equal(expected, document.Encoding);
            Assert.Equal(expected, iHTMLDocument2.GetCharset());
        }

        [WinFormsTheory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("reasonable")]
        public async Task HtmlDocument_Charset_SetInvalid_ThrowsArgumentException(string value)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Throws<ArgumentException>(null, () => document.Encoding = value);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Focused_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.False(document.Focused);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Focused_GetFocused_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;
            iHTMLDocument4.Focus();
            Assert.False(document.Focused);

            // Have to do it again.
            iHTMLDocument4.Focus();
            Assert.False(document.Focused);
        }

        [WinFormsFact]
        public async Task HtmlDocument_ForeColor_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0x00), document.ForeColor);
        }

        public static IEnumerable<object[]> ForeColor_GetCustomValueOnBody_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x11, 0x30, 0x60) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x30, 0x41, 0x89) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_GetCustomValueOnBody_TestData))]
        public async Task HtmlDocument_ForeColor_GetCustomValueOnBody_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html><body text={value}></html>";
            HtmlDocument document = await GetDocument(control, html);
            Assert.Equal(expected, document.ForeColor);
        }

        public static IEnumerable<object[]> ForeColor_GetCustomValueSet_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(ForeColor_GetCustomValueSet_TestData))]
        public async Task HtmlDocument_ForeColor_GetCustomValueSet_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);

            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetFgColor(value);
            Assert.Equal(expected, document.ForeColor);
        }

        [WinFormsTheory]
        [MemberData(nameof(Color_Set_TestData))]
        public async Task HtmlDocument_ForeColor_Set_GetReturnsExpected(Color value, Color expected, string expectedNative)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.ForeColor = value;
            Assert.Equal(expected, document.ForeColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetFgColor());

            // Set same.
            document.ForeColor = value;
            Assert.Equal(expected, document.ForeColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetFgColor());
        }

        [WinFormsFact]
        public async Task HtmlDocument_Forms_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><img id=\"img1\" /><img id=\"img2\" /><a id=\"link1\">Href</a><a id=\"link2\">Href</a><form id=\"form1\"></form><form id=\"form2\"></form></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.Forms;
            Assert.NotSame(collection, document.Forms);
            Assert.Equal(2, collection.Count);
            Assert.Equal("FORM", collection[0].TagName);
            Assert.Equal("form1", collection[0].GetAttribute("id"));
            Assert.Equal("FORM", collection[1].TagName);
            Assert.Equal("form2", collection[1].GetAttribute("id"));
        }

        [WinFormsFact]
        public async Task HtmlDocument_Forms_GetEmpty_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.Forms;
            Assert.NotSame(collection, document.Forms);
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Images_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><img id=\"img1\" /><img id=\"img2\" /><a id=\"link1\">Href</a><a id=\"link2\">Href</a><form id=\"form1\"></form><form id=\"form2\"></form></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.Images;
            Assert.NotSame(collection, document.Images);
            Assert.Equal(2, collection.Count);
            Assert.Equal("IMG", collection[0].TagName);
            Assert.Equal("img1", collection[0].GetAttribute("id"));
            Assert.Equal("IMG", collection[1].TagName);
            Assert.Equal("img2", collection[1].GetAttribute("id"));
        }

        [WinFormsFact]
        public async Task HtmlDocument_Images_GetEmpty_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.Images;
            Assert.NotSame(collection, document.Images);
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public async Task HtmlDocument_LinkColor_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal(Color.FromArgb(0xFF, 0x00, 0x00, 0xFF), document.LinkColor);
        }

        public static IEnumerable<object[]> LinkColor_GetCustomValueOnBody_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x11, 0x30, 0x60) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x30, 0x41, 0x89) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(LinkColor_GetCustomValueOnBody_TestData))]
        public async Task HtmlDocument_LinkColor_GetCustomValueOnBody_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html><body link={value}></html>";
            HtmlDocument document = await GetDocument(control, html);
            Assert.Equal(expected, document.LinkColor);
        }

        public static IEnumerable<object[]> LinkColor_GetCustomValueSet_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x00, 0x00, 0xFF) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(LinkColor_GetCustomValueSet_TestData))]
        public async Task HtmlDocument_LinkColor_GetCustomValueSet_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);

            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetLinkColor(value);
            Assert.Equal(expected, document.LinkColor);
        }

        [WinFormsTheory]
        [MemberData(nameof(Color_Set_TestData))]
        public async Task HtmlDocument_LinkColor_Set_GetReturnsExpected(Color value, Color expected, string expectedNative)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.LinkColor = value;
            Assert.Equal(expected, document.LinkColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetLinkColor());

            // Set same.
            document.LinkColor = value;
            Assert.Equal(expected, document.LinkColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetLinkColor());
        }

        [WinFormsFact]
        public async Task HtmlDocument_Links_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><img id=\"img1\" /><img id=\"img2\" /><a href=\"#1\" id=\"link1\">Href</a><a href=\"#1\" id=\"link2\">Href</a><form id=\"form1\"></form><form id=\"form2\"></form></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.Links;
            Assert.NotSame(collection, document.Links);
            Assert.Equal(2, collection.Count);
            Assert.Equal("A", collection[0].TagName);
            Assert.Equal("link1", collection[0].GetAttribute("id"));
            Assert.Equal("A", collection[1].TagName);
            Assert.Equal("link2", collection[1].GetAttribute("id"));
        }

        [WinFormsFact]
        public async Task HtmlDocument_Links_GetEmpty_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElementCollection collection = document.Links;
            Assert.NotSame(collection, document.Links);
            Assert.Empty(collection);
        }

        [WinFormsFact]
        public async Task HtmlDocument_RightToLeft_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.False(document.RightToLeft);
        }

        public static IEnumerable<object[]> RightToLeft_CustomValueOnHtml_TestData()
        {
            yield return new object[] { "rtl", true };
            yield return new object[] { "RTL", true };
            yield return new object[] { "ltr", false };
            yield return new object[] { "abc", false };
            yield return new object[] { "", false };
            yield return new object[] { "123", false };
        }

        [WinFormsTheory]
        [MemberData(nameof(RightToLeft_CustomValueOnHtml_TestData))]
        public async Task HtmlDocument_RightToLeft_GetCustomValueOnHtml_ReturnsExpected(string rtl, bool expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html dir={rtl}></html>";
            HtmlDocument document = await GetDocument(control, html);
            Assert.Equal(expected, document.RightToLeft);
        }

        public static IEnumerable<object[]> RightToLeft_CustomValueSet_TestData()
        {
            yield return new object[] { "rtl", true };
            yield return new object[] { "RTL", true };
            yield return new object[] { "ltr", false };
            yield return new object[] { "", false };
        }

        [WinFormsTheory]
        [MemberData(nameof(RightToLeft_CustomValueSet_TestData))]
        public async Task HtmlDocument_RightToLeft_GetCustomValueSet_ReturnsExpected(string rtl, bool expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html></html>";
            HtmlDocument document = await GetDocument(control, html);

            IHTMLDocument3 iHTMLDocument3 = (IHTMLDocument3)document.DomDocument;
            iHTMLDocument3.SetDir(rtl);
            Assert.Equal(expected, document.RightToLeft);
        }

        [WinFormsTheory]
        [InlineData(true, "rtl", "ltr")]
        [InlineData(false, "ltr", "rtl")]
        public async Task HtmlDocument_RightToLeft_Set_GetReturnsExpected(bool value, string expectedNative1, string expectedNative2)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument3 iHTMLDocument3 = (IHTMLDocument3)document.DomDocument;

            document.RightToLeft = value;
            Assert.Equal(value, document.RightToLeft);
            Assert.Equal(expectedNative1, iHTMLDocument3.GetDir());

            // Set same.
            document.RightToLeft = value;
            Assert.Equal(value, document.RightToLeft);
            Assert.Equal(expectedNative1, iHTMLDocument3.GetDir());

            // Set different.
            document.RightToLeft = !value;
            Assert.Equal(!value, document.RightToLeft);
            Assert.Equal(expectedNative2, iHTMLDocument3.GetDir());
        }

        [WinFormsFact]
        public async Task HtmlDocument_Title_GetWithTitle_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal("Title", document.Title);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Title_GetNoTitle_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Empty(document.Title);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public async Task HtmlDocument_Title_GetCustomValueSet_ReturnsExpected(string title, string expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetTitle(title);
            Assert.Equal(expected, document.Title);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public async Task HtmlDocument_Title_Set_GetReturnsExpected(string value, string expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.Title = value;
            Assert.Equal(expected, document.Title);
            Assert.Equal(expected, iHTMLDocument2.GetTitle());

            // Set same.
            document.Title = value;
            Assert.Equal(expected, document.Title);
            Assert.Equal(expected, iHTMLDocument2.GetTitle());
        }

        [WinFormsFact]
        public async Task HtmlDocument_Url_GetDocument_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            var source = new TaskCompletionSource<bool>();
            control.DocumentCompleted += (sender, e) => source.SetResult(true);

            using var file = CreateTempFile(Html);
            await Task.Run(() => control.Navigate(file.Path));
            Assert.True(await source.Task);

            HtmlDocument document = control.Document;
            Assert.Equal(new Uri(file.Path), document.Url);
        }
        [WinFormsFact]
        public async Task HtmlDocument_VisitedLinkColor_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Equal(Color.FromArgb(0xFF, 0x80, 0x00, 0x80), document.VisitedLinkColor);
        }

        public static IEnumerable<object[]> VisitedLinkColor_GetCustomValueOnBody_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x80, 0x00, 0x80) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x80, 0x00, 0x80) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x11, 0x30, 0x60) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x30, 0x41, 0x89) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(VisitedLinkColor_GetCustomValueOnBody_TestData))]
        public async Task HtmlDocument_VisitedLinkColor_GetCustomValueOnBody_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            string html = $"<html><body vlink={value}></html>";
            HtmlDocument document = await GetDocument(control, html);
            Assert.Equal(expected, document.VisitedLinkColor);
        }

        public static IEnumerable<object[]> VisitedLinkColor_GetCustomValueSet_TestData()
        {
            yield return new object[] { null, Color.FromArgb(0xFF, 0x80, 0x00, 0x80) };
            yield return new object[] { "", Color.FromArgb(0xFF, 0x80, 0x00, 0x80) };
            yield return new object[] { "NoSuchName", Color.FromArgb(0xFF, 0x00, 0xC0, 0x0E) };
            yield return new object[] { "Invalid", Color.FromArgb(0xFF, 0x00, 0xA0, 0xD0) };
            yield return new object[] { "Red", Color.FromArgb(0xFF, 0xFF, 0x00, 0x00) };
            yield return new object[] { (int)0x123456, Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { (int)0x12345678, Color.FromArgb(0xFF, 0x80, 0x00, 0x80) };
            yield return new object[] { "#", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
            yield return new object[] { "#1", Color.FromArgb(0xFF, 0x1, 0x00, 0x00) };
            yield return new object[] { "#123456", Color.FromArgb(0xFF, 0x12, 0x34, 0x56) };
            yield return new object[] { "#12345678", Color.FromArgb(0xFF, 0x12, 0x45, 0x78) };
            yield return new object[] { "abc#123456", Color.FromArgb(0xFF, 0xAB, 0x12, 0x56) };
            yield return new object[] { "#G", Color.FromArgb(0xFF, 0x00, 0x00, 0x00) };
        }

        [WinFormsTheory]
        [MemberData(nameof(VisitedLinkColor_GetCustomValueSet_TestData))]
        public async Task HtmlDocument_VisitedLinkColor_GetCustomValueSet_ReturnsExpected(object value, Color expected)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>Title</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);

            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;
            iHTMLDocument2.SetVlinkColor(value);
            Assert.Equal(expected, document.VisitedLinkColor);
        }

        [WinFormsTheory]
        [MemberData(nameof(Color_Set_TestData))]
        public async Task HtmlDocument_VisitedLinkColor_Set_GetReturnsExpected(Color value, Color expected, string expectedNative)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument2 iHTMLDocument2 = (IHTMLDocument2)document.DomDocument;

            document.VisitedLinkColor = value;
            Assert.Equal(expected, document.VisitedLinkColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetVlinkColor());

            // Set same.
            document.VisitedLinkColor = value;
            Assert.Equal(expected, document.VisitedLinkColor);
            Assert.Equal(expectedNative, iHTMLDocument2.GetVlinkColor());
        }

        [WinFormsFact]
        public async Task HtmlDocument_Window_Get_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlWindow window = document.Window;
            Assert.NotSame(window, document.Window);
            Assert.NotSame(document, window.Document);
            Assert.Equal("InnerText", window.Document.Body.InnerText);
        }

        [WinFormsTheory]
        [InlineData("eventName")]
        [InlineData("onclick")]
        public async Task HtmlDocument_AttachEventHandler_AttachDetach_Success(string eventName)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            document.AttachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);

            // Attach again.
            document.AttachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [InlineData("onclick")]
        [InlineData("oncontextmenu")]
        [InlineData("onfocusin")]
        [InlineData("onfocusout")]
        [InlineData("onmousemove")]
        [InlineData("onmousedown")]
        [InlineData("onmouseout")]
        [InlineData("onmouseover")]
        [InlineData("onmouseup")]
        [InlineData("onstop")]
        [InlineData("onhelp")]
        [InlineData("ondblclick")]
        [InlineData("onkeydown")]
        [InlineData("onkeyup")]
        [InlineData("onkeypress")]
        [InlineData("onreadystatechange")]
        [InlineData("onbeforeupdate")]
        [InlineData("onafterupdate")]
        [InlineData("onrowexit")]
        [InlineData("onrowenter")]
        [InlineData("ondragstart")]
        [InlineData("onselectstart")]
        [InlineData("onerrorupdate")]
        [InlineData("onrowsdelete")]
        [InlineData("onrowsinserted")]
        [InlineData("oncellchange")]
        [InlineData("onpropertychange")]
        [InlineData("ondatasetchanged")]
        [InlineData("ondataavailable")]
        [InlineData("ondatasetcomplete")]
        [InlineData("onbeforeeditfocus")]
        [InlineData("onselectionchange")]
        [InlineData("oncontrolselect")]
        [InlineData("onmousewheel")]
        [InlineData("onactivate")]
        [InlineData("ondeactivate")]
        [InlineData("onbeforeactivate")]
        [InlineData("onbeforedeactivate")]
        public async Task HtmlDocument_AttachEventHandler_InvokeClick_Success(string eventName)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;
            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.NotSame(document, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            }
            document.AttachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);

            iHTMLDocument4.FireEvent(eventName, null);
            Assert.Equal(1, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_AttachEventHandler_EmptyEventName_ThrowsCOMException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            COMException ex = Assert.Throws<COMException>(() => document.AttachEventHandler(string.Empty, handler));
            Assert.Equal(HRESULT.DISP_E_UNKNOWNNAME, (HRESULT)ex.HResult);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_AttachEventHandler_NullEventName_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            document.AttachEventHandler(null, handler);
            Assert.Equal(0, callCount);
        }

        [WinFormsTheory]
        [InlineData("", "")]
        [InlineData("TagName", "TagName")]
        [InlineData("h1", "H1")]
        public async Task HtmlDocument_CreateElement_Invoke_ReturnsExpected(string tagName, string expectedTagName)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body>InnerText</body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlElement element = document.CreateElement(tagName);
            Assert.Equal(expectedTagName, element.TagName);
            Assert.NotSame(document, element.Document);
            Assert.Null(element.Document.Body);
        }

        [WinFormsFact]
        public async Task HtmlDocument_CreateElement_NullElementTag_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Throws<ArgumentException>(null, () => document.CreateElement(null));
        }

        [WinFormsTheory]
        [InlineData("eventName")]
        [InlineData("onclick")]
        public async Task HtmlDocument_DetachEventHandler_AttachDetach_Success(string eventName)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);

            document.DetachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);

            // Detach again.
            document.DetachEventHandler(eventName, handler);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_DetachEventHandler_EmptyEventName_ThrowsCOMException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            document.DetachEventHandler(string.Empty, handler);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_DetachEventHandler_NullEventName_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            int callCount = 0;
            void handler(object sender, EventArgs e) => callCount++;
            document.DetachEventHandler(null, handler);
            Assert.Equal(0, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Equals_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlDocument newDocument = document.OpenNew(false);

            Assert.True(document.Equals(document));
            Assert.False(document.Equals(newDocument));
            Assert.Throws<InvalidCastException>(() => document.Equals(new object()));
            Assert.False(document.Equals(null));
        }

        [WinFormsTheory]
        [InlineData("copy", true, null)]
        [InlineData("copy", true, "abc")]
        [InlineData("copy", false, null)]
        [InlineData("copy", false, "def")]
        public async Task HtmlDocument_ExecCommand_Invoke_Success(string command, bool showUI, object value)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>NewDocument</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            document.ExecCommand(command, showUI, value);
        }

        [WinFormsTheory]
        [InlineData(null, true, null)]
        [InlineData("NoSuchCommand", true, null)]
        [InlineData(null, false, null)]
        [InlineData("NoSuchCommand", false, null)]
        public async Task HtmlDocument_ExecCommand_InvalidCommand_ThrowsCOMException(string command, bool showUI, object value)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>NewDocument</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Throws<COMException>(() => document.ExecCommand(command, showUI, value));
        }

        [WinFormsFact]
        public async Task HtmlDocument_Focus_Invoke_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            document.Focus();
            Assert.False(document.Focused);

            // Call again.
            document.Focus();
            Assert.False(document.Focused);
        }

        [WinFormsFact]
        public async Task HtmlDocument_GetElementById_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p id=\"para1\">InnerText1</p><p id=\"para2\">InnerText2</p><p>InnerText3</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);

            Assert.Equal("InnerText1", document.GetElementById("para1").InnerText);
            Assert.Equal("InnerText1", document.GetElementById("PARA1").InnerText);
            Assert.Null(document.GetElementById("NoSuchId"));
            Assert.Null(document.GetElementById(string.Empty));
        }

        [WinFormsFact]
        public async Task HtmlDocument_GetElementById_NullId_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Throws<ArgumentException>(null, () => document.GetElementById(null));
        }

        [WinFormsFact]
        public async Task HtmlDocument_GetElementFromPoint_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p id=\"para1\">InnerText1</p><p id=\"para2\">InnerText2</p><p>InnerText3</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);

            Assert.Equal("BODY", document.GetElementFromPoint(Point.Empty).TagName);
            Assert.Equal("BODY", document.GetElementFromPoint(new Point(int.MinValue, int.MinValue)).TagName);
            Assert.Equal("BODY", document.GetElementFromPoint(new Point(int.MaxValue, int.MaxValue)).TagName);
        }

        [WinFormsFact]
        public async Task HtmlDocument_GetElementsByTagName_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><img id=\"img1\" /><img id=\"img2\" /><a id=\"link1\">Href</a><a id=\"link2\">Href</a><form id=\"form1\"></form><form id=\"form2\"></form></body></html>";
            HtmlDocument document = await GetDocument(control, Html);

            HtmlElementCollection collection = document.GetElementsByTagName("form");
            Assert.NotSame(collection, document.GetElementsByTagName("form"));
            Assert.Equal(2, collection.Count);
            Assert.Equal("FORM", collection[0].TagName);
            Assert.Equal("form1", collection[0].GetAttribute("id"));
            Assert.Equal("FORM", collection[1].TagName);
            Assert.Equal("form2", collection[1].GetAttribute("id"));

            Assert.Empty(document.GetElementsByTagName("NoSuchTagName"));
            Assert.Equal("HTML", ((HtmlElement)Assert.Single(document.GetElementsByTagName("html"))).TagName);
            Assert.Empty(document.GetElementsByTagName(""));
        }

        [WinFormsFact]
        public async Task HtmlDocument_GetElementsByTagName_NullTagName_ThrowsArgumentException()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);
            Assert.Throws<ArgumentException>(null, () => document.GetElementsByTagName(null));
        }

        [WinFormsFact]
        public async Task HtmlDocument_GetHashCode_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlDocument newDocument = document.OpenNew(false);

            Assert.NotEqual(0, document.GetHashCode());
            Assert.Equal(document.GetHashCode(), document.GetHashCode());
            Assert.NotEqual(document.GetHashCode(), newDocument.GetHashCode());
        }

        [WinFormsFact]
        public async Task HtmlDocument_InvokeScript_ScriptExists_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = @"
<html>
    <head>
        <script>
            function divide(p1, p2)
            {
                if (!p1)
                {
                    return ""NoParameter1"";
                }
                if (!p2)
                {
                    return ""NoParameter2"";
                }

                return p1 / p2;
            }
        </script>
    </head>
</html>";
            HtmlDocument document = await GetDocument(control, Html);

            Assert.Equal("NoParameter1", document.InvokeScript("divide"));
            Assert.Equal("NoParameter1", document.InvokeScript("divide", null));
            Assert.Equal("NoParameter1", document.InvokeScript("divide", Array.Empty<object>()));
            Assert.Equal("NoParameter2", document.InvokeScript("divide", new object[] { 2 }));
            Assert.Equal(6, document.InvokeScript("divide", new object[] { 12, 2 }));
            Assert.Equal(6, document.InvokeScript("divide", new object[] { 12, 2 }));
        }

        [WinFormsFact]
        public async Task HtmlDocument_InvokeScript_NoSuchScript_ReturnsNull()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><head><title>NewDocument</title></head></html>";
            HtmlDocument document = await GetDocument(control, Html);

            Assert.Null(document.InvokeScript("NoSuchScript"));
            Assert.Null(document.InvokeScript("NoSuchScript", null));
            Assert.Null(document.InvokeScript("NoSuchScript", Array.Empty<object>()));
            Assert.Null(document.InvokeScript("NoSuchScript", new object[] { 1 }));
        }

        public static IEnumerable<object[]> Write_TestData()
        {
            yield return new object[] { null, "undefined" };
            yield return new object[] { "", null };
            yield return new object[] { "InnerText", "InnerText" };
            yield return new object[] { "<p>Hi</p>", "<P>Hi</P>" };
        }

        [WinFormsTheory]
        [MemberData(nameof(Write_TestData))]
        public async Task HtmlDocument_Write_InvokeEmpty_Success(string text, string expectedInnerHtml)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html></html>";
            HtmlDocument document = await GetDocument(control, Html);

            document.Write(text);
            Assert.Equal(expectedInnerHtml, document.Body?.InnerHtml);
        }

        [WinFormsTheory]
        [MemberData(nameof(Write_TestData))]
        public async Task HtmlDocument_Write_InvokeNotEmpty_Success(string text, string expectedInnerHtml)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><h1>OldH1</h1></body></html>";
            HtmlDocument document = await GetDocument(control, Html);

            document.Write(text);
            Assert.Equal(expectedInnerHtml, document.Body?.InnerHtml);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public async Task HtmlDocument_OpenNew_Invoke_Success(bool replaceInHistory)
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><h1>InnerText</h1></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlDocument newDocument = document.OpenNew(replaceInHistory);
            Assert.Empty(newDocument.All);
            Assert.Null(newDocument.Body);
            Assert.Equal("about:blank", newDocument.Url.OriginalString);
        }

#pragma warning disable CS1718 // Disable "Comparison made to same variable" warning.
        [WinFormsFact]
        public async Task HtmlDocument_OperatorEquals_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlDocument newDocument = document.OpenNew(false);

            Assert.True(document == document);
            Assert.False(document == newDocument);
            Assert.False((HtmlDocument)null == document);
            Assert.False(document == (HtmlDocument)null);
            Assert.True((HtmlDocument)null == (HtmlDocument)null);
        }

        [WinFormsFact]
        public async Task HtmlDocument_OperatorNotEquals_Invoke_ReturnsExpected()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><div id=\"id1\"></div><div id=\"id2\"></div></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            HtmlDocument newDocument = document.OpenNew(false);

            Assert.False(document != document);
            Assert.True(document != newDocument);
            Assert.True((HtmlDocument)null != document);
            Assert.True(document != (HtmlDocument)null);
            Assert.False((HtmlDocument)null != (HtmlDocument)null);
        }
#pragma warning restore CS1718

        [WinFormsFact]
        public async Task HtmlDocument_Click_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.Click += handler;
            iHTMLDocument4.FireEvent("onclick", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.Click -= handler;
            iHTMLDocument4.FireEvent("onclick", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_ContextMenuShowing_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.ContextMenuShowing += handler;
            iHTMLDocument4.FireEvent("oncontextmenu", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.ContextMenuShowing -= handler;
            iHTMLDocument4.FireEvent("oncontextmenu", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Focusing_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.Focusing += handler;
            iHTMLDocument4.FireEvent("onfocusin", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.Focusing -= handler;
            iHTMLDocument4.FireEvent("onfocusin", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_LosingFocus_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.LosingFocus += handler;
            iHTMLDocument4.FireEvent("onfocusout", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.LosingFocus -= handler;
            iHTMLDocument4.FireEvent("onfocusout", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_MouseDown_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.MouseDown += handler;
            iHTMLDocument4.FireEvent("onmousedown", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.MouseDown -= handler;
            iHTMLDocument4.FireEvent("onmousedown", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_MouseLeave_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.MouseLeave += handler;
            iHTMLDocument4.FireEvent("onmouseout", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.MouseLeave -= handler;
            iHTMLDocument4.FireEvent("onmouseout", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_MouseMove_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.MouseMove += handler;
            iHTMLDocument4.FireEvent("onmousemove", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.MouseMove -= handler;
            iHTMLDocument4.FireEvent("onmousemove", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_MouseOver_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.MouseOver += handler;
            iHTMLDocument4.FireEvent("onmouseover", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.MouseOver -= handler;
            iHTMLDocument4.FireEvent("onmouseover", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_MouseUp_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.MouseUp += handler;
            iHTMLDocument4.FireEvent("onmouseup", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.MouseUp -= handler;
            iHTMLDocument4.FireEvent("onmouseup", null);
            Assert.Equal(1, callCount);
        }

        [WinFormsFact]
        public async Task HtmlDocument_Stop_InvokeEvent_Success()
        {
            using var parent = new Control();
            using var control = new WebBrowser
            {
                Parent = parent
            };

            const string Html = "<html><body><p>InnerText</p></body></html>";
            HtmlDocument document = await GetDocument(control, Html);
            IHTMLDocument4 iHTMLDocument4 = (IHTMLDocument4)document.DomDocument;

            int callCount = 0;
            void handler(object sender, EventArgs e)
            {
                Assert.Same(document, sender);
                Assert.IsType<HtmlElementEventArgs>(e);
                callCount++;
            }

            document.Stop += handler;
            iHTMLDocument4.FireEvent("onstop", null);
            Assert.Equal(1, callCount);

            // Remove handler.
            document.Stop -= handler;
            iHTMLDocument4.FireEvent("onstop", null);
            Assert.Equal(1, callCount);
        }

        private async static Task<HtmlDocument> GetDocument(WebBrowser control, string html)
        {
            var source = new TaskCompletionSource<bool>();
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
}
