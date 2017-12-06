using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calendar;
using DataAccess;
using System.Reflection;
using System.Collections.Generic;
using CalendarModel;
using Rhino.Mocks;
using System.Linq;

namespace CalendarUnitTest
{
    [TestClass]
    public class DayTest
    {
        List<Event> expected;

        [TestInitialize]
        public void TestInitialize()
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                expected = new List<Event>()
                {
                    new Event()
                    {
                        Name = "Test Event 1",
                        Type = db.Types.Where(t => t.Id == 2).First(),
                        Start = new DateTime(2017, 11, 9, 12, 0, 0),
                        End = new DateTime(2017, 11, 9, 14, 0, 0)
                    },
                    new Event()
                    {
                        Name = "Test Event 2",
                        Type = db.Types.Where(t => t.Id == 4).First(),
                        Start = new DateTime(2017, 11, 9, 9, 0, 0),
                        End = new DateTime(2017, 11, 9, 10, 0, 0)
                    }
                };
                db.Events.AddRange(expected);
                db.SaveChanges();
            }
        }

        [TestMethod]
        public void GetEventsFromDatabaseTest()
        {
            var day = MockRepository.GenerateMock<Day>(new DateTime(2017, 11, 9));
            day.Expect(p => day.GetPlanId()).Return(-1);
            day.GetEvents();
            List<Event> result = day.Events;
            this.AssertListOfEventsEqual(result);
        }

        private void AssertListOfEventsEqual(List<Event> tested)
        {
            foreach (Event e1 in expected)
            {
                bool found = false;
                foreach (Event e2 in tested)
                    if (e1.Name == e2.Name && e1.Type.Id == e2.Type.Id && e1.Start == e2.Start && e1.End == e2.End)
                    {
                        found = true;
                        break;
                    }
                if (!found)
                    Assert.Fail();
            }       
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                foreach (Event ev in db.Events.AsEnumerable().Where(e => e.Name == expected[0].Name || e.Name == expected[1].Name))
                    db.Events.Remove(ev);
            }
        }
    }
}
