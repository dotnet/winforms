// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;

namespace WinformsControlsTest
{
    public class MessageBoxTest : Form
    {
        public MessageBoxTest()
        {
            Text = "MessageBox tests";

            int currentButtonCount = 0;
            void AddButtonForAction(string name, Action action)
            {
                int nextButton = ++currentButtonCount;

                var button = new Button()
                {
                    Text = name,
                    Size = new Size(180, 23),
                    Location = new Point((nextButton / 20 * 200) + 20, nextButton % 20 * 30)
                };

                button.Click += (s, e) => action();

                Controls.Add(button);
            }

            AddButtonForAction("With Ok.", ShowOkMessageBox);
            AddButtonForAction("With Ok & Cancel.", ShowOkCancelMessageBox);
            AddButtonForAction("With Abort, Retry, & Ignore.", ShowAbortRetryIgnoreMessageBox);
            AddButtonForAction("With Yes, No, & Cancel.", ShowYesNoCancelMessageBox);
            AddButtonForAction("With Yes & No.", ShowYesNoMessageBox);
            AddButtonForAction("With Retry & Cancel.", ShowRetryCancelMessageBox);
            AddButtonForAction("With Cancel, Try Again, & Continue", ShowCancelTryContinueMessageBox);
        }

        private void ShowOkMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            if (result is DialogResult.OK && result2 is DialogResult.OK && result3 is DialogResult.OK && result4 is DialogResult.OK
                && result5 is DialogResult.OK && result6 is DialogResult.OK && result7 is DialogResult.OK && result8 is DialogResult.OK
                && result9 is DialogResult.OK && result10 is DialogResult.OK)
            {
                Console.WriteLine("User pressed ok.");
            }
        }

        private void ShowOkCancelMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            DialogResult result11 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            DialogResult result12 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult result13 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            DialogResult result14 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            DialogResult result15 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.OKCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            if (result is DialogResult.OK || result2 is DialogResult.OK
                || result3 is DialogResult.OK || result4 is DialogResult.OK
                || result5 is DialogResult.OK || result6 is DialogResult.OK
                || result7 is DialogResult.OK || result8 is DialogResult.OK
                || result9 is DialogResult.OK || result10 is DialogResult.OK
                || result11 is DialogResult.OK || result12 is DialogResult.OK
                || result13 is DialogResult.OK || result14 is DialogResult.OK
                || result15 is DialogResult.OK)
            {
                Console.WriteLine("User pressed ok.");
            }

