using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CyanVideos
{
    public class MyVolume : Label
    {
        bool update = false;
        bool mousePressed = false;
        Label mask;
        public MyVolume()
        {
            SetVolumeApplication((int)(65535 * ((double)Properties.Settings.Default.volume / 100)));
            mask = new Label() { BackgroundImage = Properties.Resources.mask, BackgroundImageLayout = ImageLayout.Stretch,};
            Timer timer = new Timer() { Enabled = true, Interval = 3, }; timer.Tick += (o, e) => { UpdateVol(); };
            mask.Bounds = new Rectangle(0,0,0,0);
            Controls.Add(mask);
            MouseDown += (o, e) => { mousePressed = true; update = true; };
            MouseUp += (o, e) => { mousePressed = false; };
            MouseMove += (o, e) => { if (mousePressed) update = true; };
            mask.MouseDown += (o, e) => { mousePressed = true; update = true; };
            mask.MouseUp += (o, e) => { mousePressed = false; };
            mask.MouseMove += (o, e) => { if (mousePressed) update = true; };


        }
        int firstPointY = 11;
        int firstPoint = 19;
        int rightMargin = 8;
        public void UpdateVol(bool forced = false)
        {
            if (Program.win!= null && Program.win.mediaPanel!= null && !Program.win.mediaPanel.onTop) return;
            int lastPoint = Width - rightMargin;

            if (forced)
            {
                int pointToClientX = (int)(Properties.Settings.Default.volume * (double)(lastPoint - firstPoint) / 100) + firstPoint;
                mask.Bounds = new Rectangle(pointToClientX, firstPointY, lastPoint - pointToClientX, Height - 2 * firstPointY+1);
                SetVolumeApplication(Properties.Settings.Default.volume);
            }
            if (update)
            {
                int value = (int)((double)(PointToClient(MousePosition).X - firstPoint) / (double)(lastPoint - firstPoint) * 100);
                if (value < 0) value = 0;
                if (value > 100) value = 100;

                int pointToClientX = (int)(value * (double)(lastPoint - firstPoint) / 100) + firstPoint;
                if (value != Properties.Settings.Default.volume)
                {
                    Properties.Settings.Default.volume = value;
                    Properties.Settings.Default.Save();
                    mask.Bounds = new Rectangle(pointToClientX, firstPointY, lastPoint - pointToClientX, Height - 2*firstPointY+1);
                    mask.Update();
                    SetVolumeApplication(value);
                }
            }
            update = false;
        }
        private const int APPCOMMAND_VOLUME_MUTE = 0x80000;
        private const int APPCOMMAND_VOLUME_UP = 0xA0000;
        private const int APPCOMMAND_VOLUME_DOWN = 0x90000;
        private const int WM_APPCOMMAND = 0x319;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessageW(IntPtr hWnd, int Msg,
            IntPtr wParam, IntPtr lParam);




        [DllImport("winmm.dll")]
        private static extern int waveOutSetVolume(IntPtr hwo, uint dwVolume);
        public static void MuteApplication()
        {
            //int NewVolume = 0;
            //uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            //waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);

            setVol("VLC media player (LibVLC ", 0);
        }
        public static void UnMuteApplication()
        {
            int NewVolume = 65535;
            uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            waveOutSetVolume(IntPtr.Zero, NewVolumeAllChannels);
        }
        public static void SetVolumeApplication(int NewVolume)
        {
            //uint NewVolumeAllChannels = (((uint)NewVolume & 0x0000ffff) | ((uint)NewVolume << 16));
            //Console.WriteLine(NewVolumeAllChannels);
            //if(Program.win!= null) waveOutSetVolume(Program.win.Handle, NewVolumeAllChannels);
            setVol("VLC media player (LibVLC 3.0.11)", NewVolume);
            //setVol("Mozilla Firefox", NewVolume);
        }

        static void setVol(string app, int vol)
        {
            foreach (string name in EnumerateApplications())
            {
                //Console.WriteLine("name:" + name);
                if (name.Contains(app))
                {
                    // display mute state & volume level (% of master)
                    //Console.WriteLine("Mute:" + GetApplicationMute(app));
                    //Console.WriteLine("Volume:" + GetApplicationVolume(app));

                    // mute the application
                    //SetApplicationMute(app, true);

                    // set the volume to half of master volume (50%)
                    //Console.WriteLine("Setting vol to "+vol + "  -  "+name);
                    SetApplicationVolume(app, vol);
                }
            }
        }

        public static float? GetApplicationVolume(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            float level;
            volume.GetMasterVolume(out level);
            return level * 100;
        }

        public static bool? GetApplicationMute(string name)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return null;

            bool mute;
            volume.GetMute(out mute);
            return mute;
        }

        public static void SetApplicationVolume(string name, float level)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMasterVolume(level / 100, ref guid);
        }

        public static void SetApplicationMute(string name, bool mute)
        {
            ISimpleAudioVolume volume = GetVolumeObject(name);
            if (volume == null)
                return;

            Guid guid = Guid.Empty;
            volume.SetMute(mute, ref guid);
        }

        public static IEnumerable<string> EnumerateApplications()
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                string dn;
                ctl.GetDisplayName(out dn);
                yield return dn;
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
        }

        private static ISimpleAudioVolume GetVolumeObject(string name)
        {
            // get the speakers (1st render + multimedia) device
            IMMDeviceEnumerator deviceEnumerator = (IMMDeviceEnumerator)(new MMDeviceEnumerator());
            IMMDevice speakers;
            deviceEnumerator.GetDefaultAudioEndpoint(EDataFlow.eRender, ERole.eMultimedia, out speakers);

            // activate the session manager. we need the enumerator
            Guid IID_IAudioSessionManager2 = typeof(IAudioSessionManager2).GUID;
            object o;
            speakers.Activate(ref IID_IAudioSessionManager2, 0, IntPtr.Zero, out o);
            IAudioSessionManager2 mgr = (IAudioSessionManager2)o;

            // enumerate sessions for on this device
            IAudioSessionEnumerator sessionEnumerator;
            mgr.GetSessionEnumerator(out sessionEnumerator);
            int count;
            sessionEnumerator.GetCount(out count);

            // search for an audio session with the required name
            // NOTE: we could also use the process id instead of the app name (with IAudioSessionControl2)
            ISimpleAudioVolume volumeControl = null;
            for (int i = 0; i < count; i++)
            {
                IAudioSessionControl ctl;
                sessionEnumerator.GetSession(i, out ctl);
                string dn;
                ctl.GetDisplayName(out dn);
                if (string.Compare(name, dn, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    volumeControl = ctl as ISimpleAudioVolume;
                    break;
                }
                Marshal.ReleaseComObject(ctl);
            }
            Marshal.ReleaseComObject(sessionEnumerator);
            Marshal.ReleaseComObject(mgr);
            Marshal.ReleaseComObject(speakers);
            Marshal.ReleaseComObject(deviceEnumerator);
            return volumeControl;
        }
    }

    [ComImport]
    [Guid("BCDE0395-E52F-467C-8E3D-C4579291692E")]
    internal class MMDeviceEnumerator
    {
    }

    internal enum EDataFlow
    {
        eRender,
        eCapture,
        eAll,
        EDataFlow_enum_count
    }

    internal enum ERole
    {
        eConsole,
        eMultimedia,
        eCommunications,
        ERole_enum_count
    }

    [Guid("A95664D2-9614-4F35-A746-DE8DB63617E6"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDeviceEnumerator
    {
        int NotImpl1();

        [PreserveSig]
        int GetDefaultAudioEndpoint(EDataFlow dataFlow, ERole role, out IMMDevice ppDevice);

        // the rest is not implemented
    }

    [Guid("D666063F-1587-4E43-81F1-B948E807363F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IMMDevice
    {
        [PreserveSig]
        int Activate(ref Guid iid, int dwClsCtx, IntPtr pActivationParams, [MarshalAs(UnmanagedType.IUnknown)] out object ppInterface);

        // the rest is not implemented
    }

    [Guid("77AA99A0-1BD6-484F-8BC7-2C654C9A9B6F"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionManager2
    {
        int NotImpl1();
        int NotImpl2();

        [PreserveSig]
        int GetSessionEnumerator(out IAudioSessionEnumerator SessionEnum);

        // the rest is not implemented
    }

    [Guid("E2F5BB11-0570-40CA-ACDD-3AA01277DEE8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionEnumerator
    {
        [PreserveSig]
        int GetCount(out int SessionCount);

        [PreserveSig]
        int GetSession(int SessionCount, out IAudioSessionControl Session);
    }

    [Guid("F4B1A599-7266-4319-A8CA-E70ACB11E8CD"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface IAudioSessionControl
    {
        int NotImpl1();

        [PreserveSig]
        int GetDisplayName([MarshalAs(UnmanagedType.LPWStr)] out string pRetVal);

        // the rest is not implemented
    }

    [Guid("87CE5498-68D6-44E5-9215-6DA47EF883D8"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    internal interface ISimpleAudioVolume
    {
        [PreserveSig]
        int SetMasterVolume(float fLevel, ref Guid EventContext);

        [PreserveSig]
        int GetMasterVolume(out float pfLevel);

        [PreserveSig]
        int SetMute(bool bMute, ref Guid EventContext);

        [PreserveSig]
        int GetMute(out bool pbMute);
    }
}
