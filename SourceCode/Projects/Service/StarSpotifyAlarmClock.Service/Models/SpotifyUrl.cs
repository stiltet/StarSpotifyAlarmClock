using System.Text.RegularExpressions;
using StarSpotifyAlarmClock.Service.Exceptions;

namespace StarSpotifyAlarmClock.Service.Models
{
    public class SpotifyUrl
    {
        private string _value;

        private readonly Regex _validationRegex =
            new Regex(
                "^((https://)(open|play)(.spotify.com/)(user/[a-z]+/playlist/[a-zA-Z0-9]+|track/[a-zA-Z0-9]+|artist/[a-zA-Z0-9]+|album/[a-zA-Z0-9]+))$");

        public string Value
        {
            get { return _value; }
            set { _value = ValidateSpotifyUrl(value); }
        }

        private string ValidateSpotifyUrl(string spotifyUrl)
        {
            if (string.IsNullOrEmpty(spotifyUrl) || !_validationRegex.IsMatch(spotifyUrl))
                throw new ValidationFailedException(
                    "Spotify URL is not valid! Must be a link to a spotify playlist/album/artist or track!");

            return spotifyUrl;
        }

        public SpotifyUrl(string spotifyUrl)
        {
            Value = spotifyUrl;
        }
    }
}