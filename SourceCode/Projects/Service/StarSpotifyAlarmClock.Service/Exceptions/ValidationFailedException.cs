using System;

namespace StarSpotifyAlarmClock.Service.Exceptions
{
    public class ValidationFailedException : Exception
    {
        public ValidationFailedException()
        {
        }

        public ValidationFailedException(string message)
            : base(message)
        {
        }

        public ValidationFailedException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}