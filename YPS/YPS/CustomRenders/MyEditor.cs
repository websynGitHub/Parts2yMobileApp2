using System;
using Xamarin.Forms;

namespace YPS.CustomRenders
{
    public class MyEditor : Editor
    {
        public MyEditor(){}
        public event EventHandler KeyBoardAppear;
        public event EventHandler KeyBoardDisappear;
        public void NotifyKeyBoardAppear(object item, EventArgs e = null)
        {
            if (KeyBoardAppear != null)
                KeyBoardAppear(item, e);
        }
        public void NotifyKeyBoardDisappear(object item, EventArgs e = null)
        {
            if (KeyBoardDisappear != null)
                KeyBoardDisappear(item, e);
        }
    }
}
