// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.ComponentModel;
using System.Drawing;
using System.Data;

using System.Windows.Forms;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public static class DataGridViewUtils
    {
        // Different DataGridView set-ups
        private static DataGridView DataGridView = null;
        private static DataGridView s_typicalDataGridView;
       // private static readonly DataGridView s_maxCapacityDataGridView = null;
        private static DataGridView largeDataGridView = null;
        private static DataGridView oneCellDataGridView = null;
        private static DataGridView oneColumnDataGridView = null;
        private static DataGridView noRowDataGridView = null;
        private static DataGridView textBoxDataGridView = null;
        private static DataGridView imageDataGridView = null;
        private static DataGridView checkBoxDataGridView = null;
       // private static readonly DataGridView s_richTextBoxDataGridView = null;
        private static DataGridView mixedControlDataGridView = null;
        private static DataGridView userControlDataGridView = null;
        private static DataGridView nestedFormsDataGridView = null;
        private static DataGridView nestedDataGridViewsDataGridView = null;
        private static DataGridView visibleDataGridView = null;
        private static DataGridView invisibleDataGridView = null;
        private static DataGridView mixedInvisibleVisibleDataGridView = null;

        private static DataGridViewColumn dataGridViewColumn = null;

        private const int typical_rows = 100;
        private const int typical_cols = 10;
        private const int large_rows = 10000;
        private const int large_cols = 100;
        public static int standard_rows = 22;
        public static int standard_cols = 14;
        private const int min_row_index = 0;
        private static int max_row_index = 0;
        private const int min_col_index = 0;
        private static int max_col_index = 0;
        private static string[,] DataGridViewValues;
        private static int s_num_DataGridView_rows;
        private static int s_num_DataGridView_cols;

        private static int upperSize = 0x7FFFFF; //DGV defines upperSize
        public static Size MaxSize
        {
            get
            {
                return new Size(upperSize, upperSize);
            }
        }

        // Create an array containing some of the DataGridView.DataGridViewCellBorderStyle properties that are valid for
        // assigning to the CellBorderStyle.All property
        public static DataGridViewCellBorderStyle[] validDataGridViewCellBorderStylesAll = {DataGridViewCellBorderStyle.None, 
													 /*DataGridViewCellBorderStyle.InsetDouble,*/
													 DataGridViewCellBorderStyle.Raised,
													 /*DataGridViewCellBorderStyle.NotSet,*/
													 DataGridViewCellBorderStyle.RaisedHorizontal ,
													 /*DataGridViewCellBorderStyle.OutsetDouble,*/
													 /*DataGridViewCellBorderStyle.OutsetPartial,*/
													 DataGridViewCellBorderStyle.RaisedVertical
                                                    };
        // Create an array containing some of the DataGridView.DataGridViewCellBorderStyle properties that are valid for
        // assigning to the RowHeadersBorderStyle.All and ColumnHeadersBorderStyle.All properties
        public static DataGridViewCellBorderStyle[] validRowColCellBorderStylesAll = {DataGridViewCellBorderStyle.None, 
													 /*DataGridViewCellBorderStyle.InsetDouble*/
													 DataGridViewCellBorderStyle.Single,
													 /*DataGridViewCellBorderStyle.NotSet,*/
													 DataGridViewCellBorderStyle.SingleHorizontal,
                                                     DataGridViewCellBorderStyle.SingleVertical,
                                                     DataGridViewCellBorderStyle.Sunken,
                                                     DataGridViewCellBorderStyle.SunkenHorizontal
                                                    };

        // Create an array containing some of the DataGridView.DataGridViewCellBorderStyle properties that are valid
        // for assigning to the majority of the RowHeaders/ColumnHeaders/CellBorderStyles properties
        public static DataGridViewCellBorderStyle[] validDataGridViewCellBorderStyles = {DataGridViewCellBorderStyle.None,
                                                     DataGridViewCellBorderStyle.Single,
                                                     DataGridViewCellBorderStyle.Sunken,
													 /*DataGridViewCellBorderStyle.NotSet,*/
													 DataGridViewCellBorderStyle.Raised,
                                                     DataGridViewCellBorderStyle.RaisedHorizontal,
                                                     DataGridViewCellBorderStyle.SingleHorizontal,
                                                     DataGridViewCellBorderStyle.SingleVertical
                                                    };

        // Create an array containing all 3 of DataGridView BorderStyle properties
        public static BorderStyle[] borderStyle = {BorderStyle.None,
                                                BorderStyle.Fixed3D,
                                                BorderStyle.FixedSingle};

        /*
         * Creates a typical DataGridView of size 100 x 10 if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView of size 100 x 10
         */
        public static DataGridView GetTypicalDataGridView(bool newDataGridView)
        {
            return GetTypicalDataGridView(newDataGridView, false);
        }

        /*
         * Creates a typical DataGridView of size 100 x 10 if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         *         dataBound - true if data bound
         * returns: DataGridView of size 100 x 10
         */
        public static DataGridView GetTypicalDataGridView(bool newDataGridView, bool dataBound)
        {
            return GetDataGridView(s_typicalDataGridView, newDataGridView, dataBound, typical_cols, typical_rows);
        }

        /*
         * Creates a typical DataGridView of size 100 x 10 if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         *         dataBound - true if data bound
         *         cols - number of columns
         *         rows - number of rows
         * returns: DataGridView of given size
         */
        public static DataGridView GetDataGridView(DataGridView theDataGridView, bool newDataGridView, bool dataBound, int cols, int rows)
        {
            if (theDataGridView != null && !newDataGridView)
                return theDataGridView;
            return GetDataGridView(dataBound, cols, rows);
        }

        /*
         * Creates a typical DataGridView of size 100 x 10 if none exists
         * params: theDataGridView - DataGridView you want to use
         *         newDataGridView - true iff want a new DataGridView to be created
         *         dataBound - true if data bound
         *         cols - number of columns
         *         rows - number of rows
         * returns: DataGridView of given size
         */
        public static DataGridView GetDataGridView(bool dataBound, int cols, int rows)
        {
            DataGridView theDataGridView = new DerivedDataGridView();
            DataGridViewTextBoxColumn DataGridViewTextBoxColumn;

            DataTable dt = null;
            if (dataBound)
                dt = new DataTable();

            // Set up columns for the DataGridView
            for (int i = 0; i < cols; i++)
            {
                string str = "Column " + i;

                if (dataBound)
                    dt.Columns.Add(str);
                else
                {
                    DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                    DataGridViewTextBoxColumn.Name = str;
                    theDataGridView.Columns.Add(DataGridViewTextBoxColumn);
                }
            }

            // Set up rows for the DataGridView
            if (dataBound)
            {
                for (int row = 0; row < rows; row++)
                {
                    DataRow dr = dt.NewRow();
                    for (int col = 0; col < cols; col++)
                        dr[col] = "val" + row + "," + col;
                    dt.Rows.Add(dr);
                }
                theDataGridView.DataSource = dt;
            }
            else
            {
                if (rows > 0)
                {
                    theDataGridView.Rows.Add();
                    if (rows > 1)
                        theDataGridView.Rows.AddCopies(0, rows - 1);
                }
            }
            return theDataGridView;
        }

        /*
         * Creates a large DataGridView of size 10,000 x 100 if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView of size 10,000 x 100
         */
        public static DataGridView GetLargeDataGridView(bool newDataGridView)
        {
            return GetDataGridView(largeDataGridView, newDataGridView, false, large_cols, large_rows);
        }

        /*
         * Creates a DataGridView that has no cells if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView with 0 cells
         */
        public static DataGridView GetZeroCellDataGridView()
        {
            return new DerivedDataGridView();
        }

        /*
         * Creates a DataGridView with one cell if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView with 1 cell
         */
        public static DataGridView GetOneCellDataGridView(bool newDataGridView)
        {
            return GetDataGridView(oneCellDataGridView, newDataGridView, false, 1, 1);
        }

        /*
         * Creates a DataGridView with one columns if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView with one column and many rows
         */
        public static DataGridView GetOneColumnDataGridView(bool newDataGridView)
        {
            return GetDataGridView(oneColumnDataGridView, newDataGridView, false, 1, typical_rows);
        }

        /*
         * Creates a DataGridView with no rows if none exists
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView with no rows
         */
        public static DataGridView GetNoRowDataGridView(bool newDataGridView)
        {
            return GetDataGridView(noRowDataGridView, newDataGridView, false, 1, 0);
        }

        /*
         * Obtains random DataGridViewCellBorderStyle enum value that can be assigned to RowHeadersBorderStyle.All
         * property and ColumnHeadersBorderStyle.All property
         * params: p - Test parameters
         * returns: DataGridViewCellBorderStyle - enum value to assign to property
         */
        public static DataGridViewCellBorderStyle GetRandRowColHeaderBorderStyleAll(TParams p)
        {
            DataGridViewCellBorderStyle tcbs;
            do
            {
                tcbs = (DataGridViewCellBorderStyle)p.ru.GetDifferentEnumValue(typeof(System.Windows.Forms.DataGridViewCellBorderStyle), (int)DataGridViewCellBorderStyle.None);
            } while (tcbs == DataGridViewCellBorderStyle.Single);
            return tcbs;
        }

        /*
         * Obtains random DataGridViewCellBorderStyle enum value that can be assigned to RowHeadersBorderStyle.All
         * property and ColumnHeadersBorderStyle.All property
         * params: p - Test parameters
         * returns: DataGridViewCellBorderStyle - enum value to assign to property
         */
        public static DataGridViewCellBorderStyle GetRandCellBorderStyleAll(TParams p)
        {
            DataGridViewCellBorderStyle tcbs;
            do
            {
                tcbs = (DataGridViewCellBorderStyle)p.ru.GetDifferentEnumValue(typeof(System.Windows.Forms.DataGridViewCellBorderStyle), (int)DataGridViewCellBorderStyle.None);
            } while (tcbs == DataGridViewCellBorderStyle.Single || tcbs == DataGridViewCellBorderStyle.Sunken || tcbs == DataGridViewCellBorderStyle.Raised);
            return tcbs;
        }

        public static DataGridView GetDataGridViewWithTextBoxControls(TParams p, bool createNewDataGridView)
        {
            if (textBoxDataGridView == null || createNewDataGridView)
            {
                textBoxDataGridView = GetStandardDataGridView(typeof(DataGridViewTextBoxColumn));
            }
            return textBoxDataGridView;
        }

        public static DataGridView GetDataGridViewWithImageControls(TParams p, bool createNewDataGridView)
        {
            if (imageDataGridView == null || createNewDataGridView)
            {
                imageDataGridView = GetStandardDataGridView(typeof(DataGridViewImageColumn));
            }
            return imageDataGridView;
        }

        public static DataGridView GetDataGridViewWithCheckBoxControls(TParams p, bool createNewDataGridView)
        {
            if (checkBoxDataGridView == null || createNewDataGridView)
            {
                checkBoxDataGridView = GetStandardDataGridView(typeof(DataGridViewCheckBoxColumn));
            }
            return checkBoxDataGridView;
        }


        public static DataGridView GetDataGridViewWithMixtureOfControls(TParams p, bool createNewDataGridView)
        {
            if (mixedControlDataGridView == null || createNewDataGridView)
            {
                mixedControlDataGridView = new DerivedDataGridView();
                mixedControlDataGridView.Visible = true;

                // Set up columns for the DataGridView
                for (int i = 0; i < typical_cols; i++)
                    mixedControlDataGridView.Columns.Add(GetRandomColumn(p));

                // add rows
                mixedControlDataGridView.Rows.Add();
                mixedControlDataGridView.Rows[0].MinimumHeight = 19;
                mixedControlDataGridView.Rows.AddCopies(0, typical_rows - 1);
            }
            return mixedControlDataGridView;
        }

        public static DataGridView GetDataGridViewContainedWithinUserControl(TParams p, bool createNewDataGridView)
        {
            if (userControlDataGridView == null || createNewDataGridView)
            {
                userControlDataGridView = new DerivedDataGridView();
                userControlDataGridView.Visible = true;
            }
            return userControlDataGridView;
        }

        public static DataGridView GetDataGridViewNestedWithinSeveralForms(TParams p, bool createNewDataGridView)
        {
            if (nestedFormsDataGridView == null || createNewDataGridView)
            {
                nestedFormsDataGridView = new DerivedDataGridView();
                nestedFormsDataGridView.Visible = true;
            }
            return nestedFormsDataGridView;
        }

        public static DataGridView GetDataGridViewNestedWithinAnotherDataGridView(TParams p, bool createNewDataGridView)
        {
            if (nestedDataGridViewsDataGridView == null || createNewDataGridView)
            {
                nestedDataGridViewsDataGridView = new DerivedDataGridView();
                nestedDataGridViewsDataGridView.Visible = true;
            }
            return nestedDataGridViewsDataGridView;
        }

        /**
         * Returns a simple test DataGridView
         */
        public static DataGridView GetSimpleDataGridView()
        {
            DataGridView simpleDataGridView = new DerivedDataGridView();

            // Set the DataGridView up so that it has a few columns and rows
            simpleDataGridView.VirtualMode = false;
            simpleDataGridView.GridColor = Color.Navy;
            simpleDataGridView.BackgroundColor = Color.AliceBlue;
            simpleDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            simpleDataGridView.RowHeadersWidth = 45;

            simpleDataGridView.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            simpleDataGridView.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;

            // Set the DataGridView up
            // so that it has a few columns and rows
            DataGridViewTextBoxColumn DataGridViewTextBoxColumn;

            DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            max_col_index += 1;
            DataGridViewTextBoxColumn.Name = "First Name";
            simpleDataGridView.Columns.Add(DataGridViewTextBoxColumn);

            DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            max_col_index += 1;
            DataGridViewTextBoxColumn.Name = "Middle Name";
            simpleDataGridView.Columns.Add(DataGridViewTextBoxColumn);

            DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            max_col_index += 1;
            DataGridViewTextBoxColumn.Name = "Last Name";
            simpleDataGridView.Columns.Add(DataGridViewTextBoxColumn);

            DataGridViewCellStyle DataGridViewCellStyle;

            DataGridViewCellStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle.ForeColor = Color.Black;
            simpleDataGridView.DefaultCellStyle = DataGridViewCellStyle;

            DataGridViewCellStyle = new DataGridViewCellStyle();
            DataGridViewCellStyle.BackColor = Color.White;
            DataGridViewCellStyle.ForeColor = Color.Black;
            DataGridViewCellStyle.Font = new Font(Control.DefaultFont, FontStyle.Bold);
            simpleDataGridView.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle;
            simpleDataGridView.RowHeadersDefaultCellStyle = DataGridViewCellStyle;

            simpleDataGridView.Rows.Add();
            simpleDataGridView.Rows[0].MinimumHeight = 19;
            simpleDataGridView.Rows.AddCopies(0, 5);

            max_row_index = simpleDataGridView.Rows.Count - 1;

            // instantiate our string array of DataGridView values
            DataGridViewValues = new string[5, 3];

            simpleDataGridView.Visible = true;
            simpleDataGridView.Rows[0].Cells[0].Value = "Barney";
            DataGridViewValues[0, 0] = "Barney";
            simpleDataGridView.Rows[0].Cells[1].Value = "Bedrock";
            DataGridViewValues[0, 1] = "Bedrock";
            simpleDataGridView.Rows[0].Cells[2].Value = "Rubble";
            DataGridViewValues[0, 2] = "Rubble";
            simpleDataGridView.Rows[1].Cells[0].Value = "Betty";
            DataGridViewValues[1, 0] = "Betty";
            simpleDataGridView.Rows[1].Cells[1].Value = "Bedrock";
            DataGridViewValues[1, 1] = "Bedrock";
            simpleDataGridView.Rows[1].Cells[2].Value = "Rubble";
            DataGridViewValues[1, 2] = "Rubble";
            simpleDataGridView.Rows[2].Cells[0].Value = "Wilma";
            DataGridViewValues[2, 0] = "Wilma";
            simpleDataGridView.Rows[2].Cells[1].Value = "Bedrock";
            DataGridViewValues[2, 1] = "Bedrock";
            simpleDataGridView.Rows[2].Cells[2].Value = "Flintstone";
            DataGridViewValues[2, 2] = "Flintstone";
            simpleDataGridView.Rows[3].Cells[0].Value = "Fred";
            DataGridViewValues[3, 0] = "Fred";
            simpleDataGridView.Rows[3].Cells[1].Value = "Bedrock";
            DataGridViewValues[3, 1] = "Bedrock";
            simpleDataGridView.Rows[3].Cells[2].Value = "Flintstone";
            DataGridViewValues[3, 2] = "Flintstone";
            simpleDataGridView.Rows[4].Cells[0].Value = "Pebbles";
            DataGridViewValues[4, 0] = "Pebbles";
            simpleDataGridView.Rows[4].Cells[1].Value = "Bedrock";
            DataGridViewValues[4, 1] = "Bedrock";
            simpleDataGridView.Rows[4].Cells[2].Value = "Flintstone";
            DataGridViewValues[4, 2] = "Flintstone";

            s_num_DataGridView_rows = 5;
            s_num_DataGridView_cols = 3;

            return simpleDataGridView;
        }

        /**
         * Creates and returns a DataGridView to use to test
         * the single cell boundary cases
         */
        public static DataGridView GetStandardDataGridView(Type columnType)
        {
            DataGridView = new DerivedDataGridView();

            // Make the DataGridView visible
            DataGridView.Visible = true;

            // Set up columns for the DataGridView
            for (int i = 0; i < standard_cols; i++)
            {
                dataGridViewColumn = (DataGridViewColumn)Activator.CreateInstance(columnType);
                dataGridViewColumn.Name = "Column # " + i;
                if (i == 11 || i == 6)
                {
                    dataGridViewColumn.Visible = false;
                }
                else
                {
                    dataGridViewColumn.Visible = true;
                }
                DataGridView.Columns.Add(dataGridViewColumn);
            }

            // add rows
            DataGridView.Rows.Add();
            DataGridView.Rows[0].MinimumHeight = 19;
            DataGridView.Rows.AddCopies(0, standard_rows);

            // Make one of the rows invisible
            DataGridView.Rows[19].Visible = false;

            // Set the DataGridView up so that it has a few columns and rows
            DataGridView.VirtualMode = false;

            return DataGridView;
        }

        /*
         * Creates a DataGridView in which every cell is visible
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView in which every cell is visible
         */
        public static DataGridView GetVisibleDataGridView(bool newDataGridView)
        {
            if (visibleDataGridView == null || newDataGridView)
            {
                visibleDataGridView = new DerivedDataGridView();

                DataGridViewTextBoxColumn DataGridViewTextBoxColumn;
                // Set up columns for the DataGridView
                for (int i = 0; i < typical_cols; i++)
                {
                    DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                    DataGridViewTextBoxColumn.Name = "Column " + 1;
                    visibleDataGridView.Columns.Add(DataGridViewTextBoxColumn);
                }

                // Set up rows for the DataGridView
                visibleDataGridView.Rows.Add();
                visibleDataGridView.Rows[0].MinimumHeight = 25;
                visibleDataGridView.Rows.AddCopies(0, typical_rows);

                visibleDataGridView.Visible = true;
            }
            return visibleDataGridView;
        }

        /*
         * Creates a DataGridView in which every cell is invisible
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView in which every cell is invisible
         */
        public static DataGridView GetInvisibleDataGridView(bool newDataGridView)
        {
            if (invisibleDataGridView == null || newDataGridView)
            {
                invisibleDataGridView = new DerivedDataGridView();

                DataGridViewTextBoxColumn DataGridViewTextBoxColumn;
                // Set up columns for the DataGridView
                for (int i = 0; i < typical_cols; i++)
                {
                    DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                    DataGridViewTextBoxColumn.Name = "Column " + 1;
                    invisibleDataGridView.Columns.Add(DataGridViewTextBoxColumn);
                }

                // Set up rows for the DataGridView
                invisibleDataGridView.Rows.Add();
                invisibleDataGridView.Rows[0].MinimumHeight = 25;
                invisibleDataGridView.Rows.AddCopies(0, typical_rows);

                // ensure each cell in the DataGridView is invisible
                for (int i = 0; i < invisibleDataGridView.Rows.Count; i++)
                {
                    invisibleDataGridView.Rows[i].Visible = false;
                }
                for (int j = 0; j < invisibleDataGridView.Columns.Count; j++)
                {
                    invisibleDataGridView.Columns[j].Visible = false;
                }
            }

            return invisibleDataGridView;
        }

        /*
         * Creates a DataGridView with a mixture of invisible and visible cells
         * params: newDataGridView - true iff want a new DataGridView to be created
         * returns: DataGridView with a mixture of invisible and visible cells
         */
        public static DataGridView GetMixedInvisibleVisibleDataGridView(bool newDataGridView, TParams p)
        {
            if (mixedInvisibleVisibleDataGridView == null || newDataGridView)
            {
                mixedInvisibleVisibleDataGridView = new DerivedDataGridView();

                DataGridViewTextBoxColumn DataGridViewTextBoxColumn;
                // Set up columns for the DataGridView
                for (int i = 0; i < typical_cols; i++)
                {
                    DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                    DataGridViewTextBoxColumn.Name = "Column " + 1;
                    mixedInvisibleVisibleDataGridView.Columns.Add(DataGridViewTextBoxColumn);
                }

                // Set up rows for the DataGridView
                mixedInvisibleVisibleDataGridView.Rows.Add();
                mixedInvisibleVisibleDataGridView.Rows[0].MinimumHeight = 25;
                mixedInvisibleVisibleDataGridView.Rows.AddCopies(0, typical_rows);

                int rowIndex = 0;

                // make some cells invisible
                for (int i = 0; i < typical_rows / 2; i++)
                {
                    // cannot make (Rows.Count-1) Row Invisible when AllowUserToAddRows=true
                    // will use (Rows.Count - 2) as an upper bound
                    // determine a random row to make invisible
                    rowIndex = p.ru.GetRange(0, mixedInvisibleVisibleDataGridView.Rows.Count - 2);

                    // make row invisible
                    mixedInvisibleVisibleDataGridView.Rows[rowIndex].Visible = false;
                }
            }
            return mixedInvisibleVisibleDataGridView;
        }

        public static DerivedDataGridView GetDerivedDataGridView(TParams p)
        {
            DerivedDataGridView derivedDataGridView = new DerivedDataGridView();

            DataGridViewTextBoxColumn DataGridViewTextBoxColumn;
            // Set up columns for the DataGridView
            for (int i = 0; i < standard_cols; i++)
            {
                DataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
                DataGridViewTextBoxColumn.Name = "Column " + 1;
                derivedDataGridView.Columns.Add(DataGridViewTextBoxColumn);
            }

            // Set up rows for the DataGridView
            derivedDataGridView.Rows.Add();
            derivedDataGridView.Rows[0].MinimumHeight = 25;
            derivedDataGridView.Rows.AddCopies(0, standard_rows);

            int rowIndex = 0;

            // make some cells invisible
            for (int i = 0; i < standard_rows / 2; i++)
            {
                // determine a random row to make invisible
                // when UserAllowToAddRows == true, last row cannot be set Visible=false
                int delta = 1;
                if (derivedDataGridView.AllowUserToAddRows)
                    delta = 2;
                rowIndex = p.ru.GetRange(0, derivedDataGridView.Rows.Count - delta);
                // make row invisible
                derivedDataGridView.Rows[rowIndex].Visible = false;
            }

            return derivedDataGridView;
        }

        //
        // Returns true if DataGridView.Size is big enought so that at least 
        // part of the cell is visible and entire DataGridView is visible 
        // on the screen
        //
        public static bool IsCellPartiallyVisible(DataGridViewCell cell)
        {
            bool positive = (cell.DataGridView.Height > (cell.DataGridView.ColumnHeadersHeight + 5)) &&
                    (cell.DataGridView.Width > (cell.DataGridView.RowHeadersWidth + 5));
            Rectangle workingArea = SystemInformation.WorkingArea;
            Rectangle controlRectangle = cell.DataGridView.RectangleToScreen(cell.DataGridView.Bounds);
            positive &= workingArea.Contains(controlRectangle);
            return positive;
        }

        public static DataGridViewCellStyle GetRandomCellStyle(TParams p)
        {
            DataGridViewCellStyle stl = new DataGridViewCellStyle();
            stl.Alignment = (DataGridViewContentAlignment)p.ru.GetDifferentEnumValue(typeof(DataGridViewContentAlignment), (int)DataGridViewContentAlignment.NotSet);
            stl.BackColor = p.ru.GetColor();
            stl.Font = p.ru.GetFont();
            stl.ForeColor = p.ru.GetColor();
            stl.NullValue = p.ru.GetString(10);
            stl.SelectionBackColor = p.ru.GetColor();
            stl.SelectionForeColor = p.ru.GetColor();
            stl.Tag = p.ru.GetString(10);
            if (stl.Tag == null)
                stl.Tag = "";
            stl.WrapMode = p.ru.GetDifferentEnumValue<DataGridViewTriState>(DataGridViewTriState.NotSet);
            return stl;
        }

        public static DataGridViewColumn GetRandomColumn(TParams p)
        {
            return (DataGridViewColumn)Activator.CreateInstance(GetRandomColumnType(p));
        }

        static List<Type> columnTypes = null;
        public static Type GetRandomColumnType(TParams p)
        {
            if (columnTypes == null)
            {
                columnTypes = new List<Type>();
                Assembly winForms = typeof(Button).Assembly;
                foreach (Type t in winForms.GetTypes())
                    if (t.Name.StartsWith("DataGridView") && t.Name.EndsWith("Column") && t.Name != "DataGridViewColumn")
                    {
                        columnTypes.Add(t);
                    }
            }
            return columnTypes[p.ru.GetRange(0, columnTypes.Count - 1)];
        }

        static List<Type> cellTypes = null;
        public static Type GetRandomCellType(TParams p)
        {
            if (cellTypes == null)
            {
                cellTypes = new List<Type>();
                Assembly winForms = typeof(Button).Assembly;
                foreach (Type t in winForms.GetTypes())
                    if (t.Name.StartsWith("DataGridView") && t.Name.EndsWith("Cell") && t.Name != "DataGridViewCell")
                    {
                        cellTypes.Add(t);
                    }
            }
            return cellTypes[p.ru.GetRange(0, cellTypes.Count - 1)];
        }

        public static DataGridViewCell GetRandomCell(TParams p)
        {
            Type cellType = GetRandomCellType(p);
            return (DataGridViewCell)Activator.CreateInstance(cellType);
        }

        public static bool CompareCellStyles(DataGridViewCellStyle style1, DataGridViewCellStyle style2)
        {
            bool equal = (style1.Alignment == style2.Alignment) &&
                (style1.BackColor.ToArgb() == style2.BackColor.ToArgb()) &&
                (style1.Font.Equals(style2.Font)) &&
                (style1.ForeColor.ToArgb() == style2.ForeColor.ToArgb()) &&
                (String.Compare(style1.NullValue.ToString(), style2.NullValue.ToString()) == 0) &&
                (style1.SelectionBackColor.ToArgb() == style2.SelectionBackColor.ToArgb()) &&
                (style1.SelectionForeColor.ToArgb() == style2.SelectionForeColor.ToArgb()) &&
                (style1.WrapMode == style2.WrapMode);
            if (style1.Tag != null && style2.Tag != null)
                equal &= (String.Compare(style1.Tag.ToString().Trim(), style2.Tag.ToString().Trim()) == 0);

            return equal;
        }

        public static DataGridViewCell GetNewCellOfSameType(DataGridViewCell cell)
        {
            return (DataGridViewCell)Activator.CreateInstance(cell.GetType());
        }

        public static void MakeLastCellCurrent(DataGridView grid)
        {
            grid.CurrentCell = grid.Rows[grid.RowCount - 1].Cells[grid.ColumnCount - 1];
        }

        public static void ThrowOnDataError(DataGridView dgv)
        {
            dgv.DataError += new DataGridViewDataErrorEventHandler(dgv_DataError);
        }

        public static Point GetRandomCellAddress(DataGridView dgv, TParams p)
        {
            if (dgv.RowCount < 1 || dgv.ColumnCount < 1)
                throw new InvalidOperationException("You needs rows and columns to get a random cell address!");
            return new Point(p.ru.GetRange(0, dgv.ColumnCount - 1), p.ru.GetRange(0, dgv.RowCount - 1));
        }

        public static DataGridViewCell GetRandomCell(DataGridView dgv, TParams p)
        {
            Point newAddress = GetRandomCellAddress(dgv, p);
            return dgv.Rows[newAddress.Y].Cells[newAddress.X];
        }

        public static Point GetNoncurrentRandomCellAddress(DataGridView dgv, TParams p)
        {
            if (dgv.RowCount == 1 && dgv.ColumnCount == 1 &&
                dgv.CurrentCellAddress.X == 0 && dgv.CurrentCellAddress.Y == 0)
                throw new InvalidOperationException("Can't find a different cell: you're on the only cell in the DataGridView!");
            Point newValue;
            do
            {
                newValue = GetRandomCellAddress(dgv, p);
            } while (newValue.X == dgv.CurrentCellAddress.X && newValue.Y == dgv.CurrentCellAddress.Y);
            return newValue;
        }

        public static DataGridViewCell GetNoncurrentRandomCell(DataGridView dgv, TParams p)
        {
            Point newAddress = GetNoncurrentRandomCellAddress(dgv, p);
            return dgv.Rows[newAddress.Y].Cells[newAddress.X];
        }

        static void dgv_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            throw new NullReferenceException("Remove this once VSWhidbey 255711 is fixed");
            throw new DataGridViewException("Unexpected exception", e.Exception);
        }

        public static DateTime ObsoleteMemberExpireDate
        {
            get
            {
                return new DateTime(2004, 10, 31);
            }
        }

        public static int S_num_DataGridView_rows { get => s_num_DataGridView_rows; set => s_num_DataGridView_rows = value; }
        public static int S_num_DataGridView_cols { get => s_num_DataGridView_cols; set => s_num_DataGridView_cols = value; }
        public static DataGridView DataGridView1 { get => DataGridView; set => DataGridView = value; }
        public static DataGridView S_typicalDataGridView { get => s_typicalDataGridView; set => s_typicalDataGridView = value; }

        public static void ReorderColumn(DataGridView dgv, int columnToReorder)
        {
            // Not complex, I know, but it's just a simple reorder.  Move the column to the
            // right one unless we're already at the last column, in which case we move to the
            // left.

            int newIndex = columnToReorder;

            if (newIndex == dgv.Columns.Count - 1)
                newIndex--;
            else
                newIndex++;

            dgv.Columns[columnToReorder].DisplayIndex = newIndex;
        }

        public static void ClickRowHeader(TParams p, int row, DataGridView dgv)
        {
            DataGridViewUtils.ClickRowHeader(p, row, Maui.Core.KeyboardFlags.NoFlag, dgv);
        }

        public static void ClickRowHeader(TParams p, int row, Maui.Core.KeyboardFlags flags, DataGridView dgv)
        {
            p.log.WriteLine("Clicking header of Row index: " + row.ToString());
            dgv.FirstDisplayedScrollingRowIndex = row;
            Rectangle rect = dgv.GetCellDisplayRectangle(-1, row, true);
            Point clickPoint = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            clickPoint = dgv.PointToScreen(clickPoint);
            Application.DoEvents();
            Maui.Core.Mouse.Click(Maui.Core.MouseClickType.SingleClick, Maui.Core.MouseFlags.LeftButton, clickPoint.X, clickPoint.Y, flags);
            Application.DoEvents();
        }

        public static void ClickColumnHeader(TParams p, int col, DataGridView dgv)
        {
            DataGridViewUtils.ClickColumnHeader(p, col, Maui.Core.KeyboardFlags.NoFlag, dgv);
        }

        public static void ClickColumnHeader(TParams p, int col, Maui.Core.KeyboardFlags flags, DataGridView dgv)
        {
            p.log.WriteLine("Clicking header of Column index: " + col.ToString());
            dgv.FirstDisplayedScrollingColumnIndex = col;
            Rectangle rect = dgv.GetCellDisplayRectangle(col, -1, true);
            Point clickPoint = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            clickPoint = dgv.PointToScreen(clickPoint);
            Application.DoEvents();
            Maui.Core.Mouse.Click(Maui.Core.MouseClickType.SingleClick, Maui.Core.MouseFlags.LeftButton, clickPoint.X, clickPoint.Y, flags);
            Application.DoEvents();
        }

        public static void ClickTopLeftHeader(TParams p, Maui.Core.KeyboardFlags flags, DataGridView dgv)
        {
            p.log.WriteLine("Clicking top left header");
            Rectangle rect = dgv.GetCellDisplayRectangle(-1, -1, true);
            Point clickPoint = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            clickPoint = dgv.PointToScreen(clickPoint);
            Application.DoEvents();
            Maui.Core.Mouse.Click(Maui.Core.MouseClickType.SingleClick, Maui.Core.MouseFlags.LeftButton, clickPoint.X, clickPoint.Y, flags);
            Application.DoEvents();
        }

        public static void ClickCell(TParams p, int row, int col, Maui.Core.KeyboardFlags flags, DataGridView dgv)
        {
            dgv.FirstDisplayedCell = dgv.Rows[row].Cells[col];
            Rectangle rect = dgv.GetCellDisplayRectangle(col, row, true);
            Point clickPoint = new Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            clickPoint = dgv.PointToScreen(clickPoint);
            Application.DoEvents();
            Maui.Core.Mouse.Click(Maui.Core.MouseClickType.SingleClick, Maui.Core.MouseFlags.LeftButton, clickPoint.X, clickPoint.Y, flags);
            Application.DoEvents();
        }

        public static void ClickOutsideDGV(TParams p, Maui.Core.KeyboardFlags flags, DataGridView dgv)
        {
            Rectangle rect = dgv.ClientRectangle;
            Point clickPoint = new Point(rect.Right + 2, rect.Bottom + 2);
            clickPoint = dgv.PointToScreen(clickPoint);
            Application.DoEvents();
            Maui.Core.Mouse.Click(Maui.Core.MouseClickType.SingleClick, Maui.Core.MouseFlags.LeftButton, clickPoint.X, clickPoint.Y, flags);
            Application.DoEvents();
        }

        public static void SetSelectionMode(TParams p, DataGridViewSelectionMode mode, DataGridView dgv)
        {
            p.log.WriteLine("DataGridViewSelectionMode: " + mode.ToString());
            if (mode == DataGridViewSelectionMode.FullColumnSelect || mode == DataGridViewSelectionMode.ColumnHeaderSelect)
            {
                for (int i = 0; i < dgv.Columns.Count; i++)
                    dgv.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }
            dgv.SelectionMode = mode;
        }

        public static bool IsAnyCellOfColumnSelected(DataGridView dgv, int colIndex)
        {
            for (int i = 0; i < dgv.RowCount; i++)
                if (dgv.Rows[i].Cells[colIndex].Selected)
                    return true;
            return false;
        }

        public static bool IsAnyCellOfRowSelected(DataGridView dgv, int rowIndex)
        {
            for (int i = 0; i < dgv.ColumnCount; i++)
                if (dgv.Rows[rowIndex].Cells[i].Selected)
                    return true;
            return false;
        }

        private static bool IsTabRequired(DataGridView dgv, int colIndex)
        {
            if (colIndex < 0 || colIndex >= dgv.ColumnCount - 1)
                return false;

            bool prevSelected = false;
            for (int i = 0; i < colIndex && !prevSelected; i++)
                if (IsAnyCellOfColumnSelected(dgv, i))
                    prevSelected = true;

            bool postSelected = false;
            for (int i = colIndex + 1; i < dgv.ColumnCount && !postSelected; i++)
                if (IsAnyCellOfColumnSelected(dgv, i))
                    postSelected = true;

            if (dgv.SelectionMode == DataGridViewSelectionMode.FullColumnSelect)
            {

                if (IsAnyCellOfColumnSelected(dgv, colIndex))
                    return postSelected;
                else
                    return false;
            }
            else
            {

                if (IsAnyCellOfColumnSelected(dgv, colIndex) && postSelected)
                    return true;

                if (prevSelected && postSelected)
                    return true;
                else
                    return false;
            }
        }

        public static string GetExpectedClipboardText(TParams p, DataGridView dgv)
        {
            // Please note there are several VSWhidbey bugs around this. IDs are:
            // 517857, 544262, 544265, 544268, 385134
            p.log.WriteLine("DataGridViewClipboardCopyMode: " + dgv.ClipboardCopyMode.ToString());
            p.log.WriteLine("DataGridViewSelectionMode: " + dgv.SelectionMode.ToString());

            string retVal = String.Empty;

            switch (dgv.ClipboardCopyMode)
            {
                case DataGridViewClipboardCopyMode.Disable:
                    return retVal;
                case DataGridViewClipboardCopyMode.EnableWithoutHeaderText:
                    for (int i = 0; i < dgv.RowCount; i++)
                    {
                        if (dgv.SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                            !IsAnyCellOfRowSelected(dgv, i)) continue;

                        if (i != 0)
                            retVal += "\r\n";
                        for (int j = 0; j < dgv.ColumnCount; j++)
                        {
                            if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                            if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                retVal += "\t";
                        }
                    }
                    return retVal.Trim();
                case DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText:
                    for (int i = 0; i < dgv.ColumnCount; i++)
                    {
                        if ((IsAnyCellOfColumnSelected(dgv, i) || IsTabRequired(dgv, i)) && dgv.ColumnHeadersVisible)
                            retVal += "\t" + dgv.Columns[i].HeaderText;
                    }

                    bool startRow = false;
                    for (int i = 0; i < dgv.RowCount; i++)
                    {
                        if (!IsAnyCellOfRowSelected(dgv, i) && !startRow)
                            continue;

                        startRow = true;

                        if (dgv.SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                            !IsAnyCellOfRowSelected(dgv, i)) continue;

                        // Check if any cell is selected in any column starting from this row
                        bool anyCellSelected = false;
                        for (int row = i; row < dgv.RowCount && !anyCellSelected; row++)
                            if (IsAnyCellOfRowSelected(dgv, row))
                                anyCellSelected = true;
                        if (!anyCellSelected)
                            continue;

                        retVal += "\r\n";

                        if (dgv.Rows[i].HeaderCell.FormattedValue != null && dgv.RowHeadersVisible)
                            retVal += dgv.Rows[i].HeaderCell.FormattedValue.ToString() + "\t";

                        for (int j = 0; j < dgv.ColumnCount; j++)
                        {
                            if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                            if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                retVal += "\t";
                        }
                    }
                    return retVal.Trim();
                case DataGridViewClipboardCopyMode.EnableWithAutoHeaderText:
                    switch (dgv.SelectionMode)
                    {
                        case DataGridViewSelectionMode.CellSelect:
                            // Same as EnableWithoutHeaderText
                            for (int i = 0; i < dgv.RowCount; i++)
                            {
                                if (i != 0)
                                    retVal += "\r\n";
                                for (int j = 0; j < dgv.ColumnCount; j++)
                                {
                                    if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                        retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                                    if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                        retVal += "\t";
                                }
                            }
                            return retVal.Trim();
                        case DataGridViewSelectionMode.RowHeaderSelect:
                        case DataGridViewSelectionMode.FullRowSelect:
                            if (dgv.SelectedRows == null || dgv.SelectedRows.Count <= 0)
                            {
                                // Same as EnableWithoutHeaderText
                                for (int i = 0; i < dgv.RowCount; i++)
                                {
                                    if (i != 0)
                                        retVal += "\r\n";
                                    for (int j = 0; j < dgv.ColumnCount; j++)
                                    {
                                        if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                            retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                                        if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                            retVal += "\t";
                                    }
                                }
                                return retVal.Trim();
                            }
                            else
                            {
                                // Same as EnableAlwaysIncludeHeaderText. Only difference is that the
                                // column headers should not be copied.
                                bool startRow2 = false;
                                for (int i = 0; i < dgv.RowCount; i++)
                                {
                                    if (!IsAnyCellOfRowSelected(dgv, i) && !startRow2)
                                        continue;

                                    startRow2 = true;

                                    if (dgv.SelectionMode == DataGridViewSelectionMode.FullRowSelect &&
                                        !IsAnyCellOfRowSelected(dgv, i)) continue;

                                    // Check if any cell is selected in any column starting from this row
                                    bool anyCellSelected = false;
                                    for (int row = i; row < dgv.RowCount && !anyCellSelected; row++)
                                        if (IsAnyCellOfRowSelected(dgv, row))
                                            anyCellSelected = true;
                                    if (!anyCellSelected)
                                        continue;

                                    retVal += "\r\n";

                                    if (dgv.Rows[i].HeaderCell.FormattedValue != null && dgv.RowHeadersVisible)
                                        retVal += dgv.Rows[i].HeaderCell.FormattedValue.ToString() + "\t";

                                    for (int j = 0; j < dgv.ColumnCount; j++)
                                    {
                                        if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                            retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                                        if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                            retVal += "\t";
                                    }
                                }
                                return retVal.Trim();
                            }
                            //break;
                        case DataGridViewSelectionMode.ColumnHeaderSelect:
                        case DataGridViewSelectionMode.FullColumnSelect:
                            if (dgv.SelectedColumns == null || dgv.SelectedColumns.Count <= 0)
                            {
                                // Same as EnableWithoutHeaderText
                                for (int i = 0; i < dgv.RowCount; i++)
                                {
                                    if (i != 0)
                                        retVal += "\r\n";
                                    for (int j = 0; j < dgv.ColumnCount; j++)
                                    {
                                        if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                            retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                                        if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                            retVal += "\t";
                                    }
                                }
                                return retVal.Trim();
                            }
                            else
                            {
                                for (int i = 0; i < dgv.ColumnCount; i++)
                                {
                                    if ((IsAnyCellOfColumnSelected(dgv, i) || IsTabRequired(dgv, i)) && dgv.ColumnHeadersVisible)
                                        retVal += "\t" + dgv.Columns[i].HeaderText;
                                }
                                bool startRow3 = false;
                                for (int i = 0; i < dgv.RowCount; i++)
                                {
                                    if (!IsAnyCellOfRowSelected(dgv, i) && !startRow3)
                                        continue;
                                    startRow3 = true;
                                    retVal += "\r\n";
                                    for (int j = 0; j < dgv.ColumnCount; j++)
                                    {
                                        if (dgv.Rows[i].Cells[j].Selected && dgv.Rows[i].Cells[j].Visible)
                                            retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                                        if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                            retVal += "\t";
                                    }
                                }
                                return retVal.Trim();
                            }
                        default:
                            return retVal;
                    }
                    //break;
                default:
                    return retVal;
            }
        }

        public static string GetExpectedClipboardTextOld(TParams p, DataGridView dgv)
        {
            p.log.WriteLine("DataGridViewClipboardCopyMode: " + dgv.ClipboardCopyMode.ToString());
            p.log.WriteLine("DataGridViewSelectionMode: " + dgv.SelectionMode.ToString());

            string retVal = String.Empty;

            if (dgv.ClipboardCopyMode == DataGridViewClipboardCopyMode.Disable)
                return retVal;

            // Check VSWhidbey 459535
            if (dgv.ClipboardCopyMode == DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText)
            {
                for (int i = 0; i < dgv.ColumnCount; i++)
                {
                    if (IsAnyCellOfColumnSelected(dgv, i) && dgv.ColumnHeadersVisible)
                        retVal += "\t" + dgv.Columns[i].HeaderText;
                }

                for (int i = 0; i < dgv.RowCount; i++)
                {
                    if (!IsAnyCellOfRowSelected(dgv, i))
                        continue;

                    retVal += "\r\n";

                    if (dgv.Rows[i].HeaderCell.Value != null && dgv.RowHeadersVisible)
                        retVal += dgv.Rows[i].HeaderCell.Value.ToString();

                    if (dgv.RowHeadersVisible)
                        retVal += "\t";

                    for (int j = 0; j < dgv.ColumnCount; j++)
                    {
                        if (dgv.Rows[i].Cells[j].Selected)
                            retVal += dgv.Rows[i].Cells[j].Value.ToString();
                        if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                            retVal += "\t";
                    }
                }
                return retVal.Trim();
            }

            if (dgv.ClipboardCopyMode == DataGridViewClipboardCopyMode.EnableWithoutHeaderText)
            {
                for (int i = 0; i < dgv.RowCount; i++)
                {
                    if (i != 0)
                        retVal += "\r\n";
                    for (int j = 0; j < dgv.ColumnCount; j++)
                    {
                        if (dgv.Rows[i].Cells[j].Selected)
                            retVal += dgv.Rows[i].Cells[j].FormattedValue.ToString();
                        if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                            retVal += "\t";
                    }
                }
                return retVal.Trim();
            }

            switch (dgv.SelectionMode)
            {
                case DataGridViewSelectionMode.CellSelect:
                    switch (dgv.ClipboardCopyMode)
                    {
                        case DataGridViewClipboardCopyMode.EnableWithAutoHeaderText:
                            for (int i = 0; i < dgv.RowCount; i++)
                            {
                                if (i != 0)
                                    retVal += "\r\n";
                                for (int j = 0; j < dgv.ColumnCount; j++)
                                {
                                    if (dgv.Rows[i].Cells[j].Selected)
                                        retVal += dgv.Rows[i].Cells[j].Value.ToString();
                                    if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                        retVal += "\t";
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case DataGridViewSelectionMode.ColumnHeaderSelect:
                    switch (dgv.ClipboardCopyMode)
                    {
                        // Check VSWhidbey 385134
                        case DataGridViewClipboardCopyMode.EnableWithAutoHeaderText:
                            if (dgv.ColumnHeadersVisible)
                            {
                                for (int i = 0; i < dgv.ColumnCount; i++)
                                {
                                    if (!IsAnyCellOfColumnSelected(dgv, i))
                                        continue;

                                    if (dgv.Columns[i].Selected && dgv.ColumnHeadersVisible)
                                        retVal += "\t" + dgv.Columns[i].HeaderText;
                                    else
                                        retVal += "\t";
                                }
                            }

                            for (int i = 0; i < dgv.RowCount; i++)
                            {
                                if (!IsAnyCellOfRowSelected(dgv, i))
                                    continue;

                                retVal += "\r\n";
                                for (int j = 0; j < dgv.ColumnCount; j++)
                                {
                                    if (dgv.Rows[i].Cells[j].Selected)
                                        retVal += dgv.Rows[i].Cells[j].Value.ToString();
                                    if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                        retVal += "\t";
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case DataGridViewSelectionMode.RowHeaderSelect:
                    switch (dgv.ClipboardCopyMode)
                    {
                        case DataGridViewClipboardCopyMode.EnableWithAutoHeaderText:
                            for (int i = 0; i < dgv.RowCount; i++)
                            {
                                if (!IsAnyCellOfRowSelected(dgv, i))
                                    continue;

                                if (i != 0)
                                    retVal += "\r\n";

                                if (dgv.Rows[i].Selected && dgv.Rows[i].HeaderCell.Value != null && dgv.RowHeadersVisible)
                                    retVal += dgv.Rows[i].HeaderCell.Value.ToString();
                                retVal += "\t";

                                for (int j = 0; j < dgv.ColumnCount; j++)
                                {
                                    if (dgv.Rows[i].Cells[j].Selected)
                                        retVal += dgv.Rows[i].Cells[j].Value.ToString();
                                    if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                        retVal += "\t";
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case DataGridViewSelectionMode.FullColumnSelect:
                    switch (dgv.ClipboardCopyMode)
                    {
                        case DataGridViewClipboardCopyMode.EnableWithAutoHeaderText:
                            for (int i = 0; i < dgv.ColumnCount; i++)
                            {
                                if (!IsAnyCellOfColumnSelected(dgv, i))
                                    continue;

                                retVal += "\t";
                                if (dgv.Columns[i].Selected == true)
                                    retVal += dgv.Columns[i].HeaderText;
                            }

                            for (int i = 0; i < dgv.RowCount; i++)
                            {
                                retVal += "\r\n";
                                for (int j = 0; j < dgv.ColumnCount; j++)
                                {
                                    if (dgv.Rows[i].Cells[j].Selected)
                                        retVal += dgv.Rows[i].Cells[j].Value.ToString();
                                    if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                        retVal += "\t";
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case DataGridViewSelectionMode.FullRowSelect:
                    switch (dgv.ClipboardCopyMode)
                    {
                        case DataGridViewClipboardCopyMode.EnableWithAutoHeaderText:
                            for (int i = 0; i < dgv.ColumnCount; i++)
                            {
                                retVal += "\t";
                                retVal += dgv.Columns[i].HeaderText;
                            }

                            for (int i = 0; i < dgv.RowCount; i++)
                            {
                                if (i != 0)
                                    retVal += "\r\n";

                                if (dgv.Rows[i].HeaderCell.Value != null)
                                    retVal += dgv.Rows[i].HeaderCell.Value.ToString();
                                retVal += "\t";

                                for (int j = 0; j < dgv.ColumnCount; j++)
                                {
                                    if (dgv.Rows[i].Cells[j].Selected)
                                        retVal += dgv.Rows[i].Cells[j].Value.ToString();
                                    if (j < dgv.ColumnCount - 1 && IsTabRequired(dgv, j))
                                        retVal += "\t";
                                }
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return retVal.Trim();
        }
    }

    class DataGridViewException : Exception
    {
        public DataGridViewException(string str, Exception ex) : base(str, ex) { }

        public override string ToString()
        {
            return base.ToString() + "\n\nInner Exception: \n" + this.InnerException.ToString();
        }

    }
}
