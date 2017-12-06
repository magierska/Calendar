using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarModel
{
    public abstract class Model
    {
        public abstract void GenerateDays(Day day);
        public abstract void GetEventsForDays();
        public abstract Day GetNextDay();
        public abstract Day GetPreviousDay();
    }
}
