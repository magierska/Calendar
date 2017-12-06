using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Event
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public bool Deleted { get; set; }
        public virtual ICollection<Plan> Plans { get; set; }
        public int TypeId { get { return Type.Id; } }
        public virtual Type Type { get; set; }
        public virtual User Owner { get; set; }
        public int OwnerId { get; set; }
        public bool Canceled { get; set; }
        public virtual ICollection<EventApproval> EventApprovals { get; set; }
        public Event()
        {
            Plans = new HashSet<Plan>();
        }

        public void Change(string name, Type type, DateTime start, DateTime end)
        {
            Name = name;
            Type = type;
            Start = start;
            End = end;
        }
        public override string ToString()
        {
            return $"{Start.ToString("HH:mm")} - {Name}";
        }

    }
}
