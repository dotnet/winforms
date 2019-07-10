using System;
using System.Drawing;
using System.Windows.Forms;

using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms.IntegrationTests.Common;

//
// Testcase:    DGV_BasicValidationEvents
// Description: Validating, Validated
// Author:      t-timw
//
namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiDGV_BasicValidationEventsTests : ReflectBase
    {

        #region Testcase setup
        public MauiDGV_BasicValidationEventsTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiDGV_BasicValidationEventsTests(args));
        }
        DataGridView grid;
        Button button;


        protected override void InitTest(TParams p)
        {
            base.InitTest(p);


        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            ResetGrid();
            return base.BeforeScenario(p, scenario);
        }

        #endregion
        void ResetGrid()
        {
            Controls.Remove(grid);
            Controls.Remove(button);
            grid = DataGridViewUtils.GetSimpleDataGridView();
            grid.Validating += new System.ComponentModel.CancelEventHandler(grid_Validating);
            grid.Validated += new EventHandler(grid_Validated);
            button = new Button();
            Controls.Add(button);
            Controls.Add(grid);
            button.Location = new Point(grid.Location.X, grid.Location.Y + grid.Height + 10);
            button.Show();
            grid.Show();
            grid.Focus();
            events.Clear();

        }
        #region EventInfo Stuff
        // EventInfo - Class to store an event and its arguments
        class EventInfo
        {
            public EventInfo(EventArgs e, string eventName)
            {
                this.EventArgs = e;
                this.eventName = eventName;
            }
            private EventArgs eventArgs;
            public EventArgs EventArgs
            {
                get
                {
                    return eventArgs;

                }
                set
                {
                    eventArgs = value;
                }
            }
            private string eventName;
            public string EventName
            {
                get
                {
                    return eventName;
                }
                set
                {
                    eventName = value;
                }
            }

        }

        List<EventInfo> events = new List<EventInfo>();

        void OutputEvents(List<EventInfo> eventList)
        {
            foreach (EventInfo ei in eventList)
            {
                scenarioParams.log.WriteLine(GetTextForEvent(ei));
            }
        }
        void AddEvent(List<EventInfo> eventList, EventArgs e, string eventName)
        {

            eventList.Add(new EventInfo(e, eventName));
        }
        string GetTextForEvent(EventInfo ei)
        {
            string text = GetTextForEvent(ei.EventArgs);

            return ei.EventName + ": " + text;

        }
        string GetTextForEvent(EventArgs e)
        {
            PropertyInfo[] pi = e.GetType().GetProperties();
            if (pi.Length == 0)
            {
                return "";
            }
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < pi.Length - 1; i++)
            {
                sb.Append(pi[i].Name + ": " + pi[i].GetValue(e, null) + "; ");
            }
            sb.Append(pi[pi.Length - 1].Name + ": " + pi[pi.Length - 1].GetValue(e, null));
            return sb.ToString();
        }
        string GetTextForEvent(DataGridViewCellMouseEventArgs e)
        {
            return e.ColumnIndex + " " + e.RowIndex + " " + e.Location + " " + e.Button;
        }
        string GetTextForEvent(DataGridViewCellValidatingEventArgs e)
        {
            return e.ColumnIndex + " " + e.RowIndex + " " + e.FormattedValue + " " + e.Cancel;
        }
        #endregion
        [Scenario(false)]
        public ScenarioResult CheckNumberEventsFired(int expectedNumberEvents)
        {
            return new ScenarioResult(expectedNumberEvents, events.Count, "Unexpected number of events fired.", log);
        }
        [Scenario(false)]
        // Checks whether the correct validating event occurred in the event list
        // expectedArgs should be null if checking for Validated event.
        public ScenarioResult CheckEventFire(int index, string expectedEventName, CancelEventArgs expectedArgs)
        {
            ScenarioResult result = new ScenarioResult();

            EventInfo ei = events[index];
            result.IncCounters(expectedEventName, ei.EventName, "Event name is incorrect.", log);
            if (expectedEventName == "Validating")
            {
                result.IncCounters(expectedArgs.Cancel, ((CancelEventArgs)ei.EventArgs).Cancel, "e.Cancel is incorrect.", log);
            }
            return result;

        }
        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Verify that Validating and Validated fire when grid loses focus.")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            button.Focus();

            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, "Validating", new CancelEventArgs(false)));
            result.IncCounters(CheckEventFire(1, "Validated", null));
            return result;
        }

        //[Scenario("Verify that Validating and Validated do not fire when entering data into the cells.")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.CurrentCell = grid.Rows[p.ru.GetRange(0, grid.RowCount - 1)].Cells[p.ru.GetRange(0, grid.ColumnCount - 1)];

            // Try escaping an edit
            SendKeys.SendWait("The event shouldn't fire when escaping an edit.");
            result.IncCounters(CheckNumberEventsFired(0));
            SendKeys.SendWait("{ESCAPE}");
            result.IncCounters(CheckNumberEventsFired(0));

            // Try entering data
            ResetGrid();
            grid.CurrentCell = grid.Rows[p.ru.GetRange(0, grid.RowCount - 1)].Cells[p.ru.GetRange(0, grid.ColumnCount - 1)];
            SendKeys.SendWait("The events should not fire.");
            result.IncCounters(CheckNumberEventsFired(0));
            SendKeys.SendWait("{Enter}");
            result.IncCounters(CheckNumberEventsFired(0));

            return result;
        }
        //[Scenario("Verify that Validating and Validated fire when the grid loses focus after having data entered.")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            grid.CurrentCell = grid.Rows[p.ru.GetRange(0, grid.RowCount - 1)].Cells[p.ru.GetRange(0, grid.ColumnCount - 1)];
            SendKeys.SendWait("Entering some data.");
            result.IncCounters(CheckNumberEventsFired(0));
            SendKeys.SendWait("{Enter}");
            result.IncCounters(CheckNumberEventsFired(0));

            button.Focus();
            result.IncCounters(CheckNumberEventsFired(2));
            result.IncCounters(CheckEventFire(0, "Validating", new CancelEventArgs(false)));
            result.IncCounters(CheckEventFire(1, "Validated", null));


            return result;
        }
        //[Scenario("Verify that cancelling the Validating event causes Validated not to fire.")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult result = new ScenarioResult();

            // Install the alternate handler for this scenario
            grid.Validating -= new System.ComponentModel.CancelEventHandler(grid_Validating);
            grid.Validating += new System.ComponentModel.CancelEventHandler(grid_Validating_Cancel);

            grid.CurrentCell = grid.Rows[p.ru.GetRange(0, grid.RowCount - 1)].Cells[p.ru.GetRange(0, grid.ColumnCount - 1)];
            SendKeys.SendWait("Entering some data.");
            result.IncCounters(CheckNumberEventsFired(0));
            SendKeys.SendWait("{Enter}");
            result.IncCounters(CheckNumberEventsFired(0));

            button.Focus();
            result.IncCounters(CheckNumberEventsFired(1));
            result.IncCounters(CheckEventFire(0, "Validating", new CancelEventArgs(true)));
            grid.Validating -= new System.ComponentModel.CancelEventHandler(grid_Validating_Cancel);
            grid.Validating += new System.ComponentModel.CancelEventHandler(grid_Validating);

            return result;
        }
        #endregion

        void grid_Validating(object sender, CancelEventArgs e)
        {
            AddEvent(events, e, "Validating");
        }
        // Handler that cancels the event
        void grid_Validating_Cancel(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            AddEvent(events, e, "Validating");
        }
        void grid_Validated(object sender, EventArgs e)
        {
            AddEvent(events, e, "Validated");
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that Validating and Validated fire when grid loses focus.
//@ Verify that Validating and Validated do not fire when entering data into the cells.
//@ Verify that Validating and Validated fire when the grid loses focus after having data entered.
//@ Verify that cancelling the Validating event causes Validated not to fire.
