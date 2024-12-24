
using System.Diagnostics;

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
            listView1.BeforeLabelEdit += listView1_BeforeLabelEdit;
        }

        private void listView1_BeforeLabelEdit(object? sender, LabelEditEventArgs e)
        {
            Debug.WriteLine("ListView Here!");
        }


        private void treeView1_BeforeLabelEdit(object? sender, NodeLabelEditEventArgs e)
        {
            Debug.WriteLine("TreeView Here!");
        }

    }
}
