using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Recognizers.Text.DataTypes.TimexExpression;

namespace WhoIsWho.Helpers
{
    public static class DateTimeDialogExtensions
    {
        public static bool IsDateTime(this string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return timexProperty.Types.Contains("date") ||
                   timexProperty.Types.Contains("time");
        }

        public static bool IsTimeRange(this string timex)
        {
            var timexProperty = new TimexProperty(timex);
            return timexProperty.Types.Contains("TimeRange") ||
                   timexProperty.Types.Contains("DateTimeRange");
        }

        public static DateTime GetDateTime(this string timex)
        {
            var timexProperty = new TimexProperty(timex);
            var today = DateTime.Today;

            var year = timexProperty.Year ?? today.Year;
            var month = timexProperty.Month ?? today.Month;
            var day = timexProperty.DayOfMonth ?? today.Day;
            var hour = timexProperty.Hour ?? 0;
            var minute = timexProperty.Minute ?? 0;

            DateTime result;

            if (timexProperty.DayOfWeek.HasValue)
            {
                result = TimexDateHelpers.DateOfNextDay((DayOfWeek) timexProperty.DayOfWeek.Value, today);
                result = result.AddHours(hour);
                result = result.AddMinutes(minute);
            }
            else
            {
                result = new DateTime(year, month, day, hour, minute, 0);
                if (result < today)
                {
                    result = result.AddYears(1);
                }
            }

            return result;
        }
    }
}
