using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CalendarModel;
using DataAccess;
using System.Collections.Generic;
using Rhino.Mocks;

namespace CalendarUnitTest
{
    [TestClass]
    public class DataModelTest
    {
        [TestMethod]
        public void AddEventTest()
        {
            var day = MockRepository.GenerateMock<Day>(new DateTime(2017, 9, 1));
            day.Expect(p => day.GetPlanId()).Return(-1);
            day.GetEvents();
            List<Event> before = day.Events;

            string name = "Event Name";
            DataAccess.Type type = new DataAccess.Type() { Id = 2 };
            DateTime start = new DateTime(2017, 9, 1, 12, 0, 0);
            DateTime end = new DateTime(2017, 9, 1, 14, 0, 0);
            List<int> plansId = new List<int>();
            DataModel.AddEvent(name, type, start, end, plansId);
            
            day.GetEvents();
            List<Event> after = day.Events;
            Assert.AreEqual(before.Count + 1, after.Count);
            foreach (Event ev in after)
                if (!before.Exists(e => e.Id == ev.Id))
                {
                    Assert.AreEqual(name, ev.Name);
                    Assert.AreEqual(type.Id, ev.Type.Id);
                    Assert.AreEqual(start, ev.Start);
                    Assert.AreEqual(end, ev.End);
                    Assert.AreEqual(plansId.Count, ev.Plans.Count);
                    foreach (Plan plan in ev.Plans)
                        Assert.IsTrue(plansId.Contains(plan.Id));
                }
        }

        [TestMethod]
        public void EditEventTest()
        {
            var day = MockRepository.GenerateMock<Day>(new DateTime(2017, 9, 1));
            day.Expect(p => day.GetPlanId()).Return(-1);
            day.GetEvents();
            List<Event> before = day.Events;

            string name = "Edited Event Name";
            DataAccess.Type type = new DataAccess.Type() { Id = 3 };
            DateTime start = new DateTime(2017, 9, 1, 15, 0, 0);
            DateTime end = new DateTime(2017, 9, 1, 18, 0, 0);
            List<int> plansId = new List<int>();
            DataModel.EditEvent(before[0], name, type, start, end, plansId);

            day.GetEvents();
            List<Event> after = day.Events;
            Assert.AreEqual(before.Count, after.Count);
            foreach (Event ev in after)
            {
                if (!before.Exists(e => e.Id == ev.Id))
                    Assert.Fail();
                if (ev.Id == before[0].Id)
                {
                    Assert.AreEqual(name, ev.Name);
                    Assert.AreEqual(type.Id, ev.Type.Id);
                    Assert.AreEqual(start, ev.Start);
                    Assert.AreEqual(end, ev.End);
                    Assert.AreEqual(plansId.Count, ev.Plans.Count);
                    foreach (Plan plan in ev.Plans)
                        Assert.IsTrue(plansId.Contains(plan.Id));
                }
            }
        }

        [TestMethod]
        public void DeleteEventTest()
        {
            var day = MockRepository.GenerateMock<Day>(new DateTime(2017, 9, 1));
            day.Expect(p => day.GetPlanId()).Return(-1);
            day.GetEvents();
            List<Event> before = day.Events;

            int deletedId = before[0].Id;
            DataModel.DeleteEvent(before[0]);

            day.GetEvents();
            List<Event> after = day.Events;
            Assert.AreEqual(before.Count - 1, after.Count);
            foreach (Event ev in after)
                if (!before.Exists(e => e.Id == ev.Id) || ev.Id == deletedId)
                {
                    Assert.Fail();
                }
        }

        [TestMethod]
        public void AddPlansTest()
        {
            List<Plan> plans = DataModel.Plans;
            List<Plan> newPlans = DataModel.Plans;
            newPlans.Add(new Plan() { Name = "Test" });
            DataModel.EditPlans(newPlans);
            newPlans = DataModel.Plans;
            Assert.AreEqual(plans.Count + 1, newPlans.Count);
            foreach (Plan plan in newPlans)
                if (!plans.Contains(plan))
                    Assert.AreEqual("Test", plan.Name);
        }

        [TestMethod]
        public void DeletePlansTest()
        {
            List<Plan> plans = DataModel.Plans;
            List<Plan> newPlans = DataModel.Plans;
            newPlans.RemoveAll(p => p.Name == "Test");
            DataModel.EditPlans(newPlans);
            newPlans = DataModel.Plans;
            Assert.AreEqual(plans.Count - 1, newPlans.Count);
            foreach (Plan plan in plans)
            {
                bool contain = newPlans.Contains(plan);
                if (contain && plan.Name == "Test")
                    Assert.Fail();
                if (!contain && plan.Name != "Test")
                    Assert.Fail();
            }
        }

        [TestMethod]
        public void EditEmailAddressTest()
        {
            string newAddress = "adrestestowy@wp.pl";
            DataModel.EditEmailAddress(newAddress);
            Assert.AreEqual(newAddress, DataModel.EmailAddress);
        }
    }
}
