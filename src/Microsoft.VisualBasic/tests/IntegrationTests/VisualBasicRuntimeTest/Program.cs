// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Microsoft.VisualBasic.ApplicationServices;
using Microsoft.VisualBasic.CompilerServices;

namespace VisualBasicRuntimeTest
{
    internal class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException();
            }

            switch (args[0])
            {
                case "Interaction.InputBox":
                    Interaction_InputBox(useVbHost: false);
                    break;
                case "Interaction.InputBox_VbHost":
                    Interaction_InputBox(useVbHost: true);
                    break;
                case "Interaction.MsgBox":
                    Interaction_MsgBox(useVbHost: false);
                    break;
                case "Interaction.MsgBox_VbHost":
                    Interaction_MsgBox(useVbHost: true);
                    break;
                case "WindowsFormsApplicationBase.Run":
                    WindowsFormsApplicationBase_Run();
                    break;
                case "ProgressDialog.ShowProgressDialog":
                    ProgressDialog_ShowProgressDialog();
                    break;
                default:
                    throw new ArgumentException();
            }
        }

        private static void Interaction_InputBox(bool useVbHost)
        {
            var host = useVbHost ? new VbHost() : null;
            HostServices.VBHost = host;
            Interaction.InputBox(Prompt: "Prompt", Title: "Title");
        }

        private static void Interaction_MsgBox(bool useVbHost)
        {
            var host = useVbHost ? new VbHost() : null;
            HostServices.VBHost = host;
            Interaction.MsgBox(Prompt: "Message", Buttons: MsgBoxStyle.OkCancel, Title: "Title");
        }

        private static void WindowsFormsApplicationBase_Run()
        {
            var mainForm = new Form();
            var application = new WindowsApplication(mainForm);
            bool valid = false;

            mainForm.Load += (object sender, EventArgs e) =>
            {
                var forms = application.OpenForms;
                valid = forms.Count == 1 &&
                    forms[0] == mainForm &&
                    application.ApplicationContext.MainForm == mainForm;
                if (!valid)
                {
                    mainForm.Close();
                }
            };

            application.Run(new string[0]);
            if (!valid)
            {
                throw new InvalidOperationException();
            }
        }

        private static void ProgressDialog_ShowProgressDialog()
        {
            var dialogType = typeof(ApplicationBase).Assembly.GetType("Microsoft.VisualBasic.MyServices.Internal.ProgressDialog");
            var showMethod = dialogType.GetMethod("ShowProgressDialog");
            var dialog = (Form)Activator.CreateInstance(dialogType, nonPublic: true);
            showMethod.Invoke(dialog, null);
        }

        private sealed class WindowsApplication : WindowsFormsApplicationBase
        {
            internal WindowsApplication(Form mainForm)
            {
                MainForm = mainForm;
            }
        }

        private static string GetUniqueName() => Guid.NewGuid().ToString("D");

        private sealed class VbHost : IVbHost
        {
            private readonly string _title = GetUniqueName();

            public IWin32Window GetParentWindow() => null;

            public string GetWindowTitle() => _title;
        }
    }
}
