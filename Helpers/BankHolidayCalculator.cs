using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WhoIsWho.Helpers
{
    //Credits: https://yetanotherchris.dev/csharp/a-c-uk-bank-holiday-calculator/
    public class BankHolidayCalculator
    {
        public bool IsBankHoliday(DateTime dateTime) => IsJanuaryBankHolidayMonday(dateTime) ||
                                                        IsEasterBankHolidayMonday(dateTime) ||
                                                        IsMayBankHolidayMonday(dateTime) ||
                                                        IsAugustBankHolidayMonday(dateTime) ||
                                                        IsDecemberBankHoliday(dateTime);
        public bool IsJanuaryBankHolidayMonday(DateTime currentDateTime)
        {
            // January bank holiday falls on the first working day after New Year's day,
            // which is usually January 1st itself.
            DateTime newYearsDay = new DateTime(currentDateTime.Year, 01, 01);
            DateTime bankHoliday = newYearsDay;
            while (IsWorkingDay(bankHoliday.DayOfWeek) == false)
            {
                bankHoliday = bankHoliday.AddDays(1);
            }

            return currentDateTime.Date == bankHoliday.Date;
        }

        public bool IsEasterBankHolidayMonday(DateTime currentDateTime)
        {
            // Easter bank holiday is always the Monday after Easter Sunday.
            DateTime easterSunday = GetEasterSunday(currentDateTime.Year);
            return easterSunday.AddDays(1).Date == currentDateTime.Date;
        }

        public DateTime GetEasterSunday(int year)
        {
            // From http://stackoverflow.com/a/2510411/21574
            int day = 0;
            int month = 0;

            int g = year % 19;
            int c = year / 100;
            int h = (c - (int)(c / 4) - (int)((8 * c + 13) / 25) + 19 * g + 15) % 30;
            int i = h - (int)(h / 28) * (1 - (int)(h / 28) * (int)(29 / (h + 1)) * (int)((21 - g) / 11));

            day = i - ((year + (int)(year / 4) + i + 2 - c + (int)(c / 4)) % 7) + 28;
            month = 3;

            if (day > 31)
            {
                month++;
                day -= 31;
            }

            return new DateTime(year, month, day);
        }

        public bool IsMayBankHolidayMonday(DateTime currentDateTime)
        {
            // The first Monday of May is a bank holiday (May day)
            DateTime firstMayBankHoliday = new DateTime(currentDateTime.Year, 05, 01);
            while (firstMayBankHoliday.DayOfWeek != DayOfWeek.Monday)
            {
                firstMayBankHoliday = firstMayBankHoliday.AddDays(1);
            }

            if (currentDateTime.Date == firstMayBankHoliday.Date)
                return true;

            // The last Monday of May is a bank holiday (Spring bank holiday)
            DateTime lastMayBankHoliday = new DateTime(currentDateTime.Year, 05, 31);
            while (lastMayBankHoliday.DayOfWeek != DayOfWeek.Monday)
            {
                lastMayBankHoliday = lastMayBankHoliday.AddDays(-1);
            }

            if (currentDateTime.Date == lastMayBankHoliday.Date)
                return true;

            return false;
        }

        public bool IsAugustBankHolidayMonday(DateTime currentDateTime)
        {
            // The last Monday of August is a bank holiday
            DateTime augustBankHoliday = new DateTime(currentDateTime.Year, 08, 31);
            while (augustBankHoliday.DayOfWeek != DayOfWeek.Monday)
            {
                augustBankHoliday = augustBankHoliday.AddDays(-1);
            }

            if (currentDateTime.Date == augustBankHoliday.Date)
                return true;
            else
                return false;
        }

        public bool IsDecemberBankHoliday(DateTime currentDateTime)
        {
            // December's bank holiday is the first working day on, or after Boxing day.
            DateTime boxingDay = new DateTime(currentDateTime.Year, 12, 26);
            DateTime bankHoliday = boxingDay;

            if (boxingDay.DayOfWeek == DayOfWeek.Monday)
                bankHoliday = boxingDay.AddDays(1);

            while (IsWorkingDay(bankHoliday.DayOfWeek) == false)
            {
                bankHoliday = bankHoliday.AddDays(1);
            }

            if (currentDateTime.Date == bankHoliday.Date)
                return true;
            else
                return false;
        }

        public bool IsWorkingDay(DayOfWeek dayOfWeek)
        {
            if (dayOfWeek == DayOfWeek.Saturday || dayOfWeek == DayOfWeek.Sunday)
                return false;

            return true;
        }
    }
}