using System;
using System.Windows.Forms;
using System.Reflection;
using System.ComponentModel;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections;
using System.IO;
using System.Collections.Generic;

//
// Testcase:    DataBindingAttributes01_Existence
// Description: Ensure that properties specified in the Data Binding Attributes exist.
// Author:      ariroth
//
public class DataBindingAttributes01_Existence : ReflectBase
{

    #region Testcase setup
    public DataBindingAttributes01_Existence(string[] args) : base(args) { }

    [STAThread]
    public static void Main(string[] args)
    {
        if (args.Length > 0 && args[0].ToUpper() == "GENERATE")
            generateBaseFile = true;
        Application.Run(new DataBindingAttributes01_Existence(args));
    }


    private ArrayList ReplaceLines(string source, ArrayList destination, ScenarioResult sr, TParams p)
    {
        // Necessary to insert the known values into the base file.  Only called when the base file is generated.
        string[] controlInfo = source.Split(':');
        int destPos = FindItem(destination, "Control: " + controlInfo[0]);
        if (destPos < 0)
        {
            sr.IncCounters(false, "The type you were looking for does not exist in the destination ArrayList.", p.log);
            return destination;
        }

        switch (controlInfo.Length - 1)
        {
            case 1:
                destination[destPos + 1] = "   DefaultBindingPropertyAttribute: " + controlInfo[1];
                break;
            case 2:
                destination[destPos + 1] = "   ComplexBindingPropertiesAttribute:";
                destination[destPos + 2] = "      DataMember: " + controlInfo[1];
                destination[destPos + 3] = "      DataSource: " + controlInfo[2];
                break;
            case 3:
                destination[destPos + 1] = "   DefaultBindingPropertyAttribute: " + controlInfo[1];
                destination[destPos + 2] = "   ComplexBindingPropertiesAttribute:";
                destination[destPos + 3] = "      DataMember: " + controlInfo[2];
                destination[destPos + 4] = "      DataSource: " + controlInfo[3];
                break;
            case 4:
                destination[destPos + 1] = "   LookupBindingPropertiesAttribute:";
                destination[destPos + 2] = "      DataSource   : " + controlInfo[1];
                destination[destPos + 3] = "      DisplayMember: " + controlInfo[2];
                destination[destPos + 4] = "      ValueMember  : " + controlInfo[3];
                destination[destPos + 5] = "      LookupMember : " + controlInfo[4];
                break;
            case 5:
                destination[destPos + 1] = "   DefaultBindingPropertyAttribute: " + controlInfo[1];
                destination[destPos + 2] = "   LookupBindingPropertiesAttribute:";
                destination[destPos + 3] = "      DataSource   : " + controlInfo[2];
                destination[destPos + 4] = "      DisplayMember: " + controlInfo[3];
                destination[destPos + 5] = "      ValueMember  : " + controlInfo[4];
                destination[destPos + 6] = "      LookupMember : " + controlInfo[5];
                break;
            case 6:
                destination[destPos + 1] = "   ComplexBindingPropertiesAttribute:";
                destination[destPos + 2] = "      DataMember: " + controlInfo[1];
                destination[destPos + 3] = "      DataSource: " + controlInfo[2];
                destination[destPos + 4] = "   LookupBindingPropertiesAttribute:";
                destination[destPos + 5] = "      DataSource   : " + controlInfo[3];
                destination[destPos + 6] = "      DisplayMember: " + controlInfo[4];
                destination[destPos + 7] = "      ValueMember  : " + controlInfo[5];
                destination[destPos + 8] = "      LookupMember : " + controlInfo[6];
                break;
            case 7:
                destination[destPos + 1] = "   DefaultBindingPropertyAttribute: " + controlInfo[1];
                destination[destPos + 2] = "   ComplexBindingPropertiesAttribute:";
                destination[destPos + 3] = "      DataMember: " + controlInfo[2];
                destination[destPos + 4] = "      DataSource: " + controlInfo[3];
                destination[destPos + 5] = "   LookupBindingPropertiesAttribute :";
                destination[destPos + 6] = "      DataSource   : " + controlInfo[4];
                destination[destPos + 7] = "      DisplayMember: " + controlInfo[5];
                destination[destPos + 8] = "      ValueMember  : " + controlInfo[6];
                destination[destPos + 9] = "      LookupMember : " + controlInfo[7];
                break;
        }

        return destination;
    }

