using System;
using System.Text.RegularExpressions;
using StarSpotifyAlarmClock.Exceptions;

namespace StarSpotifyAlarmClock.Models
{
    public class MilliSecondsToSleep
    {
        private int _value;
        private string _valueAsAString;

        private readonly Regex _validationRegex =
            new Regex(
                "^(([1-9]|[1-2][0-9]|30))$");

        public int Value
        {
            get { return _value; }
            set
            {
                _value = ValidateValue(value.ToString());
                ValueAsAString = _value.ToString();
            }
        }

        private string ValueAsAString
        {
            set
            {
                _valueAsAString = ValidateValue(value).ToString();
                _value = Convert.ToInt32(_valueAsAString);
            }
        }

        private int ValidateValue(string secondsToSleepAsString)
        {
            if (string.IsNullOrEmpty(secondsToSleepAsString))
                return 5000;

            if (!_validationRegex.IsMatch(secondsToSleepAsString))
                throw new ValidationFailedException(
                    "Seconds to sleep must be an intriger with a value between 1 and 30.");

            var milliSecondsToSleep = Convert.ToInt32(secondsToSleepAsString)*1000;

            return milliSecondsToSleep;
        }

        public MilliSecondsToSleep(string secondsToSleepAsString)
        {
            ValueAsAString = secondsToSleepAsString;
        }

        public MilliSecondsToSleep(int secondsToSleep)
        {
            Value = secondsToSleep;
        }
    }
}