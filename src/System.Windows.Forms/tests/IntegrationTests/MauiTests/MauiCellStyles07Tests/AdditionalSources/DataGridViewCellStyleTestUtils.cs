using System;
using System.Globalization;
using System.Windows.Forms;
using System.Drawing;

using WFCTestLib.Util;
using WFCTestLib.Log;
using System.Reflection;

/// Contains helper methods for testing the *CellStyleChanged events
/// The helpers are included here instead of the test case because:
/// 1) The code is reused in 7 cases
/// 2) Setting certain properties of a CellStyle to the same things causes
///    *CellStyleChanged to fire.  (WrapMode, BackColor, Font, ForeColor, 
///    Format, NullValue) (321283)
///    Note: It looks like the issues with setting all but the WrapMode from NotSet to NotSet are fixed.
/// 3) DataGridViewCellStyle's FormatProvider property spec is in question (bugid 321121)
/// 
/// author: t-timw
/// 
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public static class DataGridViewCellStyleTestUtils
    {

        public static ScenarioResult SetCellStylePropertyAndCheck(string propertyName, int expectedTimesFired,
        ref int actualTimesFired,
            DataGridViewCellStyle styleToSet, object oldValue, object newValue, Log log)
        {
            ScenarioResult result = new ScenarioResult();

            PropertyInfo pi = typeof(DataGridViewCellStyle).GetProperty(propertyName);

            pi.SetValue(styleToSet, oldValue, null);
            Application.DoEvents();
            actualTimesFired = 0;
            pi.SetValue(styleToSet, newValue, null);
            Application.DoEvents();

            // Use the bug IncCounters here
            if ((propertyName == "WrapMode" && (DataGridViewTriState)oldValue == DataGridViewTriState.NotSet && (DataGridViewTriState)newValue == DataGridViewTriState.NotSet))
            {
                // Setting DataGridView's DefaultCellStyle's
                // BackColor, Font, ForeColor, Format, NullValue, SelectionBackColor and SelectionForeColor to
                // the same ValueType causes DefaultCellStyleChanged to fire.
                // Waiting for action by regisb to fix this.
                result.IncCounters(expectedTimesFired == actualTimesFired, log,
                    BugDb.VSWhidbey, 321283, "Possible code defect. Waiting for fix by regisb.");
            }
            else if (propertyName == "FormatProvider" && oldValue == newValue)
            {
                // DataGridViewCellStyle's format provider spec issue
                return new ScenarioResult(actualTimesFired == expectedTimesFired, log,
                    BugDb.VSWhidbey, 321121, "Spec in question.");

            }
            // The rest of the cases should work as expected
            else
            {
                result.IncCounters(expectedTimesFired, actualTimesFired,
                "Event fired an unexpected number of times when setting the " + pi.Name + " property of cell style.", log);
            }
            return result;
        }


    }
}
