using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using AppAnalytics = Microsoft.AppCenter.Analytics.Analytics;

namespace YPS.Helpers
{
    public static class YPSLogger
    {
        public static void ReportException(Exception exception, string exceptionMessage)
        {
            var parameters = string.IsNullOrEmpty(exceptionMessage) ? null
                             : new Dictionary<string, string>()
                             {{ "Message", exceptionMessage }};
            Crashes.TrackError(exception, parameters);
        }
       
        public static void TrackEvent(string eventName, string message)
        {
            var parameters = string.IsNullOrEmpty(message) ? null
                            : new Dictionary<string, string>()
                            {{ "Page", message }};
            AppAnalytics.TrackEvent(eventName, parameters);
        }
    }
}