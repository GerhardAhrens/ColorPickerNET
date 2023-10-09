//-----------------------------------------------------------------------
// <copyright file="GlobalKeyboardHook.cs" company="Lifeprojects.de">
//     Class: GlobalKeyboardHook
//     Copyright © Gerhard Ahrens, 2023
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>02.10.2023</date>
//
// <summary>
// Die Klasse stellt einen globale Hotkeys für die Applikation zur Verfügung.
// </summary>
// <Website>
// </Website>
// <example>
// GlobalKeyboardHook globalKey = new GlobalKeyboardHook();
// globalKey.KeyDown += globalKey_KeyDown;
// globalKey.HookedKeys.Add(Keys.F2);
// 
// private void globalKey_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
// {
//    if (e.KeyCode == Keys.F2)
//    {
//        e.Handled = true;
//    }
// }
// </example>
//-----------------------------------------------------------------------

namespace ColorPickerNET.Core
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;
    using System.Security;
    using System.Windows.Forms;

    public class GlobalKeyboardHook
    {
        const int WH_KEYBOARD_LL = 13;
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int WM_SYSKEYDOWN = 0x104;
        const int WM_SYSKEYUP = 0x105;

        private keyboardHookProc khp;
        private IntPtr hhook = IntPtr.Zero;

        public List<Keys> HookedKeys = new List<Keys>();

        #region DLL imports

        /*
        SecurityCritical: 
         Gibt an, dass Code oder eine Assembly sicherheitsrelevante Vorgänge ausführt.
        SuppressUnmanagedCodeSecurity:
          Ermöglicht es verwaltetem Code, Aufrufe in nicht verwaltetem Code ohne Stackwalk durchzuführen
        */

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(int idHook, keyboardHookProc callback, IntPtr hInstance, uint threadId);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
        private static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr idHook, int nCode, int wParam, ref keyboardHookStruct lParam);

        [SecurityCritical, SuppressUnmanagedCodeSecurity, DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        #endregion

        public delegate int keyboardHookProc(int code, int wParam, ref keyboardHookStruct lParam);

        public GlobalKeyboardHook()
        {
            khp = new keyboardHookProc(hookProc);
            hook();
        }


        ~GlobalKeyboardHook()
        {
            unhook();
        }

        public struct keyboardHookStruct
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }

        public event KeyEventHandler KeyDown;
        public event KeyEventHandler KeyUp;

        public void hook()
        {
            IntPtr hInstance = LoadLibrary("User32");
            hhook = SetWindowsHookEx(WH_KEYBOARD_LL, khp, hInstance, 0);
        }

        public void unhook()
        {
            UnhookWindowsHookEx(hhook);
        }

        public int hookProc(int code, int wParam, ref keyboardHookStruct lParam)
        {
            if (code >= 0)
            {
                Keys key = (Keys)lParam.vkCode;
                if (HookedKeys.Contains(key))
                {
                    KeyEventArgs kea = new KeyEventArgs(key);
                    if ((wParam == WM_KEYDOWN || wParam == WM_SYSKEYDOWN) && (KeyDown != null))
                    {
                        KeyDown(this, kea);
                    }
                    else if ((wParam == WM_KEYUP || wParam == WM_SYSKEYUP) && (KeyUp != null))
                    {
                        KeyUp(this, kea);
                    }
                    if (kea.Handled)
                        return 1;
                }
            }

            return (CallNextHookEx(hhook, code, wParam, ref lParam));
        }
    }
}