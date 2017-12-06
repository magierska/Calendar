using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CalendarModel;


namespace Calendar
{
    public abstract class View
    {
        protected CalendarModel.Day selectedDay;
        protected Color selectedDayColor = Color.Purple, daysAndHoursColor = Color.DarkGray, notAcceptedEventColor = Color.Gray;
        protected Model model;

        public CalendarModel.Day SelectedDay
        {
            get
            {
                return selectedDay;
            }
        }
        public virtual void Load(CalendarModel.Day _selectedDay = null)
        {
            model.GenerateDays(_selectedDay);
            model.GetEventsForDays();
            this.ShowDays(_selectedDay);
        }
        public abstract void ShowDays(CalendarModel.Day _selectedDay = null);
        public abstract void ShowDay(CalendarModel.Day day);
        public void LoadNext()
        {
            this.Load(model.GetNextDay());
        }
        public void LoadPrevious()
        {
            this.Load(model.GetPreviousDay());
        }
        public virtual void SelectDay(CalendarModel.Day day)
        {
            selectedDay = day;
        }

        public static void SetDoubleBuffered(System.Windows.Forms.Control c)
        {
            if (System.Windows.Forms.SystemInformation.TerminalServerSession)
                return;
            System.Reflection.PropertyInfo aProp = typeof(System.Windows.Forms.Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.NonPublic |
            System.Reflection.BindingFlags.Instance);
            aProp.SetValue(c, true, null);
        }

    }
}
