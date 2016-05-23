using System;
using System.Globalization;
using System.Text.RegularExpressions;
using StarSpotifyAlarmClock.Exceptions;

namespace StarSpotifyAlarmClock.Models
{
    public class MilliSecondsToSleep
    {
        private TimeSpan _value;

        private readonly Regex _validationRegex =
            new Regex(@"^(([0-9]|[1-9][0-9]|[1-9][0-9][0-9]|[1-9][0-9][0-9][0-9]|[1-3][0-5][0-9][0-9][0-9]|36000))$");

        public TimeSpan Value
        {
            get { return _value; }
            set { _value = ValidateValue(value.TotalMilliseconds.ToString(CultureInfo.InvariantCulture)); }
        }

        private string ValueAsAString
        {
            set { _value = ValidateValue(value); }
        }

        private TimeSpan ValidateValue(string millisecondsToSleepAsString)
        {
            if (string.IsNullOrEmpty(millisecondsToSleepAsString))
                return new TimeSpan(0, 0, 0, 5, 0);

            if (!_validationRegex.IsMatch(millisecondsToSleepAsString))
                throw new ValidationFailedException(
                    "Milliseconds to sleep must be an integer with a value between 0 and 36000.");

            var millisecondsToSleep = new TimeSpan(0, 0, 0, 0, Convert.ToInt32(millisecondsToSleepAsString));

            return millisecondsToSleep;
        }

        public MilliSecondsToSleep(string millisecondsToSleepAsString)
        {
            ValueAsAString = millisecondsToSleepAsString;
        }

        public MilliSecondsToSleep(TimeSpan millisecondsToSleep)
        {
            Value = millisecondsToSleep;
        }
    }
}