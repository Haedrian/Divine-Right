using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DRObjects.DataStructures.Enum;

namespace DRObjects.DataStructures
{
    /// <summary>
    /// This class represents the DateTime structure used for Divine Right
    /// It is a variant on decimal time
    /// Each year has 10 months of 10 days. Each day is composed of 10 hours each of 100 minutes, each of 100 seconds
    /// </summary>
    [Serializable]
    public class DivineRightDateTime
    {
        /// <summary>
        /// Holds the months of the year. Starts at 1 so we can easily get the month
        /// </summary>
        private readonly string[] MONTHS = new string[11] { "", "Ew", "Tin", "Tie", "Rab", "Ham", "Sit", "Seb", "Tmin", "Dis", "Ash" };

        private readonly long[] MULTIPLIERS = new long[6] { 1, 100, 10000, 100000, 1000000, 10000000 };

        /// <summary>
        /// The month has changed.
        /// </summary>
        public event EventHandler MonthChanged;

        /// <summary>
        /// The Day has Changed
        /// </summary>
        public event EventHandler DayChanged;
        /// <summary>
        /// The Minute has changed
        /// </summary>
        public event EventHandler MinuteChanged;

        /// <summary>
        /// Hooray for decimal time. We only need to store the total seconds in here and then just read different multiples of 100 to read the different times
        /// </summary>
        private long time;

        /// <summary>
        /// Creates a new DateTime having the same time as the passed one
        /// </summary>
        /// <param name="oldTime"></param>
        public DivineRightDateTime(DivineRightDateTime oldTime)
        {
            this.time = oldTime.time;
        }

        public DivineRightDateTime(long time)
        {
            this.time = time;
        }

        /// <summary>
        /// Creates a new DRDateTime representing a particular year.
        /// </summary>
        /// <param name="year"></param>
        /// <param name="month"></param>
        /// <param name="day"></param>
        public DivineRightDateTime(int year, int month, int day)
        {
            //Month and year are subtracted by 1 because people tend to start months and days at 1
            time = 0;
            Add(DRTimeComponent.YEAR, year);
            Add(DRTimeComponent.MONTH, month);
            Add(DRTimeComponent.DAY, day);
        }

        public DivineRightDateTime(int year, int month, int day, int hour, int minute, int seconds)
        {
            //Month and year are subtracted by 1 because people tend to start months and days at 1
            time = 0;
            Add(DRTimeComponent.YEAR, year);
            Add(DRTimeComponent.MONTH, month);
            Add(DRTimeComponent.DAY, day);
            Add(DRTimeComponent.HOUR, hour);
            Add(DRTimeComponent.MINUTE, minute);
            Add(DRTimeComponent.SECOND, seconds);
        }

        /// <summary>
        /// Expresses the Date in [Dayth of Month, year]
        /// </summary>
        /// <returns></returns>
        public string GetDateString()
        {
            string dateString = String.Empty;

            //Get the day part
            int day = GetTimeComponent(DRTimeComponent.DAY);

            dateString += day;

            switch(day)
            {
                case 1:
                    dateString += "st of"; break;
                case 2:
                    dateString += "nd of"; break;
                case 3: dateString += "rd of"; break;
                default:
                    dateString += "th of"; break;
            } 

            dateString += GetMonthName() + ",";

            dateString += this.GetTimeComponent(DRTimeComponent.YEAR);

            return dateString;
        }

        /// <summary>
        /// Gets some part of the time component based on the multiplier
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public int GetTimeComponent(DRTimeComponent component)
        {
            //First we divide time by the multiplier
            long dividedTime = Math.Abs( time / MULTIPLIERS[(int) component]);

            int returnValue = 0;

            //Then, except for years, remainder divide with the multiplier after it
            if ((int)component >= MULTIPLIERS.Length -1)
            {
                return (int)dividedTime;
            }
            else
            {
                returnValue = (int) (dividedTime % (MULTIPLIERS[(int)component + 1] / MULTIPLIERS[(int)component]));
            }

            if (component == DRTimeComponent.DAY || component == DRTimeComponent.MONTH)
            {
                //Increment by 1
                return returnValue + 1;
            }

            return returnValue;
        }

        /// <summary>
        /// Sets a particular Time Component to a particular value.
        /// If the value is 'too large' it will trickle over
        /// </summary>
        /// <param name="component"></param>
        public void SetTimeComponent(DRTimeComponent component, int value)
        {
            if (component == DRTimeComponent.DAY || component == DRTimeComponent.MONTH)
            {
                value--; //Because people start days and months at 1
            }

            //First determine the current value
            long currentValue = GetTimeComponent(component);

            //Multiply that by the multiplier
            currentValue *= MULTIPLIERS[(int)component];

            time -= currentValue;

            //And replace it with our new value
            time += MULTIPLIERS[(int)component] * value;
        }

        /// <summary>
        /// Returns the name of the current month
        /// </summary>
        /// <returns></returns>
        public string GetMonthName()
        {
            return MONTHS[GetTimeComponent(DRTimeComponent.MONTH)];
        }

        /// <summary>
        /// Adds a time component to the total
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public void Add(DRTimeComponent component, int value)
        {
            if (component == DRTimeComponent.MONTH || component == DRTimeComponent.DAY)
            {
                value--;
            }

            //Store Minute, Month and Year
            int minute = this.GetTimeComponent(DRTimeComponent.MINUTE);
            int month = this.GetTimeComponent(DRTimeComponent.MONTH);
            int day = this.GetTimeComponent(DRTimeComponent.DAY);

            time += MULTIPLIERS[(int)component] * value;

            if (minute != this.GetTimeComponent(DRTimeComponent.MINUTE))
            {
                if (MinuteChanged != null)
                {
                    //Fire the event
                    MinuteChanged(this, null);
                }
            }
            
            if (month != this.GetTimeComponent(DRTimeComponent.MONTH))
            {
                if (MonthChanged != null)
                {
                    MonthChanged(this, null);
                }
            }
            
            if (day != this.GetTimeComponent(DRTimeComponent.DAY))
            {
                if (DayChanged != null)
                {
                    //Fire the event
                    DayChanged(this, null);
                }
            }
        }

        public void Subtract(DRTimeComponent component, int value)
        {
            if (component == DRTimeComponent.MONTH || component == DRTimeComponent.DAY)
            {
                value--;
            }

            time -= MULTIPLIERS[(int)component] * value;
        }

        public static DivineRightDateTime operator+(DivineRightDateTime date1, DivineRightDateTime date2) 
        {
            return new DivineRightDateTime(date1.time + date2.time);
        }

        public static DivineRightDateTime operator-(DivineRightDateTime date1,DivineRightDateTime date2)
        {
            return new DivineRightDateTime(date1.time - date2.time);
        }
    }
}
