using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarModel
{
    public class DayModel : Model
    {
        public Day Day { get; private set; }

        public override void GenerateDays(Day day)
        {
            Day = new Day(new DateTime(day.Year, day.Month, day.Number));
            //Day.GetEvents();
        }

        public override void GetEventsForDays()
        {
            Day.GetEvents();
        }

        public override Day GetNextDay()
        {
            return this.Day.AddDays(1);
        }

        public override Day GetPreviousDay()
        {
            return this.Day.AddDays(-1);
        }
    }
}
