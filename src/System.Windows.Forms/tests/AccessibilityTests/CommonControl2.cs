using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Accessibility_Core_App
{
    public partial class CommonControl2 : Form
    {
        public CommonControl2()
        {
            InitializeComponent();
            
            string executable = Environment.ProcessPath;
            string executablePath = Path.GetDirectoryName(executable);
            var page = Path.Combine(executablePath, "HTMLPage1.html");
            this.webBrowser1.Url = new System.Uri($"file://{page}", System.UriKind.Absolute);
        }
    }
}
