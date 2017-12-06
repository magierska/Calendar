using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalendarModel;
using Rhino.Mocks;

namespace CalendarUnitTest
{
    [TestClass]
    public class MonthModelTest
    {
        [TestMethod]
        public void MonthGenerateDaysTest()
        {
            MonthModel model = new MonthModel();
            model.GenerateDays(new Day(new DateTime(2017, 11, 11)));
            Assert.IsTrue(model.Days[0][0].DateEquals(new Day(new DateTime(2017, 10, 30))));
            Assert.IsTrue(model.Days.Count == 5);
            Assert.IsTrue(model.Days[4][6].DateEquals(new Day(new DateTime(2017, 12, 3))));
        }

        [TestMethod]
        public void MonthGetNextDays()
        {
            MonthModel model = new MonthModel();
            model.GenerateDays(new Day(new DateTime(2017, 11, 11)));
            Day nextDay = model.GetNextDay();
            Assert.AreEqual(12, nextDay.Month);
        }

        [TestMethod]
        public void MonthGetPreviousDays()
        {
            MonthModel model = new MonthModel();
            model.GenerateDays(new Day(new DateTime(2017, 11, 11)));
            Day nextDay = model.GetPreviousDay();
            Assert.AreEqual(10, nextDay.Month);
        }
    }
}
