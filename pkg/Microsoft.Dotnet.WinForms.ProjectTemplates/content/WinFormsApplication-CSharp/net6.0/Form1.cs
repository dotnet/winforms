#if (!csharpFeature_ImplicitUsings)
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#endif
#if (csharpFeature_FileScopedNamespaces)
namespace Company.WinFormsApplication1;

public partial class Form1 : Form
{
    public Form1()
    {
        InitializeComponent();
    }
}
#else
namespace Company.WinFormsApplication1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
    }
}
#endif
