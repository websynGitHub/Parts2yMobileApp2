using System;
using System.Collections.Generic;
using System.Text;

namespace YPS.Helpers
{
    public class ExceptionFormat
    {
        public static string GetExceptionMessage(Exception ex)
        {
            string retValue = string.Format("Message: {0}\r\nStackTrace: {1}", ex.Message, ex.StackTrace);
            if (ex.InnerException != null)
            {
                retValue = retValue + string.Format("\r\n\r\nInner Exception: {0}", GetExceptionMessage(ex.InnerException));
            }
            return retValue;
        }
    }
}
