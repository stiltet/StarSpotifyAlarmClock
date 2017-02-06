using System;
using System.Runtime.InteropServices;
using StarSpotifyAlarmClock.Service.Models;

namespace StarSpotifyAlarmClock.Service.Interfaces
{
    //TODO: REMOVE???
    public interface IVolumeService
    {
        bool IncreaseVolumeToMaxOverTime(MilliSecondsToSleep milliSecondsToSleep);
        bool MuteComputerVolume();
    }

    [Guid("5CDF2C82-841E-4546-9722-0CF74078229A"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IAudioEndpointVolume
    {
        void _VtblGap1_6();
        float GetMasterVolumeLevelScalar();
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMmDeviceEnumerator
    {
        void _VtblGap1_1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(int dataFlow, int role, out IMmDevice ppDevice);
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IMmDevice
    {
        [PreserveSig]
        int Activate([MarshalAs(UnmanagedType.LPStruct)] Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);
    }
}
