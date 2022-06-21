using System;
using System.Timers;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Xml;
using Newtonsoft.Json;
namespace KeyboardMouseRecorder
{

    static class MouseHook
    {
        private static MSLLHOOKSTRUCT mouseData1;
        private static int nCode;
        private static Timer aTimer;
        private static int timeClock=10;
        private static int last_x = 0;
        private static int last_y = 0;


        //Define output constants
        private const UInt32 MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const UInt32 MOUSEEVENTF_LEFTUP = 0x0004;
        private const UInt32 MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const UInt32 MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const UInt32 MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const UInt32 MOUSEEVENTF_RIGHTUP = 0x0010;
        private const UInt32 MOUSEEVENTF_WHEEL = 0x0800;
        private const int WH_MOUSE_LL = 14;

        private static bool bHookMouse = false;

        private static IntPtr _hookIDMouse = IntPtr.Zero;
        private static LowLevelMouseProc _procMouse = HookCallbackMouse;

     

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        //Mouse Hook
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        //Mouse Output
        //Import DLLs
        [DllImport("user32.dll")]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, uint dwExtraInf);

        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        private struct POINT
        {
            public int X;
            public int Y;
        }


        //
        // SetHookMouse(LowLevelMouseProc proc)
        // Hooks the low level hook to the process
        //
        private static IntPtr SetHookMouse(LowLevelMouseProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_MOUSE_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        //
        // HookCallbackMouse(int nCode, IntPtr wParam, IntPtr lParam)
        // Callback for when a mouse event occurs
        //
        private static IntPtr HookCallbackMouse(  int nCode, IntPtr wParam, IntPtr lParam)
        {

            if (nCode >= 0)
            {
                //salvo le azioni di ogni evento del mouse.Queste azioni le userò poi in onTimedEvent,cosi da prenderne 1 ogni timeclock millisecondi
                mouseData1 = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                if (wParam.ToInt64() != 512)
                {
                    MouseAction ma = new MouseAction
                    {
                        wParam = wParam.ToInt32(),
                        mouseCode = nCode,
                        mouseX = mouseData1.pt.X,
                        mouseY = mouseData1.pt.Y,
                        actionUnixTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds())
                    };
                    Registration.rec_istance.getMouseQueue().Enqueue(ma);
                }
            }
   
            return CallNextHookEx(_hookIDMouse, nCode, wParam, lParam);
        }

        public static void StopHook()
        {
            if (bHookMouse)
            {
                if (_hookIDMouse != IntPtr.Zero)
                {
                    UnhookWindowsHookEx(_hookIDMouse);
                    Debug.WriteLine("Stop Hook Mouse");
                    _hookIDMouse = IntPtr.Zero;

                    bHookMouse = false;
                }
                GC.Collect();
            }
        }

        public static void StartHook()
        {
            if (!bHookMouse)
            {
                _hookIDMouse = SetHookMouse(_procMouse);
                Debug.WriteLine("Start Hook Mouse");
                bHookMouse = true;
                // timer lettura posizione mouse
                startTimer();
            }
        }

        

        private static void startTimer() {
            aTimer = new System.Timers.Timer();
            aTimer.Interval = timeClock;

            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += OnTimedEvent;

            // Have the timer fire repeated events (true is the default)
            aTimer.AutoReset = true;

            // Start the timer
            aTimer.Enabled = true;

        }
        private static void OnTimedEvent(Object source, System.Timers.ElapsedEventArgs e)

        {
           
            if (last_x != mouseData1.pt.X || last_y != mouseData1.pt.Y)
            {
                //creo l'azione con i dati corretti
                MouseAction ma = new MouseAction
                {
                    wParam = 512,
                    mouseCode = nCode,
                    mouseX = mouseData1.pt.X,
                    mouseY = mouseData1.pt.Y,
                    actionUnixTime = (DateTimeOffset.Now.ToUnixTimeMilliseconds())
                };
                //mando l'azione nella coda appropriata
                Registration.rec_istance.getMouseQueue().Enqueue(ma);
                //salvo le ultime coordinate
                last_x = mouseData1.pt.X;
                last_y = mouseData1.pt.Y;
           
            }
        }
    }
}

