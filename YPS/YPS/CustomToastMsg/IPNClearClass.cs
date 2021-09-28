using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.CustomToastMsg
{
    class PNClearClass
    {
    }

    public interface IPNClearClass
    {
        void CancelPush(string tag, int id);
    }
}
