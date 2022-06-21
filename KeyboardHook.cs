
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json;
using System;
using System;
using System.Timers;
using System.Collections;
namespace KeyboardMouseRecorder
{

    static class KeyboardHook
    {
     
        
        public delegate void GlobalKeyMenu();
        public static event GlobalKeyMenu OnKeyMenu = null;
        public static int MenuKeyScancode = 29;
        public static UInt64 MenuKeyTime = 300;
        public static bool MenuKeyisExtended = true;
        private static UInt64 MenuKeyDeltaTime = 0;
       
        private static bool bHook = false;
        private static int MaxItems = 50;
        private static UdpClient client = null;
        private static IPEndPoint ep = null;

        //ultimo pulsante premuto
        private static long last_key = 0;
        private static long last_time = 0;
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32")]
        internal extern static UInt64 GetTickCount64();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);


        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct KBDLLHOOKSTRUCT
        {
            public UInt32 vkCode;
            public UInt32 scanCode;
            public UInt32 flags;
            public UInt32 time;
            public UInt32 dwExtraInfo;
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            
                if (nCode >= 0) //&& wParam == (IntPtr)WM_KEYDOWN
                {
                    bool bMenu = false;
                    KBDLLHOOKSTRUCT info = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));
                    BitArray BitFlags = new BitArray(new byte[] { Convert.ToByte(info.flags) });

                    if (info.scanCode == MenuKeyScancode)
                    {
                        if (BitFlags.Get(0) == MenuKeyisExtended)
                            if (BitFlags.Get(7) == true)
                            {
                                if (MenuKeyDeltaTime == 0)
                                    MenuKeyDeltaTime = GetTickCount64();
                                else
                                {
                                    MenuKeyDeltaTime = GetTickCount64() - MenuKeyDeltaTime;
                                    if (MenuKeyDeltaTime < MenuKeyTime)
                                    {
                                        MenuKeyDeltaTime = 0;
                                        bMenu = true;
                                    }
                                }
                            }
                    }

                   // if (info.scanCode != 541)
                  //  {

                        //invio informazione tasto  
                        KeyboardAction ka = new KeyboardAction
                        {
                           keyboardVK = info.vkCode.ToString(),
                            keyboardScancode = info.scanCode.ToString(),
                            keyboardFlags = info.flags.ToString(),
                            actionUnixTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds())
                        };
                        Debug.WriteLine("VkCode: "+info.vkCode);
                        Debug.WriteLine("Flags: " +info.flags);
                        Registration.rec_istance.getKeyboardQueue().Enqueue(ka);
                        //se combinazione particolare - fermo registrazione
                        //Left Ctrl + Left Shift
                        if (info.vkCode == 160 && last_key == 162 && (last_time > info.time - 200))
                        {
                            //salto la combinazione
                            Debug.WriteLine("Combinazione riconosciuta");
                            //fine registrazione
                            Registration.rec_istance.stopRegistration((DateTimeOffset.Now.ToUnixTimeMilliseconds()));
                        }
                        last_key = info.vkCode;
                        last_time = info.time;   
                    }
                //}
            
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }


        public static void StopHook()
        {
            if (bHook)
            {
                if (_hookID != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(_hookID);
                    Debug.WriteLine("Stop Hook");
                    _hookID = IntPtr.Zero;

                    bHook = false;
                }
                GC.Collect();
            }
        }

        public static void StartHook()
        {
            if (!bHook)
            {
                _hookID = SetHook(_proc);
                Debug.WriteLine("Start Hook");
                bHook = true;
                
            }
        }
       

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
    }
}

