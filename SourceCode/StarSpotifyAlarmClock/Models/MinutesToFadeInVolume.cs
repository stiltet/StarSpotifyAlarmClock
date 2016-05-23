using System;
using System.Text.RegularExpressions;
using StarSpotifyAlarmClock.Exceptions;

namespace StarSpotifyAlarmClock.Models
{
    public class MinutesToFadeInVolume
    {
        private int _value;

        private readonly Regex _validationRegex =
            new Regex("^(([0-9]|[1-2][0-9]|30))$");

        public int Value
        {
            get { return _value; }
            set { _value = ValidateValue(value.ToString()); }
        }

        private string ValueAsAString
        {
            set { _value = ValidateValue(value); }
        }

        private int ValidateValue(string minutesToFadeInVolumeAsString)
        {
            if (string.IsNullOrEmpty(minutesToFadeInVolumeAsString))
                return 5;

            if (!_validationRegex.IsMatch(minutesToFadeInVolumeAsString))
                throw new ValidationFailedException(
                    "Minutes to fade in volume must be an intriger with a value between 0 and 30.");

            var minutesToFadeInVolume = Convert.ToInt32(minutesToFadeInVolumeAsString);

            return minutesToFadeInVolume;
        }

        public MinutesToFadeInVolume(string minutesToFadeInVolumeAsString)
        {
            ValueAsAString = minutesToFadeInVolumeAsString;
        }

        public MinutesToFadeInVolume(int minutesToFadeInVolume)
        {
            Value = minutesToFadeInVolume;
        }
    }
}