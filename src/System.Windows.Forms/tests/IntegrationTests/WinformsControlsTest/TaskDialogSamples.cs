﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public class TaskDialogSamples : Form
    {
        public TaskDialogSamples()
        {
            this.Text = "Task Dialog Demos";

            int currentButtonCount = 0;
            void AddButtonForAction(string name, Action action)
            {
                int nextButton = ++currentButtonCount;

                var button = new Button()
                {
                    Text = name,
                    Size = new Size(180, 23),
                    Location = new Point(nextButton / 20 * 200 + 20, nextButton % 20 * 30)
                };

                button.Click += (s, e) => action();

                Controls.Add(button);
            }

            AddButtonForAction("Confirmation Dialog (3x)", ShowSimpleTaskDialog);
            AddButtonForAction("Close Document Confirmation", ShowCloseDocumentTaskDialog);
            AddButtonForAction("Minesweeper Difficulty", ShowMinesweeperDifficultySelectionTaskDialog);
            AddButtonForAction("Auto-Closing Dialog", ShowAutoClosingTaskDialog);
            AddButtonForAction("Multi-Page Dialog (modeless)", ShowMultiPageTaskDialog);
            AddButtonForAction("Elevation Required", ShowElevatedProcessTaskDialog);
            AddButtonForAction("Events Demo", ShowEventsDemoTaskDialog);
        }

        private void ShowSimpleTaskDialog()
        {
            // Show a message box.
            DialogResult messageBoxResult = MessageBox.Show(
                this,
                text: "Stopping the operation might leave your database in a corrupted state. Are you sure you want to stop?",
                caption: "Confirmation [Message Box]",
                buttons: MessageBoxButtons.YesNo,
                icon: MessageBoxIcon.Warning,
                defaultButton: MessageBoxDefaultButton.Button2);

            if (messageBoxResult == DialogResult.Yes)
            {
                Console.WriteLine("User confirmed to stop the operation.");
            }

            // Show a task dialog (simple).
            TaskDialogButton result = TaskDialog.ShowDialog(this, new TaskDialogPage()
            {
                Text = "Stopping the operation might leave your database in a corrupted state.",
                Heading = "Are you sure you want to stop?",
                Caption = "Confirmation (Task Dialog)",
                Buttons =
                {
                    TaskDialogButton.Yes,
                    TaskDialogButton.No
                },
                Icon = TaskDialogIcon.Warning,
                DefaultButton = TaskDialogButton.No
            });

            if (result == TaskDialogButton.Yes)
            {
                Console.WriteLine("User confirmed to stop the operation.");
            }

            // Show a task dialog (enhanced).
            var page = new TaskDialogPage()
            {
                Heading = "Are you sure you want to stop?",
                Text = "Stopping the operation might leave your database in a corrupted state.",
                Caption = "Confirmation (Task Dialog)",
                Icon = TaskDialogIcon.Warning,
                AllowCancel = true,

                Verification = new TaskDialogVerificationCheckBox()
                {
                    Text = "Do not show again"
                },

                Buttons =
                {
                    TaskDialogButton.Yes,
                    TaskDialogButton.No
                },

                DefaultButton = TaskDialogButton.No
            };

            var resultButton = TaskDialog.ShowDialog(this, page);

            if (resultButton == TaskDialogButton.Yes)
            {
                if (page.Verification.Checked)
                    Console.WriteLine("Do not show this confirmation again.");

                Console.WriteLine("User confirmed to stop the operation.");
            }
        }

        private void ShowCloseDocumentTaskDialog()
        {
            // Create the page which we want to show in the dialog.
            TaskDialogButton btnCancel = TaskDialogButton.Cancel;
            TaskDialogButton btnSave = new TaskDialogButton("&Save");
            TaskDialogButton btnDontSave = new TaskDialogButton("Do&n't save");

            var page = new TaskDialogPage()
            {
                Caption = "My Application",
                Heading = "Do you want to save changes to Untitled?",
                Buttons =
                {
                    btnCancel,
                    btnSave,
                    btnDontSave
                }
            };

            // Show a modal dialog, then check the result.
            TaskDialogButton result = TaskDialog.ShowDialog(this, page);

            if (result == btnSave)
                Console.WriteLine("Saving");
            else if (result == btnDontSave)
                Console.WriteLine("Not saving");
            else
                Console.WriteLine("Canceling");
        }

        private void ShowMinesweeperDifficultySelectionTaskDialog()
        {
            var page = new TaskDialogPage()
            {
                Caption = "Minesweeper",
                Heading = "What level of difficulty do you want to play?",
                AllowCancel = true,

                Footnote = new TaskDialogFootnote()
                {
                    Text = "Note: You can change the difficulty level later " +
                        "by clicking Options on the Game menu.",
                },

                Buttons =
                {
                    new TaskDialogCommandLinkButton("&Beginner", "10 mines, 9 x 9 tile grid")
                    {
                        Tag = 10
                    },
                    new TaskDialogCommandLinkButton("&Intermediate", "40 mines, 16 x 16 tile grid")
                    {
                        Tag = 40
                    },
                    new TaskDialogCommandLinkButton("&Advanced", "99 mines, 16 x 30 tile grid")
                    {
                        Tag = 99
                    }
                }
            };

            TaskDialogButton result = TaskDialog.ShowDialog(this, page);

            if (result.Tag is int resultingMines)
                Console.WriteLine($"Playing with {resultingMines} mines...");
            else
                Console.WriteLine("User canceled.");
        }

        private void ShowAutoClosingTaskDialog()
        {
            const string textFormat = "Reconnecting in {0} seconds...";
            int remainingTenthSeconds = 50;

            var reconnectButton = new TaskDialogButton("&Reconnect now");
            var cancelButton = TaskDialogButton.Cancel;

            var page = new TaskDialogPage()
            {
                Heading = "Connection lost; reconnecting...",
                Text = string.Format(textFormat, (remainingTenthSeconds + 9) / 10),
                // Display the form's icon in the task dialog.
                // Note however that the task dialog will not scale the icon.
                Icon = new TaskDialogIcon(this.Icon),
                ProgressBar = new TaskDialogProgressBar()
                {
                    State = TaskDialogProgressBarState.Paused
                },
                Buttons =
                {
                    reconnectButton,
                    cancelButton
                }
            };

            // Create a WinForms timer that raises the Tick event every tenth second.
            using (var timer = new Timer()
            {
                Enabled = true,
                Interval = 100
            })
            {
                timer.Tick += (s, e) =>
                {
                    remainingTenthSeconds--;
                    if (remainingTenthSeconds > 0)
                    {
                        // Update the remaining time and progress bar.
                        page.Text = string.Format(textFormat, (remainingTenthSeconds + 9) / 10);
                        page.ProgressBar.Value = 100 - remainingTenthSeconds * 2;
                    }
                    else
                    {
                        // Stop the timer and click the "Reconnect" button - this will
                        // close the dialog.
                        timer.Enabled = false;
                        reconnectButton.PerformClick();
                    }
                };

                TaskDialogButton result = TaskDialog.ShowDialog(this, page);
                if (result == reconnectButton)
                    Console.WriteLine("Reconnecting.");
                else
                    Console.WriteLine("Not reconnecting.");
            }
        }

        private void ShowMultiPageTaskDialog()
        {
            // Disable the "Yes" button and only enable it when the check box is checked.
            // Also, don't close the dialog when this button is clicked.
            var initialButtonYes = TaskDialogButton.Yes;
            initialButtonYes.Enabled = false;
            initialButtonYes.AllowCloseDialog = false;

            var initialPage = new TaskDialogPage()
            {
                Caption = "My Application",
                Heading = "Clean up database?",
                Text = "Do you really want to do a clean-up?\nThis action is irreversible!",
                Icon = TaskDialogIcon.ShieldWarningYellowBar,
                AllowCancel = true,
                // A modeless dialog can be minimizable.
                AllowMinimize = true,

                Verification = new TaskDialogVerificationCheckBox()
                {
                    Text = "I know what I'm doing"
                },

                Buttons =
                {
                    TaskDialogButton.No,
                    initialButtonYes
                },
                DefaultButton = TaskDialogButton.No
            };

            // For the "In Progress" page, don't allow the dialog to close, by adding
            // a disabled button (if no button was specified, the task dialog would
            // get an (enabled) 'OK' button).
            var inProgressCloseButton = TaskDialogButton.Close;
            inProgressCloseButton.Enabled = false;

            var inProgressPage = new TaskDialogPage()
            {
                Caption = "My Application",
                Heading = "Operation in progress...",
                Text = "Please wait while the operation is in progress.",
                Icon = TaskDialogIcon.Information,
                AllowMinimize = true,

                ProgressBar = new TaskDialogProgressBar()
                {
                    State = TaskDialogProgressBarState.Marquee
                },

                Expander = new TaskDialogExpander()
                {
                    Text = "Initializing...",
                    Position = TaskDialogExpanderPosition.AfterFootnote
                },

                Buttons =
                {
                    inProgressCloseButton
                }
            };

            // Add an invisible Cancel button where we will intercept the Click event
            // to prevent the dialog from closing (when the User clicks the "X" button
            // in the title bar or presses ESC or Alt+F4).
            var invisibleCancelButton = TaskDialogButton.Cancel;
            invisibleCancelButton.Visible = false;
            invisibleCancelButton.AllowCloseDialog = false;
            inProgressPage.Buttons.Add(invisibleCancelButton);

            var finishedPage = new TaskDialogPage()
            {
                Caption = "My Application",
                Heading = "Success!",
                Text = "The operation finished.",
                Icon = TaskDialogIcon.ShieldSuccessGreenBar,
                AllowMinimize = true,
                Buttons =
                {
                    TaskDialogButton.Close
                }
            };

            TaskDialogButton showResultsButton = new TaskDialogCommandLinkButton("Show &Results");
            finishedPage.Buttons.Add(showResultsButton);

            // Enable the "Yes" button only when the checkbox is checked.
            TaskDialogVerificationCheckBox checkBox = initialPage.Verification;
            checkBox.CheckedChanged += (sender, e) =>
            {
                initialButtonYes.Enabled = checkBox.Checked;
            };

            // When the user clicks "Yes", navigate to the second page.
            initialButtonYes.Click += (sender, e) =>
            {
                // Navigate to the "In Progress" page that displays the
                // current progress of the background work.
                initialPage.Navigate(inProgressPage);

                // NOTE: When you implement a "In Progress" page that represents
                // background work that is done e.g. by a separate thread/task,
                // which eventually calls Control.Invoke()/BeginInvoke() when
                // its work is finished in order to navigate or update the dialog,
                // then DO NOT start that work here already (directly after
                // setting the Page property). Instead, start the work in the
                // TaskDialogPage.Created event of the new page.
                //
                // See comments in the code sample in https://github.com/dotnet/winforms/issues/146
                // for more information.
            };

            // Simulate work by starting an async operation from which we are updating the
            // progress bar and the expander with the current status.
            inProgressPage.Created += async (s, e) =>
            {
                // Run the background operation and iterate over the streamed values to update
                // the progress. Because we call the async method from the GUI thread,
                // it will use this thread's synchronization context to run the continuations,
                // so we don't need to use Control.[Begin]Invoke() to schedule the callbacks.
                var progressBar = inProgressPage.ProgressBar;

                await foreach (int progressValue in StreamBackgroundOperationProgressAsync())
                {
                    // When we display the first progress, switch the marquee progress bar
                    // to a regular one.
                    if (progressBar.State == TaskDialogProgressBarState.Marquee)
                        progressBar.State = TaskDialogProgressBarState.Normal;

                    progressBar.Value = progressValue;
                    inProgressPage.Expander.Text = $"Progress: {progressValue} %";
                }

                // Work is finished, so navigate to the third page.
                inProgressPage.Navigate(finishedPage);
            };

            // Show the dialog (modeless).
            TaskDialogButton result = TaskDialog.ShowDialog(initialPage);
            if (result == showResultsButton)
            {
                Console.WriteLine("Showing Results!");
            }

            static async IAsyncEnumerable<int> StreamBackgroundOperationProgressAsync()
            {
                // Note: The code here will run in the GUI thread - use
                // "await Task.Run(...)" to schedule CPU-intensive operations in a
                // worker thread.

                // Wait a bit before reporting the first progress.
                await Task.Delay(2800);

                for (int i = 0; i <= 100; i += 4)
                {
                    // Report the progress.
                    yield return i;

                    // Wait a bit to simulate work.
                    await Task.Delay(200);
                }
            }
        }

        private void ShowElevatedProcessTaskDialog()
        {
            var page = new TaskDialogPage()
            {
                Heading = "Settings saved - Service Restart required",
                Text = "The service needs to be restarted to apply the changes.",
                Icon = TaskDialogIcon.ShieldSuccessGreenBar,
                Buttons =
                {
                    TaskDialogButton.Close
                }
            };

            var restartNowButton = new TaskDialogCommandLinkButton("&Restart now");
            page.Buttons.Add(restartNowButton);

            restartNowButton.ShowShieldIcon = true;
            restartNowButton.Click += (s, e) =>
            {
                restartNowButton.AllowCloseDialog = true;
                restartNowButton.Enabled = false;

                // Try to start an elevated cmd.exe.
                var psi = new ProcessStartInfo("cmd.exe", "/k echo Hi, this is an elevated command prompt.")
                {
                    UseShellExecute = true,
                    Verb = "runas"
                };

                try
                {
                    Process.Start(psi)?.Dispose();
                }
                catch (Win32Exception ex) when (ex.NativeErrorCode == 1223)
                {
                    // The user canceled the UAC prompt, so don't close the dialog and
                    // re-enable the restart button.
                    restartNowButton.AllowCloseDialog = false;
                    restartNowButton.Enabled = true;
                    return;
                }
            };

            TaskDialog.ShowDialog(this, page);
        }

        private void ShowEventsDemoTaskDialog()
        {
            var page1 = new TaskDialogPage()
            {
                Caption = Text,
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

            page1.Expander.ExpandedChanged += (s, e) => Console.WriteLine("Expander ExpandedChanged: " + page1.Expander.Expanded);

            var buttonOK = TaskDialogButton.OK;
            var buttonHelp = TaskDialogButton.Help;
            var buttonCancelClose = new TaskDialogCommandLinkButton("C&ancel Close", allowCloseDialog: false);
            var buttonShowInnerDialog = new TaskDialogCommandLinkButton("&Show (modeless) Inner Dialog", "(and don't cancel the Close)");
            var buttonNavigate = new TaskDialogCommandLinkButton("&Navigate", allowCloseDialog: false);

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
                var page2 = new TaskDialogPage()
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

            page1.Verification = new TaskDialogVerificationCheckBox("&CheckBox");
            page1.Verification.CheckedChanged += (s, e) => Console.WriteLine("CheckBox CheckedChanged: " + page1.Verification.Checked);

            var radioButton1 = page1.RadioButtons.Add("Radi&oButton 1");
            var radioButton2 = page1.RadioButtons.Add("RadioB&utton 2");

            radioButton1.CheckedChanged += (s, e) => Console.WriteLine("RadioButton1 CheckedChanged: " + radioButton1.Checked);
            radioButton2.CheckedChanged += (s, e) => Console.WriteLine("RadioButton2 CheckedChanged: " + radioButton2.Checked);

            var dialogResult = TaskDialog.ShowDialog(page1);
            Console.WriteLine("---> Dialog Result: " + dialogResult);
        }
    }
}
