// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms.TestUtilities;
using Point = System.Drawing.Point;
using Size = System.Drawing.Size;

namespace System.Windows.Forms.Tests;

[Collection("Sequential")] // workaround for WebBrowser control corrupting memory when run on multiple UI threads
public class WebBrowserTests
{
    [WinFormsFact]
    public void WebBrowser_Ctor_Default()
    {
        using SubWebBrowser control = new();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.AllowDrop);
        Assert.True(control.AllowNavigation);
        Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, control.Anchor);
        Assert.False(control.AutoSize);
        Assert.Equal(Control.DefaultBackColor, control.BackColor);
        Assert.Null(control.BackgroundImage);
        Assert.Equal(ImageLayout.Tile, control.BackgroundImageLayout);
        Assert.Null(control.BindingContext);
        Assert.Equal(250, control.Bottom);
        Assert.Equal(new Rectangle(0, 0, 250, 250), control.Bounds);
        Assert.False(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.True(control.CanEnableIme);
        Assert.True(control.CanRaiseEvents);
        Assert.False(control.CanSelect);
        Assert.True(control.CausesValidation);
        Assert.Equal(new Size(250, 250), control.ClientSize);
        Assert.Equal(new Rectangle(0, 0, 250, 250), control.ClientRectangle);
        Assert.Null(control.Container);
        Assert.Null(control.ContextMenuStrip);
        Assert.Empty(control.Controls);
        Assert.Same(control.Controls, control.Controls);
        Assert.False(control.Created);
        Assert.Same(Cursors.Default, control.Cursor);
        Assert.Same(Cursors.Default, control.DefaultCursor);
        Assert.Equal(ImeMode.Inherit, control.DefaultImeMode);
        Assert.Equal(new Padding(3), control.DefaultMargin);
        Assert.Equal(Size.Empty, control.DefaultMaximumSize);
        Assert.Equal(Size.Empty, control.DefaultMinimumSize);
        Assert.Equal(Padding.Empty, control.DefaultPadding);
        Assert.Equal(new Size(250, 250), control.DefaultSize);
        Assert.False(control.DesignMode);
        Assert.Equal(new Rectangle(0, 0, 250, 250), control.DisplayRectangle);
        Assert.Equal(DockStyle.None, control.Dock);
        Assert.False(control.DoubleBuffered);
        Assert.True(control.Enabled);
        Assert.NotNull(control.Events);
        Assert.Same(control.Events, control.Events);
        Assert.Equal(Control.DefaultFont, control.Font);
        Assert.Equal(control.Font.Height, control.FontHeight);
        Assert.Equal(Control.DefaultForeColor, control.ForeColor);
        Assert.False(control.HasChildren);
        Assert.Equal(250, control.Height);
        Assert.Equal(ImeMode.NoControl, control.ImeMode);
        Assert.Equal(ImeMode.NoControl, control.ImeModeBase);
        Assert.True(control.IsWebBrowserContextMenuEnabled);
        Assert.NotNull(control.LayoutEngine);
        Assert.Same(control.LayoutEngine, control.LayoutEngine);
        Assert.Equal(0, control.Left);
        Assert.Equal(Point.Empty, control.Location);
        Assert.Equal(new Padding(3), control.Margin);
        Assert.Equal(Size.Empty, control.MaximumSize);
        Assert.Equal(Size.Empty, control.MinimumSize);
        Assert.Null(control.ObjectForScripting);
        Assert.Equal(Padding.Empty, control.Padding);
        Assert.Null(control.Parent);
        Assert.Equal("Microsoft\u00AE .NET", control.ProductName);
        Assert.Equal(new Size(250, 250), control.PreferredSize);
        Assert.False(control.RecreatingHandle);
        Assert.Null(control.Region);
        Assert.False(control.ResizeRedraw);
        Assert.Equal(250, control.Right);
        Assert.Equal(RightToLeft.No, control.RightToLeft);
        Assert.Null(control.Site);
        Assert.Equal(new Size(250, 250), control.Size);
        Assert.True(control.ScrollBarsEnabled);
        Assert.Equal(0, control.TabIndex);
        Assert.True(control.TabStop);
        Assert.Empty(control.Text);
        Assert.Equal(0, control.Top);
        Assert.Null(control.TopLevelControl);
        Assert.NotNull(control.Version);
        Assert.NotSame(control.Version, control.Version);
        Assert.True(control.Visible);
        Assert.True(control.WebBrowserShortcutsEnabled);
        Assert.Equal(250, control.Width);

        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_CreateParams_GetDefault_ReturnsExpected()
    {
        using SubWebBrowser control = new();
        CreateParams createParams = control.CreateParams;
        Assert.Null(createParams.Caption);
        Assert.Null(createParams.ClassName);
        Assert.Equal(0x8, createParams.ClassStyle);
        Assert.Equal(0, createParams.ExStyle);
        Assert.Equal(250, createParams.Height);
        Assert.Equal(IntPtr.Zero, createParams.Parent);
        Assert.Null(createParams.Param);
        Assert.Equal(0x56010000, createParams.Style);
        Assert.Equal(250, createParams.Width);
        Assert.Equal(0, createParams.X);
        Assert.Equal(0, createParams.Y);
        Assert.Same(createParams, control.CreateParams);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ActiveXInstance_GetWithHandle_ReturnsNull()
    {
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        Assert.Null(control.ActiveXInstance);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_AllowNavigation_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            AllowNavigation = value
        };
        Assert.Equal(value, control.AllowNavigation);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.AllowNavigation = value;
        Assert.Equal(value, control.AllowNavigation);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.AllowNavigation = !value;
        Assert.Equal(!value, control.AllowNavigation);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_AllowNavigation_SetWithInstance_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.AllowNavigation = value;
        Assert.Equal(value, control.AllowNavigation);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.AllowNavigation = value;
        Assert.Equal(value, control.AllowNavigation);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.AllowNavigation = !value;
        Assert.Equal(!value, control.AllowNavigation);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_AllowNavigation_SetWithSink_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.CreateSink();

        control.AllowNavigation = value;
        Assert.Equal(value, control.AllowNavigation);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.AllowNavigation = value;
        Assert.Equal(value, control.AllowNavigation);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.AllowNavigation = !value;
        Assert.Equal(!value, control.AllowNavigation);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        control.Stop();
    }

    [WinFormsFact]
    public void WebBrowser_AllowWebBrowserDrop_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.True(control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_AllowWebBrowserDrop_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.True(control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_AllowWebBrowserDrop_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.AllowWebBrowserDrop);
    }

    [WinFormsFact]
    public void WebBrowser_AllowWebBrowserDrop_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.AllowWebBrowserDrop);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_AllowWebBrowserDrop_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            AllowWebBrowserDrop = value
        };
        Assert.Equal(value, control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.AllowWebBrowserDrop = value;
        Assert.Equal(value, control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.AllowWebBrowserDrop = !value;
        Assert.Equal(!value, control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public unsafe void WebBrowser_AllowWebBrowserDrop_SetWithInstance_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.AllowWebBrowserDrop = value;
        Assert.Equal(value, control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        VARIANT_BOOL register = default;
        using var webBrowser = ComHelpers.GetComScope<IWebBrowser2>(control.ActiveXInstance);
        Assert.True(webBrowser.Value->get_RegisterAsDropTarget(&register).Succeeded);
        Assert.Equal(value, (bool)register);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.AllowWebBrowserDrop = value;
        Assert.Equal(value, control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(webBrowser.Value->get_RegisterAsDropTarget(&register).Succeeded);
        Assert.Equal(value, (bool)register);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.AllowWebBrowserDrop = !value;
        Assert.Equal(!value, control.AllowWebBrowserDrop);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(webBrowser.Value->get_RegisterAsDropTarget(&register).Succeeded);
        Assert.Equal(!value, (bool)register);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_AllowWebBrowserDrop_SetDisposed_ThrowsObjectDisposedException(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.AllowWebBrowserDrop = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_AllowWebBrowserDrop_SetDetached_ThrowsInvalidOperationException(bool value)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.AllowWebBrowserDrop = value);
    }

    [WinFormsFact]
    public void WebBrowser_CanGoBack_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        Assert.False(control.CanGoBack);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_CanGoBack_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file1 = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file1.Path));
        Assert.True(await source.Task);

        Assert.False(control.CanGoBack);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_CanGoForward_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.False(control.CanGoForward);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_CanGoForward_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file1 = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file1.Path));
        Assert.True(await source.Task);

        Assert.False(control.CanGoForward);
    }

    [WinFormsFact]
    public void WebBrowser_Document_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Null(control.Document);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Document_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Null(control.Document);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("<title>NewDocument</title>", "NewDocument")]
    [InlineData("<title></title>", "")]
    public async Task WebBrowser_Document_GetWithDocument_ReturnsExpected(string titleHtml, string expectedTitle)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        string html = $"<html><head>{titleHtml}</head></html>";
        using var file = CreateTempFile(html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        HtmlDocument document = control.Document;
        Assert.NotNull(document);
        Assert.NotSame(document, control.Document);
        Assert.Equal(expectedTitle, document.Title);
    }

    [WinFormsFact]
    public void WebBrowser_Document_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.Document);
    }

    [WinFormsFact]
    public void WebBrowser_Document_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.Document);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentStream_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Null(control.DocumentStream);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentStream_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Null(control.DocumentStream);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_DocumentStream_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        MemoryStream documentStream = Assert.IsType<MemoryStream>(control.DocumentStream);
        Assert.Equal(Html, Encoding.UTF8.GetString(documentStream.ToArray()));
        Assert.NotSame(documentStream, control.DocumentStream);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentStream_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.DocumentStream);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentStream_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.DocumentStream);
    }

    [WinFormsFact]
    public async Task WebBrowser_DocumentStream_Set_GetReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using MemoryStream value = new(Encoding.UTF8.GetBytes(Html));
        await Task.Run(() => control.DocumentStream = value);
        Assert.True(await source.Task);
        Assert.Equal(Html, control.DocumentText);
        Assert.Equal("about:blank", control.Url.OriginalString);

        // Set null.
        control.DocumentStream = null;
        Assert.True(await source.Task);
        Assert.Equal(Html, control.DocumentText);
        Assert.Equal("about:blank", control.Url.OriginalString);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentStream_SetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.DocumentStream = null);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentStream_SetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.DocumentStream = null);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentText_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Empty(control.DocumentText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentText_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Empty(control.DocumentText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_DocumentText_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        string documentText = control.DocumentText;
        Assert.Equal(documentText, control.DocumentText);
        Assert.NotSame(documentText, control.DocumentText);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentText_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.DocumentText);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentText_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.DocumentText);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void WebBrowser_DocumentText_Set_GetReturnsExpected(string value)
    {
        using WebBrowser control = new()
        {
            DocumentText = value
        };
        Assert.NotEmpty(control.DocumentText);
        Assert.DoesNotContain('\0', control.DocumentText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.DocumentText = value;
        Assert.NotEmpty(control.DocumentText);
        Assert.DoesNotContain('\0', control.DocumentText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [StringWithNullData]
    public void WebBrowser_DocumentText_SetWithInstance_GetReturnsExpected(string value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.DocumentText = value;
        Assert.NotEmpty(control.DocumentText);
        Assert.DoesNotContain('\0', control.DocumentText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.DocumentText = value;
        Assert.NotEmpty(control.DocumentText);
        Assert.DoesNotContain('\0', control.DocumentText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_DocumentText_SetValidHtml_GetReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        TaskCompletionSource<bool> source = new();
        int navigatingCallCount = 0;
        control.Navigating += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal("about:blank", e.Url.OriginalString);
            Assert.Empty(e.TargetFrameName);
            Assert.False(e.Cancel);
            navigatingCallCount++;
        };
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal("about:blank", e.Url.OriginalString);
            navigatedCallCount++;
        };
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            documentTitleChangedCallCount++;
        };
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal("about:blank", e.Url.OriginalString);
            documentCompletedCallCount++;
            source.SetResult(true);
        };
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            encryptionLevelChangedCallCount++;
        };
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            statusTextChangedCallCount++;
        };
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        await Task.Run(() => control.DocumentText = Html);
        Assert.True(await source.Task);
        Assert.Equal("about:blank", control.Url.OriginalString);
        Assert.Equal(1, navigatingCallCount);
        Assert.Equal(2, navigatedCallCount);
        Assert.True(documentTitleChangedCallCount > 0);
        Assert.Equal(1, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);

        // Call again
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.DocumentText = Html);
        Assert.True(await source.Task);
        Assert.Equal("about:blank", control.Url.OriginalString);
        Assert.Equal(2, navigatingCallCount);
        Assert.Equal(4, navigatedCallCount);
        Assert.True(documentTitleChangedCallCount > 0);
        Assert.Equal(2, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);

        // Set null.
        control.DocumentText = null;
        Assert.True(await source.Task);
        Assert.Equal(Html, control.DocumentText);
        Assert.Equal("about:blank", control.Url.OriginalString);

        // Set empty.
        control.DocumentText = string.Empty;
        Assert.True(await source.Task);
        Assert.Equal(Html, control.DocumentText);
        Assert.Equal("about:blank", control.Url.OriginalString);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentText_SetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.DocumentText = null);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentText_SetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.DocumentText = null);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentTitle_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Empty(control.DocumentTitle);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentTitle_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Empty(control.DocumentTitle);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [InlineData("<title>NewDocument</title>", "NewDocument")]
    [InlineData("<title></title>", "")]
    [InlineData("", "")]
    public async Task WebBrowser_DocumentTitle_GetWithDocument_ReturnsExpected(string titleHtml, string expected)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        string html = $"<html><head>{titleHtml}</head></html>";
        using var file = CreateTempFile(html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.Equal(expected, control.DocumentTitle);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentTitle_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.DocumentTitle);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentTitle_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.DocumentTitle);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentType_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Empty(control.DocumentType);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentType_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Empty(control.DocumentType);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_DocumentType_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.NotEmpty(control.DocumentType);
        Assert.DoesNotContain('\0', control.DocumentType);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentType_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.DocumentType);
    }

    [WinFormsFact]
    public void WebBrowser_DocumentType_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.DocumentType);
    }

    [WinFormsFact]
    public void WebBrowser_EncryptionLevel_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Equal(WebBrowserEncryptionLevel.Unknown, control.EncryptionLevel);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_EncryptionLevel_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Equal(WebBrowserEncryptionLevel.Unknown, control.EncryptionLevel);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_EncryptionLevel_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.Equal(WebBrowserEncryptionLevel.Insecure, control.EncryptionLevel);
    }

    [WinFormsFact]
    public void WebBrowser_EncryptionLevel_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.EncryptionLevel);
    }

    [WinFormsFact]
    public void WebBrowser_EncryptionLevel_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.EncryptionLevel);
    }

    [WinFormsFact]
    public void WebBrowser_Focused_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.False(control.Focused);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Focused_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.False(control.Focused);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Focused_GetWithHandle_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Assert.False(control.Focused);
        Assert.Null(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_IsBusy_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.False(control.IsBusy);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_IsBusy_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.False(control.IsBusy);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_IsBusy_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.False(control.IsBusy);
    }

    [WinFormsFact]
    public void WebBrowser_IsBusy_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.IsBusy);
    }

    [WinFormsFact]
    public void WebBrowser_IsBusy_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.IsBusy);
    }

    [WinFormsFact]
    public void WebBrowser_IsOffline_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.False(control.IsOffline);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_IsOffline_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.False(control.IsOffline);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_IsOffline_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.IsOffline);
    }

    [WinFormsFact]
    public void WebBrowser_IsOffline_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.IsOffline);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_IsWebBrowserContextMenuEnabled_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            IsWebBrowserContextMenuEnabled = value
        };
        Assert.Equal(value, control.IsWebBrowserContextMenuEnabled);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.IsWebBrowserContextMenuEnabled = value;
        Assert.Equal(value, control.IsWebBrowserContextMenuEnabled);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.IsWebBrowserContextMenuEnabled = !value;
        Assert.Equal(!value, control.IsWebBrowserContextMenuEnabled);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_IsWebBrowserContextMenuEnabled_SetWithInstance_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.IsWebBrowserContextMenuEnabled = value;
        Assert.Equal(value, control.IsWebBrowserContextMenuEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.IsWebBrowserContextMenuEnabled = value;
        Assert.Equal(value, control.IsWebBrowserContextMenuEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.IsWebBrowserContextMenuEnabled = !value;
        Assert.Equal(!value, control.IsWebBrowserContextMenuEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> ObjectForScripting_Set_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new CustomScriptingObject() };
    }

    [WinFormsTheory]
    [MemberData(nameof(ObjectForScripting_Set_TestData))]
    public void WebBrowser_ObjectForScripting_Set_GetReturnsExpected(object value)
    {
        using WebBrowser control = new()
        {
            ObjectForScripting = value
        };
        Assert.Same(value, control.ObjectForScripting);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.ObjectForScripting = value;
        Assert.Same(value, control.ObjectForScripting);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [MemberData(nameof(ObjectForScripting_Set_TestData))]
    public void WebBrowser_ObjectForScripting_SetWithInstance_GetReturnsExpected(object value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent,
            ObjectForScripting = value
        };
        Assert.Same(value, control.ObjectForScripting);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.ObjectForScripting = value;
        Assert.Same(value, control.ObjectForScripting);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ObjectForScripting_SetNotComVisible_ThrowsArgumentException()
    {
        using WebBrowser control = new();
        Assert.Throws<ArgumentException>("value", () => control.ObjectForScripting = new PrivateClass());
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void WebBrowser_Padding_Set_GetReturnsExpected(Padding value, Padding expected)
    {
        using WebBrowser control = new()
        {
            Padding = value
        };
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [CommonMemberData(typeof(CommonTestHelperEx), nameof(CommonTestHelperEx.GetPaddingNormalizedTheoryData))]
    public void WebBrowser_Padding_SetWithHandle_GetReturnsExpected(Padding value, Padding expected)
    {
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);

        // Set same.
        control.Padding = value;
        Assert.Equal(expected, control.Padding);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_Padding_SetWithHandler_CallsPaddingChanged()
    {
        using WebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Equal(control, sender);
            Assert.Equal(EventArgs.Empty, e);
            callCount++;
        };
        control.PaddingChanged += handler;

        // Set different.
        Padding padding1 = new(1);
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set same.
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(1, callCount);

        // Set different.
        Padding padding2 = new(2);
        control.Padding = padding2;
        Assert.Equal(padding2, control.Padding);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.PaddingChanged -= handler;
        control.Padding = padding1;
        Assert.Equal(padding1, control.Padding);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetNonNull_AddsToControls()
    {
        using WebBrowser parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Same(control, Assert.Single(parent.Controls));
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetParent_GetReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetParentNotVisible_GetReturnsExpected()
    {
        using Control parent = new()
        {
            Visible = false
        };
        using WebBrowser control = new()
        {
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetChildNotVisible_GetReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Visible = false,
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetWithHandle_GetReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetWithHandleParentNotVisible_GetReturnsExpected()
    {
        using Control parent = new()
        {
            Visible = false
        };
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetWithHandleChildNotVisible_GetReturnsExpected()
    {
        using Control parent = new();
        Assert.NotEqual(IntPtr.Zero, parent.Handle);

        using WebBrowser control = new()
        {
            Visible = false,
            Parent = parent
        };
        Assert.Same(parent, control.Parent);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetWithHandler_CallsParentChanged()
    {
        using WebBrowser parent = new();
        using WebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.ParentChanged += handler;

        // Set different.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set same.
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(1, callCount);

        // Set null.
        control.Parent = null;
        Assert.Null(control.Parent);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.ParentChanged -= handler;
        control.Parent = parent;
        Assert.Same(parent, control.Parent);
        Assert.Equal(2, callCount);
    }

    [WinFormsFact]
    public void WebBrowser_Parent_SetSame_ThrowsArgumentException()
    {
        using WebBrowser control = new();
        Assert.Throws<ArgumentException>(() => control.Parent = control);
        Assert.Null(control.Parent);
    }

    [WinFormsFact]
    public void WebBrowser_ReadyState_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Equal(WebBrowserReadyState.Uninitialized, control.ReadyState);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ReadyState_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Equal(WebBrowserReadyState.Uninitialized, control.ReadyState);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_ReadyState_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.Equal(WebBrowserReadyState.Complete, control.ReadyState);
    }

    [WinFormsFact]
    public void WebBrowser_ReadyState_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.ReadyState);
    }

    [WinFormsFact]
    public void WebBrowser_ReadyState_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.ReadyState);
    }

    [WinFormsFact]
    public void WebBrowser_ScriptErrorsSuppressed_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.False(control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ScriptErrorsSuppressed_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.False(control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ScriptErrorsSuppressed_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.ScriptErrorsSuppressed);
    }

    [WinFormsFact]
    public void WebBrowser_ScriptErrorsSuppressed_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.ScriptErrorsSuppressed);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_ScriptErrorsSuppressed_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            ScriptErrorsSuppressed = value
        };
        Assert.Equal(value, control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.ScriptErrorsSuppressed = value;
        Assert.Equal(value, control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.ScriptErrorsSuppressed = !value;
        Assert.Equal(!value, control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public unsafe void WebBrowser_ScriptErrorsSuppressed_SetWithInstance_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ScriptErrorsSuppressed = value;
        Assert.Equal(value, control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        using var webBrowser = ComHelpers.GetComScope<IWebBrowser2>(control.ActiveXInstance);
        VARIANT_BOOL silent = default;
        Assert.True(webBrowser.Value->get_Silent(&silent).Succeeded);
        Assert.Equal(value, (bool)silent);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.ScriptErrorsSuppressed = value;
        Assert.Equal(value, control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(webBrowser.Value->get_Silent(&silent).Succeeded);
        Assert.Equal(value, (bool)silent);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.ScriptErrorsSuppressed = !value;
        Assert.Equal(!value, control.ScriptErrorsSuppressed);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_ScriptErrorsSuppressed_SetDisposed_ThrowsObjectDisposedException(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.ScriptErrorsSuppressed = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_ScriptErrorsSuppressed_SetDetached_ThrowsInvalidOperationException(bool value)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.ScriptErrorsSuppressed = value);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_ScrollBarsEnabled_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            ScrollBarsEnabled = value
        };
        Assert.Equal(value, control.ScrollBarsEnabled);
        Assert.Equal(!value, control.ActiveXInstance is not null);
        Assert.Equal(!value, control.IsHandleCreated);

        // Set same.
        control.ScrollBarsEnabled = value;
        Assert.Equal(value, control.ScrollBarsEnabled);
        Assert.Equal(!value, control.ActiveXInstance is not null);
        Assert.Equal(!value, control.IsHandleCreated);

        // Set different.
        control.ScrollBarsEnabled = !value;
        Assert.Equal(!value, control.ScrollBarsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_ScrollBarsEnabled_SetWithInstance_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ScrollBarsEnabled = value;
        Assert.Equal(value, control.ScrollBarsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.ScrollBarsEnabled = value;
        Assert.Equal(value, control.ScrollBarsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.ScrollBarsEnabled = !value;
        Assert.Equal(!value, control.ScrollBarsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_StatusText_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Empty(control.StatusText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_StatusText_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Empty(control.StatusText);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_StatusText_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.Equal("Done", control.StatusText);
    }

    [WinFormsFact]
    public void WebBrowser_StatusText_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.StatusText);
    }

    [WinFormsFact]
    public void WebBrowser_StatusText_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.StatusText);
    }

    [WinFormsFact]
    public void WebBrowser_Url_Get_ReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Url_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_Url_GetWithDocument_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);

        Assert.Equal(new Uri(file.Path), control.Url);
    }

    [WinFormsFact]
    public void WebBrowser_Url_GetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.Url);
    }

    [WinFormsFact]
    public void WebBrowser_Url_GetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.Url);
    }

    [WinFormsFact]
    public async Task WebBrowser_Url_Set_GetReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        int navigatingCallCount = 0;
        control.Navigating += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            Assert.Empty(e.TargetFrameName);
            Assert.False(e.Cancel);
            navigatingCallCount++;
        };
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            navigatedCallCount++;
        };
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            documentTitleChangedCallCount++;
        };
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            documentCompletedCallCount++;
            source.SetResult(true);
        };
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            encryptionLevelChangedCallCount++;
        };
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            statusTextChangedCallCount++;
        };
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        await Task.Run(() => control.Url = new Uri(file.Path));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(1, navigatingCallCount);
        Assert.Equal(1, navigatedCallCount);
        Assert.Equal(2, documentTitleChangedCallCount);
        Assert.Equal(1, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);

        // Set same.
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Url = new Uri(file.Path));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(2, navigatingCallCount);
        Assert.Equal(2, navigatedCallCount);
        Assert.Equal(4, documentTitleChangedCallCount);
        Assert.Equal(2, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);
    }

    public static IEnumerable<object[]> Url_Set_NullOrEmpty_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Uri("", UriKind.Relative) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Url_Set_NullOrEmpty_TestData))]
    public async Task WebBrowser_Url_SetNullOrEmpty_GoesToBlank(Uri nullOrEmptyUri)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        await Task.Run(() => control.Url = new Uri(file.Path));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);

        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Url = nullOrEmptyUri);
        Assert.True(await source.Task);
        Assert.Equal("about:blank", control.Url.OriginalString);
    }

    [WinFormsFact]
    public void WebBrowser_Url_SetRelativeUri_ThrowsArgumentException()
    {
        using WebBrowser control = new();
        Uri relativeUri = new("/path", UriKind.Relative);
        Assert.Throws<ArgumentException>(() => control.Url = relativeUri);
    }

    [WinFormsFact]
    public void WebBrowser_Url_SetDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.Url = null);
    }

    [WinFormsFact]
    public void WebBrowser_Url_SetDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.Url = null);
    }

    [WinFormsFact]
    public void WebBrowser_Version_GetWithInstance_ReturnsExpected()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.NotNull(control.Version);
        Assert.NotSame(control.Version, control.Version);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_Visible_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            Visible = value
        };
        Assert.Equal(value, control.Visible);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.Visible = !value;
        Assert.Equal(!value, control.Visible);
        Assert.Equal(!value, control.ActiveXInstance is not null);
        Assert.Equal(!value, control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Visible_SetTrue_GetReturnsExpected()
    {
        using WebBrowser control = new()
        {
            Visible = false
        };
        Assert.False(control.Visible);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        control.Visible = true;
        Assert.True(control.Visible);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_Visible_SetWithHandle_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Visible = value;
        Assert.Equal(value, control.Visible);

        // Set same.
        control.Visible = value;
        Assert.Equal(value, control.Visible);

        // Set different.
        control.Visible = value;
        Assert.Equal(value, control.Visible);
    }

    [WinFormsFact]
    public void WebBrowser_Visible_SetTrueWithHandle_GetReturnsExpected()
    {
        using WebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);

        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Null(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Visible_SetWithHandler_CallsVisibleChanged()
    {
        using WebBrowser control = new()
        {
            Visible = true
        };
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        control.VisibleChanged += handler;

        // Set different.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set same.
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(1, callCount);

        // Set different.
        control.Visible = true;
        Assert.True(control.Visible);
        Assert.Equal(2, callCount);

        // Remove handler.
        control.VisibleChanged -= handler;
        control.Visible = false;
        Assert.False(control.Visible);
        Assert.Equal(2, callCount);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_WebBrowserShortcutsEnabled_Set_GetReturnsExpected(bool value)
    {
        using WebBrowser control = new()
        {
            WebBrowserShortcutsEnabled = value
        };
        Assert.Equal(value, control.WebBrowserShortcutsEnabled);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set same.
        control.WebBrowserShortcutsEnabled = value;
        Assert.Equal(value, control.WebBrowserShortcutsEnabled);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Set different.
        control.WebBrowserShortcutsEnabled = !value;
        Assert.Equal(!value, control.WebBrowserShortcutsEnabled);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_WebBrowserShortcutsEnabled_SetWithInstance_GetReturnsExpected(bool value)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.WebBrowserShortcutsEnabled = value;
        Assert.Equal(value, control.WebBrowserShortcutsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set same.
        control.WebBrowserShortcutsEnabled = value;
        Assert.Equal(value, control.WebBrowserShortcutsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Set different.
        control.WebBrowserShortcutsEnabled = !value;
        Assert.Equal(!value, control.WebBrowserShortcutsEnabled);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_AttachInterfaces_Invoke_Success()
    {
        object nativeActiveXObject;

        try
        {
            Type t = Type.GetTypeFromCLSID(new Guid("0002DF01-0000-0000-C000-000000000046"));
            nativeActiveXObject = Activator.CreateInstance(t);
        }
        catch (COMException)
        {
            // Windows doesn't have IE browser capability installed,
            // run 'Get-WindowsCapability -online' for more details
            //
            // xUnit doesn't support dynamic test skipping, https://github.com/xunit/xunit/issues/2073
            // just return. The test will be marked as success, but it is better than just fail.
            //
            // Skip.If(true, "Windows doesn't have IE browser capability installed");
            return;
        }

        using SubWebBrowser control = new();
        control.AttachInterfaces(nativeActiveXObject);

        // Attach again.
        control.AttachInterfaces(nativeActiveXObject);

        control.DetachInterfaces();

        // Attach null.
        control.AttachInterfaces(null);
    }

    [WinFormsFact]
    public void WebBrowser_AttachInterfaces_InvokeWithInstance_Success()
    {
        object nativeActiveXObject;

        try
        {
            Type t = Type.GetTypeFromCLSID(new Guid("0002DF01-0000-0000-C000-000000000046"));
            nativeActiveXObject = Activator.CreateInstance(t);
        }
        catch (COMException)
        {
            // Windows doesn't have IE browser capability installed,
            // run 'Get-WindowsCapability -online' for more details
            //
            // xUnit doesn't support dynamic test skipping, https://github.com/xunit/xunit/issues/2073
            // just return. The test will be marked as success, but it is better than just fail.
            //
            // Skip.If(true, "Windows doesn't have IE browser capability installed");
            return;
        }

        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.AttachInterfaces(nativeActiveXObject);

        // Attach again.
        control.AttachInterfaces(nativeActiveXObject);

        control.DetachInterfaces();

        // Attach null.
        control.AttachInterfaces(null);
    }

    [WinFormsFact]
    public void WebBrowser_AttachInterfaces_InvalidNativeActiveXObject_ThrowsInvalidCastException()
    {
        using SubWebBrowser control = new();
        Assert.Throws<InvalidCastException>(() => control.AttachInterfaces(new object()));
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_CreateSink_InvokeWithInstance_Success(bool allowNavigation)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation,
            Parent = parent
        };
        control.CreateSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        control.CreateSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_CreateSink_InvokeWithoutInstance_Nop(bool allowNavigation)
    {
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation
        };
        control.CreateSink();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        control.CreateSink();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_CreateWebBrowserSiteBase_Invoke_ReturnsExpected()
    {
        using SubWebBrowser control = new();
        WebBrowserSiteBase siteBase = control.CreateWebBrowserSiteBase();
        Assert.NotNull(siteBase);
        Assert.NotSame(siteBase, control.CreateWebBrowserSiteBase());
    }

    [WinFormsFact]
    public void WebBrowser_DetachInterfaces_Invoke_Success()
    {
        using SubWebBrowser control = new();
        control.DetachInterfaces();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.DetachInterfaces();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_DetachInterfaces_InvokeWithInstance_Success()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };

        control.DetachInterfaces();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Call again.
        control.DetachInterfaces();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_DetachSink_InvokeWithCreatedSink_Success(bool allowNavigation)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation,
            Parent = parent
        };
        control.CreateSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        control.DetachSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Call again.
        control.DetachSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_DetachSink_InvokeWithInstance_Success(bool allowNavigation)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation,
            Parent = parent
        };

        control.DetachSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Call again.
        control.DetachSink();
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_DetachSink_InvokeWithoutInstance_Nop(bool allowNavigation)
    {
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation
        };
        control.DetachSink();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Call again.
        control.DetachSink();
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Dispose_Invoke_Success()
    {
        using WebBrowser control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            Assert.Null(control.ActiveXInstance);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowser_Dispose_InvokeWithInstance_Success(bool allowNavigation)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation,
            Parent = parent
        };

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            Assert.Null(control.ActiveXInstance);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose();
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void WebBrowserDispose_InvokeDisposing_Success()
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            Assert.Null(control.ActiveXInstance);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowserDispose_InvokeNotDisposing_Success(bool allowNavigation)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation,
            Parent = parent
        };

        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.NotNull(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.NotNull(control.ActiveXInstance);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.NotNull(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.NotNull(control.ActiveXInstance);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsTheory]
    [BoolData]
    public void WebBrowserDispose_InvokeDisposingWithInstance_Success(bool allowNavigation)
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            AllowNavigation = allowNavigation,
            Parent = parent
        };

        int callCount = 0;
        void handler(object sender, EventArgs e)
        {
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsHandleCreated);
            Assert.True(control.Disposing);
            Assert.Equal(callCount > 0, control.IsDisposed);
            Assert.Null(control.ActiveXInstance);
            callCount++;
        }

        control.Disposed += handler;

        try
        {
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(1, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(true);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.True(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(2, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void WebBrowserDispose_InvokeNotDisposingWithInstance_Success()
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        void handler(object sender, EventArgs e) => callCount++;
        control.Disposed += handler;

        try
        {
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);

            // Dispose multiple times.
            control.Dispose(false);
            Assert.Null(control.Parent);
            Assert.Empty(control.Controls);
            Assert.Empty(control.DataBindings);
            Assert.False(control.IsDisposed);
            Assert.False(control.Disposing);
            Assert.Null(control.ActiveXInstance);
            Assert.Equal(0, callCount);
            Assert.False(control.IsHandleCreated);
        }
        finally
        {
            control.Disposed -= handler;
        }
    }

    [WinFormsFact]
    public void WebBrowser_GetAutoSizeMode_Invoke_ReturnsExpected()
    {
        using SubWebBrowser control = new();
        Assert.Equal(AutoSizeMode.GrowOnly, control.GetAutoSizeMode());
    }

    [WinFormsTheory]
    [InlineData(ControlStyles.ContainerControl, false)]
    [InlineData(ControlStyles.UserPaint, false)]
    [InlineData(ControlStyles.Opaque, false)]
    [InlineData(ControlStyles.ResizeRedraw, false)]
    [InlineData(ControlStyles.FixedWidth, false)]
    [InlineData(ControlStyles.FixedHeight, false)]
    [InlineData(ControlStyles.StandardClick, true)]
    [InlineData(ControlStyles.Selectable, true)]
    [InlineData(ControlStyles.UserMouse, false)]
    [InlineData(ControlStyles.SupportsTransparentBackColor, false)]
    [InlineData(ControlStyles.StandardDoubleClick, true)]
    [InlineData(ControlStyles.AllPaintingInWmPaint, true)]
    [InlineData(ControlStyles.CacheText, false)]
    [InlineData(ControlStyles.EnableNotifyMessage, false)]
    [InlineData(ControlStyles.DoubleBuffer, false)]
    [InlineData(ControlStyles.OptimizedDoubleBuffer, false)]
    [InlineData(ControlStyles.UseTextForAccessibility, true)]
    [InlineData((ControlStyles)0, true)]
    [InlineData((ControlStyles)int.MaxValue, false)]
    [InlineData((ControlStyles)(-1), false)]
    public void WebBrowser_GetStyle_Invoke_ReturnsExpected(ControlStyles flag, bool expected)
    {
        using SubWebBrowser control = new();
        Assert.Equal(expected, control.GetStyle(flag));

        // Call again to test caching.
        Assert.Equal(expected, control.GetStyle(flag));
    }

    [WinFormsFact]
    public void WebBrowser_GoBack_InvokeCantGoBack_Nop()
    {
        using WebBrowser control = new();
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        Assert.False(control.GoBack());
        Assert.Null(control.Url);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_GoBack_InvokeCantGoBackWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        Assert.False(control.GoBack());
        Assert.Null(control.Url);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_GoBack_Invoke_CallsCanGoBackChanged()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            canGoBackChangedCallCount++;
        };
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            canGoForwardChangedCallCount++;
        };

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file1 = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file1.Path));
        Assert.True(await source.Task);
        Assert.False(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Navigate.
        using var file2 = CreateTempFile(Html);
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(file2.Path));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Navigate again.
        using var file3 = CreateTempFile(Html);
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(file3.Path));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Go back.
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => Assert.True(control.GoBack()));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.True(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(1, canGoForwardChangedCallCount);

        // Go back again.
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => Assert.True(control.GoBack()));
        Assert.True(await source.Task);
        Assert.False(control.CanGoBack);
        Assert.True(control.CanGoForward);
        Assert.Equal(2, canGoBackChangedCallCount);
        Assert.Equal(1, canGoForwardChangedCallCount);

        // Go back again.
        Assert.False(control.GoBack());
        Assert.False(control.CanGoBack);
        Assert.True(control.CanGoForward);
        Assert.Equal(2, canGoBackChangedCallCount);
        Assert.Equal(1, canGoForwardChangedCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_GoBack_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.False(control.GoBack());
    }

    [WinFormsFact]
    public void WebBrowser_GoForward_InvokeCantGoForward_Nop()
    {
        using WebBrowser control = new();
        Assert.False(control.GoForward());
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_GoForward_InvokeCantGoForwardWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        Assert.False(control.GoForward());
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_GoForward_Invoke_CallsCanGoForwardChanged()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            canGoBackChangedCallCount++;
        };
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            canGoForwardChangedCallCount++;
        };

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file1 = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file1.Path));
        Assert.True(await source.Task);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Navigate.
        using var file2 = CreateTempFile(Html);
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(file2.Path));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Navigate again.
        using var file3 = CreateTempFile(Html);
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(file3.Path));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Go back.
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => Assert.True(control.GoBack()));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.True(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(1, canGoForwardChangedCallCount);

        // Go forward.
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => Assert.True(control.GoForward()));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(2, canGoForwardChangedCallCount);

        // Go forward again.
        Assert.False(control.GoForward());
        Assert.True(control.CanGoBack);
        Assert.False(control.CanGoForward);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(2, canGoForwardChangedCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_GoForward_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.False(control.GoForward());
    }

    [WinFormsFact]
    public void WebBrowser_GoHome_InvokeCantGoHome_Nop()
    {
        using WebBrowser control = new();
        control.GoHome();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_GoHome_InvokeCantGoHomeWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.GoHome();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_GoHome_InvokeDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(control.GoHome);
    }

    [WinFormsFact]
    public void WebBrowser_GoHome_InvokeDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(control.GoHome);
    }

    [WinFormsFact]
    public void WebBrowser_GoSearch_InvokeCantGoSearch_Nop()
    {
        using WebBrowser control = new();
        control.GoSearch();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_GoSearch_InvokeCantGoSearchWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.GoSearch();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_GoSearch_InvokeDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(control.GoSearch);
    }

    [WinFormsFact]
    public void WebBrowser_GoSearch_InvokeDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(control.GoSearch);
    }

    [WinFormsFact]
    public async Task WebBrowser_Navigate_InvokeString_CallsMethods()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        int navigatingCallCount = 0;
        control.Navigating += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            Assert.Empty(e.TargetFrameName);
            Assert.False(e.Cancel);
            navigatingCallCount++;
        };
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            navigatedCallCount++;
        };
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            documentTitleChangedCallCount++;
        };
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            documentCompletedCallCount++;
            source.SetResult(true);
        };
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            encryptionLevelChangedCallCount++;
        };
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            statusTextChangedCallCount++;
        };
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(1, navigatingCallCount);
        Assert.Equal(1, navigatedCallCount);
        Assert.Equal(2, documentTitleChangedCallCount);
        Assert.Equal(1, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);

        // Call again
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(2, navigatingCallCount);
        Assert.Equal(2, navigatedCallCount);
        Assert.Equal(4, documentTitleChangedCallCount);
        Assert.Equal(2, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);
    }

    [WinFormsTheory]
    [NullAndEmptyStringData]
    public async Task WebBrowser_Navigate_NullOrEmptyString_GoesToBlank(string nullOrEmptyString)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        await Task.Run(() => control.Navigate(new Uri(file.Path)));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);

        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(nullOrEmptyString));
        Assert.True(await source.Task);
        Assert.Equal("about:blank", control.Url.OriginalString);
    }

    [WinFormsFact]
    public async Task WebBrowser_Navigate_InvokeUri_CallsMethods()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        int navigatingCallCount = 0;
        control.Navigating += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            Assert.Empty(e.TargetFrameName);
            Assert.False(e.Cancel);
            navigatingCallCount++;
        };
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            navigatedCallCount++;
        };
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            documentTitleChangedCallCount++;
        };
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Equal(new Uri(file.Path), e.Url);
            documentCompletedCallCount++;
            source.SetResult(true);
        };
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            encryptionLevelChangedCallCount++;
        };
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            statusTextChangedCallCount++;
        };
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        await Task.Run(() => control.Navigate(new Uri(file.Path)));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(1, navigatingCallCount);
        Assert.Equal(1, navigatedCallCount);
        Assert.Equal(2, documentTitleChangedCallCount);
        Assert.Equal(1, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);

        // Call again.
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(new Uri(file.Path)));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(2, navigatingCallCount);
        Assert.Equal(2, navigatedCallCount);
        Assert.Equal(4, documentTitleChangedCallCount);
        Assert.Equal(2, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount > 0);
        Assert.True(statusTextChangedCallCount > 0);
    }

    public static IEnumerable<object[]> Navigate_NullOrEmptyUri_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new Uri("", UriKind.Relative) };
    }

    [WinFormsTheory]
    [MemberData(nameof(Navigate_NullOrEmptyUri_TestData))]
    public async Task WebBrowser_Navigate_NullOrEmptyUri_GoesToBlank(Uri nullOrEmptyUri)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);

        await Task.Run(() => control.Navigate(new Uri(file.Path)));
        Assert.True(await source.Task);
        Assert.Equal(new Uri(file.Path), control.Url);

        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(nullOrEmptyUri));
        Assert.True(await source.Task);
        Assert.Equal("about:blank", control.Url.OriginalString);
    }

    [WinFormsFact]
    public async Task WebBrowser_Navigate_InvokeWithDocument_CallsCanGoBackChanged()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        TaskCompletionSource<bool> source = new();
        control.DocumentCompleted += (sender, e) => source.SetResult(true);
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            canGoBackChangedCallCount++;
        };
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file1 = CreateTempFile(Html);
        await Task.Run(() => control.Navigate(file1.Path));
        Assert.True(await source.Task);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);

        // Navigate again.
        using var file2 = CreateTempFile(Html);
        source = new TaskCompletionSource<bool>();
        await Task.Run(() => control.Navigate(file2.Path));
        Assert.True(await source.Task);
        Assert.True(control.CanGoBack);
        Assert.Equal(1, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_Navigate_RelativeUri_ThrowsArgumentException()
    {
        using WebBrowser control = new();
        Uri relativeUri = new("/path", UriKind.Relative);
        Assert.Throws<ArgumentException>(() => control.Navigate(relativeUri));
        Assert.Throws<ArgumentException>(() => control.Navigate(relativeUri, "targetFrameName"));
        Assert.Throws<ArgumentException>(() => control.Navigate(relativeUri, false));
        Assert.Throws<ArgumentException>(() => control.Navigate(relativeUri, "targetFrameName", null, null));
    }

    [WinFormsFact]
    public void WebBrowser_Navigate_InvokeDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.Navigate("about:blank"));
        Assert.Throws<ObjectDisposedException>(() => control.Navigate(new Uri("http://google.com")));
        Assert.Throws<ObjectDisposedException>(() => control.Navigate(new Uri("http://google.com"), "targetFrameName"));
        Assert.Throws<ObjectDisposedException>(() => control.Navigate(new Uri("http://google.com"), false));
        Assert.Throws<ObjectDisposedException>(() => control.Navigate(new Uri("http://google.com"), "targetFrameName", null, null));
    }

    [WinFormsFact]
    public void WebBrowser_Navigate_InvokeDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.Navigate("about:blank"));
        Assert.Throws<InvalidOperationException>(() => control.Navigate(new Uri("http://google.com")));
        Assert.Throws<InvalidOperationException>(() => control.Navigate(new Uri("http://google.com"), "targetFrameName"));
        Assert.Throws<InvalidOperationException>(() => control.Navigate(new Uri("http://google.com"), false));
        Assert.Throws<InvalidOperationException>(() => control.Navigate(new Uri("http://google.com"), "targetFrameName", null, null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void WebBrowser_OnCanGoBackChanged_Invoke_CallsCanGoBackChanged(EventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.CanGoBackChanged += handler;
        control.OnCanGoBackChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.CanGoBackChanged -= handler;
        control.OnCanGoBackChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void WebBrowser_OnCanGoForwardChanged_Invoke_CallsCanGoForwardChanged(EventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.CanGoForwardChanged += handler;
        control.OnCanGoForwardChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.CanGoForwardChanged -= handler;
        control.OnCanGoForwardChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnDocumentCompleted_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new WebBrowserDocumentCompletedEventArgs(null) };
        yield return new object[] { new WebBrowserDocumentCompletedEventArgs(new Uri("http://microsoft.com")) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnDocumentCompleted_TestData))]
    public void WebBrowser_OnDocumentCompleted_Invoke_CallsDocumentCompleted(WebBrowserDocumentCompletedEventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        WebBrowserDocumentCompletedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.DocumentCompleted += handler;
        control.OnDocumentCompleted(eventArgs);
        Assert.Equal(1, callCount);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.DocumentCompleted -= handler;
        control.OnDocumentCompleted(eventArgs);
        Assert.Equal(1, callCount);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnDocumentCompleted_WithInstance_TestData()
    {
        foreach (bool allowWebBrowserDrop in new bool[] { true, false })
        {
            yield return new object[] { allowWebBrowserDrop, null };
            yield return new object[] { allowWebBrowserDrop, new WebBrowserDocumentCompletedEventArgs(null) };
            yield return new object[] { allowWebBrowserDrop, new WebBrowserDocumentCompletedEventArgs(new Uri("http://microsoft.com")) };
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(OnDocumentCompleted_WithInstance_TestData))]
    public unsafe void WebBrowser_OnDocumentCompleted_InvokeWithInstance_CallsDocumentCompleted(bool allowWebBrowserDrop, WebBrowserDocumentCompletedEventArgs eventArgs)
    {
        Control parent = new();
        SubWebBrowser control = new()
        {
            Parent = parent,
            AllowWebBrowserDrop = allowWebBrowserDrop
        };

        int callCount = 0;
        WebBrowserDocumentCompletedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };
        using var webBrowser = ComHelpers.GetComScope<IWebBrowser2>(control.ActiveXInstance);
        Assert.True(webBrowser.Value->put_RegisterAsDropTarget(!allowWebBrowserDrop).Succeeded);

        // Call with handler.
        control.DocumentCompleted += handler;
        control.OnDocumentCompleted(eventArgs);
        Assert.Equal(1, callCount);
        Assert.NotNull(control.ActiveXInstance);
        VARIANT_BOOL register = default;
        Assert.True(webBrowser.Value->get_RegisterAsDropTarget(&register).Succeeded);
        Assert.Equal(!allowWebBrowserDrop, (bool)register);
        Assert.True(control.IsHandleCreated);

        // Remove handler.
        control.DocumentCompleted -= handler;
        control.OnDocumentCompleted(eventArgs);
        Assert.Equal(1, callCount);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(webBrowser.Value->get_RegisterAsDropTarget(&register).Succeeded);
        Assert.Equal(!allowWebBrowserDrop, (bool)register);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_OnDocumentCompleted_InvokeDisposed_ThrowsObjectDisposedException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        Assert.Throws<ObjectDisposedException>(() => control.OnDocumentCompleted(null));
    }

    [WinFormsFact]
    public void WebBrowser_OnDocumentCompleted_InvokeDetached_ThrowsInvalidOperationException()
    {
        using Control parent = new();
        using SubWebBrowser control = new()
        {
            Parent = parent
        };
        control.DetachInterfaces();
        Assert.Throws<InvalidOperationException>(() => control.OnDocumentCompleted(null));
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void WebBrowser_OnDocumentTitleChanged_Invoke_CallsDocumentTitleChanged(EventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.DocumentTitleChanged += handler;
        control.OnDocumentTitleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.DocumentTitleChanged -= handler;
        control.OnDocumentTitleChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void WebBrowser_OnEncryptionLevelChanged_Invoke_CallsEncryptionLevelChanged(EventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.EncryptionLevelChanged += handler;
        control.OnEncryptionLevelChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.EncryptionLevelChanged -= handler;
        control.OnEncryptionLevelChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void WebBrowser_OnFileDownload_Invoke_CallsFileDownload(EventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.FileDownload += handler;
        control.OnFileDownload(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.FileDownload -= handler;
        control.OnFileDownload(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnNavigated_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new WebBrowserNavigatedEventArgs(null) };
        yield return new object[] { new WebBrowserNavigatedEventArgs(new Uri("http://microsoft.com")) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnNavigated_TestData))]
    public void WebBrowser_OnNavigated_Invoke_CallsNavigated(WebBrowserNavigatedEventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        WebBrowserNavigatedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Navigated += handler;
        control.OnNavigated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Navigated -= handler;
        control.OnNavigated(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnNavigating_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new WebBrowserNavigatingEventArgs(null, null) };
        yield return new object[] { new WebBrowserNavigatingEventArgs(new Uri("http://microsoft.com"), "targetFrameName") };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnNavigating_TestData))]
    public void WebBrowser_OnNavigating_Invoke_CallsNavigating(WebBrowserNavigatingEventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        WebBrowserNavigatingEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.Navigating += handler;
        control.OnNavigating(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.Navigating -= handler;
        control.OnNavigating(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnNewWindow_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new CancelEventArgs(false) };
        yield return new object[] { new CancelEventArgs(true) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnNewWindow_TestData))]
    public void WebBrowser_OnNewWindow_Invoke_CallsNewWindow(CancelEventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        CancelEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.NewWindow += handler;
        control.OnNewWindow(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.NewWindow -= handler;
        control.OnNewWindow(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    public static IEnumerable<object[]> OnProgressChanged_TestData()
    {
        yield return new object[] { null };
        yield return new object[] { new WebBrowserProgressChangedEventArgs(-1, -2) };
        yield return new object[] { new WebBrowserProgressChangedEventArgs(2, 1) };
        yield return new object[] { new WebBrowserProgressChangedEventArgs(1, 100) };
    }

    [WinFormsTheory]
    [MemberData(nameof(OnProgressChanged_TestData))]
    public void WebBrowser_OnProgressChanged_Invoke_CallsProgressChanged(WebBrowserProgressChangedEventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        WebBrowserProgressChangedEventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.ProgressChanged += handler;
        control.OnProgressChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.ProgressChanged -= handler;
        control.OnProgressChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [NewAndDefaultData<EventArgs>]
    public void WebBrowser_OnStatusTextChanged_Invoke_CallsStatusTextChanged(EventArgs eventArgs)
    {
        using SubWebBrowser control = new();
        int callCount = 0;
        EventHandler handler = (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(eventArgs, e);
            callCount++;
        };

        // Call with handler.
        control.StatusTextChanged += handler;
        control.OnStatusTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);

        // Remove handler.
        control.StatusTextChanged -= handler;
        control.OnStatusTextChanged(eventArgs);
        Assert.Equal(1, callCount);
        Assert.Null(control.ActiveXInstance);
        Assert.False(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Print_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.Print();
    }

    [WinFormsFact]
    public void WebBrowser_Refresh_InvokeCantRefresh_Nop()
    {
        using WebBrowser control = new();
        control.Refresh();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Refresh_InvokeCantRefreshWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.Refresh();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public async Task WebBrowser_Refresh_InvokeWithDocument_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        void oldHandler(object sender, WebBrowserDocumentCompletedEventArgs e) => source.SetResult(true);
        control.DocumentCompleted += oldHandler;
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);
        control.DocumentCompleted -= oldHandler;

        int navigatingCallCount = 0;
        control.Navigating += (sender, e) => navigatingCallCount++;
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) => navigatedCallCount++;
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) => documentTitleChangedCallCount++;
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) => documentCompletedCallCount++;
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) => encryptionLevelChangedCallCount++;
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) => statusTextChangedCallCount++;
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        control.Refresh();
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.True(documentTitleChangedCallCount >= 0);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount >= 0);
        Assert.True(statusTextChangedCallCount >= 0);

        // Call again
        control.Refresh();
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.True(documentTitleChangedCallCount >= 0);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount >= 0);
        Assert.True(statusTextChangedCallCount >= 0);
    }

    [WinFormsFact]
    public async Task WebBrowser_Refresh_InvokeWithDocumentText_Success()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        void oldHandler(object sender, WebBrowserDocumentCompletedEventArgs e) => source.SetResult(true);
        control.DocumentCompleted += oldHandler;
        await Task.Run(() => control.DocumentText = Html);
        Assert.True(await source.Task);
        control.DocumentCompleted -= oldHandler;

        int navigatingCallCount = 0;
        control.Navigating += (sender, e) => navigatingCallCount++;
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) => navigatedCallCount++;
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) => documentTitleChangedCallCount++;
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) => documentCompletedCallCount++;
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) => encryptionLevelChangedCallCount++;
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) => statusTextChangedCallCount++;
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        control.Refresh();
        Assert.Equal("about:blank", control.Url.OriginalString);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.Equal(0, documentTitleChangedCallCount);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.Equal(0, encryptionLevelChangedCallCount);
        Assert.Equal(0, statusTextChangedCallCount);

        // Call again
        control.Refresh();
        Assert.Equal("about:blank", control.Url.OriginalString);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.Equal(0, documentTitleChangedCallCount);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.Equal(0, encryptionLevelChangedCallCount);
        Assert.Equal(0, statusTextChangedCallCount);
    }

    [WinFormsTheory]
    [EnumData<WebBrowserRefreshOption>]
    [InvalidEnumData<WebBrowserRefreshOption>]
    public void WebBrowser_Refresh_InvokeWebBrowserRefreshOptionCantRefresh_Nop(WebBrowserRefreshOption opt)
    {
        using WebBrowser control = new();
        control.Refresh(opt);
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<WebBrowserRefreshOption>]
    [InvalidEnumData<WebBrowserRefreshOption>]
    public void WebBrowser_Refresh_InvokeWebBrowserRefreshOptionCantRefreshWithInstance_Nop(WebBrowserRefreshOption opt)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.Refresh(opt);
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsTheory]
    [EnumData<WebBrowserRefreshOption>]
    [InvalidEnumData<WebBrowserRefreshOption>]
    public async Task WebBrowser_Refresh_InvokeWebBrowserRefreshOptionWithDocument_Success(WebBrowserRefreshOption opt)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        void oldHandler(object sender, WebBrowserDocumentCompletedEventArgs e) => source.SetResult(true);
        control.DocumentCompleted += oldHandler;
        await Task.Run(() => control.Navigate(file.Path));
        Assert.True(await source.Task);
        control.DocumentCompleted -= oldHandler;

        int navigatingCallCount = 0;
        control.Navigating += (sender, e) => navigatingCallCount++;
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) => navigatedCallCount++;
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) => documentTitleChangedCallCount++;
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) => documentCompletedCallCount++;
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) => encryptionLevelChangedCallCount++;
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) => statusTextChangedCallCount++;
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        control.Refresh(opt);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.True(documentTitleChangedCallCount >= 0);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount >= 0);
        Assert.True(statusTextChangedCallCount >= 0);

        // Call again
        control.Refresh(opt);
        Assert.Equal(new Uri(file.Path), control.Url);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.True(documentTitleChangedCallCount >= 0);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.True(encryptionLevelChangedCallCount >= 0);
        Assert.True(statusTextChangedCallCount >= 0);
    }

    [WinFormsTheory]
    [EnumData<WebBrowserRefreshOption>]
    [InvalidEnumData<WebBrowserRefreshOption>]
    public async Task WebBrowser_Refresh_InvokeWebBrowserRefreshOptionWithDocumentText_Success(WebBrowserRefreshOption opt)
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        const string Html = "<html><head><title>NewDocument</title></head></html>";
        using var file = CreateTempFile(Html);
        TaskCompletionSource<bool> source = new();
        void oldHandler(object sender, WebBrowserDocumentCompletedEventArgs e) => source.SetResult(true);
        control.DocumentCompleted += oldHandler;
        await Task.Run(() => control.DocumentText = Html);
        Assert.True(await source.Task);
        control.DocumentCompleted -= oldHandler;

        int navigatingCallCount = 0;
        control.Navigating += (sender, e) => navigatingCallCount++;
        int navigatedCallCount = 0;
        control.Navigated += (sender, e) => navigatedCallCount++;
        int documentTitleChangedCallCount = 0;
        control.DocumentTitleChanged += (sender, e) => documentTitleChangedCallCount++;
        int documentCompletedCallCount = 0;
        control.DocumentCompleted += (sender, e) => documentCompletedCallCount++;
        int encryptionLevelChangedCallCount = 0;
        control.EncryptionLevelChanged += (sender, e) => encryptionLevelChangedCallCount++;
        int statusTextChangedCallCount = 0;
        control.StatusTextChanged += (sender, e) => statusTextChangedCallCount++;
        int canGoBackChangedCallCount = 0;
        control.CanGoBackChanged += (sender, e) => canGoBackChangedCallCount++;
        int canGoForwardChangedCallCount = 0;
        control.CanGoForwardChanged += (sender, e) => canGoForwardChangedCallCount++;

        control.Refresh(opt);
        Assert.Equal("about:blank", control.Url.OriginalString);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.Equal(0, documentTitleChangedCallCount);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.Equal(0, encryptionLevelChangedCallCount);
        Assert.Equal(0, statusTextChangedCallCount);

        // Call again
        control.Refresh(opt);
        Assert.Equal("about:blank", control.Url.OriginalString);
        Assert.Equal(0, navigatingCallCount);
        Assert.Equal(0, navigatedCallCount);
        Assert.Equal(0, documentTitleChangedCallCount);
        Assert.Equal(0, documentCompletedCallCount);
        Assert.Equal(0, canGoBackChangedCallCount);
        Assert.Equal(0, canGoForwardChangedCallCount);
        Assert.Equal(0, encryptionLevelChangedCallCount);
        Assert.Equal(0, statusTextChangedCallCount);
    }

    [WinFormsFact]
    public void WebBrowser_Refresh_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.Refresh();
        control.Refresh(WebBrowserRefreshOption.Normal);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPageSetupDialog_Invoke_Nop()
    {
        using WebBrowser control = new();
        control.ShowPageSetupDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPageSetupDialog_InvokeWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ShowPageSetupDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPageSetupDialog_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.ShowPageSetupDialog();
    }

    [WinFormsFact]
    public void WebBrowser_ShowPrintDialog_Invoke_Nop()
    {
        using WebBrowser control = new();
        control.ShowPrintDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPrintDialog_InvokeWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ShowPrintDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPrintDialog_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.ShowPrintDialog();
    }

    [WinFormsFact]
    public void WebBrowser_ShowPrintPreviewDialog_Invoke_Nop()
    {
        using WebBrowser control = new();
        control.ShowPrintPreviewDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPrintPreviewDialog_InvokeWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ShowPrintPreviewDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPrintPreviewDialog_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.ShowPrintPreviewDialog();
    }

    [WinFormsFact]
    public void WebBrowser_ShowPropertiesDialog_Invoke_Nop()
    {
        using WebBrowser control = new();
        control.ShowPropertiesDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPropertiesDialog_InvokeWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ShowPropertiesDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowPropertiesDialog_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.ShowPropertiesDialog();
    }

    [WinFormsFact]
    public void WebBrowser_ShowSaveAsDialog_Invoke_Nop()
    {
        using WebBrowser control = new();
        control.ShowSaveAsDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowSaveAsDialog_InvokeWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.ShowSaveAsDialog();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_ShowSaveAsDialog_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.ShowSaveAsDialog();
    }

    [WinFormsFact]
    public void WebBrowser_Stop_InvokeCantStop_Nop()
    {
        using WebBrowser control = new();
        control.Stop();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Stop_InvokeCantStopWithInstance_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };

        control.Stop();
        Assert.Null(control.Url);
        Assert.NotNull(control.ActiveXInstance);
        Assert.True(control.IsHandleCreated);
    }

    [WinFormsFact]
    public void WebBrowser_Stop_InvokeDisposed_Nop()
    {
        using Control parent = new();
        using WebBrowser control = new()
        {
            Parent = parent
        };
        control.Dispose();
        control.Stop();
    }

    [WinFormsFact]
    public void WebBrowser_WndProc_InvokeMouseHoverWithHandle_Success()
    {
        using SubWebBrowser control = new();
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        int callCount = 0;
        ((Control)control).MouseHover += (sender, e) =>
        {
            Assert.Same(control, sender);
            Assert.Same(EventArgs.Empty, e);
            callCount++;
        };
        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_MOUSEHOVER,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.Equal(1, callCount);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithoutContextMenuStrip_TestData()
    {
        yield return new object[] { new Size(10, 20), (IntPtr)(-1) };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(0, 0) };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(1, 2) };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(-1, -2) };

        yield return new object[] { Size.Empty, (IntPtr)(-1) };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(0, 0) };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(1, 2) };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(-1, -2) };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithoutContextMenuStrip_TestData))]
    public void WebBrowser_WndProc_InvokeContextMenuWithoutContextMenuStripWithoutHandle_Success(Size size, IntPtr lParam)
    {
        using (new NoAssertContext())
        {
            using SubWebBrowser control = new()
            {
                Size = size
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CONTEXTMENU,
                LParam = lParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(IntPtr.Zero, m.Result);
            Assert.False(control.IsHandleCreated);
        }
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithContextMenuStripWithoutHandle_TestData()
    {
        using Control control = new();
        Point p = control.PointToScreen(new Point(5, 5));

        yield return new object[] { new Size(10, 20), (IntPtr)(-1), (IntPtr)250, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(0, 0), IntPtr.Zero, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(1, 2), IntPtr.Zero, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(p.X, p.Y), (IntPtr)250, true };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(-1, -2), (IntPtr)250, true };

        yield return new object[] { Size.Empty, (IntPtr)(-1), IntPtr.Zero, false };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(0, 0), IntPtr.Zero, true };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(1, 2), IntPtr.Zero, true };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(p.X, p.Y), IntPtr.Zero, true };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(-1, -2), IntPtr.Zero, false };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithContextMenuStripWithoutHandle_TestData))]
    public void WebBrowser_WndProc_InvokeContextMenuWithContextMenuStripWithoutHandle_Success(Size size, IntPtr lParam, IntPtr expectedResult, bool expectedHandleCreated)
    {
        using (new NoAssertContext())
        {
            using ContextMenuStrip menu = new();
            using SubWebBrowser control = new()
            {
                ContextMenuStrip = menu,
                Size = size
            };
            Message m = new()
            {
                Msg = (int)PInvokeCore.WM_CONTEXTMENU,
                LParam = lParam,
                Result = 250
            };
            control.WndProc(ref m);
            Assert.Equal(expectedResult, m.Result);
            Assert.False(menu.Visible);
            Assert.Equal(expectedResult == 250, menu.SourceControl == control);
            Assert.Equal(expectedHandleCreated, control.IsHandleCreated);
        }
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithoutContextMenuStrip_TestData))]
    public void WebBrowser_WndProc_InvokeContextMenuWithoutContextMenuStripWithHandle_Success(Size size, IntPtr lParam)
    {
        using SubWebBrowser control = new()
        {
            Size = size
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CONTEXTMENU,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(IntPtr.Zero, m.Result);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    public static IEnumerable<object[]> WndProc_ContextMenuWithContextMenuStripWithHandle_TestData()
    {
        using Control control = new();
        Point p = control.PointToScreen(new Point(5, 5));

        yield return new object[] { new Size(10, 20), (IntPtr)(-1), (IntPtr)250 };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(0, 0), IntPtr.Zero };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(1, 2), IntPtr.Zero };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(p.X, p.Y), (IntPtr)250 };
        yield return new object[] { new Size(10, 20), PARAM.FromLowHigh(-1, -2), (IntPtr)250 };

        yield return new object[] { Size.Empty, (IntPtr)(-1), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(0, 0), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(1, 2), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(p.X, p.Y), IntPtr.Zero };
        yield return new object[] { Size.Empty, PARAM.FromLowHigh(-1, -2), IntPtr.Zero };
    }

    [WinFormsTheory]
    [MemberData(nameof(WndProc_ContextMenuWithContextMenuStripWithHandle_TestData))]
    public void WebBrowser_WndProc_InvokeContextMenuWithContextMenuStripWithHandle_Success(Size size, IntPtr lParam, IntPtr expectedResult)
    {
        using ContextMenuStrip menu = new();
        using SubWebBrowser control = new()
        {
            ContextMenuStrip = menu,
            Size = size
        };
        Assert.NotEqual(IntPtr.Zero, control.Handle);
        int invalidatedCallCount = 0;
        control.Invalidated += (sender, e) => invalidatedCallCount++;
        int styleChangedCallCount = 0;
        ((Control)control).StyleChanged += (sender, e) => styleChangedCallCount++;
        int createdCallCount = 0;
        control.HandleCreated += (sender, e) => createdCallCount++;

        Message m = new()
        {
            Msg = (int)PInvokeCore.WM_CONTEXTMENU,
            LParam = lParam,
            Result = 250
        };
        control.WndProc(ref m);
        Assert.Equal(expectedResult, m.Result);
        Assert.False(menu.Visible);
        Assert.Equal(expectedResult == 250, menu.SourceControl == control);
        Assert.True(control.IsHandleCreated);
        Assert.Equal(0, invalidatedCallCount);
        Assert.Equal(0, styleChangedCallCount);
        Assert.Equal(0, createdCallCount);
    }

    private static TempFile CreateTempFile(string html)
    {
        byte[] data = Encoding.UTF8.GetBytes(html);
        return TempFile.Create(data);
    }

    private class PrivateClass
    {
    }

    // This class must be ComVisible because WebBrowser scripting requires IDispatch and ITypeInfo support.
    [ComVisible(true)]
    [ClassInterface(ClassInterfaceType.AutoDispatch)]
    public class CustomScriptingObject
    {
    }

    private class SubWebBrowser : WebBrowser
    {
        public new bool CanEnableIme => base.CanEnableIme;

        public new bool CanRaiseEvents => base.CanRaiseEvents;

        public new CreateParams CreateParams => base.CreateParams;

        public new Cursor DefaultCursor => base.DefaultCursor;

        public new ImeMode DefaultImeMode => base.DefaultImeMode;

        public new Padding DefaultMargin => base.DefaultMargin;

        public new Size DefaultMaximumSize => base.DefaultMaximumSize;

        public new Size DefaultMinimumSize => base.DefaultMinimumSize;

        public new Padding DefaultPadding => base.DefaultPadding;

        public new Size DefaultSize => base.DefaultSize;

        public new bool DesignMode => base.DesignMode;

        public new bool DoubleBuffered
        {
            get => base.DoubleBuffered;
            set => base.DoubleBuffered = value;
        }

        public new EventHandlerList Events => base.Events;

        public new int FontHeight
        {
            get => base.FontHeight;
            set => base.FontHeight = value;
        }

        public new ImeMode ImeModeBase
        {
            get => base.ImeModeBase;
            set => base.ImeModeBase = value;
        }

        public new bool ResizeRedraw
        {
            get => base.ResizeRedraw;
            set => base.ResizeRedraw = value;
        }

        public new void AttachInterfaces(object nativeActiveXObject) => base.AttachInterfaces(nativeActiveXObject);

        public new void CreateSink() => base.CreateSink();

        public new WebBrowserSiteBase CreateWebBrowserSiteBase() => base.CreateWebBrowserSiteBase();

        public new void DetachInterfaces() => base.DetachInterfaces();

        public new void DetachSink() => base.DetachSink();

        public new void Dispose(bool disposing) => base.Dispose(disposing);

        public new AutoSizeMode GetAutoSizeMode() => base.GetAutoSizeMode();

        public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

        public new void OnCanGoBackChanged(EventArgs e) => base.OnCanGoBackChanged(e);

        public new void OnCanGoForwardChanged(EventArgs e) => base.OnCanGoForwardChanged(e);

        public new void OnDocumentCompleted(WebBrowserDocumentCompletedEventArgs e) => base.OnDocumentCompleted(e);

        public new void OnDocumentTitleChanged(EventArgs e) => base.OnDocumentTitleChanged(e);

        public new void OnEncryptionLevelChanged(EventArgs e) => base.OnEncryptionLevelChanged(e);

        public new void OnFileDownload(EventArgs e) => base.OnFileDownload(e);

        public new void OnNavigated(WebBrowserNavigatedEventArgs e) => base.OnNavigated(e);

        public new void OnNavigating(WebBrowserNavigatingEventArgs e) => base.OnNavigating(e);

        public new void OnNewWindow(CancelEventArgs e) => base.OnNewWindow(e);

        public new void OnProgressChanged(WebBrowserProgressChangedEventArgs e) => base.OnProgressChanged(e);

        public new void OnStatusTextChanged(EventArgs e) => base.OnStatusTextChanged(e);

        public new void WndProc(ref Message m) => base.WndProc(ref m);
    }

    [WinFormsFact]
    public void WebBrowser_NavigateToFileFolder()
    {
        using Form form = new();
        using WebBrowser browser = new()
        {
            Dock = DockStyle.Fill
        };

        string navigated = null;
        browser.Navigated += (sender, e) =>
        {
            navigated = browser.Url.ToString();
            form.Close();
        };

        form.Controls.Add(browser);

        form.Load += (sender, e) =>
        {
            browser.Navigate(@"file://C:/");
        };

        form.Show();

        navigated.Should().Be(@"file:///C:/");
    }
}
