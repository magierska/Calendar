using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarModel
{
    public class WeekModel : Model
    {
        public Day[] Days { get; private set; }
        public override void GenerateDays(Day day)
        {
            Days = new Day[7];
            Day _day = day.AddDays(-(((int)day.DayOfWeek - 1 + 7) % 7));
            for (int i = 0; i < 7; i++)
            {
                Days[i] = _day;
                //Days[i].GetEvents();
                _day = _day.AddDays(1);
            }
        }

        public override void GetEventsForDays()
        {
            foreach (Day day in Days)
                day.GetEvents();
        }

        public override Day GetNextDay()
        {
            return this.Days[0].AddDays(7);
        }

        public override Day GetPreviousDay()
        {
            return this.Days[0].AddDays(-1);
        }
    }
}
