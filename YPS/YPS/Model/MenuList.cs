using System.Collections.Generic;

namespace YPS.Model
{
    public class MenuList
    {
        public string Title { get; set; }
        public string IconSource { get; set; }
        public double opacity { get; set; }
        public List<int> roule { get; set; }
        public bool IconType { get; set; }
        public bool Image { get; set; }
        public bool Label { get; set; }
    }
}
