using System;

namespace DAL.Util
{
    public static class DateTimeUtil
    {
        public static string GetIndianFinancialYear(DateTime dateTime)
        {
            if(dateTime.Month > 3)
                return dateTime.Year.ToString().Substring(2,2) + (dateTime.Year + 1).ToString().Substring(2, 2);
            else
                return (dateTime.Year - 1).ToString().Substring(2, 2) + dateTime.Year.ToString().Substring(2, 2);

        }

        public static string GetUSAFinancialYear(DateTime dateTime)
        {
            return dateTime.Year.ToString();
        }
    }
}
