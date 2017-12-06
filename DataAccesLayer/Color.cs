using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataAccess
{
    public class ColorE
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public int R { get; set; }
        public int G { get; set; }
        public int B { get; set; }

        [NotMapped]
        public Color Color {
            get
            {
                return Color.FromArgb(R, G, B);
            }
            set
            {
                R = value.R;
                G = value.G;
                B = value.B;
            }
        }
        [NotMapped]
        public System.Drawing.Color LighterColor
        {
            get
            {
                return ControlPaint.LightLight(ControlPaint.LightLight(Color));
            }
        }
  
    }
}
