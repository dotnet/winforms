// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Reflection;
using System.Resources;
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
            try
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
                    case "WindowsFormsApplicationBase.RunSingleInstance":
                        WindowsFormsApplicationBase_RunSingleInstance();
                        break;
                    case "ProgressDialog.ShowProgressDialog":
                        ProgressDialog_ShowProgressDialog();
                        break;
                    case "VBInputBox.ShowDialog":
                        VBInputBox_ShowDialog();
                        break;
                    default:
                        throw new ArgumentException();
                }
            }
            catch (Exception)
            {
                Environment.Exit(2);
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
            var application = new WindowsApplication(mainForm, false);
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
        private static void WindowsFormsApplicationBase_RunSingleInstance()
        {
            var mainForm = new Form();
            var application = new WindowsApplication(mainForm, true);

            bool valid = false;

            application.StartupNextInstance += (object sender, StartupNextInstanceEventArgs e) =>
            {
                // this needs testing
            };

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
            var dialog = (Form)Activator.CreateInstance(dialogType, nonPublic: true);

            var resources = new ResourceManager(dialogType);
            var expectedValue = (Point)resources.GetObject("ProgressBarWork.Location");
            if (expectedValue == new Point(0, 0))
            {
                throw new InvalidOperationException();
            }

            var controlProperty = dialogType.GetProperty("ProgressBarWork", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var control = (Control)controlProperty.GetValue(dialog);
            var actualValue = control.Location;
            if (actualValue != expectedValue)
            {
                throw new InvalidOperationException();
            }

            var showMethod = dialogType.GetMethod("ShowProgressDialog");
            showMethod.Invoke(dialog, null);
        }

        private static void VBInputBox_ShowDialog()
        {
            var formType = typeof(ApplicationBase).Assembly.GetType("Microsoft.VisualBasic.CompilerServices.VBInputBox");
            var form = (Form)Activator.CreateInstance(formType, nonPublic: true);

            var resources = new ResourceManager(formType);
            var expectedValue = (Point)resources.GetObject("TextBox.Location");
            if (expectedValue == new Point(0, 0))
            {
                throw new InvalidOperationException();
            }

            var controlField = formType.GetField("TextBox", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var control = (Control)controlField.GetValue(form);
            var actualValue = control.Location;
            if (actualValue != expectedValue)
            {
                throw new InvalidOperationException();
            }

            form.ShowDialog();
        }

        private sealed class WindowsApplication : WindowsFormsApplicationBase
        {
            internal WindowsApplication(Form mainForm, bool isSingleInstance)
            {
                MainForm = mainForm;
                IsSingleInstance = isSingleInstance;
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
