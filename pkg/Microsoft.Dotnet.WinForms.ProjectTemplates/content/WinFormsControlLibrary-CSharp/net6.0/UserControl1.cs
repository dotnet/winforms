#if (!csharpFeature_ImplicitUsings)
using System;
using System.Windows.Forms;

#endif
#if (csharpFeature_FileScopedNamespaces)
namespace Company.ControlLibrary1;

public partial class UserControl1: UserControl
{
    public UserControl1()
    {
        InitializeComponent();
    }
}
#else
namespace Company.ControlLibrary1
{
    public partial class UserControl1: UserControl
    {
        public UserControl1()
        {
            InitializeComponent();
        }
    }
}
#endif
