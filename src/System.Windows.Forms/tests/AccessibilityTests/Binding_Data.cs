using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient; 

namespace AccessibilityTests
{
    public partial class Binding_Data : Form
    {
        List<Student> studentA = new List<Student>();
        List<Student> studentB = new List<Student>();
        public Binding_Data()
        {
            InitializeComponent();

        }

        private void Binding_Data_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < 6; i++)
            {
                studentA.Add(new Student(i, "Name 1" + i, "Male"));
                studentB.Add(new Student(i * 2, "Name 11" + i * 2, "Female"));
            }

            // Binding Data For ListBox & ComboBox & CheckedListBox controls by using DadSource property
            listBox1.DataSource = studentA;
            comboBox1.DataSource = studentB;

            listBox1.DisplayMember = "StudentName";
            comboBox1.DisplayMember = "StudentName";

            // Binding Data For DataGridView control by using DadSource property
            dataGridView1.DataSource = new List<Student>
             {
             new Student(1, "StudentA", "Female", 12121, "1001","Basketball",false, 10, 11),
             new Student(2, "StudentB", "Male", 12122, "1002","Basketball",true, 10, 11),
             new Student(3, "StudentC", "Female", 12123, "1003","Football",false, 10, 11),
             new Student(4, "StudentD", "Male", 12124,"1004","Football",true, 10, 11),
            };


            //Binding Data For TextBox/Label control/DomianUpDown/NumericUpDown/LinkLabel/CheckBox/RadioButton/RichTextBox/MaskedTextBox/Button by using DadaBindings property
            Student stu = new Student(1, "StudentNumber", "Female", 12121, "HomeNumber", "Habits" + "\n" + "Basketball" + '\n' + "Football", true, 10, 11);
            this.textBox1.DataBindings.Add("Text", stu, "StudentNo");
            this.domainUpDown1.DataBindings.Add("Text", stu, "Lucky_Number");
            this.numericUpDown1.DataBindings.Add("Text", stu, "Student_Count");
            this.label1.DataBindings.Add("Text", stu, "StudentName");
            this.button1.DataBindings.Add("Text", stu, "StudentSex");
            this.maskedTextBox1.DataBindings.Add("Text", stu, "StudentPhoneNum");
            this.linkLabel1.DataBindings.Add("Text", stu, "HomeNumber");
            this.richTextBox1.DataBindings.Add("Text", stu, "Student_habit");
            this.checkBox1.DataBindings.Add("Checked", stu, "Is_Student");
            this.radioButton1.DataBindings.Add("Checked", stu, "Is_Student");

            //Binding Data For TreeView control by using DataSet
            BindTree();

            //Binding Data For ListView control by using DataSet
            BindListView();
        }

        //Binding Data For TreeView control by using DataSet
        public void BindTree()
        {
            DataSet ds = CreateDataSet();
            if (ds.Tables[0].Rows.Count > 0)
            {
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    TreeNode node = new TreeNode();
                    node.Text = ds.Tables[0].Rows[i]["StuName"].ToString();
                    this.treeView1.Nodes.Add(node);
                }
            }
        }

        //Binding Data For ListView control by using DataSet
        public void BindListView()
        {
            listView1.View = View.Details;
            DataSet ds = CreateDataSet();

            int row_Count = ds.Tables[0].Rows.Count;
            int col_Count = ds.Tables[0].Columns.Count;

            for ( int j=0; j<col_Count; j++)
            {
                string colName = ds.Tables[0].Columns[j].ColumnName;
                listView1.Columns.Add(colName);
            }

            for (int i =0; i < row_Count; i++)
            {
                string itemName = ds.Tables[0].Rows[i][0].ToString();
                ListViewItem item = new ListViewItem(itemName,i);
                listView1.Items.Add(item);

                for (int j = 1; j < col_Count; j++)
                {
                    item.SubItems.Add(ds.Tables[0].Rows[i][j].ToString());
                }
            }

        }

       // Create DataSet 
        public DataSet CreateDataSet()
        {
            DataSet stuDS = new DataSet();
            DataTable stuTable = new DataTable("Students");
            DataColumn stuColumn = new DataColumn();

            stuTable.Columns.Add("StuName", typeof(string));
            stuTable.Columns.Add("StuSex", typeof(string));
            stuTable.Columns.Add("StuAge", typeof(int));

            DataRow stuRow = stuTable.NewRow();
            stuRow["StuName"] = "sofie";
            stuRow["StuSex"] = "male";
            stuRow["StuAge"] = 21;
            stuTable.Rows.Add(stuRow);

            stuRow = stuTable.NewRow();
            stuRow["StuName"] = "Jrain";
            stuRow["StuSex"] = "Female";
            stuRow["StuAge"] = 21;
            stuTable.Rows.Add(stuRow);

            stuRow = stuTable.NewRow();
            stuRow["StuName"] = "Lida";
            stuRow["StuSex"] = "male";
            stuRow["StuAge"] = 21;
            stuTable.Rows.Add(stuRow);
            stuDS.Tables.Add(stuTable);

            return stuDS;
        }
    }
}
