
//-----------------------------------------------------------------------
// <copyright file="WindowFromPoint.cs" company="Lifeprojects.de">
//     Class: WindowFromPoint
//     Copyright © Lifeprojects.de 2022
// </copyright>
//
// <author>Gerhard Ahrens - Lifeprojects.de</author>
// <email>developer@lifeprojects.de</email>
// <date>11.01.2022</date>
//
// <summary>
// Klasse zum aufrufen von Win API Funktionen
// </summary>
//-----------------------------------------------------------------------


namespace ColorPickerNET.Core
{
    using System;
    using System.Runtime.InteropServices;
    using System.Windows;
    using System.Windows.Interop;

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int x;
        public int y;

        public POINT(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    public class WindowFromCursor
    {
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(POINT point);

        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT lpPoint);

        static WindowFromCursor()
        {
        }

        public static Window GetWindowFromPoint(System.Drawing.Point point)
        {
            var hwnd = WindowFromPoint(new POINT((int)point.X, (int)point.Y));
            if (hwnd == IntPtr.Zero) return null;
            var p = GetParent(hwnd);
            while (p != IntPtr.Zero)
            {
                hwnd = p;
                p = GetParent(hwnd);
            }
            foreach (Window w in Application.Current.Windows)
            {
                if (w.IsVisible)
                {
                    var helper = new WindowInteropHelper(w);
                    if (helper.Handle == hwnd) return w;
                }
            }
            return null;
        }

        public static Window GetWindowFromMousePosition()
        {
            POINT p = new POINT();
            GetCursorPos(ref p);
            return GetWindowFromPoint(new System.Drawing.Point(p.x, p.y));
        }
    }
}
