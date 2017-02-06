using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Win32;
using SpotifyAPI.Local;
using StarSpotifyAlarmClock.Service.Exceptions;
using StarSpotifyAlarmClock.Service.Models;
using StarSpotifyAlarmClock.Service.Services;

namespace StarSpotifyAlarmClock.ConsoleApp
{
    public class Program
    {
        // ReSharper disable once RedundantAssignment
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
            #endif

            StartProgram(arguments);
        }

        private static void StartProgram(
            IReadOnlyList<string> arguments = null,
            InputArguments inputArguments = null,
            string previousError = null,
            bool restart = true)
        {
            if (previousError == null)
                ShowIntroText();
            else
                ShowFailureText(previousError, restart);

            if (inputArguments == null)
                inputArguments = ValidateInputAguments(arguments);

            ShowArgumentsText(inputArguments);

            try
            {
                if (!IsSpotifyInstalled())
                    FailureHandling(inputArguments, "Spotify isn't installed on this computer...", false);

                VolumeService.MuteComputerVolume();

                StartSpotifyIfItsNotRunning(previousError != null);

                var spotifyLocalApi = new SpotifyLocalAPI();

                if (!spotifyLocalApi.Connect())
                    FailureHandling(inputArguments);

                SpotifyAPI.Local.Models.StatusResponse spotifyStatus;
                const int numberOfTries = 5;
                var loopVariable = 0;

                do
                {
                    spotifyStatus = spotifyLocalApi.GetStatus();
                    loopVariable++;
                } while (spotifyStatus == null && loopVariable < numberOfTries);

                if (null == spotifyStatus || !spotifyStatus.Online)
                    FailureHandling(inputArguments, "You are not logged in to Spotify...");

                spotifyLocalApi.SetSpotifyVolume();
                spotifyLocalApi.UnMute();

                spotifyLocalApi.PlayURL(inputArguments.SpotifyUrl.Value);

                VolumeService.IncreaseVolumeToMaxOverTime(inputArguments.MilliSecondsToSleep);

                #if DEBUG
                    Console.WriteLine("    Press the escape key to exit...");
                    while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                    {
                        Console.WriteLine("    Press the escape key to exit...");
                    }
                    Environment.Exit(0);
                #endif
            }
            catch (Exception exception)
            {
                FailureHandling(inputArguments, exception.Message);
            }
        }

        private static void ShowIntroText()
        {
            Console.Title = "Star Spotify Alarm Clock";
            Console.BackgroundColor = ConsoleColor.DarkGreen;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Clear();
            Console.WriteLine();
            Console.WriteLine(
                "----------------------------------------------- Star Spotify Alarm Clock -----------------------------------------------");
            Console.WriteLine("    Created by Stellan Lindell (https://github.com/stiltet/)");
            Console.WriteLine("    Both source code and software is open source and licenced under");
            Console.WriteLine(
                "    Creative Commons Attribution 4.0 International Licence (http://creativecommons.org/licenses/by/4.0/).");
            Console.WriteLine();
            Console.WriteLine("    Application started: {0}", DateTime.Now);
            Console.WriteLine();
            Console.WriteLine(
                "------------------------------------------------------------------------------------------------------------------------");
        }

        private static void ShowFailureText(string previousErrorMessage, bool restart)
        {
            previousErrorMessage = previousErrorMessage ?? string.Empty;

            Console.WriteLine(
                "------------------------------------------------------- Error! ---------------------------------------------------------");
            Console.WriteLine("    Application failed last rund due to: \"{0}\"", previousErrorMessage);
            Console.WriteLine();
            if (restart)
            {
                Console.WriteLine("    Trying again...");
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("    Press the escape key to exit...");
                while (Console.ReadKey(true).Key != ConsoleKey.Escape)
                {
                    Console.WriteLine("    Press the escape key to exit...");
                }
                Environment.Exit(0);
            }
            Console.WriteLine(
                "------------------------------------------------------------------------------------------------------------------------");
        }

        private static InputArguments ValidateInputAguments(IReadOnlyList<string> arguments)
        {
            if (arguments?.FirstOrDefault() == null)
            {
                ShowHelp();
                Environment.Exit(0);
            }

            var spotifyUrl = new SpotifyUrl(arguments[0]);
            var minutesToFadeInVolume = new MinutesToFadeInVolume(null);

            if (arguments.Count >= 2)
                minutesToFadeInVolume = new MinutesToFadeInVolume(arguments[1]);

            return new InputArguments
            {
                SpotifyUrl = spotifyUrl,
                MinutesToFadeInVolume = minutesToFadeInVolume
            };
        }

        private static void ShowHelp()
        {
            Console.WriteLine("    You havn't entered any arguments witch this program needs to be able to run.");
            Console.WriteLine();
            Console.WriteLine("    Argument help:");
            Console.WriteLine();
            Console.WriteLine(
                "        First argument  - This is mandatory and shoul'd always be a Spotify URL to a playlist/album/artist or track.");
            Console.WriteLine(
                "                          Example: \"https://open.spotify.com/track/4hR2PmKODnlFa5fe8iWzeo\"");
            Console.WriteLine();
            Console.WriteLine(
                "        Second argument - This and shoul'd be a an integer between 0 and 30 that represent during how many minutes");
            Console.WriteLine(
                "                          the application will fade in the system volume. Default vaule is 5.");
            Console.WriteLine();
            Console.WriteLine(
                "    Example with all arguments: \"StarSpotifyAlarmClock.exe\" https://open.spotify.com/track/4hR2PmKODnlFa5fe8iWzeo 2");
            Console.WriteLine();
            Console.WriteLine("    Please run the application again with valid arguments.");
            Console.WriteLine();
            Console.WriteLine("    Press the escape key to exit...");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape)
            {
                Console.WriteLine("    Press the escape key to exit...");
            }
        }

        private static void ShowArgumentsText(InputArguments inputArguments)
        {
            Console.WriteLine("    Running application with the following arguments:");
            Console.WriteLine();
            Console.WriteLine("        1 - Spotify URL: \"{0}\"", inputArguments.SpotifyUrl.Value);
            Console.WriteLine("        2 - Minutes to fade in system volume: {0}",
                inputArguments.MinutesToFadeInVolume.Value);
            Console.WriteLine(
                "        3 - Milliseconds to sleep between each increase of volume by 2 (Automaticly converted): {0}",
                inputArguments.MilliSecondsToSleep.Value.TotalMilliseconds);
            Console.WriteLine();
        }

        private static bool IsSpotifyInstalled()
        {
            return Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Spotify") != null;
        }

        private static void StartSpotifyIfItsNotRunning(bool extraSleepTime = false)
        {
            if (!SpotifyLocalAPI.IsSpotifyRunning())
            {
                SpotifyLocalAPI.RunSpotify();
            }

            System.Threading.Thread.Sleep(extraSleepTime ? 50000 : 10000);
        }

        private static void FailureHandling(InputArguments inputArguments, string exceptionMessage = null, bool restart = true)
        {
            try
            {
                var processes = Process.GetProcessesByName("spotify");

                foreach (var process in processes)
                {
                    process.Kill();
                }

                throw new SpotifyException("Couldn't connect to Spotify...");
            }
            catch (Exception exception)
            {
                StartProgram(null, inputArguments, exceptionMessage ?? exception.Message, restart);
            }
        }
    }
}