using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess
{
    public class Type
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ColorID { get { return Color.Id; } }
        public virtual ColorE Color { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
