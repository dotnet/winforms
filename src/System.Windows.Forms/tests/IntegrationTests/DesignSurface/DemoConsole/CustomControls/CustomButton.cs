using System.Windows.Forms;
using System.ComponentModel;

namespace TestConsole;

[Designer(typeof(CustomButtonDesigner), typeof(System.ComponentModel.Design.IDesigner))]
public class CustomButton : Button
{
}
