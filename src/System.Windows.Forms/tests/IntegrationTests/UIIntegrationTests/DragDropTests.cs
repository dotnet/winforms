// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Drawing;
using FluentAssertions;
using Windows.Win32.System.Com;
using Windows.Win32.UI.Accessibility;
using Windows.Win32.UI.WindowsAndMessaging;
using Xunit.Abstractions;
using static System.Windows.Forms.TestUtilities.DataObjectTestHelpers;

namespace System.Windows.Forms.UITests;

public class DragDropTests : ControlTestBase
{
    private const int DesktopNormalizedMax = 65536;
    public const int DragDropDelayMS = 100;
    public const string DragAcceptRtf = "DragAccept.rtf";
    public const string DragDrop = "DragDrop";
    public const string Explorer = "explorer";
    public const string Resources = "Resources";
    private readonly Bitmap _dragImage = new("./Resources/move.bmp");

    public DragDropTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task DragDrop_QueryDefaultCursors_Async()
    {
        await RunFormWithoutControlAsync(() => new DragDropForm(TestOutputHelper), async (form) =>
        {
            await MoveMouseToControlAsync(form.ListDragSource);

            // Select the item under the caret
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.LeftButtonClick());

            var targetMousePosition = ToVirtualPoint(form.ListDragTarget.PointToScreen(new Point(20, 20)));

            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse
                    .LeftButtonDown()
                    .MoveMouseTo(targetMousePosition.X - 40, targetMousePosition.Y)
                    .MoveMouseTo(targetMousePosition.X, targetMousePosition.Y)
                    .MoveMouseTo(targetMousePosition.X + 2, targetMousePosition.Y + 2)
                    .MoveMouseTo(targetMousePosition.X + 4, targetMousePosition.Y + 4)
                    .LeftButtonUp());

            Assert.Single(form.ListDragTarget.Items);
        });
    }

    [WinFormsFact]
    public async Task DragDrop_NonSerializedObject_ReturnsExpected_Async()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/7864, and it verifies that we can successfully drag and drop a
        // non-serialized object.

        Button? button = null;
        Button? data = null;
        await RunFormWithoutControlAsync(() => new Form(), async (form) =>
        {
            form.AllowDrop = true;
            form.ClientSize = new Size(100, 100);
            form.DragEnter += (s, e) =>
            {
                if (e.Data?.GetDataPresent(typeof(Button)) ?? false)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.DragOver += (s, e) =>
            {
                if (e.Data?.GetDataPresent(typeof(Button)) ?? false)
                {
                    // Get the non-serialized Button.
                    data = (Button?)e.Data?.GetData(typeof(Button));
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.MouseDown += (s, e) =>
            {
                DataObject data = new();

                // Button does not participate in serialization scenarios.
                // Store a non-serialized Button in the DataObject.
                button = new()
                {
                    Name = "button1",
                    Text = "Click"
                };
                data.SetData(typeof(Button), button);
                form.DoDragDrop(data, DragDropEffects.Copy);
            };

            var startRect = form.DisplayRectangle;
            var startCoordinates = form.PointToScreen(GetCenter(startRect));
            Point endCoordinates = new(startCoordinates.X + 5, startCoordinates.Y + 5);
            var virtualPointStart = ToVirtualPoint(startCoordinates);
            var virtualPointEnd = ToVirtualPoint(endCoordinates);

            await InputSimulator.SendAsync(
                form,
                inputSimulator
                    => inputSimulator.Mouse
                        .MoveMouseTo(virtualPointStart.X + 6, virtualPointStart.Y + 6)
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .LeftButtonUp());
        });

        Assert.NotNull(data);
        Assert.Equal(button?.Name, data.Name);
        Assert.Equal(button?.Text, data.Text);
    }

    [WinFormsFact(Skip = "Crashes dotnet.exe, see: https://github.com/dotnet/winforms/issues/8598")]
    public async Task DragDrop_RTF_FromExplorer_ToRichTextBox_ReturnsExpected_Async()
    {
        await RunTestAsync(dragDropForm =>
        {
            string dragAcceptRtfDestPath = string.Empty;
            string dragDropDirectory = Path.Combine(Directory.GetCurrentDirectory(), DragDrop);
            RunTest();

            unsafe void RunTest()
            {
                using ComScope<IUIAutomation> uiAutomation = new(null);
                using ComScope<IUIAutomationElement> uiAutomationElement = new(null);

                try
                {
                    string dragAcceptRtfSourcePath = Path.Combine(Directory.GetCurrentDirectory(), Resources, DragAcceptRtf);

                    if (!Directory.Exists(dragDropDirectory))
                    {
                        Directory.CreateDirectory(dragDropDirectory);
                    }

                    dragAcceptRtfDestPath = Path.Combine(dragDropDirectory, DragAcceptRtf);

                    if (!File.Exists(dragAcceptRtfDestPath))
                    {
                        File.Copy(dragAcceptRtfSourcePath, dragAcceptRtfDestPath);
                    }

                    string dragAcceptRtfContent = string.Empty;
                    string dragAcceptRtfTextContent = string.Empty;

                    using (RichTextBox richTextBox = new())
                    {
                        richTextBox.Rtf = File.ReadAllText(dragAcceptRtfDestPath);
                        dragAcceptRtfContent = richTextBox.Rtf;
                        dragAcceptRtfTextContent = richTextBox.Text;
                    }

                    TestOutputHelper.WriteLine($"dragAcceptRtfPath: {dragAcceptRtfDestPath}");

                    // Open the DragDrop directory and set focus on DragAccept.rtf
                    Process.Start("explorer.exe", $"/select,\"{dragAcceptRtfDestPath}\"");
                    WaitForExplorer(DragDrop, new Point(dragDropForm.Location.X + dragDropForm.Width, dragDropForm.Location.Y));
                    Assert.True(IsExplorerOpen(DragDrop));

                    // Create a CUIAutomation object and obtain the IUIAutomation interface
                    Assert.True(TryGetUIAutomation(uiAutomation));
                    Assert.False(uiAutomation.IsNull);

                    // Retrieve the UI element that has focus
                    Assert.Equal(HRESULT.S_OK, uiAutomation.Value->GetFocusedElement(uiAutomationElement));
                    Assert.False(uiAutomationElement.IsNull);

                    string elementName = GetElementName(uiAutomationElement);
                    TestOutputHelper.WriteLine($"Focused element name: {elementName}");

                    uiAutomationElement.Value->get_CurrentBoundingRectangle(out RECT rect);
                    TestOutputHelper.WriteLine($"Focused element bounding rect: {rect}");

                    // Retrieve a point that can be clicked
                    Assert.Equal(HRESULT.S_OK, uiAutomationElement.Value->GetClickablePoint(out Point clickable, out BOOL gotClickable));
                    Assert.True(gotClickable);
                    TestOutputHelper.WriteLine($"gotClickable: {gotClickable}");
                    TestOutputHelper.WriteLine($"clickable: {clickable}");

                    Point startCoordinates = clickable;
                    Rectangle endRect = dragDropForm.RichTextBoxDropTarget.ClientRectangle;
                    Point endCoordinates = dragDropForm.RichTextBoxDropTarget.PointToScreen(GetCenter(endRect));

                    Rectangle vscreen = SystemInformation.VirtualScreen;
                    Point virtualPointStart = new()
                    {
                        X = (startCoordinates.X - vscreen.Left) * DesktopNormalizedMax / vscreen.Width + DesktopNormalizedMax / (vscreen.Width * 2),
                        Y = (startCoordinates.Y - vscreen.Top) * DesktopNormalizedMax / vscreen.Height + DesktopNormalizedMax / (vscreen.Height * 2)
                    };
                    Point virtualPointEnd = new()
                    {
                        X = (endCoordinates.X - vscreen.Left) * DesktopNormalizedMax / vscreen.Width + DesktopNormalizedMax / (vscreen.Width * 2),
                        Y = (endCoordinates.Y - vscreen.Top) * DesktopNormalizedMax / vscreen.Height + DesktopNormalizedMax / (vscreen.Height * 2)
                    };

                    TestOutputHelper.WriteLine($"virtualPointStart: {virtualPointStart}");
                    TestOutputHelper.WriteLine($"virtualPointEnd: {virtualPointEnd}");

                    Assert.Equal(HRESULT.S_OK, uiAutomationElement.Value->SetFocus());

                    RunInputSimulator(virtualPointStart, virtualPointEnd);

                    Assert.NotNull(dragDropForm);
                    Assert.NotNull(dragDropForm.RichTextBoxDropTarget);

                    TestOutputHelper.WriteLine($"dragAcceptRtfContent: {dragAcceptRtfContent}");
                    TestOutputHelper.WriteLine($"RichTextBoxDropTarget.Rtf: {dragDropForm.RichTextBoxDropTarget.Rtf}");
                    Assert.False(string.IsNullOrWhiteSpace(dragDropForm.RichTextBoxDropTarget.Rtf),
                        $"Actual form.RichTextBoxDropTarget.Rtf: {dragDropForm.RichTextBoxDropTarget.Rtf}");

                    TestOutputHelper.WriteLine($"dragAcceptRtfTextContent: {dragAcceptRtfTextContent}");
                    TestOutputHelper.WriteLine($"RichTextBoxDropTarget.Text: {dragDropForm.RichTextBoxDropTarget.Text}");
                    Assert.False(string.IsNullOrWhiteSpace(dragDropForm.RichTextBoxDropTarget.Text),
                        $"Actual form.RichTextBoxDropTarget.Text: {dragDropForm.RichTextBoxDropTarget.Text}");

                    Assert.Equal(dragAcceptRtfContent, dragDropForm.RichTextBoxDropTarget?.Rtf);
                    Assert.Equal(dragAcceptRtfTextContent, dragDropForm.RichTextBoxDropTarget?.Text);
                }
                finally
                {
                    if (IsExplorerOpen(DragDrop))
                    {
                        CloseExplorer(DragDrop);
                    }

                    if (File.Exists(dragAcceptRtfDestPath))
                    {
                        File.Delete(dragAcceptRtfDestPath);
                    }

                    if (Directory.Exists(dragDropDirectory))
                    {
                        Directory.Delete(dragDropDirectory, true);
                    }
                }
            }

            return Task.CompletedTask;

#pragma warning disable VSTHRD100 // Avoid async void methods
            async void RunInputSimulator(Point virtualPointStart, Point virtualPointEnd)
            {
                await InputSimulator.SendAsync(
                    dragDropForm,
                    inputSimulator
                        => inputSimulator.Mouse
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointStart.X, virtualPointStart.Y)
                            .LeftButtonDown()
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X, virtualPointEnd.Y)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                            .MoveMouseToPositionOnVirtualDesktop(virtualPointEnd.X, virtualPointEnd.Y)
                            .LeftButtonUp());
            }
#pragma warning restore VSTHRD100 // Avoid async void methods
        });
    }

    [WinFormsFact]
    public async Task DragDrop_SerializedObject_ReturnsExpected_Async()
    {
        // Verifies that we can successfully drag and drop a serialized object.

        ListViewItem? listViewItem = null;
        ListViewItem? data = null;
        await RunFormWithoutControlAsync(() => new Form(), async (form) =>
        {
            form.AllowDrop = true;
            form.ClientSize = new Size(100, 100);
            form.DragEnter += (s, e) =>
            {
                if (e.Data?.GetDataPresent(DataFormats.Serializable) ?? false)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.DragOver += (s, e) =>
            {
                if (e.Data?.GetDataPresent(DataFormats.Serializable) ?? false)
                {
                    // Get the serialized ListViewItem.
                    data = (ListViewItem?)e.Data?.GetData(DataFormats.Serializable);
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.MouseDown += (s, e) =>
            {
                DataObject data = new();

                // ListViewItem participates in resx serialization scenarios.
                // Store a serialized ListViewItem in the DataObject.
                listViewItem = new("listViewItem1")
                {
                    Text = "View"
                };
                data.SetData(DataFormats.Serializable, listViewItem);
                form.DoDragDrop(data, DragDropEffects.Copy);
            };

            var startRect = form.DisplayRectangle;
            var startCoordinates = form.PointToScreen(GetCenter(startRect));
            Point endCoordinates = new(startCoordinates.X + 5, startCoordinates.Y + 5);
            var virtualPointStart = ToVirtualPoint(startCoordinates);
            var virtualPointEnd = ToVirtualPoint(endCoordinates);

            await InputSimulator.SendAsync(
                form,
                inputSimulator
                    => inputSimulator.Mouse
                        .MoveMouseTo(virtualPointStart.X + 6, virtualPointStart.Y + 6)
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .LeftButtonUp());
        });

        Assert.NotNull(data);
        Assert.Equal(listViewItem?.Name, data.Name);
        Assert.Equal(listViewItem?.Text, data.Text);
    }

    [WinFormsFact]
    public async Task DragEnter_Set_DropImageType_Message_MessageReplacementToken_ReturnsExpected_Async()
    {
        await RunFormWithoutControlAsync(() => new DragDropForm(TestOutputHelper), async (form) =>
        {
            DropImageType dropImageType = DropImageType.Move;
            string message = "Move to %1";
            string messageReplacementToken = "Drop Target";

            form.ListDragSource.GiveFeedback += (s, e) =>
            {
                e.DragImage = _dragImage;
                e.CursorOffset = new Point(0, 48);
                e.UseDefaultCursors = false;
            };

            form.ListDragTarget.DragEnter += (s, e) =>
            {
                e.DropImageType = dropImageType;
                e.Message = message;
                e.MessageReplacementToken = messageReplacementToken;
                e.Effect = DragDropEffects.Copy;
            };

            form.ListDragTarget.DragOver += (s, e) =>
            {
                Assert.Equal(dropImageType, e.DropImageType);
                Assert.Equal(message, e.Message);
                Assert.Equal(messageReplacementToken, e.MessageReplacementToken);
            };

            form.ListDragTarget.DragDrop += (s, e) =>
            {
                Assert.Equal(dropImageType, e.DropImageType);
                Assert.Equal(message, e.Message);
                Assert.Equal(messageReplacementToken, e.MessageReplacementToken);
            };

            var startRect = form.ListDragSource.DisplayRectangle;
            var startCoordinates = form.ListDragSource.PointToScreen(GetCenter(startRect));
            var endRect = form.ListDragTarget.DisplayRectangle;
            var endCoordinates = form.ListDragTarget.PointToScreen(GetCenter(endRect));
            var virtualPointStart = ToVirtualPoint(startCoordinates);
            var virtualPointEnd = ToVirtualPoint(endCoordinates);

            await InputSimulator.SendAsync(
                form,
                inputSimulator
                    => inputSimulator.Mouse
                        .MoveMouseTo(virtualPointStart.X + 6, virtualPointStart.Y + 6)
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .LeftButtonUp());

            await InputSimulator.SendAsync(
                form,
                inputSimulator
                    => inputSimulator.Mouse
                        .MoveMouseTo(virtualPointStart.X + 6, virtualPointStart.Y + 6)
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .LeftButtonUp());

            Assert.Equal(2, form.ListDragTarget.Items.Count);
        });
    }

    [WinFormsFact]
    public async Task PictureBox_SetData_DoDragDrop_RichTextBox_ReturnsExpected_Async()
    {
        await RunFormWithoutControlAsync(() => new DragImageDropDescriptionForm(TestOutputHelper), async (form) =>
        {
            string dragAcceptRtfPath = Path.Combine(Directory.GetCurrentDirectory(), Resources, DragAcceptRtf);
            using RichTextBox richTextBox = new();
            richTextBox.Rtf = File.ReadAllText(dragAcceptRtfPath);
            string dragAcceptRtfContent = richTextBox.Rtf;
            string dragAcceptRtfTextContent = richTextBox.Text;
            Point startCoordinates = form.PictureBoxDragSource.PointToScreen(new Point(20, 20));
            Point virtualPointStart = ToVirtualPoint(startCoordinates);
            startCoordinates.Offset(155, 0);
            Point virtualPointEnd = ToVirtualPoint(startCoordinates);
            await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualPointStart.X, virtualPointStart.Y)
                                                                .LeftButtonDown()
                                                                .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                                                                .LeftButtonUp());

            Assert.NotNull(form);
            Assert.NotNull(form.RichTextBoxDropTarget);
            Assert.False(string.IsNullOrWhiteSpace(form.RichTextBoxDropTarget.Rtf));
            Assert.False(string.IsNullOrWhiteSpace(form.RichTextBoxDropTarget.Text));
            Assert.Equal(dragAcceptRtfContent, form.RichTextBoxDropTarget?.Rtf);
            Assert.Equal(dragAcceptRtfTextContent, form.RichTextBoxDropTarget?.Text);
        });
    }

    [WinFormsFact]
    public async Task ToolStripItem_SetData_DoDragDrop_RichTextBox_ReturnsExpected_Async()
    {
        await RunFormWithoutControlAsync(() => new DragImageDropDescriptionForm(TestOutputHelper), async (form) =>
        {
            string dragAcceptRtfPath = Path.Combine(Directory.GetCurrentDirectory(), Resources, DragAcceptRtf);
            using RichTextBox richTextBox = new();
            richTextBox.Rtf = File.ReadAllText(dragAcceptRtfPath);
            string dragAcceptRtfContent = richTextBox.Rtf;
            string dragAcceptRtfTextContent = richTextBox.Text;

            await MoveMouseToControlAsync(form.ToolStrip);
            await InputSimulator.SendAsync(
                form,
                inputSimulator => inputSimulator.Mouse.LeftButtonClick());

            Point toolStripItemCoordinates = form.ToolStrip.PointToScreen(new Point(5, 5));
            toolStripItemCoordinates.Offset(0, 40);
            Point virtualToolStripItemCoordinates = ToVirtualPoint(toolStripItemCoordinates);

            await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualToolStripItemCoordinates.X, virtualToolStripItemCoordinates.Y));

            Point virtualPointStart = virtualToolStripItemCoordinates;
            toolStripItemCoordinates.Offset(50, 50);
            Point virtualPointEnd = ToVirtualPoint(toolStripItemCoordinates);
            await InputSimulator.SendAsync(
                        form,
                        inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualPointStart.X, virtualPointStart.Y)
                                                                .LeftButtonDown()
                                                                .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                                                                .LeftButtonUp());

            Assert.NotNull(form);
            Assert.NotNull(form.RichTextBoxDropTarget);
            Assert.False(string.IsNullOrWhiteSpace(form.RichTextBoxDropTarget.Rtf));
            Assert.False(string.IsNullOrWhiteSpace(form.RichTextBoxDropTarget.Text));
            Assert.Equal(dragAcceptRtfContent, form.RichTextBoxDropTarget?.Rtf);
            Assert.Equal(dragAcceptRtfTextContent, form.RichTextBoxDropTarget?.Text);
        });
    }

    [WinFormsFact]
    public async Task DragDrop_JsonSerialized_ReturnsExpected_Async()
    {
        // Verifies that we can successfully drag and drop a JSON serialized object.

        SimpleTestData testData = new() { X = 10, Y = 10 };
        object? dropped = null;
        await RunFormWithoutControlAsync(() => new Form(), async (form) =>
        {
            form.AllowDrop = true;
            form.ClientSize = new Size(100, 100);
            form.DragEnter += (s, e) =>
            {
                if (e.Data?.GetDataPresent(typeof(SimpleTestData)) ?? false)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.DragOver += (s, e) =>
            {
                if (e.Data?.TryGetData(out SimpleTestData data) ?? false)
                {
                    // Get the JSON serialized Point.
                    dropped = data;
                    e.Effect = DragDropEffects.Copy;
                }
            };
            form.MouseDown += (s, e) =>
            {
                form.DoDragDropAsJson(testData, DragDropEffects.Copy);
            };

            var startRect = form.DisplayRectangle;
            var startCoordinates = form.PointToScreen(GetCenter(startRect));
            Point endCoordinates = new(startCoordinates.X + 5, startCoordinates.Y + 5);
            var virtualPointStart = ToVirtualPoint(startCoordinates);
            var virtualPointEnd = ToVirtualPoint(endCoordinates);

            await InputSimulator.SendAsync(
                form,
                inputSimulator
                    => inputSimulator.Mouse
                        .MoveMouseTo(virtualPointStart.X + 6, virtualPointStart.Y + 6)
                        .LeftButtonDown()
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                        .MoveMouseTo(virtualPointEnd.X + 2, virtualPointEnd.Y + 2)
                        .MoveMouseTo(virtualPointEnd.X + 4, virtualPointEnd.Y + 4)
                        .LeftButtonUp());
        });

        dropped.Should().BeOfType(typeof(SimpleTestData));
        dropped.Should().BeEquivalentTo(testData);
    }

    private void CloseExplorer(string directory)
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName == Explorer && process.MainWindowTitle == directory)
            {
                process.CloseMainWindow();
                process.Close();
                return;
            }
        }
    }

    private unsafe string GetElementName(IUIAutomationElement* element)
    {
        using BSTR name = default;
        if (element->get_CurrentName(&name).Succeeded)
        {
            return name.ToString();
        }

        return string.Empty;
    }

    private bool IsExplorerOpen(string directory)
    {
        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName == Explorer && process.MainWindowTitle == directory)
            {
                return true;
            }
        }

        return false;
    }

    private async Task RunTestAsync(Func<DragImageDropDescriptionForm, Task> runTest)
    {
        await RunFormWithoutControlAsync(
            testDriverAsync: runTest,
            createForm: () =>
            {
                return new DragImageDropDescriptionForm(TestOutputHelper)
                {
                    Location = new Point(0, 0),
                    StartPosition = FormStartPosition.Manual
                };
            });
    }

    private unsafe bool TryGetUIAutomation(IUIAutomation** uiAutomation)
    {
        Guid CLSID_CUIAutomation = new("FF48DBA4-60EF-4201-AA87-54103EEF594E");
        HRESULT hr = PInvokeCore.CoCreateInstance(
            in CLSID_CUIAutomation,
            pUnkOuter: null,
            CLSCTX.CLSCTX_INPROC_SERVER,
            out *uiAutomation);

        return hr.Succeeded;
    }

    private void WaitForExplorer(string directory, Point startPosition)
    {
        int wait = 0, maxWait = 100;
        while (!IsExplorerOpen(directory) && wait++ < maxWait)
        {
            TestOutputHelper.WriteLine($"Waiting for Explorer to open, wait {wait}");
            Thread.Sleep(DragDropDelayMS);
        }

        foreach (Process process in Process.GetProcesses())
        {
            if (process.ProcessName == Explorer && process.MainWindowTitle == directory)
            {
                PInvoke.SetWindowPos(
                    (HWND)process.MainWindowHandle,
                    HWND.HWND_TOP,
                    startPosition.X,
                    startPosition.Y,
                    0,
                    0,
                    SET_WINDOW_POS_FLAGS.SWP_NOSIZE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER | SET_WINDOW_POS_FLAGS.SWP_NOACTIVATE);
            }
        }
    }

    private class DragDropForm : Form
    {
        public ListBox ListDragSource;
        public ListBox ListDragTarget;
        private readonly CheckBox _useCustomCursorsCheck;
        private readonly Label _dropLocationLabel;

        private int _indexOfItemUnderMouseToDrag;
        private int _indexOfItemUnderMouseToDrop;

        private Rectangle _dragBoxFromMouseDown;
        private Point _screenOffset;

        private Cursor? _myNoDropCursor;
        private Cursor? _myNormalCursor;

        private readonly ITestOutputHelper _testOutputHelper;

        public DragDropForm(ITestOutputHelper testOutputHelper)
        {
            ListDragSource = new ListBox();
            ListDragTarget = new ListBox();
            _useCustomCursorsCheck = new CheckBox();
            _dropLocationLabel = new Label();

            SuspendLayout();

            // ListDragSource
            ListDragSource.Items.AddRange("one", "two", "three", "four", "five", "six", "seven", "eight", "nine", "ten");
            ListDragSource.Location = new Point(10, 17);
            ListDragSource.Size = new Size(120, 225);
            ListDragSource.MouseDown += ListDragSource_MouseDown;
            ListDragSource.QueryContinueDrag += ListDragSource_QueryContinueDrag;
            ListDragSource.MouseUp += ListDragSource_MouseUp;
            ListDragSource.MouseMove += ListDragSource_MouseMove;
            ListDragSource.GiveFeedback += ListDragSource_GiveFeedback;

            // ListDragTarget
            ListDragTarget.AllowDrop = true;
            ListDragTarget.Location = new Point(154, 17);
            ListDragTarget.Size = new Size(120, 225);
            ListDragTarget.DragOver += ListDragTarget_DragOver;
            ListDragTarget.DragDrop += ListDragTarget_DragDrop;
            ListDragTarget.DragEnter += ListDragTarget_DragEnter;
            ListDragTarget.DragLeave += ListDragTarget_DragLeave;

            // UseCustomCursorsCheck
            _useCustomCursorsCheck.Location = new Point(10, 243);
            _useCustomCursorsCheck.Size = new Size(137, 24);
            _useCustomCursorsCheck.Text = "Use Custom Cursors";

            // DropLocationLabel
            _dropLocationLabel.Location = new Point(154, 245);
            _dropLocationLabel.Size = new Size(137, 24);
            _dropLocationLabel.Text = "None";

            // Form1
            ClientSize = new Size(292, 270);
            Controls.AddRange(
            [
                ListDragSource,
                ListDragTarget,
                _useCustomCursorsCheck,
                _dropLocationLabel
            ]);

            _testOutputHelper = testOutputHelper;
        }

        private void ListDragSource_MouseDown(object? sender, MouseEventArgs e)
        {
            // Get the index of the item the mouse is below.
            _indexOfItemUnderMouseToDrag = ListDragSource.IndexFromPoint(e.X, e.Y);
            _testOutputHelper.WriteLine($"Mouse down on drag source at position ({e.X},{e.Y}). Index of element under mouse: {_indexOfItemUnderMouseToDrag}");

            if (_indexOfItemUnderMouseToDrag != ListBox.NoMatches)
            {
                // Remember the point where the mouse down occurred. The DragSize indicates
                // the size that the mouse can move before a drag event should be started.
                Size dragSize = SystemInformation.DragSize;

                // Create a rectangle using the DragSize, with the mouse position being
                // at the center of the rectangle.
                _dragBoxFromMouseDown = new Rectangle(
                    new Point(e.X - (dragSize.Width / 2),
                              e.Y - (dragSize.Height / 2)),
                    dragSize);
            }
            else
            {
                // Reset the rectangle if the mouse is not over an item in the ListBox.
                _dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void ListDragSource_MouseUp(object? sender, MouseEventArgs e)
        {
            // Reset the drag rectangle when the mouse button is raised.
            _dragBoxFromMouseDown = Rectangle.Empty;
            _testOutputHelper.WriteLine($"Mouse up on drag source at position ({e.X},{e.Y}).");
        }

        private void ListDragSource_MouseMove(object? sender, MouseEventArgs e)
        {
            _testOutputHelper.WriteLine($"Mouse move on drag source to position ({e.X},{e.Y}) with buttons {e.Button}.");
            if ((e.Button & MouseButtons.Left) == MouseButtons.Left)
            {
                // If the mouse moves outside the rectangle, start the drag.
                if (_dragBoxFromMouseDown != Rectangle.Empty &&
                    !_dragBoxFromMouseDown.Contains(e.X, e.Y))
                {
                    // Create custom cursors for the drag-and-drop operation.
                    try
                    {
                        _myNormalCursor = new Cursor("./Resources/3dwarro.cur");
                        _myNoDropCursor = new Cursor("./Resources/3dwno.cur");
                    }
                    catch
                    {
                        // An error occurred while attempting to load the cursors, so use
                        // standard cursors.
                        _useCustomCursorsCheck.Checked = false;
                    }
                    finally
                    {
                        // The screenOffset is used to account for any desktop bands
                        // that may be at the top or left side of the screen when
                        // determining when to cancel the drag drop operation.
                        _screenOffset = SystemInformation.WorkingArea.Location;

                        // Proceed with the drag-and-drop, passing in the list item.
                        DragDropEffects dropEffect = ListDragSource.DoDragDrop(
                            ListDragSource.Items[_indexOfItemUnderMouseToDrag],
                            DragDropEffects.All | DragDropEffects.Link);

                        // If the drag operation was a move then remove the item.
                        if (dropEffect == DragDropEffects.Move)
                        {
                            ListDragSource.Items.RemoveAt(_indexOfItemUnderMouseToDrag);

                            // Selects the previous item in the list as long as the list has an item.
                            if (_indexOfItemUnderMouseToDrag > 0)
                                ListDragSource.SelectedIndex = _indexOfItemUnderMouseToDrag - 1;

                            else if (ListDragSource.Items.Count > 0)
                                // Selects the first item.
                                ListDragSource.SelectedIndex = 0;
                        }

                        // Dispose of the cursors since they are no longer needed.
                        _myNormalCursor?.Dispose();

                        _myNoDropCursor?.Dispose();
                    }
                }
            }
        }

        private void ListDragSource_GiveFeedback(object? sender, GiveFeedbackEventArgs e)
        {
            _testOutputHelper.WriteLine($"Give feedback on drag source.");

            // Use custom cursors if the check box is checked.
            if (_useCustomCursorsCheck.Checked)
            {
                // Sets the custom cursor based upon the effect.
                e.UseDefaultCursors = false;
                if ((e.Effect & DragDropEffects.Move) == DragDropEffects.Move)
                    Cursor.Current = _myNormalCursor;
                else
                    Cursor.Current = _myNoDropCursor;
            }
        }

        private void ListDragTarget_DragOver(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag over on the drag target.");

            // Determine whether string data exists in the drop data. If not, then
            // the drop effect reflects that the drop cannot occur.
            if (e.Data is null || !e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.None;
                _dropLocationLabel.Text = "None - no string data.";
                return;
            }

            // Set the effect based upon the KeyState.
            if ((e.KeyState & (8 + 32)) == (8 + 32) &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // KeyState 8 + 32 = CTRL + ALT

                // Link drag-and-drop effect.
                e.Effect = DragDropEffects.Link;
            }
            else if ((e.KeyState & 32) == 32 &&
                (e.AllowedEffect & DragDropEffects.Link) == DragDropEffects.Link)
            {
                // ALT KeyState for link.
                e.Effect = DragDropEffects.Link;
            }
            else if ((e.KeyState & 4) == 4 &&
                (e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // SHIFT KeyState for move.
                e.Effect = DragDropEffects.Move;
            }
            else if ((e.KeyState & 8) == 8 &&
                (e.AllowedEffect & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                // CTRL KeyState for copy.
                e.Effect = DragDropEffects.Copy;
            }
            else if ((e.AllowedEffect & DragDropEffects.Move) == DragDropEffects.Move)
            {
                // By default, the drop action should be move, if allowed.
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }

            // Get the index of the item the mouse is below.

            // The mouse locations are relative to the screen, so they must be
            // converted to client coordinates.

            _indexOfItemUnderMouseToDrop =
                ListDragTarget.IndexFromPoint(ListDragTarget.PointToClient(new Point(e.X, e.Y)));

            // Updates the label text.
            if (_indexOfItemUnderMouseToDrop != ListBox.NoMatches)
            {
                _dropLocationLabel.Text = $"Drops before item #{(_indexOfItemUnderMouseToDrop + 1)}";
            }
            else
            {
                _dropLocationLabel.Text = "Drops at the end.";
            }
        }

        private void ListDragTarget_DragDrop(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag drop on drag target.");

            // Ensure that the list item index is contained in the data.
            if (e.Data is not null && e.Data.GetDataPresent(typeof(string)))
            {
                object? item = e.Data.GetData(typeof(string));

                // Perform drag-and-drop, depending upon the effect.
                if (item is not null && (e.Effect == DragDropEffects.Copy || e.Effect == DragDropEffects.Move))
                {
                    // Insert the item.
                    if (_indexOfItemUnderMouseToDrop != ListBox.NoMatches)
                        ListDragTarget.Items.Insert(_indexOfItemUnderMouseToDrop, item);
                    else
                        ListDragTarget.Items.Add(item);
                }
            }

            // Reset the label text.
            _dropLocationLabel.Text = "None";
        }

        private void ListDragSource_QueryContinueDrag(object? sender, QueryContinueDragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Query for drag continuation.");

            // Cancel the drag if the mouse moves off the form.
            if (sender is ListBox lb)
            {
                Form form = lb.FindForm()!;

                // Cancel the drag if the mouse moves off the form. The screenOffset
                // takes into account any desktop bands that may be at the top or left
                // side of the screen.
                if (((MousePosition.X - _screenOffset.X) < form.DesktopBounds.Left) ||
                    ((MousePosition.X - _screenOffset.X) > form.DesktopBounds.Right) ||
                    ((MousePosition.Y - _screenOffset.Y) < form.DesktopBounds.Top) ||
                    ((MousePosition.Y - _screenOffset.Y) > form.DesktopBounds.Bottom))
                {
                    _testOutputHelper.WriteLine($"Canceling drag.");
                    e.Action = DragAction.Cancel;
                }
            }
        }

        private void ListDragTarget_DragEnter(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag enter on target.");

            // Reset the label text.
            _dropLocationLabel.Text = "None";

            // Also treat this as a DragOver to start the drag/drop operation
            ListDragTarget_DragOver(sender, e);
        }

        private void ListDragTarget_DragLeave(object? sender, EventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag leave on target.");

            // Reset the label text.
            _dropLocationLabel.Text = "None";
        }
    }

    private class DragImageDropDescriptionForm : Form
    {
        private readonly Bitmap _dragImage = new("./Resources/image.png");
        private readonly Bitmap _dragAcceptBmp = new("./Resources/DragAccept.bmp");
        private readonly ITestOutputHelper _testOutputHelper;

        private ContextMenuStrip? _contextMenuStrip;

        public PictureBox PictureBoxDragSource;
        public RichTextBox RichTextBoxDropTarget;
        public ToolStrip ToolStrip = new();

        public DragImageDropDescriptionForm(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            PictureBoxDragSource = new PictureBox();
            RichTextBoxDropTarget = new RichTextBox();

            SuspendLayout();

            // PictureBoxDragSource
            PictureBoxDragSource.AllowDrop = true;
            PictureBoxDragSource.BorderStyle = BorderStyle.FixedSingle;
            PictureBoxDragSource.Location = new Point(10, 45);
            PictureBoxDragSource.Size = new Size(125, 119);
            PictureBoxDragSource.DragEnter += PictureBoxDragSource_DragEnter;
            PictureBoxDragSource.DragOver += PictureBoxDragSource_DragOver;
            PictureBoxDragSource.MouseDown += PictureBoxDragSource_MouseDown;

            // RichTextBoxDropTarget
            RichTextBoxDropTarget.AllowDrop = true;
            RichTextBoxDropTarget.EnableAutoDragDrop = true;
            RichTextBoxDropTarget.Location = new Point(145, 45);
            RichTextBoxDropTarget.Size = new Size(125, 119);
            RichTextBoxDropTarget.DragEnter += RichTextBoxDropTarget_DragEnter;
            RichTextBoxDropTarget.DragDrop += RichTextBoxDropTarget_DragDrop;

            // ToolStrip
            CreateToolStrip();

            // Form1
            ClientSize = new Size(285, 175);
            Controls.AddRange(
            [
                PictureBoxDragSource,
                RichTextBoxDropTarget
            ]);
        }

        private void CreateToolStrip()
        {
            TableLayoutPanel tableLayoutPanel = new()
            {
                ColumnCount = 1,
                Dock = DockStyle.Top,
                Height = 35,
                RowCount = 1
            };

            tableLayoutPanel.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _contextMenuStrip = new ContextMenuStrip
            {
                AllowDrop = true,
                AutoSize = true,
                ImageScalingSize = new Size(25, 25)
            };

            _contextMenuStrip.Opening += ContextMenuStrip_Opening;

            ToolStrip = new()
            {
                DefaultDropDownDirection = ToolStripDropDownDirection.BelowLeft,
                Dock = DockStyle.Right,
                GripStyle = ToolStripGripStyle.Hidden
            };

            ToolStripDropDownButton toolStripDropDownButton = new()
            {
                AutoSize = false,
                AutoToolTip = false,
                DropDown = _contextMenuStrip,
                Height = 35,
                Name = "toolStripDropDownButton",
                Text = "Drag Images",
                ToolTipText = string.Empty,
                Width = 100
            };

            ToolStrip.Items.Add(toolStripDropDownButton);
            tableLayoutPanel.Controls.Add(ToolStrip, 0, 0);
            Controls.Add(tableLayoutPanel);
            ContextMenuStrip = _contextMenuStrip;
        }

        private void ContextMenuStrip_Opening(object? sender, CancelEventArgs e)
        {
            if (_contextMenuStrip is null)
            {
                return;
            }

            _contextMenuStrip.DefaultDropDownDirection = ToolStripDropDownDirection.BelowLeft;
            _contextMenuStrip.Items.Clear();

            ToolStripItem dragAcceptItem = new ToolStripMenuItem()
            {
                AllowDrop = true,
                Image = _dragAcceptBmp,
                ImageScaling = ToolStripItemImageScaling.SizeToFit,
                Text = "DragAccept",
                Name = "dragAcceptItem",
            };
            dragAcceptItem.DragEnter += DragAcceptItem_DragEnter;
            dragAcceptItem.MouseDown += DragAcceptItem_MouseDown;

            _contextMenuStrip.Items.Add(dragAcceptItem);
            e.Cancel = false;
        }

        private void DragAcceptItem_DragEnter(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag enter on target.");

            e.DropImageType = DropImageType.Link;
            e.Message = "DragAcceptFiles";
            e.Effect = DragDropEffects.Link;
        }

        private void DragAcceptItem_MouseDown(object? sender, MouseEventArgs e)
        {
            _testOutputHelper.WriteLine($"Mouse down on drag source at position ({e.X},{e.Y}).");

            if (sender is not ToolStripItem dragAcceptItem)
            {
                return;
            }

            string dragAcceptRtf = Path.Combine(Directory.GetCurrentDirectory(), Resources, DragAcceptRtf);
            if (File.Exists(dragAcceptRtf))
            {
                string[] dropFiles = [dragAcceptRtf];
                DataObject data = new(DataFormats.FileDrop, dropFiles);
                dragAcceptItem.DoDragDrop(data, DragDropEffects.All, _dragAcceptBmp, new Point(0, 16), false);
            }
        }

        private void PictureBoxDragSource_DragOver(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag over on source.");

            e.DropImageType = DropImageType.None;
            e.Effect = DragDropEffects.None;
        }

        private void RichTextBoxDropTarget_DragDrop(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag drop on target.");

            if (e.Data is not null
                && e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] fileNames
                && fileNames.Length > 0 && fileNames[0].Contains(DragAcceptRtf))
            {
                RichTextBoxDropTarget.Clear();
                RichTextBoxDropTarget.LoadFile(fileNames[0], RichTextBoxStreamType.RichText);
                e.Effect = DragDropEffects.None;
            }
        }

        private void RichTextBoxDropTarget_DragEnter(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag enter on target.");

            if (e.Data is not null
                && e.Data.GetDataPresent(DataFormats.FileDrop)
                && e.Data.GetData(DataFormats.FileDrop) is string[] fileNames
                && fileNames.Length > 0 && fileNames[0].Contains(DragAcceptRtf))
            {
                e.DropImageType = DropImageType.Link;
                e.Message = "%1 (shellapi.h)";
                e.MessageReplacementToken = "DragAcceptFiles";
                e.Effect = DragDropEffects.Link;
            }
        }

        private void PictureBoxDragSource_DragEnter(object? sender, DragEventArgs e)
        {
            _testOutputHelper.WriteLine($"Drag enter on source.");

            e.DropImageType = DropImageType.None;
            e.Effect = DragDropEffects.None;
        }

        private void PictureBoxDragSource_MouseDown(object? sender, MouseEventArgs e)
        {
            _testOutputHelper.WriteLine($"Mouse down on drag source at position ({e.X},{e.Y}).");

            string dragAcceptRtf = Path.Combine(Directory.GetCurrentDirectory(), Resources, DragAcceptRtf);
            if (File.Exists(dragAcceptRtf))
            {
                string[] dropFiles = [dragAcceptRtf];
                DataObject data = new(DataFormats.FileDrop, dropFiles);
                PictureBoxDragSource.DoDragDrop(data, DragDropEffects.All, _dragImage, new Point(0, 16), false);
            }
        }
    }
}
