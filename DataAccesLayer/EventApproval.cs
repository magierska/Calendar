using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class EventApproval
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int EventId { get; set; }
        public virtual Event Event { get; set; }
        public virtual User User { get; set; }
        public bool Accepted { get; set; }
    }
}