            if (result is DialogResult.Cancel || result2 is DialogResult.Cancel
                || result3 is DialogResult.Cancel || result4 is DialogResult.Cancel
                || result5 is DialogResult.Cancel || result6 is DialogResult.Cancel
                || result7 is DialogResult.Cancel || result8 is DialogResult.Cancel
                || result9 is DialogResult.Cancel || result10 is DialogResult.Cancel
                || result11 is DialogResult.Cancel || result12 is DialogResult.Cancel
                || result13 is DialogResult.Cancel || result14 is DialogResult.Cancel
                || result15 is DialogResult.Cancel)
            {
                Console.WriteLine("User pressed cancel.");
            }
        }

        private void ShowAbortRetryIgnoreMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            DialogResult result11 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            DialogResult result12 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult result13 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            DialogResult result14 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            DialogResult result15 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            DialogResult result16 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.None, MessageBoxDefaultButton.Button3);
            DialogResult result17 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            DialogResult result18 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Information, MessageBoxDefaultButton.Button3);
            DialogResult result19 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            DialogResult result20 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error, MessageBoxDefaultButton.Button3);
            if (result is DialogResult.Abort || result2 is DialogResult.Abort
                || result3 is DialogResult.Abort || result4 is DialogResult.Abort
                || result5 is DialogResult.Abort || result6 is DialogResult.Abort
                || result7 is DialogResult.Abort || result8 is DialogResult.Abort
                || result9 is DialogResult.Abort || result10 is DialogResult.Abort
                || result11 is DialogResult.Abort || result12 is DialogResult.Abort
                || result13 is DialogResult.Abort || result14 is DialogResult.Abort
                || result15 is DialogResult.Abort || result16 is DialogResult.Abort
                || result17 is DialogResult.Abort || result18 is DialogResult.Abort
                || result19 is DialogResult.Abort || result20 is DialogResult.Abort)
            {
                Console.WriteLine("User pressed abort.");
            }

            if (result is DialogResult.Retry || result2 is DialogResult.Retry
                || result3 is DialogResult.Retry || result4 is DialogResult.Retry
                || result5 is DialogResult.Retry || result6 is DialogResult.Retry
                || result7 is DialogResult.Retry || result8 is DialogResult.Retry
                || result9 is DialogResult.Retry || result10 is DialogResult.Retry
                || result11 is DialogResult.Retry || result12 is DialogResult.Retry
                || result13 is DialogResult.Retry || result14 is DialogResult.Retry
                || result15 is DialogResult.Retry || result16 is DialogResult.Retry
                || result17 is DialogResult.Retry || result18 is DialogResult.Retry
                || result19 is DialogResult.Retry || result20 is DialogResult.Retry)
            {
                Console.WriteLine("User pressed retry.");
            }

            if (result is DialogResult.Ignore || result2 is DialogResult.Ignore
                || result3 is DialogResult.Ignore || result4 is DialogResult.Ignore
                || result5 is DialogResult.Ignore || result6 is DialogResult.Ignore
                || result7 is DialogResult.Ignore || result8 is DialogResult.Ignore
                || result9 is DialogResult.Ignore || result10 is DialogResult.Ignore
                || result11 is DialogResult.Ignore || result12 is DialogResult.Ignore
                || result13 is DialogResult.Ignore || result14 is DialogResult.Ignore
                || result15 is DialogResult.Ignore || result16 is DialogResult.Ignore
                || result17 is DialogResult.Ignore || result18 is DialogResult.Ignore
                || result19 is DialogResult.Ignore || result20 is DialogResult.Ignore)
            {
                Console.WriteLine("User pressed ignore.");
            }
        }

        private void ShowYesNoCancelMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            DialogResult result11 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            DialogResult result12 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult result13 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            DialogResult result14 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            DialogResult result15 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            DialogResult result16 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button3);
            DialogResult result17 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            DialogResult result18 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button3);
            DialogResult result19 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            DialogResult result20 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button3);
            if (result is DialogResult.Yes || result2 is DialogResult.Yes
                || result3 is DialogResult.Yes || result4 is DialogResult.Yes
                || result5 is DialogResult.Yes || result6 is DialogResult.Yes
                || result7 is DialogResult.Yes || result8 is DialogResult.Yes
                || result9 is DialogResult.Yes || result10 is DialogResult.Yes
                || result11 is DialogResult.Yes || result12 is DialogResult.Yes
                || result13 is DialogResult.Yes || result14 is DialogResult.Yes
                || result15 is DialogResult.Yes || result16 is DialogResult.Yes
                || result17 is DialogResult.Yes || result18 is DialogResult.Yes
                || result19 is DialogResult.Yes || result20 is DialogResult.Yes)
            {
                Console.WriteLine("User pressed yes.");
            }

            if (result is DialogResult.No || result2 is DialogResult.No
                || result3 is DialogResult.No || result4 is DialogResult.No
                || result5 is DialogResult.No || result6 is DialogResult.No
                || result7 is DialogResult.No || result8 is DialogResult.No
                || result9 is DialogResult.No || result10 is DialogResult.No
                || result11 is DialogResult.No || result12 is DialogResult.No
                || result13 is DialogResult.No || result14 is DialogResult.No
                || result15 is DialogResult.No || result16 is DialogResult.No
                || result17 is DialogResult.No || result18 is DialogResult.No
                || result19 is DialogResult.No || result20 is DialogResult.No)
            {
                Console.WriteLine("User pressed no.");
            }

            if (result is DialogResult.Cancel || result2 is DialogResult.Cancel
                || result3 is DialogResult.Cancel || result4 is DialogResult.Cancel
                || result5 is DialogResult.Cancel || result6 is DialogResult.Cancel
                || result7 is DialogResult.Cancel || result8 is DialogResult.Cancel
                || result9 is DialogResult.Cancel || result10 is DialogResult.Cancel
                || result11 is DialogResult.Cancel || result12 is DialogResult.Cancel
                || result13 is DialogResult.Cancel || result14 is DialogResult.Cancel
                || result15 is DialogResult.Cancel || result16 is DialogResult.Cancel
                || result17 is DialogResult.Cancel || result18 is DialogResult.Cancel
                || result19 is DialogResult.Cancel || result20 is DialogResult.Cancel)
            {
                Console.WriteLine("User pressed cancel.");
            }
        }

        private void ShowYesNoMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            DialogResult result11 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            DialogResult result12 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult result13 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            DialogResult result14 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            DialogResult result15 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.YesNo, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            if (result is DialogResult.Yes || result2 is DialogResult.Yes
                || result3 is DialogResult.Yes || result4 is DialogResult.Yes
                || result5 is DialogResult.Yes || result6 is DialogResult.Yes
                || result7 is DialogResult.Yes || result8 is DialogResult.Yes
                || result9 is DialogResult.Yes || result10 is DialogResult.Yes
                || result11 is DialogResult.Yes || result12 is DialogResult.Yes
                || result13 is DialogResult.Yes || result14 is DialogResult.Yes
                || result15 is DialogResult.Yes)
            {
                Console.WriteLine("User pressed yes.");
            }

            if (result is DialogResult.No || result2 is DialogResult.No
                || result3 is DialogResult.No || result4 is DialogResult.No
                || result5 is DialogResult.No || result6 is DialogResult.No
                || result7 is DialogResult.No || result8 is DialogResult.No
                || result9 is DialogResult.No || result10 is DialogResult.No
                || result11 is DialogResult.No || result12 is DialogResult.No
                || result13 is DialogResult.No || result14 is DialogResult.No
                || result15 is DialogResult.No)
            {
                Console.WriteLine("User pressed no.");
            }
        }

        private void ShowRetryCancelMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            DialogResult result11 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            DialogResult result12 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult result13 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            DialogResult result14 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            DialogResult result15 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            if (result is DialogResult.Retry || result2 is DialogResult.Retry
                || result3 is DialogResult.Retry || result4 is DialogResult.Retry
                || result5 is DialogResult.Retry || result6 is DialogResult.Retry
                || result7 is DialogResult.Retry || result8 is DialogResult.Retry
                || result9 is DialogResult.Retry || result10 is DialogResult.Retry
                || result11 is DialogResult.Retry || result12 is DialogResult.Retry
                || result13 is DialogResult.Retry || result14 is DialogResult.Retry
                || result15 is DialogResult.Retry)
            {
                Console.WriteLine("User pressed retry.");
            }

            if (result is DialogResult.Cancel || result2 is DialogResult.Cancel
                || result3 is DialogResult.Cancel || result4 is DialogResult.Cancel
                || result5 is DialogResult.Cancel || result6 is DialogResult.Cancel
                || result7 is DialogResult.Cancel || result8 is DialogResult.Cancel
                || result9 is DialogResult.Cancel || result10 is DialogResult.Cancel
                || result11 is DialogResult.Cancel || result12 is DialogResult.Cancel
                || result13 is DialogResult.Cancel || result14 is DialogResult.Cancel
                || result15 is DialogResult.Cancel)
            {
                Console.WriteLine("User pressed cancel.");
            }
        }

        private void ShowCancelTryContinueMessageBox()
        {
            // without default button these should be testing each and every icon as well too already with just 4 boxes.
            DialogResult result = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.None);
            DialogResult result2 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Question);
            DialogResult result3 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Information);
            DialogResult result4 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning);
            DialogResult result5 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Error);

            // with default button.
            DialogResult result6 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.None, MessageBoxDefaultButton.Button1);
            DialogResult result7 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            DialogResult result8 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
            DialogResult result9 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            DialogResult result10 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
            DialogResult result11 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.None, MessageBoxDefaultButton.Button2);
            DialogResult result12 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            DialogResult result13 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Information, MessageBoxDefaultButton.Button2);
            DialogResult result14 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            DialogResult result15 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Error, MessageBoxDefaultButton.Button2);
            DialogResult result16 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.None, MessageBoxDefaultButton.Button3);
            DialogResult result17 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Question, MessageBoxDefaultButton.Button3);
            DialogResult result18 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Information, MessageBoxDefaultButton.Button3);
            DialogResult result19 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button3);
            DialogResult result20 = MessageBox.Show(this, "Test", "Testing...", MessageBoxButtons.CancelTryContinue, MessageBoxIcon.Error, MessageBoxDefaultButton.Button3);
            if (result is DialogResult.Cancel || result2 is DialogResult.Cancel
                || result3 is DialogResult.Cancel || result4 is DialogResult.Cancel
                || result5 is DialogResult.Cancel || result6 is DialogResult.Cancel
                || result7 is DialogResult.Cancel || result8 is DialogResult.Cancel
                || result9 is DialogResult.Cancel || result10 is DialogResult.Cancel
                || result11 is DialogResult.Cancel || result12 is DialogResult.Cancel
                || result13 is DialogResult.Cancel || result14 is DialogResult.Cancel
                || result15 is DialogResult.Cancel || result16 is DialogResult.Cancel
                || result17 is DialogResult.Cancel || result18 is DialogResult.Cancel
                || result19 is DialogResult.Cancel || result20 is DialogResult.Cancel)
            {
                Console.WriteLine("User pressed cancel.");
            }

            if (result is DialogResult.TryAgain || result2 is DialogResult.TryAgain
                || result3 is DialogResult.TryAgain || result4 is DialogResult.TryAgain
                || result5 is DialogResult.TryAgain || result6 is DialogResult.TryAgain
                || result7 is DialogResult.TryAgain || result8 is DialogResult.TryAgain
                || result9 is DialogResult.TryAgain || result10 is DialogResult.TryAgain
                || result11 is DialogResult.TryAgain || result12 is DialogResult.TryAgain
                || result13 is DialogResult.TryAgain || result14 is DialogResult.TryAgain
                || result15 is DialogResult.TryAgain || result16 is DialogResult.TryAgain
                || result17 is DialogResult.TryAgain || result18 is DialogResult.TryAgain
                || result19 is DialogResult.TryAgain || result20 is DialogResult.TryAgain)
            {
                Console.WriteLine("User pressed try again.");
            }

            if (result is DialogResult.Continue || result2 is DialogResult.Continue
                || result3 is DialogResult.Continue || result4 is DialogResult.Continue
                || result5 is DialogResult.Continue || result6 is DialogResult.Continue
                || result7 is DialogResult.Continue || result8 is DialogResult.Continue
                || result9 is DialogResult.Continue || result10 is DialogResult.Continue
                || result11 is DialogResult.Continue || result12 is DialogResult.Continue
                || result13 is DialogResult.Continue || result14 is DialogResult.Continue
                || result15 is DialogResult.Continue || result16 is DialogResult.Continue
                || result17 is DialogResult.Continue || result18 is DialogResult.Continue
                || result19 is DialogResult.Continue || result20 is DialogResult.Continue)
            {
                Console.WriteLine("User pressed continue.");
            }
        }
    }
}
