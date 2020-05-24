using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Native;

namespace ConsoleKeyPresserTest
{
    class Program
    {

        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static readonly LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static readonly InputSimulator SimulatedInput = new InputSimulator();

        //the key beeing pressed
        private static readonly WindowsInput.Native.VirtualKeyCode FakeKeyPress = 
            WindowsInput.Native.VirtualKeyCode.SPACE;

        private static readonly string replacementKey = "LControlKey";

        public static void Main()
        {
            //var handle = GetConsoleWindow();

            // Hide
            //ShowWindow(handle, SW_HIDE);

            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);

        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        private static void FakePPTSplit()
        {
            SimulatedInput.Keyboard.KeyPress(VirtualKeyCode.MENU);
            System.Threading.Thread.Sleep(100);
            SimulatedInput.Keyboard.KeyPress(VirtualKeyCode.VK_J);
            System.Threading.Thread.Sleep(100);
            SimulatedInput.Keyboard.KeyPress(VirtualKeyCode.VK_L);
            System.Threading.Thread.Sleep(100);
            SimulatedInput.Keyboard.KeyPress(VirtualKeyCode.VK_M);
            //System.Threading.Thread.Sleep(100);
            //SimulatedInput.Keyboard.KeyPress(VirtualKeyCode.RETURN);
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                string KeypresssName = ((Keys)vkCode).ToString();
                if (KeypresssName.Equals(replacementKey))
                {
                    Console.WriteLine(replacementKey + " is Pressed...!");
                    //SimulatedInput.Keyboard.KeyPress(FakeKeyPress);
                    FakePPTSplit();
                }
                //Console.WriteLine(KeypresssName);
                //StreamWriter sw = new StreamWriter(Application.StartupPath + @"\log.txt", true);
                //sw.Write((Keys)vkCode);
                //sw.Close();
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //const int SW_HIDE = 0;

    }
}
