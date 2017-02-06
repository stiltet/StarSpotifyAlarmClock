using System;

namespace StarSpotifyAlarmClock.Service.Exceptions
{
    public class WrongInputDataException : Exception
    {
        public WrongInputDataException()
        {
        }

        public WrongInputDataException(string message)
            : base(message)
        {
        }

        public WrongInputDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}