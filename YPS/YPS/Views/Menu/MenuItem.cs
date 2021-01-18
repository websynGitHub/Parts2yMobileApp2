using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace YPS.Views.Menu
{
    public class MenuItem
    {
        public string Title { get; set; }

        public string IconSource { get; set; }
        public Color Textcolor { get; set; }

        public double opacity { get; set; }

        public Type TargetType { get; set; }
    }
}
