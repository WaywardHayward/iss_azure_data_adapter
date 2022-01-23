using System;

namespace iss_azure_data_adapter.Utils
{
    public static class IssUtils
    {

        public static DateTime ConvertHoursToTimeStamp(string hours){
            return ConvertHoursToTimeStamp(double.TryParse(hours, out double hoursDouble) ? hoursDouble : 0);
        }

        public static DateTime ConvertHoursToTimeStamp(double hours)
        {
            var yearStart = new DateTime(DateTime.Now.Year,1,1);
            var now = yearStart.AddHours(hours);
            return now;
        }


    }
}