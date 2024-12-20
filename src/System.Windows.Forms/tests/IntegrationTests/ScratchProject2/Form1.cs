
namespace ScratchProject2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
        }

        private void Form1_Load(object? sender, EventArgs e)     
        {
            treeView1.BeforeLabelEdit += treeView1_BeforeLabelEdit;
        }

        private void treeView1_BeforeLabelEdit(object? sender, NodeLabelEditEventArgs e)
        {
            Console.WriteLine("Here!");
        } 

    }
}
