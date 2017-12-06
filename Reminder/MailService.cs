using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DataAccess;

namespace Reminder
{
    public partial class MailService : ServiceBase
    {
        private Timer timer;
        private SmtpClient client;
        private CalendarDBContext db;

        public MailService()
        {
            InitializeComponent();
            db = new CalendarDBContext();
            client = new SmtpClient();
            timer = new Timer();

            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = ConfigurationManager.AppSettings["Host"];
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["MailFrom"], ConfigurationManager.AppSettings["Password"]);
            timer.Interval = 60000;
            timer.Elapsed += new ElapsedEventHandler(SendMail);
            timer.Start();
            
        }
        
        private void SendMail(object sender, ElapsedEventArgs e)
        {
            var events = db.Events
                .Where(ev => ev.Start.Minute - DateTime.Now.Minute > 28 && ev.Start.Minute - DateTime.Now.Minute < 31);
            if (events.Count() == 0) return;
            foreach (Event myEvent in events)
            {
                List<EventApproval> approvals = db.EventApprovals.Where(ea => ea.EventId == myEvent.Id && ea.Accepted).ToList();
                foreach (EventApproval approval in approvals)
                {
                    var mailAddress = db.EMailAddresses.Where(em => em.Id == approval.User.MailId).First();
                    MailMessage mail = new MailMessage();
                    string body = $"We'd like to remind that event: \"{myEvent.Name}\" will start in 30 minutes:\n";
                    mail.To.Add(mailAddress.Address);
                    mail.From = new MailAddress(ConfigurationManager.AppSettings["MailFrom"]);
                    mail.Subject = "Event reminder";
                    mail.Body = body;
                    client.Send(mail);
                }
            }
        }

        protected override void OnStart(string[] args)
        {
            ServiceLog.WriteErrorLog("Mail service started");
        }

        protected override void OnStop()
        {
            ServiceLog.WriteErrorLog("Mail service stopped");
            timer.Stop();
            timer.Dispose();
            db.Dispose();
        }
    }
}
