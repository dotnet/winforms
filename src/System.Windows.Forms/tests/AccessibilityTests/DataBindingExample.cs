// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Data;
using System.Windows.Forms;

namespace Accessibility_Core_App;

public partial class DataBindingExample : Form
{
    private readonly List<Student> _studentA = [];
    private readonly List<Student> _studentB = [];

    public DataBindingExample()
    {
        InitializeComponent();
    }

    protected override void OnLoad(EventArgs e)
    {
        BindingData();
    }

    private void BindingData()
    {
        for (int i = 0; i < 6; i++)
        {
            _studentA.Add(new Student(i, $"Name 1{i}", "Male"));
            _studentB.Add(new Student(i * 2, $"Name 11{i * 2}", "Female"));
        }

        // Binding Data For ListBox & ComboBox & CheckedListBox controls by using DataSource property
        listBox1.DataSource = _studentA;
        comboBox1.DataSource = _studentB;

        listBox1.DisplayMember = nameof(Student.Name);
        comboBox1.DisplayMember = nameof(Student.Name);

        // Binding Data For DataGridView control by using DadSource property
        dataGridView1.DataSource = new List<Student>
        {
            new(1, "StudentA", "Female", 12121, "1001", "Basketball", false, 10, 11),
            new(2, "StudentB", "Male", 12122, "1002", "Basketball", true, 10, 11),
            new(3, "StudentC", "Female", 12123, "1003", "Football", false, 10, 11),
            new(4, "StudentD", "Male", 12124, "1004", "Football", true, 10, 11),
        };

        // Binding Data For TextBox/Label/DomainUpDown/NumericUpDown/LinkLabel/CheckBox/RadioButton/RichTextBox/MaskedTextBox/Button
        // controls by using DadaBindings property
        Student stu = new(1, "Number", "Female", 12121, "HomeNumber", "Habits\nBasketball\nFootball", true, 10, 11);
        textBox1.DataBindings.Add("Text", stu, "StudentNumber");
        domainUpDown1.DataBindings.Add("Text", stu, "LuckyNumber");
        numericUpDown1.DataBindings.Add("Text", stu, "Count");
        label1.DataBindings.Add("Text", stu, "Name");
        button1.DataBindings.Add("Text", stu, "Sex");
        maskedTextBox1.DataBindings.Add("Text", stu, "PhoneNumber");
        linkLabel1.DataBindings.Add("Text", stu, "HomeNumber");
        richTextBox1.DataBindings.Add("Text", stu, "Hobby");
        checkBox1.DataBindings.Add("Checked", stu, "IsStudent");
        radioButton1.DataBindings.Add("Checked", stu, "IsStudent");

        // Binding Data For TreeView control by using DataSet
        BindTreeView();

        // Binding Data For ListView control by using DataSet
        BindListView();
    }

    private void BindTreeView()
    {
        using DataSet dataSet = CreateDataSet();

        if (dataSet.Tables[0].Rows.Count > 0)
        {
            for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
            {
                TreeNode node = new()
                {
                    Text = dataSet.Tables[0].Rows[i]["StuName"].ToString()
                };
                treeView1.Nodes.Add(node);
            }
        }
    }

    private void BindListView()
    {
        listView1.View = View.Details;
        using DataSet dataSet = CreateDataSet();

        int row_Count = dataSet.Tables[0].Rows.Count;
        int col_Count = dataSet.Tables[0].Columns.Count;

        for (int j = 0; j < col_Count; j++)
        {
            string colName = dataSet.Tables[0].Columns[j].ColumnName;
            listView1.Columns.Add(colName);
        }

        for (int i = 0; i < row_Count; i++)
        {
            string itemName = dataSet.Tables[0].Rows[i][0].ToString();
            ListViewItem item = new(itemName, i);
            listView1.Items.Add(item);

            for (int j = 1; j < col_Count; j++)
            {
                item.SubItems.Add(dataSet.Tables[0].Rows[i][j].ToString());
            }
        }
    }

    // Create DataSet
    public DataSet CreateDataSet()
    {
        DataSet stuDS = new();
        DataTable stuTable = new("Students");

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
