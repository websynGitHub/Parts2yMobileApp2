using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Parts2y.Parts2y_Models
{
    public class MenuList
    {
        public string Title { get; set; }

        public string IconSource { get; set; }
        public List<int> Role { get; set; }

        public double opacity { get; set; }
        public bool IconType { get; set; }
        public bool Image { get; set; }
        public bool Label { get; set; }
        public bool ISVisible { get; set; }
    }
}
