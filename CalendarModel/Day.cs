using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataAccess;

namespace CalendarModel
{
    public class Day
    {
        DateTime dateTime;
        Label labelTitle;
        PictureBox pictureBox;
        List<Event> events;
        //CalendarDBContext db = new CalendarDBContext();
        public static ToolStripComboBox plansToolStripComboBox;

        public Day(DateTime _dateTime)
        {
            dateTime = _dateTime;
                
            pictureBox = new PictureBox();
            pictureBox.Dock = DockStyle.Fill;
            pictureBox.Image = new Bitmap(pictureBox.Width, pictureBox.Height);
            pictureBox.Tag = this;
        }

        public void GetEvents()
        {
            int id = this.GetPlanId();
            List<Event> tmp = this.GetEventsFromDatabaseWithPlan(id);
            events = this.GetEventsFromDatabaseWithPlan(id);
        }

        public virtual int GetPlanId()
        {
            int id = -1;
            if (plansToolStripComboBox.SelectedIndex > 0)
                id = DataModel.Plans.Find(p => p.Id == (plansToolStripComboBox.SelectedItem as Plan).Id).Id;
            return id;
        }

        private List<Event> GetEventsFromDatabaseWithPlan(int planId)
        {
            int year = dateTime.Year;
            int month = dateTime.Month;
            int day = dateTime.Day;
            List<Event> result;
            using (CalendarDBContext db = new CalendarDBContext())
            {
                result = db.Events
                    .Include("Type.Color")
                    .Include("Plans")
                    .Where(e => e.EventApprovals.Where(ea => ea.User.Id == DataModel.ActiveUser.Id).Count() > 0)
                    .Where(e => SqlFunctions.DatePart("year", e.Start) == year)
                    .Where(e => SqlFunctions.DatePart("month", e.Start) == month)
                    .Where(e => SqlFunctions.DatePart("day", e.Start) == day)
                    .Where(e => planId != -1 ? e.Plans.Any(p => p.Id == planId) : true)
                    .OrderBy(e => e.Start)
                    .ToList();
            }
            return result;
        }

        public int Year
        {
            get
            {
                return dateTime.Year;
            }
        }

        public int Month
        {
            get
            {
                return dateTime.Month;
            }
        }

        public int Number
        {
            get
            {
                return dateTime.Day;
            }
        }

        public DayOfWeek DayOfWeek
        {
            get
            {
                return dateTime.DayOfWeek;
            }
        }

        public string MonthName
        {
            get
            {
                CultureInfo ci = new CultureInfo("en-US");
                return dateTime.ToString("MMMM", ci);
            }
        }

        public List<Event> Events
        {
            get
            {
                return events;
            }
        }

        public Day AddDays(int number)
        {
            return new Day(dateTime.AddDays(number));
        }

        public Day AddMonths(int number)
        {
            return new Day(dateTime.AddMonths(number));
        }

        public int WeekOfYear
        {
            get
            {
                return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }
        }

        public int DayOfYear
        {
            get
            {
                return dateTime.DayOfYear;
            }
        }

        public bool DateEquals(Day day)
        {
            return Year == day.Year && Month == day.Month && Number == day.Number;
        }

        public Label LabelTitle
        {
            get
            {
                return labelTitle;
            }
            set
            {
                labelTitle = value;
            }
        }

        public PictureBox PictureBox
        {
            get
            {
                return pictureBox;
            }
        }
    }
}
