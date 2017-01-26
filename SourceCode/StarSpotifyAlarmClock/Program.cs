using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using SpotifyAPI.Local;
using StarSpotifyAlarmClock.Exceptions;
using StarSpotifyAlarmClock.Models;

namespace StarSpotifyAlarmClock
{
    public class Program
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

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

                MuteComputerVolume();

                StartSpotifyIfItsNotRunning(previousError != null);

                var spotifyLocalApi = new SpotifyLocalAPI();

                if (!spotifyLocalApi.Connect())
                    FailureHandling(inputArguments);

                SpotifyAPI.Local.Models.StatusResponse spotifyStatus = null;
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

                while (Math.Abs(GetMasterVolume()) < 1)
                {
                    System.Threading.Thread.Sleep(inputArguments.MilliSecondsToSleep.Value);
                    keybd_event((byte) Keys.VolumeUp, 0, 0, 0);
                }

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

        private static void MuteComputerVolume()
        {
            while (Math.Abs(GetMasterVolume()) > 0)
            {
                keybd_event((byte) Keys.VolumeDown, 0, 0, 0);
            }
        }

        //TODO: Refactor everything with master volume. Maybe remove unused nuget package.
        public static float GetMasterVolume()
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            const int eRender = 0;
            const int eMultimedia = 1;
            deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out speakers);

            object o;
            speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out o);
            IAudioEndpointVolume aepv = (IAudioEndpointVolume)o;
            float volume = aepv.GetMasterVolumeLevelScalar();
            Marshal.ReleaseComObject(aepv);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
            return volume;
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MMDeviceEnumerator
        {
        }

        [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IAudioEndpointVolume
        {
            void _VtblGap1_6();
            float GetMasterVolumeLevelScalar();
        }

        [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDeviceEnumerator
        {
            void _VtblGap1_1();

            [PreserveSig]
            int GetDefaultAudioEndpoint(int dataFlow, int role, out IMMDevice ppDevice);
        }

        [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IMMDevice
        {
            [PreserveSig]
            int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
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