using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public virtual EMailAddress Mail { get; set; }
        public string Password { get; set; }
        public int MailId { get; set; }
        public virtual ICollection<EventApproval> EventApprovals { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }

        public User()
        {
            EventApprovals = new HashSet<EventApproval>();
            Plans = new HashSet<Plan>();
        }
    }
}
