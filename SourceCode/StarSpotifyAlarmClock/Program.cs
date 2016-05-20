using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using StarSpotifyAlarmClock.Models;

namespace StarSpotifyAlarmClock
{
    public class Program
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public static void Main(string[] arguments)
        {
#if DEBUG
            arguments = new[]
            {
                //"https://open.spotify.com/user/stiltet/playlist/7iGyN8pjT9D3vuoNuChHje",
                //"https://open.spotify.com/track/6iA3KyiC4RxcwjlUnRrhjU",
                //"https://open.spotify.com/album/7EUG9mn2FHSdfpGRrSYoyw",
                //"https://open.spotify.com/artist/2FHOS0GkJv3EyT8a9BhP9E",
                "https://play.spotify.com/user/stiltet/playlist/7iGyN8pjT9D3vuoNuChHje",
                "1"
            };
            arguments = null;
#endif

            Console.Title = "Star Spotify Alarm Clock";
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(
                "----------------------------------------------- Star Spotify Alarm Clock -----------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("   Created by Stellan Lindell (https://github.com/stiltet/)");
            Console.WriteLine("   Both source code and software is open source and licenced under");
            Console.WriteLine(
                "   Creative Commons Attribution 4.0 International Licence (http://creativecommons.org/licenses/by/4.0/).");
            Console.WriteLine();
            Console.WriteLine(
                "------------------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();

            var inputArguments = ValidateInputAguments(arguments);

            MuteComputerVolume();

            if (!SpotifyAPI.Local.SpotifyLocalAPI.IsSpotifyRunning())
                SpotifyAPI.Local.SpotifyLocalAPI.RunSpotify();

            System.Threading.Thread.Sleep(10000);

            var spotifyApi = new SpotifyAPI.Local.SpotifyLocalAPI();

            if (!spotifyApi.Connect())
                throw new AccessViolationException("Couldn't connect to Spotify...");

            System.Threading.Thread.Sleep(5000);

            spotifyApi.SetSpotifyVolume();
            spotifyApi.UnMute();

            spotifyApi.PlayURL(inputArguments.SpotifyUrl.Value);

            var loopVariable = 0;

            while (loopVariable < 50)
            {
                System.Threading.Thread.Sleep(inputArguments.MilliSecondsToSleep.Value);
                keybd_event((byte) Keys.VolumeUp, 0, 0, 0);
                loopVariable++;
            }
        }

        private static InputArguments ValidateInputAguments(IReadOnlyList<string> arguments)
        {
            if (null == arguments || null == arguments.FirstOrDefault())
            {
                ShowHelp();
                Environment.Exit(0);
            }

            var spotifyUrl = new SpotifyUrl(arguments[0]);
            var milliSecondsToSleep = new MilliSecondsToSleep(null);

            if (arguments.Count >= 2)
                milliSecondsToSleep = new MilliSecondsToSleep(arguments[1]);

            return new InputArguments
            {
                SpotifyUrl = spotifyUrl,
                MilliSecondsToSleep = milliSecondsToSleep
            };
        }

        private static void ShowHelp()
        {
            Console.WriteLine("   You havn't entered any arguments witch this program needs to be able to run.");
            Console.WriteLine();
            Console.WriteLine("   Argument help:");
            Console.WriteLine();
            Console.WriteLine(
                "        First argument  - This is mandatory and shoul'd always be a Spotify URL to a playlist/album/artist or track.");
            Console.WriteLine(
                "                          Example: \"https://open.spotify.com/track/4hR2PmKODnlFa5fe8iWzeo\"");
            Console.WriteLine();
            Console.WriteLine(
                "        Second argument - This and shoul'd be a an integer that represent the amount of seconds the application will");
            Console.WriteLine(
                "                          wait before fading up the system volume by 2. Default vaule is 5.");
            Console.WriteLine();
            Console.WriteLine(
                "   Example: \"StarSpotifyAlarmClock.exe\" https://open.spotify.com/track/4hR2PmKODnlFa5fe8iWzeo 2");
            Console.WriteLine();
            Console.WriteLine("   Please run the application again with valid arguments.");
            Console.WriteLine();
            Console.WriteLine("   Press any key to exit...");
            Console.WriteLine();
            Console.ReadKey();
        }

        private static void MuteComputerVolume()
        {
            var loopVariable = 0;

            while (loopVariable < 5000)
            {
                keybd_event((byte) Keys.VolumeDown, 0, 0, 0);
                loopVariable++;
            }
        }
    }
}