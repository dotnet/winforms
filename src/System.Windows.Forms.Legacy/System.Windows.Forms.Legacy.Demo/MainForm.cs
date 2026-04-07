using System.Windows.Forms;

namespace Demo;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
    }

    private void MenuStackButton_Click(object? sender, EventArgs e)
    {
        MenuStackForm form = new();
        form.Show(this);
    }

    private void DataGridButton_Click(object? sender, EventArgs e)
    {
        DataGridForm form = new();
        form.Show(this);
    }

    private void ToolBarButton_Click(object? sender, EventArgs e)
    {
        ToolBarForm form = new();
        form.Show(this);
    }

    private void StatusBarButton_Click(object? sender, EventArgs e)
    {
        StatusBarForm form = new();
        form.Show(this);
    }

    private void AnchorLayoutHighDpiRegressionButton_Click(object? sender, EventArgs e)
    {
        AnchorLayoutHighDpiRegressionDemoForm form = new();
        form.Show(this);
    }
}
