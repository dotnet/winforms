#if (!csharpFeature_ImplicitUsings)
using System;
using System.Windows.Forms;

#endif
#if (csharpFeature_FileScopedNamespaces)
namespace Company.WinFormsApplication1

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
