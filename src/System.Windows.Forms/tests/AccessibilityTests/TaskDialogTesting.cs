// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class TaskDialogTesting
{
    internal void ShowEventsDemoTaskDialog()
    {
        TaskDialogPage page1 = new()
        {
            Caption = nameof(TaskDialogTesting),
            Heading = "Event Demo",
            Text = "Event Demo...",
        };

        page1.Created += (s, e) => Console.WriteLine("Page1 Created");
        page1.Destroyed += (s, e) => Console.WriteLine("Page1 Destroyed");
        page1.HelpRequest += (s, e) => Console.WriteLine("Page1 HelpRequest");

        page1.Expander = new TaskDialogExpander("Expander")
        {
            Position = TaskDialogExpanderPosition.AfterFootnote
        };

        page1.Expander.ExpandedChanged += (s, e) => Console.WriteLine($"Expander ExpandedChanged: {page1.Expander.Expanded}");

        var buttonOK = TaskDialogButton.OK;
        var buttonHelp = TaskDialogButton.Help;
        TaskDialogCommandLinkButton buttonCancelClose = new("C&ancel Close", allowCloseDialog: false);
        TaskDialogCommandLinkButton buttonShowInnerDialog = new("&Show (modeless) Inner Dialog", "(and don't cancel the Close)");
        TaskDialogCommandLinkButton buttonNavigate = new("&Navigate", allowCloseDialog: false);

        page1.Buttons.Add(buttonOK);
        page1.Buttons.Add(buttonHelp);
        page1.Buttons.Add(buttonCancelClose);
        page1.Buttons.Add(buttonShowInnerDialog);
        page1.Buttons.Add(buttonNavigate);

        buttonOK.Click += (s, e) => Console.WriteLine($"Button '{s}' Click");
        buttonHelp.Click += (s, e) => Console.WriteLine($"Button '{s}' Click");

        buttonCancelClose.Click += (s, e) =>
        {
            Console.WriteLine($"Button '{s}' Click");
        };

        buttonShowInnerDialog.Click += (s, e) =>
        {
            Console.WriteLine($"Button '{s}' Click");
            TaskDialog.ShowDialog(new TaskDialogPage()
            {
                Text = "Inner Dialog"
            });
            Console.WriteLine($"(returns) Button '{s}' Click");
        };

        buttonNavigate.Click += (s, e) =>
        {
            Console.WriteLine($"Button '{s}' Click");

            // Navigate to a new page.
            TaskDialogPage page2 = new()
            {
                Heading = "AfterNavigation.",
                Buttons =
                {
                    TaskDialogButton.Close
                }
            };
            page2.Created += (s, e) => Console.WriteLine("Page2 Created");
            page2.Destroyed += (s, e) => Console.WriteLine("Page2 Destroyed");

            page1.Navigate(page2);
        };

        page1.Verification = new TaskDialogVerificationCheckBox("&CheckBox1");
        page1.Verification.CheckedChanged += (s, e) => Console.WriteLine($"CheckBox CheckedChanged: {page1.Verification.Checked}");

        var radioButton1 = page1.RadioButtons.Add("Radi&oButton1");
        var radioButton2 = page1.RadioButtons.Add("RadioB&utton2");

        radioButton1.CheckedChanged += (s, e) => Console.WriteLine($"RadioButton1 CheckedChanged: {radioButton1.Checked}");
        radioButton2.CheckedChanged += (s, e) => Console.WriteLine($"RadioButton2 CheckedChanged: {radioButton2.Checked}");

        var dialogResult = TaskDialog.ShowDialog(page1);
        Console.WriteLine($"---> Dialog Result: {dialogResult}");
    }
}
