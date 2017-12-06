using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarModel
{
    public class MonthModel : Model
    {
        public List<Day[]> Days { get; private set; }

        public override void GenerateDays(Day day)
        {
            Days = new List<Day[]>();
            DateTime date = new DateTime(day.Year, day.Month, 1);
            date = date.AddDays(-(((int)date.DayOfWeek - 1 + 7) % 7));
            int i = -1, j = -1;
            while (true)
            {
                j++;
                j = j % 7;
                if (j == 0)
                {
                    if (date.DayOfWeek == DayOfWeek.Monday && date.Month == day.AddMonths(1).Month)
                        break;
                    Days.Add(new Day[7]);
                    i++;
                }
                Days[i][j] = new Day(date);
                //Days[i][j].GetEvents();
                date = date.AddDays(1);
            }
        }

        public override void GetEventsForDays()
        {
            foreach (Day[] days in Days)
                foreach (Day day in days)
                    day.GetEvents();
        }

        public override Day GetNextDay()
        {
            return this.Days[1][0].AddMonths(1);
        }

        public override Day GetPreviousDay()
        {
            return this.Days[1][0].AddMonths(-1);
        }
    }
}
