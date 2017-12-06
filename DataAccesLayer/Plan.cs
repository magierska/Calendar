using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Plan
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Deleted { get; set; }

        public virtual ICollection<Event> Events { get; set; }
        public virtual ICollection<User> Users { get; set; }

        public Plan()
        {
            Events = new HashSet<Event>();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
