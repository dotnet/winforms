using System.ComponentModel.Design;
using System.Windows.Forms.Design;

namespace TestConsole;

public class CustomButtonDesigner : ControlDesigner
{
    private DesignerActionListCollection _actionLists;

    public override DesignerActionListCollection ActionLists =>
        _actionLists ??= new DesignerActionListCollection
        {
            new CustomButtonDesignerActionList(Component)
        };
}
