using System;

namespace StarSpotifyAlarmClock.Models
{
    public class InputArguments
    {
        public SpotifyUrl SpotifyUrl { get; set; }

        public MinutesToFadeInVolume MinutesToFadeInVolume
        {
            get { return _minutesToFadeInVolume; }
            set
            {
                _minutesToFadeInVolume = value;
                MilliSecondsToSleep = ConvertMinutesToFadeInVolumeToMilliSecondsToSleep(value);
            }
        }

        public MilliSecondsToSleep MilliSecondsToSleep { get; private set; }

        private MinutesToFadeInVolume _minutesToFadeInVolume;

        private static MilliSecondsToSleep ConvertMinutesToFadeInVolumeToMilliSecondsToSleep(
            MinutesToFadeInVolume minutesToFadeInVolume)
        {
            var totalMilliseconds = TimeSpan.FromMinutes(minutesToFadeInVolume.Value).TotalMilliseconds;
            var milliSecondsToSleep = totalMilliseconds/50;
            return new MilliSecondsToSleep(new TimeSpan(0, 0, 0, 0, Convert.ToInt32(milliSecondsToSleep)));
        }
    }
}