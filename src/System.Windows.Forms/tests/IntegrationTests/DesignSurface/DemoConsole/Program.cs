// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms;

namespace TestConsole;

internal class Program
{
    // - NOTE: the code needs to run under a thread with an STA ApartmentState
    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        try
        {
            Console.WriteLine("Begin the demo...");

            // Form frm = new Form();
            // IDesignSurfaceExt surface = new DesignSurfaceExt.DesignSurfaceExt();
            // surface.CreateRootComponent( typeof( Form ), new Size( 400, 400 ) );
            // surface.CreateControl( typeof( Button ), new Size( 100, 40 ), new Point( 10, 10 ) );
            // TextBox t1 = (TextBox) surface.CreateControl( typeof( TextBox ), new Size( 300, 20 ), new Point( 10, 80 ) );
            // t1.Text = "Hello World by DesignSurfaceExt";
            // surface.GetView().Parent = frm;
            // frm.ShowDialog();

            MainForm f = new();
            f.ShowDialog();

            Console.WriteLine("Bye!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception (strike a key to quit!): {ex.Message}");
            Console.ReadLine();
        }
    }
}
