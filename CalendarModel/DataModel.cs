using DataAccess;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace CalendarModel
{
    public static class DataModel
    {
        public static User ActiveUser { get; private set; }
        public static List<User> Users
        {
            get
            {
                List<User> users;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    users = db.Users.ToList();
                }
                return users;
            }
        }
        public static List<EventApproval> EventApprovals
        {
            get
            {
                List<EventApproval> eventApprovals;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    eventApprovals = db.EventApprovals.ToList();
                }
                return eventApprovals;
            }
        }
        public static List<Event> Events
        {
            get
            {
                List<Event> events;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    events = db.Events.Where(e => e.EventApprovals.Where(ea => ea.User.Id == ActiveUser.Id).Count() > 0).ToList();
                }
                return events;
            }
        }
        public static List<Plan> Plans
        {
            get
            {
                List<Plan> plans;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    int id = ActiveUser.Id;
                    plans = db.Plans.Where(p => p.Users.Where(u => u.Id == id).Count() > 0).ToList();
                }
                return plans;
            }
        }
        public static List<DataAccess.Type> Types
        {
            get
            {
                List<DataAccess.Type> types;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    types = db.Types.ToList();
                }
                return types;
            }
        }

        public static List<EMailAddress> EmailAddresses
        {
            get
            {
                List<EMailAddress> emailAdresses;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    emailAdresses = db.EMailAddresses.ToList();
                }
                return emailAdresses;
            }
        }

        public static string EmailAddress
        {
            get
            {
                string email;
                using (CalendarDBContext db = new CalendarDBContext())
                {
                    email = db.Users.Where(u => u.Id == ActiveUser.Id).First().Mail.Address;
                }
                return email;
            }
        }

        public static void AddEvent(string name, DataAccess.Type type, DateTime start, DateTime end, List<int> plansId, List<int> usersId, bool accepted)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                Event ev = new Event();
                ev.Owner = db.Users.Where(u => u.Id == ActiveUser.Id).First();
                foreach (int id in plansId)
                {
                    db.Plans.AsEnumerable().Where(p => p.Id == id).First().Events.Add(ev);
                    ev.Plans.Add(db.Plans.AsEnumerable().Where(p => p.Id == id).First());
                }
                type = db.Types.Where(t => t.Id == type.Id).First();
                ev.Change(name, type, start, end);
                db.Events.Add(ev);
                db.EventApprovals.Add(new EventApproval() {
                    Event = ev,
                    User = db.Users.Where(u => u.Id == ActiveUser.Id).First(),
                    Accepted = accepted
                });
                foreach (int id in usersId)
                {
                    User user = db.Users.Where(u => u.Id == id).First();
                    db.EventApprovals.Add(new EventApproval()
                    {
                        Event = ev,
                        User = user,
                        Accepted = false
                    });
                    SendInvitationMail(ev, user);
                }
                db.SaveChanges();
            }
        }

        public static void DeleteEvent(Event ev)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                db.EventApprovals.RemoveRange(db.EventApprovals.Where(ea => ea.EventId == ev.Id && ea.UserId == ActiveUser.Id));
                if (!db.EventApprovals.Any(ea => ea.EventId == ev.Id && ea.UserId != ActiveUser.Id))
                    db.Events.Remove(db.Events.Where(e => e.Id == ev.Id).First());
                db.SaveChanges();
            }
        }

        public static void EditEvent(Event ev, string name, DataAccess.Type type, DateTime start, DateTime end, List<int> plansId, List<int> usersId, bool accepted)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                ev = db.Events.Where(e => e.Id == ev.Id).First();
                var plans = db.Plans;
                foreach (Plan plan in plans)
                {
                    plan.Events.Remove(ev);
                }
                foreach (int id in plansId)
                {
                    db.Plans.AsEnumerable().Where(p => p.Id == id).First().Events.Add(ev);
                    ev.Plans.Add(db.Plans.AsEnumerable().Where(p => p.Id == id).First());
                }
                type = db.Types.Where(t => t.Id == type.Id).First();
                ev.Change(name, type, start, end);
                db.EventApprovals.Where(ea => ev.Id == ea.EventId && ActiveUser.Id == ea.UserId).First().Accepted = accepted;
                foreach (int id in usersId)
                {
                    Event thisEvent = db.Events.Where(e => e.Id == ev.Id).First();
                    User user = db.Users.Where(u => u.Id == id).First();
                    db.EventApprovals.Add(new EventApproval()
                    {
                        Event = thisEvent,
                        User = user,
                        Accepted = false
                    });
                    SendInvitationMail(thisEvent, user);
                }
                db.SaveChanges();
            }
        }

        public static void EditPlans(List<Plan> newPlans)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                foreach (Plan plan in Plans)
                    if (newPlans.FindAll(p => p.Id == plan.Id).Count == 0)
                    {
                        if (db.Plans.Where(p => p.Id == plan.Id).First().Users.Count <= 1)
                            db.Plans.Remove(db.Plans.Where(p => p.Id == plan.Id).First());
                        db.Users.Where(u => u.Id == ActiveUser.Id).First().Plans.Remove(db.Plans.Where(p => p.Id == plan.Id).First());
                    }
                foreach (Plan plan in newPlans)
                    if (Plans.FindAll(p => p.Id == plan.Id).Count == 0)
                    {
                        db.Plans.Add(plan);
                        db.Users.Where(u => u.Id == ActiveUser.Id).First().Plans.Add(plan);
                    }
                db.SaveChanges();
            }
        }

        public static void EditEmailAddress(string emailAddress)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                db.Users.Where(u => u.Id == ActiveUser.Id).First().Mail.Address = emailAddress;
                db.SaveChanges();
            }
        }

        public static bool LogIn(string username, string password)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                var users = db.Users.Where(u => (u.UserName == username || u.Mail.Address == username) && u.Password == password);
                if (users.Count() <= 0)
                    return false;
                ActiveUser = users.First();
            }
            return true;
        }

        public static bool SignUp(string username, string email, string password)
        {
            using (CalendarDBContext db = new CalendarDBContext())
            {
                var users = db.Users.Where(u => u.UserName == username || u.Mail.Address == email);
                if (users.Count() > 0)
                    return false;
                db.Users.Add(new User()
                {
                    UserName = username,
                    Password = password,
                    Mail = new EMailAddress()
                    {
                        Address = email
                    }
                });
                db.SaveChanges();
            }
            using (CalendarDBContext db = new CalendarDBContext())
            {
                List<User> tmp = db.Users.ToList();
                ActiveUser = db.Users.Where(u => u.UserName == username && u.Mail.Address == email && u.Password == password).First();
            }
            return true;
        }

        public static void InviteToEvent(Event myEvent, int userId)
        {
            using(CalendarDBContext db = new CalendarDBContext())
            {
                Event thisEvent = db.Events.Where(e => e.Id == myEvent.Id).First();
                User user = db.Users.Where(u => u.Id == userId).First();
                db.EventApprovals.Add(new EventApproval()
                {
                    Event = myEvent,
                    User = user,
                    Accepted = false
                });
                db.SaveChanges();
                SendInvitationMail(thisEvent, user);
            }
        }

        private static void SendInvitationMail(Event myEvent, User guest)
        {
            SmtpClient mailClient = new SmtpClient();
            mailClient.Port = 587;
            mailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
            mailClient.UseDefaultCredentials = false;
            mailClient.Host = "smtp.gmail.com";
            mailClient.EnableSsl = true;
            mailClient.Credentials = new NetworkCredential("calendar.mini@gmail.com", "ZMcal30!");
            using (CalendarDBContext db = new CalendarDBContext())
            {
                var mailAddress = db.EMailAddresses.Where(em => em.Id == guest.MailId).First();
                MailMessage mail = new MailMessage();
                string body = $"User: {ActiveUser.UserName} invites you to event: {myEvent.Name}.\n";
                mail.To.Add(mailAddress.Address);
                mail.From = new MailAddress("calendar.mini@gmail.com");
                mail.Subject = "Invitation";
                mail.Body = body;
                mailClient.Send(mail);
            }
        }

        public static bool IsEventAccepted(int eventId)
        {
            bool answer = false;
            using (CalendarDBContext db = new CalendarDBContext())
            {
                answer = db.EventApprovals.Where(ea => ea.EventId == eventId && ea.UserId == ActiveUser.Id).First().Accepted;
            }
            return answer;
        }
    }
}
