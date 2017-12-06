using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalendarModel;

namespace CalendarUnitTest
{
    [TestClass]
    public class WeekModelTest
    {
        [TestMethod]
        public void WeekGenerateDaysTest()
        {
            WeekModel model = new WeekModel();
            model.GenerateDays(new Day(new DateTime(2017, 11, 11)));
            Assert.IsTrue(model.Days[0].DateEquals(new Day(new DateTime(2017, 11, 6))));
            Assert.IsTrue(model.Days.Length == 7);
            Assert.IsTrue(model.Days[6].DateEquals(new Day(new DateTime(2017, 11, 12))));
        }

        [TestMethod]
        public void WeekGetNextDays()
        {
            WeekModel model = new WeekModel();
            Day day = new Day(new DateTime(2017, 11, 11));
            model.GenerateDays(day);
            Day nextDay = model.GetNextDay();
            Assert.AreEqual(day.WeekOfYear + 1, nextDay.WeekOfYear);
        }

        [TestMethod]
        public void WeekGetPreviousDays()
        {
            WeekModel model = new WeekModel();
            Day day = new Day(new DateTime(2017, 11, 11));
            model.GenerateDays(day);
            Day previousDay = model.GetPreviousDay();
            Assert.AreEqual(day.WeekOfYear - 1, previousDay.WeekOfYear);
        }
    }
}