    private int FindItem(ArrayList source, string item)
    {
        for (int i = 0; i < source.Count; i++)
        {
            if (source[i].Equals(item))
                return i;
        }
        return -1;
    }

    private void CompareFiles(ArrayList newFile, ScenarioResult sr, TParams p)
    {
        string line;
        int i = 0;
        StreamReader basefile = new StreamReader("base.txt");
        while ((line = basefile.ReadLine()) != null)
        {
            sr.IncCounters(line.Equals(newFile[i]), "Line " + i.ToString() + ": Attributes in base file didn't match current attributes.", line, newFile[i], p.log);
            i++;
        }
        basefile.Close();
    }
    #endregion

    //==========================================
    // Scenarios
    //==========================================
    #region Scenarios
    // Set this to true to generate a new base file.  Note that this does not run the comparison part of the test.
    static bool generateBaseFile = true;

    [Scenario("Reflect through all controls in Windows.Forms.dll.  Ensure that any control that has a Data Binding Attribute contains a property that matches the value of the Data Binding Attribute.")]
    public ScenarioResult ReflectForAttributeExistence(TParams p)
    {
        Assembly winFormsDll = typeof(Button).Assembly;

        Type[] allTypes = winFormsDll.GetTypes();
        Array.Sort(allTypes, new SortHelper());

        ScenarioResult ret = new ScenarioResult();
        ArrayList lines = new ArrayList();

        foreach (Type t in allTypes)
        {
            if (!t.Name.Equals("SysInfoForm") && !t.Name.Equals("ArrangedElement") && !t.Name.Equals("RaftingCell"))
            {
                if (typeof(Control).IsAssignableFrom(t) || typeof(Component).IsAssignableFrom(t))
                {
                    Attribute[] defaultBinding = (Attribute[])t.GetCustomAttributes(typeof(DefaultBindingPropertyAttribute), false);
                    Attribute[] complexBinding = (Attribute[])t.GetCustomAttributes(typeof(ComplexBindingPropertiesAttribute), false);
                    Attribute[] lookupBinding = (Attribute[])t.GetCustomAttributes(typeof(LookupBindingPropertiesAttribute), false);

                    // If there are no relevant attributes at all, ignore the control
                    if (defaultBinding.Length > 0 || complexBinding.Length > 0 || lookupBinding.Length > 0)
                    {
                        lines.Add("Control: " + t.Name);

                        // Handle the DefaultBindingPropertyAttribute
                        ret.IncCounters(defaultBinding.Length < 2, "There is more than one DefaultBindingPropertyAttribute on control " + t.FullName + ".", p.log);
                        if (defaultBinding.Length > 0)
                        {
                            ret.IncCounters(t.GetProperty(((DefaultBindingPropertyAttribute)defaultBinding[0]).Name) != null,
                                "Property " + ((DefaultBindingPropertyAttribute)defaultBinding[0]).Name + " does not exist on control " + t.FullName + ".",
                                p.log);
                            lines.Add("   DefaultBindingPropertyAttribute: " + ((DefaultBindingPropertyAttribute)defaultBinding[0]).Name);
                        }

                        // Handle the ComplexBindingPropertiesAttribute
                        ret.IncCounters(complexBinding.Length < 2, "There is more than one ComplexBindingPropertiesAttribute on control " + t.FullName + ".", p.log);
                        if (complexBinding.Length > 0)
                        {
                            ret.IncCounters(t.GetProperty(((ComplexBindingPropertiesAttribute)complexBinding[0]).DataMember) != null,
                                "Property " + ((ComplexBindingPropertiesAttribute)complexBinding[0]).DataMember + " does not exist on control " + t.FullName + ".",
                                p.log);
                            ret.IncCounters(t.GetProperty(((ComplexBindingPropertiesAttribute)complexBinding[0]).DataSource) != null,
                                "Property " + ((ComplexBindingPropertiesAttribute)complexBinding[0]).DataSource + " does not exist on control " + t.FullName + ".",
                                p.log);
                            lines.Add("   ComplexBindingPropertiesAttribute:");
                            lines.Add("      DataMember: " + ((ComplexBindingPropertiesAttribute)complexBinding[0]).DataMember);
                            lines.Add("      DataSource: " + ((ComplexBindingPropertiesAttribute)complexBinding[0]).DataSource);
                        }

                        // Handle the LookupBindingPropertiesAttribute
                        ret.IncCounters(lookupBinding.Length < 2, "There is more than one LookupBindingPropertiesAttribute on control " + t.FullName + ".", p.log);

                        if (lookupBinding.Length > 0 && ((LookupBindingPropertiesAttribute)lookupBinding[0]).DataSource != null)
                        {
                            ret.IncCounters(t.GetProperty(((LookupBindingPropertiesAttribute)lookupBinding[0]).DataSource) != null,
                                "Property " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).DataSource + " does not exist on control " + t.FullName + ".",
                                p.log);
                            ret.IncCounters(t.GetProperty(((LookupBindingPropertiesAttribute)lookupBinding[0]).DisplayMember) != null,
                                "Property " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).DisplayMember + " does not exist on control " + t.FullName + ".",
                                p.log);
                            ret.IncCounters(t.GetProperty(((LookupBindingPropertiesAttribute)lookupBinding[0]).ValueMember) != null,
                                "Property " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).ValueMember + " does not exist on control " + t.FullName + ".",
                                p.log);
                            ret.IncCounters(t.GetProperty(((LookupBindingPropertiesAttribute)lookupBinding[0]).LookupMember) != null,
                                "Property " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).LookupMember + " does not exist on control " + t.FullName + ".",
                                p.log);
                            lines.Add("   LookupBindingPropertiesAttribute:");
                            lines.Add("      DataSource   : " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).DataSource);
                            lines.Add("      DisplayMember: " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).DisplayMember);
                            lines.Add("      ValueMember  : " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).ValueMember);
                            lines.Add("      LookupMember : " + ((LookupBindingPropertiesAttribute)lookupBinding[0]).LookupMember);
                        }
                    }
                }
            }
        }

        if (generateBaseFile)
        {
            // The base file is generated while writing this test and attached as a requirement to the testcase.
            // It contains the values as they should be for the various controls.  When the control values change,
            // this file should be regenerated and reattached to use as the base.

            // This is the array containing the hard-coded strings necessary to generate the base file.  The format
            // for each string is as follows:
            //		[control_name]:[prop_name_1]:[prop_name_2]:...:[prop_name_n]
            // The three attributes each take a different number of arguments (Default: 1, Complex: 2, Lookup: 4) we can figure out
            // which attributes should be used based on the number of prop_name elements there are in the string.
            //
            // Should there be more than one attribute on a control, the prop_name elements should always come in the same
            // order.  This order is as follows (this is the full string with attribute names shortened for space; remove props as necessary):
            //		[control_name]:[Default.Name]:[Complex.DataMember]:[Complex.DataSource]:[Lookup.DataSource]:[Lookup.DisplayMember]:
            //			[Lookup.ValueMember]:[Lookup.LookupMember]
            string[] knownControls = new string[]{
                "DomainUpDown:SelectedItem",
                "NumericUpDown:Value",
                "CheckBox:CheckState",
                "RadioButton:Checked",
                "TextBoxBase:Text",
                "DateTimePicker:Value",
                "Label:Text",
                "MonthCalendar:SelectionStart",
                "PictureBox:Image",
                "ProgressBar:Value",
                "TrackBar:Value",
                "ListControl:Text:DataSource:DisplayMember:ValueMember:SelectedValue",
                "DataGridView:DataMember:DataSource"
            };

            // Replace the lines as necessary
            for (int i = 0; i < knownControls.Length; i++)
                ReplaceLines(knownControls[i], lines, ret, p);

            // Output to file
            StreamWriter sw = new StreamWriter(File.Open("base.txt", FileMode.Create));
            for (int i = 0; i < lines.Count; i++)
                sw.WriteLine(lines[i]);

            sw.Close();
        }
        else
            CompareFiles(lines, ret, p);

        return ret;

    }
    #endregion

    internal class SortHelper : IComparer
    {
        // Only necessary because Type doesn't implement IComparer and therefore Array.Sort() doesn't work on its own.
        public int Compare(object x, object y)
        {
            return ((Type)x).Name.ToString().CompareTo(((Type)y).Name.ToString());
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Reflect through all controls in Windows.Forms.dll.  Ensure that any control that has a Data Binding Attribute contains a property that matches the value of the Data Binding Attribute, and that the controls we say have attributes in the spec have the correct ones.