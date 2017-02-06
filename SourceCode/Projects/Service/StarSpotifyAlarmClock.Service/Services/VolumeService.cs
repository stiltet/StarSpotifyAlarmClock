using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using StarSpotifyAlarmClock.Service.Exceptions;
using StarSpotifyAlarmClock.Service.Interfaces;
using StarSpotifyAlarmClock.Service.Models;

namespace StarSpotifyAlarmClock.Service.Services
{
    public class VolumeService
    {
        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);

        public static bool IncreaseVolumeToMaxOverTime(MilliSecondsToSleep milliSecondsToSleep)
        {
            if(milliSecondsToSleep?.Value == null)
                throw new WrongInputDataException($"Error in: VolumeService.IncreaseVolumeToMaxOverTime(): {nameof(milliSecondsToSleep)} can't be null!");

            while (Math.Abs(GetMasterVolume()) < 1)
            {
                System.Threading.Thread.Sleep(milliSecondsToSleep.Value);
                keybd_event((byte)Keys.VolumeUp, 0, 0, 0);
            }

            return Math.Abs(GetMasterVolume()) >= 1;
        }

        public static bool MuteComputerVolume()
        {
            while (Math.Abs(GetMasterVolume()) > 0)
            {
                keybd_event((byte)Keys.VolumeDown, 0, 0, 0);
            }

            return Math.Abs(GetMasterVolume()) <= 0;
        }

        //TODO: Refactor everything with master volume including interfaces. Maybe remove unused nuget package.
        public static float GetMasterVolume()
        {
            // get the speakers (1st render + multimedia) device
            // ReSharper disable once SuspiciousTypeConversion.Global
            var deviceEnumerator = (IMmDeviceEnumerator) (new MmDeviceEnumerator());
            IMmDevice speakers;
            const int eRender = 0;
            const int eMultimedia = 1;
            deviceEnumerator.GetDefaultAudioEndpoint(eRender, eMultimedia, out speakers);

            object o;
            speakers.Activate(typeof(IAudioEndpointVolume).GUID, 0, IntPtr.Zero, out o);
            var aepv = (IAudioEndpointVolume)o;
            var volume = aepv.GetMasterVolumeLevelScalar();
            Marshal.ReleaseComObject(aepv);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
            return volume;
        }

        [ComImport]
        [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
        private class MmDeviceEnumerator
        {
        }
    }
}
