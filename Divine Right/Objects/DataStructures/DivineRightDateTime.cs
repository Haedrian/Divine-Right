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

        private readonly int[] MULTIPLIERS = new int[6] { 0, 100, 100000, 1000000, 10000000, 100000000 };

        /// <summary>
        /// Hooray for decimal time. We only need to store the total seconds in here and then just read different multiples of 100 to read the different times
        /// </summary>
        private long time;

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
        /// Gets some part of the time component based on the multiplier
        /// </summary>
        /// <param name="multiplier"></param>
        /// <returns></returns>
        public int GetTimeComponent(DRTimeComponent component)
        {
            //First we divide time by the multiplier
            long dividedTime = time / MULTIPLIERS[(int) component];

            int returnValue = 0;

            //Then, except for years, remainder divide with the multiplier after it
            if ((int)component >= MULTIPLIERS.Length)
            {
                return (int)dividedTime;
            }
            else
            {
                returnValue = (int) dividedTime % MULTIPLIERS[(int)component + 1];
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
            time += MULTIPLIERS[(int)component] * value;
        }

        public void Subtract(DRTimeComponent component, int value)
        {
            time -= MULTIPLIERS[(int)component] * value;
        }

    }
}
