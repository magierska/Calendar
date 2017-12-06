using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalendarModel;

namespace CalendarUnitTest
{
    [TestClass]
    public class DayModelTest
    {
        [TestMethod]
        public void DayGenerateDaysTest()
        {
            DayModel model = new DayModel();
            model.GenerateDays(new Day(new DateTime(2017, 11, 11)));
            Assert.IsTrue(model.Day.DateEquals(new Day(new DateTime(2017, 11, 11))));
        }

        [TestMethod]
        public void DayGetNextDays()
        {
            DayModel model = new DayModel();
            Day day = new Day(new DateTime(2017, 11, 11));
            model.GenerateDays(day);
            Day nextDay = model.GetNextDay();
            Assert.AreEqual(day.DayOfYear + 1, nextDay.DayOfYear);
        }

        [TestMethod]
        public void DayGetPreviousDays()
        {
            DayModel model = new DayModel();
            Day day = new Day(new DateTime(2017, 11, 11));
            model.GenerateDays(day);
            Day previousDay = model.GetPreviousDay();
            Assert.AreEqual(day.DayOfYear - 1, previousDay.DayOfYear);
        }
    }
}
